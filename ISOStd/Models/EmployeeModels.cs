using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class EmployeeModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "ID")]
        public string EmpID { get; set; }

        [Display(Name = "Emp ID")]
        [System.Web.Mvc.Remote("doesEmpIDExist", "Employee", HttpMethod = "POST", ErrorMessage = "Employee ID already exists. Please enter a different ID.")]
        public string CompEmpId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Active")]
        public int Active { get; set; }

        // [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [System.Web.Mvc.Remote("doesEmailIDExist", "Employee", HttpMethod = "POST", ErrorMessage = "Email ID already exists. Please enter a different ID.")]
        public string emailAddress { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string DeptID { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Required]
        [Display(Name = "DeptInCharge")]
        public string DeptInCharge { get; set; }

        //[Required]
        //[Display(Name = "Emp Type")]
        //[DisplayFormat(NullDisplayText = "Emp Type")]
        //public string EmpType { get; set; }

        ////[Required]
        //[Display(Name = "Profile Pic")]
        //public string ProfilePic { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Pwd { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Pwd", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPwd { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string OldPwd { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name = "Work Location")]
        public string Work_Location { get; set; }

        public string division { get; set; }

        internal bool FunRegisterUser(EmployeeModels objEmployeeModel)
        {
            try
            {
               // string sBranch = objGlobalData.GetCurrentUserSession().Work_Location;

                string sSqlstmt = "insert into t_employee (CompEmpId,emailAddress";

                //if (objEmployeeModel.ProfilePic != null && objEmployeeModel.ProfilePic != "")
                //{
                //    sSqlstmt = sSqlstmt + ", ProfilePic";
                //}

                sSqlstmt = sSqlstmt + ") values('" + objEmployeeModel.CompEmpId + "','" + objEmployeeModel.emailAddress + "'";

                //if (objEmployeeModel.ProfilePic != null && objEmployeeModel.ProfilePic != "")
                //{
                //    sSqlstmt = sSqlstmt + ", '" + objEmployeeModel.ProfilePic + "'";
                //}

                sSqlstmt = sSqlstmt + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return MailTempPassword(objEmployeeModel.emailAddress, objEmployeeModel.FirstName);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunRegisterUser: " + ex.ToString());

            }
            return false;
        }

        internal bool FunUpdateUser(EmployeeModels objEmployeeModel)
        {
            try
            {
                //string sSqlstmt = "update t_employee set LastName='" + objEmployeeModel.LastName + "', "
                //    + "FirstName='" + objEmployeeModel.FirstName + "', MiddleName='" + objEmployeeModel.MiddleName + "', DeptID='" + objEmployeeModel.DeptID
                //    + "', Designation='" + objEmployeeModel.Designation + "', DeptInCharge='"
                //    + objEmployeeModel.DeptInCharge[0] + "', EmpType='" + objEmployeeModel.EmpType[0] + "', Role='" + objEmployeeModel.Role
                //    + "', Work_Location='" + objEmployeeModel.Work_Location + "', emailAddress='" + objEmployeeModel.emailAddress+ "'";

                string sSqlstmt = "update t_employee set  Role='" + objEmployeeModel.Role + "'";

                //if (objEmployeeModel.ProfilePic != null && objEmployeeModel.ProfilePic != "")
                //{
                //    sSqlstmt = sSqlstmt + ", ProfilePic='" + objEmployeeModel.ProfilePic + "'";
                //}

                sSqlstmt = sSqlstmt + " where EmpID=" + objEmployeeModel.EmpID;
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateUser: " + ex.ToString());

            }
            return false;
        }

        internal bool FunDeleteEmployee(string EmpID)
        {
            try
            {
                string sSqlstmt = "update t_employee set active=0 where EmpID=" + EmpID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEmployee: " + ex.ToString());

            }
            return false;
        }

        internal bool FunActivateEmployee(string EmpID)
        {
            try
            {
                string sSqlstmt = "update t_employee set active=1 where EmpID=" + EmpID;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunActivateEmployee: " + ex.ToString());

            }
            return false;
        }

        internal bool FunPwdReset(string EmpID, string sPwd)
        {
            try
            {
                string today = DateTime.Now.ToString("yyyy/MM/dd");
                string sSqlstmt = "update t_employee set pwd='" + sPwd + "', logged_date='" + today + "' where CompEmpId=" + EmpID + " and active=1";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunPwdReset: " + ex.ToString());

            }
            return false;
        }


        public bool MailTempPassword(string sEmailid, string sUsername)
        {
            string sTempPassword = objGlobalData.GenerateTempPassword();

            string sSqlstmt = "update t_employee set pwd='" + clsGlobal.Encrypt(sTempPassword) + "' where emailaddress='" + sEmailid + "'";

            if (objGlobalData.ExecuteQuery(sSqlstmt))
            {
                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUsername, "passwordreset", "Password: " + sTempPassword);
                objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "");
            }

            return true;
        }


    }

    public class EmployeeModelList
    {
        public List<EmployeeModels> EmployeeList { get; set; }
    }
}