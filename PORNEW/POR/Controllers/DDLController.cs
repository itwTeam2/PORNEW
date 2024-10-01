using POR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace POR.Controllers
{
    public class DDLController : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbCommonData = new dbContextCommonData();  
       
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult FromEstablishment(string id)
        {
            List<Establishment> FromEstablishment = new List<Establishment>();
            var FromEstablishmentList = this.LinqFromEstablishment(id);


            var EstablishmentListData = FromEstablishmentList.Select(x => new SelectListItem()
            {
                Text = x.DivisionName.ToString(),
                Value = x.DivisionID.ToString(),
            });


            return Json(EstablishmentListData, JsonRequestBehavior.AllowGet);
        }
        public IList<Division> LinqFromEstablishment(string id)
        {
            List<Division> Result = new List<Division>();
            Result = _dbCommonData.Divisions.Where(x => x.LocationID == id).ToList();
            
            return Result;
        }

        //__To Formation_____________________________________________________________________//
        public JsonResult ToEstablishment(string id)
        {
            List<Establishment> ToEstablishment = new List<Establishment>();
            var ToEstablishmentList = this.LinqToEstablishment(id);
            var ToEstablishmentListData = ToEstablishmentList.Select(x => new SelectListItem()
            {
                Text = x.DivisionName.ToString(),
                Value = x.DivisionID.ToString(),
            });
            return Json(ToEstablishmentListData, JsonRequestBehavior.AllowGet);
        }
        public IList<Division> LinqToEstablishment(string id)
        {
            List<Division> Result = new List<Division>();
            Result = _dbCommonData.Divisions.Where(x => x.LocationID == id).ToList();
            return Result;
        }

        //___AllowanceCategory DDL load using Json_____________________________________________// 
        public JsonResult AllowanceCategory(int id)
        {
            List<AllowanceCategory> PorCategorySub = new List<AllowanceCategory>();


            var AllowanceCategoryList = this.LinqAllowanceCategory(id);
            var AllowanceCategoryListData = AllowanceCategoryList.Select(x => new SelectListItem()
            {
                Text = x.Description.ToString(),
                Value = x.ACID.ToString(),
            });
            return Json(AllowanceCategoryListData, JsonRequestBehavior.AllowGet);
        }
        public IList<AllowanceCategory> LinqAllowanceCategory(int id)
        {
            List<AllowanceCategory> Result = new List<AllowanceCategory>();
            Result = _db.AllowanceCategories.Where(x => x.AllowanceId == id).ToList();
            return Result;
        }

        //___Allowance Payment Type DDL Load Using AllowanceCategories_________________________//

        public JsonResult AllowancePaymentType(int id)
        {
            List<AllowancePaymentType> PorCategorySub = new List<AllowancePaymentType>();

            var AllowancePaymentTypeList = this.LinqAllowancePaymentType(id);

            var AllowancePaymentTypeListData = AllowancePaymentTypeList.Select(x => new SelectListItem()
            {
                Text = x.PaymentType.ToString(),
                Value = x.APID.ToString(),
            });
            return Json(AllowancePaymentTypeListData, JsonRequestBehavior.AllowGet);
        }
        public IList<AllowancePaymentType> LinqAllowancePaymentType(int id)
        {
            List<AllowancePaymentType> Result = new List<AllowancePaymentType>();
            Result = _db.AllowancePaymentTypes.Where(x => x.AllowanceId == id).ToList();
            return Result;
        }

        public JsonResult AllowanceType(int id)
        {
            ///Create By:   Cpl madudsanka
            ///Create Date: 2/09/2022
            ///Description: Select to AllowanceType and load AllowanceCategory..

            List<AllowanceType> PorCategorySub = new List<AllowanceType>();
            var AllowanceTypeList = this.LinqAllowanceType(id);
            var AllowanceTypeListData = AllowanceTypeList.Select(x => new SelectListItem()
            {
                Text = x.AllowanceName.ToString(),
                Value = x.ALWID.ToString(),
            });
            return Json(AllowanceTypeListData, JsonRequestBehavior.AllowGet);
        }
        public IList<Allowance> LinqAllowanceType(int AllowanceTypeId)
        {
            ///Create By:   Cpl madudsanka
            ///Create Date: 2/09/2022
            ///Description: Select to AllowanceType Quary

            List<Allowance> Result = new List<Allowance>();
            Result = _db.Allowances.Where(x => x.AllowanceTypeId == AllowanceTypeId).ToList();
            return Result;
        }


        [HttpPost]
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

        public JsonResult GetPaymentType(int LeaveCategoryId)
        {
            List<PaymentType> PaymentType = new List<PaymentType>();

            var PaymentTypeList = this.LinqGetPaymentType(LeaveCategoryId);

            var PaymentTypeListData = PaymentTypeList.Select(x => new SelectListItem()
            {
                Text = x.PaymentTypeName.ToString(),
                Value = x.PTID.ToString(),
            });
            return Json(PaymentTypeListData, JsonRequestBehavior.AllowGet);
        }
        public IList<PaymentType> LinqGetPaymentType(int LeaveCategoryId)
        {
            List<PaymentType> Result = new List<PaymentType>();

            if (LeaveCategoryId == 1 || LeaveCategoryId == 4 || LeaveCategoryId == 16)
            {
                Result = _db.PaymentTypes.Where(x => x.PTID == 2).ToList();
            }
            else
            {
                Result = _db.PaymentTypes.ToList();
            }
           
            return Result;
        }

     }
}