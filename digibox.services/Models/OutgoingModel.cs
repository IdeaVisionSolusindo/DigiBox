using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
     interface IOutgoingBaseModel
    {
         System.Guid id { get; set; }
         Nullable<System.Guid> drequestid { get; set; }
         Nullable<decimal> amount { get; set; }
        Nullable<System.Guid> inventoryid { get; set; }
    }

    public class OutgoingModel : IOutgoingBaseModel
    {
        public Guid id { get ; set ; }
        public Guid? drequestid { get ; set ; }
        public decimal? amount { get ; set ; }
        public Nullable<System.Guid> inventoryid { get; set; }
        public string rfidcode { get; set; }
        public Guid materialid { get; set; }
    }
}
