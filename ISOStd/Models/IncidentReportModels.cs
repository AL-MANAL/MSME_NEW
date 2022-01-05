using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ISOStd.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class IncidentReportModels
    {
        clsGlobal objGlobalData = new clsGlobal();
               
        [Display(Name = "ID")]
        public string Incident_Id { get; set; }

        [Display(Name = "Accident No.")]
        public string Incident_Num { get; set; }
        
        [Display(Name = "Reported On")]
        public DateTime Reported_On { get; set; }

        [Display(Name = "Prepared By")]
        public string PreparedBy { get; set; }

        [Display(Name = "Accident Type")]
        public string Incident_Type { get; set; }

        [Display(Name = "Date & Time")]
        public DateTime Incident_Datetime { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Is incident occurred while on job?")]
        public string Occurred_onJob { get; set; }
        
        [Display(Name = "Is incident occurred within the work premises?")]
        public string InWork_Premises { get; set; }
        
        [Display(Name = "Accident witnessed by (name and other detail)")]
        public string WitnessedBy { get; set; }
        
        [Display(Name = "Upload the witness statement")]
        public string Witness_StmtDoc { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description of an Incident (Specify the situation at the time of incident, number of personnel involved, number of equipment involved):")]
        public string Incident_Desc { get; set; }
          
        [DataType(DataType.MultilineText)]
        [Display(Name = "Consequences of an Accident:")]
        public string Consequences { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Any damages to the property (equipment / infrastructure)?")]
        public string Damages { get; set; }

        [Display(Name = "Nature of Injury")]
        public string Injury_Nature { get; set; }

        [Display(Name = "Number of personnel with minor injuries (Only First Aid Only but no LTI)")]
        public string Minor_InjuriesNo { get; set; }

        [Display(Name = "Number of personnel with minor injuries (Treatment in the clinic and there is LTI case)")]
        public string Minor_Injuries_LTINo { get; set; }
        
        [Display(Name = "Number of personnel with major injuries (ong term LTI and / or permanent disability)")]
        public string Major_InjuriesNo { get; set; }

        [Display(Name = "Number of fatalities")]
        public int FatalitiesNo { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Immediate actions taken(containment action)")]
        public string Actions_Taken { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Reasons for Accident Occurrence")]
        public string Incident_Reasons { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Corrective measures proposed (specify the resources required)")]
        public string Corrective_Measures { get; set; }

        [Display(Name = "Corrective measures reviewed and approved by")]
        public string Corrective_ApprovedBy_ReviewedBy { get; set; }
        
        [Display(Name = "Due date for the implementation of Corrective measures")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Report closed on")]
        public DateTime ClosedOn { get; set; }

        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }
        
        [Display(Name = "Logged Date")]
        public DateTime Loggeddate { get; set; }

        [Display(Name = "Accident Type Select")]
        public string AccidentTypeSelect { get; set; }

        [Display(Name = "Emp_Id Select")]
        public string Emp_IdSelect { get; set; }

        [Display(Name = "LTI_Hrs")]
        public string LTI_Hrs { get; set; }
        
        [Display(Name = "Injuries")]
        public string Injuries { get; set; }

        [Display(Name = "CAPA")]
        public string CAPA { get; set; }

        [Display(Name = "CAPA Status")]
        public string CAPA_Status { get; set; }

        [Display(Name = "Incident Cost")]
        public string incident_cost { get; set; }
       
        [Display(Name = "Incident Report To HSE")]
        public string ReportToHSE { get; set; }

        [Display(Name = "Accident Report No")]
        public string accident_reportno { get; set; }

        [Display(Name = "Risk due to Accident")]
        public string risk_incident { get; set; }

        [Display(Name = "HSE officer")]
        public string hse_officer { get; set; }

        [Display(Name = "Witness statement")]
        public string witness_stmt { get; set; }

        [Display(Name = "Investigation team")]
        public string invest_team { get; set; }

        [Display(Name = "Report")]
        public string report_upload { get; set; }

        [Display(Name = "External Witness")]
        public string ext_witness { get; set; }

        [Display(Name = "Id")]
        public string id_incident_action { get; set; }

        [Display(Name = "Action to be taken")]
        public string incident_action { get; set; }

        [Display(Name = "Responsible Person")]
        public string resp_pers { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        [Display(Name = "Action Status")]
        public string incident_status { get; set; }

        [Display(Name = "Contractor")]
        public string contractor { get; set; }

        [Display(Name = "Report")]
        public string action_report { get; set; }

        [Display(Name = "Id")]
        public string id_incident_closeout { get; set; }

        [Display(Name = "Closed By")]
        public string closed_by { get; set; }

        [Display(Name = "Investigation Result")]
        public string invest_result { get; set; }

        [Display(Name = "Investigation Report")]
        public string invest_upload { get; set; }

        [Display(Name = "Overall Status")]
        public string overall_status { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Details of accident")]
        public string details { get; set; }

        [Display(Name = "Date and timing of Accident")]
        public DateTime acc_date { get; set; }

        [Display(Name = "Date of Accident Report")]
        public DateTime reported_date { get; set; }

        //t_incident_report_lti
        [Display(Name = "Type of injury")]
        public string injury_type { get; set; }

        [Display(Name = "Investigation start date")]
        public DateTime invest_start_date { get; set; }

        [Display(Name = "Investigation end date")]
        public DateTime invest_end_date { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Update Date")]
        public DateTime update_date { get; set; }

        internal bool FunUpdateIncidentAction(string Incident_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_incident_action where Incident_Id='" + Incident_Id + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIncidentAction: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateIncidentCloseStatus(string Incident_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_incident_closeout where Incident_Id='" + Incident_Id + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIncidentCloseStatus: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteIncidentDoc(string sIncident_Id)
        {
            try
            {
                string sSqlstmt = "update t_incident_report set Active=0 where Incident_Id='" + sIncident_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteIncidentDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddIncidentReport(IncidentReportModels objIncidentReport, IncidentLTIModelsList objIncidentLTIModelsList, IncidentReportModelsList objActionList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";               
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_incident_report ( PreparedBy, Incident_Type, Location, Occurred_onJob,"
                + " InWork_Premises, WitnessedBy, Incident_Desc, Consequences, Damages, Injury_Nature, Minor_InjuriesNo, Minor_Injuries_LTINo, "
                + " Major_InjuriesNo, FatalitiesNo, Actions_Taken, Incident_Reasons, Corrective_Measures, Corrective_ApprovedBy_ReviewedBy, LoggedBy,Injuries,CAPA,CAPA_Status,incident_cost,"
                + "ReportToHSE,branch,accident_reportno,risk_incident,hse_officer,witness_stmt,invest_team,report_upload,ext_witness,overall_status";

                if (objIncidentReport.Reported_On > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Reported_On";
                    sValues = sValues + ", '" + objIncidentReport.Reported_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.Incident_Datetime > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Incident_Datetime";
                    sValues = sValues + ", '" + objIncidentReport.Incident_Datetime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.DueDate > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", DueDate";
                    sValues = sValues + ", '" + objIncidentReport.DueDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.ClosedOn > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", ClosedOn";
                    sValues = sValues + ", '" + objIncidentReport.ClosedOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.Loggeddate > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Loggeddate";
                    sValues = sValues + ", '" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.Witness_StmtDoc != null && objIncidentReport.Witness_StmtDoc != "")
                {
                    sColumn = sColumn + ", Witness_StmtDoc";
                    sValues = sValues + ", '" + objIncidentReport.Witness_StmtDoc + "' ";
                }


                sSqlstmt = sSqlstmt + sColumn + ") values('" + objIncidentReport.PreparedBy
                    + "','" + objIncidentReport.Incident_Type + "','" + objIncidentReport.Location + "','" + objIncidentReport.Occurred_onJob + "','" + objIncidentReport.InWork_Premises
                    + "','" + objIncidentReport.WitnessedBy + "','" + objIncidentReport.Incident_Desc + "','" + objIncidentReport.Consequences + "','" + objIncidentReport.Damages
                    + "','" + objIncidentReport.Injury_Nature + "','" + objIncidentReport.Minor_InjuriesNo + "','" + objIncidentReport.Minor_Injuries_LTINo + "','" + objIncidentReport.Major_InjuriesNo
                    + "','" + objIncidentReport.FatalitiesNo + "','" + objIncidentReport.Actions_Taken + "','" + objIncidentReport.Incident_Reasons + "','" + objIncidentReport.Corrective_Measures
                    + "','" + objIncidentReport.Corrective_ApprovedBy_ReviewedBy + "','" + user
                    + "','" + objIncidentReport.Injuries + "','" + objIncidentReport.CAPA + "','" + objIncidentReport.CAPA_Status + "','" + objIncidentReport.incident_cost + "','" + objIncidentReport.ReportToHSE + "','" + sBranch + "'"
                    +",'" + objIncidentReport.accident_reportno + "','" + objIncidentReport.risk_incident + "','" + objIncidentReport.hse_officer + "','" + objIncidentReport.witness_stmt + "','" + objIncidentReport.invest_team + "','" + objIncidentReport.report_upload + "','" + objIncidentReport.ext_witness + "','" + objIncidentReport.overall_status + "'";

                sSqlstmt = sSqlstmt + sValues + ")";
                int iIncidentId = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iIncidentId))
                {
                    if (iIncidentId > 0 && Convert.ToInt32(objIncidentLTIModelsList.lstIncidentLTIModels.Count) > 0)
                    {
                        objIncidentLTIModelsList.lstIncidentLTIModels[0].Incident_Id = iIncidentId.ToString();
                        IncidentLTIModels objIncidentLTIModels = new IncidentLTIModels();
                        objIncidentLTIModels.FunAddIncidentLTI(objIncidentLTIModelsList);
                    }
                    if (iIncidentId > 0 && Convert.ToInt32(objActionList.lstIncidentReportModels.Count) > 0)
                    {
                        objActionList.lstIncidentReportModels[0].Incident_Id = iIncidentId.ToString();
                        FunAddActionList(objActionList);
                    }
                    DataSet dsData = objGlobalData.GetReportNo("INC", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        Incident_Num = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql = "update t_incident_report set Incident_Num='" + Incident_Num + "' where Incident_Id='" + iIncidentId + "'";
                    if (objGlobalData.ExecuteQuery(sql))
                    {
                        SendIncidentEmail(objIncidentReport);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddIncidentReport: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddActionList(IncidentReportModelsList objActionList)
        {
            try
            {
                string sSqlstmt = "set sql_safe_updates=0;delete from t_incident_action where Incident_Id='" + objActionList.lstIncidentReportModels[0].Incident_Id + "'; ";


                for (int i = 0; i < objActionList.lstIncidentReportModels.Count; i++)
                {
                    
                    string sid_incident_action = "null";
                    sSqlstmt = sSqlstmt + "insert into t_incident_action(id_incident_action,Incident_Id,incident_action,resp_pers,incident_status,contractor,action_report";

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    if (objActionList.lstIncidentReportModels[i].id_incident_action != null)
                    {
                        sid_incident_action = objActionList.lstIncidentReportModels[i].id_incident_action;
                    }
                    if (objActionList.lstIncidentReportModels[i].target_date != null && objActionList.lstIncidentReportModels[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", target_date= values(target_date)";
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objActionList.lstIncidentReportModels[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objActionList.lstIncidentReportModels[i].update_date != null && objActionList.lstIncidentReportModels[i].update_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", update_date= values(update_date)";
                        sFields = sFields + ", update_date";
                        sFieldValue = sFieldValue + ", '" + objActionList.lstIncidentReportModels[i].update_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_incident_action + ",'" + objActionList.lstIncidentReportModels[0].Incident_Id + "','" + objActionList.lstIncidentReportModels[i].incident_action + "','" + objActionList.lstIncidentReportModels[i].resp_pers + "'"
                        + ",'" + objActionList.lstIncidentReportModels[i].incident_status + "','" + objActionList.lstIncidentReportModels[i].contractor + "','" + objActionList.lstIncidentReportModels[i].action_report + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_incident_action= values(id_incident_action), Incident_Id= values(Incident_Id), incident_action = values(incident_action), resp_pers= values(resp_pers), incident_status= values(incident_status), contractor= values(contractor), action_report= values(action_report)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddActionList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionList(IncidentReportModelsList objActionList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objActionList.lstIncidentReportModels.Count; i++)
                {

                    sSqlstmt = sSqlstmt+ "update t_incident_action set  incident_status='" + objActionList.lstIncidentReportModels[i].incident_status
                    + "', remarks='" + objActionList.lstIncidentReportModels[i].remarks + "'";

                    if (objActionList.lstIncidentReportModels[i].update_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", update_date='" + objActionList.lstIncidentReportModels[i].update_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                    }

                    sSqlstmt = sSqlstmt + " where id_incident_action='" + objActionList.lstIncidentReportModels[i].id_incident_action + "';";

                }

                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCloseList(IncidentReportModelsList objCloseList)
        {
            try
            {
                string sSqlstmt = "delete from t_incident_closeout where Incident_Id='" + objCloseList.lstIncidentReportModels[0].Incident_Id + "'; ";


                for (int i = 0; i < objCloseList.lstIncidentReportModels.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_incident_closeout(Incident_Id,closed_by,invest_result,invest_upload";

                    string sFieldValue = "", sFields = "";
                  
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objCloseList.lstIncidentReportModels[0].Incident_Id + "', '" + objCloseList.lstIncidentReportModels[i].closed_by + "', '" + objCloseList.lstIncidentReportModels[i].invest_result + "', '" + objCloseList.lstIncidentReportModels[i].invest_upload + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCloseList: " + ex.ToString());
            }
            return false;
        }

        internal bool SendIncidentEmail(IncidentReportModels objReport)
        {

            try
            {
                string sInformation = "", sHeader = "";
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetTopMgmtEmployee());
                string sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetHSEEmployee());
                string sUserName = objGlobalData.GetMultiHrEmpNameById(objGlobalData.GetHSEEmployee());
             
                sInformation = "Incident Reported"
                + " <br />"
               + "Incident No:'" + objReport.Incident_Num + "'"
                +" <br />"
             + "Incident Date and Timing:'" + objReport.Incident_Datetime.ToString("yyyy-MM-dd HH':'mm':'ss") + "'"
               + " <br />"
             + "Description:'" + objReport.Incident_Desc + "'";
                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "incident", sHeader, "", "Initial Incident Reported");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendIncidentEmail: " + ex.ToString());
            }
            return false;
        }
     
        internal bool FunUpdateIncidentReport(IncidentReportModels objIncidentReport, IncidentLTIModelsList objIncidentLTIModelsList, IncidentReportModelsList objActionList, IncidentReportModelsList objCloseList)
        {
            try
            {
                string sSqlstmt = "update t_incident_report set  PreparedBy='" + objIncidentReport.PreparedBy
                    + "', Incident_Type='" + objIncidentReport.Incident_Type + "', Location='" + objIncidentReport.Location + "', Occurred_onJob='" + objIncidentReport.Occurred_onJob
                    + "', InWork_Premises='" + objIncidentReport.InWork_Premises + "', WitnessedBy='" + objIncidentReport.WitnessedBy
                    + "', Incident_Desc='" + objIncidentReport.Incident_Desc + "', Consequences='" + objIncidentReport.Consequences + "', Damages='" + objIncidentReport.Damages
                    + "', Injury_Nature='" + objIncidentReport.Injury_Nature + "', Minor_InjuriesNo='" + objIncidentReport.Minor_InjuriesNo
                    + "', Minor_Injuries_LTINo='" + objIncidentReport.Minor_Injuries_LTINo + "', Major_InjuriesNo='" + objIncidentReport.Major_InjuriesNo
                    + "', FatalitiesNo='" + objIncidentReport.FatalitiesNo + "', Actions_Taken='" + objIncidentReport.Actions_Taken
                    + "', Incident_Reasons='" + objIncidentReport.Incident_Reasons + "', Corrective_Measures='" + objIncidentReport.Corrective_Measures
                    + "', Corrective_ApprovedBy_ReviewedBy='" + objIncidentReport.Corrective_ApprovedBy_ReviewedBy
                    + "', Injuries='" + objIncidentReport.Injuries + "', CAPA='" + objIncidentReport.CAPA + "', CAPA_Status='" + objIncidentReport.CAPA_Status + "', incident_cost='" + objIncidentReport.incident_cost + "', ReportToHSE='" + objIncidentReport.ReportToHSE + "'"
                    + ", accident_reportno='" + objIncidentReport.accident_reportno + "', risk_incident='" + objIncidentReport.risk_incident + "', hse_officer='" + objIncidentReport.hse_officer + "', witness_stmt='" + objIncidentReport.witness_stmt + "', invest_team='" + objIncidentReport.invest_team + "', report_upload='" + objIncidentReport.report_upload + "', ext_witness='" + objIncidentReport.ext_witness + "'"
                    + ", overall_status='" + objIncidentReport.overall_status + "'";

                if (objIncidentReport.Reported_On > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Reported_On='" + objIncidentReport.Reported_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.Incident_Datetime > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Incident_Datetime='" + objIncidentReport.Incident_Datetime.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.DueDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", DueDate='" + objIncidentReport.DueDate.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.ClosedOn > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ClosedOn='" + objIncidentReport.ClosedOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objIncidentReport.Witness_StmtDoc != null && objIncidentReport.Witness_StmtDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", Witness_StmtDoc='" + objIncidentReport.Witness_StmtDoc + "' ";
                }

                sSqlstmt = sSqlstmt + " where Incident_Id='" + objIncidentReport.Incident_Id + "'";

                int iIncidentId = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iIncidentId))
                {
                    //if (iIncidentId > 0 && Convert.ToInt32(objIncidentLTIModelsList.lstIncidentLTIModels.Count) > 0)
                    //{
                    IncidentLTIModels objIncidentLTIModels = new IncidentLTIModels();
                    if (objIncidentLTIModelsList.lstIncidentLTIModels.Count > 0)
                    {
                        objIncidentLTIModelsList.lstIncidentLTIModels[0].Incident_Id = objIncidentReport.Incident_Id;
                      
                        objIncidentLTIModels.FunAddIncidentLTI(objIncidentLTIModelsList);
                    }
                    else
                    {
                         objIncidentLTIModels.FunUpdateIncidentLTI(objIncidentReport.Incident_Id);
                    
                    }
                    if (objActionList.lstIncidentReportModels.Count > 0)
                    {
                        objActionList.lstIncidentReportModels[0].Incident_Id = Incident_Id.ToString();
                        FunAddActionList(objActionList);
                    }
                    else
                    {
                        FunUpdateIncidentAction(Incident_Id);

                    }
                    if (objCloseList.lstIncidentReportModels.Count > 0)
                    {
                        objCloseList.lstIncidentReportModels[0].Incident_Id = Incident_Id.ToString();
                        FunAddCloseList(objCloseList);
                    }
                    else
                    {
                        FunUpdateIncidentCloseStatus(Incident_Id);

                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIncidentReport: " + ex.ToString());
            }
            return false;
        }

        internal string GetIncidentTypeNameById(string sIncident)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Incident Type' and (item_id='" + sIncident + "' or item_desc='" + sIncident + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetIncidentTypeNameById: " + ex.ToString());
            }
            return "";
        }    
     
    }

    public class IncidentLTIModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string LTI_Id { get; set; }

        [Display(Name = "Incident")]
        public string Incident_Id { get; set; }

        [Display(Name = "Accident Type")]
        public string AccidentType { get; set; }

        [Display(Name = "Person Name")]
        public string Emp_Id { get; set; }

        [Display(Name = "LTI Hrs")]
        public string LTI_Hrs { get; set; }

        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        //t_incident_report_lti
        [Display(Name = "Type of injury")]
        public string injury_type { get; set; }

        [Display(Name = "Investigation start date")]
        public DateTime invest_start_date { get; set; }

        [Display(Name = "Investigation end date")]
        public DateTime invest_end_date { get; set; }

        internal bool FunUpdateIncidentLTI(string Incident_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_incident_report_lti where Incident_Id='" + Incident_Id + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIncidentLTI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddIncidentLTI(IncidentLTIModelsList objIncidentLTIList)
        {
            try
            {
                string sSqlstmt = "delete from t_incident_report_lti where Incident_Id='" + objIncidentLTIList.lstIncidentLTIModels[0].Incident_Id + "'; ";

                for (int i = 0; i < objIncidentLTIList.lstIncidentLTIModels.Count; i++)
                {
                    string sLtiid = "null";
                    if (objIncidentLTIList.lstIncidentLTIModels[i].LTI_Id != null)
                    {
                        sLtiid = objIncidentLTIList.lstIncidentLTIModels[i].LTI_Id;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_incident_report_lti (LTI_Id, Incident_Id, AccidentType, Emp_Id, LTI_Hrs,injury_type";
                          string sFieldValue = "", sFields = "", sValue = "", sStatement = ""; ;
                    if (objIncidentLTIList.lstIncidentLTIModels[i].invest_start_date != null && objIncidentLTIList.lstIncidentLTIModels[i].invest_start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", invest_start_date= values(invest_start_date)";
                        sFields = sFields + ", invest_start_date";
                        sFieldValue = sFieldValue + ", '" + objIncidentLTIList.lstIncidentLTIModels[i].invest_start_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objIncidentLTIList.lstIncidentLTIModels[i].invest_end_date != null && objIncidentLTIList.lstIncidentLTIModels[i].invest_end_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", invest_end_date= values(invest_end_date)";
                        sFields = sFields + ", invest_end_date";
                        sFieldValue = sFieldValue + ", '" + objIncidentLTIList.lstIncidentLTIModels[i].invest_end_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sLtiid + ", " + objIncidentLTIList.lstIncidentLTIModels[0].Incident_Id
                    + ",'" + objIncidentLTIList.lstIncidentLTIModels[i].AccidentType + "','" + objIncidentLTIList.lstIncidentLTIModels[i].Emp_Id
                    + "'," + objIncidentLTIList.lstIncidentLTIModels[i].LTI_Hrs + "," + objIncidentLTIList.lstIncidentLTIModels[i].injury_type + "";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " LTI_Id= values(LTI_Id), Incident_Id= values(Incident_Id), AccidentType = values(AccidentType), LTI_Hrs= values(LTI_Hrs), injury_type= values(injury_type)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";

                   
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddIncidentLTI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateIncidentLTI(IncidentLTIModels objIncidentLTI)
        {
            try
            {
                string sSqlstmt = "";

                sSqlstmt = sSqlstmt + "update t_incident_report_lti set AccidentType='" + objIncidentLTI.AccidentType
                    + "', Emp_Id='" + objIncidentLTI.Emp_Id + "', LTI_Hrs='" + objIncidentLTI.LTI_Hrs + "' where LTI_Id='" + objIncidentLTI.LTI_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIncidentLTI: " + ex.ToString());
            }
            return false;
        }
    }
    public class IncidentReportModelsList
    {
        public List<IncidentReportModels> lstIncidentReportModels { get; set; }
    }

    public class IncidentLTIModelsList
    {
        public List<IncidentLTIModels> lstIncidentLTIModels { get; set; }
    }

    public class DropdownIncidentItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownIncidentList
    {
        public List<DropdownIncidentItems> lstIncident { get; set; }
    }
}