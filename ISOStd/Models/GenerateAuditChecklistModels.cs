using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ISOStd.Models
{
    public class GenerateAuditChecklistModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public int id_AuditChecklist { get; set; }

        [Required]
        [Display(Name = "Id")]
        public int idt_checklist { get; set; }

        [Required]
        [Display(Name = "Item No")]
        public int Itemno { get; set; }

        [Display(Name = "location")]
        public string location { get; set; }

        [Display(Name = "Checklist Reference")]
        public string ChecklistRef { get; set; }

        //[Display(Name = "Division")]
        //public string division { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Audit Criteria")]
        public string AuditCriteria { get; set; }

        [Required]
        [Display(Name = "Audit No")]
        public string AuditNo { get; set; }

        [Required]
        [Display(Name = "Audit Date")]
        public DateTime AuditDate { get; set; }

        [Required]
        [Display(Name = "Auditor(s)")]
        public string Auditors { get; set; }

        [Required]
        [Display(Name = "Auditee(s)")]
        public string Auditee { get; set; }
       
        [Display(Name = "Other Notes")]
        public string Notes { get; set; }
       
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Comment")]
        public string comment { get; set; }

        //[Required]
        //[Display(Name = "Questions")]
        //public string Questions { get; set; }

        //[Display(Name = "Division")]
        //public string audit_division { get; set; }

        //[Display(Name = "If Department is Common then click this")]
        //public string dept_common { get; set; }

        [Display(Name = "Prepared By")]
        public string prepared_by { get; set; }

        [Display(Name = "Created On")]
        public DateTime created_on { get; set; }

        [Display(Name = "Ammended On")]
        public DateTime ammended_on { get; set; }

        [Display(Name = "Notified To")]
        public string notified_to { get; set; }

        [Display(Name = "Division")]
        public string directorate { get; set; }       

        [Display(Name = "Department")]
        public string grp { get; set; }
        public string grp_common { get; set; }

        //[Display(Name = "Team")]
        //public string team { get; set; }
        //public string team_common { get; set; }

        public string ReportStatus { get; set; }
        public string AuditNum { get; set; }

        //AuditChecklistTrans
        public string id_auditchecklist_trans { get; set; }

        [Display(Name = "Standard / Procedure")]
        public string IsoStd { get; set; }

        [Display(Name = "Clause / Section No")]
        public string Clause { get; set; }

        [Display(Name = "Questions")]
        public string Questions { get; set; }

        internal bool FunAddAuditChecklist(GenerateAuditChecklistModels objchecklist, GenerateAuditChecklistModelsList objList)
        {
            try
            {

                string sSqlstmt = "insert into t_auditchecklist (AuditCriteria,location,ChecklistRef,prepared_by," +
                    "notified_to,directorate,grp,grp_common";
                string sFields = "", sFieldValue = "";
                if (objchecklist.created_on != null && objchecklist.created_on > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", created_on";
                    sFieldValue = sFieldValue + ", '" + objchecklist.created_on.ToString("yyyy/MM/dd") + "'";
                }
                if (objchecklist.ammended_on != null && objchecklist.ammended_on > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", ammended_on";
                    sFieldValue = sFieldValue + ", '" + objchecklist.ammended_on.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('"+ objchecklist.AuditCriteria + "','" + objchecklist.location + "','" + objchecklist.ChecklistRef 
                    + "','" + objchecklist.prepared_by + "','" + objchecklist.notified_to + "','" + objchecklist.directorate 
                    + "','" + objchecklist.grp + "','" + objchecklist.grp_common + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                //return objGlobalData.ExecuteQuery(sSqlstmt);
                int iid_AuditChecklist = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out iid_AuditChecklist))
                {
                    if (iid_AuditChecklist > 0 && Convert.ToInt32(objList.AuditCheckList.Count) > 0)
                    {
                        objList.AuditCheckList[0].id_AuditChecklist = iid_AuditChecklist;
                        FunAddChkListTrans(objList);
                    }

                    if (iid_AuditChecklist > 0)
                    {
                        //string LocationName = objGlobalData.GetCompanyBranchNameById(audit_division);
                        //string DeptName = "Common";
                        //if (grp_common.ToLower() != "common")
                        //{
                        //    DeptName = objGlobalData.GetDeptNameById(Department);
                        //}
                        //DeptName = DeptName.Substring(0,5);
                        //LocationName = LocationName.Substring(0, 5);
                        //DataSet dsData = objGlobalData.GetReportNo("Chklist", DeptName, LocationName);
                        DataSet dsData = objGlobalData.GetReportNo("Chklist", "", objGlobalData.GetBranchShortNameByID(objGlobalData.GetCurrentUserSession().division));
                        if (dsData != null && dsData.Tables.Count > 0)
                        {
                            ChecklistRef = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                        }                     

                        string sql = "update t_auditchecklist set ChecklistRef='" + ChecklistRef + "' where id_AuditChecklist = '" + iid_AuditChecklist + "'";

                        return objGlobalData.ExecuteQuery(sql);
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddChkListTrans(GenerateAuditChecklistModelsList objList)
        {
            try
            {
                string sSqlstmt = "delete from t_auditchecklist_trans where id_AuditChecklist='" + objList.AuditCheckList[0].id_AuditChecklist + "'; ";

                for (int i = 0; i < objList.AuditCheckList.Count; i++)
                {
                    string sid_auditchecklist_trans = "null";
                    if (objList.AuditCheckList[i].id_auditchecklist_trans != null)
                    {
                        sid_auditchecklist_trans = objList.AuditCheckList[i].id_auditchecklist_trans;
                    }
                    sSqlstmt = sSqlstmt + " insert into t_auditchecklist_trans (id_auditchecklist_trans,id_AuditChecklist,IsoStd,Clause,Questions)"
                    + " values(" + sid_auditchecklist_trans + "," + objList.AuditCheckList[0].id_AuditChecklist
                    + ",'" + objList.AuditCheckList[i].IsoStd + "','" + objList.AuditCheckList[i].Clause + "','" + objList.AuditCheckList[i].Questions + "')"
                    + " ON DUPLICATE KEY UPDATE "
                    + " id_auditchecklist_trans= values(id_auditchecklist_trans), id_AuditChecklist= values(id_AuditChecklist), IsoStd = values(IsoStd), Clause= values(Clause), Questions= values(Questions) ; ";

                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddChkListTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateChkListTrans(string sid_AuditChecklist)
        {
            try
            {
                string sSqlstmt = "delete from t_auditchecklist_trans where id_AuditChecklist='" + sid_AuditChecklist + "'; ";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateChkListTrans: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteAuditChecklist(string sid_AuditChecklist)
        {
            try
            {
                string sSqlstmt = "update t_auditchecklist set Active=0 where id_AuditChecklist='" + sid_AuditChecklist + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAuditChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteChecklist(string sidt_checklist)
        {
            try
            {
                string sSqlstmt = "update t_generateauditchecklist set Active=0 where idt_checklist='" + sidt_checklist + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChecklist: " + ex.ToString());
            }
            return false;
        }
               
        internal bool FunAddAuditPerformance(GenerateAuditChecklistModels objAuditChecklist, AuditPerformanceModelsList objAudit)
        {
            try
            {
                string sAuditDate = objAuditChecklist.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_generateauditchecklist (id_AuditChecklist,AuditNo,AuditDate,Auditors,Auditee,Notes,Remarks)"
                + "values('" + objAuditChecklist.id_AuditChecklist + "','" + objAuditChecklist.AuditNo + "','" + sAuditDate + "','" + objAuditChecklist.Auditors + "'"
                + ",'" + objAuditChecklist.Auditee + "','" + objAuditChecklist.Notes + "','" + objAuditChecklist.Remarks + "')";


                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    string sqlstmt = "select idt_checklist from t_generateauditchecklist where AuditNo='" + objAuditChecklist.AuditNo + "'"
                   + " and AuditDate='" + sAuditDate + "'and Auditors='" + objAuditChecklist.Auditors + "' and Auditee='" + objAuditChecklist.Auditee + "'"
                    + "and Notes='" + objAuditChecklist.Notes + "' and Active=1";
                    DataSet dsList = objGlobalData.Getdetails(sqlstmt);
                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            idt_checklist = Convert.ToInt32(dsList.Tables[0].Rows[0]["idt_checklist"].ToString());

                        }
                        catch (Exception ex)
                        {
                            objGlobalData.AddFunctionalLog("Exception in FunAddAuditPerformance: " + ex.ToString());

                        }
                    }

                    AuditPerformanceModels objElement = new AuditPerformanceModels();
                    objAudit.lstAudit[0].idt_checklist = objAuditChecklist.idt_checklist;
                    objElement.FunAddAuditPerformance(objAudit);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditPerformance: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditPerformance(GenerateAuditChecklistModels objAuditChecklist, AuditPerformanceModelsList objAudit)
        {
            try
            {
                string sAuditDate = objAuditChecklist.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_generateauditchecklist set AuditNo='" + objAuditChecklist.AuditNo + "', "
                        + "Auditors='" + objAuditChecklist.Auditors + "',Auditee='" + objAuditChecklist.Auditee + "',Notes='" + objAuditChecklist.Notes + "'"
                        + ",Remarks='" + objAuditChecklist.Remarks + "'";

                if (objAuditChecklist.AuditDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", AuditDate='" + sAuditDate + "' ";
                }
                sSqlstmt = sSqlstmt + " where idt_checklist='" + objAuditChecklist.idt_checklist + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    AuditPerformanceModels objElement = new AuditPerformanceModels();
                    objAudit.lstAudit[0].idt_checklist = objAuditChecklist.idt_checklist;
                    objElement.FunUpdateAuditPerformance(objAudit);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditPerformance: " + ex.ToString());
            }
            return false;
        }


        internal bool FunUpdateAuditChecklist(GenerateAuditChecklistModels objAudit, GenerateAuditChecklistModelsList objList)
        {
            try
            {
                //if (cnt > 0)
                //{
                    string sSqlstmt = "update t_auditchecklist set  prepared_by='" + objAudit.prepared_by + "',AuditCriteria='" + objAudit.AuditCriteria + "',notified_to='" + objAudit.notified_to
                       + "',directorate='" + objAudit.directorate + "',grp='" + objAudit.grp + "',grp_common='" + objAudit.grp_common + "'";

                    if (objAudit.created_on != null && objAudit.created_on > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", created_on='" + objAudit.created_on.ToString("yyyy/MM/dd") + "'";
                    }

                    if (objAudit.ammended_on != null && objAudit.ammended_on > Convert.ToDateTime("01/01/0001"))
                    {
                        sSqlstmt = sSqlstmt + ", ammended_on='" + objAudit.ammended_on.ToString("yyyy/MM/dd") + "'";
                    }

                    sSqlstmt = sSqlstmt + " where id_AuditChecklist='" + objAudit.id_AuditChecklist + "';";
                    //return objGlobalData.ExecuteQuery(sSqlstmt);
                    if (objGlobalData.ExecuteQuery(sSqlstmt))
                    {
                        if (id_AuditChecklist > 0 && Convert.ToInt32(objList.AuditCheckList.Count) > 0)
                        {
                            objList.AuditCheckList[0].id_AuditChecklist = id_AuditChecklist;
                            FunAddChkListTrans(objList);
                        }
                        else
                        {
                            FunUpdateChkListTrans(id_AuditChecklist.ToString());
                        }
                    return true;
                    }
                //else
                //{

                //    string sSqlstmt = "update t_auditchecklist set Department='" + objAudit.Department + "', "
                //         + "branch='" + objAudit.division  + "',location='" + objAudit.location + "',ChecklistRef='" + objAudit.ChecklistRef + "',AuditCriteria='" + objAudit.AuditCriteria + "'";

                //    sSqlstmt = sSqlstmt + " where id_AuditChecklist='" + objAudit.id_AuditChecklist + "';";
                //    return objGlobalData.ExecuteQuery(sSqlstmt);
                //}
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditChecklist: " + ex.ToString());
            }
            return false;           
        }
    }
    public class AuditElementsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public string id_element { get; set; }
      
        [Display(Name = "Element")]
        public string Audit_Elements { get; set; }
     
        [Display(Name = "Department")]
        public string Department { get; set; }
      
        [Display(Name = "location")]
        public string location { get; set; }
        
        [Display(Name = "division")]
        public string division { get; set; }
        
        [Display(Name = "Audit Criteria")]
        public string IsoStd { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string id_auditratings { get; set; }

        [Required]
        [Display(Name = "Options")]
        public int Options { get; set; }

        public DataSet GetAuditRating()
        {           
            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_auditratings, Options from t_auditratings order by id_auditratings");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        public string GetAuditQuestionNameById(string sid_element)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Audit_Elements FROM t_auditelements where id_element='" + sid_element + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Audit_Elements"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditQuestionNameById: " + ex.ToString());
            }
            return "";
        }

        public string GetAuditRatingNameById(string sid_auditratings)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Options FROM t_auditratings where id_auditratings='" + sid_auditratings + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Options"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditRatingNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetAuditElementsListbox(string Department)
        {
            AuditElementsModelsList AuditElemt = new AuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<AuditElementsModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_element,Audit_Elements from t_auditelements where Department='" + Department + "' and Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        AuditElementsModels data = new AuditElementsModels()
                        {
                            id_element =dsElement.Tables[0].Rows[i]["id_element"].ToString(),
                            Audit_Elements = dsElement.Tables[0].Rows[i]["Audit_Elements"].ToString()
                        };

                        AuditElemt.lstAuditElement.Add(data);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.lstAuditElement, "id_element", "Audit_Elements");
        }

        public MultiSelectList GetQuestionsListbox(string chklist_id)
        {
            AuditElementsModelsList AuditElemt = new AuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<AuditElementsModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_auditchecklist_trans as id_element, b.Questions as Audit_Elements from t_auditchecklist a, t_auditchecklist_trans b where a.id_AuditChecklist = b.id_AuditChecklist and a.id_AuditChecklist='" + chklist_id + "'  and Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        AuditElementsModels data = new AuditElementsModels()
                        {
                            id_element = dsElement.Tables[0].Rows[i]["id_element"].ToString(),
                            Audit_Elements = dsElement.Tables[0].Rows[i]["Audit_Elements"].ToString()
                        };

                        AuditElemt.lstAuditElement.Add(data);
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetQuestionsListbox: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.lstAuditElement, "id_element", "Audit_Elements");
        }

        public MultiSelectList GetAuditElementsListboxByIsoStd(string Department,string IsoStd)
        {
            AuditElementsModelsList AuditElemt = new AuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<AuditElementsModels>();

            try
            {

                string[] bu = IsoStd.Split(',');
                int z = bu.Length;
                string sSqlstmt = null;
                if (z == 1)
                {
                    sSqlstmt = "select id_element,Audit_Elements from t_auditelements where IsoStd='" + IsoStd + "' and Department='" + Department + "' and Active=1";
                }

                else
                {
                    sSqlstmt = "select id_element,Audit_Elements from t_auditelements where  Department='" + Department + "' and Active=1" + " and (find_in_set('" + bu[0] + "',IsoStd)";
                    for (int k = 1; k < z; k++)
                    {
                        sSqlstmt = sSqlstmt + " or find_in_set('" + bu[k] + "',IsoStd)";

                    }
                    sSqlstmt = sSqlstmt + ")";
                }

                DataSet dsElement = objGlobalData.Getdetails(sSqlstmt);


               // DataSet dsElement = objGlobalData.Getdetails("select id_element,Audit_Elements from t_auditelements where IsoStd='" + IsoStd + "' and Department='" + Department + "' and Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        AuditElementsModels data = new AuditElementsModels()
                        {
                            id_element = dsElement.Tables[0].Rows[i]["id_element"].ToString(),
                            Audit_Elements = dsElement.Tables[0].Rows[i]["Audit_Elements"].ToString()
                        };

                        AuditElemt.lstAuditElement.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListboxByIsoStd: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.lstAuditElement, "id_element", "Audit_Elements");
        }


        public MultiSelectList GetAuditNoListbox()
        {
            AuditModelsList AuditElemt = new AuditModelsList();
            AuditElemt.AuditNoList = new List<AuditModels>();
            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select AuditID,AuditNum from t_internal_audit where Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        AuditModels data = new AuditModels()
                        {
                            AuditID = dsElement.Tables[0].Rows[i]["AuditID"].ToString(),
                            AuditNum = dsElement.Tables[0].Rows[i]["AuditNum"].ToString()
                        };

                        AuditElemt.AuditNoList.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.AuditNoList, "AuditID", "AuditNum");
        }
        public MultiSelectList GetAuditElementsListbox()
        {
            AuditElementsModelsList AuditElemt = new AuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<AuditElementsModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_element,Audit_Elements from t_auditelements where Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        AuditElementsModels data = new AuditElementsModels()
                        {
                            id_element = dsElement.Tables[0].Rows[i]["id_element"].ToString(),
                            Audit_Elements = dsElement.Tables[0].Rows[i]["Audit_Elements"].ToString()
                        };

                        AuditElemt.lstAuditElement.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.lstAuditElement, "id_element", "Audit_Elements");
        }

        internal bool FunAddAuditElements(AuditElementsModels objElementModels)
        {
            try
            {
                string sSqlstmt = "insert into t_auditelements (Audit_Elements,Department,IsoStd) values('" + objElementModels.Audit_Elements + "','" + objElementModels.Department + "','" + objElementModels.IsoStd + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditElements: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditElements(int id_element, string Audit_Elements)
        {
            try
            {
                string sSqlstmt = "update t_auditelements set Audit_Elements='" + Audit_Elements + "' where id_element='" + id_element + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditElements: " + ex.ToString());
            }
            return false;
        }

        public bool checkAuditElementExists(string sAudit_Elements)
        {
            try
            {
                string sSqlstmt = "select Audit_Elements from t_auditelements where Audit_Elements='" + sAudit_Elements + "' and active=1";

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkAuditElementExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunDeleteQuestions(string sid_element)
        {
            try
            {
                string sSqlstmt = "update t_auditelements set Active=0 where id_element='" + sid_element + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuestions: " + ex.ToString());
            }
            return false;
        }

 

    }
    public class AuditPerformanceModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public int id_AuditPerformanceChecklist { get; set; }

        [Required]
        [Display(Name = "Id")]
        public int idt_checklist { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string id_element { get; set; }

        [Required]
        [Display(Name = "Id")]
        public string id_auditratings { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string comment { get; set; }

        [Required]
        [Display(Name = "Evidence")]
        public string evidence_upload { get; set; }

        internal bool FunAddAuditPerformance(AuditPerformanceModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "delete from t_auditperformancechecklist where idt_checklist='"
                        + objAudit.lstAudit[0].idt_checklist + "'; ";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_auditperformancechecklist (idt_checklist, id_element, id_auditratings,comment,evidence_upload"
                        + ") values('" + objAudit.lstAudit[0].idt_checklist + "','" + objAudit.lstAudit[i].id_element
                        + "','" + objAudit.lstAudit[i].id_auditratings + "','" + objAudit.lstAudit[i].comment + "','" + objAudit.lstAudit[i].evidence_upload + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditPerformance: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateAuditPerformance(AuditPerformanceModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_auditperformancechecklist set id_element='" + objAudit.lstAudit[i].id_element + "',"
                            + "id_auditratings='" + objAudit.lstAudit[i].id_auditratings + "',comment='" + objAudit.lstAudit[i].comment + "'";
                        if (objAudit.lstAudit[i].evidence_upload != null)
                        {
                            sSqlstmt = sSqlstmt + ", evidence_upload='" + objAudit.lstAudit[i].evidence_upload + "' ";
                        }
                        sSqlstmt = sSqlstmt + " where id_AuditPerformanceChecklist='" + objAudit.lstAudit[i].id_AuditPerformanceChecklist + "';";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditPerformance: " + ex.ToString());
            }
            return false;
        }
    }

    public class AuditModels
    {

        public string AuditID { get; set; }
        public string AuditNum { get; set; }
    }
    public class AuditModelsList
    {
        public List<AuditModels> AuditNoList { get; set; }
    }


    public class AuditElementsModelsList
    {
        public List<AuditElementsModels> lstAuditElement { get; set; }
    }
    public class GenerateAuditChecklistModelsList
    {
        public List<GenerateAuditChecklistModels> AuditCheckList { get; set; }
    }
    public class AuditPerformanceModelsList
    {
        public List<AuditPerformanceModels> lstAudit { get; set; }
    }
}