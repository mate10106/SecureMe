using Microsoft.Win32;
using SecureMe.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for PasswordsPage.xaml
    /// </summary>
    public partial class PasswordsPage : Page
    {
        public PasswordsPage()
        {
            InitializeComponent();
            LoadPasswords();
        }
        private void BtnImportPasswords_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Select Password File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    List<Passwords.PasswordEntry> importedPasswords = new List<Passwords.PasswordEntry>();

                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length < 2)
                    {
                        MessageBox.Show("The selected file is empty or invalid.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string[] headers = lines[0].Split(',');
                    int nameIndex = Array.IndexOf(headers, "name");
                    int usernameIndex = Array.IndexOf(headers, "username");
                    int passwordIndex = Array.IndexOf(headers, "password");
                    int urlIndex = Array.IndexOf(headers, "url");

                    if (nameIndex == -1 || usernameIndex == -1 || passwordIndex == -1)
                    {
                        MessageBox.Show("CSV file does not contain the required fields.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] fields = lines[i].Split(',');

                        if (fields.Length < 3) continue;

                        var passwordEntry = new Passwords.PasswordEntry
                        {
                            Title = fields[nameIndex].Trim(),
                            Username = fields[usernameIndex].Trim(),
                            EncryptedPassword = SecureMe.Utilities.FileManager.EncryptData(fields[passwordIndex].Trim()),
                            URL = urlIndex != -1 ? fields[urlIndex].Trim() : "",
                            LastUsed = DateTime.Now
                        };

                        importedPasswords.Add(passwordEntry);
                    }

                    if (importedPasswords.Any())
                    {
                        PasswordManager.SavePasswords(importedPasswords);
                        MessageBox.Show("Passwords imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("No valid passwords found in the file.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing passwords: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadPasswords()
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe", "passwords.dat")))
            {
                BtnImportPasswords.Visibility = Visibility.Collapsed;
                PasswordListPanel.Visibility = Visibility.Visible;

                List<Passwords.PasswordEntry> passwords = PasswordManager.LoadPasswords();

                PasswordListPanel.Children.Clear();
                foreach (var password in passwords)
                {
                    AddPasswordToUI(password);
                }
            }
            else
            {
                BtnImportPasswords.Visibility = Visibility.Visible;
                PasswordListPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void AddPasswordToUI(Passwords.PasswordEntry passwordEntry)
        {
            CheckBox passwordCheckBox = new CheckBox
            {
                Content = passwordEntry.Title,
                Style = (Style)FindResource("CustomCheckBoxStyle"), 
                Tag = passwordEntry,
                ToolTip = passwordEntry.Username,
                Margin = new Thickness(5)
            };

            Border passwordBorder = new Border
            {
                Child = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children = { passwordCheckBox }
                }
            };

            PasswordListPanel.Children.Add(passwordBorder);
        }


        private void BtnAddPassword_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
