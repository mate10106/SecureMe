using SecureMe.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SecureMe.Views
{
    public partial class RecoveryPhrasePage : Page
    {
        public ObservableCollection<string> RecoveryWords { get; set; }

        public RecoveryPhrasePage()
        {
            InitializeComponent();

            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "english.txt");
            var wordList = LoadWordListFromFile(filePath);

            RecoveryWords = new ObservableCollection<string>(GenerateRandomWords(wordList, 16));

            DataContext = this;
        }

        private string[] LoadWordListFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Word list file not found.", filePath);

                return File.ReadAllLines(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading word list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Array.Empty<string>();
            }
        }

        private string[] GenerateRandomWords(string[] wordList, int count)
        {
            var random = new Random();
            return wordList.OrderBy(x => random.Next()).Take(count).ToArray();
        }

        private void SavedRecoveryPhrase_Click(object sender, RoutedEventArgs e)
        {
            User currentUser = UserManager.LoadUser();

            string recoveryPhrase = string.Join(" ", RecoveryWords);

            string encryptedPhrase = SecureMe.Utilities.FileManager.EncryptData(recoveryPhrase);

            currentUser.RecoveryPhrase = new List<string> { encryptedPhrase };

            UserManager.SaveUser(currentUser); 
   
            NavigationService.Navigate(new VerifyRecoveryPhrasePage());

        }
        private void CopyRecoveryPhrase_Click(object sender, RoutedEventArgs e)
        {
            string phrase = string.Join(" ", RecoveryWords);

            Clipboard.SetText(phrase);

            MessageBox.Show("Recovery phrase copied to clipboard!", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}