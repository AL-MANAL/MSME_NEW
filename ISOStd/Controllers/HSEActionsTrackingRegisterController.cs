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
    public class HSEActionsTrackingRegisterController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public HSEActionsTrackingRegisterController()
        {
            ViewBag.Menutype = "Actions";
            ViewBag.SubMenutype = "HSEActionsTrackingRegister";
        }

        //
        // GET: /HSEActionsTrackingRegister/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /HSEActionsTrackingRegister/AddHSEActionsTrackingRegister

        [AllowAnonymous]
        public ActionResult AddHSEActionsTrackingRegister()
        {
            HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();

            try
            {
                objHSE.branch = objGlobaldata.GetCurrentUserSession().division;
                objHSE.Resp_Dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objHSE.Applicable_Site = objGlobaldata.GetCurrentUserSession().Work_Location;
                
                //ViewBag.ReviewerList = objGlobaldata.GetGRoleList(objHSE.branch, objHSE.Resp_Dept, objHSE.Applicable_Site, "Reviewer");
              //  ViewBag.ApproverList = objGlobaldata.GetGRoleList(objHSE.branch, objHSE.Resp_Dept, objHSE.Applicable_Site, "Approver");
                //ViewBag.EmpList= objGlobaldata.GetGEmpListBymulitBDL(objHSE.branch, objHSE.Resp_Dept, objHSE.Applicable_Site);
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objHSE.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objHSE.branch);
                ViewBag.Action_Status = objHSE.GetMultiHSEStatusList();
                ViewBag.ShortFallSource = objHSE.GetMultiShortFallSourceList();
                ViewBag.RiskRank = objHSE.GetMultiRiskRankList();              
                ViewBag.HSECategory = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.ReviewerList = objGlobaldata.GetReviewer();
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();              
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHSEActionsTrackingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objHSE);
        }

        // POST: /HSEActionsTrackingRegister/AddHSEActionsTrackingRegister

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddHSEActionsTrackingRegister(HSEActionsTrackingRegisterModels objHSEActionsTrackingRegister, FormCollection form,
            IEnumerable<HttpPostedFileBase> Upload_Report, IEnumerable<HttpPostedFileBase> Additional_Attachment)
        {
            try
            {
                objHSEActionsTrackingRegister.Personnel_Resp = form["Personnel_Resp"];
                objHSEActionsTrackingRegister.branch = form["branch"];
                objHSEActionsTrackingRegister.Resp_Dept = form["Resp_Dept"];
                objHSEActionsTrackingRegister.Applicable_Site = form["Applicable_Site"];
                objHSEActionsTrackingRegister.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;

                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;

                if (form["Action_Initiated_On"] != null && DateTime.TryParse(form["Action_Initiated_On"], out dateValue) == true)
                {
                    objHSEActionsTrackingRegister.Action_Initiated_On = dateValue;
                }

                if (form["Due_Date"] != null && DateTime.TryParse(form["Due_Date"], out dateValue) == true)
                {
                    objHSEActionsTrackingRegister.Due_Date = dateValue;
                }
                int Notificationval = 1;

                if (objHSEActionsTrackingRegister.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objHSEActionsTrackingRegister.NotificationValue = "0";
                }
                else if (objHSEActionsTrackingRegister.NotificationPeriod == "Weeks" && int.TryParse(objHSEActionsTrackingRegister.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objHSEActionsTrackingRegister.NotificationPeriod == "Months" && int.TryParse(objHSEActionsTrackingRegister.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objHSEActionsTrackingRegister.NotificationDays = Notificationval;

                if (Upload_Report != null && files.ContentLength > 0)
                {
                    objHSEActionsTrackingRegister.Upload_Report = "";
                    foreach (var file in Upload_Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "HSEAction" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddHSEActionsTrackingRegister: " + ex.ToString());
                        }
                    }
                    objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (Additional_Attachment != null && files.ContentLength > 0)
                {
                    objHSEActionsTrackingRegister.Additional_Attachment = "";
                    foreach (var file in Additional_Attachment)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "HSEAction" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddHSEActionsTrackingRegister: " + ex.ToString());
                        }
                    }
                    objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objHSEActionsTrackingRegister.FunAddHSEActionsTrackingRegister(objHSEActionsTrackingRegister))
                {
                    TempData["Successdata"] = "Added HSE Actions Tracking Register details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHSEActionsTrackingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HSEActionsTrackingRegisterList");
        }
        
        [AllowAnonymous]
        public JsonResult HSEDocDelete(FormCollection form)
        {
            try
            {

                
                    if (form["Actions_TrackingReg_Id"] != null && form["Actions_TrackingReg_Id"] != "")
                    {

                        HSEActionsTrackingRegisterModels Doc = new HSEActionsTrackingRegisterModels();
                        string sActions_TrackingReg_Id = form["Actions_TrackingReg_Id"];


                        if (Doc.FunDeleteSafetyVoilationLogDoc(sActions_TrackingReg_Id))
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
                        TempData["alertdata"] = "HSE Tracking Id cannot be Null or empty";
                        return Json("Failed");
                    }
             

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
                
        [AllowAnonymous]
        public ActionResult HSEActionsTrackingRegisterList(string Short_Fall_Source, string HSEfromDate, string HSEtoDate, int? page, string branch_name)
        {
            HSEActionsTrackingRegisterModelsList objHSEActionsTrackingRegisterList = new HSEActionsTrackingRegisterModelsList();
            objHSEActionsTrackingRegisterList.lstHSEActionsTrackingRegister = new List<HSEActionsTrackingRegisterModels>();

            HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
            ViewBag.ShortFallSource = objHSE.GetMultiShortFallSourceList();

            try
            {
                string sSqlstmt = "select Actions_TrackingReg_Id, Action_No, Action_Initiated_On, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                      + " Personnel_Resp, Due_Date, Action_Status, Upload_Report, LoggedBy, Report_date, Short_Fall_Source, Applicable_Site, Category, Risk_Rank,"
                        + " KPI_Completion_Date, Resp_Dept, Resp_Section, EndorsedBy, ApprovedBy, ReviewedBy, Remarks, Additional_Attachment,branch "
                        + "from t_hse_actions_trackingreg where Active=1";

                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                DateTime dtFromdate, dtToDate;
                if (Short_Fall_Source != null && Short_Fall_Source != "Select")
                {
                    ViewBag.Short_Fall_SourceVal = Short_Fall_Source;
                    sSearchtext = sSearchtext + " (Short_Fall_Source ='" + Short_Fall_Source + "')";
                }

                if (sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }
                if (HSEfromDate != null && DateTime.TryParse(HSEfromDate, out dtFromdate) && DateTime.TryParse(HSEtoDate, out dtToDate))
                {
                    ViewBag.HSEfromDate = HSEfromDate;
                    ViewBag.HSEtoDate = HSEtoDate;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and Report_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and Report_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Actions_TrackingReg_Id desc";
                DataSet dsHSEActionsTracking = objGlobaldata.Getdetails(sSqlstmt);
                if (dsHSEActionsTracking.Tables.Count > 0)
                {  
                    for (int i = 0; i < dsHSEActionsTracking.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            HSEActionsTrackingRegisterModels objPPEIssueLog = new HSEActionsTrackingRegisterModels
                            {
                                Actions_TrackingReg_Id = dsHSEActionsTracking.Tables[0].Rows[i]["Actions_TrackingReg_Id"].ToString(),
                                Action_No = (dsHSEActionsTracking.Tables[0].Rows[i]["Action_No"].ToString()),
                                Action_Taken = dsHSEActionsTracking.Tables[0].Rows[i]["Action_Taken"].ToString(),
                                Reason_Initiating_Action = dsHSEActionsTracking.Tables[0].Rows[i]["Reason_Initiating_Action"].ToString(),
                                Initiated_By = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Initiated_By"].ToString()),
                                Personnel_Resp = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Personnel_Resp"].ToString()),
                                Action_Status = objHSE.GetHSEStatusNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Action_Status"].ToString()),
                                Upload_Report = dsHSEActionsTracking.Tables[0].Rows[i]["Upload_Report"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                Short_Fall_Source = objHSE.GetShortFallSourceNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Short_Fall_Source"].ToString()),
                                Category = objGlobaldata.GetIsoStdDescriptionById(dsHSEActionsTracking.Tables[0].Rows[i]["Category"].ToString()),
                                Risk_Rank = objHSE.GetRiskRankNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Risk_Rank"].ToString()),
                                Resp_Dept = objGlobaldata.GetMultiDeptNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Resp_Dept"].ToString()),
                                Resp_Section = (dsHSEActionsTracking.Tables[0].Rows[i]["Resp_Section"].ToString()),
                                Remarks = (dsHSEActionsTracking.Tables[0].Rows[i]["Remarks"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ReviewedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHSEActionsTracking.Tables[0].Rows[i]["branch"].ToString()),
                                Applicable_Site = objGlobaldata.GetDivisionLocationById(dsHSEActionsTracking.Tables[0].Rows[i]["Applicable_Site"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Action_Initiated_On"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Action_Initiated_On = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Due_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Due_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Report_date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Report_date = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["KPI_Completion_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.KPI_Completion_Date = dateValue;
                            }

                            objHSEActionsTrackingRegisterList.lstHSEActionsTrackingRegister.Add(objPPEIssueLog);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objHSEActionsTrackingRegisterList.lstHSEActionsTrackingRegister.ToList());
        }

        [AllowAnonymous]
        public JsonResult HSEActionsTrackingRegisterListSearch(string Short_Fall_Source, string HSEfromDate, string HSEtoDate, int? page, string branch_name)
        {
            HSEActionsTrackingRegisterModelsList objHSEActionsTrackingRegisterList = new HSEActionsTrackingRegisterModelsList();
            objHSEActionsTrackingRegisterList.lstHSEActionsTrackingRegister = new List<HSEActionsTrackingRegisterModels>();

            HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
            ViewBag.ShortFallSource = objHSE.GetMultiShortFallSourceList();

            try
            {
                string sSqlstmt = "select Actions_TrackingReg_Id, Action_No, Action_Initiated_On, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                      + " Personnel_Resp, Due_Date, Action_Status, Upload_Report, LoggedBy, Report_date, Short_Fall_Source, Applicable_Site, Category, Risk_Rank,"
                        + " KPI_Completion_Date, Resp_Dept, Resp_Section, EndorsedBy, ApprovedBy, ReviewedBy, Remarks, Additional_Attachment,branch "
                        + "from t_hse_actions_trackingreg where Active=1";

                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                DateTime dtFromdate, dtToDate;
                if (Short_Fall_Source != null && Short_Fall_Source != "Select")
                {
                    ViewBag.Short_Fall_SourceVal = Short_Fall_Source;
                    sSearchtext = sSearchtext + " (Short_Fall_Source ='" + Short_Fall_Source + "')";
                }

                if (sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }
                if (HSEfromDate != null && DateTime.TryParse(HSEfromDate, out dtFromdate) && DateTime.TryParse(HSEtoDate, out dtToDate))
                {
                    ViewBag.HSEfromDate = HSEfromDate;
                    ViewBag.HSEtoDate = HSEtoDate;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and Report_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and Report_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Actions_TrackingReg_Id desc";
                DataSet dsHSEActionsTracking = objGlobaldata.Getdetails(sSqlstmt);
                if (dsHSEActionsTracking.Tables.Count > 0)
                {                  

                    for (int i = 0; i < dsHSEActionsTracking.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            HSEActionsTrackingRegisterModels objPPEIssueLog = new HSEActionsTrackingRegisterModels
                            {
                                Actions_TrackingReg_Id = dsHSEActionsTracking.Tables[0].Rows[i]["Actions_TrackingReg_Id"].ToString(),
                                Action_No = (dsHSEActionsTracking.Tables[0].Rows[i]["Action_No"].ToString()),
                                Action_Taken = dsHSEActionsTracking.Tables[0].Rows[i]["Action_Taken"].ToString(),
                                Reason_Initiating_Action = dsHSEActionsTracking.Tables[0].Rows[i]["Reason_Initiating_Action"].ToString(),
                                Initiated_By = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Initiated_By"].ToString()),
                                Personnel_Resp = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Personnel_Resp"].ToString()),
                                Action_Status = objHSE.GetHSEStatusNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Action_Status"].ToString()),
                                Upload_Report = dsHSEActionsTracking.Tables[0].Rows[i]["Upload_Report"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                Short_Fall_Source = objHSE.GetShortFallSourceNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Short_Fall_Source"].ToString()),
                                Category = objGlobaldata.GetIsoStdDescriptionById(dsHSEActionsTracking.Tables[0].Rows[i]["Category"].ToString()),
                                Risk_Rank = objHSE.GetRiskRankNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Risk_Rank"].ToString()),
                                Resp_Dept = objGlobaldata.GetMultiDeptNameById(dsHSEActionsTracking.Tables[0].Rows[i]["Resp_Dept"].ToString()),
                                Resp_Section = (dsHSEActionsTracking.Tables[0].Rows[i]["Resp_Section"].ToString()),
                                Remarks = (dsHSEActionsTracking.Tables[0].Rows[i]["Remarks"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ReviewedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHSEActionsTracking.Tables[0].Rows[i]["branch"].ToString()),
                                Applicable_Site = objGlobaldata.GetDivisionLocationById(dsHSEActionsTracking.Tables[0].Rows[i]["Applicable_Site"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Action_Initiated_On"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Action_Initiated_On = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Due_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Due_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["Report_date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Report_date = dateValue;
                            }

                            if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[i]["KPI_Completion_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.KPI_Completion_Date = dateValue;
                            }

                            objHSEActionsTrackingRegisterList.lstHSEActionsTrackingRegister.Add(objPPEIssueLog);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
               
        [AllowAnonymous]
        public ActionResult HSEActionsTrackingRegisterDetails()
        {
            try
            {
                HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
                if (Request.QueryString["Actions_TrackingReg_Id"] != null && Request.QueryString["Actions_TrackingReg_Id"] != "")
                {
                    string sActions_TrackingReg_Id = Request.QueryString["Actions_TrackingReg_Id"];
                    string sSqlstmt = "select Actions_TrackingReg_Id, Action_No, Action_Initiated_On, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                         + " Personnel_Resp, Due_Date, Action_Status, Upload_Report, LoggedBy, Report_date, Short_Fall_Source, Applicable_Site, Category, Risk_Rank,"
                        + " KPI_Completion_Date, Resp_Dept, Resp_Section, EndorsedBy, ApprovedBy, ReviewedBy, Remarks, Additional_Attachment,NotificationDays,NotificationPeriod,NotificationValue,branch from t_hse_actions_trackingreg "
                    + "where Actions_TrackingReg_Id='" + sActions_TrackingReg_Id + "'";

                    DataSet dsHSEActionsTracking = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsHSEActionsTracking.Tables.Count > 0)
                    {

                        HSEActionsTrackingRegisterModels objPPEIssueLog = new HSEActionsTrackingRegisterModels
                        {
                            Actions_TrackingReg_Id = dsHSEActionsTracking.Tables[0].Rows[0]["Actions_TrackingReg_Id"].ToString(),
                            Action_No = (dsHSEActionsTracking.Tables[0].Rows[0]["Action_No"].ToString()),
                            Action_Taken = dsHSEActionsTracking.Tables[0].Rows[0]["Action_Taken"].ToString(),
                            Reason_Initiating_Action = dsHSEActionsTracking.Tables[0].Rows[0]["Reason_Initiating_Action"].ToString(),
                            Initiated_By = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Initiated_By"].ToString()),
                            Personnel_Resp = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Personnel_Resp"].ToString()),
                            Action_Status = objHSE.GetHSEStatusNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Status"].ToString()),
                            Upload_Report = dsHSEActionsTracking.Tables[0].Rows[0]["Upload_Report"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Short_Fall_Source = objHSE.GetShortFallSourceNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Short_Fall_Source"].ToString()),
                            Category = objGlobaldata.GetIsoStdDescriptionById(dsHSEActionsTracking.Tables[0].Rows[0]["Category"].ToString()),
                            Risk_Rank = objHSE.GetRiskRankNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Risk_Rank"].ToString()),
                            Resp_Dept = objGlobaldata.GetMultiDeptNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString()),
                            Resp_Section = (dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Section"].ToString()),
                            Remarks = (dsHSEActionsTracking.Tables[0].Rows[0]["Remarks"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ReviewedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            Additional_Attachment = dsHSEActionsTracking.Tables[0].Rows[0]["Additional_Attachment"].ToString(),
                            EndorsedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["EndorsedBy"].ToString()),
                            NotificationPeriod = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString()),
                            Applicable_Site = objGlobaldata.GetDivisionLocationById(dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Initiated_On"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Action_Initiated_On = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Due_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Report_date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Report_date = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["KPI_Completion_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.KPI_Completion_Date = dateValue;
                        }

                        return View(objPPEIssueLog);

                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HSEActionsTrackingRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "HSE Tracking Id cannot be null";
                    return RedirectToAction("HSEActionsTrackingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HSEActionsTrackingRegisterList");
        }
        
        [AllowAnonymous]
        public ActionResult HSEActionsTrackingRegisterInfo(int id)
        {
            try
            {
                HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
                string sSqlstmt = "select Actions_TrackingReg_Id, Action_No, Action_Initiated_On, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                     + " Personnel_Resp, Due_Date, Action_Status, Upload_Report, LoggedBy, Report_date, Short_Fall_Source, Applicable_Site, Category, Risk_Rank,"
                    + " KPI_Completion_Date, Resp_Dept, Resp_Section, EndorsedBy, ApprovedBy, ReviewedBy, Remarks, Additional_Attachment,NotificationDays,NotificationPeriod,NotificationValue,branch from t_hse_actions_trackingreg "
                + "where Actions_TrackingReg_Id='" + id + "'";

                DataSet dsHSEActionsTracking = objGlobaldata.Getdetails(sSqlstmt);
                if (dsHSEActionsTracking.Tables.Count > 0)
                {

                    HSEActionsTrackingRegisterModels objPPEIssueLog = new HSEActionsTrackingRegisterModels
                    {
                        Actions_TrackingReg_Id = dsHSEActionsTracking.Tables[0].Rows[0]["Actions_TrackingReg_Id"].ToString(),
                        Action_No = (dsHSEActionsTracking.Tables[0].Rows[0]["Action_No"].ToString()),
                        Action_Taken = dsHSEActionsTracking.Tables[0].Rows[0]["Action_Taken"].ToString(),
                        Reason_Initiating_Action = dsHSEActionsTracking.Tables[0].Rows[0]["Reason_Initiating_Action"].ToString(),
                        Initiated_By = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Initiated_By"].ToString()),
                        Personnel_Resp = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Personnel_Resp"].ToString()),
                        Action_Status = objHSE.GetHSEStatusNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Status"].ToString()),
                        Upload_Report = dsHSEActionsTracking.Tables[0].Rows[0]["Upload_Report"].ToString(),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        Short_Fall_Source = objHSE.GetShortFallSourceNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Short_Fall_Source"].ToString()),
                        Category = objGlobaldata.GetIsoStdDescriptionById(dsHSEActionsTracking.Tables[0].Rows[0]["Category"].ToString()),
                        Risk_Rank = objHSE.GetRiskRankNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Risk_Rank"].ToString()),
                        Resp_Dept = objGlobaldata.GetMultiDeptNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString()),
                        Resp_Section = (dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Section"].ToString()),
                        Remarks = (dsHSEActionsTracking.Tables[0].Rows[0]["Remarks"].ToString()),
                        ApprovedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        ReviewedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                        Additional_Attachment = dsHSEActionsTracking.Tables[0].Rows[0]["Additional_Attachment"].ToString(),
                        EndorsedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["EndorsedBy"].ToString()),
                        NotificationPeriod = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                        NotificationValue = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationValue"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString()),
                        Applicable_Site = objGlobaldata.GetDivisionLocationById(dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString()),
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Initiated_On"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.Action_Initiated_On = dateValue;
                    }

                    if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.Due_Date = dateValue;
                    }

                    if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Report_date"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.Report_date = dateValue;
                    }

                    if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["KPI_Completion_Date"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.KPI_Completion_Date = dateValue;
                    }

                    return View(objPPEIssueLog);

                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("HSEActionsTrackingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HSEActionsTrackingRegisterList");
        }

        [AllowAnonymous]
        public ActionResult HSEActionsTrackingRegisterEdit()
        {
            try
            {
                HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
                if (Request.QueryString["Actions_TrackingReg_Id"] != null && Request.QueryString["Actions_TrackingReg_Id"] != "")
                {
                    ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                    string sActions_TrackingReg_Id = Request.QueryString["Actions_TrackingReg_Id"];
                    string sSqlstmt = "select Actions_TrackingReg_Id, Action_No, Action_Initiated_On, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                        + " Personnel_Resp, Due_Date, Action_Status, Upload_Report, LoggedBy, Report_date, Short_Fall_Source, Applicable_Site, Category, Risk_Rank,"
                        + " KPI_Completion_Date, Resp_Dept, Resp_Section, EndorsedBy, ApprovedBy, ReviewedBy, Remarks, Additional_Attachment,NotificationDays,"
                        + "NotificationPeriod,NotificationValue,branch from t_hse_actions_trackingreg "
                    + "where Actions_TrackingReg_Id='" + sActions_TrackingReg_Id + "'";

                    DataSet dsHSEActionsTracking = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsHSEActionsTracking.Tables.Count > 0)
                    {
                        HSEActionsTrackingRegisterModels objPPEIssueLog = new HSEActionsTrackingRegisterModels
                        {
                            Actions_TrackingReg_Id = dsHSEActionsTracking.Tables[0].Rows[0]["Actions_TrackingReg_Id"].ToString(),
                            Action_No = (dsHSEActionsTracking.Tables[0].Rows[0]["Action_No"].ToString()),
                            Action_Taken = dsHSEActionsTracking.Tables[0].Rows[0]["Action_Taken"].ToString(),
                            Reason_Initiating_Action = dsHSEActionsTracking.Tables[0].Rows[0]["Reason_Initiating_Action"].ToString(),
                            Initiated_By = dsHSEActionsTracking.Tables[0].Rows[0]["Initiated_By"].ToString(),
                            Personnel_Resp = (dsHSEActionsTracking.Tables[0].Rows[0]["Personnel_Resp"].ToString()),
                            Action_Status = objHSE.GetHSEStatusNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Status"].ToString()),
                            Upload_Report = dsHSEActionsTracking.Tables[0].Rows[0]["Upload_Report"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsHSEActionsTracking.Tables[0].Rows[0]["LoggedBy"].ToString()),

                            Short_Fall_Source = objHSE.GetShortFallSourceNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Short_Fall_Source"].ToString()),
                            Category = dsHSEActionsTracking.Tables[0].Rows[0]["Category"].ToString(),
                            Risk_Rank = objHSE.GetRiskRankNameById(dsHSEActionsTracking.Tables[0].Rows[0]["Risk_Rank"].ToString()),
                            Resp_Dept = (dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString()),
                            Resp_Section = (dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Section"].ToString()),
                            Remarks = (dsHSEActionsTracking.Tables[0].Rows[0]["Remarks"].ToString()),
                            ApprovedBy = (dsHSEActionsTracking.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ReviewedBy = (dsHSEActionsTracking.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            Additional_Attachment = dsHSEActionsTracking.Tables[0].Rows[0]["Additional_Attachment"].ToString(),
                            EndorsedBy = dsHSEActionsTracking.Tables[0].Rows[0]["EndorsedBy"].ToString(),
                            NotificationPeriod = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsHSEActionsTracking.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            branch = (dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString()),
                            Applicable_Site = (dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString()),

                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Action_Initiated_On"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Action_Initiated_On = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Due_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Due_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["Report_date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Report_date = dateValue;
                        }

                        if (DateTime.TryParse(dsHSEActionsTracking.Tables[0].Rows[0]["KPI_Completion_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.KPI_Completion_Date = dateValue;
                        }

                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString());
                      //  ViewBag.EmpLists = objGlobaldata.GetGEmpListBymulitBDL(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString());
                      //  ViewBag.ReviewerList = objGlobaldata.GetGRoleList(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString(), "Reviewer");
                      //  ViewBag.ApproverList = objGlobaldata.GetGRoleList(dsHSEActionsTracking.Tables[0].Rows[0]["branch"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Resp_Dept"].ToString(), dsHSEActionsTracking.Tables[0].Rows[0]["Applicable_Site"].ToString(), "Approver");
                        
                        ViewBag.Action_Status = objHSE.GetMultiHSEStatusList();
                        ViewBag.ShortFallSource = objHSE.GetMultiShortFallSourceList();
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.HSECategory = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.ReviewerList = objGlobaldata.GetReviewer();
                        ViewBag.ApproverList = objGlobaldata.GetApprover();
                        ViewBag.RiskRank = objHSE.GetMultiRiskRankList();

                        return View(objPPEIssueLog);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("HSEActionsTrackingRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "HSE Tracking Id cannot be null";
                    return RedirectToAction("HSEActionsTrackingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HSEActionsTrackingRegisterList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HSEActionsTrackingRegisterEdit(HSEActionsTrackingRegisterModels objHSEActionsTrackingRegister, FormCollection form,
            IEnumerable<HttpPostedFileBase> Upload_Report, IEnumerable<HttpPostedFileBase> Additional_Attachment)
        {
            try
            {
                objHSEActionsTrackingRegister.Personnel_Resp = form["Personnel_Resp"];
                objHSEActionsTrackingRegister.branch = form["branch"];
                objHSEActionsTrackingRegister.Resp_Dept = form["Resp_Dept"];
                objHSEActionsTrackingRegister.Applicable_Site = form["Applicable_Site"];

                DateTime dateValue;
                string QCDelete1 = Request.Form["QCDocsValselectall1"];
                string QCDelete = Request.Form["QCDocsValselectall"];

                HttpPostedFileBase files = Request.Files[0];
                if (form["Action_Initiated_On"] != null && DateTime.TryParse(form["Action_Initiated_On"], out dateValue) == true)
                {
                    objHSEActionsTrackingRegister.Action_Initiated_On = dateValue;
                }

                if (form["Due_Date"] != null && DateTime.TryParse(form["Due_Date"], out dateValue) == true)
                {
                    objHSEActionsTrackingRegister.Due_Date = dateValue;
                }
                int Notificationval = 1;

                if (objHSEActionsTrackingRegister.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objHSEActionsTrackingRegister.NotificationValue = "0";
                }
                else if (objHSEActionsTrackingRegister.NotificationPeriod == "Weeks" && int.TryParse(objHSEActionsTrackingRegister.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objHSEActionsTrackingRegister.NotificationPeriod == "Months" && int.TryParse(objHSEActionsTrackingRegister.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objHSEActionsTrackingRegister.NotificationDays = Notificationval;

                if (Upload_Report != null && files.ContentLength > 0)
                {
                    objHSEActionsTrackingRegister.Upload_Report = "";
                    foreach (var file in Upload_Report)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "HSEActionTracking" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal1"] != null && form["QCDocsVal1"] != "")
                {
                    objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report + "," + form["QCDocsVal"];
                    objHSEActionsTrackingRegister.Upload_Report = objHSEActionsTrackingRegister.Upload_Report.Trim(',');
                }
                else if (form["QCDocsVal1"] == null && QCDelete1 != null && files.ContentLength == 0)
                {
                    objHSEActionsTrackingRegister.Upload_Report = null;
                }
                else if (form["QCDocsVal1"] == null && files.ContentLength == 0)
                {
                    objHSEActionsTrackingRegister.Upload_Report = null;
                }

                if (Additional_Attachment != null && files.ContentLength > 0)
                {
                    objHSEActionsTrackingRegister.Additional_Attachment = "";
                    foreach (var file in Additional_Attachment)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "HSEActionTracking" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment + "," + form["QCDocsVal"];
                    objHSEActionsTrackingRegister.Additional_Attachment = objHSEActionsTrackingRegister.Additional_Attachment.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objHSEActionsTrackingRegister.Additional_Attachment = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objHSEActionsTrackingRegister.Additional_Attachment = null;
                }
                if (objHSEActionsTrackingRegister.FunUpdateHSEActionsTrackingRegister(objHSEActionsTrackingRegister))
                {
                    TempData["Successdata"] = "Updated HSE Actions Tracking Register details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEActionsTrackingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("HSEActionsTrackingRegisterList");
        }


        [AllowAnonymous]
        public ActionResult ShortFallSourceChart(string HSEfromDate, string HSEtoDate)
        {

            ProductShareBarchartList objProductshareList = new ProductShareBarchartList();
            objProductshareList.lstProductSharechart = new List<ProductShareBarchart>();
            try
            {
                HSEActionsTrackingRegisterModels objHSE = new HSEActionsTrackingRegisterModels();
                DateTime dtFromdate, dtToDate;

                if (HSEfromDate != null && DateTime.TryParse(HSEfromDate, out dtFromdate) && DateTime.TryParse(HSEtoDate, out dtToDate))
                {
                    DataSet dsProductShareList = objGlobaldata.GetShortFallSourcedetails(dtFromdate.ToString("yyyy/MM/dd"), dtToDate.ToString("yyyy/MM/dd"));

                    //objGlobaldata.Getdetails("select short_fall_source as reportmonth,count(short_fall_source) as cntshortfall from"
                    //+ " t_hse_actions_trackingreg where report_date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd")
                    //    + "' group by month(report_date)");
                    for (int i = 0; i < dsProductShareList.Tables[0].Rows.Count; i++)
                    {
                        ProductShareBarchart objProduct = new ProductShareBarchart
                        {
                            Product = objHSE.GetShortFallSourceNameById(dsProductShareList.Tables[0].Rows[i]["short_fall_source"].ToString()),
                            Percentage = Convert.ToDecimal(dsProductShareList.Tables[0].Rows[i]["cntshortfall"].ToString())
                        };
                        objProductshareList.lstProductSharechart.Add(objProduct);
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ShortFallSourceChart: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objProductshareList.lstProductSharechart.ToList());
        }
    }
}
