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
    public class PortalModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_portal_master { get; set; }

        [Display(Name = "Sub-CR No.")]
        public string subcr_no { get; set; }

        [Display(Name = "Commercial Name")]
        public string commercial_name { get; set; }

        [Display(Name = "Business Related Activities")]
        public string activities { get; set; }

        [Display(Name = "Issue Date")]
        public DateTime issue_date { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime expiry_date { get; set; }

        [Display(Name = "Issue Authority")]
        public string issue_authority { get; set; }

        [Display(Name = "Region")]
        public string region { get; set; }

        [Display(Name = "Manager")]
        public string master_manager { get; set; }

        [Display(Name = "Person nominated by Manager")]
        public string recommed_by_manager { get; set; }

        [Display(Name = "Division")]
        public string master_div { get; set; }

        [Display(Name = "Department")]
        public string master_dept { get; set; }

        [Display(Name = "Attach Sub-CR copy")]
        public string upload { get; set; }

        [Display(Name = "ISIC Code")]
        public string isc_code { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        //t_portal_authorization
        [Display(Name = "Id")]
        public string id_authorization { get; set; }

        [Display(Name = "Access Authorization Number")]
        public string access_no { get; set; }

        [Display(Name = "Request Date")]
        public DateTime request_date { get; set; }

        [Display(Name = "Access Effective Date")]
        public DateTime access_date { get; set; }

        [Display(Name = "Valid Till")]
        public DateTime valid_date { get; set; }

        [Display(Name = "Portal Category")]
        public string portal_category { get; set; }
        public string portal_categoryName { get; set; }

        [Display(Name = "Nominated Employee")]
        public string nominated_emp { get; set; }

        [Display(Name = "Purpose of Access")]
        public string access_purpose { get; set;}

        [Display(Name = "Justification")]
        public string justification { get; set; }

        [Display(Name = "Requested/Prepared by ")]
        public string requested_by { get; set; }

        [Display(Name = "Recommended by divisional manager")]
        public string recommended_by { get; set; }

        //[Display(Name = "Divisional CEO")]
        [Display(Name = "Divisional CEO approver")]
        public string approve_ceo { get; set; }

        [Display(Name = "Ex Com President / VP")]
        public string approve_vp { get; set; }

        [Display(Name = "Final Appover (Dy Chairman)")]
        public string approve_chairman { get; set; }

        [Display(Name = "Status")]
        public string apporved_status { get; set; }
        public string apporved_statusId { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Portal Name")]
        public string portal_name { get; set; }

        [Display(Name = "Ministry Name")]
        public string ministry_name { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime approve_recommed_date { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        public string ministry_url { get; set; }
        public DateTime approve_ceo_date { get; set; }
        public DateTime approve_vp_date { get; set; }
        public DateTime approve_chairman_date { get; set; }
        public DateTime rejected_date { get; set; }

        public string recommend_comments { get; set; }
        public string ceo_comments  { get;set;}
        public string vp_comments { get; set; }
        public string chairman_comments { get; set; }
        public string reject_comments { get; set; }


        internal bool FunAddSubCRMaster(PortalModels objPortal)
        {
            try
            {
                // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_portal_master (subcr_no,commercial_name, activities,issue_authority," +
                    " region,master_manager,master_div,master_dept,upload,isc_code,description,logged_by,recommed_by_manager";
                
                string sFields = "", sFieldValue = "";

                if (objPortal.issue_date != null && objPortal.issue_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", issue_date";
                    sFieldValue = sFieldValue + ", '" + objPortal.issue_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
                if (objPortal.expiry_date != null && objPortal.expiry_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", expiry_date";
                    sFieldValue = sFieldValue + ", '" + objPortal.expiry_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPortal.subcr_no + "','" + objPortal.commercial_name + "','" + objPortal.activities 
                    + "','" + objPortal.issue_authority + "','" + objPortal.region + "','" + objPortal.master_manager 
                    + "','" + objPortal.master_div + "','" + objPortal.master_dept + "','" + objPortal.upload + "','"
                    + objPortal.isc_code + "','" + objPortal.description + "','" + objGlobalData.GetCurrentUserSession().empid
                    + "','" + objPortal.recommed_by_manager+ "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSubCRMaster: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSubCRMaster(PortalModels objPortal)
        {
            try
            {
                string sSqlstmt = "update t_portal_master set subcr_no ='" + objPortal.subcr_no + "', commercial_name='" + objPortal.commercial_name + "', "
                   + "activities='" + objPortal.activities + "',issue_authority='" + objPortal.issue_authority + "',region='" + objPortal.region + "',master_manager='" + objPortal.master_manager
                   + "',master_div='" + objPortal.master_div + "',master_dept='" + objPortal.master_dept + "',upload='" + objPortal.upload
                   + "',isc_code='" + objPortal.isc_code + "',description='" + objPortal.description + "',recommed_by_manager='" + objPortal.recommed_by_manager + "'";

                if (objPortal.issue_date != null && objPortal.issue_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", issue_date='" + objPortal.issue_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objPortal.expiry_date != null && objPortal.expiry_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", expiry_date='" + objPortal.expiry_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_portal_master='" + objPortal.id_portal_master + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSubCRMaster: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeleteSubCRMaster(string sid_portal_master)
        {
            try
            {
                string sSqlstmt = "update t_portal_master set Active=0 where id_portal_master='" + sid_portal_master + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSubCRMaster: " + ex.ToString());
            }
            return false;
        }

        //Portal Authorization

        public string GetSUBCRNoById(string sid_portal_master)
        {
            try
            {
                if (sid_portal_master != "")
                {
                    string sSsqlstmt = "select subcr_no as Name from t_portal_master where id_portal_master='" + sid_portal_master + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSUBCRNoById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetSUBCRPortalMasterList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_portal_master as Id, subcr_no as Name from t_portal_master where active = 1 order by id_portal_master desc";
                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSUBCRPortalMasterList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");

        }

        public string GetCommercialNameById(string sid_portal_master)
        {
            try
            {
                if (sid_portal_master != "")
                {
                    string sSsqlstmt = "select commercial_name as Name from t_portal_master where id_portal_master='" + sid_portal_master + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCommercialNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetActivityBySUBCRId(string sid_portal_master)
        {
            try
            {
                if (sid_portal_master != "")
                {
                    string sSsqlstmt = "select activities as Name from t_portal_master where id_portal_master='" + sid_portal_master + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetActivityBySUBCRId: " + ex.ToString());
            }
            return "";
        }

        internal bool FunAddPortalAuth(PortalModels objPortal)
        {
            try
            {
                // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_portal_authorization (portal_category, id_portal_master,upload, " +
                    "nominated_emp,justification,requested_by,recommended_by,approve_vp,approve_ceo,approve_chairman,division," +
                    "logged_by,access_purpose,portal_name,department";

                string sFields = "", sFieldValue = "";

                if (objPortal.request_date != null && objPortal.request_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", request_date";
                    sFieldValue = sFieldValue + ", '" + objPortal.request_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objPortal.access_date != null && objPortal.access_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", access_date";
                    sFieldValue = sFieldValue + ", '" + objPortal.access_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objPortal.valid_date != null && objPortal.valid_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", valid_date";
                    sFieldValue = sFieldValue + ", '" + objPortal.valid_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPortal.portal_category + "','" + objPortal.id_portal_master
                    + "','" + objPortal.upload + "','" + objPortal.nominated_emp + "','" + objPortal.justification + "','" + objPortal.requested_by
                    + "','" + objPortal.recommended_by + "','" + objPortal.approve_vp + "','" + objPortal.approve_ceo + "','" + objPortal.approve_chairman + "','"
                    + objPortal.division + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objPortal.access_purpose
                    + "','" + objPortal.portal_name  + "','" + objPortal.department + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id))
                {
                    string Category =objGlobalData.GetDropdownitemById(objPortal.portal_category);
                    string sCategory = Category.Substring(0, 3);
                    DataSet dsData = objGlobalData.GetReportNo("portal", "", sCategory);
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        access_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_portal_authorization set access_no='" + access_no + "' where id_authorization='" + id + "'";
                    objGlobalData.ExecuteQuery(sql1);                   
                }

                SendPortalAuthorizationEmail(id);
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPortalAuth: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePortalAuth(PortalModels objPortal)
        {
            try
            {
                string sSqlstmt = "update t_portal_authorization set portal_category='" + objPortal.portal_category  
                   + "', id_portal_master='" + objPortal.id_portal_master + "',upload='" + objPortal.upload + "',nominated_emp='" + objPortal.nominated_emp + "',justification='" + objPortal.justification
                   + "',recommended_by='" + objPortal.recommended_by + "',requested_by='" + objPortal.requested_by + "',approve_ceo='" + objPortal.approve_ceo + "',access_purpose='" + objPortal.access_purpose 
                   + "',division='" + objPortal.division + "',portal_name='" + objPortal.portal_name /*+ "',ministry_name='" + objPortal.ministry_name*/ + "',department='" + objPortal.department + "'";

                if (objPortal.request_date != null && objPortal.request_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", request_date='" + objPortal.request_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objPortal.access_date != null && objPortal.access_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", access_date='" + objPortal.access_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objPortal.valid_date != null && objPortal.valid_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", valid_date='" + objPortal.valid_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_authorization='" + objPortal.id_authorization + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePortalAuth: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeletePortalAuth(string sid_authorization)
        {
            try
            {
                string sSqlstmt = "update t_portal_authorization set Active=0 where id_authorization='" + sid_authorization + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePortalAuth: " + ex.ToString());
            }
            return false;
        }

        internal bool SendPortalAuthorizationEmail(int id_authorization)
        {
            try
            {
                PortalModels objParties = new PortalModels();
                string sToEmailIds = "", sCCList = "", sExtraMsg = "", aAttachment = "", sHeader = "";
                               

                //DataSet AuthPortal = objGlobalData.Getdetails("Select max(id_authorization) as id_authorization from t_portal_authorization where active = 1");
                //if (AuthPortal.Tables.Count > 0 && AuthPortal.Tables[0].Rows.Count > 0)
                //{
                //    id_authorization = AuthPortal.Tables[0].Rows[0]["id_authorization"].ToString();
                //}
                if(id_authorization > 0)
                { 
                DataSet dsDocument = objGlobalData.Getdetails("select access_no,access_date, valid_date, portal_category," +
                    "id_portal_master,nominated_emp,requested_by,recommended_by,approve_ceo,upload,logged_by" +
                     " from t_portal_authorization where id_authorization='" + id_authorization + "'");

                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    aAttachment = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[0]["upload"].ToString());


                    sHeader = "<br/><tr><td colspan=3><b>Access No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["access_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Access Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["access_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Valid Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["valid_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Sub-Cr No:<b></td> <td colspan=3>" + objParties.GetSUBCRNoById(dsDocument.Tables[0].Rows[0]["id_portal_master"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Commercial Name:<b></td> <td colspan=3>" + objParties.GetCommercialNameById(dsDocument.Tables[0].Rows[0]["id_portal_master"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Nominated Employee:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["nominated_emp"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    if (dsDocument.Tables[0].Rows[0]["recommended_by"].ToString() != "")
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["recommended_by"].ToString());
                    }

                    sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                    if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                    {
                        sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString());
                    }
                    sExtraMsg = "Portal Authorization created for following details";

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCList = Regex.Replace(sCCList, ",+", ",");
                    sCCList = sCCList.Trim();
                    sCCList = sCCList.TrimEnd(',');
                    sCCList = sCCList.TrimStart(',');

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString()), "PortalAuthorization", sExtraMsg + sHeader);
                    //objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                    return true;
                }
               }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendPortalAuthorizationEmail: " + ex.ToString());
            }
            return false;
        }

        //Authorization Approve Process

        internal bool FunApproveAuthRecommend(string id_authorization, int sStatus, string Comments)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string sSqlstmt = "";
                if (sStatus == 1) // Approved by Recommended Person
                {                    
                    sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', recommend_comments ='" + Comments + "', approve_recommed_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";                    
                }
                //else if (sStatus == 2) // Apporved by CEO 
                //{
                //    sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', approve_vp='" + Comments + "', approve_chairman='" + Comments + "', ceo_comments='" + Comments + "', approve_ceo_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";                    
                //}
                else if (sStatus == 3) // Approved by VP
                {
                    sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', vp_comments='" + Comments + "', approve_vp_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";                    
                }
                else if (sStatus == 4) // Rejected
                {
                    sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', reject_comments='" + Comments + "', rejected_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";                    
                }
                else //Approved vy Chairman
                {
                    sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', chairman_comments='" + Comments + "', approve_chairman_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";                    
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendApproveAuthRecommendEmail(id_authorization, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunApproveAuthRecommend: " + ex.ToString());
            }

            return false;
        }

        internal bool FunApproveAuthRecommendCEO(string id_authorization, int sStatus, string Comments, string sapprove_vp, string sapprove_chairman)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd");
                string sSqlstmt = "";
                if (sapprove_vp == "" || sapprove_vp == "-1")
                {
                    sSqlstmt = "update t_portal_authorization set apporved_status = 3, approve_chairman='" + sapprove_chairman + "', ceo_comments='" + Comments + "', approve_ceo_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";
                }
                else
                {
                     sSqlstmt = "update t_portal_authorization set apporved_status ='" + sStatus + "', approve_vp='" + sapprove_vp + "', approve_chairman='" + sapprove_chairman + "', ceo_comments='" + Comments + "', approve_ceo_date='" + sApprovedDate + "' where id_authorization='" + id_authorization + "'";
                }

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendApproveAuthRecommendEmail(id_authorization, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunApproveAuthRecommendCEO: " + ex.ToString());
            }

            return false;
        }

        internal bool SendApproveAuthRecommendEmail(string id_authorization, int iStatus)
        {
            try
            {
                PortalModels objParties = new PortalModels();

                string sToEmailIds = "", sCCList = "", sExtraMsg = "", aAttachment = "", sHeader = "";

                DataSet dsDocument = objGlobalData.Getdetails("select access_no,access_date, valid_date, portal_category," +
                    "id_portal_master,nominated_emp,requested_by,recommended_by,approve_ceo,approve_vp,approve_chairman,upload,logged_by," +
                    "approve_recommed_date,approve_ceo_date,approve_vp_date" +
                    " from t_portal_authorization where id_authorization='" + id_authorization + "'");

                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    aAttachment = HttpContext.Current.Server.MapPath(dsDocument.Tables[0].Rows[0]["upload"].ToString());


                    sHeader = "<br/><tr><td colspan=3><b>Access No.:<b></td> <td colspan=3> " + dsDocument.Tables[0].Rows[0]["access_no"].ToString() + "</td></tr>"
                        + "<tr><td colspan=3><b>Access Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["access_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Valid Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsDocument.Tables[0].Rows[0]["valid_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                        + "<tr><td colspan=3><b>Sub-Cr No:<b></td> <td colspan=3>" + objParties.GetSUBCRNoById(dsDocument.Tables[0].Rows[0]["id_portal_master"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Commercial Name:<b></td> <td colspan=3>" + objParties.GetCommercialNameById(dsDocument.Tables[0].Rows[0]["id_portal_master"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Nominated Employee:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["nominated_emp"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    if (iStatus == 1)//approved by Recommended Person
                    {
                        if (dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString() != "")
                        {
                            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString());
                        }

                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                        if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString());
                        }
                       sExtraMsg = "Portal Authorization approved for following details"; 
                    }
                    else if (iStatus == 2)//approved by CEO
                    {
                        if (dsDocument.Tables[0].Rows[0]["approve_vp"].ToString() != "")
                        {
                            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_vp"].ToString());
                        }

                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                        if (dsDocument.Tables[0].Rows[0]["recommended_by"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["recommended_by"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString()) ;
                        }
                       
                        sExtraMsg = "Portal Authorization approved for following details";
                    }
                    else if (iStatus == 3)//approved by VP
                    {
                        if (dsDocument.Tables[0].Rows[0]["approve_chairman"].ToString() != "")
                        {
                            sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_chairman"].ToString());
                        }

                        sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                        if (dsDocument.Tables[0].Rows[0]["recommended_by"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["recommended_by"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["approve_vp"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_vp"].ToString());
                        }
                        
                        sExtraMsg = "Portal Authorization approved for following details";
                    }
                    else if (iStatus == 5)//approved by Chairman
                    {
                        sToEmailIds= objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());

                        if (dsDocument.Tables[0].Rows[0]["nominated_emp"].ToString() != "")
                        {
                            sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["nominated_emp"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                        {
                            sToEmailIds = sToEmailIds + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["recommended_by"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["recommended_by"].ToString()) ;
                        }                       
                        if (dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString());
                        }
                        if (dsDocument.Tables[0].Rows[0]["approve_vp"].ToString() != "")
                        {
                            sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_vp"].ToString());
                        }
                      
                        sExtraMsg = "Portal Authorization approved for following details";
                    }

                    if (iStatus == 4)//Rejected 
                    {
                        sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                        if (dsDocument.Tables[0].Rows[0]["requested_by"].ToString() != "")
                        {
                            sToEmailIds = sToEmailIds + "," + objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["requested_by"].ToString());
                        }

                        if (dsDocument.Tables[0].Rows[0]["approve_recommed_date"].ToString() != "")
                        {
                            if (dsDocument.Tables[0].Rows[0]["recommended_by"].ToString() != "")
                            {
                                sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["recommended_by"].ToString());
                            }
                        }
                        if (dsDocument.Tables[0].Rows[0]["approve_ceo_date"].ToString() != "")
                        {
                            if (dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString() != "")
                            {
                               sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString());
                            }
                        }
                        if (dsDocument.Tables[0].Rows[0]["approve_vp_date"].ToString() != "")
                        {
                            if (dsDocument.Tables[0].Rows[0]["approve_vp"].ToString() != "")
                            {
                               sCCList = sCCList + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approve_vp"].ToString());
                            }
                        }

                        sExtraMsg = "Portal Authorization Rejected for the following details";
                    }

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCList = Regex.Replace(sCCList, ",+", ",");
                    sCCList = sCCList.Trim();
                    sCCList = sCCList.TrimEnd(',');
                    sCCList = sCCList.TrimStart(',');

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["approve_ceo"].ToString()), "PortalAuthorization", sExtraMsg + sHeader);
                    //objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                    objGlobalData.Sendmail(sToEmailIds, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sCCList, "");
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivesApproval: " + ex.ToString());
            }
            return false;
        }

    }

    public class PortalModelsList
    {
        public List<PortalModels> ProtalList { get; set; }
    }
}