using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;


namespace ISOStd.Models
{
    public class EmployeeMasterModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string emp_no { get; set; }

        // [Required]
        [Display(Name = "Employee Id")]
        //[System.Web.Mvc.Remote("doesEmpIDExist", "EmployeeDetails", HttpMethod = "POST", ErrorMessage = "Employee ID already exists. Please enter a different ID.")]
        public string emp_id { get; set; }

        // [Required]
        [Display(Name = "First Name")]
        public string emp_firstname { get; set; }

        //[Required]
        [Display(Name = "Middle Name")]
        public string emp_middlename { get; set; }

        // [Required]
        [Display(Name = "Last Name")]
        public string emp_lastname { get; set; }

        // [Required]
        [Display(Name = "Status")]
        public int emp_status { get; set; }

        // [Required]
        [Display(Name = "Nationality")]
        public string Nationaliity { get; set; }

        // [Required]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        // [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        // [Required]
        [Display(Name = "Date of Birth")]
        public DateTime Date_of_Birth { get; set; }

        // [Required]
        [Display(Name = "Marital status")]
        public string Marital_status { get; set; }

        //  [Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        [Display(Name = "Email Address")]
        //[System.Web.Mvc.Remote("doesEmailIDExist", "EmployeeDetails", HttpMethod = "POST", ErrorMessage = "Email ID already exists. Please enter a different ID.")]
        public string EmailId { get; set; }

        // [Required]
        [Display(Name = "Department")]
        public string dept_id { get; set; }

        public string DeptId { get; set; }

        // [Required]
        [Display(Name = "Department In charge")]
        public string DeptInCharge { get; set; }

        // [Required]
        [Display(Name = "Reporting to")]
        public string ReportingTo { get; set; }

        // [Required]
        [Display(Name = "Mobile No.")]
        public string MobileNo { get; set; }

        //[Required]
        [Display(Name = "UID No.")]
        public string UID_no { get; set; }

        //[Required]
        [Display(Name = "Visa Type")]
        public string Visa_Type { get; set; }

        //[Required]
        [Display(Name = "Visa No.")]
        public string Visa_no { get; set; }

        //[Required]
        [Display(Name = "Visa Stamp Date")]
        public DateTime Visa_stamped_on { get; set; }

        //[Required]
        [Display(Name = "Emirates/Saudi ID No.(Iqama No.)")]
        public string Eid_no { get; set; }

        //[Required]
        [Display(Name = "Employee Info No.")]
        public string Emp_info_no { get; set; }

        // [Required]
        [Display(Name = "Passport No.")]
        public string Passport_no { get; set; }

        //[Required]
        [Display(Name = "Passport Expiry Date")]
        public DateTime Passport_expiry { get; set; }

        // [Required]
        [Display(Name = "Labour Card No.")]
        public string Labour_cardno { get; set; }

        //[Required]
        [Display(Name = "Labour Card Expiry Date")]
        public DateTime Labour_cardexpiry { get; set; }

        //[Required]
        [Display(Name = "EID Expiry Date")]
        public DateTime Eid_visa_date { get; set; }

        //[Required]
        [Display(Name = "Visa Expiry Date")]
        public DateTime visa_Exp_date { get; set; }

        // [Required]
        [Display(Name = "Health Insurance Provider")]
        public string Health_insurance_provider { get; set; }

        //  [Required]
        [Display(Name = "Health Insurance Expiry Date")]
        public DateTime Health_Insurance_Expiry { get; set; }

        //[Required]
        [Display(Name = "Local Contact No.")]
        public string Emp_local_contact { get; set; }

        // [Required]
        [Display(Name = "Home Contact No.")]
        public string Emp_native_phoneno { get; set; }

        // [Required]
        [Display(Name = "Native Country")]
        public string Emp_native_country { get; set; }

        //[Required]
        [Display(Name = "Work Location")]
        public string Emp_work_location { get; set; }

        //[Required]
        [Display(Name = "Accomodation")]
        public string Emp_accomodation { get; set; }

