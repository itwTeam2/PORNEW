using PagedList;
using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class AnnualLeavePlanController : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        AnnualLeavePlan ObjAnnualLeavePlan = new AnnualLeavePlan();
        //
        // GET: /AnnualLeavePlan/
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DDL_LeaveCategories = new SelectList(_db.LeaveCategories.Where(x => x.Active == 1), "LCID", "LeaveCategoryName");

            return View();
        }

        [HttpPost]
        public ActionResult Create(_AnnualLeavePlan Obj_AnnualLeavePlan)
        {
            ViewBag.DDL_LeaveCategories = new SelectList(_db.LeaveCategories.Where(x => x.Active == 1), "LCID", "LeaveCategoryName");
            try
            {
                if (ModelState.IsValid)
                {
                    ObjAnnualLeavePlan.LeaveCategoryID = Obj_AnnualLeavePlan.LeaveCategoryID;
                    ObjAnnualLeavePlan.Year = Obj_AnnualLeavePlan.Year;
                    ObjAnnualLeavePlan.TotalLeaveDays = Obj_AnnualLeavePlan.TotalLeaveDays;
                    ObjAnnualLeavePlan.TotalLeaveMonth = 0;
                    ObjAnnualLeavePlan.Active = 1;
                    ObjAnnualLeavePlan.CreatedBy = Convert.ToInt32(Session["UID"]);
                    ObjAnnualLeavePlan.CreatedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    ObjAnnualLeavePlan.ModifiedMac = MacAddress;

                    _db.AnnualLeavePlans.Add(ObjAnnualLeavePlan);

                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Save";
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
           
           
            return View();
        }

         [HttpGet]
         public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
         {
             int? UID = Convert.ToInt32(Session["UID"]);
             int pageSize = 0;
             int pageNumber = 1;

             if (UID != 0)
             {
                 var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                 int year = Convert.ToInt32(searchString);
                 ViewBag.CurrentSort = sortOrder;
                 ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
                 ViewBag.DateSortParm = sortOrder == "AllowanceName" ? "Rank" : "EffectiveDate";

                 if (searchString != null)
                 {
                     page = 1;
                 }
                 else
                 {
                     searchString = currentFilter;
                 }

                 ViewBag.CurrentFilter = searchString;

                 List<AnnualLeavePlan> objAnnualLeavePlan = new List<AnnualLeavePlan>();

                 var abc = _db.Vw_LeavePlan.Where(x => x.Year == year).ToList();  
                                     
                  switch (sortOrder)
                 {
                     case "Year":
                         objAnnualLeavePlan = objAnnualLeavePlan.OrderBy(s => s.Year).ToList();
                         break;                     
                 }

                 pageSize = 10;
                 pageNumber = (page ?? 1);
                 return View(abc.ToPagedList(pageNumber, pageSize));
             }
             else
             {
                 return RedirectToAction("Login", "User");
             }

         }
         [HttpGet]
         public ActionResult AnnualViewReport(string searchString)
         {
            
          return View();          

         }
	}
}