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

        private List<Passwords.PasswordEntry> allPasswords = new List<Passwords.PasswordEntry>();
        private int loadedCount = 0;
        private const int PageSize = 15;
        private bool isLoading = false;
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

                allPasswords = PasswordManager.LoadPasswords(0, PageSize);
                loadedCount = allPasswords.Count;

                PasswordListPanel.Children.Clear();
                foreach (var password in allPasswords)
                {
                    AddPasswordToUI(password);
                }
            }
            else
            {
                BtnImportPasswords.Visibility = Visibility.Visible;
                PasswordListPanel.Visibility = Visibility.Collapsed;
            }

            PasswordListPanel.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(PasswordListScroll));
        }

        private void AddPasswordToUI(Passwords.PasswordEntry passwordEntry)
        {
            Border passwordBorder = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Transparent, // Default: no border
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5),
                Padding = new Thickness(5) // Adds spacing inside the border
            };

            Grid passwordGrid = new Grid();
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); 
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); 

            CheckBox passwordCheckBox = new CheckBox
            {
                Content = passwordEntry.Title,
                Style = (Style)FindResource("CustomCheckBoxStyle"),
                Tag = GetAbbreviation(passwordEntry.Title),
                ToolTip = GetTimeAgo(passwordEntry.LastUsed),
                VerticalAlignment = VerticalAlignment.Center
            };

            Button editButton = new Button
            {
                Style = (Style)FindResource("EditButtonStyle"),
                Tag = passwordEntry
            };
            editButton.Click += EditButton_Click;

            Grid.SetColumn(passwordCheckBox, 0);
            Grid.SetColumn(editButton, 1);
            passwordGrid.Children.Add(passwordCheckBox);
            passwordGrid.Children.Add(editButton);

            passwordBorder.Child = passwordGrid;

            passwordBorder.MouseEnter += (s, e) => passwordBorder.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#1E90FF");
            passwordBorder.MouseLeave += (s, e) => passwordBorder.BorderBrush = Brushes.Transparent;

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
                return "N/A";

            string[] words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                return words[0].Length >= 2 ? words[0].Substring(0, 2).ToUpper() : words[0].ToUpper();
            }

            return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Tag is Passwords.PasswordEntry passwordEntry)
            {
                // Save current scroll position
                double scrollOffset = PasswordListScrollViewer.VerticalOffset;

                EditPasswordWindow editWindow = new EditPasswordWindow(passwordEntry);
                editWindow.ShowDialog();

                var updatedPassword = PasswordManager.LoadPasswords(0, int.MaxValue)
                    .FirstOrDefault(p => p.Title == passwordEntry.Title);

                if (updatedPassword != null)
                {
                    UpdatePasswordInUI(updatedPassword);
                }

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PasswordListScrollViewer.ScrollToVerticalOffset(scrollOffset);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }


        private void BtnAddPassword_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PasswordListScroll(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (scrollViewer != null)
            {
                if (scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 10)
                {
                    LoadMorePasswords();
                }
            }
        }

        private async void LoadMorePasswords()
        {
            if (isLoading) return;
            isLoading = true;

            LoadingPanel.Visibility = Visibility.Visible;

            await Task.Delay(750);

            var newPasswords = PasswordManager.LoadPasswords(loadedCount, PageSize);

            if (newPasswords.Count > 0)
            {
                foreach (var password in newPasswords)
                {
                    AddPasswordToUI(password);
                }
                loadedCount += newPasswords.Count;
            }

            LoadingPanel.Visibility = Visibility.Collapsed;
            isLoading = false;
        }

        private void UpdatePasswordInUI(Passwords.PasswordEntry updatedPassword)
        {
            foreach (UIElement element in PasswordListPanel.Children)
            {
                if (element is Border border && border.Child is Grid grid &&
                    grid.Children.OfType<CheckBox>().FirstOrDefault() is CheckBox checkBox &&
                    checkBox.Content.ToString() == updatedPassword.Title)
                {
                    checkBox.ToolTip = GetTimeAgo(updatedPassword.LastUsed);
                    return; // Stop searching after finding the matching password
                }
            }
        }
    }
}
