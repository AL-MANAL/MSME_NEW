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
    public class WasteManagementController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();


        public WasteManagementController()
        {
            ViewBag.Menutype = "HSE";
            ViewBag.SubMenutype = "WasteManagement";
        }


        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult AddWasteManagement()
        {
            try
            {
                ViewBag.SubMenutype = "Waste management";
                ViewBag.Units = objGlobaldata.GetDropdownList("Waste Units");
                ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Supplier = objGlobaldata.GetSupplierList();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Waste Management: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWasteManagement(WasteManagementModels objComp, FormCollection form, IEnumerable<HttpPostedFileBase> Upload_doc)
        {
            try
            {
                
                HttpPostedFileBase files = Request.Files[0];
                if (Upload_doc != null && files.ContentLength > 0)
                {
                    objComp.Upload_doc = "";
                    foreach (var file in Upload_doc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/WasteManagement"), Path.GetFileName(file.FileName));
                            string sFilename = "Waste" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.Upload_doc = objComp.Upload_doc + "," + "~/DataUpload/MgmtDocs/WasteManagement/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddWasteManagement-upload: " + ex.ToString());

                        }
                    }
                    objComp.Upload_doc = objComp.Upload_doc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objComp.FunAddWaste(objComp))
                {
                    TempData["Successdata"] = "Added Waste details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddWaste: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("WasteManagementList");
        }

        [AllowAnonymous]
        public ActionResult WasteManagementList(string branch_name)
        {
            ViewBag.SubMenutype = "WasteManagement";

            WasteManagementModelsList objCom = new WasteManagementModelsList();
            objCom.WasteManagementList = new List<WasteManagementModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select Id_waste,Wate_type,Quantity,Units,Collection_Method,Location,Destination,Recv_facility," +
                    "Collected_by,Upload_doc,Remarks,Disposal_date,DisposalBy from t_waste_management where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt= sSqlstmt + sSearchtext + " order by Id_waste";
                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {

                    

                    for (int i = 0; i < dsComplList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WasteManagementModels objWasteModels = new WasteManagementModels
                            {
                                Id_waste = dsComplList.Tables[0].Rows[i]["Id_waste"].ToString(),
                                Wate_type = dsComplList.Tables[0].Rows[i]["Wate_type"].ToString(),
                                Quantity = Convert.ToDecimal(dsComplList.Tables[0].Rows[i]["Quantity"].ToString()),
                                Units = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["Units"].ToString()),
                                Collection_Method = dsComplList.Tables[0].Rows[i]["Collection_Method"].ToString(),
                                Location = /*objGlobaldata.GetCompanyBranchNameById*/(dsComplList.Tables[0].Rows[i]["Location"].ToString()),
                                Destination = dsComplList.Tables[0].Rows[i]["Destination"].ToString(),
                                Recv_facility = dsComplList.Tables[0].Rows[i]["Recv_facility"].ToString(),
                                Collected_by = dsComplList.Tables[0].Rows[i]["Collected_by"].ToString(),
                                Upload_doc = dsComplList.Tables[0].Rows[i]["Upload_doc"].ToString(),
                                Remarks = dsComplList.Tables[0].Rows[i]["Remarks"].ToString(),
                                DisposalBy = objGlobaldata.GetSupplierNameById(dsComplList.Tables[0].Rows[i]["DisposalBy"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Disposal_date"].ToString(), out dateValue))
                            {
                                objWasteModels.Disposal_date = dateValue;
                            }

                            objCom.WasteManagementList.Add(objWasteModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in WastemanagementList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteManagementList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objCom.WasteManagementList.ToList());
        }

        [AllowAnonymous]
        public JsonResult WasteManagementListSearch(string branch_name)
        {
            ViewBag.SubMenutype = "WasteManagement";

            WasteManagementModelsList objCom = new WasteManagementModelsList();
            objCom.WasteManagementList = new List<WasteManagementModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select Id_waste,Wate_type,Quantity,Units,Collection_Method,Location,Destination," +
                    "Recv_facility,Collected_by,Upload_doc,Remarks,Disposal_date,DisposalBy from t_waste_management where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Id_waste";
                DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                {

                   
                    for (int i = 0; i < dsComplList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            WasteManagementModels objWasteModels = new WasteManagementModels
                            {
                                Id_waste = dsComplList.Tables[0].Rows[i]["Id_waste"].ToString(),
                                Wate_type = dsComplList.Tables[0].Rows[i]["Wate_type"].ToString(),
                                Quantity = Convert.ToDecimal(dsComplList.Tables[0].Rows[i]["Quantity"].ToString()),
                                Units = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[i]["Units"].ToString()),
                                Collection_Method = dsComplList.Tables[0].Rows[i]["Collection_Method"].ToString(),
                                Location = objGlobaldata.GetCompanyBranchNameById(dsComplList.Tables[0].Rows[i]["Location"].ToString()),
                                Destination = dsComplList.Tables[0].Rows[i]["Destination"].ToString(),
                                Recv_facility = dsComplList.Tables[0].Rows[i]["Recv_facility"].ToString(),
                                Collected_by = dsComplList.Tables[0].Rows[i]["Collected_by"].ToString(),
                                Upload_doc = dsComplList.Tables[0].Rows[i]["Upload_doc"].ToString(),
                                Remarks = dsComplList.Tables[0].Rows[i]["Remarks"].ToString(),
                                DisposalBy = objGlobaldata.GetSupplierNameById(dsComplList.Tables[0].Rows[i]["DisposalBy"].ToString()),
                            };

                            DateTime dateValue;
                            if (DateTime.TryParse(dsComplList.Tables[0].Rows[i]["Disposal_date"].ToString(), out dateValue))
                            {
                                objWasteModels.Disposal_date = dateValue;
                            }

                            objCom.WasteManagementList.Add(objWasteModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in WasteManagementListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteManagementListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        public JsonResult FunDeleteWaste(FormCollection form)
        {
            try
            {
                
                    if (form["Id_waste"] != null && form["Id_waste"] != "")
                    {

                        WasteManagementModels Doc = new WasteManagementModels();
                        string sId_waste = form["Id_waste"];


                        if (Doc.FunDeleteWaste(sId_waste))
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
                        TempData["alertdata"] = "Id_waste  cannot be Null or empty";
                        return Json("Failed");
                    }
               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in IssueDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public ActionResult WasteManagementEdit()
        {
            ViewBag.SubMenutype = "WasteManagement";
            WasteManagementModels objComp = new WasteManagementModels();

            UserCredentials objUser = new UserCredentials();

            objUser = objGlobaldata.GetCurrentUserSession();

            try
            {

                if (Request.QueryString["Id_waste"] != null && Request.QueryString["Id_waste"] != "")
                {
                    string sId_waste = Request.QueryString["Id_waste"];
                    string sSqlstmt = "select Id_waste,Wate_type,Quantity,Units,Collection_Method,Location,Destination," +
                        "Recv_facility,Collected_by,Upload_doc,Remarks,Disposal_date,DisposalBy from t_waste_management"
                    + " where Id_waste='" + sId_waste + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {

                        objComp = new WasteManagementModels
                        {
                                Id_waste = dsComplList.Tables[0].Rows[0]["Id_waste"].ToString(),
                                Wate_type = dsComplList.Tables[0].Rows[0]["Wate_type"].ToString(),
                                Quantity = Convert.ToDecimal(dsComplList.Tables[0].Rows[0]["Quantity"].ToString()),
                                Units = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["Units"].ToString()),
                                Collection_Method = dsComplList.Tables[0].Rows[0]["Collection_Method"].ToString(),
                                Location = /*objGlobaldata.GetCompanyBranchNameById*/(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                                Destination = dsComplList.Tables[0].Rows[0]["Destination"].ToString(),
                                Recv_facility = dsComplList.Tables[0].Rows[0]["Recv_facility"].ToString(),
                                Collected_by = dsComplList.Tables[0].Rows[0]["Collected_by"].ToString(),
                                Upload_doc = dsComplList.Tables[0].Rows[0]["Upload_doc"].ToString(),
                                Remarks = dsComplList.Tables[0].Rows[0]["Remarks"].ToString(),
                                DisposalBy = /*objGlobaldata.GetSupplierNameById*/(dsComplList.Tables[0].Rows[0]["DisposalBy"].ToString()),
                        };
                        ViewBag.Units = objGlobaldata.GetDropdownList("Waste Units");
                        ViewBag.Location = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Supplier = objGlobaldata.GetSupplierList();

                        DateTime dateValue;
                        if (DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Disposal_date"].ToString(), out dateValue))
                        {
                            objComp.Disposal_date = dateValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("WasteManagementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("WasteManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteManagementEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("WasteManagementList");
            }
            return View(objComp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WasteManagementEdit(WasteManagementModels objComp, FormCollection form, IEnumerable<HttpPostedFileBase> Upload_doc)
        {
            try
            {
                DateTime dateValue;
                if (form["Disposal_date"] != null && DateTime.TryParse(form["Disposal_date"], out dateValue) == true)
                {
                    objComp.Disposal_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];

                if (Upload_doc != null && files.ContentLength > 0)
                {
                    objComp.Upload_doc = "";
                    foreach (var file in Upload_doc)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/WasteManagement"), Path.GetFileName(file.FileName));
                            string sFilename = "Waste" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objComp.Upload_doc = objComp.Upload_doc + "," + "~/DataUpload/MgmtDocs/WasteManagement/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in WasteEdit-upload: " + ex.ToString());

                        }
                    }
                    objComp.Upload_doc = objComp.Upload_doc.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objComp.Upload_doc = objComp.Upload_doc + "," + form["QCDocsVal"];
                    objComp.Upload_doc = objComp.Upload_doc.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objComp.Upload_doc = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objComp.Upload_doc = null;
                }
                if (objComp.FunUpdateWaste(objComp))
                {
                    TempData["Successdata"] = "Updated Waste details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("WasteManagementList");
        }

        public ActionResult WasteDetails()
        {
            ViewBag.SubMenutype = "Waste";
            WasteManagementModels objComp = new WasteManagementModels();

            try
            {
                if (Request.QueryString["Id_waste"] != null && Request.QueryString["Id_waste"] != "")
                {
                    string sId_waste = Request.QueryString["Id_waste"];
                    string sSqlstmt = "select Id_waste,Wate_type,Quantity,Units,Collection_Method,Location,Destination,Recv_facility,Collected_by,Upload_doc,Remarks,Disposal_date,DisposalBy from t_waste_management"
                    + " where Id_waste='" + sId_waste + "'";

                    DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);


                    if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
                    {

                        objComp = new WasteManagementModels
                        {
                            Id_waste = dsComplList.Tables[0].Rows[0]["Id_waste"].ToString(),
                            Wate_type = dsComplList.Tables[0].Rows[0]["Wate_type"].ToString(),
                            Quantity = Convert.ToDecimal(dsComplList.Tables[0].Rows[0]["Quantity"].ToString()),
                            Units = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["Units"].ToString()),
                            Collection_Method = dsComplList.Tables[0].Rows[0]["Collection_Method"].ToString(),
                            Location = /*objGlobaldata.GetCompanyBranchNameById*/(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                            Destination = dsComplList.Tables[0].Rows[0]["Destination"].ToString(),
                            Recv_facility = dsComplList.Tables[0].Rows[0]["Recv_facility"].ToString(),
                            Collected_by = dsComplList.Tables[0].Rows[0]["Collected_by"].ToString(),
                            Upload_doc = dsComplList.Tables[0].Rows[0]["Upload_doc"].ToString(),
                            Remarks = dsComplList.Tables[0].Rows[0]["Remarks"].ToString(),
                            DisposalBy = objGlobaldata.GetSupplierNameById(dsComplList.Tables[0].Rows[0]["DisposalBy"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Disposal_date"].ToString(), out dateValue))
                        {
                            objComp.Disposal_date = dateValue;
                        }

                    }
                    else
                    {
                        TempData["alertdata"] = "ID cannot be Null or empty";
                        return RedirectToAction("WasteManagementList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "ID cannot be Null or empty";
                    return RedirectToAction("WasteManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("WasteManagementList");
            }
            return View(objComp);
        }
        
        public ActionResult WasteInfo(int id)
        {
            WasteManagementModels objComp = new WasteManagementModels();

            try
            {
            string sSqlstmt = "select Id_waste,Wate_type,Quantity,Units,Collection_Method,Location,Destination,Recv_facility,Collected_by,Upload_doc,Remarks,Disposal_date,DisposalBy from t_waste_management"
        + " where Id_waste='" + id + "'";

            DataSet dsComplList = objGlobaldata.Getdetails(sSqlstmt);


            if (dsComplList.Tables.Count > 0 && dsComplList.Tables[0].Rows.Count > 0)
            {

                objComp = new WasteManagementModels
                {
                    Id_waste = dsComplList.Tables[0].Rows[0]["Id_waste"].ToString(),
                    Wate_type = dsComplList.Tables[0].Rows[0]["Wate_type"].ToString(),
                    Quantity = Convert.ToDecimal(dsComplList.Tables[0].Rows[0]["Quantity"].ToString()),
                    Units = objGlobaldata.GetDropdownitemById(dsComplList.Tables[0].Rows[0]["Units"].ToString()),
                    Collection_Method = dsComplList.Tables[0].Rows[0]["Collection_Method"].ToString(),
                    Location =/* objGlobaldata.GetCompanyBranchNameById*/(dsComplList.Tables[0].Rows[0]["Location"].ToString()),
                    Destination = dsComplList.Tables[0].Rows[0]["Destination"].ToString(),
                    Recv_facility = dsComplList.Tables[0].Rows[0]["Recv_facility"].ToString(),
                    Collected_by = dsComplList.Tables[0].Rows[0]["Collected_by"].ToString(),
                    Upload_doc = dsComplList.Tables[0].Rows[0]["Upload_doc"].ToString(),
                    Remarks = dsComplList.Tables[0].Rows[0]["Remarks"].ToString(),
                    DisposalBy = objGlobaldata.GetSupplierNameById(dsComplList.Tables[0].Rows[0]["DisposalBy"].ToString()),
                };
                    DateTime dateValue;
                    if (DateTime.TryParse(dsComplList.Tables[0].Rows[0]["Disposal_date"].ToString(), out dateValue))
                    {
                        objComp.Disposal_date = dateValue;
                    }
                }
                else
                {
                    TempData["alertdata"] = "No Data exists";
                    return RedirectToAction("WasteManagementList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in WasteDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objComp);

        }

    }
}

