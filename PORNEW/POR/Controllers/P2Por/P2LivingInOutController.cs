using PagedList;
using POR.Models;
using POR.Models.LivingInOut;
using ReportData.DAL;
using ReportData.BAL;
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
    public class P2LivingInOutController : Controller
    {
        dbContext _db = new dbContext();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        DALCommanQueryP2 objDALCommanQueryP2 = new DALCommanQueryP2();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        P3HRMS _dbP3HRMS = new P3HRMS();
        string MacAddress = new DALBase().GetMacAddress();
        [HttpGet]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2023/02/08
            /// Description : Index Page for Forward Living InOut POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_LivingInOut> LivingInOuList = new List<_LivingInOut>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                //ViewBag.DateSortParm = sortOrder == "Sno" ? "Rank" : "CreateDate";
                ViewBag.CurrentFilter = searchString;
                //int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = UserInfo.RoleId;

                long sno = 0;

                if (!string.IsNullOrEmpty(searchString))
                {
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
                dt = objDALCommanQuery.CallLivingINOutSP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).CopyToDataTable();
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
                    _LivingInOut obj_LivingInOut = new _LivingInOut();
                    obj_LivingInOut.LIOHID = Convert.ToInt32(dt3.Rows[i]["LIOHID"]);
                    obj_LivingInOut.Snumber = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_LivingInOut.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_LivingInOut.FullName = dt3.Rows[i]["Name"].ToString();
                    obj_LivingInOut.Location = dt3.Rows[i]["Location"].ToString();
                    obj_LivingInOut.CategoryName = dt3.Rows[i]["LivingStatusName"].ToString();
                    obj_LivingInOut.RefNo = dt3.Rows[i]["RefNo"].ToString();


                    LivingInOuList.Add(obj_LivingInOut);

                }

                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(LivingInOuList.ToPagedList(pageNumber, pageSize));

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpGet]
        public ActionResult CreateBechelorsInOut()
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022.09.14
            ///Des: Load the page of create page of CreateBechelorsInOut

            ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
            ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
            ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
            ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
            ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");
            ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
            ViewBag.DDL_InOutCategories = new SelectList(_db.LivingStatus, "LSID", "LivingStatusName");
            ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
            ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");

            if (Session["UID"] != null)
            {
                int UID_ = Convert.ToInt32(Session["UID"]);
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
            }
            return View();

        }
        [HttpPost]
        public ActionResult CreateBechelorsInOut(_LivingInOut obj_LivingInOut, string btnName)
        {

            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022.09.14
            ///Des: Firts step data entering code

            LivingInOutHeader objLivingInOutHeader = new LivingInOutHeader();
            NOKChangeHeader ojbNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            FlowStatusLivingInOutDetail objFlowStatusLivingInOut = new FlowStatusLivingInOutDetail();
            List<PORRecordCount> proRecorlist = new List<PORRecordCount>();

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            obj_LivingInOut.RSID = (int)POR.Enum.RecordStatus.Insert;


           int UID_ = 0;           
            
            /// Get the service type related to service number.
            var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_LivingInOut.Snumber).Select(x => new {x.service_type,x.SNo }).FirstOrDefault();
            
            long ConSNo = Convert.ToInt64(ServiceInfo.SNo);

            ///Get District name
            //var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_LivingInOut.District).Select(x => x.DESCRIPTION).FirstOrDefault();
            try
            {
                if (Session["UID"] != null)
                {
                    UID_ = Convert.ToInt32(Session["UID"]);
                    string ConPROV_CODE = obj_LivingInOut.ProvinceId.ToString();
                    var UserInfoDetails = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => new { x.LocationId ,x.DivisionId ,x.RoleId}).FirstOrDefault();
                    var DistrictName = _dbCommonData.P2_District.Where(x => x.DIST_CODE == obj_LivingInOut.District).Select(x => x.DESCRIPTION).FirstOrDefault();
                    var ProvinceName = _dbCommonData.ProvinceNews.Where(x => x.PROV_CODE == ConPROV_CODE).Select(x => x.DESCRIPTION).FirstOrDefault();

                    ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "DESCRIPTION", "DESCRIPTION");
                    ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                    ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                    ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
                    ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                    ViewBag.DDL_InOutCategories = new SelectList(_db.LivingStatus, "LSID", "LivingStatusName");
                    ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
                    ViewBag.DDL_PostOffice = new SelectList(_dbCommonData.P2_PostCode, "PostOfficeName", "PostOfficeName");

                    proRecorlist = PorNoCreate(UserInfoDetails.LocationId);
                    string createdPorNo = proRecorlist[0].RefNo;

                    ModelState.Remove("RelationshipID");
                    ModelState.Remove("District");
                    ModelState.Remove("District1");
                    ModelState.Remove("Marriage_Status");
                    ModelState.Remove("ServiceNo");
                    ModelState.Remove("ToDate");
                    ModelState.Remove("FromDate");
                    ModelState.Remove("NokID");

                    int validity = ChecktheValidation(obj_LivingInOut.Authority, obj_LivingInOut.FromDate.ToString(), obj_LivingInOut.ToDate.ToString(), obj_LivingInOut.NOKWEFDate.ToString());

                    if (validity == 1)
                    {

                        if (obj_LivingInOut.NOKWEFDate != null)
                        {
                            objLivingInOutHeader.FromDate = Convert.ToDateTime(obj_LivingInOut.NOKWEFDate);
                        }
                        else
                        {
                            
                            objLivingInOutHeader.FromDate = obj_LivingInOut.FromDate;
                            objLivingInOutHeader.ToDate = obj_LivingInOut.ToDate;                       

                        }

                        objLivingInOutHeader.Sno = ConSNo;
                        objLivingInOutHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                        objLivingInOutHeader.LivingStatusID = obj_LivingInOut.LSID;
                        objLivingInOutHeader.Location = UserInfoDetails.LocationId;
                        objLivingInOutHeader.ServiceTypeId = ServiceInfo.service_type;
                        objLivingInOutHeader.RefNo = proRecorlist[0].RefNo;
                        objLivingInOutHeader.Authority = obj_LivingInOut.Authority;
                        objLivingInOutHeader.CreatedDate = DateTime.Now;
                        objLivingInOutHeader.CreatedBy = UID_;
                       // objLivingInOutHeader.CreatedMac = MacAddress;
                        objLivingInOutHeader.IPAddress = this.Request.UserHostAddress;
                        objLivingInOutHeader.Active = 1;


                        if (btnName == "No" || btnName == "Save Details")
                        {
                            objLivingInOutHeader.IsNOKChange = 2;
                            _db.LivingInOutHeaders.Add(objLivingInOutHeader);

                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                if (_db.SaveChanges() > 0)
                                {
                                    ///get the LIOHID
                                    var LIOHID = _db.LivingInOutHeaders.Where(x => x.Sno == ConSNo
                                               && x.Location == UserInfoDetails.LocationId && x.Active == 1 && x.RefNo == createdPorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.LIOHID).FirstOrDefault();

                                    /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                   InserFlowStatus(LIOHID, UserInfoDetails.RoleId, UID_, obj_LivingInOut.FMSID, obj_LivingInOut.RSID);

                                    scope.Complete();
                                    TempData["ScfMsg"] = "Data Successfully Saved.";
                                }
                                else
                                {
                                    TempData["ErrMsg"] = "Process Unsuccessful.Try again.";
                                    scope.Dispose();
                                }

                            }
                        }
                        else
                        {
                            objLivingInOutHeader.IsNOKChange = 1;
                            _db.LivingInOutHeaders.Add(objLivingInOutHeader);
                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                if (_db.SaveChanges() > 0)
                                {
                                    var LIOHID = _db.LivingInOutHeaders.Where(x => x.Sno == ConSNo
                                                  && x.Location == UserInfoDetails.LocationId && x.Active == 1 && x.RefNo == createdPorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.LIOHID).FirstOrDefault();

                                    ///This linq get the NOK Change Status and it save in NOKChangeHeader table, SubCatID 100 mean 'Living In/Out Change' Status
                                    var NokStatus = _db.NOKStatus.Where(x => x.SubCatID == 100 && x.Active == 1).Select(x => x.NOKSID).FirstOrDefault();

                                    ojbNOKChangeHeader.InOutHeaderID = LIOHID;
                                    ojbNOKChangeHeader.Sno = ConSNo;
                                    ojbNOKChangeHeader.Location = UserInfoDetails.LocationId;
                                    ojbNOKChangeHeader.NOKStatus = NokStatus;
                                    ojbNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                                    ojbNOKChangeHeader.WFDate = obj_LivingInOut.NOKWEFDate;
                                    ojbNOKChangeHeader.RefNo = proRecorlist[0].RefNo;
                                    ojbNOKChangeHeader.Authority = obj_LivingInOut.Authority;
                                    ojbNOKChangeHeader.CreatedBy = UID_;
                                    ojbNOKChangeHeader.CreatedDate = DateTime.Now;
                                    //ojbNOKChangeHeader.CreatedMac = MacAddress;
                                    ojbNOKChangeHeader.CreateIpAddess = this.Request.UserHostAddress;
                                    ojbNOKChangeHeader.Active = 1;

                                    _db.NOKChangeHeaders.Add(ojbNOKChangeHeader);

                                    if (_db.SaveChanges() > 0)
                                    {
                                        var NOKCHID = _db.NOKChangeHeaders.Where(x => x.Sno == ConSNo
                                                 && x.Location == UserInfoDetails.LocationId && x.NOKStatus == NokStatus && x.Active == 1 && x.InOutHeaderID == LIOHID).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();

                                        objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                        objNOKChangeDetail.NOKAddress = obj_LivingInOut.NOKaddress;
                                        objNOKChangeDetail.NOKName = obj_LivingInOut.NOKName;
                                        objNOKChangeDetail.NOKChangeTo = obj_LivingInOut.NOKRelationship1;
                                        objNOKChangeDetail.Province = ProvinceName;
                                        objNOKChangeDetail.District = DistrictName;
                                        objNOKChangeDetail.GSDivision = obj_LivingInOut.GSName;
                                        objNOKChangeDetail.NearestTown = obj_LivingInOut.Town1;
                                        objNOKChangeDetail.PoliceStation = obj_LivingInOut.PoliceStation1;
                                        objNOKChangeDetail.PostOffice = obj_LivingInOut.PostOfficeName;
                                        objNOKChangeDetail.Remarks = obj_LivingInOut.Remarks;
                                        objNOKChangeDetail.CreatedBy = UID_;
                                        objNOKChangeDetail.CreatedDate = DateTime.Now;
                                        //objNOKChangeDetail.CreatedMac = MacAddress;
                                        objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                        objNOKChangeDetail.Active = 1;

                                        _db.NOKChangeDetails.Add(objNOKChangeDetail);

                                        /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                        InserFlowStatus(LIOHID, UserInfoDetails.RoleId, UID_, obj_LivingInOut.FMSID, obj_LivingInOut.RSID);                                        
                                        scope.Complete();
                                        TempData["ScfMsg"] = "Data Successfully Saved.";

                                    }
                                    else
                                    {
                                        TempData["ErrMsg"] = "Process Unsuccessful.Try again.";
                                        scope.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (validity)
                        {
                            case 0:
                                TempData["ErrMsg"] = "Input Fields Can not be empty. Please Fill all the Fields.";
                                break;
                            case 2:
                                TempData["ErrMsg"] = "Please Do not exceed the To Date 6 month of Period";
                                break;
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
            //ModelState.Clear();
            return View();
        }        
        [HttpGet]
        public ActionResult Details(int LIOHID, int Rejectstatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/02/09
            /// Description : Details related Living In Out por 

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                int? CurrentStatusUserRole;
                string LivingStatusCode;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_LivingInOut> LivingInOutList = new List<_LivingInOut>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => new {x.RoleId,x.LocationId}).First();                
                TempData["UserRoleId"] = userInfo.RoleId;                

                var CurrentStatus_UserRole = (from f in _db.FlowStatusLivingInOutDetails
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.InOut_ID == LIOHID
                                              orderby f.LFSID descending
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
                dt = objDALCommanQuery.CallLivingINOutSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("LIOHID") == LIOHID).CopyToDataTable();


                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (Rejectstatus != 2)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _LivingInOut obj_LivingInOut = new _LivingInOut();
                        object valFromDate = dt2.Rows[i]["FromDate"];
                        object valToDate = dt2.Rows[i]["ToDate"];

                        /// Check the rercord is previously reject or not
                        var prvReject = _db.LivingInOutHeaders.Where(x => x.LIOHID == LIOHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();


                        if (valFromDate != DBNull.Value && valToDate != DBNull.Value)
                        {
                            obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                            obj_LivingInOut.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);
                        }

                        if (valFromDate != DBNull.Value)
                        {
                            obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);

                        }

                        obj_LivingInOut.LIOHID = Convert.ToInt32(dt2.Rows[i]["LIOHID"]);
                        obj_LivingInOut.Snumber = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_LivingInOut.ServiceNo = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                        obj_LivingInOut.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_LivingInOut.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_LivingInOut.Location = dt2.Rows[i]["Location"].ToString();
                        obj_LivingInOut.CategoryName = dt2.Rows[i]["LivingStatusName"].ToString();
                        obj_LivingInOut.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_LivingInOut.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_LivingInOut.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_LivingInOut.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_LivingInOut.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_LivingInOut.ProvinceName = dt2.Rows[i]["Province"].ToString();
                        obj_LivingInOut.District1 = dt2.Rows[i]["District"].ToString();
                        obj_LivingInOut.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_LivingInOut.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_LivingInOut.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_LivingInOut.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_LivingInOut.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_LivingInOut.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);

                        if (prvReject >= 1)
                        {
                            obj_LivingInOut.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_LivingInOut.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_LivingInOut.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                        {
                            obj_LivingInOut.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                        }

                        LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        TempData["LivingStatusCode"] = LivingStatusCode;

                        LivingInOutList.Add(obj_LivingInOut);
                    }

                    //var CivilStatusDetail = _db.CivilStatusHeaders.Where(x => x.CSHID == CSHID);
                    //int? CurrentStatus = CivilStatusDetail.Select(x => x.C).First();
                    //TempData["CurrentStatus"] = CurrentStatus;

                    //int? SubmitStatus = LivingInOutDetail.Select(x => x.SubmitStatus).First();
                    //TempData["SubmitStatus"] = SubmitStatus;

                    return View(LivingInOutList);
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusLivingInOutDetails.Where(x => x.RSID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.InOut_ID == LIOHID)
                                        .OrderByDescending(x => x.LFSID).Select(x => x.RID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _LivingInOut obj_LivingInOut = new _LivingInOut();
                        object valFromDate = dt2.Rows[i]["FromDate"];
                        object valToDate = dt2.Rows[i]["ToDate"];

                        if (valFromDate != DBNull.Value && valToDate != DBNull.Value)
                        {
                            obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                            obj_LivingInOut.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);
                        }

                        if (valFromDate != DBNull.Value)
                        {
                            obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                        }

                        obj_LivingInOut.LIOHID = Convert.ToInt32(dt2.Rows[i]["LIOHID"]);
                        obj_LivingInOut.Snumber = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_LivingInOut.ServiceNo = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                        obj_LivingInOut.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_LivingInOut.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_LivingInOut.Location = dt2.Rows[i]["Location"].ToString();
                        obj_LivingInOut.CategoryName = dt2.Rows[i]["LivingStatusName"].ToString();
                        obj_LivingInOut.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_LivingInOut.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_LivingInOut.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_LivingInOut.RejectRoleName = RoleName.ToString();
                        obj_LivingInOut.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_LivingInOut.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_LivingInOut.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_LivingInOut.ProvinceName = dt2.Rows[i]["Province"].ToString();
                        obj_LivingInOut.District1 = dt2.Rows[i]["District"].ToString();
                        obj_LivingInOut.GSName = dt2.Rows[i]["GSDivision"].ToString();
                        obj_LivingInOut.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_LivingInOut.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_LivingInOut.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_LivingInOut.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_LivingInOut.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);


                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_LivingInOut.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                        {
                            obj_LivingInOut.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                        }

                        LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        TempData["LivingStatusCode"] = LivingStatusCode;

                        LivingInOutList.Add(obj_LivingInOut);
                    }

                    return View(LivingInOutList);

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
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/02/09
            ///Description : Delete the p2 Clerk entred data. All Active 1 record turn into 0

            LivingInOut objLivingInOut = new LivingInOut();
            LivingInOutHeader objLivingInOutHeader = new LivingInOutHeader();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            int UID_ = Convert.ToInt32(Session["UID"]);

            var NokchangeStatus = _db.LivingInOutHeaders.Where(x => x.LIOHID == id && x.Active == 1).Select(x => x.IsNOKChange).FirstOrDefault();
            try
            {

                objLivingInOutHeader = _db.LivingInOutHeaders.Find(id);
                objLivingInOutHeader.Active = 0;
                objLivingInOutHeader.ModifiedDate = DateTime.Now;
                objLivingInOutHeader.ModifiedBy = UID_;
                objLivingInOutHeader.ModifiedMac = MacAddress;

                _db.Entry(objLivingInOutHeader).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        if (NokchangeStatus == 1)
                        {
                            ///NokchangeStatus == 1 mean Livining In out record have NOK change details

                            var NokHId = _db.NOKChangeHeaders.Where(x => x.InOutHeaderID == id && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();
                            var NokDId = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == NokHId && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();

                            objNOKChangeHeader = _db.NOKChangeHeaders.Find(NokHId);
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
                        else
                        {
                            scope.Complete();
                            TempData["ScfMsg"] = "Record Sucessfully Deleted.";
                        }
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsucessfull.";
                        scope.Dispose();
                    }
                }
                return RedirectToAction("IndexLivingINOutStatus", "LivingInOut");
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
            ///Created Date : 2023/02/10
            ///Description : Load Edit page with user enter data.

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                string LivingStatusCode;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_LivingInOut> LivingInOutList = new List<_LivingInOut>();
                string RejectRef;

                ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");

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
                dt = objDALCommanQuery.CallLivingINOutSP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("LIOHID") == id).CopyToDataTable();
                _LivingInOut obj_LivingInOut = new _LivingInOut();
                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {

                    object valFromDate = dt2.Rows[i]["FromDate"];
                    object valToDate = dt2.Rows[i]["ToDate"];

                    if (valFromDate != DBNull.Value && valToDate != DBNull.Value)
                    {
                        obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                        obj_LivingInOut.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);
                    }

                    if (valFromDate != DBNull.Value)
                    {
                        obj_LivingInOut.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);

                    }

                    obj_LivingInOut.LIOHID = Convert.ToInt32(dt2.Rows[i]["LIOHID"]);
                    obj_LivingInOut.Snumber = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_LivingInOut.ServiceNo = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                    obj_LivingInOut.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_LivingInOut.FullName = dt2.Rows[i]["Name"].ToString();
                    obj_LivingInOut.Location = dt2.Rows[i]["Location"].ToString();
                    obj_LivingInOut.CategoryName = dt2.Rows[i]["LivingStatusName"].ToString();
                    obj_LivingInOut.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_LivingInOut.Authority = dt2.Rows[i]["Authority"].ToString();
                    obj_LivingInOut.NOKName = dt2.Rows[i]["NOKName"].ToString();
                    obj_LivingInOut.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                    obj_LivingInOut.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                    obj_LivingInOut.EditedProvince = dt2.Rows[i]["Province"].ToString();
                    obj_LivingInOut.EditedDistrict1 = dt2.Rows[i]["District"].ToString();
                    obj_LivingInOut.EditedGSnumber = dt2.Rows[i]["GSDivision"].ToString();
                    obj_LivingInOut.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                    obj_LivingInOut.EditPoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                    obj_LivingInOut.EditPostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                    obj_LivingInOut.Remarks = dt2.Rows[i]["Remarks"].ToString();
                    obj_LivingInOut.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);

                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.LivingInOutHeaders.Where(x => x.LIOHID == id && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_LivingInOut.RefNo + " " + " - Reject";
                            obj_LivingInOut.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_LivingInOut.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_LivingInOut.RejectRefNo = RejectRef;
                        }
                    }


                    if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                    {
                        obj_LivingInOut.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                    }
                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                    {
                        obj_LivingInOut.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                    }

                    LivingStatusCode = dt2.Rows[i]["LivingStatusID"].ToString();

                    TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                    TempData["LivingStatusCode"] = LivingStatusCode;                   
                }
                return View(obj_LivingInOut);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }


        }
        [HttpPost]
        public ActionResult Edit(_LivingInOut obj_LivingInOut, int rejectStatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/05/23
            ///Description : Save User edit data to Data based

            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            LivingInOutHeader objLivingInOutHeader = new LivingInOutHeader();
            FlowStatusLivingInOutDetail objFSL = new FlowStatusLivingInOutDetail();

            ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
            ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
            ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
            ViewBag.DDL_Relationship = new SelectList(_dbCommonData.NOKRelationships, "NOKRelationship1", "NOKRelationship1");


            int UID_ = 0;
            int RoleId = 0;

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            int RecordStatus = (int)POR.Enum.RecordStatus.Insert;
            try
            {
                UID_ = Convert.ToInt32(Session["UID"]);
                var LivingDetails = _db.LivingInOutHeaders.Where(x => x.LIOHID == obj_LivingInOut.LIOHID && x.Active == 1).Select(x => new { x.IsNOKChange, x.LivingStatusID }).FirstOrDefault();

                string ConPROV_CODE = obj_LivingInOut.ProvinceId.ToString();
                var ProvinceName = _dbCommonData.ProvinceNews.Where(x => x.PROV_CODE == ConPROV_CODE).Select(x => x.DESCRIPTION).FirstOrDefault();
                ///Get District name
                var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_LivingInOut.District).Select(x => x.DESCRIPTION).FirstOrDefault();
                
                //NoKStatus == 1 means Record has change the Nok details
                if (LivingDetails.IsNOKChange != 1)
                {
                    objLivingInOutHeader = _db.LivingInOutHeaders.Find(obj_LivingInOut.LIOHID);

                    if (LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.BLIN || LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.MLIN || LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.MLOut)
                    {
                        objLivingInOutHeader.Authority = obj_LivingInOut.Authority;
                        objLivingInOutHeader.FromDate = obj_LivingInOut.FromDate;
                    }
                    else
                    {
                        objLivingInOutHeader.Authority = obj_LivingInOut.Authority;
                        objLivingInOutHeader.FromDate = obj_LivingInOut.FromDate;
                        objLivingInOutHeader.ToDate = obj_LivingInOut.ToDate;
                    }
                    objLivingInOutHeader.ModifiedBy = UID_;
                    objLivingInOutHeader.ModifiedDate = DateTime.Now;
                    objLivingInOutHeader.ModifiedMac = MacAddress;

                    _db.Entry(objLivingInOutHeader).State = EntityState.Modified;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (_db.SaveChanges() > 0)
                        {
                            ////Here check the reject record has been edited
                            if (rejectStatus == 2)
                            {
                                ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                RoleId = (int)POR.Enum.UserRole.P2CLERK;
                                InserFlowStatus(obj_LivingInOut.LIOHID, RoleId, UID_, obj_LivingInOut.FMSID, RecordStatus);

                                /// Update Living In/Out Header details to table
                                /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                var rejectCount = _db.LivingInOutHeaders.Where(x => x.LIOHID == obj_LivingInOut.LIOHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                objLivingInOutHeader = _db.LivingInOutHeaders.Find(obj_LivingInOut.LIOHID);
                                if (rejectCount == null)
                                {
                                    objLivingInOutHeader.PreviousReject = 1;
                                }
                                else
                                {
                                    int refIncrement = Convert.ToInt32(rejectCount + 1);
                                    objLivingInOutHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                }
                                objLivingInOutHeader.RejectAuth = obj_LivingInOut.RejectRefNo;
                                _db.Entry(objLivingInOutHeader).State = EntityState.Modified;

                                /// previous reject record active 1 status turn in to 0
                                var RejLFSID = _db.FlowStatusLivingInOutDetails.Where(x => x.InOut_ID == obj_LivingInOut.LIOHID && x.RSID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.LFSID).FirstOrDefault();
                                objFSL = _db.FlowStatusLivingInOutDetails.Find(RejLFSID);
                                objFSL.Active = 0;
                                objFSL.ModifiedBy = UID_;
                                objFSL.ModifiedDate = DateTime.Now;
                                objFSL.ModifiedMac = MacAddress;
                                _db.Entry(objFSL).State = EntityState.Modified;

                                if (_db.SaveChanges() > 0)
                                {
                                    scope.Complete();
                                    TempData["ScfMsg"] = "Data Edited Successfully";
                                }
                                else
                                {
                                    scope.Dispose();
                                    TempData["ErrMsg"] = " Not Complete Your Process";
                                }
                            }
                            else
                            {
                                scope.Complete();
                                TempData["ScfMsg"] = "Data Edited Successfully";
                            }
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
                    objLivingInOutHeader = _db.LivingInOutHeaders.Find(obj_LivingInOut.LIOHID);

                    if (LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.BLIN || LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.MLIN || LivingDetails.LivingStatusID == (int)POR.Enum.LivinInOutCategories.MLOut)
                    {
                        objLivingInOutHeader.Authority = obj_LivingInOut.Authority;
                        objLivingInOutHeader.FromDate = obj_LivingInOut.FromDate;
                    }
                    else
                    {
                        objLivingInOutHeader.Authority = obj_LivingInOut.Authority;
                        objLivingInOutHeader.FromDate = obj_LivingInOut.FromDate;
                        objLivingInOutHeader.ToDate = obj_LivingInOut.ToDate;
                    }
                    objLivingInOutHeader.ModifiedBy = UID_;
                    objLivingInOutHeader.ModifiedDate = DateTime.Now;
                    objLivingInOutHeader.ModifiedMac = MacAddress;

                    _db.Entry(objLivingInOutHeader).State = EntityState.Modified;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if ((_db.SaveChanges() > 0))
                        {
                            /// Update Nok header table
                            var NOKHID = _db.NOKChangeHeaders.Where(x => x.InOutHeaderID == obj_LivingInOut.LIOHID && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();

                            objNOKChangeHeader = _db.NOKChangeHeaders.Find(NOKHID);
                            objNOKChangeHeader.WFDate = obj_LivingInOut.NOKWEFDate;
                            objNOKChangeHeader.ModifiedBy = UID_;
                            objNOKChangeHeader.ModifiedDate = DateTime.Now;
                            objNOKChangeHeader.ModifiedMac = MacAddress;
                            _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                            /// Update Nok Detail table
                            var NOKDID = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == NOKHID && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();

                            objNOKChangeDetail = _db.NOKChangeDetails.Find(NOKDID);
                            objNOKChangeDetail.NOKName = obj_LivingInOut.NOKName;

                            if (obj_LivingInOut.NOKRelationship1 != null)
                            {
                                objNOKChangeDetail.NOKChangeTo = obj_LivingInOut.NOKRelationship1;
                            }
                            objNOKChangeDetail.NOKAddress = obj_LivingInOut.NOKaddress;

                            //// meka kale different type of object assign value
                            if (obj_LivingInOut.ProvinceId != 0)
                            {
                                                               
                                objNOKChangeDetail.Province = ProvinceName;

                                if (obj_LivingInOut.District !=0)
                                {
                                    objNOKChangeDetail.District = DistName;
                                }
                                if (obj_LivingInOut.GSName != null)
                                {
                                    objNOKChangeDetail.GSDivision = obj_LivingInOut.GSName;
                                }

                                if (obj_LivingInOut.Town1 != "SELECT")
                                {
                                    objNOKChangeDetail.NearestTown = obj_LivingInOut.Town1;
                                }
                                
                                if (obj_LivingInOut.PoliceStation1 != null)
                                {
                                    objNOKChangeDetail.PoliceStation = obj_LivingInOut.PoliceStation1;
                                }
                                if (obj_LivingInOut.PostOfficeName != "SELECT")
                                {
                                    objNOKChangeDetail.PostOffice = obj_LivingInOut.PostOfficeName;
                                }

                            }
                            _db.Entry(objNOKChangeDetail).State = EntityState.Modified;

                            if (_db.SaveChanges() > 0)
                            {
                                ////Here check the reject record has been edited
                                if (rejectStatus == 2)
                                {
                                    ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                    RoleId = (int)POR.Enum.UserRole.P2CLERK;
                                    InserFlowStatus(obj_LivingInOut.LIOHID, RoleId, UID_, obj_LivingInOut.FMSID, RecordStatus);

                                    /// Update Living In/Out Header details to table
                                    /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                    var rejectCount = _db.LivingInOutHeaders.Where(x => x.LIOHID == obj_LivingInOut.LIOHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                    objLivingInOutHeader = _db.LivingInOutHeaders.Find(obj_LivingInOut.LIOHID);
                                    if (rejectCount == null)
                                    {
                                        objLivingInOutHeader.PreviousReject = 1;
                                    }
                                    else
                                    {
                                        int refIncrement = Convert.ToInt32(rejectCount + 1);
                                        objLivingInOutHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                    }
                                    objLivingInOutHeader.RejectAuth = obj_LivingInOut.RejectRefNo;
                                    _db.Entry(objLivingInOutHeader).State = EntityState.Modified;

                                    /// previous reject record active 1 status turn in to 0
                                    var RejLFSID = _db.FlowStatusLivingInOutDetails.Where(x => x.InOut_ID == obj_LivingInOut.LIOHID && x.RSID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.LFSID).FirstOrDefault();
                                    objFSL = _db.FlowStatusLivingInOutDetails.Find(RejLFSID);
                                    objFSL.Active = 0;
                                    objFSL.ModifiedBy = UID_;
                                    objFSL.ModifiedDate = DateTime.Now;
                                    objFSL.ModifiedMac = MacAddress;
                                    _db.Entry(objFSL).State = EntityState.Modified;

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
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("Index", "P2LivingInOut");
        }
        [HttpGet]
        public ActionResult Forward(int? id, string Sno, int? InOut_CAT_ID)
        {
            //Singal Forward

            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023.02.10
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
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new {x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();                
                string RecordEstablishmentId = _db.LivingInOutHeaders.Where(x => x.LIOHID == id).Select(x => x.Location).FirstOrDefault();   
                int? SubmitStatus = NextFlowStatusId(id, userInfo.LocationId, RecordEstablishmentId, userInfo.DivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                FlowStatusLivingInOutDetail objFlowStatusLivingInOut = new FlowStatusLivingInOutDetail();
                FlowStatusCivilStatusDetail objFlowStatusCivilStatusDetail = new FlowStatusCivilStatusDetail();

                objFlowStatusLivingInOut.InOut_ID = id;
                objFlowStatusLivingInOut.RSID = (int)POR.Enum.RecordStatus.Forward;
                objFlowStatusLivingInOut.UID = UID;
                objFlowStatusLivingInOut.FMSID = SubmitStatus;
                objFlowStatusLivingInOut.RID = UserRoleId;
                objFlowStatusLivingInOut.CreatedBy = UID;
                objFlowStatusLivingInOut.CreatedDate = DateTime.Now;
                objFlowStatusLivingInOut.CreatedMac = MacAddress;
                objFlowStatusLivingInOut.IPAddress = this.Request.UserHostAddress;
                objFlowStatusLivingInOut.Active = 1;

                ///This function is to update the Hrmis data base. After account one certified, the details will update P2 HRMS 
                if (userInfo.RoleId == (int)POR.Enum.UserRole.ACCOUNTS01)
                {
                    var NokChangeState = _db.LivingInOutHeaders.Where(x => x.LIOHID == id && x.Active == 1).Select(x => new { x.IsNOKChange, x.LivingStatusID, x.Sno }).FirstOrDefault();

                    var LivingStatusName = _db.LivingStatus.Where(x => x.LSID == NokChangeState.LivingStatusID).Select(x => x.LivingStatusShortName).FirstOrDefault();
                    switch (NokChangeState.IsNOKChange)
                    {
                        /// case 01 mean nok changed
                        case 1:
                            /// Insert new record to P3hrms Nokchange details table and update the P3hrms service personal profile LivingIn_Out Column 
                            updateStatus = UpdateHrmis(LivingStatusName, MacAddress, UID, NokChangeState.Sno, id);
                            break;
                        ///case 02 mean nok not change
                        case 2:
                            /// Update the P3hrms service personal profile LivingIn_Out Column                           
                            updateStatus = objDALCommanQueryP2.SQL_UpdateServicePersonnelProfileP2(LivingStatusName, MacAddress, UID, NokChangeState.Sno);
                            break;
                        default:
                            break;
                    }
                    if (updateStatus == true)
                    {
                        _db.FlowStatusLivingInOutDetails.Add(objFlowStatusLivingInOut);
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
                    _db.FlowStatusLivingInOutDetails.Add(objFlowStatusLivingInOut);
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                        return RedirectToAction("Index");

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
            string UserEstablishmentId = null;
            string RecordEstablishmentId = null;
            int? UserRoleId = 0;
            int? UID = 0;
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            if (id != null)
            {
                foreach (int IDs in id)
                {
                    if (UID != 0)
                    {

                        var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
                        UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                        RecordEstablishmentId = _db.LivingInOutHeaders.Where(x => x.LIOHID == IDs).Select(x => x.Location).FirstOrDefault();
                        string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();


                        int? SubmitStatus = NextFlowStatusId(IDs, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);
                        //Get Next FlowStatus User Role Name for Add Successfull Msg

                        UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                        SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                        FlowStatusLivingInOutDetail objFlowStatusLivingInOutDetail = new FlowStatusLivingInOutDetail();

                        objFlowStatusLivingInOutDetail.InOut_ID = IDs;
                        objFlowStatusLivingInOutDetail.RSID = (int)POR.Enum.RecordStatus.Forward;
                        objFlowStatusLivingInOutDetail.UID = UID;
                        objFlowStatusLivingInOutDetail.FMSID = SubmitStatus;
                        objFlowStatusLivingInOutDetail.RID = UserRoleId;
                        objFlowStatusLivingInOutDetail.CreatedBy = UID;
                        objFlowStatusLivingInOutDetail.CreatedDate = DateTime.Now;
                        objFlowStatusLivingInOutDetail.CreatedMac = MacAddress;
                        objFlowStatusLivingInOutDetail.IPAddress = this.Request.UserHostAddress;
                        objFlowStatusLivingInOutDetail.Active = 1;

                        _db.FlowStatusLivingInOutDetails.Add(objFlowStatusLivingInOutDetail);
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
            }


            return Json("");
        }
        public bool UpdateHrmis(string LivingStatusName, string MacAddress, int? UID, long? Sno, int? InOutHeaderID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/12/06
            /// Description : update the hrmis

            _LivingInOut obj_LivingInOut = new _LivingInOut();
            BALNOK_Change_Details objNOK_Change_Details = new BALNOK_Change_Details();
            DataTable dt = new DataTable();

            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.livinInOut;
            string Relationship = "";
            string ProvinceName = "";
            string districtName = "";
            string GsName = "";
            string nearestTown = "";
            string policeStation = "";
            string postOffice = "";
            bool status;
                       
            string SSNo = Convert.ToString(Sno);
            try
            {
                // get the Next NOK Primary key ID
                int key = objDALCommanQueryP2.GenerateKey();
                string NokID = Convert.ToString("NOKID/" + key);
                
                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(InOutHeaderID, detailsCollectCategory);

                foreach (DataRow row in dt.Rows)
                {
                    Relationship = row["NOKChangeTo"].ToString();
                    ProvinceName = row["Province"].ToString();
                    districtName = row["District"].ToString();
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
                    objNOK_Change_Details.PORRefNo = row["RefNo"].ToString();
                    objNOK_Change_Details.CreatedDate = DateTime.Now;
                    objNOK_Change_Details.CreatedUser = Convert.ToString(UID);
                    objNOK_Change_Details.CreatedMachine = MacAddress;

                }

                var districtId = _dbCommonData.Districts.Where(x => x.DESCRIPTION == districtName).Select(x => x.DIST_CODE).FirstOrDefault();
                var provinceId = _dbCommonData.ProvinceNews.Where(x => x.DESCRIPTION == ProvinceName).Select(x => x.PROV_CODE).FirstOrDefault();
                var GSDivisionID = _dbCommonData.P2_GSDivision.Where(x => x.GSName == GsName).Select(x => x.GSCode).FirstOrDefault();
                var NearestTownID = _dbCommonData.P2_Town.Where(x => x.Town == nearestTown).Select(x => x.TownCOde).FirstOrDefault();
                var PoliceStationId = _dbCommonData.P2_PoliceStation.Where(x => x.PoliceStation == policeStation).Select(x => x.PoliceStationCode).FirstOrDefault();
                var PostOfficeId = _dbCommonData.P2_PostCode.Where(x => x.POST_NAME == postOffice).Select(x => x.POST_CODE).FirstOrDefault();
                var RelationshipID = _dbCommonData.NOKRelationships.Where(x => x.NOKRelationship1 == Relationship).Select(x => x.RelationCode).FirstOrDefault();

                objNOK_Change_Details.Relationship = Convert.ToInt32(RelationshipID);
                objNOK_Change_Details.GramaseDiv = GSDivisionID;
                objNOK_Change_Details.GramaseDivName = GsName;
                objNOK_Change_Details.District = districtId;
                objNOK_Change_Details.PresentprovinceId = Convert.ToInt32(provinceId);
                objNOK_Change_Details.NearTown = NearestTownID;
                objNOK_Change_Details.NearPoliceSta = PoliceStationId;
                objNOK_Change_Details.P2NearPostOff = PostOfficeId;
                

                status = objDALCommanQueryP2.HRMSdataEntering(LivingStatusName, MacAddress, UID, objNOK_Change_Details, RelationshipID, key);

            }            
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public int? NextFlowStatusId(int? InOut_ID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Fg off RGSD GAMAGE
            ///Created Date :2021.03.25
            ///Des: get the next flow status id FMSID

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxLFSID = _db.FlowStatusLivingInOutDetails.Where(x => x.InOut_ID == InOut_ID).Select(x => x.LFSID).Max();
                int? CurrentFMSID = _db.FlowStatusLivingInOutDetails.Where(x => x.LFSID == MaxLFSID).Select(x => x.FMSID).FirstOrDefault();
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
        public ActionResult Reject(int LIOHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/02/09
            /// Description : this function is to reject the record

            _LivingInOut model = new _LivingInOut();
            try
            {
                model.LIOHID = LIOHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_P2RejectCommentLivingInOut", model);
        }
        [HttpGet]
        public ActionResult IndexRejectLivingInOutP2(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/02/13
            /// Description : Show the reject list user wise

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            ViewBag.CurrentSort = sortOrder;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault(); 
            var FMSID = _db.FlowManagementStatus.Where(x => x.DivisionId == UserInfo.DivisionId && x.EstablishmentId == UserInfo.LocationId && x.UserRoleID == UserInfo.RoleId).Select(x => x.FMSID).FirstOrDefault();

            TempData["RoleId"] = UserInfo.RoleId;
            if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK)
            {
                var objLivingStatus = _db.Vw_LivingStatusReject.Take(500).Where(x => x.FMSID == FMSID && x.CurrentStatus == UserInfo.RoleId && x.Location == UserInfo.LocationId && x.LIActive == 1 && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer|| 
                                      x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer)).OrderByDescending(x => x.LIOHID).ToList();

                #region Switch
                //objLivingStatus = objLivingStatus.Where(x => x.FMSID == FMSID && x.CurrentStatus == UserInfo.RoleId).ToList();

                switch (sortOrder)
                {

                    case "Service No":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "Rank":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Rank).ToList();
                        break;
                    case "Name With Initials":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Name).ToList();
                        break;
                    case "Category Name":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.LivingStatusName).ToList();
                        break;
                    case "POR No":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.RefNo).ToList();
                        break;
                    case "Establishment":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Location).ToList();
                        break;
                    case "Authority":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Authority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objLivingStatus.ToPagedList(pageNumber, pageSize));
                #endregion
            }
            else
            {
                var objLivingStatus = _db.Vw_LivingStatusReject.Take(500).Where(x => x.LIActive == 1).OrderByDescending(x => x.LIOHID).ToList();

                if (UserInfo.RoleId == (int)POR.Enum.UserRole.KOPNR || UserInfo.RoleId == (int)POR.Enum.UserRole.SNCOSALARY || UserInfo.RoleId == (int)POR.Enum.UserRole.WOSALARY ||
                    UserInfo.RoleId == (int)POR.Enum.UserRole.ACCOUNTS01 || UserInfo.RoleId == (int)POR.Enum.UserRole.HRMSCLKP2VOL || UserInfo.RoleId == (int)POR.Enum.UserRole.HRMSP2SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.ASORSOVRP2VOL)
                {

                     objLivingStatus = _db.Vw_LivingStatusReject.Take(500).Where(x => x.LIActive == 1 && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer ||
                                    x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer)).OrderByDescending(x => x.LIOHID).ToList();

                    
                }
                else
                {
                    objLivingStatus = _db.Vw_LivingStatusReject.Take(500).Where(x =>x.Location == UserInfo.LocationId && x.LIActive == 1 && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer ||
                                    x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer)).OrderByDescending(x => x.LIOHID).ToList();                   

                }

                #region switch

                switch (sortOrder)
                {

                    case "Service No":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.ServiceNo).ToList();
                        break;
                    case "Rank":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Rank).ToList();
                        break;
                    case "Name With Initials":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Name).ToList();
                        break;
                    case "Category Name":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.LivingStatusName).ToList();
                        break;
                    case "POR No":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.RefNo).ToList();
                        break;
                    case "Establishment":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Location).ToList();
                        break;
                    case "Authority":
                        objLivingStatus = objLivingStatus.OrderBy(s => s.Authority).ToList();
                        break;
                }

                pageSize = 10;
                pageNumber = (page ?? 1);
                return View(objLivingStatus.ToPagedList(pageNumber, pageSize));

                #endregion

            }
        }
        [HttpGet]
        public ActionResult RejectConfirm(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/08/31
            /// Description : P3 Clerk finally confirm the reject Confirm. After confirm record Status came to 0

            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);
                var NOKCHID = _db.NOKChangeHeaders.Where(x => x.InOutHeaderID == id && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();

                //Update LivingInOutHeader Active colum to 0
                LivingInOutHeader objLivingInOutHeader = _db.LivingInOutHeaders.Find(id);
                objLivingInOutHeader.Active = 0;
                objLivingInOutHeader.ModifiedBy = UID_;
                objLivingInOutHeader.ModifiedDate = DateTime.Now;
                objLivingInOutHeader.ModifiedMac = MacAddress;
                _db.Entry(objLivingInOutHeader).Property(x => x.Active).IsModified = true;

                if (NOKCHID != 0)
                {
                    //Update NOKChangeHeader Active colum to 0
                    NOKChangeHeader objNOKChangeHeader = _db.NOKChangeHeaders.Find(NOKCHID);
                    objNOKChangeHeader.Active = 0;
                    objNOKChangeHeader.ModifiedBy = UID_;
                    objNOKChangeHeader.ModifiedDate = DateTime.Now;
                    objNOKChangeHeader.ModifiedMac = MacAddress;

                    _db.Entry(objNOKChangeHeader).Property(x => x.Active).IsModified = true;
                }

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Successfully Complete the Reject Confirmed.";
                        scope.Complete();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again.";
                        scope.Dispose();
                    }
                    return RedirectToAction("IndexRejectLivingInOutP2");
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
            ///Created Date : 2021/10/26
            /// Description : Load data user roll wise, this fuction call from IndexLivingINOutStatus()

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();


                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P2CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var row = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                        if (row.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                        }                       
                        
                        break;
                    #endregion

                    default:

                        #region CodeArea
                        ///Data Table first row CurrentStatus gave null value, hence it occures an error. Because of that check the column and delete that row, 
                        /// [30] means null coloumn number

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();

                        /// Check the data table has row or not
                        if (AllowedCriteria == null)
                        {
                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                            if (rows.Any())
                            {
                                dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                         || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                         || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                            }

                        }
                        else
                        {
                            if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                            {
                                var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                if (rows.Any())
                                {
                                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                          (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") ==
                                          (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                                }
                                else
                                {

                                }

                            }
                            else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                            {
                                var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                      (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).Count();

                                if (Count != 0)
                                {
                                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                              (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).CopyToDataTable();
                                }
                            }
                            else
                            {
                                if (RoleId != (int)POR.Enum.UserRole.ACCOUNTS01)
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                     (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                     x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                                    }
                                }
                                else
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                     (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                     x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

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
        public void InserFlowStatus(int InOut_ID, int RoleId, int UID_, int? FMSID, int? RSID)
        {
            ///Created BY   : Fg off RGSD Gamage
            ///Created Date : 2021/03.05
            /// Description : Insert living in out details stutus to flow status table

            try
            {
                FlowStatusLivingInOutDetail objFlowStatusDetail = new FlowStatusLivingInOutDetail();
                // LivingInOutDetail objLivingInOutDetail = new LivingInOutDetail();


                objFlowStatusDetail.InOut_ID = InOut_ID;
                objFlowStatusDetail.RSID = RSID;
                objFlowStatusDetail.UID = UID_;
                objFlowStatusDetail.FMSID = FMSID;
                objFlowStatusDetail.RID = RoleId;
                objFlowStatusDetail.CreatedBy = UID_;
                objFlowStatusDetail.CreatedMac = MacAddress;
                objFlowStatusDetail.IPAddress = this.Request.UserHostAddress;
                objFlowStatusDetail.CreatedDate = DateTime.Now;
                objFlowStatusDetail.Active = 1;

                _db.FlowStatusLivingInOutDetails.Add(objFlowStatusDetail);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int? PreviousFlowStatusId(int? InOut_ID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2023.02.09
            ///Des: get the reject flow status id FMSID

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFADFID = _db.FlowStatusLivingInOutDetails.Where(x => x.InOut_ID == InOut_ID).Select(x => x.LFSID).Max();
                int? CurrentFMSID = _db.FlowStatusLivingInOutDetails.Where(x => x.LFSID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);

                //FADID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).First();
                }
                else
                {
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO || CurrentUserRole == (int)POR.Enum.UserRole.P2OIC)
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
        public int ChecktheValidation(string Authority, string FromDate, string ToDate, string WEFDate)
        {
            ///Created BY   : FLT LT WAKY Wickramasinghe
            ///Created Date : 2022/02/22
            /// Description : when creat the record, check the validation
            /// status = 0 - input fields not fill
            /// status = 1 - NoProblem 
            /// status = 2 - Todate exceed the 6 month of period
            int status = 0;
            _LivingInOut obj = new _LivingInOut();
            if (Authority != null && FromDate != "" && ToDate != "")
            {
                DateTime ConFromDate = Convert.ToDateTime(FromDate);
                DateTime ConToDate = Convert.ToDateTime(ToDate);
                obj.FromSixMonth = ConFromDate.AddMonths(+6);

                if (ConToDate > obj.FromSixMonth)
                {
                   status = 2;
                }
                else
                {
                    status = 1;
                }
                
            }
            if (Authority != null && WEFDate != "")
            {
                status = 1;
            }

            return status;

        }
        public List<PORRecordCount> PorNoCreate(string EstablishmentId)
        {
            ///Created BY   : Fg Off RGSD Gamage
            ///Created Date : 2021/02/25
            /// Description : create POR Number 

            try
            {
                List<PORRecordCount> ListRecordList = new List<PORRecordCount>();

                int currentmonth = Convert.ToInt32(DateTime.Now.Month);
                int currentyear = Convert.ToInt32(DateTime.Now.Year);
                int currentdate = Convert.ToInt32(DateTime.Now.Day);

                int jobcount = objDALCommanQueryP2.LivingInOutNextRecordId(EstablishmentId,currentyear);


                //int jobcount = _db.LivingInOutHeaders.Where(x => x.Location == EstablishmentId && x.CreatedDate.Value.Year == currentyear && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer))).Max(x=>x.RecordCount).Value;
                int RocordId = jobcount + 1;

                string RefNo = EstablishmentId + "/" + "OK-1" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;

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
        [HttpGet]
        public ActionResult IndividualSearchLivingInOutP2(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ///Created BY   : 03558 WAKY Wickramasinghe
            ///Created Date : 2023/02/14
            /// Description : Search details for Individual Search

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            long sno = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_LivingInOut> LivingInOutList = new List<_LivingInOut>();


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
                dt = objDALCommanQuery.CallLivingINOutSP(sno);
                //dt = objDALCommanQuery.CallGSQSP();

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _LivingInOut obj_LivingInOut = new _LivingInOut();


                        obj_LivingInOut.LIOHID = Convert.ToInt32(dt2.Rows[i]["LIOHID"]);
                        obj_LivingInOut.Snumber = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_LivingInOut.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_LivingInOut.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_LivingInOut.Location = dt2.Rows[i]["Location"].ToString();
                        obj_LivingInOut.CategoryName = dt2.Rows[i]["LivingStatusName"].ToString();
                        obj_LivingInOut.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_LivingInOut.UserRole = dt2.Rows[i]["UserRoleName"].ToString();
                        //obj_LivingInOut.CurrentUserRole = UserInfo.RoleId;
                        LivingInOutList.Add(obj_LivingInOut);
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
            return View(LivingInOutList.ToPagedList(pageNumber, pageSize));

        }
        [HttpGet]
        public ActionResult IndividuaLivingDetailsP2(int id)
        {
            ///Created BY   : 03558 Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/07/27
            /// Description : Respective user can see the records current status afte click the detail button

            int UID_ = 0;           
           
            //For popup box
            TempData["CivilStatusHeaderID"] = id;

            if (Session["UID"].ToString() != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                var userInfo = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => new {x.LocationId ,x.RoleId}).FirstOrDefault();
                
                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == userInfo.LocationId).ToList();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                TempData["UserRoleId"] = userInfo.RoleId;

                var LivingINOut = _db.Vw_CurrentUserLiving.Where(x => x.InOut_ID == id);
                int? CurrentStatus = LivingINOut.Select(x => x.CurrentStatus).First();
                TempData["CurrentStatus"] = CurrentStatus;
                int? SubmitStatus = LivingINOut.Select(x => x.SubmitStatus).First();
                TempData["SubmitStatus"] = SubmitStatus;
                return View(LivingINOut.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public ActionResult AdvancedSearchLivingInP2(string FromDate, string ToDate, string SearchLoc, string SearchLivingStatus, string RecordStatus, int? page, string currentFilterFDate, string currentFilterTDate, string currentFilterLocation, string currentFilterLivingStatus, string currentFilterRecStatus)
        {
            ///Created By   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date :2023-06-12
            ///Des: Searching Option for history details

            DataTable dt = new DataTable();
            List<_LivingInOut> LivingInOutList = new List<_LivingInOut>();
            int LSID;
            int recordType;

            if (RecordStatus == null && page == null)
            {
                ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                ViewBag.DDL_InOutCategories = new SelectList(_db.LivingStatus, "LSID", "LivingStatusName");

                TempData["ErrMsg"] = "Please selecct the Record Status.";
            }
            else
            {
                recordType = Convert.ToInt32(RecordStatus);

                if (SearchLivingStatus == "")
                {
                    LSID = 0;
                }
                else
                {
                    LSID = Convert.ToInt32(SearchLivingStatus);
                }

                if (SearchLivingStatus != null)
                {
                    page = 1;
                    ViewBag.currentFilterFDate = FromDate;
                    ViewBag.currentFilterTDate = ToDate;
                    ViewBag.currentFilterLocation = SearchLoc;
                    ViewBag.currentFilterCategory = SearchLivingStatus;
                    ViewBag.currentFilterRecStatus = recordType;

                }
                else
                {
                    ViewBag.currentFilterFDate = currentFilterFDate;
                    ViewBag.currentFilterTDate = currentFilterTDate;
                    ViewBag.currentFilterLocation = currentFilterLocation;
                    ViewBag.currentFilterCategory = currentFilterLivingStatus;
                    ViewBag.currentFilterRecStatus = currentFilterRecStatus;
                }

                try
                {
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                    ViewBag.DDL_InOutCategories = new SelectList(_db.LivingStatus, "LSID", "LivingStatusName");

                    if (page != 1)
                    {
                        ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                        if (currentFilterFDate != null && currentFilterTDate != null && currentFilterLocation != null && currentFilterLivingStatus != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            LSID = Convert.ToInt32(currentFilterLivingStatus);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                        }
                        else if (currentFilterFDate != null && currentFilterTDate != null && currentFilterLivingStatus != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            LSID = Convert.ToInt32(currentFilterLivingStatus);
                            SearchLoc = "";
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                        }
                        else if (currentFilterFDate != null && currentFilterLocation != null && currentFilterLivingStatus != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = "";
                            SearchLoc = currentFilterLocation;
                            LSID = Convert.ToInt32(currentFilterLivingStatus);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                        }
                        else if (currentFilterTDate != null && currentFilterLocation != null && currentFilterLivingStatus != null && currentFilterRecStatus != null)
                        {
                            FromDate = "";
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            LSID = Convert.ToInt32(currentFilterLivingStatus);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (FromDate == null && ToDate == null && SearchLoc == null && SearchLivingStatus == null && recordType == 0)
                        {
                            TempData["ErrMsg"] = "Please selecct any search criteria.";
                        }
                        else
                        {
                            ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                            //ETWCID = Convert.ToInt32(SearchWorkshopSection);
                            if (FromDate != "" && ToDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                            {
                                dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                            }
                            else if (FromDate != "" && ToDate != "" && LSID != 0 && recordType != 0)
                            {
                                SearchLoc = "";
                                dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                            }
                            else if (FromDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                            {
                                ToDate = "";
                                dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
                            }
                            else if (ToDate != "" && SearchLoc != "" && LSID != 0 && recordType != 0)
                            {
                                FromDate = "";
                                dt = objDALCommanQuery.getLivingStatusSearchDetails(FromDate, ToDate, SearchLoc, LSID, recordType);
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
                        _LivingInOut obj_LivingInOut = new _LivingInOut();
                        obj_LivingInOut.Snumber = dt.Rows[i]["ServiceNo"].ToString();
                        obj_LivingInOut.Rank = dt.Rows[i]["Rank"].ToString();
                        obj_LivingInOut.FullName = dt.Rows[i]["Name"].ToString();
                        obj_LivingInOut.CategoryName = dt.Rows[i]["LivingStatusShortName"].ToString();
                        obj_LivingInOut.Authority = dt.Rows[i]["Authority"].ToString();
                        obj_LivingInOut.Location = dt.Rows[i]["Location"].ToString();
                        obj_LivingInOut.FromDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"]);
                        obj_LivingInOut.UserRole = dt.Rows[i]["RoleName"].ToString();
                        obj_LivingInOut.LIOHID = Convert.ToInt32(dt.Rows[i]["InOut_ID"]);
                        LivingInOutList.Add(obj_LivingInOut);

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
                return View(LivingInOutList.ToPagedList(pageNumber, pageSize));
            }
            return View();

        }

        [HttpGet]
        public ActionResult PrintData(int LIOHID)
        {
            ///Create By: Sqn ldr WAKY Wickramasinghe
            ///Create Date: 05/03/2023  
            ///Description: F121 Report view OCPS to Next level

            Session["LIOHID"] = LIOHID;

            return View();

        }
        public ActionResult ClearFileds(_LivingInOut obj_LivingInOut)
        {
            /// Clear view text box fields.

            ModelState.Clear();
            return RedirectToAction("CreateBechelorsInOut", "P2LivingInOut");
        }

        #region Json Methods
        [HttpPost]
        public JsonResult Index_Reject(string id, int LIOHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/02/13
            /// Description : this function is to reject the record

            string message = "";
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                if (id != "")
                {
                    var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
                    string RecordEstablishmentId = _db.LivingInOutHeaders.Where(x => x.LIOHID == LIOHID).Select(x => x.Location).FirstOrDefault();

                    //Method use for get FMSID
                    int? PreviousFMSID = PreviousFlowStatusId(LIOHID, userInfo.LocationId, RecordEstablishmentId, userInfo.DivisionId);
                    //Get Next FlowStatus User Role Name for Add Successfull Msg

                    int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == userInfo.LocationId)).Select(x => x.UserRoleID).FirstOrDefault();

                    PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                    FlowStatusLivingInOutDetail objFlowStatusLivingInOut = new FlowStatusLivingInOutDetail();
                    FlowStatusCivilStatusDetail objFlowStatusCivilStatus = new FlowStatusCivilStatusDetail();
                    objFlowStatusLivingInOut.InOut_ID = LIOHID;
                    objFlowStatusLivingInOut.FMSID = PreviousFMSID;
                    objFlowStatusLivingInOut.CreatedBy = UID;
                    objFlowStatusLivingInOut.UID = UID;

                    //Record Status Releted to RecordStatus Table
                    //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//
                    objFlowStatusLivingInOut.RSID = (int)POR.Enum.RecordStatus.Reject;
                    objFlowStatusLivingInOut.Comment = id;
                    objFlowStatusLivingInOut.CreatedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objFlowStatusLivingInOut.CreatedMac = MacAddress;
                    objFlowStatusLivingInOut.Active = 1;
                    objFlowStatusLivingInOut.IPAddress = this.Request.UserHostAddress;
                    _db.FlowStatusLivingInOutDetails.Add(objFlowStatusLivingInOut);

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
                    message = "Please Enter the reject Reason.";
                }              

            }
            else
            {
                // return RedirectToAction("Login", "User");
            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }
        public JsonResult getname(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2023/02/07
            /// Description : Load Full name details according to the service number

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
        public JsonResult HrmisNokDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Description  : get the hrmis include NOK Info

            Vw_NokDetailsP2 objNokDetailsP2 = new Vw_NokDetailsP2();
            var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            var S_no = Convert.ToInt64(SNo);

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
        public JsonResult PORNokDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/09/06
            /// Description : get the POR system Include NOK info, this may in pending approval stage

            Vw_NOK_Details objNOK = new Vw_NOK_Details();
            var NOKList = new List<NOKChangeDetail>();
            long s_no = 0;
            var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => new {x.SNo,x.service_type }).FirstOrDefault();

            if (ServiceInfo != null) s_no = Convert.ToInt64(ServiceInfo.SNo);              
            
            try
            {
                var Nok = from N in _db.NOKChangeHeaders
                          join Nc in _db.NOKChangeDetails on N.NOKCHID equals Nc.NOKChangeHeadrerID
                          where N.Sno == s_no
                          select new
                          {
                              Nc.NOKName,
                              Nc.NOKAddress,
                              Nc.NOKChangeTo
                          };
                foreach (var item in Nok)
                {
                    NOKList.Add(new NOKChangeDetail()
                    {
                        NOKName = item.NOKName,
                        NOKAddress = item.NOKAddress,
                        NOKChangeTo = item.NOKChangeTo

                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(NOKList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getMarriageDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wicramasinghr
            ///Created Date : 2021/09/05
            /// Description : get Marriage person name and load it to Spouse Name Text Box

            _LivingInOut obj_LivingInOut = new _LivingInOut();
            try
            {
                var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
                var Servicetype = _db.Vw_PersonalDetail.Where(x => x.SNo == SNo).Select(x => x.service_type).FirstOrDefault();

                if (Servicetype == (int)POR.Enum.ServiceType.RegAirmen || Servicetype == (int)POR.Enum.ServiceType.RegAirWomen || Servicetype == (int)POR.Enum.ServiceType.VolAirmen || Servicetype == (int)POR.Enum.ServiceType.VolAirWomen)
                {
                    var SpouseName = _dbP3HRMS.Marriages.Where(x => x.SNo == SNo).Select(x => x.SpouseName).FirstOrDefault();
                    if (SpouseName == null)
                    {
                        obj_LivingInOut.SpouseName = "No records to found in HRMIS and Please Contact the Cmd P3 Section";
                    }
                    else
                    {
                        obj_LivingInOut.SpouseName = SpouseName;
                    }

                }
                //else
                //{
                //    obj_LivingInOut.SpouseName = "Please Enter Name Manualy";
                //}

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(obj_LivingInOut, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMarriedDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/10/29
            /// Description : this function check person married or not, for following category M/L/IN, M/L/OUT ,M/R/OUT
            int IsMarried = 0;
            try
            {
                var serviceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => new {x.SNo,x.service_type }).FirstOrDefault();
                long SNO = Convert.ToInt64(serviceInfo.SNo);

                var marriedInfoPOR = _db.CivilStatusHeaders.Where(x => x.Sno == SNO && x.SubCategoryId == (int)POR.Enum.CivilStatusCategory.Marriage && x.Active == 1).Count();

                if (serviceInfo.service_type == 1001 || serviceInfo.service_type == 1002 || serviceInfo.service_type == 1003 || serviceInfo.service_type == 1004)
                {
                    var marriedInfoP2HRMS = _db.Vw_P2Marriage.Where(x => x.SNo == serviceInfo.SNo).Count();

                    if (marriedInfoPOR != 0 || marriedInfoP2HRMS != 0)
                    {
                        IsMarried = 1;
                        return Json(IsMarried, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(IsMarried, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    
                    var marriedInfoHRMS = _dbP3HRMS.Marriages.Where(x => x.SNo == serviceInfo.SNo).Count();

                    if (marriedInfoPOR != 0 || marriedInfoHRMS != 0)
                    {
                        IsMarried = 1;
                        return Json(IsMarried, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(IsMarried, JsonRequestBehavior.AllowGet);
                    }
                }               

            }
            catch (Exception ex)
            {

                throw ex;
            }

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

        #endregion

    }
}