using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;


namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class EmergencyPlanNRecordController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public EmergencyPlanNRecordController()
        {

            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "EmergencyPlanNRecord";
        }

        //
        // GET: /EmergencyPlanNRecord/
        
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /EmergencyPlanNRecord/AddEmergencyPlanNRecord
        
        [AllowAnonymous]
        public ActionResult AddEmergencyPlanNRecord()
        {
            EmergencyPlanNRecordModels objEmergency = new EmergencyPlanNRecordModels();

            try
            {
                objEmergency.branch = objGlobaldata.GetCurrentUserSession().division;
                objEmergency.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objEmergency.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objEmergency.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objEmergency.branch);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
                ViewBag.HSEPerformance = objEmergency.GetMultiHSEPerformanceList();
                ViewBag.DrillPerformance = objEmergency.GetMultiDrillPerformanceList();
                ViewBag.EmergencyType = objEmergency.GetMultiEmergencyTypeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.DrillLocation = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmergencyPlanNRecord: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objEmergency);
        }

        //
        // POST: /EmergencyPlanNRecord/AddEmergencyPlanNRecord
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddEmergencyPlanNRecord(EmergencyPlanNRecordModels objEmergencyPlanNRecord, FormCollection form, HttpPostedFileBase Upload_Report)
        {
            try
            {
                objEmergencyPlanNRecord.branch = form["branch"];
                objEmergencyPlanNRecord.Location = form["Location"];
                objEmergencyPlanNRecord.Department = form["Department"];

                DateTime dateValue;
                if (Upload_Report != null && Upload_Report.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(Upload_Report.FileName));
                        string sFilename = "Emergency" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload_Report.SaveAs(sFilepath + "/" + sFilename);
                        objEmergencyPlanNRecord.Upload_Report = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmergencyPlanNRecord: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["Plan_From"] != null && DateTime.TryParse(form["Plan_From"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_From = dateValue;
                }

                if (form["Plan_To"] != null && DateTime.TryParse(form["Plan_To"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_To = dateValue;
                }

                if (form["Plan_Date_Time"] != null && DateTime.TryParse(form["Plan_Date_Time"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_Date_Time = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objEmergencyPlanNRecord.Plan_Date_Time = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    } 
                }

                EmergencyPlanNRecordModelsList objPlanList = new EmergencyPlanNRecordModelsList();
                objPlanList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();
                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["Corrrective_action " + i] != null)
                        {
                            EmergencyPlanNRecordModels objPlan = new EmergencyPlanNRecordModels();
                            objPlan.Corrrective_action = form["Corrrective_action " + i];
                            if (DateTime.TryParse(form["Target_date " + i], out dateValue) == true)
                            {
                                objPlan.Target_date = dateValue;
                            }
                            objPlanList.lstEmergencyPlanNRecord.Add(objPlan);
                        }
                    }
                }


                if (objEmergencyPlanNRecord.FunAddEmergencyPlan(objEmergencyPlanNRecord, objPlanList))
                {
                    TempData["Successdata"] = "Added Emerygency Plan details successfully  with Reference Number '" + objEmergencyPlanNRecord.ReportNo + "'"; 
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddEmergencyPlanNRecord: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmergencyPlanNRecordList");
        }

        
        [AllowAnonymous]
        public JsonResult EmergencyReportDocDelete(FormCollection form)
        {
            try
            {
               
                       if (form["Emergency_Plan_Id"] != null && form["Emergency_Plan_Id"] != "")
                        {

                            EmergencyPlanNRecordModels Doc = new EmergencyPlanNRecordModels();
                            string sEmergency_Plan_Id = form["Emergency_Plan_Id"];


                            if (Doc.FunDeleteEmergencyPlanDoc(sEmergency_Plan_Id))
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
                            TempData["alertdata"] = "Emergency plan Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyReportDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        [AllowAnonymous]
        public ActionResult EmergencyPlanNRecordList(string SearchText, int? page, string branch_name)
        {
            EmergencyPlanNRecordModelsList objEmergencyPlanNRecordList = new EmergencyPlanNRecordModelsList();
            objEmergencyPlanNRecordList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();
            EmergencyPlanNRecordModels objem = new EmergencyPlanNRecordModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                string sSqlstmt = "select Emergency_Plan_Id, emp_id, Plan_From, Plan_To, Emergency_Type, Plan_Date_Time, Drill_Location, Incharge, Need_Reporting, "
                    +" Plan_Status, Performance, Remarks, ReportNo, Upload_Report, LoggedBy, ReviewedBy, ApprovedBy,branch,Department,Location from t_emergency_plan_record where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Plan_From desc";
                DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCompetenceList.Tables.Count > 0)
                {   
                    for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmergencyPlanNRecordModels objEmployeeCompetence = new EmergencyPlanNRecordModels
                            {
                                Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[i]["Emergency_Plan_Id"].ToString(),
                                emp_id = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["emp_id"].ToString()),
                                Emergency_Type =objem.GetEmergencyTypeNameById(dsCompetenceList.Tables[0].Rows[i]["Emergency_Type"].ToString()),
                                Drill_Location = objGlobaldata.GetCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[i]["Drill_Location"].ToString()),
                                Incharge = dsCompetenceList.Tables[0].Rows[i]["Incharge"].ToString(),
                                Need_Reporting = (dsCompetenceList.Tables[0].Rows[i]["Need_Reporting"].ToString()),
                                Plan_Status =objem.GetHSEPerformanceNameById(dsCompetenceList.Tables[0].Rows[i]["Plan_Status"].ToString()),
                                Performance =objem.GetDrillPerformanceNameById(dsCompetenceList.Tables[0].Rows[i]["Performance"].ToString()),
                                Remarks = (dsCompetenceList.Tables[0].Rows[i]["Remarks"].ToString()),
                                ReportNo = dsCompetenceList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                Upload_Report = (dsCompetenceList.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCompetenceList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCompetenceList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_From"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_From = dateValue;
                            }

                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_To"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_To = dateValue;
                            }

                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_Date_Time"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_Date_Time = dateValue;
                            }

                            objEmergencyPlanNRecordList.lstEmergencyPlanNRecord.Add(objEmployeeCompetence);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objEmergencyPlanNRecordList.lstEmergencyPlanNRecord.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public JsonResult EmergencyPlanNRecordListSearch(string SearchText, int? page, string branch_name)
        {
            EmergencyPlanNRecordModelsList objEmergencyPlanNRecordList = new EmergencyPlanNRecordModelsList();
            objEmergencyPlanNRecordList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();
            EmergencyPlanNRecordModels objem = new EmergencyPlanNRecordModels();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";
                string sSqlstmt = "select Emergency_Plan_Id, emp_id, Plan_From, Plan_To, Emergency_Type, Plan_Date_Time, Drill_Location, Incharge, Need_Reporting, "
                    + " Plan_Status, Performance, Remarks, ReportNo, Upload_Report, LoggedBy, ReviewedBy, ApprovedBy,branch,Department,Location from t_emergency_plan_record where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Plan_From desc";
                DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCompetenceList.Tables.Count > 0)
                { 
                    for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            EmergencyPlanNRecordModels objEmployeeCompetence = new EmergencyPlanNRecordModels
                            {
                                Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[i]["Emergency_Plan_Id"].ToString(),
                                emp_id = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["emp_id"].ToString()),
                                Emergency_Type = objem.GetEmergencyTypeNameById(dsCompetenceList.Tables[0].Rows[i]["Emergency_Type"].ToString()),
                                Drill_Location = objGlobaldata.GetCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[i]["Drill_Location"].ToString()),
                                Incharge = dsCompetenceList.Tables[0].Rows[i]["Incharge"].ToString(),
                                Need_Reporting = (dsCompetenceList.Tables[0].Rows[i]["Need_Reporting"].ToString()),
                                Plan_Status = objem.GetHSEPerformanceNameById(dsCompetenceList.Tables[0].Rows[i]["Plan_Status"].ToString()),
                                Performance = objem.GetDrillPerformanceNameById(dsCompetenceList.Tables[0].Rows[i]["Performance"].ToString()),
                                Remarks = (dsCompetenceList.Tables[0].Rows[i]["Remarks"].ToString()),
                                ReportNo = dsCompetenceList.Tables[0].Rows[i]["ReportNo"].ToString(),
                                Upload_Report = (dsCompetenceList.Tables[0].Rows[i]["Upload_Report"].ToString()),
                                ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["ReviewedBy"].ToString()),
                                ApprovedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCompetenceList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCompetenceList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_From"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_From = dateValue;
                            }

                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_To"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_To = dateValue;
                            }

                            if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Plan_Date_Time"].ToString(), out dateValue))
                            {
                                objEmployeeCompetence.Plan_Date_Time = dateValue;
                            }

                            objEmergencyPlanNRecordList.lstEmergencyPlanNRecord.Add(objEmployeeCompetence);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
              
        [AllowAnonymous]
        public ActionResult EmergencyPlanNRecordDetails()
        {
            try
            {
                EmergencyPlanNRecordModels objem = new EmergencyPlanNRecordModels();
                if (Request.QueryString["Emergency_Plan_Id"] != null && Request.QueryString["Emergency_Plan_Id"] != "")
                {
                    string sEmergency_Plan_Id = Request.QueryString["Emergency_Plan_Id"];
                    string sSqlstmt = "select Emergency_Plan_Id, emp_id, Plan_From, Plan_To, Emergency_Type, Plan_Date_Time, Drill_Location, Incharge, Need_Reporting, "
                        + " Plan_Status, Performance, Remarks, ReportNo, Upload_Report, LoggedBy, ReviewedBy, ApprovedBy,Observation,No_Emp,branch,Department,Location from t_emergency_plan_record where "
                        + "Emergency_Plan_Id='" + sEmergency_Plan_Id + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0)
                    {
                        EmergencyPlanNRecordModels objEmployeeCompetence = new EmergencyPlanNRecordModels
                        {
                            Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[0]["Emergency_Plan_Id"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["emp_id"].ToString()),
                            Emergency_Type =objem.GetEmergencyTypeNameById(dsCompetenceList.Tables[0].Rows[0]["Emergency_Type"].ToString()),
                            Drill_Location = objGlobaldata.GetCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[0]["Drill_Location"].ToString()),
                            Incharge = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["Incharge"].ToString()),
                            Need_Reporting = (dsCompetenceList.Tables[0].Rows[0]["Need_Reporting"].ToString()),
                            Plan_Status =objem.GetHSEPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Plan_Status"].ToString()),
                            Performance =objem.GetDrillPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Performance"].ToString()),
                            Remarks = (dsCompetenceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            ReportNo = dsCompetenceList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Upload_Report = (dsCompetenceList.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Observation = (dsCompetenceList.Tables[0].Rows[0]["Observation"].ToString()),
                            No_Emp = (dsCompetenceList.Tables[0].Rows[0]["No_Emp"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCompetenceList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCompetenceList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_From"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_From = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_To"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_To = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_Date_Time"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_Date_Time = dateValue;
                        }

                        EmergencyPlanNRecordModelsList objPlanList = new EmergencyPlanNRecordModelsList();
                        objPlanList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();

                        sSqlstmt = "SELECT id_emergency_trans,Emergency_Plan_Id,Corrrective_action,Target_date FROM t_emergency_plan_record_trans where Emergency_Plan_Id='" + sEmergency_Plan_Id
                            + "' order by id_emergency_trans asc";

                        dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmergencyPlanNRecordModels objIncident = new EmergencyPlanNRecordModels
                                    {
                                        id_emergency_trans = Convert.ToInt32(dsCompetenceList.Tables[0].Rows[i]["id_emergency_trans"].ToString()),
                                        Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[i]["Emergency_Plan_Id"].ToString(),
                                        Corrrective_action = dsCompetenceList.Tables[0].Rows[i]["Corrrective_action"].ToString()

                                    };
                                    if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Target_date"].ToString(), out dateValue))
                                    {
                                        objIncident.Target_date = dateValue;
                                    }
                                    objPlanList.lstEmergencyPlanNRecord.Add(objIncident);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

                                }
                            }
                            ViewBag.objEmergency = objPlanList;
                        }

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmergencyPlanNRecordList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Plan Id cannot be null";
                    return RedirectToAction("EmergencyPlanNRecordList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmergencyPlanNRecordList");
        }
                
        [AllowAnonymous]
        public ActionResult EmergencyPlanNRecordInfo(int id)
        {
            try
            {
                EmergencyPlanNRecordModels objem = new EmergencyPlanNRecordModels();
               
                    string sSqlstmt = "select Emergency_Plan_Id, emp_id, Plan_From, Plan_To, Emergency_Type, Plan_Date_Time, Drill_Location, Incharge, Need_Reporting, "
                        + " Plan_Status, Performance, Remarks, ReportNo, Upload_Report, LoggedBy, ReviewedBy, ApprovedBy,Observation,No_Emp,branch,Department,Location from t_emergency_plan_record where "
                        + "Emergency_Plan_Id='" + id + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0)
                    {

                        EmergencyPlanNRecordModels objEmployeeCompetence = new EmergencyPlanNRecordModels
                        {
                            Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[0]["Emergency_Plan_Id"].ToString(),
                            emp_id = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["emp_id"].ToString()),
                            Emergency_Type = objem.GetEmergencyTypeNameById(dsCompetenceList.Tables[0].Rows[0]["Emergency_Type"].ToString()),
                            Drill_Location =objGlobaldata.GetCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[0]["Drill_Location"].ToString()),
                            Incharge = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["Incharge"].ToString()),
                            Need_Reporting = (dsCompetenceList.Tables[0].Rows[0]["Need_Reporting"].ToString()),
                            Plan_Status = objem.GetHSEPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Plan_Status"].ToString()),
                            Performance = objem.GetDrillPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Performance"].ToString()),
                            Remarks = (dsCompetenceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            ReportNo = dsCompetenceList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Upload_Report = (dsCompetenceList.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            ReviewedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Observation = (dsCompetenceList.Tables[0].Rows[0]["Observation"].ToString()),
                            No_Emp = (dsCompetenceList.Tables[0].Rows[0]["No_Emp"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCompetenceList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCompetenceList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCompetenceList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_From"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_From = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_To"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_To = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_Date_Time"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_Date_Time = dateValue;
                        }

                        EmergencyPlanNRecordModelsList objPlanList = new EmergencyPlanNRecordModelsList();
                        objPlanList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();

                        sSqlstmt = "SELECT id_emergency_trans,Emergency_Plan_Id,Corrrective_action,Target_date FROM t_emergency_plan_record_trans where Emergency_Plan_Id='" + id
                            + "' order by id_emergency_trans asc";

                        dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmergencyPlanNRecordModels objIncident = new EmergencyPlanNRecordModels
                                    {
                                        id_emergency_trans = Convert.ToInt32(dsCompetenceList.Tables[0].Rows[i]["id_emergency_trans"].ToString()),
                                        Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[i]["Emergency_Plan_Id"].ToString(),
                                        Corrrective_action = dsCompetenceList.Tables[0].Rows[i]["Corrrective_action"].ToString()

                                    };
                                    if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Target_date"].ToString(), out dateValue))
                                    {
                                        objIncident.Target_date = dateValue;
                                    }
                                    objPlanList.lstEmergencyPlanNRecord.Add(objIncident);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

                                }
                            }
                            ViewBag.objEmergency = objPlanList;
                        }

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmergencyPlanNRecordList");
                    }
             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmergencyPlanNRecordList");
        }
               
        [AllowAnonymous]
        public ActionResult EmergencyPlanNRecordEdit()
        {
            try
            {
                EmergencyPlanNRecordModels objem = new EmergencyPlanNRecordModels();
               
                if (Request.QueryString["Emergency_Plan_Id"] != null && Request.QueryString["Emergency_Plan_Id"] != "")
                {
                    string sEmergency_Plan_Id = Request.QueryString["Emergency_Plan_Id"];
                    string sSqlstmt = "select Emergency_Plan_Id, emp_id, Plan_From, Plan_To, Emergency_Type, Plan_Date_Time, Drill_Location, Incharge, Need_Reporting, "
                        + " Plan_Status, Performance, Remarks, ReportNo, Upload_Report, LoggedBy, ReviewedBy, ApprovedBy,Observation,No_Emp,branch,Department,Location from t_emergency_plan_record where "
                        + "Emergency_Plan_Id='" + sEmergency_Plan_Id + "'";

                    DataSet dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCompetenceList.Tables.Count > 0)
                    {

                        EmergencyPlanNRecordModels objEmployeeCompetence = new EmergencyPlanNRecordModels
                        {
                            Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[0]["Emergency_Plan_Id"].ToString(),
                            emp_id = (dsCompetenceList.Tables[0].Rows[0]["emp_id"].ToString()),
                            Emergency_Type =objem.GetEmergencyTypeNameById(dsCompetenceList.Tables[0].Rows[0]["Emergency_Type"].ToString()),
                            Drill_Location = dsCompetenceList.Tables[0].Rows[0]["Drill_Location"].ToString(),
                            Incharge = dsCompetenceList.Tables[0].Rows[0]["Incharge"].ToString(),
                            Need_Reporting = (dsCompetenceList.Tables[0].Rows[0]["Need_Reporting"].ToString()),
                            Plan_Status =objem.GetHSEPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Plan_Status"].ToString()),
                            Performance =objem.GetDrillPerformanceNameById(dsCompetenceList.Tables[0].Rows[0]["Performance"].ToString()),
                            Remarks = (dsCompetenceList.Tables[0].Rows[0]["Remarks"].ToString()),
                            ReportNo = dsCompetenceList.Tables[0].Rows[0]["ReportNo"].ToString(),
                            Upload_Report = (dsCompetenceList.Tables[0].Rows[0]["Upload_Report"].ToString()),
                            ReviewedBy = (dsCompetenceList.Tables[0].Rows[0]["ReviewedBy"].ToString()),
                            ApprovedBy = (dsCompetenceList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsCompetenceList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            Observation = (dsCompetenceList.Tables[0].Rows[0]["Observation"].ToString()),
                            No_Emp = (dsCompetenceList.Tables[0].Rows[0]["No_Emp"].ToString()),
                            branch = (dsCompetenceList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsCompetenceList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsCompetenceList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_From"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_From = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_To"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_To = dateValue;
                        }

                        if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[0]["Plan_Date_Time"].ToString(), out dateValue))
                        {
                            objEmployeeCompetence.Plan_Date_Time = dateValue;                            
                        }
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsCompetenceList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCompetenceList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                                                EmergencyPlanNRecordModelsList objPlanList = new EmergencyPlanNRecordModelsList();
                        objPlanList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();

                        sSqlstmt = "SELECT id_emergency_trans,Emergency_Plan_Id,Corrrective_action,Target_date FROM t_emergency_plan_record_trans where Emergency_Plan_Id='" + sEmergency_Plan_Id
                            + "' order by id_emergency_trans asc";

                        dsCompetenceList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsCompetenceList.Tables.Count > 0 && dsCompetenceList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCompetenceList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    EmergencyPlanNRecordModels objIncident = new EmergencyPlanNRecordModels
                                    {
                                        id_emergency_trans =Convert.ToInt32(dsCompetenceList.Tables[0].Rows[i]["id_emergency_trans"].ToString()),
                                        Emergency_Plan_Id = dsCompetenceList.Tables[0].Rows[i]["Emergency_Plan_Id"].ToString(),
                                        Corrrective_action = dsCompetenceList.Tables[0].Rows[i]["Corrrective_action"].ToString()
                                        
                                    };
                                    if (DateTime.TryParse(dsCompetenceList.Tables[0].Rows[i]["Target_date"].ToString(), out dateValue))
                                    {
                                        objIncident.Target_date = dateValue;
                                    }
                                  
                                    objPlanList.lstEmergencyPlanNRecord.Add(objIncident);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                
                                }
                            }
                            ViewBag.objEmergency = objPlanList;
                        }

                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
                        ViewBag.EmergencyType = objem.GetMultiEmergencyTypeList();
                        ViewBag.HSEPerformance = objem.GetMultiHSEPerformanceList();
                        ViewBag.DrillPerformance = objem.GetMultiDrillPerformanceList();
                        ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.Drill_Location = objGlobaldata.GetCompanyBranchListbox();

                        return View(objEmployeeCompetence);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("EmergencyPlanNRecordList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Plan Id cannot be null";
                    return RedirectToAction("EmergencyPlanNRecordList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmergencyPlanNRecordList");
        }
         
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EmergencyPlanNRecordEdit(EmergencyPlanNRecordModels objEmergencyPlanNRecord, FormCollection form, HttpPostedFileBase Upload_Report)
        {
            try
            {
                objEmergencyPlanNRecord.branch = form["branch"];
                objEmergencyPlanNRecord.Location = form["Location"];
                objEmergencyPlanNRecord.Department = form["Department"];

                DateTime dateValue;

                if (Upload_Report != null && Upload_Report.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(Upload_Report.FileName));
                        string sFilename = "Emergency" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload_Report.SaveAs(sFilepath + "/" + sFilename);
                        objEmergencyPlanNRecord.Upload_Report = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddEmergencyPlanNRecord: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["Plan_From"] != null && DateTime.TryParse(form["Plan_From"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_From = dateValue;
                }

                if (form["Plan_To"] != null && DateTime.TryParse(form["Plan_To"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_To = dateValue;
                }

                if (form["Plan_Date_Time"] != null && DateTime.TryParse(form["Plan_Date_Time"], out dateValue) == true)
                {
                    objEmergencyPlanNRecord.Plan_Date_Time = dateValue;
                    int iHr, iMin;
                    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                    {
                        objEmergencyPlanNRecord.Plan_Date_Time = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                    }                    
                }
                EmergencyPlanNRecordModelsList objPlanList = new EmergencyPlanNRecordModelsList();
                objPlanList.lstEmergencyPlanNRecord = new List<EmergencyPlanNRecordModels>();
                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["id_emergency_trans " + i] != null || form["Corrrective_action " + i] != null)
                        {
                            EmergencyPlanNRecordModels objPlan = new EmergencyPlanNRecordModels();

                            objPlan.Corrrective_action = form["Corrrective_action " + i];
                            if (DateTime.TryParse(form["Target_date " + i], out dateValue) == true)
                            {
                                objPlan.Emergency_Plan_Id = objPlan.Emergency_Plan_Id;
                                objPlan.Target_date = dateValue;
                            }
                            objPlanList.lstEmergencyPlanNRecord.Add(objPlan);
                        }
                    }
                }

                if (objEmergencyPlanNRecord.FunUpdateEmergencyPlan(objEmergencyPlanNRecord, objPlanList))
                {
                    TempData["Successdata"] = "Updated Emerygency Plan details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmergencyPlanNRecordEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("EmergencyPlanNRecordList");
        }
    }
}
