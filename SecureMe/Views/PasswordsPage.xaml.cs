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
        public PasswordsPage(List<Passwords.PasswordEntry> filteredPasswords = null)
        {
            InitializeComponent();

            if (filteredPasswords != null)
            {
                isSearchMode = true;
                allPasswords = filteredPasswords;
                loadedCount = filteredPasswords.Count;

                PasswordListPanel.Children.Clear();
                foreach (var password in filteredPasswords)
                {
                    AddPasswordToUI(password);
                }

                BtnImportPasswords.Visibility = Visibility.Collapsed;
                PasswordListPanel.Visibility = Visibility.Visible;
            }
            else
            {
                LoadPasswords();
            }
        }

        private List<Passwords.PasswordEntry> allPasswords = new List<Passwords.PasswordEntry>();
        private int loadedCount = 0;
        private const int PageSize = 15;
        private bool isLoading = false;
        private readonly bool isSearchMode = false;
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
                    // Lists for both Passwords and Secure Notes
                    List<Passwords.PasswordEntry> importedPasswords = new List<Passwords.PasswordEntry>();
                    List<SecureNotes> importedSecureNotes = new List<SecureNotes>();

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
                    int noteIndex = Array.IndexOf(headers, "note"); // Added note index

                    if (nameIndex == -1 || usernameIndex == -1 || passwordIndex == -1)
                    {
                        MessageBox.Show("CSV file does not contain the required fields.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Process each row
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] fields = lines[i].Split(',');
                        if (fields.Length < 3) continue;  // Skip incomplete rows

                        // Check if the password field is empty or whitespace.
                        if (string.IsNullOrWhiteSpace(fields[passwordIndex].Trim()))
                        {
                            // Treat as a secure note.
                            var secureNote = new SecureNotes
                            {
                                Title = fields[nameIndex].Trim(),
                                // Use the "note" field for secure note content.
                                SecuredNotes = noteIndex != -1 ? fields[noteIndex].Trim() : string.Empty,
                                CreatedNotes = DateTime.Now
                            };
                            importedSecureNotes.Add(secureNote);
                        }
                        else
                        {
                            // Create a regular password entry.
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
                    }

                    // Save the imported items separately.
                    if (importedPasswords.Any())
                    {
                        PasswordManager.SavePasswords(importedPasswords);
                        MessageBox.Show("Passwords imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (importedSecureNotes.Any())
                    {
                        SecureNotesManager.SaveSecureNotes(importedSecureNotes);
                        MessageBox.Show("Secure notes imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (!importedPasswords.Any() && !importedSecureNotes.Any())
                    {
                        MessageBox.Show("No valid entries found in the file.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                BorderBrush = Brushes.Transparent,
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Cursor = Cursors.Hand,
            };

            Grid passwordGrid = new Grid();
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Checkbox column
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Monogram column
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Title column
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Edit button
            passwordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Copy button

            // Checkbox (Separate from Monogram)
            CheckBox passwordCheckBox = new CheckBox
            {
                Style = (Style)FindResource("CustomCheckBoxStyle"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            passwordCheckBox.Checked += PasswordCheckBox_Checked;
            passwordCheckBox.Unchecked += PasswordCheckBox_Unchecked;

            // Monogram (Using Title abbreviation)
            Border monogramBorder = new Border
            {
                Background = Brushes.Blue, // Ensures the monogram has the correct background
                Width = 30,
                Height = 30,
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create a TextBlock for the monogram text
            TextBlock monogramTextBlock = new TextBlock
            {
                Text = GetAbbreviation(passwordEntry.Title),
                Foreground = Brushes.White,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Title Button with TimeAgo
            Button titleButton = new Button
            {
                Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children =
            {
                new TextBlock
                {
                    Text = passwordEntry.Title,
                    Foreground = Brushes.White,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10, 0, 0, 0)
                },
                new TextBlock
                {
                    Text = GetTimeAgo(passwordEntry.LastUsed),
                    Foreground = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    Margin = new Thickness(10, 0, 0, 0)
                }
            }
                },
                Style = (Style)FindResource("TransparentButtonStyle"),
                Tag = passwordEntry
            };
            titleButton.Click += TitleButton_Click;

            // Edit Button
            Button editButton = new Button
            {
                Style = (Style)FindResource("EditButtonStyle"),
                Tag = passwordEntry,
                Margin = new Thickness(5)
            };
            editButton.Click += EditButton_Click;

            // Copy Button
            Button copyButton = new Button
            {
                Style = (Style)FindResource("CopyButtonStyle"),
                Tag = passwordEntry,
                Margin = new Thickness(5)
            };
            copyButton.Click += CopyButton_Click;

            // Assign columns
            Grid.SetColumn(passwordCheckBox, 0);
            Grid.SetColumn(monogramTextBlock, 1);
            Grid.SetColumn(titleButton, 2);
            Grid.SetColumn(editButton, 3);
            Grid.SetColumn(copyButton, 4);

            // Add elements to grid
            passwordGrid.Children.Add(passwordCheckBox);
            passwordGrid.Children.Add(monogramTextBlock);
            passwordGrid.Children.Add(titleButton);
            passwordGrid.Children.Add(editButton);
            passwordGrid.Children.Add(copyButton);

            passwordBorder.Child = passwordGrid;

            // Add hover effect
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
            if (isSearchMode) return;

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
                if (element is Border border && border.Child is Grid grid)
                {
                    var titleButton = grid.Children.OfType<Button>().FirstOrDefault(b => Grid.GetColumn(b) == 1);
                    if (titleButton?.Tag is Passwords.PasswordEntry entry && entry.Title == updatedPassword.Title)
                    {
                        var stackPanel = titleButton.Content as StackPanel;
                        var timeTextBlock = stackPanel?.Children.OfType<TextBlock>().LastOrDefault();
                        if (timeTextBlock != null)
                        {
                            timeTextBlock.Text = GetTimeAgo(updatedPassword.LastUsed);
                        }
                    }
                }
            }
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Passwords.PasswordEntry passwordEntry)
            {
                passwordEntry.LastUsed = DateTime.Now;
                PasswordManager.UpdatePassword(passwordEntry);
                UpdatePasswordInUI(passwordEntry);

                var detailsWindow = new DetailsPasswordWindow(passwordEntry);
                detailsWindow.Show();
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Passwords.PasswordEntry passwordEntry)
            {
                string decryptedPassword = SecureMe.Utilities.FileManager.DecryptData(passwordEntry.EncryptedPassword);
                Clipboard.SetText(decryptedPassword);
                // Optional: Show a toast or message
            }
        }

        private void CheckAllBox_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(true);
        }

        private void CheckAllBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(false);
        }

        private void SetAllCheckboxes(bool isChecked)
        {
            int count = 0;

            foreach (UIElement element in PasswordListPanel.Children)
            {
                if (element is Border border && border.Child is Grid grid)
                {
                    CheckBox checkBox = grid.Children.OfType<CheckBox>().FirstOrDefault();
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = isChecked;
                        count += isChecked ? 1 : 0;
                    }
                }
            }

            CheckedCountText.Text = $"({count} selected)";
        }
        private void PasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCheckedCount();
        }

        private void PasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateCheckedCount();
        }

        private void UpdateCheckedCount()
        {
            int count = PasswordListPanel.Children.OfType<Border>()
                .Select(border => border.Child as Grid)
                .SelectMany(grid => grid.Children.OfType<CheckBox>())
                .Count(cb => cb.IsChecked == true);

            CheckedCountText.Text = $"({count} selected)";

            // Update Select All checkbox state
            CheckAllBox.Checked -= CheckAllBox_Checked;
            CheckAllBox.Unchecked -= CheckAllBox_Unchecked;
            CheckAllBox.IsChecked = count == PasswordListPanel.Children.Count;
            CheckAllBox.Checked += CheckAllBox_Checked;
            CheckAllBox.Unchecked += CheckAllBox_Unchecked;
        }
    }
}