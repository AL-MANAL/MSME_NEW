using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class AuditLogModels
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        [Display(Name = "Audit Criteria")]
        public string Audit_criteria { get; set; }

        [Display(Name = "Audit Id")]
        public string Audit_Id { get; set; }

        [Display(Name = "Planned By")]
        public string PlannedBy { get; set; }

        public string PlannedById { get; set; }

        [Display(Name = "Check List")]
        public string checklist { get; set; }

        //public string checklistId { get; set; }

        [Display(Name = "Audit No")]
        public string Audit_no { get; set; }

        [Display(Name = "Audit Planned On")]
        public DateTime AuditPlanDate { get; set; }

        [Display(Name = "Audit Date")]
        public DateTime AuditDate { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        public string deptId { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        public string locationId { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        public string divisionId { get; set; }

        [Display(Name = "From Timing")]
        public string FromPlanTimeInHour { get; set; }

        [Display(Name = "To Timing")]
        public string ToPlanTimeInHour { get; set; }

        [Display(Name = "Internal Audit Team")]
        public string internal_audit_team { get; set; }

        public string internal_audit_teamId { get; set; }

        [Display(Name = "Notified To")]
        public string Notified_To { get; set; }

        [Display(Name = "Reasons for rescheduling")]
        public string Reasons_Reschedule { get; set; }

        [Display(Name = "Audit Status")]
        public string Audit_Status { get; set; }

        [Display(Name = "Department Head")]
        public string dept_head { get; set; }

        [Display(Name = "Department Head Status")]
        public string dept_head_status { get; set; }

        [Display(Name = "Audit Team Status")]
        public string audit_team_status { get; set; }

        public DateTime dep_head_status_date { get; set; }
        public DateTime audit_team_status_date { get; set; }
        public string audit_status_date { get; set; }

        public string id_audit_process_perform { get; set; }
        public string Details { get; set; }
        public string Evidence { get; set; }
        public string Category { get; set; }
        public string Conformance { get; set; }
        public string evidence_upload { get; set; }

        [Display(Name = "NC No.")]
        public string Non_comformance { get; set; }

        public string nc_step_status { get; set; }
        public string nc_status { get; set; }
        public string nc_reject_reason { get; set; }
        public string nc_reject_upload { get; set; }
        public string nc_reject_response { get; set; }

        //Disposition
        [Display(Name = "Are actions taken solved NC")]
        public string disp_action_taken { get; set; }

        [Display(Name = "Brief explanation")]
        public string disp_explain { get; set; }

        [Display(Name = "Notified to")]
        public string disp_notifiedto { get; set; }

        [Display(Name = "Notified on")]
        public DateTime disp_notifeddate { get; set; }

        //Team
        [Display(Name = "Team")]
        public string nc_team { get; set; }

        [Display(Name = "Team Approved By")]
        public string team_approvedby { get; set; }

        [Display(Name = "Notified to")]
        public string team_notifiedto { get; set; }

        [Display(Name = "Target date to complete the RCA")]
        public DateTime team_targetdate { get; set; }

        //RCA
        [Display(Name = "Techniques adopted")]
        public string rca_technique { get; set; }

        [Display(Name = "Details of root causes analysis")]
        public string rca_details { get; set; }

        [Display(Name = "Upload documents")]
        public string rca_upload { get; set; }

        [Display(Name = "Need of corrective action")]
        public string rca_action { get; set; }

        [Display(Name = "If No, justify")]
        public string rca_justify { get; set; }

        [Display(Name = "Reported by")]
        public string rca_reportedby { get; set; }

        [Display(Name = "Notified to")]
        public string rca_notifiedto { get; set; }

        [Display(Name = "Reported On")]
        public DateTime rca_reporteddate { get; set; }

        //CA

        [Display(Name = "Verification due date")]
        public DateTime ca_verfiry_duedate { get; set; }

        [Display(Name = "Proposed by")]
        public string ca_proposed_by { get; set; }

        [Display(Name = "Notified to")]
        public string ca_notifiedto { get; set; }

        [Display(Name = "Notified Date")]
        public DateTime ca_notifed_date { get; set; }

        //Verification
        [Display(Name = "Are proposed actions implemented effectively?")]
        public string v_implement { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_implement_explain { get; set; }

        [Display(Name = "Is RCA process effective?")]
        public string v_rca { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_rca_explain { get; set; }

        [Display(Name = "Similar discrepancies detected from date of implementing corrective action?")]
        public string v_discrepancies { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_discrep_explain { get; set; }

        [Display(Name = "Upload Documents")]
        public string v_upload { get; set; }

        [Display(Name = "NCR Status")]
        public string v_status { get; set; }

        [Display(Name = "Closed Date")]
        public DateTime v_closed_date { get; set; }

        [Display(Name = "Verified by")]
        public string v_verifiedto { get; set; }

        [Display(Name = "Verified On")]
        public DateTime v_verified_date { get; set; }

        [Display(Name = "Notified To")]
        public string v_notifiedto { get; set; }

        //t_audit_nc_disp_action
        public string id_audit_nc_disp_action { get; set; }

        [Display(Name = "Action")]
        public string disp_action { get; set; }

        [Display(Name = "Person Responsible")]
        public string disp_resp_person { get; set; }

        [Display(Name = "Action Completed On")]
        public DateTime disp_complete_date { get; set; }

        //t_audit_nc_ca
        public string id_audit_nc_ca { get; set; }

        [Display(Name = "Division")]
        public string ca_div { get; set; }

        [Display(Name = "Location")]
        public string ca_loc { get; set; }

        [Display(Name = "Department")]
        public string ca_dept { get; set; }

        [Display(Name = "Root cause")]
        public string ca_rootcause { get; set; }

        [Display(Name = "Corrective action")]
        public string ca_ca { get; set; }

        [Display(Name = "Resource required")]
        public string ca_resource { get; set; }

        [Display(Name = "Target date")]
        public DateTime ca_target_date { get; set; }

        [Display(Name = "Person Responsible")]
        public string ca_resp_person { get; set; }

        [Display(Name = "Implementation status")]
        public string implement_status { get; set; }

        [Display(Name = "Is CA effective?")]
        public string ca_effective { get; set; }

        [Display(Name = "If No, reasons")]
        public string reason { get; set; }

        public string ca_active { get; set; }

        //---------------------------------------------------------------
        [Display(Name = "Id")]
        public string id_nc { get; set; }

        [Display(Name = "NC No")]
        public string nc_no { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string group_name { get; set; }

        [Display(Name = "Finding Category")]
        public string finding_category { get; set; }

        [Display(Name = "Finding Detail")]
        public string finding_details { get; set; }

        [Display(Name = "Corrective Action Date")]
        public DateTime corrective_action_date { get; set; }

        [Display(Name = "Risk due to NC")]
        public string risk_nc { get; set; }

        [Display(Name = "Risk level")]
        public string risk_level { get; set; }

        [Display(Name = "Is proposed corrective action effective to eliminate root causes of the NC reported?")]
        public string v_eleminate_root_cause { get; set; }

        [Display(Name = "Is identified Risk reduced to acceptable level?")]
        public string v_risk_reduce { get; set; }

        [Display(Name = "If Risk not reduced, specify the reasons")]
        public string v_risk_reduce_reason { get; set; }

        [Display(Name = "Any process amended or developed?")]
        public string v_process_amended { get; set; }

        [Display(Name = "Any document amended or developed?")]
        public string v_document_amended { get; set; }

        [Display(Name = "Document Ref")]
        public string v_amend_docref { get; set; }

        [Display(Name = "Document Name")]
        public string v_amend_docname { get; set; }

        [Display(Name = "Document Date")]
        public DateTime v_amend_docdate { get; set; }

        [Display(Name = "NCR Status")]
        public string v_ncr_status { get; set; }

        [Display(Name = "If open, specify the reasons")]
        public string v_ncr_reason { get; set; }

        [Display(Name = "Is proposed corrective action implemented effectively?")]
        public string v_corrective_implement { get; set; }

        [Display(Name = "Upload Documents")]
        public string disp_upload { get; set; }

        [Display(Name = "Id")]
        public string Plan_Id { get; set; }

        [Display(Name = "Clause/Section")]
        public string audit_clause { get; set; }

        [Display(Name = "Clause/Section Description")]
        public string description { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        [Display(Name = "Approval Status")]
        public string apprv_status { get; set; }

        [Display(Name = "Auditee(s)")]
        public string auditee_team { get; set; }

        public string auditors { get; set; }

        [Display(Name = "NC Date")]
        public DateTime nc_date { get; set; }

        [Display(Name = "Follow-up Date")]
        public DateTime followup_date { get; set; }

        public string finding_categorize { get; set; }

        public string auditors_name { get; set; }

        public string auditee_team_name { get; set; }

        [Display(Name = "RCA process started on")]
        public DateTime rca_started_date { get; set; }

        [Display(Name = "Audit Start Date")]
        public DateTime audit_start_date { get; set; }

        [Display(Name = "Corrective Action Agreed Date")]
        public DateTime corrective_agreed_date { get; set; }

        [Display(Name = "Audit Type")]
        public string audit_type { get; set; }

        [Display(Name = "External Entity Name")]
        public string entity_name { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Why finding is categorized as NC / PNC?")]
        public string why_finding { get; set; }

        internal bool FunUpdateDisposition(AuditLogModels objModels, AuditLogModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set  disp_action_taken='" + objModels.disp_action_taken + "', "
                    + "disp_explain='" + objModels.disp_explain + "', disp_notifiedto='" + objModels.disp_notifiedto + "', risk_nc='" + objModels.risk_nc + "',risk_level='" + objModels.risk_level + "',disp_upload='" + objModels.disp_upload + "'";

                if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objDispList.LogList.Count) > 0)
                    {
                        objDispList.LogList[0].id_nc = id_nc.ToString();
                        FunAddDispList(objDispList);
                    }
                    else
                    {
                        FunUpdateDispList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDisposition: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddDispList(AuditLogModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "delete from t_audit_nc_disp_action where id_nc='" + objDispList.LogList[0].id_nc + "'; ";

                for (int i = 0; i < objDispList.LogList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_audit_nc_disp_action(id_nc,disp_action,disp_resp_person";

                    string sFieldValue = "", sFields = "";
                    if (objDispList.LogList[i].disp_complete_date != null && objDispList.LogList[i].disp_complete_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sFields = sFields + ", disp_complete_date";
                        sFieldValue = sFieldValue + ", '" + objDispList.LogList[i].disp_complete_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objDispList.LogList[0].id_nc + "', '" + objDispList.LogList[i].disp_action + "', '" + objDispList.LogList[i].disp_resp_person + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddDispList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDispList(string id_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_audit_nc_disp_action where id_nc='" + id_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDispList: " + ex.ToString());
            }
            return false;
        }

        //Team
        internal bool FunUpdateTeam(AuditLogModels objModels)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set  nc_team='" + objModels.nc_team + "', "
                    + "team_approvedby='" + objModels.team_approvedby + "', team_notifiedto='" + objModels.team_notifiedto + "'";

                if (objModels.team_targetdate != null && objModels.team_targetdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", team_targetdate ='" + objModels.team_targetdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateTeam: " + ex.ToString());
            }
            return false;
        }

        //RCA
        internal bool FunUpdateRCA(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set  rca_technique='" + objModels.rca_technique + "', "
                    + "rca_details  ='" + objModels.rca_details + "', rca_upload='" + objModels.rca_upload + "', rca_action='" + objModels.rca_action
                    + "', rca_justify='" + objModels.rca_justify + "', rca_reportedby='" + objModels.rca_reportedby + "', rca_notifiedto='" + objModels.rca_notifiedto + "'";

                if (objModels.rca_reporteddate != null && objModels.rca_reporteddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_reporteddate ='" + objModels.rca_reporteddate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.rca_started_date != null && objModels.rca_started_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_started_date ='" + objModels.rca_started_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRCA: " + ex.ToString());
            }
            return false;
        }

        //CA
        internal bool FunUpdateCA(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set  ca_proposed_by='" + objModels.ca_proposed_by + "', "
                    + "ca_notifiedto='" + objModels.ca_notifiedto + "'";

                if (objModels.ca_verfiry_duedate != null && objModels.ca_verfiry_duedate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_verfiry_duedate ='" + objModels.ca_verfiry_duedate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.ca_notifed_date != null && objModels.ca_notifed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_notifed_date ='" + objModels.ca_notifed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.LogList.Count) > 0)
                    {
                        objList.LogList[0].id_nc = id_nc.ToString();
                        FunAddCAList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_audit_process_perform);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateCA: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCAList(AuditLogModelsList objCAList)
        {
            try
            {
                //string sSqlstmt = "delete from t_audit_nc_ca where id_audit_process_perform='" + objCAList.LogList[0].id_audit_process_perform + "'; ";
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.LogList.Count; i++)
                {
                    string sid_audit_nc_ca = "null";
                    string sca_target_date = "";

                    if (objCAList.LogList[i].id_audit_nc_ca != null && objCAList.LogList[i].id_audit_nc_ca != "")
                    {
                        sid_audit_nc_ca = objCAList.LogList[i].id_audit_nc_ca;
                    }

                    if (objCAList.LogList[i].ca_target_date != null && objCAList.LogList[i].ca_target_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sca_target_date = objCAList.LogList[i].ca_target_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_audit_nc_ca (id_audit_nc_ca,id_nc,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource,ca_resp_person,ca_target_date)"
                    + " values(" + sid_audit_nc_ca + "," + objCAList.LogList[0].id_nc + ",'" + objCAList.LogList[i].ca_div + "','" + objCAList.LogList[i].ca_loc + "','" + objCAList.LogList[i].ca_dept
                    + "','" + objCAList.LogList[i].ca_rootcause + "','" + objCAList.LogList[i].ca_ca + "','" + objCAList.LogList[i].ca_resource + "','" + objCAList.LogList[i].ca_resp_person + "','" + sca_target_date + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_audit_nc_ca= values(id_audit_nc_ca),id_nc= values(id_nc), ca_div= values(ca_div), ca_loc = values(ca_loc), ca_dept = values(ca_dept), ca_rootcause = values(ca_rootcause)," +
                    " ca_ca = values(ca_ca), ca_resource = values(ca_resource), ca_resp_person = values(ca_resp_person), ca_target_date = values(ca_target_date); ";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddCAList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCAList(string id_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_audit_nc_ca where id_nc='" + id_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateCAList: " + ex.ToString());
            }
            return false;
        }

        //Verification
        internal bool FunUpdateVerification(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set  v_implement='" + objModels.v_implement + "', v_implement_explain='" + objModels.v_implement_explain
                    + "', v_rca='" + objModels.v_rca + "', v_rca_explain='" + objModels.v_rca_explain + "', v_discrepancies='" + objModels.v_discrepancies + "', v_discrep_explain='" + objModels.v_discrep_explain
                    + "', v_upload='" + objModels.v_upload + "', v_status='" + objModels.v_status + "', v_verifiedto='" + objModels.v_verifiedto + "', v_notifiedto='" + objModels.v_notifiedto + "'"
                    + ",v_eleminate_root_cause='" + objModels.v_eleminate_root_cause + "',v_risk_reduce='" + objModels.v_risk_reduce + "',v_risk_reduce_reason='" + objModels.v_risk_reduce_reason + "',v_process_amended='" + objModels.v_process_amended + "',v_document_amended='" + objModels.v_document_amended + "',v_amend_docref='" + objModels.v_amend_docref + "',v_amend_docname='" + objModels.v_amend_docname + "',v_ncr_reason='" + objModels.v_ncr_reason + "'";

                if (objModels.v_closed_date != null && objModels.v_closed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_closed_date ='" + objModels.v_closed_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.v_amend_docdate != null && objModels.v_amend_docdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_amend_docdate ='" + objModels.v_amend_docdate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.v_verified_date != null && objModels.v_verified_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_verified_date ='" + objModels.v_verified_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.LogList.Count) > 0)
                    {
                        objList.LogList[0].id_nc = id_nc.ToString();
                        FunAddVerificationList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateVerification: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddVerificationList(AuditLogModelsList objCAList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.LogList.Count; i++)
                {
                    string sid_audit_nc_ca = "";

                    if (objCAList.LogList[i].id_audit_nc_ca != null && objCAList.LogList[i].id_audit_nc_ca != "")
                    {
                        sid_audit_nc_ca = objCAList.LogList[i].id_audit_nc_ca;
                    }
                    sSqlstmt = sSqlstmt + " update t_audit_nc_ca set implement_status = '" + objCAList.LogList[i].implement_status
                        + "', ca_effective = '" + objCAList.LogList[i].ca_effective + "', reason ='" + objCAList.LogList[i].reason + "' where id_audit_nc_ca = '" + sid_audit_nc_ca + "';";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddVerificationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCADoc(string sid_audit_nc_ca)
        {
            try
            {
                string sSqlstmt = "update t_audit_nc_ca set ca_active=0 where id_audit_nc_ca='" + sid_audit_nc_ca + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteCADoc: " + ex.ToString());
            }
            return false;
        }

        //----------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------------
        internal bool FunExtAuditUpdateDisposition(AuditLogModels objModels, AuditLogModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set  disp_action_taken='" + objModels.disp_action_taken + "', "
                    + "disp_explain='" + objModels.disp_explain + "', disp_notifiedto='" + objModels.disp_notifiedto + "', risk_nc='" + objModels.risk_nc + "',risk_level='" + objModels.risk_level + "',disp_upload='" + objModels.disp_upload + "'";

                if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objDispList.LogList.Count) > 0)
                    {
                        objDispList.LogList[0].id_nc = id_nc.ToString();
                        FunExtAuditAddDispList(objDispList);
                    }
                    else
                    {
                        FunExtAuditUpdateDispList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDisposition: " + ex.ToString());
            }
            return false;
        }

        internal bool FunExtAuditAddDispList(AuditLogModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "delete from t_external_audit_nc_disp_action where id_nc='" + objDispList.LogList[0].id_nc + "'; ";

                for (int i = 0; i < objDispList.LogList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_external_audit_nc_disp_action(id_nc,disp_action,disp_resp_person";

                    string sFieldValue = "", sFields = "";
                    if (objDispList.LogList[i].disp_complete_date != null && objDispList.LogList[i].disp_complete_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sFields = sFields + ", disp_complete_date";
                        sFieldValue = sFieldValue + ", '" + objDispList.LogList[i].disp_complete_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objDispList.LogList[0].id_nc + "', '" + objDispList.LogList[i].disp_action + "', '" + objDispList.LogList[i].disp_resp_person + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddDispList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunExtAuditUpdateDispList(string id_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_external_audit_nc_disp_action where id_nc='" + id_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDispList: " + ex.ToString());
            }
            return false;
        }

        //RCA
        internal bool FunExtAuditUpdateRCA(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set  rca_technique='" + objModels.rca_technique + "', "
                    + "rca_details  ='" + objModels.rca_details + "', rca_upload='" + objModels.rca_upload + "', rca_action='" + objModels.rca_action
                    + "', rca_justify='" + objModels.rca_justify + "', rca_reportedby='" + objModels.rca_reportedby + "', rca_notifiedto='" + objModels.rca_notifiedto + "'";

                if (objModels.rca_reporteddate != null && objModels.rca_reporteddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_reporteddate ='" + objModels.rca_reporteddate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.rca_started_date != null && objModels.rca_started_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_started_date ='" + objModels.rca_started_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAuditUpdateRCA: " + ex.ToString());
            }
            return false;
        }

        //CA
        internal bool FunExtAuditUpdateCA(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set  ca_proposed_by='" + objModels.ca_proposed_by + "', "
                    + "ca_notifiedto='" + objModels.ca_notifiedto + "'";

                if (objModels.ca_verfiry_duedate != null && objModels.ca_verfiry_duedate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_verfiry_duedate ='" + objModels.ca_verfiry_duedate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.ca_notifed_date != null && objModels.ca_notifed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_notifed_date ='" + objModels.ca_notifed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.LogList.Count) > 0)
                    {
                        objList.LogList[0].id_nc = id_nc.ToString();
                        FunExtAudiAddCAList(objList);
                    }
                    else
                    {
                        FunExtAuditUpdateCAList(id_audit_process_perform);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAuditUpdateCA: " + ex.ToString());
            }
            return false;
        }

        internal bool FunExtAudiAddCAList(AuditLogModelsList objCAList)
        {
            try
            {
                //string sSqlstmt = "delete from t_audit_nc_ca where id_audit_process_perform='" + objCAList.LogList[0].id_audit_process_perform + "'; ";
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.LogList.Count; i++)
                {
                    string sid_audit_nc_ca = "null";
                    string sca_target_date = "";

                    if (objCAList.LogList[i].id_audit_nc_ca != null && objCAList.LogList[i].id_audit_nc_ca != "")
                    {
                        sid_audit_nc_ca = objCAList.LogList[i].id_audit_nc_ca;
                    }

                    if (objCAList.LogList[i].ca_target_date != null && objCAList.LogList[i].ca_target_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sca_target_date = objCAList.LogList[i].ca_target_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_external_audit_nc_ca (id_audit_nc_ca,id_nc,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource,ca_resp_person,ca_target_date)"
                    + " values(" + sid_audit_nc_ca + "," + objCAList.LogList[0].id_nc + ",'" + objCAList.LogList[i].ca_div + "','" + objCAList.LogList[i].ca_loc + "','" + objCAList.LogList[i].ca_dept
                    + "','" + objCAList.LogList[i].ca_rootcause + "','" + objCAList.LogList[i].ca_ca + "','" + objCAList.LogList[i].ca_resource + "','" + objCAList.LogList[i].ca_resp_person + "','" + sca_target_date + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_audit_nc_ca= values(id_audit_nc_ca),id_nc= values(id_nc), ca_div= values(ca_div), ca_loc = values(ca_loc), ca_dept = values(ca_dept), ca_rootcause = values(ca_rootcause)," +
                    " ca_ca = values(ca_ca), ca_resource = values(ca_resource), ca_resp_person = values(ca_resp_person), ca_target_date = values(ca_target_date); ";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAudiAddCAList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunExtAuditUpdateCAList(string id_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_external_audit_nc_ca where id_nc='" + id_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAuditUpdateCAList: " + ex.ToString());
            }
            return false;
        }

        //Team
        internal bool FunExtAuditUpdateTeam(AuditLogModels objModels)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set  nc_team='" + objModels.nc_team + "', "
                    + "team_approvedby='" + objModels.team_approvedby + "', team_notifiedto='" + objModels.team_notifiedto + "'";

                if (objModels.team_targetdate != null && objModels.team_targetdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", team_targetdate ='" + objModels.team_targetdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAuditUpdateTeam: " + ex.ToString());
            }
            return false;
        }

        //Verification
        internal bool FunExtAuditUpdateVerification(AuditLogModels objModels, AuditLogModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set  v_implement='" + objModels.v_implement + "', v_implement_explain='" + objModels.v_implement_explain
                    + "', v_rca='" + objModels.v_rca + "', v_rca_explain='" + objModels.v_rca_explain + "', v_discrepancies='" + objModels.v_discrepancies + "', v_discrep_explain='" + objModels.v_discrep_explain
                    + "', v_upload='" + objModels.v_upload + "', v_status='" + objModels.v_status + "', v_verifiedto='" + objModels.v_verifiedto + "', v_notifiedto='" + objModels.v_notifiedto + "'"
                    + ",v_eleminate_root_cause='" + objModels.v_eleminate_root_cause + "',v_risk_reduce='" + objModels.v_risk_reduce + "',v_risk_reduce_reason='" + objModels.v_risk_reduce_reason + "',v_process_amended='" + objModels.v_process_amended + "',v_document_amended='" + objModels.v_document_amended + "',v_amend_docref='" + objModels.v_amend_docref + "',v_amend_docname='" + objModels.v_amend_docname + "',v_ncr_reason='" + objModels.v_ncr_reason + "'";

                if (objModels.v_closed_date != null && objModels.v_closed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_closed_date ='" + objModels.v_closed_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.v_amend_docdate != null && objModels.v_amend_docdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_amend_docdate ='" + objModels.v_amend_docdate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.v_verified_date != null && objModels.v_verified_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_verified_date ='" + objModels.v_verified_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.LogList.Count) > 0)
                    {
                        objList.LogList[0].id_nc = id_nc.ToString();
                        FunAddExtAuditVerificationList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunExtAuditUpdateVerification: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExtAuditVerificationList(AuditLogModelsList objCAList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.LogList.Count; i++)
                {
                    string sid_audit_nc_ca = "";

                    if (objCAList.LogList[i].id_audit_nc_ca != null && objCAList.LogList[i].id_audit_nc_ca != "")
                    {
                        sid_audit_nc_ca = objCAList.LogList[i].id_audit_nc_ca;
                    }
                    sSqlstmt = sSqlstmt + " update t_external_audit_nc_ca set implement_status = '" + objCAList.LogList[i].implement_status
                        + "', ca_effective = '" + objCAList.LogList[i].ca_effective + "', reason ='" + objCAList.LogList[i].reason + "' where id_audit_nc_ca = '" + sid_audit_nc_ca + "';";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddExtAuditVerificationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteExtAuditCADoc(string sid_audit_nc_ca)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc_ca set ca_active=0 where id_audit_nc_ca='" + sid_audit_nc_ca + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteExtAuditCADoc: " + ex.ToString());
            }
            return false;
        }
    }

    public class AuditLogModelsList
    {
        public List<AuditLogModels> LogList { get; set; }
    }
}