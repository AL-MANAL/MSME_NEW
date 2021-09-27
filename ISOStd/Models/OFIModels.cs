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
    public class OFIModels
    {
        clsGlobal objGlobaldata = new clsGlobal();

        //t_ofi
        [Display(Name = "Id")]
        public string id_ofi { get; set; }

        [Display(Name = "IO Number")]
        public string ofi_no { get; set; }

        [Display(Name = "Risk Number")]
        public string risk_no { get; set; }
        public string risk_nodesc { get; set; }

        [Display(Name = "IO Reported by")]
        public string reportedby { get; set; }

        [Display(Name = "IO Reported Date")]
        public DateTime reported_date { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "IO identified in")]
        public string identified_in { get; set; }

        [Display(Name = "Identified Improvement Opportunity")]
        public string opportunity { get; set; }

        [Display(Name = "Expected improvement")]
        public string improvement { get; set; }

        [Display(Name = "Target date to achieve the planned improvement")]
        public DateTime target_date { get; set; }

        [Display(Name = "To be approved by")]
        public string approvedby { get; set; }

        [Display(Name = "Approved Status")]
        public string approved_status { get; set; }

        [Display(Name = "Apprvoed Date")]
        public DateTime approved_date { get; set; }

        [Display(Name = "Actions Proposed by")]
        public string action_proposedby { get; set; }

        [Display(Name = "Realization To be Approved by")]
        public string realization_approved_by { get; set; }

        [Display(Name = "Realization Approved Status")]
        public string realization_approved_status { get; set; }

        [Display(Name = "Realization Approved Date")]
        public DateTime realization_apporved_date { get; set; }

        [Display(Name = "Any document to be updated?")]
        public string updated { get; set; }

        [Display(Name = "Updated by")]
        public string updatedby { get; set; }

        [Display(Name = "Is expected improvement achieved?")]
        public string improvement_achieve { get; set; }

        [Display(Name = "IO Status to be checked by")]
        public string checkedby { get; set; }
        public string checkedbyId { get; set; }

        [Display(Name = "IO Status ")]
        public string ofi_status { get; set; }

        [Display(Name = "Remarks")]
        public string remark { get; set; }

        [Display(Name = "Logged By")]
        public string loggedby { get; set; }

        //t_ofi_action
        [Display(Name = "Id")]
        public string id_ofi_action { get; set; }

        [Display(Name = "Action Details")]
        public string action_details { get; set; }

        [Display(Name = "Area to be improved")]
        public string area_improved { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string resp { get; set; }

        [Display(Name = "Resource Required")]
        public string resource { get; set; }

        [Display(Name = "Target Date")]
        public DateTime action_target_date { get; set; }

        //t_ofi_doc

        [Display(Name = "Id")]
        public string id_ofi_doc { get; set; }

        [Display(Name = "Document Type")]
        public string doc_type { get; set; }

        [Display(Name = "Document Reference")]
        public string doc_ref { get; set; }

        [Display(Name = "Document Name")]
        public string doc_name { get; set; }

        //t_ofi_improvement
        [Display(Name = "Id")]
        public string id_ofi_improvement { get; set; }

        [Display(Name = "Improvement Details")]
        public string improve_details { get; set; }

        [Display(Name = "Improvement Achieved In")]
        public string improve_achievedin { get; set; }

        [Display(Name = "Measurable Value")]
        public string improve_measurable { get; set; }

        [Display(Name = "Document Upload")]
        public string imporve_upload { get; set; }

        public string checkedbystatus { get; set; }
        public string ofi_statusId { get; set; }

        [Display(Name = "Person responsible for action")]
        public string pers_resp { get; set; }

        [Display(Name = "Comments")]
        public string approver_comments { get; set; }

        [Display(Name = "Document(s)")]
        public string approver_upload { get; set; }

        [Display(Name = "Comments")]
        public string realization_comments { get; set; }

        [Display(Name = "Document(s)")]
        public string realization_upload { get; set; }

        //[Required(ErrorMessage = "Please enter action detail")]
        [Range(1, 25, ErrorMessage = "Please enter action detail")]
        public int cntaction { get; set; }

        public MultiSelectList GetOFIExpectedImporvementList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='OFI Expected Improvement' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIExpectedImporvementList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        public string GetOFIExpectedImporvementById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='OFI Expected Improvement' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIExpectedImporvementById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetOFIIdentifiedList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='OFI Identified In' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIIdentifiedList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        public string GetOFIIdentifiedById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='OFI Identified In' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIIdentifiedById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetOFIAreaImprovedList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='OFI Improvement Area' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIAreaImprovedList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        public string GetOFIAreaImprovedById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='OFI Improvement Area' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIAreaImprovedById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetOFIStatusList()
        {
            DropdownNCList NcdropList = new DropdownNCList();
            NcdropList.NcDropdownList = new List<DropdownNCItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='OFI Status' order by item_desc asc";
                DataSet dsNc = objGlobaldata.Getdetails(sSqlstmt);
                if (dsNc.Tables.Count > 0 && dsNc.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsNc.Tables[0].Rows.Count; i++)
                    {
                        DropdownNCItems ncmodel = new DropdownNCItems()
                        {
                            Id = dsNc.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsNc.Tables[0].Rows[i]["Name"].ToString()
                        };
                        NcdropList.NcDropdownList.Add(ncmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIStatusList: " + ex.ToString());
            }
            return new MultiSelectList(NcdropList.NcDropdownList, "Id", "Name");
        }

        public string GetOFIStatusById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='OFI Status' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIStatusById: " + ex.ToString());
            }
            return "";
        }

        internal string GetOFIStatusIdByName(string sitem_desc)//checked
        {
            try
            {
                DataSet dsEmp = objGlobaldata.Getdetails("select item_id as Id from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='OFI Status' and item_desc='" + sitem_desc + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GetOFIStatusIdByName: " + ex.ToString());
            }
            return "";
        }

        
        //OFI
        internal bool FunAddReportOFI(OFIModels objModels)//checked
        {
            try
            {
                string sFields = "", sFieldValue = "";

                string sSqlstmt = "insert into t_ofi ( ofi_no, risk_no, reportedby, division, department, location,identified_in,opportunity,improvement,approvedby,ofi_status,loggedby,pers_resp";

                if (objModels.approvedby == null || objModels.approvedby == "")
                {
                    sFields = sFields + ", approved_status";
                    sFieldValue = sFieldValue + ", 'Approved'";
                }
                if (objModels.reported_date != null && objModels.reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", reported_date";
                    sFieldValue = sFieldValue + ", '" + objModels.reported_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objModels.target_date != null && objModels.target_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", target_date";
                    sFieldValue = sFieldValue + ", '" + objModels.target_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields + ") values('" + objModels.ofi_no + "','" + objModels.risk_no
                + "','" + objModels.reportedby + "','" + objModels.division + "','" + objModels.department + "','" + objModels.location
                + "','" + objModels.identified_in + "','" + objModels.opportunity + "','" + objModels.improvement + "','" + objModels.approvedby
                + "','" + objModels.ofi_status + "','" + objGlobaldata.GetCurrentUserSession().empid + "','" + objModels.pers_resp + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int iid_ofi = 0;

                if (int.TryParse(objGlobaldata.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_ofi))
                {
                    if (iid_ofi > 0)
                    {
                        SendOFIReportEmail(iid_ofi, "Opportunity for Improvement (OFI)");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddReportOFI: " + ex.ToString());
            }
            return false;
        }

        internal bool SendOFIReportEmail(int iid_ofi, string sMessage = "")//checked
        {
            try
            {
                string sType = "email";

                string sSqlstmt = "select id_ofi, ofi_no, risk_no, reportedby, reported_date, division, department, location, identified_in,opportunity, improvement,"
                    + "target_date, approvedby,loggedby from t_ofi where id_ofi ='" + iid_ofi + "'";

                DataSet dsOFIList = objGlobaldata.Getdetails(sSqlstmt);
                OFIModels objType = new OFIModels();

                if (dsOFIList.Tables.Count > 0 && dsOFIList.Tables[0].Rows.Count > 0)
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
                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["approvedby"].ToString()) != "")
                    {
                        sToEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["approvedby"].ToString()) + ",";
                    }
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    string sCCEmailIds = objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["loggedby"].ToString());

                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["reportedby"].ToString()) != "")
                    {
                        sCCEmailIds = sCCEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["reportedby"].ToString()) + ",";
                    }
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    if (objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["approvedby"].ToString()) != "")
                    {
                        sCCEmailIds = sCCEmailIds + "," + objGlobaldata.GetMultiHrEmpEmailIdById(dsOFIList.Tables[0].Rows[0]["approvedby"].ToString()) + ",";
                    }
                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');
                    // aAttachment = HttpContext.Current.Server.MapPath(dsOFIList.Tables[0].Rows[0]["nc_upload"].ToString());

                    sHeader = "<tr><td colspan=3><b>OFI Number:<b></td> <td colspan=3>"
                        + dsOFIList.Tables[0].Rows[0]["ofi_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Risk Number:<b></td> <td colspan=3>" + objGlobaldata.GetRiskRefByRiskId(dsOFIList.Tables[0].Rows[0]["risk_no"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Risk Description:<b></td> <td colspan=3>" + objGlobaldata.GetRiskDescriptionByRiskId(dsOFIList.Tables[0].Rows[0]["risk_no"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Reported Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsOFIList.Tables[0].Rows[0]["reported_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Reported by:<b></td> <td colspan=3>" + objGlobaldata.GetMultiHrEmpNameById(dsOFIList.Tables[0].Rows[0]["reportedby"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Division:<b></td> <td colspan=3>" + objGlobaldata.GetMultiCompanyBranchNameById(dsOFIList.Tables[0].Rows[0]["division"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobaldata.GetMultiDeptNameById(dsOFIList.Tables[0].Rows[0]["department"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Location:<b></td> <td colspan=3>" + objGlobaldata.GetDivisionLocationById(dsOFIList.Tables[0].Rows[0]["location"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>OFI identified in:<b></td> <td colspan=3>" + objType.GetOFIIdentifiedById(dsOFIList.Tables[0].Rows[0]["identified_in"].ToString()) + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Identified Improvement Opportunity:<b></td> <td colspan=3>" + (dsOFIList.Tables[0].Rows[0]["opportunity"].ToString()) + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Expected improvement:<b></td> <td colspan=3>" + (dsOFIList.Tables[0].Rows[0]["improvement"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Target Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsOFIList.Tables[0].Rows[0]["target_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>";

                    //if (File.Exists(aAttachment))
                    //{
                    //    sHeader = sHeader + "<tr><td colspan=3><b>Document Uploaded:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    //}

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "OFI Details");
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
                objGlobaldata.AddFunctionalLog("Exception in SendOFIReportEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateReportOFI(OFIModels objModels)//checked
        {
            try
            {
                string sSqlstmt = "update t_ofi set reportedby='" + objModels.reportedby + "', " + "division='" + objModels.division + "', department='" + objModels.department
                     + "', location='" + objModels.location + "', identified_in='" + objModels.identified_in + "', opportunity='" + objModels.opportunity + "', improvement='" + objModels.improvement
                    + "', approvedby='" + objModels.approvedby + "', pers_resp='" + objModels.pers_resp + "'";

                if (objModels.reported_date != null && objModels.reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", reported_date ='" + objModels.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objModels.target_date != null && objModels.target_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", target_date ='" + objModels.target_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_ofi='" + objModels.id_ofi + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateReportOFI: " + ex.ToString());
            }
            return false;
        }


        //RealizationOFI

        internal bool FunAddRealizationOFI(OFIModels objModels, OFIModelsList objOFIList)
        {
            try
            {
                string sSqlstmt = "update t_ofi set  action_proposedby='" + objModels.action_proposedby + "', "
                    + "realization_approved_by='" + objModels.realization_approved_by + "'";

                if (objModels.realization_approved_by == null || objModels.realization_approved_by == "")
                {
                    sSqlstmt = sSqlstmt + ", realization_approved_status ='Approved'";
                }
                //if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                //}
                sSqlstmt = sSqlstmt + " where id_ofi='" + objModels.id_ofi + "'";
                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objOFIList.OFIList.Count) > 0)
                    {
                        objOFIList.OFIList[0].id_ofi = id_ofi.ToString();
                        FunAddRealizationList(objOFIList);
                    }
                    else
                    {
                        FunUpdateOFIList(id_ofi);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddRealizationOFI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddRealizationList(OFIModelsList objOFIList)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_action where id_ofi='" + objOFIList.OFIList[0].id_ofi + "'; ";

                for (int i = 0; i < objOFIList.OFIList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_ofi_action(id_ofi,action_details,area_improved,resp,resource";

                    string sFieldValue = "", sFields = "";
                    if (objOFIList.OFIList[i].action_target_date != null && objOFIList.OFIList[i].action_target_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sFields = sFields + ", action_target_date";
                        sFieldValue = sFieldValue + ", '" + objOFIList.OFIList[i].action_target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objOFIList.OFIList[0].id_ofi + "', '" + objOFIList.OFIList[i].action_details + "', '" + objOFIList.OFIList[i].area_improved + "', '" + objOFIList.OFIList[i].resp + "', '" + objOFIList.OFIList[i].resource + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddRealizationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateOFIList(string sid_ofi)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_action where id_ofi='" + sid_ofi + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateOFIList: " + ex.ToString());
            }
            return false;
        }

        //Improvement Achieved

        internal bool FunAddImprovementOFI(OFIModels objModels, OFIModelsList objOFIList, OFIModelsList objOFIDOCList)
        {
            try
            {
                string sSqlstmt = "update t_ofi set  updated='" + objModels.updated + "', updatedby='" + objModels.updatedby + "', improvement_achieve='" + objModels.improvement_achieve +
                    "', checkedby='" + objModels.checkedby + "' ,checkedbystatus='0' where id_ofi='" + objModels.id_ofi + "'";

                //if (objModels.disp_notifeddate != null && objModels.disp_notifeddate > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", disp_notifeddate ='" + objModels.disp_notifeddate.ToString("yyyy/MM/dd") + "'";
                //}

                if (objGlobaldata.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objOFIList.OFIList.Count) > 0)
                    {
                        objOFIList.OFIList[0].id_ofi = id_ofi.ToString();
                        FunAddImprovementList(objOFIList);
                    }
                    else
                    {
                        FunUpdateImprovementList(id_ofi);
                    }
                    if (Convert.ToInt32(objOFIDOCList.OFIList.Count) > 0)
                    {
                        objOFIDOCList.OFIList[0].id_ofi = id_ofi.ToString();
                        FunAddImprovementDocList(objOFIDOCList);
                    }
                    else
                    {
                        FunUpdateImprovementDocList(id_ofi);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddImprovementOFI: " + ex.ToString());
            }
            return false;
        }

        //t_ofi_improvement
        internal bool FunAddImprovementList(OFIModelsList objOFIList)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_improvement where id_ofi='" + objOFIList.OFIList[0].id_ofi + "'; ";

                for (int i = 0; i < objOFIList.OFIList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_ofi_improvement(id_ofi,improve_details,improve_achievedin,improve_measurable,imporve_upload)"
                           + "values('" + objOFIList.OFIList[0].id_ofi + "', '" + objOFIList.OFIList[i].improve_details + "', '" + objOFIList.OFIList[i].improve_achievedin + "', '" + objOFIList.OFIList[i].improve_measurable + "', '" + objOFIList.OFIList[i].imporve_upload + "');";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddImprovementList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateImprovementList(string sid_ofi)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_improvement where id_ofi='" + sid_ofi + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateImprovementList: " + ex.ToString());
            }
            return false;
        }

        //t_ofi_doc
        internal bool FunAddImprovementDocList(OFIModelsList objOFIList)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_doc where id_ofi='" + objOFIList.OFIList[0].id_ofi + "'; ";

                for (int i = 0; i < objOFIList.OFIList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_ofi_doc(id_ofi,doc_type,doc_ref,doc_name) values('"
                        + objOFIList.OFIList[0].id_ofi + "', '" + objOFIList.OFIList[i].doc_type + "', '" + objOFIList.OFIList[i].doc_ref + "', '" + objOFIList.OFIList[i].doc_name + "' );";
                }

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunAddImprovementList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateImprovementDocList(string sid_ofi)
        {
            try
            {
                string sSqlstmt = "delete from t_ofi_doc where id_ofi='" + sid_ofi + "'; ";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateImprovementDocList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunOFIInitialApproval(string sid_ofi, string sStatus, OFIModels objModel)
        {
            try
            {
                OFIModels ofModl = new OFIModels();

                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sSql = "Select approved_status,realization_approved_status from t_ofi where id_ofi = '" + sid_ofi + "' and active=1";

                DataSet dsInitial = objGlobaldata.Getdetails(sSql);
                if (dsInitial.Tables.Count > 0 && dsInitial.Tables[0].Rows.Count > 0)
                {
                    ofModl = new OFIModels
                    {
                        approved_status = dsInitial.Tables[0].Rows[0]["approved_status"].ToString(),
                        realization_approved_status = dsInitial.Tables[0].Rows[0]["realization_approved_status"].ToString()
                    };
                }
                if (ofModl.approved_status == "0")
                {
                    string sSqlstmt = "update t_ofi set approved_status='" + sStatus + "', approved_date='" + sApprovedDate + "', approver_comments='" + approver_comments + "', approver_upload='" + approver_upload + "' where id_ofi='" + sid_ofi + "'";
                    return objGlobaldata.ExecuteQuery(sSqlstmt);
                }
                else if (ofModl.realization_approved_status == "0")
                {
                    string sSqlstmt = "update t_ofi set realization_approved_status='" + sStatus + "', realization_apporved_date='" + sApprovedDate + "', realization_comments='" + realization_comments + "', realization_upload='" + realization_upload + "' where id_ofi='" + sid_ofi + "'";
                    return objGlobaldata.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunOFIInitialApproval: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddOFICheckedbystatus(OFIModels objModels)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd");

                string sSqlstmt = "update t_ofi set ofi_status='" + objModels.ofi_status + "', remark='" + objModels.remark + "', checkedbystatus='" + objModels.checkedbystatus
                    + "', checkedbydate='" + sApprovedDate + "' where id_ofi='" + objModels.id_ofi + "'";
                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunUpdateReportOFI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteOFIDoc(string sid_ofi)
        {
            try
            {
                string sSqlstmt = "update t_ofi set Active=0 where id_ofi='" + sid_ofi + "'";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunDeleteOFIDoc: " + ex.ToString());
            }
            return false;
        }
    }

    public class OFIModelsList
    {
        public List<OFIModels> OFIList { get; set; }
    }
}