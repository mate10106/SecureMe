using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SecureMe.Views
{
    public partial class RecoveryPhrase : Page
    {
        public ObservableCollection<string> RecoveryWords { get; set; }

        public RecoveryPhrase()
        {
            InitializeComponent();

            // Load words from the english.txt file
            var wordList = LoadWordListFromFile("Resources/english.txt");

            // Generate 16 random words
            RecoveryWords = new ObservableCollection<string>(GenerateRandomWords(wordList, 16));

            // Set the DataContext to this instance of the class
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
    }
}
