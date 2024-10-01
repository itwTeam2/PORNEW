using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POR.Models.F121
{
    public class _F121
    {
        public string Snumber { get; set; }
        public string ServiceNo { get; set; }
        public string SNo { get; set; }
        public string Name { get; set; }
        public string Trade { get; set; }
        public string Rank { get; set; }
        public string LocShortName { get; set; }
        public string OffencePlace { get; set; }
        public Nullable<System.DateTime> ChargeDate { get; set; }
        public int OffenceID { get; set; }
        public string OffenceName { get; set; }
        public string OffenceWASO { get; set; }
        public int ChargeNo { get; set; }
        public string ChargeNo2 { get; set; }
        public Nullable<System.DateTime> OffenceDate { get; set; }
        public string Appointment { get; set; }
        public int PunishmentID { get; set; }

        public string Punishment { get; set; }
        public int Sec40 { get; set; }

        public int OptCM { get; set; }
        public int RecCM { get; set; }
        public string PunishmentDescription { get; set; }
        public Nullable<System.DateTime> PunishDate { get; set; }
        public Nullable<int> FMSID { get; set; }
        public Nullable<int> RSID { get; set; }
        public int CHID { get; set; }

        public string Comment { get; set; }

        public DateTime? fromDate { get; set; }

        public DateTime? toDate { get; set; }

    }
}