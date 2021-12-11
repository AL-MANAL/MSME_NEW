using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Web.Mvc.Html;
using System.Text.RegularExpressions;
namespace ISOStd.Models
{
    public class LeaveManagementModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int master_id { get; set; }
   
        [Display(Name = "Employee No")]
        public string emp_no { get; set; }

        [Display(Name = "Employee Name")]
        public string emp_firstname { get; set; }

        [Display(Name = "Date of Join")]
        public DateTime Date_of_join { get; set; }

        [Display(Name = "Days of Joined")]
        public Decimal joined_days { get; set; }

        [Display(Name = "Years of Joined")]
        public Decimal joined_years { get; set; }

        [Display(Name = "Accrued")]
        public Decimal accrued { get; set; }

        [Display(Name = "Balance")]
        public Decimal balance { get; set; }

 

        /*==============Second Table ===============*/

        //[Required]
        [Display(Name = " Leave Id")]
        public int leave_id { get; set; }

     
        [Display(Name = " Leave From  Date")]
        public DateTime fr_date { get; set; }

        [Display(Name = "Leave To  Date")]
        public DateTime to_date { get; set; }

   
        [Display(Name = " Leave Type")]
        public string leave_type { get; set; }

       
        [Display(Name = "Leave Days")]
        public Decimal duration { get; set; }

      
        [Display(Name = "Approved By")]
        public string approver { get; set; }

      
        [Display(Name = " Leave Applied On")]
        public DateTime applied_date { get; set; }

  
        [Display(Name = " Leave Status")]
        public string approved_status { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime approved_Date { get; set; }

        [Display(Name = " Reason For Applying Leave")]
        [DataType(DataType.MultilineText)]
        public string reason_leave { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        public string bal_type { get; set; }

        internal bool FunDeleteLeave(string sleave_id)
        {
            try
            {
                string sSqlstmt = "update t_leave_trans set Active=0 where leave_id='" + sleave_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteLeave: " + ex.ToString());
            }
            return false;
        }
        public string GetEmpHrNameById(string Emp_no)
        {
            try
            {
                if (Emp_no != "")
                {
                    DataSet dsEmp = objGlobalData.Getdetails("select emp_firstname as firstname from t_leave_master  where emp_no='" + Emp_no + "'");
                    if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                    {
                        return Regex.Replace(dsEmp.Tables[0].Rows[0]["firstname"].ToString(), " +", " ");

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEmpHrNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetEmpListbox()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string sSqlstmt = "select emp_no as Empid,emp_firstname as Empname from t_leave_master  order by emp_firstname asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {

                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["Empid"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")

                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                //AddFunctionalLog("Exception in GetHrEmployeeListbox: " + ex.ToString());
            }

            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        //internal bool FunAddLeaveSheetmaster(LeaveManagementModels objLeaveManagementModels)
        //{

        //    try
        //    {
        //        string sSqlStmt = "update t_leave_master set leaveActive ='0' where emp_no='" + objLeaveManagementModels.emp_no + "'";
        //        if (objGlobalData.ExecuteQuery(sSqlStmt))
        //        {
        //            string sSql = "insert into t_leave_master (cal_year, emp_no,open_bal, close_bal, balance,onSite,emp_firstname ";
        //            sSql = sSql + ") values('" + objLeaveManagementModels.cal_year + "'"
        //             + ",'" + objLeaveManagementModels.emp_no + "','" + objLeaveManagementModels.open_bal + "',"
        //            + "'" + objLeaveManagementModels.close_bal + "','" + objLeaveManagementModels.balance + "','" + objLeaveManagementModels.onSite + "','" + objLeaveManagementModels.emp_firstname + "'";
        //            sSql = sSql + ")";
        //            return objGlobalData.ExecuteQuery(sSql);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunAddLeaveSheetmaster: " + ex.ToString());
        //    }
        //    return false;
        //}
        //internal bool FunUpdateLeaveSheetmaster(LeaveManagementModels objLeaveModels)
        //{

        //    try
        //    {
        //        string sSqlstmt = "update t_leave_master set cal_year='"
        //            + objLeaveModels.cal_year + "', emp_no='" + objLeaveModels.emp_no + "', open_bal='" + objLeaveModels.open_bal + "'"
        //            + ", close_bal='" + objLeaveModels.close_bal + "', balance='" + objLeaveModels.balance + "',"
        //        + "onSite='" + objLeaveModels.onSite + "', emp_firstname='" + objLeaveModels.emp_firstname + "' where leave_id='" + objLeaveModels.leave_id + "'";

        //        return objGlobalData.ExecuteQuery(sSqlstmt);
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunUpdateLeaveSheetmaster: " + ex.ToString());
        //    }
        //    return false;
        //}


        internal bool funCheckHoliday(string dt)
        {
            try
            {

                DataSet dsBal = objGlobalData.Getdetails("select Id_holiday  from t_holiday where '" + dt + "' between frdate  and Todate");
                if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in funCheckHoliday: " + ex.ToString());
            }
            return true;
        }
        internal bool funCheckWeekend(DateTime dt)
        {
            try
            {
            
                if (dt.DayOfWeek.ToString() == "Friday" || dt.DayOfWeek.ToString() == "Saturday")
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in funCheckHoliday: " + ex.ToString());
            }
            return true;
        }
        internal bool FuncCheckCalender(DateTime dt)
        {
            try
            {
                string dtYear = dt.ToString("yyyy");
                DataSet dsBal = objGlobalData.Getdetails("select Frdate from t_holiday where year(Frdate)='" + dtYear + "' or year(Todate)='" + dtYear + "'");
                if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FuncCheckCalender: " + ex.ToString());
            }

            return false;
        }
        internal bool FuncCheckPreviousDate(string emp_no, DateTime dt)
        {
            try
            {
                string dt1 = dt.ToString("yyyy-MM-dd HH':'mm':'ss");
                DataSet dsBal = objGlobalData.Getdetails("select leave_id from t_leave_trans where emp_no='" + emp_no + "' and Active=1 and approved_status =1 and"
                + "'" + dt1 + "' between fr_date  and to_date");
                if (dsBal.Tables.Count > 0 && dsBal.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FuncCheckPreviousDate: " + ex.ToString());
            }

            return false;
        }
        internal int Funcalculatetotalleave(string emp_no, DateTime frdt, DateTime todt)
        {
            int bal = 0;
            try
            {
                for (DateTime fdt = frdt; fdt <= todt; fdt = fdt.AddDays(1.0))
                {

                    string dt = fdt.ToString("yyyy-MM-dd HH':'mm':'ss");
                    if (funCheckHoliday(dt) && funCheckWeekend(fdt))
                    {
                        bal++;
                    }

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in Funcalculatetotalleave: " + ex.ToString());
            }
            return bal;

        }

        internal bool SendEmail(LeaveManagementModels objLeaveModel)
        {
            try
            {
                string sHeader = "", sCCList="";
               
                     sCCList = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                
                string sToEmailId = objGlobalData.GetHrEmpEmailIdById(objLeaveModel.approver);
                string sUserName = objGlobalData.GetEmpHrNameById(objLeaveModel.approver);

                sHeader = "<tr><td ><b>Emp No:<b></td> <td >"
                       + objLeaveModel.emp_no + "</td></tr>"
                       + "<tr><td ><b>Emp Name:<b></td> <td >" + GetEmpHrNameById(objLeaveModel.emp_no) + "</td></tr>"
                       + "<tr><td ><b>From Date:<b></td> <td >" + objLeaveModel.fr_date.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>To Date:<b></td> <td >" + objLeaveModel.to_date.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>Reason:<b></td> <td >" + objLeaveModel.reason_leave + "</td></tr>"
                        + "<tr><td ><b>Duration:<b></td> <td >" + objLeaveModel.duration + "</td></tr>";


                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "Leave", sHeader, "", "Leave request for approval");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmail: " + ex.ToString());

            }
            return false;
        }
        internal bool FunAddLeave(LeaveManagementModels objLeaveModel)
        {
           
            try
            {
                string user = "";
               
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "insert into t_leave_trans(leave_id, emp_no,duration,reason_leave, leave_type,approver,logged_by";
                string sFields = "", sFieldValue = "";

                if (objLeaveModel.fr_date != null && objLeaveModel.fr_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", fr_date";
                    sFieldValue = sFieldValue + ", '" + objLeaveModel.fr_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                if (objLeaveModel.to_date != null && objLeaveModel.to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", to_date";
                    sFieldValue = sFieldValue + ", '" + objLeaveModel.to_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objLeaveModel.leave_id
                   + "','" + objLeaveModel.emp_no + "'"
               + ",'" + objLeaveModel.duration + "','" + objLeaveModel.reason_leave + "','" + objLeaveModel.leave_type
               + "','" + objLeaveModel.approver + "','" + user + "'";


                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendEmail(objLeaveModel);
                    return true;

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLeave: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddUsedLeave(LeaveManagementModels objLeaveModel)
        {

            try
            {
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "insert into t_leave_trans(emp_no,duration,logged_by,bal_type,approved_status";
                string sFields = "", sFieldValue = "";

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objLeaveModel.emp_no + "'"
               + ",'" + objLeaveModel.duration + "','" + user + "','Adjusted','1'";


                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                   
                    return true;

                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddUsedLeave: " + ex.ToString());
            }
            return false;
        }

        internal bool FunLeaveApproveOrReject(string leave_id, decimal duration, int sStatus, string comments)
        {
            try
            {
                string apprvd_date = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlStmt = "update t_leave_trans set approved_status ='" + sStatus + "',approved_Date ='" + apprvd_date + "',comments ='" + comments + "' where leave_id='" + leave_id + "'";

                if (objGlobalData.ExecuteQuery(sSqlStmt))
                {
                    DataSet dsLeave = objGlobalData.Call_Generate_leave_master_proc();
                    return SendApproveEmail(leave_id);
                   
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunLeaveApproveOrReject: " + ex.ToString());
            }
            return false;
        }
        internal bool SendApproveEmail(string leave_id)
        {
            try
            {
                string sSqlstmt = "select leave_id,emp_no,fr_date, to_date, leave_type,duration,approver,"
                + "applied_date,(case when approved_status='1' then 'Leave Approved' when approved_status = '0'"
                 + "then 'Yet To Be Reviewed'else 'Rejected' end) as approved_status,logged_by,reason_leave"
                + " from t_leave_trans where  Active=1 and leave_id='" + leave_id + "'";

                DataSet dsLeaveList = objGlobalData.Getdetails(sSqlstmt);
                if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                {

                    string sHeader = "";
                    string sCCList = objGlobalData.GetHrEmpEmailIdById(dsLeaveList.Tables[0].Rows[0]["approver"].ToString());
                    string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsLeaveList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sUserName = objGlobalData.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[0]["logged_by"].ToString());


                    sHeader = "<tr><td ><b>Emp No:<b></td> <td >"
                           + dsLeaveList.Tables[0].Rows[0]["emp_no"].ToString() + "</td></tr>"
                           + "<tr><td ><b>Leave Status:<b></td> <td >" + dsLeaveList.Tables[0].Rows[0]["approved_status"].ToString() + "</td></tr>"
                           + "<tr><td ><b>Emp Name:<b></td> <td >" + GetEmpHrNameById(dsLeaveList.Tables[0].Rows[0]["emp_no"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>From Date:<b></td> <td >" + Convert.ToDateTime(dsLeaveList.Tables[0].Rows[0]["fr_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                           + "<tr><td ><b>To Date:<b></td> <td >" + Convert.ToDateTime(dsLeaveList.Tables[0].Rows[0]["to_date"].ToString()).ToString("dd/MM/yyyy") + "</td></tr>"
                           + "<tr><td ><b>Reason:<b></td> <td >" + dsLeaveList.Tables[0].Rows[0]["reason_leave"].ToString() + "</td></tr>"
                            + "<tr><td ><b>Duration:<b></td> <td >" + dsLeaveList.Tables[0].Rows[0]["duration"].ToString() + "</td></tr>";


                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "LeaveApprove", sHeader, "", "Leave request approval status");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmail: " + ex.ToString());

            }
            return false;
        }
        
        //internal bool FunCheckEmpStatus(string emp_no)
        //{
        //    try
        //    {

        //        DataSet dsEmp = objGlobalData.Getdetails("select tran_id from t_leave_tran where emp_no='" + emp_no + "' and Active=1 and approved_status=0");
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FuncCheckCalender: " + ex.ToString());
        //    }

        //    return false;
        //}

    }



    public class LeaveModelsList
    {
        public List<LeaveManagementModels> LeaveMList { get; set; }
    }
}