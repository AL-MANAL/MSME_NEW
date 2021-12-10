using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class CertificatesModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string certId { get; set; }

        [Display(Name = "Certificate Name")]
        public string certName { get; set; }

        // [Required]
        [Display(Name = "Document Upload")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime expiry_date { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Branch")]
        public string branch { get; set; }

        internal bool FunAddCertificate(CertificatesModels objCertModel)
        {
            try
            {
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_certificates (certName,DocUploadPath,branch,Department,Location";
                string sFields = "", sFieldValue = "";
                if (objCertModel.expiry_date != null && objCertModel.expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", expiry_date";
                    sFieldValue = sFieldValue + ", '" + objCertModel.expiry_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")values('" + objCertModel.certName + "','" + objCertModel.DocUploadPath
                    + "','" + objCertModel.branch + "','" + objCertModel.Department + "','" + objCertModel.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCertificate: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCertificate(CertificatesModels objCertModel)
        {
            try
            {
                string sSqlstmt = "update t_certificates set certName='" + objCertModel.certName + "', branch='" + objCertModel.branch
                    + "', Department='" + objCertModel.Department + "', Location='" + objCertModel.Location + "'";
                if (objCertModel.DocUploadPath != null && objCertModel.DocUploadPath != "")
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objCertModel.DocUploadPath + "'";
                }
                string sFields = "";
                if (objCertModel.expiry_date != null && objCertModel.expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", expiry_date='";
                    sFields = sFields + objCertModel.expiry_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields + " where certId='" + objCertModel.certId + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditCertificate: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCertificate(string scertId)
        {
            try
            {
                string sSqlstmt = "update t_certificates set Status=0 where certId='" + scertId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCertificate: " + ex.ToString());
            }
            return false;
        }
    }

    public class CertificatesModelsList
    {
        public List<CertificatesModels> CertificateList { get; set; }
    }
}