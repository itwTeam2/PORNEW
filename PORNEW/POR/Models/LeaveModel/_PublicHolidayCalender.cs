using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.LeaveModel
{
    public class _PublicHolidayCalender
    {           
        [Key]
        public int PHCID { get; set; }
        public Nullable<int> HolidayTypeId { get; set; }
        public Nullable<System.DateTime> ApplicableDate { get; set; }
        public string Reason { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
        public Nullable<int> Active { get; set; }
    }
}