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
    public class CustomerModels
    {

        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string CustID { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        [System.Web.Mvc.Remote("doesCompNameExist", "CustomerMgmt", HttpMethod = "POST", ErrorMessage = "Customer Name already exists. Please enter a different name.")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Contact Person Name")]
        public string ContactPerson { get; set; }

        //[Required]
        [Display(Name = "Company Code")]
        public string Cust_Code { get; set; }

        // [Required]
        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [System.Web.Mvc.Remote("doesEmailIDExist", "CustomerMgmt", HttpMethod = "POST", ErrorMessage = "Email Id already exists. Please enter a different Id.")]
        public string EmailId { get; set; }

        //[Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        // [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        //[Required]
        [Display(Name = "State/Province")]
        public string State { get; set; }

        //[Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Company Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [System.Web.Mvc.Remote("doesEmailIDExist", "CustomerMgmt", HttpMethod = "POST", ErrorMessage = "Email Id already exists. Please enter a different Id.")]
        public string Email_Id { get; set; }

        //[Required]
        [Display(Name = "P.O. Box No.")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Mobile No.")]
        public string MobileNumber { get; set; }

        //[Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter proper fax details.")]
        [Display(Name = "Fax No.")]
        public string FaxNumber { get; set; }

        //[Required]
        [Display(Name = "Logo")]
        public string logo { get; set; }

        //[Required]
        [Display(Name = "Customer Type")]
        public string CustType { get; set; }

        //[Required]
        //[Display(Name = "Type of Entity")]
        //public string Cust_Supp_Type { get; set; }

        [Display(Name = "Upload")]
        public string upload { get; set; }

        [Display(Name = "GST No")]
        public string VatNo { get; set; }

        [Display(Name = "Licence Expiry")]
        public DateTime LicenceExpiry { get; set; }

        [Display(Name = "Division")]
        public string branch { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        internal bool FunRegisterCustomer(CustomerModels objCustomerModel, CustomerContactsModelsList objCustomerContactsList)
        {
            try
            {
                
                string sSqlstmt = "insert into t_customer_info (CompanyName, Address, City, State, PostalCode, Country, ContactPerson, PhoneNumber, FaxNumber," +
                    " Email_Id, CustType,Cust_Code,branch,upload,VatNo,Department,Location";

                if (objCustomerModel.logo != null && objCustomerModel.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", logo";
                }

                string sFields = "", sFieldValue = "";

                if (objCustomerModel.LicenceExpiry != null && objCustomerModel.LicenceExpiry > Convert.ToDateTime("01/01/0001"))
                {
                    sFields = sFields + ", LicenceExpiry";
                    sFieldValue = sFieldValue + ", '" + objCustomerModel.LicenceExpiry.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + sFields;
                sSqlstmt = sSqlstmt + ") values('" + objCustomerModel.CompanyName + "','" + objCustomerModel.Address + "','" + objCustomerModel.City
                        + "','" + objCustomerModel.State + "','" + objCustomerModel.PostalCode + "','" + objCustomerModel.Country + "','" + objCustomerModel.ContactPerson
                        + "','" + objCustomerModel.PhoneNumber + "','" + objCustomerModel.FaxNumber + "', '" + objCustomerModel.Email_Id + "','" + objCustomerModel.CustType
                        + "','" + objCustomerModel.Cust_Code + "','" + objCustomerModel.branch + "','" + objCustomerModel.upload + "','" + objCustomerModel.VatNo
                        + "','" + objCustomerModel.Department + "','" + objCustomerModel.Location + "'";


                if (objCustomerModel.logo != null && objCustomerModel.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", '" + objCustomerModel.logo + "'";
                }
                sSqlstmt = sSqlstmt + sFieldValue + ")";

                int iValue = objGlobalData.ExecuteQueryReturnId(sSqlstmt);
                if (iValue > 0)
                {
                    if (Cust_Code == null || Cust_Code == "")
                    {
                        string custcode = CompanyName.Substring(0, 4);
                        string sql = "update t_customer_info set Cust_Code='" + custcode + "' where CustID='" + iValue + "'";
                        objGlobalData.ExecuteQuery(sql);
                    }
                    if (objCustomerContactsList.lstCustomerContacts.Count > 0)
                    {
                        CustomerContactsModels objCustomerContacts = new CustomerContactsModels();

                        objCustomerContactsList.lstCustomerContacts[0].CustID = iValue.ToString();
                        objCustomerContacts.FunAddContactsDetails(objCustomerContactsList);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunRegisterCustomer: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateCustomer(CustomerModels objCustomerModel)
        {
            try
            {
                string sSqlstmt = "update t_customer_info set Address='" + objCustomerModel.Address + "', "
                    + "City='" + objCustomerModel.City + "', State='" + objCustomerModel.State + "', PostalCode='" + objCustomerModel.PostalCode
                    + "', Country='" + objCustomerModel.Country + "', PhoneNumber='" + objCustomerModel.PhoneNumber + "', FaxNumber='"
                    + objCustomerModel.FaxNumber + "', ContactPerson='" + objCustomerModel.ContactPerson + "', Email_Id='" + objCustomerModel.Email_Id
                    + "', CustType='" + objCustomerModel.CustType + "',Cust_Code='" + objCustomerModel.Cust_Code + "',VatNo='" + objCustomerModel.VatNo
                    + "',upload='" + objCustomerModel.upload + "',branch='" + objCustomerModel.branch + "',Department='" + objCustomerModel.Department + "',location='" + objCustomerModel.Location + "' ";

                if (objCustomerModel.logo != null && objCustomerModel.logo != "")
                {
                    sSqlstmt = sSqlstmt + ", logo='" + objCustomerModel.logo + "' ";
                }

                if (objCustomerModel.LicenceExpiry != null && objCustomerModel.LicenceExpiry > Convert.ToDateTime("01/01/0001"))
                {
                    sSqlstmt = sSqlstmt + ", LicenceExpiry='" + objCustomerModel.LicenceExpiry.ToString("yyyy/MM/dd") + "'";
                }

                sSqlstmt = sSqlstmt + " where CustID=" + objCustomerModel.CustID;
                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateCustomer: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteCustomer(string sId)
        {
            try
            {
                string sSqlstmt = "update t_customer_info set Compstatus=0 where CustID=" + sId;

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteCustomer: " + ex.ToString());
            }
            return false;
        }

        public bool checkCompanyNameExists(string CompanyName)
        {
            try
            {
                string sSqlstmt = "select CustID from t_customer_info where CompanyName='" + CompanyName + "'";
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

        public bool checkEmailAddressExists(string EmailId)
        {
            try
            {
                string sSqlstmt = "select EmailId from t_customer_info where EmailId='" + EmailId + "'";
                DataSet dsEmp = objGlobalData.Getdetails(sSqlstmt);
                if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in checkEmailAddressExists: " + ex.ToString());
            }
            return true;
        }


        //public bool emailFeedbackForm(string sEmaildId)
        //{
        //    try
        //    {
        //        if (sEmaildId != null && sEmaildId.Length > 0)
        //        {
        //            string sExtraMsg = "Greetings for the day!" +
        //                "<br/><br/>" + "As an ongoing commitment to provide quality products and services, MSME would be grateful if you could spare a few minutes"
        //            + " and give us your assessment of our products and services provided to you. Your valuable feedback will help us to "
        //            + "improve our products and services quality to bring our best to you."
        //            + "<p style='color: grey;'> <br/> Please follow the following steps to register your feedback: <br/> <br/>" +
        //             "1. Open the attachment and mark the ratings against each element <br/> 2. Click on done, file “Feedback.json” will be generated<br/> " +
        //            "3. Attach the file in email and send it to respected person <br/> <br/>" +
        //            "*Note: This is system generated email, replies to the email id are not monitored. In case you have any questions, please send email to respected person</p> ";


        //            //For Mehul company plz do as below
        //            //   "<br/><br/>" + "As an ongoing commitment to provide quality products and services, MSME would be grateful if you could spare a few minutes"
        //            //+ " and give us your assessment of our products and services provided to you. Your valuable feedback will help us to "
        //            //+ "improve our products and services quality to bring our best to you."
        //            //+ "<p style='color: grey;'> <br/> Please follow the following steps to register your feedback: <br/> <br/>" +
        //            // "1. Open the attachment and mark the ratings against each element <br/> 2. Click on done, file “Feedback.json” will be generated<br/> " +
        //            //"3. Attach the file in email and send it to respected person /*  mehul.parmar@trychem.com */ <br/> <br/>" +
        //            //"*Note: This is system generated email, replies to the email id are not monitored. In case you have any questions, please send email to respected person/* mehul.parmar@trychem.com */</p> ";

        //            Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody("Dear Customer", "CustomerFeedback", sExtraMsg);
        //            string sFilepath = Path.Combine(HttpContext.Current.Server.MapPath("~/DataUpload"), "CustomerSurveyForm.html");
        //            if (!File.Exists(sFilepath))
        //            {
        //                SurveyModels objSurveyModels = new SurveyModels();
        //                objSurveyModels.GenerateCustomerSurveyForm();
        //            }

        //            return objGlobalData.SendmailonlyBCC(sEmaildId, dicEmailContent["subject"], dicEmailContent["body"], sFilepath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobalData.AddFunctionalLog("Exception in emailFeedbackForm: " + ex.ToString());
        //    }
        //    return false;
        //}


        public bool emailFeedbackForm(string sEmaildId,string selectedSurveysId, string selectedSurveysName)
        {
            try
            {
                if (sEmaildId != null && sEmaildId.Length > 0)
                {
                    string sSqlstmt = "SELECT * from t_survey_setup_cloud";

                    DataSet Step1 = objGlobalData.Getdetails(sSqlstmt);
                    string surveyip = "";

                    if (Step1.Tables[0].Rows.Count > 0 && Step1.Tables[0].Rows.Count < 2)
                    {
                        surveyip = Step1.Tables[0].Rows[0]["remote_url"].ToString();

                    }


                    string[] SurveysId = selectedSurveysId.Split(',');
                    string[] SurveysName = selectedSurveysName.Split(',');
                    string[] survey = new string[SurveysName.Length]; ;

                    for (int i = 0; i < SurveysId.Length; i++)
                    {
                        survey[i] = surveyip + "/Survey/Responses/CreateByName?surveyId="+ SurveysId[i]+ "&&surveyName="+ SurveysName[i].Replace(" ", "%20");


                    }


                    string sExtraMsg = "Greetings for the day!" +
                        "<br/><br/>" + "As an ongoing commitment to provide quality products and services, MSME would be grateful if you could spare a few minutes"
                    + " and give us your assessment of our products and services provided to you. Your valuable feedback will help us to "
                    + "improve our products and services quality to bring our best to you."
                    + "<p style='color: grey;'> <br/> Please open the following link to fill your feedback: <br/> <br/>";

                    for (int j = 0; j < survey.Length; j++)
                    {
                        sExtraMsg = sExtraMsg+ "<br/>" + survey[j] + "<br/>";

                    }

                    Dictionary<string, string> dicEmailContent = objGlobalData.FormEmailBody("Dear Customer", "CustomerFeedback", sExtraMsg);
                    //string sFilepath = Path.Combine(HttpContext.Current.Server.MapPath("~/DataUpload"), "CustomerSurveyForm.html");
                    //if (!File.Exists(sFilepath))
                    //{
                    //    SurveyModels objSurveyModels = new SurveyModels();
                    //    objSurveyModels.GenerateCustomerSurveyForm();
                    //}

                    return objGlobalData.Sendmail2(sEmaildId, dicEmailContent["subject"], dicEmailContent["body"]);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in emailFeedbackForm: " + ex.ToString());
            }
            return false;
        }
     
    }

    public class CustomerContactsModels
    {
        clsGlobal objGlobalData = new clsGlobal();

        //[Required]
        [Display(Name = "Id")]
        public string ContactsId { get; set; }

        //[Required]
        [Display(Name = "Cust ID")]
        public string CustID { get; set; }

        //[Required]
        [Display(Name = "Company")]
        public string CustName { get; set; }

        // [Required]
        [Display(Name = "Sales Person")]
        public string ContactPerson { get; set; }

        // [Required]
        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        //[System.Web.Mvc.Remote("doesEmailIDExist", "CustomerMgmt", HttpMethod = "POST", ErrorMessage = "Email Id already exists. Please enter a different Id.")]
        public string EmailId { get; set; }

        //[Required]
        //[RegularExpression(@"^\d+$", ErrorMessage = "Please enter proper contact details.")]
        [Display(Name = "Phone No.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "MobileNumber")]
        public string MobileNumber { get; set; }


        internal bool FunAddContactsDetails(CustomerContactsModelsList objCustomerContactsList)
        {
            try
            {
                string sSqlstmt = "";
                if (objCustomerContactsList.lstCustomerContacts.Count > 0)
                {
                    for (int i = 0; i < objCustomerContactsList.lstCustomerContacts.Count; i++)
                    {
                        sSqlstmt = sSqlstmt + "insert into t_customer_info_contacts (CustID, ContactPerson, PhoneNumber, EmailId, MobileNumber,designation";

                        sSqlstmt = sSqlstmt + ") values('" + objCustomerContactsList.lstCustomerContacts[0].CustID
                            + "','" + objCustomerContactsList.lstCustomerContacts[i].ContactPerson + "','" + objCustomerContactsList.lstCustomerContacts[i].PhoneNumber
                                + "','" + objCustomerContactsList.lstCustomerContacts[i].EmailId + "','" + objCustomerContactsList.lstCustomerContacts[i].MobileNumber
                                + "','" + objCustomerContactsList.lstCustomerContacts[i].designation + "'); ";
                    }
                    return objGlobalData.ExecuteQuery(sSqlstmt);
                }
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunAddContactsDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunUpdateContactsDetails(CustomerContactsModels objCustomerContacts)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_contacts set ContactPerson='" + objCustomerContacts.ContactPerson + "', PhoneNumber='" + objCustomerContacts.PhoneNumber
                        + "', EmailId='" + objCustomerContacts.EmailId + "', MobileNumber='" + objCustomerContacts.MobileNumber
                        + "', designation='" + objCustomerContacts.designation + "' where ContactsId='" + objCustomerContacts.ContactsId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunUpdateContactsDetails: " + ex.ToString());
            }
            return false;
        }

        internal bool FunDeleteContacts(string sId)
        {
            try
            {
                string sSqlstmt = "update t_customer_info_contacts set active=0 where ContactsId='" + sId + "'";

                return objGlobalData.ExecuteQuery(sSqlstmt);
            }
            catch (Exception ex)
            {
                objGlobalData.AddFunctionalLog("Exception in FunDeleteContacts: " + ex.ToString());
            }
            return false;
        }

        
    }


    public class CustomerModelsList
    {
        public List<CustomerModels> CustomerMList { get; set; }
    }

    public class CustomerContactsModelsList
    {
        public List<CustomerContactsModels> lstCustomerContacts { get; set; }
    }

    public class DropdownCustomerItems
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class DropdownCustomerList
    {
        public List<DropdownCustomerItems> lstCust { get; set; }
    }
}