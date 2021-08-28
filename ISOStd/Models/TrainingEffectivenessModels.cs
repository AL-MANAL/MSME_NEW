using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class TrainingEffectivenessModels 
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_trainings_evalution
        [Display(Name = "Id")]
        public string id_training_evalution { get; set; }

        [Display(Name = "Report Number")]
        public string report_no { get; set; }

        [Display(Name = "Performance Monitor Period")]
        public string perf_monitor_period { get; set; }

        [Display(Name = "Employee Name")]
        public string emp_name { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Is Employee Performance Improved ?")]
        public string emp_perf_improved { get; set; }
        
        [Display(Name = "Action to be taken to improve the employee competency")]
        public string action_taken { get; set; }

        [Display(Name = "Was training effective to achieve the planned objective(s)?")]
        public string planned_objective { get; set; }

        [Display(Name = "Logged By")]
        public string logged_by { get; set; }

        public string Training_Topic { get; set; }
        public string Sourceof_Training { get; set; }
        public string Trainer_Name { get; set; }

        //t_trainings_evalution
        [Display(Name = "Id")]
        public string id_train_trans { get; set; }

        [Display(Name = "Id")]
        public string SQId { get; set; }

        [Display(Name = "Id")]
        public string SQ_OptionsId { get; set; }

        public MultiSelectList GetTrainingReportNoList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select TrainingID as Id, report_no as Name from t_trainings where RequestStatus=1 " +
                    "and Training_Actual_Date > '01/01/0001' and Training_Actual_Completion_Date  > '01/01/0001' order by TrainingID Desc";

                DataSet dsReportType = objGlobalData.Getdetails(sSsqlstmt);
                if (dsReportType.Tables.Count > 0 && dsReportType.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsReportType.Tables[0].Rows.Count; i++)
                    {
                        DropdownItems objReport = new DropdownItems()
                        {
                            Id = dsReportType.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsReportType.Tables[0].Rows[i]["Name"].ToString()
                        };
                        objReportList.lstDropdown.Add(objReport);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingReportNoList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

        public string GetTrainingReportNoById(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select report_no as Name from t_trainings where TrainingID='" + item_id + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingReportNoById: " + ex.ToString());
            }
            return "";
        }

        public string GetTrainingTopicByReportNo(string item_id)
        {
            try
            {
                if (item_id != "")
                {
                    string sSsqlstmt = "select Training_Topic as Name from t_trainings where TrainingID='" + item_id + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTrainingTopicByReportNo: " + ex.ToString());
            }
            return "";
        }

        public string GetSorceofTrainingByReportNo(string item_id)
        {
            try
            {
                
                if (item_id != "")
                {
                    string sSsqlstmt = "select Sourceof_Training as Name from t_trainings where TrainingID='" + item_id + "'";
                    DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        return (dsData.Tables[0].Rows[0]["Name"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSorceofTrainingByReportNo: " + ex.ToString());
            }
            return "";
        }


        public MultiSelectList GetTainingEmpList(string report_no = "")
        {
            EmployeeList emplist = new EmployeeList();
            emplist.EmpList = new List<Employee>();
            try
            {
                string user = objGlobalData.GetCurrentUserSession().empid;
                //string sSqlstmt = "select concat(emp_firstname,' ',ifnull(emp_middlename,' '),' ',ifnull(emp_lastname,' ')) as Empname, emp_no as Empid from t_hr_employee where emp_status=1 order by emp_firstname asc";
                string sSqlstmt = "select distinct emp_no,concat(emp_firstname, ' ', ifnull(emp_middlename, ' '), ' ', ifnull(emp_lastname, ' ')) Empname" +
                    " from t_hr_employee a, t_trainings  b where TrainingID = '" + report_no + "' and active = 1 and find_in_set(a.emp_no, concat(Attendees,',',ApprovedBy,',',Training_Requested_By)) > 0" +
                    "&& find_in_set(a.EvaluatedBy,("+user+") )> 0  ";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);// and CompanyId='" + sCompanyId+"'");

                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        Employee emp = new Employee()
                        {
                            Empid = dsEmp.Tables[0].Rows[i]["emp_no"].ToString(),
                            Empname = Regex.Replace(dsEmp.Tables[0].Rows[i]["Empname"].ToString(), " +", " ")
                        };
                        emp.Empname = emp.Empname.Trim();
                        emplist.EmpList.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetTainingEmpList: " + ex.ToString());
            }
            return new MultiSelectList(emplist.EmpList, "Empid", "Empname");
        }

        internal bool FunAddTrainingEvaluation(TrainingEffectivenessModels ObjModel, TrainingEffectivenessModelsList ObjList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string sBranch = objGlobalData.GetCurrentUserSession().division;


                string sSqlstmt = "insert into t_trainings_evalution (report_no, perf_monitor_period, emp_name, upload, comments,emp_perf_improved," +
                    "action_taken,planned_objective,logged_by,branch";

                //if (ObjModel.evalu_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sColumn = sColumn + ", evalu_date";
                //    sValues = sValues + ", '" + ObjModel.evalu_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                sSqlstmt = sSqlstmt + sColumn + ") values('" + ObjModel.report_no + "','" + ObjModel.perf_monitor_period + "','" + ObjModel.emp_name
                 + "','" + ObjModel.upload + "','" + ObjModel.comments + "','" + ObjModel.emp_perf_improved + "','" + ObjModel.action_taken + "','" + ObjModel.planned_objective + "','" + user + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int iId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iId))
                {
                    TrainingEffectivenessModels objElement = new TrainingEffectivenessModels();

                    ObjList.TrainList[0].id_training_evalution = iId.ToString();
                    objElement.FunAddTrainingEvaluationList(ObjList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool FunAddTrainingEvaluationList(TrainingEffectivenessModelsList ObjList)
        {
            try
            {
                if (ObjList.TrainList.Count > 0)
                {
                    string sSqlstmt = "delete from t_trainings_evalution_trans where id_training_evalution='"
                        + ObjList.TrainList[0].id_training_evalution + "'; ";
                    for (int i = 0; i < ObjList.TrainList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_trainings_evalution_trans (id_training_evalution, SQId, SQ_OptionsId"
                        + ") values('" + ObjList.TrainList[0].id_training_evalution + "','" + ObjList.TrainList[i].SQId
                        + "','" + ObjList.TrainList[i].SQ_OptionsId + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingEvaluationList: " + ex.ToString());

            }
            return false;
        }


        internal bool FunUpdateTrainingEvaluation(TrainingEffectivenessModels ObjModel, TrainingEffectivenessModelsList ObjList)
        {
            try
            {
                string sSqlstmt = "update t_trainings_evalution set perf_monitor_period='" + ObjModel.perf_monitor_period + "', emp_name='" + ObjModel.emp_name
                    + "', comments='" + ObjModel.comments + "', upload='" + ObjModel.upload + "', emp_perf_improved='" + ObjModel.emp_perf_improved + "', action_taken='" + ObjModel.action_taken
                     + "', planned_objective='" + ObjModel.planned_objective + "'";

                //if (ObjModel.upload != null)
                //{
                //    sSqlstmt = sSqlstmt + ", upload='" + ObjModel.upload + "' ";
                //}

                //if (ObjModel.evalu_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", evalu_date='" + ObjModel.evalu_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}


                sSqlstmt = sSqlstmt + " where id_training_evalution='" + ObjModel.id_training_evalution + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    TrainingEffectivenessModels objElement = new TrainingEffectivenessModels();
                    objElement.FunAddTrainingEvaluationList(ObjList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingEvaluation: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDeleteTrainingEvaluation(string sid_training_evalution)
        {
            try
            {
                string sSqlstmt = "update t_trainings_evalution set Active=0 where id_training_evalution='" + sid_training_evalution + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrainingEvaluation: " + ex.ToString());

            }
            return false;
        }

        public string GetFunRatingId(string rating)
        {
            try
            {

                string sSqlstmt = "select SQ_OptionsId from t_surveyquestion_options a,t_survey_type b where" +
                    " a.Survey_TypeId = b.Survey_TypeId and RatingOptions = '" + rating + "' and b.TypeName = 'Training Effectiveness Survey' and a.Active = 1";
                DataSet dsrate = objGlobalData.Getdetails(sSqlstmt);
                if (dsrate.Tables.Count > 0 && dsrate.Tables[0].Rows.Count > 0)
                {
                    return (dsrate.Tables[0].Rows[0]["SQ_OptionsId"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFunRatingId: " + ex.ToString());
            }
            return "";
        }
    }
    public class TrainingEffectivenessModelsList
    {
        public List<TrainingEffectivenessModels> TrainList { get; set; }
    }
}