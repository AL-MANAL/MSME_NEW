using ISOStd.Filters;
using ISOStd.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class TUVController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        [AllowAnonymous]
        public ActionResult AddCustomer()
        {
            try
            {
                ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.CustType = objGlobaldata.GetDropdownList("Customer Type");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomer: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomer(TUVCustomerModels objCust, FormCollection form)
        {
            try
            {
                if (objCust != null)
                {
                    TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
                    objCustList.CustomerList = new List<TUVCustomerModels>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        TUVCustomerModels objCustModel = new TUVCustomerModels();

                        DateTime dateValue;

                        if (DateTime.TryParse(form["Contract_from" + i], out dateValue) == true)
                        {
                            objCustModel.Contract_from = dateValue;
                        }
                        if (DateTime.TryParse(form["Contract_to" + i], out dateValue) == true)
                        {
                            objCustModel.Contract_to = dateValue;
                        }

                        objCustModel.Project_no = form["Project_no" + i];
                        objCustModel.Project_desc = form["Project_desc" + i];
                        objCustModel.ISOStd = form["ISOStd" + i];
                        objCustModel.Audit_team = form["Audit_team" + i];

                        objCustList.CustomerList.Add(objCustModel);
                    }

                    if (objCust.FunAddCustomer(objCust, objCustList))
                    {
                        TempData["Successdata"] = "Added Customer successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
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
        public ActionResult CustomerList(FormCollection form, int? page, string CompanyName, string chkAll)
        {
            TUVCustomerModels objCust = new TUVCustomerModels();
            TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
            objCustList.CustomerList = new List<TUVCustomerModels>();

            try
            {
                string sSqlstmt = "select CustID,Cust_Code,CompanyName,Address,City,PostalCode,Country,FaxNumber,Email_Id,"
                + "CustType from t_customer_info_tuv where Active=1";
                ViewBag.CustomerList = objCust.GetCustomerListbox();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (CompanyName != null && CompanyName != "")
                    {
                        ViewBag.CustomerListVal = CompanyName;
                        sSearchtext = sSearchtext + " and (CustID ='" + CompanyName + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by CompanyName asc";

                DataSet dsCustList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCustList.Tables.Count > 0 && dsCustList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCustList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TUVCustomerModels objCustModel = new TUVCustomerModels
                            {
                                CustID = dsCustList.Tables[0].Rows[i]["CustID"].ToString(),
                                Cust_Code = dsCustList.Tables[0].Rows[i]["Cust_Code"].ToString(),
                                CompanyName = dsCustList.Tables[0].Rows[i]["CompanyName"].ToString(),
                                Address = dsCustList.Tables[0].Rows[i]["Address"].ToString(),
                                City = dsCustList.Tables[0].Rows[i]["City"].ToString(),
                                PostalCode = dsCustList.Tables[0].Rows[i]["PostalCode"].ToString(),
                                Country = dsCustList.Tables[0].Rows[i]["Country"].ToString(),
                                FaxNumber = dsCustList.Tables[0].Rows[i]["FaxNumber"].ToString(),
                                Email_Id = dsCustList.Tables[0].Rows[i]["Email_Id"].ToString(),
                                CustType = objGlobaldata.GetDropdownitemById(dsCustList.Tables[0].Rows[i]["CustType"].ToString())
                            };
                            objCustList.CustomerList.Add(objCustModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCustList.CustomerList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public JsonResult CustomerDocDelete(FormCollection form)
        {
            try
            {
                if (form["CustID"] != null && form["CustID"] != "")
                {
                    TUVCustomerModels Doc = new TUVCustomerModels();
                    string sCustID = form["CustID"];

                    if (Doc.FunDeleteCustomerDoc(sCustID))
                    {
                        TempData["Successdata"] = "Document deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Customer Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult ProjectDocDelete(FormCollection form)
        {
            try
            {
                if (form["id_project"] != null && form["id_project"] != "")
                {
                    TUVCustomerModels Doc = new TUVCustomerModels();
                    string sid_project = form["id_project"];

                    if (Doc.FunDeleteProject(sid_project))
                    {
                        TempData["Successdata"] = "Project deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Project Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public JsonResult CustContactDelete(FormCollection form)
        {
            try
            {
                if (form["ContactsId"] != null && form["ContactsId"] != "")
                {
                    TUVCustomerContactsModels Doc = new TUVCustomerContactsModels();
                    string sContactsId = form["ContactsId"];

                    if (Doc.FunDeleteContacts(sContactsId))
                    {
                        TempData["Successdata"] = "Contact deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Contact Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult doesCompNameExist(string CompanyName)
        {
            var Valid = true;
            if (CompanyName != null)
            {
                TUVCustomerModels objCompModel = new TUVCustomerModels();
                Valid = objCompModel.checkCompanyNameExists(CompanyName);
            }

            return Json(Valid);
        }

        [AllowAnonymous]
        public ActionResult CustomerEdit()
        {
            try
            {
                if (Request.QueryString["CustID"] != null)
                {
                    string sCustID = Request.QueryString["CustID"];

                    string sSqlstmt = "select CustID,Cust_Code,CompanyName,Address,City,PostalCode,Country,FaxNumber,Email_Id,"
                    + "CustType from t_customer_info_tuv  where CustID='" + sCustID + "'";

                    DataSet dsCustList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustList.Tables.Count > 0 && dsCustList.Tables[0].Rows.Count > 0)
                    {
                        TUVCustomerModels objCust = new TUVCustomerModels
                        {
                            CustID = dsCustList.Tables[0].Rows[0]["CustID"].ToString(),
                            Cust_Code = dsCustList.Tables[0].Rows[0]["Cust_Code"].ToString(),
                            CompanyName = dsCustList.Tables[0].Rows[0]["CompanyName"].ToString(),
                            Address = dsCustList.Tables[0].Rows[0]["Address"].ToString(),
                            City = dsCustList.Tables[0].Rows[0]["City"].ToString(),
                            PostalCode = dsCustList.Tables[0].Rows[0]["PostalCode"].ToString(),
                            Country = dsCustList.Tables[0].Rows[0]["Country"].ToString(),
                            FaxNumber = dsCustList.Tables[0].Rows[0]["FaxNumber"].ToString(),
                            Email_Id = dsCustList.Tables[0].Rows[0]["Email_Id"].ToString(),
                            CustType = objGlobaldata.GetDropdownitemById(dsCustList.Tables[0].Rows[0]["CustType"].ToString())
                        };

                        sSqlstmt = "select id_project,CustID,Project_no,Project_desc,ISOStd,Audit_team,Contract_from,Contract_to"
                        + " from t_projectmaster_tuv  where CustID='" + sCustID + "' and Active=1 order by Project_no desc";
                        ViewBag.Project = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.CustType = objGlobaldata.GetDropdownList("Customer Type");

                        return View(objCust);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Customer Id cannot be Null or empty";
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

        [AllowAnonymous]
        public ActionResult CustomerDetails()
        {
            try
            {
                if (Request.QueryString["CustID"] != null)
                {
                    string sCustID = Request.QueryString["CustID"];

                    string sSqlstmt = "select CustID,Cust_Code,CompanyName,Address,City,PostalCode,Country,FaxNumber,Email_Id,"
                    + "CustType from t_customer_info_tuv  where CustID='" + sCustID + "'";

                    DataSet dsCustList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustList.Tables.Count > 0 && dsCustList.Tables[0].Rows.Count > 0)
                    {
                        TUVCustomerModels objCust = new TUVCustomerModels
                        {
                            CustID = dsCustList.Tables[0].Rows[0]["CustID"].ToString(),
                            Cust_Code = dsCustList.Tables[0].Rows[0]["Cust_Code"].ToString(),
                            CompanyName = dsCustList.Tables[0].Rows[0]["CompanyName"].ToString(),
                            Address = dsCustList.Tables[0].Rows[0]["Address"].ToString(),
                            City = dsCustList.Tables[0].Rows[0]["City"].ToString(),
                            PostalCode = dsCustList.Tables[0].Rows[0]["PostalCode"].ToString(),
                            Country = dsCustList.Tables[0].Rows[0]["Country"].ToString(),
                            FaxNumber = dsCustList.Tables[0].Rows[0]["FaxNumber"].ToString(),
                            Email_Id = dsCustList.Tables[0].Rows[0]["Email_Id"].ToString(),
                            CustType = objGlobaldata.GetDropdownitemById(dsCustList.Tables[0].Rows[0]["CustType"].ToString())
                        };

                        sSqlstmt = "select id_project,CustID,Project_no,Project_desc,ISOStd,Audit_team,Contract_from,Contract_to"
                        + " from t_projectmaster_tuv  where CustID='" + sCustID + "' and Active=1 order by Project_no desc";
                        ViewBag.Project = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.CustType = objGlobaldata.GetDropdownList("Customer Type");

                        return View(objCust);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Customer Id cannot be Null or empty";
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
        public ActionResult AddCustomerContact()
        {
            TUVCustomerContactsModels objCustomerContactsModels = new TUVCustomerContactsModels();
            try
            {
                if (Request.QueryString["CustID"] != null && Request.QueryString["CustID"] != "")
                {
                    string sCustID = Request.QueryString["CustID"];
                    objCustomerContactsModels.CustID = sCustID;
                }
                else
                {
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerContact: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCustomerContactsModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomerContact(TUVCustomerContactsModels objContactModels, FormCollection form)
        {
            try
            {
                if (objContactModels.FunAddContactsDetails(objContactModels))
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
            return RedirectToAction("CustomerContactList", new { CustID = objContactModels.CustID });
        }

        [AllowAnonymous]
        public ActionResult CustomerContactList(string SearchText, int? page)
        {
            TUVCustomerContactsModelsList objContactsList = new TUVCustomerContactsModelsList();
            objContactsList.ContactList = new List<TUVCustomerContactsModels>();
            try
            {
                if (Request.QueryString["CustID"] != null)
                {
                    string sCustID = Request.QueryString["CustID"];
                    string sSqlstmt = "select ContactsId,CustID,ContactPerson,PhoneNumber,EmailId,MobileNumber from"
                    + " t_customer_info_contacts_tuv where CustID='" + sCustID + "' and Active=1";
                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.CustID = sCustID;
                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                TUVCustomerContactsModels objCustomer = new TUVCustomerContactsModels
                                {
                                    CustID = dsCustomer.Tables[0].Rows[i]["CustID"].ToString(),
                                    ContactsId = dsCustomer.Tables[0].Rows[i]["ContactsId"].ToString(),
                                    PhoneNumber = dsCustomer.Tables[0].Rows[i]["PhoneNumber"].ToString(),
                                    MobileNumber = dsCustomer.Tables[0].Rows[i]["MobileNumber"].ToString(),
                                    ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                    EmailId = dsCustomer.Tables[0].Rows[i]["EmailId"].ToString(),
                                };
                                objContactsList.ContactList.Add(objCustomer);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in CustomerContactList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Contact Id cannot be Null or empty";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerContactList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objContactsList.ContactList.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public ActionResult CustomerContactEdit()
        {
            try
            {
                if (Request.QueryString["ContactsId"] != null && Request.QueryString["ContactsId"] != "")
                {
                    string sContactsId = Request.QueryString["ContactsId"];
                    string sSqlstmt = "select ContactsId,CustID,ContactPerson,PhoneNumber,EmailId,MobileNumber from"
                    + " t_customer_info_contacts_tuv where ContactsId='" + sContactsId + "'";

                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        TUVCustomerContactsModels objCustomer = new TUVCustomerContactsModels
                        {
                            CustID = dsCustomer.Tables[0].Rows[0]["CustID"].ToString(),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerContactEdit(TUVCustomerContactsModels objContactsModels, FormCollection form)
        {
            if (objContactsModels.FunUpdateContactsDetails(objContactsModels))
            {
                TempData["Successdata"] = "Updated Contact details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerContactList", new { CustID = objContactsModels.CustID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerEdit(TUVCustomerModels objCust, FormCollection form)
        {
            try
            {
                if (objCust != null)
                {
                    if (objCust.FunUpdateCustomer(objCust))
                    {
                        TempData["Successdata"] = "Updated Customer successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerList");
        }

        [AllowAnonymous]
        public ActionResult ProjectUpdate(TUVCustomerModels objCust, FormCollection form)
        {
            try
            {
                if (Request["button"].Equals("Save"))
                {
                    TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
                    objCustList.CustomerList = new List<TUVCustomerModels>();

                    DateTime dateValue;

                    if (DateTime.TryParse(form["Contract_from"], out dateValue) == true)
                    {
                        objCust.Contract_from = dateValue;
                    }
                    if (DateTime.TryParse(form["Contract_to"], out dateValue) == true)
                    {
                        objCust.Contract_to = dateValue;
                    }
                    objCust.ISOStd = form["ISOStd"];
                    objCustList.CustomerList.Add(objCust);

                    if (objCust.FunAddProject(objCustList, Convert.ToInt16(objCust.CustID)))
                    {
                        TempData["Successdata"] = "Added Project successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    DateTime dateValue;

                    if (DateTime.TryParse(form["Contract_from"], out dateValue) == true)
                    {
                        objCust.Contract_from = dateValue;
                    }
                    if (DateTime.TryParse(form["Contract_to"], out dateValue) == true)
                    {
                        objCust.Contract_to = dateValue;
                    }
                    objCust.ISOStd = form["ISOStd"];
                    if (objCust.FunUpdateProject(objCust))
                    {
                        TempData["Successdata"] = "Updated Project successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return RedirectToAction("CustomerEdit", new { CustID = objCust.CustID });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerList");
        }

        [AllowAnonymous]
        public ActionResult AddSupplier()
        {
            try
            {
                if (Request.QueryString["id_project"] != null)
                {
                    ViewBag.SubMenutype = "SupplierList";
                    ViewBag.id_project = Request.QueryString["id_project"];
                    ViewBag.ApprovalCriteria = objGlobaldata.GetDropdownList("Supplier-ApprovalCriteria");
                    ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                    ViewBag.SupplierType = objGlobaldata.GetDropdownList("Supplier Type");
                    ViewBag.paymentTerm = objGlobaldata.GetDropdownList("Supplier payment term");
                }
                else
                {
                    TempData["alertdata"] = "Project Id cannot be Null or empty";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSupplier: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplier(TUVSupplierModels objSupplierModels, FormCollection form, IEnumerable<HttpPostedFileBase> SupportingDoc)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                objSupplierModels.ApprovalCriteria = form["ApprovalCriteria"];

                objSupplierModels.Added_Updated_By = objGlobaldata.GetCurrentUserSession().empid;

                objSupplierModels.Supplier_type = form["Supplier_type"];
                objSupplierModels.Payment_term = form["Payment_term"];

                if (SupportingDoc != null && files.ContentLength > 0)
                {
                    objSupplierModels.SupportingDoc = "";
                    foreach (var file in SupportingDoc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TUV"), Path.GetFileName(file.FileName));
                            string sFilename = objSupplierModels.SupplierName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc + "," + "~/DataUpload/MgmtDocs/TUV/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSupplier-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objSupplierModels.FunAddSupplier(objSupplierModels))
                {
                    TempData["Successdata"] = "Added Supplier details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSupplier: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SupplierList", new { id_project = objSupplierModels.id_project });
        }

        [AllowAnonymous]
        public ActionResult SupplierList(string SearchText, string Approvestatus, int? page, string chkAll)
        {
            ViewBag.SubMenutype = "SupplierList";
            TUVSupplierModelsList objSupplierModelsList = new TUVSupplierModelsList();
            objSupplierModelsList.lstSupplier = new List<TUVSupplierModels>();

            try
            {
                if (Request.QueryString["id_project"] != null)
                {
                    string sid_project = Request.QueryString["id_project"];
                    string sSqlstmt = "select SupplierId,id_project, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, Remarks, "
                        + " Added_Updated_By, UpdatedOn,"
                        + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type FROM t_supplier_tuv where Active=1"
                        + " and id_project='" + sid_project + "'";

                    string sSearchtext = "";
                    ViewBag.id_project = sid_project;
                    sSqlstmt = sSqlstmt + sSearchtext + " order by SupplierName asc";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TUVSupplierModels objSupplierModels = new TUVSupplierModels
                            {
                                SupplierId = dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString(),
                                id_project = dsSupplier.Tables[0].Rows[i]["id_project"].ToString(),
                                SupplierCode = dsSupplier.Tables[0].Rows[i]["SupplierCode"].ToString(),
                                SupplierName = dsSupplier.Tables[0].Rows[i]["SupplierName"].ToString(),
                                SupplyScope = dsSupplier.Tables[0].Rows[i]["SupplyScope"].ToString(),
                                ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["ApprovalCriteria"].ToString()),
                                SupportingDoc = dsSupplier.Tables[0].Rows[i]["SupportingDoc"].ToString(),
                                Remarks = dsSupplier.Tables[0].Rows[i]["Remarks"].ToString(),
                                Added_Updated_By = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["Added_Updated_By"].ToString()),
                                City = dsSupplier.Tables[0].Rows[i]["City"].ToString(),
                                Country = dsSupplier.Tables[0].Rows[i]["Country"].ToString(),
                                ContactPerson = dsSupplier.Tables[0].Rows[i]["ContactPerson"].ToString(),
                                ContactNo = dsSupplier.Tables[0].Rows[i]["ContactNo"].ToString(),
                                Address = dsSupplier.Tables[0].Rows[i]["Address"].ToString(),
                                FaxNo = dsSupplier.Tables[0].Rows[i]["FaxNo"].ToString(),
                                PO_No = dsSupplier.Tables[0].Rows[i]["PO_No"].ToString(),
                                Email = dsSupplier.Tables[0].Rows[i]["Email"].ToString(),
                                VatNo = dsSupplier.Tables[0].Rows[i]["VatNo"].ToString(),
                                RefNo = dsSupplier.Tables[0].Rows[i]["RefNo"].ToString(),
                                Supplier_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["Supplier_type"].ToString())
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["UpdatedOn"].ToString(), out dateValue))
                            {
                                objSupplierModels.UpdatedOn = dateValue;
                            }

                            if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["ApprovedOn"].ToString(), out dateValue))
                            {
                                objSupplierModels.ApprovedOn = dateValue;
                            }
                            objSupplierModelsList.lstSupplier.Add(objSupplierModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
                else
                {
                    TempData["alertdata"] = "Project Id cannot be Null or empty";
                    return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSupplierModelsList.lstSupplier.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult SupplierEdit()
        {
            ViewBag.SubMenutype = "SupplierList";
            ViewBag.paymentTerm = objGlobaldata.GetDropdownList("Supplier payment term");
            TUVSupplierModels objSupplierModels = new TUVSupplierModels();

            try
            {
                ViewBag.SupplierType = objGlobaldata.GetDropdownList("Supplier Type");
                if (Request.QueryString["SupplierId"] != null && Request.QueryString["SupplierId"] != "")
                {
                    string sSupplierId = Request.QueryString["SupplierId"];

                    string sSqlstmt = "select SupplierId,id_project, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, Remarks,"
                        + " Added_Updated_By, UpdatedOn , City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,"
                        + " Supplier_type,Payment_term FROM t_supplier_tuv where SupplierId='"
                        + sSupplierId + "'";
                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        objSupplierModels = new TUVSupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                            id_project = dsSupplier.Tables[0].Rows[0]["id_project"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[0]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[0]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[0]["SupplyScope"].ToString(),
                            ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[0]["SupportingDoc"].ToString(),
                            Remarks = dsSupplier.Tables[0].Rows[0]["Remarks"].ToString(),
                            City = dsSupplier.Tables[0].Rows[0]["City"].ToString(),
                            Country = dsSupplier.Tables[0].Rows[0]["Country"].ToString(),
                            ContactPerson = dsSupplier.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            ContactNo = dsSupplier.Tables[0].Rows[0]["ContactNo"].ToString(),
                            Address = dsSupplier.Tables[0].Rows[0]["Address"].ToString(),
                            FaxNo = dsSupplier.Tables[0].Rows[0]["FaxNo"].ToString(),
                            PO_No = dsSupplier.Tables[0].Rows[0]["PO_No"].ToString(),
                            Email = dsSupplier.Tables[0].Rows[0]["Email"].ToString(),
                            VatNo = dsSupplier.Tables[0].Rows[0]["VatNo"].ToString(),
                            RefNo = dsSupplier.Tables[0].Rows[0]["RefNo"].ToString(),
                            Supplier_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Supplier_type"].ToString()),
                            Payment_term = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Payment_term"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["ApprovedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.ApprovedOn = dateValue;
                        }

                        ViewBag.ApprovalCriteria = objGlobaldata.GetDropdownList("Supplier-ApprovalCriteria");
                        ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");
                        ViewBag.SupplierType = objGlobaldata.GetDropdownList("Supplier Type");
                        ViewBag.paymentTerm = objGlobaldata.GetDropdownList("Supplier payment term");

                        return View(objSupplierModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SupplierList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("SupplierList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SupplierList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierEdit(TUVSupplierModels objSupplierModels, FormCollection form, IEnumerable<HttpPostedFileBase> SupportingDoc)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                objSupplierModels.ApprovalCriteria = form["ApprovalCriteria"];
                objSupplierModels.Payment_term = form["Payment_term"];

                if (SupportingDoc != null && files.ContentLength > 0)
                {
                    objSupplierModels.SupportingDoc = "";
                    foreach (var file in SupportingDoc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TUV"), Path.GetFileName(file.FileName));
                            string sFilename = objSupplierModels.SupplierName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc + "," + "~/DataUpload/MgmtDocs/TUV/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSupplier-uploaddocument: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc + "," + form["QCDocsVal"];
                    objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objSupplierModels.SupportingDoc = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objSupplierModels.SupportingDoc = null;
                }
                if (objSupplierModels.FunUpdateSupplier(objSupplierModels))
                {
                    TempData["Successdata"] = "Supplier details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SupplierList", new { id_project = objSupplierModels.id_project });
        }

        [AllowAnonymous]
        public ActionResult SupplierDetails()
        {
            ViewBag.SubMenutype = "SupplierList";
            TUVSupplierModels objSupplierModels = new TUVSupplierModels();

            try
            {
                if (Request.QueryString["SupplierId"] != null && Request.QueryString["SupplierId"] != "")
                {
                    string sSupplierId = Request.QueryString["SupplierId"];

                    string sSqlstmt = "select SupplierId,id_project, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, Remarks,"
                    + " Added_Updated_By, UpdatedOn, "
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type,Payment_term FROM t_supplier_tuv where SupplierId='" + sSupplierId + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        objSupplierModels = new TUVSupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                            id_project = dsSupplier.Tables[0].Rows[0]["id_project"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[0]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[0]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[0]["SupplyScope"].ToString(),
                            ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[0]["SupportingDoc"].ToString(),
                            Remarks = dsSupplier.Tables[0].Rows[0]["Remarks"].ToString(),
                            Added_Updated_By = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["Added_Updated_By"].ToString()),
                            City = dsSupplier.Tables[0].Rows[0]["City"].ToString(),
                            Country = dsSupplier.Tables[0].Rows[0]["Country"].ToString(),
                            ContactPerson = dsSupplier.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            ContactNo = dsSupplier.Tables[0].Rows[0]["ContactNo"].ToString(),
                            Address = dsSupplier.Tables[0].Rows[0]["Address"].ToString(),
                            FaxNo = dsSupplier.Tables[0].Rows[0]["FaxNo"].ToString(),
                            PO_No = dsSupplier.Tables[0].Rows[0]["PO_No"].ToString(),
                            Email = dsSupplier.Tables[0].Rows[0]["Email"].ToString(),
                            VatNo = dsSupplier.Tables[0].Rows[0]["VatNo"].ToString(),
                            RefNo = dsSupplier.Tables[0].Rows[0]["RefNo"].ToString(),
                            Supplier_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Supplier_type"].ToString()),
                            Payment_term = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Payment_term"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["ApprovedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.ApprovedOn = dateValue;
                        }
                        return View(objSupplierModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("SupplierList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("SupplierList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SupplierList");
        }

        [AllowAnonymous]
        public ActionResult ProjectList(FormCollection form, int? page, string CompanyName, string Project_no, string chkAll)
        {
            TUVCustomerModelsList objCustList = new TUVCustomerModelsList();
            objCustList.CustomerList = new List<TUVCustomerModels>();
            TUVCustomerModels objCust = new TUVCustomerModels();

            try
            {
                string sSqlstmt = "select t.CustID,id_project,CompanyName,Project_no,Project_desc,ISOStd,Audit_team,Contract_from, Contract_to"
                + " from t_customer_info_tuv t,t_projectmaster_tuv tt where t.Active=1 and tt.Active=1 and t.CustID=tt.CustID";
                ViewBag.CustomerList = objCust.GetCustomerListbox();
                ViewBag.ProjectList = objCust.GetProjectListbox();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (CompanyName != null && CompanyName != "")
                    {
                        ViewBag.CustomerListVal = CompanyName;
                        sSearchtext = sSearchtext + " and (t.CustID ='" + CompanyName + "')";
                    }
                    if (Project_no != null && Project_no != "")
                    {
                        ViewBag.ProjectListVal = Project_no;
                        sSearchtext = sSearchtext + " and (id_project ='" + Project_no + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by CompanyName asc,Project_no asc";
                DataSet dsCustList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsCustList.Tables.Count > 0 && dsCustList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCustList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TUVCustomerModels objCustModel = new TUVCustomerModels
                            {
                                CustID = dsCustList.Tables[0].Rows[i]["CustID"].ToString(),
                                id_project = dsCustList.Tables[0].Rows[i]["id_project"].ToString(),
                                CompanyName = dsCustList.Tables[0].Rows[i]["CompanyName"].ToString(),
                                Project_no = dsCustList.Tables[0].Rows[i]["Project_no"].ToString(),
                                Project_desc = dsCustList.Tables[0].Rows[i]["Project_desc"].ToString(),
                                ISOStd = objGlobaldata.GetIsoStdNameById(dsCustList.Tables[0].Rows[i]["ISOStd"].ToString()),
                                Audit_team = dsCustList.Tables[0].Rows[i]["Audit_team"].ToString(),
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsCustList.Tables[0].Rows[i]["Contract_from"].ToString(), out dtValue))
                            {
                                objCustModel.Contract_from = dtValue;
                            }
                            if (DateTime.TryParse(dsCustList.Tables[0].Rows[i]["Contract_to"].ToString(), out dtValue))
                            {
                                objCustModel.Contract_to = dtValue;
                            }
                            objCustList.CustomerList.Add(objCustModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCustList.CustomerList.ToList().ToPagedList(page ?? 1, 1000));
        }

        [HttpPost]
        public JsonResult SupplierDelete(FormCollection form)
        {
            try
            {
                if (form["SupplierId"] != null && form["SupplierId"] != "")
                {
                    TUVSupplierModels Doc = new TUVSupplierModels();
                    string SupplierId = form["SupplierId"];

                    if (Doc.FunDeleteSupplier(SupplierId))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Supplier Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AddAudit()
        {
            try
            {
                TUVCustomerModels objCust = new TUVCustomerModels();
                ViewBag.CustomerList = objCust.GetCustomerListbox();
                ViewBag.AuditCycle = objGlobaldata.GetConstantValue("AuditCycle");
                ViewBag.Status = objGlobaldata.GetConstantValue("AuditStatus");
                ViewBag.AuditTimeInHour = objGlobaldata.GetAuditTimeInHour();
                ViewBag.AuditTimeInMin = objGlobaldata.GetAuditTimeInMin();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult AuditList(FormCollection form, int? page, string CompanyName, string Project_no, string chkAll)
        {
            TUVCustomerModels objCust = new TUVCustomerModels();
            TUVAuditModelsList objAuditList = new TUVAuditModelsList();
            objAuditList.lstAudit = new List<TUVAuditModels>();

            try
            {
                string sSqlstmt = "select id_audit,CustID,Project_no,Audit_cycle,Audit_date,Audit_criteria,Location"
                + " from t_audit_tuv where Active=1";
                ViewBag.CustomerList = objCust.GetCustomerListbox();
                ViewBag.ProjectList = objCust.GetProjectListbox();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (CompanyName != null && CompanyName != "")
                    {
                        ViewBag.CustomerListVal = CompanyName;
                        sSearchtext = sSearchtext + " and (CustID ='" + CompanyName + "')";
                    }
                    if (Project_no != null && Project_no != "")
                    {
                        ViewBag.ProjectListVal = Project_no;
                        sSearchtext = sSearchtext + " and (Project_no ='" + Project_no + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                sSqlstmt = sSqlstmt + sSearchtext;
                DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsAuditList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            TUVAuditModels objCustModel = new TUVAuditModels
                            {
                                id_audit = dsAuditList.Tables[0].Rows[i]["id_audit"].ToString(),
                                CustID = objCust.GetCustomerByID(dsAuditList.Tables[0].Rows[i]["CustID"].ToString()),
                                Project_no = objCust.GetProjectByID(dsAuditList.Tables[0].Rows[i]["Project_no"].ToString()),
                                Audit_cycle = dsAuditList.Tables[0].Rows[i]["Audit_cycle"].ToString(),
                                Audit_criteria = dsAuditList.Tables[0].Rows[i]["Audit_criteria"].ToString(),
                                Location = dsAuditList.Tables[0].Rows[i]["Location"].ToString()
                            };
                            DateTime dtValue;
                            if (DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["Audit_date"].ToString(), out dtValue))
                            {
                                objCustModel.Audit_date = dtValue;
                            }
                            objAuditList.lstAudit.Add(objCustModel);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AuditList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAuditList.lstAudit.ToList().ToPagedList(page ?? 1, 1000));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAudit(TUVAuditModels objAudit, FormCollection form)
        {
            try
            {
                if (objAudit != null)
                {
                    DateTime dateValue;
                    TUVAuditModelsList objAuditList = new TUVAuditModelsList();
                    objAuditList.lstAudit = new List<TUVAuditModels>();

                    if (DateTime.TryParse(form["Audit_date"], out dateValue))
                    {
                        objAudit.Audit_date = dateValue;
                    }

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        TUVAuditModels objAuditModel = new TUVAuditModels();

                        objAuditModel.SupplierName = form["SupplierName" + i];
                        objAuditModel.AuditTime = form["AuditFromTimeInHour" + i] + ":" + form["AuditFromTimeInMin" + i];
                        objAuditModel.AuditToTime = form["AuditToTimeInHour" + i] + ":" + form["AuditToTimeInMin" + i];
                        objAuditModel.Audit_team = form["Audit_team" + i];
                        objAuditModel.Auditee = form["Auditee" + i];
                        if (DateTime.TryParse(form["Scheduled_date" + i], out dateValue) == true)
                        {
                            objAuditModel.Scheduled_date = dateValue;
                        }
                        objAuditModel.Audit_status = form["Audit_status" + i];

                        objAuditList.lstAudit.Add(objAuditModel);
                    }

                    if (objAudit.FunAddAudit(objAudit, objAuditList))
                    {
                        TempData["Successdata"] = "Added Audit successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditList");
        }

        [HttpPost]
        public JsonResult AuditDelete(FormCollection form)
        {
            try
            {
                if (form["id_audit"] != null && form["id_audit"] != "")
                {
                    TUVAuditModels Doc = new TUVAuditModels();
                    string sid_audit = form["id_audit"];

                    if (Doc.FunDeleteAudit(sid_audit))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Audit Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult AuditTransDelete(FormCollection form)
        {
            try
            {
                if (form["id_audit_trans"] != null && form["id_audit_trans"] != "")
                {
                    TUVAuditModels Doc = new TUVAuditModels();
                    string sid_audit_trans = form["id_audit_trans"];

                    if (Doc.FunDeleteAuditTrans(sid_audit_trans))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Audit Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AuditEdit()
        {
            TUVCustomerModels objCust = new TUVCustomerModels();
            TUVSupplierModels objSup = new TUVSupplierModels();
            try
            {
                if (Request.QueryString["id_audit"] != null)
                {
                    string sid_audit = Request.QueryString["id_audit"];

                    string sSqlstmt = "select id_audit,CustID,Project_no,Audit_cycle,Audit_date,Audit_criteria,Location"
                + " from t_audit_tuv where id_audit='" + sid_audit + "' ";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        TUVAuditModels objCustModel = new TUVAuditModels
                        {
                            id_audit = dsAuditList.Tables[0].Rows[0]["id_audit"].ToString(),
                            CustID = objCust.GetCustomerByID(dsAuditList.Tables[0].Rows[0]["CustID"].ToString()),
                            Project_no = objCust.GetProjectByID(dsAuditList.Tables[0].Rows[0]["Project_no"].ToString()),
                            Audit_cycle = dsAuditList.Tables[0].Rows[0]["Audit_cycle"].ToString(),
                            Audit_criteria = dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString(),
                            Location = dsAuditList.Tables[0].Rows[0]["Location"].ToString()
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["Audit_date"].ToString(), out dtValue))
                        {
                            objCustModel.Audit_date = dtValue;
                        }
                        sSqlstmt = "select id_audit_trans,id_audit,SupplierName,fromAuditTime,toAuditTime,Audit_team,Auditee,"
                        + "Scheduled_date,Audit_status from t_audit_trans_tuv where id_audit='" + sid_audit + "' and Active=1";
                        ViewBag.Audit = objGlobaldata.Getdetails(sSqlstmt);

                        ViewBag.AuditCycle = objGlobaldata.GetConstantValue("AuditCycle");
                        ViewBag.Status = objGlobaldata.GetConstantValue("AuditStatus");
                        ViewBag.AuditTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.AuditTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.SupplierList = objSup.GetSupplierListbox(dsAuditList.Tables[0].Rows[0]["Project_no"].ToString());
                        return View(objCustModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Audit Id cannot be Null or empty";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuditEdit(TUVAuditModels objAudit, FormCollection form)
        {
            try
            {
                if (objAudit != null)
                {
                    DateTime dateValue;
                    if (DateTime.TryParse(form["Audit_date"], out dateValue))
                    {
                        objAudit.Audit_date = dateValue;
                    }

                    if (objAudit.FunUpdateAudit(objAudit))
                    {
                        TempData["Successdata"] = "Updated Audit successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditList");
        }

        [AllowAnonymous]
        public ActionResult AuditTransEdit(TUVAuditModels objAudit, FormCollection form)
        {
            try
            {
                if (Request["button"].Equals("Save"))
                {
                    TUVAuditModelsList objAuditList = new TUVAuditModelsList();
                    objAuditList.lstAudit = new List<TUVAuditModels>();

                    DateTime dateValue;

                    objAudit.SupplierName = form["SupplierName"];
                    objAudit.AuditTime = form["AuditFromTimeInHour"] + ":" + form["AuditFromTimeInMin"];
                    objAudit.AuditToTime = form["AuditToTimeInHour"] + ":" + form["AuditToTimeInMin"];
                    objAudit.Audit_team = form["Audit_team"];
                    objAudit.Auditee = form["Auditee"];
                    if (DateTime.TryParse(form["Scheduled_date"], out dateValue) == true)
                    {
                        objAudit.Scheduled_date = dateValue;
                    }
                    objAudit.Audit_status = form["Audit_status"];

                    objAuditList.lstAudit.Add(objAudit);

                    if (objAudit.FunAddAuditTrans(objAuditList, Convert.ToInt16(objAudit.id_audit)))
                    {
                        TempData["Successdata"] = "Added Audit Plan successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    DateTime dateValue;
                    objAudit.AuditTime = form["AuditFromTimeInHour"] + ":" + form["AuditFromTimeInMin"];
                    objAudit.AuditToTime = form["AuditToTimeInHour"] + ":" + form["AuditToTimeInMin"];
                    if (DateTime.TryParse(form["Scheduled_date"], out dateValue) == true)
                    {
                        objAudit.Scheduled_date = dateValue;
                    }

                    if (objAudit.FunUpdateAuditTrans(objAudit))
                    {
                        TempData["Successdata"] = "Updated Audit plan successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return RedirectToAction("AuditEdit", new { id_audit = objAudit.id_audit });
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ProjectUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerList");
        }

        [AllowAnonymous]
        public ActionResult AuditDetails()
        {
            TUVCustomerModels objCust = new TUVCustomerModels();
            TUVSupplierModels objSup = new TUVSupplierModels();
            try
            {
                if (Request.QueryString["id_audit"] != null)
                {
                    string sid_audit = Request.QueryString["id_audit"];

                    string sSqlstmt = "select id_audit,CustID,Project_no,Audit_cycle,Audit_date,Audit_criteria,Location"
                + " from t_audit_tuv where id_audit='" + sid_audit + "' ";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        TUVAuditModels objCustModel = new TUVAuditModels
                        {
                            id_audit = dsAuditList.Tables[0].Rows[0]["id_audit"].ToString(),
                            CustID = objCust.GetCustomerByID(dsAuditList.Tables[0].Rows[0]["CustID"].ToString()),
                            Project_no = objCust.GetProjectByID(dsAuditList.Tables[0].Rows[0]["Project_no"].ToString()),
                            Audit_cycle = dsAuditList.Tables[0].Rows[0]["Audit_cycle"].ToString(),
                            Audit_criteria = dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString(),
                            Location = dsAuditList.Tables[0].Rows[0]["Location"].ToString()
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["Audit_date"].ToString(), out dtValue))
                        {
                            objCustModel.Audit_date = dtValue;
                        }
                        sSqlstmt = "select id_audit_trans,id_audit,SupplierName,fromAuditTime,toAuditTime,Audit_team,Auditee,"
                        + "Scheduled_date,Audit_status from t_audit_trans_tuv where id_audit='" + sid_audit + "' and Active=1";
                        ViewBag.Audit = objGlobaldata.Getdetails(sSqlstmt);

                        ViewBag.AuditCycle = objGlobaldata.GetConstantValue("AuditCycle");
                        ViewBag.Status = objGlobaldata.GetConstantValue("AuditStatus");
                        ViewBag.AuditTimeInHour = objGlobaldata.GetAuditTimeInHour();
                        ViewBag.AuditTimeInMin = objGlobaldata.GetAuditTimeInMin();
                        ViewBag.SupplierList = objSup.GetSupplierListbox(dsAuditList.Tables[0].Rows[0]["Project_no"].ToString());
                        return View(objCustModel);
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Audit Id cannot be Null or empty";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("AuditList");
        }

        public ActionResult FunGetSupplierList(string Project_no)
        {
            TUVSupplierModels objSup = new TUVSupplierModels();
            MultiSelectList lstSup = objSup.GetSupplierListbox(Project_no);
            return Json(lstSup);
        }

        public ActionResult FunGetProjectList(string CustID)
        {
            TUVCustomerModels objPrj = new TUVCustomerModels();
            MultiSelectList lstPrj = objPrj.GetProjectListboxByCustID(CustID);
            return Json(lstPrj);
        }

        [HttpPost]
        public JsonResult FunGetProjectAuditCriteria(string Project_no)
        {
            TUVCustomerModels objCust = new TUVCustomerModels();
            string Isostd = "";
            if (Project_no != "")
            {
                Isostd = objCust.GetAuditCriteriaByProject(Project_no);
            }
            return Json(Isostd);
        }

        [AllowAnonymous]
        public ActionResult GenerateAuditQuestion(string CustID, string Project_no, string SupplierID)
        {
            try
            {
                TUVCustomerModels objCust = new TUVCustomerModels();
                TUVSupplierModels objSup = new TUVSupplierModels();
                TUVPerformAuditModels obj = new TUVPerformAuditModels();
                ViewBag.CustomerList = objCust.GetCustomerListbox();

                if (CustID != null && CustID != "" && Project_no != null && Project_no != "" && SupplierID != null && SupplierID != "")
                {
                    ViewBag.CustomerListVal = CustID;
                    ViewBag.ProjectList = objCust.GetProjectListboxByCustID(CustID);
                    ViewBag.ProjectListVal = Project_no;
                    ViewBag.SupplierList = objSup.GetSupplierListbox(Project_no);
                    ViewBag.SupplierListVal = SupplierID;
                    ViewBag.AuditElements = obj.GetAuditQuestions(CustID, Project_no, SupplierID);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateAuditChecklist: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateAuditQuestion(TUVPerformAuditModels objAudit, FormCollection form)
        {
            try
            {
                TUVCustomerModels objCust = new TUVCustomerModels();
                TUVSupplierModels objSup = new TUVSupplierModels();
                TUVPerformAuditModels obj = new TUVPerformAuditModels();
                ViewBag.CustomerList = objCust.GetCustomerListbox();
                ViewBag.CustomerListVal = objAudit.CustID;
                ViewBag.ProjectList = objCust.GetProjectListboxByCustID(objAudit.CustID);
                ViewBag.ProjectListVal = objAudit.Project_no;
                ViewBag.SupplierList = objSup.GetSupplierListbox(objAudit.Project_no);
                ViewBag.SupplierListVal = objAudit.SupplierID;
                if (objAudit.FunAddAuditQuestions(objAudit))
                {
                    TempData["Successdata"] = "Added Questions added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                ViewBag.AuditElements = obj.GetAuditQuestions(objAudit.CustID, objAudit.Project_no, objAudit.SupplierID);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in GenerateAuditQuestion: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAudit);
        }

        [HttpPost]
        public JsonResult GetQuestions(string CustID, string Project_no, string SupplierID)
        {
            TUVPerformAuditModels obj = new TUVPerformAuditModels();
            MultiSelectList data = obj.GetAuditQuestions(CustID, Project_no, SupplierID);
            return Json(data);
        }

        [AllowAnonymous]
        public JsonResult QuestionUpdate(string id_element, string Questions)
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            TUVPerformAuditModels obj = new TUVPerformAuditModels();
            try
            {
                if (obj.FunUpdateQuestions(id_element, Questions))
                {
                    TempData["Successdata"] = "Questions updated successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuestionUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult QuestionDelete(string id_element)
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            TUVPerformAuditModels obj = new TUVPerformAuditModels();
            try
            {
                if (obj.FunDeleteQuestions(id_element))
                {
                    TempData["Successdata"] = "Survey details deleted successfully";
                    return Json("Success");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuestionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult QuestionDelete1(string id_element, string CustID, string Project_no, string SupplierID)
        {
            ViewBag.SubMenutype = "CustomerSurvey";
            TUVPerformAuditModels obj = new TUVPerformAuditModels();
            try
            {
                if (obj.FunDeleteQuestions(id_element))
                {
                    TempData["Successdata"] = "Survey details deleted successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuestionDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("GenerateAuditQuestion", new { CustID = CustID, Project_no = Project_no, SupplierID = SupplierID });
        }

        public ActionResult PerformAudit()
        {
            TUVPerformAuditModels objAudit = new TUVPerformAuditModels();

            try
            {
                TUVSupplierModels objSup = new TUVSupplierModels();
                TUVPerformAuditModels obj = new TUVPerformAuditModels();
                ViewBag.AuditRating = obj.GetAuditRating();
                ViewBag.FindingCategory = objGlobaldata.GetConstantValue("Findingcategory");
                if (Request.QueryString["id_audit_trans"] != null && Request.QueryString["id_audit_trans"] != "")
                {
                    string sid_audit_trans = Request.QueryString["id_audit_trans"];
                    string sSqlstmt = "select id_audit_trans,tt.id_audit,SupplierName,CustID,Project_no,Audit_criteria"
                    + " from t_audit_tuv t,t_audit_trans_tuv tt where t.id_audit=tt.id_audit and  id_audit_trans='" + sid_audit_trans + "'";
                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAuditList.Tables[0].Rows.Count; i++)
                        {
                            objAudit = new TUVPerformAuditModels
                            {
                                id_audit_trans = dsAuditList.Tables[0].Rows[i]["id_audit_trans"].ToString(),
                                id_audit = dsAuditList.Tables[0].Rows[i]["id_audit"].ToString(),
                                SupplierName = objSup.GetSupplierByID(dsAuditList.Tables[0].Rows[i]["SupplierName"].ToString()),
                                SupplierID = (dsAuditList.Tables[0].Rows[i]["SupplierName"].ToString()),
                                CustID = dsAuditList.Tables[0].Rows[i]["CustID"].ToString(),
                                Project_no = dsAuditList.Tables[0].Rows[i]["Project_no"].ToString(),
                                Audit_criteria = (dsAuditList.Tables[0].Rows[i]["Audit_criteria"].ToString()),
                            };
                            ViewBag.AuditElements = obj.GetAuditElementsListbox(dsAuditList.Tables[0].Rows[i]["CustID"].ToString(), dsAuditList.Tables[0].Rows[i]["Project_no"].ToString(), dsAuditList.Tables[0].Rows[i]["SupplierName"].ToString());
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objAudit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAudit(TUVPerformAuditModels objAudit, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["Audit_date"], out dateValue) == true)
                {
                    objAudit.Audit_date = dateValue;
                }
                TUVPerformAuditModelsList AuditElemtlist = new TUVPerformAuditModelsList();
                AuditElemtlist.lstAudit = new List<TUVPerformAuditModels>();
                TUVPerformAuditModels obj = new TUVPerformAuditModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAudit.CustID, objAudit.Project_no, objAudit.SupplierID);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        TUVPerformAuditModels objElements = new TUVPerformAuditModels();
                        objElements.id_element = form["Questions " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];
                        objElements.evidence_upload = form["evidence_upload" + i];
                        objElements.finding_category = form["finding_category " + i];
                        AuditElemtlist.lstAudit.Add(objElements);
                    }
                    i++;
                }

                if (objAudit.FunAddPerformAudit(objAudit, AuditElemtlist))
                {
                    TempData["Successdata"] = "Audit Performance details added successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAudit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PerformAuditList", new { id_audit_trans = objAudit.id_audit_trans });
        }

        [AllowAnonymous]
        public ActionResult PerformAuditList(FormCollection form, int? page)
        {
            TUVPerformAuditModelsList AuditElemtlist = new TUVPerformAuditModelsList();
            AuditElemtlist.lstAudit = new List<TUVPerformAuditModels>();
            TUVSupplierModels objSup = new TUVSupplierModels();
            try
            {
                if (Request.QueryString["id_audit_trans"] != null && Request.QueryString["id_audit_trans"] != "")
                {
                    string sid_audit_trans = Request.QueryString["id_audit_trans"];
                    string sSqlstmt = "select t.id_paudit,tt.id_audit,tt.id_audit_trans,SupplierName,Audit_date,Auditors,t.Auditee"
                    + " from t_performaudit_tuv t,t_audit_trans_tuv tt where t.id_audit_trans=tt.id_audit_trans and"
                    + " t.id_audit_trans='" + sid_audit_trans + "' and t.Active=1 and tt.Active=1";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsAuditList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                TUVPerformAuditModels objAuditModel = new TUVPerformAuditModels
                                {
                                    id_audit = dsAuditList.Tables[0].Rows[i]["id_audit"].ToString(),
                                    id_paudit = dsAuditList.Tables[0].Rows[i]["id_paudit"].ToString(),
                                    id_audit_trans = dsAuditList.Tables[0].Rows[i]["id_audit_trans"].ToString(),
                                    Auditors = dsAuditList.Tables[0].Rows[i]["Auditors"].ToString(),
                                    Auditee = dsAuditList.Tables[0].Rows[i]["Auditee"].ToString(),
                                    SupplierName = objSup.GetSupplierByID(dsAuditList.Tables[0].Rows[i]["SupplierName"].ToString()),
                                };
                                ViewBag.id_audit = dsAuditList.Tables[0].Rows[i]["id_audit"].ToString();
                                DateTime dateValue;
                                if (DateTime.TryParse(dsAuditList.Tables[0].Rows[i]["Audit_date"].ToString(), out dateValue))
                                {
                                    objAuditModel.Audit_date = dateValue;
                                }
                                AuditElemtlist.lstAudit.Add(objAuditModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in PerformAuditList: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be null";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAuditList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(AuditElemtlist.lstAudit.ToList().ToPagedList(page ?? 1, 1000));
        }

        [AllowAnonymous]
        public ActionResult PerformAuditDetails()
        {
            try
            {
                TUVSupplierModels objSup = new TUVSupplierModels();
                if (Request.QueryString["id_paudit"] != null && Request.QueryString["id_paudit"] != "")
                {
                    string sid_paudit = Request.QueryString["id_paudit"];

                    string sSqlstmt = "select tt.id_audit_trans,SupplierName,Audit_criteria,tp.Audit_date,Auditors,tp.Auditee"
                    + " from t_performaudit_tuv tp,t_audit_tuv t,t_audit_trans_tuv tt where t.id_audit=tt.id_audit"
                    + " and  tp.id_audit_trans=tt.id_audit_trans and id_paudit='" + sid_paudit + "'";

                    DataSet dsAudit = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAudit.Tables.Count > 0 && dsAudit.Tables[0].Rows.Count > 0)
                    {
                        TUVPerformAuditModels objAudit = new TUVPerformAuditModels
                        {
                            SupplierName = objSup.GetSupplierByID(dsAudit.Tables[0].Rows[0]["SupplierName"].ToString()),
                            Audit_criteria = dsAudit.Tables[0].Rows[0]["Audit_criteria"].ToString(),
                            Auditors = dsAudit.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = dsAudit.Tables[0].Rows[0]["Auditee"].ToString(),
                            id_audit_trans = dsAudit.Tables[0].Rows[0]["id_audit_trans"].ToString(),
                        };
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAudit.Tables[0].Rows[0]["Audit_date"].ToString(), out dtValue))
                        {
                            objAudit.Audit_date = dtValue;
                        }

                        sSqlstmt = "SELECT id_checklist,id_paudit,id_element,id_auditratings,"
                       + "comment,evidence_upload,finding_category FROM t_performauditchecklist_tuv where id_paudit='"
                           + sid_paudit + "'";

                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);
                        TUVPerformAuditModelsList AuditElemtlist = new TUVPerformAuditModelsList();
                        AuditElemtlist.lstAudit = new List<TUVPerformAuditModels>();

                        TUVPerformAuditModels obj = new TUVPerformAuditModels();

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            TUVPerformAuditModels objElements = new TUVPerformAuditModels
                            {
                                id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
                                id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
                                comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
                                evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
                                finding_category = dsAuditElement.Tables[0].Rows[i]["finding_category"].ToString(),
                            };
                            AuditElemtlist.lstAudit.Add(objElements);
                        }

                        ViewBag.AuditElement = AuditElemtlist;
                        ViewBag.AuditRating = obj.GetAuditRating();
                        return View(objAudit);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = " Id cannot be null";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAuditDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditList");
        }

        [AllowAnonymous]
        public ActionResult PerformAuditEdit()
        {
            try
            {
                TUVSupplierModels objSup = new TUVSupplierModels();
                if (Request.QueryString["id_paudit"] != null && Request.QueryString["idt_checklist"] != "")
                {
                    string sid_paudit = Request.QueryString["id_paudit"];

                    string sSqlstmt = "select id_paudit,tp.id_audit_trans,CustID,Project_no,tp.Audit_date,tp.Auditors,tp.Auditee,SupplierName,Audit_criteria"
                     + " from t_audit_tuv t,t_audit_trans_tuv tt,t_performaudit_tuv tp where t.id_audit=tt.id_audit and"
                     + " tp.id_audit_trans=tt.id_audit_trans and id_paudit='" + sid_paudit + "'";

                    DataSet dsAuditList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsAuditList.Tables.Count > 0 && dsAuditList.Tables[0].Rows.Count > 0)
                    {
                        TUVPerformAuditModels objAudit = new TUVPerformAuditModels
                        {
                            id_audit_trans = dsAuditList.Tables[0].Rows[0]["id_audit_trans"].ToString(),
                            id_paudit = dsAuditList.Tables[0].Rows[0]["id_paudit"].ToString(),
                            Auditors = dsAuditList.Tables[0].Rows[0]["Auditors"].ToString(),
                            Auditee = dsAuditList.Tables[0].Rows[0]["Auditee"].ToString(),
                            SupplierName = objSup.GetSupplierByID(dsAuditList.Tables[0].Rows[0]["SupplierName"].ToString()),
                            Audit_criteria = (dsAuditList.Tables[0].Rows[0]["Audit_criteria"].ToString()),
                            CustID = (dsAuditList.Tables[0].Rows[0]["CustID"].ToString()),
                            Project_no = (dsAuditList.Tables[0].Rows[0]["Project_no"].ToString()),
                            SupplierID = (dsAuditList.Tables[0].Rows[0]["SupplierName"].ToString())
                        };
                        string CustID = dsAuditList.Tables[0].Rows[0]["CustID"].ToString();
                        string Project_no = dsAuditList.Tables[0].Rows[0]["Project_no"].ToString();
                        string SupplierID = (dsAuditList.Tables[0].Rows[0]["SupplierName"].ToString());
                        DateTime dtValue;
                        if (DateTime.TryParse(dsAuditList.Tables[0].Rows[0]["Audit_date"].ToString(), out dtValue))
                        {
                            objAudit.Audit_date = dtValue;
                        }

                        sSqlstmt = "select id_checklist,id_paudit,id_element,id_auditratings,comment,evidence_upload,finding_category"
                        + " from t_performauditchecklist_tuv where id_paudit='" + sid_paudit + "'";
                        DataSet dsAuditElement = objGlobaldata.Getdetails(sSqlstmt);

                        TUVPerformAuditModelsList AuditElemtlist = new TUVPerformAuditModelsList();
                        AuditElemtlist.lstAudit = new List<TUVPerformAuditModels>();

                        TUVPerformAuditModels obj = new TUVPerformAuditModels();

                        Dictionary<string, string> dicPerformanceElements = new Dictionary<string, string>();

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            ViewBag.dsAudit = dsAuditElement;
                        }

                        for (int i = 0; dsAuditElement.Tables.Count > 0 && i < dsAuditElement.Tables[0].Rows.Count; i++)
                        {
                            TUVPerformAuditModels objElements = new TUVPerformAuditModels
                            {
                                id_element = obj.GetAuditQuestionNameById(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString()),
                                id_auditratings = obj.GetAuditRatingNameById(dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString()),
                                comment = dsAuditElement.Tables[0].Rows[i]["comment"].ToString(),
                                evidence_upload = dsAuditElement.Tables[0].Rows[i]["evidence_upload"].ToString(),
                                finding_category = dsAuditElement.Tables[0].Rows[i]["finding_category"].ToString(),
                            };

                            dicPerformanceElements.Add(dsAuditElement.Tables[0].Rows[i]["id_element"].ToString(), dsAuditElement.Tables[0].Rows[i]["id_auditratings"].ToString());
                            AuditElemtlist.lstAudit.Add(objElements);
                        }

                        ViewBag.PerformanceElement = AuditElemtlist;
                        ViewBag.dicPerformanceElement = dicPerformanceElements;
                        ViewBag.AuditQuestions = obj.GetAuditElementsListbox(CustID, Project_no, SupplierID);
                        ViewBag.AuditRating = obj.GetAuditRating();
                        ViewBag.FindingCategory = objGlobaldata.GetConstantValue("Findingcategory");
                        return View(objAudit);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("AuditList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("AuditList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("AuditList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerformAuditEdit(TUVPerformAuditModels objAudit, FormCollection form)
        {
            try
            {
                DateTime dateValue;
                if (DateTime.TryParse(form["Audit_date"], out dateValue) == true)
                {
                    objAudit.Audit_date = dateValue;
                }
                TUVPerformAuditModelsList AuditElemtlist = new TUVPerformAuditModelsList();
                AuditElemtlist.lstAudit = new List<TUVPerformAuditModels>();
                TUVPerformAuditModels obj = new TUVPerformAuditModels();
                MultiSelectList AuditQuestions = obj.GetAuditElementsListbox(objAudit.CustID, objAudit.Project_no, objAudit.SupplierID);
                int cnt = Convert.ToInt16(form["itmctn"]);
                int i = 1;

                foreach (var item in AuditQuestions)
                {
                    if (i <= Convert.ToInt16(form["itmctn"]))
                    {
                        TUVPerformAuditModels objElements = new TUVPerformAuditModels();
                        objElements.id_checklist = form["id_checklist " + i];
                        objElements.id_element = form["Questions " + item.Value];
                        objElements.id_auditratings = form["id_auditratings " + item.Value];
                        objElements.comment = form["comment " + i];
                        objElements.finding_category = form["finding_category " + i];
                        string upload = form["evidence_upload" + i];
                        if (upload != null)
                        {
                            objElements.evidence_upload = form["evidence_upload" + i];
                        }
                        AuditElemtlist.lstAudit.Add(objElements);
                    }
                    i++;
                }

                if (objAudit.FunUpdateAuditPerformance(objAudit, AuditElemtlist))
                {
                    TempData["Successdata"] = "Audit Performance details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PerformAuditEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PerformAuditList", new { id_audit_trans = objAudit.id_audit_trans });
        }

        [HttpPost]
        public JsonResult AuditPerformanceDelete(FormCollection form)
        {
            try
            {
                if (form["id_paudit"] != null && form["id_paudit"] != "")
                {
                    TUVPerformAuditModels Doc = new TUVPerformAuditModels();
                    string sid_paudit = form["id_paudit"];

                    if (Doc.FunDeleteAuditPerformance(sid_paudit))
                    {
                        TempData["Successdata"] = "Deleted successfully";
                        return Json("Success");
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        return Json("Failed");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Audit Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AuditPerformanceDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Evidence_upload = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/TUV"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/TUV/" + "Evidence" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
            }

            return Json("Failed");
        }
    }
}