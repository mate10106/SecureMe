using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SecureMe.Models
{
    internal class PasswordManager
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe");
        private static readonly string passwordsFilePath = Path.Combine(appDataFolder, "passwords.dat");

        public static List<Passwords.PasswordEntry> LoadPasswords()
        {
            return LoadPasswords(0, int.MaxValue);
        }
        public static List<Passwords.PasswordEntry> LoadPasswords(int offset, int limit)
        {
            try
            {
                if (!File.Exists(passwordsFilePath))
                {
                    Console.WriteLine($"Passwords file does not exist at: {passwordsFilePath}");
                    return new List<Passwords.PasswordEntry>();
                }

                string encryptedData = File.ReadAllText(passwordsFilePath);
                string decryptedData = SecureMe.Utilities.FileManager.DecryptData(encryptedData);
                List<Passwords.PasswordEntry> allPasswords = JsonConvert.DeserializeObject<List<Passwords.PasswordEntry>>(decryptedData);

                return allPasswords.Skip(offset).Take(limit).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing passwords data: {ex.Message}");
            }
            return new List<Passwords.PasswordEntry>();
        }

        public static void SavePasswords(List<Passwords.PasswordEntry> passwords)
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }
                string json = JsonConvert.SerializeObject(passwords, Formatting.Indented);
                string encryptedData = SecureMe.Utilities.FileManager.EncryptData(json);
                File.WriteAllText(passwordsFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving passwords data: {ex.Message}");
            }
        }

        public static void AddPassword(Passwords.PasswordEntry password)
        {
            List<Passwords.PasswordEntry> passwords = LoadPasswords();
            passwords.Add(password);
            SavePasswords(passwords);
        }

        public static void RemovePassword(Passwords.PasswordEntry password)
        {
            List<Passwords.PasswordEntry> passwords = LoadPasswords();
            passwords.Remove(password);
            SavePasswords(passwords);
        }

        public static void UpdatePassword(Passwords.PasswordEntry password)
        {
            List<Passwords.PasswordEntry> passwords = LoadPasswords();
            var existingPassword = passwords.FirstOrDefault(p => p.Title == password.Title);
            if (existingPassword != null)
            {
                existingPassword.Username = password.Username;
                existingPassword.EncryptedPassword = password.EncryptedPassword;
                existingPassword.URL = password.URL;
                existingPassword.LastUsed = password.LastUsed;
                SavePasswords(passwords);
            }
        }

        public static void UpdatePasswords(Passwords.PasswordEntry updatedEntry)
        {
            List<Passwords.PasswordEntry> passwords = LoadPasswords();
            int index = passwords.FindIndex(p => p.Title == updatedEntry.Title);

            if (index != -1)
            {
                passwords[index] = updatedEntry;
                SavePasswords(passwords);
            }
        }
    }
}
