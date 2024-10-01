using POR.Models;
using POR.Models.LeaveModel;
using ReportData.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data.Entity;

namespace POR.Controllers
{
    public class LeaveController : Controller
    {
        dbContext _db = new dbContext();
        DALCommanQuery objDALCommanQuery = new DALCommanQuery();
        DataTable dt = new DataTable();
        int UID = 0;
        string Sno = "";
        LeaveCount objLeaveCount = new LeaveCount();

        [HttpGet]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
            ViewBag.DateSortParm = sortOrder == "LeaveCategoryName" ? "Rank" : "FromDate";

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

           // List<Vw_Leave> objleave = new List<Vw_Leave>();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();

                   
            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
            //Create Evenet FormationId and DivisionId           

          
            var objleave = await _db.Vw_Leave.Where(x => x.Active != 0 && x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 2000 || x.RecordStatusId == 1000).ToListAsync();
            
             //objleave = _db.Vw_Leave.Where(x => x.EstablishmentId == LocationId && x.DeivisionId == UserDivisionId).ToList();

             //if (objleave.Count() <= 0)
             //{
             //    objleave = objleave.Where(x => x.EstablishmentId == LocationId && x.DivisionId == UserDivisionId).ToList();
             //}
             //else
             //{
             //    objleave = _db.Vw_Leave.Where(x => x.EstablishmentId_ == LocationId && x.DeivisionId == UserDivisionId).ToList();
             //}
            
