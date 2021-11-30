using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ISOStd.Filters;
using ISOStd.Models;
namespace ISOStd.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        //
        // GET: /Account/Login

      
        [AllowAnonymous]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;

            ViewBag.VerNo = objGlobaldata.GetVersionNumber();
           
            return View();
        }

        //
        // POST: /Account/Login


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, FormCollection form)
        {
           
            string sUsername = form["emailAddress"];
            string sPwd = form["Pwd"];
            if (model.LoginAuthenticate(sUsername, sPwd))
            {
                if (model.PasswordExpiryAuthenticate(sUsername, sPwd))
                {
                    System.Data.DataSet dscust = objGlobaldata.Getdetails("select * from t_CompanyInfo ");
                    if (dscust.Tables.Count > 0 && dscust.Tables[0].Rows.Count > 0 && dscust.Tables[0].Rows[0]["address"].ToString() != "")
                    {

                        return RedirectToAction("QHSE", "Home");
                       
                        //return RedirectToRoute("LocationDefault", new { controller="Home", action = "QHSE"});
                    }
                    else if (dscust.Tables.Count > 0 && dscust.Tables[0].Rows.Count > 0 && dscust.Tables[0].Rows[0]["address"].ToString() == "")
                    {

                        return RedirectToAction("CompanyEdit", "Company", new { CompanyID = dscust.Tables[0].Rows[0]["CompanyID"].ToString() });
                    }
                }
                else
                {
                    return RedirectToAction("PasswordExpiry", "Employee");
                }
            }
            // If we got this far, something failed, redisplay form
            TempData["alertdata"] = "The Email Id or password provided is incorrect.";
            
            return View(model);
        }

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    //string sTempPwd= objGlobaldata.GenerateTempPassword();
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ForgotPassword(LoginModel model, FormCollection form)
        //{
        //    string sUsername = form["emailAddress"];
        //    EmployeeModels objEmp=new EmployeeModels();
        //    if (objGlobaldata.checkEmailAddressExists(sUsername))
        //    {
        //        //if (model.MailTempPassword(sUsername))
        //        //{
        //        //    return RedirectToAction("Login", "Account");
        //        //}
        //    }
        //    ModelState.AddModelError("", "The email id does not exists.");

        //    return View();
        //}


        //
        // POST: /Account/LogOff

        
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            objGlobaldata.ClearUserSession();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
