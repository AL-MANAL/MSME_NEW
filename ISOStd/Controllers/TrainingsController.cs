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
    public class TrainingsController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        TrainingsModels objTrainings = new TrainingsModels();

        public TrainingsController()
        {
            ViewBag.Menutype = "Trainings";
        }
        //
        // GET: /Trainings/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Trainings/AddTrainings

        [AllowAnonymous]
        public ActionResult AddTrainings()
        {
            return InitilizeAddTrainings();
        }

        // GET: /Trainings/InitilizeAddTrainings

        private ActionResult InitilizeAddTrainings()
        {
            try
            {
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.ApprovedBy = objGlobaldata.GetApprover();
                ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddTrainings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Returns only Department List
        // GET: /Trainings/FunAttendeesList

        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            //MultiSelectList lstEmp = objGlobaldata.GetDeptHeadList(DeptId);
            MultiSelectList lstEmp = objGlobaldata.GetHrEmpListByDept(DeptId);
            return Json(lstEmp);
        }


        public ActionResult FunAllEmpList()
        {
            MultiSelectList lstEmp = objGlobaldata.GetHrEmployeeListbox();
            return Json(lstEmp);
        }

        // POST: /Trainings/AddTrainings

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddTrainings(TrainingsModels objTrainings, FormCollection form)
        {
            try
            {
                if (objTrainings != null)
                {
                    //objTrainings.Training_Status = form["RequestStatus"];
                    objTrainings.Training_Topic = form["Training_Topic"];
                    objTrainings.Attendees = form["Attendees"];
                    objTrainings.ApprovedBy = form["ApprovedBy"];

                    objTrainings.Training_Requested_By = objGlobaldata.GetCurrentUserSession().empid;

                    objTrainings.DeptId = form["DeptId"];
                    objTrainings.TopicContent = form["TopicContent"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Traning_BeforeDate"], out dateValue) == true)
                    {
                        objTrainings.Traning_BeforeDate = dateValue;
                    }

                    if (objTrainings.FunAddTrainings(objTrainings))
                    {
                        TempData["Successdata"] = "Added Traning details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingsList");
        }


        // GET: /Trainings/TrainingsList

        [AllowAnonymous]
        public ActionResult TrainingsList(string Training_Topic, string TrainingStartfromDate, string TrainingStartToDate, string chkAll, string Approvestatus, int? page, string branch_name)
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

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status,Attended_no, "
                    + " (case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end) as RequestStatus"
                    + ", ApprovedBy,Training_Status from t_trainings where Active=1";

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                string sRolename = objGlobaldata.GetRoleName(objUser.role);

                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                ViewBag.Training_Topic = objGlobaldata.GetDropdownList("Training Topic");

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (Training_Topic != null && Training_Topic != "")
                    {
                        ViewBag.Training_TopicVal = Training_Topic;
                        sSearchtext = sSearchtext + " ( FIND_IN_SET (" + Training_Topic + ",Training_Topic) )";
                    }


                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (TrainingStartfromDate != null && DateTime.TryParse(TrainingStartfromDate, out dtFromdate) && DateTime.TryParse(TrainingStartToDate, out dtToDate))
                    {
                        ViewBag.AuditfromDate = TrainingStartfromDate;
                        ViewBag.AuditToDate = TrainingStartToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and requestedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and requestedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }

                    if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "" && Approvestatus != "Select")
                    {
                        ViewBag.ApprovestatusVal = Approvestatus;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (RequestStatus ='" + Approvestatus + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (RequestStatus ='" + Approvestatus + "')";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
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
                            Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[i]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Requested_By"].ToString()),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[i]["Reasonfor_Training"].ToString(),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[i]["RequestStatus"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            Training_Status = objtr.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Status"].ToString()),
                            Training_Attended_no = dsTrainingsList.Tables[0].Rows[i]["Attended_no"].ToString()
                        };
                        DateTime dtTraning_BeforeDate;
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[i]["Traning_BeforeDate"].ToString(), out dtTraning_BeforeDate))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtTraning_BeforeDate;
                        }
                        objTrainingsModelsList.TrainingsMList.Add(objTrainingsModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrainingsModelsList.TrainingsMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult TrainingsListSearch(string Training_Topic, string TrainingStartfromDate, string TrainingStartToDate, string chkAll, string Approvestatus, int? page, string branch_name)
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

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status,Attended_no, "
                    + " (case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end) as RequestStatus"
                    + ", ApprovedBy,Training_Status from t_trainings where Active=1";

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                string sRolename = objGlobaldata.GetRoleName(objUser.role);

                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("RequestStatus");
                ViewBag.Training_Topic = objGlobaldata.GetDropdownList("Training Topic");

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (Training_Topic != null && Training_Topic != "")
                    {
                        ViewBag.Training_TopicVal = Training_Topic;
                        sSearchtext = sSearchtext + " ( FIND_IN_SET (" + Training_Topic + ",Training_Topic) )";
                    }


                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (TrainingStartfromDate != null && DateTime.TryParse(TrainingStartfromDate, out dtFromdate) && DateTime.TryParse(TrainingStartToDate, out dtToDate))
                    {
                        ViewBag.AuditfromDate = TrainingStartfromDate;
                        ViewBag.AuditToDate = TrainingStartToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and requestedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and requestedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }

                    if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "" && Approvestatus != "Select")
                    {
                        ViewBag.ApprovestatusVal = Approvestatus;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and (RequestStatus ='" + Approvestatus + "')";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and (RequestStatus ='" + Approvestatus + "')";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
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
                            Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[i]["Training_Topic"].ToString()),
                            Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Requested_By"].ToString()),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[i]["Reasonfor_Training"].ToString(),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[i]["RequestStatus"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            Training_Status = objtr.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[i]["Training_Status"].ToString()),
                            Training_Attended_no = dsTrainingsList.Tables[0].Rows[i]["Attended_no"].ToString()
                        };
                        DateTime dtTraning_BeforeDate;
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[i]["Traning_BeforeDate"].ToString(), out dtTraning_BeforeDate))
                        {
                            objTrainingsModels.Traning_BeforeDate = dtTraning_BeforeDate;
                        }
                        objTrainingsModelsList.TrainingsMList.Add(objTrainingsModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        public ActionResult TrainingsApproveReject(string TrainingID, string ApprovedBy, int iStatus, string PendingFlg)
        {
            try
            {
                TrainingsModels objTrainingsModels = new TrainingsModels();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().firstname;

                /* if (user == ApprovedBy)
                 {
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

                     }*/
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
                if (objTrainingsModels.FunTrainingsApproveOrReject(TrainingID, iStatus))
                {
                    //  TempData["Successdata"] = "Training " + sStatus;
                    TempData["Successdata"] = "Training" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            /*else if (user == ApprovedBy)
            {
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
                if (objTrainingsModels.FunTrainingsApproveOrReject(TrainingID, iStatus))
                {
                    //TempData["Successdata"] = "Training " + sStatus;
                    TempData["Successdata"] = "Training"+sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "Access Denied";
            }

        }*/
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("TrainingsList");
            }
        }
            

        // GET: /Trainings/TrainingsRequestEdit

        [AllowAnonymous]
        public ActionResult TrainingsRequestEdit()
        {
            try
            {
                if (Request.QueryString["TrainingID"] != null && Request.QueryString["TrainingID"] != "")
                {
                    string sTrainingID = Request.QueryString["TrainingID"];
                    ViewBag.TrainingID = sTrainingID;
                    UserCredentials objUser = new UserCredentials();
                    objUser = objGlobaldata.GetCurrentUserSession();

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, RequestStatus,"
                        + " ApprovedBy,TopicContent,Attended_no from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        if (objUser.empid == dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()
                            || objUser.empid == dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString())
                        {
                            DateTime dtTraning_BeforeDate = Convert.ToDateTime(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString());

                            TrainingsModels objTrainingsModels = new TrainingsModels
                            {
                                TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                                DeptId = (dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                                Attendees = (dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                                Training_Topic = (dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                                Training_Requested_By = dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString(),
                                Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                                RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                                // ApprovedBy = dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString(),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                                TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                                Training_Attended_no = dsTrainingsList.Tables[0].Rows[0]["Attended_no"].ToString(),
                                Traning_BeforeDate = dtTraning_BeforeDate
                            };
                            //ViewBag.RequestStatus = objGlobaldata.GetConstantValue("RequestStatus");
                            ViewBag.EmpLists = objGlobaldata.GetHrEmpListByDept(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString());
                            ViewBag.Topics = objGlobaldata.GetDropdownList("Training Topic");
                            ViewBag.ApprovedBy = objGlobaldata.GetApprover();//Reviewer
                            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();

                            if ((dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == "" || dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString() == null)
                                && dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString() != "")
                            {
                                ViewBag.chkAll = true;
                            }
                            return View(objTrainingsModels);
                        }
                        else
                        {
                            ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("TrainingsList");
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("TrainingsList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Training Id cannot be null";
                    return RedirectToAction("TrainingsList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsRequestEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingsList");

        }

        //POST: /Trainings/TrainingsRequestEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult TrainingsRequestEdit(TrainingsModels objTrainings, FormCollection form)
        {
            try
            {
                if (objTrainings != null)
                {
                    objTrainings.TrainingID = form["TrainingID"];
                    objTrainings.Training_Topic = form["Training_Topic"];
                    objTrainings.Attendees = form["Attendees"];
                    objTrainings.ApprovedBy = form["ApprovedBy"];

                    objTrainings.Training_Requested_By = objGlobaldata.GetCurrentUserSession().empid;

                    objTrainings.DeptId = form["DeptId"];
                    objTrainings.TopicContent = form["TopicContent"];
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Traning_BeforeDate"], out dateValue) == true)
                    {
                        objTrainings.Traning_BeforeDate = dateValue;
                    }

                    if (objTrainings.FunUpdateTrainings(objTrainings))
                    {
                        TempData["Successdata"] = "Traning details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsRequestEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingsList");
        }

        // GET: /Trainings/TrainingsDetails

        [AllowAnonymous]
        public ActionResult TrainingsDetails()
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();

            try
            {
                if (Request.QueryString["TrainingID"] != null && Request.QueryString["TrainingID"] != "")
                {
                    string sTrainingID = Request.QueryString["TrainingID"];
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "SELECT TrainingID, Attendees,DeptId, Training_Topic,TopicContent, Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date, Training_Actual_Completion_Date, "
                                + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end)"
                                + " as RequestStatus, ApprovedBy,  Traning_BeforeDate,Course_Material  from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                            Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            Training_Status = objTrainingsModels.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString()),
                            Training_Cert_Status = objTrainingsModels.GetTrainingCertificateStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()),
                            Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                            Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                            Training_Effect_Eval_Method = objTrainingsModels.GetTrainingEvaluationNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()),
                            Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                            Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString(),
                            TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                        };

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

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }

                        return View(objTrainingsModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("TrainingsList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Training Id cannot be null";
                    return RedirectToAction("TrainingsList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingsList");
        }


        [AllowAnonymous]
        public ActionResult TrainingsInfo(int id)
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();

            try
            {
                string sSqlstmt = "SELECT TrainingID, Attendees,DeptId, Training_Topic,TopicContent, Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                            + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                            + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                            + ",Training_Actual_Date, Training_Actual_Completion_Date, "
                            + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end)"
                            + " as RequestStatus, ApprovedBy,  Traning_BeforeDate,Course_Material  from t_trainings where TrainingID='" + id + "'";

                DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtDatetime;

                    objTrainingsModels = new TrainingsModels
                    {
                        TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                        DeptId = objGlobaldata.GetMultiDeptNameById(dsTrainingsList.Tables[0].Rows[0]["DeptId"].ToString()),
                        Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                        Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                        Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                        Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                        ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                        Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                        Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                        Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                        Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                        Training_Status = objTrainingsModels.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString()),
                        Training_Cert_Status = objTrainingsModels.GetTrainingCertificateStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()),
                        Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                        Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                        Training_Effect_Eval_Method = objTrainingsModels.GetTrainingEvaluationNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()),
                        Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                        Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString(),
                        TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                    };

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

                    if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                    {
                        objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                    }

                    if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                    {
                        objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                    }

                    if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                    {
                        objTrainingsModels.Training_Actual_Date = dtDatetime;
                    }
                    if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                    {
                        objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                    }

                    return View(objTrainingsModels);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("TrainingsList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingsList");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingsDetails(FormCollection form)
        {
            string sTrainingID = form["TrainingID"];
            TrainingsModels objTrainingsModels = new TrainingsModels();
            try
            {
                if (sTrainingID != "")
                {
                    //string sTrainingID = Request.QueryString["TrainingID"];
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic, Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date, Training_Actual_Completion_Date, "
                                + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end)"
                                + " as RequestStatus, ApprovedBy,  Traning_BeforeDate,Course_Material  from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;
                        DateTime dtTraning_BeforeDate = Convert.ToDateTime(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString());

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                            Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                            Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Traning_BeforeDate = dtTraning_BeforeDate,
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                            Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            Training_Status = objTrainingsModels.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString()),
                            Training_Cert_Status = objTrainingsModels.GetTrainingCertificateStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()),
                            Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                            Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                            Training_Effect_Eval_Method = objTrainingsModels.GetTrainingEvaluationNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()),
                            Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                            Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString()
                        };

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

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }
                        ViewBag.objTraining = objTrainingsModels;
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingsDetails: " + ex.ToString());
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


        [AllowAnonymous]
        public ActionResult TrainingsDetailsToPDF(string sTrainingID)
        {
            TrainingsModels objTrainingsModels = new TrainingsModels();

            try
            {
                //if (Request.QueryString["TrainingID"] != null && Request.QueryString["TrainingID"] != "")
                //{
                if (sTrainingID != "")
                {
                    //string sTrainingID = Request.QueryString["TrainingID"];
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic, Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date, Training_Actual_Completion_Date, "
                                + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end)"
                                + " as RequestStatus, ApprovedBy,  Traning_BeforeDate,Course_Material  from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtDatetime;
                        DateTime dtTraning_BeforeDate = Convert.ToDateTime(dsTrainingsList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString());

                        objTrainingsModels = new TrainingsModels
                        {
                            TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                            Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                            Training_Topic = dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString(),
                            Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                            Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                            Traning_BeforeDate = dtTraning_BeforeDate,
                            Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                            Sourceof_Training = dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString(),
                            Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                            Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                            Training_Status = dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString(),
                            Training_Cert_Status = dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString(),
                            Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                            Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                            Training_Effect_Eval_Method = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString(),
                            Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                            Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString()
                        };

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

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                        }

                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Date = dtDatetime;
                        }
                        if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                        {
                            objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                        }
                        ViewBag.objTraining = objTrainingsModels;

                        return View();
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return View();
                    }
                }
                else
                {
                    TempData["alertdata"] = "Training Id cannot be null";
                    return View();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsDetailsToPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        // GET: /Trainings/TrainingsEdit


        [AllowAnonymous]
        public ActionResult TrainingsEdit()
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

                    string sSqlstmt = "SELECT TrainingID, Attendees, Training_Topic,TopicContent ,Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                                + "Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                                + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                                + ",Training_Actual_Date,Course_Material, Training_Actual_Completion_Date,(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end) as RequestStatus, ApprovedBy,  Traning_BeforeDate from t_trainings where TrainingID='" + sTrainingID + "'";

                    DataSet dsTrainingsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrainingsList.Tables.Count > 0 && dsTrainingsList.Tables[0].Rows.Count > 0)
                    {
                        string CurrentUserId = "";

                        CurrentUserId = objGlobaldata.GetCurrentUserSession().empid;

                        if (dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString() == "Approved"
                            && (CurrentUserId == dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString() ||
                                CurrentUserId == dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()))
                        {
                            DateTime dtDatetime;

                            objTrainingsModels = new TrainingsModels
                            {
                                TrainingID = dsTrainingsList.Tables[0].Rows[0]["TrainingID"].ToString(),
                                Attendees = objGlobaldata.GetMultiHrEmpNameById(dsTrainingsList.Tables[0].Rows[0]["Attendees"].ToString()),
                                Training_Topic = objGlobaldata.GetDropdownitemById(dsTrainingsList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                                Expected_Duration = dsTrainingsList.Tables[0].Rows[0]["Expected_Duration"].ToString(),
                                Training_Requested_By = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Requested_By"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsTrainingsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                                RequestStatus = dsTrainingsList.Tables[0].Rows[0]["RequestStatus"].ToString(),
                                Reasonfor_Training = dsTrainingsList.Tables[0].Rows[0]["Reasonfor_Training"].ToString(),
                                Sourceof_Training = objTrainingsModels.GetTrainingSourceNameById(dsTrainingsList.Tables[0].Rows[0]["Sourceof_Training"].ToString()),
                                Trainer_Name = dsTrainingsList.Tables[0].Rows[0]["Trainer_Name"].ToString(),
                                Venue = dsTrainingsList.Tables[0].Rows[0]["Venue"].ToString(),
                                Training_Status = objTrainingsModels.GetTrainingStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Status"].ToString()),
                                Training_Cert_Status = objTrainingsModels.GetTrainingCertificateStatusNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()),
                                Training_Attendance = dsTrainingsList.Tables[0].Rows[0]["Training_Attendance"].ToString(),
                                Reasons_for_Not_Completed = dsTrainingsList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString(),
                                Training_Effect_Eval_Method = objTrainingsModels.GetTrainingEvaluationNameById(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()),
                                Training_Effect_Eval_Record_Ref = dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Record_Ref"].ToString(),
                                TopicContent = dsTrainingsList.Tables[0].Rows[0]["TopicContent"].ToString(),
                                Course_Material = dsTrainingsList.Tables[0].Rows[0]["Course_Material"].ToString()
                            };

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

                            if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_Effect_Eval_Plan_Date = dtDatetime;
                            }

                            if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_ReSchedule_Date = dtDatetime;
                            }

                            if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_Actual_Date = dtDatetime;
                            }
                            if (DateTime.TryParse(dsTrainingsList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtDatetime))
                            {
                                objTrainingsModels.Training_Actual_Completion_Date = dtDatetime;
                            }

                            ViewBag.Sourceof_Training = objGlobaldata.GetDropdownList("Training-SourceList");
                            ViewBag.RequestStatus = objGlobaldata.GetConstantValue("RequestStatus");
                            ViewBag.Training_Cert_Status = objTrainingsModels.GetMultiTrainingCertificateStatusList();
                            ViewBag.Training_Attendance = objGlobaldata.GetConstantValue("YesNo");
                            ViewBag.Training_Effect_Eval_Method = objTrainingsModels.GetMultiTrainingEvaluationList();
                            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                            ViewBag.ApprovedBy = objGlobaldata.GetReviewer();//Reviewer
                            ViewBag.Training_Status = objTrainingsModels.GetMultiTrainingStatusList();

                            return View(objTrainingsModels);
                        }
                        else
                        {
                            TempData["alertdata"] = "Access Denied";
                        }
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return RedirectToAction("TrainingsList");
        }

        //POST: /Trainings/TrainingsEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingsEdit(TrainingsModels objTrainingsModels, FormCollection form,
            HttpPostedFileBase Training_Attendance, HttpPostedFileBase Training_Effect_Eval_Record_Ref
            , HttpPostedFileBase Course_Material)
        {
            try
            {
                objTrainingsModels.Sourceof_Training = form["Sourceof_Training"];
                objTrainingsModels.Training_Status = form["Training_Status"];
                objTrainingsModels.Training_Cert_Status = form["Training_Cert_Status"];
                objTrainingsModels.Training_Effect_Eval_Method = form["Training_Effect_Eval_Method"];
                //objTrainingsModels.Attendees = form["Attendees"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Training_Planned_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Planned_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Start_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Start_Date = dateValue;
                }
                if (DateTime.TryParse(form["Expected_Date_Completion"], out dateValue) == true)
                {
                    objTrainingsModels.Expected_Date_Completion = dateValue;
                }
                if (DateTime.TryParse(form["Training_ReSchedule_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_ReSchedule_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Effect_Eval_Plan_Date"], out dateValue) == true)
                {
                    objTrainingsModels.Training_Effect_Eval_Plan_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Actual_Date"], out dateValue) == true)
                {
                    objTrainings.Training_Actual_Date = dateValue;
                }
                if (DateTime.TryParse(form["Training_Actual_Completion_Date"], out dateValue) == true)
                {
                    objTrainings.Training_Actual_Completion_Date = dateValue;
                }

                if (Training_Attendance != null && Training_Attendance.ContentLength > 0)
                {
                    try
                    {
                        string sVirtualPath = "~/DataUpload/MgmtDocs/Training";
                        string spath = Path.Combine(Server.MapPath(sVirtualPath), Path.GetFileName(Training_Attendance.FileName));
                        string sFilename = objTrainingsModels.Training_Topic + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetExtension(Training_Attendance.FileName);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Training_Attendance.SaveAs(sFilepath + "/" + sFilename);
                        objTrainingsModels.Training_Attendance = sVirtualPath + "/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
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

                if (Training_Effect_Eval_Record_Ref != null && Training_Effect_Eval_Record_Ref.ContentLength > 0)
                {
                    try
                    {
                        string sVirtualPath = "~/DataUpload/MgmtDocs/Training";
                        string spath = Path.Combine(Server.MapPath(sVirtualPath), Path.GetFileName(Training_Effect_Eval_Record_Ref.FileName));
                        string sFilename = "Training_Eval_Report" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetExtension(Training_Effect_Eval_Record_Ref.FileName);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Training_Effect_Eval_Record_Ref.SaveAs(sFilepath + "/" + sFilename);
                        objTrainingsModels.Training_Effect_Eval_Record_Ref = sVirtualPath + "/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
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
                if (Course_Material != null && Course_Material.ContentLength > 0)
                {
                    try
                    {
                        string sVirtualPath = "~/DataUpload/MgmtDocs/Training";
                        string spath = Path.Combine(Server.MapPath(sVirtualPath), Path.GetFileName(Course_Material.FileName));
                        string sFilename = "Training_course_Report" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetExtension(Course_Material.FileName);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Course_Material.SaveAs(sFilepath + "/" + sFilename);
                        objTrainingsModels.Course_Material = sVirtualPath + "/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
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
                if (objTrainingsModels.FunUpdatePlanTrainings(objTrainingsModels))
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingsList");
        }

        //Returns only training Attendees List

        // GET: /Trainings/FunAttendeesList

        public ActionResult FunGetAttendeesList(string TrainingId)
        {
            List<string> lstAuditee = objTrainings.GetAttendeesList(TrainingId);
            return Json(lstAuditee);
        }
        
        [AllowAnonymous]
        public JsonResult TrainingDocDelete(FormCollection form)
        {
            try
            {

                   if (form["TrainingID"] != null && form["TrainingID"] != "")
                    {

                        TrainingsModels Doc = new TrainingsModels();
                        string sTrainingID = form["TrainingID"];


                        if (Doc.FunDeleteTrainingDoc(sTrainingID))
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
                objGlobaldata.AddFunctionalLog("Exception in TrainingDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
         
        public JsonResult TrainingsApproveRejectNoty(string TrainingID, string ApprovedBy, int iStatus, string PendingFlg)
        {
            try
            {
                TrainingsModels objTrainingsModels = new TrainingsModels();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().firstname;

                /* if (user == ApprovedBy)
                 {
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

                     }*/
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
                if (objTrainingsModels.FunTrainingsApproveOrReject(TrainingID, iStatus))
                {
                    //  TempData["Successdata"] = "Training " + sStatus;
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }
            /*else if (user == ApprovedBy)
            {
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
                if (objTrainingsModels.FunTrainingsApproveOrReject(TrainingID, iStatus))
                {
                    //TempData["Successdata"] = "Training " + sStatus;
                    TempData["Successdata"] = "Training"+sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "Access Denied";
            }

        }*/
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingsApprove: " + ex.ToString());
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

        //Training Events

        [AllowAnonymous]
        public ActionResult IssueTrainingCertificate()
        {
            return InitilizeIssueTrainingCertificate();
        }

        private ActionResult InitilizeIssueTrainingCertificate()
        {
            string sTrainingID = Request.QueryString["TrainingID"];
            string topic = objTrainings.GetTopic(sTrainingID);
            ViewBag.Training_Topic = /*objGlobaldata.GetDropdownitemById*/(topic);
            ViewBag.TrainingID = sTrainingID;
            if (objTrainings.GetAttendees(sTrainingID) != "" && objTrainings.GetAttendees(sTrainingID) != null)
            {
                ViewBag.Person_Name = objGlobaldata.GetHrMultiEmployeeListByID(objTrainings.GetAttendees(sTrainingID));
            }
            else
            {
                ViewBag.Person_Name = "";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IssueTrainingCertificate(TrainingCertificatesModels objCertificates, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                if (objCertificates != null)
                {
                    objCertificates.TrainingId = form["TrainingID"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Cert_Issue_Date"], out dateValue) == true)
                    {
                        objCertificates.Cert_Issue_Date = dateValue;
                    }
                    if (DateTime.TryParse(form["Cert_Exp_Date"], out dateValue) == true)
                    {
                        objCertificates.Cert_Exp_Date = dateValue;
                    }

                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + objCertificates.Cert_Ref + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                            objCertificates.Cert_Upload = "~/DataUpload/MgmtDocs/Training/" + objCertificates.Cert_Ref + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;
                            ViewBag.Message = "File uploaded successfully";
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

                    if (objCertificates.FunAddTrainingCertificates(objCertificates))
                    {
                        TempData["Successdata"] = "Traning Certificate details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueTrainingCertificate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("TrainingCertificateList", "Trainings", new { TrainingID = objCertificates.TrainingId });
        }

        [AllowAnonymous]
        public ActionResult TrainingCertificateList(string chkAll, string TrainingID, string Training_Topic, int? page)
        {
            TrainingCertificatesModelsList objTrainingCertificatesModelsList = new TrainingCertificatesModelsList();
            objTrainingCertificatesModelsList.TrainingCertificatesList = new List<TrainingCertificatesModels>();

            //string sTrainingId = Request.QueryString["TrainingID"]; 
            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            try
            {
                ViewBag.TrainingID = TrainingID;
             
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select t_trainings.Training_Topic, ttc.*  from t_trainings , t_training_certificate as ttc where t_trainings.TrainingID=ttc.TrainingID "
                    + "and ttc.TrainingID='" + TrainingID + "'";

                //string sSearchtext = "";
                //if (SearchText != null && SearchText != "")
                //{
                //    ViewBag.SearchText = objGlobaldata.getTrainingTopicIdByName(SearchText);
                //    sSearchtext = sSearchtext + " and (t_trainings.Training_Topic ='" + ViewBag.SearchText + "' or t_trainings.Training_Topic  in (" + ViewBag.SearchText + "))";
                //}
                if (chkAll == null && chkAll != "All")
                {
                    //ViewBag.chkAll = false;
                    //if (Training_Topic != null && Training_Topic != "")
                    //{
                    //    ViewBag.Training_TopicVal = Training_Topic;
                    //    sSqlstmt = sSqlstmt + " and ( FIND_IN_SET (" + Training_Topic + ",Training_Topic) )";
                    //}
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + " order by CertId desc";

                DataSet dsTrainingCertificatesList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsTrainingCertificatesList.Tables.Count > 0 && dsTrainingCertificatesList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTrainingCertificatesList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtCert_Issue_Date = Convert.ToDateTime(dsTrainingCertificatesList.Tables[0].Rows[i]["Cert_Issue_Date"].ToString());
                        DateTime dtCert_Exp_Date = Convert.ToDateTime(dsTrainingCertificatesList.Tables[0].Rows[i]["Cert_Exp_Date"].ToString());

                        try
                        {
                            TrainingCertificatesModels objTrainingsModels = new TrainingCertificatesModels
                            {
                                CertId = Convert.ToInt16(dsTrainingCertificatesList.Tables[0].Rows[i]["CertId"].ToString()),
                                TrainingId = dsTrainingCertificatesList.Tables[0].Rows[i]["TrainingID"].ToString(),
                                Training_Topic = /*objGlobaldata.GetDropdownitemById*/(dsTrainingCertificatesList.Tables[0].Rows[i]["Training_Topic"].ToString()),
                                Cert_Ref = dsTrainingCertificatesList.Tables[0].Rows[i]["Cert_Ref"].ToString(),
                                Person_Name = objGlobaldata.GetEmpHrNameById(dsTrainingCertificatesList.Tables[0].Rows[i]["Person_Name"].ToString()),
                                Cert_Issue_Date = dtCert_Issue_Date,
                                Cert_Exp_Date = dtCert_Exp_Date,
                                Cert_issued_by = dsTrainingCertificatesList.Tables[0].Rows[i]["Cert_issued_by"].ToString(),
                                Cert_Upload = dsTrainingCertificatesList.Tables[0].Rows[i]["Cert_Upload"].ToString()
                            };
                            objTrainingCertificatesModelsList.TrainingCertificatesList.Add(objTrainingsModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in TrainingCertificateList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingCertificateList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objTrainingCertificatesModelsList.TrainingCertificatesList.ToList());
        }

        [AllowAnonymous]
        public ActionResult TrainingCertificateEdit()
        {
            string sCertId = Request.QueryString["CertId"];

            TrainingCertificatesModels objTrainingsModels = new TrainingCertificatesModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                string sSqlstmt = "select t_trainings.Training_Topic, ttc.*  from t_trainings , t_training_certificate as ttc where t_trainings.TrainingID=ttc.TrainingID "
                    + "and CertId='" + sCertId + "'";

                DataSet dsTrainingCertificatesList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsTrainingCertificatesList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtCert_Issue_Date = Convert.ToDateTime(dsTrainingCertificatesList.Tables[0].Rows[0]["Cert_Issue_Date"].ToString());
                    DateTime dtCert_Exp_Date = Convert.ToDateTime(dsTrainingCertificatesList.Tables[0].Rows[0]["Cert_Exp_Date"].ToString());

                    objTrainingsModels = new TrainingCertificatesModels
                    {
                        CertId = Convert.ToInt16(dsTrainingCertificatesList.Tables[0].Rows[0]["CertId"].ToString()),
                        TrainingId = dsTrainingCertificatesList.Tables[0].Rows[0]["TrainingID"].ToString(),
                        Training_Topic = /*objGlobaldata.GetDropdownitemById*/(dsTrainingCertificatesList.Tables[0].Rows[0]["Training_Topic"].ToString()),
                        Cert_Ref = dsTrainingCertificatesList.Tables[0].Rows[0]["Cert_Ref"].ToString(),
                        Person_No = (dsTrainingCertificatesList.Tables[0].Rows[0]["Person_Name"].ToString()),
                        Person_Name = objGlobaldata.GetEmpHrNameById(dsTrainingCertificatesList.Tables[0].Rows[0]["Person_Name"].ToString()),
                        Cert_Issue_Date = dtCert_Issue_Date,
                        Cert_Exp_Date = dtCert_Exp_Date,
                        Cert_issued_by = dsTrainingCertificatesList.Tables[0].Rows[0]["Cert_issued_by"].ToString(),
                        Cert_Upload = dsTrainingCertificatesList.Tables[0].Rows[0]["Cert_Upload"].ToString()
                    };
                    ViewBag.Empid = dsTrainingCertificatesList.Tables[0].Rows[0]["Person_Name"].ToString();
                    ViewBag.TrainingID = dsTrainingCertificatesList.Tables[0].Rows[0]["TrainingID"].ToString();
                    //ViewBag.Person_Name = objGlobaldata.GetHrEmployeeListbox();
                    if (objTrainings.GetAttendees(objTrainingsModels.TrainingId) != "" && objTrainings.GetAttendees(objTrainingsModels.TrainingId) != null)
                    {
                        ViewBag.Person_Name = objGlobaldata.GetHrMultiEmployeeListByID(objTrainings.GetAttendees(objTrainingsModels.TrainingId));
                    }
                    else
                    {
                        ViewBag.Person_Name = "";
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingCertificateEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.CertId = sCertId;
            return View(objTrainingsModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TrainingCertificateEdit(TrainingCertificatesModels objCertificates, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objCertificates.TrainingId = form["TrainingID"];
                objCertificates.Person_Name = form["Person_Name"];
                objCertificates.CertId = Convert.ToInt16(form["CertId"]);

                DateTime dateValue;

                if (DateTime.TryParse(form["Cert_Issue_Date"], out dateValue) == true)
                {
                    objCertificates.Cert_Issue_Date = dateValue;
                }
                if (DateTime.TryParse(form["Cert_Exp_Date"], out dateValue) == true)
                {
                    objCertificates.Cert_Exp_Date = dateValue;
                }

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Training"), Path.GetFileName(file.FileName));
                        string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                        file.SaveAs(sFilepath + "/" + objCertificates.Person_Name + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                        objCertificates.Cert_Upload = "~/DataUpload/MgmtDocs/Training/" + objCertificates.Person_Name + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;
                        ViewBag.Message = "File uploaded successfully";
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
                if (objCertificates.FunUpdateTrainingCertificates(objCertificates))
                {
                    TempData["Successdata"] = "Traning certificate details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in TrainingCertificateEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("TrainingCertificateList", "Trainings", new { TrainingID = objCertificates.TrainingId });
        }

    }
}
