namespace UI;
using Travelling;
using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
    private TextBox nameTextBox;
    private TextBox cityTextBox;
    private TextBox mapTextBox;
    private Button submitButton;
    private Button loadTravelerButton;
    private Button mapSelectButton;
    private Button calculateRouteButton;
    private Label resultLabel;
    private TableLayoutPanel routePanel;
    private Label routeLabel;
    private TextBox routeTextBox;
    private Button toggleRouteButton;
    private Traveler? loadedTraveler;

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
        }

    private void SetupForm()
    {
        this.Text = "Traveler Form";
        this.Size = new System.Drawing.Size(1280, 720);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 11);

        TableLayoutPanel table = new TableLayoutPanel();
        table.RowCount = 7;
        table.ColumnCount = 2;
        table.Dock = DockStyle.Fill;
        table.Padding = new Padding(20);
        table.AutoSize = true;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // toggleRouteButton
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // routePanel
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        Label nameLabel = new Label() { Text = "Name *:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill, ForeColor = Color.DarkRed };
        nameTextBox = new TextBox() { Dock = DockStyle.Fill };

        Label cityLabel = new Label() { Text = "Start city (optional):", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
        cityTextBox = new TextBox() { Dock = DockStyle.Fill };

        Label mapLabel = new Label() { Text = "Map file (.txt):", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };

        loadTravelerButton = new Button()
        {
            Text = "Load traveler data via json",
            Dock = DockStyle.Fill,
            Height = 50
        };
        loadTravelerButton.Click += TravelerSelectButton_Click;

        TableLayoutPanel mapPanel = new TableLayoutPanel();
        mapPanel.ColumnCount = 2;
        mapPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75f));
        mapPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        mapPanel.Dock = DockStyle.Fill;
        mapPanel.Margin = new Padding(0);

        mapTextBox = new TextBox() 
        { 
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkGray,
            Text = "Select a map file to enable route calculation..."
        };

        mapTextBox.GotFocus += (s, e) => {
            if (mapTextBox.Text == "Select a map file to enable route calculation...")
            {
                mapTextBox.Text = "";
                mapTextBox.ForeColor = Color.Black;
            }
        };
        mapTextBox.LostFocus += (s, e) => {
            if (string.IsNullOrWhiteSpace(mapTextBox.Text))
            {
                mapTextBox.Text = "Select a map file to enable route calculation...";
                mapTextBox.ForeColor = Color.DarkGray;
            }
        };

        mapSelectButton = new Button();
        mapSelectButton.Text = "Browse...";
        mapSelectButton.Dock = DockStyle.Fill;
        mapSelectButton.Click += MapSelectButton_Click;

        mapPanel.Controls.Add(mapTextBox, 0, 0);
        mapPanel.Controls.Add(mapSelectButton, 1, 0);

        toggleRouteButton = new Button()
        {
            Text = "Show Route",
            Dock = DockStyle.Left,
            Height = 30,
            Width = 160
        };
        toggleRouteButton.Click += ToggleRouteButton_Click;

        TableLayoutPanel routeTable = new TableLayoutPanel()
        {
            Dock = DockStyle.Fill,
            Height = 50,
            Visible = false,
            ColumnCount = 3,
            Margin = new Padding(0)
        };
        routePanel = routeTable;
        ((TableLayoutPanel)routePanel).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
        ((TableLayoutPanel)routePanel).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        ((TableLayoutPanel)routePanel).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

        routeLabel = new Label()
        {
            Text = "Route:",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        routeTextBox = new TextBox()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };

        Button editRouteButton = new Button()
        {
            Text = "Edit",
            Dock = DockStyle.Fill,
            Height = 30
        };
        editRouteButton.Click += EditRouteButton_Click;
        
        routeTable.Controls.Add(routeLabel, 0, 0);
        routeTable.Controls.Add(routeTextBox, 1, 0);
        routeTable.Controls.Add(editRouteButton, 2, 0);

        submitButton = new Button()
        {
            Text = "Submit",
            Dock = DockStyle.Fill,
            Height = 40
        };
        submitButton.Click += SubmitButton_Click;

        resultLabel = new Label()
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.DarkBlue,
            AutoSize = true,
            MaximumSize = new System.Drawing.Size(500, 0)
        };

        table.Controls.Add(nameLabel, 0, 0);
        table.Controls.Add(nameTextBox, 1, 0);

        table.Controls.Add(cityLabel, 0, 1);
        table.Controls.Add(cityTextBox, 1, 1);

        table.Controls.Add(loadTravelerButton, 1, 2);

        table.Controls.Add(toggleRouteButton, 1, 3);
        table.Controls.Add(routePanel, 1, 4);

        table.Controls.Add(mapLabel, 0, 5);
        table.Controls.Add(mapPanel, 1, 5);

        calculateRouteButton = new Button()
        {
            Text = "Calculate Route",
            Dock = DockStyle.Fill,
            Height = 40,
            Enabled = false 
        };
        calculateRouteButton.Click += CalculateRouteButton_Click;

        TableLayoutPanel buttonPanel = new TableLayoutPanel()
        {
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };
        buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        buttonPanel.Controls.Add(submitButton, 0, 0);
        buttonPanel.Controls.Add(calculateRouteButton, 1, 0);

        table.Controls.Add(buttonPanel, 1, 6);
        table.Controls.Add(resultLabel, 0, 7);
        table.SetColumnSpan(resultLabel, 2);

        this.Controls.Add(table);
    }

    private void TravelerSelectButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = "Json Files (*.json)|*.json";
            dialog.Title = "Select Traveler Data";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    loadedTraveler = Traveler.LoadFromFile(dialog.FileName);
                    nameTextBox.Text = loadedTraveler.GetName();
                    cityTextBox.Text = loadedTraveler.GetLocation();
                    
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
                catch (Exception)
                {
                    MessageBox.Show("Invalid JSON format!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    private void ToggleRouteButton_Click(object sender, EventArgs e)
    {
        routePanel.Visible = !routePanel.Visible;
        toggleRouteButton.Text = routePanel.Visible ? "Hide Route" : "Show Route";
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
                        MessageBox.Show("Please fill in Name first!", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            MessageBox.Show($"Invalid city name: {city}\n{ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    private void MapSelectButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.Title = "Select Map File";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!File.Exists(dialog.FileName))
                    {
                        throw new FileNotFoundException("The selected map file does not exist.");
                    }

                    mapTextBox.Text = dialog.FileName;
                    mapTextBox.ForeColor = Color.Black;
                    calculateRouteButton.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading map file: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    calculateRouteButton.Enabled = false;
                    
                    mapTextBox.Text = "Select a map file to enable route calculation...";
                    mapTextBox.ForeColor = Color.DarkGray;
                }
            }
        }
    }

    private void CalculateRouteButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string mapFile = mapTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Please enter traveler's name first!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!File.Exists(mapFile))
        {
            MessageBox.Show("Please select a valid map file first!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            CityGraph graph = CityGraph.LoadFromFile(mapFile);

            if (loadedTraveler is null)
            {
                loadedTraveler = new Traveler(name);
                if (!string.IsNullOrEmpty(cityTextBox.Text))
                {
                    loadedTraveler.SetLocation(cityTextBox.Text.Trim());
                }
            }

            using (CalculateForm calcForm = new CalculateForm(loadedTraveler, graph))
            {
                if (calcForm.ShowDialog() == DialogResult.OK)
                {
                    if (loadedTraveler.route != null && loadedTraveler.route.Count > 0)
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
            MessageBox.Show($"Error loading map or calculating route: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string currentCity = cityTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Please fill in the Name field!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Traveler currentTraveler;
        if (loadedTraveler is not null)
        {
            loadedTraveler.name = name;
            loadedTraveler.SetLocation(currentCity);
            currentTraveler = loadedTraveler;
        }
        else
        {
            currentTraveler = new Traveler(name);
            currentTraveler.SetLocation(currentCity);
        }

        resultLabel.Text = "Your travel data:\n" + currentTraveler.ToString();
    }
}
