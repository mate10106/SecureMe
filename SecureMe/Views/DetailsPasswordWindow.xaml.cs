using SecureMe.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System;

namespace SecureMe.Views
{
    public partial class DetailsPasswordWindow : Window
    {
        [DllImport("user32.dll")]
         private static extern IntPtr GetOpenClipboardWindow();
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
                border.Background = new SolidColorBrush(Color.FromRgb(67, 86, 117)); 

                if (border.Child is Grid grid)
                {
                    if (grid.Children[2] is Button copyButton)
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
                    if (grid.Children[2] is Button copyButton)
                        copyButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string textToCopy = "";

                if (button == btnCopyUsername)
                    textToCopy = txtUsername.Content.ToString();
                else if (button == btnCopyPassword)
                    textToCopy = txtPassword.Password;
                else if (button == btnCopyUrl)
                    textToCopy = txtUrl.Content.ToString();

                TryCopyToClipboard(textToCopy);
            }
        }

        private void TryCopyToClipboard(string text)
        {
            const int maxAttempts = 3;
            Exception lastException = null;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    IDataObject dataObject = new DataObject();
                    dataObject.SetData(DataFormats.Text, text);
                    Clipboard.SetDataObject(dataObject, true);
                    return;
                }
                catch (ExternalException ex)
                {
                    lastException = ex;

                    System.Threading.Thread.Sleep(100);
                }
            }

            MessageBox.Show($"Could not copy to clipboard: {lastException.Message}\n\nYour antivirus software might be blocking clipboard access.",
                "Clipboard Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
