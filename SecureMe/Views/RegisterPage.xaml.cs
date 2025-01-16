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
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
             this.NavigationService.Navigate(new LoginPage());
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SecureMe.Utilities.FileManager.InitializeStorage();

            try
            {
                string hashedPassword = HashPassword(password);

                var user = new { Username = username, PasswordHash = hashedPassword };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(user);

                SecureMe.Utilities.FileManager.RegisterUser(username, hashedPassword);

                MessageBox.Show("Registered Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NavigationService.Navigate(new LoginPage());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
