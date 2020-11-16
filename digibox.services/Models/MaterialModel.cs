using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{

    public class MaterialBaseModel
    {
        public System.Guid id { get; set; }
        public string partno { get; set; }
        public string name { get; set; }
        public Nullable<decimal> minstock { get; set; }
        public string unit { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public Nullable<System.DateTime> datecreate { get; set; }
        public Nullable<decimal> maxstock { get; set; }
        public string movementtype { get; set; }
        public string materialtype { get; set; }
        public string sbu { get; set; }
        public string distributor { get; set; }
        public string binlocation { get; set; }
        public string plant { get; set; }
        public string sloc { get; set; }
        public string calhorizon { get; set; }

    }
    public class MaterialModel: MaterialBaseModel
    {
        public string partno { get; set; }
        public string name { get; set; }
        public Nullable<decimal> minstock { get; set; }
        public string unit { get; set; }
        public string description { get; set; }
        public Nullable<System.Guid> location { get; set; }
        public Nullable<System.DateTime> datecreate { get; set; }
        public Nullable<decimal> maxstock { get; set; }
        public Nullable<System.Guid> movementtype { get; set; }
        public Nullable<System.Guid> materialtype { get; set; }
        public Nullable<System.Guid> distributor { get; set; }
        public Nullable<System.Guid> binlocation { get; set; }
        public System.Guid[] sbu { get; set; }
        public string plant { get; set; }
        public string sloc { get; set; }
        public string calhorizon { get; set; }
    }

    public class MaterialListModel:MaterialBaseModel
    {
        public Guid collectorid { get; set; }
        public Nullable<decimal> currentstock { get; set; }
    }

    public class MaterialAssignmentModel
    {
        public Guid id { get; set; }
        public bool Assigned { get; set; }
        public string partno { get; set; }
        public string name { get; set; }
        public string distributor { get; set; }

    }


    public class CurrentMaterialStatus:MaterialBaseModel
    {
        public decimal price { get; set; }
        public Nullable<decimal> currentstock { get; set; }

    }
}
