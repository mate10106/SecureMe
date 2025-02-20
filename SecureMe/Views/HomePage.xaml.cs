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
using System.Windows.Threading;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private List<Passwords.PasswordEntry> _allPasswords;
        private DispatcherTimer _inactivityTimer;
        public HomePage()
        {
            InitializeComponent();

            StartInactivityTimer();

            this.PreviewMouseMove += ResetInactivityTimer;
            this.PreviewKeyDown += ResetInactivityTimer;


            _allPasswords = PasswordManager.LoadPasswords();
            MainContentFrame.Navigate(new AllItemsPage());
        }

        private void StartInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(10);
            _inactivityTimer.Tick += InactivityTimer_Tick;
            _inactivityTimer.Start();
        }
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();

            NavigationService.Navigate(new MasterPasswordPage());
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                // Reset to show all items if search is empty
                MainContentFrame.Navigate(new PasswordsPage());
                return;
            }

            // Filter passwords based on the search query
            var filteredPasswords = _allPasswords
                .Where(p => p.Title.ToLower().Contains(query) || p.Username.ToLower().Contains(query))
                .ToList();

            // Navigate to PasswordsPage with filtered results
            MainContentFrame.Navigate(new PasswordsPage(filteredPasswords));
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
        private void BtnSecureNotes_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new SecureNotesPage());
        }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User _currentUser = UserManager.LoadUser();

                _currentUser.IsLoginWithMasterPassword = false;

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