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
        private static string userFilePath = "user.dat";

        public static User LoadUser()
        {
            if (File.Exists(userFilePath))
            {
                string json = File.ReadAllText(userFilePath);
                return JsonConvert.DeserializeObject<User>(json);
            }
            return null;
        }

        public static void SaveUser(User user)
        {
            string json = JsonConvert.SerializeObject(user, Formatting.Indented);
            File.WriteAllText(userFilePath, json);
        }

        public static bool UserFileExists()
        {
            return File.Exists(userFilePath);
        }
    }
}
