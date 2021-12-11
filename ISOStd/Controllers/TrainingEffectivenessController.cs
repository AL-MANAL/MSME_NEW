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
    public class TrainingEffectivenessController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public TrainingEffectivenessController()
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            ViewBag.Menutype = "Employee";
        }

        [AllowAnonymous]
        public ActionResult AddTrainingEffectiveness()
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                SurveyModels objSurvey = new SurveyModels();
                TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();
                ViewBag.SurveyQuestions = objSurvey.GetSurveyTypeListbox("Training Effectiveness Survey");
                ViewBag.SurveyRating = objSurvey.GetSurveyRating(objSurvey.getSurveyIDByName("Training Effectiveness Survey"));
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PerfPeriod = objGlobaldata.GetDropdownList("Performance Monitoring Period");
                ViewBag.ReportNo = objModel.GetTrainingReportNoList();
                ViewBag.PlannedObjective = objGlobaldata.GetDropdownList("Training Planned Objectives");
                ViewBag.Method = objGlobaldata.GetDropdownList("Method of Effectiveness Evaluation");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingEffectiveness: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainingEffectiveness(TrainingEffectivenessModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";

                // objModel.loggedby = objGlobaldata.GetCurrentUserSession().empid;
                //objModel.na = form["N/A"];
                //objModel.auditor = objGlobaldata.GetDeptIdByHrEmpId(objModel.auditor);

                //DateTime dateValue;
                //if (DateTime.TryParse(form["evalu_date"], out dateValue) == true)
                //{
                //    objModel.evalu_date = dateValue;
                //}
                HttpPostedFileBase files = Request.Files[0];

                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "Training_Eval" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddTrainingEffectiveness-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                TrainingEffectivenessModelsList objList = new TrainingEffectivenessModelsList();
                objList.TrainList = new List<TrainingEffectivenessModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Training Effectiveness Survey");

                foreach (var item in SurveyQuestions)
                {
                    TrainingEffectivenessModels objElements = new TrainingEffectivenessModels();

                    objElements.SQId = form["Question " + item.Value];
                    objElements.SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value]);

                    objList.TrainList.Add(objElements);
                }

                if (objModel.FunAddTrainingEvaluation(objModel, objList))
                {
                    TempData["Successdata"] = "Added Training Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingEffectiveness: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingEffectivenessList");
        }

        [AllowAnonymous]
        public ActionResult TrainingEffectivenessList(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            TrainingEffectivenessModelsList ObjList = new TrainingEffectivenessModelsList();
            ObjList.TrainList = new List<TrainingEffectivenessModels>();

            TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                    "emp_perf_improved,action_taken,planned_objective,logged_by from t_trainings_evalution where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsTraining.Tables.Count > 0 && i < dsTraining.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        TrainingEffectivenessModels objTraining = new TrainingEffectivenessModels
                        {
                            id_training_evalution = dsTraining.Tables[0].Rows[i]["id_training_evalution"].ToString(),
                            report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[i]["report_no"].ToString()),
                            perf_monitor_period = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[i]["perf_monitor_period"].ToString()),
                            emp_name = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[i]["emp_name"].ToString()),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[i]["logged_by"].ToString()),
                            upload = dsTraining.Tables[0].Rows[i]["upload"].ToString(),
                            comments = (dsTraining.Tables[0].Rows[i]["comments"].ToString()),
                            emp_perf_improved = dsTraining.Tables[0].Rows[i]["emp_perf_improved"].ToString(),
                            action_taken = (dsTraining.Tables[0].Rows[i]["action_taken"].ToString()),
                            planned_objective = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[i]["planned_objective"].ToString()),
                        };

                        //DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["evalu_date"].ToString(), out dtValue))
                        //{
                        //    objSupplier.evalu_date = dtValue;
                        //}

                        ObjList.TrainList.Add(objTraining);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(ObjList.TrainList.ToList());
        }

        [AllowAnonymous]
        public JsonResult TrainingEffectivenessListSearch(string SearchText, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            TrainingEffectivenessModelsList ObjList = new TrainingEffectivenessModelsList();
            ObjList.TrainList = new List<TrainingEffectivenessModels>();

            TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                    "emp_perf_improved,action_taken,planned_objective,logged_by from t_trainings_evalution where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + "";
                DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsTraining.Tables.Count > 0 && i < dsTraining.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        TrainingEffectivenessModels objTraining = new TrainingEffectivenessModels
                        {
                            id_training_evalution = dsTraining.Tables[0].Rows[i]["id_training_evalution"].ToString(),
                            report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[i]["report_no"].ToString()),
                            perf_monitor_period = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[i]["perf_monitor_period"].ToString()),
                            emp_name = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[i]["emp_name"].ToString()),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[i]["logged_by"].ToString()),
                            upload = dsTraining.Tables[0].Rows[i]["upload"].ToString(),
                            comments = (dsTraining.Tables[0].Rows[i]["comments"].ToString()),
                            emp_perf_improved = dsTraining.Tables[0].Rows[i]["emp_perf_improved"].ToString(),
                            action_taken = (dsTraining.Tables[0].Rows[i]["action_taken"].ToString()),
                            planned_objective = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[i]["planned_objective"].ToString()),
                        };

                        //DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["evalu_date"].ToString(), out dtValue))
                        //{
                        //    objSupplier.evalu_date = dtValue;
                        //}

                        ObjList.TrainList.Add(objTraining);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult TrainingEffectivenessEdit()
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();

                if (Request.QueryString["id_training_evalution"] != null && Request.QueryString["id_training_evalution"] != "")
                {
                    string sid_training_evalution = Request.QueryString["id_training_evalution"];

                    string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                    "emp_perf_improved,action_taken,planned_objective,logged_by,method_eval,further_training from t_trainings_evalution where id_training_evalution='" + sid_training_evalution + "'";

                    DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTraining.Tables.Count > 0 && dsTraining.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingEffectivenessModels
                        {
                            id_training_evalution = dsTraining.Tables[0].Rows[0]["id_training_evalution"].ToString(),
                            report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            perf_monitor_period = /*objModel.GetPefpMonitoringPeriodById*/(dsTraining.Tables[0].Rows[0]["perf_monitor_period"].ToString()),
                            emp_name = /*objGlobaldata.GetEmpHrNameById*/(dsTraining.Tables[0].Rows[0]["emp_name"].ToString()),
                            upload = dsTraining.Tables[0].Rows[0]["upload"].ToString(),
                            comments = (dsTraining.Tables[0].Rows[0]["comments"].ToString()),
                            emp_perf_improved = dsTraining.Tables[0].Rows[0]["emp_perf_improved"].ToString(),
                            action_taken = (dsTraining.Tables[0].Rows[0]["action_taken"].ToString()),
                            planned_objective = /*objModel.GetPlannedObjectivesById*/(dsTraining.Tables[0].Rows[0]["planned_objective"].ToString()),
                            method_eval = (dsTraining.Tables[0].Rows[0]["method_eval"].ToString()),
                            further_training = (dsTraining.Tables[0].Rows[0]["further_training"].ToString()),
                        };

                        //DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        //{
                        //    objSupplier.evalu_date = dtValue;
                        //}

                        sSqlstmt = "SELECT id_train_trans, id_training_evalution, SQId, SQ_OptionsId FROM t_trainings_evalution_trans where id_training_evalution='"
                             + sid_training_evalution + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        TrainingEffectivenessModelsList objList = new TrainingEffectivenessModelsList();
                        objList.TrainList = new List<TrainingEffectivenessModels>();

                        SurveyModels objSurvey = new SurveyModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            TrainingEffectivenessModels objElements = new TrainingEffectivenessModels
                            {
                                id_train_trans = dsPerformanceElement.Tables[0].Rows[i]["id_train_trans"].ToString(),
                                SQId = (dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = (dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objList.TrainList.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objList;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;

                        ViewBag.SurveyQuestions = objSurvey.GetSurveyTypeListbox("Training Effectiveness Survey");
                        ViewBag.SurveyRating = objSurvey.GetSurveyRating(objSurvey.getSurveyIDByName("Training Effectiveness Survey"));
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.EmpList = objModel.GetTainingEmpList(dsTraining.Tables[0].Rows[0]["report_no"].ToString());
                        ViewBag.PerfPeriod = objGlobaldata.GetDropdownList("Performance Monitoring Period");
                        ViewBag.ReportNo = objModel.GetTrainingReportNoList();
                        ViewBag.PlannedObjective = objGlobaldata.GetDropdownList("Training Planned Objectives");
                        ViewBag.Method = objGlobaldata.GetDropdownList("Method of Effectiveness Evaluation");
                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("TrainingEffectivenessList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("TrainingEffectivenessList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEffectivenessList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingEffectivenessEdit(TrainingEffectivenessModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                //objModel.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                //objModel.Eval_ReviewedBy_DeptId = objGlobaldata.GetDeptIdByHrEmpId(objModel.Eval_ReviewedBy);
                //objModel.na = form["N/A"];

                //DateTime dateValue;
                //if (DateTime.TryParse(form["evalu_date"], out dateValue) == true)
                //{
                //    objModel.evalu_date = dateValue;
                //}

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "Training" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.upload = objModel.upload + "," + form["QCDocsVal"];
                    objModel.upload = objModel.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objModel.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.upload = null;
                }

                TrainingEffectivenessModelsList objList = new TrainingEffectivenessModelsList();
                objList.TrainList = new List<TrainingEffectivenessModels>();

                SurveyModels objSurvey = new SurveyModels();
                MultiSelectList SurveyQuestions = objSurvey.GetSurveyTypeListbox("Training Effectiveness Survey");

                foreach (var item in SurveyQuestions)
                {
                    TrainingEffectivenessModels objElements = new TrainingEffectivenessModels
                    {
                        SQId = form["Question " + item.Value],
                        SQ_OptionsId = objModel.GetFunRatingId(form["SQ_OptionsId " + item.Value])
                    };

                    objList.TrainList.Add(objElements);
                }
                if (objList.TrainList.Count > 0)
                {
                    objList.TrainList[0].id_training_evalution = objModel.id_training_evalution;
                }
                if (objModel.FunUpdateTrainingEvaluation(objModel, objList))
                {
                    TempData["Successdata"] = "Updated Training Effective Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingEffectivenessList");
        }

        [AllowAnonymous]
        public ActionResult TrainingEffectivenessDetails()
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                TrainingsModels TrainingModel = new TrainingsModels();
                TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();

                if (Request.QueryString["id_training_evalution"] != null && Request.QueryString["id_training_evalution"] != "")
                {
                    string sid_training_evalution = Request.QueryString["id_training_evalution"];

                    string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                    "emp_perf_improved,action_taken,planned_objective,logged_by,method_eval,further_training from t_trainings_evalution where id_training_evalution='" + sid_training_evalution + "'";

                    DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTraining.Tables.Count > 0 && dsTraining.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingEffectivenessModels
                        {
                            id_training_evalution = dsTraining.Tables[0].Rows[0]["id_training_evalution"].ToString(),
                            report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            perf_monitor_period = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["perf_monitor_period"].ToString()),
                            emp_name = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[0]["emp_name"].ToString()),
                            upload = dsTraining.Tables[0].Rows[0]["upload"].ToString(),
                            comments = (dsTraining.Tables[0].Rows[0]["comments"].ToString()),
                            emp_perf_improved = dsTraining.Tables[0].Rows[0]["emp_perf_improved"].ToString(),
                            action_taken = (dsTraining.Tables[0].Rows[0]["action_taken"].ToString()),
                            planned_objective = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["planned_objective"].ToString()),
                            Training_Topic = objModel.GetTrainingTopicByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            Sourceof_Training = TrainingModel.GetTrainingSourceNameById(objModel.GetSorceofTrainingByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString())),
                            //Trainer_Name =/* objModel.GetTrainingReportNoById*/(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            method_eval = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["method_eval"].ToString()),
                            further_training = (dsTraining.Tables[0].Rows[0]["further_training"].ToString()),
                        };

                        //DateTime dtValue;
                        //if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["evalu_date"].ToString(), out dtValue))
                        //{
                        //    objSupplier.evalu_date = dtValue;
                        //}

                        sSqlstmt = "SELECT id_train_trans, id_training_evalution, SQId, SQ_OptionsId FROM t_trainings_evalution_trans where id_training_evalution='"
                             + sid_training_evalution + "'";
                        DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        TrainingEffectivenessModelsList objList = new TrainingEffectivenessModelsList();
                        objList.TrainList = new List<TrainingEffectivenessModels>();

                        SurveyModels objSurvey = new SurveyModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                        {
                            TrainingEffectivenessModels objElements = new TrainingEffectivenessModels
                            {
                                id_train_trans = dsPerformanceElement.Tables[0].Rows[i]["id_train_trans"].ToString(),
                                SQId = objSurvey.GetSurveyQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                                SQ_OptionsId = objSurvey.GetSurveyRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                            };

                            dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                            objList.TrainList.Add(objElements);
                        }

                        ViewBag.PerformanceElement = objList;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("TrainingEffectivenessList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("TrainingEffectivenessList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEffectivenessList");
        }

        [AllowAnonymous]
        public ActionResult TrainingEffectivenessInfo(int Id)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                TrainingsModels TrainingModel = new TrainingsModels();
                TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();

                string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                   "emp_perf_improved,action_taken,planned_objective,logged_by from t_trainings_evalution where id_training_evalution='" + Id + "'";

                DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTraining.Tables.Count > 0 && dsTraining.Tables[0].Rows.Count > 0)
                {
                    objModel = new TrainingEffectivenessModels
                    {
                        id_training_evalution = dsTraining.Tables[0].Rows[0]["id_training_evalution"].ToString(),
                        report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                        perf_monitor_period = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["perf_monitor_period"].ToString()),
                        emp_name = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[0]["emp_name"].ToString()),
                        upload = dsTraining.Tables[0].Rows[0]["upload"].ToString(),
                        comments = (dsTraining.Tables[0].Rows[0]["comments"].ToString()),
                        emp_perf_improved = dsTraining.Tables[0].Rows[0]["emp_perf_improved"].ToString(),
                        action_taken = (dsTraining.Tables[0].Rows[0]["action_taken"].ToString()),
                        planned_objective = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["planned_objective"].ToString()),
                        Training_Topic = objModel.GetTrainingTopicByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                        Sourceof_Training = TrainingModel.GetTrainingSourceNameById(objModel.GetSorceofTrainingByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString())),
                        //Trainer_Name =/* objModel.GetTrainingReportNoById*/(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                    };

                    sSqlstmt = "SELECT id_train_trans, id_training_evalution, SQId, SQ_OptionsId FROM t_trainings_evalution_trans where id_training_evalution='" + Id + "'";
                    DataSet dsPerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                    TrainingEffectivenessModelsList objList = new TrainingEffectivenessModelsList();
                    objList.TrainList = new List<TrainingEffectivenessModels>();

                    SurveyModels objSurvey = new SurveyModels();

                    Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                    for (int i = 0; dsPerformanceElement.Tables.Count > 0 && i < dsPerformanceElement.Tables[0].Rows.Count; i++)
                    {
                        TrainingEffectivenessModels objElements = new TrainingEffectivenessModels
                        {
                            id_train_trans = dsPerformanceElement.Tables[0].Rows[i]["id_train_trans"].ToString(),
                            SQId = objSurvey.GetSurveyQuestionNameById(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString()),
                            SQ_OptionsId = objSurvey.GetSurveyRatingNameById(dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString())
                        };

                        dicPerformanceElements.Add(dsPerformanceElement.Tables[0].Rows[i]["SQId"].ToString(), dsPerformanceElement.Tables[0].Rows[i]["SQ_OptionsId"].ToString());
                        objList.TrainList.Add(objElements);
                    }

                    ViewBag.PerformanceElement = objList;
                    ViewBag.dicPerformanceElement = dicPerformanceElements;

                    return View(objModel);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("TrainingEffectivenessList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEffectivenessList");
        }

        [AllowAnonymous]
        public ActionResult TrainingEffectivenessPDF(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                TrainingsModels TrainingModel = new TrainingsModels();
                TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();

                if (form["id_training_evalution"] != null && form["id_training_evalution"] != "")
                {
                    string sid_training_evalution = form["id_training_evalution"];

                    string sSqlstmt = "select id_training_evalution, report_no, perf_monitor_period, emp_name, upload,comments," +
                    "emp_perf_improved,action_taken,planned_objective,logged_by from t_trainings_evalution where id_training_evalution='" + sid_training_evalution + "'";

                    DataSet dsTraining = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTraining.Tables.Count > 0 && dsTraining.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingEffectivenessModels
                        {
                            id_training_evalution = dsTraining.Tables[0].Rows[0]["id_training_evalution"].ToString(),
                            report_no = objModel.GetTrainingReportNoById(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            perf_monitor_period = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["perf_monitor_period"].ToString()),
                            emp_name = objGlobaldata.GetEmpHrNameById(dsTraining.Tables[0].Rows[0]["emp_name"].ToString()),
                            upload = dsTraining.Tables[0].Rows[0]["upload"].ToString(),
                            comments = (dsTraining.Tables[0].Rows[0]["comments"].ToString()),
                            emp_perf_improved = dsTraining.Tables[0].Rows[0]["emp_perf_improved"].ToString(),
                            action_taken = (dsTraining.Tables[0].Rows[0]["action_taken"].ToString()),
                            planned_objective = objGlobaldata.GetDropdownitemById(dsTraining.Tables[0].Rows[0]["planned_objective"].ToString()),
                            Training_Topic = objModel.GetTrainingTopicByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                            Sourceof_Training = TrainingModel.GetTrainingSourceNameById(objModel.GetSorceofTrainingByReportNo(dsTraining.Tables[0].Rows[0]["report_no"].ToString())),
                            //Trainer_Name =/* objModel.GetTrainingReportNoById*/(dsTraining.Tables[0].Rows[0]["report_no"].ToString()),
                        };

                        sSqlstmt = "SELECT id_train_trans, id_training_evalution, SQId, SQ_OptionsId FROM t_trainings_evalution_trans where id_training_evalution='"
                             + sid_training_evalution + "'";
                        ViewBag.PerformanceElement = objGlobaldata.Getdetails(sSqlstmt);

                        ViewBag.Evaluation = objModel;
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("TrainingEffectivenessList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Performance Id cannot be null";
                    return RedirectToAction("TrainingEffectivenessList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEffectivenessPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("TrainingEffectivenessPDF")
            {
                //FileName = "Evaluation.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        public ActionResult GetRating()
        {
            try
            {
                string sql = "select RatingOptions from t_survey_type a,t_surveyquestion_options b where a.Survey_TypeId=b.Survey_TypeId and a.TypeName='Training Effectiveness Survey' and b.active=1";
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

        // [HttpPost]
        public JsonResult FunGetTrainingDetails(string report_no)
        {
            TrainingsModels objModel = new TrainingsModels();

            if (report_no != "")
            {
                string sql = "select Training_Topic,Sourceof_Training,Trainer_Name,Ext_Trainer_Name from t_trainings where TrainingID= '" + report_no + "' and active=1";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objModel = new TrainingsModels()
                    {
                        Training_Topic = dsData.Tables[0].Rows[0]["Training_Topic"].ToString(),
                        Sourceof_Training = objModel.GetTrainingSourceNameById(dsData.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                        Trainer_Name = objGlobaldata.GetEmpHrNameById(dsData.Tables[0].Rows[0]["Trainer_Name"].ToString()),
                        Ext_Trainer_Name = dsData.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                    };
                }
            }
            return Json(objModel);
        }

        public JsonResult FunGetTrainingDetails1(string report_no)
        {
            TrainingsModels objModel = new TrainingsModels();

            if (report_no != "")
            {
                string sql = "select Training_Topic,Sourceof_Training,Trainer_Name,Ext_Trainer_Name from t_trainings where report_no= '" + report_no + "' and active=1";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objModel = new TrainingsModels()
                    {
                        Training_Topic = dsData.Tables[0].Rows[0]["Training_Topic"].ToString(),
                        Sourceof_Training = objModel.GetTrainingSourceNameById(dsData.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                        Trainer_Name = objGlobaldata.GetEmpHrNameById(dsData.Tables[0].Rows[0]["Trainer_Name"].ToString()),
                        Ext_Trainer_Name = dsData.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                    };
                }
            }
            return Json(objModel);
        }

        public ActionResult FunGetEmployeeList(string report_no)
        {
            TrainingEffectivenessModels objModel = new TrainingEffectivenessModels();
            MultiSelectList lstDept = objModel.GetTainingEmpList(report_no);
            return Json(lstDept);
        }

        //Training Effective Evaluation Questions

        [AllowAnonymous]
        public ActionResult AddEvaluationQuestions()
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            SurveyModels objSurvey = new SurveyModels();
            try
            {
                //ViewBag.Survey_Type = objSurvey.getSurveyIDByName("Training Effectiveness Survey");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluationQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSurvey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEvaluationQuestions(SurveyModels objSurveyModels, FormCollection form)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
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
                        TempData["Successdata"] = "Added Training Evalution Questions successfully";
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
                //ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId);
                ViewBag.dsSurveyQuestions = objSurvey.GetSurveyQuestionsListbox(objSurveyModels.Survey_TypeId);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluationQuestions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSurveyModels);
        }

        [AllowAnonymous]
        public ActionResult EvaluationQuestionsDelete(string SQId)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";

            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyQuestions(SQId))
                {
                    TempData["Successdata"] = "Training Evaluation details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EvaluationQuestionsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddEvaluationQuestions", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByQuestionId(SQId) });
        }

        [AllowAnonymous]
        public ActionResult EvaluationQuestionsDelete1(string SQId)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
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
                objGlobaldata.AddFunctionalLog("Exception in EvaluationQuestionsDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult EvaluationQuestionUpdate(string SQId, string Questions)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
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
                objGlobaldata.AddFunctionalLog("Exception in EvaluationQuestionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //Training Effective Evaluation Rating

        [AllowAnonymous]
        public ActionResult AddEvaluationRatings()
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            SurveyModels objSurvey = new SurveyModels();
            try
            {
                //ViewBag.Survey_Type = objSurvey.getSurveyIDByName("Training Effectiveness Survey");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluationRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSurvey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEvaluationRatings(SurveyModels objSurveyModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "TrainingEffectiveness";
                SurveyModels objSurvey = new SurveyModels();
                ViewBag.dsSurvey = objSurvey.GetSurveyTypeListbox();

                ViewBag.Survey_Type = objSurveyModels.Survey_TypeId;
                ViewBag.Survey_TypeId = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                objSurveyModels.Survey_TypeId1 = objSurvey.getSurveyIDByName(objSurveyModels.Survey_TypeId);
                objSurveyModels.Weightage = "0";
                if (objSurveyModels.RatingOptions != "")
                {
                    if (objSurveyModels.FunAddSurveyRatingFactor(objSurveyModels))
                    {
                        TempData["Successdata"] = "Added Training Evalution Ratings successfully";
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
                // ViewBag.dsSurveyRating = objSurvey.GetSurveyRating(objSurveyModels.Survey_TypeId);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEvaluationRatings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSurveyModels);
        }

        [AllowAnonymous]
        public ActionResult EvaluationRatingsDelete(string SQ_OptionsId)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyRatingFactor(SQ_OptionsId))
                {
                    TempData["Successdata"] = "Training Evaluation details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EvaluationRatingsDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AddEvaluationRatings", new { Survey_TypeId = objSurveyModels.GetSurveyTypeIdByRatingId(SQ_OptionsId) });
        }

        [AllowAnonymous]
        public ActionResult EvaluationRatingsDelete1(string SQ_OptionsId)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunDeleteSurveyRatingFactor(SQ_OptionsId))
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
                objGlobaldata.AddFunctionalLog("Exception in EvaluationRatingsDelete1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult EvaluationRatingUpdate(string SQ_OptionsId, string RatingOptions)
        {
            ViewBag.SubMenutype = "TrainingEffectiveness";
            SurveyModels objSurveyModels = new SurveyModels();
            try
            {
                if (objSurveyModels.FunUpdateSurveyRating(SQ_OptionsId, RatingOptions))
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
                objGlobaldata.AddFunctionalLog("Exception in EvaluationRatingUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}