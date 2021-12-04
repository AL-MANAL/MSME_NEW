using ISOStd.Filters;
using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class PPEIssueLogController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public PPEIssueLogController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "PPEIssueLog";
        }

        //
        // GET: /PPEIssueLog/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /PPEIssueLog/AddPPEIssueLog

        [AllowAnonymous]
        public ActionResult AddPPEIssueLog()
        {
            PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels();
            objPPEIssueLog.branch = objGlobaldata.GetCurrentUserSession().division;
            objPPEIssueLog.Department = objGlobaldata.GetCurrentUserSession().DeptID;
            objPPEIssueLog.Work_Location = objGlobaldata.GetCurrentUserSession().Work_Location;

            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Department = objGlobaldata.GetDepartmentListbox(objPPEIssueLog.branch);
            ViewBag.Location = objGlobaldata.GetDivisionLocationList(objPPEIssueLog.branch);
            ViewBag.PPEIssued = objGlobaldata.GetDropdownList("PPE Issued");
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            //ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            ViewBag.IssueBy = objGlobaldata.GetHrEmployeeList();
            return View(objPPEIssueLog);
        }

        // POST: /PPEIssueLog/AddPPEIssueLog

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPPEIssueLog(PPEIssueLogModels objPPEIssueLog, FormCollection form, HttpPostedFileBase PPE_Issue_Voucher)
        {
            try
            {
                objPPEIssueLog.LoggedBy = objGlobaldata.GetCurrentUserSession().empid;
                objPPEIssueLog.branch = form["branch"];
                objPPEIssueLog.Work_Location = form["Work_Location"];
                objPPEIssueLog.Department = form["Department"];
                objPPEIssueLog.PPE_Issued = form["PPE_Issued"];

                DateTime dateValue;

                if (form["Issue_Date"] != null && DateTime.TryParse(form["Issue_Date"], out dateValue) == true)
                {
                    objPPEIssueLog.Issue_Date = dateValue;
                }

                if (form["PPE_Issued_Last_Date"] != null && DateTime.TryParse(form["PPE_Issued_Last_Date"], out dateValue) == true)
                {
                    objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                }

                if (PPE_Issue_Voucher != null && PPE_Issue_Voucher.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(PPE_Issue_Voucher.FileName));
                        string sFilename = "PPE" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        PPE_Issue_Voucher.SaveAs(sFilepath + "/" + sFilename);
                        objPPEIssueLog.PPE_Issue_Voucher = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddPPEIssueLog: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPPEIssueLog.FunAddPPEIssueLog(objPPEIssueLog))
                {
                    TempData["Successdata"] = "Added PPE Issue Log details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPPEIssueLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PPEIssueLogList");
        }

        [AllowAnonymous]
        public JsonResult PPELogDocDelete(FormCollection form)
        {
            try
            {
                if (form["IssueLog_Id"] != null && form["IssueLog_Id"] != "")
                {
                    PPEIssueLogModels Doc = new PPEIssueLogModels();
                    string sIssueLog_Id = form["IssueLog_Id"];

                    if (Doc.FunDeletePPELogDoc(sIssueLog_Id))
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
                    TempData["alertdata"] = "PPE Log Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPELogDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //
        // GET: /PPEIssueLog/PPEIssueLogList

        [AllowAnonymous]
        public ActionResult PPEIssueLogList(string branch_name)
        {
            PPEIssueLogModelsList objPPEIssueLogList = new PPEIssueLogModelsList();
            objPPEIssueLogList.lstPPEIssueLog = new List<PPEIssueLogModels>();

            // ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            ViewBag.PPEIssued = objGlobaldata.GetDropdownList("PPE Issued");

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                    + " PPE_Issue_Voucher, LoggedBy,branch,Department from t_ppe_issuelog where Active=1 ";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Issue_Date desc";

                DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPPELog.Tables.Count > 0)
                {
                    for (int i = 0; i < dsPPELog.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                            {
                                IssueLog_Id = dsPPELog.Tables[0].Rows[i]["IssueLog_Id"].ToString(),
                                Receiver_Name = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Receiver_Name"].ToString()),
                                Position = dsPPELog.Tables[0].Rows[i]["Position"].ToString(),
                                Cust_Project_Name = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["Cust_Project_Name"].ToString()),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsPPELog.Tables[0].Rows[i]["Work_Location"].ToString()),
                                PPE_Issued = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["PPE_Issued"].ToString()),
                                Issued_By = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Issued_By"].ToString()),
                                PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[i]["PPE_Issue_Voucher"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPPELog.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPPELog.Tables[0].Rows[i]["Department"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["Issue_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Issue_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                            }

                            objPPEIssueLogList.lstPPEIssueLog.Add(objPPEIssueLog);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objPPEIssueLogList.lstPPEIssueLog.ToList());
        }

        [AllowAnonymous]
        public ActionResult PPEIssueLogHistoryList()
        {
            PPEIssueLogModelsList objPPEIssueLogList = new PPEIssueLogModelsList();
            objPPEIssueLogList.lstPPEIssueLog = new List<PPEIssueLogModels>();

            try
            {
                if (Request.QueryString["IssueLog_Id"] != null && Request.QueryString["IssueLog_Id"] != "")
                {
                    string IssueLog_Id = Request.QueryString["IssueLog_Id"];
                    string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                    + " PPE_Issue_Voucher, LoggedBy,branch,Department from t_ppe_issuelog_trans where IssueLog_Id='" + IssueLog_Id + "' order by Issue_Date desc";

                    DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPPELog.Tables.Count > 0)
                    {
                        for (int i = 0; i < dsPPELog.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                                {
                                    IssueLog_Id = dsPPELog.Tables[0].Rows[i]["IssueLog_Id"].ToString(),
                                    Receiver_Name = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Receiver_Name"].ToString()),
                                    Position = dsPPELog.Tables[0].Rows[i]["Position"].ToString(),
                                    Cust_Project_Name = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["Cust_Project_Name"].ToString()),
                                    Work_Location = objGlobaldata.GetDivisionLocationById(dsPPELog.Tables[0].Rows[i]["Work_Location"].ToString()),
                                    PPE_Issued = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["PPE_Issued"].ToString()),
                                    Issued_By = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Issued_By"].ToString()),
                                    PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[i]["PPE_Issue_Voucher"].ToString(),
                                    LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                    branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPPELog.Tables[0].Rows[i]["branch"].ToString()),
                                    Department = objGlobaldata.GetMultiDeptNameById(dsPPELog.Tables[0].Rows[i]["Department"].ToString()),
                                };

                                DateTime dateValue;
                                if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["Issue_Date"].ToString(), out dateValue))
                                {
                                    objPPEIssueLog.Issue_Date = dateValue;
                                }

                                if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                                {
                                    objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                                }

                                objPPEIssueLogList.lstPPEIssueLog.Add(objPPEIssueLog);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogHistoryList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["Successdata"] = "No Data Exists";
                        return RedirectToAction("PPEIssueLogList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogHistoryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objPPEIssueLogList.lstPPEIssueLog.ToList());
        }

        [AllowAnonymous]
        public JsonResult PPEIssueLogListSearch(string branch_name)
        {
            PPEIssueLogModelsList objPPEIssueLogList = new PPEIssueLogModelsList();
            objPPEIssueLogList.lstPPEIssueLog = new List<PPEIssueLogModels>();

            ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Project = objGlobaldata.GetDropdownList("Projects");
            ViewBag.PPEIssued = objGlobaldata.GetDropdownList("PPE Issued");

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                    + " PPE_Issue_Voucher, LoggedBy,branch,Department from t_ppe_issuelog where Active=1 ";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by Issue_Date desc";

                DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPPELog.Tables.Count > 0)
                {
                    for (int i = 0; i < dsPPELog.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                            {
                                IssueLog_Id = dsPPELog.Tables[0].Rows[i]["IssueLog_Id"].ToString(),
                                Receiver_Name = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Receiver_Name"].ToString()),
                                Position = dsPPELog.Tables[0].Rows[i]["Position"].ToString(),
                                Cust_Project_Name = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["Cust_Project_Name"].ToString()),
                                Work_Location = objGlobaldata.GetDivisionLocationById(dsPPELog.Tables[0].Rows[i]["Work_Location"].ToString()),
                                PPE_Issued = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[i]["PPE_Issued"].ToString()),
                                Issued_By = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["Issued_By"].ToString()),
                                PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[i]["PPE_Issue_Voucher"].ToString(),
                                LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[i]["LoggedBy"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPPELog.Tables[0].Rows[i]["branch"].ToString()),
                                Department = objGlobaldata.GetMultiDeptNameById(dsPPELog.Tables[0].Rows[i]["Department"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["Issue_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.Issue_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsPPELog.Tables[0].Rows[i]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                            {
                                objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                            }

                            objPPEIssueLogList.lstPPEIssueLog.Add(objPPEIssueLog);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult PPEIssueLogDetails()
        {
            try
            {
                if (Request.QueryString["IssueLog_Id"] != null && Request.QueryString["IssueLog_Id"] != "")
                {
                    string sIssueLog_Id = Request.QueryString["IssueLog_Id"];
                    string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                        + " PPE_Issue_Voucher, LoggedBy,branch,Department,ppe_detail,ppe_qty from t_ppe_issuelog where IssueLog_Id='" + sIssueLog_Id + "'";

                    DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPPELog.Tables.Count > 0)
                    {
                        PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                        {
                            IssueLog_Id = dsPPELog.Tables[0].Rows[0]["IssueLog_Id"].ToString(),
                            Receiver_Name = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["Receiver_Name"].ToString()),
                            Position = dsPPELog.Tables[0].Rows[0]["Position"].ToString(),
                            Cust_Project_Name = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[0]["Cust_Project_Name"].ToString()),
                            Work_Location = objGlobaldata.GetDivisionLocationById(dsPPELog.Tables[0].Rows[0]["Work_Location"].ToString()),
                            PPE_Issued = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[0]["PPE_Issued"].ToString()),
                            Issued_By = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["Issued_By"].ToString()),
                            PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[0]["PPE_Issue_Voucher"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPPELog.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsPPELog.Tables[0].Rows[0]["Department"].ToString()),
                            ppe_detail = dsPPELog.Tables[0].Rows[0]["ppe_detail"].ToString(),
                            ppe_qty = dsPPELog.Tables[0].Rows[0]["ppe_qty"].ToString(),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["Issue_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Issue_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                        }

                        return View(objPPEIssueLog);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("PPEIssueLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "PPE issue Id cannot be null";
                    return RedirectToAction("PPEIssueLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PPEIssueLogList");
        }

        [AllowAnonymous]
        public ActionResult PPEIssueLogInfo(int id)
        {
            try
            {
                string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                    + " PPE_Issue_Voucher, LoggedBy,branch,Department from t_ppe_issuelog where IssueLog_Id='" + id + "'";

                DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPPELog.Tables.Count > 0)
                {
                    PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                    {
                        IssueLog_Id = dsPPELog.Tables[0].Rows[0]["IssueLog_Id"].ToString(),
                        Receiver_Name = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["Receiver_Name"].ToString()),
                        Position = dsPPELog.Tables[0].Rows[0]["Position"].ToString(),
                        Cust_Project_Name = dsPPELog.Tables[0].Rows[0]["Cust_Project_Name"].ToString(),
                        Work_Location = objGlobaldata.GetDivisionLocationById(dsPPELog.Tables[0].Rows[0]["Work_Location"].ToString()),
                        PPE_Issued = objGlobaldata.GetDropdownitemById(dsPPELog.Tables[0].Rows[0]["PPE_Issued"].ToString()),
                        Issued_By = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["Issued_By"].ToString()),
                        PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[0]["PPE_Issue_Voucher"].ToString(),
                        LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["LoggedBy"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsPPELog.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsPPELog.Tables[0].Rows[0]["Department"].ToString()),
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["Issue_Date"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.Issue_Date = dateValue;
                    }

                    if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                    {
                        objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                    }

                    return View(objPPEIssueLog);
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("PPEIssueLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PPEIssueLogList");
        }

        [AllowAnonymous]
        public ActionResult PPEIssueLogEdit()
        {
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Project = objGlobaldata.GetDropdownList("Projects");

                if (Request.QueryString["IssueLog_Id"] != null && Request.QueryString["IssueLog_Id"] != "")
                {
                    string sIssueLog_Id = Request.QueryString["IssueLog_Id"];
                    string sSqlstmt = "select IssueLog_Id, Issue_Date, Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, PPE_Issued_Last_Date, Issued_By,"
                        + " PPE_Issue_Voucher, LoggedBy,branch,Department,ppe_detail,ppe_qty from t_ppe_issuelog where IssueLog_Id='" + sIssueLog_Id + "'";

                    DataSet dsPPELog = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPPELog.Tables.Count > 0)
                    {
                        PPEIssueLogModels objPPEIssueLog = new PPEIssueLogModels
                        {
                            IssueLog_Id = dsPPELog.Tables[0].Rows[0]["IssueLog_Id"].ToString(),
                            Receiver_Name = (dsPPELog.Tables[0].Rows[0]["Receiver_Name"].ToString()),
                            Position = dsPPELog.Tables[0].Rows[0]["Position"].ToString(),
                            Cust_Project_Name = dsPPELog.Tables[0].Rows[0]["Cust_Project_Name"].ToString(),
                            Work_Location = /*objGlobaldata.GetDivisionLocationById*/(dsPPELog.Tables[0].Rows[0]["Work_Location"].ToString()),
                            PPE_Issued = (dsPPELog.Tables[0].Rows[0]["PPE_Issued"].ToString()),
                            Issued_By = dsPPELog.Tables[0].Rows[0]["Issued_By"].ToString(),
                            PPE_Issue_Voucher = dsPPELog.Tables[0].Rows[0]["PPE_Issue_Voucher"].ToString(),
                            LoggedBy = objGlobaldata.GetEmpHrNameById(dsPPELog.Tables[0].Rows[0]["LoggedBy"].ToString()),
                            branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsPPELog.Tables[0].Rows[0]["branch"].ToString()),
                            Department = /*objGlobaldata.GetMultiDeptNameById*/(dsPPELog.Tables[0].Rows[0]["Department"].ToString()),
                            ppe_detail = (dsPPELog.Tables[0].Rows[0]["ppe_detail"].ToString()),
                            ppe_qty = (dsPPELog.Tables[0].Rows[0]["ppe_qty"].ToString()),
                        };
                        if (dsPPELog.Tables[0].Rows[0]["Receiver_Name"].ToString() != "")
                        {
                            ViewBag.ReceivedByArray = (dsPPELog.Tables[0].Rows[0]["Receiver_Name"].ToString()).Split(',');
                        }
                        DateTime dateValue;
                        if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["Issue_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.Issue_Date = dateValue;
                        }

                        if (DateTime.TryParse(dsPPELog.Tables[0].Rows[0]["PPE_Issued_Last_Date"].ToString(), out dateValue))
                        {
                            objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                        }
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsPPELog.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsPPELog.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.PPEIssued = objGlobaldata.GetDropdownList("PPE Issued");
                        ViewBag.IssueBy = objGlobaldata.GetHrEmployeeList();
                        return View(objPPEIssueLog);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("PPEIssueLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "PPE issue Id cannot be null";
                    return RedirectToAction("PPEIssueLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PPEIssueLogList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PPEIssueLogEdit(PPEIssueLogModels objPPEIssueLog, FormCollection form, HttpPostedFileBase PPE_Issue_Voucher)
        {
            try
            {
                objPPEIssueLog.branch = form["branch"];
                objPPEIssueLog.Work_Location = form["Work_Location"];
                objPPEIssueLog.Department = form["Department"];
                objPPEIssueLog.PPE_Issued = form["PPE_Issued"];

                DateTime dateValue;

                if (form["Issue_Date"] != null && DateTime.TryParse(form["Issue_Date"], out dateValue) == true)
                {
                    objPPEIssueLog.Issue_Date = dateValue;
                }

                if (form["PPE_Issued_Last_Date"] != null && DateTime.TryParse(form["PPE_Issued_Last_Date"], out dateValue) == true)
                {
                    objPPEIssueLog.PPE_Issued_Last_Date = dateValue;
                }

                if (PPE_Issue_Voucher != null && PPE_Issue_Voucher.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/HSE"), Path.GetFileName(PPE_Issue_Voucher.FileName));
                        string sFilename = "PPE" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        PPE_Issue_Voucher.SaveAs(sFilepath + "/" + sFilename);
                        objPPEIssueLog.PPE_Issue_Voucher = "~/DataUpload/MgmtDocs/HSE/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddPPEIssueLog: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPPEIssueLog.FunUpdatePPEIssueLog(objPPEIssueLog))
                {
                    TempData["Successdata"] = "Updated PPE Issue Log details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PPEIssueLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PPEIssueLogList");
        }

        [HttpPost]
        public JsonResult FunGetLastIssueDate(string Receiver_Name)
        {
            DateTime max_date = new DateTime();
            try
            {
                string sql = "select max(Issue_Date) as max_date from t_ppe_issuelog_trans where Receiver_Name = 6 and active = 1 ";
                DataSet dsList = objGlobaldata.Getdetails(sql);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtDocDate;
                    if (dsList.Tables[0].Rows[0]["max_date"].ToString() != ""
                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["max_date"].ToString(), out dtDocDate))
                    {
                        max_date = dtDocDate;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetLastIssueDate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(max_date);
        }

        //[HttpPost]
        public JsonResult doesEmployeeExist(string Receiver_Name)
        {
            PPEIssueLogModels objModel = new PPEIssueLogModels();
            var user = "";
            //string id = objGlobaldata.GetEmpIDByEmpNo(CompEmpId);
            if (Receiver_Name != null)
            {
                user = objModel.checkEmployeePPEExists(Receiver_Name);
            }

            return Json(user);
        }
    }
}