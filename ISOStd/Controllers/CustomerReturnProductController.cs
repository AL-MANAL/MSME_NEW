using ISOStd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;

namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CustomerReturnProductController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public CustomerReturnProductController()
        {
            ViewBag.Menutype = "CustomerMgmt";
            ViewBag.SubMenutype = "CustomerReturnProduct";
        }
        public ActionResult AddCustReturnProduct()
        {
            try
            {
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                ViewBag.ProductStatus = objGlobaldata.GetDropdownList("Customer Property Status");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustReturnProduct: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddCustReturnProduct(CustomerReturnProductModels objProp, FormCollection form,IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                DateTime dateValue;
                if (DateTime.TryParse(form["return_date"], out dateValue) == true)
                {
                    objProp.return_date = dateValue;
                }
                if (DateTime.TryParse(form["retun_deliverydate"], out dateValue) == true)
                {
                    objProp.retun_deliverydate = dateValue;
                }
                if (upload != null && files.ContentLength > 0)
                {
                    objProp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(file.FileName));
                            string sFilename = "CustomerReturn" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objProp.upload = objProp.upload + "," + "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddCustReturnProduct-upload: " + ex.ToString());
                        }
                    }
                    objProp.upload = objProp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objProp.FunAddCustReturnProduct(objProp,upload))
                {
                    TempData["Successdata"] = "Customer Return Product Added successfully with Reference No : '"+ objProp.refno+ "'";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCustReturnProduct: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustReturnProductList");
        }

        [AllowAnonymous]
        public ActionResult CustReturnProductList(FormCollection form, int? page, string branch_name)
        {
            CustomerReturnProductModelsList objPropList = new CustomerReturnProductModelsList();
            objPropList.PropList = new List<CustomerReturnProductModels>();
            try
            {

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_return_product,return_date,retun_deliverydate,a.CustID,refno,retun_deliverynote,product_details," +
                    "reason,action_taken,remarks,Ncn_ref,product_status,loggedby,resp_person,b.branch "
                    + " from t_cust_return_product a,t_customer_info b where a.active=1 and a.CustID=b.CustID";
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', branch)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by return_date desc";

                DataSet dsPropList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPropList.Tables.Count > 0 && dsPropList.Tables[0].Rows.Count > 0)
                {

                   
                    for (int i = 0; i < dsPropList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CustomerReturnProductModels objCustModels = new CustomerReturnProductModels
                            {
                                id_return_product = dsPropList.Tables[0].Rows[i]["id_return_product"].ToString(),
                                CustID =objGlobaldata.GetCustomerNameById(dsPropList.Tables[0].Rows[i]["CustID"].ToString()),
                                refno = dsPropList.Tables[0].Rows[i]["refno"].ToString(),
                                retun_deliverynote = dsPropList.Tables[0].Rows[i]["retun_deliverynote"].ToString(),
                                product_details = dsPropList.Tables[0].Rows[i]["product_details"].ToString(),
                                reason = dsPropList.Tables[0].Rows[i]["reason"].ToString(),                               
                                action_taken = dsPropList.Tables[0].Rows[i]["action_taken"].ToString(),
                                remarks = dsPropList.Tables[0].Rows[i]["remarks"].ToString(),
                                Ncn_ref = dsPropList.Tables[0].Rows[i]["Ncn_ref"].ToString(),
                                product_status = objGlobaldata.GetDropdownitemById(dsPropList.Tables[0].Rows[i]["product_status"].ToString()),
                                loggedby = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[i]["loggedby"].ToString()),
                                resp_person = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[i]["resp_person"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPropList.Tables[0].Rows[i]["return_date"].ToString() != ""
                               && DateTime.TryParse(dsPropList.Tables[0].Rows[i]["return_date"].ToString(), out dtDocDate))
                            {
                                objCustModels.return_date = dtDocDate;
                            }
                            if (dsPropList.Tables[0].Rows[i]["retun_deliverydate"].ToString() != ""
                               && DateTime.TryParse(dsPropList.Tables[0].Rows[i]["retun_deliverydate"].ToString(), out dtDocDate))
                            {
                                objCustModels.retun_deliverydate = dtDocDate;
                            }
                            objPropList.PropList.Add(objCustModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustReturnProductList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objPropList.PropList.ToList());
        }

        [AllowAnonymous]
        public JsonResult CustReturnProductListSearch(FormCollection form, int? page, string branch_name)
        {
            CustomerReturnProductModelsList objPropList = new CustomerReturnProductModelsList();
            objPropList.PropList = new List<CustomerReturnProductModels>();
            try
            {

                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                string sSqlstmt = "select id_return_product,return_date,retun_deliverydate,a.CustID,refno,retun_deliverynote,product_details," +
                     "reason,action_taken,remarks,Ncn_ref,product_status,loggedby,resp_person,b.branch "
                     + " from t_cust_return_product a,t_customer_info b where a.active=1 and a.CustID=b.CustID";
                string sSearchtext = "";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and branch='" + branch_name + "' ";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and branch='" + sBranch_name + "' ";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by return_date desc";

                DataSet dsPropList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPropList.Tables.Count > 0 && dsPropList.Tables[0].Rows.Count > 0)
                {

                    for (int i = 0; i < dsPropList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CustomerReturnProductModels objCustModels = new CustomerReturnProductModels
                            {
                                id_return_product = dsPropList.Tables[0].Rows[i]["id_return_product"].ToString(),
                                CustID = objGlobaldata.GetCustomerNameById(dsPropList.Tables[0].Rows[i]["CustID"].ToString()),
                                refno = dsPropList.Tables[0].Rows[i]["refno"].ToString(),
                                retun_deliverynote = dsPropList.Tables[0].Rows[i]["retun_deliverynote"].ToString(),
                                product_details = dsPropList.Tables[0].Rows[i]["product_details"].ToString(),
                                reason = dsPropList.Tables[0].Rows[i]["reason"].ToString(),
                                action_taken = dsPropList.Tables[0].Rows[i]["action_taken"].ToString(),
                                remarks = dsPropList.Tables[0].Rows[i]["remarks"].ToString(),
                                Ncn_ref = dsPropList.Tables[0].Rows[i]["Ncn_ref"].ToString(),
                                product_status = objGlobaldata.GetDropdownitemById(dsPropList.Tables[0].Rows[i]["product_status"].ToString()),
                                loggedby = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[i]["loggedby"].ToString()),
                                resp_person = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[i]["resp_person"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsPropList.Tables[0].Rows[i]["return_date"].ToString() != ""
                               && DateTime.TryParse(dsPropList.Tables[0].Rows[i]["return_date"].ToString(), out dtDocDate))
                            {
                                objCustModels.return_date = dtDocDate;
                            }
                            if (dsPropList.Tables[0].Rows[i]["retun_deliverydate"].ToString() != ""
                               && DateTime.TryParse(dsPropList.Tables[0].Rows[i]["retun_deliverydate"].ToString(), out dtDocDate))
                            {
                                objCustModels.retun_deliverydate = dtDocDate;
                            }
                            objPropList.PropList.Add(objCustModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustReturnProductListSearch: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductListSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json("Success");
        }
        
        [AllowAnonymous]
        public ActionResult CustReturnProductEdit()
        {
            CustomerReturnProductModels objCust = new CustomerReturnProductModels();
            try
            {
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                ViewBag.ProductStatus = objGlobaldata.GetDropdownList("Customer Property Status");
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();

                if (Request.QueryString["id_return_product"] != null && Request.QueryString["id_return_product"] != "")
                {
                    string id_return_product = Request.QueryString["id_return_product"];
                    string sSqlstmt = "select id_return_product,return_date,retun_deliverydate,CustID,refno,retun_deliverynote,product_details,reason,action_taken,remarks,Ncn_ref,product_status,loggedby,resp_person,upload"
                    + " from t_cust_return_product where id_return_product='" + id_return_product + "'";
                    DataSet dsPropList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPropList.Tables.Count > 0 && dsPropList.Tables[0].Rows.Count > 0)
                    {
                        objCust = new CustomerReturnProductModels
                        {
                            id_return_product = dsPropList.Tables[0].Rows[0]["id_return_product"].ToString(),
                            CustID = /*objGlobaldata.GetCustomerNameById*/(dsPropList.Tables[0].Rows[0]["CustID"].ToString()),
                            refno = dsPropList.Tables[0].Rows[0]["refno"].ToString(),
                            retun_deliverynote = dsPropList.Tables[0].Rows[0]["retun_deliverynote"].ToString(),
                            product_details = dsPropList.Tables[0].Rows[0]["product_details"].ToString(),
                            reason = dsPropList.Tables[0].Rows[0]["reason"].ToString(),
                            action_taken = dsPropList.Tables[0].Rows[0]["action_taken"].ToString(),
                            remarks = dsPropList.Tables[0].Rows[0]["remarks"].ToString(),
                            Ncn_ref = dsPropList.Tables[0].Rows[0]["Ncn_ref"].ToString(),
                            upload = dsPropList.Tables[0].Rows[0]["upload"].ToString(),
                            product_status = (dsPropList.Tables[0].Rows[0]["product_status"].ToString()),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[0]["loggedby"].ToString()),
                            resp_person = /*objGlobaldata.GetEmpHrNameById*/(dsPropList.Tables[0].Rows[0]["resp_person"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsPropList.Tables[0].Rows[0]["return_date"].ToString() != ""
                           && DateTime.TryParse(dsPropList.Tables[0].Rows[0]["return_date"].ToString(), out dtDocDate))
                        {
                            objCust.return_date = dtDocDate;
                        }
                        if (dsPropList.Tables[0].Rows[0]["retun_deliverydate"].ToString() != ""
                           && DateTime.TryParse(dsPropList.Tables[0].Rows[0]["retun_deliverydate"].ToString(), out dtDocDate))
                        {
                            objCust.retun_deliverydate = dtDocDate;
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustReturnProductList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustReturnProductList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustReturnProductList");
            }
            return View(objCust);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CustReturnProductEdit(CustomerReturnProductModels objProp, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                HttpPostedFileBase files = Request.Files[0];
                string QCDelete = Request.Form["QCDocsValselectall"];
                DateTime dateValue;
                if (DateTime.TryParse(form["return_date"], out dateValue) == true)
                {
                    objProp.return_date = dateValue;
                }
                if (DateTime.TryParse(form["retun_deliverydate"], out dateValue) == true)
                {
                    objProp.retun_deliverydate = dateValue;
                }
                if (upload != null && files.ContentLength > 0)
                {
                    objProp.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Customer"), Path.GetFileName(file.FileName));
                            string sFilename = "CustomerReturn" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objProp.upload = objProp.upload + "," + "~/DataUpload/MgmtDocs/Customer/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in CustReturnProductEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objProp.upload = objProp.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objProp.upload = objProp.upload + "," + form["QCDocsVal"];
                    objProp.upload = objProp.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objProp.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objProp.upload = null;
                }
                if (objProp.FunUpdateCustReturnProduct(objProp))
                {
                    TempData["Successdata"] = "Updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("CustReturnProductList");
        }

        [AllowAnonymous]
        public ActionResult CustReturnProductDetails()
        {
            CustomerReturnProductModels objCust = new CustomerReturnProductModels();
            try
            {
                ViewBag.Customer = objGlobaldata.GetCustomerListbox();
                ViewBag.Status = objGlobaldata.GetDropdownList("Customer Property Status");
                if (Request.QueryString["id_return_product"] != null && Request.QueryString["id_return_product"] != "")
                {
                    string id_return_product = Request.QueryString["id_return_product"];
                    string sSqlstmt = "select id_return_product,return_date,retun_deliverydate,CustID,refno,retun_deliverynote,product_details,reason,action_taken,remarks,Ncn_ref,product_status,loggedby,resp_person"
                    + " from t_cust_return_product where id_return_product='" + id_return_product + "'";
                    DataSet dsPropList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsPropList.Tables.Count > 0 && dsPropList.Tables[0].Rows.Count > 0)
                    {
                        objCust = new CustomerReturnProductModels
                        {
                            id_return_product = dsPropList.Tables[0].Rows[0]["id_return_product"].ToString(),
                            CustID = objGlobaldata.GetCustomerNameById(dsPropList.Tables[0].Rows[0]["CustID"].ToString()),
                            refno = dsPropList.Tables[0].Rows[0]["refno"].ToString(),
                            retun_deliverynote = dsPropList.Tables[0].Rows[0]["retun_deliverynote"].ToString(),
                            product_details = dsPropList.Tables[0].Rows[0]["product_details"].ToString(),
                            reason = dsPropList.Tables[0].Rows[0]["reason"].ToString(),
                            action_taken = dsPropList.Tables[0].Rows[0]["action_taken"].ToString(),
                            remarks = dsPropList.Tables[0].Rows[0]["remarks"].ToString(),
                            Ncn_ref = dsPropList.Tables[0].Rows[0]["Ncn_ref"].ToString(),
                            product_status = objGlobaldata.GetDropdownitemById(dsPropList.Tables[0].Rows[0]["product_status"].ToString()),
                            loggedby = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[0]["loggedby"].ToString()),
                            resp_person = objGlobaldata.GetEmpHrNameById(dsPropList.Tables[0].Rows[0]["resp_person"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsPropList.Tables[0].Rows[0]["return_date"].ToString() != ""
                           && DateTime.TryParse(dsPropList.Tables[0].Rows[0]["return_date"].ToString(), out dtDocDate))
                        {
                            objCust.return_date = dtDocDate;
                        }
                        if (dsPropList.Tables[0].Rows[0]["retun_deliverydate"].ToString() != ""
                           && DateTime.TryParse(dsPropList.Tables[0].Rows[0]["retun_deliverydate"].ToString(), out dtDocDate))
                        {
                            objCust.retun_deliverydate = dtDocDate;
                        }
                        string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + dsPropList.Tables[0].Rows[0]["CustID"].ToString() + "'";
                        DataSet CustList = objGlobaldata.Getdetails(sql);
                        if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
                        {
                            objCust.branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString());
                            objCust.Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString());
                            objCust.Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString());
                        }
                    }
                    else
                    {

                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return RedirectToAction("CustReturnProductList");
                    }
                }
                else
                {

                    TempData["alertdata"] = "Id cannot be Null or empty";
                    return RedirectToAction("CustReturnProductList");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("CustReturnProductList");
            }
            return View(objCust);
        }

        [AllowAnonymous]
        public JsonResult CustReturnProductDelete(FormCollection form)
        {
            try
            {

                 if (form["id_return_product"] != null && form["id_return_product"] != "")
                    {
                        CustomerReturnProductModels Doc = new CustomerReturnProductModels();
                        string sid_return_product = form["id_return_product"];
                        if (Doc.FunDelCustReturnProduct(sid_return_product))
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
                        TempData["alertdata"] = "Id cannot be Null or empty";
                        return Json("Failed");
                    }                             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CustReturnProductDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult GetCustomerDetails(string CustId)
        {
            CustComplaintModels objCust = new CustComplaintModels();
            string sql = "Select branch,Department,Location from t_customer_info where CustID = '" + CustId + "'";
            DataSet CustList = objGlobaldata.Getdetails(sql);
            if (CustList.Tables.Count > 0 && CustList.Tables[0].Rows.Count > 0)
            {
                objCust = new CustComplaintModels
                {
                    branch = objGlobaldata.GetMultiCompanyBranchNameById(CustList.Tables[0].Rows[0]["branch"].ToString()),
                    Department = objGlobaldata.GetMultiDeptNameById(CustList.Tables[0].Rows[0]["Department"].ToString()),
                    Location = objGlobaldata.GetDivisionLocationById(CustList.Tables[0].Rows[0]["Location"].ToString()),
                };
            }
            return Json(objCust);
        }

    }
}