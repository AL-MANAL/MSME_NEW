using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ISOStd.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class EmployeeCompetenceEvalModels
    {
        clsGlobal objGlobalData = new clsGlobal();


        [Display(Name = "ID")]
        public string CompetenceId { get; set; }


        [Display(Name = "Employee Name")]
        public string Name { get; set; }


        [Display(Name = "Evaluation Date")]
        public DateTime Evaluation_DoneOn { get; set; }


        [Display(Name = "Evaluation Freq")]
        public string Evaluated_Freq { get; set; }


        [Display(Name = "Evaluated By")]
        public string Evalaution_Done_By { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Minimum Competence Details as required by the Position")]
        public string Academic_MinComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Employee Competence Details")]
        public string Academic_EmpComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Evaluation Output")]
        public string Academic_EvalOutput { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Minimum Competence Details as required by the Position")]
        public string YrExp_MinComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Employee Competence Details")]
        public string YrExp_EmpComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Evaluation Output")]
        public string YrExp_EvalOutput { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Minimum Competence Details as required by the Position")]
        public string Relevant_MinComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Employee Competence Details")]
        public string Relevant_EmpComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Evaluation Output")]
        public string Relevant_EvalOutput { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Minimum Competence Details as required by the Position")]
        public string Skills_MinComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Employee Competence Details")]
        public string Skills_EmpComp_Details { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Evaluation Output")]
        public string Skills_EvalOutput { get; set; }


        [Display(Name = "Is employee suitable to hold the position?")]
        public string Emp_Suit_Hold_Pos { get; set; }


        [Display(Name = "Is employee competent to be promoted to next position?")]
        public string Emp_Prom_Nxt_Pos { get; set; }


        [Display(Name = "Need of trainings to enhance the competence:")]
        public string Need_Of_Trainings { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "If employee is not competent to hold the position, actions to be taken:")]
        public string Emp_Not_Competent_Action { get; set; }


        [Display(Name = "To be reviewed by")]
        public string Eval_ReviewedBy { get; set; }


        [Display(Name = "To be approved by")]
        public string Eval_ApprovedBy { get; set; }


        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Evaluation status")]
        public string eval_status { get; set; }

        public string eval_status_id { get; set; }

        [Display(Name = "Remarks")]
        public string review_comments { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime reviewed_date { get; set; }

        [Display(Name = "Document(s)")]
        public string review_upload { get; set; }

        [Display(Name = "Remarks")]
        public string approver_comments { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime approved_date { get; set; }

        [Display(Name = "Document(s)")]
        public string approver_upload { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        //t_emp_competence_eval_training

        public string id_training { get; set; }

        [Display(Name = "Source of training")]
        public string source_training { get; set; }

        [Display(Name = "Training topic")]
        public string training_topic { get; set; }

        [Display(Name = "Criticality")]
        public string criticality { get; set; }

        [Display(Name = "Type of Training")]
        public string training_type { get; set; }

        [Display(Name = "Years of Experience")]
        public string years_exp { get; set; }

        [Display(Name = "Skills")]
        public string skill { get; set; }

        [Display(Name = "Qualification")]
        public string qualification { get; set; }

        [Display(Name = "Date of Join")]
        public DateTime Date_of_join { get; set; }

        internal bool FunDeleteCompetenceDoc(string sCompetenceId)
        {
            try
            {
                string sSqlstmt = "update t_emp_competence_eval set Active=0 where CompetenceId='" + sCompetenceId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCompetenceDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCompetenceEvaluation(EmployeeCompetenceEvalModels objEmployeeCompetence, EmployeeCompetenceEvalModelsList objModelList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_emp_competence_eval (Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                    + " Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                    + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                    + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy,Name,branch,LoggedBy,notified_to";

                if (objEmployeeCompetence.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluation_DoneOn";
                    sValues = sValues + ", '" + objEmployeeCompetence.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objEmployeeCompetence.Evaluated_Freq + "','" + objEmployeeCompetence.Evalaution_Done_By
                 + "','" + objEmployeeCompetence.Academic_MinComp_Details + "','" + objEmployeeCompetence.Academic_EmpComp_Details
                 + "','" + objEmployeeCompetence.Academic_EvalOutput + "','" + objEmployeeCompetence.YrExp_MinComp_Details + "','" + objEmployeeCompetence.YrExp_EmpComp_Details
                 + "','" + objEmployeeCompetence.YrExp_EvalOutput + "','" + objEmployeeCompetence.Relevant_MinComp_Details + "','" + objEmployeeCompetence.Relevant_EmpComp_Details
                 + "','" + objEmployeeCompetence.Relevant_EvalOutput + "','" + objEmployeeCompetence.Skills_MinComp_Details + "','" + objEmployeeCompetence.Skills_EmpComp_Details
                 + "','" + objEmployeeCompetence.Skills_EvalOutput + "','" + objEmployeeCompetence.Emp_Suit_Hold_Pos + "','" + objEmployeeCompetence.Emp_Prom_Nxt_Pos
                 + "','" + objEmployeeCompetence.Need_Of_Trainings + "','" + objEmployeeCompetence.Emp_Not_Competent_Action + "','" + objEmployeeCompetence.Eval_ReviewedBy
                 + "','" + objEmployeeCompetence.Eval_ApprovedBy + "','" + objEmployeeCompetence.Name + "','" + sBranch + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + notified_to + "'";

                sSqlstmt = sSqlstmt + sValues + ")";
                int CompetenceId = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out CompetenceId))
                {
                    if (objModelList.lstEmployeeCompetenceEval.Count > 0)
                    {
                        objModelList.lstEmployeeCompetenceEval[0].CompetenceId = CompetenceId.ToString();
                        FunAddTrainingList(objModelList);
                    }
                    return SendEmpCompetencemail(CompetenceId, "Employee Completence Evaluation for review");
                }

               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCompetenceEvaluation: " + ex.ToString());
            }
            return false;


        }
        internal bool FunAddTrainingList(EmployeeCompetenceEvalModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_emp_competence_eval_training where CompetenceId='" + objModelsList.lstEmployeeCompetenceEval[0].CompetenceId + "'; ";

                for (int i = 0; i < objModelsList.lstEmployeeCompetenceEval .Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_emp_competence_eval_training(CompetenceId,training_topic,source_training,criticality";

                    string sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModelsList.lstEmployeeCompetenceEval[0].CompetenceId + "', '" + objModelsList.lstEmployeeCompetenceEval[i].training_topic + "', '" + objModelsList.lstEmployeeCompetenceEval[i].source_training + "', '" + objModelsList.lstEmployeeCompetenceEval[i].criticality + "'";

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
        internal bool SendEmpCompetencemail(int  CompetenceId, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select CompetenceId,Name,Evaluation_DoneOn,Evalaution_Done_By,LoggedBy,Eval_ReviewedBy,Eval_ApprovedBy,notified_to"
               + " from t_emp_competence_eval  where CompetenceId = '" + CompetenceId + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString())+","+ objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notified_to"].ToString());


                    sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                    + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Name"].ToString()) + "</td></tr>"
                    + "<tr><td colspan=3><b>Evaluation done by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()) + "</td></tr>";

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee Competence Evaluation");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmpCompetencemail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunCompetenceEvaluationReview(EmployeeCompetenceEvalModels objModel)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_emp_competence_eval set eval_status='" + eval_status + "',review_comments='" + review_comments + "',reviewed_date='" + TodayDate + "',review_upload='" + review_upload + "'";

                sSqlstmt = sSqlstmt + " where CompetenceId='" + objModel.CompetenceId + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);
                return SendEmpCompetencemailReviewmail(CompetenceId, eval_status, "Employee competence evaluation status");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInspChecklistReview: " + ex.ToString());
            }
            return false;
        }
        internal bool FunCompetenceEvaluationApprove(EmployeeCompetenceEvalModels objModel)
        {
            try
            {
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_emp_competence_eval set eval_status='" + eval_status + "',approver_comments='" + approver_comments + "',approved_date='" + TodayDate + "',approver_upload='" + approver_upload + "'";

                sSqlstmt = sSqlstmt + " where CompetenceId='" + objModel.CompetenceId + "'";
                objGlobalData.ExecuteQuery(sSqlstmt);
                return SendEmpCompetencemailApprovemail(CompetenceId, eval_status, "Employee competence evaluation status");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunCompetenceEvaluationApprove: " + ex.ToString());
            }
            return false;
        }
        internal bool SendEmpCompetencemailApprovemail(string CompetenceId, string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select CompetenceId,Name,Evaluation_DoneOn,Evalaution_Done_By,LoggedBy,Eval_ReviewedBy,Eval_ApprovedBy,review_comments,approver_comments,notified_to"
                + " from t_emp_competence_eval  where CompetenceId = '" + CompetenceId + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notified_to"].ToString());


                    if (iStatus == "4") // Approve
                    {
                        sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                  + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Name"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Evaluation done by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Approved</td></tr>"
                       + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Name"].ToString()) + "</td></tr>"
                + "<tr><td colspan=3><b>Evaluation done by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()) + "</td></tr>"
                            + "<tr><td colspan=3><b>Approve Status:<b></td> <td colspan=3>Rejected</td></tr>"
                       + "<tr><td colspan=3><b>Approved By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["approver_comments"].ToString()) + "</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee competence evaluation");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmpCompetencemailApprovemail: " + ex.ToString());
            }
            return false;
        }
        internal bool SendEmpCompetencemailReviewmail(string CompetenceId, string iStatus, string sMessage = "")
        {
            try
            {
                string sType = "email";
                string sSqlstmt = "select CompetenceId,Name,Evaluation_DoneOn,Evalaution_Done_By,LoggedBy,Eval_ReviewedBy,Eval_ApprovedBy,review_comments,notified_to"
                + " from t_emp_competence_eval  where CompetenceId = '" + CompetenceId + "'";

                DataSet dsList = objGlobalData.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    string sName = objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString());
                    string sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Eval_ApprovedBy"].ToString());
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString())+","+ objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "," + objGlobalData.GetMultiHrEmpEmailIdById(dsList.Tables[0].Rows[0]["notified_to"].ToString());


                    if (iStatus == "2") // Approve
                    {
                        sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                  + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Name"].ToString()) + "</td></tr>"
                  + "<tr><td colspan=3><b>Evaluation done by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()) + "</td></tr>"
                        +"<tr><td colspan=3><b>Review Status:<b></td> <td colspan=3>Reviewed</td></tr>"
                       + "<tr><td colspan=3><b>Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["review_comments"].ToString()) + "</td></tr>";
                    }
                    else
                    {
                        sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Name"].ToString()) + "</td></tr>"
                + "<tr><td colspan=3><b>Evaluation done by:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Evalaution_Done_By"].ToString()) + "</td></tr>"
                      + "<tr><td colspan=3><b>Review Status:<b></td> <td colspan=3>Rejected</td></tr>"
                        + "<tr><td colspan=3><b>Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsList.Tables[0].Rows[0]["Eval_ReviewedBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Comment:<b></td> <td colspan=3>" + (dsList.Tables[0].Rows[0]["review_comments"].ToString()) + "</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee competence evaluation");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');

                    sCCEmailIds = Regex.Replace(sCCEmailIds, ",+", ",");
                    sCCEmailIds = sCCEmailIds.Trim();
                    sCCEmailIds = sCCEmailIds.TrimEnd(',');
                    sCCEmailIds = sCCEmailIds.TrimStart(',');

                    return objGlobalData.Sendmail(sToEmailIds, sSubject + sMessage, sContent, aAttachment, sCCEmailIds, "");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendEmpCompetencemailReviewmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateCompetenceEvaluation(EmployeeCompetenceEvalModels objEmployeeCompetence, EmployeeCompetenceEvalModelsList objModelList)
        {
            try
            {
                string sSqlstmt = " update t_emp_competence_eval set Evaluated_Freq='" + objEmployeeCompetence.Evaluated_Freq
                 + "', Evalaution_Done_By='" + objEmployeeCompetence.Evalaution_Done_By
                 + "', Academic_MinComp_Details='" + objEmployeeCompetence.Academic_MinComp_Details + "', Academic_EmpComp_Details='" + objEmployeeCompetence.Academic_EmpComp_Details
                 + "', Academic_EvalOutput='" + objEmployeeCompetence.Academic_EvalOutput + "', YrExp_MinComp_Details='" + objEmployeeCompetence.YrExp_MinComp_Details
                 + "', YrExp_EmpComp_Details='" + objEmployeeCompetence.YrExp_EmpComp_Details + "', YrExp_EvalOutput='" + objEmployeeCompetence.YrExp_EvalOutput
                 + "', Relevant_MinComp_Details='" + objEmployeeCompetence.Relevant_MinComp_Details + "', Relevant_EmpComp_Details='" + objEmployeeCompetence.Relevant_EmpComp_Details
                 + "', Relevant_EvalOutput='" + objEmployeeCompetence.Relevant_EvalOutput + "', Skills_MinComp_Details='" + objEmployeeCompetence.Skills_MinComp_Details
                 + "', Skills_EmpComp_Details='" + objEmployeeCompetence.Skills_EmpComp_Details + "', Skills_EvalOutput='" + objEmployeeCompetence.Skills_EvalOutput
                 + "', Emp_Suit_Hold_Pos='" + objEmployeeCompetence.Emp_Suit_Hold_Pos + "', Emp_Prom_Nxt_Pos='" + objEmployeeCompetence.Emp_Prom_Nxt_Pos
                 + "', Need_Of_Trainings='" + objEmployeeCompetence.Need_Of_Trainings + "', Emp_Not_Competent_Action='" + objEmployeeCompetence.Emp_Not_Competent_Action
                 + "', Eval_ReviewedBy='" + objEmployeeCompetence.Eval_ReviewedBy + "',Eval_ApprovedBy='" + objEmployeeCompetence.Eval_ApprovedBy
                 + "', Name='" + objEmployeeCompetence.Name + "', notified_to='" + objEmployeeCompetence.notified_to + "'";

                if (objEmployeeCompetence.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Evaluation_DoneOn='" + objEmployeeCompetence.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where CompetenceId='" + objEmployeeCompetence.CompetenceId + "'";

                if(objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (objModelList.lstEmployeeCompetenceEval.Count > 0)
                    {
                        objModelList.lstEmployeeCompetenceEval[0].CompetenceId = CompetenceId.ToString();

                        return FunAddTrainingList(objModelList);
                    }
                    return true;
                }

                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCompetenceEvaluation: " + ex.ToString());
            }
            return false;

        }

        public MultiSelectList GetMultiEvaluationOutputList()
        {
            DropdownRiskList lstImpact = new DropdownRiskList();
            lstImpact.lstMitigationimpact = new List<DropdownRiskItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Evaluation Output' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownRiskItems reg = new DropdownRiskItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstImpact.lstMitigationimpact.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiEvaluationOutputList: " + ex.ToString());
            }

            return new MultiSelectList(lstImpact.lstMitigationimpact, "Id", "Name");
        }
        internal string GetEvaluationOutputNameById(string sEvalOutput)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Evaluation Output' and (item_id='" + sEvalOutput + "' or item_desc='" + sEvalOutput + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEvaluationOutputNameById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetMultiEvaluationFrequencyList()
        {
            DropdownRiskList lstImpact = new DropdownRiskList();
            lstImpact.lstMitigationimpact = new List<DropdownRiskItems>();
            try
            {
                //string sSqlstmt = "select impact_id, impact_name from impact";
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Evaluation Frequency' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownRiskItems reg = new DropdownRiskItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstImpact.lstMitigationimpact.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiEvaluationFrequencyList: " + ex.ToString());
            }

            return new MultiSelectList(lstImpact.lstMitigationimpact, "Id", "Name");
        }
        internal string GetEvaluationFrequencyNameById(string sEvalFrequency)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Evaluation Frequency' and (item_id='" + sEvalFrequency + "' or item_desc='" + sEvalFrequency + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEvaluationFrequencyNameById: " + ex.ToString());
            }
            return "";
        }
    }

    public class EmployeeCompetenceEvalModelsList
    {
        public List<EmployeeCompetenceEvalModels> lstEmployeeCompetenceEval { get; set; }
    }

    public class DropdownCompetenceItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownCompetenceList
    {
        public List<DropdownCompetenceItems> lst { get; set; }
    }
}