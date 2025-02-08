using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureMe.Models
{
    public class SecureNotes
    {
        public string Title { get; set; }
        public string SecuredNotes { get; set; }
        public DateTime CreatedNotes { get; set; }

        [JsonIgnore]
        public string Preview =>
            !string.IsNullOrEmpty(SecuredNotes) ?
            (SecuredNotes.Length > 50 ? SecuredNotes.Substring(0, 50) + "..." : SecuredNotes)
            : string.Empty;

        [JsonIgnore]
        public string FormattedDate => CreatedNotes.ToString("MM/dd/yyyy");
    }
}
