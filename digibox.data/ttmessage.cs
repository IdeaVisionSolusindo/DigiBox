//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace digibox.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ttmessage
    {
        public System.Guid id { get; set; }
        public Nullable<System.Guid> fromid { get; set; }
        public Nullable<System.Guid> toid { get; set; }
        public string messages { get; set; }
        public Nullable<bool> isdismissed { get; set; }
        public Nullable<System.DateTime> createdat { get; set; }
        public Nullable<System.DateTime> updatedat { get; set; }
        public Nullable<System.DateTime> deletedat { get; set; }
        public string createdby { get; set; }
        public string updatedby { get; set; }
        public string deletedby { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public Nullable<System.DateTime> dateposted { get; set; }
    }
}
