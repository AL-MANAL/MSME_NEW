using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class DocumentReviewModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [Display(Name = "Id")]
        public string id_doc_review { get; set; }

        [Display(Name = "Defined On")]
        public DateTime review_date { get; set; }

        [Display(Name = "Document Level")]
        public string doc_level { get; set; }

        [Display(Name = "Document Type")]
        public string doc_type { get; set; }

        [Display(Name = "Review frequency")]
        public string frequency { get; set; }

        [Display(Name = "Criteria to define the frequency")]
        public string criteria { get; set; }

        [Display(Name = "Approver")]
        public string approvedby { get; set; }
        public string approvedbyId { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Document Ref.")]
        public string DocRef { get; set; }

        [Display(Name = "Document Title")]
        public string DocName { get; set; }

        [Display(Name = "Current Issue No")]
        public string IssueNo { get; set; }

        [Display(Name = "Current Rev. No")]
        public string RevNo { get; set; }

        [Display(Name = "Current Doc. Date")]
        public DateTime DocDate { get; set; }

        public DateTime next_review_date { get; set; }

        [Display(Name = "Status")]
        public string approve_status { get; set; }
        public string approve_statusId { get; set; }
        public DateTime approve_date { get; set; }

        public MultiSelectList GetDocReviewFrequencyList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Document Review Frequency' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetDocReviewFrequencyList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }
        public string GetDocReviewFrequencyById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Document Review Frequency' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetDocReviewFrequencyById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDocReviewCriteriaList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Document Review Criteria' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetDocReviewCriteriaList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }
        public string GetDocReviewCriteriaById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Document Review Criteria' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetDocReviewCriteriaById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunAddDocReview(DocumentReviewModels objModels)
        {
            try
            {
                string sFields = "", sFieldValue = "";

                string sSqlstmt = "insert into t_document_review ( doc_level, doc_type, frequency, criteria, approvedby,loggedby, division";

                if (objModels.review_date != null && objModels.review_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", review_date";
                    sFieldValue = sFieldValue + ", '" + objModels.review_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields + ") values('" + objModels.doc_level + "','" + objModels.doc_type
                + "','" + objModels.frequency + "','" + objModels.criteria + "','" + objModels.approvedby + "','" + objGlobaldata.GetCurrentUserSession().empid + "','" + objModels.division + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int iid_doc_review = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_doc_review))
                {
                    if (iid_doc_review > 0)
                    {
                        SendReviewEmail(iid_doc_review);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddDocReview: " + ex.ToString());
            }
            return false;
        }

        internal bool SendReviewEmail(int iid_doc_review)
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_doc_review, review_date, doc_level, doc_type, frequency," +
                    " criteria, approvedby,loggedby,division from t_document_review where id_doc_review ='" + iid_doc_review + "'";

                DataSet dsReviewList = objGlobaldata.Getdetails(sSqlstmt);
                DocumentReviewModels objType = new DocumentReviewModels();

                if (dsReviewList.Tables.Count > 0 && dsReviewList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsReviewList.Tables[0].Rows[0]["approvedby"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsReviewList.Tables[0].Rows[0]["approvedby"].ToString()) + ",";
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsReviewList.Tables[0].Rows[0]["loggedby"].ToString());
                  //  aAttachment = HttpContext.Current.Server.MapPath(dsReviewList.Tables[0].Rows[0]["nc_upload"].ToString());


                    sHeader = "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>"
                        + objGlobaldata.GetMultiCompanyBranchNameById(dsReviewList.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Defined On:<b></td> <td colspan=3>" + Convert.ToDateTime(dsReviewList.Tables[0].Rows[0]["review_date"].ToString()).ToString("dd/MM/yyyy")
                       + "</td></tr>"
                        + "<tr><td colspan=3><b>Document Level:<b></td> <td colspan=3>" + objGlobaldata.GetDocumentLevelbyId(dsReviewList.Tables[0].Rows[0]["doc_level"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Document Type:<b></td> <td colspan=3>" +objGlobaldata.GetDropdownitemById(dsReviewList.Tables[0].Rows[0]["doc_type"].ToString()) + "</td></tr>"                     
                    + "<tr><td colspan=3><b>Review Frequency:<b></td> <td colspan=3>" + objType.GetDocReviewFrequencyById(dsReviewList.Tables[0].Rows[0]["frequency"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Criteria to define the frequency:<b></td> <td colspan=3>" + objType.GetDocReviewCriteriaById(dsReviewList.Tables[0].Rows[0]["criteria"].ToString()) + "</td></tr>";
                    
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Document Review Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobaldata.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendReviewEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDocReview(DocumentReviewModels objModels)
        {
            try
            {

                string sSqlstmt = "update t_document_review set frequency='" + objModels.frequency
                     + "', criteria='" + objModels.criteria + "', approvedby='" + objModels.approvedby + "'";

                if (objModels.review_date != null && objModels.review_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", review_date ='" + objModels.review_date.ToString("yyyy/MM/dd") + "'";
                }
                              
                sSqlstmt = sSqlstmt + " where id_doc_review='" + objModels.id_doc_review + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDocReview: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDocReview(string sid_doc_review)
        {
            try
            {
                string sSqlstmt = "update t_document_review set active=0 where id_doc_review='" + sid_doc_review + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteDocReview: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDocReviewFreApproveOrReject(string sid_doc_review, int sStatus)
        {
            try
            {
                string sApproveDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;
                
                string Sql1 = "update t_document_review set approve_status='"+ sStatus + "', approve_date='" + sApproveDate + "' where id_doc_review='" + sid_doc_review + "'";
                objGlobaldata.ExecuteQuery(Sql1);

                //SendMgmtDocReviewApprovalmail(sid_doc_review);
                return true;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDocReviewFreApproveOrReject: " + ex.ToString());
            }
            return false;
        }


        //internal bool SendMgmtReviewApprovalmail(string sidMgmt, System.IO.FileStream fsSource, string filename, string sMessage = "")
        //{
        //    try
        //    {
        //        DataSet dsDocument = objGlobalData.Getdetails("select DocName, PreparedBy, ReviewedBy, ApprovedBy, UploadedBy from t_mgmt_documents where idMgmt='" + sidMgmt + "'");
        //        if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
        //        {
        //            string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString());
        //            sEmailid = Regex.Replace(sEmailid, ",+", ",");
        //            sEmailid = sEmailid.Trim();
        //            sEmailid = sEmailid.TrimEnd(',');
        //            sEmailid = sEmailid.TrimStart(',');
        //            string sExtraMsg = "Internal Document has been Reviewed, Document Name: " + dsDocument.Tables[0].Rows[0]["DocName"].ToString();

        //            string sEmailCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["ReviewedBy"].ToString()) + ","
        //                + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["UploadedBy"].ToString());
        //            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
        //            objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["PreparedBy"].ToString()), "mgmtdoc5", sExtraMsg);

        //            return objGlobalData.SendmailReview(sEmailid, fsSource, filename, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendDocChangeApprovalmail: " + ex.ToString());
        //    }
        //    return false;
        //}


    }

    public class DocumentReviewModelsList
        {
           public List<DocumentReviewModels> ReviewList { get; set; }
        }
}