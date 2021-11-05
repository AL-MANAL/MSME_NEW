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
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TrainingEventsController:Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        TrainingsModels objTrainings = new TrainingsModels();

        public TrainingEventsController()
        {
            ViewBag.SubMenutype = "TrainingEvents";
            ViewBag.Menutype = "Employee";
        }

        [AllowAnonymous]
        public ActionResult TrainingEventsList(int? page, string branch_name)
        {
            TrainingsModelsList objTrainingsModelsList = new TrainingsModelsList();
            objTrainingsModelsList.TrainingsMList = new List<TrainingsModels>();
            TrainingsModels objtr = new TrainingsModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                 string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status,Attended_no, "
                    + " (case when RequestStatus='0' then 'Registration Phase Completed' when (RequestStatus='1' and (Training_Actual_Date <='01/01/0001'|| Training_Actual_Date is null)) then 'Training Approved'"
                    + " when(RequestStatus = '1' and Training_Actual_Date > '01/01/0001') then 'Training Completed' when RequestStatus = '2' then 'Training Rejected' end) as RequestStatus"
                    + ", ApprovedBy,Training_Status,report_no from t_trainings where Active=1";

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                string sRolename = objGlobaldata.GetRoleName(objUser.role);

                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                ViewBag.Training_Topic = objGlobaldata.GetDropdownList("Training Topic");

                string sSearchtext = "";            

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Traning_BeforeDate desc";
                DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                {


                    for (int i = 0; i < dsTrainingsList.Tables[0].Rows.Count; i++)
                    {

                        TrainingsModels objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[i]["TrainingID"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[i]["DeptId"].ToString()),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["Attendees"].ToString()),
                            Training_Topic = /*objGlobaldata.GetDropdownitemById*/(dsTrainingsList.Tables[0].Rows[i]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Requested_By"].ToString()),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[i]["Reasonfor_Training"].ToString(),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[i]["RequestStatus"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            //Training_Status = objtr.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Status"].ToString()),
                            //Training_Attended_no = dsTrainingsList.Tables[0].Rows[i]["Attended_no"].ToString()
                            report_no = dsTrainingsList.Tables[0].Rows[i]["report_no"].ToString(),
                        };
                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;
                        //DateTime dtTraning_BeforeDate;
                        //if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[i]["Traning_BeforeDate"].ToString(), out dtTraning_BeforeDate))
                        //{
                        //    objTrainingsModels.Traning_BeforeDate = dtTraning_BeforeDate;
                        //}
                        objTrainingsModelsList.TrainingsMList.Add(objTrainingsModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingsModelsList.TrainingsMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult TrainingEventsListSearch(int? page, string branch_name)
        {
            TrainingsModelsList objTrainingsModelsList = new TrainingsModelsList();
            objTrainingsModelsList.TrainingsMList = new List<TrainingsModels>();
            TrainingsModels objtr = new TrainingsModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status,Attended_no, "
                   + " (case when RequestStatus='0' then 'Registration Phase Completed' when (RequestStatus='1' and (Training_Actual_Date <='01/01/0001'|| Training_Actual_Date is null)) then 'Training Approved'"
                   + " when(RequestStatus = '1' and Training_Actual_Date > '01/01/0001') then 'Training Completed' when RequestStatus = '2' then 'Training Rejected' end) as RequestStatus"
                   + ", ApprovedBy,Training_Status,report_no from t_trainings where Active=1";

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                string sRolename = objGlobaldata.GetRoleName(objUser.role);

                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                ViewBag.Training_Topic = objGlobaldata.GetDropdownList("Training Topic");

                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Traning_BeforeDate desc";
                DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                {


                    for (int i = 0; i < dsTrainingsList.Tables[0].Rows.Count; i++)
                    {

                        TrainingsModels objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[i]["TrainingID"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[i]["DeptId"].ToString()),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["Attendees"].ToString()),
                            Training_Topic = /*objGlobaldata.GetDropdownitemById*/(dsTrainingsList.Tables[0].Rows[i]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Requested_By"].ToString()),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[i]["Reasonfor_Training"].ToString(),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[i]["RequestStatus"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            //Training_Status = objtr.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Status"].ToString()),
                            //Training_Attended_no = dsTrainingsList.Tables[0].Rows[i]["Attended_no"].ToString()
                            report_no = dsTrainingsList.Tables[0].Rows[i]["report_no"].ToString(),
                        };
                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;
                        //DateTime dtTraning_BeforeDate;
                        //if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[i]["Traning_BeforeDate"].ToString(), out dtTraning_BeforeDate))
                        //{
                        //    objTrainingsModels.Traning_BeforeDate = dtTraning_BeforeDate;
                        //}
                        objTrainingsModelsList.TrainingsMList.Add(objTrainingsModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }


        [AllowAnonymous]
        public ActionResult TrainingEventsEdit()
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();
            try
            {
                if (Request.QueryString["TrainingID"] != null && Request.QueryString["TrainingID"] != "")
                {
                    UserCredentials objUser = new UserCredentials();
                    objUser = objGlobaldata.GetCurrentUserSession();
                    string sRolename = objGlobaldata.GetRoleName(objUser.role);

                    ViewBag.Role = sRolename;
                    ViewBag.CurrentEmpName = objUser.firstname;
                    ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                    string sTrainingID = Request.QueryString["TrainingID"];

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    ViewBag.Category = objGlobaldata.GetDropdownList("Training Category");
                    ViewBag.Sourceof_Training = objGlobaldata.GetDropdownList("Training-SourceList");
                    ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                    ViewBag.AllEmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                    ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                    ViewBag.Approver = objGlobaldata.GetApprover();

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic,TopicContent ,Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date,Course_Material, Training_Actual_Completion_Date, ApprovedBy, Traning_BeforeDate,DeptId,Category,Service_provider,Ext_Trainer_Name," +
                                "Upload,Attendees_Upload,RequestStatus,report_no from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = /*objGlobaldata.GetDropdownitemById*/(dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            // Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = /*objGlobaldata.GetMultiHrEmpNameById*/(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ApprovedByName = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = /*objTrainingsModels.GetTrainingSourceNameById*/(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            // Training_Status = objTrainingsModels.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString()),
                            //Training_Cert_Status = objTrainingsModels.GetTrainingCertificateStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()),
                            //Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                            //Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                            //Training_Effect_Eval_Method = objTrainingsModels.GetTrainingEvaluationNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()),
                            //Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                            TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                            //Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString(),
                            DeptId = dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString(),
                            Category = dsTrainingsList.Tables[0].Rows[0]["Category"].ToString(),
                            Service_provider = dsTrainingsList.Tables[0].Rows[0]["Service_provider"].ToString(),
                            Ext_Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                            Upload = dsTrainingsList.Tables[0].Rows[0]["Upload"].ToString(),
                            Attendees_Upload = dsTrainingsList.Tables[0].Rows[0]["Attendees_Upload"].ToString(),
                            report_no = dsTrainingsList.Tables[0].Rows[0]["report_no"].ToString(),
                        };

                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Planned_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Planned_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Start_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Start_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Expected_Date_Completion"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Expected_Date_Completion = dtDatetime;
                        }
                       // if (objTrainingsModels.Training_Start_Date.ToString("dd/MM/yyyy") == objTrainingsModels.Expected_Date_Completion.ToString("dd/MM/yyyy"))
                       //     { 
                       // int Start_time = Convert.ToInt32(objTrainingsModels.Training_Start_Date.ToString("HH")) * 60 + Convert.ToInt32(objTrainingsModels.Training_Start_Date.ToString("mm"));
                       // int End_time = Convert.ToInt32(objTrainingsModels.Expected_Date_Completion.ToString("HH")) * 60 + Convert.ToInt32(objTrainingsModels.Expected_Date_Completion.ToString("mm"));
                       // decimal duration = (End_time - Start_time);
                       // int minutes = Convert.ToInt32(duration) / 60;
                       // decimal seconds = duration % 60;
                       // ViewBag.minutes = minutes;
                       // ViewBag.seconds = seconds;
                       //}
                        var result = objTrainingsModels.Expected_Date_Completion - objTrainingsModels.Training_Start_Date;
                        ViewBag.days = result.Days;
                        ViewBag.minutes = result.Hours;
                        ViewBag.seconds = result.Minutes;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }

                        //    if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                        //    {
                        //        objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                        //    }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_Actual_Date = dtDatetime;
                            }
                            if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                            }

                          //  ViewBag.RequestStatus = objGlobaldata.GetConstantValue("RequestStatus");
                            ViewBag.Training_Cert_Status = objTrainingsModels.GetMultiTrainingCertificateStatusList();
                           // ViewBag.Training_Attendance = objGlobaldata.GetConstantValue("YesNo");
                          //  ViewBag.Training_Effect_Eval_Method = objTrainingsModels.GetMultiTrainingEvaluationList();
                          //  ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                            ViewBag.EmpLists = objGlobaldata.GetHrEmpListByDept(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString());

                          // ViewBag.ApprovedBy = objGlobaldata.GetReviewer();//Reviewer
                          //  ViewBag.Training_Status = objTrainingsModels.GetMultiTrainingStatusList();

                        if ((dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == "" || dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == null)
                            && dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString() != "")
                        {
                            ViewBag.chkAll = true;
                        }

                        return View(objTrainingsModels);                    
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                    }
                }
                else
                {
                    TempData["alertdata"] = "Training Id cannot be null";
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEventsList");
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult TrainingEventsEdit(TrainingsModels objTrainingsModels, FormCollection form, IEnumerable<HttpPostedFileBase> Upload, IEnumerable<HttpPostedFileBase> Attendees_Upload)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                string QCDelete1 = Request.Form["QCDocsValselectall1"];

                IList<HttpPostedFileBase> UploadList = (IList<HttpPostedFileBase>)Upload;
                IList<HttpPostedFileBase> Attendees_UploadList = (IList<HttpPostedFileBase>)Attendees_Upload;

                objTrainingsModels.Sourceof_Training = form["Sourceof_Training"];
                //objTrainingsModels.Training_Status = form["Training_Status"];
                //objTrainingsModels.Training_Cert_Status = form["Training_Cert_Status"];
                objTrainingsModels.ApprovedBy = form["ApprovedBy"];
                objTrainingsModels.Attendees = form["Attendees"];
                objTrainingsModels.Trainer_Name = form["Trainer_Name"];
                objTrainingsModels.Service_provider = form["Service_provider"];
                objTrainingsModels.DeptId = form["DeptId"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Training_Planned_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Planned_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Start_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Start_Date = dateValue;
                }
                //if (DateTime.TryParse(form["Expected_Date_Completion"], out dateValue) == true)
                //{
                //    objTrainingsModels.Expected_Date_Completion = dateValue;
                //}
                //if (DateTime.TryParse(form["Training_ReSchedule_Date"], out dateValue) == true)
                //{
                //    objTrainingsModels.Training_ReSchedule_Date = dateValue;
                //}
                if (DateTime.TryParse(form["Training_Effect_Eval_Plan_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Effect_Eval_Plan_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Actual_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Actual_Date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objTrainingsModels.Training_Actual_Date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (DateTime.TryParse(form["Training_Actual_Completion_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Actual_Completion_Date = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour1"] != null && int.TryParse(form["PlanTimeInHour1"], out iHr) &&
                        form["PlanTimeInMin1"] != null && int.TryParse(form["PlanTimeInMin1"], out iMin))
                    {
                        objTrainingsModels.Training_Actual_Completion_Date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                if (UploadList[0] != null)
                {
                    objTrainingsModels.Upload = "";
                    foreach (var file in Upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "Training" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objTrainingsModels.Upload = objTrainingsModels.Upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objTrainingsModels.Upload = objTrainingsModels.Upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objTrainingsModels.Upload = objTrainingsModels.Upload + "," + form["QCDocsVal"];
                    objTrainingsModels.Upload = objTrainingsModels.Upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && UploadList[0] == null)
                {
                    objTrainingsModels.Upload = null;
                }
                else if (form["QCDocsVal"] == null && UploadList[0] == null)
                {
                    objTrainingsModels.Upload = null;
                }
                if (Attendees_UploadList != null)
                {
                    if (Attendees_UploadList[0] != null)
                {
                    objTrainingsModels.Attendees_Upload = "";
                    foreach (var file in Attendees_Upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = "Training" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objTrainingsModels.Attendees_Upload = objTrainingsModels.Attendees_Upload + "," + "~/DataUpload/MgmtDocs/Training/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objTrainingsModels.Attendees_Upload = objTrainingsModels.Attendees_Upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal1"] != null && form["QCDocsVal1"] != "")
                {
                    objTrainingsModels.Attendees_Upload = objTrainingsModels.Attendees_Upload + "," + form["QCDocsVal1"];
                    objTrainingsModels.Attendees_Upload = objTrainingsModels.Attendees_Upload.Trim(',');
                }
                else if (form["QCDocsVal1"] == null && QCDelete1 != null && Attendees_UploadList[0] == null)
                {
                    objTrainingsModels.Attendees_Upload = null;
                }
                else if (form["QCDocsVal1"] == null && Attendees_UploadList[0] == null)
                {
                    objTrainingsModels.Attendees_Upload = null;
                }
                         
            }
                if (objTrainingsModels.FunUpdateTrainingEvents(objTrainingsModels))
                {
                    TempData["Successdata"] = "Traning details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult TrainingEventsInfo(int id)
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();
            try
            {                
                    UserCredentials objUser = new UserCredentials();
                    objUser = objGlobaldata.GetCurrentUserSession();
                    string sRolename = objGlobaldata.GetRoleName(objUser.role);

                    //ViewBag.Role = sRolename;
                    //ViewBag.CurrentEmpName = objUser.firstname;
                    //ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                    string sTrainingID = Request.QueryString["TrainingID"];

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic,TopicContent ,Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date,Course_Material, Training_Actual_Completion_Date, ApprovedBy, Traning_BeforeDate,DeptId,Category,Service_provider,Ext_Trainer_Name," +
                                "Upload,Attendees_Upload,RequestStatus,report_no from t_trainings where TrainingID='" + id + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = (dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString()),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Category = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Category"].ToString()),
                            Service_provider = dsTrainingsList.Tables[0].Rows[0]["Service_provider"].ToString(),
                            Ext_Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                            Upload = dsTrainingsList.Tables[0].Rows[0]["Upload"].ToString(),
                            Attendees_Upload = dsTrainingsList.Tables[0].Rows[0]["Attendees_Upload"].ToString(),
                            report_no = dsTrainingsList.Tables[0].Rows[0]["report_no"].ToString(),
                        };

                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Planned_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Planned_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Start_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Start_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Expected_Date_Completion"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Expected_Date_Completion = dtDatetime;
                        }

                        var result = objTrainingsModels.Expected_Date_Completion - objTrainingsModels.Training_Start_Date;
                        ViewBag.days = result.Days;
                        ViewBag.minutes = result.Hours;
                        ViewBag.seconds = result.Minutes;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }
                        //if ((dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == "" || dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == null)
                        //    && dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString() != "")
                        //{
                        //    ViewBag.chkAll = true;
                        //}

                        return View(objTrainingsModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                    }  
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEventsList");
        }

        [AllowAnonymous]
        public ActionResult TrainingEventsDetails()
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();
            try
            {
                if (Request.QueryString["TrainingID"] != null && Request.QueryString["TrainingID"] != "")
                {
                    UserCredentials objUser = new UserCredentials();
                    objUser = objGlobaldata.GetCurrentUserSession();
                    string sRolename = objGlobaldata.GetRoleName(objUser.role);

                    //ViewBag.Role = sRolename;
                    //ViewBag.CurrentEmpName = objUser.firstname;
                    //ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                    string sTrainingID = Request.QueryString["TrainingID"];

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                                      
                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic,TopicContent ,Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date,Course_Material, Training_Actual_Completion_Date, ApprovedBy, Traning_BeforeDate,DeptId,Category,Service_provider,Ext_Trainer_Name," +
                                "Upload,Attendees_Upload,RequestStatus,report_no from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = (dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString()),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Category = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Category"].ToString()),
                            Service_provider = dsTrainingsList.Tables[0].Rows[0]["Service_provider"].ToString(),
                            Ext_Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                            Upload = dsTrainingsList.Tables[0].Rows[0]["Upload"].ToString(),
                            Attendees_Upload = dsTrainingsList.Tables[0].Rows[0]["Attendees_Upload"].ToString(),
                            report_no = dsTrainingsList.Tables[0].Rows[0]["report_no"].ToString(),
                        };

                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Planned_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Planned_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Start_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Start_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Expected_Date_Completion"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Expected_Date_Completion = dtDatetime;
                        }
                        
                        var result = objTrainingsModels.Expected_Date_Completion - objTrainingsModels.Training_Start_Date;
                        ViewBag.days = result.Days;
                        ViewBag.minutes = result.Hours;
                        ViewBag.seconds = result.Minutes;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }                        
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }
                        if ((dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == "" || dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == null)
                            && dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString() != "")
                        {
                            ViewBag.chkAll = true;
                        }

                        return View(objTrainingsModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                    }
                }
                else
                {
                    TempData["alertdata"] = "Training Id cannot be null";
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingEventsList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult TrainingEventsDetails(FormCollection form)
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();
            string sTrainingID = form["TrainingID"];
            try
            {
                
                //UserCredentials objUser = new UserCredentials();
                //    objUser = objGlobaldata.GetCurrentUserSession();
                //    string sRolename = objGlobaldata.GetRoleName(objUser.role);

                    //ViewBag.Role = sRolename;
                    //ViewBag.CurrentEmpName = objUser.firstname;
                    //ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                   

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic,TopicContent ,Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date,Course_Material, Training_Actual_Completion_Date, ApprovedBy, Traning_BeforeDate,DeptId,Category,Service_provider,Ext_Trainer_Name," +
                                "Upload,Attendees_Upload,RequestStatus,report_no from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = (dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString()),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Category = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Category"].ToString()),
                            Service_provider = dsTrainingsList.Tables[0].Rows[0]["Service_provider"].ToString(),
                            Ext_Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Ext_Trainer_Name"].ToString(),
                            Upload = dsTrainingsList.Tables[0].Rows[0]["Upload"].ToString(),
                            Attendees_Upload = dsTrainingsList.Tables[0].Rows[0]["Attendees_Upload"].ToString(),
                            report_no = dsTrainingsList.Tables[0].Rows[0]["report_no"].ToString(),
                        };

                        string[] AttedeesCount = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()).Split(',');
                        objTrainingsModels.Attended_no = AttedeesCount.Length;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Planned_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Planned_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Start_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Start_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Expected_Date_Completion"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Expected_Date_Completion = dtDatetime;
                        }

                        var result = objTrainingsModels.Expected_Date_Completion - objTrainingsModels.Training_Start_Date;
                        ViewBag.days = result.Days;
                        ViewBag.minutes = result.Hours;
                        ViewBag.seconds = result.Minutes;

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }

                    string logged_Id = objGlobaldata.GetCurrentUserSession().empid;

                    CompanyModels objCompany = new CompanyModels();
                    dsTrainingsList = objCompany.GetCompanyDetailsForReport(dsTrainingsList);
                    {
                        dsTrainingsList = objGlobaldata.GetReportDetails(dsTrainingsList, objTrainingsModels.report_no, logged_Id, "TRAINING REPORT");
                    }

                    ViewBag.CompanyInfo = dsTrainingsList;
                    ViewBag.objTraining = objTrainingsModels;
                  }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                    }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("TrainingsDetailsToPDF", new { TrainingID = sTrainingID })
            {
                //FileName = "TrainingsDetailsToPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        public ActionResult TrainingEventsApproveReject(string TrainingID, string ApprovedBy, int iStatus, string PendingFlg)
        {
            try
            {
                TrainingsModels objTrainingsModels = new TrainingsModels();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().firstname;
                               
                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";
                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";

                }
                if (objTrainingsModels.FunTrainingEventApproveOrReject(TrainingID, iStatus))
                {
                    TempData["Successdata"] = "Training" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
           
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("TrainingEventsList");
            }
        }
      
        public JsonResult TrainingEventsApproveRejectNoty(string TrainingID, string ApprovedBy, int iStatus, string PendingFlg)
        {
            try
            {
                TrainingsModels objTrainingsModels = new TrainingsModels();

                if (objTrainingsModels.FunTrainingEventApproveOrReject(TrainingID, iStatus))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }              
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingEventsApproveRejectNoty: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }
               
    }
}