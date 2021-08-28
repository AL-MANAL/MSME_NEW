using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class NearMissModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        //1st table
        [Display(Name = "Id")]
        public int id_nearmiss { get; set; }

        [Display(Name = "Date and time")]
        public DateTime incident_date { get; set; }

        [Display(Name = "Reported on")]
        public DateTime reported_date { get; set; }

        [Display(Name = "Report Number")]
        public string report_no { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Position")]
        public string reported_by_position { get; set; }

        [Display(Name = "Department")]
        public string reported_by_dept { get; set; }

        [Display(Name = "Location/Area of incident")]
        public string location { get; set; }

        [Display(Name = "Description of near-miss incident")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Effect of incident")]
        [DataType(DataType.MultilineText)]
        public string effect_incident { get; set; }

        [Display(Name = "Reviewed by")]
        public string reviewed_by { get; set; }

        [Display(Name = "Probable immediate causes")]
        public string causes { get; set; }

        //2nd table
        [Display(Name = "Id")]
        public int id_nearmiss_action { get; set; }

        [Display(Name = "Immediate Correction Taken")]
        public string correction { get; set; }

        [Display(Name = "Corrective Action proposed")]
        public string action { get; set; }

        [Display(Name = "Need of Amending hazard risk register")]
        public string hazard { get; set; }

        [Display(Name = "Verified By")]
        public string verified_by { get; set; }
        
        [Display(Name = "Project")]
        public string project { get; set; }

        internal bool FunDeleteNearMiss(string sid_nearmiss)
        {
            try
            {
                string sSqlstmt = "update t_nearmiss set active=0 where id_nearmiss='" + sid_nearmiss + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNearMiss: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteNearMissAction(string sid_nearmiss_action)
        {
            try
            {
                string sSqlstmt = "update t_nearmiss_action set active=0 where id_nearmiss_action='" + sid_nearmiss_action + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteNearMiss: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddNearMissDetails(NearMissModels objNearMiss)
        {
            try
            {
                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                sSqlstmt = "insert into t_nearmiss (report_no,reported_by,reported_by_position,reported_by_dept,location,"
                + "description,effect_incident,reviewed_by,causes,project,branch";
                string sFields = "", sFieldValue = "";

                if (objNearMiss.incident_date != null && objNearMiss.incident_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", incident_date";
                    sFieldValue = sFieldValue + ", '" + objNearMiss.incident_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (objNearMiss.reported_date != null && objNearMiss.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", reported_date";
                    sFieldValue = sFieldValue + ", '" + objNearMiss.reported_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objNearMiss.upload != null && objNearMiss.upload != "")
                {
                    sFields = sFields + ", upload";
                    sFieldValue = sFieldValue + ", '" + objNearMiss.upload + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objNearMiss.report_no + "','" + objNearMiss.reported_by + "'"
                + ",'" + objNearMiss.reported_by_position + "'"
                + ",'" + objNearMiss.reported_by_dept + "','" + objNearMiss.location + "','" + objNearMiss.description + "','" + objNearMiss.effect_incident + "'"
                + ",'" + objNearMiss.reviewed_by + "','" + objNearMiss.causes + "','" + objNearMiss.project + "','" + sBranch + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNearMissDetails: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateNearMissDetails(NearMissModels objNearMiss)
        {
            try
            {
                string sSqlstmt = "update t_nearmiss set report_no ='" + objNearMiss.report_no + "', reported_by='" + objNearMiss.reported_by + "', "
                    + "reported_by_position='" + objNearMiss.reported_by_position + "',reported_by_dept='" + objNearMiss.reported_by_dept + "',location='" + objNearMiss.location + "',description='" + objNearMiss.description + "'"
                    + ",upload='" + objNearMiss.upload + "',effect_incident='" + objNearMiss.effect_incident + "',reviewed_by='" + objNearMiss.reviewed_by + "',causes='" + objNearMiss.causes + "',project='" + objNearMiss.project + "'";

                if (objNearMiss.incident_date != null && objNearMiss.incident_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", incident_date='" + objNearMiss.incident_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (objNearMiss.reported_date != null && objNearMiss.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", reported_date='" + objNearMiss.reported_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nearmiss='" + objNearMiss.id_nearmiss + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNearMissDetails: " + ex.ToString());
            }

            return false;
        }

        internal bool FunAddNearMissActionDetails(NearMissModels objNearMiss)
        {
            try
            {
                string sSqlstmt = "";
                sSqlstmt = "insert into t_nearmiss_action (id_nearmiss,correction,action,hazard,verified_by,reviewed_by)"
                + "values('" + objNearMiss.id_nearmiss + "','" + objNearMiss.correction + "','" + objNearMiss.action + "'"
                + ",'" + objNearMiss.hazard + "'"
                + ",'" + objNearMiss.verified_by + "','" + objNearMiss.reviewed_by + "')";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNearMissActionDetails: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateNearMissActionDetails(NearMissModels objNearMiss)
        {
            try
            {
                string sSqlstmt = "update t_nearmiss_action set correction ='" + objNearMiss.correction + "',"
                + "action ='" + objNearMiss.action + "',hazard ='" + objNearMiss.hazard + "',verified_by ='" + objNearMiss.verified_by + "'"
                + ",reviewed_by ='" + objNearMiss.reviewed_by + "'where id_nearmiss_action='" + objNearMiss.id_nearmiss_action + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNearMissActionDetails: " + ex.ToString());
            }

            return false;
        }
    }

    public class NearMissModelsList
    {
        public List<NearMissModels> lstNearMiss { get; set; }
    }
}