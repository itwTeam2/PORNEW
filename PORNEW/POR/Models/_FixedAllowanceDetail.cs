using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _FixedAllowanceDetail
    {
        [Key]
        public int FADID { get; set; }

        [Required(ErrorMessage = "The value 'SELECT' is not valid for Service Category.")]        
        public Nullable<int> ServiceCategoryId { get; set; }
        public string RankName { get; set; }
        public string FullName { get; set; }
        public Nullable<int> RankId { get; set; }
        [Required(ErrorMessage = "The value 'SELECT' is not valid for Allowance Category.")]
        public Nullable<int> AllowanceCategoryID { get; set; }
        [Required(ErrorMessage = "The value 'SELECT' is not valid for Payment Type.")]
        public Nullable<int> FullHalfPay { get; set; }         
        public Nullable<int> ServiceType { get; set; }

        [Required(ErrorMessage = "The value 'SELECT' is not valid for Allowance.")]
        public Nullable<int> AllowanceId { get; set; }         
        public string Sno { get; set; }

        [Required(ErrorMessage = "Effective Date cann't Null")]
        public Nullable<System.DateTime> EffectiveDate { get; set; }        
        public Nullable<System.DateTime> EndDate { get; set; }
        [Required(ErrorMessage = "Authority cann't Null")]
        public string CampAuthority { get; set; }
        [Required(ErrorMessage = "Authority Date cann't Null")]
        public Nullable<System.DateTime> CampAuthorityDate { get; set; }
        public string Remark { get; set; }
        [Required(ErrorMessage = "Service No cann't Null")]
        public string ServiceNo_ { get; set; }

        public string LocationId { get; set; }

        public int ATID { get; set; }
        public string AllowanceDescreption { get; set; }
        public Nullable<int> Status { get; set; }
    }
}