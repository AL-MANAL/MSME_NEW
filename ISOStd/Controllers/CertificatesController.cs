using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISOStd.Models;
using System.Data;
using System.IO;
using PagedList;
using PagedList.Mvc;
using ISOStd.Filters;


namespace ISOStd.Controllers
{
    [PreventFromUrl]
    public class CertificatesController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();
        public CertificatesController()
        {
            ViewBag.Menutype = "LegalRegister";
        }
        // GET: Certificates

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Certificate(string chkAll, string SearchText, int? page, string branch_name)
        {
            CertificatesModelsList objCertModelList = new CertificatesModelsList();
            objCertModelList.CertificateList = new List<CertificatesModels>();

            try
            {
                ViewBag.sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.sDepartment = objGlobaldata.GetCurrentUserSession().DeptID;
                ViewBag.sLocation = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.DepartmentList = objGlobaldata.GetDepartmentListbox(ViewBag.sBranch_name);
                ViewBag.LocationList = objGlobaldata.GetDivisionLocationList(ViewBag.sBranch_name);

                string sSqlstmt = "select certId,certName,DocUploadPath,expiry_date,Department,Location,branch from t_certificates where Status=1";
                string sSearchtext = "";
                //string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + "  where (certName ='" + SearchText + "' or certName like '" + SearchText + "%' or certName like '%" + SearchText + "%'"
                        + " or certName like '%" + SearchText + "')";
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
                    sSearchtext = sSearchtext + " and find_in_set('" + ViewBag.sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by certName asc";
                DataSet dsCertList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCertList.Tables.Count > 0 && dsCertList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCertList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CertificatesModels objCertificate = new CertificatesModels
                            {
                                certId = (dsCertList.Tables[0].Rows[i]["certId"].ToString()),
                                certName = dsCertList.Tables[0].Rows[i]["certName"].ToString(),
                                DocUploadPath = dsCertList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCertList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCertList.Tables[0].Rows[i]["Location"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById( dsCertList.Tables[0].Rows[i]["branch"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsCertList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                               && DateTime.TryParse(dsCertList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objCertificate.expiry_date = dtDocDate;
                            }
                            objCertModelList.CertificateList.Add(objCertificate);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in Certificate: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in Certificate: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return View(objCertModelList.CertificateList.ToList());
        }


        [AllowAnonymous]
        public JsonResult CertificateSearch(string chkAll, string SearchText, int? page, string branch_name)
        {
            CertificatesModelsList objCertModelList = new CertificatesModelsList();
            objCertModelList.CertificateList = new List<CertificatesModels>();

            try
            {
                ViewBag.sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                ViewBag.sDepartment = objGlobaldata.GetCurrentUserSession().DeptID;
                ViewBag.sLocation = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.BranchList = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.DepartmentList = objGlobaldata.GetDepartmentListbox(ViewBag.sBranch_name);
                ViewBag.LocationList = objGlobaldata.GetDivisionLocationList(ViewBag.sBranch_name);

                string sSqlstmt = "select certId,certName,DocUploadPath,expiry_date,Department,Location,branch from t_certificates where Status=1";
                string sSearchtext = "";
                //string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);

                if (chkAll == null && chkAll != "All")
                {
                    ViewBag.chkAll = false;
                    if (SearchText != null && SearchText != "")
                    {
                        ViewBag.SearchText = SearchText;
                        sSearchtext = sSearchtext + "  where (certName ='" + SearchText + "' or certName like '" + SearchText + "%' or certName like '%" + SearchText + "%'"
                        + " or certName like '%" + SearchText + "')";
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
                    sSearchtext = sSearchtext + " and find_in_set('" + ViewBag.sBranch_name + "', branch)";
                }

                sSqlstmt = sSqlstmt + sSearchtext + " order by certName asc";
                DataSet dsCertList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsCertList.Tables.Count > 0 && dsCertList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsCertList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            CertificatesModels objCertificate = new CertificatesModels
                            {
                                certId = (dsCertList.Tables[0].Rows[i]["certId"].ToString()),
                                certName = dsCertList.Tables[0].Rows[i]["certName"].ToString(),
                                DocUploadPath = dsCertList.Tables[0].Rows[i]["DocUploadPath"].ToString(),
                                Department = objGlobaldata.GetMultiDeptNameById(dsCertList.Tables[0].Rows[i]["Department"].ToString()),
                                Location = objGlobaldata.GetDivisionLocationById(dsCertList.Tables[0].Rows[i]["Location"].ToString()),
                                branch = objGlobaldata.GetMultiCompanyBranchNameById(dsCertList.Tables[0].Rows[i]["branch"].ToString()),
                            };
                            DateTime dtDocDate;
                            if (dsCertList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                               && DateTime.TryParse(dsCertList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objCertificate.expiry_date = dtDocDate;
                            }
                            objCertModelList.CertificateList.Add(objCertificate);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in Certificate: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateSearch: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return Json ("Success");
        }
       
        [AllowAnonymous]
        public ActionResult AddCertificates(CertificatesModels objCertModel, FormCollection form, HttpPostedFileBase file)
        {
            try
            {
                objCertModel.branch = form["branch"];
                objCertModel.Location= form["Location"];
                objCertModel.Department = form["Department"];
                DateTime dateValue;
                if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                {
                    objCertModel.expiry_date = dateValue;
                }
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Certificate"), Path.GetFileName(file.FileName));
                        string sFilename = objCertModel.certName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        file.SaveAs(sFilepath + "/" + sFilename);
                        objCertModel.DocUploadPath = "~/DataUpload/MgmtDocs/Certificate/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddCertificates: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (objCertModel.FunAddCertificate(objCertModel))
                {
                    TempData["Successdata"] = "Added Certificate details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddCertificates: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Certificate");
        }
        
        [AllowAnonymous]
        public JsonResult CertificateDelete(FormCollection form)
        {
            try
            {

                
                   if (form["certId"] != null && form["certId"] != "")
                    {

                        CertificatesModels cert = new CertificatesModels();
                        string scertId = form["certId"];


                        if (cert.FunDeleteCertificate(scertId))
                        {
                            TempData["Successdata"] = "Certificate details deleted successfully";
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
                        TempData["alertdata"] = "Certificate Id cannot be Null or empty";
                        return Json("Failed");
                    }

              
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }
        
        public ActionResult CertificateEdit()
        {
            CertificatesModels objCertificate = new CertificatesModels();
            try
            {

                if (Request.QueryString["certId"] != null && Request.QueryString["certId"] != "")
                {
                    string scertId = Request.QueryString["certId"];

                    string sSqlstmt = "select certId,certName,DocUploadPath,expiry_date,branch,Location,Department from t_certificates where Status=1 and certId='" + scertId + "'";
                    DataSet dsCertList = objGlobaldata.Getdetails(sSqlstmt);
                    if (dsCertList.Tables.Count > 0 && dsCertList.Tables[0].Rows.Count > 0)
                    {

                        objCertificate = new CertificatesModels
                        {
                            certId = (dsCertList.Tables[0].Rows[0]["certId"].ToString()),
                            certName = dsCertList.Tables[0].Rows[0]["certName"].ToString(),
                            DocUploadPath = dsCertList.Tables[0].Rows[0]["DocUploadPath"].ToString(),
                            branch = (dsCertList.Tables[0].Rows[0]["branch"].ToString()),
                            Department = (dsCertList.Tables[0].Rows[0]["Department"].ToString()),
                            Location = (dsCertList.Tables[0].Rows[0]["Location"].ToString()),
                        };
                        DateTime dtDocDate;
                        if (dsCertList.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                           && DateTime.TryParse(dsCertList.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                        {
                            objCertificate.expiry_date = dtDocDate;
                        }
                        ViewBag.Location = objGlobaldata.GetLocationbyMultiDivision(dsCertList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Department = objGlobaldata.GetDepartmentList1(dsCertList.Tables[0].Rows[0]["branch"].ToString());
                        ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                    }
                    else
                    {
                        TempData["alertdata"] = "Certificate ID cannot be Null or empty";
                        return RedirectToAction("Certificate");
                    }
                }
                else
                {
                    TempData["alertdata"] = "Certificate ID cannot be Null or empty";
                    return RedirectToAction("Certificate");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                return RedirectToAction("Certificate");
            }
            return View(objCertificate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CertificateEdit(CertificatesModels obj, FormCollection form, HttpPostedFileBase file)
        {

            try
            {
                obj.branch = form["branch"];
                obj.Location = form["Location"];
                obj.Department = form["Department"];

                DateTime dateValue;
                if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                {
                    obj.expiry_date = dateValue;
                }
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Certificate"), Path.GetFileName(file.FileName));
                        string sFilename = obj.certName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath);
                        string sFilepath = Path.GetDirectoryName(spath);
                        file.SaveAs(sFilepath + "/" + sFilename);
                        obj.DocUploadPath = "~/DataUpload/MgmtDocs/Certificate/" + sFilename;
                        ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        objGlobaldata.AddFunctionalLog("Exception in AddCertificates: " + ex.ToString());
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (obj.FunUpdateCertificate(obj))
                {
                    TempData["Successdata"] = "Certificate details updated successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }

            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in CertificateEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("Certificate");
        }
    }
}