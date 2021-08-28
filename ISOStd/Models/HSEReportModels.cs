using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;

namespace ISOStd.Models
{
    public class HSEReportModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //HSE PERFORMANCE
        public string months { get; set; }
        public string ind_type { get; set; }
        public string ind_desc { get; set; }
        public int mth_total { get; set; }
        public int ytd { get; set; }

        internal DataSet FunGetDashboardHSEReport(string sProcName, string reportType, string year, string month)
        {
            // return objGlobalData.Getdetails("select * from enquiry_kpi_temp");
            return FunGenerateDashboardHSEReport(sProcName, reportType, year,month);
        }

        internal DataSet FunGenerateDashboardHSEReport(string sProcName, string sReportType, string sYear, string sMonth)
        {
            DataSet dsHSE = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                cmd.Parameters.AddWithValue("@ReportType", sReportType);
                cmd.Parameters.AddWithValue("@years", sYear);
                cmd.Parameters.AddWithValue("@months", sMonth);
              
                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsHSE);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGenerateDashboardHSEReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return dsHSE;
        }
    
    }

    public class HSEReportModelsList
    {
        public List<HSEReportModels> objList { get; set; }
    }
}