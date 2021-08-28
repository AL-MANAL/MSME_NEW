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
    public class PortalController : Controller
    {
        clsGlobal objGlobaldata = new clsGlobal();

        public PortalController()
        {
            ViewBag.Menutype = "Protal";
            ViewBag.SubMenutype = "Sub-CR";
        }

        [AllowAnonymous]
        public ActionResult AddSubCRMaster()
        {
            PortalModels objPortal = new PortalModels();
            try
            {
                ViewBag.SubMenutype = "Sub-CR";

                objPortal.master_div = objGlobaldata.GetCurrentUserSession().division;
                objPortal.master_dept = objGlobaldata.GetCurrentUserSession().DeptID;
               // objParties.region = objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.ManagerType = objGlobaldata.GetDropdownList("SubCR Manager Type");
                ViewBag.IssueAuthority = objGlobaldata.GetDropdownList("Portal Ministry Name");
                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objPortal.master_div);
                ViewBag.Location = objGlobaldata.GetLocationListBox();                
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSubCRMaster: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSubCRMaster(PortalModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objPortal.master_div = form["master_div"];
                objPortal.master_dept = form["master_dept"];
                objPortal.region = form["region"];

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objPortal.issue_date = dateValue;
                }

                if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                {
                    objPortal.expiry_date = dateValue;
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objPortal.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Portal"), Path.GetFileName(file.FileName));
                            string sFilename = "Portal" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPortal.upload = objPortal.upload + "," + "~/DataUpload/MgmtDocs/Portal/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddSubCRMaster-upload: " + ex.ToString());
                        }
                    }
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPortal.FunAddSubCRMaster(objPortal))
                {
                    TempData["Successdata"] = "Added Sub-CR Master details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddSubCRMaster: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("SubCRMasterList");
        }

        [AllowAnonymous]
        public ActionResult SubCRMasterList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Sub-CR";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_portal_master,subcr_no,commercial_name,activities,issue_date,expiry_date," +
                    "issue_authority,region,master_manager,master_div,master_dept,upload,isc_code,description,logged_by from t_portal_master where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', master_div)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', master_div)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_portal_master desc";
                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPortalList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_portal_master = (dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                subcr_no = dsPortalList.Tables[0].Rows[i]["subcr_no"].ToString(),
                                commercial_name = (dsPortalList.Tables[0].Rows[i]["commercial_name"].ToString()),
                                activities = dsPortalList.Tables[0].Rows[i]["activities"].ToString(),
                                issue_authority =objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["issue_authority"].ToString()),
                                region = objGlobaldata.GetLocationNameById(dsPortalList.Tables[0].Rows[i]["region"].ToString()),
                                master_manager = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["master_manager"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[i]["upload"].ToString(),
                                isc_code = dsPortalList.Tables[0].Rows[i]["isc_code"].ToString(),
                                description = (dsPortalList.Tables[0].Rows[i]["description"].ToString()),
                                master_div = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[i]["master_div"].ToString()),
                                master_dept = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[i]["master_dept"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.issue_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.expiry_date = dtDocDate;
                            }
                            objPortalList.ProtalList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SubCRMasterList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SubCRMasterList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortalList.ProtalList.ToList());
        }
        
        [AllowAnonymous]
        public JsonResult SubCRMasterSearchList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Sub-CR";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_portal_master,subcr_no,commercial_name,activities,issue_date,expiry_date," +
                    "issue_authority,region,master_manager,master_div,master_dept,upload,isc_code,description,logged_by from t_portal_master where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', master_div)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', master_div)";
                }
                sSqlstmt = sSqlstmt + sSearchtext + "order by id_portal_master desc";
                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPortalList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_portal_master = (dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                subcr_no = dsPortalList.Tables[0].Rows[i]["subcr_no"].ToString(),
                                commercial_name = (dsPortalList.Tables[0].Rows[i]["commercial_name"].ToString()),
                                activities = dsPortalList.Tables[0].Rows[i]["activities"].ToString(),
                                issue_authority = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["issue_authority"].ToString()),
                                region = objGlobaldata.GetLocationNameById(dsPortalList.Tables[0].Rows[i]["region"].ToString()),
                                master_manager = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["master_manager"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[i]["upload"].ToString(),
                                isc_code = dsPortalList.Tables[0].Rows[i]["isc_code"].ToString(),
                                description = (dsPortalList.Tables[0].Rows[i]["description"].ToString()),
                                master_div = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[i]["master_div"].ToString()),
                                master_dept = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[i]["master_dept"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[i]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["issue_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.issue_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["expiry_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.expiry_date = dtDocDate;
                            }
                            objPortalList.ProtalList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SubCRMasterSearchList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SubCRMasterSearchList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult SubCRMasterEdit()
        {
            ViewBag.SubMenutype = "Sub-CR";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sid_portal_master = Request.QueryString["id_portal_master"];

                if (sid_portal_master != "")
                { 
                string sSqlstmt = "select id_portal_master,subcr_no,commercial_name,activities,issue_date,expiry_date," +
                    "issue_authority,region,master_manager,master_div,master_dept,upload,isc_code,description,logged_by,recommed_by_manager " +
                    "from t_portal_master where id_portal_master = '" + sid_portal_master + "'";

                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_portal_master = (dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                subcr_no = dsPortalList.Tables[0].Rows[0]["subcr_no"].ToString(),
                                commercial_name = (dsPortalList.Tables[0].Rows[0]["commercial_name"].ToString()),
                                activities = dsPortalList.Tables[0].Rows[0]["activities"].ToString(),
                                issue_authority =/* objGlobaldata.GetPortalMinistryNameById*/(dsPortalList.Tables[0].Rows[0]["issue_authority"].ToString()),
                                region =/* objGlobaldata.GetLocationNameById*/(dsPortalList.Tables[0].Rows[0]["region"].ToString()),
                                master_manager = /*objGlobaldata.GetDropdownitemById*/(dsPortalList.Tables[0].Rows[0]["master_manager"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                isc_code = dsPortalList.Tables[0].Rows[0]["isc_code"].ToString(),
                                description = (dsPortalList.Tables[0].Rows[0]["description"].ToString()),
                                master_div = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsPortalList.Tables[0].Rows[0]["master_div"].ToString()),
                                master_dept = /*objGlobaldata.GetMultiDeptNameById*/(dsPortalList.Tables[0].Rows[0]["master_dept"].ToString()),
                                recommed_by_manager = dsPortalList.Tables[0].Rows[0]["recommed_by_manager"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[0]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["issue_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.issue_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.expiry_date = dtDocDate;
                            }
                            ViewBag.ManagerType = objGlobaldata.GetDropdownList("SubCR Manager Type");
                            ViewBag.IssueAuthority = objGlobaldata.GetDropdownList("Portal Ministry Name");
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.Department = objGlobaldata.GetDepartmentList1(dsPortalList.Tables[0].Rows[0]["master_div"].ToString());
                           // ViewBag.Department = objGlobaldata.GetDepartmentList1;
                            ViewBag.Location = objGlobaldata.GetLocationListBox();

                            return View(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SubCRMasterEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }             
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SubCRMasterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SubCRMasterList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubCRMasterEdit(PortalModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                HttpPostedFileBase files = Request.Files[0];

                objPortal.master_div = form["master_div"];
                objPortal.master_dept = form["master_dept"];
                objPortal.region = form["region"];

                DateTime dateValue;
                if (DateTime.TryParse(form["issue_date"], out dateValue) == true)
                {
                    objPortal.issue_date = dateValue;
                }

                if (DateTime.TryParse(form["expiry_date"], out dateValue) == true)
                {
                    objPortal.expiry_date = dateValue;
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objPortal.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Portal"), Path.GetFileName(file.FileName));
                            string sFilename = "Portal" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPortal.upload = objPortal.upload + "," + "~/DataUpload/MgmtDocs/Portal/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SubCRMasterEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objPortal.upload = objPortal.upload + "," + form["QCDocsVal"];
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objPortal.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objPortal.upload = null;
                }
                if (objPortal.FunUpdateSubCRMaster(objPortal))
                {
                    TempData["Successdata"] = "updated Sub-CR Master details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SubCRMasterEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SubCRMasterList");
        }

        [AllowAnonymous]
        public ActionResult SubCRMasterDetails()
        {
            ViewBag.SubMenutype = "Sub-CR";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sid_portal_master = Request.QueryString["id_portal_master"];

                if (sid_portal_master != "")
                {
                    string sSqlstmt = "select id_portal_master,subcr_no,commercial_name,activities,issue_date,expiry_date," +
                        "issue_authority,region,master_manager,master_div,master_dept,upload,isc_code,description,logged_by,recommed_by_manager " +
                        "from t_portal_master where id_portal_master = '" + sid_portal_master + "'";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_portal_master = (dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                subcr_no = dsPortalList.Tables[0].Rows[0]["subcr_no"].ToString(),
                                commercial_name = (dsPortalList.Tables[0].Rows[0]["commercial_name"].ToString()),
                                activities = dsPortalList.Tables[0].Rows[0]["activities"].ToString(),
                                issue_authority =objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["issue_authority"].ToString()),
                                region = objGlobaldata.GetLocationNameById(dsPortalList.Tables[0].Rows[0]["region"].ToString()),
                                master_manager = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["master_manager"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                isc_code = dsPortalList.Tables[0].Rows[0]["isc_code"].ToString(),
                                description = (dsPortalList.Tables[0].Rows[0]["description"].ToString()),
                                master_div = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[0]["master_div"].ToString()),
                                master_dept = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[0]["master_dept"].ToString()),
                                recommed_by_manager = dsPortalList.Tables[0].Rows[0]["recommed_by_manager"].ToString(),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[0]["issue_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["issue_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.issue_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["expiry_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["expiry_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.expiry_date = dtDocDate;
                            }
                            return View(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in SubCRMasterDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in SubCRMasterDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("SubCRMasterList");
        }


        [AllowAnonymous]
        public JsonResult PortalDocDelete(FormCollection form)
        {
            try
            {

                if (form["id_portal_master"] != null && form["id_portal_master"] != "")
                {

                    PortalModels Doc = new PortalModels();
                    string sid_portal_master = form["id_portal_master"];


                    if (Doc.FunDeleteSubCRMaster(sid_portal_master))
                    {
                        TempData["Successdata"] = "portal details deleted successfully";
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
                objGlobaldata.AddFunctionalLog("Exception in PortalDocDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        //Portal Authorization
        [AllowAnonymous]
        public ActionResult AddPortalAuthorization()
        {
            PortalModels objPortal = new PortalModels();
            try
            {
                ViewBag.SubMenutype = "Authorization";
                ViewBag.Session = Request.QueryString["View"];

                if (ViewBag.Session == "1")
                {
                    ViewBag.PName = "Government";
                }
                else if (ViewBag.Session == "2")
                {
                    ViewBag.PName = "Principal";
                }
                else if (ViewBag.Session == "3")
                {
                    ViewBag.PName = "Customer";
                }
                else if (ViewBag.Session == "4")
                {
                    ViewBag.PName = "Supplier";
                }
                else if (ViewBag.Session == "5")
                {
                    ViewBag.PName ="Others";
                }

                objPortal.division = objGlobaldata.GetCurrentUserSession().division;
                objPortal.department = objGlobaldata.GetCurrentUserSession().DeptID;
                objPortal.requested_by = objGlobaldata.GetCurrentUserSession().empid;
                // objParties.region =approve_depthead objGlobaldata.GetCurrentUserSession().Work_Location;

                ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                ViewBag.ProtalCategory = objGlobaldata.GetDropdownList("Portal Category");
                //ViewBag.ProtalName = objGlobaldata.GetPotalNameList();
                ViewBag.PortalMaster = objPortal.GetSUBCRPortalMasterList();
                ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
                ViewBag.CEO = objGlobaldata.GetCEO();
                //ViewBag.VP = objGlobaldata.GetVP();
                //ViewBag.Chairman = objGlobaldata.GetChairman();
                ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");

                ViewBag.Department = objGlobaldata.GetDepartmentListbox(objPortal.division);
                ViewBag.ProtalName = objGlobaldata.GetPortalNameListBox();
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPortalAuthorization: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPortalAuthorization(PortalModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            try
            {
                objPortal.division = form["division"];
                ViewBag.Session = form["View"];
                objPortal.department = form["department"];

                if (ViewBag.Session == "1")
                {
                    objPortal.portal_category = objGlobaldata.GetPortalCategoryIdByName("Government");
                }
                else if (ViewBag.Session == "2")
                {
                    objPortal.portal_category = objGlobaldata.GetPortalCategoryIdByName("Principal");
                }
                else if (ViewBag.Session == "3")
                {
                    objPortal.portal_category = objGlobaldata.GetPortalCategoryIdByName("Customer");
                }
                else if (ViewBag.Session == "4")
                {
                    objPortal.portal_category = objGlobaldata.GetPortalCategoryIdByName("Supplier");
                }
                else if (ViewBag.Session == "5")
                {
                    objPortal.portal_category = objGlobaldata.GetPortalCategoryIdByName("Others");
                }

                DateTime dateValue;
                if (DateTime.TryParse(form["request_date"], out dateValue) == true)
                {
                    objPortal.request_date = dateValue;
                }

                if (DateTime.TryParse(form["access_date"], out dateValue) == true)
                {
                    objPortal.access_date = dateValue;
                }

                if (DateTime.TryParse(form["valid_date"], out dateValue) == true)
                {
                    objPortal.valid_date = dateValue;
                }

                //Nominated Employee
                for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                {
                    if (form["empno " + i] != "" && form["empno " + i] != null)
                    {
                        objPortal.nominated_emp = objPortal.nominated_emp + "," + form["empno " + i];
                    }
                }

                if (objPortal.nominated_emp != null)
                {
                    objPortal.nominated_emp = objPortal.nominated_emp.Trim(',');
                }


                //Recommended Employees
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["rempno " + i] != "" && form["rempno " + i] != null)
                    {
                        objPortal.recommended_by = objPortal.recommended_by + "," + form["rempno " + i];
                    }
                }

                if (objPortal.recommended_by != null)
                {
                    objPortal.recommended_by = objPortal.recommended_by.Trim(',');
                }

                HttpPostedFileBase files = Request.Files[0];
                if (upload != null && files.ContentLength > 0)
                {
                    objPortal.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Portal"), Path.GetFileName(file.FileName));
                            string sFilename = "Portal" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPortal.upload = objPortal.upload + "," + "~/DataUpload/MgmtDocs/Portal/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in AddPortalAuthorization-upload: " + ex.ToString());
                        }
                    }
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }

                if (objPortal.FunAddPortalAuth(objPortal))
                {
                    TempData["Successdata"] = "Added Portal Authorization  details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in AddPortalAuthorization: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            return RedirectToAction("PortalAuthorizationList",new { View = ViewBag.Session });
        }

        [AllowAnonymous]
        public ActionResult PortalAuthorizationList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Authorization";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                       "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status as apporved_statusId,access_purpose," +
                       "case when apporved_status = '0' then 'Pending' when apporved_status = '1' then 'Approved by Recommended by' when apporved_status = '2' then 'Apporved By Divisional Approver(CEO)' when apporved_status = '3' then 'Approved by division President / VP'" +
                       " when apporved_status = '4' then 'Request Rejected' else 'Final Approved'  end as apporved_status,recommend_comments,ceo_comments,vp_comments,chairman_comments,reject_comments," +
                       "approve_recommed_date,approve_ceo_date,approve_vp_date,approve_chairman_date,rejected_date,department from t_portal_authorization where Active=1";
                              
                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }
                sSqlstmt = sSqlstmt + sSearchtext;

                ViewBag.Session = Request.QueryString["View"];

                ViewBag.PortalType1 = objGlobaldata.GetPortalCategoryIdByName("Government");
                ViewBag.PortalType2 = objGlobaldata.GetPortalCategoryIdByName("Principal");
                ViewBag.PortalType3 = objGlobaldata.GetPortalCategoryIdByName("Customer");
                ViewBag.PortalType4 = objGlobaldata.GetPortalCategoryIdByName("Supplier");
                ViewBag.PortalType5 = objGlobaldata.GetPortalCategoryIdByName("Others");
                if (ViewBag.Session == "1")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType1 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "2")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType2 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "3")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType3 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "4")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType4 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "5")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType5 + "'order by id_authorization desc";
                }

                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPortalList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_authorization = (dsPortalList.Tables[0].Rows[i]["id_authorization"].ToString()),
                                access_no = dsPortalList.Tables[0].Rows[i]["access_no"].ToString(),
                                portal_category = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["portal_category"].ToString()),
                                subcr_no = objParties.GetSUBCRNoById(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                commercial_name = objParties.GetCommercialNameById(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                access_purpose = objParties.GetActivityBySUBCRId(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                nominated_emp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["nominated_emp"].ToString()),
                                justification = (dsPortalList.Tables[0].Rows[i]["justification"].ToString()),
                                requested_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["requested_by"].ToString()),
                                recommended_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["recommended_by"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[i]["upload"].ToString(),
                                approve_chairman = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_chairman"].ToString()),
                                approve_vp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_vp"].ToString()),
                                approve_ceo = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_ceo"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[i]["division"].ToString()),
                                apporved_status = dsPortalList.Tables[0].Rows[i]["apporved_status"].ToString(),
                                apporved_statusId = dsPortalList.Tables[0].Rows[i]["apporved_statusId"].ToString(),
                                department = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[i]["department"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[i]["request_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["request_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.request_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["access_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["access_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.access_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["valid_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["valid_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.valid_date = dtDocDate;
                            }
                            objPortalList.ProtalList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return View(objPortalList.ProtalList.ToList());
        }

        [AllowAnonymous]
        public JsonResult PortalAuthorizationSearchList(FormCollection form, int? page, string branch_name)
        {
            ViewBag.SubMenutype = "Authorization";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sBranch_name = objGlobaldata.GetCurrentUserSession().division;
                string sBranchtree = objGlobaldata.GetCurrentUserSession().BranchTree;
                ViewBag.Branch = objGlobaldata.GetMultiBranchListByID(sBranchtree);
                string sSearchtext = "";

                string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                       "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status as apporved_statusId,access_purpose," +
                       "case when apporved_status = '0' then 'Pending' when apporved_status = '1' then 'Approved by Recommended by' when apporved_status = '2' then 'Apporved By Divisional Approver(CEO)' when apporved_status = '3' then 'Approved by division President / VP'" +
                       " when apporved_status = '4' then 'Request Rejected' else 'Final Approved'  end as apporved_status,recommend_comments,ceo_comments,vp_comments,chairman_comments,reject_comments," +
                       "approve_recommed_date,approve_ceo_date,approve_vp_date,approve_chairman_date,rejected_date,department from t_portal_authorization where Active=1";

                if (branch_name != null && branch_name != "")
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + branch_name + "', division)";
                    ViewBag.Branch_name = branch_name;
                }
                else
                {
                    sSearchtext = sSearchtext + " and find_in_set('" + sBranch_name + "', division)";
                }
                sSqlstmt = sSqlstmt + sSearchtext;

                ViewBag.Session = Request.QueryString["View"];

                ViewBag.PortalType1 = objGlobaldata.GetPortalCategoryIdByName("Government");
                ViewBag.PortalType2 = objGlobaldata.GetPortalCategoryIdByName("Principal");
                ViewBag.PortalType3 = objGlobaldata.GetPortalCategoryIdByName("Customer");
                ViewBag.PortalType4 = objGlobaldata.GetPortalCategoryIdByName("Supplier");
                ViewBag.PortalType5 = objGlobaldata.GetPortalCategoryIdByName("Others");
                if (ViewBag.Session == "1")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType1 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "2")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType2 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "3")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType3 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "4")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType4 + "'order by id_authorization desc";
                }
                else if (ViewBag.Session == "5")
                {
                    sSqlstmt = sSqlstmt + " and portal_category= '" + ViewBag.PortalType5 + "'order by id_authorization desc";
                }

                DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsPortalList.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_authorization = (dsPortalList.Tables[0].Rows[i]["id_authorization"].ToString()),
                                access_no = dsPortalList.Tables[0].Rows[i]["access_no"].ToString(),
                                portal_category = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[i]["portal_category"].ToString()),
                                subcr_no = objParties.GetSUBCRNoById(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                commercial_name = objParties.GetCommercialNameById(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                access_purpose = objParties.GetActivityBySUBCRId(dsPortalList.Tables[0].Rows[i]["id_portal_master"].ToString()),
                                nominated_emp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["nominated_emp"].ToString()),
                                justification = (dsPortalList.Tables[0].Rows[i]["justification"].ToString()),
                                requested_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["requested_by"].ToString()),
                                recommended_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["recommended_by"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[i]["upload"].ToString(),
                                approve_chairman = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_chairman"].ToString()),
                                approve_vp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_vp"].ToString()),
                                approve_ceo = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[i]["approve_ceo"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[i]["division"].ToString()),
                                apporved_status = dsPortalList.Tables[0].Rows[i]["apporved_status"].ToString(),
                                apporved_statusId = dsPortalList.Tables[0].Rows[i]["apporved_statusId"].ToString(),
                                department = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[i]["department"].ToString()),
                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[i]["request_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["request_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.request_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["access_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["access_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.access_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[i]["valid_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[i]["valid_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.valid_date = dtDocDate;
                            }
                            objPortalList.ProtalList.Add(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationSearchList: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationSearchList: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Success");
        }

        [AllowAnonymous]
        public ActionResult PortalAuthorizationEdit()
        {
            ViewBag.SubMenutype = "Authorization";
           
            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            ViewBag.Session = Request.QueryString["View"];

            try
            {
                string sid_authorization = Request.QueryString["id_authorization"];
                if (sid_authorization != null && sid_authorization != "")
                {
                   string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                        "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status as apporved_statusId,access_purpose," +
                        "case when apporved_status = '0' then 'Pending' when apporved_status = '1' then 'Approved by Recommended by' when apporved_status = '2' then 'Apporved By Divisional Approver(CEO)' when apporved_status = '3' then 'Approved by division President / VP'" +
                        " when apporved_status = '4' then 'Request Rejected' else 'Final Approved'  end as apporved_status,recommend_comments,ceo_comments,vp_comments,chairman_comments,reject_comments," +
                        "approve_recommed_date,approve_ceo_date,approve_vp_date,approve_chairman_date,rejected_date,portal_name,ministry_name,department from t_portal_authorization where id_authorization= '" + sid_authorization + "';";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_authorization = (dsPortalList.Tables[0].Rows[0]["id_authorization"].ToString()),
                                access_no = dsPortalList.Tables[0].Rows[0]["access_no"].ToString(),
                                portal_category = (dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                                portal_categoryName= objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                                subcr_no = /*objParties.GetSUBCRNoById*/(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                commercial_name = objParties.GetCommercialNameById(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                activities = objParties.GetActivityBySUBCRId(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                nominated_emp = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["nominated_emp"].ToString()),
                                justification = (dsPortalList.Tables[0].Rows[0]["justification"].ToString()),
                                requested_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["requested_by"].ToString()),
                                recommended_by = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["recommended_by"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                approve_chairman = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["approve_chairman"].ToString()),
                                approve_vp = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["approve_vp"].ToString()),
                                approve_ceo = /*objGlobaldata.GetMultiHrEmpNameById*/(dsPortalList.Tables[0].Rows[0]["approve_ceo"].ToString()),
                                division = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsPortalList.Tables[0].Rows[0]["division"].ToString()),
                                apporved_status = dsPortalList.Tables[0].Rows[0]["apporved_status"].ToString(),
                                apporved_statusId = dsPortalList.Tables[0].Rows[0]["apporved_statusId"].ToString(),
                                access_purpose= dsPortalList.Tables[0].Rows[0]["access_purpose"].ToString(),
                                portal_name = /*objGlobaldata.GetPortalNamebyPortalId*/(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                //ministry_name = objGlobaldata.GetMinistryNamebyPortalId(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                department = /*objGlobaldata.GetMultiCompanyBranchNameById*/(dsPortalList.Tables[0].Rows[0]["department"].ToString()),
                            };
                            ViewBag.Dept = objGlobaldata.GetDepartmentList1(dsPortalList.Tables[0].Rows[0]["division"].ToString());
                                                       
                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[0]["request_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["request_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.request_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["access_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["access_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.access_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["valid_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["valid_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.valid_date = dtDocDate;
                            }

                            string sSqlstmt1 = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,MobileNo," +
                            "Date_of_Birth,Eid_no,Nationaliity from t_hr_employee where emp_no = '" + objPartiesModels.nominated_emp + "'";
                            DataSet dsList = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString());
                                ViewBag.empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString());
                                ViewBag.division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString());
                                ViewBag.location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString());
                                ViewBag.department = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                ViewBag.EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString());
                                ViewBag.MobileNo = (dsList.Tables[0].Rows[0]["MobileNo"].ToString());
                                ViewBag.Eid_no = (dsList.Tables[0].Rows[0]["Eid_no"].ToString());
                                ViewBag.Date_of_Birth = (dsList.Tables[0].Rows[0]["Date_of_Birth"].ToString());
                                ViewBag.Nationaliity = (dsList.Tables[0].Rows[0]["Nationaliity"].ToString());
                                ViewBag.divisionId = (dsList.Tables[0].Rows[0]["division"].ToString());
                            }


                            string sSqlstmt2 = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId" +
                           " from t_hr_employee where emp_no = '" + objPartiesModels.recommended_by + "'";
                            DataSet dsList1 = objGlobaldata.Getdetails(sSqlstmt2);
                            if (dsList1.Tables.Count > 0 && dsList1.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.Remp_id = (dsList1.Tables[0].Rows[0]["emp_id"].ToString());
                                ViewBag.Rempname = objGlobaldata.GetEmpHrNameById(dsList1.Tables[0].Rows[0]["emp_no"].ToString());
                                ViewBag.Rdivision = objGlobaldata.GetCompanyBranchNameById(dsList1.Tables[0].Rows[0]["division"].ToString());
                                ViewBag.Rlocation = objGlobaldata.GetDivisionLocationById(dsList1.Tables[0].Rows[0]["Emp_work_location"].ToString());
                                ViewBag.Rdepartment = objGlobaldata.GetMultiDeptNameById(dsList1.Tables[0].Rows[0]["Dept_Id"].ToString());
                                ViewBag.REmailId = (dsList1.Tables[0].Rows[0]["EmailId"].ToString());
                            }

                            //ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadbyDivisionList(ViewBag.divisionId);
                            ViewBag.Branch = objGlobaldata.GetCompanyBranchListbox();
                            ViewBag.ProtalCategory = objGlobaldata.GetDropdownList("Portal Category");
                            //ViewBag.ProtalName = objGlobaldata.GetPotalNameList();
                            ViewBag.PortalMaster = objParties.GetSUBCRPortalMasterList();
                            ViewBag.EmpList = objGlobaldata.GetHrEmployeeListbox();
                            ViewBag.DeptHeadList = objGlobaldata.GetDeptHeadList();
                            ViewBag.CEO = objGlobaldata.GetCEO();
                            //ViewBag.VP = objGlobaldata.GetVP();
                            //ViewBag.Chairman = objGlobaldata.GetChairman();
                            ViewBag.YesNo = objGlobaldata.GetConstantValue("YesNo");
                            // ViewBag.MinistryName = objGlobaldata.GetPortalMinistryNameList();
                            ViewBag.ProtalName = objGlobaldata.GetPortalNameListBox();
                            return View(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationEdit: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }


                }               
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PortalAuthorizationList",new { View = ViewBag.Session });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PortalAuthorizationEdit(PortalModels objPortal, FormCollection form, IEnumerable<HttpPostedFileBase> upload)
        {
            string sView = form["View"];
            objPortal.department = form["department"];
            objPortal.division = form["division"];

            try
            {
                string QCDelete = Request.Form["QCDocsValselectall"];
                HttpPostedFileBase files = Request.Files[0];

                objPortal.division = form["division"];
                objPortal.portal_category = form["portal_category"];               

                DateTime dateValue;
                if (DateTime.TryParse(form["request_date"], out dateValue) == true)
                {
                    objPortal.request_date = dateValue;
                }

                if (DateTime.TryParse(form["access_date"], out dateValue) == true)
                {
                    objPortal.access_date = dateValue;
                }

                if (DateTime.TryParse(form["valid_date"], out dateValue) == true)
                {
                    objPortal.valid_date = dateValue;
                }

                //Nominated Employee
                //for (int i = 0; i < Convert.ToInt16(form["itemcount"]); i++)
                //{
                //    if (form["empno " + i] != "" && form["empno " + i] != null)
                //    {
                //        objPortal.nominated_emp = objPortal.nominated_emp + "," + form["empno " + i];
                //    }
                //}

                //if (objPortal.nominated_emp != null)
                //{
                //    objPortal.nominated_emp = objPortal.nominated_emp.Trim(',');
                //}

                objPortal.nominated_emp = form["nominated_emp"];

               

                //Recommended Employees
                for (int i = 0; i < Convert.ToInt16(form["itemcnt"]); i++)
                {
                    if (form["rempno " + i] != "" && form["rempno " + i] != null)
                    {
                        objPortal.recommended_by = objPortal.recommended_by + "," + form["rempno " + i];
                    }
                }

                if (objPortal.recommended_by != null)
                {
                    objPortal.recommended_by = objPortal.recommended_by.Trim(',');
                }

                if (upload != null && files.ContentLength > 0)
                {
                    objPortal.upload = "";
                    foreach (var file in upload)
                    {
                        try
                        {
                            string spath = Path.Combine(Server.MapPath("~/DataUpload/MgmtDocs/Portal"), Path.GetFileName(file.FileName));
                            string sFilename = "Portal" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + Path.GetFileName(spath), sFilepath = Path.GetDirectoryName(spath);
                            file.SaveAs(sFilepath + "/" + sFilename);
                            objPortal.upload = objPortal.upload + "," + "~/DataUpload/MgmtDocs/Portal/" + sFilename;
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationEdit-upload: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else
                {
                    ViewBag.Message = "You have not specified a file.";
                }
                if (form["QCDocsVal"] != null && form["QCDocsVal"] != "")
                {
                    objPortal.upload = objPortal.upload + "," + form["QCDocsVal"];
                    objPortal.upload = objPortal.upload.Trim(',');
                }
                else if (form["QCDocsVal"] == null && QCDelete != null && files.ContentLength == 0)
                {
                    objPortal.upload = null;
                }
                else if (form["QCDocsVal"] == null && files.ContentLength == 0)
                {
                    objPortal.upload = null;
                }
                if (objPortal.FunUpdatePortalAuth(objPortal))
                {
                    TempData["Successdata"] = "Updated Portal Authorization  details successfully";
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationEdit: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PortalAuthorizationList", new { View = sView });
        }

        [AllowAnonymous]
        public ActionResult PortalAuthorizationDetails()
        {
            ViewBag.SubMenutype = "Authorization";

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            ViewBag.View = Request.QueryString["View"];
            try
            {
                string sid_authorization = Request.QueryString["id_authorization"];
                if (sid_authorization != null && sid_authorization != "")
                {
                    string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                        "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status as apporved_statusId,access_purpose," +
                        "case when apporved_status = '0' then 'Pending' when apporved_status = '1' then 'Approved by Recommended by' when apporved_status = '2' then 'Apporved By Divisional Approver(CEO)' when apporved_status = '3' then 'Approved by division President / VP'" +
                        " when apporved_status = '4' then 'Request Rejected' else 'Final Approved'  end as apporved_status,recommend_comments,ceo_comments,vp_comments,chairman_comments,reject_comments," +
                        "approve_recommed_date,approve_ceo_date,approve_vp_date,approve_chairman_date,rejected_date,portal_name,ministry_name,department from t_portal_authorization where id_authorization= '" + sid_authorization + "';";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                    {
                        try
                        {
                            PortalModels objPartiesModels = new PortalModels
                            {
                                id_authorization = (dsPortalList.Tables[0].Rows[0]["id_authorization"].ToString()),
                                access_no = dsPortalList.Tables[0].Rows[0]["access_no"].ToString(),
                                portal_category = objGlobaldata.GetDropdownitemById(dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                                subcr_no = objParties.GetSUBCRNoById(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                commercial_name = objParties.GetCommercialNameById(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                activities = objParties.GetActivityBySUBCRId(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                nominated_emp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["nominated_emp"].ToString()),
                                justification = (dsPortalList.Tables[0].Rows[0]["justification"].ToString()),
                                requested_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["requested_by"].ToString()),
                                recommended_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["recommended_by"].ToString()),
                                upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                approve_chairman = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_chairman"].ToString()),
                                approve_vp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_vp"].ToString()),
                                approve_ceo = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_ceo"].ToString()),
                                division = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[0]["division"].ToString()),
                                apporved_status = dsPortalList.Tables[0].Rows[0]["apporved_status"].ToString(),
                                apporved_statusId = dsPortalList.Tables[0].Rows[0]["apporved_statusId"].ToString(),
                                access_purpose = dsPortalList.Tables[0].Rows[0]["access_purpose"].ToString(),
                                recommend_comments = dsPortalList.Tables[0].Rows[0]["recommend_comments"].ToString(),
                                ceo_comments = dsPortalList.Tables[0].Rows[0]["ceo_comments"].ToString(),
                                vp_comments = dsPortalList.Tables[0].Rows[0]["vp_comments"].ToString(),
                                chairman_comments = dsPortalList.Tables[0].Rows[0]["chairman_comments"].ToString(),
                                reject_comments = dsPortalList.Tables[0].Rows[0]["reject_comments"].ToString(),
                                portal_name = objGlobaldata.GetPortalNamebyPortalId(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                ministry_name =objGlobaldata.GetMinistryNamebyPortalId(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                department = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[0]["department"].ToString()),

                            };

                            DateTime dtDocDate;
                            if (dsPortalList.Tables[0].Rows[0]["request_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["request_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.request_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["access_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["access_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.access_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["valid_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["valid_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.valid_date = dtDocDate;
                            }

                            if (dsPortalList.Tables[0].Rows[0]["approve_recommed_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_recommed_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_recommed_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_ceo_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_ceo_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_ceo_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_vp_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_vp_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_vp_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_chairman_date"].ToString() != ""
                            && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_chairman_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_chairman_date = dtDocDate;
                            }

                            string sSqlstmt1 = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,MobileNo," +
                            "Date_of_Birth,Eid_no,Nationaliity from t_hr_employee where emp_no = '" + objPartiesModels.nominated_emp + "'";
                            DataSet dsList = objGlobaldata.Getdetails(sSqlstmt1);
                            if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString());
                                ViewBag.empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString());
                                ViewBag.division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString());
                                ViewBag.location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString());
                                ViewBag.department = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString());
                                ViewBag.EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString());
                                ViewBag.MobileNo = (dsList.Tables[0].Rows[0]["MobileNo"].ToString());
                                ViewBag.Eid_no = (dsList.Tables[0].Rows[0]["Eid_no"].ToString());
                                ViewBag.Date_of_Birth = (dsList.Tables[0].Rows[0]["Date_of_Birth"].ToString());
                                ViewBag.Nationaliity = (dsList.Tables[0].Rows[0]["Nationaliity"].ToString());
                                ViewBag.divisionId = (dsList.Tables[0].Rows[0]["division"].ToString());
                            }

                            string sSqlstmt2 = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId" +
                           " from t_hr_employee where emp_no = '" + objPartiesModels.recommended_by + "'";
                            DataSet dsList1 = objGlobaldata.Getdetails(sSqlstmt2);
                            if (dsList1.Tables.Count > 0 && dsList1.Tables[0].Rows.Count > 0)
                            {
                                ViewBag.Remp_id = (dsList1.Tables[0].Rows[0]["emp_id"].ToString());
                                ViewBag.Rempname = objGlobaldata.GetEmpHrNameById(dsList1.Tables[0].Rows[0]["emp_no"].ToString());
                                ViewBag.Rdivision = objGlobaldata.GetCompanyBranchNameById(dsList1.Tables[0].Rows[0]["division"].ToString());
                                ViewBag.Rlocation = objGlobaldata.GetDivisionLocationById(dsList1.Tables[0].Rows[0]["Emp_work_location"].ToString());
                                ViewBag.Rdepartment = objGlobaldata.GetMultiDeptNameById(dsList1.Tables[0].Rows[0]["Dept_Id"].ToString());
                                ViewBag.REmailId = (dsList1.Tables[0].Rows[0]["EmailId"].ToString());
                            }                          

                            return View(objPartiesModels);
                        }
                        catch (Exception ex)
                        {
                            objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationDetails: " + ex.ToString());
                            TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("PortalAuthorizationList", new { View = ViewBag.View });
        }

        [AllowAnonymous]
        public ActionResult PortalAuthorizationApprove()
        {
            ViewBag.SubMenutype = "Authorization";
            ViewBag.VP = objGlobaldata.GetVP();
            ViewBag.Chairman = objGlobaldata.GetChairman();

            PortalModelsList objPortalList = new PortalModelsList();
            objPortalList.ProtalList = new List<PortalModels>();
            PortalModels objParties = new PortalModels();
            try
            {
                string sid_authorization = Request.QueryString["id_authorization"];
                if (sid_authorization != null && sid_authorization != "")
                {
                    string sSqlstmt = "select id_authorization,access_no,request_date,access_date,valid_date,portal_category," +
                          "id_portal_master,upload,nominated_emp,justification,recommended_by,requested_by,approve_chairman,approve_vp,approve_ceo,logged_by,division,apporved_status as apporved_statusId,access_purpose," +
                          "case when apporved_status = '0' then 'Pending' when apporved_status = '1' then 'Approved by Recommended by' when apporved_status = '2' then 'Apporved By Divisional Approver(CEO)' when apporved_status = '3' then 'Approved by division President / VP'" +
                          " when apporved_status = '4' then 'Request Rejected' else 'Final Approved'  end as apporved_status,recommend_comments,ceo_comments,vp_comments,chairman_comments,reject_comments," +
                          "approve_recommed_date,approve_ceo_date,approve_vp_date,approve_chairman_date,rejected_date,department,portal_name from t_portal_authorization where id_authorization= '" + sid_authorization + "';";

                    DataSet dsPortalList = objGlobaldata.Getdetails(sSqlstmt);

                    if (dsPortalList.Tables.Count > 0 && dsPortalList.Tables[0].Rows.Count > 0)
                    {
                         try
                            {
                                PortalModels objPartiesModels = new PortalModels
                                {
                                    id_authorization = (dsPortalList.Tables[0].Rows[0]["id_authorization"].ToString()),
                                    access_no = dsPortalList.Tables[0].Rows[0]["access_no"].ToString(),
                                    portal_category = (dsPortalList.Tables[0].Rows[0]["portal_category"].ToString()),
                                    subcr_no = objParties.GetSUBCRNoById(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                    commercial_name = objParties.GetCommercialNameById(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                    activities = objParties.GetActivityBySUBCRId(dsPortalList.Tables[0].Rows[0]["id_portal_master"].ToString()),
                                    access_purpose = dsPortalList.Tables[0].Rows[0]["access_purpose"].ToString(),
                                    nominated_emp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["nominated_emp"].ToString()),
                                    justification = (dsPortalList.Tables[0].Rows[0]["justification"].ToString()),
                                    requested_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["requested_by"].ToString()),
                                    recommended_by = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["recommended_by"].ToString()),
                                    upload = dsPortalList.Tables[0].Rows[0]["upload"].ToString(),
                                    approve_chairman = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_chairman"].ToString()),
                                    approve_vp = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_vp"].ToString()),
                                    approve_ceo = objGlobaldata.GetMultiHrEmpNameById(dsPortalList.Tables[0].Rows[0]["approve_ceo"].ToString()),
                                    division = objGlobaldata.GetMultiCompanyBranchNameById(dsPortalList.Tables[0].Rows[0]["division"].ToString()),
                                    apporved_status = dsPortalList.Tables[0].Rows[0]["apporved_status"].ToString(),
                                    apporved_statusId = dsPortalList.Tables[0].Rows[0]["apporved_statusId"].ToString(),
                                    reject_comments = (dsPortalList.Tables[0].Rows[0]["reject_comments"].ToString()),
                                    department = objGlobaldata.GetMultiDeptNameById(dsPortalList.Tables[0].Rows[0]["department"].ToString()),
                                    recommend_comments = dsPortalList.Tables[0].Rows[0]["recommend_comments"].ToString(),
                                    ceo_comments = dsPortalList.Tables[0].Rows[0]["ceo_comments"].ToString(),
                                    vp_comments = dsPortalList.Tables[0].Rows[0]["vp_comments"].ToString(),
                                    chairman_comments = dsPortalList.Tables[0].Rows[0]["chairman_comments"].ToString(),
                                    portal_name = objGlobaldata.GetPortalNamebyPortalId(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),
                                    ministry_name = objGlobaldata.GetMinistryNamebyPortalId(dsPortalList.Tables[0].Rows[0]["portal_name"].ToString()),                                 
                                };

                                DateTime dtDocDate;
                                if (dsPortalList.Tables[0].Rows[0]["request_date"].ToString() != ""
                                   && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["request_date"].ToString(), out dtDocDate))
                                {
                                    objPartiesModels.request_date = dtDocDate;
                                }
                                if (dsPortalList.Tables[0].Rows[0]["access_date"].ToString() != ""
                                  && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["access_date"].ToString(), out dtDocDate))
                                {
                                    objPartiesModels.access_date = dtDocDate;
                                }
                                if (dsPortalList.Tables[0].Rows[0]["valid_date"].ToString() != ""
                                  && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["valid_date"].ToString(), out dtDocDate))
                                {
                                    objPartiesModels.valid_date = dtDocDate;
                                }

                            if (dsPortalList.Tables[0].Rows[0]["approve_recommed_date"].ToString() != ""
                               && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_recommed_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_recommed_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_ceo_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_ceo_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_ceo_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_vp_date"].ToString() != ""
                              && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_vp_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_vp_date = dtDocDate;
                            }
                            if (dsPortalList.Tables[0].Rows[0]["approve_chairman_date"].ToString() != ""
                            && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["approve_chairman_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.approve_chairman_date = dtDocDate;
                            }

                            if (dsPortalList.Tables[0].Rows[0]["rejected_date"].ToString() != ""
                             && DateTime.TryParse(dsPortalList.Tables[0].Rows[0]["rejected_date"].ToString(), out dtDocDate))
                            {
                                objPartiesModels.rejected_date = dtDocDate;
                            }
                            return View(objPartiesModels);
                            }
                            catch (Exception ex)
                            {
                                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationApprove: " + ex.ToString());
                                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationApprove: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public ActionResult ApproveAuthRecommend(FormCollection from)
        {
            try
            {
                PortalModels objPortal = new PortalModels();
                string sid_authorization = from["id_authorization"];
                int sapporved_status = Convert.ToInt32(from["approve"]);
                string scomments = from["comments"];
                if (sapporved_status != 2)
                {
                    if (objPortal.FunApproveAuthRecommend(sid_authorization, sapporved_status, scomments))
                    {
                        TempData["Successdata"] = "Access request Approved";
                    }
                    else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
                else
                {
                    string sapprove_chairman = from["approve_chairman"];
                    string sapprove_vp = from["approve_vp"];

                    if (objPortal.FunApproveAuthRecommendCEO(sid_authorization, sapporved_status, scomments, sapprove_vp, sapprove_chairman))
                     {
                        TempData["Successdata"] = "Access request Approved";
                     }
                else
                    {
                        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                    }
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ApproveAuthRecommend: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public JsonResult PortalAuthorizationDelete(FormCollection form)
        {
            try
            {
                if (form["id_authorization"] != null && form["id_authorization"] != "")
                {
                    PortalModels Doc = new PortalModels();
                    string sid_authorization = form["id_authorization"];

                    if (Doc.FunDeletePortalAuth(sid_authorization))
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
                objGlobaldata.AddFunctionalLog("Exception in PortalAuthorizationDelete: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetMasterPortalDetails(string id_portal_master)
        {           
            PortalModels objModel = new PortalModels();
            try
            {
                if (id_portal_master != "")
                {
                    string sqlstmt = "select activities,commercial_name from t_portal_master where id_portal_master='" + id_portal_master + "'";
                    DataSet dsData = objGlobaldata.Getdetails(sqlstmt);
                    if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        objModel.activities = dsData.Tables[0].Rows[0]["activities"].ToString();
                        objModel.commercial_name = dsData.Tables[0].Rows[0]["commercial_name"].ToString();
                    }
                }
                return Json(objModel);
            }
            catch(Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetMasterPortalDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("Failed");
        }

        public JsonResult FunGetEmpDetails(string semp_no)
        {

            NCModels objModels = new NCModels();
            try
            {
                string sSqlstmt = "select emp_no,emp_id,division,Emp_work_location,Dept_Id,EmailId,MobileNo,Date_of_Birth,Eid_no,Nationaliity from t_hr_employee where emp_no = '" + semp_no + "'";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    objModels = new NCModels
                    {
                        //emp_no = Convert.ToInt32(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        emp_id = (dsList.Tables[0].Rows[0]["emp_id"].ToString()),
                        empname = objGlobaldata.GetEmpHrNameById(dsList.Tables[0].Rows[0]["emp_no"].ToString()),
                        division = objGlobaldata.GetCompanyBranchNameById(dsList.Tables[0].Rows[0]["division"].ToString()),
                        location = objGlobaldata.GetDivisionLocationById(dsList.Tables[0].Rows[0]["Emp_work_location"].ToString()),
                        department = objGlobaldata.GetMultiDeptNameById(dsList.Tables[0].Rows[0]["Dept_Id"].ToString()),
                        EmailId = (dsList.Tables[0].Rows[0]["EmailId"].ToString()),
                        MobileNo = (dsList.Tables[0].Rows[0]["MobileNo"].ToString()),
                        Eid_no = (dsList.Tables[0].Rows[0]["Eid_no"].ToString()),
                        Date_of_Birth = (dsList.Tables[0].Rows[0]["Date_of_Birth"].ToString()),
                        Nationaliity = (dsList.Tables[0].Rows[0]["Nationaliity"].ToString()),
                        nc_division= dsList.Tables[0].Rows[0]["division"].ToString()
                    };
                }
                return Json(objModels);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }

        [AllowAnonymous]
        public ActionResult ApproveAuthRecommend(string id_authorization, int iStatus, string PendingFlg, string Comment)
        {
            try
            {
                PortalModels objPortal = new PortalModels();
                //string user = "";

                //user = objGlobaldata.GetCurrentUserSession().empid;

                string sStatus = "";
                if (iStatus == 0)
                {
                    sStatus = "Pending";
                }
                else if (iStatus == 1 || iStatus == 2 || iStatus == 3 || iStatus == 5)
                {
                    sStatus = "Approved";
                }
                else if (iStatus == 4)
                {
                    sStatus = "Rejected";
                }

                if (objPortal.FunApproveAuthRecommend(id_authorization, iStatus, Comment))
                {
                    TempData["Successdata"] = "Portal Authorization is " + sStatus;
                }
                else
                {
                    TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ApproveAuthRecommend: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }

            if (PendingFlg != null && PendingFlg == "true")
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
            else
            {
                return RedirectToAction("ListPendingForApproval", "Dashboard");
            }
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult ApproveAuthRecommend()
        //{
        //    PortalModels objPortal = new PortalModels();
        //    try
        //    {
        //        ViewBag.VP = objGlobaldata.GetVP();
        //        ViewBag.Chairman = objGlobaldata.GetChairman();
        //    }
        //    catch (Exception ex)
        //    {
        //        objGlobaldata.AddFunctionalLog("Exception in ApproveAuthRecommend: " + ex.ToString());
        //        TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
        //    }
        //    return View();
        //}
            
        public JsonResult ApproveAuthRecommendNoty(string id_authorization, int iStatus, string PendingFlg,string Comment)
        {
            try
            {
                PortalModels objPortal = new PortalModels();
                 
                if (objPortal.FunApproveAuthRecommend(id_authorization, iStatus, Comment))
                {
                    return Json("Success"+ iStatus);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ApproveAuthRecommendNoty: " + ex.ToString());
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

        public JsonResult ApproveAuthRecommendNotyCEO(string id_authorization, int iStatus, string PendingFlg, string Comment,string Approve_vp,string Approve_chairman)
        {
            try
            {
                PortalModels objPortal = new PortalModels();              

                if (objPortal.FunApproveAuthRecommendCEO(id_authorization, iStatus, Comment, Approve_vp, Approve_chairman))
                {
                    return Json("Success"+ iStatus);
                }
                else
                {
                    return Json("Failed");
                }
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in ApproveAuthRecommendNoty: " + ex.ToString());
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

        public JsonResult FunGetDivisionHODList(string DivisionId)
        {
            MultiSelectList HODList = objGlobaldata.GetDeptHeadbyDivisionList(DivisionId);
            return Json(HODList);
        }

        public JsonResult FunGetMinistryDetails(string portal_name)
        {
            PortalModels objModel = new PortalModels();
            try
            {
                string sSqlstmt = "select ministry_name,ministry_url from t_portal_ministry_name where id_portal_ministry_name = '" + portal_name + "'";
                DataSet dsList = objGlobaldata.Getdetails(sSqlstmt);
                if (dsList.Tables.Count > 0 && dsList.Tables[0].Rows.Count > 0)
                {
                    objModel = new PortalModels
                    {
                        ministry_name = dsList.Tables[0].Rows[0]["ministry_name"].ToString(),
                        ministry_url = dsList.Tables[0].Rows[0]["ministry_url"].ToString()
                    };                   
                }
                return Json(objModel);
            }
            catch (Exception ex)
            {
                objGlobaldata.AddFunctionalLog("Exception in FunGetEmpDetails: " + ex.ToString());
                TempData["alertdata"] = objGlobaldata.GetConstantValue("ExceptionError")[0];
            }
            return Json("");
        }
    }
}