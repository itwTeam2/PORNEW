using POR.Models;
using POR.Models.LeaveModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class PublicHolidayCalenderController : Controller
    {
        //
        // GET: /PublicHolidayCalender/
        dbContext _db = new dbContext();
        public ActionResult Index()
        {
            
            //var PublicHolidayCalender = from s in _db.PublicHolidayCalenders
            //              join c in _db.HolidayTypes on s.HolidayTypeId equals c.HTID
            //              select s;  
            return View(_db.Vw_Holiday.Where(x=>x.Active==1).ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DDL_HolidayType = new SelectList(_db.HolidayTypes, "HTID", "HolidayTypeName");
            //ViewBag.DDL_ServiceType = new SelectList(_dbCommonData.ServiceTypes, "SVCType", "Service_Type");
            return View();
        }

        [HttpPost]
        public ActionResult Create(_PublicHolidayCalender obj_PublicHolidayCalender)
        {
            PublicHolidayCalender objPublicHolidayCalender = new PublicHolidayCalender();

            try
            {
                ViewBag.DDL_HolidayType = new SelectList(_db.HolidayTypes, "HTID", "HolidayTypeName");
                int? UID = 0;
                if (Session["UID"].ToString() != "")
                {
                    UID = Convert.ToInt32(Session["UID"]);
                }

                if (ModelState.IsValid)
                {
                    objPublicHolidayCalender.Reason = obj_PublicHolidayCalender.Reason;
                    objPublicHolidayCalender.HolidayTypeId = obj_PublicHolidayCalender.HolidayTypeId;
                    objPublicHolidayCalender.ApplicableDate = obj_PublicHolidayCalender.ApplicableDate;
                    objPublicHolidayCalender.CreatedBy = UID;
                    objPublicHolidayCalender.CreatedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objPublicHolidayCalender.CreatedMac = MacAddress;
                    objPublicHolidayCalender.Active = 1;

                    _db.PublicHolidayCalenders.Add(objPublicHolidayCalender);

                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Saved";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    }
                }
                else
                {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();

        }
        public ActionResult Delete(int? id)
        {
             PublicHolidayCalender objPublicHolidayCalender = _db.PublicHolidayCalenders.Find(id);
             objPublicHolidayCalender.ModifiedBy = Convert.ToInt32(Session["UID"]);
             objPublicHolidayCalender.ModifiedDate = DateTime.Now;
             string MacAddress = new DALBase().GetMacAddress();
             objPublicHolidayCalender.ModifiedMac = MacAddress;

             objPublicHolidayCalender.Active = 0;
            _db.Entry(objPublicHolidayCalender).State = EntityState.Modified;
            if (_db.SaveChanges()>0)
            {
                TempData["ScfMsg"] = "Data Successfully Delete";
            }            
            return RedirectToAction("Index", "PublicHolidayCalender");
        }
	}
}