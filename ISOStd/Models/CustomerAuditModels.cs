using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class CustomerAuditModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        public string CustAuditID { get; set; }

        public string CustAudtTransId { get; set; }

        [Required]
        [Display(Name = "Audit Number")]
        public string AuditNum { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Customer Audit Team")]
        public string Customer_Audit_Team { get; set; }

        [Required]
        [Display(Name = "Audit Date")]
        public DateTime AuditDate { get; set; }

        //[Required]
        [Display(Name = "NCR No.")]
        public string NCNo { get; set; }

       // [Required]
        [Display(Name = "Audit Finding")]
        public string AuditFindingDesc { get; set; }

       // [Required]
        [Display(Name = "Correction Taken")]
        public string CorrectionTaken { get; set; }

       // [Required]
        [Display(Name = "Correction Date")]
        public DateTime CorrectionDate { get; set; }

       // [Required]
        [Display(Name = "Proposed corrective Action")]
        public string ProposedcorrectiveAction { get; set; }

       // [Required]
        [Display(Name = "Corrective Action Date")]
        public DateTime CorrectiveActionDate { get; set; }

        //[Required]
        [Display(Name = "Action taken by")]
        public string Action_taken_by { get; set; }

        //[Required]
        [Display(Name = "NCR status")]
        public string NC_status { get; set; }

       // [Required]
        [Display(Name = "Reviewed by")]
        public string reviewed_by { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunDeleteCustomerAuditDoc(string sCustAuditID)
        {
            try
            {
                string sSqlstmt = "update t_customer_audit set Active=0 where CustAuditID='" + sCustAuditID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustomerAuditDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCustomerAudit(CustomerAuditModels objCustomerAudit, CustomerAuditModelsList objCustomerAuditModelsList)
        {
            try
            {
                string sAuditDate = objCustomerAudit.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");// +" " + objInternalAudit.AuditTime;            
               
                string sSqlstmt = "insert into t_customer_audit (AuditNum, CustomerName, Customer_Audit_Team, AuditDate,branch,Department,Location)"
                                    + "values('" + objCustomerAudit.AuditNum + "', '" + objCustomerAudit.CustomerName + "','" + objCustomerAudit.Customer_Audit_Team + "', '"
                                    + sAuditDate + "', '" + objCustomerAudit.branch + "', '" + objCustomerAudit.Department + "', '" + objCustomerAudit.Location + "')";

                return FunAddCustomerAuditFindings(objCustomerAuditModelsList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerAudit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCustomerAuditFindings(CustomerAuditModelsList objCustomerAuditModelsList, int CustAuditID)
        {
            try
            {
                
                string sSqlstmt = "";
                for (int i = 0; i < objCustomerAuditModelsList.CustomerAuditMList.Count; i++)
                {
                    if (objCustomerAuditModelsList.CustomerAuditMList[i].NCNo != null)
                    {
                        string sCorrectionDate = objCustomerAuditModelsList.CustomerAuditMList[i].CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                        string sCorrectiveActionDate = objCustomerAuditModelsList.CustomerAuditMList[i].CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                        sSqlstmt = "insert into t_customer_audit_trans (CustAuditID, NCNo, AuditFindingDesc, CorrectionTaken"
                            + ", CorrectionDate, ProposedcorrectiveAction, CorrectiveActionDate, Action_taken_by, NC_status, reviewed_by";

                        if (objCustomerAuditModelsList.CustomerAuditMList[i].NC_status.ToLower() == "closed")
                        {
                            sSqlstmt = sSqlstmt + ", ClosedDate";
                            //sDynamicColumnVal = sDynamicColumnVal + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                        }
                        sSqlstmt = sSqlstmt + ")values('" + CustAuditID + "', '" + objCustomerAuditModelsList.CustomerAuditMList[i].NCNo + "', '" + objCustomerAuditModelsList.CustomerAuditMList[i].AuditFindingDesc
                                            + "','" + objCustomerAuditModelsList.CustomerAuditMList[i].CorrectionTaken + "', '" + sCorrectionDate
                                            + "','" + objCustomerAuditModelsList.CustomerAuditMList[i].ProposedcorrectiveAction + "', '" + sCorrectiveActionDate
                                            + "', '" + objCustomerAuditModelsList.CustomerAuditMList[i].Action_taken_by + "', '" + objCustomerAuditModelsList.CustomerAuditMList[i].NC_status
                                            + "','" + objCustomerAuditModelsList.CustomerAuditMList[i].reviewed_by + "'";

                        if (objCustomerAuditModelsList.CustomerAuditMList[i].NC_status.ToLower() == "closed")
                        {
                            sSqlstmt = sSqlstmt + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                        }
                        sSqlstmt = sSqlstmt + ");";
                    }
                }

               return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerAuditFindings: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateCustomerAudit(CustomerAuditModels objCustomerAuditModels)
        {
            try
            {
                string sAuditDate = objCustomerAuditModels.AuditDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_customer_audit set AuditNum='" + objCustomerAuditModels.AuditNum + "', CustomerName ='" + objCustomerAuditModels.CustomerName + "', Customer_Audit_Team='"
                    + objCustomerAuditModels.Customer_Audit_Team + "', " + "AuditDate='" + sAuditDate + "', branch ='" + objCustomerAuditModels.branch + "', Department ='" + objCustomerAuditModels.Department + "', Location ='" + objCustomerAuditModels.Location + "' where CustAuditID='" + objCustomerAuditModels.CustAuditID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomerAudit: " + ex.ToString());
            }

            return false;
        }

        internal bool FunUpdateCustomerAuditFindings(CustomerAuditModels objCustomerAuditModels)
        {
            try
            {
                string sCorrectionDate = objCustomerAuditModels.CorrectionDate.ToString("yyyy-MM-dd HH':'mm':'ss");
                string sCorrectiveActionDate = objCustomerAuditModels.CorrectiveActionDate.ToString("yyyy-MM-dd HH':'mm':'ss");

                string sSqlstmt = "update t_customer_audit_trans set NCNo='" + objCustomerAuditModels.NCNo + "', AuditFindingDesc='" + objCustomerAuditModels.AuditFindingDesc
                    + "', CorrectionTaken='" + objCustomerAuditModels.CorrectionTaken + "', CorrectionDate='" + sCorrectionDate
                    + "', ProposedcorrectiveAction='" + objCustomerAuditModels.ProposedcorrectiveAction + "', CorrectiveActionDate='" + objCustomerAuditModels.CorrectiveActionDate
                    + "', Action_taken_by='" + objCustomerAuditModels.Action_taken_by + "', reviewed_by='" + objCustomerAuditModels.reviewed_by + "'";

                if (objCustomerAuditModels.NC_status.ToLower() == "closed")
                {
                    sSqlstmt = sSqlstmt + ", ClosedDate='" + DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss") + "' ";
                }
                sSqlstmt = sSqlstmt + " where CustAudtTransId='" + objCustomerAuditModels.CustAudtTransId + "'";


                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomerAuditFindings: " + ex.ToString());
            }

            return false;
        }
    }
    public class CustomerAuditModelsList
    {
        public List<CustomerAuditModels> CustomerAuditMList { get; set; }
    }

    public class CustomerAudit
    {
        public int CustAuditID { get; set; }
        public string AuditNum { get; set; }
    }

    public class CustomerAuditList
    {
        public List<CustomerAudit> CustomerAuditsList { get; set; }
    }

    public class DropdownCustAuditItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownCustAuditList
    {
        public List<DropdownCustAuditItems> lst { get; set; }
    }
}