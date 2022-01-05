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
    public class EmergencyPlanNRecordModels
    {
        clsGlobal objGlobalData = new clsGlobal();

  
        [Display(Name = "ID")]
        public string Emergency_Plan_Id { get; set; }


        [Display(Name = "Plan Prepared By")]
        public string emp_id { get; set; }

 
        [Display(Name = "Plan Period From")]
        public DateTime Plan_From { get; set; }

        [Display(Name = "Plan Period To")]
        public DateTime Plan_To { get; set; }

        [Display(Name = "Emergency Type")]
        public string Emergency_Type { get; set; }

        [Display(Name = "Planned Date and Timing")]
        public DateTime Plan_Date_Time { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Location where drill to be conducted?")]
        public string Drill_Location { get; set; }

        [Display(Name = "In Charge")]
        public string Incharge { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Need of Reporting to any regulatory authorities")]
        public string Need_Reporting { get; set; }


        [Display(Name = "Status")]
        public string Plan_Status { get; set; }


        [Display(Name = "Emergency Drill Performance?")]
        public string Performance { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        //[Required]
        [Display(Name = "Report No.")]
        public string ReportNo { get; set; }

        //[Required]
        [Display(Name = "Upload Report")]
        public string Upload_Report { get; set; }

        //[Required]
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        //[Required]
        [Display(Name = "Plan Reviewed By")]
        public string ReviewedBy { get; set; }

        //[Required]
        [Display(Name = "Plan Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Observation")]
        public string Observation { get; set; }

        [Display(Name = "ID")]
        public int id_emergency_trans { get; set; }

        [Display(Name = "Corrective action")]
        public string Corrrective_action { get; set; }

        [Display(Name = "Target Date")]
        public DateTime Target_date { get; set; }

        [Display(Name = "No of Employees attended")]
        public string No_Emp { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Id")]
        public string id_perform { get; set; }

        [Display(Name = "Action Details")]
        public string action_details { get; set; }

        [Display(Name = "Alarm raised by")]
        public string alarm_raised_by { get; set; }

        [Display(Name = "Assembly point")]
        public string assembly_point { get; set; }

        [Display(Name = "Date and time of drill")]
        public DateTime drill_date { get; set; }

        [Display(Name = "Observation")]
        public string perf_observation { get; set; }

        [Display(Name = "Remarks")]
        public string perf_remarks { get; set; }

        [Display(Name = "Location")]
        public string perf_location { get; set; }


        internal bool FunDeleteEmergencyPlanDoc(string sEmergency_Plan_Id)
        {
            try
            {
                string sSqlstmt = "update t_emergency_plan_record set Active=0 where Emergency_Plan_Id='" + sEmergency_Plan_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEmergencyPlanDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddEmergencyPlan(EmergencyPlanNRecordModels objEmergencyPlanNRecord, EmergencyPlanNRecordModelsList objPlanList)
        {
            try
            {

                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_emergency_plan_record (emp_id, Emergency_Type, Drill_Location, Incharge, Need_Reporting,"
                        + " Plan_Status, Performance, Remarks, ReportNo, ReviewedBy, ApprovedBy, LoggedBy,Observation,No_Emp,branch,Department,Location";
                string user = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                user = objGlobalData.GetCurrentUserSession().empid;
                
                if (objEmergencyPlanNRecord.Plan_From > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Plan_From";
                    sValues = sValues + ", '" + objEmergencyPlanNRecord.Plan_From.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Plan_To > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Plan_To";
                    sValues = sValues + ", '" + objEmergencyPlanNRecord.Plan_To.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Plan_Date_Time > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Plan_Date_Time";
                    sValues = sValues + ", '" + objEmergencyPlanNRecord.Plan_Date_Time.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Upload_Report != null && objEmergencyPlanNRecord.Upload_Report != "")
                {
                    sColumn = sColumn + ", Upload_Report";
                    sValues = sValues + ", '" + objEmergencyPlanNRecord.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objEmergencyPlanNRecord.emp_id + "','" + objEmergencyPlanNRecord.Emergency_Type
                    + "','" + objEmergencyPlanNRecord.Drill_Location + "','" + objEmergencyPlanNRecord.Incharge + "','" + objEmergencyPlanNRecord.Need_Reporting
                 + "','" + objEmergencyPlanNRecord.Plan_Status + "','" + objEmergencyPlanNRecord.Performance + "','" + objEmergencyPlanNRecord.Remarks
                 + "','" + objEmergencyPlanNRecord.ReportNo + "','" + objEmergencyPlanNRecord.ReviewedBy + "','" + objEmergencyPlanNRecord.ApprovedBy
                 + "','" + user + "','" + objEmergencyPlanNRecord.Observation + "','" + objEmergencyPlanNRecord.No_Emp + "','" + objEmergencyPlanNRecord.branch 
                 + "','" + objEmergencyPlanNRecord.Department + "','" + objEmergencyPlanNRecord.Location + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                //return FunAddEmegencyTrans(objPlanList,Convert.ToString(objGlobalData.ExecuteQueryReturnId(sSqlstmt)));


                int Emergency_Plan_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Emergency_Plan_Id))
                {
                    if (FunAddEmegencyTrans(objPlanList, Convert.ToString(Emergency_Plan_Id)))
                    {
                        DataSet dsData = objGlobalData.GetReportNo("EMY", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ReportNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }
                        string sql1 = "update t_emergency_plan_record set ReportNo='" + ReportNo + "' where Emergency_Plan_Id='" + Emergency_Plan_Id + "'";
                        return (objGlobalData.ExecuteQuery(sql1));
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmergencyPlan: " + ex.ToString());
            }
            return false;

        }
        internal bool FunUpdateEmegencyTrans(string Emergency_Plan_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_emergency_plan_record_trans where Emergency_Plan_Id='" + Emergency_Plan_Id + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmegencyTrans: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddEmegencyTrans(EmergencyPlanNRecordModelsList objPlanList, string Emergency_Plan_Id)
        {
            try
            {
                string sSqlstmt = "delete from t_emergency_plan_record_trans where Emergency_Plan_Id='" + Emergency_Plan_Id + "'; ";

                for (int i = 0; i < objPlanList.lstEmergencyPlanNRecord.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_emergency_plan_record_trans (Emergency_Plan_Id,Corrrective_action";
                    string sFields = "", sFieldValue = "";

                    if (objPlanList.lstEmergencyPlanNRecord[i].Target_date != null && objPlanList.lstEmergencyPlanNRecord[i].Target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Target_date";
                        sFieldValue = sFieldValue + ", '" + objPlanList.lstEmergencyPlanNRecord[i].Target_date.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + Emergency_Plan_Id + "','" + objPlanList.lstEmergencyPlanNRecord[i].Corrrective_action + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                     
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmegencyTrans: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateEmergencyPlan(EmergencyPlanNRecordModels objEmergencyPlanNRecord, EmergencyPlanNRecordModelsList objPlanList)
        {
            try
            {
                string sSqlstmt = " update t_emergency_plan_record set emp_id='" + objEmergencyPlanNRecord.emp_id + "', Emergency_Type='" + objEmergencyPlanNRecord.Emergency_Type
                    + "', Drill_Location='" + objEmergencyPlanNRecord.Drill_Location + "', Incharge='" + objEmergencyPlanNRecord.Incharge
                    + "', Need_Reporting='" + objEmergencyPlanNRecord.Need_Reporting + "', Plan_Status='" + objEmergencyPlanNRecord.Plan_Status
                    + "', Performance='" + objEmergencyPlanNRecord.Performance + "', Remarks='" + objEmergencyPlanNRecord.Remarks
                    + "', ReviewedBy='" + objEmergencyPlanNRecord.ReviewedBy + "', ApprovedBy='" + objEmergencyPlanNRecord.ApprovedBy
                    + "', Observation='" + objEmergencyPlanNRecord.Observation + "', No_Emp='" + objEmergencyPlanNRecord.No_Emp + "', Department='" + objEmergencyPlanNRecord.Department
                    + "', Location='" + objEmergencyPlanNRecord.Location + "', branch='" + objEmergencyPlanNRecord.branch + "'";

                if (objEmergencyPlanNRecord.Plan_From > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Plan_From='" + objEmergencyPlanNRecord.Plan_From.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Plan_To > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Plan_To='" + objEmergencyPlanNRecord.Plan_To.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Plan_Date_Time > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Plan_Date_Time='" + objEmergencyPlanNRecord.Plan_Date_Time.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmergencyPlanNRecord.Upload_Report != null && objEmergencyPlanNRecord.Upload_Report != "")
                {
                    sSqlstmt = sSqlstmt + ", Upload_Report='" + objEmergencyPlanNRecord.Upload_Report + "' ";
                }

                sSqlstmt = sSqlstmt + " where Emergency_Plan_Id='" + objEmergencyPlanNRecord.Emergency_Plan_Id + "'";

                int Emergency_Plan_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Emergency_Plan_Id))
                {


                    if (objPlanList.lstEmergencyPlanNRecord.Count > 0)
                    {
                        return FunAddEmegencyTrans(objPlanList, objEmergencyPlanNRecord.Emergency_Plan_Id);
                    }
                    else
                    {
                        return FunUpdateEmegencyTrans(objEmergencyPlanNRecord.Emergency_Plan_Id);

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmergencyPlan: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePerformPlan(EmergencyPlanNRecordModels objEmergencyPlanNRecord)
        {
            try
            {
                string sSqlstmt = " update t_emergency_plan_record_perform set action_details='" + objEmergencyPlanNRecord.action_details + "', perf_observation='" + objEmergencyPlanNRecord.perf_observation
                    + "', perf_remarks='" + objEmergencyPlanNRecord.perf_remarks + "', perf_location='" + objEmergencyPlanNRecord.perf_location
                    + "', alarm_raised_by='" + objEmergencyPlanNRecord.alarm_raised_by + "', assembly_point='" + objEmergencyPlanNRecord.assembly_point + "'";


                if (objEmergencyPlanNRecord.drill_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", drill_date='" + objEmergencyPlanNRecord.drill_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }


                sSqlstmt = sSqlstmt + " where id_perform='" + objEmergencyPlanNRecord.id_perform + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePerformPlan: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddPerformPlan(EmergencyPlanNRecordModels objEmergencyPlanNRecord)
        {
            try
            {

                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_emergency_plan_record_perform (Emergency_Plan_Id,action_details,perf_observation,perf_remarks,perf_location,alarm_raised_by,assembly_point";
               
                if (objEmergencyPlanNRecord.drill_date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", drill_date";
                    sValues = sValues + ", '" + objEmergencyPlanNRecord.drill_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + Emergency_Plan_Id+ "','" + action_details + "','" + perf_observation + "','" + perf_remarks + "','" + perf_location + "','" + alarm_raised_by + "','" + assembly_point + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmergencyPlan: " + ex.ToString());
            }
            return false;

        }

        internal string GetEmergencyTypeNameById(string sEmergency)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Emergency Type' and (item_id='" + sEmergency + "' or item_desc='" + sEmergency + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEmergencyTypeNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiEmergencyTypeList()
        {
            DropdownEmergencyList lst = new DropdownEmergencyList();
            lst.lstEmergency = new List<DropdownEmergencyItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Emergency Type' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEmergencyItems reg = new DropdownEmergencyItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lstEmergency.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiEmergencyTypeList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lstEmergency, "Id", "Name");
        }

        internal string GetHSEPerformanceNameById(string sStatus)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='HSE Performance' and (item_id='" + sStatus + "' or item_desc='" + sStatus + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetHSEPerformanceNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiHSEPerformanceList()
        {
            DropdownEmergencyList lst = new DropdownEmergencyList();
            lst.lstEmergency = new List<DropdownEmergencyItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='HSE Performance' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEmergencyItems reg = new DropdownEmergencyItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lstEmergency.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHSEPerformanceList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lstEmergency, "Id", "Name");
        }
        internal string GetDrillPerformanceNameById(string sDrill)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Emergency Dril Performance' and (item_id='" + sDrill + "' or item_desc='" + sDrill + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetDrillPerformanceNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiDrillPerformanceList()
        {
            DropdownEmergencyList lst = new DropdownEmergencyList();
            lst.lstEmergency = new List<DropdownEmergencyItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Emergency Dril Performance' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownEmergencyItems reg = new DropdownEmergencyItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lstEmergency.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiDrillPerformanceList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lstEmergency, "Id", "Name");
        }
    }

    public class EmergencyPlanNRecordModelsList
    {
        public List<EmergencyPlanNRecordModels> lstEmergencyPlanNRecord { get; set; }
    }
    public class DropdownEmergencyItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownEmergencyList
    {
        public List<DropdownEmergencyItems> lstEmergency { get; set; }
    }
}