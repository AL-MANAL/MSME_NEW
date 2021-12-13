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
    public class AuditScheduleController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public AuditScheduleController()
        {
            ViewBag.Menutype = "Audit";
            ViewBag.SubMenutype = "InternalAudit";
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddAuditSchedule()
        {
            AuditScheduleModels ObjMdl = new AuditScheduleModels();
            try
            {
                ObjMdl.branch = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.EmpList = objGlobaldata.GetHrEmpListByDivision(ObjMdl.branch);
                ViewBag.Dept = objGlobaldata.GetDepartmentList1(ObjMdl.branch);
                ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(ObjMdl.branch);

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.AuditType = objGlobaldata.GetDropdownList("Schedule Audit Type");
                ViewBag.EntityType = objGlobaldata.GetDropdownList("Schedule Entity Type");
                ViewBag.EntityNo = objGlobaldata.GetDropdownList("Schedule Entity Number");
                ViewBag.AuditCriteria = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.AuditCriticality = objGlobaldata.GetDropdownList("Schedule Audit Criticality");
                ViewBag.AuditScope = objGlobaldata.GetDropdownList("Schedule Audit Scope");
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.CheckList = objGlobaldata.GetChecklistType();
                ViewBag.Availability = objGlobaldata.GetConstantValue("Availability");
                ViewBag.Auditor = objGlobaldata.GetAuditor();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditSchedule: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(ObjMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddAuditSchedule(FormCollection form, AuditScheduleModels objSchedule, IEnumerable<HttpPostedFileBase> contractual_agreement)
        {
            try
            {
                DateTime dateValue;

                objSchedule.audit_criteria = form["audit_criteria"];
                objSchedule.audit_scope = form["audit_scope"];
                objSchedule.branch = form["branch"];
                //if (form["acc_date"] != null && DateTime.TryParse(form["acc_date"], out dateValue) == true)
                //{
                //    objSchedule.acc_date = dateValue;
                //    int iHr, iMin;
                //    if (form["PlanTimeInHour"] != null && int.TryParse(form["PlanTimeInHour"], out iHr) &&
                //        form["PlanTimeInMin"] != null && int.TryParse(form["PlanTimeInMin"], out iMin))
                //    {
                //        objSchedule.acc_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                //    }
                //}
                //if (DateTime.TryParse(form["reported_date"], out dateValue) == true)
                //{
                //    objSchedule.reported_date = dateValue;
                //}
                HttpPostedFileBase files = Request.Files[0];

                if (contractual_agreement != null && files.ContentLength > 0)
                {
                    objSchedule.contractual_agreement = "";
                    foreach (var file in contractual_agreement)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "Audit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objSchedule.contractual_agreement = objSchedule.contractual_agreement + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddAuditSchedule Upload: " + ex.ToString());
                        }
                    }
                    objSchedule.contractual_agreement = objSchedule.contractual_agreement.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                //AuditScheduleModelsList objAccTypeList = new AuditScheduleModelsList();
                //objAccTypeList.AccidentList = new List<AuditScheduleModels>();
                //for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                //{
                //    AuditScheduleModels objAccRpt = new AuditScheduleModels();

                //    objAccRpt.injury_type = form["injury_type " + i];
                //    objAccRpt.no_person = form["no_person " + i];

                //    objAccTypeList.AccidentList.Add(objAccRpt);
                //}

                AuditScheduleModelsList objAuditList = new AuditScheduleModelsList();
                objAuditList.ScheculeList = new List<AuditScheduleModels>();
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditScheduleModels AudModel = new AuditScheduleModels();

                    AudModel.auditor_name = form["auditor_name" + i];
                    AudModel.audit_dept = form["audit_dept" + i];
                    AudModel.audit_loc = form["audit_loc" + i];
                    AudModel.focused_area = form["focused_area" + i];
                    AudModel.checklist_id = form["checklist_id" + i];
                    AudModel.auditor_availability = form["auditor_availability" + i];
                    AudModel.next_availability = form["next_availability" + i];

                    if (DateTime.TryParse(form["scheduled_time_date" + i], out dateValue) == true)
                    {
                        AudModel.scheduled_time_date = dateValue;
                        int iHr, iMin;
                        if (form["PlanTimeInHour" + i] != null && int.TryParse(form["PlanTimeInHour" + i], out iHr) &&
                            form["PlanTimeInMin" + i] != null && int.TryParse(form["PlanTimeInMin" + i], out iMin))
                        {
                            AudModel.scheduled_time_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                        }
                    }
                    objAuditList.ScheculeList.Add(AudModel);
                }

                if (objSchedule.FunAddAuditSchedule(objSchedule, objAuditList))
                {
                    TempData["Successdata"] = "Added Audit Schedule details successfully with Audit Number '" + objSchedule.audit_no + "'";
                    //Send MOM email
                    //AuditScheduleModels objAudit = new AuditScheduleModels();
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditSchedule: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditScheduleList");
        }

        [AllowAnonymous]
        public ActionResult AuditScheduleList(FormCollection form, int? page, string branch_name)
        {
            AuditScheduleModelsList objAttList = new AuditScheduleModelsList();
            objAttList.ScheculeList = new List<AuditScheduleModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
                    "contractual_agreement,purpose,site_access,schedule_by,approvedby,branch from t_audit_schedule where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_audit_schedule desc";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAccidentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditScheduleModels objScheduleMdl = new AuditScheduleModels
                            {
                                id_audit_schedule = dsAccidentList.Tables[0].Rows[i]["id_audit_schedule"].ToString(),
                                audit_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_type"].ToString()),
                                audit_no = (dsAccidentList.Tables[0].Rows[i]["audit_no"].ToString()),
                                entity_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["entity_type"].ToString()),
                                entity_no = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["entity_no"].ToString()),
                                audit_scope = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_scope"].ToString()),
                                audit_criticallty = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_criticallty"].ToString()),
                                audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAccidentList.Tables[0].Rows[i]["audit_criteria"].ToString()),
                                contractual_agreement = dsAccidentList.Tables[0].Rows[i]["contractual_agreement"].ToString(),
                                purpose = dsAccidentList.Tables[0].Rows[i]["purpose"].ToString(),
                                site_access = dsAccidentList.Tables[0].Rows[i]["site_access"].ToString(),
                                schedule_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["schedule_by"].ToString()),
                                approvedby = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["approvedby"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[i]["branch"].ToString()),
                            };

                            //DateTime dtDocDate;
                            //if (dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString() != ""
                            //   && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                            //{
                            //    objScheduleMdl.acc_date = dtDocDate;
                            //}

                            objAttList.ScheculeList.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditScheduleList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAttList.ScheculeList.ToList());
        }

        [AllowAnonymous]
        public JsonResult AuditScheduleListSearch(FormCollection form, int? page, string branch_name)
        {
            AuditScheduleModelsList objAttList = new AuditScheduleModelsList();
            objAttList.ScheculeList = new List<AuditScheduleModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
                    "contractual_agreement,purpose,site_access,schedule_by,approvedby,branch from t_audit_schedule where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + " order by id_audit_schedule desc";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAccidentList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditScheduleModels objScheduleMdl = new AuditScheduleModels
                            {
                                id_audit_schedule = dsAccidentList.Tables[0].Rows[i]["id_audit_schedule"].ToString(),
                                audit_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_type"].ToString()),
                                audit_no = (dsAccidentList.Tables[0].Rows[i]["audit_no"].ToString()),
                                entity_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["entity_type"].ToString()),
                                entity_no = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["entity_no"].ToString()),
                                audit_scope = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_scope"].ToString()),
                                audit_criticallty = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[i]["audit_criticallty"].ToString()),
                                audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAccidentList.Tables[0].Rows[i]["audit_criteria"].ToString()),
                                contractual_agreement = dsAccidentList.Tables[0].Rows[i]["contractual_agreement"].ToString(),
                                purpose = dsAccidentList.Tables[0].Rows[i]["purpose"].ToString(),
                                site_access = dsAccidentList.Tables[0].Rows[i]["site_access"].ToString(),
                                schedule_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["schedule_by"].ToString()),
                                approvedby = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[i]["approvedby"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[i]["branch"].ToString()),
                            };

                            //DateTime dtDocDate;
                            //if (dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString() != ""
                            //   && DateTime.TryParse(dsAccidentList.Tables[0].Rows[i]["acc_date"].ToString(), out dtDocDate))
                            //{
                            //    objScheduleMdl.acc_date = dtDocDate;
                            //}

                            objAttList.ScheculeList.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditScheduleListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult AuditScheduleEdit()
        {
            AuditScheduleModels objModel = new AuditScheduleModels();
            try
            {
                ViewBag.AuditType = objGlobaldata.GetDropdownList("Schedule Audit Type");
                ViewBag.EntityType = objGlobaldata.GetDropdownList("Schedule Entity Type");
                ViewBag.EntityNo = objGlobaldata.GetDropdownList("Schedule Entity Number");
                ViewBag.AuditCriteria = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.AuditCriticality = objGlobaldata.GetDropdownList("Schedule Audit Criticality");
                ViewBag.AuditScope = objGlobaldata.GetDropdownList("Schedule Audit Scope");
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.CheckList = objGlobaldata.GetChecklistType();
                ViewBag.Availability = objGlobaldata.GetConstantValue("Availability");
                ViewBag.Auditor = objGlobaldata.GetAuditor();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                if (Request.QueryString["id_audit_schedule"] != null && Request.QueryString["id_audit_schedule"] != "")
                {
                    string id_audit_schedule = Request.QueryString["id_audit_schedule"];

                    string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
                    "contractual_agreement,purpose,site_access,schedule_by,approvedby,branch from t_audit_schedule where id_audit_schedule ='" + id_audit_schedule + "'";

                    DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditScheduleModels
                        {
                            id_audit_schedule = dsAccidentList.Tables[0].Rows[0]["id_audit_schedule"].ToString(),
                            audit_type = (dsAccidentList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_no = (dsAccidentList.Tables[0].Rows[0]["audit_no"].ToString()),
                            entity_type = (dsAccidentList.Tables[0].Rows[0]["entity_type"].ToString()),
                            entity_no = (dsAccidentList.Tables[0].Rows[0]["entity_no"].ToString()),
                            audit_scope = (dsAccidentList.Tables[0].Rows[0]["audit_scope"].ToString()),
                            audit_criticallty = (dsAccidentList.Tables[0].Rows[0]["audit_criticallty"].ToString()),
                            audit_criteria = (dsAccidentList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            contractual_agreement = dsAccidentList.Tables[0].Rows[0]["contractual_agreement"].ToString(),
                            purpose = dsAccidentList.Tables[0].Rows[0]["purpose"].ToString(),
                            site_access = dsAccidentList.Tables[0].Rows[0]["site_access"].ToString(),
                            schedule_by = (dsAccidentList.Tables[0].Rows[0]["schedule_by"].ToString()),
                            approvedby = (dsAccidentList.Tables[0].Rows[0]["approvedby"].ToString()),
                            branch = (dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                        };
                        //DateTime dtDocDate;
                        //if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                        //      && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                        //{
                        //    objModel.acc_date = dtDocDate;
                        //}
                        ViewBag.EmpList = objGlobaldata.GetHrEmpListByDivision(dsAccidentList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsAccidentList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Dept = objGlobaldata.GetDepartmentList1(dsAccidentList.Tables[0].Rows[0]["branch"].ToString());
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AuditScheduleList");
                    }

                    AuditScheduleModelsList objList = new AuditScheduleModelsList();
                    objList.ScheculeList = new List<AuditScheduleModels>();

                    sSqlstmt = "select id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area,scheduled_time_date,checklist_id," +
                        "auditor_availability,next_availability from t_audit_schedule_trans where id_audit_schedule='" + id_audit_schedule + "'";

                    DataSet dsScheduleList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsScheduleList.Tables.Count > 0 && dsScheduleList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsScheduleList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AuditScheduleModels objTeamMdl = new AuditScheduleModels
                                {
                                    id_audit_schedule_trans = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule_trans"].ToString(),
                                    id_audit_schedule = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule"].ToString(),
                                    auditor_name = /*objGlobaldata.GetMultiHrEmpNameById*/(dsScheduleList.Tables[0].Rows[i]["auditor_name"].ToString()),
                                    audit_dept = dsScheduleList.Tables[0].Rows[i]["audit_dept"].ToString(),
                                    audit_loc = dsScheduleList.Tables[0].Rows[i]["audit_loc"].ToString(),
                                    focused_area = dsScheduleList.Tables[0].Rows[i]["focused_area"].ToString(),
                                    checklist_id = dsScheduleList.Tables[0].Rows[i]["checklist_id"].ToString(),
                                    auditor_availability = dsScheduleList.Tables[0].Rows[i]["auditor_availability"].ToString(),
                                    next_availability = dsScheduleList.Tables[0].Rows[i]["next_availability"].ToString(),
                                };

                                DateTime dtDocDate1;
                                if (dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString() != ""
                         && DateTime.TryParse(dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString(), out dtDocDate1))
                                {
                                    objTeamMdl.scheduled_time_date = dtDocDate1;
                                }

                                objList.ScheculeList.Add(objTeamMdl);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AuditScheduleList");
                            }
                        }
                        ViewBag.objAuditList = objList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditScheduleList");
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AuditScheduleEdit(FormCollection form, AuditScheduleModels objSchedule, IEnumerable<HttpPostedFileBase> contractual_agreement)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];

                objSchedule.audit_criteria = form["audit_criteria"];
                objSchedule.audit_scope = form["audit_scope"];
                objSchedule.branch = form["branch"];
                //if (DateTime.TryParse(form["acc_date"], out dateValue) == true)
                //{
                //    objSchedule.acc_date = dateValue;
                //}

                HttpPostedFileBase files = Request.Files[0];

                if (Request.Files.Count > 0)
                {
                    if (contractual_agreement != null && files.ContentLength > 0)
                    {
                        objSchedule.contractual_agreement = "";
                        foreach (var file in contractual_agreement)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                                string sFilename = "Accident" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objSchedule.contractual_agreement = objSchedule.contractual_agreement + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleEdit-upload: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objSchedule.contractual_agreement = objSchedule.contractual_agreement.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objSchedule.contractual_agreement = objSchedule.contractual_agreement + "," + form["QCDocsVal"];
                    objSchedule.contractual_agreement = objSchedule.contractual_agreement.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objSchedule.contractual_agreement = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objSchedule.contractual_agreement = null;
                }

                AuditScheduleModelsList objTeamList = new AuditScheduleModelsList();
                objTeamList.ScheculeList = new List<AuditScheduleModels>();

                //for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                //{
                //    if (form["injury_type " + i] != null && form["no_person " + i] != null)
                //    {
                //        AuditScheduleModels objAccRpt = new AuditScheduleModels();
                //        objAccRpt.injury_type = form["injury_type " + i];
                //        objAccRpt.no_person = form["no_person " + i];

                //        objAccTypeList.AccidentList.Add(objAccRpt);
                //    }
                //}

                //if (objSchedule.injury_type != null)
                //{
                //    AuditScheduleModels objAccRpt = new AuditScheduleModels();
                //    objAccRpt.injury_type = form["injury_type "];
                //    objAccRpt.no_person = form["no_person "];

                //    objAccTypeList.AccidentList.Add(objAccRpt);
                //}

                int iCnt = 0;
                DateTime dateValue;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < /*Convert.ToInt16(form["itemcnt"])*/iCnt; i++)
                    {
                        if (form["auditor_name " + i] != null || form["audit_dept " + i] != null)
                        {
                            AuditScheduleModels objAuditmdl = new AuditScheduleModels
                            {
                                id_audit_schedule = form["id_audit_schedule " + i],
                                auditor_name = form["auditor_name " + i],
                                audit_dept = form["audit_dept " + i],
                                audit_loc = form["audit_loc " + i],
                                focused_area = form["focused_area " + i],
                                checklist_id = form["checklist_id " + i],
                                auditor_availability = form["auditor_availability " + i],
                                next_availability = form["next_availability " + i],
                            };
                            if (DateTime.TryParse(form["scheduled_time_date " + i], out dateValue) == true)
                            {
                                objAuditmdl.scheduled_time_date = dateValue;

                                int iHr, iMin;
                                if (form["PlanTimeInHour " + i] != null && int.TryParse(form["PlanTimeInHour " + i], out iHr) &&
                                    form["PlanTimeInMin " + i] != null && int.TryParse(form["PlanTimeInMin " + i], out iMin))
                                {
                                    objAuditmdl.scheduled_time_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                                }
                                //int iHr ,iMin;
                                //if (form["PlanTimeInHour " + i] != null  &&
                                //    form["PlanTimeInMin " + i] != null)
                                //{
                                //    iHr = Convert.ToInt32(form["PlanTimeInHour " + i]);
                                //    iMin = Convert.ToInt32(form["PlanTimeInMin " + i]);
                                //    objAuditmdl.scheduled_time_date = DateTime.Parse(dateValue.ToString("dd/MM/yyyy") + " " + iHr + ":" + iMin + ":00");
                                //}
                            }
                            objTeamList.ScheculeList.Add(objAuditmdl);
                        }
                    }
                }

                if (objSchedule.FunUpdateAuditSchedule(objSchedule, objTeamList))
                {
                    TempData["Successdata"] = "Audit details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditScheduleList");
        }

        [AllowAnonymous]
        public ActionResult AuditScheduleInfo(int id)
        {
            AuditScheduleModels objModel = new AuditScheduleModels();
            try
            {
                string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
                    "contractual_agreement,purpose,site_access,schedule_by,approvedby,branch from t_audit_schedule where id_audit_schedule ='" + id + "'";

                DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                {
                    objModel = new AuditScheduleModels
                    {
                        id_audit_schedule = dsAccidentList.Tables[0].Rows[0]["id_audit_schedule"].ToString(),
                        audit_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_type"].ToString()),
                        audit_no = (dsAccidentList.Tables[0].Rows[0]["audit_no"].ToString()),
                        entity_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["entity_type"].ToString()),
                        entity_no = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["entity_no"].ToString()),
                        audit_scope = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_scope"].ToString()),
                        audit_criticallty = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_criticallty"].ToString()),
                        audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAccidentList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                        contractual_agreement = dsAccidentList.Tables[0].Rows[0]["contractual_agreement"].ToString(),
                        purpose = dsAccidentList.Tables[0].Rows[0]["purpose"].ToString(),
                        site_access = dsAccidentList.Tables[0].Rows[0]["site_access"].ToString(),
                        schedule_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["schedule_by"].ToString()),
                        approvedby = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["approvedby"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                    };
                    //DateTime dtDocDate;
                    //if (dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString() != ""
                    //      && DateTime.TryParse(dsAccidentList.Tables[0].Rows[0]["acc_date"].ToString(), out dtDocDate))
                    //{
                    //    objModel.acc_date = dtDocDate;
                    //}
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditScheduleList");
                }

                //AuditScheduleModelsList objList = new AuditScheduleModelsList();
                //objList.ScheculeList = new List<AuditScheduleModels>();

                //sSqlstmt = "select id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area,scheduled_time_date,checklist_id," +
                //    "auditor_availability,next_availability from t_audit_schedule_trans where id_audit_schedule='" + id_audit_schedule + "'";

                //DataSet dsScheduleList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsScheduleList.Tables.Count > 0 && dsScheduleList.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < dsScheduleList.Tables[0].Rows.Count; i++)
                //    {
                //        try
                //        {
                //            AuditScheduleModels objTeamMdl = new AuditScheduleModels
                //            {
                //                id_audit_schedule_trans = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule_trans"].ToString(),
                //                id_audit_schedule = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule"].ToString(),
                //                auditor_name = /*objGlobaldata.GetMultiHrEmpNameById*/(dsScheduleList.Tables[0].Rows[i]["auditor_name"].ToString()),
                //                audit_dept = dsScheduleList.Tables[0].Rows[i]["audit_dept"].ToString(),
                //                audit_loc = dsScheduleList.Tables[0].Rows[i]["audit_loc"].ToString(),
                //                focused_area = dsScheduleList.Tables[0].Rows[i]["focused_area"].ToString(),
                //                checklist_id = dsScheduleList.Tables[0].Rows[i]["checklist_id"].ToString(),
                //                auditor_availability = dsScheduleList.Tables[0].Rows[i]["auditor_availability"].ToString(),
                //                next_availability = dsScheduleList.Tables[0].Rows[i]["next_availability"].ToString(),
                //            };

                //            DateTime dtDocDate1;
                //            if (dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString() != ""
                //     && DateTime.TryParse(dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString(), out dtDocDate1))
                //            {
                //                objTeamMdl.scheduled_time_date = dtDocDate1;
                //            }

                //            objList.ScheculeList.Add(objTeamMdl);
                //        }
                //        catch (Exception ex)
                //        {
                //            objGlobaldata.AddFunctionalLog("Exception in AuditScheduleEdit: " + ex.ToString());
                //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                //            return RedirectToAction("AuditScheduleList");
                //        }
                //    }
                //    ViewBag.objAuditList = objList;
                //}
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditScheduleList");
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public ActionResult AuditScheduleDetails()
        {
            AuditScheduleModels objModel = new AuditScheduleModels();
            try
            {
                if (Request.QueryString["id_audit_schedule"] != null && Request.QueryString["id_audit_schedule"] != "")
                {
                    string id_audit_schedule = Request.QueryString["id_audit_schedule"];

                    string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
                    "contractual_agreement,purpose,site_access,schedule_by,approvedby,branch from t_audit_schedule where id_audit_schedule ='" + id_audit_schedule + "'";

                    DataSet dsAccidentList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAccidentList.Tables.Count > 0 && dsAccidentList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditScheduleModels
                        {
                            id_audit_schedule = dsAccidentList.Tables[0].Rows[0]["id_audit_schedule"].ToString(),
                            audit_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_no = (dsAccidentList.Tables[0].Rows[0]["audit_no"].ToString()),
                            entity_type = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["entity_type"].ToString()),
                            entity_no = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["entity_no"].ToString()),
                            audit_scope = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_scope"].ToString()),
                            audit_criticallty = objGlobaldata.GetDropdownitemById(dsAccidentList.Tables[0].Rows[0]["audit_criticallty"].ToString()),
                            audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsAccidentList.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            contractual_agreement = dsAccidentList.Tables[0].Rows[0]["contractual_agreement"].ToString(),
                            purpose = dsAccidentList.Tables[0].Rows[0]["purpose"].ToString(),
                            site_access = dsAccidentList.Tables[0].Rows[0]["site_access"].ToString(),
                            schedule_by = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["schedule_by"].ToString()),
                            approvedby = objGlobaldata.GetMultiHrEmpNameById(dsAccidentList.Tables[0].Rows[0]["approvedby"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsAccidentList.Tables[0].Rows[0]["branch"].ToString()),
                        };
                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AuditScheduleList");
                    }

                    AuditScheduleModelsList objList = new AuditScheduleModelsList();
                    objList.ScheculeList = new List<AuditScheduleModels>();

                    sSqlstmt = "select id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area,scheduled_time_date,checklist_id," +
                        "auditor_availability,next_availability from t_audit_schedule_trans where id_audit_schedule='" + id_audit_schedule + "'";

                    DataSet dsScheduleList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsScheduleList.Tables.Count > 0 && dsScheduleList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsScheduleList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AuditScheduleModels objTeamMdl = new AuditScheduleModels
                                {
                                    id_audit_schedule_trans = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule_trans"].ToString(),
                                    id_audit_schedule = dsScheduleList.Tables[0].Rows[i]["id_audit_schedule"].ToString(),
                                    auditor_name = /*objGlobaldata.GetMultiHrEmpNameById*/(dsScheduleList.Tables[0].Rows[i]["auditor_name"].ToString()),
                                    audit_dept = dsScheduleList.Tables[0].Rows[i]["audit_dept"].ToString(),
                                    audit_loc = dsScheduleList.Tables[0].Rows[i]["audit_loc"].ToString(),
                                    focused_area = dsScheduleList.Tables[0].Rows[i]["focused_area"].ToString(),
                                    checklist_id = dsScheduleList.Tables[0].Rows[i]["checklist_id"].ToString(),
                                    auditor_availability = dsScheduleList.Tables[0].Rows[i]["auditor_availability"].ToString(),
                                    next_availability = dsScheduleList.Tables[0].Rows[i]["next_availability"].ToString(),
                                };

                                DateTime dtDocDate1;
                                if (dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString() != ""
                         && DateTime.TryParse(dsScheduleList.Tables[0].Rows[i]["scheduled_time_date"].ToString(), out dtDocDate1))
                                {
                                    objTeamMdl.scheduled_time_date = dtDocDate1;
                                }

                                objList.ScheculeList.Add(objTeamMdl);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleDetails: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AuditScheduleList");
                            }
                        }
                        ViewBag.objAuditList = objList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditScheduleList");
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public JsonResult AuditScheduleDelete(FormCollection form)
        {
            try
            {
                if (form["id_audit_schedule"] != null && form["id_audit_schedule"] != "")
                {
                    AuditScheduleModels Doc = new AuditScheduleModels();
                    string sid_audit_schedule = form["id_audit_schedule"];
                    if (Doc.FunDeleteAuditSchedule(sid_audit_schedule))
                    {
                        TempData["Successdata"] = "Audit Schedule deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in AuditScheduleDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}