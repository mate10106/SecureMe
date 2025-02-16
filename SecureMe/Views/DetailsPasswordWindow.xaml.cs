using SecureMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for DetailsPasswordWindow.xaml
    /// </summary>
    public partial class DetailsPasswordWindow : Window
    {
        private bool isPasswordVisible = false;
        public DetailsPasswordWindow(Passwords.PasswordEntry passwordEntry)
        {
            InitializeComponent();

            // Load existing values
            txtTitle.Text = passwordEntry.Title;
            txtUsername.Text = passwordEntry.Username;
            txtPassword.Password = SecureMe.Utilities.FileManager.DecryptData(passwordEntry.EncryptedPassword);
            txtUrl.Text = passwordEntry.URL;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }
    }
}