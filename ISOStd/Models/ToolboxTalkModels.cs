using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class ToolboxTalkModels
    {
        clsGlobal objGlobalData = new clsGlobal();
           
        [Display(Name = "ID")]
        public string ToolBox_TalkId { get; set; }
        
        [Display(Name = "Date and Timing")]
        public DateTime Talk_DateTime { get; set; }
         
        [Display(Name = "Toolbox Talk Conducted By")]
        public string Conducted_By { get; set; }
             
        [DataType(DataType.MultilineText)]
        [Display(Name = "Topic")]
        public string Topic { get; set; }

        //[Required]
        [Display(Name = "Attendees")]
        public int Attendee_No { get; set; }

        //[Required]
        [Display(Name = "Report Number")]
        public string Report_No { get; set; }

        //[Required]
        [Display(Name = "Document(s)")]
        public string Upload_Report { get; set; }

        [Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Attended By")]
        public string Attended_by { get; set; }

        [Display(Name = "Activity type")]
        public string Activity_type { get; set; }

        [Display(Name = "Method/Procedure Identified")]
        public string Identified_method { get; set; }

        [Display(Name = "PTW")]
        public string ptw { get; set; }

        [Display(Name = "Complying with PPE")]
        public string Comply_ppe { get; set; }

        [Display(Name = "Identified Hazards")]
        public string Identified_hazard { get; set; }

        [Display(Name = "Worker/Employee feedback")]
        public string feedback_tbt { get; set; }

        [Display(Name = "Details of explanation")]
        public string outcome_tbt { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunDeletToolBoxTalkDoc(string sToolBox_TalkId)
        {
            try
            {
                string sSqlstmt = "update t_toolbox_talk set Active=0 where ToolBox_TalkId='" + sToolBox_TalkId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletToolBoxTalkDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddToolboxTalk(ToolboxTalkModels objToolboxTalk)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_toolbox_talk (Conducted_By, Topic, Attendee_No, Report_No, LoggedBy, Project, Location, Department,Attended_by,Activity_type," +
                    "Identified_method,ptw,Comply_ppe,Identified_hazard,branch,feedback_tbt,outcome_tbt";
                string user = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                user = objGlobalData.GetCurrentUserSession().empid;
                
                if (objToolboxTalk.Talk_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Talk_DateTime";
                    sValues = sValues + ", '" + objToolboxTalk.Talk_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objToolboxTalk.Upload_Report != null && objToolboxTalk.Upload_Report != "")
                {
                    sColumn = sColumn + ", Upload_Report";
                    sValues = sValues + ", '" + objToolboxTalk.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objToolboxTalk.Conducted_By + "','" + objToolboxTalk.Topic
                    + "','" + objToolboxTalk.Attendee_No + "','" + objToolboxTalk.Report_No
                 + "','" + user + "','" + objToolboxTalk.Project + "','" + objToolboxTalk.Location + "','" + objToolboxTalk.Department + "'"
                 + ",'" + objToolboxTalk.Attended_by + "','" + objToolboxTalk.Activity_type + "','" + objToolboxTalk.Identified_method + "','" 
                 + objToolboxTalk.ptw + "','" + objToolboxTalk.Comply_ppe + "','" + objToolboxTalk.Identified_hazard + "','" + objToolboxTalk.branch
                 + "','" + objToolboxTalk.feedback_tbt + "','" + objToolboxTalk.outcome_tbt + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int ToolBox_TalkId = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out ToolBox_TalkId))
                {
                    if (ToolBox_TalkId > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("TBT", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            Report_No = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_toolbox_talk set Report_No='" + Report_No + "' where ToolBox_TalkId = '" + ToolBox_TalkId + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddToolboxTalk: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateToolboxTalk(ToolboxTalkModels objToolboxTalk)
        {
            try
            {
                string sSqlstmt = " update t_toolbox_talk set Conducted_By='" + objToolboxTalk.Conducted_By + "', Topic='" + objToolboxTalk.Topic + "',Identified_method='" + objToolboxTalk.Identified_method + "',Comply_ppe='" + objToolboxTalk.Comply_ppe
                    + "', Attendee_No='" + objToolboxTalk.Attendee_No /*+ "', Report_No='" + objToolboxTalk.Report_No*/ + "',Project='" + objToolboxTalk.Project + "',ptw='" + objToolboxTalk.ptw + "',Identified_hazard='" + objToolboxTalk.Identified_hazard
                   + "',Location='" + objToolboxTalk.Location+ "',Department='" + objToolboxTalk.Department + "',Attended_by='" + objToolboxTalk.Attended_by + "',Activity_type='" + objToolboxTalk.Activity_type
                   + "',feedback_tbt='" + objToolboxTalk.feedback_tbt + "',outcome_tbt='" + objToolboxTalk.outcome_tbt + "',branch='" + objToolboxTalk.branch + "'";

                if (objToolboxTalk.Talk_DateTime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Talk_DateTime='" + objToolboxTalk.Talk_DateTime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objToolboxTalk.Upload_Report != null && objToolboxTalk.Upload_Report != "")
                {
                    sSqlstmt = sSqlstmt + ", Upload_Report='" + objToolboxTalk.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + " where ToolBox_TalkId='" + objToolboxTalk.ToolBox_TalkId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateToolboxTalk: " + ex.ToString());
            }
            return false;
        }
    }

    public class ToolboxTalkModelsList
    {
        public List<ToolboxTalkModels> lstToolboxTalkModels { get; set; }
    }
}