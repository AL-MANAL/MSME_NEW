using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class AuditScheduleModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_audit_schedule { get; set; }

        [Display(Name = "Typeof Audit")]
        public string audit_type { get; set; }

        [Display(Name = "Audit Number")]
        public string audit_no { get; set; }

        [Display(Name = "Entity Type")]
        public string entity_type { get; set; }

        [Display(Name = "Entity Number")]
        public string entity_no { get; set; }

        [Display(Name = "Audit Scope")]
        public string audit_scope { get; set; }

        [Display(Name = "Audit Criticallty")]
        public string audit_criticallty { get; set; }

        [Display(Name = "Audit Criteria")]
        public string audit_criteria { get; set; }

        [Display(Name = "Contractual Agreement")]
        public string contractual_agreement { get; set; }

        [Display(Name = "Audit Objectives")]
        public string purpose { get; set; }

        [Display(Name = "Access Requirements")]
        public string site_access { get; set; }

        [Display(Name = "Scheduled By")]
        public string schedule_by { get; set; }

        [Display(Name = "Final schedule to be Approved By")]
        public string approvedby { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }
      
        //t_audit_schedule_trans

        [Display(Name = "Id")]
        public string id_audit_schedule_trans { get; set; }

        [Display(Name = "Auditor Name")]
        public string auditor_name { get; set; }

        [Display(Name = "Department to be audited")]
        public string audit_dept { get; set; }

        [Display(Name = "Audit Location")]
        public string audit_loc { get; set; }

        [Display(Name = "Focused Areas")]
        public string focused_area { get; set; }

        [Display(Name = "Scheduled Date and Time")]
        public DateTime scheduled_time_date { get; set; }

        [Display(Name = "Recommended Checklist")]
        public string checklist_id { get; set; }

        [Display(Name = "Auditor Availability")]
        public string auditor_availability { get; set; }

        [Display(Name = "If not available now, next avaialbe date")]
        public string next_availability { get; set; }

        [Display(Name = "PlanTimeInHour")]
        public string PlanTimeInHour { get; set; }


        internal bool FunAddAuditSchedule(AuditScheduleModels objAudit, AuditScheduleModelsList objAuditList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_audit_schedule (audit_type,entity_type,entity_no,audit_scope,audit_criticallty," +
                    "audit_criteria,contractual_agreement,purpose,site_access,schedule_by,approvedby,loggedby,branch";
                //string sFields = "", sFieldValue = "";
                //if (objAudit.acc_date != null && objAudit.acc_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                //{
                //    sFields = sFields + ", acc_date";
                //    sFieldValue = sFieldValue + ", '" + objAudit.acc_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                //}
               
                //sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objAudit.audit_type + "','" + objAudit.entity_type + "'"
                    + ",'" + objAudit.entity_no + "','" + objAudit.audit_scope + "','" + objAudit.audit_criticallty + "','" + objAudit.audit_criteria
                    + "','" + objAudit.contractual_agreement + "','" + objAudit.purpose + "','" + objAudit.site_access + "','" + objAudit.schedule_by 
                    + "','" + objAudit.approvedby + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objAudit.branch + "')";
              //  sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_audit_schedule = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_audit_schedule))
                {
                    if (id_audit_schedule > 0 && Convert.ToInt32(objAuditList.ScheculeList.Count) > 0)
                    {
                        objAuditList.ScheculeList[0].id_audit_schedule = id_audit_schedule.ToString();
                        FunAddAuditTeamDetailswithEmail(objAudit,objAuditList);
                    }

                    if (id_audit_schedule > 0)
                    {

                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("SCH", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            audit_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_audit_schedule set audit_no='" + audit_no + "' where id_audit_schedule = '" + id_audit_schedule + "'";
                        return (objGlobalData.ExecuteQuery(sql));
                        //{
                        //    SendAuditScheduleEmail(objAudit, "Planning or Scheduling the Audits");
                        //}
                        //return true;
                    }                    
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditSchedule: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAuditTeamDetails(AuditScheduleModelsList objAuditList)
        {
            try
            {
               
                string sSqlstmt = "delete from t_audit_schedule_trans where id_audit_schedule='" + objAuditList.ScheculeList[0].id_audit_schedule + "'; ";

                for (int i = 0; i < objAuditList.ScheculeList.Count; i++)
                {
                    string sid_audit_schedule_trans = "null";
                    if (objAuditList.ScheculeList[i].id_audit_schedule_trans != null)
                    {
                        sid_audit_schedule_trans = objAuditList.ScheculeList[i].id_audit_schedule_trans;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_audit_schedule_trans (id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area," +
                        "checklist_id,auditor_availability,next_availability";
                    string sFields = "", sFieldValue = "", sFieldValues = "";

                    if (objAuditList.ScheculeList[i].scheduled_time_date != null && objAuditList.ScheculeList[i].scheduled_time_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", scheduled_time_date";
                        sFieldValue = sFieldValue + ", '" + objAuditList.ScheculeList[i].scheduled_time_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                        sFieldValues = sFieldValues + ", scheduled_time_date = values(scheduled_time_date)";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_audit_schedule_trans + ",'" + objAuditList.ScheculeList[0].id_audit_schedule
                    + "','" + objAuditList.ScheculeList[i].auditor_name + "','" + objAuditList.ScheculeList[i].audit_dept + "','" + objAuditList.ScheculeList[i].audit_loc + "','" + objAuditList.ScheculeList[i].focused_area
                    + "','" + objAuditList.ScheculeList[i].checklist_id + "','" + objAuditList.ScheculeList[i].auditor_availability + "','" + objAuditList.ScheculeList[i].next_availability + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sSqlstmt = sSqlstmt + " ON DUPLICATE KEY UPDATE "
                     + " id_audit_schedule_trans= values(id_audit_schedule_trans), id_audit_schedule= values(id_audit_schedule), auditor_name = values(auditor_name), " +
                     "audit_dept= values(audit_dept), audit_loc= values(audit_loc), focused_area= values(focused_area), checklist_id = values(checklist_id), auditor_availability = values(auditor_availability), next_availability = values(next_availability)";
                    sSqlstmt = sSqlstmt + sFieldValues+ " ;";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditTeamDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAuditTeamDetailswithEmail(AuditScheduleModels objAudit,AuditScheduleModelsList objAuditList)
        {
            try
            {

                string sSqlstmt = "delete from t_audit_schedule_trans where id_audit_schedule='" + objAuditList.ScheculeList[0].id_audit_schedule + "'; ";

                for (int i = 0; i < objAuditList.ScheculeList.Count; i++)
                {
                    string sid_audit_schedule_trans = "null";
                    if (objAuditList.ScheculeList[i].id_audit_schedule_trans != null)
                    {
                        sid_audit_schedule_trans = objAuditList.ScheculeList[i].id_audit_schedule_trans;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_audit_schedule_trans (id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area," +
                        "checklist_id,auditor_availability,next_availability";
                    string sFields = "", sFieldValue = "", sFieldValues = "";

                    if (objAuditList.ScheculeList[i].scheduled_time_date != null && objAuditList.ScheculeList[i].scheduled_time_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", scheduled_time_date";
                        sFieldValue = sFieldValue + ", '" + objAuditList.ScheculeList[i].scheduled_time_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                        sFieldValues = sFieldValues + ", scheduled_time_date = values(scheduled_time_date)";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_audit_schedule_trans + ",'" + objAuditList.ScheculeList[0].id_audit_schedule
                    + "','" + objAuditList.ScheculeList[i].auditor_name + "','" + objAuditList.ScheculeList[i].audit_dept + "','" + objAuditList.ScheculeList[i].audit_loc + "','" + objAuditList.ScheculeList[i].focused_area
                    + "','" + objAuditList.ScheculeList[i].checklist_id + "','" + objAuditList.ScheculeList[i].auditor_availability + "','" + objAuditList.ScheculeList[i].next_availability + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sSqlstmt = sSqlstmt + " ON DUPLICATE KEY UPDATE "
                     + " id_audit_schedule_trans= values(id_audit_schedule_trans), id_audit_schedule= values(id_audit_schedule), auditor_name = values(auditor_name), " +
                     "audit_dept= values(audit_dept), audit_loc= values(audit_loc), focused_area= values(focused_area), checklist_id = values(checklist_id), auditor_availability = values(auditor_availability), next_availability = values(next_availability)";
                    sSqlstmt = sSqlstmt + sFieldValues + " ;";
                    
                }
         
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    for (int j = 0; j < objAuditList.ScheculeList.Count; j++)
                    {
                        SendAuditScheduleEmail(objAudit,objAuditList, j);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditTeamDetailswithEmail: " + ex.ToString());
            }
            return false;
        }
       
        internal bool SendAuditScheduleEmail(AuditScheduleModels objAudit,AuditScheduleModelsList objAuditList, int i)
        {
            try
            {
                string sType = "SheduleAudit";
                string sid_audit_schedule = objAuditList.ScheculeList[0].id_audit_schedule;

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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(objAudit.approvedby.ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(objAudit.approvedby.ToString());
                    }
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(objAudit.schedule_by.ToString());

                    sHeader = "<tr><td colspan=3><b>Audit Type:<b></td> <td colspan=3>"
                        + objGlobalData.GetDropdownitemById(objAudit.audit_type.ToString()) + " </td></tr>"
                        //+ "<tr><td colspan=3><b>Audit Number:<b></td> <td colspan=3>" + (objAudit.audit_no.ToString()) + " </td></tr>"
                        + "<tr><td colspan=3><b>Audit Criticality:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(objAudit.audit_criticallty.ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Audit Criteria:<b></td> <td colspan=3>" + objGlobalData.GetIsoStdDescriptionById(objAudit.audit_criteria.ToString()) + " </td></tr>";

                 
                            sInformation = "<tr> "
                             + "<td colspan=6><label><b>Audit Team Details</b></label> </td> </tr>"
                             + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                             + "<th>Auditor Name</th>"
                             + "<th>Audit Department</th>"
                             + "<th>Audit Location</th>"
                             + "<th>Focused Area</th>"
                             + "<th>Scheduled Date and Time</th>"
                             //+ "<th>Recommanded Checklist</th>"
                             + "</tr>";

                            sInformation = sInformation + "<tr>"
                                + " <td>" + objGlobalData.GetMultiHrEmpNameById(objAuditList.ScheculeList[i].auditor_name.ToString()) + "</td>"
                                + " <td>" + objGlobalData.GetMultiDeptNameById(objAuditList.ScheculeList[i].audit_dept.ToString()) + "</td>"
                                + "<td> " + objGlobalData.GetDivisionLocationById(objAuditList.ScheculeList[i].audit_loc.ToString()) + " </td>"
                                 + " <td>" + objAuditList.ScheculeList[i].focused_area.ToString() + "</td>"
                                 + "<td> " + Convert.ToDateTime(objAuditList.ScheculeList[i].scheduled_time_date.ToString()).ToString("dd/MM/yyyy HH:mm:ss") + " </td>"
                                //+ "<td> " + objGlobalData.GetChecklistDeptBychecklistId(objAuditList.ScheculeList[i].checklist_id.ToString()) + " </td>"
                                + " </tr>";
                            if (objGlobalData.GetMultiHrEmpEmailIdById(objAuditList.ScheculeList[i].auditor_name.ToString()) != "")
                            {
                                sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(objAuditList.ScheculeList[i].auditor_name.ToString()) + ",";
                            }
                        
                        sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sToEmailIds = sToEmailIds.Trim();
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        sToEmailIds = sToEmailIds.TrimStart(',');
                   
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Schedule Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');

                    sCCEmailIds = sCCEmailIds.Trim(',');

                    objGlobalData.Sendmail(sToEmailIds, sSubject + "Planning or Scheduling the Audits", sContent, aAttachment, sCCEmailIds, "");
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendAuditScheduleEmail: " + ex.ToString());
            }
            return false;
        }

        //internal bool SendAuditScheduleEmail(int id_audit_schedule, string sMessage = "")
        //{
        //    try
        //    {
        //        string sType = "SheduleAudit";
        //        AuditScheduleModels ObjSchedule = new AuditScheduleModels();
        //        string sid_audit_schedule = Convert.ToString(id_audit_schedule);

        //        string sSqlstmt = "select id_audit_schedule,audit_type,audit_no,entity_type,entity_no,audit_scope,audit_criticallty,audit_criteria," +
        //            "contractual_agreement,purpose,site_access,schedule_by,approvedby from t_audit_schedule where id_audit_schedule ='" + sid_audit_schedule + "'";

        //        DataSet dsSchedulingList = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsSchedulingList.Tables.Count > 0 && dsSchedulingList.Tables[0].Rows.Count > 0)
        //        {                    
        //            string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

        //            DataSet dsEmailXML = new DataSet();
        //            dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

        //            if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
        //            {
        //                sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
        //            }

        //            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
        //            {
        //                sContent = reader.ReadToEnd();
        //            }
        //            string sName = "All";
        //            string sToEmailIds = "";
        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsSchedulingList.Tables[0].Rows[0]["approvedby"].ToString()) != "")
        //            {
        //                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsSchedulingList.Tables[0].Rows[0]["approvedby"].ToString());
        //            }
        //            sToEmailIds = sToEmailIds.Trim();
        //            sToEmailIds = sToEmailIds.TrimEnd(',');
        //            sToEmailIds = sToEmailIds.TrimStart(',');
        //            string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsSchedulingList.Tables[0].Rows[0]["schedule_by"].ToString());
        //            //aAttachment = HttpContext.Current.Server.MapPath(dsSchedulingList.Tables[0].Rows[0]["Attendees"].ToString());


        //            sHeader = "<tr><td colspan=3><b>Audit Type:<b></td> <td colspan=3>"
        //                + (dsSchedulingList.Tables[0].Rows[0]["audit_type"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Audit Number:<b></td> <td colspan=3>" + (dsSchedulingList.Tables[0].Rows[0]["audit_no"].ToString()) + "</td></tr>"
        //               + "<tr><td colspan=3><b>Entity Type:<b></td> <td colspan=3>" + objGlobalData.GetScheduleEntityTypeById(dsSchedulingList.Tables[0].Rows[0]["entity_type"].ToString()) + "</td></tr>"

        //               + "<tr><td colspan=3><b>Entity Number:<b></td> <td colspan=3>" + objGlobalData.GetScheduleEntityNoById(dsSchedulingList.Tables[0].Rows[0]["entity_no"].ToString())
        //               + "</td></tr>"
        //            + "<tr><td colspan=3><b>Audit Criticality:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsSchedulingList.Tables[0].Rows[0]["audit_criticallty"].ToString()) + "</td></tr>"
        //            + "<tr><td colspan=3><b>Audit Criteria:<b></td> <td colspan=3>" + objGlobalData.GetIsoStdDescriptionById(dsSchedulingList.Tables[0].Rows[0]["audit_criteria"].ToString()) + "</td></tr>";

        //            //if (File.Exists(aAttachment))
        //            //{
        //            //    sHeader = sHeader + "<tr><td colspan=3><b>Attendance Sheet:<b></td> <td colspan=3>Please find the attachment</td></tr>";
        //            //}

        //            sSqlstmt = "select id_audit_schedule_trans,id_audit_schedule,auditor_name,audit_dept,audit_loc,focused_area,scheduled_time_date,checklist_id," +
        //                "auditor_availability,next_availability from t_audit_schedule_trans where id_audit_schedule='" + sid_audit_schedule + "'";

        //            DataSet dsItems = new DataSet();
        //            dsItems = objGlobalData.Getdetails(sSqlstmt);

        //            if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
        //                {
        //                   sInformation = "<tr> "
        //                    + "<td colspan=6><label><b>Audit Team Details</b></label> </td> </tr>"
        //                    + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"                           
        //                    + "<th>Auditor Name</th>"
        //                    + "<th>Audit Department</th>"
        //                    + "<th>Audit Location</th>"
        //                    + "<th>Focused Area</th>"
        //                    + "<th>Scheduled Date and Time</th>"
        //                    + "<th>Recommanded Checklist</th>"                           
        //                    + "</tr>";

        //                    sInformation = sInformation + "<tr>"                               
        //                        + " <td>" + objGlobalData.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["auditor_name"].ToString()) + "</td>"
        //                        + " <td>" + objGlobalData.GetDeptNameById(dsItems.Tables[0].Rows[i]["audit_dept"].ToString())+ "</td>"
        //                        + "<td> " + objGlobalData.GetMultiWorkLocationById(dsItems.Tables[0].Rows[i]["audit_loc"].ToString()) + " </td>"
        //                         + " <td>" + dsItems.Tables[0].Rows[i]["focused_area"].ToString() + "</td>"
        //                         + "<td> " + Convert.ToDateTime(dsItems.Tables[0].Rows[i]["scheduled_time_date"].ToString()).ToString("dd/MM/yyyy HH:mm:ss") + " </td>"
        //                        + "<td> " + objGlobalData.GetChecklistDeptBychecklistId(dsItems.Tables[0].Rows[i]["checklist_id"].ToString()) + " </td>"                               
        //                        + " </tr>";
        //                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["auditor_name"].ToString()) != "")
        //                    {
        //                        sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["auditor_name"].ToString()) + ",";
        //                    }

        //                }
        //                sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
        //                sToEmailIds = sToEmailIds.Trim();
        //                sToEmailIds = sToEmailIds.TrimEnd(',');
        //                sToEmailIds = sToEmailIds.TrimStart(',');
        //            }
        //            sContent = sContent.Replace("{FromMsg}", "");
        //            sContent = sContent.Replace("{UserName}", sName);
        //            sContent = sContent.Replace("{Title}", "Schedule Details");
        //            sContent = sContent.Replace("{content}", sHeader + sInformation);
        //            sContent = sContent.Replace("{message}", "");
        //            sContent = sContent.Replace("{extramessage}", "");

        //            sToEmailIds = sToEmailIds.Trim(',');

        //            sCCEmailIds = sCCEmailIds.Trim(',');

        //            objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendAuditScheduleEmail: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunUpdateAuditSchedule(AuditScheduleModels objAudit, AuditScheduleModelsList objAuditList)
        {
            try
            {
                string sSqlstmt = "update t_audit_schedule set audit_type ='" + objAudit.audit_type + "', entity_type='" + objAudit.entity_type + "', "
                    + "entity_no='" + objAudit.entity_no + "',audit_scope='" + objAudit.audit_scope + "',audit_criticallty='" + objAudit.audit_criticallty 
                    + "',audit_criteria='" + objAudit.audit_criteria + "',contractual_agreement='" + objAudit.contractual_agreement + "',purpose='" + objAudit.purpose
                    + "',site_access='" + objAudit.site_access + "',schedule_by='" + objAudit.schedule_by + "',approvedby='" + objAudit.approvedby + "',branch='" + objAudit.branch + "'";

                //if (objAudit.acc_date != null && objAudit.acc_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                //{
                //    sSqlstmt = sSqlstmt + ", acc_date='" + objAudit.acc_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                //}
                
                sSqlstmt = sSqlstmt + " where id_audit_schedule='" + objAudit.id_audit_schedule + "'";
                int id_audit_schedule = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_audit_schedule))
                {
                    if (Convert.ToInt32(objAuditList.ScheculeList.Count) > 0)
                    {
                        objAuditList.ScheculeList[0].id_audit_schedule = objAudit.id_audit_schedule;
                        FunAddAuditTeamDetails(objAuditList);
                    }
                    else
                    {
                        FunUpdateAuditTeamDetails(objAudit.id_audit_schedule);
                    }                    

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditSchedule: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditTeamDetails(string sid_audit_schedule)
        {
            try
            {
                string sSqlstmt = "delete from t_audit_schedule_trans where id_audit_schedule='" + sid_audit_schedule + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditTeamDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteAuditSchedule(string sid_audit_schedule)
        {
            try
            {
                string sSqlstmt = "update t_audit_schedule set Active=0 where id_audit_schedule='" + sid_audit_schedule + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAuditSchedule: " + ex.ToString());
            }
            return false;
        }

    }

    public class AuditScheduleModelsList
    {
        public List<AuditScheduleModels> ScheculeList { get; set; }
    }
}