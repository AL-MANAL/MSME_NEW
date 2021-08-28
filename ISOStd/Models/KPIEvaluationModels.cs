using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class KPIEvaluationModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        public int KPI_Id { get; set; }

        public int KPI_Trans_Id { get; set; }
        public int KPI_Trans_Eval_Id { get; set; }

        
        [Display(Name = "Department/Process")]
        public string DeptId { get; set; }

        [Display(Name = "Person Responsible")]
        public string Person_Responsible { get; set; }

        [Required]
        [Display(Name = "KPI Reference")]
        [System.Web.Mvc.Remote("doesObjRefExist", "KPIEvaluation", HttpMethod = "POST", ErrorMessage = "KPI reference already exists. Please enter a different Ref.")]
        public string KPI_Ref { get; set; }

        
        [Display(Name = "Frequency of Evaluation")]
        public string Freq_of_Eval { get; set; }

        
        [Display(Name = "Established On")]
        public DateTime EstablishedOn { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "KPI Desc.")]
        public string Process_Indicator { get; set; }
        
        [Display(Name = "Target Value")]
        public string Measurable_Value { get; set; }
       
        [Display(Name = "Monitoring Mechanism")]
        public string Monitoring_Mechanism { get; set; }
        
        [Display(Name = "Actual Value")]
        public string Measured_Value { get; set; }
       
        [Display(Name = "Evaluation Status")]
        public string Eval_Status { get; set; }
        
        [Display(Name = "Evaluated On")]
        public DateTime Eval_On { get; set; }
       
        [Display(Name = "NCR Ref.")]
        public string NCRRef { get; set; }
        public string EstYear { get; set; }

        [Display(Name = "Evidence")]
        public string upload { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Target Date")]
        public DateTime target_date { get; set; }

        internal bool FunDeleteKPIEvalDoc(string sKPI_Id)
        {
            try
            {
                string sSqlstmt = "update t_kpi set Active=0 where KPI_Id='" + sKPI_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteKPIEvalDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPI(KPIEvaluationModels objKPIEvaluationModels, KPIEvaluationModelsList objKPIEvaluationModelsList)
        {
            try
            {
                string sBranch = objGlobalData.GetCurrentUserSession().division;
    
                string sSqlstmt = "insert into t_kpi (DeptId, Person_Responsible, Freq_of_Eval,branch,Location,KPI_Ref)"
                + " values('" + objKPIEvaluationModels.DeptId + "','" + objKPIEvaluationModels.Person_Responsible
                + "','" + objKPIEvaluationModels.Freq_of_Eval + "','" + objKPIEvaluationModels.branch + "','" + objKPIEvaluationModels.Location + "','" + objKPIEvaluationModels.KPI_Ref + "')";

                //return FunAddKPIPlanTrans(objKPIEvaluationModelsList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));

                int KPI_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out KPI_Id))
                {
                    if (FunAddKPIPlanTrans(objKPIEvaluationModelsList, KPI_Id))
                    {
                        //    DataSet dsData = objGlobalData.GetReportNo("KPI", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                        //    if (dsData != null && dsData.Tables.Count > 0)
                        //    {
                        //        KPI_Ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        //    }
                        //    string sql1 = "update t_kpi set KPI_Ref='" + KPI_Ref + "' where KPI_Id='" + KPI_Id + "'";
                        //    return (objGlobalData.ExecuteQuery(sql1));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPIPlanTrans(KPIEvaluationModelsList objKPIEvaluationModelsList, int KPI_Id)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objKPIEvaluationModelsList.lstKPIEvaluation.Count; i++)
                {
                    if (objKPIEvaluationModelsList.lstKPIEvaluation[i].Process_Indicator != null)
                    {
                        string sObj_Estld_OnDate = objKPIEvaluationModelsList.lstKPIEvaluation[i].EstablishedOn.ToString("yyyy-MM-dd HH':'mm':'ss");
                        string starget_date = objKPIEvaluationModelsList.lstKPIEvaluation[i].target_date.ToString("yyyy-MM-dd");

                        sSqlstmt = sSqlstmt + "insert into t_kpi_trans (KPI_Id, EstablishedOn, Process_Indicator, Measurable_Value, Monitoring_Mechanism,Freq_of_Eval,target_date)"
                            + " values('" + KPI_Id + "','" + sObj_Estld_OnDate + "','" + objKPIEvaluationModelsList.lstKPIEvaluation[i].Process_Indicator
                        + "','" + objKPIEvaluationModelsList.lstKPIEvaluation[i].Measurable_Value + "','" + objKPIEvaluationModelsList.lstKPIEvaluation[i].Monitoring_Mechanism
                         + "','" + objKPIEvaluationModelsList.lstKPIEvaluation[i].Freq_of_Eval + "','" + starget_date + "');";
                    }
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPIPlanTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddKPIPlanEvaluation(KPIEvaluationModels objKPIEvaluationModels)
        {
            try
            {
                string sObj_Eval_OnDate = objKPIEvaluationModels.Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_kpi_trans_evaluation (KPI_Trans_Id, Measured_Value, Eval_Status, Eval_On, NCRRef,upload)"
                + " values('" + objKPIEvaluationModels.KPI_Trans_Id + "','" + objKPIEvaluationModels.Measured_Value + "','" + objKPIEvaluationModels.Eval_Status
                + "','" + sObj_Eval_OnDate + "','" + objKPIEvaluationModels.NCRRef + "','" + objKPIEvaluationModels.upload + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddKPIPlanEvaluation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPI(KPIEvaluationModels objKPIEvaluationModels)
        {
            try
            {
                string sSqlstmt = "update t_kpi set DeptId='" + objKPIEvaluationModels.DeptId + "', "
                    + "Freq_of_Eval='" + objKPIEvaluationModels.Freq_of_Eval + "', Person_Responsible='" + objKPIEvaluationModels.Person_Responsible
                    + "', branch='" + objKPIEvaluationModels.branch + "', Location='" + objKPIEvaluationModels.Location + "', KPI_Ref='" + objKPIEvaluationModels.KPI_Ref
                    + "' where KPI_Id='" + objKPIEvaluationModels.KPI_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPI: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPIPlan(KPIEvaluationModels objKPIEvaluationModels)
        {
            try
            {
                string sObj_Estld_OnDate = objKPIEvaluationModels.EstablishedOn.ToString("yyyy-MM-dd HH':'mm':'ss");
                string starget_date = objKPIEvaluationModels.target_date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_kpi_trans set EstablishedOn ='" + sObj_Estld_OnDate + "', target_date='" + starget_date + "', Process_Indicator='" + objKPIEvaluationModels.Process_Indicator
                    + "', Measurable_Value='" + objKPIEvaluationModels.Measurable_Value + "', Monitoring_Mechanism='" + objKPIEvaluationModels.Monitoring_Mechanism + "'";

                sSqlstmt = sSqlstmt + " where KPI_Trans_Id='" + objKPIEvaluationModels.KPI_Trans_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPIPlan: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateKPIPlanEvaluation(KPIEvaluationModels objKPIEvaluationModels)
        {
            try
            {
                string sObj_Eval_OnDate = objKPIEvaluationModels.Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_kpi_trans_evaluation set Measured_Value='" + objKPIEvaluationModels.Measured_Value
                    + "', Eval_Status='" + objKPIEvaluationModels.Eval_Status + "', Eval_On='" + sObj_Eval_OnDate + "', NCRRef='" + objKPIEvaluationModels.NCRRef
                    + "',upload='" + objKPIEvaluationModels .upload+ "' where KPI_Trans_Eval_Id='" + objKPIEvaluationModels.KPI_Trans_Eval_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateKPIPlanEvaluation: " + ex.ToString());
            }
            return false;
        }

        public bool checkObjRefExists(string KPI_Ref)
        {
            try
            {
                string sSqlstmt = "select KPI_Id from t_kpi where KPI_Ref='" + KPI_Ref + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkObjRefExists: " + ex.ToString());
            }
            return true;
        }

        public string GetFrequencyNameById(string sFreq)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='KPI Frequency Evaluation' and (item_id='" + sFreq + "' or item_desc='" + sFreq + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFrequencyNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetFrequencyList()
        {
            DropdownRiskList lstImpact = new DropdownRiskList();
            lstImpact.lstMitigationimpact = new List<DropdownRiskItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Frequency Evaluation' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownRiskItems reg = new DropdownRiskItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lstImpact.lstMitigationimpact.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFrequencyList: " + ex.ToString());
            }

            return new MultiSelectList(lstImpact.lstMitigationimpact, "Id", "Name");
        }
        internal string GetEvaluationNameById(string sEval)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                     + "and header_desc='KPI Evaluation Status' and (item_id='" + sEval + "' or item_desc='" + sEval + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEvaluationNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetEvaluationList()
        {
            DropdownKPIList lst = new DropdownKPIList();
            lst.lst = new List<DropdownKPIItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='KPI Evaluation Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownKPIItems reg = new DropdownKPIItems()
                        {
                            Id = dsEmp.Tables[0].Rows[i]["Id"].ToString(),
                            Name = dsEmp.Tables[0].Rows[i]["Name"].ToString()
                        };

                        lst.lst.Add(reg);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetEvaluationList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

    }

    public class KPIEvaluationModelsList
    {
        public List<KPIEvaluationModels> lstKPIEvaluation { get; set; }
    }

    public class DropdownKPIItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownKPIList
    {
        public List<DropdownKPIItems> lst { get; set; }
    }
}