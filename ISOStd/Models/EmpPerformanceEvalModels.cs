using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class EmpPerformanceEvalModels
    {        
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string Performance_EvalId { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string emp_id { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Department")]
        public string Dept_id { get; set; }

        [Required]
        [Display(Name = "Evaluation Date")]
        public DateTime Evaluation_DoneOn { get; set; }

        [Required]
        [Display(Name = "Evaluation From")]
        public DateTime Evaluated_From { get; set; }

        [Required]
        [Display(Name = "Evaluation To")]
        public DateTime Evaluated_To { get; set; }

        [Required]
        [Display(Name = "Evaluation Done By")]
        public string Eval_DoneBy { get; set; }

        [Display(Name = "Designation")]
        public string Eval_DoneBy_Desig { get; set; }
       
        [Display(Name = "Department")]
        public string Eval_DoneBy_DeptId { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Improvement area")]
        public string Weakness { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Strength")]
        public string Strengths { get; set; }

        [Required]
        [Display(Name = "Need of trainings to enhance the competence")]
        public string Training_Reqd { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "If employee performance is not adequate, actions to be taken")]
        public string Actions_Taken { get; set; }

        [Required]
        [Display(Name = "Evaluation Reviewed By")]
        public string Eval_ReviewedBy { get; set; }

        [Display(Name = "Designation")]
        public string Eval_ReviewedBy_Desig { get; set; }

        [Display(Name = "Department")]
        public string Eval_ReviewedBy_DeptId { get; set; }

        [Display(Name = "Logged By")]
        public string LoggedBy { get; set; }

        [Display(Name = "Document")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Reviewed By")]
        public string JrMgr { get; set; }
        public string JrMgrId { get; set; }

        [Display(Name = "Reviewed By Comment")]
        public string Comment_JrMgr { get; set; }

        [Display(Name = "Reviewed By Comment Date")]
        public DateTime JrMgr_Comment_Date { get; set; }

        [Display(Name = "Approved By")]
        public string SrMgr { get; set; }
        public string SrMgrId { get; set; }

        [Display(Name = "Approved By Comment")]
        public string Comment_SrMgr { get; set; }

        [Display(Name = "Approved By Comment Date")]
        public DateTime SrMgr_Comment_Date { get; set; }

        public string Eval_DoneById { get; set; }
        public string sstatus { get; set; }

        //t_emp_performance_eval
        [Display(Name = "Need of trainings")]
        public string training_need { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Recommendation")]
        public string recommendation { get; set; }

        [Display(Name = "Notified to")]
        public string notified_to { get; set; }

        //t_emp_performance_training
        [Display(Name = "Id")]
        public string id_training { get; set; }

        [Display(Name = "Training Topic")]
        public string training_topic { get; set; }

        [Display(Name = "Criticality")]
        public string criticality { get; set; }

        [Display(Name = "Evaluation Period")]
        public string eval_period { get; set; }

        [Display(Name = "Category")]
        public string eval_category { get; set; }

        public string Weightage { get; set; }

        public DateTime Date_of_join { get; set; }

        [Display(Name = "Evaluation Status")]
        public string eval_status { get; set; }

        [Display(Name = "Top Mgmt")]
        public string top_mgmt { get; set; }

        internal bool FunDeletePerformanceDoc(string sPerformance_EvalId)
        {
            try
            {
                string sSqlstmt = "update t_emp_performance_eval set Active=0 where Performance_EvalId='" + sPerformance_EvalId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePerformanceDoc: " + ex.ToString());

            }
            return false;
        }

        internal bool FunAddEmpPerformanceEvaluation(EmpPerformanceEvalModels objEmpPerformanceEval, EmpPerformanceElementsModelsList objEmpPerformanceEeleList, EmpPerformanceEvalModelsList objModelList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";               
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string VarJrMgr = objGlobalData.GetHrEmpEvaluatedById(Eval_DoneBy);
                string VarSrMgr = "";
                if (VarJrMgr != "")
                {
                     VarSrMgr = objGlobalData.GetHrEmpEvaluatedById(VarJrMgr);
                }
                

                string sSqlstmt = "insert into t_emp_performance_eval (emp_id, Designation, Dept_id, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + " Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, LoggedBy,DocUploadPath,branch,JrMgr,SrMgr,training_need,remarks,recommendation,notified_to,eval_period,eval_category";

                if (objEmpPerformanceEval.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluation_DoneOn";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_From > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluated_From";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluated_From.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_To > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluated_To";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluated_To.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objEmpPerformanceEval.emp_id + "','" + objEmpPerformanceEval.Designation + "','" + objEmpPerformanceEval.Dept_id
                 + "','" + objEmpPerformanceEval.Eval_DoneBy + "','" + objEmpPerformanceEval.Eval_DoneBy_Desig
                 + "','" + objEmpPerformanceEval.Eval_DoneBy_DeptId + "','" + objEmpPerformanceEval.Weakness
                 + "','" + objEmpPerformanceEval.Strengths + "','" + objEmpPerformanceEval.Training_Reqd + "','" + objEmpPerformanceEval.Actions_Taken
                 + "','" + objEmpPerformanceEval.Eval_ReviewedBy + "','" + objEmpPerformanceEval.Eval_ReviewedBy_Desig + "','" + objEmpPerformanceEval.Eval_ReviewedBy_DeptId
                 + "','" + user + "','" + objEmpPerformanceEval.DocUploadPath + "','" + sBranch + "','" + VarJrMgr + "','" + VarSrMgr + "','" + training_need + "','" + remarks + "','" + recommendation + "','" + notified_to + "','" + eval_period + "','" + eval_category + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int iPerformance_EvalId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iPerformance_EvalId))
                {
                    if (objEmpPerformanceEeleList.lstEmpPerformanceElements.Count > 0)
                    {
                        EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();

                      
                        if (objEmpPerformanceEeleList.lstEmpPerformanceElements.Count > 0)
                        {
                            objEmpPerformanceEeleList.lstEmpPerformanceElements[0].Performance_EvalId = iPerformance_EvalId.ToString();

                            objElement.FunAddEmpPerformanceEvaluation(objEmpPerformanceEeleList);
                        }
                        if (objModelList.lstEmpPerformanceEvalModels.Count > 0)
                        {
                            objModelList.lstEmpPerformanceEvalModels[0].Performance_EvalId = iPerformance_EvalId.ToString();
                            FunAddTrainingList(objModelList);
                        }
                    }
                    SendJrMgrPerpEmail(iPerformance_EvalId, "Employee Performace Evaluation");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool SendJrMgrPerpEmail(int sPerformance_EvalId, string sMessage = "")
        {
            try
            {
                //string sPerformance_EvalId = "";
                //string PerfIdSql = "select max(Performance_EvalId) as Performance_EvalId from t_emp_performance_eval where Active=1";
                //DataSet dsPerfIdList = objGlobalData.Getdetails(PerfIdSql);
                //if (dsPerfIdList.Tables.Count > 0 && dsPerfIdList.Tables[0].Rows.Count > 0)
                //{
                //    sPerformance_EvalId = dsPerfIdList.Tables[0].Rows[0]["Performance_EvalId"].ToString();
                //}
                string sType = "EmpPerformace";                
                string sSqlstmt = "SELECT emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To,  Eval_DoneBy, Weakness,Strengths,DocUploadPath,JrMgr,LoggedBy"
                    + " from t_emp_performance_eval where  Performance_EvalId='" + sPerformance_EvalId + "'";

                DataSet dsEmpList = objGlobalData.Getdetails(sSqlstmt);
                EmpPerformanceElementsModels objModels = new EmpPerformanceElementsModels();

                if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["JrMgr"].ToString());
                    string sToEmailIds = "";

                    sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString());
                   
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    aAttachment = HttpContext.Current.Server.MapPath(dsEmpList.Tables[0].Rows[0]["DocUploadPath"].ToString());


                    sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                        + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["emp_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Designation"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation Done Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluated From Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_From"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation To Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_To"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evlaution Done By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString()) + "</td></tr>";
                        //+ "<tr><td colspan=3><b>Weakness:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Weakness"].ToString()) + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Strengths:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Strengths"].ToString()) + "</td></tr>";

                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee Performance Details");
                    sContent = sContent.Replace("{content}", sHeader );
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendJrMgrPerpEmail: " + ex.ToString());
            }
            return false;
        }
        
        internal bool FunUpdateEmpPerformanceEvaluation(EmpPerformanceEvalModels objEmpPerformanceEval, EmpPerformanceElementsModelsList objEmpPerformanceEeleList, EmpPerformanceEvalModelsList objModelList)
        {
            try
            {
                string sSqlstmt = "update t_emp_performance_eval set emp_id='" + objEmpPerformanceEval.emp_id + "', Designation='" + objEmpPerformanceEval.Designation
                    + "', Dept_id='" + objEmpPerformanceEval.Dept_id + "', Eval_DoneBy='" + objEmpPerformanceEval.Eval_DoneBy + "', Eval_DoneBy_Desig='" + objEmpPerformanceEval.Eval_DoneBy_Desig
                 + "', Eval_DoneBy_DeptId='" + objEmpPerformanceEval.Eval_DoneBy_DeptId + "', Weakness='" + objEmpPerformanceEval.Weakness
                 + "', Strengths='" + objEmpPerformanceEval.Strengths + "', Training_Reqd='" + objEmpPerformanceEval.Training_Reqd + "', Actions_Taken='" + objEmpPerformanceEval.Actions_Taken
                 + "', Eval_ReviewedBy='" + objEmpPerformanceEval.Eval_ReviewedBy + "', Eval_ReviewedBy_Desig='" + objEmpPerformanceEval.Eval_ReviewedBy_Desig
                 + "', Eval_ReviewedBy_DeptId='" + objEmpPerformanceEval.Eval_ReviewedBy_DeptId + "',training_need='" + training_need + "',remarks='" + remarks + "',recommendation='" + recommendation + "',notified_to='" + notified_to + "',eval_period='" + eval_period + "',eval_category='" + eval_category + "'";

                if (objEmpPerformanceEval.DocUploadPath != null)
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objEmpPerformanceEval.DocUploadPath + "' ";
                }

                if (objEmpPerformanceEval.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Evaluation_DoneOn='" + objEmpPerformanceEval.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_From > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Evaluated_From='" + objEmpPerformanceEval.Evaluated_From.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_To > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Evaluated_To='" + objEmpPerformanceEval.Evaluated_To.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + " where Performance_EvalId='" + objEmpPerformanceEval.Performance_EvalId + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                    objElement.FunAddEmpPerformanceEvaluation(objEmpPerformanceEeleList);

                   
                    FunAddTrainingList(objModelList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmpPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool FunUpdateEmpPerfManagerEvaluation(EmpPerformanceEvalModels objEmpPerformanceEval, EmpPerformanceElementsModelsList objEmpPerformanceEeleList, EmpPerformanceEvalModelsList objModelList)
        {
            try
            {
                string sSqlstmt = "update t_emp_performance_eval set  Weakness='" + objEmpPerformanceEval.Weakness
                 + "', Strengths='" + objEmpPerformanceEval.Strengths + "', Training_Reqd='" + objEmpPerformanceEval.Training_Reqd + "', Actions_Taken='" + objEmpPerformanceEval.Actions_Taken
                 + "', Eval_ReviewedBy='" + objEmpPerformanceEval.Eval_ReviewedBy + "', Eval_ReviewedBy_Desig='" + objEmpPerformanceEval.Eval_ReviewedBy_Desig
                 + "', Eval_ReviewedBy_DeptId='" + objEmpPerformanceEval.Eval_ReviewedBy_DeptId + "',training_need='" + training_need + "',remarks='" + remarks + "',recommendation='" + recommendation + "',notified_to='" + notified_to + "',eval_status='1'";

                if (objEmpPerformanceEval.DocUploadPath != null)
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objEmpPerformanceEval.DocUploadPath + "' ";
                }

                sSqlstmt = sSqlstmt + " where Performance_EvalId='" + objEmpPerformanceEval.Performance_EvalId + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();
                    objElement.FunAddEmpPerformanceEvaluation(objEmpPerformanceEeleList);


                    FunAddTrainingList(objModelList);

                    return SendMangerPerpEmail(Performance_EvalId, "Employee Performace Evaluation");
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmpPerfManagerEvaluation: " + ex.ToString());

            }
            return false;
        }


        internal bool SendMangerPerpEmail(string sPerformance_EvalId, string sMessage = "")
        {
            try
            {
                string sType = "EmpPerformace";
                string sSqlstmt = "SELECT emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To,  Eval_DoneBy, Weakness,Strengths,DocUploadPath,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,notified_to,LoggedBy"
                    + " from t_emp_performance_eval where  Performance_EvalId='" + sPerformance_EvalId + "'";

                DataSet dsEmpList = objGlobalData.Getdetails(sSqlstmt);
                EmpPerformanceElementsModels objModels = new EmpPerformanceElementsModels();

                if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["SrMgr"].ToString());
                    string sToEmailIds = "";
                   
                    sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["notified_to"].ToString()) + ",";
                    
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    string sCCEmailIds = objGlobalData.GetHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "," + objGlobalData.GetHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString());
                    aAttachment = HttpContext.Current.Server.MapPath(dsEmpList.Tables[0].Rows[0]["DocUploadPath"].ToString());


                    sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                        + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["emp_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Designation"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation Done Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluated From Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_From"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation To Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_To"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evlaution Done By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Weakness:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Weakness"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Strengths:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Strengths"].ToString()) + "</td></tr>"
                        + "<tr><br /></tr>"
                        //+ "<tr><td colspan=3><b>Evaluation Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["JrMgr"].ToString()) + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Evaluation Reviewed Comments:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Comment_JrMgr"].ToString()) + "</td></tr>"
                        //+ "<tr><td colspan=3><b>Evaluation Reviewed Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>";
                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                   // sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee Performance Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendSrMgrPerpEmail: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateEmpPerformanceComments(EmpPerformanceEvalModels objEmpPerformanceEval)
        {
            try
            {
                string sJrMgr_Comment_Date = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sSrMgr_Comment_Date = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_emp_performance_eval set ";

                if (objEmpPerformanceEval.Comment_JrMgr != null)
                {
                    sSqlstmt = sSqlstmt + " Comment_JrMgr='" + objEmpPerformanceEval.Comment_JrMgr + "', JrMgr_Comment_Date='" + sJrMgr_Comment_Date + "',top_mgmt='" + top_mgmt + "',eval_status='2' ";
                }

                if (objEmpPerformanceEval.Comment_SrMgr != null)
                {
                    sSqlstmt = sSqlstmt + " Comment_SrMgr='" + objEmpPerformanceEval.Comment_SrMgr + "', SrMgr_Comment_Date='" + sSrMgr_Comment_Date + "',eval_status='3' ";
                }

                sSqlstmt = sSqlstmt + " where Performance_EvalId='" + objEmpPerformanceEval.Performance_EvalId + "'";

                return (objGlobalData.ExecuteQuery(sSqlstmt));
               
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmpPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool SendSrMgrPerpEmail(string sPerformance_EvalId,string sMessage = "")
        {
            try
            {               
                string sType = "EmpPerformace";
                string sSqlstmt = "SELECT emp_id, Designation, Dept_id, Evaluation_DoneOn, Evaluated_From, Evaluated_To,  Eval_DoneBy, Weakness,Strengths,DocUploadPath,JrMgr,Comment_JrMgr,JrMgr_Comment_Date,SrMgr,top_mgmt,LoggedBy,Comment_SrMgr,SrMgr_Comment_Date,eval_status"
                    + " from t_emp_performance_eval where  Performance_EvalId='" + sPerformance_EvalId + "'";

                DataSet dsEmpList = objGlobalData.Getdetails(sSqlstmt);
                EmpPerformanceElementsModels objModels = new EmpPerformanceElementsModels();

                if (dsEmpList.Tables.Count > 0 && dsEmpList.Tables[0].Rows.Count > 0)
                {
                    string sHeader, sSubject = "", sContent = "", aAttachment = "";

                    //using streamreader for reading my htmltemplate 
                    //Form the Email Subject and Body content
                    DataSet dsEmailXML = new DataSet();
                    dsEmailXML.ReadXml(HttpContext.Current.Server.MapPath("~/EmailTemplates.xml"));

                    if (sType != "" && dsEmailXML.Tables.Count > 0 && dsEmailXML.Tables[sType] != null && dsEmailXML.Tables[sType].Rows.Count > 0)
                    {
                        sSubject = dsEmailXML.Tables[sType].Rows[0]["subject"].ToString();
                    }

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Views/EmailTemplate/EmailTemplate.html")))
                    {
                        sContent = reader.ReadToEnd();
                    }
                    //string sName = objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["SrMgr"].ToString());
                    string sToEmailIds = "";
                   
                    sToEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["top_mgmt"].ToString()) + ",";
                    
                    sToEmailIds = Regex.Replace(sToEmailIds, ",+", ",");
                    sToEmailIds = sToEmailIds.Trim();
                    sToEmailIds = sToEmailIds.TrimEnd(',');
                    sToEmailIds = sToEmailIds.TrimStart(',');
                    string sCCEmailIds = objGlobalData.GetMultiHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString()) + "," + objGlobalData.GetHrEmpEmailIdById(dsEmpList.Tables[0].Rows[0]["LoggedBy"].ToString());
                    aAttachment = HttpContext.Current.Server.MapPath(dsEmpList.Tables[0].Rows[0]["DocUploadPath"].ToString());


                    sHeader = "<tr><td colspan=3><b>Employee Name:<b></td> <td colspan=3>"
                        + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["emp_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Designation:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Designation"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Department:<b></td> <td colspan=3>" + objGlobalData.GetDeptNameById(dsEmpList.Tables[0].Rows[0]["Dept_id"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation Done Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluation_DoneOn"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluated From Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_From"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evaluation To Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["Evaluated_To"].ToString()).ToString("dd/MM/yyyy")
                        + "</td></tr>"
                        + "<tr><td colspan=3><b>Evlaution Done By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["Eval_DoneBy"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Weakness:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Weakness"].ToString()) + "</td></tr>"
                        + "<tr><td colspan=3><b>Strengths:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Strengths"].ToString()) + "</td></tr>"
                        + "<tr><br /></tr>";
                    if(dsEmpList.Tables[0].Rows[0]["eval_status"].ToString() == "2")
                    {
                        sHeader= sHeader + "<tr><td colspan=3><b>Evaluation Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["LoggedBy"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Reviewed Comments:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Comment_JrMgr"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Reviewed Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["JrMgr_Comment_Date"].ToString()).ToString("dd/MM/yyyy")
                       + "</td></tr>";
                    }
                    else
                    {
                        sHeader= sHeader + "<tr><td colspan=3><b>Evaluation Reviewed By:<b></td> <td colspan=3>" + objGlobalData.GetMultiHrEmpNameById(dsEmpList.Tables[0].Rows[0]["top_mgmt"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Reviewed Comments:<b></td> <td colspan=3>" + (dsEmpList.Tables[0].Rows[0]["Comment_SrMgr"].ToString()) + "</td></tr>"
                       + "<tr><td colspan=3><b>Evaluation Reviewed Date:<b></td> <td colspan=3>" + Convert.ToDateTime(dsEmpList.Tables[0].Rows[0]["SrMgr_Comment_Date"].ToString()).ToString("dd/MM/yyyy")
                       + "</td></tr>";
                    }
                       
                    if (File.Exists(aAttachment))
                    {
                        sHeader = sHeader + "<tr><td colspan=3><b>Document Upload:<b></td> <td colspan=3>Please find the attachment</td></tr>";
                    }

                    sContent = sContent.Replace("{FromMsg}", "");
                    //sContent = sContent.Replace("{UserName}", sName);
                    sContent = sContent.Replace("{Title}", "Employee Performance Details");
                    sContent = sContent.Replace("{content}", sHeader);
                    sContent = sContent.Replace("{message}", "");
                    sContent = sContent.Replace("{extramessage}", "");

                    sToEmailIds = sToEmailIds.Trim(',');


                    objGlobalData.Sendmail(sToEmailIds, sSubject, sContent, aAttachment, sCCEmailIds, "");
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in SendSrMgrPerpEmail: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddTrainingList(EmpPerformanceEvalModelsList objModelsList)
        {
            try
            {
                string sSqlstmt = "delete from t_emp_performance_training where Performance_EvalId='" + objModelsList.lstEmpPerformanceEvalModels[0].Performance_EvalId + "'; ";

                for (int i = 0; i < objModelsList.lstEmpPerformanceEvalModels.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_emp_performance_training(Performance_EvalId,training_topic,criticality";

                    string sFieldValue = "", sFields = "";
                    
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + objModelsList.lstEmpPerformanceEvalModels[0].Performance_EvalId + "', '" + objModelsList.lstEmpPerformanceEvalModels[i].training_topic + "', '" + objModelsList.lstEmpPerformanceEvalModels[i].criticality + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddTrainingList: " + ex.ToString());
            }
            return false;
        }
    }


    public class EmpPerformanceElementsModels
    {
        clsGlobal objGlobalData = new clsGlobal();
        
        //[Required]
        [Display(Name = "Id")]
        public string Performance_Id { get; set; }

        //[Required]
        [Display(Name = "Evaluation Id")]
        public string Performance_EvalId { get; set; }

        //[Required]
        //[Display(Name = "Element")]
        //public string SQId { get; set; }

        //[Display(Name = "Rating")]
        //public string SQ_OptionsId { get; set; }

        //public string SQ_Weightage { get; set; }

        //t_emp_performance_eval_questions
        [Display(Name = "Question Id")]
        public string SQId { get; set; }

        [Display(Name = "Section")]
        public string Section { get; set; }

        [Display(Name = "Questions")]
        public string Questions { get; set; }

        [Display(Name = "Evaluation Period")]
        public string eval_period { get; set; }

        [Display(Name = "Category")]
        public string eval_category { get; set; }

        //t_emp_performance_eval_options

        [Display(Name = "Rating Id")]
        public string SQ_OptionsId { get; set; }

        [Required]
        [Display(Name = "Rating Options")]
        public string RatingOptions { get; set; }

        [Required]
        [Display(Name = "Weightage")]
        public string Weightage { get; set; }

        public string SQ_Weightage { get; set; }
        public string Section_Weightage { get; set; }


        public string GetMaxRatingWeightage()
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select max(Weightage) Weightage from t_emp_performance_eval_rating");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Weightage"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMaxRatingWeightage: " + ex.ToString());
            }
            return "";
        }

        //NameByIds

        public string GetRatingNameById(string SQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select RatingOptions from t_emp_performance_eval_rating where SQ_OptionsId='" + SQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["RatingOptions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRatingNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetRatingWeightageById(string SQ_OptionsId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("select Weightage from t_emp_performance_eval_rating where SQ_OptionsId='" + SQ_OptionsId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Weightage"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetRatingWeightageById: " + ex.ToString());
            }
            return "";
        }

        public string GetQuestionNameById(string SQId)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Questions FROM t_emp_performance_eval_questions where SQId='" + SQId + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Questions"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetQuestionNameById: " + ex.ToString());
            }
            return "";
        }


        //Section
        public MultiSelectList GetSectionsList()
        {
            string sbranch = objGlobalData.GetCurrentUserSession().division;

            EmpPerformanceElementsModelsList Typelist = new EmpPerformanceElementsModelsList();
            Typelist.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

            try
            {
                string sSqlstmt = "SELECT SQId, Section FROM t_emp_performance_eval_questions where branch = '" + sbranch + "' and active=1 order by Section,SQId asc";

                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        EmpPerformanceElementsModels data = new EmpPerformanceElementsModels()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Section = objGlobalData.GetDropdownitemById(dsEmp.Tables[0].Rows[i]["Section"].ToString())
                        };

                        Typelist.lstEmpPerformanceElements.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSectionsList: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstEmpPerformanceElements, "SQId", "Section");
        }


        //Questions

        public MultiSelectList GetQuestionWithSectionList(string section,string branch, string eval_period, string eval_category)
        {
            EmpPerformanceElementsModelsList Typelist = new EmpPerformanceElementsModelsList();
            Typelist.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();
            string sSqlstmt = "";
            try
            {
                if(section != ""  && eval_period == "" && eval_category == "")
                {
                     sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions where Section='" + section + "' and branch = '" + branch + "' and active=1";

                }
                if (section != "" &&  eval_period != "" && eval_category == "")
                {
                     sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions where Section='" + section + "' and branch = '" + branch + "' and eval_period = '" + eval_period + "'  and active=1";

                }
                if (section != "" && eval_period == "" && eval_category != "")
                {
                     sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions where Section='" + section + "' and branch = '" + branch + "' and eval_category = '" + eval_category + "'  and active=1";

                }
                if (section != "" && eval_period != "" && eval_category != "")
                {
                     sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions where Section='" + section + "' and branch = '" + branch + "' and  eval_period = '" + eval_period + "' and eval_category = '" + eval_category + "'  and active=1";

                }
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        EmpPerformanceElementsModels data = new EmpPerformanceElementsModels()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString(),
                          
                        };

                        Typelist.lstEmpPerformanceElements.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetQuestionWithSectionList: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstEmpPerformanceElements, "SQId", "Questions");
        }

        public MultiSelectList GetQuestionList(string branch)
        {
            EmpPerformanceElementsModelsList Typelist = new EmpPerformanceElementsModelsList();
            Typelist.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

            try
            {              
                string sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions where branch = '" + branch + "' and active=1 order by Section,SQId asc";

                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        EmpPerformanceElementsModels data = new EmpPerformanceElementsModels()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString(),
                           
                        };

                        Typelist.lstEmpPerformanceElements.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetQuestionList: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstEmpPerformanceElements, "SQId", "Questions");
        }

        //public MultiSelectList GetQuestionListBox(string eval_period,string eval_category)
        //{
        //    EmpPerformanceElementsModelsList Typelist = new EmpPerformanceElementsModelsList();
        //    Typelist.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

        //    try
        //    {
        //        string sbranch = objGlobalData.GetCurrentUserSession().division;

        //        string sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions  where branch = '" + sbranch + "' and eval_period = '" + eval_period + "' and eval_category = '" + eval_category + "' and active=1 order by Section,SQId asc";

        //        DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
        //        if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
        //            {
        //                EmpPerformanceElementsModels data = new EmpPerformanceElementsModels()
        //                {
        //                    SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
        //                    Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString()
        //                };

        //                Typelist.lstEmpPerformanceElements.Add(data);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in GetQuestionListBox: " + ex.ToString());
        //    }
        //    return new MultiSelectList(Typelist.lstEmpPerformanceElements, "SQId", "Questions");
        //}

        public MultiSelectList GetQuestionListBox(string eval_period,string eval_category)
        {
            EmpPerformanceElementsModelsList Typelist = new EmpPerformanceElementsModelsList();
            Typelist.lstEmpPerformanceElements = new List<EmpPerformanceElementsModels>();

            try
            {
                string sbranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "SELECT SQId, Questions FROM t_emp_performance_eval_questions  where branch = '" + sbranch + "' and eval_period='"+ eval_period + "' and eval_category='"+ eval_category + "' and active=1 order by Section,SQId asc";

                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        EmpPerformanceElementsModels data = new EmpPerformanceElementsModels()
                        {
                            SQId = dsEmp.Tables[0].Rows[i]["SQId"].ToString(),
                            Questions = dsEmp.Tables[0].Rows[i]["Questions"].ToString()
                        };

                        Typelist.lstEmpPerformanceElements.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetQuestionListBox: " + ex.ToString());
            }
            return new MultiSelectList(Typelist.lstEmpPerformanceElements, "SQId", "Questions");
        }

        internal bool FunAddPerformanceQuestions(EmpPerformanceElementsModels objModels,string sBranch)
        {
            try
            {
                //string sBranch = objGlobalData.GetCurrentUserSession().Work_Location;

                string sSqlstmt = "insert into t_emp_performance_eval_questions (Section,Questions,branch,eval_period,eval_category) values('" + objModels.Section + "','"  + objModels.Questions + "','" + sBranch + "','" + objModels.eval_period + "','" + objModels.eval_category + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPerformanceQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePerformanceQuestions(EmpPerformanceElementsModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_emp_performance_eval_questions set Questions='" + objModel.Questions + "' where SQId='" + objModel.SQId + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePerformanceQuestions: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeletePerformaceQuestions(string sSQId)
        {
            try
            {
                string sSqlstmt = "Update t_emp_performance_eval_questions set Active=0 where SQId='" + sSQId + "' ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePerformaceQuestions: " + ex.ToString());
            }
            return false;
        }


        //Rating

        public DataSet PerformanceRating()
        {
            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select SQ_OptionsId, RatingOptions, Weightage from t_emp_performance_eval_rating where Active=1 order by Weightage desc");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in PerformanceRating: " + ex.ToString());
            }
            return dsRating;
        }

        internal bool FunAddPerformanceRatings(EmpPerformanceElementsModels objModel)
        {
            try
            {
                string sSqlstmt = "insert into t_emp_performance_eval_rating (RatingOptions, Weightage) values('" + objModel.RatingOptions + "','" + objModel.Weightage + "')";
                return (objGlobalData.ExecuteQuery(sSqlstmt));                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPerformanceRatings: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdatePerformanceRating(EmpPerformanceElementsModels objModel)
        {
            try
            {
                string sSqlstmt = "update t_emp_performance_eval_rating set RatingOptions='" + objModel.RatingOptions + "',Weightage='" + objModel.Weightage + "' where SQ_OptionsId='" + objModel.SQ_OptionsId + "' ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdatePerformanceRating: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeletePerformaceRatings(string sSQ_OptionsId)
        {
            try
            {
                string sSqlstmt = "Update t_emp_performance_eval_rating set Active=0 where SQ_OptionsId='" + sSQ_OptionsId + "' ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeletePerformaceRatings: " + ex.ToString());
            }
            return false;
        }

        //Employee Performance Evalution

        internal bool FunAddEmpPerformanceEvaluation(EmpPerformanceEvalModels objEmpPerformanceEval, EmpPerformanceElementsModelsList objEmpPerformanceEeleList)
        {
            try
            {
                string sColumn = "", sValues = "";
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string VarJrMgr = objGlobalData.GetHrEmpEvaluatedById(user);
                string VarSrMgr = "";
                if (VarJrMgr != "")
                {
                    VarSrMgr = objGlobalData.GetHrEmpEvaluatedById(VarJrMgr);
                }


                string sSqlstmt = "insert into t_emp_performance_eval (emp_id, Designation, Dept_id, Eval_DoneBy, Eval_DoneBy_Desig,"
                    + "Eval_DoneBy_DeptId, Weakness, Strengths, Training_Reqd, Actions_Taken, Eval_ReviewedBy, Eval_ReviewedBy_Desig, Eval_ReviewedBy_DeptId, LoggedBy,DocUploadPath,branch,JrMgr,SrMgr";

                if (objEmpPerformanceEval.Evaluation_DoneOn > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluation_DoneOn";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluation_DoneOn.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_From > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluated_From";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluated_From.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                if (objEmpPerformanceEval.Evaluated_To > Convert.ToDateTime("01/01/0001"))
                {
                    sColumn = sColumn + ", Evaluated_To";
                    sValues = sValues + ", '" + objEmpPerformanceEval.Evaluated_To.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }

                sSqlstmt = sSqlstmt + sColumn + ") values('" + objEmpPerformanceEval.emp_id + "','" + objEmpPerformanceEval.Designation + "','" + objEmpPerformanceEval.Dept_id
                 + "','" + objEmpPerformanceEval.Eval_DoneBy + "','" + objEmpPerformanceEval.Eval_DoneBy_Desig
                 + "','" + objEmpPerformanceEval.Eval_DoneBy_DeptId + "','" + objEmpPerformanceEval.Weakness
                 + "','" + objEmpPerformanceEval.Strengths + "','" + objEmpPerformanceEval.Training_Reqd + "','" + objEmpPerformanceEval.Actions_Taken
                 + "','" + objEmpPerformanceEval.Eval_ReviewedBy + "','" + objEmpPerformanceEval.Eval_ReviewedBy_Desig + "','" + objEmpPerformanceEval.Eval_ReviewedBy_DeptId
                 + "','" + user + "','" + objEmpPerformanceEval.DocUploadPath + "','" + sBranch + "','" + VarJrMgr + "','" + VarSrMgr + "'";

                sSqlstmt = sSqlstmt + sValues + ")";

                int iPerformance_EvalId;

                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iPerformance_EvalId))
                {
                    EmpPerformanceElementsModels objElement = new EmpPerformanceElementsModels();

                    objEmpPerformanceEeleList.lstEmpPerformanceElements[0].Performance_EvalId = iPerformance_EvalId.ToString();
                    objElement.FunAddEmpPerformanceEvaluation(objEmpPerformanceEeleList);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpPerformanceEvaluation: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddEmpPerformanceEvaluation(EmpPerformanceElementsModelsList objEmpPerformanceEeleList)
        {
            try
            {
                if (objEmpPerformanceEeleList.lstEmpPerformanceElements.Count > 0)
                {
                    string sSqlstmt = "SET SQL_SAFE_UPDATES=0;delete from t_emp_performance_elements where Performance_EvalId='"
                        + objEmpPerformanceEeleList.lstEmpPerformanceElements[0].Performance_EvalId + "'; ";
                    for (int i = 0; i < objEmpPerformanceEeleList.lstEmpPerformanceElements.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_emp_performance_elements (Performance_EvalId, SQId, SQ_OptionsId"
                        + ") values('" + objEmpPerformanceEeleList.lstEmpPerformanceElements[0].Performance_EvalId + "','" + objEmpPerformanceEeleList.lstEmpPerformanceElements[i].SQId
                        + "','" + objEmpPerformanceEeleList.lstEmpPerformanceElements[i].SQ_OptionsId + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddEmpPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

        internal bool FunUpdateEmpPerformanceEvaluation(EmpPerformanceElementsModelsList objEmpPerformanceEeleList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objEmpPerformanceEeleList.lstEmpPerformanceElements.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "update t_emp_performance_elements set"
                    + " SQId='" + objEmpPerformanceEeleList.lstEmpPerformanceElements[i].SQId
                    + "', SQ_OptionsId='" + objEmpPerformanceEeleList.lstEmpPerformanceElements[i].SQ_OptionsId
                    + "' where Performance_Id='" + objEmpPerformanceEeleList.lstEmpPerformanceElements[i].Performance_Id + "'; ";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateEmpPerformanceEvaluation: " + ex.ToString());

            }
            return false;
        }

    }

    public class EmpPerformanceEvalModelsList
    {
        public List<EmpPerformanceEvalModels> lstEmpPerformanceEvalModels { get; set; }
    }


    public class EmpPerformanceElementsModelsList
    {
        public List<EmpPerformanceElementsModels> lstEmpPerformanceElements { get; set; }
    }
}