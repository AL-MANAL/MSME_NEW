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
    public class LeaveMgmtModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_leave_credit { get; set; }

        [Display(Name = "Id")]
        public string id_leavemaster { get; set; }

        [Display(Name = "Id")]
        public string id_adjustment { get; set; }


        [Display(Name = "Employee Name")]
        public string emp_firstname { get; set; }

        public string emp_no { get; set; }

        [Display(Name = "Year")]
        public string  syear { get; set; }

        [Display(Name = "Annual Leave")]
        public decimal annual_leave { get; set; }

        [Display(Name = "Sick Leave")]
        public decimal sick_leave { get; set; }

        [Display(Name = "Other Leave")]
        public decimal other_leave { get; set; }

        [Display(Name = "Carry Forward")]
        public decimal carry_leave { get; set; }

        [Display(Name = "Balalnce Annual Leave")]
        public decimal bal_annual_leave { get; set; }


        [Display(Name = "Balalnce Sick Leave")]
        public decimal bal_sick_leave { get; set; }


        [Display(Name = "Balalnce Other Leave")]
        public decimal bal_other_leave { get; set; }


        public string leave_gen { get; set; }

        public List<LeaveMgmtModels> leaveTransList { get; set; }

        [Display(Name = "No of Days")]
        public int days { get; set; }

        public string comp_id { get; set; }

        public decimal balance { get; set; }

        [Display(Name = "Type")]
        public string day_type { get; set; }
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
        public decimal duration { get; set; }


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

        public string op_type { get; set; }
        public string adj_type { get; set; }

        [Display(Name = "Generated Date")]
        public DateTime logged_date { get; set; }

        internal bool FunUpdateLeaveAdjust(LeaveMgmtModels objLeave)
        {
            try
            {
                string sSqlstmt = "";

                string Sqlstmt = "insert into t_leaveadjustment (emp_no,syear,adj_type,days,op_type,logged_by)"
                + "values('" + objLeave.emp_no + "','" + objLeave.syear + "','" + objLeave.adj_type + "','" + objLeave.days + "','" + objLeave.op_type + "','" + objGlobalData.GetCurrentUserSession().empid + "')";

                if(objGlobalData.ExecuteQuery(Sqlstmt))
                {
                  if (objLeave.op_type == "Add")
                  {
                    switch (objLeave.adj_type)
                    {
                        case "Annual Leave":
                            sSqlstmt = "update t_leavemaster set annual_leave=annual_leave + '" + days + "', bal_annual_leave=bal_annual_leave + '" + days + "' where emp_no='" + emp_no + "'";
                        return objGlobalData.ExecuteQuery(sSqlstmt);

                        case "Sick Leave":
                        sSqlstmt = "update t_leavemaster set sick_leave=sick_leave + '" + days + "',bal_sick_leave=bal_sick_leave + '" + days + "' where emp_no='" + emp_no + "'";
                        return objGlobalData.ExecuteQuery(sSqlstmt);

                        case "Other Leave":
                        sSqlstmt = "update t_leavemaster set other_leave=other_leave + '" + days + "',bal_other_leave=bal_other_leave + '" + days + "' where emp_no='" + emp_no + "'";
                        return objGlobalData.ExecuteQuery(sSqlstmt);

                        default: break;
                    }
                   
                }
                else
                {
                    switch (objLeave.adj_type)
                    {
                        case "Annual Leave":
                            sSqlstmt = "update t_leavemaster set annual_leave=annual_leave - '" + days + "', bal_annual_leave=bal_annual_leave - '" + days + "' where emp_no='" + emp_no + "'";
                            return objGlobalData.ExecuteQuery(sSqlstmt);

                        case "Sick Leave":
                            sSqlstmt = "update t_leavemaster set sick_leave=sick_leave - '" + days + "',bal_sick_leave=bal_sick_leave - '" + days + "' where emp_no='" + emp_no + "'";
                            return objGlobalData.ExecuteQuery(sSqlstmt);

                        case "Other Leave":
                            sSqlstmt = "update t_leavemaster set other_leave=other_leave - '" + days + "',bal_other_leave=bal_other_leave - '" + days + "' where emp_no='" + emp_no + "'";
                            return objGlobalData.ExecuteQuery(sSqlstmt);
                        default: break;
                    }
                }
              
                }

              
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLeaveAdjust: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddLeaveCredit(LeaveMgmtModels objLeave)
        {
            try
            {
                string sSqlstmt = "insert into t_leave_credit (syear,annual_leave,sick_leave,other_leave,carry_leave,logged_by)"
                + "values('" + objLeave.syear + "','" + objLeave.annual_leave + "','" + objLeave.sick_leave + "','" + objLeave.other_leave + "','" + objLeave.carry_leave + "','" + objGlobalData.GetCurrentUserSession().empid + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    DataSet dsData = objGlobalData.generateAnnualLeave(syear);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLeaveCredit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateLeaveCredit(LeaveMgmtModels objLeave)
        {
            try
            {
                string sToday = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlstmt = "update t_leave_credit set annual_leave='" + annual_leave + "',sick_leave='" + sick_leave + "',other_leave='" + other_leave + "',carry_leave='" + carry_leave + "',leave_gen='N',logged_by='" + objGlobalData.GetCurrentUserSession().empid + "',logged_date='"+sToday+"'"
                    + " where id_leave_credit='" + id_leave_credit + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    DataSet dsData = objGlobalData.generateAnnualLeave(syear);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLeaveCredit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteLeaveCredit(string sid_leave_credit)
        {
            try
            {
                string sSqlstmt = "update t_leave_credit set Active=0 where id_leave_credit='" + sid_leave_credit + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteLeaveCredit: " + ex.ToString());
            }
            return false;
        }
        public bool FunCheckYearExists(int syear)
        {
            int maxYear = 0;
            string sSql = "select Max(syear) as year from t_leave_credit where active=1";
            DataSet dsDatas = objGlobalData.Getdetails(sSql);
            if (dsDatas.Tables.Count > 0 && dsDatas.Tables[0].Rows.Count > 0)
            {
                if (dsDatas.Tables[0].Rows[0]["year"].ToString() != null && dsDatas.Tables[0].Rows[0]["year"].ToString() != "")
                {
                    maxYear = Convert.ToInt32(dsDatas.Tables[0].Rows[0]["year"].ToString());
                }
                else
                {
                    return true;
                }
            }
          
            string sSqlstmt = "select id_leave_credit from t_leave_credit where syear='" + syear + "' and active=1";
            DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
            if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
            {

                return false;
            }
            else
            {
                if (syear == maxYear+1)
                {
                    return true;
                }
                
            }
            return false;
        }
    

        internal bool FunAddLeave(LeaveMgmtModels objLeaveModel)
        {

            try
            {
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "insert into t_leavetrans(syear, emp_no,duration,reason_leave, leave_type,approver,logged_by,day_type";
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
                sSqlstmt = sSqlstmt + ") values('" + objLeaveModel.syear
                   + "','" + objLeaveModel.emp_no + "'"
               + ",'" + objLeaveModel.duration + "','" + objLeaveModel.reason_leave + "','" + objLeaveModel.leave_type
               + "','" + objLeaveModel.approver + "','" + user + "','" + objLeaveModel.day_type + "'";


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
        internal bool SendEmail(LeaveMgmtModels objLeaveModel)
        {
            try
            {
                string sInformation = "", sHeader = "", sCCList = "", to_date="";

                sCCList = objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);

                string sToEmailId = objGlobalData.GetHrEmpEmailIdById(objLeaveModel.approver);
                string sUserName = objGlobalData.GetEmpHrNameById(objLeaveModel.approver);
                if (objLeaveModel.to_date != null && objLeaveModel.to_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    to_date = objLeaveModel.to_date.ToString("dd/MM/yyyy");
                }
                sHeader = "<tr><td ><b>Emp No:<b></td> <td >"
                       + objGlobalData.GetEmpIDByEmpNo(objLeaveModel.emp_no) + "</td></tr>"
                       + "<tr><td ><b>Emp Name:<b></td> <td >" + objGlobalData.GetEmpHrNameById(objLeaveModel.emp_no) + "</td></tr>"
                       + "<tr><td ><b>From Date:<b></td> <td >" + objLeaveModel.fr_date.ToString("dd/MM/yyyy") + "</td></tr>"
                       + "<tr><td ><b>To Date:<b></td> <td >" + to_date + "</td></tr>"
                       + "<tr><td ><b>Reason:<b></td> <td >" + objLeaveModel.reason_leave + "</td></tr>"
                       + "<tr><td ><b>Duration:<b></td> <td >" + objLeaveModel.duration + "</td></tr>"
                       + "<tr><td ><b>Leave Type:<b></td> <td >" + objLeaveModel.leave_type + "</td></tr>";


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

        internal bool FunLeaveApproveOrReject(string leave_id, decimal duration, int sStatus, string comments, string emp_no, string leave_type)
        {
            try
            {
                string apprvd_date = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSqlStmt = "update t_leavetrans set approved_status ='" + sStatus + "',approved_Date ='" + apprvd_date + "',comments ='" + comments + "' where leave_id='" + leave_id + "'";

                if (objGlobalData.ExecuteQuery(sSqlStmt))
                {
                    string sql = "";
                    if (sStatus == 1)
                    {
                        switch (leave_type)
                        {

                            case "Annual Leave":
                                sql = "update t_leavemaster set bal_annual_leave=bal_annual_leave-'" + duration + "' where emp_no='" + emp_no + "'";
                                objGlobalData.ExecuteQuery(sql);
                                return SendApproveEmail(leave_id);

                            case "Sick Leave":
                                sql = "update t_leavemaster set bal_sick_leave=bal_sick_leave-'" + duration + "' where emp_no='" + emp_no + "'";
                                objGlobalData.ExecuteQuery(sql);
                                return SendApproveEmail(leave_id);

                            case "Other Leave":
                                sql = "update t_leavemaster set bal_other_leave=bal_other_leave-'" + duration + "' where emp_no='" + emp_no + "'";
                                objGlobalData.ExecuteQuery(sql);
                                return SendApproveEmail(leave_id);
                            default:
                                break;

                        }
                    }
                    else
                    {
                        return SendApproveEmail(leave_id);
                    }
                  
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
                + " from t_leavetrans where  Active=1 and leave_id='" + leave_id + "'";

                DataSet dsLeaveList = objGlobalData.Getdetails(sSqlstmt);
                if (dsLeaveList.Tables.Count > 0 && dsLeaveList.Tables[0].Rows.Count > 0)
                {

                    string sInformation = "", sHeader = "", fr_date = "", to_date="";
                    string sCCList = objGlobalData.GetHrEmpEmailIdById(dsLeaveList.Tables[0].Rows[0]["approver"].ToString());
                    string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsLeaveList.Tables[0].Rows[0]["logged_by"].ToString());
                    string sUserName = objGlobalData.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[0]["logged_by"].ToString());

                    if (dsLeaveList.Tables[0].Rows[0]["fr_date"].ToString() != null && dsLeaveList.Tables[0].Rows[0]["fr_date"].ToString() != "")
                    {
                       fr_date = Convert.ToDateTime(dsLeaveList.Tables[0].Rows[0]["fr_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    if (dsLeaveList.Tables[0].Rows[0]["to_date"].ToString() != null && dsLeaveList.Tables[0].Rows[0]["to_date"].ToString() != "")
                    {
                        to_date = Convert.ToDateTime(dsLeaveList.Tables[0].Rows[0]["to_date"].ToString()).ToString("dd/MM/yyyy");
                    }
                    sHeader = "<tr><td ><b>Emp No:<b></td> <td >"
                           +objGlobalData.GetEmpIDByEmpNo(dsLeaveList.Tables[0].Rows[0]["emp_no"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>Leave Status:<b></td> <td >" + dsLeaveList.Tables[0].Rows[0]["approved_status"].ToString() + "</td></tr>"
                           + "<tr><td ><b>Emp Name:<b></td> <td >" +objGlobalData.GetEmpHrNameById(dsLeaveList.Tables[0].Rows[0]["emp_no"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>From Date:<b></td> <td >" + fr_date + "</td></tr>"
                           + "<tr><td ><b>To Date:<b></td> <td >" + to_date + "</td></tr>"
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
        public MultiSelectList GetEmpListbox()
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();

            try
            {
                string role = objGlobalData.GetCurrentUserSession().role;
                string sSqlstmt = "";

                if (!(objGlobalData.GetMultiRolesNameById(role).Contains("Admin")))
                {
                     sSqlstmt = "select t.emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname from t_leavemaster t,t_hr_employee tt where t.emp_no=tt.emp_no and active=1 "
                    + " and t.emp_no='" + objGlobalData.GetCurrentUserSession().empid+ "' order by emp_firstname asc";

                }
                else
                {
                     sSqlstmt = "select t.emp_no as Empid,concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname from t_leavemaster t,t_hr_employee tt where t.emp_no=tt.emp_no and active=1 order by emp_firstname asc";

                }

             
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
        internal bool FuncCheckPreviousDate(string emp_no, DateTime dt)
        {
            try
            {
                string dt1 = dt.ToString("yyyy-MM-dd HH':'mm':'ss");
                DataSet dsBal = objGlobalData.Getdetails("select leave_id from t_leavetrans where emp_no='" + emp_no + "' and Active=1 and approved_status =1 and"
                + "('" + dt1 + "' between fr_date  and to_date or '" + dt1 + "'=fr_date)");
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

        internal bool FunDeleteLeave(string sleave_id, string emp_no,decimal duration)
        {
            try
            {
                string sql = "", approved_status="", leave_type="";
                string sSql = "select (case when approved_status='1' then 'Leave Approved' when approved_status = '0' then 'Yet To Be Reviewed'"
                   + "else 'Rejected' end) as approved_status,leave_type from t_leavetrans where leave_id='" + sleave_id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSql);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    approved_status = (dsData.Tables[0].Rows[0]["approved_status"].ToString());
                    leave_type = (dsData.Tables[0].Rows[0]["leave_type"].ToString());
                }
                string sSqlstmt = "update t_leavetrans set Active=0 where leave_id='" + sleave_id + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (approved_status == "Leave Approved")
                    {
                       switch(leave_type)
                       {
                           case "Annual Leave":
                               sql="update t_leavemaster set bal_annual_leave=bal_annual_leave+'"+duration+"' where emp_no='"+emp_no+"'";
                               return objGlobalData.ExecuteQuery(sql);

                           case "Sick Leave":
                               sql = "update t_leavemaster set bal_sick_leave=bal_sick_leave+'" + duration + "' where emp_no='" + emp_no + "'";
                               return objGlobalData.ExecuteQuery(sql);

                           case "Other Leave":
                               sql = "update t_leavemaster set bal_other_leave=bal_other_leave+'" + duration + "' where emp_no='" + emp_no + "'";
                               return objGlobalData.ExecuteQuery(sql);

                           default:break;

                       }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteLeave: " + ex.ToString());
            }
            return false;
        }
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
        internal bool funCheckWeekend(DateTime dt, string weekend)
        {
            try
            {

                if (weekend.Contains(dt.DayOfWeek.ToString()))
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
        internal int Funcalculatetotalleave(string emp_no, DateTime frdt, DateTime todt)
        {
            int bal = 0;
            string weekend = objGlobalData.FunGetWeekend();
            try
            {
                for (DateTime fdt = frdt; fdt <= todt; fdt = fdt.AddDays(1.0))
                {

                    string dt = fdt.ToString("yyyy-MM-dd HH':'mm':'ss");
                    if (funCheckHoliday(dt) && funCheckWeekend(fdt, weekend))
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
        internal decimal Funcalculatetotalleave(string emp_no, DateTime frdt)
        {
            decimal bal = 0;
            string weekend = objGlobalData.FunGetWeekend();
            try
            {
                    DateTime fdt = frdt;

                    string dt = fdt.ToString("yyyy-MM-dd HH':'mm':'ss");
                    if (funCheckHoliday(dt) && funCheckWeekend(fdt, weekend))
                    {
                        bal = 0.5M;
                    }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in Funcalculatetotalleave: " + ex.ToString());
            }
            return bal;

        }
    }

    public class LeaveMgmtModelsList
    {
        public List<LeaveMgmtModels> lstleave { get; set; }
    }
}