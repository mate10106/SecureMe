using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SecureMe.Utilities
{
    internal class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            // Hash the input password
            string hashedInputPassword = HashPassword(inputPassword);

            // Compare the hashed input password with the stored hash
            return hashedInputPassword == storedHash;
        }
    }
}
