using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class OuterViewModel
    {

        public InnerViewModel InnerViewModel { get; set; }
    }

    public class InnerViewModel
    {
        public string Service_Type { get; set; }
        public Nullable<int> ServiceType { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string AllowanceName { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string CampAuthority { get; set; }
        public Nullable<System.DateTime> CampAuthorityDate { get; set; }
        public string Remark { get; set; }
        public Nullable<int> AllowanceId { get; set; }
        public Nullable<int> FADID { get; set; }
        public string Sno { get; set; }
        public string ServiceNo { get; set; }
        public Nullable<int> RankID { get; set; }
        public string Description { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> UserRoleID { get; set; }
        public string EstablishmentId { get; set; }
        public Nullable<int> RejectStatus { get; set; }
        public Nullable<int> CurrentStatus { get; set; }
        public Nullable<int> SubmitStatus { get; set; }
        public Nullable<int> FullHalfPay { get; set; }
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public int ALWID { get; set; }
        public int FADFID { get; set; }
        public Nullable<int> RecordStatusId { get; set; }
        public string Comment { get; set; }
    }
}