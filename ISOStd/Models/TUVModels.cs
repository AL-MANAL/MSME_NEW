using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace ISOStd.Models
{
    public class TUVModels
    {
    }
    public class TUVCustomerModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string CustID { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        [System.Web.Mvc.Remote("doesCompNameExist", "TUV", HttpMethod = "POST", ErrorMessage = "Customer Name already exists. Please enter a different name.")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Customer Code")]
        public string Cust_Code { get; set; }

        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email_Id { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "P.O. Box No.")]
        public string PostalCode { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter proper fax details.")]
        [Display(Name = "Fax No.")]
        public string FaxNumber { get; set; }

        [Display(Name = "Customer Type")]
        public string CustType { get; set; }

        [Display(Name = "Id")]
        public string id_project { get; set; }

        [Display(Name = "Project No")]
        public string Project_no { get; set; }

        [Display(Name = "Project Description")]
        public string Project_desc { get; set; }

        [Display(Name = "Audit Criteria")]
        public string ISOStd { get; set; }

        [Display(Name = "Audit Team")]
        public string Audit_team { get; set; }

        [Display(Name = "Contract From")]
        public DateTime Contract_from { get; set; }

        [Display(Name = "Contract To")]
        public DateTime Contract_to { get; set; }

        public string GetAuditCriteriaByProject(string Project_no)
        {

            try {

                string sqlstmt = "select ISOStd from t_projectmaster_tuv where id_project='" + Project_no + "'";
                DataSet dsProject = objGlobalData.Getdetails(sqlstmt);
                if (dsProject.Tables.Count > 0 && dsProject.Tables[0].Rows.Count > 0)
                {
                    return objGlobalData.GetIsoStdNameById(dsProject.Tables[0].Rows[0]["ISOStd"].ToString());
                }
            }
            catch(Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditCriteriaByProject: " + ex.ToString());
            }
            return "";
        
        }
        public string GetCustomerByID(string CustID)
        {

            try
            {

                string sqlstmt = "select CompanyName from t_customer_info_tuv where CustID='" + CustID + "'";
                DataSet dsData = objGlobalData.Getdetails(sqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["CompanyName"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCustomerByID: " + ex.ToString());
            }
            return "";

        }
        public string GetProjectByID(string id_project)
        {

            try
            {

                string sqlstmt = "select Project_no from t_projectmaster_tuv where id_project='" + id_project + "'";
                DataSet dsData = objGlobalData.Getdetails(sqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Project_no"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetProjectByID: " + ex.ToString());
            }
            return "";

        }
        public MultiSelectList GetCustomerListbox()
        {
            TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
            objCustList.CustomerList = new List<TUVCustomerModels>();

            try
            {
                string sSsqlstmt = "select CustID, CompanyName from t_customer_info_tuv where Active=1 order by CompanyName asc";

                DataSet dsBranch = objGlobalData.Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        TUVCustomerModels Branch = new TUVCustomerModels()
                        {
                            CustID = dsBranch.Tables[0].Rows[i]["CustID"].ToString(),
                            CompanyName = dsBranch.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper()
                        };

                        objCustList.CustomerList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCustomerListbox: " + ex.ToString());
            }

            return new MultiSelectList(objCustList.CustomerList, "CustID", "CompanyName");
        }
        public MultiSelectList GetProjectListbox()
        {
            TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
            objCustList.CustomerList = new List<TUVCustomerModels>();

            try
            {
                string sSsqlstmt = "select id_project, Project_no from t_customer_info_tuv t,t_projectmaster_tuv tt where" 
                +" t.CustID=tt.CustID and t.Active=1 and tt.Active=1 order by Project_no asc";

                DataSet dsBranch = objGlobalData.Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        TUVCustomerModels Branch = new TUVCustomerModels()
                        {
                            id_project = dsBranch.Tables[0].Rows[i]["id_project"].ToString(),
                            Project_no = dsBranch.Tables[0].Rows[i]["Project_no"].ToString().ToUpper()
                        };
                        objCustList.CustomerList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCustomerListbox: " + ex.ToString());
            }
            return new MultiSelectList(objCustList.CustomerList, "id_project", "Project_no");
        }
        public MultiSelectList GetProjectListboxByCustID(string CustID)
        {
            TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
            objCustList.CustomerList = new List<TUVCustomerModels>();

            try
            {
                string sSsqlstmt = "select id_project, Project_no from t_projectmaster_tuv where CustID='"+CustID+"'"
                + " and Active=1 order by Project_no asc";

                DataSet dsBranch = objGlobalData.Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        TUVCustomerModels Branch = new TUVCustomerModels()
                        {
                            id_project = dsBranch.Tables[0].Rows[i]["id_project"].ToString(),
                            Project_no = dsBranch.Tables[0].Rows[i]["Project_no"].ToString().ToUpper()
                        };
                        objCustList.CustomerList.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetCustomerListbox: " + ex.ToString());
            }
            return new MultiSelectList(objCustList.CustomerList, "id_project", "Project_no");
        }
        internal bool FunDeleteCustomerDoc(string sCustID)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_tuv set Active=0 where CustID='" + sCustID + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustomerDoc: " + ex.ToString());
            }
            return false;

        }
        internal bool FunDeleteProject(string sid_project)
        {
            try
            {
                string sSqlstmt = "update t_projectmaster_tuv set Active=0 where id_project='" + sid_project + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteProject: " + ex.ToString());
            }
            return false;

        }
        public bool checkCompanyNameExists(string CompanyName)
        {
            try
            {
                string sSqlstmt = "select CustID from t_customer_info_tuv where CompanyName='" + CompanyName + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkCompanyNameExists: " + ex.ToString());
            }
            return true;
        }

        internal bool FunAddCustomer(TUVCustomerModels objCust, TUVCustomerModelsList objCustList)
        {
            try
            {
                string sSqlstmt = "insert into t_customer_info_tuv (Cust_Code,CompanyName,Address,City,PostalCode,Country,"
                +"FaxNumber,Email_Id,CustType)"
                + " values('" + objCust.Cust_Code + "','" + objCust.CompanyName + "','" + objCust.Address + "','" + objCust.City + "'"
                + ",'" + objCust.PostalCode + "','" + objCust.Country + "','" + objCust.FaxNumber + "','" + objCust.Email_Id + "'"
                + ",'" + objCust.CustType + "')";

                return FunAddProject(objCustList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomer: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddProject(TUVCustomerModelsList objCustList, int CustID)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objCustList.CustomerList.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_projectmaster_tuv (CustID,Project_no,Project_desc,ISOStd,Audit_team";
                    string sFields = "", sFieldValue = "";

                    if (objCustList.CustomerList[i].Contract_from != null && objCustList.CustomerList[i].Contract_from > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                         sFields = sFields + ", Contract_from";
                         sFieldValue = sFieldValue + ", '" + objCustList.CustomerList[i].Contract_from.ToString("yyyy/MM/dd") + "'";
                    }
                    if (objCustList.CustomerList[i].Contract_to != null && objCustList.CustomerList[i].Contract_to > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Contract_to";
                        sFieldValue = sFieldValue + ", '" + objCustList.CustomerList[i].Contract_to.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;

                    sSqlstmt = sSqlstmt + ") values('" + CustID + "','" + objCustList.CustomerList[i].Project_no + "',"
                    + "'" + objCustList.CustomerList[i].Project_desc + "','" + objCustList.CustomerList[i].ISOStd + "','" + objCustList.CustomerList[i].Audit_team + "'";

                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddProject: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCustomer(TUVCustomerModels objCust)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_tuv set Cust_Code ='" + objCust.Cust_Code + "', "
                    + "Address='" + objCust.Address + "',City='" + objCust.City + "',PostalCode='" + objCust.PostalCode + "',Country='" + objCust.Country + "'"
                + ",FaxNumber='" + objCust.FaxNumber + "',Email_Id='" + objCust.Email_Id + "',CustType='" + objCust.CustType + "'";

                sSqlstmt = sSqlstmt + " where CustID='" + objCust.CustID + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomer: " + ex.ToString());
            }

            return false;
        }
        internal bool FunUpdateProject(TUVCustomerModels objCust)
        {
            try
            {
                string sSqlstmt = "update t_projectmaster_tuv set Project_no ='" + objCust.Project_no + "', "
                    + "Project_desc='" + objCust.Project_desc + "',ISOStd='" + objCust.ISOStd + "',Audit_team='" + objCust.Audit_team + "'";
                if (objCust.Contract_from != null && objCust.Contract_from > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",Contract_from='" + objCust.Contract_from.ToString("yyyy/MM/dd") + "'";
                }
                if (objCust.Contract_to != null && objCust.Contract_to > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ",Contract_to='" + objCust.Contract_to.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + " where id_project='" + objCust.id_project + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateProject: " + ex.ToString());
            }

            return false;
        }
    }

    public class TUVCustomerContactsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string ContactsId { get; set; }

        [Display(Name = "Cust ID")]
        public string CustID { get; set; }

        [Display(Name = "Sales Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailId { get; set; }

        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "MobileNumber")]
        public string MobileNumber { get; set; }


        internal bool FunAddContactsDetails(TUVCustomerContactsModels objCust)
        {
            try
                {
                    string sSqlstmt = "insert into t_customer_info_contacts_tuv (CustID,ContactPerson,PhoneNumber,EmailId,MobileNumber)"
                    + " values('" + objCust.CustID + "','" + objCust.ContactPerson + "','" + objCust.PhoneNumber + "','" + objCust.EmailId + "'"
                    + ",'" + objCust.MobileNumber + "')";

                    return (objGlobalData.ExecuteQuery(sSqlstmt));
                }
                catch (Exception ex)
                {
                    objGlobalData.AddFunctionalLog("Exception in FunAddContactsDetails: " + ex.ToString());
                }
                return false;
        }

        internal bool FunUpdateContactsDetails(TUVCustomerContactsModels objCustomerContacts)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_contacts_tuv set ContactPerson='" + objCustomerContacts.ContactPerson + "', PhoneNumber='" + objCustomerContacts.PhoneNumber
                        + "', EmailId='" + objCustomerContacts.EmailId + "', MobileNumber='" + objCustomerContacts.MobileNumber + "' where ContactsId='" + objCustomerContacts.ContactsId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateContactsDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteContacts(string sContactsId)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_contacts_tuv set Active=0 where ContactsId='" + sContactsId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteContacts: " + ex.ToString());
            }
            return false;
        }
    }

    public class TUVSupplierModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string SupplierId { get; set; }

        [Display(Name = "Id")]
        public string id_project { get; set; }  

        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; }

        [Display(Name = "SupplierName")]
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

        [Display(Name = "PO No.")]
        public string PO_No { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Vat No")]
        public string VatNo { get; set; }

        [Display(Name = "Approval Reference No")]
        public string RefNo { get; set; }

        [Display(Name = "Suppier Type")]
        public string Supplier_type { get; set; }

        [Display(Name = "Payment Term")]
        public string Payment_term { get; set; }

        public string GetSupplierByID(string SupplierID)
        {
            try
            {
                string sqlstmt = "select SupplierName from t_supplier_tuv where SupplierID='" + SupplierID + "'";
                DataSet dsData = objGlobalData.Getdetails(sqlstmt);
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["SupplierName"].ToString());
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSupplierByID: " + ex.ToString());
            }
            return "";
        }
        public MultiSelectList GetSupplierListbox(string Project_no)
        {
            TUVSupplierModelsList objSupList = new TUVSupplierModelsList();
            objSupList.lstSupplier = new List<TUVSupplierModels>();
            try
            {
                string sSsqlstmt = "select SupplierID,SupplierName from t_supplier_tuv where id_project='" + Project_no + "' and Active=1";

                DataSet dsBranch = objGlobalData.Getdetails(sSsqlstmt);

                if (dsBranch.Tables.Count > 0 && dsBranch.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsBranch.Tables[0].Rows.Count; i++)
                    {
                        TUVSupplierModels Branch = new TUVSupplierModels()
                        {
                            SupplierId = dsBranch.Tables[0].Rows[i]["SupplierId"].ToString(),
                            SupplierName = dsBranch.Tables[0].Rows[i]["SupplierName"].ToString().ToUpper()
                        };
                        objSupList.lstSupplier.Add(Branch);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSupplierListbox: " + ex.ToString());
            }
            return new MultiSelectList(objSupList.lstSupplier, "SupplierId", "SupplierName");
        }
        internal bool FunDeleteSupplier(string sId)
        {
            try
            {
                string sSqlstmt = "update t_supplier_tuv set Active=0 where SupplierId=" + sId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteSupplier: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateSupplier(TUVSupplierModels objSupplierModels)
        {
            try
            {
                string user = "";
                
                    user = objGlobalData.GetCurrentUserSession().empid;
                
                string sSqlstmt = "update t_supplier_tuv set SupplierCode ='" + objSupplierModels.SupplierCode + "',SupplyScope ='" + objSupplierModels.SupplyScope + "', ApprovalCriteria='" + objSupplierModels.ApprovalCriteria + "', "
                    + "Remarks='" + objSupplierModels.Remarks + "', UpdatedOn='"
                    + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "', Added_Updated_By='" + user
                     + "', City='" + objSupplierModels.City + "', Country='" + objSupplierModels.Country + "', ContactPerson='" + objSupplierModels.ContactPerson + "', ContactNo='" + objSupplierModels.ContactNo
                     + "', Address='" + objSupplierModels.Address + "', FaxNo='" + objSupplierModels.FaxNo + "', PO_No='" + objSupplierModels.PO_No + "', Email='" + objSupplierModels.Email + "', VatNo='" + objSupplierModels.VatNo + "'"
                + ", RefNo='" + objSupplierModels.RefNo + "', Supplier_type='" + objSupplierModels.Supplier_type + "', Payment_term='" + objSupplierModels.Payment_term + "'";

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
     

        internal bool FunAddSupplier(TUVSupplierModels objSupplierModels)
        {
            try
            {
                string sColumn = "", sValues = "";
                string sSqlstmt = "insert into t_supplier_tuv (id_project,SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, Remarks, Added_Updated_By, ApprovedOn,"
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,UpdatedOn,Email,VatNo,RefNo,Supplier_type,Payment_term";
                string UpdateOn = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                if (objSupplierModels.SupportingDoc != null && objSupplierModels.SupportingDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", SupportingDoc";
                }
                sSqlstmt = sSqlstmt + ") values('" + objSupplierModels.id_project + "','" + objSupplierModels.SupplierCode + "','" + objSupplierModels.SupplierName + "','" + objSupplierModels.SupplyScope
                        + "','" + objSupplierModels.ApprovalCriteria + "','" + objSupplierModels.Remarks
                        + "','" + objSupplierModels.Added_Updated_By + "','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                        + "','" + objSupplierModels.City + "','" + objSupplierModels.Country + "','" + objSupplierModels.ContactPerson + "','" + objSupplierModels.ContactNo + "','" + objSupplierModels.Address
                        + "','" + objSupplierModels.FaxNo + "','" + objSupplierModels.PO_No + "','" + UpdateOn + "','" + objSupplierModels.Email + "','" + objSupplierModels.VatNo + "','" + objSupplierModels.RefNo + "',"
                        + "'" + objSupplierModels.Supplier_type + "','" + objSupplierModels.Payment_term + "'";

                if (objSupplierModels.SupportingDoc != null && objSupplierModels.SupportingDoc != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objSupplierModels.SupportingDoc + "'";
                }
                sSqlstmt = sSqlstmt + ")";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddSupplier: " + ex.ToString());
            }
            return false;
        }

       
    }

    public class TUVAuditModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_audit { get; set; }

        [Display(Name = "Customer Name")]
        public string CustID { get; set; }

        [Display(Name = "Project No")]
        public string Project_no { get; set; }

        [Display(Name = "Audit Cycle")]
        public string Audit_cycle { get; set; }

        [Display(Name = "Audit Prepared Date")]
        public DateTime Audit_date { get; set; }

        [Display(Name = "Audit Criteria")]
        public string Audit_criteria { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Id")]
        public string id_audit_trans { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierName { get; set; }

        public string fromAuditTime { get; set; }

        public string toAuditTime { get; set; }

        [Display(Name = "Audit Team")]
        public string Audit_team { get; set; }

        [Display(Name = "Auditee")]
        public string Auditee { get; set; }

        [Display(Name = "Scheduled Date")]
        public DateTime Scheduled_date { get; set; }

        [Display(Name = "Audit Status")]
        public string Audit_status { get; set; }

        [Display(Name = "Audit Date Time")]
        public string AuditTime { get; set; }

        [Display(Name = "To Time")]
        public string AuditToTime { get; set; }


       
        internal bool FunDeleteAudit(string sid_audit)
        {
            try
            {
                string sSqlstmt = "update t_audit_tuv set Active=0 where id_audit='" + sid_audit + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAudit: " + ex.ToString());
            }
            return false;

        }
        internal bool FunDeleteAuditTrans(string sid_audit_trans)
        {
            try
            {
                string sSqlstmt = "update t_audit_trans_tuv set Active=0 where id_audit_trans='" + sid_audit_trans + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAuditTrans: " + ex.ToString());
            }
            return false;

        }
        internal bool FunAddAudit(TUVAuditModels objAudit, TUVAuditModelsList objAuditList)
        {
            try
            {
                string sSqlstmt = "insert into t_audit_tuv (CustID,Project_no,Audit_cycle,Audit_criteria,Location";
                string sFields = "", sFieldValue = "";
                if (objAudit.Audit_date != null && objAudit.Audit_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Audit_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.Audit_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objAudit.CustID + "','" + objAudit.Project_no + "',"
                + "'" + objAudit.Audit_cycle + "','" + objAudit.Audit_criteria + "','" + objAudit.Location + "'";
   
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return FunAddAuditTrans(objAuditList, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddCustomer: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAuditTrans(TUVAuditModelsList objAuditList, int id_audit)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < objAuditList.lstAudit.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_audit_trans_tuv (id_audit,SupplierName,fromAuditTime,toAuditTime,Audit_team,"
                        + "Auditee,Audit_status";
                    string sFields = "", sFieldValue = "";
                    if (objAuditList.lstAudit[i].Scheduled_date != null && objAuditList.lstAudit[i].Scheduled_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                    {
                        sFields = sFields + ", Scheduled_date";
                        sFieldValue = sFieldValue + ", '" + objAuditList.lstAudit[i].Scheduled_date.ToString("yyyy/MM/dd") + "'";
                    }
                    sSqlstmt = sSqlstmt + sFields;
                    sSqlstmt = sSqlstmt + ") values('" + id_audit + "','" + objAuditList.lstAudit[i].SupplierName + "',"
                    + "'" + objAuditList.lstAudit[i].AuditTime + "','" + objAuditList.lstAudit[i].AuditToTime + "','" + objAuditList.lstAudit[i].Audit_team + "'"
                    + ",'" + objAuditList.lstAudit[i].Auditee + "','" + objAuditList.lstAudit[i].Audit_status + "'";
                    sSqlstmt = sSqlstmt + sFieldValue + ");";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditTrans: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateAuditTrans(TUVAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_audit_trans_tuv set SupplierName ='" + objAudit.SupplierName + "', fromAuditTime='" + objAudit.AuditTime + "',"
                    + "toAuditTime ='" + objAudit.AuditToTime + "',Audit_team ='" + objAudit.Audit_team + "',Auditee ='" + objAudit.Auditee + "',Audit_status ='" + objAudit.Audit_status + "'";
                if (objAudit.Scheduled_date != null && objAudit.Scheduled_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Scheduled_date='" + objAudit.Scheduled_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_audit_trans='" + objAudit.id_audit_trans + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditTrans: " + ex.ToString());
            }

            return false;
        }
        internal bool FunUpdateAudit(TUVAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "update t_audit_tuv set Audit_cycle ='" + objAudit.Audit_cycle + "', Location='" + objAudit.Location + "'";
                if (objAudit.Audit_date != null && objAudit.Audit_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sSqlstmt = sSqlstmt + ", Audit_date='" + objAudit.Audit_date.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where id_audit='" + objAudit.id_audit + "'";
                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAudit: " + ex.ToString());
            }

            return false;
        }

    }

    public class TUVPerformAuditModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        [Display(Name = "Id")]
        public string id_element { get; set; }

        [Display(Name = "Id")]
        public string id_paudit { get; set; }

        [Display(Name = "Id")]
        public string id_audit { get; set; }

        [Display(Name = "Customer")]
        public string CustID { get; set; }

        [Display(Name = "Project No")]
        public string Project_no { get; set; }

        [Display(Name = "id")]
        public string id_audit_trans { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Audit Criteria")]
        public string Audit_criteria { get; set; }

        [Display(Name = "Questions")]
        public string Questions { get; set; }

        [Display(Name = "Audit Date")]
        public DateTime Audit_date { get; set; }

        [Display(Name = "Auditors")]
        public string Auditors { get; set; }

        [Display(Name = "Auditee")]
        public string Auditee { get; set; }

        [Display(Name = "id")]
        public string id_checklist { get; set; }

        [Display(Name = "Rating")]
        public string id_auditratings { get; set; }

        [Display(Name = "Remarks/NC Description")]
        public string comment { get; set; }

        [Display(Name = "Evidence")]
        public string evidence_upload { get; set; }

        [Display(Name = "Finding Category")]
        public string finding_category { get; set; }
        internal bool FunDeleteAuditPerformance(string sid_paudit)
        {
            try
            {
                string sSqlstmt = "update t_performaudit_tuv set Active=0 where id_paudit='" + sid_paudit + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteAuditPerformance: " + ex.ToString());
            }
            return false;

        }
        internal bool FunUpdateAuditPerformance(TUVPerformAuditModels objAudit, TUVPerformAuditModelsList AuditElemtlist)
        {
            try
            {
                string sSqlstmt = "update t_performaudit_tuv set Auditors='" + objAudit.Auditors + "', "
                        + "Auditee='" + objAudit.Auditee + "'";
                if (objAudit.Audit_date > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", Audit_date='" + Audit_date.ToString("yyyy/MM/dd") + "' ";
                }
                sSqlstmt = sSqlstmt + " where id_paudit='" + objAudit.id_paudit + "'";

                if (objGlobalData.ExecuteQuery(sSqlstmt))
                {
                    TUVPerformAuditModels objElement = new TUVPerformAuditModels();
                    AuditElemtlist.lstAudit[0].id_paudit = objAudit.id_paudit;
                    objElement.FunUpdateAuditPerformance(AuditElemtlist);
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateAuditPerformance: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateAuditPerformance(TUVPerformAuditModelsList AuditElemtlist)
        {
            try
            {
                if (AuditElemtlist.lstAudit.Count > 0)
                {
                    string sSqlstmt = "";
                    for (int i = 0; i < AuditElemtlist.lstAudit.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "update t_performauditchecklist_tuv set id_element='" + AuditElemtlist.lstAudit[i].id_element + "',"
                            + "id_auditratings='" + AuditElemtlist.lstAudit[i].id_auditratings + "',comment='" + AuditElemtlist.lstAudit[i].comment + "',finding_category='" + AuditElemtlist.lstAudit[i].finding_category + "'";
                        if (AuditElemtlist.lstAudit[i].evidence_upload != null)
                        {
                            sSqlstmt = sSqlstmt + ", evidence_upload='" + AuditElemtlist.lstAudit[i].evidence_upload + "' ";
                        }
                        sSqlstmt = sSqlstmt + " where id_checklist='" + AuditElemtlist.lstAudit[i].id_checklist + "';";
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
        public string GetAuditQuestionNameById(string sid_element)
        {
            try
            {
                DataSet dsData = objGlobalData.Getdetails("SELECT Questions FROM t_auditelements_tuv where id_element='" + sid_element + "'");
                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    return (dsData.Tables[0].Rows[0]["Questions"].ToString());
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
                DataSet dsData = objGlobalData.Getdetails("SELECT Options FROM t_auditratings_tuv where id_auditratings='" + sid_auditratings + "'");
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
        internal bool FunAddPerformAudit(TUVPerformAuditModels objAudit, TUVPerformAuditModelsList AuditElemtlist)
        {
            try
            {
                string sSqlstmt = "insert into t_performaudit_tuv (id_audit_trans,Auditors,Auditee";
                string sFields = "", sFieldValue = "";
                if (objAudit.Audit_date != null && objAudit.Audit_date > Convert.ToDateTime("01/01/0001 00:00:00"))
                {
                    sFields = sFields + ", Audit_date";
                    sFieldValue = sFieldValue + ", '" + objAudit.Audit_date.ToString("yyyy/MM/dd") + "'";
                }
                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objAudit.id_audit_trans + "','" + objAudit.Auditors + "',"
                + "'" + objAudit.Auditee + "'";
                sSqlstmt = sSqlstmt + sFieldValue + ")";
                return FunAddAuditChecklist(AuditElemtlist, objGlobalData.ExecuteQueryReturnId(sSqlstmt));
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddPerformAudit: " + ex.ToString());
            }
            return false;
        }

        internal bool FunAddAuditChecklist(TUVPerformAuditModelsList AuditElemtlist, int id_paudit)
        {
            try
            {
                string sSqlstmt = "";
                for (int i = 0; i < AuditElemtlist.lstAudit.Count; i++)
                {
                    sSqlstmt = sSqlstmt + "insert into t_performauditchecklist_tuv (id_paudit,id_element,id_auditratings,comment,evidence_upload,finding_category)"
                    + "values('" + id_paudit + "','" + AuditElemtlist.lstAudit[i].id_element + "',"
                    + "'" + AuditElemtlist.lstAudit[i].id_auditratings + "','" + AuditElemtlist.lstAudit[i].comment + "','" + AuditElemtlist.lstAudit[i].evidence_upload + "','" + AuditElemtlist.lstAudit[i].finding_category + "');";
                }
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditChecklist: " + ex.ToString());
            }
            return false;
        }
        public MultiSelectList GetAuditElementsListbox(string CustID, string id_project, string SupplierId)
        {
            TUVPerformAuditModelsList AuditElemt = new TUVPerformAuditModelsList();
            AuditElemt.lstAudit = new List<TUVPerformAuditModels>();

            try
            {
                DataSet dsElement = objGlobalData.Getdetails("select id_element,Questions from t_auditelements_tuv"
                + " where CustID='" + CustID + "' and Project_no='" + id_project + "' and SupplierID='" + SupplierId + "' and Active=1");
                if (dsElement.Tables.Count > 0 && dsElement.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsElement.Tables[0].Rows.Count; i++)
                    {
                        TUVPerformAuditModels data = new TUVPerformAuditModels()
                        {
                            id_element = dsElement.Tables[0].Rows[i]["id_element"].ToString(),
                            Questions = dsElement.Tables[0].Rows[i]["Questions"].ToString()
                        };
                        AuditElemt.lstAudit.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditElementsListbox: " + ex.ToString());
            }
            return new MultiSelectList(AuditElemt.lstAudit, "id_element", "Questions");
        }
        public DataSet GetAuditRating()
        {

            DataSet dsRating = new DataSet();
            try
            {
                dsRating = objGlobalData.Getdetails("select id_auditratings, Options from t_auditratings_tuv order by id_auditratings");
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetSurveyRating: " + ex.ToString());
            }
            return dsRating;//new MultiSelectList(lstSurvey.lstSurveyRating, "SQ_OptionsId", "RatingOptions");
        }

        internal bool FunDeleteQuestions(string id_element)
        {
            try
            {
                string sSqlstmt = "update t_auditelements_tuv set Active=0 where id_element='" + id_element + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteQuestions: " + ex.ToString());
            }
            return false;

        }
        public MultiSelectList GetAuditQuestions(string CustID,string Project_no,string SupplierID)
        {
            TUVPerformAuditModelsList objList = new TUVPerformAuditModelsList();
            objList.lstAudit = new List<TUVPerformAuditModels>();

            try
            {
                string sSsqlstmt = "select id_element, Questions from t_auditelements_tuv where Active=1 and CustID='" + CustID + "'"
                    + " and Project_no='" + Project_no + "' and SupplierID='" + SupplierID + "'";

                DataSet dsData = objGlobalData.Getdetails(sSsqlstmt);

                if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsData.Tables[0].Rows.Count; i++)
                    {
                        TUVPerformAuditModels Audit = new TUVPerformAuditModels()
                        {
                            id_element = dsData.Tables[0].Rows[i]["id_element"].ToString(),
                            Questions = dsData.Tables[0].Rows[i]["Questions"].ToString().ToUpper()
                        };

                        objList.lstAudit.Add(Audit);
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in GetAuditQuestions: " + ex.ToString());
            }
            return new MultiSelectList(objList.lstAudit, "id_element", "Questions");
        }

        internal bool FunAddAuditQuestions(TUVPerformAuditModels objAudit)
        {
            try
            {
                string sSqlstmt = "insert into t_auditelements_tuv (CustID,Project_no,SupplierID,Questions)"
                + " values('" + objAudit.CustID + "','" + objAudit.Project_no + "','" + objAudit.SupplierID + "','" + objAudit.Questions + "')";
                return (objGlobalData.ExecuteQuery(sSqlstmt));
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddAuditQuestions: " + ex.ToString());
            }
            return false;
        }
        internal bool FunUpdateQuestions(string id_element, string Questions)
        {
            try
            {
                string sSqlstmt = "update t_auditelements_tuv set Questions='" + Questions + "' where id_element='" + id_element + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateQuestions: " + ex.ToString());
            }
            return false;
        }
    }
    public class TUVCustomerModelsList
    {
        public List<TUVCustomerModels> CustomerList { get; set; }
    }
    public class TUVCustomerContactsModelsList
    {
        public List<TUVCustomerContactsModels> ContactList { get; set; }
    }
    public class TUVSupplierModelsList
    {
        public List<TUVSupplierModels> lstSupplier { get; set; }
    }
    public class TUVAuditModelsList
    {
        public List<TUVAuditModels> lstAudit { get; set; }
    }
    public class TUVPerformAuditModelsList
    {
        public List<TUVPerformAuditModels> lstAudit { get; set; }
    }
}