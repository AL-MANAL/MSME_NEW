using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class MgmtExternalDocumentModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int Mgmt_doc_External_Id { get; set; }
  
        [Required]
        [Display(Name = "Document Reference")]
        [System.Web.Mvc.Remote("doesDocRefExist", "MgmtExternalDocument", HttpMethod = "POST", ErrorMessage = "Document Reference ID already exists. Please enter a different Reference ID.")]
        public string DocRef { get; set; }

        [Required]
        [Display(Name = "Document Name")]
        [System.Web.Mvc.Remote("doesDocNameExist", "MgmtExternalDocument", HttpMethod = "POST", ErrorMessage = "Document name already exists. Please enter a different name.")]
        public string DocName { get; set; }

        [Display(Name = "Document Origin")]
        public string Doc_Origin { get; set; }

        [Display(Name = "Issue/Revision No.")]
        public string IssueNo { get; set; }

        [Display(Name = "Revision No.")]
        public string RevNo { get; set; }

        [Display(Name = "Date of Issue")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Department")]
        public string DeptId { get; set; }

        [Display(Name = "Under the Custody Of")]
        public string CustodyOf { get; set; }
            
        [Display(Name = "Document Soft Copy")]
        public string SoftCopy_Path { get; set; }
          
        [Display(Name = "Method Of Updating")]
        public string MethodOf_Updating { get; set; }
         
        [Display(Name = "Updated Through")]
        public string Updated_Thru { get; set; }
             
        [Display(Name = "Personnel Responsible to maintain the Document")]
        public string Person_Responsible { get; set; }

        [Display(Name = "Remarks")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }
         
        [Display(Name = "Uploaded On")]
        public DateTime UploadedDate { get; set; }

        [Display(Name = "Uploaded by")]
        public string UploadedBy { get; set; }

        //[Display(Name = "Review Date")]
        [Display(Name = "Expiry Date")]
        public DateTime Eve_Date { get; set; }

        //[Required]
        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

       //[Required]
        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        //[Required]
        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name ="Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunDeleteExternalMgmtDoc(string sMgmt_doc_External_Id)
        {
            try
            {
                string sSqlstmt = "update t_mgmt_doc_external set Active=0 where Mgmt_doc_External_Id='" + sMgmt_doc_External_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteExternalMgmtDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddMgmtExternalDocument(MgmtExternalDocumentModels objMgmtExternalDocumentModels)
        {
            try
            {
                string sDocDate = objMgmtExternalDocumentModels.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sUploadedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sEve_Date= objMgmtExternalDocumentModels.Eve_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
               // string sBranch = objGlobalData.GetCurrentUserSession().division;

                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "insert into t_mgmt_doc_external (DocRef, DocName, Doc_Origin, IssueNo, DocDate, CustodyOf, SoftCopy_Path,"
                    + " MethodOf_Updating, Updated_Thru, Person_Responsible, Remarks, UploadedBy, UploadedDate,NotificationDays,NotificationPeriod,NotificationValue,branch,DeptId,Location";


                string sFields = "", sFieldValue = "";
                if (objMgmtExternalDocumentModels.Eve_Date != null && objMgmtExternalDocumentModels.Eve_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Eve_Date";
                    sFieldValue = sFieldValue + ", '" + sEve_Date + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objMgmtExternalDocumentModels.DocRef + "','" + objMgmtExternalDocumentModels.DocName
                + "','" + objMgmtExternalDocumentModels.Doc_Origin + "','" + objMgmtExternalDocumentModels.IssueNo + "','" + sDocDate + "','" + objMgmtExternalDocumentModels.CustodyOf
                + "','" + objMgmtExternalDocumentModels.SoftCopy_Path + "','" + objMgmtExternalDocumentModels.MethodOf_Updating + "','" + objMgmtExternalDocumentModels.Updated_Thru
                + "','" + objMgmtExternalDocumentModels.Person_Responsible + "','" + objMgmtExternalDocumentModels.Remarks + "','" + user
                + "','" + sUploadedDate + "','" + objMgmtExternalDocumentModels.NotificationDays + "','" + objMgmtExternalDocumentModels.NotificationPeriod + "','" + objMgmtExternalDocumentModels.NotificationValue + "','" + objMgmtExternalDocumentModels.branch
                + "','" + objMgmtExternalDocumentModels.DeptId + "','" + objMgmtExternalDocumentModels.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMgmtExternalDocument: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateMgmtExternalDocument(MgmtExternalDocumentModels objMgmtExternalDocumentModels)
        {
            try
            {
                string sDocDate = objMgmtExternalDocumentModels.DocDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sEve_Date = objMgmtExternalDocumentModels.Eve_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_mgmt_doc_external set  Doc_Origin ='"
                    + objMgmtExternalDocumentModels.Doc_Origin + "', IssueNo='" + objMgmtExternalDocumentModels.IssueNo
                    + "', RevNo='" + objMgmtExternalDocumentModels.RevNo + "', DocDate='" + sDocDate + "', DeptId='" + objMgmtExternalDocumentModels.DeptId
                    + "', CustodyOf='" + objMgmtExternalDocumentModels.CustodyOf + "', MethodOf_Updating='" + objMgmtExternalDocumentModels.MethodOf_Updating +
                    "', Updated_Thru='" + objMgmtExternalDocumentModels.Updated_Thru + "', Person_Responsible='" + objMgmtExternalDocumentModels.Person_Responsible
                    + "', Remarks='" + objMgmtExternalDocumentModels.Remarks + "', NotificationDays='" + objMgmtExternalDocumentModels.NotificationDays +
                    "', NotificationPeriod='" + objMgmtExternalDocumentModels.NotificationPeriod + "', NotificationValue='" + objMgmtExternalDocumentModels.NotificationValue 
                    + "', branch='" + objMgmtExternalDocumentModels.branch + "', Location='" + objMgmtExternalDocumentModels.Location + "'";

                if (objMgmtExternalDocumentModels.Eve_Date != null && objMgmtExternalDocumentModels.Eve_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Eve_Date='" + sEve_Date + "'";
                }

                if (objMgmtExternalDocumentModels.SoftCopy_Path != null)
                {
                    sSqlstmt = sSqlstmt + ", SoftCopy_Path='" + objMgmtExternalDocumentModels.SoftCopy_Path + "'";
                }
                sSqlstmt = sSqlstmt + " where Mgmt_doc_External_Id='" + objMgmtExternalDocumentModels.Mgmt_doc_External_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMgmtExternalDocument: " + ex.ToString());
            }
            return false;
        }

        internal bool CheckForDocNameExists(string sDocName)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select DocName from t_mgmt_doc_external where DocName='" + sDocName + "'");

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
                DataSet dsDoc = objGlobalData.Getdetails("select DocRef from t_mgmt_doc_external where DocRef='" + sDocRef + "'");

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
    }

    public class MgmtExternalDocumentModelsList
    {
        public List<MgmtExternalDocumentModels> lstMgmtExternalDocument { get; set; }
    }
}