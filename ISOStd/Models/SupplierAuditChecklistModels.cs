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
    public class SupplierAuditChecklistModels
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

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Business Nature")]
        public string Business { get; set; }

        [Required]
        [Display(Name = "External Provider")]
        public string Supplier { get; set; }

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

        [Required]
        [Display(Name = "Questions")]
        public string Questions { get; set; }


        public string ReportStatus { get; set; }

        internal bool FunAddAuditChecklist(SupplierAuditChecklistModels objchecklist)
        {
            try
            {
                string sSqlstmt = "insert into t_supplier_auditchecklist (Itemno,Department,AuditCriteria,Questions,Business,Supplier) values('" + objchecklist.Itemno + "',"
                + "'" + objchecklist.Department + "','" + objchecklist.AuditCriteria + "','" + objchecklist.Questions + "','" + objchecklist.Business + "','" + objchecklist.Supplier + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditChecklist: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteAuditChecklist(string sid_AuditChecklist)
        {
            try
            {
                string sSqlstmt = "update t_supplier_auditchecklist set Active=0 where id_AuditChecklist='" + sid_AuditChecklist + "'";

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
                string sSqlstmt = "update t_supplier_generateauditchecklist set Active=0 where idt_checklist='" + sidt_checklist + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteChecklist: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAuditPerformance(SupplierAuditChecklistModels objAuditChecklist, SupplierAuditPerformanceModelsList objAudit)
        {
            try
            {
                string sAuditDate = objAuditChecklist.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "insert into t_supplier_generateauditchecklist (id_AuditChecklist,AuditNo,AuditDate,Auditors,Auditee,Notes,Remarks)"
                + "values('" + objAuditChecklist.id_AuditChecklist + "','" + objAuditChecklist.AuditNo + "','" + sAuditDate + "','" + objAuditChecklist.Auditors + "'"
                + ",'" + objAuditChecklist.Auditee + "','" + objAuditChecklist.Notes + "','" + objAuditChecklist.Remarks + "')";


                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {

                    string sqlstmt = "select idt_checklist from t_supplier_generateauditchecklist where AuditNo='" + objAuditChecklist.AuditNo + "'"
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

                    SupplierAuditPerformanceModels objElement = new SupplierAuditPerformanceModels();
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

        internal bool FunUpdateAuditPerformance(SupplierAuditChecklistModels objAuditChecklist, SupplierAuditPerformanceModelsList objAudit)
        {
            try
            {
                string sAuditDate = objAuditChecklist.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_supplier_generateauditchecklist set AuditNo='" + objAuditChecklist.AuditNo + "', "
                        + "Auditors='" + objAuditChecklist.Auditors + "',Auditee='" + objAuditChecklist.Auditee + "',Notes='" + objAuditChecklist.Notes + "'"
                        + ",Remarks='" + objAuditChecklist.Remarks + "'";

                if (objAuditChecklist.AuditDate > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", AuditDate='" + sAuditDate + "' ";
                }
                sSqlstmt = sSqlstmt + " where idt_checklist='" + objAuditChecklist.idt_checklist + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {


                    SupplierAuditPerformanceModels objElement = new SupplierAuditPerformanceModels();
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


        internal bool FunUpdateAuditChecklist(SupplierAuditChecklistModels objAudit, int cnt)
        {
            try
            {
                if (cnt > 0)
                {
                    string sSqlstmt = "update t_supplier_auditchecklist set Department='" + objAudit.Department + "', "
                        + "AuditCriteria='" + objAudit.AuditCriteria + "',Questions='" + objAudit.Questions.TrimEnd(',') + "'";

                    sSqlstmt = sSqlstmt + " where id_AuditChecklist='" + objAudit.id_AuditChecklist + "';";
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
                else
                {

                    string sSqlstmt = "update t_supplier_auditchecklist set Department='" + objAudit.Department + "', "
                          + "AuditCriteria='" + objAudit.AuditCriteria + "'";

                    sSqlstmt = sSqlstmt + " where id_AuditChecklist='" + objAudit.id_AuditChecklist + "';";
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditChecklist: " + ex.ToString());
            }
            return false;

        }
    }
    public class SupplierAuditElementsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Id")]
        public string id_element { get; set; }

        [Required]
        [Display(Name = "Element")]
        [System.Web.Mvc.Remote("doesElementExist", "GenerateAuditChecklist", HttpMethod = "POST", ErrorMessage = "Audit Element already exists. Please enter a different Reference ID.")]
        public string Audit_Elements { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Nature of Business")]
        public string Business { get; set; }

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
                objGlobalData.AddFunctionalLog("Exception in GetAuditRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }
        public string GetAuditQuestionNameById(string sid_element)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Audit_Elements FROM t_supplier_auditelements where id_element='" + sid_element + "'");
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
        public MultiSelectList GetAuditElementsListbox(string Business, string Department)
        {
            SupplierAuditElementsModelsList AuditElemt = new SupplierAuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<SupplierAuditElementsModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_element,Audit_Elements from t_supplier_auditelements where Business='" + Business + "' and  Department='" + Department + "' and Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        SupplierAuditElementsModels data = new SupplierAuditElementsModels()
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


        public MultiSelectList GetAuditElementsListbox()
        {

            SupplierAuditElementsModelsList AuditElemt = new SupplierAuditElementsModelsList();
            AuditElemt.lstAuditElement = new List<SupplierAuditElementsModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_element,Audit_Elements from t_supplier_auditelements where Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        SupplierAuditElementsModels data = new SupplierAuditElementsModels()
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

        internal bool FunAddAuditElements(SupplierAuditElementsModels objElementModels)
        {
            try
            {
                string sSqlstmt = "insert into t_supplier_auditelements (Audit_Elements,Department,Business) values('" + objElementModels.Audit_Elements + "','" + objElementModels.Department + "','" + objElementModels.Business + "')";
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
                string sSqlstmt = "update t_supplier_auditelements set Audit_Elements='" + Audit_Elements + "' where id_element='" + id_element + "'";

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
                string sSqlstmt = "select Audit_Elements from t_supplier_auditelements where Audit_Elements='" + sAudit_Elements + "' and active=1";
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
                string sSqlstmt = "update t_supplier_auditelements set Active=0 where id_element='" + sid_element + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuestions: " + ex.ToString());
            }
            return false;
        }



    }
    public class SupplierAuditPerformanceModels
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


        [Display(Name = "Finding Category")]
        public string finding_cat { get; set; }

        internal bool FunAddAuditPerformance(SupplierAuditPerformanceModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "delete from t_supplier_auditperformancechecklist where idt_checklist='"
                        + objAudit.lstAudit[0].idt_checklist + "'; ";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_supplier_auditperformancechecklist (idt_checklist, id_element, id_auditratings,comment,evidence_upload,finding_cat"
                        + ") values('" + objAudit.lstAudit[0].idt_checklist + "','" + objAudit.lstAudit[i].id_element
                        + "','" + objAudit.lstAudit[i].id_auditratings + "','" + objAudit.lstAudit[i].comment + "','" + objAudit.lstAudit[i].evidence_upload + "','" + objAudit.lstAudit[i].finding_cat + "'); ";
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

        internal bool FunUpdateAuditPerformance(SupplierAuditPerformanceModelsList objAudit)
        {
            try
            {
                if (objAudit.lstAudit.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < objAudit.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_supplier_auditperformancechecklist set id_element='" + objAudit.lstAudit[i].id_element + "',"
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

    
    public class SupplierAuditElementsModelsList
    {
        public List<SupplierAuditElementsModels> lstAuditElement { get; set; }
    }

    public class SupplierAuditChecklistModelsList
    {
        public List<SupplierAuditChecklistModels> AuditCheckList { get; set; }
    }
    public class SupplierAuditPerformanceModelsList
    {
        public List<SupplierAuditPerformanceModels> lstAudit { get; set; }
    }
}