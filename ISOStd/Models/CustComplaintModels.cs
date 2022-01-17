using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class CustComplaintModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "ID")]
        public int id_complaint { get; set; }

        [Required]
        [Display(Name = "Complaint Number")]
        public string ComplaintNo { get; set; }

        [Required]
        [Display(Name = "Logged Date")]
        public DateTime LoggedDate { get; set; }

        [Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        //[Required]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Required]
        [Display(Name = "Complaint Received Date")]
        public DateTime ReceivedDate { get; set; }

        [Required]
        [Display(Name = "Complaint Reported By")]
        public string ReportedBy { get; set; }

        [Required]
        [Display(Name = "Mode Of Complaint")]
        public string ModeOfComplaint { get; set; }

        [Required]
        [Display(Name = "Details of Complaint")]
        public string Details { get; set; }

        [Display(Name = "Division")]
        public string divisionId { get; set; }

        [Required]
        [Display(Name = "Complaint Forwarded To")]
        public string ForwardTo { get; set; }
        public string ForwardToId { get; set; }

        [Required]
        [Display(Name = "Overall Complaint Status")]
        public string ComplaintStatus { get; set; }

        [Required]
        [Display(Name = "Upload Document")]
        public string Document { get; set; }

        [Required]
        [Display(Name = "Forwarder count")]
        public int ForwarderCount { get; set; }

        [Required]
        [Display(Name = "Forwarder Assign")]
        public string ForwarderAssign { get; set; }

        [Required]
        [Display(Name = "ID")]
        public int id_complaint_assign { get; set; }

        [Required]
        [Display(Name = "Assigned By")]
        public string AssignedBy { get; set; }

        [Required]
        [Display(Name = "Assign To")]
        public string AssignedTo { get; set; }
        public string AssignedToId { get; set; }

        [Required]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }

        [Required]
        [Display(Name = "Assigned Date")]
        public DateTime AssignDate { get; set; }

        [Required]
        [Display(Name = "ID")]
        public int id_custcomplaint_action { get; set; }

        [Required]
        [Display(Name = "Reason for the problem / complaint")]
        public string ReasonForProblem { get; set; }


        [Required]
        [Display(Name = "Immediate actions to be taken or actions taken")]
        public string ImmediateAction { get; set; }

        [Required]
        [Display(Name = "Proposed corrective actions")]
        public string CorrectiveAction { get; set; }

        [Required]
        [Display(Name = "Document")]
        public string Reason_document { get; set; }

        [Required]
        [Display(Name = "Document")]
        public string ImmediateAction_document { get; set; }

        [Required]
        [Display(Name = "Target Date")]
        public DateTime ImmediateAction_TargetDate { get; set; }

        [Required]
        [Display(Name = "Target Date")]
        public DateTime CorrectiveAction_TargetDate { get; set; }

        [Display(Name = "Id")]
        public int id_complaint_comment { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Document")]
        public string DataUpload { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        public string file { get; set; }

        public string AssignTo { get; set; }

        [Display(Name = "Customer Reference")]
        public string CustomerRef { get; set; }

        [Display(Name = "Reported person email")]
        public string reportedby_email { get; set; }

        [Display(Name = "Reported person designation")]
        public string reportedby_desig { get; set; }

        [Display(Name = "Reported person contact number")]
        public string reportedby_no { get; set; }

        [Display(Name = "Complaint registered date")]
        public DateTime registered_on { get; set; }

        [Display(Name = "Initial Observation Details")]
        public string initial_observation { get; set; }

        [Display(Name = "Complaint Related to")]
        public string complaint_relatedto { get; set; }

        [Display(Name = "Complaint Copied to")]
        public string complaint_copiedto { get; set; }
        public List<CustComplaintModels> CustList { get; set; }
        public List<CustComplaintModels> DispList { get; set; }
        public List<CustComplaintModels> CAList { get; set; }
        //-------------t_custcomplaint_nc------------------

        [Display(Name = "Id")]
        public string id_custcomplaint_nc { get; set; }

        [Display(Name = "Complaint Reference No.")]
        public string nc_no { get; set; }

        //Disposition
        [Display(Name = "Are actions effective to resolve complaint?")]
        public string disp_action_taken { get; set; }       

        [Display(Name = "Brief explanation")]
        public string disp_explain { get; set; }

        [Display(Name = "Notified to")]
        public string disp_notifiedto { get; set; }

        [Display(Name = "Notified on")]
        public DateTime disp_notifeddate { get; set; }

        [Display(Name = "Upload")]
        public string disp_upload { get; set; }

        //t_custcomplaint_nc_disp_action

        public string id_cust_nc_disp_action { get; set; }

        [Display(Name = "Action Taken/To be Taken")]
        public string disp_action { get; set; }

        [Display(Name = "Person Responsible")]
        public string disp_resp_person { get; set; }

        [Display(Name = "Action completed on / Target date to complete")]
        public DateTime disp_complete_date { get; set; }

        [Display(Name ="Feedback reminder date")]
        public DateTime disp_remiderdate { get; set; }

        //Team
        [Display(Name = "Team")]
        public string nc_team { get; set; }
        
        [Display(Name = "Target date to complete the Root Cause Analysis")]
        public DateTime team_targetdate { get; set; }

        //RCA
        [Display(Name = "Techniques adopted")]
        public string rca_technique { get; set; }

        [Display(Name = "Details of root cause analysis")]
        public string rca_details { get; set; }

        [Display(Name = "Upload the Document")]
        public string rca_upload { get; set; }

        [Display(Name = "Need of corrective action")]
        public string rca_action { get; set; }

        [Display(Name = "If No, justify")]
        public string rca_justify { get; set; }

        [Display(Name = "RCA Completed By")]
        public string rca_reportedby { get; set; }

        [Display(Name = "Notified to")]
        public string rca_notifiedto { get; set; }

        [Display(Name = "RCA Completed / Reported On")]
        public DateTime rca_reporteddate { get; set; }

        [Display(Name = "RCA Started On")]
        public DateTime rca_startdate { get; set; }

        //CA

        [Display(Name = "Verification due date")]
        public DateTime ca_verfiry_duedate { get; set; }

        [Display(Name = "To be verified by")]
        public string ca_proposed_by { get; set; }

        [Display(Name = "Notified to")]
        public string ca_notifiedto { get; set; }

        [Display(Name = "Notified Date")]
        public DateTime ca_notifed_date { get; set; }


        //t_custcomplaint_custcomplaint_nc_corrective_action

        public string id_cust_nc_ca { get; set; }

        [Display(Name = "Corrective action")]
        public string ca_ca { get; set; }

        [Display(Name = "Resource required")]
        public string ca_resource { get; set; }

        [Display(Name = "Target date")]
        public DateTime ca_target_date { get; set; }

        [Display(Name = "Person Responsible")]
        public string ca_resp_person { get; set; }

        [Display(Name = "Implementation status")]
        public string implement_status { get; set; }

        [Display(Name = "Is Corrective Action effective?")]
        public string ca_effective { get; set; }

        [Display(Name = "If No, reasons")]
        public string reason { get; set; }

        //Verification
        [Display(Name = "Are proposed actions implemented effectively?")]
        public string v_implement { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_implement_explain { get; set; }

        [Display(Name = "Is RCA process effective?")]
        public string v_rca { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_rca_explain { get; set; }

        [Display(Name = "Similar discrepancies detected from date of implementing corrective action?")]
        public string v_discrepancies { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_discrep_explain { get; set; }

        [Display(Name = "Upload the Document")]
        public string v_upload { get; set; }

        [Display(Name = "Complaint Status")]
        public string v_status { get; set; }

        [Display(Name = "Closed Date")]
        public DateTime v_closed_date { get; set; }

        [Display(Name = "Verified by")]
        public string v_verifiedto { get; set; }

        [Display(Name = "Verified On")]
        public DateTime v_verified_date { get; set; }

        [Display(Name = "Notified To")]
        public string v_notifiedto { get; set; }

        [Display(Name = "Complaint Status")]
        public string nc_status { get; set; }
       
        public string nc_active { get; set; }

        //Assing Customer complaint
        [Display(Name = "Review Start Date")]
        public DateTime complain_review_sdate { get; set; }

        [Display(Name = "Review End Date")]
        public DateTime complain_review_date { get; set; }

        [Display(Name = "Is it valid complaint?")]
        public string complaint_valid { get; set; }

        [Display(Name = "What is the deviation")]
        public string complaint_deviation { get; set; }

        [Display(Name = "Remark")]
        public string complaint_remark { get; set; }

        [Display(Name = "Reason of rejecting complaint")]
        public string rej_reason { get; set; }

        [Display(Name = "Upload")]
        public string rej_upload { get; set; }
      
        [Display(Name = "Complaint Review Status")]
        public string complaint_review_status { get; set; }
       
   
        // Customer Response Details
        [Display(Name = "Customer Complaint Response")]
        public string c_response { get; set; }

        [Display(Name = "Response Details")]
        public string c_response_details { get; set; }

        [Display(Name = "Upload Document")]
        public string c_reponse_upload { get; set; }

        [Display(Name = "Response Date")]
        public DateTime c_response_date { get; set; }

        //t_custcomplaint
        public string ForwarderAssignId { get; set; }

        internal bool FunDeleteCustComplaintDoc(string sid_complaint)
        {
            try
            {
                string sSqlstmt = "update t_custcomplaint set Active=0 where id_complaint='" + sid_complaint + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustComplaintDoc: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddCustomerCompliant(CustComplaintModels objCustomerCompliantModels)
        {
            try
            {
                string[] name = objCustomerCompliantModels.ForwardTo.Split(',');
                int count = name.Length;
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

               
                string sBranch = objGlobalData.GetCurrentUserSession().division;                

                //if (objCustomerCompliantModels.divisionId != "" && objCustomerCompliantModels.divisionId != sBranch)
                //{
                //    sBranch = sBranch + "," + objCustomerCompliantModels.divisionId;
                //}                

                string sReceivedDate = objCustomerCompliantModels.ReceivedDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sregistered_on = objCustomerCompliantModels.registered_on.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sLoggedDate = DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_custcomplaint(ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                + "Details,divisionId,ForwardTo,ComplaintStatus,Document,ForwarderCount,CustomerRef,registered_on,branch,initial_observation,complaint_relatedto,complaint_copiedto)"
                + " values('" + objCustomerCompliantModels.ComplaintNo + "','" + sLoggedDate + "','" + user
                + "','" + objCustomerCompliantModels.CustomerName + "','" + objCustomerCompliantModels.ProjectName + "','" + sReceivedDate
                + "','" + objCustomerCompliantModels.ReportedBy + "','" + objCustomerCompliantModels.ModeOfComplaint + "','" + objCustomerCompliantModels.Details
                + "','" + objCustomerCompliantModels.divisionId + "','" + objCustomerCompliantModels.ForwardTo + "','" + objCustomerCompliantModels.ComplaintStatus + "','" + objCustomerCompliantModels.Document
                + "','" + count + "','" + objCustomerCompliantModels.CustomerRef /*+ "','" + objCustomerCompliantModels.reportedby_email + "','" + objCustomerCompliantModels.reportedby_desig + "','" + objCustomerCompliantModels.reportedby_no */
                +"','"+ sregistered_on + "','" + sBranch + "','" + objCustomerCompliantModels.initial_observation + "','" + objCustomerCompliantModels.complaint_relatedto + "','" + objCustomerCompliantModels.complaint_copiedto + "')";

                int Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id))
                {
                    if (Id > 0)
                    {
                        DataSet dsData = objGlobalData.GetReportNo("CustComplaint", "", "");
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ComplaintNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_custcomplaint set ComplaintNo='" + ComplaintNo + "' where id_complaint = '" + Id + "'";
                       
                        return objGlobalData.ExecuteQuery(sql);

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerCompliant: " + ex.ToString());
            }

            return false;
        }
        ////customer complaint
        //internal bool SendCustomerComplaintmail(int id_complaint, string sMessage = "")
        //{
        //    try
        //    {
        //        string sType = "email";
        //        string sSqlstmt = "select ReceivedDate,LoggedBy,ForwardTo,registered_on,CustomerName,ReportedBy,ComplaintNo,Details from t_custcomplaint where id_complaint = '" + id_complaint + "'; ";

        //        DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

        //        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
        //        {
        //            string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

        //            DataSet dsEmailXML = new DataSet();
        //            dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

        //            if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
        //            {
        //                sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
        //            }

        //            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
        //            {
        //                sContent = reader.ReadToEnd();
        //            }
        //            string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["ForwardTo"].ToString());
        //            string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["ForwardTo"].ToString());
        //            string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["ReportedBy"].ToString());

        //            string ReceivedDate = "", registered_on="";
        //            if (dsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != null && dsList.Tables[0].Rows[0]["ReceivedDate"].ToString() != "")
        //            {
        //                ReceivedDate = Convert.ToDateTime(dsList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy");
        //            }
        //            if (dsList.Tables[0].Rows[0]["registered_on"].ToString() != null && dsList.Tables[0].Rows[0]["registered_on"].ToString() != "")
        //            {
        //                registered_on = Convert.ToDateTime(dsList.Tables[0].Rows[0]["registered_on"].ToString()).ToString("dd/MM/yyyy");
        //            }

        //            sHeader = "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>"
        //            + ReceivedDate + "</td></tr>"
        //            + "<tr><td colspan=3><b>Complaint registered date:<b></td> <td colspan=3>" + registered_on + "</td></tr>"
        //            + "<tr><td colspan=3><b>Customer Complaint Registered By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "</td></tr>"
        //            + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
        //            + "<tr><td colspan=3><b>Details of Complaint:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["Details"].ToString()) + "</td></tr>";

        //            sContent = sContent.Replace("{FromMsg}", "");
        //            sContent = sContent.Replace("{UserName}", sName);
        //            sContent = sContent.Replace("{Title}", "Customer Complaint Details");
        //            sContent = sContent.Replace("{content}", sHeader);
        //            sContent = sContent.Replace("{message}", "");
        //            sContent = sContent.Replace("{extramessage}", "");

        //            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
        //            sToEmailIds = sToEmailIds.Trim();
        //            sToEmailIds = sToEmailIds.TrimEnd(',');
        //            sToEmailIds = sToEmailIds.TrimStart(',');

        //            sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
        //            sCCEmailIds = sCCEmailIds.Trim();
        //            sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            sCCEmailIds = sCCEmailIds.TrimStart(',');

        //            return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendCustomerComplaintmail: " + ex.ToString());
        //    }
        //    return false;
        //}
        internal bool FunUpdateCustomerComplaint(CustComplaintModels objCustomerCompliantModels)
        {
            try
            {
                string[] name = objCustomerCompliantModels.ForwardTo.Split(',');
                int count = name.Length;
                
                string sReceivedDate = objCustomerCompliantModels.ReceivedDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sRegisterdDate = objCustomerCompliantModels.registered_on.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_custcomplaint set CustomerName='" + objCustomerCompliantModels.CustomerName + "', "
                    + "ProjectName='" + objCustomerCompliantModels.ProjectName + "', ReceivedDate='" + sReceivedDate
                    + "', ReportedBy='" + objCustomerCompliantModels.ReportedBy + "', ModeOfComplaint='" + objCustomerCompliantModels.ModeOfComplaint + "', Details='"
                    + objCustomerCompliantModels.Details + "', divisionId='" + objCustomerCompliantModels.divisionId + "', ForwardTo='" + objCustomerCompliantModels.ForwardTo + "', ComplaintStatus='"
                    + objCustomerCompliantModels.ComplaintStatus + "',ForwarderCount='" + count + "', CustomerRef='" + objCustomerCompliantModels.CustomerRef 
                   /* + "', reportedby_email='" + objCustomerCompliantModels.reportedby_email + "', reportedby_desig='" + objCustomerCompliantModels.reportedby_desig + "', reportedby_no='" + objCustomerCompliantModels.reportedby_no */
                   + "', registered_on='" + sRegisterdDate + "', initial_observation='" + objCustomerCompliantModels.initial_observation + "', complaint_relatedto='" + objCustomerCompliantModels.complaint_relatedto + "', complaint_copiedto='" + objCustomerCompliantModels.complaint_copiedto + "'";

                if (objCustomerCompliantModels.Document != null)
                {
                    sSqlstmt = sSqlstmt + ",Document='" + objCustomerCompliantModels.Document + "'";
                }

                sSqlstmt = sSqlstmt + " where id_complaint='" + objCustomerCompliantModels.id_complaint + "';";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomerComplaint: " + ex.ToString());
            }

            return false;
        }

        //calling from controller
        internal bool SendCustComplaintRegistmail(string ComplaintNo, string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,ForwardTo,Document,initial_observation,complaint_relatedto,complaint_copiedto from t_custcomplaint where ComplaintNo='" + ComplaintNo + "'";

                DataSet dsComplaintList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();


                if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "", aAttachment1 = "";

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
                    string sName = "All";
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    if(dsComplaintList.Tables[0].Rows[0]["complaint_copiedto"].ToString() != "" && dsComplaintList.Tables[0].Rows[0]["complaint_copiedto"] != null)
                    {
                        foreach (var item in dsComplaintList.Tables[0].Rows[0]["complaint_copiedto"].ToString().Split(','))
                        {
                            sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(item);
                        }
                    }

                    sToEmailIds = sToEmailIds.Trim(',');
                    sCCEmailIds = sCCEmailIds.Trim(',');

                    //aAttachment = HttpContext.Current.Server.MapPath(dsComplaintList.Tables[0].Rows[0]["Document"].ToString());

                    string sqlstmt = "SELECT id_complaint,SUBSTRING_INDEX(SUBSTRING_INDEX(Document, ',', numbers.n), ',', -1) Document FROM  numbers INNER JOIN t_custcomplaint" +
                        "  ON CHAR_LENGTH(Document)-CHAR_LENGTH(REPLACE(Document, ',', '')) >= numbers.n - 1 where ComplaintNo='" + ComplaintNo + "' ORDER BY id_complaint, n ";

                    DataSet dsDocument = objGlobalData.Getdetails(sqlstmt);
                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsDocument.Tables[0].Rows.Count; i++)
                        {
                            aAttachment1 = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[i]["Document"].ToString());
                            aAttachment = aAttachment1 + "," + aAttachment;
                        }
                        aAttachment = aAttachment.Trim(',');
                    }


                    sHeader = "<tr><td colspan=3><b>Complaint No:<b></td> <td colspan=3>"
                        + dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Logged By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Project Name:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" +objGlobalData.GetCustomerContactPersonByCustId(dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>ModeOfComplaint:<b></td> <td colspan=3>" + objGlobalData.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["Details"].ToString() + "</td></tr>"
                       + "<tr><td colspan=3><b>Initial Observation:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["initial_observation"].ToString() + "</td></tr>"
                       + "<tr><td colspan=3><b>Complaint Related to:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsComplaintList.Tables[0].Rows[0]["complaint_relatedto"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Customer Complaint Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                   

                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustComplaintRegistmail: " + ex.ToString());
            }
            return false;
        }

        //Assignment
        internal bool FunAssignCustomerComplaint(CustComplaintModels objCustomerCompliantModels)
        {
            try
            {
                string sAssignDate = DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sTargetDate = objCustomerCompliantModels.TargetDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sComplaintReview = objCustomerCompliantModels.complain_review_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sComplaintStartReview = objCustomerCompliantModels.complain_review_sdate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_custcomplaint set ForwarderAssign= '" + objCustomerCompliantModels.AssignedTo + "', complain_review_date='" + sComplaintReview
                    + "', divisionId='" + objCustomerCompliantModels.divisionId + "', complaint_valid='" + objCustomerCompliantModels.complaint_valid + "', complaint_deviation='" + objCustomerCompliantModels.complaint_deviation
                    + "', complaint_remark='" + objCustomerCompliantModels.complaint_remark + "', rej_reason='" + objCustomerCompliantModels.rej_reason + "', rej_upload='" + objCustomerCompliantModels.rej_upload + "' , TargetDate='" + sTargetDate
                    + "', AssignDate='" + sAssignDate + "', complain_review_sdate='" + sComplaintStartReview + "', complaint_review_status='" + objCustomerCompliantModels.complaint_review_status + "' where id_complaint='" + objCustomerCompliantModels.id_complaint +"';";

                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if(objCustomerCompliantModels.AssignedTo != null && objCustomerCompliantModels.AssignedTo != "")
                    {
                       string[] snumber = objCustomerCompliantModels.AssignedTo.Split(',');
                       int count = snumber.Length;

                        for(int index=0; index< count; index ++)
                        {
                            string sql = "insert into t_custcomplaint_nc (id_complaint,ForwarderAssign) values ('"+ objCustomerCompliantModels.id_complaint + "','" + snumber[index] + "')";
                            objGlobalData.ExecuteQuery(sql);
                        }                        
                    }                    
                }
                SendAssignmail(objCustomerCompliantModels.id_complaint);
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAssignCustomerComplaint: " + ex.ToString());
            }
            return false;
        }


        internal bool SendAssignmail(int id_complaint)
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;


                string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,ForwardTo,ComplaintStatus,Document,ForwarderAssign,complaint_valid,complaint_deviation,complaint_remark,rej_reason,rej_upload,AssignDate,TargetDate from t_custcomplaint where"
                + " id_complaint='" + id_complaint + "'";

                DataSet dsComplaintList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();


                if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", RejectaAttachment = "", sSubject = "", sContent = "";

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
                    string sName = "All";
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["ForwardTo"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["ForwarderAssign"].ToString());

               //     Attachment = HttpContext.Current.Server.MapPath(dsComplaintList.Tables[0].Rows[0]["Document"].ToString());
                    if (dsComplaintList.Tables[0].Rows[0]["rej_upload"].ToString() != "")
                    {
                        RejectaAttachment = HttpContext.Current.Server.MapPath(dsComplaintList.Tables[0].Rows[0]["rej_upload"].ToString());
              //          aAttachment = Attachment + "," +RejectaAttachment;
                    }

                    sHeader = "<tr><td colspan=3><b>Complaint No:<b></td> <td colspan=3>"
                        + dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Logged By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Project Name:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobalData.GetCustomerContactPersonByCustId(dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>ModeOfComplaint:<b></td> <td colspan=3>" + objGlobalData.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["Details"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Is it valid complaint? :<b></td> <td colspan=3>" + (dsComplaintList.Tables[0].Rows[0]["complaint_valid"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>What is the deviation:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsComplaintList.Tables[0].Rows[0]["complaint_deviation"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Remark :<b></td> <td colspan=3>" + (dsComplaintList.Tables[0].Rows[0]["complaint_remark"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Response Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["AssignDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>";

                    if (dsComplaintList.Tables[0].Rows[0]["complaint_valid"].ToString() == "No")
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Reason of rejecting complaint :<b></td> <td colspan=3>" + (dsComplaintList.Tables[0].Rows[0]["rej_reason"].ToString()) + "</td></tr>";
                    }
                    else if (dsComplaintList.Tables[0].Rows[0]["complaint_valid"].ToString() == "Yes")
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Target Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["TargetDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>" + "<tr><td colspan=3><b>Responsible Person :<b></td> <td colspan=3>" +objGlobalData.GetMultiHrEmpNameById(dsComplaintList.Tables[0].Rows[0]["ForwarderAssign"].ToString()) + "</td></tr>";
                    }

                    if(RejectaAttachment.Contains(','))
                    {
                        string[] var = RejectaAttachment.Split(',');
                        for(int i = 0; i < var.Length; i++)
                        {
                            if (File.Exists(var[i]))
                            {
                                sHeader = sHeader + "<tr><td colspan=3><b>Document for Rejection of Complaint:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                                break;
                            }
                        }
                    }
                   
                    //if (File.Exists(Attachment))
                    //{
                    //    sHeader = sHeader + "<tr><td colspan=3><b>Document:<b></td> <td colspan=3>Please find the attachment</td></tr>" ;
                    //}

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Customer Complaint Assignment");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Assignment Details", sContent, RejectaAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendAssignmail: " + ex.ToString());
            }
            return false;
        }

        //customer Response
        internal bool FunUpdateCustomerResponse(CustComplaintModels objModel)
        {
            try
            {
                string Response_date = objModel.c_response_date.ToString("yyyy-MM-dd");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "update t_custcomplaint set c_response= '" + objModel.c_response + "' , c_response_details='" + objModel.c_response_details + "' , c_reponse_upload='" + objModel.c_reponse_upload
                    + "', c_response_date='" + Response_date + "' where id_complaint='" + objModel.id_complaint + "';";
                
                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendCustomerComplaintmail(objModel.id_complaint);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomerResponse: " + ex.ToString());
            }

            return false;
        }

        internal bool SendCustomerComplaintmail(int id_complaint)
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;


                string sSqlstmt = "select id_complaint,ComplaintNo,LoggedDate,LoggedBy,CustomerName,ProjectName,ReceivedDate,ReportedBy,ModeOfComplaint,"
                    + "Details,ForwardTo,ComplaintStatus,Document,ForwarderAssign,c_response,c_response_details,c_reponse_upload,c_response_date from t_custcomplaint where"
                + " id_complaint='" + id_complaint + "'";

                DataSet dsComplaintList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();


                if (dsComplaintList.Tables.Count > 0 && dsComplaintList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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
                    string sName = "All";
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsComplaintList.Tables[0].Rows[0]["ForwarderAssign"].ToString());

                    aAttachment = HttpContext.Current.Server.MapPath(dsComplaintList.Tables[0].Rows[0]["Document"].ToString());

                    sHeader = "<tr><td colspan=3><b>Complaint No:<b></td> <td colspan=3>"
                        + dsComplaintList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Logged By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsComplaintList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsComplaintList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Project Name:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["ProjectName"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobalData.GetCustomerContactPersonByCustId(dsComplaintList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>ModeOfComplaint:<b></td> <td colspan=3>" + objGlobalData.GetModeOfComplaintById(dsComplaintList.Tables[0].Rows[0]["ModeOfComplaint"].ToString()) + "</td></tr>"
                       //+ "<tr><td colspan=3><b>Status:<b></td> <td colspan=3>" + GetComplaintStatusNameById(dsComplaintList.Tables[0].Rows[0]["ComplaintStatus"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details:<b></td> <td colspan=3>" + dsComplaintList.Tables[0].Rows[0]["Details"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Complaint Response:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsComplaintList.Tables[0].Rows[0]["c_response"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Response Details:<b></td> <td colspan=3>" + (dsComplaintList.Tables[0].Rows[0]["c_response_details"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Response Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsComplaintList.Tables[0].Rows[0]["c_response_date"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Customer Complaint Response");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Response Details", sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustomerComplaintmail: " + ex.ToString());
            }
            return false;
        }
             
        
        //------------------NC-----------------
                 
        //Disposition

        internal bool FunUpdateDisposition(CustComplaintModels objModels, CustComplaintModelsList objDispList)
        {
            try
            {

                string sSqlstmt = "update t_custcomplaint_nc set  disp_action_taken='" + objModels.disp_action_taken + "', "
                    + "disp_explain='" + objModels.disp_explain + "', disp_notifiedto='" + objModels.disp_notifiedto + "', disp_upload='" + objModels.disp_upload + "'";

                if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.disp_remiderdate != null && objModels.disp_remiderdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", disp_remiderdate ='" + objModels.disp_remiderdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_custcomplaint_nc='" + objModels.id_custcomplaint_nc + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objDispList.CustComplaintList.Count) > 0)
                    {
                        objDispList.CustComplaintList[0].id_custcomplaint_nc = id_custcomplaint_nc.ToString();
                        FunAddDispList(objDispList);
                    }
                    else
                    {
                        FunUpdateDispList(id_custcomplaint_nc);
                    }
                    SendCustDispEmail(objModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDisposition: " + ex.ToString());
            }
            return true;
        }

        internal bool FunAddDispList(CustComplaintModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "delete from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + objDispList.CustComplaintList[0].id_custcomplaint_nc + "'; ";

                for (int i = 0; i < objDispList.CustComplaintList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_custcomplaint_nc_disp_action(id_custcomplaint_nc,disp_action,disp_resp_person";

                    string sFieldValue = "", sFields = "";
                    if (objDispList.CustComplaintList[i].disp_complete_date != null && objDispList.CustComplaintList[i].disp_complete_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sFields = sFields + ", disp_complete_date";
                        sFieldValue = sFieldValue + ", '" + objDispList.CustComplaintList[i].disp_complete_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objDispList.CustComplaintList[0].id_custcomplaint_nc + "', '" + objDispList.CustComplaintList[i].disp_action + "', '" + objDispList.CustComplaintList[i].disp_resp_person + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDispList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDispList(string sid_custcomplaint_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_custcomplaint_nc_disp_action where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDispList: " + ex.ToString());
            }
            return false;
        }

        internal bool SendCustDispEmail(CustComplaintModels objModels)
        {
            try
            {
                string sType = "email";

                
                string sSqlstmt = "select id_custcomplaint_nc, ComplaintNo, CustomerName, ReportedBy, ReceivedDate,Document,LoggedBy,b.ForwarderAssign,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate from t_custcomplaint_nc a, " +
                    "t_custcomplaint b where id_custcomplaint_nc =  '" + objModels.id_custcomplaint_nc + "' and a.id_complaint = b.id_complaint";
                DataSet dsNCList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["disp_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["disp_notifiedto"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    //if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString()) != "")
                    //{
                    //    sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString());
                    //}
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["Document"].ToString());


                    sHeader = "<tr><td colspan=3><b>Complaint Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsNCList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint ReportedBy:<b></td> <td colspan=3>" +objGlobalData.GetCustomerContactPersonByCustId(dsNCList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Are action effective to resolve complaint?:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["disp_action_taken"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Brief explanation:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["disp_explain"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Notified Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["disp_notifeddate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Immediate Action(Disposition) Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Immediate Action(Disposition) Details", sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustDispEmail: " + ex.ToString());
            }
            return false;
        }


        //Team
        internal bool FunUpdateTeam(CustComplaintModels objModels)
        {
            try
            {

                string sSqlstmt = "update t_custcomplaint_nc set  nc_team='" + objModels.nc_team + "'";

                if (objModels.team_targetdate != null && objModels.team_targetdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", team_targetdate ='" + objModels.team_targetdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_custcomplaint_nc='" + objModels.id_custcomplaint_nc + "'";
                //return objGlobalData.ExecuteQuery(sSqlstmt);
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendCustTeamEmail(objModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTeam: " + ex.ToString());
            }
            return false;
        }
        
        internal bool SendCustTeamEmail(CustComplaintModels objModels)
        {
            try
            {
                string sType = "email";

                //string sSqlstmt = "select id_custcomplaint_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed,  nc_pnc, nc_upload,"
                //    + "nc_impact, nc_risk, risklevel, team_targetdate,nc_team from t_custcomplaint_nc where id_custcomplaint_nc ='" + objModels.id_custcomplaint_nc + "'";

                string sSqlstmt = "select id_custcomplaint_nc, ComplaintNo, CustomerName, ReportedBy, ReceivedDate,Document,LoggedBy,b.ForwarderAssign,team_targetdate, nc_team from t_custcomplaint_nc a, " +
                    "t_custcomplaint b where id_custcomplaint_nc =  '" + objModels.id_custcomplaint_nc + "' and a.id_complaint = b.id_complaint";
                DataSet dsNCList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_team"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_team"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString()) != "")
                    {
                        sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    }
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["Document"].ToString());


                    sHeader = "<tr><td colspan=3><b>Complaint Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint ReportedBy:<b></td> <td colspan=3>" + objGlobalData.GetCustomerContactPersonByCustId(dsNCList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Target date to complete the Root Cause Analysis:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["team_targetdate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Team Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Team Details", sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustTeamEmail: " + ex.ToString());
            }
            return false;
        }
               
        //RCA
        internal bool FunUpdateRCA(CustComplaintModels objModels, CustComplaintModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_custcomplaint_nc set  rca_technique='" + objModels.rca_technique + "', "
                    + "rca_details  ='" + objModels.rca_details + "', rca_upload='" + objModels.rca_upload + "', rca_action='" + objModels.rca_action
                    + "', rca_justify='" + objModels.rca_justify + "', rca_reportedby='" + objModels.rca_reportedby + "', rca_notifiedto='" + objModels.rca_notifiedto + "', ca_proposed_by='" + objModels.ca_proposed_by + "'";

                if (objModels.rca_startdate != null && objModels.rca_startdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_startdate ='" + objModels.rca_startdate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.rca_reporteddate != null && objModels.rca_reporteddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_reporteddate ='" + objModels.rca_reporteddate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.ca_verfiry_duedate != null && objModels.ca_verfiry_duedate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_verfiry_duedate ='" + objModels.ca_verfiry_duedate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_custcomplaint_nc='" + objModels.id_custcomplaint_nc + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendCustRCAEmail(objModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRCA: " + ex.ToString());
            }
            return false;
        }

        internal bool SendCustRCAEmail(CustComplaintModels objModels)
        {
            try
            {
                string sType = "email";


                string sSqlstmt = "select id_custcomplaint_nc, ComplaintNo, CustomerName, ReportedBy, ReceivedDate,Document,LoggedBy,b.ForwarderAssign,rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate from t_custcomplaint_nc a, " +
                    "t_custcomplaint b where id_custcomplaint_nc =  '" + objModels.id_custcomplaint_nc + "' and a.id_complaint = b.id_complaint";
                DataSet dsNCList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["rca_reportedby"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["rca_reportedby"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["rca_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds +","+ objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["rca_notifiedto"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    //if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString()) != "")
                    //{
                    //    sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString());
                    //}
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["Document"].ToString());


                    sHeader = "<tr><td colspan=3><b>Complaint Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsNCList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint ReportedBy:<b></td> <td colspan=3>" +objGlobalData.GetCustomerContactPersonByCustId(dsNCList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Techniques adopted:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["rca_technique"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Details of root cause analysis:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["rca_details"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Need of corrective action:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["rca_action"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>If No, justify:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["rca_justify"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Notified Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["rca_reporteddate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Root Cause Analysis Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Root Cause Analysis Details", sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustRCAEmail: " + ex.ToString());
            }
            return false;
        }

        //CA
        internal bool FunUpdateCA(CustComplaintModels objModels, CustComplaintModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_custcomplaint_nc set  ca_proposed_by='" + objModels.ca_proposed_by + "', "
                    + "ca_notifiedto='" + objModels.ca_notifiedto + "'";

                if (objModels.ca_verfiry_duedate != null && objModels.ca_verfiry_duedate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_verfiry_duedate ='" + objModels.ca_verfiry_duedate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.ca_notifed_date != null && objModels.ca_notifed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_notifed_date ='" + objModels.ca_notifed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_custcomplaint_nc='" + objModels.id_custcomplaint_nc + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.CustComplaintList.Count) > 0)
                    {
                        objList.CustComplaintList[0].id_custcomplaint_nc = id_custcomplaint_nc.ToString();
                        FunAddCAList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_custcomplaint_nc);
                    }
                    SendCustCAEmail(objModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCA: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCAList(CustComplaintModelsList objCAList)
        {
            try
            {
                //string sSqlstmt = "delete from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc='" + objCAList.CustComplaintList[0].id_custcomplaint_nc + "'; ";
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.CustComplaintList.Count; i++)
                {
                    string sid_cust_nc_ca = "null";
                    string sca_target_date = "";

                    if (objCAList.CustComplaintList[i].id_cust_nc_ca != null && objCAList.CustComplaintList[i].id_cust_nc_ca != "")
                    {
                        sid_cust_nc_ca = objCAList.CustComplaintList[i].id_cust_nc_ca;
                    }

                    if (objCAList.CustComplaintList[i].ca_target_date != null && objCAList.CustComplaintList[i].ca_target_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sca_target_date = objCAList.CustComplaintList[i].ca_target_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_custcomplaint_nc_corrective_action (id_cust_nc_ca,id_custcomplaint_nc,ca_ca,ca_resource,ca_resp_person,ca_target_date)"
                    + " values(" + sid_cust_nc_ca + ",'" + objCAList.CustComplaintList[0].id_custcomplaint_nc + "','" + objCAList.CustComplaintList[i].ca_ca + "','" + objCAList.CustComplaintList[i].ca_resource + "','" + objCAList.CustComplaintList[i].ca_resp_person + "','" + sca_target_date + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_cust_nc_ca= values(id_cust_nc_ca),id_custcomplaint_nc= values(id_custcomplaint_nc), ca_ca = values(ca_ca), ca_resource = values(ca_resource), ca_resp_person = values(ca_resp_person), ca_target_date = values(ca_target_date); ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCAList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCAList(string sid_custcomplaint_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_custcomplaint_nc_corrective_action where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCAList: " + ex.ToString());
            }
            return false;
        }
        
        internal bool SendCustCAEmail(CustComplaintModels objModels)
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_custcomplaint_nc, ComplaintNo, CustomerName, ReportedBy, ReceivedDate,Document,LoggedBy,b.ForwarderAssign,ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_custcomplaint_nc a, " +
                    "t_custcomplaint b where id_custcomplaint_nc =  '" + objModels.id_custcomplaint_nc + "' and a.id_complaint = b.id_complaint";
                DataSet dsNCList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ca_proposed_by"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ca_proposed_by"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ca_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ca_notifiedto"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    //if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString()) != "")
                    //{
                    //    sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString());
                    //}
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["Document"].ToString());


                    sHeader = "<tr><td colspan=3><b>Complaint Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsNCList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint ReportedBy:<b></td> <td colspan=3>" + objGlobalData.GetCustomerContactPersonByCustId(dsNCList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Verification due date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Need of corrective action:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["ca_proposed_by"].ToString() + "</td></tr>"
                        //+ "<tr><td colspan=3><b>If No, justify:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["ca_notifiedto"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Notified Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ca_notifed_date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Corrective Action Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');

                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Corrective Action Details", sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustCAEmail: " + ex.ToString());
            }
            return false;
        }

        // Verification
        internal bool FunUpdateVerification(CustComplaintModels objModels, CustComplaintModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_custcomplaint_nc set  v_implement='" + objModels.v_implement + "', v_implement_explain='" + objModels.v_implement_explain
                    + "', v_rca='" + objModels.v_rca + "', v_rca_explain='" + objModels.v_rca_explain + "', v_discrepancies='" + objModels.v_discrepancies + "', v_discrep_explain='" + objModels.v_discrep_explain
                    + "', v_upload='" + objModels.v_upload + "', v_status='" + objModels.v_status + "', v_verifiedto='" + objModels.v_verifiedto + "', v_notifiedto='" + objModels.v_notifiedto + "'";

                if (objModels.v_closed_date != null && objModels.v_closed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_closed_date ='" + objModels.v_closed_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModels.v_verified_date != null && objModels.v_verified_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_verified_date ='" + objModels.v_verified_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_custcomplaint_nc='" + objModels.id_custcomplaint_nc + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.CustComplaintList.Count) > 0)
                    {
                        objList.CustComplaintList[0].id_custcomplaint_nc = id_custcomplaint_nc.ToString();
                        FunAddVerificationList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_custcomplaint_nc);
                    }
                    SendCustVerficationEmail(objModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateVerification: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddVerificationList(CustComplaintModelsList objCAList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.CustComplaintList.Count; i++)
                {
                    string sid_cust_nc_ca = "";

                    if (objCAList.CustComplaintList[i].id_cust_nc_ca != null && objCAList.CustComplaintList[i].id_cust_nc_ca != "")
                    {
                        sid_cust_nc_ca = objCAList.CustComplaintList[i].id_cust_nc_ca;
                    }
                    sSqlstmt = sSqlstmt + " update t_custcomplaint_nc_corrective_action set implement_status = '" + objCAList.CustComplaintList[i].implement_status
                        + "', ca_effective = '" + objCAList.CustComplaintList[i].ca_effective + "', reason ='" + objCAList.CustComplaintList[i].reason + "' where id_cust_nc_ca = '" + sid_cust_nc_ca + "';";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddVerificationList: " + ex.ToString());
            }
            return false;
        }

        internal bool SendCustVerficationEmail(CustComplaintModels objModels)
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_custcomplaint_nc, ComplaintNo, CustomerName, ReportedBy, ReceivedDate,Document,LoggedBy,b.ForwarderAssign,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload,v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_custcomplaint_nc a, " +
                    "t_custcomplaint b where id_custcomplaint_nc =  '" + objModels.id_custcomplaint_nc + "' and a.id_complaint = b.id_complaint";
                DataSet dsNCList = objGlobalData.Getdetails(sSqlstmt);
                CustComplaintModels objType = new CustComplaintModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["v_verifiedto"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["v_verifiedto"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["v_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["v_notifiedto"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    //if (objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString()) != "")
                    //{
                    //    sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["ForwarderAssign"].ToString());
                    //}
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["Document"].ToString());


                    sHeader = "<tr><td colspan=3><b>Complaint Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["ComplaintNo"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Customer Name:<b></td> <td colspan=3>" + objGlobalData.GetCustomerNameById(dsNCList.Tables[0].Rows[0]["CustomerName"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint ReportedBy:<b></td> <td colspan=3>" +objGlobalData.GetCustomerContactPersonByCustId(dsNCList.Tables[0].Rows[0]["ReportedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Complaint Received Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["ReceivedDate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Verification due date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["v_verified_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Are proposed actions implemented effectively?:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["v_implement"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Brief explanation:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["v_implement_explain"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Is RCA process effective?:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["v_rca"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Brief explanation:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["v_rca_explain"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Similar discrepancies detected from date of implementing corrective action?:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["v_discrepancies"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Brief explanation:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["v_discrep_explain"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Notified Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["v_closed_date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Verification Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');

                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Customer Complaint Verification Details", sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendCustVerficationEmail: " + ex.ToString());
            }
            return false;
        }


        // Delete
        internal bool FunDeleteNCDoc(string sid_custcomplaint_nc)
        {
            try
            {
                string sSqlstmt = "update t_custcomplaint_nc set Active=0 where id_custcomplaint_nc='" + sid_custcomplaint_nc + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNCDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCADoc(string sid_cust_nc_ca)
        {
            try
            {
                string sSqlstmt = "update t_custcomplaint_nc_corrective_action set ca_active=0 where id_cust_nc_ca='" + sid_cust_nc_ca + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCADoc: " + ex.ToString());
            }
            return false;
        }
    }

    public class CustComplaintModelsList
    {
        public List<CustComplaintModels> CustComplaintList { get; set; }
    }

    public class DropdownComplaintItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownComplaintList
    {
        public List<DropdownComplaintItems> lst { get; set; }
    }
}