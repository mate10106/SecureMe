using SecureMe.Models;
using SecureMe.Utilities;
using System;
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
            LoadAndDisplayPasswordAnalysis();
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

                // Main Grid layout
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Left side (text)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Right side (button)

                // Left StackPanel for text info
                var textStack = new StackPanel();

                // Title (e.g., "Facebook", "Twitter")
                textStack.Children.Add(new TextBlock
                {
                    Text = entry.Title,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                });

                // Password Strength
                textStack.Children.Add(new TextBlock
                {
                    Text = $"{analysis.Strength} Password",
                    Foreground = analysis.Strength == "Medium" ? Brushes.Goldenrod : Brushes.Red,
                    FontSize = 16
                });

                // Issues
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

                // Change Password Button (aligned right)
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

                // Border for hover effect
                var border = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5),
                    Padding = new Thickness(10),
                    BorderBrush = Brushes.Transparent, // Default border (hidden)
                    BorderThickness = new Thickness(1)
                };

                // Hover effect
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
