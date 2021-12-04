using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    [Bind(Exclude = "proposal_date")]
    public class BiddingModels
    {
        private clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_bidding { get; set; }

        [Display(Name = "Client")]
        public string client { get; set; }

        [Display(Name = "Client folder ref")]
        public string folderref { get; set; }

        [Display(Name = "Project Name")]
        public string projectname { get; set; }

        [Required]
        [Display(Name = "Submission Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime submission_date { get; set; }

        [Display(Name = "Prepared By")]
        public string preparedby { get; set; }

        [Display(Name = "To Be Checked By")]
        public string checkedby { get; set; }

        [Display(Name = "Proposal Ref")]
        public string proposalref { get; set; }

        [Required]
        [Display(Name = "To be checked by Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime proposal_date { get; set; }

        [Display(Name = "Proposal Status")]
        public string proposal_status { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime approved_date { get; set; }

        [Display(Name = "Approved By")]
        public string Approvers { get; set; }

        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        public string sub_date { get; set; }
        public string prop_date { get; set; }

        internal bool FunDeleteDoc(string sid_bidding)
        {
            try
            {
                string sSqlstmt = "update t_bidding set Active=0 where id_bidding=" + sid_bidding;
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddBiddingDetails(BiddingModels objBidModels)
        {
            try
            {
                string[] name = objBidModels.checkedby.Split(',');
                int count = name.Length;
                string user = objGlobalData.GetCurrentUserSession().empid;
                string sSqlstmt = "insert into t_bidding (client,folderref,projectname,preparedby,checkedby,proposalref,loggedby,approver_count,upload,NotificationPeriod,NotificationValue";

                string sFields = "", sFieldValue = "";

                if (objBidModels.submission_date != null && objBidModels.submission_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", submission_date";
                    sFieldValue = sFieldValue + ", '" + objBidModels.submission_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objBidModels.proposal_date != null && objBidModels.proposal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", proposal_date";
                    //Console.WriteLine(proposal_date.HasValue ? proposal_date.Value.ToString("yyyy/MM/dd") : "");
                    sFieldValue = sFieldValue + ", '" + objBidModels.proposal_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objBidModels.client + "','" + objBidModels.folderref + "',"
                + "'" + objBidModels.projectname + "','" + objBidModels.preparedby + "','" + objBidModels.checkedby + "',"
                + "'" + objBidModels.proposalref + "','" + user + "','" + count + "','" + objBidModels.upload + "',"
                + "'" + objBidModels.NotificationPeriod + "','" + objBidModels.NotificationValue + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendEmail(objBidModels);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEnquiryDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool SendEmail(BiddingModels objBidModels)
        {
            try
            {
                string sInformation = "", sHeader = "";
                string sCCList = objGlobalData.GetHrEmpEmailIdById(objBidModels.preparedby);

                string sUserName = objGlobalData.GetMultiHrEmpNameById(objBidModels.checkedby);
                string sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objBidModels.checkedby);

                sHeader = "<tr><td ><b>Client:<b></td> <td >"
                       + objBidModels.client + "</td></tr>"
                       + "<tr><td ><b>Project Name:<b></td> <td >" + objBidModels.projectname + "</td></tr>"
                       + "<tr><td ><b>Submission Date:<b></td> <td >" + objBidModels.submission_date.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>Prepared By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objBidModels.preparedby) + "</td></tr>"
                       + "<tr><td ><b>Proposal Ref:<b></td> <td >" + objBidModels.proposalref + "</td></tr>"
                        + "<tr><td ><b>Proposal Date:<b></td> <td >" + objBidModels.proposal_date.ToString("dd/MM/yyyy") + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "BiddingApproval", sHeader, "", "Bidding Details for approval");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateBiddingDetails(BiddingModels objBidModels)
        {
            try
            {
                string sSqlstmt = "update t_bidding set client ='" + objBidModels.client + "', folderref='" + objBidModels.folderref + "', "
                    + "projectname='" + objBidModels.projectname + "',preparedby='" + objBidModels.preparedby + "',checkedby='" + objBidModels.checkedby + "',"
                    + "proposalref='" + objBidModels.proposalref + "',upload='" + objBidModels.upload + "'";

                if (objBidModels.submission_date != null && objBidModels.submission_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", submission_date='" + objBidModels.submission_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objBidModels.proposal_date != null && objBidModels.proposal_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", proposal_date='" + objBidModels.proposal_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_bidding='" + objBidModels.id_bidding + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEnquiryDetails: " + ex.ToString());
            }

            return false;
        }

        internal bool FunBiddingDocApprove(string sid_bidding, int sStatus, string comments)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                if (sStatus == 1)
                {
                    string sSqlstmt1 = "update t_bidding set approver_count=approver_count-1 where id_bidding='" + sid_bidding + "'";
                    if (objGlobalData.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select approver_count from t_bidding where id_bidding='" + sid_bidding + "'";
                        DataSet dsBidding = objGlobalData.Getdetails(sSqlstmt);
                        if (dsBidding.Tables.Count > 0 && dsBidding.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsBidding.Tables[0].Rows[0]["approver_count"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_bidding set proposal_status ='1', approved_date='" + sApprovedDate + "' where id_bidding='" + sid_bidding + "'";
                                if (objGlobalData.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_bidding set Approvers= concat(Approvers,',','" + user + "') where id_bidding='" + sid_bidding + "'";
                                    objGlobalData.ExecuteQuery(Sql1);

                                    string Sql2 = "insert into t_bidding_comments set id_bidding='" + sid_bidding + "',CommentBy='" + user + "',Comments='" + comments + "',ApprovalStatus='Approved',ApproveDate='" + sApprovedDate + "'";
                                    objGlobalData.ExecuteQuery(Sql2);
                                    SendApproveEmail(sid_bidding);
                                    return true;
                                }
                            }
                            else
                            {
                                string Sql3 = "update t_bidding set Approvers= concat(Approvers,',','" + user + "') where id_bidding='" + sid_bidding + "'";
                                objGlobalData.ExecuteQuery(Sql3);

                                string Sql4 = "insert into t_bidding_comments set id_bidding='" + sid_bidding + "',CommentBy='" + user + "',Comments='" + comments + "',ApprovalStatus='Approved',ApproveDate='" + sApprovedDate + "'";
                                return objGlobalData.ExecuteQuery(Sql4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateBiddingDetails: " + ex.ToString());
            }

            return false;
        }

        internal bool SendApproveEmail(string sid_bidding)
        {
            try
            {
                string sInformation = "", sHeader = "";

                string sSqlStmt = "select id_bidding,client,folderref,projectname,submission_date,preparedby,checkedby,proposalref"
                    + ",proposal_date from t_bidding where id_bidding='" + sid_bidding + "'";

                DataSet dsBidding = objGlobalData.Getdetails(sSqlStmt);

                if (dsBidding.Tables.Count > 0 && dsBidding.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBidding.Tables[0].Rows.Count; i++)
                    {
                        string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsBidding.Tables[0].Rows[i]["checkedby"].ToString());
                        string sUserName = objGlobalData.GetEmpHrNameById(dsBidding.Tables[0].Rows[i]["preparedby"].ToString());
                        string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsBidding.Tables[0].Rows[i]["preparedby"].ToString());

                        sHeader = "<tr><td ><b>Client:<b></td> <td >"
                               + dsBidding.Tables[0].Rows[i]["client"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Project Name:<b></td> <td >" + dsBidding.Tables[0].Rows[i]["projectname"].ToString() + "</td></tr>"
                               + "<tr><td ><b>Submission Date:<b></td> <td >" + Convert.ToDateTime(dsBidding.Tables[0].Rows[i]["submission_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                               + "<tr><td ><b>Prepared By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(dsBidding.Tables[0].Rows[i]["preparedby"].ToString()) + "</td></tr>"
                                + "<tr><td ><b>Checked By:<b></td> <td >" + objGlobalData.GetEmpHrNameById(dsBidding.Tables[0].Rows[i]["checkedby"].ToString()) + "</td></tr>"
                                + "<tr><td ><b>Proposal Ref:<b></td> <td >" + dsBidding.Tables[0].Rows[i]["proposalref"].ToString() + "</td></tr>"
                                + "<tr><td ><b>Proposal Date:<b></td> <td >" + Convert.ToDateTime(dsBidding.Tables[0].Rows[i]["proposal_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>";

                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "Bidding", sHeader, "", "Bidding Details Approved");
                        objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendApproveEmail: " + ex.ToString());
            }
            return false;
        }

        public MultiSelectList GetPreparer()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            string Id = "";

            try
            {
                string sSqlstmt1 = "select Id from roles where Active=1 and RoleName='Preparer'";

                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt1);

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Id = dsEmp.Tables[0].Rows[i]["Id"].ToString();
                    }
                }
                string sSqlstmt = "select FirstName, empid from t_employee where active=1 and FIND_IN_SET('" + Id + "',Role) order by FirstName asc";

                DataSet dsEmp1 = objGlobalData.Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");
                if (dsEmp1.Tables.Count > 0 && dsEmp1.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp1.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp1.Tables[0].Rows[i]["empid"].ToString(),
                            Empname = dsEmp1.Tables[0].Rows[i]["FirstName"].ToString()
                        };

                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetApprover: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }
    }

    public class BiddingModelsList
    {
        public List<BiddingModels> BiddingList { get; set; }
    }
}