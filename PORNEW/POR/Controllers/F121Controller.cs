using System.Linq;
using System.Web.Mvc;
using System.Data;
using POR.Models;
using System;
using POR.Models.F121;
using System.Transactions;
using System.Collections.Generic;
using PagedList;

namespace POR.Controllers
{
    public class F121Controller : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbCommonData = new dbContextCommonData();
        List<Cls_ItemList> lst_ListPartItem = new List<Cls_ItemList>();
        List<Cls_ItemList> lst_ListItem = new List<Cls_ItemList>();
        // GET: F121
        [HttpGet]
        public ActionResult Create()
        {
            ///Create By: Fg Off RGSD GAMAGE
            ///Create Date: 01/01/2022  
            ///Description: por clerk initial step stage of crate F121 Charge cheet

            try
            {

                ViewBag.DDL_Establishment = new SelectList(_dbCommonData.Establishments, "LocShortName", "LocShortName");
                ViewBag.DDL_OffenceName = new SelectList(_db.Vw_Offences.OrderBy(x => x.OffenceName), "OffenceID", "OffenceName");
                ViewBag.DDL_Punishments = new SelectList(_db.Vw_Punishments.OrderBy(x => x.Punishment), "PunishmentID", "Punishment");


                //List<SelectListItem> Sec40 = new List<SelectListItem>();

                //Sec40.Add(new SelectListItem { Text = "Yes", Value = "1" });
                //Sec40.Add(new SelectListItem { Text = "No", Value = "2" });
                //ViewBag.Sec40 = Sec40;

                //List<SelectListItem> OptCM = new List<SelectListItem>();

                //OptCM.Add(new SelectListItem { Text = "Yes", Value = "3" });
                //OptCM.Add(new SelectListItem { Text = "No", Value = "4" });
                //ViewBag.OptCM = OptCM;


                //List<SelectListItem> RecCM = new List<SelectListItem>();

                //RecCM.Add(new SelectListItem { Text = "Yes", Value = "5" });
                //RecCM.Add(new SelectListItem { Text = "No", Value = "6" });
                //ViewBag.RecCM = RecCM;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
        }






        [HttpPost]
        public ActionResult Create(_F121 _objF252ChargeHeader)
        {

            ///Create By: Fg Off RGSD GAMAGE
            ///Create Date: 01/01/2022  
            ///Description: por clerk initial step stage of crate F121 Charge cheet data save to DB 


            F252ChargeHeader objF252ChargeHeader = new F252ChargeHeader();
            F252WitnessHeader objF252WitnessHeader = new F252WitnessHeader();
            F252WittnessPerson objF252WittnessPerson = new F252WittnessPerson();
            F252OffenceInfo objF252OffenceInfo = new F252OffenceInfo();
            WitnessDocument ObjWitnessDocument = new WitnessDocument();
            F252PunishmentHeader objF252PunishmentHeader = new F252PunishmentHeader();
            F252PunishmentDetails objF252PunishmentDetails = new F252PunishmentDetails();

            //Get the MAC Address of the User PC
            string MacAddress = new DALBase().GetMacAddress();
            string message = "";
            int UID_ = 0;
            int RoleId = 0;

            lst_ListItem = (List<Cls_ItemList>)Session["ListItem"];
            lst_ListPartItem = (List<Cls_ItemList>)Session["lst_ListPartItem"];


            if (Session["UID"] != null)
            {


                try
                {
                    if (_objF252ChargeHeader.ChargeDate != null && _objF252ChargeHeader.OptCM != 0 && _objF252ChargeHeader.RecCM != 0 &&
                    _objF252ChargeHeader.Sec40 != 0 && _objF252ChargeHeader.OffenceWASO != null && _objF252ChargeHeader.LocShortName != null &&
                    _objF252ChargeHeader.OffenceDate != null && _objF252ChargeHeader.PunishDate != null && _objF252ChargeHeader.PunishmentID != 0 &&
                    _objF252ChargeHeader.Appointment != null && _objF252ChargeHeader.PunishmentDescription != null && lst_ListItem.Count != 0 && lst_ListPartItem.Count != 0)
                    {
                        UID_ = Convert.ToInt32(Session["UID"]);

                        ViewBag.DDL_Establishment = new SelectList(_dbCommonData.Establishments, "LocShortName", "LocShortName");
                        ViewBag.DDL_OffenceName = new SelectList(_db.Vw_Offences.OrderBy(x=>x.OffenceName), "OffenceID", "OffenceName");
                        
                        ViewBag.DDL_Punishments = new SelectList(_db.Vw_Punishments.OrderBy(x => x.Punishment), "PunishmentID", "Punishment");

                        RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
                        var LocationId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();
                        /// Get the service type related to service number.
                        var ServiceType = _db.Vw_PersonalDetail.Where(x => x.SNo == _objF252ChargeHeader.Snumber).Select(x => x.service_type).FirstOrDefault();

                        /////


                        ////







                        if (ModelState.IsValid)
                        {
                            objF252ChargeHeader.ChargeNo = _objF252ChargeHeader.ChargeNo;
                            objF252ChargeHeader.ChargeNo2 = _objF252ChargeHeader.ChargeNo2;
                            objF252ChargeHeader.ChargeDate = _objF252ChargeHeader.ChargeDate;
                            objF252ChargeHeader.CreateLocation = LocationId;
                            objF252ChargeHeader.Sno = _objF252ChargeHeader.Snumber;
                            objF252ChargeHeader.ServiceTypeId = ServiceType;
                            objF252ChargeHeader.IsOptedCM = _objF252ChargeHeader.OptCM;
                            objF252ChargeHeader.IsRemanForCM = _objF252ChargeHeader.RecCM;
                            objF252ChargeHeader.ISConcurrnt = 3;
                            objF252ChargeHeader.IsSec40Compiled = _objF252ChargeHeader.Sec40;
                            objF252ChargeHeader.CreatedBy = UID_;
                            objF252ChargeHeader.CreatedDate = DateTime.Now;
                            objF252ChargeHeader.CreatedMac = MacAddress;
                            objF252ChargeHeader.CreatedIP = this.Request.UserHostAddress;
                            objF252ChargeHeader.Active = 1;
                            _db.F252ChargeHeader.Add(objF252ChargeHeader);

                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                if (_db.SaveChanges() > 0)
                                {
                                    var CHID = _db.F252ChargeHeader.Where(x => x.ChargeNo == (_objF252ChargeHeader.ChargeNo)
                                                 && x.Sno == _objF252ChargeHeader.Snumber && x.Active == 1).OrderByDescending(x => x.CreatedDate)
                                                 .Select(x => x.CHID).FirstOrDefault();



                                    /// This function is to  Enter intial record Status in to FlowStatusDetails 
                                    InserFlowStatus(CHID, RoleId, UID_, _objF252ChargeHeader.FMSID, _objF252ChargeHeader.RSID);
                                    //string Location = Convert.ToString(_objF252ChargeHeader.LocShortName);

                                    objF252OffenceInfo.OffenceTypeId = _objF252ChargeHeader.OffenceID;
                                    objF252OffenceInfo.CHID = CHID;
                                    objF252OffenceInfo.OffenceWASO = _objF252ChargeHeader.OffenceWASO;
                                    objF252OffenceInfo.OffencePlace = _objF252ChargeHeader.LocShortName.Trim();
                                    objF252OffenceInfo.OffenceDate = _objF252ChargeHeader.OffenceDate;
                                    objF252OffenceInfo.CreatedBy = UID_;
                                    objF252OffenceInfo.CreatedDate = DateTime.Now;
                                    objF252OffenceInfo.CreatedMac = MacAddress;
                                    objF252OffenceInfo.CreatedIP = Request.UserHostAddress;
                                    objF252OffenceInfo.Active = 1;
                                    _db.F252OffenceInfo.Add(objF252OffenceInfo);

                                    //InsertFlowStatus(_objF252ChargeHeader, CHID);
                                    objF252PunishmentHeader.PunishDate = _objF252ChargeHeader.PunishDate;
                                    objF252PunishmentHeader.PunishmentID = _objF252ChargeHeader.PunishmentID;
                                    objF252PunishmentHeader.Appointment = _objF252ChargeHeader.Appointment;
                                    objF252PunishmentHeader.CHID = CHID;
                                    objF252PunishmentHeader.CreatedBy = UID_;
                                    objF252PunishmentHeader.CreatedDate = DateTime.Now;
                                    objF252PunishmentHeader.CreatedMac = MacAddress;
                                    objF252PunishmentHeader.CreatedIP = Request.UserHostAddress;
                                    objF252PunishmentHeader.Active = 1;
                                    _db.F252PunishmentHeader.Add(objF252PunishmentHeader);


                                    objF252WitnessHeader.CHID = CHID;
                                    objF252WitnessHeader.CreatedBy = UID_;
                                    objF252WitnessHeader.CreatedDate = DateTime.Now;
                                    objF252WitnessHeader.CreatedMac = MacAddress;
                                    objF252WitnessHeader.CreatedIP = Request.UserHostAddress;
                                    objF252WitnessHeader.Active = 1;
                                    _db.F252WitnessHeader.Add(objF252WitnessHeader);

                                    if (_db.SaveChanges() > 0)
                                    {

                                        var FPIID = _db.F252PunishmentHeader.Where(x => x.CHID == CHID
                                                   && x.Active == 1).OrderByDescending(x => x.CreatedDate)
                                                   .Select(x => x.FPIID).FirstOrDefault();

                                        objF252PunishmentDetails.PunishmentDescription = _objF252ChargeHeader.PunishmentDescription;
                                        objF252PunishmentDetails.FPIID = FPIID;
                                        objF252PunishmentDetails.CreatedBy = UID_;
                                        objF252PunishmentDetails.CreatedDate = DateTime.Now;
                                        objF252PunishmentDetails.CreatedMac = MacAddress;
                                        objF252PunishmentDetails.CreatedIP = Request.UserHostAddress;
                                        objF252PunishmentDetails.Active = 1;
                                        _db.F252PunishmentDetails.Add(objF252PunishmentDetails);
                                        _db.SaveChanges();

                                        var WHID = _db.F252WitnessHeader.Where(x => x.CHID == objF252WitnessHeader.CHID && x.Active == 1).OrderByDescending(x => x.CreatedDate)
                                              .Select(x => x.WHID).FirstOrDefault();

                                        //foreach (var item in lst_ListItem)
                                        //{

                                        foreach (var a in lst_ListItem.Select((value, i) => new { i, value }))
                                        {
                                            var value = a.value;
                                            var index = a.i + 1;

                                            objF252WittnessPerson.Sno = a.value.ServiceNo;
                                            objF252WittnessPerson.No = index;
                                            objF252WittnessPerson.Title = a.value.Rank;
                                            objF252WittnessPerson.FullName = a.value.ServiceNo + " " + a.value.Rank + " " + a.value.FullName;
                                            objF252WittnessPerson.WHID = WHID;
                                            objF252WittnessPerson.CreatedBy = UID_;
                                            objF252WittnessPerson.CreatedDate = DateTime.Now;
                                            objF252WittnessPerson.CreatedMac = MacAddress;
                                            objF252WittnessPerson.CreatedIP = Request.UserHostAddress;
                                            objF252WittnessPerson.Active = 1;
                                            _db.F252WittnessPerson.Add(objF252WittnessPerson);
                                            _db.SaveChanges();
                                        }
                                        //}

                                        //foreach (var item in lst_ListPartItem)
                                        //{
                                        foreach (var b in lst_ListPartItem.Select((value, p) => new { p, value }))
                                        {
                                            var value = b.value;
                                            var indexP = b.p + 1;

                                            ObjWitnessDocument.No = indexP;
                                            ObjWitnessDocument.DetailsOfDocument = b.value.DocName;
                                            ObjWitnessDocument.WHID = WHID;
                                            ObjWitnessDocument.CreatedBy = UID_;
                                            ObjWitnessDocument.CreatedDate = DateTime.Now;
                                            ObjWitnessDocument.CreatedMac = MacAddress;
                                            ObjWitnessDocument.CreatedIP = Request.UserHostAddress;
                                            ObjWitnessDocument.Active = 1;
                                            _db.WitnessDocuments.Add(ObjWitnessDocument);
                                            _db.SaveChanges();
                                        }
                                        //}

                                        scope.Complete();
                                        //TempData["ScfMsg"] = "Data Successfully Saved.";
                                        message = "Data Successfully Saved.";
                                        scope.Dispose();
                                        Session.Remove("ListItem");
                                        Session.Remove("ListPartItem");
                                        Session.Remove("lst_ListPartItem");


                                    }
                                    else
                                    {
                                        //TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                        message = "Process Unsuccessful.Try again...";
                                        scope.Dispose();
                                        Session.Remove("ListItem");
                                        Session.Remove("ListPartItem");
                                        Session.Remove("lst_ListPartItem");
                                    }
                                }
                                else
                                {
                                    //TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                    message = "Process Unsuccessful.Try again...";
                                    scope.Dispose();
                                    Session.Remove("ListItem");
                                    Session.Remove("ListPartItem");
                                    Session.Remove("lst_ListPartItem");
                                }
                            }

                        }
                    }
                    else
                    {
                        message = "Please Fill all the details.";
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                return Json(new { Message = message, JsonRequestBehavior.AllowGet });
                //return View();

            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }

        [HttpGet]
        public ActionResult IndexF121(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {

            ///Created BY   : FLT LT RGSD GAMAGE
            ///Created Date : 03/01/2022  
            /// Description : Index view of the all users all pending list shows accoring to the user role and the location

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_F121> F121List = new List<_F121>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                //ViewBag.DateSortParm = sortOrder == "Sno" ? "Rank" : "CreateDate";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                long sno = 0;

                if (searchString != null)
                {
                    page = 1;

                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                    sno = Convert.ToInt64(Sno);

                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallF121SP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P3CLERK:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P3SNCO:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P3OIC:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);



                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    searchString = currentFilter;
                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallF121SP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).ToList();
                    //var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 2000 || x.Field<int>("RSID") == 1000).CopyToDataTable();
                        //dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P1CLERK:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1SNCO:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1OIC:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = loadDataUserWise(RoleId, dt2, LocationId, UID);
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {

                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);

                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public DataTable loadDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created BY   : Flt Lt RGSD Gamage
            ///Created Date : 2022/03/26
            /// Description : Load data user roll wise

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();



                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P1CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).ToList();


                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert).CopyToDataTable();

                        }
                        break;
                    #endregion
                    case (int)POR.Enum.UserRole.HRMSCLKP3VOL:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && ((x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert) || (x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward)) || (x.Field<string>("CreateLocation") == LocationId)).ToList();
                        //dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).ToList();


