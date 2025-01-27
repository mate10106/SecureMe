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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for CreateMasterPasswordPage.xaml
    /// </summary>
    public partial class CreateMasterPasswordPage : Page
    {
        public CreateMasterPasswordPage()
        {
            InitializeComponent();
        }

        private void BtnMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            string password = txtMasterPassword.Password.Trim();
            string confirmPassword = txtConfirmMasterPassword.Password.Trim();


            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SecureMe.Utilities.FileManager.InitializeStorage();

            try
            {
                User existingUser = UserManager.LoadUser();

                if (existingUser != null)
                {
                    existingUser.HashedMasterPassword = Utilities.PasswordHasher.HashPassword(password);
                    existingUser.LastLoginDate = DateTime.Now;
                    UserManager.SaveUser(existingUser);

                    MessageBox.Show("Master password set successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new HomePage());
                }
                else
                {
                    MessageBox.Show("Failed to load user data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
