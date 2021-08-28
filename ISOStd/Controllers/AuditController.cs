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
    public class AuditController : Controller
    {
        InternalAuditModels objIAudit = new InternalAuditModels();
        clsGlobal objGlobaldata = new clsGlobal();

        public AuditController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "ScheduleAudit";
        }             
        
        public ActionResult Index()
        {
            return View();
        }       

        //
        // GET: /Audit/AddInternalaudit

        [AllowAnonymous]
        public ActionResult AddInternalAudit()
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            InternalAuditModels objAudit = new InternalAuditModels();
            try
            {

                objAudit.AuditLocation = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();//new MultiSelectList(emplist.EmpList, "Empid", "Empname");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();//GetDepartmentList();
                ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox(); //GetISOStdList();
                ViewBag.AuditTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.AuditTimeInMin = objGlobaldata.GetAuditTimeInMin();
               // ViewBag.AuditLocation = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.AuditStatus = objGlobaldata.GetDropdownList("Audit Status"); 
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAudit);
        }
               
        [AllowAnonymous]
        public JsonResult InternalAuditDocDelete(FormCollection form)
        {
            try
            {
                ViewBag.SubMenutype = "ScheduleAudit";

                if (form["AuditID"] != null && form["AuditID"] != "")
                        {
                            InternalAuditModels Doc = new InternalAuditModels();
                            string sAuditID = form["AuditID"];


                            if (Doc.FunDeleteInternalAuditDoc(sAuditID))
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
                            TempData["alertdata"] = "Internal Audit Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerEnquiryDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        //
        // POST: /Audit/AddInternalaudit
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddInternalAudit(InternalAuditModels objInterAudit, FormCollection form, HttpPostedFileBase upload)
        {
            //objGlobaldata.CreateUserSession();
            ViewBag.SubMenutype = "ScheduleAudit";
            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            
            try
            {
            
                //objInterAudit.AuditType = "12";
                DateTime dateValue;

                if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                {
                    objInterAudit.AuditDate = dateValue;
                }
                objInterAudit.AuditCriteria = form["AuditCriteria"];
                objInterAudit.AuditLocation = form["AuditLocation"];

                if (upload != null && upload.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(upload.FileName));
                        string sFilename = "InternalAudit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        upload.SaveAs(sFilepath + "/" + sFilename);
                        objInterAudit.upload = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                  

                InternalAuditModelsList objInternalAuditModelsList = new InternalAuditModelsList();
                objInternalAuditModelsList.InternalAuditList = new List<InternalAuditModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    InternalAuditModels objInternalAuditModels = new InternalAuditModels();

                    //objInterAudit.CompanyId = form["CompanyId"];//objUserInfo.CompanyId;
                    objInternalAuditModels.DeptName = form["DeptName"+i];
                    objInternalAuditModels.AuditTime = form["AuditFromTimeInHour" + i] + ":" + form["AuditFromTimeInMin" + i];
                    objInternalAuditModels.AuditToTime = form["AuditToTimeInHour" + i] + ":" + form["AuditToTimeInMin" + i];
                    objInternalAuditModels.Auditee = form["Auditee" + i];
                    objInternalAuditModels.Auditor = form["Auditor" + i];
                    objInternalAuditModels.Audit_Prepared_by = form["Audit_Prepared_by" + i];
                    objInternalAuditModels.AuditStatus = form["AuditStatus" + i];
                    objInternalAuditModels.Audit_Approved_by = form["ApprovedBy" + i];
                    objInternalAuditModels.Audit_Activity = form["AuditActivity" + i];

                    if (DateTime.TryParse(form["Audit_Planned_Date"+i], out dateValue) == true)
                    {
                        objInternalAuditModels.Audit_Planned_Date = dateValue;
                    }
                    if (DateTime.TryParse(form["DateScheduled"+i], out dateValue) == true)
                    {
                        objInternalAuditModels.DateScheduled = dateValue;
                    }
                    objInternalAuditModelsList.InternalAuditList.Add(objInternalAuditModels);
              }
            
             if (objIAudit.FunAddInternalAudit(objInterAudit, objInternalAuditModelsList))
             {
                 TempData["Successdata"] = "Added Audit details successfully  with Reference Number '" + objInterAudit.AuditNum + "'";
              
             }
             else
             {
                 TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
             }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InternalauditList");

            //return View(objInterAudit);
        }
                
        [AllowAnonymous]
        public ActionResult InternalAuditList(string branch_name)
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            InternalAuditModelsList objInternalAuditList =new InternalAuditModelsList();
            objInternalAuditList.InternalAuditList = new List<InternalAuditModels>();

            try
            {
               
               UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                //string sSqlstmt = "SELECT tAudit.AuditID, AuditNum, AuditDate, AuditCriteria, AuditLocation, deptId, Audit_Planned_Date, fromAuditTime, toAuditTime"
                //    + " FROM t_internal_audit as tAudit, t_internal_audit_trans as tiAudit where tAudit.AuditID=tiAudit.AuditID and tAudit.Active=1";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,Audit_Prepared_by,ApprovedBy," +
                    "+ (case when ApprvStatus = '1' then 'Approved' when ApprvStatus = '2' then 'Resheduled' else 'Not Approved' end) as ApprvStatus from t_internal_audit where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" +  branch_name + "', AuditLocation)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', AuditLocation)";
                }
                sSqlstmt= sSqlstmt+ sSearchtext+ " order by AuditDate desc"; 

                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                   
                    for (int i = 0; i < dsInternalAuditList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["AuditDate"].ToString());
                     //   DateTime dtAudit_Prepared_Date = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["Audit_Planned_Date"].ToString());

                        //string sAudittime = dtAudit_Prepared_Date.Date.ToString("dd/MM/yyyy") + " " + dsInternalAuditList.Tables[0].Rows[i]["fromAuditTime"].ToString()
                        //    + " - " + dsInternalAuditList.Tables[0].Rows[i]["toAuditTime"].ToString();

                        InternalAuditModels objInterAudit = new InternalAuditModels
                        {
                            AuditID = dsInternalAuditList.Tables[0].Rows[i]["AuditID"].ToString(),
                            AuditNum = dsInternalAuditList.Tables[0].Rows[i]["AuditNum"].ToString(),
                            AuditDate = dtAuditDate,
                          //  AuditTime = sAudittime,
                           // DeptName = objGlobaldata.GetDeptNameById(dsInternalAuditList.Tables[0].Rows[i]["deptId"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                            AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsInternalAuditList.Tables[0].Rows[i]["AuditLocation"].ToString()),
                            Audit_Prepared_by =objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["Audit_Prepared_by"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            ApprvStatus = (dsInternalAuditList.Tables[0].Rows[i]["ApprvStatus"].ToString()),
                        };
                        objInternalAuditList.InternalAuditList.Add(objInterAudit);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInternalAuditList.InternalAuditList.ToList());
        }

        [AllowAnonymous]
        public JsonResult InternalAuditListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            InternalAuditModelsList objInternalAuditList = new InternalAuditModelsList();
            objInternalAuditList.InternalAuditList = new List<InternalAuditModels>();

            try
            {

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                //string sSqlstmt = "SELECT tAudit.AuditID, AuditNum, AuditDate, AuditCriteria, AuditLocation, deptId, Audit_Planned_Date, fromAuditTime, toAuditTime"
                //    + " FROM t_internal_audit as tAudit, t_internal_audit_trans as tiAudit where tAudit.AuditID=tiAudit.AuditID and tAudit.Active=1";
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,Audit_Prepared_by,ApprovedBy," +
                    "+ (case when ApprvStatus = '1' then 'Approved' when ApprvStatus = '2' then 'Resheduled' else 'Not Approved' end) as ApprvStatus from t_internal_audit where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', AuditLocation)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', AuditLocation)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by AuditDate desc";

                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {

                  

                    for (int i = 0; i < dsInternalAuditList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["AuditDate"].ToString());
                        //   DateTime dtAudit_Prepared_Date = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["Audit_Planned_Date"].ToString());

                        //string sAudittime = dtAudit_Prepared_Date.Date.ToString("dd/MM/yyyy") + " " + dsInternalAuditList.Tables[0].Rows[i]["fromAuditTime"].ToString()
                        //    + " - " + dsInternalAuditList.Tables[0].Rows[i]["toAuditTime"].ToString();

                        InternalAuditModels objInterAudit = new InternalAuditModels
                        {
                            AuditID = dsInternalAuditList.Tables[0].Rows[i]["AuditID"].ToString(),
                            AuditNum = dsInternalAuditList.Tables[0].Rows[i]["AuditNum"].ToString(),
                            AuditDate = dtAuditDate,
                            //  AuditTime = sAudittime,
                            // DeptName = objGlobaldata.GetDeptNameById(dsInternalAuditList.Tables[0].Rows[i]["deptId"].ToString()),
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[i]["AuditCriteria"].ToString()),
                            AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsInternalAuditList.Tables[0].Rows[i]["AuditLocation"].ToString()),
                            Audit_Prepared_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["Audit_Prepared_by"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            ApprvStatus = (dsInternalAuditList.Tables[0].Rows[i]["ApprvStatus"].ToString()),
                        };
                        objInternalAuditList.InternalAuditList.Add(objInterAudit);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult InternalAuditDetails()
        {
            InternalAuditModelsList objInternalAuditList = new InternalAuditModelsList();
            objInternalAuditList.InternalAuditList = new List<InternalAuditModels>();
            ViewBag.SubMenutype = "ScheduleAudit";
            if (Request.QueryString["AuditNum"] != null)
            {
                string AuditNum = Request.QueryString["AuditNum"];
                try
                {
                    string sSqlstmt = "SELECT * from t_internal_audit where AuditNum='" + AuditNum + "'";

                    DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsInternalAuditList.Tables[0].Rows.Count; i++)
                        {
                            DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["AuditDate"].ToString());

                            InternalAuditModels objInterAudit = new InternalAuditModels
                            {
                                AuditID = dsInternalAuditList.Tables[0].Rows[0]["AuditID"].ToString(),
                                AuditNum = dsInternalAuditList.Tables[0].Rows[0]["AuditNum"].ToString(),
                                AuditDate = dtAuditDate,
                                AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                                AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditLocation"].ToString()),
                                upload = dsInternalAuditList.Tables[0].Rows[0]["upload"].ToString(),
                               
                            };
                            objInternalAuditList.InternalAuditList.Add(objInterAudit);
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("InternalAuditList");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in InternalAuditDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }        
            else
            {
                TempData["alertdata"] = "Audit id cannot be empty";
                return RedirectToAction("InternalAuditList");
            }

            return View(objInternalAuditList.InternalAuditList.ToList());
        }
                
        [AllowAnonymous]
        public ActionResult InternalAuditNumDetails()
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            InternalAuditModelsList objInternalAuditList = new InternalAuditModelsList();
            objInternalAuditList.InternalAuditList = new List<InternalAuditModels>();
            if (Request.QueryString["AuditID"] != null)
            {
                string AuditID = Request.QueryString["AuditID"];
                try
                {
                 
                        UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


                    string sSqlstmt = "SELECT AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,Audit_Prepared_by,ApprovedBy,upload," +
                   "+ (case when ApprvStatus = '1' then 'Approved' when ApprvStatus = '2' then 'Resheduled' else 'Not Approved' end) as ApprvStatus from t_internal_audit where AuditID='" + AuditID + "'";

                    //string sSqlstmt = "SELECT * FROM t_internal_audit where AuditID='" + AuditID + "'";
                    DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["AuditDate"].ToString());

                        InternalAuditModels objInterAudit = new InternalAuditModels
                        {
                            AuditID = dsInternalAuditList.Tables[0].Rows[0]["AuditID"].ToString(),
                            AuditNum = dsInternalAuditList.Tables[0].Rows[0]["AuditNum"].ToString(),
                            AuditDate = dtAuditDate,
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditLocation"].ToString()),
                            upload = dsInternalAuditList.Tables[0].Rows[0]["upload"].ToString(),
                            Audit_Prepared_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[0]["Audit_Prepared_by"].ToString()),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            ApprvStatus = (dsInternalAuditList.Tables[0].Rows[0]["ApprvStatus"].ToString()),
                        };
                        sSqlstmt = "select AuditTransID, AuditID, DeptID, fromAuditTime, toAuditTime, Auditee, Auditor, Audit_Prepared_by, AuditStatus, Audit_Planned_Date,ApprovedBy,Activity,"
                                 + "DateScheduled, CompletionDate,AuditDetails from t_internal_audit_trans where AuditID='" + objInterAudit.AuditID + "' order by AuditTransID desc";
                        ViewBag.PlanningDetails = objGlobaldata.Getdetails(sSqlstmt);

                        ViewBag.AuditNum = objInterAudit.AuditNum;
                        return View(objInterAudit);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("InternalAuditList");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in InternalAuditNumDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                TempData["alertdata"] = "Audit Number cannot be empty";
                return RedirectToAction("InternalAuditList");
            }
            return View(objInternalAuditList.InternalAuditList.ToList());
        }
                       
        [AllowAnonymous]
        public ActionResult InternalAuditNumInfo(int id)
        {
          
                try
                {
                ViewBag.SubMenutype = "ScheduleAudit";
                string sSqlstmt = "SELECT * FROM t_internal_audit where AuditID='" + id + "'";
                    DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["AuditDate"].ToString());

                        InternalAuditModels objInterAudit = new InternalAuditModels
                        {
                            AuditID = dsInternalAuditList.Tables[0].Rows[0]["AuditID"].ToString(),
                            AuditNum = dsInternalAuditList.Tables[0].Rows[0]["AuditNum"].ToString(),
                            AuditDate = dtAuditDate,
                            AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                            AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditLocation"].ToString()),
                            upload = dsInternalAuditList.Tables[0].Rows[0]["upload"].ToString()
                        };
                        sSqlstmt = "select AuditTransID, AuditID, DeptID, fromAuditTime, toAuditTime, Auditee, Auditor, Audit_Prepared_by, AuditStatus, Audit_Planned_Date,ApprovedBy,Activity,"
                                 + "DateScheduled, CompletionDate from t_internal_audit_trans where AuditID='" + objInterAudit.AuditID + "' order by AuditTransID desc";
                        ViewBag.PlanningDetails = objGlobaldata.Getdetails(sSqlstmt);
                        return View(objInterAudit);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("InternalAuditList");
                    }
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in InternalAuditNumDetails: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
           
            return View();
        }
                 
        [AllowAnonymous]
        public ActionResult InternalAuditEdit()
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            string AuditID = Request.QueryString["AuditID"];
            //objGlobaldata.CreateUserSession();            
            InternalAuditModels objAudit = new InternalAuditModels();
            if (AuditID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            string sSqlstmt = "SELECT AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,Audit_Prepared_by,ApprovedBy,upload," +
                "(case when ApprvStatus = '1' then 'Approved' when ApprvStatus = '2' then 'Resheduled' else 'Not Approved' end) as ApprvStatus from t_internal_audit where AuditID='" + AuditID + "'";

            // string sSqlstmt = "SELECT * FROM t_internal_audit where AuditID='" + AuditID + "'";
            DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                if (dsInternalAuditList.Tables.Count >0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                    //DateTime dtAuditDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["AuditDate"].ToString());

                    InternalAuditModels objInterAudit = new InternalAuditModels
                    {
                        AuditID = dsInternalAuditList.Tables[0].Rows[0]["AuditID"].ToString(),
                        AuditNum = dsInternalAuditList.Tables[0].Rows[0]["AuditNum"].ToString(),
                        //AuditDate = dtAuditDate,
                        AuditCriteria = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["AuditCriteria"].ToString()),
                        AuditLocation =/* objGlobaldata.GetMultiCompanyBranchNameById*/(dsInternalAuditList.Tables[0].Rows[0]["AuditLocation"].ToString()),
                        upload = dsInternalAuditList.Tables[0].Rows[0]["upload"].ToString(),
                        Audit_Prepared_by =(dsInternalAuditList.Tables[0].Rows[0]["Audit_Prepared_by"].ToString()),
                        ApprovedBy = (dsInternalAuditList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        ApprvStatus = (dsInternalAuditList.Tables[0].Rows[0]["ApprvStatus"].ToString()),
                    };
                    DateTime dtValue;
                    if (DateTime.TryParse(dsInternalAuditList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                    {
                        objInterAudit.AuditDate = dtValue;
                    }

                    sSqlstmt = "select AuditTransID, AuditID, DeptID, fromAuditTime, toAuditTime, Auditee, Auditor, Audit_Prepared_by, AuditStatus, Audit_Planned_Date,ApprovedBy,Activity,"
                              +"DateScheduled, CompletionDate from t_internal_audit_trans where AuditID='" + objInterAudit.AuditID + "' order by AuditTransID desc";
                    ViewBag.PlanningDetails = objGlobaldata.Getdetails(sSqlstmt);

                    ViewBag.AuditID = objInterAudit.AuditID;

                    MultiSelectList msltLst = objGlobaldata.GetHrEmployeeListbox();

                    ViewBag.EmpLists = msltLst;//new MultiSelectList(emplist.EmpList, "Empid", "Empname");
                    ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();//GetDepartmentList();
                    ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox(); //GetISOStdList();
                    ViewBag.AuditTimeInHour = objGlobaldata.GetAuditTimeInHour();
                    ViewBag.AuditTimeInMin = objGlobaldata.GetAuditTimeInMin();
                    ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                    ViewBag.AuditStatus = objGlobaldata.GetDropdownList("Audit Status");
                    ViewBag.AuditNum = objInterAudit.AuditNum;
                    return View(objInterAudit);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditEdit(InternalAuditModels objInterAudit, FormCollection form, HttpPostedFileBase upload)
        {
            try
            {
                ViewBag.SubMenutype = "ScheduleAudit";
                if (Request["button"].Equals("Audit Update"))
                {
                    string sIsoStdId = form["AuditCriteria"];
                    string sAuditNumVal = form["AuditNum"];
                    objInterAudit.AuditID = form["AuditID"];
                    if (HttpContext.Session["UserCredentials"] != null)
                    {
                        UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                    }
                    objInterAudit.AuditCriteria = sIsoStdId;

                    objInterAudit.AuditNum = sAuditNumVal;
                    objInterAudit.AuditLocation = form["AuditLocation"];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(upload.FileName));
                            string sFilename = "InternalAudit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objInterAudit.upload = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddInternalAudit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    objIAudit.FunUpdateInternalAudit(objInterAudit);
                }
                else if (Request["button"].Equals("Save"))
                {
                    InternalAuditPlanAdd(objInterAudit, form);
                    TempData["Successdata"] = " Audit Plan added successfully";
                }
                else
                {
                    InternalAuditPlanUpdate(objInterAudit, form);
                    TempData["Successdata"] = " Audit Plan updated successfully";
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            
            return RedirectToAction("InternalAuditList");
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditPlanUpdate(InternalAuditModels objInternalAuditModels, FormCollection form)
        {
            ViewBag.SubMenutype = "ScheduleAudit";

            try
            {
                objInternalAuditModels.AuditID = form["AuditID"];
                objInternalAuditModels.AuditTransID = form["AuditTransID"];

                objInternalAuditModels.DeptName = form["DeptName"];
                objInternalAuditModels.AuditTime = form["AuditFromTimeInHour"] + ":" + form["AuditFromTimeInMin"];
                objInternalAuditModels.AuditToTime = form["AuditToTimeInHour"] + ":" + form["AuditToTimeInMin"];
                objInternalAuditModels.Auditee = form["Auditee"];
                objInternalAuditModels.Auditor = form["Auditor"];
                objInternalAuditModels.Audit_Prepared_by = form["Audit_Prepared_by"];
                objInternalAuditModels.AuditStatus = form["AuditStatus"];
                objInternalAuditModels.Audit_Prepared_by = form["Audit_Prepared_by"];
                objInternalAuditModels.Audit_Approved_by = form["ApprovedBy"];
                objInternalAuditModels.Audit_Activity = form["Activity"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Audit_Planned_Date"], out dateValue) == true)
                {
                    objInternalAuditModels.Audit_Planned_Date = dateValue;
                }
                if (DateTime.TryParse(form["DateScheduled"], out dateValue) == true)
                {
                    objInternalAuditModels.DateScheduled = dateValue;
                }

                if (objInternalAuditModels.FunUpdateInternalAuditPlan(objInternalAuditModels))
                {
                    return Json("Update Success");
                }
                else
                {
                    return Json("Update Failed");
                
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditPlanUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return Json("Update Failed: " + ex.ToString());
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditPlanAdd(InternalAuditModels objInternalAuditModels, FormCollection form)
        {
            ViewBag.SubMenutype = "ScheduleAudit";
            try
            {
                InternalAuditModelsList objInternalAuditModelsList = new InternalAuditModelsList();
                objInternalAuditModelsList.InternalAuditList = new List<InternalAuditModels>();

                //objInterAudit.CompanyId = form["CompanyId"];//objUserInfo.CompanyId;
                objInternalAuditModels.AuditID = form["AuditID"];

                objInternalAuditModels.DeptName = form["DeptName"];
                objInternalAuditModels.AuditTime = form["AuditFromTimeInHour"] + ":" + form["AuditFromTimeInMin"];
                objInternalAuditModels.AuditToTime = form["AuditToTimeInHour"] + ":" + form["AuditToTimeInMin"];
                objInternalAuditModels.Auditee = form["Auditee"];
                objInternalAuditModels.Auditor = form["Auditor"];
                objInternalAuditModels.Audit_Prepared_by = form["Audit_Prepared_by"];
                objInternalAuditModels.AuditStatus = form["AuditStatus"];
                objInternalAuditModels.Audit_Prepared_by = form["Audit_Prepared_by"];
                objInternalAuditModels.Audit_Approved_by = form["ApprovedBy"];

                DateTime dateValue;

                if (DateTime.TryParse(form["Audit_Planned_Date"], out dateValue) == true)
                {
                    objInternalAuditModels.Audit_Planned_Date = dateValue;
                }
                if (DateTime.TryParse(form["DateScheduled"], out dateValue) == true)
                {
                    objInternalAuditModels.DateScheduled = dateValue;
                }

                objInternalAuditModelsList.InternalAuditList.Add(objInternalAuditModels);
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();
                if (objInternalAuditModels.FunAddInternalAuditPlan(objInternalAuditModelsList, Convert.ToInt16(objInternalAuditModels.AuditID)))
                {
                    TempData["Successdata"] = "Update Success";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                
                }
                return Json("Update Success");
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return Json("Update Failed: " + ex.ToString());
            }
        }


        //[AllowAnonymous]
        //public ActionResult AddInternalAuditNoFindingsLog()
        //{
        //    try
        //    {
        //        //objGlobaldata.CreateUserSession();

        //        InternalAuditModels objInternalAuditModel = new InternalAuditModels();

        //            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


        //        InternalAuditModels objAudit = new InternalAuditModels();
        //        // ViewBag.AuditNumList = objInternalAuditModel.FunGetInternalAuditList(objUserInfo.CompanyId);
        //        DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria from t_internal_audit, t_internal_audit_trans where"
        //            + " t_internal_audit.AuditID=t_internal_audit_trans.AuditID and AuditTransID='" + Request.QueryString["AuditTransID"] + "'");
        //        ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
        //        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
        //        ViewBag.AuditTransID = Request.QueryString["AuditTransID"];
        //        ViewBag.AuditNum = Request.QueryString["AuditNum"];
        //        //ViewBag.AuditDateList = GetAuditDateList(Request.QueryString["AuditNum"]);

        //       
        //        ViewBag.ReportStatus = objGlobaldata.GetDropdownList("Audit Status");
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditNoFindingsLog: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}



        [AllowAnonymous]
        public ActionResult AddInternalAuditFindingsLog()
        {
            ViewBag.SubMenutype = "InternalAudit";
            try
            {
                //objGlobaldata.CreateUserSession();

                InternalAuditModels objInternalAuditModel = new InternalAuditModels();
               
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                // ViewBag.AuditNumList = objInternalAuditModel.FunGetInternalAuditList(objUserInfo.CompanyId);

                string sAuditTransID = Request.QueryString["AuditTransID"];
                DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria,DeptID,Auditee,Auditor from t_internal_audit a, t_internal_audit_trans b where"
                    + " a.AuditID=b.AuditID and AuditTransID='" +sAuditTransID + "'");
                ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
                ViewBag.DeptID = objGlobaldata.GetDeptNameById(dsIso.Tables[0].Rows[0]["DeptID"].ToString());
                ViewBag.Auditee = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditee"].ToString());
                ViewBag.Auditor = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditor"].ToString());
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.AuditTransID = Request.QueryString["AuditTransID"];
                ViewBag.AuditNum = Request.QueryString["AuditNum"];
                ViewBag.AuditID = Request.QueryString["AuditID"];
                //ViewBag.AuditDateList = GetAuditDateList(Request.QueryString["AuditNum"]);

                ViewBag.FindingCategory = objGlobaldata.GetDropdownList("Audit Finding Category");
                ViewBag.ReportStatus = objGlobaldata.GetDropdownList("Audit Status");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditFindingsLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        
        private List<string> GetAuditDateList(string sAuditNum)
        {
            List<string> lstAuditDate = new List<string>();
            try
            {
                InternalAuditModels objInternalAuditModel = new InternalAuditModels();
                lstAuditDate = objInternalAuditModel.FunGetInternalAuditDateList(sAuditNum);
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in GetAuditDateList: " + ex.ToString());
            }
            return lstAuditDate;//Json(lstAuditDate); 
        }

        [AllowAnonymous]
        public ActionResult GetAuditorDeptList(string sAuditNum, DateTime AuditDatetime)
        {
            List<string> lstAuditDeptList = new List<string>();
            try
            {
                InternalAuditModels objInternalAuditModel = new InternalAuditModels();

                lstAuditDeptList = objInternalAuditModel.FunGetInternalDeptforAduitNum(sAuditNum, AuditDatetime);
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in GetAuditorDeptList: " + ex.ToString());
            }
            return Json(lstAuditDeptList);
        }

        [AllowAnonymous]
        public ActionResult GetAuditorAndDeptDetails(string sAuditNum, DateTime AuditDatetime, string sDeptName)
        {
            InternalAuditModels objInternalAuditModel = new InternalAuditModels();

            DataSet dsAuditdetails = objInternalAuditModel.FunGetInternalAduitDetails(sAuditNum, AuditDatetime, sDeptName);
            string sISO = "", sFirstName = "", sauditid = "", sISOId = "", sEmpdIds = "";
            try
            {
                if (dsAuditdetails.Tables.Count > 0 && dsAuditdetails.Tables[0].Rows.Count > 0)
                {
                    sFirstName = objGlobaldata.GetMultiHrEmpNameById(dsAuditdetails.Tables[0].Rows[0]["auditor"].ToString());
                    sEmpdIds = dsAuditdetails.Tables[0].Rows[0]["auditor"].ToString();
                    sISO = objGlobaldata.GetISONameById(dsAuditdetails.Tables[0].Rows[0]["auditcriteria"].ToString());
                    sISOId = dsAuditdetails.Tables[0].Rows[0]["auditcriteria"].ToString();
                    sauditid = dsAuditdetails.Tables[0].Rows[0]["auditid"].ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in GetAuditorAndDeptDetails: " + ex.ToString());
            }
            return Json(new { ISO = sISO, ISOID = sISOId, FirstName = sFirstName, EmpIds = sEmpdIds, auditid = sauditid }, JsonRequestBehavior.AllowGet);
        }

        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddInternalAuditNoFindingsLog(InternalAuditFindingsLog objInterAuditFindingsLog, FormCollection form, HttpPostedFileBase fileUploader)
        //{
        //    ////if (ModelState.IsValid)
        //    //if (objInterAuditFindingsLog != null)
        //    //{
        //    // string sAuditNumList = form["AuditNumList"];
        //    string sAuditTransID = form["AuditTransID"];
        //    string sAuditNum = form["AuditNum"];
        //    InternalAuditFindingsLogModelsList objInternalAuditFindingsLogModelsList = new InternalAuditFindingsLogModelsList();
        //    objInternalAuditFindingsLogModelsList.InternalAuditFindingsLogList = new List<InternalAuditFindingsLog>();

        //    try
        //    {
        //        if (form["itemcnt"] != null)
        //        {
        //            for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
        //            {
        //                InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

        //                //DateTime dateValue;
        //                //if (DateTime.TryParse(form["CorrectionDate" + i], out dateValue) == true)
        //                //{
        //                //    objInternalAuditFindingsLog.CorrectionDate = dateValue;
        //                //}

        //                //if (DateTime.TryParse(form["CorrectiveActionDate" + i], out dateValue) == true)
        //                //{
        //                //    objInternalAuditFindingsLog.CorrectiveActionDate = dateValue;
        //                //}

        //                //if (DateTime.TryParse(form["Followupdate" + i], out dateValue) == true)
        //                //{
        //                //    objInternalAuditFindingsLog.Followupdate = dateValue;
        //                //}
        //                //  string sAuditConductedBy = form["EmpIds"];
        //                string sReviewed_by = form["Reviewed_by" + i];
        //                // string sDeptId = form["AuditDepartment"];
        //                string sIsoStdId = form["ISOstandardID" + i];

        //                objInternalAuditFindingsLog.AuditTransID = sAuditTransID;
        //                //objInterAuditFindingsLog.AuditConductedBy = sAuditConductedBy;
        //                //objInternalAuditFindingsLog.NCRNo = form["NCRNo" + i];
        //                objInternalAuditFindingsLog.NCRDesc = form["NCRDesc" + i];
        //                objInternalAuditFindingsLog.ISO_standard_clause = form["ISO_standard_clause" + i];
        //                objInternalAuditFindingsLog.Reviewed_by = sReviewed_by;
        //                objInternalAuditFindingsLog.ISOstandardID = sIsoStdId;
        //                objInternalAuditFindingsLog.FindingCategory = form["FindingCategory" + i];
        //                objInternalAuditFindingsLog.AuditNum = sAuditNum;
        //                //objInterAuditFindingsLog.DeptId = sDeptId;
        //                //objInternalAuditFindingsLog.ReportStatus = form["ReportStatus" + i];

        //                if (fileUploader != null && fileUploader.ContentLength > 0)
        //                {
        //                    try
        //                    {
        //                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(fileUploader.FileName));
        //                        string sFilename = objInternalAuditFindingsLog.AuditNum + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
        //                        string sFilepath = Path.GetDirectoryName(spath);

        //                        fileUploader.SaveAs(sFilepath + "/" + sFilename);
        //                        objInternalAuditFindingsLog.Checklist = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
        //                        ViewBag.Message = "File uploaded successfully";
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                        objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditNoFindingsLog: " + ex.ToString());
        //                    }
        //                }
        //                else
        //                {
        //                    ViewBag.Message = "You have not specified a file.";
        //                }
        //                objInternalAuditFindingsLogModelsList.InternalAuditFindingsLogList.Add(objInternalAuditFindingsLog);
        //            }


        //            //InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

        //            if (objInterAuditFindingsLog.FunAddInternalAuditNoFindingsLog(objInternalAuditFindingsLogModelsList))
        //            {

        //                TempData["Successdata"] = "Audit findings Log updated successfully";
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                       
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditNoFindingsLog: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return RedirectToAction("InternalAuditFindingsLogList", new { AuditTransID = sAuditTransID, AuditNum = sAuditNum });
        //    //}            
        //}
        //// POST: /Audit/AddInternalAuditFindingsLog

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult AddInternalAuditFindingsLog(InternalAuditFindingsLog objInterAuditFindingsLog, FormCollection form, HttpPostedFileBase fileUploader)
        {
            ////if (ModelState.IsValid)
            //if (objInterAuditFindingsLog != null)
            //{
            // string sAuditNumList = form["AuditNumList"];
            string sAuditTransID = form["AuditTransID"];
            string sAuditNum = form["AuditNum"];
            string sAuditID = form["AuditID"];
            InternalAuditFindingsLogModelsList objInternalAuditFindingsLogModelsList = new InternalAuditFindingsLogModelsList();
            objInternalAuditFindingsLogModelsList.InternalAuditFindingsLogList = new List<InternalAuditFindingsLog>();

            try
            {
                if (form["itemcnt"] != null)
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

                        DateTime dateValue;
                        if (DateTime.TryParse(form["CorrectionDate" + i], out dateValue) == true)
                        {
                            objInternalAuditFindingsLog.CorrectionDate = dateValue;
                        }

                        if (DateTime.TryParse(form["CorrectiveActionDate" + i], out dateValue) == true)
                        {
                            objInternalAuditFindingsLog.CorrectiveActionDate = dateValue;
                        }

                        if (DateTime.TryParse(form["Followupdate" + i], out dateValue) == true)
                        {
                            objInternalAuditFindingsLog.Followupdate = dateValue;
                        }
                        //  string sAuditConductedBy = form["EmpIds"];
                        string sReviewed_by = form["Reviewed_by" + i];
                        // string sDeptId = form["AuditDepartment"];
                        string sIsoStdId = form["ISOstandardID" + i];

                        objInternalAuditFindingsLog.AuditTransID = sAuditTransID;
                        //objInterAuditFindingsLog.AuditConductedBy = sAuditConductedBy;
                        objInternalAuditFindingsLog.NCRNo = form["NCRNo" + i];
                        objInternalAuditFindingsLog.NCRDesc = form["NCRDesc" + i];
                        objInternalAuditFindingsLog.ISO_standard_clause = form["ISO_standard_clause" + i];
                        objInternalAuditFindingsLog.Reviewed_by = sReviewed_by;
                        objInternalAuditFindingsLog.ISOstandardID = sIsoStdId;
                        objInternalAuditFindingsLog.FindingCategory = form["FindingCategory" + i];
                        objInternalAuditFindingsLog.AuditNum = sAuditNum;
                        //objInterAuditFindingsLog.DeptId = sDeptId;
                        objInternalAuditFindingsLog.ReportStatus = form["ReportStatus" + i];
                        objInternalAuditFindingsLog.Audit_Details = form["Audit_Details" + i];


                        if (fileUploader != null && fileUploader.ContentLength > 0)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(fileUploader.FileName));
                                string sFilename = objInternalAuditFindingsLog.AuditNum + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                                string sFilepath = Path.GetDirectoryName(spath);

                                fileUploader.SaveAs(sFilepath + "/" + sFilename);
                                objInternalAuditFindingsLog.Checklist = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                                ViewBag.Message = "File uploaded successfully";
                            }
                            catch (Exception ex)
                            {
                                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                            }
                        }
                        else
                        {
                            ViewBag.Message = "You have not specified a file.";
                        }

                        objInternalAuditFindingsLogModelsList.InternalAuditFindingsLogList.Add(objInternalAuditFindingsLog);
                    }


                    //InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

                    objInterAuditFindingsLog.FunAddInternalAuditFindingsLog(objInternalAuditFindingsLogModelsList, fileUploader);
                }

                TempData["Successdata"] = "Audit findings Log updated successfully";
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditFindingsLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InternalAuditFindingsLogList", new { AuditTransID = sAuditTransID, AuditNum = sAuditNum, AuditId=sAuditID });
            //}            
        }

        //
        // GET: /Audit/InternalAuditFindingsLogList

        
        [AllowAnonymous]
        public ActionResult InternalAuditFindingsLogList()
        {
            ViewBag.SubMenutype = "InternalAudit";
            InternalAuditFindingsLogModelsList objInternalAuditList = new InternalAuditFindingsLogModelsList();
            objInternalAuditList.InternalAuditFindingsLogList = new List<InternalAuditFindingsLog>();
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            try
            {
                string sAuditTransID = Request.QueryString["AuditTransID"];
                string sAuditNum = Request.QueryString["AuditNum"];
                ViewBag.AuditID = Request.QueryString["AuditID"];
                InternalAuditModels obj = new InternalAuditModels();

                
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                

                string sSqlstmt = "SELECT AuditFindingID, AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                        + "ReportStatus, ReportCloseDate, Reviewed_by, FindingCategory, CorrectiveActionDate, Followupdate, Correctiontaken, Resultsofinvestigation,"
                        + " CorrectiveAction, PreventiveAction, VerificationofImplementation,Checklist,Audit_Details from t_auditfindings where AuditTransID='" + sAuditTransID
                        + "' order by AuditFindingID desc";
                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInternalAuditList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsInternalAuditList.Tables[0].Rows.Count; i++)
                    {
                        //DateTime dtCorrectionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["CorrectionDate"].ToString());
                        //DateTime dtFollowupDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["FollowupDate"].ToString());
                        //DateTime dtReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString());

                        InternalAuditFindingsLog objInterAudit = new InternalAuditFindingsLog
                        {
                            AuditTransID = dsInternalAuditList.Tables[0].Rows[i]["AuditTransID"].ToString(),
                            AuditFindingID = dsInternalAuditList.Tables[0].Rows[i]["AuditFindingID"].ToString(),
                            //CorrectionDate = dtCorrectionDate,
                            NCRNo = dsInternalAuditList.Tables[0].Rows[i]["NCR_Num"].ToString(),
                            NCRDesc = dsInternalAuditList.Tables[0].Rows[i]["NCRDesc"].ToString(),
                            ISOstandardID = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[i]["ISO_standard_ID"].ToString()),
                            ISO_standard_clause =objMgmtDocuments.GetMultiISOClauseNameById( dsInternalAuditList.Tables[0].Rows[i]["ISO_standard_clause"].ToString()),
                            //FollowupDate = dtFollowupDate,
                            // ReportCloseDate = dtReportCloseDate,
                            Reviewed_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["Reviewed_by"].ToString()),
                            //AuditConductedBy = objGlobaldata.GetMultiHrEmpNameById(dsInternalAuditList.Tables[0].Rows[i]["AuditConductedBy"].ToString()),
                            FindingCategory = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["FindingCategory"].ToString()),
                            ReportStatus = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["ReportStatus"].ToString()),
                            Checklist = dsInternalAuditList.Tables[0].Rows[i]["Checklist"].ToString(),
                            Audit_Details = dsInternalAuditList.Tables[0].Rows[i]["Audit_Details"].ToString(),

                        };
                        //if ( objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["FindingCategory"].ToString()) == "No findings")
                        //{
                        //    ViewBag.Category = "Yes";
                        //}
                        //else
                        //    ViewBag.Category = "No";
                        //objInterAudit.Finding = ViewBag.Category;
                        if (dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString() != "" && dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString() != null)
                        {
                            objInterAudit.ReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString());
                        }
                        objInternalAuditList.InternalAuditFindingsLogList.Add(objInterAudit);
                    }
                }


                DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria,DeptID,Auditee,Auditor from t_internal_audit a, t_internal_audit_trans b where"
                    + " a.AuditID=b.AuditID and AuditTransID='" + sAuditTransID + "'");
                if(dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
                    ViewBag.DeptID = objGlobaldata.GetDeptNameById(dsIso.Tables[0].Rows[0]["DeptID"].ToString());
                    ViewBag.Auditee = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditee"].ToString());
                    ViewBag.Auditor = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditor"].ToString());
                }

                ViewBag.AuditNum = sAuditNum;
                ViewBag.sAuditTransID = sAuditTransID;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInternalAuditList.InternalAuditFindingsLogList.ToList());
            //return View();
        }


      
        
        public ActionResult InternalAuditFindingsLogDetails(InternalAuditFindingsLog objInterAuditFindingsLog)
        {
            ViewBag.SubMenutype = "InternalAudit";
            InternalAuditFindingsLog objInterAudit = new InternalAuditFindingsLog();
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            try
            {
                //objGlobaldata.CreateUserSession();
                string sAuditFindingID = Request.QueryString["AuditFindingID"];
                string sAuditTransID = Request.QueryString["AuditTransID"];
                string sAuditNum = Request.QueryString["AuditNum"];

                if (sAuditFindingID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                InternalAuditModels objAudit = new InternalAuditModels();
               

                string sSqlstmt = "SELECT AuditFindingID,  AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                                    + "ReportStatus, ReportCloseDate, Reviewed_by, FindingCategory, CorrectiveActionDate, Followupdate, Correctiontaken, Resultsofinvestigation,"
                                    + " CorrectiveAction, PreventiveAction, VerificationofImplementation from t_auditfindings where AuditFindingID='" + sAuditFindingID + "'";
                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtCorrectionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectionDate"].ToString());
                    DateTime dtFollowupDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["FollowupDate"].ToString());
                    DateTime dtCorrectiveActionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectiveActionDate"].ToString());

                    objInterAudit = new InternalAuditFindingsLog
                    {
                        AuditTransID = dsInternalAuditList.Tables[0].Rows[0]["AuditTransID"].ToString(),
                        AuditFindingID = dsInternalAuditList.Tables[0].Rows[0]["AuditFindingID"].ToString(),
                        CorrectionDate = dtCorrectionDate,
                        //Reference = dsInternalAuditList.Tables[0].Rows[0]["Reference"].ToString(),
                        //FindingDescription = dsInternalAuditList.Tables[0].Rows[0]["FindingDescription"].ToString(),
                        NCRNo = dsInternalAuditList.Tables[0].Rows[0]["NCR_Num"].ToString(),
                        NCRDesc = dsInternalAuditList.Tables[0].Rows[0]["NCRDesc"].ToString(),
                        ISO_standard_clause =objMgmtDocuments.GetMultiISOClauseNameById(  dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_clause"].ToString()),

                        Reviewed_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[0]["Reviewed_by"].ToString()),
                        // AuditConductedBy = objGlobaldata.GetMultiHrEmpNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditConductedBy"].ToString()),
                        FindingCategory = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["FindingCategory"].ToString()),
                        ISOstandardID = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_ID"].ToString()),
                        ReportStatus = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["ReportStatus"].ToString()),
                        Followupdate = dtFollowupDate,
                        CorrectiveActionDate = dtCorrectiveActionDate,
                        Correctiontaken = dsInternalAuditList.Tables[0].Rows[0]["Correctiontaken"].ToString(),
                        Resultsofinvestigation = dsInternalAuditList.Tables[0].Rows[0]["Resultsofinvestigation"].ToString(),
                        CorrectiveAction = dsInternalAuditList.Tables[0].Rows[0]["CorrectiveAction"].ToString(),
                        PreventiveAction = dsInternalAuditList.Tables[0].Rows[0]["PreventiveAction"].ToString(),
                        VerificationofImplementation = dsInternalAuditList.Tables[0].Rows[0]["VerificationofImplementation"].ToString()
                    };
                    if (dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != "" && dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != null)
                    {
                        objInterAudit.ReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString());
                    }
                    ViewBag.AuditFindingID = objInterAudit.AuditFindingID;
                }

                ViewBag.AuditTransID = sAuditTransID;
                ViewBag.AuditNum = sAuditNum;
                ViewBag.AuditID = Request.QueryString["AuditID"];
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInterAudit);
        }

        
        
        public ActionResult InternalAuditFindingsLogEdit1(InternalAuditFindingsLog objInterAuditFindingsLog)
        {
            ViewBag.SubMenutype = "InternalAudit";
            InternalAuditFindingsLog objInterAudit = new InternalAuditFindingsLog();
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();
            try
            {
                //objGlobaldata.CreateUserSession();
                string sAuditFindingID = Request.QueryString["AuditFindingID"];
                InternalAuditModels objAud = new InternalAuditModels();
                if (sAuditFindingID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
               

                string sSqlstmt = "SELECT AuditFindingID, AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                                + " ReportStatus, ReportCloseDate, Reviewed_by, FindingCategory, CorrectiveActionDate, Followupdate, Correctiontaken, Resultsofinvestigation,"
                                + " CorrectiveAction, PreventiveAction, VerificationofImplementation,Checklist,Audit_Details from t_auditfindings where AuditFindingID='" + sAuditFindingID + "'";
                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtCorrectionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectionDate"].ToString());
                    DateTime dtFollowupDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["FollowupDate"].ToString());
                    DateTime dtCorrectiveActionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectiveActionDate"].ToString());

                    objInterAudit = new InternalAuditFindingsLog
                    {
                        AuditTransID = dsInternalAuditList.Tables[0].Rows[0]["AuditTransID"].ToString(),
                        AuditFindingID = dsInternalAuditList.Tables[0].Rows[0]["AuditFindingID"].ToString(),
                        CorrectionDate = dtCorrectionDate,
                        NCRNo = dsInternalAuditList.Tables[0].Rows[0]["NCR_Num"].ToString(),
                        NCRDesc = dsInternalAuditList.Tables[0].Rows[0]["NCRDesc"].ToString(),
                        ISO_standard_clause = objMgmtDocumentsModels.GetMultiISOClauseNameById(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_clause"].ToString()),
                        Reviewed_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[0]["Reviewed_by"].ToString()),
                        //AuditConductedBy = objGlobaldata.GetMultiHrEmpNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditConductedBy"].ToString()),
                        FindingCategory = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["FindingCategory"].ToString()),
                        ISOstandardID = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_ID"].ToString()),
                        ReportStatus =  objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["ReportStatus"].ToString()),
                        Followupdate = dtFollowupDate,
                        CorrectiveActionDate = dtCorrectiveActionDate,
                        Correctiontaken = dsInternalAuditList.Tables[0].Rows[0]["Correctiontaken"].ToString(),
                        Resultsofinvestigation = dsInternalAuditList.Tables[0].Rows[0]["Resultsofinvestigation"].ToString(),
                        CorrectiveAction = dsInternalAuditList.Tables[0].Rows[0]["CorrectiveAction"].ToString(),
                        PreventiveAction = dsInternalAuditList.Tables[0].Rows[0]["PreventiveAction"].ToString(),
                        VerificationofImplementation = dsInternalAuditList.Tables[0].Rows[0]["VerificationofImplementation"].ToString(),
                        Checklist = dsInternalAuditList.Tables[0].Rows[0]["Checklist"].ToString(),
                        Audit_Details = dsInternalAuditList.Tables[0].Rows[0]["Audit_Details"].ToString()
                    };
                    ViewBag.AppClauses = objMgmtDocumentsModels.GetMultiISOClauseList(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_ID"].ToString());
                    if (dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != "" && dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != null)
                    {
                        objInterAudit.ReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString());
                    }
                    ViewBag.AuditTransID = objInterAudit.AuditTransID;
                }
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.AuditNum = Request.QueryString["AuditNum"];
                ViewBag.AuditID = Request.QueryString["AuditID"];
                ViewBag.AuditDateList = GetAuditDateList(Request.QueryString["AuditNum"]);
                ViewBag.FindingCategory = objGlobaldata.GetDropdownList("Audit Finding Category");
                ViewBag.ReportStatus = objGlobaldata.GetDropdownList("Audit Status");

                DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria,DeptID,Auditee,Auditor from t_internal_audit a, t_internal_audit_trans b where"
                    + " a.AuditID=b.AuditID and AuditTransID='" + objInterAudit.AuditTransID + "'");
                if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
                    ViewBag.DeptID = objGlobaldata.GetDeptNameById(dsIso.Tables[0].Rows[0]["DeptID"].ToString());
                    ViewBag.Auditee = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditee"].ToString());
                    ViewBag.Auditor = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditor"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInterAudit);

        }

       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditFindingsLogEdit1(InternalAuditFindingsLog objInterAuditFindingsLog, FormCollection form, HttpPostedFileBase file)
        {
            ViewBag.SubMenutype = "InternalAudit";
            string sAuditTransID = form["AuditTransID"];
            string sAuditNum = form["AuditNum"];
            string sAuditID = form["AuditID"];
            string sAuditFindingID = form["AuditFindingID"];
            try
            {
                string sAuditNumList = form["AuditNumList"];
                            
                //string sAuditConductedBy = form["EmpIds"];
                string sReviewed_by = form["Reviewed_by"];
                //string sDeptId = form["AuditDepartment"];
                string sIsoStdId = form["ISOstandardID"];

                DateTime dateValue;
                if (DateTime.TryParse(form["CorrectionDate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.CorrectionDate = dateValue;
                }
                if (DateTime.TryParse(form["CorrectiveActionDate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.CorrectiveActionDate = dateValue;
                }

                if (DateTime.TryParse(form["Followupdate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.Followupdate = dateValue;
                }

                objInterAuditFindingsLog.AuditFindingID = sAuditFindingID;
                // objInterAuditFindingsLog.AuditConductedBy = sAuditConductedBy;
                objInterAuditFindingsLog.Reviewed_by = sReviewed_by;
                objInterAuditFindingsLog.ISOstandardID = sIsoStdId;
                objInterAuditFindingsLog.FindingCategory = form["FindingCategory"];
                //objInterAuditFindingsLog.AuditNum = sAuditNum;
                //objInterAuditFindingsLog.DeptId = sDeptId;
                objInterAuditFindingsLog.ReportStatus = form["ReportStatus"];
                objInterAuditFindingsLog.ISO_standard_clause = form["ISO_standard_clause"];
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                        string sFilename = objInterAuditFindingsLog.AuditNum+ "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objInterAuditFindingsLog.Checklist = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

                if (objInternalAuditFindingsLog.FunUpdateInternalAuditFindingsLog1(objInterAuditFindingsLog))
                {
                    TempData["Successdata"] = "Inetrnal Audit Findings log updated successfully";
                }
                else
                {             
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];              
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InternalAuditFindingsLogList", new { AuditTransID = sAuditTransID, AuditNum = sAuditNum, AuditID = sAuditID });
        }


        [AllowAnonymous]
        public ActionResult InternalAuditFindingsLogList1()
        {
            ViewBag.SubMenutype = "InternalAudit";
            InternalAuditFindingsLogModelsList objInternalAuditList = new InternalAuditFindingsLogModelsList();
            objInternalAuditList.InternalAuditFindingsLogList = new List<InternalAuditFindingsLog>();
            MgmtDocumentsModels objMgmtDocuments = new MgmtDocumentsModels();
            try
            {
                string sAuditTransID = Request.QueryString["AuditTransID"];
                string sAuditNum = Request.QueryString["AuditNum"];
                InternalAuditModels obj = new InternalAuditModels();


                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


                string sSqlstmt = "SELECT AuditFindingID, AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                        + "ReportStatus, ReportCloseDate, Reviewed_by, FindingCategory, CorrectiveActionDate, Followupdate, Correctiontaken, Resultsofinvestigation,"
                        + " CorrectiveAction, PreventiveAction, VerificationofImplementation,Checklist,Audit_Details from t_auditfindings where AuditTransID='" + sAuditTransID
                        + "' order by AuditFindingID desc";
                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInternalAuditList.Tables.Count > 0)
                {
                    for (int i = 0; i < dsInternalAuditList.Tables[0].Rows.Count; i++)
                    {
                        DateTime dtCorrectionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["CorrectionDate"].ToString());
                        DateTime dtFollowupDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["FollowupDate"].ToString());
                        //DateTime dtReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString());

                        InternalAuditFindingsLog objInterAudit = new InternalAuditFindingsLog
                        {
                            AuditTransID = dsInternalAuditList.Tables[0].Rows[i]["AuditTransID"].ToString(),
                            AuditFindingID = dsInternalAuditList.Tables[0].Rows[i]["AuditFindingID"].ToString(),
                            CorrectionDate = dtCorrectionDate,
                            NCRNo = dsInternalAuditList.Tables[0].Rows[i]["NCR_Num"].ToString(),
                            NCRDesc = dsInternalAuditList.Tables[0].Rows[i]["NCRDesc"].ToString(),
                            ISOstandardID = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[i]["ISO_standard_ID"].ToString()),
                            ISO_standard_clause = objMgmtDocuments.GetMultiISOClauseNameById(dsInternalAuditList.Tables[0].Rows[i]["ISO_standard_clause"].ToString()),
                            Followupdate = dtFollowupDate,
                            // ReportCloseDate = dtReportCloseDate,
                            Reviewed_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[i]["Reviewed_by"].ToString()),
                            //AuditConductedBy = objGlobaldata.GetMultiHrEmpNameById(dsInternalAuditList.Tables[0].Rows[i]["AuditConductedBy"].ToString()),
                            FindingCategory = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["FindingCategory"].ToString()),
                            ReportStatus = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["ReportStatus"].ToString()),
                            Correctiontaken = dsInternalAuditList.Tables[0].Rows[i]["Correctiontaken"].ToString(),
                            Resultsofinvestigation = dsInternalAuditList.Tables[0].Rows[i]["Resultsofinvestigation"].ToString(),
                            CorrectiveAction = dsInternalAuditList.Tables[0].Rows[i]["CorrectiveAction"].ToString(),
                            PreventiveAction = dsInternalAuditList.Tables[0].Rows[i]["PreventiveAction"].ToString(),
                            VerificationofImplementation = dsInternalAuditList.Tables[0].Rows[i]["VerificationofImplementation"].ToString(),

                        };
                        //if ( objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[i]["FindingCategory"].ToString()) == "No findings")
                        //{
                        //    ViewBag.Category = "Yes";
                        //}
                        //else
                        //    ViewBag.Category = "No";
                        //objInterAudit.Finding = ViewBag.Category;
                        if (dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString() != "" && dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString() != null)
                        {
                            objInterAudit.ReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[i]["ReportCloseDate"].ToString());
                        }
                        objInternalAuditList.InternalAuditFindingsLogList.Add(objInterAudit);
                    }
                }


                ViewBag.AuditNum = sAuditNum;
                ViewBag.sAuditTransID = sAuditTransID;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogList1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInternalAuditList.InternalAuditFindingsLogList.ToList());
            //return View();
        }



        public ActionResult InternalAuditFindingsLogEdit(InternalAuditFindingsLog objInterAuditFindingsLog)
        {
            InternalAuditFindingsLog objInterAudit = new InternalAuditFindingsLog();
            MgmtDocumentsModels objMgmtDocumentsModels = new MgmtDocumentsModels();
            ViewBag.SubMenutype = "InternalAudit";
            try
            {
                //objGlobaldata.CreateUserSession();
                string sAuditFindingID = Request.QueryString["AuditFindingID"];
                InternalAuditModels objAud = new InternalAuditModels();
                if (sAuditFindingID == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];


                string sSqlstmt = "SELECT AuditFindingID, AuditTransID, NCR_Num, NCRDesc, ISO_standard_ID, ISO_standard_clause, CorrectionDate,"
                                + " ReportStatus, ReportCloseDate, Reviewed_by, FindingCategory, CorrectiveActionDate, Followupdate, Correctiontaken, Resultsofinvestigation,"
                                + " CorrectiveAction, PreventiveAction, VerificationofImplementation,Checklist,Audit_Details from t_auditfindings where AuditFindingID='" + sAuditFindingID + "'";
                DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                    DateTime dtCorrectionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectionDate"].ToString());
                    DateTime dtFollowupDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["FollowupDate"].ToString());
                    DateTime dtCorrectiveActionDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["CorrectiveActionDate"].ToString());

                    objInterAudit = new InternalAuditFindingsLog
                    {
                        AuditTransID = dsInternalAuditList.Tables[0].Rows[0]["AuditTransID"].ToString(),
                        AuditFindingID = dsInternalAuditList.Tables[0].Rows[0]["AuditFindingID"].ToString(),
                        CorrectionDate = dtCorrectionDate,
                        NCRNo = dsInternalAuditList.Tables[0].Rows[0]["NCR_Num"].ToString(),
                        NCRDesc = dsInternalAuditList.Tables[0].Rows[0]["NCRDesc"].ToString(),
                        ISO_standard_clause = (dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_clause"].ToString()),
                        Reviewed_by = objGlobaldata.GetEmpHrNameById(dsInternalAuditList.Tables[0].Rows[0]["Reviewed_by"].ToString()),
                        //AuditConductedBy = objGlobaldata.GetMultiHrEmpNameById(dsInternalAuditList.Tables[0].Rows[0]["AuditConductedBy"].ToString()),
                        FindingCategory = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["FindingCategory"].ToString()),
                        ISOstandardID = objGlobaldata.GetIsoStdDescriptionById(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_ID"].ToString()),
                        ReportStatus = objGlobaldata.GetDropdownitemById(dsInternalAuditList.Tables[0].Rows[0]["ReportStatus"].ToString()),
                        Followupdate = dtFollowupDate,
                        CorrectiveActionDate = dtCorrectiveActionDate,
                        Correctiontaken = dsInternalAuditList.Tables[0].Rows[0]["Correctiontaken"].ToString(),
                        Resultsofinvestigation = dsInternalAuditList.Tables[0].Rows[0]["Resultsofinvestigation"].ToString(),
                        CorrectiveAction = dsInternalAuditList.Tables[0].Rows[0]["CorrectiveAction"].ToString(),
                        PreventiveAction = dsInternalAuditList.Tables[0].Rows[0]["PreventiveAction"].ToString(),
                        VerificationofImplementation = dsInternalAuditList.Tables[0].Rows[0]["VerificationofImplementation"].ToString(),
                        Checklist = dsInternalAuditList.Tables[0].Rows[0]["Checklist"].ToString(),
                        Audit_Details = dsInternalAuditList.Tables[0].Rows[0]["Audit_Details"].ToString()
                    };
                    ViewBag.AppClauses = objMgmtDocumentsModels.GetMultiISOClauseList(dsInternalAuditList.Tables[0].Rows[0]["ISO_standard_ID"].ToString());
                    if (dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != "" && dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString() != null)
                    {
                        objInterAudit.ReportCloseDate = Convert.ToDateTime(dsInternalAuditList.Tables[0].Rows[0]["ReportCloseDate"].ToString());
                    }
                    ViewBag.AuditTransID = objInterAudit.AuditTransID;
                }
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.AuditNum = Request.QueryString["AuditNum"];
                ViewBag.AuditID = objAud.GetAuditIDbyAuditNum(Request.QueryString["AuditNum"]);
                ViewBag.AuditDateList = GetAuditDateList(Request.QueryString["AuditNum"]);
                ViewBag.FindingCategory = objGlobaldata.GetDropdownList("Audit Finding Category");
                ViewBag.ReportStatus = objGlobaldata.GetDropdownList("Audit Status");

                //DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria from t_internal_audit, t_internal_audit_trans where"
                //    + " t_internal_audit.AuditID=t_internal_audit_trans.AuditID and AuditTransID='" + objInterAudit.AuditTransID + "'");
                //ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());

                DataSet dsIso = objGlobaldata.Getdetails("select AuditCriteria,DeptID,Auditee,Auditor from t_internal_audit a, t_internal_audit_trans b where"
                   + " a.AuditID=b.AuditID and AuditTransID='" + objInterAudit.AuditTransID + "'");
                if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
                    ViewBag.DeptID = objGlobaldata.GetDeptNameById(dsIso.Tables[0].Rows[0]["DeptID"].ToString());
                    ViewBag.Auditee = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditee"].ToString());
                    ViewBag.Auditor = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditor"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInterAudit);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult InternalAuditFindingsLogEdit(InternalAuditFindingsLog objInterAuditFindingsLog, FormCollection form, HttpPostedFileBase file)
        {
            string sAuditTransID = form["AuditTransID"];
            string sAuditNum = form["AuditNum"];
            string sAuditId = form["AuditId"];
            ViewBag.SubMenutype = "InternalAudit";
            try
            {
                string sAuditNumList = form["AuditNumList"];
                string sAuditFindingID = form["AuditFindingID"];
                //string sAuditConductedBy = form["EmpIds"];
                string sReviewed_by = form["Reviewed_by"];
                //string sDeptId = form["AuditDepartment"];
                string sIsoStdId = form["ISOstandardID"];

                DateTime dateValue;
                if (DateTime.TryParse(form["CorrectionDate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.CorrectionDate = dateValue;
                }
                if (DateTime.TryParse(form["CorrectiveActionDate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.CorrectiveActionDate = dateValue;
                }

                if (DateTime.TryParse(form["Followupdate"], out dateValue) == true)
                {
                    objInterAuditFindingsLog.Followupdate = dateValue;
                }

                objInterAuditFindingsLog.AuditFindingID = sAuditFindingID;
                // objInterAuditFindingsLog.AuditConductedBy = sAuditConductedBy;
                objInterAuditFindingsLog.Reviewed_by = sReviewed_by;
                objInterAuditFindingsLog.ISOstandardID = sIsoStdId;
                objInterAuditFindingsLog.FindingCategory = form["FindingCategory"];
                //objInterAuditFindingsLog.AuditNum = sAuditNum;
                //objInterAuditFindingsLog.DeptId = sDeptId;
                objInterAuditFindingsLog.ReportStatus = form["ReportStatus"];
                objInterAuditFindingsLog.ISO_standard_clause = form["ISO_standard_clause"];
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                        string sFilename = objInterAuditFindingsLog.AuditNum + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objInterAuditFindingsLog.Checklist = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                InternalAuditFindingsLog objInternalAuditFindingsLog = new InternalAuditFindingsLog();

                if (objInternalAuditFindingsLog.FunUpdateInternalAuditFindingsLog(objInterAuditFindingsLog))
                {
                    TempData["Successdata"] = "Inetrnal Audit Findings log updated successfully";
                }
                else
                {

                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditFindingsLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InternalAuditFindingsLogList", new { AuditTransID = sAuditTransID, AuditNum = sAuditNum , AuditId = sAuditId});
        }
        
        [AllowAnonymous]
        public ActionResult InternalAuditChecklist(string branch_name)
        {
            AuditElementsModels obj = new AuditElementsModels();
            ChecklistModelsList objInterAudit = new ChecklistModelsList();
            objInterAudit.CheckList=new List<InternalAuditChecklistModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentListbox();
            ViewBag.AuditNo = obj.GetAuditNoListbox();
            ViewBag.SubMenutype = "InternalAuditChecklist";
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select checklistId,checklistName,DocUploadPath,a.AuditNo,Department,b.AuditLocation" +
                    " from t_internalauditchecklist a,t_internal_audit b where Status=1 and b.AuditID=a.AuditNo";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', AuditLocation)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', AuditLocation)";
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsCheckList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCheckList.Tables.Count > 0 && dsCheckList.Tables[0].Rows.Count > 0)
                {   
                    for (int i = 0; i < dsCheckList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InternalAuditChecklistModels objChecklist = new InternalAuditChecklistModels
                            {
                                checklistId = (dsCheckList.Tables[0].Rows[i]["checklistId"].ToString()),
                                checklistName = dsCheckList.Tables[0].Rows[i]["checklistName"].ToString(),
                                DocUploadPath = dsCheckList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                AuditNo = objGlobaldata.GetAuditNoById(dsCheckList.Tables[0].Rows[i]["AuditNo"].ToString()),
                                Department = objGlobaldata.GetDeptNameById(dsCheckList.Tables[0].Rows[i]["Department"].ToString()),
                                Branch = objGlobaldata.GetAuditDivisionById(dsCheckList.Tables[0].Rows[i]["AuditNo"].ToString()),
                            };
                            objInterAudit.CheckList.Add(objChecklist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InternalAuditChecklist: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objInterAudit.CheckList.ToList());
        }
        
        [AllowAnonymous]
        public JsonResult InternalAuditChecklistSearch(string branch_name)
        {
            AuditElementsModels obj = new AuditElementsModels();
            ChecklistModelsList objInterAudit = new ChecklistModelsList();
            objInterAudit.CheckList = new List<InternalAuditChecklistModels>();
            ViewBag.Department = objGlobaldata.GetDepartmentListbox();
            ViewBag.AuditNo = obj.GetAuditNoListbox();
            ViewBag.SubMenutype = "InternalAuditChecklist";
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select checklistId,checklistName,DocUploadPath,a.AuditNo,Department,b.AuditLocation" +
                   " from t_internalauditchecklist a,t_internal_audit b where Status=1 and b.AuditID=a.AuditNo";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', AuditLocation)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', AuditLocation)";
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsCheckList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCheckList.Tables.Count > 0 && dsCheckList.Tables[0].Rows.Count > 0)
                {    
                    for (int i = 0; i < dsCheckList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            InternalAuditChecklistModels objChecklist = new InternalAuditChecklistModels
                            {
                                checklistId = (dsCheckList.Tables[0].Rows[i]["checklistId"].ToString()),
                                checklistName = dsCheckList.Tables[0].Rows[i]["checklistName"].ToString(),
                                DocUploadPath = dsCheckList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                AuditNo = objGlobaldata.GetAuditNoById(dsCheckList.Tables[0].Rows[i]["AuditNo"].ToString()),
                                Department = objGlobaldata.GetDeptNameById(dsCheckList.Tables[0].Rows[i]["Department"].ToString()),
                                Branch = objGlobaldata.GetAuditDivisionById(dsCheckList.Tables[0].Rows[i]["AuditNo"].ToString()),
                            };
                            objInterAudit.CheckList.Add(objChecklist);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in InternalAuditChecklistSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InternalAuditChecklistSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public ActionResult AddInternalAuditDetails()
        {
            string AuditID = Request.QueryString["AuditID"];
            string AuditTransID = Request.QueryString["AuditTransID"];
            //objGlobaldata.CreateUserSession();            
            InternalAuditModels objAudit = new InternalAuditModels();
            if (AuditID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            string sSqlstmt = "SELECT AuditDetails from t_internal_audit_trans where AuditTransID='" + AuditTransID + "'";

            DataSet dsInternalAuditList = objGlobaldata.Getdetails(sSqlstmt);

            try
            {
                
                if (dsInternalAuditList.Tables.Count > 0 && dsInternalAuditList.Tables[0].Rows.Count > 0)
                {
                    objAudit = new InternalAuditModels
                    {
                        AuditDetails = dsInternalAuditList.Tables[0].Rows[0]["AuditDetails"].ToString(),
                    };
                    
                }


                    DataSet dsIso = objGlobaldata.Getdetails("select AuditTransID,b.AuditID,AuditNum,AuditCriteria,DeptID,Auditee,Auditor,AuditDetails from t_internal_audit a, t_internal_audit_trans b where"
                     + " a.AuditID=b.AuditID and AuditTransID='" +AuditTransID + "'");
                    if (dsIso.Tables.Count > 0 && dsIso.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.AuditTransID = dsIso.Tables[0].Rows[0]["AuditTransID"].ToString();
                        ViewBag.AuditID = dsIso.Tables[0].Rows[0]["AuditID"].ToString();
                        ViewBag.AuditNum = dsIso.Tables[0].Rows[0]["AuditNum"].ToString();
                        ViewBag.IsoStdList = objGlobaldata.GetIsoStdListboxIn(dsIso.Tables[0].Rows[0]["AuditCriteria"].ToString());
                        ViewBag.DeptID = objGlobaldata.GetDeptNameById(dsIso.Tables[0].Rows[0]["DeptID"].ToString());
                        ViewBag.Auditee = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditee"].ToString());
                        ViewBag.Auditor = objGlobaldata.GetMultiHrEmpNameById(dsIso.Tables[0].Rows[0]["Auditor"].ToString());
                     }
                 }
        
                   catch (Exception ex)
                   {
                       objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditDetails: " + ex.ToString());
                       TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                   }
                  
                 return View(objAudit);
           }

        [HttpPost]
        [ValidateInput(false)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddInternalAuditDetails(InternalAuditModels objInterAudit, FormCollection form)
        {
            string sAuditTransID = form["AuditTransID"];
            string sAuditNum = form["AuditNum"];
            string sAuditId = form["AuditId"];
            try
            {
                string sAuditNumList = form["AuditNumList"];
                string sAuditFindingID = form["AuditFindingID"];
                //string sAuditConductedBy = form["EmpIds"];
                ///string sReviewed_by = form["Reviewed_by"];
                //string sDeptId = form["AuditDepartment"];
                //string sIsoStdId = form["ISOstandardID"];


                //objInterAudit.AuditFindingID = sAuditFindingID;
                //// objInterAudit.AuditConductedBy = sAuditConductedBy;
                //objInterAudit.Reviewed_by = sReviewed_by;
                //objInterAudit.ISOstandardID = sIsoStdId;
                //objInterAudit.FindingCategory = form["FindingCategory"];
                ////objInterAudit.AuditNum = sAuditNum;
                ////objInterAudit.DeptId = sDeptId;
                //objInterAudit.ReportStatus = form["ReportStatus"];
                //objInterAudit.ISO_standard_clause = form["ISO_standard_clause"];
               
              
                if (objInterAudit.FunUpdateAuditDetails(objInterAudit))
                {
                    TempData["Successdata"] = "Inetrnal Audit Details updated successfully";
                }
                else
                {

                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddInternalAuditDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("InternalAuditNumDetails", new { AuditTransID = sAuditTransID, AuditNum = sAuditNum, AuditId = sAuditId });
        }
                     
        [AllowAnonymous]
        public ActionResult AddChecklist(InternalAuditChecklistModels objChecklistModel, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                ViewBag.SubMenutype = "InternalAuditChecklist";

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                        string sFilename = objChecklistModel.checklistName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objChecklistModel.DocUploadPath = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        objGlobaldata.AddFunctionalLog("Exception in AddChecklist: " + ex.ToString());
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objChecklistModel.FunAddChecklist(objChecklistModel))
                {
                    TempData["Successdata"] = "Added Internal Audit check list details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddOrgChart: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("InternalAuditChecklist");
        }
                 
        [AllowAnonymous]
        public JsonResult ChecklistDelete(FormCollection form)
        {
            try
            {
               
                    
                        if (form["checklistId"] != null && form["checklistId"] != "")
                        {

                            InternalAuditChecklistModels checklist = new InternalAuditChecklistModels();
                            string schecklistId = form["checklistId"];


                            if (checklist.FunDeleteChecklist(schecklistId))
                            {
                                TempData["Successdata"] = "Checklist details deleted successfully";
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
                            TempData["alertdata"] = "Checklist Id cannot be Null or empty";
                            return Json("Failed");
                        }
                   
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChecklistDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
                
        [HttpPost]
        public JsonResult doesAuditNumExist(string AuditNum)
        {
            InternalAuditModels obj = new InternalAuditModels();
            var AuditNo = true;
            if (AuditNum != null)
            {
                AuditNo = obj.checkAuditNoExists(AuditNum);
            }

            return Json(AuditNo);
        }

        public ActionResult AuditApproveReject(string AuditID, int iStatus, string PendingFlg)
        {
            try
            {
                InternalAuditModels objModel = new InternalAuditModels();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().firstname;
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
                    sStatus = "Rescheduled";
                }
                if (objModel.FunAuditApprvReject(AuditID, iStatus))
                {
                    TempData["Successdata"] = "Audit " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
          
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
        }      

        public ActionResult FunGetReportStatus(string CategoryType)
        {
            InternalAuditModels obj = new InternalAuditModels();
            //MultiSelectList lstEmp = obj.GetReportStatus(CategoryType);
            MultiSelectList lstEmp = objGlobaldata.GetDropdownList("Audit Status");
            return Json(lstEmp);
        }

        public JsonResult AuditApproveRejectNoty(string AuditID, int iStatus, string PendingFlg)
        {
            try
            {
                InternalAuditModels objModel = new InternalAuditModels();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().firstname;

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
                    sStatus = "Rescheduled";
                }
                if (objModel.FunAuditApprvReject(AuditID, iStatus))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
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
