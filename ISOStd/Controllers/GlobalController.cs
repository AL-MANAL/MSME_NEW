using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class GlobalController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public JsonResult FunGetGEmpListByMultiDept(string dept)
        {
            try
            { 
               if (dept != "")
               {
                    MultiSelectList lstEmp = objGlobaldata.GetGEmpListByMultiDept(dept);
                    return Json(lstEmp);
               }            
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGEmpListByMultiDept: " + ex.ToString());
            }
            return Json("");
        }
       
        public JsonResult FunGetGEmpListbymultiBranch(string branch)
        {
            try
            {
                if (branch != "")
                {
                    MultiSelectList lstEmp = objGlobaldata.GetGEmpListbymultiBranch(branch);
                    return Json(lstEmp);
                }             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGEmpListbymultiBranch: " + ex.ToString());
            }
            return Json("");
        }

        public JsonResult FunGetGEmpListBymulitBDL(string sDivision = "", string sDept = "", string sLoc = "")
        {
            try
            {
              MultiSelectList lstEmp = objGlobaldata.GetGEmpListBymulitBDL(sDivision, sDept, sLoc);
              return Json(lstEmp);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGEmpListBymulitBDL: " + ex.ToString());
            }
            return Json("");
        }
      
        public JsonResult FunGetGLocListbymultiBranch(string branch)
        {            
            try
            {
                if (branch != "" && branch != null)
                {
                    MultiSelectList lstLoc = objGlobaldata.GetGLocListbymultiBranch(branch);
                    return Json(lstLoc);
                }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGLocListbymultiBranch: " + ex.ToString());
            }
            return Json("");
        }

        public JsonResult FunGetGDeptListbymultiBranch(string branch)
        {           
            try
            {               
                if (branch != "" && branch != null)
                {
                    MultiSelectList lstDept = objGlobaldata.GetGDeptListbymultiBranch(branch);
                    return Json(lstDept);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGDeptListbymultiBranch: " + ex.ToString());
            }
            return Json("");
        }

        public JsonResult FunGetGRoleList(string sDivision = "", string sDept = "", string sLoc = "",string sRole="")
        {           
            try
            {
                if (sRole != "")
                {
                    MultiSelectList lstRole = objGlobaldata.GetGRoleList(sDivision, sDept, sLoc, sRole);
                    return Json(lstRole);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGRoleList: " + ex.ToString());
            }
            return Json("");
        }

        public JsonResult FunGetGRoleLikeList(string sDivision = "", string sDept = "", string sLoc = "", string sRole = "")
        {
            try
            {
                if (sRole != "")
                {
                    MultiSelectList lstRole = objGlobaldata.GetGRolelikeList(sDivision, sDept, sLoc, sRole);
                    return Json(lstRole);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetGRoleLikeList: " + ex.ToString());
            }
            return Json("");
        }

        public JsonResult FunGetEmpDetailsbyId(string emp_no)
        {
            try
            {
                EmployeeModels objmodel = new EmployeeModels();
                string stmt = "Select division,Dept_Id,Emp_work_location,Designation from t_hr_employee where emp_no = '" + emp_no + "' and  emp_status=1";
                DataSet dsEmp = objGlobaldata.Getdetails(stmt);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    objmodel.division = objGlobaldata.GetCompanyBranchNameById(dsEmp.Tables[0].Rows[0]["division"].ToString());
                    objmodel.DeptID= objGlobaldata.GetDeptNameById(dsEmp.Tables[0].Rows[0]["Dept_Id"].ToString());
                    objmodel.Work_Location = objGlobaldata.GetDivisionLocationById(dsEmp.Tables[0].Rows[0]["Emp_work_location"].ToString());
                    objmodel.Designation = dsEmp.Tables[0].Rows[0]["Designation"].ToString();
                }
                return Json(objmodel);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetailsbyId: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

    }
}