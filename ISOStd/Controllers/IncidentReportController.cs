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
    public class IncidentReportController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public IncidentReportController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "IncidentReport";
        }

        //
        // GET: /IncidentReport/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /IncidentReport/AddIncidentReport

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddIncidentReport()
        {
            return InitilizeAddMgmtDocuments();
        }

        private ActionResult InitilizeAddMgmtDocuments()
        {
            try
            {
                IncidentReportModels objInc = new IncidentReportModels();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.InjuryNature = objGlobaldata.GetConstantValue("InjuryNature");
                ViewBag.AccidentType = objGlobaldata.GetDropdownList("Accident Type");
                ViewBag.CAPAStatus = objGlobaldata.GetDropdownList("Meeting Item Status");
                //ViewBag.CAPAStatus = objGlobaldata.GetConstantValue("CAPAStatus");
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Report_HSE = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.AccidentReport = objGlobaldata.GetAccidentReportsWithInvestList();
                ViewBag.HSEEmp = objGlobaldata.GetHrEmpHseList();
                ViewBag.Status = objGlobaldata.GetDropdownList("Incident Status");
                ViewBag.Injury = objGlobaldata.GetDropdownList("Type of Injury");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddMgmtDocuments: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        [AllowAnonymous]
        public JsonResult IncidentReportDocDelete(FormCollection form)
        {
            try
            {
                if (form["Incident_Id"] != null && form["Incident_Id"] != "")
                {
                    IncidentReportModels Doc = new IncidentReportModels();
                    string sIncident_Id = form["Incident_Id"];

                    if (Doc.FunDeleteIncidentDoc(sIncident_Id))
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
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        // POST: /PPEIssueLog/AddPPEIssueLog

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddIncidentReport(IncidentReportModels objIncidentReport, FormCollection form, IEnumerable<HttpPostedFileBase> Witness_StmtDoc, IEnumerable<HttpPostedFileBase> report_upload)
        {
            try
            {
                IList<HttpPostedFileBase> Witness_StmtDocList = (IList<HttpPostedFileBase>)Witness_StmtDoc;
                IList<HttpPostedFileBase> report_uploadList = (IList<HttpPostedFileBase>)report_upload;

                objIncidentReport.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                objIncidentReport.Minor_InjuriesNo = form["Minor_InjuriesNo"];
                objIncidentReport.Minor_Injuries_LTINo = form["Minor_Injuries_LTINo"];
                objIncidentReport.Major_InjuriesNo = form["Major_InjuriesNo"];
                objIncidentReport.Location = form["Location"];
                objIncidentReport.ReportToHSE = form["ReportToHSE"];
                objIncidentReport.hse_officer = form["hse_officer"];
                objIncidentReport.invest_team = form["invest_team"];
                objIncidentReport.WitnessedBy = form["WitnessedBy"];

                DateTime dateValue;

                if (form["Reported_On"] != null && DateTime.TryParse(form["Reported_On"], out dateValue) == true)
                {
                    objIncidentReport.Reported_On = dateValue;
                }

                if (form["Incident_Datetime"] != null && DateTime.TryParse(form["Incident_Datetime"], out dateValue) == true)
                {
                    objIncidentReport.Incident_Datetime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objIncidentReport.Incident_Datetime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (form["DueDate"] != null && DateTime.TryParse(form["DueDate"], out dateValue) == true)
                {
                    objIncidentReport.DueDate = dateValue;
                }

                if (form["ClosedOn"] != null && DateTime.TryParse(form["ClosedOn"], out dateValue) == true)
                {
                    objIncidentReport.ClosedOn = dateValue;
                }

                if (Witness_StmtDocList[0] != null)
                {
                    objIncidentReport.Witness_StmtDoc = "";
                    foreach (var file in Witness_StmtDoc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Incident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (report_uploadList[0] != null)
                {
                    objIncidentReport.report_upload = "";
                    foreach (var file in report_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Incident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objIncidentReport.report_upload = objIncidentReport.report_upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objIncidentReport.report_upload = objIncidentReport.report_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["LTI_Id " + i] != null || form["AccidentType " + i] != null)
                        {
                            IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                            {
                                Incident_Id = objIncidentReport.Incident_Id,
                                LTI_Id = form["LTI_Id " + i],
                                AccidentType = form["AccidentType " + i],
                                Emp_Id = form["Emp_Id " + i],
                                LTI_Hrs = form["LTI_Hrs " + i],
                                injury_type = form["injury_type " + i],
                            };
                            if (form["invest_start_date " + i] != null && DateTime.TryParse(form["invest_start_date " + i], out dateValue) == true)
                            {
                                objIncidentLTI.invest_start_date = dateValue;
                            }
                            if (form["invest_end_date " + i] != null && DateTime.TryParse(form["invest_end_date " + i], out dateValue) == true)
                            {
                                objIncidentLTI.invest_end_date = dateValue;
                            }
                            objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                        }
                    }
                }

                IncidentReportModelsList objActionList = new IncidentReportModelsList();
                objActionList.lstIncidentReportModels = new List<IncidentReportModels>();

                if (form["itemcnts"] != null && form["itemcnts"] != "" && int.TryParse(form["itemcnts"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                    {
                        if (form["incident_action " + i] != null)
                        {
                            IncidentReportModels objAction = new IncidentReportModels
                            {
                                Incident_Id = objIncidentReport.Incident_Id,
                                incident_action = form["incident_action " + i],
                                resp_pers = form["resp_pers " + i],
                                incident_status = form["incident_status " + i],
                                contractor = form["contractor " + i],
                                action_report = form["action_report " + i]
                            };
                            if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                            {
                                objAction.target_date = dateValue;
                            }
                            objActionList.lstIncidentReportModels.Add(objAction);
                        }
                    }
                }

                if (objIncidentReport.FunAddIncidentReport(objIncidentReport, objIncidentLTIList, objActionList))
                {
                    TempData["Successdata"] = "Added Incident report successfully  with Reference Number '" + objIncidentReport.Incident_Num + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }

        //
        // GET: /SafetyViolationLog/IncidentReportList

        [AllowAnonymous]
        public ActionResult IncidentReportList(string branch_name)
        {
            IncidentReportModelsList objIncidentReportList = new IncidentReportModelsList();
            objIncidentReportList.lstIncidentReportModels = new List<IncidentReportModels>();
            IncidentReportModels objinc = new IncidentReportModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                    + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                    + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                    + " ClosedOn, LoggedBy, Incident_Datetime, Loggeddate,ReportToHSE FROM t_incident_report where Active=1 ";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by Incident_Datetime desc";
                DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                if (dsIncident.Tables.Count > 0)
                {
                    for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            IncidentReportModels objIncidentReport = new IncidentReportModels
                            {
                                Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                Incident_Num = (dsIncident.Tables[0].Rows[i]["Incident_Num"].ToString()),
                                PreparedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["PreparedBy"].ToString()),
                                Incident_Type = objinc.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[i]["Incident_Type"].ToString()),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[i]["Location"].ToString()),
                                Occurred_onJob = (dsIncident.Tables[0].Rows[i]["Occurred_onJob"].ToString()),
                                InWork_Premises = dsIncident.Tables[0].Rows[i]["InWork_Premises"].ToString(),
                                WitnessedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["WitnessedBy"].ToString()),
                                Witness_StmtDoc = dsIncident.Tables[0].Rows[i]["Witness_StmtDoc"].ToString(),
                                Incident_Desc = dsIncident.Tables[0].Rows[i]["Incident_Desc"].ToString(),
                                Consequences = dsIncident.Tables[0].Rows[i]["Consequences"].ToString(),
                                Damages = (dsIncident.Tables[0].Rows[i]["Damages"].ToString()),
                                Injury_Nature = dsIncident.Tables[0].Rows[i]["Injury_Nature"].ToString(),
                                Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Minor_InjuriesNo"].ToString()),
                                Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Minor_Injuries_LTINo"].ToString()),
                                Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Major_InjuriesNo"].ToString()),
                                Actions_Taken = (dsIncident.Tables[0].Rows[i]["Actions_Taken"].ToString()),
                                Incident_Reasons = dsIncident.Tables[0].Rows[i]["Incident_Reasons"].ToString(),
                                Corrective_Measures = (dsIncident.Tables[0].Rows[i]["Corrective_Measures"].ToString()),
                                Corrective_ApprovedBy_ReviewedBy = (dsIncident.Tables[0].Rows[i]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                ReportToHSE = dsIncident.Tables[0].Rows[i]["ReportToHSE"].ToString()
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Reported_On"].ToString(), out dateValue))
                            {
                                objIncidentReport.Reported_On = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Incident_Datetime"].ToString(), out dateValue))
                            {
                                objIncidentReport.Incident_Datetime = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["DueDate"].ToString(), out dateValue))
                            {
                                objIncidentReport.DueDate = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["ClosedOn"].ToString(), out dateValue))
                            {
                                objIncidentReport.ClosedOn = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Incident_Datetime"].ToString(), out dateValue))
                            {
                                objIncidentReport.Incident_Datetime = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Loggeddate"].ToString(), out dateValue))
                            {
                                objIncidentReport.Loggeddate = dateValue;
                            }

                            int iValue;
                            if (int.TryParse(dsIncident.Tables[0].Rows[i]["FatalitiesNo"].ToString(), out iValue))
                            {
                                objIncidentReport.FatalitiesNo = iValue;
                            }

                            objIncidentReportList.lstIncidentReportModels.Add(objIncidentReport);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in IncidentReportList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objIncidentReportList.lstIncidentReportModels.ToList());
        }

        [AllowAnonymous]
        public JsonResult IncidentReportListSearch(string branch_name)
        {
            IncidentReportModelsList objIncidentReportList = new IncidentReportModelsList();
            objIncidentReportList.lstIncidentReportModels = new List<IncidentReportModels>();
            IncidentReportModels objinc = new IncidentReportModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                    + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                    + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                    + " ClosedOn, LoggedBy, Incident_Datetime, Loggeddate,ReportToHSE FROM t_incident_report where Active=1 ";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by Incident_Datetime desc";
                DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                if (dsIncident.Tables.Count > 0)
                {
                    for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            IncidentReportModels objIncidentReport = new IncidentReportModels
                            {
                                Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                Incident_Num = (dsIncident.Tables[0].Rows[i]["Incident_Num"].ToString()),
                                PreparedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["PreparedBy"].ToString()),
                                Incident_Type = objinc.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[i]["Incident_Type"].ToString()),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[i]["Location"].ToString()),
                                Occurred_onJob = (dsIncident.Tables[0].Rows[i]["Occurred_onJob"].ToString()),
                                InWork_Premises = dsIncident.Tables[0].Rows[i]["InWork_Premises"].ToString(),
                                WitnessedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["WitnessedBy"].ToString()),
                                Witness_StmtDoc = dsIncident.Tables[0].Rows[i]["Witness_StmtDoc"].ToString(),
                                Incident_Desc = dsIncident.Tables[0].Rows[i]["Incident_Desc"].ToString(),
                                Consequences = dsIncident.Tables[0].Rows[i]["Consequences"].ToString(),
                                Damages = (dsIncident.Tables[0].Rows[i]["Damages"].ToString()),
                                Injury_Nature = dsIncident.Tables[0].Rows[i]["Injury_Nature"].ToString(),
                                Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Minor_InjuriesNo"].ToString()),
                                Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Minor_Injuries_LTINo"].ToString()),
                                Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["Major_InjuriesNo"].ToString()),
                                Actions_Taken = (dsIncident.Tables[0].Rows[i]["Actions_Taken"].ToString()),
                                Incident_Reasons = dsIncident.Tables[0].Rows[i]["Incident_Reasons"].ToString(),
                                Corrective_Measures = (dsIncident.Tables[0].Rows[i]["Corrective_Measures"].ToString()),
                                Corrective_ApprovedBy_ReviewedBy = (dsIncident.Tables[0].Rows[i]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                ReportToHSE = dsIncident.Tables[0].Rows[i]["ReportToHSE"].ToString()
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Reported_On"].ToString(), out dateValue))
                            {
                                objIncidentReport.Reported_On = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Incident_Datetime"].ToString(), out dateValue))
                            {
                                objIncidentReport.Incident_Datetime = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["DueDate"].ToString(), out dateValue))
                            {
                                objIncidentReport.DueDate = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["ClosedOn"].ToString(), out dateValue))
                            {
                                objIncidentReport.ClosedOn = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Incident_Datetime"].ToString(), out dateValue))
                            {
                                objIncidentReport.Incident_Datetime = dateValue;
                            }

                            if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["Loggeddate"].ToString(), out dateValue))
                            {
                                objIncidentReport.Loggeddate = dateValue;
                            }

                            int iValue;
                            if (int.TryParse(dsIncident.Tables[0].Rows[i]["FatalitiesNo"].ToString(), out iValue))
                            {
                                objIncidentReport.FatalitiesNo = iValue;
                            }

                            objIncidentReportList.lstIncidentReportModels.Add(objIncidentReport);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in IncidentReportListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        //
        // GET: /IncidentReport/IncidentReportDetails

        [AllowAnonymous]
        public ActionResult IncidentReportDetails()
        {
            try
            {
                IncidentReportModels objinc = new IncidentReportModels();
                IncidentReportModels objIncidentReport = new IncidentReportModels();
                // EquipmentDetailModels objEqpModel = new EquipmentDetailModels();
                if (Request.QueryString["Incident_Id"] != null && Request.QueryString["Incident_Id"] != "")
                {
                    string sIncident_Id = Request.QueryString["Incident_Id"];

                    string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                    + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                    + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                    + " ClosedOn, LoggedBy, Loggeddate,Injuries,CAPA,CAPA_Status,incident_cost,ReportToHSE,accident_reportno,risk_incident,hse_officer,witness_stmt,invest_team,report_upload,ext_witness,overall_status FROM t_incident_report where Incident_Id='" + sIncident_Id + "'";

                    DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsIncident.Tables.Count > 0)
                    {
                        objIncidentReport = new IncidentReportModels
                        {
                            Incident_Id = dsIncident.Tables[0].Rows[0]["Incident_Id"].ToString(),
                            Incident_Num = (dsIncident.Tables[0].Rows[0]["Incident_Num"].ToString()),
                            PreparedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            Incident_Type = objIncidentReport.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[0]["Incident_Type"].ToString()),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[0]["Location"].ToString()),
                            Occurred_onJob = (dsIncident.Tables[0].Rows[0]["Occurred_onJob"].ToString()),
                            InWork_Premises = dsIncident.Tables[0].Rows[0]["InWork_Premises"].ToString(),
                            WitnessedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["WitnessedBy"].ToString()),
                            Witness_StmtDoc = dsIncident.Tables[0].Rows[0]["Witness_StmtDoc"].ToString(),
                            Incident_Desc = dsIncident.Tables[0].Rows[0]["Incident_Desc"].ToString(),
                            Consequences = dsIncident.Tables[0].Rows[0]["Consequences"].ToString(),
                            Damages = (dsIncident.Tables[0].Rows[0]["Damages"].ToString()),
                            Injury_Nature = dsIncident.Tables[0].Rows[0]["Injury_Nature"].ToString(),
                            Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_InjuriesNo"].ToString()),
                            Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_Injuries_LTINo"].ToString()),
                            Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Major_InjuriesNo"].ToString()),
                            Actions_Taken = (dsIncident.Tables[0].Rows[0]["Actions_Taken"].ToString()),
                            Incident_Reasons = dsIncident.Tables[0].Rows[0]["Incident_Reasons"].ToString(),
                            Corrective_Measures = (dsIncident.Tables[0].Rows[0]["Corrective_Measures"].ToString()),
                            Corrective_ApprovedBy_ReviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Injuries = dsIncident.Tables[0].Rows[0]["Injuries"].ToString(),
                            CAPA = dsIncident.Tables[0].Rows[0]["CAPA"].ToString(),
                            CAPA_Status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["CAPA_Status"].ToString()),
                            incident_cost = dsIncident.Tables[0].Rows[0]["incident_cost"].ToString(),
                            ReportToHSE = dsIncident.Tables[0].Rows[0]["ReportToHSE"].ToString(),
                            accident_reportno = dsIncident.Tables[0].Rows[0]["accident_reportno"].ToString(),
                            risk_incident = dsIncident.Tables[0].Rows[0]["risk_incident"].ToString(),
                            hse_officer = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["hse_officer"].ToString()),
                            witness_stmt = dsIncident.Tables[0].Rows[0]["witness_stmt"].ToString(),
                            invest_team = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["invest_team"].ToString()),
                            report_upload = dsIncident.Tables[0].Rows[0]["report_upload"].ToString(),
                            ext_witness = dsIncident.Tables[0].Rows[0]["ext_witness"].ToString(),
                            overall_status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["overall_status"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                        {
                            objIncidentReport.Reported_On = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Incident_Datetime"].ToString(), out dateValue))
                        {
                            objIncidentReport.Incident_Datetime = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["DueDate"].ToString(), out dateValue))
                        {
                            objIncidentReport.DueDate = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["ClosedOn"].ToString(), out dateValue))
                        {
                            objIncidentReport.ClosedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Loggeddate"].ToString(), out dateValue))
                        {
                            objIncidentReport.Loggeddate = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsIncident.Tables[0].Rows[0]["FatalitiesNo"].ToString(), out iValue))
                        {
                            objIncidentReport.FatalitiesNo = iValue;
                        }

                        IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                        objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                        sSqlstmt = "SELECT LTI_Id, Incident_Id, AccidentType, Emp_Id, LTI_Hrs,injury_type,invest_start_date,invest_end_date FROM t_incident_report_lti where Incident_Id='" + sIncident_Id
                            + "' order by AccidentType asc";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        LTI_Id = (dsIncident.Tables[0].Rows[i]["LTI_Id"].ToString()),
                                        AccidentType = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["AccidentType"].ToString()),
                                        Emp_Id = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["Emp_Id"].ToString()),
                                        LTI_Hrs = dsIncident.Tables[0].Rows[i]["LTI_Hrs"].ToString(),
                                        injury_type = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["injury_type"].ToString()),
                                    };
                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["invest_start_date"].ToString(), out dateValue))
                                    {
                                        objIncidentLTI.invest_start_date = dateValue;
                                    }
                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["invest_end_date"].ToString(), out dateValue))
                                    {
                                        objIncidentLTI.invest_end_date = dateValue;
                                    }
                                    objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                                }
                                catch (Exception ex)
                                { }
                            }
                            ViewBag.objIncidentLTIList = objIncidentLTIList;
                        }
                        IncidentReportModelsList objActionList = new IncidentReportModelsList();
                        objActionList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select id_incident_action,Incident_Id,incident_action,resp_pers,target_date,incident_status,contractor,"
                        + " action_report from t_incident_action where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objAction = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        incident_action = (dsIncident.Tables[0].Rows[i]["incident_action"].ToString()),
                                        resp_pers = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["resp_pers"].ToString()),
                                        contractor = (dsIncident.Tables[0].Rows[i]["contractor"].ToString()),
                                        incident_status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["incident_status"].ToString()),
                                        action_report = dsIncident.Tables[0].Rows[i]["action_report"].ToString()
                                    };

                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["target_date"].ToString(), out dateValue))
                                    {
                                        objAction.target_date = dateValue;
                                    }

                                    objActionList.lstIncidentReportModels.Add(objAction);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objActionList = objActionList;
                        }

                        IncidentReportModelsList objCloseList = new IncidentReportModelsList();
                        objCloseList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select Incident_Id,closed_by,invest_result,invest_upload from t_incident_closeout where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objClose = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        invest_result = (dsIncident.Tables[0].Rows[i]["invest_result"].ToString()),
                                        closed_by = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["closed_by"].ToString()),
                                        invest_upload = (dsIncident.Tables[0].Rows[i]["invest_upload"].ToString()),
                                    };

                                    objCloseList.lstIncidentReportModels.Add(objClose);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objCloseList = objCloseList;
                        }

                        string sSqlstmt1 = "select acc_date,reported_date,reported_by,details,Incident_Type from t_accident_report"
                + " where id_accident_rept = '" + objIncidentReport.accident_reportno + "'";
                        DataSet dsAccident = objGlobaldata.Getdetails(sSqlstmt1);

                        if (dsAccident.Tables.Count > 0 && dsAccident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAccident.Tables[0].Rows.Count; i++)
                            {
                                objIncidentReport.reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccident.Tables[0].Rows[i]["reported_by"].ToString());
                                objIncidentReport.details = dsAccident.Tables[0].Rows[i]["details"].ToString();
                                objIncidentReport.Incident_Type = objinc.GetIncidentTypeNameById(dsAccident.Tables[0].Rows[i]["Incident_Type"].ToString());

                                DateTime dtDocDate;
                                if (dsAccident.Tables[0].Rows[i]["acc_date"].ToString() != ""
                                   && DateTime.TryParse(dsAccident.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                                {
                                    objIncidentReport.acc_date = dtDocDate;
                                }
                                if (dsAccident.Tables[0].Rows[i]["reported_date"].ToString() != ""
                                   && DateTime.TryParse(dsAccident.Tables[0].Rows[i]["reported_date"].ToString(), out dtDocDate))
                                {
                                    objIncidentReport.reported_date = dtDocDate;
                                }
                            }
                        }

                        return View(objIncidentReport);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("IncidentReportList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("IncidentReportList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("IncidentReportList");
        }

        [AllowAnonymous]
        public ActionResult IncidentReportInfo(int id)
        {
            try
            {
                IncidentReportModels objinc = new IncidentReportModels();
                // EquipmentDetailModels objEqpModel = new EquipmentDetailModels();

                string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                + " ClosedOn, LoggedBy, Loggeddate,Injuries,CAPA,CAPA_Status,incident_cost,ReportToHSE FROM t_incident_report where Incident_Id='" + id + "'";

                DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                if (dsIncident.Tables.Count > 0)
                {
                    IncidentReportModels objIncidentReport = new IncidentReportModels
                    {
                        Incident_Id = dsIncident.Tables[0].Rows[0]["Incident_Id"].ToString(),
                        Incident_Num = (dsIncident.Tables[0].Rows[0]["Incident_Num"].ToString()),
                        PreparedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["PreparedBy"].ToString()),
                        Incident_Type = objinc.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[0]["Incident_Type"].ToString()),
                        Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[0]["Location"].ToString()),
                        Occurred_onJob = (dsIncident.Tables[0].Rows[0]["Occurred_onJob"].ToString()),
                        InWork_Premises = dsIncident.Tables[0].Rows[0]["InWork_Premises"].ToString(),
                        WitnessedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["WitnessedBy"].ToString()),
                        Witness_StmtDoc = dsIncident.Tables[0].Rows[0]["Witness_StmtDoc"].ToString(),
                        Incident_Desc = dsIncident.Tables[0].Rows[0]["Incident_Desc"].ToString(),
                        Consequences = dsIncident.Tables[0].Rows[0]["Consequences"].ToString(),
                        Damages = (dsIncident.Tables[0].Rows[0]["Damages"].ToString()),
                        Injury_Nature = dsIncident.Tables[0].Rows[0]["Injury_Nature"].ToString(),
                        Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_InjuriesNo"].ToString()),
                        Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_Injuries_LTINo"].ToString()),
                        Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Major_InjuriesNo"].ToString()),
                        Actions_Taken = (dsIncident.Tables[0].Rows[0]["Actions_Taken"].ToString()),
                        Incident_Reasons = dsIncident.Tables[0].Rows[0]["Incident_Reasons"].ToString(),
                        Corrective_Measures = (dsIncident.Tables[0].Rows[0]["Corrective_Measures"].ToString()),
                        Corrective_ApprovedBy_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        Injuries = dsIncident.Tables[0].Rows[0]["Injuries"].ToString(),
                        CAPA = dsIncident.Tables[0].Rows[0]["CAPA"].ToString(),
                        CAPA_Status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["CAPA_Status"].ToString()),
                        incident_cost = dsIncident.Tables[0].Rows[0]["incident_cost"].ToString(),
                        ReportToHSE = dsIncident.Tables[0].Rows[0]["ReportToHSE"].ToString()
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                    {
                        objIncidentReport.Reported_On = dateValue;
                    }

                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Incident_Datetime"].ToString(), out dateValue))
                    {
                        objIncidentReport.Incident_Datetime = dateValue;
                    }

                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["DueDate"].ToString(), out dateValue))
                    {
                        objIncidentReport.DueDate = dateValue;
                    }

                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["ClosedOn"].ToString(), out dateValue))
                    {
                        objIncidentReport.ClosedOn = dateValue;
                    }

                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Loggeddate"].ToString(), out dateValue))
                    {
                        objIncidentReport.Loggeddate = dateValue;
                    }

                    int iValue;
                    if (int.TryParse(dsIncident.Tables[0].Rows[0]["FatalitiesNo"].ToString(), out iValue))
                    {
                        objIncidentReport.FatalitiesNo = iValue;
                    }

                    IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                    objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                    sSqlstmt = "SELECT LTI_Id, Incident_Id, AccidentType, Emp_Id, LTI_Hrs FROM t_incident_report_lti where Incident_Id='" + id
                        + "' order by AccidentType asc";

                    dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                                {
                                    Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                    LTI_Id = (dsIncident.Tables[0].Rows[i]["LTI_Id"].ToString()),
                                    AccidentType = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["AccidentType"].ToString()),
                                    Emp_Id = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["Emp_Id"].ToString()),
                                    LTI_Hrs = dsIncident.Tables[0].Rows[i]["LTI_Hrs"].ToString()
                                };
                                objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in IncidentReportInfo: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        ViewBag.objIncidentLTIList = objIncidentLTIList;
                    }

                    return View(objIncidentReport);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("IncidentReportList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("IncidentReportList");
        }

        //
        // GET: /IncidentReport/IncidentReportEdit

        [HttpGet]
        [AllowAnonymous]
        public ActionResult IncidentReportEdit()
        {
            IncidentReportModels objInc = new IncidentReportModels();

            // EquipmentCategoryModels objEqpModel = new EquipmentCategoryModels();
            try
            {
                if (Request.QueryString["Incident_Id"] != null && Request.QueryString["Incident_Id"] != "")
                {
                    string sIncident_Id = Request.QueryString["Incident_Id"];
                    ViewBag.CurrentEmp = objGlobaldata.GetCurrentUserSession().CompEmpId;
                    string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                    + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                    + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                    + " ClosedOn, LoggedBy, Loggeddate,Injuries,CAPA,CAPA_Status,incident_cost,ReportToHSE,accident_reportno,risk_incident,hse_officer,witness_stmt,invest_team,report_upload,ext_witness,overall_status FROM t_incident_report where Incident_Id='" + sIncident_Id + "'";

                    DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsIncident.Tables.Count > 0)
                    {
                        IncidentReportModels objIncidentReport = new IncidentReportModels
                        {
                            Incident_Id = dsIncident.Tables[0].Rows[0]["Incident_Id"].ToString(),
                            Incident_Num = (dsIncident.Tables[0].Rows[0]["Incident_Num"].ToString()),
                            PreparedBy = dsIncident.Tables[0].Rows[0]["PreparedBy"].ToString(),
                            Incident_Type = objInc.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[0]["Incident_Type"].ToString()),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[0]["Location"].ToString()),
                            Occurred_onJob = (dsIncident.Tables[0].Rows[0]["Occurred_onJob"].ToString()),
                            InWork_Premises = dsIncident.Tables[0].Rows[0]["InWork_Premises"].ToString(),
                            WitnessedBy = (dsIncident.Tables[0].Rows[0]["WitnessedBy"].ToString()),
                            Witness_StmtDoc = dsIncident.Tables[0].Rows[0]["Witness_StmtDoc"].ToString(),
                            Incident_Desc = dsIncident.Tables[0].Rows[0]["Incident_Desc"].ToString(),
                            Consequences = dsIncident.Tables[0].Rows[0]["Consequences"].ToString(),
                            Damages = (dsIncident.Tables[0].Rows[0]["Damages"].ToString()),
                            Injury_Nature = dsIncident.Tables[0].Rows[0]["Injury_Nature"].ToString(),
                            Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_InjuriesNo"].ToString()),
                            Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_Injuries_LTINo"].ToString()),
                            Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Major_InjuriesNo"].ToString()),
                            Actions_Taken = (dsIncident.Tables[0].Rows[0]["Actions_Taken"].ToString()),
                            Incident_Reasons = dsIncident.Tables[0].Rows[0]["Incident_Reasons"].ToString(),
                            Corrective_Measures = (dsIncident.Tables[0].Rows[0]["Corrective_Measures"].ToString()),
                            Corrective_ApprovedBy_ReviewedBy = (dsIncident.Tables[0].Rows[0]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Injuries = (dsIncident.Tables[0].Rows[0]["Injuries"].ToString()),
                            CAPA = (dsIncident.Tables[0].Rows[0]["CAPA"].ToString()),
                            CAPA_Status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["CAPA_Status"].ToString()),
                            incident_cost = (dsIncident.Tables[0].Rows[0]["incident_cost"].ToString()),
                            ReportToHSE = dsIncident.Tables[0].Rows[0]["ReportToHSE"].ToString(),
                            accident_reportno = dsIncident.Tables[0].Rows[0]["accident_reportno"].ToString(),
                            risk_incident = dsIncident.Tables[0].Rows[0]["risk_incident"].ToString(),
                            hse_officer = dsIncident.Tables[0].Rows[0]["hse_officer"].ToString(),
                            witness_stmt = dsIncident.Tables[0].Rows[0]["witness_stmt"].ToString(),
                            invest_team = dsIncident.Tables[0].Rows[0]["invest_team"].ToString(),
                            report_upload = dsIncident.Tables[0].Rows[0]["report_upload"].ToString(),
                            ext_witness = dsIncident.Tables[0].Rows[0]["ext_witness"].ToString(),
                            overall_status = dsIncident.Tables[0].Rows[0]["overall_status"].ToString(),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                        {
                            objIncidentReport.Reported_On = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Incident_Datetime"].ToString(), out dateValue))
                        {
                            objIncidentReport.Incident_Datetime = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["DueDate"].ToString(), out dateValue))
                        {
                            objIncidentReport.DueDate = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["ClosedOn"].ToString(), out dateValue))
                        {
                            objIncidentReport.ClosedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Loggeddate"].ToString(), out dateValue))
                        {
                            objIncidentReport.Loggeddate = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsIncident.Tables[0].Rows[0]["FatalitiesNo"].ToString(), out iValue))
                        {
                            objIncidentReport.FatalitiesNo = iValue;
                        }

                        IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                        objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                        sSqlstmt = "SELECT LTI_Id, Incident_Id, AccidentType, Emp_Id, LTI_Hrs,injury_type,invest_start_date,invest_end_date FROM t_incident_report_lti where Incident_Id='" + sIncident_Id
                            + "' order by AccidentType asc";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        LTI_Id = (dsIncident.Tables[0].Rows[i]["LTI_Id"].ToString()),
                                        AccidentType = (dsIncident.Tables[0].Rows[i]["AccidentType"].ToString()),
                                        Emp_Id = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["Emp_Id"].ToString()),
                                        LTI_Hrs = dsIncident.Tables[0].Rows[i]["LTI_Hrs"].ToString(),
                                        injury_type = dsIncident.Tables[0].Rows[i]["injury_type"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["invest_start_date"].ToString(), out dateValue))
                                    {
                                        objIncidentLTI.invest_start_date = dateValue;
                                    }
                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["invest_end_date"].ToString(), out dateValue))
                                    {
                                        objIncidentLTI.invest_end_date = dateValue;
                                    }
                                    objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objIncidentLTIList = objIncidentLTIList;
                        }

                        IncidentReportModelsList objActionList = new IncidentReportModelsList();
                        objActionList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select id_incident_action,Incident_Id,incident_action,resp_pers,target_date,incident_status,contractor,"
                        + " action_report from t_incident_action where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objAction = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        incident_action = (dsIncident.Tables[0].Rows[i]["incident_action"].ToString()),
                                        resp_pers = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["resp_pers"].ToString()),
                                        contractor = (dsIncident.Tables[0].Rows[i]["contractor"].ToString()),
                                        incident_status = dsIncident.Tables[0].Rows[i]["incident_status"].ToString(),
                                        action_report = dsIncident.Tables[0].Rows[i]["action_report"].ToString()
                                    };

                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["target_date"].ToString(), out dateValue))
                                    {
                                        objAction.target_date = dateValue;
                                    }

                                    objActionList.lstIncidentReportModels.Add(objAction);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objActionList = objActionList;
                        }

                        IncidentReportModelsList objCloseList = new IncidentReportModelsList();
                        objCloseList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select Incident_Id,closed_by,invest_result,invest_upload from t_incident_closeout where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objClose = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        invest_result = (dsIncident.Tables[0].Rows[i]["invest_result"].ToString()),
                                        closed_by = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["closed_by"].ToString()),
                                        invest_upload = (dsIncident.Tables[0].Rows[i]["invest_upload"].ToString()),
                                    };

                                    objCloseList.lstIncidentReportModels.Add(objClose);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objCloseList = objCloseList;
                        }

                        // ViewBag.CAPAStatus = objGlobaldata.GetConstantValue("CAPAStatus");
                        ViewBag.CAPAStatus = objGlobaldata.GetDropdownList("Meeting Item Status");
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.InjuryNature = objGlobaldata.GetConstantValue("InjuryNature");
                        ViewBag.Incident_Type = objGlobaldata.GetDropdownList("Incident Type");
                        ViewBag.AccidentType = objGlobaldata.GetDropdownList("Accident Type");
                        ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.AccidentReport = objGlobaldata.GetAccidentReportsWithInvestList();
                        ViewBag.HSEEmp = objGlobaldata.GetHrEmpHseList();
                        ViewBag.Status = objGlobaldata.GetDropdownList("Incident Status");
                        ViewBag.Injury = objGlobaldata.GetDropdownList("Type of Injury");

                        if (objGlobaldata.GetIncidentInvestigators(sIncident_Id) != "" && objGlobaldata.GetIncidentInvestigators(sIncident_Id) != null)
                        {
                            ViewBag.InvestTeam = objGlobaldata.GetHrMultiEmployeeListByID(objGlobaldata.GetIncidentInvestigators(sIncident_Id));
                        }
                        else
                        {
                            ViewBag.InvestTeam = "";
                        }

                        return View(objIncidentReport);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("IncidentReportList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Tool Talk Id cannot be null";
                    return RedirectToAction("IncidentReportList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("IncidentReportList");
        }

        // POST: /PPEIssueLog/IncidentReportEdit

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IncidentReportEdit(IncidentReportModels objIncidentReport, FormCollection form, IEnumerable<HttpPostedFileBase> Witness_StmtDoc, IEnumerable<HttpPostedFileBase> report_upload)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                string QCDelete1 = Request.Form["QCDocsValselectall1"];

                IList<HttpPostedFileBase> Witness_StmtDocList = (IList<HttpPostedFileBase>)Witness_StmtDoc;
                IList<HttpPostedFileBase> report_uploadList = (IList<HttpPostedFileBase>)report_upload;

                DateTime dateValue;
                objIncidentReport.Minor_InjuriesNo = form["Minor_InjuriesNo"];
                objIncidentReport.Minor_Injuries_LTINo = form["Minor_Injuries_LTINo"];
                objIncidentReport.Major_InjuriesNo = form["Major_InjuriesNo"];
                objIncidentReport.ReportToHSE = form["ReportToHSE"];
                objIncidentReport.Location = form["Location"];
                objIncidentReport.hse_officer = form["hse_officer"];
                objIncidentReport.invest_team = form["invest_team"];
                objIncidentReport.WitnessedBy = form["WitnessedBy"];
                if (form["Reported_On"] != null && DateTime.TryParse(form["Reported_On"], out dateValue) == true)
                {
                    objIncidentReport.Reported_On = dateValue;
                }

                if (form["Incident_Datetime"] != null && DateTime.TryParse(form["Incident_Datetime"], out dateValue) == true)
                {
                    objIncidentReport.Incident_Datetime = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objIncidentReport.Incident_Datetime = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }
                }

                if (form["DueDate"] != null && DateTime.TryParse(form["DueDate"], out dateValue) == true)
                {
                    objIncidentReport.DueDate = dateValue;
                }

                if (form["ClosedOn"] != null && DateTime.TryParse(form["ClosedOn"], out dateValue) == true)
                {
                    objIncidentReport.ClosedOn = dateValue;
                }

                if (Witness_StmtDocList[0] != null)
                {
                    objIncidentReport.Witness_StmtDoc = "";
                    foreach (var file in Witness_StmtDoc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Incident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc + "," + form["QCDocsVal"];
                    objIncidentReport.Witness_StmtDoc = objIncidentReport.Witness_StmtDoc.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && Witness_StmtDocList[0] == null)
                {
                    objIncidentReport.Witness_StmtDoc = null;
                }
                else if (form["QCDocsVal"] == null && Witness_StmtDocList[0] == null)
                {
                    objIncidentReport.Witness_StmtDoc = null;
                }

                if (report_uploadList[0] != null)
                {
                    objIncidentReport.report_upload = "";
                    foreach (var file in report_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                            string sFilename = "Incident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objIncidentReport.report_upload = objIncidentReport.report_upload + "," + "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddIncidentReport: " + ex.ToString());
                        }
                    }
                    objIncidentReport.report_upload = objIncidentReport.report_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal1"] != null && form["QCDocsVal1"] != "")
                {
                    objIncidentReport.report_upload = objIncidentReport.report_upload + "," + form["QCDocsVal1"];
                    objIncidentReport.report_upload = objIncidentReport.report_upload.Trim(',');
                }
                else if (form["QCDocsVal1"] == null && QCDelete1 != null && report_uploadList[0] == null)
                {
                    objIncidentReport.report_upload = null;
                }
                else if (form["QCDocsVal1"] == null && report_uploadList[0] == null)
                {
                    objIncidentReport.report_upload = null;
                }

                IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["LTI_Id " + i] != null || form["AccidentType " + i] != null)
                        {
                            IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                            {
                                Incident_Id = objIncidentReport.Incident_Id,
                                LTI_Id = form["LTI_Id " + i],
                                AccidentType = form["AccidentType " + i],
                                Emp_Id = form["Emp_Id " + i],
                                LTI_Hrs = form["LTI_Hrs " + i],
                                injury_type = form["injury_type " + i]
                            };
                            if (form["invest_start_date " + i] != null && DateTime.TryParse(form["invest_start_date " + i], out dateValue) == true)
                            {
                                objIncidentLTI.invest_start_date = dateValue;
                            }
                            if (form["invest_end_date " + i] != null && DateTime.TryParse(form["invest_end_date " + i], out dateValue) == true)
                            {
                                objIncidentLTI.invest_end_date = dateValue;
                            }
                            objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                        }
                    }
                }
                IncidentReportModelsList objActionList = new IncidentReportModelsList();
                objActionList.lstIncidentReportModels = new List<IncidentReportModels>();

                if (form["itemcnts"] != null && form["itemcnts"] != "" && int.TryParse(form["itemcnts"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                    {
                        if (form["incident_action " + i] != null)
                        {
                            IncidentReportModels objAction = new IncidentReportModels
                            {
                                Incident_Id = objIncidentReport.Incident_Id,
                                incident_action = form["incident_action " + i],
                                resp_pers = form["resp_pers " + i],
                                incident_status = form["incident_status " + i],
                                contractor = form["contractor " + i],
                                action_report = form["action_report " + i]
                            };
                            if (DateTime.TryParse(form["target_date " + i], out dateValue) == true)
                            {
                                objAction.target_date = dateValue;
                            }
                            objActionList.lstIncidentReportModels.Add(objAction);
                        }
                    }
                }

                IncidentReportModelsList objCloseList = new IncidentReportModelsList();
                objCloseList.lstIncidentReportModels = new List<IncidentReportModels>();

                if (form["itemcnts1"] != null && form["itemcnts1"] != "" && int.TryParse(form["itemcnts1"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnts1"]); i++)
                    {
                        if (form["invest_result " + i] != null)
                        {
                            IncidentReportModels objClose = new IncidentReportModels
                            {
                                Incident_Id = objIncidentReport.Incident_Id,
                                closed_by = form["closed_by " + i],
                                invest_result = form["invest_result " + i],
                                invest_upload = form["invest_upload " + i],
                            };

                            objCloseList.lstIncidentReportModels.Add(objClose);
                        }
                    }
                }
                if (objIncidentReport.FunUpdateIncidentReport(objIncidentReport, objIncidentLTIList, objActionList, objCloseList))
                {
                    TempData["Successdata"] = "Updated Incident report details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult IncidentReportPDF(FormCollection form)
        {
            CompanyModels objCompany = new CompanyModels();
            try
            {
                string sIncident_Id = form["Incident_Id"];

                IncidentReportModels objinc = new IncidentReportModels();
                IncidentReportModels objIncidentReport = new IncidentReportModels();
                // EquipmentDetailModels objEqpModel = new EquipmentDetailModels();

                if (sIncident_Id != null && sIncident_Id != "")
                {
                    string sSqlstmt = "SELECT Incident_Id, Incident_Num, Reported_On, PreparedBy, Incident_Type, Incident_Datetime, Location, Occurred_onJob, InWork_Premises, "
                   + " WitnessedBy, Witness_StmtDoc, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                   + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, DueDate,"
                   + " ClosedOn, LoggedBy, Loggeddate,Injuries,CAPA,CAPA_Status,incident_cost,ReportToHSE,accident_reportno,risk_incident,hse_officer,witness_stmt,invest_team,report_upload,ext_witness,overall_status FROM t_incident_report where Incident_Id='" + sIncident_Id + "'";

                    DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsIncident.Tables.Count > 0)
                    {
                        objIncidentReport = new IncidentReportModels
                        {
                            Incident_Id = dsIncident.Tables[0].Rows[0]["Incident_Id"].ToString(),
                            Incident_Num = (dsIncident.Tables[0].Rows[0]["Incident_Num"].ToString()),
                            PreparedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["PreparedBy"].ToString()),
                            Incident_Type = objIncidentReport.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[0]["Incident_Type"].ToString()),
                            Location = objGlobaldata.GetCompanyBranchNameById(dsIncident.Tables[0].Rows[0]["Location"].ToString()),
                            Occurred_onJob = (dsIncident.Tables[0].Rows[0]["Occurred_onJob"].ToString()),
                            InWork_Premises = dsIncident.Tables[0].Rows[0]["InWork_Premises"].ToString(),
                            WitnessedBy = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["WitnessedBy"].ToString()),
                            Witness_StmtDoc = dsIncident.Tables[0].Rows[0]["Witness_StmtDoc"].ToString(),
                            Incident_Desc = dsIncident.Tables[0].Rows[0]["Incident_Desc"].ToString(),
                            Consequences = dsIncident.Tables[0].Rows[0]["Consequences"].ToString(),
                            Damages = (dsIncident.Tables[0].Rows[0]["Damages"].ToString()),
                            Injury_Nature = dsIncident.Tables[0].Rows[0]["Injury_Nature"].ToString(),
                            Minor_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_InjuriesNo"].ToString()),
                            Minor_Injuries_LTINo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Minor_Injuries_LTINo"].ToString()),
                            Major_InjuriesNo = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["Major_InjuriesNo"].ToString()),
                            Actions_Taken = (dsIncident.Tables[0].Rows[0]["Actions_Taken"].ToString()),
                            Incident_Reasons = dsIncident.Tables[0].Rows[0]["Incident_Reasons"].ToString(),
                            Corrective_Measures = (dsIncident.Tables[0].Rows[0]["Corrective_Measures"].ToString()),
                            Corrective_ApprovedBy_ReviewedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["Corrective_ApprovedBy_ReviewedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Injuries = dsIncident.Tables[0].Rows[0]["Injuries"].ToString(),
                            CAPA = dsIncident.Tables[0].Rows[0]["CAPA"].ToString(),
                            CAPA_Status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["CAPA_Status"].ToString()),
                            incident_cost = dsIncident.Tables[0].Rows[0]["incident_cost"].ToString(),
                            ReportToHSE = dsIncident.Tables[0].Rows[0]["ReportToHSE"].ToString(),
                            accident_reportno = dsIncident.Tables[0].Rows[0]["accident_reportno"].ToString(),
                            risk_incident = dsIncident.Tables[0].Rows[0]["risk_incident"].ToString(),
                            hse_officer = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["hse_officer"].ToString()),
                            witness_stmt = dsIncident.Tables[0].Rows[0]["witness_stmt"].ToString(),
                            invest_team = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[0]["invest_team"].ToString()),
                            report_upload = dsIncident.Tables[0].Rows[0]["report_upload"].ToString(),
                            ext_witness = dsIncident.Tables[0].Rows[0]["ext_witness"].ToString(),
                            overall_status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[0]["overall_status"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Reported_On"].ToString(), out dateValue))
                        {
                            objIncidentReport.Reported_On = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Incident_Datetime"].ToString(), out dateValue))
                        {
                            objIncidentReport.Incident_Datetime = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["DueDate"].ToString(), out dateValue))
                        {
                            objIncidentReport.DueDate = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["ClosedOn"].ToString(), out dateValue))
                        {
                            objIncidentReport.ClosedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsIncident.Tables[0].Rows[0]["Loggeddate"].ToString(), out dateValue))
                        {
                            objIncidentReport.Loggeddate = dateValue;
                        }

                        int iValue;
                        if (int.TryParse(dsIncident.Tables[0].Rows[0]["FatalitiesNo"].ToString(), out iValue))
                        {
                            objIncidentReport.FatalitiesNo = iValue;
                        }

                        string sql = "select reported_by from t_accident_report where id_accident_rept='" + objIncidentReport.accident_reportno + "'";
                        DataSet dsData = objGlobaldata.Getdetails(sql);

                        dsIncident = objCompany.GetCompanyDetailsForReport(dsIncident);
                        dsIncident = objGlobaldata.GetReportDetails(dsIncident, objIncidentReport.Incident_Num, dsIncident.Tables[0].Rows[0]["LoggedBy"].ToString(), "INCIDENT INVESTIGATION REPORT");

                        ViewBag.CompanyInfo = dsIncident;

                        IncidentLTIModelsList objIncidentLTIList = new IncidentLTIModelsList();
                        objIncidentLTIList.lstIncidentLTIModels = new List<IncidentLTIModels>();

                        sSqlstmt = "SELECT LTI_Id, Incident_Id, AccidentType, Emp_Id, LTI_Hrs FROM t_incident_report_lti where Incident_Id='" + sIncident_Id
                            + "' order by AccidentType asc";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentLTIModels objIncidentLTI = new IncidentLTIModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        LTI_Id = (dsIncident.Tables[0].Rows[i]["LTI_Id"].ToString()),
                                        AccidentType = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["AccidentType"].ToString()),
                                        Emp_Id = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["Emp_Id"].ToString()),
                                        LTI_Hrs = dsIncident.Tables[0].Rows[i]["LTI_Hrs"].ToString()
                                    };
                                    objIncidentLTIList.lstIncidentLTIModels.Add(objIncidentLTI);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objIncidentLTIList = objIncidentLTIList;
                        }
                        IncidentReportModelsList objActionList = new IncidentReportModelsList();
                        objActionList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select id_incident_action,Incident_Id,incident_action,resp_pers,target_date,incident_status,contractor,"
                        + " action_report from t_incident_action where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objAction = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        incident_action = (dsIncident.Tables[0].Rows[i]["incident_action"].ToString()),
                                        resp_pers = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["resp_pers"].ToString()),
                                        contractor = (dsIncident.Tables[0].Rows[i]["contractor"].ToString()),
                                        incident_status = objGlobaldata.GetDropdownitemById(dsIncident.Tables[0].Rows[i]["incident_status"].ToString()),
                                        action_report = dsIncident.Tables[0].Rows[i]["action_report"].ToString()
                                    };

                                    if (DateTime.TryParse(dsIncident.Tables[0].Rows[i]["target_date"].ToString(), out dateValue))
                                    {
                                        objAction.target_date = dateValue;
                                    }

                                    objActionList.lstIncidentReportModels.Add(objAction);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objActionList = objActionList;
                        }

                        IncidentReportModelsList objCloseList = new IncidentReportModelsList();
                        objCloseList.lstIncidentReportModels = new List<IncidentReportModels>();

                        sSqlstmt = "select Incident_Id,closed_by,invest_result,invest_upload from t_incident_closeout where Incident_Id = '" + sIncident_Id + "'";

                        dsIncident = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    IncidentReportModels objClose = new IncidentReportModels
                                    {
                                        Incident_Id = dsIncident.Tables[0].Rows[i]["Incident_Id"].ToString(),
                                        invest_result = (dsIncident.Tables[0].Rows[i]["invest_result"].ToString()),
                                        closed_by = objGlobaldata.GetEmpHrNameById(dsIncident.Tables[0].Rows[i]["closed_by"].ToString()),
                                        invest_upload = (dsIncident.Tables[0].Rows[i]["invest_upload"].ToString()),
                                    };

                                    objCloseList.lstIncidentReportModels.Add(objClose);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in IncidentReportEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.objCloseList = objCloseList;
                        }

                        string sSqlstmt1 = "select acc_date,reported_date,reported_by,details,Incident_Type from t_accident_report"
               + " where id_accident_rept = '" + objIncidentReport.accident_reportno + "'";
                        DataSet dsAccident = objGlobaldata.Getdetails(sSqlstmt1);

                        if (dsAccident.Tables.Count > 0 && dsAccident.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsAccident.Tables[0].Rows.Count; i++)
                            {
                                objIncidentReport.reported_by = objGlobaldata.GetMultiHrEmpNameById(dsAccident.Tables[0].Rows[i]["reported_by"].ToString());
                                objIncidentReport.details = dsAccident.Tables[0].Rows[i]["details"].ToString();
                                objIncidentReport.Incident_Type = objinc.GetIncidentTypeNameById(dsAccident.Tables[0].Rows[i]["Incident_Type"].ToString());

                                DateTime dtDocDate;
                                if (dsAccident.Tables[0].Rows[i]["acc_date"].ToString() != ""
                                   && DateTime.TryParse(dsAccident.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                                {
                                    objIncidentReport.acc_date = dtDocDate;
                                }
                                if (dsAccident.Tables[0].Rows[i]["reported_date"].ToString() != ""
                                   && DateTime.TryParse(dsAccident.Tables[0].Rows[i]["reported_date"].ToString(), out dtDocDate))
                                {
                                    objIncidentReport.reported_date = dtDocDate;
                                }
                            }
                        }

                        ViewBag.Incident = objIncidentReport;
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("IncidentReportList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("IncidentReportList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IncidentReportPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("IncidentReportToPDF")
            {
                //FileName = "IncidentReport.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [HttpPost]
        public JsonResult FunGetIncidentDesc()
        {
            string[] len = new string[100];
            DataSet dsIncident = new DataSet();
            try
            {
                string sSqlstmt = "select item_desc,item_fulldesc from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Incident Type' order by item_desc asc";
                dsIncident = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                    {
                        len[i] = dsIncident.Tables[0].Rows[i]["item_fulldesc"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetIncidentDesc: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(len);
        }

        [HttpPost]
        public JsonResult FunGetAccidentReportDetails(string accident_reportno)
        {
            IncidentReportModels objinc = new IncidentReportModels();
            AccidentReportModels objModel = new AccidentReportModels();
            try
            {
                string sSqlstmt = "select acc_date,reported_date,reported_by,details,Incident_Type from t_accident_report"
                + " where id_accident_rept = '" + accident_reportno + "'";
                DataSet dsIncident = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIncident.Tables.Count > 0 && dsIncident.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsIncident.Tables[0].Rows.Count; i++)
                    {
                        objModel = new AccidentReportModels
                        {
                            reported_by = objGlobaldata.GetMultiHrEmpNameById(dsIncident.Tables[0].Rows[i]["reported_by"].ToString()),
                            details = dsIncident.Tables[0].Rows[i]["details"].ToString(),
                            Incident_Type = objinc.GetIncidentTypeNameById(dsIncident.Tables[0].Rows[i]["Incident_Type"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsIncident.Tables[0].Rows[i]["acc_date"].ToString() != ""
                           && DateTime.TryParse(dsIncident.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                        {
                            objModel.acc_date = dtDocDate;
                        }
                        if (dsIncident.Tables[0].Rows[i]["reported_date"].ToString() != ""
                           && DateTime.TryParse(dsIncident.Tables[0].Rows[i]["reported_date"].ToString(), out dtDocDate))
                        {
                            objModel.reported_date = dtDocDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetAccidentReportDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(objModel);
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase report_upload = Request.Files;
            IncidentReportModels obj = new IncidentReportModels();
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "Incident" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                //return Json("~/DataUpload/MgmtDocs/Surveillance/" + "Surveillance" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                obj.action_report = obj.action_report + "," + "~/DataUpload/MgmtDocs/HSE/" + "Incident" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename;
            }
            obj.action_report = obj.action_report.Trim(',');
            return Json(obj.action_report);
        }
    }
}