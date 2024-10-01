//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GSQHeader
    {
        public int GSQHID { get; set; }
        public Nullable<long> Sno { get; set; }
        public Nullable<int> ServiceTypeId { get; set; }
        public string Location { get; set; }
        public string GSQLocation { get; set; }
        public string GSQNo { get; set; }
        public Nullable<int> GSQStatus { get; set; }
        public Nullable<System.DateTime> AllocatedDate { get; set; }
        public Nullable<System.DateTime> VacantDate { get; set; }
        public Nullable<System.DateTime> PORDate { get; set; }
        public string RefNo { get; set; }
        public Nullable<short> PreviousReject { get; set; }
        public string RejectAuth { get; set; }
        public Nullable<short> MQRecoveryType { get; set; }
        public Nullable<short> ISSpouseHusbandWrkSLAF { get; set; }
        public Nullable<int> RecordCount { get; set; }
        public string Authority { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
        public Nullable<int> Active { get; set; }
        public string CreateIpAddess { get; set; }
    }
}
