namespace UI;
using Travelling;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class SecondForm : Form
{
    private Traveler? traveler;
    private TextBox? travelerInfoTextBox;

    private Label? infoLabel;
    private Button? loadMapButton;
    private TextBox? loadStatusTextBox;
    private TextBox? destinationTextBox;
    private Button? planButton;
    private Label? planResultLabel;

    private ListBox? routeListBox;
    private Button? saveRouteButton;
    private Button? loadRouteButton;
    private Button? clearRouteButton;
    private Button? exitButton;

    private CityGraph? currentGraph;
    private List<string>? currentPath;

    public SecondForm(Traveler traveler)
    {
        this.traveler = traveler;
        SetupForm();
    }
    private void SetupForm()
    {
        this.Text = "Traveler Planner";
        this.Size = new System.Drawing.Size(1100, 700);

        FlowLayoutPanel mainPanel = new FlowLayoutPanel();
        mainPanel.FlowDirection = FlowDirection.TopDown;
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Padding = new Padding(12);
        mainPanel.AutoScroll = true;

        // Traveler input row
        FlowLayoutPanel travelerPanel = new FlowLayoutPanel();
        travelerPanel.FlowDirection = FlowDirection.LeftToRight;
        travelerPanel.AutoSize = true;

    // Single traveler info textbox (shows name and current location)
    travelerInfoTextBox = new TextBox() { ReadOnly = true, Width = 600, Multiline = true, Height = 48 };
    travelerInfoTextBox.Text = traveler?.ToString() ?? string.Empty;
    travelerInfoTextBox.Margin = new Padding(0, 6, 0, 0);
    travelerPanel.Controls.Add(travelerInfoTextBox);

        // Map load row
        FlowLayoutPanel mapPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        loadMapButton = new Button() { Text = "Load Map (.txt)", AutoSize = true };
        loadMapButton.Click += LoadMapButton_Click;
        loadStatusTextBox = new TextBox() { ReadOnly = true, Width = 520, Margin = new Padding(8, 6, 0, 0) };
        mapPanel.Controls.Add(loadMapButton);
        mapPanel.Controls.Add(loadStatusTextBox);

        // Destination / Plan row
        FlowLayoutPanel planPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        Label destLabel = new Label() { Text = "Destination:", AutoSize = true, Margin = new Padding(0,6,6,0) };
        destinationTextBox = new TextBox() { Width = 220 };
        planButton = new Button() { Text = "Plan Route", AutoSize = true, Margin = new Padding(12,0,0,0) };
        planButton.Click += PlanButton_Click;
        planResultLabel = new Label() { AutoSize = true, Margin = new Padding(12,6,0,0) };
        planPanel.Controls.Add(destLabel);
        planPanel.Controls.Add(destinationTextBox);
        planPanel.Controls.Add(planButton);
        planPanel.Controls.Add(planResultLabel);

        // Route list and actions
        FlowLayoutPanel contentPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };

        routeListBox = new ListBox() { Width = 420, Height = 360, Margin = new Padding(0,6,12,0) };

        FlowLayoutPanel actionsPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.TopDown, AutoSize = true };
        saveRouteButton = new Button() { Text = "Save Route", AutoSize = true, Margin = new Padding(0,0,0,6) };
        saveRouteButton.Click += SaveRouteButton_Click;
        loadRouteButton = new Button() { Text = "Load Route", AutoSize = true, Margin = new Padding(0,0,0,6) };
        loadRouteButton.Click += LoadRouteButton_Click;
        clearRouteButton = new Button() { Text = "Clear Route", AutoSize = true, Margin = new Padding(0,0,0,6) };
        clearRouteButton.Click += ClearRouteButton_Click;
        exitButton = new Button() { Text = "Exit", AutoSize = true, Margin = new Padding(0,20,0,0) };
        exitButton.Click += ExitButton_Click;

        actionsPanel.Controls.Add(saveRouteButton);
        actionsPanel.Controls.Add(loadRouteButton);
        actionsPanel.Controls.Add(clearRouteButton);
        actionsPanel.Controls.Add(exitButton);

        contentPanel.Controls.Add(routeListBox);
        contentPanel.Controls.Add(actionsPanel);

        // Add everything to main panel
        mainPanel.Controls.Add(travelerPanel);
        mainPanel.Controls.Add(mapPanel);
        mainPanel.Controls.Add(planPanel);
        mainPanel.Controls.Add(contentPanel);

        this.Controls.Add(mainPanel);
    }
    // Removed Submit button - traveler info is shown in the readonly info textbox

    private void LoadMapButton_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Text files|*.txt|All files|*.*";
        if (ofd.ShowDialog() != DialogResult.OK) return;

        try
        {
            currentGraph = CityGraph.LoadFromFile(ofd.FileName);
            if (loadStatusTextBox != null) loadStatusTextBox.Text = $"Loaded: {Path.GetFileName(ofd.FileName)}";
            MessageBox.Show("Map loaded successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load map: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (loadStatusTextBox != null) loadStatusTextBox.Text = "Load failed";
            currentGraph = null;
        }
    }

    private void PlanButton_Click(object? sender, EventArgs e)
    {
        string origin = traveler?.GetLocation() ?? string.Empty;
        string dest = destinationTextBox?.Text?.Trim() ?? string.Empty;

        if (currentGraph == null)
        {
            MessageBox.Show("Please load a map first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(dest))
        {
            MessageBox.Show("Please enter both origin and destination.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var path = currentGraph.FindShortestPath(origin, dest);
        if (path == null || path.Count == 0)
        {
            MessageBox.Show("No route found between the selected cities.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        currentPath = path;
        routeListBox?.Items.Clear();
        foreach (var city in path)
        {
            routeListBox?.Items.Add(city);
        }

        int distance = currentGraph.GetPathDistance(path);
        if (planResultLabel != null) planResultLabel.Text = $"Total distance: {distance}";
    }

    private void SaveRouteButton_Click(object? sender, EventArgs e)
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            MessageBox.Show("No route to save.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Route file|*.route.txt|Text file|*.txt|All|*.*";
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
        ofd.Filter = "Route file|*.route.txt|Text file|*.txt|All|*.*";
        if (ofd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var lines = File.ReadAllLines(ofd.FileName);
            currentPath = new List<string>(lines);
            routeListBox?.Items.Clear();
            foreach (var city in currentPath) routeListBox?.Items.Add(city);

            if (currentGraph != null)
            {
                int d = currentGraph.GetPathDistance(currentPath);
                if (planResultLabel != null) planResultLabel.Text = $"Total distance: {d}";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load route: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearRouteButton_Click(object? sender, EventArgs e)
    {
        currentPath = null;
        routeListBox?.Items.Clear();
        if (planResultLabel != null) planResultLabel.Text = string.Empty;
    }

    private void ExitButton_Click(object? sender, EventArgs e)
    {
        this.Close();
    }

    
}
