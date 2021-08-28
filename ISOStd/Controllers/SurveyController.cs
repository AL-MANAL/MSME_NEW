using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]

    public class SurveyController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public SurveyController()
        {
            ViewBag.Menutype = "CustomerMgmt";
            ViewBag.SubMenutype = "CustomerSurvey";
        }
        
        public ActionResult Index()
        {
            return View();
        }
       
        [AllowAnonymous]
        public ActionResult AddSurvey()
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurvey = new SurveyModels();
            try
            {
                // ViewBag.SubMenutype = "CustomerSurvey";
                // ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();

               
                if (Request.QueryString["Survey_TypeId"] != null && Request.QueryString["Survey_TypeId"] != "")
                {
                    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId"];
                    //ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId"]);
                    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId"]);
                    ViewBag.SurveyType = objSurvey.getSurveyNameById(ViewBag.Survey_TypeId);
                }
                //if (Request.QueryString["Survey_TypeId1"] != null && Request.QueryString["Survey_TypeId1"] != "")
                //{
                //    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId1"];
                //    ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId1"]);
                //    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId1"]);
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSurvey: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSurvey);
        }
        
        [HttpPost]
        public JsonResult GetSurveyQuestions(string Survey_Type)
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurvey = new SurveyModels();
            string Surveyname = objSurvey.getSurveyIDByName(Survey_Type);
            var data = objSurvey.GetSurveyQuestionsListbox(Surveyname);

            return Json(data);
        }
        
        [HttpPost]
        public JsonResult GetSurveyRatings(string Survey_Type)
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurvey = new SurveyModels();
            string Survey_TypeId = objSurvey.getSurveyIDByName(Survey_Type);
            SurveyRatingList objRatingList = new SurveyRatingList();
            objRatingList.lstSurveyRating = new List<SurveyRating>();
            try
            {
                DataSet dsRatingList = objSurvey.GetSurveyRating(Survey_TypeId);
                if (dsRatingList.Tables.Count > 0 && dsRatingList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsRatingList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SurveyRating objRating = new SurveyRating
                            {
                                SQ_OptionsId = dsRatingList.Tables[0].Rows[i]["SQ_OptionsId"].ToString(),
                                Survey_TypeId = dsRatingList.Tables[0].Rows[i]["Survey_TypeId"].ToString(),
                                RatingOptions = dsRatingList.Tables[0].Rows[i]["RatingOptions"].ToString(),
                                Weightage = (dsRatingList.Tables[0].Rows[i]["Weightage"].ToString()),
                            };
                            objRatingList.lstSurveyRating.Add(objRating);

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in GetSurveyRatings: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSurveyRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            var json = new JavaScriptSerializer().Serialize(objRatingList.lstSurveyRating);

            return Json(json);
        }
        
        [HttpPost]
        public JsonResult doesQuestionExist(string Questions, string Survey_TypeId)
        {
            SurveyModels objSurvey = new SurveyModels();

            var exists = objSurvey.checkQuestionExists(Questions, Survey_TypeId);

            return Json(exists);
        }
        
        [HttpPost]
        public JsonResult doesOptionExist(string RatingOptions)
        {
            SurveyModels objSurvey = new SurveyModels();

            var exists = objSurvey.checkRatingFactorExists(RatingOptions);

            return Json(exists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSurvey(SurveyModels objSurveyModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "CustomerSurvey";
                SurveyModels objSurvey = new SurveyModels();
                //ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();


                //ViewBag.SubMenutype = "CustomerSurvey";
                ViewBag.Survey_TypeId = objSurveyModels.Survey_TypeId;
                objSurveyModels.Survey_TypeId1 = objSurveyModels.getSurveyIDByName(objSurveyModels.Survey_TypeId);

                if (objSurveyModels.Questions != "")
                {

                    if (objSurveyModels.FunAddSurvey(objSurveyModels))
                    {
                        TempData["Successdata"] = "Added Survey details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Meeting type name is required.";
                }
                ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId);
                ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(objSurveyModels.Survey_TypeId);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSurvey: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.Survey_TypeId1 });

            //return View(objSurveyModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSurveyRating(SurveyModels objSurveyModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "CustomerSurvey";
                SurveyModels objSurvey = new SurveyModels();
                //ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();

                //ViewBag.SubMenutype = "CustomerSurvey";

                objSurveyModels.Survey_TypeId1 = objSurveyModels.getSurveyIDByName(form["Survey_TypeId"]);
                objSurveyModels.RatingOptions = form["RatingOptions"];
                objSurveyModels.Weightage = form["Weightage"];
                objSurveyModels.Survey_TypeId = form["Survey_Id"];
                if (AddSurveyRating(objSurveyModels))
                {
                    TempData["Successdata"] = "Added Survey rating details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId1);

                ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(objSurveyModels.Survey_TypeId1);

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSurveyRating: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.Survey_TypeId1/*, Survey_TypeId1 = objSurveyModels.Survey_TypeId1*/ });

        }
        
        public bool AddSurveyRating(SurveyModels objSurveyModels)
        {
            //ViewBag.SubMenutype = "CustomerSurvey";

            return objSurveyModels.FunAddSurveyRatingFactor(objSurveyModels);
        }
        
        [AllowAnonymous]
        public JsonResult SurveyQuestionUpdate(string SQId, string Questions)
        {

            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunUpdateSurveyQuestions(SQId, Questions))
                {
                    TempData["Successdata"] = "Survey details updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SurveyQuestionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");

            //if (SQId != null && SQId != "")
            //{
            //    return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
            //}

            //return RedirectToAction("AddSurvey");
        }

        [AllowAnonymous]
        public ActionResult SurveyDelete(string SQId)
        {

            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Survey details deleted successfully";
                    //return Json("Success", JsonRequestBehavior.AllowGet);

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SurveyDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            //if (SQId != null && SQId != "")
            //{
            //    return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
            //}

            return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
        }
        
        [AllowAnonymous]
        public ActionResult SurveyDelete1(string SQId)
        {

            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Survey details deleted successfully";
                    return Json("Success");

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SurveyDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            //if (SQId != null && SQId != "")
            //{
            //    return RedirectToAction("AddSurvey", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
            //}
            return Json("Failed");

        }

        [AllowAnonymous]
        public JsonResult SurveyRatingDelete(string SQ_OptionsId)
        {

            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyRatingFactor(SQ_OptionsId))
                {
                    TempData["Successdata"] = "Survey rating factor details deleted successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SurveyRatingDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }
        
        [AllowAnonymous]
        public ActionResult SurveyRatingDelete1(string SQ_OptionsId, string Survey_TypeId)
        {

            ViewBag.SubMenutype = "CustomerSurvey";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyRatingFactor(SQ_OptionsId))
                {
                    TempData["Successdata"] = "Survey rating factor details deleted successfully";

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SurveyDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return RedirectToAction("AddSurvey", new { Survey_TypeId = Survey_TypeId });
        }

        public FileResult GenerateSurveyForm()
        {
            if (objGlobaldata.GetCurrentUserSession() != null)
            {
                try
                {
                    SurveyModels objSurveyModels = new SurveyModels();
                    string Document = objSurveyModels.GenerateCustomerSurveyForm();
                    if (Document != null && Document != "") //(dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

                        return File(fsSource, MimeMapping.GetMimeMapping(Path.GetFileName(Document)));
                    }
                    return File(Server.MapPath("~/Views/Error/FileNotFound.html"), "text/html");
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in GenerateSurveyForm: " + ex.ToString());
                    return File(Server.MapPath("~/Views/Error/FileNotFound.html"), "text/html");
                }
            }
            return File(Server.MapPath("~/Views/Error/AccessDenied.html"), "text/html");
        }
         
        public ActionResult CustomerSurveyList(string branch_name)
        {
            try
            {
                //ViewBag.Menutype = "Feedback";
                ViewBag.SubMenutype = "CustomerSurvey";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                SurveyModels objSurveyModels = new SurveyModels();

                ViewBag.SubMenutype = "CustomerSurvey";
                //ViewBag.CustList = objGlobaldata.GetCustomerListbox();
                ViewBag.CustList = objGlobaldata.GetCustomerListboxwithBranch(sBranch_name);
                if(branch_name != "" && branch_name!=null)
                {
                    ViewBag.dsCustomerSurvey = objSurveyModels.GetCustSurvey(branch_name);
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    ViewBag.dsCustomerSurvey = objSurveyModels.GetCustSurvey(sBranch_name);
                }
                
                ViewBag.Survey_TypeId = objSurveyModels.getSurveyIDByName("Client Satisfaction Survey");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerSurveyList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        public JsonResult CustomerSurveyListSearch(string branch_name)
        {
            try
            {
                //ViewBag.Menutype = "Feedback";
                ViewBag.SubMenutype = "CustomerSurvey";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                SurveyModels objSurveyModels = new SurveyModels();

                ViewBag.SubMenutype = "CustomerSurvey";
                //ViewBag.CustList = objGlobaldata.GetCustomerListbox();
                ViewBag.CustList = objGlobaldata.GetCustomerListboxwithBranch(sBranch_name);
                ViewBag.dsCustomerSurvey = objSurveyModels.GetCustSurvey(branch_name);
                ViewBag.Survey_TypeId = objSurveyModels.getSurveyIDByName("Client Satisfaction Survey");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerSurveyListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCustomerSurvey(FormCollection form, HttpPostedFileBase CustSurveyFile)
        {
            try
            {
                ViewBag.SubMenutype = "CustomerSurvey";
                SurveyModels objSurveyModels = new SurveyModels();
                CustomerSurveyFeedbackList lstFeedback = new CustomerSurveyFeedbackList();
                lstFeedback.lstCustomerSurveyFeedback = new List<CustomerSurveyFeedbackJson>();

                string sCustId = form["CustId"];
                if (CustSurveyFile != null && CustSurveyFile.ContentLength > 0)
                {
                    try
                    {
                        string sContent = new StreamReader(CustSurveyFile.InputStream).ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        CustomerSurveyFeedbackJson[] FeedbackJson = js.Deserialize<CustomerSurveyFeedbackJson[]>(sContent);
                        if (FeedbackJson != null && FeedbackJson.Length > 0)
                        {
                            foreach (CustomerSurveyFeedbackJson objData in FeedbackJson)
                            {
                                CustomerSurveyFeedbackJson objCust = new CustomerSurveyFeedbackJson
                                {
                                    id = objData.id,
                                    answer = objData.answer
                                };

                                lstFeedback.lstCustomerSurveyFeedback.Add(objCust);
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "Invalid File content, please and upload again.";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objSurveyModels.FunAddCustomerFeedback(lstFeedback, sCustId))
                {
                    TempData["Successdata"] = "Added Survey details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UploadCustomerSurvey: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerSurveyList");
        }

        public ActionResult CustomerSurveyDetails()
        {
            CustomerModels objCustomer = new CustomerModels();
            ViewBag.SubMenutype = "CustomerSurvey";
            try
            {
                if (Request.QueryString["CustId"] != null && Request.QueryString["CustId"] != ""
                    && Request.QueryString["SurveyDate"] != null && Request.QueryString["SurveyDate"] != "")
                {
                    string sCustID = Request.QueryString["CustID"];
                    string sSUrveyDate = Request.QueryString["SurveyDate"];
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "select Customer_SurveyId, companyname, Questions, RatingOptions, SurveyDate from t_customer_survey as tcs, t_surveyquestions as ts, "
                        + " t_surveyquestion_options as tso, t_customer_info as tCust where tcs.SQId= ts.SQId and tcs.SQ_OptionsId= tso.SQ_OptionsId and"
                        + " tcs.custid=tCust.custid and SurveyDate='" + sSUrveyDate + "' and tcs.CustId='" + sCustID + "'";
                    DataSet dsSurvey = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSurvey.Tables.Count > 0 && dsSurvey.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.dsSurvey = dsSurvey;
                        return View();
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerSurveyList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerSurveyList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerSurveyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerSurveyList");
        }

        [HttpPost]
        public JsonResult GetTrainingEvaluationRatings(string Survey_Type)
        {
            SurveyModels objSurvey = new SurveyModels();
            string Surveyname = objSurvey.getSurveyIDByName(Survey_Type);
            var data = objSurvey.GetSurveyRatings(Surveyname);

            return Json(data);
        }       

    }
}
