using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Security.Principal;
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TrainingPlanController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public ActionResult AddTrainingPlan()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                objModel.branch = objGlobaldata.GetCurrentUserSession().division;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.branch);

                ViewBag.Topic = objGlobaldata.GetTrainingTopicList();

               
                ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.CheckList = objGlobaldata.GetChecklistTypeByChecklistRef(objModel.branch);
                objModel.PlannedBy = objGlobaldata.GetCurrentUserSession().empid;
                ViewBag.Process = objGlobaldata.GetAuidtCycleList();
                ViewBag.ApprovedBy = objGlobaldata.GetHrEmpListByDivision(objModel.branch);
                ViewBag.Audit = objGlobaldata.GetAuidtTypeList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTrainingPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }
    }
}