using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ISOStd.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace ISOStd.Models
{
    public class HSEActionsTrackingRegisterModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string Actions_TrackingReg_Id { get; set; }

        [Display(Name = "HSE Record No.")]
        public string Action_No { get; set; }

        [Display(Name = "Action Initiated On")]
        public DateTime Action_Initiated_On { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Immediate action taken")]
        public string Action_Taken { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Short Fall Gap/Recommendation")]
        public string Reason_Initiating_Action { get; set; }

        [Display(Name = "Reported By")]
        public string Initiated_By { get; set; }

        [Display(Name = "Personnel Resp. to take the Action")]
        public string Personnel_Resp { get; set; }

        [Display(Name = "Due Date")]
        public DateTime Due_Date { get; set; }

        [Display(Name = "Current Status")]
        public string Action_Status { get; set; }

        [Display(Name = "Document, Records for Completed items")]
        public string Upload_Report { get; set; }

        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Report Date")]
        public DateTime Report_date { get; set; }

        [Display(Name = "Short Fall Source")]
        public string Short_Fall_Source { get; set; }

        [Display(Name = "Division")]
        //[Display(Name = "Applicable Site")]
        public string Applicable_Site { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Risk Rank")]
        public string Risk_Rank { get; set; }

        [Display(Name = "KPI Completion Date")]
        public DateTime KPI_Completion_Date { get; set; }

        [Display(Name = "Responsible Department")]
        public string Resp_Dept { get; set; }

        [Display(Name = "Responsible Section")]
        public string Resp_Section { get; set; }

        [Display(Name = "Endorsed By")]
        public string EndorsedBy { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Reviewed By")]
        public string ReviewedBy { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Additional Attachment")]
        public string Additional_Attachment { get; set; }

        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunDeleteSafetyVoilationLogDoc(string sActions_TrackingReg_Id)
        {
            try
            {
                string sSqlstmt = "update t_hse_actions_trackingreg set Active=0 where Actions_TrackingReg_Id='" + sActions_TrackingReg_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSafetyVoilationLogDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddHSEActionsTrackingRegister(HSEActionsTrackingRegisterModels objHSEActionsTrackingRegister)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_hse_actions_trackingreg (Action_No, Action_Taken, Reason_Initiating_Action, Initiated_By,"
                    + " Personnel_Resp, Action_Status, LoggedBy, "
                    + "Short_Fall_Source, Applicable_Site, Category, Risk_Rank, Resp_Dept, Resp_Section,"
                    + " EndorsedBy, ApprovedBy, ReviewedBy, Remarks,NotificationDays,NotificationPeriod,NotificationValue,branch";

                if (objHSEActionsTrackingRegister.Action_Initiated_On > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Action_Initiated_On";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.Action_Initiated_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.Due_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Due_Date";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.Due_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.Report_date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Report_date";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.Report_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.KPI_Completion_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", KPI_Completion_Date";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.KPI_Completion_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.Upload_Report != null && objHSEActionsTrackingRegister.Upload_Report != "")
                {
                    sColumn = sColumn + ", Upload_Report";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.Upload_Report + "' ";
                }

                if (objHSEActionsTrackingRegister.Additional_Attachment != null && objHSEActionsTrackingRegister.Additional_Attachment != "")
                {
                    sColumn = sColumn + ", Additional_Attachment";
                    sValues = sValues + ", '" + objHSEActionsTrackingRegister.Additional_Attachment + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objHSEActionsTrackingRegister.Action_No + "','" + objHSEActionsTrackingRegister.Action_Taken
                    + "','" + objHSEActionsTrackingRegister.Reason_Initiating_Action + "','" + objHSEActionsTrackingRegister.Initiated_By
                    + "','" + objHSEActionsTrackingRegister.Personnel_Resp + "','" + objHSEActionsTrackingRegister.Action_Status
                    + "','" + user
                     + "', '" + objHSEActionsTrackingRegister.Short_Fall_Source + "', '" + objHSEActionsTrackingRegister.Applicable_Site
                    + "', '" + objHSEActionsTrackingRegister.Category + "', '" + objHSEActionsTrackingRegister.Risk_Rank
                    + "', '" + objHSEActionsTrackingRegister.Resp_Dept + "', '" + objHSEActionsTrackingRegister.Resp_Section
                    + "', '" + objHSEActionsTrackingRegister.EndorsedBy + "', '" + objHSEActionsTrackingRegister.ApprovedBy
                    + "', '" + objHSEActionsTrackingRegister.ReviewedBy + "', '" + objHSEActionsTrackingRegister.Remarks + "'"
                    + ",'" + objHSEActionsTrackingRegister.NotificationDays + "','" + objHSEActionsTrackingRegister.NotificationPeriod + "'"
                    + ",'" + objHSEActionsTrackingRegister.NotificationValue + "','" + objHSEActionsTrackingRegister.branch + "'"; 

                 sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddHSEActionsTrackingRegister: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateHSEActionsTrackingRegister(HSEActionsTrackingRegisterModels objHSEActionsTrackingRegister)
        {
            try
            {
                string sSqlstmt = " update t_hse_actions_trackingreg set Action_No='" + objHSEActionsTrackingRegister.Action_No
                    + "', Action_Taken='" + objHSEActionsTrackingRegister.Action_Taken
                    + "', Reason_Initiating_Action='" + objHSEActionsTrackingRegister.Reason_Initiating_Action + "', Initiated_By='" + objHSEActionsTrackingRegister.Initiated_By
                    + "', Personnel_Resp='" + objHSEActionsTrackingRegister.Personnel_Resp + "', Action_Status='" + objHSEActionsTrackingRegister.Action_Status
                    + "', Short_Fall_Source='" + objHSEActionsTrackingRegister.Short_Fall_Source + "', Applicable_Site='" + objHSEActionsTrackingRegister.Applicable_Site
                    + "', Category='" + objHSEActionsTrackingRegister.Category + "', Risk_Rank='" + objHSEActionsTrackingRegister.Risk_Rank
                    + "', Resp_Dept='" + objHSEActionsTrackingRegister.Resp_Dept + "', Resp_Section='" + objHSEActionsTrackingRegister.Resp_Section
                    + "', EndorsedBy='" + objHSEActionsTrackingRegister.EndorsedBy + "', ApprovedBy='" + objHSEActionsTrackingRegister.ApprovedBy
                    + "', ReviewedBy='" + objHSEActionsTrackingRegister.ReviewedBy + "', Remarks='" + objHSEActionsTrackingRegister.Remarks + "'"
                    + ", NotificationDays='" + objHSEActionsTrackingRegister.NotificationDays + "', NotificationPeriod='" + objHSEActionsTrackingRegister.NotificationPeriod + "', NotificationValue='" + objHSEActionsTrackingRegister.NotificationValue + "'"
                    + ", Upload_Report='" + objHSEActionsTrackingRegister.Upload_Report + "', Additional_Attachment='" + objHSEActionsTrackingRegister.Additional_Attachment + "', branch='" + objHSEActionsTrackingRegister.branch + "'";

                if (objHSEActionsTrackingRegister.Action_Initiated_On > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Action_Initiated_On='" + objHSEActionsTrackingRegister.Action_Initiated_On.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.Due_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Due_Date='" + objHSEActionsTrackingRegister.Due_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.Report_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Report_date='" + objHSEActionsTrackingRegister.Report_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objHSEActionsTrackingRegister.KPI_Completion_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", KPI_Completion_Date='" + objHSEActionsTrackingRegister.KPI_Completion_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Actions_TrackingReg_Id='" + objHSEActionsTrackingRegister.Actions_TrackingReg_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateHSEActionsTrackingRegister: " + ex.ToString());
            }
            return false;
        }

        internal string GetHSEStatusNameById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='HSE Status' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetHSEStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiHSEStatusList()
        {
            DropdownHSEList lstHSE = new DropdownHSEList();
            lstHSE.lstHSE = new List<DropdownHSEItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='HSE Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownHSEItems reg = new DropdownHSEItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstHSE.lstHSE.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHSEStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lstHSE.lstHSE, "Id", "Name");
        }

        internal string GetShortFallSourceNameById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='HSE Short Fall Source' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetShortFallSourceNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiShortFallSourceList()
        {
            DropdownHSEList lstHSE = new DropdownHSEList();
            lstHSE.lstHSE = new List<DropdownHSEItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='HSE Short Fall Source' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownHSEItems reg = new DropdownHSEItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstHSE.lstHSE.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiShortFallSourceList: " + ex.ToString());
            }

            return new MultiSelectList(lstHSE.lstHSE, "Id", "Name");
        }

        internal string GetRiskRankNameById(string sRank)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='HSE Risk Rank' and (item_id='" + sRank + "' or item_desc='" + sRank + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRiskRankNameById: " + ex.ToString());
            }

            return "";
        }

        public MultiSelectList GetMultiRiskRankList()
        {
            DropdownHSEList lstHSE = new DropdownHSEList();
            lstHSE.lstHSE = new List<DropdownHSEItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='HSE Risk Rank' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownHSEItems reg = new DropdownHSEItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstHSE.lstHSE.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiRiskRankList: " + ex.ToString());
            }

            return new MultiSelectList(lstHSE.lstHSE, "Id", "Name");
        }

    }

    public class HSEActionsTrackingRegisterModelsList
    {
        public List<HSEActionsTrackingRegisterModels> lstHSEActionsTrackingRegister { get; set; }
    }
    public class ProductShareBarchart
    {
        public string Product { get; set; }
        public decimal Percentage { get; set; }
    }

    public class DropdownHSEItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownHSEList
    {
        public List<DropdownHSEItems> lstHSE { get; set; }
    }
    public class ProductShareBarchartList
    {
        public List<ProductShareBarchart> lstProductSharechart { get; set; }
    }
}