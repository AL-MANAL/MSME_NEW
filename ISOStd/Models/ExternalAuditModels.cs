using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class CertificationBodyModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "CertID")]
        public string CertID { get; set; }

        [Required]
        [Display(Name = "Cert Name")]
        public string CertName { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string address { get; set; }

        [Required]
        [Display(Name = "Email address")]
        public string emailaddress { get; set; }

        [Required]
        [Display(Name = "Contact name")]
        public string contact_name { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Fax Number")]
        public string FaxNumber { get; set; }

      
        //--------------------------------------------------------------------------------------
        internal bool FunAddCertificationBody(CertificationBodyModels objCertificationBody)
        {
            try
            {
                string sSqlstmt = "insert into t_certification_body(CertName, Location, address, emailaddress, contact_name, PhoneNumber, FaxNumber) "
                + "values('" + objCertificationBody.CertName + "', '" + objCertificationBody.Location + "', '" + objCertificationBody.address + "', '" + objCertificationBody.emailaddress
                + "', '" + objCertificationBody.contact_name + "', '" + objCertificationBody.PhoneNumber + "', '" + objCertificationBody.FaxNumber + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCertificationBody: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCertificationBody(CertificationBodyModels objCertificationBody)
        {
            try
            {
                string sSqlstmt = "Update t_certification_body set Location='" + objCertificationBody.Location + "', address='" + objCertificationBody.address + "', "
                    + "emailaddress='" + objCertificationBody.emailaddress + "', contact_name='" + objCertificationBody.contact_name + "', PhoneNumber='" + objCertificationBody.PhoneNumber
                    + "', FaxNumber='" + objCertificationBody.FaxNumber + "' where certid='" + objCertificationBody.CertID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCertificationBody: " + ex.ToString());
            }
            return false;
        }


        internal MultiSelectList FunGetCertificateList()
        {
            CertBodyModelsList CertBodylist = new CertBodyModelsList();
            CertBodylist.CertBodyList = new List<CertBody>();
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("SELECT CertName,CertId FROM t_certification_body");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        CertBody objCertBody = new CertBody()
                        {
                            CertId = Convert.ToInt16(dsEmp.Tables[0].Rows[i]["CertId"].ToString()),
                            CertName = dsEmp.Tables[0].Rows[i]["CertName"].ToString()
                        };

                        CertBodylist.CertBodyList.Add(objCertBody);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetCertificateList: " + ex.ToString());
            }
            return new MultiSelectList(CertBodylist.CertBodyList, "CertId", "CertName");
           
        }        
    }

    public class ExternalAuditorModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Required]
        [Display(Name = "Auditor name")]
        public string Auditor_name { get; set; }

        [Required]
        [Display(Name = "CertID")]
        public string CertID { get; set; }

        [Required]
        [Display(Name = "CertName")]
        public string CertName { get; set; }

        [Required]
        [Display(Name = "Email address")]
        public string emailaddress { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Auditor Name")]
        public string auditor_name { get; set; }
        internal bool FunAddInternalAuditor(ExternalAuditorModels objExternalAuditor)
        {
            try
            {
                string sSqlstmt = "insert into t_external_auditor (CertID, emailaddress, Auditor_name, PhoneNumber) values('" + objExternalAuditor.CertID
                    + "','" + objExternalAuditor.emailaddress + "','" + objExternalAuditor.Auditor_name + "','" + objExternalAuditor.PhoneNumber + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddInternalAuditor: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateExternalAuditor(ExternalAuditorModels objExternalAuditorModels)
        {
            try
            {
                string sSqlstmt = "update t_external_auditor set certid =" + objExternalAuditorModels.CertID + ", emailaddress='" + objExternalAuditorModels.emailaddress
                    + "', " + "PhoneNumber='" + objExternalAuditorModels.PhoneNumber + "' where Auditor_name='" + objExternalAuditorModels.Auditor_name + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExternalAuditor: " + ex.ToString());
            }
            return false;
        }

        //Returns all the Auditor details
        internal List<ExternalAuditorModels> FunGetExternalAuditorList()
        {
            ExternalAuditorModelsList objExternalAuditorModelsList = new ExternalAuditorModelsList();
            objExternalAuditorModelsList.ExternalAuditorList = new List<ExternalAuditorModels>();

            try
            {
                string sSqlstmt = "SELECT CertName, tea.Auditor_name, tea.PhoneNumber, tea.emailaddress FROM t_external_auditor as TEA, t_certification_body "
                    + "where tea.certid= t_certification_body.certid";

                DataSet dsExternalAuditorList = objGlobalData.Getdetails(sSqlstmt);
                if (dsExternalAuditorList.Tables.Count > 0 && dsExternalAuditorList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsExternalAuditorList.Tables[0].Rows.Count; i++)
                    {
                        ExternalAuditorModels objExternalAuditorModels = new ExternalAuditorModels
                        {
                            CertID = dsExternalAuditorList.Tables[0].Rows[i]["CertName"].ToString(),
                            Auditor_name = dsExternalAuditorList.Tables[0].Rows[i]["Auditor_name"].ToString(),
                            emailaddress = dsExternalAuditorList.Tables[0].Rows[i]["emailaddress"].ToString(),
                            PhoneNumber = dsExternalAuditorList.Tables[0].Rows[i]["PhoneNumber"].ToString()
                        };
                        objExternalAuditorModelsList.ExternalAuditorList.Add(objExternalAuditorModels);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetExternalAuditorList: " + ex.ToString());
            }

            return objExternalAuditorModelsList.ExternalAuditorList.ToList();
        }

        
    }

    public class ExternalAuditModels
    {
        clsGlobal objGlobalData = new clsGlobal();

       
        [Display(Name = "AuditID")]
        public string AuditID { get; set; }

        [Display(Name = "Certification Body")]
        public string CertID { get; set; }
                
        [Display(Name = "Department")]
        public string DeptId { get; set; }
                
        [Display(Name = "Audit Number")]
        public string AuditNum { get; set; }
               
        //[Display(Name = "Auditor Name(s)")]
        //public string Auditor_name { get; set; }

        //[Required]
        //[Display(Name = "Audit Type")]
        //public string AuditType { get; set; }
                
        [Display(Name = "Audit Date")]
        public DateTime AuditDate { get; set; }

        //[Required]
        [Display(Name = "No. of major audit findings")]
        public int MajorFindingsNo { get; set; }

        //[Required]
        [Display(Name = "No. of minor audit findings")]
        public int MinorFindingNo { get; set; }

        //[Required]
        [Display(Name = "No. of observations")]
        public int ObservationNo { get; set; }
               
        [Display(Name = "NCR No.")]
        public string NCNo { get; set; }
               
        [Display(Name = "Audit Finding")]
        [DataType(DataType.MultilineText)]
        public string AuditFindingDesc { get; set; }
              
        [Display(Name = "Correction Taken")]
        [DataType(DataType.MultilineText)]
        public string CorrectionTaken { get; set; }
                
        [Display(Name = "Correction Date")]
        public DateTime CorrectionDate { get; set; }
              
        [Display(Name = "Proposed Corrective Action")]
        [DataType(DataType.MultilineText)]
        public string ProposedcorrectiveAction { get; set; }
               
        [Display(Name = "Corrective Action Date")]
        public DateTime CorrectiveActionDate { get; set; }
               
        [Display(Name = "Action Taken By")]
        public string Action_taken_by { get; set; }
             
        //[Display(Name = "NCR Status")]
        //public string nc_status { get; set; }
              
        [Display(Name = "Reviewed By")]
        public string reviewed_by { get; set; }
                
        [Display(Name = "Finding Category")]
        public string FindingCategory { get; set; }
                
        [Display(Name = "Division")]
        public string AuditLocation { get; set; }

        [Display(Name = "Upload document(s)")]
        public string upload { get; set; }
    
        public string ExternalAuditID { get; set; }
       
        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Team")]
        public string team { get; set; }

        //[Required]
        //[Display(Name = "Report")]
        //public string Report { get; set; }
        //public string DocUploadPath { get; internal set; }
        //-------------------------------------------t_external_audit--------------------------------------------

        [Display(Name = "Id")]
        public string id_external_audit { get; set; }

        [Display(Name = "Audit No")]
        public string audit_no { get; set; }

        [Display(Name = "Planned Start Date")]
        public DateTime audit_start_date { get; set; }

        [Display(Name = "Planned Completed Date")]
        public DateTime audit_complete_date { get; set; }

        [Display(Name = "Audit Type")]
        public string audit_type { get; set; }

        [Display(Name = "Audit Category")]
        public string audit_category { get; set; }

        [Display(Name = "Audit Criteria")]
        public string audit_criteria { get; set; }

        [Display(Name = "Division")]
        public string directorate { get; set; }

        [Display(Name = "Department")]
        public string group_name { get; set; }

        [Display(Name = "Location")]
        public string location { get; set; }

        [Display(Name = "External Entity Name")]
        public string entity_name { get; set; }

        [Display(Name = "No. of Major NCs")]
        public string major_nc { get; set; }

        [Display(Name = "No. of Minor NCs")]
        public string minor_nc { get; set; }

        [Display(Name = "No. of Observations")]
        public string no_observation { get; set; }

        [Display(Name = "No. of Note-worthy findings")]
        public string no_noteworthy { get; set; }

        [Display(Name = "Upload document(s)")]
        public string status_upload { get; set; }

        [Display(Name = "Audit Status")]
        public string audit_status { get; set; }

        [Display(Name = "Completed On")]
        public DateTime audit_status_date { get; set; }

        [Display(Name = "Notification To")]
        public string notification_to { get; set; }

        [Display(Name = "Company Name")]
        public string company_name { get; set; }

        internal bool FunAddExternalAudit(ExternalAuditModels objAudit, ExternalAuditModelsList objAuditList, ExternalAuditModelsList objAuditList1)
        {
            try
            {

                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string auditstatus = objGlobalData.GetAuditStatusIdByName("Planned");
                string sSqlstmt = "insert into t_external_audit (audit_category,audit_type,audit_criteria,upload,logged_by,entity_name,notification_to,company_name,audit_status";

                string sFields = "", sFieldValue = "";

                if (objAudit.audit_start_date != null && objAudit.audit_start_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", audit_start_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.audit_start_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objAudit.audit_complete_date != null && objAudit.audit_complete_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", audit_complete_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.audit_complete_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + audit_category + "','" + audit_type + "','" + audit_criteria + "','" + upload + "','" + user + "','" + entity_name + "','" + notification_to + "','" + company_name + "','" + auditstatus + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                int id_external_audit = 0;
                if (int.TryParse(objGlobalData.ExecuteQueryReturnId(sSqlstmt).ToString(), out id_external_audit))
                {
                    DataSet dsData = objGlobalData.GetReportNo("EXTAUDIT", "", objGlobalData.GetCompanyBranchNameById(directorate));
                    if (dsData != null && dsData.Tables.Count > 0)
                    {
                        audit_no = dsData.Tables[0].Rows[0]["ReportNO"].ToString();
                    }
                    string sql1 = "update t_external_audit set audit_no='" + audit_no + "' where id_external_audit='" + id_external_audit + "'";
                    objGlobalData.ExecuteQuery(sql1);
                    if (id_external_audit > 0)
                    {
                        objAuditList1.ExternalAuditList[0].id_external_audit = id_external_audit.ToString();
                        FunAddAuditeeList(objAuditList1);

                        objAuditList.ExternalAuditList[0].id_external_audit = id_external_audit.ToString();
                        return FunAddAuditorList(objAuditList);                     
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExternalAudit: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddAuditeeList(ExternalAuditModelsList objAuditList)
        {
            try
            {

                string sSqlstmt = "delete from t_external_auditee where id_external_audit='" + objAuditList.ExternalAuditList[0].id_external_audit + "'; ";

                for (int i = 0; i < objAuditList.ExternalAuditList.Count; i++)
                {
                    string sid_external_auditee = "null";
                    sSqlstmt = sSqlstmt + "insert into t_external_auditee(id_external_auditee,id_external_audit,directorate,group_name,location";

                    string sFieldValue = "", sFields = "", sValue = "";
                    if (objAuditList.ExternalAuditList[i].id_external_auditee != null)
                    {
                        sid_external_auditee = objAuditList.ExternalAuditList[i].id_external_auditee;
                    }

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_external_auditee + ",'" + objAuditList.ExternalAuditList[0].id_external_audit + "','" + objAuditList.ExternalAuditList[i].directorate + "','" + objAuditList.ExternalAuditList[i].group_name + "','" + objAuditList.ExternalAuditList[i].location + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_external_auditee= values(id_external_auditee),id_external_audit= values(id_external_audit), directorate= values(directorate), group_name = values(group_name), location= values(location)";
                    sSqlstmt = sSqlstmt + sValue + ";";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditorList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunAddAuditorList(ExternalAuditModelsList objAuditList)
        {
            try
            {

                string sSqlstmt = "delete from t_external_auditor where id_external_audit='" + objAuditList.ExternalAuditList[0].id_external_audit + "'; ";

          
                for (int i = 0; i < objAuditList.ExternalAuditList.Count; i++)
                {
                    string sid_external_audit = "null";
                    sSqlstmt = sSqlstmt + "insert into t_external_auditor(id_external_auditor,id_external_audit,auditor_name,auditor_level,contact_no,email_address";

                    string sFieldValue = "", sFields = "", sValue = "";
                    if (objAuditList.ExternalAuditList[i].id_external_audit != null)
                    {
                        sid_external_audit = objAuditList.ExternalAuditList[i].id_external_audit;
                    }                   

                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values(" + sid_external_audit + ",'" + objAuditList.ExternalAuditList[0].id_external_audit + "','" + objAuditList.ExternalAuditList[i].auditor_name + "','" + objAuditList.ExternalAuditList[i].auditor_level + "','" + objAuditList.ExternalAuditList[i].contact_no + "','" + objAuditList.ExternalAuditList[i].email_address + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ")";
                    sValue = " ON DUPLICATE KEY UPDATE "
                    + " id_external_auditor= values(id_external_auditor),id_external_audit= values(id_external_audit), auditor_name= values(auditor_name), auditor_level = values(auditor_level), contact_no= values(contact_no), email_address= values(email_address)";
                
                    sSqlstmt = sSqlstmt + sValue + ";";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditorList: " + ex.ToString());
            }
            return false;
        }
        internal bool FunDeleteExternalDoc(string sid_external_audit)
        {
            try
            {
                string sSqlstmt = "update t_external_audit set Active=0 where id_external_audit='" + sid_external_audit + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteExternalDoc: " + ex.ToString());
            }
            return false;
        }
        internal bool FunEditExternalAudit(ExternalAuditModels objAudit, ExternalAuditModelsList objAuditList, ExternalAuditModelsList objAuditList1)
        {
            try
            {
                
                string sSqlstmt = "update t_external_audit set audit_type='" + audit_type + "',audit_criteria='" + audit_criteria + "',upload='" + upload + "',entity_name='" + entity_name + "',notification_to='" + notification_to + "'";

                if (objAudit.audit_start_date != null && objAudit.audit_start_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",audit_start_date='" + objAudit.audit_start_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objAudit.audit_complete_date != null && objAudit.audit_complete_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",audit_complete_date='" + objAudit.audit_complete_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_external_audit='" + objAudit.id_external_audit + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                   
                    if (objAuditList.ExternalAuditList.Count > 0)
                    {
                        objAuditList1.ExternalAuditList[0].id_external_audit = id_external_audit.ToString();
                        FunAddAuditeeList(objAuditList1);

                        objAuditList.ExternalAuditList[0].id_external_audit = id_external_audit.ToString();
                        return FunAddAuditorList(objAuditList);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExternalAudit: " + ex.ToString());
            }
            return false;
        }
        //Audit status update
        internal bool FunUpdateAuditStatus(ExternalAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_external_audit set audit_status='" + audit_status + "',remarks='" + remarks + "',major_nc='" + major_nc + "',minor_nc='" + minor_nc + "',no_observation='" + no_observation + "',no_noteworthy='" + no_noteworthy + "',status_upload='" + status_upload + "'";
                if (objAudit.audit_status_date != null && objAudit.audit_status_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",audit_status_date='" + objAudit.audit_status_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_external_audit='" + objAudit.id_external_audit + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditStatus: " + ex.ToString());
            }
            return false;
        }
        //--------------------------------------t_external_auditor------------------------------------------
        [Display(Name = "Id")]
        public string id_external_auditor { get; set; }

        [Display(Name = "Id")]
        public string id_external_auditee { get; set; }

        [Display(Name = "Auditor Name")]
        public string auditor_name { get; set; }

        [Display(Name = "Auditor Level")]
        public string auditor_level { get; set; }

        [Display(Name = "Contact No")]
        public string contact_no { get; set; }

        [Display(Name = "Email Address")]
        public string email_address { get; set; }

        //--------------------------------------t_external_audit_nc-----------------------------------------------------
        [Display(Name = "Id")]
        public string id_nc { get; set; }

        [Display(Name = "NC No")]
        public string nc_no { get; set; }

        [Display(Name = "Corrective Action Agreed Date")]
        public DateTime corrective_agreed_date { get; set; }

        [Display(Name = "Finding Detail")]
        public string finding_details { get; set; }

        [Display(Name = "Finding Category")]
        public string finding_category { get; set; }

        [Display(Name = "Why finding is categorized as NC / PNC?")]
        public string why_finding { get; set; }

        public string logged_by { get; set; }

        public DateTime logged_date { get; set; }

        [Display(Name = "NC Status")]
        public string nc_status { get; set; }

        [Display(Name = "Remarks")]
        public string nc_status_remarks { get; set; }

        //Raise NC
        internal bool FunAddNonconformity(ExternalAuditModels objAudit)
        {
            try
            {
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string sql = "select id_nc from t_external_audit_nc where id_external_audit='" + id_external_audit + "'";
                DataSet dsData = objGlobalData.Getdetails(sql);
                int count = dsData.Tables[0].Rows.Count + 1;
                string nc_status = objGlobalData.GetNCStatusIdByName("Open");
                nc_no = audit_no + " - " + count;

                string sSqlstmt = "insert into t_external_audit_nc (id_external_audit,nc_no,finding_details,why_finding,upload,finding_category,logged_by,nc_status";

                string sFields = "", sFieldValue = "";

                if (objAudit.corrective_agreed_date != null && objAudit.corrective_agreed_date > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", corrective_agreed_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.corrective_agreed_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + id_external_audit + "','" + nc_no + "','" + finding_details + "','" + why_finding + "','" + upload + "','" + finding_category + "','" + user + "','" + nc_status + "'";

                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddNonconformity: " + ex.ToString());
            }
            return false;
        }

        //NC update
        internal bool FunUpdateNonconformity(ExternalAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set finding_details='" + finding_details + "',why_finding='" + why_finding + "',upload='" + upload + "',finding_category='" + finding_category + "'";
                if (objAudit.corrective_agreed_date != null && objAudit.corrective_agreed_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",corrective_agreed_date='" + objAudit.corrective_agreed_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_nc='" + objAudit.id_nc + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNonconformity: " + ex.ToString());
            }
            return false;
        }

        //NC status update
        internal bool FunUpdateNCStatus(ExternalAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_external_audit_nc set nc_status='" + nc_status + "',nc_status_remarks='" + nc_status_remarks + "'";
              
                sSqlstmt = sSqlstmt + " where id_nc='" + objAudit.id_nc + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateNCStatus: " + ex.ToString());
            }
            return false;
        }
        //---------------------------------------------------------------------------------------------
        internal bool FunAddExternalAuditWithNoFindings(ExternalAuditModels objExternalAudit, HttpPostedFileBase filepath)
        {
            try
            {
                string sAuditDate = objExternalAudit.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;
                
                string sSqlstmt = "insert into t_external_audit (CertID, AuditNum, AuditDate, MajorFindingsNo, MinorFindingNo, ObservationNo, Auditor_name, AuditLocation,remarks,upload)"
                                    + " values('" + objExternalAudit.CertID + "', '" + objExternalAudit.AuditNum + "','" + sAuditDate + "', '" + objExternalAudit.MajorFindingsNo
                                    + "', '" + objExternalAudit.MinorFindingNo + "', '" + objExternalAudit.ObservationNo + "','" + objExternalAudit.auditor_name
                                    + "','" + objExternalAudit.AuditLocation + "','" + objExternalAudit.remarks + "','" + objExternalAudit.upload + "')";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddExternalAuditWithNoFindings: " + ex.ToString());
            }
            return false;
        }

        //internal bool FunAddExternalAudit(ExternalAuditModels objExternalAudit, ExternalAuditModelsList objExternalAuditModelsList)
        //{
        //    try
        //    {
        //        string sAuditDate = objExternalAudit.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;
        //      //  string sBranch = objGlobalData.GetCurrentUserSession().division;

        //        string sSqlstmt = "insert into t_external_audit (CertID, AuditNum, AuditDate, MajorFindingsNo, MinorFindingNo, ObservationNo, Auditor_name, AuditLocation, upload)"
        //                            + " values('" + objExternalAudit.CertID + "', '" + objExternalAudit.AuditNum + "','" + sAuditDate + "', '" + objExternalAudit.MajorFindingsNo
        //                            + "', '" + objExternalAudit.MinorFindingNo + "', '" + objExternalAudit.ObservationNo + "','" + objExternalAudit.Auditor_name
        //                            + "','" + objExternalAudit.AuditLocation + "','" + objExternalAudit.upload + "')";

        //        return FunAddExternalAuditFindings(objExternalAuditModelsList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in FunAddExternalAudit: " + ex.ToString());
        //    }
        //    return false;
        //}

         internal bool FunAddExternalAuditFindings(ExternalAuditModelsList objExternalAuditModelsList, int ExternalAuditID)
         {
             try
             {
                 string sSqlstmt = "";
                 for (int i = 0; i < objExternalAuditModelsList.ExternalAuditList.Count; i++)
                 {
                     if (objExternalAuditModelsList.ExternalAuditList[i].NCNo != null)
                     {
                         string sCorrectionDate = objExternalAuditModelsList.ExternalAuditList[i].CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                         string sCorrectiveActionDate = objExternalAuditModelsList.ExternalAuditList[i].CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                         sSqlstmt = sSqlstmt + "insert into t_external_audit_trans (ExternalAuditID, DeptId, NCNo, AuditFindingDesc, FindingCategory, CorrectionTaken, CorrectionDate, "
                                            + " ProposedcorrectiveAction, CorrectiveActionDate, Action_taken_by, nc_status, reviewed_by,team";
                         if (objExternalAuditModelsList.ExternalAuditList[i].nc_status.ToLower() == "closed")
                         {
                             sSqlstmt = sSqlstmt + ", ClosedDate";
                             //sDynamicColumnVal = sDynamicColumnVal + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                         }
                         sSqlstmt = sSqlstmt + ") values('" + ExternalAuditID + "', '" + objExternalAuditModelsList.ExternalAuditList[i].DeptId 
                            +"','"+ objExternalAuditModelsList.ExternalAuditList[i].NCNo
                            + "', '" + objExternalAuditModelsList.ExternalAuditList[i].AuditFindingDesc + "', '" + objExternalAuditModelsList.ExternalAuditList[i].FindingCategory
                            + "', '" + objExternalAuditModelsList.ExternalAuditList[i].CorrectionTaken + "', '" + sCorrectionDate
                            + "', '" + objExternalAuditModelsList.ExternalAuditList[i].ProposedcorrectiveAction
                            + "', '" + sCorrectiveActionDate + "', '" + objExternalAuditModelsList.ExternalAuditList[i].Action_taken_by + "', '"
                            + objExternalAuditModelsList.ExternalAuditList[i].nc_status + "','" + objExternalAuditModelsList.ExternalAuditList[i].reviewed_by + "','" + objExternalAuditModelsList.ExternalAuditList[i].team + "'";
                         
                         if (objExternalAuditModelsList.ExternalAuditList[i].nc_status.ToLower() == "closed")
                         {
                             sSqlstmt = sSqlstmt + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                         }
                         sSqlstmt = sSqlstmt + "); ";
                     }

                 }

                 if (sSqlstmt != "")
                 {
                     return objGlobalData.ExecuteQuery(sSqlstmt);
                 }
             }
             catch (Exception ex)
             {
                 objGlobalData.AddFunctionalLog("Exception in FunAddExternalAuditFindings: " + ex.ToString());
             }

             return false;
         }

        internal bool FunUpdateExternalAudit(ExternalAuditModels objExternalAuditModels)
        {
            try
            {
                string sAuditDate = objExternalAuditModels.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;           

                string sSqlstmt = "update t_external_audit set certid ='" + objExternalAuditModels.CertID + "', AuditDate='" + sAuditDate
                    + "', MajorFindingsNo='" + objExternalAuditModels.MajorFindingsNo + "', MinorFindingNo='" + objExternalAuditModels.MinorFindingNo
                    + "', ObservationNo='" + objExternalAuditModels.ObservationNo + "', AuditNum='" + objExternalAuditModels.AuditNum
                    + "', Auditor_name='" + objExternalAuditModels.auditor_name + "', AuditLocation='" + objExternalAuditModels.AuditLocation
                    + "', upload='" + objExternalAuditModels.upload+ "' where AuditID='" + objExternalAuditModels.AuditID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExternalAudit: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateExternalAuditFindings(ExternalAuditModels objExternalAuditModels)
        {
            try
            {
                string sCorrectionDate = objExternalAuditModels.CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCorrectiveActionDate = objExternalAuditModels.CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_external_audit_trans set DeptId='" + objExternalAuditModels.DeptId + "', NCNo ='" + objExternalAuditModels.NCNo
                    + "', AuditFindingDesc='" + objExternalAuditModels.AuditFindingDesc
                    + "', FindingCategory='" + objExternalAuditModels.FindingCategory + "', CorrectionTaken='" + objExternalAuditModels.CorrectionTaken
                    + "', CorrectionDate='" + sCorrectionDate + "', ProposedcorrectiveAction='" + objExternalAuditModels.ProposedcorrectiveAction
                    + "', CorrectiveActionDate='" + sCorrectiveActionDate + "', Action_taken_by='" + objExternalAuditModels.Action_taken_by
                    + "', nc_status='" + objExternalAuditModels.nc_status + "', reviewed_by='" + objExternalAuditModels.reviewed_by + "', team='" + objExternalAuditModels.team + "'";


                if (objExternalAuditModels.nc_status.ToLower() == "closed")
                {
                    sSqlstmt = sSqlstmt + ", ClosedDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                sSqlstmt = sSqlstmt + " where ExtAuditTransID='" + objExternalAuditModels.ExternalAuditID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateExternalAuditFindings: " + ex.ToString());
            }

            return false;
        }

        public List<string> FunGetExternalAuditorsListbox(string sCertId)
        {

            List<string> lstAuditorList = new List<string>();
            try
            {
                string sSqlstmt = "SELECT Auditor_name, AuditorID FROM t_external_auditor where certid=" + sCertId;

                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    lstAuditorList = dsData.Tables[0].AsEnumerable().Select(r => r[0].ToString()).ToList();
                }
                
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunGetExternalAuditorsListbox: " + ex.ToString());
            }
            return lstAuditorList;
        }

        public string GetFindingCategoryNameById(string sCat)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Audit Finding Category' and (item_id='" + sCat + "' or item_desc='" + sCat + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetFindingCategoryNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiFindingCategoryList()
        {
            DropdownExtAuditList lst = new DropdownExtAuditList();
            lst.lst = new List<DropdownExtAuditItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Audit Finding Category' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownExtAuditItems reg = new DropdownExtAuditItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiFindingCategoryList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

        internal string GetAuditStatusNameById(string sAudit)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Audit Status' and (item_id='" + sAudit + "' or item_desc='" + sAudit + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditStatusNameById: " + ex.ToString());
            }
            return "";
        }

        public MultiSelectList GetMultiAuditStatusList()
        {
            DropdownExtAuditList lst = new DropdownExtAuditList();
            lst.lst = new List<DropdownExtAuditItems>();
            try
            {

                string sSqlstmt = "select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                    + "and header_desc='Audit Status' order by item_desc asc";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsEmp.Tables[0].Rows.Count; i++)
                    {
                        DropdownExtAuditItems reg = new DropdownExtAuditItems()
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
                objGlobalData.AddFunctionalLog("Exception in GetMultiAuditStatusList: " + ex.ToString());
            }

            return new MultiSelectList(lst.lst, "Id", "Name");
        }

    }


    public class CertificationBodyModelsList
    {
        public List<CertificationBodyModels> CertificationBodyList { get; set; }
    }

    public class ExternalAuditorModelsList
    {
        public List<ExternalAuditorModels> ExternalAuditorList { get; set; }
    }

    public class ExternalAuditModelsList
    {
        public List<ExternalAuditModels> ExternalAuditList { get; set; }
    }

    public class CertBody
    {
        public int CertId { get; set; }
        public string CertName { get; set; }
    }

    public class CertBodyModelsList
    {
        public List<CertBody> CertBodyList { get; set; }
    }

    public class ExternalAuditors
    {
        public int AuditorID { get; set; }
        public string Auditor_name { get; set; }
    }

    public class ExternalAuditorsModelsList
    {
        public List<ExternalAuditors> ExternalAuditorsList { get; set; }
    }

    public class ExternalAudit
    {
        public int AuditID { get; set; }
        public string AuditNum { get; set; }
    }

    public class ExternalAuditList
    {
        public List<ExternalAudit> ExternalAuditsList { get; set; }
    }

    public class DropdownExtAuditItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownExtAuditList
    {
        public List<DropdownExtAuditItems> lst { get; set; }
    }
}