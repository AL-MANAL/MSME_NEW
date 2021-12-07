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
    public class TrainingPlanModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_training_plan { get; set; }

        [Display(Name = "Training topic ")]
        public string topic { get; set; }

        [Display(Name = "Employee names ")]
        public string emp_id { get; set; }

        [Display(Name = "From Date")]
        public DateTime from_date { get; set; }

        [Display(Name = "To Date")]
        public DateTime to_date { get; set; }

        [Display(Name = "Training source")]
        public string source_id { get; set; }

        [Display(Name = "Trainer Name")]
        public string trainer_name { get; set; }

        [Display(Name = "To be reviewed by")]
        public string reviewed_by { get; set; }

        [Display(Name = "To be approved by")]
        public string approved_by { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Entity Name")]
        public string ext_entity { get; set; }

        [Display(Name = "Training Approval Status")]
        public string approval_status { get; set; }

        public string approval_status_id { get; set; }
        public string reviewed_by_id { get; set; }
        public string approved_by_id { get; set; }

        [Display(Name = "Comments")]
        public string reviewer_comments { get; set; }

        [Display(Name = "Comments")]
        public string approver_comments { get; set; }

        [Display(Name = "Date")]
        public DateTime reviewed_date { get; set; }

        [Display(Name = "Date")]
        public DateTime approved_date { get; set; }

        internal bool FunAddTrainingPlan(TrainingPlanModels objModel)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "insert into t_training_plan (division,department,location,topic,emp_id,source_id,trainer_name,reviewed_by,approved_by,ext_entity,logged_by";

                string sFields = "", sFieldValue = "";

                if (objModel.from_date != null && objModel.from_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", from_date";
                    sFieldValue = sFieldValue + ", '" + objModel.from_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.to_date != null && objModel.to_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", to_date";
                    sFieldValue = sFieldValue + ", '" + objModel.to_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('"+division+ "','" + department + "','" + location + "','" + topic + "','" + emp_id + "','" + source_id + "','" + trainer_name + "','" + reviewed_by + "','" + approved_by + "','" + ext_entity + "','" + user + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_training_plan = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_training_plan))
                {
                    return sendTrainingPlanMail(id_training_plan,"Training Plan for Review");
                }
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingPlan: " + ex.ToString());
            }
            return false;
        }

        internal bool sendTrainingPlanMail(int id_training_plan, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_training_plan,division,department,location,topic,emp_id,from_date,to_date,source_id,trainer_name,reviewed_by,approved_by,ext_entity,logged_date,logged_by from  t_training_plan where id_training_plan='" + id_training_plan + "'";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                   // string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetHRDeptEmployees());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(objGlobalData.GetTopMgmtEmployee()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["trainer_name"].ToString());

                    string from_date = "", to_date="";
                    if (dsData.Tables[0].Rows[0]["from_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["from_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        from_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["from_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["to_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["to_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        to_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["to_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobalData.GetMultiCompanyBranchNameById(dsData.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetMultiDeptNameById(dsData.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobalData.GetDivisionLocationById(dsData.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Training topic:<b></td> <td colspan=3>" + objGlobalData.GetTrainingTopicById(dsData.Tables[0].Rows[0]["topic"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Employee names:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["emp_id"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Training duration From Date:<b></td> <td colspan=3>" + from_date + "</td></tr>"

                    + "<tr><td colspan=3><b>Training duration To Date:<b></td> <td colspan=3>" + to_date + "</td></tr>"


                    + "<tr><td colspan=3><b>Training source:<b></td> <td colspan=3>" + objGlobalData.GetDropdownitemById(dsData.Tables[0].Rows[0]["source_id"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Trainer Name:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["trainer_name"].ToString()) + "</td></tr>" 

                    +"<tr><td colspan=3><b>Entity Name:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["ext_entity"].ToString()) + "</td></tr>";


                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Training Plan Details");
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
                objGlobalData.AddFunctionalLog("Exception in sendTrainingPlanMail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateTrainingPlan(TrainingPlanModels objModel)
        {
            try
            {


                string sSqlstmt = "update t_training_plan set division= '" + division + "',department = '" + department + "',location = '" + location + "',topic = '" + topic + "',emp_id = '" + emp_id + "',source_id = '" + source_id + "',trainer_name = '" + trainer_name + "',reviewed_by = '" + reviewed_by + "',approved_by = '" + approved_by + "',ext_entity = '" + ext_entity + "'";

                if (objModel.from_date != null && objModel.from_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",from_date='" + objModel.from_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.to_date != null && objModel.to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",to_date='" + objModel.to_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_training_plan='" + objModel.id_training_plan + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingPlan: " + ex.ToString());
            }
            return false;
        }

    }

    public class TrainingPlanModelsList
    {
        public List<TrainingPlanModels> ObjList { get; set; }
    }
}