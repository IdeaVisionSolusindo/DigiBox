using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class DrawBaseModel
    {
        public System.Guid id { get; set; }
        public string no { get; set; }
        public Nullable<System.DateTime> drawdate { get; set; }
        public Nullable<System.Guid> drawerid { get; set; }
        public string status { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public Nullable<System.DateTime> receiveddate { get; set; }
    }
}