                        if (result2.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && ((x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert) || (x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward)) || (x.Field<string>("CreateLocation") == LocationId)).CopyToDataTable();

                        }
                        #endregion
                        break;
                    default:

                        #region CodeArea
                        ///Data Table first row CurrentStatus gave null value, hence it occures an error. Because of that check the column and delete that row, 
                        /// [30] means null coloumn number

                        /// check the vol flow management process
                        var AllowedCriteria = _db.UserPermissions.Where(x => x.UserId == UID && x.Active == 1).Select(x => new { x.AllowVAF, x.AllowRAF }).FirstOrDefault();



                        /// Check the data table has row or not

                        result = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward).CopyToDataTable();
                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                                    }
                                    else
                                    {

                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).CopyToDataTable();
                                    }
                                }
                                else
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Forward && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
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
        public ActionResult Delete(int CHID)
        {
            F252ChargeHeader objF252ChargeHeader = new F252ChargeHeader();

            var existingModel = _db.F252ChargeHeader.Find(CHID);

            if (existingModel != null)
            {

                existingModel.Active = 0;

            }
            _db.SaveChanges();

            return RedirectToAction("IndexF121");
        }

        [HttpGet]
        public ActionResult DeleteOIC(int CHID)
        {
            F252ChargeHeader objF252ChargeHeader = new F252ChargeHeader();

            var existingModel = _db.F252ChargeHeader.Find(CHID);

            if (existingModel != null)
            {
                existingModel.Active = 0;
                existingModel.OicProcess = 0;

            }
            _db.SaveChanges();

            return RedirectToAction("IndexConfirmF121");
        }








        [HttpGet]
        public ActionResult Details(int CHID)
        {
            ///Created BY   : Flt Lt RGSD GAMAGE
            ///Created Date : 2022/02/01
            /// Description : Details related to F121

            if (Session["UID"] != null)
            {

                int? UID = Convert.ToInt32(Session["UID"]);
                int UID_ = 0;
                string EstablishmentId;
                int? UserRoleId;
                string PunishmentName;
                int? CurrentStatusUserRole;

                DataTable dt = new DataTable();

                DataTable dt2 = new DataTable();
                List<_F121> F121List = new List<_F121>();


                UID_ = Convert.ToInt32(Session["UID"]);
                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                var CurrentStatus_UserRole = (from f in _db.F121FlowStatus
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.CHID == CHID
                                              orderby f.CHID descending
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
                dt = objDALCommanQuery.CallF121SP(0);

                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("CHID") == CHID).CopyToDataTable();

                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    _F121 ObjF121List = new _F121();


                    ObjF121List.CHID = Convert.ToInt32(dt2.Rows[i]["CHID"]);
                    ObjF121List.ServiceNo = dt2.Rows[i]["ServiceNo"].ToString();
                    ObjF121List.Rank = dt2.Rows[i]["Rank"].ToString();
                    ObjF121List.Name = dt2.Rows[i]["Name"].ToString();
                    ObjF121List.OffencePlace = dt2.Rows[i]["OffencePlace"].ToString();
                    ObjF121List.ChargeDate = Convert.ToDateTime(dt2.Rows[i]["ChargeDate"].ToString());
                    ObjF121List.OffenceName = dt2.Rows[i]["OffenceName"].ToString();
                    ObjF121List.OffenceWASO = dt2.Rows[i]["OffenceWASO"].ToString();
                    ObjF121List.ChargeNo = Convert.ToInt32(dt2.Rows[i]["ChargeNo"].ToString());
                    // ObjF121List.ChargeNo2 = dt2.Rows[i]["ChargeNo2"].ToString();
                    ObjF121List.OffenceDate = Convert.ToDateTime(dt2.Rows[i]["OffenceDate"].ToString());
                    ObjF121List.Appointment = dt2.Rows[i]["Appointment"].ToString();
                    ObjF121List.Comment = dt2.Rows[i]["Comment"].ToString();

                    int PunishmentID = Convert.ToInt32(dt2.Rows[i]["PunishmentID"]);
                    PunishmentName = _db.Vw_Punishments.Where(x => x.PunishmentID == PunishmentID).Select(x => x.Punishment).First();

                    ObjF121List.Punishment = PunishmentName;
                    ObjF121List.PunishmentDescription = dt2.Rows[i]["PunishmentDescription"].ToString();
                    ObjF121List.PunishDate = Convert.ToDateTime(dt2.Rows[i]["PunishDate"].ToString());
                    ObjF121List.RSID = Convert.ToInt32(dt2.Rows[i]["RSID"]);


                    int WHID = _db.F252WitnessHeader.Where(x => x.CHID == CHID && x.Active == 1).OrderByDescending(x => x.CreatedDate)
                                           .Select(x => x.WHID).FirstOrDefault();

                    ViewBag.F252WittnessPerson = _db.F252WittnessPerson.Where(x => x.WHID == WHID).ToList().OrderBy(x => x.CreatedDate);
                    ViewBag.WitnessDocuments = _db.WitnessDocuments.Where(x => x.WHID == WHID).ToList().OrderBy(x => x.CreatedDate);

                    //ItemListForJob(WHID);




                    if (dt2.Rows[i]["CurrentStatus"] != DBNull.Value)
                    {
                        TempData["CurrentStatus"] = Convert.ToInt32(dt2.Rows[i]["CurrentStatus"]);
                        TempData["SubmitStatus"] = Convert.ToInt32(dt2.Rows[i]["SubmitStatus"]);
                    }

                    if (dt2.Rows[i]["FMSID"] != DBNull.Value)
                    {
                        ObjF121List.FMSID = Convert.ToInt32(dt2.Rows[i]["FMSID"]);
                    }



                    F121List.Add(ObjF121List);
                }


                return View(F121List);
            }
            else
            {
                //when Session Expired Redirect To  login page
                return RedirectToAction("Login", "User");
            }

        }

        [HttpGet]
        public ActionResult Forward(int id, string Sno)
        {
            //Singal Forward

            ///Created By   :FLT LT RGSD GAMAGE
            ///Created Date :2022/04/02
            ///Des: Data forward to user by user, get the data from flowmanagemt table 

            //Singal Forward        
            int? UID = 0;
            if (Session["UID"] != null)
            {
                UID = Convert.ToInt32(Session["UID"]); ;
            }

            string SubmitStatus_UserRole;

            string MacAddress = new DALBase().GetMacAddress();
            //int? NextFlowStatusId;
            ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();

            if (UID != 0)
            {
                string SNO = _db.F252ChargeHeader.Where(x => x.CHID == id).Select(x => x.Sno).FirstOrDefault();
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.F252ChargeHeader.Where(x => x.CHID == id).Select(x => x.CreateLocation).FirstOrDefault();
                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();

                int? SubmitStatus = NextFlowStatusId(id, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                //Insert data to Flowstatusdetails table ow forward with RSID =2000

                F121FlowStatus objF121FlowStatus = new F121FlowStatus();
                F252ChargeHeader objF252ChargeHeader = new F252ChargeHeader();

                objF121FlowStatus.CHID = id;
                objF121FlowStatus.RSID = (int)POR.Enum.RecordStatus.Forward;
                objF121FlowStatus.UID = UID;
                objF121FlowStatus.FMSID = SubmitStatus;
                objF121FlowStatus.RID = UserRoleId;
                objF121FlowStatus.CreatedBy = UID;
                objF121FlowStatus.CreatedMac = MacAddress;
                //objF121FlowStatus.IPAddress = Request.UserHostAddress;
                objF121FlowStatus.CreatedDate = DateTime.Now;
                objF121FlowStatus.Active = 1;
                _db.F121FlowStatus.Add(objF121FlowStatus);



                // in here check OCPS process because report include only OCPS process records
                if (RoleId == 4)
                {
                    var existingModel = _db.F252ChargeHeader.Find(id);

                    if (existingModel != null)
                    {

                        existingModel.OcpsProcess = 1;

                    }

                }
                if (RoleId == 23)
                {
                    var existingModel = _db.F252ChargeHeader.Find(id);

                    if (existingModel != null)
                    {

                        existingModel.OicProcess = 1;

                    }

                }

                {
                    int count = _db.F252ChargeHeader.Where(x => x.Sno == SNO && x.Active == 1 && x.OicProcess != 1).Count();
                    int CHID = _db.F252ChargeHeader.Where(x => x.Sno == SNO && x.Active == 1 && x.OicProcess != 1).Select(x => x.CHID).FirstOrDefault();
                    if (count >= 1)
                    {
                        int? UID1 = _db.F121FlowStatus.Where(x => x.CHID == CHID && x.Active == 1).Max(x => x.UID);
                        int? RID1 = _db.F121FlowStatus.Where(x => x.UID == UID1 && x.Active == 1).Select(x => x.RID).FirstOrDefault();
                        string PendingStatus_UserRole = _db.UserRoles.Where(x => x.RID == RID1).Select(x => x.RoleName).FirstOrDefault();
                    }
                }



                if (_db.SaveChanges() > 0)
                {
                    TempData["ScfMsg"] = "Data Successfully Forwarded to " + SubmitStatus_UserRole + " " + "";
                    return RedirectToAction("IndexF121");
                }
                else
                {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                    return RedirectToAction("IndexF121");
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }


        [HttpGet]
        public ActionResult Reject(int CHID, int FMSID)
        {
            ///Created BY   : FLT RGSD GAMAGE
            ///Created Date : 2022/04/02
            /// Description : Reject method with reject POPUP window

            _F121 model = new _F121();
            try
            {
                model.CHID = CHID;
                model.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_RejectCommentF121", model);
        }

        [HttpPost]
        public JsonResult Index_Reject(string Comment, int CHID, int FMSID)
        {
            ///Created BY   : Flt Lt WAKY Wickramasinghe 
            ///Created Date : 2021/07/20
            /// Description : this function is to reject the record

            string message = "";
            string MacAddress = new DALBase().GetMacAddress();
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.F252ChargeHeader.Where(x => x.CHID == CHID).Select(x => x.CreateLocation).FirstOrDefault();
                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.RoleId).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(CHID, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();


                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//


                F121FlowStatus objF121FlowStatus = new F121FlowStatus();

                objF121FlowStatus.CHID = CHID;
                objF121FlowStatus.RSID = (int)POR.Enum.RecordStatus.Reject;
                objF121FlowStatus.UID = UID;
                objF121FlowStatus.FMSID = PreviousFMSID;
                objF121FlowStatus.RID = UserRoleId;
                objF121FlowStatus.CreatedBy = UID;
                objF121FlowStatus.CreatedMac = MacAddress;
                objF121FlowStatus.Comment = Comment;
                objF121FlowStatus.CreatedDate = DateTime.Now;
                objF121FlowStatus.Active = 1;
                _db.F121FlowStatus.Add(objF121FlowStatus);

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


                }
                else
                {
                    message = "Process Unsuccessful.Try again...";
                }

            }
            else
            {

            }
            return Json(new { Message = message, JsonRequestBehavior.AllowGet });
        }

        public ActionResult IndexRejectF121(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : FLT LT RGSD GAMAGE
            ///Created Date : 03/01/2022  
            /// Description : Index view of the all users all reject list shows accoring to the user role and the location

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_F121> F121List = new List<_F121>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                //ViewBag.DateSortParm = sortOrder == "Sno" ? "Rank" : "CreateDate";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                //int RejectRID = RoleId - 1;
                long sno = 0;

                if (searchString != null)
                {
                    page = 1;

                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                    sno = Convert.ToInt64(Sno);

                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallF121SP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 3000).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 3000).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P1CLERK:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1SNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1OIC:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);
                        ObjF121List.Comment = dt3.Rows[i]["Comment"].ToString();



                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    searchString = currentFilter;
                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.CallF121SP(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 3000).ToList();
                    //var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RSID") == 3000).CopyToDataTable();
                        //dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P1CLERK:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1SNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1OIC:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {

                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);
                        ObjF121List.Comment = dt3.Rows[i]["Comment"].ToString();

                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        //To view confirm list to ASOR OFFICER and delete option

        public ActionResult IndexConfirmF121(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            ///Created BY   : FLT LT RGSD GAMAGE
            ///Created Date : 03/01/2022  
            /// Description : Index view of the all users all reject list shows accoring to the user role and the location

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();
            List<_F121> F121List = new List<_F121>();

            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).FirstOrDefault();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

            if (UID != 0)
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Ref_No" : "";
                //ViewBag.DateSortParm = sortOrder == "Sno" ? "Rank" : "CreateDate";
                ViewBag.CurrentFilter = searchString;
                int RoleId = UserInfo.RoleId;
                //int RejectRID = RoleId - 1;
                long sno = 0;

                if (searchString != null)
                {
                    page = 1;

                    var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == searchString).Select(x => x.SNo).FirstOrDefault();
                    sno = Convert.ToInt64(Sno);

                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.OICPROCESS(sno);

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OICPROCESS") == 1).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OICPROCESS") == 1).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P1CLERK:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1SNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1OIC:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OICPROCESS") == 1).CopyToDataTable();
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);
                        ObjF121List.Comment = dt3.Rows[i]["Comment"].ToString();



                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    searchString = currentFilter;
                    ViewBag.CurrentFilter = searchString;
                    ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
                    dt = objDALCommanQuery.OICPROCESS(sno);
                    //dt =  _db.F121AllDetails.Where(x => x.OicProcess == 1).ToList();

                    var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OicProcess") == 1).ToList();
                    //var resultStatus = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).ToList();

                    if (resultStatus.Count != 0)
                    {
                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OicProcess") == 1).CopyToDataTable();
                        //dt2 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1).CopyToDataTable();
                    }
                    switch (UserInfo.RoleId)
                    {
                        case (int)POR.Enum.UserRole.P1CLERK:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1SNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.P1OIC:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.OCPSOCA:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSCLKP3VOL:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.HRMSSNCO:
                            dt3 = loadRejectDataUserWise(RoleId, dt2, LocationId, UID);
                            break;
                        case (int)POR.Enum.UserRole.ASORSOVRP3VOL:
                            dt3 = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("OICPROCESS") == 1).CopyToDataTable();
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {

                        _F121 ObjF121List = new _F121();
                        ObjF121List.CHID = Convert.ToInt32(dt3.Rows[i]["CHID"].ToString());
                        ObjF121List.ServiceNo = dt3.Rows[i]["ServiceNo"].ToString();
                        ObjF121List.Name = dt3.Rows[i]["Name"].ToString();
                        ObjF121List.Rank = dt3.Rows[i]["Rank"].ToString();
                        ObjF121List.OffenceName = dt3.Rows[i]["OffenceName"].ToString();
                        ObjF121List.OffencePlace = dt3.Rows[i]["OffencePlace"].ToString();
                        ObjF121List.ChargeDate = Convert.ToDateTime(dt3.Rows[i]["ChargeDate"]);
                        ObjF121List.Comment = dt3.Rows[i]["Comment"].ToString();

                        F121List.Add(ObjF121List);

                    }

                    pageSize = 10;
                    pageNumber = (page ?? 1);
                    return View(F121List.ToPagedList(pageNumber, pageSize));
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }


        //

        public ActionResult RejectConfirm(int id)
        {
            ///Created BY   : Flt Lt RGSD GAMAGE 
            ///Created Date : 2021/08/31
            /// Description : P3 Clerk finally confirm the reject Confirm. After confirm record Status came to 0
                     //Get the MAC Address of the User PC
            string MacAddress = new DALBase().GetMacAddress();
            int UID_ = 0;
            if (Session["UID"] != null)
            {
                UID_ = Convert.ToInt32(Session["UID"]);


                //Update F121 Active colum to 0
                F252ChargeHeader objF252ChargeHeader = _db.F252ChargeHeader.Find(id);
                objF252ChargeHeader.Active = 0;
                objF252ChargeHeader.ModifiedBy = UID_;
                objF252ChargeHeader.ModifiedDate = DateTime.Now;
                objF252ChargeHeader.ModifiedMac = MacAddress;
                _db.Entry(objF252ChargeHeader).Property(x => x.Active).IsModified = true;

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
                    return RedirectToAction("IndexRejectF121");
                }

            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
        public DataTable loadRejectDataUserWise(int RoleId, DataTable dt, string LocationId, int? UID)
        {
            ///Created BY   : Flt Lt RGSD Gamage
            ///Created Date : 2022/03/26
            /// Description : Load data user roll wise rejected data

            try
            {
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();



                switch (RoleId)
                {
                    case (int)POR.Enum.UserRole.P1CLERK:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).ToList();


                        if (result.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

                        }
                        break;
                    #endregion

                    case (int)POR.Enum.UserRole.ASORSOVRP3VOL:

                        #region CodeArea
                        /// Check the data table has row or not
                        var result2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).ToList();


                        if (result2.Count != 0)
                        {
                            dt2 = dt.AsEnumerable().Where(x => x.Field<int>("RID") == RoleId && x.Field<string>("CreateLocation") == LocationId && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();

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

                        result = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).ToList();

                        if (result.Count != 0)
                        {
                            if (AllowedCriteria == null)
                            {
                                dt2 = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject).CopyToDataTable();
                            }
                            else
                            {
                                if (AllowedCriteria.AllowVAF == true && AllowedCriteria.AllowRAF == false)
                                {
                                    var rows = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen));

                                    if (rows.Any())
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") ==
                                              (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();

                                    }
                                    else
                                    {

                                    }

                                }
                                else if (AllowedCriteria.AllowVAF == false && AllowedCriteria.AllowRAF == true)
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen)).CopyToDataTable();
                                    }
                                }
                                else
                                {
                                    var Count = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") ==
                                          (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                          x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                                    if (Count != 0)
                                    {
                                        dt2 = dt.AsEnumerable().Where(x => x.Field<int>("UID") == UID && x.Field<int>("RSID") ==
                                                                                  (int)POR.Enum.RecordStatus.Reject && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirmen ||
                                                                                  x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegAirWomen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirmen || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolAirWomen)).CopyToDataTable();
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
        public int? NextFlowStatusId(int? CHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Fg off RGSD GAMAGE
            ///Created Date :2021.03.25
            ///Des: get the next flow status id FMSID

            int? FMSID = 0;
            try
            {

                //Current Record FMSID
                int? MaxF121FSID = _db.F121FlowStatus.Where(x => x.CHID == CHID).Select(x => x.F121FSID).Max();
                int? CurrentFMSID = _db.F121FlowStatus.Where(x => x.F121FSID == MaxF121FSID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? SubmitStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);

                //LHID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null

                    SubmitStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.FlowGroup == "F" || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId && x.FlowGroup == "F")).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                }
                else
                {
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P1SNCO || CurrentUserRole == (int)POR.Enum.UserRole.P1OIC)
                    {
                        SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == "F" && (x.EstablishmentId == RecordEstablishmentId
                                                                   && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                        //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "F121" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();


                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

                    }
                    else
                    {
                        //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "F121" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                        SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                       && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();

                        //SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId
                        if (SubmitStatus == (int)POR.Enum.UserRole.HRMSCLKP3VOL)
                        {
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.HRMSCLKP3VOL && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();
                        }
                        else if (SubmitStatus == (int)POR.Enum.UserRole.CERTIFIED)
                        {
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == (int)POR.Enum.UserRole.CERTIFIED && x.FlowGroup == porFlowgroup).Select(x => x.FMSID).First();

                        }
                        else
                        {
                            FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId || x.DivisionId3 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

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
        public int? PreviousFlowStatusId(int CHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            ///Created By   : Fg off RGSD GAMAGE
            ///Created Date :2021.03.25
            ///Des: get the reject flow status id FMSID

            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxF121FSID = _db.F121FlowStatus.Where(x => x.CHID == CHID).Select(x => x.F121FSID).Max();
                int? CurrentFMSID = _db.F121FlowStatus.Where(x => x.F121FSID == MaxF121FSID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();
                int? RejectStatus = 0;
                int? UID = Convert.ToInt32(Session["UID"]);

                //FADID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null

                    RejectStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId && x.FlowGroup == "F" || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId && x.FlowGroup == "F")).Select(x => x.RejectStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();
                }
                else
                {
                    if (CurrentUserRole == (int)POR.Enum.UserRole.P1CLERK || CurrentUserRole == (int)POR.Enum.UserRole.P1OIC)
                    {
                        RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == "F" && (x.EstablishmentId == RecordEstablishmentId
                                                                  && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();
                        //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority
                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "F121" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();


                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId || x.DivisionId2 == UserDivisionId)).Select(x => x.FMSID).FirstOrDefault();

                    }
                    else
                    {
                        //// Get the pro flow grop. because this por category has to send P&R and P3 section for authority

                        var porFlowgroup = _db.PORFlowGroups.Where(x => x.PORCode == "F121" && x.Active == 1).Select(x => x.FlowGroupP3).FirstOrDefault();
                        RejectStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && x.FlowGroup == porFlowgroup && (x.EstablishmentId == RecordEstablishmentId
                                                                                       && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId || x.DivisionId == UserDivisionId)).Select(x => x.RejectStatus).FirstOrDefault();
                        FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == RejectStatus && x.FlowGroup == porFlowgroup && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).FirstOrDefault();

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        public JsonResult getname(string id)
        {
            ///Created BY   : Fg off RGSD GAMAGE
            ///Created Date : 2022/01/05
            /// Description : Load Full name details according to the service number

            Vw_PersonalDetail obj_VwPersonalProfile = new Vw_PersonalDetail();

            var count = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).Count();
            if (count == 1)
            {
                obj_VwPersonalProfile = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).FirstOrDefault();
            }
            else
            {
                obj_VwPersonalProfile.ServiceNo = "Service Number is not valid.";
            }

            return Json(obj_VwPersonalProfile, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckChargeDetails(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Get the charge number to the witness person add dev and document add dev
            ///

            F252ChargeHeader obj_F252ChargeHeader = new F252ChargeHeader();
            Vw_PersonalDetail obj_VwPersonalProfile = new Vw_PersonalDetail();

            obj_VwPersonalProfile = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == id).FirstOrDefault();

            var chargeNo = _db.F252ChargeHeader.Where(x => x.Sno == obj_VwPersonalProfile.SNo && x.Active == 1).Count();

            return Json(chargeNo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LoadWitnessesService(string ServiceNo, string Rank, string Name)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/02/02
            ///Description : Add list to service personal 

            Cls_ItemList sItm = new Cls_ItemList();

            sItm.ServiceNo = ServiceNo;
            sItm.Rank = Rank;
            sItm.FullName = Name;



            if (Session["ListItem"] != null)
            {
                lst_ListItem = (List<Cls_ItemList>)Session["ListItem"];

                lst_ListItem.Add(sItm);
                Session["ListItem"] = lst_ListItem;
            }
            else
            {
                lst_ListItem.Add(sItm);
                Session["ListItem"] = lst_ListItem;
            }


            return Json(1);
        }
        public JsonResult LoadWitnessesCivil(string FullName)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/02/02
            ///Description : Add list to Civila Personal
            ///
            Cls_ItemList sItm = new Cls_ItemList();
            sItm.FullName = FullName;

            if (Session["ListItem"] != null)
            {
                lst_ListItem = (List<Cls_ItemList>)Session["ListItem"];

                lst_ListItem.Add(sItm);
                Session["ListItem"] = lst_ListItem;
            }
            else
            {
                lst_ListItem.Add(sItm);
                Session["ListItem"] = lst_ListItem;
            }


            return Json(1);
        }

        public JsonResult GetNameFor(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Get the WitnessPerson detils with service number or name
            ///
            Cls_ItemList sItm = new Cls_ItemList();

            string SpaceRemoveSvcNo = id.Trim();
            sItm.ServiceNo = SpaceRemoveSvcNo;
            string FullName = "";
            try
            {
                var count = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Count();
                if (count == 0)
                {
                    if (sItm.ServiceNo == "")
                    {
                        FullName = "Please Enter the Witness Details";
                    }
                    else
                    {
                        FullName = sItm.ServiceNo;
                        LoadWitnessesCivil(FullName);
                    }

                }
                else if (count != 0)
                {
                    var Rank = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Rank).FirstOrDefault();
                    var Name = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Name).FirstOrDefault();
                    FullName = sItm.ServiceNo + "     " + Rank + "      " + Name;
                    LoadWitnessesService(sItm.ServiceNo, Rank, Name);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(FullName, JsonRequestBehavior.AllowGet);
        }

        // delete witness

        public JsonResult LoadWitnessesService2(string ServiceNo, string Rank, string Name)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/02/02
            ///Description : Add list to service personal 

            Cls_ItemList sItm = new Cls_ItemList();

            sItm.ServiceNo = ServiceNo;
            sItm.Rank = Rank;
            sItm.FullName = Name;



            if (Session["ListItem"] != null)
            {
                lst_ListItem = (List<Cls_ItemList>)Session["ListItem"];

                Session.Remove("ListItem");
            }
            else
            {
                Session.Remove("ListItem");
            }


            return Json(1);
        }


        public JsonResult GetNameFordelete(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Get the WitnessPerson detils with service number or name
            ///
            Cls_ItemList sItm = new Cls_ItemList();

            string SpaceRemoveSvcNo = id.Trim();
            sItm.ServiceNo = SpaceRemoveSvcNo;
            string FullName = "";
            try
            {
                var count = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Count();
                if (count == 0)
                {
                    if (sItm.ServiceNo == "")
                    {
                        var Rank = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Rank).FirstOrDefault();
                        var Name = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Name).FirstOrDefault();
                        FullName = sItm.ServiceNo + "     " + Rank + "      " + Name;
                        LoadWitnessesService2(sItm.ServiceNo, Rank, Name);
                    }
                    else
                    {
                        FullName = sItm.ServiceNo;
                        LoadWitnessesCivil(FullName);
                    }

                }
                else if (count != 0)
                {
                    var Rank = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Rank).FirstOrDefault();
                    var Name = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SpaceRemoveSvcNo).Select(x => x.Name).FirstOrDefault();
                    FullName = sItm.ServiceNo + "     " + Rank + "      " + Name;
                    LoadWitnessesService2(sItm.ServiceNo, Rank, Name);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(FullName, JsonRequestBehavior.AllowGet);
        }
        //




        public JsonResult LoadDocCetails(string Doc)
        {
            ///Create By   : Fg Off RGSD GAMAGE
            ///Create Date : 2021/01/10
            ///Description : Load the Entered Document details to Div

            Cls_ItemList sItm = new Cls_ItemList();
            sItm.DocName = Doc;



            if (Session["lst_ListPartItem"] != null)
            {
                lst_ListPartItem = (List<Cls_ItemList>)Session["lst_ListPartItem"];

                lst_ListPartItem.Add(sItm);
                Session["lst_ListPartItem"] = lst_ListPartItem;
            }
            else
            {
                lst_ListPartItem.Add(sItm);
                Session["lst_ListPartItem"] = lst_ListPartItem;
            }


            return Json(1);
        }
        public JsonResult GetNameOfDocument(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Load the document details for grid 
            ///
            Cls_ItemList sItm = new Cls_ItemList();
            sItm.DocName = id;
            string DocFullName = "";

            try
            {

                if (sItm.DocName == "")
                {
                    DocFullName = "Please Enter the Document Details";
                }
                else
                {

                    DocFullName = sItm.DocName;
                    LoadDocCetails(DocFullName);

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(DocFullName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadDocCetails2(string Doc)
        {
            ///Create By   : Fg Off RGSD GAMAGE
            ///Create Date : 2021/01/10
            ///Description : Load the Entered Document details to Div

            Cls_ItemList sItm = new Cls_ItemList();
            sItm.DocName = Doc;

            //
            if (Session["lst_ListPartItem"] != null)
            {
                //lst_ListItem = (List<Cls_ItemList>)Session["lst_ListPartItem"];

                Session.Remove("lst_ListPartItem");
            }
            else
            {
                Session.Remove("lst_ListPartItem");
            }

            ///





            return Json(1);
        }
        public JsonResult GetNameOfDocumentDelete(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Load the document details for grid 
            ///
            Cls_ItemList sItm = new Cls_ItemList();
            sItm.DocName = id;
            string DocFullName = "";

            try
            {

                if (sItm.DocName == "")
                {
                    LoadDocCetails2(DocFullName);
                }
                else
                {

                    DocFullName = sItm.DocName;
                    LoadDocCetails2(DocFullName);

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(DocFullName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNameOfProc(string id)
        {
            ///Create By   : Fg Off RGSD Gamage
            ///Create Date : 2021/01/10
            ///Description : Load the document details for grid 
            ///
            Cls_ItemList sItm = new Cls_ItemList();
            sItm.ProcName = id;
            string ProcName = "";

            try
            {

                if (sItm.ProcName == "")
                {
                    ProcName = "Please Enter the Document Details";
                }
                else
                {

                    ProcName = sItm.ProcName;
                    LoadProcCetails(ProcName);

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(ProcName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadProcCetails(string Doc)
        {
            ///Create By   : Fg Off RGSD GAMAGE
            ///Create Date : 2021/01/10
            ///Description : Load the Entered Document details to Div

            Cls_ItemList sItm = new Cls_ItemList();
            sItm.ProcName = Doc;



            if (Session["lst_ListPartItem"] != null)
            {
                lst_ListPartItem = (List<Cls_ItemList>)Session["lst_ListPartItem"];

                lst_ListPartItem.Add(sItm);
                Session["lst_ListPartItem"] = lst_ListPartItem;
            }
            else
            {
                lst_ListPartItem.Add(sItm);
                Session["lst_ListPartItem"] = lst_ListPartItem;
            }


            return Json(1);
        }

        public void InserFlowStatus(int CHID, int RoleId, int UID_, int? FMSID, int? RSID)
        {
            ///Created BY   : Fg off RGSD Gamage
            ///Created Date : 2022/02/26
            /// Description : Inset Flow status to flow status table

            try
            {
                F252ChargeHeader objF252ChargeHeader = new F252ChargeHeader();
                F121FlowStatus objF121FlowStatus = new F121FlowStatus();
                string MacAddress = new DALBase().GetMacAddress();

                objF121FlowStatus.CHID = CHID;
                objF121FlowStatus.RSID = RSID;
                objF121FlowStatus.UID = UID_;
                objF121FlowStatus.FMSID = FMSID;
                objF121FlowStatus.RID = RoleId;
                objF121FlowStatus.CreatedBy = UID_;
                objF121FlowStatus.CreatedMac = MacAddress;
                //objF121FlowStatus.IPAddress = Request.UserHostAddress;
                objF121FlowStatus.CreatedDate = DateTime.Now;
                objF121FlowStatus.Active = 1;

                _db.F121FlowStatus.Add(objF121FlowStatus);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        public ActionResult ReportCampLevel()
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 15/05/2022  
            ///Description: F121 Report view OCPS to Next level
            ///

            Session["fromDate"] = DateTime.Now;
            Session["toDate"] = DateTime.Now;
            Session["PageLoad"] = 1;
            int UID_ = 0;
            UID_ = Convert.ToInt32(Session["UID"]);

            string location = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            Session["location"] = location;

            return View();
        }

        [HttpPost]
        public ActionResult ReportCampLevel(string fromDate, string toDate)
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 15/05/2022  
            ///Description: F121 Report view OCPS to Next level

            int UID_ = 0;
            Session["PageLoad"] = 2;
            Session["fromDate"] = fromDate;
            Session["toDate"] = toDate;

            UID_ = Convert.ToInt32(Session["UID"]);

            string location = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            Session["location"] = location;

            return View();
        }
        public ActionResult ReportHqLevel()
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 02/06/2022  
            ///Description: F121 Report view HQ  level
            ///
            ViewBag.DDL_Establishment = new SelectList(_dbCommonData.Establishments, "LocShortName", "LocShortName");

            Session["fromDate"] = DateTime.Now;
            Session["toDate"] = DateTime.Now;
            Session["PageLoad"] = 1;
            int UID_ = 0;
            UID_ = Convert.ToInt32(Session["UID"]);

            string location = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
            Session["location"] = location;

            return View();
        }

        [HttpPost]
        public ActionResult ReportHqLevel(string fromDate, string toDate, string LocShortName)
        {
            ///Create By: Flt Lt RGSD GAMAGE
            ///Create Date: 02/06/2022  
            ///Description: F121 Report view HQ  level
            ///
            ViewBag.DDL_Establishment = new SelectList(_dbCommonData.Establishments, "LocShortName", "LocShortName");

            Session["PageLoad"] = 2;
            Session["fromDate"] = fromDate;
            Session["toDate"] = toDate;


            Session["location"] = LocShortName.Trim();

            return View();
        }
    }
}