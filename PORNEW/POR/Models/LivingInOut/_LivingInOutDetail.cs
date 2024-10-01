using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.LivingInOut
{
    public class _LivingInOutDetail
    {
        [Key]
        public int InOut_ID { get; set; }
        public string Sno { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Location { get; set; }
        public string Authority { get; set; }
        public Nullable<int> NOKchangeStatus { get; set; }
        public int InOut_CAT_ID { get; set; }
        public Nullable<int> NOKType_ID { get; set; }
        public string Ref_No { get; set; }
        public Nullable<int> RID { get; set; }
        public Nullable<int> UID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Active { get; set; }
        public string Remarks { get; set; }
    }
}