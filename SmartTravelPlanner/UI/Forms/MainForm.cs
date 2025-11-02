namespace UI;

using Travelling;
using UI.Helpers;

using System;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

public partial class MainForm : Form
{
    private TextBox nameTextBox;
    private TextBox cityTextBox;
    private TextBox mapTextBox;
    private Button mapSelectButton;
    private Button loadTravelerButton;
    private Button saveTravelerButton;
    private Button createTravelerButton;
    private bool isTravelerCreatedManually = false;
    private Traveler? loadedTraveler;
    private Button calculateRouteButton;
    private TableLayoutPanel routePanel;
    private Label routeLabel;
    private TextBox routeTextBox;
    private Button toggleRouteButton;

    public MainForm()
    {
        InitializeComponent();
        SetupForm();
    }

    private void SetupForm()
    {
        SetupWindow();
        var mainTable = BuildMainLayout();

        var jsonPanel = BuildJsonPanel();
        var mapPanel = BuildMapPanel();
        routePanel = BuildRoutePanel();

        PlaceControls(mainTable, jsonPanel, mapPanel, routePanel);
        this.Controls.Add(mainTable);
    }

    private void SetupWindow()
    {
        this.Text = "Smart Travel Planner";
        this.Size = new Size(1280, 720);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 11);

        this.FormClosing += (s, e) =>
        {
            if (!DialogHelper.Confirm("Are you sure you want to exit?"))
                e.Cancel = true;
        };
    }

    private TableLayoutPanel BuildMainLayout()
    {
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoSize = true,
            RowCount = 8,
            ColumnCount = 3
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));

        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        return table;
    }

// ----------------------------------------------------------------------------------------
// ----------------------------------------------------------------------------------------

    private TableLayoutPanel BuildJsonPanel()
    {
        var panel = new TableLayoutPanel
        {
            ColumnCount = 2,
            Dock = DockStyle.Fill
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

        loadTravelerButton = new Button { Text = "Load traveler data via JSON", Dock = DockStyle.Fill, Height = 50 };
        loadTravelerButton.Click += LoadJsonButton_Click;

        saveTravelerButton = new Button { Text = "Save traveler data to JSON", Dock = DockStyle.Fill, Height = 50 };
        saveTravelerButton.Click += SaveJsonButton_Click;

        panel.Controls.Add(loadTravelerButton, 0, 0);
        panel.Controls.Add(saveTravelerButton, 1, 0);

        return panel;
    }

    private void LoadJsonButton_Click(object sender, EventArgs e)
    {
        if (!FileHelper.TrySelectTravelerJson(out string jsonPath))
            return;

        try
        {
            loadedTraveler = Traveler.LoadFromFile(jsonPath);
            isTravelerCreatedManually = false;

            nameTextBox.Text = loadedTraveler.GetName();

            if (!string.IsNullOrWhiteSpace(loadedTraveler.GetLocation()))
            {
                cityTextBox.Text = loadedTraveler.GetLocation();
                cityTextBox.ForeColor = Color.Black;
            }

            if (loadedTraveler.route != null && loadedTraveler.route.Count > 0)
            {
                routeTextBox.Text = loadedTraveler.GetRoute();
                routePanel.Visible = true;
                toggleRouteButton.Text = "Hide Route";
            }
            else
            {
                routePanel.Visible = false;
                toggleRouteButton.Text = "Show Route";
            }
        }
        catch (FileNotFoundException)
        {
            DialogHelper.ShowError("File not found");
        }
        catch (FileLoadException ex)
        {
            DialogHelper.ShowError(ex.Message);
        }
        catch (JsonException)
        {
            DialogHelper.ShowError("Invalid JSON format");
        }
        catch (Exception ex)
        {
            DialogHelper.ShowError($"Unexpected error: {ex.Message}");
        }
    }
    private void SaveJsonButton_Click(object sender, EventArgs e)
    {
        if (loadedTraveler is null)
        {
            string name = nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                DialogHelper.ShowWarning("Cannot save empty traveler data!\nPlease fill in at least the Name field", "Empty Traveler Data");
                return;
            }
            loadedTraveler = new Traveler(name);
            string cityText = GetValidCityText(cityTextBox.Text);
            loadedTraveler.SetLocation(cityText);
        }

        if (string.IsNullOrEmpty(loadedTraveler.GetName()))
        {
            DialogHelper.ShowWarning("Cannot save traveler without a name!", "Invalid Data");
            return;
        }

        if (!FileHelper.TrySaveTravelerJson(out string jsonPath))
            return;

        try
        {
            loadedTraveler.SaveToFile(jsonPath);
            DialogHelper.ShowInfo("Traveler data saved successfully!", "Success");
        }
        catch (Exception ex)
        {
            DialogHelper.ShowError($"Error saving file: {ex.Message}");
        }
    }

