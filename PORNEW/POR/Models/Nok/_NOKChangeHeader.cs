using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.Nok
{
    public class _NOKChangeHeader
    {
        public int NOKCDID { get; set; }
        public int NOKCHID { get; set; }
        public Nullable<int> NOKChangeHeadrerID { get; set; }
        [Required(ErrorMessage = "NOK Address is reqiured.")]
        public string NOKAddress { get; set; }
        [Required(ErrorMessage = "NOK Name is reqiured.")]
        public string NOKName { get; set; }
        public string NOKChangeTo { get; set; }
        [Required(ErrorMessage = "Please select the With effect Date.")]
        public Nullable<System.DateTime> WFDate { get; set; }
        public string ServiceNo { get; set; }
        public string Location { get; set; }
        public string Rank { get; set; }
        public string FullName { get; set; }
        public string Branch { get; set; }
        public string Trade { get; set; }
        public string RefNo { get; set; }
        public string RejectRefNo { get; set; }
        public int DistrictName { get; set; }
        [Required(ErrorMessage = "Please Select the District.")]
        public string District { get; set; }
        public int DistrictID { get; set; }
        [Required(ErrorMessage = "Please Select the GS Division.")]
        public string GSDivision { get; set; }
        public string GSName { get; set; }
        [Required(ErrorMessage = "Please Select the Nearest Town.")]
        public string NearestTown { get; set; }
        public string Town1 { get; set; }
        [Required(ErrorMessage = "Please Select the Police Station.")]
        public string PoliceStation { get; set; }
        public string PoliceStation1 { get; set; }
        public string PostOffice { get; set; }
        public string PostOfficeName { get; set; }
        [Required(ErrorMessage = "NOK Authority is reqiured.")]
        public string Authority { get; set; }
        public string RelationshipName { get; set; }
        public string Remarks { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<int> RSID { get; set; }
        public int CurrentUserRole { get; set; }
        public string Comment { get; set; }
        public string RejectRoleName { get; set; }
        public Nullable<int> FMSID { get; set; }
        public int FSNOKCDID { get; set; }
        public int PreviousReject { get; set; }
        public string RejectAuth { get; set; }
        public string EditedDistrict1 { get; set; }
        public string RoleName { get; set; }
        public string EditedGSnumber { get; set; }
        public string EditPoliceStation { get; set; }
        public string EditPostOfficeName { get; set; }
        //public Nullable<int> CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public string CreatedMac { get; set; }
        //public Nullable<int> ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public string ModifiedMac { get; set; }
        //public Nullable<int> Active { get; set; }
        //public string CreateIpAddess { get; set; }
    }
}