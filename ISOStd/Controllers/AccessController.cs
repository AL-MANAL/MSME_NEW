using ISOStd.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class AccessController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public AccessController()
        {
            ViewBag.Menutype = "Access Privileges";
            ViewBag.SubMenutype = "Access Privileges";
        }

        [AllowAnonymous]
        public JsonResult UpdateAccess(string selected, int status, string role_id)
        {
            AccessModels objAccessList = new AccessModels();
            bool IssueNo = false;
            if (selected != null)
            {
                IssueNo = objAccessList.FunUpdateAccess(selected, status, role_id);
            }

            return Json(IssueNo);
        }

        public ActionResult AccessList(int? page, string chkAll, string RoleId, FormCollection form)
        {
            AccessModelsList objAccessList = new AccessModelsList();
            objAccessList.AccessList = new List<AccessModels>();
            try
            {
                string sSqlstmt = "";
                string sSearchtext = "";

                //if (Request.QueryString["appl_branch"] != null && Request.QueryString["appl_branch"] != "" && Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "" && Request.QueryString["branch_id"] != null && Request.QueryString["branch_id"] != "")
                if (Request.QueryString["RoleId"] != null && Request.QueryString["RoleId"] != "")
                {
                    string appl_branch = Request.QueryString["appl_branch"];
                    string sRoleID = Request.QueryString["RoleId"];
                    string sbranch_id = Request.QueryString["branch_id"];
                    string role = objGlobaldata.GetRolesNameById(sRoleID);
                    string branch = objGlobaldata.GetCompanyBranchNameById(sbranch_id);

                    //if (appl_branch == "Yes")
                    //{
                    //    ViewBag.Role = String.Concat(role, "-", branch);
                    //}
                    //else
                    //{
                    //    ViewBag.Role = String.Concat(role);
                    //}
                    sSqlstmt = "Select * from t_access";

                    //sSearchtext = sSearchtext + " where role_id ='" + sRoleID + "' and branch_id ='" + sbranch_id + "'";
                    sSearchtext = sSearchtext + " where role_id ='" + sRoleID + "'";
                    sSqlstmt = sSqlstmt + sSearchtext;

                    DataSet dsPrivileges = objGlobaldata.Getdetails(sSqlstmt);

                    for (int i = 0; dsPrivileges.Tables.Count > 0 && i < dsPrivileges.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AccessModels objAccess = new AccessModels
                            {
                                Id_access = dsPrivileges.Tables[0].Rows[i]["Id_access"].ToString(),
                                //EmpId = objGlobaldata.GetEmpIDByEmpNo(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                                //Emp_Name = objGlobaldata.GetEmpHrNameById(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                                role_id = dsPrivileges.Tables[0].Rows[i]["role_id"].ToString(),
                                branch_id = dsPrivileges.Tables[0].Rows[i]["branch_id"].ToString(),
                                Events = dsPrivileges.Tables[0].Rows[i]["Events"].ToString(),
                                Documents = dsPrivileges.Tables[0].Rows[i]["Documents"].ToString(),
                                InternalDoc = dsPrivileges.Tables[0].Rows[i]["InternalDoc"].ToString(),
                                ExternalDoc = dsPrivileges.Tables[0].Rows[i]["ExternalDoc"].ToString(),
                                Record = dsPrivileges.Tables[0].Rows[i]["Record"].ToString(),
                                DocChangeReq = dsPrivileges.Tables[0].Rows[i]["DocChangeReq"].ToString(),
                                DocTrack = dsPrivileges.Tables[0].Rows[i]["DocTrack"].ToString(),
                                ObjKPI = dsPrivileges.Tables[0].Rows[i]["ObjKPI"].ToString(),
                                Objectives = dsPrivileges.Tables[0].Rows[i]["Objectives"].ToString(),
                                Kpi = dsPrivileges.Tables[0].Rows[i]["Kpi"].ToString(),
                                RiskMgmt = dsPrivileges.Tables[0].Rows[i]["RiskMgmt"].ToString(),
                                ChangeMgmt = dsPrivileges.Tables[0].Rows[i]["ChangeMgmt"].ToString(),
                                Parties = dsPrivileges.Tables[0].Rows[i]["Parties"].ToString(),
                                Issues = dsPrivileges.Tables[0].Rows[i]["Issues"].ToString(),
                                Risk = dsPrivileges.Tables[0].Rows[i]["Risk"].ToString(),
                                HazardRiskReg = dsPrivileges.Tables[0].Rows[i]["HazardRiskReg"].ToString(),
                                HR = dsPrivileges.Tables[0].Rows[i]["HR"].ToString(),
                                Emp = dsPrivileges.Tables[0].Rows[i]["Emp"].ToString(),
                                EmpPerfEval = dsPrivileges.Tables[0].Rows[i]["EmpPerfEval"].ToString(),
                                Competancy = dsPrivileges.Tables[0].Rows[i]["Competancy"].ToString(),
                                OrgChart = dsPrivileges.Tables[0].Rows[i]["OrgChart"].ToString(),
                                ManHour = dsPrivileges.Tables[0].Rows[i]["ManHour"].ToString(),
                                ExitEmp = dsPrivileges.Tables[0].Rows[i]["ExitEmp"].ToString(),
                                Visitor = dsPrivileges.Tables[0].Rows[i]["Visitor"].ToString(),
                                LeaveMgmt = dsPrivileges.Tables[0].Rows[i]["LeaveMgmt"].ToString(),
                                LeaveCredit = dsPrivileges.Tables[0].Rows[i]["LeaveCredit"].ToString(),
                                AdjustLeave = dsPrivileges.Tables[0].Rows[i]["AdjustLeave"].ToString(),
                                ApplyLeave = dsPrivileges.Tables[0].Rows[i]["ApplyLeave"].ToString(),
                                LeaveMaster = dsPrivileges.Tables[0].Rows[i]["LeaveMaster"].ToString(),
                                Holiday = dsPrivileges.Tables[0].Rows[i]["Holiday"].ToString(),
                                ATSS = dsPrivileges.Tables[0].Rows[i]["ATSS"].ToString(),
                                ATS = dsPrivileges.Tables[0].Rows[i]["ATS"].ToString(),
                                HseATS = dsPrivileges.Tables[0].Rows[i]["HseATS"].ToString(),
                                Meeting = dsPrivileges.Tables[0].Rows[i]["Meeting"].ToString(),
                                MAgenda = dsPrivileges.Tables[0].Rows[i]["MAgenda"].ToString(),
                                MSchedule = dsPrivileges.Tables[0].Rows[i]["MSchedule"].ToString(),
                                MUnplaned = dsPrivileges.Tables[0].Rows[i]["MUnplaned"].ToString(),
                                Suppliers = dsPrivileges.Tables[0].Rows[i]["Suppliers"].ToString(),
                                Supplier = dsPrivileges.Tables[0].Rows[i]["Supplier"].ToString(),
                                DLog = dsPrivileges.Tables[0].Rows[i]["DLog"].ToString(),
                                SupplierPer = dsPrivileges.Tables[0].Rows[i]["SupplierPer"].ToString(),
                                Providers = dsPrivileges.Tables[0].Rows[i]["Providers"].ToString(),
                                SupplierRate = dsPrivileges.Tables[0].Rows[i]["SupplierRate"].ToString(),
                                CustMgmt = dsPrivileges.Tables[0].Rows[i]["CustMgmt"].ToString(),
                                Visits = dsPrivileges.Tables[0].Rows[i]["Visits"].ToString(),
                                AddCust = dsPrivileges.Tables[0].Rows[i]["AddCust"].ToString(),
                                Complaints = dsPrivileges.Tables[0].Rows[i]["Complaints"].ToString(),
                                SurveyQues = dsPrivileges.Tables[0].Rows[i]["SurveyQues"].ToString(),
                                UploadSurvey = dsPrivileges.Tables[0].Rows[i]["UploadSurvey"].ToString(),
                                CustReturnProcuct = dsPrivileges.Tables[0].Rows[i]["CustReturnProcuct"].ToString(),
                                Bidding = dsPrivileges.Tables[0].Rows[i]["Bidding"].ToString(),
                                TrainingOri = dsPrivileges.Tables[0].Rows[i]["TrainingOri"].ToString(),
                                AddTopic = dsPrivileges.Tables[0].Rows[i]["AddTopic"].ToString(),
                                Perftraining = dsPrivileges.Tables[0].Rows[i]["Perftraining"].ToString(),
                                EmpTrainingOri = dsPrivileges.Tables[0].Rows[i]["EmpTrainingOri"].ToString(),
                                Audit = dsPrivileges.Tables[0].Rows[i]["Audit"].ToString(),
                                InterAudit = dsPrivileges.Tables[0].Rows[i]["InterAudit"].ToString(),
                                ExterAudit = dsPrivileges.Tables[0].Rows[i]["ExterAudit"].ToString(),
                                ExtAuditRpt = dsPrivileges.Tables[0].Rows[i]["ExtAuditRpt"].ToString(),
                                CustAudit = dsPrivileges.Tables[0].Rows[i]["CustAudit"].ToString(),
                                RaiseNc = dsPrivileges.Tables[0].Rows[i]["RaiseNc"].ToString(),
                                InterChecklist = dsPrivileges.Tables[0].Rows[i]["InterChecklist"].ToString(),
                                AuditChecklist = dsPrivileges.Tables[0].Rows[i]["AuditChecklist"].ToString(),
                                HSE = dsPrivileges.Tables[0].Rows[i]["HSE"].ToString(),
                                IncidentRpt = dsPrivileges.Tables[0].Rows[i]["IncidentRpt"].ToString(),
                                NearMissRept = dsPrivileges.Tables[0].Rows[i]["NearMissRept"].ToString(),
                                EmergPlan = dsPrivileges.Tables[0].Rows[i]["EmergPlan"].ToString(),
                                ToolTalk = dsPrivileges.Tables[0].Rows[i]["ToolTalk"].ToString(),
                                SafetyLog = dsPrivileges.Tables[0].Rows[i]["SafetyLog"].ToString(),
                                PpeLog = dsPrivileges.Tables[0].Rows[i]["PpeLog"].ToString(),
                                HseInsp = dsPrivileges.Tables[0].Rows[i]["HseInsp"].ToString(),
                                AirNoise = dsPrivileges.Tables[0].Rows[i]["AirNoise"].ToString(),
                                Waste = dsPrivileges.Tables[0].Rows[i]["Waste"].ToString(),
                                ObservCard = dsPrivileges.Tables[0].Rows[i]["ObservCard"].ToString(),
                                HseIndu = dsPrivileges.Tables[0].Rows[i]["HseIndu"].ToString(),
                                FirstBox = dsPrivileges.Tables[0].Rows[i]["FirstBox"].ToString(),
                                FireInspection = dsPrivileges.Tables[0].Rows[i]["FireInspection"].ToString(),
                                Project = dsPrivileges.Tables[0].Rows[i]["Project"].ToString(),
                                Equip = dsPrivileges.Tables[0].Rows[i]["Equip"].ToString(),
                                Maintenance = dsPrivileges.Tables[0].Rows[i]["Maintenance"].ToString(),
                                Calibration = dsPrivileges.Tables[0].Rows[i]["Calibration"].ToString(),
                                LegalReg = dsPrivileges.Tables[0].Rows[i]["LegalReg"].ToString(),
                                Legal = dsPrivileges.Tables[0].Rows[i]["Legal"].ToString(),
                                Certificates = dsPrivileges.Tables[0].Rows[i]["Certificates"].ToString(),
                                Training = dsPrivileges.Tables[0].Rows[i]["Training"].ToString(),
                                TrainingList = dsPrivileges.Tables[0].Rows[i]["TrainingList"].ToString(),
                                TrainingCal = dsPrivileges.Tables[0].Rows[i]["TrainingCal"].ToString(),
                                Report = dsPrivileges.Tables[0].Rows[i]["Report"].ToString(),
                                Rept = dsPrivileges.Tables[0].Rows[i]["Rept"].ToString(),
                                DashRept = dsPrivileges.Tables[0].Rows[i]["DashRept"].ToString(),
                                MISRept = dsPrivileges.Tables[0].Rows[i]["MISRept"].ToString(),
                                Permits = dsPrivileges.Tables[0].Rows[i]["Permits"].ToString(),
                                AccessPermit = dsPrivileges.Tables[0].Rows[i]["AccessPermit"].ToString(),
                                WorkPermit = dsPrivileges.Tables[0].Rows[i]["WorkPermit"].ToString(),
                                Settings = dsPrivileges.Tables[0].Rows[i]["Settings"].ToString(),
                                Company = dsPrivileges.Tables[0].Rows[i]["Company"].ToString(),
                                Dept = dsPrivileges.Tables[0].Rows[i]["Dept"].ToString(),
                                User = dsPrivileges.Tables[0].Rows[i]["User"].ToString(),
                                DropDown = dsPrivileges.Tables[0].Rows[i]["DropDown"].ToString(),
                                EmpRole = dsPrivileges.Tables[0].Rows[i]["EmpRole"].ToString(),
                                ISOStd = dsPrivileges.Tables[0].Rows[i]["ISOStd"].ToString(),
                                TRA = dsPrivileges.Tables[0].Rows[i]["TRA"].ToString(),
                                ResConsump = dsPrivileges.Tables[0].Rows[i]["ResConsump"].ToString(),
                            };
                            objAccessList.AccessList.Add(objAccess);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAccessList.AccessList.ToList().ToPagedList(page ?? 1, 1000));
        }

        public JsonResult FungetAccessList(string RoleId, string branch_id)
        {
            AccessModels objAccess = new AccessModels();

            AccessModelsList objAccessList = new AccessModelsList();
            objAccessList.AccessList = new List<AccessModels>();
            try
            {
                string sSqlstmt = "";
                string sSearchtext = "";

                //if (RoleId != null && RoleId != "" && branch_id != null && branch_id != "")
                if (RoleId != null && RoleId != "")
                {
                    string sRoleID = RoleId;
                    string sbranch_id = branch_id;
                    sSqlstmt = "Select * from t_access";
                    ViewBag.Roleval = sRoleID;
                    //sSearchtext = sSearchtext + " where role_id ='" + sRoleID + "' and branch_id ='" + sbranch_id + "'";
                    sSearchtext = sSearchtext + " where role_id ='" + sRoleID + "'";
                    sSqlstmt = sSqlstmt + sSearchtext;

                    DataSet dsPrivileges = objGlobaldata.Getdetails(sSqlstmt);

                    for (int i = 0; dsPrivileges.Tables.Count > 0 && i < dsPrivileges.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objAccess = new AccessModels
                            {
                                Id_access = dsPrivileges.Tables[0].Rows[i]["Id_access"].ToString(),
                                //EmpId = objGlobaldata.GetEmpIDByEmpNo(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                                //Emp_Name = objGlobaldata.GetEmpHrNameById(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                                role_id = dsPrivileges.Tables[0].Rows[i]["role_id"].ToString(),
                                branch_id = dsPrivileges.Tables[0].Rows[i]["branch_id"].ToString(),
                                Events = dsPrivileges.Tables[0].Rows[i]["Events"].ToString(),
                                Documents = dsPrivileges.Tables[0].Rows[i]["Documents"].ToString(),
                                InternalDoc = dsPrivileges.Tables[0].Rows[i]["InternalDoc"].ToString(),
                                ExternalDoc = dsPrivileges.Tables[0].Rows[i]["ExternalDoc"].ToString(),
                                Record = dsPrivileges.Tables[0].Rows[i]["Record"].ToString(),
                                DocChangeReq = dsPrivileges.Tables[0].Rows[i]["DocChangeReq"].ToString(),
                                DocTrack = dsPrivileges.Tables[0].Rows[i]["DocTrack"].ToString(),
                                ObjKPI = dsPrivileges.Tables[0].Rows[i]["ObjKPI"].ToString(),
                                Objectives = dsPrivileges.Tables[0].Rows[i]["Objectives"].ToString(),
                                Kpi = dsPrivileges.Tables[0].Rows[i]["Kpi"].ToString(),
                                RiskMgmt = dsPrivileges.Tables[0].Rows[i]["RiskMgmt"].ToString(),
                                ChangeMgmt = dsPrivileges.Tables[0].Rows[i]["ChangeMgmt"].ToString(),
                                ContextOrganise = dsPrivileges.Tables[0].Rows[i]["ContextOrganise"].ToString(),
                                Parties = dsPrivileges.Tables[0].Rows[i]["Parties"].ToString(),
                                Issues = dsPrivileges.Tables[0].Rows[i]["Issues"].ToString(),
                                Risk = dsPrivileges.Tables[0].Rows[i]["Risk"].ToString(),
                                HazardRiskReg = dsPrivileges.Tables[0].Rows[i]["HazardRiskReg"].ToString(),
                                HR = dsPrivileges.Tables[0].Rows[i]["HR"].ToString(),
                                Emp = dsPrivileges.Tables[0].Rows[i]["Emp"].ToString(),
                                EmpPerfEval = dsPrivileges.Tables[0].Rows[i]["EmpPerfEval"].ToString(),
                                Competancy = dsPrivileges.Tables[0].Rows[i]["Competancy"].ToString(),
                                OrgChart = dsPrivileges.Tables[0].Rows[i]["OrgChart"].ToString(),
                                ManHour = dsPrivileges.Tables[0].Rows[i]["ManHour"].ToString(),
                                ExitEmp = dsPrivileges.Tables[0].Rows[i]["ExitEmp"].ToString(),
                                Visitor = dsPrivileges.Tables[0].Rows[i]["Visitor"].ToString(),
                                LeaveMgmt = dsPrivileges.Tables[0].Rows[i]["LeaveMgmt"].ToString(),
                                LeaveCredit = dsPrivileges.Tables[0].Rows[i]["LeaveCredit"].ToString(),
                                AdjustLeave = dsPrivileges.Tables[0].Rows[i]["AdjustLeave"].ToString(),
                                ApplyLeave = dsPrivileges.Tables[0].Rows[i]["ApplyLeave"].ToString(),
                                LeaveMaster = dsPrivileges.Tables[0].Rows[i]["LeaveMaster"].ToString(),
                                Holiday = dsPrivileges.Tables[0].Rows[i]["Holiday"].ToString(),
                                ATSS = dsPrivileges.Tables[0].Rows[i]["ATSS"].ToString(),
                                ATS = dsPrivileges.Tables[0].Rows[i]["ATS"].ToString(),
                                HseATS = dsPrivileges.Tables[0].Rows[i]["HseATS"].ToString(),
                                Meeting = dsPrivileges.Tables[0].Rows[i]["Meeting"].ToString(),
                                MAgenda = dsPrivileges.Tables[0].Rows[i]["MAgenda"].ToString(),
                                MSchedule = dsPrivileges.Tables[0].Rows[i]["MSchedule"].ToString(),
                                MUnplaned = dsPrivileges.Tables[0].Rows[i]["MUnplaned"].ToString(),
                                Suppliers = dsPrivileges.Tables[0].Rows[i]["Suppliers"].ToString(),
                                Supplier = dsPrivileges.Tables[0].Rows[i]["Supplier"].ToString(),
                                DLog = dsPrivileges.Tables[0].Rows[i]["DLog"].ToString(),
                                SupplierPer = dsPrivileges.Tables[0].Rows[i]["SupplierPer"].ToString(),
                                Providers = dsPrivileges.Tables[0].Rows[i]["Providers"].ToString(),
                                SupplierRate = dsPrivileges.Tables[0].Rows[i]["SupplierRate"].ToString(),
                                CustMgmt = dsPrivileges.Tables[0].Rows[i]["CustMgmt"].ToString(),
                                Visits = dsPrivileges.Tables[0].Rows[i]["Visits"].ToString(),
                                AddCust = dsPrivileges.Tables[0].Rows[i]["AddCust"].ToString(),
                                Complaints = dsPrivileges.Tables[0].Rows[i]["Complaints"].ToString(),
                                SurveyQues = dsPrivileges.Tables[0].Rows[i]["SurveyQues"].ToString(),
                                UploadSurvey = dsPrivileges.Tables[0].Rows[i]["UploadSurvey"].ToString(),
                                CustReturnProcuct = dsPrivileges.Tables[0].Rows[i]["CustReturnProcuct"].ToString(),
                                Bidding = dsPrivileges.Tables[0].Rows[i]["Bidding"].ToString(),
                                TrainingOri = dsPrivileges.Tables[0].Rows[i]["TrainingOri"].ToString(),
                                AddTopic = dsPrivileges.Tables[0].Rows[i]["AddTopic"].ToString(),
                                Perftraining = dsPrivileges.Tables[0].Rows[i]["Perftraining"].ToString(),
                                EmpTrainingOri = dsPrivileges.Tables[0].Rows[i]["EmpTrainingOri"].ToString(),
                                Audit = dsPrivileges.Tables[0].Rows[i]["Audit"].ToString(),
                                InterAudit = dsPrivileges.Tables[0].Rows[i]["InterAudit"].ToString(),
                                ExterAudit = dsPrivileges.Tables[0].Rows[i]["ExterAudit"].ToString(),
                                ExtAuditRpt = dsPrivileges.Tables[0].Rows[i]["ExtAuditRpt"].ToString(),
                                CustAudit = dsPrivileges.Tables[0].Rows[i]["CustAudit"].ToString(),
                                RaiseNc = dsPrivileges.Tables[0].Rows[i]["RaiseNc"].ToString(),
                                InterChecklist = dsPrivileges.Tables[0].Rows[i]["InterChecklist"].ToString(),
                                AuditChecklist = dsPrivileges.Tables[0].Rows[i]["AuditChecklist"].ToString(),
                                HSE = dsPrivileges.Tables[0].Rows[i]["HSE"].ToString(),
                                IncidentRpt = dsPrivileges.Tables[0].Rows[i]["IncidentRpt"].ToString(),
                                NearMissRept = dsPrivileges.Tables[0].Rows[i]["NearMissRept"].ToString(),
                                EmergPlan = dsPrivileges.Tables[0].Rows[i]["EmergPlan"].ToString(),
                                ToolTalk = dsPrivileges.Tables[0].Rows[i]["ToolTalk"].ToString(),
                                SafetyLog = dsPrivileges.Tables[0].Rows[i]["SafetyLog"].ToString(),
                                PpeLog = dsPrivileges.Tables[0].Rows[i]["PpeLog"].ToString(),
                                HseInsp = dsPrivileges.Tables[0].Rows[i]["HseInsp"].ToString(),
                                AirNoise = dsPrivileges.Tables[0].Rows[i]["AirNoise"].ToString(),
                                Waste = dsPrivileges.Tables[0].Rows[i]["Waste"].ToString(),
                                ObservCard = dsPrivileges.Tables[0].Rows[i]["ObservCard"].ToString(),
                                HseIndu = dsPrivileges.Tables[0].Rows[i]["HseIndu"].ToString(),
                                FirstBox = dsPrivileges.Tables[0].Rows[i]["FirstBox"].ToString(),
                                FireInspection = dsPrivileges.Tables[0].Rows[i]["FireInspection"].ToString(),
                                Project = dsPrivileges.Tables[0].Rows[i]["Project"].ToString(),
                                Equip = dsPrivileges.Tables[0].Rows[i]["Equip"].ToString(),
                                Maintenance = dsPrivileges.Tables[0].Rows[i]["Maintenance"].ToString(),
                                Calibration = dsPrivileges.Tables[0].Rows[i]["Calibration"].ToString(),
                                LegalReg = dsPrivileges.Tables[0].Rows[i]["LegalReg"].ToString(),
                                Legal = dsPrivileges.Tables[0].Rows[i]["Legal"].ToString(),
                                Certificates = dsPrivileges.Tables[0].Rows[i]["Certificates"].ToString(),
                                Training = dsPrivileges.Tables[0].Rows[i]["Training"].ToString(),
                                TrainingList = dsPrivileges.Tables[0].Rows[i]["TrainingList"].ToString(),
                                TrainingCal = dsPrivileges.Tables[0].Rows[i]["TrainingCal"].ToString(),
                                Report = dsPrivileges.Tables[0].Rows[i]["Report"].ToString(),
                                Rept = dsPrivileges.Tables[0].Rows[i]["Rept"].ToString(),
                                DashRept = dsPrivileges.Tables[0].Rows[i]["DashRept"].ToString(),
                                MISRept = dsPrivileges.Tables[0].Rows[i]["MISRept"].ToString(),
                                Permits = dsPrivileges.Tables[0].Rows[i]["Permits"].ToString(),
                                AccessPermit = dsPrivileges.Tables[0].Rows[i]["AccessPermit"].ToString(),
                                WorkPermit = dsPrivileges.Tables[0].Rows[i]["WorkPermit"].ToString(),
                                Settings = dsPrivileges.Tables[0].Rows[i]["Settings"].ToString(),
                                Company = dsPrivileges.Tables[0].Rows[i]["Company"].ToString(),
                                Dept = dsPrivileges.Tables[0].Rows[i]["Dept"].ToString(),
                                User = dsPrivileges.Tables[0].Rows[i]["User"].ToString(),
                                DropDown = dsPrivileges.Tables[0].Rows[i]["DropDown"].ToString(),
                                EmpRole = dsPrivileges.Tables[0].Rows[i]["EmpRole"].ToString(),
                                ISOStd = dsPrivileges.Tables[0].Rows[i]["ISOStd"].ToString(),
                                TRA = dsPrivileges.Tables[0].Rows[i]["TRA"].ToString(),
                                ResConsump = dsPrivileges.Tables[0].Rows[i]["ResConsump"].ToString(),
                                NC = dsPrivileges.Tables[0].Rows[i]["NC"].ToString(),
                                Portal = dsPrivileges.Tables[0].Rows[i]["Portal"].ToString(),
                                sub_cr = dsPrivileges.Tables[0].Rows[i]["sub_cr"].ToString(),
                                access_portal = dsPrivileges.Tables[0].Rows[i]["access_portal"].ToString(),
                                portal_userlog = dsPrivileges.Tables[0].Rows[i]["portal_userlog"].ToString(),
                            };
                            objAccessList.AccessList.Add(objAccess);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AccessList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(objAccess);
        }
    }
}