using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ISOStd.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class HealthSafetyModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        [Display(Name = "Hazard id")]
        public string id_hazard { get; set; }
        public string id_hazard_trans { get; set; }

        [Display(Name = "Report No")]
        public string hazard_refno { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        [Display(Name = "Division")]
        public string branch_id { get; set; }

        [Display(Name = "Location where activity performed")]
        public string Location { get; set; }

        [Display(Name = "Source of the Hazard")]
        public string source_id { get; set; }

        [Display(Name = "Activity type")]
        public string activity_type { get; set; }

        [Display(Name = "Consequences")]
        public string consequences { get; set; }

        [Display(Name = "Extent of injury")]
        public string injury { get; set; }

        [Display(Name = "Activity")]
        public string activity { get; set; }

        [Display(Name = "Health & Safety Hazards Identified")]
        public string hazards { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Reported Date")]
        public DateTime reported_date { get; set; }

        [Display(Name = "Further consequences identified")]
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

        [Display(Name = "Elimination")]
        public string cnt_elimination { get; set; }

        [Display(Name = "Substitution")]
        public string cnt_substitution { get; set; }

        [Display(Name = "Applicable legal requirement(s)")]
        public string legal { get; set; }

        [Display(Name = "Legal violation, if any")]
        public string legal_voilation { get; set; }

        [Display(Name = "Evaluated By")]
        public string evaluated_by { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime evaluation_date { get; set; }

        [Display(Name = "id")]
        public string mit_id { get; set; }

        [Display(Name = "Control Type")]
        public string cnt_type { get; set; }

        [Display(Name = "Control Description")]
        public string cnt_desc { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

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
        public string op_control5 { get; set; }
        public string op_control6 { get; set; }

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

        public string mit_id_trans { get; set; }

        [Display(Name = "Risk Rating(Severity*Probability)")]
        public string RiskRating { get; set; }

        [Display(Name = "Color Code")]
        public string color_code { get; set; }

        [Display(Name = "Mitigation Measures Proposed")]
        public string mitmeasure { get; set; }

        [Display(Name = "Risk due to")]
        public string Issue { get; set; }


        [Display(Name = "Risk Rating(Severity*Probability)")]
        public string RiskRating_curr { get; set; }

        [Display(Name = "Color Code")]
        public string color_code_curr { get; set; }


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
               
        public string GetLikelihoodNameById(string sLike_id)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Risk-likelihood' and (item_id='" + sLike_id + "' or item_desc='" + sLike_id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetLikelihoodNameById: " + ex.ToString());
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

        internal bool FunAddHazard(HealthSafetyModels HazardModels)
        {
            try
            {

                string sSqlstmt = "insert into t_hazard(dept,branch_id,Location,source_id,activity_type,consequences,injury,activity,hazards,notified_to,reported_by,Issue";
                string sFields = "", sFieldValue = "";

                if (HazardModels.reported_date != null && HazardModels.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", reported_date";
                    sFieldValue = sFieldValue + ", '" + HazardModels.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + HazardModels.dept + "','" + HazardModels.branch_id + "','" + HazardModels.Location + "','" + HazardModels.source_id + "','" + HazardModels.activity_type +
                    "','" + HazardModels.consequences + "','" + HazardModels.injury + "','" + HazardModels.activity + "','" + HazardModels.hazards + "','" + HazardModels.notified_to + "','" + HazardModels.reported_by + "','" + HazardModels.Issue + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_hazard = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_hazard))
                {
                    DataSet dsData = objGlobaldata.GetReportNo("HAZARD", "", objGlobaldata.GetCompanyBranchNameById(branch_id));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        hazard_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_hazard set hazard_refno='" + hazard_refno + "' where id_hazard='" + id_hazard + "'";
                    objGlobaldata.ExecuteQuery(sql1);
                    return SendHazardmail(id_hazard, "Health & Safety Hazard Reporting");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddHazard: " + ex.ToString());
            }
            return false;
        }
        internal bool SendHazardmail(int id_hazard, string sMessage = "")
        {
            RiskMgmtModels objRisk = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select hazard_refno,branch_id,dept,Location,source_id,Issue,activity_type,consequences,injury,activity,hazards,reported_date,reported_by,notified_to"
                + " from t_hazard where id_hazard='" + id_hazard + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazard_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>"
                      + objGlobaldata.GetMultiCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsData.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Location where activity performed:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsData.Tables[0].Rows[0]["Location"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Source of the Hazard:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["source_id"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk due to:<b></td> <td colspan=3>" + objRisk.GetIssueNameById(dsData.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Activity type:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["activity_type"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Consequences:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["consequences"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Extent of injury:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["injury"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Activity:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["activity"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Health & Safety Hazards Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazards"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Reported Date:<b></td> <td colspan=3>" + reported_date + "</td></tr>"

                      + "<tr><td colspan=3><b>Reported By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["reported_by"].ToString()) + "</td></tr>";


                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Health & Safety Hazard");
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
                objGlobaldata.AddFunctionalLog("Exception in SendHazardmail: " + ex.ToString());
            }
            return false;
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

        internal bool FunUpdateRiskEvaluation(HealthSafetyModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_hazard set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering
                    + "',cnt_administrative='" + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id 
                    + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by 
                    + "',eval_notified_to='" + objModel.eval_notified_to + "',cnt_elimination='" + objModel.cnt_elimination + "',cnt_substitution='" + objModel.cnt_substitution + "'";
                if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_hazard='" + objModel.id_hazard + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return SendHazardInitialRiskEvalmail(id_hazard, "Initial Health & Safety Hazard Risk Evaluation");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskEvaluation: " + ex.ToString());
            }

            return false;
        }

        internal bool SendHazardInitialRiskEvalmail(string id_hazard, string sMessage = "")
        {
            HealthSafetyModels objModels = new HealthSafetyModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select hazard_refno,hazards,further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,impact_id,like_id,legal,legal_voilation,evaluated_by,eval_notified_to,"
                +"cnt_elimination,cnt_substitution,evaluation_date from t_hazard where id_hazard='" + id_hazard + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazard_refno"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Health & Safety Hazards Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazards"].ToString()) + "</td></tr>"

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
                    sContent = sContent.Replace("{Title}", "Health & Safety Hazard");
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
                objGlobaldata.AddFunctionalLog("Exception in SendHazardInitialRiskEvalmail: " + ex.ToString());
            }
            return false;
        }



        internal bool FunUpdateRiskMitigation(HealthSafetyModels objModel, HealthSafetyModelsList objRiskList)
        {
            try
            {
                
                string sSqlstmt = "update t_hazard set mit_evaluated_by='" + objModel.mit_evaluated_by + "',mit_notified_to='" + objModel.mit_notified_to + "'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.proposed_date != null && objModel.proposed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",proposed_date='" + objModel.proposed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_hazard='" + objModel.id_hazard + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstHazard.Count) > 0)
                    {
                        objRiskList.lstHazard[0].id_hazard = id_hazard.ToString();
                        FunAddMitigationList(objRiskList);
                       return SendHazardMitigationmail(id_hazard, " Health & Safety Hazard Risk Mitigation Measures");
                    }                 
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskMitigation: " + ex.ToString());
            }
            return false;
        }
        internal bool SendHazardMitigationmail(string id_hazard, string sMessage = "")
        {
            HealthSafetyModels objModels = new HealthSafetyModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select hazard_refno,branch_id,dept,Location,source_id,Issue,activity_type,consequences,injury,activity,hazards,reported_date,reported_by,notified_to,mit_evaluated_by,proposed_date,reeval_due_date,mit_notified_to,impact_id,like_id"
            + " from t_hazard where id_hazard='" + id_hazard + "'";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazard_refno"].ToString()) + "</td></tr>"

                         + "<tr><td colspan=3><b>Health & Safety Hazards Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazards"].ToString()) + "</td></tr>"

                        + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk evaluated by:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Proposed Date:<b></td> <td colspan=3>" + proposed_date + "</td></tr>"

                  
                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations where id_hazard = '" + id_hazard + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
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
                    sContent = sContent.Replace("{Title}", "Health & Safety Hazard");
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
                objGlobaldata.AddFunctionalLog("Exception in SendHazardMitigationmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddMitigationList(HealthSafetyModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from t_hazard_mitigations where id_hazard='" + objRiskList.lstHazard[0].id_hazard + "'; ";

                for (int i = 0; i < objRiskList.lstHazard.Count; i++)
                {
                  
                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    string smit_id = "null";
                    sSqlstmt = sSqlstmt + " insert into t_hazard_mitigations (mit_id,id_hazard,cnt_type,cnt_desc,pers_resp";
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
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id + "," + objRiskList.lstHazard[0].id_hazard
                    + ",'" + objRiskList.lstHazard[i].cnt_type + "','" + objRiskList.lstHazard[i].cnt_desc + "','" + objRiskList.lstHazard[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " mit_id= values(mit_id), id_hazard= values(id_hazard), cnt_type = values(cnt_type),cnt_desc = values(cnt_desc), pers_resp= values(pers_resp)";
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

        internal bool FunUpdateRiskReEvaluation(HealthSafetyModels objModel)
        {
            try
            {
                DateTime today_date = DateTime.Now;
                string sSqlstmt = "select id_hazard,reeval_due_date from t_hazard_trans where id_hazard_trans='"
                     + id_hazard_trans + "' order by id_hazard_trans desc limit 1";
                DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                {
                    DateTime dtValue;

                    if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                    {
                        if (today_date >= dtValue)
                        {
                            sSqlstmt = "insert into t_hazard_trans(id_hazard,impact_id,like_id,hazard_refno,further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,legal,legal_voilation,evaluated_by,eval_notified_to," +
                                "cnt_elimination,cnt_substitution";
                            string sFields = "", sFieldValue = "";

                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sFields = sFields + ", evaluation_date";
                                sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }
                            sSqlstmt = sSqlstmt + sFields;
                            sSqlstmt = sSqlstmt + ") values('" + objModel.id_hazard + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.hazard_refno + "','" + objModel.further_consequences + "','" + objModel.op_control + "'"
                                + ",'" + objModel.cnt_engineering + "','" + objModel.cnt_administrative + "','" + objModel.cnt_ppe + "','" + objModel.cnt_general + "','" + objModel.legal + "','" + objModel.legal_voilation + "','" + objModel.evaluated_by + "'"
                                + ",'" + objModel.eval_notified_to + "','" + objModel.cnt_elimination + "','" + objModel.cnt_substitution + "'";

                            sSqlstmt = sSqlstmt + sFieldValue + ")";
                            int sid_hazard_trans = 0;

                            if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out sid_hazard_trans))
                            {
                                string sSqlstmt1 = "select id_hazard_trans,mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations_trans where id_hazard_trans='" + sid_hazard_trans + "'";
                                DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                                if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                                {
                                    sSqlstmt = "";
                                    for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                                    {
                                        sSqlstmt = sSqlstmt + "insert into t_hazard_mitigations_trans(id_hazard_trans,mit_id,id_hazard,cnt_type,cnt_desc,pers_resp";

                                        string sFieldValues = "", sField = "";
                                        if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                        {
                                            sField = sField + ", target_date";
                                            sFieldValues = sFieldValues + ", '" + Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                        }
                                        sSqlstmt = sSqlstmt + sField;
                                        sSqlstmt = sSqlstmt + ") values('" + sid_hazard_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["id_hazard"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_type"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                        sSqlstmt = sSqlstmt + sFieldValues + ");";
                                    }

                                    if (objGlobaldata.ExecuteQuery(sSqlstmt))
                                    {
                                        return SendHazardRiskReEvalmail(id_hazard, "Health & Safety Hazard Risk ReEvaluation");
                                    }
                                    }

                            }
                        }
                        else
                        {
       
                            sSqlstmt = "update t_hazard_trans set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering + "',cnt_administrative='" + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe 
                                + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by 
                                + "',eval_notified_to='" + objModel.eval_notified_to + "',cnt_elimination='" + objModel.cnt_elimination + "',cnt_substitution='" + objModel.cnt_substitution + "'";
                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }

                            sSqlstmt = sSqlstmt + " where id_hazard_trans='" + objModel.id_hazard_trans + "'";

                            if (objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendHazardRiskReEvalmail(id_hazard, "Health & Safety Hazard Risk ReEvaluation");
                            }
                        }

                    }
                    else
                    {
                        sSqlstmt = "update t_hazard_trans set further_consequences='" + objModel.further_consequences + "',op_control='" + objModel.op_control + "',cnt_engineering='" + objModel.cnt_engineering + "',cnt_administrative='"
                            + objModel.cnt_administrative + "',cnt_ppe='" + objModel.cnt_ppe + "',cnt_general='" + objModel.cnt_general + "',impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',legal='" + objModel.legal
                            + "',legal_voilation='" + objModel.legal_voilation + "',evaluated_by='" + objModel.evaluated_by + "',eval_notified_to='" + objModel.eval_notified_to + "',cnt_elimination='" + objModel.cnt_elimination + "',cnt_substitution='" + objModel.cnt_substitution + "'";
                        if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                        }

                        sSqlstmt = sSqlstmt + " where id_hazard_trans='" + objModel.id_hazard_trans + "'";

                        if (objGlobaldata.ExecuteQuery(sSqlstmt))
                        {
                            return SendHazardRiskReEvalmail(id_hazard, "Health & Safety Hazard Risk ReEvaluation");
                        }
                    }

                }
                else
                {
                    sSqlstmt = "insert into t_hazard_trans(id_hazard,impact_id,like_id,hazard_refno,further_consequences,op_control,cnt_engineering,cnt_administrative,cnt_ppe,cnt_general,legal,legal_voilation,evaluated_by,eval_notified_to,cnt_elimination,cnt_substitution";
                    string sFields = "", sFieldValue = "";

                    if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", evaluation_date";
                        sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModel.id_hazard + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.hazard_refno + "','" + objModel.further_consequences + "','" + objModel.op_control + "'"
                        + ",'" + objModel.cnt_engineering + "','" + objModel.cnt_administrative + "','" + objModel.cnt_ppe + "','" + objModel.cnt_general + "','" + objModel.legal + "','" + objModel.legal_voilation + "','" + objModel.evaluated_by + "'"
                        + ",'" + objModel.eval_notified_to + "','" + objModel.cnt_elimination + "','" + objModel.cnt_substitution + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    int sid_hazard_trans = 0;

                    if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out sid_hazard_trans))
                    {
                        string sSqlstmt1 = "select mit_id,id_hazard,cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations where id_hazard='" + id_hazard + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            sSqlstmt = "";
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                sSqlstmt = sSqlstmt + "insert into t_hazard_mitigations_trans(id_hazard_trans,mit_id,id_hazard,cnt_type,cnt_desc,pers_resp";

                                string sFieldValues = "", sField = "";
                                if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                {
                                    sField = sField + ", target_date";
                                    sFieldValues = sFieldValues + ", '" + Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                }
                                sSqlstmt = sSqlstmt + sField;
                                sSqlstmt = sSqlstmt + ") values('" + sid_hazard_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["id_hazard"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_type"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["cnt_desc"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                sSqlstmt = sSqlstmt + sFieldValues + ");";
                            }
                            if(objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendHazardRiskReEvalmail(id_hazard, "Health & Safety Hazard Risk ReEvaluation");
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

        internal bool SendHazardRiskReEvalmail(string id_hazard, string sMessage = "")
        {
            HealthSafetyModels objModels = new HealthSafetyModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select t.hazard_refno,t.hazards,tt.further_consequences,tt.impact_id,tt.like_id,"
                +"tt.legal,tt.legal_voilation,tt.evaluated_by,tt.eval_notified_to,tt.evaluation_date"
                +" from t_hazard t left join  t_hazard_trans tt on t.id_hazard = tt.id_hazard where t.id_hazard = '"+ id_hazard + "' order by id_hazard_trans desc limit 1; ";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazard_refno"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Health & Safety Hazards Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazards"].ToString()) + "</td></tr>"

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
                    sContent = sContent.Replace("{Title}", "Health & Safety Hazard");
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
                objGlobaldata.AddFunctionalLog("Exception in SendHazardRiskReEvalmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateFurtherRiskMitigation(HealthSafetyModels objModel, HealthSafetyModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "update t_hazard_trans set mit_evaluated_by='" + objModel.mit_evaluated_by + "',mit_notified_to='" + objModel.mit_notified_to + "'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModel.proposed_date != null && objModel.proposed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",proposed_date='" + objModel.proposed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_hazard_trans='" + objModel.id_hazard_trans + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstHazard.Count) > 0)
                    {
                        objRiskList.lstHazard[0].id_hazard_trans = id_hazard_trans.ToString();
                        objRiskList.lstHazard[0].id_hazard = id_hazard.ToString();
                        FunAddFurtherMitigationList(objRiskList);
                        return SendHazardFurtherMitigationmail(id_hazard, " Health & Safety Hazard Risk Further Mitigation Measures");
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

        internal bool SendHazardFurtherMitigationmail(string id_hazard, string sMessage = "")
        {
            HealthSafetyModels objModels = new HealthSafetyModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_hazard_trans,t.hazard_refno,t.hazards,tt.further_consequences,tt.impact_id,tt.like_id,"
                + "tt.legal,tt.legal_voilation,tt.evaluated_by,tt.eval_notified_to,tt.evaluation_date,tt.mit_notified_to,tt.mit_evaluated_by,tt.proposed_date,tt.reeval_due_date"
                + " from t_hazard t left join  t_hazard_trans tt on t.id_hazard = tt.id_hazard where t.id_hazard = '"+ id_hazard + "' order by id_hazard_trans desc limit 1";

                DataSet dsData = objGlobaldata.Getdetails(sSqlstmt);
                KPIModels objType = new KPIModels();

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    //objGlobalData.AddFunctionalLog("Step");
                    //AddFunctionalLog("Step");
                    string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazard_refno"].ToString()) + "</td></tr>"

                         + "<tr><td colspan=3><b>Health & Safety Hazards Identified:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["hazards"].ToString()) + "</td></tr>"

                        + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objModels.RiskRating + "</td></tr>"

                      + "<tr><td colspan=3><b>Risk evaluated by:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["mit_evaluated_by"].ToString()) + "</td></tr>"

                      + "<tr><td colspan=3><b>Proposed Date:<b></td> <td colspan=3>" + proposed_date + "</td></tr>"


                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select cnt_type,cnt_desc,pers_resp,target_date from t_hazard_mitigations_trans where id_hazard_trans = '" + dsData.Tables[0].Rows[0]["id_hazard_trans"].ToString() + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
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
                    sContent = sContent.Replace("{Title}", "Health & Safety Hazard");
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
                objGlobaldata.AddFunctionalLog("Exception in SendHazardFurtherMitigationmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddFurtherMitigationList(HealthSafetyModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from t_hazard_mitigations_trans where id_hazard_trans='" + objRiskList.lstHazard[0].id_hazard_trans + "'; ";

                for (int i = 0; i < objRiskList.lstHazard.Count; i++)
                {

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    string smit_id_trans = "null";
                    sSqlstmt = sSqlstmt + " insert into t_hazard_mitigations_trans (mit_id_trans,id_hazard_trans,mit_id,id_hazard,cnt_type,cnt_desc,pers_resp";
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
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id_trans + ",'" + objRiskList.lstHazard[0].id_hazard_trans + "','" + objRiskList.lstHazard[i].mit_id + "'," + objRiskList.lstHazard[0].id_hazard
                    + ",'" + objRiskList.lstHazard[i].cnt_type + "','" + objRiskList.lstHazard[i].cnt_desc + "','" + objRiskList.lstHazard[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + "mit_id_trans= values(mit_id_trans),id_hazard_trans= values(id_hazard_trans), mit_id= values(mit_id), id_hazard= values(id_hazard), cnt_type = values(cnt_type),cnt_desc = values(cnt_desc), pers_resp= values(pers_resp)";
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

        internal bool FunUpdateHazard(HealthSafetyModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_hazard set dept='" + objModel.dept + "',branch_id='" + objModel.branch_id + "',location='" + objModel.Location + "',source_id='" + objModel.source_id + "'"
                    + ",activity_type='" + objModel.activity_type + "',consequences='" + objModel.consequences + "',injury='" + objModel.injury + "',activity='" + objModel.activity + "',hazards='" + objModel.hazards + "',reported_by='" + objModel.reported_by + "',notified_to='" + objModel.notified_to + "'";

                if (objModel.reported_date != null && objModel.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reported_date='" + objModel.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_hazard='" + objModel.id_hazard + "'";

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
                string sSqlstmt = "update t_hazard set Active=0 where id_hazard='" + id_hazard + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteHazard: " + ex.ToString());
            }
            return false;
        }
    }

    public class HealthSafetyModelsList
    {
        public List<HealthSafetyModels> lstHazard { get; set; }
    }
}