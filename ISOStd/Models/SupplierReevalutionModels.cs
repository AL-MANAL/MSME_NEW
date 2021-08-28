using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ISOStd.Models
{
    public class SupplierReevalutionModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_reevaluation { get; set; }
           
        [Display(Name = "Performance Review Year")]
        public string perf_review_year { get; set; }

        [Display(Name = "Supplier")]
        public string supplier { get; set; }

        [Display(Name = "Verification of certification")]
        public string certification { get; set; }

        [Display(Name = "If others certification, specify")]
        public string other_certification { get; set; }

        [Display(Name = "Audit at supplier site required")]
        public string supp_required { get; set; }

        [Display(Name = "If Yes, Auditor Name")]
        public string auditor_name { get; set; }

        [Display(Name = "Audit Done on")]
        public DateTime audit_date { get; set; }

        [Display(Name = "Attach Audit report")]
        public string audit_upload { get; set; }

        [Display(Name = "Complaints received during this fiscal year related to the above supplier?")]
        public string supp_complaint { get; set; }

        [Display(Name = "If yes, was the complaint handled satisfactorily")]
        public string issatisfactory { get; set; }

        [Display(Name = "Any incident / accident reported during this fiscal year related to the above supplier? ")]
        public string anycomplaint { get; set; }

        [Display(Name = "If yes, was it handled satisfactorily?")]
        public string ishandled { get; set; }

        [Display(Name = "REMARKS / OBSERVATIONS ")]
        public string remark { get; set; }

        [Display(Name = "On site visit carried out")]
        public string visited { get; set; }
        
        [Display(Name = "If yes, submit Visit Report.")]
        public string visit_upload { get; set; }

        [Display(Name = "Date of visit")]
        public DateTime date_visited { get; set; }

        [Display(Name = "Visited by")]
        public string visited_by { get; set; }
        
        [Display(Name = "Status of Recommendation")]
        public string recommanded { get; set; }

        [Display(Name = "Status of Approval")]
        public string approved { get; set; }

        [Display(Name = "Recommended by")]
        public string recommanded_to { get; set; }

        [Display(Name = "Approved by")]
        public string approved_to { get; set; }

        public string logged_by { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        //t_supplier_reevaluation_trans
        [Display(Name = "ID")]
        public string id_reevaluation_trans { get; set; }
        
        [Display(Name = "Date")]
        public DateTime date_reevaluation { get; set; }

        [Display(Name = "Customer Name")]
        public string cust_name { get; set; }

        [Display(Name = "Compliments[Brief Description of Compliments]")]
        public string complaints { get; set; }

        [Display(Name = "Complaints[Brief Description of Complaints: Malfunction, Rejection etc.]")]
        public string description_complaint { get; set; }

        [Display(Name = "Complaint Ref. No. where it is recorded")]
        public string ref_no_complaint { get; set; }

        public string supp_code { get; set; }
        public string supp_scope { get; set; }

        //t_supplier_reevaluation_quest
        [Display(Name = "ID")]
        public string id_reevaluation_quest { get; set; }

        [Display(Name = "Questions")]
        public string SQId { get; set; }

        [Display(Name = "Options")]
        public string SQ_OptionsId { get; set; }

        internal bool FunAddSupplierReevalution(SupplierReevalutionModels ObjModel, SupplierReevalutionModelsList ObjList, SupplierReevalutionModelsList ObjQuestList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string sBranch = objGlobalData.GetCurrentUserSession().division;
                
                string sSqlstmt = "insert into t_supplier_reevaluation (supplier, certification, other_certification, supp_required," +
                    "auditor_name,audit_upload,supp_complaint,issatisfactory,anycomplaint,ishandled,remark,visited,visit_upload,visited_by," +
                    "recommanded,approved,recommanded_to,approved_to,logged_by,branch,perf_review_year,Department,Location";

                //if (ObjModel.perf_review_year > Convert.ToDateTime("01/01/0001"))
                //{
                //    sColumn = sColumn + ", perf_review_year";
                //    sValues = sValues + ", '" + ObjModel.perf_review_year.ToString("yyyy-MM-dd") + "' ";
                //}

                if (ObjModel.audit_date > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", audit_date";
                    sValues = sValues + ", '" + ObjModel.audit_date.ToString("yyyy-MM-dd") + "' ";
                }

                if (ObjModel.date_visited > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", date_visited";
                    sValues = sValues + ", '" + ObjModel.date_visited.ToString("yyyy-MM-dd") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + ObjModel.supplier + "','" + ObjModel.certification
                 + "','" + ObjModel.other_certification + "','" + ObjModel.supp_required + "','" + ObjModel.auditor_name + "','" + ObjModel.audit_upload + "','" + ObjModel.supp_complaint
                 + "','" + ObjModel.issatisfactory + "','" + ObjModel.anycomplaint + "','" + ObjModel.ishandled + "','" + ObjModel.remark + "','" + ObjModel.visited + "','" + ObjModel.visit_upload
                 + "','" + ObjModel.visited_by + "','" + ObjModel.recommanded + "','" + ObjModel.approved + "','" + ObjModel.recommanded_to 
                 + "','" + ObjModel.approved_to + "','" + user + "','" + ObjModel.branch + "','" + ObjModel.perf_review_year + "','" + ObjModel.Department + "','" + ObjModel.Location + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int iId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iId))
                {
                    SupplierReevalutionModels objElement = new SupplierReevalutionModels();
                    int SuppId = iId;
                    if (SuppId > 0 && Convert.ToInt32(ObjList.EvalList.Count) > 0)
                    {
                        ObjList.EvalList[0].id_reevaluation = SuppId.ToString();
                        objElement.FunAddSupplierReevalutionTans(ObjList);
                    }

                    if (SuppId > 0 && Convert.ToInt32(ObjQuestList.EvalList.Count) > 0)
                    {
                        ObjQuestList.EvalList[0].id_reevaluation = SuppId.ToString();
                        objElement.FunAddSupplierReevalutionQuestTans(ObjQuestList);
                    }
                    if (SuppId > 0)
                    {
                        string ssEmailid = "";
                        string ssName = "";
                        if (ObjModel.recommanded_to != "")
                        {
                            ssEmailid = objGlobalData.GetHrEmpEmailIdById(ObjModel.recommanded_to);
                            ssName = objGlobalData.GetMultiHrEmpNameById(ObjModel.recommanded_to);
                        }
                        if (ssEmailid != "")
                        {
                            ssEmailid = ssEmailid +"," + objGlobalData.GetHrEmpEmailIdById(ObjModel.approved_to);                           
                        }
                        else
                        {
                            ssEmailid = objGlobalData.GetHrEmpEmailIdById(ObjModel.approved_to);
                        }
                        string sEmailid = ssEmailid.TrimEnd(',');
                    
                        if (sEmailid != null && sEmailid != "")
                        {
                            string sExtraMsg = "Supplier Reevaluation is Pending for your Recommandation and Approval, Division: " +objGlobalData.GetMultiCompanyBranchNameById(ObjModel.branch) + "," + " Supplier:" + objGlobalData.GetSupplierNameById(ObjModel.supplier);

                            string sEmailCCList = objGlobalData.GetHrEmpEmailIdById(ObjModel.logged_by);
                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(ssName, "Supplier_Reevaluation", sExtraMsg);

                            // sEmailCCList = sEmailCCList.Trim(',');

                            sEmailid = Regex.Replace(sEmailid, ",+", ",");
                            sEmailid = sEmailid.Trim();
                            sEmailid = sEmailid.TrimEnd(',');
                            sEmailid = sEmailid.TrimStart(',');
                            //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                            return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList, "");
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplierReevalution: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSupplierReevalution(SupplierReevalutionModels ObjModel, SupplierReevalutionModelsList ObjList, SupplierReevalutionModelsList ObjQuestList)
        {
            try
            {
                string sSqlstmt = "update t_supplier_reevaluation set branch='" + ObjModel.branch + "', supplier='" + ObjModel.supplier + "', certification='" + ObjModel.certification
                    + "', other_certification='" + ObjModel.other_certification + "', supp_required='" + ObjModel.supp_required + "', auditor_name='" + ObjModel.auditor_name
                    + "', supp_complaint='" + ObjModel.supp_complaint + "', issatisfactory='" + ObjModel.issatisfactory + "', anycomplaint='" + ObjModel.anycomplaint + "', ishandled='" + ObjModel.ishandled
                    + "', remark='" + ObjModel.remark + "', visited='" + ObjModel.visited + "', visited_by='" + ObjModel.visited_by + "', recommanded='" + ObjModel.recommanded + "', approved='" + ObjModel.approved
                    + "', recommanded_to='" + ObjModel.recommanded_to + "', approved_to='" + ObjModel.approved_to + "', perf_review_year='" + ObjModel.perf_review_year
                    + "', audit_upload='" + ObjModel.audit_upload + "', visit_upload='" + ObjModel.visit_upload + "', Department='" + ObjModel.Department + "', Location='" + ObjModel.Location + "'";

                //if (ObjModel.audit_upload != null)
                //{
                //    sSqlstmt = sSqlstmt + ", audit_upload='" + ObjModel.audit_upload + "' ";
                //}
                //if (ObjModel.visit_upload != null)
                //{
                //    sSqlstmt = sSqlstmt + ", visit_upload='" + ObjModel.visit_upload + "' ";
                //}
                //if (ObjModel.perf_review_year > Convert.ToDateTime("01/01/0001"))
                //{
                //    sSqlstmt = sSqlstmt + ", perf_review_year='" + ObjModel.perf_review_year.ToString("yyyy-MM-dd") + "' ";
                //}
                if (ObjModel.audit_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", audit_date='" + ObjModel.audit_date.ToString("yyyy-MM-dd") + "' ";
                }
                if (ObjModel.date_visited > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", date_visited='" + ObjModel.date_visited.ToString("yyyy-MM-dd") + "' ";
                }
                sSqlstmt = sSqlstmt + " where id_reevaluation='" + ObjModel.id_reevaluation + "'";

                //if (objGlobalData.ExecuteQuery(sSqlstmt))
                //{
                //    SupplierReevalutionModels objElement = new SupplierReevalutionModels();
                //    objElement.FunAddSupplierReevalutionTans(ObjList);
                //    return true;
                //}
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Convert.ToInt32(ObjList.EvalList.Count) > 0)
                    {
                        ObjList.EvalList[0].id_reevaluation = ObjModel.id_reevaluation;
                        FunAddSupplierReevalutionTans(ObjList);
                    }
                    else
                    {
                        FunUpdateSupplierReevalutionTrans(ObjModel.id_reevaluation);
                    }
                    if (Convert.ToInt32(ObjQuestList.EvalList.Count) > 0)
                    {
                        ObjQuestList.EvalList[0].id_reevaluation = ObjModel.id_reevaluation;
                        FunAddSupplierReevalutionQuestTans(ObjQuestList);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierReevalution: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateReevaluationRecommend(string sid_reevaluation, int sStatus)
        {
            try
            {
                string sRecommandDate = DateTime.Now.ToString("yyyy-MM-dd");

                string sSqlstmt = "update t_supplier_reevaluation set isrecommand ='" + sStatus + "', recommand_date='" + sRecommandDate + "' where id_reevaluation='" + sid_reevaluation + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendReevaluationRecommandEmail(sid_reevaluation, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateReevaluationRecommend: " + ex.ToString());
            }
            return false;
        }

        internal bool SendReevaluationRecommandEmail(string id_reevaluation, int iStatus)
        {
            try
            {
                string sEmailid = "", sCCList = "", sExtraMsg = "";

                DataSet dsDocument = objGlobalData.Getdetails("select branch, supplier, recommanded,approved,logged_by from t_supplier_reevaluation where id_reevaluation='" + id_reevaluation + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    if (iStatus == 1)//approved 
                    {
                        if (dsDocument.Tables[0].Rows[0]["logged_by"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                        }

                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approved"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Supplier Reevaualtion has been Recommanded,Supplier: " + objGlobalData.GetSupplierNameById(dsDocument.Tables[0].Rows[0]["supplier"].ToString());
                    }

                    if (iStatus == 2)//Rejected 
                    {
                        if (dsDocument.Tables[0].Rows[0]["approved"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approved"].ToString());
                        }

                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Supplier Reevaluation Recommandation has been Rejected, Supplier: " + objGlobalData.GetSupplierNameById(dsDocument.Tables[0].Rows[0]["supplier"].ToString());
                    }

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["recommanded"].ToString()), "Supplier_Reevaluation", sExtraMsg);

                    objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendReevaluationRecommandEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateReevaluationApprvReject(string sid_reevaluation, int sStatus)
        {
            try
            {
                string sApproveDate = DateTime.Now.ToString("yyyy-MM-dd");

                string sSqlstmt = "update t_supplier_reevaluation set isapproved ='" + sStatus + "', approved_date='" + sApproveDate + "' where id_reevaluation='" + sid_reevaluation + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendReevaluationApprvRejectEmail(sid_reevaluation, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateReevaluationApprvReject: " + ex.ToString());
            }
            return false;
        }

        internal bool SendReevaluationApprvRejectEmail(string id_reevaluation, int iStatus)
        {
            try
            {
                string sEmailid = "", sCCList = "", sExtraMsg = "";

                DataSet dsDocument = objGlobalData.Getdetails("select branch, supplier, recommanded,approved,logged_by from t_supplier_reevaluation where id_reevaluation='" + id_reevaluation + "'");
                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                {
                    if (iStatus == 1)//approved 
                    {
                        if (dsDocument.Tables[0].Rows[0]["logged_by"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                        }

                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approved"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Supplier Reevaualtion has been Approved,Supplier: " + objGlobalData.GetSupplierNameById(dsDocument.Tables[0].Rows[0]["supplier"].ToString());
                    }

                    if (iStatus == 2)//Rejected 
                    {
                        if (dsDocument.Tables[0].Rows[0]["approved"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["approved"].ToString());
                        }

                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["logged_by"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Supplier Reevaluation has been Rejected, Supplier: " + objGlobalData.GetSupplierNameById(dsDocument.Tables[0].Rows[0]["supplier"].ToString());
                    }

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["recommanded"].ToString()), "Supplier_Reevaluation", sExtraMsg);

                    objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                    return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendReevaluationApprvRejectEmail: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunAddSupplierReevalutionTans(SupplierReevalutionModelsList ObjList)
        {
            try
            {
                if (ObjList.EvalList.Count > 0)
                {
                    string sSqlstmt = "delete from t_supplier_reevaluation_trans where id_reevaluation='"
                        + ObjList.EvalList[0].id_reevaluation + "'; ";
                    for (int i = 0; i < ObjList.EvalList.Count; i++)
                    {
                        string sdate_reevaluation = ObjList.EvalList[i].date_reevaluation.ToString("yyyy-MM-dd");
                        sSqlstmt = sSqlstmt + "insert into t_supplier_reevaluation_trans (id_reevaluation, date_reevaluation, cust_name," +
                            "complaints,description_complaint,ref_no_complaint "
                        + ") values('" + ObjList.EvalList[0].id_reevaluation + "','" + sdate_reevaluation + "','" + ObjList.EvalList[i].cust_name
                        + "','" + ObjList.EvalList[i].complaints + "','" + ObjList.EvalList[i].description_complaint + "','" + ObjList.EvalList[i].ref_no_complaint + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplierReevalutionTans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddSupplierReevalutionQuestTans(SupplierReevalutionModelsList ObjQuestList)
        {
            try
            {
                if (ObjQuestList.EvalList.Count > 0)
                {
                    string sSqlstmt = "delete from t_supplier_reevaluation_quest where id_reevaluation='"
                        + ObjQuestList.EvalList[0].id_reevaluation + "'; ";
                    for (int i = 0; i < ObjQuestList.EvalList.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_supplier_reevaluation_quest (id_reevaluation, SQId, SQ_OptionsId"
                        + ") values('" + ObjQuestList.EvalList[0].id_reevaluation + "','" + ObjQuestList.EvalList[i].SQId
                        + "','" + ObjQuestList.EvalList[i].SQ_OptionsId + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplierReevalutionQuestTans: " + ex.ToString());
            }
            return false;
        }
              
        internal bool FunUpdateSupplierReevalutionTrans(string sid_reevaluation)
        {
            try
            {
                string sSqlstmt = "delete from t_supplier_reevaluation_trans where id_reevaluation='" + sid_reevaluation + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierReevalutionTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteSupplierReevalution(string sid_reevaluation)
        {
            try
            {
                string sSqlstmt = "update t_supplier_reevaluation set Active=0 where id_reevaluation='" + sid_reevaluation + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSupplierReevalution: " + ex.ToString());

            }
            return false;
        }

        public string GetFunRatingId(string rating)
        {
            try
            {

                string sSqlstmt = "select SQ_OptionsId from t_surveyquestion_options t ,t_survey_type tt where t.Survey_TypeId =tt.Survey_TypeId and t.RatingOptions  = '" + rating + "' and tt.TypeName= 'Supplier Reevaluation'";
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
    public class SupplierReevalutionModelsList
    {
        public List<SupplierReevalutionModels> EvalList { get; set; }
    }
}