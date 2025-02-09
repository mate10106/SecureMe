using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SecureMe.Utilities;

namespace SecureMe.Models
{
    internal class SecureNotesManager
    {
        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureMe");
        private static readonly string secureNotesFilePath = Path.Combine(appDataFolder, "securenotes.dat");

        public static List<SecureNotes> LoadSecureNotes()
        {
            return LoadSecureNotes(0, int.MaxValue);
        }

        public static List<SecureNotes> LoadSecureNotes(int offset, int limit)
        {
            try
            {
                if (!File.Exists(secureNotesFilePath))
                {
                    Console.WriteLine($"SecureNotes file does not exist at: {secureNotesFilePath}");
                    return new List<SecureNotes>();
                }

                string encryptedData = File.ReadAllText(secureNotesFilePath);
                string decryptedData = FileManager.DecryptData(encryptedData);
                List<SecureNotes> allSecureNotes = JsonConvert.DeserializeObject<List<SecureNotes>>(decryptedData);
                return allSecureNotes.Skip(offset).Take(limit).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing secure notes data: {ex.Message}");
            }
            return new List<SecureNotes>();
        }

        public static void SaveSecureNotes(List<SecureNotes> secureNotes)
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                {
                    Directory.CreateDirectory(appDataFolder);
                }
                string json = JsonConvert.SerializeObject(secureNotes, Formatting.Indented);
                string encryptedData = FileManager.EncryptData(json);
                File.WriteAllText(secureNotesFilePath, encryptedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving secure notes data: {ex.Message}");
            }
        }

        public static void AddSecureNote(SecureNotes note)
        {
            List<SecureNotes> notes = LoadSecureNotes();
            notes.Add(note);
            SaveSecureNotes(notes);
        }

        public static void UpdateSecureNote(SecureNotes note)
        {
            List<SecureNotes> notes = LoadSecureNotes();
            int index = notes.FindIndex(n => n.Title == note.Title);
            if (index != -1)
            {
                notes[index] = note;
                SaveSecureNotes(notes);
            }
        }

        public static void RemoveSecureNote(SecureNotes note)
        {
            List<SecureNotes> notes = LoadSecureNotes();
            var noteToRemove = notes.FirstOrDefault(n => n.Title == note.Title && n.SecuredNotes == note.SecuredNotes);

            if (noteToRemove != null)
            {
                notes.Remove(noteToRemove);
                SaveSecureNotes(notes);
            }
        }
    }
}
