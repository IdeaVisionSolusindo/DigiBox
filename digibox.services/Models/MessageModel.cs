using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class MessageModel
    {
        public System.Guid id { get; set; }
        public Nullable<System.Guid> fromid { get; set; }
        public Nullable<System.Guid> toid { get; set; }
        public string messages { get; set; }
        public Nullable<bool> isdismissed { get; set; }
        public Nullable<System.DateTime> dateposted { get; set; }
        public Nullable<System.Guid> billoflandingid { get; set; }

    }

    public class MessageListModel
    {
        public System.Guid id { get; set; }
        public string fromName { get; set; }
        public string toName { get; set; }
        public string messages { get; set; }
        public Nullable<bool> isdismissed { get; set; }
        public Nullable<System.DateTime> dateposted { get; set; }
    }
}
