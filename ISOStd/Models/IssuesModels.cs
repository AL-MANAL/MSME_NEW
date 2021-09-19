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
        [Display(Name = "Impact")]
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
        public string Repet_Issue_detail { get; set; }

        [Display(Name = "Status")]
        public string issue_status { get; set; }

        [Display(Name = "Status updated on")]
        public DateTime status_date { get; set; }

        [Display(Name = "Action taken")]
        public string action_taken { get; set; }

        [Display(Name = "Notified To")]
        public string status_notifiedto { get; set; }


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

        internal bool FunAddIssues(IssuesModels objissue, HttpPostedFileBase file)
        {
            try
            {
                // string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sFiled = "";
                string sFiledValue = "";

                string sSqlstmt = "insert into t_issues (Issue,IssueType, Impact,Isostd, Evidence,ImpactDesc," +
                    "Effect,Issue_refno,Deptid,Issue_Category,branch,Location,reporting_to,notified_to,loggedby,Impact_detail,Repet_Issue";
                    
                    if(objissue.issue_date != null && objissue.issue_date > Convert.ToDateTime("01/01/0001"))
                    {
                       sFiled = sFiled + ", issue_date";
                       sFiledValue = sFiledValue + ", '" + objissue.issue_date.ToString("yyyy/MM/dd") + "'";
                    }

                sSqlstmt = sSqlstmt + sFiled + " ) values('" + objissue.Issue + "','" + objissue.IssueType + "','" + objissue.Impact + "'"
                    + ",'" + objissue.Isostd + "','" + objissue.Evidence + "','" + objissue.ImpactDesc + "','" + objissue.Effect + "','" + objissue.Issue_refno + "','" + objissue.Deptid + "','"
                    + objissue.Issue_Category + "','" + objissue.branch + "','" + objissue.Location + "','" + objissue.reporting_to + "','" + objissue.notified_to 
                    + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objissue.Impact_detail + "','" + objissue.Repet_Issue + "'";
                    
                sSqlstmt = sSqlstmt + sFiledValue + ")";
                int iid_issue = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_issue))
                {
                   if(iid_issue > 0)
                    { 
                    string sBranch = objGlobalData.GetBranchShortNameByID(objissue.branch)+"-"+ objGlobalData.GetDeptNameById(objissue.Deptid);
                    string sLoc = objGlobalData.GetLocationNameById(objissue.Location);

                    DataSet dsData = objGlobalData.GetReportNo("Issue", sLoc, sBranch);
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        Issue_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_issues set Issue_refno='" + Issue_refno + "' where id_issue='" + iid_issue + "'";
                    if (objGlobalData.ExecuteQuery(sql1))


                    SendIssueEmail(iid_issue);
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

        internal bool SendIssueEmail(int id_issue, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select Issue_refno,id_issue,Issue,IssueType,Impact,Isostd,Evidence," +
                    "ImpactDesc,Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to,loggedby from t_issues where id_issue ='" + id_issue + "'";

                DataSet dsIssueList = objGlobalData.Getdetails(sSqlstmt);
                IssuesModels objType = new IssuesModels();

                if (dsIssueList.Tables.Count > 0 && dsIssueList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "";

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
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["loggedby"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["loggedby"].ToString()) + ",";
                    }

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) != "")
                    {
                        sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) + ",";
                    }
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) != "")
                    {
                        sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) + ",";
                    }

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');
                    aAttachment = HttpContext.Current.Server.MapPath(dsIssueList.Tables[0].Rows[0]["Evidence"].ToString());


                    sHeader = "<tr><td colspan=3><b>Issue Number:<b></td> <td colspan=3>"
                        + dsIssueList.Tables[0].Rows[0]["Issue_refno"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Issue Reported Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsIssueList.Tables[0].Rows[0]["issue_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Issue Category:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Issue_Category"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Issue Type:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["IssueType"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Effect Due to Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Effect"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Impact:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Impact"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Issue Details");
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
                objGlobalData.AddFunctionalLog("Exception in SendIssueEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateIssues(IssuesModels objIssue, HttpPostedFileBase file)
        {
            try
            {
                string sSqlstmt = "update t_issues set Issue ='" + objIssue.Issue + "', IssueType='" + objIssue.IssueType + "', "
                    + "Impact='" + objIssue.Impact + "',Isostd='" + objIssue.Isostd + "',ImpactDesc='" + objIssue.ImpactDesc + "',Effect='" + objIssue.Effect
                /*+ ",Deptid='" + objIssue.Deptid*/ + "',Issue_Category='" + objIssue.Issue_Category + "',reporting_to='" + objIssue.reporting_to 
                + "',notified_to='" + objIssue.notified_to + "',Impact_detail='" + objIssue.Impact_detail + "',Repet_Issue='" + objIssue.Repet_Issue + "'";

                if (objIssue.Evidence != null)
                {
                    sSqlstmt = sSqlstmt + ", Evidence='" + objIssue.Evidence + "'";
                }

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

                string sSqlstmt = "update t_issues set issue_status='" + issue_status + "',action_taken='" + action_taken + "',status_notifiedto='" + status_notifiedto + "'";
                if (objIssue.status_date != null && objIssue.status_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",status_date='" + objIssue.status_date.ToString("yyyy/MM/dd") + "'";
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