using POR.Models;
using POR.Models.LivingInOut;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Transactions;
using System.Data.Entity;
using PagedList;
using System.Threading.Tasks;
using POR.Models.SecondaryDuty;

namespace POR.Controllers
{
    public class SecondaryDutyController : Controller
    {
        dbContext _db = new dbContext();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        P3HRMS _dbP3HRMS = new P3HRMS();
        DALCommanQueryP2 objDALCommanQueryP2 = new DALCommanQueryP2();
        string MacAddress = new DALBase().GetMacAddress();



        [HttpGet]
        public ActionResult Create()
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            ///Des: Load the page of create page of Secondary Duty
            ///
            string EstablishmentId1 = "";
            int UID_1 = 0;
            UID_1 = Convert.ToInt32(Session["UID"]);
            EstablishmentId1 = _db.UserInfoes.Where(x => x.UID == UID_1 && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1 && x.Camp == EstablishmentId1), "DutyLocId", "CampDutyLocation");
            ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
            //ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1), "DutyLocId", "CampDutyLocation");
            ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus.Where(x => x.SDSID == 1), "SDSID", "StatusName");
            ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");

            if (Session["UID"] != null)
            {
                int UID_ = Convert.ToInt32(Session["UID"]);
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
            }
            return View();

        }
        [HttpPost]
        public ActionResult Create(_SecondaryDuty obj_SecondaryDuty, string btnName, string FromDate, string Remarks, string WEDate, string CeasedDate)
        {

            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            ///Des: Firts step data entering code
            SecondaryDutyHeader objSecondaryDutyHeader = new SecondaryDutyHeader();
            SecondaryDutyDetail objSecondaryDutyDetail = new SecondaryDutyDetail();


            obj_SecondaryDuty.RSID = 1000;


            string EstablishmentId = "";
            string DivisionId = "";
            int UID_ = 0;
            int RoleId = 0;
            string EstablishmentId1 = "";
            int UID_1 = 0;
            int DutyCount= 0;
            int OicCount = 0;
            var ServiceType = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber).Select(x => x.service_type).FirstOrDefault();
            int Appointment = Convert.ToInt32(obj_SecondaryDuty.Appointment);
            var AppointmentType = _db.SecDutyAppointments.Where(x => x.AppointmentID == Appointment).Select(x => x.Appointment).FirstOrDefault();
            var SNO = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber).Select(x => x.SNo).FirstOrDefault();
            //ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1), "DutyLocId", "CampDutyLocation");
            long ConSNo = Convert.ToInt64(SNO);
            int AreaCode = Convert.ToInt32(obj_SecondaryDuty.AreaforResponsibility);
            var Area = _db.DutyLocations.Where(x => x.DutyLocId == AreaCode).Select(x => x.CampDutyLocation).FirstOrDefault();
            ///Get District name
            int SDHIDNew = Convert.ToInt32(obj_SecondaryDuty.SDHID);
            var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_SecondaryDuty.District).Select(x => x.DESCRIPTION).FirstOrDefault();
            UID_1 = Convert.ToInt32(Session["UID"]);
            EstablishmentId1 = _db.UserInfoes.Where(x => x.UID == UID_1 && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1 && x.Camp == EstablishmentId1), "DutyLocId", "CampDutyLocation");
            var DutyLoc = _db.SecondaryDutyHeaders.Where(x => x.Location == EstablishmentId1).Select(x => x.Location).FirstOrDefault();
            OicCount= _db.SecondaryDutyHeaders.Where(x => x.SDHID == SDHIDNew).Select(x => x.OicProces).Count();
            if (EstablishmentId1== DutyLoc)
            {
                 DutyCount = _db.SecondaryDutyDetails.Where(x => x.AreaForResponsibility == Area && x.Appointment == AppointmentType && x.Active==1).Count();
            }
            if (DutyCount < 1)
            {
                try
                {
                    if (Session["UID"] != null)
                    {
                        UID_ = Convert.ToInt32(Session["UID"]);
                        EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
                        DivisionId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.DivisionId).FirstOrDefault();
                        RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
                        ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                        ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus.Where(x => x.SDSID == 1), "SDSID", "StatusName");
                        ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");
                        string CreatePorNo = PorNoCreate(EstablishmentId, obj_SecondaryDuty.InOut_CAT_ID);

                        ModelState.Remove("RelationshipID");
                        ModelState.Remove("District");
                        ModelState.Remove("District1");
                        ModelState.Remove("Marriage_Status");
                        ModelState.Remove("ServiceNo");
                        ModelState.Remove("ToDate");
                        ModelState.Remove("FromDate");
                        ModelState.Remove("NokID");

                        bool validity = ChecktheValidation(obj_SecondaryDuty.Authority, FromDate, WEDate, CeasedDate);

                        if (validity == true)
                        {
                            objSecondaryDutyHeader.Sno = ConSNo;
                            if (obj_SecondaryDuty.SDSID == "1")
                            {
                                objSecondaryDutyHeader.WFDate = Convert.ToDateTime(WEDate);
                            }

                            objSecondaryDutyHeader.Location = EstablishmentId;
                            objSecondaryDutyHeader.ServiceTypeId = ServiceType;
                            objSecondaryDutyHeader.RefNo = CreatePorNo;
                            objSecondaryDutyHeader.Authority = obj_SecondaryDuty.Authority;
                            objSecondaryDutyHeader.CreatedDate = DateTime.Now;
                            objSecondaryDutyHeader.CreatedBy = Convert.ToString(UID_);
                            objSecondaryDutyHeader.CreatedMac = MacAddress;
                            objSecondaryDutyHeader.CreateIpAddess = this.Request.UserHostAddress;
                            objSecondaryDutyHeader.Active = 1;
                            objSecondaryDutyHeader.OicProces = 0;

                            _db.SecondaryDutyHeaders.Add(objSecondaryDutyHeader);
                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                if (_db.SaveChanges() > 0)
                                {
                                    ///get the LIOHID
                                    var LIOHID = _db.SecondaryDutyHeaders.Where(x => x.Sno == ConSNo
                                               && x.Location == EstablishmentId && x.Active == 1 && x.RefNo == CreatePorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.SDHID).FirstOrDefault();


                                    objSecondaryDutyDetail.SecondaryDutyHeaderID = LIOHID;
                                    objSecondaryDutyDetail.AreaForResponsibility = Area;
                                    objSecondaryDutyDetail.Appointment = AppointmentType;
                                    objSecondaryDutyDetail.DutyStatus = Convert.ToInt32(obj_SecondaryDuty.DutyStatus);
                                    if (obj_SecondaryDuty.SDSID == "1")
                                    {
                                        objSecondaryDutyDetail.WithEffectDate = Convert.ToDateTime(WEDate);
                                    }


                                    objSecondaryDutyDetail.TakingOverSno = Convert.ToInt32(obj_SecondaryDuty.Snumber2);

                                    if (obj_SecondaryDuty.SDSID == "2")
                                    {
                                        objSecondaryDutyDetail.TakingOverWithEffectDate = Convert.ToDateTime(FromDate);
                                        objSecondaryDutyDetail.CeasedDate = Convert.ToDateTime(CeasedDate);
                                    }

                                    objSecondaryDutyDetail.CreatedBy = Convert.ToString(UID_);
                                    objSecondaryDutyDetail.CreatedDate = DateTime.Now;
                                    objSecondaryDutyDetail.CreatedMac = MacAddress;
                                    objSecondaryDutyDetail.CreateIpAddess = this.Request.UserHostAddress;
                                    objSecondaryDutyDetail.Active = 1;

                                    _db.SecondaryDutyDetails.Add(objSecondaryDutyDetail);
                                    /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                    InserFlowStatus(LIOHID, RoleId, UID_, obj_SecondaryDuty.FMSID, obj_SecondaryDuty.RSID);

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
                            TempData["ErrMsg"] = "Input Fields Can not be empty. Please Fill all the Fields.";
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
            }
            else
            {
                TempData["ErrMsg"] = "Already insert Secondary duty. First Ceased the duty";
                return RedirectToAction("Create", "SecondaryDuty");
            }
            //ModelState.Clear();
            return View();
        }



        [HttpGet]
        public ActionResult CreateLocation()
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            ///Des: Load the page of create page of Secondary Duty
            ///


            ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
            ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1), "DutyLocId", "CampDutyLocation");
            ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus.Where(x => x.SDSID == 1), "SDSID", "StatusName");
            ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");

            if (Session["UID"] != null)
            {
                int UID_ = Convert.ToInt32(Session["UID"]);
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
            }
            return View();

        }



        [HttpPost]
        public ActionResult CreateLocation(_SecondaryDuty obj_SecondaryDuty, string btnName, string FromDate, string Remarks, string WEDate, string CeasedDate)
        {

            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            ///Des: Firts step data entering code
            SecondaryDutyHeader objSecondaryDutyHeader = new SecondaryDutyHeader();
            SecondaryDutyDetail objSecondaryDutyDetail = new SecondaryDutyDetail();
            DutyLocation objDutyLocation = new DutyLocation();

            obj_SecondaryDuty.RSID = 1000;


            string EstablishmentId = "";
            string DivisionId = "";
            int UID_ = 0;
            int RoleId = 0;

            var ServiceType = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber).Select(x => x.service_type).FirstOrDefault();
            int Appointment = Convert.ToInt32(obj_SecondaryDuty.Appointment);
            var AppointmentType = _db.SecDutyAppointments.Where(x => x.AppointmentID == Appointment).Select(x => x.Appointment).FirstOrDefault();
            var SNO = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber).Select(x => x.SNo).FirstOrDefault();

            long ConSNo = Convert.ToInt64(SNO);

            ///Get District name
            var DistName = _dbCommonData.Districts.Where(x => x.DIST_CODE == obj_SecondaryDuty.District).Select(x => x.DESCRIPTION).FirstOrDefault();
            try
            {
                if (Session["UID"] != null)
                {
                    UID_ = Convert.ToInt32(Session["UID"]);
                    EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
                    DivisionId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.DivisionId).FirstOrDefault();
                    RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
                    ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
                    ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus.Where(x => x.SDSID == 1), "SDSID", "StatusName");
                    ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");
                   // string CreatePorNo = PorNoCreate(EstablishmentId, obj_SecondaryDuty.InOut_CAT_ID);

                    ModelState.Remove("RelationshipID");
                    ModelState.Remove("District");
                    ModelState.Remove("District1");
                    ModelState.Remove("Marriage_Status");
                    ModelState.Remove("ServiceNo");
                    ModelState.Remove("ToDate");
                    ModelState.Remove("FromDate");
                    ModelState.Remove("NokID");

                   

                    if (obj_SecondaryDuty.DutyLocation != null)
                    {
                        
                        
                        
                        
                                objDutyLocation.CampDutyLocation =  obj_SecondaryDuty.DutyLocation.ToUpper();
                                objDutyLocation.Camp = EstablishmentId;
                                objDutyLocation.Active = 1;
                                

                                _db.DutyLocations.Add(objDutyLocation);
                            /// This function is to  Enter intial record Status in to FlowStatusDetails 

                            if (_db.SaveChanges() > 0)
                            {

                                TempData["ScfMsg"] = "Data Successfully Saved.";
                                return RedirectToAction("CreateLocation", "SecondaryDuty");
                        }

                        
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Input Fields Can not be empty. Please Fill all the Fields.";
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
       

        public bool ChecktheValidation(string Authority, string FromDate, string WEFDate, string CeasedDate)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            /// Description : when creat the record, check the validation

            bool status = false;

            if (Authority != null && FromDate != "")
            {
                status = true;
            }
            if (Authority != null && WEFDate != "")
            {
                status = true;
            }

            return status;

        }





        ////////////////////////////  accont wise
        public ActionResult IndexConfirm(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            /// Description : Index Page for Forward Load Person Contact Details            

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            int serviceStatus = 0;

            List<_SecondaryDuty> SecondaryDutyHeaderList = new List<_SecondaryDuty>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

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

                dt = objDALCommanQuery.CallDutySP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("OicProces") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("OicProces") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                }
                switch (UserInfo.RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:
                    case (int)POR.Enum.UserRole.P2CLERK:
                        dt3 = loadDataUserWiseDuty(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                        break;
                    //case (int)POR.Enum.UserRole.P3SNCO:
                    //case (int)POR.Enum.UserRole.P2SNCO:
                    //    dt3 = loadDataUserWiseDuty(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.P3OIC:
                    //case (int)POR.Enum.UserRole.P2OIC:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.KOPNR:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.SNCOSALARY:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.WOSALARY:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                    //case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.HRMSSNCO:
                    //case (int)POR.Enum.UserRole.HRMSP2SNCO:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                    //case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    //case (int)POR.Enum.UserRole.ACCOUNTS01:
                    //    dt3 = loadDataUserWise(RoleId, dt2, UserInfo.LocationId, UID, serviceStatus);
                    //    break;
                    default:
                        break;
                }

                for (int i = 0; i < dt3.Rows.Count; i++)

                {
                    _SecondaryDuty objSecondaryDutyHeader = new _SecondaryDuty();
                    objSecondaryDutyHeader.SDHID = (dt3.Rows[i]["SDHID"].ToString());
                    objSecondaryDutyHeader.ServiceNo = (dt3.Rows[i]["ServiceNo"].ToString());
                    objSecondaryDutyHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                    objSecondaryDutyHeader.FullName = dt3.Rows[i]["Name"].ToString();
                    objSecondaryDutyHeader.Location = dt3.Rows[i]["Location"].ToString();
                    objSecondaryDutyHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    objSecondaryDutyHeader.AreaforResponsibility = dt3.Rows[i]["AreaforResponsibility"].ToString();
                    objSecondaryDutyHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);

                    SecondaryDutyHeaderList.Add(objSecondaryDutyHeader);

                }
                pageSize = 100;
                pageNumber = (page ?? 1);
                return View(SecondaryDutyHeaderList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }





        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            /// Description : Index Page for Forward Load Person Contact Details            

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            int serviceStatus = 0;

            List<_SecondaryDuty> SecondaryDutyHeaderList = new List<_SecondaryDuty>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

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

                dt = objDALCommanQuery.CallDutySP(sno);

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                if (resultStatus.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward || x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
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
                    _SecondaryDuty objSecondaryDutyHeader = new _SecondaryDuty();
                    objSecondaryDutyHeader.SDHID = (dt3.Rows[i]["SDHID"].ToString());
                    objSecondaryDutyHeader.ServiceNo = (dt3.Rows[i]["ServiceNo"].ToString());
                    objSecondaryDutyHeader.Rank = dt3.Rows[i]["Rank"].ToString();
                    objSecondaryDutyHeader.FullName = dt3.Rows[i]["Name"].ToString();
                    objSecondaryDutyHeader.Location = dt3.Rows[i]["Location"].ToString();
                    objSecondaryDutyHeader.RefNo = dt3.Rows[i]["RefNo"].ToString();
                    objSecondaryDutyHeader.AreaforResponsibility = dt3.Rows[i]["AreaforResponsibility"].ToString();
                    objSecondaryDutyHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);

                    SecondaryDutyHeaderList.Add(objSecondaryDutyHeader);

                }
                pageSize = 20;
                pageNumber = (page ?? 1);
                return View(SecondaryDutyHeaderList.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }

        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID, int serviceStatus)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
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
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).ToList();
                        //var result = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();
                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
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
                        result = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                /// here send a serviceStatus as 1 from index function. this is to categorized the officer and other rank seperettly
                                if (serviceStatus == 1)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));

                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                    }

                                }
                                else
                                {
                                    var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen
                                           || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                    if (row.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen
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

                                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer));
                                            if (rows.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                                      (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                            }

                                            break;
                                        default:
                                            var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                            if (row.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
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

                                            var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer));

                                            if (rows.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer)).CopyToDataTable();
                                            }
                                            break;

                                        default:

                                            var row = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                        (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                        x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen));

                                            if (row.Any())
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
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

                                            Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                           (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                           x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                                            if (Count != 0)
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                                                                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).CopyToDataTable();
                                            }

                                            break;

                                        default:

                                            Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                            (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                            if (Count != 0)
                                            {
                                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
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

        public DataTable loadDataUserWiseDuty(int RoleId, DataTable dt, string LocationId, int? UID, int serviceStatus)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06
            /// Description : Load data user roll wise, this fuction call from Index()        

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                //int serviceStatus = 1;



                #region CodeArea
                /// Check the data table has row or not
                var result = dt.AsEnumerable().Where(x => x.Field<int>("OicProces") == 1 && x.Field<string>("Location") == LocationId).ToList();
                //var result = dt.AsEnumerable().Where(x => x.Field<int>("RoleID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert).ToList();
                if (result.Count != 0)
                {
                    dt2 = dt.AsEnumerable().Where(x => x.Field<int>("OicProces") == 1 && x.Field<string>("Location") == LocationId).CopyToDataTable();
                }

                return dt2;
            }


            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        //////////////////////////
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="RSID"></param>
        /// <returns></returns>





        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created By   : Flt Lt Priyantha
            ///Created Date :2024.01.06 
            /// Description : Load data user roll wise, this fuction call from IndexLivingINOutStatus()

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();


                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P3CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).ToList();

                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("Location") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();
                        }
                        break;
                    #endregion

                    default:

                        #region CodeArea

                        /// Data Table first row CurrentStatus gave null value, hence it occures an error. Because of that check the column and delete that row, 
                        /// [30] means null coloumn number

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();

                        for (int x = 0; x < dt.Rows.Count; x++)
                        {
                            if (dt.Rows[x][30] == DBNull.Value)
                            {
                                dt.Rows[x].Delete();
                            }
                        }
                        dt.AcceptChanges();

                        /// Check the data table has row or not
                        result = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).CopyToDataTable();
                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                                    }
                                    else
                                    {

                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).CopyToDataTable();
                                    }
                                }
                                else
                                {
                                    if (RoleId != (int)POR.Enum.UserRole.ACCOUNTS01)
                                    {
                                        var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                         (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                         x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                        if (Count != 0)
                                        {
                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                                                                      (int)POR.Enum.RecordStatus.Forward && x.Field<string>("Location") == LocationId && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                      x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
                                        }
                                    }
                                    else
                                    {
                                        var Count = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
                                         (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                         x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                        if (Count != 0)
                                        {
                                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("CurrentStatus") == RoleId && x.Field<int>("RSID") ==
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


        [HttpGet]
        public ActionResult PrintData(int LIOHID)
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 15/06/2022  
            ///Description: Load print out version of Living IN Out

            Session["LIOHID"] = LIOHID;

            var servicerType = _db.LivingInOutHeaders.Where(x => x.LIOHID == LIOHID && x.Active == 1).Select(x => x.ServiceTypeId).FirstOrDefault();
            TempData["ServiceType"] = servicerType;
            return View();

        }

        [HttpGet]
        public ActionResult Details(int SDHID, int Rejectstatus)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/08/26
            /// Description : Details related Living In Out por 
            List<_SecondaryDuty> SecondaryDutyHeaderList2 = new List<_SecondaryDuty>();
            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                int? UserRoleId;
                int? CurrentStatusUserRole;
                string LivingStatusCode;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                List<_LivingInOut> LivingInOutList = new List<_LivingInOut>();
                ViewBag.OicProces = _db.SecondaryDutyHeaders.Where(x => x.SDHID == SDHID).Select(x => x.OicProces).FirstOrDefault();
                UID_ = Convert.ToInt32(Session["UID"]);
                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                var CurrentStatus_UserRole = (from f in _db.FlowStatusSecondaryDutyDetails
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.SDuty_ID == SDHID
                                              orderby f.SFSID descending
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
                dt = objDALCommanQuery.CallDutySP(0);
                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("SDHID") == SDHID).CopyToDataTable();
                _SecondaryDuty objSecondaryDutyHeader = new _SecondaryDuty();

                ///This Rejectstatus value assign from after clicking RejectIndex Details button. It assign value 2 
                if (true)
                {
                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _LivingInOut obj_SecondaryDuty = new _LivingInOut();
                        //object valFromDate = dt2.Rows[i]["FromDate"];
                        //object valToDate = dt2.Rows[i]["ToDate"];

                        /// Check the rercord is previously reject or not
                       // var prvReject = _db.SecondaryDutyHeaders.Where(x => x.SDHID == SDHID && x.Active == 1).Select(x => x.).FirstOrDefault();


                        //if (valFromDate != DBNull.Value && valToDate != DBNull.Value)
                        //{
                        //    obj_SecondaryDuty.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                        //    obj_SecondaryDuty.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);
                        //}

                        //if (valFromDate != DBNull.Value)
                        //{
                        //    obj_SecondaryDuty.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);

                        //}

                        objSecondaryDutyHeader.SDHID = (dt2.Rows[i]["SDHID"].ToString());
                        objSecondaryDutyHeader.ServiceNo = (dt2.Rows[i]["ServiceNo"].ToString());
                        objSecondaryDutyHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        //objSecondaryDutyHeader.Trade = dt2.Rows[i]["Trade"].ToString();
                        objSecondaryDutyHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        objSecondaryDutyHeader.Location = dt2.Rows[i]["Location"].ToString();
                        objSecondaryDutyHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        objSecondaryDutyHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                        objSecondaryDutyHeader.Appointment = dt2.Rows[i]["Appointment"].ToString();
                        objSecondaryDutyHeader.AreaforResponsibility = dt2.Rows[i]["AreaforResponsibility"].ToString();
                        //objSecondaryDutyHeader.CurrentUserRole = Convert.ToInt32(dt3.Rows[i]["CurrentStatus"]);
                        objSecondaryDutyHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);
                        objSecondaryDutyHeader.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);

                        SecondaryDutyHeaderList2.Add(objSecondaryDutyHeader);

                        //if (prvReject >= 1)
                        //{
                        //    obj_SecondaryDuty.PreviousReject = Convert.ToInt32(dt2.Rows[i]["PreviousReject"]);
                        //    obj_SecondaryDuty.RejectAuth = dt2.Rows[i]["RejectAuth"].ToString();
                        //}

                        //if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        //{
                        //    obj_SecondaryDuty.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        //}
                        //if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        //{
                        //    TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        //    TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        //}

                        //if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                        //{
                        //    obj_SecondaryDuty.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                        //}

                        //LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        //TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        //TempData["LivingStatusCode"] = LivingStatusCode;


                    }

                    //var CivilStatusDetail = _db.CivilStatusHeaders.Where(x => x.CSHID == CSHID);
                    //int? CurrentStatus = CivilStatusDetail.Select(x => x.C).First();
                    //TempData["CurrentStatus"] = CurrentStatus;

                    //int? SubmitStatus = LivingInOutDetail.Select(x => x.SubmitStatus).First();
                    //TempData["SubmitStatus"] = SubmitStatus;

                    return View(SecondaryDutyHeaderList2);
                }
                else
                {
                    /// When clerk click the details of button he redirect to details action result reject section. this include Reject person
                    /// comment and reject Authority

                    TempData["Rejectstatus"] = Rejectstatus;
                    /// 1st Get the record reject Person  role Id 
                    /// 2nd Get the Role Name using Role Id

                    var RejectRoleId = _db.FlowStatusSecondaryDutyDetails.Where(x => x.RSID == (int)POR.Enum.RecordStatus.Forward && x.Active == 1 && x.SDuty_ID == SDHID)
                                        .OrderByDescending(x => x.SFSID).Select(x => x.RID).FirstOrDefault();

                    var RoleName = _db.UserRoles.Where(x => x.RID == RejectRoleId && x.Active == 1).Select(x => x.RoleName).FirstOrDefault();

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {
                        _LivingInOut obj_SecondaryDuty = new _LivingInOut();
                        object valFromDate = dt2.Rows[i]["FromDate"];
                        object valToDate = dt2.Rows[i]["ToDate"];

                        if (valFromDate != DBNull.Value && valToDate != DBNull.Value)
                        {
                            obj_SecondaryDuty.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                            obj_SecondaryDuty.ToDate = Convert.ToDateTime(dt2.Rows[i]["ToDate"]);
                        }

                        if (valFromDate != DBNull.Value)
                        {
                            obj_SecondaryDuty.FromDate = Convert.ToDateTime(dt2.Rows[i]["FromDate"]);
                        }

                        objSecondaryDutyHeader.SDSID = (dt2.Rows[i]["SDHID"].ToString());
                        objSecondaryDutyHeader.ServiceNo = (dt2.Rows[i]["ServiceNo"].ToString());
                        objSecondaryDutyHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                        objSecondaryDutyHeader.FullName = dt2.Rows[i]["Name"].ToString();
                        objSecondaryDutyHeader.Location = dt2.Rows[i]["Location"].ToString();
                        objSecondaryDutyHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        objSecondaryDutyHeader.AreaforResponsibility = dt2.Rows[i]["AreaforResponsibility"].ToString();


                        if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                        {
                            obj_SecondaryDuty.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                        }
                        if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                        {
                            TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                            TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                        }

                        if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                        {
                            obj_SecondaryDuty.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                        }

                        LivingStatusCode = dt2.Rows[i]["LivingStatusName"].ToString();

                        TempData["NOKchangeStatus"] = dt2.Rows[i]["IsNOKChange"].ToString();
                        TempData["LivingStatusCode"] = LivingStatusCode;

                        SecondaryDutyHeaderList2.Add(objSecondaryDutyHeader);
                    }

                    return View(SecondaryDutyHeaderList2);

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
            ///Created Date : 2022/05/20
            ///Description : Delete the p3 Clerk entred data. All Active 1 record turn into 0

            LivingInOut objLivingInOut = new LivingInOut();
            LivingInOutHeader objLivingInOutHeader = new LivingInOutHeader();
            NOKChangeHeader objNOKChangeHeader = new NOKChangeHeader();
            NOKChangeDetail objNOKChangeDetail = new NOKChangeDetail();
            int UID_ = Convert.ToInt32(Session["UID"]);
            SecondaryDutyHeader objSecondaryDutyHeader = new SecondaryDutyHeader();
            SecondaryDutyDetail objSecondaryDutyDetail = new SecondaryDutyDetail();

            var NokchangeStatus = _db.SecondaryDutyHeaders.Where(x => x.SDHID == id && x.Active == 1).Select(x => x.Active).FirstOrDefault();
            try
            {

                //objSecondaryDutyHeader = _db.SecondaryDutyHeaders.Find(id);
                //objSecondaryDutyHeader.Active = 0;
                //objSecondaryDutyHeader.ModifiedDate = DateTime.Now;
                //objSecondaryDutyHeader.ModifiedBy = Convert.ToString(UID_);
                //objSecondaryDutyHeader.ModifiedMac = MacAddress;

                //_db.Entry(objSecondaryDutyHeader).State = EntityState.Modified;

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    if (NokchangeStatus > 0)
                    {
                        if (NokchangeStatus == 1)
                        {
                            ///NokchangeStatus == 1 mean Livining In out record have NOK change details

                            var DutyId = _db.SecondaryDutyHeaders.Where(x => x.SDHID == id && x.Active == 1).Select(x => x.SDHID).FirstOrDefault();
                            var DutyDID = _db.SecondaryDutyDetails.Where(x => x.SecondaryDutyHeaderID == DutyId && x.Active == 1).Select(x => x.SDDID).FirstOrDefault();

                            objSecondaryDutyHeader = _db.SecondaryDutyHeaders.Find(DutyId);
                            objSecondaryDutyHeader.Active = 0;
                            objSecondaryDutyHeader.ModifiedBy = Convert.ToString(UID_);
                            objSecondaryDutyHeader.ModifiedDate = DateTime.Now;
                            objSecondaryDutyHeader.ModifiedMac = MacAddress;

                            _db.Entry(objSecondaryDutyHeader).State = EntityState.Modified;

                            objSecondaryDutyDetail = _db.SecondaryDutyDetails.Find(DutyDID);
                            objSecondaryDutyDetail.Active = 0;
                            objSecondaryDutyDetail.ModifiedBy = Convert.ToString(UID_);
                            objSecondaryDutyDetail.ModifiedDate = DateTime.Now;
                            objSecondaryDutyDetail.ModifiedMac = MacAddress;

                            _db.Entry(objSecondaryDutyDetail).State = EntityState.Modified;

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
                return RedirectToAction("Index", "SecondaryDuty");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/05/20
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
               
                ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");
                ViewBag.DDL_Relationship = new SelectList(_dbP3HRMS.Relationships, "RelationshipName", "RelationshipName");
                ViewBag.DDL_DistricSelectAll_Result = new SelectList(_dbCommonData.Districts.OrderBy(x => x.DESCRIPTION), "DIST_CODE", "DESCRIPTION");
                ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus, "SDSID", "StatusName");
                UID_ = Convert.ToInt32(Session["UID"]);
                ViewBag.OICPROCESS = _db.SecondaryDutyHeaders.Where(x => x.SDHID == id).Select(x => x.OicProces).FirstOrDefault();
                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();
                ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1 && x.Camp == EstablishmentId), "DutyLocId", "CampDutyLocation");
                var CurrentStatus_UserRole = (from f in _db.FlowStatusSecondaryDutyDetails
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.SDuty_ID == id
                                              orderby f.SFSID descending
                                              select new
                                              {
                                                  u.RoleName,
                                                  u.RID
                                              }).FirstOrDefault();

                ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                dt = objDALCommanQuery.CallDutySP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("SDHID") == id).CopyToDataTable();
                _SecondaryDuty objSecondaryDutyHeader = new _SecondaryDuty();
                // TempData["rejectStatus"] = rejectStatus;

                for (int i = 0; i < dt2.Rows.Count; i++)
                {


                    objSecondaryDutyHeader.SDSID = "2";
                    objSecondaryDutyHeader.SDHID = (dt2.Rows[i]["SDHID"].ToString());
                    objSecondaryDutyHeader.ServiceNo = (dt2.Rows[i]["ServiceNo"].ToString());
                    objSecondaryDutyHeader.Rank = dt2.Rows[i]["Rank"].ToString();
                    //objSecondaryDutyHeader.Trade = dt2.Rows[i]["Trade"].ToString();
                    objSecondaryDutyHeader.FullName = dt2.Rows[i]["Name"].ToString();
                    objSecondaryDutyHeader.Location = dt2.Rows[i]["Location"].ToString();
                    objSecondaryDutyHeader.RefNo = dt2.Rows[i]["RefNo"].ToString();
                    objSecondaryDutyHeader.Authority = dt2.Rows[i]["Authority"].ToString();
                    objSecondaryDutyHeader.AreaforResponsibility = dt2.Rows[i]["AreaforResponsibility"].ToString();
                    objSecondaryDutyHeader.Appointment = dt2.Rows[i]["Appointment"].ToString();
                    objSecondaryDutyHeader.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);
                    objSecondaryDutyHeader.WFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);



                    if (dt2.Rows[i]["WFDate"] != DBNull.Value)
                    {
                        objSecondaryDutyHeader.NOKWEFDate = Convert.ToDateTime(dt2.Rows[i]["WFDate"]);
                    }
                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                    {
                        objSecondaryDutyHeader.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                    }


                }
                //return RedirectToAction("Edit", "User");
                return View(objSecondaryDutyHeader);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }


        }
        [HttpPost]
        public ActionResult Edit(_SecondaryDuty obj_SecondaryDuty, int rejectStatus, string FromDate, string Remarks, string WEDate, string CeasedDate)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2022/05/23
            ///Description : Save User edit data to Data based
            SecondaryDutyHeader objSecondaryDutyHeader = new SecondaryDutyHeader();
            SecondaryDutyDetail objSecondaryDutyDetail = new SecondaryDutyDetail();
            Sec_Duty_Details objSec_Duty_Details = new Sec_Duty_Details();
            int UID_ = Convert.ToInt32(Session["UID"]);
            string EstablishmentId = "";
            string DivisionId = "";           
            int RoleId = 0;
            ViewBag.DDL_SDutyAppointment = new SelectList(_db.SecDutyAppointments.Where(x => x.Status == 1), "AppointmentID", "Appointment");
            EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            DivisionId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.DivisionId).FirstOrDefault();
            RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
            ViewBag.DDL_Location = new SelectList(_dbCommonData.EstablishmentNews, "LocationID", "LocationName");
            ViewBag.DDL_SDutyCategories = new SelectList(_db.SecondaryDutyStatus.Where(x => x.SDSID == 1), "SDSID", "StatusName");
            var OicProces = _db.SecondaryDutyHeaders.Where(x => x.RefNo == obj_SecondaryDuty.RefNo ).Select(x => x.OicProces).FirstOrDefault();
            var ServiceType = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber2).Select(x => x.service_type).FirstOrDefault();
            var SNO = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber2).Select(x => x.SNo).FirstOrDefault();
            //if (OicProces < 1)
            //{
            //    int Appointment = Convert.ToInt32(obj_SecondaryDuty.Appointment);
            //    var AppointmentType = _db.SecDutyAppointments.Where(x => x.AppointmentID == Appointment).Select(x => x.Appointment).FirstOrDefault();
            //    ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1), "DutyLocId", "CampDutyLocation");
            //    int DutyLoc = Convert.ToInt32(obj_SecondaryDuty.AreaforResponsibility);
            //    var DutyLocId = _db.DutyLocations.Where(x => x.DutyLocId == DutyLoc && x.Camp == EstablishmentId).Select(x => x.CampDutyLocation).FirstOrDefault();


            //}


            long ConSNo = Convert.ToInt64(SNO);
            try
            {

                
                if (OicProces < 1)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                        var DutyId = _db.SecondaryDutyHeaders.Where(x => x.RefNo == obj_SecondaryDuty.RefNo && x.Active == 1).Select(x => x.SDHID).FirstOrDefault();
                        var DutyDID = _db.SecondaryDutyDetails.Where(x => x.SecondaryDutyHeaderID == DutyId && x.Active == 1).Select(x => x.SDDID).FirstOrDefault();

                        objSecondaryDutyHeader = _db.SecondaryDutyHeaders.Find(DutyId);
                        objSecondaryDutyHeader.Authority = obj_SecondaryDuty.Authority;
                        objSecondaryDutyHeader.ModifiedBy = Convert.ToString(UID_);
                        objSecondaryDutyHeader.ModifiedDate = DateTime.Now;
                        objSecondaryDutyHeader.ModifiedMac = MacAddress;
                        objSecondaryDutyHeader.WFDate = Convert.ToDateTime(obj_SecondaryDuty.WFDate);

                        _db.Entry(objSecondaryDutyHeader).State = EntityState.Modified;

                        objSecondaryDutyDetail = _db.SecondaryDutyDetails.Find(DutyDID);
                        if (OicProces == 1)
                        {
                            objSecondaryDutyDetail.Appointment = obj_SecondaryDuty.Appointment;
                            objSecondaryDutyDetail.AreaForResponsibility = obj_SecondaryDuty.AreaforResponsibility;

                        }

                        if (OicProces < 1)
                        {
                            int Appointment = Convert.ToInt32(obj_SecondaryDuty.Appointment);
                            var AppointmentType = _db.SecDutyAppointments.Where(x => x.AppointmentID == Appointment).Select(x => x.Appointment).FirstOrDefault();
                            ViewBag.DDL_SDutyLocation = new SelectList(_db.DutyLocations.Where(x => x.Active == 1 && x.Camp == EstablishmentId), "DutyLocId", "CampDutyLocation");
                            int DutyLoc = Convert.ToInt32(obj_SecondaryDuty.AreaforResponsibility);
                            var DutyLocId = _db.DutyLocations.Where(x => x.DutyLocId == DutyLoc && x.Camp == EstablishmentId).Select(x => x.CampDutyLocation).FirstOrDefault();
                            objSecondaryDutyDetail.Appointment = AppointmentType;
                            objSecondaryDutyDetail.AreaForResponsibility = DutyLocId;
                        }
                        objSecondaryDutyDetail.WithEffectDate = Convert.ToDateTime(obj_SecondaryDuty.WFDate);
                        objSecondaryDutyDetail.Active = 1;
                        objSecondaryDutyDetail.ModifiedBy = Convert.ToString(UID_);
                        objSecondaryDutyDetail.ModifiedDate = DateTime.Now;
                        objSecondaryDutyDetail.ModifiedMac = MacAddress;

                        _db.Entry(objSecondaryDutyDetail).State = EntityState.Modified;

                        if (_db.SaveChanges() > 0)
                        {
                            scope.Complete();
                            TempData["ScfMsg"] = "Record Sucessfully Editrd.";
                        }
                        else
                        {
                            TempData["ErrMsg"] = "Process Unsucessfull.";
                            scope.Dispose();
                        }

                    }
                }
                        if (OicProces > 0)
                        {

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        var DutyId = _db.SecondaryDutyHeaders.Where(x => x.RefNo == obj_SecondaryDuty.RefNo && x.Active == 1).Select(x => x.SDHID).FirstOrDefault();
                        var DutyDID = _db.SecondaryDutyDetails.Where(x => x.SecondaryDutyHeaderID == DutyId && x.Active == 1).Select(x => x.SDDID).FirstOrDefault();
                        // var ServiceType = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == obj_SecondaryDuty.Snumber2).Select(x => x.service_type).FirstOrDefault();
                        objSecondaryDutyDetail = _db.SecondaryDutyDetails.Find(DutyDID);

                        objSecondaryDutyDetail.CeasedDate = Convert.ToDateTime(FromDate);
                        
                        _db.Entry(objSecondaryDutyDetail).State = EntityState.Modified;
                        objSecondaryDutyHeader = _db.SecondaryDutyHeaders.Find(DutyId);
                        objSecondaryDutyHeader.OicProces = 2;
                        _db.Entry(objSecondaryDutyHeader).State = EntityState.Modified;
                        if (_db.SaveChanges() > 0)
                        {
                            scope.Complete();
                        }
                    }

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        
                        string SecDutyID = Convert.ToString(_dbP3HRMS.Sec_Duty_Details.Where(x => x.AuthReff == obj_SecondaryDuty.RefNo && x.Status == 1).Select(x => x.SecDutyID).FirstOrDefault());
                        
                        objSec_Duty_Details = _dbP3HRMS.Sec_Duty_Details.Find(SecDutyID);
                        objSec_Duty_Details.DutyEndDate= Convert.ToDateTime(FromDate);
                        _dbP3HRMS.Entry(objSec_Duty_Details).State = EntityState.Modified;
                       


                            if (_dbP3HRMS.SaveChanges() > 0)
                        {
                            scope.Complete();
                       }
                    }
                       
                        obj_SecondaryDuty.RSID = 1000;
                            string CreatePorNo = PorNoCreate(EstablishmentId, obj_SecondaryDuty.InOut_CAT_ID);
                           
                                objSecondaryDutyHeader.Sno = ConSNo;
                                if (obj_SecondaryDuty.SDSID == "2")
                                {
                                    objSecondaryDutyHeader.WFDate = Convert.ToDateTime(FromDate);
                                }

                                objSecondaryDutyHeader.Location = EstablishmentId;
                                objSecondaryDutyHeader.ServiceTypeId = ServiceType;
                                objSecondaryDutyHeader.RefNo = CreatePorNo;
                                objSecondaryDutyHeader.Authority = obj_SecondaryDuty.Remarks;
                                objSecondaryDutyHeader.CreatedDate = DateTime.Now;
                                objSecondaryDutyHeader.CreatedBy = Convert.ToString(UID_);
                                objSecondaryDutyHeader.CreatedMac = MacAddress;
                                objSecondaryDutyHeader.CreateIpAddess = this.Request.UserHostAddress;
                                objSecondaryDutyHeader.Active = 1;
                                objSecondaryDutyHeader.OicProces = 0;

                                _db.SecondaryDutyHeaders.Add(objSecondaryDutyHeader);
                                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                                {
                                    if (_db.SaveChanges() > 0)
                                    {
                                        ///get the LIOHID
                                        var LIOHID = _db.SecondaryDutyHeaders.Where(x =>  x.RefNo == CreatePorNo).OrderByDescending(x => x.CreatedDate).Select(x => x.SDHID).FirstOrDefault();


                                        objSecondaryDutyDetail.SecondaryDutyHeaderID = LIOHID;
                                        objSecondaryDutyDetail.AreaForResponsibility = obj_SecondaryDuty.AreaforResponsibility;
                                        objSecondaryDutyDetail.Appointment = obj_SecondaryDuty.Appointment;
                                        objSecondaryDutyDetail.DutyStatus = Convert.ToInt32(obj_SecondaryDuty.DutyStatus);
                                        if (obj_SecondaryDuty.SDSID == "1")
                                        {
                                            objSecondaryDutyDetail.WithEffectDate = Convert.ToDateTime(FromDate);
                                        }


                                        //objSecondaryDutyDetail.TakingOverSno = Convert.ToInt32(obj_SecondaryDuty.Snumber2);

                                        if (obj_SecondaryDuty.SDSID == "2")
                                        {
                                            objSecondaryDutyDetail.WithEffectDate = Convert.ToDateTime(FromDate);
                                          
                                        }
                                        objSecondaryDutyDetail.CeasedDate = null;
                                        objSecondaryDutyDetail.CreatedBy = Convert.ToString(UID_);
                                        objSecondaryDutyDetail.CreatedDate = DateTime.Now;
                                        objSecondaryDutyDetail.CreatedMac = MacAddress;
                                        objSecondaryDutyDetail.CreateIpAddess = this.Request.UserHostAddress;
                                        objSecondaryDutyDetail.Active = 1;

                                        _db.SecondaryDutyDetails.Add(objSecondaryDutyDetail);
                                        /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                        InserFlowStatus(LIOHID, RoleId, UID_, obj_SecondaryDuty.FMSID, obj_SecondaryDuty.RSID);

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
                        
                
                return RedirectToAction("Index", "SecondaryDuty");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        // new forward
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
                var RecordInfo = _db.SecondaryDutyHeaders.Where(x => x.SDHID == id).Select(x => new { x.Location, x.ServiceTypeId, x.Sno,x.Authority,x.RefNo }).FirstOrDefault();
                var RecordInfoDetail = _db.SecondaryDutyDetails.Where(x => x.SecondaryDutyHeaderID == id).Select(x => new { x.AreaForResponsibility, x.WithEffectDate, x.CeasedDate,x.Appointment }).FirstOrDefault();
                int? SubmitStatus = NextFlowStatusId(id, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceTypeId, userInfo.DivisionId);
                int AppoitId = _db.SecDutyAppointments.Where(x=>x.Appointment== RecordInfoDetail.Appointment).Select(x=>x.AppointmentID).FirstOrDefault();
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                FlowStatusSecondaryDutyDetail objFlowStatusSecondaryDutyDetail = new FlowStatusSecondaryDutyDetail();


                Sec_Duty_Details objSec_Duty_Details = new Sec_Duty_Details();
                objFlowStatusSecondaryDutyDetail.SDuty_ID = id;
                objFlowStatusSecondaryDutyDetail.RSID = (int)POR.Enum.RecordStatus.Forward;
                objFlowStatusSecondaryDutyDetail.UID = UID;
                objFlowStatusSecondaryDutyDetail.FMSID = SubmitStatus;
                objFlowStatusSecondaryDutyDetail.RID = UserRoleId;
                objFlowStatusSecondaryDutyDetail.CreatedBy = Convert.ToString(UID);
                objFlowStatusSecondaryDutyDetail.CreatedDate = DateTime.Now;
                //objFlowStatusSecondaryDutyDetail.CreatedIP = MacAddress;
                objFlowStatusSecondaryDutyDetail.CreatedIP = this.Request.UserHostAddress;
                objFlowStatusSecondaryDutyDetail.Active = 1;
                

                    ///This function is to update the Hrmis data base. After account one certified, the details will update p3hrmis and P2hrms 
                if (userInfo.RoleId == (int)POR.Enum.UserRole.P3OIC || userInfo.RoleId == (int)POR.Enum.UserRole.ASORSOVRP2VOL)
                {
                    #region Switch Case
                    objSec_Duty_Details.SecDutyID = Convert.ToString(id);
                objSec_Duty_Details.SNo = Convert.ToString(RecordInfo.Sno);
                objSec_Duty_Details.Appointment = AppoitId;
                objSec_Duty_Details.SecDutyLocation = 0;
                objSec_Duty_Details.AuthReff = RecordInfo.RefNo;
                objSec_Duty_Details.Base = RecordInfo.Location;
                objSec_Duty_Details.DutyStatus =1;
                objSec_Duty_Details.Duty_Allocate_Date = RecordInfoDetail.WithEffectDate;
                objSec_Duty_Details.DutyEndDate =null;
                objSec_Duty_Details.Remarks = null;
                objSec_Duty_Details.CreatedUser = Convert.ToString(UID);
                objSec_Duty_Details.CreatedDate = DateTime.Now;
                objSec_Duty_Details.Area = RecordInfoDetail.AreaForResponsibility;

                objSec_Duty_Details.ModifiedUser = null;
                objSec_Duty_Details.ModifiedDate = null;
                objSec_Duty_Details.CreatedMachine = this.Request.UserHostAddress;
                objSec_Duty_Details.ModifiedMachine = null;
                objSec_Duty_Details.Status = 1;
                
                _dbP3HRMS.Sec_Duty_Details.Add(objSec_Duty_Details);
                if (_dbP3HRMS.SaveChanges() > 0)
                {
                    if (true) { }
                }
                    switch (userInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P3OIC:
                            //switch (RecordInfo.ServiceTypeId)
                            {


                                {

                                    
                                    var existingModel = _db.SecondaryDutyHeaders.Find(id);

                                    if (existingModel != null)
                                    {

                                        existingModel.OicProces = 1;
                                        _db.FlowStatusSecondaryDutyDetails.Add(objFlowStatusSecondaryDutyDetail);
                                        
                                        if (_db.SaveChanges() > 0)
                                        {
                                            TempData["ScfMsg"] = "Data Successfully Forwarded for HRMS Update";

                                            return RedirectToAction("Index");
                                        }
                                    }

                                }
                            }

                            break;
                       
                    }
                    #endregion

                   
                  
                }

                else
                {
                    _db.FlowStatusSecondaryDutyDetails.Add(objFlowStatusSecondaryDutyDetail);
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

                        var RecordInfo = _db.SecondaryDutyHeaders.Where(x => x.SDHID == IDs).Select(x => new { x.Location, x.ServiceTypeId }).FirstOrDefault();
                        int? SubmitStatus = NextFlowStatusId(IDs, userInfo.LocationId, RecordInfo.Location, RecordInfo.ServiceTypeId, userInfo.DivisionId);
                        //Get Next FlowStatus User Role Name for Add Successfull Msg

                        UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                        SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
                        FlowStatusSecondaryDutyDetail objFlowStatusSecondaryDutyDetail = new FlowStatusSecondaryDutyDetail();
                        //FlowStatusPsnContact objFlowStatusPsnContact = new FlowStatusPsnContact();

                        objFlowStatusSecondaryDutyDetail.SDuty_ID = IDs;
                        objFlowStatusSecondaryDutyDetail.RSID = (int)POR.Enum.RecordStatus.Forward;
                        objFlowStatusSecondaryDutyDetail.UID = UID;
                        objFlowStatusSecondaryDutyDetail.FMSID = SubmitStatus;
                        objFlowStatusSecondaryDutyDetail.RID = UserRoleId;
                        objFlowStatusSecondaryDutyDetail.CreatedBy = Convert.ToString(UID);
                        objFlowStatusSecondaryDutyDetail.CreatedDate = DateTime.Now;
                        //objFlowStatusSecondaryDutyDetail.CreatedIP = MacAddress;
                        objFlowStatusSecondaryDutyDetail.CreatedIP = this.Request.UserHostAddress;
                        objFlowStatusSecondaryDutyDetail.Active = 1;

                        _db.FlowStatusSecondaryDutyDetails.Add(objFlowStatusSecondaryDutyDetail);
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


        public int? NextFlowStatusId(int? SDHID, string UserEstablishmentId, string RecordEstablishmentId, int? ServiceTypeId, string UserDivisionId)
        {
            ///Created By   :Sqn Ldr Wickramasinghe
            ///Created Date :2023.05.25
            ///Des: get the next flow status id FMSID 5556

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxFSNOKCDID = _db.FlowStatusSecondaryDutyDetails.Where(x => x.SDuty_ID == SDHID).Select(x => x.SFSID).Max();
                int? CurrentFMSID = _db.FlowStatusSecondaryDutyDetails.Where(x => x.SFSID == MaxFSNOKCDID).Select(x => x.FMSID).FirstOrDefault();
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
                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                                 && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId ||
                                    x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();


                        }
                        else if (CurrentUserRole == (int)POR.Enum.UserRole.P3OIC)
                        {
                            //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                            var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AC" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();

                            SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                  && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();



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
                                FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup && (x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).FirstOrDefault();

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





        //



        [HttpGet]
        public ActionResult RejectRecord(int id, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/07/20
            /// Description : this function is to reject the record

            string message = "";
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.SecondaryDutyHeaders.Where(x => x.SDHID == id).Select(x => x.Location).FirstOrDefault();
                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(id, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                FlowStatusSecondaryDutyDetail objFlowStatusSecondaryDutyDetail = new FlowStatusSecondaryDutyDetail();


                Sec_Duty_Details objSec_Duty_Details = new Sec_Duty_Details();
                objFlowStatusSecondaryDutyDetail.SDuty_ID = id;
                objFlowStatusSecondaryDutyDetail.RSID = (int)POR.Enum.RecordStatus.Reject;
                objFlowStatusSecondaryDutyDetail.UID = UID;
                objFlowStatusSecondaryDutyDetail.FMSID = PreviousFMSID;
                objFlowStatusSecondaryDutyDetail.RID = UserRoleId;
                objFlowStatusSecondaryDutyDetail.CreatedBy = Convert.ToString(UID);
                objFlowStatusSecondaryDutyDetail.CreatedDate = DateTime.Now;
                //objFlowStatusSecondaryDutyDetail.CreatedIP = MacAddress;
                objFlowStatusSecondaryDutyDetail.CreatedIP = this.Request.UserHostAddress;
                objFlowStatusSecondaryDutyDetail.Active = 1;
                _db.FlowStatusSecondaryDutyDetails.Add(objFlowStatusSecondaryDutyDetail);

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
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Reject(string SDHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/07/20
            /// Description : this function is to reject the record

            _SecondaryDuty model = new _SecondaryDuty();
            try
            {
                model.SDHID = SDHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_RejectCommentLivingInOut", model);
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
            List<_SecondaryDuty> PsnConList = new List<_SecondaryDuty>();
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
                dt = objDALCommanQuery.CallDutyRejectSP();
                //dt = objDALCommanQuery.CallGSQSP();

                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                var FMSID = _db.FlowManagementStatus.Where(x => (x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId) && (x.EstablishmentId == LocationId && x.UserRoleID == UserInfo.RoleId)).Select(x => x.FMSID).FirstOrDefault();

                TempData["RoleId"] = UserInfo.RoleId;

                var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                if (resultStatus.Count != 0)
                {
                    /// here send a serviceStatus as 1 is to categorized the officer and other rank seperettly
                    if (serviceStatus == 1)
                    {
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer ||
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
                        var rows = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                            x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                        if (rows.Any())
                        {
                            dt2 = dt.AsEnumerable().Where(x => (x.Field<int>("Active") == 1 && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject) && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
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
                        _SecondaryDuty obj_PsnContactHeader = new _SecondaryDuty();
                        obj_PsnContactHeader.SDHID = Convert.ToString(dt3.Rows[i]["SDHID"]);
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
        public ActionResult IndividualSearchLivingInOut(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ///Created BY   : 03558 WAKY Wickramasinghe
            ///Created Date : 2021/09/03
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
                        _LivingInOut obj_SecondaryDuty = new _LivingInOut();


                        obj_SecondaryDuty.LIOHID = Convert.ToInt32(dt2.Rows[i]["LIOHID"]);
                        obj_SecondaryDuty.Snumber = dt2.Rows[i]["ServiceNo"].ToString();
                        obj_SecondaryDuty.Rank = dt2.Rows[i]["Rank"].ToString();
                        obj_SecondaryDuty.FullName = dt2.Rows[i]["Name"].ToString();
                        obj_SecondaryDuty.Location = dt2.Rows[i]["Location"].ToString();
                        obj_SecondaryDuty.CategoryName = dt2.Rows[i]["LivingStatusName"].ToString();
                        obj_SecondaryDuty.RefNo = dt2.Rows[i]["RefNo"].ToString();
                        obj_SecondaryDuty.UserRole = dt2.Rows[i]["UserRoleName"].ToString();
                        //obj_SecondaryDuty.CurrentUserRole = UserInfo.RoleId;
                        LivingInOutList.Add(obj_SecondaryDuty);
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
        public ActionResult IndividuaLivingDetails(int id)
        {
            ///Created BY   : 03558 Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/07/27
            /// Description : Respective user can see the records current status afte click the detail button


            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            //int? CurrentStatusUserRole;

            //For popup box
            TempData["CivilStatusHeaderID"] = id;

            if (Session["UID"].ToString() != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

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
        public ActionResult AdvancedSearchLivingIn(string FromDate, string ToDate, string SearchLoc, string SearchLivingStatus, string RecordStatus, int? page, string currentFilterFDate, string currentFilterTDate, string currentFilterLocation, string currentFilterLivingStatus, string currentFilterRecStatus)
        {
            ///Created By   : Flt Lt WAKY Wickramasinghe
            ///Created Date :2022-08-03
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
                        _LivingInOut obj_SecondaryDuty = new _LivingInOut();
                        obj_SecondaryDuty.Snumber = dt.Rows[i]["ServiceNo"].ToString();
                        obj_SecondaryDuty.Rank = dt.Rows[i]["Rank"].ToString();
                        obj_SecondaryDuty.FullName = dt.Rows[i]["Name"].ToString();
                        obj_SecondaryDuty.CategoryName = dt.Rows[i]["LivingStatusShortName"].ToString();
                        obj_SecondaryDuty.Authority = dt.Rows[i]["Authority"].ToString();
                        obj_SecondaryDuty.Location = dt.Rows[i]["Location"].ToString();
                        obj_SecondaryDuty.FromDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"]);
                        obj_SecondaryDuty.UserRole = dt.Rows[i]["RoleName"].ToString();
                        obj_SecondaryDuty.LIOHID = Convert.ToInt32(dt.Rows[i]["InOut_ID"]);
                        LivingInOutList.Add(obj_SecondaryDuty);

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
                        TempData["ScfMsg"] = "Successfully Reject Confirmed.";
                        scope.Complete();
                    }
                    else
                    {
                        TempData["ErrMsg"] = "Process Unsuccessful.Try again.";
                        scope.Dispose();
                    }
                    return RedirectToAction("IndexRejectLivingInOut");
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public int? CancelNextFlowStatusId(int? InOut_ID, string UserEstablishmentId, string RecordEstablishmentId)
        {

            ///Created BY   : Fg Off RGSD Gamage
            ///Created Date : 2021/02/25
            /// Description : Find the new flowstutus to whom it will forward 

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFADFID = _db.FlowStatusLivingInOutDetails.Where(x => x.InOut_ID == InOut_ID).Select(x => x.LFSID).Max();
                int? CurrentFMSID = _db.FlowStatusLivingInOutDetails.Where(x => x.LFSID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowStatusLivingInOutDetails.Where(x => x.LFSID == MaxFADFID && x.FMSID == CurrentFMSID).Select(x => x.RID).FirstOrDefault();

                int? UID = Convert.ToInt32(Session["UID"]);

                //FADID=Null (actclk create record)
                if (CurrentUserRole == (int)POR.Enum.UserRole.P3CLERK || CurrentUserRole == (int)POR.Enum.UserRole.P3SNCO)
                {
                    //Get First FMSID if Current FMSID is null
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.FMSID).First();

                }
                else if (CurrentUserRole == (int)POR.Enum.UserRole.P3OIC)
                {
                    //UserRoleId = 5 to P&R first User 'ko_pnr'
                    //currentUser-Station/Base = OCPS
                    //RecordUser - Station/Base no P&R there for get UserRole ID from First User Role In P&R
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 5).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole == (int)POR.Enum.UserRole.CERTIFIED)
                {
                    //UserRoleId = 5 to P&R first User 'ko_pnr'
                    //currentUser-Station/Base = OCPS
                    //RecordUser - Station/Base no P&R there for get UserRole ID from First User Role In P&R
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 12).Select(x => x.FMSID).First();
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
        public void UpdateLivingStatus(string FSno, int? InOut_CAT_ID)
        {
            ///Created BY   : Fg Off RGSD Gamage
            ///Created Date : 2021/04/07
            ///Description : Final proces after P3 User updated the data living in out status

            ServicePersonnelProfile objServicePersonnelProfile = new ServicePersonnelProfile();
            try
            {

                if (InOut_CAT_ID == 1 || InOut_CAT_ID == 7 || InOut_CAT_ID == 9 || InOut_CAT_ID == 10)
                {
                    objServicePersonnelProfile = _dbP3HRMS.ServicePersonnelProfiles.Find(FSno);
                    objServicePersonnelProfile.LivingIn_Out = "OUT";
                    _dbP3HRMS.Entry(objServicePersonnelProfile).State = EntityState.Modified;
                    //_dbP3HRMS.SaveChanges();

                }
                else if (InOut_CAT_ID == 4 || InOut_CAT_ID == 5 || InOut_CAT_ID == 6 || InOut_CAT_ID == 8)
                {

                    objServicePersonnelProfile = _dbP3HRMS.ServicePersonnelProfiles.Find(FSno);
                    objServicePersonnelProfile.LivingIn_Out = "IN";
                    _dbP3HRMS.Entry(objServicePersonnelProfile).State = EntityState.Modified;

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
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
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P3OIC || CurrentUserRole == (int)POR.Enum.UserRole.P3SNCO)
                    {
                        SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                                                                   && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                        //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AK" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();


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
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AK" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
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

      




        public int? PreviousFlowStatusId(int? id, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Fg off RGSD GAMAGE
            ///Created Date :2021.03.25
            ///Des: get the reject flow status id FMSID

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
               
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);
                int? MaxFSNOKCDID = _db.FlowStatusSecondaryDutyDetails.Where(x => x.SDuty_ID == id).Select(x => x.SFSID).Max();
                int? CurrentFMSID = _db.FlowStatusSecondaryDutyDetails.Where(x => x.SFSID == MaxFSNOKCDID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                //int? SubmitStatus = 0;
                //FADID=Null (actclk create record)
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
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "AK" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                        RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                       && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();

                        //SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                        //if (RejectStatus == (int)POR.Enum.UserRole.SNCOSALARY)
                        //{
                        //    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.SNCOSALARY && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();
                        //}
                        //else if (RejectStatus == (int)POR.Enum.UserRole.ACCOUNTS01)
                        //{
                        //    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.ACCOUNTS01 && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();

                        //}
                        //else
                        //{
                        //    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

                        //}
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
        public string PorNoCreate(string EstablishmentId, int InOut_CAT_ID)
        {
            ///Created BY   : Fg Off RGSD Gamage
            ///Created Date : 2021/02/25
            /// Description : create POR Number 

            try
            {
                int currentmonth = Convert.ToInt32(DateTime.Now.Month);
                int currentyear = Convert.ToInt32(DateTime.Now.Year);
                int currentdate = Convert.ToInt32(DateTime.Now.Day);

                int jobcount = _db.SecondaryDutyHeaders.Where(x => x.Location == EstablishmentId && x.CreatedDate.Value.Month == currentmonth && x.CreatedDate.Value.Year == currentyear && x.Active == 1).Count();
                int RocordId = jobcount + 1;

                string NewJobNo = EstablishmentId + "/" + "AT-1" + "/" + RocordId + "/" + " " + "D/D/" + currentdate + "/" + currentmonth + "/" + currentyear;
                return NewJobNo;

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
                //FlowStatusLivingInOutDetail objFlowStatusDetail = new FlowStatusLivingInOutDetail();
                FlowStatusSecondaryDutyDetail objFlowStatusDetail = new FlowStatusSecondaryDutyDetail();
                // LivingInOutDetail objLivingInOutDetail = new LivingInOutDetail();


                objFlowStatusDetail.SDuty_ID = InOut_ID;
                objFlowStatusDetail.RSID = RSID;
                objFlowStatusDetail.UID = UID_;
                objFlowStatusDetail.FMSID = FMSID;
                objFlowStatusDetail.RID = RoleId;
                objFlowStatusDetail.CreatedBy = Convert.ToString(UID_);
                //objFlowStatusDetail.CreatedIP = MacAddress;
                objFlowStatusDetail.CreatedIP = this.Request.UserHostAddress;
                objFlowStatusDetail.CreatedDate = DateTime.Now;
                objFlowStatusDetail.Active = 1;

                _db.FlowStatusSecondaryDutyDetails.Add(objFlowStatusDetail);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool UpdateHrmis(string LivingStatusName, string MacAddress, int? UID, long? Sno, int? InOutHeaderID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/12/06
            /// Description : update the hrmis

            _LivingInOut obj_SecondaryDuty = new _LivingInOut();
            NOK_Change_Details objNOK_Change_Details = new NOK_Change_Details();
            DataTable dt = new DataTable();

            int detailsCollectCategory = (int)POR.Enum.NOKSelectCategory.livinInOut;
            string Relationship = "";
            string districtName = "";
            string GsName = "";
            string nearestTown = "";
            string policeStation = "";
            bool status;
            bool status2;
            bool status3 = false;

            string SSNo = Convert.ToString(Sno);
            try
            {

                /// Update the service personal profile LivingIn_Out coloum
                status = objDALCommanQuery.SQL_UpdateServicePersonnelProfile(LivingStatusName, MacAddress, UID, Sno);

                /// Get Nok details
                dt = objDALCommanQuery.getNokDetails(InOutHeaderID, detailsCollectCategory);

                foreach (DataRow row in dt.Rows)
                {
                    Relationship = row["NOKChangeTo"].ToString();
                    districtName = row["District"].ToString();
                    GsName = row["GSDivision"].ToString();
                    nearestTown = row["NearestTown"].ToString();
                    policeStation = row["PoliceStation"].ToString();
                }


                var districtId = _dbCommonData.Districts.Where(x => x.DESCRIPTION == districtName).Select(x => x.DIST_CODE).FirstOrDefault();
                var GSDivisionID = _dbCommonData.GSDivisions.Where(x => x.GSName == GsName && x.District == districtId).Select(x => x.GSDivisionID).FirstOrDefault();
                var NearestTownID = _dbCommonData.Towns.Where(x => x.Town1 == nearestTown && x.DIST_CODE == districtId).Select(x => x.TownCOde).FirstOrDefault();
                var PoliceStationId = _dbCommonData.PoliceStations.Where(x => x.PoliceStation1 == policeStation).Select(x => x.PoliceStationCode).FirstOrDefault();
                var RelationshipID = _dbP3HRMS.Relationships.Where(x => x.RelationshipName == Relationship).Select(x => x.RelationshipID).FirstOrDefault();

                long key = objDALCommanQuery.GenerateKey("NOK_Change_Details", "NOKID");

                string NokID = Convert.ToString(Sno + "/" + key);

                /// Update the perivous NoK type into 2

                status = objDALCommanQuery.UpdatePreviousNokTypeId(Sno, UID, MacAddress);

                /// Insert new record to NOKChangeHeader table in P3 hrmis data based

                if (status == true)
                {
                    List<_LivingInOut> listLivingInOut = new List<_LivingInOut>();
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
                    _dbP3HRMS.NOK_Change_Details.Add(objNOK_Change_Details);

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        if (_dbP3HRMS.SaveChanges() > 0)
                        {
                            status2 = true;
                            scope.Complete();
                        }
                        else
                        {
                            status2 = false;
                            scope.Dispose();
                        }

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
        public ActionResult ClearFileds(_SecondaryDuty obj_SecondaryDuty)
        {
            /// Clear view text box fields.

            ModelState.Clear();
            return RedirectToAction("Create", "SecondaryDuty");
        }

        #region Json Methods
        #endregion
        public JsonResult getname(string id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/12
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
        public JsonResult FromDistrict(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/14
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
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/14
            /// Description : Load Section

            List<GSDivision> Result = new List<GSDivision>();
            Result = _dbCommonData.GSDivisions.Where(x => x.District == id).OrderBy(x => x.GSName).ToList();

            return Result;
        }
        public JsonResult FromPoliceStation(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/14
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
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/14
            /// Description :  Load Police Stations

            List<PoliceStation> Result = new List<PoliceStation>();
            Result = _dbCommonData.PoliceStations.Where(x => x.District == id).OrderBy(x => x.PoliceStation1).ToList();

            return Result;
        }
        public JsonResult FromTown(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/18
            /// Description : Load Town according to distrit

            List<District> FromTown = new List<District>();

            var LinqFromTown = this.LinqFromTown(id);

            var TownListData = LinqFromTown.Select(x => new SelectListItem()
            {
                Text = x.Town1.ToString(),
                Value = x.Town1.ToString(),
            });


            return Json(TownListData, JsonRequestBehavior.AllowGet);
        }
        public IList<Town> LinqFromTown(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/18
            /// Description :  Load Town according to distrit

            List<Town> Result = new List<Town>();
            Result = _dbCommonData.Towns.Where(x => x.DIST_CODE == id).OrderBy(x => x.Town1).ToList();

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
                Text = x.PostOfficeName.ToString(),
                Value = x.PostOfficeName.ToString(),
            });


            return Json(TownListData, JsonRequestBehavior.AllowGet);
        }
        public IList<PostOffice> LinqFromPostOffice(int id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2021/02/18
            /// Description :  Load Town according to distrit

            List<PostOffice> Result = new List<PostOffice>();
            Result = _dbCommonData.PostOffices.Where(x => x.District == id).OrderBy(x => x.PostOfficeName).ToList();

            return Result;
        }
        public JsonResult GetDetails(string id)
        {
            ///Created BY   : Fg off RGSD Gamage
            ///Created Date : 2021/03/19
            /// Description : get the details from livinginout table his prvious details

            LivingInOutHeader objLivingInOutHeader = new LivingInOutHeader();
            long ConSNo = 0;
            var sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            if (sno != null)
            {
                ConSNo = Convert.ToInt64(sno);
            }


            var count = _db.LivingInOutHeaders.Where(x => x.Sno == ConSNo && x.Active == 1).Count();

            if (count == 1)
            {
                objLivingInOutHeader = _db.LivingInOutHeaders.Where(x => x.Sno == ConSNo && x.Active == 1).FirstOrDefault();

            }
            else
            {
                objLivingInOutHeader.LIOHID = 0;
            }

            return Json(objLivingInOutHeader, JsonRequestBehavior.AllowGet);



        }
        public JsonResult HrmisNokDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/09/06
            ///Description  : get the hrmis include NOK Info

            Vw_NOK_Details objNOK_Details = new Vw_NOK_Details();
            Vw_NokDetailsP2 objNokDetailsP2 = new Vw_NokDetailsP2();
            var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            var S_no = Convert.ToInt64(SNo);
            var Servicetype = _db.Vw_PersonalDetail.Where(x => x.SNo == SNo).Select(x => x.service_type).FirstOrDefault();
            try
            {
                if (Servicetype == (int)POR.Enum.ServiceType.RegAirmen || Servicetype == (int)POR.Enum.ServiceType.RegAirWomen || Servicetype == (int)POR.Enum.ServiceType.VolAirmen || Servicetype == (int)POR.Enum.ServiceType.VolAirWomen)
                {
                    var count = _db.Vw_NOK_Details.Where(x => x.ActiveNo == id).Count();
                    if (count == 1)
                    {
                        objNOK_Details = _db.Vw_NOK_Details.Where(x => x.ActiveNo == id).FirstOrDefault();
                    }
                    else
                    {
                        objNOK_Details.NOKName = "No records to find.";
                    }
                }
                else
                {
                    /// Yet to develop
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(objNOK_Details, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PORNokDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/09/06
            /// Description : get the POR system Include NOK info, this may in pending approval stage

            Vw_NOK_Details objNOK = new Vw_NOK_Details();
            var NOKList = new List<NOKChangeDetail>();
            var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
            long s_no = Convert.ToInt64(SNo);
            var Servicetype = _db.Vw_PersonalDetail.Where(x => x.SNo == SNo).Select(x => x.service_type).FirstOrDefault();
            //var Nok = _db.NOKChangeHeaders.Where(x=>x.Sno == s_no && x.Active == 1).ToList(
            try
            {
                if (Servicetype == (int)POR.Enum.ServiceType.RegAirmen || Servicetype == (int)POR.Enum.ServiceType.RegAirWomen || Servicetype == (int)POR.Enum.ServiceType.VolAirmen || Servicetype == (int)POR.Enum.ServiceType.VolAirWomen)
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
                else
                {
                    /// Yet to develop
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(NOKList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMarriedDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/10/29
            /// Description : this function check person married or not for following category M/L/IN,M/L/OUT ,M/R/OUT
            int IsMarried = 0;
            try
            {
                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
                long SNO = Convert.ToInt64(Sno);
                //string Sno = 
                var marriedInfoPOR = _db.CivilStatusHeaders.Where(x => x.Sno == SNO && x.SubCategoryId == (int)POR.Enum.CivilStatusCategory.Marriage && x.Active == 1).Count();
                var marriedInfoHRMS = _dbP3HRMS.Marriages.Where(x => x.SNo == Sno).Count();

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
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public JsonResult GetSdutyDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe
            ///Created Date : 2021/10/29
            /// Description : this function check person married or not for following category M/L/IN,M/L/OUT ,M/R/OUT
            int IsMarried = 0;
            try
            {
                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
                long SNO = Convert.ToInt64(Sno);
                //string Sno = 
                var marriedInfoPOR = _db.CivilStatusHeaders.Where(x => x.Sno == SNO && x.SubCategoryId == (int)POR.Enum.CivilStatusCategory.Marriage && x.Active == 1).Count();
                var marriedInfoHRMS = _dbP3HRMS.Marriages.Where(x => x.SNo == Sno).Count();
                marriedInfoHRMS = 0;

                if (marriedInfoPOR != 0 || marriedInfoHRMS == 0)
                {
                    IsMarried = 1;
                    return Json(IsMarried, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(IsMarried, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //


        //



        public JsonResult getMarriageDetails(string id)
        {
            ///Created BY   : Flt Lt WAKY Wicramasinghr
            ///Created Date : 2021/09/05
            /// Description : get Marriage person name and load it to Spouse Name Text Box

            _LivingInOut obj_SecondaryDuty = new _LivingInOut();
            try
            {
                var SNo = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Select(x => x.SNo).FirstOrDefault();
                var Servicetype = _db.Vw_PersonalDetail.Where(x => x.SNo == SNo).Select(x => x.service_type).FirstOrDefault();

                if (Servicetype == (int)POR.Enum.ServiceType.RegAirmen || Servicetype == (int)POR.Enum.ServiceType.RegAirWomen || Servicetype == (int)POR.Enum.ServiceType.VolAirmen || Servicetype == (int)POR.Enum.ServiceType.VolAirWomen)
                {
                    var SpouseName = _dbP3HRMS.Marriages.Where(x => x.SNo == SNo).Select(x => x.SpouseName).FirstOrDefault();
                    if (SpouseName == null)
                    {
                        obj_SecondaryDuty.SpouseName = "No records to found in HRMIS and Please Contact the Cmd P3 Section";
                    }
                    else
                    {
                        obj_SecondaryDuty.SpouseName = SpouseName;
                    }

                }
                else
                {
                    obj_SecondaryDuty.SpouseName = "Please Enter Name Manualy";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(obj_SecondaryDuty, JsonRequestBehavior.AllowGet);
        }



    } }





