using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.IO;
using System.Data;
using PagedList;
using PagedList.Mvc;
using System.Text.RegularExpressions;
using ISOStd.Filters;
using System.Net.Http;
namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustomerMgmtController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public CustomerMgmtController()
        {
            ViewBag.Menutype = "CustomerMgmt";
            ViewBag.SubMenutype = "CustomerList";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddCustomer()
        {
            CustomerModels objcust = new CustomerModels();
            objcust.branch = objGlobaldata.GetCurrentUserSession().division;
            objcust.Department = objGlobaldata.GetCurrentUserSession().DeptID;
            objcust.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
            ViewBag.Department = objGlobaldata.GetDepartmentListbox(objcust.branch);
            ViewBag.Location = objGlobaldata.GetDivisionLocationList(objcust.branch);
            ViewBag.CustType = objGlobaldata.GetDropdownList("Customer Type");
            return View(objcust);
        }

        [HttpPost]
        public JsonResult LoadingProgress(string CompanyName)
        {
            System.Threading.Thread.Sleep(10000);

            return Json("");
        }

        [HttpPost]
        public JsonResult doesCompNameExist(string CompanyName)
        {
            var Valid = true;
            if (CompanyName != null)
            {
                CustomerModels objCompModel = new CustomerModels();
                Valid = objCompModel.checkCompanyNameExists(CompanyName);
            }
            return Json(Valid);
        }
        
        [HttpPost]
        public JsonResult doesEmailIDExist(string EmailId)
        {
            var user = true;
            if (EmailId != null)
            {
                CustomerModels objCompModel = new CustomerModels();
                user = objCompModel.checkEmailAddressExists(EmailId);
            }
            return Json(user);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer(CustomerModels objCustomerModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objCustomerModels.branch = form["branch"];
                objCustomerModels.Department = form["Department"];
                objCustomerModels.Location = form["Location"];

                DateTime dateValue;
                HttpPostedFileBase files = Request.Files[0];

                if (form["LicenceExpiry"] != null && DateTime.TryParse(form["LicenceExpiry"], out dateValue) == true)
                {
                    objCustomerModels.LicenceExpiry = dateValue;
                }               
               
                if (upload != null && files.ContentLength > 0)
                {
                    objCustomerModels.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(file.FileName));
                            string sFilename = "Safety" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objCustomerModels.upload = objCustomerModels.upload + "," + "~/DataUpload/ProfilePic/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSafetyObservation-upload: " + ex.ToString());
                        }
                    }
                    objCustomerModels.upload = objCustomerModels.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }


                CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
                objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

                int iCnt = 0;
                if (form["itemcnt"] != null && form["itemcnt"] != "" && int.TryParse(form["itemcnt"], out iCnt))
                {
                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        if (form["ContactPerson " + i] != null)
                        {
                            CustomerContactsModels objCustomerContacts = new CustomerContactsModels
                            {
                                ContactPerson = form["ContactPerson " + i],
                                designation = form["designation " + i],
                                EmailId = form["EmailId " + i],
                                PhoneNumber = form["PhoneNumber " + i],
                                MobileNumber = form["MobileNumber " + i]
                            };
                            objCustomerContactsList.lstCustomerContacts.Add(objCustomerContacts);
                        }
                    }
                }

                if (objCustomerModels.FunRegisterCustomer(objCustomerModels, objCustomerContactsList))
                {
                    TempData["Successdata"] = "Added Customer details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomer: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerList");
        }

        [AllowAnonymous]
        public ActionResult CustomerList(string SearchText, int? page, string branch_name)
        {
            //string sCustSuppID = Request.QueryString["CustID"];

            //objGlobaldata.CreateUserSession();
            CustomerModelsList objCustomerModelsList = new CustomerModelsList();
            objCustomerModelsList.CustomerMList = new List<CustomerModels>();
            CustomerModels objCust = new CustomerModels();

            try
            {
                IEnumerable<SurveyModel> Surveys = null;

                //SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();

                //string sSqlstmt2 = "SELECT * from t_survey_setup_cloud";

                //DataSet Step1 = objGlobaldata.Getdetails(sSqlstmt2);

                //sSqlstmt2 = "SELECT * from t_survey_setup_user_credentials";

                //DataSet Step2 = objGlobaldata.Getdetails(sSqlstmt2);

                //if (Step1.Tables[0].Rows.Count > 0 && Step1.Tables[0].Rows.Count < 2)
                //{
                //    ObjSurveySetupModel.Cloud = Step1.Tables[0].Rows[0]["remote_url"].ToString();

                //}

                //if (Step2.Tables[0].Rows.Count > 0 && Step2.Tables[0].Rows.Count < 2)
                //{
                //    ObjSurveySetupModel.Email = Step2.Tables[0].Rows[0]["user_email"].ToString();
                //    ObjSurveySetupModel.Password = Step2.Tables[0].Rows[0]["user_password"].ToString();
                //}

                //if ((ObjSurveySetupModel.Cloud != "" && ObjSurveySetupModel.Cloud != null) && (ObjSurveySetupModel.Email != "" && ObjSurveySetupModel.Email != null) && (ObjSurveySetupModel.Password != "" && ObjSurveySetupModel.Password != null))
                //{
                //    using (var client = new HttpClient())
                //    {
                //        client.BaseAddress = new Uri(ObjSurveySetupModel.Cloud);
                //        //HTTP GET
                //        var responseTask = client.GetAsync("/Survey/Surveys/GetSurveys?Email="+ ObjSurveySetupModel.Email);
                //        responseTask.Wait();

                //        var result = responseTask.Result;
                //        if (result.IsSuccessStatusCode)
                //        {
                //            var readTask = result.Content.ReadAsAsync<IList<SurveyModel>>();
                //            readTask.Wait();

                //            Surveys = readTask.Result;
                //        }
                //        else //web api sent error response 
                //        {
                //            //log response status here..

                //            Surveys = Enumerable.Empty<SurveyModel>();
                //            TempData["SurveyError"] = "Error Connecting to Surveys";

                //        }
                //    }
                //}
                //else
                //{
                //    TempData["InfoData"] = "Survey Setup Not Complete";

                //}
                ViewBag.Surveys = Surveys;

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select CustID, CompanyName, Address, City, State, PostalCode, Country, ContactPerson, PhoneNumber, FaxNumber, "
                            + " logo, Email_Id, CustType,Cust_Code,upload,VatNo,LicenceExpiry,branch,Location,Department from t_customer_info where compstatus=1";
                string sSearchtext = "";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (CompanyName ='" + SearchText + "' or CompanyName like '" + SearchText + "%')";
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by CompanyName asc";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        CustomerModels objCustomer = new CustomerModels
                        {
                            CustID = dsCustomer.Tables[0].Rows[i]["CustID"].ToString(),
                            CompanyName = dsCustomer.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper(),
                            Cust_Code = dsCustomer.Tables[0].Rows[i]["Cust_Code"].ToString().ToUpper(),
                            Address = dsCustomer.Tables[0].Rows[i]["Address"].ToString(),
                            City = dsCustomer.Tables[0].Rows[i]["City"].ToString(),
                            State = dsCustomer.Tables[0].Rows[i]["State"].ToString(),
                            PostalCode = dsCustomer.Tables[0].Rows[i]["PostalCode"].ToString(),
                            Country = dsCustomer.Tables[0].Rows[i]["Country"].ToString(),
                            ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
                            PhoneNumber = dsCustomer.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                            //Audit_location = dsCustomer.Tables[0].Rows[0]["Audit_location"].ToString(),
                            FaxNumber = dsCustomer.Tables[0].Rows[i]["FaxNumber"].ToString(),
                            //Audit_Criteria = objGlobaldata.GetMultiIsoStdNameById(dsCustomer.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                            logo = dsCustomer.Tables[0].Rows[i]["logo"].ToString(),
                            Email_Id = dsCustomer.Tables[0].Rows[i]["Email_Id"].ToString(),
                            // BranchAddress = dsCustomer.Tables[0].Rows[0]["BranchAddress"].ToString(),
                            CustType = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[i]["CustType"].ToString()),
                            upload = dsCustomer.Tables[0].Rows[i]["upload"].ToString(),
                            VatNo = dsCustomer.Tables[0].Rows[i]["VatNo"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["LicenceExpiry"].ToString(), out dateValue))
                        {
                            objCustomer.LicenceExpiry = dateValue;
                        }

                        //sSqlstmt = "select * from t_Company_Branch where CompId=" + objCustomer.CustID;
                        //ViewBag.Branches = objGlobaldata.GetCompanyBranch(objCustomer.CustID);
                        objCustomerModelsList.CustomerMList.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCustomerModelsList.CustomerMList.ToList());

        }

        //[AllowAnonymous]
        //public JsonResult CustomerListSearch(string SearchText, int? page, string branch_name)
        //{
        //    //string sCustSuppID = Request.QueryString["CustID"];

        //    //objGlobaldata.CreateUserSession();
        //    CustomerModelsList objCustomerModelsList = new CustomerModelsList();
        //    objCustomerModelsList.CustomerMList = new List<CustomerModels>();
        //    CustomerModels objCust = new CustomerModels();

        //    try
        //    {
        //        IEnumerable<SurveyModel> Surveys = null;

        //        SurveySetupModel ObjSurveySetupModel = new SurveySetupModel();

        //        string sSqlstmt2 = "SELECT * from t_survey_setup_cloud";

        //        DataSet Step1 = objGlobaldata.Getdetails(sSqlstmt2);

        //        sSqlstmt2 = "SELECT * from t_survey_setup_user_credentials";

        //        DataSet Step2 = objGlobaldata.Getdetails(sSqlstmt2);

        //        if (Step1.Tables[0].Rows.Count > 0 && Step1.Tables[0].Rows.Count < 2)
        //        {
        //            ObjSurveySetupModel.Cloud = Step1.Tables[0].Rows[0]["remote_url"].ToString();

        //        }

        //        if (Step2.Tables[0].Rows.Count > 0 && Step2.Tables[0].Rows.Count < 2)
        //        {
        //            ObjSurveySetupModel.Email = Step2.Tables[0].Rows[0]["user_email"].ToString();
        //            ObjSurveySetupModel.Password = Step2.Tables[0].Rows[0]["user_password"].ToString();
        //        }

        //        if ((ObjSurveySetupModel.Cloud != "" && ObjSurveySetupModel.Cloud != null) && (ObjSurveySetupModel.Email != "" && ObjSurveySetupModel.Email != null) && (ObjSurveySetupModel.Password != "" && ObjSurveySetupModel.Password != null))
        //        {
        //            using (var client = new HttpClient())
        //            {
        //                client.BaseAddress = new Uri(ObjSurveySetupModel.Cloud);
        //                //HTTP GET
        //                var responseTask = client.GetAsync("/Survey/Surveys/GetSurveys?Email=" + ObjSurveySetupModel.Email);
        //                responseTask.Wait();

        //                var result = responseTask.Result;
        //                if (result.IsSuccessStatusCode)
        //                {
        //                    var readTask = result.Content.ReadAsAsync<IList<SurveyModel>>();
        //                    readTask.Wait();

        //                    Surveys = readTask.Result;
        //                }
        //                else //web api sent error response 
        //                {
        //                    //log response status here..

        //                    Surveys = Enumerable.Empty<SurveyModel>();
        //                    TempData["SurveyError"] = "Error Connecting to Surveys";

        //                }
        //            }
        //        }
        //        else
        //        {
        //            TempData["InfoData"] = "Survey Setup Not Complete";

        //        }
        //        ViewBag.Surveys = Surveys;

        //        string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
        //        string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
        //        ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

        //        string sSqlstmt = "select CustID, CompanyName, Address, City, State, PostalCode, Country, ContactPerson, PhoneNumber, FaxNumber, "
        //                    + " logo, Email_Id, CustType,Cust_Code,upload,VatNo,LicenceExpiry,branch,Location,Department from t_customer_info where compstatus=1";
        //        string sSearchtext = "";
        //        if (SearchText != null && SearchText != "")
        //        {
        //            ViewBag.SearchText = SearchText;
        //            sSearchtext = sSearchtext + " and (CompanyName ='" + SearchText + "' or CompanyName like '" + SearchText + "%')";
        //        }
        //        if (branch_name != null && branch_name != "")
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
        //            ViewBag.Branch_name = branch_name;
        //        }
        //        else
        //        {
        //            sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
        //        }
        //        sSqlstmt = sSqlstmt + sSearchtext + " order by CompanyName asc";
        //        DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

        //        for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
        //        {
        //            try
        //            {
        //                CustomerModels objCustomer = new CustomerModels
        //                {
        //                    CustID = dsCustomer.Tables[0].Rows[i]["CustID"].ToString(),
        //                    CompanyName = dsCustomer.Tables[0].Rows[i]["CompanyName"].ToString().ToUpper(),
        //                    Cust_Code = dsCustomer.Tables[0].Rows[i]["Cust_Code"].ToString().ToUpper(),
        //                    Address = dsCustomer.Tables[0].Rows[i]["Address"].ToString(),
        //                    City = dsCustomer.Tables[0].Rows[i]["City"].ToString(),
        //                    State = dsCustomer.Tables[0].Rows[i]["State"].ToString(),
        //                    PostalCode = dsCustomer.Tables[0].Rows[i]["PostalCode"].ToString(),
        //                    Country = dsCustomer.Tables[0].Rows[i]["Country"].ToString(),
        //                    ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
        //                    PhoneNumber = dsCustomer.Tables[0].Rows[i]["PhoneNumber"].ToString(),
        //                    //Audit_location = dsCustomer.Tables[0].Rows[0]["Audit_location"].ToString(),
        //                    FaxNumber = dsCustomer.Tables[0].Rows[i]["FaxNumber"].ToString(),
        //                    //Audit_Criteria = objGlobaldata.GetMultiIsoStdNameById(dsCustomer.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
        //                    logo = dsCustomer.Tables[0].Rows[i]["logo"].ToString(),
        //                    Email_Id = dsCustomer.Tables[0].Rows[i]["Email_Id"].ToString(),
        //                    // BranchAddress = dsCustomer.Tables[0].Rows[0]["BranchAddress"].ToString(),
        //                    CustType = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[i]["CustType"].ToString()),
        //                    upload = dsCustomer.Tables[0].Rows[i]["upload"].ToString(),
        //                    VatNo = dsCustomer.Tables[0].Rows[i]["VatNo"].ToString(),
        //                    branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[i]["branch"].ToString()),
        //                    Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[i]["Department"].ToString()),
        //                    Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[i]["Location"].ToString()),
        //                };

        //                DateTime dateValue;
        //                if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["LicenceExpiry"].ToString(), out dateValue))
        //                {
        //                    objCustomer.LicenceExpiry = dateValue;
        //                }

        //                //sSqlstmt = "select * from t_Company_Branch where CompId=" + objCustomer.CustID;
        //                //ViewBag.Branches = objGlobaldata.GetCompanyBranch(objCustomer.CustID);
        //                objCustomerModelsList.CustomerMList.Add(objCustomer);
        //            }
        //            catch (Exception ex)
        //            {
        //                objGlobaldata.AddFunctionalLog("Exception in CustomerListSearch: " + ex.ToString());
        //                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in CustomerListSearch: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return Json("Success");
        //}


        //[AllowAnonymous]
        //public ActionResult ExportCustomerList(string SearchText, int? page)
        //{
        //    //string sCustSuppID = Request.QueryString["CustID"];

        //    //objGlobaldata.CreateUserSession();
        //    CustomerModelsList objCustomerModelsList = new CustomerModelsList();
        //    objCustomerModelsList.CustomerMList = new List<CustomerModels>();

        //    try
        //    {

        //        string sSqlstmt = " select CompanyName, Address, City, State, PostalCode, Country, ContactPerson, PhoneNumber, FaxNumber, "
        //                    + " EmailId, CustType,upload,VatNo,LicenceExpiry from t_customer_info where compstatus=1";
        //        string sSearchtext = "";
        //        if (SearchText != null && SearchText != "")
        //        {
        //            ViewBag.SearchText = SearchText;
        //            sSearchtext = sSearchtext + " and (CompanyName ='" + SearchText + "' or CompanyName like '" + SearchText + "%')";
        //        }
        //        sSqlstmt = sSqlstmt + sSearchtext + " order by CompanyName asc";
        //        DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

        //        if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
        //        {
        //            objGlobaldata.ExportDataSetToExcel(dsCustomer, "CustomerList.xls");
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = "No record exists";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in ExportCustomerList: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }

        //    return File(Server.MapPath("~/DataUpload/CustomerList.xls"), "application/vnd.ms-excel", "CustomerList.xls");
        //}

        //
        // GET: /CustomerMgmt/CustomerDetails

        [AllowAnonymous]
        public ActionResult CustomerDetails()
        {
            //objGlobaldata.CreateUserSession();
            CustomerModels objCustomer = new CustomerModels();

            try
            {
                if (Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    string sCustSuppID = Request.QueryString["CustID"];

                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];

                    string sSqlstmt = "select CustID, CompanyName, Address, City, State, PostalCode, ContactPerson, Country, PhoneNumber, FaxNumber, "
                                + " logo, Email_Id, CustType,Cust_Code,upload,VatNo,LicenceExpiry,branch,Department,Location from t_customer_info where CustID=" + sCustSuppID;
                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {

                        objCustomer = new CustomerModels
                        {
                            CustID = dsCustomer.Tables[0].Rows[0]["CustID"].ToString(),
                            CompanyName = dsCustomer.Tables[0].Rows[0]["CompanyName"].ToString().ToUpper(),
                            Cust_Code = dsCustomer.Tables[0].Rows[0]["Cust_Code"].ToString().ToUpper(),
                            Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                            City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                            State = dsCustomer.Tables[0].Rows[0]["State"].ToString(),
                            PostalCode = dsCustomer.Tables[0].Rows[0]["PostalCode"].ToString(),
                            Country = dsCustomer.Tables[0].Rows[0]["Country"].ToString(),
                            ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            PhoneNumber = dsCustomer.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                            //Audit_location = dsCustomer.Tables[0].Rows[0]["Audit_location"].ToString(),
                            FaxNumber = dsCustomer.Tables[0].Rows[0]["FaxNumber"].ToString(),
                            //Audit_Criteria = objGlobaldata.GetMultiIsoStdNameById(dsCustomer.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                            logo = dsCustomer.Tables[0].Rows[0]["logo"].ToString(),
                            Email_Id = dsCustomer.Tables[0].Rows[0]["Email_Id"].ToString(),
                            CustType = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[0]["CustType"].ToString()),
                            upload = dsCustomer.Tables[0].Rows[0]["upload"].ToString(),
                            VatNo = dsCustomer.Tables[0].Rows[0]["VatNo"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[0]["Location"].ToString()),

                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["LicenceExpiry"].ToString(), out dateValue))
                        {
                            objCustomer.LicenceExpiry = dateValue;
                        }
                        //sSqlstmt = "select * from t_Company_Branch where CompId=" + objCustomer.CustID;
                        //ViewBag.Branches = objGlobaldata.GetCompanyBranch(objCustomer.CustID);


                        CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
                        objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

                        try
                        {
                            sSqlstmt = "select ContactsId, tci.CustID, CompanyName, tcic.PhoneNumber, tcic.EmailId, tcic.ContactPerson, tcic.MobileNumber, tcic.designation from t_customer_info_contacts as tcic,"
                                    + " t_customer_info as tci where tcic.CustID=tci.CustID and tci.CustID='" + sCustSuppID + "'  and tcic.active=1 ";


                            sSqlstmt = sSqlstmt + " order by tcic.ContactPerson asc";

                            DataSet dsCustomerContacts = objGlobaldata.Getdetails(sSqlstmt);
                            
                            if (dsCustomerContacts.Tables.Count > 0 && dsCustomerContacts.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; dsCustomerContacts.Tables.Count > 0 && i < dsCustomerContacts.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        CustomerContactsModels objCustomerContacts = new CustomerContactsModels
                                        {
                                            CustID = dsCustomerContacts.Tables[0].Rows[i]["CustID"].ToString(),
                                            CustName = dsCustomerContacts.Tables[0].Rows[i]["CompanyName"].ToString(),
                                            ContactsId = dsCustomerContacts.Tables[0].Rows[i]["ContactsId"].ToString().ToUpper(),
                                            PhoneNumber = dsCustomerContacts.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                            MobileNumber = dsCustomerContacts.Tables[0].Rows[i]["MobileNumber"].ToString(),
                                            ContactPerson = dsCustomerContacts.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                            EmailId = dsCustomerContacts.Tables[0].Rows[i]["EmailId"].ToString(),
                                            designation = dsCustomerContacts.Tables[0].Rows[i]["designation"].ToString(),
                                        };
                                        objCustomerContactsList.lstCustomerContacts.Add(objCustomerContacts);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                                ViewBag.CustomerContactsList = objCustomerContactsList;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }


                        return View(objCustomer);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerList");
        }
        
        [AllowAnonymous]
        public ActionResult CustomerInfo(int id)
        {
            CustomerModels objCustomer = new CustomerModels();

            try
            {
                string sSqlstmt = "select CustID, CompanyName, Address, City, State, PostalCode, ContactPerson, Country, PhoneNumber, FaxNumber, "
                            + " logo, Email_Id, CustType,Cust_Code,upload,VatNo,LicenceExpiry,branch,Department,Location from t_customer_info where CustID=" + id;
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                {
                    objCustomer = new CustomerModels
                    {
                        CustID = dsCustomer.Tables[0].Rows[0]["CustID"].ToString(),
                        CompanyName = dsCustomer.Tables[0].Rows[0]["CompanyName"].ToString().ToUpper(),
                        Cust_Code = dsCustomer.Tables[0].Rows[0]["Cust_Code"].ToString().ToUpper(),
                        Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                        City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                        State = dsCustomer.Tables[0].Rows[0]["State"].ToString(),
                        PostalCode = dsCustomer.Tables[0].Rows[0]["PostalCode"].ToString(),
                        Country = dsCustomer.Tables[0].Rows[0]["Country"].ToString(),
                        ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                        PhoneNumber = dsCustomer.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                        FaxNumber = dsCustomer.Tables[0].Rows[0]["FaxNumber"].ToString(),
                        logo = dsCustomer.Tables[0].Rows[0]["logo"].ToString(),
                        Email_Id = dsCustomer.Tables[0].Rows[0]["Email_Id"].ToString(),
                        CustType = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[0]["CustType"].ToString()),
                        upload = dsCustomer.Tables[0].Rows[0]["upload"].ToString(),
                        VatNo = dsCustomer.Tables[0].Rows[0]["VatNo"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[0]["Location"].ToString()),
                    };
                    DateTime dateValue;
                    if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["LicenceExpiry"].ToString(), out dateValue))
                    {
                        objCustomer.LicenceExpiry = dateValue;
                    }

                    CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
                    objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

                    try
                    {
                        sSqlstmt = "select ContactsId, tci.CustID, CompanyName, tcic.PhoneNumber, tcic.EmailId, tcic.ContactPerson, tcic.MobileNumber,tcic.designation from t_customer_info_contacts as tcic,"
                                + " t_customer_info as tci where tcic.CustID=tci.CustID and tci.CustID='" + id + "'  and tcic.active=1 ";
                        sSqlstmt = sSqlstmt + " order by tcic.ContactPerson asc";
                        DataSet dsCustomerContacts = objGlobaldata.Getdetails(sSqlstmt);
                        if (dsCustomerContacts.Tables.Count > 0 && dsCustomerContacts.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; dsCustomerContacts.Tables.Count > 0 && i < dsCustomerContacts.Tables[0].Rows.Count; i++)
                            {
                                try
                                {
                                    CustomerContactsModels objCustomerContacts = new CustomerContactsModels
                                    {
                                        CustID = dsCustomerContacts.Tables[0].Rows[i]["CustID"].ToString(),
                                        CustName = dsCustomerContacts.Tables[0].Rows[i]["CompanyName"].ToString(),
                                        ContactsId = dsCustomerContacts.Tables[0].Rows[i]["ContactsId"].ToString().ToUpper(),
                                        PhoneNumber = dsCustomerContacts.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                        MobileNumber = dsCustomerContacts.Tables[0].Rows[i]["MobileNumber"].ToString(),
                                        ContactPerson = dsCustomerContacts.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                        EmailId = dsCustomerContacts.Tables[0].Rows[i]["EmailId"].ToString(),
                                        designation = dsCustomerContacts.Tables[0].Rows[i]["designation"].ToString(),
                                    };
                                    objCustomerContactsList.lstCustomerContacts.Add(objCustomerContacts);
                                }
                                catch (Exception ex)
                                {
                                    objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                }
                            }
                            ViewBag.CustomerContactsList = objCustomerContactsList;
                        }
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }


                    return View(objCustomer);
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerList");
        }
        
        [AllowAnonymous]
        public ActionResult CustomerEdit()
        {
            //objGlobaldata.CreateUserSession();
            CustomerModels objCustomer = new CustomerModels();
            try
            {
                if (Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    string sCustSuppID = Request.QueryString["CustID"];
                    string sSqlstmt = "select CustID, CompanyName, Address, City, State, PostalCode, Country, ContactPerson,  PhoneNumber, FaxNumber, logo, "
                        + "Email_Id, CustType,Cust_Code,upload,VatNo,LicenceExpiry,branch,Department,Location from t_customer_info where CustID=" + sCustSuppID;
                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        objCustomer = new CustomerModels
                        {
                            CustID = dsCustomer.Tables[0].Rows[0]["CustID"].ToString(),
                            CompanyName = dsCustomer.Tables[0].Rows[0]["CompanyName"].ToString().ToUpper(),
                            Cust_Code = dsCustomer.Tables[0].Rows[0]["Cust_Code"].ToString(),
                            Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                            City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                            State = dsCustomer.Tables[0].Rows[0]["State"].ToString(),
                            PostalCode = dsCustomer.Tables[0].Rows[0]["PostalCode"].ToString(),
                            Country = dsCustomer.Tables[0].Rows[0]["Country"].ToString(),
                            ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            PhoneNumber = dsCustomer.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                            // Audit_location = dsCustomer.Tables[0].Rows[0]["Audit_location"].ToString(),
                            FaxNumber = dsCustomer.Tables[0].Rows[0]["FaxNumber"].ToString(),
                            //Audit_Criteria = objGlobaldata.GetMultiIsoStdNameById(dsCustomer.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                            logo = dsCustomer.Tables[0].Rows[0]["logo"].ToString(),
                            Email_Id = dsCustomer.Tables[0].Rows[0]["Email_Id"].ToString(),
                            CustType = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[0]["CustType"].ToString()),
                            upload = dsCustomer.Tables[0].Rows[0]["upload"].ToString(),
                            VatNo = dsCustomer.Tables[0].Rows[0]["VatNo"].ToString(),
                            branch =(dsCustomer.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsCustomer.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dateValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["LicenceExpiry"].ToString(), out dateValue))
                        {
                            objCustomer.LicenceExpiry = dateValue;
                        }

                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsCustomer.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCustomer.Tables[0].Rows[0]["branch"].ToString());

                        ViewBag.CustID = sCustSuppID;
                        ViewBag.CustType = objGlobaldata.GetDropdownList("Customer Type");

                        CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
                        objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

                        try
                        {
                            sSqlstmt = "select ContactsId, tci.CustID, CompanyName, tcic.PhoneNumber, tcic.EmailId, tcic.ContactPerson, tcic.MobileNumber,tcic.designation from t_customer_info_contacts as tcic,"
                                    + " t_customer_info as tci where tcic.CustID=tci.CustID and tci.CustID='" + sCustSuppID + "'  and tcic.active=1 ";


                            sSqlstmt = sSqlstmt + " order by tcic.ContactPerson asc";

                            DataSet dsCustomerContacts = objGlobaldata.Getdetails(sSqlstmt);


                            if (dsCustomerContacts.Tables.Count > 0 && dsCustomerContacts.Tables[0].Rows.Count > 0)
                            {

                                for (int i = 0; dsCustomerContacts.Tables.Count > 0 && i < dsCustomerContacts.Tables[0].Rows.Count; i++)
                                {
                                    try
                                    {
                                        CustomerContactsModels objCustomerContacts = new CustomerContactsModels
                                        {
                                            CustID = dsCustomerContacts.Tables[0].Rows[i]["CustID"].ToString(),
                                            CustName = dsCustomerContacts.Tables[0].Rows[i]["CompanyName"].ToString(),
                                            ContactsId = dsCustomerContacts.Tables[0].Rows[i]["ContactsId"].ToString().ToUpper(),
                                            PhoneNumber = dsCustomerContacts.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                            MobileNumber = dsCustomerContacts.Tables[0].Rows[i]["MobileNumber"].ToString(),
                                            ContactPerson = dsCustomerContacts.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                            EmailId = dsCustomerContacts.Tables[0].Rows[i]["EmailId"].ToString(),
                                            designation= dsCustomerContacts.Tables[0].Rows[i]["designation"].ToString(),
                                        };
                                        objCustomerContactsList.lstCustomerContacts.Add(objCustomerContacts);
                                    }
                                    catch (Exception ex)
                                    {
                                        objGlobaldata.AddFunctionalLog("Exception in CustomerEdit: " + ex.ToString());
                                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                    }
                                }
                                ViewBag.CustomerContactsList = objCustomerContactsList;
                            }
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }

                        return View(objCustomer);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerList");
        }
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerEdit(CustomerModels objCustomerModels, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            //objCustomerModels.Audit_Criteria = form["Audit_Criteria"];
            objCustomerModels.CustID = form["CustID"];
            objCustomerModels.branch = form["branch"];
            objCustomerModels.Department = form["Department"];
            objCustomerModels.Location = form["Location"];

            HttpPostedFileBase files = Request.Files[0];
            string QCDelete = Request.Form["QCDocsValselectall"];

            DateTime dateValue;
            if (form["LicenceExpiry"] != null && DateTime.TryParse(form["LicenceExpiry"], out dateValue) == true)
            {
                objCustomerModels.LicenceExpiry = dateValue;
            }
           
            if (upload != null && files.ContentLength > 0)
            {
                objCustomerModels.upload = "";
                foreach (var file in upload)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(file.FileName));
                        string sFilename = "Customer" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                        file.SaveAs(sFilepath + "/" + sFilename);
                        objCustomerModels.upload = objCustomerModels.upload + "," + "~/DataUpload/ProfilePic/" + sFilename;
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SafetyObservationEdit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                objCustomerModels.upload = objCustomerModels.upload.Trim(',');
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
            {
                objCustomerModels.upload = objCustomerModels.upload + "," + form["QCDocsVal"];
                objCustomerModels.upload = objCustomerModels.upload.Trim(',');
            }
            else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
            {
                objCustomerModels.upload = null;
            }
            else if (form["QCDocsVal"] == null && files.ContentLength == 0)
            {
                objCustomerModels.upload = null;
            }
            
            if (objCustomerModels.FunUpdateCustomer(objCustomerModels))
            {
                TempData["Successdata"] = "Updated successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerList");
        }

        //Get: /CustomerMgmt/CustomerDelete
        [HttpPost]

        public JsonResult CustomerDelete(FormCollection form)
        {
            string sCustID = form["CustID"];
            if (sCustID != "")
            {
                try
                {
                    CustomerModels objCustomerModels = new CustomerModels();

                    if (objCustomerModels.FunDeleteCustomer(sCustID))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Deleted successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }

                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in CustomerDelete: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            return Json("Delete Failed, Customer Id null");
        }


        //Get: /CustomerMgmt/EmailFeedbackform

        public ActionResult EmailFeedbackform(string EmailIds,string selectedSurveysId,string selectedSurveysName)
        {
            try
            {
                EmailIds = Regex.Replace(EmailIds, ",+", ",");
                EmailIds = EmailIds.Trim();
                EmailIds = EmailIds.TrimEnd(',');
                EmailIds = EmailIds.TrimStart(',');

                CustomerModels objCustomerModels = new CustomerModels();
                if (objCustomerModels.emailFeedbackForm(EmailIds, selectedSurveysId, selectedSurveysName))
                {
                    TempData["Successdata"] = "Feedback form mailed successfully to customer";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    return Json("Failed");

                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in EmailFeedbackform: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerList");
        }

        //
        // GET: /CustomerMgmt/AddCustomerContact

        [AllowAnonymous]
        public ActionResult AddCustomerContact()
        {
            CustomerContactsModels objCustomerContactsModels = new CustomerContactsModels();
            try
            {
                if (Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    string sCustID = Request.QueryString["CustID"];
                    objCustomerContactsModels.CustID = sCustID;
                    objCustomerContactsModels.CustName = objGlobaldata.GetCustomerNameById(sCustID);
                }
                else
                {
                    return RedirectToAction("CustomerContactList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerContact: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCustomerContactsModels);
        }

        //
        // POST: /CustomerMgmt/AddCustomerContact

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomerContact(CustomerContactsModels objCustomerContactsModels, FormCollection form)
        {
            try
            {
                objCustomerContactsModels.CustID = form["CustID"];
                CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
                objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

                objCustomerContactsList.lstCustomerContacts.Add(objCustomerContactsModels);

                if (objCustomerContactsModels.FunAddContactsDetails(objCustomerContactsList))
                {
                    TempData["Successdata"] = "Added Contact details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerContact: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerEdit", new { CustID = objCustomerContactsModels.CustID });
        }

        //
        // GET: /CustomerMgmt/CustomerContactList

        [AllowAnonymous]
        public ActionResult CustomerContactList(string SearchText, int? page)
        {
            CustomerContactsModelsList objCustomerContactsList = new CustomerContactsModelsList();
            objCustomerContactsList.lstCustomerContacts = new List<CustomerContactsModels>();

            try
            {
                string sSqlstmt = "select ContactsId, tci.CustID, CompanyName, tcic.PhoneNumber, tcic.EmailId, tcic.ContactPerson, tcic.MobileNumber from t_customer_info_contacts as tcic,"
                        + " t_customer_info as tci where tcic.CustID=tci.CustID  and tcic.active=1 ";


                if (Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    string sCustID = Request.QueryString["CustID"];

                    sSqlstmt = sSqlstmt + " and tci.CustID='" + sCustID + "' ";
                    ViewBag.CustID = sCustID;
                }

                string sSearchtext = "";
                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + " and (tcic.ContactPerson ='" + SearchText + "' or tcic.ContactPerson like '" + SearchText + "%')";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by tcic.ContactPerson asc";

                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);


                if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CustomerContactsModels objCustomer = new CustomerContactsModels
                            {
                                CustID = dsCustomer.Tables[0].Rows[i]["CustID"].ToString(),
                                CustName = dsCustomer.Tables[0].Rows[i]["CompanyName"].ToString(),
                                ContactsId = dsCustomer.Tables[0].Rows[i]["ContactsId"].ToString().ToUpper(),
                                PhoneNumber = dsCustomer.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                MobileNumber = dsCustomer.Tables[0].Rows[i]["MobileNumber"].ToString(),
                                ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                EmailId = dsCustomer.Tables[0].Rows[i]["EmailId"].ToString(),
                            };
                            objCustomerContactsList.lstCustomerContacts.Add(objCustomer);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerContactList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerContactList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCustomerContactsList.lstCustomerContacts.ToList().ToPagedList(page ?? 1, 40));
        }


        //
        // GET: /CustomerMgmt/CustomerContactEdit

        [AllowAnonymous]
        public ActionResult CustomerContactEdit()
        {
            try
            {
                if (Request.QueryString["ContactsId"] != null && Request.QueryString["ContactsId"] != "")
                {
                    string sContactsId = Request.QueryString["ContactsId"];
                    string sSqlstmt = "select ContactsId, tci.CustID, CompanyName, tcic.PhoneNumber, tcic.EmailId, tcic.ContactPerson, tcic.MobileNumber from t_customer_info_contacts as tcic,"
                            + " t_customer_info as tci where tcic.CustID=tci.CustID and ContactsId='" + sContactsId + "'  and tcic.active=1 ";

                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        CustomerContactsModels objCustomer = new CustomerContactsModels
                        {
                            CustID = dsCustomer.Tables[0].Rows[0]["CustID"].ToString(),
                            CustName = dsCustomer.Tables[0].Rows[0]["CompanyName"].ToString(),
                            ContactsId = dsCustomer.Tables[0].Rows[0]["ContactsId"].ToString().ToUpper(),
                            PhoneNumber = dsCustomer.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                            MobileNumber = dsCustomer.Tables[0].Rows[0]["MobileNumber"].ToString(),
                            ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            EmailId = dsCustomer.Tables[0].Rows[0]["EmailId"].ToString(),
                        };
                        return View(objCustomer);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerContactList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerContactList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerContactEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerContactList");
        }


        //
        // POST: /CustomerMgmt/CustomerEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerContactEdit(CustomerContactsModels objCustomerContactsModels, FormCollection form)
        {
            objCustomerContactsModels.CustID = form["CustID"];

            if (objCustomerContactsModels.FunUpdateContactsDetails(objCustomerContactsModels))
            {
                TempData["Successdata"] = "Updated Contact details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerEdit", new { CustID = objCustomerContactsModels.CustID });
        }

        //Get: /CustomerMgmt/CustomerContactDelete
        [HttpPost]
        // [ValidateAntiForgeryToken]

        public JsonResult CustomerContactDelete(FormCollection form)
        {
            string sContactsId = form["ContactsId"];
            try
            {
                if (sContactsId != "")
                {
                    CustomerContactsModels objCustomerModels = new CustomerContactsModels();

                    if (objCustomerModels.FunDeleteContacts(sContactsId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Deleted successfully");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerContactDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Delete Failed");
        }
        public JsonResult ChangeManagementApproveRejectNoty(string IdMgmt, int iStatus, string PendingFlg)
        {
            try
            {
                ManagementChangeModels objManagementModels = new ManagementChangeModels();

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1)
                {
                    sStatus = "Approved";

                }
                else if (iStatus == 2)
                {
                    sStatus = "Rejected";

                }
                if (objManagementModels.FunChangeManagementRequestApproveOrReject(IdMgmt, iStatus))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ChangeManagementApproveReject: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return Json("Success");
            }
            else
            {
                return Json("Failed");
            }
        }
    }
}
