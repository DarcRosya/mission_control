namespace UI;
using Travelling;
using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
    // Поля для хранения введённых данных
    private TextBox nameTextBox;
    private TextBox cityTextBox;
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
        this.Size = new System.Drawing.Size(1280, 720);

        // Label и TextBox для имени
        Label nameLabel = new Label();
        nameLabel.Text = "Name:";
        nameLabel.Location = new System.Drawing.Point(30, 20); // X AND Y
        nameLabel.Size = new System.Drawing.Size(63, 25); // WIDE AND HEIGHT (25 is maybe minimum when text starts erasing)
        this.Controls.Add(nameLabel);

        nameTextBox = new TextBox();
        nameTextBox.Location = new System.Drawing.Point(100, 20);
        nameTextBox.Width = 200;
        this.Controls.Add(nameTextBox);

        // Label и TextBox для города
        Label cityLabel = new Label();
        cityLabel.Text = "City:";
        cityLabel.Location = new System.Drawing.Point(40, 60);
        cityLabel.Size = new System.Drawing.Size(50, 25);
        this.Controls.Add(cityLabel);

        cityTextBox = new TextBox();
        cityTextBox.Location = new System.Drawing.Point(100, 60);
        cityTextBox.Width = 200;
        this.Controls.Add(cityTextBox);

        // Кнопка Submit
        submitButton = new Button();
        submitButton.Text = "Submit";
        submitButton.Location = new System.Drawing.Point(144, 96);
        submitButton.Size = new System.Drawing.Size(85, 36);
        submitButton.Click += SubmitButton_Click;
        this.Controls.Add(submitButton);

        // Label для результата
        resultLabel = new Label();
        resultLabel.Location = new System.Drawing.Point(20, 150);
        resultLabel.Width = 350;
        resultLabel.Height = 50;
        this.Controls.Add(resultLabel);
    }

    private void SubmitButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string currentCity = cityTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(currentCity))
        {
            MessageBox.Show("Please enter both Name and City.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Создаём объект класса Traveler
        Traveler traveler = new Traveler(name);

        traveler.SetLocation(currentCity);

        // Показываем информацию
        resultLabel.Text = traveler.ToString();
    }
}
