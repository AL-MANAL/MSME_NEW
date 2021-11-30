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
    public class MgmtDocumentsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Mgmt Trans Id")]
        public int idMgmtTrans { get; set; }

        [Display(Name = "Annexure Trans Id")]
        public int idAnnexTrans { get; set; }

        [Display(Name = "Mgmt Id")]
        public int idMgmt { get; set; }

        [Display(Name = "Document Level")]
        public string DocLevels { get; set; }

        [Display(Name = "Document Type")]
        public string Doctype { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }
        public string Company_N { get; set; }

        [Display(Name = "Standards")]
        public string ISOStds { get; set; }

        [Display(Name = "Applicable Clauses")]
        public string AppClauses { get; set; }

        [Required]
        [Display(Name = "Document Ref.")]
        [System.Web.Mvc.Remote("doesDocRefExist", "MgmtDocuments", HttpMethod = "POST", ErrorMessage = "Document Reference ID already exists. Please enter a different Reference ID.")]
        public string DocRef { get; set; }

        [Required]
        [Display(Name = "Document Name")]
        [System.Web.Mvc.Remote("doesDocNameExist", "MgmtDocuments", HttpMethod = "POST", ErrorMessage = "Document name already exists. Please enter a different name.")]
        public string DocName { get; set; }

        [Display(Name = "Issue No.")]
        public string IssueNo { get; set; }

        [Required]
        [Display(Name = "Rev No.")]
        public string RevNo { get; set; }

        [Display(Name = "Preparer")]
        public string PreparedBy { get; set; }

        [Display(Name = "Reviewer")]
        public string ReviewedBy { get; set; }
        public string ReviewedById { get; set; }

        [Display(Name = "Document Date")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Document Upload")]
        public string DocUploadPath { get; set; }

        [Display(Name = "IdAnnexure")]
        public int idAnnexure { get; set; }

        [Display(Name = "Trans Date")]
        public DateTime TransDate { get; set; }

        [Display(Name = "Approver")]
        public string ApprovedBy { get; set; }
        public string ApprovedById { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime ReviewedDate { get; set; }

        [Display(Name = "Approved Status")]
        public string Approved_Status { get; set; }
        public string Approved_StatusId { get; set; }

        [Display(Name = "Approved Status")]
        public string Reviewed_Status { get; set; }
        public string Reviewed_StatusId { get; set; }

        [Display(Name = "Uploaded By")]
        public string UploadedBy { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }
        public string Dept { get; set; }

        public string ApprovedStatus { get; set; }

        [Display(Name = "View By")]
        public string view_by { get; set; }

        [Display(Name = "Doc Retention")]
        public string DocRetention { get; set; }

        [Display(Name ="Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "DCR No.")]
        public string dcr_no { get; set; }

        public string ReviewRejector { get; set; }
        public string ApprovalRejector { get; set; }
        public string Reviewers { get; set; }
        public string Approvers { get; set; }

        internal bool FunDeleteMgmtDoc(string sidMgmt)
        {
            try
            {
                string sSqlstmt = "update t_mgmt_documents set Status=0 where idMgmt='" + sidMgmt + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteMgmtDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteAnnexure(string sidAnnexure)
        {
            try
            {
                string sSqlstmt = "update t_mgmt_annexure set Status=0 where idAnnexure='" + sidAnnexure + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAnnexure: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddMgmtDocuments(MgmtDocumentsModels objMgmtDocuments, HttpPostedFileBase filepath)
        {
            try
            {
                string sDocDate = objMgmtDocuments.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string[] name = objMgmtDocuments.ReviewedBy.Split(',');
                int revcount = name.Length;
                string[] name1 = objMgmtDocuments.ApprovedBy.Split(',');
                int appcount = name1.Length;
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_mgmt_documents (DocLevels,Department, Doctype, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                + " DocUploadPath, ApprovedBy, UploadedBy,view_by,ReviewerCount,ApproverCount,branch,Location,dcr_no) values('" + objMgmtDocuments.DocLevels + "','" + objMgmtDocuments.Department + "','" + objMgmtDocuments.Doctype + "','" + objMgmtDocuments.ISOStds + "','" + objMgmtDocuments.AppClauses
                + "','" + objMgmtDocuments.DocRef + "','" + objMgmtDocuments.DocName + "','" + objMgmtDocuments.IssueNo + "','" + objMgmtDocuments.RevNo
                + "','" + objMgmtDocuments.PreparedBy + "','" + objMgmtDocuments.ReviewedBy + "','" + sDocDate + "','" + objMgmtDocuments.DocUploadPath
                + "','" + objMgmtDocuments.ApprovedBy + "','" + user + "','" + objMgmtDocuments.view_by + "','" + revcount + "','" + appcount + "','" + objMgmtDocuments.branch + "','" + objMgmtDocuments.Location + "','" + objMgmtDocuments.dcr_no + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ApprovedBy);
                    if (sEmailid != null && sEmailid != "")
                    {
                        string sExtraMsg = "Attached is the document Pending for your Review, Document Name: " + objMgmtDocuments.DocName + "," + " Prepared by:" + objGlobalData.GetMultiHrEmpNameById(objMgmtDocuments.PreparedBy);

                        //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetEmpHrNameById(objMgmtDocuments.ApprovedBy), "mgmtdoc", sExtraMsg);

                        // sEmailCCList = sEmailCCList.Trim(',');
                        sEmailid = sEmailid.Trim(',');
                        //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        return objGlobalData.SendmailNew(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], filepath, "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMgmtDocuments: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateMgmtDocuments(MgmtDocumentsModels objMgmtDocuments, HttpPostedFileBase filepath, string OldIssueNo, string OldRevNo)
        {
            try
            {
                string[] name = objMgmtDocuments.ReviewedBy.Split(',');
                int revcount = name.Length;
                string[] name1 = objMgmtDocuments.ApprovedBy.Split(',');
                int appcount = name1.Length;

                string sDocDate = objMgmtDocuments.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_mgmt_documents set DocLevels ='" + objMgmtDocuments.DocLevels + "', Doctype='" + objMgmtDocuments.Doctype + "', "
                    + "ISOStds='" + objMgmtDocuments.ISOStds + "', Department='" + objMgmtDocuments.Department + "', AppClauses='" + objMgmtDocuments.AppClauses
                    + "', IssueNo='" + objMgmtDocuments.IssueNo + "', RevNo='" + objMgmtDocuments.RevNo
                    + "', PreparedBy='" + objMgmtDocuments.PreparedBy + "', ReviewedBy='" + objMgmtDocuments.ReviewedBy + "', DocDate='" + sDocDate + "', ApprovedBy='"
                    + objMgmtDocuments.ApprovedBy + "', UploadedBy='" + objMgmtDocuments.UploadedBy + "', view_by='" + objMgmtDocuments.view_by
                    + "', ReviewerCount='" + revcount + "', ApproverCount='" + appcount + "', branch='" + objMgmtDocuments.branch + "', Location='" + objMgmtDocuments.Location + "', dcr_no='" + objMgmtDocuments.dcr_no + "'"
                    + ",Approved_Status='0',Reviewed_Status='0', ApprovedDate=NULL,ReviewedDate=NULL,Reviewers='0',ReviewRejector='0', Approvers='0',ApprovalRejector='0'";

                if (objMgmtDocuments.DocUploadPath != null)
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objMgmtDocuments.DocUploadPath + "', approved_status=0";
                }
                sSqlstmt = sSqlstmt + " where idMgmt='" + objMgmtDocuments.idMgmt + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                    {

                    //if (objMgmtDocuments.ReviewRejector != "0" || objMgmtDocuments.ReviewRejector != null || objMgmtDocuments.ApprovalRejector != "0" || objMgmtDocuments.ApprovalRejector != null)
                    //{
                    string sSqlstmt1 = "update t_mgmt_documents set Approved_Status='0',Reviewed_Status='0', ApprovedDate=NULL,ReviewedDate=NULL,Reviewers='0',ReviewRejector='0', Approvers='0',ApprovalRejector='0'";

                    sSqlstmt1 = sSqlstmt1 + " where idMgmt='" + objMgmtDocuments.idMgmt + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ApprovedBy);
                        if (sEmailid != null && sEmailid != "")
                        {
                            string sExtraMsg = "Attached is the document Pending for your Review, Document Name: " + objGlobalData.GetDocNameByID(objMgmtDocuments.idMgmt.ToString()) + "," + "Prepared by:" + objGlobalData.GetMultiHrEmpNameById(objMgmtDocuments.PreparedBy);

                            //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetEmpHrNameById(objMgmtDocuments.ReviewedBy), "mgmtdoc", sExtraMsg);

                            // sEmailCCList = sEmailCCList.Trim(',');
                            sEmailid = sEmailid.Trim(',');
                            //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                            return objGlobalData.SendmailNew(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], filepath, "", "");
                        }
                    }
                    //}
                }
                //if (objMgmtDocuments.IssueNo != OldIssueNo || objMgmtDocuments.RevNo != OldRevNo)
                //{
                //    string sql1 = "update t_documentchangerequest set ChangeStatus=2 where DocRef='" + objMgmtDocuments.idMgmt + "'"
                //   + "and DocName='" + objMgmtDocuments.idMgmt + "' and ApproveStatus=1 and ChangeStatus=0 and Active=1";
                //    objGlobalData.ExecuteQuery(sql1);

                //    string sSqlstmt1 = "update t_mgmt_documents set Approved_Status='0',Reviewed_Status='0', ApprovedDate=NULL,ReviewedDate=NULL";
                //    sSqlstmt1 = sSqlstmt1 + " where idMgmt='" + objMgmtDocuments.idMgmt + "'";
                //    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                //    {
                //        string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ApprovedBy);
                //        if (sEmailid != null && sEmailid != "")
                //        {
                //            string sExtraMsg = "Attached is the document Pending for your Review, Document Name: " +objGlobalData.GetDocNameByID(objMgmtDocuments.idMgmt.ToString()) + "," + "Prepared by:" + objGlobalData.GetMultiHrEmpNameById(objMgmtDocuments.PreparedBy);

                //            //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                //            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetEmpHrNameById(objMgmtDocuments.ReviewedBy), "mgmtdoc", sExtraMsg);

                //            // sEmailCCList = sEmailCCList.Trim(',');
                //            sEmailid = sEmailid.Trim(',');
                //            //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                //            return objGlobalData.SendmailNew(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], filepath, "", "");
                //        }
                //    }


                //}

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtDocuments: " + ex.ToString());
            }

            return true;
        }

        //internal bool FunUpdateMgmtDocumentsApproval(string sidMgmt, int iStatus, System.IO.Stream fsSource, string filename, string DocName, string DocRef)
        //{

        //    try
        //    {
        //        string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

        //        string sql1 = "update t_documentchangerequest set ChangeStatus=1 where DocRef='" + sidMgmt + "'"
        //            + "and DocName='" + sidMgmt + "' and ApproveStatus=1 and Active=1";
        //        objGlobalData.ExecuteQuery(sql1);

        //        string sSqlstmt = "update t_mgmt_documents set Approved_Status ='" + iStatus + "', ApprovedDate='" + sApprovedDate + "' where idMgmt='" + sidMgmt + "'";

        //        if (objGlobalData.ExecuteQuery(sSqlstmt))
        //        {
        //            DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy,(case when Approved_Status='0' then 'Pending' when Approved_Status='1' then 'Approved' when Approved_Status='2' then 'Rejected' end) as Doc_Status  from t_mgmt_documents where idMgmt='" + sidMgmt + "'");

        //            if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
        //            {
        //                string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString());
        //                sEmailid = Regex.Replace(sEmailid, ",+", ",");
        //                sEmailid = sEmailid.Trim();
        //                sEmailid = sEmailid.TrimEnd(',');
        //                sEmailid = sEmailid.TrimStart(',');
        //                string sExtraMsg = "Document Status: " + dsDocument.Tables[0].Rows[0]["Doc_Status"].ToString() + ", Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

        //                string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
        //                    + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
        //                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
        //                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc3", sExtraMsg);

        //                return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtDocumentsApproval: " + ex.ToString());
        //    }

        //    return false;
        //}
        internal bool FunMgmtDocumentApproveOrReject(string sidMgmt, int sStatus, System.IO.Stream fsSource, string filename, string DocName, string DocRef)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                if (sStatus == 1)
                {

                    string sSqlstmt1 = "update t_mgmt_documents set ApproverCount=ApproverCount-1 where idMgmt='" + sidMgmt + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount from t_mgmt_documents where idMgmt='" + sidMgmt + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_mgmt_documents set Approved_Status ='1', ApprovedDate='" + sApprovedDate + "' where idMgmt='" + sidMgmt + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {

                                    string sqlDCR = "update t_documentchangerequest set ChangeStatus=1 where DocRef='" + sidMgmt + "' and DocName='" + sidMgmt + "' and ApproveStatus=1 and Active=1";
                                    objGlobalData.ExecuteQuery(sqlDCR);

                                    string Sql1 = "update t_mgmt_documents set Approvers= concat(Approvers,',','" + user + "') where idMgmt='" + sidMgmt + "'";
                                    objGlobalData.ExecuteQuery(Sql1);
                                    SendMgmtApprovalmail(sidMgmt, fsSource, filename, "Internal Document is Approved");
                                    return true;
                                }
                            }
                            else
                            {
                                string Sql1 = "update t_mgmt_documents set Approvers= concat(Approvers,',','" + user + "')"
                                + " where idMgmt='" + sidMgmt + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
                else
                {
                    string Sql1 = "update t_mgmt_documents set Approved_Status='" + sStatus + "',ApprovalRejector= concat(ApprovalRejector,',','" + user + "') where idMgmt='" + sidMgmt + "'";
                    objGlobalData.ExecuteQuery(Sql1);
                    //SendMgmtRejectionmail(sidMgmt, fsSource, filename, "Internal Document Rejected");
                    SendMgmtRejectionmail(sidMgmt, "Internal Document Rejected");
                    return true;                    
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunMgmtDocumentApproveOrReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendMgmtApprovalmail(string sidMgmt, System.IO.Stream fsSource, string filename, string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Internal Document has been approved, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
                        + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc3", sExtraMsg);

                    return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocChangeApprovalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool SendMgmtRejectionmail(string sidMgmt,string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Internal Document has been Rejected, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
                        + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc4", sExtraMsg);

                    //return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                    return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList,"");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMgmtRejectionmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunMgmtDocumentsReviewApproveOrReject(string sidMgmt, int sStatus, System.IO.FileStream fsSource, string filename, string DocName, string DocRef)
        {
            try
            {
                string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;
                if (sStatus == 1)
                {
                    string sSqlstmt1 = "update t_mgmt_documents set ReviewerCount=ReviewerCount-1 where idMgmt='" + sidMgmt + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ReviewerCount from t_mgmt_documents where idMgmt='" + sidMgmt + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ReviewerCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_mgmt_documents set Reviewed_Status ='1', ReviewedDate='" + sReviewedDate + "' where idMgmt='" + sidMgmt + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_mgmt_documents set Reviewers= concat(Reviewers,',','" + user + "') where idMgmt='" + sidMgmt + "'";
                                    objGlobalData.ExecuteQuery(Sql1);

                                    SendMgmtReviewApprovalmail(sidMgmt, fsSource, filename, "Internal Document is Reviewed");
                                    return true;
                                }
                            }
                            else
                            {
                                string Sql1 = "update t_mgmt_documents set Reviewers= concat(Reviewers,',','" + user + "')"
                                + " where idMgmt='" + sidMgmt + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
                else
                {
                    string Sql1 = "update t_mgmt_documents set Reviewed_Status='" + sStatus + "',ReviewRejector= concat(ReviewRejector,',','" + user + "') where idMgmt='" + sidMgmt + "'";
                    objGlobalData.ExecuteQuery(Sql1);
                    //SendMgmtReviewRejectionmail(sidMgmt, fsSource, filename, "Internal Document Rejected after Reviewing ");
                    SendMgmtReviewRejectionmail(sidMgmt, "Internal Document Rejected after Reviewing ");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtDocumentsApproval: " + ex.ToString());
            }

            return false;
        }

        internal bool SendMgmtReviewApprovalmail(string sidMgmt, System.IO.FileStream fsSource, string filename, string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Internal Document has been Reviewed, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
                        + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc5", sExtraMsg);

                    return objGlobalData.SendmailReview(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocChangeApprovalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool SendMgmtReviewRejectionmail(string sidMgmt, string sMessage = "")
        {
            try
            {
                DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "Internal Document Review has been Rejected, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
                        + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                    objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc6", sExtraMsg);

                    //return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                    return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMgmtRejectionmail: " + ex.ToString());
            }
            return false;
        }


        //internal bool FunUpdateMgmtDocumentsReview(string sidMgmt, System.IO.FileStream fsSource,string filename)
        //{
        //    try
        //    {
        //        string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

        //        string sSqlstmt = "update t_mgmt_documents set Reviewed_Status='1', ReviewedDate='" + sReviewedDate + "' where idMgmt='" + sidMgmt + "'";

        //        if (objGlobalData.ExecuteQuery(sSqlstmt))
        //        {
        //            DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
        //            if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
        //            {
        //                string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
        //                if (sEmailid != null && sEmailid != "")
        //                {
        //                    string sExtraMsg = "Attached document for approval, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

        //                    string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString());
        //                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
        //                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()), "mgmtdoc2", sExtraMsg);

        //                    return objGlobalData.SendmailReview(sEmailid,fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
        //                }
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtDocumentsApproval: " + ex.ToString());
        //    }

        //    return false;
        //}

        internal bool FunAddComments(string sEmpId, string sComments, string sidMgmt)
        {
            try
            {
                string sCommentDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into  t_mgmt_docs_comments (idMgmt, empId, Comments, CommentDate) values('" + sidMgmt + "','" + sEmpId + "','" + sComments + "','"
                    + sCommentDate + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddComments: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddAnnexure(MgmtDocumentsModels objMgmtDocuments, HttpPostedFileBase filepath)
        {
            try
            {
                string sDocDate = objMgmtDocuments.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_mgmt_annexure (MgmtId, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                + " DocUploadPath, ApprovedBy,Logged_by) values('" + objMgmtDocuments.idMgmt + "','" + objMgmtDocuments.DocRef + "','" + objMgmtDocuments.DocName + "','"
                + objMgmtDocuments.IssueNo + "','" + objMgmtDocuments.RevNo + "','" + objMgmtDocuments.PreparedBy + "','" + objMgmtDocuments.ReviewedBy + "','"
                + sDocDate + "','" + objMgmtDocuments.DocUploadPath + "','" + objMgmtDocuments.ApprovedBy + "','" + objGlobalData.GetCurrentUserSession().empid + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ApprovedBy);
                    if (sEmailid != null && sEmailid != "")
                    {
                        string sExtraMsg = "Attached is the Annexure document Pending for your Approval, Document Name: " + objMgmtDocuments.DocName + "," + "Prepared by:" + objGlobalData.GetMultiHrEmpNameById(objMgmtDocuments.PreparedBy);

                        //string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.ReviewedBy) + "," + objGlobalData.GetMultiHrEmpEmailIdById(objMgmtDocuments.UploadedBy);
                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetMultiHrEmpNameById(objMgmtDocuments.ApprovedBy), "annexure", sExtraMsg);

                        // sEmailCCList = sEmailCCList.Trim(',');
                        sEmailid = sEmailid.Trim(',');
                        //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        return objGlobalData.SendmailNew(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], filepath, "", "");
                    }
                }


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAnnexure: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateAnnexure(MgmtDocumentsModels objMgmtDocuments)
        {
            try
            {
                string sDocDate = objMgmtDocuments.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_mgmt_annexure set MgmtId ='" + objMgmtDocuments.idMgmt + "', DocRef='" + objMgmtDocuments.DocRef
                    + "', DocName='" + objMgmtDocuments.DocName + "', IssueNo='" + objMgmtDocuments.IssueNo + "', RevNo='" + objMgmtDocuments.RevNo
                    + "', PreparedBy='" + objMgmtDocuments.PreparedBy + "', ReviewedBy='" + objMgmtDocuments.ReviewedBy + "', DocDate='" + sDocDate + "',ApprovedBy='"
                    + objMgmtDocuments.ApprovedBy + "' ";
                if (objMgmtDocuments.DocUploadPath != null)
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objMgmtDocuments.DocUploadPath + "'";
                }
                sSqlstmt = sSqlstmt + " where idAnnexure=" + objMgmtDocuments.idAnnexure;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAnnexure: " + ex.ToString());
            }

            return false;
        }

        internal string GetDocName(int idMgmt)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Doctype from t_mgmt_documents where idMgmt='" + idMgmt + "' and Status=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return dsDoc.Tables[0].Rows[0]["Doctype"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocName: " + ex.ToString());
            }
            return "";
        }

        public string GetDocumentTypeIdbyMgtId(string Id)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Doctype from t_mgmt_documents where idMgmt='" + Id + "'");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return dsDoc.Tables[0].Rows[0]["Doctype"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocumentTypeIdbyMgtId: " + ex.ToString());
            }
            return "";
        }      


        internal bool CheckForDocNameExists(string sDocName)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Doctype from t_mgmt_documents where DocName='" + sDocName + "' and Status=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForDocNameExists: " + ex.ToString());
            }
            return true;
        }

        internal bool CheckForDocRefExists(string sDocRef)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select DocRef from t_mgmt_documents where DocRef='" + sDocRef + "' and Status=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForDocRefExists: " + ex.ToString());
            }
            return true;
        }

        internal MultiSelectList GetMultiISOClauseList(string ISOStdId)
        {
            ISOStdClauseList lstClause = new ISOStdClauseList();
            lstClause.lstISOStdClause = new List<ISOStdClause>();
            try
            {
                string sSqlstmt = "select clause_id, clause_no, clause_desc from t_isoclause where stdid in(" + ISOStdId + ") ";

                DataSet dsClause = objGlobalData.Getdetails(sSqlstmt);
                if (dsClause.Tables.Count > 0 && dsClause.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsClause.Tables[0].Rows.Count; i++)
                    {
                        ISOStdClause Clause = new ISOStdClause()
                        {
                            clause_id = dsClause.Tables[0].Rows[i]["clause_id"].ToString(),
                            clause = dsClause.Tables[0].Rows[i]["clause_no"].ToString() + ". " + dsClause.Tables[0].Rows[i]["clause_desc"].ToString()
                        };

                        lstClause.lstISOStdClause.Add(Clause);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiISOClauseList: " + ex.ToString());
            }

            return new MultiSelectList(lstClause.lstISOStdClause, "clause_id", "clause");
        }

        public string GetMultiISOClauseNameById(string sClause_id)
        {
            try
            {
                if (sClause_id != "")
                {
                    DataSet dsClause = objGlobalData.Getdetails("select clause_no, clause_desc from t_isoclause where clause_id in (" + sClause_id + ")");
                    if (dsClause.Tables.Count > 0 && dsClause.Tables[0].Rows.Count > 0)
                    {
                        string sClause = "";
                        for (int i = 0; i < dsClause.Tables[0].Rows.Count; i++)
                        {
                            sClause = sClause + (dsClause.Tables[0].Rows[i]["clause_no"].ToString() + ". " + dsClause.Tables[0].Rows[i]["clause_desc"].ToString()) + ", ";
                        }
                        return sClause.Trim().TrimEnd(',');
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiISOClauseNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetIssueNoHistory(string idMgmt)
        {
            try
            {
                if (idMgmt != "")
                {
                    DataSet DsRev = objGlobalData.Getdetails("select IssueNo from t_mgmt_documents where idMgmt='" + idMgmt + "' and Status=1");
                    if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
                        {
                            IssueNo = DsRev.Tables[0].Rows[i]["IssueNo"].ToString();
                        }
                        return IssueNo;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetIssueNoHistory: " + ex.ToString());
            }
            return "";
        }

        public string GetRevNoHistory(string idMgmt)
        {
            try
            {
                if (idMgmt != "")
                {
                    DataSet DsRev = objGlobalData.Getdetails("select RevNo from t_mgmt_documents where idMgmt='" + idMgmt + "' and Status=1");
                    if (DsRev.Tables.Count > 0 && DsRev.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < DsRev.Tables[0].Rows.Count; i++)
                        {
                            RevNo = DsRev.Tables[0].Rows[i]["RevNo"].ToString();
                        }
                        return RevNo;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRevNoHistory: " + ex.ToString());
            }
            return "";
        }

        public string GetDocumentTypeIdbyName(string item_desc)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id from dropdownitems where item_desc='" + item_desc + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDocumentTypeIdbyName: " + ex.ToString());
            }
            return "";
        }

        internal bool FunUpdateAnnexureReview(string idAnnexure, System.IO.FileStream fsSource, string filename)
        {
            try
            {
                // string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                //string sSqlstmt = "update t_mgmt_annexure set ReviewStatus='1', Reviewed_date='" + sReviewedDate + "' where idAnnexure='" + idAnnexure + "'";

                string sApproved_date = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_mgmt_annexure set ApprovedStatus='1', Approved_date='" + sApproved_date + "' where idAnnexure='" + idAnnexure + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ApprovedBy,Logged_by from t_mgmt_annexure where idAnnexure='" + idAnnexure + "'");
                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Logged_by"].ToString());
                        if (sEmailid != null && sEmailid != "")
                        {
                            string sExtraMsg = "Attached document Approved, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                            string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["Logged_by"].ToString()), "annexure1", sExtraMsg);

                            //return objGlobalData.SendmailReview(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                            return objGlobalData.SendmailApprove(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAnnexureReview: " + ex.ToString());
            }

            return false;
        }
             


        //Craig code


        internal bool FunUpdateMgmtDocumentsReview(string sidMgmt, System.IO.FileStream fsSource, string filename, int iStatus)
        {
            try
            {
                string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_mgmt_documents set Reviewed_Status='" + iStatus + "', ReviewedDate='" + sReviewedDate + "' where idMgmt='" + sidMgmt + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    DataSet dsDocument = objGlobalData.Getdetails("select DocName,(case when Reviewed_Status='1' then 'Approved' else 'Rejected' end) as Reviewed_Status, " +
                        "PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        if (sEmailid != null && sEmailid != "")
                        {
                            string sExtraMsg = "Document Status: " + dsDocument.Tables[0].Rows[0]["Reviewed_Status"].ToString() + ", Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

                            string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString());
                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()), "mgmtdoc2", sExtraMsg);

                            return objGlobalData.SendmailReview(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtDocumentsApproval: " + ex.ToString());
            }

            return false;
        }
    }

    public class MgmtDocumentsModelsList
    {
        public List<MgmtDocumentsModels> MgmtDocumentsMList { get; set; }
    }

    public class ISOStdClause
    {
        public string clause_id { get; set; }
        public string clause { get; set; }
    }

    public class ISOStdClauseList
    {
        public List<ISOStdClause> lstISOStdClause { get; set; }
    }

    public class DropdownMultiItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }

    public class DropdownMultiItemsList
    {
        public List<DropdownMultiItems> DropdownList { get; set; }
    }
}