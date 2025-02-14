using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SecureMe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            // Dispose of the tray icon
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.TrayIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
