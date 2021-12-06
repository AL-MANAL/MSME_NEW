using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace ISOStd.Models
{

    public class ObjectivesModels
    {
        clsGlobal objGlobalData = new clsGlobal();
      
        [Display(Name = "Id")]
        public int Objectives_Id { get; set; }
    
        public int ObjectivesTrans_Id { get; set; }

        public int ObjectivesEval_Id { get; set; }
      
        [Display(Name = "Department")]
        public string Dept { get; set; }
     
        [Display(Name = "Department Head")]
        public string Dept_Head { get; set; }
     
        [Display(Name = "Objective Reference")]
        [System.Web.Mvc.Remote("doesObjRefExist", "Objectives", HttpMethod = "POST", ErrorMessage = "Objectives reference already exists. Please enter a different Ref.")]
        public string Obj_Ref { get; set; }
       
        [Display(Name = "Established On")]
        public DateTime Obj_Estld_On { get; set; }
      
        [Display(Name = "Frequency of Evaluation")]
        public string Freq_of_Eval { get; set; }
      
        [Display(Name = "Established by")]
        public string Estld_by { get; set; }
       
        [Display(Name = "To be approved by")]
        public string Approved_By { get; set; }
        public string Approved_ById { get; set; }

        [Display(Name = "Objective Description")]
        [DataType(DataType.MultilineText)]
        public string Objectives_val { get; set; }
      
        [Display(Name = "Base Line Value")]
        public string Base_Line_Value { get; set; }
      
        [Display(Name = "Target Value")]
        public string Obj_Target { get; set; }

        [Display(Name = "Target Date to Achieve")]
        public DateTime Target_Date { get; set; }
      
        //[Display(Name = "Plan to achieve the Objective")]
        [Display(Name = "File Upload")]
        [DataType(DataType.MultilineText)]
        public string Action_Plan { get; set; }
       
        [Display(Name = "Monitoring Parameters")]
        public string Monitoring_Mechanism { get; set; }
      
        [Display(Name = "Evaluation Date")]
        public DateTime Obj_Eval_On { get; set; }
        
        [Display(Name = "Measured Value")]
        public string Obj_Achieved_Val { get; set; }
     
        [Display(Name = "Overall Objective Status")]
        public string Status_Obj_Eval { get; set; }
     
        [Display(Name = "NCR Ref. (If Objective not achieved)")]
        public string NCR_Ref { get; set; }
       
        [Display(Name = "Objective Status for the period")]
        public string Trend { get; set; }

        [Display(Name = "Upload")]
        public string Evidence { get; set; }
       
        [Display(Name = "Evaluation From Period")]
        public DateTime FromPeriod { get; set; }
        
        [Display(Name = "Evaluation To Period")]
        public DateTime ToPeriod { get; set; }

        [Display(Name = "Source of Data")]
        public string Source_data { get; set; }

        [Display(Name = "Method of Evaluation")]
        public string Method_eval { get; set; }

        [Display(Name = "Remarks")]
        public string Remark { get; set; }

        [Display(Name = "Notified to")]
        public string Notified_to { get; set; }


        [Display(Name = "Responsible Person")]
        public string Personal_Responsible { get; set; }
        //[Display(Name = "Responsible Person Edit check")]
        public string Personal_Responsible_EditCheck { get; set; }
        //[Display(Name = "Responsible Person Edit check 2")]
        public string Personal_Responsible_EditCheck2 { get; set; }

        [Display(Name = "Mgmt System Std(s)")]
        public string Audit_Criteria { get; set; }
       
        [Display(Name = "Approved Date")]
        public DateTime ApprovedDate { get; set; }
       
        [Display(Name = "Approved Status")]
        public string Approved_Status { get; set; }
       
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        
        [Display(Name = "Master Action Plan")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Objective Level")]
        public string objective_level { get; set; }
        public string EstYear { get; set; }
        
        //Action Plan
        [Display(Name = "Id")]
        public string id_objective_action { get; set; }

        [Display(Name = "Action to be taken")]
        public string actionplan { get; set; }

        [Display(Name = "Planned On")]
        public DateTime begin_date { get; set; }

        [Display(Name = "Target completion date")]
        public DateTime end_date { get; set; }

        [Display(Name = "File Upload")]
        public string upload { get; set; }

        [Display(Name = "Resource Required")]
        public string resource { get; set; }      

        [Display(Name = "Personnel Responsible")]
        public string resp_person { get; set; }

        [Display(Name = "Action Status")]
        public string action_status { get; set; }

        [Display(Name = "Reasons for not completing")]
        public string reason_notcomplete { get; set; }

        [Display(Name = "Status Updated On")]
        public DateTime status_updated_on { get; set; }


        // new fields
        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Accepted Value")]
        public string Accepted_Value { get; set; }

        [Display(Name = "Risk, if not achieved")]
        public string Risk_ifObjFails { get; set; }

        public string Approved_Status_id { get; set; }

        public List<ObjectivesModels> ObjectivesMList { get; set; }
        public List<ObjectivesModels> ObjectivesEvalList { get; set; }
        //New fileds
        [Display(Name = "Objective Inline With")]
        public string obj_inline { get; set; }

        [Display(Name = "Unit of measurement")]
        public string unit { get; set; }

        [Display(Name = "Source of Baseline Data")]
        public string baseline_data { get; set; }

        [Display(Name ="Ref. Action Plan")]
        public string action_ref_no { get; set; }
        public string branch_view { get; set; }
        public string id_potential { get; set; }

        [Display(Name = "Potential Cause")]
        public string potential_causes { get; set; }

        [Display(Name = "Impact")]
        public string impact { get; set; }

        [Display(Name = "Mitigation Measure(s)")]
        public string mitigation_measure { get; set; }

        [Display(Name = "Status")]
        public string potential_status { get; set; }

        [Display(Name = "Target Date to implement the measure")]
        public DateTime targeted_on { get; set; }

        [Display(Name = "Status Updated On")]
        public DateTime updated_on { get; set; }

        [Display(Name = "Upload Document")]
        public string obj_reject_upload { get; set; }

        [Display(Name = "Reason for not accepting Objective")]
        public string obj_reject_comment { get; set; }

        [Display(Name ="Notified to person")]
        public string Pcff_Notify { get; set; }

        internal bool FunAddActionPlanTrans(ObjectivesModels objObjectivesModels)
        {
            try
            {
                string sbegin_date = objObjectivesModels.begin_date.ToString("yyyy-MM-dd");
                string send_date = objObjectivesModels.end_date.ToString("yyyy-MM-dd");
                
                string sSqlstmt = "insert into t_objectives_actionplan (ObjectivesTrans_Id, actionplan, begin_date, end_date, resource, resp_person,action_ref_no";

                if (objObjectivesModels.upload != null && objObjectivesModels.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload";
                }
                sSqlstmt = sSqlstmt + ") values('" + objObjectivesModels.ObjectivesTrans_Id + "','" + objObjectivesModels.actionplan + "','" + sbegin_date + "','" + send_date 
                + "','" + objObjectivesModels.resource + "','" + objObjectivesModels.resp_person + "','" + objObjectivesModels.action_ref_no + "'";

                if (objObjectivesModels.upload != null && objObjectivesModels.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objObjectivesModels.upload + "'";
                }
                sSqlstmt = sSqlstmt + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSurveillanceTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionPlanTrans(ObjectivesModels ObjectivesModels)
        {
            try
            {
                string sSqlstmt = "update t_objectives_actionplan set actionplan='" + ObjectivesModels.actionplan + 
                 "',resource='" + ObjectivesModels.resource + "',resp_person='" + ObjectivesModels.resp_person + "'";

                if (ObjectivesModels.begin_date != null && ObjectivesModels.begin_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", begin_date='" + ObjectivesModels.begin_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }
                if (ObjectivesModels.end_date != null && ObjectivesModels.end_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", end_date='" + ObjectivesModels.end_date.ToString("yyyy/MM/dd HH':'mm':'ss") + "'";
                }

                if (ObjectivesModels.upload != null && ObjectivesModels.upload != "")
                {
                    sSqlstmt = sSqlstmt + ", upload='" + ObjectivesModels.upload + "'";
                }
                sSqlstmt = sSqlstmt + " where id_objective_action='" + ObjectivesModels.id_objective_action + "'";
                return (objGlobalData.ExecuteQuery(sSqlstmt));

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionPlanTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateActionPlanStatus(ObjectivesModelsList ObjList)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < ObjList.ObjectivesMList.Count; i++)
                {
                    string sid_objective_action = "null";
                    string sstatus_updated_on = "";


                    if (ObjList.ObjectivesMList[i].id_objective_action != null && ObjList.ObjectivesMList[i].id_objective_action != "")
                    {
                        sid_objective_action = ObjList.ObjectivesMList[i].id_objective_action;
                    }

                    if (ObjList.ObjectivesMList[i].status_updated_on != null && ObjList.ObjectivesMList[i].status_updated_on > Convert.ToDateTime("01/01/0001"))
                    {
                        sstatus_updated_on = ObjList.ObjectivesMList[i].status_updated_on.ToString("yyyy-MM-dd");
                    }

                    sSqlstmt = sSqlstmt + " update t_objectives_actionplan set action_status = '" + ObjList.ObjectivesMList[i].action_status
                       + "', status_updated_on = '" + sstatus_updated_on + "', reason_notcomplete ='" + ObjList.ObjectivesMList[i].reason_notcomplete + "'";

                    sSqlstmt = sSqlstmt + " where id_objective_action='" + sid_objective_action + "';";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateActionPlanStatus: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDeleteObjectivePlan(string sid_objective_action)
        {
            try
            {
                string sSqlstmt = "update t_objectives_actionplan set Active=0 where id_objective_action='" + sid_objective_action + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteObjectivePlan: " + ex.ToString());
            }
            return false;
        }


        internal bool FunDeleteObjectiveDoc(string sObjectivesTrans_Id)
        {
            try
            {
                string sSqlstmt = "update t_objectives_trans set trans_active=0 where ObjectivesTrans_Id='" + sObjectivesTrans_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteObjectiveDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddObjectives(ObjectivesModels objObjectivesModels, ObjectivesModelsList objList, HttpPostedFileBase filepath)
        {
            try
            {
                
                string sSqlstmt = "insert into t_objectives ( Dept, Audit_Criteria, Estld_by, "
                    + "CreatedBy,DocUploadPath,objective_level,branch,Location,branch_view)"
                + " values('" + objObjectivesModels.Dept + "','" + objObjectivesModels.Audit_Criteria + "','" + objObjectivesModels.Estld_by + "','"
                + objObjectivesModels.CreatedBy + "','" + objObjectivesModels.DocUploadPath + "','" + objObjectivesModels.objective_level + "','" + objObjectivesModels.branch + "','" + objObjectivesModels.Location + "','" + objObjectivesModels.branch_view + "')";

                //return FunAddObjectivesTrans(objObjectivesModelsList, objGlobalData.ExecuteQueryReturnId(sSqlstmt), objObjectivesModels);

                int Objectives_Id = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out Objectives_Id))
                {                    
                    //if (FunAddObjectivesTrans(objObjectivesModelsList, Objectives_Id, objObjectivesModels))
                    if(Objectives_Id > 0 && Convert.ToInt32(objList.ObjectivesMList.Count) > 0)
                    {
                        objList.ObjectivesMList[0].Objectives_Id = Convert.ToInt32(Objectives_Id.ToString());
                        FunAddObjectivesTrans(objList, Objectives_Id, objObjectivesModels.Estld_by);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddObjectives: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddObjectivesTrans(ObjectivesModelsList objObjectivesModelsList,int Objectives_Id,string sEstld_by)
        {
            try
            {
                ObjectivesModels objModels=new ObjectivesModels();

                string sSqlstmt = "";
                string sBranch = objGlobalData.GetCurrentUserSession().division;
                string sDept = objGlobalData.GetCurrentUserSession().DeptID;
                string sEmailid = "";
                string sEmailCCList = "";
                string sUsers = "";
                string aAttachment = "";

                for (int i = 0; i < objObjectivesModelsList.ObjectivesMList.Count; i++)
                {
                    if (objObjectivesModelsList.ObjectivesMList[i].Objectives_val != null)
                    {
                        string sObj_Estld_OnDate = objObjectivesModelsList.ObjectivesMList[i].Obj_Estld_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                        string sTarget_Date = objObjectivesModelsList.ObjectivesMList[i].Target_Date.ToString("yyyy-MM-dd HH':'mm':'ss");
                        DataSet dsData = objGlobalData.GetReportNo("OBJ", "", objGlobalData.GetCompanyBranchNameById(sBranch));
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            Obj_Ref = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }


                        sSqlstmt = sSqlstmt + "insert into t_objectives_trans (Obj_Ref,Objectives_Id, Obj_Estld_On, Objectives_val, Obj_Target, Base_Line_Value, Monitoring_Mechanism, Target_Date, "
                            + " Action_Plan, Freq_of_Eval,Personal_Responsible,Approved_By,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline"
                            + ") values('" + Obj_Ref + "','" + Objectives_Id + "','" + sObj_Estld_OnDate + "','" + objObjectivesModelsList.ObjectivesMList[i].Objectives_val
                         + "','" + objObjectivesModelsList.ObjectivesMList[i].Obj_Target + "','" + objObjectivesModelsList.ObjectivesMList[i].Base_Line_Value + "','"
                         + objObjectivesModelsList.ObjectivesMList[i].Monitoring_Mechanism + "','" + sTarget_Date + "','" + objObjectivesModelsList.ObjectivesMList[i].Action_Plan
                         + "','" + objObjectivesModelsList.ObjectivesMList[i].Freq_of_Eval + "','" + objObjectivesModelsList.ObjectivesMList[i].Personal_Responsible
                         + "','" + objObjectivesModelsList.ObjectivesMList[i].Approved_By + "','" + objObjectivesModelsList.ObjectivesMList[i].Accepted_Value + "','" + objObjectivesModelsList.ObjectivesMList[i].Risk_ifObjFails
                         + "','" + objObjectivesModelsList.ObjectivesMList[i].baseline_data + "','" + objObjectivesModelsList.ObjectivesMList[i].unit + "','" + objObjectivesModelsList.ObjectivesMList[i].obj_inline + "'); ";
                    }
                }                

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    //Email Generation
                    for (int i = 0; i < objObjectivesModelsList.ObjectivesMList.Count; i++)
                        {
                            if (objObjectivesModelsList.ObjectivesMList[i].Approved_By != null)
                            {
                                sUsers = sUsers + objGlobalData.GetMultiHrEmpNameById(objObjectivesModelsList.ObjectivesMList[i].Approved_By) + "," ;
                                sEmailid = sEmailid + objGlobalData.GetMultiHrEmpEmailIdById(objObjectivesModelsList.ObjectivesMList[i].Approved_By) + "," ;
                                sEmailCCList = sEmailCCList + objGlobalData.GetMultiHrEmpEmailIdById(sEstld_by) + "," ;
                                aAttachment = HttpContext.Current.Server.MapPath(objObjectivesModelsList.ObjectivesMList[i].Action_Plan) + ",";
                            }
                        }

                    if (sEmailid != null && sEmailid != "")
                        {                       

                            string sExtraMsg = "The Objective document Pending for your Approval, Objective Reference: " + objModels.GetObjRefbyObjId(Objectives_Id);


                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUsers, "objective", sExtraMsg);

                            sEmailCCList = sEmailCCList.Trim();
                            sEmailCCList = sEmailCCList.TrimEnd(',');
                            sEmailCCList = sEmailCCList.TrimStart(',');

                            sUsers = sUsers.Trim();
                            sUsers = sUsers.TrimEnd(',');
                            sUsers = sUsers.TrimStart(',');

                            sEmailid = Regex.Replace(sEmailid, ",+", ",");
                            sEmailid = sEmailid.Trim();
                            sEmailid = sEmailid.TrimEnd(',');
                            sEmailid = sEmailid.TrimStart(',');
                            //return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sEmailCCList);
                            return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], aAttachment, sEmailCCList, "");
                        }
                }                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddObjectivesTrans: " + ex.ToString());
            }
            return false;
        }
            
        internal bool FunAddObjectivesEvaluation(ObjectivesModels objObjectivesModels)
        {
            try
            {
                //string sFromPeriodDate = objObjectivesModels.FromPeriod.ToString("yyyy-MM-dd HH':'mm':'ss");
                //string sToPeriodDate = objObjectivesModels.ToPeriod.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sObj_Eval_OnDate = objObjectivesModels.Obj_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                
                string sSqlstmt = "update t_objectives_Evaluation set Obj_Eval_On='" + sObj_Eval_OnDate
                   + "', Obj_Achieved_Val='" + objObjectivesModels.Obj_Achieved_Val
                    + "', Trend='" + objObjectivesModels.Trend + "', NCR_Ref='" + objObjectivesModels.NCR_Ref
                    + "', Status_Obj_Eval='" + objObjectivesModels.Status_Obj_Eval + "', Source_data='" + objObjectivesModels.Source_data + "', Method_eval='" + objObjectivesModels.Method_eval + "', Remark='" + objObjectivesModels.Remark + "', Notified_to='" + objObjectivesModels.Notified_to + "'";

                if (objObjectivesModels.Evidence != null)
                {
                    sSqlstmt = sSqlstmt + ",Evidence='" + objObjectivesModels.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + " where ObjectivesEval_Id='" + objObjectivesModels.ObjectivesEval_Id + "';";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddObjectivesEvaluation: " + ex.ToString());
            }
            return false;
        }

        //internal bool FunUpdateObjectives(ObjectivesModels objObjectivesModels, ObjectivesModelsList objList)
        //{
        //    try
        //    {
        //        string sObj_Eval_OnDate = objObjectivesModels.Obj_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");

        //        string sSqlstmt = "update t_objectives set Dept='" + objObjectivesModels.Dept
        //            /*+ "', Audit_Criteria='" + objObjectivesModels.Audit_Criteria*/ + "', Estld_by='" + objObjectivesModels.Estld_by
        //            + "', objective_level='" + objObjectivesModels.objective_level + "', branch='" + objObjectivesModels.branch
        //            + "', Location='" + objObjectivesModels.Location + "'";

        //        if (objObjectivesModels.DocUploadPath != null)
        //        {
        //            sSqlstmt = sSqlstmt + ",DocUploadPath='" + objObjectivesModels.DocUploadPath + "'";
        //        }

        //        sSqlstmt = sSqlstmt + " where Objectives_Id='" + objObjectivesModels.Objectives_Id + "';"; 
        //        if (objGlobalData.ExecuteQuery(sSqlstmt))
        //        {
        //            if (Convert.ToInt32(objList.ObjectivesMList.Count) > 0)
        //            {
        //                objList.ObjectivesMList[0].Objectives_Id = Convert.ToInt32(Objectives_Id.ToString());
        //                FunAddObjectivesTrans(objList, objObjectivesModels.Objectives_Id);
        //            }
        //            else
        //            {
        //                FunUpdateObjTrans(Objectives_Id);
        //            }
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectives: " + ex.ToString());
        //    }
        //    return false;
        //}


        internal bool FunUpdateObjectives(ObjectivesModels objObjectivesModels)
        {
            try
            {
                //string sObj_Eval_OnDate = objObjectivesModels.Obj_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sTarget_Date = objObjectivesModels.Target_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_objectives set Dept='" + objObjectivesModels.Dept
                    /*+ "', Audit_Criteria='" + objObjectivesModels.Audit_Criteria*/ + "', Estld_by='" + objObjectivesModels.Estld_by
                    + "', objective_level='" + objObjectivesModels.objective_level + "', branch='" + objObjectivesModels.branch
                    + "', Location='" + objObjectivesModels.Location + "', branch_view='" + objObjectivesModels.branch_view + "'";

                if (objObjectivesModels.DocUploadPath != null)
                {
                    sSqlstmt = sSqlstmt + ",DocUploadPath='" + objObjectivesModels.DocUploadPath + "'";
                }

                sSqlstmt = sSqlstmt + " where Objectives_Id='" + objObjectivesModels.Objectives_Id + "';";

                sSqlstmt = sSqlstmt + "delete from t_objectives_trans where ObjectivesTrans_Id='" + objObjectivesModels.ObjectivesTrans_Id + "';";



                sSqlstmt = sSqlstmt + "insert into t_objectives_trans (Objectives_Id,Objectives_val,Obj_Target,Base_Line_Value,Monitoring_Mechanism,Freq_of_Eval,Approved_By,Accepted_Value,Risk_ifObjFails,baseline_data,unit,obj_inline,Obj_Ref";

                string sFields = "", sFieldValue = "";

                if (objObjectivesModels.Obj_Estld_On != null && objObjectivesModels.Obj_Estld_On > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Obj_Estld_On";
                    sFieldValue = sFieldValue + ", '" + objObjectivesModels.Obj_Estld_On.ToString("yyyy/MM/dd") + "'";
                }
                if (objObjectivesModels.Target_Date != null && objObjectivesModels.Target_Date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", Target_Date";
                    sFieldValue = sFieldValue + ", '" + objObjectivesModels.Target_Date.ToString("yyyy/MM/dd") + "'";
                }
                if (objObjectivesModels.Action_Plan != null && objObjectivesModels.Action_Plan != "")
                {
                    sFields = sFields + ", Action_Plan";
                    sFieldValue = sFieldValue + ", '" + objObjectivesModels.Action_Plan + "'";
                    
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objObjectivesModels.Objectives_Id + "','" + objObjectivesModels.Objectives_val + "','" + objObjectivesModels.Obj_Target + "','" + objObjectivesModels.Base_Line_Value + "', '" + objObjectivesModels.Monitoring_Mechanism + "', '" + objObjectivesModels.Freq_of_Eval + "'"
                      + ",'" + objObjectivesModels.Approved_By + "','" + objObjectivesModels.Accepted_Value + "','" + objObjectivesModels.Risk_ifObjFails + "', '" + objObjectivesModels.baseline_data + "', '" + objObjectivesModels.unit + "', '" + objObjectivesModels.obj_inline + "', '" + objObjectivesModels.Obj_Ref + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";



                //sSqlstmt = sSqlstmt + "update t_objectives_trans set  Objectives_val='" + objObjectivesModels.Objectives_val + "', "
                //    + "Obj_Target='" + objObjectivesModels.Obj_Target + "', Base_Line_Value='" + objObjectivesModels.Base_Line_Value
                //    + "', Monitoring_Mechanism='" + objObjectivesModels.Monitoring_Mechanism + "', Target_Date='" + sTarget_Date
                //    + "', Approved_By='" + objObjectivesModels.Approved_By + "', Approved_Status=0 ,ApprovedDate = '0001/01/01', Accepted_Value='" + objObjectivesModels.Accepted_Value + "', Risk_ifObjFails='" + objObjectivesModels.Risk_ifObjFails
                //    + "', baseline_data='" + objObjectivesModels.baseline_data + "', unit='" + objObjectivesModels.unit + "', obj_inline='" + objObjectivesModels.obj_inline + "'";

                //if (objObjectivesModels.Action_Plan != null && objObjectivesModels.Action_Plan != "")
                //{
                //    sSqlstmt = sSqlstmt + ", Action_Plan='" + objObjectivesModels.Action_Plan + "'";
                //}

                //sSqlstmt = sSqlstmt + " where ObjectivesTrans_Id='" + objObjectivesModels.ObjectivesTrans_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectives: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateObjectivesPlan(ObjectivesModels objObjectivesModels)
        {
            try
            {
                string sObj_Estld_OnDate = objObjectivesModels.Obj_Estld_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sTarget_Date = objObjectivesModels.Target_Date.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_objectives_trans set Obj_Estld_On ='" + sObj_Estld_OnDate + "', Objectives_val='" + objObjectivesModels.Objectives_val + "', "
                    + "Obj_Target='" + objObjectivesModels.Obj_Target + "', Base_Line_Value='" + objObjectivesModels.Base_Line_Value
                    + "', Monitoring_Mechanism='" + objObjectivesModels.Monitoring_Mechanism + "', Target_Date='" + sTarget_Date
                    + "', Freq_of_Eval='" + objObjectivesModels.Freq_of_Eval + "', Approved_By='" + objObjectivesModels.Approved_By
                    + "', Accepted_Value='" + objObjectivesModels.Accepted_Value + "', Risk_ifObjFails='" + objObjectivesModels.Risk_ifObjFails + "'";

                if (objObjectivesModels.Action_Plan != null && objObjectivesModels.Action_Plan != "")
                {
                    sSqlstmt = sSqlstmt + ", Action_Plan='" + objObjectivesModels.Action_Plan + "'";
                }

                sSqlstmt = sSqlstmt + " where ObjectivesTrans_Id='" + objObjectivesModels.ObjectivesTrans_Id + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivesPlan: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateObjectivesEvaluation(ObjectivesModels objObjectivesModels)
        {
            try
            {
                string sObj_Eval_OnDate = objObjectivesModels.Obj_Eval_On.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sFromPeriodDate = objObjectivesModels.FromPeriod.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sToPeriodDate = objObjectivesModels.ToPeriod.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_objectives_Evaluation set Obj_Eval_On ='" + sObj_Eval_OnDate + "', FromPeriod='" + sFromPeriodDate + "', "
                    + "ToPeriod='" + sToPeriodDate + "', Obj_Achieved_Val='" + objObjectivesModels.Obj_Achieved_Val + "', Trend='" + objObjectivesModels.Trend + "', NCR_Ref='"
                    + objObjectivesModels.NCR_Ref + "',Status_Obj_Eval='" + objObjectivesModels.Status_Obj_Eval + "'";

                if (objObjectivesModels.Evidence != null && objObjectivesModels.Evidence != "")
                {
                    sSqlstmt = sSqlstmt + ", Evidence='" + objObjectivesModels.Evidence + "'";
                }
                sSqlstmt = sSqlstmt + " where ObjectivesEval_Id='" + objObjectivesModels.ObjectivesEval_Id + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivesEvaluation: " + ex.ToString());
            }
            return false;
        }

        //-----Objective Potention starts--------

        internal bool FunAddObjectivePotential(ObjectivesModels objObjectivesModels)
        {
            try
            {
                string stargeted_on = objObjectivesModels.targeted_on.ToString("yyyy-MM-dd");
                string supdated_on = objObjectivesModels.updated_on.ToString("yyyy-MM-dd");

                string sSqlstmt = "insert into t_objectives_potential (ObjectivesTrans_Id, potential_causes, impact, mitigation_measure, targeted_on, updated_on,potential_status,logged_by,Pcff_Notify) values('"
                    + objObjectivesModels.ObjectivesTrans_Id + "','" + objObjectivesModels.potential_causes + "','" + objObjectivesModels.impact + "','" + objObjectivesModels.mitigation_measure
                    + "','" + stargeted_on + "','" + supdated_on + "','" + objObjectivesModels.potential_status + "','" + objGlobalData.GetCurrentUserSession().empid + "','" + objObjectivesModels.Pcff_Notify + "')";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                    {
                    return objGlobalData.ExecuteQuery("update t_objectives_trans set Approved_Status=0 where ObjectivesTrans_Id ='" + objObjectivesModels.ObjectivesTrans_Id + "'");
                   }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddObjectivePotential: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateObjectivePotential(ObjectivesModels ObjectivesModels)
        {
            try
            {
                string sSqlstmt = "update t_objectives_potential set potential_causes='" + ObjectivesModels.potential_causes +
                 "',impact='" + ObjectivesModels.impact + "',mitigation_measure='" + ObjectivesModels.mitigation_measure
                 + "',potential_status='" + ObjectivesModels.potential_status + "',Pcff_Notify='" + ObjectivesModels.Pcff_Notify + "'";

                if (ObjectivesModels.targeted_on != null && ObjectivesModels.targeted_on > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", targeted_on='" + ObjectivesModels.targeted_on.ToString("yyyy/MM/dd") + "'";
                }
                if (ObjectivesModels.updated_on != null && ObjectivesModels.updated_on > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", updated_on='" + ObjectivesModels.updated_on.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_potential='" + ObjectivesModels.id_potential + "'";
                return (objGlobalData.ExecuteQuery(sSqlstmt));

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivePotential: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteObjectivePotential(string sid_potential)
        {
            try
            {
                string sSqlstmt = "update t_objectives_potential set pot_active=0 where id_potential='" + sid_potential + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteObjectivePotential: " + ex.ToString());
            }
            return false;
        }

        //-----Objective Potention Ends--------



        public List<string> GetObjectiveRef(string sDeptId)
        {

            List<string> lstObjectives = new List<string>();
            try
            {
                DataSet dsObjectives = objGlobalData.Getdetails("select Obj_ref,  Audit_Criteria,  Estld_by "
                    + "from t_objectives where Dept='" + sDeptId + "' and Active=1");
                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    lstObjectives = dsObjectives.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
             
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveRef: " + ex.ToString());
            }
            return lstObjectives;
        }

        public DataSet GetObjectiveDetails(string DeptId)
        {
            DataSet dsObjectives = new DataSet();
            try
            {
               dsObjectives = objGlobalData.Getdetails("select * from t_objectives where Dept='" + DeptId + "'");
                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    return (dsObjectives);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveDetails: " + ex.ToString());
            }
            return dsObjectives;
        }

        public bool checkObjRefExists(string Obj_Ref)
        {
            try
            {
                string sSqlstmt = "select Objectives_Id from t_objectives where Obj_Ref='" + Obj_Ref + "' and Active=1";
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

        internal bool FunUpdateObjectivesApproval(string sEMpid, string ObjectivesTrans_Id, int sStatus)
        {
            try
            {
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                //string sSqlstmt = "update t_objectives set Approved_Status ='" + sStatus + "', ApprovedDate='" + sApprovedDate + "' where Objectives_Id='" + Objectives_Id + "'";
                string sSqlstmt = "update t_objectives_trans set Approved_Status ='" + sStatus + "', ApprovedDate='" + sApprovedDate + "' where ObjectivesTrans_Id='" + ObjectivesTrans_Id + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    SendObjectiveApprvRejectEmail(sEMpid, ObjectivesTrans_Id, sStatus);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivesApproval: " + ex.ToString());
            }

            return false;
        }


        internal bool FunObjectivesApprovalbyDetail(ObjectivesModels objmodel)
        {
            try
            {
                string sEMpid = objGlobalData.GetCurrentUserSession().empid;
                string sApprovedDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");
                int Status = Convert.ToInt32(objmodel.Approved_Status);
                string ObjectivesTrans_Id = Convert.ToString(objmodel.ObjectivesTrans_Id);

                string sSqlstmt = "update t_objectives_trans set Approved_Status ='" + Status + "', ApprovedDate='" + sApprovedDate
                    + "', obj_reject_comment='" + objmodel.obj_reject_comment + "', obj_reject_upload='" + objmodel.obj_reject_upload + "' where ObjectivesTrans_Id='" + objmodel.ObjectivesTrans_Id + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    if (Status == 2)//Rejected
                    {
                        string sEmailid = "", sCCList = "", sExtraMsg = "";

                        //DataSet dsDocument = objGlobalData.Getdetails("select Obj_Ref, Estld_by, Personal_Responsible, Approved_By from t_objectives where Objectives_Id='" + Objectives_Id + "'");
                        DataSet dsDocument = objGlobalData.Getdetails("select ObjectivesTrans_Id,Obj_Ref, a.Estld_by, b.Personal_Responsible, b.Approved_By,b.Objectives_val,obj_reject_comment from t_objectives a,t_objectives_trans b" +
                            " where a.Objectives_Id=b.Objectives_Id and ObjectivesTrans_Id='" + ObjectivesTrans_Id + "'");

                        if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                        {

                            sCCList = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Approved_By"].ToString());

                            if (dsDocument.Tables[0].Rows[0]["Estld_by"].ToString() != "")
                            {
                                sEmailid = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Estld_by"].ToString());
                            }

                            sEmailid = Regex.Replace(sEmailid, ",+", ",");
                            sEmailid = sEmailid.Trim();
                            sEmailid = sEmailid.TrimEnd(',');
                            sEmailid = sEmailid.TrimStart(',');
                            sExtraMsg = "Objective has been Rejected, Objective Reference: " + dsDocument.Tables[0].Rows[0]["Obj_Ref"].ToString()
                                + " and Objective Value: " + dsDocument.Tables[0].Rows[0]["Objectives_val"].ToString()
                                + " and Comments on Rejection: " + dsDocument.Tables[0].Rows[0]["obj_reject_comment"].ToString();


                            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                                objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["Estld_by"].ToString()), "objective1", sExtraMsg);

                            objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList, "");
                            return true;

                        }
                    }
                    else
                    {
                        SendObjectiveApprvRejectEmail(sEMpid, ObjectivesTrans_Id, Status);
                        return true;
                    }                   
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunObjectivesApprovalbyDetail: " + ex.ToString());
            }

            return false;
        }

        internal bool SendObjectiveApprvRejectEmail(string sEMpid, string ObjectivesTrans_Id, int iStatus)
        {
            try
            {
                string sEmailid = "", sCCList = "", sExtraMsg="";

                //DataSet dsDocument = objGlobalData.Getdetails("select Obj_Ref, Estld_by, Personal_Responsible, Approved_By from t_objectives where Objectives_Id='" + Objectives_Id + "'");
                DataSet dsDocument = objGlobalData.Getdetails("select ObjectivesTrans_Id,Obj_Ref, a.Estld_by, b.Personal_Responsible, b.Approved_By,b.Objectives_val from t_objectives a,t_objectives_trans b" +
                    " where a.Objectives_Id=b.Objectives_Id and ObjectivesTrans_Id='" + ObjectivesTrans_Id + "'");

                if (dsDocument.Tables.Count > 0 && dsDocument.Tables[0].Rows.Count > 0)
                    {

                    if (iStatus == 1)//approved 
                    {
                        if(dsDocument.Tables[0].Rows[0]["Estld_by"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Estld_by"].ToString());
                        }
                       
                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Approved_By"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Objective has been approved,Objective Reference: " + dsDocument.Tables[0].Rows[0]["Obj_Ref"].ToString();
                    }

                    if (iStatus == 2)//approved 
                    {
                        if (dsDocument.Tables[0].Rows[0]["Estld_by"].ToString() != "")
                        {
                            sCCList = objGlobalData.GetHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Estld_by"].ToString());
                        }

                        sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(dsDocument.Tables[0].Rows[0]["Approved_By"].ToString());
                        sEmailid = Regex.Replace(sEmailid, ",+", ",");
                        sEmailid = sEmailid.Trim();
                        sEmailid = sEmailid.TrimEnd(',');
                        sEmailid = sEmailid.TrimStart(',');
                        sExtraMsg = "Objective has been Rejected, Objective Reference: " + dsDocument.Tables[0].Rows[0]["Obj_Ref"].ToString()
                            +" and Objective Value: " + dsDocument.Tables[0].Rows[0]["Objectives_val"].ToString();
                    }

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(
                        objGlobalData.GetMultiHrEmpNameById(dsDocument.Tables[0].Rows[0]["Approved_By"].ToString()), "objective1", sExtraMsg);

                        objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "", sCCList,"");
                        return true;
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateObjectivesApproval: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddComments(string sEmpId, string sComments, string Objectives_Id)
        {
            try
            {
                string sCommentDate = DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into  t_objectives_comments (Objectives_Id, empId, Comments, CommentDate) values('" + Objectives_Id + "','" + sEmpId + "','"
                    + sComments + "','" + sCommentDate + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddComments: " + ex.ToString());
            }
            return false;
        }
       
        public string GetObjectiveStatusNameById(string sFreq)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Evaluation Status' and (item_id='" + sFreq + "' or item_desc='" + sFreq + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetObjectiveStatusIdByName(string sFreq)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Evaluation Status' and  item_desc='" + sFreq + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveStatusIdByName: " + ex.ToString());
            }
            return "";
        }


        public MultiSelectList GetMultiObjectiveStatusList()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Evaluation Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiObjectiveStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        public string GetMultiObjectiveEvaluationById(string id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Status Period' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetMultiObjectiveEvaluationById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiObjectiveEvaluation()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Status Period' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiObjectiveStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        //New Functions
        

        public MultiSelectList GetObjectiveInlineList()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Inline With' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveInlineList: " + ex.ToString());
            }
            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        public string GetObjectiveInlineById(string id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Inline With' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveInlineById: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetObjectiveUnitList()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Unit' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveUnitList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }
        public string GetObjectiveUnitById(string id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Unit' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveUnitById: " + ex.ToString());
            }
            return "";
        }

        public string GetObjRefbyObjId(int id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select group_concat(Obj_Ref) as Name from t_objectives_trans b,t_objectives a where " +
                    "a.Objectives_Id= '"+id+"' and a.active=1 and a.Objectives_Id=b.Objectives_Id");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectiveUnitById: " + ex.ToString());
            }
            return "";
        }

        public string GetObjectivePotentialStatusByName(string Name)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Potential Status' and item_desc='" + Name + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Id"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectivePotentialStatusByName: " + ex.ToString());
            }
            return "";
        }

        public string GetObjectivePotentialStatusById(string id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Potential Status' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectivePotentialStatusById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetObjectivePotentialStatusList()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Potential Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetObjectivePotentialStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        public string GetObjectivePotentialImpactById(string id)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                      + "and header_desc='Objective Potential Impact' and (item_id='" + id + "' or item_desc='" + id + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetObjectivePotentialImpactById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetObjectivePotentialImpactList()
        {
            DropdownObjectiveList lst = new DropdownObjectiveList();
            lst.lst = new List<DropdownObjectiveItems>();
            try
            {
                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Objective Potential Impact' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownObjectiveItems reg = new DropdownObjectiveItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetObjectivePotentialImpactList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

    }

    public class ObjectivesModelsList
    {
        public List<ObjectivesModels> ObjectivesMList { get; set; }
    }

    public class DropdownObjectiveItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownObjectiveList
    {
        public List<DropdownObjectiveItems> lst { get; set; }
    }
}