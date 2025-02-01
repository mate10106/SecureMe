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
                Tag = GetAbbreviation(passwordEntry.Title),
                ToolTip = GetTimeAgo(passwordEntry.LastUsed),
                Margin = new Thickness(5)
            };

            Button editButton = new Button
            {
                Content = "Edit",
                Width = 50,
                Height = 25,
                Margin = new Thickness(10, 0, 0, 0),
                Tag = passwordEntry
            };

            editButton.Click += EditButton_Click;

            StackPanel passwordPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children = { passwordCheckBox, editButton }
            };

            Border passwordBorder = new Border
            {
                Child = passwordPanel,
                Margin = new Thickness(5)
            };

            PasswordListPanel.Children.Add(passwordBorder);
        }


        private string GetTimeAgo(DateTime lastUsed)
        {
            TimeSpan timeSince = DateTime.Now - lastUsed;

            if (timeSince.TotalSeconds < 60)
                return "Just now";
            else if (timeSince.TotalMinutes < 60)
                return $"{(int)timeSince.TotalMinutes} minutes ago";
            else if (timeSince.TotalHours < 24)
                return $"{(int)timeSince.TotalHours} hours ago";
            else if (timeSince.TotalDays < 7)
                return $"{(int)timeSince.TotalDays} days ago";
            else if (timeSince.TotalDays < 30)
                return $"{(int)(timeSince.TotalDays / 7)} weeks ago";
            else if (timeSince.TotalDays < 365)
                return $"{(int)(timeSince.TotalDays / 30)} months ago";
            else
                return $"{(int)(timeSince.TotalDays / 365)} years ago";
        }

        private string GetAbbreviation(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return "N/A"; // Default if empty

            string[] words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                return words[0].Length >= 2 ? words[0].Substring(0, 2).ToUpper() : words[0].ToUpper();
            }

            return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && clickedButton.Tag is Passwords.PasswordEntry passwordEntry)
            {
                EditPasswordWindow editWindow = new EditPasswordWindow(passwordEntry);
                editWindow.ShowDialog();

                // Reload passwords after editing
                LoadPasswords();
            }
        }


        private void BtnAddPassword_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
