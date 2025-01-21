using Newtonsoft.Json;
using SecureMe.Models;
using SecureMe.Utilities;
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
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string encryptedData = FileManager.ReadData();

                if (string.IsNullOrEmpty(encryptedData))
                {
                    MessageBox.Show("No data found or data is corrupted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var decryptedData = FileManager.DecryptData(encryptedData);

                var users = JsonConvert.DeserializeObject<List<User>>(decryptedData);

                string hashedPassword = HashPassword(password);
                Console.WriteLine($"Login Attempt: Username={username}, HashedPassword={hashedPassword}");

                var user = users?.Find(u => u.Username == username && u.HashedPassword == hashedPassword);

                if (user != null)
                {
                    MessageBox.Show("Login successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow main = Application.Current.MainWindow as MainWindow;
                    main._MainFrame.Content = new CreateMasterPasswordPage();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (JsonException jsonEx)
            {
                MessageBox.Show($"An error occurred while reading user data: {jsonEx.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

    }
}
