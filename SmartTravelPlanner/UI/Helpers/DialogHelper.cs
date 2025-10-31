namespace UI.Helpers;

using System.Windows.Forms;

public static class DialogHelper
{
    public static void ShowWarning(string message, string title = "Warning")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowInfo(string message, string title = "Information")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static bool Confirm(string message, string title = "Confirmation")
    {
        return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}