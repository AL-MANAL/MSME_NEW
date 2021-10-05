using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class NCModels
    {
        clsGlobal objGlobaldata = new clsGlobal();        

        [Display(Name = "Id")]
        public string id_nc { get; set; }

        [Display(Name = "NC No.")]
        public string nc_no { get; set; }

        [Display(Name = "Id")]
        public string id_nc_issue { get; set; }

        [Display(Name = "NC reported on")]
        public DateTime nc_reported_date { get; set; }

        [Display(Name = "NC detected on")]
        public DateTime nc_detected_date { get; set; }

        [Display(Name = "NC Category")]
        public string nc_category { get; set; }

        [Display(Name = "NC description in detail")]
        public string nc_description { get; set; }

        [Display(Name = "Activity where NC observed")]
        public string nc_activity { get; set; }

        [Display(Name = "Location or Area where Activity performed")]
        public string nc_performed { get; set; }    
        
        [Display(Name = "NC related to")]
        public string id_nc_related { get; set; }

        [Display(Name = "Why it is NC or PNC?")]
        public string nc_pnc { get; set; }

        [Display(Name = "Upload documents")]
        public string nc_upload { get; set; }

        [Display(Name = "NC impact")]
        public string nc_impact { get; set; }

        [Display(Name = "Risk due to NC")]
        public string nc_risk { get; set; }

        [Display(Name = "Risk level")]
        public string risklevel { get; set; }

        [Display(Name = "NC reported by")]
        public string nc_reportedby { get; set; }
        public string nc_reportedby_dept { get; set; }
        public string nc_reportedbyId { get; set; }

        [Display(Name = "NC notified to")]
        public string nc_notifiedto { get; set; }
        public string nc_notifiedtoId { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "NC Issued To")]
        public string nc_issueto { get; set; }
        public string nc_issueto_dept { get; set; }
        public string nc_issuetoId { get; set; }

        [Display(Name = "Division")]
        public string nc_division { get; set; }

        [Display(Name = "Is NC Related To Audit ?")]
        public string nc_audit { get; set; }

        [Display(Name = "Audit Number")]
        public string audit_no { get; set; }
        ///

        [Display(Name = "Immediate actions initiated")]
        public string id_nc_disp_action { get; set; }

        [Display(Name = "Are actions effective to resolve NC?")]
        public string disp_action_taken { get; set; }

        //[Display(Name = "Consequences")]
        //[DataType(DataType.MultilineText)]
        //public string disp_explain { get; set; }

        [Display(Name = "Brief explanation")]
        public string disp_explain { get; set; }

        [Display(Name = "Notified to")]
        public string disp_notifiedto { get; set; }

        [Display(Name = "Notified on")]
        public DateTime disp_notifeddate { get; set; }

        [Display(Name = "NC Raised due to")]
        public string nc_raise_dueto { get; set; }

        public string logged_by { get; set; }

        [Display(Name = "NC Status")]
        public string nc_issuedto_status { get; set; }
        public string nc_issuedto_statusId { get; set; }

        [Display(Name = "Upload Document")]
        public string nc_reject_upload { get; set; }

        [Display(Name = "Reason for not accepting NC")]
        public string nc_reject_comment { get; set; }

        public string nc_issuedto { get; set; }
        public string nc_stauts { get; set; }
        public DateTime nc_approve_reject_date { get; set; }
        public string nc_initial_status { get; set; }
        public string nc_initial_statusId { get; set; }
        public string nc_issuers { get; set; }
        public string nc_issuer_rejector { get; set; }

        //
        [Display(Name = "Team")]
        public string nc_team { get; set; }

        [Display(Name = "Team Approved By")]
        public string team_approvedby { get; set; }

        [Display(Name = "Notified to")]
        public string team_notifiedto { get; set; }

        [Display(Name = "Target date to complete the Root Cause Analysis")]
        public DateTime team_targetdate { get; set; }

        //
        [Display(Name = "Techniques adopted")]
        public string rca_technique { get; set; }

        [Display(Name = "Details of root causes analysis")]
        public string rca_details { get; set; }

        [Display(Name = "Upload documents")]
        public string rca_upload { get; set; }

        [Display(Name = "Need of corrective action")]
        public string rca_action { get; set; }

        [Display(Name = "If No, justify")]
        public string rca_justify { get; set; }

        [Display(Name = "RCA Completed By")]
        public string rca_reportedby { get; set; }

        [Display(Name = "Notified to")]
        public string rca_notifiedto { get; set; }

        [Display(Name = "RCA Completed / Reported On")]
        public DateTime rca_reporteddate { get; set; }

        [Display(Name = "RCA Started On")]
        public DateTime rca_startedon { get; set; }

        //

        [Display(Name = "Id")]
        public string id_nc_corrective_action { get; set; }

        [Display(Name = "Verification due date")]
        public DateTime ca_verfiry_duedate { get; set; }

        [Display(Name = "To be verified by")]
        public string ca_proposed_by { get; set; }

        [Display(Name = "Notified to")]
        public string ca_notifiedto { get; set; }

        [Display(Name = "Notified Date")]
        public DateTime ca_notifed_date { get; set; }

        /// <summary>
        [Display(Name = "Are proposed actions implemented effectively?")]
        public string v_implement { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_implement_explain { get; set; }

        [Display(Name = "Is RCA process effective?")]
        public string v_rca { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_rca_explain { get; set; }

        [Display(Name = "Similar discrepancies detected from date of implementing corrective action?")]
        public string v_discrepancies { get; set; }

        [Display(Name = "Brief explanation")]
        public string v_discrep_explain { get; set; }

        [Display(Name = "Upload Documents")]
        public string v_upload { get; set; }

        [Display(Name = "NC Status")]
        public string v_status { get; set; }

        [Display(Name = "Closed Date")]
        public DateTime v_closed_date { get; set; }

        [Display(Name = "Verified by")]
        public string v_verifiedto { get; set; }

        [Display(Name = "Verified On")]
        public DateTime v_verified_date { get; set; }

        [Display(Name = "Notified To")]
        public string v_notifiedto { get; set; }

        //t_nc_issue

        [Display(Name = "Name")]
        public string nc_issue_name { get; set; }

        [Display(Name = "Entity")]
        public string nc_issue_entity { get; set; }

        [Display(Name = "Division")]
        public string nc_issue_div { get; set; }

        [Display(Name = "Location")]
        public string nc_issue_loc { get; set; }

        [Display(Name = "Department")]
        public string nc_issue_dept { get; set; }

        //t_nc_related

        [Display(Name = "Which aspect(s)?")]
        public string nc_related_aspect { get; set; }

        [Display(Name = "Brief explanation")]
        public string nc_related_explain { get; set; }

        [Display(Name = "Document reference")]
        public string nc_related_doc { get; set; }

        //t_nc_disp_action

        [Display(Name = "Action Taken/To be Taken")]
        public string disp_action { get; set; }

        [Display(Name = "Person Responsible")]
        public string disp_resp_person { get; set; }

        [Display(Name = "Action completed on / Target date to complete")]
        public DateTime disp_complete_date { get; set; }

        //t_nc_corrective_action

        [Display(Name = "Division")]
        public string ca_div { get; set; }

        [Display(Name = "Location")]
        public string ca_loc { get; set; }

        [Display(Name = "Department")]
        public string ca_dept { get; set; }

        [Display(Name = "Root cause")]
        public string ca_rootcause { get; set; }

        [Display(Name = "Corrective action")]
        public string ca_ca { get; set; }

        [Display(Name = "Resource required")]
        public string ca_resource { get; set; }

        [Display(Name = "Target date")]
        public DateTime ca_target_date { get; set; }        

        [Display(Name = "Person Responsible")]
        public string ca_resp_person { get; set; }

        [Display(Name = "Implementation status")]
        public string implement_status { get; set; }

        [Display(Name = "Is Corrective Action effective?")]
        public string ca_effective { get; set; }

        [Display(Name = "If No, reasons")]
        public string reason { get; set; }

        public string emp_id { get; set; }
        //public int emp_no { get; set; }
        public string EmailId { get; set; }
        public string empname { get; set; }
        public string divisionName { get; set; }
        public string departmentName { get; set; }
        public string locationName { get; set; }
        public string MobileNo { get; set; }
        public string Eid_no { get; set; }
        public string Date_of_Birth { get; set; }
        public string Nationaliity { get; set; }

        //New field

        [Display(Name = "Upload Document")]
        public string disp_upload { get; set; }

        //t_nc
        [Display(Name = "OA No")]
        public string oa_no { get; set; }

        [Display(Name = "Model code")]
        public string model_code { get; set; }

        [Display(Name = "Part Name")]
        public string part_name { get; set; }

        [Display(Name = "Stage")]
        public string stage { get; set; }

        [Display(Name = "Responsible person")]
        public string nc_resp_pers { get; set; }

        [Display(Name = "Supplier name")]
        public string supplier_name { get; set; }

        [Display(Name = "Supplier DC")]
        public string supplier_dc { get; set; }

        [Display(Name = "DC/PO")]
        public string dc_po { get; set; }

        [Display(Name = "Batch qty")]
        public string batch_qty { get; set; }

        [Display(Name = "NC qty")]
        public string nc_qty { get; set; }

        


        //NC
        internal bool FunAddNC(NCModels objModels, NCModelsList objRelatedList)
        {
            try
            {             
                
                string sFields = "", sFieldValue = "";
                string sdivision = objGlobaldata.GetCurrentUserSession().division;
                int issuecount = 0;
                if (objModels.nc_issueto != null && objModels.nc_issueto != "")
                {
                    string[] name = objModels.nc_issueto.Split(',');
                    issuecount = name.Length;


                    string otherdiv="";
                    DataSet dsEmpLocList=objGlobaldata.Getdetails("select group_concat(division) as division from t_hr_employee where find_in_set(emp_no,'" + objModels.nc_issueto + "')");
                    if(dsEmpLocList != null && dsEmpLocList.Tables.Count>0 && dsEmpLocList.Tables[0].Rows.Count > 0)
                    {
                        otherdiv = dsEmpLocList.Tables[0].Rows[0]["division"].ToString();
                    }
                    sdivision = sdivision + "," + otherdiv;
                    List<string> uniques = sdivision.Split(',').Distinct().ToList();
                    sdivision = string.Join(",", uniques);
                    sdivision = sdivision.Trim();
                }
                string sSqlstmt = "insert into t_nc ( nc_category, nc_description, nc_activity, nc_performed, nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby, nc_notifiedto, nc_division, department,location,logged_by,nc_issueto,nc_audit,audit_no,division,nc_raise_dueto,nc_issuedtocount"
                    + ",oa_no,model_code,part_name,stage,nc_resp_pers,supplier_name,supplier_dc,dc_po,batch_qty,nc_qty";

                if (objModels.nc_reported_date != null && objModels.nc_reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", nc_reported_date";
                    sFieldValue = sFieldValue + ", '" + objModels.nc_reported_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.nc_detected_date != null && objModels.nc_detected_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", nc_detected_date";
                    sFieldValue = sFieldValue + ", '" + objModels.nc_detected_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields + ") values('" + objModels.nc_category + "','" + objModels.nc_description
                + "','" + objModels.nc_activity + "','" + objModels.nc_performed + "','" + objModels.nc_pnc + "','" + objModels.nc_upload
                + "','" + objModels.nc_impact + "','" + objModels.nc_risk + "','" + objModels.risklevel + "','" + objModels.nc_reportedby
                + "','" + objModels.nc_notifiedto + "','" + objModels.nc_division + "','" + objModels.department + "','" + objModels.location
                + "','" + objGlobaldata.GetCurrentUserSession().empid + "','" + objModels.nc_issueto + "','" + objModels.nc_audit + "','" + objModels.audit_no
                + "','" + sdivision + "','" + objModels.nc_raise_dueto + "','" + issuecount + "',"
                + "'" + oa_no + "','" + model_code + "','" + part_name + "','" + stage + "','" + nc_resp_pers + "','" + supplier_name + "','" + supplier_dc + "','" + dc_po + "','" + batch_qty + "','" + nc_qty + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int iid_nc = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_nc))
                {                    
                    //if (iid_nc > 0 && Convert.ToInt32(objIssueList.lstNC.Count) > 0)
                    //{
                    //    objIssueList.lstNC[0].id_nc = iid_nc.ToString();
                    //    FunAddNCIssue(objIssueList);
                    //}

                    if (iid_nc > 0 && Convert.ToInt32(objRelatedList.lstNC.Count) > 0)
                    {
                        objRelatedList.lstNC[0].id_nc = iid_nc.ToString();
                        FunAddNCRelated(objRelatedList);
                    }

                    if (iid_nc > 0)
                    {
                        DataSet dsData = objGlobaldata.GetReportNo("NonCon", "", objGlobaldata.GetCompanyBranchNameById(sdivision));
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            nc_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        string sql1 = "update t_nc set nc_no='" + nc_no + "' where id_nc='" + iid_nc + "'";
                        if(objGlobaldata.ExecuteQuery(sql1))
                        {
                            SendNCEmail(iid_nc, "NonConformance & Corrective Action");
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddNC: " + ex.ToString());
            }
            return false;
        }

        internal bool SendNCEmail(int id_nc, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed,  nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, division, department, location,nc_issueto,logged_by from t_nc where id_nc ='" + id_nc + "'";

                DataSet dsNCList = objGlobaldata.Getdetails(sSqlstmt);
                NCModels objType = new NCModels();
               
                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_issueto"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_issueto"].ToString()) + ",";
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_notifiedto"].ToString()) != "")
                    {
                        sToEmailIds = sToEmailIds+","+ objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_notifiedto"].ToString()) + ",";
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_reportedby"].ToString()) +"," + objGlobaldata.GetHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["logged_by"].ToString());
                    aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["nc_upload"].ToString());


                    sHeader = "<tr><td colspan=3><b>NC Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["nc_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Category:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["nc_category"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>NC Description:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["nc_description"].ToString() + "</td></tr>"

                       + "<tr><td colspan=3><b>Reported Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["nc_reported_date"].ToString()).ToString("dd/MM/yyyy")
                       + "</td></tr>"
                    + "<tr><td colspan=3><b>NC Activity:<b></td> <td colspan=3>" +(dsNCList.Tables[0].Rows[0]["nc_activity"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>NC Risk:<b></td> <td colspan=3>" + (dsNCList.Tables[0].Rows[0]["nc_risk"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "NC Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateNC(NCModels objModels, NCModelsList objRelatedList)
        {
            try
            {
                //if (objModels.nc_issueto != "")
                //{
                //    string otherdiv = "";
                //    DataSet dsEmpLocList = objGlobaldata.Getdetails("select group_concat(Emp_work_location) as work_loc from t_hr_employee where find_in_set(emp_no,'" + objModels.nc_issueto + "')");
                //    if (dsEmpLocList != null && dsEmpLocList.Tables.Count > 0 && dsEmpLocList.Tables[0].Rows.Count > 0)
                //    {
                //        otherdiv = dsEmpLocList.Tables[0].Rows[0]["work_loc"].ToString();
                //    }
                //    string sdivision = objModels.division + "," + otherdiv;
                //    List<string> uniques = sdivision.Split(',').Distinct().ToList();
                //    sdivision = string.Join(",", uniques);
                //    sdivision = sdivision.Trim();
                //}

                string sSqlstmt = "update t_nc set  nc_category='" + objModels.nc_category + "', " + "nc_description='" + objModels.nc_description + "', nc_activity='" + objModels.nc_activity
                     + "', nc_performed='" + objModels.nc_performed + "', nc_pnc='" + objModels.nc_pnc + "', nc_upload='" + objModels.nc_upload + "', nc_impact='" + objModels.nc_impact
                     + "', nc_risk='" + objModels.nc_risk + "', risklevel='" + objModels.risklevel + "', nc_reportedby='" + objModels.nc_reportedby + "', nc_notifiedto='" + objModels.nc_notifiedto
                     + "', nc_division='" + objModels.nc_division + "', department='" + objModels.department + "', location='" + objModels.location/* + "', nc_issueto='" + objModels.nc_issueto*/
                     + "', nc_audit='" + objModels.nc_audit + "', audit_no='" + objModels.audit_no + "', nc_raise_dueto='" + objModels.nc_raise_dueto + "'"
                     + ", oa_no='" + objModels.oa_no + "', model_code='" + objModels.model_code + "', part_name='" + objModels.part_name + "', stage='" + objModels.stage + "', nc_resp_pers='" + objModels.nc_resp_pers + "', supplier_name='" + objModels.supplier_name + "', supplier_dc='" + objModels.supplier_dc + "', dc_po='" + objModels.dc_po + "', batch_qty='" + objModels.batch_qty + "', nc_qty='" + objModels.nc_qty + "'";

                if (objModels.nc_reported_date != null && objModels.nc_reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", nc_reported_date ='" + objModels.nc_reported_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModels.nc_detected_date != null && objModels.nc_detected_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", nc_detected_date ='" + objModels.nc_detected_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    //if (Convert.ToInt32(objIssueList.lstNC.Count) > 0)
                    //{
                    //    objIssueList.lstNC[0].id_nc = id_nc.ToString();
                    //    FunAddNCIssue(objIssueList);
                    //}
                    //else
                    //{
                    //    FunUpdateNCIssue(id_nc);
                    //}

                    if (Convert.ToInt32(objRelatedList.lstNC.Count) > 0)
                    {
                        objRelatedList.lstNC[0].id_nc = id_nc.ToString();
                        FunAddNCRelated(objRelatedList);
                    }
                    else
                    {
                        FunUpdateNCRelated(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDisposition: " + ex.ToString());
            }
            return false;
        }

        //NCIssue List
        internal bool FunAddNCIssue(NCModelsList objIssueList)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_issue where id_nc='" + objIssueList.lstNC[0].id_nc + "'; ";

                for (int i = 0; i < objIssueList.lstNC.Count; i++)
                {
                    //string sid_accident_info = "null";
                    //string sreportedon_date =objIssueList.lstNC[i].reportedon_date.ToString("yyyy-MM-dd");

                    //if (objIssueList.lstNC[i].id_accident_info != null)
                    //{
                    //    sid_accident_info = objIssueList.lstNC[i].id_accident_info;
                    //}
                    sSqlstmt = sSqlstmt + " insert into t_nc_issue (id_nc,nc_issue_name,nc_issue_entity,nc_issue_div,nc_issue_loc,nc_issue_dept)"
                    + " values(" +objIssueList.lstNC[0].id_nc + ",'" +objIssueList.lstNC[i].nc_issue_name + "','" + objIssueList.lstNC[i].nc_issue_entity
                    + "','" +objIssueList.lstNC[i].nc_issue_div + "','" + objIssueList.lstNC[i].nc_issue_loc + "','" + objIssueList.lstNC[i].nc_issue_dept + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_nc= values(id_nc), nc_issue_name= values(nc_issue_name), nc_issue_entity = values(nc_issue_entity), nc_issue_div= values(nc_issue_div)," 
                    + " nc_issue_loc= values(nc_issue_loc), nc_issue_dept= values(nc_issue_dept) ; ";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddNCIssue: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateNCIssue(string sid_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_issue where id_nc='" + sid_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNCIssue: " + ex.ToString());
            }
            return false;
        }

        //NCRelated List
        internal bool FunAddNCRelated(NCModelsList objRelatedList)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_related where id_nc='" + objRelatedList.lstNC[0].id_nc + "'; ";

                for (int i = 0; i < objRelatedList.lstNC.Count; i++)
                {
                    sSqlstmt = sSqlstmt + " insert into t_nc_related (id_nc,nc_related_aspect,nc_related_explain,nc_related_doc)"
                    + " values(" + objRelatedList.lstNC[0].id_nc + ",'" + objRelatedList.lstNC[i].nc_related_aspect + "','" + objRelatedList.lstNC[i].nc_related_explain
                    + "','" + objRelatedList.lstNC[i].nc_related_doc + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_nc= values(id_nc), nc_related_aspect= values(nc_related_aspect), nc_related_explain = values(nc_related_explain), nc_related_doc = values(nc_related_doc); ";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddNCRelated: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateNCRelated(string sid_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_related where id_nc='" + sid_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateNCRelated: " + ex.ToString());
            }
            return false;
        }

         
        //Disposition

        internal bool FunUpdateDisposition (NCModels objModels, NCModelsList objDispList)
        {
            try
            {
               
                string sSqlstmt = "update t_nc set  disp_action_taken='" + objModels.disp_action_taken + "', "
                    + "disp_explain='" + objModels.disp_explain + "', disp_notifiedto='" + objModels.disp_notifiedto + "', disp_upload='" + objModels.disp_upload + "'";

                if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objDispList.lstNC.Count) > 0)
                    {
                        objDispList.lstNC[0].id_nc = id_nc.ToString();
                        FunAddDispList(objDispList);
                    }
                    else
                    {
                        FunUpdateDispList(id_nc);
                    }
                    return true;
                }                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDisposition: " + ex.ToString());
            }
            return true;
        }

        internal bool FunAddDispList(NCModelsList objDispList)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_disp_action where id_nc='" + objDispList.lstNC[0].id_nc + "'; ";

                for (int i = 0; i < objDispList.lstNC.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_nc_disp_action(id_nc,disp_action,disp_resp_person";

                    string sFieldValue = "", sFields = "";
                    if (objDispList.lstNC[i].disp_complete_date != null && objDispList.lstNC[i].disp_complete_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sFields = sFields + ", disp_complete_date";
                        sFieldValue = sFieldValue + ", '" + objDispList.lstNC[i].disp_complete_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objDispList.lstNC[0].id_nc + "', '" + objDispList.lstNC[i].disp_action + "', '" + objDispList.lstNC[i].disp_resp_person + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddDispList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateDispList(string sid_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_disp_action where id_nc='" + sid_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateDispList: " + ex.ToString());
            }
            return false;
        }

        //Team
        internal bool FunUpdateTeam(NCModels objModels)
        {
            try
            {

                string sSqlstmt = "update t_nc set  nc_team='" + objModels.nc_team + "', "
                    + "team_approvedby='" + objModels.team_approvedby + "', team_notifiedto='" + objModels.team_notifiedto + "'";

                if (objModels.team_targetdate != null && objModels.team_targetdate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", team_targetdate ='" + objModels.team_targetdate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    SendNCTeamEmail(objModels, "NonConformance & Corrective Action");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateTeam: " + ex.ToString());
            }
            return false;
        }


        internal bool SendNCTeamEmail(NCModels objModels, string sMessage = "")
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_nc, nc_no, nc_reported_date, nc_detected_date, nc_category, nc_description, nc_activity, nc_performed,  nc_pnc, nc_upload,"
                    + "nc_impact, nc_risk, risklevel, nc_reportedby,  nc_notifiedto, team_targetdate,team_notifiedto,team_approvedby,nc_team from t_nc where id_nc ='" + id_nc + "'";

                DataSet dsNCList = objGlobaldata.Getdetails(sSqlstmt);
                NCModels objType = new NCModels();

                if (dsNCList.Tables.Count > 0 && dsNCList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sInformation = "", sTitle = "", sSubject = "", sContent = "", aAttachment = "", BccEmailIds = "";

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
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_team"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["nc_team"].ToString());
                    }

                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = "";
                    sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["team_approvedby"].ToString());
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["team_notifiedto"].ToString()) != "")
                    {
                        sCCEmailIds = sCCEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsNCList.Tables[0].Rows[0]["team_notifiedto"].ToString());
                    }
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');
                    
                   aAttachment = HttpContext.Current.Server.MapPath(dsNCList.Tables[0].Rows[0]["nc_upload"].ToString());


                    sHeader = "<tr><td colspan=3><b>NC Number:<b></td> <td colspan=3>"
                        + dsNCList.Tables[0].Rows[0]["nc_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Category:<b></td> <td colspan=3>" + objGlobaldata.GetDropdownitemById(dsNCList.Tables[0].Rows[0]["nc_category"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Description:<b></td> <td colspan=3>" + dsNCList.Tables[0].Rows[0]["nc_description"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Detected Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["nc_detected_date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Activity:<b></td> <td colspan=3>" + (dsNCList.Tables[0].Rows[0]["nc_activity"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>NC Risk:<b></td> <td colspan=3>" + (dsNCList.Tables[0].Rows[0]["nc_risk"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Target date to complete the Root Cause Analysis:<b></td> <td colspan=3>" + Convert.ToDateTime(dsNCList.Tables[0].Rows[0]["team_targetdate"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "NC Details");
                    sContent = sContent.Replace("{content}", sHeader + sInformation);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobaldata.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCEmail: " + ex.ToString());
            }
            return false;
        }

        //RCA
        internal bool FunUpdateRCA(NCModels objModels, NCModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_nc set  rca_technique='" + objModels.rca_technique + "', "
                    + "rca_details  ='" + objModels.rca_details + "', rca_upload='" + objModels.rca_upload + "', rca_action='" + objModels.rca_action
                    + "', rca_justify='" + objModels.rca_justify + "', rca_reportedby='" + objModels.rca_reportedby + "', rca_notifiedto='" + objModels.rca_notifiedto + "'";

                if (objModels.rca_startedon != null && objModels.rca_startedon > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_startedon ='" + objModels.rca_startedon.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.rca_reporteddate != null && objModels.rca_reporteddate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", rca_reporteddate ='" + objModels.rca_reporteddate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {                   
                  return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateRCA: " + ex.ToString());
            }
            return false;
        }

        //CA
        internal bool FunUpdateCA(NCModels objModels, NCModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_nc set  ca_proposed_by='" + objModels.ca_proposed_by + "', "
                    + "ca_notifiedto='" + objModels.ca_notifiedto  + "'";

                if (objModels.ca_verfiry_duedate != null && objModels.ca_verfiry_duedate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_verfiry_duedate ='" + objModels.ca_verfiry_duedate.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.ca_notifed_date != null && objModels.ca_notifed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ca_notifed_date ='" + objModels.ca_notifed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.lstNC.Count) > 0)
                    {
                        objList.lstNC[0].id_nc = id_nc.ToString();
                        FunAddCAList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateCA: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCAList(NCModelsList objCAList)
        {
            try
            {
                //string sSqlstmt = "delete from t_nc_corrective_action where id_nc='" + objCAList.lstNC[0].id_nc + "'; ";
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.lstNC.Count; i++)
                {
                    string sid_nc_corrective_action = "null";
                    string sca_target_date = "";

                    if (objCAList.lstNC[i].id_nc_corrective_action != null && objCAList.lstNC[i].id_nc_corrective_action != "")
                    {
                        sid_nc_corrective_action = objCAList.lstNC[i].id_nc_corrective_action;
                    }

                    if (objCAList.lstNC[i].ca_target_date != null && objCAList.lstNC[i].ca_target_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sca_target_date =  objCAList.lstNC[i].ca_target_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_nc_corrective_action (id_nc_corrective_action,id_nc,ca_div,ca_loc,ca_dept,ca_rootcause,ca_ca,ca_resource,ca_resp_person,ca_target_date)"
                    + " values(" + sid_nc_corrective_action + "," + objCAList.lstNC[0].id_nc + ",'" + objCAList.lstNC[i].ca_div + "','" + objCAList.lstNC[i].ca_loc + "','" + objCAList.lstNC[i].ca_dept
                    + "','" + objCAList.lstNC[i].ca_rootcause + "','" + objCAList.lstNC[i].ca_ca + "','" + objCAList.lstNC[i].ca_resource + "','" + objCAList.lstNC[i].ca_resp_person + "','" + sca_target_date + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + "id_nc_corrective_action= values(id_nc_corrective_action),id_nc= values(id_nc), ca_div= values(ca_div), ca_loc = values(ca_loc), ca_dept = values(ca_dept), ca_rootcause = values(ca_rootcause)," +
                    " ca_ca = values(ca_ca), ca_resource = values(ca_resource), ca_resp_person = values(ca_resp_person), ca_target_date = values(ca_target_date); ";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddCAList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCAList(string sid_nc)
        {
            try
            {
                string sSqlstmt = "delete from t_nc_corrective_action where id_nc='" + sid_nc + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateCAList: " + ex.ToString());
            }
            return false;
        }

        //Verification
        internal bool FunUpdateVerification(NCModels objModels, NCModelsList objList)
        {
            try
            {

                string sSqlstmt = "update t_nc set  v_implement='" + objModels.v_implement + "', v_implement_explain='" + objModels.v_implement_explain 
                    + "', v_rca='" + objModels.v_rca + "', v_rca_explain='" + objModels.v_rca_explain + "', v_discrepancies='" + objModels.v_discrepancies + "', v_discrep_explain='" + objModels.v_discrep_explain
                    + "', v_upload='" + objModels.v_upload + "', v_status='" + objModels.v_status + "', v_verifiedto='" + objModels.v_verifiedto + "', v_notifiedto='" + objModels.v_notifiedto + "'";

                if (objModels.v_closed_date != null && objModels.v_closed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_closed_date ='" + objModels.v_closed_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModels.v_verified_date != null && objModels.v_verified_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", v_verified_date ='" + objModels.v_verified_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_nc='" + objModels.id_nc + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objList.lstNC.Count) > 0)
                    {
                        objList.lstNC[0].id_nc = id_nc.ToString();
                        FunAddVerificationList(objList);
                    }
                    else
                    {
                        FunUpdateCAList(id_nc);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateVerification: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddVerificationList(NCModelsList objCAList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objCAList.lstNC.Count; i++)
                {
                    string sid_nc_corrective_action = "";
                    
                    if (objCAList.lstNC[i].id_nc_corrective_action != null && objCAList.lstNC[i].id_nc_corrective_action != "")
                    {
                        sid_nc_corrective_action = objCAList.lstNC[i].id_nc_corrective_action;
                    }
                    sSqlstmt = sSqlstmt + " update t_nc_corrective_action set implement_status = '"+ objCAList.lstNC[i].implement_status 
                        + "', ca_effective = '"+ objCAList.lstNC[i].ca_effective + "', reason ='"+ objCAList.lstNC[i].reason + "' where id_nc_corrective_action = '"+ sid_nc_corrective_action + "';";
                }
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddVerificationList: " + ex.ToString());
            }
            return false;
        }


        //Delete
        internal bool FunDeleteNCDoc(string sid_nc)
        {
            try
            {
                string sSqlstmt = "update t_nc set Active=0 where id_nc='" + sid_nc + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteNCDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCADoc(string sid_nc_corrective_action)
        {
            try
            {
                string sSqlstmt = "update t_nc_corrective_action set ca_active=0 where id_nc_corrective_action='" + sid_nc_corrective_action + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteCADoc: " + ex.ToString());
            }
            return false;
        }

        //internal bool FunNCApproveOrReject(string sid_nc, int sStatus)
        //{
        //    try
        //    {
        //        string snc_issueddate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
        //        string user = "";

        //        user = objGlobaldata.GetCurrentUserSession().empid;

        //        if (sStatus == 1)
        //        {
        //            string sSqlstmt1 = "update t_nc set nc_issuedtocount=nc_issuedtocount-1 where id_nc='" + sid_nc + "'";
        //            if (objGlobaldata.ExecuteQuery(sSqlstmt1))
        //            {
        //                string sSqlstmt = "Select nc_issuedtocount from t_nc where id_nc='" + sid_nc + "'";
        //                DataSet dsManagementChange = objGlobaldata.Getdetails(sSqlstmt);
        //                if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
        //                {
        //                    if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["nc_issuedtocount"].ToString()) == 0)
        //                    {
        //                        string sSSqlstmt = "update t_nc set nc_issuedto_status ='1', nc_issueddate='" + snc_issueddate + "' where id_nc='" + sid_nc + "'";
        //                        if (objGlobaldata.ExecuteQuery(sSSqlstmt))
        //                        {
        //                            string Sql1 = "update t_nc set nc_issuers= concat(nc_issuers,',','" + user + "') where id_nc='" + sid_nc + "'";
        //                            objGlobaldata.ExecuteQuery(Sql1);
        //                            SendNCApprovalmail(sid_nc,1);
        //                            return true;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        string Sql1 = "update t_nc set nc_issuers= concat(nc_issuers,',','" + user + "') where id_nc='" + sid_nc + "'";
        //                        return objGlobaldata.ExecuteQuery(Sql1);
        //                    }

        //                }
        //            }
        //        }
        //        else
        //        {
        //            string Sql1 = "update t_nc set nc_issuedto_status='" + sStatus + "',nc_issuer_rejector= concat(nc_issuer_rejector,',','" + user + "') where id_nc='" + sid_nc + "'";
        //            objGlobaldata.ExecuteQuery(Sql1);                    
        //            SendNCApprovalmail(sid_nc,2);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in FunNCApproveOrReject: " + ex.ToString());
        //    }
        //    return false;
        //}

        internal bool FunNCApproveOrReject(NCModels objModel)
        {
            try
            {
                string snc_issueddate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";
                string sid_nc = objModel.id_nc;
                user = objGlobaldata.GetCurrentUserSession().empid;

                string Statustmt = "update t_nc_status set nc_stauts='"+ objModel.nc_stauts+ "', nc_reject_comment='"+ objModel.nc_reject_comment + "', nc_approve_reject_date='"
                    + snc_issueddate + "', nc_reject_upload='" + objModel.nc_reject_upload + "' where id_nc='"+ sid_nc + "' and nc_issuedto= '"+ user + "'";
                objGlobaldata.ExecuteQuery(Statustmt);

                if (objModel.nc_stauts == "1")
                {  
                    string sSqlstmt1 = "update t_nc set nc_issuedtocount=nc_issuedtocount-1,nc_initial_status=1 where id_nc='" + sid_nc + "'";
                    if (objGlobaldata.ExecuteQuery(sSqlstmt1))
                    {
                        string sSqlstmt = "Select nc_issuedtocount from t_nc where id_nc='" + sid_nc + "'";
                        DataSet dsManagementChange = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsManagementChange.Tables.Count > 0 && dsManagementChange.Tables[0].Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dsManagementChange.Tables[0].Rows[0]["nc_issuedtocount"].ToString()) == 0)
                            {
                                string sSSqlstmt = "update t_nc set nc_issuedto_status ='1', nc_issueddate='" + snc_issueddate + "' where id_nc='" + sid_nc + "'";
                                if (objGlobaldata.ExecuteQuery(sSSqlstmt))
                                {
                                    string Sql1 = "update t_nc set nc_issuers= concat(nc_issuers,',','" + user + "') where id_nc='" + sid_nc + "'";
                                    objGlobaldata.ExecuteQuery(Sql1);                                   
                                }
                            }
                            else
                            {
                                string Sql1 = "update t_nc set nc_issuers= concat(nc_issuers,',','" + user + "') where id_nc='" + sid_nc + "'";
                                objGlobaldata.ExecuteQuery(Sql1);
                            }
                            SendNCApprovalmail(sid_nc, 1);
                            return true;
                        }
                    }
                }
                else
                {
                    //string Sql1 = "update t_nc set nc_issuedto_status='" + objModel.nc_stauts + "',nc_issuer_rejector= concat(nc_issuer_rejector,',','" + user + "') where id_nc='" + sid_nc + "'";
                    string Sql1 = "update t_nc set nc_issuer_rejector= concat(nc_issuer_rejector,',','" + user + "') where id_nc='" + sid_nc + "'";
                    objGlobaldata.ExecuteQuery(Sql1);
                    SendNCApprovalmail(sid_nc, 2);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunNCApproveOrReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendNCApprovalmail(string sid_nc,int status)
        {
            try
            {
                DataSet dsDocument = objGlobaldata.Getdetails("select id_nc,nc_no, nc_reported_date, nc_detected_date,nc_category,nc_description,nc_reportedby,nc_notifiedto,nc_issueto from t_nc where id_nc='" + sid_nc + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    string sEmailid = objGlobaldata.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["nc_reportedby"].ToString());
                    sEmailid = Regex.Replace(sEmailid, ",+", ",");
                    sEmailid = sEmailid.Trim();
                    sEmailid = sEmailid.TrimEnd(',');
                    sEmailid = sEmailid.TrimStart(',');
                    string sExtraMsg = "";
                    string sEmailTemplate = "";
                    string user= objGlobaldata.GetEmpHrNameById(objGlobaldata.GetCurrentUserSession().empid);
                    if (status == 1)
                    {
                        sExtraMsg = "Nonconformance & Corrective Action has been approved by "+ user + ", NC Number: " + dsDocument.Tables[0].Rows[0]["nc_no"].ToString();
                        sEmailTemplate= "NCApprove";
                    }
                    else
                    {
                        sExtraMsg = "Nonconformance & Corrective Action has been rejected by "+ user + ", NC Number: " + dsDocument.Tables[0].Rows[0]["nc_no"].ToString();
                        sEmailTemplate = "NCReject";
                    }                   

                    string sEmailCCList = objGlobaldata.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["nc_notifiedto"].ToString());
                    Dictionary<string, string> dicEmailContent = objGlobaldata.FormEmailBody(
                    objGlobaldata.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["nc_reportedby"].ToString()), sEmailTemplate, sExtraMsg);
                                                       
                    return objGlobaldata.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList,"");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SendNCApprovalmail: " + ex.ToString());
            }
            return false;
        }
    }   

    public class NCModelsList
    {
        public List<NCModels> lstNC { get; set; }
    }

    public class DropdownNCItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownNCList
    {
        public List<DropdownNCItems> NcDropdownList { get; set; }
    }
}