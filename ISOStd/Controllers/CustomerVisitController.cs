using System.Web.UI;
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
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustomerVisitController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public CustomerVisitController()
        {
            ViewBag.Menutype = "CustomerMgmt";
            ViewBag.SubMenutype = "CustomerVisit";
        }      

        public ActionResult Index()
        {
            return View();
        }        

        [AllowAnonymous]
        public ActionResult AddCustomerVisit()
        {
            CustomerVisitModels ObjMdl = new CustomerVisitModels();
            try
            {
                ObjMdl.Branch = objGlobaldata.GetCurrentUserSession().division;
                ObjMdl.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                ObjMdl.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(ObjMdl.Branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(ObjMdl.Branch);
                ViewBag.Product_id = objGlobaldata.GetDropdownList("Product desc");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerVisit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(ObjMdl);
        }

        [AllowAnonymous]
        public JsonResult CustomerVisitDocDelete(FormCollection form)
        {
            try
            {                
                    if (form["CustomerVisitId"] != null && form["CustomerVisitId"] != "")
                    {

                        CustomerVisitModels Doc = new CustomerVisitModels();
                        string sCustomerVisitId = form["CustomerVisitId"];


                        if (Doc.FunDeleteCustomerVisitDoc(sCustomerVisitId))
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
                    TempData["alertdata"] = "Access Denied";
                    return Json("Access Denied");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCustomerVisit(CustomerVisitModels objCustomerVisitModels, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objCustomerVisitModels.Branch = form["Branch"];
                objCustomerVisitModels.Department = form["Department"];
                objCustomerVisitModels.Location = form["Location"];

                DateTime dateValue;

                if (DateTime.TryParse(form["PlannedDate"], out dateValue) == true)
                {
                    objCustomerVisitModels.PlannedDate = dateValue;
                }
                if (DateTime.TryParse(form["DateOfVisit"], out dateValue) == true)
                {
                    objCustomerVisitModels.DateOfVisit = dateValue;
                }
                if (DateTime.TryParse(form["FollowUp_Date"], out dateValue) == true)
                {
                    objCustomerVisitModels.FollowUp_Date = dateValue;
                }

                objCustomerVisitModels.Branch = form["Branch"];

                objCustomerVisitModels.ProcessedBy = objGlobaldata.GetCurrentUserSession().empid;

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(file.FileName));
                        string sFilename = "CustomerVisit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objCustomerVisitModels.DocUploadPath = "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddCustomerVisit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }


                if (objCustomerVisitModels.FunAddCustomerVisit(objCustomerVisitModels))
                {
                    TempData["Successdata"] = "Added Visit details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustomerVisit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerVisitList");
        }
               
        [AllowAnonymous]
        public ActionResult CustomerVisitList(string SearchText, string PlanfromDate, string PlanToDate, string chkAll, int? Page, string branch_name)
        {
            CustomerVisitModelsList objCustomerVisitList = new CustomerVisitModelsList();
            objCustomerVisitList.lstCustomerVisit = new List<CustomerVisitModels>();
            ViewBag.Branch_id = objGlobaldata.GetCompanyBranchListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "SELECT CustomerVisitId, Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber, Designation, Address,"
                    + " Product, FollowUp_Date, Outcomeof_Visit, Remarks,City,ProcessedBy,DocUploadPath,Location,Department FROM t_customervisit where Active=1";

                ViewBag.BranchId = objGlobaldata.GetCompanyBranchListbox();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " (CustomerName ='" + SearchText + "' or CustomerName like '" + SearchText + "%')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
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

                    if (PlanfromDate != null && DateTime.TryParse(PlanfromDate, out dtFromdate) && DateTime.TryParse(PlanToDate, out dtToDate))
                    {
                        ViewBag.PlanfromDate = PlanfromDate;
                        ViewBag.PlanToDate = PlanToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and PlannedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd")
                                + "' or FollowUp_Date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and PlannedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd")
                                + "' or FollowUp_Date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by CustomerName asc";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                    try
                    {  
                        CustomerVisitModels objCustomer = new CustomerVisitModels
                        {
                            CustomerVisitId = dsCustomer.Tables[0].Rows[i]["CustomerVisitId"].ToString(),
                            Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[i]["Branch"].ToString()),
                            CustomerName = dsCustomer.Tables[0].Rows[i]["CustomerName"].ToString().ToUpper(),
                            ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
                            EmailID = dsCustomer.Tables[0].Rows[i]["EmailID"].ToString(),
                            ContactNumber = dsCustomer.Tables[0].Rows[i]["ContactNumber"].ToString(),
                            Designation = dsCustomer.Tables[0].Rows[i]["Designation"].ToString(),
                            Address = dsCustomer.Tables[0].Rows[i]["Address"].ToString(),
                            Product = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[i]["Product"].ToString()),
                            Outcomeof_Visit = dsCustomer.Tables[0].Rows[i]["Outcomeof_Visit"].ToString(),
                            Remarks = dsCustomer.Tables[0].Rows[i]["Remarks"].ToString(),
                            ProcessedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["ProcessedBy"].ToString()),
                            City = dsCustomer.Tables[0].Rows[i]["City"].ToString(),
                            DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["PlannedDate"].ToString(), out dtValue))
                        {
                            objCustomer.PlannedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["DateOfVisit"].ToString(), out dtValue))
                        {
                            objCustomer.DateOfVisit = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["FollowUp_Date"].ToString(), out dtValue))
                        {
                            objCustomer.FollowUp_Date = dtValue;
                        }
                        objCustomerVisitList.lstCustomerVisit.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in CustomerVisitList: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCustomerVisitList.lstCustomerVisit.ToList());

        }

        public JsonResult CustomerVisitListSearch(string SearchText, string PlanfromDate, string PlanToDate, string chkAll, int? Page, string branch_name)
        {
            CustomerVisitModelsList objCustomerVisitList = new CustomerVisitModelsList();
            objCustomerVisitList.lstCustomerVisit = new List<CustomerVisitModels>();
            ViewBag.Branch_id = objGlobaldata.GetCompanyBranchListbox();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "SELECT CustomerVisitId, Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber, Designation, Address,"
                    + " Product, FollowUp_Date, Outcomeof_Visit, Remarks,City,ProcessedBy,DocUploadPath,Department,Location FROM t_customervisit where Active=1";

                ViewBag.BranchId = objGlobaldata.GetCompanyBranchListbox();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " (CustomerName ='" + SearchText + "' or CustomerName like '" + SearchText + "%')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
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

                    if (PlanfromDate != null && DateTime.TryParse(PlanfromDate, out dtFromdate) && DateTime.TryParse(PlanToDate, out dtToDate))
                    {
                        ViewBag.PlanfromDate = PlanfromDate;
                        ViewBag.PlanToDate = PlanToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and PlannedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd")
                                + "' or FollowUp_Date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and PlannedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd")
                                + "' or FollowUp_Date between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '" + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by CustomerName asc";
                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsCustomer.Tables.Count > 0 && i < dsCustomer.Tables[0].Rows.Count; i++)
                {
                    try
                    {   
                        CustomerVisitModels objCustomer = new CustomerVisitModels
                        {
                            CustomerVisitId = dsCustomer.Tables[0].Rows[i]["CustomerVisitId"].ToString(),
                            Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[i]["Branch"].ToString()),
                            CustomerName = dsCustomer.Tables[0].Rows[i]["CustomerName"].ToString().ToUpper(),
                            ContactPerson = dsCustomer.Tables[0].Rows[i]["ContactPerson"].ToString(),
                            EmailID = dsCustomer.Tables[0].Rows[i]["EmailID"].ToString(),
                            ContactNumber = dsCustomer.Tables[0].Rows[i]["ContactNumber"].ToString(),
                            Designation = dsCustomer.Tables[0].Rows[i]["Designation"].ToString(),
                            Address = dsCustomer.Tables[0].Rows[i]["Address"].ToString(),
                            Product = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[i]["Product"].ToString()),
                            Outcomeof_Visit = dsCustomer.Tables[0].Rows[i]["Outcomeof_Visit"].ToString(),
                            Remarks = dsCustomer.Tables[0].Rows[i]["Remarks"].ToString(),
                            ProcessedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[i]["ProcessedBy"].ToString()),
                            City = dsCustomer.Tables[0].Rows[i]["City"].ToString(),
                            DocUploadPath = dsCustomer.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["PlannedDate"].ToString(), out dtValue))
                        {
                            objCustomer.PlannedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["DateOfVisit"].ToString(), out dtValue))
                        {
                            objCustomer.DateOfVisit = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[i]["FollowUp_Date"].ToString(), out dtValue))
                        {
                            objCustomer.FollowUp_Date = dtValue;
                        }
                        objCustomerVisitList.lstCustomerVisit.Add(objCustomer);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in CustomerVisitListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult CustomerVisitDetails()
        {
            try
            {
                if (Request.QueryString["CustomerVisitId"] != null && Request.QueryString["CustomerVisitId"] != "")
                {
                    string sCustomerVisitId = Request.QueryString["CustomerVisitId"];
                    string sSqlstmt = "SELECT CustomerVisitId, Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber, Designation, Address,"
                        + " Product, FollowUp_Date, Outcomeof_Visit, Remarks, ProcessedBy,City,DocUploadPath,Department,Location FROM t_customervisit where CustomerVisitId='" + sCustomerVisitId + "'";

                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        CustomerVisitModels objCustomer = new CustomerVisitModels
                        {
                            CustomerVisitId = dsCustomer.Tables[0].Rows[0]["CustomerVisitId"].ToString(),
                            Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[0]["Branch"].ToString()),
                            CustomerName = dsCustomer.Tables[0].Rows[0]["CustomerName"].ToString().ToUpper(),
                            ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            EmailID = dsCustomer.Tables[0].Rows[0]["EmailID"].ToString(),
                            ContactNumber = dsCustomer.Tables[0].Rows[0]["ContactNumber"].ToString(),
                            Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                            Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                            Product = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[0]["Product"].ToString()),
                            Outcomeof_Visit = dsCustomer.Tables[0].Rows[0]["Outcomeof_Visit"].ToString(),
                            Remarks = dsCustomer.Tables[0].Rows[0]["Remarks"].ToString(),
                            ProcessedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[0]["ProcessedBy"].ToString()),
                            City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                            DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["PlannedDate"].ToString(), out dtValue))
                        {
                            objCustomer.PlannedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["DateOfVisit"].ToString(), out dtValue))
                        {
                            objCustomer.DateOfVisit = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["FollowUp_Date"].ToString(), out dtValue))
                        {
                            objCustomer.FollowUp_Date = dtValue;
                        }
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                        return View(objCustomer);

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerVisitList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerVisitList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerVisitList");
        }

        [AllowAnonymous]
        public ActionResult CustomerVisitInfo(int id)
        {
            try
            {
                string sSqlstmt = "SELECT CustomerVisitId, Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber, Designation, Address,"
                    + " Product, FollowUp_Date, Outcomeof_Visit, Remarks, ProcessedBy,City,DocUploadPath,Department,Location FROM t_customervisit where CustomerVisitId='" + id + "'";

                DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                {
                    CustomerVisitModels objCustomer = new CustomerVisitModels
                    {
                        CustomerVisitId = dsCustomer.Tables[0].Rows[0]["CustomerVisitId"].ToString(),
                        Branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCustomer.Tables[0].Rows[0]["Branch"].ToString()),
                        CustomerName = dsCustomer.Tables[0].Rows[0]["CustomerName"].ToString().ToUpper(),
                        ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                        EmailID = dsCustomer.Tables[0].Rows[0]["EmailID"].ToString(),
                        ContactNumber = dsCustomer.Tables[0].Rows[0]["ContactNumber"].ToString(),
                        Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                        Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                        Product = objGlobaldata.GetDropdownitemById(dsCustomer.Tables[0].Rows[0]["Product"].ToString()),
                        Outcomeof_Visit = dsCustomer.Tables[0].Rows[0]["Outcomeof_Visit"].ToString(),
                        Remarks = dsCustomer.Tables[0].Rows[0]["Remarks"].ToString(),
                        ProcessedBy = objGlobaldata.GetEmpHrNameById(dsCustomer.Tables[0].Rows[0]["ProcessedBy"].ToString()),
                        City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                        DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                        Department = objGlobaldata.GetMultiDeptNameById(dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsCustomer.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    DateTime dtValue;
                    if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["PlannedDate"].ToString(), out dtValue))
                    {
                        objCustomer.PlannedDate = dtValue;
                    }
                    if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["DateOfVisit"].ToString(), out dtValue))
                    {
                        objCustomer.DateOfVisit = dtValue;
                    }
                    if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["FollowUp_Date"].ToString(), out dtValue))
                    {
                        objCustomer.FollowUp_Date = dtValue;
                    }

                    return View(objCustomer);

                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("CustomerVisitList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerVisitList");
        }

        [AllowAnonymous]
        public ActionResult CustomerVisitEdit()
        {
            try
            {
                if (Request.QueryString["CustomerVisitId"] != null && Request.QueryString["CustomerVisitId"] != "")
                {
                    string sCustomerVisitId = Request.QueryString["CustomerVisitId"];
                    string sSqlstmt = "SELECT CustomerVisitId, Branch, PlannedDate, DateOfVisit, CustomerName, ContactPerson, EmailID, ContactNumber, Designation, Address,"
                        + " Product, FollowUp_Date, Outcomeof_Visit, Remarks, ProcessedBy,City,DocUploadPath,Department,Location FROM t_customervisit where CustomerVisitId='" + sCustomerVisitId + "'";

                    DataSet dsCustomer = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsCustomer.Tables.Count > 0 && dsCustomer.Tables[0].Rows.Count > 0)
                    {
                        CustomerVisitModels objCustomer = new CustomerVisitModels
                        {
                            CustomerVisitId = dsCustomer.Tables[0].Rows[0]["CustomerVisitId"].ToString(),
                            Branch = (dsCustomer.Tables[0].Rows[0]["Branch"].ToString()),
                            CustomerName = dsCustomer.Tables[0].Rows[0]["CustomerName"].ToString().ToUpper(),
                            ContactPerson = dsCustomer.Tables[0].Rows[0]["ContactPerson"].ToString(),
                            EmailID = dsCustomer.Tables[0].Rows[0]["EmailID"].ToString(),
                            ContactNumber = dsCustomer.Tables[0].Rows[0]["ContactNumber"].ToString(),
                            Designation = dsCustomer.Tables[0].Rows[0]["Designation"].ToString(),
                            Address = dsCustomer.Tables[0].Rows[0]["Address"].ToString(),
                            Product = dsCustomer.Tables[0].Rows[0]["Product"].ToString(),
                            Outcomeof_Visit = dsCustomer.Tables[0].Rows[0]["Outcomeof_Visit"].ToString(),
                            Remarks = dsCustomer.Tables[0].Rows[0]["Remarks"].ToString(),
                            ProcessedBy = dsCustomer.Tables[0].Rows[0]["ProcessedBy"].ToString(),
                            City = dsCustomer.Tables[0].Rows[0]["City"].ToString(),
                            DocUploadPath = dsCustomer.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            Department = (dsCustomer.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsCustomer.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtValue;
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["PlannedDate"].ToString(), out dtValue))
                        {
                            objCustomer.PlannedDate = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["DateOfVisit"].ToString(), out dtValue))
                        {
                            objCustomer.DateOfVisit = dtValue;
                        }
                        if (DateTime.TryParse(dsCustomer.Tables[0].Rows[0]["FollowUp_Date"].ToString(), out dtValue))
                        {
                            objCustomer.FollowUp_Date = dtValue;
                        }

                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsCustomer.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCustomer.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Product_id = objGlobaldata.GetDropdownList("Product desc");

                        return View(objCustomer);

                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("CustomerVisitList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Id cannot be null";
                    return RedirectToAction("CustomerVisitList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("CustomerVisitList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerVisitEdit(CustomerVisitModels objCustomerVisitModels, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objCustomerVisitModels.Branch = form["Branch"];
                objCustomerVisitModels.Department = form["Department"];
                objCustomerVisitModels.Location = form["Location"];

                DateTime dateValue;

                if (DateTime.TryParse(form["PlannedDate"], out dateValue) == true)
                {
                    objCustomerVisitModels.PlannedDate = dateValue;
                }

                if (DateTime.TryParse(form["DateOfVisit"], out dateValue) == true)
                {
                    objCustomerVisitModels.DateOfVisit = dateValue;
                }
                if (DateTime.TryParse(form["FollowUp_Date"], out dateValue) == true)
                {
                    objCustomerVisitModels.FollowUp_Date = dateValue;
                }

                objCustomerVisitModels.Branch = form["Branch"];

                objCustomerVisitModels.ProcessedBy = objGlobaldata.GetCurrentUserSession().empid;

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(file.FileName));
                        string sFilename = "CustomerVisit" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        file.SaveAs(sFilepath + "/" + sFilename);
                        objCustomerVisitModels.DocUploadPath = "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddCustomerVisit-upload: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objCustomerVisitModels.FunUpdateCustomerVisit(objCustomerVisitModels))
                {
                    TempData["Successdata"] = "Updated Visit details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustomerVisitEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustomerVisitList");
        }
    }
}
