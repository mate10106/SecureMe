using System;
using System.Windows;
using SecureMe.Models;

namespace SecureMe.Views
{
    public partial class EditPasswordWindow : Window
    {
        private Passwords.PasswordEntry _passwordEntry;
        private bool isPasswordVisible = false;

        public EditPasswordWindow(Passwords.PasswordEntry passwordEntry)
        {
            InitializeComponent();
            _passwordEntry = passwordEntry;

            txtTitle.Text = _passwordEntry.Title;
            txtUsername.Text = _passwordEntry.Username;
            txtPassword.Password = SecureMe.Utilities.FileManager.DecryptData(_passwordEntry.EncryptedPassword);
            txtUrl.Text = _passwordEntry.URL;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _passwordEntry.Title = txtTitle.Text;
            _passwordEntry.Username = txtUsername.Text;
            _passwordEntry.EncryptedPassword = SecureMe.Utilities.FileManager.EncryptData(txtPassword.Password);
            _passwordEntry.URL = txtUrl.Text;
            _passwordEntry.LastUsed = DateTime.Now;

            PasswordManager.UpdatePasswords(_passwordEntry);
            MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
