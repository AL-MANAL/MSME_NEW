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
    public class ManagementChangeModels
    {

        clsGlobal objGlobalData = new clsGlobal();
        
        [Display(Name = "Id")]
        public int IdMgmt { get; set; }
   
        [Display(Name = "Change Request No")]
        public string ChangeRequestNo { get; set; }
        
        [Display(Name = "Change Requested Date")]
        public DateTime ChangeInitiatedDate { get; set; }
          
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }
            
        [Display(Name = "Change Requested By")]
        public string ChangeRequestedBy { get; set; }
            
        [Display(Name = "Change In")]
        public string ChangeIn { get; set; }
        
        [Display(Name = "Details of Change")]
        public string DetailsOfChange { get; set; }
        
        [Display(Name = "Reason For Change")]
        public string ResonForChange { get; set; }
          
        [Display(Name = "Impact")]
        public string Impact { get; set; }
        
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
               
        [Display(Name = "Approved Date")]
        public DateTime ApproveOrRejectOn { get; set; }
            
        [Display(Name = "Completed Date")]
        public DateTime CompletionDate { get; set; }

        [Display(Name = "Approve Status")]
        public string ApproveStatus { get; set; }
         
        [Display(Name = "Completion Date")]
        public DateTime ActionCompletionDate { get; set; }

        [Display(Name = "Requested Department")]
        public string RequestedByDept { get; set; }
        
        [Display(Name = "Approvers")]
        public string Approvers { get; set; }
        
        [Display(Name = "Approver Count")]
        public int ApproverCount { get; set; }
        
        [Display(Name = "Change Status")]
        public string ChangeStatus { get; set; }
        
        [Display(Name = "Id")]
        public int Id { get; set; }
        
        [Display(Name = "Action")]
        public string Action { get; set; }
        
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }
        
        [Display(Name = "Person Responsible")]
        public string PersonResponsible { get; set; }
        
        [Display(Name = "Action Status")]
        public string Action_Status { get; set; }

        [Display(Name = "Change Type")]
        public string ChangeType { get; set; }

        [Display(Name = "Risk Level")]
        public string RiskLevel { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        public string ApproveStatusId { get; set; }

        internal bool FunDeleteChangeMgmtDoc(string sIdMgmt)
        {
            try
            {
                string sSqlstmt = "update t_changemanagement set Active=0 where IdMgmt='" + sIdMgmt + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChangeMgmtDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddChangeManagement(ManagementChangeModels objManagement)
        {
            try
            {
                string[] name = objManagement.ApprovedBy.Split(',');
                int count = name.Length;
                string user = "";              
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sChangeInitiatedDate = DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "insert into t_changemanagement (ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange," +
                    "ResonForChange,Impact,ApprovedBy,ApproverCount,ChangeType,RiskLevel,branch,Department,Location)"
                + " values('" + sChangeInitiatedDate + "','" + user
                + "','" + objManagement.ChangeRequestedBy + "','" + objManagement.ChangeIn + "','" + objManagement.DetailsOfChange + "','"
                + objManagement.ResonForChange + "','" + objManagement.Impact + "','" + objManagement.ApprovedBy + "','" 
                + count + "','" + objManagement.ChangeType + "','" + objManagement.RiskLevel + "','" + objManagement.branch
                + "','" + objManagement.Department + "','" + objManagement.Location + "')";
               
                int IdMgmt = objGlobalData.ExecuteQueryReturnId(sSqlstmt);
                if (IdMgmt != 0)
                {        
                        return SendChangeMgmtmail(IdMgmt, "Change Management Request for Approval");                
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChangeManagement: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddChangeManagementActionDetails(ChangeManagementModelsList obManagmentModelsList, int IdMgmt)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < obManagmentModelsList.ChangeMgmtList.Count; i++)
                {
                    if (obManagmentModelsList.ChangeMgmtList[i].Action != null)
                    {
                        string sTargetDate = obManagmentModelsList.ChangeMgmtList[i].TargetDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                        sSqlstmt = sSqlstmt + "insert into t_changemanagement_actions (IdMgmt,Action,TargetDate,PersonResponsible,Action_Status) values('" + IdMgmt + "','" + obManagmentModelsList.ChangeMgmtList[i].Action + "','" + sTargetDate
                         + "','" + obManagmentModelsList.ChangeMgmtList[i].PersonResponsible + "','" + obManagmentModelsList.ChangeMgmtList[i].Action_Status + "'); ";
                    }
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChangeManagementActionDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool SendChangeMgmtmail( int sIdMgmt,string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,"
                    + " (case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus from t_changemanagement"
                    + " where IdMgmt='" + sIdMgmt + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                ManagementChangeModels objType = new ManagementChangeModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["LoggedBy"].ToString()); 

                    if (objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) != null)
                    {
                        if (sCCEmailIds != "" && sCCEmailIds != null)
                        {
                            sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString());
                        }
                        else
                        {
                            sCCEmailIds =  objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString());
                        }
                   
                    }
                    sHeader = "<tr><td colspan=3><b>Change In:<b></td> <td colspan=3>"
                        + objGlobalData.GetDropdownitemById(dsManagementList.Tables[0].Rows[0]["ChangeIn"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Change Initiated Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details of Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["DetailsOfChange"].ToString() + "</td></tr>"

                       + "<tr><td colspan=3><b>Reasons for Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ResonForChange"].ToString()
                       + "</td></tr>"
                    + "<tr><td colspan=3><b>Impact:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Impact"].ToString() + "</td></tr>";

                    //sSqlstmt = "select Id,IdMgmt,Action,TargetDate,PersonResponsible,Action_Status"
                    //                 + " from t_changemanagement_actions where IdMgmt='" + dsManagementList.Tables[0].Rows[0]["IdMgmt"].ToString()
                    //                 + "' order by Id";

                    //DataSet dsItems = new DataSet();
                    //dsItems = objGlobalData.Getdetails(sSqlstmt);

                    //if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    //{
                    //    sInformation = "<tr> "
                    //        + "<td colspan=6><label><b>Actions to be taken to mitigate the impact on consequences </b></label> </td> </tr>"
                    //        + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                    //        + "<th>Sl. No.</th>"
                    //        + "<th>Action</th>"
                    //        + "<th>Target Date</th>"
                    //        + "<th>Person Responsible</th>"
                    //        + "<th>Action Status</th>"
                    //        + "</tr>";

                    //    for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                    //    {
                    //        sInformation = sInformation + "<tr>"
                    //            + " <td>" + (i + 1) + "</td>"
                    //            + " <td>" + dsItems.Tables[0].Rows[i]["Action"].ToString() + "</td>"
                    //            + "<td> " +Convert.ToDateTime(dsItems.Tables[0].Rows[i]["TargetDate"].ToString()).ToString("yyyy-MM-dd") + " </td>"
                    //             + " <td>" + objGlobalData.GetEmpHrNameById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString()) + "</td>"
                    //            + "<td> " + dsItems.Tables[0].Rows[i]["Action_Status"].ToString() + " </td>"
                    //                                   + " </tr>";
                    //        //sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());

                    //        string sPersonEmail = objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                    //        sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                    //        sPersonEmail = sPersonEmail.Trim();
                    //        sPersonEmail = sPersonEmail.TrimEnd(',');
                    //        sPersonEmail = sPersonEmail.TrimStart(',');
                    //        if (sPersonEmail != null && sPersonEmail!="" )
                    //        {
                    //            sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                    //        }
                            
                    //    }
                    //}
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Change Management Request Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                     return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMOMEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool SendChangeEditMgmtmail(int Idmgmt, string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select IdMgmt,ChangeRequestNo,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,"
                    + " (case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus from t_changemanagement"
                    + " where IdMgmt='" + Idmgmt + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                ManagementChangeModels objType = new ManagementChangeModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "," + objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString());



                    sHeader = "<tr><td colspan=3><b>Change In:<b></td> <td colspan=3>"
                        + objGlobalData.GetDropdownitemById(dsManagementList.Tables[0].Rows[0]["ChangeIn"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Change Requested No:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ChangeRequestNo"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Change Initiated Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details of Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["DetailsOfChange"].ToString() + "</td></tr>"

                       + "<tr><td colspan=3><b>Reasons for Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ResonForChange"].ToString()
                       + "</td></tr>"
                    + "<tr><td colspan=3><b>Impact:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Impact"].ToString() + "</td></tr>";


                    sSqlstmt = "select Id,IdMgmt,Action,TargetDate,PersonResponsible,Action_Status"
                                       + " from t_changemanagement_actions where IdMgmt='" + dsManagementList.Tables[0].Rows[0]["IdMgmt"].ToString()
                                       + "' order by Id";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=6><label><b>Actions to be taken to mitigate the impact on consequences </b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th>Action</th>"
                            + "<th>Target Date</th>"
                            + "<th>Person Responsible</th>"
                            + "<th>Action Status</th>"
                            + "</tr>";

                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td>" + dsItems.Tables[0].Rows[i]["Action"].ToString() + "</td>"
                                + "<td> " + Convert.ToDateTime(dsItems.Tables[0].Rows[i]["TargetDate"].ToString()).ToString("yyyy-MM-dd") + " </td>"
                                 + " <td>" + objGlobalData.GetEmpHrNameById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString()) + "</td>"
                                + "<td> " + dsItems.Tables[0].Rows[i]["Action_Status"].ToString() + " </td>"
                                                       + " </tr>";
                            sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                        }
                    }
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Change Management Request Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMOMEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool SendChangeMgmtApprovalmail(string IdMgmt, string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,"
                    + " (case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus from t_changemanagement"
                    + " where IdMgmt='" + IdMgmt + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                ManagementChangeModels objType = new ManagementChangeModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["LoggedBy"].ToString()); 

                    if (objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) != null)
                    {
                        if (sCCEmailIds != "" && sCCEmailIds != null)
                        {
                            sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString());
                        }
                        else
                        {
                            sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString());
                        }

                    }

                    sHeader = "<tr><td colspan=3><b>Change In:<b></td> <td colspan=3>"
                        + objGlobalData.GetDropdownitemById(dsManagementList.Tables[0].Rows[0]["ChangeIn"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ApproveStatus"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Change Initiated Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Details of Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["DetailsOfChange"].ToString() + "</td></tr>"

                       + "<tr><td colspan=3><b>Reasons for Change:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ResonForChange"].ToString()
                       + "</td></tr>"
                    + "<tr><td colspan=3><b>Impact:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Impact"].ToString() + "</td></tr>";


                    //sSqlstmt = "select Id,IdMgmt,Action,TargetDate,PersonResponsible,Action_Status"
                    //                   + " from t_changemanagement_actions where IdMgmt='" + dsManagementList.Tables[0].Rows[0]["IdMgmt"].ToString()
                    //                   + "' order by Id";

                    //DataSet dsItems = new DataSet();
                    //dsItems = objGlobalData.Getdetails(sSqlstmt);

                    //if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    //{
                    //    sInformation = "<tr> "
                    //        + "<td colspan=6><label><b>Actions to be taken to mitigate the impact on consequences </b></label> </td> </tr>"
                    //        + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                    //        + "<th>Sl. No.</th>"
                    //        + "<th>Action</th>"
                    //        + "<th>Target Date</th>"
                    //        + "<th>Person Responsible</th>"
                    //        + "<th>Action Status</th>"
                    //        + "</tr>";

                    //    for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                    //    {
                    //        sInformation = sInformation + "<tr>"
                    //            + " <td>" + (i + 1) + "</td>"
                    //            + " <td>" + dsItems.Tables[0].Rows[i]["Action"].ToString() + "</td>"
                    //            + "<td> " + Convert.ToDateTime(dsItems.Tables[0].Rows[i]["TargetDate"].ToString()).ToString("yyyy-MM-dd") + " </td>"
                    //             + " <td>" + objGlobalData.GetEmpHrNameById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString()) + "</td>"
                    //            + "<td> " + dsItems.Tables[0].Rows[i]["Action_Status"].ToString() + " </td>"
                    //                                   + " </tr>";
                    //        string sPersonEmail = objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                    //        sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                    //        sPersonEmail = sPersonEmail.Trim();
                    //        sPersonEmail = sPersonEmail.TrimEnd(',');
                    //        sPersonEmail = sPersonEmail.TrimStart(',');
                    //        if(sPersonEmail != "" && sPersonEmail != null)
                    //        {
                    //            sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                    //        }
                            
                    //    }
                    //}
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Change Management Request Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMOMEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateChangeManagement(ManagementChangeModels objManagement)
        {
            try
            {
                string[] name = objManagement.ApprovedBy.Split(',');
                int count = name.Length;
                string user = "";
               
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "update t_changemanagement set LoggedBy='" + user + "', "
                    + "ChangeRequestedBy='" + objManagement.ChangeRequestedBy + "', ChangeIn='" + objManagement.ChangeIn
                    + "', DetailsOfChange='" + objManagement.DetailsOfChange + "', ResonForChange='" + objManagement.ResonForChange + "', Impact='"
                    + objManagement.Impact + "', ApprovedBy='" + objManagement.ApprovedBy + "',"
                    + "ApproverCount='" +count + "'," +  "ChangeType='" + objManagement.ChangeType + "', RiskLevel='" + objManagement.RiskLevel
                    + "' ,branch='" + objManagement.branch + "', Department='" + objManagement.Department + "', Location='" + objManagement.Location + "'";

                sSqlstmt = sSqlstmt + " where IdMgmt='" + objManagement.IdMgmt + "';";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateChangeManagement: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateManagementChangeAction(ManagementChangeModels objManagementModels)
        {
            try
            {
                string sTargetDate = objManagementModels.TargetDate.ToString("yyyy-MM-dd HH':'mm':'ss");


                string sSqlstmt = "update t_changemanagement_actions set Action ='" + objManagementModels.Action + "', TargetDate='" + sTargetDate + "', "
                    + "PersonResponsible='" + objManagementModels.PersonResponsible + "', Action_Status='" + objManagementModels.Action_Status + "'";


                sSqlstmt = sSqlstmt + " where Id='" + objManagementModels.Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateManagementChangeAction: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddChangeMgmtActions(ChangeManagementModelsList objManagementModelsList, int IdMgmt)
        {
            try
            {
                string sSqlstmt = "";
                sSqlstmt = "delete from t_changemanagement_actions where IdMgmt='" + IdMgmt + "'; ";
                for (int i = 0; i < objManagementModelsList.ChangeMgmtList.Count; i++)
                {
                    if (objManagementModelsList.ChangeMgmtList[i].Action != null)
                    {   
                        sSqlstmt = sSqlstmt + " insert into t_changemanagement_actions (IdMgmt,Action,PersonResponsible,Action_Status";

                        string sFields = "", sFieldValue = "";
                        if (objManagementModelsList.ChangeMgmtList[i].TargetDate != null && objManagementModelsList.ChangeMgmtList[i].TargetDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sFields = sFields + ", TargetDate";
                            sFieldValue = sFieldValue + ", '" + objManagementModelsList.ChangeMgmtList[i].TargetDate.ToString("yyyy/MM/dd") + "'";
                        }
                        if (objManagementModelsList.ChangeMgmtList[i].ActionCompletionDate != null && objManagementModelsList.ChangeMgmtList[i].ActionCompletionDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sFields = sFields + ", ActionCompletionDate";
                            sFieldValue = sFieldValue + ", '" + objManagementModelsList.ChangeMgmtList[i].ActionCompletionDate.ToString("yyyy/MM/dd") + "'";
                        }
                        sSqlstmt = sSqlstmt + sFields;
                        sSqlstmt = sSqlstmt + ") values('" + IdMgmt + "','" + objManagementModelsList.ChangeMgmtList[i].Action
                          + "','" + objManagementModelsList.ChangeMgmtList[i].PersonResponsible + "','" + objManagementModelsList.ChangeMgmtList[i].Action_Status + "'";
                        sSqlstmt = sSqlstmt + sFieldValue + ");";
                    }
                }

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    UpdateChangeStatus(IdMgmt);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChangeMgmtActions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunChangeManagementRequestApproveOrReject(string IdMgmt, int  iStatus)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                if (iStatus == 2)
                {
                    string sSSqlstmt = "update t_changemanagement set ApproveStatus ='2', ApproveOrRejectOn='" + sApprovedDate + "' where IdMgmt='" + IdMgmt + "'";

                    if (objGlobalData.ExecuteQuery(sSSqlstmt))
                    {
                       return SendChangeMgmtApprovalmail(IdMgmt, "Change Management Approval Status");
                    }
                }

                else
                {

                    string sSqlstmt1 = "update t_changemanagement set ApproverCount=ApproverCount-1 where IdMgmt='" + IdMgmt + "'";
                    string user = "";

                    user = objGlobalData.GetCurrentUserSession().empid;

                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount from t_changemanagement where IdMgmt='" + IdMgmt + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_changemanagement set ApproveStatus ='1', ApproveOrRejectOn='" + sApprovedDate + "' where IdMgmt='" + IdMgmt + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_changemanagement set Approvers= concat(Approvers,',','" + user + "') where idMgmt='" + IdMgmt + "'";
                                    objGlobalData.ExecuteQuery(Sql1);
                                    SendChangeMgmtApprovalmail(IdMgmt, "Change Management Approval Status");
                                    return true;

                                }
                            }
                            else
                            {
                                string Sql1 = "update t_changemanagement set Approvers= concat(Approvers,',','" + user + "') where idMgmt='" + IdMgmt + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunChangeManagementRequestApproveOrReject: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateManagementActionPlan(ManagementChangeModels objManagementModels)
        {
            try
            {
                string sTargetDate = objManagementModels.TargetDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCompletionDate = objManagementModels.ActionCompletionDate.ToString("yyyy-MM-dd HH':'mm':'ss");


                string sSqlstmt = "update t_changemanagement_actions set Action ='" + objManagementModels.Action + "', TargetDate='" + sTargetDate + "', "
                    + "PersonResponsible='" + objManagementModels.PersonResponsible + "', Action_Status='" + objManagementModels.Action_Status + "', ActionCompletionDate='" + sCompletionDate + "'";
                sSqlstmt = sSqlstmt + " where Id='" + objManagementModels.Id + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    UpdateChangeStatus(objManagementModels.Id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateManagementActionPlan: " + ex.ToString());
            }
            return false;
        }


        internal bool UpdateChangeStatus(int IdMgmt)
        {
            string sql2 = "";

            string sql = "select Action_Status from t_changemanagement_actions where IdMgmt='" + IdMgmt + "'";
            DataSet dsManagementChange = objGlobalData.Getdetails(sql);
            int count = 0;
            int rowcount = dsManagementChange.Tables[0].Rows.Count;
            if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsManagementChange.Tables[0].Rows.Count; i++)
                {
                    if (dsManagementChange.Tables[0].Rows[i]["Action_Status"].ToString() == "Completed")
                    {
                        count++;
                        continue;
                    }
                    else
                    {

                    }
                }
                if (count == rowcount)
                {
                    sql2 = "update t_changemanagement set ChangeStatus=1,CompletionDate='" + DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where IdMgmt='" + IdMgmt + "'";
                    return objGlobalData.ExecuteQuery(sql2);
                }
            }
            return true;
        }
    }

   

    public class ChangeManagementModelsList
    {
        public List<ManagementChangeModels> ChangeMgmtList { get; set; }
    }
}