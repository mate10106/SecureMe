using SecureMe.Models;
using SecureMe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static SecureMe.Models.Passwords;

namespace SecureMe.Views
{
    public partial class PasswordHealthPage : Page
    {
        public PasswordHealthPage()
        {
            InitializeComponent();

            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var allPasswords = PasswordManager.LoadPasswords();
            UpdatePasswordStats(allPasswords);
            LoadAndDisplayPasswordAnalysis();
        }

        private void UpdatePasswordStats(List<Passwords.PasswordEntry> allPasswords)
        {
            // Total password count
            int totalCount = allPasswords.Count;
            TotalPasswordsCount.Text = totalCount.ToString();

            // Count weak passwords using the PasswordAnalysisHelper
            int weakCount = 0;
            Dictionary<string, int> passwordOccurrences = new Dictionary<string, int>();

            foreach (var entry in allPasswords)
            {
                string plainPassword = FileManager.DecryptData(entry.EncryptedPassword);

                // Check if password is weak
                var analysis = PasswordAnalysisHelper.EvaluatePasswordStrength(plainPassword);
                if (analysis.Strength == "Weak")
                    weakCount++;

                // Track password occurrences for reuse detection
                if (!passwordOccurrences.ContainsKey(plainPassword))
                    passwordOccurrences[plainPassword] = 0;
                passwordOccurrences[plainPassword]++;
            }

            // Set weak password count
            WeakPasswordsCount.Text = weakCount.ToString();

            // Count reused passwords
            int reusedPasswords = passwordOccurrences.Count(p => p.Value > 1);
            ReusedPasswordsCount.Text = reusedPasswords.ToString();
        }

        private void LoadAndDisplayPasswordAnalysis()
        {
            var passwords = PasswordManager.LoadPasswords();
            AnalysisPanel.Children.Clear();

            foreach (var entry in passwords)
            {
                string plainPassword = FileManager.DecryptData(entry.EncryptedPassword);
                var analysis = PasswordAnalysisHelper.EvaluatePasswordStrength(plainPassword);

                if (analysis.Strength == "Strong") continue;

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Left side (text)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Right side (button)

                var textStack = new StackPanel();

                textStack.Children.Add(new TextBlock
                {
                    Text = entry.Title,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                });

                textStack.Children.Add(new TextBlock
                {
                    Text = $"{analysis.Strength} Password",
                    Foreground = analysis.Strength == "Medium" ? Brushes.Goldenrod : Brushes.Red,
                    FontSize = 16
                });

                if (analysis.Issues?.Count > 0)
                {
                    foreach (var issue in analysis.Issues)
                    {
                        textStack.Children.Add(new TextBlock
                        {
                            Text = $"• {issue}",
                            Foreground = Brushes.Red,
                            FontSize = 14
                        });
                    }
                }

                Grid.SetColumn(textStack, 0);
                grid.Children.Add(textStack);

                var changePasswordButton = new Button
                {
                    Content = "Change Password",
                    Foreground = Brushes.Blue,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    FontSize = 14,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Padding = new Thickness(10)
                };
                changePasswordButton.Click += (sender, e) => OpenEditPasswordWindow(entry);

                Grid.SetColumn(changePasswordButton, 1);
                grid.Children.Add(changePasswordButton);

                var border = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.Transparent,
                    BorderThickness = new Thickness(1)
                };

                border.MouseEnter += (s, e) => border.BorderBrush = Brushes.Gray;
                border.MouseLeave += (s, e) => border.BorderBrush = Brushes.Transparent;

                border.Child = grid;
                AnalysisPanel.Children.Add(border);

                // Divider line
                var line = new Rectangle
                {
                    Height = 1,
                    Fill = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                    Margin = new Thickness(5, 10, 5, 10)
                };
                AnalysisPanel.Children.Add(line);
            }
        }

        private void OpenEditPasswordWindow(PasswordEntry entry)
        {
            var editWindow = new EditPasswordWindow(entry);
            editWindow.ShowDialog();
        }
    }
}
