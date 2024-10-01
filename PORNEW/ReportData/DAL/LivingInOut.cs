using System;

namespace ReportData.DAL
{
    public class LivingInOut
    {
        
        public int DIST_CODE { get; set; }
        public int NOKType { get; set; }
        public string NOKID { get; set; }
        public string RefNo { get; set; }
        public string Snumber { get; set; }
        public string DESCRIPTION { get; set; }
        public string NOKChangeTo { get; set; }
        public int District { get; set; }
        public string ItemSendLocation { get; set; }
        public string GSName { get; set; }
        public int PoliceStation1 { get; set; }
        public string Town1 { get; set; }
        public string TownName { get; set; }
        public int NearestTown { get; set; }
        public string NOKName { get; set; }
        public string Relationship { get; set; }
        public string RelationshipName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }      
        public string Authority { get; set; }
        public DateTime WEFDate { get; set; }      
        public int GSnumber { get; set; }
        public string NOKaddress { get; set; }
        public string MStatusID { get; set; }       
        public int StatusID { get; set; }        
        public string EstablishmentId { get; set; }
        public string CategotyName { get; set; }
        public int RelationshipID { get; set; }
        public string Remarks { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedMachine { get; set; }


    }
}