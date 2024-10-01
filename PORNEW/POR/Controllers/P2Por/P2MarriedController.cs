using PagedList;
using POR.Models;
using POR.Models.Married;
using ReportData.BAL;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers.P2Por
{
    public class P2MarriedController : Controller
    {
        dbContext _db = new dbContext();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        P3HRMS _dbP3HRMS = new P3HRMS();
        string MacAddress = new DALBase().GetMacAddress();
        int RoleId = 0;
        // GET: Married
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/07/19
            /// Description : Index Page for Forward CiviStatus POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_CivilStatus> civilStatusList = new List<_CivilStatus>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x=> new { x.LocationId,x.RoleId}).FirstOrDefault();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                //ViewBag.DateSortParm = sortOrder == "Sno" ? "Rank" : "CreateDate";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = RoleId;
                long sno = 0;

                if (searchString != null)
                {
                    page = 1;                   

                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                    sno = Convert.ToInt64(Sno);
                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallSP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).CopyToDataTable();
                    }                   

                }
                else
                {
                    searchString = currentFilter;

                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallSP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("RecordStatusID") == 2000 || x.Field<int>("RecordStatusID") == 1000).CopyToDataTable();

                    }                    
                }

                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P3SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.P3OIC:
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
                    case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.HRMSSNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID);
                        break;
                    case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
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
                    _CivilStatus obj_CivilStatus = new _CivilStatus();
                    obj_CivilStatus.CSHID = Convert.ToInt32(dt3.Rows[i]["CSHID"]);
                    obj_CivilStatus.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_CivilStatus.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_CivilStatus.Name = dt3.Rows[i]["Name"].ToString();
                    obj_CivilStatus.Location = dt3.Rows[i]["Location"].ToString();
                    obj_CivilStatus.CategoryName = dt3.Rows[i]["CategoryName"].ToString();
                    obj_CivilStatus.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    obj_CivilStatus.CurrentUserRole = UserInfo.RoleId;
                    civilStatusList.Add(obj_CivilStatus);

                }

                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(civilStatusList.ToPagedList(pageNumber, pageSize));
            }
            return View();
        }
        public ActionResult Create()
        {
            /// Create By: Flt lt Wickramasinghe
            /// Create Date: 25/05/2021  
            /// Description: por clerk initial step stage of crate married,divorce,widow por

            try
            {
                ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                //ViewBag.DDL_Town_Result = new SelectList(_dbCommonData.P2_Town, "TownCOde", "Town");
                //ViewBag.DDL_Postoffice = new SelectList(_dbCommonData.PostOffices.OrderBy(x => x.PostOfficeName), "PostOfficeName", "PostOfficeName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");

                ViewBag.DDL_CivilStatusCat = new SelectList(_db.CivilStatusCategories.Where(x => x.Active == 1), "CSCID", "CategoryName");
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(_CivilStatus obj_CivilStatus)
        {
            ///Create By:   Flt lt Wickramasinghe
            ///Create Date: 28/05/2021  
            ///Description: Insert data to CivilStatusHeader,CivilStatusSpouseDetails table. Civil Status details table insert data 
            ///             depend on user select civil subcategory

            CivilStatusHeader objCivilStatusHeader = new CivilStatusHeader();
            CivilStatusMarriedDetail objCivilStatusMarriedDetail = new CivilStatusMarriedDetail();
            CivilStatusDivorceDetail objCivilStatusDivorceDetail = new CivilStatusDivorceDetail();
            CivilStatusWidowDetail objCivilStatusWidowDetail = new CivilStatusWidowDetail();
            CivilStatusSpouseDetail objCivilStatusSpouseDetail = new CivilStatusSpouseDetail();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            List<PORRecordCount> proRecorlist = new List<PORRecordCount>();

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            obj_CivilStatus.RSID = 1000;

            try
            {
               
                int UID_ = 0;

                ViewBag.DDL_CivilStatusCat = new SelectList(_db.CivilStatusCategories.Where(x => x.Active == 1), "CSCID", "CategoryName");
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_Provincial_Result = new SelectList(_dbCommonData.ProvinceNews.OrderBy(x => x.PROV_CODE), "PROV_CODE", "DESCRIPTION");
                ViewBag.DDL_GSDivisionSelectAll_Result = new SelectList(_dbCommonData.P2_GSDivision.OrderBy(x => x.GSCode), "GSName", "GSName");
                ViewBag.DDL_PoliceStation = new SelectList(_dbCommonData.P2_PoliceStation, "PoliceStation", "PoliceStation");
                /// Get the service type related to service number.
                var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_CivilStatus.ServiceNo).Select(x => new { x.service_type,x.SNo} ).FirstOrDefault();

               long ConSNo = Convert.ToInt64(ServiceInfo.SNo);

                ///Get District name
                var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_CivilStatus.District).Select(x => x.DESCRIPTION).FirstOrDefault();

                if (Session["UID"] != null)
                {
                    UID_ = Convert.ToInt32(Session["UID"]);

                    //Get the user login Establishment
                    var UserInfo = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => new { x.LocationId ,x.RoleId}).FirstOrDefault();

                    proRecorlist = PorNoCreate(UserInfo.LocationId, obj_CivilStatus.CSCID);
                    string CreatePorNo = proRecorlist[0].RefNo;


                    if (ConSNo != 0 && obj_CivilStatus.CSCID != 0 && obj_CivilStatus.SpouseName != null && obj_CivilStatus.NOKName != null &&
                        obj_CivilStatus.NOKaddress != null && DistName != null && obj_CivilStatus.GSName != null && obj_CivilStatus.Town1 != null && obj_CivilStatus.PoliceStation1 != null)
                    {

                        var checktheDuplicate = _db.CivilStatusHeaders.Where(x => x.Sno == ConSNo && x.SubCategoryId == obj_CivilStatus.CSCID && x.Active == 1).Count();
                        if (checktheDuplicate == 0)
                        {
                            /// Enter Civil Status Header details to table
                            objCivilStatusHeader.Sno = ConSNo;
                            objCivilStatusHeader.SubCategoryId = obj_CivilStatus.CSCID;
                            objCivilStatusHeader.ServiceTypeId = ServiceInfo.service_type;
                            objCivilStatusHeader.Location = UserInfo.LocationId;
                            objCivilStatusHeader.RefNo = CreatePorNo;
                            objCivilStatusHeader.Authority = obj_CivilStatus.Authority;
                            objCivilStatusHeader.RecordCount = Convert.ToInt32(proRecorlist[0].RecordCount);
                            objCivilStatusHeader.CreatedBy = UID_;
                            objCivilStatusHeader.CreatedDate = DateTime.Now;
                            objCivilStatusHeader.Active = 1;

                            objCivilStatusHeader.CreatedMac = MacAddress;
                            objCivilStatusHeader.IPAddress = this.Request.UserHostAddress;

                            _db.CivilStatusHeaders.Add(objCivilStatusHeader);
                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                if (_db.SaveChanges() > 0)
                                {
                                    //var CSHID =_db.CivilStatusHeaders.Where(x=>x.Sno == obj_CivilStatus.Snumber && x.SubCategoryId == obj_CivilStatus.CSCID && x.Active == 1 
                                    //          && x.Location == EstablishmentId).OrderByDescending(x => x.CreatedDate).Select(x => x.CSHID).FirstOrDefault();

                                    var CSHID = _db.CivilStatusHeaders.Where(x => x.Active == 1 && x.RefNo == CreatePorNo && x.Sno == ConSNo && x.SubCategoryId == obj_CivilStatus.CSCID).OrderByDescending(x => x.CreatedDate).Select(x => x.CSHID).FirstOrDefault();

                                    /// Insert details to CivilStatusSpouseDetail table 
                                    objCivilStatusSpouseDetail.CivilStatusHeaderID = CSHID;
                                    objCivilStatusSpouseDetail.SpouseName = obj_CivilStatus.SpouseName;
                                    objCivilStatusSpouseDetail.CreatedBy = UID_;
                                    objCivilStatusSpouseDetail.CreatedDate = DateTime.Now;
                                    objCivilStatusSpouseDetail.CreatedBy = UID_;
                                    objCivilStatusSpouseDetail.CreatedMac = MacAddress;
                                    objCivilStatusSpouseDetail.CreateIpAddess = this.Request.UserHostAddress;
                                    objCivilStatusSpouseDetail.Active = 1;

                                    _db.CivilStatusSpouseDetails.Add(objCivilStatusSpouseDetail);

                                    switch (obj_CivilStatus.CSCID)
                                    {
                                        /// if sub Catgeory Married, Insert Married details to CivilStatusMarriedDetail
                                        case (int)POR.Enum.CivilStatusCategory.Marriage:
                                            objCivilStatusMarriedDetail.CivilStatusHeaderID = CSHID;
                                            objCivilStatusMarriedDetail.MarriageDate = Convert.ToDateTime(obj_CivilStatus.MarriageDate);
                                            objCivilStatusMarriedDetail.MarriageCertificateNo = obj_CivilStatus.MarriageCertificateNo;
                                            objCivilStatusMarriedDetail.RegistarOfficeLocation = obj_CivilStatus.RegistarOfficeLocation;
                                            objCivilStatusMarriedDetail.MarriageDate = obj_CivilStatus.MarriageDate;
                                            objCivilStatusMarriedDetail.WithEffectDate = Convert.ToDateTime(obj_CivilStatus.WEFDate);
                                            objCivilStatusMarriedDetail.CreatedBy = UID_;
                                            objCivilStatusMarriedDetail.CreatedDate = DateTime.Now;
                                            objCivilStatusMarriedDetail.CreatedMac = MacAddress;
                                            objCivilStatusMarriedDetail.CreateIpAddess = this.Request.UserHostAddress;
                                            objCivilStatusMarriedDetail.Active = 1;

                                            _db.CivilStatusMarriedDetails.Add(objCivilStatusMarriedDetail);
                                            break;

                                        /// if sub Catgeory Divorce, Insert Married details to CivilStatusDivorceDetail
                                        case (int)POR.Enum.CivilStatusCategory.Divorce:
                                            objCivilStatusDivorceDetail.CivilStatusHeaderID = CSHID;
                                            objCivilStatusDivorceDetail.DivorceDtae = Convert.ToDateTime(obj_CivilStatus.DivorceDate);
                                            objCivilStatusDivorceDetail.Place = obj_CivilStatus.DivorceLocation;
                                            objCivilStatusDivorceDetail.CourseCaseNo = obj_CivilStatus.CourtCaseNo;
                                            objCivilStatusDivorceDetail.DateOfCase = Convert.ToDateTime(obj_CivilStatus.DivorceCaseDate);
                                            objCivilStatusDivorceDetail.CreatedBy = UID_;
                                            objCivilStatusDivorceDetail.CreatedDate = DateTime.Now;
                                            objCivilStatusDivorceDetail.CreatedMac = MacAddress;
                                            objCivilStatusDivorceDetail.CreateIpAddess = this.Request.UserHostAddress;
                                            objCivilStatusDivorceDetail.Active = 1;

                                            _db.CivilStatusDivorceDetails.Add(objCivilStatusDivorceDetail);

                                            break;

                                        /// if sub Catgeory Widow, Insert Married details to CivilStatusWidowDetail
                                        case (int)POR.Enum.CivilStatusCategory.Widow:
                                            objCivilStatusWidowDetail.CivilStatusHeaderID = CSHID;
                                            objCivilStatusWidowDetail.DeathCertificateNoOfSpouse = obj_CivilStatus.DeathCertificateNo;
                                            objCivilStatusWidowDetail.DateofDecease = Convert.ToDateTime(obj_CivilStatus.DateofDecease);
                                            objCivilStatusWidowDetail.CreatedBy = UID_;
                                            objCivilStatusWidowDetail.CreatedDate = DateTime.Now;
                                            objCivilStatusWidowDetail.CreatedMac = MacAddress;
                                            objCivilStatusWidowDetail.CreateIpAddess = this.Request.UserHostAddress;
                                            objCivilStatusWidowDetail.Active = 1;

                                            _db.CivilStatusWidowDetails.Add(objCivilStatusWidowDetail);
                                            break;
                                        default:
                                            break;
                                    }

                                    /// Enetr NOK header details to NOKChangeHeaders

                                    var NokStatus = _db.NOKStatus.Where(x => x.SubCatID == obj_CivilStatus.CSCID && x.Active == 1).Select(x => x.NOKSID).FirstOrDefault();

                                    objNOKChangeHeader.CivilStatusHeaderID = CSHID;
                                    objNOKChangeHeader.NOKStatus = NokStatus;
                                    objNOKChangeHeader.Sno = ConSNo;
                                    objNOKChangeHeader.Location =UserInfo.LocationId;
                                    objNOKChangeHeader.ServiceTypeId = ServiceInfo.service_type;
                                    objNOKChangeHeader.WFDate = Convert.ToDateTime(obj_CivilStatus.NOKWDate);
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
                                                      x.Location == UserInfo.LocationId && x.NOKStatus == NokStatus && x.Active == 1 && x.CivilStatusHeaderID == CSHID).OrderByDescending(x => x.CreatedDate).Select(x => x.NOKCHID).FirstOrDefault();


                                        objNOKChangeDetail.NOKChangeHeadrerID = NOKCHID;
                                        objNOKChangeDetail.NOKAddress = obj_CivilStatus.NOKaddress;
                                        objNOKChangeDetail.NOKName = obj_CivilStatus.NOKName;
                                        objNOKChangeDetail.NOKChangeTo = obj_CivilStatus.RelationshipName;
                                        objNOKChangeDetail.District = DistName;
                                        objNOKChangeDetail.GSDivision = obj_CivilStatus.GSName;
                                        objNOKChangeDetail.NearestTown = obj_CivilStatus.Town1;
                                        objNOKChangeDetail.PoliceStation = obj_CivilStatus.PoliceStation1;
                                        objNOKChangeDetail.PostOffice = obj_CivilStatus.PostOfficeName;
                                        objNOKChangeDetail.Remarks = obj_CivilStatus.Remarks;
                                        objNOKChangeDetail.CreatedBy = UID_;
                                        objNOKChangeDetail.CreatedDate = DateTime.Now;
                                        objNOKChangeDetail.CreatedMac = MacAddress;
                                        objNOKChangeDetail.CreateIpAddess = this.Request.UserHostAddress;
                                        objNOKChangeDetail.Active = 1;

                                        _db.NOKChangeDetails.Add(objNOKChangeDetail);

                                        if (_db.SaveChanges() > 0)
                                        {
                                            ////Insert First Flow Mgt record to FlowStatusCivilStatusDetails table
                                            InsertFlowStatus(CSHID, RoleId, UID_, obj_CivilStatus.FMSID, obj_CivilStatus.RSID);
                                            scope.Complete();
                                            TempData["ScfMsg"] = "Complete Your Process";
                                        }
                                        else
                                        {
                                            scope.Dispose();
                                            TempData["ErrMsg"] = " Not Complete Your Process";
                                        }
                                    }
                                }
                            }
                            return RedirectToAction("Create", "Married");
                        }
                        else
                        {
                            TempData["ErrMsg"] = "Record already exist.";
                        }

                    }
                    else
                    {
                        TempData["ErrMsg"] = "Please Fill all the details.";
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
            return View();
        }
        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created BY   : Sqn Ldr Wickramasinghe
            ///Created Date : 2023/07/20
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
                        
                        var row = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                        if (row.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                  || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                        }

                        break;
                    #endregion

                    default:

                        #region CodeArea                                               

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();

                        if (AllowedCriteria == null)
                        {
                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward 
                                       && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                       || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                            if (rows.Any())
                            {
                                dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == 
                                       (int)POR.Enum.RecordStatus.Forward) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || 
                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                         || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                         || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                            }
                        }
                        else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                        {
                            var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).Count();

                            if (Count != 0)
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
                                var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                           (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                           x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer ||
                                           x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                if (Count != 0)
                                {
                                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                              (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || 
                                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                                }
                            }
                            else
                            {
                                var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                            (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || 
                                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                if (Count != 0)
                                {
                                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                              (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || 
                                                                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

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
        [HttpGet]
        public ActionResult Details(int CSHID, int Rejectstatus)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/07/20
            /// Description : Details related Civil Status por 

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;               
                int? CurrentStatusUserRole;
                string CivilStatusCode;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_CivilStatus> civilStatusList = new List<_CivilStatus>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID_).Select(x =>  new { x.RoleId ,x.LocationId} ).First();
                //UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = userInfo.RoleId;
               

                var CurrentStatus_UserRole = (from f in _db.FlowStatusCivilStatusDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.CivilStatusHeaderID == CSHID
                                              orderby f.CivilStatusHeaderID descending
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
                dt = objDALCommanQuery.CallSP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("CSHID") == CSHID).CopyToDataTable();

                /// This Rejectstatus value assign from after clicking RejectIndex Details button. value 2 mean reject status
                if (Rejectstatus != 2)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        /// Check the rercord is previously reject or not
                        var prvReject = _db.CivilStatusHeaders.Where(x => x.CSHID == CSHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        _CivilStatus obj_CivilStatus = new _CivilStatus();
                        obj_CivilStatus.CSHID = Convert.ToInt32(dt2.Rows[i]["CSHID"]);
                        obj_CivilStatus.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_CivilStatus.Snumber = Convert.ToInt64(dt2.Rows[i]["Sno"]);
                        obj_CivilStatus.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_CivilStatus.Name = dt2.Rows[i]["Name"].ToString();
                        obj_CivilStatus.Location = dt2.Rows[i]["Location"].ToString();
                        obj_CivilStatus.CategoryName = dt2.Rows[i]["CategoryName"].ToString();
                        obj_CivilStatus.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_CivilStatus.SpouseName = dt2.Rows[i]["SpouseName"].ToString();
                        obj_CivilStatus.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_CivilStatus.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_CivilStatus.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_CivilStatus.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_CivilStatus.District1 = dt2.Rows[i]["District"].ToString();
                        obj_CivilStatus.GSnumber = dt2.Rows[i]["GSDivision"].ToString();
                        obj_CivilStatus.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_CivilStatus.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_CivilStatus.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_CivilStatus.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_CivilStatus.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);


                        if (prvReject == 1)
                        {
                            obj_CivilStatus.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_CivilStatus.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_CivilStatus.NOKWDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_CivilStatus.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        CivilStatusCode = dt2.Rows[i]["CategoryCode"].ToString();

                        TempData["CivilStatusCode"] = CivilStatusCode;

                        switch (CivilStatusCode)
                        {
                            case "AJ-1":
                                obj_CivilStatus.MarriageDate = Convert.ToDateTime(dt2.Rows[i]["MarriageDate"]);
                                obj_CivilStatus.RegistarOfficeLocation = dt2.Rows[i]["RegistarOfficeLocation"].ToString();
                                obj_CivilStatus.MarriageCertificateNo = dt2.Rows[i]["MarriageCertificateNo"].ToString();
                                obj_CivilStatus.WEFDate = Convert.ToDateTime(dt2.Rows[i]["MarriageWithEffectDate"]);
                                break;
                            case "AJ-2":
                            case "AJ-4":

                                obj_CivilStatus.DivorceDate = Convert.ToDateTime(dt2.Rows[i]["DivorceDtae"]);
                                obj_CivilStatus.DivorceLocation = dt2.Rows[i]["Place"].ToString();
                                obj_CivilStatus.CourtCaseNo = dt2.Rows[i]["CourseCaseNo"].ToString();
                                obj_CivilStatus.DivorceCaseDate = Convert.ToDateTime(dt2.Rows[i]["DateOfCase"]);
                                break;
                            case "AJ-3":
                                obj_CivilStatus.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNoOfSpouse"].ToString();
                                obj_CivilStatus.DateofDecease = Convert.ToDateTime(dt2.Rows[i]["DateofDecease"]);
                                break;
                            default:
                                break;
                        }
                        civilStatusList.Add(obj_CivilStatus);
                    }
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id
                    var RejectRoleId = _db.FlowStatusCivilStatusDetails.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.CivilStatusHeaderID == CSHID)
                                      .OrderByDescending(x => x.FSCSID).Select(x => x.RoleID).FirstOrDefault();

                   

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _CivilStatus obj_CivilStatus = new _CivilStatus();

                        obj_CivilStatus.CSHID = Convert.ToInt32(dt2.Rows[i]["CSHID"]);
                        obj_CivilStatus.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_CivilStatus.Snumber = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                        obj_CivilStatus.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_CivilStatus.Name = dt2.Rows[i]["Name"].ToString();
                        obj_CivilStatus.Location = dt2.Rows[i]["Location"].ToString();
                        obj_CivilStatus.CategoryName = dt2.Rows[i]["CategoryName"].ToString();
                        obj_CivilStatus.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_CivilStatus.SpouseName = dt2.Rows[i]["SpouseName"].ToString();
                        obj_CivilStatus.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_CivilStatus.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_CivilStatus.RejectRoleName = RoleName.ToString();
                        obj_CivilStatus.NOKName = dt2.Rows[i]["NOKName"].ToString();
                        obj_CivilStatus.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                        obj_CivilStatus.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                        obj_CivilStatus.District1 = dt2.Rows[i]["District"].ToString();
                        obj_CivilStatus.GSnumber = dt2.Rows[i]["GSDivision"].ToString();
                        obj_CivilStatus.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                        obj_CivilStatus.PoliceStation1 = dt2.Rows[i]["PoliceStation"].ToString();
                        obj_CivilStatus.PostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                        obj_CivilStatus.Remarks = dt2.Rows[i]["Remarks"].ToString();
                        obj_CivilStatus.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);

                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_CivilStatus.NOKWDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_CivilStatus.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        CivilStatusCode = dt2.Rows[i]["CategoryCode"].ToString();

                        TempData["CivilStatusCode"] = CivilStatusCode;

                        switch (CivilStatusCode)
                        {
                            case "AJ-1":
                                obj_CivilStatus.MarriageDate = Convert.ToDateTime(dt2.Rows[i]["MarriageDate"]);
                                obj_CivilStatus.RegistarOfficeLocation = dt2.Rows[i]["RegistarOfficeLocation"].ToString();
                                obj_CivilStatus.MarriageCertificateNo = dt2.Rows[i]["MarriageCertificateNo"].ToString();
                                obj_CivilStatus.WEFDate = Convert.ToDateTime(dt2.Rows[i]["MarriageWithEffectDate"]);
                                break;
                            case "AJ-2":
                            case "AJ-4":

                                obj_CivilStatus.DivorceDate = Convert.ToDateTime(dt2.Rows[i]["DivorceDtae"]);
                                obj_CivilStatus.DivorceLocation = dt2.Rows[i]["Place"].ToString();
                                obj_CivilStatus.CourtCaseNo = dt2.Rows[i]["CourseCaseNo"].ToString();
                                obj_CivilStatus.DivorceCaseDate = Convert.ToDateTime(dt2.Rows[i]["DateOfCase"]);
                                break;
                            case "AJ-3":
                                obj_CivilStatus.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNoOfSpouse"].ToString();
                                obj_CivilStatus.DateofDecease = Convert.ToDateTime(dt2.Rows[i]["DateofDecease"]);
                                break;
                            default:
                                break;
                        }
                        civilStatusList.Add(obj_CivilStatus);
                    }
                }                
                return View(civilStatusList);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }
        [HttpGet]
        public ActionResult PrintData(int CSHID)
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 15/05/2022  
            ///Description: F121 Report view OCPS to Next level

            Session["CSHID"] = CSHID;

            return View();

        }
        [HttpGet]
        public ActionResult Edit(int id, int rejectStatus)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 20/07/2023
            /// Description : Load Details to Clerks for the edit facility
            /// 

            ViewBag.DDL_CivilStatusCat = new SelectList(_db.CivilStatusCategories.Where(x => x.Active == 1), "CSCID", "CategoryName");
            ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
            ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
            ViewBag.DDL_Postoffice = new SelectList(_dbCommonData.PostOffices.OrderBy(x => x.PostOfficeName), "PostOfficeName", "PostOfficeName");

            if (Session["UID"] != null)
            {
                int CSHID = id;
                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;               
                int? CurrentStatusUserRole;
                string CivilStatusCode;
                string RejectRef;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_CivilStatus> civilStatusList = new List<_CivilStatus>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => new { x.RoleId,x.LocationId} ).First();                
                TempData["UserRoleId"] = userInfo.RoleId;               

                var CurrentStatus_UserRole = (from f in _db.FlowStatusCivilStatusDetails
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.CivilStatusHeaderID == CSHID
                                              orderby f.CivilStatusHeaderID descending
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
                dt = objDALCommanQuery.CallSP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CSHActive") == 1 && x.Field<int>("CSHID") == CSHID).CopyToDataTable();
                _CivilStatus obj_CivilStatus = new _CivilStatus();

                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {

                    obj_CivilStatus.CSHID = Convert.ToInt32(dt2.Rows[i]["CSHID"]);
                    obj_CivilStatus.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_CivilStatus.Snumber = Convert.ToInt64(dt2.Rows[i]["Sno"].ToString());
                    obj_CivilStatus.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_CivilStatus.Name = dt2.Rows[i]["Name"].ToString();
                    obj_CivilStatus.Location = dt2.Rows[i]["Location"].ToString();
                    obj_CivilStatus.CategoryName = dt2.Rows[i]["CategoryName"].ToString();
                    obj_CivilStatus.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_CivilStatus.SpouseName = dt2.Rows[i]["SpouseName"].ToString();
                    obj_CivilStatus.Authority = dt2.Rows[i]["Authority"].ToString();
                    obj_CivilStatus.NOKName = dt2.Rows[i]["NOKName"].ToString();
                    obj_CivilStatus.NOKChangeTo = dt2.Rows[i]["NOKChangeTo"].ToString();
                    obj_CivilStatus.NOKaddress = dt2.Rows[i]["NOKAddress"].ToString();
                    obj_CivilStatus.District1 = dt2.Rows[i]["District"].ToString();
                    obj_CivilStatus.GSnumber = dt2.Rows[i]["GSDivision"].ToString();
                    obj_CivilStatus.NearestTown = dt2.Rows[i]["NearestTown"].ToString();
                    obj_CivilStatus.EditPoliceStation = dt2.Rows[i]["PoliceStation"].ToString();
                    obj_CivilStatus.EditPostOfficeName = dt2.Rows[i]["PostOffice"].ToString();
                    obj_CivilStatus.Remarks = dt2.Rows[i]["Remarks"].ToString();
                    obj_CivilStatus.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);


                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.CivilStatusHeaders.Where(x => x.CSHID == CSHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_CivilStatus.RefNo + " " + " - Reject";
                            obj_CivilStatus.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_CivilStatus.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_CivilStatus.RejectRefNo = RejectRef;
                        }

                    }

                    if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                    {
                        obj_CivilStatus.NOKWDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                    }
                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_CivilStatus.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }

                    CivilStatusCode = dt2.Rows[i]["CategoryCode"].ToString();

                    TempData["CivilStatusCode"] = CivilStatusCode;

                    switch (CivilStatusCode)
                    {
                        case "AJ-1":
                            obj_CivilStatus.MarriageDate = Convert.ToDateTime(dt2.Rows[i]["MarriageDate"]);
                            obj_CivilStatus.RegistarOfficeLocation = dt2.Rows[i]["RegistarOfficeLocation"].ToString();
                            obj_CivilStatus.MarriageCertificateNo = dt2.Rows[i]["MarriageCertificateNo"].ToString();
                            obj_CivilStatus.WEFDate = Convert.ToDateTime(dt2.Rows[i]["MarriageWithEffectDate"]);
                            break;
                        case "AJ-2":
                        case "AJ-4":

                            obj_CivilStatus.DivorceDate = Convert.ToDateTime(dt2.Rows[i]["DivorceDtae"]);
                            obj_CivilStatus.DivorceLocation = dt2.Rows[i]["Place"].ToString();
                            obj_CivilStatus.CourtCaseNo = dt2.Rows[i]["CourseCaseNo"].ToString();
                            obj_CivilStatus.DivorceCaseDate = Convert.ToDateTime(dt2.Rows[i]["DateOfCase"]);
                            break;
                        case "AJ-3":
                            obj_CivilStatus.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNoOfSpouse"].ToString();
                            obj_CivilStatus.DateofDecease = Convert.ToDateTime(dt2.Rows[i]["DateofDecease"]);
                            break;
                        default:
                            break;
                    }
                }

                return View(obj_CivilStatus);
                //return View();
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }
        [HttpPost]
        public ActionResult Edit(_CivilStatus obj_CivilStatus, int rejectStatus)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 20/07/2023
            ///Description  : Edit details  Save in to Db

            try
            {
                CivilStatusHeader objCivilStatusHeader = new CivilStatusHeader();
                CivilStatusMarriedDetail objCivilStatusMarriedDetail = new CivilStatusMarriedDetail();
                CivilStatusDivorceDetail objCivilStatusDivorceDetail = new CivilStatusDivorceDetail();
                CivilStatusWidowDetail objCivilStatusWidowDetail = new CivilStatusWidowDetail();
                CivilStatusSpouseDetail objCivilStatusSpouseDetail = new CivilStatusSpouseDetail();
                FlowStatusCivilStatusDetail obj = new FlowStatusCivilStatusDetail();
                NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
                NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();

                /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
                obj_CivilStatus.RSID = (int)POR.Enum.RecordStatus.Insert;
                int UID_ = 0;

                ViewBag.DDL_CivilStatusCat = new SelectList(_db.CivilStatusCategories.Where(x => x.Active == 1), "CategoryName", "CategoryName");
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_Postoffice = new SelectList(_dbCommonData.PostOffices.OrderBy(x => x.PostOfficeName), "PostOfficeName", "PostOfficeName");

                ///Get District name
                var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_CivilStatus.District).Select(x => x.DESCRIPTION).FirstOrDefault();

                if (Session["UID"] != null)
                {

                    UID_ = Convert.ToInt32(Session["UID"]);

                    //Get the Civil Status Category
                    var CSCID = _db.CivilStatusCategories.Where(x => x.CategoryName == obj_CivilStatus.CategoryName && x.Active == 1).Select(x => new { x.CSCID, x.CategoryCode }).FirstOrDefault();
                    TempData["CivilStatusCode"] = CSCID.CategoryCode;

                    /// Update Civil Status Header details to table
                    objCivilStatusHeader = _db.CivilStatusHeaders.Find(obj_CivilStatus.CSHID);
                    objCivilStatusHeader.Authority = obj_CivilStatus.Authority;
                    objCivilStatusHeader.ModifiedBy = UID_;
                    objCivilStatusHeader.ModifiedDate = DateTime.Now;
                    objCivilStatusHeader.ModifiedMac = MacAddress;

                    _db.Entry(objCivilStatusHeader).State = EntityState.Modified;
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (_db.SaveChanges() > 0)
                        {
                            /// Get SpouseDetail table id 
                            var CSSPDID = _db.CivilStatusSpouseDetails.Where(x => x.Active == 1 && x.CivilStatusHeaderID == obj_CivilStatus.CSHID).Select(x => x.CSSPDID).FirstOrDefault();
                            objCivilStatusSpouseDetail = _db.CivilStatusSpouseDetails.Find(CSSPDID);
                            objCivilStatusSpouseDetail.SpouseName = obj_CivilStatus.SpouseName;
                            objCivilStatusSpouseDetail.ModifiedBy = UID_;
                            objCivilStatusSpouseDetail.ModifiedDate = DateTime.Now;
                            objCivilStatusSpouseDetail.ModifiedMac = MacAddress;

                            _db.Entry(objCivilStatusSpouseDetail).State = EntityState.Modified;

                            switch (CSCID.CSCID)
                            {
                                /// if sub Catgeory Married, Insert Married details to CivilStatusMarriedDetail
                                case (int)POR.Enum.CivilStatusCategory.Marriage:

                                    var MID = _db.CivilStatusMarriedDetails.Where(x => x.CivilStatusHeaderID == obj_CivilStatus.CSHID && x.Active == 1).Select(x => x.CSMDID).FirstOrDefault();

                                    objCivilStatusMarriedDetail = _db.CivilStatusMarriedDetails.Find(MID);
                                    objCivilStatusMarriedDetail.MarriageDate = obj_CivilStatus.MarriageDate;
                                    objCivilStatusMarriedDetail.MarriageCertificateNo = obj_CivilStatus.MarriageCertificateNo;
                                    objCivilStatusMarriedDetail.RegistarOfficeLocation = obj_CivilStatus.RegistarOfficeLocation;
                                    objCivilStatusMarriedDetail.WithEffectDate = obj_CivilStatus.WEFDate;
                                    objCivilStatusMarriedDetail.ModifiedBy = UID_;
                                    objCivilStatusMarriedDetail.ModifiedDate = DateTime.Now;
                                    objCivilStatusMarriedDetail.ModifiedMac = MacAddress;
                                    objCivilStatusMarriedDetail.Active = 1;

                                    _db.Entry(objCivilStatusMarriedDetail).State = EntityState.Modified;
                                    break;

                                /// if sub Catgeory Divorce, Insert Married details to CivilStatusDivorceDetail
                                case (int)POR.Enum.CivilStatusCategory.Divorce:
                                case (int)POR.Enum.CivilStatusCategory.NullVoid:

                                    var DID = _db.CivilStatusDivorceDetails.Where(x => x.CivilStatusHeaderID == obj_CivilStatus.CSHID && x.Active == 1).Select(x => x.CSDDID).FirstOrDefault();

                                    objCivilStatusDivorceDetail = _db.CivilStatusDivorceDetails.Find(DID);
                                    objCivilStatusDivorceDetail.DivorceDtae = obj_CivilStatus.DivorceDate;
                                    objCivilStatusDivorceDetail.Place = obj_CivilStatus.DivorceLocation;
                                    objCivilStatusDivorceDetail.CourseCaseNo = obj_CivilStatus.CourtCaseNo;
                                    objCivilStatusDivorceDetail.DateOfCase = obj_CivilStatus.DivorceCaseDate;
                                    objCivilStatusDivorceDetail.ModifiedBy = UID_;
                                    objCivilStatusDivorceDetail.ModifiedDate = DateTime.Now;
                                    objCivilStatusDivorceDetail.ModifiedMac = MacAddress;
                                    objCivilStatusDivorceDetail.Active = 1;

                                    _db.Entry(objCivilStatusDivorceDetail).State = EntityState.Modified;

                                    break;

                                /// if sub Catgeory Widow, Insert Married details to CivilStatusWidowDetail
                                case (int)POR.Enum.CivilStatusCategory.Widow:

                                    var WID = _db.CivilStatusWidowDetails.Where(x => x.CivilStatusHeaderID == obj_CivilStatus.CSHID && x.Active == 1).Select(x => x.CSWDID).FirstOrDefault();

                                    objCivilStatusWidowDetail = _db.CivilStatusWidowDetails.Find(WID);
                                    objCivilStatusWidowDetail.DeathCertificateNoOfSpouse = obj_CivilStatus.DeathCertificateNo;
                                    objCivilStatusWidowDetail.DateofDecease = obj_CivilStatus.DateofDecease;
                                    objCivilStatusWidowDetail.ModifiedBy = UID_;
                                    objCivilStatusWidowDetail.ModifiedDate = DateTime.Now;
                                    objCivilStatusWidowDetail.ModifiedMac = MacAddress;
                                    objCivilStatusWidowDetail.Active = 1;

                                    _db.Entry(objCivilStatusWidowDetail).State = EntityState.Modified;
                                    break;
                                default:
                                    break;
                            }

                            /// Update Nok header table
                            var NOKHID = _db.NOKChangeHeaders.Where(x => x.CivilStatusHeaderID == obj_CivilStatus.CSHID && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();
                            objNOKChangeHeader = _db.NOKChangeHeaders.Find(NOKHID);
                            objNOKChangeHeader.WFDate = obj_CivilStatus.NOKWDate;
                            objNOKChangeHeader.ModifiedBy = UID_;
                            objNOKChangeHeader.ModifiedDate = DateTime.Now;
                            objNOKChangeHeader.ModifiedMac = MacAddress;
                            _db.Entry(objNOKChangeHeader).State = EntityState.Modified;

                            if (_db.SaveChanges() > 0)
                            {
                                /// Update Nok Detail table
                                var NOKDID = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == NOKHID && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();

                                objNOKChangeDetail = _db.NOKChangeDetails.Find(NOKDID);
                                objNOKChangeDetail.NOKName = obj_CivilStatus.NOKName;
                                if (obj_CivilStatus.RelationshipName != null)
                                {
                                    objNOKChangeDetail.NOKChangeTo = obj_CivilStatus.RelationshipName;
                                }
                                objNOKChangeDetail.NOKAddress = obj_CivilStatus.NOKaddress;

                                //// meka kale different type of object assign value
                                if (obj_CivilStatus.District != 0)
                                {
                                    objNOKChangeDetail.District = DistName;
                                    if (obj_CivilStatus.Town1 != "SELECT")
                                    {
                                        objNOKChangeDetail.NearestTown = obj_CivilStatus.Town1;
                                    }
                                    if (obj_CivilStatus.GSName != "SELECT")
                                    {
                                        objNOKChangeDetail.GSDivision = obj_CivilStatus.GSName;
                                    }
                                    if (obj_CivilStatus.PoliceStation1 != "SELECT")
                                    {
                                        objNOKChangeDetail.PoliceStation = obj_CivilStatus.PoliceStation1;
                                    }
                                    if (obj_CivilStatus.PostOfficeName != "SELECT")
                                    {
                                        objNOKChangeDetail.PostOffice = obj_CivilStatus.PostOfficeName;
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
                                        InsertFlowStatus(obj_CivilStatus.CSHID, RoleId, UID_, obj_CivilStatus.FMSID, obj_CivilStatus.RSID);

                                        /// Update Civil Status Header details to table
                                        /// PreviousReject =1  means, this record has been reject  early and 1 is indicate it
                                        var rejectCount = _db.CivilStatusHeaders.Where(x => x.CSHID == obj_CivilStatus.CSHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                        objCivilStatusHeader = _db.CivilStatusHeaders.Find(obj_CivilStatus.CSHID);
                                        if (rejectCount == null)
                                        {
                                            objCivilStatusHeader.PreviousReject = 1;
                                        }
                                        else
                                        {
                                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                                            objCivilStatusHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                        }

                                        objCivilStatusHeader.RejectAuth = obj_CivilStatus.RejectRefNo;
                                        _db.Entry(objCivilStatusHeader).State = EntityState.Modified;

                                        /// previous reject record active 1 status turn in to 0
                                        var RejFSCSID = _db.FlowStatusCivilStatusDetails.Where(x => x.CivilStatusHeaderID == obj_CivilStatus.CSHID && x.RecordStatusID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.FSCSID).FirstOrDefault();
                                        obj = _db.FlowStatusCivilStatusDetails.Find(RejFSCSID);
                                        obj.Active = 0;
                                        obj.ModifiedBy = UID_;
                                        obj.ModifiedDate = DateTime.Now;
                                        obj.ModifiedMac = MacAddress;
                                        _db.Entry(obj).State = EntityState.Modified;

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
                        else
                        {
                            scope.Dispose();
                            TempData["ErrMsg"] = " Not Complete Your Process";
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
            return View(obj_CivilStatus);
        }
        public ActionResult Delete(int id)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/07/20
            ///Description : Delete the p3 Clerk entred data. All Active 1 record turn into 0
            /// 
            CivilStatusHeader objCivilStatusHeader = new CivilStatusHeader();
            CivilStatusMarriedDetail objCivilStatusMarriedDetail = new CivilStatusMarriedDetail();
            CivilStatusDivorceDetail objCivilStatusDivorceDetail = new CivilStatusDivorceDetail();
            CivilStatusWidowDetail objCivilStatusWidowDetail = new CivilStatusWidowDetail();
            CivilStatusSpouseDetail objCivilStatusSpouseDetail = new CivilStatusSpouseDetail();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            int UID_ = Convert.ToInt32(Session["UID"]);

            try
            {
                var CivilCategory = _db.CivilStatusHeaders.Where(x => x.CSHID == id && x.Active == 1).Select(x => x.SubCategoryId).FirstOrDefault();
                var NokHId = _db.NOKChangeHeaders.Where(x => x.CivilStatusHeaderID == id && x.Active == 1).Select(x => x.NOKCHID).FirstOrDefault();
                var NokDId = _db.NOKChangeDetails.Where(x => x.NOKChangeHeadrerID == NokHId && x.Active == 1).Select(x => x.NOKCDID).FirstOrDefault();
                var SpouseId = _db.CivilStatusSpouseDetails.Where(x => x.CivilStatusHeaderID == id && x.Active == 1).Select(x => x.CSSPDID).FirstOrDefault();

                //// CivilStatusHeader active colum 1 turn into 0
                objCivilStatusHeader = _db.CivilStatusHeaders.Find(id);
                objCivilStatusHeader.Active = 0;
                objCivilStatusHeader.ModifiedDate = DateTime.Now;
                objCivilStatusHeader.ModifiedBy = UID_;
                objCivilStatusHeader.ModifiedMac = MacAddress;

                _db.Entry(objCivilStatusHeader).State = EntityState.Modified;

                ////CivilStatusSpouse active colum 1 turn into 0
                objCivilStatusSpouseDetail = _db.CivilStatusSpouseDetails.Find(SpouseId);
                objCivilStatusSpouseDetail.Active = 0;
                objCivilStatusSpouseDetail.ModifiedBy = UID_;
                objCivilStatusSpouseDetail.ModifiedDate = DateTime.Now;
                objCivilStatusSpouseDetail.ModifiedMac = MacAddress;

                _db.Entry(objCivilStatusSpouseDetail).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        switch (CivilCategory)
                        {
                            case (int)POR.Enum.CivilStatusCategory.Marriage:
                                var MarriageId = _db.CivilStatusMarriedDetails.Where(x => x.CivilStatusHeaderID == id && x.Active == 1).Select(x => x.CSMDID).FirstOrDefault();
                                objCivilStatusMarriedDetail = _db.CivilStatusMarriedDetails.Find(MarriageId);
                                objCivilStatusMarriedDetail.Active = 0;
                                objCivilStatusMarriedDetail.ModifiedBy = UID_;
                                objCivilStatusMarriedDetail.ModifiedDate = DateTime.Now;
                                objCivilStatusMarriedDetail.ModifiedMac = MacAddress;

                                _db.Entry(objCivilStatusMarriedDetail).State = EntityState.Modified;
                                break;
                            case (int)POR.Enum.CivilStatusCategory.Divorce:
                            case (int)POR.Enum.CivilStatusCategory.NullVoid:

                                var DivorceId = _db.CivilStatusDivorceDetails.Where(x => x.CivilStatusHeaderID == id && x.Active == 1).Select(x => x.CSDDID).FirstOrDefault();
                                objCivilStatusDivorceDetail = _db.CivilStatusDivorceDetails.Find(DivorceId);
                                objCivilStatusDivorceDetail.Active = 0;
                                objCivilStatusDivorceDetail.ModifiedBy = UID_;
                                objCivilStatusDivorceDetail.ModifiedDate = DateTime.Now;
                                objCivilStatusDivorceDetail.ModifiedMac = MacAddress;

                                _db.Entry(objCivilStatusDivorceDetail).State = EntityState.Modified;
                                break;
                            case (int)POR.Enum.CivilStatusCategory.Widow:
                                var WidowId = _db.CivilStatusWidowDetails.Where(x => x.CivilStatusHeaderID == id && x.Active == 1).Select(x => x.CSWDID).FirstOrDefault();
                                objCivilStatusWidowDetail = _db.CivilStatusWidowDetails.Find(WidowId);
                                objCivilStatusWidowDetail.Active = 0;
                                objCivilStatusWidowDetail.ModifiedBy = UID_;
                                objCivilStatusWidowDetail.ModifiedDate = DateTime.Now;
                                objCivilStatusWidowDetail.ModifiedMac = MacAddress;

                                _db.Entry(objCivilStatusWidowDetail).State = EntityState.Modified;
                                break;
                            default:
                                break;
                        }

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
                        TempData["ErrMsg"] = "Process Unsucessfull.";
                        scope.Dispose();
                    }
                }
                return RedirectToAction("IndexCivilStatus", "Married");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Forward(int? id, string Sno)
        {
            //Singal Forward

            ///Created By   : Sqn ldr Wickramasinghe
            ///Created Date :2023.07.20
            ///Des: Data forward to user by user getting the data from flowmanagemt table and find the how is the forward user

            //Singal Forward        
            int? UID = 0;
            if (Session["UID"] != null)
            {
                UID = Convert.ToInt32(Session["UID"]);
            }

            string SubmitStatus_UserRole;
            bool updateStatus = false;
            //int? NextFlowStatusId;

            if (UID != 0)
            {
                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId,x.DivisionId, x.RoleId }).FirstOrDefault();
                string RecordEstablishmentId = _db.CivilStatusHeaders.Where(x => x.CSHID == id).Select(x => x.Location).FirstOrDefault();
                int? SubmitStatus = NextFlowStatusId(id, UserInfo.LocationId, RecordEstablishmentId, UserInfo.DivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                long SNO = Convert.ToInt64(Sno);

                //Insert data to Flowstatusdetails table ow forward with RSID =2000
                FlowStatusCivilStatusDetail objFlowStatusCivilStatusDetail = new FlowStatusCivilStatusDetail();

                objFlowStatusCivilStatusDetail.CivilStatusHeaderID = id;
                objFlowStatusCivilStatusDetail.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                objFlowStatusCivilStatusDetail.UserID = UID;
                objFlowStatusCivilStatusDetail.FlowManagementStatusID = SubmitStatus;
                objFlowStatusCivilStatusDetail.RoleID = UserRoleId;
                objFlowStatusCivilStatusDetail.CreatedBy = UID;
                objFlowStatusCivilStatusDetail.CreatedDate = DateTime.Now;
                objFlowStatusCivilStatusDetail.CreatedMac = MacAddress;
                objFlowStatusCivilStatusDetail.IPAddress = this.Request.UserHostAddress;
                objFlowStatusCivilStatusDetail.Active = 1;


                ///This function is to update the Hrmis data base. After account one certified, the details will update p3hrmis 
                if (RoleId == (int)POR.Enum.UserRole.ACCOUNTS01)
                {
                    /// Insert new record to P3hrms Nokchange details table
                    updateStatus = UpdateHrmis(MacAddress, UID, SNO, id);

                    if (updateStatus == true)
                    {
                        _db.FlowStatusCivilStatusDetails.Add(objFlowStatusCivilStatusDetail);

                        if (_db.SaveChanges() > 0)
                        {
                            TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                            return RedirectToAction("IndexCivilStatus");
                        }
                        else
                        {
                            TempData["ErrMsg"] = "Process Unsuccessful.";
                        }
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Something error in HRMS Record. Please Contact ITW";
                    }

                }
                else
                {
                    _db.FlowStatusCivilStatusDetails.Add(objFlowStatusCivilStatusDetail);
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                        return RedirectToAction("IndexCivilStatus");

                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    }
                }

                return RedirectToAction("IndexCivilStatus");
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
            if (Session["UID"].ToString() != null)
            {
                UID = Convert.ToInt32(Session["UID"]);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId }).FirstOrDefault();
            foreach (int IDs in id)
            {
                
                if (UID != 0)
                {
                    
                    RecordEstablishmentId = _db.CivilStatusHeaders.Where(x => x.CSHID == IDs).Select(x => x.Location).FirstOrDefault();
                   
                    int? SubmitStatus = NextFlowStatusId(IDs, userInfo.LocationId, RecordEstablishmentId, userInfo.DivisionId);

                    //Get Next FlowStatus User Role Name for Add Successfull Msg

                    UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                    SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();



                    //Insert data to Flowstatusdetails table ow forward with RSID =2000

                    FlowStatusLivingInOutDetail objFlowStatusLivingInOut = new FlowStatusLivingInOutDetail();
                    FlowStatusCivilStatusDetail objFlowStatusCivilStatusDetail = new FlowStatusCivilStatusDetail();

                    objFlowStatusCivilStatusDetail.CivilStatusHeaderID = IDs;
                    objFlowStatusCivilStatusDetail.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                    objFlowStatusCivilStatusDetail.UserID = UID;
                    objFlowStatusCivilStatusDetail.FlowManagementStatusID = SubmitStatus;
                    objFlowStatusCivilStatusDetail.RoleID = UserRoleId;
                    objFlowStatusCivilStatusDetail.CreatedBy = UID;
                    objFlowStatusCivilStatusDetail.CreatedDate = DateTime.Now;
                    objFlowStatusCivilStatusDetail.CreatedMac = MacAddress;
                    objFlowStatusCivilStatusDetail.IPAddress = this.Request.UserHostAddress;
                    objFlowStatusCivilStatusDetail.Active = 1;

                    _db.FlowStatusCivilStatusDetails.Add(objFlowStatusCivilStatusDetail);


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
            return Json("");
        }
        public int? NextFlowStatusId(int? CSHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Sqn ldr Wickramasinghe
            ///Created Date :2023.07.20
            ///Des: get the next flow status id FMSID

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxFSCSID = _db.FlowStatusCivilStatusDetails.Where(x => x.CivilStatusHeaderID == CSHID && x.Active == 1).Select(x => x.FSCSID).Max();
                int? CurrentFMSID = _db.FlowStatusCivilStatusDetails.Where(x => x.FSCSID == MaxFSCSID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
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
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OJ" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();


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
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OJ" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
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
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId || x.DivisionId3 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

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
        public List<PORRecordCount> PorNoCreate(string EstablishmentId, int CivilStatusSubCat)
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

                int jobcount = _db.CivilStatusHeaders.Where(x => x.Location == EstablishmentId && x.CreatedDate.Value.Year == currentyear && 
                              (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer) || (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer) || 
                               (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || (x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer))).Max(x => x.RecordCount).Value;

                int RocordId = jobcount + 1;

                string SubCategoryName = _db.CivilStatusCategories.Where(x => x.CSCID == CivilStatusSubCat && x.Active == 1).Select(x => x.CategoryCode).FirstOrDefault();

                string RefNo = EstablishmentId + "/"+ SubCategoryName  + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;

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
        public void InsertFlowStatus(int CSHID, int RoleId, int UID_, int? FMSID, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/0720
            /// Description : Insert Marriage details stutus to flow status table

            try
            {
                FlowStatusCivilStatusDetail objFlowStatusCivil = new FlowStatusCivilStatusDetail();

                objFlowStatusCivil.CivilStatusHeaderID = CSHID;
                objFlowStatusCivil.RecordStatusID = RSID;
                objFlowStatusCivil.UserID = UID_;
                objFlowStatusCivil.FlowManagementStatusID = FMSID;
                objFlowStatusCivil.RoleID = RoleId;
                objFlowStatusCivil.CreatedBy = UID_;
                objFlowStatusCivil.CreatedDate = DateTime.Now;
                objFlowStatusCivil.CreatedMac = MacAddress;
                objFlowStatusCivil.IPAddress = this.Request.UserHostAddress;
                objFlowStatusCivil.Active = 1;

                _db.FlowStatusCivilStatusDetails.Add(objFlowStatusCivil);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool UpdateHrmis(string MacAddress, int? UID, long? Sno, int? CivilStatusHeaderID)
        {

            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/12/06
            /// Description : update the hrmis

            BALNOK_Change_Details objNOK_Change_Details = new BALNOK_Change_Details();
            bool status = false;
            string Relationship = "";
            string districtName = "";
            string GsName = "";
            string nearestTown = "";
            string policeStation = "";
            string PostOffice = "";


            DataTable dt = new DataTable();
            DataTable civildt = new DataTable();
            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.civilStatus;
            int recordCount = 0;
            string SSNo = Convert.ToString(Sno);
            try
            {
                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(CivilStatusHeaderID, detailsCollectCategory);

                ///Generate NOK ID
                long key = objDALCommanQuery.GenerateKey("NOK_Change_Details", "NOKID");

                string NokID = Convert.ToString(Sno + "/" + key);

                foreach (DataRow row in dt.Rows)
                {
                    Relationship = row["NOKChangeTo"].ToString();// dt.Rows[13].Field<string>(13);
                    districtName = row["District"].ToString(); //dt.Rows[14].Field<string>(14);
                    GsName = row["GSDivision"].ToString(); //dt.Rows[15].Field<string>(15);
                    nearestTown = row["NearestTown"].ToString(); //dt.Rows[16].Field<string>(16);
                    policeStation = row["PoliceStation"].ToString(); //dt.Rows[17].Field<string>(17);
                    PostOffice = row["PostOffice"].ToString();
                }


                var districtId = _dbCommonData.Districts.Where(x => x.DESCRIPTION == districtName).Select(x => x.DIST_CODE).FirstOrDefault();
                var GSDivisionID = _dbCommonData.GSDivisions.Where(x => x.GSName == GsName && x.District == districtId).Select(x => x.GSDivisionID).FirstOrDefault();
                var NearestTownID = _dbCommonData.Towns.Where(x => x.Town1 == nearestTown && x.DIST_CODE == districtId).Select(x => x.TownCOde).FirstOrDefault();
                var PoliceStationId = _dbCommonData.PoliceStations.Where(x => x.PoliceStation1 == policeStation).Select(x => x.PoliceStationCode).FirstOrDefault();
                var PostOfficeId = _dbCommonData.PostOffices.Where(x => x.PostOfficeName == PostOffice).Select(x => x.PostOfficeCode).FirstOrDefault();
                var RelationshipID = _dbP3HRMS.Relationships.Where(x => x.RelationshipName == Relationship).Select(x => x.RelationshipID).FirstOrDefault();


                foreach (DataRow row in dt.Rows)
                {
                    objNOK_Change_Details.NOKID = NokID;
                    objNOK_Change_Details.SNo = Sno.ToString();
                    objNOK_Change_Details.NOKType = 1; /// Latest record NOK Type always 1... 
                    objNOK_Change_Details.Relationship = RelationshipID;
                    objNOK_Change_Details.NOKName = row["NOKName"].ToString();
                    objNOK_Change_Details.NOKAddress = row["NOKAddress"].ToString();
                    objNOK_Change_Details.District = districtId;
                    objNOK_Change_Details.GramaseDiv = GSDivisionID;
                    objNOK_Change_Details.NearPoliceSta = PoliceStationId;
                    objNOK_Change_Details.NearPostOff = Convert.ToInt32(PostOfficeId);
                    objNOK_Change_Details.NearTown = NearestTownID;
                    objNOK_Change_Details.PORRefNo = row["RefNo"].ToString();
                    objNOK_Change_Details.AuthRefNo = row["Authority"].ToString();
                    objNOK_Change_Details.WEFDate = Convert.ToDateTime(row["WFDate"]);
                    objNOK_Change_Details.LivingStatus = 1;
                    objNOK_Change_Details.Status = 1;
                    objNOK_Change_Details.CreatedDate = DateTime.Now;
                    objNOK_Change_Details.CreatedUser = Convert.ToString(UID);
                    objNOK_Change_Details.CreatedMachine = MacAddress;


                }
                /// Update P3 Hrmis DB Marriage table
                var CivilStatusCat = _db.CivilStatusHeaders.Where(x => x.CSHID == CivilStatusHeaderID && x.Active == 1).Select(x => x.SubCategoryId).FirstOrDefault();

                switch (CivilStatusCat)
                {
                    case (int)POR.Enum.CivilStatusCategory.Marriage:

                        recordCount = _dbP3HRMS.Marriages.Where(x => x.SNo == SSNo && x.MarriedStatus == 0).Count();

                        if (recordCount != 0)
                        {
                            /// Get CiviStatus Details
                            civildt = objDALCommanQuery.getCivilStatusDetails(CivilStatusHeaderID, CivilStatusCat);
                            /// Update the perivous NoK type into 2                
                            status = objDALCommanQuery.UpdateMarriageControlDetaild(Sno, UID, MacAddress, objNOK_Change_Details, civildt, CivilStatusCat);

                        }
                        else
                        {
                            status = false;
                        }
                        break;

                    case (int)POR.Enum.CivilStatusCategory.Divorce:

                        recordCount = _dbP3HRMS.Marriages.Where(x => x.SNo == SSNo && x.MarriedStatus == 1).Count();

                        if (recordCount != 0)
                        {
                            /// Get CiviStatus Details
                            civildt = objDALCommanQuery.getCivilStatusDetails(CivilStatusHeaderID, CivilStatusCat);

                            /// Update the perivous NoK type into 2                
                            status = objDALCommanQuery.UpdateMarriageControlDetaild(Sno, UID, MacAddress, objNOK_Change_Details, civildt, CivilStatusCat);

                        }
                        else
                        {
                            status = false;
                        }

                        break;

                    case (int)POR.Enum.CivilStatusCategory.Widow:

                        recordCount = _dbP3HRMS.Marriages.Where(x => x.SNo == SSNo && x.MarriedStatus == 1).Count();

                        if (recordCount != 0)
                        {
                            /// Get CiviStatus Details
                            civildt = objDALCommanQuery.getCivilStatusDetails(CivilStatusHeaderID, CivilStatusCat);

                            /// Update the perivous NoK type into 2                
                            status = objDALCommanQuery.UpdateMarriageControlDetaild(Sno, UID, MacAddress, objNOK_Change_Details, civildt, CivilStatusCat);

                        }
                        else
                        {
                            status = false;
                        }

                        break;
                    case (int)POR.Enum.CivilStatusCategory.NullVoid:

                        recordCount = _dbP3HRMS.Marriages.Where(x => x.SNo == SSNo && x.MarriedStatus == 1).Count();

                        if (recordCount != 0)
                        {
                            /// Get CiviStatus Details
                            civildt = objDALCommanQuery.getCivilStatusDetails(CivilStatusHeaderID, CivilStatusCat);

                            /// Update the perivous NoK type into 2                
                            status = objDALCommanQuery.UpdateMarriageControlDetaild(Sno, UID, MacAddress, objNOK_Change_Details, civildt, CivilStatusCat);

                        }
                        else
                        {
                            status = false;
                        }

                        break;
                    default:
                        break;
                }

                return status;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        
        #region Json Methods
        [HttpPost]
        
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
            var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => new { x.SNo, x.service_type }).FirstOrDefault();

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