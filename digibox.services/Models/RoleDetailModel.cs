using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class RoleDetailModel
    {
        public System.Guid id { get; set; }
        public System.Guid roleid { get; set; }
        public System.Guid functionid { get; set; }
        [Display (Name = "Role Menu")]
        public string Name { get; set; }
        public bool isinList { get; set; }
    }
}
