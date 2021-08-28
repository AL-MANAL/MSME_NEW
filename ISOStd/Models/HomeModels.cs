using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Data;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
namespace ISOStd.Models
{
    public class HomeModels
    {
        [Display(Name = "Items")]
        public string items { get; set; }

        [Display(Name = "Pending For Approval")]
        public string pending_approval { get; set; }

        [Display(Name = "Pending For Review")]
        public string pending_review { get; set; }

        [Display(Name = "Approved")]
        public string approved { get; set; }

        [Display(Name = "Rejected")]
        public string rejected { get; set; }

        [Display(Name = "Not Planned")]
        public string Not_planned { get; set; }

        [Display(Name = "Not Completed")]
        public string Not_completed { get; set; }

        [Display(Name = "Completed")]
        public string Completed { get; set; }

        [Display(Name = "Rescheduled")]
        public string Rescheduled { get; set; }

        [Display(Name = "Cancelled")]
        public string Cancelled { get; set; }

        [Display(Name = "Open")]
        public string Open { get; set; }

        [Display(Name = "Closed")]
        public string Closed { get; set; }

        [Display(Name = "Pending")]
        public string Pending { get; set; }

        [Display(Name = "Total")]
        public string total { get; set; }

        clsGlobal objGlobalData = new clsGlobal();
        internal DataSet FunGetDashboardReport(string sProcName)
        {
            // return objGlobalData.Getdetails("select * from enquiry_kpi_temp");
            return FunGenerateDashboardReport(sProcName);
        }

        internal DataSet FunGenerateDashboardReport(string sProcName)
        {
            DataSet dsHSE = new DataSet();
            MySqlConnection con = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["IsoSoftDBContext"].ConnectionString);
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sProcName, con);
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlDataAdapter objAdap = new MySqlDataAdapter();

                objAdap.SelectCommand = cmd;

                objAdap.Fill(dsHSE);
                con.Close();
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGenerateDashboardReport: " + ex.ToString());
            }
            finally
            {
                con.Close();
            }

            return dsHSE;
        }

        public MultiSelectList GetTrainingStatusList()
        {
            DropdownList objReportList = new DropdownList();
            objReportList.lstDropdown = new List<DropdownItems>();
            try
            {
                string sSsqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader " +
                    "where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Calendar Event Status' and dropdownitems.item_desc='Open' order by item_desc asc";

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
                objGlobalData.AddFunctionalLog("Exception in GetTrainingStatusList: " + ex.ToString());
            }
            return new MultiSelectList(objReportList.lstDropdown, "Id", "Name");
        }

    }

    public class HomeModelsList
    {
        public List<HomeModels> HomeModelList { get; set; }
    }
}