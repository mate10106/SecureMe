using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Models
{
    internal class Passwords
    {
        public class PasswordEntry
        {
            public string Username { get; set; }
            public string Title { get; set; }
            public string EncryptedPassword { get; set; }
            public string URL { get; set; }
            public DateTime LastUsed { get; set; }
        }
    }
}
