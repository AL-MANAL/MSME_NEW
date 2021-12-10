using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace ISOStd.Models
{
    public class SupplierModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string SupplierId { get; set; }

        [Required]
        [Display(Name = "External Provider Code")]
        [System.Web.Mvc.Remote("doesSupplierCodeExist", "Supplier", HttpMethod = "POST", ErrorMessage = "Supplier Code already exists. Please enter a different name.")]
        public string SupplierCode { get; set; }

        [Required]
        [Display(Name = "External Provider Name")]
        [System.Web.Mvc.Remote("doesSupplierNameExist", "Supplier", HttpMethod = "POST", ErrorMessage = "Supplier Name already exists. Please enter a different name.")]
        public string SupplierName { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Scope of Supply")]
        public string SupplyScope { get; set; }


        [Display(Name = "Approval Criteria")]
        public string ApprovalCriteria { get; set; }


        [Display(Name = "Supporting Document")]
        public string SupportingDoc { get; set; }


        [Display(Name = "Approved On")]
        public DateTime ApprovedOn { get; set; }


        [Display(Name = "Approval By")]
        public string ApprovedBy { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }


        [Display(Name = "Approved Status")]
        public string ApprovedStatus { get; set; }


        [Display(Name = "Added/Updated By")]
        public string Added_Updated_By { get; set; }


        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }


        [Display(Name = "City")]
        public string City { get; set; }


        [Display(Name = "Country")]
        public string Country { get; set; }


        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }


        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Fax No.")]
        public string FaxNo { get; set; }

        [Display(Name = "Pin No")]
        public string PO_No { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "GST No")]
        public string VatNo { get; set; }

        [Display(Name = "Approval Reference No")]
        public string RefNo { get; set; }

        [Display(Name = "External Service Provider")]
        public string Supplier_type { get; set; }

        [Display(Name = "Payment Term")]

        public string Payment_term { get; set; }

        [Display(Name = "License Expires On")]
        public DateTime License_Expires { get; set; }

        [Display(Name = "Email Notification Period")]
        public string NotificationPeriod { get; set; }

        [Display(Name = "Notification Value")]
        public string NotificationValue { get; set; }

        [Display(Name = "Notification Days")]
        public int NotificationDays { get; set; }

        [Display(Name = "External Provider Criticality")]
        public string Criticality { get; set; }

        [Display(Name = "External Provider Type")]
        public string Supplier_Work_Nature { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "PAN No")]
        public string pan_no { get; set; }

        internal bool FunInvalidSupplier(string sSupplierID, string invalid_reason)
        {
            try
            {
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;
                string TodayDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sSqlstmt = "update t_supplier set chk_valid='Invalid',invalid_reason='" + invalid_reason + "',invalid_date='" + TodayDate + "',invalid_logged_by='" + user + "' where SupplierID='" + sSupplierID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunInvalidSupplier: " + ex.ToString());
            }
            return false;
        }

        public bool checkSupplierNameExists(string SupplierName)
        {
            try
            {
                string sSqlstmt = "select SupplierId from t_supplier where SupplierName='" + SupplierName + "'";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkSupplierNameExists: " + ex.ToString());
            }
            return true;
        }

        public bool checkSupplierCodeExists(string SupplierCode)
        {
            try
            {
                string sSqlstmt = "select SupplierId from t_supplier where SupplierCode='" + SupplierCode + "'";
                DataSet dsData = objGlobalData.Getdetails(sSqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkSupplierCodeExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunAddSupplier(SupplierModels objSupplierModels)
        {
            try
            {
                string dtDateOfExpiry = objSupplierModels.License_Expires.ToString("yyyy/MM/dd HH:mm");
               // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_supplier (SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, ApprovedBy, Remarks, Added_Updated_By, ApprovedOn,"
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,UpdatedOn,Email,VatNo,RefNo,Supplier_type,Payment_term,License_Expiry,NotificationDays," +
                    "NotificationPeriod,NotificationValue,Criticality,branch,Supplier_Work_Nature,Department,Location,pan_no";
                string UpdateOn = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                if (objSupplierModels.SupportingDoc != null && objSupplierModels.SupportingDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", SupportingDoc";
                }
                sSqlstmt = sSqlstmt + ") values('" + objSupplierModels.SupplierCode + "','" + objSupplierModels.SupplierName + "','" + objSupplierModels.SupplyScope
                        + "','" + objSupplierModels.ApprovalCriteria + "','" + objSupplierModels.ApprovedBy + "','" + objSupplierModels.Remarks
                        + "','" + objSupplierModels.Added_Updated_By + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                        + "','" + objSupplierModels.City + "','" + objSupplierModels.Country + "','" + objSupplierModels.ContactPerson + "','" + objSupplierModels.ContactNo + "','" + objSupplierModels.Address
                        + "','" + objSupplierModels.FaxNo + "','" + objSupplierModels.PO_No + "','" + UpdateOn + "','" + objSupplierModels.Email + "','" + objSupplierModels.VatNo + "','" + objSupplierModels.RefNo + "',"
                        + "'" + objSupplierModels.Supplier_type + "','" + objSupplierModels.Payment_term + "','" + dtDateOfExpiry + "','" + objSupplierModels.NotificationDays + "','" + objSupplierModels.NotificationPeriod 
                        + "','" + objSupplierModels.NotificationValue + "','" + objSupplierModels.Criticality + "','" + objSupplierModels.branch + "','" + objSupplierModels.Supplier_Work_Nature
                        + "','" + objSupplierModels.Department + "','" + objSupplierModels.Location + "','" + objSupplierModels.pan_no + "'";

                if (objSupplierModels.SupportingDoc != null && objSupplierModels.SupportingDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objSupplierModels.SupportingDoc + "'";
                }
                sSqlstmt = sSqlstmt + ")";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    string sUsername = objGlobalData.GetEmpHrNameById(objSupplierModels.ApprovedBy);
                    string sEmailid = objGlobalData.GetMultiHrEmpEmailIdById(objSupplierModels.ApprovedBy);
                    if (sEmailid != null && sEmailid != "")
                    {
                        string sExtraMsg = "There is Supplier details Pending for your approval,Supplier Name: " + objSupplierModels.SupplierName;
                        //Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(objGlobalData.GetEmpHrNameById(objSupplierModels.ApprovedBy), "supplierdoc", sExtraMsg);

                        Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody(sUsername, "supplierdoc", sExtraMsg);
                        return objGlobalData.Sendmail(sEmailid, dicEmailContent["subject"], dicEmailContent["body"], "");
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplier: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSupplier(SupplierModels objSupplierModels)
        {
            try
            {
                string dtDateOfExpiry = objSupplierModels.License_Expires.ToString("yyyy/MM/dd HH:mm");
                string user = "";
                user = objGlobalData.GetCurrentUserSession().empid;

                string sSqlstmt = "update t_supplier set SupplyScope ='" + objSupplierModels.SupplyScope + "', ApprovalCriteria='" + objSupplierModels.ApprovalCriteria + "', "
                    + "ApprovedBy='" + objSupplierModels.ApprovedBy + "', Remarks='" + objSupplierModels.Remarks + "', UpdatedOn='"
                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "', Added_Updated_By='" + user
                     + "', City='" + objSupplierModels.City + "', Country='" + objSupplierModels.Country + "', ContactPerson='" + objSupplierModels.ContactPerson + "', ContactNo='" + objSupplierModels.ContactNo
                     + "', Address='" + objSupplierModels.Address + "', FaxNo='" + objSupplierModels.FaxNo + "', PO_No='" + objSupplierModels.PO_No + "', Email='" + objSupplierModels.Email + "', VatNo='" + objSupplierModels.VatNo + "'"
                    + ", RefNo='" + objSupplierModels.RefNo + "', Supplier_type='" + objSupplierModels.Supplier_type + "', Payment_term='" + objSupplierModels.Payment_term + "', License_Expiry='" + dtDateOfExpiry
                    + "', NotificationDays='" + objSupplierModels.NotificationDays + "', NotificationPeriod='" + objSupplierModels.NotificationPeriod + "', NotificationValue='" + objSupplierModels.NotificationValue 
                    + "', Criticality='" + objSupplierModels.Criticality + "', Supplier_Work_Nature='" + objSupplierModels.Supplier_Work_Nature
                    + "', branch='" + objSupplierModels.branch + "', Department='" + objSupplierModels.Department + "', Location='" + objSupplierModels.Location + "', pan_no='" + pan_no + "'";

                if (objSupplierModels.SupportingDoc != null && objSupplierModels.SupportingDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", SupportingDoc='" + objSupplierModels.SupportingDoc + "' ";
                }
                sSqlstmt = sSqlstmt + " where SupplierId='" + objSupplierModels.SupplierId + "'";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplier: " + ex.ToString());
            }
            return false;
        }

        internal bool FunApproveSupplier(string SupplierID, int iStatus)
        {
            try
            {
                string sSqlstmt = "update t_supplier set ApprovedStatus='" + iStatus + "', ApprovedOn='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' where SupplierId='" + SupplierID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunApproveSupplier: " + ex.ToString());
            }
            return false;

        }

        internal bool FunDeleteSupplier(string sId)
        {
            try
            {
                string sSqlstmt = "update t_supplier set Active=0 where SupplierId=" + sId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSupplier: " + ex.ToString());
            }
            return false;
        }

        public bool checkApproverValid(string Empid, string SupplierId)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select SupplierName from t_supplier where ApprovedBy='" + Empid + "' and SupplierId='" + SupplierId + "'");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkApproverValid: " + ex.ToString());
            }
            return false;
        }
        
        internal string GetApprovalCriteriaNameById(string sCriteriaId)
        {
            try
            {
                DataSet dsEmp = objGlobalData.Getdetails("select item_id as Id, item_desc as Name from dropdownitems, dropdownheader where dropdownheader.header_id=dropdownitems.header_id "
                       + "and header_desc='Supplier-ApprovalCriteria' and (item_id='" + sCriteriaId + "' or item_desc='" + sCriteriaId + "')");
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return (dsEmp.Tables[0].Rows[0]["Name"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetApprovalCriteriaNameById: " + ex.ToString());
            }
            return "";
        }
    }

    public class SupplierModelsList
    {
        public List<SupplierModels> lstSupplier { get; set; }
    }

    public class SupplierList
    {
        public List<Suppliers> lstSupplier { get; set; }
    }

    public class Suppliers
    {
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
    }

    public class SupplierDiscrepencyLogModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string SupplierId { get; set; }

        [Display(Name = "Id")]
        public string SupplierDiscreLogId { get; set; }

        [Required]
        [Display(Name = "Discrepancy Logged Date")]
        public DateTime Discrepency_LoggedDate { get; set; }

        [Required]
        [Display(Name = "PO Number")]
        public string PONo { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description of Supplier Discrepancies")]
        public string Discrepency_Desc { get; set; }

        [Required]
        [Display(Name = "Action Taken")]
        public string ActionTaken { get; set; }

        [Display(Name = "Upload")]
        public string Upload { get; set; }


        //supplier discrepancy
        [Display(Name = "External Provider Type")]
        public string providertype { get; set; }

        [Display(Name = "P.O.Date")]
        public DateTime po_date { get; set; }

        [Display(Name = "NC Identified during")]
        public string nc_identified { get; set; }

        [Display(Name = "Type of Discrepancies")]
        public string discrepancy_type { get; set; }

        [Display(Name = "NCR Ref.")]
        public string ncr_ref { get; set; }

        [Display(Name = "Supplier Ref")]
        public string supplier_ref { get; set; }

        [Display(Name = "Any Impact on customer requirements")]
        public string impact { get; set; }

        [Display(Name = "Status")]
        public string disc_status { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        internal bool FunDeleteDiscrepencyLogDoc(string sSupplierDiscreLogId)
        {
            try
            {
                string sSqlstmt = "update t_supplier_discrepancylog set Active=0 where SupplierDiscreLogId='" + sSupplierDiscreLogId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteDiscrepencyLogDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddSupplierDiscrepencyLog(SupplierDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
                string dtDiscrepency_LoggedDate = objDescripencyLog.Discrepency_LoggedDate.ToString("yyyy/MM/dd HH:mm");
               // string sBranch = objGlobalData.GetCurrentUserSession().division;

                string sSqlstmt = "insert into t_supplier_discrepancylog (SupplierId, Discrepency_LoggedDate, PONo, Discrepency_Desc," +
                    " ActionTaken,Upload,providertype,nc_identified,discrepancy_type,ncr_ref,supplier_ref,impact,disc_status,remarks,branch,Department,Location";
                string sFields = "", sFieldValue = "";

                if (objDescripencyLog.po_date != null && objDescripencyLog.po_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", po_date";
                    sFieldValue = sFieldValue + ", '" + objDescripencyLog.po_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;

                sSqlstmt = sSqlstmt + ") values('" + objDescripencyLog.SupplierId + "','" + dtDiscrepency_LoggedDate + "','" + objDescripencyLog.PONo
                        + "','" + objDescripencyLog.Discrepency_Desc + "','" + objDescripencyLog.ActionTaken + "','" + objDescripencyLog.Upload 
                        + "','" + objDescripencyLog.providertype + "','" + objDescripencyLog.nc_identified + "','" + objDescripencyLog.discrepancy_type 
                        + "','" + objDescripencyLog.ncr_ref + "','" + objDescripencyLog.supplier_ref + "','" + objDescripencyLog.impact + "','"
                        + objDescripencyLog.disc_status + "','" + objDescripencyLog.remarks + "','" + objDescripencyLog.branch + "','" + objDescripencyLog.Department + "','" + objDescripencyLog.Location + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplierDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateSupplierDiscrepencyLog(SupplierDiscrepencyLogModels objDescripencyLog)
        {
            try
            {
                string dtDiscrepency_LoggedDate = objDescripencyLog.Discrepency_LoggedDate.ToString("yyyy/MM/dd HH:mm");

                string sSqlstmt = "update t_supplier_discrepancylog set Discrepency_LoggedDate='" + dtDiscrepency_LoggedDate + "', PONo='" + objDescripencyLog.PONo
                    + "', Discrepency_Desc='" + objDescripencyLog.Discrepency_Desc + "', ActionTaken='" + objDescripencyLog.ActionTaken + "', providertype='" + objDescripencyLog.providertype + "', nc_identified='" + objDescripencyLog.nc_identified + "', discrepancy_type='" + objDescripencyLog.discrepancy_type + "'"
                    + ", ncr_ref='" + objDescripencyLog.ncr_ref + "', supplier_ref='" + objDescripencyLog.supplier_ref + "', impact='" + objDescripencyLog.impact + "', disc_status='" + objDescripencyLog.disc_status + "', remarks='" + objDescripencyLog.remarks
                    + "', branch='" + objDescripencyLog.branch + "', Department='" + objDescripencyLog.Department + "', Location='" + objDescripencyLog.Location + "'";

                if (objDescripencyLog.po_date != null && objDescripencyLog.po_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", po_date='" + objDescripencyLog.po_date.ToString("yyyy/MM/dd") + "'";
                }
                if (objDescripencyLog.Upload != null)
                {
                    sSqlstmt = sSqlstmt + ", Upload='" + objDescripencyLog.Upload + "'";
                }
                sSqlstmt = sSqlstmt + " where SupplierDiscreLogId='" + objDescripencyLog.SupplierDiscreLogId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateSupplierDiscrepencyLog: " + ex.ToString());
            }
            return false;
        }
    }

    public class DescripencyLogModelsList
    {
        public List<SupplierDiscrepencyLogModels> lstDescripencyLog { get; set; }
    }

    public class DropdownSupplierItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownSupplierList
    {
        public List<DropdownSupplierItems> lst { get; set; }
    }

}