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
        private static readonly byte[] GlobalEncryptionKey = Encoding.UTF8.GetBytes("YourGlobalKey1234567890123456");

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
            List<dynamic> users = new List<dynamic>();

            string encryptedData = ReadData();
            if (!string.IsNullOrEmpty(encryptedData))
            {
                string jsonData = DecryptData(encryptedData);
                users = JsonConvert.DeserializeObject<List<dynamic>>(jsonData) ?? new List<dynamic>();
            }

            string userKey = GenerateSecureKey();

            var newUser = new
            {
                Username = username,
                HashedPassword = hashedPassword,
                EncryptionKey = userKey
            };

            users.Add(newUser);

            string updatedJsonData = JsonConvert.SerializeObject(users);
            string updatedEncryptedData = EncryptData(updatedJsonData);
            SaveData(updatedEncryptedData);

            Console.WriteLine("User registered successfully.");
        }

        public static string LoginUser(string username, string hashedPassword)
        {
            string encryptedData = ReadData();
            if (string.IsNullOrEmpty(encryptedData)) return null;

            string jsonData = DecryptData(encryptedData);
            var users = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

            var user = users?.Find(u => u.Username == username && u.HashedPassword == hashedPassword);

            return user?.EncryptionKey;
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

        public static string ReadData()
        {
            return File.Exists(FilePath) ? File.ReadAllText(FilePath) : null;
        }

        private static string EncryptData(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GlobalEncryptionKey;
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length);

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        private static string DecryptData(string encryptedText)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = GlobalEncryptionKey;

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, iv.Length); // Extract IV from the input
                    aes.IV = iv;

                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
