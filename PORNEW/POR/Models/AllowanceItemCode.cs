//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POR.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AllowanceItemCode
    {
        public int AICID { get; set; }
        public Nullable<int> AllowanceId { get; set; }
        public Nullable<int> ItemCode { get; set; }
        public Nullable<int> CalculateType { get; set; }
        public Nullable<int> ItemMTypeId { get; set; }
        public Nullable<decimal> Amount { get; set; }
    
        public virtual Allowance Allowance { get; set; }
    }
}
