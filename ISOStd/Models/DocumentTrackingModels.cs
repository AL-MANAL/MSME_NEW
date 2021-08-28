using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class DocumentTrackingModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_document_tracking { get; set; }

        [Display(Name = "Document Type")]
        public string doctype { get; set; }

        [Display(Name = "Document Name")]
        public string docname { get; set; }

        [Display(Name = "Issue Authority")]
        public string issue_autority { get; set; }

        [Display(Name = "Notification To")]
        public string NotificationPerson { get; set; }

        [Display(Name = "Documents")]
        public string upload { get; set; }

        [Display(Name = "Issue Date")]
        public DateTime issue_date { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime exp_date { get; set; }

        [Required]
        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Required]
        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Required]
        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Logged By")]
        public string Logged_by { get; set; }
        
        [Display(Name = "Id")]
        public int id_document_tracking_trans { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunAddDocTracking(DocumentTrackingModels objPlan, DocumentTrackingModelsList TrackList)
        {
            try
            {
                string sSqlstmt = "";
                //string sBranch = objGlobalData.GetCurrentUserSession().Work_Location;

                for (int i = 0; i < TrackList.DocList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_document_tracking (doctype,docname,issue_autority,NotificationPerson,upload,NotificationDays,"
                    + "NotificationPeriod,NotificationValue,Logged_by,branch,Department,Location";
                    string sFields = "", sFieldValue = "";

                    if (TrackList.DocList[i].issue_date != null && TrackList.DocList[i].issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", issue_date";
                        sFieldValue = sFieldValue + ", '" + TrackList.DocList[i].issue_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (TrackList.DocList[i].exp_date != null && TrackList.DocList[i].exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", exp_date";
                        sFieldValue = sFieldValue + ", '" + TrackList.DocList[i].exp_date.ToString("yyyy/MM/dd") + "'";
                    }
                    
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + TrackList.DocList[i].doctype + "','" + TrackList.DocList[i].docname + "',"
                   + "'" + TrackList.DocList[i].issue_autority + "','" + TrackList.DocList[i].NotificationPerson + "','" + TrackList.DocList[i].upload + "','" + TrackList.DocList[i].NotificationDays + "','" + TrackList.DocList[i].NotificationPeriod + "'"
                   + ",'" + TrackList.DocList[i].NotificationValue + "','" + objPlan.Logged_by + "','" + TrackList.DocList[i].branch + "','" + TrackList.DocList[i].Department + "','" + TrackList.DocList[i].Location + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";

                }

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    //for (int i = 0; i < TrackList.DocList.Count; i++)
                    //{
                    //    SendDocEmail(TrackList, i);
                    //}
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddDocTracking: " + ex.ToString());

            }
            return false;
        }

        //internal bool SendDocEmail(DocumentTrackingModelsList DocTrackList, int i)
        //{

        //    try
        //    {

        //        string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

        //        // string sInformation = "", sHeader = "";
        //        sCCList = (DocTrackList.DocList[i].issue_autority);
        //        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(DocTrackList.DocList[i].NotificationPerson);

        //        sInformation =
        //        "Document Type:'" + DocTrackList.DocList[i].doctype + "'"
        //        + " <br />"
        //        + "Document Name:'" + DocTrackList.DocList[i].docname + "'"
        //        + " <br />"
        //        + "Issue Date:'" + DocTrackList.DocList[i].issue_date.ToString("dd/MM/yyyy") + "'"
        //        + " <br />"
        //        + "Expiry Date:'" + DocTrackList.DocList[i].exp_date.ToString("dd/MM/yyyy") + "'";
                
        //        sHeader = sHeader + sInformation;

        //        sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
        //        sToEmailId = sToEmailId.Trim();
        //        sToEmailId = sToEmailId.TrimEnd(',');
        //        sToEmailId = sToEmailId.TrimStart(',');

        //        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "DocTracking", sHeader, "", "Document Tracking");
        //        objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendIncidentEmail: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunUpdateDocTracking(DocumentTrackingModels objDoc)
        {
            try
            {
                string sSqlstmt = "update t_document_tracking set doctype ='" + objDoc.doctype + "', docname='" + objDoc.docname + "', "
                    + "issue_autority='" + objDoc.issue_autority + "',NotificationPerson='" + objDoc.NotificationPerson + "',upload='" + objDoc.upload + "'"
                    + ",NotificationDays='" + objDoc.NotificationDays + "',NotificationPeriod='" + objDoc.NotificationPeriod
                    + "',NotificationValue='" + objDoc.NotificationValue + "',branch='" + objDoc.branch + "',Department='" + objDoc.Department + "',Location='" + objDoc.Location + "'";

                if (objDoc.issue_date != null && objDoc.issue_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", issue_date='" + objDoc.issue_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objDoc.exp_date != null && objDoc.exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", exp_date='" + objDoc.exp_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_document_tracking='" + objDoc.id_document_tracking + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateDocTracking: " + ex.ToString());

            }
            return false;
        }

        internal bool FunDeleteDocTracking(string sid_document_tracking)
        {
            try
            {
                string sSqlstmt = "update t_document_tracking set Active=0 where id_document_tracking='" + sid_document_tracking + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDocTracking: " + ex.ToString());
            }
            return false;
        }
        
    }

    public class DocumentTrackingModelsList
    {
        public List<DocumentTrackingModels> DocList { get; set; }
    }
}