using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using PagedList;
using System.IO;
using PagedList.Mvc;
using iTextSharp.text.pdf.parser;
using ISOStd.Filters;
using Rotativa;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class ExternalAuditController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        ExternalAuditorModels objEAudit = new ExternalAuditorModels();
        CertificationBodyModels objCertBody = new CertificationBodyModels();
        public ExternalAuditController()
        {
            ViewBag.Menutype = "Audit";

        }

        public ActionResult Index()
        {
            return View();
        }
        //-------------------------------------------------------------------------------------
        [AllowAnonymous]
        public ActionResult AddExternalAudit()
        {
            ExternalAuditModels objAudit = new ExternalAuditModels();
            try
            {

                objAudit.branch = objGlobaldata.GetCurrentUserSession().division;
                objAudit.dept_name = objGlobaldata.GetCurrentUserSession().DeptID;
                // objAudit.team = objGlobaldata.GetCurrentUserSession().team;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objAudit.branch);
                //ViewBag.Team = objGlobaldata.GetMultiTeambyMultiGroup(objAudit.dept_name);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objAudit.branch);

                ViewBag.Level = objGlobaldata.GetAuidtorLevelList();
                ViewBag.Type = objGlobaldata.GetAuidtTypeList();
                ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();

                ViewBag.CompanyName = objGlobaldata.GetGlobalCompanyName();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddExternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAudit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExternalAudit(ExternalAuditModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objModel.notification_to = form["notification_to"];

                DateTime dateValue;
                objModel.audit_criteria = form["audit_criteria"];
                if (DateTime.TryParse(form["audit_start_date"], out dateValue) == true)
                {
                    objModel.audit_start_date = dateValue;
                }

                ExternalAuditModelsList objAudit = new ExternalAuditModelsList();
                objAudit.ExternalAuditList = new List<ExternalAuditModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    ExternalAuditModels obj = new ExternalAuditModels();
                    if (form["auditor_name" + i] != "" && form["auditor_name" + i] != null)
                    {
                        obj.auditor_name = form["auditor_name" + i];
                        obj.auditor_level = form["auditor_level" + i];
                        obj.contact_no = form["contact_no" + i];
                        obj.email_address = form["email_address" + i];
                        objAudit.ExternalAuditList.Add(obj);
                    }
                }
                ExternalAuditModelsList objAudit1 = new ExternalAuditModelsList();
                objAudit1.ExternalAuditList = new List<ExternalAuditModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    ExternalAuditModels objList = new ExternalAuditModels();
                    if (form["branch" + i] != "" && form["branch" + i] != null)
                    {
                        objList.branch = form["branch" + i];
                        objList.dept_name = form["dept_name" + i];
                        objList.team = form["team" + i];
                        objList.location = form["location" + i];
                        objAudit1.ExternalAuditList.Add(objList);
                    }
                }
                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "Ext" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddExternalAudit-upload: " + ex.ToString());
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objModel.FunAddExternalAudit(objModel, objAudit, objAudit1))
                {
                    TempData["Successdata"] = "External audit details Addded successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddExternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExternalAuditList");
        }


        [AllowAnonymous]
        public ActionResult ExternalAuditList(string branch_name)
        {
            ExternalAuditModelsList objExternalAuditModelsList = new ExternalAuditModelsList();
            objExternalAuditModelsList.ExternalAuditList = new List<ExternalAuditModels>();
            ViewBag.SubMenutype = "ExtAuditList";


            try
            {
                //string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                //string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                //ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select id_external_audit,audit_category,audit_no,audit_start_date,audit_type,"
                + "audit_criteria,entity_name,logged_by,audit_status,audit_status_date,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,audit_complete_date,notification_to,company_name from t_external_audit where active=1";

                //if (branch_name != null && branch_name != "")
                //{
                //    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                //    ViewBag.Branch_name = branch_name;
                //}
                //else
                //{
                //    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                //}

                sSqlstmt = sSqlstmt + " order by id_external_audit desc";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsAuditList.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        ExternalAuditModels objModels = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[i]["id_external_audit"].ToString(),
                            audit_category = dsAuditList.Tables[0].Rows[i]["audit_category"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[i]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsAuditList.Tables[0].Rows[i]["audit_type"].ToString()),
                            audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[i]["audit_criteria"].ToString()),
                            //branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[i]["branch"].ToString()),
                            //dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[i]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[i]["team"].ToString()),
                            //location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[i]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[i]["entity_name"].ToString(),
                            audit_status = objGlobaldata.GetAuditStatusById(dsAuditList.Tables[0].Rows[i]["audit_status"].ToString()),

                            notification_to = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[i]["notification_to"].ToString()),
                            company_name = dsAuditList.Tables[0].Rows[i]["company_name"].ToString(),
                            logged_by = dsAuditList.Tables[0].Rows[i]["logged_by"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsAuditList.Tables[0].Rows[i]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModels.audit_start_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[i]["audit_status_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["audit_status_date"].ToString(), out dtDocDate))
                        {
                            objModels.audit_status_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[i]["audit_complete_date"].ToString() != ""
                        && DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["audit_complete_date"].ToString(), out dtDocDate))
                        {
                            objModels.audit_complete_date = dtDocDate;
                        }
                        objExternalAuditModelsList.ExternalAuditList.Add(objModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in ExternalAuditList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objExternalAuditModelsList.ExternalAuditList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ExternalAuditDocDelete(FormCollection form)
        {
            try
            {

                if (form["Id"] != null && form["Id"] != "")
                {
                    ExternalAuditModels Doc = new ExternalAuditModels();
                    string sid_external_audit = form["Id"];


                    if (Doc.FunDeleteExternalDoc(sid_external_audit))
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
                    TempData["alertdata"] = "External Id cannot be Null or empty";
                    return Json("Failed");
                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditEdit()
        {

            ExternalAuditModels objModel = new ExternalAuditModels();

            try
            {

                if (Request.QueryString["id_external_audit"] != null && Request.QueryString["id_external_audit"] != "")
                {
                    string id_external_audit = Request.QueryString["id_external_audit"];

                    string sSqlstmt = "select id_external_audit,audit_category,audit_no,audit_start_date,audit_type,audit_criteria,upload,entity_name,audit_complete_date,notification_to  from t_external_audit where id_external_audit=" + id_external_audit;

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            audit_category = dsAuditList.Tables[0].Rows[0]["audit_category"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = (dsAuditList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_criteria = (dsAuditList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            //branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["branch"].ToString()),
                            //dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[0]["team"].ToString()),
                            //location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[0]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[0]["entity_name"].ToString(),
                            upload = dsAuditList.Tables[0].Rows[0]["upload"].ToString(),
                            notification_to = (dsAuditList.Tables[0].Rows[0]["notification_to"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_complete_date"].ToString(), out dtValue))
                        {
                            objModel.audit_complete_date = dtValue;
                        }
                        ViewBag.Level = objGlobaldata.GetAuidtorLevelList();
                        ViewBag.Type = objGlobaldata.GetAuidtTypeList();
                        ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                        objModel.branch = objGlobaldata.GetCurrentUserSession().division;
                        objModel.dept_name = objGlobaldata.GetCurrentUserSession().DeptID;
                        //objModel.team = objGlobaldata.GetCurrentUserSession().team;

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                        //ViewBag.Team = objGlobaldata.GetMultiTeambyMultiGroup();
                        ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.branch);
                        ExternalAuditModelsList objAudit = new ExternalAuditModelsList();
                        objAudit.ExternalAuditList = new List<ExternalAuditModels>();

                        sSqlstmt = "select id_external_auditor,id_external_audit,auditor_name,auditor_level,contact_no,email_address from t_external_auditor where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ExternalAuditModels objAuditModel = new ExternalAuditModels
                                    {
                                        id_external_auditor = dsList.Tables[0].Rows[i]["id_external_auditor"].ToString(),
                                        id_external_audit = dsList.Tables[0].Rows[i]["id_external_audit"].ToString(),
                                        auditor_name = dsList.Tables[0].Rows[i]["auditor_name"].ToString(),
                                        auditor_level = dsList.Tables[0].Rows[i]["auditor_level"].ToString(),
                                        contact_no = dsList.Tables[0].Rows[i]["contact_no"].ToString(),
                                        email_address = dsList.Tables[0].Rows[i]["email_address"].ToString(),
                                    };

                                    objAudit.ExternalAuditList.Add(objAuditModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("ExternalAuditList");
                                }
                            }
                            ViewBag.Auditor = objAudit;
                        }

                        // Auditee

                        ExternalAuditModelsList objAudit1 = new ExternalAuditModelsList();
                        objAudit1.ExternalAuditList = new List<ExternalAuditModels>();

                        sSqlstmt = "select id_external_auditee,id_external_audit,branch,dept_name,team,location from t_external_auditee where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList1 = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList1.Tables.Count > 0 && dsList1.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList1.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    ExternalAuditModels objAuditModel = new ExternalAuditModels
                                    {
                                        id_external_auditee = dsList1.Tables[0].Rows[i]["id_external_auditee"].ToString(),
                                        id_external_audit = dsList1.Tables[0].Rows[i]["id_external_audit"].ToString(),
                                        branch = dsList1.Tables[0].Rows[i]["branch"].ToString(),
                                        dept_name = dsList1.Tables[0].Rows[i]["dept_name"].ToString(),
                                        team = dsList1.Tables[0].Rows[i]["team"].ToString(),
                                        location = dsList1.Tables[0].Rows[i]["location"].ToString(),
                                    };

                                    objAudit1.ExternalAuditList.Add(objAuditModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("ExternalAuditList");
                                }
                            }
                            ViewBag.Auditee = objAudit1;
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalAuditEdit(ExternalAuditModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objModel.notification_to = form["notification_to"];
                DateTime dateValue;
                objModel.audit_criteria = form["audit_criteria"];
                if (DateTime.TryParse(form["audit_start_date"], out dateValue) == true)
                {
                    objModel.audit_start_date = dateValue;
                }

                ExternalAuditModelsList objAudit = new ExternalAuditModelsList();
                objAudit.ExternalAuditList = new List<ExternalAuditModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    ExternalAuditModels obj = new ExternalAuditModels();
                    if (form["auditor_name" + i] != "" && form["auditor_name" + i] != null)
                    {
                        obj.id_external_auditor = form["id_external_auditor" + i];
                        obj.auditor_name = form["auditor_name" + i];
                        obj.auditor_level = form["auditor_level" + i];
                        obj.contact_no = form["contact_no" + i];
                        obj.email_address = form["email_address" + i];
                        objAudit.ExternalAuditList.Add(obj);
                    }
                }
                ExternalAuditModelsList objAudit1 = new ExternalAuditModelsList();
                objAudit1.ExternalAuditList = new List<ExternalAuditModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    ExternalAuditModels objList = new ExternalAuditModels();
                    if (form["branch" + i] != "" && form["branch" + i] != null)
                    {
                        objList.id_external_auditee = form["id_external_auditee" + i];
                        objList.branch = form["branch" + i];
                        objList.dept_name = form["dept_name" + i];
                        objList.team = form["team" + i];
                        objList.location = form["location" + i];
                        objAudit1.ExternalAuditList.Add(objList);
                    }
                }
                IList<HttpPostedFileBase> upload_List = (IList<HttpPostedFileBase>)upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload_List[0] != null)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "Ext" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit-upload: " + ex.ToString());
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.upload = objModel.upload + "," + form["QCDocsVal"];
                    objModel.upload = objModel.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && upload_List[0] == null)
                {
                    objModel.upload = null;
                }
                else if (form["QCDocsVal"] == null && upload_List[0] == null)
                {
                    objModel.upload = null;
                }

                if (objModel.FunEditExternalAudit(objModel, objAudit, objAudit1))
                {
                    TempData["Successdata"] = "External audit details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExternalAuditList");
        }
        //Auditee List
        [AllowAnonymous]
        public ActionResult AuditeeListInfo(int id)
        {

            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select branch,dept_name,team,location from t_external_auditee where id_external_audit ='" + id + "'";
                    DataSet dsAuditee = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.objAuditee = dsAuditee;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditeeListInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Auditor List
        [AllowAnonymous]
        public ActionResult AuditorListInfo(int id)
        {

            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select auditor_name,auditor_level,contact_no,email_address from t_external_auditor where id_external_audit ='" + id + "'";
                    DataSet dsAuditor = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.objAudit = dsAuditor;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorListInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Audit Status    
        [AllowAnonymous]
        public ActionResult AuditStatusUpdate()
        {
            ExternalAuditModels objModel = new ExternalAuditModels();
            try
            {
                if (Request.QueryString["id_external_audit"] != null && Request.QueryString["id_external_audit"] != "")
                {
                    string id_external_audit = Request.QueryString["id_external_audit"];

                    string sSqlstmt = "select id_external_audit,audit_category,audit_no,audit_start_date,audit_type,audit_criteria,upload,entity_name,"
                        + "audit_status,remarks,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,status_upload,audit_status_date,audit_complete_date"
                        + " from t_external_audit where id_external_audit=" + id_external_audit;

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsAuditList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            //branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["branch"].ToString()),
                            //dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[0]["team"].ToString()),
                            //location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[0]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[0]["entity_name"].ToString(),
                            audit_status = dsAuditList.Tables[0].Rows[0]["audit_status"].ToString(),
                            remarks = dsAuditList.Tables[0].Rows[0]["remarks"].ToString(),
                            status_upload = dsAuditList.Tables[0].Rows[0]["status_upload"].ToString(),

                        };
                        if (dsAuditList.Tables[0].Rows[0]["major_nc"].ToString() != "")
                        {
                            objModel.major_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["major_nc"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["minor_nc"].ToString() != "")
                        {
                            objModel.minor_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["minor_nc"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_observation"].ToString() != "")
                        {
                            objModel.no_observation = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_observation"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_noteworthy"].ToString() != "")
                        {
                            objModel.no_noteworthy = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_noteworthy"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_ci"].ToString() != "")
                        {
                            objModel.no_ci = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_ci"].ToString());
                        }
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_complete_date"].ToString(), out dtValue))
                        {
                            objModel.audit_complete_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_status_date"].ToString(), out dtValue))
                        {
                            objModel.audit_status_date = dtValue;
                        }
                    }

                    ViewBag.AuditStatus = objGlobaldata.GetAuditStatusList();
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditStatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AuditStatusUpdate(FormCollection form, ExternalAuditModels obj, IEnumerable<HttpPostedFileBase> status_upload)
        {
            try
            {
                DateTime dateValue;


                IList<HttpPostedFileBase> upload_List = (IList<HttpPostedFileBase>)status_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload_List[0] != null)
                {
                    obj.status_upload = "";
                    foreach (var file in status_upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "Ext" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            obj.status_upload = obj.status_upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditStatusUpdate-upload: " + ex.ToString());
                        }
                    }
                    obj.status_upload = obj.status_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    obj.status_upload = obj.status_upload + "," + form["QCDocsVal"];
                    obj.status_upload = obj.status_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && upload_List[0] == null)
                {
                    obj.status_upload = null;
                }
                else if (form["QCDocsVal"] == null && upload_List[0] == null)
                {
                    obj.status_upload = null;
                }

                if (DateTime.TryParse(form["audit_status_date"], out dateValue) == true)
                {
                    obj.audit_status_date = dateValue;
                }
                if (obj.FunUpdateAuditStatus(obj))
                {
                    TempData["Successdata"] = "Audit Status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditStatusUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExternalAuditList");
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditDetails()
        {
            ExternalAuditModels objModel = new ExternalAuditModels();
            try
            {

                if (Request.QueryString["id_external_audit"] != null && Request.QueryString["id_external_audit"] != "")
                {
                    string id_external_audit = Request.QueryString["id_external_audit"];

                    string sSqlstmt = "select id_external_audit,audit_category,audit_no,audit_start_date,audit_type,audit_criteria,upload,entity_name,"
                       + "audit_status,remarks,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,status_upload,audit_status_date,audit_complete_date,notification_to"
                        + " from t_external_audit where id_external_audit=" + id_external_audit;

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            audit_category = dsAuditList.Tables[0].Rows[0]["audit_category"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsAuditList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            //branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["branch"].ToString()),
                            //dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[0]["team"].ToString()),
                            //location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[0]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[0]["entity_name"].ToString(),
                            upload = dsAuditList.Tables[0].Rows[0]["upload"].ToString(),

                            audit_status = objGlobaldata.GetAuditStatusById(dsAuditList.Tables[0].Rows[0]["audit_status"].ToString()),
                            remarks = dsAuditList.Tables[0].Rows[0]["remarks"].ToString(),

                            status_upload = dsAuditList.Tables[0].Rows[0]["status_upload"].ToString(),
                            notification_to = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["notification_to"].ToString()),
                        };

                        if (dsAuditList.Tables[0].Rows[0]["major_nc"].ToString() != "")
                        {
                            objModel.major_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["major_nc"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["minor_nc"].ToString() != "")
                        {
                            objModel.minor_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["minor_nc"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_observation"].ToString() != "")
                        {
                            objModel.no_observation = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_observation"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_noteworthy"].ToString() != "")
                        {
                            objModel.no_noteworthy = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_noteworthy"].ToString());
                        }
                        if (dsAuditList.Tables[0].Rows[0]["no_ci"].ToString() != "")
                        {
                            objModel.no_ci = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_ci"].ToString());
                        }
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_complete_date"].ToString(), out dtValue))
                        {
                            objModel.audit_complete_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_status_date"].ToString(), out dtValue))
                        {
                            objModel.audit_status_date = dtValue;
                        }


                        sSqlstmt = "select id_external_auditor,id_external_audit,auditor_name,auditor_level,contact_no,email_address from t_external_auditor where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Auditor = dsList;

                        sSqlstmt = "select id_external_auditee,id_external_audit,branch,dept_name,team,location from t_external_auditee where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList1 = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Auditee = dsList1;

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);

        }

        [AllowAnonymous]
        public ActionResult ExternalAuditPDF(FormCollection form)
        {
            ExternalAuditModels objModel = new ExternalAuditModels();
            try
            {

                if (form["id_external_audit"] != null && form["id_external_audit"] != "")
                {
                    string id_external_audit = form["id_external_audit"];

                    string sSqlstmt = "select id_external_audit,audit_category,audit_no,audit_start_date,audit_type,audit_criteria,upload,entity_name,"
                       + "audit_status,remarks,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,status_upload,audit_status_date,audit_complete_date,notification_to"
                        + " from t_external_audit where id_external_audit=" + id_external_audit;

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            audit_category = dsAuditList.Tables[0].Rows[0]["audit_category"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsAuditList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            //branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["branch"].ToString()),
                            //dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[0]["team"].ToString()),
                            //location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[0]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[0]["entity_name"].ToString(),
                            upload = dsAuditList.Tables[0].Rows[0]["upload"].ToString(),

                            audit_status = objGlobaldata.GetAuditStatusById(dsAuditList.Tables[0].Rows[0]["audit_status"].ToString()),
                            remarks = dsAuditList.Tables[0].Rows[0]["remarks"].ToString(),
                            major_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["major_nc"].ToString()),
                            minor_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["minor_nc"].ToString()),
                            no_observation = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_observation"].ToString()),
                            no_noteworthy = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_noteworthy"].ToString()),
                            no_ci = Convert.ToInt32(dsAuditList.Tables[0].Rows[0]["no_ci"].ToString()),
                            status_upload = dsAuditList.Tables[0].Rows[0]["status_upload"].ToString(),
                            notification_to = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["notification_to"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_complete_date"].ToString(), out dtValue))
                        {
                            objModel.audit_complete_date = dtValue;
                        }
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_status_date"].ToString(), out dtValue))
                        {
                            objModel.audit_status_date = dtValue;
                        }
                        ViewBag.Audit = objModel;

                        sSqlstmt = "select id_external_auditor,id_external_audit,auditor_name,auditor_level,contact_no,email_address from t_external_auditor where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Auditor = dsList;

                        sSqlstmt = "select id_external_auditee,id_external_audit,branch,dept_name,team,location from t_external_auditee where id_external_audit='" + id_external_audit + "'";
                        DataSet dsList1 = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.Auditee = dsList1;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/ExternalAudit/PrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                 "--header-spacing \"0\" ", header);

            return new ViewAsPdf("ExternalAuditPDF", "ExternalAudit")
            {
                FileName = "ExternalAudit.pdf",
                Cookies = cookieCollection,
                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches =
                   customSwitches,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 35 },
                // PageMargins = new Rotativa.Options.Margins(0, 3, 32, 3),
            };

        }

        [AllowAnonymous]
        public ActionResult RaiseNCList()
        {
            ViewBag.SubMenutype = "ExtNCList";
            ExternalAuditModelsList objExternalAuditModelsList = new ExternalAuditModelsList();
            objExternalAuditModelsList.ExternalAuditList = new List<ExternalAuditModels>();

            try
            {
                string sSqlstmt = "select  t.id_external_audit,audit_no,audit_start_date,entity_name,audit_status_date,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,"
                + "(select group_concat(auditor_name, '-', email_address) from t_external_auditor t3 where t.id_external_audit = t3.id_external_audit) as auditor_name,"
                + "(select group_concat(branch) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as team,"
                + " (select group_concat(location) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as location,"
                + " t.id_external_audit,audit_no,audit_start_date,entity_name,audit_status_date"
                + " from t_external_audit t, dropdownitems d  where  t.audit_status = d.item_id and item_desc = 'Completed' and active = 1";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsAuditList.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        ExternalAuditModels objModels = new ExternalAuditModels
                        {
                            id_external_audit = dsAuditList.Tables[0].Rows[i]["id_external_audit"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[i]["audit_no"].ToString(),
                            auditor_name = dsAuditList.Tables[0].Rows[i]["auditor_name"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[i]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[i]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[i]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[i]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[i]["entity_name"].ToString(),
                            major_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[i]["major_nc"].ToString()),
                            minor_nc = Convert.ToInt32(dsAuditList.Tables[0].Rows[i]["minor_nc"].ToString()),
                            no_observation = Convert.ToInt32(dsAuditList.Tables[0].Rows[i]["no_observation"].ToString()),
                            no_noteworthy = Convert.ToInt32(dsAuditList.Tables[0].Rows[i]["no_noteworthy"].ToString()),
                            no_ci = Convert.ToInt32(dsAuditList.Tables[0].Rows[i]["no_ci"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsAuditList.Tables[0].Rows[i]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModels.audit_start_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[i]["audit_status_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["audit_status_date"].ToString(), out dtDocDate))
                        {
                            objModels.audit_status_date = dtDocDate;
                        }


                        string sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + objModels.id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Major Nonconformity'";
                        DataSet dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModels.cnt_major = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + objModels.id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Minor Nonconformity'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModels.cnt_minor = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + objModels.id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Note-Worthy'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModels.cnt_note = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + objModels.id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Potential Nonconformity'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModels.cnt_potential = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + objModels.id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Continual Improvement'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModels.cnt_ci = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        objExternalAuditModelsList.ExternalAuditList.Add(objModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in RaiseNCList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RaiseNCList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objExternalAuditModelsList.ExternalAuditList.ToList());
        }

        //Raise NC    
        [AllowAnonymous]
        public ActionResult RaiseNonconformity()
        {
            ExternalAuditModels objModel = new ExternalAuditModels();
            try
            {
                if (Request.QueryString["id_external_audit"] != null && Request.QueryString["id_external_audit"] != "")
                {
                    string id_external_audit = Request.QueryString["id_external_audit"];

                    string sSqlstmt = "select t.id_external_audit,audit_category,audit_no,audit_start_date,major_nc,minor_nc,no_observation,no_noteworthy,no_ci,"
                       + " (select group_concat(branch) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where t.id_external_audit = t3.id_external_audit) as team"

                        + " from t_external_audit t where id_external_audit = '" + id_external_audit + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsModelsList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            audit_category = dsModelsList.Tables[0].Rows[0]["audit_category"].ToString(),
                            audit_no = dsModelsList.Tables[0].Rows[0]["audit_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsModelsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModelsList.Tables[0].Rows[0]["team"].ToString()),
                            major_nc = Convert.ToInt32(dsModelsList.Tables[0].Rows[0]["major_nc"].ToString()),
                            minor_nc = Convert.ToInt32(dsModelsList.Tables[0].Rows[0]["minor_nc"].ToString()),
                            no_observation = Convert.ToInt32(dsModelsList.Tables[0].Rows[0]["no_observation"].ToString()),
                            no_noteworthy = Convert.ToInt32(dsModelsList.Tables[0].Rows[0]["no_noteworthy"].ToString()),
                            no_ci = Convert.ToInt32(dsModelsList.Tables[0].Rows[0]["no_ci"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }

                        string sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Major Nonconformity'";
                        DataSet dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModel.cnt_major = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Minor Nonconformity'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModel.cnt_minor = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Note-Worthy'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModel.cnt_note = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Potential Nonconformity'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModel.cnt_potential = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        sql1 = "select count(id_nc) as nc_count from t_external_audit_nc T1,dropdownitems T2 where id_external_audit = '" + id_external_audit + "' and finding_category = T2.item_id and item_desc = 'Continual Improvement'";
                        dsList = objGlobaldata.Getdetails(sql1);

                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            objModel.cnt_ci = Convert.ToInt32(dsList.Tables[0].Rows[0]["nc_count"].ToString());
                        }

                        objModel.pend_major = Convert.ToInt32(objModel.major_nc) - objModel.cnt_major;
                        objModel.pend_minor = Convert.ToInt32(objModel.minor_nc) - objModel.cnt_minor;
                        objModel.pend_potential = Convert.ToInt32(objModel.no_observation) - objModel.cnt_potential;
                        objModel.pend_note = Convert.ToInt32(objModel.no_noteworthy) - objModel.cnt_note;
                        objModel.pend_ci = Convert.ToInt32(objModel.no_ci) - objModel.cnt_ci;

                        ViewBag.Category = objGlobaldata.GetAuditNCList();

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RaiseNonconformity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //Raise NC 
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RaiseNonconformity(ExternalAuditModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                DateTime dateValue;


                if (DateTime.TryParse(form["corrective_agreed_date"], out dateValue) == true)
                {
                    objModel.corrective_agreed_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RaiseNonconformity-upload: " + ex.ToString());
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objModel.FunAddNonconformity(objModel))
                {
                    TempData["Successdata"] = "External audit finding raised successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RaiseNonconformity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("RaiseNCList");
        }

        //NC List
        [AllowAnonymous]
        public ActionResult NonconformityList()
        {
            ExternalAuditModelsList objAttList = new ExternalAuditModelsList();
            objAttList.ExternalAuditList = new List<ExternalAuditModels>();

            try
            {
                if (Request.QueryString["id_external_audit"] != null && Request.QueryString["id_external_audit"] != "")
                {
                    string id_external_audit = Request.QueryString["id_external_audit"];
                    string id_nc = Request.QueryString["id_nc"];
                    string sSqlstmt = "select id_nc,id_external_audit,nc_no,corrective_agreed_date,finding_details,why_finding,upload,finding_category,nc_status"
                    + " from t_external_audit_nc where id_external_audit = '" + id_external_audit + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                ExternalAuditModels objScheduleMdl = new ExternalAuditModels
                                {
                                    id_nc = dsList.Tables[0].Rows[i]["id_nc"].ToString(),
                                    id_external_audit = dsList.Tables[0].Rows[i]["id_external_audit"].ToString(),
                                    nc_no = dsList.Tables[0].Rows[i]["nc_no"].ToString(),
                                    finding_details = dsList.Tables[0].Rows[i]["finding_details"].ToString(),
                                    why_finding = dsList.Tables[0].Rows[i]["why_finding"].ToString(),
                                    upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                    finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[i]["finding_category"].ToString()),
                                    nc_status = objGlobaldata.GetDropdownitemById(dsList.Tables[0].Rows[i]["nc_status"].ToString()),

                                };
                                DateTime dtDocDate;
                                if (dsList.Tables[0].Rows[i]["corrective_agreed_date"].ToString() != ""
                                 && DateTime.TryParse(dsList.Tables[0].Rows[i]["corrective_agreed_date"].ToString(), out dtDocDate))
                                {
                                    objScheduleMdl.corrective_agreed_date = dtDocDate;
                                }
                                objAttList.ExternalAuditList.Add(objScheduleMdl);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NonconformityList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }

                        string sql2 = "select id_nc,id_external_audit,nc_no,corrective_agreed_date,finding_details,why_finding,upload,finding_category"
                  + " from t_external_audit_nc where id_nc = '" + id_nc + "'";
                        DataSet dsNCList = objGlobaldata.Getdetails(sql2);
                        if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                ExternalAuditModels objNC = new ExternalAuditModels
                                {
                                    id_nc = dsNCList.Tables[0].Rows[0]["id_nc"].ToString(),
                                    id_external_audit = dsNCList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                                    nc_no = dsNCList.Tables[0].Rows[0]["nc_no"].ToString(),
                                    finding_details = dsNCList.Tables[0].Rows[0]["finding_details"].ToString(),
                                    upload = dsNCList.Tables[0].Rows[0]["upload"].ToString(),
                                    finding_category = (dsNCList.Tables[0].Rows[0]["finding_category"].ToString()),
                                    why_finding = (dsNCList.Tables[0].Rows[0]["why_finding"].ToString()),

                                };
                                DateTime dtDocDate;
                                if (dsNCList.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                                 && DateTime.TryParse(dsNCList.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtDocDate))
                                {
                                    objNC.corrective_agreed_date = dtDocDate;
                                }
                                ViewBag.NC = objNC;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NonconformityList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }

                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "Audit Finding Not Raised";
                        return RedirectToAction("RaiseNCList");
                    }
                    ViewBag.Category = objGlobaldata.GetAuditNCList();

                }
                else
                {
                    TempData["alertdata"] = "Id Cannot be null";
                    return RedirectToAction("RaiseNCList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RaiseNCList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAttList.ExternalAuditList.ToList());
        }

        //NC update
        [HttpPost]
        [AllowAnonymous]
        public ActionResult NonconformityUpdate(FormCollection form, ExternalAuditModels objModel, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;


                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NonconformityUpdate-upload: " + ex.ToString());
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.upload = objModel.upload + "," + form["QCDocsVal"];
                    objModel.upload = objModel.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objModel.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.upload = null;
                }
                if (DateTime.TryParse(form["corrective_agreed_date"], out dateValue) == true)
                {
                    objModel.corrective_agreed_date = dateValue;
                }


                if (objModel.FunUpdateNonconformity(objModel))
                {
                    TempData["Successdata"] = "External audit finding updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NonconformityList", new { id_external_audit = objModel.id_external_audit });
        }

        //Nonconformity  detail
        [AllowAnonymous]
        public ActionResult NonconformityDetails()
        {
            ExternalAuditModels objModel = new ExternalAuditModels();

            try
            {

                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.ApprStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditNC");
                    string sSqlstmt = "select group_concat(auditor_name,'-',email_address) as auditor_name,T1.id_external_audit,audit_no,audit_start_date,entity_name,audit_status_date,"
                    + "nc_no,corrective_agreed_date,finding_details,why_finding,T1.upload,finding_category,T1.logged_by,T1.logged_date,nc_status,nc_status_remarks,"
                     + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                    + " from t_external_audit_nc T1, t_external_audit T2,t_external_auditor T3 where T1.id_external_audit = T2.id_external_audit and T3.id_external_audit = T2.id_external_audit and id_nc = '" + sid_nc + "'";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ExternalAuditModels
                        {
                            auditor_name = dsAuditList.Tables[0].Rows[0]["auditor_name"].ToString(),
                            audit_no = dsAuditList.Tables[0].Rows[0]["audit_no"].ToString(),
                            id_external_audit = dsAuditList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAuditList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsAuditList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsAuditList.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsAuditList.Tables[0].Rows[0]["location"].ToString()),
                            entity_name = dsAuditList.Tables[0].Rows[0]["entity_name"].ToString(),
                            nc_no = dsAuditList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsAuditList.Tables[0].Rows[0]["finding_details"].ToString(),
                            upload = dsAuditList.Tables[0].Rows[0]["upload"].ToString(),
                            finding_category = objGlobaldata.GetAuditNCById(dsAuditList.Tables[0].Rows[0]["finding_category"].ToString()),
                            why_finding = (dsAuditList.Tables[0].Rows[0]["why_finding"].ToString()),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString()),
                            nc_status = objGlobaldata.GetDropdownitemById(dsAuditList.Tables[0].Rows[0]["nc_status"].ToString()),
                            nc_status_remarks = (dsAuditList.Tables[0].Rows[0]["nc_status_remarks"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.audit_start_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[0]["audit_status_date"].ToString() != ""
                          && DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["audit_status_date"].ToString(), out dtDocDate))
                        {
                            objModel.audit_status_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_agreed_date = dtDocDate;
                        }
                        if (dsAuditList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                       && DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //NC Update    
        [AllowAnonymous]
        public ActionResult NCStatus()
        {
            ExternalAuditModels objModel = new ExternalAuditModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string id_nc = Request.QueryString["id_nc"];
                    ViewBag.Status = objGlobaldata.GetDropdownList("NCR Status");
                    string sSqlstmt = "select T1.id_external_audit,id_nc,audit_category,audit_no,audit_start_date,finding_details,nc_status,corrective_agreed_date,nc_status_remarks,"
                     + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team"

                        + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit=T2.id_external_audit and id_nc = '" + id_nc + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new ExternalAuditModels
                        {
                            id_external_audit = dsModelsList.Tables[0].Rows[0]["id_external_audit"].ToString(),
                            id_nc = dsModelsList.Tables[0].Rows[0]["id_nc"].ToString(),
                            audit_category = dsModelsList.Tables[0].Rows[0]["audit_category"].ToString(),
                            audit_no = dsModelsList.Tables[0].Rows[0]["audit_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsModelsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModelsList.Tables[0].Rows[0]["team"].ToString()),

                            finding_details = dsModelsList.Tables[0].Rows[0]["finding_details"].ToString(),
                            nc_status = dsModelsList.Tables[0].Rows[0]["nc_status"].ToString(),
                            nc_status_remarks = dsModelsList.Tables[0].Rows[0]["nc_status_remarks"].ToString(),


                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_agreed_date = dtValue;
                        }
                    }
                    ViewBag.Category = objGlobaldata.GetAuditNCList();

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult NCStatus(FormCollection form, ExternalAuditModels obj)
        {
            try
            {
                if (obj.FunUpdateNCStatus(obj))
                {
                    TempData["Successdata"] = "NC status updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCStatus: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("NonconformityList", new { id_external_audit = obj.id_external_audit });
        }


        [AllowAnonymous]
        public ActionResult AuditLogList(string branch_name)
        {
            AuditLogModelsList objAttList = new AuditLogModelsList();
            objAttList.LogList = new List<AuditLogModels>();
            ViewBag.SubMenutype = "ExtAuditLog";
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);


                string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,risk_level,rca_details,ca_notifiedto,nc_team,"
                + "(select group_concat(auditor_name, '-', email_address) from t_external_auditor t3 where T1.id_external_audit = t3.id_external_audit) as auditor_name,"
                + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                    + " from t_external_audit T1,t_external_audit_nc T2"
                + " where T1.id_external_audit = T2.id_external_audit and active = 1";

                //if (branch_name != null && branch_name != "")
                //{
                //    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                //    ViewBag.Branch_name = branch_name;
                //}
                //else
                //{
                //    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                //    ViewBag.Branch_name = sBranch_name;
                //}


                sSqlstmt = sSqlstmt + " order by id_nc desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditLogModels objScheduleMdl = new AuditLogModels
                            {
                                id_nc = dsList.Tables[0].Rows[i]["id_nc"].ToString(),
                                Audit_no = dsList.Tables[0].Rows[i]["audit_no"].ToString(),
                                nc_no = dsList.Tables[0].Rows[i]["nc_no"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[i]["branch"].ToString()),
                                dept_name = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[i]["dept_name"].ToString()),
                                //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[i]["team"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                Audit_Status = objGlobaldata.GetAuditStatusById(dsList.Tables[0].Rows[i]["audit_status"].ToString()),
                                finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[i]["finding_category"].ToString()),
                                auditors = dsList.Tables[0].Rows[i]["auditor_name"].ToString(),

                                risk_level = dsList.Tables[0].Rows[i]["risk_level"].ToString(),
                                rca_details = dsList.Tables[0].Rows[i]["rca_details"].ToString(),
                                ca_notifiedto = dsList.Tables[0].Rows[i]["ca_notifiedto"].ToString(),
                                nc_team = dsList.Tables[0].Rows[i]["nc_team"].ToString(),

                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["audit_start_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["audit_start_date"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.audit_start_date = dtDocDate;
                            }

                            objAttList.LogList.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAttList.LogList.ToList());
        }

        //Immediate Action
        [AllowAnonymous]
        public ActionResult AddDisposition()
        {
            AuditLogModels objModel = new AuditLogModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    NCModels objNCModel = new NCModels();
                    ViewBag.DispositonAction = objNCModel.GetNCDispositionActionList();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.RiskLevel = objGlobaldata.GetNCRiskLevelList();
                    string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,corrective_agreed_date,finding_details,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,risk_nc,risk_level,disp_upload,"
                + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                        + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc='" + sid_nc + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["audit_status"].ToString()),
                            finding_category = objGlobaldata.GetAuditNCById(dsNCModels.Tables[0].Rows[0]["finding_category"].ToString()),
                            finding_details = dsNCModels.Tables[0].Rows[0]["finding_details"].ToString(),

                            disp_action_taken = (dsNCModels.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                            disp_explain = (dsNCModels.Tables[0].Rows[0]["disp_explain"].ToString()),
                            disp_notifiedto = (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
                            risk_nc = (dsNCModels.Tables[0].Rows[0]["risk_nc"].ToString()),
                            risk_level = (dsNCModels.Tables[0].Rows[0]["risk_level"].ToString()),
                            disp_upload = (dsNCModels.Tables[0].Rows[0]["disp_upload"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.audit_start_date = dtDocDate;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_agreed_date = dtDocDate;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["disp_notifeddate"].ToString() != ""
                          && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtDocDate))
                        {
                            objModel.disp_notifeddate = dtDocDate;
                        }
                        AuditLogModelsList NcDispList = new AuditLogModelsList();
                        NcDispList.LogList = new List<AuditLogModels>();

                        string sSqlstmt1 = "select id_audit_nc_disp_action,id_nc,disp_action" +
                            ",disp_resp_person,disp_complete_date from t_external_audit_nc_disp_action where id_nc='" + sid_nc + "'";
                        DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt1);

                        if (dsDispModels.Tables.Count > 0 && dsDispModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsDispModels.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    AuditLogModels objDispModel = new AuditLogModels
                                    {
                                        id_audit_nc_disp_action = (dsDispModels.Tables[0].Rows[i]["id_audit_nc_disp_action"].ToString()),
                                        disp_action = (dsDispModels.Tables[0].Rows[i]["disp_action"].ToString()),
                                        disp_resp_person = (dsDispModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
                                    };
                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsDispModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
                                    {
                                        objDispModel.disp_complete_date = dtValue;
                                    }
                                    NcDispList.LogList.Add(objDispModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                                ViewBag.NcDispList = NcDispList;
                            }
                        }
                        return View(objModel);
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditLogList");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddDisposition(AuditLogModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> disp_upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (disp_upload != null && files.ContentLength > 0)
                {
                    objModel.disp_upload = "";
                    foreach (var file in disp_upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.disp_upload = objModel.disp_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddDisposition-upload: " + ex.ToString());
                        }
                    }
                    objModel.disp_upload = objModel.disp_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.disp_upload = objModel.disp_upload + "," + form["QCDocsVal"];
                    objModel.disp_upload = objModel.disp_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objModel.disp_upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objModel.disp_upload = null;
                }

                AuditLogModelsList objModelList = new AuditLogModelsList();
                objModelList.LogList = new List<AuditLogModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["disp_notifeddate"], out dateValue) == true)
                {
                    objModel.disp_notifeddate = dateValue;
                }


                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.disp_notifiedto = objModel.disp_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.disp_notifiedto != null)
                {
                    objModel.disp_notifiedto = objModel.disp_notifiedto.Trim(',');
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    AuditLogModels objNCModel = new AuditLogModels();
                    if (form["disp_action " + i] != "" && form["disp_action " + i] != null)
                    {
                        objNCModel.disp_action = form["disp_action " + i];
                        objNCModel.disp_resp_person = form["disp_resp_person " + i];
                        if (DateTime.TryParse(form["disp_complete_date " + i], out dateValue) == true)
                        {
                            objNCModel.disp_complete_date = dateValue;
                        }
                        objModelList.LogList.Add(objNCModel);
                    }
                }

                if (objModel.FunExtAuditUpdateDisposition(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Disposition details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }


        //RCA
        public ActionResult AddRCA()
        {
            AuditLogModels objModel = new AuditLogModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];

                    NCModels objNCModel = new NCModels();
                    ViewBag.RCATechniqueList = objNCModel.GetRCATechniqueList();
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");


                    string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,corrective_agreed_date,finding_details,"
                        + "rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_started_date,"
                         + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                        + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["audit_status"].ToString()),
                            finding_category = objGlobaldata.GetAuditNCById(dsNCModels.Tables[0].Rows[0]["finding_category"].ToString()),
                            finding_details = dsNCModels.Tables[0].Rows[0]["finding_details"].ToString(),

                            rca_technique = (dsNCModels.Tables[0].Rows[0]["rca_technique"].ToString()),
                            rca_details = (dsNCModels.Tables[0].Rows[0]["rca_details"].ToString()),
                            rca_upload = (dsNCModels.Tables[0].Rows[0]["rca_upload"].ToString()),
                            rca_action = (dsNCModels.Tables[0].Rows[0]["rca_action"].ToString()),
                            rca_justify = (dsNCModels.Tables[0].Rows[0]["rca_justify"].ToString()),
                            rca_reportedby = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()),
                            rca_notifiedto = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()),
                        };

                        if (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()).Split(',');
                        }
                        if (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_agreed_date = dtValue;
                        }


                        if (dsNCModels.Tables[0].Rows[0]["rca_started_date"].ToString() != ""
                        && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["rca_started_date"].ToString(), out dtValue))
                        {
                            objModel.rca_started_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
                        {
                            objModel.rca_reporteddate = dtValue;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditLogList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditLogList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddRCA(AuditLogModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> rca_upload)
        {
            try
            {
                AuditLogModelsList objModelList = new AuditLogModelsList();
                objModelList.LogList = new List<AuditLogModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
                }
                if (DateTime.TryParse(form["rca_started_date"], out dateValue) == true)
                {
                    objModel.rca_started_date = dateValue;
                }
                IList<HttpPostedFileBase> rca_uploadList = (IList<HttpPostedFileBase>)rca_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (rca_uploadList[0] != null)
                {
                    objModel.rca_upload = "";
                    foreach (var file in rca_upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.rca_upload = objModel.rca_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddRCA-upload: " + ex.ToString());
                        }
                    }
                    objModel.rca_upload = objModel.rca_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.rca_upload = objModel.rca_upload + "," + form["QCDocsVal"];
                    objModel.rca_upload = objModel.rca_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && rca_uploadList[0] == null)
                {
                    objModel.rca_upload = null;
                }
                else if (form["QCDocsVal"] == null && rca_uploadList[0] == null)
                {
                    objModel.rca_upload = null;
                }

                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.rca_reportedby = objModel.rca_reportedby + "," + form["aempno " + i];
                    }
                }
                if (objModel.rca_reportedby != null)
                {
                    objModel.rca_reportedby = objModel.rca_reportedby.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.rca_notifiedto = objModel.rca_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.rca_notifiedto != null)
                {
                    objModel.rca_notifiedto = objModel.rca_notifiedto.Trim(',');
                }

                if (objModel.FunExtAuditUpdateRCA(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Root Cause Analysis details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //Corrective Action
        public ActionResult AddCA()
        {
            AuditLogModels objModel = new AuditLogModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    //ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    //ViewBag.NCRStatus = objModel.GetNCRStatusList();
                    //ViewBag.Action = objModel.GetNCActionImplementList();
                    ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                    ViewBag.Location = objGlobaldata.GetLocationListBox();

                    string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,corrective_agreed_date,finding_details,"
                      + "ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date,"
                       + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                      + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc='" + sid_nc + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["audit_status"].ToString()),
                            finding_category = objGlobaldata.GetAuditNCById(dsNCModels.Tables[0].Rows[0]["finding_category"].ToString()),
                            finding_details = dsNCModels.Tables[0].Rows[0]["finding_details"].ToString(),

                            ca_proposed_by = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
                            ca_notifiedto = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()),
                        };
                        if (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()).Split(',');
                        }
                        if (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_agreed_date = dtValue;
                        }

                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
                        {
                            objModel.ca_verfiry_duedate = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
                        {
                            objModel.ca_notifed_date = dtValue;
                        }


                        AuditLogModelsList CAList = new AuditLogModelsList();
                        CAList.LogList = new List<AuditLogModels>();

                        string Ssql = "select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_external_audit_nc_ca where id_nc = '" + sid_nc + "' and ca_active=1";

                        DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

                        if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    AuditLogModels objNCModels = new AuditLogModels
                                    {
                                        id_audit_nc_ca = (dsCALsit.Tables[0].Rows[i]["id_audit_nc_ca"].ToString()),
                                        ca_div = (dsCALsit.Tables[0].Rows[i]["ca_div"].ToString()),
                                        ca_loc = (dsCALsit.Tables[0].Rows[i]["ca_loc"].ToString()),
                                        ca_dept = (dsCALsit.Tables[0].Rows[i]["ca_dept"].ToString()),
                                        ca_rootcause = (dsCALsit.Tables[0].Rows[i]["ca_rootcause"].ToString()),
                                        ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
                                        ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
                                        ca_resp_person = (dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                    };
                                    if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                    {
                                        objNCModels.ca_target_date = dtValue;
                                    }

                                    CAList.LogList.Add(objNCModels);


                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                        }
                        ViewBag.CorrectiveAction = CAList;

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditLogList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditLogList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCA(AuditLogModels objModel, FormCollection form)
        {
            try
            {
                AuditLogModelsList objModelList = new AuditLogModelsList();
                objModelList.LogList = new List<AuditLogModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["ca_verfiry_duedate"], out dateValue) == true)
                {
                    objModel.ca_verfiry_duedate = dateValue;
                }

                if (DateTime.TryParse(form["ca_notifed_date"], out dateValue) == true)
                {
                    objModel.ca_notifed_date = dateValue;
                }


                //Reported By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.ca_proposed_by = objModel.ca_proposed_by + "," + form["aempno " + i];
                    }
                }
                if (objModel.ca_proposed_by != null)
                {
                    objModel.ca_proposed_by = objModel.ca_proposed_by.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.ca_notifiedto = objModel.ca_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.ca_notifiedto != null)
                {
                    objModel.ca_notifiedto = objModel.ca_notifiedto.Trim(',');
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    AuditLogModels objNCModel = new AuditLogModels();
                    if (form["ca_div " + i] != "" && form["ca_div " + i] != null)
                    {
                        objNCModel.id_audit_nc_ca = form["id_audit_nc_ca " + i];
                        objNCModel.ca_div = form["ca_div " + i];
                        objNCModel.ca_loc = form["ca_loc " + i];
                        objNCModel.ca_dept = form["ca_dept " + i];
                        objNCModel.ca_rootcause = form["ca_rootcause " + i];
                        objNCModel.ca_ca = form["ca_ca " + i];
                        objNCModel.ca_resource = form["ca_resource " + i];
                        objNCModel.ca_resp_person = form["ca_resp_person " + i];
                        if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
                        {
                            objNCModel.ca_target_date = dateValue;
                        }
                        objModelList.LogList.Add(objNCModel);
                    }
                }

                if (objModel.FunExtAuditUpdateCA(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Corrective Action details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        //team
        [AllowAnonymous]
        public ActionResult AddTeam()
        {
            AuditLogModels objModel = new AuditLogModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();


                    string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,corrective_agreed_date,finding_details,"
                        + "nc_team,team_approvedby,team_notifiedto,team_targetdate,"
                          + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                        + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["audit_status"].ToString()),
                            finding_category = objGlobaldata.GetAuditNCById(dsNCModels.Tables[0].Rows[0]["finding_category"].ToString()),
                            finding_details = dsNCModels.Tables[0].Rows[0]["finding_details"].ToString(),

                            nc_team = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()),
                            team_approvedby = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()),
                            team_notifiedto = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()),
                        };
                        if (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString() != "")
                        {
                            ViewBag.TeamArray = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString() != "")
                        {
                            ViewBag.ApprovedByArray = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_agreed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
                        {
                            objModel.team_targetdate = dtValue;
                        }
                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditLogList");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditLogList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddTeam(AuditLogModels objModel, FormCollection form)
        {
            try
            {
                //AuditLogModelsList objModelList = new AuditLogModelsList();
                //objModelList.LogList = new List<AuditLogModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
                }

                //Team
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["tempno " + i] != "" && form["tempno " + i] != null)
                    {
                        objModel.nc_team = objModel.nc_team + "," + form["tempno " + i];
                    }
                }
                if (objModel.nc_team != null)
                {
                    objModel.nc_team = objModel.nc_team.Trim(',');
                }

                //ApprovedBy
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.team_approvedby = objModel.team_approvedby + "," + form["aempno " + i];
                    }
                }
                if (objModel.team_approvedby != null)
                {
                    objModel.team_approvedby = objModel.team_approvedby.Trim(',');
                }


                //Notified To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.team_notifiedto = objModel.team_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.team_notifiedto != null)
                {
                    objModel.team_notifiedto = objModel.team_notifiedto.Trim(',');
                }

                if (objModel.FunExtAuditUpdateTeam(objModel))
                {
                    TempData["Successdata"] = "Added Team details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }
        //Verification
        public ActionResult AddVerification()
        {
            AuditLogModels objModel = new AuditLogModels();
            try
            {
                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];

                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    ViewBag.OpenClose = objGlobaldata.GetConstantValue("OpenClose");
                    ViewBag.Causes = objGlobaldata.GetConstantValue("NC-RootCause");

                    NCModels objNCModel = new NCModels();
                    ViewBag.NCRStatus = objNCModel.GetNCRStatusList();
                    ViewBag.Action = objNCModel.GetNCActionImplementList();

                    string sSqlstmt = "select id_nc,audit_no,nc_no,audit_start_date,audit_status,finding_category,corrective_agreed_date,finding_details,"
                         + "v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload,v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto,v_eleminate_root_cause,v_risk_reduce,v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason,"
                      + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                         + " from t_external_audit T1,t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["audit_status"].ToString()),
                            finding_category = objGlobaldata.GetAuditNCById(dsNCModels.Tables[0].Rows[0]["finding_category"].ToString()),
                            finding_details = dsNCModels.Tables[0].Rows[0]["finding_details"].ToString(),

                            v_implement = (dsNCModels.Tables[0].Rows[0]["v_implement"].ToString()),
                            v_implement_explain = (dsNCModels.Tables[0].Rows[0]["v_implement_explain"].ToString()),
                            v_rca = (dsNCModels.Tables[0].Rows[0]["v_rca"].ToString()),
                            v_rca_explain = (dsNCModels.Tables[0].Rows[0]["v_rca_explain"].ToString()),
                            v_discrepancies = (dsNCModels.Tables[0].Rows[0]["v_discrepancies"].ToString()),
                            v_discrep_explain = (dsNCModels.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
                            v_upload = (dsNCModels.Tables[0].Rows[0]["v_upload"].ToString()),
                            v_status = (dsNCModels.Tables[0].Rows[0]["v_status"].ToString()),
                            v_verifiedto = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                            v_notifiedto = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()),

                            v_eleminate_root_cause = (dsNCModels.Tables[0].Rows[0]["v_eleminate_root_cause"].ToString()),
                            v_risk_reduce = (dsNCModels.Tables[0].Rows[0]["v_risk_reduce"].ToString()),
                            v_risk_reduce_reason = (dsNCModels.Tables[0].Rows[0]["v_risk_reduce_reason"].ToString()),
                            v_process_amended = (dsNCModels.Tables[0].Rows[0]["v_process_amended"].ToString()),
                            v_document_amended = (dsNCModels.Tables[0].Rows[0]["v_document_amended"].ToString()),
                            v_amend_docref = (dsNCModels.Tables[0].Rows[0]["v_amend_docref"].ToString()),
                            v_amend_docname = (dsNCModels.Tables[0].Rows[0]["v_amend_docname"].ToString()),
                            v_ncr_reason = (dsNCModels.Tables[0].Rows[0]["v_ncr_reason"].ToString()),

                        };

                        if (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString() != "")
                        {
                            ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()).Split(',');
                        }

                        if (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString() != "")
                        {
                            ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()).Split(',');
                        }

                        DateTime dtValue;
                        if (dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                        && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtValue))
                        {
                            objModel.audit_start_date = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_agreed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
                        {
                            objModel.v_verified_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
                        {
                            objModel.v_closed_date = dtValue;
                        }
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_amend_docdate"].ToString(), out dtValue))
                        {
                            objModel.v_amend_docdate = dtValue;
                        }
                        AuditLogModelsList objVeriList = new AuditLogModelsList();
                        objVeriList.LogList = new List<AuditLogModels>();

                        string Sql = "Select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                       "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_external_audit_nc_ca where id_nc = '" + sid_nc + "' and ca_active=1";

                        DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

                        if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsCAModels.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    AuditLogModels objCAModel = new AuditLogModels
                                    {
                                        id_audit_nc_ca = (dsCAModels.Tables[0].Rows[i]["id_audit_nc_ca"].ToString()),
                                        ca_div = (dsCAModels.Tables[0].Rows[i]["ca_div"].ToString()),
                                        ca_loc = (dsCAModels.Tables[0].Rows[i]["ca_loc"].ToString()),
                                        ca_dept = (dsCAModels.Tables[0].Rows[i]["ca_dept"].ToString()),
                                        ca_rootcause = (dsCAModels.Tables[0].Rows[i]["ca_rootcause"].ToString()),
                                        ca_ca = (dsCAModels.Tables[0].Rows[i]["ca_ca"].ToString()),
                                        ca_resource = (dsCAModels.Tables[0].Rows[i]["ca_resource"].ToString()),
                                        ca_resp_person = (dsCAModels.Tables[0].Rows[i]["ca_resp_person"].ToString()),
                                        implement_status = (dsCAModels.Tables[0].Rows[i]["implement_status"].ToString()),
                                        ca_effective = (dsCAModels.Tables[0].Rows[i]["ca_effective"].ToString()),
                                        reason = (dsCAModels.Tables[0].Rows[i]["reason"].ToString()),
                                    };

                                    if (DateTime.TryParse(dsCAModels.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
                                    {
                                        objCAModel.ca_target_date = dtValue;
                                    }
                                    objVeriList.LogList.Add(objCAModel);
                                }
                                catch (Exception Ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + Ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.CorrectiveAction = objVeriList;
                        }

                        return View(objModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditLogList");
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditLogList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddVerification(AuditLogModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> v_upload)
        {
            try
            {
                AuditLogModelsList objModelList = new AuditLogModelsList();
                objModelList.LogList = new List<AuditLogModels>();

                DateTime dateValue;

                if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
                {
                    objModel.team_targetdate = dateValue;
                }
                if (DateTime.TryParse(form["v_verified_date"], out dateValue) == true)
                {
                    objModel.v_verified_date = dateValue;
                }
                IList<HttpPostedFileBase> v_uploadList = (IList<HttpPostedFileBase>)v_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (v_uploadList[0] != null)
                {
                    objModel.v_upload = "";
                    foreach (var file in v_upload)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), System.IO.Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.v_upload = objModel.v_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddVerification-upload: " + ex.ToString());
                        }
                    }
                    objModel.v_upload = objModel.v_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.v_upload = objModel.v_upload + "," + form["QCDocsVal"];
                    objModel.v_upload = objModel.v_upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && v_uploadList[0] == null)
                {
                    objModel.v_upload = null;
                }
                else if (form["QCDocsVal"] == null && v_uploadList[0] == null)
                {
                    objModel.v_upload = null;
                }
                //Notified By
                for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
                {
                    if (form["aempno " + i] != "" && form["aempno " + i] != null)
                    {
                        objModel.v_verifiedto = objModel.v_verifiedto + "," + form["aempno " + i];
                    }
                }
                if (objModel.v_verifiedto != null)
                {
                    objModel.v_verifiedto = objModel.v_verifiedto.Trim(',');
                }


                //Notifed To
                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["nempno " + i] != "" && form["nempno " + i] != null)
                    {
                        objModel.v_notifiedto = objModel.v_notifiedto + "," + form["nempno " + i];
                    }
                }
                if (objModel.v_notifiedto != null)
                {
                    objModel.v_notifiedto = objModel.v_notifiedto.Trim(',');
                }


                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    AuditLogModels objNCModel = new AuditLogModels();
                    if (form["ca_div " + i] != "" && form["ca_div " + i] != null)
                    {
                        // objNCModel.ca_div = form["ca_div " + i];
                        // objNCModel.ca_loc = form["ca_loc " + i];
                        //   objNCModel.ca_dept = form["ca_dept " + i];
                        //  //objNCModel.ca_rootcause = form["ca_rootcause " + i];
                        //  objNCModel.ca_ca = form["ca_ca " + i];
                        //objNCModel.ca_resource = form["ca_resource " + i];
                        //objNCModel.ca_resp_person = form["ca_resp_person " + i];
                        //if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
                        //{
                        //    objNCModel.ca_target_date = dateValue;
                        //}
                        objNCModel.id_audit_nc_ca = form["id_audit_nc_ca " + i];
                        objNCModel.implement_status = form["implement_status " + i];
                        objNCModel.ca_effective = form["ca_effective " + i];
                        objNCModel.reason = form["reason " + i];
                    }
                    objModelList.LogList.Add(objNCModel);
                }

                if (objModel.FunExtAuditUpdateVerification(objModel, objModelList))
                {
                    TempData["Successdata"] = "Added Verification details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(true);
        }

        [AllowAnonymous]
        public JsonResult CADocDelete(FormCollection form)
        {
            try
            {

                if (form["id_audit_nc_ca"] != null && form["id_audit_nc_ca"] != "")
                {

                    AuditLogModels Doc = new AuditLogModels();
                    string sid_audit_nc_ca = form["id_audit_nc_ca"];

                    if (Doc.FunDeleteExtAuditCADoc(sid_audit_nc_ca))
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
                objGlobaldata.AddFunctionalLog("Exception in CADocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //NC Report
        [AllowAnonymous]
        public ActionResult NonconformityReportPDF(FormCollection form)
        {
            AuditLogModels objModel = new AuditLogModels();
            NCModels objNCModel = new NCModels();
            MgmtDocumentsModels objMgmt = new MgmtDocumentsModels();
            try
            {

                string sid_nc = form["id_nc"];
                if (sid_nc != null && sid_nc != "")
                {

                    string sSqlstmt = "select audit_no,audit_start_date,audit_type,T1.audit_criteria,"
                    + "entity_name,T2.logged_by,T2.logged_date,audit_status,remarks,audit_status_date,nc_no,corrective_agreed_date,finding_details,why_finding,finding_category,"
                    + "disp_action_taken,disp_explain,risk_nc,risk_level,rca_details,rca_technique,rca_reporteddate,rca_action,rca_justify,ca_proposed_by,"
                    + "v_verifiedto,v_verified_date,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_eleminate_root_cause,v_risk_reduce,"
                    + "v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason,v_status,v_closed_date,"
                     + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                    + " from t_external_audit T1, t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            Audit_no = dsList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsList.Tables[0].Rows[0]["audit_type"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[0]["team"].ToString()),
                            entity_name = dsList.Tables[0].Rows[0]["entity_name"].ToString(),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsList.Tables[0].Rows[0]["audit_status"].ToString()),
                            remarks = dsList.Tables[0].Rows[0]["remarks"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),
                            why_finding = dsList.Tables[0].Rows[0]["why_finding"].ToString(),
                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),

                            disp_action_taken = objNCModel.GetNCDispositionActionById(dsList.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                            disp_explain = dsList.Tables[0].Rows[0]["disp_explain"].ToString(),

                            risk_nc = dsList.Tables[0].Rows[0]["risk_nc"].ToString(),
                            risk_level = objGlobaldata.GetNCRiskLevelById(dsList.Tables[0].Rows[0]["risk_level"].ToString()),

                            rca_details = dsList.Tables[0].Rows[0]["rca_details"].ToString(),
                            rca_technique = objNCModel.GetRCATechniqueById(dsList.Tables[0].Rows[0]["rca_technique"].ToString()),
                            rca_action = dsList.Tables[0].Rows[0]["rca_action"].ToString(),
                            rca_justify = dsList.Tables[0].Rows[0]["rca_justify"].ToString(),

                            ca_proposed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["ca_proposed_by"].ToString()),

                            v_implement = objNCModel.GetNCActionImplementById(dsList.Tables[0].Rows[0]["v_implement"].ToString()),
                            v_implement_explain = dsList.Tables[0].Rows[0]["v_implement_explain"].ToString(),
                            v_rca = dsList.Tables[0].Rows[0]["v_rca"].ToString(),
                            v_rca_explain = dsList.Tables[0].Rows[0]["v_rca_explain"].ToString(),
                            v_discrepancies = dsList.Tables[0].Rows[0]["v_discrepancies"].ToString(),
                            v_discrep_explain = dsList.Tables[0].Rows[0]["v_discrep_explain"].ToString(),
                            v_eleminate_root_cause = dsList.Tables[0].Rows[0]["v_eleminate_root_cause"].ToString(),
                            v_risk_reduce = dsList.Tables[0].Rows[0]["v_risk_reduce"].ToString(),
                            v_risk_reduce_reason = dsList.Tables[0].Rows[0]["v_risk_reduce_reason"].ToString(),
                            v_process_amended = dsList.Tables[0].Rows[0]["v_process_amended"].ToString(),
                            v_document_amended = dsList.Tables[0].Rows[0]["v_document_amended"].ToString(),
                            v_amend_docref = dsList.Tables[0].Rows[0]["v_amend_docref"].ToString(),
                            v_amend_docname = dsList.Tables[0].Rows[0]["v_amend_docname"].ToString(),
                            v_ncr_reason = dsList.Tables[0].Rows[0]["v_ncr_reason"].ToString(),
                            v_status = dsList.Tables[0].Rows[0]["v_status"].ToString(),
                            v_verifiedto = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                        };

                        objModel.finding_categorize = objModel.Audit_criteria + " " + objModel.audit_clause + " " + objModel.description;

                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_agreed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.audit_start_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_amend_docdate"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_amend_docdate"].ToString(), out dtDocDate))
                        {
                            objModel.v_amend_docdate = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_closed_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtDocDate))
                        {
                            objModel.v_closed_date = dtDocDate;
                        }


                        if (dsList.Tables[0].Rows[0]["rca_reporteddate"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtDocDate))
                        {
                            objModel.rca_reporteddate = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_verified_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtDocDate))
                        {
                            objModel.v_verified_date = dtDocDate;
                        }
                        ViewBag.Audit = objModel;


                        string sql1 = "select disp_action,disp_resp_person,disp_complete_date from t_external_audit_nc_disp_action where id_nc='" + sid_nc + "'";
                        ViewBag.ImAction = objGlobaldata.Getdetails(sql1);

                        string sql2 = "select ca_ca,ca_resp_person,ca_target_date from t_external_audit_nc_ca where id_nc='" + sid_nc + "'";
                        ViewBag.CA = objGlobaldata.Getdetails(sql2);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityReportPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/ExternalAudit/NCPrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                "--header-spacing \"0\" " +
            " --header-font-size \"10\" ", header);

            return new ViewAsPdf("NonconformityReportPDF", "ExternalAudit")
            {
                FileName = "NonconformityReport.pdf",
                Cookies = cookieCollection,
                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches =
                   customSwitches,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 35 },
                // PageMargins = new Rotativa.Options.Margins(0, 3, 32, 3),
            };
        }

        [AllowAnonymous]
        public ActionResult NCDetails(FormCollection form)
        {
            AuditLogModels objModel = new AuditLogModels();
            NCModels objNCModel = new NCModels();
            MgmtDocumentsModels objMgmt = new MgmtDocumentsModels();
            try
            {

                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];

                    string sSqlstmt = "select audit_no,audit_start_date,audit_type,T1.audit_criteria,"
                    + "entity_name,T2.logged_by,T2.logged_date,audit_status,remarks,audit_status_date,nc_no,corrective_agreed_date,finding_details,why_finding,finding_category,"
                    + "disp_action_taken,disp_explain,risk_nc,risk_level,rca_details,rca_technique,rca_reporteddate,rca_action,rca_justify,ca_proposed_by,"
                    + "v_verifiedto,v_verified_date,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_eleminate_root_cause,v_risk_reduce,"
                    + "v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason,v_status,v_closed_date,"
                     + " (select group_concat(branch) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as branch,"
                + " (select group_concat(dept_name) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as dept_name,"
                + " (select group_concat(team) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as team,"
                 + " (select group_concat(location) from t_external_auditee t3 where T1.id_external_audit = t3.id_external_audit) as location"

                    + " from t_external_audit T1, t_external_audit_nc T2 where T1.id_external_audit = T2.id_external_audit  and active = 1 and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            Audit_no = dsList.Tables[0].Rows[0]["audit_no"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsList.Tables[0].Rows[0]["audit_type"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[0]["team"].ToString()),
                            entity_name = dsList.Tables[0].Rows[0]["entity_name"].ToString(),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsList.Tables[0].Rows[0]["audit_status"].ToString()),
                            remarks = dsList.Tables[0].Rows[0]["remarks"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),
                            why_finding = dsList.Tables[0].Rows[0]["why_finding"].ToString(),
                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),

                            disp_action_taken = objNCModel.GetNCDispositionActionById(dsList.Tables[0].Rows[0]["disp_action_taken"].ToString()),
                            disp_explain = dsList.Tables[0].Rows[0]["disp_explain"].ToString(),

                            risk_nc = dsList.Tables[0].Rows[0]["risk_nc"].ToString(),
                            risk_level = objGlobaldata.GetNCRiskLevelById(dsList.Tables[0].Rows[0]["risk_level"].ToString()),

                            rca_details = dsList.Tables[0].Rows[0]["rca_details"].ToString(),
                            rca_technique = objNCModel.GetRCATechniqueById(dsList.Tables[0].Rows[0]["rca_technique"].ToString()),
                            rca_action = dsList.Tables[0].Rows[0]["rca_action"].ToString(),
                            rca_justify = dsList.Tables[0].Rows[0]["rca_justify"].ToString(),

                            ca_proposed_by = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["ca_proposed_by"].ToString()),

                            v_implement = objNCModel.GetNCActionImplementById(dsList.Tables[0].Rows[0]["v_implement"].ToString()),
                            v_implement_explain = dsList.Tables[0].Rows[0]["v_implement_explain"].ToString(),
                            v_rca = dsList.Tables[0].Rows[0]["v_rca"].ToString(),
                            v_rca_explain = dsList.Tables[0].Rows[0]["v_rca_explain"].ToString(),
                            v_discrepancies = dsList.Tables[0].Rows[0]["v_discrepancies"].ToString(),
                            v_discrep_explain = dsList.Tables[0].Rows[0]["v_discrep_explain"].ToString(),
                            v_eleminate_root_cause = dsList.Tables[0].Rows[0]["v_eleminate_root_cause"].ToString(),
                            v_risk_reduce = dsList.Tables[0].Rows[0]["v_risk_reduce"].ToString(),
                            v_risk_reduce_reason = dsList.Tables[0].Rows[0]["v_risk_reduce_reason"].ToString(),
                            v_process_amended = dsList.Tables[0].Rows[0]["v_process_amended"].ToString(),
                            v_document_amended = dsList.Tables[0].Rows[0]["v_document_amended"].ToString(),
                            v_amend_docref = dsList.Tables[0].Rows[0]["v_amend_docref"].ToString(),
                            v_amend_docname = dsList.Tables[0].Rows[0]["v_amend_docname"].ToString(),
                            v_ncr_reason = dsList.Tables[0].Rows[0]["v_ncr_reason"].ToString(),
                            v_status = dsList.Tables[0].Rows[0]["v_status"].ToString(),
                            v_verifiedto = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["v_verifiedto"].ToString()),
                        };

                        objModel.finding_categorize = objModel.Audit_criteria + " " + objModel.audit_clause + " " + objModel.description;

                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["corrective_agreed_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_agreed_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_agreed_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["audit_start_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["audit_start_date"].ToString(), out dtDocDate))
                        {
                            objModel.audit_start_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_amend_docdate"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_amend_docdate"].ToString(), out dtDocDate))
                        {
                            objModel.v_amend_docdate = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_closed_date"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtDocDate))
                        {
                            objModel.v_closed_date = dtDocDate;
                        }


                        if (dsList.Tables[0].Rows[0]["rca_reporteddate"].ToString() != ""
                        && DateTime.TryParse(dsList.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtDocDate))
                        {
                            objModel.rca_reporteddate = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["v_verified_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtDocDate))
                        {
                            objModel.v_verified_date = dtDocDate;
                        }

                        ViewBag.Audit = objModel;
                        string sql1 = "select disp_action,disp_resp_person,disp_complete_date from t_external_audit_nc_disp_action where id_nc='" + sid_nc + "'";
                        ViewBag.ImAction = objGlobaldata.Getdetails(sql1);

                        string sql2 = "select ca_ca,ca_resp_person,ca_target_date from t_external_audit_nc_ca where id_nc='" + sid_nc + "'";
                        ViewBag.CA = objGlobaldata.Getdetails(sql2);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityReportPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalAuditEdit(ExternalAuditModels objExternalAudit, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        //{
        //    try
        //    {
        //        HttpPostedFileBase files = Request.Files[0];
        //        string QCDelete = Request.Form["QCDocsValselectall"];

        //        if (Request["button"].Equals("Audit Update"))
        //        {
        //            //string sCertBodyList = form["CertID"];
        //            //string sauditor_name = form["auditor_name"];

        //            //objExternalAudit.CertID = sCertBodyList;
        //            //objExternalAudit.auditor_name = sauditor_name;
        //            //objExternalAudit.nc_status = form["nc_status"];
        //            objExternalAudit.AuditID = form["AuditID"];
        //            objExternalAudit.AuditLocation = form["AuditLocation"];


        //            DateTime dateValue;

        //            if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
        //            {
        //                objExternalAudit.AuditDate = dateValue;
        //            }

        //            if (upload != null && files.ContentLength > 0)
        //            {
        //                objExternalAudit.upload = "";
        //                foreach (var file in upload)
        //                {
        //                    try
        //                    {
        //                        string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(file.FileName));
        //                        string sFilename = System.IO.Path.GetFileName(spath), sFilepath = System.IO.Path.GetDirectoryName(spath);
        //                        file.SaveAs(sFilepath + "/" + sFilename);
        //                        objExternalAudit.upload = objExternalAudit.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit-upload: " + ex.ToString());
        //                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                    }
        //                }
        //                objExternalAudit.upload = objExternalAudit.upload.Trim(',');
        //            }
        //            else
        //            {
        //                ViewBag.Message = "You have not specified a file.";
        //            }
        //            if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
        //            {
        //                objExternalAudit.upload = objExternalAudit.upload + "," + form["QCDocsVal"];
        //                objExternalAudit.upload = objExternalAudit.upload.Trim(',');
        //            }
        //            else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
        //            {
        //                objExternalAudit.upload = null;
        //            }
        //            else if (form["QCDocsVal"] == null && files.ContentLength == 0)
        //            {
        //                objExternalAudit.upload = null;
        //            }

        //            if (objExternalAudit.FunUpdateExternalAudit(objExternalAudit))
        //            {
        //                TempData["Successdata"] = "External audit details updated successfully";
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //        else if (Request["button"].Equals("Save"))
        //        {

        //            if (ExternalAuditFindingsAdd(objExternalAudit, form))
        //            {
        //                TempData["Successdata"] = "Added External audit Findings details successfully";
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //        else
        //        {
        //            if (ExternalAuditFindingsUpdate(objExternalAudit, form))
        //            {
        //                TempData["Successdata"] = "External audit details updated successfully";
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in ExternalAuditEdit: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return RedirectToAction("ExternalAuditList");
        //}

        //--------------------------------------------------------------------------------------    
        [AllowAnonymous]
        public ActionResult AddCertificationBody()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCertificationBody(CertificationBodyModels objCertificationBody)
        {
            //if (ModelState.IsValid)
            {
                objCertBody.FunAddCertificationBody(objCertificationBody);
            }
            return RedirectToAction("CertificationBodyList");
        }

        [AllowAnonymous]
        public ActionResult CertificationBodyList()
        {
            CertificationBodyModelsList objCertificationBodyList = new CertificationBodyModelsList();
            objCertificationBodyList.CertificationBodyList = new List<CertificationBodyModels>();

            try
            {
                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                string sSqlstmt = "SELECT CertID, CertName, Location, address, emailaddress, contact_name, PhoneNumber, FaxNumber FROM t_certification_body order by CertID desc";

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCertificationBodyList.Tables.Count > 0 && dsCertificationBodyList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CertificationBodyModels objCertificationBodyModels = new CertificationBodyModels
                            {
                                CertID = dsCertificationBodyList.Tables[0].Rows[i]["CertID"].ToString(),
                                CertName = dsCertificationBodyList.Tables[0].Rows[i]["CertName"].ToString(),
                                Location = dsCertificationBodyList.Tables[0].Rows[i]["Location"].ToString(),
                                address = dsCertificationBodyList.Tables[0].Rows[i]["address"].ToString(),
                                emailaddress = dsCertificationBodyList.Tables[0].Rows[i]["emailaddress"].ToString(),
                                contact_name = dsCertificationBodyList.Tables[0].Rows[i]["contact_name"].ToString(),
                                PhoneNumber = dsCertificationBodyList.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                FaxNumber = dsCertificationBodyList.Tables[0].Rows[i]["FaxNumber"].ToString()
                            };
                            objCertificationBodyList.CertificationBodyList.Add(objCertificationBodyModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CertificationBodyList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificationBodyList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCertificationBodyList.CertificationBodyList.ToList());
        }

        [AllowAnonymous]
        public ActionResult CertificationBodyDetails()
        {
            CertificationBodyModels objCertificationBody = new CertificationBodyModels();

            try
            {
                if (Request.QueryString["CertID"] != null && Request.QueryString["CertID"] != "")
                {
                    string sCertID = Request.QueryString["CertID"];

                    string sSqlstmt = "SELECT * FROM t_certification_body where CertID='" + sCertID + "'";
                    DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCertificationBodyList.Tables.Count > 0 && dsCertificationBodyList.Tables[0].Rows.Count > 0)
                    {
                        objCertificationBody = new CertificationBodyModels
                        {
                            CertName = dsCertificationBodyList.Tables[0].Rows[0]["CertName"].ToString(),
                            Location = dsCertificationBodyList.Tables[0].Rows[0]["Location"].ToString(),
                            address = dsCertificationBodyList.Tables[0].Rows[0]["address"].ToString(),
                            emailaddress = dsCertificationBodyList.Tables[0].Rows[0]["emailaddress"].ToString(),
                            contact_name = dsCertificationBodyList.Tables[0].Rows[0]["contact_name"].ToString(),
                            PhoneNumber = dsCertificationBodyList.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                            FaxNumber = dsCertificationBodyList.Tables[0].Rows[0]["FaxNumber"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificationBodyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCertificationBody);
        }

        [AllowAnonymous]
        public ActionResult CertificationBodyEdit()
        {
            CertificationBodyModels objCertificationBody = new CertificationBodyModels();

            try
            {
                if (Request.QueryString["CertID"] != null && Request.QueryString["CertID"] != "")
                {
                    string sCertID = Request.QueryString["CertID"];

                    string sSqlstmt = "SELECT * FROM t_certification_body where CertID='" + sCertID + "'";
                    DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCertificationBodyList.Tables[0].Rows.Count > 0)
                    {
                        objCertificationBody = new CertificationBodyModels
                        {
                            CertID = dsCertificationBodyList.Tables[0].Rows[0]["CertID"].ToString(),
                            CertName = dsCertificationBodyList.Tables[0].Rows[0]["CertName"].ToString(),
                            Location = dsCertificationBodyList.Tables[0].Rows[0]["Location"].ToString(),
                            address = dsCertificationBodyList.Tables[0].Rows[0]["address"].ToString(),
                            emailaddress = dsCertificationBodyList.Tables[0].Rows[0]["emailaddress"].ToString(),
                            contact_name = dsCertificationBodyList.Tables[0].Rows[0]["contact_name"].ToString(),
                            PhoneNumber = dsCertificationBodyList.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                            FaxNumber = dsCertificationBodyList.Tables[0].Rows[0]["FaxNumber"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificationBodyEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCertificationBody);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CertificationBodyEdit(CertificationBodyModels objCertificationBodyModels)
        {
            try
            {
                string sCertID = Request.QueryString["CertID"];

                if (objCertificationBodyModels.FunUpdateCertificationBody(objCertificationBodyModels))
                {
                    TempData["Successdata"] = "Certification body details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificationBodyEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddExternalAuditor()
        {
            return InitilizeAddExternalAuditor();
        }

        [AllowAnonymous]
        private ActionResult InitilizeAddExternalAuditor()
        {
            ViewBag.CertBodyList = objCertBody.FunGetCertificateList();


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExternalAuditor(ExternalAuditorModels objExternalAuditor, FormCollection form)
        {
            objExternalAuditor.CertID = form["CertId"];

            //if (ModelState.IsValid)
            {
                objEAudit.FunAddInternalAuditor(objExternalAuditor);
            }
            return RedirectToAction("ExternalAuditorList");
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditorList()
        {
            ExternalAuditorModelsList objCertificationBodyList = new ExternalAuditorModelsList();
            objCertificationBodyList.ExternalAuditorList = new List<ExternalAuditorModels>();

            try
            {

                UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                string sSqlstmt = "SELECT TEA.CertID, CertName, tea.auditor_name, tea.PhoneNumber, tea.emailaddress FROM t_external_auditor as TEA, t_certification_body "
                     + "where tea.certid= t_certification_body.certid order by AuditorID desc";

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
                {
                    ExternalAuditorModels objExternalAuditorModels = new ExternalAuditorModels
                    {
                        CertID = dsCertificationBodyList.Tables[0].Rows[i]["CertID"].ToString(),
                        CertName = dsCertificationBodyList.Tables[0].Rows[i]["CertName"].ToString(),
                        auditor_name = dsCertificationBodyList.Tables[0].Rows[i]["auditor_name"].ToString(),
                        emailaddress = dsCertificationBodyList.Tables[0].Rows[i]["emailaddress"].ToString(),
                        PhoneNumber = dsCertificationBodyList.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                    };
                    objCertificationBodyList.ExternalAuditorList.Add(objExternalAuditorModels);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditorList " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCertificationBodyList.ExternalAuditorList.ToList());
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditorDetails()
        {
            try
            {
                string sauditor_name = Request.QueryString["auditor_name"];

                if (sauditor_name == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string sSqlstmt = "SELECT CertName, tea.auditor_name, tea.PhoneNumber, tea.emailaddress FROM t_external_auditor as TEA, t_certification_body "
                     + "where tea.certid= t_certification_body.certid and tea.auditor_name='" + sauditor_name + "'";

                DataSet dsExternalAuditorList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsExternalAuditorList.Tables[0].Rows.Count > 0)
                {
                    ExternalAuditorModels objExternalAuditorModels = new ExternalAuditorModels
                    {
                        CertID = dsExternalAuditorList.Tables[0].Rows[0]["CertName"].ToString(),
                        auditor_name = dsExternalAuditorList.Tables[0].Rows[0]["auditor_name"].ToString(),
                        emailaddress = dsExternalAuditorList.Tables[0].Rows[0]["emailaddress"].ToString(),
                        PhoneNumber = dsExternalAuditorList.Tables[0].Rows[0]["PhoneNumber"].ToString()
                    };
                    return View(objExternalAuditorModels);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditorDetails:" + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        public ActionResult ExternalAuditorEdit()
        {
            try
            {
                string sauditor_name = Request.QueryString["auditor_name"];

                if (sauditor_name == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string sSqlstmt = "SELECT CertName, tea.auditor_name, tea.PhoneNumber, tea.emailaddress FROM t_external_auditor as TEA, t_certification_body "
                     + "where tea.certid= t_certification_body.certid and tea.auditor_name='" + sauditor_name + "'";

                DataSet dsExternalAuditorList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsExternalAuditorList.Tables[0].Rows.Count > 0)
                {
                    ExternalAuditorModels objExternalAuditorModels = new ExternalAuditorModels
                    {
                        CertID = dsExternalAuditorList.Tables[0].Rows[0]["CertName"].ToString(),
                        auditor_name = dsExternalAuditorList.Tables[0].Rows[0]["auditor_name"].ToString(),
                        emailaddress = dsExternalAuditorList.Tables[0].Rows[0]["emailaddress"].ToString(),
                        PhoneNumber = dsExternalAuditorList.Tables[0].Rows[0]["PhoneNumber"].ToString()
                    };
                    ViewBag.CertBodyList = objCertBody.FunGetCertificateList();
                    ViewBag.UserCertID = objExternalAuditorModels.CertID;

                    return View(objExternalAuditorModels);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditorEdit:" + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalAuditorEdit(ExternalAuditorModels objExternalAuditorModels, FormCollection form)
        {
            string sauditor_name = Request.QueryString["auditor_name"];

            if (sauditor_name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            objExternalAuditorModels.auditor_name = sauditor_name;
            objExternalAuditorModels.CertID = form["CertId"];

            objEAudit.FunUpdateExternalAuditor(objExternalAuditorModels);

            return View();
        }



        [AllowAnonymous]
        public ActionResult AddExternalAuditWithNoFindings()
        {
            ViewBag.SubMenutype = "ExternalAuditWithNoFindings";
            ExternalAuditModels obj = new ExternalAuditModels();
            try
            {
                obj.AuditLocation = objGlobaldata.GetCurrentUserSession().division;

                ViewBag.nc_status = obj.GetMultiAuditStatusList();
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.Findingcategory = obj.GetMultiFindingCategoryList();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddExternalAuditWithNoFindings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExternalAuditWithNoFindings(ExternalAuditModels objExternalAudit, FormCollection form, HttpPostedFileBase upload)
        {
            try
            {
                //if (ModelState.IsValid)
                if (objExternalAudit != null)
                {

                    string sCertBodyList = form["CertID"];
                    string sauditor_name = form["auditor_name"];

                    objExternalAudit.CertID = sCertBodyList;
                    objExternalAudit.auditor_name = sauditor_name;
                    objExternalAudit.AuditLocation = form["AuditLocation"];

                    DateTime dateValue;

                    if (DateTime.TryParse(form["AuditDate"], out dateValue) == true)
                    {
                        objExternalAudit.AuditDate = dateValue;
                    }

                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = System.IO.Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), System.IO.Path.GetFileName(upload.FileName));
                            string sFilename = objExternalAudit.CertID + "_" + System.IO.Path.GetFileName(spath);
                            string sFilepath = System.IO.Path.GetDirectoryName(spath);

                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objExternalAudit.upload = "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddExternalAuditWithNoFindings-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    if (objExternalAudit.FunAddExternalAuditWithNoFindings(objExternalAudit, upload))
                    {
                        TempData["Successdata"] = "External audit details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddExternalAuditWithNoFindings: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ExternalAuditList");
        }

        private ActionResult InitilizeAddExternalAudit()
        {
            ExternalAuditModels obj = new ExternalAuditModels();

            try
            {
                //ViewBag.CertBodyList = objCertBody.FunGetCertificateList();
                ViewBag.nc_status = obj.GetMultiAuditStatusList();
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptList = objGlobaldata.GetDepartmentWithIdListbox();
                ViewBag.Findingcategory = obj.GetMultiFindingCategoryList();

                obj.AuditLocation = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddExternalAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(obj);
        }

        public ActionResult FunGetExternalAuditorsList(string sCertId)
        {
            ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();

            List<string> lstAuditors = objExternalAuditModels.FunGetExternalAuditorsListbox(sCertId);
            return Json(lstAuditors);
        }



        [AllowAnonymous]
        public JsonResult ExternalAuditListSearch(string branch_name)
        {
            ExternalAuditModelsList objExternalAuditModelsList = new ExternalAuditModelsList();
            objExternalAuditModelsList.ExternalAuditList = new List<ExternalAuditModels>();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "SELECT  AuditID, AuditNum, auditor_name, CertID, AuditDate, MajorFindingsNo, MinorFindingNo, ObservationNo, AuditLocation,upload FROM t_external_audit as TEA where Active=1";

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

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
                {


                    DateTime AuditDateVal = Convert.ToDateTime(dsCertificationBodyList.Tables[0].Rows[i]["AuditDate"].ToString());
                    int MajorFindingsNo = 0, MinorFindingNo = 0, ObservationNo = 0;

                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[i]["MajorFindingsNo"].ToString(), out MajorFindingsNo);
                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[i]["MinorFindingNo"].ToString(), out MinorFindingNo);
                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[i]["ObservationNo"].ToString(), out ObservationNo);
                    try
                    {
                        ExternalAuditModels objExternalAuditorModels = new ExternalAuditModels
                        {
                            AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsCertificationBodyList.Tables[0].Rows[i]["AuditLocation"].ToString()),
                            AuditID = dsCertificationBodyList.Tables[0].Rows[i]["AuditID"].ToString(),
                            CertID = dsCertificationBodyList.Tables[0].Rows[i]["CertID"].ToString(),
                            AuditNum = dsCertificationBodyList.Tables[0].Rows[i]["AuditNum"].ToString(),
                            auditor_name = dsCertificationBodyList.Tables[0].Rows[i]["auditor_name"].ToString(),
                            //Report = dsCertificationBodyList.Tables[0].Rows[i]["Report"].ToString(),
                            upload = dsCertificationBodyList.Tables[0].Rows[i]["upload"].ToString(),
                            AuditDate = AuditDateVal,
                            MajorFindingsNo = MajorFindingsNo,
                            MinorFindingNo = MinorFindingNo,
                            ObservationNo = ObservationNo
                        };

                        objExternalAuditModelsList.ExternalAuditList.Add(objExternalAuditorModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in ExternalAuditListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        //[AllowAnonymous]
        //public ActionResult ExternalAuditDetails()
        //{

        //    ExternalAuditModels objExternalAuditorModels = new ExternalAuditModels();
        //    try
        //    {
        //        if (Request.QueryString["AuditID"] != null && Request.QueryString["AuditID"] != "")
        //        {
        //            string sAuditID = Request.QueryString["AuditID"];

        //            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

        //            //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
        //            string sSqlstmt = "SELECT  AuditID, AuditNum, CertID, AuditDate, auditor_name, MajorFindingsNo, MinorFindingNo, ObservationNo, AuditLocation,remarks,upload FROM t_external_audit as TEA"
        //                + " where AuditID=" + sAuditID;

        //            DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);

        //            //for (int i = 0; i < dsCertificationBodyList.Tables[0].Rows.Count; i++)
        //            //{
        //            if (dsCertificationBodyList.Tables.Count > 0 && dsCertificationBodyList.Tables[0].Rows.Count > 0)
        //            {
        //                DateTime AuditDateVal = Convert.ToDateTime(dsCertificationBodyList.Tables[0].Rows[0]["AuditDate"].ToString());
        //                int MajorFindingsNo = 0, MinorFindingNo = 0, ObservationNo = 0;

        //                int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["MajorFindingsNo"].ToString(), out MajorFindingsNo);
        //                int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["MinorFindingNo"].ToString(), out MinorFindingNo);
        //                int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["ObservationNo"].ToString(), out ObservationNo);

        //                objExternalAuditorModels = new ExternalAuditModels
        //                {
        //                    AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsCertificationBodyList.Tables[0].Rows[0]["AuditLocation"].ToString()),
        //                    AuditID = dsCertificationBodyList.Tables[0].Rows[0]["AuditID"].ToString(),
        //                    CertID = dsCertificationBodyList.Tables[0].Rows[0]["CertID"].ToString(),
        //                    AuditNum = dsCertificationBodyList.Tables[0].Rows[0]["AuditNum"].ToString(),
        //                    auditor_name = dsCertificationBodyList.Tables[0].Rows[0]["auditor_name"].ToString(),
        //                    remarks = dsCertificationBodyList.Tables[0].Rows[0]["remarks"].ToString(),
        //                    upload = dsCertificationBodyList.Tables[0].Rows[0]["upload"].ToString(),
        //                    AuditDate = AuditDateVal,
        //                    MajorFindingsNo = MajorFindingsNo,
        //                    MinorFindingNo = MinorFindingNo,
        //                    ObservationNo = ObservationNo
        //                };
        //                sSqlstmt = "select ExtAuditTransID, ExternalAuditID, NCNo, tsat.DeptId,AuditFindingDesc, FindingCategory,"
        //                +"CorrectionTaken, CorrectionDate, ProposedcorrectiveAction,CorrectiveActionDate, Action_taken_by, nc_status,"
        //                + "reviewed_by,team from t_external_audit_trans as tsat where ExternalAuditID='" + objExternalAuditorModels.AuditID + "' order by ExtAuditTransID desc";
        //                ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
        //            }
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "Audit Id cannot be Null or empty";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in ExternalAuditDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objExternalAuditorModels);
        //}

        [AllowAnonymous]
        public ActionResult ExternalAuditInfo(int id)
        {

            ExternalAuditModels objExternalAuditorModels = new ExternalAuditModels();
            try
            {
                string sSqlstmt = "SELECT  AuditID, AuditNum, CertID, AuditDate, auditor_name, MajorFindingsNo, MinorFindingNo, ObservationNo, AuditLocation,remarks FROM t_external_audit as TEA"
                    + " where AuditID=" + id;

                DataSet dsCertificationBodyList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCertificationBodyList.Tables.Count > 0 && dsCertificationBodyList.Tables[0].Rows.Count > 0)
                {
                    DateTime AuditDateVal = Convert.ToDateTime(dsCertificationBodyList.Tables[0].Rows[0]["AuditDate"].ToString());
                    int MajorFindingsNo = 0, MinorFindingNo = 0, ObservationNo = 0;

                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["MajorFindingsNo"].ToString(), out MajorFindingsNo);
                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["MinorFindingNo"].ToString(), out MinorFindingNo);
                    int.TryParse(dsCertificationBodyList.Tables[0].Rows[0]["ObservationNo"].ToString(), out ObservationNo);

                    objExternalAuditorModels = new ExternalAuditModels
                    {
                        AuditLocation = objGlobaldata.GetMultiCompanyBranchNameById(dsCertificationBodyList.Tables[0].Rows[0]["AuditLocation"].ToString()),
                        AuditID = dsCertificationBodyList.Tables[0].Rows[0]["AuditID"].ToString(),
                        CertID = dsCertificationBodyList.Tables[0].Rows[0]["CertID"].ToString(),
                        AuditNum = dsCertificationBodyList.Tables[0].Rows[0]["AuditNum"].ToString(),
                        auditor_name = dsCertificationBodyList.Tables[0].Rows[0]["auditor_name"].ToString(),
                        remarks = dsCertificationBodyList.Tables[0].Rows[0]["remarks"].ToString(),
                        AuditDate = AuditDateVal,
                        MajorFindingsNo = MajorFindingsNo,
                        MinorFindingNo = MinorFindingNo,
                        ObservationNo = ObservationNo
                    };

                    sSqlstmt = "select ExtAuditTransID, ExternalAuditID, NCNo, tsat.DeptId,  AuditFindingDesc,FindingCategory,"
                    + "CorrectionTaken, CorrectionDate,ProposedcorrectiveAction,CorrectiveActionDate, Action_taken_by, nc_status,"
                    + "reviewed_by,team from t_external_audit_trans as tsat where ExternalAuditID='" + objExternalAuditorModels.AuditID + "' order by ExtAuditTransID desc;";

                    ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objExternalAuditorModels);
        }


        //POST: /ExternalAudit/ExternalAuditUpdate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ExternalAuditFindingsUpdate(ExternalAuditModels objExternalAuditModels, FormCollection form)
        {
            try
            {
                objExternalAuditModels.AuditID = form["AuditID"];
                objExternalAuditModels.ExternalAuditID = form["ExtAuditTransID"];
                objExternalAuditModels.DeptId = form["DeptId"];
                objExternalAuditModels.Action_taken_by = form["Action_taken_by"];
                objExternalAuditModels.reviewed_by = form["reviewed_by"];
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();

                if (objExternalAuditModels.FunUpdateExternalAuditFindings(objExternalAuditModels))
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
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditFindingsUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return false;
        }


        //POST: /ExternalAudit/ExternalAuditUpdate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ExternalAuditFindingsAdd(ExternalAuditModels objExternalAuditModels, FormCollection form)
        {
            try
            {
                ExternalAuditModelsList objExternalAuditModelsList = new ExternalAuditModelsList();
                objExternalAuditModelsList.ExternalAuditList = new List<ExternalAuditModels>();


                objExternalAuditModels.AuditID = form["AuditID"];

                objExternalAuditModels.DeptId = form["DeptId"];
                // objExternalAuditModels.ExternalAuditID = form["ExtAuditTransID"];

                objExternalAuditModelsList.ExternalAuditList.Add(objExternalAuditModels);
                //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();
                return objExternalAuditModels.FunAddExternalAuditFindings(objExternalAuditModelsList, Convert.ToInt16(objExternalAuditModels.AuditID));
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ExternalAuditFindingsAdd: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return false;
        }
    }
}
