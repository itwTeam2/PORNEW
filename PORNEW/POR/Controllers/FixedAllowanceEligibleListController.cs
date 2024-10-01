using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;

namespace POR.Controllers
{
    public class FixedAllowanceEligibleListController : Controller
    {
        dbContext _db = new dbContext();
        //
        // GET: /FixedAllowanceEligibleList/
    
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {               

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "ServiceNo" : "";
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

                List<Vw_FAEligibleList> objEligibleList = new List<Vw_FAEligibleList>();

                objEligibleList = _db.Vw_FAEligibleList.Where(x=>x.Active==1).ToList();  

                if (!String.IsNullOrEmpty(searchString))
                {
                    objEligibleList = objEligibleList.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.Name.Contains(searchString) || s.AllowanceName.Contains(searchString)).Take(500).ToList();
                }

                switch (sortOrder)
                {
                    case "ServiceNo":
                        objEligibleList = objEligibleList.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "Rank":
                        objEligibleList = objEligibleList.OrderBy(s => s.RankID).ToList();
                        break;
                    case "Name":
                        objEligibleList = objEligibleList.OrderBy(s => s.Name).ToList();
                        break;
                    case "AllowanceName":
                        objEligibleList = objEligibleList.OrderBy(s => s.AllowanceName).ToList();
                        break;
                    case "EffectiveDate":
                        objEligibleList = objEligibleList.OrderBy(s => s.EffectiveDate).ToList();
                        break;
                    case "EstablishmentId":
                        objEligibleList = objEligibleList.OrderBy(s => s.PostedLocation).ToList();
                        break;
                    default:
                        objEligibleList = objEligibleList.OrderBy(s => s.ServiceNo).ToList();
                        break;
                }

                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(objEligibleList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                int UID_ = 0;
                if (Session["UID"] != null)
                {
                    UID_ = Convert.ToInt32(Session["UID"]);
                }

                var LocationType = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationType).First();
                var LocationId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).First();
                TempData["EstablishmentId"] = LocationId;
                ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories, "ServiceCategoryId", "ServiceCategoryName");
                ViewBag.DDL_Allowance = new SelectList(_db.Allowances, "ALWID", "AllowanceName");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(_FixedAllowanceEligibleList obj_FixedAllowanceEligibleList)
        {
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories, "ServiceCategoryId", "ServiceCategoryName");
            ViewBag.DDL_Allowance = new SelectList(_db.Allowances, "ALWID", "AllowanceName");

            FixedAllowanceEligibleList objFixedAllowanceEligibleList = new FixedAllowanceEligibleList();
            try
            {

                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_FixedAllowanceEligibleList.ServiceNo_).Select(x => x.SNo).FirstOrDefault();

                //Check Duplicate Entry //Check Database 
                if (obj_FixedAllowanceEligibleList.EffectiveDate == null)
                {
                    var FixDate = "01/01/1800";
                    objFixedAllowanceEligibleList.EffectiveDate = Convert.ToDateTime(FixDate);
                }
                else
                {
                    objFixedAllowanceEligibleList.EffectiveDate = obj_FixedAllowanceEligibleList.EffectiveDate;
                }
                bool Status = false;
                Status = CheckDuplicate(Sno, obj_FixedAllowanceEligibleList.AllowanceId, obj_FixedAllowanceEligibleList.EffectiveDate);
                if (Status == true)
                {
                    TempData["ErrMsg"] = "Duplicate Entry Not Allow. Date Period Overlap";
                }
                else
                {
                    objFixedAllowanceEligibleList.SNo = Sno;
                    objFixedAllowanceEligibleList.AllowanceId = obj_FixedAllowanceEligibleList.AllowanceId;
                    objFixedAllowanceEligibleList.EffectiveDate = obj_FixedAllowanceEligibleList.EffectiveDate;

                    objFixedAllowanceEligibleList.CreatedBy = UID;
                    objFixedAllowanceEligibleList.CreatedDate = DateTime.Now;
                    objFixedAllowanceEligibleList.Active = 1;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixedAllowanceEligibleList.CreatedMac = MacAddress;
                    _db.FixedAllowanceEligibleLists.Add(objFixedAllowanceEligibleList);
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Saved...!";
                        //return RedirectToAction("Create");
                        return View();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        public bool CheckDuplicate(string Sno, int? AllowanceId, DateTime? EffectiveDate)
        {
            bool Status = false;
            try
            {
                int NumRows;

                NumRows = _db.FixedAllowanceEligibleLists.Where(x => x.SNo == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && x.EffectiveDate >= EffectiveDate).Count();

                // abc = _db.FixedAllowanceDetails.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && ((x.EffectiveDate >= EffectiveDate && x.EffectiveDate < EndDate) || (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate) || (x.EndDate <= EndDate))).Select(x => x.FADID);

                if (NumRows >= 1)
                {
                    Status = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Status;

        }
        [HttpGet]
        public ActionResult EditEligibleListPerson(int id)
        {
            _Vw_FAEligibleList _objVw_FAEligibleList = new _Vw_FAEligibleList();

            var FAEligibleList = _db.Vw_FAEligibleList.Where(x => x.ELFAID == id).ToList();
            
            foreach (var item in FAEligibleList)
            {
                _objVw_FAEligibleList.AllowanceName = item.AllowanceName;
                _objVw_FAEligibleList.ServiceNo = item.ServiceNo;
                _objVw_FAEligibleList.Rank = item.Rank;
                _objVw_FAEligibleList.Name = item.Name;
                _objVw_FAEligibleList.EffectiveDate = item.EffectiveDate;
                _objVw_FAEligibleList.ELFAID = id;
            }
            return View(_objVw_FAEligibleList);
        }
        [HttpPost]
        public ActionResult EditEligibleListPerson(_Vw_FAEligibleList _objVw_FAEligibleList)
        {
            FixedAllowanceEligibleList objFixedAllowanceEligibleList = _db.FixedAllowanceEligibleLists.Find(_objVw_FAEligibleList.ELFAID);
          
            objFixedAllowanceEligibleList.EffectiveDate = _objVw_FAEligibleList.EffectiveDate;
            objFixedAllowanceEligibleList.ReferanceNo = _objVw_FAEligibleList.ReferanceNo;
            
                        
            _db.Entry(objFixedAllowanceEligibleList).State = EntityState.Modified;

            if (_db.SaveChanges() > 0)
            {
                TempData["ScfMsg"] = "Data Successfully Saved...!";
                //return RedirectToAction("Create");
                return RedirectToAction("Index", "FixedAllowanceEligibleList");
            }
            else
            {
                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
            }

            return View();
        }


        [HttpGet]
        public ActionResult DeleteEligibleListPerson(int id)
        {
            _Vw_FAEligibleList _objVw_FAEligibleList = new _Vw_FAEligibleList();

            var FAEligibleList = _db.Vw_FAEligibleList.Where(x => x.ELFAID == id).ToList();

            foreach (var item in FAEligibleList)
            {
                _objVw_FAEligibleList.AllowanceName = item.AllowanceName;
                _objVw_FAEligibleList.ServiceNo = item.ServiceNo;
                _objVw_FAEligibleList.Rank = item.Rank;
                _objVw_FAEligibleList.Name = item.Name;
                _objVw_FAEligibleList.EffectiveDate = item.EffectiveDate;
                _objVw_FAEligibleList.ELFAID = id;
            }
            return View(_objVw_FAEligibleList);
        }
        [HttpPost]
        public ActionResult DeleteEligibleListPerson(_Vw_FAEligibleList _objVw_FAEligibleList)
        {
            FixedAllowanceEligibleList objFixedAllowanceEligibleList = _db.FixedAllowanceEligibleLists.Find(_objVw_FAEligibleList.ELFAID);
            objFixedAllowanceEligibleList.CancelReferanceNo = _objVw_FAEligibleList.CancelReferanceNo;


            objFixedAllowanceEligibleList.ModifiedBy = Convert.ToInt32(Session["UID"]);
            objFixedAllowanceEligibleList.ModifiedDate = DateTime.Now;
            string MacAddress = new DALBase().GetMacAddress();
            objFixedAllowanceEligibleList.ModifiedMac = MacAddress;

            objFixedAllowanceEligibleList.Active = 0;
            _db.Entry(objFixedAllowanceEligibleList).State = EntityState.Modified;
            if (_db.SaveChanges() > 0)
            {
                TempData["ScfMsg"] = "Eligibility Successfully Cancel";
                return RedirectToAction("Index", "FixedAllowanceEligibleList");
            }
               else
            {
                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
            }

            return View();
        }
    }
}