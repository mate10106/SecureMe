using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Utilities
{
    internal class EncryptionHelper
    {
        public static string GenerateEncryptionKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32]; 
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }
    }
}
