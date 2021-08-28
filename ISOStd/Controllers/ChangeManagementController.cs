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
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ChangeManagementController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public ChangeManagementController()
        {
            //ViewBag.Menutype = "Risk";
            ViewBag.SubMenutype = "ChangeManagement";
        }
         
        public ActionResult AddChangeManagement()
        {
            return InitializeChangeManagement();
        }
                 
        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetDeptHeadList(DeptId);
            return Json(lstEmp);
        }
                 
        public ActionResult InitializeChangeManagement()
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ManagementChangeModels objMgmt = new ManagementChangeModels();
            try {
                objMgmt.branch = objGlobaldata.GetCurrentUserSession().division;
                objMgmt.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objMgmt.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                RiskMgmtModels objRisk = new RiskMgmtModels();
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.Samples = objGlobaldata.GetDropdownList("ManagementSample");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                //ViewBag.EmpLists = objGlobaldata.GetGEmpListBymulitBDL(objMgmt.branch, objMgmt.Department, objMgmt.Location);
                //ViewBag.Approver = objGlobaldata.GetGRoleList(objMgmt.branch, objMgmt.Department, objMgmt.Location, "Approver");
                ViewBag.Approver = objGlobaldata.GetApprover();
                ViewBag.ActionStatus = objGlobaldata.GetConstantValue("Action_status");
                ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                ViewBag.changetype = objGlobaldata.GetConstantValue("change_type");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objMgmt.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objMgmt.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitializeChangeManagement: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMgmt);
        }
                
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddChangeManagement(ManagementChangeModels objManagement, FormCollection form)
        {
            ViewBag.SubMenutype = "ChangeManagement";
            try
            {
                if (objManagement != null)
                { 
                    objManagement.ChangeRequestedBy = form["ChangeRequestedBy"];
                    objManagement.ChangeIn = form["ChangeIn"];
                    objManagement.DetailsOfChange = form["DetailsOfChange"];
                    objManagement.ResonForChange = form["ResonForChange"];
                    objManagement.Impact = form["Impact"];
                    objManagement.ApprovedBy = form["ApprovedBy"];
                    objManagement.branch = form["branch"];
                    objManagement.Department = form["Department"];
                    objManagement.Location = form["Location"];

                    //To count the no of approver selected

                    objManagement.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                    
                    ChangeManagementModelsList obManagmentModelsList = new ChangeManagementModelsList();
                    obManagmentModelsList.ChangeMgmtList = new List<ManagementChangeModels>();


                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        ManagementChangeModels objManagementModels = new ManagementChangeModels();

                        DateTime dateValue;

                        if (DateTime.TryParse(form["TargetDate" + i], out dateValue) == true)
                        {
                            objManagementModels.TargetDate = dateValue;
                        }
                      
                        objManagementModels.Action = form["Action" + i];
                        objManagementModels.PersonResponsible = form["PersonResponsible" + i];
                        objManagementModels.Action_Status = form["Action_Status" + i];
                        obManagmentModelsList.ChangeMgmtList.Add(objManagementModels);
                    }

                    if (objManagement.FunAddChangeManagement(objManagement))
                    {
                        TempData["Successdata"] = "Added Change Management details successfully";

                       
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddObjectives: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ChangeManagementList");
        }
        
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddChangeManagementActions()
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ManagementChangeModels objManagementModels = new ManagementChangeModels();

            try
            {             
                if (Request.QueryString["IdMgmt"] != null)
                {
                    string sIdMgmt = Request.QueryString["IdMgmt"];
                    string sSqlstmt = "select IdMgmt,(case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,branch,Department,Location"
                    + "  from t_changemanagement where IdMgmt='" + sIdMgmt + "'";

                    DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                    {
                        objManagementModels = new ManagementChangeModels
                        {
                            IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[0]["IdMgmt"].ToString()),
                            ApproveStatus = dsManagementModelsList.Tables[0].Rows[0]["ApproveStatus"].ToString()
                        };

                        ChangeManagementModelsList objAttList = new ChangeManagementModelsList();
                        objAttList.ChangeMgmtList = new List<ManagementChangeModels>();

                        sSqlstmt = "select Id,IdMgmt,Action,TargetDate,ActionCompletionDate,PersonResponsible,Action_Status,ActionCompletionDate"
                       + " from t_changemanagement_actions where IdMgmt='" + sIdMgmt + "' order by Id";                 
                        DataSet dsActnList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsActnList.Tables.Count > 0 && dsActnList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsActnList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ManagementChangeModels objActn = new ManagementChangeModels
                                    {
                                        Id =Convert.ToInt32(dsActnList.Tables[0].Rows[i]["Id"].ToString()),
                                        IdMgmt = Convert.ToInt32(dsActnList.Tables[0].Rows[i]["IdMgmt"].ToString()),
                                        Action = dsActnList.Tables[0].Rows[i]["Action"].ToString(),
                                        PersonResponsible = dsActnList.Tables[0].Rows[i]["PersonResponsible"].ToString(),
                                        Action_Status = dsActnList.Tables[0].Rows[i]["Action_Status"].ToString(),
                                    };
                                    DateTime dtDocDate;
                                    if (dsActnList.Tables[0].Rows[i]["TargetDate"].ToString() != ""
                                       && DateTime.TryParse(dsActnList.Tables[0].Rows[i]["TargetDate"].ToString(), out dtDocDate))
                                    {
                                        objActn.TargetDate = dtDocDate;
                                    }
                                    if (dsActnList.Tables[0].Rows[i]["ActionCompletionDate"].ToString() != ""
                                     && DateTime.TryParse(dsActnList.Tables[0].Rows[i]["ActionCompletionDate"].ToString(), out dtDocDate))
                                    {
                                        objActn.ActionCompletionDate = dtDocDate;
                                    }
                                    objAttList.ChangeMgmtList.Add(objActn);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddChangeManagementActions: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("ChangeManagementList");
                                }
                            }
                            ViewBag.objAttnList = objAttList;
                        }
                       // ViewBag.EmpList = objGlobaldata.GetGEmpListBymulitBDL(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Department"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Location"].ToString());

                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();               
                        ViewBag.ActionStatus = objGlobaldata.GetConstantValue("Action_status");
                        return View(objManagementModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ChangeManagementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("ChangeManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddChangeManagementActions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ChangeManagementList");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddChangeManagementActions(ManagementChangeModels objManagement, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                ChangeManagementModelsList obManagmentModelsList = new ChangeManagementModelsList();
                    obManagmentModelsList.ChangeMgmtList = new List<ManagementChangeModels>();


                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        ManagementChangeModels objManagementModels = new ManagementChangeModels();

                        DateTime dateValue;

                        if (form["Action" + i] != null && form["Action" + i] != "")
                        {
                            if (DateTime.TryParse(form["TargetDate" + i], out dateValue) == true)
                            {
                                objManagementModels.TargetDate = dateValue;
                            }
                            if (DateTime.TryParse(form["ActionCompletionDate" + i], out dateValue) == true)
                            {
                                objManagementModels.ActionCompletionDate = dateValue;
                            }
                            objManagementModels.Action = form["Action" + i];
                            objManagementModels.PersonResponsible = form["PersonResponsible" + i];
                            objManagementModels.Action_Status = form["Action_Status" + i];
                            obManagmentModelsList.ChangeMgmtList.Add(objManagementModels);
                        }

                    }

                    if (objManagement.FunAddChangeMgmtActions(obManagmentModelsList, objManagement.IdMgmt))
                    {
                        TempData["Successdata"] = "Added Actions successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddChangeManagementActions: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ChangeManagementList");
        }
       
        [AllowAnonymous]
        public ActionResult ChangeManagementList(string SearchText, string ChangeIn, string Approvestatus, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ChangeManagementModelsList objManagemtModelsList = new ChangeManagementModelsList();
            objManagemtModelsList.ChangeMgmtList = new List<ManagementChangeModels>();
           
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {

                ViewBag.ChangeIn = objGlobaldata.GetDropdownList("ManagementSample");
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.User = objUser.firstname;
                
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,Approvers,ChangeType,RiskLevel,"
                    + " (case when ApproveStatus='1' then 'Approved'  when ApproveStatus='2' then 'Rejected' else 'Pending' end) as ApproveStatus,branch,Department,Location from t_changemanagement where Active=1";

                string sSearchtext = "";

                //if (SearchText != null && SearchText != "")
                //{
                //    ViewBag.SearchText = SearchText;
                //    sSearchtext = sSearchtext + "  and (ChangeRequestNo ='" + SearchText + "' or ChangeRequestNo like '" + SearchText + "%' or ChangeRequestNo like '%" + SearchText + "%' or ChangeRequestNo='" + SearchText + "')";
                //}

                if (ChangeIn != null && ChangeIn != "Select")
                {
                    ViewBag.Change_In = ChangeIn;

                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                    }
                }
                if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                {
                    ViewBag.ApprovestatusVal = Approvestatus;
                    if (sSearchtext != "" || ChangeIn !="")
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by ChangeInitiatedDate desc";
                DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                { 
                    for (int i = 0; i < dsManagementModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ManagementChangeModels objManagementModels = new ManagementChangeModels
                            {
                                IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[i]["IdMgmt"].ToString()),
                                
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                ChangeRequestedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["ChangeRequestedBy"].ToString()),
                                ChangeIn = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["ChangeIn"].ToString()),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsManagementModelsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ApproveStatus = dsManagementModelsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                            
                                Approvers = dsManagementModelsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                RiskLevel = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["RiskLevel"].ToString()),
                                ChangeType = dsManagementModelsList.Tables[0].Rows[i]["ChangeType"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsManagementModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsManagementModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsManagementModelsList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString() != ""
                                && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[i]["ChangeInitiatedDate"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ChangeInitiatedDate = dtDocDate;
                            }
                            
                            if (dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString() != ""
                          && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[i]["ApproveOrRejectOn"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ApproveOrRejectOn = dtDocDate;
                            }
                            string Sql = "select Action_Status from t_changemanagement_actions where IdMgmt='" + objManagementModels.IdMgmt + "'";
                            DataSet dsData = objGlobaldata.Getdetails(Sql);
                            string action_status = "";
                            if (dsData.Tables.Count > 0)
                            {
                                for (int j = 0; j < dsData.Tables[0].Rows.Count; j++)
                                {
                                    if (dsData.Tables[0].Rows[j]["Action_Status"].ToString() == "Completed")
                                    {
                                        action_status = (dsData.Tables[0].Rows[j]["Action_Status"].ToString());
                                    }
                                    else
                                    {
                                        action_status = (dsData.Tables[0].Rows[j]["Action_Status"].ToString());
                                        break;
                                    }
                                }
                            }
                            objManagementModels.ChangeStatus = action_status;
                        
                            objManagemtModelsList.ChangeMgmtList.Add(objManagementModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ChangeManagementList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objManagemtModelsList.ChangeMgmtList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ChangeManagementListSearch(string SearchText, string ChangeIn, string Approvestatus, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ChangeManagementModelsList objManagemtModelsList = new ChangeManagementModelsList();
            objManagemtModelsList.ChangeMgmtList = new List<ManagementChangeModels>();

            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {

                ViewBag.ChangeIn = objGlobaldata.GetDropdownList("ManagementSample");
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.User = objUser.firstname;

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,Approvers,ChangeType,RiskLevel,"
                    + " (case when ApproveStatus='1' then 'Approved'  when ApproveStatus='2' then 'Rejected' else 'Pending' end) as ApproveStatus,branch,Department,Location from t_changemanagement where Active=1";

                string sSearchtext = "";

                //if (SearchText != null && SearchText != "")
                //{
                //    ViewBag.SearchText = SearchText;
                //    sSearchtext = sSearchtext + "  and (ChangeRequestNo ='" + SearchText + "' or ChangeRequestNo like '" + SearchText + "%' or ChangeRequestNo like '%" + SearchText + "%' or ChangeRequestNo='" + SearchText + "')";
                //}

                if (ChangeIn != null && ChangeIn != "Select")
                {
                    ViewBag.Change_In = ChangeIn;

                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (ChangeIn ='" + ChangeIn + "')";
                    }
                }
                if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                {
                    ViewBag.ApprovestatusVal = Approvestatus;
                    if (sSearchtext != "" || ChangeIn != "")
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " and (ApproveStatus ='" + Approvestatus + "')";
                    }
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by ChangeInitiatedDate desc";
                DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                {  
                    for (int i = 0; i < dsManagementModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            ManagementChangeModels objManagementModels = new ManagementChangeModels
                            {
                                IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[i]["IdMgmt"].ToString()),

                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                ChangeRequestedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["ChangeRequestedBy"].ToString()),
                                ChangeIn = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["ChangeIn"].ToString()),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsManagementModelsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ApproveStatus = dsManagementModelsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),

                                Approvers = dsManagementModelsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                RiskLevel = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["RiskLevel"].ToString()),
                                ChangeType = dsManagementModelsList.Tables[0].Rows[i]["ChangeType"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsManagementModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsManagementModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsManagementModelsList.Tables[0].Rows[i]["Location"].ToString()),

                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString() != ""
                                && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[i]["ChangeInitiatedDate"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ChangeInitiatedDate = dtDocDate;
                            }

                            if (dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString() != ""
                          && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[i]["ApproveOrRejectOn"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ApproveOrRejectOn = dtDocDate;
                            }
                            string Sql = "select Action_Status from t_changemanagement_actions where IdMgmt='" + objManagementModels.IdMgmt + "'";
                            DataSet dsData = objGlobaldata.Getdetails(Sql);
                            string action_status = "";
                            if (dsData.Tables.Count > 0)
                            {
                                for (int j = 0; j < dsData.Tables[0].Rows.Count; j++)
                                {
                                    if (dsData.Tables[0].Rows[j]["Action_Status"].ToString() == "Completed")
                                    {
                                        action_status = (dsData.Tables[0].Rows[j]["Action_Status"].ToString());
                                    }
                                    else
                                    {
                                        action_status = (dsData.Tables[0].Rows[j]["Action_Status"].ToString());
                                        break;
                                    }
                                }
                            }
                            objManagementModels.ChangeStatus = action_status;

                            objManagemtModelsList.ChangeMgmtList.Add(objManagementModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ChangeManagementListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult ChangeManagementInfo(int id)
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ChangeManagementModelsList objManagemtModelsList = new ChangeManagementModelsList();
            objManagemtModelsList.ChangeMgmtList = new List<ManagementChangeModels>();

            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ApproveOrRejectOn,Approvers,ChangeType,RiskLevel,"
                    + " (case when ApproveStatus='1' then 'Approved' else 'Not Approved' end) as ApproveStatus,(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus,branch,Department,Location"
                + " from t_changemanagement where Active=1 and IdMgmt='"+id+"'";

                DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if(dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsManagementModelsList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            ManagementChangeModels objManagementModels = new ManagementChangeModels
                            {
                                IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[i]["IdMgmt"].ToString()),

                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                ChangeRequestedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[i]["ChangeRequestedBy"].ToString()),
                                ChangeIn = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["ChangeIn"].ToString()),
                                ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsManagementModelsList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                                ApproveStatus = dsManagementModelsList.Tables[0].Rows[i]["ApproveStatus"].ToString(),
                                ChangeStatus = dsManagementModelsList.Tables[0].Rows[i]["ChangeStatus"].ToString(),
                                Approvers = dsManagementModelsList.Tables[0].Rows[i]["Approvers"].ToString(),
                                RiskLevel = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[i]["RiskLevel"].ToString()),
                                ChangeType = dsManagementModelsList.Tables[0].Rows[i]["ChangeType"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsManagementModelsList.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsManagementModelsList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsManagementModelsList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString() != ""
                                && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ChangeInitiatedDate = dtDocDate;
                            }

                            if (dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString() != ""
                          && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString(), out dtDocDate))
                            {
                                objManagementModels.ApproveOrRejectOn = dtDocDate;
                            }

                            objManagemtModelsList.ChangeMgmtList.Add(objManagementModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ChangeManagementList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    sSqlstmt = "select Id,IdMgmt,Action,TargetDate,ActionCompletionDate,PersonResponsible,Action_Status"
                        + " from t_changemanagement_actions where IdMgmt='" + id + "' order by Id";
                    ViewBag.ActionDetails = objGlobaldata.Getdetails(sSqlstmt);
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objManagemtModelsList.ChangeMgmtList.ToList());
        }
                 
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult ChangeManagementEdit(int? page)
        {
            ViewBag.SubMenutype = "ChangeManagement";
            ChangeManagementModelsList objManagementModelsList = new ChangeManagementModelsList();
            objManagementModelsList.ChangeMgmtList = new List<ManagementChangeModels>();
            RiskMgmtModels objRisk = new RiskMgmtModels();
            ManagementChangeModels objManagementModels = new ManagementChangeModels();
           
            try
            {
                UserCredentials objUser = new UserCredentials();
                ViewBag.impact_id = objGlobaldata.GetDropdownList("Risk-Severity");
                ViewBag.changetype = objGlobaldata.GetConstantValue("change_type");

                if (Request.QueryString["IdMgmt"] != null)
                {
                    string sIdMgmt = Request.QueryString["IdMgmt"];
                    string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,ChangeType,RiskLevel,branch,Department,Location"
                    + "  from t_changemanagement where IdMgmt='" + sIdMgmt + "'";

                    DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                    {

                        objManagementModels = new ManagementChangeModels
                        {
                            IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[0]["IdMgmt"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            ChangeRequestedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()),
                            ChangeIn = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[0]["ChangeIn"].ToString()),
                            ApprovedBy = (dsManagementModelsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            DetailsOfChange = dsManagementModelsList.Tables[0].Rows[0]["DetailsOfChange"].ToString(),
                            ResonForChange = dsManagementModelsList.Tables[0].Rows[0]["ResonForChange"].ToString(),
                            Impact = dsManagementModelsList.Tables[0].Rows[0]["Impact"].ToString(),
                            RiskLevel = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[0]["RiskLevel"].ToString()),
                            ChangeType = dsManagementModelsList.Tables[0].Rows[0]["ChangeType"].ToString(),
                            ChangeInitiatedDate = Convert.ToDateTime(dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString()),
                            branch = (dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsManagementModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location =(dsManagementModelsList.Tables[0].Rows[0]["Location"].ToString()),
                          };
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString());
                        //  ViewBag.EmpLists = objGlobaldata.GetGEmpListBymulitBDL(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Department"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Location"].ToString());
                        //ViewBag.Approver = objGlobaldata.GetGRoleList(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Department"].ToString(), dsManagementModelsList.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.Approver = objGlobaldata.GetApprover();

                        sSqlstmt = "select Id,IdMgmt,Action,TargetDate,PersonResponsible,Action_Status"
                        + " from t_changemanagement_actions where IdMgmt='" + sIdMgmt + "' order by Id";
                        ViewBag.ActionDetails = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                        ViewBag.Samples = objGlobaldata.GetDropdownList("ManagementSample");
                        ViewBag.ActionStatus = objGlobaldata.GetConstantValue("Action_status");
                        return View(objManagementModels);
                    }
                    else
                    {
                       
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ChangeManagementList");

                    }
                }
                else
                {
                  
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("ChangeManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ChangeManagementList");
        }
                 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult ChangeManagementEdit(ManagementChangeModels objManagement, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                objManagement.ChangeRequestedBy = form["ChangeRequestedBy"];
                objManagement.ChangeIn = form["ChangeIn"];
                objManagement.DetailsOfChange = form["DetailsOfChange"];
                objManagement.ResonForChange = form["ResonForChange"];
                objManagement.Impact = form["Impact"];
                objManagement.ApprovedBy = form["ApprovedBy"];
                objManagement.branch = form["branch"];
                objManagement.Department = form["Department"];
                objManagement.Location = form["Location"];

                objManagement.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                

                if (objManagement.FunUpdateChangeManagement(objManagement))
                {
                    TempData["Successdata"] = "Change Management details updated successfully";
                    
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ChangeManagementList", new { IdMgmt = objManagement.IdMgmt });
        }
                 
        [AllowAnonymous]
        public ActionResult ChangeManagementActionUpdate(ManagementChangeModels objManagementModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                if (Request["button"].Equals("Save"))
                {
                    if (ManagementActionAdd(objManagementModels, form))
                    {
                        TempData["Successdata"] = "Added Action Details successfully";
                        //objManagementModels.SendChangeEditMgmtmail(objManagementModels.IdMgmt, "Change Management Request for Approval");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                   
                    DateTime dateValue;

                    if (DateTime.TryParse(form["TargetDate"], out dateValue) == true)
                    {
                        objManagementModels.TargetDate = dateValue;
                    }

                    objManagementModels.Action = form["Action"];
                    objManagementModels.PersonResponsible = form["PersonResponsible"];
                    objManagementModels.Action_Status = form["Action_Status"];


                    if (objManagementModels.FunUpdateManagementChangeAction(objManagementModels))
                    {
                        TempData["Successdata"] = "Action plan updated successfully";
                        //objManagementModels.SendChangeEditMgmtmail(objManagementModels.IdMgmt, "Change Management Request for Approval");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                
                //return objObjectivesModels.FunUpdateObjectivesPlan(objObjectivesModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementActionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ChangeManagementEdit", new { IdMgmt = objManagementModels.IdMgmt });
        }
                 
        [AllowAnonymous]
        public bool ManagementActionAdd(ManagementChangeModels objManagementModels, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                ChangeManagementModelsList objManagementModelsList = new ChangeManagementModelsList();
                objManagementModelsList.ChangeMgmtList = new List<ManagementChangeModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["TargetDate"], out dateValue) == true)
                {
                    objManagementModels.TargetDate = dateValue;
                }

                objManagementModels.Action = form["Action"];
                objManagementModels.PersonResponsible = form["PersonResponsible"];
                objManagementModels.Action_Status = form["Action_Status"];

                objManagementModelsList.ChangeMgmtList.Add(objManagementModels);

                if (objManagementModels.FunAddChangeMgmtActions(objManagementModelsList, Convert.ToInt16(objManagementModels.IdMgmt)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
                 
        [AllowAnonymous]
        public ActionResult ChangeManagementDetails()
        {
            ManagementChangeModels objManagementModels = new ManagementChangeModels();
            UserCredentials objUser = new UserCredentials();   
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";

                objUser = objGlobaldata.GetCurrentUserSession();
                    ViewBag.user = objUser.firstname;
                
                if (Request.QueryString["IdMgmt"] != null && Request.QueryString["IdMgmt"] != "")
                {
                    string sIdMgmt = Request.QueryString["IdMgmt"];

                    string sSqlstmt = "select IdMgmt,ChangeInitiatedDate,LoggedBy,ChangeRequestedBy,ChangeIn,DetailsOfChange,ResonForChange,Impact,ApprovedBy,CompletionDate,ChangeType,RiskLevel,"
                        + " (case when ApproveStatus='1' then 'Approved' when ApproveStatus='2' then 'Rejected' else 'Pending' end) as ApproveStatus,ApproveStatus as ApproveStatusId,(case when ChangeStatus='1' then 'Completed' else 'Pending' end) as ChangeStatus , ApproveOrRejectOn,branch,Department,Location"
                        + " from t_changemanagement where IdMgmt='" + sIdMgmt + "'";

                    DataSet dsManagementModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsManagementModelsList.Tables.Count > 0 && dsManagementModelsList.Tables[0].Rows.Count > 0)
                    {
                        objManagementModels = new ManagementChangeModels
                        {
                            IdMgmt = Convert.ToInt16(dsManagementModelsList.Tables[0].Rows[0]["IdMgmt"].ToString()),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            ChangeRequestedBy = objGlobaldata.GetEmpHrNameById(dsManagementModelsList.Tables[0].Rows[0]["ChangeRequestedBy"].ToString()),
                            ChangeIn = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[0]["ChangeIn"].ToString()),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsManagementModelsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ApproveStatus = dsManagementModelsList.Tables[0].Rows[0]["ApproveStatus"].ToString(),
                            DetailsOfChange = dsManagementModelsList.Tables[0].Rows[0]["DetailsOfChange"].ToString(),
                            ResonForChange = dsManagementModelsList.Tables[0].Rows[0]["ResonForChange"].ToString(),
                            Impact = dsManagementModelsList.Tables[0].Rows[0]["Impact"].ToString(),
                            ChangeStatus = dsManagementModelsList.Tables[0].Rows[0]["ChangeStatus"].ToString(),
                            RiskLevel = objGlobaldata.GetDropdownitemById(dsManagementModelsList.Tables[0].Rows[0]["RiskLevel"].ToString()),
                            ChangeType = dsManagementModelsList.Tables[0].Rows[0]["ChangeType"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsManagementModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsManagementModelsList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsManagementModelsList.Tables[0].Rows[0]["Location"].ToString()),
                            ApproveStatusId = dsManagementModelsList.Tables[0].Rows[0]["ApproveStatusId"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString() != ""
                            && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[0]["ChangeInitiatedDate"].ToString(), out dtDocDate))
                        {
                            objManagementModels.ChangeInitiatedDate = dtDocDate;
                        }
                        if (dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString() != ""
                           && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[0]["ApproveOrRejectOn"].ToString(), out dtDocDate))
                        {
                            objManagementModels.ApproveOrRejectOn = dtDocDate;
                        }
                        if (dsManagementModelsList.Tables[0].Rows[0]["CompletionDate"].ToString() != ""
                           && DateTime.TryParse(dsManagementModelsList.Tables[0].Rows[0]["CompletionDate"].ToString(), out dtDocDate))
                        {
                            objManagementModels.CompletionDate = dtDocDate;
                        }

                        sSqlstmt = "select Id,IdMgmt,Action,TargetDate,ActionCompletionDate,PersonResponsible,Action_Status"
                         + " from t_changemanagement_actions where IdMgmt='" + sIdMgmt + "' order by Id";
                        ViewBag.ActionDetails = objGlobaldata.Getdetails(sSqlstmt);

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("ChangeManagementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("ChangeManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objManagementModels);

        }
                 
        [AllowAnonymous]
        public ActionResult ChangeManagementApproveReject(string IdMgmt, int iStatus, string PendingFlg)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                ManagementChangeModels objManagementModels = new ManagementChangeModels();

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";

                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";

                }
                if (objManagementModels.FunChangeManagementRequestApproveOrReject(IdMgmt, iStatus))
                {
                    TempData["Successdata"] = "Change Management request" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("ChangeManagementList");
            }
        }
               
        public JsonResult ChangeManagementApproveRejectNoty(string IdMgmt, int iStatus, string PendingFlg)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                ManagementChangeModels objManagementModels = new ManagementChangeModels();

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";

                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";

                }
                if (objManagementModels.FunChangeManagementRequestApproveOrReject(IdMgmt, iStatus))
                {
                    TempData["Successdata"] = "Change Management request" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success"+ iStatus);
            }
            else
            {
                return Json("Failed");
            }
        }
        
        [AllowAnonymous]
        public ActionResult UpdateChangeManagementAction()
        {
            try
            {
                ManagementChangeModels objMgmt = new ManagementChangeModels();


                if (Request.QueryString["IdMgmt"] != null)
                {
                    string Id = Request.QueryString["IdMgmt"];
                    string sSqlstmt = "select Id,IdMgmt,Action,TargetDate,PersonResponsible,Action_Status"
                    + " from t_changemanagement_actions where  Id='" + Id + "' order by Id";
                    ViewBag.ActionDetails = objGlobaldata.Getdetails(sSqlstmt);

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.ActionStatus = objGlobaldata.GetConstantValue("Action_status");
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("ChangeManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateChangeManagementAction: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangeManagementUpdatePlan(ManagementChangeModels objManagementModels, FormCollection form)
        {
            try
            {
                    DateTime dateValue;

                    if (DateTime.TryParse(form["TargetDate"], out dateValue) == true)
                    {
                        objManagementModels.TargetDate = dateValue;
                    }

                    objManagementModels.Action = form["Action"];
                    objManagementModels.PersonResponsible = form["PersonResponsible"];
                    objManagementModels.Action_Status = form["Action_Status"];


                    if (objManagementModels.FunUpdateManagementActionPlan(objManagementModels))
                    {
                        TempData["Successdata"] = "Action plan updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                    return RedirectToAction("ChangeManagementList");
                //return objObjectivesModels.FunUpdateObjectivesPlan(objObjectivesModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementUpdatePlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ChangeManagementList");
        }
                 
        [AllowAnonymous]
        public JsonResult ChangeManagementDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ChangeManagement";
                if (form["IdMgmt"] != null && form["IdMgmt"] != "")
                        {
                            ManagementChangeModels Doc = new ManagementChangeModels();
                            string sIdMgmt = form["IdMgmt"];


                            if (Doc.FunDeleteChangeMgmtDoc(sIdMgmt))
                            {
                                TempData["Successdata"] = "Document deleted successfully";
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
                            TempData["alertdata"] = "Change Management Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}