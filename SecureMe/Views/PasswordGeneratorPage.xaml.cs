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
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            int length = (int)PasswordLengthSlider.Value;
            bool useUppercase = UppercaseCheckBox.IsChecked ?? false;
            bool useLowercase = LowercaseCheckBox.IsChecked ?? false;
            bool useNumbers = NumbersCheckBox.IsChecked ?? false;
            bool useSymbols = SymbolsCheckBox.IsChecked ?? false;

            string password = GenerateSecurePassword(length, useUppercase, useLowercase, useNumbers, useSymbols);
            GeneratedPasswordTextBox.Text = password;
        }

        private string GenerateSecurePassword(int length, bool useUpper, bool useLower, bool useNumbers, bool useSymbols)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string symbolChars = "!@#$%^&*()-_=+[]{};:,.<>?";

            string validChars = "";
            if (useUpper) validChars += upperChars;
            if (useLower) validChars += lowerChars;
            if (useNumbers) validChars += numberChars;
            if (useSymbols) validChars += symbolChars;

            if (string.IsNullOrEmpty(validChars))
                return "Select at least one option!";

            StringBuilder password = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % validChars.Length;
                    password.Append(validChars[index]);
                }
            }

            return password.ToString();
        }

        private void CopyPasswordToClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GeneratedPasswordTextBox.Text);
            MessageBox.Show("Password copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthText != null)
            {
                PasswordLengthText.Text = ((int)e.NewValue).ToString();
            }
        }
    }
}
