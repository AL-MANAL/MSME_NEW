using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ActionTrackingRegisterController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public ActionTrackingRegisterController()
        {
            ViewBag.Menutype = "Actions";
            ViewBag.SubMenutype = "ActionTrackingRegister";
        }

        [AllowAnonymous]
        public ActionResult AddActionTrackingRegister()
        {
            try
            {
                ViewBag.SubMenutype = "ActionTrackingRegister";

                ViewBag.Status = objGlobaldata.GetDropdownList("ActionTrackingRegister Status");
                ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Source = objGlobaldata.GetATSSourceFullFormList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddActionTrackingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddActionTrackingRegister(ActionTrackingRegisterModels objTrack, FormCollection form)
        {
            try
            {
                objTrack.ActionBy = form["ActionBy"];
                objTrack.Status = form["Status"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Target_date"], out dateValue) == true)
                {
                    objTrack.Target_date = dateValue;
                }
                if (DateTime.TryParse(form["Due_date"], out dateValue) == true)
                {
                    objTrack.Due_date = dateValue;
                }
                if (DateTime.TryParse(form["Completion_date"], out dateValue) == true)
                {
                    objTrack.Completion_date = dateValue;
                }
                int Notificationval = 1;

                if (objTrack.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objTrack.NotificationValue = "0";
                }
                else if (objTrack.NotificationPeriod == "Weeks" && int.TryParse(objTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objTrack.NotificationPeriod == "Months" && int.TryParse(objTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objTrack.NotificationDays = Notificationval;

                if (objTrack.FunAddActionTrackingRegister(objTrack))
                {
                    TempData["Successdata"] = "Action Tracking Register details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddActionTrackingRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ActionTrackingRegisterList");
        }

        [AllowAnonymous]
        public ActionResult ActionTrackingRegisterList(int? page, string chkAll, string source_id, string Status, string command, string branch_name)
        {
            ViewBag.SubMenutype = "ActionTrackingRegister";

            string currentuser = "";
            ViewBag.Source = objGlobaldata.GetATSSourceFullFormList();
            ViewBag.Status = objGlobaldata.GetDropdownList("ActionTrackingRegister Status");

            ActionTrackingRegisterModelsList objTrackList = new ActionTrackingRegisterModelsList();
            objTrackList.lstTrack = new List<ActionTrackingRegisterModels>();

            ActionTrackingRegisterModels objTrackingModels = new ActionTrackingRegisterModels();

            try
            {
                currentuser = objGlobaldata.GetCurrentUserSession().empid;
                string role = objGlobaldata.GetCurrentUserSession().role;
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                //employeeid = objGlobaldata.GetEmployeeCompEmpIdByEmpId(currentuser);

                string sSqlstmt = "select id_action,Action_name,Target_date,Due_date,Completion_date,ActionBy,Status,Remarks,NotificationDays,NotificationPeriod,NotificationValue,source_id,report_no"
                + " from t_action_tracking_register where Active=1 ";

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (source_id != "" && source_id != null)
                    {
                        ViewBag.Sourceval = objGlobaldata.GetATSSourceFullFormById(source_id);
                        sSearchtext = sSearchtext + " and source_id ='" + source_id + "'";
                    }
                    if (Status != "" && Status != null)
                    {
                        ViewBag.Statusval = objGlobaldata.GetDropdownitemById(Status);
                        sSearchtext = sSearchtext + " and Status ='" + Status + "'";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (objGlobaldata.GetMultiRolesNameById(role).Contains("Admin"))
                {
                    sSqlstmt = sSqlstmt + sSearchtext;
                }
                else
                {
                    sSqlstmt = sSqlstmt + sSearchtext + " and ( ActionBy = " + currentuser + " || ActionBy = '') ";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "  order by Status";
                DataSet dsActionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsActionList.Tables.Count > 0 && dsActionList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsActionList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objTrackingModels = new ActionTrackingRegisterModels
                            {
                                id_action = Convert.ToInt16(dsActionList.Tables[0].Rows[i]["id_action"].ToString()),
                                Action_name = dsActionList.Tables[0].Rows[i]["Action_name"].ToString(),
                                ActionBy = objGlobaldata.GetEmpHrNameById(dsActionList.Tables[0].Rows[i]["ActionBy"].ToString()),
                                Status = objGlobaldata.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["Status"].ToString()),
                                Remarks = dsActionList.Tables[0].Rows[i]["Remarks"].ToString(),
                                NotificationPeriod = dsActionList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsActionList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                source_id = objGlobaldata.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["source_id"].ToString()),
                                source = objGlobaldata.GetATSSourceFullFormById(dsActionList.Tables[0].Rows[i]["source_id"].ToString()),
                                report_no = dsActionList.Tables[0].Rows[i]["report_no"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Target_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Target_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Due_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Completion_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Completion_date = dtValue;
                            }
                            objTrackList.lstTrack.Add(objTrackingModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrackList.lstTrack.ToList());
        }

        [AllowAnonymous]
        public JsonResult ActionTrackingRegisterListSearch(int? page, string chkAll, string source_id, string Status, string command, string branch_name)
        {
            ViewBag.SubMenutype = "ActionTrackingRegister";

            string currentuser = "";
            ViewBag.Source = objGlobaldata.GetATSSourceFullFormList();
            ViewBag.Status = objGlobaldata.GetDropdownList("ActionTrackingRegister Status");

            ActionTrackingRegisterModelsList objTrackList = new ActionTrackingRegisterModelsList();
            objTrackList.lstTrack = new List<ActionTrackingRegisterModels>();

            ActionTrackingRegisterModels objTrackingModels = new ActionTrackingRegisterModels();

            try
            {
                currentuser = objGlobaldata.GetCurrentUserSession().empid;
                string role = objGlobaldata.GetCurrentUserSession().role;
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                //employeeid = objGlobaldata.GetEmployeeCompEmpIdByEmpId(currentuser);

                string sSqlstmt = "select id_action,Action_name,Target_date,Due_date,Completion_date,ActionBy,Status,Remarks,NotificationDays,NotificationPeriod,NotificationValue,source_id,report_no"
                + " from t_action_tracking_register where Active=1 ";

                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (source_id != "" && source_id != null)
                    {
                        ViewBag.Sourceval = objGlobaldata.GetATSSourceFullFormById(source_id);
                        sSearchtext = sSearchtext + " and source_id ='" + source_id + "'";
                    }
                    if (Status != "" && Status != null)
                    {
                        ViewBag.Statusval = objGlobaldata.GetDropdownitemById(Status);
                        sSearchtext = sSearchtext + " and Status ='" + Status + "'";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                if (objGlobaldata.GetMultiRolesNameById(role).Contains("Admin"))
                {
                    sSqlstmt = sSqlstmt + sSearchtext + "  order by Status";
                }
                else
                {
                    sSqlstmt = sSqlstmt + sSearchtext + " and ActionBy = " + currentuser + "  order by Status";
                }

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsActionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsActionList.Tables.Count > 0 && dsActionList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsActionList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objTrackingModels = new ActionTrackingRegisterModels
                            {
                                id_action = Convert.ToInt16(dsActionList.Tables[0].Rows[i]["id_action"].ToString()),
                                Action_name = dsActionList.Tables[0].Rows[i]["Action_name"].ToString(),
                                ActionBy = objGlobaldata.GetEmpHrNameById(dsActionList.Tables[0].Rows[i]["ActionBy"].ToString()),
                                Status = objGlobaldata.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["Status"].ToString()),
                                Remarks = dsActionList.Tables[0].Rows[i]["Remarks"].ToString(),
                                NotificationPeriod = dsActionList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsActionList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                source_id = objGlobaldata.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["source_id"].ToString()),
                                source = objGlobaldata.GetATSSourceFullFormById(dsActionList.Tables[0].Rows[i]["source_id"].ToString()),
                                report_no = dsActionList.Tables[0].Rows[i]["report_no"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Target_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Target_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Due_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Completion_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Completion_date = dtValue;
                            }
                            objTrackList.lstTrack.Add(objTrackingModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult ActionTrackingRegisterInfo(int id)
        {
            ViewBag.SubMenutype = "ActionTrackingRegister";

            ActionTrackingRegisterModels objTrackingModels = new ActionTrackingRegisterModels();
            try
            {
                string sSqlstmt = "select id_action,Action_name,Target_date,Due_date,Completion_date,ActionBy,Status,Remarks,NotificationDays,NotificationPeriod,NotificationValue,source_id,report_no"
                + " from t_action_tracking_register where Active=1 and id_action='" + id + "'";
                DataSet dsActionList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsActionList.Tables.Count > 0 && dsActionList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsActionList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objTrackingModels = new ActionTrackingRegisterModels
                            {
                                id_action = Convert.ToInt16(dsActionList.Tables[0].Rows[i]["id_action"].ToString()),
                                Action_name = dsActionList.Tables[0].Rows[i]["Action_name"].ToString(),
                                ActionBy = objGlobaldata.GetEmpHrNameById(dsActionList.Tables[0].Rows[i]["ActionBy"].ToString()),
                                Status = objGlobaldata.GetDropdownitemById(dsActionList.Tables[0].Rows[i]["Status"].ToString()),
                                Remarks = dsActionList.Tables[0].Rows[i]["Remarks"].ToString(),
                                NotificationPeriod = dsActionList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsActionList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                source_id = objGlobaldata.GetATSSourceFullFormById(dsActionList.Tables[0].Rows[i]["source_id"].ToString()),
                                report_no = dsActionList.Tables[0].Rows[i]["report_no"].ToString(),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Target_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Target_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Due_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Due_date = dtValue;
                            }
                            if (DateTime.TryParse(dsActionList.Tables[0].Rows[i]["Completion_date"].ToString(), out dtValue))
                            {
                                objTrackingModels.Completion_date = dtValue;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objTrackingModels);
        }

        [AllowAnonymous]
        public ActionResult ActionTrackingRegisterEdit()
        {
            ViewBag.SubMenutype = "ActionTrackingRegister";

            ActionTrackingRegisterModels objTrackModels = new ActionTrackingRegisterModels();
            try
            {
                ViewBag.Status = objGlobaldata.GetDropdownList("ActionTrackingRegister Status");
                ViewBag.Employee = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Source = objGlobaldata.GetATSSourceFullFormList();

                if (Request.QueryString["id_action"] != null && Request.QueryString["id_action"] != "")
                {
                    string sid_action = Request.QueryString["id_action"];

                    string sSqlstmt = "select id_action,Action_name,Target_date,Due_date,Completion_date,ActionBy,Status,Remarks,NotificationDays,"
                        + "NotificationPeriod,NotificationValue,source_id,report_no from t_action_tracking_register where id_action='" + sid_action + "'";

                    DataSet dsTrackingList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsTrackingList.Tables.Count > 0 && dsTrackingList.Tables[0].Rows.Count > 0)
                    {
                        objTrackModels = new ActionTrackingRegisterModels
                        {
                            id_action = Convert.ToInt16(dsTrackingList.Tables[0].Rows[0]["id_action"].ToString()),
                            Action_name = dsTrackingList.Tables[0].Rows[0]["Action_name"].ToString(),
                            ActionBy = objGlobaldata.GetEmpHrNameById(dsTrackingList.Tables[0].Rows[0]["ActionBy"].ToString()),
                            Status = objGlobaldata.GetDropdownitemById(dsTrackingList.Tables[0].Rows[0]["Status"].ToString()),
                            Remarks = dsTrackingList.Tables[0].Rows[0]["Remarks"].ToString(),
                            NotificationPeriod = dsTrackingList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsTrackingList.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            source_id = objGlobaldata.GetATSSourceFullFormById(dsTrackingList.Tables[0].Rows[0]["source_id"].ToString()),
                            report_no = dsTrackingList.Tables[0].Rows[0]["report_no"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsTrackingList.Tables[0].Rows[0]["Target_date"].ToString(), out dtValue))
                        {
                            objTrackModels.Target_date = dtValue;
                        }
                        if (DateTime.TryParse(dsTrackingList.Tables[0].Rows[0]["Due_date"].ToString(), out dtValue))
                        {
                            objTrackModels.Due_date = dtValue;
                        }
                        if (DateTime.TryParse(dsTrackingList.Tables[0].Rows[0]["Completion_date"].ToString(), out dtValue))
                        {
                            objTrackModels.Completion_date = dtValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "ActionID cannot be Null or empty";
                        return RedirectToAction("ActionTrackingRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ActionID cannot be Null or empty";
                    return RedirectToAction("ActionTrackingRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ActionTrackingRegisterList");
            }
            return View(objTrackModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ActionTrackingRegisterEdit(ActionTrackingRegisterModels objTrack, FormCollection form)
        {
            try
            {
                objTrack.ActionBy = form["ActionBy"];
                objTrack.Status = form["Status"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Target_date"], out dateValue) == true)
                {
                    objTrack.Target_date = dateValue;
                }
                if (DateTime.TryParse(form["Due_date"], out dateValue) == true)
                {
                    objTrack.Due_date = dateValue;
                }
                if (DateTime.TryParse(form["Completion_date"], out dateValue) == true)
                {
                    objTrack.Completion_date = dateValue;
                }
                int Notificationval = 1;

                if (objTrack.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objTrack.NotificationValue = "0";
                }
                else if (objTrack.NotificationPeriod == "Weeks" && int.TryParse(objTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objTrack.NotificationPeriod == "Months" && int.TryParse(objTrack.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objTrack.NotificationDays = Notificationval;
                if (objTrack.FunUpdateActionTrackingRegister(objTrack))
                {
                    TempData["Successdata"] = "Action Tracking Register updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ActionTrackingRegisterList");
            }

            return RedirectToAction("ActionTrackingRegisterList");
        }

        [AllowAnonymous]
        public JsonResult ActionTrackingRegisterDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ActionTrackingRegister";

                if (form["id_action"] != null && form["id_action"] != "")
                {
                    ActionTrackingRegisterModels Doc = new ActionTrackingRegisterModels();
                    string sid_action = form["id_action"];

                    if (Doc.FunDeleteActionTrackingRegister(sid_action))
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
                    TempData["alertdata"] = "Action Tracking Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionTrackingRegisterDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}