using SecureMe.Models;
using SecureMe.Views;
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

namespace SecureMe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            CheckMasterPasswordAndNavigate();
        }

        private void CheckMasterPasswordAndNavigate()
        {
            User user = UserManager.LoadUser();

            if (user == null || string.IsNullOrEmpty(user.HashedMasterPassword))
            {
                _MainFrame.Content = new CreateMasterPasswordPage();
            }
            else
            {
                if (user.LastLoginDate.AddDays(14) < DateTime.Now)
                {
                    _MainFrame.Content = new MasterPasswordPage();
                }
                else
                {
                    _MainFrame.Content = new HomePage();
                }
            }
        }
    }
}
