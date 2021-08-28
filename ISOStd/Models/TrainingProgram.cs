using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;

namespace ISOStd.Models
{
    public class TrainingProgram
    {
        clsGlobal objGlobalData = new clsGlobal();


        public string Id_Training_program { get; set; }

        [Display(Name = "Training Topic")]
        public string topic_id { get; set; }

   
         public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }

        public string June { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sept { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }
        [Display(Name = "Year")]
        public string year { get; set; }

        public string Status { get; set; }
        public DateTime Planned_Date { get; set; }
        public string Logged_id { get; set; }

        public string remark { get; set; }



        internal bool FunUpdateTrainingProgram(string selected, int status)
        {
            try
            {
                string[] Sselected = null;
                Sselected = selected.Split(',');
                if (status == 1)
                {
                    for (int j = 0; j <= Sselected.Length - 1; j = j + 2)
                    {
                        string sqlstmt = "update t_training_program set " + Sselected[j + 1] + "=1 where id_training_program='" + Sselected[j] + "'";
                        objGlobalData.ExecuteQuery(sqlstmt);
                    }
                    return true;
                }
                if (status == 0)
                {
                    for (int i = 0; i <= Sselected.Length - 1; i = i + 2)
                    {
                        string sqlstmt = "update t_training_program set " + Sselected[i + 1] + "=0 where id_training_program='" + Sselected[i] + "'";
                        objGlobalData.ExecuteQuery(sqlstmt);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunTrainingProgram: " + ex.ToString());
            }
            return false;
        }


        //internal bool FunUpdate2TrainingProgram(string selected)
        
        //    {
        //    string app ="";
        //    char app2;
        //    try
        //    {
                
        //        int i, j = 1;
        //        for (i = 0; i < selected.Length - 3; i = i + 2)
        //        {

        //            while (selected[i] == selected[i + 2])
        //            {
        //                app = app + selected[j] + ",";
        //                i = i + 2;
        //                j = j + 2;
        //            }
        //            app = app + selected[j];
        //            j = j + 2;
        //            app2 = selected[i];
        //            if (app != "undefined")
        //            {
        //                string sqlstmt = "update t_training_program set topic_id='" + app + "'"+" where id_training_program='" + app2  + "'";
        //                   objGlobalData.ExecuteQuery(sqlstmt);
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in Fun2TrainingProgram: " + ex.ToString());
        //    }
        //    return false;
        //}



        internal bool FunAddProgram(TrainingProgram objTrainingProgram)
        {
            try
            {
                //string sColumn = "", sValues = "";
                string user = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                user = objGlobalData.GetCurrentUserSession().empid;
                //if (objTrainingProgram.Planned_Date > Convert.ToDateTime("01/01/0001"))
                //{
                //    //sColumn = sColumn + ", Evaluated_From";
                //    sValues = sValues + ", '" + objTrainingProgram.Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                string sSqlstmt = "insert into t_training_program(Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sept,Oct,Nov,Dece,topic_id,calyear,planned_date,logged_id,remark,active,branch) values('"
                   + objTrainingProgram.Jan + "','" + objTrainingProgram.Feb + "','" + objTrainingProgram.Mar
                   + "','" + objTrainingProgram.Apr + "','" + objTrainingProgram.May +"','" + objTrainingProgram.June +
                    "','" + objTrainingProgram.Jul + "','" + objTrainingProgram.Aug + "','" + objTrainingProgram.Sept
                     + "','" + objTrainingProgram.Oct + "','" + objTrainingProgram.Nov + "','" + objTrainingProgram.Dec +"','" + objTrainingProgram.topic_id + "','" + objTrainingProgram.year + "','" + objTrainingProgram.Planned_Date.ToString("yyyy-MM-dd HH':'mm':'ss")
                    + "','"+ user + "','" + objTrainingProgram.remark + "','" + 1 + "','" + sBranch + "')";





                //+ objInternalAudit.AuditCriteria + "','" + objInternalAudit.AuditLocation + "','" + objInternalAudit.upload + "')";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingProgram: " + ex.ToString());
            }

            return false;
        }

        internal bool FunDeleteTrainingProgram(string sid_training_program)
        {
            try
            {
                string sSqlstmt = "update t_training_program set active=0 where id_training_program='" + sid_training_program + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePartiesDoc: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateTrainingProgram(TrainingProgram objPlan)
        {
            try
            {
                string sSqlstmt = "update t_training_program set topic_id ='" + objPlan.topic_id + "', remark='" + objPlan.remark + "'";



                sSqlstmt = sSqlstmt + " where Id_Training_program='" + objPlan.Id_Training_program + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTrainingProgram: " + ex.ToString());

            }
            return false;
        }

    }


    









    public class TrainingProgramModelsList
    {
        public List<TrainingProgram> TrainingProgramList { get; set; }
    }
}