using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SecureMe.Utilities
{
    public static class FileManager
    {
        private static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe");
        private static readonly string FilePath = Path.Combine(AppDataFolder, "users.dat");
        private static readonly byte[] GlobalEncryptionKey = new byte[32];

        public static void InitializeStorage()
        {
            if (GlobalEncryptionKey.Length != 16 && GlobalEncryptionKey.Length != 24 && GlobalEncryptionKey.Length != 32)
            {
                throw new ArgumentException("Invalid key length for AES encryption.");
            }

            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            if (!File.Exists(FilePath))
            {
                File.Create(FilePath).Dispose();
            }
        }

        public static void RegisterUser(string username, string hashedPassword)
        {
            string userKey = GenerateSecureKey();

            var userData = new
            {
                Username = username,
                HashedPassword = hashedPassword,
                EncryptionKey = userKey
            };

            string jsonData = JsonConvert.SerializeObject(userData);
            string encryptedData = EncryptData(jsonData);
            SaveData(encryptedData);

            Console.WriteLine("User registered successfully.");
        }

        public static string LoginUser(string username, string hashedPassword)
        {
            string encryptedData = ReadData();
            if (string.IsNullOrEmpty(encryptedData)) return null;

            string jsonData = DecryptData(encryptedData);
            var userData = JsonConvert.DeserializeObject<dynamic>(jsonData);

            if (userData.Username == username && userData.HashedPassword == hashedPassword)
            {
                return userData.EncryptionKey;
            }

            return null;
        }

        private static string GenerateSecureKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] keyBytes = new byte[32];
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        private static void SaveData(string data)
        {
            File.WriteAllText(FilePath, data);
        }

        private static string ReadData()
        {
            return File.Exists(FilePath) ? File.ReadAllText(FilePath) : null;
        }

        private static string EncryptData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GlobalEncryptionKey;
                aes.IV = new byte[16];

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                        writer.Close();
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private static string DecryptData(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GlobalEncryptionKey;
                aes.IV = new byte[16];

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
