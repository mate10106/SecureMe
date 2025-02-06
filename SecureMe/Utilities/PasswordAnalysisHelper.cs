using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Utilities
{
    public class PasswordAnalysisResult
    {
        public string Strength { get; set; }
        public List<string> Issues { get; set; }
    }
    public static class PasswordAnalysisHelper
    {
        public static PasswordAnalysisResult EvaluatePasswordStrength(string password)
        {
            var result = new PasswordAnalysisResult();
            var issues = new List<string>();

            // Check if the password is too short
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                issues.Add("Password is too short");

            // Check if it has any special characters
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
                issues.Add("No special characters");

            // Optionally check if the password is common (a very small sample list)
            var commonPasswords = new List<string> { "123456", "password", "qwerty", "abc123" };
            if (commonPasswords.Contains(password))
                issues.Add("Common password");

            // Decide the strength based on the issues
            if (issues.Count == 0)
                result.Strength = "Strong";
            else if (issues.Count == 1)
                result.Strength = "Medium";
            else
                result.Strength = "Weak";

            result.Issues = issues;
            return result;
        }
    }
}
