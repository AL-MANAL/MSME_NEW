using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;
using System.IO;

namespace ISOStd.Models
{
    public class CustomerVisitModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string CustomerVisitId { get; set; }

        [Display(Name = "Division")]
        public string Branch { get; set; }

        [Display(Name = "Planned Date")]
        public DateTime PlannedDate { get; set; }

        [Display(Name = "Visit Date")]
        public DateTime DateOfVisit { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailID { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Product")]
        public string Product { get; set; }

        [Display(Name = "Follow Up Date")]
        public DateTime FollowUp_Date { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Outcome of Visit")]
        public string Outcomeof_Visit { get; set; }
        
        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        
        [Display(Name = "Processed By")]
        public string ProcessedBy { get; set; }

        [Display(Name = "Document")]
        public string DocUploadPath { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        internal bool FunDeleteCustomerVisitDoc(string sCustomerVisitId)
        {
            try
            {
                string sSqlstmt = "update t_customervisit set Active=0 where CustomerVisitId='" + sCustomerVisitId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustomerVisitDoc: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddCustomerVisit(CustomerVisitModels objCustomerVisitModels)
        {
            try
            {
               // string sBranch = objGlobalData.GetCurrentUserSession().division;
                string dtPlannedDate = objCustomerVisitModels.PlannedDate.ToString("yyyy/MM/dd HH:mm");
                string dtDateOfVisit = objCustomerVisitModels.DateOfVisit.ToString("yyyy/MM/dd HH:mm");
                string dtFollowUp_Date = objCustomerVisitModels.FollowUp_Date.ToString("yyyy/MM/dd HH:mm");

                string sSqlstmt = "insert into t_customervisit (Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber,"
                    + " Designation, Address, Product, FollowUp_Date, Outcomeof_Visit, Remarks, ProcessedBy,City,DocUploadPath,Department,Location"
                + ") values('" + objCustomerVisitModels.Branch + "','" + dtPlannedDate + "','" + dtDateOfVisit
                        + "','" + objCustomerVisitModels.CustomerName + "','" + objCustomerVisitModels.ContactPerson + "','" + objCustomerVisitModels.EmailID
                        + "','" + objCustomerVisitModels.ContactNumber + "','" + objCustomerVisitModels.Designation + "','" + objCustomerVisitModels.Address
                        + "','" + objCustomerVisitModels.Product + "','" + dtFollowUp_Date + "','" + objCustomerVisitModels.Outcomeof_Visit
                        + "', '" + objCustomerVisitModels.Remarks + "', '" + objCustomerVisitModels.ProcessedBy + "','" + objCustomerVisitModels.City + "','" + objCustomerVisitModels.DocUploadPath + "','" + objCustomerVisitModels.Department
                        + "','" + objCustomerVisitModels.Location + "'"; 
                 sSqlstmt = sSqlstmt + ")";
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomerVisit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCustomerVisit(CustomerVisitModels objCustomerVisitModels)
        {
            try
            {
                string dtPlannedDate = objCustomerVisitModels.PlannedDate.ToString("yyyy/MM/dd HH:mm");
                string dtDateOfVisit = objCustomerVisitModels.DateOfVisit.ToString("yyyy/MM/dd HH:mm");
                string dtFollowUp_Date = objCustomerVisitModels.FollowUp_Date.ToString("yyyy/MM/dd HH:mm");

                string sSqlstmt = "update t_customervisit set Branch='" + objCustomerVisitModels.Branch + "', "
                    + "PlannedDate='" + dtPlannedDate + "', DateOfVisit='" + dtDateOfVisit + "', CustomerName='" + objCustomerVisitModels.CustomerName
                    + "', ContactPerson='" + objCustomerVisitModels.ContactPerson + "', EmailID='" + objCustomerVisitModels.EmailID + "', ContactNumber='"
                    + objCustomerVisitModels.ContactNumber + "', Designation='" + objCustomerVisitModels.Designation + "', Address='" + objCustomerVisitModels.Address
                    + "', Product='" + objCustomerVisitModels.Product + "', FollowUp_Date='" + dtFollowUp_Date + "', Outcomeof_Visit='" + objCustomerVisitModels.Outcomeof_Visit
                    + "', ProcessedBy='" + objCustomerVisitModels.ProcessedBy + "', Remarks='" + objCustomerVisitModels.Remarks + "',City='" + objCustomerVisitModels.City
                    + "',Department='" + objCustomerVisitModels.Department + "',Location='" + objCustomerVisitModels.Location + "'";
                if (objCustomerVisitModels.DocUploadPath != null && objCustomerVisitModels.DocUploadPath != "")
                {
                    sSqlstmt = sSqlstmt + ", DocUploadPath='" + objCustomerVisitModels.DocUploadPath + "' ";
                }
                sSqlstmt = sSqlstmt + " where CustomerVisitId=" + objCustomerVisitModels.CustomerVisitId;
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomerVisit: " + ex.ToString());
            }
            return false;
        }
    }

    public class CustomerVisitModelsList
    {
        public List<CustomerVisitModels> lstCustomerVisit { get; set; }
    }
}