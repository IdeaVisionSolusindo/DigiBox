using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
 
    public class DetailReplenishBaseModel
    {
        public System.Guid id { get; set; }

        [Required]
        [Display(Name="Jumlah")]
        public decimal amount { get; set; }
        [Display(Name="Price")]
        public decimal price { get; set; }

    }

    public class DetailReplenishModel: DetailReplenishBaseModel
    {
        public System.Guid replenishid { get; set; }
        [Display(Name="Material")]
        [Required]
        public System.Guid materialid { get; set; }
    }

    public class DetailReplenishListModel : DetailReplenishBaseModel
    {
        public System.Guid materialid { get; set; }
        public string partno { get; set; }
        public string material { get; set; }
        public string unit { get; set; }
        public string rfidcode { get; set; }
    }
}
