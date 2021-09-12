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
    public class DocumentCreateRequestModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_doc_request { get; set; }

        [Display(Name = "DCR No.")]
        public string dcr_no { get; set; }

        [Display(Name = "Date of Request")]
        public DateTime date_request { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }
        public string divisionId { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }
        public string departmentId { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }
        //public string teamId { get; set; }

        [Display(Name = "Reason")]
        public string reason { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "To be checked by (Department Head.)")]
        public string checkedby { get; set; }      

        [Display(Name = "Agreed")]
        public string agreed { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Assistant Manager QHSE")]
        public string doc_control { get; set; }
        public string doc_controlName { get; set;}

        [Display(Name = "Document Level")]
        public string doc_level { get; set; }

        [Display(Name = "Document Title")]
        public string doc_title { get; set; }

        [Display(Name = "Serial No")]
        public string serial_no { get; set; }

        [Display(Name = "New Document Reference")]
        public string new_doc_ref { get; set; }

        [Display(Name = "Document Status")]
        public string doc_status { get; set; }
        public string doc_statusId { get; set; }

        [Display(Name = "Department Head Approve date")]
        public DateTime checkedby_approve_date { get; set; }

        [Display(Name = "Assistant manager QHSE Approve Date")]
        public DateTime controller_approve_date { get; set; }

        [Display (Name = "Check List" )]
        public string checklist_id { get; set; }

        internal bool FunAddDocCreateRequest(DocumentCreateRequestModels objPortal)
        {
            try
            {
                // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_document_create_request (division,`department`,location, reason,upload,checkedby,logged_by";

                string sFields = "", sFieldValue = "";

                if (objPortal.date_request != null && objPortal.date_request > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", date_request";
                    sFieldValue = sFieldValue + ", '" + objPortal.date_request.ToString("yyyy/MM/dd") + "'";
                }
               
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPortal.division + "','" + objPortal.department + "','" + objPortal.location + "','" + objPortal.reason
                    + "','" + objPortal.upload + "','" + objPortal.checkedby + "','" + objGlobalData.GetCurrentUserSession().empid + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int iid_doc_request = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(),out iid_doc_request))
                {
                    if (iid_doc_request > 0)
                    {
                        // string sName = objGlobalData.GetBranchShortNameByID(division)+"-"+ objGlobalData.GetDeptNameById(department);
                        string sName = objGlobalData.GetBranchShortNameByID(division);
                        DataSet dsData = objGlobalData.GetReportNo("DocCreateRequest", "", sName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            dcr_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        objGlobalData.ExecuteQuery("Update t_document_create_request set dcr_no = '" + dcr_no + "' where id_doc_request= '"+ iid_doc_request + "'");
                        SendDocCreateRequestEmail();
                        return true;
                    }                  
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocCreateRequest: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDocCreateRequest(DocumentCreateRequestModels objPortal)
        {
            try
            {
                string sSqlstmt = "update t_document_create_request set division ='" + objPortal.division + "', `department`='" + objPortal.department + "', `location`='" + objPortal.location
                    + "', reason='" + objPortal.reason + "',upload='" + objPortal.upload + "',checkedby='" + objPortal.checkedby  + "'";

                if (objPortal.date_request != null && objPortal.date_request > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_request='" + objPortal.date_request.ToString("yyyy/MM/dd") + "'";
                }               

                sSqlstmt = sSqlstmt + " where id_doc_request='" + objPortal.id_doc_request + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDocCreateRequest: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddDCRCheckedbyApprove(DocumentCreateRequestModels objPortal)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                string sSqlstmt = "update t_document_create_request set checklist_id='" + objPortal.checklist_id 
                    + "',agreed='" + objPortal.agreed + "',comments='" + objPortal.comments 
                    + "',doc_control='" + objPortal.doc_control + "',checkedby_approve_date='" + sApprovedDate + "',doc_status='" + objPortal.doc_status + "'";

                sSqlstmt = sSqlstmt + " where id_doc_request='" + objPortal.id_doc_request + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sToEmailIds = "", sCCList = "", sExtraMsg = "", aAttachment = "", sHeader = "";

                    DataSet dsDocument = objGlobalData.Getdetails("select id_doc_request,dcr_no, date_request, division," +
                        "`department`,location,reason,upload,checkedby,logged_by,doc_control" +
                         " from t_document_create_request where id_doc_request='" + id_doc_request + "'");

                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        aAttachment = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[0]["upload"].ToString());

                        if (objPortal.doc_status == "3")
                        {
                            sHeader = "<br/><tr><td colspan=3><b>DCR No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["dcr_no"].ToString() + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Date of Request:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["date_request"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsDocument.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsDocument.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsDocument.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>";


                            if (File.Exists(aAttachment))
                            {
                                sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                            }

                            if (dsDocument.Tables[0].Rows[0]["logged_by"].ToString() != "")
                            {
                                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                            }

                           // sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                            sExtraMsg = "Document Create Request Rejected with following details";

                            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                            sToEmailIds = sToEmailIds.Trim();
                            sToEmailIds = sToEmailIds.TrimEnd(',');
                            sToEmailIds = sToEmailIds.TrimStart(',');

                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString()), "DocCreateRequest", sExtraMsg + sHeader);
                            objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                            return true;
                        }
                        else
                        {
                            sHeader = "<br/><tr><td colspan=3><b>DCR No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["dcr_no"].ToString() + "</td></tr>"
                           + "<tr><td colspan=3><b>Date of Request:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["date_request"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                           + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsDocument.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                           + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsDocument.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsDocument.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + objPortal.comments + "</td></tr>";

                            if (File.Exists(aAttachment))
                            {
                                sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                            }

                            if (dsDocument.Tables[0].Rows[0]["doc_control"].ToString() != "")
                            {
                                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["doc_control"].ToString());
                            }

                            sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                            sExtraMsg = "Document Create Request Approved with following details";

                            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                            sToEmailIds = sToEmailIds.Trim();
                            sToEmailIds = sToEmailIds.TrimEnd(',');
                            sToEmailIds = sToEmailIds.TrimStart(',');

                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                  objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["doc_control"].ToString()), "DocCreateRequest", sExtraMsg + sHeader);
                            objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                            return true;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDCRCheckedbyApprove: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddDCRControllerApprove(DocumentCreateRequestModels objPortal)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                string sSqlstmt = "update t_document_create_request set division ='" + objPortal.division + "', `department`='" + objPortal.department
                   /* + "', `location`='" + objPortal.location+ "', doc_level='" + objPortal.doc_level + "',doc_title='" + objPortal.doc_title*/
                   + "',serial_no='" + objPortal.serial_no + "',new_doc_ref='" + objPortal.new_doc_ref + "',controller_approve_date='" + sApprovedDate + "',doc_status='"+ objPortal.doc_status + "'";

                sSqlstmt = sSqlstmt + " where id_doc_request='" + objPortal.id_doc_request + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sToEmailIds = "", sCCList = "", sExtraMsg = "", aAttachment = "", sHeader = "";

                    DataSet dsDocument = objGlobalData.Getdetails("select id_doc_request,dcr_no, date_request, division," +
                        "`department`,location,reason,upload,checkedby,logged_by" +
                         " from t_document_create_request where id_doc_request='" + id_doc_request + "'");

                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        aAttachment = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[0]["upload"].ToString());

                        if (objPortal.doc_status == "3")
                        {
                            sHeader = "<br/><tr><td colspan=3><b>DCR No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["dcr_no"].ToString() + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Date of Request:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["date_request"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsDocument.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsDocument.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                                                       + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsDocument.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>";


                            if (File.Exists(aAttachment))
                            {
                                sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                            }

                            if (dsDocument.Tables[0].Rows[0]["checkedby"].ToString() != "")
                            {
                                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["checkedby"].ToString());
                            }

                            sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                            sExtraMsg = "Document Create Request Rejected with following details";

                            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                            sToEmailIds = sToEmailIds.Trim();
                            sToEmailIds = sToEmailIds.TrimEnd(',');
                            sToEmailIds = sToEmailIds.TrimStart(',');

                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["checkedby"].ToString()), "DocCreateRequest", sExtraMsg + sHeader);
                            objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                            return true;
                        }
                        else
                        {
                            sHeader = "<br/><tr><td colspan=3><b>DCR No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["dcr_no"].ToString() + "</td></tr>"
                           + "<tr><td colspan=3><b>Date of Request:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["date_request"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                           + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsDocument.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                           + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsDocument.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                           + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsDocument.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                           + "<tr><td colspan=3><b>Serial Number:<b></td> <td colspan=3>" + objPortal.serial_no + "</td></tr>"
                           + "<tr><td colspan=3><b>New Document Reference:<b></td> <td colspan=3>" + objPortal.new_doc_ref + "</td></tr>";

                            if (File.Exists(aAttachment))
                            {
                                sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                            }

                            if (dsDocument.Tables[0].Rows[0]["checkedby"].ToString() != "")
                            {
                                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["checkedby"].ToString());
                            }

                            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                            sExtraMsg = "Document Create Request Approved with following details";

                            sCCList = Regex.Replace(sCCList, ",+", ",");
                            sCCList = sCCList.Trim();
                            sCCList = sCCList.TrimEnd(',');
                            sCCList = sCCList.TrimStart(',');

                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                  objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString()), "DocCreateRequest", sExtraMsg + sHeader);
                            objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                            return true;
                        } 
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDCRControllerApprove: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDelDocCreateRequest(string sid_doc_request)
        {
            try
            {
                string sSqlstmt = "update t_document_create_request set Active=0 where id_doc_request='" + sid_doc_request + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDelDocCreateRequest: " + ex.ToString());
            }
            return false;
        }

        internal bool SendDocCreateRequestEmail()
        {
            try
            {
                DocumentCreateRequestModels objParties = new DocumentCreateRequestModels();
                string sToEmailIds = "", sCCList = "", sExtraMsg = "", aAttachment = "", sHeader = "";

                string id_doc_request = "";

                DataSet AuthPortal = objGlobalData.Getdetails("Select max(id_doc_request) as id_doc_request from t_document_create_request where active = 1");
                if (AuthPortal.Tables.Count > 0 && AuthPortal.Tables[0].Rows.Count > 0)
                {
                    id_doc_request = AuthPortal.Tables[0].Rows[0]["id_doc_request"].ToString();
                }
                if (id_doc_request != "")
                {
                    DataSet dsDocument = objGlobalData.Getdetails("select id_doc_request,dcr_no, date_request, division," +
                        "`department`,location,reason,upload,checkedby,logged_by" +
                         " from t_document_create_request where id_doc_request='" + id_doc_request + "'");

                    if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {
                        aAttachment = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[0]["upload"].ToString());


                        sHeader = "<br/><tr><td colspan=3><b>DCR No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["dcr_no"].ToString() + "</td></tr>"
                            + "<tr><td colspan=3><b>Date of Request:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["date_request"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                            + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsDocument.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsDocument.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsDocument.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>";


                        if (File.Exists(aAttachment))
                        {
                            sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                        }

                        if (dsDocument.Tables[0].Rows[0]["checkedby"].ToString() != "")
                        {
                            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["checkedby"].ToString());
                        }

                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                        sExtraMsg = "Document Create Request for following details";

                        sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sToEmailIds = sToEmailIds.Trim();
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        sToEmailIds = sToEmailIds.TrimStart(',');

                        //sCCList = Regex.Replace(sCCList, ",+", ",");
                        //sCCList = sCCList.Trim();
                        //sCCList = sCCList.TrimEnd(',');
                        //sCCList = sCCList.TrimStart(',');

                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                            objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["checkedby"].ToString()), "DocCreateRequest", sExtraMsg + sHeader);
                        //objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                        objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendDocCreateRequestEmail: " + ex.ToString());
            }
            return false;
        }
       
    }

    public class DocCreateRequestModelsList
    {
        public List<DocumentCreateRequestModels> RequestList { get; set; }
    }

    public class DCRChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //DCR CheckList

        [Display(Name = "Id")]
        public string id_chklist { get; set; }

        [Display(Name = "Document Number")]
        public string checklistRef { get; set; }

        [Display(Name = "Revision")]
        public string revision { get; set; }

        [Display(Name = "Document Title")]
        public string doc_title { get; set; }

        [Display(Name = "DCRF/DRRF No")]
        public string dcrf_no { get; set; }

        [Display(Name = "Incoming Memo Ref")]
        public string memo_ref { get; set; }

        [Display(Name = "Created By")]
        public string logged_by { get; set; }

        //CheckList Trans
        [Display(Name = "ID")]
        public string id_chklist_trans { get; set; }

        [Display(Name = "Ratings")]
        public string ratings { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        //Questions

        [Display(Name = "Id")]
        public string id_questions { get; set; }

        [Display(Name = "Questions")]
        public string questions { get; set; }

        [Display(Name = "Section")]
        public string section_id { get; set; }

        public MultiSelectList GetDCRQuestionList()
        {
            DCRChecklistModelsList Quest = new DCRChecklistModelsList();
            Quest.DCRChkList = new List<DCRChecklistModels>();

            try
            {              
                    DataSet dsQuest1 = objGlobalData.Getdetails("select id_questions,questions from t_document_create_request_questions where active=1");
                    if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                        {
                           DCRChecklistModels Qst = new DCRChecklistModels()
                            {
                                id_questions = dsQuest1.Tables[0].Rows[i]["id_questions"].ToString(),
                                questions = dsQuest1.Tables[0].Rows[i]["questions"].ToString()
                            };
                           Quest.DCRChkList.Add(Qst);
                        }
                    }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetInspectionQuestions" + ex.ToString());
            }
            return new MultiSelectList(Quest.DCRChkList, "id_questions", "questions");
        }

        public string GetDCRQuestionNameById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select questions as Name from t_document_create_request_questions where id_questions='" + item_id + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDCRQuestionNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetDCRSectionList()
        {
            DCRChecklistModelsList Quest = new DCRChecklistModelsList();
            Quest.DCRChkList = new List<DCRChecklistModels>();

            try
            {
                DataSet dsQuest1 = objGlobalData.Getdetails("select id_questions,section_id from t_document_create_request_questions where active=1");
                if (dsQuest1.Tables.Count > 0 && dsQuest1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuest1.Tables[0].Rows.Count; i++)
                    {
                        DCRChecklistModels Qst = new DCRChecklistModels()
                        {
                            id_questions = dsQuest1.Tables[0].Rows[i]["id_questions"].ToString(),
                            section_id = dsQuest1.Tables[0].Rows[i]["section_id"].ToString()
                        };
                        Quest.DCRChkList.Add(Qst);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDCRSectionList" + ex.ToString());
            }
            return new MultiSelectList(Quest.DCRChkList, "id_questions", "section_id");
        }

        internal bool FunAddDCRChecklist(DCRChecklistModels objModel, DCRChecklistModelsList objModelList)
        {
            try
            {                
                string sSqlstmt = "insert into t_document_create_request_chklist (checklistRef,revision,doc_title,dcrf_no,memo_ref,logged_by) " +
                    "values('" + objModel.checklistRef + "','" + objModel.revision + "','" + objModel.doc_title + "','" + objModel.dcrf_no + "','" +  objModel.memo_ref + "','" + objGlobalData.GetCurrentUserSession().empid + "')";

                int iid_chklist = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_chklist))
                {
                    if(iid_chklist > 0)
                    { 
                        FunAddDCRChecklistTrans(objModelList, iid_chklist);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDCRChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddDCRChecklistTrans(DCRChecklistModelsList objModelList, int id_chklist)
        {
            try
            {
                if (objModelList.DCRChkList.Count > 0)
                {
                    string sSqlstmt = "delete from t_document_create_request_chklist_trans where id_chklist='" + id_chklist + "'; ";
                    for (int i = 0; i < objModelList.DCRChkList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_document_create_request_chklist_trans ( id_chklist, id_questions,ratings,comments"
                        + ") values('" + id_chklist + "','" + objModelList.DCRChkList[i].id_questions + "','" + objModelList.DCRChkList[i].ratings + "','" + objModelList.DCRChkList[i].comments + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDCRChecklistTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDCRChecklist(DCRChecklistModels objModel, DCRChecklistModelsList objModelList)
        {
            try
            {     
                string sSqlstmt = "update t_document_create_request_chklist set "+/*checklistRef='" + objModel.checklistRef + "', */"revision='" + objModel.revision
                   + "', doc_title='" + objModel.doc_title /*+ "', dcrf_no='" + objModel.dcrf_no */+ "', memo_ref='" + objModel.memo_ref + "'";

                //if (objInspChecklist.Inspection_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", Inspection_date='" + Inspection_date + "' ";
                //}
                  sSqlstmt = sSqlstmt + " where id_chklist='" + objModel.id_chklist + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                  //  objModelList.DCRChkList[0].id_chklist = objModel.id_chklist;
                    FunUpdateDCRChecklistTrans(objModelList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDCRChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDCRChecklistTrans(DCRChecklistModelsList objModelList)
        {
            try
            {
                if (objModelList.DCRChkList.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objModelList.DCRChkList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_document_create_request_chklist_trans set id_questions='" + objModelList.DCRChkList[i].id_questions + "',"
                            + "ratings='" + objModelList.DCRChkList[i].ratings + "',comments='" + objModelList.DCRChkList[i].comments + "' where id_chklist_trans='" + objModelList.DCRChkList[i].id_chklist_trans + "';";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDCRChecklistTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDCRChecklist(string sid_chklist)
        {
            try
            {
                string sSqlstmt = "update t_document_create_request_chklist set active=0 where id_chklist='" + sid_chklist + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDCRChecklist: " + ex.ToString());
            }
            return false;
        }
    }

    public class DCRChecklistModelsList
    {
        public List<DCRChecklistModels> DCRChkList { get; set; }
    }
}