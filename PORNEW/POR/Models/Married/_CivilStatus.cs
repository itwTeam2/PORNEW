using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POR.Models.Married
{
    public class _CivilStatus
    {
        public long Snumber { get; set; }
        public string ServiceNo { get; set; }
        public string Name { get; set; }
        public string Trade { get; set; }
        public int CSHID { get; set; }
        public int CurrentUserRole { get; set; }
        public string UserRole { get; set; }

        public string CategoryName { get; set; }

        public string SpouseName { get; set; }
        public string SpousNICNo { get; set; }
        public string SpousOccupation { get; set; }
        public string SpousOffcialAddress { get; set; }
        public int CSCID { get; set; }
        public int Marriage_Status { get; set; }
        public Nullable<System.DateTime> MarriageDate { get; set; }
        public Nullable<System.DateTime> WEFDate { get; set; }
        public Nullable<System.DateTime> NOKWDate { get; set; }
        public string RegistarOfficeLocation { get; set; }
        public string MarriageCertificateNo { get; set; }
        //public Nullable<System.DateTime> DivorceDate { get; set; }
        public DateTime DivorceDate { get; set; }
        public string DivorceLocation { get; set; }
        //public Nullable<System.DateTime> DivorceCaseDate { get; set; }
        public DateTime DivorceCaseDate { get; set; }
        public string CourtCaseNo{ get; set; }       
        public string DeathCertificateNo { get; set; }
        public DateTime? DateofDecease { get; set; }
        public DateTime? RecordCreatedDate { get; set; }
        
        public string NOKName { get; set; }
        public string NOKChangeTo { get; set; }
        public string Rank { get; set; }
        public string DESCRIPTION { get; set; }
        public int District { get; set; }
        public string District1 { get; set; }
        public string PostOfficeName { get; set; }
        public string EditPostOfficeName { get; set; }
        public string RejectRoleName { get; set; }
        public int PreviousReject { get; set; }
        public string RejectAuth { get; set; }

        public string GSName { get; set; }
        public string PoliceStation1 { get; set; }
        public string EditPoliceStation { get; set; }

        public string Town1 { get; set; }

        public string NearestTown { get; set; }

        public string RelationshipName { get; set; }
       

        public string Authority { get; set; } 
        public string Location { get; set; }
        public string GSnumber { get; set; }
        public string NOKaddress { get; set; }       
        public string Comment { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> RSID { get; set; }
        public string RefNo { get; set; }
        public string RejectRefNo { get; set; }
        
        #region EditProperty
        public string EditCategoryName { get; set; }
        public DateTime? EditDivorceCaseDate { get; set; }
        public DateTime? EditDivorceDate { get; set; }
        public string EditDistrict { get; set; }
        public string EditNokChange { get; set; }
        #endregion

    }
}