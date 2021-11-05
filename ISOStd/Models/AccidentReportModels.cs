using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class AccidentReportModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_accident_rept { get; set; }

        [Display(Name = "Date and timing of Incident")]
        public DateTime acc_date { get; set; }

        [Display(Name = "Date of Incident Report")]
        public DateTime reported_date { get; set; }

        [Display(Name = "Report No")]
        public string report_no { get; set; }

        [Display(Name = "Reported By")]
        public string reported_by { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "Details of Incident")]
        public string details { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Consequences")]
        public string damage { get; set; }

        [Display(Name = "Need of thorough investigation?")]
        public string invest_need { get; set; }

        [Display(Name = "Justify for no investigation")]
        public string justify { get; set; }

        [Display(Name = "Incident investigation report number")]
        public string invest_reportno { get; set; }

        public string logged_by { get; set; }

        //t_accident_type

        [Display(Name = "Id")]
        public string id_accident_type { get; set; }

        [Display(Name = "Any injuries or fatalities")]
        public string injury_type { get; set; }

        [Display(Name = "Number of personnel")]
        public string no_person { get; set; }

        //t_accident_info

        [Display(Name = "Id")]
        public string id_accident_info { get; set; }

        [Display(Name = "Reported to Person")]
        public string reported_to { get; set; }

        [Display(Name = "Reported to Date")]
        public DateTime reportedon_date { get; set; }

        [Display(Name = "Review Comments")]
        public string comments { get; set; }

        [Display(Name = "Place of Incident")]
        public string accident_place { get; set; }

        [Display(Name = "Incident Type")]
        public string Incident_Type { get; set; }
       
        [DataType(DataType.MultilineText)]
        [Display(Name = "Immediate actions taken(containment action)")]
        public string Actions_Taken { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        //t_accident_type
        [Display(Name = "Person Name")]
        public string pers_name { get; set; }

        internal bool FunAddAccidentReport(AccidentReportModels objAcc, AccidentReportModelsList objAccTypeList, AccidentReportModelsList objAccInfoList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_accident_report (reported_by,location,details,upload,damage,invest_need,justify,logged_by,branch,accident_place,Incident_Type,Actions_Taken,Department";
                string sFields = "", sFieldValue = "";
                if (objAcc.acc_date != null && objAcc.acc_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", acc_date";
                    sFieldValue = sFieldValue + ", '" + objAcc.acc_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }
                if (objAcc.reported_date != null && objAcc.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", reported_date";
                    sFieldValue = sFieldValue + ", '" + objAcc.reported_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objAcc.reported_by + "','" + objAcc.location + "','" + objAcc.details + "'"
                    + ",'" + objAcc.upload + "','" + objAcc.damage + "','" + objAcc.invest_need + "','" + objAcc.justify + "','" 
                    + objGlobalData.GetCurrentUserSession().empid + "','" + objAcc.branch + "','" + objAcc.accident_place + "','" + objAcc.Incident_Type + "','" + objAcc.Actions_Taken + "','" + objAcc.Department + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_accident_rept = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_accident_rept))
                {
                    if (id_accident_rept > 0 && Convert.ToInt32(objAccTypeList.AccidentList.Count) > 0)
                    {
                        objAccTypeList.AccidentList[0].id_accident_rept = id_accident_rept.ToString();
                        FunAddAccidentType(objAccTypeList);
                    }

                    if (id_accident_rept > 0 && Convert.ToInt32(objAccInfoList.AccidentList.Count) > 0)
                    {
                        objAccInfoList.AccidentList[0].id_accident_rept = id_accident_rept.ToString();
                        FunAddAccInformation(objAccInfoList);
                    }

                    if (id_accident_rept > 0)
                    {

                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("ACC", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            report_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        DataSet dsData1 = objGlobalData.GetReportNo("ACC1", "", LocationName);
                        if (dsData1 != null && dsData1.Tables.Count > 0)
                        {
                            invest_reportno = dsData1.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_accident_report set report_no='" + report_no + "', invest_reportno ='" + invest_reportno + "' where id_accident_rept = '" + id_accident_rept + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAccidentReport: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAccidentType(AccidentReportModelsList objAccTypeList)
        {
            try
            {
                string sSqlstmt = "delete from t_accident_type where id_accident_rept='" + objAccTypeList.AccidentList[0].id_accident_rept + "'; ";

                for (int i = 0; i < objAccTypeList.AccidentList.Count; i++)
                {
                    string sid_accident_type = "null";
                    if (objAccTypeList.AccidentList[i].id_accident_type != null)
                    {
                        sid_accident_type = objAccTypeList.AccidentList[i].id_accident_type;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_accident_type (id_accident_type,id_accident_rept,injury_type,no_person,pers_name)"
                    + " values(" + sid_accident_type + "," + objAccTypeList.AccidentList[0].id_accident_rept
                    + ",'" + objAccTypeList.AccidentList[i].injury_type + "','" + objAccTypeList.AccidentList[i].no_person + "','" + objAccTypeList.AccidentList[i].pers_name + "')"
                    + " ON DUPLICATE KEY UPDATE "
                     + " id_accident_type= values(id_accident_type), id_accident_rept= values(id_accident_rept), injury_type = values(injury_type), no_person= values(no_person), pers_name= values(pers_name) ; ";

                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAccidentType: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAccInformation(AccidentReportModelsList objAccInfoList)
        {
            try
            {
                string sSqlstmt = "delete from t_accident_info where id_accident_rept='" + objAccInfoList.AccidentList[0].id_accident_rept + "'; ";

                for (int i = 0; i < objAccInfoList.AccidentList.Count; i++)
                {
                    string sid_accident_info = "null";
                    string sreportedon_date = objAccInfoList.AccidentList[i].reportedon_date.ToString("yyyy-MM-dd");

                    if (objAccInfoList.AccidentList[i].id_accident_info != null)
                    {
                        sid_accident_info = objAccInfoList.AccidentList[i].id_accident_info;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_accident_info (id_accident_info,id_accident_rept,reported_to,reportedon_date,comments)"
                    + " values(" + sid_accident_info + "," + objAccInfoList.AccidentList[0].id_accident_rept + ",'" + objAccInfoList.AccidentList[i].reported_to
                    + "','" + sreportedon_date + "','" + objAccInfoList.AccidentList[i].comments + "')"
                    + " ON DUPLICATE KEY UPDATE "
                     + " id_accident_info= values(id_accident_info),id_accident_rept= values(id_accident_rept), reported_to= values(reported_to), reportedon_date = values(reportedon_date), comments= values(comments) ; ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAccInformation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAccidentReport(AccidentReportModels objAcc, AccidentReportModelsList objAccTypeList, AccidentReportModelsList objAccInfoList)
        {
            try
            {
                string sSqlstmt = "update t_accident_report set reported_by ='" + objAcc.reported_by + "', location='" + objAcc.location + "', "
                    + "details='" + objAcc.details + "',upload='" + objAcc.upload + "',damage='" + objAcc.damage + "',invest_need='" + objAcc.invest_need
                    + "',justify='" + objAcc.justify + "',accident_place='" + objAcc.accident_place + "',Incident_Type='" + objAcc.Incident_Type 
                    + "',Actions_Taken='" + objAcc.Actions_Taken + "',branch='" + objAcc.branch + "',Department='" + objAcc.Department + "'";

                if (objAcc.acc_date != null && objAcc.acc_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", acc_date='" + objAcc.acc_date.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }

                if (objAcc.reported_date != null && objAcc.reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", reported_date='" + objAcc.reported_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_accident_rept='" + objAcc.id_accident_rept + "'";
                int id_accident_rept = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_accident_rept))
                {
                    if (Convert.ToInt32(objAccTypeList.AccidentList.Count) > 0)
                    {
                        objAccTypeList.AccidentList[0].id_accident_rept = objAcc.id_accident_rept;
                        FunAddAccidentType(objAccTypeList);
                    }
                    else
                    {
                        FunUpdateAccidentType(objAcc.id_accident_rept);

                    }
                    if (Convert.ToInt32(objAccInfoList.AccidentList.Count) > 0)
                    {
                        objAccInfoList.AccidentList[0].id_accident_rept = objAcc.id_accident_rept;
                        FunAddAccInformation(objAccInfoList);
                    }
                    else
                    {
                        FunUpdateAccInformation(objAcc.id_accident_rept);
                    }                   
                   
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccidentReport: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAccidentType(string sid_accident_rept)
        {
            try
            {
                string sSqlstmt = "delete from t_accident_info where id_accident_rept='" + sid_accident_rept + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccidentType: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAccInformation(string sid_accident_rept)
        {
            try
            {
                string sSqlstmt = "delete from t_accident_type where id_accident_rept='" + sid_accident_rept + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAccInformation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteAccidentDoc(string sid_accident_rept)
        {
            try
            {
                string sSqlstmt = "update t_accident_report set Active=0 where id_accident_rept='" + sid_accident_rept + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAccidentDoc: " + ex.ToString());
            }
            return false;
        }

    }

    public class AccidentReportModelsList
    {
        public List<AccidentReportModels> AccidentList { get; set; }
    }
}