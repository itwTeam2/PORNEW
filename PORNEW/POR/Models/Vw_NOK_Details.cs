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
    
    public partial class Vw_NOK_Details
    {
        public string NOKID { get; set; }
        public string ActiveNo { get; set; }
        public string SNo { get; set; }
        public int NOKType { get; set; }
        public Nullable<int> Relationship { get; set; }
        public string RelationshipName { get; set; }
        public string NOKName { get; set; }
        public string NOKAddress { get; set; }
        public Nullable<System.DateTime> WEFDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
