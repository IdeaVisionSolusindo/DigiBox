using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class RoleFunctionModel
    {
        public Guid id { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
    }
}
