using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
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
    public class NCRCAPAController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public NCRCAPAController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "NCRCAPAList";
        }

        //
        // GET: /NCRCAPA/

        public ActionResult Index()
        {
            return View();
        }

        //Start: NCRLog

        //
        // GET: /NCRCAPA/AddNCRLog

        [AllowAnonymous]
        public ActionResult AddNCRLog()
        {
            try
            {
                //ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                DataSet dsNCRDetails = objGlobaldata.Getdetails("select NCR_Num, IssuedToDept from t_ncr_capa where CAR_ID='" + Request.QueryString["CAR_ID"] + "'");
                if (dsNCRDetails.Tables.Count > 0 && dsNCRDetails.Tables[0].Rows.Count > 0)
                {
                    ViewBag.Deptname = objGlobaldata.GetDeptNameById(dsNCRDetails.Tables[0].Rows[0]["IssuedToDept"].ToString());
                    ViewBag.DeptId = dsNCRDetails.Tables[0].Rows[0]["IssuedToDept"].ToString();

                    ViewBag.NC_Num = dsNCRDetails.Tables[0].Rows[0]["NCR_Num"].ToString();
                }
                ViewBag.CAR_ID = Request.QueryString["CAR_ID"];
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitilizeAddNCRLog();
        }

        // GET: /NCRCAPA/InitilizeAddNCRLog

        private ActionResult InitilizeAddNCRLog()
        {
            return View();
        }

        // POST: /NCRCAPA/AddNCRLog

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNCRLog(NCRLogModels objNCRLog, FormCollection form)
        {
            try
            {
                //if (ModelState.IsValid)
                //if (objNCRLog != null)
                {
                    objNCRLog.NCR_Dept = form["NCR_Dept"];
                    objNCRLog.ReportStatus = form["ReportStatus"];
                    objNCRLog.NCRNo = form["NC_Num"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Ncr_logDate"], out dateValue))
                    {
                        objNCRLog.Ncr_logDate = dateValue;
                    }

                    if (DateTime.TryParse(form["Correction_Date"], out dateValue))
                    {
                        objNCRLog.Correction_Date = dateValue;
                    }

                    if (DateTime.TryParse(form["Corrective_Action_date"], out dateValue))
                    {
                        objNCRLog.Corrective_Action_date = dateValue;
                    }

                    if (DateTime.TryParse(form["FollowupDate"], out dateValue))
                    {
                        objNCRLog.FollowupDate = dateValue;
                    }

                    if (DateTime.TryParse(form["Report_Close_Date"], out dateValue))
                    {
                        objNCRLog.Report_Close_Date = dateValue;
                    }

                    objNCRLog.FunAddNCRLog(objNCRLog);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.CAR_ID = form["CAR_ID"];

            return RedirectToAction("NCRLogList", new { CAR_ID = form["CAR_ID"] });
        }

        // GET: /NCRCAPA/NCRLogList

        [AllowAnonymous]
        public ActionResult NCRLogList()
        {
            NCRLogModelsList objNCRLogModelsList = new NCRLogModelsList();
            objNCRLogModelsList.NCRLogMList = new List<NCRLogModels>();

            try
            {
                string sNCRNo = "";

                DataSet dsNCRDetails = objGlobaldata.Getdetails("select NCR_Num, IssuedToDept from t_ncr_capa where CAR_ID='" + Request.QueryString["CAR_ID"] + "'");

                sNCRNo = dsNCRDetails.Tables[0].Rows[0]["NCR_Num"].ToString();

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "SELECT idt_NCR_Log, NCRNo, Ncr_logDate, NCR_Details, NCR_Dept, DeptName , Correction_Date, Corrective_Action_date, FollowupDate, ReportStatus, "
                    + "Report_Close_Date, Correction_Details from t_NCR_Log as TNRLog, t_departments as dept where TNRLog. NCR_Dept=dept.DeptId "
                    + " and NCRNo='" + sNCRNo + "' order by idt_NCR_Log desc";

                DataSet dsNCRLogList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNCRLogList.Tables.Count > 0 && dsNCRLogList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNCRLogList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NCRLogModels objNCRLogModels = new NCRLogModels
                            {
                                idt_NCR_Log = Convert.ToInt16(dsNCRLogList.Tables[0].Rows[i]["idt_NCR_Log"].ToString()),
                                NCRNo = dsNCRLogList.Tables[0].Rows[i]["NCRNo"].ToString(),
                                NCR_Details = dsNCRLogList.Tables[0].Rows[i]["NCR_Details"].ToString(),
                                NCR_Dept = dsNCRLogList.Tables[0].Rows[i]["DeptName"].ToString(),
                                ReportStatus = dsNCRLogList.Tables[0].Rows[i]["ReportStatus"].ToString(),
                                Correction_Details = dsNCRLogList.Tables[0].Rows[i]["Correction_Details"].ToString()
                            };

                            DateTime dateValue;

                            if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[i]["Ncr_logDate"].ToString(), out dateValue))
                            {
                                objNCRLogModels.Ncr_logDate = dateValue;
                            }

                            if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[i]["Correction_Date"].ToString(), out dateValue))
                            {
                                objNCRLogModels.Correction_Date = dateValue;
                            }

                            if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[i]["Corrective_Action_date"].ToString(), out dateValue))
                            {
                                objNCRLogModels.Corrective_Action_date = dateValue;
                            }

                            if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[i]["FollowupDate"].ToString(), out dateValue))
                            {
                                objNCRLogModels.FollowupDate = dateValue;
                            }

                            if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[i]["Report_Close_Date"].ToString(), out dateValue))
                            {
                                objNCRLogModels.Report_Close_Date = dateValue;
                            }

                            objNCRLogModelsList.NCRLogMList.Add(objNCRLogModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.CAR_ID = Request.QueryString["CAR_ID"];
            return View(objNCRLogModelsList.NCRLogMList.ToList());
        }

        // GET: /NCRCAPA/NCRLogDetails

        [AllowAnonymous]
        public ActionResult NCRLogDetails()
        {
            string sidt_NCR_Log = Request.QueryString["idt_NCR_Log"];

            NCRLogModels objNCRLogModels = new NCRLogModels();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "SELECT idt_NCR_Log, NCRNo, Ncr_logDate, NCR_Details, NCR_Dept, DeptName, Correction_Date, Corrective_Action_date, FollowupDate, ReportStatus, "
                    + "Report_Close_Date, Correction_Details from t_NCR_Log as TNRLog, t_departments as dept where TNRLog. NCR_Dept=dept.DeptId and idt_NCR_Log='"
                    + sidt_NCR_Log + "'";

                DataSet dsNCRLogList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNCRLogList.Tables.Count > 0 && dsNCRLogList.Tables[0].Rows.Count > 0)
                {
                    objNCRLogModels = new NCRLogModels
                    {
                        idt_NCR_Log = Convert.ToInt16(dsNCRLogList.Tables[0].Rows[0]["idt_NCR_Log"].ToString()),
                        NCRNo = dsNCRLogList.Tables[0].Rows[0]["NCRNo"].ToString(),
                        NCR_Details = dsNCRLogList.Tables[0].Rows[0]["NCR_Details"].ToString(),
                        NCR_Dept = dsNCRLogList.Tables[0].Rows[0]["DeptName"].ToString(),
                        ReportStatus = dsNCRLogList.Tables[0].Rows[0]["ReportStatus"].ToString(),
                        Correction_Details = dsNCRLogList.Tables[0].Rows[0]["Correction_Details"].ToString()
                    };

                    DateTime dateValue;

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Ncr_logDate"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Ncr_logDate = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Correction_Date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Correction_Date = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Corrective_Action_date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Corrective_Action_date = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["FollowupDate"].ToString(), out dateValue))
                    {
                        objNCRLogModels.FollowupDate = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Report_Close_Date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Report_Close_Date = dateValue;
                    }

                    ViewBag.NC_Num = objNCRLogModels.NCRNo;

                    DataSet dsNCRDetails = objGlobaldata.Getdetails("select CAR_ID from t_ncr_capa where NCR_Num='" + objNCRLogModels.NCRNo + "' and IssuedToDept='"
                        + dsNCRLogList.Tables[0].Rows[0]["NCR_Dept"].ToString() + "'");
                    if (dsNCRDetails.Tables.Count > 0 && dsNCRDetails.Tables[0].Rows.Count > 0)
                    {
                        int iCarID = 0;
                        int.TryParse(dsNCRDetails.Tables[0].Rows[0]["CAR_ID"].ToString(), out iCarID);

                        ViewBag.CAR_ID = iCarID;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRLogModels);
        }

        // GET: /NCRCAPA/NCRLogEdit

        [AllowAnonymous]
        public ActionResult NCRLogEdit()
        {
            NCRLogModels objNCRLogModels = new NCRLogModels();

            try
            {
                string sidt_NCR_Log = Request.QueryString["idt_NCR_Log"];
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS
                string sSqlstmt = "SELECT idt_NCR_Log, NCRNo, Ncr_logDate, NCR_Details, NCR_Dept, DeptName, Correction_Date, Corrective_Action_date, FollowupDate, ReportStatus, "
                    + "Report_Close_Date, Correction_Details from t_NCR_Log as TNRLog, t_departments as dept where TNRLog. NCR_Dept=dept.DeptId and idt_NCR_Log='"
                    + sidt_NCR_Log + "'";

                DataSet dsNCRLogList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsNCRLogList.Tables.Count > 0 && dsNCRLogList.Tables[0].Rows.Count > 0)
                {
                    objNCRLogModels = new NCRLogModels
                    {
                        idt_NCR_Log = Convert.ToInt16(dsNCRLogList.Tables[0].Rows[0]["idt_NCR_Log"].ToString()),
                        NCRNo = dsNCRLogList.Tables[0].Rows[0]["NCRNo"].ToString(),
                        NCR_Details = dsNCRLogList.Tables[0].Rows[0]["NCR_Details"].ToString(),
                        NCR_Dept = dsNCRLogList.Tables[0].Rows[0]["DeptName"].ToString(),
                        ReportStatus = dsNCRLogList.Tables[0].Rows[0]["ReportStatus"].ToString(),
                        Correction_Details = dsNCRLogList.Tables[0].Rows[0]["Correction_Details"].ToString()
                    };

                    DateTime dateValue;

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Ncr_logDate"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Ncr_logDate = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Correction_Date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Correction_Date = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Corrective_Action_date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Corrective_Action_date = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["FollowupDate"].ToString(), out dateValue))
                    {
                        objNCRLogModels.FollowupDate = dateValue;
                    }

                    if (DateTime.TryParse(dsNCRLogList.Tables[0].Rows[0]["Report_Close_Date"].ToString(), out dateValue))
                    {
                        objNCRLogModels.Report_Close_Date = dateValue;
                    }

                    int iCarID = 0;

                    DataSet dsNCRDetails = objGlobaldata.Getdetails("select CAR_ID from t_ncr_capa as tnc, t_ncr_log as tnl where NCR_Num=NCRNo and NCR_Dept=issuedtodept "
                        + "and idt_NCR_Log='" + objNCRLogModels.idt_NCR_Log + "'");
                    if (dsNCRDetails.Tables.Count > 0 && dsNCRDetails.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(dsNCRDetails.Tables[0].Rows[0]["CAR_ID"].ToString(), out iCarID);

                        ViewBag.CAR_ID = iCarID;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.ReportStatus = new string[] { "Open", "Closed" };

            return View(objNCRLogModels);
        }

        //POST: /NCRCAPA/NCRLogEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NCRLogEdit(NCRLogModels objNCRLogModels, FormCollection form)
        {
            try
            {
                string sidt_NCR_Log = Request.QueryString["idt_NCR_Log"];

                objNCRLogModels.NCR_Dept = form["NCR_Dept"];
                objNCRLogModels.ReportStatus = form["ReportStatus"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Ncr_logDate"], out dateValue))
                {
                    objNCRLogModels.Ncr_logDate = dateValue;
                }

                if (DateTime.TryParse(form["Correction_Date"], out dateValue))
                {
                    objNCRLogModels.Correction_Date = dateValue;
                }

                if (DateTime.TryParse(form["Corrective_Action_date"], out dateValue))
                {
                    objNCRLogModels.Corrective_Action_date = dateValue;
                }

                if (DateTime.TryParse(form["FollowupDate"], out dateValue))
                {
                    objNCRLogModels.FollowupDate = dateValue;
                }

                if (DateTime.TryParse(form["Report_Close_Date"], out dateValue))
                {
                    objNCRLogModels.Report_Close_Date = dateValue;
                }

                if (objNCRLogModels.FunUpdateNCRLog(objNCRLogModels))
                {
                    TempData["Successdata"] = "MCR log details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            ViewBag.CAR_ID = form["CAR_ID"];

            return RedirectToAction("NCRLogList", new { CAR_ID = form["CAR_ID"] });
        }

        //End

        //
        // GET: /NCRCAPA/AddNCRCAPA

        [AllowAnonymous]
        public ActionResult AddNCRCAPA()
        {
            try
            {
                ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DiscrepancyRelatedList = new string[] { "Documentation", "Employee", "Procedure", "Equipment", "Materials", "Supplier", "Service Provider", "Management", "Process" };
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRCAPA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNCRCAPA(NCRCAPAModels objNCRCAPA, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                string[] sDiscrepancy_Related = new string[] { "Documentation", "Employee", "Procedure", "Equipment", "Materials", "Supplier", "Service Provider", "Management", "Process" };
                HttpPostedFileBase files = Request.Files[0];

                objNCRCAPA.IssuedBy = form["IssuedBy"];
                objNCRCAPA.issuedTo = form["issuedTo"];
                objNCRCAPA.FindingType = form["FindingType"];
                objNCRCAPA.FindingIdentified = form["FindingIdentified"];

                foreach (string item in sDiscrepancy_Related)
                {
                    if (form["chk" + item] != null)
                    {
                        objNCRCAPA.Discrepancy_Related = objNCRCAPA.Discrepancy_Related + item + ",";
                    }
                }
                if (objNCRCAPA.Discrepancy_Related != null && objNCRCAPA.Discrepancy_Related != "")
                {
                    objNCRCAPA.Discrepancy_Related = objNCRCAPA.Discrepancy_Related.TrimEnd(',');
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["IssuedOn"], out dateValue) == true)
                {
                    objNCRCAPA.IssuedOn = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPA.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCR" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPA.upload = objNCRCAPA.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddNCRCAPA-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objNCRCAPA.FunAddNCRCAPA(objNCRCAPA))
                {
                    TempData["Successdata"] = "Added NCR-CAPA details successfully  with Reference Number '" + objNCRCAPA.NC_Num + "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRCAPA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRCAPAList");
        }

        [AllowAnonymous]
        public ActionResult NCRCAPAList(string branch_name)
        {
            NCRCAPAModelsList NCRCAPAModelsList = new NCRCAPAModelsList();
            NCRCAPAModelsList.NCRCAPAMList = new List<NCRCAPAModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select CAR_ID,NC_Num,IssuedOn,IssuedBy,IssuedByPosition,IssuedByDept,issuedTo,Discrepancy_Details,"
                + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum from t_ncr_raise where Active=1";

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

                DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNCRCAPAList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels
                            {
                                CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["CAR_ID"].ToString()),
                                NC_Num = dsNCRCAPAList.Tables[0].Rows[i]["NC_Num"].ToString(),
                                IssuedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[i]["IssuedBy"].ToString()),
                                IssuedByPosition = dsNCRCAPAList.Tables[0].Rows[i]["IssuedByPosition"].ToString(),
                                IssuedByDept = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[i]["IssuedByDept"].ToString()),
                                issuedTo = objGlobaldata.GetMultiHrEmpNameById(dsNCRCAPAList.Tables[0].Rows[i]["issuedTo"].ToString()),
                                Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[i]["Discrepancy_Details"].ToString(),
                                Discrepancy_Related = dsNCRCAPAList.Tables[0].Rows[i]["Discrepancy_Related"].ToString(),
                                FindingType = dsNCRCAPAList.Tables[0].Rows[i]["FindingType"].ToString(),
                                upload = dsNCRCAPAList.Tables[0].Rows[i]["upload"].ToString(),
                                FindingIdentified = dsNCRCAPAList.Tables[0].Rows[i]["FindingIdentified"].ToString(),
                                AuditNum = objGlobaldata.GetAuditNumById(dsNCRCAPAList.Tables[0].Rows[i]["AuditNum"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["IssuedOn"].ToString(), out dateValue))
                            {
                                objNCRCAPAModels.IssuedOn = dateValue;
                            }
                            NCRCAPAModelsList.NCRCAPAMList.Add(objNCRCAPAModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCRCAPAList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPAList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(NCRCAPAModelsList.NCRCAPAMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult NCRCAPAListSearch(string branch_name)
        {
            NCRCAPAModelsList NCRCAPAModelsList = new NCRCAPAModelsList();
            NCRCAPAModelsList.NCRCAPAMList = new List<NCRCAPAModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select CAR_ID,NC_Num,IssuedOn,IssuedBy,IssuedByPosition,IssuedByDept,issuedTo,Discrepancy_Details,"
                + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum from t_ncr_raise where Active=1";

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

                DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNCRCAPAList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels
                            {
                                CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["CAR_ID"].ToString()),
                                NC_Num = dsNCRCAPAList.Tables[0].Rows[i]["NC_Num"].ToString(),
                                IssuedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[i]["IssuedBy"].ToString()),
                                IssuedByPosition = dsNCRCAPAList.Tables[0].Rows[i]["IssuedByPosition"].ToString(),
                                IssuedByDept = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[i]["IssuedByDept"].ToString()),
                                issuedTo = objGlobaldata.GetMultiHrEmpNameById(dsNCRCAPAList.Tables[0].Rows[i]["issuedTo"].ToString()),
                                Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[i]["Discrepancy_Details"].ToString(),
                                Discrepancy_Related = dsNCRCAPAList.Tables[0].Rows[i]["Discrepancy_Related"].ToString(),
                                FindingType = dsNCRCAPAList.Tables[0].Rows[i]["FindingType"].ToString(),
                                upload = dsNCRCAPAList.Tables[0].Rows[i]["upload"].ToString(),
                                FindingIdentified = dsNCRCAPAList.Tables[0].Rows[i]["FindingIdentified"].ToString(),
                                AuditNum = objGlobaldata.GetAuditNumById(dsNCRCAPAList.Tables[0].Rows[i]["AuditNum"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["IssuedOn"].ToString(), out dateValue))
                            {
                                objNCRCAPAModels.IssuedOn = dateValue;
                            }
                            NCRCAPAModelsList.NCRCAPAMList.Add(objNCRCAPAModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCRCAPAListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPAListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult NCRCAPAEdit()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.FindingType = new string[] { "Major NonConformity", "Minor NonConformity", "Observation" };
                ViewBag.FindingIdentified = new string[] { "Internal Audit", "External Audit", "During Review", "Observed" };
                ViewBag.AuditNumList = objGlobaldata.GetAuditNumList();
                if (Request.QueryString["CAR_ID"] != null && Request.QueryString["CAR_ID"] != "")
                {
                    string sCAR_ID = Request.QueryString["CAR_ID"];

                    string sSqlstmt = "select CAR_ID,NC_Num,IssuedOn,IssuedBy,IssuedByPosition,IssuedByDept,issuedTo,Discrepancy_Details,"
               + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum from t_ncr_raise where CAR_ID='" + sCAR_ID + "'";

                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["CAR_ID"].ToString()),
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            IssuedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedBy"].ToString()),
                            IssuedByPosition = dsNCRCAPAList.Tables[0].Rows[0]["IssuedByPosition"].ToString(),
                            IssuedByDept = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedByDept"].ToString()),
                            issuedTo = (dsNCRCAPAList.Tables[0].Rows[0]["issuedTo"].ToString()),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString(),
                            Discrepancy_Related = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Related"].ToString(),
                            FindingType = dsNCRCAPAList.Tables[0].Rows[0]["FindingType"].ToString(),
                            upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString(),
                            FindingIdentified = dsNCRCAPAList.Tables[0].Rows[0]["FindingIdentified"].ToString(),
                            AuditNum = dsNCRCAPAList.Tables[0].Rows[0]["AuditNum"].ToString()
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["IssuedOn"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.IssuedOn = dateValue;
                        }
                        string[] sDiscrepancy = new string[] { "Documentation", "Employee", "Procedure", "Equipment", "Materials", "Supplier", "ServiceProvider", "Management", "Process" };

                        string[] sDiscrepancy_Related_val = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Related"].ToString().Replace(" ", "").Split(',');
                        List<DiscrepancyRelatedList> lstdisp = new List<DiscrepancyRelatedList>();
                        foreach (string item in sDiscrepancy)
                        {
                            if (Array.Exists(sDiscrepancy_Related_val, element => element == item))
                            {
                                lstdisp.Add(new DiscrepancyRelatedList { Discrepancy = item, Checked = "checked" });
                            }
                            else
                            {
                                lstdisp.Add(new DiscrepancyRelatedList { Discrepancy = item, Checked = "" });
                            }
                        }
                        ViewBag.DiscrepancyRelatedList = lstdisp;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        //POST: /NCRCAPA/NCRCAPAEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NCRCAPAEdit(NCRCAPAModels objNCRCAPAModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                objNCRCAPAModels.IssuedBy = form["IssuedBy"];
                objNCRCAPAModels.issuedTo = form["issuedTo"];
                objNCRCAPAModels.FindingType = form["FindingType"];
                objNCRCAPAModels.FindingIdentified = form["FindingIdentified"];

                string[] sDiscrepancy_Related = new string[] { "Documentation", "Employee", "Procedure", "Equipment", "Materials", "Supplier", "Service Provider", "Management", "Process" };
                foreach (string item in sDiscrepancy_Related)
                {
                    if (form["chk" + item] != null)
                    {
                        objNCRCAPAModels.Discrepancy_Related = objNCRCAPAModels.Discrepancy_Related + item + ",";
                    }
                }

                if (objNCRCAPAModels.Discrepancy_Related != null && objNCRCAPAModels.Discrepancy_Related != "")
                {
                    objNCRCAPAModels.Discrepancy_Related = objNCRCAPAModels.Discrepancy_Related.TrimEnd(',');
                }

                DateTime dateValue;
                if (DateTime.TryParse(form["IssuedOn"], out dateValue) == true)
                {
                    objNCRCAPAModels.IssuedOn = dateValue;
                }
                string QCDelete = Request.Form["QCDocsValselectall"];
                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPAModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCR" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPAModels.upload = objNCRCAPAModels.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in BiddingEdit-upload: " + ex.ToString());
                        }
                    }
                    objNCRCAPAModels.upload = objNCRCAPAModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objNCRCAPAModels.upload = objNCRCAPAModels.upload + "," + form["QCDocsVal"];
                    objNCRCAPAModels.upload = objNCRCAPAModels.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null)
                {
                    objNCRCAPAModels.upload = null;
                }

                if (objNCRCAPAModels.FunUpdateNCRCAPA(objNCRCAPAModels))
                {
                    TempData["Successdata"] = "NCR-CAPA details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRCAPAList");//View();
        }

        [AllowAnonymous]
        public ActionResult ActionNCRCAPA()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();
            try
            {
                if (Request.QueryString["CAR_ID"] != null && Request.QueryString["CAR_ID"] != "")
                {
                    string CAR_ID = Request.QueryString["CAR_ID"];
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.CAR_ID = CAR_ID;

                    string sSqlstmt = "select NC_Num,Discrepancy_Details from t_ncr_raise where  CAR_ID='" + CAR_ID + "'";
                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString()
                        };
                    }
                }
                else
                {
                    TempData["Successdata"] = "ID cannot be null";
                    return RedirectToAction("NCRCAPAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionNCRCAPA(NCRCAPAModels objNCRCAPA, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];

                objNCRCAPA.Correction_taken_by = form["Correction_taken_by"];
                objNCRCAPA.CAPA_Need = form["CAPA_Need"];
                objNCRCAPA.CA_Proposed_By = form["CA_Proposed_By"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Correction_taken_on"], out dateValue) == true)
                {
                    objNCRCAPA.Correction_taken_on = dateValue;
                }
                if (DateTime.TryParse(form["CA_Impl_On"], out dateValue) == true)
                {
                    objNCRCAPA.CA_Impl_On = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPA.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCRAction" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPA.upload = objNCRCAPA.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPA-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objNCRCAPA.FunAddNCRAction(objNCRCAPA))
                {
                    TempData["Successdata"] = "Added NCR-Action details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRActionList", new { objNCRCAPA.CAR_ID });
        }

        [AllowAnonymous]
        public ActionResult NCRActionList()
        {
            NCRCAPAModelsList NCRCAPAModelsList = new NCRCAPAModelsList();
            NCRCAPAModelsList.NCRCAPAMList = new List<NCRCAPAModels>();

            try
            {
                if (Request.QueryString["CAR_ID"] != null && Request.QueryString["CAR_ID"] != "")
                {
                    string sCAR_ID = Request.QueryString["CAR_ID"];
                    string sSqlstmt = "select NC_Num,id_ncr_action,t.CAR_ID,NCR_CORRECTION_DESC,Correction_taken_on,Correction_taken_by,"
                    + "RootCause_Invtiresult,CAPA_Need,CAPA_DESC,CA_Impl_On,CA_Proposed_By,t.upload from t_ncr_action t,t_ncr_raise tt where t.CAR_ID=tt.CAR_ID and t.CAR_ID='" + sCAR_ID + "' and t.Active=1";
                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsNCRCAPAList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels
                                {
                                    NC_Num = dsNCRCAPAList.Tables[0].Rows[i]["NC_Num"].ToString(),
                                    id_ncr_action = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["id_ncr_action"].ToString()),
                                    CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["CAR_ID"].ToString()),
                                    NCR_CORRECTION_DESC = dsNCRCAPAList.Tables[0].Rows[i]["NCR_CORRECTION_DESC"].ToString(),
                                    Correction_taken_by = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[i]["Correction_taken_by"].ToString()),
                                    RootCause_Invtiresult = dsNCRCAPAList.Tables[0].Rows[i]["RootCause_Invtiresult"].ToString(),
                                    CAPA_Need = dsNCRCAPAList.Tables[0].Rows[i]["CAPA_Need"].ToString(),
                                    CAPA_DESC = dsNCRCAPAList.Tables[0].Rows[i]["CAPA_DESC"].ToString(),
                                    CA_Proposed_By = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[i]["CA_Proposed_By"].ToString()),
                                    upload = dsNCRCAPAList.Tables[0].Rows[i]["upload"].ToString()
                                };

                                DateTime dateValue;
                                if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["Correction_taken_on"].ToString(), out dateValue))
                                {
                                    objNCRCAPAModels.Correction_taken_on = dateValue;
                                }
                                if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["CA_Impl_On"].ToString(), out dateValue))
                                {
                                    objNCRCAPAModels.CA_Impl_On = dateValue;
                                }
                                NCRCAPAModelsList.NCRCAPAMList.Add(objNCRCAPAModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCRActionList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                    return RedirectToAction("NCRCAPAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRActionList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(NCRCAPAModelsList.NCRCAPAMList.ToList());
        }

        [AllowAnonymous]
        public ActionResult ActionNCRCAPAEdit()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.CAPA_Need = new string[] { "Yes", "No" };

                if (Request.QueryString["id_ncr_action"] != null && Request.QueryString["id_ncr_action"] != "")
                {
                    string sid_ncr_action = Request.QueryString["id_ncr_action"];

                    string sSqlstmt = "select NC_Num,Discrepancy_Details,id_ncr_action,t.CAR_ID,NCR_CORRECTION_DESC,Correction_taken_on,Correction_taken_by,"
                + "RootCause_Invtiresult,CAPA_Need,CAPA_DESC,CA_Impl_On,CA_Proposed_By,t.upload from t_ncr_action t,t_ncr_raise tt where t.CAR_ID=tt.CAR_ID and id_ncr_action='" + sid_ncr_action + "'";

                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString(),
                            id_ncr_action = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["id_ncr_action"].ToString()),
                            CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["CAR_ID"].ToString()),
                            NCR_CORRECTION_DESC = dsNCRCAPAList.Tables[0].Rows[0]["NCR_CORRECTION_DESC"].ToString(),
                            Correction_taken_by = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["Correction_taken_by"].ToString()),
                            RootCause_Invtiresult = dsNCRCAPAList.Tables[0].Rows[0]["RootCause_Invtiresult"].ToString(),
                            CAPA_Need = dsNCRCAPAList.Tables[0].Rows[0]["CAPA_Need"].ToString(),
                            CAPA_DESC = dsNCRCAPAList.Tables[0].Rows[0]["CAPA_DESC"].ToString(),
                            CA_Proposed_By = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["CA_Proposed_By"].ToString()),
                            upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString()
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["Correction_taken_on"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.Correction_taken_on = dateValue;
                        }
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["CA_Impl_On"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.CA_Impl_On = dateValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActionNCRCAPAEdit(NCRCAPAModels objNCRCAPA, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];

                objNCRCAPA.Correction_taken_by = form["Correction_taken_by"];
                objNCRCAPA.CAPA_Need = form["CAPA_Need"];
                objNCRCAPA.CA_Proposed_By = form["CA_Proposed_By"];

                DateTime dateValue;
                if (DateTime.TryParse(form["Correction_taken_on"], out dateValue) == true)
                {
                    objNCRCAPA.Correction_taken_on = dateValue;
                }
                if (DateTime.TryParse(form["CA_Impl_On"], out dateValue) == true)
                {
                    objNCRCAPA.CA_Impl_On = dateValue;
                }

                string QCDelete = Request.Form["QCDocsValselectall"];
                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPA.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCRAction" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPA.upload = objNCRCAPA.upload + "," + "~/DataUpload/MgmtDocs/Audit" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in BiddingEdit-upload: " + ex.ToString());
                        }
                    }
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objNCRCAPA.upload = objNCRCAPA.upload + "," + form["QCDocsVal"];
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objNCRCAPA.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objNCRCAPA.upload = null;
                }
                if (objNCRCAPA.FunUpdateActionNCRCAPA(objNCRCAPA))
                {
                    TempData["Successdata"] = "NCR Action details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRActionList", new { objNCRCAPA.CAR_ID });//View();
        }

        [AllowAnonymous]
        public ActionResult AddNCRClose()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();
            try
            {
                if (Request.QueryString["id_ncr_action"] != null && Request.QueryString["id_ncr_action"] != "")
                {
                    string id_ncr_action = Request.QueryString["id_ncr_action"];
                    string CAR_ID = Request.QueryString["CAR_ID"];
                    ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.id_ncr_action = id_ncr_action;
                    ViewBag.CAR_ID = CAR_ID;
                    string sSqlstmt = "select NC_Num,Discrepancy_Details from t_ncr_raise where  CAR_ID='" + CAR_ID + "'";
                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString()
                        };
                    }
                }
                else
                {
                    TempData["Successdata"] = "ID cannot be null";
                    return RedirectToAction("");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRClose: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNCRClose(NCRCAPAModels objNCRCAPA, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];

                objNCRCAPA.NCR_Status = form["NCR_Status"];
                objNCRCAPA.VerifiedBy = form["VerifiedBy"];
                objNCRCAPA.VerifiedDeptName = form["VerifiedDeptName"];

                DateTime dateValue;
                if (DateTime.TryParse(form["verified_on"], out dateValue) == true)
                {
                    objNCRCAPA.verified_on = dateValue;
                }
                if (DateTime.TryParse(form["Report_Closed_On"], out dateValue) == true)
                {
                    objNCRCAPA.Report_Closed_On = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPA.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCRClose" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPA.upload = objNCRCAPA.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddNCRClose-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objNCRCAPA.FunAddNCRClose(objNCRCAPA))
                {
                    TempData["Successdata"] = "Added NCR Close details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNCRClose: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRCloseList", new { objNCRCAPA.id_ncr_action });
        }

        [AllowAnonymous]
        public ActionResult NCRCloseList(int? page)
        {
            NCRCAPAModelsList NCRCAPAModelsList = new NCRCAPAModelsList();
            NCRCAPAModelsList.NCRCAPAMList = new List<NCRCAPAModels>();

            try
            {
                if (Request.QueryString["id_ncr_action"] != null && Request.QueryString["id_ncr_action"] != "")
                {
                    string sid_ncr_action = Request.QueryString["id_ncr_action"];
                    string sSqlstmt = "select NC_Num,id_ncr_close,t.CAR_ID,id_ncr_action,CAPA_EFF_VERIFICATION,verified_on,NCR_Status,"
                    + "Report_Closed_On,VerifiedBy,VerifiedByPosition,VerifiedDeptName,t.upload from t_ncr_close t,t_ncr_raise tt where t.CAR_ID=tt.CAR_ID and id_ncr_action='" + sid_ncr_action + "' and t.Active=1";
                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsNCRCAPAList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels
                                {
                                    NC_Num = dsNCRCAPAList.Tables[0].Rows[i]["NC_Num"].ToString(),
                                    id_ncr_close = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["id_ncr_close"].ToString()),
                                    id_ncr_action = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["id_ncr_action"].ToString()),
                                    CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[i]["CAR_ID"].ToString()),
                                    CAPA_EFF_VERIFICATION = dsNCRCAPAList.Tables[0].Rows[i]["CAPA_EFF_VERIFICATION"].ToString(),
                                    NCR_Status = dsNCRCAPAList.Tables[0].Rows[i]["NCR_Status"].ToString(),
                                    VerifiedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[i]["VerifiedBy"].ToString()),
                                    VerifiedByPosition = dsNCRCAPAList.Tables[0].Rows[i]["VerifiedByPosition"].ToString(),
                                    VerifiedDeptName = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[i]["VerifiedDeptName"].ToString()),
                                    upload = dsNCRCAPAList.Tables[0].Rows[i]["upload"].ToString(),
                                };
                                ViewBag.CAR_ID = objNCRCAPAModels.CAR_ID;
                                DateTime dateValue;
                                if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["verified_on"].ToString(), out dateValue))
                                {
                                    objNCRCAPAModels.verified_on = dateValue;
                                }
                                if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[i]["Report_Closed_On"].ToString(), out dateValue))
                                {
                                    objNCRCAPAModels.Report_Closed_On = dateValue;
                                }
                                NCRCAPAModelsList.NCRCAPAMList.Add(objNCRCAPAModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NCRCloseList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                    return RedirectToAction("NCRCAPAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCloseList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(NCRCAPAModelsList.NCRCAPAMList.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public ActionResult NCRCloseEdit()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                ViewBag.NCR_status = new string[] { "Open", "Closed" };
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();

                if (Request.QueryString["id_ncr_close"] != null && Request.QueryString["id_ncr_close"] != "")
                {
                    string sid_ncr_close = Request.QueryString["id_ncr_close"];

                    string sSqlstmt = "select NC_Num,id_ncr_close,id_ncr_action,CAPA_EFF_VERIFICATION,verified_on,NCR_Status,"
                    + "Report_Closed_On,VerifiedBy,VerifiedByPosition,VerifiedDeptName,t.upload from t_ncr_close t,t_ncr_raise tt where t.CAR_ID=tt.CAR_ID and id_ncr_close='" + sid_ncr_close + "'";

                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            id_ncr_close = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["id_ncr_close"].ToString()),
                            id_ncr_action = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["id_ncr_action"].ToString()),
                            CAPA_EFF_VERIFICATION = dsNCRCAPAList.Tables[0].Rows[0]["CAPA_EFF_VERIFICATION"].ToString(),
                            NCR_Status = dsNCRCAPAList.Tables[0].Rows[0]["NCR_Status"].ToString(),
                            VerifiedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["VerifiedBy"].ToString()),
                            VerifiedByPosition = dsNCRCAPAList.Tables[0].Rows[0]["VerifiedByPosition"].ToString(),
                            VerifiedDeptName = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[0]["VerifiedDeptName"].ToString()),
                            upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString(),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["verified_on"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.verified_on = dateValue;
                        }
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["Report_Closed_On"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.Report_Closed_On = dateValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCloseEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NCRCloseEdit(NCRCAPAModels objNCRCAPA, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];

                objNCRCAPA.NCR_Status = form["NCR_Status"];
                objNCRCAPA.VerifiedBy = form["VerifiedBy"];
                objNCRCAPA.VerifiedDeptName = form["VerifiedDeptName"];

                DateTime dateValue;
                if (DateTime.TryParse(form["verified_on"], out dateValue) == true)
                {
                    objNCRCAPA.verified_on = dateValue;
                }
                if (DateTime.TryParse(form["Report_Closed_On"], out dateValue) == true)
                {
                    objNCRCAPA.Report_Closed_On = dateValue;
                }

                string QCDelete = Request.Form["QCDocsValselectall"];
                if (upload != null && files.ContentLength > 0)
                {
                    objNCRCAPA.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NCRClose" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objNCRCAPA.upload = objNCRCAPA.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in BiddingEdit-upload: " + ex.ToString());
                        }
                    }
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objNCRCAPA.upload = objNCRCAPA.upload + "," + form["QCDocsVal"];
                    objNCRCAPA.upload = objNCRCAPA.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null)
                {
                    objNCRCAPA.upload = null;
                }

                if (objNCRCAPA.FunUpdateNCRClose(objNCRCAPA))
                {
                    TempData["Successdata"] = "NCR Close details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ActionNCRCAPAEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("NCRCloseList", new { objNCRCAPA.id_ncr_action });//View();
        }

        [AllowAnonymous]
        public ActionResult NCRActionDetails()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                if (Request.QueryString["id_ncr_action"] != null && Request.QueryString["id_ncr_action"] != "")
                {
                    string sid_ncr_action = Request.QueryString["id_ncr_action"];

                    string sSqlstmt = "select NC_Num,Discrepancy_Details,id_ncr_action,t.CAR_ID,NCR_CORRECTION_DESC,Correction_taken_on,Correction_taken_by,"
                + "RootCause_Invtiresult,CAPA_Need,CAPA_DESC,CA_Impl_On,CA_Proposed_By,t.upload from t_ncr_action t,t_ncr_raise tt where t.CAR_ID=tt.CAR_ID and id_ncr_action='" + sid_ncr_action + "'";

                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString(),
                            id_ncr_action = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["id_ncr_action"].ToString()),
                            CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["CAR_ID"].ToString()),
                            NCR_CORRECTION_DESC = dsNCRCAPAList.Tables[0].Rows[0]["NCR_CORRECTION_DESC"].ToString(),
                            Correction_taken_by = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["Correction_taken_by"].ToString()),
                            RootCause_Invtiresult = dsNCRCAPAList.Tables[0].Rows[0]["RootCause_Invtiresult"].ToString(),
                            CAPA_Need = dsNCRCAPAList.Tables[0].Rows[0]["CAPA_Need"].ToString(),
                            CAPA_DESC = dsNCRCAPAList.Tables[0].Rows[0]["CAPA_DESC"].ToString(),
                            CA_Proposed_By = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["CA_Proposed_By"].ToString()),
                            upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString()
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["Correction_taken_on"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.Correction_taken_on = dateValue;
                        }
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["CA_Impl_On"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.CA_Impl_On = dateValue;
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("NCRCAPAList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("NCRCAPAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRActionDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [AllowAnonymous]
        public JsonResult NCRCAPADocDelete(FormCollection form)
        {
            try
            {
                if (form["CAR_ID"] != null && form["CAR_ID"] != "")
                {
                    NCRCAPAModels Doc = new NCRCAPAModels();
                    string sCAR_ID = form["CAR_ID"];

                    if (Doc.FunDeleteNCRCAPADoc(sCAR_ID))
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
                    TempData["alertdata"] = "NCRCAPA Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPADocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult NCRActionDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_ncr_action"] != null && form["id_ncr_action"] != "")
                {
                    NCRCAPAModels Doc = new NCRCAPAModels();
                    string sid_ncr_action = form["id_ncr_action"];

                    if (Doc.FunDeleteNCRActionDoc(sid_ncr_action))
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
                    TempData["alertdata"] = "NCRCAPA Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRActionDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult NCRCloseDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_ncr_close"] != null && form["id_ncr_close"] != "")
                {
                    NCRCAPAModels Doc = new NCRCAPAModels();
                    string sid_ncr_close = form["id_ncr_close"];

                    if (Doc.FunDeleteNCRCloseDoc(sid_ncr_close))
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
                    TempData["alertdata"] = "NCRCAPA Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRActionDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult NCRCAPADetails()
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                if (Request.QueryString["CAR_ID"] != null && Request.QueryString["CAR_ID"] != "")
                {
                    string sCAR_ID = Request.QueryString["CAR_ID"];
                    string sSqlstmt = "select CAR_ID,NC_Num,IssuedOn,IssuedBy,IssuedByPosition,IssuedByDept,issuedTo,Discrepancy_Details,"
             + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum from t_ncr_raise where CAR_ID='" + sCAR_ID + "'";
                    DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                    {
                        objNCRCAPAModels = new NCRCAPAModels
                        {
                            CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["CAR_ID"].ToString()),
                            NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                            IssuedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedBy"].ToString()),
                            IssuedByPosition = dsNCRCAPAList.Tables[0].Rows[0]["IssuedByPosition"].ToString(),
                            IssuedByDept = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedByDept"].ToString()),
                            issuedTo = objGlobaldata.GetMultiHrEmpNameById(dsNCRCAPAList.Tables[0].Rows[0]["issuedTo"].ToString()),
                            Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString(),
                            Discrepancy_Related = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Related"].ToString(),
                            FindingType = dsNCRCAPAList.Tables[0].Rows[0]["FindingType"].ToString(),
                            upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString(),
                            FindingIdentified = dsNCRCAPAList.Tables[0].Rows[0]["FindingIdentified"].ToString(),
                            AuditNum = objGlobaldata.GetAuditNumById(dsNCRCAPAList.Tables[0].Rows[0]["AuditNum"].ToString()),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["IssuedOn"].ToString(), out dateValue))
                        {
                            objNCRCAPAModels.IssuedOn = dateValue;
                        }

                        string sql = "Select AuditLocation from t_internal_audit where AuditID = '" + dsNCRCAPAList.Tables[0].Rows[0]["AuditNum"].ToString() + "'";
                        DataSet dsAuditList = objGlobaldata.Getdetails(sql);
                        if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                        {
                            objNCRCAPAModels.Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["AuditLocation"].ToString());
                        }
                    }
                    string SqlStmt = "Select NC_Num,Correction_taken_by,Correction_taken_on,CAPA_Need,CA_Impl_On,CA_Proposed_By,"
                    + "verified_on,NCR_Status,Report_Closed_On,VerifiedBy FROM t_ncr_raise t LEFT JOIN t_ncr_action tt"
                    + " ON tt.CAR_ID = t.CAR_ID LEFT JOIN t_ncr_close ttt ON t.CAR_ID = ttt.CAR_ID where t.CAR_ID = '" + sCAR_ID + "'";

                    DataSet dsNCRCAPA = objGlobaldata.Getdetails(SqlStmt);
                    ViewBag.CAPADetails = dsNCRCAPA;
                }
                else
                {
                    TempData["alertdata"] = "CAR Id cannot be Null or empty";
                    return RedirectToAction("NCRCAPAList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPADetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        [AllowAnonymous]
        public ActionResult NCRCAPAInfo(int id)
        {
            NCRCAPAModels objNCRCAPAModels = new NCRCAPAModels();

            try
            {
                string sSqlstmt = "select CAR_ID,NC_Num,IssuedOn,IssuedBy,IssuedByPosition,IssuedByDept,issuedTo,Discrepancy_Details,"
         + "Discrepancy_Related,FindingType,upload,FindingIdentified,AuditNum from t_ncr_raise where CAR_ID='" + id + "'";
                DataSet dsNCRCAPAList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNCRCAPAList.Tables.Count > 0 && dsNCRCAPAList.Tables[0].Rows.Count > 0)
                {
                    objNCRCAPAModels = new NCRCAPAModels
                    {
                        CAR_ID = Convert.ToInt16(dsNCRCAPAList.Tables[0].Rows[0]["CAR_ID"].ToString()),
                        NC_Num = dsNCRCAPAList.Tables[0].Rows[0]["NC_Num"].ToString(),
                        IssuedBy = objGlobaldata.GetEmpHrNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedBy"].ToString()),
                        IssuedByPosition = dsNCRCAPAList.Tables[0].Rows[0]["IssuedByPosition"].ToString(),
                        IssuedByDept = objGlobaldata.GetDeptNameById(dsNCRCAPAList.Tables[0].Rows[0]["IssuedByDept"].ToString()),
                        issuedTo = objGlobaldata.GetMultiHrEmpNameById(dsNCRCAPAList.Tables[0].Rows[0]["issuedTo"].ToString()),
                        Discrepancy_Details = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Details"].ToString(),
                        Discrepancy_Related = dsNCRCAPAList.Tables[0].Rows[0]["Discrepancy_Related"].ToString(),
                        FindingType = dsNCRCAPAList.Tables[0].Rows[0]["FindingType"].ToString(),
                        upload = dsNCRCAPAList.Tables[0].Rows[0]["upload"].ToString(),
                        FindingIdentified = dsNCRCAPAList.Tables[0].Rows[0]["FindingIdentified"].ToString(),
                        AuditNum = objGlobaldata.GetAuditNumById(dsNCRCAPAList.Tables[0].Rows[0]["AuditNum"].ToString()),
                    };
                    DateTime dateValue;
                    if (DateTime.TryParse(dsNCRCAPAList.Tables[0].Rows[0]["IssuedOn"].ToString(), out dateValue))
                    {
                        objNCRCAPAModels.IssuedOn = dateValue;
                    }
                }
                string SqlStmt = "Select NC_Num,Correction_taken_by,Correction_taken_on,CAPA_Need,CA_Impl_On,CA_Proposed_By,"
                + "verified_on,NCR_Status,Report_Closed_On,VerifiedBy FROM t_ncr_raise t LEFT JOIN t_ncr_action tt"
                + " ON tt.CAR_ID = t.CAR_ID LEFT JOIN t_ncr_close ttt ON t.CAR_ID = ttt.CAR_ID where t.CAR_ID = '" + id + "'";

                DataSet dsNCRCAPA = objGlobaldata.Getdetails(SqlStmt);
                ViewBag.CAPADetails = dsNCRCAPA;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCRCAPADetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objNCRCAPAModels);
        }

        public JsonResult GetDivisionDetailsbyAuditNum(string AuditNum)
        {
            string Branch = "";
            string sql = "Select AuditLocation from t_internal_audit where AuditID = '" + AuditNum + "'";
            DataSet dsAuditList = objGlobaldata.Getdetails(sql);
            if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
            {
                Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["AuditLocation"].ToString());
            }
            return Json(Branch);
        }
    }
}