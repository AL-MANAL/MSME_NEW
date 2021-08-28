using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using ISOStd.Filters;

namespace ISOStd.Controllers
{

   
    public class SurveySetupController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        // GET: SurveySetup
        public ActionResult SurveySetupWizard()
        {
            SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();

            string sSqlstmt = "SELECT * from t_survey_setup_cloud";

            DataSet Step1 = objGlobaldata.Getdetails(sSqlstmt);

            sSqlstmt = "SELECT * from t_survey_setup_user_credentials";

            DataSet Step2 = objGlobaldata.Getdetails(sSqlstmt);

            if (Step1.Tables[0].Rows.Count > 0 && Step1.Tables[0].Rows.Count < 2)
            {
                ViewBag.Step1 = Step1.Tables[0].Rows[0]["remote_url"].ToString();

            }

            if (Step2.Tables[0].Rows.Count > 0 && Step2.Tables[0].Rows.Count < 2)
            {
                ObjSurveySetupModel.Email = Step2.Tables[0].Rows[0]["user_email"].ToString();
                ObjSurveySetupModel.Password = Step2.Tables[0].Rows[0]["user_password"].ToString();


            }
            if ((ViewBag.Step1 !="" && ViewBag.Step1 !=null) && (ObjSurveySetupModel.Email != "" && ObjSurveySetupModel.Email != null) && (ObjSurveySetupModel.Password != "" && ObjSurveySetupModel.Password != null))
            {
                return RedirectToAction("SurveySetupDetails");

            }
            else
            {
                return View();
            }
        }

        public ActionResult SurveySetupDetails()
        {
            SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();

            string sSqlstmt = "SELECT * from t_survey_setup_cloud";

            DataSet Step1 = objGlobaldata.Getdetails(sSqlstmt);

            sSqlstmt = "SELECT * from t_survey_setup_user_credentials";

            DataSet Step2 = objGlobaldata.Getdetails(sSqlstmt);

            if (Step1.Tables[0].Rows.Count > 0 && Step1.Tables[0].Rows.Count < 2)
            {
                ObjSurveySetupModel.Cloud = Step1.Tables[0].Rows[0]["remote_url"].ToString();

            }

            if (Step2.Tables[0].Rows.Count > 0 && Step2.Tables[0].Rows.Count < 2)
            {
                ObjSurveySetupModel.Email = Step2.Tables[0].Rows[0]["user_email"].ToString();
                ObjSurveySetupModel.Password = Step2.Tables[0].Rows[0]["user_password"].ToString();

            }
            if ((ObjSurveySetupModel.Cloud != "" && ObjSurveySetupModel.Cloud != null) && (ObjSurveySetupModel.Email != "" && ObjSurveySetupModel.Email != null) && (ObjSurveySetupModel.Password != "" && ObjSurveySetupModel.Password != null))
            {
                return View(ObjSurveySetupModel);
            }
            else
            {
                return RedirectToAction("SurveySetupWizard");

            }
          
        }



        [HttpPost]
    
        public ActionResult AddCloud(string cloud)
        {
            SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();
            ObjSurveySetupModel.Cloud = cloud;
            try
            {
                if (ObjSurveySetupModel.FunAddCloud(ObjSurveySetupModel))
                {
                    //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                    return Json("Success");

                }
                else
                {
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Adding Cloud: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

           return Json("Failed");
        }



        [HttpPost]

        public ActionResult AddCredentials(string user,string password)
        {
            SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();
            ObjSurveySetupModel.Email = user;
            ObjSurveySetupModel.Password = password;

            try
            {
                if (ObjSurveySetupModel.FunAddCredentials(ObjSurveySetupModel))
                {
                    //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                    return Json("Success");

                }
                else
                {
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Adding Cloud: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }



        [HttpPost]

        public ActionResult DeleteSurvey()
        {
            SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();
            try
            {
                if (ObjSurveySetupModel.FunDelete())
                {
                    //objEmployeeModel.MailTempPassword(objEmployeeModel.emailAddress);
                    return Json("Success");

                }
                else
                {
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Adding Cloud: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }






    }
}