namespace UI;
using Travelling;
using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
    // Поля для хранения введённых данных
    private TextBox nameTextBox;
    private TextBox cityTextBox;
    private TextBox mapTextBox;
    private Button submitButton;
    private Label resultLabel;

    public MainForm()
    {
        InitializeComponent();
        SetupForm();
    }

    private void SetupForm()
    {
        this.Text = "Traveler Form";
        this.Size = new System.Drawing.Size(820, 550);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 11);

        TableLayoutPanel table = new TableLayoutPanel();
        table.RowCount = 4;
        table.ColumnCount = 2;
        table.Dock = DockStyle.Fill;
        table.Padding = new Padding(20);
        table.AutoSize = true;
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35f));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65f));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        Label nameLabel = new Label() { Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
        nameTextBox = new TextBox() { Dock = DockStyle.Fill };

        Label cityLabel = new Label() { Text = "Start city:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
        cityTextBox = new TextBox() { Dock = DockStyle.Fill };

        Label mapLabel = new Label() { Text = "Map file (.txt):", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };

        TableLayoutPanel mapPanel = new TableLayoutPanel();
        mapPanel.ColumnCount = 2;
        mapPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75f));
        mapPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
        mapPanel.Dock = DockStyle.Fill;
        mapPanel.Margin = new Padding(0);

        mapTextBox = new TextBox() { Dock = DockStyle.Fill };

        Button mapSelectButton = new Button();
        mapSelectButton.Text = "Browse...";
        mapSelectButton.Dock = DockStyle.Fill;
        mapSelectButton.Click += MapSelectButton_Click;

        mapPanel.Controls.Add(mapTextBox, 0, 0);
        mapPanel.Controls.Add(mapSelectButton, 1, 0);

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

        table.Controls.Add(mapLabel, 0, 2);
        table.Controls.Add(mapPanel, 1, 2);

        table.Controls.Add(submitButton, 1, 3);
        table.Controls.Add(resultLabel, 0, 4);
        table.SetColumnSpan(resultLabel, 2);

        this.Controls.Add(table);
    }

    private void MapSelectButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = "Text Files (*.txt)|*.txt";
            dialog.Title = "Select Map File";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mapTextBox.Text = dialog.FileName;
            }
        }
    }
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string currentCity = cityTextBox.Text.Trim();
        string mapFile = mapTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name) ||
            string.IsNullOrEmpty(currentCity) ||
            string.IsNullOrEmpty(mapFile))
        {
            MessageBox.Show("Please fill all fields!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!File.Exists(mapFile))
        {
            MessageBox.Show("Map file does not exist!", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Создаём объект класса Traveler
        Traveler traveler = new Traveler(name);
        traveler.SetLocation(currentCity);

        CityGraph graph = CityGraph.LoadFromFile(mapTextBox.Text);
        // Показываем информацию
        resultLabel.Text = traveler.ToString();
    }
}
