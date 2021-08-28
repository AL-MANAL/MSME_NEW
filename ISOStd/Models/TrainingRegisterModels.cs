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
    public class TrainingRegisterModels
    {
        clsGlobal objGlobalData = new clsGlobal();
                    
        public string id_tr { get; set; }

        [Display(Name = "Employee Name")]
        public string emp_id { get; set; }           
        public string division { get; set; }

        [Display(Name = "Department Head")]
        public string dept_head { get; set; }

        [Display(Name = "Training Register Date")]
        public DateTime date_tr { get; set; }

        [Display(Name = "Training Type")]
        public string tr_type { get; set; }

        [Display(Name = "Training Topic")]
        public string tr_topic { get; set; }

        [Display(Name = "Training Center Name")]
        public string tr_centername { get; set; }

        [Display(Name = "Training Location")]
        public string tr_location { get; set; }

        [Display(Name = "Training Start Date")]
        public DateTime tr_startdate { get; set; }

        [Display(Name = "Training End Date")]
        public DateTime tr_enddate { get; set; }

        [Display(Name = "Training Expiry Date")]
        public DateTime tr_expirydate { get; set; }
       
        [Display(Name = "Upload Attachemnt")]
        public string upload { get; set; }

        public string loggedby { get; set; }
        public string department { get; set; }
        public string location { get; set; }
        public string designation { get; set; }


        internal bool FunAddTR(TrainingRegisterModels objissue)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sFiled = "";
                string sFiledValue = "";

                string sSqlstmt = "insert into t_training_register (emp_id,dept_head, tr_type,tr_topic, tr_centername,tr_location,upload,division,loggedby";

                if (objissue.date_tr != null && objissue.date_tr > Convert.ToDateTime("01/01/0001"))
                {
                    sFiled = sFiled + ", date_tr";
                    sFiledValue = sFiledValue + ", '" + objissue.date_tr.ToString("yyyy/MM/dd") + "'";
                }
                if (objissue.tr_startdate != null && objissue.tr_startdate > Convert.ToDateTime("01/01/0001"))
                {
                    sFiled = sFiled + ", tr_startdate";
                    sFiledValue = sFiledValue + ", '" + objissue.tr_startdate.ToString("yyyy/MM/dd") + "'";
                }
                if (objissue.tr_enddate != null && objissue.tr_enddate > Convert.ToDateTime("01/01/0001"))
                {
                    sFiled = sFiled + ", tr_enddate";
                    sFiledValue = sFiledValue + ", '" + objissue.tr_enddate.ToString("yyyy/MM/dd") + "'";
                }
                if (objissue.tr_expirydate != null && objissue.tr_expirydate > Convert.ToDateTime("01/01/0001"))
                {
                    sFiled = sFiled + ", tr_expirydate";
                    sFiledValue = sFiledValue + ", '" + objissue.tr_expirydate.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFiled + " ) values('" + objissue.emp_id + "','" + objissue.dept_head + "','" + objissue.tr_type + "'"
                    + ",'" + objissue.tr_topic + "','" + objissue.tr_centername + "','" + objissue.tr_location + "','" + objissue.upload + "','" + sBranch  + "','" + objGlobalData.GetCurrentUserSession().empid + "'";

                sSqlstmt = sSqlstmt + sFiledValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
                //int iid_tr = 0;
                //if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_tr))
                //{
                //    if (iid_tr > 0)
                //    {
                //    //    string sBranch = objGlobalData.GetBranchShortNameByID(objissue.branch) + "-" + objGlobalData.GetDeptNameById(objissue.Deptid);
                //    //    string sLoc = objGlobalData.GetLocationNameById(objissue.Location);

                //        //DataSet dsData = objGlobalData.GetReportNo("Issue", sLoc, sBranch);
                //        //if (dsData != null && dsData.Tables.Count > 0)
                //        //{
                //        //    Issue_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                //        //}
                //        //string sql1 = "update t_training_register set Issue_refno='" + Issue_refno + "' where id_tr='" + iid_issue + "'";
                //        //if (objGlobalData.ExecuteQuery(sql1))


                //        SendTREmail(iid_tr);
                //        return true;
                //    }
                //}
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTR: " + ex.ToString());
            }
            return false;
        }

        //internal bool SendTREmail(int sid_tr)
        //{
        //    try
        //    {
        //        string sType = "email";

        //        string sSqlstmt = "select Issue_refno,id_tr,Issue,IssueType,Impact,Isostd,Evidence," +
        //            "ImpactDesc,Effect,Deptid,Issue_Category,branch,Location,issue_date,reporting_to,notified_to,loggedby from t_training_register where id_tr ='" + sid_tr + "'";

        //        DataSet dsIssueList = objGlobalData.Getdetails(sSqlstmt);
        //        TrainingRegisterModels objType = new TrainingRegisterModels();

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
        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["loggedby"].ToString()) != "")
        //            {
        //                sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["loggedby"].ToString()) + ",";
        //            }

        //            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
        //            sToEmailIds = sToEmailIds.Trim();
        //            sToEmailIds = sToEmailIds.TrimEnd(',');
        //            sToEmailIds = sToEmailIds.TrimStart(',');

        //            string sCCEmailIds = "";

        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) != "")
        //            {
        //                sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["reporting_to"].ToString()) + ",";
        //            }
        //            sCCEmailIds = sCCEmailIds.Trim();
        //            sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            sCCEmailIds = sCCEmailIds.TrimStart(',');

        //            if (objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) != "")
        //            {
        //                sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsIssueList.Tables[0].Rows[0]["notified_to"].ToString()) + ",";
        //            }

        //            sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
        //            sCCEmailIds = sCCEmailIds.Trim();
        //            sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            sCCEmailIds = sCCEmailIds.TrimStart(',');
        //            aAttachment = HttpContext.Current.Server.MapPath(dsIssueList.Tables[0].Rows[0]["Evidence"].ToString());


        //            sHeader = "<tr><td colspan=3><b>Issue Number:<b></td> <td colspan=3>"
        //                + dsIssueList.Tables[0].Rows[0]["Issue_refno"].ToString() + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Reported Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsIssueList.Tables[0].Rows[0]["issue_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Category:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsIssueList.Tables[0].Rows[0]["Issue_Category"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Issue Type:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["IssueType"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Effect Due to Issue:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Effect"].ToString()) + "</td></tr>"
        //                + "<tr><td colspan=3><b>Impact:<b></td> <td colspan=3>" + (dsIssueList.Tables[0].Rows[0]["Impact"].ToString()) + "</td></tr>";

        //            if (File.Exists(aAttachment))
        //            {
        //                sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
        //            }

        //            sContent = sContent.Replace("{FromMsg}", "");
        //            sContent = sContent.Replace("{UserName}", sName);
        //            sContent = sContent.Replace("{Title}", "Issue Details");
        //            sContent = sContent.Replace("{content}", sHeader + sInformation);
        //            sContent = sContent.Replace("{message}", "");
        //            sContent = sContent.Replace("{extramessage}", "");

        //            sToEmailIds = sToEmailIds.Trim(',');


        //            objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendTREmail: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunUpdateTR(TrainingRegisterModels objTR)
        {
            try
            {
                string sSqlstmt = "update t_training_register set emp_id ='" + objTR.emp_id + "', dept_head='" + objTR.dept_head + "', "
                    + "tr_type='" + objTR.tr_type + "',tr_topic='" + objTR.tr_topic + "',tr_centername='" + objTR.tr_centername + "',tr_location='" + objTR.tr_location + "'";

                if (objTR.upload != null)
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objTR.upload + "'";
                }

                if (objTR.date_tr != null && objTR.date_tr > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_tr ='" + objTR.date_tr.ToString("yyyy/MM/dd") + "'";
                }

                if (objTR.tr_startdate != null && objTR.tr_startdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", tr_startdate ='" + objTR.tr_startdate.ToString("yyyy/MM/dd") + "'";
                }

                if (objTR.tr_enddate != null && objTR.tr_enddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", tr_enddate ='" + objTR.tr_enddate.ToString("yyyy/MM/dd") + "'";
                }

                if (objTR.tr_expirydate != null && objTR.tr_expirydate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", tr_expirydate ='" + objTR.tr_expirydate.ToString("yyyy/MM/dd") + "'";
                }


                sSqlstmt = sSqlstmt + " where id_tr='" + objTR.id_tr + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTR: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteTR(string sid_tr)
        {
            try
            {
                string sSqlstmt = "update t_training_register set active=0 where id_tr='" + sid_tr + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTR: " + ex.ToString());
            }
            return false;
        }

    }

    public class TrainingRegisterModelsList
    {
       public List <TrainingRegisterModels> TRList { get; set; }
    }
}