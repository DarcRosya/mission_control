namespace UI
{
    using Travelling;
    using System;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private Traveler traveler;
        private TextBox nameTextBox;
        private TextBox cityTextBox;
        private Button submitButton;
        private Button newPageButton;
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
            this.StartPosition = FormStartPosition.CenterScreen;

            // === Основная панель ===
            FlowLayoutPanel mainPanel = new FlowLayoutPanel();
            mainPanel.FlowDirection = FlowDirection.TopDown;
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);
            mainPanel.WrapContents = false;
            mainPanel.AutoScroll = true; // если контента много

            // === Контейнер для поля Name ===
            FlowLayoutPanel namePanel = new FlowLayoutPanel();
            namePanel.FlowDirection = FlowDirection.LeftToRight;
            namePanel.AutoSize = true;
            namePanel.WrapContents = false;
            namePanel.Margin = new Padding(0, 0, 0, 10);

            Label nameLabel = new Label();
            nameLabel.Text = "Name:";
            nameLabel.AutoSize = true;
            nameLabel.Margin = new Padding(0, 5, 10, 0);

            nameTextBox = new TextBox();
            nameTextBox.Width = 200;

            namePanel.Controls.Add(nameLabel);
            namePanel.Controls.Add(nameTextBox);

            // === Контейнер для поля City ===
            FlowLayoutPanel cityPanel = new FlowLayoutPanel();
            cityPanel.FlowDirection = FlowDirection.LeftToRight;
            cityPanel.AutoSize = true;
            cityPanel.WrapContents = false;
            cityPanel.Margin = new Padding(0, 0, 0, 10);

            Label cityLabel = new Label();
            cityLabel.Text = "City:";
            cityLabel.AutoSize = true;
            cityLabel.Margin = new Padding(0, 5, 10, 0);

            cityTextBox = new TextBox();
            cityTextBox.Width = 200;

            cityPanel.Controls.Add(cityLabel);
            cityPanel.Controls.Add(cityTextBox);

            // === Кнопки ===
            submitButton = new Button();
            submitButton.Text = "Submit";
            submitButton.AutoSize = true;
            submitButton.Margin = new Padding(0, 10, 10, 10);
            submitButton.Click += SubmitButton_Click;

            newPageButton = new Button();
            newPageButton.Text = "New Page";
            newPageButton.AutoSize = true;
            newPageButton.Margin = new Padding(10, 10, 0, 10);
            newPageButton.Click += NewPageButton_Click;

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            buttonPanel.AutoSize = true;
            buttonPanel.Controls.Add(submitButton);
            buttonPanel.Controls.Add(newPageButton);

            // === Label для результата ===
            resultLabel = new Label();
            resultLabel.AutoSize = true;
            resultLabel.MaximumSize = new System.Drawing.Size(600, 0);
            resultLabel.Margin = new Padding(0, 10, 0, 0);

            // === Добавляем всё в основную панель ===
            mainPanel.Controls.Add(namePanel);
            mainPanel.Controls.Add(cityPanel);
            mainPanel.Controls.Add(buttonPanel);
            mainPanel.Controls.Add(resultLabel);

            // === Добавляем основную панель на форму ===
            this.Controls.Add(mainPanel);
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
            traveler = new Traveler(name);
            traveler.SetLocation(currentCity);
            // Показываем информацию
            resultLabel.Text = traveler.ToString();
        }

        private void NewPageButton_Click(object sender, EventArgs e)
        {
            // Переход на вторую форму
            SecondForm secondForm = new SecondForm(traveler);
            secondForm.Show();
            this.Hide(); // прячем текущую форму
        }
    }
}
