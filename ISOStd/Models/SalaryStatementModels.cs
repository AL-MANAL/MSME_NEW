using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;

namespace ISOStd.Models
{
    public class SalaryStatementModels
    {
        clsGlobal objGlobalData = new clsGlobal();
       
        //[Required]
        [Display(Name = "SalaryStmtId")]
        public string SalaryStmtId { get; set; }

        [Required]
        [Display(Name = "Employee Code")]
        public string Emp_Code { get; set; }

        [Required]
        [Display(Name = "Employee Info. No.")]
        public string Emp_info_no { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string emp_firstname { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Required]
        [Display(Name = "Basic Salary")]
        public double Basic_Salary { get; set; }

        [Required]
        [Display(Name = "Accommodation Allowance")]
        public double Acc_allow { get; set; }

        [Required]
        [Display(Name = "Other Allowance")]
        public double Other_allow { get; set; }

        [Required]
        [Display(Name = "Food Allowance")]
        public double Food_allow { get; set; }

        [Required]
        [Display(Name = "Transport Allowance")]
        public double Transport_allow { get; set; }

        //[Required]
        [Display(Name = "Employee Name")]
        public string EmpId { get; set; }

        [Required]
        [Display(Name = "Salary Month")]
        public DateTime SalaryFor_Month { get; set; }

        [Required]
        [Display(Name = "Other Earnings")]
        public double Other_Earnings { get; set; }

        [Required]
        [Display(Name = "Bonus")]
        public double Bonus { get; set; }

        [Required]
        [Display(Name = "Salary Days")]
        public int Salary_Days { get; set; }

        [Required]
        [Display(Name = "Leave Days")]
        public int Leave_Days { get; set; }

        [Required]
        [Display(Name = "Normal OT Hrs.")]
        public int Normal_OT_Hrs { get; set; }

        [Required]
        [Display(Name = "Friday OT Hrs.")]
        public int Friday_OT_Hrs { get; set; }

        [Required]
        [Display(Name = "Public Holiday OT Hrs.")]
        public int Public_Holiday_OT_Hrs { get; set; }

        [Required]
        [Display(Name = "Other Income")]
        public double Other_Income { get; set; }

        [Required]
        [Display(Name = "Salary Advance")]
        public double Salary_Adv { get; set; }

        [Required]
        [Display(Name = "Miscellaneous Deduction")]
        public double Misslliance_Deduction { get; set; }

        [Required]
        [Display(Name = "ProcessedBy")]
        public string ProcessedBy { get; set; }

        internal bool FunAddSalaryStmt(SalaryStatementModels objSalaryStmtModels)
        {
            string sSqlstmt = "insert into t_emp_salarystmt (EmpId, SalaryFor_Month, Other_Earnings, Bonus, Salary_Days, Leave_Days, Normal_OT_Hrs, "
                +"Friday_OT_Hrs, Public_Holiday_OT_Hrs, Other_Income, Salary_Adv, ProcessedBy";
            string dtSalaryFor_Month = objSalaryStmtModels.SalaryFor_Month.ToString("yyyy/MM/dd HH:mm");

            sSqlstmt = sSqlstmt + ") values('" + objSalaryStmtModels.EmpId + "','" + dtSalaryFor_Month + "','" + objSalaryStmtModels.Other_Earnings
                + "', '" + objSalaryStmtModels.Bonus + "','" + objSalaryStmtModels.Salary_Days + "','" + objSalaryStmtModels.Leave_Days
                + "','" + objSalaryStmtModels.Normal_OT_Hrs + "','" + objSalaryStmtModels.Friday_OT_Hrs + "','" + objSalaryStmtModels.Public_Holiday_OT_Hrs
                + "','" + objSalaryStmtModels.Other_Income + "','" + objSalaryStmtModels.Salary_Adv + "','" + objGlobalData.GetCurrentUserSession().empid + "')";
            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        internal bool FunGenerateSalaryStmt(SalaryStatementModels objSalaryStatement)
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                string dtSalaryFor_Month = objSalaryStatement.SalaryFor_Month.ToString("yyyy/MM/dd");
                con.Open();
                MySqlCommand cmd = new MySqlCommand("GenerateSalaryStmt_Proc", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SalaryMonth", dtSalaryFor_Month);
                cmd.Parameters.AddWithValue("@ProcessedBy", objSalaryStatement.ProcessedBy);
                cmd.Parameters.AddWithValue("@SalaryDays", DateTime.DaysInMonth(objSalaryStatement.SalaryFor_Month.Year, objSalaryStatement.SalaryFor_Month.Month));

                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGenerateSalaryStmt: " + ex.ToString());
               return false;
            }
            finally
            {
                con.Close();
            }
            return true;
        }

        internal bool FunUpdateSalaryStmt(SalaryStatementModels objSalaryStmtModels)
        {
            //string dtSalaryFor_Month = objSalaryStmtModels.SalaryFor_Month.ToString("yyyy/MM/dd HH:mm");

            string sSqlstmt = "update t_emp_salarystmt set Other_Earnings='" + objSalaryStmtModels.Other_Earnings
                + "', Bonus='" + objSalaryStmtModels.Bonus + "', ProcessedBy='" + objGlobalData.GetCurrentUserSession().empid
                + "', Salary_Days='" + objSalaryStmtModels.Salary_Days + "', Leave_Days='" + objSalaryStmtModels.Leave_Days
                + "', Normal_OT_Hrs='" + objSalaryStmtModels.Normal_OT_Hrs + "', Friday_OT_Hrs='" + objSalaryStmtModels.Friday_OT_Hrs
                + "', Public_Holiday_OT_Hrs='" + objSalaryStmtModels.Public_Holiday_OT_Hrs + "', Other_Income='" + objSalaryStmtModels.Other_Income
                + "', Salary_Adv='" + objSalaryStmtModels.Salary_Adv + "', Misslliance_Deduction='" + objSalaryStmtModels.Misslliance_Deduction
                + "' where SalaryStmtId='" + objSalaryStmtModels.SalaryStmtId + "'";

            return objGlobalData.ExecuteQuery(sSqlstmt);
        }
    }

    public class SalaryStatementModelsList
    {
        public List<SalaryStatementModels> lstSalaryStatement { get; set; }
    }
}