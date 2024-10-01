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
    
    public partial class Vw_RejectLeave
    {
        public string ServiceNo { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string PaymentTypeName { get; set; }
        public string ServiceCategoryName { get; set; }
        public string LeaveCategoryName { get; set; }
        public int LHID { get; set; }
        public Nullable<int> ServiceCategoryId { get; set; }
        public string Sno { get; set; }
        public Nullable<int> LeaveCategoryId { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public Nullable<int> PaymentTypeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Authority { get; set; }
        public Nullable<int> PorStatus { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
        public Nullable<int> Active { get; set; }
        public string Formation { get; set; }
        public string EstablishmentId { get; set; }
        public Nullable<int> RankID { get; set; }
        public int LFSID { get; set; }
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> RecordStatusId { get; set; }
        public string Comment { get; set; }
        public Nullable<int> CurrentStatus { get; set; }
        public Nullable<int> RejectStatus { get; set; }
        public Nullable<int> SubmitStatus { get; set; }
        public Nullable<int> Expr1 { get; set; }
        public string DeivisionId { get; set; }
        public string DivisionId { get; set; }
        public string EstablishmentId_ { get; set; }
        public Nullable<int> PrivilegeLeave { get; set; }
        public Nullable<int> CasualLeave { get; set; }
        public Nullable<int> AnnualLeave { get; set; }
        public Nullable<int> LeaveLeave { get; set; }
        public Nullable<decimal> Weekend { get; set; }
        public Nullable<int> PublicHoliday { get; set; }
        public Nullable<int> ReEngagementLeave { get; set; }
    }
}
