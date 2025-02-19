using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Models
{
    public class User
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string EncryptionKey { get; set; } 
        public string HashedMasterPassword { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsLoggedIn { get; set; }
        public List<string> RecoveryPhrase { get; set; }
        public bool HasRecoveryPhraseVerified { get; set; }
    }
}
