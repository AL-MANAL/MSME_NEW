using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class SupplierPerformanceModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        public int Sup_Perf_id { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime Eval_FromDate { get; set; }

        [Display(Name = "Evaluation Year")]
        //public DateTime Eval_ToDate { get; set; }
        public string Eval_ToDate { get; set; }

        [Display(Name = "External Provider Name")]
        public string Supplier_Name { get; set; }

        [Display(Name = "Scope of Supplies / Services")]
        public string Scope_OfSupplies { get; set; }

        [Display(Name = "Total no of deliveries")]
        public decimal total_delv { get; set; }

        [Display(Name = "Accepted No of deliveries")]
        public decimal accept_delv { get; set; }

        [Display(Name = "No of delivery on time")]
        public decimal ontime_delv { get; set; }

        [Display(Name = "lowest net price")]
        public decimal lowest_price { get; set; }

        [Display(Name = "External Provider net price")]
        public decimal supp_price { get; set; }

        [Display(Name = "Number of SHE compliance")]
        public decimal SHE_total { get; set; }

        [Display(Name = "Action to be taken")]
        public string Action_sup { get; set; }

        public string Criteria { get; set; }
        public string perc { get; set; }

        [Display(Name = "QUALITY")]
        public decimal QUALITY { get; set; }

        [Display(Name = "PRICE")]
        public decimal PRICE { get; set; }

        [Display(Name = "SERVICE")]
        public decimal SERVICE { get; set; }

        [Display(Name = "Safety,Health & Environment(SHE) compliance")]
        public decimal SHE { get; set; }

        public decimal rating { get; set; }

        [Display(Name = "OSH Compliance")]
        public string hse_compliance { get; set; }

        [Display(Name = "ISO 9001 Compliance")]
        public string iso9001_compliance { get; set; }

        [Display(Name = "Recommended By")]
        public string recomend_by { get; set; }

        [Display(Name = "Payment Terms")]
        public string payment_terms { get; set; }

        [Display(Name = "After Sale Performance")]
        public string sale_perf { get; set; }

        [Display(Name = "Environment Mgmt Sytem Compliance")]
        public string EnvMgmt { get; set; }

        internal bool FunDeleteSupplierPerformanceDoc(string sSup_Perf_id)
        {
            try
            {
                string sSqlstmt = "update t_supplier_performance set Active=0 where Sup_Perf_id='" + sSup_Perf_id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSupplierPerformanceDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddSupplierPerf(SupplierPerformanceModels objSupplierPerformanceModels)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_Supplier_Performance (Supplier_Name, Scope_OfSupplies, total_delv,"
                    + " accept_delv, ontime_delv, lowest_price,supp_price,SHE_total,hse_compliance,iso9001_compliance,recomend_by,payment_terms,sale_perf,EnvMgmt,branch";
                string sFields = "", sFieldValue = "";
                if (objSupplierPerformanceModels.Eval_FromDate != null && objSupplierPerformanceModels.Eval_FromDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Eval_FromDate";
                    sFieldValue = sFieldValue + ", '" + objSupplierPerformanceModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }
                //  if (objSupplierPerformanceModels.Eval_ToDate != null && objSupplierPerformanceModels.Eval_ToDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                if (objSupplierPerformanceModels.Eval_ToDate != null)
                {
                    sFields = sFields + ", Eval_ToDate";
                    sFieldValue = sFieldValue + ", '" + objSupplierPerformanceModels.Eval_ToDate + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ")  values('" + objSupplierPerformanceModels.Supplier_Name + "','" + objSupplierPerformanceModels.Scope_OfSupplies
                + "','" + objSupplierPerformanceModels.total_delv + "','" + objSupplierPerformanceModels.accept_delv + "','" + objSupplierPerformanceModels.ontime_delv
                + "','" + objSupplierPerformanceModels.lowest_price + "','" + objSupplierPerformanceModels.supp_price + "','" + objSupplierPerformanceModels.SHE_total + "','" + objSupplierPerformanceModels.hse_compliance + "'"
                + ",'" + objSupplierPerformanceModels.iso9001_compliance + "','" + objSupplierPerformanceModels.recomend_by + "','" + objSupplierPerformanceModels.payment_terms + "','" + objSupplierPerformanceModels.sale_perf + "','" + objSupplierPerformanceModels.EnvMgmt + "','" + sBranch + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplierPerf: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSupplierPerf(SupplierPerformanceModels objSupplierPerformanceModels)
        {
            try
            {
                string sSqlstmt = "update t_Supplier_Performance set  Supplier_Name='" + objSupplierPerformanceModels.Supplier_Name
                    + "', Scope_OfSupplies='" + objSupplierPerformanceModels.Scope_OfSupplies + "', total_delv='" + objSupplierPerformanceModels.total_delv
                    + "', accept_delv='" + objSupplierPerformanceModels.accept_delv + "', ontime_delv='"
                    + objSupplierPerformanceModels.ontime_delv + "', lowest_price='" + objSupplierPerformanceModels.lowest_price + "'"
                    + ", supp_price='" + objSupplierPerformanceModels.supp_price + "', SHE_total='" + objSupplierPerformanceModels.SHE_total + "'"
                    + ", hse_compliance='" + objSupplierPerformanceModels.hse_compliance + "', iso9001_compliance='" + objSupplierPerformanceModels.iso9001_compliance + "'"
                    + ", recomend_by='" + objSupplierPerformanceModels.recomend_by + "', payment_terms='" + objSupplierPerformanceModels.payment_terms + "'"
                    + ", sale_perf='" + objSupplierPerformanceModels.sale_perf + "', EnvMgmt='" + objSupplierPerformanceModels.EnvMgmt + "'";
                if (objSupplierPerformanceModels.Eval_FromDate != null && objSupplierPerformanceModels.Eval_FromDate > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Eval_FromDate='" + objSupplierPerformanceModels.Eval_FromDate.ToString("yyyy/MM/dd") + "'";
                }
                if (objSupplierPerformanceModels.Eval_ToDate != null)
                {
                    sSqlstmt = sSqlstmt + ", Eval_ToDate='" + objSupplierPerformanceModels.Eval_ToDate + "'";
                }
                sSqlstmt = sSqlstmt + " where Sup_Perf_id='" + objSupplierPerformanceModels.Sup_Perf_id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierPerf: " + ex.ToString());
            }
            return false;
        }



    }

    public class SupplierPerformanceModelsList
    {
        public List<SupplierPerformanceModels> SupplierPerformanceMList { get; set; }
    }
}