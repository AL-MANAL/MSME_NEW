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
    public class JDModels
    {
        clsGlobal objGlobalData = new clsGlobal();


        public string designation_id { get; set; }

        [Display(Name = "Id")]
        public string id_jd { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

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

        [Display(Name = "To be Approved By")]
        public string approvedby { get; set; }

        //t_jd
        [Display(Name = "To be Reviewed By")]
        public string reviewed_by { get; set; }

        public string ReviewRejector { get; set; }
        public string ApprovalRejector { get; set; }
        public string Reviewers { get; set; }
        public string Approvers { get; set; }

        [Display(Name = "Revision no")]
        public string rev_no { get; set; }

        //t_jd_review
        public string id_review { get; set; }
        public string apprv_status { get; set; }
        public string emp_id { get; set; }
        public DateTime apprv_date { get; set; }
        public string comments { get; set; }
        public string approve_status { get; set; }

        

     

        internal bool FunAddJD(JDModels objModel)
        {
            try
            {
                string[] name = objModel.reviewed_by.Split(',');
                int revcount = name.Length;
                string[] name1 = objModel.approved_by.Split(',');
                int appcount = name1.Length;

                string sSqlstmt = "insert into t_jd(designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,accountable,"
                        + "academic_mandatory,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,trainings_optional,"
                        + "skills_mandatory,skills_optional,approved_by,logged_by,reviewed_by,ApproverCount,ReviewerCount,rev_no";
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

                sSqlstmt = sSqlstmt + ") values('" + objModel.designation_id + "','" + objModel.deptid + "','" + objModel.report_to + "','" + objModel.supervises + "','" + objModel.responsibility +
                    "','" + objModel.authorities + "','" + objModel.interfaces_internal + "','" + objModel.interfaces_external + "','" + objModel.accountable + "'"
                    + ",'" + objModel.academic_mandatory + "','" + objModel.academic_optional + "','" + objModel.trade_mandatory + "','" + objModel.trade_optional + "','" + objModel.experience_mandatory + "','" + objModel.experience_optional + "'"
                    + ",'" + objModel.trainings_mandatory + "','" + objModel.trainings_optional + "','" + objModel.skills_mandatory + "','" + objModel.skills_optional + "','" + objModel.approved_by + "','" + objGlobalData.GetCurrentUserSession().empid + "'"
                    + ",'" + reviewed_by + "','" + appcount + "','" + revcount + "','" + rev_no + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";


                int id_jd = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_jd))
                {
                    //reviewer table
                    string[] review_by = objModel.reviewed_by.Split(',');
                    string sql = "";
                    foreach (var item in review_by)
                    {
                        sql = sql + "insert into t_jd_review(id_jd,emp_id) values ('" + id_jd + "','" + item + "');";
                    }
                    objGlobalData.ExecuteQuery(sql);
                    //approver table
                    string[] approved_by = objModel.approved_by.Split(',');
                    sql = "";
                    foreach (var item in approved_by)
                    {
                        sql = sql + "insert into t_jd_approve(id_jd,emp_id) values ('" + id_jd + "','" + item + "');";
                    }
                    return objGlobalData.ExecuteQuery(sql);

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddJD: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateJD(JDModels objModel)
        {
            try
            {
                string[] name = objModel.reviewed_by.Split(',');
                int revcount = name.Length;
                string[] name1 = objModel.approved_by.Split(',');
                int appcount = name1.Length;

                string sSqlstmt = "update t_jd set designation_id='" + objModel.designation_id + "',deptid='" + objModel.deptid + "',report_to='" + objModel.report_to + "',supervises='" + objModel.supervises + "'"
                    + ",responsibility='" + objModel.responsibility + "',authorities='" + objModel.authorities + "',interfaces_internal='" + objModel.interfaces_internal + "',interfaces_external='" + objModel.interfaces_external + "'"
                    + ",accountable='" + objModel.accountable + "',academic_mandatory='" + objModel.academic_mandatory + "',academic_optional='" + objModel.academic_optional + "',trade_mandatory='" + objModel.trade_mandatory + "'"
                + ",trade_optional='" + objModel.trade_optional + "',experience_mandatory='" + objModel.experience_mandatory + "',experience_optional='" + objModel.experience_optional + "',trainings_mandatory='" + objModel.trainings_mandatory + "'"
                   + ",trainings_optional='" + objModel.trainings_optional + "',skills_mandatory='" + objModel.skills_mandatory + "',skills_optional='" + objModel.skills_optional + "',approved_by='" + objModel.approved_by + "',reviewed_by='" + objModel.reviewed_by + "'"
                   + ",ApproverCount='" + appcount + "',Approvers='0',ApprovalRejector='0',ReviewerCount='" + revcount + "',Reviewers='0',ReviewRejector='0',approve_status='0',rev_no='" + objModel.rev_no + "'";
                if (objModel.jd_date != null && objModel.jd_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",jd_date='" + objModel.jd_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.revised_date != null && objModel.revised_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",revised_date='" + objModel.revised_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_jd='" + objModel.id_jd + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string[] review_by = objModel.reviewed_by.Split(',');
                    string sql = "";
                    sql = "delete from t_jd_review where id_jd='" + id_jd + "';";
                    objGlobalData.ExecuteQuery(sql);
                    foreach (var item in review_by)
                    {
                        sql = sql + "insert into t_jd_review(id_jd,emp_id) values ('" + id_jd + "','" + item + "');";
                    }
                    objGlobalData.ExecuteQuery(sql);
                    //approver table
                    sql = "";
                    sql = "delete from t_jd_approve where id_jd='" + id_jd + "';";
                    objGlobalData.ExecuteQuery(sql);
                    string[] approved_by = objModel.approved_by.Split(',');

                    foreach (var item in approved_by)
                    {
                        sql = sql + "insert into t_jd_approve(id_jd,emp_id) values ('" + id_jd + "','" + item + "');";
                    }
                    return objGlobalData.ExecuteQuery(sql);
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateJD: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateJDReport(JDModels objModel, HttpPostedFileBase jd_report, string flag)
        {
            try
            {
                string sSqlstmt = "update t_jd set jd_report = '" + objModel.jd_report + "' where id_jd=" + id_jd;
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    SendJDEmail(id_jd, jd_report, "Job description detail");

                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateJDReport: " + ex.ToString());
            }
            return false;
        }

        internal bool SendJDEmail(string id_jd, HttpPostedFileBase jd_report, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                          + "accountable,academic_mandatory,academic_optional,logged_by,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                          + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by,reviewed_by,rev_no from t_jd where id_jd='" + id_jd + "'";


                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                   
                    string sHeader, sInformation = "", sSubject = "", sContent = "";

                  
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

                    string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reviewed_by"].ToString());

                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());

                    string jd_date = "", revised_date = "";

                    if (dsData.Tables[0].Rows[0]["jd_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        jd_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["revised_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        revised_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()).ToString("yyyy-MM-dd");
                    }

                    sHeader = "<table><tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["designation_id"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Date:<b></td> <td colspan=3>" + jd_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["deptid"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Reports to:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["report_to"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Supervises:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["supervises"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Roles and Responsibilities:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["responsibility"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Authorities (What authorities this position has):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["authorities"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Interfaces with internal department:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_internal"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>External Interfaces:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_external"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Accountable for (liable for the consequences if failed to perform):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["accountable"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Revised on:<b></td> <td colspan=3>" + revised_date + "</td></tr>"

                       + "<tr><td colspan=3><b>Revision no:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["rev_no"].ToString()) + "</td></tr></table>";

                    sInformation = "<table><tr> "
                           + "<td colspan=8><label><b>Minimum Competency Required to hold the position</b></label> </td> </tr>"
                           + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"

                           + "<th style='width:300px'>Competence Parameters</th>"
                           + "<th style='width:300px'>Mandatory</th>"
                           + "<th style='width:300px'>Optional</th>"
                           + "</tr>"
                        + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Academic qualification</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Trade Certificate</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Relevant experience</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Relevant trainings</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Skills (Wherever required)</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_optional"].ToString()) + "</td>"
                      + "</tr></table>";





                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Job description Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
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

                    return objGlobalData.SendJDmail(sToEmailIds, sSubject + sMessage, sContent, "", jd_report, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendJDEmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunJDReviewOrReject(JDModels objModel)
        {
            try
            {
                string sReviewedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sql = "update t_jd_review set apprv_status='" + apprv_status + "',apprv_date='" + sReviewedDate + "',comments='" + comments + "'"
                    + " where emp_id='" + user + "' and id_jd='" + id_jd + "'";
                if (objGlobalData.ExecuteQuery(sql))
                {
                    if (apprv_status == "2")
                    {
                        string sSqlstmt1 = "update t_jd set ReviewerCount=ReviewerCount-1,Reviewers= concat(Reviewers,',','" + user + "') where id_jd='" + id_jd + "'";
                        if (objGlobalData.ExecuteQuery(sSqlstmt1))
                        {
                            string sSqlstmt = "Select ReviewerCount from t_jd where id_jd='" + id_jd + "'";
                            DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                            if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ReviewerCount"].ToString()) == 0)
                                {
                                    string sSSqlstmt = "update t_jd set approve_status ='" + apprv_status + "' where id_jd='" + id_jd + "'";
                                    objGlobalData.ExecuteQuery(sSSqlstmt);
                                    return SendJDReviewEmail(apprv_status, id_jd, "Job description reviewed");
                                }
                            }
                            return true;
                        }

                    }
                    else
                    {
                        string Sql1 = "update t_jd set ReviewerCount=ReviewerCount-1,approve_status='" + apprv_status + "',ReviewRejector= concat(ReviewRejector,',','" + user + "') where id_jd='" + id_jd + "'";
                        objGlobalData.ExecuteQuery(Sql1);
                        return SendJDReviewEmail(apprv_status, id_jd, "Job description review rejected");
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunJDReviewOrReject: " + ex.ToString());
            }
            return false;
        }
        internal bool FunJDApproveOrReject(JDModels objModel)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sql = "update t_jd_approve set apprv_status='" + apprv_status + "',apprv_date='" + sApprovedDate + "',comments='" + comments + "'"
                    + " where emp_id='" + user + "' and id_jd='" + id_jd + "'";
                if (objGlobalData.ExecuteQuery(sql))
                {
                    if (apprv_status == "4")
                    {
                        string sSqlstmt1 = "update t_jd set ApproverCount=ApproverCount-1,Approvers= concat(Approvers,',','" + user + "') where id_jd='" + id_jd + "'";
                        if (objGlobalData.ExecuteQuery(sSqlstmt1))
                        {
                            string sSqlstmt = "Select ApproverCount from t_jd where id_jd='" + id_jd + "'";
                            DataSet dsManagementChange = objGlobalData.Getdetails(sSqlstmt);
                            if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                            {
                                if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["ApproverCount"].ToString()) == 0)
                                {
                                    string sSSqlstmt = "update t_jd set approve_status ='" + apprv_status + "' where id_jd='" + id_jd + "'";
                                    objGlobalData.ExecuteQuery(sSSqlstmt);
                                    return SendJDApproveEmail(apprv_status, id_jd, "Job description approved");
                                }
                            }
                        }
                    }
                    else
                    {
                        string Sql1 = "update t_jd set ApproverCount=ApproverCount-1,approve_status='" + apprv_status + "',ApprovalRejector= concat(ApprovalRejector,',','" + user + "') where id_jd='" + id_jd + "'";
                        objGlobalData.ExecuteQuery(Sql1);
                        return SendJDApproveEmail(apprv_status, id_jd, "Job description rejected");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunJDApproveOrReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendJDApproveEmail(string apprv_status, string id_jd, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                          + "accountable,academic_mandatory,logged_by,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                          + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by,reviewed_by,rev_no from t_jd where id_jd='" + id_jd + "'";


                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
              
                    string sToEmailIds = "", sCCEmailIds = "", sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

                  
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

                    string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());

                    if (apprv_status == "3")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());

                        sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reviewed_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());

                    }
                    if (apprv_status == "4")
                    {
                        sToEmailIds = objGlobalData.GetMultiEmpEmailIdByRole(dsData.Tables[0].Rows[0]["designation_id"].ToString());

                        sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reviewed_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()); ;

                    }
                    string jd_date = "", revised_date = "";

                    if (dsData.Tables[0].Rows[0]["jd_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        jd_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["revised_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        revised_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()).ToString("yyyy-MM-dd");
                    }

                    sHeader = "<table><tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["designation_id"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Date:<b></td> <td colspan=3>" + jd_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["deptid"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Reports to:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["report_to"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Supervises:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["supervises"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Roles and Responsibilities:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["responsibility"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Authorities (What authorities this position has):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["authorities"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Interfaces with internal department:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_internal"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>External Interfaces:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_external"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Accountable for (liable for the consequences if failed to perform):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["accountable"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Revised on:<b></td> <td colspan=3>" + revised_date + "</td></tr>"

                       + "<tr><td colspan=3><b>Revision no:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["rev_no"].ToString()) + "</td></tr></table>";

                    sInformation = "<table><tr> "
                           + "<td colspan=8><label><b>Minimum Competency Required to hold the position</b></label> </td> </tr>"
                           + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"

                           + "<th style='width:300px'>Competence Parameters</th>"
                           + "<th style='width:300px'>Mandatory</th>"
                           + "<th style='width:300px'>Optional</th>"
                           + "</tr>"
                        + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Academic qualification</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Trade Certificate</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Relevant experience</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Relevant trainings</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_optional"].ToString()) + "</td>"
                      + "</tr>"

                       + "<tr> "
                        + " <td style='width:300px;background-color: #4cc4dd'>Skills (Wherever required)</td>"
                         + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_mandatory"].ToString()) + "</td>"
                     + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_optional"].ToString()) + "</td>"
                      + "</tr></table>";





                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Job description Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
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

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendJDApproveEmail: " + ex.ToString());
            }
            return false;
        }


        internal bool SendJDReviewEmail(string apprv_status, string id_jd, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_jd,designation_id,deptid,report_to,supervises,responsibility,authorities,interfaces_internal,interfaces_external,"
                          + "accountable,academic_mandatory,logged_by,academic_optional,trade_mandatory,trade_optional,experience_mandatory,experience_optional,trainings_mandatory,"
                          + "trainings_optional,skills_mandatory,skills_optional,jd_date,revised_date,approved_by,reviewed_by,rev_no from t_jd where id_jd='" + id_jd + "'";


                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                   
                    string sToEmailIds = "", sCCEmailIds = "", sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

                 
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

                    string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());

                    if (apprv_status == "1")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());

                        sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reviewed_by"].ToString());

                    }
                    if (apprv_status == "2")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());

                        sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reviewed_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());

                    }
                    string jd_date = "", revised_date = "";

                    if (dsData.Tables[0].Rows[0]["jd_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        jd_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["jd_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["revised_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        revised_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["revised_date"].ToString()).ToString("yyyy-MM-dd");
                    }

                    sHeader = "<table><tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["designation_id"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Date:<b></td> <td colspan=3>" + jd_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["deptid"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Reports to:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["report_to"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Supervises:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["supervises"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Roles and Responsibilities:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["responsibility"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Authorities (What authorities this position has):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["authorities"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Interfaces with internal department:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_internal"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>External Interfaces:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["interfaces_external"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Accountable for (liable for the consequences if failed to perform):<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["accountable"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Revised on:<b></td> <td colspan=3>" + revised_date + "</td></tr>"

                       + "<tr><td colspan=3><b>Revision no:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["rev_no"].ToString()) + "</td></tr></table>";

                    sInformation = "<table><tr> "
                          + "<td colspan=8><label><b>Minimum Competency Required to hold the position</b></label> </td> </tr>"
                          + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"

                          + "<th style='width:300px'>Competence Parameters</th>"
                          + "<th style='width:300px'>Mandatory</th>"
                          + "<th style='width:300px'>Optional</th>"
                          + "</tr>"
                       + "<tr> "
                       + " <td style='width:300px;background-color: #4cc4dd'>Academic qualification</td>"
                        + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_mandatory"].ToString()) + "</td>"
                    + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["academic_optional"].ToString()) + "</td>"
                     + "</tr>"

                      + "<tr> "
                       + " <td style='width:300px;background-color: #4cc4dd'>Trade Certificate</td>"
                        + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_mandatory"].ToString()) + "</td>"
                    + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trade_optional"].ToString()) + "</td>"
                     + "</tr>"

                      + "<tr> "
                       + " <td style='width:300px;background-color: #4cc4dd'>Relevant experience</td>"
                        + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_mandatory"].ToString()) + "</td>"
                    + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["experience_optional"].ToString()) + "</td>"
                     + "</tr>"

                      + "<tr> "
                       + " <td style='width:300px;background-color: #4cc4dd'>Relevant trainings</td>"
                        + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_mandatory"].ToString()) + "</td>"
                    + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["trainings_optional"].ToString()) + "</td>"
                     + "</tr>"

                      + "<tr> "
                       + " <td style='width:300px;background-color: #4cc4dd'>Skills (Wherever required)</td>"
                        + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_mandatory"].ToString()) + "</td>"
                    + " <td style='width:300px'>" + (dsData.Tables[0].Rows[0]["skills_optional"].ToString()) + "</td>"
                     + "</tr></table>";





                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Job description Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
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

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendJDEmail: " + ex.ToString());
            }
            return false;
        }

    }
    public class JDModelsList
    {
        public List<JDModels> JDList { get; set; }
    }
}