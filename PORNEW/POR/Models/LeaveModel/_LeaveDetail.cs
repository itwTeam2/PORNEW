using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.LeaveModel
{
    public class _LeaveDetail
    {
         [Key]  
         public int LDID { get; set; }
           
         public Nullable<int> LeaveHeaderId { get; set; }
         [Required(ErrorMessage = "Privilege Leave cann't null")]
         public Nullable<int> PrivilegeLeave { get; set; }
         [Required(ErrorMessage = "Casual Leave cann't null")]
         public Nullable<int> CasualLeave { get; set; }
         [Required(ErrorMessage = "Annual Leave cann't null")]
         public Nullable<int> AnnualLeave { get; set; }
         [Required(ErrorMessage = "Leave Leave cann't null")]
         public Nullable<int> LeaveLeave { get; set; }
        // [Required(ErrorMessage = "Weekend cann't null")]
         public Nullable<decimal> Weekend { get; set; }
         [Required(ErrorMessage = "Public Holiday cann't null")]
         public Nullable<int> PublicHoliday { get; set; }

         public Nullable<int> Re_engagement { get; set; }        
         
         public Nullable<int> CreatedBy { get; set; }
         public Nullable<System.DateTime> CreatedDate { get; set; }
         public string CreatedMac { get; set; }
         public Nullable<int> ModifiedBy { get; set; }
         public Nullable<System.DateTime> ModifiedDate { get; set; }
         public string ModifiedMac { get; set; }
         public Nullable<int> Active { get; set; }
         public string Weekend_Disc { get; set; }

    }
}