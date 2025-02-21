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
using System.Windows.Threading;

namespace SecureMe
{
    public partial class MainWindow : Window
    {
        private const int LoginValidityDays = 14;
        private DispatcherTimer _inactivityTimer;
        private NotifyIcon _trayIcon;

        public NotifyIcon TrayIcon => _trayIcon;

        public MainWindow()
        {
            InitializeComponent();
            StartInactivityTimer();
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
            try
            {
                User user = UserManager.LoadUser();
                if (user != null)
                {
                    user.IsLoginWithMasterPassword = false;
                    UserManager.SaveUser(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing: {ex.Message}");
            }

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
                user.IsLoggedIn = false;
                _MainFrame.Content = new LoginPage();
                Console.WriteLine("Navigating to LoginPage for authentication.");
            }
            else
            {
                _MainFrame.Content = new HomePage();
                Console.WriteLine("Navigating to HomePage.");
            }
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();

            try
            {
                User user = UserManager.LoadUser();
                if (user != null)
                {
                    user.IsLoginWithMasterPassword = false;
                    UserManager.SaveUser(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during auto-lock: {ex.Message}");
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                _MainFrame.Content = new MasterPasswordPage();
            });
        }

        private void StartInactivityTimer()
        {
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(1);
            _inactivityTimer.Tick += InactivityTimer_Tick;
            _inactivityTimer.Start();

            this.PreviewMouseMove += ResetInactivityTimer;
            this.PreviewKeyDown += ResetInactivityTimer;
        }

        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

    }
}