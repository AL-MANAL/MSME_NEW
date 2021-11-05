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
using ISOStd.Filters;
using System;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class SupplierPerpRatingController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public SupplierPerpRatingController()
        {
            ViewBag.Menutype = "Suppliers";
        }

        [AllowAnonymous]
        public ActionResult AddSuppPerformanceEval()
        {
            SupplierPerpRatingModels objModel = new SupplierPerpRatingModels();
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";
                objModel.branch = objGlobaldata.GetCurrentUserSession().division;
                objModel.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.branch);
               // ViewBag.EmpList = objGlobaldata.GetGRolelikeList(objModel.branch, objModel.Department, objModel.Location, "%Auditor%");

                SurveyModels objSurvey = new SurveyModels();
                ViewBag.SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Performance Survey");
                ViewBag.SurveyRating = objSurvey.GetSurveyRating(objSurvey.getSurveyIDByName("Supplier Performance Survey"));
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
                ViewBag.Performance = objGlobaldata.GetDropdownList("Supplier Overall Performace");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppPerformanceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuppPerformanceEval(SupplierPerpRatingModels objModel, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {

                objModel.loggedby = objGlobaldata.GetCurrentUserSession().empid;
                objModel.na = form["N/A"];
                objModel.branch = form["branch"];
                objModel.Department = form["Department"];
                objModel.Location = form["Location"];
                //objModel.auditor = objGlobaldata.GetDeptIdByHrEmpId(objModel.auditor);

                DateTime dateValue;
                if (DateTime.TryParse(form["evalu_date"], out dateValue) == true)
                {
                    objModel.evalu_date = dateValue;
                }
                
                if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = "Perf" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objModel.upload = "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddSuppPerformanceEval-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                SupplierPerpRatingModelsList objList = new SupplierPerpRatingModelsList();
                objList.PerpList = new List<SupplierPerpRatingModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Performance Survey");

                foreach (var item in SurveyQuestions)
                {
                    SupplierPerpRatingModels objElements = new SupplierPerpRatingModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value]);

                    objList.PerpList.Add(objElements);
                }

                if (objModel.FunAddSupPerformanceEvaluation(objModel, objList))
                {
                    TempData["Successdata"] = "Added Performance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppPerformanceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SuppPerformanceEvalList");
        }
        
        [AllowAnonymous]
        public ActionResult SuppPerformanceEvalList(string SearchText, int? page, string branch_name)
        {
            SupplierPerpRatingModelsList ObjList = new SupplierPerpRatingModelsList();
            ObjList.PerpList = new List<SupplierPerpRatingModels>();

            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee, auditor, upload," +
                    " loggedby,overall_perf,branch,Department,Location from t_supplier_perf_rating where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {                  

                    try
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[i]["id_sup_rating"].ToString(),
                            auditor = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["auditor"].ToString()),
                            supplier_name = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[i]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[i]["upload"].ToString(),
                            overall_perf = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["overall_perf"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }
                      
                        ObjList.PerpList.Add(objSupplier);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(ObjList.PerpList.ToList());
        }

        [AllowAnonymous]
        public JsonResult SuppPerformanceEvalListSearch(string SearchText, int? page, string branch_name)
        {
            SupplierPerpRatingModelsList ObjList = new SupplierPerpRatingModelsList();
            ObjList.PerpList = new List<SupplierPerpRatingModels>();

            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee, auditor, upload, " +
                    "loggedby,overall_perf,branch,Department,Location  from t_supplier_perf_rating where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {                   
                    try
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[i]["id_sup_rating"].ToString(),
                            auditor = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["auditor"].ToString()),
                            supplier_name = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[i]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[i]["upload"].ToString(),
                            overall_perf = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["overall_perf"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),

                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }

                        ObjList.PerpList.Add(objSupplier);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult SuppPerformanceEvalEdit()
        {
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                if (Request.QueryString["id_sup_rating"] != null && Request.QueryString["id_sup_rating"] != "")
                {
                    string sid_sup_rating = Request.QueryString["id_sup_rating"];

                    string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee, auditor, upload," +
                        " loggedby,overall_perf,exceptional,satisfactory,unsatisfactory,na,insufficient,branch,Department,Location,evaluated_by  from t_supplier_perf_rating where id_sup_rating='" + sid_sup_rating + "'";


                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[0]["id_sup_rating"].ToString(),
                            auditor = /*objGlobaldata.GetEmpHrNameById*/(dsSupplier.Tables[0].Rows[0]["auditor"].ToString()),
                            supplier_name = /*objGlobaldata.GetSupplierNameById*/(dsSupplier.Tables[0].Rows[0]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[0]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            overall_perf = dsSupplier.Tables[0].Rows[0]["overall_perf"].ToString(),
                            exceptional = dsSupplier.Tables[0].Rows[0]["exceptional"].ToString(),
                            satisfactory = dsSupplier.Tables[0].Rows[0]["satisfactory"].ToString(),
                            unsatisfactory = dsSupplier.Tables[0].Rows[0]["unsatisfactory"].ToString(),
                            na = dsSupplier.Tables[0].Rows[0]["na"].ToString(),
                            insufficient = dsSupplier.Tables[0].Rows[0]["insufficient"].ToString(),
                            branch = (dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            evaluated_by = (dsSupplier.Tables[0].Rows[0]["evaluated_by"].ToString()),
                        };
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                        //ViewBag.EmpList = objGlobaldata.GetGRolelikeList(dsSupplier.Tables[0].Rows[0]["branch"].ToString(), dsSupplier.Tables[0].Rows[0]["Department"].ToString(), dsSupplier.Tables[0].Rows[0]["Location"].ToString(), "%Auditor%");

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }

                        sSqlstmt = "SELECT supplier_trans_id, id_sup_rating, SQId, SQ_OptionsId FROM t_supplier_perf_rating_trans where id_sup_rating='"
                             + objSupplier.id_sup_rating + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        SupplierPerpRatingModelsList objList = new SupplierPerpRatingModelsList();
                        objList.PerpList = new List<SupplierPerpRatingModels>();

                        SurveyModels objSurvey = new SurveyModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            SupplierPerpRatingModels objElements = new SupplierPerpRatingModels
                            {
                                supplier_trans_id = dsPerformanceElement.Tables[0].Rows[i]["supplier_trans_id"].ToString(),
                                SQId = (dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = (dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objList.PerpList.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objList;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;

                        ViewBag.SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Performance Survey");
                        ViewBag.SurveyRating = objSurvey.GetSurveyRating(objSurvey.getSurveyIDByName("Supplier Performance Survey"));
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.Supplier = objGlobaldata.GetSupplierList();
                        ViewBag.Performance = objGlobaldata.GetDropdownList("Supplier Overall Performace");

                        return View(objSupplier);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SuppPerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("SuppPerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SuppPerformanceEvalList");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuppPerformanceEvalEdit(SupplierPerpRatingModels objModel, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {

                //objModel.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                //objModel.Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objModel.Eval_ReviewedBy);
                objModel.na = form["N/A"];
                objModel.branch = form["branch"];
                objModel.Department = form["Department"];
                objModel.Location = form["Location"];

                DateTime dateValue;
                if (DateTime.TryParse(form["evalu_date"], out dateValue) == true)
                {
                    objModel.evalu_date = dateValue;
                }               

                if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = "Perf" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objModel.upload = "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalEdit: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                SupplierPerpRatingModelsList objList = new SupplierPerpRatingModelsList();
                objList.PerpList = new List<SupplierPerpRatingModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Supplier Performance Survey");

                foreach (var item in SurveyQuestions)
                {
                    SupplierPerpRatingModels objElements = new SupplierPerpRatingModels
                    {
                        SQId = form["Question " + item.Value],
                        SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value])
                    };

                    objList.PerpList.Add(objElements);
                }
                if (objList.PerpList.Count > 0)
                {
                    objList.PerpList[0].id_sup_rating = objModel.id_sup_rating;
                }

                if (objModel.FunUpdateSupPerformanceEvaluation(objModel, objList))
                {
                    TempData["Successdata"] = "Updated Performance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SuppPerformanceEvalList");
        }
        
        [AllowAnonymous]
        public JsonResult SupplierPerformanceDel(FormCollection form)
        {
            try
            {
                    if (form["id_sup_rating"] != null && form["id_sup_rating"] != "")
                    {

                        SupplierPerpRatingModels Doc = new SupplierPerpRatingModels();
                        string sid_sup_rating = form["id_sup_rating"];


                        if (Doc.FunDeleteSupPerformance(sid_sup_rating))
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
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return Json("Failed");
                    } 

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierPerformanceDel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        [AllowAnonymous]
        public ActionResult SuppPerformanceEvalDetails()
        {
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                if (Request.QueryString["id_sup_rating"] != null && Request.QueryString["id_sup_rating"] != "")
                {
                    string sid_sup_rating = Request.QueryString["id_sup_rating"];

                    string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee, auditor, upload, loggedby,overall_perf,overall_perf,exceptional,satisfactory,unsatisfactory,na,insufficient,branch,Department,Location,evaluated_by " +
                        " from t_supplier_perf_rating where id_sup_rating='" + sid_sup_rating + "'";


                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[0]["id_sup_rating"].ToString(),
                            auditor = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor"].ToString()),
                            supplier_name = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[0]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            overall_perf = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["overall_perf"].ToString()),
                            exceptional = dsSupplier.Tables[0].Rows[0]["exceptional"].ToString(),
                            satisfactory = dsSupplier.Tables[0].Rows[0]["satisfactory"].ToString(),
                            unsatisfactory = dsSupplier.Tables[0].Rows[0]["unsatisfactory"].ToString(),
                            na = dsSupplier.Tables[0].Rows[0]["na"].ToString(),
                            insufficient = dsSupplier.Tables[0].Rows[0]["insufficient"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            evaluated_by = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["evaluated_by"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }

                        sSqlstmt = "SELECT supplier_trans_id, id_sup_rating, SQId, SQ_OptionsId FROM t_supplier_perf_rating_trans where id_sup_rating='"
                             + objSupplier.id_sup_rating + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        SupplierPerpRatingModelsList objList = new SupplierPerpRatingModelsList();
                        objList.PerpList = new List<SupplierPerpRatingModels>();

                        SurveyModels objSurvey = new SurveyModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            SupplierPerpRatingModels objElements = new SupplierPerpRatingModels
                            {
                                //supplier_trans_id = dsPerformanceElement.Tables[0].Rows[i]["supplier_trans_id"].ToString(),
                                SQId = objSurvey.GetSurveyQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = objSurvey.GetSurveyRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                SQ_Weightage = objSurvey.GetSurveyRatingWeightageById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),                               
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objList.PerpList.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objList; 
                        return View(objSupplier);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SuppPerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("SuppPerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SuppPerformanceEvalList");
        }
        
        [AllowAnonymous]
        public ActionResult SuppPerformanceEvalInfo(int Id)
        {
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                string sId = Request.QueryString["Id"];
                if (sId != null && sId != "")
                {                   

                    string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee," +
                        " auditor, upload, loggedby,overall_perf,overall_perf,exceptional,satisfactory,unsatisfactory,na,insufficient,branch,Department,Location " +
                        " from t_supplier_perf_rating where id_sup_rating='" + sId + "'";
                    
                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[0]["id_sup_rating"].ToString(),
                            auditor = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor"].ToString()),
                            supplier_name = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[0]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            overall_perf = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["overall_perf"].ToString()),
                            exceptional = dsSupplier.Tables[0].Rows[0]["exceptional"].ToString(),
                            satisfactory = dsSupplier.Tables[0].Rows[0]["satisfactory"].ToString(),
                            unsatisfactory = dsSupplier.Tables[0].Rows[0]["unsatisfactory"].ToString(),
                            na = dsSupplier.Tables[0].Rows[0]["na"].ToString(),
                            insufficient = dsSupplier.Tables[0].Rows[0]["insufficient"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }

                        sSqlstmt = "SELECT supplier_trans_id, id_sup_rating, SQId, SQ_OptionsId FROM t_supplier_perf_rating_trans where id_sup_rating='"
                             + objSupplier.id_sup_rating + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        SupplierPerpRatingModelsList objList = new SupplierPerpRatingModelsList();
                        objList.PerpList = new List<SupplierPerpRatingModels>();

                        SurveyModels objSurvey = new SurveyModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            SupplierPerpRatingModels objElements = new SupplierPerpRatingModels
                            {
                                //supplier_trans_id = dsPerformanceElement.Tables[0].Rows[i]["supplier_trans_id"].ToString(),
                                SQId = objSurvey.GetSurveyQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = objSurvey.GetSurveyRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                SQ_Weightage = objSurvey.GetSurveyRatingWeightageById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objList.PerpList.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objList;
                        return View(objSupplier);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SuppPerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("SuppPerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SuppPerformanceEvalList");
        }

        [AllowAnonymous]
        public ActionResult SuppPerformanceEvalPDF(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";

                string id_sup_rating = form["id_sup_rating"];

                if (id_sup_rating != null && id_sup_rating != "")
                {
                    
                    string sSqlstmt = "select id_sup_rating, supplier_name, evalu_date, auditee, auditor, upload, loggedby,overall_perf,overall_perf,exceptional,satisfactory,unsatisfactory,na,insufficient" +
                        ",branch,Department,Location  from t_supplier_perf_rating where id_sup_rating='" + id_sup_rating + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        SupplierPerpRatingModels objSupplier = new SupplierPerpRatingModels
                        {
                            id_sup_rating = dsSupplier.Tables[0].Rows[0]["id_sup_rating"].ToString(),
                            auditor = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["auditor"].ToString()),
                            supplier_name = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["supplier_name"].ToString()),
                            auditee = dsSupplier.Tables[0].Rows[0]["auditee"].ToString(),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["loggedby"].ToString()),
                            upload = dsSupplier.Tables[0].Rows[0]["upload"].ToString(),
                            overall_perf = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["overall_perf"].ToString()),
                            exceptional = dsSupplier.Tables[0].Rows[0]["exceptional"].ToString(),
                            satisfactory = dsSupplier.Tables[0].Rows[0]["satisfactory"].ToString(),
                            unsatisfactory = dsSupplier.Tables[0].Rows[0]["unsatisfactory"].ToString(),
                            na = dsSupplier.Tables[0].Rows[0]["na"].ToString(),
                            insufficient = dsSupplier.Tables[0].Rows[0]["insufficient"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        {
                            objSupplier.evalu_date = dtValue;
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsSupplier = objCompany.GetCompanyDetailsForReport(dsSupplier);

                        string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                        dsSupplier = objGlobaldata.GetReportDetails(dsSupplier, objSupplier.id_sup_rating, loggedby, "SUPPLIER PERFORMANCE RATING REPORT");
                        ViewBag.CompanyInfo = dsSupplier;


                        sSqlstmt = "SELECT supplier_trans_id, id_sup_rating, SQId, SQ_OptionsId FROM t_supplier_perf_rating_trans where id_sup_rating='"
                             + objSupplier.id_sup_rating + "'";
                        ViewBag.PerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        ViewBag.Supplier = objSupplier;

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SuppPerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("SuppPerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerformanceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("SuppPerformanceEvalPDF")
            {
                //FileName = "Performance.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        public ActionResult GetRating()
        {
            try
            {
               // SupplierPerpRatingModels objRisk = new SupplierPerpRatingModels();
             
                string sql = "select RatingOptions,Weightage from t_survey_type a,t_surveyquestion_options b where a.Survey_TypeId=b.Survey_TypeId and a.TypeName='Supplier Performance Survey'";
                DataSet dsRating = objGlobaldata.Getdetails(sql);
                ViewBag.dsRating = dsRating;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetRating: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Supplier Performance Questions
        
        [AllowAnonymous]
        public ActionResult AddSuppPerfQuestions()
        {
            SurveyModels objSurvey = new SurveyModels();
            try
            {
                ViewBag.SubMenutype = "SupplierReportWithRate";
                //ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();
                ViewBag.Survey_Type = objSurvey.getSurveyIDByName("Supplier Performance Survey");
               
                //if (Request.QueryString["Survey_TypeId"] != null && Request.QueryString["Survey_TypeId"] != "")
                //{
                //    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId"];
                //    ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId"]);
                //    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId"]);
                //}
                //if (Request.QueryString["Survey_TypeId1"] != null && Request.QueryString["Survey_TypeId1"] != "")
                //{
                //    ViewBag.Survey_TypeId = Request.QueryString["Survey_TypeId1"];
                //    ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(Request.QueryString["Survey_TypeId1"]);
                //    ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(Request.QueryString["Survey_TypeId1"]);
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppPerfQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSurvey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSuppPerfQuestions(SurveyModels objSurveyModels, FormCollection form)
        {
            try
            {
                SurveyModels objSurvey = new SurveyModels();
                ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();


               
                ViewBag.Survey_Type = objSurveyModels.Survey_TypeId;
                ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                objSurveyModels.Survey_TypeId1 = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                if (objSurveyModels.Questions != "")
                {

                    if (objSurveyModels.FunAddSurvey(objSurveyModels))
                    {
                        TempData["Successdata"] = "Added Supplier Performance Questions successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id is required.";
                }
                ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId);
                ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(objSurveyModels.Survey_TypeId);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSuppPerfQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objSurveyModels);
        }

        [AllowAnonymous]
        public ActionResult SuppPerfQuestionsDelete(string SQId)
        {

            ViewBag.SubMenutype = "SupplierReportWithRate";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Supplier Performance details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppPerfQuestionsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }                       

            return RedirectToAction("AddSuppPerfQuestions", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
        }

        [AllowAnonymous]
        public ActionResult SuppPerfQuestionsDelete1(string SQId)
        {

            ViewBag.SubMenutype = "SupplierReportWithRate";
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
                objGlobaldata.AddFunctionalLog("Exception in SuppPerfQuestionsDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }          
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult SuppPerfQuestionUpdate(string SQId, string Questions)
        {

            ViewBag.SubMenutype = "SupplierReportWithRate";
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
                objGlobaldata.AddFunctionalLog("Exception in SuppPerfQuestionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
            
        }

    }
}