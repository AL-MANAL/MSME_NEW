using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.Net;
using System.IO;
using PagedList;
using PagedList.Mvc;
using Microsoft.Reporting.WebForms;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class LegalRegisterController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public LegalRegisterController()
        {
            ViewBag.Menutype = "LegalRegister";
            ViewBag.SubMenutype = "Compliance";
        }

        //
        // GET: /LegalRegister/

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddCompliance()
        {
            LegalRegisterModel ObjMdl = new LegalRegisterModel();
            try
            {
                ViewBag.SubMenutype = "compliance";
                ObjMdl.branch = objGlobaldata.GetCurrentUserSession().division;
                ObjMdl.deptid = objGlobaldata.GetCurrentUserSession().DeptID;
                ObjMdl.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Compliance = objGlobaldata.GetDropdownList("Compliance");
                ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(ObjMdl.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(ObjMdl.branch);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompliance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(ObjMdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCompliance(LegalRegisterModel objComp, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objComp.Isostd = form["Isostd"];
                objComp.branch = form["branch"];
                objComp.deptid = form["deptid"];
                objComp.Location = form["Location"];
                if (objComp.branch == null || objComp.branch == "")
                {
                    objComp.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["Eve_Date"], out dateValue) == true)
                {
                    objComp.Eve_Date = dateValue;
                }
                if (DateTime.TryParse(form["nexteval_date"], out dateValue) == true)
                {
                    objComp.nexteval_date = dateValue;
                }
                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objComp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                            string sFilename = "Compliance" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.upload = objComp.upload + "," + "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddCompliance-upload: " + ex.ToString());

                        }
                    }
                    objComp.upload = objComp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objComp.FunAddCompliance(objComp))
                {
                    TempData["Successdata"] = "Added Compliance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCompliance: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult ComplianceList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Compliance";

            LegalRegisterModelsList objCom = new LegalRegisterModelsList();
            objCom.LegalRegisterMList = new List<LegalRegisterModel>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url," +
                    "Eve_date,Revision_Date,Revision_No,branch,Location from t_compliance_obligation where Active=1";
            
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by lawNo";
                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {                   

                    for (int i = 0; i < dsComplList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LegalRegisterModel objLegalModels = new LegalRegisterModel
                            {
                                id_law = (dsComplList.Tables[0].Rows[i]["id_law"].ToString()),
                                lawNo = dsComplList.Tables[0].Rows[i]["lawNo"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[i]["Isostd"].ToString()),
                                lawTitle = dsComplList.Tables[0].Rows[i]["lawTitle"].ToString(),
                                deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[i]["deptid"].ToString()),
                                compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["compliance"].ToString()),
                                url = dsComplList.Tables[0].Rows[i]["url"].ToString(),
                                Revision_No = dsComplList.Tables[0].Rows[i]["Revision_No"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsComplList.Tables[0].Rows[i]["Eve_Date"].ToString() != ""
                               && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Eve_Date"].ToString(), out dtDocDate))
                            {
                                objLegalModels.Eve_Date = dtDocDate;
                            }

                            if (dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString() != ""
                               && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString(), out dtDocDate))
                            {
                                objLegalModels.Revision_Date = dtDocDate;
                            }

                            objCom.LegalRegisterMList.Add(objLegalModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ComplianceList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCom.LegalRegisterMList.ToList());
        }

        [AllowAnonymous]
        public JsonResult ComplianceListSearch(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Compliance";

            LegalRegisterModelsList objCom = new LegalRegisterModelsList();
            objCom.LegalRegisterMList = new List<LegalRegisterModel>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_date,Revision_Date,Revision_No,branch,Location from t_compliance_obligation where Active=1 ";
                
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by lawNo";
                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsComplList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LegalRegisterModel objLegalModels = new LegalRegisterModel
                            {
                                id_law = (dsComplList.Tables[0].Rows[i]["id_law"].ToString()),
                                lawNo = dsComplList.Tables[0].Rows[i]["lawNo"].ToString(),
                                Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[i]["Isostd"].ToString()),
                                lawTitle = dsComplList.Tables[0].Rows[i]["lawTitle"].ToString(),
                                deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[i]["deptid"].ToString()),
                                compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["compliance"].ToString()),
                                url = dsComplList.Tables[0].Rows[i]["url"].ToString(),
                                Revision_No = dsComplList.Tables[0].Rows[i]["Revision_No"].ToString(),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[i]["branch"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[i]["Location"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsComplList.Tables[0].Rows[i]["Eve_Date"].ToString() != ""
                               && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Eve_Date"].ToString(), out dtDocDate))
                            {
                                objLegalModels.Eve_Date = dtDocDate;
                            }

                            if (dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString() != ""
                               && DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Revision_Date"].ToString(), out dtDocDate))
                            {
                                objLegalModels.Revision_Date = dtDocDate;
                            }

                            objCom.LegalRegisterMList.Add(objLegalModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in ComplianceListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult ComplianceEdit()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            UserCredentials objUser = new UserCredentials();

            objUser = objGlobaldata.GetCurrentUserSession();

            //ViewBag.Role = objGlobaldata.GetRoleName(objUser.role);
            // ViewBag.CurrentEmpName = objUser.firstname;

            try
            {
                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    string sid_law = Request.QueryString["id_law"];
                    string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload," +
                        "url,Eve_Date,Revision_date,Revision_No,requirement,description,nexteval_date,branch,Location from t_compliance_obligation"
                    + " where id_law='" + sid_law + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {
                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            Isostd = (dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = (dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                            compliance = (dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                            url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                            Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                            requirement = dsComplList.Tables[0].Rows[0]["requirement"].ToString(),
                            description = dsComplList.Tables[0].Rows[0]["description"].ToString(),
                            branch = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location =/* objGlobaldata.GetDivisionLocationById*/(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsComplList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsComplList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Compliance = objGlobaldata.GetDropdownList("Compliance");
                        ViewBag.ISOStds = objGlobaldata.GetAllIsoStdListbox();                       
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_Date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString() != ""
                         && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString(), out dtDocDate))
                        {
                            objComp.nexteval_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Revision_Date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ComplianceEdit(LegalRegisterModel objComp, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objComp.Isostd = form["Isostd"];
                objComp.deptid = form["deptid"];
                objComp.Location = form["Location"];
                objComp.branch = form["branch"];
                if (objComp.branch == null || objComp.branch == "")
                {
                    objComp.branch = objGlobaldata.GetCurrentUserSession().division;
                }
                DateTime dateValue;

                if (DateTime.TryParse(form["Eve_Date"], out dateValue) == true)
                {
                    objComp.Eve_Date = dateValue;
                }

                if (DateTime.TryParse(form["Revision_Date"], out dateValue) == true)
                {
                    objComp.Revision_Date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (upload != null && files.ContentLength > 0)
                {
                    objComp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                            string sFilename = "Enquiry" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.upload = objComp.upload + "," + "~/DataUpload/MgmtDocs/CRM/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in EnquiryEdit-upload: " + ex.ToString());

                        }
                    }
                    objComp.upload = objComp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objComp.upload = objComp.upload + "," + form["QCDocsVal"];
                    objComp.upload = objComp.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objComp.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objComp.upload = null;
                }
                if (objComp.FunUpdateCompliance(objComp))
                {
                    TempData["Successdata"] = "Updated Compliance details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("ComplianceList");
        }

        [AllowAnonymous]
        public ActionResult ComplianceDetails()
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {

                if (Request.QueryString["id_law"] != null && Request.QueryString["id_law"] != "")
                {
                    string sid_law = Request.QueryString["id_law"];
                    string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_Date," +
                        "Revision_Date,Revision_No,requirement,description,nexteval_date,branch,Location from t_compliance_obligation"
                    + " where id_law='" + sid_law + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {

                        objComp = new LegalRegisterModel
                        {
                            id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                            lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                            Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                            lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                            compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                            upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                            url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                            Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                            requirement = dsComplList.Tables[0].Rows[0]["requirement"].ToString(),
                            description = dsComplList.Tables[0].Rows[0]["description"].ToString(),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                            Location =objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                        };

                        DateTime dtDocDate;
                        if (dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                           && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Eve_Date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["nexteval_date"].ToString(), out dtDocDate))
                        {
                            objComp.nexteval_date = dtDocDate;
                        }
                        if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                          && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                        {
                            objComp.Revision_Date = dtDocDate;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("ComplianceList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public ActionResult ComplianceInfo(int id)
        {
            ViewBag.SubMenutype = "Compliance";
            LegalRegisterModel objComp = new LegalRegisterModel();

            try
            {
                string sSqlstmt = "select id_law,lawNo,Isostd,lawTitle,deptid,compliance,upload,url,Eve_Date," +
                    "Revision_Date,Revision_No,branch,Location from t_compliance_obligation"
                + " where id_law='" + id + "'";

                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);
                
                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {

                    objComp = new LegalRegisterModel
                    {
                        id_law = (dsComplList.Tables[0].Rows[0]["id_law"].ToString()),
                        lawNo = dsComplList.Tables[0].Rows[0]["lawNo"].ToString(),
                        Isostd = objGlobaldata.GetIsoStdDescriptionById(dsComplList.Tables[0].Rows[0]["Isostd"].ToString()),
                        lawTitle = dsComplList.Tables[0].Rows[0]["lawTitle"].ToString(),
                        deptid = objGlobaldata.GetMultiDeptNameById(dsComplList.Tables[0].Rows[0]["deptid"].ToString()),
                        compliance = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["compliance"].ToString()),
                        upload = (dsComplList.Tables[0].Rows[0]["upload"].ToString()),
                        url = dsComplList.Tables[0].Rows[0]["url"].ToString(),
                        Revision_No = dsComplList.Tables[0].Rows[0]["Revision_No"].ToString(),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsComplList.Tables[0].Rows[0]["branch"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                    };
                    DateTime dtDocDate;
                    if (dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString() != ""
                       && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Eve_Date"].ToString(), out dtDocDate))
                    {
                        objComp.Eve_Date = dtDocDate;
                    }
                    if (dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString() != ""
                       && DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Revision_Date"].ToString(), out dtDocDate))
                    {
                        objComp.Revision_Date = dtDocDate;
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("ComplianceList");
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("ComplianceList");
            }
            return View(objComp);
        }

        [AllowAnonymous]
        public JsonResult ComplianceDocDelete(FormCollection form)
        {
            try
            {
                  if (form["id_law"] != null && form["id_law"] != "")
                    {

                        LegalRegisterModel Doc = new LegalRegisterModel();
                        string sid_law = form["id_law"];
                    
                        if (Doc.FunDeleteComplianceDoc(sid_law))
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
                        TempData["alertdata"] = "Compliance Id cannot be Null or empty";
                        return Json("Failed");
                    }
               

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ComplianceDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        // GET: /LegalRegister/AddLegalRegister

        [AllowAnonymous]
        public ActionResult AddLegalRegister()
        {
            return InitilizeAddLegalRegister();
        }

        // GET: /LegalRegister/AddLegalRegister

        private ActionResult InitilizeAddLegalRegister()
        {
            try
            {

                ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                ViewBag.deptlist = objGlobaldata.GetDepartmentList();
                ViewBag.Status_Obj_Eval = objGlobaldata.GetConstantValue("Status_Obj_Eval");
                ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.Empposition = objGlobaldata.GetEmpDesignationByHrEmpId("");
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                ViewBag.ApproverList = objGlobaldata.GetApprover();
                ViewBag.ReviewerList = objGlobaldata.GetReviewer();//role=1+active+internal
                ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in InitilizeAddLegalRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        //Returns only Department Head list List
        // GET: /LegalRegister/FunAttendeesList

        public ActionResult FunGetDeptEmpList(string DeptId)
        {
            MultiSelectList lstEmp = objGlobaldata.GetHrEmpListByDept(DeptId);
            return Json(lstEmp);
        }

        //Returns only Objective reference List
        // GET: /LegalRegister/GetLegalRegisterRefList

        public ActionResult GetLegalRegisterRefList(string DeptId)
        {
            LegalRegisterModel objLegalRegister = new LegalRegisterModel();
            List<string> lstObjectivesList = objLegalRegister.GetLegalRegisterRef(DeptId);
            return Json(lstObjectivesList);
        }

        [AllowAnonymous]
        public JsonResult LegalRegisterDocDelete(FormCollection form)
        {
            try
            {
             
                    if (form["LegalRequirement_Id"] != null && form["LegalRequirement_Id"] != "")
                    {
                        LegalRegisterModel Doc = new LegalRegisterModel();
                        string sLegalRequirement_Id = form["LegalRequirement_Id"];


                        if (Doc.FunDeleteLegalRequirementsDoc(sLegalRequirement_Id))
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
                        TempData["alertdata"] = "Legal Register Id cannot be Null or empty";
                        return Json("Failed");
                    }
                

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [HttpPost]
        public JsonResult doesObjRefExist(string lawNo)
        {
            LegalRegisterModel objLegalRegister = new LegalRegisterModel();
            var Objective = objLegalRegister.checkObjRefExists(lawNo);

            return Json(Objective);
        }

        //Returns only Department Head list List
        // GET: /LegalRegister/GetLegalRequirementsDetails

        public ActionResult FunGetLegalRequirementsDetails(string LegalRequirement_Id)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();

                DataSet dsObjectives = objLegalRegister.GetLegalRequirementsDetails(LegalRequirement_Id);

                if (dsObjectives.Tables.Count > 0 && dsObjectives.Tables[0].Rows.Count > 0)
                {
                    return Json(new
                    {
                        lawNo = dsObjectives.Tables[0].Rows[0]["lawNo"].ToString(),
                        lawTitle = dsObjectives.Tables[0].Rows[0]["lawTitle"].ToString(),
                        origin_of_requirement = dsObjectives.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                        document_storage_location = (dsObjectives.Tables[0].Rows[0]["document_storage_location"].ToString()),
                        frequency_of_evaluation = (dsObjectives.Tables[0].Rows[0]["frequency_of_evaluation"].ToString()),
                        activeStatus = (dsObjectives.Tables[0].Rows[0]["activeStatus"].ToString()),
                        updatedOn = objGlobaldata.GetCurrentUserSession().ToString(),
                        updatedByName = dsObjectives.Tables[0].Rows[0]["updatedByName"].ToString(),
                        reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectives.Tables[0].Rows[0]["reviewedBy"].ToString()),
                        approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectives.Tables[0].Rows[0]["approvedBy"].ToString()),
                        reviewstatus = (dsObjectives.Tables[0].Rows[0]["reviewstatus"].ToString()),
                        approvestatus = (dsObjectives.Tables[0].Rows[0]["approvestatus"].ToString())
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetLegalRequirementsDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }





        // POST: /LegalRegister/AddLegalRegister

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddLegalRegister(LegalRegisterModel objLegalRegister, FormCollection form, HttpPostedFileBase upload)
        {
            TempData["Successdata"] = "Added Objective details suhail";
            try
            {
                if (objLegalRegister != null)
                {

                    objLegalRegister.lawNo = form["lawNo"];
                    objLegalRegister.lawTitle = form["lawTitle"];
                    objLegalRegister.origin_of_requirement = form["origin_of_requirement"];
                    objLegalRegister.document_storage_location = form["document_storage_location"];
                    objLegalRegister.frequency_of_evaluation = form["frequency_of_evaluation"];
                    objLegalRegister.activeStatus = form["activeStatus"];
                    objLegalRegister.reviewedBy = form["reviewedBy"];
                    objLegalRegister.updatedByName = form["updatedByName"];
                    objLegalRegister.approvedBy = form["approvedBy"];

                    if (upload != null && upload.ContentLength > 0)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(upload.FileName));
                            string sFilename = "LegalRegister" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                            string sFilepath = Path.GetDirectoryName(spath);

                            upload.SaveAs(sFilepath + "/" + sFilename);
                            objLegalRegister.upload = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                            ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    else
                    {
                        ViewBag.Message = "You have not specified a file.";
                    }

                    DateTime dateValue;

                    if (DateTime.TryParse(form["initialdevelopmentdate"], out dateValue) == true)
                    {
                        objLegalRegister.initialdevelopmentdate = dateValue;
                    }
                    if (DateTime.TryParse(form["updatedOn"], out dateValue) == true)
                    {
                        objLegalRegister.updatedOn = dateValue;
                    }


                    LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
                    objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

                    for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                    {
                        LegalRegisterModel objObjectivesModels = new LegalRegisterModel();

                        DateTime dateValues;

                        if (DateTime.TryParse(form["updatedDate" + i], out dateValue) == true)
                        {
                            objObjectivesModels.updatedDate = dateValue;
                        }




                        objObjectivesModels.article = form["article" + i];
                        objObjectivesModels.requirements = form["requirements" + i];
                        objObjectivesModels.applicable = form["applicable" + i];
                        objObjectivesModels.nonapplicablejustify = form["nonapplicablejustify" + i];
                        // objObjectivesModels.updatedBynametrans = form["updatedBynametrans" + i];
                        objObjectivesModels.updatedByposition = form["updatedByposition" + i];
                        //  objObjectivesModels.updatedByDept = form["updatedByDept" + i];
                        objObjectivesModels.updatedBynametrans = form["updatedBynametrans" + i];
                        objObjectivesModels.updatedByDept = form["updatedByDept" + i];
                        //objObjectivesModels.updatedBynametrans = objGlobaldata.GetCurrentUserSession().empid;
                        //objObjectivesModels.updatedByDept = objGlobaldata.GetDeptNameById("");
                        objObjectivesModels.reference = form["reference" + i];
                        objObjectivesModels.monitoring = form["monitoring" + i];
                        objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                    }

                    if (objLegalRegister.FunAddLegalRegister(objLegalRegister, objObjectivesModelsList))
                    {
                        TempData["Successdata"] = "Added Legal Register details successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("LegalRegisterList");
        }




        [HttpPost]
        public JsonResult doesLawNumberExist(string lawNo)
        {
            var Valid = true;
            if (lawNo != null)
            {
                LegalRegisterModel objLegalRegisterModel = new LegalRegisterModel();
                Valid = objLegalRegisterModel.checkLawNumberExists(lawNo);
            }

            return Json(Valid);
        }

        public bool checkLawNumberExists(string lawNo)
        {
            string sSqlstmt = "select LegalRequirement_Id from t_legalregister where lawNo='" + lawNo + "'";
            DataSet dsEmp = objGlobaldata.Getdetails(sSqlstmt);
            if (dsEmp.Tables.Count > 0 && dsEmp.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            return true;
        }



        // GET: /LegalRegister/LegalRegisterApprove

        [AllowAnonymous]
        public ActionResult LegalRegisterApprove(string approvedBy, string PendingFlg)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                if (objLegalRegister.FunUpdateLegalRegisterApproval(user, approvedBy))
                {
                    TempData["Successdata"] = "LegalRegister Apporved successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                //  }
                //   else
                //  {
                //        TempData["alertdata"] = "Access Denied";
                //  }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("LegalRegisterList");
            }
        }




     

        //========================   reviewed===================


        [AllowAnonymous]
        public ActionResult LegalRegisterReviewed(string LegalRequirement_Id, string PendingFlg, string lawNo)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Reviewer")
                //{
                if (objLegalRegister.FunUpdateLegalRegisterReview(LegalRequirement_Id, lawNo))
                {
                    TempData["Successdata"] = "Document Reviewed successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Access Denied";
                //}

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterReviewed: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForReview", "Dashboard");
            }
            else
            {
                return RedirectToAction("LegalRegisterList");
            }
        }



        [HttpPost]
        public JsonResult UploadDocument()
        {
            HttpFileCollectionBase Action_Plan1 = Request.Files;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(file.FileName));
                string sFilename = Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                file.SaveAs(sFilepath + "/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);
                return Json("~/DataUpload/MgmtDocs/LegalReg/" + "ActionPlan" + DateTime.Now.ToString("ddMMyyyyHHmm") + sFilename);


            }

            return Json("Failed");
        }



        // GET: /LegalRegister/LegalRegisterList

        [AllowAnonymous]
        public ActionResult LegalRegisterList(string SearchText, string frequency_of_evaluation, string approvestatus, int? page)
        {
            // string sid_requirements = Request.QueryString["id_requirements"];
            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            try
            {
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                //ViewBag.id_requirements = sid_requirements;
                ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                //DATE_FORMAT(AuditDate,'%d/%m/%Y') AS  
                string sSqlstmt = "select LegalRequirement_Id, lawNo, lawTitle,initialdevelopmentdate, origin_of_requirement,document_storage_location, frequency_of_evaluation,"
                + " activeStatus,updatedOn, updatedByName,approvedBy,reviewedBy,"
                    + " (case when approvestatus='1' then 'Approved' else 'Not Approved' end) as approvestatus ,(case when reviewstatus='1' then 'Reviewed' else 'Not Reviewed' end) as reviewstatus from t_legalregister ";//where activeStatus=1
                string sSearchtext = "";



                UserCredentials objUser = new UserCredentials();

                objUser = objGlobaldata.GetCurrentUserSession();

                string sRolename = objGlobaldata.GetRoleName(objUser.role);
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                ViewBag.Role = sRolename;
                ViewBag.CurrentEmpName = objUser.firstname;
                ViewBag.approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");
                ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");

                if (sRolename.ToLower() == "Preparer".ToLower())
                {
                    sSqlstmt = sSqlstmt + " where approvestatus=1 or updatedByName='" + objUser.empid + "'";
                }

                if (SearchText != null && SearchText != "")
                {
                    ViewBag.SearchText = SearchText;
                    sSearchtext = " (lawNo ='" + SearchText + "' or lawNo like '" + SearchText + "%' or Dept in (select DeptId from t_departments where "
                        + "approvedBy='" + SearchText + "' or approvedBy like '" + SearchText + "%'))";
                }

                if (sRolename.ToLower() != "Preparer".ToLower() && sSearchtext != "")
                {
                    sSearchtext = " where " + sSearchtext;
                }
                else if (sRolename.ToLower() == "Preparer".ToLower() && sSearchtext != "")
                {
                    sSearchtext = " and " + sSearchtext;
                }

                if (approvestatus != null && approvestatus != "All" && approvestatus != "")
                {
                    ViewBag.ApprovestatusVal = approvestatus;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (approvestatus ='" + approvestatus + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " where (approvestatus ='" + approvestatus + "')";
                    }
                }


                if (frequency_of_evaluation != null && frequency_of_evaluation != "Select")
                {
                    ViewBag.frequency_of_evaluationText = frequency_of_evaluation;
                    if (sSearchtext != "")
                    {
                        sSearchtext = sSearchtext + " and (frequency_of_evaluation ='" + frequency_of_evaluation + "')";
                    }
                    else
                    {
                        sSearchtext = sSearchtext + " where (frequency_of_evaluation ='" + frequency_of_evaluation + "')";
                    }
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by lawNo asc";

                DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                {

                     ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                   

                    for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            LegalRegisterModel objObjectivesModels = new LegalRegisterModel
                            {
                                LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[i]["LegalRequirement_Id"].ToString()),
                                lawNo = dsObjectivesModelsList.Tables[0].Rows[i]["lawNo"].ToString(),
                                lawTitle = dsObjectivesModelsList.Tables[0].Rows[i]["lawTitle"].ToString(),
                                origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[i]["origin_of_requirement"].ToString(),
                                document_storage_location = dsObjectivesModelsList.Tables[0].Rows[i]["document_storage_location"].ToString(),
                                frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[i]["frequency_of_evaluation"].ToString(),
                                activeStatus = dsObjectivesModelsList.Tables[0].Rows[i]["activeStatus"].ToString(),
                                updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["updatedByName"].ToString()),
                                /*here*/
                                reviewedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["reviewedBy"].ToString()),
                                /*here*/
                                approvedBy = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[i]["approvedBy"].ToString()),
                                reviewstatus = dsObjectivesModelsList.Tables[0].Rows[i]["reviewstatus"].ToString(),
                                approvestatus = dsObjectivesModelsList.Tables[0].Rows[i]["approvestatus"].ToString()
                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.initialdevelopmentdate = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                               && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.updatedOn = dtDocDate;
                            }
                            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in LegalRegisterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objObjectivesModelsList.LegalRegisterMList.ToList().ToPagedList(page ?? 1, 40));
        }

        // GET: /LegalRegister/LegalRegisterDetails

        [AllowAnonymous]
        public ActionResult LegalRegisterDetails()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            try
            {
                if (Request.QueryString["LegalRequirement_Id"] != null && Request.QueryString["LegalRequirement_Id"] != "")
                {
                    string sLegalRequirement_Id = Request.QueryString["LegalRequirement_Id"];

                    ViewBag.UserRole = objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role);

                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    string sSqlstmt = "select LegalRequirement_Id, lawNo,lawTitle, initialdevelopmentdate, origin_of_requirement,document_storage_location ,frequency_of_evaluation ,activeStatus, updatedByName,updatedOn,reviewedBy, approvedBy,"
                        + " (case when approvestatus='1' then 'Approved' else 'Not Approved' end) as approvestatus,(case when reviewstatus='1' then 'Reviewed' else 'Not Reviewed' end) as reviewstatus, updatedByName,upload"
                        + "   from t_legalregister where LegalRequirement_Id='" + sLegalRequirement_Id + "'";


                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {


                        objObjectivesModels = new LegalRegisterModel
                        {
                            LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRequirement_Id"].ToString()),
                            lawNo = dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString(),
                            lawTitle = dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString(),
                            origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                            document_storage_location = dsObjectivesModelsList.Tables[0].Rows[0]["document_storage_location"].ToString(),
                            frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[0]["frequency_of_evaluation"].ToString(),
                            activeStatus = dsObjectivesModelsList.Tables[0].Rows[0]["activeStatus"].ToString(),
                            updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()),
                            reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["reviewedBy"].ToString()),
                            approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["approvedBy"].ToString()),
                            reviewstatus = dsObjectivesModelsList.Tables[0].Rows[0]["reviewstatus"].ToString(),
                            approvestatus = dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString(),
                            upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString()
                        };

                        DateTime dtDocDate = new DateTime();
                        if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                            && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.initialdevelopmentdate = dtDocDate;
                        }
                        if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                        && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                        {
                            objObjectivesModels.updatedOn = dtDocDate;
                        }

                        sSqlstmt = "select id_requirements, LegalRequirement_Id, article, requirements, applicable, nonapplicablejustify, updatedBynametrans,"
                                    + "updatedByposition, updatedByDept,updatedDate,reference,monitoring from t_legalrequirementstrans where LegalRequirement_Id='" + objObjectivesModels.LegalRequirement_Id
                                    + "' order by LegalRequirement_Id desc";
                        ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.updatedBynametrans = objGlobaldata.GetEmpHrNameById("updatedBynametrans");
                        ViewBag.updatedByDept = objGlobaldata.GetDeptNameById("updatedByDept");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");

                        //DataSet dsCommnets = objGlobaldata.Getdetails(sSqlstmt);
                        //ViewBag.dsComments = dsCommnets;
                        //   ViewBag.lawNo = objObjectivesModels.checkObjRefExists(dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString());
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                        ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                        ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                        ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                        ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                        ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                        ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        // ViewBag.DeptEmpLists = objGlobaldata.GetMultiEmployeeList(' ', "", dsObjectivesModelsList.Tables[0].Rows[0]["updatedByDept"].ToString());
                        ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                    }
                    else
                    {
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("LegalRegisterList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "LegalRegister Id cannot be null";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objObjectivesModels);

        }

        // POST: /LegalRegister/LegalRegisterDetails
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LegalRegisterDetails(LegalRegisterModel objObjectivesModels, FormCollection form)
        //{
        //    try
        //    {
        //        if (form["LegalRequirement_Id"] != null && form["LegalRequirement_Id"] != "")
        //        {
        //            //string sLegalRequirement_Id = form["LegalRequirement_Id"], sComments = form["Comment"];

        //            //if (objObjectivesModels.FunAddComments(objGlobaldata.GetCurrentUserSession().empid, sComments, sLegalRequirement_Id))
        //            //{
        //            //    TempData["Successdata"] = "Document Comments details added successfully";
        //            //    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = sLegalRequirement_Id });
        //            //}
        //            //else
        //            //{
        //            //    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //objGlobaldata.AddFunctionalLog("Exception in LegalRegisterDetails: " + ex.ToString());
        //        //TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
        //    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
        //    return RedirectToAction("LegalRegisterList");
        //}

        // GET: /LegalRegister/LegalRegisterEdit

        [AllowAnonymous]
        public ActionResult LegalRegisterEdit()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    string sLegalRequirement_Id = Request.QueryString["LegalRequirement_Id"];

                    string sSqlstmt = "select LegalRequirement_Id,   lawNo, lawTitle,initialdevelopmentdate, origin_of_requirement,document_storage_location, frequency_of_evaluation,"
                + " activeStatus,updatedOn, updatedByName,approvedBy,reviewedBy,"
                    + "  approvestatus , reviewstatus,upload from t_legalregister   where LegalRequirement_Id='" + sLegalRequirement_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        //if (objUser.empid == dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()
                        //    && dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString() == "0")
                        {
                            objObjectivesModels = new LegalRegisterModel
                            {
                                LegalRequirement_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRequirement_Id"].ToString()),
                                lawNo = dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString(),
                                lawTitle = dsObjectivesModelsList.Tables[0].Rows[0]["lawTitle"].ToString(),
                                origin_of_requirement = dsObjectivesModelsList.Tables[0].Rows[0]["origin_of_requirement"].ToString(),
                                document_storage_location = dsObjectivesModelsList.Tables[0].Rows[0]["document_storage_location"].ToString(),
                                frequency_of_evaluation = dsObjectivesModelsList.Tables[0].Rows[0]["frequency_of_evaluation"].ToString(),
                                activeStatus = dsObjectivesModelsList.Tables[0].Rows[0]["activeStatus"].ToString(),
                                updatedByName = objGlobaldata.GetEmpHrNameById(dsObjectivesModelsList.Tables[0].Rows[0]["updatedByName"].ToString()),
                                reviewedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["reviewedBy"].ToString()),
                                approvedBy = objGlobaldata.GetMultiHrEmpNameById(dsObjectivesModelsList.Tables[0].Rows[0]["approvedBy"].ToString()),
                                reviewstatus = dsObjectivesModelsList.Tables[0].Rows[0]["reviewstatus"].ToString(),
                                approvestatus = dsObjectivesModelsList.Tables[0].Rows[0]["approvestatus"].ToString(),
                                upload = dsObjectivesModelsList.Tables[0].Rows[0]["upload"].ToString()
                            };

                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["initialdevelopmentdate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.initialdevelopmentdate = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString() != ""
                            && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["updatedOn"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.updatedOn = dtDocDate;
                            }

                            sSqlstmt = "select id_requirements, LegalRequirement_Id, article, requirements, applicable, nonapplicablejustify,"
                                            + "updatedDate, updatedBynametrans,updatedByposition,updatedByDept,reference,monitoring from t_legalrequirementstrans where LegalRequirement_Id='" + objObjectivesModels.LegalRequirement_Id
                                            + "' order by id_requirements desc";
                            ViewBag.FindingsDetails = objGlobaldata.Getdetails(sSqlstmt);
                            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                            ViewBag.lawNo = objObjectivesModels.GetLegalRegisterRef(dsObjectivesModelsList.Tables[0].Rows[0]["lawNo"].ToString());
                            ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                            ViewBag.DeptList = objGlobaldata.GetDepartmentListbox();
                            ViewBag.IsoStdList = objGlobaldata.GetAllIsoStdListbox();
                            ViewBag.frequency_of_evaluation = objGlobaldata.GetConstantValue("frequency_of_evaluation");
                            ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                            ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                            ViewBag.activeStatus = objGlobaldata.GetConstantValue("activeStatus");
                            ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.deptlist = objGlobaldata.GetDepartmentList();
                            ViewBag.DeptHead = objGlobaldata.GetDeptHeadList("");
                            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                            // ViewBag.DeptEmpLists = objGlobaldata.GetMultiEmployeeList(' ', "", dsObjectivesModelsList.Tables[0].Rows[0]["Dept"].ToString());
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                            ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                            ViewBag.applicable = objGlobaldata.GetConstantValue("applicable");
                            //   ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                            return View(objObjectivesModels);
                            //  return RedirectToAction("LegalRegisterList");
                        }
                        //else
                        //{
                        //    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        //    TempData["alertdata"] = "Access Denied";
                        //    return RedirectToAction("LegalRegisterList");
                        //}
                    }
                    else
                    {
                        ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                        ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                        ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                        TempData["alertdata"] = "No Data exists";
                        return RedirectToAction("LegalRegisterList");

                    }
                }
                else
                {

                    ViewBag.approvedBy = objGlobaldata.GetEmpHrNameById("approvedBy");
                    ViewBag.reviewedBy = objGlobaldata.GetEmpHrNameById("reviewedBy");
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "LegalRegister Id cannot be Null or empty";
                    return RedirectToAction("LegalRegisterList");
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList");

        }

        // POST: /LegalRegister/LegalRegisterEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LegalRegisterEdit(LegalRegisterModel objLegalRegister, FormCollection form, HttpPostedFileBase upload)
        {
            objLegalRegister.article = form["article"];
            objLegalRegister.requirements = form["requirements"];
            objLegalRegister.applicable = form["applicable"];
            objLegalRegister.nonapplicablejustify = form["nonapplicablejustify"];
            objLegalRegister.updatedByposition = form["updatedByposition"];
            objLegalRegister.updatedBynametrans = form["updatedBynametrans"];
            objLegalRegister.updatedByDept = form["updatedByDept"];
            if (upload != null && upload.ContentLength > 0)
            {
                try
                {
                    string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(upload.FileName));
                    string sFilename = "LegalRegister" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                    string sFilepath = Path.GetDirectoryName(spath);

                    upload.SaveAs(sFilepath + "/" + sFilename);
                    objLegalRegister.upload = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    objGlobaldata.AddFunctionalLog("Exception in AddLegalRegister-upload: " + ex.ToString());
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }

            if (objLegalRegister.FunUpdateLegalRegister(objLegalRegister))
            {
                TempData["Successdata"] = "Legal Register details updated successfully";
            }
            else
            {
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList"); /*new { LegalRequirement_Id = objLegalRegister.LegalRequirement_Id });*/



        }



        [AllowAnonymous]
        public bool LegalRegisterPlanAdd(LegalRegisterModel objObjectivesModels, FormCollection form)
        {
            try
            {
                LegalRegisterModelsList objLegalRegisterModelsList = new LegalRegisterModelsList();
                objLegalRegisterModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

                DateTime dateValue;

                if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
                {
                    objObjectivesModels.updatedDate = dateValue;
                }

                objObjectivesModels.applicable = form["applicable"];

                objLegalRegisterModelsList.LegalRegisterMList.Add(objObjectivesModels);

                return objObjectivesModels.FunAddLegalRegisterTrans(objLegalRegisterModelsList, Convert.ToInt16(objObjectivesModels.LegalRequirement_Id));
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //POST: /LegalRegister/ObjectivePlanUpdate

        [AllowAnonymous]
        public ActionResult LegalRegisterPlanUpdate(LegalRegisterModel objObjectivesModels, FormCollection form)
        {
            try
            {


                if (Request["button"].Equals("Save"))
                {


                    if (LegalRegisterPlanAdd(objObjectivesModels, form))
                    {
                        TempData["Successdata"] = "Added Legal Regsiter  successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {



                    DateTime dateValue;

                    if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
                    {
                        objObjectivesModels.updatedDate = dateValue;
                    }

                    objObjectivesModels.article = form["article"];
                    objObjectivesModels.requirements = form["requirements"];
                    objObjectivesModels.applicable = form["applicable"];
                    objObjectivesModels.nonapplicablejustify = form["nonapplicablejustify"];
                    objObjectivesModels.updatedBynametrans = form["updatedBynametrans"];
                    objObjectivesModels.updatedByposition = form["updatedByposition"];
                    objObjectivesModels.updatedByDept = form["updatedByDept"];
                    objObjectivesModels.approvedBy = form["approvedBy"];


                    //ExternalAuditModels objExternalAuditModels = new ExternalAuditModels();

                    if (objObjectivesModels.FunUpdateLegisterRegisterPlan(objObjectivesModels))
                    {
                        TempData["Successdata"] = "LegalRegister plan updated successfully";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                return RedirectToAction("LegalRegisterEdit", new { LegalRequirement_Id = objObjectivesModels.LegalRequirement_Id });

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterPlanUpdate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterList");
        }
        //POST: /LegalRegister/ObjectivePlanAdd

        [AllowAnonymous]
        public bool LegalRegisterPlan(LegalRegisterModel objObjectivesModels, FormCollection form)
        {

            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            DateTime dateValue;

            if (DateTime.TryParse(form["updatedDate"], out dateValue) == true)
            {
                objObjectivesModels.updatedDate = dateValue;
            }
            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);

            return objObjectivesModels.FunAddLegalRegisterTrans(objObjectivesModelsList, Convert.ToInt16(objObjectivesModels.LegalRequirement_Id));


        }





        //
        // GET: /LegalRegister/AddLegalRegisterEvaluation

        [AllowAnonymous]
        public ActionResult AddLegalRegisterEvaluation()
        {
            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();

                if (Request.QueryString["id_requirements"] != null)
                {
                    string sid_requirements = Request.QueryString["id_requirements"];
                    ViewBag.id_requirements = sid_requirements;
                    DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id, updatedDate,updatedBynametrans from t_legalrequirementstrans where id_requirements='"
                        + sid_requirements + "'");
                    if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.activeStatus = objGlobaldata.GetConstantValue("comply");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                        ViewBag.updatedBynametrans = dsObjectives_Id.Tables[0].Rows[0]["updatedBynametrans"].ToString();
                        ViewBag.updatedDate = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["updatedDate"].ToString()).ToString("dd/MM/yyyy");
                    }
                }
                else
                {

                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "Legal Register Id cannot be Null or empty";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegisterEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }


        //
        // POST: /LegalRegister/AddLegalRegisterEvaluation

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLegalRegisterEvaluation(LegalRegisterModel objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
        {
            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["LegalRegister_Eval_On"], out dateValue) == true)
                {
                    objObjectivesModels.LegalRegister_Eval_On = dateValue;
                }

                if (DateTime.TryParse(form["Duedate"], out dateValue) == true)
                {
                    objObjectivesModels.Duedate = dateValue;
                }
                //  objObjectivesModels.id_requirements = Convert.ToInt32(form["id_requirements"]).ToString();

                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                        ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                        ViewBag.updatedByName = objGlobaldata.GetEmpHrNameById("updatedByName");
                        ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objObjectivesModels.FunAddLegalRegisterEvaluation(objObjectivesModels))
                {
                    TempData["Successdata"] = "Added Legal Register Evaluation details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddLegalRegisterEvaluation: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
            ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
            ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();


            return RedirectToAction("LegalRegisterEvaluationList", new { id_requirements = objObjectivesModels.id_requirements });
        }


        // GET: /LegalRegister/ObjectiveEvaluationList

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationList()
        {
            LegalRegisterModelsList objObjectivesModelsList = new LegalRegisterModelsList();
            objObjectivesModelsList.LegalRegisterMList = new List<LegalRegisterModel>();

            //ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
            //  ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox('I');
            try
            {
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                if (Request.QueryString["id_requirements"] != null)
                {
                    string sid_requirements = Request.QueryString["id_requirements"];
                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence,Comply_action,personel_responsible "
                                        + "from t_legalregister_evaluation where id_requirements='" + sid_requirements + "'";
                    ViewBag.id_requirements = sid_requirements;
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    ViewBag.personel_responsible = objGlobaldata.GetEmpHrNameById("personel_responsible");
                    ViewBag.EmpLists = objGlobaldata.GetHrEmployeeListbox();
                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < dsObjectivesModelsList.Tables[0].Rows.Count; i++)
                            //{
                            //    DateTime LegalRegister_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["dtlegalregister_evaluation_Id"].ToString());
                            //    DateTime dtDuedate= Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[i]["Duedate"].ToString());
                            ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        try
                        {
                            LegalRegisterModel objObjectivesModels = new LegalRegisterModel
                            {
                                legalregister_evaluation_Id = Convert.ToInt32(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                                id_requirements = Convert.ToInt32(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                                comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                                yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                                no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                                Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                                Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                                personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),

                            };
                            DateTime dtDocDate = new DateTime();
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString() != ""
                                && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.LegalRegister_Eval_On = dtDocDate;
                            }
                            if (dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString() != ""
                               && DateTime.TryParse(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString(), out dtDocDate))
                            {
                                objObjectivesModels.Duedate = dtDocDate;
                            }

                            objObjectivesModelsList.LegalRegisterMList.Add(objObjectivesModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                    if (sid_requirements != "")
                    {
                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id from t_legalrequirementstrans where id_requirements='" + sid_requirements + "'");
                        if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                        }


                        else if (Request.QueryString["LegalRequirement_Id"] != null)
                        {

                            return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                        }
                        else
                        {
                            TempData["alertdata"] = "No data exists";
                            return RedirectToAction("LegalRegisterList");
                        }
                    }

                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                }
                else
                {
                    TempData["alertdata"] = "LegalRegister Trans Id cannot be null";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }


            return View(objObjectivesModelsList.LegalRegisterMList.ToList());
        }

        //
        // GET: /LegalRegister/ObjectiveEvaluationDetails

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationDetails()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();

            try
            {
                ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                if (Request.QueryString["id_requirements"] != null && Request.QueryString["id_requirements"] != "")
                {
                    string sid_requirements = Request.QueryString["id_requirements"];

                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence ,Comply_action,personel_responsible"
                                        + "from t_legalregister_evaluation where id_requirements=" + sid_requirements;

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    if (dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtObj_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString());
                        DateTime dtDuedate = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString());

                        objObjectivesModels = new LegalRegisterModel
                        {
                            legalregister_evaluation_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                            id_requirements = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                            LegalRegister_Eval_On = dtObj_Eval_On,
                            Duedate = dtDuedate,
                            comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                            yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                            no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                            Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                            Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                            personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),
                        };

                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id from t_legalrequirementstrans where id_requirements='"
                            + objObjectivesModels.id_requirements + "'");
                        ViewBag.LegalRequirement_Id = dsObjectives_Id.Tables[0].Rows[0]["LegalRequirement_Id"].ToString();
                    }
                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                    else
                    {
                        return RedirectToAction("AddLegalRegisterEvaluation", new { id_requirements = sid_requirements });
                    }
                }
                else if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                }
                else
                {
                    ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objObjectivesModels);

        }

        //
        // GET: /LegalRegister/ObjectiveEvaluationEdit

        [AllowAnonymous]
        public ActionResult LegalRegisterEvaluationEdit()
        {
            LegalRegisterModel objObjectivesModels = new LegalRegisterModel();


            try
            {
                UserCredentials objUser = new UserCredentials();
                objUser = objGlobaldata.GetCurrentUserSession();
                ViewBag.UserRole = objGlobaldata.GetRoleName(objUser.role);
                if (Request.QueryString["legalregister_evaluation_Id"] != null && Request.QueryString["legalregister_evaluation_Id"] != "")
                {
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    string slegalregister_evaluation_Id = Request.QueryString["legalregister_evaluation_Id"];

                    string sSqlstmt = "select legalregister_evaluation_Id, id_requirements, LegalRegister_Eval_On, Duedate, comply, yes_comply_reason, no_comply_reason, Evidence, Comply_action  ,personel_responsible "
                                        + "from t_legalregister_evaluation where legalregister_evaluation_Id='" + slegalregister_evaluation_Id + "'";

                    DataSet dsObjectivesModelsList = objGlobaldata.Getdetails(sSqlstmt);
                    ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                    if (dsObjectivesModelsList.Tables.Count > 0 && dsObjectivesModelsList.Tables[0].Rows.Count > 0)
                    {
                        DateTime dtLegalRegister_Eval_On = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["LegalRegister_Eval_On"].ToString());
                        DateTime dtDuedate = Convert.ToDateTime(dsObjectivesModelsList.Tables[0].Rows[0]["Duedate"].ToString());
                        ViewBag.comply = objGlobaldata.GetConstantValue("comply");
                        objObjectivesModels = new LegalRegisterModel
                        {
                            legalregister_evaluation_Id = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["legalregister_evaluation_Id"].ToString()),
                            id_requirements = Convert.ToInt16(dsObjectivesModelsList.Tables[0].Rows[0]["id_requirements"].ToString()),
                            LegalRegister_Eval_On = dtLegalRegister_Eval_On,
                            Duedate = dtDuedate,
                            comply = dsObjectivesModelsList.Tables[0].Rows[0]["comply"].ToString(),
                            yes_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["yes_comply_reason"].ToString(),
                            no_comply_reason = dsObjectivesModelsList.Tables[0].Rows[0]["no_comply_reason"].ToString(),
                            Evidence = dsObjectivesModelsList.Tables[0].Rows[0]["Evidence"].ToString(),
                            Comply_action = dsObjectivesModelsList.Tables[0].Rows[0]["Comply_action"].ToString(),
                            personel_responsible = dsObjectivesModelsList.Tables[0].Rows[0]["personel_responsible"].ToString(),
                        };
                        ViewBag.id_requirements = objObjectivesModels.id_requirements;
                        ViewBag.legalregister_evaluation_Id = objObjectivesModels.legalregister_evaluation_Id;
                        DataSet dsObjectives_Id = objGlobaldata.Getdetails("select LegalRequirement_Id,updatedDate from t_legalrequirementstrans where id_requirements='"
                            + objObjectivesModels.id_requirements + "'");

                        if (dsObjectives_Id.Tables.Count > 0 && dsObjectives_Id.Tables[0].Rows.Count > 0)
                        {

                            objObjectivesModels.updatedDate = Convert.ToDateTime(dsObjectives_Id.Tables[0].Rows[0]["updatedDate"].ToString());
                        }
                    }
                    else if (Request.QueryString["LegalRequirement_Id"] != null)
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("LegalRegisterList");
                    }
                }
                else if (Request.QueryString["LegalRequirement_Id"] != null)
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterDetails", new { LegalRequirement_Id = Request.QueryString["LegalRequirement_Id"] });
                }
                else
                {
                    TempData["alertdata"] = "No data exists";
                    return RedirectToAction("LegalRegisterList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ObjectiveEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            ViewBag.comply = objGlobaldata.GetConstantValue("comply");

            return View(objObjectivesModels);
        }


        //
        // POST: /LegalRegister/ObjectiveEvaluationEdit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LegalRegisterEvaluationEdit(LegalRegisterModel objObjectivesModels, FormCollection form, HttpPostedFileBase Evidence)
        {
            try
            {
                DateTime dateValue;

                if (DateTime.TryParse(form["LegalRegister_Eval_On"], out dateValue) == true)
                {
                    objObjectivesModels.LegalRegister_Eval_On = dateValue;
                }
                if (DateTime.TryParse(form["Duedate"], out dateValue) == true)
                {
                    objObjectivesModels.Duedate = dateValue;
                }
                objObjectivesModels.legalregister_evaluation_Id = Convert.ToInt16(form["legalregister_evaluation_Id"]);
                objObjectivesModels.id_requirements = Convert.ToInt16(form["id_requirements"]);

                objObjectivesModels.updatedByName = objGlobaldata.GetCurrentUserSession().empid;





                if (Evidence != null && Evidence.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/LegalReg"), Path.GetFileName(Evidence.FileName));
                        string sFilename = "Evidence" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Evidence.SaveAs(sFilepath + "/" + sFilename);
                        objObjectivesModels.Evidence = "~/DataUpload/MgmtDocs/LegalReg/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objObjectivesModels.FunUpdateLegalRegisterEvaluation(objObjectivesModels))
                {
                    TempData["Successdata"] = "Legal Register Evaluation details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterEvaluationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("LegalRegisterEvaluationList", new { id_requirements = objObjectivesModels.id_requirements });
        }
        public JsonResult LegalRegisterApproveNoty(string approvedBy, string PendingFlg)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                string user = "";

                user = objGlobaldata.GetCurrentUserSession().empid;

                if (objLegalRegister.FunUpdateLegalRegisterApproval(user, approvedBy))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
                //  }
                //   else
                //  {
                //        TempData["alertdata"] = "Access Denied";
                //  }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterApprove: " + ex.ToString());
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

        public JsonResult LegalRegisterReviewedNoty(string LegalRequirement_Id, string PendingFlg, string lawNo)
        {
            try
            {
                LegalRegisterModel objLegalRegister = new LegalRegisterModel();
                //string filename = Path.GetFileName(Document);
                //FileStream fsSource = new FileStream(Server.MapPath(Document), FileMode.Open, FileAccess.Read);
                //if (objGlobaldata.GetRoleName(objGlobaldata.GetCurrentUserSession().role) == "Reviewer")
                //{
                if (objLegalRegister.FunUpdateLegalRegisterReview(LegalRequirement_Id, lawNo))
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Failed");
                }
                //}
                //else
                //{
                //    TempData["alertdata"] = "Access Denied";
                //}

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in LegalRegisterReviewed: " + ex.ToString());
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
