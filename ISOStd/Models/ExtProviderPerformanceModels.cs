using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class ExtProviderPerformanceModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_extprovider_performance
        [Display(Name = "Id")]
        public string Id_Performace { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime Eval_Date { get; set; }

        [Display(Name = "Evaluation From Date")]
        public DateTime Eval_FromDate { get; set; }

        [Display(Name = "Evaluation To Date")]
        public DateTime Eval_ToDate { get; set; }

        [Display(Name = "External Provider Name")]
        public string Ext_Provider_Name { get; set; }

        [Display(Name = "Scope of Supply or Service")]
        public string Scope_ofSupplies { get; set; }

        [Display(Name = "Report Number")]
        public string ReportNo { get; set; }

        [Display(Name = "Total number of POs issues")]
        public decimal PO_Issued { get; set; }

        [Display(Name = "Total number of POs completed")]
        public decimal PO_Completed { get; set; }

        [Display(Name = "Total no of quality issues(TQI)")]
        public decimal Quality_Issue { get; set; }

        [Display(Name = "Total no of delivery issues(TDI)")]
        public decimal Delivery_Issue { get; set; }

        [Display(Name = "Evaluated By")]
        public string Scheduled_by { get; set; }

        [Display(Name = "To be reviewed & verified by")]
        public string Approved_by { get; set; }

        //Display
        [Display(Name = "Quality rating")]
        public decimal QualityRate { get; set; }

        [Display(Name = "Delivery rating")]
        public decimal DeliveryRate { get; set; }

        [Display(Name = "External Provider Performance Rating")]
        public decimal Perfp_rate { get; set; }

        [Display(Name = "Actions based on performance rating")]
        public string Perfp_rate_action { get; set; }

        //t_extprovider_performance_trans

        public string id_Performance_trans { get; set; }

        [Display(Name = "Actions")]
        public string Actions { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string Personnel_Resp { get; set; }

        [Display(Name = "Due Date")]
        public DateTime Due_Date { get; set; }

        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        //t_extprovider_performance

        [Display(Name = "Total no of quantity issues(TQNI)")]
        public decimal quantity_issue { get; set; }

        [Display(Name = "Total number of LOTs received")]
        public decimal lots_received { get; set; }

        [Display(Name = "Quality Rating")]
        public decimal quality_rating { get; set; }

        [Display(Name = "Quantity Rating")]
        public decimal quantity_rating { get; set; }

        [Display(Name = "Delivery Rating")]
        public decimal delivery_rating { get; set; }

        [Display(Name = "Total Supplier Rating")]
        public decimal total_rating { get; set; }

        [Display(Name = "Document(s)")]
        public string upload { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        [Display(Name = "Action Initiated on")]
        public DateTime initiated_date { get; set; }

        [Display(Name = "Action to be taken")]
        public string action_taken { get; set; }

        
        [Display(Name = "Action to be taken by")]
        public string action_by { get; set; }

        [Display(Name = "Notified To")]
        public string action_notified_to { get; set; }

        [Display(Name = "PO Details")]
        public string po_detail { get; set; }

        [Display(Name = "Approval Status")]
        public string apprv_status { get; set; }

        [Display(Name = "Approval Date")]
        public DateTime approved_date { get; set; }

        [Display(Name = "Comments")]
        public string apprv_comments { get; set; }

        public string apprv_status_id { get; set; }

        [Display(Name = "Document(s)")]
        public string approver_upload { get; set; }

        internal bool FunDeleteExtProviderPerf(string sId_Performace)
        {
            try
            {
                string sSqlstmt = "update t_extprovider_performance set Active=0 where Id_Performace='" + sId_Performace + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteExtProviderPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExtProviderPerf(ExtProviderPerformanceModels ObjPerfModels, ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_extprovider_performance (Ext_Provider_Name, Scope_ofSupplies, PO_Issued,"
                    + " PO_Completed, Scheduled_by,Approved_by,branch,Department,Location,quantity_issue,lots_received,quality_rating,quantity_rating,delivery_rating,total_rating,upload,notified_to,Quality_Issue,Delivery_Issue,po_detail";
                string sFields = "", sFieldValue = "";

                if (ObjPerfModels.Eval_Date != null && ObjPerfModels.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_Date";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_Date.ToString("yyyy/MM/dd") + "'";
                }

                if (ObjPerfModels.Eval_FromDate != null && ObjPerfModels.Eval_FromDate > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_FromDate";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }

                if (ObjPerfModels.Eval_ToDate != null && ObjPerfModels.Eval_ToDate > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_ToDate";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_ToDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")  values('" + ObjPerfModels.Ext_Provider_Name + "','" + ObjPerfModels.Scope_ofSupplies
                //+ "','" + ObjPerfModels.PO_Issued + "','" + ObjPerfModels.PO_Completed + "','" + ObjPerfModels.Quality_Issue
                //+ "','" + ObjPerfModels.Delivery_Issue + "','" + ObjPerfModels.Scheduled_by + "','" + ObjPerfModels.Approved_by + "','" + sBranch + "'";
                + "','" + ObjPerfModels.PO_Issued + "','" + ObjPerfModels.PO_Completed 
                + "','" + ObjPerfModels.Scheduled_by + "','" + ObjPerfModels.Approved_by + "','" + ObjPerfModels.branch
                + "','" + ObjPerfModels.Department + "','" + ObjPerfModels.Location + "'"
                + ",'" + ObjPerfModels.quantity_issue + "','" + ObjPerfModels.lots_received + "','" + ObjPerfModels.quality_rating + "','" + ObjPerfModels.quantity_rating + "','" + ObjPerfModels.delivery_rating + "','" + ObjPerfModels.total_rating + "','" + ObjPerfModels.upload + "','" + ObjPerfModels.notified_to + "','" + ObjPerfModels.Quality_Issue + "','" + ObjPerfModels.Delivery_Issue + "','" + ObjPerfModels.po_detail + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int Id_Performace = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id_Performace))
                {
                    if (Id_Performace > 0 && Convert.ToInt32(objPerfList.ExtPerfpList.Count) > 0)
                    {
                        objPerfList.ExtPerfpList[0].Id_Performace = Id_Performace.ToString();
                        FunAddExtProviderPerfTrans(objPerfList);
                    }

                    if (Id_Performace > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("EXTPRO", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ReportNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_extprovider_performance set ReportNo='" + ReportNo + "' where Id_Performace = '" + Id_Performace + "'";
                        if (objGlobalData.ExecuteQuery(sql))
                        {
                          return SendPerformanceEvaluationmail(Id_Performace, "External Provider Performance Evaluation Review");
                        }    
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderPerf: " + ex.ToString());
            }
            return false;
        }
        internal bool SendPerformanceEvaluationmail(int Id_Performace, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select ReportNo,Eval_Date,Ext_Provider_Name,Scope_ofSupplies,po_detail,Eval_FromDate,"
                +"Eval_ToDate,lots_received,Quality_Issue,quantity_issue,Delivery_Issue,quality_rating,quantity_rating,delivery_rating,total_rating,Scheduled_by,Approved_by,notified_to"
                + " from t_extprovider_performance where Id_Performace='" + Id_Performace + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sSubject = "", sContent = "", aAttachment = "";
                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Approved_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Scheduled_by"].ToString())+","+ objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notified_to"].ToString());

                    string Eval_Date = "", Eval_FromDate="", Eval_ToDate="";
                    if (dsList.Tables[0].Rows[0]["Eval_Date"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_Date"].ToString() != "")
                    {
                        Eval_Date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_Date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    if (dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString() != "")
                    {
                        Eval_FromDate = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                    if (dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString() != "")
                    {
                        Eval_ToDate = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                    sHeader = "<tr><td colspan=3><b>Report No:<b></td> <td colspan=3>"
                        + (dsList.Tables[0].Rows[0]["ReportNo"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>External Provider Name:<b></td> <td colspan=3>" + objGlobalData.GetSupplierNameById(dsList.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()) + "</td></tr>"

                            + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + Eval_Date + "</td></tr>"

                    + "<tr><td colspan=3><b>Scope of Supply or Service:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Scope_ofSupplies"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>PO Details:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["po_detail"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation From Date:< b></td> <td colspan=3>" + Eval_FromDate + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation To Date:<b></td> <td colspan=3>" + Eval_ToDate + "</td></tr>"

                    + "<tr><td colspan=3><b>Total number of LOTs received:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["lots_received"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of quality issues(TQI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Quality_Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of quantity issues(TQNI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quantity_issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of delivery issues(TDI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Delivery_Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Quality Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quality_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Quantity Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quantity_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Delivery Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["delivery_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total Supplier Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["total_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Scheduled_by"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>To be reviewed & verified by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Approved_by"].ToString()) + "</td></tr>";

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "External Provider Performance Evaluation");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendPerformanceEvaluationmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunReviewPerfEvaluation(ExtProviderPerformanceModels objModel)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "update t_extprovider_performance set apprv_status='" + apprv_status + "',apprv_comments='" + apprv_comments + "',approved_date='" + sApprovedDate + "',approver_upload='" + approver_upload + "' where Id_Performace='" + Id_Performace + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return SendPerformanceEvaluationReviewmail(Id_Performace, apprv_status, "External Performance Evaluation Review Status");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunReviewPerfEvaluation: " + ex.ToString());
            }
            return false;
        }
        internal bool SendPerformanceEvaluationReviewmail(string Id_Performace, string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select ReportNo,Eval_Date,Ext_Provider_Name,Scope_ofSupplies,po_detail,Eval_FromDate,"
            + "Eval_ToDate,lots_received,Quality_Issue,quantity_issue,Delivery_Issue,quality_rating,quantity_rating,delivery_rating,total_rating,Scheduled_by,Approved_by,notified_to,"
                 + "(CASE WHEN apprv_status = '0' THEN 'Pending for review' WHEN apprv_status = '1' THEN 'Rejected' WHEN apprv_status = '2' THEN 'Reviewed' end) as apprv_status,apprv_comments"
                     + " from t_extprovider_performance where Id_Performace='" + Id_Performace + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Scheduled_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Scheduled_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Approved_by"].ToString()) +","+ objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notified_to"].ToString());



                    string Eval_Date = "", Eval_FromDate = "", Eval_ToDate = "";
                    if (dsList.Tables[0].Rows[0]["Eval_Date"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_Date"].ToString() != "")
                    {
                        Eval_Date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_Date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    if (dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString() != "")
                    {
                        Eval_FromDate = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_FromDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                    if (dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString() != null && dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString() != "")
                    {
                        Eval_ToDate = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Eval_ToDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                    sHeader = "<tr><td colspan=3><b>Report No:<b></td> <td colspan=3>"
                        + (dsList.Tables[0].Rows[0]["ReportNo"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>External Provider Name:<b></td> <td colspan=3>" + objGlobalData.GetSupplierNameById(dsList.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()) + "</td></tr>"

                            + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + Eval_Date + "</td></tr>"

                    + "<tr><td colspan=3><b>Scope of Supply or Service:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Scope_ofSupplies"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>PO Details:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["po_detail"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation From Date:< b></td> <td colspan=3>" + Eval_FromDate + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation To Date:<b></td> <td colspan=3>" + Eval_ToDate + "</td></tr>"

                    + "<tr><td colspan=3><b>Total number of LOTs received:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["lots_received"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of quality issues(TQI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Quality_Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of quantity issues(TQNI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quantity_issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total no of delivery issues(TDI):<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Delivery_Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Quality Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quality_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Quantity Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["quantity_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Delivery Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["delivery_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Total Supplier Rating:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["total_rating"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Scheduled_by"].ToString()) + "</td></tr>"

                    +"<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["apprv_status"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["apprv_comments"].ToString()) + "</td></tr>";

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "External Provider Performance");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendPerformanceEvaluationReviewmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddExtProviderPerfTrans(ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {

                string sSqlstmt = "delete from t_extprovider_performance_trans where Id_Performace='" + objPerfList.ExtPerfpList[0].Id_Performace + "'; ";

                for (int i = 0; i < objPerfList.ExtPerfpList.Count; i++)
                {
                    //  string sid_Performance_trans = "null";
                    //if (objPerfList.ExtPerfpList[i].id_Performance_trans != null)
                    //{
                    //    sid_Performance_trans = objPerfList.ExtPerfpList[i].id_Performance_trans;
                    //}
                    sSqlstmt = sSqlstmt + " insert into t_extprovider_performance_trans (Id_Performace,Actions,Personnel_Resp,Priority";
                    string sFields = "", sFieldValue = "", sFieldValues = "";

                    if (objPerfList.ExtPerfpList[i].Due_Date != null && objPerfList.ExtPerfpList[i].Due_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Due_Date";
                        sFieldValue = sFieldValue + ", '" + objPerfList.ExtPerfpList[i].Due_Date.ToString("yyyy/MM/dd") + "'";
                        sFieldValues = sFieldValues + ", Due_Date = values(Due_Date)";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values( '" + objPerfList.ExtPerfpList[0].Id_Performace
                    + "','" + objPerfList.ExtPerfpList[i].Actions + "','" + objPerfList.ExtPerfpList[i].Personnel_Resp + "','" + objPerfList.ExtPerfpList[i].Priority + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sSqlstmt = sSqlstmt + " ON DUPLICATE KEY UPDATE "
                     + " Id_Performace= values(Id_Performace), Actions = values(Actions), Personnel_Resp= values(Personnel_Resp), Priority= values(Priority)";
                    sSqlstmt = sSqlstmt + sFieldValues + " ;";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderPerfTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderPerf(ExtProviderPerformanceModels ObjPerfModels, ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {
               
                string sSqlstmt = "update t_extprovider_performance set  Ext_Provider_Name='" + ObjPerfModels.Ext_Provider_Name
                   + "', Scope_ofSupplies='" + ObjPerfModels.Scope_ofSupplies + "', PO_Issued='" + ObjPerfModels.PO_Issued
                   + "', PO_Completed='" + ObjPerfModels.PO_Completed + "', Quality_Issue='" + ObjPerfModels.Quality_Issue + "', Delivery_Issue='" + ObjPerfModels.Delivery_Issue +"'"
                   + ", Scheduled_by='" + ObjPerfModels.Scheduled_by + "', Approved_by='" + ObjPerfModels.Approved_by
                   + "', branch='" + ObjPerfModels.branch + "', Department='" + ObjPerfModels.Department + "', Location='" + ObjPerfModels.Location + "'"
                   + ", quantity_issue='" + ObjPerfModels.quantity_issue + "', lots_received='" + ObjPerfModels.lots_received + "', quality_rating='" + ObjPerfModels.quality_rating + "', quantity_rating='" + ObjPerfModels.quantity_rating + "', delivery_rating='" + ObjPerfModels.delivery_rating + "', total_rating='" + ObjPerfModels.total_rating + "', upload='" + ObjPerfModels.upload + "', notified_to='" + ObjPerfModels.notified_to + "', po_detail='" + ObjPerfModels.po_detail + "'";

                if (ObjPerfModels.Eval_Date != null && ObjPerfModels.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_Date='" + ObjPerfModels.Eval_Date.ToString("yyyy/MM/dd") + "'";
                }
                if (ObjPerfModels.Eval_FromDate != null && ObjPerfModels.Eval_FromDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_FromDate='" + ObjPerfModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }
                if (ObjPerfModels.Eval_ToDate != null && ObjPerfModels.Eval_ToDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_ToDate='" + ObjPerfModels.Eval_ToDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where Id_Performace='" + ObjPerfModels.Id_Performace + "'";

                int Id_Performace = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id_Performace))
                {
                    if (Convert.ToInt32(objPerfList.ExtPerfpList.Count) > 0)
                    {
                        objPerfList.ExtPerfpList[0].Id_Performace = ObjPerfModels.Id_Performace;
                        FunAddExtProviderPerfTrans(objPerfList);
                    }
                    else
                    {
                        FunUpdateExtProviderPerfTrans(ObjPerfModels.Id_Performace);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderPerfTrans(string sId_Performace)
        {
            try
            {
                string sSqlstmt = "delete from t_extprovider_performance_trans where Id_Performace='" + sId_Performace + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditTeamDetails: " + ex.ToString());
            }
            return false;
        }

        //action initiated
        internal bool FunUpdateActionInitiated(ExtProviderPerformanceModels objModel)
        {
            try
            {

                string sSqlstmt = "update t_extprovider_performance set action_taken='" + action_taken + "',action_by='" + action_by + "',action_notified_to='" + action_notified_to + "'";
                if (objModel.initiated_date != null && objModel.initiated_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",initiated_date='" + objModel.initiated_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.Due_Date != null && objModel.Due_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",Due_Date='" + objModel.Due_Date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where Id_Performace='" + Id_Performace + "'";
                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                   return SendActionInitEmail(Id_Performace, "Supplier Performance Evaluation Action Initiated");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditStatus: " + ex.ToString());
            }
            return false;
        }
        internal bool SendActionInitEmail(string  Id_Performace, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select ReportNo,Ext_Provider_Name,initiated_date,action_taken,Due_Date,action_by,action_notified_to  from t_extprovider_performance where Id_Performace ='" + Id_Performace + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);
                NCModels objType = new NCModels();

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["action_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["action_notified_to"].ToString());

                    string initiated_date = "", Due_Date="";
                    if (dsList.Tables[0].Rows[0]["initiated_date"].ToString() != null && Convert.ToDateTime(dsList.Tables[0].Rows[0]["initiated_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        initiated_date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["initiated_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsList.Tables[0].Rows[0]["Due_Date"].ToString() != null && Convert.ToDateTime(dsList.Tables[0].Rows[0]["Due_Date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        Due_Date = Convert.ToDateTime(dsList.Tables[0].Rows[0]["Due_Date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Report Number:<b></td> <td colspan=3>" + dsList.Tables[0].Rows[0]["ReportNo"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>External Provider Name:<b></td> <td colspan=3>" + objGlobalData.GetSupplierNameById(dsList.Tables[0].Rows[0]["Ext_Provider_Name"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Action Initiated on:<b></td> <td colspan=3>" + initiated_date + "</td></tr>"

                     + "<tr><td colspan=3><b>Action to be taken:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsList.Tables[0].Rows[0]["action_taken"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Due Date:<b></td> <td colspan=3>" + Due_Date + "</td></tr>"

                    + "<tr><td colspan=3><b>Action to be taken by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["action_by"].ToString()) + "</td></tr>";

                  

                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Supplier Performance Evaluation Action Initiated Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendActionInitEmail: " + ex.ToString());
            }
            return false;
        }

    }



    public class ExtProviderPerformanceModelsList
    {
        public List<ExtProviderPerformanceModels> ExtPerfpList { get; set; }
    }

    public class ExtProviderDiscrepencyLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();
      
        [Display(Name = "Id")]
        public string id_discrepancylog { get; set; }

        [Required]
        [Display(Name = "Discrepancy identified on")]
        public DateTime discre_registerd_date { get; set; }
        
        [Required]
        [Display(Name = "Discrepancy reported on")]
        public DateTime discre_reported_date { get; set; }

        [Required]
        [Display(Name = "Discrepancy Number")]
        public string discrepancy_no { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Discrepancy details")]
        public string discre_detail { get; set; }

        [Required]
        [Display(Name = "Containment actions initiated")]
        public string actions { get; set; }

        [Required]
        [Display(Name = "Documents to upload")]
        public string upload { get; set; }

        [Required]
        [Display(Name = "External Provider Name")]
        public string ext_provider_name { get; set; }

        [Required]
        [Display(Name = "Delivery Challan/Invoice No")]
        public string delivery_note_no { get; set; }

        [Required]
        [Display(Name = "Our PO number to External Provider")]
        public string po_no { get; set; }

        [Required]
        [Display(Name = "Discrepancy related to")]
        public string discre_relatedto { get; set; }

        [Required]
        [Display(Name = "Supplier Ref")]
        public string supplier_ref { get; set; }

        [Required]
        [Display(Name = "Impact on our performance due to discrepancy")]
        public string impact { get; set; }

        [Required]
        [Display(Name = "Need of raising NCR to External Provider?")]
        public string ncr_required { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "NC No")]
        public string id_nc { get; set; }
        public string id_ncs { get; set; }

        internal bool FunInvalidSupplierDescp(string sid_discrepancylog, string invalid_reason)
        {
            try
            {
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_external_provider_discrepancylog set ext_valid='Invalid',invalid_reason='" + invalid_reason + "',invalid_date='" + TodayDate + "',invalid_logged_by='" + user + "' where id_discrepancylog='" + sid_discrepancylog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInvalidInspChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExtProviderDiscrepencyLog(ExtProviderDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
               // string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sUser = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "insert into t_external_provider_discrepancylog (discrepancy_no, ext_provider_name, " +
                    "delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,logged_by,branch,Department,Location";
                string sFields = "", sFieldValue = "";

                if (objDescripencyLog.discre_registerd_date != null && objDescripencyLog.discre_registerd_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", discre_registerd_date";
                    sFieldValue = sFieldValue + ", '" + objDescripencyLog.discre_registerd_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objDescripencyLog.discre_reported_date != null && objDescripencyLog.discre_reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", discre_reported_date";
                    sFieldValue = sFieldValue + ", '" + objDescripencyLog.discre_reported_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objDescripencyLog.discrepancy_no + "','" + objDescripencyLog.ext_provider_name + "','" + objDescripencyLog.delivery_note_no
                        + "','" + objDescripencyLog.po_no + "','" + objDescripencyLog.discre_detail + "','" + objDescripencyLog.discre_relatedto + "','" + objDescripencyLog.upload 
                        + "','" + objDescripencyLog.actions + "','" + objDescripencyLog.impact + "','" + objDescripencyLog.ncr_required + "','" + sUser + "','" + objDescripencyLog.branch
                         + "','" + objDescripencyLog.Department + "','" + objDescripencyLog.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderDiscrepencyLog(ExtProviderDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
                string sSqlstmt = "update t_external_provider_discrepancylog set discrepancy_no='" + objDescripencyLog.discrepancy_no + "', ext_provider_name='" + objDescripencyLog.ext_provider_name
                    + "', delivery_note_no='" + objDescripencyLog.delivery_note_no + "', po_no='" + objDescripencyLog.po_no + "', discre_detail='" + objDescripencyLog.discre_detail + "', discre_relatedto='" + objDescripencyLog.discre_relatedto + "', actions='" + objDescripencyLog.actions + "'"
                    + ", impact='" + objDescripencyLog.impact + "', ncr_required='" + objDescripencyLog.ncr_required + "', branch='" + objDescripencyLog.branch + "', Department='" + objDescripencyLog.Department + "', Location='" + objDescripencyLog.Location + "'";

                if (objDescripencyLog.discre_registerd_date != null && objDescripencyLog.discre_registerd_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", discre_registerd_date='" + objDescripencyLog.discre_registerd_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objDescripencyLog.discre_reported_date != null && objDescripencyLog.discre_reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", discre_reported_date='" + objDescripencyLog.discre_reported_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objDescripencyLog.upload != null)
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objDescripencyLog.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where id_discrepancylog='" + objDescripencyLog.id_discrepancylog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExtProviderDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDiscrepencyLogDoc(string sid_discrepancylog)
        {
            try
            {
                string sSqlstmt = "update t_external_provider_discrepancylog set active=0 where id_discrepancylog='" + sid_discrepancylog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDiscrepencyLogDoc: " + ex.ToString());
            }
            return false;
        }

    }

    public class ExtProviderDiscrepencyLogModelsList
    {
        public List<ExtProviderDiscrepencyLogModels> DescripList{ get; set; }
    } 
}