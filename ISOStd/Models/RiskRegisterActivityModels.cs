using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{
    public class RiskRegisterActivityModels
    {
        clsGlobal objGlobalData = new clsGlobal();


        [Display(Name = "Id")]
        public string Risk_Reg_Activity_Id { get; set; }

        [Display(Name = "History Id")]
        public string Risk_Reg_Activity_Id_trans { get; set; }

        [Display(Name = "Department")]
        public string DeptId { get; set; }

        [Display(Name = "Activity No.")]
        public string Activity_No { get; set; }


        [Display(Name = "Activity")]
        public string Activity { get; set; }


        [Display(Name = "Activity Category")]
        public string Activity_Category { get; set; }
        
        [Display(Name = "Risk Type")]
        public string Risk_Type { get; set; }
        
        [Display(Name = "Activity Status")]
        public string Activity_Status { get; set; }
        
        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunDeleteRiskActivityDoc(string sRisk_Reg_Activity_Id)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_activity set Active=0 where Risk_Reg_Activity_Id='" + sRisk_Reg_Activity_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRiskActivityDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteRiskActivityHRRDoc(string srisk_hrr_id)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_hrrevaluation set Active=0 where risk_hrr_id='" + srisk_hrr_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRiskActivityHRRDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteRiskActivityEvlDoc(string sReg_Activity_eval_Id)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_activity_eval set Active=0 where Reg_Activity_eval_Id='" + sReg_Activity_eval_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteRiskActivityEvlDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddRiskRegisterActivity(RiskRegisterActivityModels objRiskRegisterActivity)
        {
            try
            {
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
              //string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_risk_register_activity (DeptId, Activity_No, Activity, Activity_Category, Risk_Type, Activity_Status, LoggedBy,branch,Location"
                    + ") values('" + objRiskRegisterActivity.DeptId + "','" + objRiskRegisterActivity.Activity_No
                    + "','" + objRiskRegisterActivity.Activity + "','" + objRiskRegisterActivity.Activity_Category + "','" + objRiskRegisterActivity.Risk_Type
                    + "','" + objRiskRegisterActivity.Activity_Status + "','" + user + "','" + objRiskRegisterActivity.branch + "','" + objRiskRegisterActivity.Location + "'";

                sSqlstmt = sSqlstmt + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRiskRegisterActivity: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateRiskRegisterActivity(RiskRegisterActivityModels objRiskRegisterActivity)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_activity set "
                    + " DeptId='" + objRiskRegisterActivity.DeptId + "', Activity_No='" + objRiskRegisterActivity.Activity_No
                    + "', Activity='" + objRiskRegisterActivity.Activity + "', Activity_Category='" + objRiskRegisterActivity.Activity_Category
                    + "', Risk_Type='" + objRiskRegisterActivity.Risk_Type + "', Activity_Status='" + objRiskRegisterActivity.Activity_Status
                    + "', Location='" + objRiskRegisterActivity.Location + "', DeptId='" + objRiskRegisterActivity.DeptId
                    + "' where Risk_Reg_Activity_Id='" + Risk_Reg_Activity_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRiskRegisterActivity: " + ex.ToString());
            }
            return false;
        }

        public string GetRiskAcivityNameById(string Risk_Reg_Activity_Id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select Activity from t_risk_register_activity where Risk_Reg_Activity_Id='" + Risk_Reg_Activity_Id + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Activity"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRiskAcivityNameById: " + ex.ToString());
            }
            return "";
        }
           
        public string GetRiskTypeIdbyName(string item_desc)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id from dropdownitems where item_desc='" + item_desc + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["item_id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRiskTypeIdbyName: " + ex.ToString());
            }
            return "";
        }
                      
    }



    public class RiskRegisterActivityEvaluationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string Reg_Activity_eval_Id { get; set; }


        [Display(Name = "Activity")]
        public string Risk_Reg_Activity_Id { get; set; }


        [Display(Name = "Evaluation Date")]
        public DateTime Eval_Date { get; set; }

        [Display(Name = "Risk Evaluated By")]
        public string EvalBy { get; set; }


        [Display(Name = "Reviewed By")]
        public string Reviewer_QHSE { get; set; }


        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Impacts")]
        public string Consequence { get; set; }


        [Display(Name = "Control Measure")]
        public string Cur_Opt_Ctrl { get; set; }


        [Display(Name = "Severity")]
        public string Severity { get; set; }


        [Display(Name = "Probability")]
        public string Probability { get; set; }

        [Display(Name = "Exposure")]
        public string Exposure_id { get; set; }

        [Display(Name = "Risk Rating")]
        public string Risk_Rating { get; set; }


        [Display(Name = "Is there any need of additional Operational Controls")]
        public string Add_Opt_Ctrl { get; set; }


        [Display(Name = "Type of Operational Control to be implemented")]
        public string Opt_Ctrl_Implt { get; set; }


        [Display(Name = "Description of Operation Control")]
        public string Desc_Opt_ctrl { get; set; }


        [Display(Name = "Due Date to Implement the Operational Controls")]
        public DateTime Due_Date { get; set; }


        [Display(Name = "Re-Evaluation Date")]
        public DateTime ReEval_Date { get; set; }


        [Display(Name = "Prepared By")]
        public string Action_TakenBy { get; set; }


        [Display(Name = "Color Code")]
        public string color_code { get; set; }

        [Display(Name = "Department")]
        public string DeptId { get; set; }

        [Display(Name = "Activity No.")]
        public string Activity_No { get; set; }


        [Display(Name = "Activity")]
        public string Activity { get; set; }


        [Display(Name = "Activity Category")]
        public string Activity_Category { get; set; }


        [Display(Name = "Risk Type")]
        public string Risk_Type { get; set; }


        [Display(Name = "Activity Status")]
        public string Activity_Status { get; set; }


        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Aspects")]
        public string hazard { get; set; }

        [Display(Name = "Applicable law & other requirements")]
        public string Appl_law { get; set; }

        [Display(Name = "Id")]
        public string risk_hrr_id { get; set; }

        [Display(Name = "Evaluation Status")]
        public string Evaluation_status { get; set; }

        [Display(Name = "Person Responsible")]
        public string Person_resp { get; set; }

        [Display(Name = "Approve Status")]
        public string Approve_status { get; set; }

        public string Risk_Id { get; set; }


        public DataSet GetHRRMatrixColordetails()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Get_HrrColourcode", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                //cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetHRRMatrixColordetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        public DataSet GetEVNMatrixColordetails()
        {
            DataSet dsData = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand("Get_EnvColourcode", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                //cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsData);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEVNMatrixColordetails: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }
            return dsData;
        }

        internal bool FunAddRiskRegisterHRREval(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval)
        {
            try
            {
                string sColumn = "", sValues = "";

                string sSqlstmt = "insert into t_risk_register_hrrevaluation (Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id"
                + ",Evaluation_status,Cur_Opt_Ctrl,Person_resp,Action_TakenBy,Reviewer_QHSE,ApprovedBy";

                if (objRiskRegisterActivityEval.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Eval_Date";
                    sValues = sValues + ", '" + objRiskRegisterActivityEval.Eval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                if (objRiskRegisterActivityEval.ReEval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", ReEval_Date";
                    sValues = sValues + ", '" + objRiskRegisterActivityEval.ReEval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objRiskRegisterActivityEval.Risk_Reg_Activity_Id + "','" + objRiskRegisterActivityEval.hazard
                    + "','" + objRiskRegisterActivityEval.Severity + "','" + objRiskRegisterActivityEval.Probability + "','" + objRiskRegisterActivityEval.Exposure_id
                    + "','" + objRiskRegisterActivityEval.Evaluation_status + "','" + objRiskRegisterActivityEval.Cur_Opt_Ctrl + "','" + objRiskRegisterActivityEval.Person_resp
                    + "','" + objRiskRegisterActivityEval.Action_TakenBy + "','" + objRiskRegisterActivityEval.Reviewer_QHSE + "','" + objRiskRegisterActivityEval.ApprovedBy + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    SendReviewerHRREmail(objRiskRegisterActivityEval);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRiskRegisterActivityEval: " + ex.ToString());
            }
            return false;
        }
        internal bool SendReviewerHRREmail(RiskRegisterActivityEvaluationModels objRisk)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRiskEval = new RiskRegisterActivityEvaluationModels();
                string sHeader = "";
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objRisk.Action_TakenBy) + "," + objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');
                string sUserName = objGlobalData.GetEmpHrNameById(objRisk.Reviewer_QHSE);
                string sToEmailId = objGlobalData.GetHrEmpEmailIdById(objRisk.Reviewer_QHSE);
                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');
                sHeader = "<tr><td ><b>Hazard:<b></td> <td >"
                       + objRisk.hazard + "</td></tr>"
                       + "<tr><td ><b>Severity:<b></td> <td >" + objGlobalData.GetDropdownitemById(objRisk.Severity) + "</td></tr>"
                       + "<tr><td ><b>Probability:<b></td> <td >" + objGlobalData.GetDropdownitemById(objRisk.Probability) + "</td></tr>"
                       + "<tr><td ><b>Exposure:<b></td> <td >" + objGlobalData.GetDropdownitemById(objRisk.Exposure_id) + "</td></tr>"
                       + "<tr><td ><b>Control Measures:<b></td> <td >" + objRisk.Cur_Opt_Ctrl + "</td></tr>"
                       + "<tr><td ><b>Person Responsible:<b></td> <td >" + objRisk.Person_resp + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "RiskActivity", sHeader, "", "Risk Register Activity Evaluation for review");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendReviewerHRREmail: " + ex.ToString());
            }
            return false;
        }
        internal bool SendHRRApproverEmail(string risk_hrr_id)
        {
            try
            {
                string sSqlstmt = "select DeptId,Activity,Activity_Category,Risk_Type,Activity_Status,risk_hrr_id,"
                                 + "t.Risk_Reg_Activity_Id,hazard,Severity,Probability,Exposure_id,Evaluation_status,Cur_Opt_Ctrl,Person_resp,"
                                 + "Eval_Date,ReEval_Date,Action_TakenBy,Reviewer_QHSE,ApprovedBy from t_risk_register_activity t,t_risk_register_hrrevaluation tt where"
                                 + " t.Risk_Reg_Activity_Id=tt.Risk_Reg_Activity_Id and risk_hrr_id='" + risk_hrr_id + "'";

                DataSet dsRisk = objGlobalData.Getdetails(sSqlstmt);
                if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                {
                    RiskRegisterActivityEvaluationModels objRiskEval = new RiskRegisterActivityEvaluationModels();
                    string sHeader = "";
                    string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["Action_TakenBy"].ToString()) + "," + objGlobalData.GetHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["Reviewer_QHSE"].ToString());
                    sCCList = Regex.Replace(sCCList, ",+", ",");
                    sCCList = sCCList.Trim();
                    sCCList = sCCList.TrimEnd(',');
                    sCCList = sCCList.TrimStart(',');
                    string sUserName = objGlobalData.GetEmpHrNameById(dsRisk.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                    sToEmailId = sToEmailId.Trim();
                    sToEmailId = sToEmailId.TrimEnd(',');
                    sToEmailId = sToEmailId.TrimStart(',');

                    sHeader = "<tr><td ><b>Hazard:<b></td> <td >"
                    + dsRisk.Tables[0].Rows[0]["hazard"].ToString() + "</td></tr>"
                    + "<tr><td ><b>Severity:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsRisk.Tables[0].Rows[0]["Severity"].ToString()) + "</td></tr>"
                    + "<tr><td ><b>Probability:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsRisk.Tables[0].Rows[0]["Probability"].ToString()) + "</td></tr>"
                    + "<tr><td ><b>Exposure:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsRisk.Tables[0].Rows[0]["Exposure_id"].ToString()) + "</td></tr>"
                    + "<tr><td ><b>Control Measures:<b></td> <td >" + dsRisk.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString() + "</td></tr>"
                    + "<tr><td ><b>Person Responsible:<b></td> <td >" + dsRisk.Tables[0].Rows[0]["Person_resp"].ToString() + "</td></tr>";


                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "RiskActivityAppr", sHeader, "", "Risk Register Activity for Approval");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendApproverEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddRiskRegisterActivityEval(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval)
        {
            try
            {
                string sColumn = "", sValues = "";

                string sSqlstmt = "insert into t_risk_register_activity_eval (Risk_Reg_Activity_Id, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence, Cur_Opt_Ctrl, Severity,"
                    + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl, Action_TakenBy, Comments,hazard,Appl_law,Exposure_id";

                if (objRiskRegisterActivityEval.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Eval_Date";
                    sValues = sValues + ", '" + objRiskRegisterActivityEval.Eval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRiskRegisterActivityEval.Due_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Due_Date";
                    sValues = sValues + ", '" + objRiskRegisterActivityEval.Due_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRiskRegisterActivityEval.ReEval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", ReEval_Date";
                    sValues = sValues + ", '" + objRiskRegisterActivityEval.ReEval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objRiskRegisterActivityEval.Risk_Reg_Activity_Id + "','" + objRiskRegisterActivityEval.EvalBy
                    + "','" + objRiskRegisterActivityEval.Reviewer_QHSE + "','" + objRiskRegisterActivityEval.ApprovedBy + "','" + objRiskRegisterActivityEval.Consequence
                    + "','" + objRiskRegisterActivityEval.Cur_Opt_Ctrl + "','" + objRiskRegisterActivityEval.Severity + "','" + objRiskRegisterActivityEval.Probability
                    + "','" + objRiskRegisterActivityEval.Risk_Rating + "','" + objRiskRegisterActivityEval.Add_Opt_Ctrl + "','" + objRiskRegisterActivityEval.Opt_Ctrl_Implt
                    + "','" + objRiskRegisterActivityEval.Desc_Opt_ctrl + "','" + objRiskRegisterActivityEval.Action_TakenBy
                    + "','" + objRiskRegisterActivityEval.Comments + "','" + objRiskRegisterActivityEval.hazard + "','" + objRiskRegisterActivityEval.Appl_law + "','" + objRiskRegisterActivityEval.Exposure_id + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    SendReviewerEmail(objRiskRegisterActivityEval);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddRiskRegisterActivityEval: " + ex.ToString());
            }
            return false;
        }
        internal bool SendReviewerEmail(RiskRegisterActivityEvaluationModels objRisk)
        {
            try
            {
                RiskRegisterActivityEvaluationModels objRiskEval = new RiskRegisterActivityEvaluationModels();
                string sHeader = "";
                string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(objRisk.Action_TakenBy) + "," + objGlobalData.GetHrEmpEmailIdById(objGlobalData.GetCurrentUserSession().empid);
                sCCList = Regex.Replace(sCCList, ",+", ",");
                sCCList = sCCList.Trim();
                sCCList = sCCList.TrimEnd(',');
                sCCList = sCCList.TrimStart(',');
                string sUserName = objGlobalData.GetEmpHrNameById(objRisk.Reviewer_QHSE);
                string sToEmailId = objGlobalData.GetHrEmpEmailIdById(objRisk.Reviewer_QHSE);
                sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                sToEmailId = sToEmailId.Trim();
                sToEmailId = sToEmailId.TrimEnd(',');
                sToEmailId = sToEmailId.TrimStart(',');
                sHeader = "<tr><td ><b>Aspects:<b></td> <td >"
                       + objRisk.hazard + "</td></tr>"
                       + "<tr><td ><b>Impacts:<b></td> <td >" + objRisk.Consequence + "</td></tr>"
                        + "<tr><td ><b>Severity:<b></td> <td >" + objGlobalData.GetDropdownitemById(objRisk.Severity) + "</td></tr>"
                       + "<tr><td ><b>Probability:<b></td> <td >" + objGlobalData.GetDropdownitemById(objRisk.Probability) + "</td></tr>"
                       + "<tr><td ><b>Control Measures:<b></td> <td >" + objRisk.Cur_Opt_Ctrl + "</td></tr>";

                Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "RiskActivity", sHeader, "", "Risk Register Activity Evaluation for review");
                objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendReviewerEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool SendApproverEmail(string Regeval_Id)
        {
            try
            {
                string sSqlstmt = "select Reg_Activity_eval_Id, trEval.Risk_Reg_Activity_Id, Eval_Date, EvalBy, Reviewer_QHSE, ApprovedBy,Consequence,hazard, Cur_Opt_Ctrl, Severity, "
                                        + " Probability, Risk_Rating, Add_Opt_Ctrl, Opt_Ctrl_Implt, Desc_Opt_ctrl,  Due_Date, ReEval_Date, Action_TakenBy, "
                                        + " DeptId, Activity_No, Activity, Activity_Category,Appl_law, Risk_Type, Activity_Status, trEval.Comments,Exposure_id from t_risk_register_activity_eval as trEval, "
                                        + "t_risk_register_activity as tract where trEval.Risk_Reg_Activity_Id = tract.Risk_Reg_Activity_Id and Reg_Activity_eval_Id='" + Regeval_Id + "'";

                DataSet dsRisk = objGlobalData.Getdetails(sSqlstmt);
                if (dsRisk.Tables.Count > 0 && dsRisk.Tables[0].Rows.Count > 0)
                {
                    RiskRegisterActivityEvaluationModels objRiskEval = new RiskRegisterActivityEvaluationModels();
                    string sHeader = "";
                    string sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["Action_TakenBy"].ToString()) + "," + objGlobalData.GetHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["Reviewer_QHSE"].ToString());
                    sCCList = Regex.Replace(sCCList, ",+", ",");
                    sCCList = sCCList.Trim();
                    sCCList = sCCList.TrimEnd(',');
                    sCCList = sCCList.TrimStart(',');
                    string sUserName = objGlobalData.GetEmpHrNameById(dsRisk.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    string sToEmailId = objGlobalData.GetHrEmpEmailIdById(dsRisk.Tables[0].Rows[0]["ApprovedBy"].ToString());
                    sToEmailId = Regex.Replace(sToEmailId, ",+", ",");
                    sToEmailId = sToEmailId.Trim();
                    sToEmailId = sToEmailId.TrimEnd(',');
                    sToEmailId = sToEmailId.TrimStart(',');
                    sHeader = "<tr><td ><b>Aspects:<b></td> <td >"
                           + dsRisk.Tables[0].Rows[0]["hazard"].ToString() + "</td></tr>"
                           + "<tr><td ><b>Impacts:<b></td> <td >" + dsRisk.Tables[0].Rows[0]["Consequence"].ToString() + "</td></tr>"
                            + "<tr><td ><b>Severity:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsRisk.Tables[0].Rows[0]["Severity"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>Probability:<b></td> <td >" + objGlobalData.GetDropdownitemById(dsRisk.Tables[0].Rows[0]["Probability"].ToString()) + "</td></tr>"
                           + "<tr><td ><b>Control Measures:<b></td> <td >" + dsRisk.Tables[0].Rows[0]["Cur_Opt_Ctrl"].ToString() + "</td></tr>";

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUserName, "RiskActivityAppr", sHeader, "", "Risk Register Activity for Approval");
                    objGlobalData.Sendmail(sToEmailId, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendApproverEmail: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateRiskRegisterActivityEval(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_activity_eval set EvalBy='" + objRiskRegisterActivityEval.EvalBy + "', Reviewer_QHSE='" + objRiskRegisterActivityEval.Reviewer_QHSE
                    + "', ApprovedBy='" + objRiskRegisterActivityEval.ApprovedBy + "', Consequence='" + objRiskRegisterActivityEval.Consequence
                    + "', Cur_Opt_Ctrl='" + objRiskRegisterActivityEval.Cur_Opt_Ctrl + "', Severity='" + objRiskRegisterActivityEval.Severity
                    + "', Probability='" + objRiskRegisterActivityEval.Probability + "', Risk_Rating='" + objRiskRegisterActivityEval.Risk_Rating
                    + "', Add_Opt_Ctrl='" + objRiskRegisterActivityEval.Add_Opt_Ctrl + "', Opt_Ctrl_Implt='" + objRiskRegisterActivityEval.Opt_Ctrl_Implt
                    + "', Desc_Opt_ctrl='" + objRiskRegisterActivityEval.Desc_Opt_ctrl + "', Action_TakenBy='" + objRiskRegisterActivityEval.Action_TakenBy
                    + "', Comments='" + objRiskRegisterActivityEval.Comments + "', hazard='" + objRiskRegisterActivityEval.hazard + "',Appl_law='" + objRiskRegisterActivityEval.Appl_law + "'"
                    + ",Exposure_id='" + objRiskRegisterActivityEval.Exposure_id + "'";

                if (objRiskRegisterActivityEval.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_Date='" + objRiskRegisterActivityEval.Eval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRiskRegisterActivityEval.Due_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Due_Date='" + objRiskRegisterActivityEval.Due_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objRiskRegisterActivityEval.ReEval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ReEval_Date='" + objRiskRegisterActivityEval.ReEval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Reg_Activity_eval_Id='" + objRiskRegisterActivityEval.Reg_Activity_eval_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRiskRegisterActivityEval: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateActivityEvalReviewUpdate(string Regeval_Id)
        {
            try
            {
                string sqlstmt = "update t_risk_register_activity_eval set Review_status=1 where Reg_Activity_eval_Id='" + Regeval_Id + "'";
                if (objGlobalData.ExecuteQuery(sqlstmt))
                {
                    SendApproverEmail(Regeval_Id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActivityEvalReviewUpdate: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateActivityEvalApproveUpdate(string Regeval_Id)
        {
            try
            {
                string sqlstmt = "update t_risk_register_activity_eval set Approve_status=1 where Reg_Activity_eval_Id='" + Regeval_Id + "'";
                return objGlobalData.ExecuteQuery(sqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActivityEvalApproveUpdate: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateActivityHRRReviewUpdate(string risk_hrr_id)
        {
            try
            {
                string sqlstmt = "update t_risk_register_hrrevaluation set Review_status=1 where risk_hrr_id='" + risk_hrr_id + "'";
                if (objGlobalData.ExecuteQuery(sqlstmt))
                {
                    SendHRRApproverEmail(risk_hrr_id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActivityHRRReviewUpdate: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateActivityHRRApproveUpdate(string risk_hrr_id)
        {
            try
            {
                string sqlstmt = "update t_risk_register_hrrevaluation set Approve_status=1 where risk_hrr_id='" + risk_hrr_id + "'";
                if (objGlobalData.ExecuteQuery(sqlstmt))
                {
                    SendHRRApproverEmail(risk_hrr_id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActivityHRRApproveUpdate: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateRiskRegisterHRRActivityEval(RiskRegisterActivityEvaluationModels objRiskRegisterActivityEval)
        {
            try
            {
                string sSqlstmt = "update t_risk_register_hrrevaluation set hazard='" + objRiskRegisterActivityEval.hazard + "', Severity='" + objRiskRegisterActivityEval.Severity
                    + "', Probability='" + objRiskRegisterActivityEval.Probability + "', Exposure_id='" + objRiskRegisterActivityEval.Exposure_id
                    + "', Evaluation_status='" + objRiskRegisterActivityEval.Evaluation_status + "', Cur_Opt_Ctrl='" + objRiskRegisterActivityEval.Cur_Opt_Ctrl
                    + "', Person_resp='" + objRiskRegisterActivityEval.Person_resp + "', Action_TakenBy='" + objRiskRegisterActivityEval.Action_TakenBy
                    + "', Reviewer_QHSE='" + objRiskRegisterActivityEval.Reviewer_QHSE + "', ApprovedBy='" + objRiskRegisterActivityEval.ApprovedBy + "'";


                if (objRiskRegisterActivityEval.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_Date='" + objRiskRegisterActivityEval.Eval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }


                if (objRiskRegisterActivityEval.ReEval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", ReEval_Date='" + objRiskRegisterActivityEval.ReEval_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where risk_hrr_id='" + objRiskRegisterActivityEval.risk_hrr_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateRiskRegisterActivityEval: " + ex.ToString());
            }
            return false;
        }


        public List<DropdownHazardItems> GetMultiHRRRiskExposureForMatrix()
        {
            List<DropdownHazardItems> listExposure = new List<DropdownHazardItems>();
            try
            {
                string sSqlstmt = "select item_fulldesc as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Risk-Exposure' order by Id desc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        listExposure.Add(new DropdownHazardItems() { Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(), Name = dsEmp.Tables[0].Rows[i]["Name"].ToString() });

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHRRRiskSeveirtyForMatrix: " + ex.ToString());
            }
            return (listExposure);
        }
        public List<DropdownHazardItems> GetMultiHRRRiskSeveirtyForMatrix()
        {
            List<DropdownHazardItems> listExposure = new List<DropdownHazardItems>();
            try
            {
                string sSqlstmt = "select item_fulldesc as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='HRR-Severity' order by Id desc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        listExposure.Add(new DropdownHazardItems() { Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(), Name = dsEmp.Tables[0].Rows[i]["Name"].ToString() });

                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiHRRRiskSeveirtyForMatrix: " + ex.ToString());
            }
            return (listExposure);
        }
   
    }


    public class RiskRegisterActivityModelsList
    {
        public List<RiskRegisterActivityModels> lstRiskRegisterActivity { get; set; }
    }

    public class RiskRegisterActivityEvaluationModelsList
    {
        public List<RiskRegisterActivityEvaluationModels> lstRiskRegisterActivityEval { get; set; }
    }


    public class RiskData
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RiskDataList
    {
        public List<RiskData> lstRiskData { get; set; }
    }

    public class DropdownHazardItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownHazardList
    {
        public List<DropdownHazardItems> lst { get; set; }
    }

    public class DropdownRiskRegisterItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownRisRigisterkList
    {
        public List<DropdownRiskRegisterItems> lst { get; set; }
    }
}