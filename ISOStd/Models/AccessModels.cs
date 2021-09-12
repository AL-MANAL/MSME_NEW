using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class AccessModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "AuditNum")]
        public string Id_access { get; set; }

        [Display(Name = "Role")]
        public string role_id { get; set; }

        public string branch_id { get; set; }

        [Display(Name = "Events")]
        public string Events { get; set; }

        [Display(Name = "Document Management")]
        public string Documents { get; set; }

        [Display(Name = "Internal System Documents")]
        public string InternalDoc { get; set; }

        [Display(Name = "Document Tracking")]
        public string DocTrack { get; set; }

        [Display(Name = "External Documents")]
        public string ExternalDoc { get; set; }

        [Display(Name = "Records Mgmt")]
        public string Record { get; set; }

        [Display(Name = "Document Revise Request")]
        public string DocChangeReq { get; set; }

        [Display(Name = "Objectives & KPI's")]
        public string ObjKPI { get; set; }

        [Display(Name = "Objectives Mgmt")]
        public string Objectives { get; set; }

        [Display(Name = "Kpi Mgmt")]
        public string Kpi { get; set; }

        [Display(Name = " Risk Mgmt")]
        public string RiskMgmt { get; set; }

        [Display(Name = "Change Management(Moc)")]
        public string ChangeMgmt { get; set; }

        [Display(Name ="Context of Organisation")]
        public string ContextOrganise { get; set; }

        [Display(Name = "Internal/External Parties")]
        public string Parties { get; set; }

        [Display(Name = "Internal/External Issues")]
        public string Issues { get; set; }

        [Display(Name = "Risk Management")]
        public string Risk { get; set; }

        [Display(Name = "Environment/Hazard Risk Register")]
        public string HazardRiskReg { get; set; }

        [Display(Name = "Human Resources")]
        public string HR { get; set; }

        [Display(Name = "Employee Mgmt")]
        public string Emp { get; set; }

        [Display(Name = "Employee Performance Evaluation")]
        public string EmpPerfEval { get; set; }

        [Display(Name = "Competancy Evaluation")]
        public string Competancy { get; set; }

        [Display(Name = "Organization Chart")]
        public string OrgChart { get; set; }

        [Display(Name = "Man Hour")]
        public string ManHour { get; set; }

        [Display(Name = "Exit Employee List")]
        public string ExitEmp { get; set; }

        [Display(Name = "Visitors")]
        public string Visitor { get; set; }

        [Display(Name = "Leave Mgmt")]
        public string LeaveMgmt { get; set; }

        [Display(Name = "Annual Leave Credit")]
        public string LeaveCredit { get; set; }

        [Display(Name = "Leave Adjustment")]
        public string AdjustLeave { get; set; }

        [Display(Name = "Apply Leave")]
        public string ApplyLeave { get; set; }

        [Display(Name = "Leave Master")]
        public string LeaveMaster { get; set; }

        [Display(Name = "Holidays")]
        public string Holiday { get; set; }

        [Display(Name = "ATS")]
        public string ATSS { get; set; }

        [Display(Name = "Action Tracking Register")]
        public string ATS { get; set; }

        [Display(Name = "Hse Actions Tracking Register")]
        public string HseATS { get; set; }

        [Display(Name = "Meetings")]
        public string Meeting { get; set; }

        [Display(Name = "Meeting Type / Agenda")]
        public string MAgenda { get; set; }

        [Display(Name = "Schedule Meeting")]
        public string MSchedule { get; set; }

        [Display(Name = "Unplanned Meeting")]
        public string MUnplaned { get; set; }

        [Display(Name = "Suppliers")]
        public string Suppliers { get; set; }

        [Display(Name = "Ext Provider/Supplier")]
        public string Supplier { get; set; }

        [Display(Name = "Discrepancy Log")]
        public string DLog { get; set; }

        [Display(Name = "Supplier Performance")]
        public string SupplierPer { get; set; }

        [Display(Name = "Appr Ext Provider List")]
        public string Providers { get; set; }

        [Display(Name = "Supplier Performance with rating")]
        public string SupplierRate { get; set; }

        [Display(Name = "Customer")]
        public string CustMgmt { get; set; }

        [Display(Name = "Visits")]
        public string Visits { get; set; }

        [Display(Name = "Add Customer/Send Feedback")]
        public string AddCust { get; set; }

        [Display(Name = "Complaints")]
        public string Complaints { get; set; }

        [Display(Name = "Survey Questions")]
        public string SurveyQues { get; set; }

        [Display(Name = "Upload Survey")]
        public string UploadSurvey { get; set; }

        [Display(Name = "Customer Return Product")]
        public string CustReturnProcuct { get; set; }

        [Display(Name = "Bidding")]
        public string Bidding { get; set; }

        [Display(Name = "Induction Training")]
        public string TrainingOri { get; set; }

        [Display(Name = "Add Topic")]
        public string AddTopic { get; set; }

        [Display(Name = "Perform Training")]
        public string Perftraining { get; set; }

        [Display(Name = "Training Lists")]
        public string EmpTrainingOri { get; set; }

        [Display(Name = "Audits")]
        public string Audit { get; set; }

        [Display(Name = "Internal Audit")]
        public string InterAudit { get; set; }

        [Display(Name = "External Audit")]
        public string ExterAudit { get; set; }

        [Display(Name = "External Audit Report")]
        public string ExtAuditRpt { get; set; }

        [Display(Name = "Customer Audit")]
        public string CustAudit { get; set; }

        [Display(Name = "Raise Nc")]
        public string RaiseNc { get; set; }

        [Display(Name = "Internal Audit Checklist(With Upload)")]
        public string InterChecklist { get; set; }

        [Display(Name = "Audit Checklist & Perform Audit")]
        public string AuditChecklist { get; set; }

        [Display(Name = "HSE")]
        public string HSE { get; set; }

        [Display(Name = "Incident Reports")]
        public string IncidentRpt { get; set; }

        [Display(Name = "Near Miss Reporting")]
        public string NearMissRept { get; set; }

        [Display(Name = "Emergency Plan And Record")]
        public string EmergPlan { get; set; }

        [Display(Name = "Toolbox Talks Log And Report")]
        public string ToolTalk { get; set; }

        [Display(Name = "Safety Violation Log And Report")]
        public string SafetyLog { get; set; }

        [Display(Name = "Ppe Issue Log")]
        public string PpeLog { get; set; }

        [Display(Name = "Hse Inspection")]
        public string HseInsp { get; set; }

        [Display(Name = "Air/Water/Noise Quality Survey")]
        public string AirNoise { get; set; }

        [Display(Name = "Waste Management")]
        public string Waste { get; set; }

        [Display(Name = "Safety Observation Card")]
        public string ObservCard { get; set; }

        [Display(Name = "Hse Induction")]
        public string HseIndu { get; set; }

        [Display(Name = "First Aid Box")]
        public string FirstBox { get; set; }

        [Display(Name = "Fire Equipment Inspection")]
        public string FireInspection { get; set; }

        [Display(Name = "Projects")]
        public string Project { get; set; }

        [Display(Name = "Equipments")]
        public string Equip { get; set; }

        [Display(Name = "Maintenance")]
        public string Maintenance { get; set; }

        [Display(Name = "Calibration")]
        public string Calibration { get; set; }

        [Display(Name = "Compliance")]
        public string LegalReg { get; set; }

        [Display(Name = "Legal Register")]
        public string Legal { get; set; }

        [Display(Name = "Certificates")]
        public string Certificates { get; set; }

        [Display(Name = "Trainings")]
        public string Training { get; set; }

        [Display(Name = "Training List")]
        public string TrainingList { get; set; }

        [Display(Name = "Training Calender")]
        public string TrainingCal { get; set; }

        [Display(Name = "Reports")]
        public string Report { get; set; }

        [Display(Name = "Reports")]
        public string Rept { get; set; }

        [Display(Name = "Dashboard Reports")]
        public string DashRept { get; set; }

        [Display(Name = "MIS Reports")]
        public string MISRept { get; set; }

        [Display(Name = "Work Permits")]
        public string Permits { get; set; }

        [Display(Name = "Access Work Permit")]
        public string AccessPermit { get; set; }

        [Display(Name = "Work Permit")]
        public string WorkPermit { get; set; }

        [Display(Name = "Settings")]
        public string Settings { get; set; }

        [Display(Name = "Company Profile")]
        public string Company { get; set; }

        [Display(Name = "Department")]
        public string Dept { get; set; }

        [Display(Name = "User Profile")]
        public string User { get; set; }

        [Display(Name = "Mgmt Dropdown Data")]
        public string DropDown { get; set; }

        [Display(Name = "Roles")]
        public string EmpRole { get; set; }

        [Display(Name = "ISO Standards")]
        public string ISOStd { get; set; }
       
        public string TRA { get; set; }
      
        public string ResConsump { get; set; }

        public string NC { get; set; }

        public string Portal { get; set; }

        public string sub_cr { get; set; }

        public string access_portal { get; set; }

        public string portal_userlog { get; set; }

        //internal bool FunUpdateAccess(string selected, int status)
        //{
        //    try
        //    {
        //        string[] Sselected = null;
        //        Sselected = selected.Split(',');
        //        if (status == 1)
        //        {
        //            for (int j = 0; j <= Sselected.Length - 1; j = j + 2)
        //            {
        //                string sqlstmt = "update t_access set " + Sselected[j + 1] + "=1 where Id_access='" + Sselected[j] + "'";
        //                objGlobalData.ExecuteQuery(sqlstmt);
        //            }
        //            return true;
        //        }
        //        if (status == 0)
        //        {
        //            for (int i = 0; i <= Sselected.Length - 1; i = i + 2)
        //            {
        //                string sqlstmt = "update t_access set " + Sselected[i + 1] + "=0 where Id_access='" + Sselected[i] + "'";
        //                objGlobalData.ExecuteQuery(sqlstmt);
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunUpdateAccess: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunUpdateAccess(string selected, int status, string role_id)
        {
            try
            {
                string sql = ""; string sql1 = "";
                string[] Sselected = null;
                Sselected = selected.Split(',');
                int len1 = Sselected.Length;
                if (status == 1)
                {
                    for (int j = 0; j <= Sselected.Length - 1; j = j + 1)
                    {
                        switch (Sselected[j])
                        {
                            case "Events":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Events,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Events,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Documents":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Documents,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Documents,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "InternalDoc":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InternalDoc,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InternalDoc,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ExternalDoc":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExternalDoc,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExternalDoc,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Record":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Record,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Record,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "DocChangeReq":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DocChangeReq,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DocChangeReq,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "DocTrack":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DocTrack,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DocTrack,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ObjKPI":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ObjKPI,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ObjKPI,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Objectives":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Objectives,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Objectives,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Kpi":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Kpi,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Kpi,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "RiskMgmt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(RiskMgmt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(RiskMgmt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "ChangeMgmt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ChangeMgmt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ChangeMgmt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "ContextOrganise":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ContextOrganise,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ContextOrganise,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Parties":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Parties,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Parties,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Issues":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Issues,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Issues,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Risk":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Risk,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Risk,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "HazardRiskReg":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HazardRiskReg,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HazardRiskReg,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "HR":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HR,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HR,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Emp":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Emp,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Emp,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "EmpPerfEval":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpPerfEval,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpPerfEval,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Competancy":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Competancy,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Competancy,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "OrgChart":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(OrgChart,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(OrgChart,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ManHour":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ManHour,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ManHour,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ExitEmp":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExitEmp,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExitEmp,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Visitor":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Visitor,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Visitor,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "LeaveMgmt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveMgmt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveMgmt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "LeaveCredit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveCredit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveCredit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "AdjustLeave":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AdjustLeave,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AdjustLeave,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ApplyLeave":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ApplyLeave,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ApplyLeave,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "LeaveMaster":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveMaster,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LeaveMaster,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Holiday":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Holiday,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Holiday,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "ATSS":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ATSS,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ATSS,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ATS":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ATS,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ATS,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "HseATS":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseATS,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseATS,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Meeting":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Meeting,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Meeting,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "MAgenda":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MAgenda,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MAgenda,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "MSchedule":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MSchedule,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MSchedule,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "MUnplaned":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MUnplaned,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MUnplaned,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Suppliers":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Suppliers,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Suppliers,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Supplier":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Supplier,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Supplier,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "DLog":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DLog,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DLog,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "SupplierPer":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SupplierPer,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SupplierPer,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Providers":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Providers,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Providers,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "SupplierRate":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SupplierRate,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SupplierRate,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "CustMgmt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustMgmt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustMgmt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Visits":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Visits,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Visits,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "AddCust":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AddCust,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AddCust,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Complaints":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Complaints,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Complaints,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "SurveyQues":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SurveyQues,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SurveyQues,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "UploadSurvey":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(UploadSurvey,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(UploadSurvey,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "CustReturnProcuct":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustReturnProcuct,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustReturnProcuct,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Bidding":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Bidding,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Bidding,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "TrainingOri":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingOri,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingOri,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "AddTopic":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AddTopic,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AddTopic,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Perftraining":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Perftraining,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Perftraining,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "EmpTrainingOri":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpTrainingOri,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpTrainingOri,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Audit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Audit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Audit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "InterAudit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InterAudit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InterAudit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ExterAudit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExterAudit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExterAudit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ExtAuditRpt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExtAuditRpt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ExtAuditRpt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "CustAudit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustAudit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(CustAudit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "RaiseNc":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(RaiseNc,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(RaiseNc,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "InterChecklist":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InterChecklist,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(InterChecklist,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "AuditChecklist":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AuditChecklist,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AuditChecklist,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "HSE":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HSE,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HSE,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "IncidentRpt":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(IncidentRpt,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(IncidentRpt,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "NearMissRept":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(NearMissRept,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(NearMissRept,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "EmergPlan":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmergPlan,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmergPlan,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "ToolTalk":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ToolTalk,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ToolTalk,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "SafetyLog":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SafetyLog,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(SafetyLog,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "PpeLog":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(PpeLog,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(PpeLog,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "HseInsp":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseInsp,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseInsp,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "AirNoise":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AirNoise,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AirNoise,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Waste":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Waste,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Waste,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "ObservCard":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ObservCard,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ObservCard,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "HseIndu":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseIndu,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(HseIndu,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "FirstBox":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(FirstBox,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(FirstBox,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "FireInspection":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(FireInspection,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(FireInspection,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Project":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Project,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Project,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Equip":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Equip,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Equip,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Maintenance":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Maintenance,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Maintenance,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Calibration":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Calibration,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Calibration,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "LegalReg":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LegalReg,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(LegalReg,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Legal":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Legal,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Legal,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Certificates":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Certificates,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Certificates,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Training":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Training,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Training,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "TrainingList":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingList,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingList,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "TrainingCal":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingCal,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TrainingCal,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Report":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Report,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Report,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Rept":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Rept,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Rept,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "DashRept":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DashRept,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DashRept,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "MISRept":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MISRept,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(MISRept,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "Permits":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Permits,',2,3,4') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Permits,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "AccessPermit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AccessPermit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(AccessPermit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "WorkPermit":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(WorkPermit,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(WorkPermit,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Settings":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);

                                //if (j < Sselected.Length - 1)
                                //{
                                    if (Sselected[j + 1] == Sselected[j - 1])
                                    {
                                        sql1 = "update t_access set " + Sselected[j] + "=concat(Settings,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                        objGlobalData.ExecuteQuery(sql1);
                                    }
                                //}
                              
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Settings,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Company":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Company,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Company,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Dept":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Dept,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Dept,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "User":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(User,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(User,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;

                            case "DropDown":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DropDown,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(DropDown,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "EmpRole":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpRole,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(EmpRole,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ISOStd":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ISOStd,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ISOStd,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "TRA":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TRA,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(TRA,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "ResConsump":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ResConsump,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(ResConsump,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "NC":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(NC,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(NC,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "Portal":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Portal,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(Portal,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "sub_cr":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(sub_cr,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(sub_cr,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "access_portal":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(access_portal,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(access_portal,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                            case "portal_userlog":
                                sql = "update t_access set " + Sselected[j] + "=1 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                if (Sselected[j + 1] == Sselected[j - 1])
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(portal_userlog,',2,3,4,5') where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                for (int i = j + 2; (i < len1 && Sselected[i].Length == 1); i = i + 2)
                                {
                                    sql1 = "update t_access set " + Sselected[j] + "=concat(portal_userlog,','," + Sselected[i] + ") where Id_access='" + Sselected[j - 1] + "'";
                                    objGlobalData.ExecuteQuery(sql1);
                                }
                                break;
                        }

                    }
                    return objGlobalData.UpdateAccesdetails(role_id);
                    //return true;
                    
                }
                if (status == 0)
                {
                    for (int j = 0; j <= Sselected.Length - 1; j = j + 1)
                    {
                        switch (Sselected[j])
                        {
                            case "Events":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Documents":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "InternalDoc":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ExternalDoc":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Record":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "DocChangeReq":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "DocTrack":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ObjKPI":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Objectives":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Kpi":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "RiskMgmt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ChangeMgmt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break; 
                            case "ContextOrganise":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Parties":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Issues":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Risk":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HazardRiskReg":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HR":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Emp":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "EmpPerfEval":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Competancy":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "OrgChart":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ManHour":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ExitEmp":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Visitor":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "LeaveMgmt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "LeaveCredit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AdjustLeave":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ApplyLeave":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "LeaveMaster":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Holiday":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ATSS":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ATS":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HseATS":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Meeting":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "MAgenda":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "MSchedule":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "MUnplaned":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Suppliers":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Supplier":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "DLog":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "SupplierPer":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Providers":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "SupplierRate":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "CustMgmt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Visits":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AddCust":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Complaints":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "SurveyQues":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "UploadSurvey":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "CustReturnProcuct":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Bidding":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "TrainingOri":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AddTopic":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Perftraining":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "EmpTrainingOri":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Audit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "InterAudit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ExterAudit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ExtAuditRpt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "CustAudit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "RaiseNc":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "InterChecklist":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AuditChecklist":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HSE":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "IncidentRpt":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "NearMissRept":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "EmergPlan":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ToolTalk":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "SafetyLog":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "PpeLog":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HseInsp":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AirNoise":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Waste":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ObservCard":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "HseIndu":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "FirstBox":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "FireInspection":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Project":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Equip":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Maintenance":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Calibration":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "LegalReg":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Legal":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Certificates":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Training":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "TrainingList":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "TrainingCal":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Report":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Rept":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "DashRept":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "MISRept":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Permits":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "AccessPermit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "WorkPermit":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Settings":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Company":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Dept":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "User":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "DropDown":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "EmpRole":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ISOStd":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "TRA":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "ResConsump":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "NC":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "Portal":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "sub_cr":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "access_portal":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;
                            case "portal_userlog":
                                sql = "update t_access set " + Sselected[j] + "=0 where Id_access='" + Sselected[j - 1] + "'";
                                objGlobalData.ExecuteQuery(sql);
                                break;

                        }
                    }
                    return objGlobalData.UpdateAccesdetails(role_id);

                    //return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccess: " + ex.ToString());
            }
            return false;
        }
    }
    public class AccessModelsList
    {
        public List<AccessModels> AccessList { get; set; }
    }

}