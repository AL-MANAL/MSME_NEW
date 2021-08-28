using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class ManHourModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string id_man_hour { get; set; }

        [Display(Name = "Month")]
        public string t_month { get; set; }

        [Display(Name = "Project")]
        public string project { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Year")]
        public string t_year { get; set; }

        [Display(Name = "Numbers of Employees")]
        public int man_power { get; set; }

        [Display(Name = "Total hrs worked (Regular) Hrs/Days")]
        public int man_hour { get; set; }

        [Display(Name = "Total hrs worked (OT) Hrs/Days")]
        public int man_hour_ot { get; set; }

        [Display(Name = "Restricted /Modified Days")]
        public int restrict_days { get; set; }

        [Display(Name = "Fuel Consumption")]
        public int fuel_consump { get; set; }

        [Display(Name = "Water Consumption")]
        public int water_consump { get; set; }

        [Display(Name = "Paper Consumption")]
        public int paper_consump { get; set; }

        [Display(Name = "Electricity Consumption")]
        public int electricity_consump { get; set; }

        [Display(Name = "Qty of solid based")]
        public int qty_solid { get; set; }

        [Display(Name = "Prepared By")]
        public string preparedby { get; set; }

        [Display(Name = "Reviewed By")]
        public string reviewedby { get; set; }

        [Display(Name = "Unit of Measurement for Paper")]
        public string paperunit { get; set; }

        [Display(Name = "Unit of Measurement Fuel")]
        public string fuelunit { get; set; }

        [Display(Name = "Unit of Measurement Water")]
        public string waterunit { get; set; }

        [Display(Name = "Unit of Measurement Solid")]
        public string solidunit { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        internal bool FunAddManHour(ManHourModels objManHr)
        {
            try
            {
                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string empid = objGlobalData.GetCurrentUserSession().empid;

                {
                    sSqlstmt = "insert into t_man_hour(project, location, t_month, t_year, man_power, man_hour,branch,man_hour_ot,restrict_days," +
                        "fuel_consump,water_consump,paper_consump,electricity_consump,qty_solid,preparedby,reviewedby,fuelunit,waterunit,paperunit,solidunit,comments)"
                                 + " values('" + objManHr.project + "', '" + objManHr.location + "','" + objManHr.t_month + "','" + objManHr.t_year
                                 + "','" + objManHr.man_power + "','" + objManHr.man_hour + "','" + sBranch + "', '" + objManHr.man_hour_ot + "','" + objManHr.restrict_days + "','" + objManHr.fuel_consump
                                 + "','" + objManHr.water_consump + "','" + objManHr.paper_consump + "','" + objManHr.electricity_consump + "','" + objManHr.qty_solid + "','" + empid + "','" + objManHr.reviewedby
                                 + "','" + objManHr.fuelunit + "','" + objManHr.waterunit + "','" + objManHr.paperunit + "','" + objManHr.solidunit + "','" + objManHr.comments + "')";
                }

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddManHour: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateManHour(ManHourModels objManHr)
        {
            try
            {
                string sSqlstmt = " update t_man_hour set t_month='" + objManHr.t_month + "',t_year='" + objManHr.t_year + "',project='" + objManHr.project
                    + "', location='" + objManHr.location + "', man_power='" + objManHr.man_power + "',man_hour='" + objManHr.man_hour
                    + "', man_hour_ot='" + objManHr.man_hour_ot + "', restrict_days='" + objManHr.restrict_days + "',fuel_consump='" + objManHr.fuel_consump
                    + "', water_consump='" + objManHr.water_consump + "', paper_consump='" + objManHr.paper_consump + "',electricity_consump='" + objManHr.electricity_consump
                    + "', qty_solid='" + objManHr.qty_solid + "',preparedby='" + objManHr.preparedby + "',reviewedby='" + objManHr.reviewedby + "', fuelunit='" + objManHr.fuelunit + "',waterunit='" + objManHr.waterunit
                    + "', paperunit='" + objManHr.paperunit + "',solidunit='" + objManHr.solidunit + "', comments='" + objManHr.comments + "'";

                //if (objSafety.observation_date > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + "', observation_date='" + objSafety.observation_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                //}

                sSqlstmt = sSqlstmt + " where id_man_hour=" + objManHr.id_man_hour;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateManHour: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteManHour(string sid_man_hour)
        {
            try
            {
                string sSqlstmt = "update t_man_hour set active=0 where id_man_hour='" + sid_man_hour + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteManHour: " + ex.ToString());
            }
            return false;
        }

    }

    public class ManHourModelsList
    {
        public List<ManHourModels> ManHrList { get; set; }
    }

}