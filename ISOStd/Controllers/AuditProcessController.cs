using ISOStd.Models;
using Rotativa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    public class AuditProcessController : Controller
    {
        // GET: AuditProcess

        clsGlobal objGlobaldata = new clsGlobal();
        public AuditProcessController()
        {
            ViewBag.Menutype = "Audit";

        }
        public ActionResult AuditPlan()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                objModel.branch = objGlobaldata.GetCurrentUserSession().division;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objModel.branch);

                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.branch);
                ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                // ViewBag.CheckList = objGlobaldata.GetChecklistTypeByChecklistRef(objModel.branch);
                objModel.PlannedBy = objGlobaldata.GetCurrentUserSession().empid;
                ViewBag.Process = objGlobaldata.GetAuidtCycleList();
                ViewBag.ApprovedBy = objGlobaldata.GetHrEmpListByDivision(objModel.branch);
                ViewBag.Audit = objGlobaldata.GetAuidtTypeList();

                ViewBag.AuditMethodology = objGlobaldata.GetDropdownList("Audit Methodology");
                ViewBag.AuditMethod = objGlobaldata.GetDropdownList("Audit Method");
                ViewBag.AuditLang = objGlobaldata.GetDropdownList("Audit Language");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditPlan: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AuditPlan(AuditProcessModels objModel, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                objModel.Audit_criteria = form["Audit_criteria"];
                objModel.PlannedBy = form["PlannedBy"];
                objModel.ApprovedBy = form["ApprovedBy"];
                objModel.Audit_no = form["auditno"];

                if (DateTime.TryParse(form["AuditPlanDate"], out dateValue) == true)
                {
                    objModel.AuditPlanDate = dateValue;
                }

                AuditProcessModelsList objAudit = new AuditProcessModelsList();
                objAudit.Obj = new List<AuditProcessModels>();


                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels obj = new AuditProcessModels();

                    if (form["branch" + i] != "" && form["branch" + i] != null)
                    {

                        obj.branch = form["branch" + i];
                        obj.dept_name = form["dept_name" + i];
                        obj.team = form["team" + i];
                        obj.location = form["location" + i];
                        obj.fromtime = form["fromtime" + i];
                        obj.totime = form["totime" + i];
                        obj.checklist = form["checklist" + i];
                        obj.auditors = form["auditors" + i];
                        obj.auditee_team = form["auditee_team" + i];
                        obj.doc_req = form["doc_req" + i];
                        if (DateTime.TryParse(form["AuditDate" + i], out dateValue) == true)
                        {
                            obj.AuditDate = dateValue;
                        }
                        objAudit.Obj.Add(obj);
                    }
                }
                if (objModel.FunAddAuditProcess(objModel, objAudit))
                {
                    TempData["Successdata"] = "Added Audit Plans successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddNC: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditProcessList");
        }


        [AllowAnonymous]
        public ActionResult AuditProcessEdit()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {

                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    string Audit_Id = Request.QueryString["Audit_Id"];

                    string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,ApprovedBy,audit_type,audit_code,audit_scope,audit_method,audit_objective,audit_lang,audit_methodology  from t_audit_process where Audit_Id=" + Audit_Id;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new AuditProcessModels
                        {
                            Audit_Id = (dsModelsList.Tables[0].Rows[0]["Audit_Id"].ToString()),
                            Audit_criteria = (dsModelsList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            PlannedBy = (dsModelsList.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            Audit_no = (dsModelsList.Tables[0].Rows[0]["Audit_no"].ToString()),
                            ApprovedBy = (dsModelsList.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            audit_type = (dsModelsList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_code = (dsModelsList.Tables[0].Rows[0]["audit_code"].ToString()),

                            audit_scope = (dsModelsList.Tables[0].Rows[0]["audit_scope"].ToString()),
                            audit_method = (dsModelsList.Tables[0].Rows[0]["audit_method"].ToString()),
                            audit_objective = (dsModelsList.Tables[0].Rows[0]["audit_objective"].ToString()),
                            audit_lang = (dsModelsList.Tables[0].Rows[0]["audit_lang"].ToString()),
                            audit_methodology = (dsModelsList.Tables[0].Rows[0]["audit_methodology"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtValue))
                        {
                            objModel.AuditPlanDate = dtValue;
                        }

                        ViewBag.AuditMethodology = objGlobaldata.GetDropdownList("Audit Methodology");
                        ViewBag.AuditMethod = objGlobaldata.GetDropdownList("Audit Method");
                        ViewBag.AuditLang = objGlobaldata.GetDropdownList("Audit Language");

                        objModel.branch = objGlobaldata.GetCurrentUserSession().division;

                        ViewBag.ApprovedBy = objGlobaldata.GetHrEmpListByDivision(objModel.branch);
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Department = objGlobaldata.GetDepartmentListbox();
                        //ViewBag.Team = objGlobaldata.GetMultiTeambyMultiGroup();
                        ViewBag.Location = objGlobaldata.GetDivisionLocationList(objModel.branch);
                        ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                        ViewBag.AuditCriteria = objGlobaldata.GetIsoStdListbox();
                        ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.CheckList = objGlobaldata.GetChecklistTypeByChecklistRef();
                        ViewBag.Audit = objGlobaldata.GetAuidtTypeList();
                        ViewBag.AuditCode = objGlobaldata.FunGetAuditProductList(objModel.audit_type);
                        AuditProcessModelsList objAudit = new AuditProcessModelsList();
                        objAudit.Obj = new List<AuditProcessModels>();

                        sSqlstmt = "select Plan_Id,Audit_Id,branch,dept_name,team,location,AuditDate,fromtime,totime,checklist,auditors,auditee_team,doc_req from t_audit_process_plan where Audit_Id='" + objModel.Audit_Id + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    AuditProcessModels objAuditModel = new AuditProcessModels
                                    {
                                        Plan_Id = dsList.Tables[0].Rows[i]["Plan_Id"].ToString(),
                                        Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                        branch = dsList.Tables[0].Rows[i]["branch"].ToString(),
                                        dept_name = dsList.Tables[0].Rows[i]["dept_name"].ToString(),
                                        team = dsList.Tables[0].Rows[i]["team"].ToString(),
                                        location = dsList.Tables[0].Rows[i]["location"].ToString(),
                                        fromtime = dsList.Tables[0].Rows[i]["fromtime"].ToString(),
                                        totime = dsList.Tables[0].Rows[i]["totime"].ToString(),
                                        checklist = dsList.Tables[0].Rows[i]["checklist"].ToString(),
                                        auditors = dsList.Tables[0].Rows[i]["auditors"].ToString(),
                                        auditee_team = dsList.Tables[0].Rows[i]["auditee_team"].ToString(),
                                        doc_req = dsList.Tables[0].Rows[i]["doc_req"].ToString(),
                                    };
                                    if (DateTime.TryParse(dsList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                                    {
                                        objAuditModel.AuditDate = dtValue;
                                    }
                                    objAudit.Obj.Add(objAuditModel);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AuditProcessEdit: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("AuditProcessList");
                                }
                            }
                            ViewBag.objList = objAudit;
                        }


                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AuditProcessEdit(FormCollection form, AuditProcessModels obj)
        {
            try
            {
                DateTime dateValue;
                obj.Audit_criteria = form["Audit_criteria"];
                obj.PlannedBy = form["PlannedBy"];
                obj.ApprovedBy = form["ApprovedBy"];

                if (DateTime.TryParse(form["AuditPlanDate"], out dateValue) == true)
                {
                    obj.AuditPlanDate = dateValue;
                }

                AuditProcessModelsList objAudit = new AuditProcessModelsList();
                objAudit.Obj = new List<AuditProcessModels>();


                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objModel = new AuditProcessModels();

                    if (form["branch" + i] != "" && form["branch" + i] != null)
                    {
                        objModel.Plan_Id = form["Plan_Id" + i];
                        objModel.branch = form["branch" + i];
                        objModel.dept_name = form["dept_name" + i];
                        objModel.team = form["team" + i];
                        objModel.location = form["location" + i];
                        objModel.fromtime = form["fromtime" + i];
                        objModel.totime = form["totime" + i];
                        objModel.checklist = form["checklist" + i];
                        objModel.auditors = form["auditors" + i];
                        objModel.auditee_team = form["auditee_team" + i];
                        objModel.doc_req = form["doc_req" + i];
                        if (DateTime.TryParse(form["AuditDate" + i], out dateValue) == true)
                        {
                            objModel.AuditDate = dateValue;
                        }
                        objAudit.Obj.Add(objModel);
                    }
                }

                if (obj.FunUpdateAuditProcess(obj, objAudit))
                {
                    TempData["Successdata"] = "Audit Plan details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditProcessList");
        }

        [AllowAnonymous]
        public ActionResult AuditProcessList(string branch_name)
        {
            ViewBag.SubMenutype = "AuditList";
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select Plan_Id,t.Audit_Id, Audit_no,branch,dept_name,team,location,AuditDate,fromtime,totime," +
                 "(CASE WHEN Approved_Status = '0' THEN 'Pending for Approval' WHEN Approved_Status = '1' THEN 'Rejected' ELSE 'Approved' END) as  Approved_Status," +
                    "checklist,auditors,auditee_team,Audit_Status from t_audit_process t, t_audit_process_plan tt where t.Audit_Id = tt.Audit_Id and active = 1";

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

                sSqlstmt = sSqlstmt + " order by t.Audit_Id desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditProcessModels objScheduleMdl = new AuditProcessModels
                            {
                                Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                Plan_Id = dsList.Tables[0].Rows[i]["Plan_Id"].ToString(),
                                Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
                                branch = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[i]["branch"].ToString()),
                                dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept_name"].ToString()),
                                //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[i]["team"].ToString()),

                                fromtime = dsList.Tables[0].Rows[i]["fromtime"].ToString(),
                                totime = dsList.Tables[0].Rows[i]["totime"].ToString(),
                                checklist = objGlobaldata.GetChecklistBychecklistId(dsList.Tables[0].Rows[i]["checklist"].ToString()),
                                Approved_Status = dsList.Tables[0].Rows[i]["Approved_Status"].ToString(),
                                Audit_Status = objGlobaldata.GetAuditStatusById(dsList.Tables[0].Rows[i]["Audit_Status"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["AuditDate"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["AuditDate"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.AuditDate = dtDocDate;
                            }

                            objAttList.Obj.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAttList.Obj.ToList());
        }

        [AllowAnonymous]
        public JsonResult AuditProcessDelete(string id_audit_schedule)
        {
            try
            {
                if (id_audit_schedule != "")
                {
                    AuditProcessModels Doc = new AuditProcessModels();
                    string sid_audit_schedule = id_audit_schedule;
                    if (Doc.FunDeleteAuditProcess(sid_audit_schedule))
                    {
                        TempData["Successdata"] = "Audit deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in AuditDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AuditApprovalDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {

                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    ViewBag.status = Request.QueryString["status"];
                    ViewBag.ApprStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditStatus");
                    ViewBag.AuditorStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditorStatus");
                    string sAudit_Id = Request.QueryString["Audit_Id"];
                    string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,ApprovedBy,logged_by,logged_date,audit_type,audit_code,"
                     + "(CASE WHEN Approved_Status = '0' THEN 'Pending for Approval' WHEN Approved_Status = '1' THEN 'Rejected' ELSE 'Approved' END) as  Approved_Status,audit_scope,audit_method,audit_objective,audit_lang,audit_methodology"
                        + " from t_audit_process where Audit_Id = '" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["logged_by"].ToString()),
                            Approved_Status = dsModels.Tables[0].Rows[0]["Approved_Status"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsModels.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_code = objGlobaldata.GetAuditProductById(dsModels.Tables[0].Rows[0]["audit_code"].ToString()),

                            //location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            //dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            //division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),
                            //Audit_Status = dsModels.Tables[0].Rows[0]["Audit_Status"].ToString(),
                            //Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            //checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            //FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            //ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            //Reasons_Reschedule = dsModels.Tables[0].Rows[0]["Reasons_Reschedule"].ToString(),

                            //auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
                            //internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            audit_scope = dsModels.Tables[0].Rows[0]["audit_scope"].ToString(),
                            audit_method = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_method"].ToString()),
                            audit_objective = dsModels.Tables[0].Rows[0]["audit_objective"].ToString(),
                            audit_lang = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_lang"].ToString()),
                            audit_methodology = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_methodology"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["logged_date"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }

                    //For approval details
                    string sSqlstmt1 = "select id_approval,Audit_Id,approver,apprv_date,comments,"
                     + "(CASE WHEN apprv_status = '0' THEN 'Pending for Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + "  from t_audit_process_approval where Audit_Id = '" + sAudit_Id + "'";
                    DataSet dsApprv = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objApprv = dsApprv;

                    //Audit plan
                    string sSqlstmt2 = "select * from t_audit_process_plan where Audit_Id = '" + sAudit_Id + "'";
                    DataSet dsPlan = objGlobaldata.Getdetails(sSqlstmt2);
                    ViewBag.objPlan = dsPlan;

                    // For Auditee pending for approval
                    string sSqlstmt3 = "select * from t_audit_process_plan where Audit_Id = '" + sAudit_Id + "' and find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "', auditee_team) ";
                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt3);
                    ViewBag.objAudit = dsAudit;

                    // For Auditor pending for approval
                    string sSqlstmt4 = "select * from t_audit_process_plan where Audit_Id = '" + sAudit_Id + "' and find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "', auditors) ";
                    DataSet dsAuditor = objGlobaldata.Getdetails(sSqlstmt4);
                    ViewBag.objAuditor = dsAuditor;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditApprovalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public ActionResult AuditDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {

                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    ViewBag.status = Request.QueryString["status"];
                    ViewBag.ApprStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditStatus");
                    string sAudit_Id = Request.QueryString["Audit_Id"];
                    string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,ApprovedBy,logged_by,logged_date,audit_type,audit_code,"
                     + "(CASE WHEN Approved_Status = '0' THEN 'Pending for Approval' WHEN Approved_Status = '1' THEN 'Rejected' ELSE 'Approved' END) as  Approved_Status,audit_scope,audit_method,audit_objective,audit_lang,audit_methodology"
                        + " from t_audit_process where Audit_Id = '" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            logged_by = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["logged_by"].ToString()),
                            Approved_Status = dsModels.Tables[0].Rows[0]["Approved_Status"].ToString(),
                            audit_type = objGlobaldata.GetAuidtTypeById(dsModels.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_code = objGlobaldata.GetAuditProductById(dsModels.Tables[0].Rows[0]["audit_code"].ToString()),

                            //location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            //dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            //division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),
                            //Audit_Status = dsModels.Tables[0].Rows[0]["Audit_Status"].ToString(),
                            //Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            //checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            //FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            //ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            //Reasons_Reschedule = dsModels.Tables[0].Rows[0]["Reasons_Reschedule"].ToString(),

                            //auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
                            //internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),

                            audit_scope = dsModels.Tables[0].Rows[0]["audit_scope"].ToString(),
                            audit_method = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_method"].ToString()),
                            audit_objective = dsModels.Tables[0].Rows[0]["audit_objective"].ToString(),
                            audit_lang = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_lang"].ToString()),
                            audit_methodology = objGlobaldata.GetDropdownitemById(dsModels.Tables[0].Rows[0]["audit_methodology"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["logged_date"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                    }

                    //For approval details
                    string sSqlstmt1 = "select id_approval,Audit_Id,approver,apprv_date,comments,"
                     + "(CASE WHEN apprv_status = '0' THEN 'Pending for Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + "  from t_audit_process_approval where Audit_Id = '" + sAudit_Id + "'";
                    DataSet dsApprv = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.objApprv = dsApprv;

                    //Audit plan
                    string sSqlstmt2 = "select * from t_audit_process_plan where Audit_Id = '" + sAudit_Id + "'";
                    DataSet dsPlan = objGlobaldata.Getdetails(sSqlstmt2);
                    ViewBag.objPlan = dsPlan;


                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //Approval
        public ActionResult AuditProcessApproval(AuditProcessModels objAuditModel, FormCollection form)
        {
            try
            {

                if (objAuditModel.FunAuditProcessApprove(objAuditModel))
                {
                    TempData["Successdata"] = "Approved Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }


        //AuditeeApproval
        public ActionResult AuditeeApproval(AuditProcessModels objAuditModel, FormCollection form)
        {
            try
            {

                AuditProcessModelsList objAudit = new AuditProcessModelsList();
                objAudit.Obj = new List<AuditProcessModels>();


                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objModel = new AuditProcessModels();
                    objModel.Plan_Id = form["Plan_Id" + i];
                    objModel.Audit_Id = form["Audit_Id" + i];
                    objModel.apprv_status = form["apprv_status" + i];
                    objModel.comments = form["comments" + i];
                    objAudit.Obj.Add(objModel);
                }
                if (objAuditModel.FunAuditeeApprove(objAudit))
                {
                    TempData["Successdata"] = "Confirmed Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditeeApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        //AuditorApproval
        public ActionResult AuditorApproval(AuditProcessModels objAuditModel, FormCollection form)
        {
            try
            {

                AuditProcessModelsList objAudit = new AuditProcessModelsList();
                objAudit.Obj = new List<AuditProcessModels>();


                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objModel = new AuditProcessModels();
                    objModel.Plan_Id = form["Plan_Id" + i];
                    objModel.Audit_Id = form["Audit_Id" + i];
                    objModel.apprv_status = form["apprv_status" + i];
                    objModel.comments = form["comments" + i];
                    objAudit.Obj.Add(objModel);
                }
                if (objAuditModel.FunAuditorApprove(objAudit))
                {
                    TempData["Successdata"] = "Confirmed Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }


        //Auditee List
        [AllowAnonymous]
        public ActionResult AuditeeListInfo(int id)
        {

            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select auditee,(CASE WHEN apprv_status = '0' THEN 'Pending for Confirmation' WHEN apprv_status = '1' THEN 'Not Confirmed' ELSE 'Confirmed' END) as apprv_status,apprv_date,comments from t_auditee_approval where Plan_Id ='" + id + "'";
                    DataSet dsAuditee = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.objAudit = dsAuditee;
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
                    string sSqlstmt = "select auditor,(CASE WHEN apprv_status = '0' THEN 'Pending for Confirmation' WHEN apprv_status = '1' THEN 'Not Confirmed' ELSE 'Confirmed' END) as apprv_status,apprv_date,comments from t_auditor_approval where Plan_Id ='" + id + "'";
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

        //Approver List
        [AllowAnonymous]
        public ActionResult ApproverListInfo(int id)
        {

            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select approver,(CASE WHEN apprv_status = '0' THEN 'Pending for Approval' WHEN apprv_status = '1' THEN 'Not Approved' ELSE 'Approved' END) as apprv_status,apprv_date,comments from t_audit_process_approval where Audit_Id ='" + id + "'";
                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.objAudit = dsAudit;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ApproverListInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Auditno exists
        public JsonResult FunCheckAuditNoExsists(string Audit_no)
        {
            AuditProcessModels objModel = new AuditProcessModels();
            var user = objModel.FunCheckAuditNoExsists(Audit_no);

            return Json(user);
        }

        //Auditor exists
        public JsonResult FunCheckAudiorExsists(string auditor_name)
        {
            AuditProcessModels objModel = new AuditProcessModels();
            var user = objModel.FunCheckAuditorExsists(auditor_name);

            return Json(user);
        }


        //Audit plan report
        [AllowAnonymous]
        public ActionResult AuditPlanPDF(FormCollection form)
        {
            try
            {
                AuditProcessModels objModel = new AuditProcessModels();
                string sAudit_Id = form["Audit_Id"];
                if (sAudit_Id != null && sAudit_Id != "")
                {
                    string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,ApprovedBy,logged_by,logged_date,"
                     + "(CASE WHEN Approved_Status = '0' THEN 'Pending for Approval' WHEN Approved_Status = '1' THEN 'Rejected' ELSE 'Approved' END) as  Approved_Status"
                        + " from t_audit_process where Audit_Id = '" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            ApprovedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }
                        ViewBag.Audit = objModel;
                    }
                    //Audit plan
                    string sSqlstmt2 = "select * from t_audit_process_plan where Audit_Id = '" + sAudit_Id + "'";
                    DataSet dsPlan = objGlobaldata.Getdetails(sSqlstmt2);
                    ViewBag.objPlan = dsPlan;
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditPlanPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string header = Server.MapPath("~/views/AuditProcess/PrintHeader.html");//Path of PrintHeader.html File

            string customSwitches = string.Format("--header-html  \"{0}\" " +
                                 "--header-spacing \"0\" ", header);

            return new ViewAsPdf("AuditPlanPDF", "AuditProcess")
            {
                //FileName = "AuditPlan.pdf",
                Cookies = cookieCollection,
                PageSize = Rotativa.Options.Size.A3,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                CustomSwitches =
                   customSwitches,
                PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 35 },
                // PageMargins = new Rotativa.Options.Margins(0, 3, 32, 3),
            };
        }

        //Audit Status    
        [AllowAnonymous]
        public ActionResult AuditStatusUpdate()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {

                if (Request.QueryString["Plan_Id"] != null && Request.QueryString["Plan_Id"] != "")
                {
                    string Plan_Id = Request.QueryString["Plan_Id"];

                    string sSqlstmt = "select Plan_Id,Audit_Id,branch,dept_name,team,location,AuditDate,fromtime,totime,checklist,auditors,auditee_team,Audit_Status,audit_status_date,remarks from t_audit_process_plan where Plan_Id='" + Plan_Id + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new AuditProcessModels
                        {
                            Plan_Id = dsModelsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_Id = dsModelsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsModelsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModelsList.Tables[0].Rows[0]["team"].ToString()),
                            location = objGlobaldata.GetDivisionLocationById(dsModelsList.Tables[0].Rows[0]["location"].ToString()),
                            fromtime = dsModelsList.Tables[0].Rows[0]["fromtime"].ToString(),
                            totime = dsModelsList.Tables[0].Rows[0]["totime"].ToString(),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModelsList.Tables[0].Rows[0]["checklist"].ToString()),
                            auditors = objGlobaldata.GetMultiHrEmpNameById(dsModelsList.Tables[0].Rows[0]["auditors"].ToString()),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModelsList.Tables[0].Rows[0]["auditee_team"].ToString()),
                            Audit_Status = dsModelsList.Tables[0].Rows[0]["Audit_Status"].ToString(),
                            remarks = dsModelsList.Tables[0].Rows[0]["remarks"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["audit_status_date"].ToString(), out dtValue))
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
        public ActionResult AuditStatusUpdate(FormCollection form, AuditProcessModels obj)
        {
            try
            {
                DateTime dateValue;
                obj.Audit_Status = form["Audit_Status"];
                obj.remarks = form["remarks"];

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
            return RedirectToAction("AuditProcessList");
        }

        //Raise NC List
        [AllowAnonymous]
        public ActionResult RaiseNCList(string branch_name)
        {
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();
            ViewBag.SubMenutype = "NCList";
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select Plan_Id,t.Audit_Id,Audit_no,branch,dept_name,team,audit_status_date,total_nc,auditors,auditee_team from t_audit_process_plan t,t_audit_process tt,"
                + "dropdownitems d where t.Audit_Id = tt.Audit_Id and t.Audit_Status = d.item_id and item_desc = 'Completed' and active=1";
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
                sSqlstmt = sSqlstmt + " order by Plan_Id desc";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditProcessModels objScheduleMdl = new AuditProcessModels
                            {
                                Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                Plan_Id = dsList.Tables[0].Rows[i]["Plan_Id"].ToString(),
                                Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
                                branch = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[i]["branch"].ToString()),
                                dept_name = objGlobaldata.GetDeptNameById(dsList.Tables[0].Rows[i]["dept_name"].ToString()),
                                //team = objGlobaldata.GetTeamNameByID(dsList.Tables[0].Rows[i]["team"].ToString()),
                                auditors = dsList.Tables[0].Rows[i]["auditors"].ToString(),
                                auditee_team = dsList.Tables[0].Rows[i]["auditee_team"].ToString(),
                                auditors_name = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["auditors"].ToString()),
                                auditee_team_name = objGlobaldata.GetMultiHrEmpNameById(dsList.Tables[0].Rows[i]["auditee_team"].ToString()),

                            };
                            if (dsList.Tables[0].Rows[i]["total_nc"].ToString() != "" && dsList.Tables[0].Rows[i]["total_nc"].ToString() != null)
                            {
                                objScheduleMdl.total_nc = Convert.ToInt32(dsList.Tables[0].Rows[i]["total_nc"].ToString());
                            }
                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["audit_status_date"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["audit_status_date"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.audit_status_date = dtDocDate;
                            }
                            objAttList.Obj.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in RaiseNCList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in RaiseNCList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAttList.Obj.ToList());
        }

        //Raise NC    
        [AllowAnonymous]
        public ActionResult RaiseNonconformity()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                if (Request.QueryString["Plan_Id"] != null && Request.QueryString["Plan_Id"] != "" && Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    string Plan_Id = Request.QueryString["Plan_Id"];
                    string Audit_Id = Request.QueryString["Audit_Id"];

                    string sSqlstmt = "select Plan_Id,t.Audit_Id,Audit_no,AuditDate,auditee_team,branch,dept_name,team"
                    + " from t_audit_process_plan t,t_audit_process tt where t.Audit_Id = tt.Audit_Id and t.Audit_Id = '" + Audit_Id + "' and Plan_Id = '" + Plan_Id + "'";

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Plan_Id = dsModelsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_Id = dsModelsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_no = dsModelsList.Tables[0].Rows[0]["Audit_no"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsModelsList.Tables[0].Rows[0]["branch"].ToString()),
                            dept_name = objGlobaldata.GetMultiDeptNameById(dsModelsList.Tables[0].Rows[0]["dept_name"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModelsList.Tables[0].Rows[0]["team"].ToString()),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModelsList.Tables[0].Rows[0]["auditee_team"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["AuditDate"].ToString(), out dtValue))
                        {
                            objModel.AuditDate = dtValue;
                        }
                    }
                    ViewBag.Category = objGlobaldata.GetAuditNCList();
                    ViewBag.ISOStds = objGlobaldata.GetIsoStdListbox();
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
        public ActionResult RaiseNonconformity(AuditProcessModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                DateTime dateValue;
                objModel.finding_details = form["finding_details"];
                objModel.finding_category = form["finding_category"];
                objModel.Audit_criteria = form["Audit_criteria"];
                objModel.audit_clause = form["audit_clause"];
                objModel.description = form["description"];

                if (DateTime.TryParse(form["nc_date"], out dateValue) == true)
                {
                    objModel.nc_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
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
                    TempData["Successdata"] = "NC Raised successfully";
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
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();
            MgmtDocumentsModels objMgmt = new MgmtDocumentsModels();

            try
            {
                if (Request.QueryString["Plan_Id"] != null && Request.QueryString["Plan_Id"] != "")
                {
                    string Plan_Id = Request.QueryString["Plan_Id"];
                    string id_nc = Request.QueryString["id_nc"];
                    string sSqlstmt = "select id_nc,Plan_Id,Audit_Id,nc_no,nc_date,finding_details,upload,finding_category,Audit_criteria,audit_clause,description,"
                      + "(CASE WHEN apprv_status = '0' THEN 'Pending for Auditee Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                        + " from t_audit_process_nc where Plan_Id = '" + Plan_Id + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AuditProcessModels objScheduleMdl = new AuditProcessModels
                                {
                                    id_nc = dsList.Tables[0].Rows[i]["id_nc"].ToString(),
                                    Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                    Plan_Id = dsList.Tables[0].Rows[i]["Plan_Id"].ToString(),
                                    nc_no = dsList.Tables[0].Rows[i]["nc_no"].ToString(),
                                    finding_details = dsList.Tables[0].Rows[i]["finding_details"].ToString(),
                                    upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                    finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[i]["finding_category"].ToString()),
                                    Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[i]["Audit_criteria"].ToString()),
                                    audit_clause = objMgmt.GetMultiISOClauseNameById(dsList.Tables[0].Rows[i]["audit_clause"].ToString()),
                                    description = dsList.Tables[0].Rows[i]["description"].ToString(),
                                    apprv_status = dsList.Tables[0].Rows[i]["apprv_status"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsList.Tables[0].Rows[i]["nc_date"].ToString() != ""
                                 && DateTime.TryParse(dsList.Tables[0].Rows[i]["nc_date"].ToString(), out dtDocDate))
                                {
                                    objScheduleMdl.nc_date = dtDocDate;
                                }
                                objAttList.Obj.Add(objScheduleMdl);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NonconformityList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }

                        string sql2 = "select id_nc,Plan_Id,Audit_Id,nc_no,nc_date,finding_details,upload,finding_category,Audit_criteria,audit_clause,description"
                  + " from t_audit_process_nc where id_nc = '" + id_nc + "'";
                        DataSet dsNCList = objGlobaldata.Getdetails(sql2);
                        if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                AuditProcessModels objNC = new AuditProcessModels
                                {
                                    id_nc = dsNCList.Tables[0].Rows[0]["id_nc"].ToString(),
                                    Audit_Id = dsNCList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                                    Plan_Id = dsNCList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                                    nc_no = dsNCList.Tables[0].Rows[0]["nc_no"].ToString(),
                                    finding_details = dsNCList.Tables[0].Rows[0]["finding_details"].ToString(),
                                    upload = dsNCList.Tables[0].Rows[0]["upload"].ToString(),
                                    finding_category = (dsNCList.Tables[0].Rows[0]["finding_category"].ToString()),
                                    Audit_criteria = (dsNCList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                                    audit_clause = (dsNCList.Tables[0].Rows[0]["audit_clause"].ToString()),
                                    description = dsNCList.Tables[0].Rows[0]["description"].ToString(),
                                };
                                DateTime dtDocDate;
                                if (dsNCList.Tables[0].Rows[0]["nc_date"].ToString() != ""
                                 && DateTime.TryParse(dsNCList.Tables[0].Rows[0]["nc_date"].ToString(), out dtDocDate))
                                {
                                    objNC.nc_date = dtDocDate;
                                }
                                ViewBag.NC = objNC;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in NonconformityList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                            ViewBag.AppClauses = objMgmt.GetMultiISOClauseList(dsNCList.Tables[0].Rows[0]["Audit_criteria"].ToString());
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "NC Not Raised";
                        return RedirectToAction("RaiseNCList");
                    }
                    ViewBag.Category = objGlobaldata.GetAuditNCList();
                    ViewBag.ISOStds = objGlobaldata.GetIsoStdListbox();
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
            return View(objAttList.Obj.ToList());
        }

        //NC update
        [HttpPost]
        [AllowAnonymous]
        public ActionResult NonconformityUpdate(FormCollection form, AuditProcessModels objModel, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;
                objModel.finding_details = form["finding_details"];
                objModel.finding_category = form["finding_category"];
                objModel.Audit_criteria = form["Audit_criteria"];
                objModel.audit_clause = form["audit_clause"];
                objModel.description = form["description"];

                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
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
                if (DateTime.TryParse(form["nc_date"], out dateValue) == true)
                {
                    objModel.nc_date = dateValue;
                }


                if (objModel.FunUpdateNonconformity(objModel))
                {
                    TempData["Successdata"] = "Nonconformity updated successfully";
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
            return RedirectToAction("NonconformityList", new { Plan_Id = objModel.Plan_Id });
        }


        //Nonconformity approval detail
        [AllowAnonymous]
        public ActionResult NonconformityApprovalDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            MgmtDocumentsModels objMgmt = new MgmtDocumentsModels();
            try
            {

                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    ViewBag.status = Request.QueryString["status"];
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.ApprStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditNC");
                    string sSqlstmt = "select id_nc,Plan_Id,T1.Audit_Id,Audit_no,nc_no,nc_date,finding_details,upload,finding_category,T1.Audit_criteria,audit_clause,description,T1.logged_by,T1.logged_date,aprvrejct_date,corrective_action_date,reason_rejection,upload_evidence,apprvby_auditee,followup_date,"
                      + "(CASE WHEN apprv_status = '0' THEN 'Pending for Auditee Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + " from t_audit_process_nc T1,t_audit_process T2 where T1.Audit_Id = T2.Audit_Id and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            id_nc = dsList.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_Id = dsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Plan_Id = dsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_no = dsList.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            audit_clause = objMgmt.GetMultiISOClauseNameById(dsList.Tables[0].Rows[0]["audit_clause"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            apprv_status = dsList.Tables[0].Rows[0]["apprv_status"].ToString(),
                            reason_rejection = dsList.Tables[0].Rows[0]["reason_rejection"].ToString(),
                            upload_evidence = dsList.Tables[0].Rows[0]["upload_evidence"].ToString(),
                            apprvby_auditee = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["apprvby_auditee"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["nc_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["nc_date"].ToString(), out dtDocDate))
                        {
                            objModel.nc_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_action_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["aprvrejct_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["aprvrejct_date"].ToString(), out dtDocDate))
                        {
                            objModel.aprvrejct_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["followup_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityApprovalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //NC Auditee Approval
        public ActionResult NonconformityAuditeeApproval(AuditProcessModels objAuditModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload_evidence)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (upload_evidence != null && files.ContentLength > 0)
                {
                    objAuditModel.upload_evidence = "";
                    foreach (var file in upload_evidence)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "NC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objAuditModel.upload_evidence = objAuditModel.upload_evidence + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in NonconformityAuditeeApproval-upload: " + ex.ToString());
                        }
                    }
                    objAuditModel.upload_evidence = objAuditModel.upload_evidence.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objAuditModel.FunNonconformityAuditeeApproval(objAuditModel))
                {
                    TempData["Successdata"] = "Approved Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityAuditeeApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        //NC Auditor update
        public ActionResult NonconformityAuditorUpdate(AuditProcessModels objAuditModel, FormCollection form)
        {
            try
            {

                if (objAuditModel.FunNonconformityAuditorUpdate(objAuditModel))
                {
                    TempData["Successdata"] = "Followup Date Saved Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in NonconformityAuditorUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        //Nonconformity  detail
        [AllowAnonymous]
        public ActionResult NonconformityDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            MgmtDocumentsModels objMgmt = new MgmtDocumentsModels();
            try
            {

                if (Request.QueryString["id_nc"] != null && Request.QueryString["id_nc"] != "")
                {
                    string sid_nc = Request.QueryString["id_nc"];
                    ViewBag.ApprStatus = objGlobaldata.GetConstantValueKeyValuePair("AuditNC");
                    string sSqlstmt = "select id_nc,Plan_Id,T1.Audit_Id,Audit_no,nc_no,nc_date,finding_details,upload,finding_category,T1.Audit_criteria,audit_clause,description,T1.logged_by,T1.logged_date,aprvrejct_date,corrective_action_date,reason_rejection,upload_evidence,apprvby_auditee,followup_date,"
                      + "(CASE WHEN apprv_status = '0' THEN 'Pending for Auditee Approval' WHEN apprv_status = '1' THEN 'Rejected' ELSE 'Approved' END) as  apprv_status"
                    + " from t_audit_process_nc T1,t_audit_process T2 where T1.Audit_Id = T2.Audit_Id and id_nc = '" + sid_nc + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            id_nc = dsList.Tables[0].Rows[0]["id_nc"].ToString(),
                            Audit_Id = dsList.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Plan_Id = dsList.Tables[0].Rows[0]["Plan_Id"].ToString(),
                            Audit_no = dsList.Tables[0].Rows[0]["Audit_no"].ToString(),
                            nc_no = dsList.Tables[0].Rows[0]["nc_no"].ToString(),
                            finding_details = dsList.Tables[0].Rows[0]["finding_details"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            finding_category = objGlobaldata.GetAuditNCById(dsList.Tables[0].Rows[0]["finding_category"].ToString()),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            audit_clause = objMgmt.GetMultiISOClauseNameById(dsList.Tables[0].Rows[0]["audit_clause"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["logged_by"].ToString()),
                            apprv_status = dsList.Tables[0].Rows[0]["apprv_status"].ToString(),
                            reason_rejection = dsList.Tables[0].Rows[0]["reason_rejection"].ToString(),
                            upload_evidence = dsList.Tables[0].Rows[0]["upload_evidence"].ToString(),
                            apprvby_auditee = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["apprvby_auditee"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsList.Tables[0].Rows[0]["nc_date"].ToString() != ""
                         && DateTime.TryParse(dsList.Tables[0].Rows[0]["nc_date"].ToString(), out dtDocDate))
                        {
                            objModel.nc_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["logged_date"].ToString() != ""
                       && DateTime.TryParse(dsList.Tables[0].Rows[0]["logged_date"].ToString(), out dtDocDate))
                        {
                            objModel.logged_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["corrective_action_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["corrective_action_date"].ToString(), out dtDocDate))
                        {
                            objModel.corrective_action_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["aprvrejct_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["aprvrejct_date"].ToString(), out dtDocDate))
                        {
                            objModel.aprvrejct_date = dtDocDate;
                        }
                        if (dsList.Tables[0].Rows[0]["followup_date"].ToString() != ""
                      && DateTime.TryParse(dsList.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
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


        //Add Auditor
        public ActionResult AddAuditorDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox("Email");
                ViewBag.standards = objGlobaldata.GetIsoStdListbox();
                ViewBag.Course = objGlobaldata.GetAuditorTypeOfCourseList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditorDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //Add Auditor
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddAuditorDetails(AuditProcessModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload_cetificate)
        {

            try
            {
                HttpPostedFileBase files = Request.Files[0];
                if (upload_cetificate != null && files.ContentLength > 0)
                {
                    objModel.upload_cetificate = "";
                    foreach (var file in upload_cetificate)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "Cert" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload_cetificate = objModel.upload_cetificate + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddAuditorDetails-upload: " + ex.ToString());

                        }
                    }
                    objModel.upload_cetificate = objModel.upload_cetificate.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                AuditProcessModelsList objModelsList = new AuditProcessModelsList();
                objModelsList.Obj = new List<AuditProcessModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objModels = new AuditProcessModels();
                    if (form["standards" + i] != null && form["standards" + i] != "")
                    {
                        objModels.standards = form["standards" + i];
                        objModels.type_course = form["type_course" + i];
                        objModels.yearsexp = form["yearsexp" + i];
                        objModels.upload = form["supload" + i];
                        DateTime dateValue;
                        if (DateTime.TryParse(form["completed_on" + i], out dateValue) == true)
                        {
                            objModels.completed_on = dateValue;
                        }

                        objModelsList.Obj.Add(objModels);
                    }
                }
                if (objModel.FunAddAuditorDetails(objModel, objModelsList))
                {
                    TempData["Successdata"] = "Added Auditor details Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditorDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditorDetailList");
        }

        //Auditor List
        [AllowAnonymous]
        public ActionResult AuditorDetailList()
        {
            ViewBag.SubMenutype = "AuditorList";
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();
            try
            {
                string sSqlstmt = "select id_auditor,auditor_name,auditor_no from t_auditor_detail where active=1";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditProcessModels objScheduleMdl = new AuditProcessModels
                            {
                                id_auditor = dsList.Tables[0].Rows[i]["id_auditor"].ToString(),
                                auditor_name = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[i]["auditor_name"].ToString()),
                                branch = objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetDivisionIdByHrEmpId(dsList.Tables[0].Rows[i]["auditor_name"].ToString())),
                                dept_name = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsList.Tables[0].Rows[i]["auditor_name"].ToString())),
                                //team = objGlobaldata.GetTeamNameByID(objGlobaldata.GetTeamIdByHrEmpId(dsList.Tables[0].Rows[i]["auditor_name"].ToString())),
                                auditor_no = dsList.Tables[0].Rows[i]["auditor_no"].ToString(),
                            };
                            objAttList.Obj.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditorDetailList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorDetailList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAttList.Obj.ToList());
        }

        [AllowAnonymous]
        public JsonResult AuditorDelete(string id_auditor)
        {
            try
            {
                if (id_auditor != "")
                {
                    AuditProcessModels Doc = new AuditProcessModels();
                    string sid_auditor = id_auditor;
                    if (Doc.FunDeleteAuditor(sid_auditor))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in AuditorDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //Auditor edit
        [AllowAnonymous]
        public ActionResult AuditorDetailsEdit()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {

                if (Request.QueryString["id_auditor"] != null && Request.QueryString["id_auditor"] != "")
                {
                    string id_auditor = Request.QueryString["id_auditor"];

                    string sSqlstmt = "select id_auditor,auditor_name,auditor_no,qualification,years_exp,trainings_completed,upload_cetificate from t_auditor_detail where id_auditor=" + id_auditor;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new AuditProcessModels
                        {
                            id_auditor = (dsModelsList.Tables[0].Rows[0]["id_auditor"].ToString()),
                            auditor_name = (dsModelsList.Tables[0].Rows[0]["auditor_name"].ToString()),
                            auditor_no = (dsModelsList.Tables[0].Rows[0]["auditor_no"].ToString()),
                            qualification = (dsModelsList.Tables[0].Rows[0]["qualification"].ToString()),
                            years_exp = (dsModelsList.Tables[0].Rows[0]["years_exp"].ToString()),
                            trainings_completed = (dsModelsList.Tables[0].Rows[0]["trainings_completed"].ToString()),
                            upload_cetificate = (dsModelsList.Tables[0].Rows[0]["upload_cetificate"].ToString()),
                        };
                    }
                    ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox("Email");
                    ViewBag.standards = objGlobaldata.GetIsoStdListbox();
                    ViewBag.Course = objGlobaldata.GetAuditorTypeOfCourseList();

                    AuditProcessModelsList objModelsList = new AuditProcessModelsList();
                    objModelsList.Obj = new List<AuditProcessModels>();

                    sSqlstmt = "select id_certificate,id_auditor,standards,type_course,completed_on,yearsexp,upload from t_auditor_detail_certificate where id_auditor='" + id_auditor + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                AuditProcessModels objCert = new AuditProcessModels
                                {
                                    id_certificate = dsList.Tables[0].Rows[i]["id_certificate"].ToString(),
                                    id_auditor = dsList.Tables[0].Rows[i]["id_auditor"].ToString(),
                                    standards = dsList.Tables[0].Rows[i]["standards"].ToString(),
                                    type_course = dsList.Tables[0].Rows[i]["type_course"].ToString(),
                                    yearsexp = dsList.Tables[0].Rows[i]["yearsexp"].ToString(),
                                    upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                };
                                DateTime dtValue;
                                if (DateTime.TryParse(dsList.Tables[0].Rows[i]["completed_on"].ToString(), out dtValue))
                                {
                                    objCert.completed_on = dtValue;
                                }
                                objModelsList.Obj.Add(objCert);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in AuditorDetailsEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("AuditorDetailList");
                            }
                        }
                        ViewBag.certList = objModelsList;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        //Auditor edit
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AuditorDetailsEdit(AuditProcessModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload_cetificate, IEnumerable<HttpPostedFileBase> supload0, IEnumerable<HttpPostedFileBase> supload1, IEnumerable<HttpPostedFileBase> supload2, IEnumerable<HttpPostedFileBase> supload3, IEnumerable<HttpPostedFileBase> supload4, IEnumerable<HttpPostedFileBase> supload5, IEnumerable<HttpPostedFileBase> supload6, IEnumerable<HttpPostedFileBase> supload7, IEnumerable<HttpPostedFileBase> supload8, IEnumerable<HttpPostedFileBase> supload9, IEnumerable<HttpPostedFileBase> supload10, IEnumerable<HttpPostedFileBase> supload_test)
        {

            try
            {
                IList<HttpPostedFileBase> upload_cetificateList = (IList<HttpPostedFileBase>)upload_cetificate;
                string QCDelete = Request.Form["QCDocsValselectall"];
                string QCDeletes = "";
                if (upload_cetificateList[0] != null)
                {
                    objModel.upload_cetificate = "";
                    foreach (var file in upload_cetificate)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "cert" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload_cetificate = objModel.upload_cetificate + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditorDetailsEdit-upload: " + ex.ToString());
                        }
                    }
                    objModel.upload_cetificate = objModel.upload_cetificate.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objModel.upload_cetificate = objModel.upload_cetificate + "," + form["QCDocsVal"];
                    objModel.upload_cetificate = objModel.upload_cetificate.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && upload_cetificateList[0] == null)
                {
                    objModel.upload_cetificate = null;
                }
                else if (form["QCDocsVal"] == null && upload_cetificateList[0] == null)
                {
                    objModel.upload_cetificate = null;
                }


                AuditProcessModelsList objModelsList = new AuditProcessModelsList();
                objModelsList.Obj = new List<AuditProcessModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objcert = new AuditProcessModels();
                    if (form["standards" + i] != null && form["standards" + i] != "")
                    {
                        objcert.standards = form["standards" + i];
                        objcert.type_course = form["type_course" + i];
                        objcert.yearsexp = form["yearsexp" + i];

                        DateTime dateValue;
                        if (DateTime.TryParse(form["completed_on" + i], out dateValue) == true)
                        {
                            objcert.completed_on = dateValue;
                        }


                        //certificate upload
                        IList<HttpPostedFileBase> upload = (IList<HttpPostedFileBase>)supload_test;
                        if (i == 0)
                        {
                            upload = (IList<HttpPostedFileBase>)supload0;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 1)
                        {
                            upload = (IList<HttpPostedFileBase>)supload1;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 2)
                        {
                            upload = (IList<HttpPostedFileBase>)supload2;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 3)
                        {
                            upload = (IList<HttpPostedFileBase>)supload3;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 4)
                        {
                            upload = (IList<HttpPostedFileBase>)supload4;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 5)
                        {
                            upload = (IList<HttpPostedFileBase>)supload5;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 6)
                        {
                            upload = (IList<HttpPostedFileBase>)supload6;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 7)
                        {
                            upload = (IList<HttpPostedFileBase>)supload7;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 8)
                        {
                            upload = (IList<HttpPostedFileBase>)supload8;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 9)
                        {
                            upload = (IList<HttpPostedFileBase>)supload9;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (i == 10)
                        {
                            upload = (IList<HttpPostedFileBase>)supload10;
                            QCDeletes = Request.Form["QCDocsValselectall" + i];
                        }
                        if (upload[0] != null)
                        {
                            objcert.upload = "";
                            foreach (var file in upload)
                            {
                                try
                                {
                                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                                    string sFilename = "cert" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                    file.SaveAs(sFilepath + "/" + sFilename);
                                    objcert.upload = objcert.upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AuditorDetailsEdit: " + ex.ToString());
                                }
                            }
                            objcert.upload = objcert.upload.Trim(',');
                        }
                        else
                        {
                            ViewBag.Message = "You have not specified a file.";
                        }
                        if (form["QCDocsVal" + i] != null && form["QCDocsVal" + i] != "")
                        {
                            objcert.upload = objcert.upload + "," + form["QCDocsVal" + i];
                            objcert.upload = objcert.upload.Trim(',');
                        }
                        else if (form["QCDocsVal" + i] == null && QCDeletes != null && upload[0] == null)
                        {
                            objcert.upload = null;
                        }
                        else if (form["QCDocsVal" + i] == null && upload[0] == null)
                        {
                            objcert.upload = null;
                        }
                        objModelsList.Obj.Add(objcert);
                    }
                }
                if (objModel.FunUpdateAuditorDetails(objModel, objModelsList))
                {
                    TempData["Successdata"] = "Updated Auditor details Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorDetailsEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditorDetailList");
        }

        //Auditor availability
        [AllowAnonymous]
        public ActionResult AddAuditorAvailability()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {

                if (Request.QueryString["id_auditor"] != null && Request.QueryString["id_auditor"] != "")
                {
                    string id_auditor = Request.QueryString["id_auditor"];

                    string sSqlstmt = "select id_auditor,auditor_name from t_auditor_detail where id_auditor='" + id_auditor + "'";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new AuditProcessModels
                        {
                            id_auditor = dsAuditList.Tables[0].Rows[0]["id_auditor"].ToString(),
                            auditor_name = objGlobaldata.GetEmpHrNameById(dsAuditList.Tables[0].Rows[0]["auditor_name"].ToString()),
                            branch = objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetDivisionIdByHrEmpId(dsAuditList.Tables[0].Rows[0]["auditor_name"].ToString())),
                            dept_name = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsAuditList.Tables[0].Rows[0]["auditor_name"].ToString())),
                            //team = objGlobaldata.GetTeamNameByID(objGlobaldata.GetTeamIdByHrEmpId(dsAuditList.Tables[0].Rows[0]["auditor_name"].ToString())),

                        };
                        AuditProcessModelsList objModelsList = new AuditProcessModelsList();
                        objModelsList.Obj = new List<AuditProcessModels>();

                        sSqlstmt = "select id_availability,id_auditor,from_date,to_date,comments from t_auditor_availability where id_auditor='" + objModel.id_auditor + "'";
                        DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    AuditProcessModels objAudit = new AuditProcessModels
                                    {
                                        id_availability = dsList.Tables[0].Rows[i]["id_availability"].ToString(),
                                        id_auditor = dsList.Tables[0].Rows[i]["id_auditor"].ToString(),
                                        comments = dsList.Tables[0].Rows[i]["comments"].ToString(),
                                    };
                                    DateTime dtValue;
                                    if (DateTime.TryParse(dsList.Tables[0].Rows[i]["from_date"].ToString(), out dtValue))
                                    {
                                        objAudit.from_date = dtValue;
                                    }
                                    if (DateTime.TryParse(dsList.Tables[0].Rows[i]["to_date"].ToString(), out dtValue))
                                    {
                                        objAudit.to_date = dtValue;
                                    }
                                    objModelsList.Obj.Add(objAudit);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in AddAuditorAvailability: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    return RedirectToAction("AuditorDetailList");
                                }
                            }
                            ViewBag.objList = objModelsList;
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditorAvailability: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objModel);
        }

        //Auditor availability
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddAuditorAvailability(AuditProcessModels objModel, FormCollection form)
        {
            try
            {

                AuditProcessModelsList objModelsList = new AuditProcessModelsList();
                objModelsList.Obj = new List<AuditProcessModels>();

                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    AuditProcessModels objModels = new AuditProcessModels();
                    if (form["from_date" + i] != null && form["from_date" + i] != "")
                    {
                        objModels.comments = form["comments" + i];
                        DateTime dateValue;
                        if (DateTime.TryParse(form["from_date" + i], out dateValue) == true)
                        {
                            objModels.from_date = dateValue;
                        }
                        if (DateTime.TryParse(form["to_date" + i], out dateValue) == true)
                        {
                            objModels.to_date = dateValue;
                        }
                        objModelsList.Obj.Add(objModels);
                    }
                }

                if (objModel.FunAddAuditorAvailability(objModelsList))
                {
                    TempData["Successdata"] = "Added Non-Availability Dates Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditorAvailability: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditorDetailList");
        }

        //Auditor details
        [AllowAnonymous]
        public ActionResult AuditorDetails()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {

                if (Request.QueryString["id_auditor"] != null && Request.QueryString["id_auditor"] != "")
                {
                    string id_auditor = Request.QueryString["id_auditor"];

                    string sSqlstmt = "select id_auditor,auditor_name,auditor_no,qualification,years_exp,trainings_completed,upload_cetificate,updated_on from t_auditor_detail where id_auditor=" + id_auditor;

                    DataSet dsModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModelsList.Tables.Count > 0 && dsModelsList.Tables[0].Rows.Count > 0)
                    {

                        objModel = new AuditProcessModels
                        {
                            id_auditor = (dsModelsList.Tables[0].Rows[0]["id_auditor"].ToString()),
                            auditor_name = objGlobaldata.GetEmpHrNameById(dsModelsList.Tables[0].Rows[0]["auditor_name"].ToString()),

                            branch = objGlobaldata.GetCompanyBranchNameById(objGlobaldata.GetDivisionIdByHrEmpId(dsModelsList.Tables[0].Rows[0]["auditor_name"].ToString())),
                            dept_name = objGlobaldata.GetDeptNameById(objGlobaldata.GetDeptIdByHrEmpId(dsModelsList.Tables[0].Rows[0]["auditor_name"].ToString())),
                            //team = objGlobaldata.GetTeamNameByID(objGlobaldata.GetTeamIdByHrEmpId(dsModelsList.Tables[0].Rows[0]["auditor_name"].ToString())),

                            auditor_no = (dsModelsList.Tables[0].Rows[0]["auditor_no"].ToString()),
                            qualification = (dsModelsList.Tables[0].Rows[0]["qualification"].ToString()),
                            years_exp = (dsModelsList.Tables[0].Rows[0]["years_exp"].ToString()),
                            trainings_completed = (dsModelsList.Tables[0].Rows[0]["trainings_completed"].ToString()),
                            upload_cetificate = (dsModelsList.Tables[0].Rows[0]["upload_cetificate"].ToString()),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsModelsList.Tables[0].Rows[0]["updated_on"].ToString(), out dtValue))
                        {
                            objModel.updated_on = dtValue;
                        }
                    }

                    string sSqlstmt1 = "select * from t_auditor_detail_certificate where id_auditor = '" + id_auditor + "'";
                    DataSet dsCert = objGlobaldata.Getdetails(sSqlstmt1);
                    ViewBag.dsCert = dsCert;

                    string sSqlstmt2 = "select * from t_auditor_availability where id_auditor = '" + id_auditor + "'";
                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt2);
                    ViewBag.dsAudit = dsAudit;

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditorDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        public JsonResult UploadDoc()
        {
            HttpFileCollectionBase upload = Request.Files;
            string sUpload = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                string sFilename = "Cert" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + sFilename);
                sUpload = sUpload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
            }
            sUpload = sUpload.Trim(',');
            return Json(sUpload);

        }

        [AllowAnonymous]
        public ActionResult AuditType(int? page)
        {
            AuditProcessModelsList objModelsList = new AuditProcessModelsList();
            objModelsList.Obj = new List<AuditProcessModels>();
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                string sSqlstmt = "select id_audit_type,audit_type,audit_code,audit_desc from t_audit_type where active = 1 order by id_audit_type desc; ";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                ViewBag.Audit = objGlobaldata.GetAuidtTypeList();
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objModel = new AuditProcessModels
                            {
                                id_audit_type = (dsList.Tables[0].Rows[i]["id_audit_type"].ToString()),
                                audit_type = objGlobaldata.GetAuidtTypeById(dsList.Tables[0].Rows[i]["audit_type"].ToString()),
                                audit_code = (dsList.Tables[0].Rows[i]["audit_code"].ToString()),
                                audit_desc = (dsList.Tables[0].Rows[i]["audit_desc"].ToString()),

                            };
                            objModelsList.Obj.Add(objModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditType: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditType: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModelsList.Obj.ToList());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddAuditType(AuditProcessModels objModel, FormCollection form)
        {
            try
            {
                if (objModel.FunAddAuditType(objModel))
                {
                    TempData["Successdata"] = "Added Successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditType: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditType");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuditTypeEdit()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {

                if (Request.QueryString["id_audit_type"] != null && Request.QueryString["id_audit_type"] != "")
                {
                    string id_audit_type = Request.QueryString["id_audit_type"];
                    ViewBag.Audit = objGlobaldata.GetAuidtTypeList();
                    string sSqlstmt = "select id_audit_type,audit_type,audit_code,audit_desc from t_audit_type where id_audit_type='" + id_audit_type + "'";
                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            id_audit_type = (dsList.Tables[0].Rows[0]["id_audit_type"].ToString()),
                            audit_type = (dsList.Tables[0].Rows[0]["audit_type"].ToString()),
                            audit_code = (dsList.Tables[0].Rows[0]["audit_code"].ToString()),
                            audit_desc = (dsList.Tables[0].Rows[0]["audit_desc"].ToString()),
                        };
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("AuditType");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("AuditType");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditTypeEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("AuditType");
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditTypeEdit(AuditProcessModels objModel, FormCollection form)
        {
            try
            {

                if (objModel.FunUpdateAuditType(objModel))
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
                objGlobaldata.AddFunctionalLog("Exception in AuditTypeEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditType");
        }

        [AllowAnonymous]
        public JsonResult AuditTypeDelete(FormCollection form)
        {
            try
            {
                if (form["Id"] != null && form["Id"] != "")
                {
                    AuditProcessModels objModel = new AuditProcessModels();
                    string sId = form["Id"];

                    if (objModel.FunDeleteAuditType(sId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in AuditTypeDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //AuditCode exists
        public JsonResult FunCheckAuditCodeExsists(string audit_code)
        {
            AuditProcessModels objModel = new AuditProcessModels();
            var user = objModel.FunCheckAuditCodeExsists(audit_code);
            return Json(user);
        }
        //public JsonResult AuditProcessApproveNoty(string Audit_Id, string iStatus, string PendingFlg)
        //{
        //    try
        //    {
        //        AuditProcessModels objModel = new AuditProcessModels();

        //        string sStatus = "";
        //        if (iStatus == "0")
        //        {
        //            sStatus = "Pending";
        //        }
        //        else if (iStatus == "1")
        //        {
        //            sStatus = "Approved";
        //        }
        //        else if (iStatus == "2")
        //        {
        //            sStatus = "Rejected";
        //        }
        //        if (objModel.FunApproveOrReject(Audit_Id, sStatus))
        //        {
        //            TempData["Successdata"] = " Audit " + sStatus;
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //        return Json("Success");
        //    }

        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AccessPermitApproveReject: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return Json("Fail");
        //}
        public ActionResult AuditProcessApproveDeptHead(string Audit_Id, string iStatus, string PendingFlg)
        {
            try
            {
                AuditProcessModels objModel = new AuditProcessModels();

                if (objModel.FunApproveOrRejectDeptHead(Audit_Id, iStatus))
                {
                    TempData["Successdata"] = "Audit Process Approved";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessApproveDeptHead: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("AuditProcessList");
            }
        }

        public ActionResult AuditProcessApprove(string Audit_Id, string iStatus, string PendingFlg)
        {
            try
            {
                AuditProcessModels objModel = new AuditProcessModels();

                string Sql = "Select audit_team_status,dept_head_status from t_audit_process where Audit_Id='" + Audit_Id + "'";
                DataSet dsAudit = objGlobaldata.Getdetails(Sql);

                if (dsAudit != null && dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                {
                    objModel.audit_team_status = dsAudit.Tables[0].Rows[0]["audit_team_status"].ToString();
                    objModel.dept_head_status = dsAudit.Tables[0].Rows[0]["dept_head_status"].ToString();
                }

                if (objModel.audit_team_status == "1" && objModel.dept_head_status == "1")
                {
                    if (objModel.FunApproveOrReject(Audit_Id, iStatus))
                    {
                        TempData["Successdata"] = "Audit Process Approved";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (objModel.FunApproveOrRejectAuditTeam(Audit_Id, iStatus))
                    {
                        TempData["Successdata"] = "Audit Process Approved";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }

            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("AuditProcessList");
            }
        }


        public JsonResult AuditProcessApproveNotyDeptHead(string Audit_Id, string iStatus, string PendingFlg)
        {
            try
            {
                AuditProcessModels objModel = new AuditProcessModels();

                if (objModel.FunApproveOrRejectDeptHead(Audit_Id, iStatus))
                {
                    TempData["Successdata"] = "Audit Process Approved";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

                return Json("Success");
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessApproveNotyDeptHead: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Fail");
        }

        public JsonResult AuditProcessApproveNoty(string Audit_Id, string iStatus, string PendingFlg)
        {
            try
            {
                AuditProcessModels objModel = new AuditProcessModels();

                string Sql = "Select audit_team_status,dept_head_status from t_audit_process where Audit_Id='" + Audit_Id + "'";
                DataSet dsAudit = objGlobaldata.Getdetails(Sql);

                if (dsAudit != null && dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                {
                    objModel.audit_team_status = dsAudit.Tables[0].Rows[0]["audit_team_status"].ToString();
                    objModel.dept_head_status = dsAudit.Tables[0].Rows[0]["dept_head_status"].ToString();
                }

                if (objModel.audit_team_status == "1" && objModel.dept_head_status == "1")
                {
                    if (objModel.FunApproveOrReject(Audit_Id, iStatus))
                    {
                        TempData["Successdata"] = "Audit Process Approved";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    if (objModel.FunApproveOrRejectAuditTeam(Audit_Id, iStatus))
                    {
                        TempData["Successdata"] = "Audit Process Approved";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return Json("Success");
            }

            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AccessPermitApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Fail");
        }

        [AllowAnonymous]
        public JsonResult AuditProcessRescedule(string id_audit_schedule, string Rescedule_Reason)
        {
            try
            {
                if (id_audit_schedule != "")
                {
                    AuditProcessModels Doc = new AuditProcessModels();
                    string sid_audit_schedule = id_audit_schedule;
                    if (Doc.FunResceduleAuditProcess(sid_audit_schedule, Rescedule_Reason))
                    {
                        TempData["Successdata"] = "Audit Resceduled successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in Reschedule: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //[AllowAnonymous]
        //public ActionResult AuditDetails()
        //{
        //    AuditProcessModels objModel = new AuditProcessModels();                   
        //    try
        //    {            

        //        if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
        //        {
        //            string sAudit_Id = Request.QueryString["Audit_Id"];
        //            string sSqlstmt = "select Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no, AuditPlanDate, AuditDate, dept,  location, division,team,"
        //            + "internal_audit_team, Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,dept_head,auditee_team from t_audit_process where Audit_Id ='" + sAudit_Id + "'";

        //            DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

        //            if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
        //            {
        //                objModel = new AuditProcessModels
        //                {
        //                    Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
        //                    Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
        //                    Audit_no= dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
        //                    location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
        //                    dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
        //                    division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
        //                    team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),
        //                    Audit_Status = dsModels.Tables[0].Rows[0]["Audit_Status"].ToString(),
        //                    Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
        //                    checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
        //                    FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
        //                    ToPlanTimeInHour= dsModels.Tables[0].Rows[0]["totime"].ToString(),
        //                    Reasons_Reschedule = dsModels.Tables[0].Rows[0]["Reasons_Reschedule"].ToString(),
        //                    PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
        //                    auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
        //                    internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
        //                 };
        //                DateTime dtDocDate;
        //                if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
        //                 && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
        //                {
        //                    objModel.AuditPlanDate = dtDocDate;
        //                }

        //                if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
        //             && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
        //                {
        //                    objModel.AuditDate = dtDocDate;
        //                }
        //            }
        //            AuditElementsModels objElement = new AuditElementsModels();
        //            ViewBag.Question = objElement.GetQuestionsListbox(dsModels.Tables[0].Rows[0]["checklist"].ToString());

        //            string sSqlstmt3 = "select * from t_audit_process_perform where Audit_Id='" + sAudit_Id + "'";
        //            DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt3);
        //            ViewBag.objAudit = dsAudit;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AuditDetails: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View(objModel);
        //}

        [AllowAnonymous]
        public ActionResult AuditDetailsPDF(FormCollection form)
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {
                if (form["Audit_Id"] != null && form["Audit_Id"] != "")
                {
                    string sAudit_Id = form["Audit_Id"];
                    string sSqlstmt = "select Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no, AuditPlanDate, AuditDate, dept,  location, division,team,"
                    + "internal_audit_team, Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,auditee_team from t_audit_process where Audit_Id ='" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),
                            Audit_Status = dsModels.Tables[0].Rows[0]["Audit_Status"].ToString(),
                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            Reasons_Reschedule = dsModels.Tables[0].Rows[0]["Reasons_Reschedule"].ToString(),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),

                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }
                    }
                    ViewBag.Audit = objModel;

                    AuditElementsModels objElement = new AuditElementsModels();
                    ViewBag.Question = objElement.GetQuestionsListbox(dsModels.Tables[0].Rows[0]["checklist"].ToString());

                    string sSqlstmt3 = "select * from t_audit_process_perform where Audit_Id='" + sAudit_Id + "'";
                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt3);
                    ViewBag.AuditTrans = dsAudit;

                    CompanyModels objCompany = new CompanyModels();
                    dsModels = objCompany.GetCompanyDetailsForReport(dsModels);
                    dsModels = objGlobaldata.GetReportDetails(dsModels, objModel.Audit_no, objGlobaldata.GetCurrentUserSession().empid, "AUDIT PROCESS REPORT");

                    ViewBag.CompanyInfo = dsModels;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditDetailsPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("AuditDetailsPDF")
            {
                //FileName = "AuditDetails.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public ActionResult AuditProcessInfo(int id)
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {
                if (id > 0)
                {
                    string sSqlstmt = "select Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no, AuditPlanDate, AuditDate, dept,  location, division,"
                     + "internal_audit_team, Reasons_Reschedule, Notified_To,Audit_Status,fromtime,totime from t_audit_process where Audit_Id ='" + id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            Audit_Status = dsModels.Tables[0].Rows[0]["Audit_Status"].ToString(),
                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            Reasons_Reschedule = dsModels.Tables[0].Rows[0]["Reasons_Reschedule"].ToString(),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [AllowAnonymous]
        public ActionResult PerformAudit()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {
                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    string sAudit_Id = Request.QueryString["Audit_Id"];
                    string sSqlstmt = "select Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no, AuditPlanDate, AuditDate, dept,  location, division,team,"
                    + "internal_audit_team, Reasons_Reschedule, Notified_To,Audit_Status, fromtime, totime,auditee_team,dept_head from t_audit_process where Audit_Id ='" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),

                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            checklistId = (dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),

                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            dept_head = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["dept_head"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }


                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }
                        AuditElementsModels obj = new AuditElementsModels();
                        // string sSqlstmt2 = "select b.Questions from t_auditchecklist a, t_auditchecklist_trans b where a.id_AuditChecklist = b.id_AuditChecklist and a.id_AuditChecklist='" + dsModels.Tables[0].Rows[0]["checklist"].ToString() + "' ";
                        //string sSqlstmt2 = "select Questions from t_auditchecklist where id_AuditChecklist='" + dsModels.Tables[0].Rows[0]["checklist"].ToString() + "'";
                        //DataSet dsQuestion = objGlobaldata.Getdetails(sSqlstmt2);


                        //if (dsQuestion.Tables.Count > 0 && dsQuestion.Tables[0].Rows.Count > 0)
                        //{
                        //    ViewBag.Question = dsQuestion.Tables[0].Rows[0]["Questions"].ToString();
                        //}
                        ViewBag.AuditeeTeam = objGlobaldata.GetHrEmpListByDept(dsModels.Tables[0].Rows[0]["dept"].ToString());

                        ViewBag.Question = obj.GetQuestionsListbox(dsModels.Tables[0].Rows[0]["checklist"].ToString());
                        //ViewBag.AuditElements = obj.GetAuditElementsListbox(dsModels.Tables[0].Rows[0]["dept"].ToString());

                        IDictionary<string, string> findingCategory = new Dictionary<string, string>();
                        findingCategory.Add("Conformance", "Conformance"); //adding a key/value using the Add() method
                        findingCategory.Add("Major Non Conformance", "Major Non Conformance");
                        findingCategory.Add("Minor Non Conformance", "Minor Non Conformance");
                        findingCategory.Add("Potential Non Conformance", "Potential Non Conformance");
                        findingCategory.Add("Note-worthy finding", "Note-worthy finding");
                        ViewBag.FindingCategory = findingCategory;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAudit(AuditProcessPerformModels objAudtChecklist, FormCollection form)
        {
            try
            {

                string AuditId = form["Audit_Id"];
                string status = form["Audit_Status"];
                string checklistId = form["checklistId"];
                string sauditee_team = form["auditee_team"];

                string sSqlstmt2 = "update t_audit_process set Audit_Status='" + status + "',auditee_team = '" + sauditee_team + "' where Audit_Id='" + AuditId + "'";
                objGlobaldata.Getdetails(sSqlstmt2);


                AuditProcessPerformModelsList objAudit = new AuditProcessPerformModelsList();
                objAudit.lstAudit = new List<AuditProcessPerformModels>();

                AuditElementsModels obj = new AuditElementsModels();


                // string sSqlstmt = "select dept"+ " from t_audit_process where Audit_Id ='" + AuditId + "'";

                //DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                //MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(dsModels.Tables[0].Rows[0]["dept"].ToString());
                MultiSelectList AuditQuestions = obj.GetQuestionsListbox(checklistId);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= cnt)
                    {
                        AuditProcessPerformModels objElements = new AuditProcessPerformModels();
                        objElements.Audit_Elements = form["Audit_Elements " + item.Value];
                        objElements.Details = form["Details " + i];
                        objElements.Evidence = form["Evidence " + i];

                        objElements.Category = form["Category " + i];
                        objElements.Conformance = form["Conformance " + i];
                        objElements.Non_comformance = form["Non-comformance " + i];
                        objElements.evidence_upload = form["evidence_upload" + i];
                        objElements.Major_Non_Conformances = form["Major_Non_Conformances"];
                        objElements.TOtal_Conformances = form["TOtal_Conformances"];
                        objElements.Minor_Non_Conformances = form["Minor_Non_Conformances"];
                        objElements.Note_Worth_Findings = form["Note_Worth_Findings"];
                        objElements.Potential_Non_Conformances = form["Potential_Non_Conformances"];

                        objElements.Audit_Id = AuditId;
                        objAudit.lstAudit.Add(objElements);
                    }
                    i++;
                }


                if (objAudtChecklist.FunAddAuditPerformance(objAudit))
                {
                    TempData["Successdata"] = "Audit Performance details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditProcessList");
        }

        [AllowAnonymous]
        public ActionResult PerformAuditEdit()
        {
            AuditProcessModels objModel = new AuditProcessModels();

            try
            {

                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["Audit_Id"] != "")
                {
                    string sAudit_Id = Request.QueryString["Audit_Id"];
                    string sSqlstmt = "select Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no, AuditPlanDate, AuditDate, dept,  location, division,team,"
                    + "internal_audit_team, Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,auditee_team,dept_head from t_audit_process where Audit_Id ='" + sAudit_Id + "'";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            //team = objGlobaldata.GetTeamNameByID(dsModels.Tables[0].Rows[0]["team"].ToString()),

                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklistId = (dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),

                            dept_head = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["dept_head"].ToString()),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            auditee_team = /*objGlobaldata.GetMultiHrEmpNameById*/(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }


                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                     && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }
                        AuditElementsModels obj = new AuditElementsModels();
                        //string sSqlstmt2 = "select Questions from t_auditchecklist where id_AuditChecklist='" + dsModels.Tables[0].Rows[0]["checklist"].ToString() + "'";
                        //DataSet dsQuestion = objGlobaldata.Getdetails(sSqlstmt2);


                        //if (dsQuestion.Tables.Count > 0 && dsQuestion.Tables[0].Rows.Count > 0)
                        //{
                        //    ViewBag.Question = dsQuestion.Tables[0].Rows[0]["Questions"].ToString();
                        //}
                        //ViewBag.AuditElements = obj.GetAuditElementsListbox(dsModels.Tables[0].Rows[0]["dept"].ToString());
                        ViewBag.AuditeeTeam = objGlobaldata.GetHrEmpListByDept(dsModels.Tables[0].Rows[0]["dept"].ToString());

                        ViewBag.Question = obj.GetQuestionsListbox(dsModels.Tables[0].Rows[0]["checklist"].ToString());

                        IDictionary<string, string> findingCategory = new Dictionary<string, string>();
                        findingCategory.Add("Conformance", "Conformance"); //adding a key/value using the Add() method
                        findingCategory.Add("Major Non Conformance", "Major Non Conformance");
                        findingCategory.Add("Minor Non Conformance", "Minor Non Conformance");
                        findingCategory.Add("Potential Non Conformance", "Potential Non Conformance");
                        findingCategory.Add("Note-worthy finding", "Note-worthy finding");
                        ViewBag.FindingCategory = findingCategory;



                        string sSqlstmt3 = "select * from t_audit_process_perform where Audit_Id='" + sAudit_Id + "'";

                        DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt3);
                        //AuditProcessPerformModelsList objAudit = new AuditProcessPerformModelsList();
                        //objAudit.lstAudit = new List<AuditProcessPerformModels>();


                        //if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                        //{

                        //    for (int i = 0; dsAudit.Tables.Count > 0 && i < dsAudit.Tables[0].Rows.Count; i++)
                        //    {
                        //        AuditProcessPerformModels objElements = new AuditProcessPerformModels();
                        //        objElements.Audit_Elements = dsAudit.Tables[0].Rows[i]["Audit_Elements"].ToString();
                        //        objElements.Details = dsAudit.Tables[0].Rows[i]["Details"].ToString();
                        //        objElements.Evidence = dsAudit.Tables[0].Rows[i]["Evidence"].ToString();

                        //        objElements.Category = dsAudit.Tables[0].Rows[i]["Category"].ToString();
                        //        objElements.Conformance = dsAudit.Tables[0].Rows[i]["Conformance"].ToString();
                        //        objElements.Non_comformance = dsAudit.Tables[0].Rows[i]["Non_comformance"].ToString();
                        //        objElements.evidence_upload = dsAudit.Tables[0].Rows[i]["evidence_upload"].ToString();
                        //        objElements.Major_Non_Conformances = dsAudit.Tables[0].Rows[i]["Major_Non_Conformances"].ToString();
                        //        objElements.TOtal_Conformances = dsAudit.Tables[0].Rows[i]["TOtal_Conformances"].ToString();
                        //        objElements.Minor_Non_Conformances = dsAudit.Tables[0].Rows[i]["Minor_Non_Conformances"].ToString();
                        //        objElements.Note_Worth_Findings = dsAudit.Tables[0].Rows[i]["Note_Worth_Findings"].ToString();

                        //        objElements.Audit_Id = dsAudit.Tables[0].Rows[i]["Audit_Id"].ToString(); ;
                        //        objAudit.lstAudit.Add(objElements);

                        //    }

                        //}

                        ViewBag.objAudit = dsAudit;

                    }

                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAuditEdit(AuditProcessPerformModels objAudtChecklist, FormCollection form)
        {
            try
            {

                string AuditId = form["Audit_Id"];
                string status = form["Audit_Status"];
                string checklistId = form["checklistId"];
                string sauditee_team = form["auditee_team"];

                string sSqlstmt2 = "update t_audit_process set Audit_Status='" + status + "', auditee_team ='" + sauditee_team + "' where Audit_Id='" + AuditId + "'";
                objGlobaldata.Getdetails(sSqlstmt2);

                AuditProcessPerformModelsList objAudit = new AuditProcessPerformModelsList();
                objAudit.lstAudit = new List<AuditProcessPerformModels>();

                AuditElementsModels obj = new AuditElementsModels();


                //                string sSqlstmt = "select dept"
                //+ " from t_audit_process where Audit_Id ='" + AuditId + "'";

                //                DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                //                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(dsModels.Tables[0].Rows[0]["dept"].ToString());
                MultiSelectList AuditQuestions = obj.GetQuestionsListbox(checklistId);

                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= cnt)
                    {
                        AuditProcessPerformModels objElements = new AuditProcessPerformModels();
                        objElements.id_audit_process_perform = form["id_audit_process_perform " + i];

                        objElements.Audit_Elements = form["Audit_Elements " + item.Value];
                        objElements.Details = form["Details " + i];
                        objElements.Evidence = form["Evidence " + i];

                        objElements.Category = form["Category " + i];
                        objElements.Conformance = form["Conformance " + i];
                        objElements.Non_comformance = form["Non-comformance " + i];
                        string upload = form["evidence_upload" + i];
                        if (upload != null)
                        {
                            objElements.evidence_upload = form["evidence_upload" + i];

                        }

                        //objElements.evidence_upload = form["evidence_upload" + i];
                        objElements.Major_Non_Conformances = form["Major_Non_Conformances"];
                        objElements.TOtal_Conformances = form["TOtal_Conformances"];
                        objElements.Minor_Non_Conformances = form["Minor_Non_Conformances"];
                        objElements.Note_Worth_Findings = form["Note_Worth_Findings"];
                        objElements.Potential_Non_Conformances = form["Potential_Non_Conformances"];

                        objElements.Audit_Id = AuditId;
                        objAudit.lstAudit.Add(objElements);
                    }
                    i++;
                }


                if (objAudtChecklist.FunUpdateAuditPerformance(objAudit))
                {
                    TempData["Successdata"] = "Audit Performance details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditProcessList");
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditorList(string branch_name)
        {
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select Audit_Id, Audit_no, AuditDate, dept, location, division, Audit_Status"
         + " from t_audit_process where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + " order by Audit_Id desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditProcessModels objScheduleMdl = new AuditProcessModels
                            {
                                Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[i]["division"].ToString()),
                                Audit_Status = dsList.Tables[0].Rows[i]["Audit_Status"].ToString(),

                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["AuditDate"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["AuditDate"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.AuditDate = dtDocDate;
                            }

                            objAttList.Obj.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAttList.Obj.ToList());
        }

        [AllowAnonymous]
        public ActionResult ExternalAuditorAvailabilityList(string branch_name)
        {
            AuditProcessModelsList objAttList = new AuditProcessModelsList();
            objAttList.Obj = new List<AuditProcessModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiCompanyBranchNameByID(sBranchtree);

                string sSqlstmt = "select Audit_Id, Audit_no, AuditDate, dept, location, division, Audit_Status"
         + " from t_audit_process where active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSqlstmt = sSqlstmt + " and find_in_set('" + sBranch_name + "', division)";
                }

                sSqlstmt = sSqlstmt + " order by Audit_Id desc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            AuditProcessModels objScheduleMdl = new AuditProcessModels
                            {
                                Audit_Id = dsList.Tables[0].Rows[i]["Audit_Id"].ToString(),
                                Audit_no = dsList.Tables[0].Rows[i]["Audit_no"].ToString(),
                                location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[i]["location"].ToString()),
                                dept = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[i]["dept"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsList.Tables[0].Rows[i]["division"].ToString()),
                                Audit_Status = dsList.Tables[0].Rows[i]["Audit_Status"].ToString(),

                            };

                            DateTime dtDocDate;
                            if (dsList.Tables[0].Rows[i]["AuditDate"].ToString() != ""
                             && DateTime.TryParse(dsList.Tables[0].Rows[i]["AuditDate"].ToString(), out dtDocDate))
                            {
                                objScheduleMdl.AuditDate = dtDocDate;
                            }

                            objAttList.Obj.Add(objScheduleMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditProcessList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objAttList.Obj.ToList());
        }

        public ActionResult AddExternalAuditorAvailability()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                objModel.division = objGlobaldata.GetCurrentUserSession().division;
                objModel.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                objModel.PlannedBy = objGlobaldata.GetCurrentUserSession().empid;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objGlobaldata.GetCurrentUserSession().division);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objGlobaldata.GetCurrentUserSession().division);
                ViewBag.AuditCriteria = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.AuditScope = objGlobaldata.GetScheduleAuditScopeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.CheckList = objGlobaldata.GetChecklistTypeByChecklistRef();
                ViewBag.EmpList = objGlobaldata.GetHrEmpListByDept(objModel.dept);

                ViewBag.Auditor = objGlobaldata.GetAuditor();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditSchedule: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }



            return View(objModel);
        }

        public ActionResult AddExternalAuditor()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                objModel.division = objGlobaldata.GetCurrentUserSession().division;
                objModel.dept = objGlobaldata.GetCurrentUserSession().DeptID;
                objModel.location = objGlobaldata.GetCurrentUserSession().Work_Location;
                objModel.PlannedBy = objGlobaldata.GetCurrentUserSession().empid;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objGlobaldata.GetCurrentUserSession().division);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objGlobaldata.GetCurrentUserSession().division);
                ViewBag.AuditCriteria = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.AuditScope = objGlobaldata.GetScheduleAuditScopeList();
                ViewBag.PlanTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.PlanTimeInMin = objGlobaldata.GetAuditTimeInMin();
                ViewBag.CheckList = objGlobaldata.GetChecklistTypeByChecklistRef();
                ViewBag.EmpList = objGlobaldata.GetHrEmpListByDept(objModel.dept);

                ViewBag.Auditor = objGlobaldata.GetAuditor();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAuditSchedule: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }



            return View(objModel);
        }

        public ActionResult AuditNonConfirmity()
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                if (Request.QueryString["Audit_Id"] != null && Request.QueryString["id_audit_process_perform"] != null && Request.QueryString["Audit_Id"] != "" && Request.QueryString["id_audit_process_perform"] != "")
                {
                    string sAudit_Id = Request.QueryString["Audit_Id"];
                    string sid_audit_process_perform = Request.QueryString["id_audit_process_perform"];
                    string sSqlstmt = "select a.Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no," +
                        " AuditPlanDate, AuditDate, dept,  location, division,internal_audit_team, dept_head," +
                        "Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,auditee_team,Audit_Elements," +
                        "id_audit_process_perform,Details,Evidence,Category,Conformance,Non_comformance,evidence_upload," +
                        "why_nc,auditee_acceptance,ca_date,followup_date,nc_step_status,nc_status,nc_reject_reason,nc_reject_upload,nc_reject_response from t_audit_process a," +
                        " t_audit_process_perform b where id_audit_process_perform='" + sid_audit_process_perform + "' and  a.Audit_Id = '" + sAudit_Id + "' and a.Audit_Id = b.Audit_Id and a.active = 1";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            deptId = (dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionId = (dsModels.Tables[0].Rows[0]["division"].ToString()),
                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklistId = (dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            PlannedById = (dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_teamId = (dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            dept_head = (dsModels.Tables[0].Rows[0]["dept_head"].ToString()),
                            id_audit_process_perform = dsModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString(),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),
                            auditee_teamId = dsModels.Tables[0].Rows[0]["auditee_team"].ToString(),
                            Details = dsModels.Tables[0].Rows[0]["Details"].ToString(),
                            Evidence = dsModels.Tables[0].Rows[0]["Evidence"].ToString(),
                            Category = dsModels.Tables[0].Rows[0]["Category"].ToString(),
                            Conformance = dsModels.Tables[0].Rows[0]["Conformance"].ToString(),
                            Non_comformance = dsModels.Tables[0].Rows[0]["Non_comformance"].ToString(),
                            evidence_upload = dsModels.Tables[0].Rows[0]["evidence_upload"].ToString(),

                            why_nc = dsModels.Tables[0].Rows[0]["why_nc"].ToString(),
                            auditee_acceptance = dsModels.Tables[0].Rows[0]["auditee_acceptance"].ToString(),

                            nc_step_status = dsModels.Tables[0].Rows[0]["nc_step_status"].ToString(),
                            nc_status = dsModels.Tables[0].Rows[0]["nc_status"].ToString(),
                            nc_reject_reason = dsModels.Tables[0].Rows[0]["nc_reject_reason"].ToString(),
                            nc_reject_upload = dsModels.Tables[0].Rows[0]["nc_reject_upload"].ToString(),
                            nc_reject_response = dsModels.Tables[0].Rows[0]["nc_reject_response"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["ca_date"].ToString() != ""
                        && DateTime.TryParse(dsModels.Tables[0].Rows[0]["ca_date"].ToString(), out dtDocDate))
                        {
                            objModel.ca_date = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["followup_date"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
                        }
                        //AuditElementsModels obj = new AuditElementsModels();                        
                        //ViewBag.Question = obj.GetQuestionsListbox(dsModels.Tables[0].Rows[0]["checklist"].ToString());

                        //IDictionary<string, string> findingCategory = new Dictionary<string, string>();
                        //findingCategory.Add("Conformance", "Conformance"); //adding a key/value using the Add() method
                        //findingCategory.Add("Major Non Conformance", "Major Non Conformance");
                        //findingCategory.Add("Minor Non Conformance", "Minor Non Conformance");
                        //findingCategory.Add("Potential Non Conformance", "Potential Non Conformance");
                        //findingCategory.Add("Note-worthy finding", "Note-worthy finding");
                        //ViewBag.FindingCategory = findingCategory;


                        //string sSqlstmt3 = "select * from t_audit_process_perform where Audit_Id='" + sAudit_Id + "'";
                        //DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt3);                       
                        //ViewBag.objAudit = dsAudit;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objModel);
        }

        [HttpPost]
        public ActionResult AuditNonConfirmity(FormCollection form, AuditProcessModels objModel)
        {
            try
            {

                objModel.why_nc = form["why_nc"];

                if (objModel.FunUpdateNC(objModel))
                {
                    TempData["Successdata"] = "Non Confirmity updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmity: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditDetails", new { Audit_Id = objModel.Audit_Id });
        }

        [HttpPost]
        public ActionResult AuditNonConfirmity1(FormCollection form, AuditProcessModels objModel, IEnumerable<HttpPostedFileBase> nc_reject_upload)
        {
            try
            {

                objModel.auditee_acceptance = form["auditee_acceptance"];

                DateTime dateValue;
                if (DateTime.TryParse(form["ca_date"], out dateValue) == true)
                {
                    objModel.ca_date = dateValue;
                }
                HttpPostedFileBase files = Request.Files[0];
                if (nc_reject_upload != null && files.ContentLength > 0)
                {
                    objModel.nc_reject_upload = "";
                    foreach (var file in nc_reject_upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Audit"), Path.GetFileName(file.FileName));
                            string sFilename = "AuditNC" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objModel.nc_reject_upload = objModel.nc_reject_upload + "," + "~/DataUpload/MgmtDocs/Audit/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmity1-upload: " + ex.ToString());

                        }
                    }
                    objModel.nc_reject_upload = objModel.nc_reject_upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objModel.FunUpdateNC1(objModel))
                {
                    TempData["Successdata"] = "Non Confirmity updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmity1: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditDetails", new { Audit_Id = objModel.Audit_Id });
        }

        [HttpPost]
        public ActionResult AuditNonConfirmity2(FormCollection form, AuditProcessModels objModel)
        {
            try
            {

                DateTime dateValue;
                if (DateTime.TryParse(form["followup_date"], out dateValue) == true)
                {
                    objModel.followup_date = dateValue;
                }

                if (objModel.FunUpdateNC2(objModel))
                {
                    TempData["Successdata"] = "Non Confirmity updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmity2: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditDetails", new { Audit_Id = objModel.Audit_Id });
        }

        public ActionResult AuditNonConfirmityPDF(FormCollection form)
        {
            AuditProcessModels objModel = new AuditProcessModels();
            try
            {
                if (form["Audit_Id"] != null && form["id_audit_process_perform"] != null && form["Audit_Id"] != "" && form["id_audit_process_perform"] != "")
                {
                    string sAudit_Id = form["Audit_Id"];
                    string sid_audit_process_perform = form["id_audit_process_perform"];
                    string sSqlstmt = "select a.Audit_Id, Audit_criteria, PlannedBy, checklist, Audit_no," +
                        " AuditPlanDate, AuditDate, dept,  location, division,internal_audit_team, " +
                        "Reasons_Reschedule, Notified_To,Audit_Status, fromtime,  totime,Audit_Elements,auditee_team," +
                        "id_audit_process_perform,Details,Evidence,Category,Conformance,Non_comformance,evidence_upload," +
                        "why_nc,auditee_acceptance,ca_date,followup_date from t_audit_process a," +
                        " t_audit_process_perform b where id_audit_process_perform='" + sid_audit_process_perform + "' and  a.Audit_Id = '" + sAudit_Id + "' and a.Audit_Id = b.Audit_Id and a.active = 1";

                    DataSet dsModels = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsModels.Tables.Count > 0 && dsModels.Tables[0].Rows.Count > 0)
                    {
                        objModel = new AuditProcessModels
                        {
                            Audit_Id = dsModels.Tables[0].Rows[0]["Audit_Id"].ToString(),
                            Audit_criteria = objGlobaldata.GetIsoStdDescriptionById(dsModels.Tables[0].Rows[0]["audit_criteria"].ToString()),
                            Audit_no = dsModels.Tables[0].Rows[0]["Audit_no"].ToString(),
                            location = objGlobaldata.GetDivisionLocationById(dsModels.Tables[0].Rows[0]["location"].ToString()),
                            dept = objGlobaldata.GetMultiDeptNameById(dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            deptId = (dsModels.Tables[0].Rows[0]["dept"].ToString()),
                            division = objGlobaldata.GetMultiCompanyBranchNameById(dsModels.Tables[0].Rows[0]["division"].ToString()),
                            divisionId = (dsModels.Tables[0].Rows[0]["division"].ToString()),
                            Notified_To = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["Notified_To"].ToString()),
                            checklistId = (dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            checklist = objGlobaldata.GetChecklistBychecklistId(dsModels.Tables[0].Rows[0]["checklist"].ToString()),
                            FromPlanTimeInHour = dsModels.Tables[0].Rows[0]["fromtime"].ToString(),
                            ToPlanTimeInHour = dsModels.Tables[0].Rows[0]["totime"].ToString(),
                            PlannedBy = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            PlannedById = (dsModels.Tables[0].Rows[0]["PlannedBy"].ToString()),
                            internal_audit_teamId = (dsModels.Tables[0].Rows[0]["internal_audit_team"].ToString()),
                            auditee_team = objGlobaldata.GetMultiHrEmpNameById(dsModels.Tables[0].Rows[0]["auditee_team"].ToString()),

                            id_audit_process_perform = dsModels.Tables[0].Rows[0]["id_audit_process_perform"].ToString(),
                            Details = dsModels.Tables[0].Rows[0]["Details"].ToString(),
                            Evidence = dsModels.Tables[0].Rows[0]["Evidence"].ToString(),
                            Category = dsModels.Tables[0].Rows[0]["Category"].ToString(),
                            Conformance = dsModels.Tables[0].Rows[0]["Conformance"].ToString(),
                            Non_comformance = dsModels.Tables[0].Rows[0]["Non_comformance"].ToString(),
                            evidence_upload = dsModels.Tables[0].Rows[0]["evidence_upload"].ToString(),

                            why_nc = dsModels.Tables[0].Rows[0]["why_nc"].ToString(),
                            auditee_acceptance = dsModels.Tables[0].Rows[0]["auditee_acceptance"].ToString(),
                        };
                        DateTime dtDocDate;
                        if (dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditPlanDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditPlanDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["AuditDate"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["AuditDate"].ToString(), out dtDocDate))
                        {
                            objModel.AuditDate = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["ca_date"].ToString() != ""
                        && DateTime.TryParse(dsModels.Tables[0].Rows[0]["ca_date"].ToString(), out dtDocDate))
                        {
                            objModel.ca_date = dtDocDate;
                        }

                        if (dsModels.Tables[0].Rows[0]["followup_date"].ToString() != ""
                         && DateTime.TryParse(dsModels.Tables[0].Rows[0]["followup_date"].ToString(), out dtDocDate))
                        {
                            objModel.followup_date = dtDocDate;
                        }

                        CompanyModels objCompany = new CompanyModels();
                        dsModels = objCompany.GetCompanyDetailsForReport(dsModels);
                        dsModels = objGlobaldata.GetReportDetails(dsModels, objModel.Audit_no, objGlobaldata.GetCurrentUserSession().empid, "AUDIT PROCESS REPORT");

                        ViewBag.CompanyInfo = dsModels;

                        ViewBag.NCDetails = objModel;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditNonConfirmityPDF: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("AuditNonConfirmityPDF")
            {
                //FileName = "AuditNC.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        public ActionResult FunGetAuditeesList(string branch, string dept_name)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {
                lstEmp = objGlobaldata.GetAuditeesList(branch, dept_name);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetAuditeesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(lstEmp);
        }
        public ActionResult FunGetAuditorsList(string branch, string shedule_date)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {

                lstEmp = objGlobaldata.GetAuditorsList(branch, shedule_date);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetAuditeesList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(lstEmp);
        }
        public ActionResult FunGetChecklist(string branch)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {

                lstEmp = objGlobaldata.GetChecklistTypeByChecklistRef(branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json(lstEmp);
        }
        //to get the product or process codes
        public ActionResult FunGetAuditProductList(string audit_type)
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            MultiSelectList lstEmp = new MultiSelectList(emplist.EmpList, "Value", "Text");
            try
            {
                lstEmp = objGlobaldata.FunGetAuditProductList(audit_type);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetAuditProductList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(lstEmp);
        }

    }
}