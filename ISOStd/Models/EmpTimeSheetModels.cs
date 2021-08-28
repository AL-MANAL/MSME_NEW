using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class EmpTimeSheetModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string TimeSheet_Id { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string EmpId { get; set; }

         [Required]
        [Display(Name = "Company Name")]
        public string CustSuppId { get; set; }

         [Required]
         [Display(Name = "Date of")]
         public DateTime MonthOf { get; set; }

         [Required]
         [Display(Name = "First Half In Time")]
         public string FirstHalf_InTime { get; set; }

         [Required]
         [Display(Name = "First Half Out Time")]
         public string FirstHalf_OutTime { get; set; }

         [Required]
         [Display(Name = "Second Half In Time")]
         public string SecondHalf_InTime { get; set; }

         [Required]
         [Display(Name = "Second Half Out Time")]
         public string SecondHalf_OutTime { get; set; }

         [Required]
         [Display(Name = "Over Time")]
         public string OverTime { get; set; }

         [Required]
         [Display(Name = "Remarks")]
         public string Remarks { get; set; }


         internal bool FunAddTimeSheet(EmpTimeSheetModels objEmpTimeSheet, EmpTimeSheetModelslsList objlstEmpTimeSheetModels)
         {
             try
             {
                 string sSqlstmt = "";
                 string sMonthof = objEmpTimeSheet.MonthOf.ToString("yyyy/MM/dd");

                 for (int i = 0; i < objlstEmpTimeSheetModels.EmpTimeSheetMList.Count; i++)
                 {
                     sSqlstmt = sSqlstmt + "insert into t_emp_timesheet (EmpId, CustSuppId, MonthOf, FirstHalf_InTime, FirstHalf_OutTime, SecondHalf_InTime, SecondHalf_OutTime"
                          + ", OverTime, Remarks";

                     sSqlstmt = sSqlstmt + ") values('" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].EmpId + "','" + objEmpTimeSheet.CustSuppId
                         + "','" + sMonthof
                             + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].FirstHalf_InTime + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].FirstHalf_OutTime
                             + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].SecondHalf_InTime + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].SecondHalf_OutTime
                             + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].OverTime + "','" + objlstEmpTimeSheetModels.EmpTimeSheetMList[i].Remarks + "'); ";
                 }

                 if (sSqlstmt != "")
                 {
                     return objGlobalData.ExecuteQuery(sSqlstmt);
                 }
             }
             catch (Exception ex)
             {
                 objGlobalData.AddFunctionalLog("Exception in FunAddTimeSheet: " + ex.ToString());
             }
             return false;
         }
       

        
    }
    public class EmpTimeSheetModelslsList
    {
        public List<EmpTimeSheetModels> EmpTimeSheetMList { get; set; }
    }

    public class EmpTimeSheetReportFieldModels
    {
        public string sNumofDays { get; set; }
        public string sNumOfWorkingDays { get; set; }
        public string sNumOfHoliday { get; set; }
        public string sNumOfOffDays { get; set; }
        public string sSickLeave { get; set; }
        public string sAbsent { get; set; }
        public string sNormalOverTime { get; set; }
        public string sHolidayOverTime { get; set; }
        public string sDeduction { get; set; }
    }
    
}