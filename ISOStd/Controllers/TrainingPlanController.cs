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
using System.Security.Principal;
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TrainingPlanController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        public ActionResult AddTrainingPlan()
        {
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {
                objModel.division = objGlobaldata.GetCurrentUserSession().division;
                objModel.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.division);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.division);


                ViewBag.Topic = objGlobaldata.GetTrainingTopicList();
                ViewBag.EmpList = objGlobaldata.GetTrainingPlanEmployeeList();
                ViewBag.Trainer = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Source = objGlobaldata.GetDropdownList("Training Source");
                ViewBag.ReviewedBy = objGlobaldata.GetHrEmployeeList();
                ViewBag.ApprovedBy = objGlobaldata.GetTopMgmtEmpListbox();


          
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTrainingPlan(TrainingPlanModels objModel, FormCollection form)
        { 
            try
            {
                objModel.emp_id = form["emp_id"];
                DateTime dateValue;
                if (form["from_date"] != null && DateTime.TryParse(form["from_date"], out dateValue) == true)
                {
                    objModel.from_date = dateValue;
                }
                if (form["to_date"] != null && DateTime.TryParse(form["to_date"], out dateValue) == true)
                {
                    objModel.to_date = dateValue;
                }
                if (objModel.FunAddTrainingPlan(objModel))
                {
                    TempData["Successdata"] = "Added training plan successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingPlanList");
        }


        [AllowAnonymous]
        public ActionResult EditTrainingPlan()
        {
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {

                if (Request.QueryString["id_training_plan"] != null && Request.QueryString["id_training_plan"] != "")
                {

                    string id_training_plan = Request.QueryString["id_training_plan"];

                    string sSqlstmt = "select id_training_plan,division,department,location,topic,emp_id,from_date,to_date,source_id,trainer_name,reviewed_by,approved_by,ext_entity"

                        + " from t_training_plan where id_training_plan = '" + id_training_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingPlanModels
                        {
                            id_training_plan = dsList.Tables[0].Rows[0]["id_training_plan"].ToString(),
                            division = (dsList.Tables[0].Rows[0]["division"].ToString()),
                            department = (dsList.Tables[0].Rows[0]["department"].ToString()),
                            location = (dsList.Tables[0].Rows[0]["location"].ToString()),
                            topic = (dsList.Tables[0].Rows[0]["topic"].ToString()),
                            emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                            source_id = (dsList.Tables[0].Rows[0]["source_id"].ToString()),
                            trainer_name = (dsList.Tables[0].Rows[0]["trainer_name"].ToString()),
                            reviewed_by = (dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            ext_entity = (dsList.Tables[0].Rows[0]["ext_entity"].ToString()),
                            approved_by = (dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                         
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["from_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["from_date"].ToString(), out dtDocDate))
                        {
                            objModel.from_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModel.to_date = dtDocDate;
                        }
                    }
                }
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.division);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.division);


                ViewBag.Topic = objGlobaldata.GetTrainingTopicList();
                ViewBag.EmpList = objGlobaldata.GetTrainingPlanEmployeeList();
                ViewBag.Trainer = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Source = objGlobaldata.GetDropdownList("Training Source");
                ViewBag.ReviewedBy = objGlobaldata.GetHrEmployeeList();
                ViewBag.ApprovedBy = objGlobaldata.GetTopMgmtEmpListbox();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditTrainingPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrainingPlan(TrainingPlanModels objModel, FormCollection form)
        {
            try
            {
                objModel.emp_id = form["emp_id"];
                DateTime dateValue;
                if (form["from_date"] != null && DateTime.TryParse(form["from_date"], out dateValue) == true)
                {
                    objModel.from_date = dateValue;
                }
                if (form["to_date"] != null && DateTime.TryParse(form["to_date"], out dateValue) == true)
                {
                    objModel.to_date = dateValue;
                }
                if (objModel.FunUpdateTrainingPlan(objModel))
                {
                    TempData["Successdata"] = "Updated training plan successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditTrainingPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingPlanList");
        }

        [AllowAnonymous]
        public ActionResult TrainingPlanList(string branch_name)
        {
            ViewBag.SubMenutype = "TrainingPlanList";
            TrainingPlanModelsList objList = new TrainingPlanModelsList();
            objList.ObjList = new List<TrainingPlanModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_training_plan,topic,department,from_date,to_date," +
                 "(CASE WHEN approval_status = '0' THEN 'Pending for Review' WHEN approval_status = '1' THEN 'Review Rejected' WHEN approval_status = '2' THEN 'Reviewed,Pending for Approval' WHEN approval_status = '3' THEN 'Rejected' WHEN approval_status = '4' THEN 'Approved' END) as  approval_status" +
                    " from t_training_plan where active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + " order by id_training_plan desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TrainingPlanModels objModel = new TrainingPlanModels
                            {
                                id_training_plan = dsList.Tables[0].Rows[i]["id_training_plan"].ToString(),
                                department =objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["department"].ToString()),
                                topic =objGlobaldata.GetTrainingTopicById(dsList.Tables[0].Rows[i]["topic"].ToString()),
                                approval_status = (dsList.Tables[0].Rows[i]["approval_status"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["from_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["from_date"].ToString(), out dtDocDate))
                            {
                                objModel.from_date = dtDocDate;
                            }
                            if (dsList.Tables[0].Rows[i]["to_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["to_date"].ToString(), out dtDocDate))
                            {
                                objModel.to_date = dtDocDate;
                            }
                            objList.ObjList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingPlanList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingPlanList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.ObjList.ToList());
        }

        [AllowAnonymous]
        public ActionResult TrainingPlanDetail()
        {
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {

                if (Request.QueryString["id_training_plan"] != null && Request.QueryString["id_training_plan"] != "")
                {

                    string id_training_plan = Request.QueryString["id_training_plan"];

                    string sSqlstmt = "select id_training_plan,division,department,location,topic,emp_id,from_date,to_date,source_id,trainer_name,reviewed_by,approved_by,ext_entity,"
                         +  "(CASE WHEN approval_status = '0' THEN 'Pending for Review' WHEN approval_status = '1' THEN 'Review Rejected' WHEN approval_status = '2' THEN 'Reviewed,Pending for Approval' WHEN approval_status = '3' THEN 'Rejected' WHEN approval_status = '4' THEN 'Approved' END) as  approval_status" 
                        + " ,approval_status as approval_status_id,reviewer_comments,reviewed_date,approver_comments,approved_date from t_training_plan where id_training_plan = '" + id_training_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingPlanModels
                        {
                            id_training_plan = dsList.Tables[0].Rows[0]["id_training_plan"].ToString(),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString()),
                            department = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["department"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["location"].ToString()),
                            topic =objGlobaldata.GetTrainingTopicById(dsList.Tables[0].Rows[0]["topic"].ToString()),
                            emp_id = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                            source_id = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["source_id"].ToString()),
                            trainer_name = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["trainer_name"].ToString()),
                            reviewed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            ext_entity = (dsList.Tables[0].Rows[0]["ext_entity"].ToString()),
                            approved_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            approval_status_id = (dsList.Tables[0].Rows[0]["approval_status_id"].ToString()),
                            reviewed_by_id = (dsList.Tables[0].Rows[0]["reviewed_by"].ToString()),
                            approved_by_id = (dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            reviewer_comments = (dsList.Tables[0].Rows[0]["reviewer_comments"].ToString()),
                            approver_comments = (dsList.Tables[0].Rows[0]["approver_comments"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["from_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["from_date"].ToString(), out dtDocDate))
                        {
                            objModel.from_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["to_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["to_date"].ToString(), out dtDocDate))
                        {
                            objModel.to_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reviewed_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["reviewed_date"].ToString(), out dtDocDate))
                        {
                            objModel.reviewed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["approved_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["approved_date"].ToString(), out dtDocDate))
                        {
                            objModel.approved_date = dtDocDate;
                        }
                    }
                }
              
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingPlanDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

    }
}