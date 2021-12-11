using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class InternalAuditModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        public string AuditID { get; set; }
        public string AuditTransID { get; set; }

        [Display(Name = "AuditNum")]
        public string AuditNum { get; set; }

        [Display(Name = "Audit Prepared Date")]
        public DateTime AuditDate { get; set; }

        [Display(Name = "Audit Date Time")]
        public string AuditTime { get; set; }

        [Display(Name = "To Time")]
        public string AuditToTime { get; set; }

        //[Required]
        [Display(Name = "Audit Time only")]
        public DateTime AuditTimeOnly { get; set; }

        // [Required]
        [Display(Name = "Auditee")]
        public string Auditee { get; set; }

        // [Required]
        [Display(Name = "Department")]
        public string DeptName { get; set; }

        // [Required]
        [Display(Name = "Auditor")]
        public string Auditor { get; set; }

        //[Required]
        [Display(Name = "Audit Planned by")]
        public string Audit_Prepared_by { get; set; }

        //[Required]
        [Display(Name = "Audit Approved by")]
        public string Audit_Approved_by { get; set; }

        //[Required]
        [Display(Name = "Audit Planned Date")]
        public DateTime Audit_Planned_Date { get; set; }

        [Display(Name = "Audit Criteria")]
        public string AuditCriteria { get; set; }

        [Display(Name = "Audit Comments")]
        public string AuditComment { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyId { get; set; }

        [Required]
        [Display(Name = "Division")]
        public string AuditLocation { get; set; }

        [Display(Name = "Audit Status")]
        public string AuditStatus { get; set; }

        [Display(Name = "Scheduled Date")]
        public DateTime DateScheduled { get; set; }

        [Display(Name = "Completion Date")]
        public DateTime CompletionDate { get; set; }

        [Display(Name = "Audit Activity")]
        public string Audit_Activity { get; set; }

        [Display(Name = "Audit Reviewed By")]
        public string Audit_Reviewed_by { get; set; }

        [Display(Name = "Checklist")]
        public string Checklist { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Status")]
        public string ApprvStatus { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime ApprvDate { get; set; }

        [Display(Name = "Audit Details")]
        public string AuditDetails { get; set; }

        public bool checkAuditNoExists(string AuditNum)
        {
            string sSqlstmt = "select AuditNum from t_auditfindings where AuditNum='" + AuditNum + "'";
            DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
            if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            return true;
        }

        internal bool FunDeleteInternalAuditDoc(string sAuditID)
        {
            try
            {
                string sSqlstmt = "update t_internal_audit set Active=0 where AuditID='" + sAuditID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteInternalAuditDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddInternalAudit(InternalAuditModels objInternalAudit, InternalAuditModelsList objInternalAuditModelsList)
        {
            try
            {
                string sAuditDate = objInternalAudit.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_internal_audit ( AuditDate, AuditCriteria, AuditLocation,upload,Audit_Prepared_by,ApprovedBy) values('"
                     + sAuditDate + "','" + objInternalAudit.AuditCriteria + "','" + objInternalAudit.AuditLocation + "','" + objInternalAudit.upload + "'"
                + ",'" + objInternalAudit.Audit_Prepared_by + "','" + objInternalAudit.ApprovedBy + "')";

                int AuditID = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out AuditID))
                {
                    FunAddInternalAuditPlan(objInternalAuditModelsList, AuditID);
                    DataSet dsData = objGlobalData.GetReportNo("IA", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        AuditNum = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_internal_audit set AuditNum='" + AuditNum + "' where AuditID='" + AuditID + "'";

                    if (objGlobalData.ExecuteQuery(sql1))
                    {
                        SendAuditEmail(objInternalAudit);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInternalAudit: " + ex.ToString());
            }

            return false;
        }

        internal bool SendAuditEmail(InternalAuditModels objAudit)
        {
            try
            {
                string sInformation = "", sHeader = "";
                string sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objAudit.ApprovedBy);
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objAudit.Audit_Prepared_by);
                string sUserName = objGlobalData.GetMultiHrEmpNameById(objAudit.ApprovedBy);

                sHeader = "Internal Audit for your approval <br />";

                sInformation = "Audit NO:'" + objAudit.AuditNum + "'"
                + " <br />"
                + "Audit Criteria:'" + objGlobalData.GetIsoStdDescriptionById(objAudit.AuditCriteria) + "'"
                + " <br />"
                + "Audit date:'" + objAudit.AuditDate.ToString("dd/MM/yyyy") + "'"
                + " <br />"
               + "Audit Location:'" + objGlobalData.GetCompanyBranchNameById(objAudit.AuditLocation) + "'";
                sHeader = sHeader + sInformation;

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "audit", sHeader, "", "");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendAuditEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAuditApprvReject(string AuditID, int sStatus)
        {
            try
            {
                string sSqlstmt = "update t_internal_audit set ApprvStatus ='" + sStatus + "', ApprvDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where AuditID='" + AuditID + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendAuditApprvRejectEmail(AuditID, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAuditApprvReject: " + ex.ToString());
            }

            return false;
        }

        internal bool SendAuditApprvRejectEmail(string AuditID, int iStatus)
        {
            try
            {
                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

                string sSqlstmt = "select AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,Audit_Prepared_by,ApprovedBy from t_internal_audit where AuditID='" + AuditID + "'";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    if (iStatus == 1)//approved
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["Audit_Prepared_by"].ToString());
                        sCCList = objGlobalData.GetHrEmpEmailIdById(dsData.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        sUserName = "All";
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);//Emp Supervisor
                        sInformation = "Internal audit approved by '" + sUser + "'"
                            + " <br />"
                            + "Audit No:'" + (dsData.Tables[0].Rows[0]["AuditNum"].ToString()) + "'"
                            + " <br />"
                            + "Audit Criteria:'" + objGlobalData.GetIsoStdDescriptionById(dsData.Tables[0].Rows[0]["AuditCriteria"].ToString()) + "'"
                            + " <br />"
                            + "Audit date:'" + Convert.ToDateTime(dsData.Tables[0].Rows[0]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + "'"
                            + " <br />"
                            + "Audit Location:'" + objGlobalData.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["AuditLocation"].ToString()) + "'";
                    }
                    else if (iStatus == 2)//rescheduled
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["Audit_Prepared_by"].ToString());
                        sCCList = objGlobalData.GetHrEmpEmailIdById(dsData.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        sUserName = "All";
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);//Emp Supervisor
                        sInformation = "Internal audit Rescheduled by '" + sUser + "'"
                            + " <br />"
                            + "Audit No:'" + (dsData.Tables[0].Rows[0]["AuditNum"].ToString()) + "'"
                            + " <br />"
                            + "Audit Criteria:'" + objGlobalData.GetIsoStdDescriptionById(dsData.Tables[0].Rows[0]["AuditCriteria"].ToString()) + "'"
                            + " <br />"
                            + "Audit date:'" + Convert.ToDateTime(dsData.Tables[0].Rows[0]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + "'"
                            + " <br />"
                            + "Audit Location:'" + objGlobalData.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["AuditLocation"].ToString()) + "'";
                    }

                    sHeader = sHeader + sInformation;

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "audit", sHeader, "", "");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendAuditApprvRejectEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddInternalAuditPlan(InternalAuditModelsList objInternalAuditModelsList, int AuditID)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objInternalAuditModelsList.InternalAuditList.Count; i++)
                {
                    if (objInternalAuditModelsList.InternalAuditList[i].DeptName != null)
                    {
                        string sAudit_Prepared_Date = objInternalAuditModelsList.InternalAuditList[i].Audit_Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                        string sDateScheduled = objInternalAuditModelsList.InternalAuditList[i].DateScheduled.ToString("yyyy-MM-dd HH':'mm':'ss");

                        string sCompletionDate = "";
                        //if (objInternalAuditModelsList.InternalAuditList[i].AuditStatus.ToLower() == "closed")
                        //{
                        //    sCompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                        //}

                        sSqlstmt = sSqlstmt + "insert into t_internal_audit_trans (AuditID, DeptID, fromAuditTime, toAuditTime, Auditee, Auditor, Audit_Prepared_by, AuditStatus,ApprovedBy,Activity,"
                            + "Audit_Planned_Date, DateScheduled ";

                        if (sCompletionDate != "")
                        {
                            sSqlstmt = sSqlstmt + ", CompletionDate";
                        }

                        sSqlstmt = sSqlstmt + ") values('"
                            + AuditID + "','" + objInternalAuditModelsList.InternalAuditList[i].DeptName + "','"
                            + objInternalAuditModelsList.InternalAuditList[i].AuditTime + "','" + objInternalAuditModelsList.InternalAuditList[i].AuditToTime + "','"
                            + objInternalAuditModelsList.InternalAuditList[i].Auditee + "','" + objInternalAuditModelsList.InternalAuditList[i].Auditor + "','"
                            + objInternalAuditModelsList.InternalAuditList[i].Audit_Prepared_by + "','" + objInternalAuditModelsList.InternalAuditList[i].AuditStatus + "','"
                            + objInternalAuditModelsList.InternalAuditList[i].Audit_Approved_by + "','"
                            + objInternalAuditModelsList.InternalAuditList[i].Audit_Activity + "','"
                            + sAudit_Prepared_Date + "','" + sDateScheduled + "'";
                        if (sCompletionDate != "")
                        {
                            sSqlstmt = sSqlstmt + "'" + sCompletionDate + "'";
                        }
                        sSqlstmt = sSqlstmt + "); ";
                    }
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInternalAuditPlan: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateInternalAudit(InternalAuditModels objInternalAudit)
        {
            try
            {
                string sAuditDate = objInternalAudit.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;

                string sSqlstmt = "update t_internal_audit set AuditDate ='" + sAuditDate + "', AuditCriteria='" + objInternalAudit.AuditCriteria
                    + "', AuditLocation='" + objInternalAudit.AuditLocation + "', Audit_Prepared_by='" + objInternalAudit.Audit_Prepared_by + "', ApprovedBy='" + objInternalAudit.ApprovedBy + "'";

                //if (objInternalAudit.AuditStatus.ToLower() == "closed")
                //{
                //    sSqlstmt = sSqlstmt + ", CompletionDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                //}
                if (objInternalAudit.upload != null && objInternalAudit.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objInternalAudit.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where AuditId=" + objInternalAudit.AuditID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInternalAudit: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateInternalAuditPlan(InternalAuditModels objInternalAudit)
        {
            try
            {
                string sAudit_Prepared_Date = objInternalAudit.Audit_Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sDateScheduled = objInternalAudit.DateScheduled.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_internal_audit_trans set fromAuditTime='" + objInternalAudit.AuditTime + "', toAuditTime='" + objInternalAudit.AuditToTime + "', "
                    + "Auditee='" + objInternalAudit.Auditee + "', DeptID='" + objInternalAudit.DeptName + "', Auditor='" + objInternalAudit.Auditor
                    + "', Audit_Prepared_by='" + objInternalAudit.Audit_Prepared_by + "', Audit_Planned_Date='" + sAudit_Prepared_Date + "', AuditStatus='"
                    + objInternalAudit.AuditStatus + "', DateScheduled='" + sDateScheduled
                    + "',ApprovedBy = '" + objInternalAudit.Audit_Approved_by
                    + "',Activity = '" + objInternalAudit.Audit_Activity + "'";

                //if (objInternalAudit.AuditStatus.ToLower() == "closed")
                //{
                //    sSqlstmt = sSqlstmt + ", CompletionDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                //}
                sSqlstmt = sSqlstmt + " where AuditTransID=" + objInternalAudit.AuditTransID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInternalAuditPlan: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateAuditDetails(InternalAuditModels objInternalAudit)
        {
            try
            {
                string sSqlstmt = "update t_internal_audit_trans set AuditDetails='" + objInternalAudit.AuditDetails + "' where AuditTransID=" + objInternalAudit.AuditTransID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditDetails: " + ex.ToString());
            }

            return false;
        }

        internal List<string> FunGetInternalAuditList()
        {
            List<string> lstAuditNum = new List<string>();
            try
            {
                string sSqlstmt = "SELECT auditnum FROM "
                                  + "("
                                  + "SELECT MAX(auditid) AS Join_auditid"
                                     + " FROM t_internal_audit"
                                     + " GROUP BY auditnum"
                                  + ") as t1"
                                   + " INNER JOIN "
                                  + "t_internal_audit ON t1.Join_auditid = t_internal_audit.auditid";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                lstAuditNum = new List<string>();
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstAuditNum = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                    lstAuditNum.Insert(0, "Select");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetInternalAuditList: " + ex.ToString());
            }
            return lstAuditNum;
        }

        internal List<string> FunGetInternalAuditDateList(string sAuditNum)
        {
            List<string> lstAuditDate = new List<string>();
            try
            {
                string sSqlstmt = "SELECT distinct (DATE_FORMAT(AuditDate,'%d/%m/%Y') ) as AuditDate FROM t_internal_audit where AuditNum='" + sAuditNum + "'";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                lstAuditDate = new List<string>();
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstAuditDate = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                    lstAuditDate.Insert(0, "Select");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetInternalAuditDateList: " + ex.ToString());
            }
            return lstAuditDate;
        }

        internal List<string> FunGetInternalDeptforAduitNum(string sAuditNum, DateTime AuditDatetime)
        {
            List<string> lstAuditDeptName = new List<string>();
            try
            {
                string sAuditDate = AuditDatetime.ToString("yyyy-MM-dd");
                string sSqlstmt = "SELECT DeptName FROM t_internal_audit as TIA, t_departments as Dept where TIA.deptid=Dept.deptid and  AuditNum="
                    + sAuditNum + " and auditdate='" + sAuditDate + "'";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                lstAuditDeptName = new List<string>();
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstAuditDeptName = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetInternalDeptforAduitNum: " + ex.ToString());
            }
            //lstAuditDeptName.Insert(0, "Select");
            return lstAuditDeptName;
        }

        internal DataSet FunGetInternalAduitDetails(string sAuditNum, DateTime AuditDatetime, string sDeptName)
        {
            string sAuditDate = AuditDatetime.ToString("yyyy-MM-dd");

            string sSqlstmt = "SELECT auditor, tia.auditid, auditcriteria  FROM t_internal_audit as tia where auditnum=" + sAuditNum
                                + " and auditdate='" + sAuditDate + "' and TIA.deptid=(select deptid from t_departments where deptname='" + sDeptName + "')";

            return objGlobalData.Getdetails(sSqlstmt);
        }

        internal string GetAuditIDbyAuditNum(string sAuditNum)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select AuditID as Id from t_internal_audit where AuditNum = '" + sAuditNum + "' and Active = 1");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditIDbyAuditNum: " + ex.ToString());
            }
            return "";
        }
    }

    public class InternalAuditFindingsLog
    {
        private clsGlobal objGlobalData = new clsGlobal();

        ////[Required]
        [Display(Name = "ID")]
        public string AuditFindingID { get; set; }

        ////[Required]
        [Display(Name = "Audit Trans ID")]
        public string AuditTransID { get; set; }

        [Display(Name = "Audit Num.")]
        public string AuditNum { get; set; }

        [Display(Name = "Department")]
        public string DeptId { get; set; }

        [Display(Name = "Audit Conducted By")]
        public string AuditConductedBy { get; set; }

        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [Display(Name = "Findings Description")]
        public string FindingDescription { get; set; }

        [Display(Name = "NCR Num.")]
        public string NCRNo { get; set; }

        [Display(Name = "NCR Description/Observation")]
        public string NCRDesc { get; set; }

        [Display(Name = "ISO Standard")]
        public string ISOstandardID { get; set; }

        [Required(ErrorMessage = "ISO Standard Clause Cannot be kept Blank")]
        [Display(Name = "ISO Standard Clause")]
        [DataType(DataType.MultilineText)]
        public string ISO_standard_clause { get; set; }

        [Display(Name = "Finding Category")]
        public string FindingCategory { get; set; }

        [Display(Name = "CorrectionDate")]
        public DateTime CorrectionDate { get; set; }

        [Display(Name = "Report Status")]
        public string ReportStatus { get; set; }

        [Display(Name = "Report Close Date")]
        public DateTime ReportCloseDate { get; set; }

        [Display(Name = "Reviewed By")]
        public string Reviewed_by { get; set; }

        [Display(Name = "Corrective Action Date")]
        public DateTime CorrectiveActionDate { get; set; }

        [Display(Name = "Follow up date")]
        public DateTime Followupdate { get; set; }

        [Display(Name = "Correction taken, if any (To correct the detected non conformity)")]
        public string Correctiontaken { get; set; }

        [Display(Name = "Results of investigation to determine the root causes of the detected non conformity / potential non conformity")]
        public string Resultsofinvestigation { get; set; }

        [Display(Name = "Corrective Action (To prevent the reoccurrence of detected non conformity)")]
        public string CorrectiveAction { get; set; }

        [Display(Name = "Preventive Action (To prevent the occurrence of detected potential non conformity)")]
        public string PreventiveAction { get; set; }

        [Display(Name = "Verification of implementation of corrective action by Auditor:")]
        public string VerificationofImplementation { get; set; }

        public string Finding { get; set; }

        [Display(Name = "Checklist")]
        public string Checklist { get; set; }

        [Display(Name = "Finding Details")]
        public string Audit_Details { get; set; }

        internal bool FunAddInternalAuditFindingsLog(InternalAuditFindingsLogModelsList objInternalAudit, HttpPostedFileBase file)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objInternalAudit.InternalAuditFindingsLogList.Count; i++)
                {
                    string sCorrectionDate = objInternalAudit.InternalAuditFindingsLogList[i].CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    string sCorrectiveActionDate = objInternalAudit.InternalAuditFindingsLogList[i].CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    string sFollowupdate = objInternalAudit.InternalAuditFindingsLogList[i].Followupdate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    string sReportCloseDate = "";
                    //if (objInternalAudit.InternalAuditFindingsLogList[i].ReportStatus.ToLower() == "closed")
                    //{
                    //    sReportCloseDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                    //}

                    sSqlstmt = sSqlstmt + "insert into t_auditfindings ( AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                                        + "  Reviewed_by, FindingCategory, AuditNum, CorrectiveActionDate, Followupdate,Checklist,Audit_Details,ReportStatus";
                    if (sReportCloseDate != "")
                    {
                        sSqlstmt = sSqlstmt + ", ReportCloseDate";
                    }
                    sSqlstmt = sSqlstmt + ") values('"
                                         + objInternalAudit.InternalAuditFindingsLogList[i].AuditTransID + "','" + objInternalAudit.InternalAuditFindingsLogList[i].NCRNo
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].NCRDesc + "','" + objInternalAudit.InternalAuditFindingsLogList[i].ISOstandardID
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].ISO_standard_clause + "','" + sCorrectionDate
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].Reviewed_by
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].FindingCategory
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].AuditNum + "','" + sCorrectiveActionDate + "','" + sFollowupdate + "','" + objInternalAudit.InternalAuditFindingsLogList[i].Checklist
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].Audit_Details + "','" + objInternalAudit.InternalAuditFindingsLogList[i].ReportStatus + "'";

                    if (sReportCloseDate != "")
                    {
                        sSqlstmt = sSqlstmt + ", '" + sReportCloseDate + "'";
                    }
                    sSqlstmt = sSqlstmt + "); ";
                }
                if (sSqlstmt != "")
                {
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInternalAuditFindingsLog: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddInternalAuditNoFindingsLog(InternalAuditFindingsLogModelsList objInternalAudit)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objInternalAudit.InternalAuditFindingsLogList.Count; i++)
                {
                    //string sCorrectionDate = objInternalAudit.InternalAuditFindingsLogList[i].CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    //string sCorrectiveActionDate = objInternalAudit.InternalAuditFindingsLogList[i].CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    //string sFollowupdate = objInternalAudit.InternalAuditFindingsLogList[i].Followupdate.ToString("yyyy-MM-dd HH':'mm':'ss");
                    //string sReportCloseDate = "";
                    //if (objInternalAudit.InternalAuditFindingsLogList[i].ReportStatus.ToLower() == "closed")
                    //{
                    //    sReportCloseDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                    //}

                    sSqlstmt = sSqlstmt + "insert into t_auditfindings ( AuditTransID, NCRDesc, ISO_standard_ID, ISO_standard_clause,"
                                        + " Reviewed_by, FindingCategory, AuditNum,Checklist";
                    //if (sReportCloseDate != "")
                    //{
                    //    sSqlstmt = sSqlstmt + ", ReportCloseDate";
                    //}
                    sSqlstmt = sSqlstmt + ") values('"
                                         + objInternalAudit.InternalAuditFindingsLogList[i].AuditTransID
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].NCRDesc + "','" + objInternalAudit.InternalAuditFindingsLogList[i].ISOstandardID
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].ISO_standard_clause

                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].Reviewed_by
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].FindingCategory
                                         + "','" + objInternalAudit.InternalAuditFindingsLogList[i].AuditNum + "','" + objInternalAudit.InternalAuditFindingsLogList[i].Checklist + "'";

                    //if (sReportCloseDate != "")
                    //{
                    //    sSqlstmt = sSqlstmt + ", '" + sReportCloseDate + "'";
                    //}
                    sSqlstmt = sSqlstmt + "); ";
                }
                if (sSqlstmt != "")
                {
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInternalAuditFindingsLog: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateInternalAuditFindingsLog(InternalAuditFindingsLog objInternalAudit)
        {
            try
            {
                string sCorrectionDate = objInternalAudit.CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCorrectiveActionDate = objInternalAudit.CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sFollowupdate = objInternalAudit.Followupdate.ToString("yyyy-MM-dd HH':'mm':'ss");

                //  string sReportCloseDate = "";
                //if (objInternalAudit.ReportStatus.ToLower() == "closed")
                //{
                //    sReportCloseDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                //}

                string sSqlstmt = "update t_auditfindings set ISO_standard_ID ='" + objInternalAudit.ISOstandardID + "', NCR_Num='" + objInternalAudit.NCRNo + "', NCRDesc='"
                    + objInternalAudit.NCRDesc + "', ISO_standard_clause='" + objInternalAudit.ISO_standard_clause + "', CorrectionDate='" + sCorrectionDate + "',"
                  + " Reviewed_by='" + objInternalAudit.Reviewed_by
                  + "', FindingCategory='" + objInternalAudit.FindingCategory + "', CorrectiveActionDate='" + sCorrectiveActionDate + "', Followupdate='" + sFollowupdate
                  + "', Checklist='" + objInternalAudit.Checklist + "', ReportStatus='" + objInternalAudit.ReportStatus +
                   "', Audit_Details='" + objInternalAudit.Audit_Details + "' ";

                sSqlstmt = sSqlstmt + " where AuditFindingID='" + objInternalAudit.AuditFindingID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInternalAuditFindingsLog: " + ex.ToString());
            }

            //DateTime.Now("yyyy-MM-dd HH':'mm':'ss");
            return false;
        }

        internal bool FunUpdateInternalAuditFindingsLog1(InternalAuditFindingsLog objInternalAudit)
        {
            try
            {
                // string sCorrectionDate = objInternalAudit.CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCorrectiveActionDate = objInternalAudit.CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sFollowupdate = objInternalAudit.Followupdate.ToString("yyyy-MM-dd HH':'mm':'ss");

                //  string sReportCloseDate = "";
                //if (objInternalAudit.ReportStatus.ToLower() == "closed")
                //{
                //    sReportCloseDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                //}

                string sSqlstmt = "update t_auditfindings set CorrectiveActionDate='" + sCorrectiveActionDate + "', Followupdate='" + sFollowupdate
                  + "', Correctiontaken='" + objInternalAudit.Correctiontaken + "', Resultsofinvestigation='" + objInternalAudit.Resultsofinvestigation
                  + "', CorrectiveAction='" + objInternalAudit.CorrectiveAction + "', PreventiveAction='" + objInternalAudit.PreventiveAction
                  + "', VerificationofImplementation='" + objInternalAudit.VerificationofImplementation + "', Audit_Details='" + objInternalAudit.Audit_Details + "' ";

                //if (sReportCloseDate != "")
                //{
                //    sSqlstmt = sSqlstmt + ", ReportCloseDate='" + sReportCloseDate + "'";
                //}

                sSqlstmt = sSqlstmt + " where AuditFindingID='" + objInternalAudit.AuditFindingID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInternalAuditFindingsLog1: " + ex.ToString());
            }

            //DateTime.Now("yyyy-MM-dd HH':'mm':'ss");
            return false;
        }
    }

    public class InternalAuditModelsList
    {
        public List<InternalAuditModels> InternalAuditList { get; set; }
    }

    public class InternalAuditFindingsLogModelsList
    {
        public List<InternalAuditFindingsLog> InternalAuditFindingsLogList { get; set; }
    }

    public class InternalAuditChecklistModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string checklistId { get; set; }

        [Display(Name = "Internal Audit checklist Name")]
        public string checklistName { get; set; }

        // [Required]
        [Display(Name = "Doc Upload")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Audit No")]
        public string AuditNo { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Branch")]
        public string Branch { get; set; }

        internal bool FunAddChecklist(InternalAuditChecklistModels objChecklistModel)
        {
            try
            {
                //  string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "insert into t_internalauditchecklist (checklistName,DocUploadPath,AuditNo,Department) values" +
                    "('" + objChecklistModel.checklistName + "','" + objChecklistModel.DocUploadPath + "','" + objChecklistModel.AuditNo + "','" + objChecklistModel.Department + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteChecklist(string schecklistId)
        {
            try
            {
                string sSqlstmt = "update t_internalauditchecklist set Status=0 where checklistId='" + schecklistId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChecklist: " + ex.ToString());
            }
            return false;
        }
    }

    public class ChecklistModelsList
    {
        public List<InternalAuditChecklistModels> CheckList { get; set; }
    }

    public class DropdownAuditItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownAuditList
    {
        public List<DropdownAuditItems> lst { get; set; }
    }
}