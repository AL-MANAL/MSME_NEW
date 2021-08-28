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
using Rotativa;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class QHSEPlannerController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public QHSEPlannerController()
        {
            ViewBag.Menutype = "QHSEPlanner";
        }
        public ActionResult AddQHSEPlanner()
        {
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Standards = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQHSEPlanner: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQHSEPlanner(QHSEPlannerModels objPlan, FormCollection form)
        {
            try
            {
                objPlan.Logged_by = objGlobaldata.GetCurrentUserSession().empid;


                QHSEPlannerModelsList lstPlanner = new QHSEPlannerModelsList();
                lstPlanner.lstPlan = new List<QHSEPlannerModels>();
                DateTime dateValue;
                int c = Convert.ToInt16(form["itemcnt"]);
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    QHSEPlannerModels objQPlan = new QHSEPlannerModels();

                    //objQPlan.QHSE_name = form["QHSE_name" + i];
                    objQPlan.QHSE_name = null;
                    objQPlan.Activity_name = form["Activity_name" + i];
                    if (DateTime.TryParse(form["Planned_date" + i], out dateValue) == true)
                    {
                        objQPlan.Planned_date = dateValue;
                    }
                    if (DateTime.TryParse(form["Target_date" + i], out dateValue) == true)
                    {
                        objQPlan.Target_date = dateValue;
                    }
                    objQPlan.Person_responsible = form["Person_responsible" + i];
                    objQPlan.Remarks = form["Remarks" + i];
                    objQPlan.NotificationPeriod = form["NotificationPeriod" + i];
                    objQPlan.NotificationValue = form["NotificationValue" + i];
                    int Notificationval = 1;

                    if (objQPlan.NotificationPeriod == "None")
                    {
                        Notificationval = 0;
                        objQPlan.NotificationValue = "0";
                    }
                    else if (objQPlan.NotificationPeriod == "Weeks" && int.TryParse(objQPlan.NotificationValue, out Notificationval))
                    {
                        Notificationval = 7 * Notificationval;
                    }
                    else if (objQPlan.NotificationPeriod == "Months" && int.TryParse(objQPlan.NotificationValue, out Notificationval))
                    {
                        Notificationval = 30 * Notificationval;
                    }
                    objQPlan.NotificationDays = Notificationval;
                    lstPlanner.lstPlan.Add(objQPlan);
                }

                if (objPlan.FunAddQHSEPlanner(objPlan,lstPlanner))
                {
                    TempData["Successdata"] = "QHSE Planner added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQHSEPlanner: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QHSEPlannerList");
        }
      

        [AllowAnonymous]
        public ActionResult QHSEPlannerList(int? page,string branch_name)
        {
            QHSEPlannerModelsList objPlanList = new QHSEPlannerModelsList();
            objPlanList.lstPlan = new List<QHSEPlannerModels>();

            try
            {
                string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Actual_date,Person_responsible,Status,Remarks"
                + " from t_qhse_planner where Active=1";
               
                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name !="")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Target_date asc";
                DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                {
                   
                                          
                   
                    for (int i = 0; i < dsPlannerList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QHSEPlannerModels objHSEModels = new QHSEPlannerModels
                            {
                                id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[i]["id_qhse"].ToString()),
                                QHSE_name =objGlobaldata.GetIsoStdNameById(dsPlannerList.Tables[0].Rows[i]["QHSE_name"].ToString()),
                                Activity_name = dsPlannerList.Tables[0].Rows[i]["Activity_name"].ToString(),
                                Person_responsible = objGlobaldata.GetMultiHrEmpNameById(dsPlannerList.Tables[0].Rows[i]["Person_responsible"].ToString()),
                                Status = objGlobaldata.GetDropdownitemById(dsPlannerList.Tables[0].Rows[i]["Status"].ToString()),
                                Remarks = dsPlannerList.Tables[0].Rows[i]["Remarks"].ToString(),

                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Planned_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Planned_date = dtValue;
                            }
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Actual_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Actual_date = dtValue;
                            }
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Target_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Target_date = dtValue;
                            }
                            objPlanList.lstPlan.Add(objHSEModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
          
            return View(objPlanList.lstPlan.ToList());
        }

        [AllowAnonymous]
        public JsonResult QHSEPlannerListSearch(int? page, string branch_name)
        {
            QHSEPlannerModelsList objPlanList = new QHSEPlannerModelsList();
            objPlanList.lstPlan = new List<QHSEPlannerModels>();

            try
            {
                string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Actual_date,Person_responsible,Status,Remarks"
                + " from t_qhse_planner where Active=1";

                string sSearchtext = "";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Target_date asc";
                DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                {

                  


                    for (int i = 0; i < dsPlannerList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QHSEPlannerModels objHSEModels = new QHSEPlannerModels
                            {
                                id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[i]["id_qhse"].ToString()),
                                QHSE_name = objGlobaldata.GetIsoStdNameById(dsPlannerList.Tables[0].Rows[i]["QHSE_name"].ToString()),
                                Activity_name = dsPlannerList.Tables[0].Rows[i]["Activity_name"].ToString(),
                                Person_responsible = objGlobaldata.GetMultiHrEmpNameById(dsPlannerList.Tables[0].Rows[i]["Person_responsible"].ToString()),
                                Status = objGlobaldata.GetDropdownitemById(dsPlannerList.Tables[0].Rows[i]["Status"].ToString()),
                                Remarks = dsPlannerList.Tables[0].Rows[i]["Remarks"].ToString(),

                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Planned_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Planned_date = dtValue;
                            }
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Actual_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Actual_date = dtValue;
                            }
                            if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[i]["Target_date"].ToString(), out dtValue))
                            {
                                objHSEModels.Target_date = dtValue;
                            }
                            objPlanList.lstPlan.Add(objHSEModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult QHSEPlannerEdit()
        {
            QHSEPlannerModels objPlanner = new QHSEPlannerModels();
            try
            {
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Standards = objGlobaldata.GetAllIsoStdListbox();
                if (Request.QueryString["id_qhse"] != null && Request.QueryString["id_qhse"] != "")
                {
                    string sid_qhse = Request.QueryString["id_qhse"];

                    string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Person_responsible,Remarks,NotificationDays,NotificationPeriod,NotificationValue"
                    + " from t_qhse_planner where id_qhse='" + sid_qhse + "'";
                    DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                    {

                        objPlanner = new QHSEPlannerModels
                        {
                            id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[0]["id_qhse"].ToString()),
                            QHSE_name = (dsPlannerList.Tables[0].Rows[0]["QHSE_name"].ToString()),
                            Activity_name = dsPlannerList.Tables[0].Rows[0]["Activity_name"].ToString(),
                            Person_responsible = (dsPlannerList.Tables[0].Rows[0]["Person_responsible"].ToString()),
                            Remarks = dsPlannerList.Tables[0].Rows[0]["Remarks"].ToString(),
                            NotificationPeriod = dsPlannerList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsPlannerList.Tables[0].Rows[0]["NotificationValue"].ToString()
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Planned_date"].ToString(), out dtValue))
                        {
                            objPlanner.Planned_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Target_date"].ToString(), out dtValue))
                        {
                            objPlanner.Target_date = dtValue;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "issueId cannot be Null or empty";
                        return RedirectToAction("QHSEPlannerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "PlannerID cannot be Null or empty";
                    return RedirectToAction("QHSEPlannerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QHSEPlannerList");
            }
            return View(objPlanner);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QHSEPlannerEdit(QHSEPlannerModels objPlan, FormCollection form)
        {
            try
            {
                objPlan.QHSE_name = form["QHSE_name"];
                objPlan.Person_responsible = form["Person_responsible"];
               
                DateTime dateValue;
                if (DateTime.TryParse(form["Planned_date"], out dateValue) == true)
                {
                    objPlan.Planned_date = dateValue;
                }
                if (DateTime.TryParse(form["Target_date"], out dateValue) == true)
                {
                    objPlan.Target_date = dateValue;
                }
                int Notificationval = 1;

                if (objPlan.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objPlan.NotificationValue = "0";
                }
                else if (objPlan.NotificationPeriod == "Weeks" && int.TryParse(objPlan.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objPlan.NotificationPeriod == "Months" && int.TryParse(objPlan.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objPlan.NotificationDays = Notificationval;
                if (objPlan.FunUpdateActionTrackingRegister(objPlan))
                {
                    TempData["Successdata"] = "QHSE Planner updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QHSEPlannerList");
            }

            return RedirectToAction("QHSEPlannerList");
        }
        
        [AllowAnonymous]
        public ActionResult QHSEPlannerActiveStatus()
        {
            QHSEPlannerModels objPlanner = new QHSEPlannerModels();
            try
            {
                ViewBag.Status = objGlobaldata.GetDropdownList("QHSEPlanner Status");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Standards = objGlobaldata.GetAllIsoStdListbox();
                if (Request.QueryString["id_qhse"] != null && Request.QueryString["id_qhse"] != "")
                {
                    string sid_qhse = Request.QueryString["id_qhse"];

                    string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Person_responsible,Remarks,Status,Upload,Actual_date"
                    + " from t_qhse_planner where id_qhse='" + sid_qhse + "'";
                    DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                    {

                        objPlanner = new QHSEPlannerModels
                        {
                            id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[0]["id_qhse"].ToString()),
                            QHSE_name = objGlobaldata.GetIsoStdNameById(dsPlannerList.Tables[0].Rows[0]["QHSE_name"].ToString()),
                            Activity_name = dsPlannerList.Tables[0].Rows[0]["Activity_name"].ToString(),
                            Person_responsible = objGlobaldata.GetMultiHrEmpNameById(dsPlannerList.Tables[0].Rows[0]["Person_responsible"].ToString()),
                            Remarks = dsPlannerList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Status =objGlobaldata.GetDropdownitemById(dsPlannerList.Tables[0].Rows[0]["Status"].ToString()),
                            Upload = dsPlannerList.Tables[0].Rows[0]["Upload"].ToString(),

                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Planned_date"].ToString(), out dtValue))
                        {
                            objPlanner.Planned_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Target_date"].ToString(), out dtValue))
                        {
                            objPlanner.Target_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Actual_date"].ToString(), out dtValue))
                        {
                            objPlanner.Actual_date = dtValue;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "issueId cannot be Null or empty";
                        return RedirectToAction("QHSEPlannerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "PlannerID cannot be Null or empty";
                    return RedirectToAction("QHSEPlannerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QHSEPlannerList");
            }
            return View(objPlanner);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QHSEPlannerActiveStatus(QHSEPlannerModels objPlan, FormCollection form, IEnumerable<HttpPostedFileBase> Upload)
        {
            try
            {
                objPlan.Status = form["Status"];
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (DateTime.TryParse(form["Actual_date"], out dateValue) == true)
                {
                    objPlan.Actual_date = dateValue;
                }

                if (Upload != null && files.ContentLength > 0)
                {
                    objPlan.Upload = "";
                    foreach (var file in Upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/QHSE"), Path.GetFileName(file.FileName));
                            string sFilename = "QHSEPlanner" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPlan.Upload = objPlan.Upload + "," + "~/DataUpload/MgmtDocs/QHSE/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerActiveStatus-upload: " + ex.ToString());

                        }
                    }
                    objPlan.Upload = objPlan.Upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
               
                if (objPlan.FunUpdateQHSEPlannerstatus(objPlan))
                {
                    TempData["Successdata"] = "QHSE Planner updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QHSEPlannerList");
            }

            return RedirectToAction("QHSEPlannerList");
        }
        
        [AllowAnonymous]
        public ActionResult QHSEPlannerDetails()
        {
            QHSEPlannerModels objPlanner = new QHSEPlannerModels();

            try
            {
                if (Request.QueryString["id_qhse"] != null && Request.QueryString["id_qhse"] != "")
                {
                    string sid_qhse = Request.QueryString["id_qhse"];

                    //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                    string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Person_responsible,Remarks,Status,Upload,Actual_date"
                   + ",NotificationDays,NotificationPeriod,NotificationValue from t_qhse_planner where id_qhse='" + sid_qhse + "'";
                    DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                    {
                        objPlanner = new QHSEPlannerModels
                        {
                            id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[0]["id_qhse"].ToString()),
                            QHSE_name = objGlobaldata.GetIsoStdNameById(dsPlannerList.Tables[0].Rows[0]["QHSE_name"].ToString()),
                            Activity_name = dsPlannerList.Tables[0].Rows[0]["Activity_name"].ToString(),
                            Person_responsible = objGlobaldata.GetMultiHrEmpNameById(dsPlannerList.Tables[0].Rows[0]["Person_responsible"].ToString()),
                            Remarks = dsPlannerList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Status = objGlobaldata.GetDropdownitemById(dsPlannerList.Tables[0].Rows[0]["Status"].ToString()),
                            Upload = dsPlannerList.Tables[0].Rows[0]["Upload"].ToString(),
                            NotificationPeriod = dsPlannerList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsPlannerList.Tables[0].Rows[0]["NotificationValue"].ToString()
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Planned_date"].ToString(), out dtValue))
                        {
                            objPlanner.Planned_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Target_date"].ToString(), out dtValue))
                        {
                            objPlanner.Target_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Actual_date"].ToString(), out dtValue))
                        {
                            objPlanner.Actual_date = dtValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("QHSEPlannerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("QHSEPlannerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
           
            return View(objPlanner);

        }
        
        [AllowAnonymous]
        public ActionResult QHSEPlannerInfo(int id)
        {
            QHSEPlannerModels objPlanner = new QHSEPlannerModels();

            try
            {
                    string sSqlstmt = "select id_qhse,QHSE_name,Activity_name,Planned_date,Target_date,Person_responsible,Remarks,Status,Upload,Actual_date"
                   + ",NotificationDays,NotificationPeriod,NotificationValue from t_qhse_planner where id_qhse='" + id + "'";
                    DataSet dsPlannerList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPlannerList.Tables.Count > 0 && dsPlannerList.Tables[0].Rows.Count > 0)
                    {
                        objPlanner = new QHSEPlannerModels
                        {
                            id_qhse = Convert.ToInt16(dsPlannerList.Tables[0].Rows[0]["id_qhse"].ToString()),
                            QHSE_name = objGlobaldata.GetIsoStdNameById(dsPlannerList.Tables[0].Rows[0]["QHSE_name"].ToString()),
                            Activity_name = dsPlannerList.Tables[0].Rows[0]["Activity_name"].ToString(),
                            Person_responsible = objGlobaldata.GetMultiHrEmpNameById(dsPlannerList.Tables[0].Rows[0]["Person_responsible"].ToString()),
                            Remarks = dsPlannerList.Tables[0].Rows[0]["Remarks"].ToString(),
                            Status = objGlobaldata.GetDropdownitemById(dsPlannerList.Tables[0].Rows[0]["Status"].ToString()),
                            Upload = dsPlannerList.Tables[0].Rows[0]["Upload"].ToString(),
                            NotificationPeriod = dsPlannerList.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsPlannerList.Tables[0].Rows[0]["NotificationValue"].ToString()
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Planned_date"].ToString(), out dtValue))
                        {
                            objPlanner.Planned_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Target_date"].ToString(), out dtValue))
                        {
                            objPlanner.Target_date = dtValue;
                        }
                        if (DateTime.TryParse(dsPlannerList.Tables[0].Rows[0]["Actual_date"].ToString(), out dtValue))
                        {
                            objPlanner.Actual_date = dtValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("QHSEPlannerList");
                    }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPlanner);

        }
        
        [AllowAnonymous]
        public JsonResult QHSEPlannerDelete(FormCollection form)
        {
            try
            {
                
                   
                        if (form["id_qhse"] != null && form["id_qhse"] != "")
                        {
                            QHSEPlannerModels Doc = new QHSEPlannerModels();
                            string sid_qhse = form["id_qhse"];
                            if (Doc.FunDeleteQHSEPlanner(sid_qhse))
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
                            TempData["alertdata"] = "Planner Id cannot be Null or empty";
                            return Json("Failed");
                        }
                                   
        
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSEPlannerDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetHrEmpListByDept(DeptId);
            return Json(lstEmp);
        }
        public ActionResult EventPlanner()
        {
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                ViewBag.Status = objGlobaldata.GetDropdownList("Calendar Event Status");
                ViewBag.Type = objGlobaldata.GetDropdownList("Calendar Event Type");
                ViewBag.Priority = objGlobaldata.GetDropdownList("Calendar Event Priority");


                ViewBag.Branch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetEvents: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        public JsonResult GetEvents(string branch,string event_status,string event_priority,string event_type)
        {
            QHSEPlannerModelsList objPlanList = new QHSEPlannerModelsList();
            objPlanList.lstPlan = new List<QHSEPlannerModels>();
            QHSEPlannerModels commentModels = new QHSEPlannerModels();
            try
            {
                string sSqlstmt = "select id_event,subject,description,start_date,end_date,Logged_by,full_day,Person_responsible,NotificationPeriod,NotificationValue,event_status,event_type,"
                    +"event_priority from t_event_planner where active=1";

                string sSearchtext = "";
                if (branch != null && branch != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch + "' ";

                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + objGlobaldata.GetCurrentUserSession().division + "' ";
                }
                if (event_status != null && event_status != "")
                {
                    sSearchtext = sSearchtext + " and event_status='" + event_status + "' ";
                    
                }
                if (event_priority != null && event_priority != "")
                {
                    sSearchtext = sSearchtext + " and event_priority='" + event_priority + "' ";

                }
                if (event_type != null && event_type != "")
                {
                    sSearchtext = sSearchtext + " and event_type='" + event_type + "' ";

                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsEventList = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                if (dsEventList.Tables.Count > 0 && dsEventList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEventList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QHSEPlannerModels objModels = new QHSEPlannerModels
                            {

                                id_event = Convert.ToInt32(dsEventList.Tables[0].Rows[i]["id_event"].ToString()),
                                subject = dsEventList.Tables[0].Rows[i]["subject"].ToString(),
                                description = dsEventList.Tables[0].Rows[i]["description"].ToString(),
                                full_day = Convert.ToBoolean(dsEventList.Tables[0].Rows[i]["full_day"].ToString()),
                                Person_responsible = (dsEventList.Tables[0].Rows[i]["Person_responsible"].ToString()),
                                Person_name = objGlobaldata.GetMultiHrEmpNameById(dsEventList.Tables[0].Rows[i]["Person_responsible"].ToString()),
                                NotificationPeriod = dsEventList.Tables[0].Rows[i]["NotificationPeriod"].ToString(),
                                NotificationValue = dsEventList.Tables[0].Rows[i]["NotificationValue"].ToString(),
                                Logged_by = objGlobaldata.GetEmpHrNameById(dsEventList.Tables[0].Rows[i]["Logged_by"].ToString()),
                                event_status = dsEventList.Tables[0].Rows[i]["event_status"].ToString(),
                                event_type = dsEventList.Tables[0].Rows[i]["event_type"].ToString(),
                                event_priority = dsEventList.Tables[0].Rows[i]["event_priority"].ToString(),
                                priority_color = objGlobaldata.GetCalendarEventPriorityColorById(dsEventList.Tables[0].Rows[i]["event_priority"].ToString()),
                                sevent_status =objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_status"].ToString()),
                                sevent_type =objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_type"].ToString()),
                                sevent_priority =objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_priority"].ToString()),

                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsEventList.Tables[0].Rows[i]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                                objModels.starttime= objModels.start_date.ToShortTimeString();
                            }
                            if (dsEventList.Tables[0].Rows[i]["end_date"].ToString() !=null && dsEventList.Tables[0].Rows[i]["end_date"].ToString() != "")
                            {
                                if (DateTime.TryParse(dsEventList.Tables[0].Rows[i]["end_date"].ToString(), out dtValue))
                                {
                                    objModels.end_date = dtValue;
                                    objModels.endtime = objModels.end_date.ToShortTimeString();
                                }
                            }
                            QHSEPlannerModelsList objCommentList = new QHSEPlannerModelsList();
                            objCommentList.lstPlan = new List<QHSEPlannerModels>();

                            string sql1 = "select id_comments,comments,logged_by,logged_date from t_event_planner_comments where id_event='" + dsEventList.Tables[0].Rows[i]["id_event"].ToString() + "'";
                            DataSet sdsData = objGlobaldata.Getdetails(sql1);
                            if (sdsData.Tables.Count > 0 && sdsData.Tables[0].Rows.Count > 0)
                            {
                                for (int k = 0; k < sdsData.Tables[0].Rows.Count; k++)
                                {
                                    commentModels = new QHSEPlannerModels
                                    {
                                        id_comments = sdsData.Tables[0].Rows[k]["id_comments"].ToString(),
                                        comments = sdsData.Tables[0].Rows[k]["comments"].ToString(),
                                        Logged_by =objGlobaldata.GetEmpHrNameById(sdsData.Tables[0].Rows[k]["Logged_by"].ToString()),
                                    };
                                    DateTime dtDocDate;
                                    if (sdsData.Tables[0].Rows[k]["logged_date"].ToString() != ""
                      && DateTime.TryParse(sdsData.Tables[0].Rows[k]["logged_date"].ToString(), out dtDocDate))
                                    {
                                        commentModels.logged_date = dtDocDate;
                                    }

                                    objCommentList.lstPlan.Add(commentModels);
                                }
                            }
                            objModels.commentList = objCommentList.lstPlan;
                            objPlanList.lstPlan.Add(objModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in GetEvents: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetEvents: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return new JsonResult { Data = objPlanList.lstPlan.ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult DeleteEvent(string  eventID)
        {
            QHSEPlannerModels objModel = new QHSEPlannerModels();
            var status = false;
            try
            {
                status = objModel.FunDeleteCalenderEvent(eventID);

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DeleteEvent: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return new JsonResult { Data = new { status = status } };
        }
        [HttpPost]
        public JsonResult SaveEvent(QHSEPlannerModels e,FormCollection form)
        {
            QHSEPlannerModels objModel = new QHSEPlannerModels();
            var status = false;
            try
            {
                DateTime dateValue;
                if (form["start_date"] != null && DateTime.TryParse(form["start_date"], out dateValue) == true)
                {
                    e.start_date = dateValue;
                   
                    if (form["starttime"] != null)
                    {
                        e.start_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + e.starttime + ":00");

                    }
                }
                if (form["end_date"] != null && DateTime.TryParse(form["end_date"], out dateValue) == true)
                {
                    e.end_date = dateValue;

                    if (form["endtime"] != null)
                    {
                        e.end_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + e.endtime + ":00");
                    }
                }
                e.Person_responsible = form["Person_responsible[]"];
                int Notificationval = 1;

                if (e.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    e.NotificationValue = "0";
                    e.NotificationDays = Notificationval;
                }
                else if (e.NotificationPeriod == "Weeks" && int.TryParse(e.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                    e.NotificationDays = Notificationval;
                }
                else if (e.NotificationPeriod == "Months" && int.TryParse(e.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                    e.NotificationDays = Notificationval;
                }
                else if (e.NotificationPeriod == "Days" && int.TryParse(e.NotificationValue, out Notificationval))
                {
                    e.NotificationDays = Notificationval;
                }
               
                if (e.id_event > 0)
                {
                    status = objModel.FunUpdateEvent(e);

                }
                else
                {
                    status = objModel.FunSaveEvent(e);
                }
              
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SaveEvent: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return new JsonResult { Data = new { status = status } };
        }
    }
}