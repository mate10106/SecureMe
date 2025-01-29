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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            MainContentFrame.Navigate(new AllItemsPage());
        }

        private void BtnAllItems_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new AllItemsPage());
        }
        private void BtnPasswords_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PasswordsPage());
        }

        private void BtnPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PersonalInfoPage());
        }
        private void BtnTrash_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new TrashPage());
        }

        private void BtnPasswordGenerator_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PasswordGeneratorPage());
        }

        private void BtnPasswordHealth_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new PasswordHealthPage());
        }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User _currentUser = UserManager.LoadUser();

                _currentUser.IsLoggedIn = false;

                UserManager.SaveUser(_currentUser);

                NavigationService.Navigate(new MasterPasswordPage());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
