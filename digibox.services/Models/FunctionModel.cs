using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class FunctionModel
    {
        public System.Guid id { get; set; }
        public string controller { get; set; }
        public string description { get; set; }
        public string action { get; set; }
        public string param { get; set; }
    }
}
