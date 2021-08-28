using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class TrainingOrientationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public int TrainingOrientation_Id { get; set; }

        [Required]
        [Display(Name = "Training Topic")]
        [System.Web.Mvc.Remote("doesTopicNameExist", "TrainingOrientation", HttpMethod = "POST", ErrorMessage = "Topic already exists. Please enter a different Topic.")]
        public string Topic { get; set; }
               
        [Display(Name = "Document Upload Path")]
        public string DocUploadPath { get; set; }
        
        [Display(Name = "Id")]
        public string emp_orientation_id { get; set; }

        [Display(Name = "Employee ID")]
        public string Emp_id { get; set; }
        
        [Display(Name = "First Name")]
        public string First_name { get; set; }
        
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Orientation Date")]
        public DateTime Orientation_date { get; set; }

        [Display(Name = "Verifified By")]
        public string VerifiedBy { get; set; }

        [Display(Name = "Training Status")]
        public string TrainingStatus { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "Evidence")]
        public string Evidence { get; set; }

        [Display(Name = "Orientation Status")]
        public string OrientationStatus { get; set; }

        [Display(Name = "Verification Date")]
        public DateTime VerificationDate { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool CheckForEmployeeExists(string EmpID)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Emp_id from t_emp_orientation where Emp_id='" + EmpID + "'");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForEmployeeExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunDeleteTrainingOrientation(string semp_orientation_id)
        {
            try
            {
                string sSqlstmt = "update t_emp_orientation set Active=0 where emp_orientation_id='" + semp_orientation_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrainingOrientation: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteTrainingOrientationTopic(string sTrainingOrientation_Id)
        {
            try
            {
                string sSqlstmt = "update t_trainingorientation set Active=0 where TrainingOrientation_Id='" + sTrainingOrientation_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrainingOrientationTopic: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunAddOrientation(TrainingOrientationModels objOrientation)
        {
            try
            {
                string sSqlstmt = "insert into t_trainingorientation (Topic,DocUploadPath,branch,Department,Location)" +
                    " values('" + objOrientation.Topic + "','" + objOrientation.DocUploadPath + "','" + objOrientation.branch
                    + "','" + objOrientation.Department + "','" + objOrientation.Location + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddOrientation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateOrientation(TrainingOrientationModels objOrientaionModels)
        {
            try
            {
                string sSqlstmt = "update t_trainingorientation set Topic='" + objOrientaionModels.Topic + "', branch = '"+ objOrientaionModels.branch
                    + "', Department = '" + objOrientaionModels.Department + "', Location = '" + objOrientaionModels.Location + "'";
                if (objOrientaionModels.DocUploadPath != null && objOrientaionModels.DocUploadPath != "")
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objOrientaionModels.DocUploadPath + "'";
                }
                sSqlstmt = sSqlstmt + " where TrainingOrientation_Id='" + objOrientaionModels.TrainingOrientation_Id + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateOrientation: " + ex.ToString());
            }
            return false;
        }

        public DataSet GetTrainingTopicListbox()
        {
            DataSet dsOrient = new DataSet();
            try
            {
                string sSqlstmt = "SELECT TrainingOrientation_Id,Topic,DocUploadPath FROM t_trainingorientation where active=1";

                dsOrient = objGlobalData.Getdetails(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingTopicListbox: " + ex.ToString());
            }
            return dsOrient;
        }

        public string GetTopicsNameById(string TrainingOrientation_Id)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Topic FROM t_trainingorientation where TrainingOrientation_Id='" + TrainingOrientation_Id + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Topic"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTopicsNameById: " + ex.ToString());
            }
            return "";
        }

        public DataSet GetSurveyRating()
        {
            //SurveyRatingList lstSurvey = new SurveyRatingList();
            //lstSurvey.lstSurveyRating = new List<SurveyRatingDetails>();
            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select SQ_OptionsId, RatingOptions, Weightage from t_trainingtopic_options where active=1 order by Weightage desc");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        internal bool FunAddEmpOrientation(TrainingOrientationModels objOrientation, EmpOrientationModelsList obj)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sOrientationDate = DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_emp_orientation (Emp_id,Orientation_date,branch";

                sSqlstmt = sSqlstmt + ") values('" + objOrientation.Emp_id + "','" + sOrientationDate + "','" + sBranch + "')";

                int iPerformance_EvalId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iPerformance_EvalId))
                {
                    EmpOrientationModels objElement = new EmpOrientationModels();

                    obj.lstOrientation[0].emp_orientation_id = iPerformance_EvalId.ToString();
                    objElement.FunAddEmpPerformanceEvaluation(obj);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpOrientation: " + ex.ToString());
            }
            return false;
        }

        internal bool CheckForTopicNameExists(string Topic)
        {
            try
            {
                DataSet dsDoc = objGlobalData.Getdetails("select Topic from t_trainingorientation where Topic='" + Topic + "'");

                if (dsDoc.Tables.Count > 0 && dsDoc.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in CheckForTopicNameExists: " + ex.ToString());
            }
            return true;
        }
        public string GetRatingNameById(string SQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select RatingOptions from t_trainingtopic_options where SQ_OptionsId='" + SQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["RatingOptions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRatingNameById: " + ex.ToString());
            }
            return "";
        }

        internal bool FunAddOrientationVerification(TrainingOrientationModels objOrientation)
        {
            try
            {
                string sVerificationDate = DateTime.Today.ToString("yyyy-MM-dd HH':'mm':'ss");
                string user = "";

                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "update t_emp_orientation set VerifiedBy='" + user + "', "
                     + "TrainingStatus='" + objOrientation.TrainingStatus + "', VerificationDate='" + sVerificationDate
                     + "', Action='" + objOrientation.Action + "', OrientationStatus='" + objOrientation.OrientationStatus + "'";

                if (objOrientation.Evidence != null)
                {
                    sSqlstmt = sSqlstmt + ",Evidence='" + objOrientation.Evidence + "'";
                }

                sSqlstmt = sSqlstmt + " where emp_orientation_id='" + objOrientation.emp_orientation_id + "';";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddOrientationVerification: " + ex.ToString());
            }
            return false;
        }
    }

    public class TrainingOrientationModelList
    {
        public List<TrainingOrientationModels> Orientation { get; set; }
    }

    public class OrientationModelsDetails
    {
        public string TrainingOrientation_Id { get; set; }
        public string Topic { get; set; }
        public string DocUploadPath { get; set; }
    }

    public class OrientationModelsList
    {
        public List<OrientationModelsDetails> lstOrientation { get; set; }
    }

    public class EmpOrientationModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        public string Id { get; set; }
        public string emp_orientation_id { get; set; }
        public string TrainingOrientation_Id { get; set; }
        public string SQ_OptionsId { get; set; }

        internal bool FunAddEmpPerformanceEvaluation(EmpOrientationModelsList objOrient)
        {
            try
            {
                if (objOrient.lstOrientation.Count > 0)
                {
                    string sSqlstmt = "delete from t_emp_orientation_elements where emp_orientation_id='"
                        + objOrient.lstOrientation[0].emp_orientation_id + "'; ";
                    for (int i = 0; i < objOrient.lstOrientation.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_emp_orientation_elements (emp_orientation_id, TrainingOrientation_Id, SQ_OptionsId"
                        + ") values('" + objOrient.lstOrientation[0].emp_orientation_id + "','" + objOrient.lstOrientation[i].TrainingOrientation_Id
                        + "','" + objOrient.lstOrientation[i].SQ_OptionsId + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpPerformanceEvaluation: " + ex.ToString());
            }
            return false;
        }


    }

    public class EmpOrientationModelsList
    {
        public List<EmpOrientationModels> lstOrientation { get; set; }
    }
}