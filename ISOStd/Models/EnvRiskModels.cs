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
using System.IO;

namespace ISOStd.Models
{
    public class EnvRiskModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [Display(Name = "id")]
        public string id_env_risk { get; set; }

        public string id_env_risk_trans { get; set; }

        [Display(Name = "Report No")]
        public string env_refno { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        [Display(Name = "Division")]
        public string branch_id { get; set; }

        [Display(Name = "Location where activity performed")]
        public string Location { get; set; }

        [Display(Name = "Activity")]
        public string activity { get; set; }

        [Display(Name = "Source of the Environmental Aspect")]
        public string source_id { get; set; }

        [Display(Name = "Products / machines associated")]
        public string product { get; set; }

        [Display(Name = "E. Aspects Identified")]
        public string aspects { get; set; }

        [Display(Name = "Environmental Impact")]
        public string impact { get; set; }

        [Display(Name = "Area affected")]
        public string area_affected { get; set; }

        [Display(Name = "Activity type")]
        public string activity_type { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        [Display(Name = "Reported Date")]
        public DateTime reported_date { get; set; }

        [Display(Name = "Risk Rating(Severity*Probability)")]
        public string RiskRating { get; set; }

        [Display(Name = "Color Code")]
        public string color_code { get; set; }

        [Display(Name = "Mitigation Measures Proposed")]
        public string mitmeasure { get; set; }

        [Display(Name = "Further E.Aspects identified")]
        public string further_consequences { get; set; }

        [Display(Name = "Type of Operational Control")]
        public string op_control { get; set; }

        [Display(Name = "Engineering")]
        public string cnt_engineering { get; set; }

        [Display(Name = "Administrative")]
        public string cnt_administrative { get; set; }

        [Display(Name = "PPE")]
        public string cnt_ppe { get; set; }

        [Display(Name = "General")]
        public string cnt_general { get; set; }

        [Display(Name = "Applicable legal requirement(s)")]
        public string legal { get; set; }

        [Display(Name = "Legal violation, if any")]
        public string legal_voilation { get; set; }

        [Display(Name = "Evaluated By")]
        public string evaluated_by { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime evaluation_date { get; set; }

        [Display(Name = "Notified To")]
        public string eval_notified_to { get; set; }

        [Display(Name = "Severity (Extent of Impact)")]
        public string impact_id { get; set; }

        [Display(Name = "Likelihood of occurrence")]
        public string like_id { get; set; }

        public string op_control1 { get; set; }
        public string op_control2 { get; set; }
        public string op_control3 { get; set; }
        public string op_control4 { get; set; }

        [Display(Name = "Risk evaluated by")]
        public string mit_evaluated_by { get; set; }

        [Display(Name = "Notified to")]
        public string mit_notified_to { get; set; }

        [Display(Name = "Proposed date")]
        public DateTime proposed_date { get; set; }

        [Display(Name = "Risk Reevaluation Due Date")]
        public DateTime reeval_due_date { get; set; }

        [Display(Name = "Severity")]
        public string initimpact_id { get; set; }

        [Display(Name = "Probability")]
        public string initlike_id { get; set; }

        [Display(Name = "Initial Evaluation Date")]
        public DateTime initevaluation_date { get; set; }

        [Display(Name = "id")]
        public string mit_id { get; set; }

        public string mit_id_trans { get; set; }

        [Display(Name = "Control Type")]
        public string cnt_type { get; set; }

        [Display(Name = "Control Description")]
        public string cnt_desc { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        [Display(Name = "Risk due to")]
        public string Issue { get; set; }

        internal Dictionary<string, string> GetRiskRatings(string sImp_id, string sLikeliId)
        {
            Dictionary<string, string> lstRatings = new Dictionary<string, string>();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                DataSet dsData = new DataSet();
                MySqlCommand cmd = new MySqlCommand("p_GetRiskRatings", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@vimpact_id", sImp_id);
                cmd.Parameters.AddWithValue("@vlike_id", sLikeliId);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstRatings.Add(dsData.Tables[0].Rows[0]["rate_desc"].ToString(), dsData.Tables[0].Rows[0]["color_code"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetRiskRatings: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return lstRatings;
        }

        internal string GetRiskSourceNameById(string sSource_id)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                   + "and header_desc='Risk-source' and (item_id='" + sSource_id + "' or item_desc='" + sSource_id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetRiskSourceNameById: " + ex.ToString());
            }

            return "";
        }

        public MultiSelectList GetMultiRiskSourceList()
        {
            DropdownRiskList lstImpact = new DropdownRiskList();
            lstImpact.lstMitigationimpact = new List<DropdownRiskItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Risk-source' order by item_desc asc";
                DataSet dsEmp = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownRiskItems reg = new DropdownRiskItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstImpact.lstMitigationimpact.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetMultiRiskSourceList: " + ex.ToString());
            }

            return new MultiSelectList(lstImpact.lstMitigationimpact, "Id", "Name");
        }

        internal bool FunAddHazard(EnvRiskModels HazardModels)
        {
            try
            {

                string sSqlstmt = "insert into t_environment_risk(dept,branch_id,Location,source_id,activity,product,aspects,impact,area_affected,notified_to,reported_by,activity_type,Issue";
                string sFields = "", sFieldValue = "";

                if (HazardModels.reported_date != null && HazardModels.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", reported_date";
                    sFieldValue = sFieldValue + ", '" + HazardModels.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + HazardModels.dept + "','" + HazardModels.branch_id + "','" + HazardModels.Location + "','" + HazardModels.source_id + "','" + HazardModels.activity +
                    "','" + HazardModels.product + "','" + HazardModels.aspects + "','" + HazardModels.impact + "','" + HazardModels.area_affected + "','" + HazardModels.notified_to + "','" + HazardModels.reported_by + "','" + HazardModels.activity_type + "','" + HazardModels.Issue + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_env_risk = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_env_risk))
                {
                    DataSet dsData = objGlobaldata.GetReportNo("ENVRISK", "", objGlobaldata.GetCompanyBranchNameById(branch_id));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        env_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_environment_risk set env_refno='" + env_refno + "' where id_env_risk='" + id_env_risk + "'";
                    objGlobaldata.ExecuteQuery(sql1);
                    return SendEnvRiskmail(id_env_risk, "Environmental Risk Assessment Reporting");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddHazard: " + ex.ToString());
            }
            return false;
        }

        internal bool SendEnvRiskmail(int id_env_risk, string sMessage = "")
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select env_refno,branch_id,dept,Location,source_id,Issue,impact,area_affected,activity,product,aspects,reported_date,reported_by,notified_to,activity_type"
                + " from t_environment_risk where id_env_risk='" + id_env_risk + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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

                    //sName = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["reported_by"].ToString());

                    string reported_date = "";
                    if (dsData.Tables[0].Rows[0]["reported_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reported_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reported_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reported_date"].ToString()).ToString("yyyy-MM-dd");
                    }

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["env_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>"
                      + objGlobaldata.GetMultiCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsData.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Location where activity performed:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsData.Tables[0].Rows[0]["Location"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Source of the Environmental Aspect:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["source_id"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk due to:<b></td> <td colspan=3>" + objRisk.GetIssueNameById(dsData.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Environmental Impact:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Area affected:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["area_affected"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Activity:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["activity"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Products/machines associated:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["product"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>E.Aspects Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["aspects"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Reported Date:<b></td> <td colspan=3>" + reported_date + "</td></tr>"

                      + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["reported_by"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Activity type:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["activity_type"].ToString()) + "</td></tr>";


                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Environmental Risk Assessment");
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

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendEnvRiskmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskEvaluation(EnvRiskModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_environment_risk set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering + "',cnt_administrative='" + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_env_risk='" + objModel.id_env_risk + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return SendEnvInitialRiskEvalmail(id_env_risk, "Initial Environmental Risk Assessment Risk Evaluation") ;
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskEvaluation: " + ex.ToString());
            }

            return false;
        }

        internal bool SendEnvInitialRiskEvalmail(string id_env_risk, string sMessage = "")
        {
            EnvRiskModels objModels = new EnvRiskModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select env_refno,impact,further_consequences,impact_id,like_id,legal,legal_voilation,evaluated_by,eval_notified_to,evaluation_date"
                + " from t_environment_risk where id_env_risk='" + id_env_risk + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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

                    //sName = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["eval_notified_to"].ToString());
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["evaluated_by"].ToString());

                    string evaluation_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }


                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["env_refno"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Environmental Impact:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Further consequences identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["further_consequences"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Severity (Extent of Impact):<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Likelihood of occurrence:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                       + "<tr><td colspan=3><b>Applicable legal requirement(s):<b></td> <td colspan=3>" + objGlobaldata.GetLawNoById(dsData.Tables[0].Rows[0]["legal"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Legal violation, if any:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["legal_voilation"].ToString()) + "</td></tr>"

                        + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["evaluated_by"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation  Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>";



                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Environmental Risk Assessment");
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

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendEnvInitialRiskEvalmail: " + ex.ToString());
            }
            return false;
        }



        internal bool FunUpdateRiskMitigation(EnvRiskModels objModel, EnvRiskModelsList objRiskList)
        {
            try
            {

                string sSqlstmt = "update t_environment_risk set mit_evaluated_by='" + objModel.mit_evaluated_by + "',mit_notified_to='" + objModel.mit_notified_to + "'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.proposed_date != null && objModel.proposed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",proposed_date='" + objModel.proposed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_env_risk='" + objModel.id_env_risk + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstHazard.Count) > 0)
                    {
                        objRiskList.lstHazard[0].id_env_risk = id_env_risk.ToString();
                        FunAddMitigationList(objRiskList);
                    }
                    return SendEnvMitigationmail(id_env_risk, " Environmental Risk Assessment Risk Mitigation Measures");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskMitigation: " + ex.ToString());
            }
            return false;
        }

        internal bool SendEnvMitigationmail(string id_env_risk, string sMessage = "")
        {
            EnvRiskModels objModels = new EnvRiskModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select env_refno,branch_id,dept,Location,source_id,Issue,impact,area_affected,activity,product,aspects,reported_date,reported_by,activity_type,"
                +"impact_id,like_id,mit_evaluated_by,proposed_date,reeval_due_date,mit_notified_to from t_environment_risk where id_env_risk = '"+ id_env_risk + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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

                    //sName = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["mit_notified_to"].ToString());
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }

                    string proposed_date = "", reeval_due_date = "";
                    if (dsData.Tables[0].Rows[0]["proposed_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["proposed_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        proposed_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["proposed_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["env_refno"].ToString()) + "</td></tr>"

                  + "<tr><td colspan=3><b>Environmental Impact:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk evaluated by:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Proposed Date:<b></td> <td colspan=3>" + proposed_date + "</td></tr>"


                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations where id_env_risk = '" + id_env_risk + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation Measures</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Control Type</th>"
                              + "<th style='width:300px'>Control Description</th>"
                            + "<th style='width:300px'>Person Responsible</th>"
                            + "<th style='width:300px'>Target Date</th>"
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string target_date = "";
                            if (dsItems.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                target_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy-MM-dd");
                            }

                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["cnt_type"].ToString()) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["cnt_desc"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + target_date + "</td>"
                                 + " </tr>";
                            //sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());

                            //string sPersonEmail = objGlobaldata.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                            //sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                            //sPersonEmail = sPersonEmail.Trim();
                            //sPersonEmail = sPersonEmail.TrimEnd(',');
                            //sPersonEmail = sPersonEmail.TrimStart(',');
                            //if (sPersonEmail != null && sPersonEmail != "")
                            //{
                            //    sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                            //}

                        }
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Environmental Risk Assessment");
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

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendEnvMitigationmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddMitigationList(EnvRiskModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from t_environment_risk_mitigations where id_env_risk='" + objRiskList.lstHazard[0].id_env_risk + "'; ";

                for (int i = 0; i < objRiskList.lstHazard.Count; i++)
                {

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    string smit_id = "null";
                    sSqlstmt = sSqlstmt + " insert into t_environment_risk_mitigations (mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp";
                    if (objRiskList.lstHazard[i].mit_id != null)
                    {
                        smit_id = objRiskList.lstHazard[i].mit_id;
                    }
                    if (objRiskList.lstHazard[i].target_date != null && objRiskList.lstHazard[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", target_date= values(target_date)";
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objRiskList.lstHazard[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id + "," + objRiskList.lstHazard[0].id_env_risk
                    + ",'" + objRiskList.lstHazard[i].cnt_type + "','" + objRiskList.lstHazard[i].cnt_desc + "','" + objRiskList.lstHazard[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " mit_id= values(mit_id), id_env_risk= values(id_env_risk), cnt_type = values(cnt_type),cnt_desc = values(cnt_desc), pers_resp= values(pers_resp)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddMitigationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskReEvaluation(EnvRiskModels objModel)
        {
            try
            {
                DateTime today_date = DateTime.Now;
                string sSqlstmt = "select id_env_risk,reeval_due_date from t_environment_risk_trans where id_env_risk_trans='"
                       + id_env_risk_trans + "' order by id_env_risk_trans desc limit 1";
                DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                {
                    DateTime dtValue;

                    if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                    {
                        if (today_date >= dtValue)
                        {
                            sSqlstmt = "insert into t_environment_risk_trans(id_env_risk,impact_id,like_id,env_refno,further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,legal,legal_voilation,evaluated_by,eval_notified_to";
                            string sFields = "", sFieldValue = "";

                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sFields = sFields + ", evaluation_date";
                                sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }
                            sSqlstmt = sSqlstmt + sFields;
                            sSqlstmt = sSqlstmt + ") values('" + objModel.id_env_risk + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.env_refno + "','" + objModel.further_consequences + "','" + objModel.op_control + "'"
                                + ",'" + objModel.cnt_engineering + "','" + objModel.cnt_administrative + "','" + objModel.cnt_ppe + "','" + objModel.cnt_general + "','" + objModel.legal + "','" + objModel.legal_voilation + "','" + objModel.evaluated_by + "'"
                                + ",'" + objModel.eval_notified_to + "'";

                            sSqlstmt = sSqlstmt + sFieldValue + ")";
                            int sid_env_risk_trans = 0;

                            if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out sid_env_risk_trans))
                            {
                                string sSqlstmt1 = "select id_env_risk_trans,mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations_trans where id_env_risk_trans='" + sid_env_risk_trans + "'";
                                DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                                if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                                {
                                    sSqlstmt = "";
                                    for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                                    {
                                        sSqlstmt = sSqlstmt + "insert into t_environment_risk_mitigations_trans(id_env_risk_trans,mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp";

                                        string sFieldValues = "", sField = "";
                                        if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                        {
                                            sField = sField + ", target_date";
                                            sFieldValues = sFieldValues + ", '" + Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                        }
                                        sSqlstmt = sSqlstmt + sField;
                                        sSqlstmt = sSqlstmt + ") values('" + sid_env_risk_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["id_env_risk"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_type"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                        sSqlstmt = sSqlstmt + sFieldValues + ");";
                                    }
                                    if (objGlobaldata.ExecuteQuery(sSqlstmt))
                                    {
                                        return SendEnvRiskReEvalmail(id_env_risk, " Environmental Risk Assessment ReEvaluation");
                                    }
                                }

                            }
                        }
                        else
                        {

                            sSqlstmt = "update t_environment_risk_trans set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering + "',cnt_administrative='" + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }

                            sSqlstmt = sSqlstmt + " where id_env_risk_trans='" + objModel.id_env_risk_trans + "'";

                            if (objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendEnvRiskReEvalmail(id_env_risk, " Environmental Risk Assessment ReEvaluation");
                            }
                        }

                    }
                    else
                    {
                        sSqlstmt = "update t_environment_risk_trans set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering + "',cnt_administrative='" + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                        if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                        }

                        sSqlstmt = sSqlstmt + " where id_env_risk_trans='" + objModel.id_env_risk_trans + "'";

                        if (objGlobaldata.ExecuteQuery(sSqlstmt))
                        {
                            return SendEnvRiskReEvalmail(id_env_risk, " Environmental Risk Assessment ReEvaluation");
                        }
                    }

                }
                else
                {
                    sSqlstmt = "insert into t_environment_risk_trans(id_env_risk,impact_id,like_id,env_refno,further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,legal,legal_voilation,evaluated_by,eval_notified_to";
                    string sFields = "", sFieldValue = "";

                    if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", evaluation_date";
                        sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModel.id_env_risk + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.env_refno + "','" + objModel.further_consequences + "','" + objModel.op_control + "'"
                        + ",'" + objModel.cnt_engineering + "','" + objModel.cnt_administrative + "','" + objModel.cnt_ppe + "','" + objModel.cnt_general + "','" + objModel.legal + "','" + objModel.legal_voilation + "','" + objModel.evaluated_by + "'"
                        + ",'" + objModel.eval_notified_to + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    int sid_env_risk_trans = 0;

                    if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out sid_env_risk_trans))
                    {
                        string sSqlstmt1 = "select mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations where id_env_risk='" + id_env_risk + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            sSqlstmt = "";
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                sSqlstmt = sSqlstmt + "insert into t_environment_risk_mitigations_trans(id_env_risk_trans,mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp";

                                string sFieldValues = "", sField = "";
                                if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                {
                                    sField = sField + ", target_date";
                                    sFieldValues = sFieldValues + ", '" + Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                }
                                sSqlstmt = sSqlstmt + sField;
                                sSqlstmt = sSqlstmt + ") values('" + sid_env_risk_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["id_env_risk"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_type"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                sSqlstmt = sSqlstmt + sFieldValues + ");";
                            }
                            if (objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendEnvRiskReEvalmail(id_env_risk, " Environmental Risk Assessment ReEvaluation");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskReEvaluation: " + ex.ToString());
            }

            return false;
        }

        internal bool SendEnvRiskReEvalmail(string id_env_risk, string sMessage = "")
        {
            EnvRiskModels objModels = new EnvRiskModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select t.env_refno,t.impact,tt.further_consequences,tt.impact_id,tt.like_id,tt.legal,tt.legal_voilation,tt.evaluated_by,tt.eval_notified_to,tt.evaluation_date"
               + " from t_environment_risk t left join  t_environment_risk_trans tt on t.id_env_risk = tt.id_env_risk where t.id_env_risk = '" + id_env_risk + "' order by id_env_risk_trans desc limit 1";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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

                    //sName = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["eval_notified_to"].ToString());
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["evaluated_by"].ToString());

                    string evaluation_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }


                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["env_refno"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Environmental Impact:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Further consequences identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["further_consequences"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Severity (Extent of Impact):<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Likelihood of occurrence:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                       + "<tr><td colspan=3><b>Applicable legal requirement(s):<b></td> <td colspan=3>" + objGlobaldata.GetLawNoById(dsData.Tables[0].Rows[0]["legal"].ToString()) + "</td></tr>"

                       + "<tr><td colspan=3><b>Legal violation, if any:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["legal_voilation"].ToString()) + "</td></tr>"

                        + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["evaluated_by"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Evaluation  Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>";



                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Environmental Risk Assessment");
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

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendEnvRiskReEvalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateFurtherRiskMitigation(EnvRiskModels objModel, EnvRiskModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "update t_environment_risk_trans set mit_evaluated_by='" + objModel.mit_evaluated_by + "',mit_notified_to='" + objModel.mit_notified_to + "'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.proposed_date != null && objModel.proposed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",proposed_date='" + objModel.proposed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_env_risk_trans='" + objModel.id_env_risk_trans + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstHazard.Count) > 0)
                    {
                        objRiskList.lstHazard[0].id_env_risk_trans = id_env_risk_trans.ToString();
                        objRiskList.lstHazard[0].id_env_risk = id_env_risk.ToString();
                        FunAddFurtherMitigationList(objRiskList);
                        return SendEnvFurtherMitigationmail(id_env_risk, "Environmental Risk Assessment Further Mitigation Measures");
                    }

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateFurtherRiskMitigation: " + ex.ToString());
            }
            return false;
        }

        internal bool SendEnvFurtherMitigationmail(string id_env_risk, string sMessage = "")
        {
            EnvRiskModels objModels = new EnvRiskModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_env_risk_trans,t.env_refno,t.impact,tt.impact_id,tt.like_id,tt.mit_evaluated_by,tt.proposed_date,tt.reeval_due_date,tt.mit_notified_to"
                 + " from t_environment_risk t left join  t_environment_risk_trans tt on t.id_env_risk = tt.id_env_risk where t.id_env_risk = '" + id_env_risk + "' order by id_env_risk_trans desc limit 1";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sSubject = "", sContent = "", aAttachment = "";

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

                    //sName = objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["notified_to"].ToString());
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["mit_notified_to"].ToString());
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }

                    string proposed_date = "", reeval_due_date = "";
                    if (dsData.Tables[0].Rows[0]["proposed_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["proposed_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        proposed_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["proposed_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["env_refno"].ToString()) + "</td></tr>"

                  + "<tr><td colspan=3><b>Environmental Impact:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk evaluated by:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Proposed Date:<b></td> <td colspan=3>" + proposed_date + "</td></tr>"


                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select cnt_type,cnt_desc,pers_resp,target_date from t_environment_risk_mitigations_trans where id_env_risk_trans = '" + dsData.Tables[0].Rows[0]["id_env_risk_trans"].ToString() + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation Measures</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Control Type</th>"
                              + "<th style='width:300px'>Control Description</th>"
                            + "<th style='width:300px'>Person Responsible</th>"
                            + "<th style='width:300px'>Target Date</th>"
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string target_date = "";
                            if (dsItems.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                target_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy-MM-dd");
                            }

                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["cnt_type"].ToString()) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["cnt_desc"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + objGlobaldata.GetMultiHrEmpNameById(dsItems.Tables[0].Rows[i]["pers_resp"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + target_date + "</td>"
                                 + " </tr>";
                            //sCCEmailIds = sCCEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());

                            //string sPersonEmail = objGlobaldata.GetMultiHrEmpEmailIdById(dsItems.Tables[0].Rows[i]["PersonResponsible"].ToString());
                            //sPersonEmail = Regex.Replace(sPersonEmail, ",+", ",");
                            //sPersonEmail = sPersonEmail.Trim();
                            //sPersonEmail = sPersonEmail.TrimEnd(',');
                            //sPersonEmail = sPersonEmail.TrimStart(',');
                            //if (sPersonEmail != null && sPersonEmail != "")
                            //{
                            //    sCCEmailIds = sCCEmailIds + "," + sPersonEmail;
                            //}

                        }
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Environmental Risk Assessment");
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

                    return objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendEnvFurtherMitigationmail: " + ex.ToString());
            }
            return false;
        }



        internal bool FunAddFurtherMitigationList(EnvRiskModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from t_environment_risk_mitigations_trans where id_env_risk_trans='" + objRiskList.lstHazard[0].id_env_risk_trans + "'; ";

                for (int i = 0; i < objRiskList.lstHazard.Count; i++)
                {

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    string smit_id_trans = "null";
                    sSqlstmt = sSqlstmt + " insert into t_environment_risk_mitigations_trans (mit_id_trans,id_env_risk_trans,mit_id,id_env_risk,cnt_type,cnt_desc,pers_resp";
                    if (objRiskList.lstHazard[i].mit_id_trans != null)
                    {
                        smit_id_trans = objRiskList.lstHazard[i].mit_id_trans;
                    }
                    if (objRiskList.lstHazard[i].target_date != null && objRiskList.lstHazard[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", target_date= values(target_date)";
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objRiskList.lstHazard[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id_trans + ",'" + objRiskList.lstHazard[0].id_env_risk_trans + "','" + objRiskList.lstHazard[i].mit_id + "'," + objRiskList.lstHazard[0].id_env_risk
                    + ",'" + objRiskList.lstHazard[i].cnt_type + "','" + objRiskList.lstHazard[i].cnt_desc + "','" + objRiskList.lstHazard[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + "mit_id_trans= values(mit_id_trans),id_env_risk_trans= values(id_env_risk_trans), mit_id= values(mit_id), id_env_risk= values(id_env_risk), cnt_type = values(cnt_type),cnt_desc = values(cnt_desc), pers_resp= values(pers_resp)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddMitigationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateHazard(EnvRiskModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_environment_risk set dept='" + objModel.dept + "',branch_id='" + objModel.branch_id + "',location='" + objModel.Location + "',source_id='" + objModel.source_id + "'"
                    + ",activity_type='" + objModel.activity_type + "',product='" + objModel.product + "',aspects='" + objModel.aspects + "',activity='" + objModel.activity + "',impact='" + objModel.impact + "',reported_by='" + objModel.reported_by + "',notified_to='" + objModel.notified_to + "',Issue='" + objModel.Issue + "'";

                if (objModel.reported_date != null && objModel.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reported_date='" + objModel.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_env_risk='" + objModel.id_env_risk + "'";

                return (objGlobaldata.ExecuteQuery(sSqlstmt));

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateHazard: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeleteHazard(string id_hazard)
        {
            try
            {
                string sSqlstmt = "update t_environment_risk set Active=0 where id_env_risk='" + id_env_risk + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteHazard: " + ex.ToString());
            }
            return false;
        }
    }
    public class EnvRiskModelsList
    {
        public List<EnvRiskModels> lstHazard { get; set; }
    }
}