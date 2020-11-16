using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
     public class AttachmentModel
    {
        public System.Guid id { get; set; }
        public string attachmenttype { get; set; }
        public Nullable<System.Guid> referenceid { get; set; }
        public byte[] attachment { get; set; }
        public string filename { get; set; }
        public string description { get; set; }
        public string attachmentstep { get; set; }
    }
}
