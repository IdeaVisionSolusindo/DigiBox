using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{

    public class ReplenishBaseModel
    {
        public System.Guid id { get; set; }

        [Display(Name = "No")]
        public string no { get; set; }
        [Display(Name = "Post Date")]
        public Nullable<System.DateTime> indate { get; set; }
        public string status { get; set; }
        [Display(Name = "Received Date")]
        public Nullable<System.DateTime> receiveddate { get; set; }
    }
    public class ReplenishModel: ReplenishBaseModel
    {
        public Nullable<System.Guid> collectorid { get; set; }
        public Nullable<System.Guid> receivedbyid { get; set; }
    }

    public class ReplenishModelListModel : ReplenishBaseModel
    {
        public string collectorName { get; set; }
        public string receiverName { get; set; }
        public  string items { get; set; }
    }


    public class ReplenishDetailBaseModel
    {
        public System.Guid id { get; set; }
        public System.Guid replenishid { get; set; }
        public System.Guid materialid { get; set; }
        public decimal amount { get; set; }
        public decimal price { get; set; }
        public string rfidcode { get; set; }

    }

    public class ReplenishDetailModel: ReplenishDetailBaseModel
    {

    }

    public class ReplenishDetailReceiveModel: ReplenishDetailBaseModel
    {
        public Nullable<System.DateTime> receivedate { get; set; }
        public Nullable<decimal> receiveamount { get; set; }

    }
}
