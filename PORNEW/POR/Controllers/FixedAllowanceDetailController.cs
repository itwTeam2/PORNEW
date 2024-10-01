using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Transactions;
using System.Data;
using ReportData.DAL;
using System.Net;
using System.Data.Entity;


namespace POR.Controllers
{

    public class FixedAllowanceDetailController : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        int? UID = 0;
        DateTime? FromDate_ = new DateTime();
        DateTime? ToDate_ = new DateTime();
        //
        // GET: /FixedAllowanceDetail/
        //RSID = RecordStatusId ,check Record Status ex:Forward or Reject
        [HttpGet]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "FADID" : "";
                ViewBag.DateSortParm = sortOrder == "ServiceNo" ? "Rank" : "EffectiveDate";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;
                //change ----var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();
                var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();

                List<Vw_FixedAllowanceDetail> objFixedAllowanceDetail = new List<Vw_FixedAllowanceDetail>();

                //foreach (var item in LocationId)
                //{
                //objFixedAllowanceDetail = from s in _db.Vw_FixedAllowanceDetail select s; 
                //objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId).ToList();
                //objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.RecordStatusId != 3000).Take(500).ToList();

                if (UserInfo.RoleId == 1)
                {
                    objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId && x.CreatedBy == UID && ((x.FMSID == null || x.RecordStatusId == 1000))).Take(500).ToList();
                }
                else if (UserInfo.RoleId <= 4 && UserInfo.RoleId != 1)
                {
                    objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId && ((x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 2000 && x.IsProcessed != 1) || (x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId))).ToList();
                }

                else if (UserInfo.RoleId >= 5 && UserInfo.RoleId != 1)
                {

                    objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => (x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 2000 && x.IsProcessed != 1)).Take(500).ToList();
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString)).Take(500).ToList();
                }

                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceType).ToList();
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID).ToList();
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name).ToList();
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName).ToList();
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate).ToList();
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate).ToList();
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        [HttpGet]
        public ActionResult Index_AllRecord(string sortOrder, string currentFilter, string searchString, string LocationId, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);

            // var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).ToList();
            ViewBag.DDL_LocationId = new SelectList(_db.UserPermissions.Where(x => x.UserId == UID), "AccessLocationId", "AccessLocationId");

            if (UID != 0)
            {

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                string location = UserInfo.LocationId;

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
                //38746
                var objVw_FixedAllowance_FLowStatus = _db.Vw_FixedAllowance_FLowStatus.ToList();


                ViewBag.CurrentFilter = searchString;
                //string LocationId_ = "CBO";
                var objFixedAllowanceDetail = from s in _db.Vw_FixedAllowance_FLowStatus
                                              where (s.FADS_CreatedBy == UID)
                                              select s;

                if (UserInfo.RoleId == 1)
                {
                    if (RSID == 3000)
                    {
                        //38746
                        objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.EstablishmentId == location && x.FADS_CreatedBy == UID && x.RecordStatusId == RSID).ToList();
                        // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.Active == 0 && x.CreatedBy == UID && x.RecordStatusId == 1000);      
                    }
                    else
                    {
                        //38746
                        objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.EstablishmentId == location && x.FADS_CreatedBy == UID).ToList();
                        // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID);  
                    }
                }
                else if (UserInfo.RoleId == 6)
                {
                    objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.FADS_CreatedBy == UID).ToList();
                    // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.RecordStatusId == RSID && x.CreatedBy == UID);  
                }
                else if (UserInfo.RoleId == 7)
                {
                    objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.FADS_CreatedBy == UID).ToList();
                    // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.RecordStatusId == RSID && x.CreatedBy == UID);  
                }
                else if (UserInfo.RoleId == 5)
                {
                    //38746
                    objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.FADS_CreatedBy == UID).ToList();
                    //  objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.RecordStatusId == RSID);  
                }
                else if (UserInfo.RoleId == 8)
                {
                    objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.FADS_CreatedBy == UID).ToList();
                    // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.RecordStatusId == RSID && x.CreatedBy == UID);  
                }
                else if (UserInfo.RoleId == 3)
                {
                    if (RSID == 3000)
                    {
                        //38746
                        objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.EstablishmentId == location && x.FADS_CreatedBy == UID).ToList();
                        // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.Active == 0 && x.CreatedBy == UID && x.RecordStatusId == 1000);      
                    }
                    else
                    {
                        //38746
                        objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.EstablishmentId == location && x.FADS_CreatedBy == UID).ToList();
                        // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID);  
                    }

                    //  objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID);  
                }
                else
                {
                    //38746
                    objVw_FixedAllowance_FLowStatus = objVw_FixedAllowance_FLowStatus.Where(x => x.EstablishmentId == location && x.FADS_CreatedBy == UID).ToList();
                    // objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID);  
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID);
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name);
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName);
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate);
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate);
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority);
                        break;
                    case "CampAuthorityDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthorityDate);
                        break;
                    default:
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public ActionResult Details_AllRecord(int id)
        {
            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            int? CurrentStatusUserRole;

            if (Session["UID"].ToString() != "")
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();

                var CurrentStatus_UserRole = (from f in _db.FixAllowanceDetailsFlowStatus
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where (u.EstablishmentId == EstablishmentId || u.EstablishmentId == "P&R") & f.FADID == id
                                              orderby f.FADFID descending
                                              select new
                                              {
                                                  u.RoleName,
                                                  u.RID
                                              }).FirstOrDefault();


                CurrentStatusUserRole = CurrentStatus_UserRole.RID;
                TempData["CurrentStatusUserRole"] = CurrentStatusUserRole;

                var FixallAwanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.FADID == id);

                return View(FixallAwanceDetail.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories, "ServiceCategoryId", "ServiceCategoryName");
            ViewBag.DDL_ServiceType = new SelectList(_dbCommonData.ServiceTypes, "SVCType", "Service_Type");
            ///Description:  Allowance Type Load..
            ViewBag.DDL_AllowanceType = new SelectList(_db.AllowanceTypes.Where(x => x.Status != 0), "ATID", "AllowanceDescreption");

            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

            }

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID_).First();

            var LocationId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).First();
            TempData["EstablishmentId"] = LocationId;

            //if (UserInfo.LocationId == LocationId && UserInfo.DivisionId == "DV065")
            //{
           ViewBag.DDL_Allowance = new SelectList(_db.Allowances.Where(x => x.SECID == 100000), "ALWID", "AllowanceName");
            // }
            //else if (UserInfo.LocationId == LocationId && UserInfo.DivisionId == "DV029")
            //{
            //return RedirectToAction("Index");
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Create(_FixedAllowanceDetail obj_FixedAllowanceDetail)
        {
            FixedAllowanceDetail objFixedAllowanceDetail = new FixedAllowanceDetail();

            ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories, "ServiceCategoryId", "ServiceCategoryName");
            ViewBag.DDL_ServiceType = new SelectList(_dbCommonData.ServiceTypes, "SVCType", "Service_Type");
            ///Description:  Allowance Type Load..
            ViewBag.DDL_AllowanceType = new SelectList(_db.AllowanceTypes.Where(x => x.Status != 0), "ATID", "AllowanceDescreption");

            int Id = 0;
            Id = new DALCommanQuery().NextId();

            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }

            string EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            var RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).First();
            //-------------------------------------------------------------------------------------------------//
            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).First();
            //var LocationType = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationType).First();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).First();
            ViewBag.DDL_Allowance = new SelectList(_db.Allowances.Where(x => x.SECID == 100000), "ALWID", "AllowanceName");
            //________________________________________________________________________________________________________//

            if (obj_FixedAllowanceDetail.AllowanceId != 105 && obj_FixedAllowanceDetail.AllowanceId == 109)
            {
                obj_FixedAllowanceDetail.FullHalfPay = 2;
                ModelState.Remove("FullHalfPay");
            }
            else if (obj_FixedAllowanceDetail.AllowanceId != 105 && obj_FixedAllowanceDetail.AllowanceId != 109)
            {
                obj_FixedAllowanceDetail.AllowanceCategoryID = 0;
                obj_FixedAllowanceDetail.FullHalfPay = 2;
                ModelState.Remove("AllowanceCategoryID");
                ModelState.Remove("FullHalfPay");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    //ServiceNo Property Manually Added to obj_FixedAllowanceDetail Model
                    //Get Sno using ServiceNo
                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_FixedAllowanceDetail.ServiceNo_).Select(x => x.SNo).FirstOrDefault();

                    //Get Service Type using ServiceNo
                    var ServiceType = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_FixedAllowanceDetail.ServiceNo_).Select(x => x.service_type).FirstOrDefault();
                    objFixedAllowanceDetail.ServiceType = ServiceType;

                    //Get service Category using Service Type
                    // var category = _dbCommonData.ServiceTypes.Where(x => x.SVCType == ServiceType).Select(x => x.IsOfficer).FirstOrDefault();
                    //Get RankId from HRMS DB
                    var RankId = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_FixedAllowanceDetail.ServiceNo_).Select(x => x.RankID).FirstOrDefault();

                    objFixedAllowanceDetail.ServiceCategoryId = obj_FixedAllowanceDetail.ServiceCategoryId;
                    objFixedAllowanceDetail.Sno = Sno;
                    objFixedAllowanceDetail.RankId = RankId;
                    objFixedAllowanceDetail.AllowanceCategoryID = obj_FixedAllowanceDetail.AllowanceCategoryID;
                    objFixedAllowanceDetail.FullHalfPay = obj_FixedAllowanceDetail.FullHalfPay;
                    objFixedAllowanceDetail.AllowanceId = obj_FixedAllowanceDetail.AllowanceId;
                    objFixedAllowanceDetail.EffectiveDate = obj_FixedAllowanceDetail.EffectiveDate;
                    objFixedAllowanceDetail.AllowanceTypeId = obj_FixedAllowanceDetail.ATID;

                    if (obj_FixedAllowanceDetail.EffectiveDate == null)
                    {
                        var FixDate = "01/01/1800";
                        objFixedAllowanceDetail.EffectiveDate = Convert.ToDateTime(FixDate);
                    }
                    else
                    {
                        objFixedAllowanceDetail.EffectiveDate = obj_FixedAllowanceDetail.EffectiveDate;
                    }

                    objFixedAllowanceDetail.EndDate = obj_FixedAllowanceDetail.EndDate;
                    //Bind Next Auto incriment Id and Genarated values in Json
                    objFixedAllowanceDetail.CampAuthority = obj_FixedAllowanceDetail.CampAuthority + "/" + Id.ToString();

                    objFixedAllowanceDetail.CampAuthorityDate = obj_FixedAllowanceDetail.CampAuthorityDate;
                    objFixedAllowanceDetail.Remark = obj_FixedAllowanceDetail.Remark;
                    objFixedAllowanceDetail.EstablishmentId = EstablishmentId;
                    objFixedAllowanceDetail.CreatedBy = UID;
                    objFixedAllowanceDetail.CreatedDate = DateTime.Now;
                    objFixedAllowanceDetail.Active = 1;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixedAllowanceDetail.CreatedMac = MacAddress;
                    FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                    //Check Duplicate Entry //Check Database 

                    //new ceased code was updated 
                    bool Status = false;
                    Status = CheckDuplicate(Sno, obj_FixedAllowanceDetail.AllowanceId, obj_FixedAllowanceDetail.EffectiveDate, obj_FixedAllowanceDetail.EndDate );

                    bool EligibilityStatus = false;
                    EligibilityStatus = CheckEligibility_FixedAllowance(Sno, obj_FixedAllowanceDetail.AllowanceId, obj_FixedAllowanceDetail.EffectiveDate);

                    if (Status == true)
                    {
                        TempData["ErrMsg"] = "Duplicate Entries Not Allow. Date Period Overlap";
                    }
                    else if (EligibilityStatus == false)
                    {
                        TempData["ErrMsg"] = "Entered Allowance Not Eligible for Perticular Service Person";
                    }
                    else
                    {
                        _db.FixedAllowanceDetails.Add(objFixedAllowanceDetail);

                        #region MyRegion

                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {

                            if (_db.SaveChanges() > 0)
                            {                               

                                //int? CurrentFlowStatus = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == Id & x.Active == 1).Select(x => x.FMSID).Max();

                                //int? CurrentFlowStatus = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == Id & x.Active == 1).Select(x => x.FADID).Max();

                                //if (CurrentFlowStatus == null)
                                //{
                                int? FirstFMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == EstablishmentId).Select(x => x.FMSID).First();

                                objFixAllowanceDetailsFlowStatu.FADID = Id;
                                objFixAllowanceDetailsFlowStatu.FMSID = null;
                                objFixAllowanceDetailsFlowStatu.CreatedBy = UID;

                                //Record Status Releted to RecordStatus Table
                                //Every Record has a Status Ex: Insert/Forward/Delete... 1000 = Clk Data Entry//
                                objFixAllowanceDetailsFlowStatu.RecordStatusId = 1000;
                                objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
                                objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
                                objFixAllowanceDetailsFlowStatu.Active = 1;
                                _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);
                                //}

                                if (_db.SaveChanges() > 0)
                                {
                                    scope.Complete();
                                    TempData["ScfMsg"] = "Data Successfully Saved," + " Reference No : " + objFixedAllowanceDetail.CampAuthority.ToString();
                                    //return RedirectToAction("Create");
                                    return View();
                                }
                                else
                                {
                                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                    scope.Dispose();
                                }
                            }
                            else
                            {
                                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                scope.Dispose();
                            }

                        }
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }
        [HttpGet]
        public ActionResult Details(int id)
        {

            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            int? CurrentStatusUserRole;

            //For popup box
            TempData["FADID"] = id;

            if (Session["UID"].ToString() != "")
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();



                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                var CurrentStatus_UserRole = (from f in _db.FixAllowanceDetailsFlowStatus
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.FADID == id
                                              orderby f.FADFID descending
                                              select new
                                              {
                                                  u.RoleName,
                                                  u.RID
                                              }).FirstOrDefault();

                if (CurrentStatus_UserRole != null)
                {
                    CurrentStatusUserRole = CurrentStatus_UserRole.RID;

                    TempData["CurrentStatusUserRole"] = CurrentStatusUserRole;
                }
                else
                {
                    TempData["CurrentStatusUserRole"] = UserRoleId;
                }

                var FixallAwanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.FADID == id);
                int? CurrentStatus = FixallAwanceDetail.Select(x => x.CurrentStatus).First();
                TempData["CurrentStatus"] = CurrentStatus;
                int? SubmitStatus = FixallAwanceDetail.Select(x => x.SubmitStatus).First();
                TempData["SubmitStatus"] = SubmitStatus;

                // Find Reject User, User Role Name           

                var FixAllowanceDetailsFlowStatus = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == id & x.RecordStatusId == 3000).FirstOrDefault();

                if (FixAllowanceDetailsFlowStatus != null)
                {
                    var FlowStatus = _db.UserInfoes.Where(x => x.UID == FixAllowanceDetailsFlowStatus.CreatedBy).First();
                    var UserRole = _db.UserRoles.Where(x => x.RID == FlowStatus.RoleId).First();
                    TempData["RejectUserRole"] = UserRole.RoleName;
                    TempData["RejectDate"] = FixAllowanceDetailsFlowStatus.CreatedDate;
                }
                else
                {
                    return View(FixallAwanceDetail.FirstOrDefault());
                }
                return View(FixallAwanceDetail.FirstOrDefault());

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public JsonResult GetServicePerson(string id, int ServiceCategoryId)
        {
            Vw_PersonalDetail objVw_PersonalDetail = new Vw_PersonalDetail();

            if (ServiceCategoryId == 1)
            {
                objVw_PersonalDetail = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id & x.RankID > 13).FirstOrDefault();

            }
            else
            {
                objVw_PersonalDetail = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id & x.RankID <= 13).FirstOrDefault();

            }

            return Json(objVw_PersonalDetail, JsonRequestBehavior.AllowGet);
        }
        //Auto Genarate Reference No
        public JsonResult ReferenceNo(int AllowanceId)
        {
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            FixedAllowanceDetail objFixedAllowanceDetail = new FixedAllowanceDetail();
            string AllowanceShortName = _db.Allowances.Where(x => x.ALWID == AllowanceId).Select(x => x.AllowanceShortName).FirstOrDefault();
            string EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            string RefNo = EstablishmentId.ToString() + "/" + AllowanceShortName.ToString() + "/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString();
            objFixedAllowanceDetail.CampAuthority = RefNo.ToString();
            return Json(objFixedAllowanceDetail, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            //Singal Delete
            FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(id);
            int? UID = Convert.ToInt32(Session["UID"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else if (objFixedAllowanceDetail == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (UID != 0)
                {
                    objFixedAllowanceDetail.Active = 0;
                    objFixedAllowanceDetail.ModifiedBy = Convert.ToInt32(Session["UID"]);
                    objFixedAllowanceDetail.ModifiedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixedAllowanceDetail.ModifiedMac = MacAddress;

                    _db.Entry(objFixedAllowanceDetail).State = EntityState.Modified;
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Deleted";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Delete(int[] id)
        {
            //Bulk Delete
            foreach (int IDs in id)
            {
                FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(IDs);
                int? UID = Convert.ToInt32(Session["UID"]);

                if (UID != 0)
                {
                    objFixedAllowanceDetail.Active = 0;
                    objFixedAllowanceDetail.ModifiedBy = Convert.ToInt32(Session["UID"]);
                    objFixedAllowanceDetail.ModifiedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFixedAllowanceDetail.ModifiedMac = MacAddress;
                    _db.Entry(objFixedAllowanceDetail).State = EntityState.Modified;
                }
            }
            if (_db.SaveChanges() > 0)
            {
                TempData["ScfMsg"] = "Data Successfully Deleted";
                return Json("");
            }
            else
            {
                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
            }
            return Json("");
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
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).OrderBy(x=>x.UserRoleID).Select(x => x.SubmitStatus).FirstOrDefault();
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
        [HttpGet]
        public ActionResult Reject(int FADID, int FMSID)
        {
            Vw_FixedAllowanceDetail obj_Vw_FixedAllowanceDetail = new Vw_FixedAllowanceDetail();
            try
            {
                obj_Vw_FixedAllowanceDetail.FADID = FADID;
                obj_Vw_FixedAllowanceDetail.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_RejectComment", obj_Vw_FixedAllowanceDetail);
        }      
        [HttpPost]
        public ActionResult Index_Reject(Vw_FixedAllowanceDetail objVw_FixedAllowanceDetail)
        {
            //Get data from Model Pop partialView _RejectComment
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.FixedAllowanceDetails.Where(x => x.FADID == objVw_FixedAllowanceDetail.FADID).Select(x => x.EstablishmentId).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(objVw_FixedAllowanceDetail.FADID, UserEstablishmentId, RecordEstablishmentId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
                FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                objFixAllowanceDetailsFlowStatu.FADID = objVw_FixedAllowanceDetail.FADID;
                objFixAllowanceDetailsFlowStatu.FMSID = PreviousFMSID;
                objFixAllowanceDetailsFlowStatu.CreatedBy = UID;
                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//
                objFixAllowanceDetailsFlowStatu.RecordStatusId = 3000;
                objFixAllowanceDetailsFlowStatu.Comment = objVw_FixedAllowanceDetail.Comment;
                objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
                objFixAllowanceDetailsFlowStatu.Active = 1;
                _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);

                if (_db.SaveChanges() > 0)
                {
                    if (UserRoleId >= 5)
                    {
                        TempData["ScfMsg"] = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + UserEstablishmentId;
                    }
                    else
                    {
                        TempData["ScfMsg"] = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + RecordEstablishmentId;
                    }

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
        [HttpGet]
        public ActionResult Ceased(int FADID, int FMSID,int FID)
        {
            Vw_FixedAllowanceDetail obj_Vw_FixedAllowanceDetail = new Vw_FixedAllowanceDetail();
            try
            {
                obj_Vw_FixedAllowanceDetail.FADID = FADID;
                obj_Vw_FixedAllowanceDetail.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_Ceased", obj_Vw_FixedAllowanceDetail);
        }
        //Select All Record for Perticiular Service Person Index 
        //Ceased Allowances 
        [HttpGet]
        public ActionResult IndexCeased(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "FADID" : "";
                ViewBag.DateSortParm = sortOrder == "ServiceNo" ? "Rank" : "EffectiveDate";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;
                //change ----var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();
                var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();

                List<Vw_FixedAllowanceDetail> objFixedAllowanceDetail = new List<Vw_FixedAllowanceDetail>();

                objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.Active == 1 && x.FMSID==135).ToList();

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString)).Take(500).ToList();
                }

                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceType).ToList();
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID).ToList();
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name).ToList();
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName).ToList();
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate).ToList();
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate).ToList();
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        //Insert Record for FixedallowanceFlowastats Table with RecordStatus 6000
        //Ceased data must include and Posting Authority
        [HttpPost]
        public ActionResult IndexCeased(Vw_FixedAllowanceDetail objVw_FixedAllowanceDetail)
        {
            //Get data from Model Pop partialView _Ceased
            int? UID = Convert.ToInt32(Session["UID"]);
            string Ceased_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.FixedAllowanceDetails.Where(x => x.FADID == objVw_FixedAllowanceDetail.FADID).Select(x => x.EstablishmentId).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(objVw_FixedAllowanceDetail.FADID, UserEstablishmentId, RecordEstablishmentId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

                Ceased_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
                FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                objFixAllowanceDetailsFlowStatu.FADID = objVw_FixedAllowanceDetail.FADID;
                objFixAllowanceDetailsFlowStatu.FMSID = 145;
                objFixAllowanceDetailsFlowStatu.CreatedBy = UID;
                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 6000 = Ceased//
                objFixAllowanceDetailsFlowStatu.RecordStatusId = 6000;
                objFixAllowanceDetailsFlowStatu.Comment = objVw_FixedAllowanceDetail.Comment;
                objFixAllowanceDetailsFlowStatu.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objFixAllowanceDetailsFlowStatu.CreatedMac = MacAddress;
                objFixAllowanceDetailsFlowStatu.Active = 1;
                _db.FixAllowanceDetailsFlowStatus.Add(objFixAllowanceDetailsFlowStatu);

                if (_db.SaveChanges() > 0)
                {

                    TempData["ScfMsg"] = "Ceased Allowacne Successfully";
                }
                 else
                    {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                }

                    return RedirectToAction("IndexCeased");
                }
             
                return View();
            }
            
        //Details Page for Ceased All Records 
        [HttpGet]   
        public ActionResult DetailCeased(int id)
        {

            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            int? CurrentStatusUserRole;

            //For popup box
            TempData["FADID"] = id;

            if (Session["UID"].ToString() != "")
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                var CurrentStatus_UserRole = (from f in _db.FixAllowanceDetailsFlowStatus
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.FADID == id
                                              orderby f.FADFID descending
                                              select new
                                              {
                                                  u.RoleName,
                                                  u.RID
                                              }).FirstOrDefault();

                if (CurrentStatus_UserRole != null)
                {
                    CurrentStatusUserRole = CurrentStatus_UserRole.RID;

                    TempData["CurrentStatusUserRole"] = CurrentStatusUserRole;
                }
                else
                {
                    TempData["CurrentStatusUserRole"] = UserRoleId;
                }

                var FixallAwanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.FADID == id);
                int? CurrentStatus = FixallAwanceDetail.Select(x => x.CurrentStatus).First();
                TempData["CurrentStatus"] = CurrentStatus;
                int? SubmitStatus = FixallAwanceDetail.Select(x => x.SubmitStatus).First();
                TempData["SubmitStatus"] = SubmitStatus;

                // Find Reject User, User Role Name           

                var FixAllowanceDetailsFlowStatus = _db.FixAllowanceDetailsFlowStatus.Where(x => x.FADID == id & x.RecordStatusId == 3000).FirstOrDefault();

                if (FixAllowanceDetailsFlowStatus != null)
                {
                    var FlowStatus = _db.UserInfoes.Where(x => x.UID == FixAllowanceDetailsFlowStatus.CreatedBy).First();
                    var UserRole = _db.UserRoles.Where(x => x.RID == FlowStatus.RoleId).First();
                    TempData["RejectUserRole"] = UserRole.RoleName;
                    TempData["RejectDate"] = FixAllowanceDetailsFlowStatus.CreatedDate;
                }
                else
                {
                    return View(FixallAwanceDetail.FirstOrDefault());
                }
                return View(FixallAwanceDetail.FirstOrDefault());

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }     

        public int? PreviousFlowStatusId(int? FADID, string UserEstablishmentId, string RecordEstablishmentId)
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
                    FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole == 5)
                {
                    //First P&R Flow Status   
                    //UserRoleId = 4 to Camp Level Last User 'OCPS'
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 4 && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole >= 5 && CurrentUserRole != 5)
                {
                    int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole == 1)
                {
                    int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole < 5 && CurrentUserRole != 1)
                {
                    int? RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.RejectStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        [HttpGet]
        public ActionResult RejectConfirm(int id)
        {
            FixedAllowanceDetail objFixedAllowanceDetail = _db.FixedAllowanceDetails.Find(id);
            objFixedAllowanceDetail.Active = 0;
            _db.Entry(objFixedAllowanceDetail).Property(x => x.Active).IsModified = true;
            _db.SaveChanges();
            TempData["ScfMsg"] = "Successfully Reject Confirmed.";
            return RedirectToAction("RejectList");
        }
        public bool CheckDuplicate(string Sno, int? AllowanceId, DateTime? EffectiveDate, DateTime? EndDate)
        {
            bool Status = false;
            try
            {
                int NumRows;               

                NumRows = _db.FixedAllowanceDetails.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && ((x.EffectiveDate >= EffectiveDate && x.EffectiveDate < EndDate) || (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate))).Count();
                             

                // abc = _db.FixedAllowanceDetails.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && ((x.EffectiveDate >= EffectiveDate && x.EffectiveDate < EndDate) || (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate) || (x.EndDate <= EndDate))).Select(x => x.FADID);

                if (NumRows >= 1)
                {
                    NumRows = _db.Vw_FixedAllowance_FLowStatus.Where(x => x.Sno == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && (x.EffectiveDate <= EffectiveDate && x.EndDate >= EndDate) && x.RecordStatusId == 6000).Count();
                    if (NumRows >= 1)
                    {
                        Status = false;
                    }
                    else
                    {
                        Status = true;
                    }
                }
                else
                {
                    Status = false;
                }
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Status;

        }
        public bool CheckEligibility_FixedAllowance(string Sno, int? AllowanceId, DateTime? EffectiveDate)
        {
            bool EligibilityStatus = false;
            try
            {
                int NumRows;
                NumRows = _db.FixedAllowanceEligibleLists.Where(x => x.SNo == Sno && x.AllowanceId == AllowanceId && x.Active == 1 && x.EffectiveDate <= EffectiveDate).Count();

                if (NumRows > 0)
                {
                    EligibilityStatus = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return EligibilityStatus;
        }
        [HttpGet]
        public ActionResult UserFlowHistory(int? FADID)
        {
            var UserFlowHistory = new object();
            IList<Vw_UserFlowHistory> obj_Vw_UserFlowHistory = new List<Vw_UserFlowHistory>();
            try
            {
                if (FADID != null)
                {
                    obj_Vw_UserFlowHistory = _db.Vw_UserFlowHistory.Where(x => x.FADID == FADID).ToList();
                }
                else
                {
                    obj_Vw_UserFlowHistory = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(obj_Vw_UserFlowHistory);

        }
        [HttpPost]
        public ActionResult UserFlowHistory(int FADID)
          {
            var UserFlowHistory = new object();
            IList<Vw_UserFlowHistory> obj_Vw_UserFlowHistory = new List<Vw_UserFlowHistory>();
            try
            {
                if (FADID != 0)
                {
                    obj_Vw_UserFlowHistory = _db.Vw_UserFlowHistory.Where(x => x.FADID == FADID).ToList();
                }
                else
                {
                    obj_Vw_UserFlowHistory = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_UserFlowHistory", obj_Vw_UserFlowHistory);

        }
        [HttpGet]
        public ActionResult Report_FlowCompleted(string sortOrder, string currentFilter, string searchString, int? page, string EffectiveDateFrom, string EffectiveDateTo, int? CommandType)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

                if (CommandType != null && CommandType != 1)
                {
                    return Redirect("~/Report/FixedAllowanceReport.aspx?EstablishmentId=" + UserInfo.LocationId + "&EffectiveDateFrom=" + EffectiveDateFrom + "&EffectiveDateTo=" + EffectiveDateTo + "");
                }
                else
                {

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

                    var FixedAllowanceDetails = from s in _db.Vw_FixedAllowanceDetail
                                                select s;

                    if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                    {
                        DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                        DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4 && (x.EffectiveDate >= FromDate_ && x.EffectiveDate < ToDate_));

                    }
                    else
                    {
                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == UserInfo.LocationId & x.CurrentStatus == 4);

                    }

                    switch (sortOrder)
                    {
                        case "ServiceNo":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceNo);
                            break;
                        case "ServiceType":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceType);
                            break;
                        case "Rank":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.RankID);
                            break;
                        case "Name":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Name);
                            break;
                        case "AllowanceName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceName);
                            break;
                        case "EffectiveDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.EffectiveDate);
                            break;
                        case "EndDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.EndDate);
                            break;
                        case "CampAuthority":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthority);
                            break;
                        case "CampAuthorityDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthorityDate);
                            break;
                        default:
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceType);
                            break;
                    }

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(FixedAllowanceDetails.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        //View All Forwerded list List
        [HttpGet]
        public ActionResult Index_Forwerded_list(string sortOrder, string currentFilter, string searchString, string LocationId, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);

            // var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).ToList();
            ViewBag.DDL_LocationId = new SelectList(_db.UserPermissions.Where(x => x.UserId == UID), "AccessLocationId", "AccessLocationId");
            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

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
                //string LocationId_ = "CBO";
                var objFixedAllowanceDetail = from s in _db.Vw_FixedAllowance_FLowStatus
                                              where (s.CreatedBy == UID)
                                              select s;


                objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID);

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Service_Type);
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID);
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name);
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName);
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate);
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate);
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority);
                        break;
                    case "CampAuthorityDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthorityDate);
                        break;
                    default:
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Service_Type);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        //View All Rejected List
        [HttpGet]
        public ActionResult Index_Rejected_List(string sortOrder, string currentFilter, string searchString, string LocationId, int? page, int? RSID)
        {

            int? UID = Convert.ToInt32(Session["UID"]);

            // var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).ToList();
            ViewBag.DDL_LocationId = new SelectList(_db.UserPermissions.Where(x => x.UserId == UID), "AccessLocationId", "AccessLocationId");
            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

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
                //string LocationId_ = "CBO";
                var objFixedAllowanceDetail = from s in _db.Vw_FixedAllowance_FLowStatus
                                              where (s.CreatedBy == UID)
                                              select s;


                objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.CreatedBy == UID && x.RecordStatusId == RSID); 

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Service_Type);
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID);
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name);
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName);
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate);
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate);
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority);
                        break;
                    case "CampAuthorityDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthorityDate);
                        break;
                    default:
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Service_Type);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpGet]
        public ActionResult ForwerdList(string sortOrder, string currentFilter, string searchString, int? page, string EffectiveDateFrom, string EffectiveDateTo, int? CommandType)
        {
            Vw_FixedAllowance_FLowStatus objVw_FixedAllowance_FLowStatus = new Vw_FixedAllowance_FLowStatus();

            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

                string Location = UserInfo.LocationId.ToString();

                if (CommandType != null && CommandType != 1)
                {
                    try
                    {
                        DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                        DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                        return Redirect("~/Report/Forwerd.aspx?FromDate=" + EffectiveDateFrom + "&Todate=" + EffectiveDateTo + " &UID=" + UID + " &Location=" + Location + " ");
                    }
                    catch (Exception)
                    {
                       

                    }

                }
                else
                {

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

                    var FixedAllowanceDetails = from s in _db.Vw_FixedAllowanceDetail
                                                select s;

                    if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                    {
                        try
                        {
                            DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                            DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);
                            // string Location_Id = objVw_FixedAllowance_FLowStatus.LocationId;

                            FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.FADFS_CreatedBy == UID && (x.CreatedDate >= FromDate_ && x.CreatedDate <= ToDate_) && x.RecordStatusId != 3000);
                        }
                        catch (Exception ex)
                        {

                            TempData["ErrMsg"] = "please Enter Date Priod... " + ex +"";
                        }

                    }
                    else
                    {
                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.FADFS_CreatedBy == UID);
                    }

                    switch (sortOrder)
                    {
                        case "ServiceNo":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceNo);
                            break;
                        case "ServiceType":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Service_Type);
                            break;
                        case "Rank":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.RankID);
                            break;
                        case "Name":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Name);
                            break;
                        case "AllowanceName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceName);
                            break;
                        case "EffectiveDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.EffectiveDate);
                            break;
                        case "EndDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.EndDate);
                            break;
                        case "CampAuthority":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthority);
                            break;
                        case "CampAuthorityDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthorityDate);
                            break;
                        case "RoleName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.RoleName);
                            break;
                        default:
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Service_Type);
                            break;
                    }

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(FixedAllowanceDetails.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        [HttpGet]
        public ActionResult ForwerdList_StationVice(string sortOrder, string currentFilter, string searchString, int? page, string EffectiveDateFrom, string EffectiveDateTo, int? CommandType, string LocationId)
        {

            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

                string Location = UserInfo.LocationId.ToString();

                if (CommandType != null && CommandType != 1)
                {
                    try
                    {
                        FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                        ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                        return Redirect("~/Report/ForwerdListCampVice.aspx?FromDate=" + EffectiveDateFrom + "&Todate=" + EffectiveDateTo + " &UID=" + UID + " &Location=" + LocationId + "");
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
                else
                {

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

                    var FixedAllowanceDetails = from s in _db.Vw_FixedAllowanceDetail
                                                select s;



                    if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                    {
                        try
                        {
                            DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                            DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                            FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == LocationId && x.RecordStatusId != 3000 && x.FMSID >= 131 && x.Active != 0 && (x.CreatedDate >= FromDate_ && x.CreatedDate <= ToDate_));
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }


                    }
                    else
                    {

                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.RecordStatusId != 3000 && x.Active != 0);

                    }

                    switch (sortOrder)
                    {
                        case "ServiceNo":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceNo);
                            break;
                        case "ServiceType":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Service_Type);
                            break;
                        case "Rank":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.RankID);
                            break;
                        case "Name":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Name);
                            break;
                        case "AllowanceName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceName);
                            break;
                        case "EffectiveDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.EffectiveDate);
                            break;
                        case "EndDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.EndDate);
                            break;
                        case "CampAuthority":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthority);
                            break;
                        case "CampAuthorityDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthorityDate);
                            break;
                        default:
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Service_Type);
                            break;
                    }

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(FixedAllowanceDetails.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult RejectList_StationVice(string sortOrder, string currentFilter, string searchString, int? page, string EffectiveDateFrom, string EffectiveDateTo, int? CommandType, string LocationId)
        {
            DALCommanQuery objDALCommanQuery = new DALCommanQuery();
            DataSet ds = new DataSet();


            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                string Location = UserInfo.LocationId.ToString();

                if (CommandType != null && CommandType != 1)
                {
                    try
                    {
                        DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                        DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                        return Redirect("~/Report/RejectList.aspx?FromDate=" + EffectiveDateFrom + "&Todate=" + EffectiveDateTo + " &UID=" + UID + " &Location=" + LocationId + " ");
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
                else
                {
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

                    var FixedAllowanceDetails = from s in _db.Vw_FixedAllowanceDetail
                                                select s;

                    if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                    {
                        try
                        {
                            DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                            DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                            objDALCommanQuery.RejectList(EffectiveDateFrom, EffectiveDateTo, LocationId);

                            FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == LocationId && x.RecordStatusId == 3000 && (x.CreatedDate >= FromDate_ && x.CreatedDate <= ToDate_) && x.Active != 0);
                        }
                        catch 
                        {

                           
                        }

                    }
                    else
                    {
                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.RecordStatusId == 3000 && x.Active != 0);
                    }

                    switch (sortOrder)
                    {
                        case "ServiceNo":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceNo);
                            break;
                        case "ServiceType":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Rank);
                            break;
                        case "Rank":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Name);
                            break;
                        case "Name":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceId);
                            break;
                        case "AllowanceName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceName);
                            break;
                        case "EffectiveDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.EffectiveDate);
                            break;
                        case "EndDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.EndDate);
                            break;
                        case "CampAuthority":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthority);
                            break;
                        case "CampAuthorityDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthorityDate);
                            break;
                        case "Comment":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.Comment);
                            break;
                        default:
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Comment);
                            break;
                    }

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(FixedAllowanceDetails.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult RejectList_CampList(string sortOrder, string currentFilter, string searchString, int? page, string EffectiveDateFrom, string EffectiveDateTo, int? CommandType, string LocationId)
        {
            DALCommanQuery objDALCommanQuery = new DALCommanQuery();
            DataSet ds = new DataSet();


            int? UID = Convert.ToInt32(Session["UID"]);
            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                string Location = UserInfo.LocationId.ToString();

                if (CommandType != null && CommandType != 1)
                {
                    try
                    {
                        DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                        DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                        return Redirect("~/Report/RejectList.aspx?FromDate=" + EffectiveDateFrom + "&Todate=" + EffectiveDateTo + " &UID=" + UID + " &Location=" + Location + " ");
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }

                }
                else
                {
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

                    var FixedAllowanceDetails = from s in _db.Vw_FixedAllowanceDetail
                                                select s;

                    if ((EffectiveDateFrom != "" && EffectiveDateFrom != null) || (EffectiveDateTo != "" && EffectiveDateTo != null))
                    {
                        try
                        {
                            DateTime? FromDate_ = Convert.ToDateTime(EffectiveDateFrom);
                            DateTime? ToDate_ = Convert.ToDateTime(EffectiveDateTo);

                            // objDALCommanQuery.RejectList(EffectiveDateFrom, EffectiveDateTo, LocationId);

                            FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.EstablishmentId == Location && x.RecordStatusId == 3000 && (x.CreatedDate >= FromDate_ && x.CreatedDate <= ToDate_) && x.Active != 0);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    else
                    {
                        FixedAllowanceDetails = FixedAllowanceDetails.Where(x => x.RecordStatusId == 3000 && x.Active != 0);
                    }

                    switch (sortOrder)
                    {
                        case "ServiceNo":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.ServiceNo);
                            break;
                        case "ServiceType":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Rank);
                            break;
                        case "Rank":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Name);
                            break;
                        case "Name":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceId);
                            break;
                        case "AllowanceName":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.AllowanceName);
                            break;
                        case "EffectiveDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.EffectiveDate);
                            break;
                        case "EndDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.EndDate);
                            break;
                        case "CampAuthority":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthority);
                            break;
                        case "CampAuthorityDate":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.CampAuthorityDate);
                            break;
                        case "Comment":
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderByDescending(s => s.Comment);
                            break;
                        default:
                            FixedAllowanceDetails = FixedAllowanceDetails.OrderBy(s => s.Comment);
                            break;
                    }

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);
                    return View(FixedAllowanceDetails.ToPagedList(pageNumber, pageSize));
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        [HttpGet]

        public ActionResult RejectList(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

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
                //change ----var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();
                var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();

                List<Vw_FixedAllowanceDetail> objFixedAllowanceDetail = new List<Vw_FixedAllowanceDetail>();

                //foreach (var item in LocationId)
                //{
                //objFixedAllowanceDetail = from s in _db.Vw_FixedAllowanceDetail select s; 
                //objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId).ToList();

                if (UserInfo.RoleId == (int)POR.Enum.UserRole.AccountsClerk || UserInfo.RoleId == (int)POR.Enum.UserRole.SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.ACCOUNTSOFFICER || UserInfo.RoleId == (int)POR.Enum.UserRole.OCPSOCA)
                {
                    objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId && ((x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId))).Take(500).OrderByDescending(x => x.CreatedDate).ToList();
                }
                else
                {
                    objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Take(500).OrderByDescending(x=>x.CreatedDate).ToList();
                }
                



                if (UserInfo.RoleId == 1)
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId && x.CreatedBy == UID && ((x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId))).ToList();
                }
                else if (UserInfo.RoleId <= 4 && UserInfo.RoleId != 1)
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId && ((x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId))).ToList();
                }
                else if (UserInfo.RoleId >= 5 && UserInfo.RoleId != 1)
                {
                    //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
                    ////////  old - objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => (x.RecordStatusId == 3000)).ToList();
                    //change done by FL OKCI Perera , @CBO 25/2020
                   objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.Active !=0 && (x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId)).ToList();
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString)).Take(500).ToList();
                }

                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceType).ToList();
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID).ToList();
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name).ToList();
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName).ToList();
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate).ToList();
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate).ToList();
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult SearchList(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();

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
                //change ----var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();
                var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).FirstOrDefault();

                List<Vw_FixedAllowanceDetail> objFixedAllowanceDetail = new List<Vw_FixedAllowanceDetail>();

                //foreach (var item in LocationId)
                //{
                //objFixedAllowanceDetail = from s in _db.Vw_FixedAllowanceDetail select s; 
                //objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.Where(x => x.EstablishmentId == LocationId).ToList();
                objFixedAllowanceDetail = _db.Vw_FixedAllowanceDetail.ToList();




                //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
                objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => (x.RecordStatusId == 3000)).ToList();
                //change done by FL OKCI Perera , @CBO 25/2020
                //objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.EstablishmentId == UserInfo.LocationId && (x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 2000) || (x.RecordStatusId == 3000 && x.CurrentStatus == UserInfo.RoleId)).ToList();


                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString)).Take(500).ToList();
                }

                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "ServiceType":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceType).ToList();
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID).ToList();
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name).ToList();
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName).ToList();
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate).ToList();
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate).ToList();
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
      
        [HttpGet]
        public ActionResult Individualsearch(string sortOrder, string currentFilter, string searchString, string LocationId, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);

            // var LocationId = _db.UserPermissions.Where(x => x.UserId == UID).Select(x => x.AccessLocationId).ToList();
            ViewBag.DDL_LocationId = new SelectList(_db.UserPermissions.Where(x => x.UserId == UID), "AccessLocationId", "AccessLocationId");

            if (UID != 0)
            {

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                string location = UserInfo.LocationId;

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
                //38746
                //var objVw_FixedAllowance_FLowStatus = _db.Vw_FixedAllowance_FLowStatus.ToList();


                ViewBag.CurrentFilter = searchString;
                //string LocationId_ = "CBO";
                var objFixedAllowanceDetail = from s in _db.Vw_FixedAllowanceDetail
                                              where (s.Active !=0)
                                              select s;

              
                objFixedAllowanceDetail = objFixedAllowanceDetail.Where(x => x.Active !=0);  
              

                if (!String.IsNullOrEmpty(searchString))
                {
                    objFixedAllowanceDetail = objFixedAllowanceDetail.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString) || s.CampAuthority.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "ServiceNo":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                    case "Rank":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.RankID);
                        break;
                    case "Name":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.Name);
                        break;
                    case "AllowanceName":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.AllowanceName);
                        break;
                    case "EffectiveDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.EffectiveDate);
                        break;
                    case "EndDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.EndDate);
                        break;
                    case "CampAuthority":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthority);
                        break;
                    case "CampAuthorityDate":
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderByDescending(s => s.CampAuthorityDate);
                        break;
                    default:
                        objFixedAllowanceDetail = objFixedAllowanceDetail.OrderBy(s => s.ServiceNo);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return View(objFixedAllowanceDetail.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

    }

}