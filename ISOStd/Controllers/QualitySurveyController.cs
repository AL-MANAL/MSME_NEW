using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using System.IO;
using PagedList;
using PagedList.Mvc;
using Rotativa;

namespace ISOStd.Controllers
{
    public class QualitySurveyController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public QualitySurveyController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "QualitySurvey";
        }
        [AllowAnonymous]
        public ActionResult AddQualitySurvey()
        {
            try
            {
                ViewBag.Survey = objGlobaldata.GetDropdownList("Quality Survey");
                ViewBag.SurveyCriteria = objGlobaldata.GetAuditCriteria();
                ViewBag.Limit = objGlobaldata.GetConstantValue("QualityLimit");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox(); 
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQualitySurvey: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQualitySurvey(QualitySurveyModels objQuality, FormCollection form, IEnumerable<HttpPostedFileBase> Report)
        {
            try
            {
                objQuality.Survey_for = form["Survey_for"];
                objQuality.ThirdParty = form["ThirdParty"];
                objQuality.ConductedBy = form["ConductedBy"];
                objQuality.Criteria = form["Criteria"];
                objQuality.Location = form["Location"];
                objQuality.IsLimit = form["IsLimit"];
                objQuality.Remarks = form["Remarks"];
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (DateTime.TryParse(form["Survey_date"], out dateValue) == true)
                {
                    objQuality.Survey_date = dateValue;
                }
                if (DateTime.TryParse(form["ExpDate"], out dateValue) == true)
                {
                    objQuality.ExpDate = dateValue;
                }
                if (DateTime.TryParse(form["ReviewDate"], out dateValue) == true)
                {
                    objQuality.ReviewDate = dateValue;
                }

                if (Report != null && files.ContentLength > 0)
                {
                    objQuality.Report = "";
                    foreach (var file in Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "QualitySurvey" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);   
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objQuality.Report = objQuality.Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objQuality.Report = objQuality.Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objQuality.FunAddQualitySurvey(objQuality))
                {
                    TempData["Successdata"] = "Added Quality Survey details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQualitySurvey: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QualitySurveyList");
        }
          
        [AllowAnonymous]
        public ActionResult QualitySurveyList(string branch_name)
        {
            QualitySurveyModelsList objQualityList = new QualitySurveyModelsList();
            objQualityList.lstQuality = new List<QualitySurveyModels>();
           
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_qualitysurvey,Survey_date,Survey_for,ThirdParty,ConductedBy,Criteria,Report,ExpDate,"
                + "ReviewDate,Location,IsLimit,Remarks,VerifiedBy from t_qualitysurvey where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by Survey_date desc";

                DataSet dsQualityList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsQualityList.Tables.Count > 0 && dsQualityList.Tables[0].Rows.Count > 0)
                {
                    
                       
                    
                    for (int i = 0; i < dsQualityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QualitySurveyModels objQualityModels = new QualitySurveyModels
                            {
                                id_qualitysurvey = Convert.ToInt16(dsQualityList.Tables[0].Rows[i]["id_qualitysurvey"].ToString()),
                                Survey_for = objGlobaldata.GetDropdownitemById(dsQualityList.Tables[0].Rows[i]["Survey_for"].ToString()),
                                ThirdParty = objGlobaldata.GetSupplierNameById(dsQualityList.Tables[0].Rows[i]["ThirdParty"].ToString()),
                                ConductedBy = dsQualityList.Tables[0].Rows[i]["ConductedBy"].ToString(),
                                Criteria =/*objGlobaldata.GetIsoStdDescriptionById*/(dsQualityList.Tables[0].Rows[i]["Criteria"].ToString()),
                                Report = dsQualityList.Tables[0].Rows[i]["Report"].ToString(),
                                Location = dsQualityList.Tables[0].Rows[i]["Location"].ToString(),
                                IsLimit = dsQualityList.Tables[0].Rows[i]["IsLimit"].ToString(),
                                Remarks = dsQualityList.Tables[0].Rows[i]["Remarks"].ToString(),
                                VerifiedBy = objGlobaldata.GetMultiHrEmpNameById(dsQualityList.Tables[0].Rows[i]["VerifiedBy"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["Survey_date"].ToString(), out dtValue))
                            {
                                objQualityModels.Survey_date = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["ExpDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ExpDate = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["ReviewDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ReviewDate = dtValue;
                            }

                            objQualityList.lstQuality.Add(objQualityModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QualitySurveyList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objQualityList.lstQuality.ToList());
        }

        [AllowAnonymous]
        public JsonResult QualitySurveyListSearch(string branch_name)
        {
            QualitySurveyModelsList objQualityList = new QualitySurveyModelsList();
            objQualityList.lstQuality = new List<QualitySurveyModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_qualitysurvey,Survey_date,Survey_for,ThirdParty,ConductedBy,Criteria,Report,ExpDate,"
                + "ReviewDate,Location,IsLimit,Remarks,VerifiedBy from t_qualitysurvey where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by Survey_date desc";

                DataSet dsQualityList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsQualityList.Tables.Count > 0 && dsQualityList.Tables[0].Rows.Count > 0)
                {

                   

                    for (int i = 0; i < dsQualityList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QualitySurveyModels objQualityModels = new QualitySurveyModels
                            {
                                id_qualitysurvey = Convert.ToInt16(dsQualityList.Tables[0].Rows[i]["id_qualitysurvey"].ToString()),
                                Survey_for = objGlobaldata.GetDropdownitemById(dsQualityList.Tables[0].Rows[i]["Survey_for"].ToString()),
                                ThirdParty = objGlobaldata.GetSupplierNameById(dsQualityList.Tables[0].Rows[i]["ThirdParty"].ToString()),
                                ConductedBy = dsQualityList.Tables[0].Rows[i]["ConductedBy"].ToString(),
                                Criteria = /*objGlobaldata.GetIsoStdDescriptionById*/(dsQualityList.Tables[0].Rows[i]["Criteria"].ToString()),
                                Report = dsQualityList.Tables[0].Rows[i]["Report"].ToString(),
                                Location = dsQualityList.Tables[0].Rows[i]["Location"].ToString(),
                                IsLimit = dsQualityList.Tables[0].Rows[i]["IsLimit"].ToString(),
                                Remarks = dsQualityList.Tables[0].Rows[i]["Remarks"].ToString(),
                                VerifiedBy = objGlobaldata.GetMultiHrEmpNameById(dsQualityList.Tables[0].Rows[i]["VerifiedBy"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["Survey_date"].ToString(), out dtValue))
                            {
                                objQualityModels.Survey_date = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["ExpDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ExpDate = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[i]["ReviewDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ReviewDate = dtValue;
                            }

                            objQualityList.lstQuality.Add(objQualityModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QualitySurveyListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult QualitySurveyDocDelete(FormCollection form)
        {
            try
            {
                
                      if (form["id_qualitysurvey"] != null && form["id_qualitysurvey"] != "")
                        {

                            QualitySurveyModels Doc = new QualitySurveyModels();
                            string sid_qualitysurvey = form["id_qualitysurvey"];


                            if (Doc.FunDeleteQualitySurveyDoc(sid_qualitysurvey))
                            {
                                TempData["Successdata"] = "Document deleted successfully";
                                return Json("Success");
                            }
                            else
                            {
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return Json("Failed");
                            }
                        }
                        else
                        {
                            TempData["alertdata"] = "Quality Survey Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
          
        [AllowAnonymous]
        public ActionResult QualitySurveyEdit()
        {
            QualitySurveyModels objQualityModels = new QualitySurveyModels();
            try
            {
                ViewBag.Survey = objGlobaldata.GetDropdownList("Quality Survey");
                ViewBag.SurveyCriteria = objGlobaldata.GetAuditCriteria();
                ViewBag.Limit = objGlobaldata.GetConstantValue("QualityLimit");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Supplier = objGlobaldata.GetSupplierList();

                if (Request.QueryString["id_qualitysurvey"] != null && Request.QueryString["id_qualitysurvey"] != "")
                {
                    string sid_qualitysurvey = Request.QueryString["id_qualitysurvey"];

                    string sSqlstmt = "select id_qualitysurvey,Survey_date,Survey_for,ThirdParty,ConductedBy,Criteria,"
                    + "Report,ExpDate,ReviewDate,Location,IsLimit,Remarks,VerifiedBy from t_qualitysurvey where id_qualitysurvey='" + sid_qualitysurvey + "'";

                    DataSet dsQualityList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsQualityList.Tables.Count > 0 && dsQualityList.Tables[0].Rows.Count > 0)
                    {

                        objQualityModels = new QualitySurveyModels
                        {
                            id_qualitysurvey = Convert.ToInt16(dsQualityList.Tables[0].Rows[0]["id_qualitysurvey"].ToString()),
                            Survey_for = objGlobaldata.GetDropdownitemById(dsQualityList.Tables[0].Rows[0]["Survey_for"].ToString()),
                            ThirdParty = dsQualityList.Tables[0].Rows[0]["ThirdParty"].ToString(),
                            ConductedBy = dsQualityList.Tables[0].Rows[0]["ConductedBy"].ToString(),
                            Criteria = /*objGlobaldata.GetIsoStdDescriptionById*/(dsQualityList.Tables[0].Rows[0]["Criteria"].ToString()),
                            Report = dsQualityList.Tables[0].Rows[0]["Report"].ToString(),
                            Location = dsQualityList.Tables[0].Rows[0]["Location"].ToString(),
                            IsLimit = dsQualityList.Tables[0].Rows[0]["IsLimit"].ToString(),
                            Remarks = dsQualityList.Tables[0].Rows[0]["Remarks"].ToString(),
                            VerifiedBy = (dsQualityList.Tables[0].Rows[0]["VerifiedBy"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["Survey_date"].ToString(), out dtValue))
                        {
                            objQualityModels.Survey_date = dtValue;
                        }
                        if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ExpDate"].ToString(), out dtValue))
                        {
                            objQualityModels.ExpDate = dtValue;
                        }
                        if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ReviewDate"].ToString(), out dtValue))
                        {
                            objQualityModels.ReviewDate = dtValue;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "QualitySurveyID cannot be Null or empty";
                        return RedirectToAction("QualitySurveyList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "QualitySurveyID cannot be Null or empty";
                    return RedirectToAction("QualitySurveyList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QualitySurveyList");
            }
            return View(objQualityModels);
        }
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QualitySurveyEdit(QualitySurveyModels objQuality, FormCollection form, IEnumerable<HttpPostedFileBase> Report)
        {
            try
            {

                objQuality.Survey_for = form["Survey_for"];
                objQuality.ThirdParty = form["ThirdParty"];
                objQuality.ConductedBy = form["ConductedBy"];
                objQuality.Criteria = form["Criteria"];
                objQuality.Location = form["Location"];
                objQuality.IsLimit = form["IsLimit"];
                objQuality.Remarks = form["Remarks"];
                string QCDelete = Request.Form["QCDocsValselectall"]; 
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (DateTime.TryParse(form["Survey_date"], out dateValue) == true)
                {
                    objQuality.Survey_date = dateValue;
                }
                if (DateTime.TryParse(form["ExpDate"], out dateValue) == true)
                {
                    objQuality.ExpDate = dateValue;
                }
                if (DateTime.TryParse(form["ReviewDate"], out dateValue) == true)
                {
                    objQuality.ReviewDate = dateValue;
                }


                if (Report != null && files.ContentLength > 0)
                {
                    objQuality.Report = "";
                    foreach (var file in Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "QualitySurvey" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);                      
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objQuality.Report = objQuality.Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objQuality.Report = objQuality.Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objQuality.Report = objQuality.Report + "," + form["QCDocsVal"];
                    objQuality.Report = objQuality.Report.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objQuality.Report = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objQuality.Report = null;
                }
                if (objQuality.FunUpdateQualitySurvey(objQuality))
                {
                    TempData["Successdata"] = "Quality Survey updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QualitySurveyList");
            }

            return RedirectToAction("QualitySurveyList");
        }

          
        [AllowAnonymous]
        public ActionResult QualitySurveyDetails()
        {
            QualitySurveyModels objQualityModels = new QualitySurveyModels();
            try
            {
                if (Request.QueryString["id_qualitysurvey"] != null && Request.QueryString["id_qualitysurvey"] != "")
                {
                    string sid_qualitysurvey = Request.QueryString["id_qualitysurvey"];
                    string sSqlstmt = "select id_qualitysurvey,Survey_date,Survey_for,ThirdParty,ConductedBy,Criteria,"
                    + "Report,ExpDate,ReviewDate,Location,IsLimit,Remarks,VerifiedBy from t_qualitysurvey where id_qualitysurvey='" + sid_qualitysurvey + "'";

                    DataSet dsQualityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsQualityList.Tables.Count > 0 && dsQualityList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objQualityModels = new QualitySurveyModels
                            {
                                id_qualitysurvey = Convert.ToInt16(dsQualityList.Tables[0].Rows[0]["id_qualitysurvey"].ToString()),
                                Survey_for = objGlobaldata.GetDropdownitemById(dsQualityList.Tables[0].Rows[0]["Survey_for"].ToString()),
                                ThirdParty = objGlobaldata.GetSupplierNameById(dsQualityList.Tables[0].Rows[0]["ThirdParty"].ToString()),
                                ConductedBy = dsQualityList.Tables[0].Rows[0]["ConductedBy"].ToString(),
                                Criteria = /*objGlobaldata.GetIsoStdDescriptionById*/(dsQualityList.Tables[0].Rows[0]["Criteria"].ToString()),
                                Report = dsQualityList.Tables[0].Rows[0]["Report"].ToString(),
                                Location = dsQualityList.Tables[0].Rows[0]["Location"].ToString(),
                                IsLimit = dsQualityList.Tables[0].Rows[0]["IsLimit"].ToString(),
                                Remarks = dsQualityList.Tables[0].Rows[0]["Remarks"].ToString(),
                                VerifiedBy = objGlobaldata.GetMultiHrEmpNameById(dsQualityList.Tables[0].Rows[0]["VerifiedBy"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["Survey_date"].ToString(), out dtValue))
                            {
                                objQualityModels.Survey_date = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ExpDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ExpDate = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ReviewDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ReviewDate = dtValue;
                            }

                            return View(objQualityModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QualitySurveyDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("QualitySurveyList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Objectives Id cannot be null";
                    return RedirectToAction("QualitySurveyList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QualitySurveyList");

        }
          
        [AllowAnonymous]
        public ActionResult QualitySurveyInfo(int id)
        {
            QualitySurveyModels objQualityModels = new QualitySurveyModels();
            try
            {
                    string sSqlstmt = "select id_qualitysurvey,Survey_date,Survey_for,ThirdParty,ConductedBy,Criteria,"
                    + "Report,ExpDate,ReviewDate,Location,IsLimit,Remarks,VerifiedBy from t_qualitysurvey where id_qualitysurvey='" + id + "'";

                    DataSet dsQualityList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsQualityList.Tables.Count > 0 && dsQualityList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objQualityModels = new QualitySurveyModels
                            {
                                id_qualitysurvey = Convert.ToInt16(dsQualityList.Tables[0].Rows[0]["id_qualitysurvey"].ToString()),
                                Survey_for = objGlobaldata.GetDropdownitemById(dsQualityList.Tables[0].Rows[0]["Survey_for"].ToString()),
                                ThirdParty = objGlobaldata.GetSupplierNameById(dsQualityList.Tables[0].Rows[0]["ThirdParty"].ToString()),
                                ConductedBy = dsQualityList.Tables[0].Rows[0]["ConductedBy"].ToString(),
                                Criteria =/* objGlobaldata.GetIsoStdDescriptionById*/(dsQualityList.Tables[0].Rows[0]["Criteria"].ToString()),
                                Report = dsQualityList.Tables[0].Rows[0]["Report"].ToString(),
                                Location = dsQualityList.Tables[0].Rows[0]["Location"].ToString(),
                                IsLimit = dsQualityList.Tables[0].Rows[0]["IsLimit"].ToString(),
                                Remarks = dsQualityList.Tables[0].Rows[0]["Remarks"].ToString(),
                                VerifiedBy = objGlobaldata.GetMultiHrEmpNameById(dsQualityList.Tables[0].Rows[0]["VerifiedBy"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["Survey_date"].ToString(), out dtValue))
                            {
                                objQualityModels.Survey_date = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ExpDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ExpDate = dtValue;
                            }
                            if (DateTime.TryParse(dsQualityList.Tables[0].Rows[0]["ReviewDate"].ToString(), out dtValue))
                            {
                                objQualityModels.ReviewDate = dtValue;
                            }

                            return View(objQualityModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QualitySurveyDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("QualitySurveyList");
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QualitySurveyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QualitySurveyList");

        }
    }
}