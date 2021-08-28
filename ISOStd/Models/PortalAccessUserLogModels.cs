using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class PortalAccessUserLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_access_userlog { get; set; }

        [Display(Name = "Portal Name")]
        public string portal_name { get; set; }

        [Display(Name = "Portal Access User Name")]
        public string portal_access_username { get; set; }

        [Display(Name = "Date of Access")]
        public DateTime date_access { get; set; }

        [Display(Name = "Activity performed on portal")]
        public string activity_performed { get; set; }

        [Display(Name = "Requested by")]
        public string requested_by { get; set; }

        [Display(Name = "Request Apporved by")]
        public string request_apporvedby { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Portal Category")]
        public string portal_category { get; set; }

        //t_portal_ministry_name

        [Display(Name = "Id")]
        public string id_portal_ministry_name { get; set; }

        [Display(Name = "Ministry Url")]
        public string ministry_url { get; set; }

        [Display(Name = "Ministry Name")]
        public string ministry_name { get; set; }

        //---------------Start Portal Accer User Log --------------------

        internal bool FunAddAccessUserLog(PortalAccessUserLogModels objPortal)
        {
            try
            {
                // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_portal_access_userlog (portal_name,portal_access_username,activity_performed,requested_by," +
                    "request_apporvedby,remarks,upload,division,loggedby,portal_category";

                string sFields = "", sFieldValue = "";

                if (objPortal.date_access != null && objPortal.date_access > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", date_access";
                    sFieldValue = sFieldValue + ", '" + objPortal.date_access.ToString("yyyy/MM/dd") + "'";
                }                
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objPortal.portal_name + "','" + objPortal.portal_access_username + "','" + objPortal.activity_performed
                    + "','" + objPortal.requested_by + "','" + objPortal.request_apporvedby + "','" + objPortal.remarks
                    + "','" + objPortal.upload + "','" + objPortal.division  + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objPortal.portal_category + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAccessUserLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePortalAccess(PortalAccessUserLogModels objPortal)
        {
            try
            {
                string sSqlstmt = "update t_portal_access_userlog set portal_name ='" + objPortal.portal_name + "', portal_access_username='" + objPortal.portal_access_username + "', "
                   + "activity_performed='" + objPortal.activity_performed + "',requested_by='" + objPortal.requested_by + "',request_apporvedby='" + objPortal.request_apporvedby + "',remarks='" + objPortal.remarks
                   + "',upload='" + objPortal.upload + "',division='" + objPortal.division + "',portal_category='" + objPortal.portal_category + "'";

                if (objPortal.date_access != null && objPortal.date_access > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_access='" + objPortal.date_access.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_access_userlog='" + objPortal.id_access_userlog + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePortalAccess: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDelAccessUserLog(string sid_access_userlog)
        {
            try
            {
                string sSqlstmt = "update t_portal_access_userlog set Active=0 where id_access_userlog='" + sid_access_userlog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDelAccessUserLog: " + ex.ToString());
            }
            return false;
        }

        //---------------End Portal Accer User Log Functions --------------------


        //---------------Start Portal Name Functions--------------------
              

        public MultiSelectList GetPortalDetailsList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select id_portal_ministry_name as Id,portal_name,ministry_name,ministry_url from t_portal_ministry_name where active=1 order by portal_name asc";
                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["portal_name"].ToString()+"__"+ dsReportType.Tables[0].Rows[i]["ministry_name"].ToString()+"::"+ dsReportType.Tables[0].Rows[i]["ministry_url"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetPortalNameListBox: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        internal bool FunAddPortalName(PortalAccessUserLogModels objPortalModels)
        {
            try
            {
                string sSqlstmt = "insert into t_portal_ministry_name ( portal_name, ministry_name, ministry_url,logged_by) values('"
                    + objPortalModels.portal_name + "','" + objPortalModels.ministry_name + "','" + objPortalModels.ministry_url + "','"+objGlobalData.GetCurrentUserSession().empid + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPortalName: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePoratlName(PortalAccessUserLogModels objPortalModels)
        {
            try
            {
                string sSqlstmt = "update t_portal_ministry_name set portal_name='" + objPortalModels.portal_name + "', ministry_name='" + objPortalModels.ministry_name + "', ministry_url='" + objPortalModels.ministry_url
                    + "' where id_portal_ministry_name='" + objPortalModels.id_portal_ministry_name + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePoratlName: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeletePortalName(string sid_portal_ministry_name)
        {
            string sSqlstmt = "update t_portal_ministry_name set active=0 where id_portal_ministry_name='" + sid_portal_ministry_name + "'";
            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        //---------------End Portal Name Functions--------------------
    }

    public class PortalAccessModelsList
    {
        public List<PortalAccessUserLogModels> AccessList { get; set; }
    }
}