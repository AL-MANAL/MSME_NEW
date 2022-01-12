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

                    string sSqlstmt = "select id_training_plan,division,department,location,topic,emp_id,from_date,to_date,source_id,trainer_name,reviewed_by,approved_by,ext_entity,ref_no"

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
                            ref_no = (dsList.Tables[0].Rows[0]["ref_no"].ToString()),
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

                string sSqlstmt = "select id_training_plan,topic,department,from_date,to_date,training_status,ref_no," +
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
                                training_status = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["training_status"].ToString()),
                                ref_no = dsList.Tables[0].Rows[i]["ref_no"].ToString(),
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
                         +  "(CASE WHEN approval_status = '0' THEN 'Pending for Review' WHEN approval_status = '1' THEN 'Review Rejected' WHEN approval_status = '2' THEN 'Reviewed,Pending for Approval' WHEN approval_status = '3' THEN 'Rejected' WHEN approval_status = '4' THEN 'Approved' END) as  approval_status,"
                         + "(CASE  WHEN review_status = '1' THEN 'Review Rejected' WHEN review_status = '2' THEN 'Reviewed'  END) as  review_status"
                        + " ,approval_status as approval_status_id,reviewer_comments,reviewed_date,approver_comments,approved_date,training_status,status_updated_date,training_start_date,training_completed_date,upload,reshedule_from_date,reshedule_to_date,reason_reschedule,reason_cancell,ref_no from t_training_plan where id_training_plan = '" + id_training_plan + "'";

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
                            approval_status = (dsList.Tables[0].Rows[0]["approval_status"].ToString()),
                            review_status = (dsList.Tables[0].Rows[0]["review_status"].ToString()),

                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            reason_reschedule = dsList.Tables[0].Rows[0]["reason_reschedule"].ToString(),
                            reason_cancell = dsList.Tables[0].Rows[0]["reason_cancell"].ToString(),
                            training_status =objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[0]["training_status"].ToString()),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
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
                        if (dsList.Tables[0].Rows[0]["status_updated_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["status_updated_date"].ToString(), out dtDocDate))
                        {
                            objModel.status_updated_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["training_start_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["training_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.training_start_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["training_completed_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["training_completed_date"].ToString(), out dtDocDate))
                        {
                            objModel.training_completed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reshedule_from_date"].ToString() != ""
                    && DateTime.TryParse(dsList.Tables[0].Rows[0]["reshedule_from_date"].ToString(), out dtDocDate))
                        {
                            objModel.reshedule_from_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reshedule_to_date"].ToString() != ""
                  && DateTime.TryParse(dsList.Tables[0].Rows[0]["reshedule_to_date"].ToString(), out dtDocDate))
                        {
                            objModel.reshedule_to_date = dtDocDate;
                        }

                        string sql = "select emp_id,certificate,expiry_date,updated_date from t_training_plan_certificate where id_training_plan='"+ id_training_plan + "'";
                        ViewBag.Cert = objGlobaldata.Getdetails(sql);
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

        public ActionResult TrainingPlanReview(TrainingPlanModels objModel, FormCollection form)
        {
            try
            {

                if (objModel.FunTrainingPlanReview(objModel))
                {
                    TempData["Successdata"] = "Status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingPlanReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult TrainingPlanApprove(TrainingPlanModels objModel, FormCollection form)
        {
            try
            {

                if (objModel.FunTrainingPlanApprove(objModel))
                {
                    TempData["Successdata"] = "Status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingPlanApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public JsonResult TrainingPlanDelete(FormCollection form)
        {
            try
            {
                if (form["Id"] != null && form["Id"] != "")
                {
                    TrainingPlanModels objModel = new TrainingPlanModels();
                    string sId = form["Id"];

                    if (objModel.FunDeleteTrainingPlan(sId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingPlanDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //Employee List
        [AllowAnonymous]
        public ActionResult EmployeeList(int id)
        {
            TrainingPlanModels objTrainings = new TrainingPlanModels();
            try
            {
                if (id > 0)
                {
                    string Training_id = objTrainings.GetAttendees(id);
                    if (Training_id != "" && Training_id != null)
                    {
                      
                        string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid"
                        + " from t_hr_employee where emp_status=1 and emp_no in (" + Training_id + ")";
                        DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Person_Name = dsData;
                    }
                    else
                    {
                        ViewBag.Person_Name = "";
                    }

                   
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult UpdateTrainingStatus()
        {
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {

                if (Request.QueryString["id_training_plan"] != null && Request.QueryString["id_training_plan"] != "")
                {

                    string id_training_plan = Request.QueryString["id_training_plan"];

                    string sSqlstmt = "select id_training_plan,training_status,status_updated_date,training_start_date,training_completed_date,upload,reshedule_from_date,reshedule_to_date,reason_reschedule,reason_cancell"

                        + " from t_training_plan where id_training_plan = '" + id_training_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingPlanModels
                        {
                            id_training_plan = dsList.Tables[0].Rows[0]["id_training_plan"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            reason_reschedule = dsList.Tables[0].Rows[0]["reason_reschedule"].ToString(),
                            reason_cancell = dsList.Tables[0].Rows[0]["reason_cancell"].ToString(),
                            training_status = dsList.Tables[0].Rows[0]["training_status"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["status_updated_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["status_updated_date"].ToString(), out dtDocDate))
                        {
                            objModel.status_updated_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["training_start_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["training_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.training_start_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["training_completed_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["training_completed_date"].ToString(), out dtDocDate))
                        {
                            objModel.training_completed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reshedule_from_date"].ToString() != ""
                    && DateTime.TryParse(dsList.Tables[0].Rows[0]["reshedule_from_date"].ToString(), out dtDocDate))
                        {
                            objModel.reshedule_from_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["reshedule_to_date"].ToString() != ""
                  && DateTime.TryParse(dsList.Tables[0].Rows[0]["reshedule_to_date"].ToString(), out dtDocDate))
                        {
                            objModel.reshedule_to_date = dtDocDate;
                        }
                    }
                }
              
                ViewBag.Status = objGlobaldata.GetDropdownList("Training Plan Status");
             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateTrainingStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateTrainingStatus(TrainingPlanModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                IList<HttpPostedFileBase> nc_uploadList = (IList<HttpPostedFileBase>)upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (nc_uploadList[0] != null)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "TP" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in UpdateTrainingStatus-upload: " + ex.ToString());

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
                else if (form["QCDocsVal"] == null && QCDelete != null && nc_uploadList[0] == null)
                {
                    objModel.upload = null;
                }
                else if (form["QCDocsVal"] == null && nc_uploadList[0] == null)
                {
                    objModel.upload = null;
                }
              
                DateTime dateValue;
                if (form["status_updated_date"] != null && DateTime.TryParse(form["status_updated_date"], out dateValue) == true)
                {
                    objModel.status_updated_date = dateValue;
                }
                if (form["training_start_date"] != null && DateTime.TryParse(form["training_start_date"], out dateValue) == true)
                {
                    objModel.training_start_date = dateValue;
                }
                if (form["training_completed_date"] != null && DateTime.TryParse(form["training_completed_date"], out dateValue) == true)
                {
                    objModel.training_completed_date = dateValue;
                }
                if (form["reshedule_from_date"] != null && DateTime.TryParse(form["reshedule_from_date"], out dateValue) == true)
                {
                    objModel.reshedule_from_date = dateValue;
                }
                if (form["reshedule_to_date"] != null && DateTime.TryParse(form["reshedule_to_date"], out dateValue) == true)
                {
                    objModel.reshedule_to_date = dateValue;
                }
                if (objModel.FunUpdateTrainingStatus(objModel))
                {
                    TempData["Successdata"] = "Updated training status successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateTrainingStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingPlanList");
        }


        //NC List
        [AllowAnonymous]
        public ActionResult CertificateList()
        {
            TrainingPlanModelsList objList = new TrainingPlanModelsList();
            objList.ObjList = new List<TrainingPlanModels>();
            
            try
            {
                if (Request.QueryString["id_training_plan"] != null && Request.QueryString["id_training_plan"] != "")
                {
                    string id_training_plan = Request.QueryString["id_training_plan"];
                    string id_certificate = Request.QueryString["id_certificate"];
                    string sSqlstmt = "select id_certificate,id_training_plan,emp_id,certificate,expiry_date,updated_date from t_training_plan_certificate where id_training_plan = '" + id_training_plan + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                TrainingPlanModels objModel = new TrainingPlanModels
                                {
                                    id_certificate = dsList.Tables[0].Rows[i]["id_certificate"].ToString(),
                                    id_training_plan = dsList.Tables[0].Rows[i]["id_training_plan"].ToString(),
                                    emp_id =objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["emp_id"].ToString()),
                                    certificate = dsList.Tables[0].Rows[i]["certificate"].ToString(),

                                };
                                DateTime dtDocDate;
                                if (dsList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                                 && DateTime.TryParse(dsList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                                {
                                    objModel.expiry_date = dtDocDate;
                                }
                                if (dsList.Tables[0].Rows[i]["updated_date"].ToString() != ""
                                && DateTime.TryParse(dsList.Tables[0].Rows[i]["updated_date"].ToString(), out dtDocDate))
                                {
                                    objModel.updated_date = dtDocDate;
                                }
                                objList.ObjList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in CertificateList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }

                        sSqlstmt = "select id_certificate,id_training_plan,emp_id,certificate,expiry_date,updated_date from t_training_plan_certificate where id_certificate = '" + id_certificate + "'";

                        dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                TrainingPlanModels objModel = new TrainingPlanModels
                                {
                                    id_certificate = dsList.Tables[0].Rows[0]["id_certificate"].ToString(),
                                    id_training_plan = dsList.Tables[0].Rows[0]["id_training_plan"].ToString(),
                                    emp_id = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                                    certificate = dsList.Tables[0].Rows[0]["certificate"].ToString(),

                                };
                                DateTime dtDocDate;
                                if (dsList.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                                 && DateTime.TryParse(dsList.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                                {
                                    objModel.expiry_date = dtDocDate;
                                }
                                if (dsList.Tables[0].Rows[0]["updated_date"].ToString() != ""
                                && DateTime.TryParse(dsList.Tables[0].Rows[0]["updated_date"].ToString(), out dtDocDate))
                                {
                                    objModel.updated_date = dtDocDate;
                                }
                                ViewBag.Certificate = objModel;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in CertificateList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                           
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Id Cannot be null";
                        return RedirectToAction("TrainingPlanList");
                    }
                   
                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                    return RedirectToAction("TrainingPlanList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.ObjList.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CertificateUpdate(TrainingPlanModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> certificate)
        {
            try
            {
                IList<HttpPostedFileBase> nc_uploadList = (IList<HttpPostedFileBase>)certificate;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (nc_uploadList[0] != null)
                {
                    objModel.certificate = "";
                    foreach (var file in certificate)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "TP" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.certificate = objModel.certificate + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CertificateUpdate-upload: " + ex.ToString());

                        }
                    }
                    objModel.certificate = objModel.certificate.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.certificate = objModel.certificate + "," + form["QCDocsVal"];
                    objModel.certificate = objModel.certificate.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && nc_uploadList[0] == null)
                {
                    objModel.certificate = null;
                }
                else if (form["QCDocsVal"] == null && nc_uploadList[0] == null)
                {
                    objModel.certificate = null;
                }

                DateTime dateValue;
                if (form["expiry_date"] != null && DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                {
                    objModel.expiry_date = dateValue;
                }
                if (form["updated_date"] != null && DateTime.TryParse(form["updated_date"], out dateValue) == true)
                {
                    objModel.updated_date = dateValue;
                }
                
                if (objModel.FunUpdateCertUpdate(objModel))
                {
                    TempData["Successdata"] = "Updated training certificate successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingPlanList");
        }

        [AllowAnonymous]
        public ActionResult TrainingCriticalityList(int? page)
        {
            TrainingPlanModelsList objList = new TrainingPlanModelsList();
            objList.ObjList = new List<TrainingPlanModels>();
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {
                string sSqlstmt = "select id_training_criticality,training_type,no_days,criticality from t_training_criticality where active = 1 order by id_training_criticality desc; ";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.criticality = objGlobaldata.GetDropdownList("Training Criticality");
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objModel = new TrainingPlanModels
                            {
                                id_training_criticality = (dsList.Tables[0].Rows[i]["id_training_criticality"].ToString()),
                                training_type = (dsList.Tables[0].Rows[i]["training_type"].ToString()),
                                no_days = (dsList.Tables[0].Rows[i]["no_days"].ToString()),
                                criticality =objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["criticality"].ToString()),
                            };
                            objList.ObjList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingCriticalityList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingCriticalityList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.ObjList.ToList());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddTrainingCriticality(TrainingPlanModels objModel, FormCollection form)
        {
            try
            {
                if (objModel.FunAddTrainingCriticality(objModel))
                {
                    TempData["Successdata"] = "Added Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingCriticality: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingCriticalityList");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EditTrainingCriticality()
        {
            TrainingPlanModels objModel = new TrainingPlanModels();
            try
            {
                if (Request.QueryString["id_training_criticality"] != null && Request.QueryString["id_training_criticality"] != "")
                {
                    string id_training_criticality = Request.QueryString["id_training_criticality"];
                    ViewBag.criticality = objGlobaldata.GetDropdownList("Training Criticality");
                    string sSqlstmt = "select id_training_criticality,training_type,no_days,criticality from t_training_criticality where id_training_criticality='" + id_training_criticality + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new TrainingPlanModels
                        {
                            id_training_criticality = (dsList.Tables[0].Rows[0]["id_training_criticality"].ToString()),
                            training_type = (dsList.Tables[0].Rows[0]["training_type"].ToString()),
                            no_days = (dsList.Tables[0].Rows[0]["no_days"].ToString()),
                            criticality = (dsList.Tables[0].Rows[0]["criticality"].ToString()),
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("TrainingCriticalityList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("TrainingCriticalityList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditTrainingCriticality: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("TrainingCriticalityList");
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrainingCriticality(TrainingPlanModels objModel, FormCollection form)
        {
            try
            {
                if (objModel.FunUpdateTrainingCriticality(objModel))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EditTrainingCriticality: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingCriticalityList");
        }

        [AllowAnonymous]
        public JsonResult TrainingCriticalityDelete(FormCollection form)
        {
            try
            {
                if (form["Id"] != null && form["Id"] != "")
                {
                    TrainingPlanModels objModel = new TrainingPlanModels();
                    string sId = form["Id"];

                    if (objModel.FunDeleteTrainingCriticality(sId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingCriticalityDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

    }
}