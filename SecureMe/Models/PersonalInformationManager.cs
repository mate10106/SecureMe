using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace SecureMe.Models
{
    internal class PersonalInformationManager
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe");
        private static readonly string personalInfoFilePath = Path.Combine(appDataFolder, "personalinfo.dat");


        public static PersonalInformation LoadPersonalInformation()
        {
            try
            {
                if (!File.Exists(personalInfoFilePath))
                {
                    Console.WriteLine($"Personal information file does not exist at: {personalInfoFilePath}");
                    return null;
                }

                string encryptedData = File.ReadAllText(personalInfoFilePath);

                string decryptedData = SecureMe.Utilities.FileManager.DecryptData(encryptedData);

                var personalInfo = JsonConvert.DeserializeObject<List<PersonalInformation>>(decryptedData);

                if (personalInfo != null && personalInfo.Count > 0)
                {
                    return personalInfo[0];
                }
                Console.WriteLine("No personal information found in the decrypted data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing personal information data: {ex.Message}");
            }
            return null;
        }

        public static void SavePersonalInformation(PersonalInformation personalInfo)
        {
                try
                {
                    if (!Directory.Exists(appDataFolder))
                    {
                        Directory.CreateDirectory(appDataFolder);
                    }
                    List<PersonalInformation> personalInfos = new List<PersonalInformation> { personalInfo };
                    string json = JsonConvert.SerializeObject(personalInfos, Formatting.Indented);
                    string encryptedData = SecureMe.Utilities.FileManager.EncryptData(json);
                    File.WriteAllText(personalInfoFilePath, encryptedData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving personal information data: {ex.Message}");
                }
        }

    }
}


