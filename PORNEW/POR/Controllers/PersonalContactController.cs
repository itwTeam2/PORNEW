using Microsoft.Reporting.WebForms;
using PagedList;
using POR.Models;
using POR.Models.PersonalContact;
using ReportData.BAL;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class PersonalContactController : Controller
    {
        
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        DALCommanQueryP2 objDALCommanQueryP2 = new DALCommanQueryP2();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        dbContext _db = new dbContext();
        P3HRMS _dbP3HRMS = new P3HRMS();


        string MacAddress = new DALBase().GetMacAddress();
        int UID = 0;
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date : 2023/04/28
            /// Description : Index Page for Forward Load Person Contact Details            

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            int serviceStatus = 0;

            List<_PsnContactHeader> PsnContactList = new List<_PsnContactHeader>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId ,x.UserRoleCategory}).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

            if (UserInfo.UserRoleCategory == "P2")
            {
                serviceStatus = 1;
            }
            else
            {
                serviceStatus = 2;
            }

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

                dt = objDALCommanQuery.CallPersonalContatSP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                }
                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:
                    case (int)POR.Enum.UserRole.P2CLERK:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.P3SNCO:
                    case (int)POR.Enum.UserRole.P2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.P3OIC:
                    case (int)POR.Enum.UserRole.P2OIC:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.KOPNR:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.SNCOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.WOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                    case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.HRMSSNCO:
                    case (int)POR.Enum.UserRole.HRMSP2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                    case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.ACCOUNTS01:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                    obj_PsnContactHeader.PCHID = Convert.ToInt32(dt3.Rows[i]["PCHID"]);
                    obj_PsnContactHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_PsnContactHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_PsnContactHeader.FullName = dt3.Rows[i]["Name"].ToString();
                    obj_PsnContactHeader.Location = dt3.Rows[i]["Location"].ToString();                   
                    obj_PsnContactHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    obj_PsnContactHeader.SubCatName = dt3.Rows[i]["SubCatName"].ToString();
                    obj_PsnContactHeader.CurrentUserRole = Convert.ToInt32(dt3.Rows[i]["CurrentStatus"]);
                    PsnContactList.Add(obj_PsnContactHeader);

                }
                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(PsnContactList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            
        }
        public ActionResult IndexChildInfo(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date : 2023/04/28
            ///Description : Load Childern details

            ///This serviceStatus assign form the when click the side navigation of the respective menu. 
            /// serviceStatus = 1 officer and serviceStatus = 2 Other rank, this serviceStatus is to get officer and other rank details seperetly

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();

            List<_PsnContactHeader> PsnContactList = new List<_PsnContactHeader>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            int serviceStatus = 0;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId, x.UserRoleCategory }).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId }).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

            if (UserInfo.UserRoleCategory == "P2")
            {
                serviceStatus = 1;
            }
            else
            {
                serviceStatus = 2;
            }

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = RoleId;
                long sno = 0;
                int PsnConHeaderID = 0;

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

                dt = objDALCommanQuery.CallChildDetailsSP(sno, PsnConHeaderID);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                }
                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:
                    case (int)POR.Enum.UserRole.P2CLERK:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.P3SNCO:
                    case (int)POR.Enum.UserRole.P2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.P3OIC:
                    case (int)POR.Enum.UserRole.P2OIC:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.KOPNR:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.SNCOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.WOSALARY:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                    case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.HRMSSNCO:
                    case (int)POR.Enum.UserRole.HRMSP2SNCO:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                    case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    case (int)POR.Enum.UserRole.ACCOUNTS01:
                        dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    default:
                        break;
                }

                for (int i = 0; i < dt3.Rows.Count; i++)
                {
                    _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                    obj_PsnContactHeader.PCHID = Convert.ToInt32(dt3.Rows[i]["PCHID"]);
                    obj_PsnContactHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                    obj_PsnContactHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                    obj_PsnContactHeader.FullName = dt3.Rows[i]["Name"].ToString();
                    obj_PsnContactHeader.Location = dt3.Rows[i]["Location"].ToString();
                    obj_PsnContactHeader.ChildFullNameWithInitial = dt3.Rows[i]["ChildFullNameWithInitial"].ToString();
                    obj_PsnContactHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    obj_PsnContactHeader.CurrentUserRole = Convert.ToInt32(dt3.Rows[i]["CurrentStatus"]);

                    PsnContactList.Add(obj_PsnContactHeader);

                }
                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(PsnContactList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID, int serviceStatus)
        {
            ///Created BY   : Sqn ldr  WAKY Wickramasinghe
            ///Created Date : 2024/04/28
            /// Description : Load data user roll wise, this fuction call from Index()        

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                //int serviceStatus = 1;

                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:
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
                        /// [30] means null coloumn number

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();

                        /// Check the data table has row or not
                        result = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                /// here send a serviceStatus as 1 from index function. this is to categorized the officer and other rank seperettly
                                if (serviceStatus == 1)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                    }

                                }
                                else
                                {
                                    var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                    if (row.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                    }
                                }
                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    switch (RoleId)
                                    {
                                        case (int)POR.Enum.UserRole.HRMSP2SNCO:
                                        case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                                        case (int)POR.Enum.UserRole.ASORSOVRP2VOL:

                                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                                            if (rows.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                            }

                                            break;
                                        default:
                                            var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                            if (row.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                                            }
                                            break;
                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    switch (RoleId)
                                    {
                                        case (int)POR.Enum.UserRole.HRMSP2SNCO:
                                        case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                                        case (int)POR.Enum.UserRole.ASORSOVRP2VOL:

                                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer));

                                            if (rows.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).CopyToDataTable();
                                            }
                                            break;

                                        default:

                                            var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                        (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen));

                                            if (row.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).CopyToDataTable();
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    var Count = 0;
                                    switch (RoleId)
                                    {
                                        case (int)POR.Enum.UserRole.P2SNCO:
                                        case (int)POR.Enum.UserRole.P2OIC:

                                             Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                    (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                    x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer 
                                                    || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                            if (Count != 0)
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                      (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                            }

                                            break;

                                        default:

                                            Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                    (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                    x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                            if (Count != 0)
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RecordStatusID") ==
                                                      (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                            }

                                            break;
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
        [HttpGet]
        public ActionResult CreatePsnContact()
        {
            /// Create By: Sqn ldr Wickramasinghe
            /// Create Date: 28/04/2023  
            /// Description: POR clerk's initial step of crate Persanal Contact info,

            try
            {
                ViewBag.DDL_PersonalContactCat = new SelectList(_db.PORMasterSubCategories.OrderBy(x => x.MSCID), "MSCID", "SubCategory");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DESCRIPTION", "DESCRIPTION");
                ViewBag.DDL_ChildLivingStatus = new SelectList(_db.PSubCategories.OrderBy(x => x.SCID), "SCID", "SubCatShortName");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreatePsnContact(_PsnContactHeader obj_PsnContactHeader, string Gender)
        {
            /// Create By: Sqn ldr Wickramasinghe
            /// Create Date: 28/04/2023  
            /// Description: POR clerk's initial step of crate Persanal Contact info,

            //PsnContactHeader objPsnContactHeader = new PsnContactHeader();

            obj_PsnContactHeader.RSID = (int)POR.Enum.RecordStatus.Insert;

            try
            {
                ViewBag.DDL_PersonalContactCat = new SelectList(_db.PORMasterSubCategories.OrderBy(x => x.MSCID), "MSCID", "SubCategory");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DESCRIPTION", "DESCRIPTION");
                ViewBag.DDL_ChildLivingStatus = new SelectList(_db.PSubCategories.OrderBy(x => x.SCID), "SCID", "SubCatShortName");               

                var ServiceInfo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_PsnContactHeader.ServiceNo).Select(x => new { x.SNo, x.service_type }).FirstOrDefault();
                long ConSNo = Convert.ToInt64(ServiceInfo.SNo);

                PsnContactHeader objPsnContactHeader = new PsnContactHeader();
                PsnContactDetail objPsnContactDetail = new PsnContactDetail();
                if (Session["UID"] != null)
                {
                    UID = Convert.ToInt32(Session["UID"]);
                   
                    //Get the user login Info
                    var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

                    string createPorNo = PorNoCreate(userInfo.LocationId, ServiceInfo.service_type, obj_PsnContactHeader.MasSubCatID);

                    if (ModelState.IsValid)
                    {
                        objPsnContactHeader.Sno = ConSNo;
                        objPsnContactHeader.ServiceType = ServiceInfo.service_type;
                        objPsnContactHeader.RefNo = createPorNo;
                        objPsnContactHeader.Location = userInfo.LocationId;
                        objPsnContactHeader.MasSubCatID = obj_PsnContactHeader.MasSubCatID;
                        objPsnContactHeader.Authority = obj_PsnContactHeader.Authority;
                        objPsnContactHeader.Active = 1;
                        objPsnContactHeader.CreatedBy = UID;
                        objPsnContactHeader.CreatedDate = DateTime.Now;
                        objPsnContactHeader.CreatedMac = MacAddress;
                        objPsnContactHeader.IPAddress = this.Request.UserHostAddress;

                        _db.PsnContactHeaders.Add(objPsnContactHeader);
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                        {
                            if (_db.SaveChanges() > 0)
                            {
                                var PCHID = _db.PsnContactHeaders.Where(x => x.Sno == ConSNo
                                                  && x.Location == userInfo.LocationId && x.Active == 1 && x.RefNo == createPorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.PCHID).FirstOrDefault();

                                if (obj_PsnContactHeader.MasSubCatID != (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth)
                                {
                                    switch (obj_PsnContactHeader.MasSubCatID)
                                    {
                                        case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                            objPsnContactDetail.TelMobileNo = obj_PsnContactHeader.MobileNo;
                                            break;
                                        case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                            objPsnContactDetail.TelMobileNo = obj_PsnContactHeader.ResidentialTeleNo;
                                            break;
                                        case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                            objPsnContactDetail.EmailAddress = obj_PsnContactHeader.EmailAddress;
                                            break;
                                    }

                                    objPsnContactDetail.PsnConHeaderID = PCHID;
                                    objPsnContactDetail.MasterSubCatID = obj_PsnContactHeader.MasSubCatID;
                                    objPsnContactDetail.Active = 1;
                                    objPsnContactDetail.CreatedBy = UID;
                                    objPsnContactDetail.CreatedDate = DateTime.Now;
                                    objPsnContactDetail.CreatedMac = MacAddress;

                                    _db.PsnContactDetails.Add(objPsnContactDetail);

                                    InserFlowStatus(PCHID, userInfo.RoleId, UID, obj_PsnContactHeader.FMSID, obj_PsnContactHeader.RSID);
                                    scope.Complete();
                                    TempData["ScfMsg"] = "Data Successfully Saved.";
                                }
                                else
                                {
                                    PsnChildDetail objPsnChildDetail = new PsnChildDetail();

                                    //child in living Status, insert the living status records to table
                                    if (obj_PsnContactHeader.SCID == (int)POR.Enum.PorSubCategory.Live)
                                    {
                                        ///Get District name
                                        //var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_PsnContactHeader.District).Select(x => x.DESCRIPTION).FirstOrDefault();

                                        bool validity = ChecktheValidation(obj_PsnContactHeader.ChildFullName, obj_PsnContactHeader.ChildFullNameWithInitial,Gender,
                                                        obj_PsnContactHeader.BirthPlace, obj_PsnContactHeader.BirthCertificateNo);
                                        if (validity == true)
                                        {
                                            objPsnChildDetail.PsnConHeaderID = PCHID;
                                            objPsnChildDetail.SubCategoryID = obj_PsnContactHeader.SCID;
                                            objPsnChildDetail.MasterSubCatID = obj_PsnContactHeader.MasSubCatID;
                                            objPsnChildDetail.BirthCertificateNo = obj_PsnContactHeader.BirthCertificateNo;
                                            objPsnChildDetail.DateOfBirth = obj_PsnContactHeader.DateOfBirth;
                                            objPsnChildDetail.BirthPlace = obj_PsnContactHeader.BirthPlace;
                                            objPsnChildDetail.Gender = Convert.ToInt16(Gender);
                                            objPsnChildDetail.ChildFullName = obj_PsnContactHeader.ChildFullName.ToUpper();
                                            objPsnChildDetail.ChildFullNameWithInitial = obj_PsnContactHeader.ChildFullNameWithInitial.ToUpper();
                                            objPsnChildDetail.District = obj_PsnContactHeader.Disrict2;
                                            objPsnChildDetail.Active = 1;
                                            objPsnChildDetail.CreatedBy = UID;
                                            objPsnChildDetail.CreatedDate = DateTime.Now;
                                            objPsnChildDetail.CreatedMac = MacAddress;

                                            _db.PsnChildDetails.Add(objPsnChildDetail);

                                            InserFlowStatus(PCHID, userInfo.RoleId, UID, obj_PsnContactHeader.FMSID, obj_PsnContactHeader.RSID);
                                            scope.Complete();
                                            TempData["ScfMsg"] = "Data Successfully Saved.";
                                        }
                                        else
                                        {
                                            TempData["ErrMsg"] = "Input Fields Can not be empty. Please Fill all the Fields.";
                                        }                                        
                                    }
                                    else
                                    {
                                        // child in a death status, select the death child name record Id and update the living Status into Detath status 
                                        // and update the DateOfDeath and DeathCertificateNo colum...

                                        bool validity = ChecktheValidation(obj_PsnContactHeader.DateOfDeath, obj_PsnContactHeader.DeathCertificateNo);
                                        var PreviousePCHID = _db.PsnChildDetails.Where(x => x.PCDID == obj_PsnContactHeader.PCDID && x.Active == 1).Select(x => x.PsnConHeaderID).FirstOrDefault();
                                        if (validity == true)
                                        {
                                            objPsnChildDetail = _db.PsnChildDetails.Find(obj_PsnContactHeader.PCDID);
                                            objPsnChildDetail.PsnConHeaderID = PCHID;
                                            objPsnChildDetail.PsnBirthHeaderID = PreviousePCHID;
                                            objPsnChildDetail.SubCategoryID = (int)POR.Enum.PorSubCategory.Death;
                                            objPsnChildDetail.DateOfDeath = obj_PsnContactHeader.DateOfDeath;
                                            objPsnChildDetail.DeathCertificateNo = obj_PsnContactHeader.DeathCertificateNo;
                                            objPsnChildDetail.ModifiedBy = UID;
                                            objPsnChildDetail.ModifiedDate = DateTime.Now;
                                            objPsnChildDetail.ModifiedMac = MacAddress;

                                            _db.Entry(objPsnChildDetail).State = EntityState.Modified;
                                            InserFlowStatus(PCHID, userInfo.RoleId, UID, obj_PsnContactHeader.FMSID, obj_PsnContactHeader.RSID);
                                            scope.Complete();
                                            TempData["ScfMsg"] = "Data Successfully Saved.";
                                        }
                                        else
                                        {
                                            TempData["ErrMsg"] = "Input Fields Can not be empty. Please Fill all the Fields.";
                                        }                                        
                                    }                              
                                }                                
                            }
                            ModelState.Clear();
                        }                        
                    }
                    else
                    {
                        string errorMessage = "";
                      
                        switch (obj_PsnContactHeader.MasSubCatID)
                        {
                            case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                errorMessage = ModelState["MobileNo"].Errors[0].ErrorMessage;
                                TempData["ErrMsg"] = errorMessage + ' ' + "Please enter correct format.";
                                ModelState.Remove("MobileNo");
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                errorMessage = ModelState["ResidentialTeleNo"].Errors[0].ErrorMessage;
                                TempData["ErrMsg"] = errorMessage + ' ' + "Please enter correct format.";
                                
                                ModelState.Remove("ResidentialTeleNo");
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                errorMessage = ModelState["EmailAddress"].Errors[0].ErrorMessage;
                                TempData["ErrMsg"] = errorMessage + ' ' + "Please type correct Email address format.";
                                
                                ModelState.Remove("EmailAddress");
                                break;                            
                        }
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
        public ActionResult PrintData(int? PCHID)
        {
            /// Create By: Cpl Madusanka
            /// Create Date: 02/06/2023  
            ///// Description: Contact Report,


            Session["PCHID"] = PCHID;
           
            return View();
        }
        [HttpGet]
        public ActionResult PrintChildDetails(int? PCHID)
        {
            /// Create By: Cpl Madusanka
            /// Create Date: 16/06/2023  
            ///// Description: Child details Print ,


            Session["PCHID"] = PCHID;

            return View();
        }
        [HttpGet]
        public ActionResult Details(int PCHID, int Rejectstatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/05/25
            /// Description : Details related Person Contact and Child

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;

                int? CurrentStatusUserRole;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_PsnContactHeader> PsnList = new List<_PsnContactHeader>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

                TempData["UserRoleId"] = userInfo.RoleId;

                var CurrentStatus_UserRole = (from f in _db.FlowStatusPsnContacts
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.PsnConHeaderID == PCHID
                                              orderby f.FSPCID descending
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
                dt = objDALCommanQuery.CallPersonalContatSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("PCHID") == PCHID).CopyToDataTable();

                var massSubId = _db.PsnContactHeaders.Where(x => x.PCHID == PCHID && x.Active == 1).Select(x => x.MasSubCatID).FirstOrDefault();
                TempData["massSubId"] = massSubId;
                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (Rejectstatus != 2)
                {
                   
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                        /// Check the rercord is previously reject or not
                        var prvReject = _db.PsnContactHeaders.Where(x => x.PCHID == PCHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        obj_PsnContactHeader.PCHID = PCHID;
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                       
                        switch (massSubId)
                        {
                            case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                obj_PsnContactHeader.MobileNo = dt2.Rows[i]["TelMobileNo"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                obj_PsnContactHeader.ResidentialTeleNo = dt2.Rows[i]["TelMobileNo"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                obj_PsnContactHeader.EmailAddress = dt2.Rows[i]["EmailAddress"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;                           
                           
                        }               
                        

                        if (prvReject >= 1)
                        {
                            obj_PsnContactHeader.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_PsnContactHeader.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }

                       
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;

                        PsnList.Add(obj_PsnContactHeader);
                    }


                    return View(PsnList);
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusPsnContacts.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.PsnConHeaderID == PCHID)
                                        .OrderByDescending(x => x.FSPCID).Select(x => x.RoleID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                        obj_PsnContactHeader.PCHID = PCHID;
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                        obj_PsnContactHeader.RejectRoleName = RoleName;
                        obj_PsnContactHeader.Comment = dt2.Rows[i]["RejectComment"].ToString();



                        switch (massSubId)
                        {
                            case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                obj_PsnContactHeader.MobileNo = dt2.Rows[i]["TelMobileNo"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                obj_PsnContactHeader.ResidentialTeleNo = dt2.Rows[i]["TelMobileNo"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                obj_PsnContactHeader.EmailAddress = dt2.Rows[i]["EmailAddress"].ToString();
                                obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                                break;
                            default:
                                break;
                        }
                       
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;

                        PsnList.Add(obj_PsnContactHeader);
                    }

                    return View(PsnList);

                }
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult ChildDetails(int PCHID, int Rejectstatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/05/25
            /// Description : Details related Person Contact and Child

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;

                int? CurrentStatusUserRole;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_PsnContactHeader> PsnList = new List<_PsnContactHeader>();

                UID_ = Convert.ToInt32(Session["UID"]);
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

                TempData["UserRoleId"] = userInfo.RoleId;

                var CurrentStatus_UserRole = (from f in _db.FlowStatusPsnContacts
                                              join u in _db.Vw_FlowStatus on f.FlowManagementStatusID equals u.FMSID
                                              where u.EstablishmentId == userInfo.LocationId & f.PsnConHeaderID == PCHID
                                              orderby f.FSPCID descending
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
                dt = objDALCommanQuery.CallChildDetailsSP(0,0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("PCHID") == PCHID).CopyToDataTable();

                var childLiveStatus = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == PCHID && x.Active == 1).Select(x => x.SubCategoryID).FirstOrDefault();
                TempData["childLiveStatus"] = childLiveStatus;
                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (Rejectstatus != 2)
                {

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                        /// Check the rercord is previously reject or not
                        var prvReject = _db.PsnContactHeaders.Where(x => x.PCHID == PCHID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        obj_PsnContactHeader.PCHID = PCHID;
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                        
                        switch (childLiveStatus)
                        {
                            case (int)POR.Enum.PorSubCategory.Live:
                                obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                                obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                                obj_PsnContactHeader.Disrict2 = dt2.Rows[i]["District"].ToString();
                                obj_PsnContactHeader.BirthCertificateNo = dt2.Rows[i]["BirthCertificateNo"].ToString();
                                obj_PsnContactHeader.BirthPlace = dt2.Rows[i]["BirthPlace"].ToString();
                                obj_PsnContactHeader.DateOfBirth = Convert.ToDateTime(dt2.Rows[i]["DateOfBirth"]);
                                obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                                break;
                            case (int)POR.Enum.PorSubCategory.Death:
                                obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                                obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                                obj_PsnContactHeader.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNo"].ToString();
                                obj_PsnContactHeader.DateOfDeath = Convert.ToDateTime(dt2.Rows[i]["DateOfDeath"]);
                                obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                                break;                           
                        }


                        if (prvReject >= 1)
                        {
                            obj_PsnContactHeader.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                            obj_PsnContactHeader.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        }


                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;

                        PsnList.Add(obj_PsnContactHeader);
                    }


                    return View(PsnList);
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusPsnContacts.Where(x => x.RecordStatusID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.PsnConHeaderID == PCHID)
                                        .OrderByDescending(x => x.FSPCID).Select(x => x.RoleID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                        obj_PsnContactHeader.PCHID = PCHID;
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                        obj_PsnContactHeader.Comment = dt2.Rows[i]["RejectComment"].ToString();
                        obj_PsnContactHeader.RejectRoleName = RoleName;


                        switch (childLiveStatus)
                        {
                            case (int)POR.Enum.PorSubCategory.Live:
                                obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                                obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                                obj_PsnContactHeader.Disrict2 = dt2.Rows[i]["District"].ToString();
                                obj_PsnContactHeader.BirthCertificateNo = dt2.Rows[i]["BirthCertificateNo"].ToString();
                                obj_PsnContactHeader.BirthPlace = dt2.Rows[i]["BirthPlace"].ToString();
                                obj_PsnContactHeader.DateOfBirth = Convert.ToDateTime(dt2.Rows[i]["DateOfBirth"]);
                                obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                                break;
                             case (int)POR.Enum.PorSubCategory.Death:
                                obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                                obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                                obj_PsnContactHeader.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNo"].ToString();
                                obj_PsnContactHeader.DateOfDeath = Convert.ToDateTime(dt2.Rows[i]["DateOfDeath"]);
                                obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                                break;
                        }

                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                        {
                            obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                        }
                     
                        PsnList.Add(obj_PsnContactHeader);
                    }

                    return View(PsnList);

                }
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult Edit(int id, int rejectStatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/05/31
            ///Description : Load Edit page with user enter data related to pesrcon Contact Details.

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_PsnContactHeader> PsnList = new List<_PsnContactHeader>();

                string RejectRef;               

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
                dt = objDALCommanQuery.CallPersonalContatSP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("PCHID") == id).CopyToDataTable();

                _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {

                    obj_PsnContactHeader.PCHID = Convert.ToInt32(dt2.Rows[i]["PCHID"]);
                    obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                    obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                    obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                    obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                    obj_PsnContactHeader.MasSubCatID = Convert.ToInt32(dt2.Rows[i]["MasSubCatID"]);
                    int massSubCatId = Convert.ToInt32(dt2.Rows[i]["MasSubCatID"]);
                    TempData["massSubId"] = massSubCatId;
                    switch (massSubCatId)
                    {
                        case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                            obj_PsnContactHeader.MobileNo = dt2.Rows[i]["TelMobileNo"].ToString();
                            obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                            obj_PsnContactHeader.ResidentialTeleNo = dt2.Rows[i]["TelMobileNo"].ToString();
                            obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                            obj_PsnContactHeader.EmailAddress = dt2.Rows[i]["EmailAddress"].ToString();
                            obj_PsnContactHeader.SubCatName = dt2.Rows[i]["SubCatName"].ToString();
                            break;
                        default:
                            break;

                    }

                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.NOKChangeHeaders.Where(x => x.NOKCHID == id && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_PsnContactHeader.RefNo + " " + " - Reject";
                            obj_PsnContactHeader.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_PsnContactHeader.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_PsnContactHeader.RejectRefNo = RejectRef;
                        }
                    }
                   
                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }


                    //LivingInOutList.Add(obj_LivingInOut);
                }
                //return RedirectToAction("Edit", "User");
                return View(obj_PsnContactHeader);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult Edit(_PsnContactHeader obj_PsnContactHeader, int rejectStatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/06/01
            ///Description : Load Edit page with user enter data related to pesrcon Contact Details.

            PsnContactHeader objPsnContactHeader = new PsnContactHeader();
            PsnContactDetail objPsnContactDetail = new PsnContactDetail();
            FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();

            int UID_ = 0;
            int RoleId = 0;

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            int RecordStatus = (int)POR.Enum.RecordStatus.Insert;

            try
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                objPsnContactHeader = _db.PsnContactHeaders.Find(obj_PsnContactHeader.PCHID);

                objPsnContactHeader.Authority = obj_PsnContactHeader.Authority;
                objPsnContactHeader.ModifiedBy = UID_;
                objPsnContactHeader.ModifiedDate = DateTime.Now;
                objPsnContactHeader.ModifiedMac = MacAddress;
                _db.Entry(objPsnContactHeader).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        //Update Psn Children Details
                        var PCDID = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == obj_PsnContactHeader.PCHID && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();

                        objPsnContactDetail = _db.PsnContactDetails.Find(PCDID);
                        
                        switch (obj_PsnContactHeader.MasSubCatID)
                        {
                            case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                objPsnContactDetail.TelMobileNo = obj_PsnContactHeader.MobileNo;
                               
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                objPsnContactDetail.TelMobileNo = obj_PsnContactHeader.ResidentialTeleNo;
                                
                                break;
                            case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                obj_PsnContactHeader.EmailAddress = obj_PsnContactHeader.EmailAddress;
                               
                                break;
                        }
                        objPsnContactDetail.ModifiedBy = UID_;
                        objPsnContactDetail.ModifiedDate = DateTime.Now;
                        objPsnContactDetail.ModifiedMac = MacAddress;
                        _db.Entry(objPsnContactDetail).State = EntityState.Modified;

                        if (_db.SaveChanges() > 0)
                        {
                            if (rejectStatus == 2)
                            {
                                ////Insert First Flow Mgt record to FlowStatuPsnConact table
                                RoleId = (int)POR.Enum.UserRole.P3CLERK;
                                InserFlowStatus(obj_PsnContactHeader.PCHID, RoleId, UID_, obj_PsnContactHeader.FMSID, RecordStatus);

                                /// Update Living In/Out Header details to table
                                /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                var rejectCount = _db.PsnContactHeaders.Where(x => x.PCHID == obj_PsnContactHeader.PCDID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                objPsnContactHeader = _db.PsnContactHeaders.Find(obj_PsnContactHeader.PCHID);

                                if (rejectCount == null)
                                {
                                    objPsnContactHeader.PreviousReject = 1;
                                }
                                else
                                {
                                    int refIncrement = Convert.ToInt32(rejectCount + 1);
                                    objPsnContactHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                }

                                objPsnContactHeader.RejectAuth = obj_PsnContactHeader.RejectRefNo;
                                _db.Entry(objPsnContactHeader).State = EntityState.Modified;

                                /// previous reject record active 1 status turn in to 0
                                var RejLFSID = _db.FlowStatusPsnContacts.Where(x => x.PsnConHeaderID == obj_PsnContactHeader.PCHID && x.RecordStatusID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.FSPCID).FirstOrDefault();
                                objFlowStatusPsnContact = _db.FlowStatusPsnContacts.Find(RejLFSID);
                                objFlowStatusPsnContact.Active = 0;
                                objFlowStatusPsnContact.ModifiedBy = UID_;
                                objFlowStatusPsnContact.ModifiedDate = DateTime.Now;
                                objFlowStatusPsnContact.ModifiedMac = MacAddress;
                                _db.Entry(objFlowStatusPsnContact).State = EntityState.Modified;

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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();        
        }

        [HttpGet]
        public ActionResult ChildEdit(int id, int rejectStatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/05/31
            ///Description : Load Edit page with user enter data related to pesrcon Child Details.
            /// 

            if (Session["UID"] != null)
            {
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DESCRIPTION", "DESCRIPTION");

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_PsnContactHeader> PsnList = new List<_PsnContactHeader>();

                string RejectRef;

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
                dt = objDALCommanQuery.CallChildDetailsSP(0,0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("PCHID") == id).CopyToDataTable();

                _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {

                    obj_PsnContactHeader.PCHID = Convert.ToInt32(dt2.Rows[i]["PCHID"]);
                    obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                    obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                    obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                    obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    obj_PsnContactHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                    obj_PsnContactHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RecordStatusID"]);
                    obj_PsnContactHeader.SCID = Convert.ToInt32(dt2.Rows[i]["SubCategoryID"]);

                    int childLiveStatus = Convert.ToInt32(dt2.Rows[i]["SubCategoryID"]);
                    TempData["childLiveStatus"] = childLiveStatus;
                    switch (childLiveStatus)
                    {
                        case (int)POR.Enum.PorSubCategory.Live:
                            obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                            obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                            obj_PsnContactHeader.Disrict2 = dt2.Rows[i]["District"].ToString();
                            obj_PsnContactHeader.BirthCertificateNo = dt2.Rows[i]["BirthCertificateNo"].ToString();
                            obj_PsnContactHeader.BirthPlace = dt2.Rows[i]["BirthPlace"].ToString();
                            obj_PsnContactHeader.DateOfBirth = Convert.ToDateTime(dt2.Rows[i]["DateOfBirth"]);
                            obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                            break;
                        case (int)POR.Enum.PorSubCategory.Death:
                            obj_PsnContactHeader.ChildFullName = dt2.Rows[i]["ChildFullName"].ToString();
                            obj_PsnContactHeader.ChildFullNameWithInitial = dt2.Rows[i]["ChildFullNameWithInitial"].ToString();
                            obj_PsnContactHeader.DeathCertificateNo = dt2.Rows[i]["DeathCertificateNo"].ToString();
                            obj_PsnContactHeader.DateOfDeath = Convert.ToDateTime(dt2.Rows[i]["DateOfDeath"]);
                            obj_PsnContactHeader.GenderType = dt2.Rows[i]["GenderType"].ToString();
                            break;
                    }

                    if (rejectStatus == 2)
                    {
                        var rejectCount = _db.NOKChangeHeaders.Where(x => x.NOKCHID == id && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                        if (rejectCount == null)
                        {
                            RejectRef = obj_PsnContactHeader.RefNo + " " + " - Reject";
                            obj_PsnContactHeader.RejectRefNo = RejectRef;
                        }
                        else
                        {
                            int refIncrement = Convert.ToInt32(rejectCount + 1);
                            RejectRef = obj_PsnContactHeader.RefNo + " " + " - Reject- " + refIncrement + "";
                            obj_PsnContactHeader.RejectRefNo = RejectRef;
                        }
                    }

                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                    {
                        obj_PsnContactHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FlowManagementStatusID"]);
                    }


                    //LivingInOutList.Add(obj_LivingInOut);
                }
                //return RedirectToAction("Edit", "User");
                return View(obj_PsnContactHeader);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }
        }

        public ActionResult ChildEdit(_PsnContactHeader obj_PsnContactHeader, int rejectStatus)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date : 2023/06/01
            ///Description : Load Edit page with user enter data related to pesrcon Contact Details.

            PsnContactHeader objPsnContactHeader = new PsnContactHeader();
            PsnChildDetail objPsnChildDetail = new PsnChildDetail();
            FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();

            int UID_ = 0;
            int RoleId = 0;

            /// intial create RSID is 1000, hense we assign manully 1000 to bj_CivilStatus.RSID
            int RecordStatus = (int)POR.Enum.RecordStatus.Insert;

            try
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                objPsnContactHeader = _db.PsnContactHeaders.Find(obj_PsnContactHeader.PCHID);

                objPsnContactHeader.Authority = obj_PsnContactHeader.Authority;
                objPsnContactHeader.ModifiedBy = UID_;
                objPsnContactHeader.ModifiedDate = DateTime.Now;
                objPsnContactHeader.ModifiedMac = MacAddress;
                _db.Entry(objPsnContactHeader).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        //Update Psn Children Details
                        var PCDID = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == obj_PsnContactHeader.PCHID && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();

                        objPsnChildDetail = _db.PsnChildDetails.Find(PCDID);

                        switch (obj_PsnContactHeader.SCID)
                        {
                            case (int)POR.Enum.PorSubCategory.Live:
                                objPsnChildDetail.ChildFullName = obj_PsnContactHeader.ChildFullName.ToUpper();
                                objPsnChildDetail.ChildFullNameWithInitial = obj_PsnContactHeader.ChildFullNameWithInitial.ToUpper();
                                objPsnChildDetail.District = obj_PsnContactHeader.Disrict3;
                                objPsnChildDetail.BirthCertificateNo = obj_PsnContactHeader.BirthCertificateNo;
                                objPsnChildDetail.BirthPlace = obj_PsnContactHeader.BirthPlace.ToUpper();
                                objPsnChildDetail.DateOfBirth = obj_PsnContactHeader.DateOfBirth;
                                
                                break;
                            case (int)POR.Enum.PorSubCategory.Death:
                                objPsnChildDetail.DeathCertificateNo = obj_PsnContactHeader.DeathCertificateNo;
                                objPsnChildDetail.DateOfDeath = obj_PsnContactHeader.DateOfDeath;
                               
                                break;                           
                        }
                        objPsnChildDetail.ModifiedBy = UID_;
                        objPsnChildDetail.ModifiedDate = DateTime.Now;
                        objPsnChildDetail.ModifiedMac = MacAddress;
                        _db.Entry(objPsnChildDetail).State = EntityState.Modified;

                        if (_db.SaveChanges() > 0)
                        {
                            if (rejectStatus == 2)
                            {
                                ////Insert First Flow Mgt record to FlowStatuPsnConact table
                                RoleId = (int)POR.Enum.UserRole.P3CLERK;
                                InserFlowStatus(obj_PsnContactHeader.PCHID, RoleId, UID_, obj_PsnContactHeader.FMSID, RecordStatus);

                                /// Update Living In/Out Header details to table
                                /// PreviousReject =1  means, this record has been reject  early stage and 1 is indicate it
                                var rejectCount = _db.PsnContactHeaders.Where(x => x.PCHID == obj_PsnContactHeader.PCDID && x.Active == 1).Select(x => x.PreviousReject).FirstOrDefault();

                                objPsnContactHeader = _db.PsnContactHeaders.Find(obj_PsnContactHeader.PCHID);

                                objPsnContactHeader = _db.PsnContactHeaders.Find(obj_PsnContactHeader.PCHID);

                                if (rejectCount == null)
                                {
                                    objPsnContactHeader.PreviousReject = 1;
                                }
                                else
                                {
                                    int refIncrement = Convert.ToInt32(rejectCount + 1);
                                    objPsnContactHeader.PreviousReject = Convert.ToInt16(refIncrement);
                                }

                                objPsnContactHeader.RejectAuth = obj_PsnContactHeader.RejectRefNo;
                                _db.Entry(objPsnContactHeader).State = EntityState.Modified;

                                /// previous reject record active 1 status turn in to 0
                                var RejLFSID = _db.FlowStatusPsnContacts.Where(x => x.PsnConHeaderID == obj_PsnContactHeader.PCHID && x.RecordStatusID == (int)POR.Enum.RecordStatus.Reject && x.Active == 1).Select(x => x.FSPCID).FirstOrDefault();
                                objFlowStatusPsnContact = _db.FlowStatusPsnContacts.Find(RejLFSID);
                                objFlowStatusPsnContact.Active = 0;
                                objFlowStatusPsnContact.ModifiedBy = UID_;
                                objFlowStatusPsnContact.ModifiedDate = DateTime.Now;
                                objFlowStatusPsnContact.ModifiedMac = MacAddress;
                                _db.Entry(objFlowStatusPsnContact).State = EntityState.Modified;

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
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }

        public ActionResult Reject(int PCHID, int FMSID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe 
            ///Created Date : 2023/06/05
            /// Description : this function is to reject the record

            _PsnContactHeader model = new _PsnContactHeader();
            try
            {
                model.PCHID = PCHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            return PartialView("_RejectCommentPsnContact", model);
        }

        [HttpGet]
        public ActionResult RejectConfirm(int id)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe 
            ///Created Date : 2023/06/07
            /// Description : P3 Clerk finally confirm the reject Confirm. Afetr confirm record Status came to 0

            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);
                var MasSubCatID = _db.PsnContactHeaders.Where(x => x.PCHID == id).Select(x => x.MasSubCatID).FirstOrDefault();

                //Update PsnHeader Active colum to 0

                PsnContactHeader objPsnContactHeader = _db.PsnContactHeaders.Find(id);
                objPsnContactHeader.Active = 0;
                objPsnContactHeader.ModifiedBy = UID_;
                objPsnContactHeader.ModifiedDate = DateTime.Now;
                objPsnContactHeader.ModifiedMac = MacAddress;
                _db.Entry(objPsnContactHeader).Property(x => x.Active).IsModified = true;

                switch (MasSubCatID)
                {
                    case (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth:
                        var PsnChildId = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();

                        PsnChildDetail objPsnChildDetail = _db.PsnChildDetails.Find(PsnChildId);
                        objPsnChildDetail.Active = 0;
                        objPsnChildDetail.ModifiedBy = UID_;
                        objPsnChildDetail.ModifiedDate = DateTime.Now;
                        objPsnChildDetail.ModifiedMac = MacAddress;
                        _db.Entry(objPsnChildDetail).Property(x => x.Active).IsModified = true;

                        break;
                    default:

                        var PCDID = _db.PsnContactDetails.Where(x=>x.PsnConHeaderID == id && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();
                        //Update PsnContactDetails Active Coloum to 0

                        PsnContactDetail objPsnContactDetail = _db.PsnContactDetails.Find(PCDID);                        
                        objPsnContactDetail.Active = 0;
                        objPsnContactDetail.ModifiedBy = UID_;
                        objPsnContactDetail.ModifiedDate = DateTime.Now;
                        objPsnContactDetail.ModifiedMac = MacAddress;
                        _db.Entry(objPsnContactDetail).Property(x => x.Active).IsModified = true;


                        break;
                }

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (_db.SaveChanges() > 0)
                    {
                        scope.Complete();
                        TempData["ScfMsg"] = "Successfully Reject Confirmed.";                        
                        
                    }
                    else
                    {
                        scope.Dispose();
                        TempData["ErrMsg"] = "Not Complete Your Process";
                    }
                    switch (MasSubCatID)
                    {
                        case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                        case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                        case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                            return RedirectToAction("IndexRejectPsnContact", "PersonalContact");

                        default:
                            return RedirectToAction("IndexRejectPsnChild", "PersonalContact");
                    }                    
                }
                  
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        public int? PreviousFlowStatusId(int? PCHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId, int? ServiceTypeId)
        {
            ///Created By   : Sqn ldr  WAKY Wickramasinghe
            ///Created Date :2023.06.05
            ///Des: get the reject flow status id FMSID

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFSPCIDID = _db.FlowStatusPsnContacts.Where(x => x.PsnConHeaderID == PCHID).Select(x => x.FSPCID).Max();
                int? CurrentFMSID = _db.FlowStatusPsnContacts.Where(x => x.FSPCID == MaxFSPCIDID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);

                switch (ServiceTypeId)
                {
                    case (int)POR.Enum.ServiceType.RegOfficer:
                    case (int)POR.Enum.ServiceType.RegLadyOfficer:
                    case (int)POR.Enum.ServiceType.VolOfficer:
                    case (int)POR.Enum.ServiceType.VolLadyOfficer:

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
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OC" && x.Active == 1).Select(x => x.FlowGroupP2).FirstOrDefault();
                            RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                           && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                        }
                        break;

                    default:

                        //LHID=Null (actclk create record)
                        if (CurrentUserRole == null)
                        {
                            //Get First FMSID if Current FMSID is null
                            FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId && x.UserRoleID == (int)POR.Enum.UserRole.P3CLERK).Select(x => x.FMSID).First();

                        }
                        else
                        {
                            if (CurrentUserRole == (int)POR.Enum.UserRole.P3OIC || CurrentUserRole == (int)POR.Enum.UserRole.P3SNCO)
                            {
                                RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                                 && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                            }
                            else
                            {
                                //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                                var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AC" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                                RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                               && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }

        [HttpGet]
        public ActionResult IndexRejectPsnContact(string sortOrder, string currentFilter, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date : 2023/06/05
            /// Description : Index Page for Reject Personal Contact POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_PsnContactHeader> PsnConList = new List<_PsnContactHeader>();
            int serviceStatus = 0;


            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            //serviceStatus = 0;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId, x.UserRoleCategory }).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();

            if (UserInfo.UserRoleCategory == "P2")
            {
                serviceStatus = 1;
            }
            else
            {
                serviceStatus = 2;
            }

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = UserInfo.RoleId;
                

               
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallPsnContactRejectSP();
                //dt = objDALCommanQuery.CallGSQSP();

                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                var FMSID = _db.FlowManagementStatus.Where(x => (x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId) && (x.EstablishmentId == LocationId && x.UserRoleID == UserInfo.RoleId)).Select(x => x.FMSID).FirstOrDefault();

                TempData["RoleId"] = UserInfo.RoleId;

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    /// here send a serviceStatus as 1 is to categorized the officer and other rank seperettly
                    if (serviceStatus == 1)
                    {
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                        if (rows.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer ||
                                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                            if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P2SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P2OIC)
                            {
                                dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();
                            }
                            else
                            {
                                dt3 = dt2;
                            }
                        }                        

                    }
                    else
                    {
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                        if (rows.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                            if (UserInfo.RoleId == (int)POR.Enum.UserRole.P3CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P3SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P3OIC)
                            {
                                dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();
                            }
                            else
                            {
                                dt3 = dt2;
                            }
                        }                       

                    }


                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                        obj_PsnContactHeader.PCHID = Convert.ToInt32(dt3.Rows[i]["PCHID"]);
                        obj_PsnContactHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt3.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt3.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                        PsnConList.Add(obj_PsnContactHeader);

                    }
                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(PsnConList.ToPagedList(pageNumber, pageSize));
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
        public ActionResult IndexRejectPsnChild(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : Sqn ldr WAKY Wickramasinghe
            ///Created Date : 2023/06/05
            /// Description : Index Page for Reject Personal Contact POR

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_PsnContactHeader> PsnConList = new List<_PsnContactHeader>();
            int serviceStatus = 0;


            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            //serviceStatus = 0;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId, x.RoleId, x.UserRoleCategory }).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();

            if (UserInfo.UserRoleCategory == "P2")
            {
                serviceStatus = 1;
            }
            else
            {
                serviceStatus = 2;
            }

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                TempData["CurrentUserRole"] = UserInfo.RoleId;


                ViewBag.CurrentFilter = searchString;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallPsnChildRejectSP();
                //dt = objDALCommanQuery.CallGSQSP();

                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                var FMSID = _db.FlowManagementStatus.Where(x => (x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId) && (x.EstablishmentId == LocationId && x.UserRoleID == UserInfo.RoleId)).Select(x => x.FMSID).FirstOrDefault();

                TempData["RoleId"] = UserInfo.RoleId;

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    /// here send a serviceStatus as 1 is to categorized the officer and other rank seperettly
                    if (serviceStatus == 1)
                    {
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                        if (rows.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer ||
                                                       x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();

                            if (UserInfo.RoleId == (int)POR.Enum.UserRole.P2CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P2SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P2OIC)
                            {
                                dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();
                            }
                            else
                            {
                                dt3 = dt2;
                            }
                        }
                    }
                    else
                    {
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                        if (rows.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                            if (UserInfo.RoleId == (int)POR.Enum.UserRole.P3CLERK || UserInfo.RoleId == (int)POR.Enum.UserRole.P3SNCO || UserInfo.RoleId == (int)POR.Enum.UserRole.P3OIC)
                            {
                                dt3 = dt2.AsEnumerable().Where(x => x.Field<string>("Location") == LocationId).CopyToDataTable();
                            }
                            else
                            {
                                dt3 = dt2;
                            }
                        }
                    }


                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                        obj_PsnContactHeader.PCHID = Convert.ToInt32(dt3.Rows[i]["PCHID"]);
                        obj_PsnContactHeader.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt3.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt3.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                        PsnConList.Add(obj_PsnContactHeader);

                    }
                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(PsnConList.ToPagedList(pageNumber, pageSize));
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
        public ActionResult PrintPsnContact()
        {
            return View();
        }
        public ActionResult PrintChildDetails()
        {
            return View();
        }
        public bool ChecktheValidation(string ChildFullName, string ChildFullNameWithInitial, string Gender, string BirthPlace, string BirthCertificateNo)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/05/18
            /// Description : when creat the record, check the validation

            bool status = false;
       
            if (ChildFullName != null && ChildFullNameWithInitial != null && Gender != null && BirthPlace != null && BirthCertificateNo != null)
            {
                status = true;
            }
           
            return status;

        }
        public bool ChecktheValidation(DateTime? DateOfDeath, string DeathCertificateNo)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/05/18
            /// Description : when creat the record, check the validation

            bool status = false;

            if (DateOfDeath != null && DeathCertificateNo != "")
            {
                status = true;
            }

            return status;

        }
        public ActionResult Delete(int id)
        {
            ///Created BY   : Sqn ldr  WAKY Wickramasinghe
            ///Created Date : 2023/05/25
            ///Description : Delete the p3 Clerk and P2 entred data. All Active 1 record turn into 0/

            PsnContactHeader objPsnContactHeader = new PsnContactHeader();
            PsnContactDetail objPsnContactDetail = new PsnContactDetail();
            PsnChildDetail objPsnChildDetail = new PsnChildDetail();

            int UID_ = Convert.ToInt32(Session["UID"]);

            try
            {
                var subCatId = _db.PsnContactHeaders.Where(x => x.PCHID == id && x.Active == 1).Select(x => x.MasSubCatID).FirstOrDefault();

                objPsnContactHeader = _db.PsnContactHeaders.Find(id);
                objPsnContactHeader.Active = 0;
                objPsnContactHeader.ModifiedBy = UID_;
                objPsnContactHeader.ModifiedDate = DateTime.Now;
                objPsnContactHeader.ModifiedMac = MacAddress;

                _db.Entry(objPsnContactHeader).State = EntityState.Modified;

                switch (subCatId)
                {
                    case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                    case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                    case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                        var psnContactDetails = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();

                        objPsnContactDetail = _db.PsnContactDetails.Find(psnContactDetails);
                        objPsnContactDetail.Active = 0;
                        objPsnContactDetail.ModifiedBy = UID_;
                        objPsnContactDetail.ModifiedDate = DateTime.Now;
                        objPsnContactDetail.ModifiedMac = MacAddress;

                        _db.Entry(objPsnContactDetail).State = EntityState.Modified;

                        break;
                    default:

                        var psnChildDetails = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.PCDID).FirstOrDefault();

                        objPsnChildDetail = _db.PsnChildDetails.Find(psnChildDetails);
                        objPsnChildDetail.Active = 0;
                        objPsnChildDetail.ModifiedBy = UID_;
                        objPsnChildDetail.ModifiedDate = DateTime.Now;
                        objPsnChildDetail.ModifiedMac = MacAddress;

                        _db.Entry(objPsnChildDetail).State = EntityState.Modified;

                        break;
                }
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

                switch (subCatId)
                {
                    case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                    case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                    case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                        return RedirectToAction("Index", "PersonalContact");
                        
                    default:
                        return RedirectToAction("IndexChildInfo", "PersonalContact");                       
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Forward(int id, int? InOut_CAT_ID)
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
                var RecordInfo = _db.PsnContactHeaders.Where(x => x.PCHID == id).Select(x => new { x.Location, x.ServiceType,x.MasSubCatID,x.Sno }).FirstOrDefault();
                int? SubmitStatus = NextFlowStatusId(id, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceType, userInfo.DivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();                

                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();
               
                objFlowStatusPsnContact.PsnConHeaderID = id;
                objFlowStatusPsnContact.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                objFlowStatusPsnContact.UserID = UID;
                objFlowStatusPsnContact.FlowManagementStatusID = SubmitStatus;
                objFlowStatusPsnContact.RoleID = UserRoleId;
                objFlowStatusPsnContact.CreatedBy = UID;
                objFlowStatusPsnContact.CreatedDate = DateTime.Now;
                objFlowStatusPsnContact.CreatedMac = MacAddress;
                objFlowStatusPsnContact.IPAddress = this.Request.UserHostAddress;
                objFlowStatusPsnContact.Active = 1;

                ///This function is to update the Hrmis data base. After account one certified, the details will update p3hrmis and P2hrms 
                if (userInfo.RoleId == (int)POR.Enum.UserRole.ASORSOVRP3VOL || userInfo.RoleId == (int)POR.Enum.UserRole.ASORSOVRP2VOL)
                {
                    #region Switch Case

                        switch (userInfo.RoleId)
                        {
                            case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                                switch (RecordInfo.MasSubCatID)
                                {
                                    case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                    case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                                    var PsnContactInfo = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.TelMobileNo).FirstOrDefault();
                                    updateStatus = objDALCommanQuery.UpdateP3PsnConDetails(MacAddress, UID, RecordInfo.Sno, RecordInfo.MasSubCatID, PsnContactInfo);
                                    break;

                                case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                    var PsnEmailInfo = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.EmailAddress).FirstOrDefault();
                                    updateStatus = objDALCommanQuery.UpdateP3PsnConDetails(MacAddress, UID, RecordInfo.Sno, RecordInfo.MasSubCatID, PsnEmailInfo);

                                    break;
                                    default:
                                    updateStatus = UpdateP3ChildrenDetails(MacAddress, UID, RecordInfo.Sno, id);
                                    break;
                                }

                                break;
                            case (int)POR.Enum.UserRole.ASORSOVRP2VOL:

                                switch (RecordInfo.MasSubCatID)
                                {
                                    case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                    case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                                        var PsnContactInfo = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.TelMobileNo).FirstOrDefault();
                                        updateStatus = objDALCommanQueryP2.UpdateP2PsnConDetails(MacAddress, UID, RecordInfo.Sno, RecordInfo.MasSubCatID, PsnContactInfo);

                                        break;
                                    case (int)POR.Enum.PORMasterSubCategory.EMailAddress:

                                        var PsnEmailInfo = _db.PsnContactDetails.Where(x => x.PsnConHeaderID == id && x.Active == 1).Select(x => x.EmailAddress).FirstOrDefault();
                                        updateStatus = objDALCommanQueryP2.UpdateP2PsnConDetails(MacAddress, UID, RecordInfo.Sno, RecordInfo.MasSubCatID, PsnEmailInfo);

                                        break;
                                    default:
                                        updateStatus = UpdateP2ChildrenDetails(MacAddress, UID, RecordInfo.Sno, id);
                                        break;

                                }

                                break;
                        }
                    #endregion

                    if (updateStatus == true)
                    {
                        _db.FlowStatusPsnContacts.Add(objFlowStatusPsnContact);
                        if (_db.SaveChanges() > 0)
                        {
                            TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + " " + " & Update the HRMIS";
                            switch (RecordInfo.MasSubCatID)
                            {
                                case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                                case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                                case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                    return RedirectToAction("Index");
                              
                                default:
                                    return RedirectToAction("IndexChildInfo");
                                   
                            }
                            
                        }
                        else
                        {
                            TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                            return RedirectToAction("Index");
                        }
                    }
                }

                else
                {
                    _db.FlowStatusPsnContacts.Add(objFlowStatusPsnContact);
                    if (_db.SaveChanges() > 0)
                    {
                        TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole;
                        switch (RecordInfo.MasSubCatID)
                        {
                            case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                            case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:
                            case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                                return RedirectToAction("Index");
                               
                            default:
                                return RedirectToAction("IndexChildInfo");                               
                        }
                       
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
                        //var RecordInfo = _db.SecondaryDutyHeaders.Where(x => x.SDHID == IDs).Select(x => new { x.Location, x.ServiceTypeId }).FirstOrDefault();
                        var RecordInfo = _db.PsnContactHeaders.Where(x => x.PCHID == IDs).Select(x => new { x.Location, x.ServiceType}).FirstOrDefault();
                        int? SubmitStatus = NextFlowStatusId(IDs, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceType, userInfo.DivisionId);
                        //Get Next FlowStatus User Role Name for Add Successfull Msg

                        UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                        SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                        FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();

                        objFlowStatusPsnContact.PsnConHeaderID = IDs;
                        objFlowStatusPsnContact.RecordStatusID = (int)POR.Enum.RecordStatus.Forward;
                        objFlowStatusPsnContact.UserID = UID;
                        objFlowStatusPsnContact.FlowManagementStatusID = SubmitStatus;
                        objFlowStatusPsnContact.RoleID = UserRoleId;
                        objFlowStatusPsnContact.CreatedBy = UID;
                        objFlowStatusPsnContact.CreatedDate = DateTime.Now;
                        objFlowStatusPsnContact.CreatedMac = MacAddress;
                        objFlowStatusPsnContact.IPAddress = this.Request.UserHostAddress;
                        objFlowStatusPsnContact.Active = 1;

                        _db.FlowStatusPsnContacts.Add(objFlowStatusPsnContact);
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

        [HttpGet]
        public ActionResult IndividualSearchPsn(string sortOrder, string currentFilter, string SearchChildDetails, string SearchContactDetails, int? page)
        {
            ///Created BY   : Sqn Ldr WAKY Wickramasinghe        
            ///Created Date : 2023/06/07
            /// Description : Search details for Individual Search

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;
            long sno = 0;
            int PsnConHeaderID = 0;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_PsnContactHeader> PsnConList = new List<_PsnContactHeader>();


            ViewBag.CurrentSort = sortOrder;

            if (SearchChildDetails != null && SearchChildDetails != "")
            {
                //// This write to Search details

                page = 1;

                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SearchChildDetails).Select(x => x.SNo).FirstOrDefault();
                sno = Convert.ToInt64(Sno);

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                TempData["UserRole"] = UserInfo.RoleId;

                ViewBag.CurrentFilter = SearchChildDetails;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallChildDetailsSP(sno, PsnConHeaderID);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                        obj_PsnContactHeader.PCHID = Convert.ToInt32(dt2.Rows[i]["PCHID"]);
                        //obj_PsnContactHeader.FSPCID = Convert.ToInt32(dt2.Rows[i]["FSPCID"]);
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.UserRoleName = dt2.Rows[i]["UserRoleName"].ToString();
                        PsnConList.Add(obj_PsnContactHeader);

                    }

                }
                
            }
            else if(SearchContactDetails !=  null && SearchContactDetails != "")
            {

                //// This write to Search details

                page = 1;

                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SearchContactDetails).Select(x => x.SNo).FirstOrDefault();
                sno = Convert.ToInt64(Sno);

                var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
                TempData["UserRole"] = UserInfo.RoleId;

                ViewBag.CurrentFilter = SearchContactDetails;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallPersonalContatSP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert || x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                        obj_PsnContactHeader.PCHID = Convert.ToInt32(dt2.Rows[i]["PCHID"]);
                        //obj_PsnContactHeader.FSPCID = Convert.ToInt32(dt2.Rows[i]["FSPCID"]);
                        obj_PsnContactHeader.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_PsnContactHeader.Location = dt2.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_PsnContactHeader.UserRoleName = dt2.Rows[i]["UserRoleName"].ToString();
                        PsnConList.Add(obj_PsnContactHeader);

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
            return View(PsnConList.ToPagedList(pageNumber, pageSize));

            // return View(_db.Vw_Leave.ToList());
        }

        public ActionResult IndividualPsnDetails(int id)
        {
            ///Created BY   : Sqn ldr WAKY Wicramasinghe 
            ///Created Date : 2023/06/12
            /// Description : click Details button  after view details by select person 
            /// 

            int UID_ = 0;
            int? UserRoleId;
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            List<_PsnContactHeader> PsnConList = new List<_PsnContactHeader>(); 
            //int? CurrentStatusUserRole;           

            if (Session["UID"].ToString() != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;
                
                var recordInfo = _db.PsnContactHeaders.Where(x => x.PCHID == id && x.Active == 1).Select(x => new { x.MasSubCatID,x.Sno } ).FirstOrDefault();
                long ConSno = Convert.ToInt64(recordInfo.Sno);
                int PsnConHeaderID = 0;
                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                switch (recordInfo.MasSubCatID)
                {
                    case (int)POR.Enum.PORMasterSubCategory.MobileNo:
                    case (int)POR.Enum.PORMasterSubCategory.EMailAddress:
                    case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:                       
                        dt = objDALCommanQuery.CallPersonalContatSP(ConSno);


                        //dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("FSNOKCDID") == id).CopyToDataTable();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                            obj_PsnContactHeader.PCHID = Convert.ToInt32(dt.Rows[i]["PCHID"]);
                            //obj_PsnContactHeader.FSNOKCDID = Convert.ToInt32(dt2.Rows[i]["FSNOKCDID"]);
                            obj_PsnContactHeader.ServiceNo = dt.Rows[i]["ServiceNo"].ToString();
                            obj_PsnContactHeader.Rank = dt.Rows[i]["Rank"].ToString();
                            obj_PsnContactHeader.FullName = dt.Rows[i]["Name"].ToString();
                            //obj_NOKChangeDetails.Branch = dt2.Rows[i]["Branch"].ToString();
                            obj_PsnContactHeader.Location = dt.Rows[i]["Location"].ToString();
                            obj_PsnContactHeader.RefNo = dt.Rows[i]["RefNo"].ToString();
                            obj_PsnContactHeader.UserRoleName = dt.Rows[i]["UserRoleName"].ToString();
                            obj_PsnContactHeader.Comment = dt.Rows[i]["RejectComment"].ToString();
                            obj_PsnContactHeader.RSID = Convert.ToInt32(dt.Rows[i]["RecordStatusID"]);


                            if (Convert.ToInt32(dt.Rows[i]["RecordStatusID"]) == (int)POR.Enum.RecordStatus.Reject)
                            {
                                TempData["RecordStatusID"] = Convert.ToInt32(dt.Rows[i]["RecordStatusID"]);

                            }

                            if (dt.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                            {
                                obj_PsnContactHeader.FMSID = Convert.ToInt32(dt.Rows[i]["FlowManagementStatusID"]);
                            }


                            PsnConList.Add(obj_PsnContactHeader);
                        }
                       
                        break;
                    default:
                        
                        dt = objDALCommanQuery.CallChildDetailsSP(ConSno, PsnConHeaderID);


                        //dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("FSNOKCDID") == id).CopyToDataTable();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();

                            obj_PsnContactHeader.PCHID = Convert.ToInt32(dt.Rows[i]["PCHID"]);
                            //obj_PsnContactHeader.FSNOKCDID = Convert.ToInt32(dt2.Rows[i]["FSNOKCDID"]);
                            obj_PsnContactHeader.ServiceNo = dt.Rows[i]["ServiceNo"].ToString();
                            obj_PsnContactHeader.Rank = dt.Rows[i]["Rank"].ToString();
                            obj_PsnContactHeader.FullName = dt.Rows[i]["Name"].ToString();
                            //obj_NOKChangeDetails.Branch = dt2.Rows[i]["Branch"].ToString();
                            obj_PsnContactHeader.Location = dt.Rows[i]["Location"].ToString();
                            obj_PsnContactHeader.RefNo = dt.Rows[i]["RefNo"].ToString();
                            obj_PsnContactHeader.UserRoleName = dt.Rows[i]["UserRoleName"].ToString();
                            obj_PsnContactHeader.Comment = dt.Rows[i]["RejectComment"].ToString();
                            obj_PsnContactHeader.RSID = Convert.ToInt32(dt.Rows[i]["RecordStatusID"]);


                            if (Convert.ToInt32(dt.Rows[i]["RecordStatusID"]) == (int)POR.Enum.RecordStatus.Reject)
                            {
                                TempData["RecordStatusID"] = Convert.ToInt32(dt.Rows[i]["RecordStatusID"]);

                            }

                            if (dt.Rows[i]["FlowManagementStatusID"] != DBNull.Value)
                            {
                                obj_PsnContactHeader.FMSID = Convert.ToInt32(dt.Rows[i]["FlowManagementStatusID"]);
                            }


                            PsnConList.Add(obj_PsnContactHeader);
                        }
                        break;
                       
                }
                return View(PsnConList);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        public ActionResult AdvancedSearchPsnCon(string FromDate, string ToDate, string SearchLoc, string SearchLivingStatus, string RecordStatus, int? page, string currentFilterFDate, string currentFilterTDate, string currentFilterLocation, string currentFilterSubCat, string currentFilterRecStatus)
        {
            ///Created By   : Sqn Ldr WAKY Wickramasinghe
            ///Created Date :2023-06-12
            ///Des: Searching Option for history details

            DataTable dt = new DataTable();
            List<_PsnContactHeader> PsnList = new List<_PsnContactHeader>();
            int? UID = Convert.ToInt32(Session["UID"]);
            int MSCID;
            int recordType;


            UID = Convert.ToInt32(Session["UID"]);
            var userInfo = _db.UserInfoes.Where(x => x.UID == UID && x.Active == 1).Select(x => new { x.LocationId, x.RoleId }).FirstOrDefault();

            if (RecordStatus == null && page == null)
            {
                ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                ViewBag.DDL_PersonalContactCat = new SelectList(_db.PORMasterSubCategories.OrderBy(x => x.MSCID), "MSCID", "SubCategory");

                //ViewBag.DDL_InOutCategories = new SelectList(_db.LivingStatus, "LSID", "LivingStatusName");

                TempData["ErrMsg"] = "Please selecct the Record Status.";
            }
            else
            {
                recordType = Convert.ToInt32(RecordStatus);

                if (SearchLivingStatus == "")
                {
                    MSCID = 0;
                }
                else
                {
                    MSCID = Convert.ToInt32(SearchLivingStatus);
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
                    ViewBag.currentFilterCategory = currentFilterSubCat;
                    ViewBag.currentFilterRecStatus = currentFilterRecStatus;
                }

                try
                {
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");                    
                    ViewBag.DDL_PersonalContactCat = new SelectList(_db.PORMasterSubCategories.OrderBy(x => x.MSCID), "MSCID", "SubCategory");

                    if (page != 1)
                    {
                        ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

                        if (currentFilterFDate != null && currentFilterTDate != null && currentFilterLocation != null && currentFilterSubCat != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            MSCID = Convert.ToInt32(currentFilterSubCat);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                        }
                        else if (currentFilterFDate != null && currentFilterTDate != null && currentFilterSubCat != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = currentFilterTDate;
                            MSCID = Convert.ToInt32(currentFilterSubCat);
                            SearchLoc = "";
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                        }
                        else if (currentFilterFDate != null && currentFilterLocation != null && currentFilterSubCat != null && currentFilterRecStatus != null)
                        {
                            FromDate = currentFilterFDate;
                            ToDate = "";
                            SearchLoc = currentFilterLocation;
                            MSCID = Convert.ToInt32(currentFilterSubCat);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                        }
                        else if (currentFilterTDate != null && currentFilterLocation != null && currentFilterSubCat != null && currentFilterRecStatus != null)
                        {
                            FromDate = "";
                            ToDate = currentFilterTDate;
                            SearchLoc = currentFilterLocation;
                            MSCID = Convert.ToInt32(currentFilterSubCat);
                            recordType = Convert.ToInt32(currentFilterRecStatus);

                            dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
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
                            if (FromDate != "" && ToDate != "" && SearchLoc != "" && MSCID != 0 && recordType != 0)
                            {
                                dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                            }
                            else if (FromDate != "" && ToDate != "" && MSCID != 0 && recordType != 0)
                            {
                                SearchLoc = "";
                                dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                            }
                            else if (FromDate != "" && SearchLoc != "" && MSCID != 0 && recordType != 0)
                            {
                                ToDate = "";
                                dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
                            }
                            else if (ToDate != "" && SearchLoc != "" && MSCID != 0 && recordType != 0)
                            {
                                FromDate = "";
                                dt = objDALCommanQuery.getPsrSearchDetails(FromDate, ToDate, SearchLoc, MSCID, recordType, userInfo.RoleId);
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
                        _PsnContactHeader obj_PsnContactHeader = new _PsnContactHeader();
                        obj_PsnContactHeader.ServiceNo = dt.Rows[i]["ServiceNo"].ToString();
                        obj_PsnContactHeader.Rank = dt.Rows[i]["Rank"].ToString();
                        obj_PsnContactHeader.FullName = dt.Rows[i]["Name"].ToString();
                        //obj_PsnContactHeader.CategoryName = dt.Rows[i]["LivingStatusShortName"].ToString();
                        obj_PsnContactHeader.Authority = dt.Rows[i]["Authority"].ToString();
                        obj_PsnContactHeader.Location = dt.Rows[i]["Location"].ToString();
                        obj_PsnContactHeader.UserRoleName = dt.Rows[i]["UserRoleName"].ToString();
                        obj_PsnContactHeader.PCHID = Convert.ToInt32(dt.Rows[i]["PCHID"]);
                        PsnList.Add(obj_PsnContactHeader);

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
                return View(PsnList.ToPagedList(pageNumber, pageSize));
            }
            return View();

        }

        public int? NextFlowStatusId(int? PCHID, string UserEstablishmentId, string RecordEstablishmentId, int? ServiceTypeId, string UserDivisionId)
        {
            ///Created By   :Sqn Ldr Wickramasinghe
            ///Created Date :2023.05.25
            ///Des: get the next flow status id FMSID 5556

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxFSNOKCDID = _db.FlowStatusPsnContacts.Where(x => x.PsnConHeaderID == PCHID).Select(x => x.FSPCID).Max();
                int? CurrentFMSID = _db.FlowStatusPsnContacts.Where(x => x.FSPCID == MaxFSNOKCDID).Select(x => x.FlowManagementStatusID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? SubmitStatus = 0;


                int? UID = Convert.ToInt32(Session["UID"]);

                switch (ServiceTypeId)
                {
                    case (int)POR.Enum.ServiceType.RegOfficer:
                    case (int)POR.Enum.ServiceType.RegLadyOfficer:
                    case (int)POR.Enum.ServiceType.VolOfficer:
                    case (int)POR.Enum.ServiceType.VolLadyOfficer:

                        //LHID=Null (actclk create record)
                        if (CurrentUserRole == null)
                        {
                            //Get First FMSID if Current FMSID is null
                            int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();

                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.UserRoleID == RoleId).Select(x => x.SubmitStatus).FirstOrDefault();
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                        }
                        else if (CurrentUserRole == (int)POR.Enum.UserRole.P2SNCO)
                        {
                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                                && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId ||
                                    x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

                        }
                        else if (CurrentUserRole == (int)POR.Enum.UserRole.P2OIC)
                        {
                            //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OC" && x.Active == 1).Select(x => x.FlowGroupP2).FirstOrDefault();

                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                   && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                            
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).FirstOrDefault();
                            
                        }
                        else
                        {
                            //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "OC" && x.Active == 1).Select(x => x.FlowGroupP2).FirstOrDefault();
                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                           && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                            //SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                            if (SubmitStatus == (int)POR.Enum.UserRole.ASORSOVRP2VOL)
                            {
                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.ASORSOVRP2VOL && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();

                            }                            
                            else
                            {                                
                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup && (x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).FirstOrDefault();
                            }
                        }
                        break;

                    default:

                        
                        if (CurrentUserRole == null)
                        {
                            //Get First FMSID if Current FMSID is null
                            int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();

                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.UserRoleID == RoleId).Select(x => x.SubmitStatus).FirstOrDefault();
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                        }
                        else if (CurrentUserRole == (int)POR.Enum.UserRole.P3SNCO)
                        {
                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole  && (x.EstablishmentId == RecordEstablishmentId
                                                                 && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || 
                                    x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();


                        }
                        else if (CurrentUserRole == (int)POR.Enum.UserRole.P3OIC)
                        {
                            //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AC" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();

                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup &&(x.EstablishmentId == RecordEstablishmentId  && x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus ).FirstOrDefault();
                                            
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).FirstOrDefault();
                            
                        }                        
                        else
                        {
                            //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AC" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                           && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                            //SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                            if (SubmitStatus == (int)POR.Enum.UserRole.ASORSOVRP3VOL)
                            {
                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.ASORSOVRP3VOL && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();

                            }                            
                            else
                            {
                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup &&  (x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).FirstOrDefault();

                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        public bool UpdateHrmis(string MacAddress, int? UID, long? Sno, int? gsqHeaderId)
        {

            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2023/02/03
            /// Description : update the hrmis

            DataTable dt = new DataTable();
            DataTable civildt = new DataTable();
            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.gsqAllocateVacanr;
            bool status = false;
            bool status2 = false;
            bool status3 = false;

            string SSNo = Convert.ToString(Sno);
            try
            {
                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(gsqHeaderId, detailsCollectCategory);

                ///Generate NOK ID
                long key = objDALCommanQuery.GenerateKey("NOK_Change_Details", "NOKID");

                string NokID = Convert.ToString(Sno + "/" + key);

                /// Update the perivous NoK type into 2                
                status = objDALCommanQuery.UpdatePreviousNokTypeId(Sno, UID, MacAddress);

                if (status == true)
                {
                    /// Insert new record to NOKChangeHeader table in P3 hrmis data based
                    //status2 = InsertNewNokRecordToHRMIS(NokID, Sno.ToString(), UID, dt);
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
        public bool UpdateP2ChildrenDetails(string MacAddress, int? UID, long? SNo,int PsnConHeaderID)
        {
            ///Created BY   : Sqn ldr Wickramsinghe
            ///Created Date : 2023/07/07
            /// Description : update the P2 hrms related to Children Info

            
            DataTable dt = new DataTable();
            string ConSno = SNo.ToString();
            bool status;            
            try
            {

                var childLiveStatus = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == PsnConHeaderID && x.Active == 1).Select(x => x.SubCategoryID).FirstOrDefault();
                var RecordLoc = _db.PsnContactHeaders.Where(x => x.PCHID == PsnConHeaderID && x.Active == 1).Select(x => new { x.Location,x.ServiceType } ).FirstOrDefault();
                var ServiceNo = _db.Vw_PersonalDetail.Where(x => x.SNo == ConSno).Select(x => x.ServiceNo).FirstOrDefault();
                /// Get Nok details
                dt = objDALCommanQuery.CallChildDetailsSP(0, PsnConHeaderID);

                BAL_PsnContactHeader objPsnDetails = new BAL_PsnContactHeader();
                foreach (DataRow row in dt.Rows)
                {
                    
                    objPsnDetails.PCHID = PsnConHeaderID;
                    objPsnDetails.SNO = Convert.ToString(SNo);
                    objPsnDetails.ServiceNo = ConSno;
                    objPsnDetails.RefNo = row["RefNo"].ToString();
                    objPsnDetails.Authority = row["Authority"].ToString();
                    objPsnDetails.SCID = childLiveStatus;

                    switch (childLiveStatus)
                    {
                        case (int)POR.Enum.PorSubCategory.Live:
                            objPsnDetails.ChildFullName = row["ChildFullName"].ToString();
                            objPsnDetails.ChildFullNameWithInitial = row["ChildFullNameWithInitial"].ToString();
                            objPsnDetails.Location = RecordLoc.Location;
                            objPsnDetails.BirthCertificateNo = row["BirthCertificateNo"].ToString();
                            objPsnDetails.BirthPlace = row["BirthPlace"].ToString();
                            objPsnDetails.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
                            objPsnDetails.Gender = Convert.ToInt32(row["Gender"]);
                            objPsnDetails.ServiceType = Convert.ToInt32(RecordLoc.ServiceType);
                            objPsnDetails.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                            break;
                        case (int)POR.Enum.PorSubCategory.Death:
                            objPsnDetails.ChildFullName = row["ChildFullName"].ToString();
                            objPsnDetails.ChildFullNameWithInitial = row["ChildFullNameWithInitial"].ToString();
                            objPsnDetails.DeathCertificateNo = row["DeathCertificateNo"].ToString();
                            objPsnDetails.DateOfDeath = Convert.ToDateTime(row["DateOfDeath"]);
                            objPsnDetails.GenderType = row["GenderType"].ToString();
                            objPsnDetails.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                            break;
                    }

                }               


                status = objDALCommanQueryP2.UpdateP2PsnChildDetails(MacAddress, UID, objPsnDetails);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public bool UpdateP3ChildrenDetails(string MacAddress, int? UID, long? SNo, int PsnConHeaderID)
        {
            ///Created BY   : Sqn ldr Wickramsinghe
            ///Created Date : 2023/07/10
            /// Description : update the P3 hrmis related to Children Info


            DataTable dt = new DataTable();
            string ConSno = SNo.ToString();
            bool status;
            try
            {

                var childLiveStatus = _db.PsnChildDetails.Where(x => x.PsnConHeaderID == PsnConHeaderID && x.Active == 1).Select(x => x.SubCategoryID).FirstOrDefault();
                var ServiceNo = _db.Vw_PersonalDetail.Where(x => x.SNo == ConSno).Select(x => x.ServiceNo).FirstOrDefault();
                /// Get Nok details
                dt = objDALCommanQuery.CallChildDetailsSP(0, PsnConHeaderID);

                BAL_PsnContactHeader objPsnDetails = new BAL_PsnContactHeader();
                foreach (DataRow row in dt.Rows)
                {

                    objPsnDetails.PCHID = PsnConHeaderID;
                    objPsnDetails.SNO = Convert.ToString(SNo);
                    objPsnDetails.ServiceNo = ConSno;
                    objPsnDetails.RefNo = row["RefNo"].ToString();
                    objPsnDetails.Authority = row["Authority"].ToString();
                    objPsnDetails.SCID = childLiveStatus;

                    switch (childLiveStatus)
                    {
                        case (int)POR.Enum.PorSubCategory.Live:
                            objPsnDetails.Disrict2 = row["District"].ToString();
                            var ChildDist = _dbCommonData.Districts.Where(x => x.DESCRIPTION == objPsnDetails.Disrict2).Select(x => x.DIST_CODE).FirstOrDefault();
                            objPsnDetails.ChildFullName = row["ChildFullName"].ToString();
                            objPsnDetails.ChildFullNameWithInitial = row["ChildFullNameWithInitial"].ToString();
                            objPsnDetails.District = ChildDist;
                            objPsnDetails.BirthCertificateNo = row["BirthCertificateNo"].ToString();
                            objPsnDetails.BirthPlace = row["BirthPlace"].ToString();
                            objPsnDetails.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
                            objPsnDetails.Gender = Convert.ToInt32(row["Gender"]);
                            objPsnDetails.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                            break;
                        case (int)POR.Enum.PorSubCategory.Death:
                            objPsnDetails.ChildFullName = row["ChildFullName"].ToString();
                            objPsnDetails.ChildFullNameWithInitial = row["ChildFullNameWithInitial"].ToString();
                            objPsnDetails.DeathCertificateNo = row["DeathCertificateNo"].ToString();
                            objPsnDetails.DateOfDeath = Convert.ToDateTime(row["DateOfDeath"]);
                            objPsnDetails.GenderType = row["GenderType"].ToString();
                            objPsnDetails.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                            break;
                    }
                }

                status = objDALCommanQuery.UpdateP3PsnChildDetails(MacAddress, UID, objPsnDetails);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return status;
        }
        public string PorNoCreate(string EstablishmentId, int? serviceType,int MasSubCatID)
        {
            ///Created BY   : Sqn Ldr WAKY Wickaramsinghe
            ///Created Date : 2021/02/25
            /// Description : create POR Number 

            try
            {
                int currentmonth = Convert.ToInt32(DateTime.Now.Month);
                int currentyear = Convert.ToInt32(DateTime.Now.Year);
                int currentdate = Convert.ToInt32(DateTime.Now.Day);
                string RefNo = "";
                int jobcount = 0;
                int RocordId = 0;


                 jobcount = _db.NOKChangeHeaders.Where(x => x.Location == EstablishmentId && x.NOKStatus == (int)POR.Enum.NOKtatus.ChangeNOKOnly && x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1).Count();
                 RocordId = jobcount + 1;

                //_LivingInOut obj_LivingInOut = new _LivingInOut();
                if (serviceType == (int)POR.Enum.ServiceType.RegOfficer || serviceType == (int)POR.Enum.ServiceType.RegLadyOfficer || serviceType == (int)POR.Enum.ServiceType.VolOfficer || serviceType == (int)POR.Enum.ServiceType.VolLadyOfficer)
                {
                    switch (MasSubCatID)
                    {
                        case (int)POR.Enum.PORMasterSubCategory.MobileNo:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.MobileNo && 
                                       x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "OC-6" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;

                        case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo &&
                                       x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "OC-7" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.EMailAddress:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.EMailAddress &&
                                       x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "OC-8" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth &&
                                        x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "OC-9" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;                        
                    }                    
                }
                else
                {
                    switch (MasSubCatID)
                    {
                        case (int)POR.Enum.PORMasterSubCategory.MobileNo:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.MobileNo &&
                                        x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "AC-6" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.ResidentialTeleNo &&
                                        x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "AC-7" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.EMailAddress:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.EMailAddress &&
                                        x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "AC-8" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                        case (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth:

                            jobcount = _db.PsnContactHeaders.Where(x => x.Location == EstablishmentId && x.MasSubCatID == (int)POR.Enum.PORMasterSubCategory.DetailOfChildBirth &&
                                        x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1 && x.ServiceType == serviceType).Count();
                            RocordId = jobcount + 1;

                            RefNo = EstablishmentId + "/" + "AC-9" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                            break;
                    }                    
                }

                return RefNo;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void InserFlowStatus(int PCHID, int RoleId, int UID_, int? FMSID, int? RSID)
        {
            ///Created BY   : Sqn ldr Wickramasinghe
            ///Created Date : 2023/05/17
            /// Description : Insert personal contact details flow status in to FlowStatusPsnContact table

            try
            {
                FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();


                objFlowStatusPsnContact.PsnConHeaderID = PCHID;
                objFlowStatusPsnContact.RecordStatusID = RSID;
                objFlowStatusPsnContact.UserID = UID;
                objFlowStatusPsnContact.FlowManagementStatusID = FMSID;
                objFlowStatusPsnContact.RoleID = RoleId;
                objFlowStatusPsnContact.CreatedBy = UID;
                objFlowStatusPsnContact.CreatedMac = MacAddress;
                objFlowStatusPsnContact.IPAddress = this.Request.UserHostAddress;
                objFlowStatusPsnContact.CreatedDate = DateTime.Now;
                objFlowStatusPsnContact.Active = 1;

                _db.FlowStatusPsnContacts.Add(objFlowStatusPsnContact);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region Json Method

        [HttpPost]
        public JsonResult RejectRecord(string id, int PCHID, int FMSID)
        {
            ///Created BY   : SQn Ldr Wickramasinghe 
            ///Created Date : 2023/06/05
            /// Description : this function is to reject the record

            string message = "";
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                var userInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x => new { x.LocationId, x.DivisionId }).FirstOrDefault();
                var recordInfo = _db.PsnContactHeaders.Where(x => x.PCHID == PCHID).Select(x => new { x.Location, x.ServiceType }).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(PCHID, userInfo.LocationId, recordInfo.Location, userInfo.DivisionId, recordInfo.ServiceType);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == recordInfo.Location || x.EstablishmentId == userInfo.LocationId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();
                objFlowStatusPsnContact.PsnConHeaderID = PCHID;
                objFlowStatusPsnContact.FlowManagementStatusID = PreviousFMSID;
                objFlowStatusPsnContact.CreatedBy = UID;
                objFlowStatusPsnContact.UserID = UID;


                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//
                objFlowStatusPsnContact.RecordStatusID = (int)POR.Enum.RecordStatus.Reject;
                objFlowStatusPsnContact.Comment = id;
                objFlowStatusPsnContact.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objFlowStatusPsnContact.CreatedMac = MacAddress;
                objFlowStatusPsnContact.Active = 1;
                objFlowStatusPsnContact.IPAddress = this.Request.UserHostAddress;
                _db.FlowStatusPsnContacts.Add(objFlowStatusPsnContact);

                if (_db.SaveChanges() > 0)
                {
                    if (UserRoleId >= 5)
                    {
                        message = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + recordInfo.Location;
                    }
                    else
                    {
                        message = "Successfully Rejected to " + PreviousFlowStatus_UserRole + " - " + recordInfo.Location;
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
        public JsonResult getname(string id)
        {
            ///Created BY   : Sqn ldr WAKY Wickaramasinghe
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
        public JsonResult getLiveChildrenName(string id)
        {
            ///Created BY   : Sqn ldr Wickaramasinghe
            ///Created Date : 2023/05/17
            /// Description : Load live Status childern details

            List<_PsnContactHeader> PsnContactt = new List<_PsnContactHeader>();
            var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            var FromChildernList = this.LinqChildrenList(Sno);

            var ChildernNameListData = FromChildernList.Select(x => new SelectListItem()
            {
                Text = x.ChildFullName.ToString(),
                Value = x.PCDID.ToString(),
            });


            return Json(ChildernNameListData, JsonRequestBehavior.AllowGet);
        }
        public IList<PsnChildDetail> LinqChildrenList(string id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/14
            /// Description : Load Section
            long conSno = Convert.ToInt64(id);

            List<PsnChildDetail> nameList = new List<PsnChildDetail>();
            PsnChildDetail obj = new PsnChildDetail();
            var Result = (from pch in _db.PsnContactHeaders
                          join pchd in _db.PsnChildDetails on pch.PCHID equals pchd.PsnConHeaderID
                          where pch.Sno == conSno
                          select new
                          {
                              //pch.Sno,
                              pchd.ChildFullName,
                              pchd.PCDID
                          }).ToList();

            if (Result.Count != 0)
            {
                foreach (var item in Result)
                {
                    obj.ChildFullName = item.ChildFullName;
                    obj.PCDID = item.PCDID;
                }
            }
            else
            {
                obj.ChildFullName = "No Childern record to found";
            }

            // Result = _dbCommonData.GSDivisions.Where(x => x.District == id).OrderBy(x => x.GSName).ToList();
            nameList.Add(obj);

            return nameList;
        }
        
        #endregion

    }
}