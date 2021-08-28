using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using ISOStd.Filters;
using System.Xml;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.IO;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class DashboardController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public DashboardController()
        {
            ViewBag.Menutype = "Dashboard";
        }
        
        // Dashboard Controller Comments comment

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ListPendingForApproval()
        {
            try
            {
                int count = 0;
                string sempid = "";

                sempid = objGlobaldata.GetCurrentUserSession().empid;
                

                //==========================Supplier Reevalution======================================
                string sSqlstmt = "select id_reevaluation, branch, supplier, logged_by, perf_review_year, recommanded,recommanded_to,isrecommand, recommand_date, approved_to,isapproved,approved_date from t_supplier_reevaluation"
                    + " where isapproved=0 and isrecommand =1 and Active=1 and approved_to ='" + sempid + "'";

                DataSet dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                               
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsSuppReevaluation = dsApprovalList;
                }

                //==========================Safety Violation Log======================================

                 sSqlstmt = "select * from t_safety_violationlog where Active= 1 and Approved_Status=0 and " +
                    "( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))" +
                    " and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector)) " +
                    "order by ViolationLog_Id, Reported_On desc ";

                 dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsSViolation = dsApprovalList;
                }

                //==========================SYSTEM DOCUMENTS======================================
                //string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=1 and" +
                //             " find_in_set('" + sempid + "',ApprovedBy) "
                //            + "and find_in_set('" + sempid + "',ApprovedBy)";
                //sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";
                 sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=1 and" +
                              " ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers)) "
                             + "and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector))";
                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";

                 dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsMgmtDocument = dsApprovalList;
                }

                //------------------------------------NC--------------------------------------------------------------
                sSqlstmt = "select id_nc,nc_no,nc_reported_date,nc_detected_date,nc_category,nc_reportedby,nc_issueto,nc_notifiedto," +
                    "division,department,location  from t_nc where active= 1 and nc_issuedto_status=0 and" +
                           " ( find_in_set('" + sempid + "',nc_issueto) and not find_in_set('" + sempid + "',nc_issuers)) "
                          + "and ( find_in_set('" + sempid + "',nc_issueto) and not find_in_set('" + sempid + "',nc_issuer_rejector))";
                sSqlstmt = sSqlstmt + " order by id_nc desc";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsNC = dsApprovalList;
                }

                //==========================ANNEXURE======================================
                sSqlstmt = "select idAnnexure,MgmtId,DocRef,DocName,IssueNo,RevNo,PreparedBy,ApprovedBy,DocDate,DocUploadPath"
                    + " from t_mgmt_annexure where Status=1 and ApprovedStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy))";

                sSqlstmt = sSqlstmt + " order by idAnnexure desc";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAnnexure = dsApprovalList;
                }

                //==========================TRAINING======================================
                dsApprovalList = new DataSet();

                //sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Training_Start_Date, Expected_Date_Completion,Training_Requested_By, Reasonfor_Training, RequestStatus, "
                //        + " ApprovedBy from t_trainings where Active=1 and RequestStatus='0' and ApprovedBy='" + sempid + "' order by TrainingID desc";

                sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Training_Start_Date, Expected_Date_Completion,Training_Requested_By, Reasonfor_Training, RequestStatus, "
                           + " ApprovedBy from t_trainings where Active=1 and RequestStatus='0' and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))"
                           + "and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',ApprovalRejector)) order by TrainingID desc";
                
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsTraining = dsApprovalList;
                }

                //==========================OBJECTIVES======================================
                //sSqlstmt = "select Objectives_Id, Obj_Ref, Dept, Freq_of_Eval, Personal_Responsible, Audit_Criteria, Estld_by, Approved_By,ApprovedDate, CreatedBy,DocUploadPath from t_objectives"
                //    + " where approved_status=0 and Active=1 and Approved_By ='" + sempid + "'";
                sSqlstmt = "select ObjectivesTrans_Id,Obj_Ref, Dept, b.Freq_of_Eval, b.Approved_By, Audit_Criteria, Estld_by, " +
                    "b.Approved_By,b.ApprovedDate, CreatedBy,DocUploadPath from t_objectives a, t_objectives_trans b" +
                    " where b.approved_status = 0 and Active = 1 and trans_active=1 and a.Objectives_Id = b.Objectives_Id  and b.Approved_By = '" + sempid + "'";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                                             
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsObjectives = dsApprovalList;
                }
                //==========================LEGAL  REGISTER======================================

                sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                    + "  reviewedBy,approvedBy,updatedByName from t_legalregister"
                    + " where approvestatus=0 and approvedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dslegalregister = dsApprovalList;
                }


                //==========================DOCUMENT CHANGE REQUEST======================================

                sSqlstmt = "select * from t_documentchangerequest where ApproveStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))"
                 + " and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Rejector))";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsDocChangeRequest = dsApprovalList;
                }



                //==========================CHANGE MANAGEMENT REQUEST======================================

                sSqlstmt = "select * from t_changemanagement where ApproveStatus=0 and ( find_in_set('" + sempid + "',ApprovedBy) and not find_in_set('" + sempid + "',Approvers))";


                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsChangeManagement = dsApprovalList;
                }

                //==========================PROJECT MANAGEMENT======================================
                sSqlstmt = "select id_projectdesign,t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,DrawingNumber,Upload,ReviewedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt "
                   + " and ApproveStatus=0 and t.Active=1 and tt.Active=1 and ReviewStatus=1 and  ApprovedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsProject = dsApprovalList;
                }

                //==========================BIDDING======================================

                sSqlstmt = "select id_bidding,client,upload,folderref,projectname,submission_date,preparedby,proposalref,"
                + "proposal_date from t_bidding where Active=1 and proposal_status=0 and"
                + " ( find_in_set('" + sempid + "',checkedby) AND"
                + " not find_in_set('" + sempid + "',Approvers))";
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsBidding = dsApprovalList;
                }

                //==========================LEAVE======================================
                sSqlstmt = "select leave_id,emp_no,fr_date,to_date,leave_type,duration,approver,applied_date,reason_leave from t_leave_trans where"
                  + " approved_status=0 and Active=1 and approver ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLeave = dsApprovalList;
                }
                //==========================LEAVE2======================================
                sSqlstmt = "select leave_id,t.emp_no,fr_date,to_date,leave_type,duration,approver,applied_date,reason_leave,bal_annual_leave,bal_sick_leave,bal_other_leave"
                 + " from t_leavetrans t,t_leavemaster tt where t.emp_no=tt.emp_no and  t.syear=tt.syear and approved_status=0 and t.Active=1 and tt.Active=1 and approver ='" + sempid + "'";


                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLeaveList = dsApprovalList;
                }

                //==========================SUPPLIER======================================
                sSqlstmt = "select SupplierID,SupplierCode,SupplierName,City,ContactPerson,ContactNo,Address,SupplyScope,ApprovalCriteria from t_supplier where"
                  + " Active=1 and ApprovedStatus=0 and ApprovedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsSupplier = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================

                sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                          + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                          + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                          + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                          + " and Approve_status=0 and Review_status=1 and tract.Active=1 and trEval.Active=1 and ApprovedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskActivity = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(HRR)======================================

                sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Approve_status=0 and Review_status=1 and ApprovedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskHRRActivity = dsApprovalList;
                }

                //==========================AUDIT======================================

                sSqlstmt = "select AuditID,AuditNum,AuditDate,AuditCriteria,AuditLocation,upload,Audit_Prepared_by,ApprovedBy from t_internal_audit where Active=1 and ApprvStatus=0 and ApprovedBy='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAudit = dsApprovalList;
                }

                //==========================AUDIT PROCESS(Internal Audit Plan) DEPT HEAD======================================

                sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria,checklist,dept,location,division,PlannedBy,Notified_To from t_audit_process where active=1 and" +
                 " ((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',dept_head)) and dept_head_status=0)";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAuditProcessdepthead = dsApprovalList;
                }

                //==========================AUDIT PROCESS(Internal Audit Plan)======================================

                sSqlstmt = "select Audit_Id,Audit_no,AuditDate,Audit_criteria,checklist,dept,location,division,PlannedBy,Notified_To from t_audit_process where active=1 and" +
                                  " (((Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',internal_audit_team)) and audit_team_status=0 ) or" +
                            " (dept_head_status=1 and audit_team_status=1 and (Audit_Status='Pending' or Audit_Status='Rescheduled') and ( find_in_set('" + sempid + "',Notified_To))))";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsAuditProcess = dsApprovalList;
                }


                //==========================OFI Approval======================================

                sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby,division,department,location from t_ofi where active=1 and " +
                "((approved_status = '0' and find_in_set('" + sempid + "',approvedby)) " +
                "or approved_status = 'Approved' and realization_approved_status = '0' and find_in_set('" + sempid + "',realization_approved_by))";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsOFI = dsApprovalList;
                }


                //==========================OFI Checkedby======================================

                sSqlstmt = "select id_ofi,ofi_no,risk_no,reported_date,approved_status,realization_approved_status,ofi_status,reportedby,division,department,location from t_ofi where active=1 and " +
                "approved_status = 'Approved' and realization_approved_status = 'Approved'  and checkedbystatus = '0' and find_in_set('" + sempid + "',checkedby)";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsOFIChekedby = dsApprovalList; ;
                }

                //==========================WORK PERMIT======================================

                sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,description,date_issued,date_expired,approved_by"
               + " from t_access_permit where Active=1 and approve_status=0 and approved_by ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsWorkPermit = dsApprovalList;
                }

                //==========================JD======================================

                sSqlstmt = "select id_role_jd,jd_report,role_id,deptid,report_to,supervises,jd_date,logged_by"
                        + " from t_role_jd where approve_status=0 and approved_by ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsJD = dsApprovalList;
                }

                //==========================Document Create Request======================================

                 sSqlstmt = "select id_doc_request,dcr_no,date_request,division,`department`,reason,upload,checkedby,doc_status as doc_statusId,logged_by," +
                      "case when doc_status = '0' then 'Pending for IMS Rep.' when doc_status = '1' then 'Pending for document controller' when doc_status = '2' then 'Approved' when doc_status = '3' then 'Rejected' end  as doc_status " +
                       " from t_document_create_request where Active=1 and (doc_status = 0 and find_in_set('" + sempid + "',checkedby)) or (doc_status = 1 and find_in_set('" + sempid + "',doc_control))";


                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsDCRequest = dsApprovalList;
                }

                //==========================Potal Authorization======================================

                sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                 "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status" +
                 " from t_portal_authorization where Active=1"
                 + " and((apporved_status = '0' and find_in_set('" + sempid + "', recommended_by))"
                 + " or(apporved_status = '1' and find_in_set('" + sempid + "', approve_ceo))"
                 + " or(apporved_status = '2' and find_in_set('" + sempid + "', approve_vp))"
                 + " or(apporved_status = '3' and find_in_set('" + sempid + "', approve_chairman)))";
             
                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                count = count + dsApprovalList.Tables[0].Rows.Count;
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsPortalAuthorization = dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ListPendingForApproval: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        public ActionResult ListPendingForReview()
        {
            try
            {
                string sempid = "";

                sempid = objGlobaldata.GetCurrentUserSession().empid;

                //==========================SYSTEM DOCUMENTS======================================

                //string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0 and" +
                //          " find_in_set('" + sempid + "',ReviewedBy) "
                //         + "and find_in_set('" + sempid + "',ReviewedBy)";
                //sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";
               
                
                //New Code Here appoval and reject both are included

                string sSqlstmt = "select * from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0 and" +
                                  " ( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',Reviewers)) "
                                 + "and ( find_in_set('" + sempid + "',ReviewedBy) and not find_in_set('" + sempid + "',ReviewRejector))";
                sSqlstmt = sSqlstmt + " order by DocLevels, idmgmt desc";                


                DataSet dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsMgmtDocument = dsApprovalList;
                }

                //string sSqlstmt1 = "";
                //if (objGlobaldata.GetMultiRolesNameById(objGlobaldata.GetCurrentUserSession().role).Contains("Admin"))
                //{
                //    sSqlstmt1 = "select idMgmt, DocLevels, Doctype,DocUploadPath, ISOStds, AppClauses, DocRef, DocName, IssueNo, RevNo, PreparedBy, ReviewedBy, DocDate,"
                //    + " DocUploadPath, ApprovedBy, Approved_Status, ApprovedDate, UploadedBy "
                //    + " from t_mgmt_documents where Status= 1 and Approved_Status=0 and Reviewed_Status=0  order by DocLevels, idmgmt desc";

                //    DataSet dsApprovalList1 = objGlobaldata.Getdetails(sSqlstmt1);
                //    if (dsApprovalList1.Tables.Count > 0 && dsApprovalList1.Tables[0].Rows.Count > 0)
                //    {
                //        ViewBag.dsMgmtDocument = dsApprovalList1;
                //    }
                //}
                //==========================ANNEXURE======================================
                //sSqlstmt = "select idAnnexure,MgmtId,DocRef,DocName,IssueNo,RevNo,PreparedBy,ReviewedBy,DocDate,DocUploadPath"
                //    + " from t_mgmt_annexure where Status=1 and ReviewStatus=0 and ( find_in_set('" + sempid + "',ReviewedBy))";

                //sSqlstmt = sSqlstmt + " order by idAnnexure desc";
                //dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                //if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                //{
                //    ViewBag.dsAnnexure = dsApprovalList;
                //}

                //==========================Supplier Reevalution======================================
                 sSqlstmt = "select id_reevaluation, branch, supplier, logged_by, perf_review_year, recommanded,isrecommand, recommand_date,recommanded_to,approved_to,isapproved,approved,approved_date from t_supplier_reevaluation"
                    + " where isrecommand =0 and Active=1 and recommanded_to ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsSuppReevaluation= dsApprovalList;
                }
                //==========================LEGAL REGISTER======================================

                sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle, initialdevelopmentdate, origin_of_requirement , document_storage_location,frequency_of_evaluation, activeStatus, updatedOn,"
                    + "  reviewedBy,approvedBy,updatedByName from t_legalregister where approvestatus=0 and reviewstatus=0 and reviewedBy='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsLegalRegister = dsApprovalList;
                }
                //==========================PROJECT MANAGEMENT======================================
                sSqlstmt = "select id_projectdesign,t.id_projectmgmt,ProjectNo,ProjectName,Dicipline,DrawingNumber,Upload,ApprovedBy from t_projectmgmt t,t_projectdesign tt where t.id_projectmgmt=tt.id_projectmgmt "
                    + " and ReviewStatus=0 and t.Active=1 and tt.Active=1 and ReviewedBy ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsProject = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(ENVIRONMENT)======================================

                sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity, "
                          + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                          + " DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, Comments,Exposure_id,Appl_law from t_risk_register_activity_eval as trEval, "
                          + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id"
                          + " and Review_status=0 and tract.Active=1 and trEval.Active=1 and Reviewer_QHSE ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskActivity = dsApprovalList;
                }

                //==========================RISK REGISTER ACTIVITY(HRR)======================================

                sSqlstmt = "select risk_hrr_id,Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,"
               + "Cur_Opt_Ctrl,Person_resp,Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_hrrevaluation"
               + " where Active=1 and Review_status=0 and Reviewer_QHSE ='" + sempid + "'";

                dsApprovalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsApprovalList.Tables.Count > 0 && dsApprovalList.Tables[0].Rows.Count > 0)
                {
                    ViewBag.dsRiskHRRActivity = dsApprovalList;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ListPendingForReview: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        public ActionResult Dashboard()
        {
            ViewBag.SubMenutype = "Dashboard";

            return View();
        }

        public ActionResult DashboardSetting()
        {
            ViewBag.SubMenutype = "DashboardSetting";
            return View();
        }

        [HttpPost]
        public ActionResult UploadChartData(HttpPostedFileBase fileUploader)
        {
            ViewBag.SubMenutype = "Dashboard";
            try
            {
                DataSet ds = new DataSet();
                if (Request.Files["fileUploader"].ContentLength > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(Request.Files["fileUploader"].FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //new changes
                        string fileLocation = Path.Combine(Server.MapPath("~/DataUpload/ChartUploadedFiles"), Path.GetFileName(Guid.NewGuid().ToString("N")) + fileExtension);
                        string sFilepath = Path.GetDirectoryName(fileLocation);
                        string sFilename = Path.GetFileName(fileLocation);


                        //string fileLocation = Server.MapPath("~/DataUpload/ImportExcelOLEDB/") + Request.Files["file"].FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {

                            System.IO.File.Delete(fileLocation);
                        }
                        //if (System.IO.File.Exists(fileLocation))
                        //{
                        //    System.IO.File.Delete(fileLocation);
                        //}
                        fileUploader.SaveAs(sFilepath + "/" + sFilename);//new changes
                        //Request.Files["fileUploader"].SaveAs(sFilepath + "/1" + sFilename);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {

                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }
                        excelConnection.Close();

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        excelConnection1.Close();
                    }

                    string SqlStmt = "select IfNull(max(BatchId), 0) BatchId FROM tbl_materialtestchart;";
                    DataSet dsEmp = objGlobaldata.Getdetails(SqlStmt);
                    int batchId = 0;
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        batchId = int.Parse(dsEmp.Tables[0].Rows[0]["BatchId"].ToString());
                    }
                    batchId++;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string SqStmt = "Insert into tbl_materialtestchart(BatchId,MaterialType,SubType,Source,TestDate, Status) Values(" + batchId + ",'" + ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() + "','" + ds.Tables[0].Rows[i][2].ToString() + "','" + Convert.ToDateTime(ds.Tables[0].Rows[i][3].ToString()).ToString("yyyy-MM-dd HH':'mm':'ss") + "','" + ds.Tables[0].Rows[i][4].ToString() + "');";
                        if (objGlobaldata.ExecuteQuery(SqStmt))
                        {
                            TempData["Successdata"] = "successfully uploaded";
                        }
                        else
                        {
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ImportToExcel: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }



            return View("DashboardSetting");
        }

        public JsonResult GetMaterialTestReport(DateTime? fromDate, DateTime? toDate)
        {
            DataSet dsReport = new DataSet();
            try
            {
                var sSqlstmt = "SELECT MaterialType, Sum(Case when Status = 'Planned' then 1 else 0 end) Planned, Sum(Case when Status = 'Performed' then 1 else 0 end) Performed, Sum(Case when Status = 'Passed' then 1 else 0 end) Passed  FROM tbl_materialtestchart where " + (fromDate.HasValue ? " TestDate >= '" + fromDate.Value.ToString("yyyy-MM-dd") + "'" : "1=1") + " and " + (toDate.HasValue ? " TestDate <= '" + toDate.Value.ToString("yyyy-MM-dd") + "'" : "1=1") + " group by MaterialType;";
                string sSearchtext = string.Empty;
                //sSearchtext = sSearchtext + " where role_id ='" + sRoleID + "' and branch_id ='" + sbranch_id + "'";

                sSqlstmt = sSqlstmt + sSearchtext;

                dsReport = objGlobaldata.Getdetails(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetSummaryReport: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            var json = JsonConvert.SerializeObject(dsReport, Newtonsoft.Json.Formatting.Indented);
            return Json(json, JsonRequestBehavior.AllowGet);
        }



    }
}
