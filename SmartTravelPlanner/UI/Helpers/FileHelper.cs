namespace UI.Helpers;

using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

public static class FileHelper
{
    public static bool ValidateMapFile(string filePath, TextBox textBox, Button calculateButton, bool showDialogs = true)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            SetInvalidState(textBox, calculateButton);
            if (showDialogs)
                DialogHelper.ShowWarning("Map file path is empty!");
            return false;
        }

        filePath = filePath.Trim();

        if (!File.Exists(filePath))
        {
            SetInvalidState(textBox, calculateButton);
            if (showDialogs)
                DialogHelper.ShowWarning("Map file not found!");
            return false;
        }

        if (Path.GetExtension(filePath).ToLower() != ".txt")
        {
            SetInvalidState(textBox, calculateButton);
            if (showDialogs)
                DialogHelper.ShowWarning("Invalid file format! Please select a .txt file.");
            return false;
        }

        try
        {
            File.ReadAllText(filePath);
            SetValidState(textBox, calculateButton);
            return true;
        }
        catch (Exception ex)
        {
            SetInvalidState(textBox, calculateButton);
            if (showDialogs)
                DialogHelper.ShowError($"Error accessing map file: {ex.Message}");
            return false;
        }
    }

    public static string OpenFile(string filter, string title)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Filter = filter;
            dialog.Title = title;
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }
    }

    public static string SaveFile(string filter, string title, string defaultExt)
    {
        using (SaveFileDialog dialog = new SaveFileDialog())
        {
            dialog.Filter = filter;
            dialog.Title = title;
            dialog.DefaultExt = defaultExt;
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : string.Empty;
        }
    }

    public static void SetValidState(TextBox textBox, Button? actionButton = null)
    {
        textBox.ForeColor = Color.Green;
        if (actionButton != null) actionButton.Enabled = true;
    }

    public static void SetInvalidState(TextBox textBox, Button? actionButton = null)
    {
        textBox.ForeColor = Color.Red;
        if (actionButton != null) actionButton.Enabled = false;
    }

    public static bool TrySelectMapFile(TextBox mapTextBox, Button calculateButton)
    {
        string filePath = OpenFile("Text Files (*.txt)|*.txt", "Select Map File");
        if (string.IsNullOrEmpty(filePath))
            return false;

        mapTextBox.Text = filePath;
        return ValidateMapFile(filePath, mapTextBox, calculateButton);
    }

    public static bool TrySelectTravelerJson(out string filePath)
    {
        filePath = OpenFile("JSON Files (*.json)|*.json", "Select Traveler Data");
        return !string.IsNullOrEmpty(filePath);
    }

    public static bool TrySaveTravelerJson(out string filePath)
    {
        filePath = SaveFile("JSON Files (*.json)|*.json", "Save Traveler Data", "json");
        return !string.IsNullOrEmpty(filePath);
    }
}
