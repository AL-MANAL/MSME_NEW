using ISOStd.Filters;
using ISOStd.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmployeePerformanceEvalController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

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
        public ActionResult AddEmployeePerformanceEval(string eval_period, string eval_category,string emp_id,string Evaluation_DoneOn, string Evaluated_From, string Evaluated_To,string Eval_DoneBy, string Date_of_join)
        {
            EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
            EmpPerformanceEvalModels obj = new EmpPerformanceEvalModels();

            try
            {
             
                string sbranch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.PeformanceQuestions = objModel.GetQuestionListBox(eval_period, eval_category);
                ViewBag.PeformanceRating = objModel.PerformanceRating();
                ViewBag.PeformanceSection = objModel.GetSectionsList();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.DeptHeadList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.EmpHead = objGlobaldata.GetDeptHeadList();
                ViewBag.Citicality = objGlobaldata.GetDropdownList("Training Criticality");
                ViewBag.TrainingNeed = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.NotifiedEmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotifiedToArray = objGlobaldata.GetHRDeptEmployees().Split(',');
                ViewBag.Recommendation = objGlobaldata.GetDropdownList("Employee Performance Recommendation");
                ViewBag.eval_period = objGlobaldata.GetDropdownList("Emp Performance Evaluation");
                ViewBag.eval_category = objGlobaldata.GetDropdownList("Emp Performance Category");
                ViewBag.eval_period_Id = eval_period;
                ViewBag.eval_category_Id = eval_category;
                ViewBag.Topic = objGlobaldata.GetTrainingTopicList();
                obj.emp_id = emp_id;
                obj.Eval_DoneBy = Eval_DoneBy;
                DateTime dtValue;
                if (DateTime.TryParse(Evaluation_DoneOn, out dtValue))
                {
                    obj.Evaluation_DoneOn = dtValue;
                }
                if (DateTime.TryParse(Evaluated_From, out dtValue))
                {
                    obj.Evaluated_From = dtValue;
                }
                if (DateTime.TryParse(Evaluated_To, out dtValue))
                {
                    obj.Evaluated_To = dtValue;
                }
                if (DateTime.TryParse(Date_of_join, out dtValue))
                {
                    obj.Date_of_join = dtValue;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeePerformanceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(obj);
        }


       
        // POST: /EmployeePerformanceEval/AddEmployeePerformanceEval

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeePerformanceEval(EmpPerformanceEvalModels objEmpPerformanceEval, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {
                EmpPerformanceEvalModelsList objModelList = new EmpPerformanceEvalModelsList();
                objModelList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmpPerformanceEvalModels objModels = new EmpPerformanceEvalModels();

                    if (form["training_topic " + i] != null && form["training_topic " + i] != "")
                    {
                        objModels.training_topic = form["training_topic " + i];
                        objModels.criticality = form["criticality " + i];
                        objModelList.lstEmpPerformanceEvalModels.Add(objModels);
                    }
                }
                //notified to
                for (int i = 0; i < Convert.ToInt16(form["notified_cnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to + "," + form["empno " + i];
                    }
                }
                if (objEmpPerformanceEval.notified_to != null)
                {
                    objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to.Trim(',');
                }

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

                MultiSelectList PeformanceQuestions = objElements.GetQuestionListBox(objEmpPerformanceEval.eval_period, objEmpPerformanceEval.eval_category);

                foreach (var item in PeformanceQuestions)
                {
                    objElements = new EmpPerformanceElementsModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = form["SQ_OptionsId " + item.Value];

                    objEmpPerformanceElements.lstEmpPerformanceElements.Add(objElements);
                }

                if (objEmpPerformanceEval.FunAddEmpPerformanceEvaluation(objEmpPerformanceEval, objEmpPerformanceElements, objModelList))
                {
                    EmpPerformanceEvalModels objMdls = new EmpPerformanceEvalModels();
                    //objMdls.SendJrMgrPerpEmail("Employee Performace Evaluation");
                    TempData["Successdata"] = "Added Performance details successfully";
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

                string sSqlstmt = "select  Performance_EvalId,emp_id,Designation,Dept_id,Evaluation_DoneOn,Evaluated_From,Evaluated_To,Eval_DoneBy," +
                    "Eval_DoneBy_Desig,Eval_DoneBy_DeptId,Weakness,Strengths,Training_Reqd,Actions_Taken,LoggedBy,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr," +
                    "Comment_SrMgr,SrMgr_Comment_Date,Eval_ReviewedBy,DocUploadPath," +
                      "(case when eval_status='0' then 'Pending for Manger update' when eval_status='1' then 'Pending for HR update'  when eval_status='2' then 'Pending for Top Mgmt update' else 'Evaluation Completed'  end) as eval_status" +
                    "  from t_emp_performance_eval where active = 1 ";
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Performance_EvalId desc";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                //DataSet dsCustomer;
                //if (branch_name != null && branch_name != "")
                //{
                //    dsCustomer = objGlobaldata.GetDecendants(empid, branch_name);
                //}
                //else
                //{
                //    dsCustomer = objGlobaldata.GetDecendants(empid, sBranch_name);
                //}

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
                            Eval_DoneById = (dsCustomer.Tables[0].Rows[i]["Eval_DoneBy"].ToString()),
                            Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_Desig"].ToString(),
                            Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_DeptId"].ToString()),
                            Weakness = dsCustomer.Tables[0].Rows[i]["Weakness"].ToString(),
                            Strengths = (dsCustomer.Tables[0].Rows[i]["Strengths"].ToString()),
                            Training_Reqd = dsCustomer.Tables[0].Rows[i]["Training_Reqd"].ToString(),
                            Actions_Taken = dsCustomer.Tables[0].Rows[i]["Actions_Taken"].ToString(),
                            Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
                            LoggedBy = (dsCustomer.Tables[0].Rows[i]["LoggedBy"].ToString()),
                            DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                            JrMgr = dsCustomer.Tables[0].Rows[i]["JrMgr"].ToString(),
                            Comment_JrMgr = dsCustomer.Tables[0].Rows[i]["Comment_JrMgr"].ToString(),
                            SrMgr = dsCustomer.Tables[0].Rows[i]["SrMgr"].ToString(),
                            Comment_SrMgr = dsCustomer.Tables[0].Rows[i]["Comment_SrMgr"].ToString(),
                            //sstatus = dsCustomer.Tables[0].Rows[i]["sstatus"].ToString(),
                            eval_status = dsCustomer.Tables[0].Rows[i]["eval_status"].ToString(),
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

        //[AllowAnonymous]
        //public JsonResult EmployeePerformanceEvalListSearch(string SearchText, string PlanfromDate, string PlanToDate, string chkAll, int? page, string branch_name)
        //{
        //    EmpPerformanceEvalModelsList objEmpPerformanceEvalList = new EmpPerformanceEvalModelsList();
        //    objEmpPerformanceEvalList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

        //    try
        //    {
        //        string empid = (objGlobaldata.GetCurrentUserSession().empid);
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
        //        DataSet dsCustomer;
        //        if (branch_name != null && branch_name != "")
        //        {
        //            dsCustomer = objGlobaldata.GetDecendants(empid, branch_name);
        //        }
        //        else
        //        {
        //            dsCustomer = objGlobaldata.GetDecendants(empid, sBranch_name);
        //        }

        //        for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
        //        {
        //            try
        //            {
        //                EmpPerformanceEvalModels objCustomer = new EmpPerformanceEvalModels
        //                {
        //                    Performance_EvalId = dsCustomer.Tables[0].Rows[i]["Performance_EvalId"].ToString(),
        //                    emp_id = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["emp_id"].ToString()),
        //                    Designation = dsCustomer.Tables[0].Rows[i]["Designation"].ToString(),
        //                    Dept_id = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Dept_id"].ToString()),
        //                    Eval_DoneBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy"].ToString()),
        //                    Eval_DoneBy_Desig = dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_Desig"].ToString(),
        //                    Eval_DoneBy_DeptId = objGlobaldata.GetDeptNameById(dsCustomer.Tables[0].Rows[i]["Eval_DoneBy_DeptId"].ToString()),
        //                    Weakness = dsCustomer.Tables[0].Rows[i]["Weakness"].ToString(),
        //                    Strengths = (dsCustomer.Tables[0].Rows[i]["Strengths"].ToString()),
        //                    Training_Reqd = dsCustomer.Tables[0].Rows[i]["Training_Reqd"].ToString(),
        //                    Actions_Taken = dsCustomer.Tables[0].Rows[i]["Actions_Taken"].ToString(),
        //                    Eval_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
        //                    LoggedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["LoggedBy"].ToString()),
        //                    DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
        //                    JrMgr = dsCustomer.Tables[0].Rows[i]["JrMgr"].ToString(),
        //                    Comment_JrMgr = dsCustomer.Tables[0].Rows[i]["Comment_JrMgr"].ToString(),
        //                    SrMgr = dsCustomer.Tables[0].Rows[i]["SrMgr"].ToString(),
        //                    Comment_SrMgr = dsCustomer.Tables[0].Rows[i]["Comment_SrMgr"].ToString(),
        //                    sstatus = dsCustomer.Tables[0].Rows[i]["sstatus"].ToString(),
        //                };

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluation_DoneOn"].ToString(), out dtValue))
        //                {
        //                    objCustomer.Evaluation_DoneOn = dtValue;
        //                }
        //                if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_From"].ToString(), out dtValue))
        //                {
        //                    objCustomer.Evaluated_From = dtValue;
        //                }
        //                if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["Evaluated_To"].ToString(), out dtValue))
        //                {
        //                    objCustomer.Evaluated_To = dtValue;
        //                }
        //                objEmpPerformanceEvalList.lstEmpPerformanceEvalModels.Add(objCustomer);
        //            }
        //            catch (Exception ex)
        //            {
        //                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalListSearch: " + ex.ToString());
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //       // EmpPerformanceElementsModels objSurvey = new EmpPerformanceElementsModels();

        //    //    ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName("Employee Performance");
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Success");
        //}

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
                    "training_need,remarks,recommendation,notified_to,eval_period,eval_category,top_mgmt," +
                    "(case when eval_status='0' then 'Pending for Manger update' when eval_status='1' then 'Pending for HR update'  when eval_status='2' then 'Pending for Top Mgmt update' else 'Evaluation Completed'  end) as eval_status"+
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
                            LoggedBy = (dsPerformance.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            DocUploadPath = dsPerformance.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            JrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            JrMgrId = (dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            Comment_JrMgr = dsPerformance.Tables[0].Rows[0]["Comment_JrMgr"].ToString(),
                            SrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            SrMgrId = (dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            Comment_SrMgr = dsPerformance.Tables[0].Rows[0]["Comment_SrMgr"].ToString(),
                            //sstatus = dsPerformance.Tables[0].Rows[0]["sstatus"].ToString(),
                            training_need = dsPerformance.Tables[0].Rows[0]["training_need"].ToString(),
                            remarks = dsPerformance.Tables[0].Rows[0]["remarks"].ToString(),
                            recommendation = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["recommendation"].ToString()),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsPerformance.Tables[0].Rows[0]["notified_to"].ToString()),
                            eval_period = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["eval_period"].ToString()),
                            eval_category = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["eval_category"].ToString()),
                            eval_status = dsPerformance.Tables[0].Rows[0]["eval_status"].ToString(),
                            top_mgmt = dsPerformance.Tables[0].Rows[0]["top_mgmt"].ToString(),
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
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        //sSqlstmt = "SELECT Performance_Id, Performance_EvalId, SQId, SQ_OptionsId FROM t_emp_performance_elements where Performance_EvalId='"
                        //    + objPerformance.Performance_EvalId + "'";

                        //sSqlstmt = " SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section FROM t_emp_performance_elements t," +
                        //    " t_emp_performance_eval_questions tt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId order by Section asc, Performance_Id asc";

                        //training
                        sSqlstmt = "select id_training,Performance_EvalId,training_topic,criticality from t_emp_performance_training where Performance_EvalId='" + objPerformance.Performance_EvalId + "'";
                        ViewBag.dsList = objGlobaldata.Getdetails(sSqlstmt);

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

                        string sql = "select SQ_OptionsId,count(Performance_Id) tot_quest from t_emp_performance_elements  where Performance_EvalId='" + sPerformance_EvalId + "' group by SQ_OptionsId";
                        ViewBag.dsData = objGlobaldata.Getdetails(sql);

                        objPerformance.Weightage = objElementMdl.GetMaxRatingWeightage();

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
                    "training_need,remarks,recommendation,notified_to,eval_period,eval_category,top_mgmt," +
                    "(case when eval_status='0' then 'Pending for Manger update' when eval_status='1' then 'Pending for HR update'  when eval_status='2' then 'Pending for Top Mgmt update' else 'Evaluation Completed'  end) as eval_status" +
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
                            LoggedBy = (dsPerformance.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            DocUploadPath = dsPerformance.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            JrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            JrMgrId = (dsPerformance.Tables[0].Rows[0]["JrMgr"].ToString()),
                            Comment_JrMgr = dsPerformance.Tables[0].Rows[0]["Comment_JrMgr"].ToString(),
                            SrMgr = objGlobaldata.GetEmpHrNameById(dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            SrMgrId = (dsPerformance.Tables[0].Rows[0]["SrMgr"].ToString()),
                            Comment_SrMgr = dsPerformance.Tables[0].Rows[0]["Comment_SrMgr"].ToString(),
                            //sstatus = dsPerformance.Tables[0].Rows[0]["sstatus"].ToString(),
                            training_need = dsPerformance.Tables[0].Rows[0]["training_need"].ToString(),
                            remarks = dsPerformance.Tables[0].Rows[0]["remarks"].ToString(),
                            recommendation = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["recommendation"].ToString()),
                            notified_to = objGlobaldata.GetMultiHrEmpNameById(dsPerformance.Tables[0].Rows[0]["notified_to"].ToString()),
                            eval_period = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["eval_period"].ToString()),
                            eval_category = objGlobaldata.GetDropdownitemById(dsPerformance.Tables[0].Rows[0]["eval_category"].ToString()),
                            eval_status = dsPerformance.Tables[0].Rows[0]["eval_status"].ToString(),
                            top_mgmt = dsPerformance.Tables[0].Rows[0]["top_mgmt"].ToString(),
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
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        //sSqlstmt = "SELECT Performance_Id, Performance_EvalId, SQId, SQ_OptionsId FROM t_emp_performance_elements where Performance_EvalId='"
                        //    + objPerformance.Performance_EvalId + "'";

                        //sSqlstmt = " SELECT Performance_Id, Performance_EvalId, tt.SQId, SQ_OptionsId,Section FROM t_emp_performance_elements t," +
                        //    " t_emp_performance_eval_questions tt where Performance_EvalId = '" + objPerformance.Performance_EvalId + "' and t.SQId = tt.SQId order by Section asc, Performance_Id asc";

                        //training
                        sSqlstmt = "select id_training,Performance_EvalId,training_topic,criticality from t_emp_performance_training where Performance_EvalId='" + objPerformance.Performance_EvalId + "'";
                        ViewBag.dsList = objGlobaldata.Getdetails(sSqlstmt);

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

                        string sql = "select SQ_OptionsId,count(Performance_Id) tot_quest from t_emp_performance_elements  where Performance_EvalId='" + sPerformance_EvalId + "' group by SQ_OptionsId";
                        ViewBag.dsData = objGlobaldata.Getdetails(sql);

                        objPerformance.Weightage = objElementMdl.GetMaxRatingWeightage();

                        string loggedby = objGlobaldata.GetCurrentUserSession().empid;

                        dsPerformance = objGlobaldata.GetReportDetails(dsPerformance, objPerformance.Performance_EvalId, loggedby, "EMPLOYEE PERFORMANCE EVALUATION REPORT");
                        ViewBag.CompanyInfo = dsPerformance;

                        ViewBag.EmpEVal = objPerformance;
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
                //FileName = "EmpEvaluationPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        //
        // GET: /EmployeePerformanceEval/EmployeePerformanceEvalEdit

        [AllowAnonymous]
        public ActionResult EmployeePerformanceEvalEdit(string eval_period, string eval_category,string Performance_EvalId, string emp_id, string Evaluation_DoneOn, string Evaluated_From, string Evaluated_To, string Eval_DoneBy, string Date_of_join)
        {
            try
            {
                if (Request.QueryString["Performance_EvalId"] != null && Request.QueryString["Performance_EvalId"] != "")
                {
                    string sPerformance_EvalId = Request.QueryString["Performance_EvalId"];

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath,training_need,remarks,recommendation,notified_to,eval_period,eval_category from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";

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
                            DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            training_need = dsCustomer.Tables[0].Rows[0]["training_need"].ToString(),
                            remarks = dsCustomer.Tables[0].Rows[0]["remarks"].ToString(),
                            recommendation = dsCustomer.Tables[0].Rows[0]["recommendation"].ToString(),
                            eval_period = dsCustomer.Tables[0].Rows[0]["eval_period"].ToString(),
                             eval_category = dsCustomer.Tables[0].Rows[0]["eval_category"].ToString()
                        };
                        if (dsCustomer.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.NotifiedToArray = (dsCustomer.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }

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
                        ViewBag.Citicality = objGlobaldata.GetDropdownList("Training Criticality");
                        ViewBag.TrainingNeed = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.NotifiedEmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.eval_period = objGlobaldata.GetDropdownList("Emp Performance Evaluation");
                        ViewBag.eval_category = objGlobaldata.GetDropdownList("Emp Performance Category");
                        ViewBag.Recommendation = objGlobaldata.GetDropdownList("Employee Performance Recommendation");
                        ViewBag.Topic = objGlobaldata.GetTrainingTopicList();
                        //training topic
                        EmpPerformanceEvalModelsList objModelList = new EmpPerformanceEvalModelsList();
                        objModelList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

                        sSqlstmt = "select id_training,Performance_EvalId,training_topic,criticality from t_emp_performance_training where Performance_EvalId='" + objPerformance.Performance_EvalId + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmpPerformanceEvalModels objModel = new EmpPerformanceEvalModels
                                    {
                                        id_training = dsList.Tables[0].Rows[i]["id_training"].ToString(),
                                        Performance_EvalId = dsList.Tables[0].Rows[i]["Performance_EvalId"].ToString(),
                                        training_topic = dsList.Tables[0].Rows[i]["training_topic"].ToString(),
                                        criticality = dsList.Tables[0].Rows[i]["criticality"].ToString(),
                                    };
                                    objModelList.lstEmpPerformanceEvalModels.Add(objModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeePerformanceEvalList");
                                }
                            }
                            ViewBag.objList = objModelList;
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

                        EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                        ViewBag.PerformanceQuestions = objElement.GetQuestionListBox(dsCustomer.Tables[0].Rows[0]["eval_period"].ToString(), dsCustomer.Tables[0].Rows[0]["eval_category"].ToString());
                        ViewBag.PerformanceRating = objElement.PerformanceRating();
                        ViewBag.PeformanceSection = objElement.GetSectionsList();

                        if (eval_period != "" && eval_category != "" && eval_period != null && eval_category != null)
                        {
                            ViewBag.eval_period_Id = eval_period;
                            ViewBag.eval_category_Id = eval_category;

                            EmpPerformanceElementsModelsList objEmpPerformanceEvalList1 = new EmpPerformanceElementsModelsList();
                            objEmpPerformanceEvalList1.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

                             dicPerformanceElements = new Dictionary<string, string>();

                            ViewBag.PerformanceElement = objEmpPerformanceEvalList1;
                            ViewBag.dicPerformanceElement = dicPerformanceElements;


                            objElement = new EmpPerformanceElementsModels();
                            ViewBag.PerformanceQuestions = objElement.GetQuestionListBox(eval_period, eval_category);
                            ViewBag.PerformanceRating = objElement.PerformanceRating();
                            ViewBag.PeformanceSection = objElement.GetSectionsList();


                            objPerformance.emp_id = emp_id;
                            objPerformance.Eval_DoneBy = Eval_DoneBy;

                           
                            if (DateTime.TryParse(Evaluation_DoneOn, out dtValue))
                            {
                                objPerformance.Evaluation_DoneOn = dtValue;
                            }
                            if (DateTime.TryParse(Evaluated_From, out dtValue))
                            {
                                objPerformance.Evaluated_From = dtValue;
                            }
                            if (DateTime.TryParse(Evaluated_To, out dtValue))
                            {
                                objPerformance.Evaluated_To = dtValue;
                            }
                            if (DateTime.TryParse(Date_of_join, out dtValue))
                            {
                                objPerformance.Date_of_join = dtValue;
                            }
                        }



                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.DeptHeadList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmpEvaluatedByList();
                        ViewBag.EmpHead = objGlobaldata.GetDeptHeadList();

                       

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
                EmpPerformanceEvalModelsList objModelList = new EmpPerformanceEvalModelsList();
                objModelList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmpPerformanceEvalModels objModels = new EmpPerformanceEvalModels();

                    if (form["training_topic " + i] != null && form["training_topic " + i] != "")
                    {
                        objModels.training_topic = form["training_topic " + i];
                        objModels.criticality = form["criticality " + i];
                        objModelList.lstEmpPerformanceEvalModels.Add(objModels);
                    }
                }
                if (objModelList.lstEmpPerformanceEvalModels.Count > 0)
                {
                    objModelList.lstEmpPerformanceEvalModels[0].Performance_EvalId = objEmpPerformanceEval.Performance_EvalId;
                }
                //notified to
                for (int i = 0; i < Convert.ToInt16(form["notified_cnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to + "," + form["empno " + i];
                    }
                }
                if (objEmpPerformanceEval.notified_to != null)
                {
                    objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to.Trim(',');
                }
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
                MultiSelectList PerformanceQuestions = objElement.GetQuestionListBox(objEmpPerformanceEval.eval_period, objEmpPerformanceEval.eval_category);

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

                if (objEmpPerformanceEval.FunUpdateEmpPerformanceEvaluation(objEmpPerformanceEval, objEmpPerformanceElements, objModelList))
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
        public ActionResult EmployeePerformanceMgrEval()
        {
            try
            {
                if (Request.QueryString["Performance_EvalId"] != null && Request.QueryString["Performance_EvalId"] != "")
                {
                    string sPerformance_EvalId = Request.QueryString["Performance_EvalId"];

                    string sSqlstmt = "select Performance_EvalId, emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, "
                    + " LoggedBy,DocUploadPath,training_need,remarks,recommendation,notified_to,eval_period,eval_category from t_emp_performance_eval where Performance_EvalId='" + sPerformance_EvalId + "'";

                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        EmpPerformanceEvalModels objPerformance = new EmpPerformanceEvalModels
                        {
                            Performance_EvalId = dsCustomer.Tables[0].Rows[0]["Performance_EvalId"].ToString(),
                            emp_id = (dsCustomer.Tables[0].Rows[0]["emp_id"].ToString()),
                            Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                            Dept_id = (dsCustomer.Tables[0].Rows[0]["Dept_id"].ToString()),
                            Eval_DoneBy = objGlobaldata.GetMultiHrEmpNameById(dsCustomer.Tables[0].Rows[0]["Eval_DoneBy"].ToString()),
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
                            DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            training_need = dsCustomer.Tables[0].Rows[0]["training_need"].ToString(),
                            remarks = dsCustomer.Tables[0].Rows[0]["remarks"].ToString(),
                            recommendation = dsCustomer.Tables[0].Rows[0]["recommendation"].ToString(),
                            eval_period =(dsCustomer.Tables[0].Rows[0]["eval_period"].ToString()),
                            eval_category = (dsCustomer.Tables[0].Rows[0]["eval_category"].ToString())
                        };
                        if (dsCustomer.Tables[0].Rows[0]["notified_to"].ToString() != "")
                        {
                            ViewBag.NotifiedToArray = (dsCustomer.Tables[0].Rows[0]["notified_to"].ToString()).Split(',');
                        }
                        else
                        {
                            ViewBag.NotifiedToArray = objGlobaldata.GetHRDeptEmployees().Split(',');
                        }

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
                        ViewBag.Citicality = objGlobaldata.GetDropdownList("Training Criticality");
                        ViewBag.TrainingNeed = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.NotifiedEmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.eval_period = objGlobaldata.GetDropdownList("Emp Performance Evaluation");
                        ViewBag.eval_category = objGlobaldata.GetDropdownList("Emp Performance Category");
                        ViewBag.Recommendation = objGlobaldata.GetDropdownList("Employee Performance Recommendation");
                        ViewBag.Topic = objGlobaldata.GetTrainingTopicList();


                        sSqlstmt = "select t1.emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,Designation,"
               + "Date_of_join,years_exp,(group_concat(distinct skill)) skill,(group_concat(distinct t2.qualification)) qualification"
               + ",(group_concat(distinct training_type)) training_type from t_hr_employee t1, t_hr_employee_qualification t2,t_hr_employee_skills t3, t_hr_employee_training t4"
               + " where t1.emp_no = t2.emp_no and t1.emp_no = t3.emp_no and t1.emp_no = t4.emp_no and t1.emp_no = '" + objPerformance.emp_id + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            if (DateTime.TryParse(dsList.Tables[0].Rows[0]["Date_of_join"].ToString(), out dtValue))
                            {
                                objPerformance.Date_of_join = dtValue;
                            }
                        }
                        //training topic
                        EmpPerformanceEvalModelsList objModelList = new EmpPerformanceEvalModelsList();
                        objModelList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

                        sSqlstmt = "select id_training,Performance_EvalId,training_topic,criticality from t_emp_performance_training where Performance_EvalId='" + objPerformance.Performance_EvalId + "'";
                         dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmpPerformanceEvalModels objModel = new EmpPerformanceEvalModels
                                    {
                                        id_training = dsList.Tables[0].Rows[i]["id_training"].ToString(),
                                        Performance_EvalId = dsList.Tables[0].Rows[i]["Performance_EvalId"].ToString(),
                                        training_topic = dsList.Tables[0].Rows[i]["training_topic"].ToString(),
                                        criticality = dsList.Tables[0].Rows[i]["criticality"].ToString(),
                                    };
                                    objModelList.lstEmpPerformanceEvalModels.Add(objModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceEvalEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("EmployeePerformanceEvalList");
                                }
                            }
                            ViewBag.objList = objModelList;
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

                        EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                        ViewBag.PerformanceQuestions = objElement.GetQuestionListBox(dsCustomer.Tables[0].Rows[0]["eval_period"].ToString(), dsCustomer.Tables[0].Rows[0]["eval_category"].ToString());
                        ViewBag.PerformanceRating = objElement.PerformanceRating();
                        ViewBag.PeformanceSection = objElement.GetSectionsList();




                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.DeptHeadList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmpEvaluatedByList();
                        ViewBag.EmpHead = objGlobaldata.GetDeptHeadList();



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
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceMgrEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeePerformanceEvalList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeePerformanceMgrEval(EmpPerformanceEvalModels objEmpPerformanceEval, FormCollection form, HttpPostedFileBase fileUploader)
        {
            try
            {
                EmpPerformanceEvalModelsList objModelList = new EmpPerformanceEvalModelsList();
                objModelList.lstEmpPerformanceEvalModels = new List<EmpPerformanceEvalModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    EmpPerformanceEvalModels objModels = new EmpPerformanceEvalModels();

                    if (form["training_topic " + i] != null && form["training_topic " + i] != "")
                    {
                        objModels.training_topic = form["training_topic " + i];
                        objModels.criticality = form["criticality " + i];
                        objModelList.lstEmpPerformanceEvalModels.Add(objModels);
                    }
                }
                if (objModelList.lstEmpPerformanceEvalModels.Count > 0)
                {
                    objModelList.lstEmpPerformanceEvalModels[0].Performance_EvalId = objEmpPerformanceEval.Performance_EvalId;
                }
                //notified to
                for (int i = 0; i < Convert.ToInt16(form["notified_cnt"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to + "," + form["empno " + i];
                    }
                }
                if (objEmpPerformanceEval.notified_to != null)
                {
                    objEmpPerformanceEval.notified_to = objEmpPerformanceEval.notified_to.Trim(',');
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
                MultiSelectList PerformanceQuestions = objElement.GetQuestionListBox(objEmpPerformanceEval.eval_period, objEmpPerformanceEval.eval_category);

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

                if (objEmpPerformanceEval.FunUpdateEmpPerfManagerEvaluation(objEmpPerformanceEval, objEmpPerformanceElements, objModelList))
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
                objGlobaldata.AddFunctionalLog("Exception in EmployeePerformanceMgrEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("Index", "Home");
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
                            emp_id = objGlobaldata.GetMultiHrEmpNameById(dsCustomer.Tables[0].Rows[0]["emp_id"].ToString()),
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
                    
                        EmpPerformanceEvalModels objMdls = new EmpPerformanceEvalModels();
                        objMdls.SendSrMgrPerpEmail(objEmpPerformanceEval.Performance_EvalId, "Employee Performace Evaluation");
                    
                    TempData["Successdata"] = "Updated Successfully";
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

            return RedirectToAction("Index", "Home");
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

        public ActionResult AddPerformanceQuestions(string sSection, string branch_name, string eval_period, string eval_category)
        {
            try
            {
                EmpPerformanceElementsModels obj = new EmpPerformanceElementsModels();

                ViewBag.Section = objGlobaldata.GetDropdownList("Emp Performace Evalutaion Section");

                ViewBag.eval_period = objGlobaldata.GetDropdownList("Emp Performance Evaluation");

                ViewBag.eval_category = objGlobaldata.GetDropdownList("Emp Performance Category");

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "" && sSection != null && sSection != "" && eval_period != null && eval_period != "" && eval_category != null && eval_category != "")
                {
                    ViewBag.Branch_name = branch_name;
                    ViewBag.Section_Id = sSection;
                    ViewBag.eval_period_Id = eval_period;
                    ViewBag.eval_category_Id = eval_category;
                    ViewBag.Questions = obj.GetQuestionWithSectionList(sSection, branch_name, eval_period, eval_category);
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

        public JsonResult AddPerformanceQuestionsSearch(string sSection, string branch_name,string eval_period,string eval_category)
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
                    ViewBag.Questions = obj.GetQuestionWithSectionList(sSection, branch_name, eval_period, eval_category);
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
            return RedirectToAction("AddPerformanceQuestions", new { sSection = objInspModels.Section, branch_name = ViewBag.branch, eval_period = objInspModels.eval_period, eval_category = objInspModels.eval_category });
        }

        [HttpPost]
        public JsonResult GetPerformanceQuestions(string Section, string branch,string eval_period,string eval_category)
        {
            EmpPerformanceElementsModels objModel = new EmpPerformanceElementsModels();
            var data = new object();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                if (branch != "" && branch != null)
                {
                    data = objModel.GetQuestionWithSectionList(Section, branch, eval_period, eval_category);
                }
                else
                {
                    data = objModel.GetQuestionWithSectionList(Section, sBranch_name, eval_period, eval_category);
                }
            }
            catch (Exception ex)
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