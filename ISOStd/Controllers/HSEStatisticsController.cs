using ISOStd.Filters;
using ISOStd.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class HSEStatisticsController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        [AllowAnonymous]
        public ActionResult HSEStatisticsReport()
        {
            try
            {
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                ViewBag.Month = objGlobaldata.GetConstantValue("Months"); ;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEStatisticsReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult HSEStatisticsReport(FormCollection form, string Vyear, string VMonth, string command)
        {
            //  WasteManagementModelsList objdisp = new WasteManagementModelsList();

            try
            {
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                ViewBag.Month = objGlobaldata.GetConstantValue("Months");
                if (Vyear != null && Vyear != "")
                {
                    ViewBag.Yearval = Vyear;
                }
                if (VMonth != null && VMonth != "")
                {
                    ViewBag.Monthval = VMonth;
                }
                string ys = objGlobaldata.GetDropdownitemById(Vyear);
                DataSet dsReport = objGlobaldata.GetHSEStatisticsReport(VMonth, ys);
                ViewBag.dsReport = dsReport;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in HSEStatisticsReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("HSEStatisticsReport");
            }

            if (command != null && !command.Equals("Generate"))
            {
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

                foreach (var key in Request.Cookies.AllKeys)
                {
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                return new ViewAsPdf("HSEStatisticsReportPrint", ViewBag.dsReport)
                {
                    //FileName = "HSEStatisticsReport.pdf",
                    Cookies = cookieCollection,
                    PageOrientation = Rotativa.Options.Orientation.Landscape
                };
            }
            return View();
        }
    }
}