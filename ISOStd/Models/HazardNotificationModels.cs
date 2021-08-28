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
    public class HazardNotificationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string id_notification { get; set; }
       
        [Display(Name = "Control No.")]
        public string control_no { get; set; }

        [Display(Name = "Observed By")]
        public string observed_by { get; set; }

        [Display(Name = "Reported  By")]
        public string reported_by { get; set; }

        [Display(Name = "Date of Hazard Notification")]
        public DateTime notify_date { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        //[Display(Name = "Division")]
        //public string division { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }        

        [Display(Name = "Activity Type")]
        public string activity_type { get; set; }

        [Display(Name = "DESCRIPTION OF ACTIVITY/OPERATION")]
        public string description { get; set; }

        [Display(Name = "HAZARD /ASPECT")]
        public string hazard_aspect { get; set; }

        [Display(Name = "WHEN / IN WHAT CONDITIONS THE HARM / IMPACT CAN HAPPEN?")]
        public string condition_impact { get; set; }

        [Display(Name = "PEOPLE/ ENVIRONMENT LIKELY TO BE AFFECTED")]
        public string affected_env { get; set; }

        [Display(Name = "PICTURE/ SKETCH TO DESCRIBE THE SITUATION  ")]
        public string upload { get; set; }

        [Display(Name = "REVIEW (By DMR)")]
        public string reviewed_by { get; set; }

        [Display(Name = "Estimated Level of Risk")]
        public string level_risk { get; set; }

        [Display(Name = "Should the Work be Stopped immediately? ")]
        public string work_stopped { get; set; }

        [Display(Name = "Immediate Control Measures Implemented")]
        public string control_measure { get; set; }

        [Display(Name = "Incorporated in HIRA/EIA Doc No")]
        public string incorporated_HIRA { get; set; }

        [Display(Name = "Incorporated By")]
        public string incorporated_by { get; set; }

        [Display(Name = "Incorporated Date")]
        public DateTime incorporated_date { get; set; }

        [Display(Name = "DMR Name")]
        public string dmr_name { get; set; }

        [Display(Name = "DMR Date")]
        public DateTime dmr_date { get; set; }

        [Display(Name = "Division Head")]
        public string dept_head { get; set; }

        [Display(Name = "Division Head Date")]
        public DateTime dept_head_date { get; set; }

        [Display(Name = "CRM Name")]
        public string cmr_name { get; set; }

        [Display(Name = "CRM Date")]
        public DateTime cmr_date { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        [Display(Name = "Hazard Location")]
        public string Hazard_loc { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunDeleteHazardNotify(string sid_notification)
        {
            try
            {
                string sSqlstmt = "update t_hazard_notification set Active=0 where id_notification='" + sid_notification + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteHazardNotify: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHazardNotify(HazardNotificationModels objHazard)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_hazard_notification (observed_by, reported_by, dept, branch,location,activity_type,description, "
                    + " hazard_aspect,condition_impact,affected_env,upload,reviewed_by,logged_by,Hazard_loc";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                if (objHazard.notify_date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", notify_date";
                    sValues = sValues + ", '" + objHazard.notify_date.ToString("yyyy-MM-dd") + "' ";
                }
                sSqlstmt = sSqlstmt + sColumn + ") values('" + objHazard.observed_by
                    + "','" + objHazard.reported_by + "','" + objHazard.dept + "','" + objHazard.branch + "','" + objHazard.location
                    + "','" + objHazard.activity_type + "','" + objHazard.description + "','" + objHazard.hazard_aspect + "','" + objHazard.condition_impact + "','" + objHazard.affected_env
                 + "','" + objHazard.upload + "','" + objHazard.reviewed_by + "','" + user + "','" + objHazard.Hazard_loc + "'";

                sSqlstmt = sSqlstmt + sValues + ")";


                int sid_notification = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out sid_notification))
                {
                    if (sid_notification > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("HazardNotify", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            control_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_hazard_notification set control_no='" + control_no + "' where id_notification = '" + sid_notification + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHazardNotify: " + ex.ToString());
            }
            return false;
        }

        internal bool SendHazardNotifyEmailbyDMR(string sMessage = "")
        {
            try
            {
                string sid_notification = "";
                string IdSqlstmt = "select max(id_notification) as id_notification from t_hazard_notification where Active=1";
                DataSet dsIdList = objGlobalData.Getdetails(IdSqlstmt);
                if (dsIdList.Tables.Count > 0 && dsIdList.Tables[0].Rows.Count > 0)
                {
                    sid_notification = dsIdList.Tables[0].Rows[0]["id_notification"].ToString();
                }
                string sType = "HazardNofitication";
                string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,division,location,activity_type,description, " +
                   "hazard_aspect, condition_impact, affected_env,upload,reviewed_by from t_hazard_notification" + 
                    " where id_notification = '" + sid_notification + "'";

                DataSet dsHazardList = objGlobalData.Getdetails(sSqlstmt);
                HazardNotificationModels objModels = new HazardNotificationModels();

                if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString());
                    string sToEmailIds = "";
                    string sCCEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString());
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    if(objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString())!= null)
                    {
                        sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) + ",";
                    }
                    if (objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) != null)
                    {
                        sCCEmailIds = sCCEmailIds + objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString());
                    }
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsHazardList.Tables[0].Rows[0]["upload"].ToString());
                    
                    sHeader = "<tr><td colspan=3><b>Control No:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["control_no"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Observed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[0]["dept"].ToString())+ "</td></tr>"
                        + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetDivisionById(dsHazardList.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetMultiWorkLocationById(dsHazardList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Activity Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["description"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Hazard Notification Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");  

                    objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendHazardNotifyEmailbyDMR: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateHazardNotify(HazardNotificationModels objHazard)
        {
            try
            {
                string sSqlstmt = "update t_hazard_notification set observed_by='" + objHazard.observed_by
                    + "', reported_by='" + objHazard.reported_by + "', dept='" + objHazard.dept + "', branch='" + objHazard.branch + "', location='" + objHazard.location
                    + "', activity_type='" + objHazard.activity_type + "', description='" + objHazard.description + "', hazard_aspect='" + objHazard.hazard_aspect 
                    + "', condition_impact='" + objHazard.condition_impact + "', affected_env='" + objHazard.affected_env + "', reviewed_by='" + objHazard.reviewed_by + "', Hazard_loc='" + objHazard.Hazard_loc + "'";

                if (objHazard.notify_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", notify_date='" + objHazard.notify_date.ToString("yyyy-MM-dd") + "' ";
                }

                if (objHazard.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objHazard.upload + "' ";
                }

                sSqlstmt = sSqlstmt + " where id_notification='" + objHazard.id_notification + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateHazardNotify: " + ex.ToString());
            }
            return false;
        }

        internal bool SendHazardNotifyEmailFromDMR(string sid_notification, string sMessage = "")
        {
            try
            {              
                string sType = "HazardNofitication";

                string sSqlstmt = "SELECT id_notification,control_no, observed_by, reported_by,notify_date, dept,branch,location,activity_type,description, " +
                   "hazard_aspect, condition_impact, affected_env,upload,reviewed_by,level_risk,work_stopped,control_measure,incorporated_HIRA,incorporated_by,incorporated_date," +
                   "dept_head,cmr_name from t_hazard_notification where id_notification = '" + sid_notification + "'";

                DataSet dsHazardList = objGlobalData.Getdetails(sSqlstmt);
                HazardNotificationModels objModels = new HazardNotificationModels();

                if (dsHazardList.Tables.Count > 0 && dsHazardList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString()) + "," + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["dept_head"].ToString());
                    string sToEmailIds = "";
                    string sCCEmailIds = "";
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString()) != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["cmr_name"].ToString()) + "," ;
                    }
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["dept_head"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds + objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["dept_head"].ToString());
                    }
                    if (objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds + objGlobalData.GetMultiHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString());
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    if (objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()) != null)
                    {
                        sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reviewed_by"].ToString()) + ",";
                    }
                    if (objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) != null)
                    {
                        sCCEmailIds = sCCEmailIds + objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) + ",";
                    }
                    if (objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) != "" && objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) != null)
                    {
                        sCCEmailIds = sCCEmailIds + objGlobalData.GetHrEmpEmailIdById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString());
                    }
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    aAttachment = HttpContext.Current.Server.MapPath(dsHazardList.Tables[0].Rows[0]["upload"].ToString());

                    sHeader = "<tr><td colspan=3><b>Control No:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["control_no"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Observed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["observed_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["reported_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["notify_date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsHazardList.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsHazardList.Tables[0].Rows[0]["branch"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetMultiWorkLocationById(dsHazardList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Activity Type:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["activity_type"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Description:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["description"].ToString()) + "</td></tr>"
                        + "<tr><br /></tr>"
                        + "<tr><td colspan=3><b>Estimated Level of Risk:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsHazardList.Tables[0].Rows[0]["level_risk"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Should the Work be Stopped immediately? :<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["work_stopped"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Immediate Control Measures Implemented:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["control_measure"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Incorporated in HIRA/EIA Doc No:<b></td> <td colspan=3>" + (dsHazardList.Tables[0].Rows[0]["incorporated_HIRA"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Incorporated By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsHazardList.Tables[0].Rows[0]["incorporated_by"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Incorporated Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsHazardList.Tables[0].Rows[0]["incorporated_date"].ToString()).ToString("dd/MM/yyyy");

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Hazard Notification Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendHazardNotifyEmailFromDMR: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateHazardNotifyDMR(HazardNotificationModels objHazard)
        {
            try
            {
                string sSqlstmt = "update t_hazard_notification set level_risk='" + objHazard.level_risk
                    + "', work_stopped='" + objHazard.work_stopped + "', control_measure='" + objHazard.control_measure 
                    + "', incorporated_HIRA='" + objHazard.incorporated_HIRA + "', incorporated_by='" + objHazard.incorporated_by
                    + "', dept_head='" + objHazard.dept_head + "', cmr_name='" + objHazard.cmr_name + "'";

                if (objHazard.incorporated_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", incorporated_date='" + objHazard.incorporated_date.ToString("yyyy-MM-dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_notification='" + objHazard.id_notification + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateHazardNotifyDMR: " + ex.ToString());
            }
            return false;
        }

    }

    public class HazardNotificationModelsList
    {
        public List<HazardNotificationModels> NofifyList { get; set; }
    }
}
