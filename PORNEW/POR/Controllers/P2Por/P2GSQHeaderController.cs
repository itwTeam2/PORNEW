    using POR.Models;
using POR.Models.LivingInOut;
using ReportData.DAL;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using System.Data.Entity;
using ReportData.BAL;

namespace POR.Controllers.P2Por
{
    public class P2GSQHeaderController : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        P3HRMS _dbP3HRMS = new P3HRMS();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        DALCommanQueryP2 objDALCommanQueryP2 = new DALCommanQueryP2();
        string MacAddress = new DALBase().GetMacAddress();
        

        // GET: P2GSQHeader
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Index Page for Forward CiviStatus POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_GSQHeader> GSQHeaderList = new List<_GSQHeader>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();

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
                dt = objDALCommanQuery.CallGSQSP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                }
                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P2CLERK:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P2SNCO:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P2OIC:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.KOPNR:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.SNCOSALARY:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.WOSALARY:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.HRMSP2SNCO:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.ACCOUNTS01:
                        dt3 = loadDataUserWise(UserInfo.RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    default:
                        break;
                   
                }
                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    _GSQHeader obj_GSQHeader = new _GSQHeader();

                    obj_GSQHeader.GSQHID = Convert.ToInt32(dt3.Rows[i]["GSQHID"]);
                    obj_GSQHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_GSQHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_GSQHeader.Name = dt3.Rows[i]["Name"].ToString();
                    obj_GSQHeader.GSQLocation = dt3.Rows[i]["GSQLocation"].ToString();
                    obj_GSQHeader.GSQStatusName = dt3.Rows[i]["StatusName"].ToString();
                    obj_GSQHeader.CurrentUserRole = UserInfo.RoleId;

                    GSQHeaderList.Add(obj_GSQHeader);

                }
                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(GSQHeaderList.ToPagedList(pageNumber, pageSize));
            }

            return View();
        }

        [HttpGet]
        public ActionResult CreateAllocateMQ()
        {
            ///Create By: Flt Lt WAKY Wickramasinghe
            ///Create Date: 2023/03/15
            ///Description: GSQ details enter view

            ViewBag.DDL_Location = new SelectList(_dbCommonData.Establishments, "LocationID", "LocationName");
            ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
            ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
            ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
            ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus, "GSQSID", "StatusName");
            ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
            ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
            ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");
            ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");

            if (Session["UID"] != null)
            {
                int UID_ = Convert.ToInt32(Session["UID"]);
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
            }
            else
            {

            }

            return View();
        }

        [HttpPost]
        public ActionResult CreateAllocateMQ(_GSQHeader obj_GSQHeader, string VacantDate, string WorkingSLAF, string MQRecovery)
        {
            ///Create By: Flt lt Wickramasinghe
            ///Create Date: 2023/03/15  
            ///Description: Data Insert Saving method include

            GSQHeader objGSQHeader = new GSQHeader();
            FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            string MacAddress = new DALBase().GetMacAddress();
            List<PORRecordCount> proRecorlist = new List<PORRecordCount>();
            
            ///WorkingSLAF = 1 (Spouse/Husband work in SLAF)
            ///WorkingSLAF =  (Spouse/Husband not work in SLAF)

            //MQ Recovery Type = 1 (Basic 10%)
            //MQ Recovery Type = 2 (Valuation)

            int UID_ = 0;
            int RoleId = 0;

            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => new { x.LocationId,x.RoleId}).FirstOrDefault();               
                RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();

                try
                {
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.Establishments, "LocationID", "LocationName");
                    ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
                    ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                    ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
                    ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus, "GSQSID", "StatusName");
                    ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                    ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
                    ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");
                    ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");

                    /// Get the service Info related to service number.
                    var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_GSQHeader.ServiceNo).Select(x => new { x.SNo, x.service_type }).FirstOrDefault();

                    if (ServiceInfo != null)
                    {
                        long ConSNo = Convert.ToInt64(ServiceInfo.SNo);

                        //// check the duplication for gsq entering data
                        int geqStatus = obj_GSQHeader.StatusName;

                        proRecorlist = PorNoCreate(userInfo.LocationId, geqStatus);

                        string CreatePorNo = proRecorlist[0].RefNo;

                        string ConPROV_CODE = obj_GSQHeader.ProvinceId.ToString();
                        ///Get District name
                        var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_GSQHeader.District).Select(x => x.DESCRIPTION).FirstOrDefault();
                        var ProvinceName = _dbCommonData.ProvinceNews.Where(x => x.PROV_CODE == ConPROV_CODE).Select(x => x.DESCRIPTION).FirstOrDefault();

                        ModelState.Remove("NOKWDate");

                        obj_GSQHeader.RSID = (int)POR.Enum.RecordStatus.Insert;

                        if (obj_GSQHeader.Location == "   ")
                        {
                            TempData["ErrMsg"] = "Please Select correct MQ Location.";
                        }
                        else
                        {
                            switch (geqStatus)
                            {
                                case (int)POR.Enum.GSQStatus.Allocate:

                                    #region CodeSegment

                                    var checkGsqVacantStatus = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Vacant && x.Active == 1).Count();

                                    if (checkGsqVacantStatus != 0)
                                    {
                                        #region CodeRegion
                                        var checkAllocateDup = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();

                                        if (checkAllocateDup == 1)
                                        {
                                            TempData["ErrMsg"] = "This person can not allocate an MQ and already has an MQ.";
                                        }
                                        else
                                        {
                                            if (ConSNo != 0 && obj_GSQHeader.Location != null && obj_GSQHeader.GSQNo != null && obj_GSQHeader.AllocatedDate != null)
                                            {
                                                objGSQHeader.Sno = ConSNo;
                                                objGSQHeader.ServiceTypeId = ServiceInfo.service_type;
                                                objGSQHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                                                objGSQHeader.ISSpouseHusbandWrkSLAF = Convert.ToInt16(WorkingSLAF);
                                                objGSQHeader.GSQLocation = obj_GSQHeader.Location;
                                                objGSQHeader.Location = userInfo.LocationId;
                                                objGSQHeader.GSQNo = obj_GSQHeader.GSQNo;
                                                objGSQHeader.MQRecoveryType = Convert.ToInt16(MQRecovery);
                                                objGSQHeader.GSQStatus = obj_GSQHeader.StatusName;
                                                objGSQHeader.AllocatedDate = Convert.ToDateTime(obj_GSQHeader.AllocatedDate);
                                                objGSQHeader.RefNo = CreatePorNo;
                                                objGSQHeader.Authority = obj_GSQHeader.Authority;

                                                //Default Data
                                                objGSQHeader.CreatedBy = UID_;
                                                objGSQHeader.CreatedDate = DateTime.Now;
                                                objGSQHeader.CreatedMac = MacAddress;
                                                objGSQHeader.Active = 1;
                                                objGSQHeader.CreateIpAddess = this.Request.UserHostAddress;
                                                _db.GSQHeaders.Add(objGSQHeader);

                                                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    if (_db.SaveChanges() > 0)
                                                    {
                                                        var GSQHID = _db.GSQHeaders.Where(x => x.RefNo == CreatePorNo
                                                             && x.GSQLocation == obj_GSQHeader.Location && x.Active == 1).OrderByDescending(x => x.CreatedDate).Select(x => x.GSQHID).FirstOrDefault();

                                                        int FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == userInfo.LocationId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).FirstOrDefault();

                                                        var NokStatus = _db.NOKStatus.Where(x => x.SubCatID == 200 && x.Active == 1).Select(x => x.NOKSID).FirstOrDefault();

                                                        objNOKChangeHeader.GSQHeaderID = GSQHID;
                                                        objNOKChangeHeader.NOKStatus = NokStatus;
                                                        objNOKChangeHeader.Sno = ConSNo;
                                                        objNOKChangeHeader.Location = userInfo.LocationId;
                                                        objNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                                                        objNOKChangeHeader.WFDate = obj_GSQHeader.NOKWDate;
                                                        objNOKChangeHeader.Authority = obj_GSQHeader.Authority;
                                                        objNOKChangeHeader.RefNo = CreatePorNo;
                                                        objNOKChangeHeader.CreatedBy = UID_;
                                                        objNOKChangeHeader.CreatedDate = DateTime.Now;
                                                        objNOKChangeHeader.CreatedMac = MacAddress;
                                                        objNOKChangeHeader.Active = 1;
                                                        objNOKChangeHeader.CreateIpAddess = this.Request.UserHostAddress;

                                                        _db.NOKChangeHeaders.Add(objNOKChangeHeader);

                                                        if (_db.SaveChanges() > 0)
                                                        {
                                                            /// Enetr NOK header details to NOKChangeDetail
                                                            /// 
                                                            var NOKCHID = _db.NOKChangeHeaders.Where(x => x.Sno == ConSNo &&
                                                                          x.Location == userInfo.LocationId && x.NOKStatus == NokStatus && x.Active == 1 && x.GSQHeaderID == GSQHID).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();

                                                            objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                                            objNOKChangeDetail.NOKAddress = obj_GSQHeader.NOKaddress;
                                                            objNOKChangeDetail.NOKName = obj_GSQHeader.NOKName;
                                                            objNOKChangeDetail.NOKChangeTo = obj_GSQHeader.NOKRelationship1;
                                                            objNOKChangeDetail.Province = ProvinceName;
                                                            objNOKChangeDetail.District = DistName;
                                                            objNOKChangeDetail.GSDivision = obj_GSQHeader.GSName;
                                                            objNOKChangeDetail.NearestTown = obj_GSQHeader.Town1;
                                                            objNOKChangeDetail.PoliceStation = obj_GSQHeader.PoliceStation1;
                                                            objNOKChangeDetail.PostOffice = obj_GSQHeader.PostOfficeName;
                                                            objNOKChangeDetail.Remarks = obj_GSQHeader.Remarks;
                                                            objNOKChangeDetail.CreatedBy = UID_;
                                                            objNOKChangeDetail.CreatedDate = DateTime.Now;
                                                            objNOKChangeDetail.CreatedMac = MacAddress;
                                                            objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                                            objNOKChangeDetail.Active = 1;

                                                            _db.NOKChangeDetails.Add(objNOKChangeDetail);

                                                            if (_db.SaveChanges() > 0)
                                                            {
                                                                /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                                                InserFlowStatus(GSQHID, RoleId, UID_, obj_GSQHeader.FMSID, obj_GSQHeader.RSID);
                                                                scope.Complete();
                                                                TempData["ScfMsg"] = "Data Added Successfully";
                                                            }
                                                            else
                                                            {
                                                                scope.Dispose();
                                                                TempData["ErrMsg"] = " Not Complete Your Process";
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        scope.Dispose();
                                                        TempData["ErrMsg"] = "Process Unsuccessfull";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TempData["ErrMsg"] = "Please Fill all the details - Check the Allocated Date field.";
                                            }
                                        }
                                        #endregion

                                    }
                                    else
                                    {
                                        #region Code Region

                                        var checkAllDupl = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();
                                        if (checkAllDupl == 0)
                                        {
                                            //// First time Record Entering 
                                            if (ConSNo != 0 && obj_GSQHeader.Location != null && obj_GSQHeader.GSQNo != null && obj_GSQHeader.AllocatedDate != null)
                                            {

                                                objGSQHeader.Sno = ConSNo;
                                                objGSQHeader.ServiceTypeId = ServiceInfo.service_type;
                                                objGSQHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                                                objGSQHeader.ISSpouseHusbandWrkSLAF = Convert.ToInt16(WorkingSLAF);
                                                objGSQHeader.GSQLocation = obj_GSQHeader.Location;
                                                objGSQHeader.Location = userInfo.LocationId;
                                                objGSQHeader.GSQNo = obj_GSQHeader.GSQNo;
                                                objGSQHeader.GSQStatus = obj_GSQHeader.StatusName;
                                                objGSQHeader.MQRecoveryType = Convert.ToInt16(MQRecovery);
                                                objGSQHeader.AllocatedDate = Convert.ToDateTime(obj_GSQHeader.AllocatedDate);
                                                objGSQHeader.RefNo = CreatePorNo;
                                                objGSQHeader.Authority = obj_GSQHeader.Authority;

                                                //Default Data
                                                objGSQHeader.CreatedBy = UID_;
                                                objGSQHeader.CreatedDate = DateTime.Now;
                                                objGSQHeader.CreatedMac = MacAddress;
                                                objGSQHeader.Active = 1;
                                                objGSQHeader.CreateIpAddess = this.Request.UserHostAddress;
                                                _db.GSQHeaders.Add(objGSQHeader);

                                                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                                {
                                                    if (_db.SaveChanges() > 0)
                                                    {
                                                        var GSQHID = _db.GSQHeaders.Where(x => x.RefNo == CreatePorNo
                                                                       && x.GSQLocation == obj_GSQHeader.Location && x.Active == 1).OrderByDescending(x => x.CreatedDate).Select(x => x.GSQHID).FirstOrDefault();

                                                        int FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == userInfo.LocationId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).FirstOrDefault();

                                                        var NokStatus = _db.NOKStatus.Where(x => x.SubCatID == 200 && x.Active == 1).Select(x => x.NOKSID).FirstOrDefault();

                                                        objNOKChangeHeader.GSQHeaderID = GSQHID;
                                                        objNOKChangeHeader.NOKStatus = NokStatus;
                                                        objNOKChangeHeader.Sno = ConSNo;
                                                        objNOKChangeHeader.Location = userInfo.LocationId;
                                                        objNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                                                        objNOKChangeHeader.WFDate = obj_GSQHeader.NOKWDate;
                                                        objNOKChangeHeader.Authority = obj_GSQHeader.Authority;
                                                        objNOKChangeHeader.RefNo = CreatePorNo;
                                                        objNOKChangeHeader.CreatedBy = UID_;
                                                        objNOKChangeHeader.CreatedDate = DateTime.Now;
                                                        objNOKChangeHeader.CreatedMac = MacAddress;
                                                        objNOKChangeHeader.Active = 1;
                                                        objNOKChangeHeader.CreateIpAddess = this.Request.UserHostAddress;

                                                        _db.NOKChangeHeaders.Add(objNOKChangeHeader);

                                                        if (_db.SaveChanges() > 0)
                                                        {
                                                            /// Enetr NOK header details to NOKChangeDetail
                                                            /// 
                                                            var NOKCHID = _db.NOKChangeHeaders.Where(x => x.Sno == ConSNo &&
                                                                          x.Location == userInfo.LocationId && x.NOKStatus == NokStatus && x.Active == 1 && x.GSQHeaderID == GSQHID).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();

                                                            objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                                            objNOKChangeDetail.NOKAddress = obj_GSQHeader.NOKaddress;
                                                            objNOKChangeDetail.NOKName = obj_GSQHeader.NOKName;
                                                            objNOKChangeDetail.NOKChangeTo = obj_GSQHeader.NOKRelationship1;
                                                            objNOKChangeDetail.Province = ProvinceName;
                                                            objNOKChangeDetail.District = DistName;
                                                            objNOKChangeDetail.GSDivision = obj_GSQHeader.GSName;
                                                            objNOKChangeDetail.NearestTown = obj_GSQHeader.Town1;
                                                            objNOKChangeDetail.PoliceStation = obj_GSQHeader.PoliceStation1;
                                                            objNOKChangeDetail.PostOffice = obj_GSQHeader.PostOfficeName;
                                                            objNOKChangeDetail.Remarks = obj_GSQHeader.Remarks;
                                                            objNOKChangeDetail.CreatedBy = UID_;
                                                            objNOKChangeDetail.CreatedDate = DateTime.Now;
                                                            objNOKChangeDetail.CreatedMac = MacAddress;
                                                            objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                                            objNOKChangeDetail.Active = 1;

                                                            _db.NOKChangeDetails.Add(objNOKChangeDetail);

                                                            if (_db.SaveChanges() > 0)
                                                            {
                                                                InserFlowStatus(GSQHID, RoleId, UID_, obj_GSQHeader.FMSID, obj_GSQHeader.RSID);
                                                                scope.Complete();
                                                                TempData["ScfMsg"] = "Data Added Successfully";
                                                            }
                                                            else
                                                            {
                                                                scope.Dispose();
                                                                TempData["ErrMsg"] = "Process Unsuccessfull";
                                                            }
                                                        }
                                                        /// This function is to  Enter intial record Status in to FlowStatusDetails                                                   

                                                    }
                                                    else
                                                    {
                                                        scope.Dispose();
                                                        TempData["ErrMsg"] = "Process Unsuccessfull";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TempData["ErrMsg"] = "Please Fill all the details - Check the Allocated Date field.";
                                            }
                                        }
                                        else
                                        {
                                            TempData["ErrMsg"] = "This person already has a MQ.";
                                        }

                                        #endregion
                                    }

                                    #endregion

                                    break;
                                case (int)POR.Enum.GSQStatus.Vacant:

                                    #region CodeSegment

                                    GSQHeader obj = new GSQHeader();
                                    /// 1st check if it has allocation active 1 record
                                    var checkAllocateDuplicate = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();

                                    if (checkAllocateDuplicate == 1)
                                    {

                                        if (ConSNo != 0 && obj_GSQHeader.Location != null && obj_GSQHeader.GSQNo != null && obj_GSQHeader.VacantDate != null)
                                        {
                                            var GQHeaderID = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Select(x => x.GSQHID).FirstOrDefault();

                                            /// Update the previous Allocation record Active Status to 0
                                            obj = _db.GSQHeaders.Find(GQHeaderID);
                                            obj.Active = 0;
                                            obj.ModifiedBy = UID_;
                                            obj.ModifiedDate = DateTime.Now;
                                            obj.ModifiedMac = MacAddress;

                                            _db.Entry(obj).State = System.Data.Entity.EntityState.Modified;

                                            /// after update the allocation record, Insert the new Allocation record.
                                            objGSQHeader.Sno = ConSNo;
                                            objGSQHeader.ServiceTypeId = ServiceInfo.service_type;
                                            objGSQHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                                            objGSQHeader.ISSpouseHusbandWrkSLAF = Convert.ToInt16(WorkingSLAF);
                                            objGSQHeader.GSQLocation = obj_GSQHeader.Location;
                                            objGSQHeader.Location = userInfo.LocationId;
                                            objGSQHeader.GSQNo = obj_GSQHeader.GSQNo;
                                            objGSQHeader.GSQStatus = obj_GSQHeader.StatusName;
                                            objGSQHeader.MQRecoveryType = Convert.ToInt16(MQRecovery);
                                            objGSQHeader.VacantDate = Convert.ToDateTime(obj_GSQHeader.VacantDate);
                                            objGSQHeader.RefNo = CreatePorNo;
                                            objGSQHeader.Authority = obj_GSQHeader.Authority;

                                            //Default Data
                                            objGSQHeader.CreatedBy = UID_;
                                            objGSQHeader.CreatedDate = DateTime.Now;
                                            objGSQHeader.CreatedMac = MacAddress;
                                            objGSQHeader.Active = 1;
                                            objGSQHeader.CreateIpAddess = this.Request.UserHostAddress;
                                            _db.GSQHeaders.Add(objGSQHeader);


                                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                            {
                                                if (_db.SaveChanges() > 0)
                                                {
                                                    var GSQHID = _db.GSQHeaders.Where(x => x.RefNo == CreatePorNo
                                                                   && x.GSQLocation == obj_GSQHeader.Location && x.Active == 1).OrderByDescending(x => x.CreatedDate).Select(x => x.GSQHID).FirstOrDefault();

                                                    int FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == userInfo.LocationId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).FirstOrDefault();

                                                    var NokStatus = _db.NOKStatus.Where(x => x.SubCatID == 200 && x.Active == 1).Select(x => x.NOKSID).FirstOrDefault();

                                                    objNOKChangeHeader.GSQHeaderID = GSQHID;
                                                    objNOKChangeHeader.NOKStatus = NokStatus;
                                                    objNOKChangeHeader.Sno = ConSNo;
                                                    objNOKChangeHeader.Location = userInfo.LocationId;
                                                    objNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                                                    objNOKChangeHeader.WFDate = obj_GSQHeader.NOKWDate;
                                                    objNOKChangeHeader.Authority = obj_GSQHeader.Authority;
                                                    objNOKChangeHeader.RefNo = CreatePorNo;
                                                    objNOKChangeHeader.CreatedBy = UID_;
                                                    objNOKChangeHeader.CreatedDate = DateTime.Now;
                                                    objNOKChangeHeader.CreatedMac = MacAddress;
                                                    objNOKChangeHeader.Active = 1;
                                                    objNOKChangeHeader.CreateIpAddess = this.Request.UserHostAddress;

                                                    _db.NOKChangeHeaders.Add(objNOKChangeHeader);

                                                    if (_db.SaveChanges() > 0)
                                                    {
                                                        /// Enetr NOK header details to NOKChangeDetail
                                                        /// 
                                                        var NOKCHID = _db.NOKChangeHeaders.Where(x => x.Sno == ConSNo &&
                                                                      x.Location == userInfo.LocationId && x.NOKStatus == NokStatus && x.Active == 1 && x.GSQHeaderID == GSQHID).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();

                                                        objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                                        objNOKChangeDetail.NOKAddress = obj_GSQHeader.NOKaddress;
                                                        objNOKChangeDetail.NOKName = obj_GSQHeader.NOKName;
                                                        objNOKChangeDetail.NOKChangeTo = obj_GSQHeader.NOKRelationship1;
                                                        objNOKChangeDetail.Province = ProvinceName;
                                                        objNOKChangeDetail.District = DistName;
                                                        objNOKChangeDetail.GSDivision = obj_GSQHeader.GSName;
                                                        objNOKChangeDetail.NearestTown = obj_GSQHeader.Town1;
                                                        objNOKChangeDetail.PoliceStation = obj_GSQHeader.PoliceStation1;
                                                        objNOKChangeDetail.PostOffice = obj_GSQHeader.PostOfficeName;
                                                        objNOKChangeDetail.Remarks = obj_GSQHeader.Remarks;
                                                        objNOKChangeDetail.CreatedBy = UID_;
                                                        objNOKChangeDetail.CreatedDate = DateTime.Now;
                                                        objNOKChangeDetail.CreatedMac = MacAddress;
                                                        objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                                        objNOKChangeDetail.Active = 1;


                                                        _db.NOKChangeDetails.Add(objNOKChangeDetail);


                                                        if (_db.SaveChanges() > 0)
                                                        {
                                                            /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                                            InserFlowStatus(GSQHID, RoleId, UID_, obj_GSQHeader.FMSID, obj_GSQHeader.RSID);
                                                            TempData["ScfMsg"] = "Data Added Successfully";
                                                            scope.Complete();
                                                        }
                                                        else
                                                        {
                                                            scope.Dispose();
                                                            TempData["ErrMsg"] = "Process Unsuccessfull";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    TempData["ErrMsg"] = "Process Unsuccessfull";
                                                    scope.Dispose();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            TempData["ErrMsg"] = "Please Fill all the details.";
                                        }
                                    }
                                    else
                                    {
                                        TempData["ErrMsg"] = "This person does not have a Married Quater to Vacant.";
                                    }
                                    #endregion

                                    break;

                                default:
                                    break;
                            }
                        }
                        return View(obj_GSQHeader);
                    }
                    else
                    {
                        return RedirectToAction("CreateAllocateMQ");
                    }                   
                }
                catch (Exception ex)
                {
                    throw ex;
                }
               
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/03/15
            /// Description : Load data user roll wise

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();


                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P2CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert && x.Field<int>("Active") == 1).ToList();

                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert && x.Field<int>("Active") == 1).CopyToDataTable();
                        }
                        break;
                    #endregion

                    default:

                        #region CodeArea
                        ///Data Table first row CurrentStatus gave null value, hence it occures an error. Because of that check the column and delete that row, 
                        /// [44] means null coloumn number

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();

                        /// Check the data table has row or not
                        result = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && x.Field<int>("Active") == 1).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {                              
                                var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                                if (rows.Any())
                                {
                                    dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                             || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                             || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                }


                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                                    
                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                             (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") ==
                                             (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                                    }
                                    else
                                    {

                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer));

                                    if (rows.Any())
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
                                       
                                        var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                        (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                        if (rows.Any())
                                        {

                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                  (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                            
                                        }
                                    }
                                    else
                                    {
                                         var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                         (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                         x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                         if (rows.Any())
                                        {

                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                 (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                 x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
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
        public List<PORRecordCount> PorNoCreate(string EstablishmentId, int gsqStatus)
        {
            ///Created BY   : Flt Lt SMSL Samaratunge
            ///Created Date : 2021/05/31
            /// Description : create POR Number 

            try
            {
                List<PORRecordCount> ListRecordList = new List<PORRecordCount>();

                int currentmonth = Convert.ToInt32(DateTime.Now.Month);
                int currentyear = Convert.ToInt32(DateTime.Now.Year);
                int currentdate = Convert.ToInt32(DateTime.Now.Day);
                string CategoryCode = "";

                int jobcount = objDALCommanQueryP2.LivingInOutNextRecordId(EstablishmentId, currentyear);

                //int jobcount = _db.GSQHeaders.Where(x => x.Location == EstablishmentId && x.CreatedDate.Value.Year == currentyear && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer) ||
                //               (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer))).Max(x => x.RecordCount).Value;


                int RocordId = jobcount + 1;

                /// According to GSQ status Porno assign Category code chnage
                if (gsqStatus == (int)POR.Enum.GSQStatus.Allocate)
                {
                    CategoryCode = "OK-2";
                }
                else
                {
                    CategoryCode = "OK-3";
                }


                _GSQHeader obj_GSQHeader = new _GSQHeader();
                string RefNo = EstablishmentId + "/" + CategoryCode + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;

                PORRecordCount obj = new PORRecordCount();
                obj.RecordCount = RocordId;
                obj.RefNo = RefNo;
                ListRecordList.Add(obj);
                return ListRecordList;

                //obj_TechConComman.JobNo = NewJobNo;
                //return Json(objUserFaultRegistry, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Details(int GSQHID, int Rejectstatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Details related GSQ 

            int? UID = Convert.ToInt32(Session["UID"]);
            int UID_ = 0;           
            int? CurrentStatusUserRole;

            UID_ = Convert.ToInt32(Session["UID"]);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_GSQHeader> GSQHeaderList = new List<_GSQHeader>();

            var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();           
            TempData["UserRoleId"] = userInfo.RoleId;           

            if (Session["UID"].ToString() != null)
            {

                var CurrentStatus_UserRole = (from f in _db.FlowStatusGSQDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.GSQHeaderID == GSQHID
                                              orderby f.GSQHeaderID descending
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
                dt = objDALCommanQuery.CallGSQSP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("GSQHID") == GSQHID).CopyToDataTable();

                /// This Rejectstatus value assign from after clicking RejectIndex Details button. value 2 mean reject status
                if (Rejectstatus != 2)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        /// Check the rercord is previously reject or not
                        var prvReject = _db.GSQHeaders.Where(x => x.GSQHID == GSQHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        _GSQHeader obj_GSQHeader = new _GSQHeader();
                        obj_GSQHeader.GSQStatus = Convert.ToInt32(dt2.Rows[i]["GSQStatus"]);

                        if (obj_GSQHeader.GSQStatus == (int)POR.Enum.GSQStatus.Allocate)
                        {
                            obj_GSQHeader.AllocatedDate = Convert.ToDateTime(dt2.Rows[i]["AllocatedDate"].ToString());
                        }
                        else
                        {
                            obj_GSQHeader.VacantDate = Convert.ToDateTime(dt2.Rows[i]["VacantDate"].ToString());
                        }

                        obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                        obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_GSQHeader.Sno = Convert.ToInt64(dt2.Rows[i]["Sno"]);
                        obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                        obj_GSQHeader.Branch = dt2.Rows[i]["Branch"].ToString();
                        obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                        obj_GSQHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                        obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                        obj_GSQHeader.MQRecoveryType = dt2.Rows[i]["MQRecoveryTypeNew"].ToString();
                        obj_GSQHeader.SpouseHusbandWorking = dt2.Rows[i]["ISSpouseHusbandWrkSLAFNew"].ToString();
                        obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_GSQHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                        obj_GSQHeader.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_GSQHeader.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_GSQHeader.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_GSQHeader.NOKWDate = Convert.ToDateTime(dt2.Rows[i]["NOKWFDate"]);
                        obj_GSQHeader.ProvinceName = dt2.Rows[i]["Province"].ToString();
                        obj_GSQHeader.DistrictName = dt2.Rows[i]["District"].ToString();
                        obj_GSQHeader.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_GSQHeader.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_GSQHeader.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_GSQHeader.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        //obj_GSQHeader.Remarks = dt2.Rows[i]["Remarks"].ToString();

                        if (prvReject >= 1)
                        {
                            obj_GSQHeader.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_GSQHeader.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_GSQHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        GSQHeaderList.Add(obj_GSQHeader);
                    }
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id
                    var RejectRoleId = _db.FlowStatusGSQDetails.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.GSQHeaderID == GSQHID)
                                      .OrderByDescending(x => x.FSGSQID).Select(x => x.RoleID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _GSQHeader obj_GSQHeader = new _GSQHeader();

                        obj_GSQHeader.GSQStatus = Convert.ToInt32(dt2.Rows[i]["GSQStatus"]);
                        if (obj_GSQHeader.GSQStatus == (int)POR.Enum.GSQStatus.Allocate)
                        {
                            obj_GSQHeader.AllocatedDate = Convert.ToDateTime(dt2.Rows[i]["AllocatedDate"].ToString());
                        }
                        else
                        {
                            obj_GSQHeader.VacantDate = Convert.ToDateTime(dt2.Rows[i]["VacantDate"].ToString());
                        }

                        obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                        obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_GSQHeader.Sno = Convert.ToInt64(dt2.Rows[i]["Sno"]);
                        obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                        obj_GSQHeader.Branch = dt2.Rows[i]["Branch"].ToString();
                        obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                        obj_GSQHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                        obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                        obj_GSQHeader.MQRecoveryType = dt2.Rows[i]["MQRecoveryTypeNew"].ToString();
                        obj_GSQHeader.SpouseHusbandWorking = dt2.Rows[i]["ISSpouseHusbandWrkSLAFNew"].ToString();
                        obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_GSQHeader.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_GSQHeader.RejectRoleName = RoleName.ToString();
                        obj_GSQHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                        obj_GSQHeader.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_GSQHeader.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_GSQHeader.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_GSQHeader.ProvinceName = dt2.Rows[i]["Province"].ToString();
                        obj_GSQHeader.DistrictName = dt2.Rows[i]["District"].ToString();
                        obj_GSQHeader.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_GSQHeader.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_GSQHeader.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_GSQHeader.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        //obj_GSQHeader.Remarks = dt2.Rows[i]["Remarks"].ToString();

                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_GSQHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        GSQHeaderList.Add(obj_GSQHeader);
                    }
                }

                return View(GSQHeaderList);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }

        public ActionResult Delete(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            ///Description  : Delete the p3 Clerk entred data. All Active 1 record turn into 0

            GSQHeader objGSQHeader = new GSQHeader();
            int UID_ = Convert.ToInt32(Session["UID"]);

            try
            {
                //// CivilStatusHeader active colum 1 turn into 0
                objGSQHeader = _db.GSQHeaders.Find(id);
                objGSQHeader.Active = 0;
                objGSQHeader.ModifiedDate = DateTime.Now;
                objGSQHeader.ModifiedBy = UID_;
                objGSQHeader.ModifiedMac = MacAddress;

                _db.Entry(objGSQHeader).State = EntityState.Modified;

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

                return RedirectToAction("Index", "P2GSQHeader");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id, int rejectStatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Edit the GSQ Hedails

            int? UID = Convert.ToInt32(Session["UID"]);
            int UID_ = 0;            
            int? CurrentStatusUserRole;
            int GSQHID = id;
            string RejectRef;

            UID_ = Convert.ToInt32(Session["UID"]);
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            _GSQHeader obj_GSQHeader = new _GSQHeader();

            var userInfo  = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();            
            TempData["UserRoleId"] = userInfo.RoleId;

            ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");
            //ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
            ViewBag.DDL_Location = new SelectList(_dbCommonData.Establishments, "LocationID", "LocationName");
            ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus, "GSQSID", "StatusName");
            ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
            ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
            ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
            ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");




            if (Session["UID"].ToString() != null)
            {

                var CurrentStatus_UserRole = (from f in _db.FlowStatusGSQDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.GSQHeaderID == GSQHID
                                              orderby f.GSQHeaderID descending
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
                dt = objDALCommanQuery.CallGSQSP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("GSQHID") == id).CopyToDataTable();

                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    obj_GSQHeader.GSQStatus = Convert.ToInt32(dt2.Rows[i]["GSQStatus"]);

                    if (obj_GSQHeader.GSQStatus == (int)POR.Enum.GSQStatus.Allocate)
                    {
                        obj_GSQHeader.AllocatedDate = Convert.ToDateTime(dt2.Rows[i]["AllocatedDate"].ToString());
                    }
                    else
                    {
                        obj_GSQHeader.VacantDate = Convert.ToDateTime(dt2.Rows[i]["VacantDate"].ToString());
                    }

                    TempData["GSQStatus"] = dt2.Rows[i]["GSQStatus"].ToString(); ;

                    obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                    obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                    obj_GSQHeader.Branch = dt2.Rows[i]["Branch"].ToString();
                    obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                    obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                    obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                    obj_GSQHeader.MQRecoveryType = dt2.Rows[i]["MQRecoveryTypeNew"].ToString();
                    obj_GSQHeader.SpouseHusbandWorking = dt2.Rows[i]["ISSpouseHusbandWrkSLAFNew"].ToString();
                    obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_GSQHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                    obj_GSQHeader.NOKName = dt2.Rows[i]["NOKName"].ToString();
                    obj_GSQHeader.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                    obj_GSQHeader.NOKWDate = Convert.ToDateTime(dt2.Rows[i]["NOKWFDate"]);
                    obj_GSQHeader.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                    obj_GSQHeader.EditedProvince = dt2.Rows[i]["Province"].ToString();
                    obj_GSQHeader.EditedDistrict1 = dt2.Rows[i]["District"].ToString();
                    obj_GSQHeader.EditedGSnumber = dt2.Rows[i]["GSDivision"].ToString();
                    obj_GSQHeader.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                    obj_GSQHeader.EditPoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                    obj_GSQHeader.EditPostOfficeName = dt2.Rows[i]["PostOffice"].ToString();

                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.GSQHeaders.Where(x => x.GSQHID == GSQHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_GSQHeader.RefNo + " " + " - Reject";
                            obj_GSQHeader.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_GSQHeader.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_GSQHeader.RejectRefNo = RejectRef;
                        }

                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_GSQHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }

                }
                return View(obj_GSQHeader);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult Edit(_GSQHeader obj_GSQHeader, int rejectStatus, string WorkingSLAF, string MQRecovery)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Edit the GSQ Hedails

            GSQHeader objGSQHeader = new GSQHeader();
            FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();

            int UID_ = 0;
            int RoleId = 0;
            //bool status;
            /// Initial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            int RecordStatus = (int)POR.Enum.RecordStatus.Insert;

            try
            {

                ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");
                //ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_Location = new SelectList(_dbCommonData.Establishments, "LocationID", "LocationName");
                ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus, "GSQSID", "StatusName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");

                if (Session["UID"] != null)
                {
                    #region CodeArea
                    //objGSQHeader.GSQStatus = obj_GSQHeader.StatusName;

                    string ConPROV_CODE = obj_GSQHeader.ProvinceId.ToString();
                    var ProvinceName = _dbCommonData.ProvinceNews.Where(x => x.PROV_CODE == ConPROV_CODE).Select(x => x.DESCRIPTION).FirstOrDefault();

                    ///Get District name
                    var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_GSQHeader.District).Select(x => x.DESCRIPTION).FirstOrDefault();

                    UID_ = Convert.ToInt32(Session["UID"]);
                    objGSQHeader = _db.GSQHeaders.Find(obj_GSQHeader.GSQHID);
                    objGSQHeader.GSQNo = obj_GSQHeader.GSQNo;


                    //// Write if condition in one line
                    if (WorkingSLAF != null) objGSQHeader.ISSpouseHusbandWrkSLAF = Convert.ToInt16(WorkingSLAF);
                    if (MQRecovery != null) objGSQHeader.MQRecoveryType = Convert.ToInt16(MQRecovery);

                    if (obj_GSQHeader.GSQStatus == (int)POR.Enum.GSQStatus.Allocate)
                    {
                        objGSQHeader.AllocatedDate = obj_GSQHeader.AllocatedDate;
                    }
                    else
                    {
                        objGSQHeader.VacantDate = obj_GSQHeader.VacantDate;
                    }

                    objGSQHeader.ModifiedBy = UID_;
                    objGSQHeader.ModifiedDate = DateTime.Now;
                    objGSQHeader.ModifiedMac = MacAddress;

                    _db.Entry(objGSQHeader).State = EntityState.Modified;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (_db.SaveChanges() > 0)
                        {
                            ////Here check the reject record has been edited
                            if (rejectStatus == 2)
                            {
                                ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                RoleId = (int)POR.Enum.UserRole.P2CLERK;
                                InserFlowStatus(obj_GSQHeader.GSQHID, RoleId, UID_, obj_GSQHeader.FMSID, RecordStatus);

                                /// Update GSQheader to table
                                /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                var rejectCount = _db.GSQHeaders.Where(x => x.GSQHID == obj_GSQHeader.GSQHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                objGSQHeader = _db.GSQHeaders.Find(obj_GSQHeader.GSQHID);

                                if (rejectCount == null)
                                {
                                    objGSQHeader.PreviousReject = 1;
                                }
                                else
                                {
                                    int refIncrement = Convert.ToInt32(rejectCount + 1);
                                    objGSQHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                }

                                objGSQHeader.RejectAuth = obj_GSQHeader.RejectRefNo;
                                _db.Entry(objGSQHeader).State = EntityState.Modified;

                                /// previous reject record active 1 status turn in to 0
                                var RejFSGSQID = _db.FlowStatusGSQDetails.Where(x => x.GSQHeaderID == obj_GSQHeader.GSQHID && x.RecordStatusID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.FSGSQID).FirstOrDefault();

                                objFlowStatusGSQDetail = _db.FlowStatusGSQDetails.Find(RejFSGSQID);
                                objFlowStatusGSQDetail.Active = 0;
                                objFlowStatusGSQDetail.ModifiedBy = UID_;
                                objFlowStatusGSQDetail.ModifiedDate = DateTime.Now;
                                objFlowStatusGSQDetail.ModifiedMac = MacAddress;
                                _db.Entry(objFlowStatusGSQDetail).State = EntityState.Modified;


                            }

                            ///// NOK Update coding section   
                            /// Update Nok header table
                            var NOKHID = _db.NOKChangeHeaders.Where(x => x.GSQHeaderID == obj_GSQHeader.GSQHID && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();
                            objNOKChangeHeader = _db.NOKChangeHeaders.Find(NOKHID);
                            objNOKChangeHeader.WFDate = obj_GSQHeader.NOKWDate;
                            objNOKChangeHeader.ModifiedBy = UID_;
                            objNOKChangeHeader.ModifiedDate = DateTime.Now;
                            objNOKChangeHeader.ModifiedMac = MacAddress;
                            _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                            /// Update Nok Detail table
                            var NOKDID = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == NOKHID && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();

                            objNOKChangeDetail = _db.NOKChangeDetails.Find(NOKDID);
                            objNOKChangeDetail.NOKName = obj_GSQHeader.NOKName;
                            if (obj_GSQHeader.NOKRelationship1 != null)
                            {
                                objNOKChangeDetail.NOKChangeTo = obj_GSQHeader.NOKRelationship1;
                            }
                            objNOKChangeDetail.NOKAddress = obj_GSQHeader.NOKaddress;

                            //// meka kale different type of object assign value

                            if (obj_GSQHeader.ProvinceId != 0)
                            {
                                objNOKChangeDetail.Province = ProvinceName;

                                if (obj_GSQHeader.District != 0)
                                {
                                    objNOKChangeDetail.District = DistName;
                                }
                                if (obj_GSQHeader.GSName != null)
                                {
                                    objNOKChangeDetail.GSDivision = obj_GSQHeader.GSName;
                                }
                                if (obj_GSQHeader.Town1 != "SELECT")
                                {
                                    objNOKChangeDetail.NearestTown = obj_GSQHeader.Town1;
                                }
                                if (obj_GSQHeader.PoliceStation1 != null)
                                {
                                    objNOKChangeDetail.PoliceStation = obj_GSQHeader.PoliceStation1;
                                }
                                if (obj_GSQHeader.PostOfficeName != "SELECT")
                                {
                                    objNOKChangeDetail.PostOffice = obj_GSQHeader.PostOfficeName;
                                }

                            }
                            
                            _db.Entry(objNOKChangeDetail).State = EntityState.Modified;

                            if (_db.SaveChanges() > 0)
                            {
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
                            scope.Dispose();
                            TempData["ErrMsg"] = "Not Complete Your Process";
                        }
                    }
                    return View(obj_GSQHeader);

                    #endregion                   

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
        }

        public ActionResult PrintData(int GSQHID)
        {
            ///Create By: Flt Lt WAKY Wickramasinghe
            ///Create Date: 2023/03/13 
            ///Description: Provide Facility to print details of the GSQ data

            Session["GSQHID"] = GSQHID;

            return View();

        }

        public bool ckeckRecordStatus(int StatusName, string ServiceNo)
        {
            /// Created Date : 2023/03/15
            /// Created By : Flt Lt WAKY Wickramasinghe
            /// Des : after clerk edit the reject record with GSQ status, this function check the duplication of that record
            //// check the duplication for gsq entering data

            bool status = true;
            var SNO = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == ServiceNo).Select(x => x.SNo).FirstOrDefault();
            long ConSNo = Convert.ToInt64(SNO);

            int geqStatus = StatusName;

            switch (geqStatus)
            {
                case (int)POR.Enum.GSQStatus.Allocate:

                    #region CodeSegment

                    var checkGsqVacantStatus = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Vacant && x.Active == 1).Count();

                    if (checkGsqVacantStatus != 0)
                    {
                        var checkAllocateDup = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();

                        if (checkAllocateDup == 1)
                        {

                            //TempData["ErrMsg"] = " You can not enter the GSQ Allocation. You have allocate a MQ already.";
                            status = false;
                        }
                        else
                        {
                            status = true; /// he allowed to get MQ
                        }
                    }
                    else
                    {
                        #region Code Region

                        var checkAllDupl = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();

                        if (checkAllDupl == 0)
                        {
                            status = true;  /// he allowed to get MQ
                        }
                        else
                        {
                            status = false;
                            //TempData["ErrMsg"] = "This person already has a MQ.";
                        }

                        #endregion
                    }

                    #endregion

                    break;
                case (int)POR.Enum.GSQStatus.Vacant:

                    #region CodeSegment

                    GSQHeader obj = new GSQHeader();
                    /// 1st check if it has allocation active 1 record
                    var checkAllocateDuplicate = _db.GSQHeaders.Where(x => x.Sno == ConSNo && x.GSQStatus == (int)POR.Enum.GSQStatus.Allocate && x.Active == 1).Count();

                    if (checkAllocateDuplicate == 1)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                        //TempData["ErrMsg"] = "This person does not have a Married Quater to Vacant.";
                    }
                    #endregion

                    break;
            }
            return status;
        }      

        [HttpGet]
        public ActionResult Forward(int? id, string Sno, int? GSQHID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Rocord Singal Forwarding for next authority level  


            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            string SubmitStatus_UserRole;
            bool updateStatus = false;
            //int? NextFlowStatusId;

            if (UID != 0)
            {
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
                string RecordEstablishmentId = _db.GSQHeaders.Where(x => x.GSQHID == id).Select(x => x.Location).FirstOrDefault();
                long SNO = Convert.ToInt64(Sno);

                int? SubmitStatus = NextFlowStatusId(id, userInfo.LocationId, RecordEstablishmentId, userInfo.DivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg
                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();

                objFlowStatusGSQDetail.GSQHeaderID = id;
                objFlowStatusGSQDetail.RecordStatusID = 2000;
                objFlowStatusGSQDetail.UserID = UID;
                objFlowStatusGSQDetail.FlowManagementStatusID = SubmitStatus;
                objFlowStatusGSQDetail.RoleID = UserRoleId;
                objFlowStatusGSQDetail.CreatedBy = UID;
                objFlowStatusGSQDetail.CreatedDate = DateTime.Now;
                objFlowStatusGSQDetail.CreatedMac = MacAddress;
                objFlowStatusGSQDetail.IPAddress = this.Request.UserHostAddress;
                objFlowStatusGSQDetail.Active = 1;



                ///This function is to update the Hrmis data base. After account one certified, the details will update p3hrmis 
                if (userInfo.RoleId == (int)POR.Enum.UserRole.ACCOUNTS01)
                {
                    /// Insert new record to P2hrms Nokchange details table
                    updateStatus = UpdateHrmis(MacAddress, UID, SNO, id);

                    if (updateStatus == true)
                    {
                        _db.FlowStatusGSQDetails.Add(objFlowStatusGSQDetail);
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (_db.SaveChanges() > 0)
                            {
                                TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                                scope.Complete();
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                scope.Dispose();
                                TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Something error in HRMS Record. Please Contact ITW";
                    }
                }
                else
                {
                    _db.FlowStatusGSQDetails.Add(objFlowStatusGSQDetail);
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (_db.SaveChanges() > 0)
                        {
                            TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                            scope.Complete();
                            return RedirectToAction("Index");

                        }
                        else
                        {
                            scope.Dispose();
                            TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                            return RedirectToAction("Index");
                        }
                    }

                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        [HttpPost]
        public ActionResult Forward(int[] id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Rocord Bulk Forwarding for next authority level  
            
            string SubmitStatus_UserRole = null;
            string UserEstablishmentId = null;
            string RecordEstablishmentId = null;
            int? UserRoleId = 0;
            int? UID = 0;

            if (Session["UID"].ToString() != null)
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            foreach (int IDs in id)
            {
                if (UID != 0)
                {

                    UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                    RecordEstablishmentId = _db.GSQHeaders.Where(x => x.GSQHID == IDs).Select(x => x.Location).FirstOrDefault();
                    string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();


                    int? SubmitStatus = NextFlowStatusId(IDs, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);

                    //Get Next FlowStatus User Role Name for Add Successfull Msg

                    UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                    SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();



                    //Insert data to Flowstatusdetails table ow forward with RSID =2000

                    FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();
                    FlowStatusCivilStatusDetail objFlowStatusCivilStatusDetail = new FlowStatusCivilStatusDetail();

                    objFlowStatusGSQDetail.GSQHeaderID = IDs;
                    objFlowStatusGSQDetail.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                    objFlowStatusGSQDetail.UserID = UID;
                    objFlowStatusGSQDetail.FlowManagementStatusID = SubmitStatus;
                    objFlowStatusGSQDetail.RoleID = UserRoleId;
                    objFlowStatusGSQDetail.CreatedDate = DateTime.Now;
                    objFlowStatusGSQDetail.CreatedMac = MacAddress;
                    objFlowStatusGSQDetail.IPAddress = this.Request.UserHostAddress;
                    objFlowStatusGSQDetail.Active = 1;

                    _db.FlowStatusGSQDetails.Add(objFlowStatusGSQDetail);
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

        public int? NextFlowStatusId(int? GSQHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Get the record next submitting status

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxFSCSID = _db.FlowStatusGSQDetails.Where(x => x.GSQHeaderID == GSQHID && x.Active == 1).Select(x => x.FSGSQID).Max();
                int? CurrentFMSID = _db.FlowStatusGSQDetails.Where(x => x.FSGSQID == MaxFSCSID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? SubmitStatus = 0;

                int? UID = Convert.ToInt32(Session["UID"]);

                //LHID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();
                    SubmitStatus = _db.FlowManagementStatus.Where(x => x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.UserRoleID == RoleId).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                }
                else
                {
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P2OIC || CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO)
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
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OK" && x.Active == 1).Select(x => x.FlowGroupP2).FirstOrDefault();
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
                            //FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId || x.DivisionId3 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus).Select(x => x.FMSID).FirstOrDefault();
                        }
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
        public ActionResult RejectIndex(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/05/23
            /// Description : Show the reject list user wise in GSQ

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            DataTable dt, dt2, dt3 = new DataTable();
            List<_GSQHeader> GSQHeaderList = new List<_GSQHeader>();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
                //ViewBag.DateSortParm = sortOrder == "LeaveCategoryName" ? "Rank" : "FromDate";

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallGSQRejectSP();


                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                var FMSID = _db.FlowManagementStatus.Where(x => (x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId) && (x.EstablishmentId == LocationId && x.UserRoleID == UserInfo.RoleId)).Select(x => x.FMSID).FirstOrDefault();

                TempData["RoleId"] = UserInfo.RoleId;

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject && 
                           (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer 
                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                    if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P2SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P2OIC)
                    {
                        dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();

                        for (int i = 0; i < dt3.Rows.Count; i++)
                        {
                            _GSQHeader obj_GSQHeader = new _GSQHeader();
                            obj_GSQHeader.GSQHID = Convert.ToInt32(dt3.Rows[i]["GSQHID"]);
                            obj_GSQHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                            obj_GSQHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                            obj_GSQHeader.Name = dt3.Rows[i]["Name"].ToString();
                            obj_GSQHeader.Location = dt3.Rows[i]["Location"].ToString();
                            obj_GSQHeader.GSQStatusName = dt3.Rows[i]["StatusName"].ToString();
                            obj_GSQHeader.GSQNo = dt3.Rows[i]["GSQNo"].ToString();
                            obj_GSQHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                            GSQHeaderList.Add(obj_GSQHeader);

                        }

                    }
                    else
                    {
                        for (int i = 0; i < dt2.Rows.Count; i++)
                        {
                            _GSQHeader obj_GSQHeader = new _GSQHeader();
                            obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                            obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                            obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                            obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                            obj_GSQHeader.Location = dt2.Rows[i]["Location"].ToString();
                            obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                            obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                            obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                            GSQHeaderList.Add(obj_GSQHeader);

                        }
                    }
                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(GSQHeaderList.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK)
                    {
                        return RedirectToAction("P2ClkHome", "Home");

                    }
                    else
                    {
                        return RedirectToAction("P2ClkHome", "Home");
                    }

                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }


            // return View(_db.Vw_Leave.ToList());
        }

        [HttpPost]
        public ActionResult Index_Reject(string id, int GSQHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/03/15
            /// Description : this function is to reject the record

            string message = "";
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId ,x.DivisionId }).FirstOrDefault();                
                string RecordEstablishmentId = _db.GSQHeaders.Where(x => x.GSQHID == GSQHID).Select(x => x.Location).FirstOrDefault();
               
                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(GSQHID, userInfo.LocationId, RecordEstablishmentId, userInfo.DivisionId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == userInfo.LocationId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
                FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();
                objFlowStatusGSQDetail.GSQHeaderID = GSQHID;
                objFlowStatusGSQDetail.FlowManagementStatusID = PreviousFMSID;
                objFlowStatusGSQDetail.UserID = UID;
                objFlowStatusGSQDetail.CreatedBy = UID;
                objFlowStatusGSQDetail.RoleID = UserRoleId;

                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//

                objFlowStatusGSQDetail.RecordStatusID = (int)POR.Enum.RecordStatus.Reject;
                objFlowStatusGSQDetail.Comment = id;
                objFlowStatusGSQDetail.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objFlowStatusGSQDetail.CreatedMac = MacAddress;
                objFlowStatusGSQDetail.Active = 1;
                _db.FlowStatusGSQDetails.Add(objFlowStatusGSQDetail);

                if (_db.SaveChanges() > 0)
                {
                    if (UserRoleId >= 5)
                    {
                        message = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + RecordEstablishmentId;
                    }
                    else
                    {
                        message = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + RecordEstablishmentId;
                    }

                    //return RedirectToAction("RejectIndex");
                }
                else
                {
                    message = "Process Unsuccessful.Try again...";
                }

            }
            else
            {
                // return RedirectToAction("Login", "User");
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        public int? PreviousFlowStatusId(int? GSQHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created BY   : Flt Lt SMSL Samaratunge
            ///Created Date : 2023/03/15
            /// Description : Find the new flowstutus to whom it will forward 

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFADFID = _db.FlowStatusGSQDetails.Where(x => x.GSQHeaderID == GSQHID).Select(x => x.FSGSQID).Max();
                int? CurrentFMSID = _db.FlowStatusGSQDetails.Where(x => x.FSGSQID == MaxFADFID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowStatusGSQDetails.Where(x => x.FSGSQID == MaxFADFID && x.FlowManagementStatusID == CurrentFMSID).Select(x => x.RoleID).FirstOrDefault();
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);


                //FADID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId && x.UserRoleID == (int)POR.Enum.UserRole.P2CLERK).Select(x => x.FMSID).First();
                }
                else
                {
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P2OIC || CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO)
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        public ActionResult ConfirmReject(int? id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/03/15
            /// Description : Reject confirmation from P2clk
                
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }

            //int? PreviousFlowStatusId;

            if (UID != 0)
            {
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.GSQHeaders.Where(x => x.GSQHID == id).Select(x => x.Location).FirstOrDefault();
                
                //After new record inserting previous stuts become Active 1 to active 0

                var FSGSQID = _db.FlowStatusGSQDetails.Where(x => x.GSQHeaderID == id && x.Active == 1).Select(x => x.FSGSQID).FirstOrDefault();

                if (FSGSQID != 0)
                {
                    FlowStatusGSQDetail objFlowStatusDetails = _db.FlowStatusGSQDetails.Find(FSGSQID);
                    objFlowStatusDetails.RecordStatusID = (int)POR.Enum.RecordStatus.Delete;
                    _db.Entry(objFlowStatusDetails).State = EntityState.Modified;

                    _db.SaveChanges();

                    TempData["ScfMsg"] = "Data Successfully Confermed Reject";
                    return RedirectToAction("IndexRejectedGSQAllocation");
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

        public ActionResult Reject(int GSQHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/03/15
            /// Description : this function is to reject the record

            _GSQHeader model = new _GSQHeader();
            try
            {
                model.GSQHID = GSQHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_P2RejectCommentGSQ", model);
        }

        [HttpGet]
        public ActionResult RejectConfirm(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/03/15
            /// Description : P2 Clerk finally confirm the reject Confirm. Afetr confirm record Status came to 0

            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                //Update CivilStatusHeader Active colum to 0
                GSQHeader objGSQHeader = _db.GSQHeaders.Find(id);

                objGSQHeader.Active = 0;
                objGSQHeader.ModifiedBy = UID_;
                objGSQHeader.ModifiedDate = DateTime.Now;
                objGSQHeader.ModifiedMac = MacAddress;
                _db.Entry(objGSQHeader).Property(x => x.Active).IsModified = true;

                if (_db.SaveChanges() > 0)
                {
                    TempData["ScfMsg"] = "Successfully Reject Confirmed.";
                }
                return RedirectToAction("RejectIndex");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public void InserFlowStatus(int GSQHID, int RoleId, int UID_, int? FMSID, int? RSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/08/16
            /// Description : Insert GSQ first record to FlowStatusGSQDetail table

            try
            {
                FlowStatusGSQDetail objFlowStatusGSQDetail = new FlowStatusGSQDetail();
                FlowStatusLivingInOutDetail objFlowStatusDetail = new FlowStatusLivingInOutDetail();
                // LivingInOutDetail objLivingInOutDetail = new LivingInOutDetail();


                objFlowStatusGSQDetail.GSQHeaderID = GSQHID;
                objFlowStatusGSQDetail.RecordStatusID = RSID;
                objFlowStatusGSQDetail.UserID = UID_;
                objFlowStatusGSQDetail.FlowManagementStatusID = FMSID;
                objFlowStatusGSQDetail.RoleID = RoleId;
                objFlowStatusGSQDetail.CreatedBy = UID_;
                objFlowStatusGSQDetail.CreatedMac = MacAddress;
                objFlowStatusGSQDetail.IPAddress = this.Request.UserHostAddress;
                objFlowStatusGSQDetail.CreatedDate = DateTime.Now;
                objFlowStatusGSQDetail.Active = 1;

                _db.FlowStatusGSQDetails.Add(objFlowStatusGSQDetail);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult AdvancedSearch(string FromDate, string ToDate, string SearchLoc, string SearchGSQStatus, string RecordStatus, int? page, string currentFilterFDate, string currentFilterTDate, string currentFilterLocation, string currentFilterCategory, string currentFilterRecStatus)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023/05/15
            ///Des: Searching Option for history details

            DataTable dt = new DataTable();
            List<_GSQHeader> GSQHeaderList = new List<_GSQHeader>();

            int GSQHID;
            int recordType;

            if (RecordStatus == null)
            {

                ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus.Where(x => x.Active == 1), "GSQSID", "StatusName");

                TempData["ErrMsg"] = "Please selecct the Record Status.";
            }
            else
            {
                recordType = Convert.ToInt32(RecordStatus);

                if (SearchGSQStatus == "")
                {
                    GSQHID = 0;
                }
                else
                {
                    GSQHID = Convert.ToInt32(SearchGSQStatus);
                }

                if (SearchGSQStatus != null)
                {
                    page = 1;
                    ViewBag.currentFilterFDate = FromDate;
                    ViewBag.currentFilterTDate = ToDate;
                    ViewBag.currentFilterLocation = SearchLoc;
                    ViewBag.currentFilterCategory = SearchGSQStatus;
                    ViewBag.currentFilterRecStatus = recordType;
                }
                else
                {
                    ViewBag.currentFilterFDate = currentFilterFDate;
                    ViewBag.currentFilterTDate = currentFilterTDate;
                    ViewBag.currentFilterLocation = currentFilterLocation;
                    ViewBag.currentFilterCategory = currentFilterCategory;
                    ViewBag.currentFilterRecStatus = currentFilterRecStatus;
                }


                try
                {
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                    ViewBag.DDL_GSQStatus = new SelectList(_db.GSQStatus.Where(x => x.Active == 1), "GSQSID", "StatusName");


                    if (page != 1)
                    {
                        ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                        if (currentFilterFDate != null && currentFilterTDate != null && currentFilterLocation != null && currentFilterCategory != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            GSQHID = Convert.ToInt32(currentFilterCategory);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                        }
                        else if (currentFilterFDate != null && currentFilterTDate != null && currentFilterCategory != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            GSQHID = Convert.ToInt32(currentFilterCategory);
                            SearchLoc = "";
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                        }
                        else if (currentFilterFDate != null && currentFilterLocation != null && currentFilterCategory != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = "";
                            SearchLoc = currentFilterLocation;
                            GSQHID = Convert.ToInt32(currentFilterCategory);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                        }
                        else if (currentFilterTDate != null && currentFilterLocation != null && currentFilterCategory != null && currentFilterRecStatus != null)
                        {
                            FromDate = "";
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            // CSCID = Convert.ToInt32(currentFilterCategory);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        if (FromDate == null && ToDate == null && SearchLoc == null && SearchGSQStatus == null && recordType == 0)
                        {
                            TempData["ErrMsg"] = "Please selecct any search criteria.";
                        }
                        else
                        {
                            ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                            //ETWCID = Convert.ToInt32(SearchWorkshopSection);
                            if (FromDate != "" && ToDate != "" && SearchLoc != "" && GSQHID != 0 && recordType != 0)
                            {
                                dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                            }
                            else if (FromDate != "" && ToDate != "" && GSQHID != 0 && recordType != 0)
                            {
                                SearchLoc = "";
                                dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                            }
                            else if (FromDate != "" && SearchLoc != "" && GSQHID != 0 && recordType != 0)
                            {
                                ToDate = "";
                                dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                            }
                            else if (ToDate != "" && SearchLoc != "" && GSQHID != 0 && recordType != 0)
                            {
                                FromDate = "";
                                dt = objDALCommanQuery.getGSQStatusSearchDetails(FromDate, ToDate, SearchLoc, GSQHID, recordType);
                            }
                            else
                            {
                                TempData["ErrMsg"] = "Please selecct any Three search criteria to search the details.";
                            }
                        }

                    }

                    int recordCount = 0;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        _GSQHeader obj_GSQHeader = new _GSQHeader();
                        obj_GSQHeader.GSQHID = Convert.ToInt32(dt.Rows[i]["GSQHID"].ToString());
                        obj_GSQHeader.ServiceNo = dt.Rows[i]["ServiceNo"].ToString();
                        obj_GSQHeader.Rank = dt.Rows[i]["Rank"].ToString();
                        obj_GSQHeader.Name = dt.Rows[i]["Name"].ToString();
                        obj_GSQHeader.GSQLocation = dt.Rows[i]["GSQLocation"].ToString();
                        obj_GSQHeader.RefNo = dt.Rows[i]["RefNo"].ToString();
                        obj_GSQHeader.GSQStatusName = dt.Rows[i]["StatusName"].ToString();
                        obj_GSQHeader.RoleName = dt.Rows[i]["UserRoleName"].ToString();
                        GSQHeaderList.Add(obj_GSQHeader);

                        recordCount++;
                    }
                    TempData["recordCount"] = recordCount;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                int pageSize = 20;
                int pageNumber = (page ?? 1);
                return View(GSQHeaderList.ToPagedList(pageNumber, pageSize));
            }
            return View();

        }

        public ActionResult ClearFileds(_GSQHeader obj_GSQHeader)
        {
            /// Clear view text box fields.

            ModelState.Clear();
            return RedirectToAction("CreateAllocateMQ", "P2GSQHeader");
        }

        [HttpGet]
        public ActionResult IndividualSearchGSQ(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe        
            ///Created Date : 2023/03/15
            /// Description : Search details for Individual Search

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            long sno = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_GSQHeader> GSQList = new List<_GSQHeader>();


            ViewBag.CurrentSort = sortOrder;

            if (searchString != null)
            {
                //// This write to Search details

                page = 1;

                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                sno = Convert.ToInt64(Sno);

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                TempData["UserRole"] = UserInfo.RoleId;

                ViewBag.CurrentFilter = searchString;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallGSQSP(sno);
                //dt = objDALCommanQuery.CallGSQSP();

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _GSQHeader obj_GSQHeader = new _GSQHeader();

                        obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                        obj_GSQHeader.FSGSQID = Convert.ToInt32(dt2.Rows[i]["FSGSQID"]);
                        obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                        obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                        obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                        obj_GSQHeader.RoleName = dt2.Rows[i]["UserRoleName"].ToString();
                        GSQList.Add(obj_GSQHeader);

                    }

                }
            }
            else
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                TempData["UserRole"] = UserInfo.RoleId;
            }
            pageSize = 20;
            pageNumber = (page ?? 1);
            return View(GSQList.ToPagedList(pageNumber, pageSize));


            // return View(_db.Vw_Leave.ToList());
        }
        public ActionResult IndividualGSQDetails(int id)
        {
            ///Created BY   : Flt Lt WAKY Wicramasinghe 
            ///Created Date : 2023/05/15
            /// Description : click Details button  after view details by select person 
            /// 

            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_GSQHeader> GSQHeaderList = new List<_GSQHeader>();
            //int? CurrentStatusUserRole;           

            if (Session["UID"].ToString() != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;


                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallGSQSP(0);


                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("FSGSQID") == id).CopyToDataTable();

                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    _GSQHeader obj_GSQHeader = new _GSQHeader();

                    obj_GSQHeader.GSQHID = Convert.ToInt32(dt2.Rows[i]["GSQHID"]);
                    obj_GSQHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_GSQHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_GSQHeader.Name = dt2.Rows[i]["Name"].ToString();
                    obj_GSQHeader.Branch = dt2.Rows[i]["Branch"].ToString();
                    obj_GSQHeader.GSQLocation = dt2.Rows[i]["GSQLocation"].ToString();
                    obj_GSQHeader.GSQNo = dt2.Rows[i]["GSQNo"].ToString();
                    obj_GSQHeader.GSQStatusName = dt2.Rows[i]["StatusName"].ToString();
                    obj_GSQHeader.AllocatedDate = Convert.ToDateTime(dt2.Rows[i]["AllocatedDate"].ToString());
                    obj_GSQHeader.VacantDate = Convert.ToDateTime(dt2.Rows[i]["VacantDate"].ToString());
                    obj_GSQHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_GSQHeader.Comment = dt2.Rows[i]["RejectComment"].ToString();
                    //obj_GSQHeader.RejectRoleName = RoleName.ToString();
                    obj_GSQHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                    if (Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]) == (int)POR.Enum.RecordStatus.Reject)
                    {
                        TempData["RecordStatusID"] = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_GSQHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }


                    GSQHeaderList.Add(obj_GSQHeader);
                }

                return View(GSQHeaderList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public bool UpdateHrmis(string MacAddress, int? UID, long? Sno, int? gsqHeaderId)
        {

            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/03/15
            /// Description : update the hrmis

            DataTable dt = new DataTable();
            DataTable civildt = new DataTable();
            BALNOK_Change_Details objNOK_Change_Details = new BALNOK_Change_Details();
            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.gsqAllocateVacanr;
            string Relationship = "";
            string districtName = "";
            string ProvinceName = "";
            string GsName = "";
            string nearestTown = "";
            string policeStation = "";
            string postOffice = "";
            bool status = false;
            

            string SSNo = Convert.ToString(Sno);
            try
            {
                // get the Next NOK Primary key ID
                int key = objDALCommanQueryP2.GenerateKey();
                string NokID = Convert.ToString("NOKID/" + key);

                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(gsqHeaderId, detailsCollectCategory);

               
                /// Update the perivous NoK type into 2                
                //status = objDALCommanQuery.UpdatePreviousNokTypeId(Sno, UID, MacAddress);

                foreach (DataRow row in dt.Rows)
                {
                    Relationship = row["NOKChangeTo"].ToString();
                    districtName = row["District"].ToString();
                    ProvinceName = row["Province"].ToString();
                    GsName = row["GSDivision"].ToString();
                    nearestTown = row["NearestTown"].ToString();
                    policeStation = row["PoliceStation"].ToString();
                    postOffice = row["PostOffice"].ToString();
                    objNOK_Change_Details.SNo = Sno.ToString();
                    objNOK_Change_Details.NOKName = row["NOKName"].ToString();
                    objNOK_Change_Details.NOKAddress = row["NOKAddress"].ToString();
                    objNOK_Change_Details.AuthRefNo = row["Authority"].ToString();
                    objNOK_Change_Details.WEFDate = Convert.ToDateTime(row["WFDate"]);
                    objNOK_Change_Details.NOKID = NokID;
                    objNOK_Change_Details.CreatedDate = DateTime.Now;
                    objNOK_Change_Details.CreatedUser = Convert.ToString(UID);
                    objNOK_Change_Details.CreatedMachine = MacAddress;
                    objNOK_Change_Details.PORRefNo = row["RefNo"].ToString();

                }

                var districtId = _dbCommonData.Districts.Where(x => x.DESCRIPTION == districtName).Select(x => x.DIST_CODE).FirstOrDefault();
                var provinceId = _dbCommonData.ProvinceNews.Where(x => x.DESCRIPTION == ProvinceName).Select(x => x.PROV_CODE).FirstOrDefault();
                var GSDivisionID = _dbCommonData.P2_GSDivision.Where(x => x.GSName == GsName).Select(x => x.GSCode).FirstOrDefault();
                var NearestTownID = _dbCommonData.P2_Town.Where(x => x.Town == nearestTown).Select(x => x.TownCOde).FirstOrDefault();
                var PoliceStationId = _dbCommonData.P2_PoliceStation.Where(x => x.PoliceStation == policeStation).Select(x => x.PoliceStationCode).FirstOrDefault();
                var PostOfficeId = _dbCommonData.P2_PostCode.Where(x => x.POST_NAME == postOffice).Select(x => x.POST_CODE).FirstOrDefault();
                var RelationshipID = _dbCommonData.NOKRelationships.Where(x => x.NOKRelationship1 == Relationship).Select(x => x.RelationCode).FirstOrDefault();


                objNOK_Change_Details.Relationship = RelationshipID;
                objNOK_Change_Details.GramaseDiv = GSDivisionID;
                objNOK_Change_Details.GramaseDivName = GsName;
                objNOK_Change_Details.District = districtId;
                objNOK_Change_Details.PresentprovinceId = Convert.ToInt32(provinceId);
                objNOK_Change_Details.NearTown = NearestTownID;
                objNOK_Change_Details.NearPoliceSta = PoliceStationId;
                objNOK_Change_Details.P2NearPostOff = PostOfficeId;


                status = objDALCommanQueryP2.HRMSdataEntering(MacAddress, UID, objNOK_Change_Details, RelationshipID, key);               

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return status;
        }

        #region Json Method
        public JsonResult getname(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Auto Load Rank and Name as per the Svc No
            /// 

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

        public JsonResult SelectDistrict(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Load District according to select province

            //List<District> FromEstablishment = new List<District>();
            var FromDistrictList = this.LinqSelectDistrict(id);

            var DistrictListData = FromDistrictList.Select(x => new SelectListItem()
            {
                Text = x.DESCRIPTION.ToString(),
                Value = x.DIST_CODE.ToString(),
            });


            return Json(DistrictListData, JsonRequestBehavior.AllowGet);
        }
        public IList<P2_District> LinqSelectDistrict(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            ///Description : Load District

            List<P2_District> Result = new List<P2_District>();
            Result = _dbCommonData.P2_District.Where(x => x.PROV_CODE == id).OrderBy(x => x.DESCRIPTION).ToList();
            return Result;
        }
        public JsonResult FromDistrict(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Load Section

            List<District> FromEstablishment = new List<District>();
            var FromDistrictList = this.LinqFromDistrict(id);

            var GSdivisionListData = FromDistrictList.Select(x => new SelectListItem()
            {
                Text = x.GSName.ToString(),
                Value = x.GSName.ToString(),
            });


            return Json(GSdivisionListData, JsonRequestBehavior.AllowGet);
        }
        public IList<GSDivision> LinqFromDistrict(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            ///Description : Load Section

            List<GSDivision> Result = new List<GSDivision>();
            Result = _dbCommonData.GSDivisions.Where(x => x.District == id).OrderBy(x => x.GSName).ToList();

            return Result;
        }
        public JsonResult FromPoliceStation(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Load Police Stations

            List<District> FromEstablishment = new List<District>();
            var LinqFromPoliceStation = this.LinqFromPoliceStation(id);

            var PoliceStationListData = LinqFromPoliceStation.Select(x => new SelectListItem()
            {
                Text = x.PoliceStation1.ToString(),
                Value = x.PoliceStation1.ToString(),
            });


            return Json(PoliceStationListData, JsonRequestBehavior.AllowGet);
        }
        public IList<PoliceStation> LinqFromPoliceStation(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description :  Load Police Stations

            List<PoliceStation> Result = new List<PoliceStation>();
            Result = _dbCommonData.PoliceStations.Where(x => x.District == id).OrderBy(x => x.PoliceStation1).ToList();

            return Result;
        }
        public JsonResult FromTown(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description : Load Town according to distrit

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
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description :  Load Town according to distrit

            List<P2_Town> Result = new List<P2_Town>();
            Result = _dbCommonData.P2_Town.Where(x => x.DIST_CODE == id).OrderBy(x => x.Town).ToList();

            return Result;
        }
        public JsonResult FromPostOffice(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
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
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            /// Description :  Load Town according to distrit

            List<P2_PostCode> Result = new List<P2_PostCode>();
            Result = _dbCommonData.P2_PostCode.Where(x => x.DIST_CODE == id).OrderBy(x => x.POST_NAME).ToList();

            return Result;
        }
        public JsonResult HrmisNokDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            ///Description  : get the hrmis include NOK Info

            Vw_NokDetailsP2 objNokDetailsP2 = new Vw_NokDetailsP2();
                     
            try
            {
                var count = _db.Vw_NokDetailsP2.Where(x => x.ActiveNo == id).Count();
                if (count == 1)
                {
                    objNokDetailsP2 = _db.Vw_NokDetailsP2.Where(x => x.ActiveNo == id).FirstOrDefault();
                }
                else
                {
                    objNokDetailsP2.NOKName = "No records to find.";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(objNokDetailsP2, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSLAFWorkingSPHusbandInfo(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/03/15
            ///Description  : get the hrmis include NOK Info/ Not In Use

            _GSQHeader obj_GSQHeader = new _GSQHeader();

            var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            var MarriageList = _dbP3HRMS.Marriages.Where(x => x.SNo == SNo && x.MarriedStatus == 1).OrderByDescending(x => x.CreatedDate).ToList();

            try
            {
                foreach (var item in MarriageList)
                {
                    ///item.IsArmedService == 1 mean, Spouse/husband worked in Armed Forces , item.ArmedService == 3 mean, Spouse/husband served in SLAF

                    if (item.IsArmedService == 1 && item.ArmedService == 3)
                    {
                        obj_GSQHeader.SpaouseServiceNo = item.SpouseServiceNo;
                        var spouseName = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_GSQHeader.SpaouseServiceNo).Select(x => x.Name).FirstOrDefault();
                        obj_GSQHeader.SpaouseName = item.SpouseRank + " " + spouseName;
                        obj_GSQHeader.SpaouseWrkStatus = 1;

                    }
                    else
                    {
                        obj_GSQHeader.SpaouseName = "Spouse/Husband is not serving at SLAF";
                        obj_GSQHeader.SpaouseWrkStatus = 2;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(obj_GSQHeader, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}