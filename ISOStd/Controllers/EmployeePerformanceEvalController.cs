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
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmployeePerformanceEvalController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public EmployeePerformanceEvalController()
        {
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "EmployeePerformanceEval";
        }
        //
        // GET: /EmployeePerformanceEval/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /EmployeePerformanceEval/AddEmployeePerformanceEval

        [AllowAnonymous]
        public ActionResult AddEmployeePerformanceEval()
        {
            try
            {
                EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
                string sbranch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.PeformanceQuestions = objModel.GetQuestionListBox();
                ViewBag.PeformanceRating = objModel.PerformanceRating();
                ViewBag.PeformanceSection = objModel.GetSectionsList();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.DeptHeadList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmpEvaluatedByList();
                ViewBag.EmpHead = objGlobaldata.GetDeptHeadList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeePerformanceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        // POST: /EmployeePerformanceEval/AddEmployeePerformanceEval

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeePerformanceEval(EmpPerformanceEvalModels objEmpPerformanceEval, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {

                objEmpPerformanceEval.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;

                objEmpPerformanceEval.Dept_id = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.emp_id);
                objEmpPerformanceEval.Designation = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.emp_id);

                objEmpPerformanceEval.Eval_DoneBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.Eval_DoneBy);
                objEmpPerformanceEval.Eval_DoneBy_Desig = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.Eval_DoneBy);

                objEmpPerformanceEval.Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.Eval_ReviewedBy);
                objEmpPerformanceEval.Eval_ReviewedBy_Desig = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.Eval_ReviewedBy);

                DateTime dateValue;
                if (DateTime.TryParse(form["Evaluation_DoneOn"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluation_DoneOn = dateValue;
                }
                if (DateTime.TryParse(form["Evaluated_From"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluated_From = dateValue;
                }

                if (DateTime.TryParse(form["Evaluated_To"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluated_To = dateValue;
                }
                if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = "Perf" + "_" + objEmpPerformanceEval.emp_id + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objEmpPerformanceEval.DocUploadPath = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmployeePerformanceEval-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                EmpPerformanceElementsModelsList objEmpPerformanceElements = new EmpPerformanceElementsModelsList();
                objEmpPerformanceElements.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                //EmpPerformanceElementsModels objSurvey = new EmpPerformanceElementsModels();
                //MultiSelectList SurveyQuestions = objSurvey.GetQuestionList();
                EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels();

                MultiSelectList PeformanceQuestions=objElements.GetQuestionListBox();

                foreach (var item in PeformanceQuestions)
                {
                    objElements = new EmpPerformanceElementsModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = form["SQ_OptionsId " + item.Value];

                    objEmpPerformanceElements.lstEmpPerformanceElements.Add(objElements);
                }

                if (objEmpPerformanceEval.FunAddEmpPerformanceEvaluation(objEmpPerformanceEval, objEmpPerformanceElements))
                {
                    EmpPerformanceEvalModels objMdls = new EmpPerformanceEvalModels();
                    objMdls.SendJrMgrPerpEmail("Employee Performace Evaluation");
                    TempData["Successdata"] = "Added Performance details successfully and Email is send to Manager";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeePerformanceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }


        [AllowAnonymous]
        public JsonResult EmployeePerformanceDocDelete(FormCollection form)
        {
            try
            {

              
                    if (form["Performance_EvalId"] != null && form["Performance_EvalId"] != "")
                    {

                        EmpPerformanceEvalModels Doc = new EmpPerformanceEvalModels();
                        string sPerformance_EvalId = form["Performance_EvalId"];


                        if (Doc.FunDeletePerformanceDoc(sPerformance_EvalId))
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
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        //
        // GET: /EmployeePerformanceEval/EmployeePerformanceEvalList


        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalList(string SearchText, string PlanfromDate, string PlanToDate, string chkAll, int? page, string branch_name)
        {
            EmpPerformanceEvalModelsList objEmpPerformanceEvalList = new EmpPerformanceEvalModelsList();
            objEmpPerformanceEvalList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

            try
            {
                string empid = (objGlobaldata.GetCurrentUserSession().empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                DataSet dsCustomer;
                if (branch_name != null && branch_name != "")
                {
                    dsCustomer = objGlobaldata.GetDecendants(empid, branch_name);
                }
                else
                {
                    dsCustomer = objGlobaldata.GetDecendants(empid, sBranch_name);
                }
                       

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {                  

                    try
                    {
                        EmpPerformanceEvalModels objCustomer = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsCustomer.Tables[0].Rows[i]["Performance_EvalId"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["emp_id"].ToString()),
                            Designation = dsCustomer.Tables[0].Rows[i]["Designation"].ToString(),
                            Dept_id = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Dept_id"].ToString()),
                            Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy"].ToString()),
                            Eval_DoneById =(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsCustomer.Tables[0].Rows[i]["Weakness"].ToString(),
                            Strengths = (dsCustomer.Tables[0].Rows[i]["Strengths"].ToString()),
                            Training_Reqd = dsCustomer.Tables[0].Rows[i]["Training_Reqd"].ToString(),
                            Actions_Taken = dsCustomer.Tables[0].Rows[i]["Actions_Taken"].ToString(),
                            Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["LoggedBy"].ToString()),
                            DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                            JrMgr = dsCustomer.Tables[0].Rows[i]["JrMgr"].ToString(),
                            Comment_JrMgr = dsCustomer.Tables[0].Rows[i]["Comment_JrMgr"].ToString(),
                            SrMgr = dsCustomer.Tables[0].Rows[i]["SrMgr"].ToString(),
                            Comment_SrMgr = dsCustomer.Tables[0].Rows[i]["Comment_SrMgr"].ToString(),
                            sstatus= dsCustomer.Tables[0].Rows[i]["sstatus"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluation_DoneOn"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluation_DoneOn = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_From"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluated_From = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_To"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluated_To = dtValue;
                        }
                        objEmpPerformanceEvalList.lstEmpPerformanceEvalModels.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0]; 
                    }
                }
                //EmpPerformanceElementsModels objSurvey = new EmpPerformanceElementsModels();

                //ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName("Employee Performance");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            
            return View(objEmpPerformanceEvalList.lstEmpPerformanceEvalModels.ToList());
        }

        [AllowAnonymous]
        public JsonResult EmployeePerformanceEvalListSearch(string SearchText, string PlanfromDate, string PlanToDate, string chkAll, int? page, string branch_name)
        {
            EmpPerformanceEvalModelsList objEmpPerformanceEvalList = new EmpPerformanceEvalModelsList();
            objEmpPerformanceEvalList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

            try
            {
                string empid = (objGlobaldata.GetCurrentUserSession().empid);
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                DataSet dsCustomer;
                if (branch_name != null && branch_name != "")
                {
                    dsCustomer = objGlobaldata.GetDecendants(empid, branch_name);
                }
                else
                {
                    dsCustomer = objGlobaldata.GetDecendants(empid, sBranch_name);
                }

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                   

                    try
                    {
                        EmpPerformanceEvalModels objCustomer = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsCustomer.Tables[0].Rows[i]["Performance_EvalId"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["emp_id"].ToString()),
                            Designation = dsCustomer.Tables[0].Rows[i]["Designation"].ToString(),
                            Dept_id = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Dept_id"].ToString()),
                            Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsCustomer.Tables[0].Rows[i]["Weakness"].ToString(),
                            Strengths = (dsCustomer.Tables[0].Rows[i]["Strengths"].ToString()),
                            Training_Reqd = dsCustomer.Tables[0].Rows[i]["Training_Reqd"].ToString(),
                            Actions_Taken = dsCustomer.Tables[0].Rows[i]["Actions_Taken"].ToString(),
                            Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["LoggedBy"].ToString()),
                            DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                            JrMgr = dsCustomer.Tables[0].Rows[i]["JrMgr"].ToString(),
                            Comment_JrMgr = dsCustomer.Tables[0].Rows[i]["Comment_JrMgr"].ToString(),
                            SrMgr = dsCustomer.Tables[0].Rows[i]["SrMgr"].ToString(),
                            Comment_SrMgr = dsCustomer.Tables[0].Rows[i]["Comment_SrMgr"].ToString(),
                            sstatus = dsCustomer.Tables[0].Rows[i]["sstatus"].ToString(),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluation_DoneOn"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluation_DoneOn = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_From"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluated_From = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_To"].ToString(), out dtValue))
                        {
                            objCustomer.Evaluated_To = dtValue;
                        }
                        objEmpPerformanceEvalList.lstEmpPerformanceEvalModels.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
               // EmpPerformanceElementsModels objSurvey = new EmpPerformanceElementsModels();

            //    ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName("Employee Performance");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        
        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalDetails()
        {

            try
            {
                if (Request.QueryString["Performance_EvalId"] != null && Request.QueryString["Performance_EvalId"] != "")
                {
                    string sPerformance_EvalId = Request.QueryString["Performance_EvalId"];

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,Comment_SrMgr,SrMgr_Comment_Date," +
                    "(case when (Comment_JrMgr is null || Comment_JrMgr = '')then 'Pending Reviewer' when (Comment_SrMgr is null || Comment_SrMgr = '') then 'Pending Approval' else 'Evaluation Completed' end) as sstatus" +
                    " from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";


                    DataSet dsPerformance = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPerformance.Tables.Count > 0 && dsPerformance.Tables[0].Rows.Count > 0)
                    {
                        EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsPerformance.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["emp_id"].ToString()),
                            Designation = dsPerformance.Tables[0].Rows[0]["Designation"].ToString(),
                            Dept_id = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Eval_DoneById = (dsPerformance.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsPerformance.Tables[0].Rows[0]["Weakness"].ToString(),
                            Strengths = (dsPerformance.Tables[0].Rows[0]["Strengths"].ToString()),
                            Training_Reqd = dsPerformance.Tables[0].Rows[0]["Training_Reqd"].ToString(),
                            Actions_Taken = dsPerformance.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            //Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            //Eval_ReviewedBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_Desig"].ToString(),
                            //Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_DeptId"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            DocUploadPath = dsPerformance.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            JrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            Comment_JrMgr = dsPerformance.Tables[0].Rows[0]["Comment_JrMgr"].ToString(),
                            SrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            Comment_SrMgr = dsPerformance.Tables[0].Rows[0]["Comment_SrMgr"].ToString(),
                            sstatus = dsPerformance.Tables[0].Rows[0]["sstatus"].ToString(),

                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluation_DoneOn = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_From"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_From = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_To"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_To = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.JrMgr_Comment_Date = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["SrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.SrMgr_Comment_Date = dtValue;
                        }
                        //sSqlstmt = "SELECT Performance_Id, Performance_EvalId, SQId, SQ_OptionsId FROM t_emp_performance_elements where Performance_EvalId='"
                        //    + objPerformance.Performance_EvalId + "'";

                        //sSqlstmt = " SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section FROM t_emp_performance_elements t," +
                        //    " t_emp_performance_eval_questions tt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId order by Section asc, Performance_Id asc";

                        string sSqlstmt1 = "select max(Weightage) as Weightage  from t_emp_performance_eval_rating where active=1";
                        ViewBag.MaxRate = objGlobaldata.Getdetails(sSqlstmt1);

                        sSqlstmt = "SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section,item_fulldesc as Section_Weightage FROM t_emp_performance_elements t, " +
                            "t_emp_performance_eval_questions tt,dropdownitems ttt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId" +
                            " and tt.Section = ttt.item_id order by Section asc, Performance_Id asc";

                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        EmpPerformanceElementsModelsList objEmpPerformanceEvalList = new EmpPerformanceElementsModelsList();
                        objEmpPerformanceEvalList.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                        EmpPerformanceElementsModels objElementMdl = new EmpPerformanceElementsModels();
                       
                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels
                            {
                                SQId = objElementMdl.GetQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = objElementMdl.GetRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                SQ_Weightage = objElementMdl.GetRatingWeightageById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                                Section = (dsPerformanceElement.Tables[0].Rows[i]["Section"].ToString()),
                                Section_Weightage = (dsPerformanceElement.Tables[0].Rows[i]["Section_Weightage"].ToString()),
                            };
                            
                            objEmpPerformanceEvalList.lstEmpPerformanceElements.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objEmpPerformanceEvalList;
                        ViewBag.PeformanceSection = objElementMdl.GetSectionsList();

                        return View(objPerformance);

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeePerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("EmployeePerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }
        
        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalInfo(int id)
        {

            try
            {
                string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                + " LoggedBy,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,Comment_SrMgr,SrMgr_Comment_Date," +
                "(case when (Comment_JrMgr is null || Comment_JrMgr = '')then 'Pending Reviewer' when (Comment_SrMgr is null || Comment_SrMgr = '') then 'Pending Approval' else 'Evaluation Completed' end) as sstatus" +
                " from t_emp_performance_eval where Performance_EvalId='" + id + "'";


                DataSet dsPerformance = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPerformance.Tables.Count > 0 && dsPerformance.Tables[0].Rows.Count > 0)
                {
                    EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                    {
                        Performance_EvalId = dsPerformance.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                        emp_id = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["emp_id"].ToString()),
                        Designation = dsPerformance.Tables[0].Rows[0]["Designation"].ToString(),
                        Dept_id = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Dept_id"].ToString()),
                        Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                        Eval_DoneBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_Desig"].ToString(),
                        Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_DeptId"].ToString()),
                        Weakness = dsPerformance.Tables[0].Rows[0]["Weakness"].ToString(),
                        Strengths = (dsPerformance.Tables[0].Rows[0]["Strengths"].ToString()),
                        Training_Reqd = dsPerformance.Tables[0].Rows[0]["Training_Reqd"].ToString(),
                        Actions_Taken = dsPerformance.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                        //Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                        //Eval_ReviewedBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_Desig"].ToString(),
                        //Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_DeptId"].ToString()),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        JrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                        Comment_JrMgr = dsPerformance.Tables[0].Rows[0]["Comment_JrMgr"].ToString(),
                        SrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                        Comment_SrMgr = dsPerformance.Tables[0].Rows[0]["Comment_SrMgr"].ToString(),
                        sstatus = dsPerformance.Tables[0].Rows[0]["sstatus"].ToString(),

                    };

                    DateTime dtValue;
                    if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dtValue))
                    {
                        objPerformance.Evaluation_DoneOn = dtValue;
                    }
                    if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_From"].ToString(), out dtValue))
                    {
                        objPerformance.Evaluated_From = dtValue;
                    }
                    if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_To"].ToString(), out dtValue))
                    {
                        objPerformance.Evaluated_To = dtValue;
                    }
                    if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString(), out dtValue))
                    {
                        objPerformance.JrMgr_Comment_Date = dtValue;
                    }
                    if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["SrMgr_Comment_Date"].ToString(), out dtValue))
                    {
                        objPerformance.SrMgr_Comment_Date = dtValue;
                    }

                    sSqlstmt = "SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section,item_fulldesc as Section_Weightage FROM t_emp_performance_elements t, " +
                             "t_emp_performance_eval_questions tt,dropdownitems ttt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId" +
                             " and tt.Section = ttt.item_id order by Section asc, Performance_Id asc";
                    DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                    string sSqlstmt1 = "select max(Weightage) as Weightage  from t_emp_performance_eval_rating where active=1";
                    ViewBag.MaxRate = objGlobaldata.Getdetails(sSqlstmt1);

                    EmpPerformanceElementsModelsList objEmpPerformanceEvalList = new EmpPerformanceElementsModelsList();
                    objEmpPerformanceEvalList.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                    EmpPerformanceElementsModels objElementMdl = new EmpPerformanceElementsModels();

                    for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                    {
                        EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels
                        {
                            SQId = objElementMdl.GetQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                            SQ_OptionsId = objElementMdl.GetRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                            SQ_Weightage = objElementMdl.GetRatingWeightageById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString()),
                            Section =(dsPerformanceElement.Tables[0].Rows[i]["Section"].ToString()),
                            Section_Weightage = (dsPerformanceElement.Tables[0].Rows[i]["Section_Weightage"].ToString()),

                        };
                        objEmpPerformanceEvalList.lstEmpPerformanceElements.Add(objElements);
                    }

                    ViewBag.PerformanceElement = objEmpPerformanceEvalList;
                    ViewBag.PeformanceSection = objElementMdl.GetSectionsList();

                    return View(objPerformance);

                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("EmployeePerformanceEvalList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }
        
        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalPDF(FormCollection form)
        {

            try
            {
                string sPerformance_EvalId = form["Performance_EvalId"];

                if (sPerformance_EvalId != null && sPerformance_EvalId != "")
                {                 

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,Comment_SrMgr,SrMgr_Comment_Date," +
                    "(case when (Comment_JrMgr is null || Comment_JrMgr = '')then 'Pending Reviewer' when (Comment_SrMgr is null || Comment_SrMgr = '') then 'Pending Approval' else 'Evaluation Completed' end) as sstatus" +
                    " from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";


                    DataSet dsPerformance = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPerformance.Tables.Count > 0 && dsPerformance.Tables[0].Rows.Count > 0)
                    {
                        EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsPerformance.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["emp_id"].ToString()),
                            Designation = dsPerformance.Tables[0].Rows[0]["Designation"].ToString(),
                            Dept_id = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsPerformance.Tables[0].Rows[0]["Weakness"].ToString(),
                            Strengths = (dsPerformance.Tables[0].Rows[0]["Strengths"].ToString()),
                            Training_Reqd = dsPerformance.Tables[0].Rows[0]["Training_Reqd"].ToString(),
                            Actions_Taken = dsPerformance.Tables[0].Rows[0]["Actions_Taken"].ToString(),                            
                            DocUploadPath = dsPerformance.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            //Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            //Eval_ReviewedBy_Desig = dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_Desig"].ToString(),
                            //Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptNameById(dsPerformance.Tables[0].Rows[0]["Eval_ReviewedBy_DeptId"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            JrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            Comment_JrMgr = dsPerformance.Tables[0].Rows[0]["Comment_JrMgr"].ToString(),
                            SrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            Comment_SrMgr = dsPerformance.Tables[0].Rows[0]["Comment_SrMgr"].ToString(),
                            sstatus = dsPerformance.Tables[0].Rows[0]["sstatus"].ToString(),

                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluation_DoneOn = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_From"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_From = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["Evaluated_To"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_To = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.JrMgr_Comment_Date = dtValue;
                        }
                        if (DateTime.TryParse(dsPerformance.Tables[0].Rows[0]["SrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.SrMgr_Comment_Date = dtValue;
                        }
                        CompanyModels objCompany = new CompanyModels();
                        dsPerformance = objCompany.GetCompanyDetailsForReport(dsPerformance);

                        string loggedby = objGlobaldata.GetCurrentUserSession().empid;
                        dsPerformance = objGlobaldata.GetReportDetails(dsPerformance, objPerformance.Performance_EvalId, loggedby, "EMPLOYEE PERFORMANCE EVALUATION REPORT");
                        ViewBag.CompanyInfo = dsPerformance;
                        //sSqlstmt = "SELECT Performance_Id, Performance_EvalId, SQId, SQ_OptionsId FROM t_emp_performance_elements where Performance_EvalId='"
                        //    + objPerformance.Performance_EvalId + "'";
                        //DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);
                        //sSqlstmt = " SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section FROM t_emp_performance_elements t," +
                        //  " t_emp_performance_eval_questions tt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId order by Section asc, Performance_Id asc";


                        string sSqlstmt1= "select max(Weightage) as Weightage  from t_emp_performance_eval_rating where active=1";
                        ViewBag.MaxRate = objGlobaldata.Getdetails(sSqlstmt1);

                        sSqlstmt = "SELECT Performance_Id, Performance_EvalId, tt.SQId, t.SQ_OptionsId,Section,item_fulldesc as Section_Weightage,tttt.Weightage " +
                            "FROM t_emp_performance_elements t, t_emp_performance_eval_questions tt,dropdownitems ttt, t_emp_performance_eval_rating tttt " +
                            "where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId and tt.Section = ttt.item_id and t.SQ_OptionsId = tttt.SQ_OptionsId order by Section asc, Performance_Id asc";

                        ViewBag.PerformanceElement= objGlobaldata.Getdetails(sSqlstmt);
                        
                        ViewBag.EmpEVal=objPerformance;

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeePerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("EmployeePerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            //string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\" --margin-bottom \"18\" ";

            return new ViewAsPdf("EmpEvaluationPDF")
            {
                FileName = "EmpEvaluationPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }
        //
        // GET: /EmployeePerformanceEval/EmployeePerformanceEvalEdit

        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalEdit()
        {

            try
            {
                if (Request.QueryString["Performance_EvalId"] != null && Request.QueryString["Performance_EvalId"] != "")
                {
                    string sPerformance_EvalId = Request.QueryString["Performance_EvalId"];

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";


                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsCustomer.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                            emp_id = (dsCustomer.Tables[0].Rows[0]["emp_id"].ToString()),
                            Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                            Dept_id = (dsCustomer.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Eval_DoneBy = (dsCustomer.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[0]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[0]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsCustomer.Tables[0].Rows[0]["Weakness"].ToString(),
                            Strengths = (dsCustomer.Tables[0].Rows[0]["Strengths"].ToString()),
                            Training_Reqd = dsCustomer.Tables[0].Rows[0]["Training_Reqd"].ToString(),
                            Actions_Taken = dsCustomer.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            Eval_ReviewedBy = (dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            Eval_ReviewedBy_Desig = dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy_Desig"].ToString(),
                            Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy_DeptId"].ToString()),
                            LoggedBy = (dsCustomer.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString()
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluation_DoneOn = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["Evaluated_From"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_From = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["Evaluated_To"].ToString(), out dtValue))
                        {
                            objPerformance.Evaluated_To = dtValue;
                        }

                        sSqlstmt = "SELECT Performance_Id, Performance_EvalId, SQId, SQ_OptionsId FROM t_emp_performance_elements where Performance_EvalId='"
                             + objPerformance.Performance_EvalId + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        EmpPerformanceElementsModelsList objEmpPerformanceEvalList = new EmpPerformanceElementsModelsList();
                        objEmpPerformanceEvalList.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        if (dsPerformanceElement.Tables.Count > 0 && dsPerformanceElement.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            ViewBag.dsElements = dsPerformanceElement;
                        }
                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels
                            {
                                Performance_Id = dsPerformanceElement.Tables[0].Rows[i]["Performance_Id"].ToString(),
                                SQId = (dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = (dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objEmpPerformanceEvalList.lstEmpPerformanceElements.Add(objElements);
                        }
                    }
                        ViewBag.PerformanceElement = objEmpPerformanceEvalList;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.DeptHeadList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmpEvaluatedByList();
                        ViewBag.EmpHead = objGlobaldata.GetDeptHeadList();

                        EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                        ViewBag.PerformanceQuestions = objElement.GetQuestionListBox();
                        ViewBag.PerformanceRating = objElement.PerformanceRating();
                        ViewBag.PeformanceSection = objElement.GetSectionsList();

                        return View(objPerformance);

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeePerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("EmployeePerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        // POST: /EmployeePerformanceEval/EmployeePerformanceEvalEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeePerformanceEvalEdit(EmpPerformanceEvalModels objEmpPerformanceEval, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {

                objEmpPerformanceEval.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;

                objEmpPerformanceEval.Dept_id = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.emp_id);
                objEmpPerformanceEval.Designation = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.emp_id);

                objEmpPerformanceEval.Eval_DoneBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.Eval_DoneBy);
                objEmpPerformanceEval.Eval_DoneBy_Desig = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.Eval_DoneBy);

                objEmpPerformanceEval.Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objEmpPerformanceEval.Eval_ReviewedBy);
                objEmpPerformanceEval.Eval_ReviewedBy_Desig = objGlobaldata.GetEmpDesignationByHrEmpId(objEmpPerformanceEval.Eval_ReviewedBy);

                DateTime dateValue;
                if (DateTime.TryParse(form["Evaluation_DoneOn"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluation_DoneOn = dateValue;
                }
                if (DateTime.TryParse(form["Evaluated_From"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluated_From = dateValue;
                }

                if (DateTime.TryParse(form["Evaluated_To"], out dateValue) == true)
                {
                    objEmpPerformanceEval.Evaluated_To = dateValue;
                }

                if (fileUploader != null && fileUploader.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Employee"), Path.GetFileName(fileUploader.FileName));
                        string sFilename = "Perf" + "_" + objEmpPerformanceEval.emp_id + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
                        objEmpPerformanceEval.DocUploadPath = "~/DataUpload/MgmtDocs/Employee/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalEdit: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                EmpPerformanceElementsModelsList objEmpPerformanceElements = new EmpPerformanceElementsModelsList();
                objEmpPerformanceElements.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                MultiSelectList PerformanceQuestions = objElement.GetQuestionListBox();

                foreach (var item in PerformanceQuestions)
                {
                    EmpPerformanceElementsModels objElements = new EmpPerformanceElementsModels
                    {
                        SQId = form["Question " + item.Value],
                        SQ_OptionsId = form["SQ_OptionsId " + item.Value]
                    };

                    objEmpPerformanceElements.lstEmpPerformanceElements.Add(objElements);
                }
                if (objEmpPerformanceElements.lstEmpPerformanceElements.Count > 0)
                {
                    objEmpPerformanceElements.lstEmpPerformanceElements[0].Performance_EvalId = objEmpPerformanceEval.Performance_EvalId;
                }

                if (objEmpPerformanceEval.FunUpdateEmpPerformanceEvaluation(objEmpPerformanceEval, objEmpPerformanceElements))
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
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalMgrEdit()
        {
            try
            {
                if (Request.QueryString["Performance_EvalId"] != null && Request.QueryString["Performance_EvalId"] != "")
                {
                    string sPerformance_EvalId = Request.QueryString["Performance_EvalId"];

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,Comment_SrMgr,SrMgr_Comment_Date from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";
                    
                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsCustomer.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                            emp_id =objGlobaldata.GetMultiHrEmpNameById(dsCustomer.Tables[0].Rows[0]["emp_id"].ToString()),
                            //Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                            //Dept_id = (dsCustomer.Tables[0].Rows[0]["Dept_id"].ToString()),
                            //Eval_DoneBy = (dsCustomer.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
                            //Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[0]["Eval_DoneBy_Desig"].ToString(),
                            //Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[0]["Eval_DoneBy_DeptId"].ToString()),
                            //Weakness = dsCustomer.Tables[0].Rows[0]["Weakness"].ToString(),
                            //Strengths = (dsCustomer.Tables[0].Rows[0]["Strengths"].ToString()),
                            //Training_Reqd = dsCustomer.Tables[0].Rows[0]["Training_Reqd"].ToString(),
                            //Actions_Taken = dsCustomer.Tables[0].Rows[0]["Actions_Taken"].ToString(),
                            //Eval_ReviewedBy = (dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            //Eval_ReviewedBy_Desig = dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy_Desig"].ToString(),
                            //Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[0]["Eval_ReviewedBy_DeptId"].ToString()),
                            //LoggedBy = (dsCustomer.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            //DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString()
                            JrMgr = dsCustomer.Tables[0].Rows[0]["JrMgr"].ToString(),
                            Comment_JrMgr = (dsCustomer.Tables[0].Rows[0]["Comment_JrMgr"].ToString()),
                            SrMgr = dsCustomer.Tables[0].Rows[0]["SrMgr"].ToString(),
                            Comment_SrMgr = (dsCustomer.Tables[0].Rows[0]["Comment_SrMgr"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.JrMgr_Comment_Date = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["SrMgr_Comment_Date"].ToString(), out dtValue))
                        {
                            objPerformance.SrMgr_Comment_Date = dtValue;
                        } 
                        return View(objPerformance);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeePerformanceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("EmployeePerformanceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalMgrEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeePerformanceEvalMgrEdit(EmpPerformanceEvalModels objEmpPerformanceEval, FormCollection form)
        {
            try
            {                    
                if (objEmpPerformanceEval.FunUpdateEmpPerformanceComments(objEmpPerformanceEval))
                {
                    if(objEmpPerformanceEval.Comment_SrMgr == null)
                    {
                        EmpPerformanceEvalModels objMdls = new EmpPerformanceEvalModels();
                        objMdls.SendSrMgrPerpEmail(objEmpPerformanceEval.Performance_EvalId, "Employee Performace Evaluation");
                    }
                    TempData["Successdata"] = "Manager Comments of Employee Performance Updated successfully and email sent to Sr.Manager";

                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalMgrEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        //Performance Rating

        public ActionResult AddPerformanceRatings()
        {
            try
            {
                EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();

                string sql = "Select SQ_OptionsId,RatingOptions,Weightage from t_emp_performance_eval_rating where Active=1";
                ViewBag.dsPerformance = objGlobaldata.Getdetails(sql);                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformanceRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerformanceRatings(EmpPerformanceElementsModels objModels, FormCollection form)
        {
           // EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
           try
            {                

                if (objModels.FunAddPerformanceRatings(objModels))
                {
                    TempData["Successdata"] = "Added Ratings successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformanceRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

           // ViewBag.CategoryRatings = objQst.GetInspectionRatingListbox(objInspModels.Category);

            //return View(objModels);
            return RedirectToAction("AddPerformanceRatings");
        }

        [AllowAnonymous]
        public JsonResult PerformanceRatingUpdate(string SQ_OptionsId, string RatingOptions, string Weightage)
        {
            EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
            objModel.SQ_OptionsId = SQ_OptionsId;
            objModel.RatingOptions = RatingOptions;
            objModel.Weightage = Weightage;
            try
            {
                if (objModel.FunUpdatePerformanceRating(objModel))
                {
                    TempData["Successdata"] = "Updated Rating options details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformanceRatingUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult PerformanceRatingDelete(string SQ_OptionsId)
        {

            EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
            try
            {

                if (obj.FunDeletePerformaceRatings(SQ_OptionsId))
                {
                    TempData["Successdata"] = "Rating deleted successfully";                    
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformanceRatingDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Failed");
        }

        //Performance Questions
       
        public ActionResult AddPerformanceQuestions(string sSection, string branch_name)
        {
            try
            {
                EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
                ViewBag.Section = objGlobaldata.GetDropdownList("Emp Performace Evalutaion Section");
               
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "" && sSection != null && sSection != "")
                {
                    ViewBag.Branch_name = branch_name;                   
                    ViewBag.Section_Id = sSection;
                    ViewBag.Questions = obj.GetQuestionWithSectionList(sSection, branch_name);
                }
                else if (branch_name != null && branch_name != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.Questions = obj.GetQuestionList(branch_name);
                }
                else 
                {
                    ViewBag.Branch_name = sBranch_name;
                    ViewBag.Questions = obj.GetQuestionList(sBranch_name);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformanceQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        public JsonResult AddPerformanceQuestionsSearch(string sSection, string branch_name)
        {
            try
            {
                EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
                ViewBag.Section = objGlobaldata.GetDropdownList("Emp Performace Evalutaion Section");

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "" && sSection != null && sSection != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.Section_Id = sSection;
                    ViewBag.Questions = obj.GetQuestionWithSectionList(sSection, branch_name);
                }
                else if (branch_name != null && branch_name != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.Questions = obj.GetQuestionList(branch_name);
                }
                else
                {
                    ViewBag.Branch_name = sBranch_name;
                    ViewBag.Questions = obj.GetQuestionList(sBranch_name);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformanceQuestionsSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPerformanceQuestions(EmpPerformanceElementsModels objInspModels, FormCollection form)
        {

            EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
           // ViewBag.Section_Id = objInspModels.Section;
            ViewBag.branch = form["branch"];
            try
            {
                             
                string sBranch_name = ViewBag.branch;

             //  ViewBag.Section = obj.GetInspectionSectionListbox(objInspModels.Category, sBranch_name);

                if (obj.FunAddPerformanceQuestions(objInspModels, sBranch_name))
                {
                    TempData["Successdata"] = "Added Inspection Questions successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPerformanceQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            //  ViewBag.CategoryQuestions = objQst.GetInspectionQuestionsListbox(objInspModels.Category);

            // return View(objInspModels);
            return RedirectToAction("AddPerformanceQuestions",new{sSection= objInspModels.Section , branch_name = ViewBag.branch });
        }

        [HttpPost]
        public JsonResult GetPerformanceQuestions(string Section, string branch)
        {
            EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
            var data = new object();           
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                if (branch != "" && branch != null)
                {
                    data = objModel.GetQuestionWithSectionList(Section, branch);                    
                }
                else
                {
                    data = objModel.GetQuestionWithSectionList(Section, sBranch_name);
                }
               
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetPerformanceQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(data);
        }

        [AllowAnonymous]
        public JsonResult PerformanceQuestionsUpdate(string SQId, string Questions)
        {
            EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
            objModel.SQId = SQId;
            objModel.Questions = Questions;     
            try
            {
                if (objModel.FunUpdatePerformanceQuestions(objModel))
                {
                    TempData["Successdata"] = "Updated Question successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformanceQuestionsUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public JsonResult PerformanceQuestionsDelete(string SQId)
        {

            EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();
            try
            {

                if (obj.FunDeletePerformaceQuestions(SQId))
                {
                    TempData["Successdata"] = "Questions deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformanceQuestionsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

    }
}
