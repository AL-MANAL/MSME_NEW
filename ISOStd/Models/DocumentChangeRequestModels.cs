using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;


namespace ISOStd.Models
{
    public class DocumentChangeRequestModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Document Ref")]
        public string DocRef { get; set; }
        
        [Display(Name = "Document Name")]
        public string DocName { get; set; }
        
        [Display(Name = "Changes To be done")]
        public string Changes { get; set; }
        
        [Display(Name = "Requested By")]
        public string RequestedBy { get; set; }
               
        [Display(Name = "Approver")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime ChangeRequestDate { get; set; }
          
        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; }
        
        [Display(Name = "Approval Status")]
        public string ApproveStatus { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Approver Count")]
        public int ApproverCount { get; set; }

        [Display(Name = "Change Status")]
        public string ChangeStatus { get; set; }

        [Display(Name = "Approvers")]
        public string Approvers { get; set; }
        public string ApprovedById { get; set; }

        [Display(Name = "Rejectors")]
        public string Rejector { get; set; }

        [Display(Name = "Attachment")]
        public string upload { get; set; }
        [Required]
        [Display(Name = "Issue No.")]
        public string IssueNo { get; set; }
        [Required]
        [Display(Name = "Rev No.")]
        public string RevNo { get; set; }

        [Display(Name = "Doc Date")]
        public DateTime DocDate { get; set; }

        public string ReivewRejector { get; set; }
        public string ApproveRejector { get; set; }
        public string ChangeStatusId { get; set; }

        internal bool FunDeleteChangeMgmtDoc(string sId)
        {
            try
            {
                string sSqlstmt = "update t_documentchangerequest set Active=0 where Id='" + sId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChangeMgmtDoc: " + ex.ToString());
            }
            return false;
        }

        public string GetDocNameByRef(string idMgmt)
        {
            try
            {
                if (DocRef != "")
                {
                    DataSet DsDoc = objGlobalData.Getdetails("select DocName from t_mgmt_documents where idMgmt='" + idMgmt + "' and Status=1");
                    if (DsDoc.Tables.Count > 0 && DsDoc.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsDoc.Tables[0].Rows.Count; i++)
                        {
                            DocName = DsDoc.Tables[0].Rows[i]["DocName"].ToString();
                        }
                        return DocName;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocNameByRef: " + ex.ToString());
            }
            return "";

        }

        public string GetDocRefByName(string DocName)
        {
            try
            {
                if (DocName != "")
                {
                    DataSet DsDoc = objGlobalData.Getdetails("select DocRef from t_mgmt_documents where DocName='" + DocName + "' and Status=1");
                    if (DsDoc.Tables.Count > 0 && DsDoc.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsDoc.Tables[0].Rows.Count; i++)
                        {
                            DocRef = DsDoc.Tables[0].Rows[i]["DocRef"].ToString();
                        }
                        return DocRef;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocRefByName: " + ex.ToString());
            }
            return "";

        }

        internal bool FunAddDocumentChangeRequest(DocumentChangeRequestModels objDocuments)
        {
            try
            {
                string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                string[] name = objDocuments.ApprovedBy.Split(',');
                int count = name.Length;
                string user = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                user = objGlobalData.GetCurrentUserSession().empid;

                 string sSqlstmt = "insert into t_documentchangerequest (DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate,ApproverCount,upload,IssueNo,RevNo,branch";
                 string sFields = "", sFieldValue = "";

                 if (objDocuments.DocDate != null && objDocuments.DocDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", DocDate";
                    sFieldValue = sFieldValue + ", '" + objDocuments.DocDate.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")values('" + objDocuments.DocRef
                    + "','" + objDocuments.DocName + "','" + objDocuments.Changes + "','" + user + "','" + objDocuments.ApprovedBy + "','" + TodayDate + "','" + count + "','" + objDocuments.upload + "','" + objDocuments.IssueNo + "','" + objDocuments.RevNo + "','" + sBranch + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sHeader = "",REmail="";
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objDocuments.ApprovedBy);
                 
                         REmail = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                    
                    if (sEmailid != null && sEmailid != "")
                    {
                        sHeader = "<tr><td ><b>Document Ref:<b></td> <td >"
                              +objGlobalData.GetDocRefByID(objDocuments.DocRef) + "</td></tr>"
                              + "<tr><td ><b>Document Name:<b></td> <td >" +objGlobalData.GetDocNameByID(objDocuments.DocName) + "</td></tr>"
                              + "<tr><td ><b>Requested On:<b></td> <td >" + TodayDate + "</td></tr>"

                             + "<tr><td ><b>Requested by:<b></td> <td >" + objGlobalData.GetEmpHrNameById(user) + "</td></tr>"

                            + "<tr><td ><b>Changes:<b></td> <td >" + objDocuments.Changes + "</td></tr>";
                      
                        //sInformation = "<tr> "
                        //+ "<td colspan=6><label><b>Document Changes Request Details</b></label> </td> </tr>"
                        //+ "<tr><th>Document Ref:</th>"
                        //+ "<td>"+objDocuments.DocRef+"</td></tr>"
                        //+ "<tr><th>Document Name:</th>"
                        //+ "<td>"+objDocuments.DocName+"</td></tr>"
                        //+ "<tr><th>Changes Requested:</th>"
                        //+ "<td>"+objDocuments.Changes+"</td></tr>"
                        //+ "<tr><th>Requested By:</th>"
                        //+ "<td>"+objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid) + "</td></tr>";
                       
                        //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetMultiHrEmpNameById(objDocuments.ApprovedBy), "Changedoc", sHeader,"", "Document Changes Request Details");

                        // sEmailCCList = sEmailCCList.Trim(',');
                        sEmailid = sEmailid.Trim(',');
                        //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", REmail, "");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocumentChangeRequest: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDocumentChangesRequestApproveOrReject(string Id, int sStatus,string comments)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
              
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                if (sStatus == 1)
                {

                    string sSqlstmt1 = "update t_documentchangerequest set ApproverCount=ApproverCount-1 where Id='" + Id + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount from t_documentchangerequest where Id='" + Id + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_documentchangerequest set ApproveStatus ='1',ChangeStatus='1', ApprovedDate='" + sApprovedDate + "' where Id='" + Id + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_documentchangerequest set Approvers= concat(Approvers,',','" + user + "') where Id='" + Id + "'";
                                    objGlobalData.ExecuteQuery(Sql1);

                                    string Sql2 = "insert into t_documentchangerequest_comments set Id='" + Id + "',CommentBy='" + user + "',Comments='" + comments + "',ApprovalStatus='Approved',ApproveRejectedDate='" + sApprovedDate + "'";
                                    objGlobalData.ExecuteQuery(Sql2);
                                    SendDocChangeApprovalmail(Id, "Document change request Approved");
                                    return true;

                                }
                            }
                            else
                            {
                                string Sql2 = "insert into t_documentchangerequest_comments set Id='" + Id + "',CommentBy='" + user + "',Comments='" + comments + "',ApprovalStatus='Approved',ApproveRejectedDate='" + sApprovedDate + "'";
                                objGlobalData.ExecuteQuery(Sql2);

                                string Sql1 = "update t_documentchangerequest set Approvers= concat(Approvers,',','" + user + "')"
                                + " where Id='" + Id + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
                else
                {
                    string Sql2 = "insert into t_documentchangerequest_comments set Id='" + Id + "',CommentBy='" + user + "',Comments='" + comments + "',ApprovalStatus='Rejected',ApproveRejectedDate='" + sApprovedDate + "'";
                    objGlobalData.ExecuteQuery(Sql2);

                    string Sql1 = "update t_documentchangerequest set ApproveStatus='" + sStatus + "',Rejector= concat(Rejector,',','" + user + "') where Id='" + Id + "'";
                    objGlobalData.ExecuteQuery(Sql1);
                    SendDocChangeRejectmail(Id, comments, "Document change request Rejected");
                    return true;

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDocumentChangesRequestApproveOrReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendDocChangeApprovalmail(string Id, string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select Id,DocRef,DocName,Changes,(case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,RequestedBy,ApprovedBy,ChangeRequestDate from t_documentchangerequest"
                    + " where Id='" + Id + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                DocumentChangeRequestModels objType = new DocumentChangeRequestModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
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
                
                    string sToEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString());
                    string sCCEmailIds =  objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());

                    sHeader = "<tr><td colspan=3><b>Document Ref:<b></td> <td colspan=3>"
                        +objGlobalData.GetDocRefByID(dsManagementList.Tables[0].Rows[0]["DocRef"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Document Name:<b></td> <td colspan=3>" +objGlobalData.GetDocNameByID(dsManagementList.Tables[0].Rows[0]["DocName"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ApproveStatus"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Approval By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Requested On:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeRequestDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Changes:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Changes"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString()) + "</td></tr>";
                     
                   
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Document Change Request Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocChangeApprovalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool SendDocChangeRejectmail(string Id, string comments,string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select Id,DocRef,DocName,Changes,(case when ApproveStatus='1' then 'Approved' else 'Rejected' end) as ApproveStatus,RequestedBy,ApprovedBy,ChangeRequestDate from t_documentchangerequest"
                    + " where Id='" + Id + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                DocumentChangeRequestModels objType = new DocumentChangeRequestModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
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
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string user = "";
                  
                        user = objGlobalData.GetCurrentUserSession().firstname;
                    

                    sHeader = "<tr><td colspan=3><b>Document Ref:<b></td> <td colspan=3>"
                        +objGlobalData.GetDocRefByID(dsManagementList.Tables[0].Rows[0]["DocRef"].ToString()) + "</td></tr>"
                         + "<tr><td colspan=3><b>Document Name:<b></td> <td colspan=3>" + objGlobalData.GetDocNameByID(dsManagementList.Tables[0].Rows[0]["DocName"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["ApproveStatus"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + comments + "</td></tr>"
                           + "<tr><td colspan=3><b>Rejected By:<b></td> <td colspan=3>" + user + "</td></tr>"
                         + "<tr><td colspan=3><b>Requested On:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeRequestDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                          + "<tr><td colspan=3><b>Changes:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Changes"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString()) + "</td></tr>";


                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Document Change Request Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocChangeRejectmail: " + ex.ToString());
            }
            return false;
        }
           
        internal bool FunUpdateChangeRequest(DocumentChangeRequestModels objManagement)
        {
            try
            {
                string[] name = objManagement.ApprovedBy.Split(',');
                int count = name.Length;
                string sSqlstmt = "update t_documentchangerequest set DocRef='" + objManagement.DocRef + "', "
                    + "DocName='" + objManagement.DocName + "', Changes='" + objManagement.Changes
                    + "', ApprovedBy='" + objManagement.ApprovedBy + "', ApproverCount='" + count + "',IssueNo='" + objManagement.IssueNo + "',RevNo='" + objManagement.RevNo + "'";
                if (objManagement.DocDate != null && objManagement.DocDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", DocDate='" + objManagement.DocDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objManagement.upload != null)
                {
                    sSqlstmt = sSqlstmt + ",upload='" + objManagement.upload + "'";
                }

                sSqlstmt = sSqlstmt + " where Id='" + objManagement.Id + "';";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateChangeRequest: " + ex.ToString());
            }
            return false;
        }

        internal bool SendDocumentChangeRequestmail(int Id, string sMessage = "")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  


                string sSqlstmt = "select Id,DocRef,DocName,Changes,RequestedBy,ApprovedBy,ChangeRequestDate from t_documentchangerequest"
                    + " where Id='" + Id + "'";

                DataSet dsManagementList = objGlobalData.Getdetails(sSqlstmt);
                DocumentChangeRequestModels objType = new DocumentChangeRequestModels();


                if (dsManagementList.Tables.Count > 0 && dsManagementList.Tables[0].Rows.Count > 0)
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
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString());


                    sHeader = "<tr><td colspan=3><b>Document Ref:<b></td> <td colspan=3>"
                        + dsManagementList.Tables[0].Rows[0]["DocRef"].ToString() + "</td></tr>"
                         + "<tr><td colspan=3><b>Requested On:<b></td> <td colspan=3>" + Convert.ToDateTime(dsManagementList.Tables[0].Rows[0]["ChangeRequestDate"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
                         + "<tr><td colspan=3><b>Document Name:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["DocName"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Changes:<b></td> <td colspan=3>" + dsManagementList.Tables[0].Rows[0]["Changes"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Change Requested By:<b></td> <td colspan=3>" + objGlobalData.GetEmpHrNameById(dsManagementList.Tables[0].Rows[0]["RequestedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approvers:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsManagementList.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "</td></tr>";
                     
                   
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Document Change Request For Approval");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocumentChangeRequestmail: " + ex.ToString());
            }
            return false;
        }

        //public bool GetDocumentStatus(string DocRef)
        //{
        //    try
        //    {
        //        if (DocRef != "")
        //        {
        //            DataSet DsRev = objGlobalData.Getdetails("select ChangeStatus from t_documentchangerequest where DocRef='" + DocRef + "'"
        //            + " and ApproveStatus=1 and (ChangeStatus=0 or ChangeStatus=2) and Active=1");
        //            if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
        //                {
        //                    ChangeStatus = DsRev.Tables[0].Rows[i]["ChangeStatus"].ToString();
        //                }
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetDocumentStatus: " + ex.ToString());
        //    }
        //    return false;
        //}

        public bool GetDocumentStatus(string DocRef)
        {
            try
            {
                if (DocRef != "")
                {
                    DataSet DsRev = objGlobalData.Getdetails("select ChangeStatus from t_documentchangerequest where DocRef='" + DocRef + "'"
                    + " and ApproveStatus=1 and (ChangeStatus=0 or ChangeStatus=2) and Active=1");
                    if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
                        {
                            ChangeStatus = DsRev.Tables[0].Rows[i]["ChangeStatus"].ToString();
                        }
                        return true;
                    }

                    DataSet Dtset = objGlobalData.Getdetails("select ReviewRejector,ApprovalRejector from t_mgmt_documents where idMgmt='" + DocRef + "' and Status=1");
                    if (Dtset.Tables.Count > 0 && Dtset.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < Dtset.Tables[0].Rows.Count; i++)
                        {
                            ReivewRejector = Dtset.Tables[0].Rows[i]["ReviewRejector"].ToString();
                            ApproveRejector = Dtset.Tables[0].Rows[i]["ApprovalRejector"].ToString();
                        }

                        if (ReivewRejector != "0" || ApproveRejector != "0")
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocumentStatus: " + ex.ToString());
            }
            return false;
        }

        public bool GetDocRefStatus(string idMgmt)
        {
            try
            {
                if (idMgmt != "")
                {
                    DataSet DsRev = objGlobalData.Getdetails("select ChangeStatus from t_documentchangerequest where DocRef='" + idMgmt + "'"
                    + "and ChangeStatus=0 and (ApproveStatus=0 or ApproveStatus=1 ) and Active=1");
                    if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
                        {
                            ChangeStatus = DsRev.Tables[0].Rows[i]["ChangeStatus"].ToString();
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocRefStatus: " + ex.ToString());
            }
            return true;
        }

        public bool GetDocNameStatus(string idMgmt)
        {
            try
            {
                if (idMgmt != "")
                {
                    DataSet DsRev = objGlobalData.Getdetails("select ChangeStatus from t_documentchangerequest where DocName='" + idMgmt + "'"
                    + "and ChangeStatus=0 and (ApproveStatus=0 or ApproveStatus=1 ) and Active=1");
                    if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
                        {
                            ChangeStatus = DsRev.Tables[0].Rows[i]["ChangeStatus"].ToString();
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocNameStatus: " + ex.ToString());
            }
            return true;
        }

    }

    public class DocumentChangeRequestModelsList
    {
        public List<DocumentChangeRequestModels> DocumentChangeList { get; set; }
    }
}