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
    public class TrainingsModels
    {       
        clsGlobal objGlobalData = new clsGlobal();
       
        public string TrainingID { get; set; }
      
        [Display(Name = "Attendees")]
        public string Attendees { get; set; }

        [Display(Name = "Training Topic")]
        public string Training_Topic { get; set; }
   
        [Display(Name = "Training Planned Date")]
        public DateTime Training_Planned_Date { get; set; }

        [Display(Name = "Training Start Date")]
        public DateTime Training_Start_Date { get; set; }

        //[Display(Name = "Expected Date of Completion")]
        [Display(Name = "Training End Date")]
        public DateTime Expected_Date_Completion { get; set; }

        [Display(Name = "Expected Duration (Days / Hours)")]
        public string Expected_Duration { get; set; }

        [Display(Name = "Training Requested By")]
        public string Training_Requested_By { get; set; }
      
        [Display(Name = "Justification for Training")]
        public string Reasonfor_Training { get; set; }

        [Display(Name = "Source of Training")]
        public string Sourceof_Training { get; set; }

        [Display(Name = "Internal Trainer(s) Name")]
        public string Trainer_Name { get; set; }
 
        [Display(Name = "Training Venue")]
        public string Venue { get; set; }

        [Display(Name = "Training Status")]
        public string Training_Status { get; set; }

        [Display(Name = "Training Certificate(s) Status")]
        public string Training_Cert_Status { get; set; }

        [Display(Name = "Attendance Sheet")]
        public string Training_Attendance { get; set; }
     
        [Display(Name = "Reasons for Not Completed")]
        public string Reasons_for_Not_Completed { get; set; }
       
        [Display(Name = "Training Re-Schedule Date")]
        public DateTime Training_ReSchedule_Date { get; set; }

        [Display(Name = "Training Effectiveness Evaluation Planned Date")]
        public DateTime Training_Effect_Eval_Plan_Date { get; set; }
          
        [Display(Name = "Actual Training Date")]
        public DateTime Training_Actual_Date { get; set; }
          
        [Display(Name = "Actual Training Completion Date")]
        public DateTime Training_Actual_Completion_Date { get; set; }
             
        [Display(Name = "Training Effectiveness Evaluation Method")]
        public string Training_Effect_Eval_Method { get; set; }
        
        [Display(Name = "Training Effectiveness Evaluation Record Reference")]
        public string Training_Effect_Eval_Record_Ref { get; set; }
        
        [Display(Name = "Training course material record reference")]
        public string Course_Material { get; set; }

        [Display(Name = "Request Status")]
        public string RequestStatus { get; set; }

        [Display(Name = "Planned Date")]
        public DateTime Traning_BeforeDate { get; set; }
            
        [Display(Name = "Approver")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }
        
        [Display(Name = "Department")]
        public string DeptId { get; set; }
        
        [Display(Name = "Topic Content")]
        public string TopicContent { get; set; }
        
        [Display(Name = "Number of People Attending")]
        public string Training_Attended_no { get; set; }
        public int Attended_no { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Service Provider")]
        public string Service_provider { get; set; }
        
        [Display(Name = "Upload")]
        public string Upload { get; set; }

        [Display(Name = "Attendees Upload")]
        public string Attendees_Upload { get; set; }

        [Display(Name = "External Trainer Name")]
        public string Ext_Trainer_Name { get; set; }

        [Display(Name = "Training Number")]
        public string report_no { get; set; }

        public string ApprovedByName { get; set; }
        //TrainingEvents

        internal bool FunUpdateTrainingEvents(TrainingsModels objTrainings)
        {
            try
            {
                int appcount;                

                string sSqlstmt = "update t_trainings set Attendees='" + objTrainings.Attendees 
                    + "', Reasonfor_Training='" + objTrainings.Reasonfor_Training + "', Sourceof_Training='" + objTrainings.Sourceof_Training   
                    + "', DeptId='" + objTrainings.DeptId + "', Sourceof_Training='" + objTrainings.Sourceof_Training
                    + "', Trainer_Name='" + objTrainings.Trainer_Name + "', Venue='" + objTrainings.Venue 
                    + "', Category='" + objTrainings.Category + "', Service_provider='" + objTrainings.Service_provider + "', Ext_Trainer_Name='" + objTrainings.Ext_Trainer_Name
                    + "',Upload = '" + objTrainings.Upload + "', Attendees_Upload = '" + objTrainings.Attendees_Upload + "'";

                if (objTrainings.ApprovedBy != null)
                {
                    if (objTrainings.ApprovedBy.Length > 1)
                    { 
                        string[] name1 = objTrainings.ApprovedBy.Split(',');
                        appcount = name1.Length;
                    }
                    else
                    {
                        appcount = 1;
                    }
                    
                    sSqlstmt = sSqlstmt + ", ApprovedBy = '" + objTrainings.ApprovedBy + "', ApproverCount = '" + appcount + "'";
                }

                if (objTrainings.Training_Start_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Start_Date='" + objTrainings.Training_Start_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Expected_Date_Completion > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Expected_Date_Completion='" + objTrainings.Expected_Date_Completion.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                //if (objTrainings.Training_Effect_Eval_Record_Ref != null && objTrainings.Training_Effect_Eval_Record_Ref != "")
                //{
                //    sSqlstmt = sSqlstmt + ", Training_Effect_Eval_Record_Ref='" + objTrainings.Training_Effect_Eval_Record_Ref + "'";
                //}

                //if (objTrainings.Training_Planned_Date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", Training_Planned_Date='" + objTrainings.Training_Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                //if (objTrainings.Training_ReSchedule_Date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", Training_ReSchedule_Date='" + objTrainings.Training_ReSchedule_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                if (objTrainings.Training_Effect_Eval_Plan_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Effect_Eval_Plan_Date='" + objTrainings.Training_Effect_Eval_Plan_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Training_Actual_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Actual_Date='" + objTrainings.Training_Actual_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objTrainings.Training_Actual_Completion_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Actual_Completion_Date='" + objTrainings.Training_Actual_Completion_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where TrainingID='" + objTrainings.TrainingID + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    //SendTrainingEmail(objTrainings.TrainingID, true, "Training Request Update", "Training Request");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingEvents: " + ex.ToString());
            }
            return false;
        }

        internal bool FunTrainingEventApproveOrReject(string sTrainingID, int sStatus)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;
                string Event_status = "";
                DataSet dsEvent_status = objGlobalData.Getdetails("select item_id from dropdownheader a,dropdownitems b where a.header_id=b.header_id " +
                    "and a.header_desc='Calendar Event Status' and b.item_desc='Close'");
                if (dsEvent_status.Tables.Count > 0 && dsEvent_status.Tables[0].Rows.Count > 0)
                {
                    Event_status=dsEvent_status.Tables[0].Rows[0]["item_id"].ToString();
                }
                if (sStatus == 1)
                {

                    string sSqlstmt1 = "update t_trainings set ApproverCount=ApproverCount-1 where TrainingID='" + sTrainingID + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select ApproverCount,id_event from t_trainings where TrainingID='" + sTrainingID + "'";
                        DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_trainings set RequestStatus ='1', ApprovedDate='" + sApprovedDate + "' where TrainingID='" + sTrainingID + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {

                                    string sSqlstmtEvnt = "update t_event_planner set event_status ="+ Event_status +" where id_event='" + dsManagementChange.Tables[0].Rows[0]["id_event"].ToString() + "'";
                                    objGlobalData.ExecuteQuery(sSqlstmtEvnt);

                                    string Sql1 = "update t_trainings set Approvers= concat(Approvers,',','" + user + "') where TrainingID='" + sTrainingID + "'";
                                    objGlobalData.ExecuteQuery(Sql1);
                                    //SendMgmtApprovalmail(sTrainingID, fsSource, filename, "Internal Document is Approved");
                                    return true;
                                }                               
                                
                            }
                            else
                            {
                                string Sql1 = "update t_trainings set Approvers= concat(Approvers,',','" + user + "')"
                                + " where TrainingID='" + sTrainingID + "'";
                                return objGlobalData.ExecuteQuery(Sql1);
                            }

                        }
                    }
                }
                else
                {
                    string Sql1 = "update t_trainings set RequestStatus='" + sStatus + "',ApprovalRejector= concat(ApprovalRejector,',','" + user + "') where TrainingID='" + sTrainingID + "'";
                    objGlobalData.ExecuteQuery(Sql1);
                    // SendMgmtRejectionmail(sTrainingID, fsSource, filename, "Internal Document Rejected");

                    string sSqlEventid = "Select id_event from t_trainings where TrainingID='" + sTrainingID + "'";
                    DataSet dsEventid = objGlobalData.Getdetails(sSqlEventid);
                    if (dsEventid.Tables.Count > 0 && dsEventid.Tables[0].Rows.Count > 0)
                    {
                        string sSqlstmtEvnts = "update t_event_planner set event_status =" + Event_status + " where id_event='" + dsEventid.Tables[0].Rows[0]["id_event"].ToString() + "'";
                        objGlobalData.ExecuteQuery(sSqlstmtEvnts);
                    }
                    else
                    {
                        objGlobalData.AddFunctionalLog("Exception in FunTrainingEventApproveOrReject");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunTrainingEventApproveOrReject: " + ex.ToString());
            }
            return false;
        }


        //Trainings

        internal bool FunDeleteTrainingDoc(string sTrainingID)
        {
            try
            {
                string sSqlstmt = "update t_trainings set Active=0 where TrainingID='" + sTrainingID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrainingDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddTrainings(TrainingsModels objTrainings)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sRequestedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "insert into t_trainings (Attendees, Training_Topic, Training_Requested_By, Reasonfor_Training, ApprovedBy"
                    + ", DeptId,RequestedDate,TopicContent,Attended_no,branch";

                if (objTrainings.Traning_BeforeDate > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Traning_BeforeDate";
                    sValues = sValues + ", '" + objTrainings.Traning_BeforeDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }


                sSqlstmt = sSqlstmt + sColumn + ") values('" + objTrainings.Attendees + "','" + objTrainings.Training_Topic + "','"
                    + objTrainings.Training_Requested_By + "','" + objTrainings.Reasonfor_Training + "','" + objTrainings.ApprovedBy + "','"
                    + objTrainings.DeptId + "','" + sRequestedDate + "','" + objTrainings.TopicContent + "','" + objTrainings.Training_Attended_no + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sValues + ")";
                int TrainingID = objGlobalData.ExecuteQueryReturnId(sSqlstmt);
                if (TrainingID > 0)
                {
                    SendTrainingEmail(TrainingID.ToString(), true, "Training Request", "Training Request");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTrainings(TrainingsModels objTrainings)
        {
            try
            {
                string sSqlstmt = "update t_trainings set Attendees='" + objTrainings.Attendees + "', Training_Topic='" + objTrainings.Training_Topic
                    + "', Training_Requested_By='" + objTrainings.Training_Requested_By + "', Reasonfor_Training='"
                    + objTrainings.Reasonfor_Training + "', ApprovedBy='" + objTrainings.ApprovedBy + "', TopicContent='" + objTrainings.TopicContent + "', DeptId='" + objTrainings.DeptId + "', Attended_no='" + objTrainings.Training_Attended_no + "'";

                if (objTrainings.Traning_BeforeDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Traning_BeforeDate='" + objTrainings.Traning_BeforeDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                sSqlstmt = sSqlstmt + " where TrainingID='" + objTrainings.TrainingID + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    //SendTrainingEmail(objTrainings.TrainingID, true, "Training Request Update", "Training Request");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunTrainingsApproveOrReject(string sTrainingID, int sStatus)
        {
            try
            {
                string sSqlstmt = "update t_trainings set RequestStatus ='" + sStatus + "', ApprovedDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where TrainingID='" + sTrainingID + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendTrainingEmail(sTrainingID, true, "Training Request Status Update", "Training Request");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunTrainingsApproveOrReject: " + ex.ToString());
            }

            return false;
        }

        internal bool SendTrainingEmail(string sTrainingID, bool bOnlyRequest, string sTitle, string sSubject)
        {
            try
            {
                TrainingsModels objTrainingsModels = new TrainingsModels();
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "SELECT TrainingID, Attendees, DeptId, Training_Topic, Traning_BeforeDate, Training_Requested_By, Reasonfor_Training,"
                + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end) "
                + "as RequestStatus, ApprovedBy from t_trainings where TrainingID='" + sTrainingID + "'";

                if (bOnlyRequest == false)
                {
                    sSqlstmt = "SELECT TrainingID, Attendees, DeptId,  Training_Topic, Training_Planned_Date, Training_Start_Date, Expected_Date_Completion, Expected_Duration,"
                        + "(case when RequestStatus='0' then 'Pending' when RequestStatus='1' then 'Approved' when RequestStatus='2' then 'Rejected' end) "
                        + "as RequestStatus, Training_Requested_By, Reasonfor_Training, Sourceof_Training, Trainer_Name, Venue, Training_Status, Training_Cert_Status, Training_Attendance, "
                        + "Reasons_for_Not_Completed, Training_ReSchedule_Date, Training_Effect_Eval_Plan_Date, Training_Effect_Eval_Method, Training_Effect_Eval_Record_Ref"
                        + ",Training_Actual_Date, Training_Actual_Completion_Date, ApprovedBy,  Traning_BeforeDate  from t_trainings where TrainingID='" + sTrainingID + "'";
                }

                DataSet dsMeetingList = objGlobalData.Getdetails(sSqlstmt);

                if (dsMeetingList.Tables.Count > 0 && dsMeetingList.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sContent = "", aAttachment = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    //if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    //{
                    //    sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    //}

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }                   
                        DateTime dtValue;
                        string dtTraning_BeforeDate="";
                        string sName = "All";
                        string sAttenEmailIds = "";
                        string sToEmailIds = objGlobalData.GetHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["ApprovedBy"].ToString());
                        string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["Training_Requested_By"].ToString());
                        if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString(), out dtValue))
                        {
                            dtTraning_BeforeDate = Convert.ToDateTime(dsMeetingList.Tables[0].Rows[0]["Traning_BeforeDate"].ToString()).ToString("dd/MM/yyyy");
                        }
                        sHeader = "<tr><td ><b>Department:<b></td> <td >"
                            + objGlobalData.GetMultiDeptNameById(dsMeetingList.Tables[0].Rows[0]["DeptId"].ToString()) + "</td></tr>"
                            + "<tr><td ><b>Traning Topic:<b></td> <td >" +objGlobalData.GetDropdownitemById(dsMeetingList.Tables[0].Rows[0]["Training_Topic"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>Traning should happen Before: <b></td> <td >" + dtTraning_BeforeDate + "</td></tr>"

                           + "<tr><td ><b>Requested by:<b></td> <td >" + objGlobalData.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["Training_Requested_By"].ToString())
                           + "</td></tr>"
                        + "<tr><td ><b>Attendees: <b></td> <td >" + objGlobalData.GetMultiHrEmpNameById(dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString()) + "</td></tr>"
                        + "<tr><td ><b>Reason for Training:<b></td> <td >" + (dsMeetingList.Tables[0].Rows[0]["Reasonfor_Training"].ToString()) + "</td></tr>"
                        + "<tr><td ><b>Request Status:<b></td> <td >" + dsMeetingList.Tables[0].Rows[0]["RequestStatus"].ToString() + "</td></tr>"; ;

                        if (bOnlyRequest == false)
                        {
                           

                            sHeader = sHeader + "<tr><td><b>Training Planned Date:<b></td> <td>";
                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_Planned_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }
                            sHeader = sHeader + "<tr><td><b>Expected Duration:<b></td> <td>" + dsMeetingList.Tables[0].Rows[0]["Expected_Duration"].ToString() + "</td></tr>"
                            + "<tr><td><b>Training Start Date: <b></td> <td>";

                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_Start_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }

                            sHeader = sHeader + "<tr><td><b>Expected_Date_Completion:<b></td> <td>";

                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Expected_Date_Completion"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }
                            sHeader = sHeader + "<tr><td ><b>Source of Training: <b></td> <td>" +objTrainingsModels.GetTrainingSourceNameById(dsMeetingList.Tables[0].Rows[0]["Sourceof_Training"].ToString()) + "</td></tr>"
                           + "<tr><td><b>Trainer(s) Name:<b></td> <td>" + (dsMeetingList.Tables[0].Rows[0]["Trainer_Name"].ToString()) + "</td></tr>"
                           + "<tr><td><b>Venue:<b></td> <td>" + dsMeetingList.Tables[0].Rows[0]["Venue"].ToString() + "</td></tr>"
                            + "<tr><td><b>Training Status:<b></td> <td>" +objTrainingsModels.GetTrainingStatusNameById(dsMeetingList.Tables[0].Rows[0]["Training_Status"].ToString()) + "</td></tr>"
                            + "<tr><td><b>Training Cert Status:<b></td> <td>" +objTrainingsModels.GetTrainingCertificateStatusNameById(dsMeetingList.Tables[0].Rows[0]["Training_Cert_Status"].ToString()) + "</td></tr>"
                             + "<tr><td><b>Reasons for Not Completng: <b></td> <td>" + (dsMeetingList.Tables[0].Rows[0]["Reasons_for_Not_Completed"].ToString()) + "</td></tr>"
                            + "<tr><td><b>Training Re-Schedule Date:<b></td> <td>";

                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_ReSchedule_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }

                            sHeader = sHeader + "<tr><td><b>Training Actual Date:<b></td> <td>";
                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_Actual_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }
                            sHeader = sHeader + "<tr><td><b>Training Actual Completion Date:<b></td> <td>";

                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_Actual_Completion_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }

                            sHeader = sHeader + "<tr><td><b>Effective Evaluation Method:<b></td> <td>" +objTrainingsModels.GetTrainingEvaluationNameById( dsMeetingList.Tables[0].Rows[0]["Training_Effect_Eval_Method"].ToString()) + "</td></tr>"
                             + "<tr><td><b>Effective Evaluation Plan Date: <b></td> <td>";

                            if (DateTime.TryParse(dsMeetingList.Tables[0].Rows[0]["Training_Effect_Eval_Plan_Date"].ToString(), out dtValue))
                            {
                                sHeader = sHeader + dtValue.ToString("dd/MM/yyyy") + "</td></tr>";
                            }
                            sAttenEmailIds=objGlobalData.GetMultiHrEmpEmailIdById(dsMeetingList.Tables[0].Rows[0]["Attendees"].ToString());
                           sAttenEmailIds = Regex.Replace(sAttenEmailIds, ",+", ",");
                           sAttenEmailIds = sAttenEmailIds.Trim();
                            sAttenEmailIds = sAttenEmailIds.TrimEnd(',');
                            sAttenEmailIds = sAttenEmailIds.TrimStart(',');
                            sToEmailIds = sToEmailIds + "," + sAttenEmailIds;
                        }
                        sToEmailIds = sToEmailIds.Trim(',');

                        sContent = sContent.Replace("{FromMsg}", "");
                        sContent = sContent.Replace("{UserName}", sName);
                        sContent = sContent.Replace("{Title}", sTitle);
                        sContent = sContent.Replace("{content}", sHeader + sInformation);
                        sContent = sContent.Replace("{message}", "");
                        sContent = sContent.Replace("{extramessage}", "");

                        objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                        return true;
                    }
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendTrainingEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePlanTrainings(TrainingsModels objTrainings)
        {
            try
            {
                string sSqlstmt = "update t_trainings set Expected_Duration='" + objTrainings.Expected_Duration + "', Sourceof_Training='" + objTrainings.Sourceof_Training
                    + "', Trainer_Name='" + objTrainings.Trainer_Name + "', Venue='" + Venue + "', Training_Status='" + objTrainings.Training_Status
                    + "', " + "Training_Cert_Status='" + objTrainings.Training_Cert_Status + "', Reasons_for_Not_Completed='" + objTrainings.Reasons_for_Not_Completed
                    + "', Training_Effect_Eval_Method='" + objTrainings.Training_Effect_Eval_Method + "', Training_Effect_Eval_Record_Ref='"
                    + objTrainings.Training_Effect_Eval_Record_Ref + "', Course_Material='" + objTrainings.Course_Material + "', Attended_no='" + objTrainings.Training_Attended_no  + "'";

                if (objTrainings.Training_Attendance != null && objTrainings.Training_Attendance != "")
                {
                    sSqlstmt = sSqlstmt + ", Training_Attendance='" + objTrainings.Training_Attendance + "'";
                }
                if (objTrainings.Training_Effect_Eval_Record_Ref != null && objTrainings.Training_Effect_Eval_Record_Ref != "")
                {
                    sSqlstmt = sSqlstmt + ", Training_Effect_Eval_Record_Ref='" + objTrainings.Training_Effect_Eval_Record_Ref + "'";
                }
                if (objTrainings.Training_Start_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Start_Date='" + objTrainings.Training_Start_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objTrainings.Training_Planned_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Planned_Date='" + objTrainings.Training_Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Expected_Date_Completion > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Expected_Date_Completion='" + objTrainings.Expected_Date_Completion.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Training_ReSchedule_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_ReSchedule_Date='" + objTrainings.Training_ReSchedule_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Training_Effect_Eval_Plan_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Effect_Eval_Plan_Date='" + objTrainings.Training_Effect_Eval_Plan_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainings.Training_Actual_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Actual_Date='" + objTrainings.Training_Actual_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objTrainings.Training_Actual_Completion_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Training_Actual_Completion_Date='" + objTrainings.Training_Actual_Completion_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where TrainingID=" + objTrainings.TrainingID;

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendTrainingEmail(objTrainings.TrainingID, false, "Training Details", "Training Update");

                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePlanTrainings: " + ex.ToString());
            }
            return false;
        }

        internal MultiSelectList FunGetTrainingList()
        {
            TrainingsList objTrainingsList = new TrainingsList();
            objTrainingsList.TrainingList = new List<Trainings>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select distinct(Training_Topic), TrainingID from t_trainings ");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Trainings training = new Trainings()
                        {
                            TrainingID = Convert.ToInt16(dsEmp.Tables[0].Rows[i]["TrainingID"].ToString()),
                            Training_Topic = dsEmp.Tables[0].Rows[i]["Training_Topic"].ToString()
                        };

                        objTrainingsList.TrainingList.Add(training);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetTrainingList: " + ex.ToString());
            }
            return new MultiSelectList(objTrainingsList.TrainingList, "TrainingID", "Training_Topic");
        }

        public List<string> GetAttendeesList(string TrainingID)
        {
            List<string> lstAttendees = new List<string>();
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Attendees from t_trainings where TrainingID=" + TrainingID);
                string[] sAttendees = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["Attendees"].ToString()).Trim().Split(',');
               lstAttendees = new List<string>(sAttendees);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAttendeesList: " + ex.ToString());
            }
            return lstAttendees;
        }

        public string GetAttendees(string TrainingID)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("SELEct  GROUP_CONCAT(m.Attendees) Attendees FROM t_trainings m  where TrainingID in (" + TrainingID + ")");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Attendees"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAttendees: " + ex.ToString());
            }
            return "";
        }

        public string GetTopic(string TrainingID)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Training_Topic from t_trainings where TrainingID=" + TrainingID);
                return dsData.Tables[0].Rows[0]["Training_Topic"].ToString();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTopic: " + ex.ToString());
            }
            return "";
        }

        internal string GetTrainingSourceNameById(string sSourceID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='Training-SourceList' and (item_id='" + sSourceID + "' or item_desc='" + sSourceID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingSourceNameById: " + ex.ToString());
            }
            return "";
        }
        
        internal string GetTrainingStatusNameById(string sStatusID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='Training Status' and (item_id='" + sStatusID + "' or item_desc='" + sStatusID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiTrainingStatusList()
        {
            DropdownTrainingList lst = new DropdownTrainingList();
            lst.lst = new List<DropdownTrainingItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Training Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownTrainingItems reg = new DropdownTrainingItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiTrainingStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetTrainingCertificateStatusNameById(string sCertID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='Training Certificate Status' and (item_id='" + sCertID + "' or item_desc='" + sCertID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingCertificateStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiTrainingCertificateStatusList()
        {
            DropdownTrainingList lst = new DropdownTrainingList();
            lst.lst = new List<DropdownTrainingItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Training Certificate Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownTrainingItems reg = new DropdownTrainingItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiTrainingCertificateStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetTrainingEvaluationNameById(string sEvalID)
        {
            try
            {
                //DataSet dsEmp = objGlobaldata.Getdetails("select impact_id, impact_name from impact where impact_id='" + sImpact_id + "'");
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='Training Evaluation Method' and (item_id='" + sEvalID + "' or item_desc='" + sEvalID + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingEvaluationNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiTrainingEvaluationList()
        {
            DropdownTrainingList lst = new DropdownTrainingList();
            lst.lst = new List<DropdownTrainingItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Training Evaluation Method' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownTrainingItems reg = new DropdownTrainingItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiTrainingEvaluationList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        //Training Events

    }

    public class TrainingCertificatesModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        public int CertId { get; set; }

        [Required]
        [Display(Name = "Training Id")]
        public string TrainingId { get; set; }

        [Required]
        [Display(Name = "Training Topic")]
        public string Training_Topic { get; set; }

        [Required]
        [Display(Name = "Training Cert Ref")]
        public string Cert_Ref { get; set; }

        [Required]
        [Display(Name = "Person Name")]
        public string Person_Name { get; set; }

        [Required]
        [Display(Name = "Cert Issue Date")]
        public DateTime Cert_Issue_Date { get; set; }

        [Required]
        [Display(Name = "Cert Expiry Date")]
        public DateTime Cert_Exp_Date { get; set; }

        [Required]
        [Display(Name = "Cert Issued By")]
        public string Cert_issued_by { get; set; }

        [Required]
        [Display(Name = "Certificate Upolad")]
        public string Cert_Upload { get; set; }

        public string Person_No { get; set; }

        internal bool FunAddTrainingCertificates(TrainingCertificatesModels objTrainingCertificates)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_training_certificate (TrainingId, Cert_Ref, Person_Name, Cert_issued_by";

                if (objTrainingCertificates.Cert_Issue_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Cert_Issue_Date";
                    sValues = sValues + ", '" + objTrainingCertificates.Cert_Issue_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainingCertificates.Cert_Exp_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Cert_Exp_Date";
                    sValues = sValues + ", '" + objTrainingCertificates.Cert_Exp_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainingCertificates.Cert_Upload != null && objTrainingCertificates.Cert_Upload != "")
                {
                    sColumn = sColumn + ", Cert_Upload";
                    sValues = sValues + ", '" + objTrainingCertificates.Cert_Upload + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objTrainingCertificates.TrainingId + "','" + objTrainingCertificates.Cert_Ref
                 + "','" + objTrainingCertificates.Person_Name + "','" + objTrainingCertificates.Cert_issued_by
                 + "'";

                sSqlstmt = sSqlstmt + sValues + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingCertificates: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTrainingCertificates(TrainingCertificatesModels objTrainingCertificates)
        {
            try
            {
                string sSqlstmt = "update t_training_certificate set TrainingId ='" + objTrainingCertificates.TrainingId + "', Cert_Ref='" + objTrainingCertificates.Cert_Ref + "', "
                    + "Person_Name='" + Person_Name + "', Cert_issued_by='" + objTrainingCertificates.Cert_issued_by + "'";

                if (objTrainingCertificates.Cert_Issue_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Cert_Issue_Date='" + objTrainingCertificates.Cert_Issue_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objTrainingCertificates.Cert_Exp_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Cert_Exp_Date='" + objTrainingCertificates.Cert_Exp_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objTrainingCertificates.Cert_Upload != null && objTrainingCertificates.Cert_Upload != "")
                {
                    sSqlstmt = sSqlstmt + ", Cert_Upload='" + objTrainingCertificates.Cert_Upload + "'";
                }

                sSqlstmt = sSqlstmt + " where CertId=" + objTrainingCertificates.CertId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingCertificates: " + ex.ToString());
            }
            return false;
        }
 
    }

    public class TrainingsModelsList
    {
        public List<TrainingsModels> TrainingsMList { get; set; }
    }

    public class TrainingCertificatesModelsList
    {
        public List<TrainingCertificatesModels> TrainingCertificatesList { get; set; }
    }

    public class Trainings
    {
        public int TrainingID { get; set; }
        public string Training_Topic { get; set; }
    }

    public class TrainingsList
    {
        public List<Trainings> TrainingList { get; set; }
    }

    public class DropdownTrainingItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownTrainingList
    {
        public List<DropdownTrainingItems> lst { get; set; }
    }

}