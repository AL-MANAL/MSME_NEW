using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;

namespace ISOStd.Models
{
    public class TRAModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_tra { get; set; }

        [Display(Name = "Id")]
        public string id_task { get; set; }

        [Display(Name = "Id")]
        public string id_jobperformer { get; set; }

        [Display(Name = "TRA NO")]
        public string tra_no { get; set; }

        [Display(Name = "Date")]
        public DateTime tra_date { get; set; }

        [Display(Name = "Ref")]
        public string tra_ref { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Description of Work")]
        public string desc_work { get; set; }

        [Display(Name = "Document Title")]
        public string document_title { get; set; }

        [Display(Name = "Duration")]
        public string duration { get; set; }

        [Display(Name = "Task Performer")]
        public string task_performer { get; set; }

        [Display(Name = "Name")]
        public string pers_name { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Activities")]
        public string activity { get; set; }

        [Display(Name = "Hazards")]
        public string hazards { get; set; }

        [Display(Name = "Causes & Threats")]
        public string causes { get; set; }

        [Display(Name = "Consequences")]
        public string consequences { get; set; }

        [Display(Name = "Existing control Measures")]
        public string existing_measure { get; set; }

        [Display(Name = "Severity")]
        public string severity { get; set; }

        [Display(Name = "Probability")]
        public string probability { get; set; }

        [Display(Name = "Risk Rating")]
        public string risk_rating { get; set; }

        [Display(Name = "Additional risk reduction measures (If evaluated risk is not acceptable)")]
        public string additional_measures { get; set; }

        [Display(Name = "Severity")]
        public string add_severity { get; set; }

        [Display(Name = "Probability")]
        public string add_probability { get; set; }

        [Display(Name = "Risk Rating")]
        public string add_risk_rating { get; set; }

        [Display(Name = "Rev No")]
        public string revno { get; set; }

        [Display(Name = "Status")]
        public string tra_status { get; set; }

        [Display(Name = "Date")]
        public DateTime close_date { get; set; }

        [Display(Name = "Closed By")]
        public string close_by { get; set; }

        [Display(Name = "Out Come")]
        public string out_come { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string department { get; set; }

        internal bool FunAddTRA(TRAModels objModel, TRAModelsList objTRAPersList, TRAModelsList objTRATaskList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "insert into t_tra(revno,tra_ref,location,desc_work,document_title,duration,task_performer,branch,department";
                string sFields = "", sFieldValue = "";

                if (objModel.tra_date != null && objModel.tra_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", tra_date";
                    sFieldValue = sFieldValue + ", '" + objModel.tra_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objModel.revno + "','" + objModel.tra_ref + "','" + objModel.location + "','" + objModel.desc_work + "','" + objModel.document_title+
                    "','" + objModel.duration + "','" + objModel.task_performer + "','" + objModel.branch + "','" + objModel.department + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int id_tra = 0;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_tra))
                {
                    if (id_tra > 0 && Convert.ToInt32(objTRAPersList.TRAList.Count) > 0)
                    {
                        objTRAPersList.TRAList[0].id_tra = id_tra.ToString();
                        FunAddTRAPersList(objTRAPersList);
                    }
                    if (id_tra > 0 && Convert.ToInt32(objTRATaskList.TRAList.Count) > 0)
                    {
                        objTRATaskList.TRAList[0].id_tra = id_tra.ToString();
                        FunAddTRATaskList(objTRATaskList);
                    }
                    DataSet dsData = objGlobalData.GetReportNo("TRA", "",objGlobalData.GetCompanyBranchNameById(sBranch));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        tra_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_tra set tra_no='" + tra_no + "' where id_tra='" + id_tra + "'";
                    return (objGlobalData.ExecuteQuery(sql1));
                }
               
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTRA: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddTRAPersList(TRAModelsList objTRAPersList)
        {
            try
            {
                string sSqlstmt = "delete from t_tra_jobperformer where id_tra='" + objTRAPersList.TRAList[0].id_tra + "'; ";


                for (int i = 0; i < objTRAPersList.TRAList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_tra_jobperformer(id_tra,pers_name,designation";

                    String sFieldValue = "", sFields = "";

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objTRAPersList.TRAList[0].id_tra + "', '" + objTRAPersList.TRAList[i].pers_name + "', '" + objTRAPersList.TRAList[i].designation + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTRAPersList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddTRATaskList(TRAModelsList objTRATaskList)
        {
            try
            {
                string sSqlstmt = "delete from t_tra_task where id_tra='" + objTRATaskList.TRAList[0].id_tra + "'; ";


                for (int i = 0; i < objTRATaskList.TRAList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_tra_task(id_tra,activity,hazards,causes,consequences,existing_measure,severity,probability,risk_rating,additional_measures,add_severity,add_probability,add_risk_rating,tra_status,close_by,out_come";

                    string sFieldValue = "", sFields = "";
                    if (objTRATaskList.TRAList[i].close_date != null && objTRATaskList.TRAList[i].close_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", close_date";
                        sFieldValue = sFieldValue + ", '" + objTRATaskList.TRAList[i].close_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objTRATaskList.TRAList[0].id_tra + "', '" + objTRATaskList.TRAList[i].activity + "', '" + objTRATaskList.TRAList[i].hazards + "', '" + objTRATaskList.TRAList[i].causes + "', '" + objTRATaskList.TRAList[i].consequences + "', '" + objTRATaskList.TRAList[i].existing_measure + "', '" + objTRATaskList.TRAList[i].severity + "', '" + objTRATaskList.TRAList[i].probability + "', '" + objTRATaskList.TRAList[i].risk_rating + "', '" + objTRATaskList.TRAList[i].additional_measures + "', '" + objTRATaskList.TRAList[i].add_severity + "', '" + objTRATaskList.TRAList[i].add_probability + "', '" + objTRATaskList.TRAList[i].add_risk_rating + "'"
                        + ", '" + objTRATaskList.TRAList[i].tra_status + "', '" + objTRATaskList.TRAList[i].close_by + "', '" + objTRATaskList.TRAList[i].out_come + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTRATaskList: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateTRA(TRAModels objModel, TRAModelsList objTRAPersList, TRAModelsList objTRATaskList)
        {
            try
            {
                string sSqlstmt = "update t_tra set revno='" + objModel.revno + "',tra_ref='" + objModel.tra_ref + "',location='" + objModel.location + "',desc_work='" + objModel.desc_work + "'"
                    + ",document_title='" + objModel.document_title + "',duration='" + objModel.duration + "',task_performer='" + objModel.task_performer + "',branch='" + objModel.branch + "',department='" + objModel.department + "'";

                if (objModel.tra_date != null && objModel.tra_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",tra_date='" + objModel.tra_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_tra='" + objModel.id_tra + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(objTRAPersList.TRAList.Count) > 0)
                    {
                        objTRAPersList.TRAList[0].id_tra = id_tra.ToString();
                        FunAddTRAPersList(objTRAPersList);
                    }
                    else
                    {
                        FunUpdatePers(id_tra);

                    }
                    if (Convert.ToInt32(objTRATaskList.TRAList.Count) > 0)
                    {
                        objTRATaskList.TRAList[0].id_tra = id_tra.ToString();
                        FunAddTRATaskList(objTRATaskList);
                    }
                    else
                    {
                        FunUpdateTRATask(id_tra);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateProject: " + ex.ToString());
            }

            return false;
        }
        internal bool FunUpdatePers(string id_tra)
        {
            try
            {
                string sSqlstmt = "delete from t_tra_jobperformer where id_tra='" + id_tra + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePers: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateTRATask(string id_tra)
        {
            try
            {
                string sSqlstmt = "delete from t_tra_task where id_tra='" + id_tra + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateTRATask: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteTRADoc(string sid_tra)
        {
            try
            {
                string sSqlstmt = "update t_tra set Active=0 where id_tra='" + sid_tra + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteTRADoc: " + ex.ToString());
            }
            return false;
        }
    }
    public class TRAModelsList
    {
        public List<TRAModels> TRAList { get; set; }
    }
}