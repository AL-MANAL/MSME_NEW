using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class ActionTrackingRegisterModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_action { get; set; }

        [Display(Name = "Action to be taken")]
        public string Action_name { get; set; }

        [Display(Name = "Target Date")]
        public DateTime Target_date { get; set; }

        [Display(Name = "Due Date")]
        public DateTime Due_date { get; set; }

        [Display(Name = "Completion Date")]
        public DateTime Completion_date { get; set; }

        [Display(Name = "Action by")]
        public string ActionBy { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Additional Attachment")]
        public string Additional_Attachment { get; set; }

        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Source of action")]
        public string source_id { get; set; }

        public string source { get; set; } //Display in the List form

        [Display(Name = "Report No")]
        public string report_no { get; set; }

        internal bool FunDeleteActionTrackingRegister(string sid_action)
        {
            try
            {
                string sSqlstmt = "update t_action_tracking_register set Active=0 where id_action='" + sid_action + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteActionTrackingRegister: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddActionTrackingRegister(ActionTrackingRegisterModels objTrack)
        {
            try
            {
                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                sSqlstmt = "insert into t_action_tracking_register (Action_name,ActionBy,Status,Remarks,NotificationDays,NotificationPeriod,NotificationValue,source_id,report_no,branch";
                string sFields = "", sFieldValue = "";

                if (objTrack.Target_date != null && objTrack.Target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Target_date";
                    sFieldValue = sFieldValue + ", '" + objTrack.Target_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objTrack.Due_date != null && objTrack.Due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Due_date";
                    sFieldValue = sFieldValue + ", '" + objTrack.Due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objTrack.Completion_date != null && objTrack.Completion_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Completion_date";
                    sFieldValue = sFieldValue + ", '" + objTrack.Completion_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objTrack.Action_name + "','" + objTrack.ActionBy + "','" + objTrack.Status + "','" + objTrack.Remarks + "','"
                    + objTrack.NotificationDays + "','" + objTrack.NotificationPeriod + "','" + objTrack.NotificationValue + "','" + objTrack.source_id + "','" + objTrack.report_no + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddActionTrackingRegister: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionTrackingRegister(ActionTrackingRegisterModels objTrack)
        {
            try
            {
                string sSqlstmt = "update t_action_tracking_register set  ActionBy='" + objTrack.ActionBy + "', "
                    + "Status='" + objTrack.Status + "',Remarks='" + objTrack.Remarks + "', NotificationDays='" + objTrack.NotificationDays + "', NotificationPeriod='" + objTrack.NotificationPeriod + "', NotificationValue='" + objTrack.NotificationValue
                    + "', source_id='" + objTrack.source_id + "'";

                if (objTrack.Target_date != null && objTrack.Target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Target_date='" + objTrack.Target_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objTrack.Due_date != null && objTrack.Due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Due_date='" + objTrack.Due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objTrack.Completion_date != null && objTrack.Completion_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Completion_date='" + objTrack.Completion_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_action='" + objTrack.id_action + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionTrackingRegister: " + ex.ToString());
            }
            return false;
        }
    }

    public class ActionTrackingRegisterModelsList
    {
        public List<ActionTrackingRegisterModels> lstTrack { get; set; }
    }
}