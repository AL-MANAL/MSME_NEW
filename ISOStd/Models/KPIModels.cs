using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;
using MySql.Data.MySqlClient;
using ExpressiveAnnotations.Attributes;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;


namespace ISOStd.Models
{
    public class KPIModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        public int EvalId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Dept { get; set; }

        [Required]
        [Display(Name = "Department Head")]
        public string Dept_Head { get; set; }

        [Required]
        [Display(Name = "KPI Reference")]
        public string KPI_Ref { get; set; }

        [Required]
        [Display(Name = "Frequency of Evaluation")]
        public string Freq_of_Eval { get; set; }

        [Required]
        [Display(Name = "Process In Charge")]
        public string SubProcess_InCharge { get; set; }

        [Required]
        [Display(Name = "KPI Established On")]
        public DateTime KPI_Estd_On { get; set; }

        [Required]
        [Display(Name = "KPI No.")]
        public string KPI_No { get; set; }

        [Required]
        [Display(Name = "Key Process Indicator")]
        public string Key_Perf_Indicator { get; set; }

        [Required]
        [Display(Name = "Measurable Value")]
        public string Measurable_Value { get; set; }

        [Required]
        [Display(Name = "KPI Monitoring Mechanism")]
        public string KPI_Monitoring_Mechanism { get; set; }

        [Required]
        [Display(Name = "KPI Status evaluated on")]
        public DateTime KPI_Status_eval_on { get; set; }

        [Required]
        [Display(Name = "KPI Evaluation Record")]
        public string KPI_Eval_Record { get; set; }

        [Required]
        [Display(Name = "NCR Ref. (If KPI not achieved)")]
        public string NCR_Ref { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DeptName { get; set; }

        [Required]
        [Display(Name = "Person Responsible")]
        public string Person_Responsible { get; set; }

        public string Format { get; set; }

        //RDLC
        public DateTime EstablishedOn { get; set; }
        public string Process_Indicator { get; set; }
        public string Monitoring_Mechanism { get; set; }
        public string Measured_Value { get; set; }
        public string Eval_Status { get; set; }
        public DateTime Eval_On { get; set; }
        public string NCRRef { get; set; }
        internal bool FunAddKPI(KPIModels objKPIModels)
        {
            try
            {
                string sKPI_Estd_OnDate = objKPIModels.KPI_Estd_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sKPI_Status_eval_onDate = objKPIModels.KPI_Status_eval_on.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_kpievaluation (Dept, Dept_Head, KPI_Ref, Freq_of_Eval, SubProcess_InCharge, KPI_Estd_On, KPI_No, Key_Perf_Indicator, "
                + "Measurable_Value, KPI_Monitoring_Mechanism, KPI_Status_eval_on, KPI_Eval_Record, NCR_Ref )"
                + " values('" + objKPIModels.Dept + "','" + objKPIModels.Dept_Head + "','" + objKPIModels.KPI_Ref + "','" + objKPIModels.Freq_of_Eval
                + "','" + objKPIModels.SubProcess_InCharge + "','" + sKPI_Estd_OnDate + "','" + objKPIModels.KPI_No + "','" + objKPIModels.Key_Perf_Indicator
                + "','" + objKPIModels.Measurable_Value + "','" + objKPIModels.KPI_Monitoring_Mechanism + "','" + sKPI_Status_eval_onDate + "','"
                + objKPIModels.KPI_Eval_Record + "','" + objKPIModels.NCR_Ref + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPI(KPIModels objKPIModels)
        {
            try
            {
                string sKPI_Estd_OnDate = objKPIModels.KPI_Estd_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sKPI_Status_eval_onDate = objKPIModels.KPI_Status_eval_on.ToString("yyyy-MM-dd HH':'mm':'ss");
                //, , , , , , 
                string sSqlstmt = "update t_kpievaluation set Dept ='" + objKPIModels.Dept + "', Dept_Head='" + objKPIModels.Dept_Head + "', "
                    + "KPI_Ref='" + objKPIModels.KPI_Ref + "', Freq_of_Eval='" + objKPIModels.Freq_of_Eval + "', SubProcess_InCharge='" + objKPIModels.SubProcess_InCharge
                    + "', KPI_Estd_On='" + sKPI_Estd_OnDate + "', KPI_No='" + objKPIModels.KPI_No + "', Key_Perf_Indicator='" + objKPIModels.Key_Perf_Indicator + "', Measurable_Value='" + objKPIModels.Measurable_Value
                   + "', KPI_Monitoring_Mechanism='" + objKPIModels.KPI_Monitoring_Mechanism + "', KPI_Status_eval_on='" + sKPI_Status_eval_onDate
                   + "', KPI_Eval_Record='" + objKPIModels.KPI_Eval_Record + "', NCR_Ref='" + objKPIModels.NCR_Ref + "' where EvalId=" + objKPIModels.EvalId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPI: " + ex.ToString());
            }
            return false;
        }
        internal DataSet FunGetKPIReport(string sProcName, string dtFromDate, string dtToDate)
        {
            // return objGlobalData.Getdetails("select * from enquiry_kpi_temp");
            return FunGenerateKPIReport(sProcName, dtFromDate,  dtToDate);
        }
        internal DataSet FunGenerateKPIReport(string sProcName, string dtFromDate, string dtToDate)
        {
            DataSet dsKPI = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@vFromperiod", dtFromDate);
                cmd.Parameters.AddWithValue("@vToperiod", dtToDate);

                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;
               
                objAdap.Fill(dsKPI);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGenerateDashboard: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return dsKPI;
        }
  

    }

    public class KPIModelsList
    {
        public List<KPIModels> KPIMList { get; set; }
    }   
}