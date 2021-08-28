using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class ReportPDFModels
    {
        public string id_complaint { get; set; }
        public string id_pfd_report { get; set; }
        public string id_nc { get; set; }
        public string report_name { get; set; }

        [Display(Name = "Immediate Action(Disposition)")]
        public string immediate_action { get; set; }

        [Display(Name = "Team")]
        public string team { get; set; }

        [Display(Name = "Corrective Action")]
        public string ca { get; set; }

        [Display(Name = "Root Cause Analysis")]
        public string rca { get; set; }

        [Display(Name = "Verification")]
        public string verification { get; set; }

        [Display(Name = "Review and Assign Customer Complaint")]
        public string review_assign { get; set; }

        [Display(Name = "Customer Complaint Response")]
        public string cust_response { get; set; }

        internal bool FunUpdatePDFReportFormat(ReportPDFModels objModels,string reportname)
        {
            clsGlobal objGlobal = new clsGlobal();
            try
            {      
                string sSqlstmt = "update t_pfd_report set immediate_action='" + objModels.immediate_action + "', team='" + objModels.team + "', ca='" + objModels.ca
                    + "', rca='" + objModels.rca + "', verification='" + objModels.verification + "', review_assign='" + objModels.review_assign + "', cust_response='" + objModels.cust_response + "', logged_by='" + objGlobal.GetCurrentUserSession().empid + "'"
                    + " where report_name='"+ reportname + "'";

                return objGlobal.ExecuteQuery(sSqlstmt);                
            }
            catch (Exception ex)
            {
                objGlobal.AddFunctionalLog("Exception in FunUpdatePDFReportFormat: " + ex.ToString());
            }
            return false;
        }

    }
}