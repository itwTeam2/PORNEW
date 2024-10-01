using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace POR.Models.LivingInOut
{
    public class _GSQHeader
    {
        [Key]
        public int GSQHID { get; set; }
        public string Snumber { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public long Sno { get; set; }
        public string Trade { get; set; }
        public int FMSID { get; set; }
        public int FSGSQID { get; set; }        
        public string ServiceNo { get; set; }
        public string Authority { get; set; }
        public string FullName { get; set; }
        public string Comment { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string MQRecoveryType { get; set; }
        public string SpouseHusbandWorking { get; set; }
        public string Service_TypeFull { get; set; }
        public int StatusName { get; set; }
        public int EditStatusName { get; set; }
        public string GSQStatusName { get; set; }
        public string RoleName { get; set; }
        public string EstablishmentId { get; set; }

        [Required(ErrorMessage = "Please select the posted location")]
        public string Location { get; set; }
        public string EditLocation { get; set; }
        public int CurrentUserRole { get; set; }
        public string GSQNo { get; set; }
        public string GSQLocation { get; set; }
        public int GSQStatus { get; set; }
        public Nullable<System.DateTime> AllocatedDate { get; set; }
        public Nullable<System.DateTime> VacantDate { get; set; }
        public Nullable<System.DateTime> PORDate { get; set; }        
        public string RefNo { get; set; }
        public string SlafWrkPersonName { get; set; }
        public int PreviousReject { get; set; }
        public string RejectAuth { get; set; }
        public string RejectRefNo { get; set; }
        public string RejectRoleName { get; set; }
        public Nullable<int> RSID { get; set; }

        ///////////////////// NOK Info
        public DateTime NOKWDate { get; set; }
        public string RelationshipName { get; set; }
        public string NOKRelationship1 { get; set; }
        public string NOKName { get; set; }        
        public string NOKaddress { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int District { get; set; }
        public string DistrictName { get; set; }
        public string GSName { get; set; }
        public string Town1 { get; set; }
        public string NearestTown { get; set; }        
        public string PoliceStation1 { get; set; }
        public string NOKChangeTo { get; set; }        
        public string PostOfficeName { get; set; }
        public string Remarks { get; set; }
        public string SpaouseName { get; set; }
        public string SpaouseServiceNo { get; set; }
        public int SpaouseWrkStatus { get; set; }
        public string DESCRIPTION { get; set; }

        public string EditedProvince { get; set; }
        public string EditedDistrict1 { get; set; }
        public string EditedGSnumber { get; set; }
        public string EditPoliceStation { get; set; }
        public string EditPostOfficeName { get; set; }
    }
}