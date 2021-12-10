using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class QHSEPlannerModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_qhse { get; set; }

        [Display(Name = "Standards/Criteria")]
        public string QHSE_name { get; set; }

        [Display(Name = "Activity")]
        public string Activity_name { get; set; }

        [Display(Name = "Planned on")]
        public DateTime Planned_date { get; set; }

        [Display(Name = "Actual Date")]
        public DateTime Actual_date { get; set; }

        [Display(Name = "Person Responsible")]
        public string Person_responsible { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Target Date")]
        public DateTime Target_date { get; set; }

        [Display(Name = "Department")]
        public string DeptId { get; set; }

        [Display(Name = "Document")]
        public string Upload { get; set; }

        [Required]
        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Required]
        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Required]
        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Logged By")]
        public string Logged_by { get; set; }

        // event calender

        public int id_event { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool full_day { get; set; }
        public string Person_name { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string event_status { get; set; }
        public string event_type { get; set; }
        public string event_priority { get; set; }
        public string priority_color { get; set; }
        public string id_comments { get; set; }
        public string comments { get; set; }
        public DateTime logged_date { get; set; }
        public string sevent_status { get; set; }
        public string sevent_type { get; set; }
        public string sevent_priority { get; set; }
        public string notification_to { get; set; }
        public string Logged_Id {get; set;}

        public List<QHSEPlannerModels> commentList { get; set; }

        internal bool FunAddComments(QHSEPlannerModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_event_planner_comments(id_event,comments,Logged_by";
                string sFields = "", sFieldValue = "";
              
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.id_event + "','" + objModel.comments + "'"
                    + ",'" + objGlobalData.GetCurrentUserSession().empid + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddComments: " + ex.ToString());
            }
            return false;
        }   

        internal bool FunSaveEvent(QHSEPlannerModels objModel)
        {
            try
            {
                
                string sSqlstmt = "insert into t_event_planner(subject,description,full_day,Person_responsible,NotificationDays,NotificationPeriod,NotificationValue,Logged_by,branch,event_status,event_type,event_priority,notification_to";
                string sFields = "", sFieldValue = "";

                if (objModel.start_date != null && objModel.start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", start_date";
                    string start = objModel.start_date.ToString("yyyy/MM/dd") + " " + objModel.starttime + ":00";
                    sFieldValue = sFieldValue + ", '" + start + "'";
                }
                if (objModel.end_date != null && objModel.end_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", end_date";
                    string end = objModel.end_date.ToString("yyyy/MM/dd") + " " + objModel.endtime + ":00";
                    sFieldValue = sFieldValue + ", '" + end + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.subject + "','" + objModel.description + "','" + objModel.full_day + "'"
                    + ",'" + objModel.Person_responsible + "','" + objModel.NotificationDays + "','" + objModel.NotificationPeriod + "','" + objModel.NotificationValue + "'"
                    + ",'" + objGlobalData.GetCurrentUserSession().empid + "','" + objGlobalData.GetCurrentUserSession().division + "'"
                    + ",'" + objModel.event_status + "','" + objModel.event_type + "','" + objModel.event_priority + "','" + objModel.notification_to + "'";
                 
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_event = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_event))
                {
                    if (objModel.comments != "" && objModel.comments != null)
                    {
                        objModel.id_event =id_event;
                        FunAddComments(objModel);
                    }
                    SendEmail(objModel);
                    return true;
                }

              
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunSaveEvent: " + ex.ToString());
            }
            return false;
        }
        internal bool SendEmail(QHSEPlannerModels objModel)
        {
            try
            {
                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "";
                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objModel.Person_responsible) + ","+objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objModel.notification_to);
                sUserName = objGlobalData.GetMultiHrEmpNameById(objModel.notification_to);

                sInformation =
                "Event:'" + objModel.subject + "'"
                + " <br />"
                + "Description:'" + objModel.description + "'"
                + " <br />"
               + "Start Date:'" + objModel.start_date.ToString("dd/MM/yyyy hh:mm:ss") + "'"
                + " <br />"
               + "End Date:'" + objModel.end_date.ToString("dd/MM/yyyy hh:mm:ss") + "'";

                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "CalendarEvent", sHeader, "", "Event Details");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateEvent(QHSEPlannerModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_event_planner set subject='" + objModel.subject + "',description='" + objModel.description + "',full_day='" + objModel.full_day + "'"
                    + ",Person_responsible='" + objModel.Person_responsible + "',NotificationDays='" + objModel.NotificationDays + "',NotificationPeriod='" + objModel.NotificationPeriod + "'"
                    + ",NotificationValue='" + objModel.NotificationValue + "',event_status='" + objModel.event_status + "',event_type='" + objModel.event_type + "',event_priority='" + objModel.event_priority + "',notification_to='" + objModel.notification_to + "'";

                if (objModel.start_date != null && objModel.start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    string start = objModel.start_date.ToString("yyyy/MM/dd") + " " + objModel.starttime + ":00";
                    sSqlstmt = sSqlstmt + ",start_date='" + start + "'";
                }
                if (objModel.end_date != null && objModel.end_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    string end = objModel.end_date.ToString("yyyy/MM/dd") + " " + objModel.endtime + ":00";
                    sSqlstmt = sSqlstmt + ",end_date='" + end + "'";
                }
                else
                {
                    sSqlstmt = sSqlstmt + ",end_date=null";
                }
                sSqlstmt = sSqlstmt + " where id_event='" + objModel.id_event + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (objModel.comments != "" && objModel.comments != null)
                    {
                        FunAddComments(objModel);
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEvent: " + ex.ToString());
            }

            return false;
        }
        internal bool FunDeleteCalenderEvent(string eventID)
        {
            try
            {
                string sSqlstmt = "update t_event_planner set Active=0 where id_event='" + eventID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCalenderEvent: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteQHSEPlanner(string sid_qhse)
        {
            try
            {
                string sSqlstmt = "update t_qhse_planner set Active=0 where id_qhse='" + sid_qhse + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQHSEPlanner: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddQHSEPlanner(QHSEPlannerModels objPlan,QHSEPlannerModelsList lstPlanner)
        {
            try
            {
                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                for (int i = 0; i < lstPlanner.lstPlan.Count; i++)
                {
                   
                    sSqlstmt = sSqlstmt + "insert into t_qhse_planner (QHSE_name,Activity_name,Person_responsible,NotificationDays,NotificationPeriod,NotificationValue,Logged_by,branch";
                    string sFields = "", sFieldValue = "";

                    if (lstPlanner.lstPlan[i].Planned_date != null && lstPlanner.lstPlan[i].Planned_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                    sFields = sFields + ", Planned_date";
                    sFieldValue = sFieldValue + ", '" + lstPlanner.lstPlan[i].Planned_date.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstPlanner.lstPlan[i].Target_date != null && lstPlanner.lstPlan[i].Target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                    sFields = sFields + ", Target_date";
                    sFieldValue = sFieldValue + ", '" + lstPlanner.lstPlan[i].Target_date.ToString("yyyy/MM/dd") + "'";
                    }
                     if (lstPlanner.lstPlan[i].Remarks == "")
                     {
                         sFields = sFields + ", Remarks";
                         sFieldValue = sFieldValue + ", ' '";
                     }
                     else
                     {
                         sFields = sFields + ", Remarks";
                         sFieldValue = sFieldValue + ", '" + lstPlanner.lstPlan[i].Remarks + "'";
                     }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + lstPlanner.lstPlan[i].QHSE_name + "','" + lstPlanner.lstPlan[i].Activity_name + "',"
                   + "'" + lstPlanner.lstPlan[i].Person_responsible + "','" + lstPlanner.lstPlan[i].NotificationDays + "','" + lstPlanner.lstPlan[i].NotificationPeriod + "'"
                   + ",'" + lstPlanner.lstPlan[i].NotificationValue + "','" + objPlan.Logged_by + "','" + sBranch + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                     
                }

               if( objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    for (int i = 0; i < lstPlanner.lstPlan.Count; i++)
                    {
                        SendEventEmail(lstPlanner, i);
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddQHSEPlanner: " + ex.ToString());

            }
            return false;
        }

        internal bool SendEventEmail(QHSEPlannerModelsList lstPlanner,int i)
        {

            try
            {

                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "";

                // string sInformation = "", sHeader = "";
                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                    sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(lstPlanner.lstPlan[i].Person_responsible);
                    //sUserName = objGlobalData.GetMultiHrEmpNameById(objGlobalData.GetHSEEmployee());

                    sInformation = 
                    "Activity Name:'" + lstPlanner.lstPlan[i].Activity_name + "'"
                    + " <br />"
                    + "Planned Date:'" + lstPlanner.lstPlan[i].Planned_date.ToString("dd/MM/yyyy")+ "'"
                    + " <br />"
                    + "Target Date:'" + lstPlanner.lstPlan[i].Target_date.ToString("dd/MM/yyyy") + "'"
                    + " <br />"
                    + "Remarks:'" + lstPlanner.lstPlan[i].Remarks + "'";
                    sHeader = sHeader + sInformation;

                    sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                    sToEmailId = sToEmailId.Trim();
                    sToEmailId = sToEmailId.TrimEnd(',');
                    sToEmailId = sToEmailId.TrimStart(',');

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "events", sHeader, "", "Planned Event Information");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendIncidentEmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateActionTrackingRegister(QHSEPlannerModels objPlan)
        {
            try
            {
                string sSqlstmt = "update t_qhse_planner set QHSE_name ='" + objPlan.QHSE_name + "', Activity_name='" + objPlan.Activity_name + "', "
                    + "Person_responsible='" + objPlan.Person_responsible + "',Remarks='" + objPlan.Remarks + "'"
                    + ",NotificationDays='" + objPlan.NotificationDays + "',NotificationPeriod='" + objPlan.NotificationPeriod + "',NotificationValue='" + objPlan.NotificationValue + "'";

                if (objPlan.Planned_date != null && objPlan.Planned_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Planned_date='" + objPlan.Planned_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objPlan.Target_date != null && objPlan.Target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Target_date='" + objPlan.Target_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_qhse='" + objPlan.id_qhse + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionTrackingRegister: " + ex.ToString());

            }
            return false;
        }
        internal bool FunUpdateQHSEPlannerstatus(QHSEPlannerModels objPlan)
        {
            try
            {
                string remarks = " "+ objPlan.Remarks;
                string sSqlstmt = "update t_qhse_planner set Status ='" + objPlan.Status + "', Upload='" + objPlan.Upload + "', "
                   + "Remarks=CONCAT(Remarks,'" + remarks + "')";

                if (objPlan.Actual_date != null && objPlan.Actual_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Actual_date='" + objPlan.Actual_date.ToString("yyyy/MM/dd") + "'";
                }
              
                sSqlstmt = sSqlstmt + " where id_qhse='" + objPlan.id_qhse + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateQHSEPlannerstatus: " + ex.ToString());
            }
            return false;
        }
        
    }
    public class QHSEPlannerModelsList
    {
        public List<QHSEPlannerModels> lstPlan { get; set; }
    }
}