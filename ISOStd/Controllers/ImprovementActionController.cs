using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using PagedList;
using PagedList.Mvc;
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    public class ImprovementActionController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public ImprovementActionController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "ImprovementAction";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddImprovement()
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementActionModels objImp = new ImprovementActionModels();
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                objImp.branch = objGlobaldata.GetCurrentUserSession().division;
                objImp.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objImp.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objImp.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objImp.branch);

                ViewBag.Complaint = objGlobaldata.GetDropdownList("Improvement Action Complaint");
                ViewBag.Severity = objGlobaldata.GetImprovementSeverityList();                
                ViewBag.Receivedby = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Disposition = objGlobaldata.GetDropdownList("Improvement Action Disposition");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Consequence = objGlobaldata.GetDropdownList("Improvement Action Consequence");
                ViewBag.MonitoringPeriod = objGlobaldata.GetDropdownList("Improvement Monitoring Period");
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddImprovement: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objImp);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        public JsonResult AddImprovement(FormCollection form, ImprovementActionModels objTRA)
        {
            ViewBag.SubMenutype = "ImprovementAction";
            try
            {
                objTRA.disposition = form["disposition"]; 
                objTRA.consequence = form["consequence"];
                objTRA.branch = form["branch"];
                objTRA.Department = form["Department"];
                objTRA.Location = form["Location"];

                DateTime dateValue;

                if (DateTime.TryParse(form["actoin_date"], out dateValue) == true)
                {
                    objTRA.actoin_date = dateValue;
                }

                if (DateTime.TryParse(form["completion_date"], out dateValue) == true)
                {
                    objTRA.completion_date = dateValue;
                }

                if (DateTime.TryParse(form["closedout_date"], out dateValue) == true)
                {
                    objTRA.closedout_date = dateValue;
                }
        
                if (objTRA.FunAddImprovement(objTRA))
                {
                    TempData["Successdata"] = "Added details successfully  with Reference Number '" + objTRA.ca_no + "'";
              
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddImprovement: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }
        
        [AllowAnonymous]
        public ActionResult ImprovementList(string branch_name)
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementList objActionList = new ImprovementList();
            objActionList.ActionList = new List<ImprovementActionModels>();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_action,ca_no,actoin_date,complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition," +
                    "qty_coordinator,analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,completion_date,monitoring_period,monitoring_value," +
                    "compeltion_action,assesed_risk,closedout_date,verifiedby,branch,Department,Location from t_improvement_action where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext ;

                DataSet dsAction = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsAction.Tables.Count > 0 && i < dsAction.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        ImprovementActionModels objTRAModels = new ImprovementActionModels
                        {
                            id_action = dsAction.Tables[0].Rows[i]["id_action"].ToString(),
                            ca_no = dsAction.Tables[0].Rows[i]["ca_no"].ToString(),
                            complaint = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["complaint"].ToString()),
                            complaint_desc = dsAction.Tables[0].Rows[i]["complaint_desc"].ToString(),
                            reference = dsAction.Tables[0].Rows[i]["reference"].ToString(),
                            non_conf = dsAction.Tables[0].Rows[i]["non_conf"].ToString(),
                            receivedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["receivedby"].ToString()),
                            risk_complaint = dsAction.Tables[0].Rows[i]["risk_complaint"].ToString(),
                            severity = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[i]["severity"].ToString()),
                            disposition = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["disposition"].ToString()),
                            qty_coordinator = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator"].ToString()),
                            analysis = dsAction.Tables[0].Rows[i]["analysis"].ToString(),
                            qty_coordinator1 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator1"].ToString()),
                            corrective_action = dsAction.Tables[0].Rows[i]["corrective_action"].ToString(),
                            qty_coordinator2 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator2"].ToString()),
                            consequence = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["consequence"].ToString()),
                            monitoring_period = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["monitoring_period"].ToString()),
                            monitoring_value = dsAction.Tables[0].Rows[i]["monitoring_value"].ToString(),
                            compeltion_action = dsAction.Tables[0].Rows[i]["compeltion_action"].ToString(),
                            assesed_risk = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[i]["assesed_risk"].ToString()),
                            verifiedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["verifiedby"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAction.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAction.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsAction.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["actoin_date"].ToString(), out dateValue))
                        {
                            objTRAModels.actoin_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["completion_date"].ToString(), out dateValue))
                        {
                            objTRAModels.completion_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["closedout_date"].ToString(), out dateValue))
                        {
                            objTRAModels.closedout_date = dateValue;
                        }
                        objActionList.ActionList.Add(objTRAModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objActionList.ActionList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ImprovementSearchList(string branch_name)
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementList objActionList = new ImprovementList();
            objActionList.ActionList = new List<ImprovementActionModels>();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_action,ca_no,actoin_date,complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition," +
                    "qty_coordinator,analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,completion_date,monitoring_period,monitoring_value," +
                    "compeltion_action,assesed_risk,closedout_date,verifiedby,branch,Department,Location from t_improvement_action where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext;

                DataSet dsAction = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsAction.Tables.Count > 0 && i < dsAction.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        ImprovementActionModels objTRAModels = new ImprovementActionModels
                        {
                            id_action = dsAction.Tables[0].Rows[i]["id_action"].ToString(),
                            ca_no = dsAction.Tables[0].Rows[i]["ca_no"].ToString(),
                            complaint = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["complaint"].ToString()),
                            complaint_desc = dsAction.Tables[0].Rows[i]["complaint_desc"].ToString(),
                            reference = dsAction.Tables[0].Rows[i]["reference"].ToString(),
                            non_conf = dsAction.Tables[0].Rows[i]["non_conf"].ToString(),
                            receivedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["receivedby"].ToString()),
                            risk_complaint = dsAction.Tables[0].Rows[i]["risk_complaint"].ToString(),
                            severity = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[i]["severity"].ToString()),
                            disposition = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["disposition"].ToString()),
                            qty_coordinator = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator"].ToString()),
                            analysis = dsAction.Tables[0].Rows[i]["analysis"].ToString(),
                            qty_coordinator1 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator1"].ToString()),
                            corrective_action = dsAction.Tables[0].Rows[i]["corrective_action"].ToString(),
                            qty_coordinator2 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["qty_coordinator2"].ToString()),
                            consequence = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[i]["consequence"].ToString()),
                            monitoring_period = dsAction.Tables[0].Rows[i]["monitoring_period"].ToString(),
                            monitoring_value = dsAction.Tables[0].Rows[i]["monitoring_value"].ToString(),
                            compeltion_action = dsAction.Tables[0].Rows[i]["compeltion_action"].ToString(),
                            assesed_risk = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[i]["assesed_risk"].ToString()),
                            verifiedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[i]["verifiedby"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAction.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAction.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsAction.Tables[0].Rows[i]["Location"].ToString()),

                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["actoin_date"].ToString(), out dateValue))
                        {
                            objTRAModels.actoin_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["completion_date"].ToString(), out dateValue))
                        {
                            objTRAModels.completion_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[i]["closedout_date"].ToString(), out dateValue))
                        {
                            objTRAModels.closedout_date = dateValue;
                        }
                        objActionList.ActionList.Add(objTRAModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in ImprovementSearchList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementSearchList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }


        [AllowAnonymous]
        public ActionResult ImprovementEdit()
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementActionModels objTRAModels = new ImprovementActionModels();
            try
            {
                ViewBag.Complaint = objGlobaldata.GetDropdownList("Improvement Action Complaint");               
                ViewBag.Severity = objGlobaldata.GetImprovementSeverityList();
                ViewBag.Receivedby = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Disposition = objGlobaldata.GetDropdownList("Improvement Action Disposition");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Consequence = objGlobaldata.GetDropdownList("Improvement Action Consequence");
                ViewBag.MonitoringPeriod = objGlobaldata.GetDropdownList("Improvement Monitoring Period");

              if (Request.QueryString["id_action"] != null)
                { 
                string sid_action = Request.QueryString["id_action"];

                string sSqlstmt = "select id_action,ca_no,actoin_date,complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition," +
                    "qty_coordinator,analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,completion_date,monitoring_period,monitoring_value," +
                    "compeltion_action,assesed_risk,closedout_date,verifiedby,branch,Department,Location from t_improvement_action where active=1 and id_action='" + sid_action + "'";

                DataSet dsAction = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAction.Tables.Count > 0 && dsAction.Tables[0].Rows.Count > 0)
                    {
                       
                         objTRAModels = new ImprovementActionModels
                        {
                            id_action = dsAction.Tables[0].Rows[0]["id_action"].ToString(),
                            ca_no = dsAction.Tables[0].Rows[0]["ca_no"].ToString(),
                            complaint = /*objGlobaldata.GetDropdownitemById*/(dsAction.Tables[0].Rows[0]["complaint"].ToString()),
                            complaint_desc = dsAction.Tables[0].Rows[0]["complaint_desc"].ToString(),
                            reference = dsAction.Tables[0].Rows[0]["reference"].ToString(),
                            non_conf = dsAction.Tables[0].Rows[0]["non_conf"].ToString(),
                            receivedby = /*objGlobaldata.GetEmpHrNameById*/(dsAction.Tables[0].Rows[0]["receivedby"].ToString()),
                            risk_complaint = dsAction.Tables[0].Rows[0]["risk_complaint"].ToString(),
                            severity = (dsAction.Tables[0].Rows[0]["severity"].ToString()),
                            disposition = (dsAction.Tables[0].Rows[0]["disposition"].ToString()),
                            qty_coordinator = /*objGlobaldata.GetEmpHrNameById*/(dsAction.Tables[0].Rows[0]["qty_coordinator"].ToString()),
                            analysis = dsAction.Tables[0].Rows[0]["analysis"].ToString(),
                            qty_coordinator1 = /*objGlobaldata.GetEmpHrNameById*/(dsAction.Tables[0].Rows[0]["qty_coordinator1"].ToString()),
                            corrective_action = dsAction.Tables[0].Rows[0]["corrective_action"].ToString(),
                            qty_coordinator2 =/* objGlobaldata.GetEmpHrNameById*/(dsAction.Tables[0].Rows[0]["qty_coordinator2"].ToString()),
                            consequence = /*objGlobaldata.GetDropdownitemById*/(dsAction.Tables[0].Rows[0]["consequence"].ToString()),
                            monitoring_period = /*objGlobaldata.GetDropdownitemById*/(dsAction.Tables[0].Rows[0]["monitoring_period"].ToString()),
                            monitoring_value = dsAction.Tables[0].Rows[0]["monitoring_value"].ToString(),
                            compeltion_action = dsAction.Tables[0].Rows[0]["compeltion_action"].ToString(),
                            assesed_risk = dsAction.Tables[0].Rows[0]["assesed_risk"].ToString(),
                            verifiedby = /*objGlobaldata.GetEmpHrNameById*/(dsAction.Tables[0].Rows[0]["verifiedby"].ToString()),
                            branch = (dsAction.Tables[0].Rows[0]["branch"].ToString()),
                            Department = dsAction.Tables[0].Rows[0]["Department"].ToString(),
                            Location = dsAction.Tables[0].Rows[0]["Location"].ToString(),

                         };

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsAction.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsAction.Tables[0].Rows[0]["branch"].ToString());

                        DateTime dateValue;
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["actoin_date"].ToString(), out dateValue))
                        {
                            objTRAModels.actoin_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["completion_date"].ToString(), out dateValue))
                        {
                            objTRAModels.completion_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["closedout_date"].ToString(), out dateValue))
                        {
                            objTRAModels.closedout_date = dateValue;
                        }                      

                    }
                    else
                        {
                            TempData["alertdata"] = "No Data exists";
                            return RedirectToAction("ImprovementList");
                        }                   
               }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ImprovementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }   

            return View(objTRAModels);
        }

        [HttpPost]
        [AllowAnonymous]     
        public JsonResult ImprovementEdit(FormCollection form, ImprovementActionModels objTRA)
        {
            ViewBag.SubMenutype = "ImprovementAction";
            try
            {
                objTRA.disposition = form["disposition"];
                objTRA.consequence = form["consequence"];
                objTRA.branch = form["branch"];
                objTRA.Department = form["Department"];
                objTRA.Location = form["Location"];

                DateTime dateValue;

                if (DateTime.TryParse(form["actoin_date"], out dateValue) == true)
                {
                    objTRA.actoin_date = dateValue;
                }

                if (DateTime.TryParse(form["completion_date"], out dateValue) == true)
                {
                    objTRA.completion_date = dateValue;
                }

                if (DateTime.TryParse(form["closedout_date"], out dateValue) == true)
                {
                    objTRA.closedout_date = dateValue;
                }

                if (objTRA.FunUpdateImplementation(objTRA))
                {
                    TempData["Successdata"] = "Added TRA successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult ImprovementDetails()
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementActionModels objTRAModels = new ImprovementActionModels();
            try
            {
                ViewBag.Complaint = objGlobaldata.GetDropdownList("Improvement Action Complaint");
                ViewBag.Severity = objGlobaldata.GetImprovementSeverityList();
                ViewBag.Receivedby = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Disposition = objGlobaldata.GetDropdownList("Improvement Action Disposition");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Consequence = objGlobaldata.GetDropdownList("Improvement Action Consequence");
                ViewBag.MonitoringPeriod = objGlobaldata.GetDropdownList("Improvement Monitoring Period");

                if (Request.QueryString["id_action"] != null)
                {
                    string sid_action = Request.QueryString["id_action"];

                    string sSqlstmt = "select id_action,ca_no,actoin_date,complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition," +
                        "qty_coordinator,analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,completion_date,monitoring_period,monitoring_value," +
                        "compeltion_action,assesed_risk,closedout_date,verifiedby,branch,Department,Location from t_improvement_action where active=1 and id_action='" + sid_action + "'";

                    DataSet dsAction = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAction.Tables.Count > 0 && dsAction.Tables[0].Rows.Count > 0)
                    {

                        objTRAModels = new ImprovementActionModels
                        {
                            id_action = dsAction.Tables[0].Rows[0]["id_action"].ToString(),
                            ca_no = dsAction.Tables[0].Rows[0]["ca_no"].ToString(),
                            complaint = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["complaint"].ToString()),
                            complaint_desc = dsAction.Tables[0].Rows[0]["complaint_desc"].ToString(),
                            reference = dsAction.Tables[0].Rows[0]["reference"].ToString(),
                            non_conf = dsAction.Tables[0].Rows[0]["non_conf"].ToString(),
                            receivedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["receivedby"].ToString()),
                            risk_complaint = dsAction.Tables[0].Rows[0]["risk_complaint"].ToString(),
                            severity = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[0]["severity"].ToString()),
                            disposition = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["disposition"].ToString()),
                            qty_coordinator = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator"].ToString()),
                            analysis = dsAction.Tables[0].Rows[0]["analysis"].ToString(),
                            qty_coordinator1 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator1"].ToString()),
                            corrective_action = dsAction.Tables[0].Rows[0]["corrective_action"].ToString(),
                            qty_coordinator2 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator2"].ToString()),
                            consequence = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["consequence"].ToString()),
                            monitoring_period = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["monitoring_period"].ToString()),
                            monitoring_value = dsAction.Tables[0].Rows[0]["monitoring_value"].ToString(),
                            compeltion_action = dsAction.Tables[0].Rows[0]["compeltion_action"].ToString(),
                            assesed_risk = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[0]["assesed_risk"].ToString()),
                            verifiedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["verifiedby"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAction.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAction.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsAction.Tables[0].Rows[0]["Location"].ToString()),

                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["actoin_date"].ToString(), out dateValue))
                        {
                            objTRAModels.actoin_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["completion_date"].ToString(), out dateValue))
                        {
                            objTRAModels.completion_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["closedout_date"].ToString(), out dateValue))
                        {
                            objTRAModels.closedout_date = dateValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ImprovementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ImprovementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objTRAModels);
        }

        [AllowAnonymous]
        public ActionResult ImprovementInfo(int id)
        {
            ViewBag.SubMenutype = "ImprovementAction";
            ImprovementActionModels objTRAModels = new ImprovementActionModels();
            try
            {  
                    string sSqlstmt = "select id_action,ca_no,actoin_date,complaint,complaint_desc,reference,non_conf,receivedby,risk_complaint,severity,disposition," +
                        "qty_coordinator,analysis,qty_coordinator1,corrective_action,qty_coordinator2,consequence,completion_date,monitoring_period,monitoring_value," +
                        "compeltion_action,assesed_risk,closedout_date,verifiedby,branch,Department,Location from t_improvement_action where active=1 and id_action='" + id + "'";

                    DataSet dsAction = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAction.Tables.Count > 0 && dsAction.Tables[0].Rows.Count > 0)
                    {

                        objTRAModels = new ImprovementActionModels
                        {
                            id_action = dsAction.Tables[0].Rows[0]["id_action"].ToString(),
                            ca_no = dsAction.Tables[0].Rows[0]["ca_no"].ToString(),
                            complaint = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["complaint"].ToString()),
                            complaint_desc = dsAction.Tables[0].Rows[0]["complaint_desc"].ToString(),
                            reference = dsAction.Tables[0].Rows[0]["reference"].ToString(),
                            non_conf = dsAction.Tables[0].Rows[0]["non_conf"].ToString(),
                            receivedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["receivedby"].ToString()),
                            risk_complaint = dsAction.Tables[0].Rows[0]["risk_complaint"].ToString(),
                            severity = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[0]["severity"].ToString()),
                            disposition = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["disposition"].ToString()),
                            qty_coordinator = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator"].ToString()),
                            analysis = dsAction.Tables[0].Rows[0]["analysis"].ToString(),
                            qty_coordinator1 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator1"].ToString()),
                            corrective_action = dsAction.Tables[0].Rows[0]["corrective_action"].ToString(),
                            qty_coordinator2 = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["qty_coordinator2"].ToString()),
                            consequence = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["consequence"].ToString()),
                            monitoring_period = objGlobaldata.GetDropdownitemById(dsAction.Tables[0].Rows[0]["monitoring_period"].ToString()),
                            monitoring_value = dsAction.Tables[0].Rows[0]["monitoring_value"].ToString(),
                            compeltion_action = dsAction.Tables[0].Rows[0]["compeltion_action"].ToString(),
                            assesed_risk = objGlobaldata.GetImprovementSeverityById(dsAction.Tables[0].Rows[0]["assesed_risk"].ToString()),
                            verifiedby = objGlobaldata.GetEmpHrNameById(dsAction.Tables[0].Rows[0]["verifiedby"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAction.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsAction.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsAction.Tables[0].Rows[0]["Location"].ToString()),

                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["actoin_date"].ToString(), out dateValue))
                        {
                            objTRAModels.actoin_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["completion_date"].ToString(), out dateValue))
                        {
                            objTRAModels.completion_date = dateValue;
                        }
                        if (DateTime.TryParse(dsAction.Tables[0].Rows[0]["closedout_date"].ToString(), out dateValue))
                        {
                            objTRAModels.closedout_date = dateValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ImprovementList");
                    }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objTRAModels);
        }

        [AllowAnonymous]
        public JsonResult ImprovementDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ImprovementAction";

                if (form["id_action"] != null && form["id_action"] != "")
                {

                    ImprovementActionModels Doc = new ImprovementActionModels();
                    string sid_action = form["id_action"];


                    if (Doc.FunDeleteImplementationDoc(sid_action))
                    {
                        TempData["Successdata"] = "Improvement Action deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImprovementDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult FunGetDeptHead(string qty_coordinator)
        {
            ImprovementActionModels objMdl = new ImprovementActionModels();
            string DeptHead = "";
            if (qty_coordinator != null && qty_coordinator != "")
            {
                string sql = "select EvaluatedBy from t_hr_employee where emp_no= '" + qty_coordinator + "'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    DeptHead =objGlobaldata.GetEmpHrNameById(dsData.Tables[0].Rows[0]["EvaluatedBy"].ToString());
                }
            }
            return Json(DeptHead);
        }


    }
}