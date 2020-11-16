using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class InventoryBaseModel
    {
        public System.Guid id { get; set; }
        public Nullable<System.DateTime> createdat { get; set; }

        public System.Guid materialid { get; set; }
        public short inout { get; set; }
        public decimal amount { get; set; }
    }
    public class InventoryModel:InventoryBaseModel
    {
        public string inOutDescription {
            get
            {
                string output = inout == 1 ? "IN" : "OUT";
                return output;
            }
        }
    }
}
