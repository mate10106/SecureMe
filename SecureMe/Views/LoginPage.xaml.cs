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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            main._MainFrame.Content = new RegisterPage();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logged in Successfully");
        }

    }
}
