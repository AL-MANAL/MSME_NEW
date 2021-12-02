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
    public class RiskMgmtModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

      
        [Display(Name = "Risk id")]
        public string risk_id { get; set; }
               
        [Display(Name = "Risk Trans Id")]
        public string risk_id_trans { get; set; }
              
        [Display(Name = "Risk Status")]
        public string risk_status_id { get; set; }

        [Display(Name = "Risk Description")]
        [DataType(DataType.MultilineText)]
        public string risk_desc { get; set; }
               
        [Display(Name = "Related Department")]
        public string dept { get; set; }
               
        [Display(Name = "Criteria")]
        public string reg_id { get; set; }
               
        [Display(Name = "Division")]
        public string branch_id { get; set; }
               
        [Display(Name = "Source of the Risk")]
        public string source_id { get; set; }
               
        [Display(Name = "Category")]
        public string cat_id { get; set; }
               
        [Display(Name = "Technology")]
        public string tech_id { get; set; }

        [Display(Name = "Notified to")]
        public string mit_notified_to { get; set; }

        [Display(Name = "Risk Reported By")]
        public string risk_owner { get; set; }


        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        [Display(Name = "Evaluated By")]
        public string risk_manager { get; set; }
              
        [Display(Name = "Assessment")]
        public string assessment { get; set; }
                
        [Display(Name = "Previous Assessment")]
        public string Assesment_trans { get; set; }

        //[Required]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string notes { get; set; }
               
        [Display(Name = "Submission Date")]
        public DateTime submission_date { get; set; }
               
        [Display(Name = "Close Date")]
        public DateTime close_date { get; set; }
              
        [Display(Name = "Closed By")]
        public string close_by { get; set; }

        [Required]
        [Display(Name = "Submitted By")]
        public string submitted_by { get; set; }
             
        [Display(Name = "Severity")]
        public string impact_id { get; set; }
               
        [Display(Name = "Probability")]
        public string like_id { get; set; }
             
        [Display(Name = "Risk Rating(Severity*Probability)")]
        public string RiskRating { get; set; }
                
        [Display(Name = "Color Code")]
        public string color_code { get; set; }
             
        [Display(Name = "Consequences")]
        [DataType(DataType.MultilineText)]
        public string consequences { get; set; }
             
        [Display(Name = "Risk Type")]
        public string Risk_Type { get; set; }
            
        [Display(Name = "Risk due to")]
        public string Issue { get; set; }

        [Display(Name = "Opportunity Description")]
        public string opp_desc { get; set; }

        [Display(Name = "Id")]
        public string mit_trans_id { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime evaluation_date { get; set; }

        [Display(Name = "Mitigation Measures")]
        public string measure { get; set; }

        [Display(Name = "Person Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        [Display(Name = "To Be Approved By")]
        public string approved_by { get; set; }

        [Display(Name = "Risk Reevaluation Due Date")]
        public DateTime reeval_due_date { get; set; }

        [Display(Name = "Mitigation id")]
        public string mit_id { get; set; }

        public string mit_id_trans { get; set; }

        [Display(Name = "Reference No")]
        public string risk_refno { get; set; }

        [Display(Name = "Mitigation Measures Proposed")]
        public string mitmeasure { get; set; }


        [Display(Name = "Severity")]
        public string initimpact_id { get; set; }

        [Display(Name = "Probability")]
        public string initlike_id { get; set; }

        [Display(Name = "Initial Evaluation Date")]
        public DateTime initevaluation_date { get; set; }

        [Display(Name = "Notified To")]
        public string eval_notified_to { get; set; }

        [Display(Name = "Approval Status")]
        public string apprv_status { get; set; }

        [Display(Name = "Comment")]
        public string apprv_comment { get; set; }

        [Display(Name = "Date")]
        public DateTime approved_date { get; set; }

        public string approved_by_Id { get; set; }

        public string init_apprv_status { get; set; }

        public string init_approved_by { get; set; }

        public DateTime init_reeval_due_date { get; set; }

        internal bool FunUpdateApprove(RiskMgmtModels objManagement)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                if (risk_id_trans != null && risk_id_trans != "")
                {
                    string sSqlstmt = "update risk_register_trans set apprv_comment ='" + apprv_comment + "',apprv_status ='" + apprv_status + "', approved_date='" + sApprovedDate + "' where risk_id_trans='" + risk_id_trans + "'";
                    if (objGlobaldata.ExecuteQuery(sSqlstmt))
                    {
                        return SendRiskFurtherMitigationApprvmail(risk_id,"Risk approval status");

                    }
                }
                else
                {
                    string sSqlstmt = "update risk_register set apprv_comment ='" + apprv_comment + "',apprv_status ='" + apprv_status + "', approved_date='" + sApprovedDate + "' where risk_id='" + risk_id + "'";
                    if (objGlobaldata.ExecuteQuery(sSqlstmt))
                    {
                        return SendRiskApprovemail(risk_id,"Risk approval status");

                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateObjectivesApproval: " + ex.ToString());
            }
            return false;
        }

        internal bool SendRiskFurtherMitigationApprvmail(string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select tt.risk_id_trans,t.risk_id,t.risk_refno,t.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,tt.impact_id,tt.like_id,t.Issue,t.Risk_Type,tt.eval_notified_to,tt.evaluation_date,tt.mit_notified_to,"
                          + "(CASE WHEN tt.apprv_status='0' THEN 'Pending for Approval' WHEN tt.apprv_status='1' THEN 'Rejected' WHEN tt.apprv_status='2' THEN 'Approved' END) as apprv_status,tt.apprv_comment"
                       + " from risk_register t left join  risk_register_trans tt on t.risk_id = tt.risk_id  where t.risk_id = '" + risk_id + "' order by risk_id_trans desc limit 1";



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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }
                    string evaluation_date = "", reeval_due_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>To Be Approved By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>"
                    +"<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["apprv_status"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["apprv_comment"].ToString()) + "</td></tr>";


                    sSqlstmt = "select measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans = '" + dsData.Tables[0].Rows[0]["risk_id_trans"].ToString() + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Mitigation Measure</th>"
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
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measure"].ToString()) + "</td>"
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
                    sContent = sContent.Replace("{Title}", "Risk Further Mitigation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskFurtherMitigationApprvmail: " + ex.ToString());
            }
            return false;
        }


        internal bool SendRiskApprovemail(string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select risk_refno,submitted_by,impact_id,like_id,risk_manager,eval_notified_to,evaluation_date,approved_by,reeval_due_date,mit_notified_to,"
                  + "(CASE WHEN apprv_status='0' THEN 'Pending for Approval' WHEN apprv_status='1' THEN 'Rejected' WHEN apprv_status='2' THEN 'Approved' END) as apprv_status,apprv_comment from risk_register"
                + " where risk_id='" + risk_id + "'";

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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }
                    string evaluation_date = "", reeval_due_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>To Be Approved By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>"

                      + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["apprv_status"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["apprv_comment"].ToString()) + "</td></tr>";


                    sSqlstmt = "select measure,pers_resp,target_date from risk_mitigations where risk_id = '" + risk_id + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Mitigation Measure</th>"
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
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measure"].ToString()) + "</td>"
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
                    sContent = sContent.Replace("{Title}", "Risk Mitigation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskApprovemail: " + ex.ToString());
            }
            return false;
        }



        public DataSet GetMatrixColordetails()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Get_colourcode", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                //cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetMatrixColordetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        internal bool FunDeleteRiskMgmtDoc(string srisk_id)
        {
            try
            {
                string sSqlstmt = "update risk_register set Active=0 where risk_id='" + srisk_id + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteRiskMgmtDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddRisk(RiskMgmtModels objRiskMgmtModels)
        {
            try
            {
                string sSubmission_date = DateTime.Now.ToString("yyyy/MM/dd");
               
                string sSqlstmt = "insert into risk_register (risk_status_id, risk_desc, dept, reg_id, branch_id, source_id,risk_owner, risk_manager,"
                    + "assessment, notes, submission_date, submitted_by, impact_id, like_id, consequences,opp_desc,Issue,Location,notified_to,Risk_Type)"
                + " values('" + objRiskMgmtModels.risk_status_id + "','" + objRiskMgmtModels.risk_desc + "','" + objRiskMgmtModels.dept
                + "','" + objRiskMgmtModels.reg_id + "','" + objRiskMgmtModels.branch_id + "','" + objRiskMgmtModels.source_id
                + "','" + objRiskMgmtModels.risk_owner + "','" + objRiskMgmtModels.risk_manager
                + "','" + objRiskMgmtModels.assessment + "','" + objRiskMgmtModels.notes + "','" + sSubmission_date + "','" + objRiskMgmtModels.submitted_by
                + "','" + objRiskMgmtModels.impact_id + "','" + objRiskMgmtModels.like_id + "','" + objRiskMgmtModels.consequences + "','" + objRiskMgmtModels.opp_desc 
                + "','" + objRiskMgmtModels.Issue + "','" + objRiskMgmtModels.Location + "','" + objRiskMgmtModels.notified_to + "','" + objRiskMgmtModels.Risk_Type + "')";

                int risk_id = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out risk_id))
                {
                    DataSet dsData = objGlobaldata.GetReportNo("RISK", "", objGlobaldata.GetBranchShortNameByID(branch_id));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        risk_refno = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update risk_register set risk_refno='" + risk_refno + "' where risk_id='" + risk_id + "'";
                    objGlobaldata.ExecuteQuery(sql1);
                    return SendRiskmail(risk_id, "Risk Reporting");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddRisk: " + ex.ToString());
            }
            return false;
        }

        internal bool SendRiskmail(int risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select risk_refno,risk_desc, dept, branch_id, source_id,risk_owner,submission_date, submitted_by,"
                + "consequences,Issue,Location,notified_to,Risk_Type from risk_register where risk_id='" + risk_id + "'";

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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["risk_owner"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     +"<tr><td colspan=3><b>Division:<b></td> <td colspan=3>"
                      + objGlobaldata.GetMultiCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Related Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsData.Tables[0].Rows[0]["dept"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsData.Tables[0].Rows[0]["Location"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Source of the Risk:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["source_id"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk due to:<b></td> <td colspan=3>" + objRiskMgmtModels.GetIssueNameById(dsData.Tables[0].Rows[0]["Issue"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk Type:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["Risk_Type"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk Description:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_desc"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Consequences:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["consequences"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Risk Reported By:<b></td> <td colspan=3>" +objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_owner"].ToString()) + "</td></tr>";


                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Risk Reporting");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRisk(RiskMgmtModels objRiskMgmtModels)
        {
            try
            {
                string user = "";
               
               user = objGlobaldata.GetCurrentUserSession().empid;
                
                string sSqlstmt = "update risk_register set  risk_desc='" + objRiskMgmtModels.risk_desc + "', "
                    + "dept='" + objRiskMgmtModels.dept + "', branch_id='" + objRiskMgmtModels.branch_id + "', Issue='" + objRiskMgmtModels.Issue + "', notified_to='" + objRiskMgmtModels.notified_to + "', source_id='" + objRiskMgmtModels.source_id
                   + "', risk_owner='" + objRiskMgmtModels.risk_owner + "', Risk_Type='" + objRiskMgmtModels.Risk_Type
                   + "', consequences='" + objRiskMgmtModels.consequences + "', Location='" + objRiskMgmtModels.Location + "'";
                 

                //if (GetRiskStatusNameById(objRiskMgmtModels.risk_status_id).ToLower() == "closed")
                //{
                //    sSqlstmt = sSqlstmt + ", close_date='" + DateTime.Now.ToString("yyyy/MM/dd") + "', close_by='" + user + "'";
                //}

                sSqlstmt = sSqlstmt + " where risk_id='" + objRiskMgmtModels.risk_id + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRisk: " + ex.ToString());
            }
            return false;
        }

        //internal string GetRegulationNameById(string sReg_id)
        //{
        //    try
        //    {
        //        DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //             + "and header_desc='Risk-Regulation' and (item_id='" + sReg_id + "' or item_desc='" + sReg_id + "')");
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GetRegulationNameById: " + ex.ToString());
        //    }
        //    return "";
        //}

        //public MultiSelectList GetMultiRegulationList()
        //{
        //    DropdownRiskList lstImpact = new DropdownRiskList();
        //    lstImpact.lstMitigationimpact = new List<DropdownRiskItems>();
        //    try
        //    {
        //        //string sSqlstmt = "select impact_id, impact_name from impact";
        //        string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
        //            + "and header_desc='Risk-Regulation' order by item_desc asc";
        //        DataSet dsEmp = objGlobaldata.Getdetails(sSqlstmt);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                DropdownRiskItems reg = new DropdownRiskItems()
        //                {
        //                    Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
        //                    Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
        //                };

        //                lstImpact.lstMitigationimpact.Add(reg);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in GetMultiRegulationList: " + ex.ToString());
        //    }
        //    return new MultiSelectList(lstImpact.lstMitigationimpact, "Id", "Name");
        //}

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
                   
        internal string GetRiskNameById(string sRisk_id)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("SELECT risk_id, risk_desc FROM risk_register where risk_id='" + sRisk_id + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["risk_desc"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetRiskNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiRiskList()
        {
            RiskDetailsList lstRisk = new RiskDetailsList();
            lstRisk.lstRiskDetails = new List<RiskDetails>();
            try
            {
                string sSqlstmt = "SELECT risk_id, risk_desc FROM risk_register";

                DataSet dsEmp = objGlobaldata.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        RiskDetails reg = new RiskDetails()
                        {
                            risk_id = dsEmp.Tables[0].Rows[i]["risk_id"].ToString(),
                            risk_desc = dsEmp.Tables[0].Rows[i]["risk_desc"].ToString()
                        };

                        lstRisk.lstRiskDetails.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetMultiRiskList: " + ex.ToString());
            }

            return new MultiSelectList(lstRisk.lstRiskDetails, "risk_id", "risk_desc");
        }
                   
       

      
        public MultiSelectList GetIsssuesNo(string id_issue = "")
        {
            issuesList lst = new issuesList();
            lst.lstIssue = new List<issues>();
            try
            {

                string sSqlstmt = "";
                if (id_issue != "")
                {
                    sSqlstmt = "select id_issue,Issue_refno from t_issues where id_issue='" + id_issue + "'";
                }
                else
                {
                    sSqlstmt = "select id_issue,Issue_refno from t_issues where Active=1";
                }
               
                DataSet dsIssue = objGlobaldata.Getdetails(sSqlstmt);
                if (dsIssue.Tables.Count > 0 && dsIssue.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsIssue.Tables[0].Rows.Count; i++)
                    {
                        issues reg = new issues()
                        {
                            id_issue = dsIssue.Tables[0].Rows[i]["id_issue"].ToString(),
                            Issue = dsIssue.Tables[0].Rows[i]["Issue_refno"].ToString()
                        };

                        lst.lstIssue.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetIsssuesNo: " + ex.ToString());
            }

            return new MultiSelectList(lst.lstIssue, "id_issue", "Issue");
        }

        internal string GetIssueNameById(string IssueId)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select Issue from t_issues where id_issue='" + IssueId + "' and Active=1");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Issue"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetIssueNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunUpdateRiskEvaluation(RiskMgmtModels objModel)
        {
            try
            {
                string sSqlstmt = "update risk_register set impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',risk_manager='" + objModel.risk_manager + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where risk_id='" + objModel.risk_id + "'";

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    return SendInitialRiskEvalmail(risk_id, "Initial Risk Evaluation");
                }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskEvaluation: " + ex.ToString());
            }

            return false;
        }

        internal bool SendInitialRiskEvalmail(string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select risk_refno,submitted_by,impact_id,like_id,risk_manager,eval_notified_to,evaluation_date from risk_register"
                + " where risk_id='" + risk_id + "'";

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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;
                       
                    }
                    string evaluation_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "</td></tr>";

                    
                  
                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Initial Risk Evaluation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendInitialRiskEvalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskMitigation(RiskMgmtModels objModel, RiskMgmtModelsList objRiskList)
        {
            try
            {
                string approved_date = DateTime.Now.ToString("yyyy/MM/dd");
                string sSqlstmt = "update risk_register set approved_by='" + objModel.approved_by + "',mit_notified_to='" + objModel.mit_notified_to + "',apprv_status='0'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where risk_id='" + objModel.risk_id + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstRiskMgmtModels.Count) > 0)
                    {
                        objRiskList.lstRiskMgmtModels[0].risk_id = risk_id.ToString();
                        FunAddMitigationList(objRiskList);
                    }
                    else
                    {
                        FunUpdateMitigation(risk_id);
                    }
                    return SendRiskMitigationmail(risk_id, "Risk mitigation for approval");
                }
                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskMitigation: " + ex.ToString());
            }
            return false;
        }
        internal bool SendRiskMitigationmail(string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select risk_refno,submitted_by,impact_id,like_id,risk_manager,eval_notified_to,evaluation_date,approved_by,reeval_due_date,mit_notified_to from risk_register"
                + " where risk_id='" + risk_id + "'";

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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }
                    string evaluation_date = "", reeval_due_date="";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>To Be Approved By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select measure,pers_resp,target_date from risk_mitigations where risk_id = '" + risk_id + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Mitigation Measure</th>"
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
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measure"].ToString()) + "</td>"
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
                    sContent = sContent.Replace("{Title}", "Risk Mitigation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskMitigationmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateMitigation(string risk_id)
        {
            try
            {
                string sSqlstmt = "delete from risk_mitigations where risk_id='" + risk_id + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateMitigation: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddMitigationList(RiskMgmtModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from risk_mitigations where risk_id='" + objRiskList.lstRiskMgmtModels[0].risk_id + "'; ";

                for (int i = 0; i < objRiskList.lstRiskMgmtModels.Count; i++)
                {
                    //sSqlstmt = sSqlstmt + "insert into risk_mitigations(risk_id,measure,pers_resp";

                    //String sFieldValue = "", sFields = "";
                    //if (objRiskList.lstRiskMgmtModels[i].target_date != null && objRiskList.lstRiskMgmtModels[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    //{
                    //    sFields = sFields + ", target_date";
                    //    sFieldValue = sFieldValue + ", '" + objRiskList.lstRiskMgmtModels[i].target_date.ToString("yyyy/MM/dd") + "'";
                    //}                
                    //sSqlstmt = sSqlstmt + sFields;
                    //sSqlstmt = sSqlstmt + ") values('" + objRiskList.lstRiskMgmtModels[0].risk_id + "', '" + objRiskList.lstRiskMgmtModels[i].measure + "', '" + objRiskList.lstRiskMgmtModels[i].pers_resp + "'";
                    //sSqlstmt = sSqlstmt + sFieldValue + ");";

                    string sFieldValue = "", sFields = "",sValue="",sStatement="";
                    string smit_id = "null";
                    sSqlstmt = sSqlstmt + " insert into risk_mitigations (mit_id,risk_id,measure,pers_resp";
                    if (objRiskList.lstRiskMgmtModels[i].mit_id != null)
                    {
                        smit_id = objRiskList.lstRiskMgmtModels[i].mit_id;
                    }
                    if (objRiskList.lstRiskMgmtModels[i].target_date != null && objRiskList.lstRiskMgmtModels[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", target_date= values(target_date)";
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objRiskList.lstRiskMgmtModels[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id + "," + objRiskList.lstRiskMgmtModels[0].risk_id
                    + ",'" + objRiskList.lstRiskMgmtModels[i].measure + "','" + objRiskList.lstRiskMgmtModels[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " mit_id= values(mit_id), risk_id= values(risk_id), measure = values(measure), pers_resp= values(pers_resp)";
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
        internal bool FunUpdateRiskReEvaluation(RiskMgmtModels objModel)
        {
            try
            {
                DateTime today_date = DateTime.Now;
                string sSqlstmt = "select risk_id,reeval_due_date from risk_register_trans where risk_id_trans='"
                     + risk_id_trans + "' order by risk_id_trans desc limit 1";
                DataSet dsRiskModels = objGlobaldata.Getdetails(sSqlstmt);
                if (dsRiskModels.Tables.Count > 0 && dsRiskModels.Tables[0].Rows.Count > 0)
                {
                    DateTime dtValue;

                    if (DateTime.TryParse(dsRiskModels.Tables[0].Rows[0]["reeval_due_date"].ToString(), out dtValue))
                    {
                        if (today_date >= dtValue)
                        {
                            sSqlstmt = "insert into risk_register_trans(risk_id,impact_id,like_id,risk_manager,risk_desc,risk_refno,eval_notified_to";
                            string sFields = "", sFieldValue = "";

                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sFields = sFields + ", evaluation_date";
                                sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }
                            sSqlstmt = sSqlstmt + sFields;
                            sSqlstmt = sSqlstmt + ") values('" + objModel.risk_id + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.risk_manager + "','" + objModel.risk_desc + "','" + objModel.risk_refno + "','" + objModel.eval_notified_to + "'";

                            sSqlstmt = sSqlstmt + sFieldValue + ")";
                            int srisk_id_trans = 0;

                            if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out srisk_id_trans))
                            {
                                string sSqlstmt1 = "select risk_id_trans,mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans='" + risk_id_trans + "'";
                                DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                                if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                                {
                                    sSqlstmt = "";
                                    for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                                    {
                                        sSqlstmt = sSqlstmt + "insert into risk_mitigations_trans(risk_id_trans,mit_id,risk_id,measure,pers_resp";

                                        string sFieldValues = "", sField = "";
                                        if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                        {
                                            sField = sField + ", target_date";
                                            sFieldValues = sFieldValues + ", '" + Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                        }
                                        sSqlstmt = sSqlstmt + sField;
                                        sSqlstmt = sSqlstmt + ") values('" + srisk_id_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["risk_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["measure"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                        sSqlstmt = sSqlstmt + sFieldValues + ");";
                                    }
                                    if(objGlobaldata.ExecuteQuery(sSqlstmt))
                                    {
                                        return SendRiskReEvalmail(objModel,risk_id, "Risk Re-Evaluation");
                                    }
                                   
                                }
                            }
                        }
                        else
                        {
                            sSqlstmt = "update risk_register_trans set impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',risk_manager='" + objModel.risk_manager + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                            if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                            {
                                sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                            }

                            sSqlstmt = sSqlstmt + " where risk_id_trans='" + objModel.risk_id_trans + "'";
                            if (objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendRiskReEvalmail(objModel,risk_id, "Risk Re-Evaluation");
                            }
                        }

                    }
                    else
                    {
                        sSqlstmt = "update risk_register_trans set impact_id='" + objModel.impact_id + "',like_id='" + objModel.like_id + "',risk_manager='" + objModel.risk_manager + "',eval_notified_to='" + objModel.eval_notified_to + "'";
                        if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sSqlstmt = sSqlstmt + ",evaluation_date='" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                        }

                        sSqlstmt = sSqlstmt + " where risk_id_trans='" + objModel.risk_id_trans + "'";
                        if (objGlobaldata.ExecuteQuery(sSqlstmt))
                        {
                            return SendRiskReEvalmail(objModel,risk_id, "Risk Re-Evaluation");
                        }
                    }
                   
                }
                else
                {
                    sSqlstmt = "insert into risk_register_trans(risk_id,impact_id,like_id,risk_manager,risk_desc,risk_refno,eval_notified_to";
                    string sFields = "", sFieldValue = "";

                    if (objModel.evaluation_date != null && objModel.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", evaluation_date";
                        sFieldValue = sFieldValue + ", '" + objModel.evaluation_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModel.risk_id + "','" + objModel.impact_id + "','" + objModel.like_id + "','" + objModel.risk_manager + "','" + objModel.risk_desc + "','" + objModel.risk_refno + "','" + objModel.eval_notified_to + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    int risk_id_trans = 0;

                    if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out risk_id_trans))
                    {
                        string sSqlstmt1 = "select mit_id,risk_id,measure,pers_resp,target_date from risk_mitigations where risk_id='" + risk_id + "'";
                        DataSet dsMitList = objGlobaldata.Getdetails(sSqlstmt1);
                        if (dsMitList.Tables.Count > 0 && dsMitList.Tables[0].Rows.Count > 0)
                        {
                            sSqlstmt = "";
                            for (int i = 0; i < dsMitList.Tables[0].Rows.Count; i++)
                            {
                                sSqlstmt = sSqlstmt + "insert into risk_mitigations_trans(risk_id_trans,mit_id,risk_id,measure,pers_resp";

                                string sFieldValues = "", sField = "";
                                if (dsMitList.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001 00:00:00"))
                                {
                                    sField = sField + ", target_date";
                                    sFieldValues = sFieldValues + ", '" +Convert.ToDateTime(dsMitList.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy/MM/dd") + "'";
                                }
                                sSqlstmt = sSqlstmt + sField;
                                sSqlstmt = sSqlstmt + ") values('" + risk_id_trans + "','" + dsMitList.Tables[0].Rows[i]["mit_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["risk_id"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["measure"].ToString() + "', '" + dsMitList.Tables[0].Rows[i]["pers_resp"].ToString() + "'";
                                sSqlstmt = sSqlstmt + sFieldValues + ");";
                            }
                            if (objGlobaldata.ExecuteQuery(sSqlstmt))
                            {
                                return SendRiskReEvalmail(objModel,risk_id, "Risk Re-Evaluation");
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

        internal bool SendRiskReEvalmail(RiskMgmtModels objModel,string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select risk_refno,submitted_by,impact_id,like_id,risk_manager,eval_notified_to,evaluation_date from risk_register"
                + " where risk_id='" + risk_id + "'";

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
                    sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(objModel.eval_notified_to);
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(objModel.risk_manager) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if ((objModel.impact_id) != "" && (objModel.like_id) != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(objModel.impact_id,
                            objModel.like_id);
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }
                    string evaluation_date = "";
                    if (objModel.evaluation_date != null && Convert.ToDateTime(objModel.evaluation_date) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(objModel.evaluation_date).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(objModel.impact_id) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(objModel.like_id) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(objModel.risk_manager) + "</td></tr>";



                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Risk Re-Evaluation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskReEvalmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateFurtherRiskMitigation(RiskMgmtModels objModel, RiskMgmtModelsList objRiskList)
        {
            try
            {
                string approved_date = DateTime.Now.ToString("yyyy/MM/dd");
                string sSqlstmt = "update risk_register_trans set approved_by='" + objModel.approved_by + "',mit_notified_to='" + objModel.mit_notified_to + "',apprv_status='0'";
                if (objModel.reeval_due_date != null && objModel.reeval_due_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",reeval_due_date='" + objModel.reeval_due_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where risk_id_trans='" + objModel.risk_id_trans + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objRiskList.lstRiskMgmtModels.Count) > 0)
                    {
                        objRiskList.lstRiskMgmtModels[0].risk_id_trans = risk_id_trans.ToString();
                        objRiskList.lstRiskMgmtModels[0].risk_id = risk_id.ToString();
                        FunAddFurtherMitigationList(objRiskList);
                    }
                    else
                    {
                        FunUpdateMitigation(risk_id);
                    }
                    return SendRiskFurtherMitigationmail(risk_id, "Risk further mitigation for approval");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskMitigation: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddFurtherMitigationList(RiskMgmtModelsList objRiskList)
        {
            try
            {
                string sSqlstmt = "delete from risk_mitigations_trans where risk_id_trans='" + objRiskList.lstRiskMgmtModels[0].risk_id_trans + "'; ";

                for (int i = 0; i < objRiskList.lstRiskMgmtModels.Count; i++)
                {
                    //sSqlstmt = sSqlstmt + "insert into risk_mitigations(risk_id,measure,pers_resp";

                    //String sFieldValue = "", sFields = "";
                    //if (objRiskList.lstRiskMgmtModels[i].target_date != null && objRiskList.lstRiskMgmtModels[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    //{
                    //    sFields = sFields + ", target_date";
                    //    sFieldValue = sFieldValue + ", '" + objRiskList.lstRiskMgmtModels[i].target_date.ToString("yyyy/MM/dd") + "'";
                    //}                
                    //sSqlstmt = sSqlstmt + sFields;
                    //sSqlstmt = sSqlstmt + ") values('" + objRiskList.lstRiskMgmtModels[0].risk_id + "', '" + objRiskList.lstRiskMgmtModels[i].measure + "', '" + objRiskList.lstRiskMgmtModels[i].pers_resp + "'";
                    //sSqlstmt = sSqlstmt + sFieldValue + ");";

                    string sFieldValue = "", sFields = "", sValue = "", sStatement = "";
                    string smit_id_trans = "null";
                    sSqlstmt = sSqlstmt + " insert into risk_mitigations_trans (mit_id_trans,risk_id_trans,mit_id,risk_id,measure,pers_resp";
                    if (objRiskList.lstRiskMgmtModels[i].mit_id_trans != null)
                    {
                        smit_id_trans = objRiskList.lstRiskMgmtModels[i].mit_id_trans;
                    }
                    if (objRiskList.lstRiskMgmtModels[i].target_date != null && objRiskList.lstRiskMgmtModels[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", target_date= values(target_date)";
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objRiskList.lstRiskMgmtModels[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ")  values(" + smit_id_trans + ",'" + objRiskList.lstRiskMgmtModels[0].risk_id_trans + "','" + objRiskList.lstRiskMgmtModels[i].mit_id + "'," + objRiskList.lstRiskMgmtModels[0].risk_id
                    + ",'" + objRiskList.lstRiskMgmtModels[i].measure + "','" + objRiskList.lstRiskMgmtModels[i].pers_resp + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                     + " mit_id_trans= values(mit_id_trans),risk_id_trans= values(risk_id_trans),mit_id= values(mit_id), risk_id= values(risk_id), measure = values(measure), pers_resp= values(pers_resp)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddFurtherMitigationList: " + ex.ToString());
            }
            return false;
        }

        internal bool SendRiskFurtherMitigationmail(string risk_id, string sMessage = "")
        {
            RiskMgmtModels objRiskMgmtModels = new RiskMgmtModels();
            try
            {
                string sType = "email";

                string sSqlstmt = "select tt.risk_id_trans,t.risk_id,t.risk_refno,t.risk_desc,t.dept,t.branch_id,t.source_id,t.risk_owner,t.risk_manager,t.submission_date,t.submitted_by,t.consequences,t.Location,tt.evaluation_date,tt.approved_by,tt.approved_date,tt.reeval_due_date,tt.impact_id,tt.like_id,t.Issue,t.Risk_Type,tt.eval_notified_to,tt.evaluation_date,tt.mit_notified_to,"
                          + "(CASE WHEN tt.apprv_status='0' THEN 'Pending for Approval' WHEN tt.apprv_status='1' THEN 'Rejected' WHEN tt.apprv_status='2' THEN 'Approved' END) as apprv_status,tt.apprv_comment"
                       + " from risk_register t left join  risk_register_trans tt on t.risk_id = tt.risk_id  where t.risk_id = '" + risk_id + "' order by risk_id_trans desc limit 1";

               

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
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["submitted_by"].ToString());

                    Dictionary<string, string> dicRatings = new Dictionary<string, string>();
                    if (dsData.Tables[0].Rows[0]["impact_id"].ToString() != "" && dsData.Tables[0].Rows[0]["like_id"].ToString() != "")
                    {
                        dicRatings = objRiskMgmtModels.GetRiskRatings(dsData.Tables[0].Rows[0]["impact_id"].ToString(),
                            dsData.Tables[0].Rows[0]["like_id"].ToString());
                    }
                    if (dicRatings != null && dicRatings.Count > 0)
                    {
                        objRiskMgmtModels.RiskRating = dicRatings.FirstOrDefault().Key;

                    }
                    string evaluation_date = "", reeval_due_date = "";
                    if (dsData.Tables[0].Rows[0]["evaluation_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        evaluation_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["evaluation_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    if (dsData.Tables[0].Rows[0]["reeval_due_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        reeval_due_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["reeval_due_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>Ref No:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["risk_refno"].ToString()) + "</td></tr>"
                     + "<tr><td colspan=3><b>Severity:<b></td> <td colspan=3>"
                      + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["impact_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Probability:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsData.Tables[0].Rows[0]["like_id"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Risk Rating:<b></td> <td colspan=3>" + objRiskMgmtModels.RiskRating + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Date:<b></td> <td colspan=3>" + evaluation_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluated By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["risk_manager"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>To Be Approved By:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Risk Reevaluation Due Date:<b></td> <td colspan=3>" + reeval_due_date + "</td></tr>";


                    sSqlstmt = "select measure,pers_resp,target_date from risk_mitigations_trans where risk_id_trans = '" + dsData.Tables[0].Rows[0]["risk_id_trans"].ToString() + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Mitigation</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Mitigation Measure</th>"
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
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measure"].ToString()) + "</td>"
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
                    sContent = sContent.Replace("{Title}", "Risk Further Mitigation");
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
                objGlobaldata.AddFunctionalLog("Exception in SendRiskFurtherMitigationmail: " + ex.ToString());
            }
            return false;
        }

    }

    public class RiskMitigationModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //[Required]
        [Display(Name = "Mitigation id")]
        public string mit_id { get; set; }

        [Display(Name = "Risk id")]
        public string risk_id { get; set; }

        [Required]
        [Display(Name = "Current Solution")]
        public string current_solution { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime submission_date { get; set; }

        [Display(Name = "Last Update")]
        public string last_update { get; set; }

        [Required]
        [Display(Name = "Risk Evaluation Status")]
        public string eval_id { get; set; }

        [Required]
        [Display(Name = "Mitigation Effort")]
        public string effort_id { get; set; }

        [Required]
        [Display(Name = "Mitigation Owner")]
        public string mitigation_owner { get; set; }

        [Required]
        [Display(Name = "Submitted By")]
        public string submitted_by { get; set; }

        [Required]
        [Display(Name = "Mitigation Status")]
        public string MitigationStatus { get; set; }

        [Required]
        [Display(Name = "Document")]
        public string DocUpload { get; set; }

        [Required]
        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }

        [Display(Name = "Id")]
        public string mit_trans_id { get; set; }


        internal bool FunAddRiskMitigations(RiskMitigationModels objRiskMitigation)
        {
            try
            {
                string sSubmission_date = DateTime.Now.ToString("yyyy/MM/dd");
                string dtTarget_Date = objRiskMitigation.TargetDate.ToString("yyyy/MM/dd HH:mm");
                //string sSqlstmt = "insert into mitigations (risk_id, submission_date, eval_id, effort_id, mitigation_owner, current_solution, submitted_by)"
                //+ " values('" + objRiskMitigation.risk_id + "','" + sSubmission_date + "','" + objRiskMitigation.eval_id + "','" + objRiskMitigation.effort_id 
                //+ "','" + objRiskMitigation.mitigation_owner + "','" + objRiskMitigation.current_solution + "','" + objRiskMitigation.submitted_by +  "')";


                string sSqlstmt = "INSERT INTO mitigations (risk_id, submission_date, eval_id, effort_id, mitigation_owner, current_solution, submitted_by, MitigationStatus,TargetDate";

                if (objRiskMitigation.DocUpload != null)
                {
                    sSqlstmt = sSqlstmt + ",DocUpload)"
                          + " VALUES('" + objRiskMitigation.risk_id + "','" + sSubmission_date + "','" + objRiskMitigation.eval_id + "','" + objRiskMitigation.effort_id
                           + "','" + objRiskMitigation.mitigation_owner + "','" + objRiskMitigation.current_solution + "','" + objRiskMitigation.submitted_by
                           + "','" + objRiskMitigation.MitigationStatus + "','" + dtTarget_Date + "','" + objRiskMitigation.DocUpload + "')"
                           + " ON DUPLICATE KEY UPDATE  eval_id= values(eval_id),  last_update = current_date(), effort_id=values(effort_id),  "
                           + "mitigation_owner= values(mitigation_owner),current_solution=values(current_solution),MitigationStatus=values(MitigationStatus),"
                           + "TargetDate= values(TargetDate),DocUpload=values(DocUpload)";
                }
                else
                {
                    sSqlstmt = sSqlstmt + ")"
                          + " VALUES('" + objRiskMitigation.risk_id + "','" + sSubmission_date + "','" + objRiskMitigation.eval_id + "','" + objRiskMitigation.effort_id
                          + "','" + objRiskMitigation.mitigation_owner + "','" + objRiskMitigation.current_solution + "','" + objRiskMitigation.submitted_by
                          + "','" + objRiskMitigation.MitigationStatus + "','" + dtTarget_Date + "')"
                          + " ON DUPLICATE KEY UPDATE  eval_id= values(eval_id),  last_update = current_date(), effort_id=values(effort_id),  "
                          + "mitigation_owner= values(mitigation_owner),current_solution=values(current_solution),MitigationStatus=values(MitigationStatus),"
                          + "TargetDate= values(TargetDate)";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddRiskMitigations: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskMitigations(RiskMitigationModels objRiskMitigation)
        {
            try
            {
                string sSqlstmt = "update mitigations set risk_id ='" + objRiskMitigation.risk_id + "', eval_id='" + objRiskMitigation.eval_id + "', "
                    + "effort_id='" + objRiskMitigation.effort_id + "', mitigation_owner='" + objRiskMitigation.mitigation_owner + "', current_solution='" + objRiskMitigation.current_solution
                   + "', submitted_by='" + objRiskMitigation.submitted_by + "', MitigationStatus='" + objRiskMitigation.MitigationStatus
                   + "' where mit_id='" + objRiskMitigation.mit_id + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskMitigations: " + ex.ToString());
            }
            return false;
        }
                
    }


    public class RiskReviewModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //[Required]
        [Display(Name = "Review id")]
        public string review_id { get; set; }

        [Display(Name = "Risk id")]
        public string risk_id { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime submission_date { get; set; }

        [Display(Name = "Reviewer")]
        public string reviewer { get; set; }

        [Required]
        [Display(Name = "Mitigation Owner")]
        public string mitigation_owner { get; set; }

        [Display(Name = "What Monitoring")]
        public string what_monit { get; set; }

        [Display(Name = "Where Monitoring")]
        public string where_monit { get; set; }

        [Display(Name = "When Monitoring")]
        public string when_monit { get; set; }

        [Display(Name = "Who Monitoring")]
        public string who_monit { get; set; }

        [Display(Name = "How Monitoring")]
        public string how_monit { get; set; }



        internal bool FunAddRiskReview(RiskReviewModels objRiskReviewModels)
        {
            try
            {
                string sSubmission_date = DateTime.Now.ToString("yyyy/MM/dd");

                string sSqlstmt = "INSERT INTO mgmt_reviews (risk_id, submission_date, reviewer,what_monit,where_monit,when_monit,who_monit,how_monit)"
                        + " VALUES('" + objRiskReviewModels.risk_id + "','" + sSubmission_date + "','" + objRiskReviewModels.reviewer
                        + "','" + objRiskReviewModels.what_monit + "','" + objRiskReviewModels.where_monit + "','" + objRiskReviewModels.when_monit + "'"
                +",'" + objRiskReviewModels.who_monit + "','" + objRiskReviewModels.how_monit + "')";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddRiskReview: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskReview(RiskReviewModels objRiskReviewModels)
        {
            try
            {
                string sSqlstmt = "update mgmt_reviews set reviewer='" + objRiskReviewModels.reviewer + "', what_monit='" + objRiskReviewModels.what_monit + "'"
                + ", where_monit='" + objRiskReviewModels.where_monit + "', when_monit='" + objRiskReviewModels.when_monit + "', who_monit='" + objRiskReviewModels.who_monit + "'"
                + ", how_monit='" + objRiskReviewModels.how_monit + "' where review_id='" + objRiskReviewModels.review_id + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRiskReview: " + ex.ToString());
            }
            return false;
        }

      
    }

    public class RiskReviewModelsList
    {
        public List<RiskReviewModels> lstRiskReviewModels { get; set; }
    }

    public class ReviewLevelList
    {
        public List<ReviewLevel> lstReviewLevel { get; set; }
    }

    public class ReviewLevel
    {
        public string level_id { get; set; }
        public string level_name { get; set; }
    }

    public class LikelihoodList
    {
        public List<Likelihood> lstLikelihood { get; set; }
    }

    public class Likelihood
    {
        public string like_id { get; set; }
        public string like_name { get; set; }
    }

    public class MitigationimpactList
    {
        public List<Mitigationimpact> lstMitigationimpact { get; set; }
    }

    public class Mitigationimpact
    {
        public string impact_id { get; set; }
        public string impact_name { get; set; }
    }

    public class MitigationEffortList
    {
        public List<MitigationEffort> lstMitigationEffort { get; set; }
    }

    public class MitigationEffort
    {
        public string effort_id { get; set; }
        public string effort_name { get; set; }
    }

    public class PlanningStrategyList
    {
        public List<PlanningStrategy> lstPlanningStrategy { get; set; }
    }

    public class PlanningStrategy
    {
        public string eval_id { get; set; }
        public string plan_name { get; set; }
    }

    public class RiskDetailsList
    {
        public List<RiskDetails> lstRiskDetails { get; set; }
    }

    public class RiskDetails
    {
        public string risk_id { get; set; }
        public string risk_desc { get; set; }
    }

    public class RiskMgmtModelsList
    {
        public List<RiskMgmtModels> lstRiskMgmtModels { get; set; }
    }

    public class RiskStatus
    {
        public string status_id { get; set; }
        public string status_name { get; set; }
    }

    public class RiskStatusList
    {
        public List<RiskStatus> lstRiskStatus { get; set; }
    }

    public class Regulation
    {
        public string reg_id { get; set; }
        public string reg_name { get; set; }
    }

    public class RegulationList
    {
        public List<Regulation> lstRegulation { get; set; }
    }

    public class RiskSource
    {
        public string source_id { get; set; }
        public string source_name { get; set; }
    }

    public class RiskSourceList
    {
        public List<RiskSource> lstRiskSource { get; set; }
    }


    public class RiskCategory
    {
        public string cat_id { get; set; }
        public string cat_name { get; set; }
    }

    public class RiskCategoryList
    {
        public List<RiskCategory> lstRiskCategory { get; set; }
    }


    public class Risktechnology
    {
        public string tech_id { get; set; }
        public string tech_name { get; set; }
    }

    public class RisktechnologyList
    {
        public List<Risktechnology> lstRisktechnology { get; set; }
    }

    public class DropdownRiskItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownRiskList
    {
        public List<DropdownRiskItems> lstMitigationimpact { get; set; }
    }
    public class issues
    {
        public string id_issue { get; set; }
        public string Issue { get; set; }
    
    }
    public class issuesList
    {
        public List<issues> lstIssue { get; set; }
    }

    public class RiskMitigationModelsList
    {
        public List<RiskMitigationModels> MitigationList { get; set; }
    }

}
