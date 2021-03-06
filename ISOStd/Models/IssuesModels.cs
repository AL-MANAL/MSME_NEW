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
    public class IssuesModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public int id_issue { get; set; }
        
        [Required]
        [Display(Name = "Issue Ref No.")]
        [System.Web.Mvc.Remote("doesIssueRefNoExist", "Issues", HttpMethod = "POST", ErrorMessage = "Issue Reference No already exists. Please enter a different Reference No.")]
        public string Issue_refno { get; set; }
        
        [Required]
        [Display(Name = "Issue")]
        public string Issue { get; set; }

        [Required]
        [Display(Name = "Issue Type")]
        public string IssueType { get; set; }

        [Required]
        [Display(Name = "Impact Level")]
        public string Impact { get; set; }

        [Display(Name = "Standards")]
        public string Isostd { get; set; }

        [Display(Name = "Evidence")]
        public string Evidence { get; set; }

        [Display(Name = "Impact Description")]
        public string ImpactDesc { get; set; }

        [Display(Name = "Issue Category")]
        public string Issue_Category { get; set; }

        [Display(Name = "Effect")]
        public string Effect { get; set; }

        [Display(Name = "Department")]
        public string Deptid { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }
        public string branchId { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Issue Reporting Date")]
        public DateTime issue_date { get; set; }

        [Display(Name = "Issue Reported To")]
        public string reporting_to { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }


        [Display(Name = "Impact in detail")]
        public string Impact_detail { get; set; }

        [Display(Name = "Repetitive Issue")]
        public string Repet_Issue { get; set; }

        [Display(Name = "Repetitive issue detail")]
        public string Repet_Issue_detail { get; set; }

        [Display(Name = "Status")]
        public string issue_status { get; set; }

        [Display(Name = "Status updated on")]
        public DateTime status_date { get; set; }

        [Display(Name = "Action taken")]
        public string action_taken { get; set; }

        [Display(Name = "Notified To")]
        public string status_notifiedto { get; set; }

        [Display(Name = "Additional Details")]
        public string additional_details { get; set; }

        [Display(Name = "Upload")]
        public string status_upload { get; set; }
        public string loggedby { get; set; }
        public string reporting_toId { get; set; }

        [Display(Name = "Standards")]
        public string ISOStds { get; set; }

        public string reporting_to_div { get; set; }

        internal bool CheckForIssueRefNoExists(string sIssue_refno)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Issue_refno from t_issues where Issue_refno='" + sIssue_refno + "' and Active=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForRefNoExists: " + ex.ToString());
            }
            return true;
        }
        internal bool CheckForPartiesRefNoExists(string sRef_no)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Ref_no from t_parties where Ref_no='" + sRef_no + "' and Active=1");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForPartiesRefNoExists: " + ex.ToString());
            }
            return true;
        }
        internal bool FunDeleteIssueDoc(string sid_issue)
        {
            try
            {
                string sSqlstmt = "update t_issues set Active=0 where id_issue='" + sid_issue + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteIssueDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddIssues(IssuesModels objissue)
        {
            try
            {
                //if (objissue.reporting_to != null && objissue.reporting_to != "")
                //{
                    
                //    string otherdiv = "";
                //    DataSet dsEmpLocList = objGlobalData.Getdetails("select group_concat(division) as division from t_hr_employee where find_in_set(emp_no,'" + objissue.reporting_to + "')");
                //    if (dsEmpLocList != null && dsEmpLocList.Tables.Count > 0 && dsEmpLocList.Tables[0].Rows.Count > 0)
                //    {
                //        otherdiv = dsEmpLocList.Tables[0].Rows[0]["division"].ToString();
                //    }
                //    objissue.branch = objissue.branch + "," + otherdiv;
                //    List<string> uniques = objissue.branch.Split(',').Distinct().ToList();
                //    objissue.branch = string.Join(",", uniques);
                //    objissue.branch = objissue.branch.Trim();
                //}
                string sFiled = "";
                string sFiledValue = "";

                string sSqlstmt = "insert into t_issues (Issue,IssueType, Impact,Isostd, Evidence,ImpactDesc," +
                    "Effect,Issue_refno,Deptid,Issue_Category,branch,Location,reporting_to,notified_to,loggedby,Impact_detail,Repet_Issue,additional_details,ISOStds";
                    
                    if(objissue.issue_date != null && objissue.issue_date > Convert.ToDateTime("01/01/0001"))
                    {
                       sFiled = sFiled + ", issue_date";
                       sFiledValue = sFiledValue + ", '" + objissue.issue_date.ToString("yyyy/MM/dd") + "'";
                    }

                sSqlstmt = sSqlstmt + sFiled + " ) values('" + objissue.Issue + "','" + objissue.IssueType + "','" + objissue.Impact + "'"
                    + ",'" + objissue.Isostd + "','" + objissue.Evidence + "','" + objissue.ImpactDesc + "','" + objissue.Effect + "','" + objissue.Issue_refno + "','" + objissue.Deptid + "','"
                    + objissue.Issue_Category + "','" + objissue.branch + "','" + objissue.Location + "','" + objissue.reporting_to + "','" + objissue.notified_to 
                    + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objissue.Impact_detail + "','" + objissue.Repet_Issue + "','" + objissue.additional_details + "','" + objissue.ISOStds + "'";
                    
                sSqlstmt = sSqlstmt + sFiledValue + ")";
                int iid_issue = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_issue))
                {
                   if(iid_issue > 0)
                    { 
                    string sBranch = objGlobalData.GetBranchShortNameByID(objissue.branch)/*+"-"+ objGlobalData.GetDeptNameById(objissue.Deptid)*/;
                    string sLoc = objGlobalData.GetLocationNameById(objissue.Location);

                    DataSet dsData = objGlobalData.GetReportNo("Issue", sLoc, sBranch);
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        Issue_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_issues set Issue_refno='" + Issue_refno + "' where id_issue='" + iid_issue + "'";
                    if (objGlobalData.ExecuteQuery(sql1))


                    SendIssueEmail(iid_issue,"Internal and External Issue");
                    return true;
                   }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddIssues: " + ex.ToString());
            }
            return false;
        }
        internal bool SendIssueEmail(int iid_issue, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select Issue,IssueType,Impact,Effect,Issue_refno,Issue_Category,"
                + "Deptid,branch,Location,issue_date,reporting_to,notified_to,loggedby,additional_details,"
                + "Impact_detail,Repet_Issue,ISOStds from t_issues where id_issue = '" + iid_issue + "'";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
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
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reporting_to"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["notified_to"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["loggedby"].ToString());

                    string issue_date = "";
                    if (dsData.Tables[0].Rows[0]["issue_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["issue_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        issue_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["issue_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Issue Ref No:<b></td> <td colspan=3>" + dsData.Tables[0].Rows[0]["Issue_refno"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["Deptid"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetLocationNameById(dsData.Tables[0].Rows[0]["Location"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Issue Reporting Date:<b></td> <td colspan=3>" + issue_date + "</td></tr>"

                    + "<tr><td colspan=3><b>Issue Reported To:<b></td> <td colspan=3>" +objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["reporting_to"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Standard:<b></td> <td colspan=3>" + objGlobalData.GetIsoStdNameById(dsData.Tables[0].Rows[0]["ISOStds"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Issue:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Effect:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Effect"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Repetitive Issue:<b></td> <td colspan=3>" + objGlobalData.GetIssueRefnobyId(dsData.Tables[0].Rows[0]["Repet_Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Additional Details:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["additional_details"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Issue Category:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Issue_Category"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Issue Type:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["IssueType"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Impact Level:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["Impact"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Impact in detail:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["Impact_detail"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Notified To:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString()) + "</td></tr>"; 


                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Internal and External Issue Details");
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
                objGlobalData.AddFunctionalLog("Exception in SendIssueEmail: " + ex.ToString());
            }
            return false;
        }
        //internal bool SendIssueEmail(int id_issue, string sMessage = "")
        //{
        //    try
        //    {
        //        string sType = "email";

        //        string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence," +
        //            "ImpactDesc,Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to,loggedby,Impact_detail from t_issues where id_issue ='" + id_issue + "'";

        //        DataSet dsIssueList = objGlobalData.Getdetails(sSqlstmt);
        //        IssuesModels objType = new IssuesModels();

        //        if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
        //        {
        //            string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "";

        //            //Form the Email Subject and Body content
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
        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) != "")
        //            {
        //                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) + ",";
        //            }
        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) != "")
        //            {
        //                sToEmailIds = sToEmailIds+","+objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) + ",";
        //            }

        //            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
        //            sToEmailIds = sToEmailIds.Trim();
        //            sToEmailIds = sToEmailIds.TrimEnd(',');
        //            sToEmailIds = sToEmailIds.TrimStart(',');

        //            string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["loggedby"].ToString());

        //            //if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) != "")
        //            //{
        //            //    sCCEmailIds = sCCEmailIds +","+ objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) + ",";
        //            //}
        //            //sCCEmailIds = sCCEmailIds.Trim();
        //            //sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            //sCCEmailIds = sCCEmailIds.TrimStart(',');


        //            sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
        //            sCCEmailIds = sCCEmailIds.Trim();
        //            sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            sCCEmailIds = sCCEmailIds.TrimStart(',');

        //            if (dsIssueList.Tables[0].Rows[0]["Evidence"].ToString() != "")
        //            {
        //                if (dsIssueList.Tables[0].Rows[0]["Evidence"].ToString().Contains(','))
        //                {
        //                    string[] varfile = dsIssueList.Tables[0].Rows[0]["Evidence"].ToString().Split(',');

        //                    for (int i = 0; i < varfile.Length; i++)
        //                    {
        //                        //if (System.IO.File.Exists(varfile[i]))
        //                        //{
        //                            aAttachment = aAttachment + "," + HttpContext.Current.Server.MapPath(varfile[i]);
        //                        //}
        //                    }
        //                }
        //                else
        //                {
        //                    //if (System.IO.File.Exists(dsIssueList.Tables[0].Rows[0]["Evidence"].ToString()))
        //                    //{
        //                        aAttachment = HttpContext.Current.Server.MapPath(dsIssueList.Tables[0].Rows[0]["Evidence"].ToString());
        //                    //}
        //                }

        //            }
        //            aAttachment = aAttachment.Trim();
        //            aAttachment = aAttachment.TrimEnd(',');
        //            aAttachment = aAttachment.TrimStart(',');

        //            sHeader = "<tr><td colspan=3><b>Issue Number:<b></td> <td colspan=3>"
        //                + dsIssueList.Tables[0].Rows[0]["Issue_refno"].ToString() + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Reported Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsIssueList.Tables[0].Rows[0]["issue_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Category:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Issue_Category"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Type:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["IssueType"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Effect Due to Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Effect"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Impact Level:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Impact"].ToString()) + "</td></tr>"
        //                 + "<tr><td colspan=3><b>Impact Details:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Impact_detail"].ToString()) + "</td></tr>";

        //            if (File.Exists(aAttachment))
        //            {
        //                sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
        //            }

        //            sContent = sContent.Replace("{FromMsg}", "");
        //            sContent = sContent.Replace("{UserName}", sName);
        //            sContent = sContent.Replace("{Title}", "Issue Details");
        //            sContent = sContent.Replace("{content}", sHeader);
        //            sContent = sContent.Replace("{message}", "");
        //            sContent = sContent.Replace("{extramessage}", "");

        //            sToEmailIds = sToEmailIds.Trim(',');


        //            objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendIssueEmail: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunUpdateIssues(IssuesModels objIssue)
        {
            try
            {
                //if (objIssue.reporting_to != null && objIssue.reporting_to != "")
                //{

                //    string otherdiv = "";
                //    DataSet dsEmpLocList = objGlobalData.Getdetails("select group_concat(division) as division from t_hr_employee where find_in_set(emp_no,'" + objIssue.reporting_to + "')");
                //    if (dsEmpLocList != null && dsEmpLocList.Tables.Count > 0 && dsEmpLocList.Tables[0].Rows.Count > 0)
                //    {
                //        otherdiv = dsEmpLocList.Tables[0].Rows[0]["division"].ToString();
                //    }
                //    objIssue.branch = objIssue.branch + "," + otherdiv;
                //    objIssue.branch = objIssue.branch.Trim();
                //    objIssue.branch = objIssue.branch.TrimEnd(',');
                //    objIssue.branch = objIssue.branch.TrimStart(',');
                //    List<string> uniques = objIssue.branch.Split(',').Distinct().ToList();
                //    objIssue.branch = string.Join(",", uniques);
                //    objIssue.branch = objIssue.branch.Trim();
                //    objIssue.branch = objIssue.branch.TrimEnd(',');
                //    objIssue.branch = objIssue.branch.TrimStart(',');
                //}

                string sSqlstmt = "update t_issues set Issue ='" + objIssue.Issue + "', IssueType='" + objIssue.IssueType + "', "
                    + "Impact='" + objIssue.Impact + "',Isostd='" + objIssue.Isostd + "',ImpactDesc='" + objIssue.ImpactDesc + "',Effect='" + objIssue.Effect
                + "',branch='" + objIssue.branch + "',Issue_Category='" + objIssue.Issue_Category + "',reporting_to='" + objIssue.reporting_to 
                + "',notified_to='" + objIssue.notified_to + "',Impact_detail='" + objIssue.Impact_detail + "',Repet_Issue='" + objIssue.Repet_Issue 
                + "',additional_details='" + objIssue.additional_details + "', Evidence='" + objIssue.Evidence + "', ISOStds='" + objIssue.ISOStds + "'";

              
                if (objIssue.issue_date != null && objIssue.issue_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", issue_date ='" + objIssue.issue_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_issue='" + objIssue.id_issue + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                     return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateIssues: " + ex.ToString());
            }
            return false;
        }

        //status update
        internal bool FunUpdateStatus(IssuesModels objIssue)
        {
            try
            {
                string sTime = DateTime.Now.ToString("HH':'mm':'ss");
                string sSqlstmt = "update t_issues set issue_status='" + issue_status + "',action_taken='" + action_taken + "',status_notifiedto='" + status_notifiedto + "',status_upload='" + status_upload + "'";
                if (objIssue.status_date != null && objIssue.status_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    string sDate = objIssue.status_date.ToString("yyyy/MM/dd");
                    string sStatus = sDate +" "+ sTime;
                    sSqlstmt = sSqlstmt + ",status_date='" + sStatus + "'";
                }
                sSqlstmt = sSqlstmt + " where id_issue='" + objIssue.id_issue + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateStatus: " + ex.ToString());
            }
            return false;
        }
    }

    public class PartiesModel
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public int id_parties { get; set; }

        [Required]
        [Display(Name = "Ref No")]
        [System.Web.Mvc.Remote("doesPartiesRefNoExist", "Parties", HttpMethod = "POST", ErrorMessage = "Reference No already exists. Please enter a different Reference No.")]
        public string Ref_no { get; set; }


        [Required]
        [Display(Name = "Party Name")]
        public string PartyName { get; set; }

        [Required]
        [Display(Name = "Party Type")]
        public string PartyType { get; set; }

        [Required]
        [Display(Name = "Needs and Expectations")]
        public string Expectations { get; set; }

        [Required]
        [Display(Name = "MS Requirements")]
        public string Impact { get; set; }

        [Required]
        [Display(Name = "Standard")]
        public string Isostd { get; set; }
                
        [Display(Name = "Issue arising out of meeting needs and expectations")]
        public string Issues { get; set; }
        
        [Display(Name = "Power Level")]
        public string priority { get; set; }

        [Display(Name = "Monitoring Mechanism")]
        public string monit_mech { get; set; }

        [Display(Name = "PI Rank Value")]
        public string pi_rank { get; set; }

        [Display(Name = "Interest Level")]
        public string interest_level { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunDeletePartiesDoc(string sid_parties)
        {
            try
            {
                string sSqlstmt = "update t_parties set Active=0 where id_parties='" + sid_parties + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePartiesDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddParties(PartiesModel objParties)
        {
            try
            {
               // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_parties (PartyName,PartyType, Expectations,Impact, Isostd,Issues,Ref_no,priority,monit_mech,pi_rank,interest_level,branch,Department,Location) values('" + objParties.PartyName + "','" + objParties.PartyType + "','" + objParties.Expectations + "'"
                    + ",'" + objParties.Impact + "','" + objParties.Isostd + "','" + objParties.Issues + "','" + objParties.Ref_no + "','" + objParties.priority + "','" + objParties.monit_mech + "','" + objParties.pi_rank + "','" + objParties.interest_level + "','" + objParties.branch + "','" + objParties.Department + "','" + objParties.Location + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddParties: " + ex.ToString());
            }
            return false;
        }

        public MultiSelectList getParties()
        {
            DropdownPartiesList objPartiesList = new DropdownPartiesList();
            objPartiesList.lstparties = new List<DropdownPartiesItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Party Name' order by item_desc asc";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);

                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownPartiesItems objReport = new DropdownPartiesItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objPartiesList.lstparties.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getParties: " + ex.ToString());
            }

            return new MultiSelectList(objPartiesList.lstparties, "Id", "Name");

        }

        public string GetPartyNameById(string PartyName)
        {
            try
            {
                if (PartyName != "")
                {
                    string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Party Name' and (item_id='" + PartyName + "' or item_desc='" + PartyName + "')";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetPartyNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunUpdateParties(PartiesModel objParties)
        {
            try
            {
                string sSqlstmt = "update t_parties set PartyName ='" + objParties.PartyName + "', PartyType='" + objParties.PartyType + "', "
                    + "Expectations='" + objParties.Expectations + "',Impact='" + objParties.Impact + "',Isostd='" + objParties.Isostd + "',Issues='" + objParties.Issues +
                    "',priority='" + objParties.priority + "',monit_mech='" + objParties.monit_mech + "',pi_rank='" + objParties.pi_rank + "',interest_level='" + objParties.interest_level
                    + "',Department='" + objParties.Department + "',Location='" + objParties.Location + "',branch='" + objParties.branch + "'"; 


                 sSqlstmt = sSqlstmt + " where id_parties='" + objParties.id_parties + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateParties: " + ex.ToString());
            }

            return false;
        }


    }
    public class IssuesModelsList
    {
        public List<IssuesModels> IssueList { get; set; }
    }

    public class PartiesModelsList
    {
        public List<PartiesModel> PartiesList { get; set; }
    }

    public class DropdownPartiesItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownPartiesList
    {
        public List<DropdownPartiesItems> lstparties { get; set; }
    }


}