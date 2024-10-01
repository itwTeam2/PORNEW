using POR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace POR.Controllers
{
    public class UserController : Controller
    {
        dbContext _db = new dbContext();
        dbContextCommonData _dbContextCommonData = new dbContextCommonData();
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public ActionResult UserRegistration()
        {
            ViewBag.DDL_SecQuestion = new SelectList(_db.SecurityQuestions, "SQID", "Question");
            ViewBag.DDL_UserRole = new SelectList(_db.UserRoles, "RID", "RoleName");
            ViewBag.DDL_Establishment = new SelectList(_dbContextCommonData.Establishments, "LocationID", "LocShortName");
            return View();
        }

      
        [HttpPost]
        public ActionResult UserRegistration(_UserInfo obj_UserInfo)
        {
            try
            {
                //if (Session["UIID"]!=null)
                //{
		            UserInfo objUserInfo = new UserInfo();
                    ViewBag.DDL_SecQuestion = new SelectList(_db.SecurityQuestions, "SQID", "Question");
                    ViewBag.DDL_UserRole = new SelectList(_db.UserRoles, "RID", "RoleName");
                    ViewBag.DDL_Establishment = new SelectList(_dbContextCommonData.Establishments, "LocationID", "LocShortName");

                    var ServicePersonCount = _db.Vw_P3PersonalDetail.Where(x => x.ServiceNo == obj_UserInfo.ServiceNo).Count();
                    if (ServicePersonCount != 0)
                    {
                        int UserLoginCount = _db.UserInfoes.Where(s => s.UserName == obj_UserInfo.UserName).Count();
                        if (UserLoginCount <= 0)
                        {
                            if (ModelState.IsValid)
                            {                            
                                objUserInfo.UserName = obj_UserInfo.UserName;
                                objUserInfo.ServiceNo = obj_UserInfo.ServiceNo;
                                objUserInfo.RoleId = obj_UserInfo.RoleId;
                                objUserInfo.UserName = obj_UserInfo.UserName;
                                objUserInfo.Password = obj_UserInfo.Password.GetHashCode().ToString();
                                objUserInfo.SecurityQuestionAnswer = obj_UserInfo.SecurityQuestionAnswer;
                                objUserInfo.SecurityQuestionId = obj_UserInfo.SecurityQuestionId;
                                objUserInfo.LocationId = obj_UserInfo.LocationId;
                                objUserInfo.Active = 1;
                                objUserInfo.CreatedDate = DateTime.Now;
                                objUserInfo.CreatedBy = Convert.ToInt32(Session["UIID"]);                                
                                
                                _db.UserInfoes.Add(objUserInfo);
                                _db.SaveChanges();
                                TempData["notice"] = "Request sent to the system Administrator for approval.";
                                return RedirectToAction("UserRegistration");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "This User alredy exist in the System.";
                    }
                }
                else
                {
                    TempData["Error"] = "Service No not exist in the HRMS, Contact Administrator.";
                }
	            //}
                //else
                //{
                //    TempData["Error"] = "Session Expaired,Try Again.";
                //}

            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes.Try agin, and if the problem persists,Contact System administrator.");
            }
            return View();
        }

        public JsonResult PersonalInfo(string id)
        {
            Vw_P3PersonalDetail objServicePersonP3 = new Vw_P3PersonalDetail();
            objServicePersonP3 = _db.Vw_P3PersonalDetail.Where(x => x.ServiceNo == id).FirstOrDefault();
            return Json(objServicePersonP3, JsonRequestBehavior.AllowGet);
        }

        
        [HttpGet]
        public ActionResult Login()
        {
            Session["UserName"] = null;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]        
        public ActionResult Login(_UserLogin obj_UserLogin)
        {
            try
            {
                SystemLog objSystemLog = new SystemLog();
                //Database DataType as 'Text'

                if (ModelState.IsValid)
                {
                    string InputPassword = obj_UserLogin.Password.GetHashCode().ToString();
                    var dbPassword = _db.UserInfoes.Where(r => r.UserName == obj_UserLogin.UserName && r.Active == 1).Select(q => q.Password).FirstOrDefault();
                    var v = _db.UserInfoes.Where(a => a.UserName.Equals(obj_UserLogin.UserName) && dbPassword == InputPassword).FirstOrDefault();

                    if (v != null)
                    {
                        Session["UID"] = v.UID.ToString();
                        Session["RID"] = v.RoleId.ToString();
                        Session["UserName"] = v.UserName.ToString();

                        objSystemLog.UserId = v.UID;
                        objSystemLog.LoginDate = DateTime.Now;
                        string SHN = Dns.GetHostName();
                        string SIP = Dns.GetHostAddresses(SHN).GetValue(1).ToString();
                        _db.SystemLogs.Add(objSystemLog);
                        if (_db.SaveChanges() > 0)
                        {
                            switch (v.RoleId)
                            {
                                case (int)POR.Enum.UserRole.AccountsClerk:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.SNCO:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.ACCOUNTSOFFICER:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.OCPSOCA:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.KOPNR:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.SNCOSALARY:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.WOSALARY:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.ACCOUNTS01:
                                    return RedirectToAction("Index", "Home");
                                    //break;                              
                                case (int)POR.Enum.UserRole.P3CLERK:
                                    return RedirectToAction("P3ClkHome", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.P3SNCO:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                                case (int)POR.Enum.UserRole.P3OIC:
                                    return RedirectToAction("Index", "Home");
                                //break;
                                case (int)POR.Enum.UserRole.P2CLERK:
                                    return RedirectToAction("P2ClkHome", "Home");
                                //break;
                                case (int)POR.Enum.UserRole.P2SNCO:
                                    return RedirectToAction("Index", "Home");
                                //break;
                                case (int)POR.Enum.UserRole.P2OIC:
                                    return RedirectToAction("Index", "Home");
                                case (int)POR.Enum.UserRole.HRMSCLKP2VOL:
                                    return RedirectToAction("Index", "Home");
                                case (int)POR.Enum.UserRole.HRMSP2SNCO:
                                    return RedirectToAction("Index", "Home");
                                //break; 
                                case (int)POR.Enum.UserRole.ASORSOVRP2VOL:
                                    return RedirectToAction("Index", "Home");
                                //break;                                                            
                                default:
                                    return RedirectToAction("Index", "Home");
                                    //break;
                            }                           
                        }
                        else
                        {
                            TempData["Error"] = "Login Failure,Contact system Administrator.";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "The User Name/Password you've entered is incorrect";
                    }
                }
                return View(obj_UserLogin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        [HttpPost]
        public ActionResult ForgetPassword([Bind(Include = "Password")] _UserInfo obj_UserInfo)
        {
            try
            {
                UserInfo objUserInfo = new UserInfo();
                int count = _db.UserInfoes.Where(x => x.UserName == obj_UserInfo.UserName && x.Active == 1).Select(z => z.UserName).Count();
                if (count == 1)
                {
                    var user = _db.UserInfoes.Where(x => x.UserName == obj_UserInfo.UserName && x.Active == 1).FirstOrDefault();
                    if (user.SecurityQuestionId == obj_UserInfo.SecurityQuestionId)
                    {
                        if (user.SecurityQuestionAnswer == obj_UserInfo.SecurityQuestionAnswer)
                        {
                            if (obj_UserInfo.Password == obj_UserInfo.ConfirmPassword)
                            {
                                objUserInfo.Password = obj_UserInfo.Password.GetHashCode().ToString();
                                _db.Entry(objUserInfo).Property(x => x.Password).IsModified = true;
                                _db.SaveChanges();
                                return RedirectToAction("Login", "User");
                            }
                            else
                            {
                                TempData["PasswordNotMatch"] = "Password and confirm password not match.";
                            }
                        }
                        else
                        {
                            TempData["IncorrectAnswer"] = "Security question answer is incorrect.";
                        }
                    }
                    else
                    {
                        TempData["IncorrectQuestion"] = "Security question is incorrect.";
                    }
                }
                else
                {
                    TempData["IncorrectUser"] = "No User for this User Name.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
       
        [HttpGet]
        public ActionResult ChangePassword()
        {           
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(_ChangePassword obj__ChangePassword)
        {
            //int i = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    obj__ChangePassword.UserName = Convert.ToString(Session["UserName"]);
                    int UID = _db.UserInfoes.Where(x => x.UserName == obj__ChangePassword.UserName && x.Active == 1).Select(z => z.UID).FirstOrDefault();
                    UserInfo objUserInfo = _db.UserInfoes.Find(UID);

                    string InputPassword = obj__ChangePassword.OldPassword.GetHashCode().ToString();
                    var pw = obj__ChangePassword.Password;
                    var confirmPw = obj__ChangePassword.ConfirmPassword;

                    if (UID != 0 && pw == confirmPw)
                    {
                        objUserInfo.Password = obj__ChangePassword.ConfirmPassword.GetHashCode().ToString();
                        //objUserInfo.ModifiedBy = Convert.ToInt32(Session["UIID"]);
                        //objUserInfo.ModifiedDate = DateTime.Now;
                        _db.Entry(objUserInfo).Property(y => y.Password).IsModified = true;
                        _db.SaveChanges();

                        if (true)
                        {
                            TempData["SuccessMeg"] = "Succesfully Change the password.";
                        }
                        else
                        {
                            //TempData["SuccessNotMeg"] = "Password not change.";
                        }
                    }
                    else
                    {
                        TempData["PasswordWrong"] = "User name not exist.";
                    }
                }
            }
            catch (Exception ex)
            {                
                 throw ex;
            }
            return View(obj__ChangePassword);
            
        }

        [HttpGet]
        public ActionResult UserList()
        {
            try
            {
                return View();
                //_db.Vw_user.ToList()
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult ActiveUser() //int? id
        {
            //try
            //{
            //    UserInfo objUserInfo = new UserInfo();
            //    objUserInfo = _db.UserInfoes.Find(id);
            //    objUserInfo.Active = 1;
            //    objUserInfo.ModifiedDate = DateTime.Now;
            //    _db.Entry(objUserInfo).Property(x => x.Active).IsModified = true;
            //    _db.SaveChanges();
            // return RedirectToAction("UserList", "User");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return View();
        }

        [HttpGet]
        public ActionResult DeactivateUser() //int? id
        {
            //try
            //{
            //    UserInfo objUserInfo = new UserInfo();
            //    objUserInfo = _db.UserInfoes.Find(id);
            //    objUserInfo.Active = 0;
            //    objUserInfo.ModifiedDate = DateTime.Now;
            //    _db.Entry(objUserInfo).Property(x => x.Active).IsModified = true;
            //    _db.SaveChanges();
            //return RedirectToAction("UserList", "User");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return View();
        }

        [HttpGet]
        public ActionResult AdminPanel()
        {
            try
            {
                var abc = Convert.ToInt32(Session["UID"]);
                if (Convert.ToInt32(Session["UID"]) == 1)
                {
                    return RedirectToAction("Error", "User");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        public JsonResult UserInfo(string id)
        {
            UserInfo objUserInfo = new UserInfo();
            objUserInfo = _db.UserInfoes.Where(x => x.ServiceNo == id).FirstOrDefault();
            return Json(objUserInfo,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login", "User");
        }
    }
}