using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{
    public class MaterialSBUModel
    {
        public Guid id { get; set; }
        public Guid materialid { get; set; }
        public string sbu { get; set; }
    }
}
