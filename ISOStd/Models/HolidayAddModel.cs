using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;
namespace ISOStd.Models
{
    public class HolidayAddModel
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string Id_holiday { get; set; }

        [Display(Name = "Holiday Name")]
        public string Name { get; set; }

      
        

        [Display(Name = "For Date")]
        public DateTime Frdate { get; set; }


        [Display(Name = "To Date")]
        public DateTime Todate { get; set; }


        internal bool FunAddHoliday(HolidayAddModel obj)
        {
            try
            {
                string sSqlstmt = "insert into t_holiday (Name";
                string sFields = "", sFieldValue = "";
                if (obj.Frdate != null && obj.Frdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Frdate";
                    sFieldValue = sFieldValue + ", '" + obj.Frdate.ToString("yyyy/MM/dd") + "'";
                }
                if (obj.Todate != null && obj.Todate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Todate";
                    sFieldValue = sFieldValue + ", '" + obj.Todate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")values('" + obj.Name + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCertificate: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateHolidady(HolidayAddModel obj)
        {
            try
            {
                string sSqlstmt = "update t_holiday set Name='" + obj.Name + "'";
               
                string sFields = "";
                if (obj.Frdate != null && obj.Frdate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Frdate='";
                    sFields = sFields + obj.Frdate.ToString("yyyy/MM/dd") + "'";
                }
                if (obj.Todate != null && obj.Todate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Todate='";
                    sFields = sFields + obj.Todate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields + " where Id_holiday='" + obj.Id_holiday + "'";


                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunEditHoliday: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCertificate(string Id_holiday)
        {
            try
            {
                string sSqlstmt = "delete from t_holiday where Id_holiday='" + Id_holiday + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCertificate: " + ex.ToString());
            }
            return false;
        }

    }

    public class HolidayAddModelsList
    {
        public List<HolidayAddModel> HolidayList { get; set; }
    }
}
