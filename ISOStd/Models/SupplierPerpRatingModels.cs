using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace ISOStd.Models
{
    public class SupplierPerpRatingModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_sup_rating { get; set; }

        [Display(Name = "External Provider Name")]
        public string supplier_name { get; set; }

        [Display(Name = "Evaluation Date")]
        public DateTime evalu_date { get; set; }

        [Display(Name = "Auditee")]
        public string auditee { get; set; }

        [Display(Name = "Auditor")]
        public string auditor { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "Overall Performance")]
        public string overall_perf { get; set; }

        [Display(Name = "Exceptional")]
        public string exceptional { get; set; }

        [Display(Name = "Satisfactory")]
        public string satisfactory { get; set; }

        [Display(Name = "UnSatisfactory")]
        public string unsatisfactory { get; set; }

        [Display(Name = "N/A")]
        public string na { get; set; }

        [Display(Name = "Insufficient info. to rate")]
        public string insufficient { get; set; }

        public string loggedby { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Evaluated By")]
        public string evaluated_by { get; set; }

        //Trans

        [Display(Name = "ID")]
        public string supplier_trans_id { get; set; }

        
        public string SQId { get; set; }
        public string SQ_OptionsId { get; set; }
        public string SQ_Weightage { get; set; }

        [Display(Name = "Actions to be Initiated")]
        public string actions_init { get; set; }
        

        internal bool FunAddSupPerformanceEvaluation(SupplierPerpRatingModels ObjModel, SupplierPerpRatingModelsList ObjList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                //string sBranch = objGlobalData.GetCurrentUserSession().division;


                string sSqlstmt = "insert into t_supplier_perf_rating (supplier_name, auditee, auditor, upload, loggedby,overall_perf,exceptional," +
                    "satisfactory,unsatisfactory,na,insufficient,branch,Department,Location,evaluated_by,actions_init";

                if (ObjModel.evalu_date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", evalu_date";
                    sValues = sValues + ", '" + ObjModel.evalu_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }               

                sSqlstmt = sSqlstmt + sColumn + ") values('" + ObjModel.supplier_name + "','" + ObjModel.auditee + "','" + ObjModel.auditor
                 + "','" + ObjModel.upload + "','" + user + "','" + ObjModel.overall_perf + "','" + ObjModel.exceptional + "','" + ObjModel.satisfactory
                 + "','" + ObjModel.unsatisfactory + "','" + ObjModel.na + "','" + ObjModel.insufficient + "','" + ObjModel.branch + "','" + ObjModel.Department + "','" + ObjModel.Location + "','" + ObjModel.evaluated_by + "','" + ObjModel.actions_init + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int iId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iId))
                {
                    SupplierPerpRatingModels objElement = new SupplierPerpRatingModels();

                    ObjList.PerpList[0].id_sup_rating = iId.ToString();
                    objElement.FunAddSuppPerformanceList(ObjList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool FunAddSuppPerformanceList(SupplierPerpRatingModelsList ObjList)
        {
            try
            {
                if (ObjList.PerpList.Count > 0)
                {
                    string sSqlstmt = "delete from t_supplier_perf_rating_trans where id_sup_rating='"
                        + ObjList.PerpList[0].id_sup_rating + "'; ";
                    for (int i = 0; i < ObjList.PerpList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_supplier_perf_rating_trans (id_sup_rating, SQId, SQ_OptionsId"
                        + ") values('" + ObjList.PerpList[0].id_sup_rating + "','" + ObjList.PerpList[i].SQId
                        + "','" + ObjList.PerpList[i].SQ_OptionsId + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSuppPerformanceList: " + ex.ToString());

            }
            return false;
        }

        
        internal bool FunUpdateSupPerformanceEvaluation(SupplierPerpRatingModels ObjModel, SupplierPerpRatingModelsList ObjList)
        {
            try
            {
                string sSqlstmt = "update t_supplier_perf_rating set supplier_name='" + ObjModel.supplier_name + "', auditee='" + ObjModel.auditee
                    + "', auditor='" + ObjModel.auditor + "', overall_perf='" + ObjModel.overall_perf + "', exceptional='" + ObjModel.exceptional + "', satisfactory='" + ObjModel.satisfactory
                     + "', unsatisfactory='" + ObjModel.unsatisfactory + "', na='" + ObjModel.na + "', insufficient='" + ObjModel.insufficient
                      + "', branch='" + ObjModel.branch + "', Department='" + ObjModel.Department + "', Location='" + ObjModel.Location + "', evaluated_by='" + ObjModel.evaluated_by + "', actions_init='" + ObjModel.actions_init + "'"; 

                if (ObjModel.upload != null)
                {
                    sSqlstmt = sSqlstmt + ", upload='" + ObjModel.upload + "' ";
                }

                if (ObjModel.evalu_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", evalu_date='" + ObjModel.evalu_date.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                               

                sSqlstmt = sSqlstmt + " where id_sup_rating='" + ObjModel.id_sup_rating + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SupplierPerpRatingModels objElement = new SupplierPerpRatingModels();
                    objElement.FunAddSuppPerformanceList(ObjList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }
      

        internal bool FunDeleteSupPerformance(string sid_sup_rating)
        {
            try
            {
                string sSqlstmt = "update t_supplier_perf_rating set Active=0 where id_sup_rating='" + sid_sup_rating + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSupPerformance: " + ex.ToString());

            }
            return false;
        }

        public string GetFunRatingId(string rating)
        {
            try
            {

                string sSqlstmt = "select SQ_OptionsId from t_surveyquestion_options where RatingOptions  = '" + rating + "' and Survey_TypeId=3";
                DataSet dsrate = objGlobalData.Getdetails(sSqlstmt);
                if (dsrate.Tables.Count > 0 && dsrate.Tables[0].Rows.Count > 0)
                {
                    return (dsrate.Tables[0].Rows[0]["SQ_OptionsId"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFunRatingId: " + ex.ToString());
            }
            return "";
        }
    }
    public class SupplierPerpRatingModelsList
    {
        public List<SupplierPerpRatingModels> PerpList { get; set; }
    }
}