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
    
    public partial class ProcessHistory
    {
        public int Id { get; set; }
        public Nullable<int> PORId { get; set; }
        public string ItemCode { get; set; }
        public string ServiceNumber { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Authority { get; set; }
        public string Category { get; set; }
        public string ProcessType { get; set; }
        public string Description { get; set; }
        public string ProcessedBy { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
    }
}
