using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{

    public interface IOpnameBaseModel
    {
        System.Guid id { get; set; }
        Nullable<System.DateTime> opnamdate { get; set; }
        Nullable<System.Guid> opnamtype { get; set; }
        string status { get; set; }
        Nullable<System.Guid> checkedby { get; set; }
    }

    public interface IOpnamListModel:IOpnameBaseModel
    {
        string CheckeByName { get; set; }
        string OpnameTypeValue { get; set; }
    }
    public class OpnamModel:IOpnameBaseModel
    {
        public System.Guid id { get; set; }
        public Nullable<System.DateTime> opnamdate { get; set; }
        public Nullable<System.Guid> opnamtype { get; set; }
        public string status { get; set; }
        public Nullable<System.Guid> checkedby { get; set; }

    }

    public class OpnamListModel : IOpnamListModel
    {
        public string CheckeByName { get ; set ; }
        public Guid id { get ; set ; }
        public DateTime? opnamdate { get ; set ; }
        public Guid? opnamtype { get ; set ; }
        public string status { get ; set ; }
        public Guid? checkedby { get ; set ; }
        public string OpnameTypeValue { get; set; }
    }

}

