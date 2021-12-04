using ISOStd.Filters;
using ISOStd.Models;
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
    public class SafetyViolationLogController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public SafetyViolationLogController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "SafetyViolationLog";
        }

        //
        // GET: /SafetyViolationLog/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /SafetyViolationLog/AddSafetyViolationLog

        [AllowAnonymous]
        public ActionResult AddSafetyViolationLog()
        {
            SafetyViolationLogModels objSafety = new SafetyViolationLogModels();
            try
            {
                objSafety.branch = objGlobaldata.GetCurrentUserSession().division;
                objSafety.Dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objSafety.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objSafety.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objSafety.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.SafetyVoilationType = objGlobaldata.GetDropdownList("HSEVoilationType");
                ViewBag.Action = objGlobaldata.GetDropdownList("SafetyVoilation Action");
                ViewBag.WaringType = objGlobaldata.GetDropdownList("SafetyViolation-TypeofWaring");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSafetyViolationLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSafety);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSafetyViolationLog(SafetyViolationLogModels objSafetyViolationLog, FormCollection form, HttpPostedFileBase Upload_Report)
        {
            try
            {
                objSafetyViolationLog.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                objSafetyViolationLog.VoilationType = form["VoilationType"];
                objSafetyViolationLog.Violation_warning = form["Violation_warning"];
                objSafetyViolationLog.Dept = form["Dept"];
                objSafetyViolationLog.Supervisor = form["Supervisor"];
                objSafetyViolationLog.UnsafeAct_Personnel = form["UnsafeAct_Personnel"];
                objSafetyViolationLog.ApprovedBy = form["ApprovedBy"];
                objSafetyViolationLog.Location = form["Location"];
                objSafetyViolationLog.branch = form["branch"];
                objSafetyViolationLog.issued_to = form["issued_to"];
                DateTime dateValue;

                if (form["Reported_On"] != null && DateTime.TryParse(form["Reported_On"], out dateValue) == true)
                {
                    objSafetyViolationLog.Reported_On = dateValue;
                }

                if (form["UnasafeAct_OccurredOn"] != null && DateTime.TryParse(form["UnasafeAct_OccurredOn"], out dateValue) == true)
                {
                    objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objSafetyViolationLog.UnasafeAct_OccurredOn = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (Upload_Report != null && Upload_Report.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(Upload_Report.FileName));
                        string sFilename = "SafetyVoilation" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload_Report.SaveAs(sFilepath + "/" + sFilename);
                        objSafetyViolationLog.Upload_Report = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddSafetyViolationLog: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objSafetyViolationLog.FunAddSafetyViolationLog(objSafetyViolationLog, Upload_Report))
                {
                    TempData["Successdata"] = "Added Safety Violation Log details successfully  with Reference Number '" + objSafetyViolationLog.Report_No + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSafetyViolationLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyViolationLogList");
        }

        [AllowAnonymous]
        public JsonResult SafetyLogDocDelete(FormCollection form)
        {
            try
            {
                if (form["ViolationLog_Id"] != null && form["ViolationLog_Id"] != "")
                {
                    SafetyViolationLogModels Doc = new SafetyViolationLogModels();
                    string sViolationLog_Id = form["ViolationLog_Id"];

                    if (Doc.FunDeleteSafetyVoilationLogDoc(sViolationLog_Id))
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
                    TempData["alertdata"] = "Safety Log Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyLogDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult SafetyViolationLogList(string branch_name)
        {
            SafetyViolationLogModelsList objSafetyViolationLogList = new SafetyViolationLogModelsList();
            objSafetyViolationLogList.lstSafetyViolationLog = new List<SafetyViolationLogModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select ViolationLog_Id, Reported_On, UnasafeAct_OccurredOn, UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, Report_No,"
                    + " Upload_Report,LoggedBy,VoilationType,Violation_warning,Dept,Supervisor,IssuedBy,Other_supervisor,ApprovedBy," +
                    " (case when Approved_Status='1' then 'Approved'  when Approved_Status='2' then 'Rejected' else 'Pending for Approved' end) as Approved_Status,branch,Location  from t_safety_violationlog where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by UnasafeAct_OccurredOn desc";
                DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                if (dsToolboxTalk.Tables.Count > 0)
                {
                    for (int i = 0; i < dsToolboxTalk.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SafetyViolationLogModels objSafetyViolationLog = new SafetyViolationLogModels
                            {
                                ViolationLog_Id = dsToolboxTalk.Tables[0].Rows[i]["ViolationLog_Id"].ToString(),
                                UnsafeAct_ReportedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_ReportedBy"].ToString()),
                                UnsafeAct_Personnel = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_Personnel"].ToString()),
                                UnsafeAct_Why = dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_Why"].ToString(),
                                Report_No = dsToolboxTalk.Tables[0].Rows[i]["Report_No"].ToString(),
                                Upload_Report = (dsToolboxTalk.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                VoilationType = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["VoilationType"].ToString()),
                                Violation_warning = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Violation_warning"].ToString()),
                                Supervisor = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["Supervisor"].ToString()),
                                IssuedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["IssuedBy"].ToString()),
                                Other_supervisor = dsToolboxTalk.Tables[0].Rows[i]["Other_supervisor"].ToString(),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                Approved_Status = dsToolboxTalk.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[i]["branch"].ToString()),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[i]["Dept"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Reported_On"].ToString(), out dateValue))
                            {
                                objSafetyViolationLog.Reported_On = dateValue;
                            }

                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["UnasafeAct_OccurredOn"].ToString(), out dateValue))
                            {
                                objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                            }

                            objSafetyViolationLogList.lstSafetyViolationLog.Add(objSafetyViolationLog);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSafetyViolationLogList.lstSafetyViolationLog.ToList());
        }

        [AllowAnonymous]
        public JsonResult SafetyViolationLogListSearch(string branch_name)
        {
            SafetyViolationLogModelsList objSafetyViolationLogList = new SafetyViolationLogModelsList();
            objSafetyViolationLogList.lstSafetyViolationLog = new List<SafetyViolationLogModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select ViolationLog_Id, Reported_On, UnasafeAct_OccurredOn, UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, Report_No,"
                    + " Upload_Report,LoggedBy,VoilationType,Violation_warning,Dept,Supervisor,IssuedBy,Other_supervisor,ApprovedBy,branch,Location  from t_safety_violationlog where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by UnasafeAct_OccurredOn desc";
                DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                if (dsToolboxTalk.Tables.Count > 0)
                {
                    for (int i = 0; i < dsToolboxTalk.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SafetyViolationLogModels objSafetyViolationLog = new SafetyViolationLogModels
                            {
                                ViolationLog_Id = dsToolboxTalk.Tables[0].Rows[i]["ViolationLog_Id"].ToString(),
                                UnsafeAct_ReportedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_ReportedBy"].ToString()),
                                UnsafeAct_Personnel = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_Personnel"].ToString()),
                                UnsafeAct_Why = dsToolboxTalk.Tables[0].Rows[i]["UnsafeAct_Why"].ToString(),
                                Report_No = dsToolboxTalk.Tables[0].Rows[i]["Report_No"].ToString(),
                                Upload_Report = (dsToolboxTalk.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                VoilationType = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["VoilationType"].ToString()),
                                Violation_warning = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[i]["Violation_warning"].ToString()),
                                Supervisor = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["Supervisor"].ToString()),
                                IssuedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[i]["IssuedBy"].ToString()),
                                Other_supervisor = dsToolboxTalk.Tables[0].Rows[i]["Other_supervisor"].ToString(),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[i]["branch"].ToString()),
                                Dept = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[i]["Dept"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["Reported_On"].ToString(), out dateValue))
                            {
                                objSafetyViolationLog.Reported_On = dateValue;
                            }

                            if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[i]["UnasafeAct_OccurredOn"].ToString(), out dateValue))
                            {
                                objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                            }

                            objSafetyViolationLogList.lstSafetyViolationLog.Add(objSafetyViolationLog);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult SafetyViolationLogDetails()
        {
            try
            {
                if (Request.QueryString["ViolationLog_Id"] != null && Request.QueryString["ViolationLog_Id"] != "")
                {
                    string sViolationLog_Id = Request.QueryString["ViolationLog_Id"];
                    string sSqlstmt = "select ViolationLog_Id, Reported_On, UnasafeAct_OccurredOn, UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, Report_No,"
                        + " Upload_Report, LoggedBy,VoilationType,Action_taken,HSE_observation,Emp_statement,Violation_warning,Dept,Supervisor,IssuedBy,Other_supervisor" +
                        ",ApprovedBy,branch,Location,voilation_detail,issued_to from t_safety_violationlog where ViolationLog_Id='" + sViolationLog_Id + "'";

                    DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsToolboxTalk.Tables.Count > 0)
                    {
                        SafetyViolationLogModels objSafetyViolationLog = new SafetyViolationLogModels
                        {
                            ViolationLog_Id = dsToolboxTalk.Tables[0].Rows[0]["ViolationLog_Id"].ToString(),
                            UnsafeAct_ReportedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_ReportedBy"].ToString()),
                            UnsafeAct_Personnel = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Personnel"].ToString()),
                            UnsafeAct_Why = dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Why"].ToString(),
                            Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                            Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            VoilationType = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["VoilationType"].ToString()),
                            Action_taken = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Action_taken"].ToString()),
                            HSE_observation = (dsToolboxTalk.Tables[0].Rows[0]["HSE_observation"].ToString()),
                            Emp_statement = (dsToolboxTalk.Tables[0].Rows[0]["Emp_statement"].ToString()),
                            Violation_warning = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Violation_warning"].ToString()),
                            Supervisor = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["Supervisor"].ToString()),
                            IssuedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["IssuedBy"].ToString()),
                            Other_supervisor = dsToolboxTalk.Tables[0].Rows[0]["Other_supervisor"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                            Dept = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[0]["Dept"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                            issued_to = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["issued_to"].ToString()),
                            voilation_detail = (dsToolboxTalk.Tables[0].Rows[0]["voilation_detail"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                        {
                            objSafetyViolationLog.Reported_On = dateValue;
                        }

                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["UnasafeAct_OccurredOn"].ToString(), out dateValue))
                        {
                            objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                        }

                        return View(objSafetyViolationLog);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SafetyViolationLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("SafetyViolationLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyViolationLogList");
        }

        [AllowAnonymous]
        public ActionResult SafetyViolationLogInfo(int id)
        {
            try
            {
                string sSqlstmt = "select ViolationLog_Id, Reported_On, UnasafeAct_OccurredOn, UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, Report_No,"
                    + " Upload_Report, LoggedBy,VoilationType,Action_taken,HSE_observation,Emp_statement,Violation_warning,Dept,Supervisor,IssuedBy,Other_supervisor," +
                    "ApprovedBy,branch,Location from t_safety_violationlog where ViolationLog_Id='" + id + "'";

                DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                if (dsToolboxTalk.Tables.Count > 0)
                {
                    SafetyViolationLogModels objSafetyViolationLog = new SafetyViolationLogModels
                    {
                        ViolationLog_Id = dsToolboxTalk.Tables[0].Rows[0]["ViolationLog_Id"].ToString(),
                        UnsafeAct_ReportedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_ReportedBy"].ToString()),
                        UnsafeAct_Personnel = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Personnel"].ToString()),
                        UnsafeAct_Why = dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Why"].ToString(),
                        Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                        Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        VoilationType = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["VoilationType"].ToString()),
                        Action_taken = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Action_taken"].ToString()),
                        HSE_observation = (dsToolboxTalk.Tables[0].Rows[0]["HSE_observation"].ToString()),
                        Emp_statement = dsToolboxTalk.Tables[0].Rows[0]["Emp_statement"].ToString(),
                        Violation_warning = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Violation_warning"].ToString()),
                        Supervisor = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["Supervisor"].ToString()),
                        IssuedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["IssuedBy"].ToString()),
                        Other_supervisor = dsToolboxTalk.Tables[0].Rows[0]["Other_supervisor"].ToString(),
                        ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsToolboxTalk.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                        Dept = objGlobaldata.GetMultiDeptNameById(dsToolboxTalk.Tables[0].Rows[0]["Dept"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                    {
                        objSafetyViolationLog.Reported_On = dateValue;
                    }

                    if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["UnasafeAct_OccurredOn"].ToString(), out dateValue))
                    {
                        objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                    }

                    return View(objSafetyViolationLog);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("SafetyViolationLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyViolationLogList");
        }

        [AllowAnonymous]
        public ActionResult SafetyViolationLogEdit()
        {
            try
            {
                ViewBag.SafetyVoilationType = objGlobaldata.GetDropdownList("HSEVoilationType");
                if (Request.QueryString["ViolationLog_Id"] != null && Request.QueryString["ViolationLog_Id"] != "")
                {
                    string sViolationLog_Id = Request.QueryString["ViolationLog_Id"];
                    string sSqlstmt = "select ViolationLog_Id, Reported_On, UnasafeAct_OccurredOn, UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, Report_No,"
                        + " Upload_Report, LoggedBy,VoilationType,Action_taken,HSE_observation,Emp_statement,Violation_warning,Dept,Supervisor,IssuedBy,Other_supervisor," +
                        "ApprovedBy,branch,Location,voilation_detail,issued_to  from t_safety_violationlog where ViolationLog_Id='" + sViolationLog_Id + "'";

                    DataSet dsToolboxTalk = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsToolboxTalk.Tables.Count > 0)
                    {
                        SafetyViolationLogModels objSafetyViolationLog = new SafetyViolationLogModels
                        {
                            ViolationLog_Id = dsToolboxTalk.Tables[0].Rows[0]["ViolationLog_Id"].ToString(),
                            UnsafeAct_ReportedBy = (dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_ReportedBy"].ToString()),
                            UnsafeAct_Personnel = dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Personnel"].ToString(),
                            UnsafeAct_Why = dsToolboxTalk.Tables[0].Rows[0]["UnsafeAct_Why"].ToString(),
                            Report_No = dsToolboxTalk.Tables[0].Rows[0]["Report_No"].ToString(),
                            Upload_Report = (dsToolboxTalk.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsToolboxTalk.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            VoilationType = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["VoilationType"].ToString()),
                            Action_taken = dsToolboxTalk.Tables[0].Rows[0]["Action_taken"].ToString(),
                            HSE_observation = dsToolboxTalk.Tables[0].Rows[0]["HSE_observation"].ToString(),
                            Emp_statement = dsToolboxTalk.Tables[0].Rows[0]["Emp_statement"].ToString(),
                            Violation_warning = objGlobaldata.GetDropdownitemById(dsToolboxTalk.Tables[0].Rows[0]["Violation_warning"].ToString()),
                            Supervisor = /*objGlobaldata.GetEmpHrNameById*/(dsToolboxTalk.Tables[0].Rows[0]["Supervisor"].ToString()),
                            IssuedBy = /*objGlobaldata.GetEmpHrNameById*/(dsToolboxTalk.Tables[0].Rows[0]["IssuedBy"].ToString()),
                            Other_supervisor = dsToolboxTalk.Tables[0].Rows[0]["Other_supervisor"].ToString(),
                            ApprovedBy = /*objGlobaldata.GetMultiHrEmpNameById*/(dsToolboxTalk.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            branch = (dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString()),
                            Dept = (dsToolboxTalk.Tables[0].Rows[0]["Dept"].ToString()),
                            Location = (dsToolboxTalk.Tables[0].Rows[0]["Location"].ToString()),
                            voilation_detail = (dsToolboxTalk.Tables[0].Rows[0]["voilation_detail"].ToString()),
                            issued_to = (dsToolboxTalk.Tables[0].Rows[0]["issued_to"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                        {
                            objSafetyViolationLog.Reported_On = dateValue;
                        }

                        if (DateTime.TryParse(dsToolboxTalk.Tables[0].Rows[0]["UnasafeAct_OccurredOn"].ToString(), out dateValue))
                        {
                            objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                        }

                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsToolboxTalk.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.Action = objGlobaldata.GetDropdownList("SafetyVoilation Action");
                        ViewBag.WaringType = objGlobaldata.GetDropdownList("SafetyViolation-TypeofWaring");
                        return View(objSafetyViolationLog);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("SafetyViolationLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("SafetyViolationLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyViolationLogList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SafetyViolationLogEdit(SafetyViolationLogModels objSafetyViolationLog, FormCollection form, HttpPostedFileBase Upload_Report)
        {
            try
            {
                objSafetyViolationLog.VoilationType = form["VoilationType"];
                objSafetyViolationLog.Violation_warning = form["Violation_warning"];
                objSafetyViolationLog.Dept = form["Dept"];
                objSafetyViolationLog.Supervisor = form["Supervisor"];
                objSafetyViolationLog.UnsafeAct_Personnel = form["UnsafeAct_Personnel"];
                objSafetyViolationLog.ApprovedBy = form["ApprovedBy"];
                objSafetyViolationLog.Location = form["Location"];
                objSafetyViolationLog.branch = form["branch"];

                DateTime dateValue;
                if (form["Reported_On"] != null && DateTime.TryParse(form["Reported_On"], out dateValue) == true)
                {
                    objSafetyViolationLog.Reported_On = dateValue;
                }

                if (form["UnasafeAct_OccurredOn"] != null && DateTime.TryParse(form["UnasafeAct_OccurredOn"], out dateValue) == true)
                {
                    objSafetyViolationLog.UnasafeAct_OccurredOn = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objSafetyViolationLog.UnasafeAct_OccurredOn = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (Upload_Report != null && Upload_Report.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(Upload_Report.FileName));
                        string sFilename = "SafetyVoilation" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload_Report.SaveAs(sFilepath + "/" + sFilename);
                        objSafetyViolationLog.Upload_Report = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddSafetyViolationLog: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objSafetyViolationLog.FunUpdateSafetyViolationLog(objSafetyViolationLog))
                {
                    TempData["Successdata"] = "Updated Safety Violation Log details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SafetyViolationLogList");
        }

        [AllowAnonymous]
        public ActionResult SafetyViolationApproveReject(string ViolationLog_Id, int iStatus, string PendingFlg, string Document)
        {
            try
            {
                SafetyViolationLogModels objMgmtDocuments = new SafetyViolationLogModels();
                string filename = Path.GetFileName(Document);
                FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

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
                if (objMgmtDocuments.FunSViolationApproveOrReject(ViolationLog_Id, iStatus, fsSource, filename))
                {
                    TempData["Successdata"] = "Safety Violation is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("SafetyViolationLogList");
            }
        }

        public JsonResult SafetyViolationApproveRejectNoty(string ViolationLog_Id, int iStatus, string PendingFlg, string Document)
        {
            try
            {
                SafetyViolationLogModels objMgmtDocuments = new SafetyViolationLogModels();
                string filename = Path.GetFileName(Document);
                FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

                if (objMgmtDocuments.FunSViolationApproveOrReject(ViolationLog_Id, iStatus, fsSource, filename))
                {
                    return Json("Success" + iStatus);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SafetyViolationApproveReject: " + ex.ToString());
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