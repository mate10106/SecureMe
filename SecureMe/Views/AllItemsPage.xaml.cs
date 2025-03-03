using SecureMe.Models;
using SecureMe.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SecureMe.Views
{
    public partial class AllItemsPage : Page
    {
        public AllItemsPage()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var allPasswords = PasswordManager.LoadPasswords();

            UpdatePasswordStats(allPasswords);

            LoadRecentItems(allPasswords);
        }

        private void UpdatePasswordStats(List<Passwords.PasswordEntry> allPasswords)
        {
            // Count total passwords
            int totalCount = allPasswords.Count;
            TotalPasswordsCount.Text = totalCount.ToString();

            // Count recent items (last 30 days)
            int recentCount = allPasswords.Count(p => (DateTime.Now - p.LastUsed).TotalDays <= 30);
            RecentItemsCount.Text = recentCount.ToString();

            // Analyze password strength
            Dictionary<string, int> strengthCounts = new Dictionary<string, int>
            {
                { "Strong", 0 },
                { "Medium", 0 },
                { "Weak", 0 }
            };

            Dictionary<string, int> passwordOccurrences = new Dictionary<string, int>();

            foreach (var entry in allPasswords)
            {
                // Check password strength
                string plainPassword = FileManager.DecryptData(entry.EncryptedPassword);
                var analysis = PasswordAnalysisHelper.EvaluatePasswordStrength(plainPassword);
                strengthCounts[analysis.Strength]++;

                // Track password reuse
                if (!passwordOccurrences.ContainsKey(plainPassword))
                    passwordOccurrences[plainPassword] = 0;
                passwordOccurrences[plainPassword]++;
            }

            // Update weak passwords count
            int weakCount = strengthCounts["Weak"];
            WeakPasswordsCount.Text = weakCount.ToString();
            WeakPasswordsCountDetail.Text = weakCount.ToString();

            // Update strong passwords count
            StrongPasswordsCount.Text = strengthCounts["Strong"].ToString();

            // Count reused passwords
            int reusedCount = passwordOccurrences.Count(kv => kv.Value > 1);
            ReusedPasswordsCount.Text = reusedCount.ToString();

            // Calculate overall score (0-100)
            int overallScore = 0;
            if (totalCount > 0)
            {
                int strongPercentage = (strengthCounts["Strong"] * 100) / totalCount;
                int reusedPercentage = reusedCount > 0 ? (reusedCount * 100) / totalCount : 0;

                // Score formula: 50% based on strong passwords, 50% based on unique passwords
                overallScore = (int)(strongPercentage * 0.7 + (100 - reusedPercentage) * 0.3);
                overallScore = Math.Min(100, Math.Max(0, overallScore)); // Ensure between 0-100
            }

            OverallScoreText.Text = $"{overallScore}%";
            OverallScoreBar.Value = overallScore;

            // Set color based on score
            Color scoreColor;
            if (overallScore >= 80)
                scoreColor = (Color)ColorConverter.ConvertFromString("#10B981"); // Green
            else if (overallScore >= 50)
                scoreColor = (Color)ColorConverter.ConvertFromString("#F59E0B"); // Orange
            else
                scoreColor = (Color)ColorConverter.ConvertFromString("#EF4444"); // Red

            OverallScoreText.Foreground = new SolidColorBrush(scoreColor);
            OverallScoreBar.Foreground = new SolidColorBrush(scoreColor);
        }

        private void LoadRecentItems(List<Passwords.PasswordEntry> allPasswords)
        {
            // Clear existing items
            RecentItemsPanel.Children.Clear();

            // Get most recent passwords (up to 7)
            var recentItems = allPasswords
                .OrderByDescending(p => p.LastUsed)
                .Take(7)
                .ToList();

            foreach (var entry in recentItems)
            {
                // Create grid for item
                Grid itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Icon
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Title
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Action

                // Icon
                Border iconBorder = new Border
                {
                    Width = 36,
                    Height = 36,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    CornerRadius = new CornerRadius(6),
                    Margin = new Thickness(0, 0, 15, 0)
                };

                TextBlock iconText = new TextBlock
                {
                    Text = entry.Title.Length > 0 ? entry.Title.Substring(0, 1).ToUpper() : "P",
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                iconBorder.Child = iconText;
                Grid.SetColumn(iconBorder, 0);
                itemGrid.Children.Add(iconBorder);

                // Title and date
                StackPanel titlePanel = new StackPanel();

                TextBlock titleText = new TextBlock
                {
                    Text = entry.Title,
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    FontSize = 16
                };

                TextBlock dateText = new TextBlock
                {
                    Text = GetTimeAgo(entry.LastUsed),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8")),
                    FontSize = 14,
                    Margin = new Thickness(0, 4, 0, 0)
                };

                titlePanel.Children.Add(titleText);
                titlePanel.Children.Add(dateText);

                Grid.SetColumn(titlePanel, 1);
                itemGrid.Children.Add(titlePanel);

                // View button
                Button viewButton = new Button
                {
                    Content = "View",
                    Padding = new Thickness(15, 6, 15, 6),
                    Background = Brushes.Transparent,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    Tag = entry
                };

                viewButton.Resources = new ResourceDictionary
                {
                    { "CornerRadius", new CornerRadius(4) }
                };

                viewButton.Click += ViewButton_Click;

                Grid.SetColumn(viewButton, 2);
                itemGrid.Children.Add(viewButton);

                // Add to panel with separator
                RecentItemsPanel.Children.Add(itemGrid);

                // Add separator if not last item
                if (recentItems.IndexOf(entry) < recentItems.Count - 1)
                {
                    Border separator = new Border
                    {
                        Height = 1,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")),
                        Margin = new Thickness(0, 15, 0, 15)
                    };

                    RecentItemsPanel.Children.Add(separator);
                }
            }
        }

        private string GetTimeAgo(DateTime date)
        {
            TimeSpan timeSpan = DateTime.Now - date;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            if (timeSpan.TotalHours < 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalDays < 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";

            return date.ToString("MMM d, yyyy");
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Passwords.PasswordEntry passwordEntry)
            {
                var detailsWindow = new DetailsPasswordWindow(passwordEntry);
                detailsWindow.Show();
            }
        }

        private void ViewPasswordHealth_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordHealthPage());
        }
    }
}