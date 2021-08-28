using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using ISOStd.Models;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

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


        [Display(Name = "Evaluation By")]
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


        [Display(Name = "Evaluation Reviewed By")]
        public string Eval_ReviewedBy { get; set; }


        [Display(Name = "Evaluation Approved By")]
        public string Eval_ApprovedBy { get; set; }


        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

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

        internal bool FunAddCompetenceEvaluation(EmployeeCompetenceEvalModels objEmployeeCompetence)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_emp_competence_eval (Evaluated_Freq, Evalaution_Done_By, Academic_MinComp_Details,"
                    + " Academic_EmpComp_Details, Academic_EvalOutput, YrExp_MinComp_Details, YrExp_EmpComp_Details, YrExp_EvalOutput, Relevant_MinComp_Details,"
                    + "Relevant_EmpComp_Details, Relevant_EvalOutput, Skills_MinComp_Details, Skills_EmpComp_Details, Skills_EvalOutput, Emp_Suit_Hold_Pos,"
                    + " Emp_Prom_Nxt_Pos, Need_Of_Trainings, Emp_Not_Competent_Action, Eval_ReviewedBy, Eval_ApprovedBy,Name,branch";

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
                 + "','" + objEmployeeCompetence.Eval_ApprovedBy + "','" + objEmployeeCompetence.Name + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCompetenceEvaluation: " + ex.ToString());
            }
            return false;


        }


        internal bool FunUpdateCompetenceEvaluation(EmployeeCompetenceEvalModels objEmployeeCompetence)
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
                 + "', Name='" + objEmployeeCompetence.Name + "'";

                if (objEmployeeCompetence.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Evaluation_DoneOn='" + objEmployeeCompetence.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where CompetenceId='" + objEmployeeCompetence.CompetenceId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
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