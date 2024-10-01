using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ReportData.DAL;

namespace POR.Controllers
{
    public class FixedAllowanceDetailCancelController : Controller
    {
        //
        // GET: /FixedAllowanceDetailCancel/
        dbContext _db = new dbContext();
        int id1 = 0;
        FixedAllowanceDetail objFixedAllowanceDetail = new FixedAllowanceDetail();
         [HttpGet]

        public string Transaction(bool Status, TransactionScope scope)
        {
            string ReturnMessage = null;

            if (Status == true)
            {
                scope.Complete();
                ReturnMessage = "Data Successfully Saved";
            }
            else
            {
                scope.Dispose();
                ReturnMessage = "Process Unsuccessful.Try again...";
            }
            return ReturnMessage;
        }

        public ActionResult Index()
        {
            return View();        
        }       

        [HttpGet]
        public ActionResult Create(int id,_FixedAllowanceCancel obj_FixedAllowanceCancel)
        {
            FixedAllowanceCancel objFixedAllowanceCancel = new FixedAllowanceCancel();

            var model = new _FixedAllowanceCancel();

            id1 = id;
            Session["FADID"] = id;
            try
            {    
                var Sno = _db.FixedAllowanceDetails.Where(x => x.FADID == id).Select(x => x.Sno).FirstOrDefault();
                //ViewData["objDataFixedAllowance"] = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).ToList();
                var PersonList = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).ToList();
                var FixedAllowanceDetails = _db.FixedAllowanceDetails.Where(x => x.FADID == id).ToList();
                foreach (var item in PersonList)
                {
                    model.Rank = item.Rank;
                    model.SurName = item.Name;                    
                }
                foreach (var item2 in FixedAllowanceDetails)
                {
                    string abc =Convert.ToDateTime(item2.EffectiveDate).ToShortDateString();
                    model.EffectiveDate = Convert.ToDateTime(abc);
                    model.EndDate = item2.EndDate;
                    model.CampAuthority = item2.CampAuthority;
                    model.FixedAllowanceDetailId = item2.FADID;
                    model.AllowanceName = _db.Allowances.Where(x => x.ALWID  == item2.AllowanceId).Select(x => x.AllowanceName).FirstOrDefault();
                    model.AllowanceCategory = _db.AllowanceCategories.Where(x => x.ACID == item2.AllowanceCategoryID).Select(x => x.Description).FirstOrDefault();
                    model.CampAuthorityDate = item2.CampAuthorityDate;
                }
            }
            catch 
            {

            }
            return View(model);
        }    
       
        [HttpPost]
        public ActionResult Create(_FixedAllowanceCancel obj_FixedAllowanceCancel,FixedAllowanceDetail objFixedAllowanceDetail)
        {
            try
            {

                FixedAllowanceCancel objFixedAllowanceCancel = new FixedAllowanceCancel();
                int FADID = (int)Session["FADID"];

                objFixedAllowanceCancel.FixedAllowanceDetailId = FADID; 
                objFixedAllowanceCancel.CancelAuthority = obj_FixedAllowanceCancel.CancelAuthority;
                objFixedAllowanceCancel.CancelAuthorityDate = obj_FixedAllowanceCancel.CancelAuthorityDate;
                objFixedAllowanceCancel.Remark = obj_FixedAllowanceCancel.Remark;
                objFixedAllowanceCancel.CancelAuthorityEffectiveDate = obj_FixedAllowanceCancel.CancelAuthorityEffectiveDate;
                if (Session["UID"]!=null)
                {
                    objFixedAllowanceCancel.CreatedBy = Convert.ToInt32(Session["UID"]);    
                }                
                objFixedAllowanceCancel.CreatedDate = DateTime.Now;

                string MacAddress = new DALBase().GetMacAddress();

                objFixedAllowanceCancel.CreatedMac = MacAddress;

                objFixedAllowanceCancel.Active = 1;
                _db.FixedAllowanceCancels.Add(objFixedAllowanceCancel);

                if (_db.SaveChanges() > 0)
                {                    
                    TempData["ScfMsg"] = "Data Successfully Saved";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    return RedirectToAction("Create", new { id = FADID });                    
                }
              
            }
            catch (Exception)
            {
                
                throw;
            }           
        
        }

        public ActionResult ManualCreate(_FixedAllowanceCancelManual obj_FixedAllowanceCancelManual) 
        {
            try
            {
                FixedAllowanceCancelManual objFixedAllowanceCancelManual = new FixedAllowanceCancelManual();
                if (ModelState.IsValid)
                {
                    objFixedAllowanceCancelManual.Description = obj_FixedAllowanceCancelManual.Description;
                    if (Session["UID"] != null)
                    {
                        objFixedAllowanceDetail.CreatedBy = Convert.ToInt32(Session["UID"]);
                    }
                    objFixedAllowanceCancelManual.CreatedDate = DateTime.Now;
                    objFixedAllowanceCancelManual.Active = 1;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixedAllowanceCancelManual.CreatedMac = MacAddress;                    
                    objFixedAllowanceCancelManual.Active = 1;
                    _db.FixedAllowanceCancelManuals.Add(objFixedAllowanceCancelManual);
                    
                    if (_db.SaveChanges() > 0)
                    {
                       
                        TempData["ScfMsg"] = "Data Successfully Saved";                       
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
	}
}