            if (UserInfo.RoleId == 11)
            {
                objleave = await Task.Run (()=>objleave.Where(x => x.Active != 0 && x.EstablishmentId == LocationId && x.RecordStatusId != 3000).Take(500).ToList());
            }
            else if (UserInfo.RoleId == 14)
            {
                //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
                objleave = await Task.Run(()=> objleave.Where(x => x.FMSID == 135 && x.RecordStatusId == 2000).ToList());
            }
            else if (UserInfo.RoleId <= 14 && UserInfo.RoleId != 11)
            {
                objleave = await _db.Vw_Leave.Where(x => x.CurrentStatus == UserInfo.RoleId && x.EstablishmentId_ == LocationId && x.RecordStatusId == 2000 && x.RecordStatusId != 3000).ToListAsync();
            }
            else if (UserInfo.RoleId >= 14 && UserInfo.RoleId != 11)
            {
                //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
                objleave = await Task.Run (()=> objleave.Where(x => x.EstablishmentId_ == UserInfo.LocationId && (x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 2000) && x.RecordStatusId != 3000).ToList());
            }            
            if (!String.IsNullOrEmpty(searchString))
            {
                objleave = await Task.Run(()=>objleave.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString)).ToList());
            }

            switch (sortOrder)
            {
                case "Service Category":
                    objleave = objleave.OrderBy(s => s.ServiceCategoryName).ToList();
                    break;
                case "Service No":
                    objleave = objleave.OrderBy(s => s.ServiceNo).ToList();
                    break;
                case "Rank":
                    objleave = objleave.OrderBy(s => s.Rank).ToList();
                    break;
                case "Name With Initials":
                    objleave = objleave.OrderBy(s => s.Name).ToList();
                    break;
                case "Leave Category":
                    objleave = objleave.OrderBy(s => s.LeaveCategoryName).ToList();
                    break;
                case "Payment Type":
                    objleave = objleave.OrderBy(s => s.PaymentTypeName).ToList();
                    break;
                case "Total Days":
                    objleave = objleave.OrderBy(s => s.TotalDays).ToList();
                    break;
                case "From Date":
                    objleave = objleave.OrderBy(s => s.FromDate).ToList();
                    break;
                case "To Date":
                    objleave = objleave.OrderBy(s => s.ToDate).ToList();
                    break;
                case "Authority":
                    objleave = objleave.OrderBy(s => s.Authority).ToList();
                    break;
            }

            pageSize = 10;
            pageNumber = (page ?? 1);
            return View(objleave.ToPagedList(pageNumber, pageSize));

           // return View(_db.Vw_Leave.ToList());
        }        
        [HttpGet]
        public async Task<ActionResult> RejectIndex(string sortOrder, string currentFilter, string searchString, int? page, int? RSID)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
            ViewBag.DateSortParm = sortOrder == "LeaveCategoryName" ? "Rank" : "FromDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var UserInfo = _db.UserInfoes.Where(x => x.UID == UID).Select(x=> new { x.LocationId,x.RoleId } ).FirstOrDefault();

           
            ///Created By   : 38746 Cpl Madusanka
            ///Created Date :2022.09.23
            ///Des: Reject Leave view for all users....
            
            var FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == UserInfo.RoleId && x.EstablishmentId == UserInfo.LocationId).Select(x => x.FMSID).FirstOrDefault();


            /////old Code change for 2022-09-29
            //string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
            //var FMSID = _db.FlowManagementStatus.Where(x => x.DivisionId == UserDivisionId && x.EstablishmentId == LocationId).Select(x => x.FMSID).FirstOrDefault();


            //Create Evenet FormationId and DivisionId           

            var objleave = await _db.VW_LeaveTest.Take(500).ToListAsync();

            objleave = await Task.Run(()=>objleave.Where(x => x.FMSID == FMSID && x.CurrentStatus==UserInfo.RoleId).ToList());

            //objleave = _db.Vw_Leave.Where(x => x.EstablishmentId == LocationId && x.DeivisionId == UserDivisionId).ToList();

            //if (objleave.Count() <= 0)
            //{
            //    objleave = objleave.Where(x => x.EstablishmentId == LocationId && x.DivisionId == UserDivisionId).ToList();
            //}
            //else
            //{
            //    objleave = _db.Vw_Leave.Where(x => x.EstablishmentId_ == LocationId && x.DeivisionId == UserDivisionId).ToList();
            //}


            //if (UserInfo.RoleId == 11)
            //{
            //    objleave = objleave.Where(x => x.Active != 0 && x.EstablishmentId == LocationId && x.RecordStatusId == 3000).Take(500).ToList();
            //}
            //else if (UserInfo.RoleId == 14)
            //{
            //    //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
            //    objleave = objleave.Where(x => x.FMSID == 135 && x.RecordStatusId == 2000).ToList();
            //}
            //else if (UserInfo.RoleId >= 05 && UserInfo.RoleId != 11 && UserInfo.RoleId <= 09)
            //{
            //    //objleave = _db.Vw_Leave.Where(x => x.CurrentStatus == UserInfo.RoleId && x.EstablishmentId_ == LocationId && x.RecordStatusId == 3000).ToList();
            //}
            //else if (UserInfo.RoleId <= 14 && UserInfo.RoleId != 11)
            //{
            //    //objleave = _db.Vw_Leave.Where(x => x.CurrentStatus == UserInfo.RoleId && x.EstablishmentId == LocationId && x.RecordStatusId == 3000).ToList();
            //}
            //else if (UserInfo.RoleId >= 14 && UserInfo.RoleId != 11)
            //{
            //    //SubmitStatus UserRole in FlowManagementStatus = SubmitStatus
            //    objleave = objleave.Where(x => x.EstablishmentId_ == UserInfo.LocationId && (x.CurrentStatus == UserInfo.RoleId && x.RecordStatusId == 3000)).ToList();
            //}
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    objleave = objleave.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString)).ToList();
            //}

            switch (sortOrder)
            {
                case "Service Category":
                    objleave = objleave.OrderBy(s => s.ServiceCategoryName).ToList();
                    break;
                case "Service No":
                    objleave = objleave.OrderBy(s => s.ServiceNo).ToList();
                    break;
                case "Rank":
                    objleave = objleave.OrderBy(s => s.Rank).ToList();
                    break;
                case "Name With Initials":
                    objleave = objleave.OrderBy(s => s.Name).ToList();
                    break;
                case "Leave Category":
                    objleave = objleave.OrderBy(s => s.LeaveCategoryName).ToList();
                    break;
                case "Payment Type":
                    objleave = objleave.OrderBy(s => s.PaymentTypeName).ToList();
                    break;
                case "Total Days":
                    objleave = objleave.OrderBy(s => s.TotalDays).ToList();
                    break;
                case "From Date":
                    objleave = objleave.OrderBy(s => s.FromDate).ToList();
                    break;
                case "To Date":
                    objleave = objleave.OrderBy(s => s.ToDate).ToList();
                    break;
                case "Authority":
                    objleave = objleave.OrderBy(s => s.Authority).ToList();
                    break;
            }

            pageSize = 10;
            pageNumber = (page ?? 1);
            return View(objleave.ToPagedList(pageNumber, pageSize));

            // return View(_db.Vw_Leave.ToList());
        }
        [HttpGet]
        public ActionResult Create()
        {

            ViewBag.DDL_LeaveCategory = new SelectList(_db.LeaveCategories.Where(x => x.LCID == 1 || x.LCID == 4 || x.LCID == 5 || x.LCID == 16), "LCID", "LeaveCategoryName");
            //when form load Leave category selected as Genaral Leave , there for Payment type must be Full
            //Full Pay payment category =2
            ViewBag.DDL_PaymentType = new SelectList(_db.PaymentTypes.Where(x=>x.PTID==2), "PTID", "PaymentTypeName");

            ViewBag.ddlLivinStatus = new SelectList(_db.LivingStatus.Where(x => x.LSID != 0), "LSID", "LivingStatusShortName");


            if (Session["UID"].ToString() != "")
            {
                int UID_ = Convert.ToInt32(Session["UID"]);             
                int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();

                if (RoleId == 11)
                {
                    ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 2), "ServiceCategoryId", "ServiceCategoryName");
                }
                else if (RoleId == 15)
                {
                    ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 1), "ServiceCategoryId", "ServiceCategoryName");
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Create(POR.Models.LeaveModel.Leave objLeave)
        {
            LeaveHeader objLeaveHeader = new LeaveHeader();

            ViewBag.DDL_LeaveCategory = new SelectList(_db.LeaveCategories.Where(x => x.LCID == 1 || x.LCID == 4 || x.LCID == 5 || x.LCID == 16), "LCID", "LeaveCategoryName");
            ViewBag.DDL_PaymentType = new SelectList(_db.PaymentTypes.Where(x => x.PTID == 2), "PTID", "PaymentTypeName");
            ViewBag.ddlLivinStatus = new SelectList(_db.LivingStatus.Where(x => x.LSID != 0), "LSID", "LivingStatusShortName");

            #region Local VARIABLE 1

            string EstablishmentId="";
            string DivisionId="";
            int UID_ = 0;
            int count = 0;
            int RoleId = 0;
            int Status = 0;          
            int MinYearFrom = 0;
            int MinYearTo = 0;
            int MidYearFrom = 0;
            int MidYearTo = 0;
            //int AnnualLeaveSum = 0;
            int Min = 0;
            int CasualLeaveCheck = 0;
            int ToatalDaysCheck = 0;
           // int HistryCheck = 0;
                    

            #endregion

            if (Session["UID"].ToString() != "")
            {
                UID_ = Convert.ToInt32(Session["UID"]);
                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active==1).Select(x=>x.LocationId).FirstOrDefault();
                DivisionId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active==1).Select(x=>x.DivisionId).FirstOrDefault();
                RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();

                if (RoleId == 11)
                {
                    ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 2), "ServiceCategoryId", "ServiceCategoryName");                    
                }
                else if (RoleId == 15)
                {
                    ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 1), "ServiceCategoryId", "ServiceCategoryName");                    
                }
            }
            try
            {
                var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == objLeave.LeaveHeader_.ServiceNo_).Select(x => x.SNo).FirstOrDefault();
                var Enlist = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == objLeave.LeaveHeader_.ServiceNo_).Select(x => x.DateOfEnlist).FirstOrDefault();
               

                    #region Local VARIABLE

                    DateTime _fromDate = Convert.ToDateTime(objLeave.LeaveHeader_.FromDate);
                    DateTime _ToDate = Convert.ToDateTime(objLeave.LeaveHeader_.ToDate);
                    int fromyear = Convert.ToInt32(_fromDate.Year.ToString());
                    int toyear = Convert.ToInt32(_ToDate.Year.ToString());
                    int CurruntLeave = Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //int MinYearAnnualLeave = 0;
                    //int MidYearAnnualLeave = 0;
                    //int CurruntYear = 0;
                    //int i = 0;
                    int PrivilegeLeaveLeaveCount = 0;
                    //int PrivilegeLeaveTotal = 0;
                    //int AccumletedLeave = 0;
                    int AccumletedLeaveTotal = 0;

                    
                    int ThisYearLeave = 0;
                    int ThisAYearLeave = 0;
                    int ThisYearPLeave = 0;

                    //int AnnualLeaveC = 0;
                    int m = 0;
                    //int CurruntYearValues = 0;
                    int AnnualLeaveDb = 0;
                    int PrivilegeLeaveTotalDb = 0;


                    MinYearFrom = fromyear - 2;
                    MidYearFrom = fromyear - 1;

                    MinYearTo = toyear - 2;
                    MidYearTo = toyear - 1;

                   long SNumber = Convert.ToInt64(Sno);

                DateTime FromYear = Convert.ToDateTime(objLeave.LeaveHeader_.ToDate);

                int Count = _db.LeaveCounts.Where(x => x.SNo == SNumber && x.Year == FromYear.Year).Count();
                if (Count > 0)
                {
                    var LeaveHistory = _db.LeaveCounts.Where(x => x.SNo == SNumber && x.Year == FromYear.Year).Select(x => x.AccumulatedLeave).FirstOrDefault();
                    DataSet ThisYearLCount = objDALCommanQuery.ThisLeaveCount(Sno, FromYear.Year);
                    DataTable ThisYearLCounts = ThisYearLCount.Tables[0];

                    foreach (DataRow ThisYear in ThisYearLCounts.Rows)
                    {
                        ThisYearLeave = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(ThisYear["AnnualLeave"]) - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                        Min = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(ThisYear["AnnualLeave"]);
                        ThisAYearLeave = Convert.ToInt32(ThisYear["AnnualLeave"]);
                        ThisYearPLeave = Convert.ToInt32(ThisYear["PrivilegeLeave"]);
                        m = 28 - ThisYearPLeave;
                    }

                    if (ThisYearLCounts.Rows.Count == 0) //frist leave apply                    {
                    {
                        ThisYearLeave = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                        Min = Convert.ToInt32(LeaveHistory) - ThisAYearLeave;
                        AnnualLeaveDb = ThisYearLeave;
                        m = 28 - Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                        Status = 1;
                    }
                    else if (ThisYearLeave >= 0 && objLeave.LeaveDetail_.PrivilegeLeave == 0) //this year get leave count
                    {
                        AnnualLeaveDb = ThisYearLeave;
                        Status = 1;
                    }
                    else if (Min > 0)
                    {
                        if (objLeave.LeaveDetail_.PrivilegeLeave > 0 && Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) <= Min)
                        {
                            AnnualLeaveDb = ThisYearLeave;
                            PrivilegeLeaveTotalDb = m - Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                            Status = 1;
                        }
                        else
                        {
                            Status = 2;
                        }

                    }
                    else
                    {
                        if (m > 0)
                        {
                            if (objLeave.LeaveDetail_.PrivilegeLeave <= m && objLeave.LeaveDetail_.PrivilegeLeave != 0)
                            {
                                PrivilegeLeaveTotalDb = m - Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                                Status = 1;
                            }
                            else
                            {
                                Status = 4;
                            }

                        }
                        else
                        {
                            Status = 4;
                        }

                    }
                }
                else
                {
                    //add to accumalated leave 
                    LeaveCalculator(objLeave);
                    Status = 4;

                    //add and check Leave
                    var LeaveHistory = _db.LeaveCounts.Where(x => x.SNo == SNumber).Select(x => x.AccumulatedLeave).FirstOrDefault();
                    DataSet ThisYearLCount = objDALCommanQuery.ThisLeaveCount(Sno, FromYear.Year);
                    DataTable ThisYearLCounts = ThisYearLCount.Tables[0];

                    foreach (DataRow ThisYear in ThisYearLCounts.Rows)
                    {
                        ThisYearLeave = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(ThisYear["AnnualLeave"]) - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                        Min = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(ThisYear["AnnualLeave"]);
                        ThisAYearLeave = Convert.ToInt32(ThisYear["AnnualLeave"]);
                        ThisYearPLeave = Convert.ToInt32(ThisYear["PrivilegeLeave"]);
                        m = 28 - ThisYearPLeave;
                    }

                    if (ThisYearLCounts.Rows.Count == 0) //frist leave apply                    {
                    {
                        ThisYearLeave = Convert.ToInt32(LeaveHistory) - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                        Min = Convert.ToInt32(LeaveHistory) - ThisAYearLeave;
                        m = 28 - Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    }
                    else if (ThisYearLeave > 0) //this year get leave count
                    {
                        AnnualLeaveDb = ThisYearLeave;
                        Status = 1;
                    }
                    else if (Min > 0)
                    {
                        Status = 2;
                    }
                    else
                    {
                        if (m > 0)
                        {
                            if (objLeave.LeaveDetail_.PrivilegeLeave > 0)
                            {
                                PrivilegeLeaveTotalDb = m - Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                                Status = 1;
                            }
                            else
                            {
                                Status = 4;
                            }

                        }
                        else
                        {
                            Status = 4;
                        }

                    }
                }

                    #endregion
                    #region EnlistCheck Year and Annual Leave Count Summary

                //DateTime EnlistDate = Convert.ToDateTime(Enlist);

                //    if (EnlistDate.Year < DateTime.Now.Year)
                //    {
                //        DataSet ds = objDALCommanQuery.AnnualLeavePlane(MinYearFrom, MidYearFrom, fromyear);
                //        DataTable Dt = ds.Tables[0];
                //        foreach (DataRow item in Dt.Rows)
                //        {
                //            AnnualLeaveC = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //            if (i == 0)
                //            {
                //                MinYearAnnualLeave = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //                i++;
                //            }
                //            else if (i == 1)
                //            {
                //                MidYearAnnualLeave = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //                AnnualLeaveSum = MinYearAnnualLeave + MidYearAnnualLeave;
                //                i++;
                //            }
                //            else
                //            {
                //                CurruntYear = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //            }
                //        }
                //    }
                //    else
                //    {
                //        DataSet ds = objDALCommanQuery.AnnualLeavePlane(EnlistDate.Year, EnlistDate.Year, fromyear);
                //        DataTable Dt = ds.Tables[0];
                //        foreach (DataRow item in Dt.Rows)
                //        {
                //            int counts = Dt.Rows.Count;
                //            AnnualLeaveC = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //            if (counts == 1)
                //            {
                //                CurruntYear = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //            }
                //            else if (i == 0)
                //            {
                //                MinYearAnnualLeave = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //                i++;
                //            }
                //            else if (i == 1)
                //            {
                //                MidYearAnnualLeave = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //                AnnualLeaveSum = MinYearAnnualLeave + MidYearAnnualLeave;
                //                i++;
                //            }
                //            else
                //            {
                //                CurruntYear = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                //            }
                //        }
                //    }
                    #endregion
                    #region AnnualLeaveCount

                    //int MidStatus = 0;
                    //int MinStatus = 0;
                    //DataSet AnnualLeaveCount = objDALCommanQuery.AnnualLeaveCount(Sno, MinYearFrom, MidYearFrom, fromyear);

                    //DataTable AnnualLeaveCounts = AnnualLeaveCount.Tables[0];

                    //if (AnnualLeaveCounts.Rows.Count <= 0)
                    //{
                    //    MinStatus = 1;
                    //    MidStatus = 1;
                    //}

                    //foreach (DataRow item in AnnualLeaveCounts.Rows)
                    //{
                    //    int FYear = Convert.ToInt32(item["D"].ToString());
                    //    int TYear = Convert.ToInt32(item["Todate"].ToString());

                    //    if (!String.IsNullOrEmpty(item["PrivilegeLeave"].ToString()))
                    //    {
                    //        PrivilegeLeaveLeaveCount = Convert.ToInt32(item["PrivilegeLeave"].ToString());
                    //    }
                    //    AccumletedLeave = Convert.ToInt32(item["AnnualLeave"].ToString());
                    //    if (FYear == MinYearFrom)
                    //    {
                    //        MinStatus = 1;
                    //    }
                    //    if (FYear != fromyear)
                    //    {
                    //        // PrivilegeLeaveTotal = PrivilegeLeaveTotal + PrivilegeLeaveLeaveCount;                       
                    //        if (MidYearFrom == FYear)
                    //        {
                    //            MidStatus = 1;
                    //            PrivilegeLeaveTotal = PrivilegeLeaveLeaveCount;
                    //        }
                    //        AccumletedLeaveTotal = Convert.ToInt32(item["AnnualLeave"].ToString());
                    //    }
                    //    else if (fromyear == FYear)
                    //    {
                    //        CurruntYearValues = 1;
                    //        AccumletedLeaveTotal = Convert.ToInt32(item["AnnualLeave"].ToString());
                    //        if (!String.IsNullOrEmpty(item["PrivilegeLeave"].ToString()))
                    //        {
                    //            PrivilegeLeaveLeaveCount = Convert.ToInt32(item["PrivilegeLeave"].ToString());
                    //        }
                    //    }

                    //}
                    #endregion
                    #region FristTime Data Save (Year)

                    //if (CurruntYearValues == 0)
                    //{
                    //    if (PrivilegeLeaveTotal > 0 && AnnualLeaveCounts.Rows.Count > 0)
                    //    {
                    //        Min = MidYearAnnualLeave - PrivilegeLeaveTotal;
                    //        if (PrivilegeLeaveTotal < CurruntYear)
                    //        {
                    //            if (Min > Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave))
                    //            {
                    //                if (Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) <= 0)
                    //                {
                    //                    AnnualLeaveDb = MidYearAnnualLeave - PrivilegeLeaveTotal - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                    Status = 1;
                    //                }
                    //                else
                    //                {
                    //                    Status = 2;
                    //                }
                    //            }
                    //            else if (objLeave.LeaveDetail_.AnnualLeave == Min)
                    //            {
                    //                AnnualLeaveDb = MidYearAnnualLeave - PrivilegeLeaveTotal - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //                Status = 1;
                    //            }
                    //            else
                    //            {
                    //                Status = 2;
                    //            }

                    //        }
                    //        else
                    //        {
                    //            if (Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) <= 0)
                    //            {
                    //                PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //                Status = 1;
                    //            }
                    //            else if (EnlistDate.Year == MidYearFrom)
                    //            {
                    //                if (PrivilegeLeaveLeaveCount > 0)
                    //                {
                    //                    int h = MinYearAnnualLeave - PrivilegeLeaveLeaveCount;
                    //                    AnnualLeaveDb = MidYearAnnualLeave + h - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                    Status = 1;
                    //                }
                    //                else
                    //                {
                    //                    AnnualLeaveDb = AnnualLeaveDb - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) + MidYearAnnualLeave;
                    //                    Status = 1;
                    //                }

                    //            }
                    //            else
                    //            {
                    //                Status = 3;
                    //            }
                    //        }

                    //    }
                    //    else if (AnnualLeaveSum > AccumletedLeaveTotal && CurruntYearValues == 0)
                    //    {
                    //        //Status = 1; 
                    //        Min = 0;
                    //        Min = AccumletedLeaveTotal;

                    //        if (MinYearAnnualLeave > AccumletedLeaveTotal)
                    //        {
                    //            // AnnualLeaveDb = AccumletedLeaveTotal + MinYearAnnualLeave;
                    //            AnnualLeaveDb = AccumletedLeaveTotal;

                    //            if (EnlistDate.Year < MidYearFrom && EnlistDate.Year < MinYearFrom && MidStatus == 0 && MinStatus == 0)
                    //            {
                    //                AnnualLeaveDb = MinYearAnnualLeave + MinYearAnnualLeave;
                    //                Status = 1;
                    //            }
                    //            if (EnlistDate.Year < MinYearFrom && MinStatus == 0)
                    //            {
                    //                AnnualLeaveDb = AnnualLeaveDb - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) + MinYearAnnualLeave;
                    //                Status = 1;
                    //            }
                    //            else if (EnlistDate.Year < MidYearFrom && MidStatus == 0)
                    //            {
                    //                if (PrivilegeLeaveLeaveCount > 0)
                    //                {
                    //                    int h = MinYearAnnualLeave - PrivilegeLeaveLeaveCount;
                    //                    AnnualLeaveDb = MidYearAnnualLeave + h - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                    Status = 1;
                    //                }
                    //                else
                    //                {
                    //                    AnnualLeaveDb = AnnualLeaveDb - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) + MidYearAnnualLeave;
                    //                    Status = 1;
                    //                }

                    //            }
                    //            else if (AnnualLeaveDb >= objLeave.LeaveDetail_.AnnualLeave)
                    //            {
                    //                // AnnualLeaveDb = AnnualLeaveDb - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                if (Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) > 0)
                    //                {
                    //                    PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //                }
                    //                else
                    //                {
                    //                    AnnualLeaveDb = MidYearAnnualLeave + AnnualLeaveDb - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                    Status = 1; //Save
                    //                }

                    //            }
                    //            else
                    //            {

                    //                Status = 2;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Min = AccumletedLeaveTotal;

                    //            if (AccumletedLeaveTotal >= Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave))
                    //            {
                    //                AnnualLeaveDb = AccumletedLeaveTotal - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //                if (Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) > 0)
                    //                {
                    //                    PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //                }
                    //                Status = 1;
                    //            }
                    //            else
                    //            {
                    //                Status = 2;
                    //            }

                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (EnlistDate.Year == DateTime.Now.Year)
                    //        {
                    //            if (Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) <= 0)
                    //            {
                    //                PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //                Status = 1;
                    //            }
                    //            else
                    //            {
                    //                Status = 3;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            AnnualLeaveDb = MinYearAnnualLeave + MidYearAnnualLeave - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //            Status = 1;
                    //        }

                    //    }
                    //}

                    #endregion
                    #region Second Time Data Save (Year)

                    //if (AccumletedLeaveTotal > 0 && CurruntYearValues == 1)
                    //{
                    //    Min = AccumletedLeaveTotal;
                    //    if (Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) <= 0)
                    //    {
                    //        if (Min >= objLeave.LeaveDetail_.AnnualLeave)
                    //        {
                    //            AnnualLeaveDb = AccumletedLeaveTotal - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    //            Status = 1; //Save  
                    //        }
                    //        else
                    //        {
                    //            Status = 2;
                    //        }

                    //    }
                    //    else if (Min == Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) && CurruntYearValues == 1)
                    //    {
                    //        AnnualLeaveDb = AccumletedLeaveTotal - Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);

                    //        PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //        Status = 1; //Save                       

                    //    }
                    //    else
                    //    {
                    //        Status = 2; //This person have '" + Min + "' Accumulated Leave...
                    //    }
                    //}
                    //else if (PrivilegeLeaveLeaveCount < CurruntYear && CurruntYearValues == 1)
                    //{
                    //    m = CurruntYear - PrivilegeLeaveLeaveCount;
                    //    int mc = CurruntYear - PrivilegeLeaveLeaveCount;
                    //    if (objLeave.LeaveDetail_.PrivilegeLeave <= 0)
                    //    {
                    //        Status = 4;
                    //    }
                    //    else if (Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) <= m)
                    //    {
                    //        PrivilegeLeaveTotalDb = PrivilegeLeaveLeaveCount + Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    //        Status = 1;
                    //    }
                    //    else
                    //    {
                    //        Status = 4;
                    //    }
                    //}
                    //else if (CurruntYearValues == 1)
                    //{
                    //    Status = 5;
                    //}
                    #endregion

                    #region DupplicateDateCheck

                    count = _db.LeaveHeaders.Where(x => x.Sno == Sno && x.FromDate <= objLeave.LeaveHeader_.FromDate && x.ToDate >= objLeave.LeaveHeader_.ToDate && x.Active != 0).Count();

                    if (count == 0)
                    {
                        count = _db.LeaveHeaders.Where(x => x.Sno == Sno && x.FromDate >= objLeave.LeaveHeader_.FromDate && x.FromDate <= objLeave.LeaveHeader_.ToDate && x.Active != 0).Count();

                        if (count == 0)
                        {
                            count = _db.LeaveHeaders.Where(x => x.Sno == Sno && x.ToDate >= objLeave.LeaveHeader_.FromDate && x.ToDate <= objLeave.LeaveHeader_.ToDate && x.Active != 0).Count();
                        }
                    }

                    #endregion

                    #region CasualLeave Check

                    if (objLeave.LeaveDetail_.CasualLeave != 0 && objLeave.LeaveDetail_.CasualLeave <= 3 && Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave) <= 0 && Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave) <= 0)
                    {
                        int AnnualCasualLeave = objDALCommanQuery.AnnualLeaveplaneCasualCheck(fromyear);
                        int CasualLeaveLeaveCount = objDALCommanQuery.CasualLeaveCount(Sno, fromyear);

                        if (AnnualCasualLeave >= CasualLeaveLeaveCount + Convert.ToInt32(objLeave.LeaveDetail_.CasualLeave))
                        {
                            if (objLeave.LeaveDetail_.Weekend == 0 && objLeave.LeaveDetail_.PublicHoliday == 0)
                            {
                                CasualLeaveCheck = 1; //Success Save
                                Status = 1;
                                PrivilegeLeaveTotalDb = PrivilegeLeaveLeaveCount;
                                AnnualLeaveDb = AccumletedLeaveTotal;
                            }
                            else
                            {
                                CasualLeaveCheck = 4; //dont use CasualLeave with Weekends  
                            }

                        }
                        else
                        {
                            CasualLeaveCheck = 2;//Annual Leave Over
                        }
                    }
                    else if (objLeave.LeaveDetail_.CasualLeave == 0)
                    {
                        CasualLeaveCheck = 1;//Success Save

                    }
                    else
                    {
                        CasualLeaveCheck = 3; //Maximum three days Only
                    }

                    #endregion

                    #region Model Status Check

                    objLeave.LeaveHeader_.EstablishmentId = EstablishmentId;
                    ModelState.Remove("LeaveHeader_.EstablishmentId");
                    if (objLeave.LeaveHeader_.LeaveCategoryId != 1)
                    {
                        ModelState.Remove("LeaveDetail_.PrivilegeLeave");
                        ModelState.Remove("LeaveDetail_.CasualLeave");
                        ModelState.Remove("LeaveDetail_.AnnualLeave");
                        ModelState.Remove("LeaveDetail_.LeaveLeave");
                        ModelState.Remove("LeaveDetail_.Weekend");
                        ModelState.Remove("LeaveDetail_.PublicHoliday");
                    }
                    #endregion

                    #region Total Day Check

                    int PrivilegeLeave = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                    int CasualLeave = Convert.ToInt32(objLeave.LeaveDetail_.CasualLeave);
                    int AnnualLeave = Convert.ToInt32(objLeave.LeaveDetail_.AnnualLeave);
                    int PublicHoliday = Convert.ToInt32(objLeave.LeaveDetail_.PublicHoliday);
                    int LeaveLeave = Convert.ToInt32(objLeave.LeaveDetail_.LeaveLeave);
                    int ReEngagementLeave = Convert.ToInt32(objLeave.LeaveDetail_.Re_engagement);

                    decimal WeekendValues = Convert.ToDecimal(objLeave.LeaveDetail_.Weekend) * 2;

                    int totalLeave = Convert.ToInt32(PrivilegeLeave + CasualLeave + AnnualLeave + WeekendValues + PublicHoliday + LeaveLeave + ReEngagementLeave);
                    if (totalLeave == objLeave.LeaveHeader_.TotalDays)
                    {
                        ToatalDaysCheck = 1;//Success Total Days
                    }
                    else
                    {
                        ToatalDaysCheck = 2;//Miss Match Total Days Count
                    }

                    #endregion

                    # region LeaveLeave
                    if (LeaveLeave != 0)
                    {
                        Status = 1;
                        CasualLeaveCheck = 1;
                        PrivilegeLeaveTotalDb = PrivilegeLeaveLeaveCount;
                        AnnualLeaveDb = AccumletedLeaveTotal;
                    }

                    #endregion

                    #region SickLeave
                    if (objLeave.LeaveHeader_.LeaveCategoryId == 4)
                    {
                        Status = 1;
                        CasualLeaveCheck = 1;
                        ToatalDaysCheck = 1;
                        PrivilegeLeaveTotalDb = PrivilegeLeaveLeaveCount;
                        AnnualLeaveDb = AccumletedLeaveTotal;
                    }
                    #endregion

                    #region Maternity Leave
                    if (objLeave.LeaveHeader_.LeaveCategoryId == 5)
                    {
                        Status = 1;
                        CasualLeaveCheck = 1;
                        ToatalDaysCheck = 1;
                        PrivilegeLeaveTotalDb = PrivilegeLeaveLeaveCount;
                        AnnualLeaveDb = AccumletedLeaveTotal;
                    }
                    #endregion 
                                   
                    #region Check Discharge leave

                    if (objLeave.LeaveHeader_.LeaveCategoryId == 16)
                   {
                       if (objLeave.LeaveDetail_.PrivilegeLeave > 0)
                       {
                           if (objLeave.LeaveDetail_.PrivilegeLeave <= 28)
                           {
                               PrivilegeLeaveTotalDb = Convert.ToInt32(objLeave.LeaveDetail_.PrivilegeLeave);
                           }
                           else
                           {
                               PrivilegeLeaveTotalDb = 28;
                           }
                       }
                       else
                       {

                       }
                       count = 0;
                       Status = 1;
                       CasualLeaveCheck = 1; 
                   }
                 else
                  {

                  }
                    #endregion

                if (ModelState.IsValid && count == 0 && Status == 1 && !String.IsNullOrEmpty(objLeave.LeaveHeader_.ServiceNo_) && CasualLeaveCheck == 1 && ToatalDaysCheck == 1)
                   {
                    objLeaveHeader.Sno = Sno;
                    objLeaveHeader.EstablishmentId = objLeave.LeaveHeader_.EstablishmentId;
                    objLeaveHeader.DeivisionId = DivisionId;
                    objLeaveHeader.LeaveCategoryId = objLeave.LeaveHeader_.LeaveCategoryId;

                    if (RoleId == 11)
                    {
                        objLeaveHeader.ServiceCategoryId = 2;
                    }
                    else if (RoleId == 15)
                    {
                        objLeaveHeader.ServiceCategoryId = 1;
                    }
                    else
                    {

                    }
                    objLeaveHeader.FromDate = objLeave.LeaveHeader_.FromDate;
                    objLeaveHeader.ToDate = objLeave.LeaveHeader_.ToDate;

                    int? MaxLHID = _db.LeaveHeaders.Where(x => x.EstablishmentId == EstablishmentId && x.LeaveCategoryId == objLeaveHeader.LeaveCategoryId).Select(x => x.LHID).Count() + 1;
                    string LeaveCategoryShortName = _db.LeaveCategories.Where(x => x.LCID == objLeaveHeader.LeaveCategoryId).Select(x => x.LeaveCategoryShortName).FirstOrDefault();
                    string RefNo = EstablishmentId.ToString() + "/" + LeaveCategoryShortName.ToString() + "/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + MaxLHID;

                    objLeaveHeader.Authority = RefNo;
                    objLeaveHeader.AnnualLeave = AnnualLeaveDb;
                    objLeaveHeader.PrivilegeLeave = PrivilegeLeaveTotalDb;

                    //Record Status when data insert
                    objLeaveHeader.PorStatus = 1000;
                    objLeaveHeader.PaymentTypeId = objLeave.LeaveHeader_.PaymentTypeId;
                    objLeaveHeader.TotalDays = objLeave.LeaveHeader_.TotalDays;
                    objLeaveHeader.CreatedBy = UID_;
                    objLeaveHeader.CreatedDate = DateTime.Now;
                    objLeaveHeader.Active = 1;
                    objLeaveHeader.LivingStatus = objLeave.LeaveHeader_.LivingStatus;
                    //int d = Convert.ToInt32(objLeave.LeaveHeader_.FromDate - objLeave.LeaveHeader_.ToDate); 

                    _db.LeaveHeaders.Add(objLeaveHeader);

                    int Id = 0;
                    Id = new DALCommanQuery().NextLeaveHeaderNextId();

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                    {
                        //Check Leave Category, whether its not equals to 1 its just save without check trasaction Proce
                        int? FirstFMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == EstablishmentId && x.DivisionId==DivisionId).Select(x => x.FMSID).First();

                        ///////38746 Edit Save SickLeave And Maternity Leave 

                        //if (objLeave.LeaveHeader_.LeaveCategoryId != 2)
                        //{
                        //    if (_db.SaveChanges() > 0)
                        //    {
                        //        LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();                             
                        //        objLeaveFlowStatu.LeaveHeaderId = Id;
                        //        objLeaveFlowStatu.FMSID = null;
                        //        objLeaveFlowStatu.RecordStatusId = 1000;
                        //        objLeaveFlowStatu.CreatedBy = UID_;
                        //        objLeaveFlowStatu.CreatedDate = DateTime.Now;
                        //        objLeaveFlowStatu.CreatedMac = ;
                        //        objLeaveFlowStatu.Active = 1;
                        //        _db.LeaveFlowStatus.Add(objLeaveFlowStatu);

                        //        if (_db.SaveChanges() > 0)
                        //        {
                        //            scope.Complete();
                        //            TempData["ScfMsg"] = "Data Successfully Saved";
                        //        }
                        //        else
                        //        {
                        //            scope.Dispose();
                        //            TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                        //        }
                                
                        //    }
                        //    else
                        //    {
                        //        scope.Dispose();
                        //        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                        //    }

                        //}
                        if (_db.SaveChanges() > 0 )
                        {
                            LeaveDetail objLeaveDetail = new LeaveDetail();

                            objLeaveDetail.LeaveHeaderId = Id;
                            objLeaveDetail.PrivilegeLeave = objLeave.LeaveDetail_.PrivilegeLeave;
                            objLeaveDetail.CasualLeave = objLeave.LeaveDetail_.CasualLeave;
                            objLeaveDetail.AnnualLeave = objLeave.LeaveDetail_.AnnualLeave;
                            objLeaveDetail.LeaveLeave = objLeave.LeaveDetail_.LeaveLeave;
                            objLeaveDetail.Weekend = objLeave.LeaveDetail_.Weekend;
                            objLeaveDetail.PublicHoliday = objLeave.LeaveDetail_.PublicHoliday;
                            objLeaveDetail.ReEngagementLeave = objLeave.LeaveDetail_.Re_engagement;
                            objLeaveDetail.CreatedBy = UID_;
                            objLeaveDetail.CreatedDate = DateTime.Now;
                            objLeaveDetail.Active = 1;
                            _db.LeaveDetails.Add(objLeaveDetail);                         

                           
                                if (_db.SaveChanges() > 0)
                                {
                                    LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();
                                    objLeaveFlowStatu.LeaveHeaderId = Id;
                                    objLeaveFlowStatu.FMSID = null;
                                    objLeaveFlowStatu.RecordStatusId = 1000;
                                    objLeaveFlowStatu.CreatedBy = UID_;
                                    objLeaveFlowStatu.CreatedDate = DateTime.Now;
                                    //objLeaveFlowStatu.CreatedMac = ;
                                    objLeaveFlowStatu.Active = 1;
                                    _db.LeaveFlowStatus.Add(objLeaveFlowStatu);
                                    if (_db.SaveChanges() > 0)
                                    {
                                        scope.Complete();
                                        TempData["ScfMsg"] = "Data Successfully Saved  - Authority = '"+RefNo+"'";
                                        Session["ServiceNo"] = "";
                                        Session["Sno"] = "";
                                        return RedirectToAction("Create");

                                    }
                                    else
                                    {
                                        scope.Dispose();
                                        TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                                    }

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
                }
                else
                {
                    if (Status==2)
                    {
                        TempData["ErrMsg"] = "This person have '" + Min + "' Accumulated Leave...";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (CasualLeaveCheck == 4)
                    {
                        TempData["ErrMsg"] = "Dont use Casual Leave with Weekends..";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (CasualLeaveCheck == 3)
                    {
                        TempData["ErrMsg"] = "Maximum three days Only..";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }     
                    else if (Status == 3)
                    {
                        TempData["ErrMsg"] = "This person don't have Accumulated Leave...";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (Status == 4)
                    {
                        TempData["ErrMsg"] = "This person don't have Accumulated and ( *avilable '" + m + "' Privilege Leave) ...";
                        
                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (Status == 5)
                    {
                        TempData["ErrMsg"] = "This person don't have Privilege Leave...";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (CasualLeaveCheck == 2)
                    {
                        TempData["ErrMsg"] = "This year Casual Leave Count is Over..";
                       
                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                                 
                    else if (objLeave.LeaveHeader_.ServiceNo_=="")
                    {
                        TempData["ErrMsg"] = "Please Check Service Number";
                        
                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (ToatalDaysCheck==2)
                  	{
                        TempData["ErrMsg"] = "Total leave count miss match with total days..";
                       
                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
	                }
                    else if (count==1)
                    {
                        TempData["ErrMsg"] = "Please check, Date is Duplicate..";
                        
                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else if (count == 10)
                    {
                        TempData["ScfMsg"] = "Leave Process Completed. Please click Save Button again to Save the details.";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                    else 
                    {
                        TempData["ErrMsg"] = "Please check, values..";

                        Session["Sno"] = Sno;
                        var ObjPersonalInfo = _db.Vw_PersonalDetail.Where(x => x.SNo == Sno).First();
                        string FullName = ObjPersonalInfo.ServiceNo + "-" + ObjPersonalInfo.Rank + ObjPersonalInfo.Name;
                        Session["ServiceNo"] = FullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpPost]
        public JsonResult ReferenceNo(int LeaveCategoryId, string SvcNo_)
        {
            if (Session["UID"].ToString() != "")
            {
                UID = Convert.ToInt32(Session["UID"]);                
            }
             Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == SvcNo_).Select(x => x.SNo).FirstOrDefault();
            //FixedAllowanceDetail objFixedAllowanceDetail = new FixedAllowanceDetail();
            _LeaveHeader objLeaveHeader = new _LeaveHeader();
             string EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
             int? MaxLHID = _db.LeaveHeaders.Where(x => x.EstablishmentId == EstablishmentId  && x.LeaveCategoryId == LeaveCategoryId).Select(x => x.LHID).Count() + 1;

            string LeaveCategoryShortName = _db.LeaveCategories.Where(x => x.LCID == LeaveCategoryId).Select(x => x.LeaveCategoryShortName).FirstOrDefault();
            int CasualLeaveLeaveCount = objDALCommanQuery.CasualLeaveCount(Sno, DateTime.Now.Year);
            int ReEngagementLeave = objDALCommanQuery.ReEngagementLeave(Sno);
            int ReEngagementLeaveHistry = objDALCommanQuery.ReEngagementLeaveHistry(Sno);
            int ReEngagement = 0;

            if (ReEngagementLeaveHistry>0)
            {
                if (ReEngagementLeave==0)
                {
                     ReEngagement = ReEngagementLeaveHistry ;
                }
                else
                {
                    ReEngagement = ReEngagementLeaveHistry - ReEngagementLeave;
                }
             
            }
            else
            {
                ReEngagement = 28 - ReEngagementLeave;
            }


           //// string RefNo = EstablishmentId.ToString() + "/" + LeaveCategoryShortName.ToString() + "/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + MaxLHID;
           // objLeaveHeader.Authority = RefNo.ToString();
            objLeaveHeader.LeaveCategoryId = LeaveCategoryId;
          
            #region Leave Count   
         
            var DischargeLeave = _db.LeaveHeaders.Where(x => x.Sno == Sno && x.LeaveCategoryId == 16 && x.Active !=0).Count();

            if (DischargeLeave<=0)
            {

                int AnnualLeaveC = 0;
                DataSet ds = objDALCommanQuery.AnnualLeavePlane(0, 0, DateTime.Now.Year);
                DataTable Dt = ds.Tables[0];
                foreach (DataRow item in Dt.Rows)
                {
                    AnnualLeaveC = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                }

                try
                {
                    var LHID = _db.LeaveHeaders.Where(x => x.Sno == Sno).Select(x => x.LHID).Max();
                    var LeaveType = _db.LeaveHeaders.Where(x => x.LHID == LHID).FirstOrDefault();
                    objLeaveHeader.AccumulatedLeave = LeaveType.AnnualLeave;
                    objLeaveHeader.PrivilegeLeave = AnnualLeaveC - LeaveType.PrivilegeLeave;
                    if (LeaveType.PrivilegeLeave == null)
                    {
                        objLeaveHeader.PrivilegeLeave = AnnualLeaveC;
                    }
                    objLeaveHeader.CasualLeave = 17 - CasualLeaveLeaveCount;
                    objLeaveHeader.Reengagement = ReEngagement;
                    objLeaveHeader.PorStatus = 1000;
                }
                catch
                {
                    objLeaveHeader.AccumulatedLeave = 0;
                    objLeaveHeader.PrivilegeLeave = AnnualLeaveC;
                    objLeaveHeader.CasualLeave = 0;
                    objLeaveHeader.Reengagement = 0;
                    objLeaveHeader.PorStatus = 1000;
                }
            }
            else
            {
                objLeaveHeader.AccumulatedLeave = 0;
                objLeaveHeader.PrivilegeLeave = 0;
                objLeaveHeader.CasualLeave = 0;
                objLeaveHeader.Reengagement = 0;
                objLeaveHeader.PorStatus = 100;
                objLeaveHeader.Authority="";
            }
            #endregion

            return Json(objLeaveHeader, JsonRequestBehavior.AllowGet);
        }
        public void LeaveCalculator(POR.Models.LeaveModel.Leave objLeave)
        {
            /// Create By: Cpl Madusanka
            /// Create Date: 27/01/2023  
            /// Description: Leave Count For previous Year  

            #region Variable

            int NowYear = Convert.ToInt32(DateTime.Now.Year.ToString());
            int MidYearFrom = NowYear - 1;
            int MinYearFrom = NowYear - 2;
            int Count = 0;
            int MidYearFromLeave = 0;
            int MinYearFromLeave = 0;
            int MidYearFromLeaveP = 0;
            int MinYearFromLeaveP = 0;
            int MidYear = 0;
            int MinYear = 0;
            int AccumlatedLeaveCount = 0;
            int PrevilageLeaveLeaveCount = 0;
            int SaveLeaveCount = 0;
            int TotalAccumlatedLeaveNextYear = 0;
            int PrevilageLeave = 0;
            int TotalPrevilageLeaveNextYear = 0;

            #endregion

            var Enlist = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == objLeave.LeaveHeader_.ServiceNo_).Select(x => x.DateOfEnlist).FirstOrDefault();
            DateTime EnlistDate = Convert.ToDateTime(Enlist);

            var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == objLeave.LeaveHeader_.ServiceNo_).Select(x => x.SNo).FirstOrDefault();
            long CSn = Convert.ToInt64(Sno);
            int NYear = DateTime.Now.Year - 1;

            if (NowYear == 2023)
            {
                AccumlatedLeaveCount = 56;
                PrevilageLeaveLeaveCount = 0;
            }
            else
            {
                var AccumlatedLeaveCount1 = _db.LeaveCounts.Where(x => x.SNo == CSn & x.Year == NYear).Select(x => x.AccumulatedLeave).FirstOrDefault();
                AccumlatedLeaveCount = Convert.ToInt32(AccumlatedLeaveCount1);
            }

            // previous Year of 2  LeaveCount **
            DataSet AnnualLeaveCount = objDALCommanQuery.LeaveCount(Sno, MinYearFrom, MidYearFrom);
            DataTable AnnualLeaveCounts = AnnualLeaveCount.Tables[0];
            foreach (DataRow item in AnnualLeaveCounts.Rows)
            {
                Count++;
                if (Count == 1)
                {
                    MidYearFromLeave = Convert.ToInt32(item["AnnualLeave"].ToString());
                    MidYearFromLeaveP = Convert.ToInt32(item["PrivilegeLeave"].ToString());
                    MidYear = Convert.ToInt32(item["FromDate"].ToString());
                }
                else
                {
                    MinYearFromLeave = Convert.ToInt32(item["AnnualLeave"].ToString());
                    MinYearFromLeaveP = Convert.ToInt32(item["PrivilegeLeave"].ToString());
                    MinYear = Convert.ToInt32(item["FromDate"].ToString());
                }

            }
            //Leave Count For previous Year  
            int LeaveCount = MidYearFromLeave + MinYearFromLeave;

            if (AccumlatedLeaveCount >= LeaveCount)
            {
                SaveLeaveCount = AccumlatedLeaveCount - LeaveCount;
                if (SaveLeaveCount <= 28)
                {
                    TotalAccumlatedLeaveNextYear = SaveLeaveCount + 28;
                }
                else
                {
                    TotalAccumlatedLeaveNextYear = 56;
                }

            }
            else
            {
                PrevilageLeave = 28 - MinYearFromLeaveP;


                if (PrevilageLeave > 0)
                {
                    TotalAccumlatedLeaveNextYear = PrevilageLeave;
                }
                else
                {
                    TotalAccumlatedLeaveNextYear = 0;
                    TotalPrevilageLeaveNextYear = 28;
                }


            }
            if (EnlistDate.Year == DateTime.Now.Year) //Check Enlist Date 
            {
                TotalAccumlatedLeaveNextYear = 0;
                TotalPrevilageLeaveNextYear = 28;
            }

            objLeaveCount.AccumulatedLeave = TotalAccumlatedLeaveNextYear;
            objLeaveCount.PrivilegeLeave = TotalPrevilageLeaveNextYear;
            objLeaveCount.SNo = Convert.ToInt64(Sno);

            DateTime FromYear = Convert.ToDateTime(objLeave.LeaveHeader_.ToDate);

            objLeaveCount.Year = FromYear.Year;
            objLeaveCount.CreatedBy = UID;
            objLeaveCount.CreatedDate = DateTime.Now;
            objLeaveCount.Active = 1;
            _db.LeaveCounts.Add(objLeaveCount);
            _db.SaveChanges();

        }

        public JsonResult PaymentType(int LeaveCategoryId)
        {
            //  LeaveHeader objLeaveHeader = new LeaveHeader();
            Leave objLeaveHeader = new Leave();
            LeaveDetail objLeaveDetail = new LeaveDetail();
            List<PublicHolidayCalender> objPublicHoliday = new List<PublicHolidayCalender>();
            int PaymentTypeId = 0;
             //int PaymentTypeId = Convert.ToInt32(obj_LeaveHeader.PaymentTypeId);

            if (LeaveCategoryId == 1 || LeaveCategoryId == 4 )
            {
                PaymentTypeId = 2;
            }
            else
            {
                PaymentTypeId = 1;
            }

            return Json(PaymentTypeId, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DateCalc(_LeaveHeader obj_LeaveHeader)
        {                     
          //  LeaveHeader objLeaveHeader = new LeaveHeader();
            Leave objLeaveHeader = new Leave();
            LeaveDetail objLeaveDetail = new LeaveDetail();
            List<PublicHolidayCalender> objPublicHoliday = new List<PublicHolidayCalender>();
           

            DateTime FromDate = Convert.ToDateTime(obj_LeaveHeader.FromDate);
            DateTime ToDate = Convert.ToDateTime(obj_LeaveHeader.ToDate);
            TimeSpan Time = ToDate - FromDate;
            double g = Time.TotalDays;

            var Weekends = _db.PublicHolidayCalenders.Where(p => p.ApplicableDate >= FromDate && p.ApplicableDate <= ToDate).ToList();

            Double WeekEnd = 0.00;
            Double sum = 0.00;
            int pubH = 0;

            foreach (var PublicHolidayCalender in Weekends)
            {
                string PH = PublicHolidayCalender.HolidayTypeId.ToString();

                if (PH == "101")
                {
                    WeekEnd++;
                }
                else if (PH == "107")
                {
                    WeekEnd++;
                }
                else if (PH == "100")
                {
                    pubH   ++;
                }
             }
            sum = WeekEnd / 2;                          
            obj_LeaveHeader.TotalDays = Convert.ToInt32(g+1);
            return Json(obj_LeaveHeader,JsonRequestBehavior.AllowGet);
        }
        public JsonResult LeaveDetailsCal(_LeaveHeader obj_LeaveHeader)
        {
                    
            _LeaveDetail objLeaveDetail = new _LeaveDetail();         
            
            DateTime FromDate = Convert.ToDateTime(obj_LeaveHeader.FromDate);
            DateTime ToDate = Convert.ToDateTime(obj_LeaveHeader.ToDate);
            
            var Weekends = _db.PublicHolidayCalenders.Where(p => p.ApplicableDate >= FromDate && p.ApplicableDate <= ToDate).ToList();

            Double WeekEnd = 0.00;
            Double sum = 0.00;
            int pubH = 0;
            foreach (var PublicHolidayCalender in Weekends)
            {
                string PH = PublicHolidayCalender.HolidayTypeId.ToString();

                if (PH == "101")
                {
                    WeekEnd++;
                }
                else if (PH == "107")
                {
                    WeekEnd++;
                }
                else if (PH == "100")
                {
                    pubH++;
                }
            }
            sum = WeekEnd / 2;
            string Substring="";
            string SubstringNum = "";
            string Week = "";

            int count = sum.ToString().Count();

            try 
	           {
                   if (count==3)
                   {
                     Substring = sum.ToString().Substring(1 , 2);
                     SubstringNum = sum.ToString().Substring(0, 1);
                   }
                   else if (count==4)
                   {
                     Substring = sum.ToString().Substring(2 , 2);
                     SubstringNum = sum.ToString().Substring(0, 2);
                   }
                   else if (count == 5)
                   {
                     Substring = sum.ToString().Substring(5 , 2);
                     SubstringNum = sum.ToString().Substring(0, 3);
                   }
                   else if (count == 1)
                   {
                       Substring = sum.ToString().Substring(0, 1);
                       SubstringNum = sum.ToString().Substring(0, 1);
                   }   
               
	           }
	        catch (Exception)
	          {	
		     
	          }
            if (sum==0.5)
            {
                Week = "Half Weekend";
            }
           else if (Substring==".5")
            {
                Week = SubstringNum + " " + "And" +" "+ "Half Day";
            }
            else
            {
                Week = sum.ToString();
            }
            objLeaveDetail.Weekend_Disc = Week.ToString();
            objLeaveDetail.Weekend = Convert.ToDecimal(sum);
            objLeaveDetail.PublicHoliday = pubH;
            objLeaveDetail.PrivilegeLeave = 0;
            objLeaveDetail.LeaveLeave = 0;
            objLeaveDetail.CasualLeave = 0;
            objLeaveDetail.AnnualLeave = 0;
            objLeaveDetail.Re_engagement = 0;

            return Json(objLeaveDetail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details (int id)
        {
            int UID_ = 0;
            string EstablishmentId;
            int? UserRoleId;
            int? CurrentStatusUserRole;

            //For popup box
            TempData["LHID"] = id;

            if (Session["UID"].ToString() != "")
            {
                UID_ = Convert.ToInt32(Session["UID"]);

                EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();

                ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                TempData["UserRoleId"] = UserRoleId;

                var CurrentStatus_UserRole = (from f in _db.LeaveFlowStatus
                                              join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                              where u.EstablishmentId == EstablishmentId & f.LeaveHeaderId == id
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
                    TempData["CurrentStatusUserRole"] = UserRoleId;
                }

                var Leave = _db.Vw_Leave.Where(x => x.LHID == id);
                int? CurrentStatus = Leave.Select(x => x.CurrentStatus).First();
                TempData["CurrentStatus"] = CurrentStatus;
                int? SubmitStatus = Leave.Select(x => x.SubmitStatus).First();
                TempData["SubmitStatus"] = SubmitStatus;

                // Finde Reject User, User Role Name           

                var LeaveFlowStatus = _db.LeaveFlowStatus.Where(x => x.LeaveHeaderId == id & x.RecordStatusId == 3000).FirstOrDefault();

                if (LeaveFlowStatus != null)
                {
                    var FlowStatus = _db.UserInfoes.Where(x => x.UID == LeaveFlowStatus.CreatedBy).First();
                    var UserRole = _db.UserRoles.Where(x => x.RID == FlowStatus.RoleId).First();
                    TempData["RejectUserRole"] = UserRole.RoleName;
                    TempData["RejectDate"] = LeaveFlowStatus.CreatedDate;
                }
                else
                {
                    return View(Leave.FirstOrDefault());
                }
                return View(Leave.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

        }
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
                string RecordEstablishmentId = _db.LeaveHeaders.Where(x => x.LHID == id).Select(x => x.EstablishmentId).FirstOrDefault();
                string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

                int? SubmitStatus = NextFlowStatusId(id, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);

                //Get Next FlowStatus User Role Name for Add Successfull Msg
                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                //FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();
                LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();
                objLeaveFlowStatu.LeaveHeaderId = id;
                objLeaveFlowStatu.FMSID = SubmitStatus;
                objLeaveFlowStatu.CreatedBy = UID;
                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 2000 = Forward//
                objLeaveFlowStatu.RecordStatusId = 2000;
                objLeaveFlowStatu.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objLeaveFlowStatu.CreatedMac = MacAddress;
                objLeaveFlowStatu.Active = 1;
                _db.LeaveFlowStatus.Add(objLeaveFlowStatu);

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
            string UserDivisionId = null;

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
                    RecordEstablishmentId = _db.LeaveHeaders.Where(x => x.LHID == IDs).Select(x => x.EstablishmentId).FirstOrDefault();
                    UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();

                    int? SubmitStatus = NextFlowStatusId(IDs, UserEstablishmentId, RecordEstablishmentId, UserDivisionId);
                    //Get Next FlowStatus User Role Name for Add Successfull Msg
                    UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == SubmitStatus).Select(x => x.UserRoleID).FirstOrDefault();
                    SubmitStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();

                    //FixAllowanceDetailsFlowStatu objFixAllowanceDetailsFlowStatu = new FixAllowanceDetailsFlowStatu();

                    LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();
                    objLeaveFlowStatu.LeaveHeaderId = IDs;
                    objLeaveFlowStatu.FMSID = SubmitStatus;
                    objLeaveFlowStatu.CreatedBy = UID;
                    //Record Status Releted to RecordStatus Table
                    //Every Record has a Status Ex: Insert/Forward/Delete... 2000 = Forward//
                    objLeaveFlowStatu.RecordStatusId = 2000;
                    objLeaveFlowStatu.CreatedDate = DateTime.Now;
                    string MacAddress = new DALBase().GetMacAddress();
                    objLeaveFlowStatu.CreatedMac = MacAddress;
                    objLeaveFlowStatu.Active = 1;
                    _db.LeaveFlowStatus.Add(objLeaveFlowStatu);
                }
                else
                {
                    return RedirectToAction("Login", "User");
                }
            }
            if (_db.SaveChanges() > 0)
            {
                if (UserRoleId > 14)
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
        public int? NextFlowStatusId(int? LHID, string UserEstablishmentId, string RecordEstablishmentId, string UserDivisionId)
        {
            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxLFSID = _db.LeaveFlowStatus.Where(x => x.LeaveHeaderId == LHID).Select(x => x.LFSID).Max();
                int? CurrentFMSID = _db.LeaveFlowStatus.Where(x => x.LFSID == MaxLFSID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();

                int? UID = Convert.ToInt32(Session["UID"]);

                //LHID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();

                }
                else if (CurrentUserRole == 13)
                {
                    //UserRoleId = 5 to P&R first User 'ko_pnr'
                    //currentUser-Station/Base = OCPS
                    //RecordUser - Station/Base no P&R there for get UserRole ID from First User Role In P&R
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 5).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole >= 5)
                {
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();
                }
                else
                {
                    int? SubmitStatus = _db.FlowManagementStatus.Where(x => x.UserRoleID == CurrentUserRole && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.SubmitStatus).FirstOrDefault();
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == SubmitStatus && (x.EstablishmentId == RecordEstablishmentId && x.DivisionId == UserDivisionId || x.EstablishmentId == UserEstablishmentId && x.DivisionId == UserDivisionId)).Select(x => x.FMSID).First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return FMSID;
        }
        [HttpPost]
        public ActionResult Reject(int LHID, int FMSID)
        {
            Vw_Leave obj_Vw_Leave = new Vw_Leave();
            try
            {
                obj_Vw_Leave.LHID = LHID;
                obj_Vw_Leave.FMSID = FMSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("_RejectCommentLeave", obj_Vw_Leave);
        }
        [HttpPost]
        public ActionResult Index_Reject(Vw_Leave objVw_Leave)
        {
            //Get data from Model Pop partialView _RejectComment
            int? UID = Convert.ToInt32(Session["UID"]);
            string PreviousFlowStatus_UserRole;
            if (UID != 0)
            {
                //int? id = objVw_FixedAllowanceDetail.FADID;
                string UserEstablishmentId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
                string RecordEstablishmentId = _db.LeaveHeaders.Where(x => x.LHID == objVw_Leave.LHID).Select(x => x.EstablishmentId).FirstOrDefault();

                //Method use for get FMSID
                int? PreviousFMSID = PreviousFlowStatusId(objVw_Leave.LHID, UserEstablishmentId, RecordEstablishmentId);
                //Get Next FlowStatus User Role Name for Add Successfull Msg

                int? UserRoleId = _db.FlowManagementStatus.Where(x => x.FMSID == PreviousFMSID && (x.EstablishmentId == RecordEstablishmentId || x.EstablishmentId == UserEstablishmentId)).Select(x => x.UserRoleID).FirstOrDefault();

                PreviousFlowStatus_UserRole = _db.UserRoles.Where(x => x.RID == UserRoleId).Select(x => x.RoleName).FirstOrDefault();
                LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();
                objLeaveFlowStatu.LeaveHeaderId = objVw_Leave.LHID;
                objLeaveFlowStatu.FMSID = PreviousFMSID;
                objLeaveFlowStatu.CreatedBy = UID;
                //Record Status Releted to RecordStatus Table
                //Every Record has a Status Ex: Insert/Forward/Delete... 3000 = Reject//
                objLeaveFlowStatu.RecordStatusId = 3000;
                objLeaveFlowStatu.Comment = objVw_Leave.Comment;
                objLeaveFlowStatu.CreatedDate = DateTime.Now;
                string MacAddress = new DALBase().GetMacAddress();
                objLeaveFlowStatu.CreatedMac = MacAddress;
                objLeaveFlowStatu.Active = 1;
                _db.LeaveFlowStatus.Add(objLeaveFlowStatu);

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

                    return RedirectToAction("RejectIndex");
                }
                else
                {
                    TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                }
               
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }
        public int? PreviousFlowStatusId(int? LHID, string UserEstablishmentId, string RecordEstablishmentId)
        {
            int? FMSID = 0;
            try
            {
                //Current Record FMSID
                int? MaxFADFID = _db.LeaveFlowStatus.Where(x => x.LeaveHeaderId == LHID).Select(x => x.LFSID).Max();
                int? CurrentFMSID = _db.LeaveFlowStatus.Where(x => x.LFSID == MaxFADFID).Select(x => x.FMSID).FirstOrDefault();
                int? CurrentUserRole = _db.FlowManagementStatus.Where(x => x.FMSID == CurrentFMSID).Select(x => x.UserRoleID).FirstOrDefault();

                int? UID = Convert.ToInt32(Session["UID"]);

                //FADID=Null (actclk create record)
                if (CurrentUserRole == null)
                {
                    //Get First FMSID if Current FMSID is null
                    FMSID = _db.FlowManagementStatus.Where(x => x.EstablishmentId == UserEstablishmentId && x.UserRoleID==11).Select(x => x.FMSID).First();
                }
                else if (CurrentUserRole == 5)
                {
                    //First P&R Flow Status   
                    //UserRoleId = 4 to Camp Level Last User 'OCPS'
                    FMSID = _db.FlowManagementStatus.Where(x => x.UserRoleID == 13 && x.EstablishmentId == RecordEstablishmentId).Select(x => x.FMSID).First();
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
            LeaveHeader objLeave = _db.LeaveHeaders.Find(id);           

            #region Leave count check
            var Sno = objLeave.Sno;
            int PLeaveTotal = 0;
            int ALeaveTotal = 0;
            int DelLeave =Convert.ToInt32(objLeave.PrivilegeLeave + objLeave.AnnualLeave);
            int Min = 0;

             DataSet ds = objDALCommanQuery.RejectLeave(Sno);             
             DataTable Dt = ds.Tables[0];
             foreach (DataRow item in Dt.Rows)
             {
                 int LastLeaveAccu = Convert.ToInt32(item["AnnualLeave"].ToString());
                 int LastLeavePriv = Convert.ToInt32(item["PrivilegeLeave"].ToString());
                 int LD_Accu = Convert.ToInt32(item["AL"].ToString());
                 int LD_Privi = Convert.ToInt32(item["PL"].ToString());

                 if (LastLeavePriv>0)
                 {
                     Min = Convert.ToInt32(LastLeavePriv - (LD_Accu + LD_Privi));
                    if (Min<0)
                    {
                        Min = 0;
                        Min = Convert.ToInt32( (LD_Accu + LD_Privi)-LastLeavePriv);
                        PLeaveTotal=0;
                        ALeaveTotal = Min; 
                    }
                     else
	                {
                       PLeaveTotal = Min;
	                }
                 }
                 else
                 {
                     ALeaveTotal = Convert.ToInt32(LastLeaveAccu + LD_Accu);
                 }
             }

            #endregion


            objLeave.Active = 0;
            objLeave.AnnualLeave = ALeaveTotal;
            objLeave.PrivilegeLeave = PLeaveTotal;
               
            _db.Entry(objLeave).Property(x => x.Active).IsModified = true;
            if (_db.SaveChanges() > 0)
            {
                try
                {
                    LeaveDetail objLeaveDetails = _db.LeaveDetails.Where(z => z.LeaveHeaderId == id).First();
                    objLeaveDetails.Active = 0;
                    _db.Entry(objLeaveDetails).Property(x => x.Active).IsModified = true;
                    _db.SaveChanges();                   
                }
                catch 
                {                    
                  
                }
                TempData["ScfMsg"] = "Successfully Reject Confirmed.";
            }
            return RedirectToAction("RejectIndex");                       
        }        
        [HttpGet]
        public ActionResult Leave_history()
         {
           //  ViewBag.DDL_LeaveCategory = new SelectList(_db.LeaveCategories.Where(x => x.LCID == 1 || x.LCID == 4 || x.LCID == 5), "LCID", "LeaveCategoryName");
             //when form load Leave category selected as Genaral Leave , there for Payment type must be Full
             //Full Pay payment category =2
             ViewBag.DDL_HistoryYear = new SelectList(_db.PorYears.Where(x => x.Status != 0 && x.YD == 5 || x.YD == 1).OrderBy(x => x.PorYear1), "PorYear1", "PorYear1");
             ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 2), "ServiceCategoryId", "ServiceCategoryName");
             ViewBag.DDL_LeaveCategory = new SelectList(_db.LeaveCategories.Where(x => x.LCID == 1 || x.LCID == 4 || x.LCID == 5), "LCID", "LeaveCategoryName");
             //when form load Leave category selected as Genaral Leave , there for Payment type must be Full
             //Full Pay payment category =2
             ViewBag.DDL_PaymentType = new SelectList(_db.PaymentTypes.Where(x => x.PTID == 2), "PTID", "PaymentTypeName");

             Session["Sno"] = "";
            
             Session["ServiceNo"] = "";

             if (Session["UID"].ToString() != "")
             {
                 int UID_ = Convert.ToInt32(Session["UID"]);
                 int RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();

                 if (RoleId == 11)
                 {
                     ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 2), "ServiceCategoryId", "ServiceCategoryName");
                 }
                 else if (RoleId == 15)
                 {
                     ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 1), "ServiceCategoryId", "ServiceCategoryName");
                 }
             }

             return View();
         }
        [HttpPost]
        public ActionResult Leave_history(POR.Models.LeaveModel.Leave objLeave)
         {
             LeaveHeader objLeaveHeader = new LeaveHeader();
             ViewBag.DDL_HistoryYear = new SelectList(_db.PorYears.Where(x => x.Status != 0 && x.YD == 5 || x.YD == 1).OrderBy(x => x.PorYear1), "PorYear1", "PorYear1");
             ViewBag.DDL_ServiceCategory = new SelectList(_db.ServiceCategories.Where(x => x.ServiceCategoryId == 2), "ServiceCategoryId", "ServiceCategoryName");
            //ViewBag.DDL_LeaveCategory = new SelectList(_db.LeaveCategories.Where(x => x.LCID == 1 || x.LCID == 4 || x.LCID == 5), "LCID", "LeaveCategoryName");
             // ViewBag.DDL_PaymentType = new SelectList(_db.PaymentTypes.Where(x => x.PTID == 2), "PTID", "PaymentTypeName");
                               
            
             string EstablishmentId = "";
             string DivisionId = "";
             int UID_ = 0;
             //int count = 0;
             int RoleId = 0;
             int Status = 0;
             int HistryCheck2019 = 0;
             int HistryCheck2018 = 0;
             int HistryCheck = 0;
            
            

             if (Session["UID"].ToString() != "")
             {
                 UID_ = Convert.ToInt32(Session["UID"]);
                 EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.LocationId).FirstOrDefault();
                 DivisionId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.DivisionId).FirstOrDefault();
                 RoleId = _db.UserInfoes.Where(x => x.UID == UID_ && x.Active == 1).Select(x => x.RoleId).FirstOrDefault();
                                
              }
             var Sno = _db.Vw_PersonalDetail.Where(x => x.ServiceNo == objLeave.LeaveHeader_.ServiceNo_).Select(x => x.SNo).FirstOrDefault();
             HistryCheck = objDALCommanQuery.HistryCheck(Sno);

             try
             {
                 HistryCheck = objDALCommanQuery.HistryCheck(Sno);
                 if (HistryCheck>0)
                 {
                     if (objLeave.LeaveHeader_.HistoryYear.ToString() == "2018")
                     {
                         Status = 1;
                     }
                    
                 }
                 else if (objLeave.LeaveHeader_.HistoryYear.ToString() == "2019")
                 {
                     HistryCheck2018 = 1;
                 }
                  
                 HistryCheck2019 = objDALCommanQuery.HistryCheck2019(Sno);
                 if (HistryCheck2019 == 0 && Status == 0 && HistryCheck2018==0) //2018 Leave Check
                 {
                     objLeave.LeaveHeader_.EstablishmentId = EstablishmentId;
                     ModelState.Remove("LeaveHeader_.EstablishmentId");
                     if (objLeave.LeaveHeader_.LeaveCategoryId != 1)
                     {
                         ModelState.Remove("LeaveDetail_.PrivilegeLeave");
                         ModelState.Remove("LeaveDetail_.CasualLeave");
                         ModelState.Remove("LeaveDetail_.AnnualLeave");
                         ModelState.Remove("LeaveDetail_.LeaveLeave");
                         ModelState.Remove("LeaveDetail_.Weekend");
                         ModelState.Remove("LeaveDetail_.PublicHoliday");
                     }

                     int AnnualLeaveC = 0;
                     int fromyear = Convert.ToInt32(objLeave.LeaveHeader_.HistoryYear.ToString());
                     int f = 0;

                     DataSet AnnualLeaveCount = objDALCommanQuery.AnnualLeaveCount(Sno, 2018, 2019, fromyear);
                     DataTable AnnualLeaveCounts = AnnualLeaveCount.Tables[0];
                     foreach (DataRow item1 in AnnualLeaveCounts.Rows)
                     {
                         f = Convert.ToInt32(item1["AnnualLeave"].ToString());
                     }

                     DataSet ds = objDALCommanQuery.AnnualLeavePlane(fromyear, fromyear, fromyear);
                     DataTable Dt = ds.Tables[0];
                     foreach (DataRow item in Dt.Rows)
                     {
                         AnnualLeaveC = Convert.ToInt32(item["TotalLeaveDays"].ToString());
                     }

                     objLeaveHeader.Sno = Sno;
                     objLeaveHeader.EstablishmentId = objLeave.LeaveHeader_.EstablishmentId;
                     objLeaveHeader.DeivisionId = DivisionId;
                     objLeaveHeader.LeaveCategoryId = 1;
                     objLeaveHeader.ServiceCategoryId = 2;

                     string Year = "10-01";
                     string FullYesr = Year + "-" + objLeave.LeaveHeader_.HistoryYear.ToString();
                     DateTime Date = Convert.ToDateTime(FullYesr);

                     objLeaveHeader.AnnualLeave = Convert.ToInt32(objLeave.LeaveHeader_.AccumulatedLeave) + f;
                     objLeaveHeader.FromDate = Date;
                     objLeaveHeader.ToDate = Date;
                     objLeaveHeader.Authority = "CO";
                     //Record Status when data insert
                     objLeaveHeader.PorStatus = 1000;
                     objLeaveHeader.IsProcessed = 1;
                     objLeaveHeader.PaymentTypeId = 2;
                     objLeaveHeader.TotalDays = 10;
                     objLeaveHeader.CreatedBy = UID_;
                     objLeaveHeader.CreatedDate = DateTime.Now;
                     objLeaveHeader.Active = 1;
                     //int d = Convert.ToInt32(objLeave.LeaveHeader_.FromDate - objLeave.LeaveHeader_.ToDate); 

                     _db.LeaveHeaders.Add(objLeaveHeader);
                     _db.SaveChanges();

                     int Id = 0;
                     Id = new DALCommanQuery().NextLeaveHeaderNextId();

                     LeaveDetail objLeaveDetail = new LeaveDetail();
                     objLeaveDetail.LeaveHeaderId = Id - 1;
                     objLeaveDetail.PrivilegeLeave = 0;
                     objLeaveDetail.CasualLeave = 0;
                     objLeaveDetail.AnnualLeave = AnnualLeaveC - Convert.ToInt32(objLeave.LeaveHeader_.AccumulatedLeave);
                     objLeaveDetail.LeaveLeave = 0;
                     objLeaveDetail.Weekend = 0;
                     objLeaveDetail.PublicHoliday = 0;
                     objLeaveDetail.ReEngagementLeave = Convert.ToInt32(objLeave.LeaveHeader_.Reengagement);
                     objLeaveDetail.CreatedBy = UID_;
                     objLeaveDetail.CreatedDate = DateTime.Now;
                     objLeaveDetail.Active = 1;
                     _db.LeaveDetails.Add(objLeaveDetail);
                     _db.SaveChanges();

                //Flow Stattus Save

                     LeaveFlowStatu objLeaveFlowStatu = new LeaveFlowStatu();
                     objLeaveFlowStatu.LeaveHeaderId = Id - 1;
                     objLeaveFlowStatu.FMSID = 144;
                     objLeaveFlowStatu.RecordStatusId = 2000;
                     objLeaveFlowStatu.CreatedBy = UID_;
                     objLeaveFlowStatu.CreatedDate = DateTime.Now;
                     //objLeaveFlowStatu.CreatedMac = ;
                     objLeaveFlowStatu.Active = 1;
                     _db.LeaveFlowStatus.Add(objLeaveFlowStatu);                  

                     if (_db.SaveChanges() > 0)
                     {
                         TempData["ScfMsg"] = "Data Successfully Saved";
                         return RedirectToAction("Leave_history");
                     }
                     else
                     {
                         TempData["ErrMsg"] = "Process Unsuccessful.Try again...";
                     }              
                 }
                 else if (HistryCheck2018==1)
                 {
                     TempData["ErrMsg"] = "This person don't have 2018 Leave...";
                 }
                 else if (HistryCheck2019>0 || Status==1)
                 {
                     TempData["ErrMsg"] = "This persons leaves are already exceed ...";  
                 }
                 else
                 {
                     TempData["ErrMsg"] = "This person don't have 2018 Leave...";
                 }
                }        
             catch (Exception ex)
                 {
                        throw ex;
                 }
             return View();
         }
        [HttpGet]
        public ActionResult Individualsearch(string sortOrder, string currentFilter, string searchString, int? page)
        {
            int? UID = Convert.ToInt32(Session["UID"]);
            int pageSize = 0;
            int pageNumber = 1;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name" : "";
            ViewBag.DateSortParm = sortOrder == "LeaveCategoryName" ? "Rank" : "FromDate";

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

            // List<Vw_Leave> objleave = new List<Vw_Leave>();
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();


            string UserDivisionId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.DivisionId).FirstOrDefault();
            //Create Evenet FormationId and DivisionId           

            var objleave = _db.Vw_Leave.Where(x => x.Active != 0).ToList().OrderBy(x=>x.LHID);

        
            if (!String.IsNullOrEmpty(searchString))
            {
                objleave = objleave.Where(s => s.ServiceNo.Contains(searchString) || s.Rank.Contains(searchString)).ToList().OrderBy(x => x.LHID);
            }

            switch (sortOrder)
            {
                case "Service No":
                    objleave = objleave.OrderBy(s => s.ServiceNo).ToList().OrderBy(x => x.LHID);
                    break;               
                case "Rank":
                    objleave = objleave.OrderBy(s => s.Rank).ToList().OrderBy(x => x.LHID);
                    break;
                case "Name With Initials":
                    objleave = objleave.OrderBy(s => s.Name).ToList().OrderBy(x => x.LHID);
                    break;             
                case "From Date":
                    objleave = objleave.OrderBy(s => s.FromDate).ToList().OrderBy(x => x.LHID);
                    break;
                case "To Date":
                    objleave = objleave.OrderBy(s => s.ToDate).ToList().OrderBy(x => x.LHID);
                    break;
                case "Authority":
                    objleave = objleave.OrderBy(s => s.Authority).ToList().OrderBy(x => x.LHID);
                    break;
                case "Status":
                    objleave = objleave.OrderBy(s => s.RoleName).ToList().OrderBy(x => x.LHID);
                    break;
            }

            pageSize = 10;
            pageNumber = (page ?? 1);
            return View(objleave.ToPagedList(pageNumber, pageSize));

            // return View(_db.Vw_Leave.ToList());
        }
        [HttpGet]
        public ActionResult IndividualDetails(int id)
         {
             int UID_ = 0;
             string EstablishmentId;
             int? UserRoleId;
             int? CurrentStatusUserRole;

             //For popup box
             TempData["LHID"] = id;

             if (Session["UID"].ToString() != "")
             {
                 UID_ = Convert.ToInt32(Session["UID"]);

                 EstablishmentId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.LocationId).FirstOrDefault();

                 ViewData["AllFlowStatus"] = _db.Vw_FlowStatus.Where(x => x.EstablishmentId == EstablishmentId).ToList();

                 ViewData["UserFlow_ToolTip"] = _db.Vw_FlowStatusUser_ToolTip.Where(x => x.FADID == id).ToList();

                 UserRoleId = _db.UserInfoes.Where(x => x.UID == UID_).Select(x => x.RoleId).First();
                 TempData["UserRoleId"] = UserRoleId;

                 var CurrentStatus_UserRole = (from f in _db.LeaveFlowStatus
                                               join u in _db.Vw_FlowStatus on f.FMSID equals u.FMSID
                                               where u.EstablishmentId == EstablishmentId & f.LeaveHeaderId == id
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
                     TempData["CurrentStatusUserRole"] = UserRoleId;
                 }

                 var Leave = _db.Vw_Leave.Where(x => x.LHID == id);
                 int? CurrentStatus = Leave.Select(x => x.CurrentStatus).First();
                 TempData["CurrentStatus"] = CurrentStatus;
                 int? SubmitStatus = Leave.Select(x => x.SubmitStatus).First();
                 TempData["SubmitStatus"] = SubmitStatus;

                 // Finde Reject User, User Role Name           

                 var LeaveFlowStatus = _db.LeaveFlowStatus.Where(x => x.LeaveHeaderId == id & x.RecordStatusId == 3000).FirstOrDefault();

                 if (LeaveFlowStatus != null)
                 {
                     var FlowStatus = _db.UserInfoes.Where(x => x.UID == LeaveFlowStatus.CreatedBy).First();
                     var UserRole = _db.UserRoles.Where(x => x.RID == FlowStatus.RoleId).First();
                     TempData["RejectUserRole"] = UserRole.RoleName;
                     TempData["RejectDate"] = LeaveFlowStatus.CreatedDate;
                 }
                 else
                 {
                     return View(Leave.FirstOrDefault());
                 }
                 return View(Leave.FirstOrDefault());
             }
             else
             {
                 return RedirectToAction("Login", "User");
             }

         }        
    }
}