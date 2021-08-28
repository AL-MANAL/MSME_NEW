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

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ToolboxTalkController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        static List<string> objLeaveList = new List<string>();

        public ToolboxTalkController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "ToolboxTalk";
        }
                
        public ActionResult Index()
        {
            return View();
        }        
        
        [AllowAnonymous]
        public ActionResult AddToolboxTalk()
        {
            ToolboxTalkModels objToolbox = new ToolboxTalkModels();
            try
            {
                objToolbox.branch = objGlobaldata.GetCurrentUserSession().division;
                objToolbox.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objToolbox.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objToolbox.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objToolbox.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
               // ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.Topics = objGlobaldata.GetDropdownList("Toolbox Topic");
                //ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.ActivityType = objGlobaldata.GetDropdownList("ToolBox-Activity Type");
                ViewBag.Identifiedhazard = objGlobaldata.GetDropdownList("ToolBox-Hazard");
                ViewBag.PTW = objGlobaldata.GetDropdownList("ToolBox-PTW");
                ViewBag.ComplyPPEType = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddToolboxTalk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objToolbox);
        }


        // POST: /ToolboxTalk/AddToolboxTalk
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToolboxTalk(ToolboxTalkModels objToolboxTalk, FormCollection form, IEnumerable<HttpPostedFileBase> Upload_Report)
        {
            try
            {
                objToolboxTalk.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                objToolboxTalk.Attended_by = form["Attended_by"];
                objToolboxTalk.Conducted_By = form["Conducted_By"];
                objToolboxTalk.Department = form["Department"];
                objToolboxTalk.Topic = form["Topic"];
                objToolboxTalk.Location = form["Location"];
                objToolboxTalk.Activity_type = form["Activity_type"];
                objToolboxTalk.Identified_method = form["Identified_method"];
                objToolboxTalk.ptw = form["ptw"];
                objToolboxTalk.Comply_ppe = form["Comply_ppe"];
                objToolboxTalk.Identified_hazard = form["Identified_hazard"];
                objToolboxTalk.branch = form["branch"];

                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;

                if (form["Talk_DateTime"] != null && DateTime.TryParse(form["Talk_DateTime"], out dateValue) == true)
                {
                    objToolboxTalk.Talk_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objToolboxTalk.Talk_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (Upload_Report != null && files.ContentLength > 0)
                {
                    objToolboxTalk.Upload_Report = "";
                    foreach (var file in Upload_Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "ToolBox" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objToolboxTalk.FunAddToolboxTalk(objToolboxTalk))
                {
                    TempData["Successdata"] = "Added Toolbox Talk details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddToolboxTalk: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ToolboxTalkList");
        }
         
        [AllowAnonymous]
        public JsonResult ToolBoxTalkDocDelete(FormCollection form)
        {
            try
            {                
                        if (form["ToolBox_TalkId"] != null && form["ToolBox_TalkId"] != "")
                        {

                            ToolboxTalkModels Doc = new ToolboxTalkModels();
                            string sToolBox_TalkId = form["ToolBox_TalkId"];


                            if (Doc.FunDeletToolBoxTalkDoc(sToolBox_TalkId))
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
                            TempData["alertdata"] = "Incident Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolBoxTalkDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
          
        [AllowAnonymous]
        public ActionResult ToolboxTalkList(string branch_name)
        {
            ToolboxTalkModelsList objToolboxTalkList = new ToolboxTalkModelsList();
            objToolboxTalkList.lstToolboxTalkModels = new List<ToolboxTalkModels>();

           //ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
           //ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
           ViewBag.Project = objGlobaldata.GetDropdownList("Projects");


            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select ToolBox_TalkId, Talk_DateTime, Conducted_By, Topic, Attendee_No, Report_No,"
                + "Upload_Report, LoggedBy,Project,Location,Department,Attended_by,Activity_type,Identified_method,ptw," +
                "Comply_ppe,Identified_hazard,feedback_tbt,outcome_tbt,branch FROM t_toolbox_talk where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Talk_DateTime desc";

                DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                if (dsToolboxTalk.Tables.Count > 0)
                {   
                    for (int i = 0; i < dsToolboxTalk.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ToolboxTalkModels objToolboxTalk = new ToolboxTalkModels
                            {
                                ToolBox_TalkId = dsToolboxTalk.Tables[0].Rows[i]["ToolBox_TalkId"].ToString(),
                                Conducted_By = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["Conducted_By"].ToString()),
                                Topic =objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Topic"].ToString()),
                                Report_No = dsToolboxTalk.Tables[0].Rows[i]["Report_No"].ToString(),
                                Upload_Report = (dsToolboxTalk.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                Project = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Project"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[i]["Location"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[i]["Department"].ToString()),
                                Attended_by = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["Attended_by"].ToString()),
                                Activity_type = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Activity_type"].ToString()),
                                Identified_method = dsToolboxTalk.Tables[0].Rows[i]["Identified_method"].ToString(),
                                ptw = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["ptw"].ToString()),
                                Comply_ppe = dsToolboxTalk.Tables[0].Rows[i]["Comply_ppe"].ToString(),
                                Identified_hazard = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Identified_hazard"].ToString()),
                                feedback_tbt = dsToolboxTalk.Tables[0].Rows[i]["feedback_tbt"].ToString(),
                                outcome_tbt = dsToolboxTalk.Tables[0].Rows[i]["outcome_tbt"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[i]["branch"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Talk_DateTime"].ToString(), out dateValue))
                            {
                                objToolboxTalk.Talk_DateTime = dateValue;
                            }

                            int iValue;

                            if (int.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Attendee_No"].ToString(), out iValue))
                            {
                                objToolboxTalk.Attendee_No = iValue;
                            }

                            objToolboxTalkList.lstToolboxTalkModels.Add(objToolboxTalk);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objToolboxTalkList.lstToolboxTalkModels.ToList());
        }
        
        [AllowAnonymous]
        public JsonResult ToolboxTalkListSearch(string branch_name)
        {
            ToolboxTalkModelsList objToolboxTalkList = new ToolboxTalkModelsList();
            objToolboxTalkList.lstToolboxTalkModels = new List<ToolboxTalkModels>();

            ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
            ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select ToolBox_TalkId, Talk_DateTime, Conducted_By, Topic, Attendee_No, Report_No,"
                + "Upload_Report, LoggedBy,Project,Location,Department,Attended_by,Activity_type,Identified_method,ptw," +
                "Comply_ppe,Identified_hazard,feedback_tbt,outcome_tbt,branch FROM t_toolbox_talk where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Talk_DateTime desc";

                DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                if (dsToolboxTalk.Tables.Count > 0)
                {
                    for (int i = 0; i < dsToolboxTalk.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ToolboxTalkModels objToolboxTalk = new ToolboxTalkModels
                            {
                                ToolBox_TalkId = dsToolboxTalk.Tables[0].Rows[i]["ToolBox_TalkId"].ToString(),
                                Conducted_By = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["Conducted_By"].ToString()),
                                Topic = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Topic"].ToString()),
                                Report_No = dsToolboxTalk.Tables[0].Rows[i]["Report_No"].ToString(),
                                Upload_Report = (dsToolboxTalk.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                Project = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Project"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[i]["Location"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[i]["Department"].ToString()),
                                Attended_by = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["Attended_by"].ToString()),
                                Activity_type = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Activity_type"].ToString()),
                                Identified_method = dsToolboxTalk.Tables[0].Rows[i]["Identified_method"].ToString(),
                                ptw = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["ptw"].ToString()),
                                Comply_ppe = dsToolboxTalk.Tables[0].Rows[i]["Comply_ppe"].ToString(),
                                Identified_hazard = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Identified_hazard"].ToString()),
                                feedback_tbt = dsToolboxTalk.Tables[0].Rows[i]["feedback_tbt"].ToString(),
                                outcome_tbt = dsToolboxTalk.Tables[0].Rows[i]["outcome_tbt"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[i]["branch"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Talk_DateTime"].ToString(), out dateValue))
                            {
                                objToolboxTalk.Talk_DateTime = dateValue;
                            }

                            int iValue;

                            if (int.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Attendee_No"].ToString(), out iValue))
                            {
                                objToolboxTalk.Attendee_No = iValue;
                            }

                            objToolboxTalkList.lstToolboxTalkModels.Add(objToolboxTalk);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult ToolboxTalkDetails()
        {
            try
            {
                if (Request.QueryString["ToolBox_TalkId"] != null && Request.QueryString["ToolBox_TalkId"] != "")
                {
                    string sToolBox_TalkId = Request.QueryString["ToolBox_TalkId"];
                    string sSqlstmt = "select ToolBox_TalkId, Talk_DateTime, Conducted_By, Topic, Attendee_No, Report_No,"
                    + "Upload_Report, LoggedBy,Project,Location,Department,Attended_by,Activity_type,Identified_method,ptw," +
                    "Comply_ppe,Identified_hazard,feedback_tbt,outcome_tbt,branch FROM t_toolbox_talk where ToolBox_TalkId='" + sToolBox_TalkId + "'";

                    DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsToolboxTalk.Tables.Count > 0)
                    {

                        ToolboxTalkModels objToolboxTalk = new ToolboxTalkModels
                        {
                            ToolBox_TalkId = dsToolboxTalk.Tables[0].Rows[0]["ToolBox_TalkId"].ToString(),
                            Conducted_By = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["Conducted_By"].ToString()),
                            Topic = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Topic"].ToString()),
                            Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                            Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Project = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Project"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[0]["Department"].ToString()),
                            Attended_by = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["Attended_by"].ToString()),
                            Activity_type = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Activity_type"].ToString()),
                            Identified_method = dsToolboxTalk.Tables[0].Rows[0]["Identified_method"].ToString(),
                            ptw = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["ptw"].ToString()),
                            Comply_ppe = dsToolboxTalk.Tables[0].Rows[0]["Comply_ppe"].ToString(),
                            Identified_hazard = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Identified_hazard"].ToString()),
                            feedback_tbt = dsToolboxTalk.Tables[0].Rows[0]["feedback_tbt"].ToString(),
                            outcome_tbt = dsToolboxTalk.Tables[0].Rows[0]["outcome_tbt"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Talk_DateTime"].ToString(), out dateValue))
                        {
                            objToolboxTalk.Talk_DateTime = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Attendee_No"].ToString(), out iValue))
                        {
                            objToolboxTalk.Attendee_No = iValue;
                        }

                        return View(objToolboxTalk);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ToolboxTalkList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("ToolboxTalkList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ToolboxTalkList");
        }
                
        [AllowAnonymous]
        public ActionResult ToolboxTalkInfo(int id)
        {
            try
            {
                    string sSqlstmt = "select ToolBox_TalkId, Talk_DateTime, Conducted_By, Topic, Attendee_No, Report_No,"
                    + "Upload_Report, LoggedBy,Project,Location,Department,Attended_by,Activity_type,Identified_method,ptw," +
                    "Comply_ppe,Identified_hazard,feedback_tbt,outcome_tbt,branch FROM t_toolbox_talk where ToolBox_TalkId='" + id + "'";

                    DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsToolboxTalk.Tables.Count > 0)
                    {

                        ToolboxTalkModels objToolboxTalk = new ToolboxTalkModels
                        {
                            ToolBox_TalkId = dsToolboxTalk.Tables[0].Rows[0]["ToolBox_TalkId"].ToString(),
                            Conducted_By = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["Conducted_By"].ToString()),
                            Topic = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Topic"].ToString()),
                            Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                            Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Project = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Project"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[0]["Department"].ToString()),
                            Attended_by = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["Attended_by"].ToString()),
                            Activity_type = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Activity_type"].ToString()),
                            Identified_method = dsToolboxTalk.Tables[0].Rows[0]["Identified_method"].ToString(),
                            ptw = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["ptw"].ToString()),
                            Comply_ppe = dsToolboxTalk.Tables[0].Rows[0]["Comply_ppe"].ToString(),
                            Identified_hazard = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Identified_hazard"].ToString()),
                            feedback_tbt = dsToolboxTalk.Tables[0].Rows[0]["feedback_tbt"].ToString(),
                            outcome_tbt = dsToolboxTalk.Tables[0].Rows[0]["outcome_tbt"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Talk_DateTime"].ToString(), out dateValue))
                        {
                            objToolboxTalk.Talk_DateTime = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Attendee_No"].ToString(), out iValue))
                        {
                            objToolboxTalk.Attendee_No = iValue;
                        }

                        return View(objToolboxTalk);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ToolboxTalkList");
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ToolboxTalkList");
        }
         
        [AllowAnonymous]
        public ActionResult ToolboxTalkEdit()
        {
            try
            {
                if (Request.QueryString["ToolBox_TalkId"] != null && Request.QueryString["ToolBox_TalkId"] != "")
                {
                    ViewBag.Department = objGlobaldata.GetDepartmentWithIdListbox();
                    string sToolBox_TalkId = Request.QueryString["ToolBox_TalkId"];
                    string sSqlstmt = "select ToolBox_TalkId, Talk_DateTime, Conducted_By, Topic, Attendee_No, Report_No,"
                    + "Upload_Report, LoggedBy,Project,Location,Department,Attended_by,Activity_type,Identified_method,ptw," +
                    "Comply_ppe,Identified_hazard,feedback_tbt,outcome_tbt,branch FROM t_toolbox_talk where ToolBox_TalkId='" + sToolBox_TalkId + "'";

                    DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsToolboxTalk.Tables.Count > 0)
                    {
                        ToolboxTalkModels objToolboxTalk = new ToolboxTalkModels
                        {
                            ToolBox_TalkId = dsToolboxTalk.Tables[0].Rows[0]["ToolBox_TalkId"].ToString(),
                            Conducted_By =/*objGlobaldata.GetEmpHrNameById*/(dsToolboxTalk.Tables[0].Rows[0]["Conducted_By"].ToString()),
                            Topic = dsToolboxTalk.Tables[0].Rows[0]["Topic"].ToString(),
                            Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                            Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            LoggedBy = (dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Project = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Project"].ToString()),
                            Location = /*objGlobaldata.GetDivisionLocationById*/(dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                            Department = /*objGlobaldata.GetMultiDeptNameById*/(dsToolboxTalk.Tables[0].Rows[0]["Department"].ToString()),
                            Attended_by = (dsToolboxTalk.Tables[0].Rows[0]["Attended_by"].ToString()),
                            Activity_type = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Activity_type"].ToString()),
                            Identified_method = dsToolboxTalk.Tables[0].Rows[0]["Identified_method"].ToString(),
                            ptw = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["ptw"].ToString()),
                            Comply_ppe = dsToolboxTalk.Tables[0].Rows[0]["Comply_ppe"].ToString(),
                            Identified_hazard = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Identified_hazard"].ToString()),
                            feedback_tbt = dsToolboxTalk.Tables[0].Rows[0]["feedback_tbt"].ToString(),
                            outcome_tbt = dsToolboxTalk.Tables[0].Rows[0]["outcome_tbt"].ToString(),
                            branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Talk_DateTime"].ToString(), out dateValue))
                        {
                            objToolboxTalk.Talk_DateTime = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Attendee_No"].ToString(), out iValue))
                        {
                            objToolboxTalk.Attendee_No = iValue;
                        }
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.Topics = objGlobaldata.GetDropdownList("Toolbox Topic");
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.ActivityType = objGlobaldata.GetDropdownList("ToolBox-Activity Type");
                        ViewBag.Identifiedhazard = objGlobaldata.GetDropdownList("ToolBox-Hazard");
                        ViewBag.PTW = objGlobaldata.GetDropdownList("ToolBox-PTW");
                        ViewBag.ComplyPPEType = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

                        return View(objToolboxTalk);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("ToolboxTalkList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("ToolboxTalkList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ToolboxTalkList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToolboxTalkEdit(ToolboxTalkModels objToolboxTalk, FormCollection form, IEnumerable<HttpPostedFileBase> Upload_Report)
        {
            try
            {
                objToolboxTalk.Attended_by = form["Attended_by"];
                objToolboxTalk.Conducted_By = form["Conducted_By"];
                objToolboxTalk.Topic = form["Topic"];
                objToolboxTalk.Location = form["Location"];
                objToolboxTalk.Activity_type = form["Activity_type"];
                objToolboxTalk.Identified_method = form["Identified_method"];
                objToolboxTalk.ptw = form["ptw"];
                objToolboxTalk.Comply_ppe = form["Comply_ppe"];
                objToolboxTalk.Department = form["Department"];
                objToolboxTalk.branch = form["branch"];

                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                string QCDelete = Request.Form["QCDocsValselectall"];
                if (form["Talk_DateTime"] != null && DateTime.TryParse(form["Talk_DateTime"], out dateValue) == true)
                {
                    objToolboxTalk.Talk_DateTime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objToolboxTalk.Talk_DateTime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }
                
                if (Upload_Report != null && files.ContentLength > 0)
                {
                    objToolboxTalk.Upload_Report = "";
                    foreach (var file in Upload_Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "ToolBox" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report + "," + form["QCDocsVal"];
                    objToolboxTalk.Upload_Report = objToolboxTalk.Upload_Report.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objToolboxTalk.Upload_Report = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objToolboxTalk.Upload_Report = null;
                }
                if (objToolboxTalk.FunUpdateToolboxTalk(objToolboxTalk))
                {
                    TempData["Successdata"] = "Updated Toolbox Talk details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ToolboxTalkEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ToolboxTalkList");
        }
                        
        public JsonResult GetEmployeeNameListMaster(string Conducted_By)
        {
            objLeaveList = objGlobaldata.GetEmployeeNameListboxForAutoCompMaster();

            List<string> lstFilteredList = objLeaveList.FindAll(d => d.StartsWith(Conducted_By.ToUpper()));

            return Json(lstFilteredList);
        }

        [HttpPost]
        public JsonResult FunGetReportNo(/*string Project,*/ string Location)
        {

            DataSet dsData; string RepNo = "";

            dsData = objGlobaldata.GetReportNo("TBT", "", Location);
            if (dsData != null && dsData.Tables.Count > 0)
            {
                RepNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
            }
            return Json(RepNo);

        }

    }
}
