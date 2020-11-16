using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public interface IOpnameDetailBaseModel
    {
        System.Guid id { get; set; }
        System.Guid materialid { get; set; }
        System.Guid opnamid { get; set; }
        Nullable<int> inout { get; set; }
        Nullable<decimal> amount { get; set; }
        string description { get; set; }

    }
    public class OpnameDetail:IOpnameDetailBaseModel
    {
        public System.Guid id { get; set; }
        public System.Guid materialid { get; set; }
        public System.Guid opnamid { get; set; }
        public Nullable<int> inout { get; set; }
        public Nullable<decimal> amount { get; set; }
        public string materialName { get; set; }
        public string inOutName { get; set; }
        public string partno { get; set; }
        public string description { get; set; }
        public string rfidcode { get; set; }
        public string qrimage { get; set; }
        public string barcodeimage { get; set; }

    }
}
