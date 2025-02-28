using SecureMe.Models;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SecureMe.Views
{
    public partial class AddNewPasswordWindow : Window
    {
        public Passwords.PasswordEntry CreatedPassword { get; private set; }
        public delegate void PasswordAddedEventHandler(Passwords.PasswordEntry newPassword);
        public event PasswordAddedEventHandler PasswordAdded;
        private bool isPasswordVisible = false;

        private Random random = new Random();

        public AddNewPasswordWindow()
        {
            InitializeComponent();
            txtPassword.PasswordChanged += TxtPassword_PasswordChanged;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            int strength = CalculatePasswordStrength(txtPassword.Password);
            passwordStrengthBar.Value = strength;

            if (strength < 40)
            {
                txtPasswordStrength.Text = "Weak";
                txtPasswordStrength.Foreground = System.Windows.Media.Brushes.Red;
                passwordStrengthBar.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (strength < 70)
            {
                txtPasswordStrength.Text = "Medium";
                txtPasswordStrength.Foreground = System.Windows.Media.Brushes.Orange;
                passwordStrengthBar.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                txtPasswordStrength.Text = "Strong";
                txtPasswordStrength.Foreground = System.Windows.Media.Brushes.LimeGreen;
                passwordStrengthBar.Foreground = System.Windows.Media.Brushes.LimeGreen;
            }
        }

        private int CalculatePasswordStrength(string password)
        {
            int score = 0;

            if (string.IsNullOrEmpty(password))
                return 0;

            score += Math.Min(30, password.Length * 3);

            if (Regex.IsMatch(password, @"[A-Z]"))
                score += 10;
            if (Regex.IsMatch(password, @"[a-z]"))
                score += 10;
            if (Regex.IsMatch(password, @"[0-9]"))
                score += 10;
            if (Regex.IsMatch(password, @"[^A-Za-z0-9]"))
                score += 15;

            int charTypes = 0;
            if (Regex.IsMatch(password, @"[A-Z]")) charTypes++;
            if (Regex.IsMatch(password, @"[a-z]")) charTypes++;
            if (Regex.IsMatch(password, @"[0-9]")) charTypes++;
            if (Regex.IsMatch(password, @"[^A-Za-z0-9]")) charTypes++;

            if (charTypes >= 3)
                score += 25;

            return Math.Min(100, score);
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string password = GenerateStrongPassword();
            txtPassword.Password = password;
        }

        private string GenerateStrongPassword()
        {
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numberChars = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            StringBuilder passwordBuilder = new StringBuilder();

            int length = random.Next(12, 17);

            passwordBuilder.Append(lowerChars[random.Next(lowerChars.Length)]);
            passwordBuilder.Append(upperChars[random.Next(upperChars.Length)]);
            passwordBuilder.Append(numberChars[random.Next(numberChars.Length)]);
            passwordBuilder.Append(specialChars[random.Next(specialChars.Length)]);

            string allChars = lowerChars + upperChars + numberChars + specialChars;
            for (int i = 4; i < length; i++)
            {
                passwordBuilder.Append(allChars[random.Next(allChars.Length)]);
            }

            return ShuffleString(passwordBuilder.ToString());
        }

        private string ShuffleString(string str)
        {
            char[] array = str.ToCharArray();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
            return new string(array);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title for this password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter a password.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            Passwords.PasswordEntry newEntry = new Passwords.PasswordEntry
            {
                Title = txtTitle.Text,
                Username = txtUsername.Text,
                EncryptedPassword = SecureMe.Utilities.FileManager.EncryptData(txtPassword.Password),
                URL = txtUrl.Text,
                LastUsed = DateTime.Now
            };

            PasswordManager.AddPassword(newEntry);

            this.CreatedPassword = newEntry;

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }
    }
}