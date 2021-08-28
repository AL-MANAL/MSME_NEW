using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;


namespace ISOStd.Models
{
    public class SalaryAdvanceDeductionModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Advance Id")]
        public string Advance_AmtId { get; set; }

        //[Required]
        [Display(Name = "Employee Name")]
        public string EmpId { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public double Adv_Amt { get; set; }

        [Required]
        [Display(Name = "Number of Installment (Months)")]
        public int Num_Installment { get; set; }

        [Required]
        [Display(Name = "Deduction Amount")]
        public double Deduction_Amt { get; set; }

        [Required]
        [Display(Name = "Details")]
        public string Details { get; set; }

        [Required]
        [Display(Name = "Balance Amount")]
        public double Balance_Amt { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime TransDate { get; set; }

        [Required]
        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [Required]
        [Display(Name = "Deducted Amount")]
        public double Deduct_Amt { get; set; }

         [Required]
         [Display(Name = "Deduction Id")]
        public string Adv_DeductionId { get; set; }


        internal bool FunAddAdvanceAmt(SalaryAdvanceDeductionModels objSalaryModels)
        {
            string sSqlstmt = "insert into t_advance_amt (Adv_Amt, EmpId, Num_Installment, Details, Balance_Amt, TransDate, ProcessedBy";
            string dtPaidDate = objSalaryModels.TransDate.ToString("yyyy/MM/dd HH:mm");

            sSqlstmt = sSqlstmt + ") values('" + objSalaryModels.Adv_Amt + "','" + objSalaryModels.EmpId + "','" + objSalaryModels.Num_Installment
                + "', '" + objSalaryModels.Details + "','" + objSalaryModels.Adv_Amt + "','" + dtPaidDate + "','"
                + objGlobalData.GetCurrentUserSession().empid + "')";
            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        internal bool FunUpdateAdvanceAmt(SalaryAdvanceDeductionModels objSalaryModels)
        {
            string dtPaidDate = objSalaryModels.TransDate.ToString("yyyy/MM/dd HH:mm");

            string sSqlstmt = "update t_advance_amt set Adv_Amt='" + objSalaryModels.Adv_Amt + "', EmpId='" + objSalaryModels.EmpId
                + "', Num_Installment='" + objSalaryModels.Num_Installment + "', ProcessedBy='" + objGlobalData.GetCurrentUserSession().empid 
                + "', Details='" + objSalaryModels.Details + "' where Advance_AmtId='" + objSalaryModels.Advance_AmtId + "'";

            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        internal int GetInstallmentDuration(string sEmpid)
        {
            DateTime dtExpiryDate = new DateTime();

            DataSet dsEmp = objGlobalData.Getdetails("select Eid_visa_date from t_hr_employee where emp_no='" + sEmpid + "'");

            if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0 && DateTime.TryParse(dsEmp.Tables[0].Rows[0]["Eid_visa_date"].ToString(), out dtExpiryDate))
            {
                return (((dtExpiryDate.Year - DateTime.Now.Year) * 12) + dtExpiryDate.Month - DateTime.Now.Month)-1;
            }
            return 0;
        }

        internal bool FunAddDeductionAmt(SalaryAdvanceDeductionModels objSalaryModels)
        {
            string sSqlstmt = "insert into t_adv_deduction (Deduct_Amt, Emp_Id, TransDate, Details, ProcessedBy";
            string sTransDate = objSalaryModels.TransDate.ToString("yyyy/MM/dd HH:mm");

            sSqlstmt = sSqlstmt + ") values('" + objSalaryModels.Adv_Amt + "','" + objSalaryModels.EmpId + "','" + sTransDate
                + "', '" + objSalaryModels.Details + "','" + objGlobalData.GetCurrentUserSession().empid + "')";
            return objGlobalData.ExecuteQuery(sSqlstmt);
        }

        internal bool FunUpdateDeductionAmt(SalaryAdvanceDeductionModels objSalaryModels)
        {
            string sTransDate = objSalaryModels.TransDate.ToString("yyyy/MM/dd HH:mm");

            string sSqlstmt = "update t_adv_deduction set Deduct_Amt='" + objSalaryModels.Deduct_Amt + "', EmpId='" + objSalaryModels.EmpId + "', TransDate='" + sTransDate
                + "', Details='" + objSalaryModels.Details + "' where Adv_DeductionId='" + objSalaryModels.Adv_DeductionId + "'";

            return objGlobalData.ExecuteQuery(sSqlstmt);
        }
    }

    public class SalaryAdvanceDeductionModelsList
    {
        public List<SalaryAdvanceDeductionModels> lstSalaryAdvanceDeduction { get; set; }
    }
}