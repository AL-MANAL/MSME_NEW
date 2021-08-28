using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmployeeCompetenceEvalController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public EmployeeCompetenceEvalController()
        {
            ViewBag.Menutype = "Employee";
            ViewBag.SubMenutype = "EmployeeCompetenceEval";
        }

        //
        // GET: /EmployeeCompetenceEval/
          
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /EmployeeCompetenceEval/AddEmployeeCompetenceEval
         
        [AllowAnonymous]
        public ActionResult AddEmployeeCompetenceEval()
        {
            try
            {
                EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
                ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");
                // ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();//GetDepartmentList();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo"); //new string[] { "No", "Yes" };
                ViewBag.EvaluationOutput = objcomp.GetMultiEvaluationOutputList();
                ViewBag.Evaluated_Freq = objcomp.GetMultiEvaluationFrequencyList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeCompetenceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        //
        // POST: /EmployeeCompetenceEval/AddEmployeeCompetenceEval
        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeeCompetenceEval(EmployeeCompetenceEvalModels objEmployeeCompetence, FormCollection form, HttpPostedFileBase ProfilePic)
        {
            try
            {
                DateTime dateValue;

                // objEmployeeCompetence.Dept_id = objGlobaldata.GetDeptIdByHrEmpId(objEmployeeCompetence.emp_id);
                //objEmployeeCompetence.Designation = objGlobaldata.GetEmpDesignationByHrEmpId(objEmployeeCompetence.emp_id);

                if (form["Evaluation_DoneOn"] != null && DateTime.TryParse(form["Evaluation_DoneOn"], out dateValue) == true)
                {
                    objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                }


                if (objEmployeeCompetence.FunAddCompetenceEvaluation(objEmployeeCompetence))
                {
                    TempData["Successdata"] = "Added Comptency details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmployeeCompetenceEval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeCompetenceEvalList");
        }

        //
        // GET: /EmployeeCompetenceEval/EmployeeCompetenceEvalList
        
        [AllowAnonymous]
        public ActionResult EmployeeCompetenceEvalList(string SearchText, int? page, string branch_name)
        {
            EmployeeCompetenceEvalModelsList objEmployeeCompetenceList = new EmployeeCompetenceEvalModelsList();
            objEmployeeCompetenceList.lstEmployeeCompetenceEval = new List<EmployeeCompetenceEvalModels>();
            EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT CompetenceId,Name,Evaluation_DoneOn, Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                        + "Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                        + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                        + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy, LoggedBy"
                        + " FROM t_emp_competence_eval where Active=1";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_id ='" + SearchText + "' or emp_id like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Name ";
                DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCompetenceList.Tables.Count > 0)
                {      
                    for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeCompetenceEvalModels objEmployeeCompetence = new EmployeeCompetenceEvalModels
                            {
                                CompetenceId = dsCompetenceList.Tables[0].Rows[i]["CompetenceId"].ToString(),
                                Name = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["Name"].ToString()),
                                // Designation = dsCompetenceList.Tables[0].Rows[i]["Designation"].ToString(),
                                //Dept_id = dsCompetenceList.Tables[0].Rows[i]["Dept_id"].ToString(),
                                Evaluated_Freq = objcomp.GetEvaluationFrequencyNameById(dsCompetenceList.Tables[0].Rows[i]["Evaluated_Freq"].ToString()),
                                Evalaution_Done_By = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Evalaution_Done_By"].ToString()),
                                Academic_MinComp_Details = dsCompetenceList.Tables[0].Rows[i]["Academic_MinComp_Details"].ToString(),
                                Academic_EmpComp_Details = dsCompetenceList.Tables[0].Rows[i]["Academic_EmpComp_Details"].ToString(),
                                //Academic_EvalOutput = objGlobaldata.GetRolesNameById(dsCompetenceList.Tables[0].Rows[i]["Academic_EvalOutput"].ToString()),
                                Academic_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[i]["Academic_EvalOutput"].ToString()),
                                Eval_ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
                                Eval_ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Eval_ApprovedBy"].ToString())
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Evaluation_DoneOn"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                            }

                            objEmployeeCompetenceList.lstEmployeeCompetenceEval.Add(objEmployeeCompetence);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmployeeCompetenceList.lstEmployeeCompetenceEval.ToList());
        }


        [AllowAnonymous]
        public JsonResult EmployeeCompetenceEvalListSearch(string SearchText, int? page, string branch_name)
        {
            EmployeeCompetenceEvalModelsList objEmployeeCompetenceList = new EmployeeCompetenceEvalModelsList();
            objEmployeeCompetenceList.lstEmployeeCompetenceEval = new List<EmployeeCompetenceEvalModels>();
            EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT CompetenceId,Name,Evaluation_DoneOn, Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                        + "Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                        + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                        + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy, LoggedBy"
                        + " FROM t_emp_competence_eval where Active=1";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSqlstmt = sSqlstmt + " and (emp_id ='" + SearchText + "' or emp_id like '" + SearchText + "%')";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Name ";
                DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCompetenceList.Tables.Count > 0)
                {                

                    for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmployeeCompetenceEvalModels objEmployeeCompetence = new EmployeeCompetenceEvalModels
                            {
                                CompetenceId = dsCompetenceList.Tables[0].Rows[i]["CompetenceId"].ToString(),
                                Name = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["Name"].ToString()),
                                // Designation = dsCompetenceList.Tables[0].Rows[i]["Designation"].ToString(),
                                //Dept_id = dsCompetenceList.Tables[0].Rows[i]["Dept_id"].ToString(),
                                Evaluated_Freq = objcomp.GetEvaluationFrequencyNameById(dsCompetenceList.Tables[0].Rows[i]["Evaluated_Freq"].ToString()),
                                Evalaution_Done_By = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Evalaution_Done_By"].ToString()),
                                Academic_MinComp_Details = dsCompetenceList.Tables[0].Rows[i]["Academic_MinComp_Details"].ToString(),
                                Academic_EmpComp_Details = dsCompetenceList.Tables[0].Rows[i]["Academic_EmpComp_Details"].ToString(),
                                //Academic_EvalOutput = objGlobaldata.GetRolesNameById(dsCompetenceList.Tables[0].Rows[i]["Academic_EvalOutput"].ToString()),
                                Academic_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[i]["Academic_EvalOutput"].ToString()),
                                Eval_ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Eval_ReviewedBy"].ToString()),
                                Eval_ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[i]["Eval_ApprovedBy"].ToString())
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Evaluation_DoneOn"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                            }

                            objEmployeeCompetenceList.lstEmployeeCompetenceEval.Add(objEmployeeCompetence);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        //
        // GET: /EmployeeCompetenceEval/EmployeeCompetenceEvalDetails

        [AllowAnonymous]
        public ActionResult EmployeeCompetenceEvalDetails()
        {
            try
            {
                EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
                if (Request.QueryString["CompetenceId"] != null && Request.QueryString["CompetenceId"] != "")
                {
                    string sCompetenceId = Request.QueryString["CompetenceId"];
                    string sSqlstmt = "SELECT CompetenceId,Evaluation_DoneOn, Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                            + "Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                            + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                            + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy,Name"
                            + " FROM t_emp_competence_eval where CompetenceId='" + sCompetenceId + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                    {
                        EmployeeCompetenceEvalModels objEmployeeCompetence = new EmployeeCompetenceEvalModels
                        {
                            CompetenceId = dsCompetenceList.Tables[0].Rows[0]["CompetenceId"].ToString(),
                            Name = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["Name"].ToString()),
                            Evaluated_Freq = objcomp.GetEvaluationFrequencyNameById(dsCompetenceList.Tables[0].Rows[0]["Evaluated_Freq"].ToString()),
                            Evalaution_Done_By = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()),
                            Academic_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_MinComp_Details"].ToString(),
                            Academic_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_EmpComp_Details"].ToString(),
                            Academic_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Academic_EvalOutput"].ToString()),
                            YrExp_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_MinComp_Details"].ToString(),
                            YrExp_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_EmpComp_Details"].ToString(),
                            YrExp_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["YrExp_EvalOutput"].ToString()),
                            Relevant_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_MinComp_Details"].ToString(),
                            Relevant_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_EmpComp_Details"].ToString(),
                            Relevant_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Relevant_EvalOutput"].ToString()),
                            Skills_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_MinComp_Details"].ToString(),
                            Skills_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_EmpComp_Details"].ToString(),
                            Skills_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Skills_EvalOutput"].ToString()),
                            Emp_Suit_Hold_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Suit_Hold_Pos"].ToString(),
                            Emp_Prom_Nxt_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Prom_Nxt_Pos"].ToString(),
                            Need_Of_Trainings = (dsCompetenceList.Tables[0].Rows[0]["Need_Of_Trainings"].ToString()),
                            Emp_Not_Competent_Action = (dsCompetenceList.Tables[0].Rows[0]["Emp_Not_Competent_Action"].ToString()),
                            Eval_ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            Eval_ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString()),
                            //LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString())
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                        }

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeeCompetenceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Competence Id cannot be null";
                    return RedirectToAction("EmployeeCompetenceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeCompetenceEvalList");
        }

        
        [AllowAnonymous]
        public ActionResult EmployeeCompetenceEvalInfo(int id)
        {
            try
            {
                EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
               
                    string sSqlstmt = "SELECT CompetenceId,Evaluation_DoneOn, Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                            + "Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                            + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                            + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy,Name"
                            + " FROM t_emp_competence_eval where CompetenceId='" + id + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                    {
                        EmployeeCompetenceEvalModels objEmployeeCompetence = new EmployeeCompetenceEvalModels
                        {
                            CompetenceId = dsCompetenceList.Tables[0].Rows[0]["CompetenceId"].ToString(),
                            Name = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["Name"].ToString()),
                            Evaluated_Freq = objcomp.GetEvaluationFrequencyNameById(dsCompetenceList.Tables[0].Rows[0]["Evaluated_Freq"].ToString()),
                            Evalaution_Done_By = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()),
                            Academic_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_MinComp_Details"].ToString(),
                            Academic_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_EmpComp_Details"].ToString(),
                            Academic_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Academic_EvalOutput"].ToString()),
                            YrExp_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_MinComp_Details"].ToString(),
                            YrExp_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_EmpComp_Details"].ToString(),
                            YrExp_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["YrExp_EvalOutput"].ToString()),
                            Relevant_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_MinComp_Details"].ToString(),
                            Relevant_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_EmpComp_Details"].ToString(),
                            Relevant_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Relevant_EvalOutput"].ToString()),
                            Skills_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_MinComp_Details"].ToString(),
                            Skills_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_EmpComp_Details"].ToString(),
                            Skills_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Skills_EvalOutput"].ToString()),
                            Emp_Suit_Hold_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Suit_Hold_Pos"].ToString(),
                            Emp_Prom_Nxt_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Prom_Nxt_Pos"].ToString(),
                            Need_Of_Trainings = (dsCompetenceList.Tables[0].Rows[0]["Need_Of_Trainings"].ToString()),
                            Emp_Not_Competent_Action = (dsCompetenceList.Tables[0].Rows[0]["Emp_Not_Competent_Action"].ToString()),
                            Eval_ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()),
                            Eval_ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsCompetenceList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString()),
                            //LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString())
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                        }

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeeCompetenceEvalList");
                    }
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeCompetenceEvalList");
        }

        
        [AllowAnonymous]
        public JsonResult CompetenceEvalDelete(FormCollection form)
        {
            try
            {

                   
                        if (form["CompetenceId"] != null && form["CompetenceId"] != "")
                        {

                            EmployeeCompetenceEvalModels Doc = new EmployeeCompetenceEvalModels();
                            string sCompetenceId = form["CompetenceId"];


                            if (Doc.FunDeleteCompetenceDoc(sCompetenceId))
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
                            TempData["alertdata"] = "MgmtDoc Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CompetenceEvalDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        //
        // GET: /EmployeeCompetenceEval/EmployeeCompetenceEvalEdit

        
        [AllowAnonymous]
        public ActionResult EmployeeCompetenceEvalEdit()
        {
            try
            {
                EmployeeCompetenceEvalModels objcomp = new EmployeeCompetenceEvalModels();
                ViewBag.EvaluationOutput = objcomp.GetMultiEvaluationOutputList();
                ViewBag.Evaluated_Freq = objcomp.GetMultiEvaluationFrequencyList();
                if (Request.QueryString["CompetenceId"] != null && Request.QueryString["CompetenceId"] != "")
                {
                    string sCompetenceId = Request.QueryString["CompetenceId"];
                    string sSqlstmt = "SELECT CompetenceId,Name,Evaluation_DoneOn, Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                            + "Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                            + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                            + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy, LoggedBy"
                            + " FROM t_emp_competence_eval where CompetenceId='" + sCompetenceId + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                    {
                        EmployeeCompetenceEvalModels objEmployeeCompetence = new EmployeeCompetenceEvalModels
                        {
                            CompetenceId = dsCompetenceList.Tables[0].Rows[0]["CompetenceId"].ToString(),
                            Name = dsCompetenceList.Tables[0].Rows[0]["Name"].ToString(),
                            Evaluated_Freq = objcomp.GetEvaluationFrequencyNameById(dsCompetenceList.Tables[0].Rows[0]["Evaluated_Freq"].ToString()),
                            Evalaution_Done_By = dsCompetenceList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString(),
                            Academic_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_MinComp_Details"].ToString(),
                            Academic_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Academic_EmpComp_Details"].ToString(),
                            Academic_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Academic_EvalOutput"].ToString()),
                            YrExp_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_MinComp_Details"].ToString(),
                            YrExp_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["YrExp_EmpComp_Details"].ToString(),
                            YrExp_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["YrExp_EvalOutput"].ToString()),
                            Relevant_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_MinComp_Details"].ToString(),
                            Relevant_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Relevant_EmpComp_Details"].ToString(),
                            Relevant_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Relevant_EvalOutput"].ToString()),
                            Skills_MinComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_MinComp_Details"].ToString(),
                            Skills_EmpComp_Details = dsCompetenceList.Tables[0].Rows[0]["Skills_EmpComp_Details"].ToString(),
                            Skills_EvalOutput = objcomp.GetEvaluationOutputNameById(dsCompetenceList.Tables[0].Rows[0]["Skills_EvalOutput"].ToString()),
                            Emp_Suit_Hold_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Suit_Hold_Pos"].ToString(),
                            Emp_Prom_Nxt_Pos = dsCompetenceList.Tables[0].Rows[0]["Emp_Prom_Nxt_Pos"].ToString(),
                            Need_Of_Trainings = (dsCompetenceList.Tables[0].Rows[0]["Need_Of_Trainings"].ToString()),
                            Emp_Not_Competent_Action = (dsCompetenceList.Tables[0].Rows[0]["Emp_Not_Competent_Action"].ToString()),

                            Eval_ReviewedBy = dsCompetenceList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString(),
                            Eval_ApprovedBy = dsCompetenceList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString(),
                            // LoggedBy = (dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString())
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                        }

                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();//GetDepartmentList();
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo"); //new string[] { "No", "Yes" };

                        ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                        ViewBag.Gender = objGlobaldata.GetConstantValue("Gender");

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmployeeCompetenceEvalList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Competence Id cannot be null";
                    return RedirectToAction("EmployeeCompetenceEvalList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeCompetenceEvalList");
        }

        //
        // POST: /EmployeeCompetenceEval/AddEmployeeCompetenceEval
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeCompetenceEvalEdit(EmployeeCompetenceEvalModels objEmployeeCompetence, FormCollection form, HttpPostedFileBase ProfilePic)
        {
            try
            {
                DateTime dateValue;
                if (form["Evaluation_DoneOn"] != null && DateTime.TryParse(form["Evaluation_DoneOn"], out dateValue) == true)
                {
                    objEmployeeCompetence.Evaluation_DoneOn = dateValue;
                }


                if (objEmployeeCompetence.FunUpdateCompetenceEvaluation(objEmployeeCompetence))
                {
                    TempData["Successdata"] = "Updated Comptency details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmployeeCompetenceEvalList");
        }
    }
}
