namespace UI;
using Travelling;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

public partial class CalculateForm : Form
{
    private Traveler? traveler;
    private CityGraph? graph;
    private TextBox? travelerInfoTextBox;

    private Label? infoLabel;
    private TextBox? destinationTextBox;
    private Button? planButton;
    private Label? planResultLabel;

    private ListBox? routeListBox;
    private ListBox? citiesListBox;
    private Button? saveRouteButton;
    private Button? loadRouteButton;
    private Button? clearRouteButton;
    private Button? exitButton;

    private Button? okButton;
    private Button? cancelButton;

    private List<string>? currentPath;

    public CalculateForm(Traveler traveler, CityGraph cityGraph)
    {
        this.traveler = traveler;
        this.graph = cityGraph;
        SetupForm();
        PopulateCities();

        // --- Add form closing event handler ---
        this.FormClosing += CalculateForm_FormClosing;
    }

    private void SetupForm()
    {
        this.Text = "Traveler Planner";
        this.Size = new System.Drawing.Size(1280, 720);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 11);

        FlowLayoutPanel mainPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            AutoScroll = true
        };

        // --- Traveler info ---
        travelerInfoTextBox = new TextBox()
        {
            ReadOnly = true,
            Width = 800,
            Multiline = true,
            Height = 48,
            Text = traveler?.ToString() ?? string.Empty,
            Margin = new Padding(0, 6, 0, 0)
        };

        // --- Destination input ---
        FlowLayoutPanel planPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true
        };

        Label destLabel = new Label()
        {
            Text = "Destination:",
            AutoSize = true,
            Margin = new Padding(0, 6, 6, 0)
        };

        destinationTextBox = new TextBox() { Width = 250 };
        planButton = new Button()
        {
            Text = "Plan Route",
            AutoSize = true,
            Margin = new Padding(12, 0, 0, 0)
        };
        planButton.Click += PlanButton_Click;

        planResultLabel = new Label()
        {
            AutoSize = true,
            Margin = new Padding(12, 6, 0, 0)
        };

        planPanel.Controls.Add(destLabel);
        planPanel.Controls.Add(destinationTextBox);
        planPanel.Controls.Add(planButton);
        planPanel.Controls.Add(planResultLabel);

        // --- Content area: cities list + route + actions ---
        FlowLayoutPanel contentPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true
        };

        // Доступные города
        citiesListBox = new ListBox()
        {
            Width = 280,
            Height = 480,
            Margin = new Padding(0, 6, 12, 0)
        };
        citiesListBox.SelectedIndexChanged += CitiesListBox_SelectedIndexChanged;

        // Маршрут
        routeListBox = new ListBox()
        {
            Width = 420,
            Height = 480,
            Margin = new Padding(0, 6, 12, 0)
        };

        // Кнопки справа
        FlowLayoutPanel actionsPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.TopDown,
            AutoSize = true
        };

        saveRouteButton = new Button() { Text = "Save Route", AutoSize = true };
        loadRouteButton = new Button() { Text = "Load Route", AutoSize = true };
        clearRouteButton = new Button() { Text = "Clear Route", AutoSize = true };
        exitButton = new Button() { Text = "Exit", AutoSize = true, Margin = new Padding(0, 20, 0, 0) };

        saveRouteButton.Click += SaveRouteButton_Click;
        loadRouteButton.Click += LoadRouteButton_Click;
        clearRouteButton.Click += ClearRouteButton_Click;
        exitButton.Click += ExitButton_Click;

        actionsPanel.Controls.Add(saveRouteButton);
        actionsPanel.Controls.Add(loadRouteButton);
        actionsPanel.Controls.Add(clearRouteButton);
        actionsPanel.Controls.Add(exitButton);

        contentPanel.Controls.Add(citiesListBox);
        contentPanel.Controls.Add(routeListBox);
        contentPanel.Controls.Add(actionsPanel);

        // --- Bottom OK/Cancel buttons ---
        FlowLayoutPanel bottomPanel = new FlowLayoutPanel()
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Padding = new Padding(12),
            AutoSize = true
        };

        okButton = new Button()
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            AutoSize = true,
            Margin = new Padding(6, 0, 0, 0)
        };

        cancelButton = new Button()
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            AutoSize = true
        };

        bottomPanel.Controls.Add(okButton);
        bottomPanel.Controls.Add(cancelButton);

        this.Controls.Add(bottomPanel);

        // Устанавливаем Accept/Cancel кнопки для Enter/Esc
        this.AcceptButton = okButton;
        this.CancelButton = cancelButton;

        // --- Add all to main panel ---
        mainPanel.Controls.Add(travelerInfoTextBox);
        mainPanel.Controls.Add(planPanel);
        mainPanel.Controls.Add(contentPanel);

        this.Controls.Add(mainPanel);

        //CalculateForm_firstRoute();
    }

    private void PopulateCities()
    {
        if (graph == null || citiesListBox == null) return;
        citiesListBox.Items.Clear();
        foreach (var city in graph.GetCities())
            citiesListBox.Items.Add(city);
    }

    private void CitiesListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (citiesListBox?.SelectedItem != null)
            destinationTextBox!.Text = citiesListBox.SelectedItem.ToString()!;
    }

    private void PlanButton_Click(object? sender, EventArgs e)
    {
        string origin = traveler?.GetLocation() ?? string.Empty;
        string dest = destinationTextBox?.Text?.Trim() ?? string.Empty;

        if (graph == null)
        {
            MessageBox.Show("Graph data is not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(dest))
        {
            MessageBox.Show("Please enter both origin and destination.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var path = graph.FindShortestPath(origin, dest);
        if (path == null || path.Count == 0)
        {
            MessageBox.Show("No route found between the selected cities.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        currentPath = path;
        routeListBox?.Items.Clear();
        traveler?.ClearRoute();
        foreach (var city in path)
        {
            routeListBox?.Items.Add(city);
            traveler.AddCity(city);
        }
        travelerInfoTextBox!.Text = traveler.ToString();

        int distance = graph.GetPathDistance(path);
        planResultLabel!.Text = $"Total distance: {distance} km";
    }

    private void SaveRouteButton_Click(object? sender, EventArgs e)
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            MessageBox.Show("No route to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Route file|*.txt|All files|*.*";
        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            File.WriteAllLines(sfd.FileName, currentPath);
            MessageBox.Show("Route saved.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save route: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRouteButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Route file|*.txt|All files|*.*";
        if (ofd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var lines = File.ReadAllLines(ofd.FileName);
            currentPath = new List<string>(lines);
            routeListBox?.Items.Clear();
            foreach (var city in currentPath)
                routeListBox?.Items.Add(city);

            if (graph != null)
            {
                int d = graph.GetPathDistance(currentPath);
                planResultLabel!.Text = $"Total distance: {d} km";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load route: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearRouteButton_Click(object? sender, EventArgs e)
    {
        traveler?.ClearRoute();
        travelerInfoTextBox!.Text = traveler.ToString();
        currentPath = null;
        routeListBox?.Items.Clear();
        destinationTextBox!.Clear();
        planResultLabel!.Text = string.Empty;
    }

    private void ExitButton_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    // --- Confirm closing form ---
    private void CalculateForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Ask confirmation if user pressed Cancel or tried to close via X
        if (this.DialogResult != DialogResult.OK)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to close the route calculation form?\nUnsaved changes may be lost.",
                "Confirm Close",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Keep the form open
            }
        }
    }

    // private void CalculateForm_firstRoute()
    // {
    //     String destCity = traveler.GetLastStop() ?? string.Empty;
    //     //System.Diagnostics.Debug.WriteLine("First route to: " + destCity);
    //     if (!string.IsNullOrEmpty(destCity))
    //     {
    //         citiesListBox.SelectedIndex = citiesListBox.FindStringExact(destCity);
    //         destinationTextBox.Text = destCity;
    //         PlanButton_Click(this, EventArgs.Empty);

    //     }
    // }

}
