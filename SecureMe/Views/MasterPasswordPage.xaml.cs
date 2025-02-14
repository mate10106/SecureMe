using SecureMe.Models;
using SecureMe.Utilities;
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
    /// Interaction logic for MasterPasswordPage.xaml
    /// </summary>
    public partial class MasterPasswordPage : Page
    {
        public MasterPasswordPage()
        {
            InitializeComponent();
        }

        private void BtnLoginWithMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            string masterPassword = txtMasterPassword.Password.Trim();

            if (string.IsNullOrEmpty(masterPassword))
            {
                MessageBox.Show("Password field is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                User _currentUser = UserManager.LoadUser();

                if (_currentUser == null)
                {
                    MessageBox.Show("No user found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verify the password using the new VerifyPassword method
                bool isPasswordValid = PasswordHasher.VerifyPassword(
                    masterPassword,
                    _currentUser.HashedMasterPassword
                );

                if (!isPasswordValid)
                {
                    MessageBox.Show("Invalid master password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update the last login date
                _currentUser.LastLoginDate = DateTime.Now;
                UserManager.SaveUser(_currentUser);

                // Navigate to the HomePage
                NavigationService.Navigate(new HomePage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Authentication failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
