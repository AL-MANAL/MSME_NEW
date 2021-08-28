using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class TrainingStaffModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_training_staff
        [Display(Name = "Id")]
        public string id_training { get; set; }

        [Display(Name ="Employee Name")]
        public string employee { get; set; }

        [Display(Name = "Date")]
        public DateTime date_taining { get; set; }

        [Display(Name = "Dept Head")]
        public string dept_head { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Comments of Dept Head")]
        public string comment_head { get; set; }

        //t_training_staff_trans
        [Display(Name ="Id")]
        public string id_training_trans { get; set; }

        [Display(Name = "Type of Training")]
        public string training_type { get; set; }

        [Display(Name = "Required On or Before")]
        public DateTime scheduled_date { get; set; }

        [Display(Name = "Justification")]
        public string justification { get; set; }

        [Display(Name = "Acceptance")]
        public string acceptance { get; set; }

        [Display(Name = "Suggestion")]
        public string suggestion { get; set; }

        public string division { get; set; }
        public string dept { get; set; }
        public string designation { get; set; }

        [Display(Name = "Training Start Date")]
        public DateTime start_date { get; set; }

        [Display(Name = "Training Completed On")]
        public DateTime end_date { get; set; }

        [Display(Name = "Training Conducted By")]
        public string conducted_by { get; set; }

        [Display(Name = "Training Details")]
        public string details { get; set; }

        [Display(Name = "Upload Documents")]
        public string upload { get; set; }

        [Display(Name = "Was Training Effective")]
        public string effective { get; set; }

        [Display(Name = "If training was not effective, specify the reasons")]
        public string reason { get; set; }

        [Display(Name = "Recommendations for the improvement")]
        public string recommendation { get; set; }

        [Display(Name = "Further training required")]
        public string traing_required { get; set; }

        [Display(Name = "Notify to")]
        public string notify_to { get; set; }

        [Display(Name = "Training Status")]
        public string training_status { get; set; }
             

                
        internal bool FunAddTrainingStaff(TrainingStaffModels objMdl, TrainingStaffModelsList objList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_training_staff (employee,dept_head,comments,loggedby,branch";
                string sFields = "", sFieldValue = "";
                if (objMdl.date_taining != null && objMdl.date_taining > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", date_taining";
                    sFieldValue = sFieldValue + ", '" + objMdl.date_taining.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objMdl.employee + "','" + objMdl.dept_head + "','" + objMdl.comments 
                    + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + sBranch + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_training = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_training))
                {
                    if (id_training > 0 && Convert.ToInt32(objList.TrainList.Count) > 0)
                    {
                        objList.TrainList[0].id_training = id_training.ToString();
                        FunAddTrainingStaffTans(objList);
                    }

                    //if (id_training > 0)
                    //{
                    //    string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                    //    DataSet dsData = objGlobalData.GetReportNo("FIRE", "", LocationName);
                    //    if (dsData != null && dsData.Tables.Count > 0)
                    //    {
                    //        report_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    //    }
                    //    string sql = "update t_training_staff set report_no='" + report_no + "' where id_training = '" + id_training + "'";

                    //    return objGlobalData.ExecuteQuery(sql);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingStaff: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunAddTrainingStaffTans(TrainingStaffModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_training_staff_trans where id_training='" + objList.TrainList[0].id_training + "'; ";

                for (int i = 0; i < objList.TrainList.Count; i++)
                {
                    string sid_training_trans = "null";
                    string sscheduled_date = "";

                    //if (objList.TrainList[i].id_fireequip_trans != null)
                    //{
                    //    sid_fireequip_trans = objList.TrainList[i].id_fireequip_trans;
                    //}
                    if (objList.TrainList[i].scheduled_date != null && objList.TrainList[i].scheduled_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sscheduled_date = objList.TrainList[i].scheduled_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_training_staff_trans (id_training_trans,id_training,training_type," +
                        "justification,scheduled_date)"
                    + " values(" + sid_training_trans + "," + objList.TrainList[0].id_training + ",'"
                    + objList.TrainList[i].training_type + "','" + objList.TrainList[i].justification + "','"
                    + sscheduled_date  + "')"
                    + " ON DUPLICATE KEY UPDATE" + " id_training_trans= values(id_training_trans), id_training= values(id_training), training_type = values(training_type)," +
                    " justification= values(justification),  scheduled_date= values(scheduled_date); ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingStaffTans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddTrainingDept_headTans(TrainingStaffModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_training_staff_trans where id_training='" + objList.TrainList[0].id_training + "'; ";

                for (int i = 0; i < objList.TrainList.Count; i++)
                {
                    string sid_training_trans = "null";
                    string sscheduled_date = "";

                    if (objList.TrainList[i].scheduled_date != null && objList.TrainList[i].scheduled_date > Convert.ToDateTime("01/01/0001"))
                    {
                        sscheduled_date = objList.TrainList[i].scheduled_date.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " insert into t_training_staff_trans (id_training_trans,id_training,training_type," +
                        "justification,scheduled_date,acceptance,suggestion)"
                    + " values(" + sid_training_trans + "," + objList.TrainList[0].id_training + ",'"
                    + objList.TrainList[i].training_type + "','" + objList.TrainList[i].justification + "','"
                    + sscheduled_date + "','" + objList.TrainList[i].acceptance + "','" + objList.TrainList[i].suggestion + "')"
                    + " ON DUPLICATE KEY UPDATE" + " id_training_trans= values(id_training_trans), id_training= values(id_training), training_type = values(training_type)," +
                    " justification= values(justification),  scheduled_date= values(scheduled_date),acceptance= values(acceptance),suggestion= values(suggestion) ; ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingDept_headTans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTrainingStaff(TrainingStaffModels objMdl, TrainingStaffModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_training_staff set employee ='" + objMdl.employee + "', dept_head = '" + objMdl.dept_head
                   + "', comments= '" + objMdl.comments + "'";

                if (objMdl.date_taining != null && objMdl.date_taining > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_taining='" + objMdl.date_taining.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_training='" + objMdl.id_training + "'";

                int id_training = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_training))
                {
                    if (Convert.ToInt32(objList.TrainList.Count) > 0)
                    {
                        objList.TrainList[0].id_training = objMdl.id_training;
                        FunAddTrainingStaffTans(objList);
                    }
                    else
                    {
                        FunUpdateTrainingStaffTrans(objMdl.id_training);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingStaff: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTrainingStaffTrans(string sid_training)
        {
            try
            {
                string sSqlstmt = "delete from t_training_staff_trans where id_training='" + sid_training + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingStaffTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTrainingDept_head(TrainingStaffModels objMdl, TrainingStaffModelsList objList)
        {
            try
            {
                string sSqlstmt = "update t_training_staff set employee ='" + objMdl.employee + "', dept_head = '" + objMdl.dept_head
                   + "', comments= '" + objMdl.comments + "', comment_head= '" + objMdl.comment_head + "'";

                if (objMdl.date_taining != null && objMdl.date_taining > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_taining='" + objMdl.date_taining.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_training='" + objMdl.id_training + "'";

                int id_training = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_training))
                {
                    if (Convert.ToInt32(objList.TrainList.Count) > 0)
                    {
                        objList.TrainList[0].id_training = objMdl.id_training;
                        FunAddTrainingDept_headTans(objList);
                    }
                    else
                    {
                        FunUpdateTrainingStaffTrans(objMdl.id_training);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingStaff: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteTrainingStaff(string sid_training)
        {
            try
            {
                string sSqlstmt = "update t_training_staff set Active=0 where id_training='" + sid_training + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTrainingStaff: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateStaffTrainingReport(TrainingStaffModels objMdl)
        {
            try
            {
                string sSqlstmt = "update t_training_staff set conducted_by ='" + objMdl.conducted_by + "', details = '" + objMdl.details
                   + "', upload= '" + objMdl.upload + "', effective= '" + objMdl.effective + "', reason= '" + objMdl.reason + "', recommendation= '" + objMdl.recommendation 
                   + "', traing_required= '" + objMdl.traing_required + "', notify_to= '" + objMdl.notify_to + "', training_status= '" + objMdl.training_status + "'";

                if (objMdl.start_date != null && objMdl.start_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", start_date='" + objMdl.start_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objMdl.end_date != null && objMdl.end_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", end_date='" + objMdl.end_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_training='" + objMdl.id_training + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateStaffTrainingReport: " + ex.ToString());
            }
            return false;
        }

    }
    public class TrainingStaffModelsList
    {
        public List<TrainingStaffModels> TrainList { get; set; }
    }
}