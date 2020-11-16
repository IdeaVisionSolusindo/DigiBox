using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class AttributeModel
    {
        public System.Guid id { get; set; }
        public string attributename { get; set; }
        
        [Display(Name ="Name")]
        [Required]
        public string attributevalue { get; set; }
        public Nullable<int> sortnumber { get; set; }
        [Display(Name ="Description")]
        public string description { get; set; }
        public Nullable<bool> isshown { get; set; }
    }
}
