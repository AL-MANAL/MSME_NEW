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
    public class LeaveMgmtController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public LeaveMgmtController()
        {
            ViewBag.Menutype = "LeaveMgmt";
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult AddLeaveCredit()
        {
            ViewBag.SubMenutype = "LeaveCredit";
            try
            {
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLeaveCredit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddLeaveCredit(LeaveMgmtModels objLeave, FormCollection form)
        {
            ViewBag.SubMenutype = "LeaveCredit";
            try
            {
                if (objLeave.FunAddLeaveCredit(objLeave))
                {
                    TempData["Successdata"] = "Added Leave Credit successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLeaveCredit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LeaveCreditList");
        }

        [AllowAnonymous]
        public ActionResult LeaveCreditList(FormCollection form, int? page)
        {
            ViewBag.SubMenutype = "LeaveCredit";
            LeaveMgmtModelsList lstparties = new LeaveMgmtModelsList();
            lstparties.lstleave = new List<LeaveMgmtModels>();

            try
            {
                string sSqlstmt = "select id_leave_credit,syear,annual_leave,sick_leave,other_leave,carry_leave,leave_gen,logged_date from t_leave_credit where active=1 order by syear desc";
                DataSet dsLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                {

                   

                    for (int i = 0; i < dsLeaveList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LeaveMgmtModels objLeave = new LeaveMgmtModels
                            {
                                id_leave_credit = dsLeaveList.Tables[0].Rows[i]["id_leave_credit"].ToString(),
                                syear = objGlobaldata.GetDropdownitemById(dsLeaveList.Tables[0].Rows[i]["syear"].ToString()),
                                annual_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[i]["annual_leave"].ToString()),
                                sick_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[i]["sick_leave"].ToString()),
                                other_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[i]["other_leave"].ToString()),
                                carry_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[i]["carry_leave"].ToString()),
                                leave_gen = (dsLeaveList.Tables[0].Rows[i]["leave_gen"].ToString()),
                            };
                            DateTime dateValuemaster;

                            if (DateTime.TryParse(dsLeaveList.Tables[0].Rows[i]["logged_date"].ToString(), out dateValuemaster))
                            {
                                objLeave.logged_date = dateValuemaster;
                            }
                            lstparties.lstleave.Add(objLeave);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in LeaveCreditList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveCreditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(lstparties.lstleave.ToList().ToPagedList(page ?? 1, 20000));
        }

        [AllowAnonymous]
        public JsonResult LeaveCreditDelete(FormCollection form)
        {
            try
            {
                  if (form["id_leave_credit"] != null && form["id_leave_credit"] != "")
                    {

                        LeaveMgmtModels Doc = new LeaveMgmtModels();
                        string sid_leave_credit = form["id_leave_credit"];


                        if (Doc.FunDeleteLeaveCredit(sid_leave_credit))
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
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return Json("Failed");
                    }
                

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveCreditDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult doesYearExists(int syear)
        {
            LeaveMgmtModels obj = new LeaveMgmtModels();

            return Json(obj.FunCheckYearExists(syear));

        }

        [AllowAnonymous]
        public ActionResult LeaveCreditEdit()
        {
            ViewBag.SubMenutype = "LeaveCredit";

            LeaveMgmtModels objLeave = new LeaveMgmtModels();

            try
            {
                ViewBag.Year = objGlobaldata.GetDropdownList("Years");
                if (Request.QueryString["id_leave_credit"] != null && Request.QueryString["id_leave_credit"] != "")
                {
                    string id_leave_credit = Request.QueryString["id_leave_credit"];
                    string sSqlstmt = "select id_leave_credit,syear,annual_leave,sick_leave,other_leave,carry_leave from t_leave_credit where id_leave_credit='" + id_leave_credit + "'";

                    DataSet dsLeaveList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                    {

                        objLeave = new LeaveMgmtModels
                        {
                            id_leave_credit = dsLeaveList.Tables[0].Rows[0]["id_leave_credit"].ToString(),
                            syear = objGlobaldata.GetDropdownitemById(dsLeaveList.Tables[0].Rows[0]["syear"].ToString()),
                        };
                        if (Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["annual_leave"].ToString()) > 0)
                        {
                            objLeave.annual_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["annual_leave"].ToString());
                        }
                        if (Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["sick_leave"].ToString()) > 0)
                        {
                            objLeave.sick_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["sick_leave"].ToString());
                        }
                        if (Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["other_leave"].ToString()) > 0)
                        {
                            objLeave.other_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["other_leave"].ToString());
                        }
                        if (Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["carry_leave"].ToString()) > 0)
                        {
                            objLeave.carry_leave = Convert.ToDecimal(dsLeaveList.Tables[0].Rows[0]["carry_leave"].ToString());
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("LeaveCreditList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("LeaveCreditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveCreditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("LeaveCreditList");
            }
            return View(objLeave);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LeaveCreditEdit(LeaveMgmtModels objLeave, FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "LeaveCredit";
                if (objLeave.FunUpdateLeaveCredit(objLeave))
                {
                    TempData["Successdata"] = "Updated Leave Credit successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveCreditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LeaveCreditList");
        }

        public ActionResult AnnualLeave()
        {
            try
            {
                if (Request.QueryString["syear"] != null && Request.QueryString["syear"] != "")
                {
                    string syear = Request.QueryString["syear"].ToString();
                    DataSet dsData = objGlobaldata.generateAnnualLeave(syear);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateLeave: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("MasterLeaveList");
        }

        public ActionResult AddLeaveSheet()
        {
            ViewBag.SubMenutype = "LeaveSheet";
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            try
            {
                ViewBag.leave_type = objGlobaldata.getLeaveTypeList();
                ViewBag.EmpList = objLeave.GetEmpListbox();
                ViewBag.ApprList = objGlobaldata.GetApprover();
                ViewBag.Type = objGlobaldata.GetConstantValue("LeaveType");
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
        public ActionResult AddLeaveSheet(FormCollection form, LeaveMgmtModels objLeave)
        {
            ViewBag.SubMenutype = "LeaveSheet";
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
            return RedirectToAction("MasterLeaveList");
        }


        [AllowAnonymous]
        public ActionResult LeaveApproveReject(string leave_id, decimal duration, int iStatus, string PendingFlg, string Leavecomments, string emp_no, string leave_type)
        {
            try
            {
                LeaveMgmtModels objLeaveModels = new LeaveMgmtModels();

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
                if (objLeaveModels.FunLeaveApproveOrReject(leave_id, duration, iStatus, Leavecomments, emp_no, leave_type))
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

        public ActionResult MasterLeaveList(int? page, string SearchText)
        {

            ViewBag.SubMenutype = "MasterLeave";
            LeaveMgmtModelsList objMasterLeaveModelsList = new LeaveMgmtModelsList();
            objMasterLeaveModelsList.lstleave = new List<LeaveMgmtModels>();


            try
            {
                string role = objGlobaldata.GetCurrentUserSession().role;
                string sSqlstmt = "select id_leavemaster,emp_no,syear,annual_leave,sick_leave,other_leave,carry_leave,bal_annual_leave,bal_sick_leave,bal_other_leave from t_leavemaster where active=1";
                string sSearchtext = "";
                if (!(objGlobaldata.GetMultiRolesNameById(role).Contains("Admin")))
                {
                    sSearchtext = sSearchtext + " and emp_no ='" + objGlobaldata.GetCurrentUserSession().empid + "'";
                }

                sSqlstmt = sSqlstmt + sSearchtext + "  order by emp_no";
                DataSet dsMasterLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsMasterLeaveList.Tables.Count > 0 && dsMasterLeaveList.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsMasterLeaveList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            LeaveMgmtModels objModel = new LeaveMgmtModels
                            {
                                id_leavemaster = (dsMasterLeaveList.Tables[0].Rows[i]["id_leavemaster"].ToString()),
                                emp_no = (dsMasterLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                comp_id = objGlobaldata.GetEmpIDByEmpNo(dsMasterLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                emp_firstname = objGlobaldata.GetEmpHrNameById(dsMasterLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                syear = dsMasterLeaveList.Tables[0].Rows[i]["syear"].ToString(),
                                bal_annual_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["bal_annual_leave"].ToString()),
                                bal_sick_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["bal_sick_leave"].ToString()),
                                bal_other_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["bal_other_leave"].ToString()),
                                annual_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["annual_leave"].ToString()),
                                sick_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["sick_leave"].ToString()),
                                other_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["other_leave"].ToString()),
                                carry_leave = Convert.ToDecimal(dsMasterLeaveList.Tables[0].Rows[i]["carry_leave"].ToString()),
                            };

                            LeaveMgmtModelsList objTransList = new LeaveMgmtModelsList();
                            objTransList.lstleave = new List<LeaveMgmtModels>();


                            string sql = "select leave_id,emp_no,fr_date,t.syear, to_date,duration,reason_leave,leave_type,approver,applied_date,bal_type,"
                   + "(case when approved_status='1' then 'Leave Approved' when approved_status = '0' then 'Yet To Be Reviewed'"
                   + "else 'Rejected' end) as approved_status,approved_Date,t.logged_by,leave_gen from t_leavetrans t,t_leave_credit tt where t.syear=tt.syear and t.Active=1 and emp_no='" + objModel.emp_no + "' order by syear desc";
                            DataSet dsTrans = objGlobaldata.Getdetails(sql);
                            if (dsTrans.Tables.Count > 0 && dsTrans.Tables[0].Rows.Count > 0)
                            {
                               
                                for (int j = 0; j < dsTrans.Tables[0].Rows.Count; j++)
                                {
                                    try
                                    {
                                        LeaveMgmtModels objLeaveModel = new LeaveMgmtModels
                                        {
                                            leave_id = Convert.ToInt16(dsTrans.Tables[0].Rows[j]["leave_id"].ToString()),
                                            emp_no = (dsTrans.Tables[0].Rows[j]["emp_no"].ToString()),
                                            syear = (dsTrans.Tables[0].Rows[j]["syear"].ToString()),
                                            leave_type = (dsTrans.Tables[0].Rows[j]["leave_type"].ToString()),
                                            duration = Convert.ToDecimal(dsTrans.Tables[0].Rows[j]["duration"].ToString()),
                                            approved_status = dsTrans.Tables[0].Rows[j]["approved_status"].ToString(),
                                            approver = objGlobaldata.GetEmpHrNameById(dsTrans.Tables[0].Rows[j]["approver"].ToString()),
                                            logged_by = objGlobaldata.GetEmpHrNameById(dsTrans.Tables[0].Rows[j]["logged_by"].ToString()),
                                            reason_leave = dsTrans.Tables[0].Rows[j]["reason_leave"].ToString(),
                                            leave_gen = dsTrans.Tables[0].Rows[j]["leave_gen"].ToString(),
                                        };
                                        DateTime dateValuemaster;

                                        if (DateTime.TryParse(dsTrans.Tables[0].Rows[j]["fr_date"].ToString(), out dateValuemaster))
                                        {
                                            objLeaveModel.fr_date = dateValuemaster;
                                        }
                                        if (DateTime.TryParse(dsTrans.Tables[0].Rows[j]["to_date"].ToString(), out dateValuemaster))
                                        {
                                            objLeaveModel.to_date = dateValuemaster;

                                        }
                                        if (DateTime.TryParse(dsTrans.Tables[0].Rows[j]["applied_date"].ToString(), out dateValuemaster))
                                        {
                                            objLeaveModel.applied_date = dateValuemaster;
                                        }
                                        if (DateTime.TryParse(dsTrans.Tables[0].Rows[j]["approved_Date"].ToString(), out dateValuemaster))
                                        {
                                            objLeaveModel.approved_Date = dateValuemaster;
                                        }
                                        objTransList.lstleave.Add(objLeaveModel);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in MasterLeaveList: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                            }
                            objModel.leaveTransList = objTransList.lstleave;

                            objMasterLeaveModelsList.lstleave.Add(objModel);
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
            return View(objMasterLeaveModelsList.lstleave.ToList().ToPagedList(page ?? 1, 40));

        }

        public JsonResult FunPreviousDate(string emp_no, string dt1)
        {

            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            DateTime dt = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);
            bool cal = false;
            cal = objLeave.FuncCheckPreviousDate(emp_no, dt);
            return Json(cal);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LeaveAdjustment()
        {
            ViewBag.SubMenutype = "LeaveAdjustment";
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            try
            {
                ViewBag.EmpList = objLeave.GetEmpListbox();
                ViewBag.Adjust = objGlobaldata.getLeaveTypeList();
                ViewBag.Operation = objGlobaldata.getLeaveOperationList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveAdjustment: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult LeaveAdjustment(LeaveMgmtModels objLeave, FormCollection form)
        {
            ViewBag.SubMenutype = "LeaveAdjustment";
            try
            {
                if (objLeave.FunUpdateLeaveAdjust(objLeave))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveAdjustment: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LeaveAdjustmentList");
        }

        public JsonResult FungetEmpBalDetails(string emp_no)
        {
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            if (emp_no != "")
            {
                string sql = "select emp_no,syear,bal_annual_leave,bal_sick_leave,bal_other_leave from t_leavemaster where emp_no='" + emp_no + "'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    objLeave = new LeaveMgmtModels()
                    {
                        emp_no = objGlobaldata.GetEmpIDByEmpNo(dsData.Tables[0].Rows[0]["emp_no"].ToString()),
                        syear = dsData.Tables[0].Rows[0]["syear"].ToString(),
                        bal_annual_leave = Convert.ToDecimal(dsData.Tables[0].Rows[0]["bal_annual_leave"].ToString()),
                        bal_sick_leave = Convert.ToDecimal(dsData.Tables[0].Rows[0]["bal_sick_leave"].ToString()),
                        bal_other_leave = Convert.ToDecimal(dsData.Tables[0].Rows[0]["bal_other_leave"].ToString()),

                    };


                }
            }
            return Json(objLeave);
        }

        public JsonResult GetBalanceByEmpID(string emp_no, string leave_type)
        {
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            if (emp_no != "" && leave_type != "")
            {

                DataSet dsBal = new DataSet();
                switch (leave_type)
                {
                    case "Annual Leave":
                        dsBal = objGlobaldata.Getdetails("select bal_annual_leave as balance,syear from t_leavemaster "
                         + " where emp_no='" + emp_no + "' and active=1");
                        if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                        {
                            objLeave = new LeaveMgmtModels()
                            {
                                balance = Convert.ToDecimal(dsBal.Tables[0].Rows[0]["balance"].ToString()),
                                syear = dsBal.Tables[0].Rows[0]["syear"].ToString()
                            };
                        }
                        break;


                    case "Sick Leave":
                        dsBal = objGlobaldata.Getdetails("select bal_sick_leave as balance,syear from t_leavemaster "
                        + " where emp_no='" + emp_no + "' and active=1");
                        if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                        {
                            objLeave = new LeaveMgmtModels()
                            {
                                balance = Convert.ToDecimal(dsBal.Tables[0].Rows[0]["balance"].ToString()),
                                syear = dsBal.Tables[0].Rows[0]["syear"].ToString()
                            };
                        }
                        break;


                    case "Other Leave":
                        dsBal = objGlobaldata.Getdetails("select bal_other_leave as balance,syear from t_leavemaster "
                        + " where emp_no='" + emp_no + "' and active=1");
                        if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                        {
                            objLeave = new LeaveMgmtModels()
                            {
                                balance = Convert.ToDecimal(dsBal.Tables[0].Rows[0]["balance"].ToString()),
                                syear = dsBal.Tables[0].Rows[0]["syear"].ToString()
                            };
                        }
                        break;

                    default: break;
                }

            }
            return Json(objLeave);
        }

        [AllowAnonymous]
        public ActionResult LeaveAdjustmentList(FormCollection form, int? page)
        {
            ViewBag.SubMenutype = "LeaveAdjustment";
            LeaveMgmtModelsList lstparties = new LeaveMgmtModelsList();
            lstparties.lstleave = new List<LeaveMgmtModels>();

            try
            {
                string sSqlstmt = "select id_adjustment,emp_no,syear,adj_type,days,op_type from t_leaveadjustment  where syear=year(current_date()) and active=1";
                DataSet dsLeaveList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                {

                    

                    for (int i = 0; i < dsLeaveList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LeaveMgmtModels objLeave = new LeaveMgmtModels
                            {
                                id_adjustment = dsLeaveList.Tables[0].Rows[i]["id_adjustment"].ToString(),
                                emp_firstname = objGlobaldata.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                emp_no = objGlobaldata.GetEmpIDByEmpNo(dsLeaveList.Tables[0].Rows[i]["emp_no"].ToString()),
                                syear = (dsLeaveList.Tables[0].Rows[i]["syear"].ToString()),
                                adj_type = (dsLeaveList.Tables[0].Rows[i]["adj_type"].ToString()),
                                days = Convert.ToInt32(dsLeaveList.Tables[0].Rows[i]["days"].ToString()),
                                op_type = (dsLeaveList.Tables[0].Rows[i]["op_type"].ToString()),
                            };
                            lstparties.lstleave.Add(objLeave);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in LeaveAdjustmentList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LeaveAdjustmentList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(lstparties.lstleave.ToList().ToPagedList(page ?? 1, 20000));
        }

        [AllowAnonymous]
        public JsonResult LeaveDelete(FormCollection form)
        {
            try
            {

                
                    if (form["leave_id"] != null && form["leave_id"] != "" && form["emp_no"] != null && form["emp_no"] != "")
                    {

                        LeaveMgmtModels Doc = new LeaveMgmtModels();
                        string sleave_id = form["leave_id"];
                        string emp_no = form["emp_no"];
                        decimal duration = Convert.ToDecimal(form["duration"]);

                        if (Doc.FunDeleteLeave(sleave_id, emp_no, duration))
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

        public JsonResult GetCalculateEmpLeave(string emp_no, string dt1, string dt2)
        {
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            DateTime sfrdate = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);

            DateTime stodate = DateTime.ParseExact(dt2, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);

            int closebal = 0;
            if (emp_no != "")
            {
                closebal = objLeave.Funcalculatetotalleave(emp_no, sfrdate, stodate);
            }
            return Json(closebal);
        }

        public JsonResult GetCalculateHalfDayEmpLeave(string emp_no, string dt1)
        {
            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            DateTime sfrdate = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);


            decimal closebal = 0;
            if (emp_no != "")
            {
                closebal = objLeave.Funcalculatetotalleave(emp_no, sfrdate);
            }
            return Json(closebal);
        }
        public JsonResult FunCheckCalExists(string dt1)
        {

            LeaveMgmtModels objLeave = new LeaveMgmtModels();
            DateTime dt = DateTime.ParseExact(dt1, "ddd MMM dd yyyy HH:mm:ss 'GMT+0400 (Gulf Standard Time)'", CultureInfo.InvariantCulture);
            bool cal = false;
            cal = objLeave.FuncCheckCalender(dt);
            return Json(cal);
        }
    }
}