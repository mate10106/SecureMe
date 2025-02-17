using SecureMe.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SecureMe.Views
{
    public partial class DetailsPasswordWindow : Window
    {
        public DetailsPasswordWindow(Passwords.PasswordEntry passwordEntry)
        {
            InitializeComponent();

            txtTitle.Content = passwordEntry.Title;

            if (!string.IsNullOrEmpty(passwordEntry.Title))
            {
                txtMonogram.Content = passwordEntry.Title.Substring(0, 2).ToUpper();
            }

            txtUsername.Content = passwordEntry.Username;
            txtPassword.Password = SecureMe.Utilities.FileManager.DecryptData(passwordEntry.EncryptedPassword);
            txtUrl.Content = passwordEntry.URL;
        }

        private void ShowCopyButton(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                // Change background on hover
                border.Background = new SolidColorBrush(Color.FromRgb(67, 86, 117)); 

                // Find the button inside the Grid
                if (border.Child is Grid grid)
                {
                    Button copyButton = grid.Children[2] as Button;
                    if (copyButton != null)
                        copyButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void HideCopyButton(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Colors.Transparent);

                if (border.Child is Grid grid)
                {
                    Button copyButton = grid.Children[2] as Button;
                    if (copyButton != null)
                        copyButton.Visibility = Visibility.Collapsed;
                }
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
            }
        }
    }
}