        // [Required]
        [Display(Name = "Date of Joining")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date_of_join { get; set; }

        //[Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Exit")]
        public DateTime Date_of_exit { get; set; }

        //[Required]
        [Display(Name = "Basic Salary")]
        public decimal Basic_Salary { get; set; }

        // [Required]
        [Display(Name = "Home Allowance")]
        public decimal Acc_allow { get; set; }

        // [Required]
        [Display(Name = "Other Allowance")]
        public decimal Other_allow { get; set; }

        [Display(Name = "Gratuity")]
        public decimal Gratuity { get; set; }

        [Display(Name = "Food Allowance")]
        public decimal Food_allow { get; set; }

        [Display(Name = "Transport Allowance")]
        public decimal Transport_allow { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Custody Documents")]
        public string Custody_Documents { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Profile Pic.")]
        public string ProfilePic { get; set; }

        [Display(Name = "Job Description")]
        public string JobDesc { get; set; }

        [Display(Name = "EvaluatedBy")]
        public string EvaluatedBy { get; set; }

        [Display(Name = "CompetancyDoc")]
        public string CompetancyDoc { get; set; }

        [Display(Name = "Competancy Date from")]
        public DateTime CompetancyFromDate { get; set; }

        [Display(Name = "Competancy Date to")]
        public DateTime CompetancyToDate { get; set; }

        [Display(Name = "ID")]
        public string chartId { get; set; }

        [Display(Name = "Chart Name")]
        public string ChartName { get; set; }

        // [Required]
        [Display(Name = "Doc Upload")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Passport Upload")]
        public string PassportUpload { get; set; }

        [Display(Name = "EID Upload")]
        public string EIDUpload { get; set; }

        [Display(Name = "Health Card Upload")]
        public string HealthCardUpload { get; set; }

        [Display(Name = "Health Card Issue Date")]
        public DateTime HealthCardIssueDate { get; set; }

        [Display(Name = "Health Card Expiry Date")]
        public DateTime HealthCardExpDate { get; set; }

        [Display(Name = "Updated On")]
        public DateTime Logged_date { get; set; }

        [Display(Name = "Visa Upload")]
        public string Visa_upload { get; set; }

        [Display(Name = "Division")]
        public string division { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Date of Upload")]
        public DateTime upload_date { get; set; }
        public DateTime upload_error_date { get; set; }

        [Display(Name = "Error found to upload")]
        public string error_found { get; set; }

        //t_hr_employee
        [Display(Name = "Employment Type")]
        public string employment_type { get; set; }

        [Display(Name = "Qualification")]
        public string qualification { get; set; }

        [Display(Name = "Years of Experience")]
        public string years_exp { get; set; }

        //t_hr_employee_skills

        [Display(Name = "Id")]
        public string id_emp_qualification { get; set; }

        [Display(Name = "Year Passed")]
        public string q_year { get; set; }

        [Display(Name = "Upload Document")]
        public string upload { get; set; }

        //t_hr_employee_skills

        [Display(Name = "Id")]
        public string id_emp_skill { get; set; }

        [Display(Name = "Skills")]
        public string skill { get; set; }

        //t_hr_employee_training

        [Display(Name = "Id")]
        public string id_training { get; set; }

        [Display(Name = "Type of Training")]
        public string training_type { get; set; }

        [Display(Name = "Training Duration in Days")]
        public string duration { get; set; }

        [Display(Name = "Training Completed On")]
        public DateTime completed_date { get; set; }

        [Display(Name = "Document(s)")]
        public string training_upload { get; set; }

        internal bool FunAddCompetenceDetails(EmployeeMasterModelList objQModelsList, EmployeeMasterModelList objSkillModelsList, EmployeeMasterModelList objTModelsList)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee set years_exp='"+years_exp+ "' where emp_no='" + emp_no + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);


                if (Convert.ToInt32(objQModelsList.EmployeeList.Count) > 0)
                {
                    objQModelsList.EmployeeList[0].emp_no = emp_no.ToString();
                    FunAddQualificationList(objQModelsList);
                }
                if (Convert.ToInt32(objSkillModelsList.EmployeeList.Count) > 0)
                {
                    objSkillModelsList.EmployeeList[0].emp_no = emp_no.ToString();
                    FunAddSkillList(objSkillModelsList);
                }
                if (Convert.ToInt32(objTModelsList.EmployeeList.Count) > 0)
                {
                    objTModelsList.EmployeeList[0].emp_no = emp_no.ToString();
                    FunAddTrainingList(objTModelsList);
                }
                return true;


            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCompetenceDetails: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddTrainingList(EmployeeMasterModelList objTModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_hr_employee_training where emp_no='" + objTModelsList.EmployeeList[0].emp_no + "'; ";

                for (int i = 0; i < objTModelsList.EmployeeList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_hr_employee_training(emp_no,training_type,duration,training_upload";

                    string sFieldValue = "", sFields = "";
                    if (objTModelsList.EmployeeList[i].completed_date != null && objTModelsList.EmployeeList[i].completed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", completed_date";
                        sFieldValue = sFieldValue + ", '" + objTModelsList.EmployeeList[i].completed_date.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objTModelsList.EmployeeList[0].emp_no + "', '" + objTModelsList.EmployeeList[i].training_type + "', '" + objTModelsList.EmployeeList[i].duration + "', '" + objTModelsList.EmployeeList[i].training_upload + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingList: " + ex.ToString());
            }
            return false;
        }


        internal bool FunAddQualificationList(EmployeeMasterModelList objQModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_hr_employee_qualification where emp_no='" + objQModelsList.EmployeeList[0].emp_no + "'; ";

                for (int i = 0; i < objQModelsList.EmployeeList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_hr_employee_qualification(emp_no,qualification,q_year,upload";

                    string sFieldValue = "", sFields = "";
                   
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objQModelsList.EmployeeList[0].emp_no + "', '" + objQModelsList.EmployeeList[i].qualification + "', '" + objQModelsList.EmployeeList[i].q_year + "', '" + objQModelsList.EmployeeList[i].upload + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddQualificationList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddSkillList(EmployeeMasterModelList objQModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_hr_employee_skills where emp_no='" + objQModelsList.EmployeeList[0].emp_no + "'; ";

                for (int i = 0; i < objQModelsList.EmployeeList.Count; i++)
                {

                    sSqlstmt = sSqlstmt + "insert into t_hr_employee_skills(emp_no,skill";

                    string sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objQModelsList.EmployeeList[0].emp_no + "', '" + objQModelsList.EmployeeList[i].skill + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSkillList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteChart(string schartId)
        {
            try
            {
                string sSqlstmt = "update orgcharts set Status=0 where chartId='" + schartId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChart: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddOrgChart(EmployeeMasterModels objEmployeeModel)
        {
            try
            {
                string sSqlstmt = "insert into orgcharts (ChartName,DocUploadPath) values('" + objEmployeeModel.ChartName + "','" + objEmployeeModel.DocUploadPath + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddOrgChart: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateOrgChart(EmployeeMasterModels objEmployeeModel)
        {
            try
            {
                string sSqlstmt = "update orgcharts set ChartName='" + objEmployeeModel.ChartName + "'";
                if (objEmployeeModel.DocUploadPath != null && objEmployeeModel.DocUploadPath != "")
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objEmployeeModel.DocUploadPath + "'";
                }
                sSqlstmt = sSqlstmt + " where chartId='" + objEmployeeModel.chartId + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                   
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateOrgChart: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddEmployeeMaster(EmployeeMasterModels objEmployeeModel,System.IO.FileStream[] fsSource)
        {
            try
            {
                string sCompetancyFromDate = objEmployeeModel.CompetancyFromDate.ToString("yyyy/MM/dd");
                string sCompetancyToDate = objEmployeeModel.CompetancyToDate.ToString("yyyy/MM/dd");
                //string sBranch = objGlobalData.GetCurrentUserSession().Work_Location;

                string sSqlstmt = "insert into t_hr_employee (emp_id, emp_lastname, emp_firstname, emp_middlename, Nationaliity, Designation, Gender, Marital_status,"
                        + " EmailId, MobileNo, UID_no, Visa_Type, Visa_no, Eid_no, Emp_info_no, Passport_no, Labour_cardno, Health_insurance_provider, Emp_local_contact,"
                        + " Emp_native_phoneno, Emp_native_country, Emp_work_location, Emp_accomodation, Basic_Salary, Acc_allow, Other_allow, Gratuity, Remarks,"
                        + " Custody_Documents, Date_of_Birth, dept_id, JobDesc, EvaluatedBy, CompetancyFromDate, CompetancyToDate, CompetancyDoc, Food_allow, Transport_allow,DeptInCharge,division,Role,employment_type,qualification,years_exp";

                string sFields = "", sFieldValue = "";

                if (objEmployeeModel.Passport_expiry != null && objEmployeeModel.Passport_expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Passport_expiry";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Passport_expiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Labour_cardexpiry != null && objEmployeeModel.Labour_cardexpiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Labour_cardexpiry";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Labour_cardexpiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Eid_visa_date != null && objEmployeeModel.Eid_visa_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Eid_visa_date";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Eid_visa_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.visa_Exp_date != null && objEmployeeModel.visa_Exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", visa_Exp_date";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.visa_Exp_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Visa_stamped_on != null && objEmployeeModel.Visa_stamped_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Visa_stamped_on";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Visa_stamped_on.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Health_Insurance_Expiry != null && objEmployeeModel.Health_Insurance_Expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Health_Insurance_Expiry";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Health_Insurance_Expiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Date_of_join != null && objEmployeeModel.Date_of_join > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Date_of_join";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Date_of_join.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Date_of_exit != null && objEmployeeModel.Date_of_exit > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Date_of_exit";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Date_of_exit.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.HealthCardIssueDate != null && objEmployeeModel.HealthCardIssueDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", HealthCardIssueDate";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.HealthCardIssueDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.HealthCardExpDate != null && objEmployeeModel.HealthCardExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", HealthCardExpDate";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.HealthCardExpDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.ProfilePic != null && objEmployeeModel.ProfilePic != "")
                {
                    sFields = sFields + ", ProfilePic";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.ProfilePic + "'";
                }
                if (objEmployeeModel.PassportUpload != null && objEmployeeModel.PassportUpload != "")
                {
                    sFields = sFields + ", PassportUpload";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.PassportUpload + "'";
                }
                if (objEmployeeModel.EIDUpload != null && objEmployeeModel.EIDUpload != "")
                {
                    sFields = sFields + ", EIDUpload";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.EIDUpload + "'";
                }
                if (objEmployeeModel.HealthCardUpload != null && objEmployeeModel.HealthCardUpload != "")
                {
                    sFields = sFields + ", HealthCardUpload";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.HealthCardUpload + "'";
                }
                if (objEmployeeModel.Visa_upload != null && objEmployeeModel.Visa_upload != "")
                {
                    sFields = sFields + ", Visa_upload";
                    sFieldValue = sFieldValue + ", '" + objEmployeeModel.Visa_upload + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objEmployeeModel.emp_id + "','" + objEmployeeModel.emp_lastname + "','" + objEmployeeModel.emp_firstname
                 + "','" + objEmployeeModel.emp_middlename + "','" + objEmployeeModel.Nationaliity + "','" + objEmployeeModel.Designation
                 + "','" + objEmployeeModel.Gender + "','" + objEmployeeModel.Marital_status + "','" + objEmployeeModel.EmailId + "','" + objEmployeeModel.MobileNo
                 + "','" + objEmployeeModel.UID_no + "','" + objEmployeeModel.Visa_Type + "','" + objEmployeeModel.Visa_no + "','" + objEmployeeModel.Eid_no
                 + "','" + objEmployeeModel.Emp_info_no + "','" + objEmployeeModel.Passport_no + "','" + objEmployeeModel.Labour_cardno
                 + "','" + objEmployeeModel.Health_insurance_provider + "','" + objEmployeeModel.Emp_local_contact
                 + "','" + objEmployeeModel.Emp_native_phoneno + "','" + objEmployeeModel.Emp_native_country + "','" + objEmployeeModel.Emp_work_location
                 + "','" + objEmployeeModel.Emp_accomodation + "','" + objEmployeeModel.Basic_Salary + "','" + objEmployeeModel.Acc_allow + "','" + objEmployeeModel.Other_allow
                 + "','" + objEmployeeModel.Gratuity + "','" + objEmployeeModel.Remarks + "','" + objEmployeeModel.Custody_Documents
                 + "','" + objEmployeeModel.Date_of_Birth.ToString("yyyy/MM/dd") + "','" + objEmployeeModel.dept_id + "','" + objEmployeeModel.JobDesc
                 + "','" + objEmployeeModel.EvaluatedBy + "','" + sCompetancyFromDate + "','" + sCompetancyToDate + "','" + objEmployeeModel.CompetancyDoc
                 + "','" + objEmployeeModel.Food_allow + "','" + objEmployeeModel.Transport_allow + "','" + objEmployeeModel.DeptInCharge + "','" + objEmployeeModel.division + "','" + objEmployeeModel.Role + "','" + employment_type + "','" + qualification + "','" + years_exp + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendEmail(objEmployeeModel, fsSource);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmployeeMaster: " + ex.ToString());
            }
            return false;
        }
        internal bool SendEmail(EmployeeMasterModels objModel,System.IO.FileStream[] fsSource)
        {
            try
            {
                string sInformation = "", sHeader = "", sToEmailId = "", sCCList = "", sUserName = "";
                sCCList = objModel.EmailId;
                sToEmailId = objGlobalData.GetMultiHrEmpEmailIdById(objModel.EvaluatedBy);
                sUserName = objGlobalData.GetMultiHrEmpNameById(objModel.EvaluatedBy);

                sInformation =
                "Employee Details"
                + " <br />"
                + "Emp Name:'" + objModel.emp_firstname + "'"
                + " <br />"
                + "Designation:'" + objModel.Designation + "'"
                +" <br />"
               + "Role:'" +objGlobalData.GetMultiRoleById(objModel.Role) + "'"
                +" <br />"
               + "Date:'" + DateTime.Today.ToString("dd/MM/yyyy") + "'";

                sHeader = sHeader + sInformation;

                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "JDAssaignment", sHeader, "", "Job Description");    
                objGlobalData.SendEmployeemail(sToEmailId, fsSource, dicEmailContent["subject"], dicEmailContent["body"], sCCList, "");
                return true;

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateEmployeeMaster(EmployeeMasterModels objEmployeeModel)
        {
            try
            {
                string sCompetancyFromDate = objEmployeeModel.CompetancyFromDate.ToString("yyyy/MM/dd");
                string sCompetancyToDate = objEmployeeModel.CompetancyToDate.ToString("yyyy/MM/dd");
                string sLogged_date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                string sSqlstmt = "update t_hr_employee set emp_lastname='" + objEmployeeModel.emp_lastname + "', emp_firstname='" + objEmployeeModel.emp_firstname
                 + "', emp_middlename='" + objEmployeeModel.emp_middlename + "', Nationaliity='" + objEmployeeModel.Nationaliity
                 + "', Designation='" + objEmployeeModel.Designation + "', Gender='" + objEmployeeModel.Gender + "', Marital_status='" + objEmployeeModel.Marital_status
                 + "', EmailId='" + objEmployeeModel.EmailId + "', MobileNo='" + objEmployeeModel.MobileNo + "', UID_no='" + objEmployeeModel.UID_no
                 + "', Visa_Type='" + objEmployeeModel.Visa_Type + "', Visa_no='" + objEmployeeModel.Visa_no + "', Eid_no='" + objEmployeeModel.Eid_no
                 + "', Emp_info_no='" + objEmployeeModel.Emp_info_no + "', Passport_no='" + objEmployeeModel.Passport_no + "', Labour_cardno='" + objEmployeeModel.Labour_cardno
                 + "', Health_insurance_provider='" + objEmployeeModel.Health_insurance_provider + "', Emp_local_contact='" + objEmployeeModel.Emp_local_contact
                 + "', Emp_native_phoneno='" + objEmployeeModel.Emp_native_phoneno + "', Emp_native_country='" + objEmployeeModel.Emp_native_country
                 + "', Emp_work_location='" + objEmployeeModel.Emp_work_location + "', Emp_accomodation='" + objEmployeeModel.Emp_accomodation
                 + "', Basic_Salary='" + objEmployeeModel.Basic_Salary + "', Acc_allow='" + objEmployeeModel.Acc_allow + "', Other_allow='" + objEmployeeModel.Other_allow
                 + "', Gratuity='" + objEmployeeModel.Gratuity + "', Remarks='" + objEmployeeModel.Remarks + "', Custody_Documents='" + objEmployeeModel.Custody_Documents
                 + "', Date_of_Birth='" + objEmployeeModel.Date_of_Birth.ToString("yyyy/MM/dd") + "', dept_id='" + objEmployeeModel.dept_id
                  + "', EvaluatedBy='" + objEmployeeModel.EvaluatedBy + "', CompetancyFromDate='" + sCompetancyFromDate
                  + "', CompetancyToDate='" + sCompetancyToDate + "', Food_allow='" + objEmployeeModel.Food_allow + "', Transport_allow='" + objEmployeeModel.Transport_allow + "'"
                + ",Logged_date='" + sLogged_date + "',DeptInCharge='" + DeptInCharge + "',emp_id='" + emp_id + "',division='" + division + "',Role='" + objEmployeeModel.Role + "',employment_type='" + employment_type + "',qualification='" + qualification + "',years_exp='" + years_exp + "'";

                if (objEmployeeModel.Passport_expiry != null && objEmployeeModel.Passport_expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Passport_expiry='" + objEmployeeModel.Passport_expiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Labour_cardexpiry != null && objEmployeeModel.Labour_cardexpiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Labour_cardexpiry='" + objEmployeeModel.Labour_cardexpiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Eid_visa_date != null && objEmployeeModel.Eid_visa_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Eid_visa_date='" + objEmployeeModel.Eid_visa_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.visa_Exp_date != null && objEmployeeModel.visa_Exp_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", visa_Exp_date='" + objEmployeeModel.visa_Exp_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Visa_stamped_on != null && objEmployeeModel.Visa_stamped_on > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Visa_stamped_on='" + objEmployeeModel.Visa_stamped_on.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Health_Insurance_Expiry != null && objEmployeeModel.Health_Insurance_Expiry > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Health_Insurance_Expiry='" + objEmployeeModel.Health_Insurance_Expiry.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.Date_of_join != null && objEmployeeModel.Date_of_join > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Date_of_join='" + objEmployeeModel.Date_of_join.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmployeeModel.Date_of_exit != null && objEmployeeModel.Date_of_exit > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Date_of_exit='" + objEmployeeModel.Date_of_exit.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.HealthCardIssueDate != null && objEmployeeModel.HealthCardIssueDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", HealthCardIssueDate='" + objEmployeeModel.HealthCardIssueDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.HealthCardExpDate != null && objEmployeeModel.HealthCardExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", HealthCardExpDate='" + objEmployeeModel.HealthCardExpDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objEmployeeModel.ProfilePic != null && objEmployeeModel.ProfilePic != "")
                {
                    sSqlstmt = sSqlstmt + ", ProfilePic='" + objEmployeeModel.ProfilePic + "'";
                }

                if (objEmployeeModel.CompetancyDoc != null && objEmployeeModel.CompetancyDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", CompetancyDoc='" + objEmployeeModel.CompetancyDoc + "'";
                }

                if (objEmployeeModel.JobDesc != null && objEmployeeModel.JobDesc != "")
                {
                    sSqlstmt = sSqlstmt + ", JobDesc='" + objEmployeeModel.JobDesc + "'";
                }
                if (objEmployeeModel.PassportUpload != null && objEmployeeModel.PassportUpload != "")
                {
                    sSqlstmt = sSqlstmt + ", PassportUpload='" + objEmployeeModel.PassportUpload + "'";
                }
                if (objEmployeeModel.EIDUpload != null && objEmployeeModel.EIDUpload != "")
                {
                    sSqlstmt = sSqlstmt + ", EIDUpload='" + objEmployeeModel.EIDUpload + "'";
                }
                if (objEmployeeModel.HealthCardUpload != null && objEmployeeModel.HealthCardUpload != "")
                {
                    sSqlstmt = sSqlstmt + ", HealthCardUpload='" + objEmployeeModel.HealthCardUpload + "'";
                }
                if (objEmployeeModel.Visa_upload != null && objEmployeeModel.Visa_upload != "")
                {
                    sSqlstmt = sSqlstmt + ", Visa_upload='" + objEmployeeModel.Visa_upload + "'";
                }
                sSqlstmt = sSqlstmt + " where emp_no='" + objEmployeeModel.emp_no + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmployeeMaster: " + ex.ToString());
            }
            return false;
        }

        public bool checkCompEmpIdExists(string CompEmpId)
        {
            try
            {
                string sSqlstmt = "select emp_no from t_hr_employee where emp_id='" + CompEmpId + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkCompEmpIdExists: " + ex.ToString());
            }
            return true;
        }

        public bool checkEmailAddressExists(string emailAddress)
        {
            try
            {
                string sSqlstmt = "select EmailId from t_hr_employee where EmailId='" + emailAddress + "' and emp_status=1";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkEmailAddressExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunDeleteEmployee(string emp_no)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee set emp_status=0 where emp_no=" + emp_no;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteEmployee: " + ex.ToString());
            }
            return false;
        }

        public string GetEmpHrNameById(string Empid)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select firstname from t_hr_employee where empid='" + Empid + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["firstname"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEmpHrNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetHrEmpEmailIdById(string Empid)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select emailaddress from t_hr_employee where empid='" + Empid + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["emailaddress"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetHrEmpEmailIdById: " + ex.ToString());
            }
            return "";
        }


        public string GetMultiHrEmpEmailIdById(string Empid)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("SELEct  GROUP_CONCAT(m.emailaddress) emailaddress FROM t_hr_employee m  where empid in (" + Empid + ")");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["emailaddress"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHrEmpEmailIdById: " + ex.ToString());
            }
            return "";
        }
    }

    public class EmployeeDependentModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_hr_emp_dependents { get; set; }

        [Display(Name = "Id")]
        public int emp_no { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Relationship")]
        public string Relationship { get; set; }

        [Display(Name = "Passport No")]
        public string PassportNo { get; set; }

        [Display(Name = "Passport Exp date")]
        public DateTime PassportExpDate { get; set; }

        [Display(Name = "EID No")]
        public string EIDNo { get; set; }

        [Display(Name = "EID Exp date")]
        public DateTime EIDExpDate { get; set; }

        [Display(Name = "Health Insurance Provider")]
        public string HealthInsProvider { get; set; }

        [Display(Name = "Health Insurance Exp date")]
        public DateTime HealthInsExp { get; set; }

        [Display(Name = "Visa No")]
        public string VisaNo { get; set; }

        [Display(Name = "Visa Exp date")]
        public DateTime VisaExpDate { get; set; }


        internal bool FunAddEmpDependentDetails(EmployeeDependentModels objEmp, EmployeeDependentModelsList lstemp)
        {

            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstemp.EmpDepList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_hr_employee_dependents (emp_no,FirstName,LastName,Gender,Relationship,PassportNo,EIDNo,HealthInsProvider,VisaNo";

                    string sFields = "", sFieldValue = "";

                    if (lstemp.EmpDepList[i].DOB != null && lstemp.EmpDepList[i].DOB > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", DOB";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpDepList[i].DOB.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstemp.EmpDepList[i].PassportExpDate != null && lstemp.EmpDepList[i].PassportExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", PassportExpDate";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpDepList[i].PassportExpDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstemp.EmpDepList[i].EIDExpDate != null && lstemp.EmpDepList[i].EIDExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", EIDExpDate";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpDepList[i].EIDExpDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstemp.EmpDepList[i].HealthInsExp != null && lstemp.EmpDepList[i].HealthInsExp > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", HealthInsExp";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpDepList[i].HealthInsExp.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstemp.EmpDepList[i].VisaExpDate != null && lstemp.EmpDepList[i].VisaExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", VisaExpDate";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpDepList[i].VisaExpDate.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objEmp.emp_no + "','" + lstemp.EmpDepList[i].FirstName
                   + "','" + lstemp.EmpDepList[i].LastName + "','" + lstemp.EmpDepList[i].Gender + "','" + lstemp.EmpDepList[i].Relationship + "','" + lstemp.EmpDepList[i].PassportNo + "','" + lstemp.EmpDepList[i].EIDNo + "'"
                    + ",'" + lstemp.EmpDepList[i].HealthInsProvider + "','" + lstemp.EmpDepList[i].VisaNo + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpDependentDetails: " + ex.ToString());

            }
            return false;
        }

        [AllowAnonymous]
        public bool AddEmpDependentDetails(EmployeeDependentModels objEmp, FormCollection form)
        {
            try
            {
                EmployeeDependentModelsList lstemp = new EmployeeDependentModelsList();
                lstemp.EmpDepList = new List<EmployeeDependentModels>();

                DateTime dateValues;
                objEmp.FirstName = form["FirstName"];
                objEmp.LastName = form["LastName"];
                objEmp.Gender = form["Gender"];
                objEmp.Relationship = form["Relationship"];
                objEmp.PassportNo = form["PassportNo"];
                objEmp.EIDNo = form["EIDNo"];
                objEmp.HealthInsProvider = form["HealthInsProvider"];
                objEmp.VisaNo = form["VisaNo"];
                if (DateTime.TryParse(form["DOB"], out dateValues) == true)
                {
                    objEmp.DOB = dateValues;
                }
                if (DateTime.TryParse(form["PassportExpDate"], out dateValues) == true)
                {
                    objEmp.PassportExpDate = dateValues;
                }
                if (DateTime.TryParse(form["EIDExpDate"], out dateValues) == true)
                {
                    objEmp.EIDExpDate = dateValues;
                }
                if (DateTime.TryParse(form["HealthInsExp"], out dateValues) == true)
                {
                    objEmp.HealthInsExp = dateValues;
                }
                if (DateTime.TryParse(form["VisaExpDate"], out dateValues) == true)
                {
                    objEmp.VisaExpDate = dateValues;
                }
                lstemp.EmpDepList.Add(objEmp);
                return objEmp.FunAddEmpDependentDetails(objEmp, lstemp);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in AddEmpDependentDetails: " + ex.ToString());

            }
            return false;
        }

        internal bool FunUpdateEmpDependant(EmployeeDependentModels objEmp)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee_dependents set FirstName ='" + objEmp.FirstName + "', LastName='" + objEmp.LastName + "', "
                    + "Gender='" + objEmp.Gender + "', Relationship='" + objEmp.Relationship
                    + "', PassportNo='" + objEmp.PassportNo + "', EIDNo='" + objEmp.EIDNo + "', HealthInsProvider='" + objEmp.HealthInsProvider + "', VisaNo='" + objEmp.VisaNo + "'";

                if (objEmp.DOB != null && objEmp.DOB > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", DOB='" + objEmp.DOB.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmp.PassportExpDate != null && objEmp.PassportExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", PassportExpDate='" + objEmp.PassportExpDate.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmp.EIDExpDate != null && objEmp.EIDExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", EIDExpDate='" + objEmp.EIDExpDate.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmp.HealthInsExp != null && objEmp.HealthInsExp > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", HealthInsExp='" + objEmp.HealthInsExp.ToString("yyyy/MM/dd") + "'";
                }

                if (objEmp.VisaExpDate != null && objEmp.VisaExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", VisaExpDate='" + objEmp.VisaExpDate.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_hr_emp_dependents='" + objEmp.id_hr_emp_dependents + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmpDependant: " + ex.ToString());

            }
            return false;
        }

    }

    public class EmployeePassModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_pass { get; set; }

        [Display(Name = "Id")]
        public int emp_no { get; set; }

        [Display(Name = "Pass Type")]
        public string PassType { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }

        [Display(Name = "Pass Expiry Date")]
        public DateTime ExpDate { get; set; }

        internal bool FunDeletePass(string sid_pass)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee_pass set Active=0 where id_pass='" + sid_pass + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePass: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteDependents(string id_hr_emp_dependents)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee_dependents set Active=0 where id_hr_emp_dependents='" + id_hr_emp_dependents + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDependents: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdatePass(EmployeePassModels objEmp)
        {
            try
            {
                string sSqlstmt = "update t_hr_employee_pass set PassType ='" + objEmp.PassType + "'";

                if (objEmp.Upload != null)
                {
                    sSqlstmt = sSqlstmt + ", Upload='" + objEmp.Upload + "'";
                }

                if (objEmp.ExpDate != null && objEmp.ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {

                    sSqlstmt = sSqlstmt + ", ExpDate='" + objEmp.ExpDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_pass='" + objEmp.id_pass + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePass: " + ex.ToString());
            }

            return false;
        }
        internal bool FunAddEmpPassDetails(EmployeePassModels objEmp, EmployeePassModelsList lstemp)
        {

            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < lstemp.EmpPassList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_hr_employee_pass (emp_no,PassType";

                    string sFields = "", sFieldValue = "";

                    if (lstemp.EmpPassList[i].ExpDate != null && lstemp.EmpPassList[i].ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", ExpDate";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpPassList[i].ExpDate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (lstemp.EmpPassList[i].Upload != null && lstemp.EmpPassList[i].Upload != "")
                    {
                        sFields = sFields + ", Upload";
                        sFieldValue = sFieldValue + ", '" + lstemp.EmpPassList[i].Upload + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objEmp.emp_no + "','" + lstemp.EmpPassList[i].PassType + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpPassDetails: " + ex.ToString());

            }
            return false;
        }
    }

    public class VisitorsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_visitors { get; set; }

        [Display(Name = "First Name")]
        public string firstname { get; set; }

        [Display(Name = "Last Name")]
        public string lastname { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Contact No")]
        public string contact_no { get; set; }

        [Display(Name = "Visitors Company")]
        public string v_company { get; set; }

        [Display(Name = "Email Id")]
        public string email { get; set; }

        [Display(Name = "Date of Visit")]
        public DateTime visit_date { get; set; }

        [Display(Name = "Visit Purpose")]
        public string purpose_visit { get; set; }

        [Display(Name = "HSE Induction Required")]
        public string hse_ind { get; set; }

        [Display(Name = "Attachment")]
        public string upload { get; set; }

        internal bool FunAddVisitors(VisitorsModels objVisit)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_visitors ( firstname, lastname, designation, contact_no, email, v_company, purpose_visit,hse_ind,branch";

                string sFields = "", sFieldValue = "";

                if (objVisit.visit_date != null && objVisit.visit_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", visit_date";
                    sFieldValue = sFieldValue + ", '" + objVisit.visit_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objVisit.upload != null && objVisit.upload != "")
                {
                    sFields = sFields + ", upload";
                    sFieldValue = sFieldValue + ", '" + objVisit.upload + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objVisit.firstname + "','" + objVisit.lastname + "','" + objVisit.designation
                 + "','" + objVisit.contact_no + "','" + objVisit.email + "','" + objVisit.v_company + "','" + objVisit.purpose_visit + "','" + objVisit.hse_ind + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddVisitors: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateVisitors(VisitorsModels objVisit)
        {
            try
            {
                string sSqlstmt = "update t_visitors set firstname='" + objVisit.firstname + "', lastname='" + objVisit.lastname + "', designation='" +
                objVisit.designation + "', contact_no='" + objVisit.contact_no + "', email='" + objVisit.email +
                "', v_company='" + objVisit.v_company + "', purpose_visit='" + objVisit.purpose_visit + "', hse_ind='" + objVisit.hse_ind + "'";

                if (objVisit.visit_date != null && objVisit.visit_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", visit_date='" + objVisit.visit_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objVisit.upload != null && objVisit.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objVisit.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where id_visitors='" + objVisit.id_visitors + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateVisitors: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteVisitors(string id_visitors)
        {
            try
            {
                string sSqlstmt = "update t_visitors set active=0 where id_visitors=" + id_visitors;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteVisitors: " + ex.ToString());
            }
            return false;
        }

    }

    public class VisitorsModelList
    {
        public List<VisitorsModels> VisiorList { get; set; }
    }

    public class EmployeeMasterModelList
    {
        public List<EmployeeMasterModels> EmployeeList { get; set; }
    }

    public class EmployeeDependentModelsList
    {
        public List<EmployeeDependentModels> EmpDepList { get; set; }
    }

    public class EmployeePassModelsList
    {
        public List<EmployeePassModels> EmpPassList { get; set; }
    }
}