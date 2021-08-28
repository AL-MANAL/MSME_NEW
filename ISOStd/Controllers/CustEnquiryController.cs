using ISOStd.Models;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustEnquiryController:Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public CustEnquiryController()
        {
            ViewBag.Menutype = "Enquiry";
        }

        public ActionResult AddCustEnquiry()
        {
            return InitilizeCustEnquiry();
        }

        private ActionResult InitilizeCustEnquiry()
        {
            try
            {
                CustEnquiryModels objcomp = new CustEnquiryModels();
                ViewBag.EnquiryMode = objcomp.GetModeofEnquiryList();
                ViewBag.Status = objcomp.GetEnquiryStatusList();
                ViewBag.CompanyName = objGlobaldata.GetSupplierNameList();
                ViewBag.Incharge = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in InitilizeCustEnquiry: " + ex.ToString());
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddCustEnquiry(CustEnquiryModels objEnquiry, FormCollection form, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {

               // objEnquiry.CustomerName = form["CustomerName"];             


                DateTime dateValue;

                if (DateTime.TryParse(form["date_enquiry"], out dateValue) == true)
                {
                    objEnquiry.date_enquiry = dateValue;
                }
                if (DateTime.TryParse(form["date_fax"], out dateValue) == true)
                {
                    objEnquiry.date_fax = dateValue;
                }
                if (DateTime.TryParse(form["date_lpo"], out dateValue) == true)
                {
                    objEnquiry.date_lpo = dateValue;
                }
                HttpPostedFileBase files = Request.Files[0];

                if (file != null && files.ContentLength > 0)
                {
                    foreach (var doc in file)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustEnquiry"), Path.GetFileName(doc.FileName));
                            string sFilename = objEnquiry.id_enquiry + "_" + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            doc.SaveAs(sFilepath + "/" + sFilename);
                            objEnquiry.upload = objEnquiry.upload + "," + "~/DataUpload/MgmtDocs/CustEnquiry/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objEnquiry.upload = objEnquiry.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objEnquiry.FunAddCustEnquiry(objEnquiry))
                {
                    TempData["Successdata"] = "Customer Enquiry registered successfully";
                    int Id=0;
                    DataSet ds= objGlobaldata.Getdetails("Select max(id_enquiry) as Id from t_custenquiry where active =1");
                    if(ds.Tables[0].Rows.Count>0)
                    {
                        Id = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"].ToString());
                    }

                    objEnquiry.SendCustEquiryEmail(Id, "Customer Eqnuiry Registered");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustEnquiry: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustEnquiryList");
        }
        
        [AllowAnonymous]
        public ActionResult CustEnquiryList(string SearchText, string Approvestatus, int? page, string Company)
        {
            CustEnquiryModels objcust = new CustEnquiryModels();
            CustEnquiryModelsList objList = new CustEnquiryModelsList();
            objList.EnquiryList = new List<CustEnquiryModels>();
           
            try
            {

                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                ViewBag.CompanyName = objGlobaldata.GetSupplierNameList();

                string sSqlstmt = "select id_enquiry,mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge,"
                    + "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno,faxno from t_custenquiry where Active=1";

                string sSearchtext = "";

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = sSearchtext + "  and (projectname ='" + SearchText + "' or projectname like '" + SearchText + "%' or projectname like '%" + SearchText + "%')";
                }
              
                if (Company != "" && Company != null && Company != "Select")
                {
                    ViewBag.CompanyNameval = (Company);
                    sSearchtext = sSearchtext + " and companyname = '" + Company + "'";
                }               

                sSqlstmt = sSqlstmt + sSearchtext + " order by date_enquiry desc";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {

                   

                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {

                        try
                        {
                            CustEnquiryModels objenquiry = new CustEnquiryModels
                            {
                                id_enquiry = Convert.ToInt16(dsList.Tables[0].Rows[i]["id_enquiry"].ToString()),
                                mode_enquiry = objcust.GetModeofEnquiryById(dsList.Tables[0].Rows[i]["mode_enquiry"].ToString()),
                                companyname = (dsList.Tables[0].Rows[i]["companyname"].ToString()),
                                description = dsList.Tables[0].Rows[i]["description"].ToString(),
                                projectname = dsList.Tables[0].Rows[i]["projectname"].ToString(),
                                contactperson = dsList.Tables[0].Rows[i]["contactperson"].ToString(),
                                location = dsList.Tables[0].Rows[i]["location"].ToString(),
                                sales_incharge = dsList.Tables[0].Rows[i]["sales_incharge"].ToString(),
                                incharge = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[i]["incharge"].ToString()),
                                amt = Convert.ToDecimal(dsList.Tables[0].Rows[i]["amt"].ToString()),
                                ref_no = dsList.Tables[0].Rows[i]["ref_no"].ToString(),
                                lpo_no = dsList.Tables[0].Rows[i]["lpo_no"].ToString(),
                                lpo_amt = Convert.ToDecimal(dsList.Tables[0].Rows[i]["lpo_amt"].ToString()),
                                status = objcust.GetEnquiryStatusById(dsList.Tables[0].Rows[i]["status"].ToString()),
                                upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                telno = dsList.Tables[0].Rows[i]["telno"].ToString(),
                                faxno = dsList.Tables[0].Rows[i]["faxno"].ToString(),
                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsList.Tables[0].Rows[i]["date_enquiry"].ToString() != ""
                                && DateTime.TryParse(dsList.Tables[0].Rows[i]["date_enquiry"].ToString(), out dtDocDate))
                            {
                                objenquiry.date_enquiry = dtDocDate;
                            }

                            if (dsList.Tables[0].Rows[i]["date_fax"].ToString() != ""
                               && DateTime.TryParse(dsList.Tables[0].Rows[i]["date_fax"].ToString(), out dtDocDate))
                            {
                                objenquiry.date_fax = dtDocDate;
                            }

                            if (dsList.Tables[0].Rows[i]["date_lpo"].ToString() != ""
                               && DateTime.TryParse(dsList.Tables[0].Rows[i]["date_lpo"].ToString(), out dtDocDate))
                            {
                                objenquiry.date_lpo = dtDocDate;
                            }
                            objList.EnquiryList.Add(objenquiry);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustEnquiryList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objList.EnquiryList.ToList().ToPagedList(page ?? 1, 40));
        }

        [AllowAnonymous]
        public ActionResult CustEnquiryEdit()
        {
            ViewBag.SubMenutype = "CustomerComplaint";
            CustEnquiryModels objcust = new CustEnquiryModels();

            try
            {
                InitilizeCustEnquiry();

                if (Request.QueryString["id_enquiry"] != null && Request.QueryString["id_enquiry"] != "")
                {
                    string sid_enquiry = Request.QueryString["id_enquiry"];

                    string sSqlstmt = "select id_enquiry,mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge,"
                    + "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno,faxno from t_custenquiry where id_enquiry='" + sid_enquiry + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                           objcust = new CustEnquiryModels
                                {
                                    id_enquiry = Convert.ToInt16(dsList.Tables[0].Rows[0]["id_enquiry"].ToString()),
                                    mode_enquiry = objcust.GetModeofEnquiryById(dsList.Tables[0].Rows[0]["mode_enquiry"].ToString()),
                                    companyname = (dsList.Tables[0].Rows[0]["companyname"].ToString()),
                                    description = dsList.Tables[0].Rows[0]["description"].ToString(),
                                    projectname = dsList.Tables[0].Rows[0]["projectname"].ToString(),
                                    contactperson = dsList.Tables[0].Rows[0]["contactperson"].ToString(),
                                    location = dsList.Tables[0].Rows[0]["location"].ToString(),
                                    sales_incharge = dsList.Tables[0].Rows[0]["sales_incharge"].ToString(),
                                    incharge = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["incharge"].ToString()),
                                    amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["amt"].ToString()),
                                    ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                                    lpo_no = dsList.Tables[0].Rows[0]["lpo_no"].ToString(),
                                    lpo_amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["lpo_amt"].ToString()),
                                    status = objcust.GetEnquiryStatusById(dsList.Tables[0].Rows[0]["status"].ToString()),
                                    upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                                    telno = dsList.Tables[0].Rows[0]["telno"].ToString(),
                                    faxno = dsList.Tables[0].Rows[0]["faxno"].ToString(),
                                };

                                DateTime dtDocDate = new DateTime();
                                if (dsList.Tables[0].Rows[0]["date_enquiry"].ToString() != ""
                                    && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_enquiry"].ToString(), out dtDocDate))
                                {
                                     objcust.date_enquiry = dtDocDate;
                                }

                                if (dsList.Tables[0].Rows[0]["date_fax"].ToString() != ""
                                   && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_fax"].ToString(), out dtDocDate))
                                {
                                      objcust.date_fax = dtDocDate;
                                }

                                if (dsList.Tables[0].Rows[0]["date_lpo"].ToString() != ""
                                   && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_lpo"].ToString(), out dtDocDate))
                                {
                                   objcust.date_lpo = dtDocDate;
                                }
                            
                        else
                        {                          
                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("CustEnquiryList");
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustEnquiryList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustEnquiryList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustEnquiryList");
            }

            return View(objcust);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult CustEnquiryEdit(CustEnquiryModels objEnquiry, FormCollection form, IEnumerable<HttpPostedFileBase> file)
        {
            try
            {                              
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;

                if (DateTime.TryParse(form["date_enquiry"], out dateValue) == true)
                {
                    objEnquiry.date_enquiry = dateValue;
                }

                if (file != null && files.ContentLength > 0)
                {
                    foreach (var document in file)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustEnquiry"), Path.GetFileName(document.FileName));
                            string sFilename =  objEnquiry.id_enquiry + "_"+Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            document.SaveAs(sFilepath + "/" + sFilename);
                            objEnquiry.upload = objEnquiry.upload + "," + "~/DataUpload/MgmtDocs/CustEnquiry/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        }
                    }
                    objEnquiry.upload = objEnquiry.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objEnquiry.upload = objEnquiry.upload + "," + form["QCDocsVal"];
                    objEnquiry.upload = objEnquiry.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objEnquiry.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                     objEnquiry.upload = null;
                }

                if (objEnquiry.FunUpdateCustEnquiry(objEnquiry))
                {
                    TempData["Successdata"] = "Customer Enquiry Updated successfully";
                    objEnquiry.SendCustEquiryEmail(objEnquiry.id_enquiry, "Registerd Customer Enquired Updated");
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustEnquiryList");
        }

        [AllowAnonymous]
        public ActionResult CustEnquiryInfo(string Id)
        {
            ViewBag.SubMenutype = "CustomerComplaint";
            CustEnquiryModels objcust = new CustEnquiryModels();

            try
            {              

                if (Id != null && Id!= "")
                {
                   
                    string sSqlstmt = "select id_enquiry,mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge,"
                    + "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno,faxno from t_custenquiry where id_enquiry='" + Id + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objcust = new CustEnquiryModels
                        {
                            id_enquiry = Convert.ToInt16(dsList.Tables[0].Rows[0]["id_enquiry"].ToString()),
                            mode_enquiry = objcust.GetModeofEnquiryById(dsList.Tables[0].Rows[0]["mode_enquiry"].ToString()),
                            companyname = (dsList.Tables[0].Rows[0]["companyname"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            projectname = dsList.Tables[0].Rows[0]["projectname"].ToString(),
                            contactperson = dsList.Tables[0].Rows[0]["contactperson"].ToString(),
                            location = dsList.Tables[0].Rows[0]["location"].ToString(),
                            sales_incharge = dsList.Tables[0].Rows[0]["sales_incharge"].ToString(),
                            incharge = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["incharge"].ToString()),
                            amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["amt"].ToString()),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            lpo_no = dsList.Tables[0].Rows[0]["lpo_no"].ToString(),
                            lpo_amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["lpo_amt"].ToString()),
                            status = objcust.GetEnquiryStatusById(dsList.Tables[0].Rows[0]["status"].ToString()),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            telno = dsList.Tables[0].Rows[0]["telno"].ToString(),
                            faxno = dsList.Tables[0].Rows[0]["faxno"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsList.Tables[0].Rows[0]["date_enquiry"].ToString() != ""
                            && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_enquiry"].ToString(), out dtDocDate))
                        {
                            objcust.date_enquiry = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["date_fax"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_fax"].ToString(), out dtDocDate))
                        {
                            objcust.date_fax = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["date_lpo"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_lpo"].ToString(), out dtDocDate))
                        {
                            objcust.date_lpo = dtDocDate;
                        }

                        else
                        {
                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("CustEnquiryList");
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustEnquiryList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustEnquiryList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustEnquiryList");
            }

            return View(objcust);
        }

        [AllowAnonymous]
        public ActionResult CustEnquiryDetails(FormCollection form)
        {
            ViewBag.SubMenutype = "CustomerComplaint";
            CustEnquiryModels objcust = new CustEnquiryModels();

            try
            {
                InitilizeCustEnquiry();

                if (Request.QueryString["id_enquiry"] != null && Request.QueryString["id_enquiry"] != "")
                {
                    string sid_enquiry = Request.QueryString["id_enquiry"];

                    string sSqlstmt = "select id_enquiry,mode_enquiry,date_enquiry,companyname,description,projectname,contactperson,location,sales_incharge,"
                    + "incharge,date_fax,amt,ref_no,date_lpo,lpo_no,lpo_amt,status,upload,telno,faxno from t_custenquiry where id_enquiry='" + sid_enquiry + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objcust = new CustEnquiryModels
                        {
                            id_enquiry = Convert.ToInt16(dsList.Tables[0].Rows[0]["id_enquiry"].ToString()),
                            mode_enquiry = objcust.GetModeofEnquiryById(dsList.Tables[0].Rows[0]["mode_enquiry"].ToString()),
                            companyname = (dsList.Tables[0].Rows[0]["companyname"].ToString()),
                            description = dsList.Tables[0].Rows[0]["description"].ToString(),
                            projectname = dsList.Tables[0].Rows[0]["projectname"].ToString(),
                            contactperson = dsList.Tables[0].Rows[0]["contactperson"].ToString(),
                            location = dsList.Tables[0].Rows[0]["location"].ToString(),
                            sales_incharge = dsList.Tables[0].Rows[0]["sales_incharge"].ToString(),
                            incharge = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["incharge"].ToString()),
                            amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["amt"].ToString()),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            lpo_no = dsList.Tables[0].Rows[0]["lpo_no"].ToString(),
                            lpo_amt = Convert.ToDecimal(dsList.Tables[0].Rows[0]["lpo_amt"].ToString()),
                            status = objcust.GetEnquiryStatusById(dsList.Tables[0].Rows[0]["status"].ToString()),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            telno = dsList.Tables[0].Rows[0]["telno"].ToString(),
                            faxno = dsList.Tables[0].Rows[0]["faxno"].ToString(),
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsList.Tables[0].Rows[0]["date_enquiry"].ToString() != ""
                            && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_enquiry"].ToString(), out dtDocDate))
                        {
                            objcust.date_enquiry = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["date_fax"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_fax"].ToString(), out dtDocDate))
                        {
                            objcust.date_fax = dtDocDate;
                        }

                        if (dsList.Tables[0].Rows[0]["date_lpo"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_lpo"].ToString(), out dtDocDate))
                        {
                            objcust.date_lpo = dtDocDate;
                        }

                        else
                        {
                            TempData["alertdata"] = "Access Denied";
                            return RedirectToAction("CustEnquiryList");
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustEnquiryList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustEnquiryList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustEnquiryList");
            }

            return View(objcust);
        }


        [AllowAnonymous]
        public JsonResult CustEnquiryDelete(FormCollection form)
        {
            try
            {
                
                    if (form["id_enquiry"] != null && form["id_enquiry"] != "")
                    {
                        CustEnquiryModels Doc = new CustEnquiryModels();
                        string sid_enquiry = form["id_enquiry"];


                        if (Doc.FunDeleteCustEnquiry(sid_enquiry))
                        {
                            TempData["Successdata"] = "Enquiry deleted successfully";
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
                        TempData["alertdata"] = "Customer Enquiry Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustEnquiryDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult AddQuotation()
        {
            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox();              
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQuotation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddQuotation(QuotationModels objModel, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {

                objModel.sum = Convert.ToDecimal(form["Tot"]);

                HttpPostedFileBase files = Request.Files[0];

                QuotationModelsList objList = new QuotationModelsList();
                objList.QuotList = new List<QuotationModels>();

                DateTime dateValue;
                if (DateTime.TryParse(form["date_quotation"], out dateValue) == true)
                {
                    objModel.date_quotation = dateValue;
                }
                              
                if (upload != null && files.ContentLength > 0)
                {
                    objModel.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustEnquiry"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            files.SaveAs(sFilepath + "/" + sFilename);
                            objModel.upload = objModel.upload + "," + "~/DataUpload/MgmtDocs/CustEnquiry/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddQuotation-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objModel.upload = objModel.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                for (int i = 0; i < Convert.ToInt16(form["itemcnts"]); i++)
                {
                    if (form["qty" + i] != null && form["price" + i] != null)
                    {
                        QuotationModels objQuot = new QuotationModels();

                        objQuot.description = (form["description" + i]);
                        objQuot.qty = Convert.ToDecimal(form["qty" + i]);
                        objQuot.price = Convert.ToDecimal(form["price" + i]);
                        objQuot.total = Convert.ToDecimal(form["total" + i]);

                        objList.QuotList.Add(objQuot);
                    }
                }

                if (objModel.FunAddQuotation(objModel, objList))
                {
                    TempData["Successdata"] = "Added Customer Quotation successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddQuotation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QuotationList");
        }
        
        [AllowAnonymous]
        public ActionResult QuotationList(FormCollection form, int? page, string chkAll, string ToQuotation, string DateToQuot, string DateFromQuot, string command)
        {

            QuotationModelsList objdisp = new QuotationModelsList();
            objdisp.QuotList = new List<QuotationModels>();

            DateTime dtdate;

            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox(); 
              
                string sSqlstmt = "select id_quotation,date_quotation,ref_no,to_quotation,telephone,email,pro_ref,approved_by,logged_by,upload,sum,vat from t_quotation where active=1";
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    if (DateFromQuot != null && DateTime.TryParse(DateFromQuot, out dtdate))
                    {
                        ViewBag.FromDate = DateFromQuot;
                        sSearchtext = sSearchtext + " and date_quotation <= '" + dtdate.ToString("yyyy/MM/dd") + "'";
                    }

                    if (DateToQuot != "" && DateTime.TryParse(DateToQuot, out dtdate))
                    {
                        ViewBag.ToDate = DateToQuot;
                        sSearchtext = sSearchtext + " and date_quotation >='" + dtdate.ToString("yyyy/MM/dd") + "'";
                    }
                    if (ToQuotation != "" && ToQuotation != null)
                    {
                        ViewBag.ToQuotationVal = ToQuotation;
                        sSearchtext = sSearchtext + " and (to_quotation ='" + ToQuotation + "' or to_quotation like '" + ToQuotation + "%' or to_quotation='%" + ToQuotation
                            + "' or to_quotation like '%" + ToQuotation + "%')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by id_quotation asc";

                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                 
                    for (int i = 0; i < dsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            QuotationModels objMdl = new QuotationModels
                            {

                                id_quotation = dsList.Tables[0].Rows[i]["id_quotation"].ToString(),
                                ref_no = dsList.Tables[0].Rows[i]["ref_no"].ToString(),
                                to_quotation = dsList.Tables[0].Rows[i]["to_quotation"].ToString(),
                                telephone = dsList.Tables[0].Rows[i]["telephone"].ToString(),
                                email = dsList.Tables[0].Rows[i]["email"].ToString(),
                                pro_ref = dsList.Tables[0].Rows[i]["pro_ref"].ToString(),
                                approved_by =objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[i]["approved_by"].ToString()),
                                logged_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[i]["logged_by"].ToString()),
                                upload = dsList.Tables[0].Rows[i]["upload"].ToString(),
                                sum = Convert.ToDecimal(dsList.Tables[0].Rows[i]["sum"].ToString()),
                                vat = Convert.ToDecimal(dsList.Tables[0].Rows[i]["vat"].ToString()),
                            };

                            DateTime dtDocDate;

                            if (dsList.Tables[0].Rows[i]["date_quotation"].ToString() != ""
                               && DateTime.TryParse(dsList.Tables[0].Rows[i]["date_quotation"].ToString(), out dtDocDate))
                            {
                                objMdl.date_quotation = dtDocDate;
                            }

                            objdisp.QuotList.Add(objMdl);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QuotationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objdisp.QuotList.ToList().ToPagedList(page ?? 1, 1000));
        }


        [AllowAnonymous]
        public ActionResult QuotationEdit()
        {
            QuotationModels objMdl = new QuotationModels();

            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox();

                if (Request.QueryString["id_quotation"] != null && Request.QueryString["id_quotation"] != "")
                {
                    string sid_quotation = Request.QueryString["id_quotation"];

                    string sSqlstmt = "select id_quotation,date_quotation,ref_no,to_quotation,telephone,email,pro_ref,approved_by,logged_by,upload,sum,vat from t_quotation where id_quotation='" + sid_quotation + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objMdl = new QuotationModels
                        {
                            id_quotation = dsList.Tables[0].Rows[0]["id_quotation"].ToString(),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            to_quotation = dsList.Tables[0].Rows[0]["to_quotation"].ToString(),
                            telephone = dsList.Tables[0].Rows[0]["telephone"].ToString(),
                            email = dsList.Tables[0].Rows[0]["email"].ToString(),
                            pro_ref = dsList.Tables[0].Rows[0]["pro_ref"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            sum = Convert.ToDecimal(dsList.Tables[0].Rows[0]["sum"].ToString()),
                            vat = Convert.ToDecimal(dsList.Tables[0].Rows[0]["vat"].ToString()),
                        };

                        DateTime dtDocDate;

                        if (dsList.Tables[0].Rows[0]["date_quotation"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_quotation"].ToString(), out dtDocDate))
                        {
                            objMdl.date_quotation = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("QuotationList");/*Change*/
                    }

                    QuotationModelsList objquot = new QuotationModelsList();
                    objquot.QuotList = new List<QuotationModels>();

                    sSqlstmt = "select id_quo_trans,id_quotation,description,qty,price,total from t_quotation_trans"
                        + " where id_quotation='" + sid_quotation + "'";

                    DataSet dsQuotList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsQuotList.Tables.Count > 0 && dsQuotList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsQuotList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                QuotationModels objModel = new QuotationModels
                                {
                                    id_quo_trans = dsQuotList.Tables[0].Rows[i]["id_quo_trans"].ToString(),
                                    id_quotation = dsQuotList.Tables[0].Rows[i]["id_quotation"].ToString(),                                   
                                    description = dsQuotList.Tables[0].Rows[i]["description"].ToString(),
                                    qty = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["qty"].ToString()),
                                    price = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["price"].ToString()),
                                    total = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["total"].ToString()),
                                };

                                objquot.QuotList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in QuotationEdit: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("QuotationList");
                            }
                        }
                        ViewBag.objQuotList = objquot;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("QuotationList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QuotationList");
            }
            return View(objMdl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult QuotationEdit(QuotationModels objQuot, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {

            QuotationModelsList objList = new QuotationModelsList();
            objList.QuotList = new List<QuotationModels>();
            try
            {
                objQuot.sum = Convert.ToDecimal(form["Tot"]);

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                DateTime dateValue;
                if (DateTime.TryParse(form["date_quotation"], out dateValue) == true)
                {
                    objQuot.date_quotation = dateValue;
                }


                if (upload != null && files.ContentLength > 0)
                {
                    objQuot.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/CustEnquiry"), Path.GetFileName(file.FileName));
                            string sFilename = Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            files.SaveAs(sFilepath + "/" + sFilename);
                            objQuot.upload = objQuot.upload + "," + "~/DataUpload/MgmtDocs/CustEnquiry/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in QuotationEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objQuot.upload = objQuot.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objQuot.upload = objQuot.upload + "," + form["QCDocsVal"];
                    objQuot.upload = objQuot.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objQuot.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objQuot.upload = null;
                }

                int count = Convert.ToInt16(form["itemcnts"]);
                for (int i = 0; i < count; i++)
                {
                    if (form["qty" + i] != null && form["price" + i] != null)
                    {
                        QuotationModels objModel = new QuotationModels();

                        objModel.id_quotation = form["id_quotation" + i];
                        objModel.description = form["description" + i];
                        objModel.qty = Convert.ToDecimal(form["qty" + i]);
                        objModel.price = Convert.ToDecimal(form["price" + i]);
                        objModel.total = Convert.ToDecimal(form["total" + i]);

                        objList.QuotList.Add(objModel);
                    }
                }
                
                if (objQuot.FunUpdateQuotation(objQuot, objList))
                {
                    TempData["Successdata"] = "Uploaded successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("QuotationList");
        }

        [AllowAnonymous]
        public ActionResult QuotationInfo(string Id)
        {
            QuotationModels objMdl = new QuotationModels();
            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox();

                if (Id != null && Id != "")
                {                   
                    string sSqlstmt = "select id_quotation,date_quotation,ref_no,to_quotation,telephone,email,pro_ref,approved_by,logged_by,upload,sum,vat from t_quotation where id_quotation='" + Id + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objMdl = new QuotationModels
                        {
                            id_quotation = dsList.Tables[0].Rows[0]["id_quotation"].ToString(),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            to_quotation = dsList.Tables[0].Rows[0]["to_quotation"].ToString(),
                            telephone = dsList.Tables[0].Rows[0]["telephone"].ToString(),
                            email = dsList.Tables[0].Rows[0]["email"].ToString(),
                            pro_ref = dsList.Tables[0].Rows[0]["pro_ref"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            sum = Convert.ToDecimal(dsList.Tables[0].Rows[0]["sum"].ToString()),
                            vat = Convert.ToDecimal(dsList.Tables[0].Rows[0]["vat"].ToString()),
                        };

                        DateTime dtDocDate;

                        if (dsList.Tables[0].Rows[0]["date_quotation"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_quotation"].ToString(), out dtDocDate))
                        {
                            objMdl.date_quotation = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("QuotationList");/*Change*/
                    }

                    QuotationModelsList objquot = new QuotationModelsList();
                    objquot.QuotList = new List<QuotationModels>();

                    sSqlstmt = "select id_quo_trans,id_quotation,description,qty,price,total from t_quotation_trans"
                        + " where id_quotation='" + Id + "'";

                    DataSet dsQuotList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.CustQuot = dsQuotList;
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("QuotationList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationInfo: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QuotationList");
            }
            return View(objMdl);
        }

        [AllowAnonymous]
        public ActionResult QuotationDetail()
        {
            QuotationModels objMdl = new QuotationModels();
            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox();

                if (Request.QueryString["id_quotation"] != null && Request.QueryString["id_quotation"] != "")
                {
                    string sid_quotation = Request.QueryString["id_quotation"];

                    string sSqlstmt = "select id_quotation,date_quotation,ref_no,to_quotation,telephone,email,pro_ref,approved_by,logged_by,upload,sum,vat from t_quotation where id_quotation='" + sid_quotation + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objMdl = new QuotationModels
                        {
                            id_quotation = dsList.Tables[0].Rows[0]["id_quotation"].ToString(),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            to_quotation = dsList.Tables[0].Rows[0]["to_quotation"].ToString(),
                            telephone = dsList.Tables[0].Rows[0]["telephone"].ToString(),
                            email = dsList.Tables[0].Rows[0]["email"].ToString(),
                            pro_ref = dsList.Tables[0].Rows[0]["pro_ref"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            sum = Convert.ToDecimal(dsList.Tables[0].Rows[0]["sum"].ToString()),
                            vat = Convert.ToDecimal(dsList.Tables[0].Rows[0]["vat"].ToString()),
                        };

                        DateTime dtDocDate;

                        if (dsList.Tables[0].Rows[0]["date_quotation"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_quotation"].ToString(), out dtDocDate))
                        {
                            objMdl.date_quotation = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("QuotationList");/*Change*/
                    }

                    QuotationModelsList objquot = new QuotationModelsList();
                    objquot.QuotList = new List<QuotationModels>();

                    sSqlstmt = "select id_quo_trans,id_quotation,description,qty,price,total from t_quotation_trans"
                        + " where id_quotation='" + sid_quotation + "'";

                    DataSet dsQuotList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.CustQuot = dsQuotList;
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("QuotationList");/*Change*/
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationDetail: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QuotationList");
            }
            return View(objMdl);
        }

        [AllowAnonymous]
        public ActionResult QuotationPrint(FormCollection form)
        {
            QuotationModels objMdl = new QuotationModels();

            try
            {
                ViewBag.Approver = objGlobaldata.GetHrEmployeeListbox();
                string sid_quotation = form["id_quotation"];

                if (sid_quotation != null && sid_quotation != "")
                { 
                    string sSqlstmt = "select id_quotation,date_quotation,ref_no,to_quotation,telephone,email,pro_ref,approved_by,logged_by,upload,sum,vat from t_quotation where id_quotation='" + sid_quotation + "'";

                    DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                    {
                        objMdl = new QuotationModels
                        {
                            id_quotation = dsList.Tables[0].Rows[0]["id_quotation"].ToString(),
                            ref_no = dsList.Tables[0].Rows[0]["ref_no"].ToString(),
                            to_quotation = dsList.Tables[0].Rows[0]["to_quotation"].ToString(),
                            telephone = dsList.Tables[0].Rows[0]["telephone"].ToString(),
                            email = dsList.Tables[0].Rows[0]["email"].ToString(),
                            pro_ref = dsList.Tables[0].Rows[0]["pro_ref"].ToString(),
                            approved_by = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["approved_by"].ToString()),
                            logged_by = dsList.Tables[0].Rows[0]["logged_by"].ToString(),
                            upload = dsList.Tables[0].Rows[0]["upload"].ToString(),
                            sum = Convert.ToDecimal(dsList.Tables[0].Rows[0]["sum"].ToString()),
                            vat = Convert.ToDecimal(dsList.Tables[0].Rows[0]["vat"].ToString()),
                        };

                        DateTime dtDocDate;

                        if (dsList.Tables[0].Rows[0]["date_quotation"].ToString() != ""
                           && DateTime.TryParse(dsList.Tables[0].Rows[0]["date_quotation"].ToString(), out dtDocDate))
                        {
                            objMdl.date_quotation = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("QuotationList");/*Change*/
                    }

                    QuotationModelsList objquot = new QuotationModelsList();
                    objquot.QuotList = new List<QuotationModels>();

                    sSqlstmt = "select id_quo_trans,id_quotation,description,qty,price,total from t_quotation_trans"
                        + " where id_quotation='" + sid_quotation + "'";

                    DataSet dsQuotList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsQuotList.Tables.Count > 0 && dsQuotList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsQuotList.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                QuotationModels objModel = new QuotationModels
                                {
                                    id_quo_trans = dsQuotList.Tables[0].Rows[i]["id_quo_trans"].ToString(),
                                    id_quotation = dsQuotList.Tables[0].Rows[i]["id_quotation"].ToString(),
                                    description = dsQuotList.Tables[0].Rows[i]["description"].ToString(),
                                    qty = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["qty"].ToString()),
                                    price = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["price"].ToString()),
                                    total = Convert.ToDecimal(dsQuotList.Tables[0].Rows[i]["total"].ToString()),
                                };

                                objquot.QuotList.Add(objModel);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in QuotationPrint: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                                return RedirectToAction("QuotationList");
                            }
                        }
                        ViewBag.objQuotList = objquot;
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("QuotationList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationPrint: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("QuotationList");
            }
            //return View(objMdl);
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();

            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            string footer = "--footer-right \"Date: [date] [time]\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"";

            return new ViewAsPdf("QuotationPrint")
            {
                FileName = "QuotationPdf.pdf",
                Cookies = cookieCollection,
                CustomSwitches = footer
            };
        }

        [AllowAnonymous]
        public JsonResult QuotationDelete(FormCollection form)
        {
            try
            {
               
                    if (form["id_quotation"] != null && form["id_quotation"] != "")
                    {
                        QuotationModels Doc = new QuotationModels();
                        string sid_quotation = form["id_quotation"];


                        if (Doc.FunDeleteQuotation(sid_quotation))
                        {
                            TempData["Successdata"] = "Quotation deleted successfully";
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
                        TempData["alertdata"] = "Quotation Id cannot be Null or empty";
                        return Json("Failed");
                    }
                

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in QuotationDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FungetTotal(decimal price, decimal qty)
        {            
            decimal Total = 0;

            if (price != 0 && qty != 0)
            {
                Total = price * qty;
            }
            return Json(Total);
        }

        public JsonResult FunGetCompanyDetails(string companyname)
        {
            CustEnquiryModels objModel = new CustEnquiryModels();
            CustEnquiryModelsList objList = new CustEnquiryModelsList();
            objList.EnquiryList = new List<CustEnquiryModels>();
            try
            {
                DataSet dsList = objGlobaldata.Getdetails("Select ContactPerson,FaxNo from t_supplier where SupplierName='" + companyname + "' and active =1");
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                      try
                        {
                           objModel = new CustEnquiryModels
                           {
                               contactperson = dsList.Tables[0].Rows[0]["ContactPerson"].ToString(),
                               faxno = dsList.Tables[0].Rows[0]["FaxNo"].ToString()                           
                           };
                            objList.EnquiryList.Add(objModel);

                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in FunGetCompanyDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }                    
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetCompanyDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            
            return Json(objModel);
        }

        public JsonResult FungetSumTotal(decimal total, decimal Tot)
        {
            decimal Sum = 0;

            if (total != 0)
            {
                Sum = total + Tot;
            }

            return Json(Sum);
        }        

        public JsonResult FungetSum(decimal total, decimal Tot)
        {
            decimal Sum = 0;

            if (total != 0)
            {
                Sum = Tot - total;
            }

            return Json(Sum);
        }

    }
}