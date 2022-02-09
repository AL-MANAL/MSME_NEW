using ISOStd.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class AuditLogController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public AuditLogController()
        {
            ViewBag.Menutype = "Audit";

        }
        [AllowAnonymous]
        public ActionResult AuditLogList(string branch_name, string Audit_no, string dept_name)
        {
            ViewBag.SubMenutype = "AuditLog";
            AuditLogModelsList objAttList = new AuditLogModelsList();
            objAttList.LogList = new List<AuditLogModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);
                ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                ViewBag.AuditNo = objGlobaldata.GetAuditListforAuditLog();

                string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,followup_date,auditors,auditee_team,risk_level,rca_details,ca_notifiedto,nc_team"
                + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                    ViewBag.Branch_name = sBranch_name;
                }
                //if (Audit_no != null && Audit_no != "")
                //{
                //    sSqlstmt = sSqlstmt + " and find_in_set('" + Audit_no + "',Audit_no)";
                //    ViewBag.sAudit_Id = Audit_no;
                //}
                if (dept_name != null && dept_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + dept_name + "', dept_name)";
                    ViewBag.Department_name = dept_name;
                }

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
                                Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
                                nc_no = dsList.Tables[0].Rows[i]["nc_no"].ToString(),
                                branch = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[i]["branch"].ToString()),
                                dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept_name"].ToString()),
                                //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[i]["team"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                Audit_Status = objGlobaldata.GetAuditStatusById(dsList.Tables[0].Rows[i]["Audit_Status"].ToString()),
                                finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[i]["finding_category"].ToString()),
                                auditors = dsList.Tables[0].Rows[i]["auditors"].ToString(),
                                auditee_team = dsList.Tables[0].Rows[i]["auditee_team"].ToString(),
                                risk_level = dsList.Tables[0].Rows[i]["risk_level"].ToString(),
                                rca_details = dsList.Tables[0].Rows[i]["rca_details"].ToString(),
                                ca_notifiedto = dsList.Tables[0].Rows[i]["ca_notifiedto"].ToString(),
                                nc_team = dsList.Tables[0].Rows[i]["nc_team"].ToString(),

                                auditors_name = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["auditors"].ToString()),
                                auditee_team_name = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["auditee_team"].ToString()),

                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["AuditDate"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["AuditDate"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.AuditDate = dtDocDate;
                            }
                            if (dsList.Tables[0].Rows[i]["corrective_action_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["corrective_action_date"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.corrective_action_date = dtDocDate;
                            }
                            if (dsList.Tables[0].Rows[i]["followup_date"].ToString() != ""
                            && DateTime.TryParse(dsList.Tables[0].Rows[i]["followup_date"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.followup_date = dtDocDate;
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
                    string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,finding_details,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate,risk_nc,risk_level,disp_upload"
               + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1 and id_nc='" + sid_nc + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["Audit_Status"].ToString()),
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
                        if (dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_action_date = dtDocDate;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["disp_notifeddate"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["disp_notifeddate"].ToString(), out dtDocDate))
                        {
                            objModel.disp_notifeddate = dtDocDate;
                        }
                        AuditLogModelsList NcDispList = new AuditLogModelsList();
                        NcDispList.LogList = new List<AuditLogModels>();

                        string sSqlstmt1 = "select id_audit_nc_disp_action,id_nc,disp_action" +
                            ",disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_nc='" + sid_nc + "'";
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
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
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

                if (objModel.FunUpdateDisposition(objModel, objModelList))
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


                    string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,finding_details,"
                        + "rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate,rca_started_date"
                        + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["Audit_Status"].ToString()),
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
                        if (dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                         && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_action_date = dtValue;
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
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
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

                if (objModel.FunUpdateRCA(objModel, objModelList))
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

                    string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,finding_details,"
                        + "ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date"
                        + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["Audit_Status"].ToString()),
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
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                       && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_action_date = dtValue;
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
                        "ca_target_date,ca_resp_person from t_audit_nc_ca where id_nc = '" + sid_nc + "' and ca_active=1";

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

                if (objModel.FunUpdateCA(objModel, objModelList))
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


                    string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,finding_details,"
                       + "nc_team,team_approvedby,team_notifiedto,team_targetdate"
                       + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["Audit_Status"].ToString()),
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
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                       && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_action_date = dtValue;
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

                if (objModel.FunUpdateTeam(objModel))
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

                    string sSqlstmt = "select T3.id_nc,Audit_no,nc_no,AuditDate,branch,dept_name,team,location,Audit_Status,finding_category,corrective_action_date,finding_details,"
                     + "v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload,v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto,v_eleminate_root_cause,v_risk_reduce,v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason"
                     + " from t_audit_process T1,t_audit_process_plan T2, t_audit_process_nc T3 where T1.Audit_Id = T2.Audit_Id and T2.Plan_Id = T3.Plan_Id and active = 1 and id_nc='" + sid_nc + "'";


                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsNCModels.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_no = dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsNCModels.Tables[0].Rows[0]["nc_no"].ToString(),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsNCModels.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                            Audit_Status = objGlobaldata.GetAuditStatusById(dsNCModels.Tables[0].Rows[0]["Audit_Status"].ToString()),
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
                        if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                        if (dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                      && DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtValue))
                        {
                            objModel.corrective_action_date = dtValue;
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
                       "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_audit_nc_ca where id_nc = '" + sid_nc + "' and ca_active=1";

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

                IList<HttpPostedFileBase> v_uploadList = (IList<HttpPostedFileBase>)v_upload;
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (v_uploadList[0] != null)
                {
                    objModel.v_upload = "";
                    foreach (var file in v_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
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

                if (objModel.FunUpdateVerification(objModel, objModelList))
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

                    string sSqlstmt = "select id_nc,T3.Plan_Id,T1.Audit_Id,Audit_no,nc_no,nc_date,finding_details,upload,finding_category,T1.Audit_criteria,audit_clause,description,T1.logged_by,T1.logged_date,aprvrejct_date,corrective_action_date,reason_rejection,upload_evidence,apprvby_auditee,followup_date,AuditDate,auditee_team,branch,dept_name,team,disp_action_taken,"
                        + "disp_action_taken,disp_explain,risk_nc,risk_level,rca_details,rca_technique,rca_reporteddate,rca_action,rca_justify,ca_proposed_by,"
                        + "v_verifiedto,v_verified_date,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_eleminate_root_cause,v_risk_reduce,v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason,v_status,v_closed_date,"
                      + "(CASE WHEN apprv_status = '0' THEN 'Pending for Auditee Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + " from t_audit_process_nc T1,t_audit_process T2,t_audit_process_plan T3 where T1.Audit_Id = T2.Audit_Id and T2.Audit_Id=T3.Audit_Id and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsList.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_Id = dsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Plan_Id = dsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_no = dsList.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),

                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            audit_clause = objMgmt.GetMultiISOClauseNameById(dsList.Tables[0].Rows[0]["audit_clause"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            apprv_status = dsList.Tables[0].Rows[0]["apprv_status"].ToString(),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["auditee_team"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[0]["team"].ToString()),

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
                        if (dsList.Tables[0].Rows[0]["nc_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["nc_date"].ToString(), out dtDocDate))
                        {
                            objModel.nc_date = dtDocDate;
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
                        if (dsList.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_action_date = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["followup_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
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


                        string sql1 = "select disp_action,disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_nc='" + sid_nc + "'";
                        ViewBag.ImAction = objGlobaldata.Getdetails(sql1);

                        string sql2 = "select ca_ca,ca_resp_person,ca_target_date from t_audit_nc_ca where id_nc='" + sid_nc + "'";
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
            string header = Server.MapPath("~/views/AuditProcess/NCPrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                "--header-spacing \"0\" " +
            " --header-font-size \"10\" ", header);

            return new ViewAsPdf("NonconformityReportPDF", "AuditProcess")
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


                    string sSqlstmt = "select id_nc,T3.Plan_Id,T1.Audit_Id,Audit_no,nc_no,nc_date,finding_details,upload,finding_category,T1.Audit_criteria,audit_clause,description,T1.logged_by,T1.logged_date,aprvrejct_date,corrective_action_date,reason_rejection,upload_evidence,apprvby_auditee,followup_date,AuditDate,auditee_team,branch,dept_name,team,disp_action_taken,"
                        + "disp_action_taken,disp_explain,risk_nc,risk_level,rca_details,rca_technique,rca_reporteddate,rca_action,rca_justify,ca_proposed_by,"
                        + "v_verifiedto,v_verified_date,v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_eleminate_root_cause,v_risk_reduce,v_risk_reduce_reason,v_process_amended,v_document_amended,v_amend_docref,v_amend_docname,v_amend_docdate,v_ncr_reason,v_status,v_closed_date,"
                      + "(CASE WHEN apprv_status = '0' THEN 'Pending for Auditee Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + " from t_audit_process_nc T1,t_audit_process T2,t_audit_process_plan T3 where T1.Audit_Id = T2.Audit_Id and T2.Audit_Id=T3.Audit_Id and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditLogModels
                        {
                            id_nc = dsList.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_Id = dsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Plan_Id = dsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_no = dsList.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),

                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            audit_clause = objMgmt.GetMultiISOClauseNameById(dsList.Tables[0].Rows[0]["audit_clause"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            apprv_status = dsList.Tables[0].Rows[0]["apprv_status"].ToString(),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["auditee_team"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[0]["team"].ToString()),

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
                        if (dsList.Tables[0].Rows[0]["nc_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["nc_date"].ToString(), out dtDocDate))
                        {
                            objModel.nc_date = dtDocDate;
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
                        if (dsList.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_action_date = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["followup_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
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


                        string sql1 = "select disp_action,disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_nc='" + sid_nc + "'";
                        ViewBag.ImAction = objGlobaldata.Getdetails(sql1);

                        string sql2 = "select ca_ca,ca_resp_person,ca_target_date from t_audit_nc_ca where id_nc='" + sid_nc + "'";
                        ViewBag.CA = objGlobaldata.Getdetails(sql2);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //--------------------------------------------------------------------
        //[AllowAnonymous]
        //public ActionResult AuditLogList(string branch_name,string Audit_no, string dept_name)
        //{
        //    AuditProcessModelsList objAttList = new AuditProcessModelsList();
        //    objAttList.Obj = new List<AuditProcessModels>();

        //    try
        //    {
        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);
        //        ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
        //        ViewBag.AuditNo = objGlobaldata.GetAuditListforAuditLog();
        //        ViewBag.Department = objGlobaldata.GetDepartmentListbox();

        //        string sSqlstmt = "select a.Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no," +
        //                      " AuditPlanDate, AuditDate, dept,team,  location, division,internal_audit_team, " +
        //                      "Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,Audit_Elements," +
        //                      "id_audit_process_perform,Details,Evidence,Category,Conformance,Non_comformance,evidence_upload," +
        //                      "why_nc,auditee_acceptance,ca_date,followup_date from t_audit_process a," +
        //                      " t_audit_process_perform b where a.Audit_Id = b.Audit_Id and a.active = 1 and followup_date !='' and nc_status='Close' ";


        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', division)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', division)";
        //        }
        //        if (Audit_no != null && Audit_no != "")
        //        {
        //            sSqlstmt = sSqlstmt + " and find_in_set('" + Audit_no + "',a. Audit_Id)";
        //            ViewBag.sAudit_Id = Audit_no;
        //        }                
        //        if (dept_name != null && dept_name != "")
        //        {
        //            sSqlstmt = sSqlstmt + " and find_in_set('" + dept_name + "', dept)";
        //            ViewBag.Department_name = dept_name;
        //        }                

        //        sSqlstmt = sSqlstmt + " order by Audit_Id desc";

        //        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
        //            {
        //                try
        //                {
        //                    AuditProcessModels objScheduleMdl = new AuditProcessModels
        //                    {
        //                        Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
        //                        Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
        //                        Non_comformance = dsList.Tables[0].Rows[i]["Non_comformance"].ToString(),
        //                        location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
        //                        dept = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
        //                        division = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[i]["division"].ToString()),
        //                        team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[i]["team"].ToString()),
        //                        Audit_Status = dsList.Tables[0].Rows[i]["Audit_Status"].ToString(),
        //                        Details = dsList.Tables[0].Rows[i]["Details"].ToString(),
        //                        Category = dsList.Tables[0].Rows[i]["Category"].ToString(),
        //                        why_nc = dsList.Tables[0].Rows[i]["why_nc"].ToString(),
        //                        id_audit_process_perform = dsList.Tables[0].Rows[i]["id_audit_process_perform"].ToString()
        //                    };

        //                    DateTime dtDocDate;
        //                    if (dsList.Tables[0].Rows[i]["AuditDate"].ToString() != ""
        //                     && DateTime.TryParse(dsList.Tables[0].Rows[i]["AuditDate"].ToString(), out dtDocDate))
        //                    {
        //                        objScheduleMdl.AuditDate = dtDocDate;
        //                    }

        //                    if (dsList.Tables[0].Rows[i]["ca_date"].ToString() != ""
        //                     && DateTime.TryParse(dsList.Tables[0].Rows[i]["ca_date"].ToString(), out dtDocDate))
        //                    {
        //                        objScheduleMdl.ca_date = dtDocDate;
        //                    }

        //                    if (dsList.Tables[0].Rows[i]["followup_date"].ToString() != ""
        //                     && DateTime.TryParse(dsList.Tables[0].Rows[i]["followup_date"].ToString(), out dtDocDate))
        //                    {
        //                        objScheduleMdl.followup_date = dtDocDate;
        //                    }

        //                    objAttList.Obj.Add(objScheduleMdl);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in AuditLogList: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AuditLogList: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objAttList.Obj.ToList());
        //}

        //Disposition
        //[AllowAnonymous]
        //public ActionResult AddDisposition()
        //{
        //    AuditLogModels objModel = new AuditLogModels();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            ViewBag.id_audit_process_perform = sid_audit_process_perform;

        //            NCModels objNCModel = new NCModels();
        //            ViewBag.DispositonAction = objNCModel.GetNCDispositionActionList();
        //            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location," +
        //                "Category,Non_comformance,Details,Evidence,disp_action_taken,disp_explain,disp_notifiedto,id_audit_process_perform from" +
        //                " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditLogModels
        //                {
        //                    Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                    Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                    divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                    Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                    Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                    Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                    Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                    disp_action_taken = (dsNCModels.Tables[0].Rows[0]["disp_action_taken"].ToString()),
        //                    disp_explain = (dsNCModels.Tables[0].Rows[0]["disp_explain"].ToString()),
        //                    disp_notifiedto = (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()),
        //                };
        //                if (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString() != "")
        //                {
        //                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["disp_notifiedto"].ToString()).Split(',');
        //                }
        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objModel.AuditDate = dtValue;
        //                }

        //                AuditLogModelsList NcDispList = new AuditLogModelsList();
        //                NcDispList.LogList = new List<AuditLogModels>();

        //                string sSqlstmt1 = "select id_audit_nc_disp_action,id_audit_process_perform,disp_action" +
        //                    ",disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //                DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt1);

        //                if (dsDispModels.Tables.Count > 0 && dsDispModels.Tables[0].Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dsDispModels.Tables[0].Rows.Count; i++)
        //                    {
        //                        try
        //                        {
        //                            AuditLogModels objDispModel = new AuditLogModels
        //                            {
        //                                id_audit_nc_disp_action = (dsDispModels.Tables[0].Rows[i]["id_audit_nc_disp_action"].ToString()),
        //                                disp_action = (dsDispModels.Tables[0].Rows[i]["disp_action"].ToString()),
        //                                disp_resp_person = (dsDispModels.Tables[0].Rows[i]["disp_resp_person"].ToString()),
        //                            };

        //                            if (DateTime.TryParse(dsDispModels.Tables[0].Rows[i]["disp_complete_date"].ToString(), out dtValue))
        //                            {
        //                                objDispModel.disp_complete_date = dtValue;
        //                            }
        //                            NcDispList.LogList.Add(objDispModel);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
        //                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                        }
        //                        ViewBag.NcDispList = NcDispList;
        //                    }
        //                }
        //                return View(objModel);
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("AuditLogList");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("AuditLogList");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult AddDisposition(AuditLogModels objModel, FormCollection form)
        //{
        //    try
        //    {
        //        AuditLogModelsList objModelList = new AuditLogModelsList();
        //        objModelList.LogList = new List<AuditLogModels>();

        //        DateTime dateValue;

        //        if (DateTime.TryParse(form["disp_notifeddate"], out dateValue) == true)
        //        {
        //            objModel.disp_notifeddate = dateValue;
        //        }


        //        //Notified To
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
        //        {
        //            if (form["nempno " + i] != "" && form["nempno " + i] != null)
        //            {
        //                objModel.disp_notifiedto = objModel.disp_notifiedto + "," + form["nempno " + i];
        //            }
        //        }
        //        if (objModel.disp_notifiedto != null)
        //        {
        //            objModel.disp_notifiedto = objModel.disp_notifiedto.Trim(',');
        //        }

        //        for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
        //        {
        //            AuditLogModels objNCModel = new AuditLogModels();
        //            if (form["disp_action " + i] != "" && form["disp_action " + i] != null)
        //            {
        //                objNCModel.disp_action = form["disp_action " + i];
        //                objNCModel.disp_resp_person = form["disp_resp_person " + i];
        //                if (DateTime.TryParse(form["disp_complete_date " + i], out dateValue) == true)
        //                {
        //                    objNCModel.disp_complete_date = dateValue;
        //                }
        //                objModelList.LogList.Add(objNCModel);
        //            }
        //        }

        //        if (objModel.FunUpdateDisposition(objModel, objModelList))
        //        {
        //            TempData["Successdata"] = "Added Disposition details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddDisposition: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(true);
        //}

        //Team
        //[AllowAnonymous]
        //public ActionResult AddTeam()
        //{
        //    AuditLogModels objModel = new AuditLogModels();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            ViewBag.id_audit_process_perform = sid_audit_process_perform;

        //            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location," +
        //               "Category,Non_comformance,Details,Evidence,id_audit_process_perform,nc_team,team_approvedby,team_notifiedto,team_targetdate from" +
        //               " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";
        //              DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditLogModels
        //                {
        //                    Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                    Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                    divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                    Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                    Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                    Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                    Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                    nc_team = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()),
        //                    team_approvedby = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()),
        //                    team_notifiedto = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()),
        //                };
        //                if (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString() != "")
        //                {
        //                    ViewBag.TeamArray = (dsNCModels.Tables[0].Rows[0]["nc_team"].ToString()).Split(',');
        //                }

        //                if (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString() != "")
        //                {
        //                    ViewBag.ApprovedByArray = (dsNCModels.Tables[0].Rows[0]["team_approvedby"].ToString()).Split(',');
        //                }

        //                if (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString() != "")
        //                {
        //                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["team_notifiedto"].ToString()).Split(',');
        //                }

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objModel.AuditDate = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["team_targetdate"].ToString(), out dtValue))
        //                {
        //                    objModel.team_targetdate = dtValue;
        //                }
        //                return View(objModel);
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("AuditLogList");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("AuditLogList");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult AddTeam(AuditLogModels objModel, FormCollection form)
        //{
        //    try
        //    {
        //        //AuditLogModelsList objModelList = new AuditLogModelsList();
        //        //objModelList.LogList = new List<AuditLogModels>();

        //        DateTime dateValue;

        //        if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
        //        {
        //            objModel.team_targetdate = dateValue;
        //        }

        //        //Team
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
        //        {
        //            if (form["tempno " + i] != "" && form["tempno " + i] != null)
        //            {
        //                objModel.nc_team = objModel.nc_team + "," + form["tempno " + i];
        //            }
        //        }
        //        if (objModel.nc_team != null)
        //        {
        //            objModel.nc_team = objModel.nc_team.Trim(',');
        //        }

        //        //ApprovedBy
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
        //        {
        //            if (form["aempno " + i] != "" && form["aempno " + i] != null)
        //            {
        //                objModel.team_approvedby = objModel.team_approvedby + "," + form["aempno " + i];
        //            }
        //        }
        //        if (objModel.team_approvedby != null)
        //        {
        //            objModel.team_approvedby = objModel.team_approvedby.Trim(',');
        //        }


        //        //Notified To
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
        //        {
        //            if (form["nempno " + i] != "" && form["nempno " + i] != null)
        //            {
        //                objModel.team_notifiedto = objModel.team_notifiedto + "," + form["nempno " + i];
        //            }
        //        }
        //        if (objModel.team_notifiedto != null)
        //        {
        //            objModel.team_notifiedto = objModel.team_notifiedto.Trim(',');
        //        }

        //        if (objModel.FunUpdateTeam(objModel))
        //        {
        //            TempData["Successdata"] = "Added Team details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddTeam: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(true);
        //}

        ////RCA
        //public ActionResult AddRCA()
        //{
        //    AuditLogModels objModel = new AuditLogModels();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            ViewBag.id_audit_process_perform = sid_audit_process_perform;

        //            NCModels objNCModel = new NCModels();
        //            ViewBag.RCATechniqueList = objNCModel.GetRCATechniqueList();
        //            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
        //            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location," +
        //                      "Category,Non_comformance,Details,Evidence,id_audit_process_perform," +
        //                      "rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate from" +
        //                      " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";

        //           DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditLogModels
        //                {
        //                    Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                    Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                    divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                    Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                    Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                    Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                    Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                    rca_technique = (dsNCModels.Tables[0].Rows[0]["rca_technique"].ToString()),
        //                    rca_details = (dsNCModels.Tables[0].Rows[0]["rca_details"].ToString()),
        //                    rca_upload = (dsNCModels.Tables[0].Rows[0]["rca_upload"].ToString()),
        //                    rca_action = (dsNCModels.Tables[0].Rows[0]["rca_action"].ToString()),
        //                    rca_justify = (dsNCModels.Tables[0].Rows[0]["rca_justify"].ToString()),
        //                    rca_reportedby = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()),
        //                    rca_notifiedto = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()),
        //                };

        //                if (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString() != "")
        //                {
        //                    ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["rca_reportedby"].ToString()).Split(',');
        //                }
        //                if (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString() != "")
        //                {
        //                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["rca_notifiedto"].ToString()).Split(',');
        //                }

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objModel.AuditDate = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["rca_reporteddate"].ToString(), out dtValue))
        //                {
        //                    objModel.rca_reporteddate = dtValue;
        //                }

        //                return View(objModel);
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("AuditLogList");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("AuditLogList");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult AddRCA(AuditLogModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> rca_upload)
        //{
        //    try
        //    {
        //        AuditLogModelsList objModelList = new AuditLogModelsList();
        //        objModelList.LogList = new List<AuditLogModels>();

        //        DateTime dateValue;

        //        if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
        //        {
        //            objModel.team_targetdate = dateValue;
        //        }

        //        IList<HttpPostedFileBase> rca_uploadList = (IList<HttpPostedFileBase>)rca_upload;
        //        string QCDelete = Request.Form["QCDocsValselectall"];

        //        if (rca_uploadList[0] != null)
        //        {
        //            objModel.rca_upload = "";
        //            foreach (var file in rca_upload)
        //            {
        //                try
        //                {
        //                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
        //                    string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
        //                    file.SaveAs(sFilepath + "/" + sFilename);
        //                    objModel.rca_upload = objModel.rca_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in AddRCA-upload: " + ex.ToString());
        //                }
        //            }
        //            objModel.rca_upload = objModel.rca_upload.Trim(',');
        //        }
        //        else
        //        {
        //            ViewBag.Message = "You have not specified a file.";
        //        }
        //        if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
        //        {
        //            objModel.rca_upload = objModel.rca_upload + "," + form["QCDocsVal"];
        //            objModel.rca_upload = objModel.rca_upload.Trim(',');
        //        }
        //        else if (form["QCDocsVal"] == null && QCDelete != null && rca_uploadList[0] == null)
        //        {
        //            objModel.rca_upload = null;
        //        }
        //        else if (form["QCDocsVal"] == null && rca_uploadList[0] == null)
        //        {
        //            objModel.rca_upload = null;
        //        }

        //        //Reported By
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
        //        {
        //            if (form["aempno " + i] != "" && form["aempno " + i] != null)
        //            {
        //                objModel.rca_reportedby = objModel.rca_reportedby + "," + form["aempno " + i];
        //            }
        //        }
        //        if (objModel.rca_reportedby != null)
        //        {
        //            objModel.rca_reportedby = objModel.rca_reportedby.Trim(',');
        //        }


        //        //Notifed To
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
        //        {
        //            if (form["nempno " + i] != "" && form["nempno " + i] != null)
        //            {
        //                objModel.rca_notifiedto = objModel.rca_notifiedto + "," + form["nempno " + i];
        //            }
        //        }
        //        if (objModel.rca_notifiedto != null)
        //        {
        //            objModel.rca_notifiedto = objModel.rca_notifiedto.Trim(',');
        //        }

        //        if (objModel.FunUpdateRCA(objModel, objModelList))
        //        {
        //            TempData["Successdata"] = "Added Root Cause Analysis details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddRCA: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(true);
        //}

        //Corrective Action
        //public ActionResult AddCA()
        //{
        //    AuditLogModels objModel = new AuditLogModels();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            ViewBag.id_audit_process_perform = sid_audit_process_perform;

        //            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
        //            //ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
        //            //ViewBag.NCRStatus = objModel.GetNCRStatusList();
        //            //ViewBag.Action = objModel.GetNCActionImplementList();
        //            ViewBag.Division = objGlobaldata.GetCompanyBranchListbox();
        //            ViewBag.Department = objGlobaldata.GetDepartmentListbox();
        //            ViewBag.Location = objGlobaldata.GetLocationListBox();
        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location," +
        //                      "Category,Non_comformance,Details,Evidence,disp_action_taken,disp_explain,disp_notifiedto,id_audit_process_perform," +
        //                      "ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from" +
        //                      " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";

        //           DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditLogModels
        //                {
        //                    Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                    Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                    divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                    Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                    Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                    Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                    Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                    ca_proposed_by = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()),
        //                    ca_notifiedto = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()),
        //                };
        //                if (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString() != "")
        //                {
        //                    ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["ca_proposed_by"].ToString()).Split(',');
        //                }
        //                if (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString() != "")
        //                {
        //                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["ca_notifiedto"].ToString()).Split(',');
        //                }

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objModel.AuditDate = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_verfiry_duedate"].ToString(), out dtValue))
        //                {
        //                    objModel.ca_verfiry_duedate = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["ca_notifed_date"].ToString(), out dtValue))
        //                {
        //                    objModel.ca_notifed_date = dtValue;
        //                }


        //                AuditLogModelsList CAList = new AuditLogModelsList();
        //                CAList.LogList = new List<AuditLogModels>();

        //                string Ssql = "select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
        //                "ca_target_date,ca_resp_person from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";

        //                DataSet dsCALsit = objGlobaldata.Getdetails(Ssql);

        //                if (dsCALsit.Tables.Count > 0 && dsCALsit.Tables[0].Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dsCALsit.Tables[0].Rows.Count; i++)
        //                    {
        //                        try
        //                        {
        //                            AuditLogModels objNCModels = new AuditLogModels
        //                            {
        //                                id_audit_nc_ca = (dsCALsit.Tables[0].Rows[i]["id_audit_nc_ca"].ToString()),
        //                                ca_div = (dsCALsit.Tables[0].Rows[i]["ca_div"].ToString()),
        //                                ca_loc = (dsCALsit.Tables[0].Rows[i]["ca_loc"].ToString()),
        //                                ca_dept = (dsCALsit.Tables[0].Rows[i]["ca_dept"].ToString()),
        //                                ca_rootcause = (dsCALsit.Tables[0].Rows[i]["ca_rootcause"].ToString()),
        //                                ca_ca = (dsCALsit.Tables[0].Rows[i]["ca_ca"].ToString()),
        //                                ca_resource = (dsCALsit.Tables[0].Rows[i]["ca_resource"].ToString()),
        //                                ca_resp_person = (dsCALsit.Tables[0].Rows[i]["ca_resp_person"].ToString()),
        //                            };
        //                            if (DateTime.TryParse(dsCALsit.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
        //                            {
        //                                objNCModels.ca_target_date = dtValue;
        //                            }

        //                            CAList.LogList.Add(objNCModels);


        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            objGlobaldata.AddFunctionalLog("Exception in NCEdit: " + ex.ToString());
        //                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                        }
        //                    }
        //                }
        //                ViewBag.CorrectiveAction = CAList;

        //                return View(objModel);
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("AuditLogList");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("AuditLogList");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult AddCA(AuditLogModels objModel, FormCollection form)
        //{
        //    try
        //    {
        //        AuditLogModelsList objModelList = new AuditLogModelsList();
        //        objModelList.LogList = new List<AuditLogModels>();

        //        DateTime dateValue;

        //        if (DateTime.TryParse(form["ca_verfiry_duedate"], out dateValue) == true)
        //        {
        //            objModel.ca_verfiry_duedate = dateValue;
        //        }

        //        if (DateTime.TryParse(form["ca_notifed_date"], out dateValue) == true)
        //        {
        //            objModel.ca_notifed_date = dateValue;
        //        }


        //        //Reported By
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
        //        {
        //            if (form["aempno " + i] != "" && form["aempno " + i] != null)
        //            {
        //                objModel.ca_proposed_by = objModel.ca_proposed_by + "," + form["aempno " + i];
        //            }
        //        }
        //        if (objModel.ca_proposed_by != null)
        //        {
        //            objModel.ca_proposed_by = objModel.ca_proposed_by.Trim(',');
        //        }


        //        //Notifed To
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
        //        {
        //            if (form["nempno " + i] != "" && form["nempno " + i] != null)
        //            {
        //                objModel.ca_notifiedto = objModel.ca_notifiedto + "," + form["nempno " + i];
        //            }
        //        }
        //        if (objModel.ca_notifiedto != null)
        //        {
        //            objModel.ca_notifiedto = objModel.ca_notifiedto.Trim(',');
        //        }

        //        for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
        //        {
        //            AuditLogModels objNCModel = new AuditLogModels();
        //            if (form["ca_div " + i] != "" && form["ca_div " + i] != null)
        //            {
        //                objNCModel.id_audit_nc_ca = form["id_audit_nc_ca " + i];
        //                objNCModel.ca_div = form["ca_div " + i];
        //                objNCModel.ca_loc = form["ca_loc " + i];
        //                objNCModel.ca_dept = form["ca_dept " + i];
        //                objNCModel.ca_rootcause = form["ca_rootcause " + i];
        //                objNCModel.ca_ca = form["ca_ca " + i];
        //                objNCModel.ca_resource = form["ca_resource " + i];
        //                objNCModel.ca_resp_person = form["ca_resp_person " + i];
        //                if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
        //                {
        //                    objNCModel.ca_target_date = dateValue;
        //                }
        //                objModelList.LogList.Add(objNCModel);
        //            }
        //        }

        //        if (objModel.FunUpdateCA(objModel, objModelList))
        //        {
        //            TempData["Successdata"] = "Added Corrective Action details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddCA: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(true);
        //}

        ////Verification
        //public ActionResult AddVerification()
        //{
        //    AuditLogModels objModel = new AuditLogModels();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            ViewBag.id_audit_process_perform = sid_audit_process_perform;

        //            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
        //            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
        //            ViewBag.OpenClose = objGlobaldata.GetConstantValue("OpenClose");

        //            NCModels objNCModel = new NCModels();
        //            ViewBag.NCRStatus = objNCModel.GetNCRStatusList();
        //            ViewBag.Action = objNCModel.GetNCActionImplementList();
        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location," +
        //                      "Category,Non_comformance,Details,Evidence,v_implement,v_implement_explain,v_rca,v_rca_explain," +
        //                      "v_discrepancies,v_discrep_explain,v_upload,v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto,id_audit_process_perform from" +
        //                      " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";
        //             DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditLogModels
        //                {
        //                    Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                    Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                    divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                    deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                    id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                    Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                    Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                    Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                    Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                    v_implement = (dsNCModels.Tables[0].Rows[0]["v_implement"].ToString()),
        //                    v_implement_explain = (dsNCModels.Tables[0].Rows[0]["v_implement_explain"].ToString()),
        //                    v_rca = (dsNCModels.Tables[0].Rows[0]["v_rca"].ToString()),
        //                    v_rca_explain = (dsNCModels.Tables[0].Rows[0]["v_rca_explain"].ToString()),
        //                    v_discrepancies = (dsNCModels.Tables[0].Rows[0]["v_discrepancies"].ToString()),
        //                    v_discrep_explain = (dsNCModels.Tables[0].Rows[0]["v_discrep_explain"].ToString()),
        //                    v_upload = (dsNCModels.Tables[0].Rows[0]["v_upload"].ToString()),
        //                    v_status = (dsNCModels.Tables[0].Rows[0]["v_status"].ToString()),
        //                    v_verifiedto = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()),
        //                    v_notifiedto = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()),
        //                };

        //                if (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString() != "")
        //                {
        //                    ViewBag.ReportedByArray = (dsNCModels.Tables[0].Rows[0]["v_verifiedto"].ToString()).Split(',');
        //                }

        //                if (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString() != "")
        //                {
        //                    ViewBag.NotifiedtoArray = (dsNCModels.Tables[0].Rows[0]["v_notifiedto"].ToString()).Split(',');
        //                }

        //                DateTime dtValue;
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                {
        //                    objModel.AuditDate = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_verified_date"].ToString(), out dtValue))
        //                {
        //                    objModel.v_verified_date = dtValue;
        //                }
        //                if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["v_closed_date"].ToString(), out dtValue))
        //                {
        //                    objModel.v_closed_date = dtValue;
        //                }

        //                AuditLogModelsList objVeriList = new AuditLogModelsList();
        //                objVeriList.LogList = new List<AuditLogModels>();

        //                string Sql = "Select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
        //               "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";

        //                DataSet dsCAModels = objGlobaldata.Getdetails(Sql);

        //                if (dsCAModels.Tables.Count > 0 && dsCAModels.Tables[0].Rows.Count > 0)
        //                {
        //                    for (int i = 0; i < dsCAModels.Tables[0].Rows.Count; i++)
        //                    {
        //                        try
        //                        {
        //                            AuditLogModels objCAModel = new AuditLogModels
        //                            {
        //                                id_audit_nc_ca = (dsCAModels.Tables[0].Rows[i]["id_audit_nc_ca"].ToString()),
        //                                ca_div = (dsCAModels.Tables[0].Rows[i]["ca_div"].ToString()),
        //                                ca_loc = (dsCAModels.Tables[0].Rows[i]["ca_loc"].ToString()),
        //                                ca_dept = (dsCAModels.Tables[0].Rows[i]["ca_dept"].ToString()),
        //                                ca_rootcause = (dsCAModels.Tables[0].Rows[i]["ca_rootcause"].ToString()),
        //                                ca_ca = (dsCAModels.Tables[0].Rows[i]["ca_ca"].ToString()),
        //                                ca_resource = (dsCAModels.Tables[0].Rows[i]["ca_resource"].ToString()),
        //                                ca_resp_person = (dsCAModels.Tables[0].Rows[i]["ca_resp_person"].ToString()),
        //                                implement_status = (dsCAModels.Tables[0].Rows[i]["implement_status"].ToString()),
        //                                ca_effective = (dsCAModels.Tables[0].Rows[i]["ca_effective"].ToString()),
        //                                reason = (dsCAModels.Tables[0].Rows[i]["reason"].ToString()),
        //                            };

        //                            if (DateTime.TryParse(dsCAModels.Tables[0].Rows[i]["ca_target_date"].ToString(), out dtValue))
        //                            {
        //                                objCAModel.ca_target_date = dtValue;
        //                            }
        //                            objVeriList.LogList.Add(objCAModel);
        //                        }
        //                        catch (Exception Ex)
        //                        {
        //                            objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + Ex.ToString());
        //                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                        }
        //                    }
        //                    ViewBag.CorrectiveAction = objVeriList;
        //                }

        //                return View(objModel);
        //            }
        //            else
        //            {
        //                TempData["alertdata"] = "No Data exists";
        //                return RedirectToAction("AuditLogList");
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("AuditLogList");
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult AddVerification(AuditLogModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> v_upload)
        //{
        //    try
        //    {
        //        AuditLogModelsList objModelList = new AuditLogModelsList();
        //        objModelList.LogList = new List<AuditLogModels>();

        //        DateTime dateValue;

        //        if (DateTime.TryParse(form["team_targetdate"], out dateValue) == true)
        //        {
        //            objModel.team_targetdate = dateValue;
        //        }

        //        IList<HttpPostedFileBase> v_uploadList = (IList<HttpPostedFileBase>)v_upload;
        //        string QCDelete = Request.Form["QCDocsValselectall"];

        //        if (v_uploadList[0] != null)
        //        {
        //            objModel.v_upload = "";
        //            foreach (var file in v_upload)
        //            {
        //                try
        //                {
        //                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/NC"), Path.GetFileName(file.FileName));
        //                    string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
        //                    file.SaveAs(sFilepath + "/" + sFilename);
        //                    objModel.v_upload = objModel.v_upload + "," + "~/DataUpload/MgmtDocs/NC/" + sFilename;
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in AddVerification-upload: " + ex.ToString());
        //                }
        //            }
        //            objModel.v_upload = objModel.v_upload.Trim(',');
        //        }
        //        else
        //        {
        //            ViewBag.Message = "You have not specified a file.";
        //        }
        //        if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
        //        {
        //            objModel.v_upload = objModel.v_upload + "," + form["QCDocsVal"];
        //            objModel.v_upload = objModel.v_upload.Trim(',');
        //        }
        //        else if (form["QCDocsVal"] == null && QCDelete != null && v_uploadList[0] == null)
        //        {
        //            objModel.v_upload = null;
        //        }
        //        else if (form["QCDocsVal"] == null && v_uploadList[0] == null)
        //        {
        //            objModel.v_upload = null;
        //        }
        //        //Notified By
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnt1"]); i++)
        //        {
        //            if (form["aempno " + i] != "" && form["aempno " + i] != null)
        //            {
        //                objModel.v_verifiedto = objModel.v_verifiedto + "," + form["aempno " + i];
        //            }
        //        }
        //        if (objModel.v_verifiedto != null)
        //        {
        //            objModel.v_verifiedto = objModel.v_verifiedto.Trim(',');
        //        }


        //        //Notifed To
        //        for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
        //        {
        //            if (form["nempno " + i] != "" && form["nempno " + i] != null)
        //            {
        //                objModel.v_notifiedto = objModel.v_notifiedto + "," + form["nempno " + i];
        //            }
        //        }
        //        if (objModel.v_notifiedto != null)
        //        {
        //            objModel.v_notifiedto = objModel.v_notifiedto.Trim(',');
        //        }


        //        for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
        //        {
        //            AuditLogModels objNCModel = new AuditLogModels();
        //            if (form["ca_div " + i] != "" && form["ca_div " + i] != null)
        //            {
        //                // objNCModel.ca_div = form["ca_div " + i];
        //                // objNCModel.ca_loc = form["ca_loc " + i];
        //                //   objNCModel.ca_dept = form["ca_dept " + i];
        //                //  //objNCModel.ca_rootcause = form["ca_rootcause " + i];
        //                //  objNCModel.ca_ca = form["ca_ca " + i];
        //                //objNCModel.ca_resource = form["ca_resource " + i];
        //                //objNCModel.ca_resp_person = form["ca_resp_person " + i];
        //                //if (DateTime.TryParse(form["ca_target_date " + i], out dateValue) == true)
        //                //{
        //                //    objNCModel.ca_target_date = dateValue;
        //                //}
        //                objNCModel.id_audit_nc_ca = form["id_audit_nc_ca " + i];
        //                objNCModel.implement_status = form["implement_status " + i];
        //                objNCModel.ca_effective = form["ca_effective " + i];
        //                objNCModel.reason = form["reason " + i];
        //            }
        //            objModelList.LogList.Add(objNCModel);
        //        }

        //        if (objModel.FunUpdateVerification(objModel, objModelList))
        //        {
        //            TempData["Successdata"] = "Added Verification details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddVerification: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json(true);
        //}

        //NC Details
        //[AllowAnonymous]
        //public ActionResult NCDetails()
        //{
        //    AuditLogModels objModel = new AuditLogModels();

        //    AuditLogModelsList NcList = new AuditLogModelsList();
        //    NcList.LogList = new List<AuditLogModels>();
        //    try
        //    {
        //        if (Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["id_audit_process_perform"] != "")
        //        {
        //            string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
        //            string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location,team," +
        //                "Category,Non_comformance,Details,Evidence,id_audit_process_perform from" +
        //                " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";

        //            DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
        //            {
        //                try
        //                {
        //                    objModel = new AuditLogModels
        //                    {
        //                        Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
        //                        Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
        //                        divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                        division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
        //                        deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                        dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
        //                        locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                        location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                        teamId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                        team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
        //                        id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
        //                        Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
        //                        Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
        //                        Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
        //                        Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
        //                        disp_action_taken = (dsNCModels.Tables[0].Rows[0]["disp_action_taken"].ToString()),
        //                    };

        //                    DateTime dtValue;
        //                    if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
        //                    {
        //                        objModel.AuditDate = dtValue;
        //                    }

        //                    NcList.LogList.Add(objModel);
        //                }
        //                catch (Exception ex)
        //                {
        //                    objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
        //                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //                }
        //            }

        //            //Disposition
        //            string sSqlstmt11 = "select id_audit_process_perform,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
        //            ViewBag.Disposition = dsDispModel;

        //            string sSqlstmt12 = "select id_audit_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
        //            ViewBag.DispAction = dsDispModels;

        //            //Team
        //            string sSqlstmt13 = "select nc_team,team_approvedby,team_notifiedto,team_targetdate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
        //            ViewBag.Team = dsTeamModel;

        //            //RCA
        //            string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
        //            ViewBag.RCA = dsRCAModels;

        //            //CA
        //            string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
        //            ViewBag.CA = dsCAModels;

        //            string sSqlstmt16 = "select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
        //                "ca_target_date,ca_resp_person from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";
        //            DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
        //            ViewBag.CAList = dsCAList;

        //            //Verification
        //            string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
        //                "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
        //            DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
        //            ViewBag.Verification = dsVerifyModels;

        //            string sSqlstmt18 = "Select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
        //            "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";
        //            DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
        //            ViewBag.VerificationList = dsVerifyModel;

        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "NC Id cannot be null";
        //            return RedirectToAction("AuditLogList");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objModel);
        //}


        //NC Details
        [AllowAnonymous]
        public ActionResult NCPDF(FormCollection form)
        {
            AuditLogModels objModel = new AuditLogModels();

            AuditLogModelsList NcList = new AuditLogModelsList();
            NcList.LogList = new List<AuditLogModels>();
            try
            {
                if (form["id_audit_process_perform"] != null && form["id_audit_process_perform"] != "")
                {
                    string sid_audit_process_perform = form["id_audit_process_perform"];
                    string sSqlstmt = "select a.Audit_Id,Audit_no,Audit_criteria,AuditDate,division,dept,location,team," +
                        "Category,Non_comformance,Details,Evidence,id_audit_process_perform from" +
                        " t_audit_process a,t_audit_process_perform b where a.Audit_Id=b.Audit_Id and active=1 and id_audit_process_perform='" + sid_audit_process_perform + "'";

                    DataSet dsNCModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsNCModels.Tables.Count > 0 && dsNCModels.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            objModel = new AuditLogModels
                            {
                                Audit_Id = (dsNCModels.Tables[0].Rows[0]["Audit_Id"].ToString()),
                                Audit_no = (dsNCModels.Tables[0].Rows[0]["Audit_no"].ToString()),
                                divisionId = (dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                                division = objGlobaldata.GetCompanyBranchNameById(dsNCModels.Tables[0].Rows[0]["division"].ToString()),
                                deptId = (dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsNCModels.Tables[0].Rows[0]["dept"].ToString()),
                                locationId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                location = objGlobaldata.GetDivisionLocationById(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                teamId = (dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                //team = objGlobaldata.GetTeamNameByID(dsNCModels.Tables[0].Rows[0]["location"].ToString()),
                                id_audit_process_perform = (dsNCModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString()),
                                Non_comformance = (dsNCModels.Tables[0].Rows[0]["Non_comformance"].ToString()),
                                Category = (dsNCModels.Tables[0].Rows[0]["Category"].ToString()),
                                Details = (dsNCModels.Tables[0].Rows[0]["Details"].ToString()),
                                Evidence = (dsNCModels.Tables[0].Rows[0]["Evidence"].ToString()),
                            };

                            DateTime dtValue;
                            if (DateTime.TryParse(dsNCModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                            {
                                objModel.AuditDate = dtValue;
                            }

                            NcList.LogList.Add(objModel);
                            ViewBag.NonConfirmance = objModel;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NCDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }



                    CompanyModels objCompany = new CompanyModels();
                    string user = objGlobaldata.GetCurrentUserSession().empid;
                    dsNCModels = objCompany.GetCompanyDetailsForReport(dsNCModels);
                    dsNCModels = objGlobaldata.GetReportDetails(dsNCModels, objModel.Non_comformance, user, "AUDIT NON CONFERMANCE REPORT");

                    ViewBag.CompanyInfo = dsNCModels;

                    //Disposition
                    string sSqlstmt11 = "select id_audit_process_perform,disp_action_taken,disp_explain,disp_notifiedto,disp_notifeddate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsDispModel = objGlobaldata.Getdetails(sSqlstmt11);
                    ViewBag.Disposition = dsDispModel;

                    string sSqlstmt12 = "select id_audit_nc_disp_action,disp_action,disp_resp_person,disp_complete_date from t_audit_nc_disp_action where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsDispModels = objGlobaldata.Getdetails(sSqlstmt12);
                    ViewBag.DispAction = dsDispModels;

                    //Team
                    string sSqlstmt13 = "select nc_team,team_approvedby,team_notifiedto,team_targetdate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsTeamModel = objGlobaldata.Getdetails(sSqlstmt13);
                    ViewBag.Team = dsTeamModel;

                    //RCA
                    string sSqlstmt14 = "select rca_technique,rca_details,rca_upload,rca_action,rca_justify,rca_reportedby,rca_notifiedto,rca_reporteddate from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsRCAModels = objGlobaldata.Getdetails(sSqlstmt14);
                    ViewBag.RCA = dsRCAModels;

                    //CA
                    string sSqlstmt15 = "select ca_verfiry_duedate,ca_proposed_by,ca_notifiedto,ca_notifed_date from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsCAModels = objGlobaldata.Getdetails(sSqlstmt15);
                    ViewBag.CA = dsCAModels;

                    string sSqlstmt16 = "select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                        "ca_target_date,ca_resp_person from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";
                    DataSet dsCAList = objGlobaldata.Getdetails(sSqlstmt16);
                    ViewBag.CAList = dsCAList;

                    //Verification
                    string sSqlstmt17 = "select v_implement,v_implement_explain,v_rca,v_rca_explain,v_discrepancies,v_discrep_explain,v_upload," +
                        "v_status,v_closed_date,v_verifiedto,v_verified_date,v_notifiedto from t_audit_process_perform where id_audit_process_perform='" + sid_audit_process_perform + "'";
                    DataSet dsVerifyModels = objGlobaldata.Getdetails(sSqlstmt17);
                    ViewBag.Verification = dsVerifyModels;

                    string sSqlstmt18 = "Select id_audit_nc_ca,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource," +
                    "ca_target_date,ca_resp_person,implement_status,ca_effective,reason from t_audit_nc_ca where id_audit_process_perform = '" + sid_audit_process_perform + "' and ca_active=1";
                    DataSet dsVerifyModel = objGlobaldata.Getdetails(sSqlstmt18);
                    ViewBag.VerificationList = dsVerifyModel;

                }
                else
                {
                    TempData["alertdata"] = "NC Id cannot be null";
                    return RedirectToAction("AuditLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NCPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("NCPDF")
            {
                FileName = "NCPDF.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
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

                    if (Doc.FunDeleteCADoc(sid_audit_nc_ca))
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

        public JsonResult FunGetEmpDetails(string semp_no)
        {

            NCModels objModels = new NCModels();
            try
            {
                string sSqlstmt = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId from t_hr_employee where emp_no = '" + semp_no + "'";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    objModels = new NCModels
                    {
                        //emp_no = Convert.ToInt32(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                        empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString()),
                        location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        department = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString()),
                        EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString())
                    };
                }
                return Json(objModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }
    }
}