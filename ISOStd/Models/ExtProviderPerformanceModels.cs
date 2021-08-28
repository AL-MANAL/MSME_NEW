using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class ExtProviderPerformanceModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //t_extprovider_performance
        [Display(Name = "Id")]
        public string Id_Performace { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime Eval_Date { get; set; }

        [Display(Name = "Evaluation From Date")]
        public DateTime Eval_FromDate { get; set; }

        [Display(Name = "Evaluation To Date")]
        public DateTime Eval_ToDate { get; set; }

        [Display(Name = "External Provider Name")]
        public string Ext_Provider_Name { get; set; }

        [Display(Name = "Scope of Supply or Service")]
        public string Scope_ofSupplies { get; set; }

        [Display(Name = "Report Number")]
        public string ReportNo { get; set; }

        [Display(Name = "Total number of POs issued")]
        public decimal PO_Issued { get; set; }

        [Display(Name = "Total number of POs completed")]
        public decimal PO_Completed { get; set; }

        [Display(Name = "Total number of quality issues")]
        public decimal Quality_Issue { get; set; }

        [Display(Name = "Total number of delivery issues")]
        public decimal Delivery_Issue { get; set; }

        [Display(Name = "Scheduled By")]
        public string Scheduled_by { get; set; }

        [Display(Name = "To be reviewed & verified by")]
        public string Approved_by { get; set; }

        //Display
        [Display(Name = "Quality rating")]
        public decimal QualityRate { get; set; }

        [Display(Name = "Delivery rating")]
        public decimal DeliveryRate { get; set; }

        [Display(Name = "External Provider Performance Rating")]
        public decimal Perfp_rate { get; set; }

        [Display(Name = "Actions based on performance rating")]
        public string Perfp_rate_action { get; set; }

        //t_extprovider_performance_trans

        public string id_Performance_trans { get; set; }

        [Display(Name = "Actions")]
        public string Actions { get; set; }

        [Display(Name = "Personnel Responsible")]
        public string Personnel_Resp { get; set; }

        [Display(Name = "Due Date")]
        public DateTime Due_Date { get; set; }

        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunDeleteExtProviderPerf(string sId_Performace)
        {
            try
            {
                string sSqlstmt = "update t_extprovider_performance set Active=0 where Id_Performace='" + sId_Performace + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteExtProviderPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExtProviderPerf(ExtProviderPerformanceModels ObjPerfModels, ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_extprovider_performance (Ext_Provider_Name, Scope_ofSupplies, PO_Issued,"
                    + " PO_Completed, Scheduled_by,Approved_by,branch,Department,Location";
                string sFields = "", sFieldValue = "";

                if (ObjPerfModels.Eval_Date != null && ObjPerfModels.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_Date";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_Date.ToString("yyyy/MM/dd") + "'";
                }

                if (ObjPerfModels.Eval_FromDate != null && ObjPerfModels.Eval_FromDate > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_FromDate";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }

                if (ObjPerfModels.Eval_ToDate != null && ObjPerfModels.Eval_ToDate > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Eval_ToDate";
                    sFieldValue = sFieldValue + ", '" + ObjPerfModels.Eval_ToDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")  values('" + ObjPerfModels.Ext_Provider_Name + "','" + ObjPerfModels.Scope_ofSupplies
                //+ "','" + ObjPerfModels.PO_Issued + "','" + ObjPerfModels.PO_Completed + "','" + ObjPerfModels.Quality_Issue
                //+ "','" + ObjPerfModels.Delivery_Issue + "','" + ObjPerfModels.Scheduled_by + "','" + ObjPerfModels.Approved_by + "','" + sBranch + "'";
                + "','" + ObjPerfModels.PO_Issued + "','" + ObjPerfModels.PO_Completed 
                + "','" + ObjPerfModels.Scheduled_by + "','" + ObjPerfModels.Approved_by + "','" + ObjPerfModels.branch
                + "','" + ObjPerfModels.Department + "','" + ObjPerfModels.Location + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int Id_Performace = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id_Performace))
                {
                    if (Id_Performace > 0 && Convert.ToInt32(objPerfList.ExtPerfpList.Count) > 0)
                    {
                        objPerfList.ExtPerfpList[0].Id_Performace = Id_Performace.ToString();
                        FunAddExtProviderPerfTrans(objPerfList);
                    }

                    if (Id_Performace > 0)
                    {
                        string LocationName = objGlobalData.GetCompanyBranchNameById(sBranch);
                        DataSet dsData = objGlobalData.GetReportNo("EXTPRO", "", LocationName);
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ReportNo = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }

                        string sql = "update t_extprovider_performance set ReportNo='" + ReportNo + "' where Id_Performace = '" + Id_Performace + "'";
                        return (objGlobalData.ExecuteQuery(sql));
                        //{
                        //    SendAuditScheduleEmail(Id_Performace, "Planning or Scheduling the Audits");
                        //}
                        //return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddExtProviderPerfTrans(ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {

                string sSqlstmt = "delete from t_extprovider_performance_trans where Id_Performace='" + objPerfList.ExtPerfpList[0].Id_Performace + "'; ";

                for (int i = 0; i < objPerfList.ExtPerfpList.Count; i++)
                {
                    //  string sid_Performance_trans = "null";
                    //if (objPerfList.ExtPerfpList[i].id_Performance_trans != null)
                    //{
                    //    sid_Performance_trans = objPerfList.ExtPerfpList[i].id_Performance_trans;
                    //}
                    sSqlstmt = sSqlstmt + " insert into t_extprovider_performance_trans (Id_Performace,Actions,Personnel_Resp,Priority";
                    string sFields = "", sFieldValue = "", sFieldValues = "";

                    if (objPerfList.ExtPerfpList[i].Due_Date != null && objPerfList.ExtPerfpList[i].Due_Date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Due_Date";
                        sFieldValue = sFieldValue + ", '" + objPerfList.ExtPerfpList[i].Due_Date.ToString("yyyy/MM/dd") + "'";
                        sFieldValues = sFieldValues + ", Due_Date = values(Due_Date)";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values( '" + objPerfList.ExtPerfpList[0].Id_Performace
                    + "','" + objPerfList.ExtPerfpList[i].Actions + "','" + objPerfList.ExtPerfpList[i].Personnel_Resp + "','" + objPerfList.ExtPerfpList[i].Priority + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sSqlstmt = sSqlstmt + " ON DUPLICATE KEY UPDATE "
                     + " Id_Performace= values(Id_Performace), Actions = values(Actions), Personnel_Resp= values(Personnel_Resp), Priority= values(Priority)";
                    sSqlstmt = sSqlstmt + sFieldValues + " ;";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderPerfTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderPerf(ExtProviderPerformanceModels ObjPerfModels, ExtProviderPerformanceModelsList objPerfList)
        {
            try
            {
               
                string sSqlstmt = "update t_extprovider_performance set  Ext_Provider_Name='" + ObjPerfModels.Ext_Provider_Name
                   + "', Scope_ofSupplies='" + ObjPerfModels.Scope_ofSupplies + "', PO_Issued='" + ObjPerfModels.PO_Issued
                   + "', PO_Completed='" + ObjPerfModels.PO_Completed /*+ "', Quality_Issue='" + ObjPerfModels.Quality_Issue + "', Delivery_Issue='" + ObjPerfModels.Delivery_Issue +*/
                   + "', Scheduled_by='" + ObjPerfModels.Scheduled_by + "', Approved_by='" + ObjPerfModels.Approved_by
                   + "', branch='" + ObjPerfModels.branch + "', Department='" + ObjPerfModels.Department + "', Location='" + ObjPerfModels.Location + "'";

                if (ObjPerfModels.Eval_Date != null && ObjPerfModels.Eval_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_Date='" + ObjPerfModels.Eval_Date.ToString("yyyy/MM/dd") + "'";
                }
                if (ObjPerfModels.Eval_FromDate != null && ObjPerfModels.Eval_FromDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_FromDate='" + ObjPerfModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }
                if (ObjPerfModels.Eval_ToDate != null && ObjPerfModels.Eval_ToDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_ToDate='" + ObjPerfModels.Eval_ToDate.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where Id_Performace='" + ObjPerfModels.Id_Performace + "'";

                int Id_Performace = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Id_Performace))
                {
                    if (Convert.ToInt32(objPerfList.ExtPerfpList.Count) > 0)
                    {
                        objPerfList.ExtPerfpList[0].Id_Performace = ObjPerfModels.Id_Performace;
                        FunAddExtProviderPerfTrans(objPerfList);
                    }
                    else
                    {
                        FunUpdateExtProviderPerfTrans(ObjPerfModels.Id_Performace);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderPerfTrans(string sId_Performace)
        {
            try
            {
                string sSqlstmt = "delete from t_extprovider_performance_trans where Id_Performace='" + sId_Performace + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditTeamDetails: " + ex.ToString());
            }
            return false;
        }
    }

    public class ExtProviderPerformanceModelsList
    {
        public List<ExtProviderPerformanceModels> ExtPerfpList { get; set; }
    }

    public class ExtProviderDiscrepencyLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();
      
        [Display(Name = "Id")]
        public string id_discrepancylog { get; set; }

        [Required]
        [Display(Name = "Discrepancy Registered On")]
        public DateTime discre_registerd_date { get; set; }
        
        [Required]
        [Display(Name = "Discrepancy Identified / Reported On")]
        public DateTime discre_reported_date { get; set; }

        [Required]
        [Display(Name = "Discrepancy Number")]
        public string discrepancy_no { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Discrepancy details")]
        public string discre_detail { get; set; }

        [Required]
        [Display(Name = "Containment actions initiated")]
        public string actions { get; set; }

        [Required]
        [Display(Name = "Documents to upload")]
        public string upload { get; set; }

        [Required]
        [Display(Name = "External Provider Name")]
        public string ext_provider_name { get; set; }

        [Required]
        [Display(Name = "Delivery Note Number")]
        public string delivery_note_no { get; set; }

        [Required]
        [Display(Name = "Our PO number to External Provider")]
        public string po_no { get; set; }

        [Required]
        [Display(Name = "Discrepancy related to")]
        public string discre_relatedto { get; set; }

        [Required]
        [Display(Name = "Supplier Ref")]
        public string supplier_ref { get; set; }

        [Required]
        [Display(Name = "Impact on our performance due to discrepancy")]
        public string impact { get; set; }

        [Required]
        [Display(Name = "Need of raising NCR to External Provider?")]
        public string ncr_required { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        internal bool FunAddExtProviderDiscrepencyLog(ExtProviderDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
               // string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sUser = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "insert into t_external_provider_discrepancylog (discrepancy_no, ext_provider_name, " +
                    "delivery_note_no, po_no, discre_detail,discre_relatedto,upload,actions,impact,ncr_required,logged_by,branch,Department,Location";
                string sFields = "", sFieldValue = "";

                if (objDescripencyLog.discre_registerd_date != null && objDescripencyLog.discre_registerd_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", discre_registerd_date";
                    sFieldValue = sFieldValue + ", '" + objDescripencyLog.discre_registerd_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objDescripencyLog.discre_reported_date != null && objDescripencyLog.discre_reported_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", discre_reported_date";
                    sFieldValue = sFieldValue + ", '" + objDescripencyLog.discre_reported_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objDescripencyLog.discrepancy_no + "','" + objDescripencyLog.ext_provider_name + "','" + objDescripencyLog.delivery_note_no
                        + "','" + objDescripencyLog.po_no + "','" + objDescripencyLog.discre_detail + "','" + objDescripencyLog.discre_relatedto + "','" + objDescripencyLog.upload 
                        + "','" + objDescripencyLog.actions + "','" + objDescripencyLog.impact + "','" + objDescripencyLog.ncr_required + "','" + sUser + "','" + objDescripencyLog.branch
                         + "','" + objDescripencyLog.Department + "','" + objDescripencyLog.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExtProviderDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExtProviderDiscrepencyLog(ExtProviderDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
                string sSqlstmt = "update t_external_provider_discrepancylog set discrepancy_no='" + objDescripencyLog.discrepancy_no + "', ext_provider_name='" + objDescripencyLog.ext_provider_name
                    + "', delivery_note_no='" + objDescripencyLog.delivery_note_no + "', po_no='" + objDescripencyLog.po_no + "', discre_detail='" + objDescripencyLog.discre_detail + "', discre_relatedto='" + objDescripencyLog.discre_relatedto + "', actions='" + objDescripencyLog.actions + "'"
                    + ", impact='" + objDescripencyLog.impact + "', ncr_required='" + objDescripencyLog.ncr_required + "', branch='" + objDescripencyLog.branch + "', Department='" + objDescripencyLog.Department + "', Location='" + objDescripencyLog.Location + "'";

                if (objDescripencyLog.discre_registerd_date != null && objDescripencyLog.discre_registerd_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", discre_registerd_date='" + objDescripencyLog.discre_registerd_date.ToString("yyyy/MM/dd") + "'";
                }

                if (objDescripencyLog.discre_reported_date != null && objDescripencyLog.discre_reported_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", discre_reported_date='" + objDescripencyLog.discre_reported_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objDescripencyLog.upload != null)
                {
                    sSqlstmt = sSqlstmt + ", upload='" + objDescripencyLog.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where id_discrepancylog='" + objDescripencyLog.id_discrepancylog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExtProviderDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteDiscrepencyLogDoc(string sid_discrepancylog)
        {
            try
            {
                string sSqlstmt = "update t_external_provider_discrepancylog set active=0 where id_discrepancylog='" + sid_discrepancylog + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDiscrepencyLogDoc: " + ex.ToString());
            }
            return false;
        }

    }

    public class ExtProviderDiscrepencyLogModelsList
    {
        public List<ExtProviderDiscrepencyLogModels> DescripList{ get; set; }
    } 
}