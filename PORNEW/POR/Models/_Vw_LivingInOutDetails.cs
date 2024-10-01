using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _Vw_LivingInOutDetails
    {
        public int InOut_ID { get; set; }
        public Nullable<int> NOK_DTLS_ID { get; set; }
        public string Sno { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Location { get; set; }
        public string Authority { get; set; }
        public Nullable<int> InOut_CAT_ID { get; set; }
        public string CategotyShortName { get; set; }
        public string Ref_No { get; set; }
        public Nullable<int> RID { get; set; }
        public Nullable<int> UID { get; set; }
        public string PoliceStation { get; set; }
        public string District { get; set; }
        public string GSDivision { get; set; }
        public int LFSID { get; set; }
        public Nullable<int> RSID { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> NOKchangeStatus { get; set; }
        public string Relationship { get; set; }
        public string NearestTown { get; set; }
        public string NOKAddress { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> RejectStatus { get; set; }
        public Nullable<int> CurrentStatus { get; set; }
        public Nullable<int> SubmitStatus { get; set; }
        public Nullable<int> UserRoleID { get; set; }
        public Nullable<int> Expr1 { get; set; }
        public string Qurter_ID { get; set; }
        public string QuarterLocation { get; set; }
        public Nullable<int> Expr2 { get; set; }
        public string DivisionId { get; set; }
        public string LDRemarks { get; set; }
    }
}