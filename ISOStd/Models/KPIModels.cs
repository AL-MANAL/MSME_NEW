using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;
using ExpressiveAnnotations.Attributes;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class KPIModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string KPI_Id { get; set; }

        [Display(Name = "Id")]
        public string id_measurable { get; set; }

        [Display(Name = "KPI Established On")]
        public DateTime established_date { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string group_name { get; set; }

        //[Display(Name = "Team")]
        //public string team { get; set; }

        [Display(Name = "Process/Performance Indicator")]
        public string process_indicator { get; set; }

        [Display(Name = "KPI Level")]
        public string kpi_level { get; set; }

        [Display(Name = "Process to be monitored")]
        public string process_monitor { get; set; }

        [Display(Name = "Measurable Indicator")]
        public string measurable_indicator { get; set; }

        [Display(Name = "Expected value to be achieved")]
        public string expected_value { get; set; }

        [Display(Name = "Unit")]
        public string expected_value_unit { get; set; }

        [Display(Name = "KPI Type")]
        public string kpi_type { get; set; }

        [Display(Name = "Monitoring Period From Date")]
        public DateTime monitoring_frm_date { get; set; }

        [Display(Name = "To Date")]
        public DateTime monitoring_to_date { get; set; }

        [Display(Name = "Monitoring Mechanism")]
        public string monitoring_mechanism { get; set; }

        [Display(Name = "Frequency of Evaluation")]
        public string frequency_eval { get; set; }

        [Display(Name = "Risk, if not achieved")]
        public string risk { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string pers_resp { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "To be Approved by")]
        public string approved_by { get; set; }

        [Display(Name = "Approved by")]
        public string approved_by_approve { get; set; }

        [Display(Name = "KPI Status")]
        public string kpi_status { get; set; }

        [Display(Name = "Reason for Status Change")]
        public string status_reason { get; set; }

        [Display(Name = "KPI Number")]
        public string kpi_ref_no { get; set; }

        [Display(Name = "Upload")]
        public string action_upload { get; set; }

        //-----------------KPI Causes for failure---------------------------------

        [Display(Name = "Id")]
        public string id_kpi_failure { get; set; }

        [Display(Name = "Potential Causes for Failure")]
        public string causes_failure { get; set; }

        [Display(Name = "Impact")]
        public string impact { get; set; }

        [Display(Name = "Mitigation Measure(s)")]
        public string mitigation_measures { get; set; }

        [Display(Name = "Target Date to implement the measure")]
        public DateTime target_date { get; set; }

        [Display(Name = "Status")]
        public string failure_status { get; set; }

        [Display(Name = "Status Updated On")]
        public DateTime status_updated_date { get; set; }

        //---------------KPI action--------------------------------------------------

        [Display(Name = "Id")]
        public string id_kpi_actions { get; set; }

        [Display(Name = "Action Details")]
        public string Action { get; set; }

        [Display(Name = "Target Date")]
        public DateTime TargetDate { get; set; }

        [Display(Name = "Person Responsible")]
        public string PersonResponsible { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        //---------------KPI evaluation--------------------------------------------------

        [Display(Name = "Id")]
        public string id_evaluation { get; set; }

        [Display(Name = "Source of data to evaluate")]
        public string source_evaluate { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime evaluation_date { get; set; }

        [Display(Name = "Method of evaluation")]
        public string method_evaluation { get; set; }

        [Display(Name = "Value achieved")]
        public string value_achieved { get; set; }

        [Display(Name = "Unit")]
        public string unit { get; set; }

        [Display(Name = "NCR Raise Need")]
        public string raise_need { get; set; }

        [Display(Name = "Approval Status")]
        public string kpiapprv_status { get; set; }

        [Display(Name = "Comments")]
        public string approvedby_comments { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime ApproveOrRejectOn { get; set; }

        [Display(Name = "From Date")]
        public DateTime FromPeriod { get; set; }

        [Display(Name = "To Date")]
        public DateTime ToPeriod { get; set; }

        public string stat_display { get; set; }

     
        //--------------------------------------------------------------------------
        internal bool FunDeleteKPI(string KPI_Id)
        {
            try
            {
                string sSqlstmt = "update t_kpi set Active='0' where KPI_Id='" + KPI_Id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPIFailure(string KPI_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_kpi_failure where KPI_Id='" + KPI_Id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPIFailure: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPI(KPIModels objKPI,KPIModelsList objModelsList, KPIModelsList objMeasureList)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string kpi_status = objGlobalData.GetKPIStatusIdByName("Active");

                string sSqlstmt = "insert into t_kpi (branch,group_name,process_indicator,kpi_level,process_monitor,"
                    + "pers_resp,upload,approved_by,logged_by,kpi_status";

                string sFields = "", sFieldValue = "";

                if (objKPI.established_date != null && objKPI.established_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", established_date";
                    sFieldValue = sFieldValue + ", '" + objKPI.established_date.ToString("yyyy/MM/dd") + "'";
                }
                //if (objKPI.monitoring_frm_date != null && objKPI.monitoring_frm_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sFields = sFields + ", monitoring_frm_date";
                //    sFieldValue = sFieldValue + ", '" + objKPI.monitoring_frm_date.ToString("yyyy/MM/dd") + "'";
                //}
                //if (objKPI.monitoring_to_date != null && objKPI.monitoring_to_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sFields = sFields + ", monitoring_to_date";
                //    sFieldValue = sFieldValue + ", '" + objKPI.monitoring_to_date.ToString("yyyy/MM/dd") + "'";
                //}
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objKPI.branch + "','" + objKPI.group_name + "','" + objKPI.process_indicator + "','" + kpi_level + "','" + process_monitor + "',"
                + "'" + pers_resp + "','" + upload + "','" + approved_by + "','" + user + "','"+ kpi_status + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int KPI_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out KPI_Id))
                {
                    if (KPI_Id > 0)
                    {
                        string sName = objGlobalData.GetBranchShortNameByID(branch);
                        DataSet dsData = objGlobalData.GetReportNo("KPI", "", sName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            kpi_ref_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        objGlobalData.ExecuteQuery("Update t_kpi set kpi_ref_no = '" + kpi_ref_no + "' where KPI_Id= '" + KPI_Id + "'");

                        //if (objModelsList.KPIMList.Count > 0)
                        //{
                        //    objModelsList.KPIMList[0].KPI_Id = KPI_Id.ToString();
                        //    FunAddKPIFailureList(objModelsList);
                          
                        //}
                        if (objMeasureList.KPIMList.Count > 0)
                        {
                            objMeasureList.KPIMList[0].KPI_Id = KPI_Id.ToString();
                            FunAddKPIMeasurableList(objMeasureList, kpi_ref_no);
                        }

                        //string sInformation = "", sHeader = "", REmail = "";
                        //string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(approved_by);
                        //REmail = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);

                        //if (sEmailid != null && sEmailid != "")
                        //{
                        //    sHeader = "<tr><td ><b>KPI Ref:<b></td> <td >"
                        //          + kpi_ref_no + "</td></tr>"
                        //          + "<tr><td ><b>Process/Performance Indicator:<b></td> <td >" +objGlobalData.GetKPIPerformanceIndicatorById(process_indicator) + "</td></tr>"
                        //          + "<tr><td ><b>KPI Level:<b></td> <td >" +objGlobalData.GetKPILevelById(kpi_level) + "</td></tr>"

                        //          + "<tr><td ><b>Process to be monitored:<b></td> <td >" + process_monitor + "</td></tr>";

                        //    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetMultiHrEmpNameById(approved_by), "kpi", sHeader, "", "KPI Details");

                        //    // sEmailCCList = sEmailCCList.Trim(',');
                        //    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        //    sEmailid = sEmailid.Trim();
                        //    sEmailid = sEmailid.TrimEnd(',');
                        //    sEmailid = sEmailid.TrimStart(',');
                        //    //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                        //    return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", REmail, "");
                        //}

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPI(KPIModels objKPI, KPIModelsList objModelsList, KPIModelsList objMeasureList)
        {
            try
            {
                string sSqlstmt = "update t_kpi set process_indicator='" + process_indicator + "',kpi_level='" + kpi_level + "',process_monitor='" + process_monitor + "',pers_resp='" + pers_resp + "',upload='" + upload + "',approved_by='" + approved_by + "'"
                    + ",kpi_status='" + kpi_status + "',status_reason='" + status_reason + "'";


                if (objKPI.established_date != null && objKPI.established_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",established_date='" + objKPI.established_date.ToString("yyyy/MM/dd") + "'";
                }
                //if (objKPI.monitoring_frm_date != null && objKPI.monitoring_frm_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                //{
                //    sSqlstmt = sSqlstmt + ",monitoring_frm_date='" + objKPI.monitoring_frm_date.ToString("yyyy/MM/dd") + "'";
                //}
                //if (objKPI.monitoring_to_date != null && objKPI.monitoring_to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                //{
                //    sSqlstmt = sSqlstmt + ",monitoring_to_date='" + objKPI.monitoring_to_date.ToString("yyyy/MM/dd") + "'";
                //}
                sSqlstmt = sSqlstmt + " where KPI_Id='" + objKPI.KPI_Id + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objModelsList.KPIMList.Count) > 0)
                    {
                        objModelsList.KPIMList[0].KPI_Id = KPI_Id.ToString();
                        FunAddKPIFailureList(objModelsList);
                    }
                    else
                    {
                        FunUpdateKPIFailure(KPI_Id);
                    }
                    if (Convert.ToInt32(objMeasureList.KPIMList.Count) > 0)
                    {
                        objMeasureList.KPIMList[0].KPI_Id = KPI_Id.ToString();
                        FunAddKPIMeasurableList(objMeasureList, kpi_ref_no);
                    }

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPIFailureList(KPIModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_kpi_failure where KPI_Id='" + objModelsList.KPIMList[0].KPI_Id + "'; ";

                for (int i = 0; i < objModelsList.KPIMList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_kpi_failure(KPI_Id,causes_failure,impact,mitigation_measures,failure_status";

                    string sFieldValue = "", sFields = "";
                    if (objModelsList.KPIMList[i].target_date != null && objModelsList.KPIMList[i].target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    { 
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.KPIMList[i].target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objModelsList.KPIMList[i].status_updated_date != null && objModelsList.KPIMList[i].status_updated_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", status_updated_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.KPIMList[i].status_updated_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModelsList.KPIMList[0].KPI_Id + "', '" + objModelsList.KPIMList[i].causes_failure + "', '" + objModelsList.KPIMList[i].impact + "', '" + objModelsList.KPIMList[i].mitigation_measures + "', '" + objModelsList.KPIMList[i].failure_status + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPIFailureList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPIMeasurableList(KPIModelsList objModelsList, string kpi_ref_no)
        {
            try
            {
                string sSqlstmt = "delete from t_kpi_measurable_indicator where KPI_Id='" + objModelsList.KPIMList[0].KPI_Id + "'; ";

                for (int i = 0; i < objModelsList.KPIMList.Count; i++)
                {
                    string ref_no = kpi_ref_no + " - " + (i+1);
                    string sid_measurable = "null";
                    sSqlstmt = sSqlstmt + "insert into t_kpi_measurable_indicator(id_measurable,KPI_Id,kpi_ref_no,measurable_indicator,expected_value,expected_value_unit,kpi_type,monitoring_mechanism,frequency_eval,risk";

                    string sFieldValue = "", sFields = "", sValue = "",sStatement = ""; ;
                    if (objModelsList.KPIMList[i].id_measurable != null)
                    {
                        sid_measurable = objModelsList.KPIMList[i].id_measurable;
                    }
                    if (objModelsList.KPIMList[i].monitoring_frm_date != null && objModelsList.KPIMList[i].monitoring_frm_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", monitoring_frm_date= values(monitoring_frm_date)";
                        sFields = sFields + ", monitoring_frm_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.KPIMList[i].monitoring_frm_date.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objModelsList.KPIMList[i].monitoring_to_date != null && objModelsList.KPIMList[i].monitoring_to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sStatement = sStatement + ", monitoring_to_date= values(monitoring_to_date)";
                        sFields = sFields + ", monitoring_to_date";
                        sFieldValue = sFieldValue + ", '" + objModelsList.KPIMList[i].monitoring_to_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values("+ sid_measurable + ",'" + objModelsList.KPIMList[0].KPI_Id + "','" + ref_no + "','" + objModelsList.KPIMList[i].measurable_indicator + "','" + objModelsList.KPIMList[i].expected_value + "','" + objModelsList.KPIMList[i].expected_value_unit + "','" + objModelsList.KPIMList[i].kpi_type + "','" + objModelsList.KPIMList[i].monitoring_mechanism + "','" + objModelsList.KPIMList[i].frequency_eval + "','" + objModelsList.KPIMList[i].risk + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_measurable= values(id_measurable), KPI_Id= values(KPI_Id), kpi_ref_no = values(kpi_ref_no), measurable_indicator= values(measurable_indicator), expected_value= values(expected_value)"
                    + ", expected_value_unit= values(expected_value_unit), kpi_type= values(kpi_type), monitoring_mechanism= values(monitoring_mechanism), frequency_eval= values(frequency_eval), risk= values(risk)";
                    sSqlstmt = sSqlstmt + sValue;
                    sSqlstmt = sSqlstmt + sStatement + ";";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPIMeasurableList: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddPotentialCauses( KPIModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "update t_kpi set kpiapprv_status='0' where KPI_Id='" + KPI_Id + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);


                if (Convert.ToInt32(objModelsList.KPIMList.Count) > 0)
                    {
                        objModelsList.KPIMList[0].KPI_Id = KPI_Id.ToString();
                        FunAddKPIFailureList(objModelsList);
                    }
                    else
                    {
                        FunUpdateKPIFailure(KPI_Id);
                    }
                    sendKPIMail(KPI_Id,"KPI Evaluation");
                    return true;
                

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPI: " + ex.ToString());
            }
            return false;
        }
        internal bool sendKPIMail(string  KPI_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,kpi_level,process_monitor,pers_resp,approved_by,logged_by,pers_resp from  t_kpi where KPI_Id='" + KPI_Id + "'";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["pers_resp"].ToString());

                    string established_date = "";
                    if (dsData.Tables[0].Rows[0]["established_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["established_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        established_date=Convert.ToDateTime(dsData.Tables[0].Rows[0]["established_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>KPI Ref No:<b></td> <td colspan=3>" + dsData.Tables[0].Rows[0]["kpi_ref_no"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>KPI Established On:<b></td> <td colspan=3>" + established_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Proposed By:<b></td> <td colspan=3>" +objGlobalData.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch"].ToString()) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp"+objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["group_name"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Process/Performance Indicator:<b></td> <td colspan=3>" + objGlobalData.GetKPIPerformanceIndicatorById(dsData.Tables[0].Rows[0]["process_indicator"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>KPI Level:<b></td> <td colspan=3>" + objGlobalData.GetKPILevelById(dsData.Tables[0].Rows[0]["kpi_level"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Process to be monitored:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["process_monitor"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Personnel Responsible:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["pers_resp"].ToString()) + "</td></tr>";


                    sSqlstmt = "select measurable_indicator,expected_value,expected_value_unit,monitoring_frm_date,monitoring_to_date,monitoring_mechanism,frequency_eval,risk from  t_kpi_measurable_indicator where KPI_Id = '"+KPI_Id+"'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<br><tr> "
                            + "<td colspan=8><label><b>Measurable Indicator</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Measurable Indicator</th>"
                            + "<th style='width:300px'>Expected value to be achieved</th>"
                            + "<th style='width:300px'>Unit</th>"
                            + "<th style='width:300px'>Monitoring Period From Date</th>"
                            + "<th style='width:300px'>To Date</th>"
                            + "<th style='width:300px'>Monitoring Mechanism</th>"
                             + "<th style='width:300px'>Frequency of Evaluation</th>"
                             + "<th style='width:300px'>Risk,if not achieved</th>"
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string monitoring_frm_date = "", monitoring_to_date="";
                            if (dsItems.Tables[0].Rows[i]["monitoring_frm_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["monitoring_frm_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                monitoring_frm_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["monitoring_frm_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            if (dsItems.Tables[0].Rows[i]["monitoring_to_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["monitoring_to_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                monitoring_to_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["monitoring_to_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measurable_indicator"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["expected_value"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["expected_value_unit"].ToString()) + "</td>"
                                  + " <td style='width:300px'>" + monitoring_frm_date + "</td>"
                                     + " <td style='width:300px'>" + monitoring_to_date + "</td>"
                                     + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["monitoring_mechanism"].ToString()) + "</td>"
                                      + " <td style='width:300px'>" +objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["frequency_eval"].ToString()) + "</td>"
                                           + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["risk"].ToString()) + "</td>"
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


                    sSqlstmt = "select causes_failure,impact,mitigation_measures,failure_status,target_date,status_updated_date from t_kpi_failure where KPI_Id = '" + KPI_Id + "'";

                    dsItems = new DataSet();
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation= sInformation +  "<br><br><tr> "
                            + "<td colspan=8><label><b>Potential Causes for Failure to achieve KPI</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Potential Causes for Failure</th>"
                            + "<th style='width:300px'>Impact</th>"
                            + "<th style='width:300px'>Mitigation Measure(s)</th>"
                            + "<th style='width:300px'>Target Date to implement the measure</th>"
                            + "<th style='width:300px'>Status</th>"
                            + "<th style='width:300px'>Status Updated On</th>"                     
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string target_date = "", status_updated_date = "";
                            if (dsItems.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                target_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            if (dsItems.Tables[0].Rows[i]["status_updated_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["status_updated_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                status_updated_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["status_updated_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["causes_failure"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["impact"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["mitigation_measures"].ToString()) + "</td>"
                                  + " <td style='width:300px'>" + target_date + "</td>"
                                     + " <td style='width:300px'>" +objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["failure_status"].ToString()) + "</td>"
                                           + " <td style='width:300px'>" + status_updated_date + "</td>"
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
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "KPI Evaluation Details");
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
                objGlobalData.AddFunctionalLog("Exception in sendKPIMail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateApprove(KPIModels objManagement)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "update t_kpi set kpiapprv_status='" + kpiapprv_status + "',approvedby_comments='" + approvedby_comments + "',ApproveOrRejectOn='" + sApprovedDate + "' where KPI_Id='" + KPI_Id + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return SendKPIApprovemail(KPI_Id, "KPI Approval Status");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateApprove: " + ex.ToString());
            }
            return false;
        }
        internal bool SendKPIApprovemail(string KPI_Id, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select KPI_Id,kpi_ref_no,established_date,branch,group_name,process_indicator,kpi_level,process_monitor,pers_resp,approved_by,logged_by,pers_resp,"
                      + "(CASE WHEN kpiapprv_status='0' THEN 'Pending for Approval' WHEN kpiapprv_status='1' THEN 'Rejected' ELSE 'Approved' END) as kpiapprv_status,approvedby_comments"
                    + " from  t_kpi where KPI_Id='" + KPI_Id + "'";

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
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["approved_by"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());

                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["pers_resp"].ToString());

                    string established_date = "";
                    if (dsData.Tables[0].Rows[0]["established_date"].ToString() != null && Convert.ToDateTime(dsData.Tables[0].Rows[0]["established_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                    {
                        established_date = Convert.ToDateTime(dsData.Tables[0].Rows[0]["established_date"].ToString()).ToString("yyyy-MM-dd");
                    }
                    sHeader = "<tr><td colspan=3><b>KPI Ref No:<b></td> <td colspan=3>" + dsData.Tables[0].Rows[0]["kpi_ref_no"].ToString() + "</td></tr>"
                    + "<tr><td colspan=3><b>KPI Established On:<b></td> <td colspan=3>" + established_date + "</td></tr>"
                    + "<tr><td colspan=3><b>Proposed By:<b></td> <td colspan=3>" + objGlobalData.GetCompanyBranchNameById(dsData.Tables[0].Rows[0]["branch"].ToString()) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp" + objGlobalData.GetDeptNameById(dsData.Tables[0].Rows[0]["group_name"].ToString()) + "</td></tr>"

                     + "<tr><td colspan=3><b>Process/Performance Indicator:<b></td> <td colspan=3>" + objGlobalData.GetKPIPerformanceIndicatorById(dsData.Tables[0].Rows[0]["process_indicator"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>KPI Level:<b></td> <td colspan=3>" + objGlobalData.GetKPILevelById(dsData.Tables[0].Rows[0]["kpi_level"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Process to be monitored:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["process_monitor"].ToString()) + "</td></tr>"

                    + "<tr><td colspan=3><b>Personnel Responsible:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["pers_resp"].ToString()) + "</td></tr>"

                    +"<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["kpiapprv_status"].ToString()) + "</td></tr>"

                    +"<tr><td colspan=3><b>Comments:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["approvedby_comments"].ToString()) + "</td></tr>";


                    sSqlstmt = "select measurable_indicator,expected_value,expected_value_unit,monitoring_frm_date,monitoring_to_date,monitoring_mechanism,frequency_eval,risk from  t_kpi_measurable_indicator where KPI_Id = '" + KPI_Id + "'";

                    DataSet dsItems = new DataSet();
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = "<tr> "
                            + "<td colspan=8><label><b>Measurable Indicator</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Measurable Indicator</th>"
                            + "<th style='width:300px'>Expected value to be achieved</th>"
                            + "<th style='width:300px'>Unit</th>"
                          
                            + "<th style='width:300px'>Monitoring Mechanism</th>"
                             + "<th style='width:300px'>Frequency of Evaluation</th>"
                             + "<th style='width:300px'>Risk,if not achieved</th>"
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string monitoring_frm_date = "", monitoring_to_date = "";
                          
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["measurable_indicator"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["expected_value"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["expected_value_unit"].ToString()) + "</td>"
                              
                                     + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["monitoring_mechanism"].ToString()) + "</td>"
                                      + " <td style='width:300px'>" + objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["frequency_eval"].ToString()) + "</td>"
                                           + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["risk"].ToString()) + "</td>"
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


                    sSqlstmt = "select causes_failure,impact,mitigation_measures,failure_status,target_date,status_updated_date from t_kpi_failure where KPI_Id = '" + KPI_Id + "'";

                    dsItems = new DataSet();
                    dsItems = objGlobalData.Getdetails(sSqlstmt);

                    if (dsItems.Tables.Count > 0 && dsItems.Tables[0].Rows.Count > 0)
                    {
                        sInformation = sInformation + "<tr> "
                            + "<td colspan=8><label><b>Potential Causes for Failure to achieve KPI</b></label> </td> </tr>"
                            + "<tr style='background-color: #4cc4dd; width: 100%; color: white; padding-left: 5px;'>"
                            + "<th>Sl. No.</th>"
                            + "<th style='width:300px'>Potential Causes for Failure</th>"
                            + "<th style='width:300px'>Impact</th>"
                            + "<th style='width:300px'>Mitigation Measure(s)</th>"
                            + "<th style='width:300px'>Target Date to implement the measure</th>"
                            + "<th style='width:300px'>Status</th>"
                            + "<th style='width:300px'>Status Updated On</th>"
                            + "</tr>";


                        for (int i = 0; i < dsItems.Tables[0].Rows.Count; i++)
                        {

                            string target_date = "", status_updated_date = "";
                            if (dsItems.Tables[0].Rows[i]["target_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                target_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["target_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            if (dsItems.Tables[0].Rows[i]["status_updated_date"].ToString() != null && Convert.ToDateTime(dsItems.Tables[0].Rows[i]["status_updated_date"].ToString()) > Convert.ToDateTime("01/01/0001"))
                            {
                                status_updated_date = Convert.ToDateTime(dsItems.Tables[0].Rows[i]["status_updated_date"].ToString()).ToString("yyyy-MM-dd");
                            }
                            sInformation = sInformation + "<tr>"
                                + " <td>" + (i + 1) + "</td>"
                                + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["causes_failure"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["impact"].ToString()) + "</td>"
                                 + " <td style='width:300px'>" + (dsItems.Tables[0].Rows[i]["mitigation_measures"].ToString()) + "</td>"
                                  + " <td style='width:300px'>" + target_date + "</td>"
                                     + " <td style='width:300px'>" + objGlobalData.GetDropdownitemById(dsItems.Tables[0].Rows[i]["failure_status"].ToString()) + "</td>"
                                           + " <td style='width:300px'>" + status_updated_date + "</td>"
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
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "KPI Evaluation Details");
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
                objGlobalData.AddFunctionalLog("Exception in SendKPIApprovemail: " + ex.ToString());
            }
            return false;
        }
        //internal bool SendKPIApprovemail( string KPI_Id, string sMessage = "")
        //{
        //    try
        //    {
        //        string sType = "email";

        //        string sSqlstmt = "select approved_by,logged_by,established_date,KPI_Id,kpi_ref_no,process_indicator,kpi_level,process_monitor,"
        //              + "(CASE WHEN kpiapprv_status='0' THEN 'Pending for Approval' WHEN kpiapprv_status='1' THEN 'Rejected' ELSE 'Approved' END) as kpiapprv_status"
        //            + " from t_kpi where KPI_Id='" + KPI_Id + "'";

        //        DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
        //        KPIModels objType = new KPIModels();

        //        if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
        //        {
        //            //objGlobalData.AddFunctionalLog("Step");
        //            //AddFunctionalLog("Step");
        //            string sName, sToEmailIds, sCCEmailIds, sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

        //            //using streamreader for reading my htmltemplate 
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

        //            sName = objGlobalData.GetMultiHrEmpNameById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
        //            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["logged_by"].ToString());
        //            sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsData.Tables[0].Rows[0]["approved_by"].ToString());


        //            sHeader = "<tr><td colspan=3><b>KPI Ref No:<b></td> <td colspan=3>"
        //               + (dsData.Tables[0].Rows[0]["kpi_ref_no"].ToString()) + "</td></tr>"
        //                 + "<tr><td colspan=3><b>Established Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsData.Tables[0].Rows[0]["established_date"].ToString()).ToString("yyyy-MM-dd") + "</td></tr>"
        //               + "<tr><td colspan=3><b>Process/Performance Indicator:<b></td> <td colspan=3>" + objGlobalData.GetKPIPerformanceIndicatorById(dsData.Tables[0].Rows[0]["process_indicator"].ToString()) + "</td></tr>"
        //              + "<tr><td colspan=3><b>KPI Level:<b></td> <td colspan=3>" + objGlobalData.GetKPILevelById(dsData.Tables[0].Rows[0]["kpi_level"].ToString()) + "</td></tr>"
        //              + "<tr><td colspan=3><b>Process to be monitored:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["process_monitor"].ToString()) + "</td></tr>"
        //               + "<tr><td colspan=3><b>Approval Status:<b></td> <td colspan=3>" + (dsData.Tables[0].Rows[0]["kpiapprv_status"].ToString()) + "</td></tr>";



        //            sContent = sContent.Replace("{FromMsg}", "");
        //            sContent = sContent.Replace("{UserName}", sName);
        //            sContent = sContent.Replace("{Title}", "KPI Details");
        //            sContent = sContent.Replace("{content}", sHeader + sInformation);
        //            sContent = sContent.Replace("{message}", "");
        //            sContent = sContent.Replace("{extramessage}", "");

        //            sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
        //            sToEmailIds = sToEmailIds.Trim();
        //            sToEmailIds = sToEmailIds.TrimEnd(',');
        //            sToEmailIds = sToEmailIds.TrimStart(',');

        //            sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
        //            sCCEmailIds = sCCEmailIds.Trim();
        //            sCCEmailIds = sCCEmailIds.TrimEnd(',');
        //            sCCEmailIds = sCCEmailIds.TrimStart(',');


        //            return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in SendKPIApprovemail: " + ex.ToString());
        //    }
        //    return false;
        //}
        //--------------------------Action Plan---------------------------------------------

        internal bool FunAddActionPlan(KPIModels objKPI, KPIModelsList objModelsList)
        {
            try
            {
              
                string sql = "update t_kpi set action_upload='" + action_upload + "' where KPI_Id='" + KPI_Id + "'";
                objGlobalData.ExecuteQuery(sql);

                string sSqlstmt = "";
                sSqlstmt = "delete from t_kpi_actions where KPI_Id='" + KPI_Id + "'; ";
                for (int i = 0; i < objModelsList.KPIMList.Count; i++)
                {
                    if (objModelsList.KPIMList[i].Action != null)
                    {
                        sSqlstmt = sSqlstmt + " insert into t_kpi_actions (KPI_Id,Action,PersonResponsible,remarks";

                        string sFields = "", sFieldValue = "";
                        if (objModelsList.KPIMList[i].TargetDate != null && objModelsList.KPIMList[i].TargetDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                        {
                            sFields = sFields + ", TargetDate";
                            sFieldValue = sFieldValue + ", '" + objModelsList.KPIMList[i].TargetDate.ToString("yyyy/MM/dd") + "'";
                        }
                     
                        sSqlstmt = sSqlstmt + sFields;
                        sSqlstmt = sSqlstmt + ") values('" + KPI_Id + "','" + objModelsList.KPIMList[i].Action
                          + "','" + objModelsList.KPIMList[i].PersonResponsible + "','" + objModelsList.KPIMList[i].remarks + "'";
                        sSqlstmt = sSqlstmt + sFieldValue + ");";

                    }
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
              
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddActionPlan: " + ex.ToString());
            }
            return false;
        }

        //--------------------------Evaluation---------------------------------------------

        internal bool FunAddKPIEvaluation(KPIModels objKPI)
        {
            try
            {

             
                string sSqlstmt = "insert into t_kpi_evaluation (KPI_Id,id_measurable,source_evaluate,upload,method_evaluation,value_achieved,unit,kpi_status,remarks,raise_need";

                string sFields = "", sFieldValue = "";

                if (objKPI.evaluation_date != null && objKPI.evaluation_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", evaluation_date";
                    sFieldValue = sFieldValue + ", '" + objKPI.evaluation_date.ToString("yyyy/MM/dd") + "'";
                }
              
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objKPI.KPI_Id + "','" + objKPI.id_measurable + "','" + objKPI.source_evaluate + "','" + objKPI.upload + "','" + method_evaluation + "','" + value_achieved + "',"
                + "'" + unit + "','" + kpi_status + "','" + remarks + "','" + raise_need + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPIEvaluation: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateKPIEvaluation(KPIModels objKPI)
        {
            try
            {
                string sSqlstmt = "update t_kpi_evaluation set source_evaluate='" + source_evaluate + "',upload='" + upload + "',method_evaluation='" + method_evaluation + "',value_achieved='" + value_achieved + "',unit='" + unit + "',kpi_status='" + kpi_status + "'"
                    + ",remarks='" + remarks + "',raise_need='" + raise_need + "'";

                if (objKPI.evaluation_date != null && objKPI.evaluation_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",evaluation_date='" + objKPI.evaluation_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_evaluation='" + objKPI.id_evaluation + "'";
                return (objGlobalData.ExecuteQuery(sSqlstmt));

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPIEvaluation: " + ex.ToString());
            }
            return false;
        }
    }

    public class KPIModelsList
    {
        public List<KPIModels> KPIMList { get; set; }
    }   
}