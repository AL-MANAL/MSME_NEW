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
    //[CusAuthentication("Admin", "CEO")]
    public class CompanyController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public CompanyController()
        {
            ViewBag.Menutype = "Settings";
        }

        //
        // GET: /Company/
        
        public ActionResult Index()
        {
            return View();
        }

        ////
        //// GET: /Company/AddCompany
        //[AllowAnonymous]
        //public ActionResult AddCompany()
        //{
        //    ViewBag.SubMenutype = "Company";

        //    string sSqlstmt = "select CompanyName from t_CompanyInfo";// where CompanyID=" + objUserInfo.empid;
        //    DataSet dsCompany = objGlobaldata.Getdetails(sSqlstmt);

        //    try
        //    {
        //        if (dsCompany.Tables.Count > 0 && dsCompany.Tables[0].Rows.Count > 0)
        //        {
        //            ViewBag.CompanyName = dsCompany.Tables[0].Rows[0]["CompanyName"].ToString();
        //            ViewBag.SubMenutype = "AddCompany";
        //            ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
        //        }
        //        TempData["alertdata"] = "Sorry Company is not registered, please contact the Software Administrator.";
        //        return RedirectToAction("Login", "Account");
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddCompany: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();

        //}


        ////
        //// POST: /Company/AddCompany
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddCompany(CompanyModels objCustomerModels, FormCollection form, HttpPostedFileBase CompanyLogo)
        //{
        //    try
        //    {
        //        ViewBag.SubMenutype = "Company";
        //        objCustomerModels.PhoneNumber = form["PhoneNumber"];
        //        objCustomerModels.FaxNumber = form["FaxNumber"];
        //    objCustomerModels.PostalCode = form["PostalCode"];

        //        objCustomerModels.Audit_Criteria = form["Audit_Criteria"];
        //        Dictionary<string, string> dicBranch = new Dictionary<string, string>();

        //        short itemcnt = 0;
        //        if (form["itemcnt"] != null && Int16.TryParse(form["itemcnt"], out itemcnt))
        //        {
        //            for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
        //            {
        //                if (form["Name" + i] != null)
        //                {
        //                    dicBranch.Add(form["Name" + i], form["Address" + i]);
        //                }
        //            }
        //            if (dicBranch.Count == 0)
        //            {
        //                dicBranch.Add("Main Office", objCustomerModels.Address.Trim());
        //            }
        //        }

        //        if (CompanyLogo != null && CompanyLogo.ContentLength > 0)
        //        {
        //            try
        //            {
        //                //file.FileName=DateTime.Now.ToString().Replace('/',' ');
        //                string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(CompanyLogo.FileName));
        //                string sFilename = objCustomerModels.CompanyName + Path.GetExtension(CompanyLogo.FileName), sFilepath = Path.GetDirectoryName(spath);
        //                CompanyLogo.SaveAs(sFilepath + "/" + sFilename);
        //                objCustomerModels.logo = "~/DataUpload/ProfilePic/" + sFilename;
        //                ViewBag.Message = "File uploaded successfully";
        //            }
        //            catch (Exception ex)
        //            {
        //                ViewBag.Message = "ERROR:" + ex.Message.ToString();
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.Message = "You have not specified a file.";
        //        }

        //        if (objCustomerModels.FunRegisterCompany(objCustomerModels, dicBranch))
        //        {
        //            TempData["Successdata"] = "Company details saved successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in AddCompany: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return RedirectToAction("CompanyDetails", "Company", new { CompanyID = objCustomerModels.CompanyID });
        //}

        //
        // GET: /Company/CompanyDetails


        // POST: /ProductionLog/GetCustomerType
          
        [HttpPost]
        public JsonResult GetCurrencyCode()
        {
            return Json(objGlobaldata.GetCurrencyCode());
        }

          
        [AllowAnonymous]
        public ActionResult CompanyDetails()
        {
            ViewBag.SubMenutype = "Company";

            //objGlobaldata.CreateUserSession();
            CompanyModels objCompany = new CompanyModels();

            try
            {
                if (HttpContext.Session["UserCredentials"] != null)
                {
                    UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
                }

                string sSqlstmt = "select CompanyID, CompanyName, Address,Scope, City, State, PostalCode, Country, PhoneNumber, FaxNumber, Audit_location,Exclusion,"
                            + " Audit_Criteria, logo,License, BranchAddress,expiry_date,Email_Id from t_CompanyInfo";// where CompanyID=" + objUserInfo.empid;
                DataSet dsCompany = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCompany.Tables[0].Rows.Count > 0)
                {
                    objCompany = new CompanyModels
                    {
                        CompanyID = dsCompany.Tables[0].Rows[0]["CompanyID"].ToString(),
                        CompanyName = dsCompany.Tables[0].Rows[0]["CompanyName"].ToString(),
                        Address = dsCompany.Tables[0].Rows[0]["Address"].ToString(),
                        Scope = dsCompany.Tables[0].Rows[0]["Scope"].ToString(),
                        Exclusion = dsCompany.Tables[0].Rows[0]["Exclusion"].ToString(),
                        City = dsCompany.Tables[0].Rows[0]["City"].ToString(),
                        State = dsCompany.Tables[0].Rows[0]["State"].ToString(),
                        PostalCode = dsCompany.Tables[0].Rows[0]["PostalCode"].ToString(),
                        Country = objGlobaldata.GetCountryNameById(dsCompany.Tables[0].Rows[0]["Country"].ToString()),
                        PhoneNumber = dsCompany.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                        Audit_location = dsCompany.Tables[0].Rows[0]["Audit_location"].ToString(),
                        FaxNumber = dsCompany.Tables[0].Rows[0]["FaxNumber"].ToString(),
                        Audit_Criteria = objGlobaldata.GetIsoStdDescriptionById(dsCompany.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                        logo = dsCompany.Tables[0].Rows[0]["logo"].ToString(),
                        BranchAddress = dsCompany.Tables[0].Rows[0]["BranchAddress"].ToString(),
                        License = dsCompany.Tables[0].Rows[0]["License"].ToString(),
                        Email_Id = dsCompany.Tables[0].Rows[0]["Email_Id"].ToString()
                    };
                    DateTime dtDocDate;
                    if (dsCompany.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                       && DateTime.TryParse(dsCompany.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                    {
                        objCompany.expiry_date = dtDocDate;
                    }
                    sSqlstmt = "select * from t_Company_Branch where CompId=" + objCompany.CompanyID;
                    ViewBag.Branches = objGlobaldata.GetCompanyBranch(objCompany.CompanyID);

                    return View(objCompany);
                }

                TempData["alertdata"] = "Sorry Company is not registered, please contact the Software Administrator.";
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Company Details: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Company/CompanyEdit
          
        [AllowAnonymous]
        public ActionResult CompanyEdit()
        {
            ViewBag.SubMenutype = "Company";
          

            CompanyModels objCompany = new CompanyModels();
            try
            {
                string sCustomerID = Request.QueryString["CompanyID"];
                string sSqlstmt = "select CompanyID, CompanyName, Address,Scope, City, State, PostalCode, Country, PhoneNumber, FaxNumber, Audit_location,Exclusion,"
                            + " Audit_Criteria, logo, BranchAddress,License,expiry_date,Email_Id from t_CompanyInfo where CompanyID='" + sCustomerID + "'";
                DataSet dsCompany = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCompany.Tables.Count > 0 && dsCompany.Tables[0].Rows.Count > 0)
                {

                    objCompany = new CompanyModels
                    {
                        CompanyID = dsCompany.Tables[0].Rows[0]["CompanyID"].ToString(),
                        CompanyName = dsCompany.Tables[0].Rows[0]["CompanyName"].ToString(),
                        Address = dsCompany.Tables[0].Rows[0]["Address"].ToString(),
                        Scope = dsCompany.Tables[0].Rows[0]["Scope"].ToString(),
                        Exclusion = dsCompany.Tables[0].Rows[0]["Exclusion"].ToString(),
                        City = dsCompany.Tables[0].Rows[0]["City"].ToString(),
                        State = dsCompany.Tables[0].Rows[0]["State"].ToString(),
                        PostalCode = dsCompany.Tables[0].Rows[0]["PostalCode"].ToString(),
                        Country = dsCompany.Tables[0].Rows[0]["Country"].ToString(),
                        PhoneNumber = dsCompany.Tables[0].Rows[0]["PhoneNumber"].ToString(),
                        Audit_location = dsCompany.Tables[0].Rows[0]["Audit_location"].ToString(),
                        FaxNumber = dsCompany.Tables[0].Rows[0]["FaxNumber"].ToString(),
                        Audit_Criteria =/* objGlobaldata.GetIsoStdDescriptionById*/(dsCompany.Tables[0].Rows[0]["Audit_Criteria"].ToString()),
                        logo = dsCompany.Tables[0].Rows[0]["logo"].ToString(),
                        BranchAddress = dsCompany.Tables[0].Rows[0]["BranchAddress"].ToString(),
                        License = dsCompany.Tables[0].Rows[0]["License"].ToString(),
                        Email_Id = dsCompany.Tables[0].Rows[0]["Email_Id"].ToString()
                    };
                    DateTime dtDocDate;
                    if (dsCompany.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                       && DateTime.TryParse(dsCompany.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                    {
                        objCompany.expiry_date = dtDocDate;
                    }
                    ViewBag.IsoStdList = objGlobaldata.GetIsoAndDescList();
                    ViewBag.Branches = objGlobaldata.GetCompanyBranch(objCompany.CompanyID);
                    ViewBag.Currency_Code = objGlobaldata.GetCurrencyCode();
                    ViewBag.Company = objGlobaldata.GetCompanyBranchListbox();
                    ViewBag.Country = objGlobaldata.GetCountryListbox();
                    return View(objCompany);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CompanyEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            TempData["alertdata"] = "Sorry Company is not registered, please contact the Software Administrator.";
            return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Company/CompanyEdit
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyEdit(CompanyModels objCustomerModels, FormCollection form, HttpPostedFileBase CompanyLogo, IEnumerable<HttpPostedFileBase> License)
        {
            ViewBag.SubMenutype = "Company";
                try
                {
                    string QCDelete = Request.Form["QCDocsValselectall"];
                    HttpPostedFileBase files = Request.Files[1];
                    objCustomerModels.Audit_Criteria = form["Audit_Criteria"];
                    objCustomerModels.CompanyID = form["CompanyID"];
                //objCustomerModels.CompanyName = "AL MANAL";
                objCustomerModels.CompanyName = form["CompanyName"];
                    objCustomerModels.PhoneNumber = form["PhoneNumber"];
                    objCustomerModels.FaxNumber = form["FaxNumber"];
                    objCustomerModels.PostalCode = form["PostalCode"];
                    objCustomerModels.Scope = form["Scope"];
                    objCustomerModels.Address = form["Address"];
                    objCustomerModels.Country = form["Country"]; 

                DateTime dateValue;
                    if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                    {
                        objCustomerModels.expiry_date = dateValue;
                    }

                    if (CompanyLogo != null && CompanyLogo.ContentLength > 0)
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/ProfilePic"), Path.GetFileName(CompanyLogo.FileName));
                        string sFilename = "CompanyLogo" + Path.GetExtension(CompanyLogo.FileName), sFilepath = Path.GetDirectoryName(spath);
                        CompanyLogo.SaveAs(sFilepath + "/" + sFilename);
                        objCustomerModels.logo = "~/DataUpload/ProfilePic/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (License != null && files.ContentLength > 0)
                    {
                        objCustomerModels.License = "";
                        foreach (var file in License)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/License"), Path.GetFileName(file.FileName));
                                string sFilename = "License" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objCustomerModels.License = objCustomerModels.License + "," + "~/DataUpload/MgmtDocs/License/" + sFilename;
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in License-uploaddocument: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                        }
                        objCustomerModels.License = objCustomerModels.License.Trim(',');
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }
                    if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                    {
                        objCustomerModels.License = objCustomerModels.License + "," + form["QCDocsVal"];
                        objCustomerModels.License = objCustomerModels.License.Trim(',');
                    }
                    else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                    {
                        objCustomerModels.License = null;
                    }
                    else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                    {
                        objCustomerModels.License = null;
                    }
                    if (objCustomerModels.FunUpdateCompany(objCustomerModels))
                    {
                        TempData["Successdata"] = "Company details updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                catch (Exception ex)
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    objGlobaldata.AddFunctionalLog("Logo error: " + ex.ToString() + ", ");
                }
          
            return RedirectToAction("CompanyEdit", "Company", new { CompanyID = objCustomerModels.CompanyID });
        }
     

        //POST: /Company/MeetingItemEdit

        public ActionResult BranchEdit(string Parent,string Name,string Code, string Address,  string sItemNo, string scope)
        {
            try
            {
                CompanyModels objCustomerModels = new CompanyModels();
                objCustomerModels.FunUpdateBranch(sItemNo,Parent, Name,Code, Address, scope);
                return Json("Update Success");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in BranchEdit: " + ex.ToString());
                return Json("Update Failed: " + ex.ToString());
            }
        }

        //POST: /Company/MeetingItemEdit
          
        //public ActionResult AddNewBranch(CompanyModels objCustomerModels, FormCollection form, string BranchName, string BranchAddress, string Curr_code, string scope, int CompId = 1)
        //{
        //    try
        //    {
        //        Dictionary<string, string> dicBranch = new Dictionary<string, string>();
        //        dicBranch.Add(BranchName, BranchAddress + "$" + Curr_code +"$" + scope);
        //        //CompanyModels objCustomerModels = new CompanyModels();
        //        if (objCustomerModels.FunAddBranch(dicBranch, CompId))
        //        {
        //            TempData["Successdata"] = "Added Branch details successfully";
        //        }
        //        else
        //        {
        //            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        }

        //        return RedirectToAction("CompanyDetails");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        objGlobaldata.AddFunctionalLog("Exception in AddNewBranch: " + ex.ToString());                
        //    }

        //    return RedirectToAction("CompanyDetails");
        //}
               

        [AllowAnonymous]
        public JsonResult AddNewBranch(string parent_level, string BranchName, string BranchCode, string BranchAddress, string scope, int CompId = 1)
        {
            CompanyModels objCustomerModels = new CompanyModels(); ;
            Dictionary<string, string> dicBranch = new Dictionary<string, string>();
            dicBranch.Add(BranchName, BranchAddress + "$" +  scope + "$" + parent_level + "$" + BranchCode);
            //CompanyModels objCustomerModels = new CompanyModels();
            if (objCustomerModels.FunAddBranch(dicBranch, CompId))
            {
                TempData["Successdata"] = "Added Branch details successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
                    

        //POST: /Company/MeetingItemEdit

        public JsonResult BranchDelete(string sItemNo)
        {
            try
            {
                CompanyModels objCustomerModels = new CompanyModels();
              
                if (objCustomerModels.FunDeleteBranch(sItemNo))
                {
                    TempData["Successdata"] = "Deleted Branch details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                return Json("Delete Success");
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in BranchDelete: " + ex.ToString());
                return Json("Delete Failed: " + ex.ToString());
            }
        }

        //
        // GET: /Company/AddDepartment
          
        [AllowAnonymous]
        public ActionResult AddDepartment()
        {
            
            return InitializeDepartment();
        }

          
        public ActionResult InitializeDepartment()
        {
            try
            {
                MultiSelectList lstDept = objGlobaldata.GetDepartmentListbox();
                ViewBag.dsDept = lstDept;
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                DataSet dsBranch = objGlobaldata.Getdetails("select DeptId,DeptName,branch from t_departments");
                ViewBag.dsBranch = dsBranch;
                ViewBag.SubMenutype = "Department";
            }
            catch (Exception ex)
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                objGlobaldata.AddFunctionalLog("Exception in AddDepartment: " + ex.ToString());
               
            }
            return View();
        }

        //
        // POST: /Company/AddDepartment
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDepartment(DepartmentModels objDepartmentModels,FormCollection form)
        {
            ViewBag.SubMenutype = "Department";
            objDepartmentModels.branch=form["branch"];
            if (objDepartmentModels.FunAddDept(objDepartmentModels))
            {
                TempData["Successdata"] = "Department details updated Successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return InitializeDepartment();
        }

        //
        // GET: /Employee/EmployeeDelete
          
        [AllowAnonymous]
        public JsonResult DepartmentDelete(string Id)
        {
            ViewBag.SubMenutype = "Department";
            DepartmentModels objDepartmentModels = new DepartmentModels();

            if (objDepartmentModels.FunDeleteDept(Id))
            {
                TempData["Successdata"] = "Department details deleted Successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
               

        [AllowAnonymous]
        public JsonResult DepartmentEdit(string Id, string dept, FormCollection form)
        {
            ViewBag.SubMenutype = "Department";
            DepartmentModels objDepartmentModels = new DepartmentModels();
            string branch = form["branch[]"];
           
            if (objDepartmentModels.FunEditDept(Id, dept, branch))
            {
                TempData["Successdata"] = "Department details Edited Successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        // To add ISOSTD

        //[AllowAnonymous]
        //public ActionResult AddISOStd()
        //{
        //    try
        //    {
        //        ViewBag.SubMenutype = "Settings";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //        objGlobaldata.AddFunctionalLog("Exception in AddISOStd: " + ex.ToString());

        //    }
        //    return View();
        //}

        [AllowAnonymous]
        public ActionResult AddISOStd(CompanyModels objModel, FormCollection form)
        {
            ViewBag.SubMenutype = "Settings";
            ViewBag.IsoStd = objGlobaldata.GetAllIsoStdListbox();
            try
            { 
              if (objModel.FunAddISOStd(objModel))
              {
                TempData["Successdata"] = "ISOStd added Successfully";
              }
              else
              {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
              }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddHoliday: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ISOStdList");
         }

        [AllowAnonymous]
        public ActionResult ISOStdList( int? page)
        {
            CompanyModelsList obj = new CompanyModelsList();
            obj.CompanyMList = new List<CompanyModels>();
            CompanyModels objstd = new CompanyModels();
            try
            {
                string sSqlstmt = "select StdId,IsoName,Descriptions from t_isostandards where Active=1 order by StdId asc";
                DataSet dsIsoList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsIsoList.Tables.Count > 0 && dsIsoList.Tables[0].Rows.Count > 0)
                {                 
                                       
                    for (int i = 0; i < dsIsoList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                          objstd = new CompanyModels
                            {
                              StdId = (dsIsoList.Tables[0].Rows[i]["StdId"].ToString()),
                              IsoName = (dsIsoList.Tables[0].Rows[i]["IsoName"].ToString()),
                              Descriptions = dsIsoList.Tables[0].Rows[i]["Descriptions"].ToString(),
                            };
                            
                            obj.CompanyMList.Add(objstd);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ISOStdList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ISOStdList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(obj.CompanyMList.ToList().ToPagedList(page ?? 1, 10));

        }

        public ActionResult ISOStdEdit()
        {
            CompanyModels obj = new CompanyModels();
            try
            {

                if (Request.QueryString["StdId"] != null && Request.QueryString["StdId"] != "")
                {
                    string sStdId = Request.QueryString["StdId"];

                    string sSqlstmt = "select StdId,IsoName,Descriptions from t_isostandards where StdId='" + sStdId + "'";
                    DataSet dsStdList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsStdList.Tables.Count > 0 && dsStdList.Tables[0].Rows.Count > 0)
                    {
                        obj = new CompanyModels
                        {
                            StdId = (dsStdList.Tables[0].Rows[0]["StdId"].ToString()),
                            IsoName = (dsStdList.Tables[0].Rows[0]["IsoName"].ToString()),
                            Descriptions = dsStdList.Tables[0].Rows[0]["Descriptions"].ToString(),
                        };
                    }
                    else
                    {

                        TempData["alertdata"] = "StdId cannot be Null or empty";
                        return RedirectToAction("ISOStdList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "StdId cannot be Null or empty";
                    return RedirectToAction("ISOStdList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ISOStdEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ISOStdList");
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ISOStdEdit(CompanyModels obj, FormCollection form)
        {
            try
            {               
                if (obj.FunUpdateISOStd(obj))
                {
                    TempData["Successdata"] = "ISOStd details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ISOStdEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ISOStdList");
        }

        [AllowAnonymous]
        public JsonResult ISOStdDelete(FormCollection form)
        {
            try
            {

                  if (form["StdId"] != null && form["StdId"] != "")
                    {

                        CompanyModels objmdl = new CompanyModels();
                        string sStdId = form["StdId"];

                        if (objmdl.FunDeleteISOStd(sStdId))
                        {
                            TempData["Successdata"] = "ISOStd deleted successfully";
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
                        TempData["alertdata"] = "StdId cannot be Null or empty";
                        return Json("Failed");
                    }

               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ISOStdDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }


}
