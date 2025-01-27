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
                MessageBox.Show("Fields is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                string encryptedMasterPassword = Utilities.PasswordHasher.HashPassword(masterPassword);

                if (_currentUser.HashedMasterPassword != encryptedMasterPassword)
                {
                    MessageBox.Show("Invalid master password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _currentUser.IsLoggedIn = true;

                UserManager.SaveUser(_currentUser);

                NavigationService.Navigate(new HomePage());


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
