﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models
{
    public class _FixedAllowanceCancelManual
    {
         [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please Enter your Description")]
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedMac { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedMac { get; set; }
        public Nullable<int> Active { get; set; }
    }
}