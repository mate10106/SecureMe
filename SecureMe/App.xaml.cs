using SecureMe.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace SecureMe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _trayIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
        }
        protected override void OnExit(ExitEventArgs e)
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
                Console.WriteLine($"Error on exit: {ex.Message}");
            }

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.TrayIcon.Dispose();
            }

            base.OnExit(e);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
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
                Console.WriteLine($"Error on exit: {ex.Message}");
            }

            _trayIcon?.Dispose();
            Application.Current.Shutdown();
        }
    }
}
