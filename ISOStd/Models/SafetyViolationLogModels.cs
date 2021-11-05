using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class SafetyViolationLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();


        [Display(Name = "ID")]
        public string ViolationLog_Id { get; set; }

        [Display(Name = "Violation Reported On")]
        public DateTime Reported_On { get; set; }

        [Display(Name = "violation Occurred On and Timing")]
        public DateTime UnasafeAct_OccurredOn { get; set; }

        [Display(Name = "violation Reported By")]
        public string UnsafeAct_ReportedBy { get; set; }

        [Display(Name = "Personnel Involved in the Unsafe Act")]
        public string UnsafeAct_Personnel { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Reason for violation?")]
        public string UnsafeAct_Why { get; set; }

        [Display(Name = "Report No.")]
        public string Report_No { get; set; }

        [Display(Name = "Document(s)")]
        public string Upload_Report { get; set; }
  
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Voilation Type")]
        public string VoilationType { get; set; }

        [Display(Name = "Action To be taken")]
        public string Action_taken { get; set; }

        [Display(Name = "HSE Observation")]
        public string HSE_observation { get; set; }

        [Display(Name = "Employee statement")]
        public string Emp_statement { get; set; }

        [Display(Name = "Department")]
        public string Dept { get; set; }

        [Display(Name = "Type of the Warning")]
        public string Violation_warning { get; set; }

        [Display(Name = "Supervisor")]
        public string Supervisor { get; set; }

        [Display(Name = "Other Supervisor")]
        public string Other_supervisor { get; set; }

        [Display(Name = "IssuedBy")]
        public string IssuedBy { get; set; }

        [Display(Name = "ApprovedBy")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Status")]
        public string Approved_Status { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Branch")]
        public string branch { get; set; }

        //public string ApproverCount { get; set; }        
        //public string ApprovalRejector { get; set; }
        //public string Approvers { get; set; }

        [Display(Name = "Details for voilation")]
        public string voilation_detail { get; set; }

        [Display(Name = "Issued To")]
        public string issued_to { get; set; }

        internal bool FunDeleteSafetyVoilationLogDoc(string sViolationLog_Id)
        {
            try
            {
                string sSqlstmt = "update t_safety_violationlog set Active=0 where ViolationLog_Id='" + sViolationLog_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSafetyVoilationLogDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddSafetyViolationLog(SafetyViolationLogModels objSafetyViolationLog, HttpPostedFileBase filepath)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                int appcount = 0;
                if (ApprovedBy != null && ApprovedBy != "")
                {
                    string[] name1 = objSafetyViolationLog.ApprovedBy.Split(',');
                    appcount = name1.Length;
                }
              

                string sSqlstmt = "insert into t_safety_violationlog (UnsafeAct_ReportedBy, UnsafeAct_Personnel, UnsafeAct_Why, LoggedBy,VoilationType"
                    + ",Action_taken,HSE_observation,Emp_statement,Violation_warning,Dept,Supervisor,branch,IssuedBy,Other_supervisor,ApprovedBy,ApproverCount,Location,voilation_detail,issued_to";

                if (ApprovedBy == "" || ApprovedBy == null)
                {
                    sColumn = sColumn + ", Approved_Status";
                    sValues = sValues + ", '2'";
                }
                
                if (objSafetyViolationLog.Reported_On > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Reported_On";
                    sValues = sValues + ", '" + objSafetyViolationLog.Reported_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objSafetyViolationLog.UnasafeAct_OccurredOn > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", UnasafeAct_OccurredOn";
                    sValues = sValues + ", '" + objSafetyViolationLog.UnasafeAct_OccurredOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objSafetyViolationLog.Upload_Report != null && objSafetyViolationLog.Upload_Report != "")
                {
                    sColumn = sColumn + ", Upload_Report";
                    sValues = sValues + ", '" + objSafetyViolationLog.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objSafetyViolationLog.UnsafeAct_ReportedBy + "','" + objSafetyViolationLog.UnsafeAct_Personnel
                    + "','" + objSafetyViolationLog.UnsafeAct_Why 
                 + "','" + user + "','" + objSafetyViolationLog.VoilationType + "','" + objSafetyViolationLog.Action_taken + "'"
                 + ",'" + objSafetyViolationLog.HSE_observation + "','" + objSafetyViolationLog.Emp_statement+ "','" + objSafetyViolationLog.Violation_warning 
                 + "','" + objSafetyViolationLog.Dept + "','" + objSafetyViolationLog.Supervisor + "','" + objSafetyViolationLog.branch + "','" + objSafetyViolationLog.IssuedBy 
                 + "','" + objSafetyViolationLog.Other_supervisor + "','" + objSafetyViolationLog.ApprovedBy + "','" + appcount + "','" + objSafetyViolationLog.Location + "','" + objSafetyViolationLog.voilation_detail + "','" + objSafetyViolationLog.issued_to + "'";

                sSqlstmt = sSqlstmt + sValues + ")";


                int ViolationLog_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out ViolationLog_Id))
                 {
                     DataSet dsData = objGlobalData.GetReportNo("SVR", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                     if (dsData != null && dsData.Tables.Count > 0)
                     {
                         Report_No = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                     }
                     string sql1 = "update t_safety_violationlog set Report_No='" + Report_No + "' where ViolationLog_Id='" + ViolationLog_Id + "'";

                    if (objGlobalData.ExecuteQuery(sql1))
                    {
                        string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objSafetyViolationLog.ApprovedBy);
                        if (sEmailid != null && sEmailid != "")
                        {
                            string sExtraMsg = "Attached is the document Pending for your Approval, with Report_No: "+ objSafetyViolationLog .Report_No+ " Reported by: " + objGlobalData.GetMultiHrEmpNameById(objSafetyViolationLog.UnsafeAct_ReportedBy) + "," + " Issued by:" + objGlobalData.GetMultiHrEmpNameById(objSafetyViolationLog.IssuedBy);

                            //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetEmpHrNameById(objSafetyViolationLog.ApprovedBy), "SafetyViolation", sExtraMsg);

                            // sEmailCCList = sEmailCCList.Trim(',');
                            sEmailid = sEmailid.Trim(',');
                            //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                            return objGlobalData.SendmailNew(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], filepath, "", "");
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSafetyViolationLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSafetyViolationLog(SafetyViolationLogModels objSafetyViolationLog)
        {
            try
            {
                string sSqlstmt = " update t_safety_violationlog set UnsafeAct_ReportedBy='" + objSafetyViolationLog.UnsafeAct_ReportedBy
                    + "', UnsafeAct_Personnel='" + objSafetyViolationLog.UnsafeAct_Personnel
                    + "', UnsafeAct_Why='" + objSafetyViolationLog.UnsafeAct_Why + "', VoilationType='" + objSafetyViolationLog.VoilationType + "'"
                    + ", Action_taken='" + objSafetyViolationLog.Action_taken + "', HSE_observation='" + objSafetyViolationLog.HSE_observation + "', Emp_statement='" + objSafetyViolationLog.Emp_statement + "'"
                    + ", Violation_warning='" + objSafetyViolationLog.Violation_warning + "', Dept='" + objSafetyViolationLog.Dept+ "', Supervisor='" + objSafetyViolationLog.Supervisor
                    + "', IssuedBy='" + objSafetyViolationLog.IssuedBy + "', Other_supervisor='" + objSafetyViolationLog.Other_supervisor + "', ApprovedBy='" + objSafetyViolationLog.ApprovedBy
                    + "', branch='" + objSafetyViolationLog.branch + "', Location='" + objSafetyViolationLog.Location + "'";

                if (objSafetyViolationLog.Reported_On > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Reported_On='" + objSafetyViolationLog.Reported_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objSafetyViolationLog.UnasafeAct_OccurredOn > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", UnasafeAct_OccurredOn='" + objSafetyViolationLog.UnasafeAct_OccurredOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objSafetyViolationLog.Upload_Report != null && objSafetyViolationLog.Upload_Report != "")
                {
                    sSqlstmt = sSqlstmt + ", Upload_Report='" + objSafetyViolationLog.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + " where ViolationLog_Id='" + objSafetyViolationLog.ViolationLog_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSafetyViolationLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunSViolationApproveOrReject(string sViolationLog_Id, int sStatus, System.IO.Stream fsSource, string filename)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                if (sStatus == 1)
                {

                    string sSqlstmt1 = "update t_safety_violationlog set ApproverCount=ApproverCount-1 where ViolationLog_Id='" + sViolationLog_Id + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount from t_safety_violationlog where ViolationLog_Id='" + sViolationLog_Id + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_safety_violationlog set Approved_Status ='1', ApprovedDate='" + sApprovedDate + "' where ViolationLog_Id='" + sViolationLog_Id + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_safety_violationlog set Approvers= concat(Approvers,',','" + user + "') where ViolationLog_Id='" + sViolationLog_Id + "'";
                                    objGlobalData.ExecuteQuery(Sql1);
                                    SendViolationApprovalmail(sViolationLog_Id, fsSource, filename, "Safety ViolationLog is Approved");
                                    return true;
                                }
                            }
                            else
                            {
                                string Sql1 = "update t_safety_violationlog set Approvers= concat(Approvers,',','" + user + "')"
                                + " where ViolationLog_Id='" + sViolationLog_Id + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
                else
                {
                    string Sql1 = "update t_safety_violationlog set Approved_Status='" + sStatus + "',ApprovalRejector= concat(ApprovalRejector,',','" + user + "') where ViolationLog_Id='" + sViolationLog_Id + "'";
                    objGlobalData.ExecuteQuery(Sql1);
                    SendViolationRejectionmail(sViolationLog_Id, fsSource, filename, "Safety ViolationLog Rejected");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunSViolationApproveOrReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendViolationApprovalmail(string sViolationLog_Id, System.IO.Stream fsSource, string filename, string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select Report_No,HSE_observation, IssuedBy, UnsafeAct_ReportedBy, ApprovedBy,Supervisor from t_safety_violationlog where ViolationLog_Id='" + sViolationLog_Id + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["IssuedBy"].ToString())+","+ objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UnsafeAct_ReportedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Safety Violation Log has been approved, Report No: " + dsDocument.Tables[0].Rows[0]["Report_No"].ToString()+ " and HSE observation: " + dsDocument.Tables[0].Rows[0]["HSE_observation"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) ;
                       
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["Supervisor"].ToString()), "SafetyViolation1", sExtraMsg);

                    return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendViolationApprovalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool SendViolationRejectionmail(string sViolationLog_Id, System.IO.Stream fsSource, string filename, string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select HSE_observation,Supervisor, IssuedBy, UnsafeAct_ReportedBy, ApprovedBy,Report_No from t_safety_violationlog where ViolationLog_Id='" + sViolationLog_Id + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["IssuedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UnsafeAct_ReportedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Safety ViolationLog has been Rejected, Report No: " + dsDocument.Tables[0].Rows[0]["Report_No"].ToString() + " and HSE observation: " + dsDocument.Tables[0].Rows[0]["HSE_observation"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) ;
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["Supervisor"].ToString()), "SafetyViolation2", sExtraMsg);

                    return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendViolationRejectionmail: " + ex.ToString());
            }
            return false;
        }

    }

    public class SafetyViolationLogModelsList
    {
        public List<SafetyViolationLogModels> lstSafetyViolationLog { get; set; }
    }
}