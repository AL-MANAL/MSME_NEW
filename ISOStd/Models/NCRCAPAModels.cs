using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.Text.RegularExpressions;


namespace ISOStd.Models
{
    public class NCRLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        public int idt_NCR_Log { get; set; }

        [Required]
        [Display(Name = "NCR No.")]
        public string NCRNo { get; set; }

        [Required]
        [Display(Name = "NCR Log Date")]
        public DateTime Ncr_logDate { get; set; }

        [Required]
        [Display(Name = "NCR Date")]
        public DateTime NCRDate { get; set; }

        [Required]
        [Display(Name = "NCR Detail")]
        public string NCR_Details { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string NCR_Dept { get; set; }

        //[Required]
        [Display(Name = "Correction Date")]
        public DateTime Correction_Date { get; set; }

        //[Required]
        [Display(Name = "Corrective Action date")]
        public DateTime Corrective_Action_date { get; set; }

        //[Required]
        [Display(Name = "Follow up Date")]
        public DateTime FollowupDate { get; set; }

        [Required]
        [Display(Name = "Report Status")]
        public string ReportStatus { get; set; }

        //[Required]
        [Display(Name = "Report Close Date")]
        public DateTime Report_Close_Date { get; set; }

         [Required]
         [Display(Name = "Correction Details")]
        public string Correction_Details { get; set; }


        internal bool FunAddNCRLog(NCRLogModels objNCRLogModels)
        {
            try
            {
                string sDynamicColumn = "", sDynamicColumnVal = "";

                if (objNCRLogModels.Correction_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sDynamicColumn = sDynamicColumn + ", Correction_Date";
                    sDynamicColumnVal = sDynamicColumnVal + ", '" + objNCRLogModels.Correction_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objNCRLogModels.Corrective_Action_date > Convert.ToDateTime("01/01/0001"))
                {
                    sDynamicColumn = sDynamicColumn + ", Corrective_Action_date";
                    sDynamicColumnVal = sDynamicColumnVal + ", '" + objNCRLogModels.Corrective_Action_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objNCRLogModels.FollowupDate > Convert.ToDateTime("01/01/0001"))
                {
                    sDynamicColumn = sDynamicColumn + ", FollowupDate";
                    sDynamicColumnVal = sDynamicColumnVal + ", '" + objNCRLogModels.FollowupDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }


                if (objNCRLogModels.Ncr_logDate > Convert.ToDateTime("01/01/0001"))
                {
                    sDynamicColumn = sDynamicColumn + ", Ncr_logDate";
                    sDynamicColumnVal = sDynamicColumnVal + ", '" + objNCRLogModels.Ncr_logDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objNCRLogModels.ReportStatus.ToLower() == "closed")
                {
                    sDynamicColumn = sDynamicColumn + ", Report_Close_Date";
                    sDynamicColumnVal = sDynamicColumnVal + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }


                string sSqlstmt = "insert into t_NCR_Log (NCRNo, NCR_Details, NCR_Dept, Correction_Details" + sDynamicColumn + ") values('" + objNCRLogModels.NCRNo
                    + "','" + objNCRLogModels.NCR_Details + "', '" + objNCRLogModels.NCR_Dept + "','" + objNCRLogModels.Correction_Details
                    + "'" + sDynamicColumnVal + ")";
                //    + "'

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNCRLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateNCRLog(NCRLogModels objNCRLogModels)
        {
            try
            {
                string sNcr_logDate = objNCRLogModels.Ncr_logDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_NCR_Log set NCR_Details='" + objNCRLogModels.NCR_Details + "', ReportStatus='" + objNCRLogModels.ReportStatus + "', Correction_Details='"
                    + objNCRLogModels.Correction_Details + "' ";

                if (objNCRLogModels.Ncr_logDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + " , Ncr_logDate='" + (objNCRLogModels.Ncr_logDate).ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objNCRLogModels.Correction_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + " , Correction_Date='" + (objNCRLogModels.Correction_Date).ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objNCRLogModels.Corrective_Action_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + " , Corrective_Action_date='" + (objNCRLogModels.Corrective_Action_date).ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objNCRLogModels.FollowupDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + " , FollowupDate='" + (objNCRLogModels.FollowupDate).ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objNCRLogModels.ReportStatus.ToLower() == "closed")
                {
                    sSqlstmt = sSqlstmt + " , Report_Close_Date='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where idt_NCR_Log='" + objNCRLogModels.idt_NCR_Log + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNCRLog: " + ex.ToString());
            }
            return false;
        }
    }

    public class NCRCAPAModels
    {
        clsGlobal objGlobalData = new clsGlobal();

       
        //NCR RAISE
       
        public int CAR_ID { get; set; }

        [Display(Name = "NCR / PNCR number")]
        public string NC_Num { get; set; }

        [Display(Name = "Issued on")]
        public DateTime IssuedOn { get; set; }

        [Display(Name = "Issued by")]
        public string IssuedBy { get; set; }

        [Display(Name = "Issued by position")]
        public string IssuedByPosition { get; set; }

        [Display(Name = "Issued by dept")]
        public string IssuedByDept { get; set; }

        [Display(Name = "Issued to")]
        public string issuedTo { get; set; }

        [Display(Name = "Details of detected discrepancy")]
        [DataType(DataType.MultilineText)]
        public string Discrepancy_Details { get; set; }

        [Display(Name = "Discrepancy related to")]
        public string Discrepancy_Related { get; set; }

        [Display(Name = "Finding type")]
        public string FindingType { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Finding Identified")]
        public string FindingIdentified { get; set; }

        [Display(Name = "Audit No")]
        public string AuditNum { get; set; }

        public string Branch { get; set; }

        //NCR Action  
        public int id_ncr_action { get; set; }

        [Required]
        [Display(Name = "Correction taken to correct detected non conformance (discrepancy)")]
        public string NCR_CORRECTION_DESC { get; set; }

        [Required]
        [Display(Name = "Correction taken on")]
        public DateTime Correction_taken_on { get; set; }

        [Required]
        [Display(Name = "Correction taken by")]
        public string Correction_taken_by { get; set; }

        [Required]
        [Display(Name = "Root Cause Investigation result")]
        public string RootCause_Invtiresult { get; set; }

        [Required]
        [Display(Name = "Need of corrective action or preventive action?")]
        public string CAPA_Need { get; set; }

        [Required]
        [Display(Name = "Corrective or preventive action")]
        public string CAPA_DESC { get; set; }

        [Required]
        [Display(Name = "Corrective action implemented on")]
        public DateTime CA_Impl_On { get; set; }

        [Required]
        [Display(Name = "Corrective action proposed by")]
        public string CA_Proposed_By { get; set; }



        //NCR Close

        public int id_ncr_close { get; set; }

        [Display(Name = "Verification of effectiveness of corrective or preventive action")]
        public string CAPA_EFF_VERIFICATION { get; set; }

        [Display(Name = "Verified on")]
        public DateTime verified_on { get; set; }

        [Display(Name = "NCR Status")]
        public string NCR_Status { get; set; }

        [Display(Name = "Report closed on")]
        public DateTime Report_Closed_On { get; set; }

        [Display(Name = "Verified by name")]
        public string VerifiedBy { get; set; }

        [Display(Name = "Verified department name")]
        public string VerifiedDeptName { get; set; }

        [Display(Name = "Verified by position")]
        public string VerifiedByPosition { get; set; }

        internal bool FunDeleteNCRCloseDoc(string sid_ncr_close)
        {
            try
            {
                string sSqlstmt = "update t_ncr_close set Active=0 where id_ncr_close='" + sid_ncr_close + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNCRCAPADoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteNCRActionDoc(string sid_ncr_action)
        {
            try
            {
                string sSqlstmt = "update t_ncr_action set Active=0 where id_ncr_action='" + sid_ncr_action + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNCRCAPADoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteNCRCAPADoc(string sCAR_ID)
        {
            try
            {
                string sSqlstmt = "update t_ncr_raise set Active=0 where CAR_ID='" + sCAR_ID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNCRCAPADoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddNCRAction(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sSqlstmt = "insert into t_ncr_action (CAR_ID,NCR_CORRECTION_DESC,Correction_taken_by,RootCause_Invtiresult,"
                +"CAPA_Need,CAPA_DESC,CA_Proposed_By,upload";

                string sFields = "", sFieldValue = "";

                if (objNCRCAPA.Correction_taken_on != null && objNCRCAPA.Correction_taken_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Correction_taken_on";
                    sFieldValue = sFieldValue + ", '" + objNCRCAPA.Correction_taken_on.ToString("yyyy/MM/dd") + "'";
                }
                if (objNCRCAPA.CA_Impl_On != null && objNCRCAPA.CA_Impl_On > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", CA_Impl_On";
                    sFieldValue = sFieldValue + ", '" + objNCRCAPA.CA_Impl_On.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objNCRCAPA.CAR_ID + "','" + objNCRCAPA.NCR_CORRECTION_DESC + "','" + objNCRCAPA.Correction_taken_by + "'"
                    + ",'" + objNCRCAPA.RootCause_Invtiresult + "','" + objNCRCAPA.CAPA_Need + "','" + objNCRCAPA.CAPA_DESC + "','" + objNCRCAPA.CA_Proposed_By + "'"
                    + ",'" + objNCRCAPA.upload + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    SendNCRActionEmail(objNCRCAPA);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNCRAction: " + ex.ToString());
            }
            return false;
        }
        internal bool SendNCRActionEmail(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sql = "select NC_Num,IssuedBy from t_ncr_raise where CAR_ID='" + objNCRCAPA.CAR_ID + "'";
                DataSet dsNCR = objGlobalData.Getdetails(sql);
                if (dsNCR.Tables.Count > 0 && dsNCR.Tables[0].Rows.Count > 0)
                {
                    NC_Num = dsNCR.Tables[0].Rows[0]["NC_Num"].ToString();
                    IssuedBy = objGlobalData.GetMultiHrEmpEmailIdById(dsNCR.Tables[0].Rows[0]["IssuedBy"].ToString());
                    IssuedBy = Regex.Replace(IssuedBy, ",+", ",");
                    IssuedBy = IssuedBy.Trim();
                    IssuedBy = IssuedBy.TrimEnd(',');
                    IssuedBy = IssuedBy.TrimStart(',');
                }
                string sHeader = "";
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objNCRCAPA.Correction_taken_by);
                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');
                string sUserName = "All";
                string sToEmailId = IssuedBy;

                sHeader = "<tr><td ><b>NCR Number:<b></td> <td >"
                       + NC_Num + "</td></tr>"
                       + "<tr><td ><b>Correction Taken:<b></td> <td >" + objNCRCAPA.NCR_CORRECTION_DESC + "</td></tr>"
                       + "<tr><td ><b>Correction Taken On:<b></td> <td >" + objNCRCAPA.Correction_taken_on.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>Action Taken By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objNCRCAPA.Correction_taken_by) + "</td></tr>"
                       + "<tr><td ><b>Root Cause:<b></td> <td >" + objNCRCAPA.RootCause_Invtiresult + "</td></tr>"
                       + "<tr><td ><b>CAPA Need:<b></td> <td >" + objNCRCAPA.CAPA_Need + "</td></tr>"
                       + "<tr><td ><b>Corrective Action:<b></td> <td >" + objNCRCAPA.CAPA_DESC + "</td></tr>"
                       + "<tr><td ><b>Corrective Action Implemented On:<b></td> <td >" + objNCRCAPA.CA_Impl_On.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>Corrective Action Proposed By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objNCRCAPA.CA_Proposed_By) + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "NCRAction", sHeader, "", "NCR Action Taken");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendNCRActionEmail: " + ex.ToString());

            }
            return false;
        }
        internal bool FunAddNCRCAPA(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_ncr_raise (IssuedBy,issuedTo,Discrepancy_Details,"
                    + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum,branch";

                string sFields = "", sFieldValue = "";

                if (objNCRCAPA.IssuedOn != null && objNCRCAPA.IssuedOn > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", IssuedOn";
                    sFieldValue = sFieldValue + ", '" + objNCRCAPA.IssuedOn.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objNCRCAPA.IssuedBy + "',"
                    + "'" + objNCRCAPA.issuedTo + "','" + objNCRCAPA.Discrepancy_Details + "','" + objNCRCAPA.Discrepancy_Related + "'"
                    + ",'" + objNCRCAPA.FindingType + "','" + objNCRCAPA.upload + "','" + objNCRCAPA.FindingIdentified + "','" + objNCRCAPA.AuditNum + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int CAR_ID = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out CAR_ID))
                {
                    DataSet dsData = objGlobalData.GetReportNo("NC", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        NC_Num = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_ncr_raise set NC_Num='" + NC_Num + "' where CAR_ID='" + CAR_ID + "'";
                    if (objGlobalData.ExecuteQuery(sql1))
                    {
                        SendNCRRaiseEmail(objNCRCAPA);
                    }
                   
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNCRCAPA: " + ex.ToString());
            }
            return false;
        }
        internal bool SendNCRRaiseEmail(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sHeader = "";
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objNCRCAPA.IssuedBy);
                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');
                string sUserName = objGlobalData.GetMultiHrEmpNameById(objNCRCAPA.issuedTo);
                string sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objNCRCAPA.issuedTo);
                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');
                sHeader = "<tr><td ><b>NCR Number:<b></td> <td >"
                       + objNCRCAPA.NC_Num + "</td></tr>"
                       + "<tr><td ><b>Issued On:<b></td> <td >" + objNCRCAPA.IssuedOn.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>Issued By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objNCRCAPA.IssuedBy) + "</td></tr>"
                       + "<tr><td ><b>Issued To:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objNCRCAPA.issuedTo) + "</td></tr>"
                       + "<tr><td ><b>Discrepancy Details:<b></td> <td >" + objNCRCAPA.Discrepancy_Details + "</td></tr>"
                       + "<tr><td ><b>Discrepancy Related:<b></td> <td >" + objNCRCAPA.Discrepancy_Related + "</td></tr>"
                       + "<tr><td ><b>Finding Type:<b></td> <td >" + objNCRCAPA.FindingType + "</td></tr>"
                       + "<tr><td ><b>Finding Identified:<b></td> <td >" + objNCRCAPA.FindingIdentified + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "NCRRaise", sHeader, "", "NCR Raised");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendNCRRaiseEmail: " + ex.ToString());

            }
            return false;
        }
        internal bool FunAddNCRClose(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sSqlstmt = "insert into t_ncr_close (id_ncr_action,CAR_ID,CAPA_EFF_VERIFICATION,NCR_Status,VerifiedBy,VerifiedByPosition,VerifiedDeptName";

                string sFields = "", sFieldValue = "";

                if (objNCRCAPA.verified_on != null && objNCRCAPA.verified_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", verified_on";
                    sFieldValue = sFieldValue + ", '" + objNCRCAPA.verified_on.ToString("yyyy/MM/dd") + "'";
                }
                if (objNCRCAPA.Report_Closed_On != null && objNCRCAPA.Report_Closed_On > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Report_Closed_On";
                    sFieldValue = sFieldValue + ", '" + objNCRCAPA.Report_Closed_On.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objNCRCAPA.id_ncr_action + "','" + objNCRCAPA.CAR_ID + "','" + objNCRCAPA.CAPA_EFF_VERIFICATION + "','" + objNCRCAPA.NCR_Status + "','" + objNCRCAPA.VerifiedBy + "'"
                    + ",'" + objNCRCAPA.VerifiedByPosition + "','" + objNCRCAPA.VerifiedDeptName + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    SendNCRCloseEmail(objNCRCAPA);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNCRClose: " + ex.ToString());
            }
            return false;
        }
        internal bool SendNCRCloseEmail(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sql = "select NC_Num,IssuedBy from t_ncr_raise where CAR_ID='" + objNCRCAPA.CAR_ID + "'";
                DataSet dsNCR = objGlobalData.Getdetails(sql);
                if (dsNCR.Tables.Count > 0 && dsNCR.Tables[0].Rows.Count > 0)
                {
                    NC_Num = dsNCR.Tables[0].Rows[0]["NC_Num"].ToString();
                    IssuedBy = objGlobalData.GetMultiHrEmpEmailIdById(dsNCR.Tables[0].Rows[0]["IssuedBy"].ToString());
                    IssuedBy = Regex.Replace(IssuedBy, ",+", ",");
                    IssuedBy = IssuedBy.Trim();
                    IssuedBy = IssuedBy.TrimEnd(',');
                    IssuedBy = IssuedBy.TrimStart(',');
                }
                string sHeader = "";
                string sCCList = "";
                string Report_close = "";

                if (objNCRCAPA.Report_Closed_On > Convert.ToDateTime("01/01/0001"))
                {
                    
                    Report_close =  objNCRCAPA.Report_Closed_On.ToString("dd/MM/yyyy") + "' ";
                }

                string sUserName = "All";
                string sToEmailId = IssuedBy;

                sHeader = "<tr><td ><b>NCR Number:<b></td> <td >"
                       + NC_Num + "</td></tr>"
                       + "<tr><td ><b>Verification:<b></td> <td >" + objNCRCAPA.CAPA_EFF_VERIFICATION + "</td></tr>"
                       + "<tr><td ><b>Verified On:<b></td> <td >" + objNCRCAPA.verified_on.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>NCR Status:<b></td> <td >" + objNCRCAPA.NCR_Status + "</td></tr>"
                       + "<tr><td ><b>Closed On:<b></td> <td >" + Report_close + "</td></tr>"
                       + "<tr><td ><b>Verified By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objNCRCAPA.VerifiedBy) + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "NCRStatus", sHeader, "", "NCR Status");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendNCRCloseEmail: " + ex.ToString());

            }
            return false;
        }
        internal bool FunUpdateNCRCAPA(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sSqlstmt = "update t_ncr_raise set IssuedBy ='" + objNCRCAPA.IssuedBy + "'"
                    + ",issuedTo ='" + objNCRCAPA.issuedTo + "',Discrepancy_Details ='" + objNCRCAPA.Discrepancy_Details + "'"
                    + ",Discrepancy_Related ='" + objNCRCAPA.Discrepancy_Related + "',FindingType ='" + objNCRCAPA.FindingType + "'"
                    + ",upload ='" + objNCRCAPA.upload + "',FindingIdentified ='" + objNCRCAPA.FindingIdentified + "',AuditNum ='" + objNCRCAPA.AuditNum + "'";

                if (objNCRCAPA.IssuedOn != null && objNCRCAPA.IssuedOn > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", IssuedOn='" + objNCRCAPA.IssuedOn.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where CAR_ID='" + objNCRCAPA.CAR_ID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNCRCAPA: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateActionNCRCAPA(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sSqlstmt = "update t_ncr_action set NCR_CORRECTION_DESC ='" + objNCRCAPA.NCR_CORRECTION_DESC + "',Correction_taken_by ='" + objNCRCAPA.Correction_taken_by + "'"
                    + ",RootCause_Invtiresult ='" + objNCRCAPA.RootCause_Invtiresult + "',CAPA_Need ='" + objNCRCAPA.CAPA_Need + "'"
                    + ",CAPA_DESC ='" + objNCRCAPA.CAPA_DESC + "',CA_Proposed_By ='" + objNCRCAPA.CA_Proposed_By + "'"
                    + ",upload ='" + objNCRCAPA.upload + "'";

                if (objNCRCAPA.Correction_taken_on != null && objNCRCAPA.Correction_taken_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Correction_taken_on='" + objNCRCAPA.Correction_taken_on.ToString("yyyy/MM/dd") + "'";
                }
                if (objNCRCAPA.CA_Impl_On != null && objNCRCAPA.CA_Impl_On > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", CA_Impl_On='" + objNCRCAPA.CA_Impl_On.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_ncr_action='" + objNCRCAPA.id_ncr_action + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionNCRCAPA: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateNCRClose(NCRCAPAModels objNCRCAPA)
        {
            try
            {
                string sSqlstmt = "update t_ncr_close set CAPA_EFF_VERIFICATION ='" + objNCRCAPA.CAPA_EFF_VERIFICATION + "',NCR_Status ='" + objNCRCAPA.NCR_Status + "'"
                    + ",VerifiedBy ='" + objNCRCAPA.VerifiedBy + "',VerifiedByPosition ='" + objNCRCAPA.VerifiedByPosition + "'"
                    + ",VerifiedDeptName ='" + objNCRCAPA.VerifiedDeptName + "',upload ='" + objNCRCAPA.upload + "'";

                if (objNCRCAPA.verified_on != null && objNCRCAPA.verified_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", verified_on='" + objNCRCAPA.verified_on.ToString("yyyy/MM/dd") + "'";
                }
                if (objNCRCAPA.Report_Closed_On != null && objNCRCAPA.Report_Closed_On > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Report_Closed_On='" + objNCRCAPA.Report_Closed_On.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_ncr_close='" + objNCRCAPA.id_ncr_close + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNCRClose: " + ex.ToString());
            }
            return false;
        }
    }

    public class NCRCAPAModelsList
    {
        public List<NCRCAPAModels> NCRCAPAMList { get; set; }
    }

    public class NCRLogModelsList
    {
        public List<NCRLogModels> NCRLogMList { get; set; }
    }

    public class DiscrepancyRelatedList
    {
        public string Discrepancy { get; set; }
        public string Checked { get; set; }
    }
}