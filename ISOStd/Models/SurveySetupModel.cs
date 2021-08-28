using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;



namespace ISOStd.Models
{
    public class SurveySetupModel
    {

        clsGlobal objGlobaldata = new clsGlobal();

        [Required]

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]

        [Display(Name = "Password")]

        public string Password { get; set; }


        [Required]

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }



        [Display(Name = "Cloud Survey Url")]
        public string Cloud { get; set; }

        [Display(Name = "Al Manal Reset Code")]
        public string Code { get; set; }




        internal bool FunAddCloud(SurveySetupModel objSurveySetupModel)
        {
            try
            {
                string sSqlstmt = "truncate t_survey_setup_cloud; insert into t_survey_setup_cloud(remote_url) values('" + objSurveySetupModel.Cloud + "');";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Adding Cloud: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCredentials(SurveySetupModel objSurveySetupModel)
        {
            try
            {
                string sSqlstmt = "truncate t_survey_setup_user_credentials; insert into t_survey_setup_user_credentials(user_email,user_password) values('" + objSurveySetupModel.Email + "','" + objSurveySetupModel.Password + "'); ";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Adding Survey Credentials: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDelete()
        {
            try
            {
                string sSqlstmt = "truncate t_survey_setup_user_credentials; truncate t_survey_setup_cloud;";

                return objGlobaldata.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Deleting Survey Credentials: " + ex.ToString());
            }
            return false;
        }

    }



    public class SurveyModel
    {

        public string Id { get; set; }


        public string Name { get; set; }



        public string SurveyCreator { get; set; }



        public string StartDate { get; set; }




        public string EndDate { get; set; }

        public List<string> Questions { get; set; }
        public List<string> Responses { get; set; }



        public bool IsActive { get; set; }

    }

}