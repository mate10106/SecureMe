using SecureMe.Models;
using SecureMe.Utilities;
using SecureMe.Views;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace SecureMe
{
    public partial class MainWindow : Window
    {
        private const int LoginValidityDays = 14;

        public MainWindow()
        {
            InitializeComponent();
            CheckFileAndNavigate();
        }

        private void CheckFileAndNavigate()
        {
            string userFilePath = FileManager.FilePath;

            if (!File.Exists(userFilePath))
            {
                _MainFrame.Content = new RegisterPage();
                Console.WriteLine("Navigating to RegisterPage as no user file exists.");
                return;
            }

            User user = UserManager.LoadUser();
            if (user == null)
            {
                _MainFrame.Content = new RegisterPage();
                Console.WriteLine("User file is invalid. Navigating to RegisterPage.");
                return;
            }

            if (string.IsNullOrEmpty(user.HashedMasterPassword))
            {
                _MainFrame.Content = new CreateMasterPasswordPage();
                Console.WriteLine("Navigating to CreateMasterPasswordPage as no master password is set.");
                return;
            }

            if (user.LastLoginDate == default || user.LastLoginDate.AddDays(LoginValidityDays) < DateTime.Now)
            {
                _MainFrame.Content = new MasterPasswordPage();
                Console.WriteLine("Navigating to MasterPasswordPage for authentication.");
            }
            else
            {
                _MainFrame.Content = new HomePage();
                Console.WriteLine("Navigating to HomePage.");
            }

            if (user != null && user.IsLoggedIn)
            {
                _MainFrame.Content = new HomePage();
            }
            else
            {
                _MainFrame.Content = new MasterPasswordPage();
            }
        }
    }
}
