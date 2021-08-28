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
    public class SafetyObservationModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "ID")]
        public string id_safety_observation { get; set; }

        [Display(Name = "Date and Timing of Safety Observation")]
        public DateTime observation_date { get; set; }

        [Display(Name = "Project")]
        public string project { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Type")]
        public string type_observation { get; set; }

        [Display(Name = "Observation")]
        public string desc_observation { get; set; }

        [Display(Name = "Action Agreed")]
        public string action_taken { get; set; }

        [Display(Name = "Observed By")]
        public string observed_by { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Report No")]
        public string report_no { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Comments")]
        public string comments { get; set; }

        [Display(Name = "Document")]
        public string upload { get; set; }

        [Display(Name = "Department")]
        public string dept { get; set; }

        [Display(Name = "Responsible Person")]
        public string resp_person { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        internal bool FunAddObservation(SafetyObservationModels objSafety)
        {
            try
            {
                string sobservation_date = objSafety.observation_date.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sSqlstmt = "";
                string sFields = "", sFieldValue = "";

                {
                    sSqlstmt = sSqlstmt + "insert into t_safety_observation(observation_date, location, type_observation, desc_observation, action_taken, observed_by, reported_by, project,status,comments,upload,branch,dept,resp_person";

                    if (objSafety.target_date != null && objSafety.target_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", target_date";
                        sFieldValue = sFieldValue + ", '" + objSafety.target_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;

                    sSqlstmt = sSqlstmt + ") values('" + sobservation_date + "', '" + objSafety.location + "','" + objSafety.type_observation + "','" + objSafety.desc_observation
                                 + "','" + objSafety.action_taken + "','" + objSafety.observed_by + "','" + objSafety.reported_by + "','" + objSafety.project
                                 + "','" + objSafety.status + "','" + objSafety.comments + "','" + objSafety.upload + "','" + sBranch + "','" + objSafety.dept + "','" + objSafety.resp_person + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                }              

               
                //if (objGlobalData.ExecuteQuery(sSqlstmt))
                int id_safety_observation = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_safety_observation))
                {
                    if(id_safety_observation > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("SOC", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            report_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_safety_observation set report_no='" + report_no + "'where id_safety_observation = '" + id_safety_observation + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddObservation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateObservation(SafetyObservationModels objSafety)
        {
            try
            {
                string sSqlstmt = " update t_safety_observation set type_observation='" + objSafety.type_observation + "',desc_observation='" + objSafety.desc_observation + "',action_taken='" + objSafety.action_taken
                    + "', observed_by='" + objSafety.observed_by + "', reported_by='" + objSafety.reported_by /*+ "',project='" + objSafety.project */+ "',location='" + objSafety.location + "',report_no='" + objSafety.report_no
                    + "',status='" + objSafety.status + "',comments='" + objSafety.comments + "',upload='" + objSafety.upload + "',dept='" + objSafety.dept + "',resp_person='" + objSafety.resp_person;

                if (objSafety.observation_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + "', observation_date='" + objSafety.observation_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objSafety.target_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", target_date='" + objSafety.target_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                sSqlstmt = sSqlstmt + " where id_safety_observation=" + objSafety.id_safety_observation;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObservation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteObservation(string sid_safety_observation)
        {
            try
            {
                string sSqlstmt = "update t_safety_observation set active=0 where id_safety_observation='" + sid_safety_observation + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteObservation: " + ex.ToString());
            }
            return false;
        }

    }

    public class SafetyObservationModelsList
    {
        public List<SafetyObservationModels> SafetyList { get; set; }
    }

}