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
            if (File.Exists(userFilePath))
            {
                try
                {
                    string json = File.ReadAllText(userFilePath);
                    Console.WriteLine($"File Content: {json}");
                    return JsonConvert.DeserializeObject<User>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing user data: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"User file does not exist at: {userFilePath}");
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

                string json = JsonConvert.SerializeObject(user, Formatting.Indented);
                File.WriteAllText(userFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }

        public static bool UserFileExists()
        {
            return File.Exists(userFilePath);
        }
    }
}
