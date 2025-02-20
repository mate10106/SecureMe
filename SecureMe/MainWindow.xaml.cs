using SecureMe.Models;
using SecureMe.Utilities;
using SecureMe.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace SecureMe
{
    public partial class MainWindow : Window
    {
        private const int LoginValidityDays = 14;
        private NotifyIcon _trayIcon;

        public NotifyIcon TrayIcon => _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            CheckFileAndNavigate();
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            _trayIcon = new NotifyIcon();

            _trayIcon.Visible = true;
            _trayIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
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

            if (!user.IsLoggedIn)
            {
                _MainFrame.Content = new LoginPage();
                Console.WriteLine("Navigating to LoginPage for authentication.");
                return;
            }

            if (!user.IsLoginWithMasterPassword)
            {
                _MainFrame.Content = new MasterPasswordPage();
                Console.WriteLine("Navigating to MasterPasswordPage.");
                return;
            }

            if (user.IsLoggedIn && user.IsLoginWithMasterPassword)
            {
                _MainFrame.Content = new HomePage();
                Console.WriteLine("Navigating to HomePage.");
                return;
            }

            _MainFrame.Content = new MasterPasswordPage();
            Console.WriteLine("Navigating to MasterPasswordPage for authentication.");

            if (user.LastLoginDate == default || user.LastLoginDate.AddDays(LoginValidityDays) < DateTime.Now)
            {
                user.IsLoginWithMasterPassword = false;
                _MainFrame.Content = new MasterPasswordPage();
                Console.WriteLine("Navigating to MasterPasswordPage for authentication.");
            }
            else
            {
                _MainFrame.Content = new HomePage();
                Console.WriteLine("Navigating to HomePage.");
            }
        }
    }
}