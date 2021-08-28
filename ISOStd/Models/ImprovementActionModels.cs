using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;

namespace ISOStd.Models
{
    public class ImprovementActionModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_action { get; set; }

        [Display(Name = "CA No")]
        public string ca_no { get; set; }

        [Display(Name = "Date")]
        public DateTime actoin_date { get; set; }

        [Display(Name = "Complaint / Adverse Remarks / Non-conformance ")]
        public string complaint { get; set; }

        [Display(Name = "Description")]
        public string complaint_desc { get; set; }

        [Display(Name = "Reference")]
        public string reference { get; set; }

        [Display(Name = "Non Conference / Complaint Details:")]
        public string non_conf { get; set; }

        [Display(Name = "Received / Recorded By")]
        public string receivedby { get; set; }

        [Display(Name = "Risk due to complaint  / Consequenes (Brief description) ")]
        public string risk_complaint { get; set; }

        [Display(Name = "Serverity of Risk")]
        public string severity { get; set; }

        [Display(Name = "Disposition / Remedial Action")]
        public string disposition { get; set; }

        [Display(Name = "Quality Co-ordinator")]
        public string qty_coordinator { get; set; } 

        [Display(Name = "Causes & Threats")]
        public string causes { get; set; }

        [Display(Name = "Analysis / Investigation to find root-cause")]
        public string analysis { get; set; }             

        [Display(Name = "Quality Co-ordinator")]
        public string qty_coordinator1 { get; set; }

        [Display(Name = "Corrective Action")]
        public string corrective_action { get; set; }

        [Display(Name = "Quality Co-ordinator")]
        public string qty_coordinator2 { get; set; }

        [Display(Name = "The resultant consequences")]
        public string consequence { get; set; }

        [Display(Name = "Corrective/Preventive  Action Completion Date")]
        public DateTime completion_date { get; set; }

        [Display(Name = "Monitoring Period")]
        public string monitoring_period { get; set; }

        [Display(Name = "")]
        public string monitoring_value { get; set; }

        [Display(Name = "On Completion of Action")]
        public string compeltion_action { get; set; }

        [Display(Name = "Assesed Risk Level ")]
        public string assesed_risk { get; set; }

        [Display(Name = "Closed Out Date")]
        public DateTime closedout_date { get; set; }

        [Display(Name = "Verification By")]
        public string verifiedby { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunAddImprovement(ImprovementActionModels objModel)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_improvement_action(complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition,qty_coordinator," +
                    "analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,monitoring_period,monitoring_value,compeltion_action,assesed_risk,verifiedby,branch,Department,Location";
                string sFields = "", sFieldValue = "";

                if (objModel.actoin_date != null && objModel.actoin_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", actoin_date";
                    sFieldValue = sFieldValue + ", '" + objModel.actoin_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.completion_date != null && objModel.completion_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", completion_date";
                    sFieldValue = sFieldValue + ", '" + objModel.completion_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.closedout_date != null && objModel.closedout_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", closedout_date";
                    sFieldValue = sFieldValue + ", '" + objModel.closedout_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.complaint +  "','" + objModel.complaint_desc + "','" + objModel.reference + "','" + objModel.non_conf + "','" + objModel.receivedby +
                    "','" + objModel.risk_complaint + "','" + objModel.severity + "','" + objModel.disposition + "','" + objModel.qty_coordinator
                    + "', '" + objModel.analysis + "', '" + objModel.qty_coordinator1 + "', '" + objModel.corrective_action + "', '" + objModel.qty_coordinator2
                    + "', '" + objModel.consequence + "', '" + objModel.monitoring_period + "', '" + objModel.monitoring_value + "', '" + objModel.compeltion_action 
                    + "', '" + objModel.assesed_risk + "', '" + objModel.verifiedby + "', '" + objModel.branch + "', '" + objModel.Department + "', '" + objModel.Location + "'"; 

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_action = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_action))
                {
                    DataSet dsData = objGlobalData.GetReportNo("CA", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        ca_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_improvement_action set ca_no='" + ca_no + "' where id_action='" + id_action + "'";
                    return objGlobalData.ExecuteQuery(sql1);
                    
                }
             
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddImprovement: " + ex.ToString());
            }
            return false;
        }

       internal bool FunUpdateImplementation( ImprovementActionModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_improvement_action set complaint='" + objModel.complaint + "',complaint_desc='" + objModel.complaint_desc + "',reference='" + objModel.reference + "',non_conf='" + objModel.non_conf + "'"
                    + ",receivedby='" + objModel.receivedby + "',risk_complaint='" + objModel.risk_complaint + "',severity='" + objModel.severity + "',disposition='" + objModel.disposition + "',qty_coordinator='" + objModel.qty_coordinator
                    + "',analysis='" + objModel.analysis + "',qty_coordinator1='" + objModel.qty_coordinator1 + "',corrective_action='" + objModel.corrective_action + "',qty_coordinator2='" + objModel.qty_coordinator2 + "',consequence='" + objModel.consequence
                    + "',monitoring_period='" + objModel.monitoring_period + "',monitoring_value='" + objModel.monitoring_value
                    + "',compeltion_action='" + objModel.compeltion_action + "',assesed_risk='" + objModel.assesed_risk + "',verifiedby='" + objModel.verifiedby
                    + "',Department='" + objModel.Department + "',Location='" + objModel.Location + "',branch='" + objModel.branch + "'";

                if (objModel.actoin_date != null && objModel.actoin_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",actoin_date='" + objModel.actoin_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModel.completion_date != null && objModel.completion_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", completion_date='" + objModel.completion_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.closedout_date != null && objModel.closedout_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt+ ", closedout_date='" + objModel.closedout_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_action='" + objModel.id_action + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateImplementation: " + ex.ToString());
            }

            return false;
        }

       internal bool FunDeleteImplementationDoc(string sid_action)
        {
            try
            {
                string sSqlstmt = "update t_improvement_action set Active=0 where id_action='" + sid_action + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteImplementationDoc: " + ex.ToString());
            }
            return false;
        }
    }

    public class ImprovementList
    {
        public List< ImprovementActionModels> ActionList { get; set; }
    }
}