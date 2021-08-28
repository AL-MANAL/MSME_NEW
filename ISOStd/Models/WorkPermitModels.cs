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
    public class WorkPermitModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_access_permit

        [Display(Name = "Id")]
        public string id_access_permit { get; set; }

        [Display(Name = "Permit No")]
        public string permitno { get; set; }

        [Display(Name = "Permit Requestor")]
        public string requestor { get; set; }

        [Display(Name = "Company")]
        public string company { get; set; }

        [Display(Name = "Contact No")]
        public string contactno { get; set; }

        [Display(Name = "Areas")]
        public string area { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime date_submitted { get; set; }

        [Display(Name = "Date Issued")]
        public DateTime date_issued { get; set; }

        [Display(Name = "Date Expired")]
        public DateTime date_expired { get; set; }

        [Display(Name = "Approval Status")]
        public string approve_status { get; set; }

        [Display(Name = "Approved By")]
        public string approved_by { get; set; }

        public string loggedby { get; set; }

        public DateTime approved_date { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }
        //t_access_permit_in

        [Display(Name = "Id")]
        public string id_access_permit_in { get; set; }

        [Display(Name = "Name")]
        public string pers_name { get; set; }

        [Display(Name = "ID No")]
        public string idno { get; set; }

        [Display(Name = "Date")]
        public DateTime date_in { get; set; }

        //t_access_permit_out

        [Display(Name = "Id")]
        public string id_access_permit_out { get; set; }

        [Display(Name = "Name")]
        public string pers_name_out { get; set; }

        [Display(Name = "ID No")]
        public string idno_out { get; set; }

        [Display(Name = "Date")]
        public DateTime date_out { get; set; }

        [Display(Name = "Work Status")]
        public string work_status { get; set; }

        [Display(Name = "The work area and adjacent areas have been inspected after completion of the work and all hazards have been made safe:")]
        public string work_safe { get; set; }

        [Display(Name = "Additional Comments")]
        public string add_comments { get; set; }

        [Display(Name = "Date of Completion")]
        public DateTime completion_date { get; set; }

        //t_work_permit

        [Display(Name = "Id")]
        public string id_work_permit { get; set; }

        [Display(Name = "Permit Type")]
        public string permit_type { get; set; }

        [Display(Name = "Job Performer")]
        public string job_performer { get; set; }

        [Display(Name = "Contractor")]
        public string contractor { get; set; }

        [Display(Name = "Section")]
        public string section { get; set; }

        [Display(Name = "Equipment")]
        public string equipment { get; set; }

        [Display(Name = "Job No.")]
        public string jobno { get; set; }

        [Display(Name = "WorkOrder")]
        public string workorder { get; set; }

        [Display(Name = "Access Level")]
        public string access_level { get; set; }

        [Display(Name = "No of Persons")]
        public string no_person { get; set; }

        [Display(Name = "Start Date & Time")]
        public DateTime start_date { get; set; }

        [Display(Name = "Finish Date & Time")]
        public DateTime finish_date { get; set; }

        [Display(Name = "No of Days")]
        public string no_days { get; set; }

        [Display(Name = "RP Name")]
        public string resp_pers { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        [Display(Name = "Responsible Section")]
        public string resp_section { get; set; }

        [Display(Name = "Documents")]
        public string upload { get; set; }

        [Display(Name = "Documents / Drawings")]
        public string drawings { get; set; }

        [Display(Name = "Method Statement & RA")]
        public string method_state { get; set; }

        [Display(Name = "Rescue Plan in Place")]
        public string rescue_plan { get; set; }

        [Display(Name = "Simultaneous Work at different levels")]
        public string work_level { get; set; }

        [Display(Name = "Pressure Utilised")]
        public string pressure { get; set; }

        [Display(Name = "Issuing Authority")]
        public string IA { get; set; }

        [Display(Name = "Controls and Comments")]
        public string comments { get; set; }

        [Display(Name = "Type of Work to be Done")]
        public string work_type { get; set; }

        [Display(Name = "Work Permit Location")]
        public string permit_location { get; set; }


        //t_work_in

        [Display(Name = "Id")]
        public string id_work_in { get; set; }

        //t_permittratings

        [Display(Name = "Id")]
        public string id_ratings { get; set; }
        public string id_questions { get; set; }
        public string id_hydro_safety { get; set; }
        public string id_excv_hazard { get; set; }
        public string id_excv_site { get; set; }
        public string id_excv_safety { get; set; }
        public string id_hot_type { get; set; }
        public string id_hot_hazard { get; set; }
        public string id_hot_welfare { get; set; }
        public string id_hot_site { get; set; }
        public string id_hot_safety { get; set; }
        public string id_false_hazard { get; set; }
        public string id_false_site { get; set; }
        public string id_false_safety { get; set; }
        public string id_shaft_hazard { get; set; }
        public string id_shaft_site { get; set; }
        public string id_shaft_safety { get; set; }
        public string id_pneumatic { get; set; }
        public string id_electrical { get; set; }
        public string id_confined { get; set; }

        internal bool FunDeleteWorkPermit(string id_work_permit)
        {
            try
            {
                string sSqlstmt = "update t_work_permit set active=0 where id_work_permit='" + id_work_permit + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteWorkPermit: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteAccessPermit(string id_access_permit)
        {
            try
            {
                string sSqlstmt = "update t_access_permit set active=0 where id_access_permit='" + id_access_permit + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAccessPermit: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddAccessPermit(WorkPermitModels objPermit, WorkPermitModelsList objInList, WorkPermitModelsList objOutList)
        {
            try
            {
                string sSqlstmt = "insert into t_access_permit (requestor,company,contactno,area,location,description,approved_by,loggedby,branch,Department";
                string sFields = "", sFieldValue = "";
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                if (objPermit.date_issued != null && objPermit.date_issued > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", date_issued";
                    sFieldValue = sFieldValue + ", '" + objPermit.date_issued.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                if (objPermit.date_expired != null && objPermit.date_expired > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", date_expired";
                    sFieldValue = sFieldValue + ", '" + objPermit.date_expired.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPermit.requestor + "','" + objPermit.company + "'"
                    + ",'" + objPermit.contactno + "','" + objPermit.area + "','" + objPermit.location + "','" 
                    + objPermit.description + "','" + objPermit.approved_by + "','" + objGlobalData.GetCurrentUserSession().empid 
                    + "','" + objPermit.branch + "','" + objPermit.Department + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_access_permit = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_access_permit))
                {
                    if (id_access_permit > 0 && Convert.ToInt32(objInList.PermitList.Count) > 0)
                    {
                        objInList.PermitList[0].id_access_permit = id_access_permit.ToString();
                        FunAddPersonnelIn(objInList);
                    }

                    if (id_access_permit > 0 && Convert.ToInt32(objOutList.PermitList.Count) > 0)
                    {
                        objOutList.PermitList[0].id_access_permit = id_access_permit.ToString();
                        FunAddPersonnelOut(objOutList);
                    }

                    if (id_access_permit > 0)
                    {

                        DataSet dsData = objGlobalData.GetReportNo("AWP", "", "");
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            permitno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_access_permit set permitno='" + permitno + "' where id_access_permit = '" + id_access_permit + "'";

                        if (objGlobalData.ExecuteQuery(sql))
                        {
                            AccessWorkPermitRequestEmail(objPermit);
                            return true;
                        }
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAudit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAccessPermit(WorkPermitModels objPermit, WorkPermitModelsList objInList, WorkPermitModelsList objOutList)
        {
            try
            {
                string sSqlstmt = "update t_access_permit set requestor='" + objPermit.requestor + "', "
                    + "company='" + objPermit.company + "',contactno='" + objPermit.contactno + "',area='" + objPermit.area + "',location='" + objPermit.location
                    + "',description='" + objPermit.description + "',approved_by='" + objPermit.approved_by
                    + "',branch='" + objPermit.branch + "',Department='" + objPermit.Department + "'";

                if (objPermit.date_issued != null && objPermit.date_issued > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", date_issued='" + objPermit.date_issued.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                if (objPermit.date_expired != null && objPermit.date_expired > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", date_expired='" + objPermit.date_expired.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_access_permit='" + objPermit.id_access_permit + "'";
                int id_access_permit = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_access_permit))
                {
                    if (Convert.ToInt32(objInList.PermitList.Count) > 0)
                    {
                        objInList.PermitList[0].id_access_permit = objPermit.id_access_permit;
                        FunAddPersonnelIn(objInList);
                    }
                    else
                    {
                        FunUpdatePersonnelIn(objPermit.id_access_permit);

                    }
                    if (Convert.ToInt32(objOutList.PermitList.Count) > 0)
                    {
                        objOutList.PermitList[0].id_access_permit = objPermit.id_access_permit;
                        FunAddPersonnelOut(objOutList);
                    }
                    else
                    {
                        FunUpdatePersonnelOut(objPermit.id_access_permit);

                    }
                    AccessWorkPermitRequestEmail(objPermit);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccessPermit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateOutPersAccessPermit(WorkPermitModels objPermit, WorkPermitModelsList objOutList)
        {
            try
            {
                if (Convert.ToInt32(objOutList.PermitList.Count) > 0)
                {
                    objOutList.PermitList[0].id_access_permit = objPermit.id_access_permit;
                    FunAddPersonnelOut(objOutList);
                }
                else
                {
                    FunUpdatePersonnelOut(objPermit.id_access_permit);

                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccessPermit: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdatePersonnelIn(string id_access_permit)
        {
            try
            {
                string sSqlstmt = "delete from t_access_permit_in where id_access_permit='" + id_access_permit + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePersonnelIn: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePersonnelOut(string id_access_permit)
        {
            try
            {
                string sSqlstmt = "delete from t_access_permit_out where id_access_permit='" + id_access_permit + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePersonnelOut: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPersonnelIn(WorkPermitModelsList objInList)
        {
            try
            {
                string sSqlstmt = "delete from t_access_permit_in where id_access_permit='" + objInList.PermitList[0].id_access_permit + "'; ";

                for (int i = 0; i < objInList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_access_permit_in (id_access_permit,pers_name,idno";
                    string sFields = "", sFieldValue = "";
                    if (objInList.PermitList[i].date_in != null && objInList.PermitList[i].date_in > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", date_in";
                        sFieldValue = sFieldValue + ", '" + objInList.PermitList[i].date_in.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objInList.PermitList[0].id_access_permit + "', '" + objInList.PermitList[i].pers_name + "', '" + objInList.PermitList[i].idno + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPersonnelIn: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPersonnelOut(WorkPermitModelsList objOutList)
        {
            try
            {
                string sSqlstmt = "delete from t_access_permit_out where id_access_permit='" + objOutList.PermitList[0].id_access_permit + "'; ";

                for (int i = 0; i < objOutList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_access_permit_out (id_access_permit,pers_name_out,idno_out";
                    string sFields = "", sFieldValue = "";
                    if (objOutList.PermitList[i].date_out != null && objOutList.PermitList[i].date_out > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", date_out";
                        sFieldValue = sFieldValue + ", '" + objOutList.PermitList[i].date_out.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objOutList.PermitList[0].id_access_permit + "', '" + objOutList.PermitList[i].pers_name_out + "', '" + objOutList.PermitList[i].idno_out + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPersonnelOut: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddOutPersAccessPermit(WorkPermitModels objModel, WorkPermitModelsList objOutList)
        {
            try
            {
                if (objOutList.PermitList.Count > 0)
                {
                    string sSqlstmt = "delete from t_access_permit_in where id_access_permit='" + objModel.id_access_permit + "'; ";

                    for (int i = 0; i < objOutList.PermitList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_access_permit_in (id_access_permit,pers_name,idno";
                        string sFields = "", sFieldValue = "";
                        if (objOutList.PermitList[i].date_in != null && objOutList.PermitList[i].date_in > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sFields = sFields + ", date_in";
                            sFieldValue = sFieldValue + ", '" + objOutList.PermitList[i].date_in.ToString("yyyy/MM/dd") + "'";
                        }
                        if (objOutList.PermitList[i].date_out != null && objOutList.PermitList[i].date_out > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sFields = sFields + ", date_out";
                            sFieldValue = sFieldValue + ", '" + objOutList.PermitList[i].date_out.ToString("yyyy/MM/dd") + "'";
                        }

                        sSqlstmt = sSqlstmt + sFields;
                        sSqlstmt = sSqlstmt + ") values('" + objModel.id_access_permit + "', '" + objOutList.PermitList[i].pers_name + "', '" + objOutList.PermitList[i].idno + "'";
                        sSqlstmt = sSqlstmt + sFieldValue + ");";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateInspectionChecklist: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAccessPermitApproveOrReject(string id_access_permit, string sStatus, string comments)
        {
            try
            {
                string sSqlstmt = "update t_access_permit set approve_status ='" + sStatus + "',approver_comment='" + comments + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where id_access_permit='" + id_access_permit + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    AccessWorkPermitApproveEmail(id_access_permit, sStatus, comments);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunTrainingsApproveOrReject: " + ex.ToString());
            }

            return false;
        }

        internal bool AccessWorkPermitRequestEmail(WorkPermitModels objPermit)
        {

            try
            {
                string sInformation = "", sHeader = "", sdate_issued = "", sdate_expired = "";
                string sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objPermit.approved_by);//Employee supervisor
                string sCCList = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);//Emp and logged by
                string sUserName = objGlobalData.GetMultiHrEmpNameById(objPermit.approved_by);
                if (objPermit.date_issued != null && objPermit.date_issued > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sdate_issued = objPermit.date_issued.ToString("yyyy/MM/dd HH:mm:ss");
                }
                if (objPermit.date_expired != null && objPermit.date_expired > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sdate_expired = objPermit.date_expired.ToString("yyyy/MM/dd HH:mm:ss");
                }
                sInformation = "Permit No:'" + objPermit.permitno + "'"
                + " <br />"
                + "Permit Requestor:'" + objPermit.requestor + "'"
                + " <br />"
                + "Company:'" + objPermit.company + "'"
                + " <br />"
                + "Area:'" + objPermit.area + "'"
                + " <br />"
                + "Location:'" + objPermit.location + "'"
                + " <br />"
                + "Date Issued:'" + sdate_issued + "'"
                  + " <br />"
                + "Date Expired:'" + sdate_expired + "'"
                 + " <br />"
                + "Description:'" + objPermit.description + "'";
                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "AccessPermit", sHeader, "", "");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AccessWorkPermitRequestEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool AccessWorkPermitApproveEmail(string id_access_permit, string iStatus, string comments)
        {
            try
            {
                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

                string sSqlstmt = "select id_access_permit,permitno,requestor,company,contactno,area,location,loggedby,description,approved_by,date_issued,date_expired"
                    + " from t_access_permit where id_access_permit='" + id_access_permit + "' ";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    if (iStatus == "2")//rejected 
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["loggedby"].ToString());
                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                        sUserName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["loggedby"].ToString());
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);
                        sInformation = "Access Permit rejected by '" + sUser + "'"
                            + " <br />"
                             + "Permit No:'" + (dsData.Tables[0].Rows[0]["permitno"].ToString()) + "'"
                              + " <br />"
                            + "Comments:'" + comments + "'";
                    }
                    if (iStatus == "1")//approved 
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["loggedby"].ToString());
                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                        sUserName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["loggedby"].ToString());
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);
                        sInformation = "Access Permit approved by '" + sUser + "'"
                            + " <br />"
                             + "Permit No:'" + (dsData.Tables[0].Rows[0]["permitno"].ToString()) + "'"
                              + " <br />"
                            + "Comments:'" + comments + "'";
                    }

                    sHeader = sHeader + sInformation;

                    sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                    sToEmailId = sToEmailId.Trim();
                    sToEmailId = sToEmailId.TrimEnd(',');
                    sToEmailId = sToEmailId.TrimStart(',');

                    sCCList = Regex.Replace(sCCList, ",+", ",");
                    sCCList = sCCList.Trim();
                    sCCList = sCCList.TrimEnd(',');
                    sCCList = sCCList.TrimStart(',');
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "AccessPermitApprove", sHeader, "", "");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AccessWorkPermitApproveEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddWorkPermit(WorkPermitModels objPermit, WorkPermitModelsList objInList, WorkPermitModelsList objHydroList, WorkPermitModelsList objExcvHazardList, WorkPermitModelsList objExcvSiteList, WorkPermitModelsList objExcvSafetyList, WorkPermitModelsList objHotTypeList, WorkPermitModelsList objHotHazardList, WorkPermitModelsList objHotWelfareList, WorkPermitModelsList objHotSiteList, WorkPermitModelsList objHotSafetyList, WorkPermitModelsList objFalseHazardList, WorkPermitModelsList objFalseSiteList, WorkPermitModelsList objFalseSafetyList, WorkPermitModelsList objShaftHazardList, WorkPermitModelsList objShaftSiteList, WorkPermitModelsList objShaftSafetyList, WorkPermitModelsList objPneumaticList, WorkPermitModelsList objElectricalList, WorkPermitModelsList objConfinedList)
        {
            try
            {
                string sFields = "", sFieldValue = "";
                //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_work_permit (permit_type,area,location,job_performer,contractor,contactno,section,equipment,"
                + "jobno,workorder,access_level,no_person,no_days,resp_pers,dept,resp_section,description,upload,pressure,IA,drawings,method_state,rescue_plan,work_level,comments,work_type,loggedby,branch,permit_location";
                
                if (objPermit.start_date != null && objPermit.start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", start_date";
                    sFieldValue = sFieldValue + ", '" + objPermit.start_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (objPermit.finish_date != null && objPermit.finish_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", finish_date";
                    sFieldValue = sFieldValue + ", '" + objPermit.finish_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPermit.permit_type + "','" + objPermit.area + "'"
                    + ",'" + objPermit.location + "','" + objPermit.job_performer + "','" + objPermit.contractor + "','" + objPermit.contactno + "','" + objPermit.section + "'"
                    + ",'" + objPermit.equipment + "','" + objPermit.jobno + "','" + objPermit.workorder + "','" + objPermit.access_level + "','" + objPermit.no_person + "','" + objPermit.no_days + "','" + objPermit.resp_pers + "'"
                    + ",'" + objPermit.dept + "','" + objPermit.resp_section + "','" + objPermit.description + "','" + objPermit.upload + "','" + objPermit.pressure + "','" + objPermit.IA + "','" + objPermit.drawings + "'"
                    + ",'" + objPermit.method_state + "','" + objPermit.rescue_plan + "','" + objPermit.work_level + "','" + objPermit.comments + "','" + objPermit.work_type + "'"
                    + ",'" + objGlobalData.GetCurrentUserSession().empid + "','" + objPermit.branch + "','" + objPermit.permit_location + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_work_permit = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_work_permit))
                {
                    if (id_work_permit > 0 && Convert.ToInt32(objInList.PermitList.Count) > 0)
                    {
                        objInList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddWorkPersonnelIn(objInList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objHydroList.PermitList.Count) > 0)
                    {
                        objHydroList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHydroSafetyList(objHydroList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objExcvHazardList.PermitList.Count) > 0)
                    {
                        objExcvHazardList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddExcavationHazardList(objExcvHazardList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objExcvSiteList.PermitList.Count) > 0)
                    {
                        objExcvSiteList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddExcavationSiteList(objExcvSiteList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objExcvSafetyList.PermitList.Count) > 0)
                    {
                        objExcvSafetyList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddExcavationSafetyList(objExcvSafetyList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objHotTypeList.PermitList.Count) > 0)
                    {
                        objHotTypeList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHotTypeList(objHotTypeList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objHotHazardList.PermitList.Count) > 0)
                    {
                        objHotHazardList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHotHazardList(objHotHazardList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objHotWelfareList.PermitList.Count) > 0)
                    {
                        objHotWelfareList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHotWelfareList(objHotWelfareList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objHotSiteList.PermitList.Count) > 0)
                    {
                        objHotSiteList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHotSiteList(objHotSiteList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objHotSafetyList.PermitList.Count) > 0)
                    {
                        objHotSafetyList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddHotSafetyList(objHotSafetyList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objFalseHazardList.PermitList.Count) > 0)
                    {
                        objFalseHazardList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddFalseHazardList(objFalseHazardList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objFalseSiteList.PermitList.Count) > 0)
                    {
                        objFalseSiteList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddFalseSiteList(objFalseSiteList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objFalseSafetyList.PermitList.Count) > 0)
                    {
                        objFalseSafetyList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddFalseSafetyList(objFalseSafetyList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objShaftHazardList.PermitList.Count) > 0)
                    {
                        objShaftHazardList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddShaftHazardList(objShaftHazardList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objShaftSiteList.PermitList.Count) > 0)
                    {
                        objShaftSiteList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddShaftSiteList(objShaftSiteList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objShaftSafetyList.PermitList.Count) > 0)
                    {
                        objShaftSafetyList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddShaftSafetyList(objShaftSafetyList);
                    }

                    if (id_work_permit > 0 && Convert.ToInt32(objPneumaticList.PermitList.Count) > 0)
                    {
                        objPneumaticList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddPneumaticList(objPneumaticList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objElectricalList.PermitList.Count) > 0)
                    {
                        objElectricalList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddElectricalList(objElectricalList);
                    }
                    if (id_work_permit > 0 && Convert.ToInt32(objConfinedList.PermitList.Count) > 0)
                    {
                        objConfinedList.PermitList[0].id_work_permit = id_work_permit.ToString();
                        FunAddConfinedList(objConfinedList);
                    }
                    if (id_work_permit > 0)
                    {
                        objPermit.id_work_permit = id_work_permit.ToString();
                        FunAddPermitNo(objPermit);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAudit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateWorkPermit(WorkPermitModels objPermit, WorkPermitModelsList objInList, WorkPermitModelsList objHydroList, WorkPermitModelsList objExcvHazardList, WorkPermitModelsList objExcvSiteList, WorkPermitModelsList objExcvSafetyList, WorkPermitModelsList objHotTypeList, WorkPermitModelsList objHotHazardList, WorkPermitModelsList objHotWelfareList, WorkPermitModelsList objHotSiteList, WorkPermitModelsList objHotSafetyList, WorkPermitModelsList objFalseHazardList, WorkPermitModelsList objFalseSiteList, WorkPermitModelsList objFalseSafetyList, WorkPermitModelsList objShaftHazardList, WorkPermitModelsList objShaftSiteList, WorkPermitModelsList objShaftSafetyList, WorkPermitModelsList objPneumaticList, WorkPermitModelsList objElectricalList, WorkPermitModelsList objConfinedList)
        {
            try
            {
                //string sSqlstmt = "update t_work_permit set permitno ='" + objPermit.permitno + "', job_performer='" + objPermit.job_performer + "', "
                //     + "contractor='" + objPermit.contractor + "',contactno='" + objPermit.contactno + "',area='" + objPermit.area + "',location='" + objPermit.location + "',description='" + objPermit.description + "',section='" + objPermit.section + "',equipment='" + objPermit.equipment + "',jobno='" + objPermit.jobno + "',workorder='" + objPermit.workorder + "',access_level='" + objPermit.access_level + "',no_person='" + objPermit.no_person + "',no_days='" + objPermit.no_days + "',resp_pers='" + objPermit.resp_pers + "',dept='" + objPermit.dept + "',resp_section='" + objPermit.resp_section + "',upload='" + objPermit.upload + "',pressure='" + objPermit.pressure + "'"
                //     + ",IA='" + objPermit.IA + "',drawings='" + objPermit.drawings + "',method_state='" + objPermit.method_state + "',rescue_plan='" + objPermit.rescue_plan + "',work_level='" + objPermit.work_level + "',comments='" + objPermit.comments + "',work_status='" + objPermit.work_status + "',work_safe='" + objPermit.work_safe + "',add_comments='" + objPermit.add_comments + "',work_type='" + objPermit.work_type + "'";

                string sSqlstmt = "update t_work_permit set job_performer='" + objPermit.job_performer + "', "
                    + "contractor='" + objPermit.contractor + "',contactno='" + objPermit.contactno + "',area='" + objPermit.area + "',description='" + objPermit.description
                    + "',section='" + objPermit.section + "',equipment='" + objPermit.equipment + "',jobno='" + objPermit.jobno + "',workorder='" + objPermit.workorder + "',access_level='" + objPermit.access_level 
                    + "',no_person='" + objPermit.no_person + "',no_days='" + objPermit.no_days + "',resp_pers='" + objPermit.resp_pers + "',dept='" + objPermit.dept 
                    + "',resp_section='" + objPermit.resp_section + "',upload='" + objPermit.upload + "',pressure='" + objPermit.pressure + "'"
                    + ",IA='" + objPermit.IA + "',drawings='" + objPermit.drawings + "',method_state='" + objPermit.method_state + "',rescue_plan='" + objPermit.rescue_plan + "',work_level='" + objPermit.work_level 
                    + "',comments='" + objPermit.comments + "',work_status='" + objPermit.work_status + "',work_safe='" + objPermit.work_safe + "',add_comments='" + objPermit.add_comments
                    + "',work_type='" + objPermit.work_type + "',branch='" + objPermit.branch + "',location='" + objPermit.location + "'";


                if (objPermit.start_date != null && objPermit.start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", start_date='" + objPermit.start_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (objPermit.finish_date != null && objPermit.finish_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", finish_date='" + objPermit.finish_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (objPermit.completion_date != null && objPermit.completion_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", completion_date='" + objPermit.completion_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_work_permit='" + objPermit.id_work_permit + "'";

                int id_work_permit = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_work_permit))
                {
                    if (Convert.ToInt32(objInList.PermitList.Count) > 0)
                    {
                        objInList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddWorkPersonnelIn(objInList);
                    }
                    if (Convert.ToInt32(objHydroList.PermitList.Count) > 0)
                    {
                        objHydroList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHydroSafetyList(objHydroList);
                    }
                    if (Convert.ToInt32(objExcvHazardList.PermitList.Count) > 0)
                    {
                        objExcvHazardList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddExcavationHazardList(objExcvHazardList);
                    }
                    if (Convert.ToInt32(objExcvSiteList.PermitList.Count) > 0)
                    {
                        objExcvSiteList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddExcavationSiteList(objExcvSiteList);
                    }
                    if (Convert.ToInt32(objExcvSafetyList.PermitList.Count) > 0)
                    {
                        objExcvSafetyList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddExcavationSafetyList(objExcvSafetyList);
                    }

                    if (Convert.ToInt32(objHotTypeList.PermitList.Count) > 0)
                    {
                        objHotTypeList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHotTypeList(objHotTypeList);
                    }
                    if (Convert.ToInt32(objHotHazardList.PermitList.Count) > 0)
                    {
                        objHotHazardList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHotHazardList(objHotHazardList);
                    }
                    if (Convert.ToInt32(objHotWelfareList.PermitList.Count) > 0)
                    {
                        objHotWelfareList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHotWelfareList(objHotWelfareList);
                    }
                    if (Convert.ToInt32(objHotSiteList.PermitList.Count) > 0)
                    {
                        objHotSiteList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHotSiteList(objHotSiteList);
                    }
                    if (Convert.ToInt32(objHotSafetyList.PermitList.Count) > 0)
                    {
                        objHotSafetyList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddHotSafetyList(objHotSafetyList);
                    }

                    if (Convert.ToInt32(objFalseHazardList.PermitList.Count) > 0)
                    {
                        objFalseHazardList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddFalseHazardList(objFalseHazardList);
                    }
                    if (Convert.ToInt32(objFalseSiteList.PermitList.Count) > 0)
                    {
                        objFalseSiteList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddFalseSiteList(objFalseSiteList);
                    }
                    if (Convert.ToInt32(objFalseSafetyList.PermitList.Count) > 0)
                    {
                        objFalseSafetyList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddFalseSafetyList(objFalseSafetyList);
                    }

                    if (Convert.ToInt32(objShaftHazardList.PermitList.Count) > 0)
                    {
                        objShaftHazardList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddShaftHazardList(objShaftHazardList);
                    }
                    if (Convert.ToInt32(objShaftSiteList.PermitList.Count) > 0)
                    {
                        objShaftSiteList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddShaftSiteList(objShaftSiteList);
                    }
                    if (Convert.ToInt32(objShaftSafetyList.PermitList.Count) > 0)
                    {
                        objShaftSafetyList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddShaftSafetyList(objShaftSafetyList);
                    }

                    if (Convert.ToInt32(objPneumaticList.PermitList.Count) > 0)
                    {
                        objPneumaticList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddPneumaticList(objPneumaticList);
                    }
                    if (Convert.ToInt32(objElectricalList.PermitList.Count) > 0)
                    {
                        objElectricalList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddElectricalList(objElectricalList);
                    }
                    if (Convert.ToInt32(objConfinedList.PermitList.Count) > 0)
                    {
                        objConfinedList.PermitList[0].id_work_permit = objPermit.id_work_permit.ToString();
                        FunAddConfinedList(objConfinedList);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateWorkPermit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPermitNo(WorkPermitModels objPermit)
        {
            try
            {
                DataSet dsData; string Location = "";
                //string permit_type = objPermit.getper(objPermit.permit_type);
                int space1 = permit_type.IndexOf(' ');
                string PermitType = objPermit.permit_type.Substring(0, space1);

                int LocSpace = objPermit.location.IndexOf(' ');
                if (LocSpace > 0)
                {
                    Location = objPermit.location.Substring(0, LocSpace);
                }
                else
                {
                    Location = objPermit.location;
                }

                dsData = objGlobalData.GetReportNo("WP", PermitType, Location);
                if (dsData != null && dsData.Tables.Count > 0)
                {
                    permitno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                }
                string sSqlstmt = "update t_work_permit set permitno ='" + permitno + "' where id_work_permit='" + objPermit.id_work_permit + "';";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddElectricalList: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddWorkPersonnelIn(WorkPermitModelsList objInList)
        {
            try
            {
                string sSqlstmt = "delete from t_work_in where id_work_permit='" + objInList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objInList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_work_in (id_work_permit,pers_name,idno";
                    string sFields = "", sFieldValue = "";
                    if (objInList.PermitList[i].date_in != null && objInList.PermitList[i].date_in > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", date_in";
                        sFieldValue = sFieldValue + ", '" + objInList.PermitList[i].date_in.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objInList.PermitList[0].id_work_permit + "', '" + objInList.PermitList[i].pers_name + "', '" + objInList.PermitList[i].idno + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddWorkPersonnelIn: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHydroSafetyList(WorkPermitModelsList objHydroList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hydro_safety where id_work_permit='" + objHydroList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHydroList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hydro_safety (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";


                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHydroList.PermitList[0].id_work_permit + "', '" + objHydroList.PermitList[i].id_questions + "', '" + objHydroList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHydroSafetyList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExcavationHazardList(WorkPermitModelsList objExcvHazardList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_excv_hazard where id_work_permit='" + objExcvHazardList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objExcvHazardList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_excv_hazard (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";


                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objExcvHazardList.PermitList[0].id_work_permit + "', '" + objExcvHazardList.PermitList[i].id_questions + "', '" + objExcvHazardList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExcavationHazardList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExcavationSiteList(WorkPermitModelsList objExcvSiteList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_excv_site where id_work_permit='" + objExcvSiteList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objExcvSiteList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_excv_site (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objExcvSiteList.PermitList[0].id_work_permit + "', '" + objExcvSiteList.PermitList[i].id_questions + "', '" + objExcvSiteList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExcavationSiteList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExcavationSafetyList(WorkPermitModelsList objExcvSafetyList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_excv_safety where id_work_permit='" + objExcvSafetyList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objExcvSafetyList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_excv_safety (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objExcvSafetyList.PermitList[0].id_work_permit + "', '" + objExcvSafetyList.PermitList[i].id_questions + "', '" + objExcvSafetyList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExcavationSafetyList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHotTypeList(WorkPermitModelsList objHotTypeList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hot_type where id_work_permit='" + objHotTypeList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHotTypeList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hot_type (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHotTypeList.PermitList[0].id_work_permit + "', '" + objHotTypeList.PermitList[i].id_questions + "', '" + objHotTypeList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHotTypeList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHotHazardList(WorkPermitModelsList objHotHazardList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hot_hazard where id_work_permit='" + objHotHazardList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHotHazardList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hot_hazard (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHotHazardList.PermitList[0].id_work_permit + "', '" + objHotHazardList.PermitList[i].id_questions + "', '" + objHotHazardList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHotHazardList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHotWelfareList(WorkPermitModelsList objHotWelfareList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hot_welfare where id_work_permit='" + objHotWelfareList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHotWelfareList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hot_welfare (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHotWelfareList.PermitList[0].id_work_permit + "', '" + objHotWelfareList.PermitList[i].id_questions + "', '" + objHotWelfareList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHotWelfareList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHotSiteList(WorkPermitModelsList objHotSiteList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hot_site where id_work_permit='" + objHotSiteList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHotSiteList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hot_site (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHotSiteList.PermitList[0].id_work_permit + "', '" + objHotSiteList.PermitList[i].id_questions + "', '" + objHotSiteList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHotSiteList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHotSafetyList(WorkPermitModelsList objHotSafetyList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_hot_safety where id_work_permit='" + objHotSafetyList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objHotSafetyList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_hot_safety (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objHotSafetyList.PermitList[0].id_work_permit + "', '" + objHotSafetyList.PermitList[i].id_questions + "', '" + objHotSafetyList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHotSafetyList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddFalseHazardList(WorkPermitModelsList objFalseHazardList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_false_hazard where id_work_permit='" + objFalseHazardList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objFalseHazardList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_false_hazard (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objFalseHazardList.PermitList[0].id_work_permit + "', '" + objFalseHazardList.PermitList[i].id_questions + "', '" + objFalseHazardList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddFalseHazardList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddFalseSiteList(WorkPermitModelsList objFalseSiteList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_false_site where id_work_permit='" + objFalseSiteList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objFalseSiteList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_false_site (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objFalseSiteList.PermitList[0].id_work_permit + "', '" + objFalseSiteList.PermitList[i].id_questions + "', '" + objFalseSiteList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddFalseSiteList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddFalseSafetyList(WorkPermitModelsList objFalseSafetyList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_false_safety where id_work_permit='" + objFalseSafetyList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objFalseSafetyList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_false_safety (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objFalseSafetyList.PermitList[0].id_work_permit + "', '" + objFalseSafetyList.PermitList[i].id_questions + "', '" + objFalseSafetyList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddFalseSafetyList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddShaftHazardList(WorkPermitModelsList objShaftHazardList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_shaft_hazard where id_work_permit='" + objShaftHazardList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objShaftHazardList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_shaft_hazard (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objShaftHazardList.PermitList[0].id_work_permit + "', '" + objShaftHazardList.PermitList[i].id_questions + "', '" + objShaftHazardList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddShaftHazardList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddShaftSiteList(WorkPermitModelsList objShaftSiteList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_shaft_site where id_work_permit='" + objShaftSiteList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objShaftSiteList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_shaft_site (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objShaftSiteList.PermitList[0].id_work_permit + "', '" + objShaftSiteList.PermitList[i].id_questions + "', '" + objShaftSiteList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddShaftSiteList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddShaftSafetyList(WorkPermitModelsList objShaftSafetyList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_shaft_safety where id_work_permit='" + objShaftSafetyList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objShaftSafetyList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_shaft_safety (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objShaftSafetyList.PermitList[0].id_work_permit + "', '" + objShaftSafetyList.PermitList[i].id_questions + "', '" + objShaftSafetyList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddShaftSafetyList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPneumaticList(WorkPermitModelsList objPneumaticList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_pneumatic where id_work_permit='" + objPneumaticList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objPneumaticList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_pneumatic (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objPneumaticList.PermitList[0].id_work_permit + "', '" + objPneumaticList.PermitList[i].id_questions + "', '" + objPneumaticList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPneumaticList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddElectricalList(WorkPermitModelsList objElectricalList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_electrical where id_work_permit='" + objElectricalList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objElectricalList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_electrical (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objElectricalList.PermitList[0].id_work_permit + "', '" + objElectricalList.PermitList[i].id_questions + "', '" + objElectricalList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddElectricalList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddConfinedList(WorkPermitModelsList objConfinedList)
        {
            try
            {
                string sSqlstmt = "delete from t_permit_confined where id_work_permit='" + objConfinedList.PermitList[0].id_work_permit + "'; ";

                for (int i = 0; i < objConfinedList.PermitList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_permit_confined (id_work_permit,id_questions,id_ratings";
                    string sFields = "", sFieldValue = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objConfinedList.PermitList[0].id_work_permit + "', '" + objConfinedList.PermitList[i].id_questions + "', '" + objConfinedList.PermitList[i].id_ratings + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddElectricalList: " + ex.ToString());
            }
            return false;
        }

        //public string getShortPermitTypeByName(string sPermitType)
        //{
        //    try
        //    {
        //        string sql = "select id_permit_type as Id,permit_type as Name from t_permit_type where permit_type='" + sPermitType + "'";
        //        DataSet dsEmp = objGlobalData.Getdetails(sql);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in getShortPermitTypeByName: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public string getShortWorkPermitLocationById(string sLike_id)
        //{
        //    try
        //    {
        //        DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //              + "and header_desc='Work Permit Location' and (item_id='" + sLike_id + "' or item_desc='" + sLike_id + "')");
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in getShortWorkPermitLocationById: " + ex.ToString());
        //    }
        //    return "";
        //}
    }

    public class WorkPermitModelsList
    {
        public List<WorkPermitModels> PermitList { get; set; }
    }
}