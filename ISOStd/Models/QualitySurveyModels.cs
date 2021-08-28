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
    public class QualitySurveyModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public int id_qualitysurvey { get; set; }

        [Display(Name = "Survey Date")]
        public DateTime Survey_date { get; set; }

        [Display(Name = "Survey For")]
        public string Survey_for { get; set; }

        [Display(Name = "Service Provider")]
        public string ThirdParty { get; set; }

        [Display(Name = "Conducted By")]
        public string ConductedBy { get; set; }

        [Display(Name = "Acceptance Criteria")]
        public string Criteria { get; set; }

        [Display(Name = "Lab Report")]
        public string Report { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime ExpDate { get; set; }

        [Display(Name = "Review Date")]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Is it within acceptance criteria")]
        public string IsLimit { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "VerifiedBy")]
        public string VerifiedBy { get; set; }

        public string LoggedBy { get; set; }
        public DateTime LoggedDate { get; set; }

        internal bool FunDeleteQualitySurveyDoc(string sid_qualitysurvey)
        {
            try
            {
                string sSqlstmt = "update t_qualitysurvey set Active=0 where id_qualitysurvey='" + sid_qualitysurvey + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQualitySurveyDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddQualitySurvey(QualitySurveyModels objQuality)
        {
            try
            {
                string sSqlstmt = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                sSqlstmt = "insert into t_qualitysurvey (Survey_for,ThirdParty,ConductedBy,Criteria,Report,Location,IsLimit,Remarks,LoggedBy,branch,VerifiedBy";
                string sFields = "", sFieldValue = "";

                if (objQuality.Survey_date != null && objQuality.Survey_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Survey_date";
                    sFieldValue = sFieldValue + ", '" + objQuality.Survey_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objQuality.ExpDate != null && objQuality.ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", ExpDate";
                    sFieldValue = sFieldValue + ", '" + objQuality.ExpDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objQuality.ReviewDate != null && objQuality.ReviewDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", ReviewDate";
                    sFieldValue = sFieldValue + ", '" + objQuality.ReviewDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objQuality.Survey_for + "','" + objQuality.ThirdParty + "','" + objQuality.ConductedBy + "'"
                + ",'" + objQuality.Criteria + "','" + objQuality.Report + "','" + objQuality.Location + "','" + objQuality.IsLimit + "'"
                +",'" + objQuality.Remarks + "','"+user + "','" + sBranch + "','" + objQuality.VerifiedBy + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    return true;
                }
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddQualitySurvey: " + ex.ToString());

            }
            return false;
        }

        internal bool FunUpdateQualitySurvey(QualitySurveyModels objQuality)
        {
            try
            {
                string sSqlstmt = "update t_qualitysurvey set Survey_for ='" + objQuality.Survey_for + "', ThirdParty='" + objQuality.ThirdParty + "', "
                    + "ConductedBy='" + objQuality.ConductedBy + "',Criteria='" + objQuality.Criteria + "',Location='" + objQuality.Location + "',IsLimit='" + objQuality.IsLimit + "'"
                    + ",Remarks='" + objQuality.Remarks + "',Report='" + objQuality.Report + "',VerifiedBy='" + objQuality.VerifiedBy + "'";

                if (objQuality.Survey_date != null && objQuality.Survey_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Survey_date='" + objQuality.Survey_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objQuality.ExpDate != null && objQuality.ExpDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", ExpDate='" + objQuality.ExpDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objQuality.ReviewDate != null && objQuality.ReviewDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", ReviewDate='" + objQuality.ReviewDate.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_qualitysurvey='" + objQuality.id_qualitysurvey + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateQualitySurvey: " + ex.ToString());
            }

            return false;
        }
    }

    public class QualitySurveyModelsList
    {
        public List<QualitySurveyModels> lstQuality { get; set; }
    }
}