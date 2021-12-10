using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
namespace ISOStd.Models
{
    public class MeetingModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Meeting Id")]
        public string meeting_id { get; set; }
              
        [Display(Name = "Last Meeting Ref.")]
        public string last_meeting_id { get; set; }

        [Required]
        [Display(Name = "Meeting Ref.")]
        [System.Web.Mvc.Remote("doesMeetingRefExist", "Meeting", HttpMethod = "POST", ErrorMessage = "Meeeting Reference ID already exists. Please enter a different ID.")]        
        public string meeting_ref { get; set; }
              
        [Display(Name = "Meeting Date and Time")]
        public DateTime meeting_date { get; set; }
               
        [Display(Name = "Meeting Details")]
        public string meeting_type_desc { get; set; }
               
        [Display(Name = "Attendees")]
        public string Attendees { get; set; }

        [Display(Name = "Attendees")]
        public string AttendeesUser { get; set; }
              
        [Display(Name = "Meeting Created By")]
        public string MeetingCreatedBy { get; set; }
        
        [Display(Name = "item No.")]
        public string item_no { get; set; }
            
        [Display(Name = "Item discussed")]
        public string item_discussed { get; set; }

        [Display(Name = "Action(s) agreed")]
        public string action_agreed { get; set; }

        [Display(Name = "Due date / period for action")]
        public DateTime due_date { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string action_owner { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }
  
        [Display(Name = "Item Status")]
        public string item_status { get; set; }
      
        [Display(Name = "Completion date")]
        public DateTime completiondate { get; set; }
 
        [Display(Name = "Agenda")]
        public string agenda_id { get; set; }
 
        [Display(Name = "Meeting Location")]
        [DataType(DataType.MultilineText)]
        public string Venue { get; set; }
     
        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }
   
        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Display(Name = "Notification Days")]
        public decimal NotificationDays { get; set; }

        public string selectall { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "If any External Attendees")]
        public string ext_attendees { get; set; }

        [Display(Name = "External Attendees Email")]
        public string ext_email { get; set; }

        //t_meeting_external_attendees

        public string id_attendees { get; set; }

        [Display(Name = "Company Name")]
        public string company_name { get; set; }

        [Display(Name = "Attendee Name")]
        public string attendee_name { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Email Id")]
        public string email_id { get; set; }

        [Display(Name = "Action status ")]
        public string action_status { get; set; }

        [Display(Name = "Status updated on")]
        public DateTime status_update_on { get; set; }


        public string GetMeetingAgendaNameById(string AgendaId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select Agenda_Desc from t_meeting_agenda where agenda_id='" + AgendaId + "'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    return (dsMeeting.Tables[0].Rows[0]["Agenda_Desc"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendaNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunDeleteMeetingDoc(string smeeting_id)
        {
            try
            {
                string sSqlstmt = "update t_meeting set Active=0 where meeting_id='" + smeeting_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteMeetingDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddMeetingSchedule(MeetingModels objMeetingModels, MeetingModelsList objModelsList)
        {
            try
            {
                string sMeetingDate = objMeetingModels.meeting_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "";

                //foreach (string AgendaId in lstAgendaId)
                {
                    sSqlstmt = sSqlstmt + "insert into t_meeting (last_meeting_id, meeting_date, meeting_type_desc, AttendeesUser, agenda_id, MeetingCreatedBy, Venue"
                        + ", NotificationPeriod, NotificationValue, NotificationDays,branch,ext_attendees,ext_email,remarks)"
                                + " values('" + objMeetingModels.last_meeting_id + "', '" + sMeetingDate + "', '" + objMeetingModels.meeting_type_desc
                                + "','" + objMeetingModels.AttendeesUser + "','" + objMeetingModels.agenda_id + "','" + objMeetingModels.MeetingCreatedBy
                                + "','" + objMeetingModels.Venue + "','" + objMeetingModels.NotificationPeriod + "','" + objMeetingModels.NotificationValue + "','"
                                + objMeetingModels.NotificationDays + "','" + sBranch + "','" + objMeetingModels.ext_attendees + "','" + objMeetingModels.ext_email + "','" + remarks + "')";
                }

                MeetingItemModels objItemModel = new MeetingItemModels();

                int meeting_id = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out meeting_id))
                {
                    DataSet dsData = objGlobalData.GetReportNo("MTG", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        meeting_ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    if (objModelsList.MeetingMList.Count > 0)
                    {
                        objModelsList.MeetingMList[0].meeting_id = meeting_id.ToString();
                        FunAddExternalAttendeesList(objModelsList);
                    }

                    string sql1 = "update t_meeting set meeting_ref='" + meeting_ref + "' where meeting_id='" + meeting_id + "'";

                    return (objGlobalData.ExecuteQuery(sql1));
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingSchedule: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddUnplanMeetingSchedule(MeetingModels objMeetingModels)
        {
            try
            {
                string sMeetingDate = objMeetingModels.meeting_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "";

                //foreach (string AgendaId in lstAgendaId)
                {
                    sSqlstmt = sSqlstmt + "insert into t_meeting (last_meeting_id, meeting_date, meeting_type_desc, AttendeesUser, agenda_id, MeetingCreatedBy, Venue"
                        + ", NotificationPeriod, NotificationValue, NotificationDays,branch,ext_attendees,ext_email)"
                                + " values('" + objMeetingModels.last_meeting_id + "', '" + sMeetingDate + "', '" + objMeetingModels.meeting_type_desc
                                + "','" + objMeetingModels.AttendeesUser + "','" + objMeetingModels.agenda_id + "','" + objMeetingModels.MeetingCreatedBy
                                + "','" + objMeetingModels.Venue + "','" + objMeetingModels.NotificationPeriod + "','" + objMeetingModels.NotificationValue + "','"
                                + objMeetingModels.NotificationDays + "','" + sBranch + "','" + objMeetingModels.ext_attendees + "','" + objMeetingModels.ext_email + "')";
                }

                MeetingItemModels objItemModel = new MeetingItemModels();

                int meeting_id = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out meeting_id))
                {
                    DataSet dsData = objGlobalData.GetReportNo("MTG", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        meeting_ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                   

                    string sql1 = "update t_meeting set meeting_ref='" + meeting_ref + "' where meeting_id='" + meeting_id + "'";

                    return (objGlobalData.ExecuteQuery(sql1));
                }
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingSchedule: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddExternalAttendeesList(MeetingModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_meeting_external_attendees where meeting_id='" + objModelsList.MeetingMList[0].meeting_id + "'; ";

                for (int i = 0; i < objModelsList.MeetingMList.Count; i++)
                {
                   
                        sSqlstmt = sSqlstmt + " insert into t_meeting_external_attendees (meeting_id,company_name,attendee_name,designation,email_id";

                        string sFields = "", sFieldValue = "";
                       
                      
                        sSqlstmt = sSqlstmt + sFields;
                        sSqlstmt = sSqlstmt + ") values('" + objModelsList.MeetingMList[0].meeting_id + "','" + objModelsList.MeetingMList[i].company_name
                          + "','" + objModelsList.MeetingMList[i].attendee_name + "','" + objModelsList.MeetingMList[i].designation + "','" + objModelsList.MeetingMList[i].email_id + "'";
                        sSqlstmt = sSqlstmt + sFieldValue + ");";
     
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExternalAttendeesList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddMeetingItems(MeetingModelsList objMeetingModels, string sagenda_id)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objMeetingModels.MeetingMList.Count; i++)
                {
                    string sCompletionDate = "";
                    if (objGlobalData.GetDropdownitemById(objMeetingModels.MeetingMList[i].item_status).ToLower() == "Closed")
                    {
                        sCompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                    }

                    string sdue_date = objMeetingModels.MeetingMList[i].due_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                    sSqlstmt = sSqlstmt + "insert into t_meeting_items (meeting_ref, agenda_id, item_discussed, action_agreed, due_date, action_owner, item_status,remarks,upload";
                    if (sCompletionDate != "")
                    {
                        sSqlstmt = sSqlstmt + ", completiondate";
                    }

                    sSqlstmt = sSqlstmt + ") values('" + objMeetingModels.MeetingMList[i].meeting_ref + "','"
                        + sagenda_id + "', '" + objMeetingModels.MeetingMList[i].item_discussed + "','" + objMeetingModels.MeetingMList[i].action_agreed + "','" + sdue_date
                        + "','" + objMeetingModels.MeetingMList[i].action_owner + "','" + objMeetingModels.MeetingMList[i].item_status + "','" + objMeetingModels.MeetingMList[i].remarks + "','" + objMeetingModels.MeetingMList[i].upload + "'";

                    if (sCompletionDate != "")
                    {
                        sSqlstmt = sSqlstmt + "'" + sCompletionDate + "'";
                    }
                    sSqlstmt = sSqlstmt + ");";

                }
                if (sSqlstmt != "")
                {
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingItems: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateMeetingItem(MeetingModels objMeetingItemModels)
        {
            try
            {
                string sSqlstmt = "";
               
                //string sCompletionDate = "";
                //if (objGlobalData.GetDropdownitemById(objMeetingItemModels.item_status).ToLower() == "closed")
                //{
                //    sCompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                string sdue_date = objMeetingItemModels.due_date.ToString("yyyy-MM-dd HH':'mm':'ss");

                sSqlstmt = sSqlstmt + "update t_meeting_items set item_discussed='" + objMeetingItemModels.item_discussed
                    + "', action_agreed='" + objMeetingItemModels.action_agreed + "', due_date='" + sdue_date
                    + "', action_owner='" + objMeetingItemModels.action_owner /*+ "', item_status='" + objMeetingItemModels.item_status*/ 
                    + "', remarks='" + objMeetingItemModels.remarks + "'";

                if (objMeetingItemModels.upload != null && objMeetingItemModels.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objMeetingItemModels.upload + "'";
                }

                //if (objGlobalData.GetDropdownitemById(objMeetingItemModels.item_status).ToLower() == "closed")
                //{
                //    sSqlstmt = sSqlstmt + ", completiondate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                //}

                sSqlstmt = sSqlstmt + " where item_no='" + objMeetingItemModels.item_no + "' ";
                //}

                if (sSqlstmt != "")
                {
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMeetingItem: " + ex.ToString());
            }

            return false;
        }
        
        internal bool FunAddMeeting(MeetingModels objMeetingModels, MeetingItemModelsList objItems, List<string> lstAgendaId)
        {
            try
            {
                string sSqlstmt = "update t_meeting set Attendees='" + objMeetingModels.Attendees + "', MeetingCreatedBy='" + objMeetingModels.MeetingCreatedBy
                    + "' where meeting_ref='" + objMeetingModels.meeting_ref + "'";

                MeetingItemModels objItemModel = new MeetingItemModels();

                if (objGlobalData.ExecuteQuery(sSqlstmt) && objItemModel.FunAddMeetingItems(objItems))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeeting: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateAttendeesSheet(MeetingModels objMeetingModels)
        {

            try
            {
                string sSqlstmt = "update t_meeting set MeetingCreatedBy='" + objMeetingModels.MeetingCreatedBy + "'";

                if (objMeetingModels.Attendees != null && objMeetingModels.Attendees != "")
                {
                    sSqlstmt = sSqlstmt + ", Attendees='" + objMeetingModels.Attendees + "' ";
                }
                sSqlstmt = sSqlstmt + " where meeting_ref='" + objMeetingModels.meeting_ref + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAttendeesSheet: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateMeeting(MeetingModels objMeetingModels, MeetingModelsList objModelsList)
        {
            try
            {
                string sMeetingDate = objMeetingModels.meeting_date.ToString("yyyy-MM-dd HH':'mm':'ss");

                //string sSqlstmt = "update t_meeting set meeting_date='" + sMeetingDate + "', MeetingCreatedBy='" + objMeetingModels.MeetingCreatedBy
                //    + "' where meeting_ref='" + objMeetingModels.meeting_ref + "'";

                string sSqlstmt = "update t_meeting set MeetingCreatedBy='" + objMeetingModels.MeetingCreatedBy + "'";

                if (objMeetingModels.Attendees != null && objMeetingModels.Attendees != "")
                {
                    sSqlstmt = sSqlstmt + ", Attendees='" + objMeetingModels.Attendees + "' ";
                }

                if (objMeetingModels.agenda_id != null && objMeetingModels.agenda_id != "")
                {
                    sSqlstmt = sSqlstmt + ", agenda_id='" + objMeetingModels.agenda_id + "', AttendeesUser='" + objMeetingModels.AttendeesUser + "', Venue='" + objMeetingModels.Venue
                        + "', meeting_date='" + sMeetingDate + "', NotificationPeriod='" + objMeetingModels.NotificationPeriod + "', NotificationValue='"
                        + objMeetingModels.NotificationValue + "', NotificationDays='" + objMeetingModels.NotificationDays + "', meeting_type_desc='" + objMeetingModels.meeting_type_desc + "', ext_attendees='" + objMeetingModels.ext_attendees + "', ext_email='" + objMeetingModels.ext_email + "', remarks='" + objMeetingModels.remarks + "'";
                }

                sSqlstmt = sSqlstmt + " where meeting_id='" + objMeetingModels.meeting_id + "'";

                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (objModelsList.MeetingMList.Count > 0)
                    {
                        objModelsList.MeetingMList[0].meeting_id = meeting_id.ToString();
                        FunAddExternalAttendeesList(objModelsList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMeeting: " + ex.ToString());
            }

            return false;
        }

        public Dictionary<string, string> GetPeriousMeetingId(string sMeeting_ref)
        {
            Dictionary<string, string> dicMeeting = new Dictionary<string, string>();
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select meeting_id, meeting_ref from t_meeting where meeting_ref='" + sMeeting_ref
                    + "' order by meeting_id desc limit 1");
              
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    if (dsMeeting.Tables[0].Rows[0]["meeting_id"].ToString() != "" && dsMeeting.Tables[0].Rows[0]["meeting_ref"].ToString() != "")
                    {
                        dicMeeting.Add(dsMeeting.Tables[0].Rows[0]["meeting_id"].ToString(), dsMeeting.Tables[0].Rows[0]["meeting_ref"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetPeriousMeetingId: " + ex.ToString());
            }
            return dicMeeting;
        }

        public string GetMeetingRefbyId(string sAgenda_Id)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select meeting_id, meeting_ref from t_meeting where agenda_Id='" + sAgenda_Id + "'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    return dsMeeting.Tables[0].Rows[0]["meeting_ref"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingRefbyId: " + ex.ToString());
            }
            return "";
        }

        public string GetMeetingRefbyPrevRefId(string last_meeting_id)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select meeting_id, meeting_ref from t_meeting where meeting_ref='" + last_meeting_id + "'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    return dsMeeting.Tables[0].Rows[0]["meeting_ref"].ToString();
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingRefbyPrevRefId: " + ex.ToString());
            }
            return "";
        }

        public bool checkMeetingRefExists(string sMeetingRef)
        {
            try
            {
                string sSqlstmt = "select meeting_id from t_meeting where meeting_ref='" + sMeetingRef + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkMeetingRefExists: " + ex.ToString());
            }
            return true;
        }

        //status update
        internal bool FunUpdateStatus(MeetingModels objModel)
        {
            try
            {

                string sSqlstmt = "update t_meeting set action_status='" + action_status + "'";
                if (objModel.status_update_on != null && objModel.status_update_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",status_update_on='" + objModel.status_update_on.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where meeting_id='" + objModel.meeting_id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateStatus: " + ex.ToString());
            }
            return false;
        }
    }

    public class MeetingItemModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Meeting Id")]
        public string Meeting_Ref { get; set; }

        [Required]
        [Display(Name = "item No.")]
        public string item_no { get; set; }

        //[Required]
        [Display(Name = "Item discussed")]
        public string item_discussed { get; set; }

        //[Required]
        [Display(Name = "Action(s) agreed")]
        public string action_agreed { get; set; }

        [Required]
        [Display(Name = "Due date / period for action")]
        public DateTime due_date { get; set; }

        [Required]
        [Display(Name = "Personnel Responsible")]
        public string action_owner { get; set; }

        [Required]
        [Display(Name = "Item Status")]
        public string item_status { get; set; }

        [Required]
        [Display(Name = "Completion date")]
        public DateTime completiondate { get; set; }

        [Required]
        [Display(Name = "Agenda")]
        public string agenda_id { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        internal bool FunAddMeetingItems(MeetingItemModelsList objMeetingModels)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objMeetingModels.MeetingItemMList.Count; i++)
                {
                    string sCompletionDate = "";
                    if (objGlobalData.GetDropdownitemById(objMeetingModels.MeetingItemMList[i].item_status).ToLower() == "Closed")
                    {
                        sCompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") ;
                    }

                    string sdue_date = objMeetingModels.MeetingItemMList[i].due_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                    sSqlstmt = sSqlstmt + "insert into t_meeting_items (agenda_id, meeting_ref, item_discussed, action_agreed, due_date, action_owner, item_status,remarks,upload";
                        
                    if (sCompletionDate != "")
                    {
                        sSqlstmt = sSqlstmt + ", completiondate";
                    }

                    sSqlstmt = sSqlstmt + ") values('"
                        + objMeetingModels.MeetingItemMList[i].agenda_id + "','" + objMeetingModels.MeetingItemMList[i].Meeting_Ref + "', '"
                        + objMeetingModels.MeetingItemMList[i].item_discussed + "','" + objMeetingModels.MeetingItemMList[i].action_agreed + "','" + sdue_date
                        + "','" + objMeetingModels.MeetingItemMList[i].action_owner + "','" + objMeetingModels.MeetingItemMList[i].item_status + "','" + objMeetingModels.MeetingItemMList[i].remarks + "','" + objMeetingModels.MeetingItemMList[i].upload + "'";

                    if (sCompletionDate != "")
                    {
                        sSqlstmt = sSqlstmt + ",'" + sCompletionDate + "'";
                    }
                    sSqlstmt = sSqlstmt + ");";
                }
                if (sSqlstmt != "")
                {
                    if (objGlobalData.ExecuteQuery(sSqlstmt))
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingItems: " + ex.ToString());
            }
            return false;
        }

        internal bool SendMOMEmail(string smeeting_ref, string sMessage="")
        {
            try
            {
                string sType = "email";
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "SELECT agenda_id, meeting_id, last_meeting_id, meeting_date, meeting_type_desc, meeting_ref,  Attendees, MeetingCreatedBy,"
                    + "AttendeesUser, Venue,ext_attendees,ext_email from t_meeting where  meeting_ref='" + smeeting_ref
                    + "' order by agenda_id desc limit 1";

                DataSet dsMeetingList = objGlobalData.Getdetails(sSqlstmt);
                MeetingTypeModels objType = new MeetingTypeModels();
                MeetingAgendaModels objMeetingAgendaModels = new MeetingAgendaModels();

                if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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
                    string sName = "All";
                    string sToEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()) != "")
                    {
                    sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString()) + ",";
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    if (dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString() != "")
                    {
                        sToEmailIds = sToEmailIds + "," + dsMeetingList.Tables[0].Rows[0]["ext_email"].ToString();
                    }
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["MeetingCreatedBy"].ToString());
                    aAttachment = HttpContext.Current.Server.MapPath(dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString());


                    sHeader = "<tr><td colspan=3><b>Meeting Type:<b></td> <td colspan=3>"
                        + objType.GetMeetingTypeNameById(dsMeetingList.Tables[0].Rows[0]["meeting_type_desc"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Agenda:<b></td> <td colspan=3>" + objMeetingAgendaModels.GetMeetingAgendaById(dsMeetingList.Tables[0].Rows[0]["Agenda_Id"].ToString())+ "</td></tr>"
                       + "<tr><td colspan=3><b>Meeting Reference:<b></td> <td colspan=3>" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "</td></tr>"

                       + "<tr><td colspan=3><b>Meeting Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["meeting_date"].ToString()).ToString("dd/MM/yyyy HH:mm:ss")
                       + "</td></tr>"
                    + "<tr><td colspan=3><b>Attendees:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["AttendeesUser"].ToString())+ "," +dsMeetingList.Tables[0].Rows[0]["ext_attendees"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>Meeting Location:<b></td> <td colspan=3>" + (dsMeetingList.Tables[0].Rows[0]["Venue"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Attendance Sheet:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sSqlstmt = "select item_no, Agenda_Desc,Agenda_Details, item_discussed, action_agreed,t_meeting_items.remarks, due_date, action_owner, item_status,"
                    + "completiondate from t_meeting_items, t_meeting, t_meeting_agenda where" 
                    +" t_meeting_items.meeting_ref=t_meeting.meeting_ref and t_meeting_items.agenda_Id=t_meeting_agenda.Agenda_Id"
                    + " and t_meeting.meeting_ref='" + dsMeetingList.Tables[0].Rows[0]["meeting_ref"].ToString() + "' order by item_no desc";

                    DataSet dsItems = new DataSet();                    
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=6><label><b>Minutes of Meeting</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th>Agenda</th>"
                            + "<th>Agenda Details</th>"
                            + "<th>Item discussed</th>"
                            + "<th>Action(s) agreed</th>"
                            + "<th>Due date / period for action</th>"
                            + "<th>Personnel Responsible</th>"
                            + "<th>Item Status</th>"
                             + "<th>Remarks</th>"
                            + "</tr>";

                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td>" + dsItems.Tables[0].Rows[i]["Agenda_Desc"].ToString() + "</td>"
                                + " <td>" + dsItems.Tables[0].Rows[i]["Agenda_Details"].ToString() + "</td>"
                                + "<td> " + dsItems.Tables[0].Rows[i]["item_discussed"].ToString() + " </td>"
                                 + " <td>" + dsItems.Tables[0].Rows[i]["action_agreed"].ToString() + "</td>"
                                 + "<td> " + Convert.ToDateTime(dsItems.Tables[0].Rows[i]["due_date"].ToString()).ToString("dd/MM/yyyy") + " </td>"
                                + "<td> " +objGlobalData.GetEmpHrNameById(dsItems.Tables[0].Rows[i]["action_owner"].ToString()) + " </td>"
                                + "<td> " + objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["item_status"].ToString()) + " </td>"
                                   + "<td> " + dsItems.Tables[0].Rows[i]["remarks"].ToString() + " </td>"
                                + " </tr>";
                            if (objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["action_owner"].ToString()) != "")
                            {
                             sToEmailIds = sToEmailIds +","+objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["action_owner"].ToString()) + ",";
                            }
                           
                        }
                        sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                        sToEmailIds = sToEmailIds.Trim();
                        sToEmailIds = sToEmailIds.TrimEnd(',');
                        sToEmailIds = sToEmailIds.TrimStart(',');
                    }
                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Meeting Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendMOMEmail: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunUpdateMeetingItem(MeetingItemModels objMeetingItemModels)
        {
            try
            {
                string sSqlstmt = "";
                string sCompletionDate = "";
                if (objGlobalData.GetDropdownitemById(objMeetingItemModels.item_status).ToLower() == "Closed")
                {
                    sCompletionDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                string sdue_date = objMeetingItemModels.due_date.ToString("yyyy-MM-dd HH':'mm':'ss");

                sSqlstmt = sSqlstmt + "update t_meeting_items set item_discussed='" + objMeetingItemModels.item_discussed
                    + "', action_agreed='" + objMeetingItemModels.action_agreed + "', due_date='" + sdue_date
                    + "', action_owner='" + objMeetingItemModels.action_owner + "', item_status='" + objMeetingItemModels.item_status + "', remarks='" + objMeetingItemModels.remarks+ "', upload='" + objMeetingItemModels.upload + "'";

                if (objGlobalData.GetDropdownitemById(objMeetingItemModels.item_status).ToLower() == "Closed")
                {
                    sSqlstmt = sSqlstmt + ", completiondate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                sSqlstmt = sSqlstmt + " where item_no='" + objMeetingItemModels.item_no + "' ";

                if (sSqlstmt != "")
                {
                   return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMeetingItem: " + ex.ToString());
            }

            return false;
        }
    }

    public class MeetingItemLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Log Id")]
        public string meeting_log_id { get; set; }

        [Required]
        [Display(Name = "item No.")]
        public string item_no { get; set; }

        [Required]
        [Display(Name = "Action taken on")]
        public DateTime action_taken_on { get; set; }

        [Required]
        [Display(Name = "Action completed by")]
        public string action_completed_by { get; set; }

        
        [Required]
        [Display(Name = "Log Status")]
        public string LogStatus { get; set; }

        internal bool FunAddItemLog(MeetingItemLogModels objMeetingItemLogModels)
        {
            try
            {
                string saction_taken_on = objMeetingItemLogModels.action_taken_on.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_meeting_items_log (Item_no, action_taken_on, action_completed_by,LogStatus) values('"
                    + objMeetingItemLogModels.item_no + "','" + saction_taken_on + "','" + objMeetingItemLogModels.action_completed_by + "','" + objMeetingItemLogModels.LogStatus + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddItemLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateItemLog(MeetingItemLogModels objMeetingItemLogModels)
        {
            try
            {
                string saction_taken_on = objMeetingItemLogModels.action_taken_on.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_meeting_items_log set action_taken_on='" + saction_taken_on
                    + "', action_completed_by='" + objMeetingItemLogModels.action_completed_by + "', LogStatus='" + objMeetingItemLogModels.LogStatus + "' where meeting_log_id=" + objMeetingItemLogModels.meeting_log_id;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateItemLog: " + ex.ToString());
            }
            return false;
        }
    }

    public class MeetingModelsList
    {
        public List<MeetingModels> MeetingMList { get; set; }
    }

    public class MeetingItemModelsList
    {
        public List<MeetingItemModels> MeetingItemMList { get; set; }
    }

    public class MeetingItemLogModelsList
    {
        public List<MeetingItemLogModels> MeetingItemLogMList { get; set; }
    }


    public class MeetingTypeModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string ID { get; set; }

        [Required]
        [Display(Name = "Meeting Name")]
        [System.Web.Mvc.Remote("doesMeetingTypeExist", "Meeting", HttpMethod = "POST", ErrorMessage = "Name already exists. Please enter a different Name.")]
        public string MeetingName { get; set; }

     
        [Display(Name = "Meeting Description")]
        public string Description { get; set; }


        internal bool FunAddMeetingType(MeetingTypeModels objMeetingType)
        {
            try
            {
                string sSqlstmt = "insert into t_meetingtype (MeetingName) values('" + objMeetingType.MeetingName + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingType: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDeleteMeetingType(string sTypeID)
        {
            try
            {
                string sSqlstmt = "delete from t_meetingtype where ID=" + sTypeID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteMeetingType: " + ex.ToString());
            }
            return false;
        }

        public MultiSelectList GetmeetingtypeListbox()
        {
            MeetingTypeList Typelist = new MeetingTypeList();
            Typelist.TypeMList = new List<MeetingTypeDetails>();

            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select ID, MeetingName from t_meetingtype");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        MeetingTypeDetails emp = new MeetingTypeDetails()
                        {
                            id = dsEmp.Tables[0].Rows[i]["ID"].ToString(),
                            MeetingName = dsEmp.Tables[0].Rows[i]["MeetingName"].ToString()
                        };

                        Typelist.TypeMList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetmeetingtypeListbox: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.TypeMList, "ID", "MeetingName");
        }

        public string GetMeetingTypeNameById(string TypeId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select MeetingName from t_MeetingType where id='" + TypeId + "'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    return (dsMeeting.Tables[0].Rows[0]["MeetingName"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingTypeNameById: " + ex.ToString());
            }
            return "";
        }

        public bool checkMeetingTypeExists(string sMeetingType)
        {
            try
            {
                string sSqlstmt = "select MeetingName from t_MeetingType where MeetingName='" + sMeetingType + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkMeetingTypeExists: " + ex.ToString());
            }
            return true;
        }
    }

    public class MeetingTypeDetails
    {
        public string id { get; set; }
        public string MeetingName { get; set; }
    }

    public class MeetingTypeList
    {
        public List<MeetingTypeDetails> TypeMList { get; set; }
    }


    public class MeetingAgendaModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string Agenda_Id { get; set; }

        [Required]
        [Display(Name = "Agenda Description")]
        public string Agenda_Desc { get; set; }

        [Required]
        [Display(Name = "Agenda Details")]
        public string Agenda_Details { get; set; }

        [Required]
        [Display(Name = "Meeting Type")]
        public string MeetingType { get; set; }


        internal bool FunAddMeetingAgenda(MeetingAgendaModels objMeetingAgenda)
        {
            try
            {
                string sSqlstmt = "insert into t_meeting_agenda (Agenda_Desc, MeetingType_Id,Agenda_Details) values('" + objMeetingAgenda.Agenda_Desc + "','" + objMeetingAgenda.MeetingType + "','" + objMeetingAgenda.Agenda_Details + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddMeetingAgenda: " + ex.ToString());
            }
            return false;
        }



        internal bool FunUpdateMeetingAgenda(MeetingAgendaModels objMeetingAgenda)
        {
            try
            {
                string sSqlstmt = "update t_meeting_agenda set Agenda_Desc='" + objMeetingAgenda.Agenda_Desc + "',Agenda_Details='" + objMeetingAgenda.Agenda_Details + "' where Agenda_Id='" + objMeetingAgenda.Agenda_Id + "' ";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMeetingAgenda: " + ex.ToString());
            }
            return false;
        }



        internal bool FunDeleteMeetingAgenda(MeetingAgendaModels objMeetingAgenda)
        {
            try
            {
                string sSqlstmt = "delete from t_meeting_agenda  where Agenda_Id='" + objMeetingAgenda.Agenda_Id + "' ";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateMeetingAgenda: " + ex.ToString());
            }
            return false;
        }




        public MultiSelectList GetOnlyMeetingAgendasListbox(string smeeting_ref, string sAgenda_Id)
        {
            MeetingAgendaList Agendalist = new MeetingAgendaList();
            Agendalist.MeetingAgendaMList = new List<MeetingAgendaDetails>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select tma.Agenda_Id, Agenda_Desc from t_meeting_agenda as tma, t_meeting as tmi where meeting_ref='"
                    + smeeting_ref + "' and tma.Agenda_Id in (" + sAgenda_Id + ")");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        MeetingAgendaDetails emp = new MeetingAgendaDetails()
                        {
                            id = dsEmp.Tables[0].Rows[i]["Agenda_Id"].ToString(),
                            Agenda_Desc = dsEmp.Tables[0].Rows[i]["Agenda_Desc"].ToString()
                        };

                        Agendalist.MeetingAgendaMList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendaListbox: " + ex.ToString());
            }
            return new MultiSelectList(Agendalist.MeetingAgendaMList, "ID", "Agenda_Desc");
        }

        public MultiSelectList GetMeetingAgendaListbox(string sMeetingTypeId)
        {
            MeetingAgendaList Agendalist = new MeetingAgendaList();
            Agendalist.MeetingAgendaMList = new List<MeetingAgendaDetails>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select Agenda_Id, Agenda_Desc,Agenda_Details from t_meeting_agenda where MeetingType_Id='" + sMeetingTypeId + "'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        MeetingAgendaDetails emp = new MeetingAgendaDetails()
                        {
                            id = dsEmp.Tables[0].Rows[i]["Agenda_Id"].ToString(),
                            Agenda_Desc = dsEmp.Tables[0].Rows[i]["Agenda_Desc"].ToString()+"_"+ dsEmp.Tables[0].Rows[i]["Agenda_Details"].ToString()
                        };

                        Agendalist.MeetingAgendaMList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendaListbox: " + ex.ToString());
            }
            return new MultiSelectList(Agendalist.MeetingAgendaMList, "ID", "Agenda_Desc");
        }

        public string GetMeetingAgendaNameById(string AgendaId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select Agenda_Desc from t_meeting_agenda where agenda_id='" + AgendaId + "'");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    return (dsMeeting.Tables[0].Rows[0]["Agenda_Desc"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendaNameById: " + ex.ToString());
            }
            return "";
        }


        public string GetOnlyMeetingAgendaById(string AgendaId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select GROUP_CONCAT(m.Agenda_Desc) Agenda_Desc from t_meeting_agenda m where Agenda_Id in (" + AgendaId + ")");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    string sDesc = dsMeeting.Tables[0].Rows[0]["Agenda_Desc"].ToString();
                    if (sDesc != "" && sDesc.Contains(','))
                    {
                        return sDesc.Replace(",", ", ");
                    }
                    return sDesc;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetOnlyMeetingAgendaById: " + ex.ToString());
            }
            return "";
        }

        public string GetMeetingAgendadetailsById(string AgendaId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select GROUP_CONCAT(m.Agenda_Details) Agenda_Details from t_meeting_agenda m where Agenda_Id in (" + AgendaId + ")");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    string sDesc = dsMeeting.Tables[0].Rows[0]["Agenda_Details"].ToString();
                    if (sDesc != "" && sDesc.Contains(','))
                    {
                        return sDesc.Replace(",", ", ");
                    }
                    return sDesc;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendadetailsById: " + ex.ToString());
            }
            return "";
        }

        public string GetMeetingAgendaById(string AgendaId)
        {
            try
            {
                DataSet dsMeeting = objGlobalData.Getdetails("select GROUP_CONCAT(m.Agenda_Desc,' - ',COALESCE(m.Agenda_Details,'')) Agenda_Desc from t_meeting_agenda m where Agenda_Id in (" + AgendaId + ")");
                if (dsMeeting.Tables.Count > 0 && dsMeeting.Tables[0].Rows.Count > 0)
                {
                    string sDesc = dsMeeting.Tables[0].Rows[0]["Agenda_Desc"].ToString();
                    if (sDesc != "" && sDesc.Contains(','))
                    {
                        return sDesc.Replace(",", ", ");
                    }
                    return sDesc;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMeetingAgendaById: " + ex.ToString());
            }
            return "";
        }


    }

    public class MeetingAgendaDetails
    {
        public string id { get; set; }
        public string Agenda_Desc { get; set; }
    }

    public class MeetingAgendaList
    {
        public List<MeetingAgendaDetails> MeetingAgendaMList { get; set; }
    }

}