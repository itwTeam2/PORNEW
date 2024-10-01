using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportData.BAL
{
    public class BAL_PsnContactHeader
    {
        public int PCHID { get; set; }
        public int FSPCID { get; set; }
        public string ServiceNo { get; set; }
        public string SNO { get; set; }
        public int ServiceType { get; set; }
        public string Rank { get; set; }
        public string FullName { get; set; }
        public string Trade { get; set; }
        public string Remarks { get; set; }
        public string RefNo { get; set; }
        public string Location { get; set; }
        public int RSID { get; set; }
        
        public int MasSubCatID { get; set; }
        public string SubCatName { get; set; }
       
        public string Authority { get; set; }
        public int PreviousReject { get; set; }
        public string RejectAuth { get; set; }
        public int ISProcessed { get; set; }
        public string Comment { get; set; }
        public string RejectRoleName { get; set; }
        public string UserRoleName { get; set; }

        public string RejectRefNo { get; set; }
        public int CurrentUserRole { get; set; }

        
        public string MobileNo { get; set; }
      

        public string ResidentialTeleNo { get; set; }
       
        public string EmailAddress { get; set; }
        /// <summary>
        ///  Child Details
        /// </summary>

        public Nullable<int> PCDID { get; set; }
        public string ChildFullName { get; set; }
        public string ChildFullNameWithInitial { get; set; }
        public string Disrict2 { get; set; }
        public string Disrict3 { get; set; }
        public string BirthPlace { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<int> District { get; set; }
        public string GenderType { get; set; }
        public int Gender { get; set; }
        public string BirthCertificateNo { get; set; }
        public string DeathCertificateNo { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> DateOfDeath { get; set; }
        public Nullable<int> SCID { get; set; }
        public Nullable<int> FMSID { get; set; }
        public DateTime ProcessedDate { get; set; }
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