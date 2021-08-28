using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class RoleModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string Id { get; set; }
        public string role_id { get; set; }
        public string branch_id { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Applicable to all Branches")]
        public string appl_branch { get; set; }

        public string id_role_group { get; set; }

        //-----------------ROLE JD------------------------------------------

        [Display(Name = "Id")]
        public string id_role_jd { get; set; }

        [Display(Name = "Department")]
        public string deptid { get; set; }

        [Display(Name = "Reports to")]
        public string report_to { get; set; }

        [Display(Name = "Supervises")]
        public string supervises { get; set; }

        [Display(Name = "Roles and Responsibilities")]
        public string responsibility { get; set; }

        [Display(Name = "Authorities (What authorities this position has)")]
        public string authorities { get; set; }

        [Display(Name = "Interfaces with internal department")]
        public string interfaces_internal { get; set; }

        [Display(Name = "External Interfaces")]
        public string interfaces_external { get; set; }

        [Display(Name = "Accountable for (liable for the consequences if failed to perform)")]
        public string accountable { get; set; }

        [Display(Name = "Academic qualification")]
        public string academic_mandatory { get; set; }

        [Display(Name = "Academic qualification")]
        public string academic_optional { get; set; }

        [Display(Name = "Trade Certificate")]
        public string trade_mandatory { get; set; }

        [Display(Name = "Trade Certificate")]
        public string trade_optional { get; set; }

        [Display(Name = "Relevant experience")]
        public string experience_mandatory { get; set; }

        [Display(Name = "Relevant experience")]
        public string experience_optional { get; set; }

        [Display(Name = "Relevant trainings")]
        public string trainings_mandatory { get; set; }

        [Display(Name = "Relevant trainings")]
        public string trainings_optional { get; set; }

        [Display(Name = "Skills (Wherever required)")]
        public string skills_mandatory { get; set; }

        [Display(Name = "Skills (Wherever required)")]
        public string skills_optional { get; set; }

        [Display(Name = "Date")]
        public DateTime jd_date { get; set; }

        [Display(Name = "Revised on")]
        public DateTime revised_date { get; set; }

        [Display(Name = "Approved By")]
        public string approved_by { get; set; }
        public string RoleId { get; set; }
        public string jd_report { get; set; }
        public string approvedby { get; set; }

        
        internal bool FunAddRole(RoleModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into roles (RoleName,appl_branch) values('" + objModel.RoleName + "','" + objModel.appl_branch + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRole: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRole(RoleModels objModel)
        {
            try
            {
                string sSqlstmt = "update roles set RoleName = '" + objModel.RoleName + "',appl_branch = '" + objModel.appl_branch + "' where Id=" + Id;
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRole: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteRole(string sId)
        {
            try
            {
                string sSqlstmt = "update roles set Active=0 where Id=" + sId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRole: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddJD(RoleModels objModel)
        {
            try
            {
               
                    string sSqlstmt = "insert into t_role_jd(role_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,accountable,"
                        + "academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,trainings_optional,"
                        + "skills_mandatory,skills_optional,approved_by,logged_by";
                    string sFields = "", sFieldValue = "";

                    if (objModel.jd_date != null && objModel.jd_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", jd_date";
                        sFieldValue = sFieldValue + ", '" + objModel.jd_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objModel.revised_date != null && objModel.revised_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", revised_date";
                        sFieldValue = sFieldValue + ", '" + objModel.revised_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;

                    sSqlstmt = sSqlstmt + ") values('" + objModel.RoleId + "','" + objModel.deptid + "','" + objModel.report_to + "','" + objModel.supervises + "','" + objModel.responsibility +
                        "','" + objModel.authorities + "','" + objModel.interfaces_internal + "','" + objModel.interfaces_external + "','" + objModel.accountable + "'"
                        + ",'" + objModel.academic_mandatory + "','" + objModel.academic_optional + "','" + objModel.trade_mandatory + "','" + objModel.trade_optional + "','" + objModel.experience_mandatory + "','" + objModel.experience_optional + "'"
                        + ",'" + objModel.trainings_mandatory + "','" + objModel.trainings_optional + "','" + objModel.skills_mandatory + "','" + objModel.skills_optional + "','" + objModel.approved_by + "','" + objGlobalData.GetCurrentUserSession().empid + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddJD: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateJD(RoleModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_role_jd set role_id='" + objModel.role_id + "',deptid='" + objModel.deptid + "',report_to='" + objModel.report_to + "',supervises='" + objModel.supervises + "'"
                    + ",responsibility='" + objModel.responsibility + "',authorities='" + objModel.authorities + "',interfaces_internal='" + objModel.interfaces_internal + "',interfaces_external='" + objModel.interfaces_external + "'"
                    + ",accountable='" + objModel.accountable + "',academic_mandatory='" + objModel.academic_mandatory + "',academic_optional='" + objModel.academic_optional + "',trade_mandatory='" + objModel.trade_mandatory + "'"
                + ",trade_optional='" + objModel.trade_optional + "',experience_mandatory='" + objModel.experience_mandatory + "',experience_optional='" + objModel.experience_optional + "',trainings_mandatory='" + objModel.trainings_mandatory + "'"
                   + ",trainings_optional='" + objModel.trainings_optional + "',skills_mandatory='" + objModel.skills_mandatory + "',skills_optional='" + objModel.skills_optional + "',approved_by='" + objModel.approved_by + "'";
                if (objModel.jd_date != null && objModel.jd_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",jd_date='" + objModel.jd_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.revised_date != null && objModel.revised_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",revised_date='" + objModel.revised_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_role_jd='" + objModel.id_role_jd + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
               
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateJD: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateJDReport(RoleModels objModel, HttpPostedFileBase jd_report,string flag)
        {
            try
            {
                string sSqlstmt = "update t_role_jd set jd_report = '" + objModel.jd_report + "' where id_role_jd=" + id_role_jd;
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (flag == "1")
                    {
                        SendJDEmail(objModel, jd_report);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateJDReport: " + ex.ToString());
            }
            return false;
        }

        internal bool SendJDEmail(RoleModels objModel, HttpPostedFileBase jd_report)
        {

            try
            {

                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

                // string sInformation = "", sHeader = "";
                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().CompEmpId);
                sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objModel.approvedby);
                //sUserName = objGlobalData.GetMultiHrEmpNameById(objGlobalData.GetHSEEmployee());

                sInformation =
                "Please find the attached job description for your approval"
                + " <br />"
                + "Role:'" +objModel.role_id + "'";
               
                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "JD", sHeader, "", "Job Description");
                objGlobalData.SendJDmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], jd_report, sCCList, "");
                return true;

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendJDEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunJDApproveOrReject(string id_role_jd, string sStatus, string comments)
        {
            try
            {
                string sSqlstmt = "update t_role_jd set approve_status ='" + sStatus + "',approver_comment='" + comments + "', approved_date='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' where id_role_jd='" + id_role_jd + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                   JDApproveEmail(id_role_jd, sStatus, comments);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunJDApproveOrReject: " + ex.ToString());
            }

            return false;
        }
        internal bool JDApproveEmail(string id_role_jd, string iStatus, string comments)
        {
            try
            {
                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "", sUser = "";

                string sSqlstmt = "select id_role_jd,role_id,deptid,report_to,supervises,approved_by,jd_date,logged_by"
                    + " from t_role_jd where id_role_jd='" + id_role_jd + "' ";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    if (iStatus == "2")//rejected 
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                        sUserName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);
                        sInformation = "Job description rejected by '" + sUser + "'"
                            + " <br />"
                             + "For Role:'" + objGlobalData.GetRolesNameById(dsData.Tables[0].Rows[0]["role_id"].ToString()) + "'"
                              + " <br />"
                            + "Comments:'" + comments + "'";
                    }
                    if (iStatus == "1")//approved 
                    {
                        sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                        sUserName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
                        sUser = objGlobalData.GetEmpHrNameById(objGlobalData.GetCurrentUserSession().empid);
                        sInformation = "Job description approved by '" + sUser + "'"
                            + " <br />"
                             + "For Role:'" + objGlobalData.GetRolesNameById(dsData.Tables[0].Rows[0]["role_id"].ToString()) + "'"
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
                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "JD", sHeader, "", "");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in JDApproveEmail: " + ex.ToString());
            }
            return false;
        }

    }

    public class RoleModelsList
    {
        public List<RoleModels> RoleList { get; set; }
    }
}