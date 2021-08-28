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
    public class SurveyModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "SQId")]
        public string SQId { get; set; }

        [Display(Name = "SQ_OptionsId")]
        public string SQ_OptionsId { get; set; }

        [Display(Name = "Survey Type")]
        public string Survey_TypeId { get; set; }

        [Required]
        [Display(Name = "Questions")]
        [System.Web.Mvc.Remote("doesQuestionExist", "Survey", HttpMethod = "POST", ErrorMessage = "Question already exists. Please enter a different Question.")]
        public string Questions { get; set; }

        [Required]
        [Display(Name = "Rating Options")]
        [System.Web.Mvc.Remote("doesOptionExist", "Survey", HttpMethod = "POST", ErrorMessage = "Rating factor already exists. Please enter a different factor.")]
        public string RatingOptions { get; set; }

        [Required]
        [Display(Name = "Weightage")]
        public string Weightage { get; set; }
        
        public string SQ_Weightage { get; set; }

        public string Survey_TypeId1 { get; set; }

        internal bool FunAddSurvey(SurveyModels objSurveyModels)
        {
            try
            {
                string sSqlstmt = "insert into t_surveyquestions (Survey_TypeId, Questions) values('" + objSurveyModels.Survey_TypeId1 + "','" + objSurveyModels.Questions + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSurvey: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddSurveyRatingFactor(SurveyModels objSurveyModels)
        {
            try
            {
                string sSqlstmt = "insert into t_surveyquestion_options (Survey_TypeId,RatingOptions, Weightage) values('" + objSurveyModels.Survey_TypeId1 + "','" + objSurveyModels.RatingOptions
                    + "','" + objSurveyModels.Weightage + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSurveyRatingFactor: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSurveyQuestions(string SQId, string Questions)
        {
            try
            {
                string sSqlstmt = "update t_surveyquestions set Questions='" + Questions + "' where SQId='" + SQId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSurveyQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSurveyRating(string sSQ_OptionsId, string sRatingOptions)
        {
            try
            {
                string sSqlstmt = "update t_surveyquestion_options set RatingOptions='" + sRatingOptions + "' where SQ_OptionsId='" + sSQ_OptionsId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSurveyRating: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSurveyQuestions(string sSQId)
        {
            try
            {
                // string sSqlstmt = "delete from t_surveyquestions where SQId=" + sSQId;
                string sSqlstmt = "update t_surveyquestions set Active=0 where SQId='" + sSQId + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSurveyQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSurveyRatingFactor(string sSQ_OptionsId)
        {
            try
            {
                //string sSqlstmt = "delete from t_surveyquestion_options where SQ_OptionsId=" + sSQ_OptionsId;
                string sSqlstmt = "update t_surveyquestion_options set Active=0 where SQ_OptionsId='" + sSQ_OptionsId + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSurveyRatingFactor: " + ex.ToString());
            }
            return false;
        }

        public bool checkQuestionExists(string sQuestions, string Survey_TypeId)
        {
            try
            {
                string sSqlstmt = "select Name from t_surveyquestions where active=1 and Questions='" + sQuestions + "' and Survey_TypeId='" + Survey_TypeId + "'";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkQuestionExists: " + ex.ToString());
            }
            return true;
        }

        public bool checkRatingFactorExists(string sRatingOptions)
        {
            try
            {
                string sSqlstmt = "select Name from t_surveyquestion_options where RatingOptions='" + sRatingOptions + "' and active=1";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkRatingFactorExists: " + ex.ToString());
            }
            return true;
        }

        public MultiSelectList GetSurveyTypeListbox(string TypeName = "")
        {
            SurveyModelsList Typelist = new SurveyModelsList();
            Typelist.lstSurvey = new List<SurveyModelsDetails>();

            try
            {
                string sSqlstmt = "SELECT Survey_TypeId as SQId, TypeName as Questions FROM t_survey_type where active=1";
                if (TypeName != "")
                {
                    sSqlstmt = "select SQId, Questions from t_surveyquestions as tsq, t_survey_type as tst where tsq.Survey_TypeId= tst.Survey_TypeId "
                        + " and TypeName='" + TypeName + "' and tsq.active=1 ";
                }
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        SurveyModelsDetails data = new SurveyModelsDetails()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString()
                        };

                        Typelist.lstSurvey.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyListbox: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstSurvey, "SQId", "Questions");
        }

        //public MultiSelectList GetSurveyTypeListboxforEdit(string TypeName = "")
        //{
        //    SurveyModelsList Typelist = new SurveyModelsList();
        //    Typelist.lstSurvey = new List<SurveyModelsDetails>();

        //    try
        //    {
        //        string sSqlstmt = "SELECT Survey_TypeId as SQId, TypeName as Questions FROM t_survey_type where active=1";
        //        if (TypeName != "")
        //        {
        //            sSqlstmt = "select SQId, Questions from t_surveyquestions as tsq, t_survey_type as tst where tsq.Survey_TypeId= tst.Survey_TypeId "
        //                + " and TypeName='" + TypeName + "'";
        //        }
        //        DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                SurveyModelsDetails data = new SurveyModelsDetails()
        //                {
        //                    SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
        //                    Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString()
        //                };

        //                Typelist.lstSurvey.Add(data);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetSurveyTypeListboxforEdit: " + ex.ToString());
        //    }
        //    return new MultiSelectList(Typelist.lstSurvey, "SQId", "Questions");
        //}


        public MultiSelectList GetSurveyQuestionsListbox(string Survey_TypeId)
        {
            SurveyModelsList Typelist = new SurveyModelsList();
            Typelist.lstSurvey = new List<SurveyModelsDetails>();

            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select SQId, Questions from t_surveyquestions as tsq, t_survey_type as tst where tsq.Survey_TypeId=tst.Survey_TypeId "
                + " and tst.Survey_TypeId='" + Survey_TypeId + "' and tsq.active=1 ");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        SurveyModelsDetails data = new SurveyModelsDetails()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString()
                        };

                        Typelist.lstSurvey.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyQuestionsListbox: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstSurvey, "SQId", "Questions");
        }

        public DataSet GetSurveyRating(string sSurvey_TypeId)
        {
            //SurveyRatingList lstSurvey = new SurveyRatingList();
            //lstSurvey.lstSurveyRating = new List<SurveyRatingDetails>();
            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select SQ_OptionsId,Survey_TypeId, RatingOptions, Weightage from t_surveyquestion_options where Survey_TypeId='" + sSurvey_TypeId + "' and active=1 order by Weightage desc");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        public string GetSurveyRatingNameById(string SQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select RatingOptions from t_surveyquestion_options where SQ_OptionsId='" + SQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["RatingOptions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRatingNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetSurveyRatingWeightageById(string SQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Weightage from t_surveyquestion_options where SQ_OptionsId='" + SQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Weightage"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRatingWeightageById: " + ex.ToString());
            }
            return "";
        }

        public string GetSurveyQuestionNameById(string SQId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Questions FROM t_surveyquestions where SQId='" + SQId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Questions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyQuestionNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetSurveyTypeIdByQuestionId(string SQId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Survey_TypeId from t_surveyquestions where SQId='" + SQId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Survey_TypeId"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyTypeIdByQuestionId: " + ex.ToString());
            }
            return "";
        }

        public string GetSurveyTypeIdByRatingId(string sSQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Survey_TypeId from t_surveyquestion_options where SQ_OptionsId='" + sSQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Survey_TypeId"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyTypeIdByRatingId: " + ex.ToString());
            }
            return "";
        }

        public string GenerateCustomerSurveyForm()
        {
            string sContent = "";
            try
            {
                //using streamreader for reading my htmltemplate   
                DataSet dsSurvey = objGlobalData.Getdetails("SELECT SQId, Questions FROM t_surveyquestions where Active=1 and Survey_TypeId=2");
                DataSet dsSurveyRatingFactor = objGlobalData.Getdetails("select SQ_OptionsId, RatingOptions from t_surveyquestion_options where Active=1 and Survey_TypeId=2 order by Weightage desc");

                if (dsSurvey.Tables.Count > 0 && dsSurvey.Tables[0].Rows.Count > 0 && dsSurveyRatingFactor.Tables.Count > 0 && dsSurveyRatingFactor.Tables[0].Rows.Count > 0)
                {
                    string Header = "", sSHeader = "";
                    string sHeader = "<tr>"
                           + "<th>Sl. No.</th>"
                           + "<th>Elements to be rated</th>";
                    for (int i = 0; i < dsSurveyRatingFactor.Tables[0].Rows.Count; i++)
                    {
                        sSHeader = sSHeader + "<th>" + dsSurveyRatingFactor.Tables[0].Rows[i]["RatingOptions"].ToString() + "</th>";
                    }
                    Header = sHeader + sSHeader + "</tr>";
                    string sQuestions = "";
                    for (int i = 0; i < dsSurvey.Tables[0].Rows.Count; i++)
                    {
                        sQuestions = sQuestions + "<tr> <td>" + (i + 1) + "</td> <td>" + dsSurvey.Tables[0].Rows[i]["Questions"].ToString() + "</td>"
                            + "<input type='hidden' id='rdbquestion" + i + "' value='" + dsSurvey.Tables[0].Rows[i]["SQId"].ToString() + "'/> "
                            + "<input type='hidden' id='rdbquestionVal" + i + "' />";
                        string sRating = "";
                        for (int options = 0; options < dsSurveyRatingFactor.Tables[0].Rows.Count; options++)
                        {
                            /*string sRatingOptionsId = dsSurveyRatingFactor.Tables[0].Rows[options]["SQ_OptionsId"].ToString();
                            sRating =sRating+ "<input type='radio' name='rdbqoption" + i + "' value='" + sRatingOptionsId
                                + @"' onclick=""SetValue('rdbquestionVal" + i + "', '" + sRatingOptionsId + @"')"">" + dsSurveyRatingFactor.Tables[0].Rows[options]["RatingOptions"].ToString();*/
                            string sRatingOptionsId = dsSurveyRatingFactor.Tables[0].Rows[options]["SQ_OptionsId"].ToString();
                            sRating = sRating + " <td align=center><input type='radio' name='rdbqoption" + i + "' value='" + sRatingOptionsId
                                + @"' onclick=""SetValue('rdbquestionVal" + i + "', '" + sRatingOptionsId + @"')""></td>";
                        }
                        sQuestions = sQuestions + sRating + "</tr>";

                    }
                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/Survey/SurveyForm.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }

                    sContent = sContent.Replace("{content}", Header + sQuestions); //replacing the required things
                    sContent = sContent.Replace("{Qcount}", dsSurvey.Tables[0].Rows.Count.ToString());

                    string sFilepath = Path.Combine(HttpContext.Current.Server.MapPath("~/DataUpload"), "CustomerSurveyForm.html");

                    File.WriteAllText(sFilepath, sContent);

                    return "~/DataUpload/CustomerSurveyForm.html";
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GenerateCustomerSurveyForm: " + ex.ToString());
            }
            return "";
        }

        internal bool FunAddCustomerFeedback(CustomerSurveyFeedbackList lstFeedback, string sCustId)
        {
            try
            {
                string sSqlstmt = "";
                string sSurveyDate = DateTime.Now.ToString("yyyy-MM-dd");
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                for (int i = 0; i < lstFeedback.lstCustomerSurveyFeedback.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert t_customer_survey (CustId, SQId, SQ_OptionsId, SurveyDate,branch) values('" + sCustId
                        + "','" + lstFeedback.lstCustomerSurveyFeedback[i].id + "','" + lstFeedback.lstCustomerSurveyFeedback[i].answer + "','" + sSurveyDate + "','" + sBranch + "'); ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerFeedback: " + ex.ToString());
            }
            return false;
        }

        internal DataSet GetCustSurvey(string sBranch_Id)
        {
            DataSet dsData = new DataSet();
            try
            {
                string sSqlstmt = "select max(Customer_SurveyId) as Customer_SurveyId, tcs.CustId, companyname, max(SurveyDate) as SurveyDate  from t_customer_survey as tcs,"
                    + " t_customer_info as tCust where tcs.custid=tCust.custid and  tcs.branch ='" + sBranch_Id+ "' group by tcs.CustId , companyname";
                dsData = objGlobalData.Getdetails(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCustSurvey: " + ex.ToString());
            }
            return dsData;
        }

        public string getSurveyIDByName(string survey_name)
        {
            try
            {
                string sSsqlstmt = "select Survey_TypeId as Id from t_survey_type where TypeName='" + survey_name + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getSurveyIDByName: " + ex.ToString());
            }
            return "";
        }

        public string getSurveyNameById(string survey_Id)
        {
            try
            {
                string sSsqlstmt = "select TypeName from t_survey_type where Survey_TypeId ='" + survey_Id + "'";
                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["TypeName"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in getSurveyIDByName: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetSurveyRatings(string Survey_TypeId)
        {
            SurveyModelsList Typelist = new SurveyModelsList();
            Typelist.lstSurvey = new List<SurveyModelsDetails>();

            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select SQ_OptionsId, RatingOptions from t_surveyquestion_options as tsq, t_survey_type as tst where tsq.Survey_TypeId=tst.Survey_TypeId "
                + " and tst.Survey_TypeId='" + Survey_TypeId + "' and tsq.active=1 ");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        SurveyModelsDetails data = new SurveyModelsDetails()
                        {
                            SQ_OptionsId = dsEmp.Tables[0].Rows[i]["SQ_OptionsId"].ToString(),
                            RatingOptions = dsEmp.Tables[0].Rows[i]["RatingOptions"].ToString()
                        };

                        Typelist.lstSurvey.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRatings: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstSurvey, "SQ_OptionsId", "RatingOptions");
        }

    }


    public class SurveyModelsDetails
    {
        public string SQId { get; set; }
        public string Questions { get; set; }
        public string SQ_OptionsId { get; set; }        
        public string RatingOptions { get; set; }
    }

    public class SurveyModelsList
    {
        public List<SurveyModelsDetails> lstSurvey { get; set; }
    }

    public class SurveyRating
    {
        public string SQ_OptionsId { get; set; }
        public string Survey_TypeId { get; set; }
        public string RatingOptions { get; set; }
        public string Weightage { get; set; }
    }

    public class SurveyRatingList
    {
        public List<SurveyRating> lstSurveyRating { get; set; }
    }

    public class CustomerSurveyFeedbacks
    {
        public string SQId { get; set; }
        public string SQ_OptionsId { get; set; }
    }

    public class CustomerSurveyFeedbackList
    {
        public List<CustomerSurveyFeedbackJson> lstCustomerSurveyFeedback { get; set; }
    }

    public class CustomerSurveyFeedbackJson
    {
        public string id { get; set; }
        public string answer { get; set; }
    }
}