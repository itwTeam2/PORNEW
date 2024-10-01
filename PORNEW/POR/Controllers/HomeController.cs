using POR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class HomeController : Controller
    {
        dbContext _db = new dbContext();
        DataTable dt,dt2 = new DataTable();
        ReportData.DAL.DALCommanQuery objDALCommanQuery = new ReportData.DAL.DALCommanQuery();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult P3ClkHome()
        {
            ///Created BY   : Flt Lt Wickramasinghe
            ///Created Date : 20/05/2022
            ///Description  : Load P3 view Home Page
            int? UID = Convert.ToInt32(Session["UID"]);
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            
            try
            {

                //show the reject count to P3 clk
                //var rejectCivilStatusCount = _db.Vw_CivliStatusReject.Where(x => x.RejectStatus == (int)POR.Enum.RecordStatus.Reject && x.Location == LocationId && x.Active == 1).Count();                                
                //TempData["RejectCivilStatusCount"] = rejectCivilStatusCount;

                #region Civil Status
                
                //show the reject count to P3 clk 
                var rejectCivilStatusCount = _db.Vw_CivliStatusReject.Where(x => x.Location == LocationId && x.CurrentStatus == (int)Enum.UserRole.P3CLERK && x.Active == 1 &&(x.ServiceTypeId == (int)POR.Enum.ServiceType.RegAirmen ||
                                      x.ServiceTypeId == (int)POR.Enum.ServiceType.RegAirWomen || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolAirmen || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolAirWomen)).Count();

                TempData["RejectCivilStatusCount"] = rejectCivilStatusCount;

                #endregion

                #region Living IN/OUt

                //show the reject count to P3 clk           
                var rejectRecorfCount = _db.Vw_LivingStatusReject.Where(x => x.Location == LocationId && x.CurrentStatus == (int)Enum.UserRole.P3CLERK && x.LIActive == 1 && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegAirmen ||
                                      x.ServiceTypeId == (int)POR.Enum.ServiceType.RegAirWomen || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolAirmen || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolAirWomen)).OrderByDescending(x => x.LIOHID).Count();
                              
                TempData["rejectRecorfCountLiving"] = rejectRecorfCount;
                
                #endregion




            }
            catch (Exception ex)
            {

                throw ex;
            } 
          

            return View();
        }

        public ActionResult P2ClkHome()
        {
            ///Created BY   : Flt Lt Wickramasinghe
            ///Created Date : 13/02/2023
            ///Description  : Load P2 view Home Page
            int? UID = Convert.ToInt32(Session["UID"]);
            var LocationId = _db.UserInfoes.Where(x => x.UID == UID).Select(x => x.LocationId).FirstOrDefault();
            

            try
            {
                #region Living IN/Out

                #region Pending Record Count
               
                dt = objDALCommanQuery.CallLivingINOutSP(0);
                var pendingRecorfCount = dt.AsEnumerable().Where(x => x.Field<int>("LIOActive") == 1 && x.Field<string>("Location") == LocationId && x.Field<int>("RID") == (int)Enum.UserRole.P2CLERK && x.Field<int>("RSID") == (int)POR.Enum.RecordStatus.Insert && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();


                //show the reject count to P3 clk
                TempData["pendingRecorfCount"] = pendingRecorfCount;

                #endregion

                #region Reject Count

                var rejectRecorfCount = _db.Vw_LivingStatusReject.Where(x => x.Location == LocationId && x.CurrentStatus == (int)Enum.UserRole.P2CLERK && x.LIActive == 1 && (x.ServiceTypeId == (int)POR.Enum.ServiceType.RegOfficer ||
                                       x.ServiceTypeId == (int)POR.Enum.ServiceType.RegLadyOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolOfficer || x.ServiceTypeId == (int)POR.Enum.ServiceType.VolLadyOfficer)).OrderByDescending(x => x.LIOHID).Count();

                //show the reject count to P3 clk
                TempData["rejectRecorfCount"] = rejectRecorfCount;

                #endregion

                #endregion

                #region GSQ Allocation/Vacation

                #region Pending Record Count
                
                dt = objDALCommanQuery.CallGSQSP(0);

                var pendingForwardCount = dt.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Insert && x.Field<string>("Location") == LocationId && x.Field<int>("RoleID") == (int)Enum.UserRole.P2CLERK && (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer ||
                              x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();              

                
                TempData["pendingForwardCount"] = pendingForwardCount;
                #endregion

                #region Reject Count

                dt2 = objDALCommanQuery.CallGSQRejectSP();

                var gsqRejectCount = dt2.AsEnumerable().Where(x => x.Field<int>("Active") == 1 && x.Field<int>("RecordStatusID") == (int)POR.Enum.RecordStatus.Reject && x.Field<string>("Location") == LocationId &&
                          (x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegOfficer || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.RegLadyOfficer
                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolOfficer
                          || x.Field<int>("ServiceTypeId") == (int)POR.Enum.ServiceType.VolLadyOfficer)).Count();

                TempData["gsqRejectCount"] = gsqRejectCount;

                #endregion

                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}