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
using Newtonsoft.Json;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
   
    public class HomeController : Controller
    {
      
        clsGlobal objGlobaldata = new clsGlobal(); 

        public ActionResult Index()
        {
            if (HttpContext.Session["UserCredentials"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
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
        //dashboard report


        public ActionResult QHSE()
        {
           
            HomeModels objModels = new HomeModels();
            HomeModelsList lst = new HomeModelsList();
            lst.HomeModelList = new List<HomeModels>();
            try
            {           
            

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QHSE: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

            }
            return View();

        }
        public ActionResult QMS(int? page)
        {
            HomeModels objModels = new HomeModels();
            HomeModelsList lst = new HomeModelsList();
            lst.HomeModelList = new List<HomeModels>();
            try
            {
                DataSet dsObjModelsList = objModels.FunGetDashboardReport("generate_dashboard_report");

                if (dsObjModelsList.Tables.Count > 0 && dsObjModelsList.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsObjModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            HomeModels obj = new HomeModels()
                            {
                                items = dsObjModelsList.Tables[0].Rows[i]["items"].ToString(),
                                pending_approval = dsObjModelsList.Tables[0].Rows[i]["pending_approval"].ToString(),
                                pending_review = dsObjModelsList.Tables[0].Rows[i]["pending_review"].ToString(),
                                approved = dsObjModelsList.Tables[0].Rows[i]["approved"].ToString(),
                                rejected = dsObjModelsList.Tables[0].Rows[i]["rejected"].ToString(),
                                Not_completed = dsObjModelsList.Tables[0].Rows[i]["Not_completed"].ToString(),
                                Completed = dsObjModelsList.Tables[0].Rows[i]["Completed"].ToString(),
                                Rescheduled = dsObjModelsList.Tables[0].Rows[i]["Rescheduled"].ToString(),
                                Cancelled = dsObjModelsList.Tables[0].Rows[i]["Cancelled"].ToString(),
                                Open = dsObjModelsList.Tables[0].Rows[i]["Open"].ToString(),
                                Closed = dsObjModelsList.Tables[0].Rows[i]["closed"].ToString(),
                                Pending = dsObjModelsList.Tables[0].Rows[i]["pending"].ToString(),
                                total = dsObjModelsList.Tables[0].Rows[i]["total"].ToString(),
                            };

                            lst.HomeModelList.Add(obj);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QMS: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];

            }
            return View(lst.HomeModelList.ToList().ToPagedList(page ?? 1, 1000));

        }
        public JsonResult FunGetNearMissDashboard()
        {
            var data = objGlobaldata.GetNearMissDashboardData();
            return Json(data);
        }
        public JsonResult FunGetMockDrillDashboard()
        {
            var data = objGlobaldata.GetMockDrillDashboardData();
            return Json(data);
        }
        public JsonResult FunGetToolBoxDashboard()
        {
            var data = objGlobaldata.GetToolboxDashboardData();
            return Json(data);
        }

        public JsonResult FunGetTotalPendingForApproval()
        {
            int apprcount = 0;
            int revcount = 0;
            var result = new { apprv = apprcount, review = revcount };
            try
            {
                var sempid = objGlobaldata.GetCurrentUserSession().empid;
                //==========================SYSTME DOCUMENTS======================================
             
                string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=1 and" +
                              " ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers)) "
                             + "and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector))";
                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

                DataSet dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsMgmtDocument = dsApprovalList;
                }


                //==========================SYSTME DOCUMENTS-review======================================
                string sSqlstmt1 = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0 and " +
                    "( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',Reviewers))"
                     + " and ( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',ReviewRejector))";
                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt1);
                revcount = revcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsMgmtDocument = dsApprovalList;
                }

                //==========================TRAINING======================================
                dsApprovalList = new DataSet();

                sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training, Training_Status, RequestStatus, "
                        + " ApprovedBy from t_trainings where Active=1 and RequestStatus='0' and ApprovedBy='" + objGlobaldata.GetCurrentUserSession().empid + "' order by TrainingID desc";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsTraining = dsApprovalList;
                }

                //==========================OBJECTIVES======================================
                //sSqlstmt = "select Objectives_Id, Obj_Ref, Dept, Freq_of_Eval, Personal_Responsible, Audit_Criteria, Estld_by, Approved_By,ApprovedDate, CreatedBy from t_objectives"
                //    + " where approved_status=0 and Active=1 and Approved_By ='" + objGlobaldata.GetCurrentUserSession().empid + "'";
                sSqlstmt = "select ObjectivesTrans_Id,Obj_Ref, Dept, b.Freq_of_Eval, b.Personal_Responsible, Audit_Criteria, Estld_by, " +
                    "b.Approved_By,b.ApprovedDate, CreatedBy,DocUploadPath from t_objectives a, t_objectives_trans b" +
                    " where b.approved_status = 0 and Active = 1 and a.Objectives_Id = b.Objectives_Id  and b.Approved_By = '" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsObjectives = dsApprovalList;
                }
                //==========================LEGAL  REGISTER======================================

                sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                    + "  reviewedBy,approvedBy,updatedByName from t_legalregister"
                    + " where approvestatus=0 and approvedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dslegalregister = dsApprovalList;
                }


                //==========================DOCUMENT CHANGE REQUEST======================================

                sSqlstmt = "select * from t_documentchangerequest where ApproveStatus=0 and ( find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',ApprovedBy) and not find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',Approvers))"
                 + "and ( find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',ApprovedBy) and not find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',Rejector))";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsDocChangeRequest = dsApprovalList;
                }



                //==========================CHANGE MANAGEMENT REQUEST======================================

                sSqlstmt = "select * from t_changemanagement where ApproveStatus=0 and ( find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',ApprovedBy) and not find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',Approvers))";


                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsChangeManagement = dsApprovalList;
                }

                //==========================PROJECT MANAGEMENT======================================
                sSqlstmt = "select id_projectdesign,t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,DrawingNumber,Upload,ReviewedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt "
                   + " and ApproveStatus=0 and t.Active=1 and tt.Active=1 and ReviewStatus=1 and  ApprovedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsProject = dsApprovalList;
                }

                //==========================BIDDING======================================

                sSqlstmt = "select id_bidding,client,upload,folderref,projectname,submission_date,preparedby,proposalref,"
                + "proposal_date from t_bidding where Active=1 and proposal_status=0 and"
                + " ( find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',checkedby) AND"
                + " not find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',Approvers))";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsBidding = dsApprovalList;
                }

                //==========================LEAVE======================================
                sSqlstmt = "select leave_id,emp_no,fr_date,to_date,leave_type,duration,approver,applied_date,reason_leave from t_leave_trans where"
                 + " approved_status=0 and Active=1 and approver ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLeave = dsApprovalList;
                }

                //==========================LEAVE2======================================
                sSqlstmt = "select leave_id,t.emp_no,fr_date,to_date,leave_type,duration,approver,applied_date,reason_leave,bal_annual_leave,bal_sick_leave,bal_other_leave"
                 + " from t_leavetrans t,t_leavemaster tt where t.emp_no=tt.emp_no and  t.syear=tt.syear and approved_status=0 and t.Active=1 and tt.Active=1 and approver ='" + objGlobaldata.GetCurrentUserSession().empid + "'";


                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLeaveList = dsApprovalList;
                }


                //==========================SUPPLIER======================================
                sSqlstmt = "select SupplierID,SupplierCode,SupplierName,City,ContactPerson,ContactNo,Address,SupplyScope,ApprovalCriteria from t_supplier where"
                  + " Active=1 and ApprovedStatus=0 and  ApprovedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsSupplier = dsApprovalList;
                }
                //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================

                sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                          + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                          + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                          + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                          + " and Approve_status=0 and Review_status=1 and tract.Active=1 and trEval.Active=1 and ApprovedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskActivity = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(HRR)======================================

                sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Approve_status=0 and Review_status=1 and ApprovedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskHRRActivity = dsApprovalList;
                }                              

                
                //==========================LEGAL  REGISTER======================================

                sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                    + "  reviewedBy,approvedBy,updatedByName from t_legalregister where approvestatus=0 and reviewstatus=0 and reviewedBy='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLegalRegister = dsApprovalList;
                }
                //==========================PROJECT MANAGEMENT======================================
                sSqlstmt = "select id_projectdesign,t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,DrawingNumber,Upload,ApprovedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt "
                    + " and ReviewStatus=0 and t.Active=1 and tt.Active=1 and ReviewedBy ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                revcount = revcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsProject = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================

                sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                          + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                          + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                          + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                          + " and Review_status=0 and tract.Active=1 and trEval.Active=1 and Reviewer_QHSE ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                revcount = revcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskActivity = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(HRR)======================================

                sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Review_status=0 and Reviewer_QHSE ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                revcount = revcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskHRRActivity = dsApprovalList;
                }

                //==========================ANNEXURE======================================               
                sSqlstmt = "select idAnnexure,MgmtId,DocRef,DocName,IssueNo,RevNo,PreparedBy,ApprovedBy,DocDate,DocUploadPath"
                   + " from t_mgmt_annexure where Status=1 and ApprovedStatus=0 and ( find_in_set('" + objGlobaldata.GetCurrentUserSession().empid + "',ApprovedBy))";

                sSqlstmt = sSqlstmt + " order by idAnnexure desc";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAnnexure = dsApprovalList;
                }

                //==========================AUDIT======================================

                sSqlstmt = "select AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,upload,Audit_Prepared_by,ApprovedBy from t_internal_audit where ApprvStatus=0 and Active=1 and ApprovedBy='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAudit = dsApprovalList;
                }

                //==========================AUDIT PROCESS(Internal Audit Plan) DEPT HEAD======================================

                sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria,checklist,dept,location,division from t_audit_process where active=1 and" +
               " ((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',dept_head)) and dept_head_status=0)";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAuditProcessdepthead = dsApprovalList;
                }

                //==========================AUDIT PROCESS(Internal Audit Plan)======================================

                sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria,checklist,dept,location,division from t_audit_process where active=1 and" +
                                 " (((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',internal_audit_team)) and audit_team_status=0 ) or" +
                           " (dept_head_status=1 and audit_team_status=1 and (Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',Notified_To))))";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAuditProcess = dsApprovalList;
                }

                //==========================OFI Approval======================================

                sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby from t_ofi where active=1 and " +
                "((approved_status = '0' and find_in_set('" + sempid + "',approvedby)) " +
                "or approved_status = 'Approved' and realization_approved_status = '0' and find_in_set('" + sempid + "',realization_approved_by))";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsOFI = dsApprovalList;
                }

                //==========================OFI Checked by======================================

                sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby from t_ofi where active=1 and " +
                "approved_status = 'Approved' and realization_approved_status = 'Approved'  and checkedbystatus = '0' and find_in_set('" + sempid + "',checkedby)";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsOFIChekedby = dsApprovalList;
                }

                //==========================WORK PERMIT======================================

                sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,date_issued,date_expired,approved_by"
               + " from t_access_permit where Active=1 and approve_status=0 and approved_by ='" + objGlobaldata.GetCurrentUserSession().empid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                apprcount = apprcount + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsWorkPermit = dsApprovalList;
                }
                result = new { apprv = apprcount, review = revcount };
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ListPendingForApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json(result);
        }

        public JsonResult GetSummaryReport()
        {

            DataSet dsReport = new DataSet();
            try
            {
                dsReport = objGlobaldata.GetDashboard();

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSummaryReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
          

            var json = JsonConvert.SerializeObject(dsReport, Formatting.Indented);
            return Json(json);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
         
        public JsonResult GetEvents(string branch, string event_status, string event_priority, string event_type, string Person_responsible)
        {
            QHSEPlannerModelsList objPlanList = new QHSEPlannerModelsList();
            objPlanList.lstPlan = new List<QHSEPlannerModels>();
            QHSEPlannerModels commentModels = new QHSEPlannerModels();
            try
            {
                string sSqlstmt = "select id_event,subject,description,start_date,end_date,Logged_by,full_day,Person_responsible,NotificationPeriod,NotificationValue,event_status,event_type,"
                    + "event_priority,notification_to from t_event_planner where active=1";

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
                if (Person_responsible != null && Person_responsible != "")
                {
                    sSearchtext = sSearchtext + " and Person_responsible='" + Person_responsible + "' ";

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
                                sevent_status = objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_status"].ToString()),
                                sevent_type = objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_type"].ToString()),
                                sevent_priority = objGlobaldata.GetDropdownitemById(dsEventList.Tables[0].Rows[i]["event_priority"].ToString()),
                                notification_to = (dsEventList.Tables[0].Rows[i]["notification_to"].ToString()),
                                Logged_Id =(dsEventList.Tables[0].Rows[i]["Logged_by"].ToString()),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsEventList.Tables[0].Rows[i]["start_date"].ToString(), out dtValue))
                            {
                                objModels.start_date = dtValue;
                                objModels.starttime = objModels.start_date.ToShortTimeString();
                            }
                            if (dsEventList.Tables[0].Rows[i]["end_date"].ToString() != null && dsEventList.Tables[0].Rows[i]["end_date"].ToString() != "")
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
                                        Logged_by = objGlobaldata.GetEmpHrNameById(sdsData.Tables[0].Rows[k]["Logged_by"].ToString()),
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
        public JsonResult DeleteEvent(string eventID)
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
        public JsonResult SaveEvent(QHSEPlannerModels e, FormCollection form)
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
                e.notification_to = form["notification_to[]"];
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

        public ActionResult FunGetStatusList()
        {
            HomeModels objModel = new HomeModels();
            MultiSelectList lstDept = objModel.GetTrainingStatusList();
            return Json(lstDept);
        }

        public ActionResult FunGetStatusList1()
        {
            MultiSelectList lstDept = objGlobaldata.GetDropdownList("Calendar Event Status");
            return Json(lstDept);
        }
    }
}
