using ISOStd.Filters;
using ISOStd.Models;
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
    public class SupplierController : Controller
    {
        private clsGlobal objGlobaldata = new clsGlobal();

        public SupplierController()
        {
            ViewBag.Menutype = "Suppliers";
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult AddSupplier()
        {
            SupplierModels objSuppModel = new SupplierModels();
            try
            {
                ViewBag.SubMenutype = "SupplierList";

                objSuppModel.branch = objGlobaldata.GetCurrentUserSession().division;
                objSuppModel.Department = objGlobaldata.GetCurrentUserSession().DeptID;
                objSuppModel.Location = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objSuppModel.branch);
                ViewBag.Location = objGlobaldata.GetDivisionLocationList(objSuppModel.branch);
                ViewBag.ApprovalCriteria = objGlobaldata.GetDropdownList("Supplier-ApprovalCriteria");
                //  ViewBag.Approver = objGlobaldata.GetGRoleList(objSuppModel.branch, objSuppModel.Department, objSuppModel.Location, "Approver");
                ViewBag.Approver = objGlobaldata.GetApprover();
                ViewBag.paymentTerm = objGlobaldata.GetDropdownList("Supplier payment term");
                ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
                //ViewBag.SuppCriticality = objGlobaldata.GetConstantValue("YesNo");
                ViewBag.SuppCriticality = objGlobaldata.GetConstantValue("Criticality");
                ViewBag.SupplierType = objGlobaldata.GetDropdownList("Supplier Type");
                ViewBag.SupplierTypes = objGlobaldata.GetDropdownList("Supplier Work Type");
                ViewBag.HeaderId = objGlobaldata.GetDropdownHeaderIdByDesc("Supplier payment term");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSupplier: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSuppModel);
        }

        [HttpPost]
        public JsonResult doesSupplierNameExist(string SupplierName)
        {
            var Valid = true;
            if (SupplierName != null)
            {
                SupplierModels objSuppModel = new SupplierModels();
                Valid = objSuppModel.checkSupplierNameExists(SupplierName);
            }

            return Json(Valid);
        }

        [HttpPost]
        public JsonResult doesSupplierCodeExist(string SupplierCode)
        {
            var Valid = true;
            if (SupplierCode != null)
            {
                SupplierModels objSuppModel = new SupplierModels();
                Valid = objSuppModel.checkSupplierCodeExists(SupplierCode);
            }

            return Json(Valid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplier(SupplierModels objSupplierModels, FormCollection form, IEnumerable<HttpPostedFileBase> SupportingDoc)
        {
            try
            {
                objSupplierModels.ApprovalCriteria = form["ApprovalCriteria"];
                objSupplierModels.Added_Updated_By = objGlobaldata.GetCurrentUserSession().empid;
                objSupplierModels.Supplier_type = form["Supplier_type"];
                objSupplierModels.Payment_term = form["Payment_term"];
                objSupplierModels.branch = form["branch"];
                objSupplierModels.Department = form["Department"];
                objSupplierModels.Location = form["Location"];
                objSupplierModels.notified_to = form["notified_to"];
                int Notificationval = 1;

                if (objSupplierModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objSupplierModels.NotificationValue = "0";
                }
                else if (objSupplierModels.NotificationPeriod == "Weeks" && int.TryParse(objSupplierModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objSupplierModels.NotificationPeriod == "Months" && int.TryParse(objSupplierModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objSupplierModels.NotificationDays = Notificationval;

                DateTime dateValue;

                if (DateTime.TryParse(form["License_Expires"], out dateValue) == true)
                {
                    objSupplierModels.License_Expires = dateValue;
                }

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    if (SupportingDoc != null && files.ContentLength > 0)
                    {
                        objSupplierModels.SupportingDoc = "";
                        foreach (var file in SupportingDoc)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = objSupplierModels.SupplierName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
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
            return RedirectToAction("SupplierList");
        }

        [HttpPost]
        public JsonResult GetSupplierList(string SupplierId)
        {
            List<string> objSupplierList = new List<string>();
            objSupplierList = objGlobaldata.GetSupplierNameList();

            List<string> lstFilteredList = objSupplierList.FindAll(d => d.StartsWith(SupplierId.ToUpper()));

            return Json(lstFilteredList);
        }

        [AllowAnonymous]
        public ActionResult SupplierList(string SearchText, string Approvestatus, int? page, string chkAll, string branch_name)
        {
            ViewBag.SubMenutype = "SupplierList";
            SupplierModelsList objSupplierModelsList = new SupplierModelsList();
            objSupplierModelsList.lstSupplier = new List<SupplierModels>();
            SupplierModels objSupp = new SupplierModels();
            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select SupplierId, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, ApprovedBy, Remarks, "
                    + " (case when ApprovedStatus='0' then 'Pending for Approval' when ApprovedStatus='1' then 'Approved' else 'Not Approved' end) as ApprovedStatus, Added_Updated_By, UpdatedOn,"
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type,License_Expiry,Criticality,Supplier_Work_Nature," +
                    "branch,Department,Location FROM t_supplier where Active=1 and chk_valid='Valid'";

                string sSearchtext = "";
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

                if (chkAll == null && chkAll != "All")
                {
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " and (SupplierName ='" + SearchText + "' or SupplierName like '" + SearchText + "%' or  SupplierName like '%" + SearchText + "' or SupplierName like '%" + SearchText + "%' or SupplierCode='" + SearchText
                            + "' or SupplierCode like '" + SearchText + "%' or SupplierCode like '%" + SearchText + "' or SupplierCode like '%" + SearchText + "%')";
                    }
                    if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                    {
                        ViewBag.ApprovestatusVal = Approvestatus;
                        sSearchtext = sSearchtext + " and (ApprovedStatus ='" + Approvestatus + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
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
                sSqlstmt = sSqlstmt + sSearchtext + " order by SupplierName asc";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        SupplierModels objSupplierModels = new SupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[i]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[i]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[i]["SupplyScope"].ToString(),
                            ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[i]["SupportingDoc"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            Remarks = dsSupplier.Tables[0].Rows[i]["Remarks"].ToString(),
                            ApprovedStatus = dsSupplier.Tables[0].Rows[i]["ApprovedStatus"].ToString(),
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
                            Supplier_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["Supplier_type"].ToString()),
                            Criticality = dsSupplier.Tables[0].Rows[i]["Criticality"].ToString(),
                            Supplier_Work_Nature = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["Supplier_Work_Nature"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["License_Expiry"].ToString(), out dateValue))
                        {
                            objSupplierModels.License_Expires = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["ApprovedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.ApprovedOn = dateValue;
                        }
                        objSupplierModelsList.lstSupplier.Add(objSupplierModels);
                    }
                    catch (Exception ex)
                    {
                        string ss = ex.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objSupplierModelsList.lstSupplier.ToList());
        }

        [AllowAnonymous]
        public JsonResult SupplierListSearch(string SearchText, string Approvestatus, int? page, string chkAll, string branch_name)
        {
            ViewBag.SubMenutype = "SupplierList";
            SupplierModelsList objSupplierModelsList = new SupplierModelsList();
            objSupplierModelsList.lstSupplier = new List<SupplierModels>();
            SupplierModels objSupp = new SupplierModels();
            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select SupplierId, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, ApprovedBy, Remarks, "
                    + " (case when ApprovedStatus='1' then 'Approved' else 'Not Approved' end) as ApprovedStatus, Added_Updated_By, UpdatedOn,"
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type,License_Expiry,Criticality,Supplier_Work_Nature,branch,Department,Location FROM t_supplier where Active=1";

                string sSearchtext = "";
                ViewBag.Approvestatus = objGlobaldata.GetConstantValueKeyValuePair("DocStatus");

                if (chkAll == null && chkAll != "All")
                {
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + " and (SupplierName ='" + SearchText + "' or SupplierName like '" + SearchText + "%' or  SupplierName like '%" + SearchText + "' or SupplierName like '%" + SearchText + "%' or SupplierCode='" + SearchText
                            + "' or SupplierCode like '" + SearchText + "%' or SupplierCode like '%" + SearchText + "' or SupplierCode like '%" + SearchText + "%')";
                    }
                    if (Approvestatus != null && Approvestatus != "All" && Approvestatus != "")
                    {
                        ViewBag.ApprovestatusVal = Approvestatus;
                        sSearchtext = sSearchtext + " and (ApprovedStatus ='" + Approvestatus + "')";
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
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
                sSqlstmt = sSqlstmt + sSearchtext + " order by SupplierName asc";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                for (int i = 0; dsSupplier.Tables.Count > 0 && i < dsSupplier.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        SupplierModels objSupplierModels = new SupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[i]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[i]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[i]["SupplyScope"].ToString(),
                            ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[i]["SupportingDoc"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[i]["ApprovedBy"].ToString()),
                            Remarks = dsSupplier.Tables[0].Rows[i]["Remarks"].ToString(),
                            ApprovedStatus = dsSupplier.Tables[0].Rows[i]["ApprovedStatus"].ToString(),
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
                            Supplier_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["Supplier_type"].ToString()),
                            Criticality = dsSupplier.Tables[0].Rows[i]["Criticality"].ToString(),
                            Supplier_Work_Nature = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["Supplier_Work_Nature"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[i]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[i]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[i]["Location"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["License_Expiry"].ToString(), out dateValue))
                        {
                            objSupplierModels.License_Expires = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["ApprovedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.ApprovedOn = dateValue;
                        }
                        objSupplierModelsList.lstSupplier.Add(objSupplierModels);
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in SupplierListSearch: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult SupplierApprove(string SupplierID, int iStatus)
        {
            try
            {
                SupplierModels objSupModels = new SupplierModels();

                string sStatus = "";
                if (iStatus == 1)
                {
                    sStatus = "Approved";
                }

                if (objSupModels.FunApproveSupplier(SupplierID, iStatus))
                {
                    TempData["Successdata"] = "Supplier" + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("ListPendingForApproval", "Dashboard");
        }

        [AllowAnonymous]
        public ActionResult SupplierDetails()
        {
            ViewBag.SubMenutype = "SupplierList";
            SupplierModels objSupplierModels = new SupplierModels();

            UserCredentials objUserInfo = (UserCredentials)Session["UserCredentials"];
            try
            {
                if (Request.QueryString["SupplierId"] != null && Request.QueryString["SupplierId"] != "")
                {
                    string sSupplierId = Request.QueryString["SupplierId"];

                    string sSqlstmt = "select SupplierId, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, ApprovedBy, Remarks,"
                    + " (case when ApprovedStatus = '0' then 'Pending for Approval' when ApprovedStatus = '1' then 'Approved' else 'Not Approved' end) as ApprovedStatus, Added_Updated_By, UpdatedOn, "
                    + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type,Payment_term,License_Expiry,NotificationDays,NotificationPeriod,NotificationValue,Criticality,Supplier_Work_Nature,branch,Department,Location,pan_no,notified_to FROM t_supplier where SupplierId='" + sSupplierId + "'";

                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        objSupplierModels = new SupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[0]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[0]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[0]["SupplyScope"].ToString(),
                            ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[0]["SupportingDoc"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Remarks = dsSupplier.Tables[0].Rows[0]["Remarks"].ToString(),
                            ApprovedStatus = dsSupplier.Tables[0].Rows[0]["ApprovedStatus"].ToString(),
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
                            NotificationPeriod = dsSupplier.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsSupplier.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Criticality = dsSupplier.Tables[0].Rows[0]["Criticality"].ToString(),
                            Supplier_Work_Nature = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Supplier_Work_Nature"].ToString()),
                            branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            pan_no = dsSupplier.Tables[0].Rows[0]["pan_no"].ToString(),
                            notified_to =objGlobaldata.GetMultiHrEmpNameById(dsSupplier.Tables[0].Rows[0]["notified_to"].ToString()),
                        };

                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["License_Expiry"].ToString(), out dateValue))
                        {
                            objSupplierModels.License_Expires = dateValue;
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
        public ActionResult SupplierInfo(int id)
        {
            ViewBag.SubMenutype = "SupplierList";
            SupplierModels objSupplierModels = new SupplierModels();
            try
            {
                string sSqlstmt = "select SupplierId, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, ApprovedBy, Remarks,"
                + " (case when ApprovedStatus = '0' then 'Pending for Approval' when ApprovedStatus = '1' then 'Approved' else 'Not Approved' end) as ApprovedStatus, Added_Updated_By, UpdatedOn, "
                + " City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,Supplier_type,Payment_term,License_Expiry,NotificationDays,NotificationPeriod,NotificationValue,Criticality,Supplier_Work_Nature,branch,Department,Location FROM t_supplier where SupplierId='" + id + "'";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    objSupplierModels = new SupplierModels
                    {
                        SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                        SupplierCode = dsSupplier.Tables[0].Rows[0]["SupplierCode"].ToString(),
                        SupplierName = dsSupplier.Tables[0].Rows[0]["SupplierName"].ToString(),
                        SupplyScope = dsSupplier.Tables[0].Rows[0]["SupplyScope"].ToString(),
                        ApprovalCriteria = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["ApprovalCriteria"].ToString()),
                        SupportingDoc = dsSupplier.Tables[0].Rows[0]["SupportingDoc"].ToString(),
                        ApprovedBy = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                        Remarks = dsSupplier.Tables[0].Rows[0]["Remarks"].ToString(),
                        ApprovedStatus = dsSupplier.Tables[0].Rows[0]["ApprovedStatus"].ToString(),
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
                        NotificationPeriod = dsSupplier.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                        NotificationValue = dsSupplier.Tables[0].Rows[0]["NotificationValue"].ToString(),
                        Criticality = dsSupplier.Tables[0].Rows[0]["Criticality"].ToString(),
                        Supplier_Work_Nature = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[0]["Supplier_Work_Nature"].ToString()),
                        branch = objGlobaldata.GetMultiCompanyBranchNameById(dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                        Department = objGlobaldata.GetMultiDeptNameById(dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                        Location = objGlobaldata.GetDivisionLocationById(dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                    };

                    DateTime dateValue;
                    if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["UpdatedOn"].ToString(), out dateValue))
                    {
                        objSupplierModels.UpdatedOn = dateValue;
                    }

                    if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["License_Expiry"].ToString(), out dateValue))
                    {
                        objSupplierModels.License_Expires = dateValue;
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
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SupplierDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SupplierList");
        }

        [AllowAnonymous]
        public ActionResult SupplierEdit()
        {
            ViewBag.SubMenutype = "SupplierList";
            ViewBag.paymentTerm = objGlobaldata.GetDropdownList("Supplier payment term");
            SupplierModels objSupplierModels = new SupplierModels();
            ViewBag.ApprovalCriteria = objGlobaldata.GetDropdownList("Supplier-ApprovalCriteria");
            ViewBag.NotificationPeriod = objGlobaldata.GetConstantValueKeyValuePair("NotificationPeriod");
            ViewBag.SuppCriticality = objGlobaldata.GetConstantValue("Criticality");
            ViewBag.SupplierTypes = objGlobaldata.GetDropdownList("Supplier Work Type");
            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            UserCredentials objUser = new UserCredentials();
            objUser = objGlobaldata.GetCurrentUserSession();
            try
            {
                ViewBag.SupplierType = objGlobaldata.GetDropdownList("Supplier Type");
                if (Request.QueryString["SupplierId"] != null && Request.QueryString["SupplierId"] != "")
                {
                    string sSupplierId = Request.QueryString["SupplierId"];

                    string sSqlstmt = "select SupplierId, SupplierCode, SupplierName, SupplyScope, ApprovalCriteria, SupportingDoc, ApprovedOn, ApprovedBy, Remarks,"
                        + " Added_Updated_By, ApprovedStatus, UpdatedOn , City,Country, ContactPerson, ContactNo, Address, FaxNo, PO_No,Email,VatNo,RefNo,"
                        + " Supplier_type,Payment_term,License_Expiry,NotificationDays,NotificationPeriod,NotificationValue,Criticality,Supplier_Work_Nature,branch,Department,Location,pan_no,notified_to FROM t_supplier where SupplierId='"
                        + sSupplierId + "'";
                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        //if (objUser.empid == dsSupplier.Tables[0].Rows[0]["Added_Updated_By"].ToString()
                        //    && dsSupplier.Tables[0].Rows[0]["ApprovedStatus"].ToString() == "0")
                        //{
                        objSupplierModels = new SupplierModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                            SupplierCode = dsSupplier.Tables[0].Rows[0]["SupplierCode"].ToString(),
                            SupplierName = dsSupplier.Tables[0].Rows[0]["SupplierName"].ToString(),
                            SupplyScope = dsSupplier.Tables[0].Rows[0]["SupplyScope"].ToString(),
                            ApprovalCriteria = (dsSupplier.Tables[0].Rows[0]["ApprovalCriteria"].ToString()),
                            SupportingDoc = dsSupplier.Tables[0].Rows[0]["SupportingDoc"].ToString(),
                            ApprovedBy = objGlobaldata.GetEmpHrNameById(dsSupplier.Tables[0].Rows[0]["ApprovedBy"].ToString()),
                            Remarks = dsSupplier.Tables[0].Rows[0]["Remarks"].ToString(),
                            ApprovedStatus = dsSupplier.Tables[0].Rows[0]["ApprovedStatus"].ToString(),
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
                            NotificationPeriod = dsSupplier.Tables[0].Rows[0]["NotificationPeriod"].ToString(),
                            NotificationValue = dsSupplier.Tables[0].Rows[0]["NotificationValue"].ToString(),
                            Criticality = dsSupplier.Tables[0].Rows[0]["Criticality"].ToString(),
                            Supplier_Work_Nature = (dsSupplier.Tables[0].Rows[0]["Supplier_Work_Nature"].ToString()),
                            branch = (dsSupplier.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsSupplier.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsSupplier.Tables[0].Rows[0]["Location"].ToString()),
                            pan_no = (dsSupplier.Tables[0].Rows[0]["pan_no"].ToString()),
                            notified_to = (dsSupplier.Tables[0].Rows[0]["notified_to"].ToString()),
                        };
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsSupplier.Tables[0].Rows[0]["branch"].ToString());
                        //  ViewBag.Approver = objGlobaldata.GetGRoleList(dsSupplier.Tables[0].Rows[0]["branch"].ToString(), dsSupplier.Tables[0].Rows[0]["Department"].ToString(), dsSupplier.Tables[0].Rows[0]["Location"].ToString(), "Approver");
                        ViewBag.Approver = objGlobaldata.GetApprover();
                        DateTime dateValue;
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["UpdatedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.UpdatedOn = dateValue;
                        }
                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["License_Expiry"].ToString(), out dateValue))
                        {
                            objSupplierModels.License_Expires = dateValue;
                        }

                        if (DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["ApprovedOn"].ToString(), out dateValue))
                        {
                            objSupplierModels.ApprovedOn = dateValue;
                        }

                        //ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList("");

                        return View(objSupplierModels);
                        //}
                        //else
                        //{
                        //    TempData["alertdata"] = "Access Denied";
                        //    return RedirectToAction("SupplierList");
                        //}
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
        public ActionResult SupplierEdit(SupplierModels objSupplierModels, FormCollection form, IEnumerable<HttpPostedFileBase> SupportingDoc)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                objSupplierModels.ApprovalCriteria = form["ApprovalCriteria"];
                objSupplierModels.Payment_term = form["Payment_term"];
                objSupplierModels.branch = form["branch"];
                objSupplierModels.Department = form["Department"];
                objSupplierModels.Location = form["Location"];
                objSupplierModels.notified_to = form["notified_to"];
                DateTime dateValue;
                if (DateTime.TryParse(form["License_Expires"], out dateValue) == true)
                {
                    objSupplierModels.License_Expires = dateValue;
                }

                int Notificationval = 1;

                if (objSupplierModels.NotificationPeriod == "None")
                {
                    Notificationval = 0;
                    objSupplierModels.NotificationValue = "0";
                }
                else if (objSupplierModels.NotificationPeriod == "Weeks" && int.TryParse(objSupplierModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 7 * Notificationval;
                }
                else if (objSupplierModels.NotificationPeriod == "Months" && int.TryParse(objSupplierModels.NotificationValue, out Notificationval))
                {
                    Notificationval = 30 * Notificationval;
                }
                objSupplierModels.NotificationDays = Notificationval;

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase files = Request.Files[0];
                    if (SupportingDoc != null && files.ContentLength > 0)
                    {
                        objSupplierModels.SupportingDoc = "";
                        foreach (var file in SupportingDoc)
                        {
                            try
                            {
                                string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Supplier"), Path.GetFileName(file.FileName));
                                string sFilename = objSupplierModels.SupplierName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                                file.SaveAs(sFilepath + "/" + sFilename);
                                objSupplierModels.SupportingDoc = objSupplierModels.SupportingDoc + "," + "~/DataUpload/MgmtDocs/Supplier/" + sFilename;
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
            return RedirectToAction("SupplierList");
        }

        [HttpPost]
        public JsonResult SupplierDelete(FormCollection form)
        {
            try
            {
                if (form["SupplierId"] != null && form["SupplierId"] != "")
                {
                    SupplierModels Doc = new SupplierModels();
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
        public ActionResult AddDiscrepancyLog()
        {
            try
            {
                ViewBag.SubMenutype = "DiscrepancyLog";

                ViewBag.SupplierList = objGlobaldata.GetSupplierList();
                ViewBag.External = objGlobaldata.GetDropdownList("Supplier External Provider Type");
                ViewBag.NC = objGlobaldata.GetDropdownList("Supplier NC Identified during");
                ViewBag.Type = objGlobaldata.GetDropdownList("Supplier Type of Discrepancies");
                ViewBag.Status = objGlobaldata.GetDropdownList("Supplier Discrepancies Status");
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDiscrepancyLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDiscrepancyLog(SupplierDiscrepencyLogModels objDiscrepencyLogModels, FormCollection form, HttpPostedFileBase Upload)
        {
            try
            {
                objDiscrepencyLogModels.SupplierId = form["SupplierId"];
                DateTime dateValue;

                if (DateTime.TryParse(form["Discrepency_LoggedDate"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.Discrepency_LoggedDate = dateValue;
                }

                if (Upload != null && Upload.ContentLength > 0)
                {
                    try
                    {
                        string sVirtualPath = "~/DataUpload/MgmtDocs/Supplier";
                        string spath = Path.Combine(Server.MapPath(sVirtualPath), Path.GetFileName(Upload.FileName));
                        string sFilename = "SDiscrepancy" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetExtension(Upload.FileName);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload.SaveAs(sFilepath + "/" + sFilename);
                        objDiscrepencyLogModels.Upload = sVirtualPath + "/" + sFilename;
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

                if (objDiscrepencyLogModels.FunAddSupplierDiscrepencyLog(objDiscrepencyLogModels))
                {
                    TempData["Successdata"] = "Added Supplier Discrepency Log details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddDiscrepancyLog: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList");
        }

        [AllowAnonymous]
        public JsonResult DiscrepancyLogDelete(FormCollection form)
        {
            try
            {
                if (form["SupplierDiscreLogId"] != null && form["SupplierDiscreLogId"] != "")
                {
                    SupplierDiscrepencyLogModels Doc = new SupplierDiscrepencyLogModels();
                    string sSupplierDiscreLogId = form["SupplierDiscreLogId"];

                    if (Doc.FunDeleteDiscrepencyLogDoc(sSupplierDiscreLogId))
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
                    TempData["alertdata"] = "Discrepency Log Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogList(string fromDate, string ToDate, string chkAll, string SupplierId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";

            DescripencyLogModelsList objDescripencyLogModelsList = new DescripencyLogModelsList();
            objDescripencyLogModelsList.lstDescripencyLog = new List<SupplierDiscrepencyLogModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select SupplierDiscreLogId, SupplierId, Discrepency_LoggedDate, PONo, Discrepency_Desc, ActionTaken,Upload,po_date,providertype,nc_identified,discrepancy_type,ncr_ref,supplier_ref,impact,disc_status,remarks from t_supplier_discrepancylog where Active=1";

                SupplierModels objSupplier = new SupplierModels();

                ViewBag.SupplierList = objGlobaldata.GetSupplierList();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (SupplierId != null && SupplierId != "Select")
                    {
                        ViewBag.SupplierId = SupplierId;
                        sSearchtext = sSearchtext + " (SupplierId ='" + SupplierId + "')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (fromDate != null && DateTime.TryParse(fromDate, out dtFromdate) && DateTime.TryParse(ToDate, out dtToDate))
                    {
                        ViewBag.fromdateval = fromDate;
                        ViewBag.todateval = ToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and Discrepency_LoggedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '"
                                + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and Discrepency_LoggedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '"
                                + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Discrepency_LoggedDate desc";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SupplierDiscrepencyLogModels objSupplierModels = new SupplierDiscrepencyLogModels
                            {
                                SupplierId = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString()),
                                SupplierDiscreLogId = dsSupplier.Tables[0].Rows[i]["SupplierDiscreLogId"].ToString(),
                                Discrepency_LoggedDate = Convert.ToDateTime(dsSupplier.Tables[0].Rows[i]["Discrepency_LoggedDate"].ToString()),
                                PONo = dsSupplier.Tables[0].Rows[i]["PONo"].ToString(),
                                Discrepency_Desc = dsSupplier.Tables[0].Rows[i]["Discrepency_Desc"].ToString(),
                                ActionTaken = dsSupplier.Tables[0].Rows[i]["ActionTaken"].ToString(),
                                Upload = dsSupplier.Tables[0].Rows[i]["Upload"].ToString(),
                                providertype = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["providertype"].ToString()),
                                nc_identified = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["nc_identified"].ToString()),
                                discrepancy_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["discrepancy_type"].ToString()),
                                ncr_ref = dsSupplier.Tables[0].Rows[i]["ncr_ref"].ToString(),
                                supplier_ref = dsSupplier.Tables[0].Rows[i]["supplier_ref"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[i]["impact"].ToString(),
                                disc_status = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["disc_status"].ToString()),
                                remarks = dsSupplier.Tables[0].Rows[i]["remarks"].ToString(),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[i]["po_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["po_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.po_date = dtDocDate;
                            }
                            objDescripencyLogModelsList.lstDescripencyLog.Add(objSupplierModels);
                        }
                        catch (Exception ex)
                        { }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objDescripencyLogModelsList.lstDescripencyLog.ToList());
        }

        [AllowAnonymous]
        public JsonResult DiscrepancyLogListSearch(string fromDate, string ToDate, string chkAll, string SupplierId, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";

            DescripencyLogModelsList objDescripencyLogModelsList = new DescripencyLogModelsList();
            objDescripencyLogModelsList.lstDescripencyLog = new List<SupplierDiscrepencyLogModels>();

            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select SupplierDiscreLogId, SupplierId, Discrepency_LoggedDate, PONo, Discrepency_Desc, ActionTaken,Upload,po_date,providertype,nc_identified,discrepancy_type,ncr_ref,supplier_ref,impact,disc_status,remarks from t_supplier_discrepancylog where Active=1";

                SupplierModels objSupplier = new SupplierModels();

                ViewBag.SupplierList = objGlobaldata.GetSupplierList();
                string sSearchtext = "";
                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    DateTime dtFromdate, dtToDate;
                    if (SupplierId != null && SupplierId != "Select")
                    {
                        ViewBag.SupplierId = SupplierId;
                        sSearchtext = sSearchtext + " (SupplierId ='" + SupplierId + "')";
                    }

                    if (sSearchtext != "")
                    {
                        sSearchtext = " and " + sSearchtext;
                    }
                    if (fromDate != null && DateTime.TryParse(fromDate, out dtFromdate) && DateTime.TryParse(ToDate, out dtToDate))
                    {
                        ViewBag.fromdateval = fromDate;
                        ViewBag.todateval = ToDate;
                        if (sSearchtext != "")
                        {
                            sSearchtext = sSearchtext + " and Discrepency_LoggedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '"
                                + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                        else
                        {
                            sSearchtext = sSearchtext + " and Discrepency_LoggedDate between '" + dtFromdate.ToString("yyyy/MM/dd") + "' and '"
                                + dtToDate.ToString("yyyy/MM/dd") + "'";
                        }
                    }
                }
                else
                {
                    ViewBag.chkAll = true;
                }
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }
                sSqlstmt = sSqlstmt + sSearchtext + " order by Discrepency_LoggedDate desc";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            SupplierDiscrepencyLogModels objSupplierModels = new SupplierDiscrepencyLogModels
                            {
                                SupplierId = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString()),
                                SupplierDiscreLogId = dsSupplier.Tables[0].Rows[i]["SupplierDiscreLogId"].ToString(),
                                Discrepency_LoggedDate = Convert.ToDateTime(dsSupplier.Tables[0].Rows[i]["Discrepency_LoggedDate"].ToString()),
                                PONo = dsSupplier.Tables[0].Rows[i]["PONo"].ToString(),
                                Discrepency_Desc = dsSupplier.Tables[0].Rows[i]["Discrepency_Desc"].ToString(),
                                ActionTaken = dsSupplier.Tables[0].Rows[i]["ActionTaken"].ToString(),
                                Upload = dsSupplier.Tables[0].Rows[i]["Upload"].ToString(),
                                providertype = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["providertype"].ToString()),
                                nc_identified = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["nc_identified"].ToString()),
                                discrepancy_type = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["discrepancy_type"].ToString()),
                                ncr_ref = dsSupplier.Tables[0].Rows[i]["ncr_ref"].ToString(),
                                supplier_ref = dsSupplier.Tables[0].Rows[i]["supplier_ref"].ToString(),
                                impact = dsSupplier.Tables[0].Rows[i]["impact"].ToString(),
                                disc_status = objGlobaldata.GetDropdownitemById(dsSupplier.Tables[0].Rows[i]["disc_status"].ToString()),
                                remarks = dsSupplier.Tables[0].Rows[i]["remarks"].ToString(),
                            };
                            DateTime dtDocDate;

                            if (dsSupplier.Tables[0].Rows[i]["po_date"].ToString() != ""
                               && DateTime.TryParse(dsSupplier.Tables[0].Rows[i]["po_date"].ToString(), out dtDocDate))
                            {
                                objSupplierModels.po_date = dtDocDate;
                            }
                            objDescripencyLogModelsList.lstDescripencyLog.Add(objSupplierModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogInfo(int id)
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            SupplierDiscrepencyLogModels objSupplierModels = new SupplierDiscrepencyLogModels();
            try
            {
                string sSqlstmt = "select SupplierDiscreLogId, SupplierId, Discrepency_LoggedDate, PONo, Discrepency_Desc, ActionTaken,Upload from t_supplier_discrepancylog where Active=1";

                DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsSupplier.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            objSupplierModels = new SupplierDiscrepencyLogModels
                            {
                                SupplierId = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[i]["SupplierId"].ToString()),
                                SupplierDiscreLogId = dsSupplier.Tables[0].Rows[i]["SupplierDiscreLogId"].ToString(),
                                Discrepency_LoggedDate = Convert.ToDateTime(dsSupplier.Tables[0].Rows[i]["Discrepency_LoggedDate"].ToString()),
                                PONo = dsSupplier.Tables[0].Rows[i]["PONo"].ToString(),
                                Discrepency_Desc = dsSupplier.Tables[0].Rows[i]["Discrepency_Desc"].ToString(),
                                ActionTaken = dsSupplier.Tables[0].Rows[i]["ActionTaken"].ToString(),
                                Upload = dsSupplier.Tables[0].Rows[i]["Upload"].ToString()
                            };
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objSupplierModels);
        }

        [AllowAnonymous]
        public ActionResult DiscrepancyLogEdit()
        {
            ViewBag.SubMenutype = "DiscrepancyLog";
            SupplierDiscrepencyLogModels objSupplierModels = new SupplierDiscrepencyLogModels();
            SupplierModels objSupplier = new SupplierModels();
            try
            {
                if (Request.QueryString["SupplierDiscreLogId"] != null && Request.QueryString["SupplierDiscreLogId"] != "")
                {
                    string sSupplierDiscreLogId = Request.QueryString["SupplierDiscreLogId"];
                    string sSqlstmt = "select SupplierDiscreLogId, SupplierId, Discrepency_LoggedDate, PONo, Discrepency_Desc, ActionTaken, Upload,providertype,po_date,nc_identified,discrepancy_type,ncr_ref,supplier_ref,impact,disc_status,remarks from t_supplier_discrepancylog"
                          + " where SupplierDiscreLogId='" + sSupplierDiscreLogId + "'";

                    ViewBag.SupplierList = objGlobaldata.GetSupplierList();
                    ViewBag.External = objGlobaldata.GetDropdownList("Supplier External Provider Type");
                    ViewBag.NC = objGlobaldata.GetDropdownList("Supplier NC Identified during");
                    ViewBag.Type = objGlobaldata.GetDropdownList("Supplier Type of Discrepancies");
                    ViewBag.Status = objGlobaldata.GetDropdownList("Supplier Discrepancies Status");
                    ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                    DataSet dsSupplier = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsSupplier.Tables.Count > 0 && dsSupplier.Tables[0].Rows.Count > 0)
                    {
                        objSupplierModels = new SupplierDiscrepencyLogModels
                        {
                            SupplierId = dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString(),
                            SupplierDiscreLogId = dsSupplier.Tables[0].Rows[0]["SupplierDiscreLogId"].ToString(),
                            Discrepency_LoggedDate = Convert.ToDateTime(dsSupplier.Tables[0].Rows[0]["Discrepency_LoggedDate"].ToString()),
                            PONo = dsSupplier.Tables[0].Rows[0]["PONo"].ToString(),
                            Discrepency_Desc = dsSupplier.Tables[0].Rows[0]["Discrepency_Desc"].ToString(),
                            ActionTaken = dsSupplier.Tables[0].Rows[0]["ActionTaken"].ToString(),
                            Upload = dsSupplier.Tables[0].Rows[0]["Upload"].ToString(),
                            providertype = dsSupplier.Tables[0].Rows[0]["providertype"].ToString(),
                            nc_identified = dsSupplier.Tables[0].Rows[0]["nc_identified"].ToString(),
                            discrepancy_type = dsSupplier.Tables[0].Rows[0]["discrepancy_type"].ToString(),
                            ncr_ref = dsSupplier.Tables[0].Rows[0]["ncr_ref"].ToString(),
                            supplier_ref = dsSupplier.Tables[0].Rows[0]["supplier_ref"].ToString(),
                            impact = dsSupplier.Tables[0].Rows[0]["impact"].ToString(),
                            disc_status = dsSupplier.Tables[0].Rows[0]["disc_status"].ToString(),
                            remarks = dsSupplier.Tables[0].Rows[0]["remarks"].ToString(),
                        };
                        DateTime dtDocDate;

                        if (dsSupplier.Tables[0].Rows[0]["po_date"].ToString() != ""
                           && DateTime.TryParse(dsSupplier.Tables[0].Rows[0]["po_date"].ToString(), out dtDocDate))
                        {
                            objSupplierModels.po_date = dtDocDate;
                        }
                        ViewBag.SupplierName = objGlobaldata.GetSupplierNameById(dsSupplier.Tables[0].Rows[0]["SupplierId"].ToString());
                        return View(objSupplierModels);
                    }
                    else
                    {
                        TempData["alertdata"] = "No data exists";
                        return RedirectToAction("DiscrepancyLogList");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Discrepancy Id cannot be null";
                    return RedirectToAction("DiscrepancyLogList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList"); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DiscrepancyLogEdit(SupplierDiscrepencyLogModels objDiscrepencyLogModels, FormCollection form, HttpPostedFileBase Upload)
        {
            try
            {
                objDiscrepencyLogModels.SupplierId = form["SupplierId"];
                DateTime dateValue;

                if (DateTime.TryParse(form["Discrepency_LoggedDate"], out dateValue) == true)
                {
                    objDiscrepencyLogModels.Discrepency_LoggedDate = dateValue;
                }

                if (Upload != null && Upload.ContentLength > 0)
                {
                    try
                    {
                        string sVirtualPath = "~/DataUpload/MgmtDocs/Supplier";
                        string spath = Path.Combine(Server.MapPath(sVirtualPath), Path.GetFileName(Upload.FileName));
                        string sFilename = "SDiscrepancy" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetExtension(Upload.FileName);
                        string sFilepath = Path.GetDirectoryName(spath);

                        Upload.SaveAs(sFilepath + "/" + sFilename);
                        objDiscrepencyLogModels.Upload = sVirtualPath + "/" + sFilename;
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

                if (objDiscrepencyLogModels.FunUpdateSupplierDiscrepencyLog(objDiscrepencyLogModels))
                {
                    TempData["Successdata"] = "Supplier Discrepency Log details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in DiscrepancyLogEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("DiscrepancyLogList");
        }

        public JsonResult SupplierApproveNoty(string SupplierID, int iStatus)
        {
            try
            {
                SupplierModels objSupModels = new SupplierModels();

                string sStatus = "";
                if (iStatus == 1)
                {
                    sStatus = "Approved";
                }
                if (iStatus == 2)
                {
                    sStatus = "Rejected";
                }
                if (objSupModels.FunApproveSupplier(SupplierID, iStatus))
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
                objGlobaldata.AddFunctionalLog("Exception in SupplierApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public JsonResult SuppliertInvalid(string SupplierID, string invalid_reason)
        {
            try
            {
                if (SupplierID != "" && SupplierID != "")
                {
                    SupplierModels Doc = new SupplierModels();
                    string sSupplierID = SupplierID;
                    if (Doc.FunInvalidSupplier(sSupplierID, invalid_reason))
                    {
                        TempData["Successdata"] = "Updated successfully";
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
                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SuppliertInvalid: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
    }
}