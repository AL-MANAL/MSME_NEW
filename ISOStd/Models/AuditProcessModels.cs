using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;


namespace ISOStd.Models
{
    public class AuditProcessModels
    {

        clsGlobal objGlobaldata = new clsGlobal();

        //----------------------------------------t_audit_process--------------------------------------

        [Display(Name = "Audit Id")]
        public string Audit_Id { get; set; }

        [Display(Name = "Audit Criteria")]
        public string Audit_criteria { get; set; }

        [Display(Name = "Planned By")]
        public string PlannedBy { get; set; }

        [Display(Name = "To be Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Audit No")]
        public string Audit_no { get; set; }

        [Display(Name = "Audit Planned On")]
        public DateTime AuditPlanDate { get; set; }

        [Display(Name = "Audit Plan Approval Status")]
        public string Approved_Status { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        [Display(Name = "Logged Date")]
        public DateTime logged_date { get; set; }

        //-------------------------------------------t_audit_process_plan--------------------------------------------------

        [Display(Name = "Id")]
        public string Plan_Id { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string group_name { get; set; }
        
        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Audit Scheduled Date")]
        public DateTime AuditDate { get; set; }

        [Display(Name = "Check List")]
        public string checklist { get; set; }

        [Display(Name = "Auditor(s)")]
        public string auditors { get; set; }

        [Display(Name = "Auditee(s)")]
        public string auditee_team { get; set; }

        [Display(Name = "From Time")]
        public string fromtime { get; set; }

        [Display(Name = "To Time")]
        public string totime { get; set; }

        [Display(Name = "Approval Status")]
        public string apprv_status { get; set; }

        public string comments { get; set; }

        [Display(Name = "Audit Status")]
        public string Audit_Status { get; set; }

        [Display(Name = "Date")]
        public DateTime audit_status_date { get; set; }

        [Display(Name = "Comments")]
        public string remarks { get; set; }

        [Display(Name = "No.of NCs Raised")]
        public int total_nc { get; set; }

        public string auditors_name { get; set; }

        public string auditee_team_name { get; set; }
        //-------------------------------------------t_audit_process_nc--------------------------------------------------
        [Display(Name = "Id")]
        public string id_nc { get; set; }

        [Display(Name = "NC Date")]
        public DateTime nc_date { get; set; }

        [Display(Name = "Finding Detail")]
        public string finding_details { get; set; }

        [Display(Name = "Upload Documents")]
        public string upload { get; set; }

        [Display(Name = "Finding Category")]
        public string finding_category { get; set; }

        [Display(Name = "Clause/Section")]
        public string audit_clause { get; set; }

        [Display(Name = "Clause/Section Description")]
        public string description { get; set; }

        [Display(Name = "NC No")]
        public string nc_no { get; set; }

        [Display(Name = "Reason for Rejection")]
        public string reason_rejection { get; set; }

        [Display(Name = "Upload Evidence")]
        public string upload_evidence { get; set; }

        [Display(Name = "Approve/Reject Date")]
        public DateTime aprvrejct_date { get; set; }

        [Display(Name = "Corrective Action Date")]
        public DateTime corrective_action_date { get; set; }

        public string apprvby_auditee { get; set; }

        [Display(Name = "Followup Date")]
        public DateTime followup_date { get; set; }

        //----------------------------------------------t_auditor_detail----------------------------------------------
        [Display(Name = "Id")]
        public string id_auditor { get; set; }

        [Display(Name = "Auditor No")]
        public string auditor_no { get; set; }

        [Display(Name = "Auditor")]
        public string auditor_name { get; set; }

        [Display(Name = "Academic qualification")]
        public string qualification { get; set; }

        [Display(Name = "Years of experience")]
        public string years_exp { get; set; }

        [Display(Name = "Trainings completed")]
        public string trainings_completed { get; set; }

        [Display(Name = "Upload certificate(s)")]
        public string upload_cetificate { get; set; }

        [Display(Name = "Updated on")]
        public DateTime updated_on { get; set; }


        //-----------------------------------------------t_auditor_availability-----------------------------------------------

        [Display(Name = "Id")]
        public string id_availability { get; set; }

        [Display(Name = "From Date")]
        public DateTime from_date { get; set; }

        [Display(Name = "To Date")]
        public DateTime to_date { get; set; }
        //--------------------------------------------t_auditor_detail_certificate-------------------------------------------

        [Display(Name = "Id")]
        public string id_certificate { get; set; }

        [Display(Name = "ISO Standards")]
        public string standards { get; set; }

        [Display(Name = "Type of Course Completed")]
        public string type_course { get; set; }

        [Display(Name = "Course Completed On")]
        public DateTime completed_on { get; set; }

        [Display(Name = "Years of Auditing Experience")]
        public string yearsexp { get; set; }

        //--------------------------------------------t_audit_type-------------------------------------------
        [Display(Name = "Id")]
        public string id_audit_type { get; set; }

        [Display(Name = "Audit Type")]
        public string audit_type { get; set; }

        [Display(Name = "Audit Code")]
        public string audit_code { get; set; }

        [Display(Name = "Audit Description")]
        public string audit_desc { get; set; }

        internal bool FunAddAuditType(AuditProcessModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_audit_type (audit_type,audit_code,audit_desc) values('" + objModel.audit_type + "','" + objModel.audit_code + "','" + objModel.audit_desc + "')";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditType: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateAuditType(AuditProcessModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_audit_type set audit_type = '" + objModel.audit_type + "',audit_code = '" + objModel.audit_code + "',audit_desc = '" + objModel.audit_desc + "'  where id_audit_type=" + id_audit_type;
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateAuditType: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteAuditType(string sId)
        {
            try
            {
                string sSqlstmt = "update t_audit_type set Active=0 where id_audit_type=" + sId;

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteAuditType: " + ex.ToString());
            }
            return false;
        }
        internal bool FunCheckAuditCodeExsists(string audit_code)
        {
            try
            {           
                if (audit_code != "" && audit_code != null)
                {
                    DataSet dsDoc = objGlobaldata.Getdetails("select audit_code from t_audit_type where active=1 and audit_code='" + audit_code + "'");

                    if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunCheckAuditCodeExsists: " + ex.ToString());
            }
            return false;
        }
        //-----------------------------------------------------------------------------------------------
        public string PlannedById { get; set; }

        
        public string checklistId { get; set; }

        

       
        
        [Display(Name = "Department")]
        public string dept { get; set; }
        public string deptId { get; set; }

      

        [Display(Name = "Division")]
        public string division { get; set; }
        public string divisionId { get; set; }

      

        public string teamId { get; set; }

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

       
       
        [Display(Name = "Department Head")]
        public string dept_head { get; set; }

        [Display(Name = "Department Head Status")]
        public string dept_head_status { get; set; }

        [Display(Name = "Audit Team Status")]
        public string audit_team_status { get; set; }

        public DateTime dep_head_status_date { get; set; }
        public DateTime audit_team_status_date { get; set; }
      


        public string id_audit_process_perform { get; set; }
        public string Details { get; set; }
        public string Evidence { get; set; }
        public string Category { get; set; }
        public string Conformance { get; set; }
        public string Non_comformance { get; set; }
        public string evidence_upload { get; set; }

        public string nc_step_status { get; set; }
        public string nc_status { get; set; }
        public string nc_reject_reason { get; set; }
        public string nc_reject_upload { get; set; }
        public string nc_reject_response { get; set; }
        [Display(Name = "Remarks")]
        public string why_nc { get; set; }

        [Display(Name = "Auditee Acceptance")]
        public string auditee_acceptance { get; set; }

        [Display(Name = "Corrective Action Date")]
        public DateTime ca_date { get; set; }

     
       
        public string auditee_teamId { get; set; }

        internal bool FunAddAuditProcess(AuditProcessModels objAudit, AuditProcessModelsList objAuditList)
        {
            try
            {

                string user = "";
                user = objGlobaldata.GetCurrentUserSession().empid;

                string[] name = objAudit.ApprovedBy.Split(',');
                int apprvcount = name.Length;

                string sSqlstmt = "insert into t_audit_process (Audit_no,Audit_criteria,PlannedBy,ApprovedBy,ApproverCount,logged_by,audit_type,audit_code";
          
                string sFields = "", sFieldValue = "";

                if (objAudit.AuditPlanDate != null && objAudit.AuditPlanDate > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", AuditPlanDate";
                    sFieldValue = sFieldValue + ", '" + objAudit.AuditPlanDate.ToString("yyyy/MM/dd") + "'";
                }
               
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + Audit_no + "','" + objAudit.Audit_criteria + "','" + objAudit.PlannedBy + "','" + objAudit.ApprovedBy + "','" + apprvcount + "','" + user + "','" + audit_type + "','" + audit_code + "'";
              
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int Audit_Id = 0;
                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out Audit_Id))
                {
                    string[] apprv_by = objAudit.ApprovedBy.Split(',');
                    string sql = "";
                    foreach (var item in apprv_by)
                    {
                        sql = sql + "insert into t_audit_process_approval(Audit_Id,approver) values ('" + Audit_Id + "','" + item + "');";
                    }
                    objGlobaldata.ExecuteQuery(sql);
                    if (Audit_Id > 0 && objAuditList.Obj.Count > 0)
                    {
                        objAuditList.Obj[0].Audit_Id = Audit_Id.ToString();
                        FunAddAuditPlanList(objAuditList);
                        return SendAuditProcessmail(Audit_Id, "Internal Audit Planning for Approval");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditProcess: " + ex.ToString());
            }
            return false;
        }

        internal bool SendAuditProcessmail(int Audit_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,logged_by,ApprovedBy"
                    + " from t_audit_process where Audit_Id='" + Audit_Id + "'";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);
               
                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());

                    sHeader = "<tr><td colspan=3><b>Audit No:<b></td> <td colspan=3>"
                        + (dsAuditList.Tables[0].Rows[0]["Audit_no"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Audit Criteria:<b></td> <td colspan=3>" + objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString()) + "</td></tr>";


                    sSqlstmt = "select Plan_Id,Audit_Id,branch,group_name,location,AuditDate,fromtime,totime,auditors,auditee_team"
                                     + " from t_audit_process_plan where Audit_Id='" + dsAuditList.Tables[0].Rows[0]["Audit_Id"].ToString()
                                     + "' order by Plan_Id";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=8><label><b>Audit Plan Details</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Division</th>"
                            + "<th style='width:300px'>Department</th>"
                            + "<th style='width:300px'>Location</th>"
                            + "<th style='width:300px'>Audit Scheduled Date</th>"
                            + "<th style='width:300px'>Auditor(s)</th>"
                            + "<th style='width:300px'>Auditee(s)</th>"
                            + "</tr>";

                      
                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {
                            string date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + " "+ dsItems.Tables[0].Rows[i]["fromtime"].ToString() +" - "+ dsItems.Tables[0].Rows[i]["totime"].ToString();


                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + objGlobaldata.GetMultiCompanyBranchNameById(dsItems.Tables[0].Rows[i]["branch"].ToString()) + "</td>"
                                + " <td style='width:300px'>" + objGlobaldata.GetMultiDeptNameById(dsItems.Tables[0].Rows[i]["group_name"].ToString()) + "</td>"
                                + " <td style='width:300px'>" + objGlobaldata.GetDivisionLocationById(dsItems.Tables[0].Rows[i]["location"].ToString()) + "</td>"
                                + " <td style='width:300px'>" + date + "</td>" 
                             
                                 + "<td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["auditors"].ToString()) + "</td>"
                                 + "<td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["auditee_team"].ToString()) + "</td>"
                                                       + " </tr>";
                            //sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());

                            //string sPersonEmail = objGlobaldata.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                            //sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                            //sPersonEmail = sPersonEmail.Trim();
                            //sPersonEmail = sPersonEmail.TrimEnd(',');
                            //sPersonEmail = sPersonEmail.TrimStart(',');
                            //if (sPersonEmail != null && sPersonEmail != "")
                            //{
                            //    sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                            //}

                        }
                    }


                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Internal Audit Planning Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendMOMEmail: " + ex.ToString());
            }
            return false;
        }

     
        internal bool FunAddAuditPlanList(AuditProcessModelsList objAuditList)
        {
            try
            {
               
                string sSqlstmt = "delete from t_audit_process_plan where Audit_Id='" + objAuditList.Obj[0].Audit_Id + "'; ";

                string sql = "delete from t_auditee_approval where Audit_Id='" + objAuditList.Obj[0].Audit_Id + "'; ";
                string sql1 = "delete from t_auditor_approval where Audit_Id='" + objAuditList.Obj[0].Audit_Id + "'; ";
                string auditstatus = objGlobaldata.GetAuditStatusIdByName("Planned");
                for (int i = 0; i < objAuditList.Obj.Count; i++)
                { 
                    string sPlan_Id = "null";
                    sSqlstmt = sSqlstmt + "insert into t_audit_process_plan(Plan_Id,Audit_Id,branch,group_name,location,fromtime,totime,checklist,auditors,auditee_team,Audit_Status";

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;
                    if (objAuditList.Obj[i].Plan_Id != null)
                    {
                        sPlan_Id = objAuditList.Obj[i].Plan_Id;
                    }
                    if (objAuditList.Obj[i].AuditDate != null && objAuditList.Obj[i].AuditDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", AuditDate= values(AuditDate)";
                        sFields = sFields + ", AuditDate";
                        sFieldValue = sFieldValue + ", '" + objAuditList.Obj[i].AuditDate.ToString("yyyy/MM/dd") + "'";
                    }
                  
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sPlan_Id + ",'" + objAuditList.Obj[0].Audit_Id + "','" + objAuditList.Obj[i].branch + "','" + objAuditList.Obj[i].group_name + "','" + objAuditList.Obj[i].location + "','" + objAuditList.Obj[i].fromtime + "','" + objAuditList.Obj[i].totime + "','" + objAuditList.Obj[i].checklist + "','" + objAuditList.Obj[i].auditors + "','" + objAuditList.Obj[i].auditee_team + "','"+ auditstatus + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " Plan_Id= values(Plan_Id), Audit_Id= values(Audit_Id), branch = values(branch), group_name= values(group_name)"
                    + ", location= values(location), fromtime= values(fromtime), totime= values(totime), checklist= values(checklist), auditors= values(auditors),auditee_team= values(auditee_team),Audit_Status= values(Audit_Status)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                    int Plan_Id = 0;
                    if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out Plan_Id))
                    {                     
                        string[] appr_by = objAuditList.Obj[i].auditee_team.Split(',');
                        foreach (var item in appr_by)
                        {
                            sql = sql + "insert into t_auditee_approval(Audit_Id,Plan_Id,auditee) values ('" + objAuditList.Obj[0].Audit_Id + "','"+Plan_Id+"','"+item+"');";
                        }       
                        objGlobaldata.ExecuteQuery(sql);

                        string[] auditor_by = objAuditList.Obj[i].auditors.Split(',');
                        foreach (var item in auditor_by)
                        {
                            sql1 = sql1 + "insert into t_auditor_approval(Audit_Id,Plan_Id,auditor) values ('" + objAuditList.Obj[0].Audit_Id + "','" + Plan_Id + "','" + item + "');";
                        }
                        objGlobaldata.ExecuteQuery(sql1);
                        sql = "";
                        sql1 = "";
                        sSqlstmt = "";
                    }
                }
                return true;
                    
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditPlanList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditProcess(AuditProcessModels objAudit, AuditProcessModelsList objAuditList)
        {
            try
            {
                string[] name = objAudit.ApprovedBy.Split(',');
                int apprvcount = name.Length;


                string sSqlstmt = "update t_audit_process set Audit_criteria='" + Audit_criteria + "',PlannedBy='" + PlannedBy + "',ApprovedBy='" + ApprovedBy + "',ApproverCount='" + apprvcount + "',audit_type='" + audit_type + "',audit_code='" + audit_code + "'";
                  
                if (objAudit.AuditPlanDate != null && objAudit.AuditPlanDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",AuditPlanDate='" + objAudit.AuditPlanDate.ToString("yyyy/MM/dd") + "'";
                }
               
                sSqlstmt = sSqlstmt + " where Audit_Id='" + objAudit.Audit_Id + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    string[] apprv_by = objAudit.ApprovedBy.Split(',');
                    string sql = "delete from t_audit_process_approval where Audit_Id='"+ Audit_Id + "';";
                    foreach (var item in apprv_by)
                    {
                        sql = sql + "insert into t_audit_process_approval(Audit_Id,approver) values ('" + Audit_Id + "','" + item + "');";
                    }
                    objGlobaldata.ExecuteQuery(sql);
                    if (objAuditList.Obj.Count > 0)
                    {
                        objAuditList.Obj[0].Audit_Id = Audit_Id.ToString();
                        return FunAddAuditPlanList(objAuditList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateAuditSchedule: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAuditProcessApprove(AuditProcessModels objAudit)
        {
            try
            {

                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                string sSql = "update t_audit_process_approval set approver='"+ user + "',apprv_status='"+ apprv_status + "',apprv_date='"+ sApprovedDate + "',comments='"+ comments + "' where Audit_Id='"+ Audit_Id + "' and approver='"+user+"'";
                objGlobaldata.ExecuteQuery(sSql);

                if (objAudit.apprv_status == "2") // approve
                {
                    string sSqlstmt1 = "update t_audit_process set ApproverCount=ApproverCount-1,Approvers= concat(Approvers,',','" + user + "') where Audit_Id='" + Audit_Id + "'";
                    if (objGlobaldata.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount,ApprovedBy,Approvers from t_audit_process where Audit_Id='" + Audit_Id + "'";
                        DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsData.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string[] apprv_by = dsData.Tables[0].Rows[0]["ApprovedBy"].ToString().Split(',');
                                string[] apprv = dsData.Tables[0].Rows[0]["Approvers"].ToString().Split(',');
                                int apprv_by_cnt = 0;int apprv_cnt=0;
                                apprv_by_cnt = apprv_by.Count();
                                apprv_cnt = apprv.Count();

                                if(apprv_by_cnt == (apprv_cnt-1)) // approve
                                {
                                    string Sql1 = "update t_audit_process set Approved_Status ='2' where Audit_Id='" + Audit_Id + "'";
                                     objGlobaldata.ExecuteQuery(Sql1);
                                    return SendAuditProcessApprvmail(Audit_Id, apprv_status, comments, "Internal Audit Planning");
                                }
                                else // reject
                                {
                                    string Sql1 = "update t_audit_process set Approved_Status ='1' where Audit_Id='" + Audit_Id + "'";
                                     objGlobaldata.ExecuteQuery(Sql1);
                                    return SendAuditProcessApprvmail(Audit_Id, apprv_status, comments, "Internal Audit Planning");
                                }
                            }
                            
                        }
                    }
                }
                if (objAudit.apprv_status == "1") // rejected
                {
                    string sSqlstmt1 = "update t_audit_process set ApproverCount=ApproverCount-1,ApprovalRejector= concat(ApprovalRejector,',','" + user + "') where Audit_Id='" + Audit_Id + "'";
                    if (objGlobaldata.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount,ApprovedBy,Approvers from t_audit_process where Audit_Id='" + Audit_Id + "'";
                        DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsData.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                    string Sql1 = "update t_audit_process set Approved_Status ='1' where Audit_Id='" + Audit_Id + "'";
                                     objGlobaldata.ExecuteQuery(Sql1);
                                    return SendAuditProcessApprvmail(Audit_Id, apprv_status, comments, "Internal Audit Planning");
                            }                          
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAuditProcessApprove: " + ex.ToString());
            }
            return false;
        }

        internal bool SendAuditProcessApprvmail(string Audit_Id, string iStatus, string comment, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select Audit_Id,Audit_criteria,PlannedBy,Audit_no,AuditPlanDate,ApprovedBy,logged_by"
                    + " from t_audit_process where Audit_Id='" + Audit_Id + "'";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["ApprovedBy"].ToString());

                    if (iStatus == "2")
                    {
                        sHeader = "<tr><td colspan=3><b>Audit No:<b></td> <td colspan=3>"
                     + (dsAuditList.Tables[0].Rows[0]["Audit_no"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Audit Criteria:<b></td> <td colspan=3>" + objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(objGlobaldata.GetCurrentUserSession().empid) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3> Approved</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>Audit No:<b></td> <td colspan=3>"
                     + (dsAuditList.Tables[0].Rows[0]["Audit_no"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Audit Criteria:<b></td> <td colspan=3>" + objGlobaldata.GetIsoStdDescriptionById(dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Rejected By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(objGlobaldata.GetCurrentUserSession().empid) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3> Rejected</td></tr>";
                    }


                    //sSqlstmt = "select Plan_Id,Audit_Id,branch,group_name,location,AuditDate,fromtime,totime,auditors,auditee_team"
                    //                 + " from t_audit_process_plan where Audit_Id='" + dsAuditList.Tables[0].Rows[0]["Audit_Id"].ToString()
                    //                 + "' order by Plan_Id";

                    //DataSet dsItems = new DataSet();
                    //dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    //if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    //{
                    //    sInformation = "<tr> "
                    //        + "<td colspan=8><label><b>Audit Plan Details</b></label> </td> </tr>"
                    //        + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                    //        + "<th>Sl. No.</th>"
                    //        + "<th style='width:300px'>Directorate</th>"
                    //        + "<th style='width:300px'>Group</th>"                    //        
                    //        + "<th style='width:300px'>Location</th>"
                    //        + "<th style='width:300px'>Audit Scheduled Date</th>"
                    //        + "<th style='width:300px'>Auditor(s)</th>"
                    //        + "<th style='width:300px'>Auditee(s)</th>"
                    //        + "</tr>";


                    //    for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                    //    {
                    //        string date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + " " + dsItems.Tables[0].Rows[i]["fromtime"].ToString() + " - " + dsItems.Tables[0].Rows[i]["totime"].ToString();


                    //        sInformation = sInformation + "<tr>"
                    //            + " <td>" + (i + 1) + "</td>"
                    //            + " <td style='width:300px'>" + objGlobaldata.GetMultiCompanyBranchNameById(dsItems.Tables[0].Rows[i]["branch"].ToString()) + "</td>"
                    //            + " <td style='width:300px'>" + objGlobaldata.GetMultiDeptNameById(dsItems.Tables[0].Rows[i]["group_name"].ToString()) + "</td>"
                    //            + " <td style='width:300px'>" + objGlobaldata.GetDivisionLocationById(dsItems.Tables[0].Rows[i]["location"].ToString()) + "</td>"
                    //            + " <td style='width:300px'>" + date + "</td>"

                    //             + "<td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["auditors"].ToString()) + "</td>"
                    //             + "<td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["auditee_team"].ToString()) + "</td>"
                    //                                   + " </tr>";
                    //        //sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());

                    //        //string sPersonEmail = objGlobaldata.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                    //        //sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                    //        //sPersonEmail = sPersonEmail.Trim();
                    //        //sPersonEmail = sPersonEmail.TrimEnd(',');
                    //        //sPersonEmail = sPersonEmail.TrimStart(',');
                    //        //if (sPersonEmail != null && sPersonEmail != "")
                    //        //{
                    //        //    sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                    //        //}

                    //    }
                    //}


                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Internal Audit Planning Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendAuditProcessApprvmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAuditeeApprove(AuditProcessModelsList objAudit)
        {
            try
            {
                string user = "";
                user = objGlobaldata.GetCurrentUserSession().empid;
                string TodayDate =  DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                if (objAudit.Obj.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objAudit.Obj.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_auditee_approval set apprv_status='" + objAudit.Obj[i].apprv_status + "',"
                            + "apprv_date='" + TodayDate + "',comments='" + objAudit.Obj[i].comments + "'";
                        //if (AuditElemtlist.lstAudit[i].evidence_upload != null)
                        //{
                        //    sSqlstmt = sSqlstmt + ", evidence_upload='" + AuditElemtlist.lstAudit[i].evidence_upload + "' ";
                        //}
                        sSqlstmt = sSqlstmt + " where Audit_Id='" + objAudit.Obj[i].Audit_Id + "' and Plan_Id='" + objAudit.Obj[i].Plan_Id + "' and auditee='"+ user + "';";
                    }
                    return objGlobaldata.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAuditeeApprove: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAuditorApprove(AuditProcessModelsList objAudit)
        {
            try
            {
                string user = "";
                user = objGlobaldata.GetCurrentUserSession().empid;
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                if (objAudit.Obj.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objAudit.Obj.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_auditor_approval set apprv_status='" + objAudit.Obj[i].apprv_status + "',"
                            + "apprv_date='" + TodayDate + "',comments='" + objAudit.Obj[i].comments + "'";
                        //if (AuditElemtlist.lstAudit[i].evidence_upload != null)
                        //{
                        //    sSqlstmt = sSqlstmt + ", evidence_upload='" + AuditElemtlist.lstAudit[i].evidence_upload + "' ";
                        //}
                        sSqlstmt = sSqlstmt + " where Audit_Id='" + objAudit.Obj[i].Audit_Id + "' and Plan_Id='" + objAudit.Obj[i].Plan_Id + "' and auditor='" + user + "';";
                    }
                    return objGlobaldata.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAuditorApprove: " + ex.ToString());
            }
            return false;
        }

        internal bool FunCheckAuditNoExsists(string Audit_no)
        {
            try
            {
                string syear = DateTime.Now.ToString("yyyy");
                if (Audit_no != "" && Audit_no != null)
                {
                    DataSet dsDoc = objGlobaldata.Getdetails("select Audit_no from t_audit_process where active=1 and year(AuditPlanDate)='"+ syear + "' and Audit_no='" + Audit_no + "'");

                    if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunCheckAuditNoExsists: " + ex.ToString());
            }
            return false;
        }

        internal bool FunCheckAuditorExsists(string auditor_name)
        {
            try
            {
               
                if (auditor_name != "" && auditor_name != null)
                {
                    DataSet dsDoc = objGlobaldata.Getdetails("select id_auditor from t_auditor_detail where active=1 and auditor_name = '"+ auditor_name + "'");

                    if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunCheckAuditorExsists: " + ex.ToString());
            }
            return false;
        }

        //Audit status update
        internal bool FunUpdateAuditStatus(AuditProcessModels objAudit)
        {
            try
            {
          
                string sSqlstmt = "update t_audit_process_plan set Audit_Status='" + Audit_Status + "',remarks='" + remarks + "'";
                if (objAudit.audit_status_date != null && objAudit.audit_status_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",audit_status_date='" + objAudit.audit_status_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where Plan_Id='" + objAudit.Plan_Id + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);          
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateAuditStatus: " + ex.ToString());
            }
            return false;
        }

        //Raise NC
        internal bool FunAddNonconformity(AuditProcessModels objAudit)
        {
            try
            {
                string user = "";
                user = objGlobaldata.GetCurrentUserSession().empid;

                string sql = "select id_nc from t_audit_process_nc where Plan_Id='"+ Plan_Id + "'";
                DataSet dsData = objGlobaldata.Getdetails(sql);
                int count = dsData.Tables[0].Rows.Count + 1;

                nc_no = Audit_no +" - "+ count;

                string sSqlstmt = "insert into t_audit_process_nc (Plan_Id,Audit_Id,nc_no,finding_details,upload,finding_category,Audit_criteria,audit_clause,description,logged_by";

                string sFields = "", sFieldValue = "";

                if (objAudit.nc_date != null && objAudit.nc_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", nc_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.nc_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + Plan_Id + "','" + Audit_Id + "','" + nc_no + "','" + finding_details + "','" + upload + "','" + finding_category + "','" + Audit_criteria + "','" + audit_clause + "','" + description + "','"+user+"'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_nc = 0;
                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_nc))
                {
                    string sql2 = "update t_audit_process_plan set total_nc='"+count+"' where Plan_Id='"+ Plan_Id + "'";
                    objGlobaldata.ExecuteQuery(sql2);
                    return SendNCmail(id_nc, Plan_Id, "Audit Nonconformity Raised for Approval");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddNonconformity: " + ex.ToString());
            }
            return false;
        }

        //Raise NC Mail
        internal bool SendNCmail(int id_nc, string Plan_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select nc_no,nc_date,finding_details,finding_category,description,auditee_team,logged_by"
                + " from t_audit_process_nc T1,t_audit_process_plan T2 where T1.Plan_Id = T2.Plan_Id and T1.Plan_Id = '" + Plan_Id + "' and id_nc = '" + id_nc + "'";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["auditee_team"].ToString());
                    string sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["auditee_team"].ToString());
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string Ncdate = "";
                    if (dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != null && dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != "")
                    {
                        Ncdate = Convert.ToDateTime(dsAuditList.Tables[0].Rows[0]["nc_date"].ToString()).ToString("dd/MM/yyyy");

                    }

                    sHeader = "<tr><td colspan=3><b>NC No:<b></td> <td colspan=3>"
                          + (dsAuditList.Tables[0].Rows[0]["nc_no"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>NC Date:<b></td> <td colspan=3>" + Ncdate + "</td></tr>"
                          + "<tr><td colspan=3><b>Finding Details:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["finding_details"].ToString() + "</td></tr>"
                          + "<tr><td colspan=3><b>Finding Category:<b></td> <td colspan=3>" + objGlobaldata.GetAuditNCById(dsAuditList.Tables[0].Rows[0]["finding_category"].ToString()) + "</td></tr>"
                          + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["description"].ToString() + "</td></tr>";

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Nonconformity Detail");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCmail: " + ex.ToString());
            }
            return false;
        }

        //NC update
        internal bool FunUpdateNonconformity(AuditProcessModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_nc set finding_details='" + finding_details + "',upload='" + upload + "',finding_category='" + finding_category + "',Audit_criteria='" + Audit_criteria + "',audit_clause='" + audit_clause + "',description='" + description + "'";
                if (objAudit.nc_date != null && objAudit.nc_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",nc_date='" + objAudit.nc_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objAudit.id_nc + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNonconformity: " + ex.ToString());
            }
            return false;
        }


        //NC Auditee Approval
        internal bool FunNonconformityAuditeeApproval(AuditProcessModels objAudit)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string user = "";
                user = objGlobaldata.GetCurrentUserSession().empid;

                string sSqlstmt = "update t_audit_process_nc set apprv_status='" + apprv_status + "',apprvby_auditee='" + user + "',aprvrejct_date='" + TodayDate + "',reason_rejection='" + reason_rejection + "',upload_evidence='" + upload_evidence + "'";
                if (objAudit.corrective_action_date != null && objAudit.corrective_action_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",corrective_action_date='" + objAudit.corrective_action_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objAudit.id_nc + "'";
                objGlobaldata.ExecuteQuery(sSqlstmt);
                return SendNCAuditeemail(id_nc, Plan_Id, apprv_status, "Audit Nonconformity Approval Status");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunNonconformityAuditeeApproval: " + ex.ToString());
            }
            return false;
        }

        //NC Auditee Mail
        internal bool SendNCAuditeemail(string id_nc, string Plan_Id,string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select nc_no,nc_date,finding_details,finding_category,description,auditee_team,logged_by,apprv_status,apprvby_auditee,corrective_action_date,reason_rejection"
                + " from t_audit_process_nc T1,t_audit_process_plan T2 where T1.Plan_Id = T2.Plan_Id and T1.Plan_Id = '" + Plan_Id + "' and id_nc = '" + id_nc + "'";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["auditee_team"].ToString());
                    string Ncdate = "", actdate="";
                    if (dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != null && dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != "")
                    {
                        Ncdate = Convert.ToDateTime(dsAuditList.Tables[0].Rows[0]["nc_date"].ToString()).ToString("dd/MM/yyyy");

                    }
                    if (dsAuditList.Tables[0].Rows[0]["corrective_action_date"].ToString() != null && dsAuditList.Tables[0].Rows[0]["corrective_action_date"].ToString() != "")
                    {
                        actdate = Convert.ToDateTime(dsAuditList.Tables[0].Rows[0]["corrective_action_date"].ToString()).ToString("dd/MM/yyyy");

                    }
                    if (iStatus == "2") // Approve
                    {
                        sHeader = "<tr><td colspan=3><b>NC No:<b></td> <td colspan=3>"
                       + (dsAuditList.Tables[0].Rows[0]["nc_no"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>NC Date:<b></td> <td colspan=3>" + Ncdate + "</td></tr>"
                       + "<tr><td colspan=3><b>Finding Details:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["finding_details"].ToString() + "</td></tr>"
                       + "<tr><td colspan=3><b>Finding Category:<b></td> <td colspan=3>" + objGlobaldata.GetAuditNCById(dsAuditList.Tables[0].Rows[0]["finding_category"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["description"].ToString() + "</td></tr>"
                       + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Approved</td></tr>"
                       + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" +objGlobaldata.GetEmpHrNameById(dsAuditList.Tables[0].Rows[0]["apprvby_auditee"].ToString()) + "</td></tr>"
                       +"<tr><td colspan=3><b>Corrective Action Date:<b></td> <td colspan=3>"+ actdate + "</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>NC No:<b></td> <td colspan=3>"
                      + (dsAuditList.Tables[0].Rows[0]["nc_no"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>NC Date:<b></td> <td colspan=3>" + Ncdate + "</td></tr>"
                      + "<tr><td colspan=3><b>Finding Details:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["finding_details"].ToString() + "</td></tr>"
                      + "<tr><td colspan=3><b>Finding Category:<b></td> <td colspan=3>" + objGlobaldata.GetAuditNCById(dsAuditList.Tables[0].Rows[0]["finding_category"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["description"].ToString() + "</td></tr>"
                      + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Rejected</td></tr>"
                      + "<tr><td colspan=3><b>Rejected By:<b></td> <td colspan=3>" + objGlobaldata.GetEmpHrNameById(dsAuditList.Tables[0].Rows[0]["apprvby_auditee"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Reason for rejection:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["reason_rejection"].ToString() + "</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Nonconformity Approval Detail");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCAuditeemail: " + ex.ToString());
            }
            return false;
        }

        //NC Auditor update
        internal bool FunNonconformityAuditorUpdate(AuditProcessModels objAudit)
        {
            try
            {
                string sSqlstmt = "";
                if (objAudit.followup_date != null && objAudit.followup_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = "update t_audit_process_nc set followup_date = '" + objAudit.followup_date.ToString("yyyy/MM/dd") + "' where id_nc='" + objAudit.id_nc + "'";
                    objGlobaldata.ExecuteQuery(sSqlstmt);
                    return SendNCAuditormail(id_nc, Plan_Id, "Audit Nonconformity Follow up Date");
                }
                return true;      
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunNonconformityAuditorUpdate: " + ex.ToString());
            }
            return false;
        }

        //NC Auditor mail
        internal bool SendNCAuditormail(string id_nc, string Plan_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select nc_no,nc_date,finding_details,finding_category,description,auditee_team,logged_by,apprv_status,apprvby_auditee,corrective_action_date,reason_rejection,followup_date"
                + " from t_audit_process_nc T1,t_audit_process_plan T2 where T1.Plan_Id = T2.Plan_Id and T1.Plan_Id = '" + Plan_Id + "' and id_nc = '" + id_nc + "'";

                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobaldata.GetMultiHrEmpNameById(dsAuditList.Tables[0].Rows[0]["auditee_team"].ToString());
                    string sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["auditee_team"].ToString());
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAuditList.Tables[0].Rows[0]["logged_by"].ToString());
                    string Ncdate = "", followdate = "";
                    if (dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != null && dsAuditList.Tables[0].Rows[0]["nc_date"].ToString() != "")
                    {
                        Ncdate = Convert.ToDateTime(dsAuditList.Tables[0].Rows[0]["nc_date"].ToString()).ToString("dd/MM/yyyy");

                    }
                    if (dsAuditList.Tables[0].Rows[0]["followup_date"].ToString() != null && dsAuditList.Tables[0].Rows[0]["followup_date"].ToString() != "")
                    {
                        followdate = Convert.ToDateTime(dsAuditList.Tables[0].Rows[0]["followup_date"].ToString()).ToString("dd/MM/yyyy");

                    }

                    sHeader = "<tr><td colspan=3><b>NC No:<b></td> <td colspan=3>"
                   + (dsAuditList.Tables[0].Rows[0]["nc_no"].ToString()) + "</td></tr>"
                   + "<tr><td colspan=3><b>NC Date:<b></td> <td colspan=3>" + Ncdate + "</td></tr>"
                   + "<tr><td colspan=3><b>Finding Details:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["finding_details"].ToString() + "</td></tr>"
                   + "<tr><td colspan=3><b>Finding Category:<b></td> <td colspan=3>" + objGlobaldata.GetAuditNCById(dsAuditList.Tables[0].Rows[0]["finding_category"].ToString()) + "</td></tr>"
                   + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + dsAuditList.Tables[0].Rows[0]["description"].ToString() + "</td></tr>"
                   + "<tr><td colspan=3><b>Follow Up Date:<b></td> <td colspan=3>" + followdate + "</td></tr>";
                    

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Nonconformity Detail");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCAuditormail: " + ex.ToString());
            }
            return false;
        }

        //Auditor add
        internal bool FunAddAuditorDetails(AuditProcessModels objAudit, AuditProcessModelsList objModelsList)
        {
            try
            {
               
                string sSqlstmt = "insert into t_auditor_detail (auditor_name,qualification,years_exp,trainings_completed,upload_cetificate)"
                    +" values ('" + auditor_name + "','"+ qualification + "','" + years_exp + "','" + trainings_completed + "','" + upload_cetificate + "')";
                int id_auditor = 0;
                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_auditor))
                {                
                    if (id_auditor > 0)
                    {
                        DataSet dsData = objGlobaldata.GetReportNo("AUDITOR", "", "");
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            auditor_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        objGlobaldata.ExecuteQuery("Update t_auditor_detail set auditor_no = '" + auditor_no + "' where id_auditor= '" + id_auditor + "'");


                        objModelsList.Obj[0].id_auditor = id_auditor.ToString();
                        return  FunAddAuditorCertList(objModelsList);
                    
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditorDetails: " + ex.ToString());
            }
            return false;
        }
        //certificate list
        internal bool FunAddAuditorCertList(AuditProcessModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_auditor_detail_certificate where id_auditor='" + objModelsList.Obj[0].id_auditor + "'; ";

                for (int i = 0; i < objModelsList.Obj.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_auditor_detail_certificate(id_auditor,standards,type_course,yearsexp,upload";

                    string sFieldValue = "", sFields = "";
                    if (objModelsList.Obj[i].completed_on != null && objModelsList.Obj[i].completed_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", completed_on";
                        sFieldValue = sFieldValue + ", '" + objModelsList.Obj[i].completed_on.ToString("yyyy/MM/dd") + "'";
                    }
                   
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModelsList.Obj[0].id_auditor + "','" + objModelsList.Obj[i].standards + "','" + objModelsList.Obj[i].type_course + "','" + objModelsList.Obj[i].yearsexp + "','" + objModelsList.Obj[i].upload + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditorCertList: " + ex.ToString());
            }
            return false;
        }
        //Auditor update
        internal bool FunUpdateAuditorDetails(AuditProcessModels objAudit, AuditProcessModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "update t_auditor_detail set auditor_name='" + auditor_name + "',qualification='" + qualification + "',years_exp='" + years_exp + "',trainings_completed='" + trainings_completed + "',upload_cetificate='" + upload_cetificate + "'";
              
                sSqlstmt = sSqlstmt + " where id_auditor='" + objAudit.id_auditor + "'";
                if(objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objModelsList.Obj.Count) > 0)
                    {
                        objModelsList.Obj[0].id_auditor = id_auditor.ToString();
                         return FunAddAuditorCertList(objModelsList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateAuditorDetails: " + ex.ToString());
            }
            return false;
        }

        //Delete auditor
        internal bool FunDeleteAuditor(string sid_auditor)
        {
            try
            {
                string sSqlstmt = "update t_auditor_detail set active=0 where id_auditor='" + sid_auditor + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteAuditor: " + ex.ToString());
            }
            return false;
        }

        //Auditor Availability
        internal bool FunAddAuditorAvailability(AuditProcessModelsList objModelsList)
        {
            try
            {

                if (Convert.ToInt32(objModelsList.Obj.Count) > 0)
                {
                    objModelsList.Obj[0].id_auditor = id_auditor.ToString();
                    return  FunAddAuditorAvailabilityList(objModelsList);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditorAvailability: " + ex.ToString());
            }
            return false;
        }

        //Auditor availability
        internal bool FunAddAuditorAvailabilityList(AuditProcessModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_auditor_availability where id_auditor='" + objModelsList.Obj[0].id_auditor + "'; ";

                for (int i = 0; i < objModelsList.Obj.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_auditor_availability(id_auditor,comments";

                    string sFieldValue = "", sFields = "";
                    if (objModelsList.Obj[i].from_date != null && objModelsList.Obj[i].from_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", from_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.Obj[i].from_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objModelsList.Obj[i].to_date != null && objModelsList.Obj[i].to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", to_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.Obj[i].to_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModelsList.Obj[0].id_auditor + "','" + objModelsList.Obj[i].comments + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddAuditorAvailabilityList: " + ex.ToString());
            }
            return false;
        }
        //------------------------------------------
        internal bool FunApproveOrRejectDeptHead(string sid_audit_schedule, string status)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                if (status == "2")
                {
                    FunApproveOrReject(sid_audit_schedule, status);
                }
                string sSqlstmt = "update t_audit_process set  dept_head_status='" + status + "' ,dep_head_status_date ='" + sApprovedDate + "' where Audit_Id='" + sid_audit_schedule + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunApproveOrRejectDeptHead: " + ex.ToString());
            }
            return false;
        }

        internal bool FunApproveOrRejectAuditTeam(string sid_audit_schedule, string status)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                if (status == "2")
                {
                    FunApproveOrReject(sid_audit_schedule, status);
                }
                string sSqlstmt = "update t_audit_process set  audit_team_status='" + status + "' ,audit_team_status_date ='" + sApprovedDate + "' where Audit_Id='" + sid_audit_schedule + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunApproveOrRejectAuditTeam: " + ex.ToString());
            }
            return false;
        }
     
        internal bool FunApproveOrReject(string sid_audit_schedule, string status)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                if (status == "0")
                {
                    status = "Pending";
                }
                else if (status == "1")
                {
                    status = "Approved";
                }
                else if (status == "2")
                {
                    status = "Rejected";
                }
                string sSqlstmt = "update t_audit_process set  Audit_Status='" + status + "', audit_status_date='" + sApprovedDate + "' where Audit_Id='" + sid_audit_schedule + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunApproveOrRejectAudit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteAuditProcess(string sid_audit_schedule)
        {
            try
            {
                string sSqlstmt = "update t_audit_process set active=0 where Audit_Id='" + sid_audit_schedule + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteAuditProcess: " + ex.ToString());
            }
            return false;
        }

        internal bool FunResceduleAuditProcess(string sid_audit_schedule, string Rescedule_Reason)
        {
            try
            {
                string sSqlstmt = "update t_audit_process set Reasons_Reschedule='" + Rescedule_Reason + "',Audit_Status='Rescheduled' where Audit_Id='" + sid_audit_schedule + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteAuditProcess: " + ex.ToString());
            }
            return false;
        }

        //nc_step_status =1
        internal bool FunUpdateNC(AuditProcessModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_perform set why_nc ='" + objAudit.why_nc + "', nc_step_status=1, nc_status='Open'";

                sSqlstmt = sSqlstmt + "where id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    string Sqlstmt = "Select Audit_no,Non_comformance,PlannedBy,internal_audit_team,dept_head,division,dept,location,AuditDate,Notified_To,why_nc,auditee_team from t_audit_process a,t_audit_process_perform b " +
                        "where a.Audit_Id=b.Audit_Id  and b.id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";
                    DataSet dsAudit = objGlobaldata.Getdetails(Sqlstmt);
                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {

                        string sType = "NonConfirmity";
                        string sName = "All";
                        string sToEmailIds = "";
                        string sHeader = "", sSubject = "", sContent = "", sCCEmailIds = "";

                        DataSet dsEmailXML = new DataSet();
                        dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                        if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                        {
                            sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                        }

                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                        {
                            sContent = reader.ReadToEnd();
                        }
                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) != "")
                        {
                            sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + ",";
                        }
                        //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) != "")
                        //{
                        //    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + ",";
                        //}
                        //sToEmailIds = sToEmailIds.TrimEnd(',');
                        //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) != "")
                        //{
                        //    sToEmailIds = sToEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + ",";
                        //}
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) != "")
                        {
                            sToEmailIds = sToEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + ",";
                        }


                        sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sToEmailIds = sToEmailIds.Trim();
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        sToEmailIds = sToEmailIds.TrimStart(',');

                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) != "")
                        {
                            sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString());
                        }
                        sCCEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sCCEmailIds = sCCEmailIds.Trim();
                        sCCEmailIds = sCCEmailIds.TrimEnd(',');
                        sCCEmailIds = sCCEmailIds.TrimStart(',');

                        sHeader = "<tr><td colspan=3><b>Audit Number:<b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["Audit_no"].ToString() + "</td></tr>"
                            + "<tr><td colspan=3><b>Audit Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                            + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsAudit.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Department Head:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Audit Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Notified To:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Auditee Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Planned By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><h3>Remarks:</h3></td> <td colspan=3><h3>" + dsAudit.Tables[0].Rows[0]["why_nc"].ToString() + "</h3></td></tr>";


                        sContent = sContent.Replace("{FromMsg}", "");
                        sContent = sContent.Replace("{UserName}", sName);
                        sContent = sContent.Replace("{Title}", "Non Confirmity Details");
                        sContent = sContent.Replace("{content}", sHeader);
                        sContent = sContent.Replace("{message}", "");
                        sContent = sContent.Replace("{extramessage}", "");

                        sToEmailIds = sToEmailIds.Trim(',');

                        objGlobaldata.Sendmail2(sToEmailIds, sSubject, sContent, sCCEmailIds, "");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNC: " + ex.ToString());
            }
            return false;
        }

        //nc_step_status=2
        internal bool FunUpdateNC1(AuditProcessModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_audit_process_perform set auditee_acceptance ='" + objAudit.auditee_acceptance + "', nc_step_status=2 ,nc_reject_reason ='" + objAudit.nc_reject_reason + "', nc_reject_upload = '" + objAudit.nc_reject_upload + "'";

                if (objAudit.ca_date != null && objAudit.ca_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_date='" + objAudit.ca_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + "where id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    string Sqlstmt = "Select Audit_no,Non_comformance,PlannedBy,internal_audit_team,dept_head,division,dept,location," +
                        "AuditDate,Notified_To,why_nc,auditee_acceptance,ca_date,auditee_team from t_audit_process a,t_audit_process_perform b " +
                        "where a.Audit_Id=b.Audit_Id  and b.id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";
                    DataSet dsAudit = objGlobaldata.Getdetails(Sqlstmt);
                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {

                        string sType = "NonConfirmity";
                        string sName = "All";
                        string sToEmailIds = "";
                        string sHeader = "", sSubject = "", sContent = "", sCCEmailIds = "";

                        DataSet dsEmailXML = new DataSet();
                        dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                        if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                        {
                            sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                        }

                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                        {
                            sContent = reader.ReadToEnd();
                        }


                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) != "")
                        {
                            sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString());
                        }

                        sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sToEmailIds = sToEmailIds.Trim();
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        sToEmailIds = sToEmailIds.TrimStart(',');

                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) != "")
                        {
                            sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + ",";
                        }
                        //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) != "")
                        //{
                        //    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + ",";
                        //}
                        //sCCEmailIds = sCCEmailIds.TrimEnd(',');
                        //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) != "")
                        //{
                        //    sCCEmailIds = sCCEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + ",";
                        //}
                        sCCEmailIds = sCCEmailIds.TrimEnd(',');
                        if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) != "")
                        {
                            sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + ",";
                        }
                        sCCEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sCCEmailIds = sCCEmailIds.Trim();
                        sCCEmailIds = sCCEmailIds.TrimEnd(',');
                        sCCEmailIds = sCCEmailIds.TrimStart(',');

                        sHeader = "<tr><td colspan=3><b>Audit Number:<b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["Audit_no"].ToString() + "</td></tr>"
                            + "<tr><td colspan=3><b>Audit Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                            + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsAudit.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Department Head:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Audit Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Notified To:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Planned By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Auditee Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Remarks:</b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["why_nc"].ToString() + "</td></tr>"
                            + "<tr><td colspan=3><h3>Auditee Acceptance:</h3></td> <td colspan=3><h3>" + dsAudit.Tables[0].Rows[0]["auditee_acceptance"].ToString() + "</h3></td></tr>"
                            + "<tr><td colspan=3><h3>Corrective Action Date:</h3></td> <td colspan=3><h3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["ca_date"].ToString()).ToString("dd/MM/yyyy") + "</h3></td></tr>";

                        sContent = sContent.Replace("{FromMsg}", "");
                        sContent = sContent.Replace("{UserName}", sName);
                        sContent = sContent.Replace("{Title}", "Non Confirmity Details");
                        sContent = sContent.Replace("{content}", sHeader);
                        sContent = sContent.Replace("{message}", "");
                        sContent = sContent.Replace("{extramessage}", "");

                        sToEmailIds = sToEmailIds.Trim(',');

                        objGlobaldata.Sendmail2(sToEmailIds, sSubject, sContent, sCCEmailIds, "");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNC1: " + ex.ToString());
            }
            return false;
        }

        //nc_step_status=3
        internal bool FunUpdateNC2(AuditProcessModels objAudit)
        {
            try
            {
                string sSqlstmt = "";
                if (objAudit.followup_date != null && objAudit.followup_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = "update t_audit_process_perform set followup_date ='" + objAudit.followup_date.ToString("yyyy/MM/dd") + "', nc_step_status=3,nc_reject_response ='" + objAudit.nc_reject_response + "' ,nc_status ='" + objAudit.nc_status + "' where id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";
                }
                if (sSqlstmt != "")
                {
                    if (objGlobaldata.ExecuteQuery(sSqlstmt))
                    {
                        string Sqlstmt = "Select Audit_no,Non_comformance,PlannedBy,internal_audit_team,dept_head,division,dept,location,AuditDate," +
                                "Notified_To,why_nc,auditee_acceptance,ca_date,followup_date,auditee_team from t_audit_process a,t_audit_process_perform b " +
                                "where a.Audit_Id=b.Audit_Id  and b.id_audit_process_perform='" + objAudit.id_audit_process_perform + "'";
                        DataSet dsAudit = objGlobaldata.Getdetails(Sqlstmt);
                        if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                        {

                            string sType = "NonConfirmity";
                            string sName = "All";
                            string sToEmailIds = "";
                            string sHeader = "", sSubject = "", sContent = "", sCCEmailIds = "";

                            DataSet dsEmailXML = new DataSet();
                            dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                            if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                            {
                                sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                            }

                            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                            {
                                sContent = reader.ReadToEnd();
                            }
                            if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) != "")
                            {
                                sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + ",";
                            }
                            //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) != "")
                            //{
                            //    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + ",";
                            //}
                            //sToEmailIds = sToEmailIds.TrimEnd(',');
                            //if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) != "")
                            //{
                            //    sToEmailIds = sToEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + ",";
                            //}
                            sToEmailIds = sToEmailIds.TrimEnd(',');
                            if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) != "")
                            {
                                sToEmailIds = sToEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + ",";
                            }


                            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                            sToEmailIds = sToEmailIds.Trim();
                            sToEmailIds = sToEmailIds.TrimEnd(',');
                            sToEmailIds = sToEmailIds.TrimStart(',');

                            if (objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) != "")
                            {
                                sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString());
                            }

                            sCCEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                            sCCEmailIds = sCCEmailIds.Trim();
                            sCCEmailIds = sCCEmailIds.TrimEnd(',');
                            sCCEmailIds = sCCEmailIds.TrimStart(',');

                            sHeader = "<tr><td colspan=3><b>Audit Number:<b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["Audit_no"].ToString() + "</td></tr>"
                                + "<tr><td colspan=3><b>Audit Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["AuditDate"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                                + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsAudit.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsAudit.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsAudit.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Department Head:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["dept_head"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Audit Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["internal_audit_team"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Notified To:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["Notified_To"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Planned By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["PlannedBy"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Auditee Team:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsAudit.Tables[0].Rows[0]["auditee_team"].ToString()) + "</td></tr>"
                                + "<tr><td colspan=3><b>Remarks:</b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["why_nc"].ToString() + "</td></tr>"
                                + "<tr><td colspan=3><b>Auditee Acceptance:</b></td> <td colspan=3>" + dsAudit.Tables[0].Rows[0]["auditee_acceptance"].ToString() + "</td></tr>"
                                + "<tr><td colspan=3><b>Corrective Action Date:</b></td> <td colspan=3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["ca_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                                + "<tr><td colspan=3><h3>Follow Up Date:</h3></td> <td colspan=3><h3>" + Convert.ToDateTime(dsAudit.Tables[0].Rows[0]["followup_date"].ToString()).ToString("dd/MM/yyyy") + "</h3></td></tr>";


                            sContent = sContent.Replace("{FromMsg}", "");
                            sContent = sContent.Replace("{UserName}", sName);
                            sContent = sContent.Replace("{Title}", "Non Confirmity Details");
                            sContent = sContent.Replace("{content}", sHeader);
                            sContent = sContent.Replace("{message}", "");
                            sContent = sContent.Replace("{extramessage}", "");

                            sToEmailIds = sToEmailIds.Trim(',');

                            objGlobaldata.Sendmail2(sToEmailIds, sSubject, sContent, sCCEmailIds, "");
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNC2: " + ex.ToString());
            }
            return true;
        }

    }

    public class AuditProcessModelsList
    {
        public List<AuditProcessModels> Obj { get; set; }
    }

    public class AuditProcessPerformModels
    {
        clsGlobal objGlobalData = new clsGlobal();      
     
        public string id_audit_process_perform { get; set; }     
      
        public string Audit_Elements { get; set; }     

        public string Details { get; set; }       
     
        public string Evidence { get; set; }      
    
        public string Category { get; set; }     
    
        public string Conformance { get; set; }

        public string Non_comformance { get; set; }

        public string evidence_upload { get; set; }

        public string Major_Non_Conformances { get; set; }
   
        public string TOtal_Conformances { get; set; }
                          
        public string Note_Worth_Findings { get; set; }
     
        public string Minor_Non_Conformances { get; set; }

        public string Potential_Non_Conformances { get; set; }

        public string Audit_Id { get; set; }

        internal bool FunAddAuditPerformance(AuditProcessPerformModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "delete from t_audit_process_perform where Audit_Id='"
                        + objAudit.lstAudit[0].Audit_Id + "'; ";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_audit_process_perform (Audit_Id, Audit_Elements, Details,Evidence,Category,Conformance,Non_comformance,evidence_upload,Major_Non_Conformances,TOtal_Conformances,Minor_Non_Conformances,Note_Worth_Findings,Potential_Non_Conformances"
                        + ") values('" + objAudit.lstAudit[0].Audit_Id + "','" + objAudit.lstAudit[i].Audit_Elements
                                                + "','" + objAudit.lstAudit[i].Details + "','" + objAudit.lstAudit[i].Evidence + "','" + objAudit.lstAudit[i].Category 
                        +"','" + objAudit.lstAudit[i].Conformance + "','" + objAudit.lstAudit[i].Non_comformance + "','" + objAudit.lstAudit[i].evidence_upload
                        + "','" + objAudit.lstAudit[i].Major_Non_Conformances + "','" + objAudit.lstAudit[i].TOtal_Conformances + "','" + objAudit.lstAudit[i].Minor_Non_Conformances + "','" + objAudit.lstAudit[i].Potential_Non_Conformances


                       + "','" + objAudit.lstAudit[i].Note_Worth_Findings + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditPerformance: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditPerformance(AuditProcessPerformModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_audit_process_perform set Audit_Elements='" + objAudit.lstAudit[i].Audit_Elements + "',"
                            + "Details='" + objAudit.lstAudit[i].Details + "',Evidence='" + objAudit.lstAudit[i].Evidence
                            + "',Category='" + objAudit.lstAudit[i].Category + "',Conformance='" + objAudit.lstAudit[i].Conformance
                            + "',Non_comformance='" + objAudit.lstAudit[i].Non_comformance + "',Major_Non_Conformances='" + objAudit.lstAudit[i].Major_Non_Conformances
                            + "',TOtal_Conformances='" + objAudit.lstAudit[i].TOtal_Conformances + "',Minor_Non_Conformances='" + objAudit.lstAudit[i].Minor_Non_Conformances
                            + "',Note_Worth_Findings='" + objAudit.lstAudit[i].Note_Worth_Findings + "',Potential_Non_Conformances='" + objAudit.lstAudit[i].Potential_Non_Conformances + "'";
                        if (objAudit.lstAudit[i].evidence_upload != null)
                        {
                            sSqlstmt = sSqlstmt + ", evidence_upload='" + objAudit.lstAudit[i].evidence_upload + "' ";
                        }
                        sSqlstmt = sSqlstmt + " where id_audit_process_perform='" + objAudit.lstAudit[i].id_audit_process_perform + "';";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditPerformance: " + ex.ToString());
            }
            return false;
        }
    }


   
    public class AuditProcessPerformModelsList
    {
        public List<AuditProcessPerformModels> lstAudit { get; set; }
    }
}