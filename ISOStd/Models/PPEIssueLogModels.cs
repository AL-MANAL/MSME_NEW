using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class PPEIssueLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string IssueLog_Id { get; set; }
        
        [Display(Name = "Date of Issue")]
        public DateTime Issue_Date { get; set; }
        
        [Display(Name = "Receiver Name")]
        public string Receiver_Name { get; set; }
        
        [Display(Name = "Position")]
        public string Position { get; set; }
        
        [Display(Name = "Customer Project Name")]
        public string Cust_Project_Name { get; set; }
        
        //[DataType(DataType.MultilineText)]
        [Display(Name = "Location")]
        public string Work_Location { get; set; }
        
        [Display(Name = "PPE Issued")]
        public string PPE_Issued { get; set; }
        
        [Display(Name = "Last Date on which PPE Issued")]
        public DateTime PPE_Issued_Last_Date { get; set; }

        [Display(Name = "Issued By")]
        public string Issued_By { get; set; }

        [Display(Name = "Upload the PPE Issue Voucher")]
        public string PPE_Issue_Voucher { get; set; }
        
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Details of PPE")]
        public string ppe_detail { get; set; }


        [Display(Name = "Quantity")]
        public string ppe_qty { get; set; }

        public string checkEmployeePPEExists(string Receiver_Name)
        {
            try
            {
                string sSqlstmt = "select IssueLog_Id from t_ppe_issuelog where Receiver_Name='" + Receiver_Name + "' and active=1";
                DataSet dsEmp =objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return "false";
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkEmployeePPEExists: " + ex.ToString());
            }
            return Receiver_Name;
        }

        internal bool FunDeletePPELogDoc(string sIssueLog_Id)
        {
            try
            {
                string sSqlstmt = "update t_ppe_issuelog set Active=0 where IssueLog_Id='" + sIssueLog_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePPELogDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddPPEIssueLog(PPEIssueLogModels objPPEIssueLog)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_ppe_issuelog (Receiver_Name, Position, Cust_Project_Name, Work_Location, PPE_Issued, "
                    + " Issued_By, LoggedBy,branch,Department,ppe_detail,ppe_qty";
                string user = "";               
                user = objGlobalData.GetCurrentUserSession().empid;
               // string sBranch = objGlobalData.GetCurrentUserSession().division;

                if (objPPEIssueLog.Issue_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Issue_Date";
                    sValues = sValues + ", '" + objPPEIssueLog.Issue_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objPPEIssueLog.PPE_Issued_Last_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", PPE_Issued_Last_Date";
                    sValues = sValues + ", '" + objPPEIssueLog.PPE_Issued_Last_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objPPEIssueLog.PPE_Issue_Voucher != null && objPPEIssueLog.PPE_Issue_Voucher != "")
                {
                    sColumn = sColumn + ", PPE_Issue_Voucher";
                    sValues = sValues + ", '" + objPPEIssueLog.PPE_Issue_Voucher + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objPPEIssueLog.Receiver_Name + "','" + objPPEIssueLog.Position
                    + "','" + objPPEIssueLog.Cust_Project_Name + "','" + objPPEIssueLog.Work_Location + "','" + objPPEIssueLog.PPE_Issued + "','" + objPPEIssueLog.Issued_By
                 + "','" + user + "','" + objPPEIssueLog.branch + "','" + objPPEIssueLog.Department + "','" + objPPEIssueLog.ppe_detail + "','" + objPPEIssueLog.ppe_qty + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPPEIssueLog: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdatePPEIssueLog(PPEIssueLogModels objPPEIssueLog)
        {
            try
            {
                string sSqlstmt = "update t_ppe_issuelog set  Position='" + objPPEIssueLog.Position
                    + "', Cust_Project_Name='" + objPPEIssueLog.Cust_Project_Name + "', Work_Location='" + objPPEIssueLog.Work_Location
                    + "', PPE_Issued='" + objPPEIssueLog.PPE_Issued + "', Issued_By='" + objPPEIssueLog.Issued_By + "', Department='" + objPPEIssueLog.Department + "', branch='" + objPPEIssueLog.branch + "', ppe_detail='" + objPPEIssueLog.ppe_detail + "', ppe_qty='" + objPPEIssueLog.ppe_qty + "'";

                if (objPPEIssueLog.Issue_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Issue_Date='" + objPPEIssueLog.Issue_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objPPEIssueLog.PPE_Issued_Last_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", PPE_Issued_Last_Date='" + objPPEIssueLog.PPE_Issued_Last_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objPPEIssueLog.PPE_Issue_Voucher != null && objPPEIssueLog.PPE_Issue_Voucher != "")
                {
                    sSqlstmt = sSqlstmt + ", PPE_Issue_Voucher='" + objPPEIssueLog.PPE_Issue_Voucher + "' ";
                }

                sSqlstmt = sSqlstmt + " where IssueLog_Id='" + objPPEIssueLog.IssueLog_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePPEIssueLog: " + ex.ToString());
            }
            return false;
        }
    }

    public class PPEIssueLogModelsList
    {
        public List<PPEIssueLogModels> lstPPEIssueLog { get; set; }
    }
}