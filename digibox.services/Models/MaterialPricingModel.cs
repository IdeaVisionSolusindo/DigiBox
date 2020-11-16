using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class MaterialPricingModel
    {
        //material id
        public Guid id { get; set; }
//material partno
        public string partno { get; set; }
//material name
        public string name { get; set; }
        public string distributor { get; set; }
        public decimal?  currentprice{ get; set; }

        //data untuk proposed new pricelist
        public decimal?  proposedprice{ get; set; }
        public string status { get; set; }
        public DateTime proposedtime { get; set; }

        public bool myproposed { get; set; }
    }

    public class MaterialPriceListModel
    {

        public System.Guid id { get; set; }

        [Display(Name = "Material")]
        [Required]
        public System.Guid materialid { get; set; }
        [Display(Name = "Price")]
        [Required]
        public decimal price { get; set; }
        [Display(Name = "Date")]
        [Required]
        public System.DateTime datestart { get; set; }
        public Nullable<System.DateTime> dateend { get; set; }
        public Nullable<bool> isactive { get; set; }
        public string status { get; set; }
        public bool ismyproposal { get; set; }
    }

    public class PriceProposalModel
    {
        public System.Guid id { get; set; }
        [Display(Name = "Date Posted")]
        public System.DateTime ddate { get; set; }
        [Display(Name = "Collector")]
        public System.Guid collectorid { get; set; }
        public string no { get; set; }
        [Display(Name = "Approved/Rejected By")]
        public Nullable<System.Guid> approvedby { get; set; }
        public string status { get; set; }
        [Display(Name = "Distributor")]
        public Nullable<System.Guid> distributorid { get; set; }
        [Display(Name = "Start Date")]
        public DateTime startdate { get; set; }
        [Display(Name = "Due Date")]
        public DateTime enddate { get; set; }
        [Display(Name = "Rejected Reason")]
        public string rejectedreason { get; set; }
    }

    public class PriceProposalDetailModel
    {
        public System.Guid id { get; set; }
        public System.Guid priceid { get; set; }
        public System.Guid materialid { get; set; }
        public decimal currentprice { get; set; }
        public Nullable<decimal> newprice { get; set; }
        public string  createdby { get; set; }
    }

    public class PriceProposalListModel
    {
        public System.Guid id { get; set; }
        [Display(Name = "Date")]
        public System.DateTime ddate { get; set; }
        [Display(Name = "Collector")]
        public string collector { get; set; }
        public string no { get; set; }
        [Display(Name = "Approved/Rejected By")]
        public string approvedby { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        public string distributor { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string rejectedreason { get; set; }


    }

}
