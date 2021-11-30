using ISOStd.Models;
using Microsoft.Reporting.WebForms;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using ClosedXML.Excel;
using ISOStd.Filters;


namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ReportsController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public ReportsController()
        {
            //ViewBag.Menutype = "Suppliers";
            ViewBag.SubMenutype = "ReportList";
        }

        //
        // GET: /Reports/
         
        public ActionResult Index()
        {
            return View();
        }

         
        [AllowAnonymous]
        public ActionResult DashboardReports(string ReportType, string Years)
        {
            try
            {
                ViewBag.SubMenutype = "DashboardReports";
                ViewBag.ReportType = objGlobaldata.GetDashboardReportType();
                ViewBag.years = objGlobaldata.GetDropdownList("Years");
                if (ReportType != null && ReportType != "select")
                {
                    ViewBag.Report_Type = objGlobaldata.GetDashboardReportTypeByID(ReportType);
                    string type = objGlobaldata.GetDashboardReportTypeByID(ReportType);
                    if (type == "Objectives Dashboard")//Objective Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeObjectiveDisplay = true;
                    }
                    if (type == "KPI Dashboard")//kpi Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeKPIDisplay = true;
                    }
                    if (type == "Meeting Dashboard")// Meeting Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMeetingDisplay = true;
                    }
                    if (type == "Risk Dashboard")//risk Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeRISKDisplay = true;
                    }
                    if (type == "Audit Dashboard")//audit Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeAuditDisplay = true;
                    }
                    if (type == "Training Dashboard")//Training Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeTrainingDisplay = true;
                    }

                    if (type == "Calibration Dashboard")// Calibration Dashboard
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeCalibrationDisplay = true;
                    }
                }

                if (Years != null && Years != "Select")
                {
                    ViewBag.Year = objGlobaldata.GetDropdownitemById(Years);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DashboardReports: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

         
        [AllowAnonymous]
        public ActionResult MISReports(string ReportType, string Years)
        {
            ViewBag.SubMenutype = "MISReports";
            try
            {
                ViewBag.years = objGlobaldata.GetDropdownList("Years");
                ViewBag.ReportType = objGlobaldata.GetDropdownList("ReportType");

                if (ReportType != null && ReportType != "select")
                {
                    ViewBag.Report_Type = objGlobaldata.GetDropdownitemById(ReportType);
                    string type = objGlobaldata.GetDropdownitemById(ReportType);
                    if (type == "Objective Report")//Objective Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeObjectiveDisplay = true;
                    }
                    if (type == "KPI Report")//kpi Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISKPIDisplay = true;
                    }
                    if (type == "Risk Management")//risk Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISRISKDisplay = true;
                    }
                    if (type == "Audit Report")//audit Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISAuditDisplay = true;
                    }
                    if (type == "Training Report")//Training Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISTrainingDisplay = true;
                    }
                    if (type == "Supplier Performance")//Supplier performance Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISSupplierDisplay = true;
                    }
                    if (type == "Employee Performance Report")// performance Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISPerfEvalDisplay = true;
                    }
                    if (type == "Customer Complaint")// performance Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISComplaintDisplay = true;
                    }
                    if (type == "Calibration")// performance Report
                    {
                        ViewBag.Type = ReportType;
                        ViewBag.IframeMISCalibrationDisplay = true;
                    }
                }

                if (Years != null && Years != "Select")
                {
                    ViewBag.Year = objGlobaldata.GetDropdownitemById(Years);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MISReports: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

      
       
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ViewResult KPIReport(FormCollection form, string dtFromDate, string dtToDate)
        //{
        //    try
        //    {
        //        DateTime fromdateValue = new DateTime();
        //        DateTime toDateValue = new DateTime();

        //        if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
        //        {
        //        }
        //        if (DateTime.TryParse(form["ToDate"], out toDateValue) == true)
        //        {
        //        }

        //        ViewBag.fromdateValue = (fromdateValue).ToString("dd/MM/yyyy");
        //        ViewBag.toDateValue = (toDateValue).ToString("dd/MM/yyyy");
        //        ViewBag.IframekpiDisplay = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in KPIReport: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}

         
        //public FileContentResult GenerateAndDisplayKPIReport(string format)
        //{

        //    Microsoft.Reporting.WebForms.LocalReport lr =
        //    new Microsoft.Reporting.WebForms.LocalReport();

        //    //LocalReport localReport = new LocalReport();
        //    lr.ReportPath = Server.MapPath("~/RPT-RDLC/KPIReport.rdlc");

        //    KPIModelsList objKPI = new KPIModelsList();
        //    objKPI.KPIMList = new List<KPIModels>();


        //    try
        //    {
        //        string sSqlstmt = "select * from kpiview";
        //        DataSet dsKPIModelsList = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsKPIModelsList.Tables.Count > 0 && dsKPIModelsList.Tables[0].Rows.Count > 0)
        //        {

        //            for (int i = 0; i < dsKPIModelsList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    KPIModels objKPIModels = new KPIModels
        //                    {
        //                        KPI_Ref = dsKPIModelsList.Tables[0].Rows[i]["KPI_Ref"].ToString(),
        //                        Key_Perf_Indicator = dsKPIModelsList.Tables[0].Rows[i]["Key_Perf_Indicator"].ToString(),
        //                        Measurable_Value = dsKPIModelsList.Tables[0].Rows[i]["Measurable_Value"].ToString(),
        //                        KPI_Monitoring_Mechanism = dsKPIModelsList.Tables[0].Rows[i]["KPI_Monitoring_Mechanism"].ToString(),
        //                        DeptName = dsKPIModelsList.Tables[0].Rows[i]["DeptName"].ToString(),
        //                        Person_Responsible = dsKPIModelsList.Tables[0].Rows[i]["Person_Responsible"].ToString(),
        //                        Freq_of_Eval = dsKPIModelsList.Tables[0].Rows[i]["Freq_of_Eval"].ToString(),
        //                    };
        //                    DateTime dtEstDate = new DateTime();
        //                    if (DateTime.TryParse(dsKPIModelsList.Tables[0].Rows[0]["KPI_Estd_On"].ToString(), out dtEstDate))
        //                    {
        //                        objKPIModels.KPI_Estd_On = dtEstDate;
        //                    }
        //                    objKPI.KPIMList.Add(objKPIModels);
        //                }
        //                catch (Exception ex)
        //                {
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GenerateAndDisplayKPIReport: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

        //    }




        //    ReportDataSource reportDataSource = new ReportDataSource();
        //    reportDataSource.Name = "KPIDataset";

        //    reportDataSource.Value = objKPI.KPIMList;

        //    lr.DataSources.Add(reportDataSource);
        //    string reportType = "Image";
        //    string mimeType;
        //    string encoding;
        //    string fileNameExtension;
        //    //The DeviceInfo settings should be changed based on the reportType            
        //    //http://msdn2.microsoft.com/en-us/library/ms155397.aspx            
        //    string deviceInfo = "<DeviceInfo>" +
        //        "  <OutputFormat>JPEG</OutputFormat>" +
        //        "  <PageWidth>8.5in</PageWidth>" +
        //        "  <PageHeight>11in</PageHeight>" +
        //        "  <MarginTop>0.5in</MarginTop>" +
        //        "  <MarginLeft>1in</MarginLeft>" +
        //        "  <MarginRight>1in</MarginRight>" +
        //        "  <MarginBottom>0.5in</MarginBottom>" +
        //        "</DeviceInfo>";
        //    Warning[] warnings;
        //    string[] streams;
        //    byte[] renderedBytes;
        //    //Render the report            
        //    renderedBytes = lr.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
        //    //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension); 
        //    if (format == null)
        //    {
        //        return File(renderedBytes, "image/jpeg");
        //    }
        //    else if (format == "PDF")
        //    {
        //        return File(renderedBytes, "pdf");
        //    }
        //    else
        //    {
        //        return File(renderedBytes, "image/jpeg");
        //    }
        //}
        ////
        //GET: /Reports/InternalAuditNCReport
         
        [AllowAnonymous]
        public ActionResult InternalAuditNCReport()
        {
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();//GetDepartmentList();
            
            return View();
 
        }

        //
        // GET: /Reports/InternalAuditNCReport
         
        [AllowAnonymous]
        public ActionResult report()
        {
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
            ViewBag.AuditType = objGlobaldata.GetAuditTypeList();
            return View();

        }

        // POST: /Reports/InternalAuditNCReport
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditNCReport(FormCollection form)
        {
            
            string sAuditnum = form["AuditNum"];
            string sDeptId = form["DeptId"];
            DataSet dsNCReport = objGlobaldata.GetAuditNCReportdetails(sAuditnum, sDeptId);
            dsNCReport.Tables[0].TableName = "AuditNCReport";

            objGlobaldata.AddFunctionalLog("Step1", "ReportLog.txt");  
         
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
            return View();
        }

        //
        // GET: /Reports/AuditLogReport
         
        [AllowAnonymous]
        public ActionResult AuditLogReport()
        {
            ViewBag.SubMenutype = "AuditLogReport";
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();//GetDepartmentList();
            ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
            return View();
        }

        [AllowAnonymous]
        public ActionResult ReportList(FormCollection form, int? page)
        {
            ViewBag.SubMenutype = "ReportList";
            ReportsModelsList objList = new ReportsModelsList();
            objList.RptList = new List<ReportsModels>();
            try
            {
                string sSqlstmt = "select id_reports,category,report_name,description,controller,action_name from t_reports where active=1";
                DataSet dsReportList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsReportList.Tables.Count > 0 && dsReportList.Tables[0].Rows.Count > 0)
                {
                  
                    for (int i = 0; i < dsReportList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ReportsModels objModel = new ReportsModels
                            {

                                id_reports = dsReportList.Tables[0].Rows[i]["id_reports"].ToString(),
                                category = dsReportList.Tables[0].Rows[i]["category"].ToString(),
                                report_name = dsReportList.Tables[0].Rows[i]["report_name"].ToString(),
                                description = dsReportList.Tables[0].Rows[i]["description"].ToString(),
                                controller = dsReportList.Tables[0].Rows[i]["controller"].ToString(),
                                action_name = dsReportList.Tables[0].Rows[i]["action_name"].ToString(),
                            };

                            objList.RptList.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ReportList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ReportList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objList.RptList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ReportDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_reports"] != null && form["id_reports"] != "")
                {
                    ReportsModels Doc = new ReportsModels();
                    string id_reports = form["id_reports"];
                    if (Doc.FunDeleteReportDoc(id_reports))
                    {
                        TempData["Successdata"] = "Report deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in ReportDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
               

        // POST: /Reports/AuditLogReport
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditLogReport(FormCollection form, string command)
        {
            CompanyModels objCompany = new CompanyModels();
            ViewBag.SubMenutype = "AuditLogReport";
            string sAuditnum = form["AuditNum"];
            string sDeptId = form["DeptId"];

            ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
            string AuditNo = objGlobaldata.GetAuditNumById(sAuditnum);
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();

            ViewBag.AuditNum = sAuditnum;
            ViewBag.DeptId = sDeptId;


            if (command != null && command.Equals("Generate"))
            {
                return View(AuditLogPrintPdf(AuditNo));
            }
            else
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

                DataSet dsAuditLogReport = objGlobaldata.GetAuditLogReportdetails(AuditNo);

                if (dsAuditLogReport.Tables.Count > 0 && dsAuditLogReport.Tables[0].Rows.Count > 0)
                {
                 
                    dsAuditLogReport = objCompany.GetCompanyDetailsForReport(dsAuditLogReport);
                }
                ViewBag.dsAuditLogReport = dsAuditLogReport;// MasterListDocReport(); 
              
                dsAuditLogReport = objGlobaldata.GetReportDetails(dsAuditLogReport, AuditNo, dsAuditLogReport.Tables[0].Rows[0]["preparer"].ToString(), "INTERNAL AUDIT LOG‎ REPORT");

                ViewBag.CompanyInfo = dsAuditLogReport;

                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                return new ViewAsPdf("AuditLogPrintPdf", new { AuditNum = AuditNo, DeptId = sDeptId })
                {
                    //FileName = "AuditLog.pdf",
                    Cookies = cookieCollection,
                    CustomSwitches = footer
                };
            }

        }


        //
        // GET: /Reports/AuditLogPrintPdf
         
        [AllowAnonymous]
        public ActionResult AuditLogPrintPdf(string AuditNum)
        {
            ViewBag.SubMenutype = "AuditLogReport";
            DataSet dsAuditLogReport = objGlobaldata.GetAuditLogReportdetails(AuditNum);

            if (dsAuditLogReport.Tables.Count > 0 && dsAuditLogReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsAuditLogReport = objCompany.GetCompanyDetailsForReport(dsAuditLogReport);
            }
            ViewBag.dsAuditLogReport = dsAuditLogReport;// MasterListDocReport();           

            return View();
        }

        //
        // GET: /Reports/AuditSummaryReport
         
        [AllowAnonymous]
        public ActionResult AuditSummaryReport()
        {
            ViewBag.SubMenutype = "AuditSummaryReport";
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();//GetDepartmentList();
            ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
            return View();

        }

        // POST: /Reports/AuditSummaryReport
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditSummaryReport(FormCollection form, string command)
        {
            ViewBag.SubMenutype = "AuditSummaryReport";
            string sAuditnum = form["AuditNum"];
            //string sDeptId = form["DeptId"];
            //string sDeptId = form["DeptId"];

            ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
            string AuditNo = objGlobaldata.GetAuditNumById(sAuditnum);
            ViewBag.AuditNum = sAuditnum;


            if (command != null && command.Equals("Generate"))
            {
                return View(AuditSummaryPrintPdf(AuditNo));
            }
            else
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

                DataSet dsAusditReport = objGlobaldata.GetAuditSummaryReportdetails(AuditNo);

                if (dsAusditReport.Tables.Count > 0 && dsAusditReport.Tables[0].Rows.Count > 0)
                {
                    CompanyModels objCompany = new CompanyModels();
                    dsAusditReport = objCompany.GetCompanyDetailsForReport(dsAusditReport);
                }

                ViewBag.dsAusditReport = dsAusditReport;// MasterListDocReport();
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.AuditNum = sAuditnum;


                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                return new ViewAsPdf("AuditSummaryPrintPdf", new { AuditNum = AuditNo })
                {
                    //FileName = "AuditSummary.pdf",
                    Cookies = cookieCollection,
                    CustomSwitches = footer
                };
            }
            return View();
        }


        //
        // GET: /Reports/MasterListDocReport
         
        [AllowAnonymous]
        public ActionResult AuditSummaryPrintPdf(string AuditNum)
        {
            ViewBag.SubMenutype = "AuditSummaryReport";
            DataSet dsAusditReport = objGlobaldata.GetAuditSummaryReportdetails(AuditNum);

            if (dsAusditReport.Tables.Count > 0 && dsAusditReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsAusditReport = objCompany.GetCompanyDetailsForReport(dsAusditReport);
            }

            ViewBag.dsAusditReport = dsAusditReport;// MasterListDocReport();
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
            //ViewBag.AuditNum = AuditNum;

            return View();
        }


        //
        // GET: /Reports/MgmtMasterListDocumentReport
         
        [AllowAnonymous]
        public ActionResult MgmtMasterListDocumentReport()
        {
            ViewBag.SubMenutype = "MgmtMasterListDocumentReport";
            return View();
        }        

        //
        // GET: /Reports/MgmtMasterListDocumentReport
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MgmtMasterListDocumentReport(FormCollection divPrint, FormCollection form)
        {
            
            ViewBag.SubMenutype = "MgmtMasterListDocumentReport";
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();
            DataSet dsDocReport = objGlobaldata.GetMgmtMasterDocListReportdetails();
            int i = dsDocReport.Tables[0].Rows.Count;
            if (dsDocReport.Tables.Count > 0 && dsDocReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsDocReport = objCompany.GetCompanyDetailsForReport(dsDocReport);
            }

            dsDocReport.Tables[0].TableName = "MasterListDoc";
            ViewBag.dsMasterDocList = dsDocReport;// MasterListDocReport();

            ViewBag.lstDocLevels = dsDocReport.Tables[0].AsEnumerable().Select(x => x["doclevels"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("MasterDocumentReport", "Reports")
            {
                //FileName = "MasterListDocument.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };           
        }

         
        [AllowAnonymous]
        public ActionResult MasterDocumentReport(FormCollection form)
        {
           

            ViewBag.SubMenutype = "MgmtMasterListDocumentReport";
            DataSet dsDocReport = objGlobaldata.GetMgmtMasterDocListReportdetails();
            int i = dsDocReport.Tables[0].Rows.Count;
            ViewBag.count = i;
            if (dsDocReport.Tables.Count > 0 && dsDocReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsDocReport = objCompany.GetCompanyDetailsForReport(dsDocReport);
            }

            dsDocReport.Tables[0].TableName = "MasterListDoc";
            ViewBag.dsMasterDocList = dsDocReport;// MasterListDocReport();
            ViewBag.DocType = objGlobaldata.GetDocTypeListbyDocLevels();
            ViewBag.lstDocLevels = dsDocReport.Tables[0].AsEnumerable().Select(x => x["Doctype"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            return View();
        }

        //
        // GET: /Reports/MasterListDocReport
         
        [AllowAnonymous]
        public ActionResult MasterDocumentPrintPdf(FormCollection form)
        {
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            
            if (Request.Form["button2"] != null)
            {
                ViewBag.SubMenutype = "MgmtMasterListDocumentReport";
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
           

                DataSet dsDocReport = objGlobaldata.GetMgmtMasterDocListReportdetails();
                int i = dsDocReport.Tables[0].Rows.Count;
                ViewBag.count = i;
                if (dsDocReport.Tables.Count > 0 && dsDocReport.Tables[0].Rows.Count > 0)
                {
                    CompanyModels objCompany = new CompanyModels();
                    dsDocReport = objCompany.GetCompanyDetailsForReport(dsDocReport);
                }

                dsDocReport.Tables[0].TableName = "MasterListDoc";
                ViewBag.dsMasterDocList = dsDocReport;// MasterListDocReport();
                ViewBag.DocType = objGlobaldata.GetDropdownList("DocType");
                ViewBag.lstDocLevels = dsDocReport.Tables[0].AsEnumerable().Select(x => x["Doctype"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                return new ViewAsPdf("MasterDocumentPrintPdf", "Reports")
                {
                    //FileName = "MasterListDocument.pdf",
                    Cookies = cookieCollection,
                    CustomSwitches = footer
                };
            }
                return View();
        }
       
        //
        // GET: /Reports/ObjectivesReport
         
        [AllowAnonymous]
        public ActionResult ObjectivesReport()
        {
            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
            ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");
            ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
            ViewBag.SubMenutype = "ObjectivesReport";
            return View();

        }
       
        //
        // GET: /Reports/ObjectivesPrintPdf
        [AllowAnonymous]
        public ActionResult ObjectivesPrintPdf(string dtFromDate, string dtToDate)
        {
            ViewBag.SubMenutype = "ObjectivesReport";
            DataSet dsObjectivesReport = objGlobaldata.GetObjectivesReportdetails(dtFromDate, dtToDate);

            if (dsObjectivesReport.Tables.Count > 0 && dsObjectivesReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsObjectivesReport = objCompany.GetCompanyDetailsForReport(dsObjectivesReport);
            }

            ViewBag.fromdateValue = Convert.ToDateTime(dtFromDate).ToString("yyyy-MM-dd");
            ViewBag.toDateValue = Convert.ToDateTime(dtToDate).ToString("yyyy-MM-dd");
            ViewBag.dsObjectivesList = dsObjectivesReport;// MasterListDocReport();

            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
            ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");
            ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

            //ViewBag.lstDocLevels = dsObjectivesReport.Tables[0].AsEnumerable().Select(x => x["doclevels"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            return View();
        }

      
        //
        // GET: /Reports/MgmtMasterListDocumentReport
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ObjectivesReport(FormCollection form, string command, string Approvestatus, string Dept, string Freq_of_Eval)
        {
            ViewBag.SubMenutype = "ObjectivesReport";
            DateTime fromdateValue = new DateTime();
            DateTime toDateValue = new DateTime();

            if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
            {
            }
            if (DateTime.TryParse(form["ToDate"], out toDateValue) == true)
            {
            }
            if (Approvestatus != null && Approvestatus != "All")
            {
                ViewBag.ApprovestatusVal = Approvestatus;
            }
            if (Dept != null && Dept != "Select")
            {
                ViewBag.DeptListVal = Dept;
            }

            if (Freq_of_Eval != null && Freq_of_Eval != "Select")
            {
                ViewBag.Freq_of_EvalVal = Freq_of_Eval;
            }

            //if (Request["button"].Equals("Generate") == false)
            //if (Request.Form["Generate"] != null)
            if (command != null && command.Equals("Generate"))
            {
                return View(ObjectivesPrintPdf(fromdateValue.ToString("yyyy-MM-dd"), toDateValue.ToString("yyyy-MM-dd")));
                
            }
            else
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();


                DataSet dsObjectivesReport = objGlobaldata.GetObjectivesReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));

                if (dsObjectivesReport.Tables.Count > 0 && dsObjectivesReport.Tables[0].Rows.Count > 0)
                {
                    CompanyModels objCompany = new CompanyModels();
                    dsObjectivesReport = objCompany.GetCompanyDetailsForReport(dsObjectivesReport);
                }

                ViewBag.fromdateValue = (fromdateValue).ToString("dd/MM/yyyy");
                ViewBag.toDateValue = (toDateValue).ToString("dd/MM/yyyy");
                ViewBag.dsObjectivesList = dsObjectivesReport;// MasterListDocReport();

                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.Freq_of_Eval = objGlobaldata.GetConstantValue("Freq_of_Eval");
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");


                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                return new ViewAsPdf("ObjectivesPrintPdf", new { dtFromDate = fromdateValue.ToString("yyyy/MM/dd"), dtToDate = toDateValue.ToString("yyyy/MM/dd") })
                {
                    //FileName = "ObjectivesList.pdf",
                    Cookies = cookieCollection,
                    CustomSwitches = footer,
                    PageSize = Rotativa.Options.Size.A3
                    
                };
            }
        }

        [AllowAnonymous]
        public ActionResult KPIReport()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult KPIPrintPdf(string dtFromDate, string dtToDate)
        {
            ViewBag.SubMenutype = "";
            DataSet dsKPIReport = objGlobaldata.GetKPIReportdetails(dtFromDate, dtToDate);

            if (dsKPIReport.Tables.Count > 0 && dsKPIReport.Tables[0].Rows.Count > 0)
            {
                CompanyModels objCompany = new CompanyModels();
                dsKPIReport = objCompany.GetCompanyDetailsForReport(dsKPIReport);
            }

            ViewBag.fromdateValue = Convert.ToDateTime(dtFromDate).ToString("yyyy-MM-dd");
            ViewBag.toDateValue = Convert.ToDateTime(dtToDate).ToString("yyyy-MM-dd");
            ViewBag.dsKPIList = dsKPIReport;// MasterListDocReport();

           
            //ViewBag.lstDocLevels = dsObjectivesReport.Tables[0].AsEnumerable().Select(x => x["doclevels"].ToString()).Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KPIReport(FormCollection form, string command, string Approvestatus, string Dept, string Freq_of_Eval)
        {
            ViewBag.SubMenutype = "KPIReport";
            DateTime fromdateValue = new DateTime();
            DateTime toDateValue = new DateTime();

            if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
            {
            }
            if (DateTime.TryParse(form["ToDate"], out toDateValue) == true)
            {
            }
           
            if (command != null && command.Equals("Generate"))
            {
                return View(KPIPrintPdf(fromdateValue.ToString("yyyy-MM-dd"), toDateValue.ToString("yyyy-MM-dd")));
            }
            else
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();


                DataSet dsKPIReport = objGlobaldata.GetKPIReportdetails(fromdateValue.ToString("yyyy/MM/dd"), toDateValue.ToString("yyyy/MM/dd"));

                if (dsKPIReport.Tables.Count > 0 && dsKPIReport.Tables[0].Rows.Count > 0)
                {
                    CompanyModels objCompany = new CompanyModels();
                    dsKPIReport = objCompany.GetCompanyDetailsForReport(dsKPIReport);
                }

                ViewBag.fromdateValue = (fromdateValue).ToString("dd/MM/yyyy");
                ViewBag.toDateValue = (toDateValue).ToString("dd/MM/yyyy");
                ViewBag.dsKPIList = dsKPIReport;// MasterListDocReport();

                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

                return new ViewAsPdf("KPIPrintPdf", new { dtFromDate = fromdateValue.ToString("yyyy/MM/dd"), dtToDate = toDateValue.ToString("yyyy/MM/dd") })
                {
                    //FileName = "KPIList.pdf",
                    Cookies = cookieCollection,
                    CustomSwitches = footer,
                    PageSize = Rotativa.Options.Size.A3

                };
            }
        }

       

        ////
        //// GET: /Reports/ObjectivesReport
        //[AllowAnonymous]
        //public ActionResult EmployeeTimeSheetReport()
        //{
        //    ViewBag.SubMenutype = "EmployeeReport";
        //    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox(' ');

        //    return View();

        //}

        ////
        //// GET: /Reports/MgmtMasterListDocumentReport
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EmployeeTimeSheetReport(FormCollection form, string command)
        //{
        //    ViewBag.SubMenutype = "EmployeeReport";
        //    DateTime fromdateValue;
        //    string fromDate = "", toDate = "";
        //    string sEmpid = form["Empid"];

        //    if (DateTime.TryParse(form["FromDate"], out fromdateValue) == true)
        //    {
        //        fromDate = fromdateValue.Year + "/" + fromdateValue.Month + "/01";
        //        toDate = fromdateValue.Year + "/" + fromdateValue.Month + "/31";
        //    }          

        //    if (command != null && command.Equals("Generate"))
        //    {
        //        return View(EmployeeTimeSheetPrintPdf(fromDate, toDate, sEmpid));
        //    }
        //    else
        //    {
        //        Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

        //        foreach (var key in Request.Cookies.AllKeys)
        //        {
        //            cookieCollection.Add(key, Request.Cookies.Get(key).Value);
        //        }
        //        return new Rotativa.ActionAsPdf("EmployeeTimeSheetPrintPdf", new { FromDate = fromDate, toDate = toDate, EmpId = sEmpid })
        //        {
        //            FileName = "EmployeeTimeSheet.pdf",
        //            Cookies = cookieCollection
        //        };
        //    }
        //}

        ////
        //// GET: /Reports/ObjectivesPrintPdf
        //[AllowAnonymous]
        //public ActionResult EmployeeTimeSheetPrintPdf(string FromDate, string toDate, string EmpId)
        //{
        //    ViewBag.SubMenutype = "EmployeeReport";

        //    string sSqlstmt = "select TimeSheet_Id, CompEmpId, temp.FirstName, temp.LastName,Designation,  CompanyName , MonthOf, FirstHalf_InTime, FirstHalf_OutTime,"
        //        + " SecondHalf_InTime, SecondHalf_OutTime, OverTime, Remarks from t_emp_timesheet as tsheet, t_employee as temp, t_cust_supp_info as tcust"
        //        + " where monthof between '" + FromDate + "' and '" + toDate + "' and tsheet.empid='" + EmpId 
        //        + "' and tsheet.EmpID=temp.EmpID and tsheet.CustID=tcust.CustID;";

        //    DataSet dsTimeSheetReport = objGlobaldata.Getdetails(sSqlstmt);

        //    EmpTimeSheetReportFieldModels objTimeSheet = new EmpTimeSheetReportFieldModels();
        //    ViewBag.objTimeSheet = objTimeSheet;

        //    ViewBag.dsTimeSheetReportList = dsTimeSheetReport;// MasterListDocReport();
        //    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox(' ');
        //    ViewBag.EmpId = EmpId;

        //    return View();
        //}

        // GET: /Reports/DisplayDocument
         
        public FileResult DisplayDocument(string Document)
        {
            if (objGlobaldata.GetCurrentUserSession() != null)
            {
                try
                {
                    if (Document != null && Document != "") //(dsMaintenanceList.Tables[0].Rows.Count > 0)
                    {
                        if (System.IO.File.Exists(Server.MapPath(Document)))
                        {
                            FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);

                            return File(fsSource, MimeMapping.GetMimeMapping(Path.GetFileName(Document)));
                        }
                        else
                        {
                            return File(Server.MapPath("~/Views/Error/FileNotFound.html"), "text/html");
                        }
                      
                    }
                    return File(Server.MapPath("~/Views/Error/FileNotFound.html"), "text/html");
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in Reports DisplayDocument: " + ex.ToString());
                    return File(Server.MapPath("~/Views/Error/FileNotFound.html"), "text/html");
                }
            }
            return File(Server.MapPath("~/Views/Error/AccessDenied.html"), "text/html");
        }

         
        public ActionResult SupplierReport()
        {
            ViewBag.Menutype = "Suppliers";
            ViewBag.SubMenutype = "SupplierReport";
            try
            {
                DataSet dsSupplier = objGlobaldata.GetSupplierReport();
                ViewBag.SupplierList = dsSupplier;
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MasterDocuent: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MRMReport()
        {
            try
            {
                ViewBag.years = objGlobaldata.GetDropdownList("Years");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MRMReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public FileResult MRMExport(FormCollection form, string Years)
        {
            IXLWorkbook wb = new XLWorkbook();
            var stream = new MemoryStream();
            try
            {
                ViewBag.years = objGlobaldata.GetDropdownList("Years");
                if (Years != null && Years != "Select")
                {
                    ViewBag.Year = Years;
                }
                DataSet dsMRMData = objGlobaldata.GetMRMData(objGlobaldata.GetDropdownitemById(Years));

                string sql1 = "select CompanyName,Logo from t_companyinfo";
                DataSet dsData1 = objGlobaldata.Getdetails(sql1);

                for (int k=0;k < dsMRMData.Tables.Count;k++)
                {
                   
                    if (dsMRMData.Tables[k].Rows.Count > 0)
                    {
                        IXLWorksheet ws = wb.Worksheets.Add("Agenda" + (k + 1));
                        var range = ws.Range(5, 1, dsMRMData.Tables[k].Rows.Count+5, dsMRMData.Tables[k].Columns.Count-1);
                        var table = range.CreateTable();
                        string img = Server.MapPath(dsData1.Tables[0].Rows[0][1].ToString());
                        ws.AddPicture(Url.Content(img)).MoveTo(ws.Cell(1, 1)).Scale(.5); // optional: resize picture
                        ws.Row(1).Height = 50;
                        ws.Column(1).Width = 20;

                        ws.Cell(1, 2).Value = dsData1.Tables[0].Rows[0][0].ToString();

                        ws.Cell(1, 2).Style.Font.FontSize = 18;
                        ws.Cell(1, 2).Style.Font.SetBold();
                        ws.Cell(1, 2).Style.Font.FontColor = XLColor.Blue;

                        ws.Cell(3, 1).Value = dsMRMData.Tables[k].Rows[0][0].ToString();
                      
                        ws.Cell(3, 1).Style.Font.FontSize = 15;
                        ws.Cell(3, 1).Style.Font.SetBold();
                        ws.Cell(3, 1).Style.Font.FontColor = XLColor.Blue;

                        for (int j = 1; j < dsMRMData.Tables[k].Columns.Count; j++)
                        {
                            table.Cell(1, j).Value = dsMRMData.Tables[k].Columns[j].ColumnName;
                        }
                        for (int i = 0; i < dsMRMData.Tables[k].Rows.Count; i++)
                        {
                            for (int j = 1; j < dsMRMData.Tables[k].Columns.Count; j++)
                            {
                                table.Cell(i + 2, j).Value = dsMRMData.Tables[k].Rows[i][j].ToString();
                            }

                        }
                        // apply style
                        table.Theme = XLTableTheme.TableStyleLight12;
                    }
                }
               

                wb.SaveAs(stream);
                stream.Flush();

        }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MRMReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return new FileContentResult(stream.ToArray(),
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "MRMExport.xlsx"
            };
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult HSEMonthlyReport()
        {
            try
            {
                ViewBag.years = objGlobaldata.GetDropdownList("Years");
                ViewBag.months = objGlobaldata.GetConstantValue("Months");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEMonthlyReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public FileResult HSEReportExport(FormCollection form, string Years, string months)
        {
            IXLWorkbook wb = new XLWorkbook();
            var stream = new MemoryStream();
           
            try
            {
                ViewBag.years = objGlobaldata.GetDropdownList("Years");
                ViewBag.months = objGlobaldata.GetConstantValue("Months");
                if (Years != null && Years != "Select")
                {
                    ViewBag.Year = Years;
                }
                if (months != null && months != "Select")
                {
                    ViewBag.month = months;
                }
                string sql1 = "select CompanyName,Logo from t_companyinfo";
                DataSet dsData = objGlobaldata.Getdetails(sql1);

                IXLWorksheet ws = wb.Worksheets.Add();
                var table = ws.Range("E2:K31");
                //var table = range.CreateTable();
                //table.Theme = XLTableTheme.None;
                //table.ShowHeaderRow = false;
                table.Style.Border.InsideBorder= XLBorderStyleValues.Thin;
                table.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                string img = Server.MapPath(dsData.Tables[0].Rows[0][1].ToString());
                ws.AddPicture(Url.Content(img)).MoveTo(table.Cell(1, 1),5,5).WithSize(150, 100); // optional: resize picture
                table.Cell(1, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            

                ws.Column("E").Width = 25;
                ws.Column("F").Width = 20;
                ws.Column("G").Width = 20;
                ws.Column("H").Width = 20;
                ws.Column("I").Width = 20;
                ws.Row(2).Height = 80;
                table.Range("F2:K2").Merge();
                table.Cell(1, 2).Value = dsData.Tables[0].Rows[0][0].ToString();
                table.Cell(1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                table.Cell(1, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                table.Cell(1, 2).Style.Font.FontSize = 15;
                table.Cell(1, 2).Style.Font.SetBold();
                table.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
               

                table.Range("E3:K3").Merge();
                table.Cell(2, 1).Value = "HSE MONTHLY PERFORMANCE REPORT";
                table.Cell(2, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                table.Cell(2, 1).Style.Font.FontSize = 15;
                table.Cell(2, 1).Style.Font.SetBold();
                table.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;

                table.Cell(3, 1).Value = "Period";
                table.Cell(4, 1).Value = "Division / Department";
                table.Cell(5, 1).Value = "Region / Location";
                table.Cell(6, 1).Value = "Work Description / Activity / Project Description";

                table.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(5, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(6, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(3, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(3, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;

                table.Cell(6, 1).Style.Alignment.WrapText = true;
                //ws.Columns().AdjustToContents();

                table.Cell(3, 2).Value = "Month";
                table.Cell(3, 4).Value = "Year";
                table.Range("I4:K4").Merge();
                table.Range("F5:K5").Merge();
                table.Range("F6:K6").Merge();
                table.Range("F7:K7").Merge();


                table.Range("E8:K8").Merge();
                table.Range("E8:K8").Style.Fill.BackgroundColor = XLColor.PeachOrange;

                table.Range("E9:K9").Merge();
                table.Range("E9:K9").Style.Fill.BackgroundColor = XLColor.YellowMunsell;
                table.Cell(8, 1).Value = " Sub-contractors if any working at the location / site under the control of your department /division";

                table.Range("E10:F10").Merge();
                table.Range("E10:F10").Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(9, 1).Value = "Sub contractor Name";
                table.Cell(9, 3).Value = "No. of Person";
                table.Cell(9, 4).Value = "No. of Incidents related to the Sub-contractors";
                table.Cell(9, 5).Value = "REMARKS";
                table.Range("I10:K10").Merge();
                table.Range("I11:K11").Merge();
                table.Range("I12:K12").Merge();
                table.Range("I13:K13").Merge();

                table.Cell(9, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                table.Cell(9, 3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                table.Cell(9, 4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                table.Cell(9, 5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                table.Cell(9, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(9, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(9, 5).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(9, 4).Style.Alignment.WrapText = true;

                table.Range("E11:F11").Merge();
                table.Range("E12:F12").Merge();
                table.Range("E13:F13").Merge();

                table.Range("E14:K14").Merge();
                table.Range("E14:K14").Style.Fill.BackgroundColor = XLColor.YellowMunsell;
                table.Cell(13, 1).Value = "MONTHLY HSE PERFORMANCE";
                table.Cell(13, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                table.Cell(14, 1).Value = "LAGGING INDICATORS";
                table.Cell(14, 2).Value = "Monthly Total";
                table.Cell(14, 3).Value = "Yearly Total";
                table.Cell(14, 4).Value = "Target / Specification";
                table.Cell(14, 5).Value = "LEADING INDICATORS";
                table.Cell(14, 6).Value = "Monthly Total";
                table.Cell(14, 7).Value = "Year to Date";

                table.Cell(14, 6).Style.Alignment.WrapText = true;
                table.Cell(14, 7).Style.Alignment.WrapText = true;


                table.Cell(14, 1).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 2).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 3).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 4).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 5).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 6).Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(14, 7).Style.Fill.BackgroundColor = XLColor.SkyBlue;

                DataSet dsHSEData = objGlobaldata.GetHSEMonthlyData(objGlobaldata.GetDropdownitemById(Years),months);

                if (dsHSEData.Tables.Count > 0 && dsHSEData.Tables[0].Rows.Count >0)
                {
                    for (int i = 0,k=15; i < dsHSEData.Tables[0].Rows.Count && dsHSEData.Tables[0].Rows[i]["ind_type"].ToString() == "LAGGING INDICATORS"; i++,k++)
                    {
                        table.Cell(k, 1).Value = dsHSEData.Tables[0].Rows[i]["ind_desc"].ToString();
                        table.Cell(k, 2).Value = dsHSEData.Tables[0].Rows[i]["mth_total"].ToString();
                        table.Cell(k, 3).Value = dsHSEData.Tables[0].Rows[i]["ytd"].ToString();
                    }
                }
                if (dsHSEData.Tables.Count > 0 && dsHSEData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0, k = 15; i < dsHSEData.Tables[0].Rows.Count ; i++)
                    {
                        if (dsHSEData.Tables[0].Rows[i]["ind_type"].ToString() == "LEADING INDICATORS")
                        {
                            table.Cell(k, 5).Value = dsHSEData.Tables[0].Rows[i]["ind_desc"].ToString();
                            table.Cell(k, 6).Value = dsHSEData.Tables[0].Rows[i]["mth_total"].ToString();
                            table.Cell(k, 7).Value = dsHSEData.Tables[0].Rows[i]["ytd"].ToString();
                            k++;
                        }
                    }
                }

                table.Range("E25:K25").Merge();
                table.Range("E25:K25").Style.Fill.BackgroundColor = XLColor.YellowMunsell;
                table.Cell(24, 1).Value = "EXTERNAL INSPECTIONS / AUDITS ";
                table.Cell(24, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);


                table.Range("E26:H26").Merge();
                table.Range("E26:H26").Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(25, 1).Value = "No. of external HSE inspections by legal authorities/customers/Principals";

                table.Range("E27:H27").Merge();
                table.Range("E27:H27").Style.Fill.BackgroundColor = XLColor.SkyBlue;
                table.Cell(26, 1).Value = "Was there any violation / fine by legal / regulatory authorities? If yes, provide  details";

                table.Range("I26:K26").Merge();
                table.Range("I27:K27").Merge();

                table.Range("E28:E29").Merge();
                table.Cell(27, 1).Value = "COMMENTS";
                table.Range("F28:K29").Merge();

                table.Cell(29, 1).Value = "PREPARED BY";
                table.Range("F30:K30").Merge();

                table.Cell(30, 1).Value = "REVIEWED BY";
                table.Range("F31:K31").Merge();

                wb.SaveAs(stream);
                stream.Flush();
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEReportExport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return new FileContentResult(stream.ToArray(),
                           "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "HSEReportExport.xlsx"
            };
        }
    }
}
