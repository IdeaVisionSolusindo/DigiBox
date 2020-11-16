using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Models
{

    #region Interface Request
    public interface IRequestBaseModel
    {
         System.Guid id { get; set; }
        string no { get; set; }
        Nullable<System.DateTime> requestdate { get; set; }
        string status { get; set; }
         DateTime? receiveddate { get; set; }

    }

    public interface IRequestModel : IRequestBaseModel
    {
        Nullable<System.Guid> userid { get; set; }
        Nullable<System.Guid> handedoverby { get; set; }
        Nullable<bool> isdeleted { get; set; }
    }

    public interface IRequestListModel:IRequestBaseModel
    {
        string RequestedByName { get; set; }
        string HandedOverByName { get; set; }
    }

    #endregion

    #region Implementation Interface of Request
    /// <summary>
    /// CLASS IMPLEMENTATION OF INTERFACE
    /// </summary>
    public class RequestModel : IRequestModel
    {
        public Guid id { get; set; }
        public string no { get; set; }
        [Display(Name = "Request Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? requestdate { get; set; }
        [Display(Name = "Requested By")]
        public Guid? userid { get; set; }
        [Display(Name = "Hand Over By")]
        public Guid? handedoverby { get; set; }
        public string status { get; set; }
        public bool? isdeleted { get; set; }
        [Display(Name = "Receive Date")]
        public DateTime? receiveddate { get; set; }
    }

    public class RequestListModel : IRequestListModel
    {
        public string RequestedByName { get; set; }
        public string HandedOverByName { get; set; }
        public Guid id { get; set; }
        public string no { get; set; }
        public DateTime? requestdate { get; set; }
        public string status { get ; set ; }
        public DateTime? receiveddate { get; set; }
    }

    #endregion


    #region Interface Request Detail

    public interface IRequestDetailBaseModel
    {
        System.Guid id { get; set; }
        System.Guid requestid { get; set; }
        System.Guid materialid { get; set; }
        decimal amount { get; set; }
    }

    public interface IRequestDetailModel
    {
        decimal price { get; set; }
        DateTime? receivedate { get; set; }
        decimal? receiveamount { get; set; }
        string rfidcode { get; set; }
    }


    #endregion

    #region Implementation Request Detail
    public class RequestDetailModel : IRequestDetailBaseModel
    {
        public Guid id { get ; set ; }
        public Guid requestid { get ; set ; }
        public Guid materialid { get ; set ; }
        public decimal amount { get ; set ; }

    }

    public class RequestDetailListModel : IRequestDetailBaseModel
    {
        public Guid id { get; set; }
        public Guid requestid { get; set; }
        public Guid materialid { get; set; }
        public decimal amount { get; set; }

        public string partno { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
        public decimal receiveamount { get; set; }
        public DateTime? receivedate { get; set; }
    }

    public class RequestDetailReceiveModel:IRequestDetailBaseModel
    {
        public decimal price { get; set; }
        public DateTime? receivedate { get; set; }
        public decimal? receiveamount { get; set; }
        public string rfidcode { get; set; }
        public Guid id { get ; set ; }
        public Guid requestid { get ; set ; }
        public Guid materialid { get ; set ; }
        public decimal amount { get ; set ; }
    }
    #endregion

}
