using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POR.Models.Nok
{
    public class _NOKChangeDetail
    {
        public int NOKCDID { get; set; }
        public int NOKCHID { get; set; }
        public int RSID { get; set; }
        public int FMSID { get; set; }
        
        public Nullable<int> NOKChangeHeadrerID { get; set; }
        public string NOKAddress { get; set; }
        public string NOKName { get; set; }
        public string NOKChangeTo { get; set; }
        public string District { get; set; }
        public string GSDivision { get; set; }
        public string NearestTown { get; set; }
        public string PoliceStation { get; set; }
        public string PostOffice { get; set; }
        public string Authority { get; set; }
        public string Remarks { get; set; }
        public DateTime WFDate { get; set; }
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