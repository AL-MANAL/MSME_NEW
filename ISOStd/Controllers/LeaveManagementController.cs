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
using Microsoft.Reporting.WebForms;
using System.Web.Mvc.Html;
using System.Globalization;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class LeaveManagementController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        static List<string> objLeaveList = new List<string>();

        public LeaveManagementController()
        {
            ViewBag.Menutype = "LeaveManagement";
        }

      
        public ActionResult MasterLeaveList(int? page, string SearchText)
        {

            ViewBag.SubMenutype = "MasterLeave";
            LeaveModelsList objMasterLeaveModelsList = new LeaveModelsList();
            objMasterLeaveModelsList.LeaveMList = new List<LeaveManagementModels>();
            try
            {
                DataSet dsReport = objGlobaldata.GetLeaveBalCalReport();
                string sSqlstmt = "select master_id,emp_no,emp_firstname,Date_of_join,joined_days,joined_years,accrued,balance"
                + " from t_leave_master ";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "  where (emp_no ='" + SearchText + "' or emp_no like '" + SearchText + "%' or emp_no like '%" + SearchText + "%' or emp_firstname='" + SearchText
                        + "' or emp_firstname like '" + SearchText + "%' or emp_firstname like '%" + SearchText + "%')";
                }

                sSqlstmt = sSqlstmt + sSearchtext+ " order by emp_firstname";

                DataSet dsMasterLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMasterLeaveList.Tables.Count > 0 && dsMasterLeaveList.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsMasterLeaveList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            LeaveManagementModels objLeaveManagementModels = new LeaveManagementModels
                            {
                                master_id = Convert.ToInt16(dsMasterLeaveList.Tables[0].Rows[i]["master_id"].ToString()),
                                emp_firstname = dsMasterLeaveList.Tables[0].Rows[i]["emp_firstname"].ToString(),
                                emp_no = dsMasterLeaveList.Tables[0].Rows[i]["emp_no"].ToString(),
                                joined_days = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["joined_days"].ToString()),
                                joined_years = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["joined_years"].ToString()),
                                accrued = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["accrued"].ToString()),
                                balance = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["balance"].ToString()),
                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsMasterLeaveList.Tables[0].Rows[i]["Date_of_join"].ToString() != ""
                                && DateTime.TryParse(dsMasterLeaveList.Tables[0].Rows[i]["Date_of_join"].ToString(), out dtDocDate))
                            {
                                objLeaveManagementModels.Date_of_join = dtDocDate;
                            }
                            objMasterLeaveModelsList.LeaveMList.Add(objLeaveManagementModels);
                        }

                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in MasterLeaveList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in MasterLeaveList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objMasterLeaveModelsList.LeaveMList.ToList().ToPagedList(page ?? 1, 40));
            



        }
         
        public ActionResult AddLeaveSheet()
        {
            LeaveManagementModels objLeave = new LeaveManagementModels();
            ViewBag.SubMenutype = "AddLeaveSheet";
            try
            {
                ViewBag.leave_type = objGlobaldata.GetDropdownList("Leave Type");
                ViewBag.EmpList = objLeave.GetEmpListbox();
                ViewBag.ApprList = objGlobaldata.GetApprover();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLeaveMaster: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
         
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddLeaveSheet(FormCollection form, LeaveManagementModels objLeave)
        {
            ViewBag.SubMenutype = "AddLeaveSheet";
            try
            {
                if (objLeave.FunAddLeave(objLeave))
                {
                    TempData["Successdata"] = "Applied Leave successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLeaveSheet: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AddLeaveSheetList");
        }
         
        public ActionResult AddLeaveSheetList(string SearchText, int? page)
        {
            ViewBag.SubMenutype = "AddLeaveSheet";
            LeaveModelsList objLeaveModelsList = new LeaveModelsList();
            objLeaveModelsList.LeaveMList = new List<LeaveManagementModels>();
            LeaveManagementModels objLeave = new LeaveManagementModels();
            try
            {
                string role = "", user = "";
               
                    role = objGlobaldata.GetCurrentUserSession().role;
                            
                     user = objGlobaldata.GetCurrentUserSession().empid;
               
                string srole = objGlobaldata.GetMultiRolesNameById(role);
                string sSqlstmt = "select leave_id,emp_no,fr_date, to_date,duration,reason_leave,leave_type,approver,applied_date,bal_type,"
                    + "(case when approved_status='1' then 'Leave Approved' when approved_status = '0' then 'Yet To Be Reviewed'"
                    + "else 'Rejected' end) as approved_status,approved_Date,logged_by from t_leave_trans where Active=1";
                if (!srole.Contains("Admin"))
                {

                    sSqlstmt = sSqlstmt + " and (logged_by='" + user + "')";
                }

                DataSet dsLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                {
                  
                      
                    
                    for (int i = 0; i < dsLeaveList.Tables[0].Rows.Count; i++)
                    {
                        ViewBag.approved_status = objGlobaldata.GetConstantValueKeyValuePair("approved_status");
                        try
                        {
                            LeaveManagementModels objLeaveManagementModels = new LeaveManagementModels
                            {
                                leave_id = Convert.ToInt16(dsLeaveList.Tables[0].Rows[i]["leave_id"].ToString()),
                                emp_no = dsLeaveList.Tables[0].Rows[i]["emp_no"].ToString(),
                                emp_firstname = objLeave.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                leave_type = objGlobaldata.GetDropdownitemById(dsLeaveList.Tables[0].Rows[i]["leave_type"].ToString()),
                                duration = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[i]["duration"].ToString()),
                                approved_status = dsLeaveList.Tables[0].Rows[i]["approved_status"].ToString(),
                                approver = objGlobaldata.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[i]["approver"].ToString()),
                                logged_by = objGlobaldata.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[i]["logged_by"].ToString()),
                                reason_leave = dsLeaveList.Tables[0].Rows[i]["reason_leave"].ToString(),
                                bal_type = dsLeaveList.Tables[0].Rows[i]["bal_type"].ToString(),
                            };
                            DateTime dateValuemaster;

                            if (DateTime.TryParse(dsLeaveList.Tables[0].Rows[i]["fr_date"].ToString(), out dateValuemaster))
                            {
                                objLeaveManagementModels.fr_date = dateValuemaster;
                            }
                            if (DateTime.TryParse(dsLeaveList.Tables[0].Rows[i]["to_date"].ToString(), out dateValuemaster))
                            {
                                objLeaveManagementModels.to_date = dateValuemaster;

                            }
                            if (DateTime.TryParse(dsLeaveList.Tables[0].Rows[i]["applied_date"].ToString(), out dateValuemaster))
                            {
                                objLeaveManagementModels.applied_date = dateValuemaster;
                            }
                            if (DateTime.TryParse(dsLeaveList.Tables[0].Rows[i]["approved_Date"].ToString(), out dateValuemaster))
                            {
                                objLeaveManagementModels.approved_Date = dateValuemaster;
                            }
                            objLeaveModelsList.LeaveMList.Add(objLeaveManagementModels);

                        }

                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddLeaveSheetList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLeaveSheetList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objLeaveModelsList.LeaveMList.ToList().ToPagedList(page ?? 1, 40));
        }

         
        [AllowAnonymous]
        public ActionResult LeaveApproveReject(string leave_id, decimal duration, int iStatus, string PendingFlg, string Leavecomments)
        {
            try
            {
                LeaveManagementModels objLeaveModels = new LeaveManagementModels();

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
                if (objLeaveModels.FunLeaveApproveOrReject(leave_id, duration, iStatus, Leavecomments))
                {
                    //TempData["Successdata"] = "Training " + sStatus;
                    TempData["Successdata"] = "Leave request" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("ListPendingForApproval");
            }
        }
         
        [AllowAnonymous]
        public JsonResult LeaveDelete(FormCollection form)
        {
            try
            {
             
                   
                        if (form["leave_id"] != null && form["leave_id"] != "")
                        {

                            LeaveManagementModels Doc = new LeaveManagementModels();
                            string sleave_id = form["leave_id"];


                            if (Doc.FunDeleteLeave(sleave_id))
                            {
                                TempData["Successdata"] = "Leave deleted successfully";
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
                            TempData["alertdata"] = "Leave Id cannot be Null or empty";
                            return Json("Failed");
                        }
                    
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
         
        [AllowAnonymous]
        public ActionResult LeaveMgmtInfo(string id)
        {
            ViewBag.SubMenutype = "AddLeaveSheet";
            LeaveManagementModels objLeaveModel = new LeaveManagementModels();
            try
            {


                string sSqlstmt = "select master_id,emp_no,emp_firstname,Date_of_join,joined_days,joined_years,accrued,balance,"
                +"logged_date from t_leave_master where emp_no='"+id+"'";

                DataSet dsMasterLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMasterLeaveList.Tables.Count > 0 && dsMasterLeaveList.Tables[0].Rows.Count > 0)
                {
                    objLeaveModel = new LeaveManagementModels
                    {
                        master_id = Convert.ToInt16(dsMasterLeaveList.Tables[0].Rows[0]["master_id"].ToString()),
                        emp_firstname = dsMasterLeaveList.Tables[0].Rows[0]["emp_firstname"].ToString(),
                        emp_no = dsMasterLeaveList.Tables[0].Rows[0]["emp_no"].ToString(),
                        joined_days = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[0]["joined_days"].ToString()),
                        joined_years = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[0]["joined_years"].ToString()),
                        accrued = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[0]["accrued"].ToString()),
                        balance = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[0]["balance"].ToString()),
                       
                    };
                    DateTime dtDocDate = new DateTime();
                    if (dsMasterLeaveList.Tables[0].Rows[0]["Date_of_join"].ToString() != ""
                        && DateTime.TryParse(dsMasterLeaveList.Tables[0].Rows[0]["Date_of_join"].ToString(), out dtDocDate))
                    {
                        objLeaveModel.Date_of_join = dtDocDate;
                    }

                    sSqlstmt = "select leave_id,emp_no,fr_date,to_date,duration,reason_leave,leave_type,approver,applied_date,bal_type,"
                    + "(case when approved_status='1' then 'Leave Approved' when approved_status = '0' then 'Yet To Be Reviewed'"
                    + " else 'Rejected' end) as approved_status,approved_Date,logged_by,comments from t_leave_trans where emp_no='" + id + "' and Active=1";
                    ViewBag.LeaveDetails = objGlobaldata.Getdetails(sSqlstmt);
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("MasterLeaveList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveMgmtInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objLeaveModel);

        }

         
        public JsonResult GetBalanceByEmpID(string emp_no)
        {
            decimal bal = 0;
            if (emp_no != "")
            {
                bal = objGlobaldata.GetLeaveBalance(emp_no);
                
            }
            return Json(bal);
        }
         
        public JsonResult FunCheckCalExists(string dt1)
        {

            LeaveManagementModels objLeave = new LeaveManagementModels();
            DateTime dt = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);
            bool cal = false;
            cal = objLeave.FuncCheckCalender(dt);
            return Json(cal);
        }
         
        public JsonResult FunPreviousDate(string emp_no,string dt1)
        {

            LeaveManagementModels objLeave = new LeaveManagementModels();
            DateTime dt = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);
            bool cal = false;
            cal = objLeave.FuncCheckPreviousDate(emp_no,dt);
            return Json(cal);
        }
         
        public JsonResult GetCalculateEmpLeave(string emp_no, string dt1, string dt2)
        {
            LeaveManagementModels objLeave = new LeaveManagementModels();
            DateTime sfrdate = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);

            DateTime stodate = DateTime.ParseExact(dt2, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);

            int closebal = 0;
            if (emp_no != "")
            {
                closebal = objLeave.Funcalculatetotalleave(emp_no, sfrdate, stodate);
            }
            return Json(closebal);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult UpdateLeaveBalance()
        {
            LeaveManagementModels objModel = new LeaveManagementModels();
            try
            {

                if (Request.QueryString["emp_no"] != null && Request.QueryString["emp_no"] != "")
                {
                    string emp_no = Request.QueryString["emp_no"];
                    string sSqlstmt = "select emp_no,emp_firstname,balance from t_leave_master where emp_no='" + emp_no + "'";
                 
                    DataSet dsLeaveList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new LeaveManagementModels
                        {
                            emp_no = dsLeaveList.Tables[0].Rows[0]["emp_no"].ToString(),
                            emp_firstname = dsLeaveList.Tables[0].Rows[0]["emp_firstname"].ToString(),
                            balance =Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["balance"].ToString()),
                        };
                     
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("MasterLeaveList");
                    }

                   
                }
                else
                {

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("MasterLeaveList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateLeaveBalance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("MasterLeaveList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateLeaveBalance(FormCollection form, LeaveManagementModels objLeave)
        {
            try
            {
                if (objLeave.FunAddUsedLeave(objLeave))
                {
                    TempData["Successdata"] = "Applied Leave successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in UpdateLeaveBalance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MasterLeaveList");
        }

    }
} 

      