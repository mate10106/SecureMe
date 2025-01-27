using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Models
{
    internal class UserManager
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe");
        private static readonly string userFilePath = Path.Combine(appDataFolder, "users.dat");

        public static User LoadUser()
        {
            try
            {
                if (!File.Exists(userFilePath))
                {
                    Console.WriteLine($"User file does not exist at: {userFilePath}");
                    return null;
                }

                string encryptedData = File.ReadAllText(userFilePath);

                string decryptedData = SecureMe.Utilities.FileManager.DecryptData(encryptedData);

                var users = JsonConvert.DeserializeObject<List<User>>(decryptedData);

                if (users != null && users.Count > 0)
                {
                    return users[0];
                }

                Console.WriteLine("No users found in the decrypted data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing user data: {ex.Message}");
            }
            return null;
        }



        public static void SaveUser(User user)
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }

                List<User> users = new List<User> { user };

                string json = JsonConvert.SerializeObject(users, Formatting.Indented);

                string encryptedData = SecureMe.Utilities.FileManager.EncryptData(json);

                File.WriteAllText(userFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }

        public static bool HasMasterPassword()
        {
            User user = LoadUser();
            return user != null && !string.IsNullOrEmpty(user.HashedMasterPassword);
        }

        public static bool UserFileExists()
        {
            return File.Exists(userFilePath);
        }
    }
}
