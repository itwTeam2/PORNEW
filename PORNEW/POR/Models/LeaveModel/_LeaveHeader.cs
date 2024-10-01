using POR.Models.LeaveModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.LeaveModel
{
    public class _LeaveHeader
    {
        [Key]   
        public int LHID { get; set; }

        [Required(ErrorMessage = "The value 'SELECT' is not valid for Service Category.")]
        public Nullable<int> ServiceCategoryId { get; set; }
        
        public string Sno { get; set; }
        [Required(ErrorMessage = "The value 'SELECT' is not valid for Payment Type.")]
        public Nullable<int> PaymentTypeId { get; set; }

        [Required(ErrorMessage = "The value 'SELECT' is not valid for Leave Category.")]
        public Nullable<int> LeaveCategoryId { get; set; }
        [Range(typeof(DateTime), "2015-01-01", "2030-12-31", ErrorMessage = "Selected date must be between {1} and {2}.")]
        public Nullable<System.DateTime> FromDate { get; set; }
        [Range(typeof(DateTime), "2015-01-01", "2030-12-31", ErrorMessage = "Selected date must be between {1} and {2}.")]
        public Nullable<System.DateTime> ToDate { get; set; }

        [Required(ErrorMessage = "Establishment cann't null")]
        public string EstablishmentId { get; set; }        
        public string Authority { get; set; }
        public Nullable<int> PorStatus { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public Nullable<int> PrivilegeLeave { get; set; }
        public Nullable<int> AccumulatedLeave { get; set; }
        public Nullable<int> Reengagement { get; set; }
        public string DeivisionId { get; set; }
        public Nullable<int> CasualLeave { get; set; }
        public Nullable<int> ReEngagementLeave { get; set; } 
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> HistoryYear { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
        public Nullable<int> Active { get; set; }

        public Nullable<int> LivingStatus { get; set; }

        //Externl Property
        public string ServiceNo_ { get; set; }
   
        

    }

    public class Leave
    {        
        public _LeaveDetail LeaveDetail_ { get; set; }
        public _LeaveHeader LeaveHeader_ { get; set; }
    }
}

