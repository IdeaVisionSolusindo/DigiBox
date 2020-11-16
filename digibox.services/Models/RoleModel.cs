using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class RoleModel
    {

        public Guid id { get; set; }
         [Display(Name = "Name")]
        public string name { get; set; }
        [Display(Name = "Description")]
        public string description { get; set; }

        public List<RoleDetailModel> detail { get; set; }

    }
}
