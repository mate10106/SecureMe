using SecureMe.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SecureMe.Views
{
    public partial class DetailsPasswordWindow : Window
    {
        public DetailsPasswordWindow(Passwords.PasswordEntry passwordEntry)
        {
            InitializeComponent();

            txtUsername.Content = passwordEntry.Username;
            txtPassword.Password = SecureMe.Utilities.FileManager.DecryptData(passwordEntry.EncryptedPassword);
            txtUrl.Content = passwordEntry.URL;
        }

        private void ShowCopyButton(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid)
            {
                Button copyButton = grid.Children[2] as Button;
                if (copyButton != null)
                    copyButton.Visibility = Visibility.Visible;
            }
        }

        private void HideCopyButton(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid)
            {
                Button copyButton = grid.Children[2] as Button;
                if (copyButton != null)
                    copyButton.Visibility = Visibility.Collapsed;
            }
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button == btnCopyUsername)
                    Clipboard.SetText(txtUsername.Content.ToString());
                else if (button == btnCopyPassword)
                    Clipboard.SetText(txtPassword.Password);
                else if (button == btnCopyUrl)
                    Clipboard.SetText(txtUrl.Content.ToString());

                MessageBox.Show("Copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
