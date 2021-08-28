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

namespace ISOStd.Controllers
{
    public class AccessPrivilegesController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

      
        public AccessPrivilegesController()
        {
            ViewBag.Menutype = "Access Privileges";
            ViewBag.SubMenutype = "Access Privileges";
        }

        
        [AllowAnonymous]
        public JsonResult UpdateAccessPrivileges(string selected,int status)
        {
            AccessPrivilegesModels objAccessList = new AccessPrivilegesModels();
            bool IssueNo=false;
            if (selected != null)
            {
                IssueNo = objAccessList.FunUpdateAccessPrivileges(selected, status);
            }

            return Json(IssueNo);
        }

       
        public ActionResult AccessPrivilegesList(int? page)
        {
            AccessPrivilegesModelsList objAccessList = new AccessPrivilegesModelsList();
            objAccessList.AccessPrivilegesList = new List<AccessPrivilegesModels>();
            try {

                 string sSqlstmt = "Select * from t_accessprivileges";
                 DataSet dsPrivileges = objGlobaldata.Getdetails(sSqlstmt);

                 for (int i = 0; dsPrivileges.Tables.Count > 0 && i < dsPrivileges.Tables[0].Rows.Count; i++)
                 {
                     try
                     {
                         AccessPrivilegesModels objAccess = new AccessPrivilegesModels
                         {
                             id_AccessPrivileges = dsPrivileges.Tables[0].Rows[i]["id_AccessPrivileges"].ToString(),
                             EmpId = objGlobaldata.GetEmpIDByEmpNo(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                             Emp_Name = objGlobaldata.GetEmpHrNameById(objGlobaldata.GetEmployeeCompEmpIdByEmpId(dsPrivileges.Tables[0].Rows[i]["EmpId"].ToString())),
                             DocMgmt = dsPrivileges.Tables[0].Rows[i]["DocMgmt"].ToString(),
                             ObjKPI = dsPrivileges.Tables[0].Rows[i]["ObjKPI"].ToString(),
                             Risk = dsPrivileges.Tables[0].Rows[i]["Risk"].ToString(),
                             HR = dsPrivileges.Tables[0].Rows[i]["HR"].ToString(),
                             Meeting = dsPrivileges.Tables[0].Rows[i]["Meeting"].ToString(),
                             Supplier = dsPrivileges.Tables[0].Rows[i]["Supplier"].ToString(),
                             Audit = dsPrivileges.Tables[0].Rows[i]["Audit"].ToString(),
                             CustomerMgmt = dsPrivileges.Tables[0].Rows[i]["CustomerMgmt"].ToString(),
                             Equipment = dsPrivileges.Tables[0].Rows[i]["Equipment"].ToString(),
                             Certificates = dsPrivileges.Tables[0].Rows[i]["Certificates"].ToString(),
                             HSE = dsPrivileges.Tables[0].Rows[i]["HSE"].ToString(),
                             MgmtReports = dsPrivileges.Tables[0].Rows[i]["MgmtReports"].ToString(),
                             LegalRegister = dsPrivileges.Tables[0].Rows[i]["LegalRegister"].ToString(),
                             LeaveMgmt = dsPrivileges.Tables[0].Rows[i]["LeaveMgmt"].ToString(),
                             TrainingOrientation = dsPrivileges.Tables[0].Rows[i]["TrainingOrientation"].ToString(),
                             KnowledgBase = dsPrivileges.Tables[0].Rows[i]["KnowledgBase"].ToString(),
                             LeaveApply = dsPrivileges.Tables[0].Rows[i]["LeaveApply"].ToString(),

                         };
                         objAccessList.AccessPrivilegesList.Add(objAccess);
                     }
                     catch (Exception ex)
                     {
                         objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalList: " + ex.ToString());
                         TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                     }
                 }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmployeeCompetenceEvalList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAccessList.AccessPrivilegesList.ToList().ToPagedList(page ?? 1, 10000));
        }
    }
}