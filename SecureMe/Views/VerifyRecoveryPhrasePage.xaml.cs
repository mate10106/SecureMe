using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SecureMe.Utilities;  // Ensure this is the correct namespace for FileManager
using SecureMe.Models;     // Ensure this is where User is defined

namespace SecureMe.Views
{
    public partial class VerifyRecoveryPhrasePage : Page
    {
        private User _currentUser;
        public List<RecoveryWordInput> InputFields { get; set; }

        public VerifyRecoveryPhrasePage()
        {
            InitializeComponent();
            _currentUser = UserManager.LoadUser();

            // Decrypt stored recovery phrase
            string decryptedPhrase = FileManager.DecryptData(_currentUser.RecoveryPhrase[0]);

            // Convert to a list of words
            var words = decryptedPhrase.Split(' ').ToList();

            // Initialize input fields
            InputFields = words.Select((word, index) => new RecoveryWordInput { Index = index + 1, Word = "" }).ToList();

            DataContext = this;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve user input as a single phrase
            string enteredPhrase = string.Join(" ", InputFields.Select(input => input.Word.Trim()));

            // Get the actual stored phrase
            string decryptedPhrase = FileManager.DecryptData(_currentUser.RecoveryPhrase[0]);

            if (enteredPhrase.Equals(decryptedPhrase, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Recovery phrase verified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Mark recovery phrase as verified
                _currentUser.HasRecoveryPhraseVerified = true;
                UserManager.SaveUser(_currentUser);

                // Navigate to CreateMasterPassword page
                NavigationService.Navigate(new CreateMasterPasswordPage());
            }
            else
            {
                MessageBox.Show("Incorrect recovery phrase. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class RecoveryWordInput
    {
        public int Index { get; set; }
        public string Word { get; set; }
    }
}
