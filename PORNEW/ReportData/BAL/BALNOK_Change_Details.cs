﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportData.BAL
{
    public class BALNOK_Change_Details
    {
        public string NOKID { get; set; }
        public string SNo { get; set; }
        public int NOKType { get; set; }
        public Nullable<int> Relationship { get; set; }
        public string NOKName { get; set; }
        public string NOKAddress { get; set; }
        public Nullable<int> District { get; set; }
        public Nullable<int> GramaseDiv { get; set; }
        public string GramaseDivName { get; set; }
        public Nullable<int> NearPoliceSta { get; set; }
        public Nullable<int> NearTown { get; set; }
        public int NearPostOff { get; set; }
        public string P2NearPostOff { get; set; }
        public int PresentprovinceId { get; set; }
        public Nullable<System.DateTime> WEFDate { get; set; }
        public string AuthRefNo { get; set; }
        public int SequenceOrder { get; set; }
        public int AliveDeceased { get; set; }

        public string PORRefNo { get; set; }
        public Nullable<int> LivingStatus { get; set; }
        public string CreatedUser { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedUser { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string CreatedMachine { get; set; }
        public string ModifiedMachine { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> GsqStatus { get; set; }
    }
}