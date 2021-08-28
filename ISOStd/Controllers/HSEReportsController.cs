using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.IO;
using System.Data;
using ISOStd.Models;
using Microsoft.Reporting.WebForms;
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class HSEReportsController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        
        [AllowAnonymous]
        public ActionResult HSEReports(string ReportType,string month,string year)
        {
            try {
 
            ViewBag.Years = objGlobaldata.GetDropdownList("Years");
            ViewBag.Months = objGlobaldata.GetConstantValue("Months");
            ViewBag.ReportType = objGlobaldata.GetDropdownList("HSE Report Type");
            if (ReportType != null && ReportType != "select")
            {
              
                ViewBag.Year_type = objGlobaldata.GetDropdownitemById(year);
                ViewBag.Months_type = month;
                ViewBag.Report_Type = objGlobaldata.GetDropdownitemById(ReportType);
                string type = objGlobaldata.GetDropdownitemById(ReportType);
                if(type == "HSE Performance")//HSE Perf Dashboard
                {
                    ViewBag.IframeHSEPerfDisplay = true;
                }

            }
            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEReports: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            } 
            return View();
        }
    }
}