// ----------------------------------------------------------------------------------------
// ----------------------------------------------------------------------------------------

    private TableLayoutPanel BuildMapPanel()
    {
        var panel = new TableLayoutPanel
        {
            ColumnCount = 2,
            Dock = DockStyle.Fill
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75f));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

        mapTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkGray,
            Text = "Select a map file to enable route calculation..."
        };
        mapTextBox.TextChanged += mapTextBox_TextChanged;
        mapTextBox.GotFocus += MapBox_GotFocus;
        mapTextBox.LostFocus += MapBox_LostFocus;

        mapSelectButton = new Button { Text = "Browse...", Dock = DockStyle.Fill };
        mapSelectButton.Click += MapSelectButton_Click;

        panel.Controls.Add(mapTextBox, 0, 0);
        panel.Controls.Add(mapSelectButton, 1, 0);
        return panel;
    }

    private void MapBox_GotFocus(object sender, EventArgs e)
    {
        if (mapTextBox.Text == "Select a map file to enable route calculation...")
        {
            mapTextBox.Text = "";
            mapTextBox.ForeColor = Color.Black;
        }
    }

    private void MapBox_LostFocus(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(mapTextBox.Text))
        {
            mapTextBox.Text = "Select a map file to enable route calculation...";
            mapTextBox.ForeColor = Color.Gray;
        }
    }

    private void MapSelectButton_Click(object sender, EventArgs e)
    {
        if (!FileHelper.TrySelectMapFile(mapTextBox, calculateRouteButton))
        {
            mapTextBox.Text = "Select a map file to enable route calculation...";
            mapTextBox.ForeColor = Color.DarkGray;
        }
    }

    void mapTextBox_TextChanged(object sender, EventArgs e)
    {
        if (mapTextBox.Text != "Select a map file to enable route calculation...")
        {
            FileHelper.ValidateMapFile(mapTextBox.Text, mapTextBox, calculateRouteButton, showDialogs: false);
        }
    }

    // ----------------------------------------------------------------------------------------
    // ----------------------------------------------------------------------------------------

    private TableLayoutPanel BuildRoutePanel()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Height = 50,
            Visible = false,
            ColumnCount = 3
        };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

        routeLabel = new Label { Text = "Route:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        routeTextBox = new TextBox { Dock = DockStyle.Fill, ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical };
        var editButton = new Button { Text = "Edit", Dock = DockStyle.Fill };
        editButton.Click += EditRouteButton_Click;

        panel.Controls.Add(routeLabel, 0, 0);
        panel.Controls.Add(routeTextBox, 1, 0);
        panel.Controls.Add(editButton, 2, 0);

        return panel;
    }

    private void EditRouteButton_Click(object sender, EventArgs e)
    {
        using (Form editForm = new Form())
        {
            editForm.Text = "Edit Route";
            editForm.Size = new Size(400, 300);
            editForm.StartPosition = FormStartPosition.CenterParent;

            TextBox editBox = new TextBox()
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Text = routeTextBox.Text.Replace(" -> ", Environment.NewLine)
            };

            Label instructionLabel = new Label()
            {
                Text = "Enter each city on a new line:",
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(5)
            };

            Button saveButton = new Button()
            {
                Text = "Save",
                Dock = DockStyle.Bottom,
                Height = 40
            };

            saveButton.Click += (s, ev) =>
            {
                if (loadedTraveler is null)
                {
                    string name = nameTextBox.Text.Trim();
                    string city = cityTextBox.Text.Trim();

                    if (string.IsNullOrEmpty(name))
                    {
                        DialogHelper.ShowWarning("Please fill in Name first!");
                        return;
                    }

                    loadedTraveler = new Traveler(name);
                    loadedTraveler.SetLocation(city);
                }

                loadedTraveler.ClearRoute();
                foreach (string city in editBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        try
                        {
                            loadedTraveler.AddCity(city.Trim());
                        }
                        catch (Exception ex)
                        {
                            DialogHelper.ShowError($"Invalid city name: {city}\n{ex.Message}");
                            return;
                        }
                    }
                }

                routeTextBox.Text = loadedTraveler.GetRoute();
                routePanel.Visible = true;
                toggleRouteButton.Text = "Hide Route";

                editForm.Close();
            };

            editForm.Controls.Add(editBox);
            editForm.Controls.Add(instructionLabel);
            editForm.Controls.Add(saveButton);

            editForm.ShowDialog(this);
        }
    }

    // ----------------------------------------------------------------------------------------
    // ----------------------------------------------------------------------------------------

    private void PlaceControls(TableLayoutPanel table, TableLayoutPanel jsonPanel, TableLayoutPanel mapPanel, TableLayoutPanel routePanel)
    {
        table.Controls.Add(new Label { Text = "Name *:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, ForeColor = Color.DarkRed }, 0, 0);
        nameTextBox = new TextBox { Dock = DockStyle.Fill };
        table.Controls.Add(nameTextBox, 1, 0);

        table.Controls.Add(new Label { Text = "Start city *?:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, ForeColor = Color.DarkGoldenrod }, 0, 1);
        cityTextBox = new TextBox { Dock = DockStyle.Fill };

        cityTextBox.Text = "Leave empty to use the first city from the route";
        cityTextBox.ForeColor = Color.DarkGray;

        cityTextBox.GotFocus += (s, e) =>
        {
            if (cityTextBox.Text == "Leave empty to use the first city from the route")
            {
                cityTextBox.Text = "";
                cityTextBox.ForeColor = Color.Black;
            }
        };
        cityTextBox.LostFocus += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(cityTextBox.Text))
            {
                cityTextBox.Text = "Leave empty to use the first city from the route";
                cityTextBox.ForeColor = Color.DarkGray;
            }
        };
        table.Controls.Add(cityTextBox, 1, 1);

        table.Controls.Add(jsonPanel, 1, 2);

        toggleRouteButton = new Button { Text = "Show Route", Dock = DockStyle.Left, Width = 160 };
        toggleRouteButton.Click += ToggleRouteButton_Click;
        table.Controls.Add(toggleRouteButton, 1, 3);

        table.Controls.Add(routePanel, 1, 4);

        table.Controls.Add(new Label { Text = "Map file (.txt):", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 5);
        table.Controls.Add(mapPanel, 1, 5);

        var actionPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2
        };
        actionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
        actionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

        createTravelerButton = new Button
        {
            Text = "Create Traveler",
            Dock = DockStyle.Fill,
            BackColor = Color.LightGray
        };
        createTravelerButton.Click += CreateTravelerButton_Click;

        calculateRouteButton = new Button
        {
            Text = "Calculate Route",
            Dock = DockStyle.Fill,
            Enabled = false
        };
        calculateRouteButton.Click += CalculateRouteButton_Click;

        actionPanel.Controls.Add(createTravelerButton, 0, 0);
        actionPanel.Controls.Add(calculateRouteButton, 1, 0);

        table.Controls.Add(actionPanel, 1, 6);

        Button exitButton = new Button
        {
            Text = "Exit",
            Dock = DockStyle.Fill,
            Height = 40,
        };
        exitButton.Click += (s, e) =>
        {
            this.Close();
        };
        table.Controls.Add(exitButton, 1, 8);
    }
    
    private async void CreateTravelerButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string city = GetValidCityText(cityTextBox.Text);

        if (string.IsNullOrWhiteSpace(name))
        {
            DialogHelper.ShowWarning("Please enter a Name first!");
            return;
        }

        loadedTraveler = new Traveler(name);

        isTravelerCreatedManually = true;

        if (!string.IsNullOrWhiteSpace(city))
            loadedTraveler.SetLocation(city);

        if (!string.IsNullOrWhiteSpace(routeTextBox.Text))
        {
            var cities = routeTextBox.Text.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in cities)
            {
                loadedTraveler.AddCity(c.Trim());
            }
        }

        var originalText = createTravelerButton.Text;
        var originalColor = createTravelerButton.BackColor;

        createTravelerButton.Text = "Traveler Created!";
        createTravelerButton.BackColor = Color.LightGreen;
        createTravelerButton.Enabled = false;

        await Task.Delay(3000);

        createTravelerButton.Text = originalText;
        createTravelerButton.BackColor = originalColor;
        createTravelerButton.Enabled = true;
    }


    private void ToggleRouteButton_Click(object sender, EventArgs e)
    {
        routePanel.Visible = !routePanel.Visible;
        toggleRouteButton.Text = routePanel.Visible ? "Hide Route" : "Show Route";
    }

    private void CalculateRouteButton_Click(object sender, EventArgs e)
    {
        if (!isTravelerCreatedManually)
        {
            DialogHelper.ShowWarning("Please create a traveler first using the 'Create Traveler' button!");
            return;
        }
        if(!ValidateCityGraphFile(mapTextBox.Text))
        {
            DialogHelper.ShowError("The selected map file is invalid. Please correct the file and try again.");
            return;
        }

        string name = nameTextBox.Text.Trim();
        string city = GetValidCityText(cityTextBox.Text);
        string mapFile = mapTextBox.Text.Trim();

        if (!ValidateInputs(name, city, mapFile))
            return;

        try
        {
            CityGraph graph = CityGraph.LoadFromFile(mapFile);

            if (loadedTraveler is null)
            {
                loadedTraveler = new Traveler(name);
            }
            loadedTraveler.SetLocation(city);

            using (CalculateForm calcForm = new CalculateForm(loadedTraveler, graph))
            {
                if (calcForm.ShowDialog() == DialogResult.OK)
                {
                    if (loadedTraveler is not null && loadedTraveler.route is not null && loadedTraveler.route.Count > 0)
                    {
                        routeTextBox.Text = loadedTraveler.GetRoute();
                        routePanel.Visible = true;
                        toggleRouteButton.Text = "Hide Route";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DialogHelper.ShowError($"Error loading map or calculating route: {ex.Message}");
        }
    }

    private bool ValidateInputs(string name, string city, string mapFile)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(city))
        {
            DialogHelper.ShowWarning("Please fill in Name and City first!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            DialogHelper.ShowWarning("Please fill in Name first!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            if (!(loadedTraveler?.route?.Count > 0))
            {
                DialogHelper.ShowWarning("Please fill in City first or enter one city in route!");
                return false;
            }
        }

        if (!FileHelper.ValidateMapFile(mapFile, mapTextBox, calculateRouteButton))
        {
            return false;
        }

        return true;
    }
    private string GetValidCityText(string cityText)
    {
        if (cityText == "Leave empty to use the first city from the route")
            cityText = string.Empty;

        if (!string.IsNullOrWhiteSpace(cityText))
        return cityText.Trim();

        if (loadedTraveler?.route != null && loadedTraveler.route.Count > 0)
        {
            string firstCity = loadedTraveler.route[0];
            cityTextBox.Text = firstCity;
            cityTextBox.ForeColor = Color.Black;
            return firstCity;
        }

        return string.Empty;
    }

    private static bool ValidateCityGraphFile(string filePath)
{
    string[] lines = File.ReadAllLines(filePath);
    HashSet<string> seenRoutes = new HashSet<string>();

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                //throw new InvalidDataException("File contains empty or whitespace line.");
                return false;
            }

            string[] parts = line.Split(',');
            if (parts.Length != 2) {
                //throw new InvalidDataException($"Invalid line format (missing comma): '{line}'");
                return false;
            }

            string[] cities = parts[0].Split('-');
            if (cities.Length != 2) {
                //throw new InvalidDataException($"Invalid city pair format (expected 'CityA-CityB'): '{line}'");
                return false;
            }

            string cityA = cities[0].Trim();
            string cityB = cities[1].Trim();
            if (string.IsNullOrEmpty(cityA) || string.IsNullOrEmpty(cityB)) {
                //throw new InvalidDataException($"One of the city names is empty: '{line}'");
                return false;
            }

            if (!int.TryParse(parts[1].Trim(), out int distance) || distance <= 0) {
                //throw new InvalidDataException($"Invalid distance value: '{line}'");
                return false;
            }

            // Detect duplicates (case-insensitive, ignoring order)
            string routeKey = string.Compare(cityA, cityB, StringComparison.OrdinalIgnoreCase) < 0
                ? $"{cityA}-{cityB}".ToLower()
                : $"{cityB}-{cityA}".ToLower();

            if (!seenRoutes.Add(routeKey))
            {
                //throw new InvalidDataException($"Duplicate route detected: {cityA}-{cityB}");
                return false;
            }
        }
    return true;
}

}