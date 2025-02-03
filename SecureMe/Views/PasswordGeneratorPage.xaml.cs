using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureMe.Views
{
    /// <summary>
    /// Interaction logic for PasswordGeneratorPage.xaml
    /// </summary>
    public partial class PasswordGeneratorPage : Page
    {
        public PasswordGeneratorPage()
        {
            InitializeComponent();
            UpdatePassword(null, null);
        }

        private readonly Random _random = new Random();

        private void UpdatePassword(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            int length = (int)PasswordLengthSlider.Value;
            bool includeUppercase = UppercaseCheckBox.IsChecked ?? false;
            bool includeLowercase = LowercaseCheckBox.IsChecked ?? false;
            bool includeNumbers = NumbersCheckBox.IsChecked ?? false;
            bool includeSymbols = SymbolsCheckBox.IsChecked ?? false;

            string newPassword = GeneratePassword(length, includeUppercase, includeLowercase, includeNumbers, includeSymbols);

            GeneratedPasswordTextBox.Text = newPassword;

            // Ensure the slider matches the actual password length
            PasswordLengthSlider.Value = newPassword.Length;
            PasswordLengthText.Text = newPassword.Length.ToString();
        }


        private string GeneratePassword(int length, bool uppercase, bool lowercase, bool numbers, bool symbols)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string symbolChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            string charSet = "";
            if (uppercase) charSet += upperChars;
            if (lowercase) charSet += lowerChars;
            if (numbers) charSet += numberChars;
            if (symbols) charSet += symbolChars;

            if (charSet.Length == 0) return "Select at least one option!";

            return new string(Enumerable.Repeat(charSet, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private void CopyPasswordToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GeneratedPasswordTextBox.Text);
            MessageBox.Show("Password copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GeneratedPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordLengthSlider.Value = GeneratedPasswordTextBox.Text.Length;
        }

        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthText != null)
            {
                PasswordLengthText.Text = ((int)e.NewValue).ToString();
            }
        }

        private void ReloadPassword(object sender, RoutedEventArgs e)
        {
            UpdatePassword(null, null);
        }
    }
}
