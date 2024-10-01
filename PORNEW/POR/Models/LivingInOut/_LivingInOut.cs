using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.LivingInOut
{
    public class _LivingInOut
    {
        [Required(ErrorMessage = "Service No is Required.")]
        public string Snumber { get; set; }
        public string CategoryName { get; set; }       
        
        public long ServiceNo { get; set; }
        [Required(ErrorMessage = "Name is Required.")]
        public string FullName { get; set; }
        public string RefNo { get; set; }
        public string RejectRefNo { get; set; }        
        public int LIOHID { get; set; }
        public int CurrentUserRole { get; set; }
        public string UserRole { get; set; }
        public int NOKchangeStatus { get; set; }
        
        [Required(ErrorMessage = "Please Select Civil Status.")]
        public int LSID { get; set; }        
        [Required(ErrorMessage = "Rank is Required.")]
        public string Rank { get; set; }
        public string Trade { get; set; }
        public Nullable<int> DIST_CODE { get; set; }       
        public string NOKChangeTo { get; set; }
        public Nullable<System.DateTime> NOKWEFDate { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public string EditedProvince { get; set; }
        public int District { get; set; }
        public string District1 { get; set; }
        public string EditedDistrict1 { get; set; }        
        public string GSName { get; set; }
        public string PoliceStation1 { get; set; }
        public int PoliceStationID { get; set; }
        public string EditPoliceStation { get; set; }
        
        public string Town1 { get; set; }
        public int TownID { get; set; }
        public string PostOfficeName { get; set; }
        public string EditPostOfficeName { get; set; }
        public string SpouseName { get; set; }
        public string TownName { get; set; }
        public string NearestTown { get; set; }
        public string NOKName { get; set; }
        public string Marriage_Status { get; set; }
        public string RelationshipName { get; set; }
        public string NOKRelationship1 { get; set; }
        
        public string DESCRIPTION { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime FromSixMonth { get; set; }
        public DateTime RecordCreatedDate { get; set; }
        
        public DateTime ToDate { get; set; }
        [Required(ErrorMessage = "Authority is Required.")]
        public string NokID { get; set; }
        public string Authority { get; set; }        
        public string Location { get; set; }
        public int GSnumber { get; set; }
        public string EditedGSnumber { get; set; }
        
        public string NOKaddress { get; set; }       
        public string CategotyShortName { get; set; }       
        public int InOut_CAT_ID { get; set; }        
        public int RelationshipID { get; set; }
        public string Remarks { get; set; }        
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> RSID { get; set; }
        public string Comment { get; set; }
        public string RejectRoleName { get; set; }
        public int PreviousReject { get; set; }
        public string RejectAuth { get; set; }

    }

}