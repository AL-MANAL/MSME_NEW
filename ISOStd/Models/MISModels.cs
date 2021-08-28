using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
namespace ISOStd.Models
{

    public class MISModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //Objective MIS RDLC
        public string DeptName { get; set; }
        public int Achieved { get; set; }
        public int NA { get; set; }
        public int Pending { get; set; }
        public int Noeval { get; set; }
        public int Total { get; set; }
        public int Uptrend { get; set; }
        public int Downtrend { get; set; }

        //RISK MIS RDLC
        public string currentstatus { get; set; }
        public int stat_no { get; set; }
        public string Risk_status { get; set; }

        //AUDIT RDLC
        public string AuditType { get; set; }
        public int plan_nos { get; set; }
        public int condt_nos { get; set; }
        public int pend_nos { get; set; }
        public int totalncrs { get; set; }
        public int closedncrs { get; set; }
        public int openncrs { get; set; }

        //TRAINING RDLC
        public string report_desc { get; set; }
        public int planned_nos { get; set; }
        public int conducted_Nos { get; set; }
        public int Pending_training { get; set; }
        public int rescheduled { get; set; }
        public int Cancelled { get; set; }


        //SUPPLIER PERFORMANCE RDLC
        public string SupplierName { get; set; }
        public int NoOf_PO_Raised { get; set; }
        public int NoOf_Defective_Delivery { get; set; }
        public int No_Times_Delayed_Delivery { get; set; }
        public decimal comp_perc { get; set; }

        //PERFORMANCE EVALUATION
        public string brname { get; set; }
        public int noof_employee { get; set; }
        public int perf_done { get; set; }
        public int perf_notdone { get; set; }
        public int compet_done { get; set; }
        public int compet_notdone { get; set; }

        //CUSTOMER COMPLAINT
        public string Report_name { get; set; }
        public int noof_complaints { get; set; }
        public int pending { get; set; }
        public int closed { get; set; }
        public int open { get; set; }

        //CALIBRATION
        public string company { get; set; }
        public int noof_equip { get; set; }
        public int cal_before { get; set; }
        public int cal_after { get; set; }

        //DASHBOARD OBJECTIVE 
        public string Obj_Ref { get; set; }
        public string Freq_of_Eval { get; set; }
        public string Objectives_val { get; set; }
        public string jan { get; set; }
        public string feb { get; set; }
        public string mar { get; set; }
        public string apr { get; set; }
        public string may { get; set; }
        public string jun { get; set; }
        public string jul { get; set; }
        public string aug { get; set; }
        public string sep { get; set; }
        public string oct { get; set; }
        public string nov { get; set; }
        public string dece { get; set; }


        //DASHBOARD KPI 
        public string KPI_Ref { get; set; }
        public string Process_Indicator { get; set; }


        //DASHBOARD Training
        public string Training_Topic { get; set; }

        //DASHBOARD Meeting
        public string meeting_ref { get; set; }
        public string meeting_type_desc { get; set; }


        //DASHBOARD Audit
        public string AuditNum { get; set; }
        public string NCRDesc { get; set; }

        //DASHBOARD Calibration
        public string equipmemt_name { get; set; }
        public string Freq_of_calibration { get; set; }

        //DASHBOARD Risk
        public string ref_no { get; set; }
        public string risk_desc { get; set; }
        public string Frequency_Review { get; set; }

        internal DataSet FunGetDashboardReport(string sProcName, string reportType, string year)
        {

            // return objGlobalData.Getdetails("select * from enquiry_kpi_temp");
            return FunGenerateDashboardReport(sProcName, reportType, year);
        }

        internal DataSet FunGetMISReport(string sProcName, string reportType, string year)
        {
            // return objGlobalData.Getdetails("select * from enquiry_kpi_temp");
            return FunGenerateMISReport(sProcName, reportType, year);
        }
        internal DataSet FunGenerateDashboardReport(string sProcName, string sReportType, string sYear)
        {
            DataSet dsMIS = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                cmd.Parameters.AddWithValue("@ReportType", sReportType);
                cmd.Parameters.AddWithValue("@years", sYear);

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsMIS);
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

            return dsMIS;
        }

        internal DataSet FunGenerateMISReport(string sProcName, string sReportType, string sYear)
        {
            DataSet dsMIS = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                cmd.Parameters.AddWithValue("@ReportType", sReportType);
                cmd.Parameters.AddWithValue("@years", sYear);

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsMIS);
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

            return dsMIS;
        }

    }

    public class MISModelsList
    {
        public List<MISModels> objList { get; set; }
    }
}