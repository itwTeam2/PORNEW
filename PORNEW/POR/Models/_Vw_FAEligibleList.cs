using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _Vw_FAEligibleList
    {
        [Key]
        public string ServiceNo { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string AllowanceName { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string SNo { get; set; }
        public int ELFAID { get; set; }
        public string PostedLocation { get; set; }
        public Nullable<int> RankID { get; set; }
        public Nullable<int> AllowanceId { get; set; }
        public Nullable<int> Active { get; set; }
        public string ReferanceNo { get; set; }
        public string CancelReferanceNo { get; set; }
    }
}