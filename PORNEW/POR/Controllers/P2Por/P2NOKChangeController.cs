﻿using PagedList;
using POR.Models;
using POR.Models.Nok;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers.P2Por
{
    public class P2NOKChangeController : Controller
    {
        dbContext _db = new dbContext();
        DALCommanQueryP2 objDALCommanQueryP2 = new DALCommanQueryP2();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        P3HRMS _dbP3HRMS = new P3HRMS();

        string MacAddress = new DALBase().GetMacAddress();

        int UID =0;
        public P2NOKChangeController()
        {
            
        }
        // GET: P2NOKChange
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            /// Description : Index Page for Forward NOK Change Details

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_NOKChangeHeader> NOKChangeList = new List<_NOKChangeHeader>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
            
            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = RoleId;
                long sno = 0;

                if (!string.IsNullOrEmpty(searchString))
                {
                    //// This write to Search details
                    page = 1;

                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                    sno = Convert.ToInt64(Sno);
                    ViewBag.CurrentFilter = searchString;
                }
                else
                {
                    searchString = currentFilter;
                    ViewBag.CurrentFilter = searchString;
                }

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallNOKSP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("NCHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("NCHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).CopyToDataTable();
                }
                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P2CLERK:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P2OIC:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.KOPNR:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.SNCOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.WOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.HRMSP2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.ACCOUNTS01:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    _NOKChangeHeader obj_NOKChangeDetails = new _NOKChangeHeader();
                    obj_NOKChangeDetails.NOKChangeHeadrerID = Convert.ToInt32(dt3.Rows[i]["NOKCHID"]);
                    obj_NOKChangeDetails.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_NOKChangeDetails.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_NOKChangeDetails.FullName = dt3.Rows[i]["Name"].ToString();
                    obj_NOKChangeDetails.Location = dt3.Rows[i]["Location"].ToString();
                    obj_NOKChangeDetails.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    obj_NOKChangeDetails.CurrentUserRole = UserInfo.RoleId;
                    NOKChangeList.Add(obj_NOKChangeDetails);
                }

                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(NOKChangeList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }      

        [HttpGet]
        public ActionResult Create()
        {
            /// Create By: Sqn ldr Wickramasinghe
            /// Create Date: 09/08/2023  
            /// Description: por clerk's initial step of crate NOK Change,

            try
            {
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");                
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
                ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
                ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Create(_NOKChangeHeader obj_NOKChangeDetails)
        {
            /// Create By:Sqn ldr Wickramsinghe
            /// Create Date: 09/08/2023  
            /// Description: por clerk's initial step of crate NOK Change,

            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            int distCode = Convert.ToInt32(obj_NOKChangeDetails.District);
            List<PORRecordCount> proRecorlist = new List<PORRecordCount>();
            
            /// intial create RSID is 1000, hense we assign manully 1000 to obj_NOKChangeDetails.RSID
            obj_NOKChangeDetails.RSID = (int)Enum.RecordStatus.Insert;

            try
            {
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
                ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
                ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");

                ///Get District name
                var DistDetails = _dbCommonData.Districts.Where(x => x.DIST_CODE == distCode).Select(x => new { x.DESCRIPTION, x.PROV_CODE }).FirstOrDefault();
                var ProvinceName = _dbCommonData.ProvinceNews.Where(x => x.PROV_CODE == DistDetails.PROV_CODE).Select(x => x.DESCRIPTION).FirstOrDefault();

                /// Get the service type related to service number and SNo.
                var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_NOKChangeDetails.ServiceNo).Select(x => new { x.SNo, x.service_type }).FirstOrDefault();
                long ConSNo = Convert.ToInt64(ServiceInfo.SNo);

                if (Session["UID"] != null)
                {
                    UID = Convert.ToInt32(Session["UID"]);

                    //Get the user login Info
                    var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

                    proRecorlist = PorNoCreate(userInfo.LocationId);
                    string CreatePorNo = proRecorlist[0].RefNo;

                    if (ModelState.IsValid)
                    {
                        objNOKChangeHeader.NOKStatus = (int)POR.Enum.NOKtatus.ChangeNOKOnly;
                        objNOKChangeHeader.Sno = ConSNo;
                        objNOKChangeHeader.Location = userInfo.LocationId;
                        objNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                        objNOKChangeHeader.WFDate = Convert.ToDateTime(obj_NOKChangeDetails.WFDate);
                        objNOKChangeHeader.RefNo = CreatePorNo;
                        objNOKChangeHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                        objNOKChangeHeader.Authority = obj_NOKChangeDetails.Authority;
                        objNOKChangeHeader.CreatedBy = UID;
                        objNOKChangeHeader.CreatedDate = DateTime.Now;
                        objNOKChangeHeader.CreatedMac = MacAddress;
                        objNOKChangeHeader.Active = 1;
                        objNOKChangeHeader.CreateIpAddess = this.Request.UserHostAddress;

                        _db.NOKChangeHeaders.Add(objNOKChangeHeader);

                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (_db.SaveChanges() > 0)
                            {
                                /// Enetr NOK header details to NOKChangeDetail

                                var NOKCHID = _db.NOKChangeHeaders.Where(x => x.Sno == ConSNo &&
                                              x.Location == userInfo.LocationId && x.NOKStatus == (int)POR.Enum.NOKtatus.ChangeNOKOnly && x.Active == 1 && x.RefNo == CreatePorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();

                                objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                objNOKChangeDetail.NOKAddress = obj_NOKChangeDetails.NOKAddress;
                                objNOKChangeDetail.NOKName = obj_NOKChangeDetails.NOKName;
                                objNOKChangeDetail.NOKChangeTo = obj_NOKChangeDetails.NOKChangeTo;
                                objNOKChangeDetail.Province = ProvinceName;
                                objNOKChangeDetail.District = DistDetails.DESCRIPTION;
                                objNOKChangeDetail.GSDivision = obj_NOKChangeDetails.GSDivision;
                                objNOKChangeDetail.NearestTown = obj_NOKChangeDetails.NearestTown;
                                objNOKChangeDetail.PoliceStation = obj_NOKChangeDetails.PoliceStation;
                                objNOKChangeDetail.PostOffice = obj_NOKChangeDetails.PostOffice;
                                objNOKChangeDetail.Remarks = obj_NOKChangeDetails.Remarks;
                                objNOKChangeDetail.CreatedBy = UID;
                                objNOKChangeDetail.CreatedDate = DateTime.Now;
                                objNOKChangeDetail.CreatedMac = MacAddress;
                                objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                objNOKChangeDetail.Active = 1;

                                _db.NOKChangeDetails.Add(objNOKChangeDetail);
                                if (_db.SaveChanges() > 0)
                                {
                                    ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                    InsertFlowStatus(NOKCHID, userInfo.RoleId, UID, obj_NOKChangeDetails.FMSID, obj_NOKChangeDetails.RSID);
                                    scope.Complete();
                                    TempData["ScfMsg"] = "Complete Your Process";
                                }
                                else
                                {
                                    scope.Dispose();
                                    TempData["ErrMsg"] = " Not Complete Your Process";
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(obj_NOKChangeDetails);

        }

        public void InsertFlowStatus(int? NOKCHID, int RoleId, int UID, int? FMSID, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            /// Description : Insert NOK Change details to flow status table

            try
            {
                FlowStatusNOKChangeDetail objFlowNOK = new FlowStatusNOKChangeDetail();

                objFlowNOK.NOKHeaderID = NOKCHID;
                objFlowNOK.RecordStatusID = RSID;
                objFlowNOK.UserID = UID;
                objFlowNOK.FlowManagementStatusID = FMSID;
                objFlowNOK.RoleID = RoleId;
                objFlowNOK.CreatedBy = UID;
                objFlowNOK.CreatedDate = DateTime.Now;
                objFlowNOK.CreatedMac = MacAddress;
                objFlowNOK.IPAddress = this.Request.UserHostAddress;
                objFlowNOK.Active = 1;

                _db.FlowStatusNOKChangeDetails.Add(objFlowNOK);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<PORRecordCount> PorNoCreate(string EstablishmentId)
        {
            ///Created BY   : Sqn Ldr Wickramasinghe
            ///Created Date : 2023/08/09
            ///Description : Create POR Number 

            try
            {
                List<PORRecordCount> ListRecordList = new List<PORRecordCount>();

                int currentmonth = Convert.ToInt32(DateTime.Now.Month);
                int currentyear = Convert.ToInt32(DateTime.Now.Year);
                int currentdate = Convert.ToInt32(DateTime.Now.Day);
                int NOKStatus = (int)POR.Enum.NOKtatus.ChangeNOKOnly;

                int jobcount = objDALCommanQueryP2.NokNextRecordId(EstablishmentId, NOKStatus, currentyear);

                //var jobcount = _db.NOKChangeHeaders.Where(x => x.Location == EstablishmentId && x.NOKStatus == (int)POR.Enum.NOKtatus.ChangeNOKOnly && x.CreatedDate.Value.Year == currentyear &&
                //                (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer))).Select(x => x.RecordCount).FirstOrDefault();
                               
                int RocordId = Convert.ToInt32(jobcount + 1);

                string RefNo = EstablishmentId + "/" + "OK-4" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;

                PORRecordCount obj = new PORRecordCount();
                obj.RecordCount = RocordId;
                obj.RefNo = RefNo;
                ListRecordList.Add(obj);

                return ListRecordList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Details(int NOKCHID, int Rejectstatus)
        {
            ///Created BY   : Sqn Ldr Wickramasinghe
            ///Created Date : 2023/08/09
            /// Description : Details related NOK Change Details

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                int? UserRoleId;
                int? CurrentStatusUserRole;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_NOKChangeHeader> NokChangeList = new List<_NOKChangeHeader>();

                UID_ = Convert.ToInt32(Session["UID"]);
                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                var CurrentStatus_UserRole = (from f in _db.FlowStatusNOKChangeDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.NOKHeaderID == NOKCHID
                                              orderby f.NOKHeaderID descending
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

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallNOKSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("NCDActive") == 1 && x.Field<int>("NOKCHID") == NOKCHID).CopyToDataTable();


                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (Rejectstatus != 2)
                {
                    #region Code Area

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _NOKChangeHeader obj_NOKChangeHeader = new _NOKChangeHeader();

                        /// Check the rercord is previously reject or not
                        var prvReject = _db.NOKChangeHeaders.Where(x => x.NOKCHID == NOKCHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();


                        obj_NOKChangeHeader.NOKCHID = Convert.ToInt32(dt2.Rows[i]["NOKCHID"]);
                        obj_NOKChangeHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        //obj_NOKChangeHeader.s = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                        obj_NOKChangeHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_NOKChangeHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_NOKChangeHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_NOKChangeHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_NOKChangeHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_NOKChangeHeader.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_NOKChangeHeader.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_NOKChangeHeader.NOKAddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_NOKChangeHeader.District = dt2.Rows[i]["District"].ToString();
                        obj_NOKChangeHeader.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_NOKChangeHeader.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_NOKChangeHeader.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_NOKChangeHeader.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_NOKChangeHeader.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_NOKChangeHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                        if (prvReject >= 1)
                        {
                            obj_NOKChangeHeader.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_NOKChangeHeader.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_NOKChangeHeader.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_NOKChangeHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }


                        NokChangeList.Add(obj_NOKChangeHeader);
                    }

                    return View(NokChangeList);

                    #endregion                    
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusNOKChangeDetails.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.NOKHeaderID == NOKCHID)
                                        .OrderByDescending(x => x.NOKHeaderID).Select(x => x.RoleID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _NOKChangeHeader obj_NOKChangeHeader = new _NOKChangeHeader();

                        obj_NOKChangeHeader.NOKCHID = Convert.ToInt32(dt2.Rows[i]["NOKCHID"]);
                        obj_NOKChangeHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_NOKChangeHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_NOKChangeHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_NOKChangeHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_NOKChangeHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_NOKChangeHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_NOKChangeHeader.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_NOKChangeHeader.RejectRoleName = RoleName.ToString();
                        obj_NOKChangeHeader.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_NOKChangeHeader.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_NOKChangeHeader.NOKAddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_NOKChangeHeader.District = dt2.Rows[i]["District"].ToString();
                        obj_NOKChangeHeader.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_NOKChangeHeader.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_NOKChangeHeader.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_NOKChangeHeader.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_NOKChangeHeader.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_NOKChangeHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);


                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_NOKChangeHeader.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_NOKChangeHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        NokChangeList.Add(obj_NOKChangeHeader);
                    }

                    return View(NokChangeList);

                }
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult DetIndex_Rejectails(int NOKCHID, int Rejectstatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/08/26
            /// Description : Details related Living In Out por 

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;

                int? CurrentStatusUserRole;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_NOKChangeHeader> NOKChangeList = new List<_NOKChangeHeader>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

                TempData["UserRoleId"] = userInfo.RoleId;

                var CurrentStatus_UserRole = (from f in _db.FlowStatusNOKChangeDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.NOKHeaderID == NOKCHID
                                              orderby f.FSNOKCDID descending
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
                    TempData["CurrentStatusUserRole"] = userInfo.RoleId;
                }

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallNOKSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("NCHActive") == 1 && x.Field<int>("NOKCHID") == NOKCHID).CopyToDataTable();

                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (Rejectstatus != 2)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _NOKChangeHeader obj_NOKChangeDetails = new _NOKChangeHeader();

                        /// Check the rercord is previously reject or not
                        var prvReject = _db.NOKChangeHeaders.Where(x => x.NOKCHID == NOKCHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        obj_NOKChangeDetails.NOKChangeHeadrerID = Convert.ToInt32(dt2.Rows[i]["NOKCHID"]);
                        obj_NOKChangeDetails.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_NOKChangeDetails.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_NOKChangeDetails.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_NOKChangeDetails.Location = dt2.Rows[i]["Location"].ToString();
                        obj_NOKChangeDetails.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_NOKChangeDetails.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_NOKChangeDetails.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_NOKChangeDetails.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_NOKChangeDetails.NOKAddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_NOKChangeDetails.District = dt2.Rows[i]["District"].ToString();
                        obj_NOKChangeDetails.GSDivision = dt2.Rows[i]["GSDivision"].ToString();
                        obj_NOKChangeDetails.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_NOKChangeDetails.PoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_NOKChangeDetails.PostOffice = dt2.Rows[i]["PostOffice"].ToString();
                        obj_NOKChangeDetails.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_NOKChangeDetails.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                        if (prvReject >= 1)
                        {
                            obj_NOKChangeDetails.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_NOKChangeDetails.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_NOKChangeDetails.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_NOKChangeDetails.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;

                        NOKChangeList.Add(obj_NOKChangeDetails);
                    }



                    return View(NOKChangeList);
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusNOKChangeDetails.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.NOKHeaderID == NOKCHID)
                                        .OrderByDescending(x => x.FSNOKCDID).Select(x => x.RoleID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _NOKChangeHeader obj_NOKChangeDetails = new _NOKChangeHeader();

                        obj_NOKChangeDetails.NOKChangeHeadrerID = Convert.ToInt32(dt2.Rows[i]["NOKCHID"]);
                        obj_NOKChangeDetails.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_NOKChangeDetails.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_NOKChangeDetails.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_NOKChangeDetails.Location = dt2.Rows[i]["Location"].ToString();
                        obj_NOKChangeDetails.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_NOKChangeDetails.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_NOKChangeDetails.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_NOKChangeDetails.RejectRoleName = RoleName.ToString();
                        obj_NOKChangeDetails.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_NOKChangeDetails.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_NOKChangeDetails.NOKAddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_NOKChangeDetails.District = dt2.Rows[i]["District"].ToString();
                        obj_NOKChangeDetails.GSDivision = dt2.Rows[i]["GSDivision"].ToString();
                        obj_NOKChangeDetails.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_NOKChangeDetails.PoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_NOKChangeDetails.PostOffice = dt2.Rows[i]["PostOffice"].ToString();
                        obj_NOKChangeDetails.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_NOKChangeDetails.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);


                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_NOKChangeDetails.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_NOKChangeDetails.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;

                        NOKChangeList.Add(obj_NOKChangeDetails);
                    }

                    return View(NOKChangeList);

                }
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }
        public ActionResult Delete(int id)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            ///Description : Delete the P2 Clerk entred data. All Active 1 record turn into 0

            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            int UID_ = Convert.ToInt32(Session["UID"]);

            try
            {

                var NokDId = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == id && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();
                objNOKChangeHeader = _db.NOKChangeHeaders.Find(id);
                objNOKChangeHeader.Active = 0;
                objNOKChangeHeader.ModifiedBy = UID_;
                objNOKChangeHeader.ModifiedDate = DateTime.Now;
                objNOKChangeHeader.ModifiedMac = MacAddress;

                _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                objNOKChangeDetail = _db.NOKChangeDetails.Find(NokDId);
                objNOKChangeDetail.Active = 0;
                objNOKChangeDetail.ModifiedBy = UID_;
                objNOKChangeDetail.ModifiedDate = DateTime.Now;
                objNOKChangeDetail.ModifiedMac = MacAddress;

                _db.Entry(objNOKChangeDetail).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        scope.Complete();
                        TempData["ScfMsg"] = "Record Sucessfully Deleted.";
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsucessfull.";
                        scope.Dispose();
                    }
                }
                return RedirectToAction("Index", "NOKChange");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id, int rejectStatus)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            ///Description : Load Edit page with user enter data.

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_NOKChangeHeader> NOKChangeList = new List<_NOKChangeHeader>();
                string RejectRef;

                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");

                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                var CurrentStatus_UserRole = (from f in _db.FlowStatusLivingInOutDetails
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.InOut_ID == id
                                              orderby f.LFSID descending
                                              select new
                                              {
                                                  u.RoleName,
                                                  u.RID
                                              }).FirstOrDefault();

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallNOKSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("NCHActive") == 1 && x.Field<int>("NOKCHID") == id).CopyToDataTable();

                _NOKChangeHeader obj_NOKChangeDetails = new _NOKChangeHeader();

                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {

                    obj_NOKChangeDetails.NOKChangeHeadrerID = Convert.ToInt32(dt2.Rows[i]["NOKCHID"]);
                    obj_NOKChangeDetails.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_NOKChangeDetails.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_NOKChangeDetails.FullName = dt2.Rows[i]["Name"].ToString();
                    obj_NOKChangeDetails.Location = dt2.Rows[i]["Location"].ToString();
                    obj_NOKChangeDetails.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_NOKChangeDetails.Authority = dt2.Rows[i]["Authority"].ToString();
                    obj_NOKChangeDetails.NOKName = dt2.Rows[i]["NOKName"].ToString();
                    obj_NOKChangeDetails.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                    obj_NOKChangeDetails.NOKAddress = dt2.Rows[i]["NOKAddress"].ToString();
                    obj_NOKChangeDetails.EditedDistrict1 = dt2.Rows[i]["District"].ToString();
                    obj_NOKChangeDetails.EditedGSnumber = dt2.Rows[i]["GSDivision"].ToString();
                    obj_NOKChangeDetails.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                    obj_NOKChangeDetails.EditPoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                    obj_NOKChangeDetails.EditPostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                    obj_NOKChangeDetails.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.NOKChangeHeaders.Where(x => x.NOKCHID == id && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_NOKChangeDetails.RefNo + " " + " - Reject";
                            obj_NOKChangeDetails.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_NOKChangeDetails.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_NOKChangeDetails.RejectRefNo = RejectRef;
                        }
                    }

                    if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                    {
                        obj_NOKChangeDetails.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                    }
                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_NOKChangeDetails.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }


                    //LivingInOutList.Add(obj_LivingInOut);
                }
                //return RedirectToAction("Edit", "User");
                return View(obj_NOKChangeDetails);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }


        }
        [HttpPost]
        public ActionResult Edit(_NOKChangeHeader obj_NOKChangeDetails, int rejectStatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/05/23
            ///Description : Save User edit data to Data based

            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            FlowStatusNOKChangeDetail objFSLNOK = new FlowStatusNOKChangeDetail();

            int UID_ = 0;
            int RoleId = 0;

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            int RecordStatus = (int)POR.Enum.RecordStatus.Insert;
            try
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                ///Get District name
                var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_NOKChangeDetails.DistrictID).Select(x => x.DESCRIPTION).FirstOrDefault();


                objNOKChangeHeader = _db.NOKChangeHeaders.Find(obj_NOKChangeDetails.NOKChangeHeadrerID);

                objNOKChangeHeader.Authority = obj_NOKChangeDetails.Authority;
                objNOKChangeHeader.WFDate = obj_NOKChangeDetails.WFDate;
                objNOKChangeHeader.ModifiedBy = UID_;
                objNOKChangeHeader.ModifiedDate = DateTime.Now;
                objNOKChangeHeader.ModifiedMac = MacAddress;

                _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if ((_db.SaveChanges() > 0))
                    {

                        /// Update Nok Detail table
                        var NOKDID = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == obj_NOKChangeDetails.NOKChangeHeadrerID && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();

                        objNOKChangeDetail = _db.NOKChangeDetails.Find(NOKDID);
                        objNOKChangeDetail.NOKName = obj_NOKChangeDetails.NOKName;

                        if (obj_NOKChangeDetails.RelationshipName != null)
                        {
                            objNOKChangeDetail.NOKChangeTo = obj_NOKChangeDetails.RelationshipName;
                        }
                        objNOKChangeDetail.NOKAddress = obj_NOKChangeDetails.NOKAddress;

                        //// meka kale different type of object assign value
                        if (obj_NOKChangeDetails.DistrictID != 0)
                        {
                            objNOKChangeDetail.District = DistName;
                            if (obj_NOKChangeDetails.Town1 != "SELECT")
                            {
                                objNOKChangeDetail.NearestTown = obj_NOKChangeDetails.Town1;
                            }
                            if (obj_NOKChangeDetails.GSName != "SELECT")
                            {
                                objNOKChangeDetail.GSDivision = obj_NOKChangeDetails.GSName;
                            }
                            if (obj_NOKChangeDetails.PoliceStation1 != "SELECT")
                            {
                                objNOKChangeDetail.PoliceStation = obj_NOKChangeDetails.PoliceStation1;
                            }
                            if (obj_NOKChangeDetails.PostOfficeName != "SELECT")
                            {
                                objNOKChangeDetail.PostOffice = obj_NOKChangeDetails.PostOfficeName;
                            }

                        }
                        _db.Entry(objNOKChangeDetail).State = EntityState.Modified;

                        if (_db.SaveChanges() > 0)
                        {
                            ////Here check the reject record has been edited
                            if (rejectStatus == 2)
                            {
                                ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                RoleId = (int)POR.Enum.UserRole.P3CLERK;
                                InsertFlowStatus(obj_NOKChangeDetails.NOKChangeHeadrerID, RoleId, UID_, obj_NOKChangeDetails.FMSID, RecordStatus);

                                /// Update Living In/Out Header details to table
                                /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                var rejectCount = _db.NOKChangeHeaders.Where(x => x.NOKCHID == obj_NOKChangeDetails.NOKChangeHeadrerID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                objNOKChangeHeader = _db.NOKChangeHeaders.Find(obj_NOKChangeDetails.NOKChangeHeadrerID);
                                if (rejectCount == null)
                                {
                                    objNOKChangeHeader.PreviousReject = 1;
                                }
                                else
                                {
                                    int refIncrement = Convert.ToInt32(rejectCount + 1);
                                    objNOKChangeHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                }
                                objNOKChangeHeader.RejectAuth = obj_NOKChangeDetails.RejectRefNo;
                                _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                                /// previous reject record active 1 status turn in to 0
                                var RejLFSID = _db.FlowStatusNOKChangeDetails.Where(x => x.NOKHeaderID == obj_NOKChangeDetails.NOKChangeHeadrerID && x.RecordStatusID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.FSNOKCDID).FirstOrDefault();
                                objFSLNOK = _db.FlowStatusNOKChangeDetails.Find(RejLFSID);
                                objFSLNOK.Active = 0;
                                objFSLNOK.ModifiedBy = UID_;
                                objFSLNOK.ModifiedDate = DateTime.Now;
                                objFSLNOK.ModifiedMac = MacAddress;
                                _db.Entry(objFSLNOK).State = EntityState.Modified;

                                if (_db.SaveChanges() > 0)
                                {
                                    scope.Complete();
                                    TempData["ScfMsg"] = "Complete Your Process";
                                }
                                else
                                {
                                    scope.Dispose();
                                    TempData["ErrMsg"] = "Not Complete Your Process";
                                }
                            }
                            else
                            {
                                scope.Complete();
                                TempData["ScfMsg"] = "Complete Your Process";
                            }
                        }
                        else
                        {
                            scope.Dispose();
                            TempData["ErrMsg"] = " Not Complete Your Process";
                        }
                    }
                    else
                    {
                        scope.Dispose();
                        TempData["ErrMsg"] = " Not Complete Your Process";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("Index", "NOKChange");

        }

        [HttpGet]
        public ActionResult Forward(int? id)
        {
            //Singal Forward

            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023/02/14
            ///Des: Data forward to user by user, get the data from flowmanagemt table 

            //Singal Forward        
            int? UID = 0;
            if (Session["UID"] != null)
            {
                UID = Convert.ToInt32(Session["UID"]);
            }

            string SubmitStatus_UserRole;
            bool updateStatus = false;
            //int? NextFlowStatusId;
            ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

            if (UID != 0)
            {
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
                var RecordInfo = _db.NOKChangeHeaders.Where(x => x.NOKCHID == id).Select(x => new { x.Location, x.ServiceTypeId }).FirstOrDefault();
                int? SubmitStatus = NextFlowStatusId(id, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceTypeId, userInfo.DivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                FlowStatusNOKChangeDetail objFlowStatusNOKDetails = new FlowStatusNOKChangeDetail();
                FlowStatusLivingInOutDetail objFlowStatusLivingInOut = new FlowStatusLivingInOutDetail();

                objFlowStatusNOKDetails.NOKHeaderID = id;
                objFlowStatusNOKDetails.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                objFlowStatusNOKDetails.UserID = UID;
                objFlowStatusNOKDetails.FlowManagementStatusID = SubmitStatus;
                objFlowStatusNOKDetails.RoleID = UserRoleId;
                objFlowStatusNOKDetails.CreatedBy = UID;
                objFlowStatusNOKDetails.CreatedDate = DateTime.Now;
                objFlowStatusNOKDetails.CreatedMac = MacAddress;
                objFlowStatusNOKDetails.IPAddress = this.Request.UserHostAddress;
                objFlowStatusNOKDetails.Active = 1;

                ///This function is to update the Hrmis data base. After account one certified, the details will update p3hrmis and P2hrms 
                if (userInfo.RoleId == (int)POR.Enum.UserRole.ACCOUNTS01)
                {

                    updateStatus = UpdateHrmis(MacAddress, UID, id);

                    if (updateStatus == true)
                    {
                        _db.FlowStatusNOKChangeDetails.Add(objFlowStatusNOKDetails);

                        if (_db.SaveChanges() > 0)
                        {
                            TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + " " + " & Update the HRMIS";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Something error in HRMS Record. Please Contact ITW";
                    }
                }
                else
                {
                    _db.FlowStatusNOKChangeDetails.Add(objFlowStatusNOKDetails);
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;

                        return RedirectToAction("Index", new { serviceStatus = 0 });

                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";

                    }
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
            string RecordEstablishmentId = null;
            int? UserRoleId = 0;
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            if (id != null)
            {
                foreach (int IDs in id)
                {
                    if (UID != 0)
                    {

                        var RecordInfo = _db.NOKChangeHeaders.Where(x => x.NOKCHID == IDs).Select(x => new { x.Location, x.ServiceTypeId }).FirstOrDefault();
                        int? SubmitStatus = NextFlowStatusId(IDs, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceTypeId, userInfo.DivisionId);
                        //Get Next FlowStatus User Role Name for Add Successfull Msg

                        UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                        SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                        FlowStatusNOKChangeDetail objFlowStatusNOKDetails = new FlowStatusNOKChangeDetail();


                        objFlowStatusNOKDetails.NOKHeaderID = IDs;
                        objFlowStatusNOKDetails.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                        objFlowStatusNOKDetails.UserID = UID;
                        objFlowStatusNOKDetails.FlowManagementStatusID = SubmitStatus;
                        objFlowStatusNOKDetails.RoleID = UserRoleId;
                        objFlowStatusNOKDetails.CreatedBy = UID;
                        objFlowStatusNOKDetails.CreatedDate = DateTime.Now;
                        objFlowStatusNOKDetails.CreatedMac = MacAddress;
                        objFlowStatusNOKDetails.IPAddress = this.Request.UserHostAddress;
                        objFlowStatusNOKDetails.Active = 1;

                        _db.FlowStatusNOKChangeDetails.Add(objFlowStatusNOKDetails);
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
                        TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + "-" + userInfo.LocationId;
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
            }


            return Json("");
        }
        public int? NextFlowStatusId(int? NOKCHID, string UserEstablishmentId, string RecordEstablishmentId, int? ServiceTypeId, string UserDivisionId)
        {
            ///Created By   : Sqn ldr Wickramasinghe
            ///Created Date :2023.08.09
            ///Des: get the next flow status id FMSID

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxFSNOKCDID = _db.FlowStatusNOKChangeDetails.Where(x => x.NOKHeaderID == NOKCHID).Select(x => x.FSNOKCDID).Max();
                int? CurrentFMSID = _db.FlowStatusNOKChangeDetails.Where(x => x.FSNOKCDID == MaxFSNOKCDID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? SubmitStatus = 0;


                int? UID = Convert.ToInt32(Session["UID"]);


                //LHID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();

                    SubmitStatus = _db.FlowManagementStatus.Where(x => x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.UserRoleID == RoleId).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                }
                else if (CurrentUserRole == (int)POR.Enum.UserRole.P2OIC || CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO)
                {
                    SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                          && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                    var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OK" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();


                    if (SubmitStatus == (int)POR.Enum.UserRole.KOPNR)
                    {
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.KOPNR && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();
                    }
                    else
                    {
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();
                    }
                }
                else
                {
                    //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                    var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OK" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                    SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                   && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                    //SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                    if (SubmitStatus == (int)POR.Enum.UserRole.SNCOSALARY)
                    {
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.SNCOSALARY && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();
                    }
                    else if (SubmitStatus == (int)POR.Enum.UserRole.ACCOUNTS01)
                    {
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.ACCOUNTS01 && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();

                    }
                    else
                    {
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || 
                                x.DivisionId2 == UserDivisionId || x.DivisionId3 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        [HttpGet]
        public ActionResult Reject(int NOKCHID, int FMSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            /// Description : this function is to reject the record

            _NOKChangeHeader model = new _NOKChangeHeader();
            try
            {
                model.NOKChangeHeadrerID = NOKCHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_RejectCommentNOKChange", model);
        }
        public int? PreviousFlowStatusId(int? NOKCHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId, int? ServiceTypeId)
        {
            ///Created By   : Sqn ldr Wickramasinghe
            ///Created Date :2023/08/08
            ///Des: get the reject flow status id FMSID

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFSNOKCDID = _db.FlowStatusNOKChangeDetails.Where(x => x.NOKHeaderID == NOKCHID).Select(x => x.FSNOKCDID).Max();
                int? CurrentFMSID = _db.FlowStatusNOKChangeDetails.Where(x => x.FSNOKCDID == MaxFSNOKCDID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);

                //LHID=Null (actclk create record)
                if (CurrentUserRole == null)
                {

                    //Get First FMSID if Current FMSID is null
                    FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).First();

                }
                else if (CurrentUserRole == (int)POR.Enum.UserRole.P2OIC || CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO)
                {
                    RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                           && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                }
                else
                {
                    //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                    var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OK" && x.Active == 1).Select(x => x.FlowGroupP2).FirstOrDefault();
                    RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                   && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        [HttpGet]
        public ActionResult IndexRejectNOKChange(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/08/09
            /// Description : Index Page for Forward CiviStatus POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_NOKChangeHeader> NOKChangeList = new List<_NOKChangeHeader>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            //serviceStatus = 0;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = UserInfo.RoleId;
               
                ViewBag.CurrentFilter = searchString;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallNOKRejectSP();
               
                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                var FMSID = _db.FlowManagementStatus.Where(x => (x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId) && (x.EstablishmentId == LocationId && x.UserRoleID == UserInfo.RoleId)).Select(x => x.FMSID).FirstOrDefault();

                TempData["RoleId"] = UserInfo.RoleId;

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("NCHActive") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    /// here send a serviceStatus as 1 is to categorized the officer and other rank seperettly

                    dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("NCHActive") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                    if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P2SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P2OIC)
                    {
                        dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();
                    }
                    else
                    {
                        dt3 = dt2;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _NOKChangeHeader obj_NOKChangeDetails = new _NOKChangeHeader();
                        obj_NOKChangeDetails.NOKChangeHeadrerID = Convert.ToInt32(dt3.Rows[i]["NOKCHID"]);
                        obj_NOKChangeDetails.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        obj_NOKChangeDetails.Rank = dt3.Rows[i]["Rank"].ToString();
                        obj_NOKChangeDetails.FullName = dt3.Rows[i]["Name"].ToString();
                        obj_NOKChangeDetails.Location = dt3.Rows[i]["Location"].ToString();
                        obj_NOKChangeDetails.RefNo = dt3.Rows[i]["RefNo"].ToString();
                        NOKChangeList.Add(obj_NOKChangeDetails);

                    }
                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(NOKChangeList.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P3CLERK:
                            return RedirectToAction("P3ClkHome", "Home");

                        case (int)POR.Enum.UserRole.P2CLERK:
                            return RedirectToAction("P2ClkHome", "Home");

                        default:
                            return RedirectToAction("Index", "Home");

                    }
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpGet]
        public ActionResult RejectConfirm(int id)
        {
            ///Created BY   : Sqn ldr Wickramasinghe 
            ///Created Date : 2023/08/08
            /// Description : P3 Clerk finally confirm the reject Confirm. Afetr confirm record Status came to 0

            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                //Update CivilStatusHeader Active colum to 0
                NOKChangeHeader objNOKChangeHeader = _db.NOKChangeHeaders.Find(id);

                objNOKChangeHeader.Active = 0;
                objNOKChangeHeader.ModifiedBy = UID_;
                objNOKChangeHeader.ModifiedDate = DateTime.Now;
                objNOKChangeHeader.ModifiedMac = MacAddress;
                _db.Entry(objNOKChangeHeader).Property(x => x.Active).IsModified = true;

                if (_db.SaveChanges() > 0)
                {
                    TempData["ScfMsg"] = "Successfully Reject Confirmed.";
                }
                return RedirectToAction("IndexRejectNOKChange");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe    
            ///Created Date : 2023/08/09
            /// Description : Load data user roll wise, this fuction call from IndexLivingINOutStatus()

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                //int serviceStatus = 1;

                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P2CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                        }
                        break;
                    #endregion

                    default:

                        #region CodeArea
                        ///Data Table first row CurrentStatus gave null value, hence it occures an error. Because of that check the column and delete that row, 

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();


                        /// Check the data table has row or not
                        result = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                /// here send a serviceStatus as 1 from index function. this is to categorized the officer and other rank seperettly
                                var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                            || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                            || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                if (row.Any())
                                {
                                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                }

                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer));

                                    if (row.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                       (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                       x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer));

                                    if (row.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).CopyToDataTable();
                                    }
                                }
                                else
                                {
                                    if (RoleId != (int)POR.Enum.UserRole.ACCOUNTS01)
                                    {
                                        var Count2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                             (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                        if (Count2 != 0)
                                        {
                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                                      (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                        }
                                    }
                                    else
                                    {
                                        var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                         (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                         x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                        if (Count != 0)
                                        {
                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                                      (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                        }
                                    }

                                }
                            }
                        }
                        #endregion

                        break;
                }
                return dt2;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool UpdateHrmis(string MacAddress, int? UID, int? NOKCHID)
        {

            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/02/03
            /// Description : update the hrmis

            DataTable dt = new DataTable();
            DataTable civildt = new DataTable();
            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.NokChange;
            bool status = false;
            bool status2 = false;
            bool status3 = false;

            var SNo = _db.NOKChangeHeaders.Where(x => x.NOKCHID == NOKCHID && x.Active == 1).Select(x => x.Sno).FirstOrDefault();
            string SSNo = Convert.ToString(SNo);
            try
            {
                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(NOKCHID, detailsCollectCategory);

                ///Generate NOK ID
                long key = objDALCommanQuery.GenerateKey("NOK_Change_Details", "NOKID");

                string NokID = Convert.ToString(SNo + "/" + key);

                /// Update the perivous NoK type into 2                
                status = objDALCommanQuery.UpdatePreviousNokTypeId(SNo, UID, MacAddress);

                if (status == true)
                {
                    /// Insert new record to NOKChangeHeader table in P3 hrmis data based
                    //status2 = InsertNewNokRecordToHRMIS(NokID, SNo.ToString(), UID, dt);
                }
                else
                {
                    status2 = false;
                }

                if (status == true && status2 == true)
                {
                    return status3 = true;
                }
                else
                {
                    return status3 = false;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        #region Json Methods
        public JsonResult getname(string id)
        {
            ///Created BY   : Flt Lt SMSL Samaratunge
            ///Created Date : 2021/05/31
            /// Description : Auto Load Rank and Name as per the Svc No

            Vw_PersonalDetail obj_VwPersonalProfile = new Vw_PersonalDetail();

            var count = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Count();
            if (count == 1)
            {
                obj_VwPersonalProfile = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).FirstOrDefault();

            }
            else
            {
                obj_VwPersonalProfile.Name = "Service Number is not valid.";
            }


            return Json(obj_VwPersonalProfile, JsonRequestBehavior.AllowGet);

        }
        public JsonResult FromTown(int id)
        {
            ///Created BY   : Sqn ldr Wickramasinghw
            ///Created Date : 2023/08/09
            /// Description : Load Town according to distrit

            List<District> FromTown = new List<District>();

            var LinqFromTown = this.LinqFromTown(id);

            var TownListData = LinqFromTown.Select(x => new SelectListItem()
            {
                Text = x.Town.ToString(),
                Value = x.Town.ToString(),
            });


            return Json(TownListData, JsonRequestBehavior.AllowGet);
        }
        public IList<P2_Town> LinqFromTown(int id)
        {
            ///Created BY   : Sqn ldr Wickramasinghw
            ///Created Date : 2023/08/09
            /// Description : Load Town according to distrit

            List<P2_Town> Result = new List<P2_Town>();
            Result = _dbCommonData.P2_Town.Where(x => x.DIST_CODE == id).OrderBy(x => x.Town).ToList();

            return Result;
        }
        public JsonResult FromPostOffice(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/04/29
            /// Description : Load Post office according to distrit

            List<District> FromPostOffice = new List<District>();

            var LinqFromPostOffice = this.LinqFromPostOffice(id);

            var TownListData = LinqFromPostOffice.Select(x => new SelectListItem()
            {
                Text = x.POST_NAME.ToString(),
                Value = x.POST_NAME.ToString(),
            });


            return Json(TownListData, JsonRequestBehavior.AllowGet);
        }
        public IList<P2_PostCode> LinqFromPostOffice(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/18
            /// Description :  Load Town according to distrit

            List<P2_PostCode> Result = new List<P2_PostCode>();
            Result = _dbCommonData.P2_PostCode.Where(x => x.DIST_CODE == id).OrderBy(x => x.POST_NAME).ToList();

            return Result;
        }
        #endregion

    }
}