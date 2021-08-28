using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class LocationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display (Name = "Id")]
        public string id_country { get; set; }

        [Display(Name = "Country Name")]
        public string country_name { get; set; }

        [Display(Name = "Id")]
        public string id_location { get; set; }

        [Display(Name = "Location Name")]
        public string location_name { get; set; }

        [Display(Name ="Area Type")]
        public string area_type { get; set; }

        internal bool FunAddLocations(LocationModels objLocationModels)
        {
            try
            {
                string sSqlstmt = "insert into t_location ( id_country, location_name,area_type) values('"
                    + objLocationModels.id_country + "','" + objLocationModels.location_name + "','" + objLocationModels.area_type + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddLocations: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateLocations(LocationModels objLocationModels)
        {
            try
            {
                string sSqlstmt = "update t_location set location_name='" + objLocationModels.location_name + "' where id_location='" + objLocationModels.id_location + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateLocations: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteLocations(string id_location)
        {
            string sSqlstmt = "update t_location set active = 0 where id_location='" + id_location + "'";
            return objGlobalData.ExecuteQuery(sSqlstmt);
        }
    }

    public class LocationModelsList
    {
        public List<LocationModels> LocList { get; set; }
    }

}