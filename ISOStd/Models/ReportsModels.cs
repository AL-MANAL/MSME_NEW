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
    public class ReportsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_reports { get; set; }

        [Display(Name = "Category")]
        public string category { get; set; }

        [Display(Name = "Report")]
        public string report_name { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Controller")]
        public string controller { get; set; }

        [Display(Name = "Action Name")]
        public string action_name { get; set; }

        internal bool FunDeleteReportDoc(string id_reports)
        {
            try
            {
                string sSqlstmt = "update t_reports set Active=0 where id_reports='" + id_reports + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteReportDoc: " + ex.ToString());
            }
            return false;
        }
    }
    public class ReportsModelsList
    {
        public List<ReportsModels> RptList { get; set; }
    }
}