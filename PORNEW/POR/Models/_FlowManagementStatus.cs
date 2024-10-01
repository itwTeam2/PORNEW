using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _FlowManagementStatus
    {
        [Key]
        public int FMSID { get; set; }
        public Nullable<int> UserRoleID { get; set; }
        public string EstablishmentId { get; set; }
        public Nullable<int> RejectStatus { get; set; }
        public Nullable<int> CurrentStatus { get; set; }
        public Nullable<int> SubmitStatus { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
    }

    public class FlowStatus
    {
        public _FlowManagementStatus FlowManagementStatus_ { get; set; }
        public _UserRole UserRole_ { get; set; }
    }
}