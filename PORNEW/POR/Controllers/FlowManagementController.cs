using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class FlowManagementController : Controller
    {
        dbContext _db = new dbContext(); 
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Forward(int? id)
        {
            //Singal Forward        
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            string SubmitStatus_UserRole;
            //int? NextFlowStatusId;

            if (UID != 0)
            {
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.FixedAllowanceDetails.Where(x => x.FADID == id).Select(x => x.EstablishmentId).FirstOrDefault();

                int? SubmitStatus = NextFlowStatusId(id, UserEstablishmentId, RecordEstablishmentId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg
                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                objFixAllowanceDetailsFlowStatu.FADID = id;
                objFixAllowanceDetailsFlowStatu.FMSID = SubmitStatus;
                objFixAllowanceDetailsFlowStatu.CreatedBy = UID;
                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 2000 = Forward//
                objFixAllowanceDetailsFlowStatu.RecordStatusId = 2000;
                objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
                objFixAllowanceDetailsFlowStatu.Active = 1;
                _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);

                if (_db.SaveChanges() > 0)
                {
                    TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                    return RedirectToAction("index");
                }
                else
                {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        [HttpPost]
        public ActionResult Forward(int[] id)
        {
            //Bulk Forwad            
            string SubmitStatus_UserRole = null;
            string UserEstablishmentId = null;
            string RecordEstablishmentId = null;
            int? UserRoleId = 0;
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            foreach (int IDs in id)
            {
                if (UID != 0)
                {
                    UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                    RecordEstablishmentId = _db.FixedAllowanceDetails.Where(x => x.FADID == IDs).Select(x => x.EstablishmentId).FirstOrDefault();

                    int? SubmitStatus = NextFlowStatusId(IDs, UserEstablishmentId, RecordEstablishmentId);
                    //Get Next FlowStatus User Role Name for Add Successfull Msg
                    UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                    SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                    FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                    objFixAllowanceDetailsFlowStatu.FADID = IDs;
                    objFixAllowanceDetailsFlowStatu.FMSID = SubmitStatus;
                    objFixAllowanceDetailsFlowStatu.CreatedBy = UID;
                    //Record Status Releted to RecordStatus Table
                    //Every Record has a Status Ex: Insert/Forward/Delete... 2000 = Forward//
                    objFixAllowanceDetailsFlowStatu.RecordStatusId = 2000;
                    objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
                    objFixAllowanceDetailsFlowStatu.Active = 1;
                    _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            if (_db.SaveChanges() > 0)
            {
                if (UserRoleId >= 5)
                {
                    TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + "-" + UserEstablishmentId;
                }
                else
                {
                    TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + "-" + RecordEstablishmentId;
                }

                return Json("");
            }
            else
            {
                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
            }
            return Json("");
        }

        public int? NextFlowStatusId(int? FADID, string UserEstablishmentId, string RecordEstablishmentId)
        {
            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFADFID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == FADID).Select(x => x.FADFID).Max();
                int? CurrentFMSID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADFID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();

                int? UID = Convert.ToInt32(Session["UID"]);

                //FADID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();

                }
                else if (CurrentUserRole == 4)
                {
                    //UserRoleId = 5 to P&R first User 'ko_pnr'
                    //currentUser-Station/Base = OCPS
                    //RecordUser - Station/Base no P&R there for get UserRole ID from First User Role In P&R
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 5).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole >= 5)
                {
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
                }
                else
                {
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }



        //public ActionResult Delete(int? id)
        //{
        //    //Singal Delete
        //    FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(id);
        //    int? UID = Convert.ToInt32(Session["UID"]);
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    else if (objFixedAllowanceDetail == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    else
        //    {
        //        if (UID != 0)
        //        {
        //            objFixedAllowanceDetail.Active = 0;
        //            objFixedAllowanceDetail.ModifiedBy = Convert.ToInt32(Session["UID"]);
        //            objFixedAllowanceDetail.ModifiedDate = DateTime.Now;
        //            string MacAddress = new DALBase().GetMacAddress();
        //            objFixedAllowanceDetail.ModifiedMac = MacAddress;

        //            _db.Entry(objFixedAllowanceDetail).State = EntityState.Modified;
        //            if (_db.SaveChanges() > 0)
        //            {
        //                TempData["ScfMsg"] = "Data Successfully Deleted";
        //                return RedirectToAction("index");
        //            }
        //            else
        //            {
        //                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
        //            }
        //        }
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(int[] id)
        //{
        //    //Bulk Delete
        //    foreach (int IDs in id)
        //    {
        //        FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(IDs);
        //        int? UID = Convert.ToInt32(Session["UID"]);

        //        if (UID != 0)
        //        {
        //            objFixedAllowanceDetail.Active = 0;
        //            objFixedAllowanceDetail.ModifiedBy = Convert.ToInt32(Session["UID"]);
        //            objFixedAllowanceDetail.ModifiedDate = DateTime.Now;
        //            string MacAddress = new DALBase().GetMacAddress();
        //            objFixedAllowanceDetail.ModifiedMac = MacAddress;
        //            _db.Entry(objFixedAllowanceDetail).State = EntityState.Modified;
        //        }
        //    }
        //    if (_db.SaveChanges() > 0)
        //    {
        //        TempData["ScfMsg"] = "Data Successfully Deleted";
        //        return Json("");
        //    }
        //    else
        //    {
        //        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
        //    }
        //    return Json("");
        //}

        
        //public int? NextFlowStatusId(int? FADID, string UserEstablishmentId, string RecordEstablishmentId)
        //{
        //    int? FMSID = 0;
        //    try
        //    {
        //        //Current Record FMSID
        //        int? MaxFADFID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == FADID).Select(x => x.FADFID).Max();
        //        int? CurrentFMSID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADFID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
        //        int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();

        //        int? UID = Convert.ToInt32(Session["UID"]);

        //        //FADID=Null (actclk create record)
        //        if (CurrentUserRole == null)
        //        {
        //            //Get First FMSID if Current FMSID is null
        //            int? SubmitStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();

        //        }
        //        else if (CurrentUserRole == 4)
        //        {
        //            //UserRoleId = 5 to P&R first User 'ko_pnr'
        //            //currentUser-Station/Base = OCPS
        //            //RecordUser - Station/Base no P&R there for get UserRole ID from First User Role In P&R
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 5).Select(x => x.FMSID).First();
        //        }
        //        else if (CurrentUserRole >= 5)
        //        {
        //            int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
        //        }
        //        else
        //        {
        //            int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return FMSID;
        //}
        //[HttpGet]
        //public ActionResult Reject(int FADID, int FMSID)
        //{
        //    Vw_FixedAllowanceDetail obj_Vw_FixedAllowanceDetail = new Vw_FixedAllowanceDetail();
        //    try
        //    {
        //        obj_Vw_FixedAllowanceDetail.FADID = FADID;
        //        obj_Vw_FixedAllowanceDetail.FMSID = FMSID;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return PartialView("_RejectComment", obj_Vw_FixedAllowanceDetail);
        //}
        //[HttpPost]
        //public ActionResult Reject(Vw_FixedAllowanceDetail objVw_FixedAllowanceDetail)
        //{
        //    //Get data from Model Pop partialView _RejectComment
        //    int? UID = Convert.ToInt32(Session["UID"]);
        //    string PreviousFlowStatus_UserRole;
        //    if (UID != 0)
        //    {
        //        //int? id = objVw_FixedAllowanceDetail.FADID;
        //        string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
        //        string RecordEstablishmentId = _db.FixedAllowanceDetails.Where(x => x.FADID == objVw_FixedAllowanceDetail.FADID).Select(x => x.EstablishmentId).FirstOrDefault();

        //        //Method use for get FMSID
        //        int? PreviousFMSID = PreviousFlowStatusId(objVw_FixedAllowanceDetail.FADID, UserEstablishmentId, RecordEstablishmentId);
        //        //Get Next FlowStatus User Role Name for Add Successfull Msg

        //        int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

        //        PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
        //        FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
        //        objFixAllowanceDetailsFlowStatu.FADID = objVw_FixedAllowanceDetail.FADID;
        //        objFixAllowanceDetailsFlowStatu.FMSID = PreviousFMSID;
        //        objFixAllowanceDetailsFlowStatu.CreatedBy = UID;
        //        //Record Status Releted to RecordStatus Table
        //        //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//
        //        objFixAllowanceDetailsFlowStatu.RecordStatusId = 3000;
        //        objFixAllowanceDetailsFlowStatu.Comment = objVw_FixedAllowanceDetail.Comment;
        //        objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
        //        string MacAddress = new DALBase().GetMacAddress();
        //        objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
        //        objFixAllowanceDetailsFlowStatu.Active = 1;
        //        _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);

        //        if (_db.SaveChanges() > 0)
        //        {
        //            if (UserRoleId >= 5)
        //            {
        //                TempData["ScfMsg"] = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + UserEstablishmentId;
        //            }
        //            else
        //            {
        //                TempData["ScfMsg"] = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + RecordEstablishmentId;
        //            }

        //            return RedirectToAction("index");
        //        }
        //        else
        //        {
        //            TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "User");
        //    }
        //}
        //public int? PreviousFlowStatusId(int? FADID, string UserEstablishmentId, string RecordEstablishmentId)
        //{
        //    int? FMSID = 0;
        //    try
        //    {
        //        //Current Record FMSID
        //        int? MaxFADFID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == FADID).Select(x => x.FADFID).Max();
        //        int? CurrentFMSID = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADFID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
        //        int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();

        //        int? UID = Convert.ToInt32(Session["UID"]);

        //        //FADID=Null (actclk create record)
        //        if (CurrentUserRole == null)
        //        {
        //            //Get First FMSID if Current FMSID is null
        //            FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId).Select(x => x.FMSID).First();
        //        }
        //        else if (CurrentUserRole == 5)
        //        {
        //            //First P&R Flow Status   
        //            //UserRoleId = 4 to Camp Level Last User 'OCPS'
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 4 && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).First();
        //        }
        //        else if (CurrentUserRole >= 5 && CurrentUserRole != 5)
        //        {
        //            int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
        //        }
        //        else if (CurrentUserRole == 1)
        //        {
        //            int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
        //        }
        //        else if (CurrentUserRole < 5 && CurrentUserRole != 1)
        //        {
        //            int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
        //            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return FMSID;
        //}
        //[HttpGet]
        //public ActionResult RejectConfirm(int id)
        //{
        //    FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(id);
        //    objFixedAllowanceDetail.Active = 0;
        //    _db.Entry(objFixedAllowanceDetail).Property(x => x.Active).IsModified = true;
        //    _db.SaveChanges();
        //    TempData["ScfMsg"] = "Successfully Reject Confirmed.";
        //    return RedirectToAction("Index");
        //}
        //public bool CheckDuplicate(string Sno, int? AllowanceId, DateTime? EffectiveDate, DateTime? EndDate)
        //{
        //    bool Status = false;
        //    try
        //    {
        //        int NumRows;

        //        NumRows = _db.FixedAllowanceDetails.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && ((x.EffectiveDate >= EffectiveDate && x.EffectiveDate < EndDate) || (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate))).Count();

        //        // abc = _db.FixedAllowanceDetails.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && ((x.EffectiveDate >= EffectiveDate && x.EffectiveDate < EndDate) || (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate) || (x.EndDate <= EndDate))).Select(x => x.FADID);

        //        if (NumRows >= 1)
        //        {
        //            Status = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Status;

        //}
        //public bool CheckEligibility_FixedAllowance(string Sno, int? AllowanceId, DateTime? EffectiveDate)
        //{
        //    bool EligibilityStatus = false;
        //    try
        //    {
        //        int NumRows;
        //        NumRows = _db.FixedAllowanceEligibleLists.Where(x => x.SNo == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && x.EffectiveDate <= EffectiveDate).Count();

        //        if (NumRows > 0)
        //        {
        //            EligibilityStatus = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return EligibilityStatus;
        //}
        //[HttpGet]
        //public ActionResult UserFlowHistory(int? FADID)
        //{
        //    var UserFlowHistory = new object();
        //    IList<Vw_UserFlowHistory> obj_Vw_UserFlowHistory = new List<Vw_UserFlowHistory>();
        //    try
        //    {
        //        if (FADID != null)
        //        {
        //            obj_Vw_UserFlowHistory = _db.Vw_UserFlowHistory.Where(x => x.FADID == FADID).ToList();

        //        }
        //        else
        //        {
        //            obj_Vw_UserFlowHistory = null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(obj_Vw_UserFlowHistory);

        //}
        //[HttpPost]
        //public ActionResult UserFlowHistory(int FADID)
        //{
        //    var UserFlowHistory = new object();
        //    IList<Vw_UserFlowHistory> obj_Vw_UserFlowHistory = new List<Vw_UserFlowHistory>();
        //    try
        //    {
        //        if (FADID != null)
        //        {
        //            obj_Vw_UserFlowHistory = _db.Vw_UserFlowHistory.Where(x => x.FADID == FADID).ToList();

        //        }
        //        else
        //        {
        //            obj_Vw_UserFlowHistory = null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return PartialView("_UserFlowHistory", obj_Vw_UserFlowHistory);

        //}     
	}
}