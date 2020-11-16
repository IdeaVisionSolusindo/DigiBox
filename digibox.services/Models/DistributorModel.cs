using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class DistributorModel
    {
        public System.Guid id { get; set; }

        [Display(Name ="Name")]
        [Required]
        public string name { get; set; }
        [Display(Name ="Address")]
        public string address { get; set; }
        [Display(Name ="Phone")]
        public string telp { get; set; }
    }
}
