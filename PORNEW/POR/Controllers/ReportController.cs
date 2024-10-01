using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        public ActionResult Index(string Month, string Year)
        {
            if (Month != null || Year != null)
            {
                return Redirect("~/Report/FixedAllowanceReport.aspx?Month=" + Month + "&Year=" + Year + "");
            }
            else
            {
                return View();
            }

        }
	}
}