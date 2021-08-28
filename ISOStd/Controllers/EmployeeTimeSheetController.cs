using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;

namespace ISOStd.Controllers
{
    public class EmployeeTimeSheetController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //
        // GET: /EmployeeTimeSheet/

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /EmployeeTimeSheet/AddTimeSheet
        [AllowAnonymous]
        public ActionResult AddTimeSheet()
        {
            return initilizeAddTimeSheet();
        }

        // GET: /EmployeeTimeSheet/AddTimeSheet
        [AllowAnonymous]
        protected ActionResult initilizeAddTimeSheet()
        {
            ViewBag.CustList = objGlobaldata.GetCustomerListbox();
            ViewBag.EmpLists = objGlobaldata.GetEmployeeListbox(' ');
            ViewBag.TimeInHour = objGlobaldata.GetAuditTimeInHour(true);
            ViewBag.TimeInMin = objGlobaldata.GetAuditTimeInMin();

            return View();
        }

        //
        // POST: /EmployeeTimeSheet/AddTimeSheet
        [HttpPost]
        [ValidateAntiForgeryToken]
         public ActionResult AddTimeSheet(EmpTimeSheetModels objEmpTimeSheet, FormCollection form)
        {           
            try
            {            
                //objInterAudit.AuditType = "12";
                DateTime dateValue;

                if (DateTime.TryParse(form["MonthOf"], out dateValue) == true)
                {
                    objEmpTimeSheet.MonthOf = dateValue;
                }
                objEmpTimeSheet.CustSuppId = form["CustSuppId"];

                EmpTimeSheetModelslsList objInternalAuditModelsList = new EmpTimeSheetModelslsList();
                objInternalAuditModelsList.EmpTimeSheetMList = new List<EmpTimeSheetModels>();

                for (int i = 0; i < Convert.ToInt16(form["ItemCount"]); i++)
                {
                    EmpTimeSheetModels objInternalAuditModels = new EmpTimeSheetModels();

                    //objInterAudit.CompanyId = form["CompanyId"];//objUserInfo.CompanyId;
                    objInternalAuditModels.EmpId = form["EmpId"+i];
                    objInternalAuditModels.FirstHalf_InTime = form["fhInTime" + i];
                    objInternalAuditModels.FirstHalf_OutTime = form["fhOutTime" + i];
                    objInternalAuditModels.SecondHalf_InTime = form["shInTime" + i];
                    objInternalAuditModels.SecondHalf_OutTime = form["shOutTime" + i];
                    objInternalAuditModels.OverTime = form["OverTime" + i];
                    objInternalAuditModels.Remarks = form["Remarks" + i];
                  
                    objInternalAuditModelsList.EmpTimeSheetMList.Add(objInternalAuditModels);
                }
                objEmpTimeSheet.FunAddTimeSheet(objEmpTimeSheet, objInternalAuditModelsList);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return initilizeAddTimeSheet();

            //return View(objInterAudit);
        }        
      
    }
}
